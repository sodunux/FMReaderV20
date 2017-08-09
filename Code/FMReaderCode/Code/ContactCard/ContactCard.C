#include "includes.h"
#include "CCID.h"
#include "SpecialAPDU.h"
#include "ContactCard.h"
#include "smartcard.h"

/* ATR source Constants */
#define ATR_SOURCE_CARD   0x01
#define ATR_SOURCE_READER 0x02

/* bRecvResult Constants */
#define RECV_RESULT_OK            0x01
#define RECV_RESULT_ATR_TS_ERROR  0x03
#define RECV_RESULT_ATR_TCK_ERROR 0x04
#define RECV_RESULT_APDU_INS_XOR  0x20
#define RECV_RESULT_APDU_INS_ERROR  0x22

															 
/* bAPDUCase Constants */
#define APDU_CASE_1  0x01			//LC=0,LE=0
#define APDU_CASE_2  0x02			//LC=0,LE!=0	
#define APDU_CASE_3  0x03			//LC!=0,LE=0	
#define APDU_CASE_4  0x04			//LC!=0,LE!=0
#define APDU_CASE_34 0x05			//APDU_CASE_3 or APDU_CASE_4

static u32 F_Table[16] = {0, 372, 558, 744, 1116, 1488, 1860, 0,
                          0, 512, 768, 1024, 1536, 2048, 0, 0};
static u32 D_Table[8] = {0, 1, 2, 4, 8, 16, 0, 0};

uint8_t 	bATRSource;   				// ATR comes from card or reader.
uint8_t 	volatile bT0Status;   // Reader will receive bytes or send bytes. 	
uint8_t 	bTA1;         				// Communication rate parameter.							ATR-TA1
uint8_t 	bTC1;         				// Extra Guard Time.													ATR-TC1
uint8_t 	bTC2;         				// Work Waiting Time Integer.									ATR-TC2
uint8_t 	bProtocolT;   				// T=0 or T=1.																ATR-T0,ATR-T1
uint32_t	lWwtInEtu;  				// Work waiting time using ETU as unit.				

uint8_t volatile bIsTimeout;    // TDA8035 Timer is overflow or not.
uint8_t volatile bIsParityErr;  // TDA8035 received enough parity error.		
uint8_t volatile bRecvResult;   // Result of receiving bytes from card.			

uint8_t bTAPos, bTBPos, bTCPos, bTDPos;   // TAi, TBi, TCi and TDi position in ATR.
uint8_t bATRCyc;      // Index value of TA, TB, TC and TD.
uint8_t bAPDUCase;    // Case 1, 2, 3 
uint8_t bINS, bP3;    // INS and P3 of APDU Header

extern bool bIsCTon;
	
/*******************************************************************************
* Function Name  : ContactCardPPS.
* Description    : Contact card PPS process.
* Input          : PPS1  : Equal to TA1 byte of ATR.
* Output         : None
* Return         : CT_T0_OK, 
                   CT_T0_PPS_TIMEOUT
*******************************************************************************/
uint8_t ContactCardPPS(uint8_t PPS1)
{
		u8 res;
		res=SC_PPS(PPS1);
		if(res)
			return CT_T0_OK;
		else
     return CT_T0_PPS_TIMEOUT;
}


/*******************************************************************************
* Function Name  : ContactCardInitCmd.
* Description    : Contact card Init APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.
                   APDURecvLen  : How many bytes received from card.
* Output         : None
* Return         : CT_T0_OK,
                   CT_T0_APDU_LEN_ERROR,                   
*******************************************************************************/

uint8_t ContactCardInitCmd(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	static uint8_t InitCmd00Doing=0;
	static uint8_t InitCmd55Doing=0;
	static uint8_t InitCmd54Doing=0;
	static uint8_t InitCmdC0Flag=0; //预期有C0指令标识
	static uint8_t C0_Buffer[280]; 	//C0指令
	uint8_t 	SCData;
	uint16_t b;
	uint8_t 	bCLA =	APDUBuffer[0];
	uint8_t   bINS =  APDUBuffer[1];
  uint8_t   bP1 = 	APDUBuffer[2];
  uint8_t   bP2 = 	APDUBuffer[3];
  uint16_t  bP3 =   APDUBuffer[4];
	
	RCC_ClocksTypeDef RCC_ClocksStatus;
	USART_InitTypeDef USART_InitStructure;
	u32 workingbaudrate = 0, apbclock = 0;
	
	
	if(InitCmd00Doing && (!InitCmdC0Flag)) bINS=0x00; //InitCMD 00 Step2
	if(InitCmd55Doing && (!InitCmdC0Flag)) bINS=0x55; //InitCMD 55 Step2
	if(InitCmd55Doing && (!InitCmdC0Flag)) bINS=0x54; //InitCMD 54 Step2
	
	switch(bINS)
	{	
		case 0x3C:
		case 0x00:
			//Step1
			if(!InitCmd00Doing)
			{
			SCData = bCLA;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bINS;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP1;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP2;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP3;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 					
			/* Flush the USART3 DR */
			(void)USART_ReceiveData(USART3);
			
			if((USART_ByteReceive(&SCData, SC_Receive_Timeout)) == SUCCESS)
			{
				if(SCData==bINS)
					{
						C0_Buffer[0]=0x00;					
						APDUBuffer[0]=0x61;
						APDUBuffer[1]=0x01;
						*APDURecvLen=2;
						InitCmd00Doing=1;
						InitCmdC0Flag=1; //00指令的C0代码 Step1
					}
			}
			else
				return CT_T0_APDU_TIMEOUT;
			
			}
			//Step2
			else 
			{								
				for(b=0;b<128;b++)
				{
					SCData =APDUBuffer[b];
					USART_SendData(USART3, SCData);
					while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){}
				}
				/* Flush the USART3 DR */
				(void)USART_ReceiveData(USART3);
				
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout*10)) == SUCCESS)
				{
					C0_Buffer[0]=SCData;
					C0_Buffer[1]=0x00;					
					APDUBuffer[0]=0x61;
					APDUBuffer[1]=0x02;										
					*APDURecvLen=2;
					InitCmdC0Flag=2;//第二次C0
				}
				else 
					{
						InitCmd00Doing=0;
						InitCmdC0Flag=0;
						return CT_T0_APDU_TIMEOUT;
						
					}
			}				
		break;
			
		case 0x01:
			SCData = bCLA;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bINS;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP1;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP2;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP3;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 					
			/* Flush the USART3 DR */
			(void)USART_ReceiveData(USART3);
			
			for(b=0;b<2;b++)
			{
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout*10)) == SUCCESS)
				{
					C0_Buffer[b]=SCData;
					
				}					
			}
			if(C0_Buffer[0]!=0x90)
			{
				return CT_T0_APDU_TIMEOUT;
			}

			if(bP1&&(C0_Buffer[0]==0x90)&&(C0_Buffer[1]==0x00))
			{
				RCC_GetClocksFreq(&RCC_ClocksStatus);
				apbclock = RCC_ClocksStatus.PCLK1_Frequency;
				apbclock /= ((USART3->GTPR & (u16)0x00FF) * 2);
				USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);
				workingbaudrate = apbclock * D_Table[(bP1 & (u8)0x0F)];
				workingbaudrate /= F_Table[((bP1 >> 4) & (u8)0x0F)];
				USART_StructInit(&USART_InitStructure);
				USART_InitStructure.USART_BaudRate = workingbaudrate;
				USART_InitStructure.USART_WordLength = USART_WordLength_9b;
				USART_InitStructure.USART_StopBits = USART_StopBits_1_5;
				USART_InitStructure.USART_Parity = USART_Parity_Even;
				USART_InitStructure.USART_Clock = USART_Clock_Enable;
				USART_Init(USART3, &USART_InitStructure);
			}			
			APDUBuffer[0]=0x61;
			APDUBuffer[1]=0x02;
			*APDURecvLen=2;
			InitCmdC0Flag=0xff;
		break;
			

		case 0x54:
			//Step1
			if(!InitCmd54Doing)
			{
				SCData = bCLA;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bINS;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bP1;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bP2;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bP3;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 					
				/* Flush the USART3 DR */
				(void)USART_ReceiveData(USART3);
			
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout)) == SUCCESS)
				{
					if(SCData==bINS)
					{
						C0_Buffer[0]=bINS;						
						APDUBuffer[0]=0x61;
						APDUBuffer[1]=0x01;
						*APDURecvLen=2;						
						InitCmdC0Flag=0x55;
						InitCmd54Doing=1;
					}
				}
				else  
					return CT_T0_APDU_TIMEOUT;
			}
			//Step2
			else 
			{							
				for(b=0;b<16;b++)
				{
					SCData =APDUBuffer[b];
					USART_SendData(USART3, SCData);
					while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){}
				}
				/* Flush the USART3 DR */
				(void)USART_ReceiveData(USART3);
				
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout)) == SUCCESS)
				{
					C0_Buffer[0]=SCData;
					C0_Buffer[1]=0x00;
					InitCmdC0Flag=0x56;					
					APDUBuffer[0]=0x61;
					APDUBuffer[1]=0x02;
					*APDURecvLen=2;
				}
			}	
		break;
		
		case 0x55:
			//Step1
			if(!InitCmd55Doing)
			{
				SCData = bCLA;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bINS;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bP1;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bP2;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
				SCData = bP3;
				USART_SendData(USART3, SCData);
				while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 					
				/* Flush the USART3 DR */
				(void)USART_ReceiveData(USART3);
			
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout)) == SUCCESS)
				{
					if(SCData==bINS)
					{
						C0_Buffer[0]=bINS;						
						APDUBuffer[0]=0x61;
						APDUBuffer[1]=0x01;
						*APDURecvLen=2;						
						InitCmdC0Flag=0x55;
						InitCmd55Doing=1;
					}
					else 
						return CT_T0_APDU_TIMEOUT;
				}
			}
			//Step2
			else 
			{							
				for(b=0;b<16;b++)
				{
					SCData =APDUBuffer[b];
					USART_SendData(USART3, SCData);
					while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){}
				}
				/* Flush the USART3 DR */
				(void)USART_ReceiveData(USART3);
				
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout)) == SUCCESS)
				{
					C0_Buffer[0]=SCData;
					C0_Buffer[1]=0x00;
					InitCmdC0Flag=0x56;					
					APDUBuffer[0]=0x61;
					APDUBuffer[1]=0x02;
					*APDURecvLen=2;
				}
			}	
		break;
			
		case 0x04: 
			apdu_commands.Header.CLA=bCLA;
			apdu_commands.Header.INS=bINS;
			apdu_commands.Header.P1=bP1;
			apdu_commands.Header.P2=bP2;
			if(!bP3)
				apdu_commands.Body.LE=0x100;
			else 
				apdu_commands.Body.LE=bP3;
			state=SC_ACTIVE_ON_T0;
			SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_ACTIVE_ON_T0	
			
			if(((apdu_responce.SW1&0x60)!=0x60)&&((apdu_responce.SW1&0x90)!=0x90))
				return CT_T0_APDU_TIMEOUT;
			
			C0_Buffer[0]=bINS;
			//if()
			for(b=0;b<apdu_commands.Body.LE;b++)
			{
				C0_Buffer[b+1]=apdu_responce.Data[b];
			}
			C0_Buffer[b+1]=apdu_responce.SW1;
			C0_Buffer[b+2]=apdu_responce.SW2;
		
			APDUBuffer[0]=0x61;
			APDUBuffer[1]=apdu_commands.Body.LE;
			*APDURecvLen=2;
			InitCmdC0Flag=0x04;
		break;
		
		case 0x0A:
			SCData = bCLA;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bINS;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP1;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP2;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP3;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 					
			/* Flush the USART3 DR */
			(void)USART_ReceiveData(USART3);
			
			for(b=0;b<5;b++)
			{
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout<<8)) == SUCCESS)
				{
					C0_Buffer[b]=SCData;
				}
				else
					return CT_T0_APDU_TIMEOUT;
					
			}
			APDUBuffer[0]=0x61;
			APDUBuffer[1]=0x05;		
			*APDURecvLen=0x02;	
			InitCmdC0Flag=0x0A;
			
		break;
		
		//case c0
		case 0xc0:
			switch(InitCmdC0Flag)
			{
				case 0x01:   //00指令第一次C0
					APDUBuffer[0]=C0_Buffer[0];
					APDUBuffer[1]	=	0x90;
					APDUBuffer[2]	=	0x00;
					*APDURecvLen=3;
				break;
				
				case 0x02:   //00指令第二次C0
					APDUBuffer[0]=C0_Buffer[0];
					APDUBuffer[1]=C0_Buffer[1];
					APDUBuffer[2]	=	0x90;
					APDUBuffer[3]	=	0x00;
					*APDURecvLen=4;
					InitCmd00Doing=0;
				break;
				
				case 0x04:   //04指令的C0指令
					if(!bP3) bP3=0x100;						
					for(b=0;b<(bP3+3);b++)
					{
						APDUBuffer[b]=C0_Buffer[b];
					}			
					APDUBuffer[b]		=	0x90;
					APDUBuffer[b+1]	=	0x00;
					*APDURecvLen=bP3+5;
				break;
				
				case 0x0A:	//0A指令的C0指令
				for(b=0;b<5;b++)
					{
						APDUBuffer[b]=C0_Buffer[b];
					}			
					APDUBuffer[b]		=	0x90;
					APDUBuffer[b+1]	=	0x00;
					*APDURecvLen=7;
					break;
					
				case 0x55:  //54,55指令的C0指令 Step1
					APDUBuffer[0]=C0_Buffer[0];
					APDUBuffer[1]	=	0x90;
					APDUBuffer[2]	=	0x00;
					*APDURecvLen=3;				
				break;
				
				case 0x56: //54,55指令的C0指令 Step2
					APDUBuffer[0]=C0_Buffer[0];
					APDUBuffer[1]=C0_Buffer[1];
					APDUBuffer[2]	=	0x90;
					APDUBuffer[3]	=	0x00;
					*APDURecvLen=0x04;
					InitCmd55Doing=0;
					InitCmd54Doing=0;
				break;
						
				case 0xff: //默认的C0
					APDUBuffer[0]=C0_Buffer[0];
					APDUBuffer[1]=C0_Buffer[1];
					APDUBuffer[2]	=	0x90;
					APDUBuffer[3]	=	0x00;
					*APDURecvLen=0x04;//0x90009000
					break;
				
				default:
					*APDURecvLen=0;			
			}
			
			InitCmdC0Flag=0;
		break;
			
		default:
			SCData = bCLA;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bINS;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP1;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP2;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 
			SCData = bP3;
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 					
			/* Flush the USART3 DR */
			(void)USART_ReceiveData(USART3);
			
			for(b=0;b<2;b++)
			{
				if((USART_ByteReceive(&SCData, SC_Receive_Timeout*10)) == SUCCESS)
				{
					C0_Buffer[b]=SCData;
				}
				else
					return CT_T0_APDU_TIMEOUT;			
			}	
			APDUBuffer[0]=0x61;
			APDUBuffer[1]=0x02;
			*APDURecvLen=2;
			InitCmdC0Flag=0xff;
			break;	
	}	
	return CT_T0_OK;	
}


/*******************************************************************************
* Function Name  : ContactCardT0APDU.
* Description    : Contact card APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.
                   APDURecvLen  : How many bytes received from card.
* Output         : None
* Return         : CT_T0_OK,
                   CT_T0_APDU_LEN_ERROR,                   
*******************************************************************************/
uint8_t ContactCardT0APDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	u8 APDU_OK_Flag=1;
	u8 b;
	if(bIsCTon == FALSE)
  {
  	return CT_T0_APDU_TIMEOUT;
  }
/*Get APDU header*/
	if(APDUSendLen < 4)
    return CT_T0_APDU_LEN_ERROR;
	
	if(APDUSendLen == 4)		
  {	
		bAPDUCase=APDU_CASE_1; //LC=0,LE=0;
		apdu_commands.Header.CLA=APDUBuffer[0];
		apdu_commands.Header.INS=APDUBuffer[1];
		apdu_commands.Header.P1	=APDUBuffer[2];
		apdu_commands.Header.P2	=APDUBuffer[3];
		apdu_commands.Body.LC=0;
		apdu_commands.Body.LE=0;
  }
  
  if(APDUSendLen==5) 
  {
		bAPDUCase=APDU_CASE_2;  //LC=0,LE!=0; 
    apdu_commands.Header.CLA=APDUBuffer[0];
		apdu_commands.Header.INS=APDUBuffer[1];
		apdu_commands.Header.P1=APDUBuffer[2];
		apdu_commands.Header.P2=APDUBuffer[3];	
		apdu_commands.Body.LE=APDUBuffer[4];
		apdu_commands.Body.LC=0; 
  }
	
  if(APDUSendLen>5)
  {
		bAPDUCase=APDU_CASE_34; //LC!=0
    apdu_commands.Header.CLA=APDUBuffer[0];
		apdu_commands.Header.INS=APDUBuffer[1];
		apdu_commands.Header.P1=APDUBuffer[2];
		apdu_commands.Header.P2=APDUBuffer[3];	
		apdu_commands.Body.LC=APDUBuffer[4];	
		
		if(apdu_commands.Body.LC==(APDUSendLen-5)) 
		{
			for(b=0;b<(APDUSendLen-5);b++)
			{
				apdu_commands.Body.Data[b]=APDUBuffer[5+b];
			}	
			apdu_commands.Body.LE=0;
		}
		
		else if(apdu_commands.Body.LC==(APDUSendLen-6))
		{
			for(b=0;b<(APDUSendLen-6);b++)
			{
				apdu_commands.Body.Data[b]=APDUBuffer[5+b];
			}	
			apdu_commands.Body.LE=APDUBuffer[5+b];
		}
		else
			APDU_OK_Flag=0;
  }
	
  
    /* Send APDU header */
	if((state==SC_ACTIVE_ON_T0)&&APDU_OK_Flag)
		SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_ACTIVE_ON_T0
	else 
		{
			APDUBuffer[0]=0x6B;
			APDUBuffer[1]=0x00;
			*APDURecvLen=2;
			return CT_T0_APDU_TIMEOUT;
		}
		
	if(((apdu_responce.SW1&0x60)!=0x60)&&((apdu_responce.SW1&0x90)!=0x90))
		return CT_T0_APDU_TIMEOUT;
		
	switch(bAPDUCase)
	{
		case APDU_CASE_1:
				APDUBuffer[0]=apdu_responce.SW1;
				APDUBuffer[1]=apdu_responce.SW2;
				*APDURecvLen=2;
				break;		
		case APDU_CASE_2:
				for(b=0;b<apdu_commands.Body.LE;b++)
				{
					APDUBuffer[b]	=	apdu_responce.Data[b];
				}
				APDUBuffer[b]		=	apdu_responce.SW1;
				APDUBuffer[b+1]	=	apdu_responce.SW2;
				*APDURecvLen=b+2;
				break;
		
		case APDU_CASE_34:
				APDUBuffer[0]=apdu_responce.SW1;
				APDUBuffer[1]=apdu_responce.SW2;
				*APDURecvLen=2;
				break;
		
		default:
				APDUBuffer[0]=apdu_responce.SW1;
				APDUBuffer[1]=apdu_responce.SW2;
				*APDURecvLen=2;
				break;	
	}
				
		return CT_T0_OK;
}


/*******************************************************************************
* Function Name  : ContactCardInterruptHandler.
* Description    : Contact card interrupt handler.
* Input          : None 
* Output         : None
* Return         : None
*******************************************************************************/
void ContactCardInterruptHandler(void)
{
	
}
