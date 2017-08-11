#include "includes.h"
#include "CCID.h"
#include "SpecialAPDU.h" 
//#include "SPI_IIC.h" 
#include "timer1.h"
#include "FM320.h"
#include "smartcard.h"

#ifdef CONTACT_CARD_ENABLE
  #include "ContactCard.h"
#endif
#ifdef CONTACTLESS_CARD_ENABLE
  #include "PICCCmdConst.h"
  #include "ContactlessCard.h"
  #include "MfErrNo.h"
#endif                

#define INDEX_INS 0x01
#define INDEX_P1  0x02
#define INDEX_P2  0x03
#define INDEX_P3  0x04
#define INDEX_DATA  0x05

InternalRegister stuInRegs;
uint8_t InitCmd04Flag=0;
uint8_t InitCmd00Flag=0;
uint8_t InitCmdFlag=0;					//0:APDU,1:Init
extern uint16_t const FrameSize[9];
extern bool bTimeOut;
/*******************************************************************************
* Function Name  : InRegsInit.
* Description    : Initialize member of stuInRegs.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void InRegsInit(void)
{
  stuInRegs.bCurrentDevice = CURRENT_DEVICE_CT;
}

/*******************************************************************************
* Function Name  : SetStatusWords.
* Description    : Put status words into array.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   SW : Two bytes status words.              
* Output         : None
* Return         : None                   
*******************************************************************************/
void SetStatusWords(uint8_t *APDUBuffer, uint16_t SW)
{
  *APDUBuffer = (uint8_t)((SW>>8)&0xFF);
  *(APDUBuffer+1) = (uint8_t)(SW&0xFF);
}

/*******************************************************************************
* Function Name  : TIM3Config.
* Description    : Config the TIM3 of MCU used for delay and width timer.
* Input          : ClkFrq : Clock frequency.
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3Config(void)
{

}

/*******************************************************************************
* Function Name  : TIM3Start.
* Description    : Start the TIM3 of MCU used for delay and width timer.
* Input          : None.
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3Start(void)
{
  /* TIM IT enable */
  TIM_ITConfig(TIM3, TIM_IT_CC1, ENABLE);

  /* TIM2 enable counter */
  TIM_Cmd(TIM3, ENABLE);
}

uint8_t bTimeoutOpen;
/*******************************************************************************
* Function Name  : SpecialAPDU.
* Description    : Special APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.
                   APDURecvLen  : How many bytes received from card.
* Output         : None
* Return         : None                   
*******************************************************************************/
void SpecialAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
  uint8_t   bINS = *(APDUBuffer+INDEX_INS);
  uint8_t   bP1 =  *(APDUBuffer+INDEX_P1);
  uint8_t   bP2 =  *(APDUBuffer+INDEX_P2);
  uint16_t  bP3 =  *(APDUBuffer+INDEX_P3);		
  uint8_t   CMD1 = (*(APDUBuffer+INDEX_DATA));
  uint8_t   CMD2 = (*(APDUBuffer+INDEX_DATA+1));
//  uint8_t   CMD3 = (*(APDUBuffer+INDEX_DATA+2));
//  uint8_t   CMD4 = (*(APDUBuffer+INDEX_DATA+3));
//  uint8_t   CMD5 = (*(APDUBuffer+INDEX_DATA+4));
	
	uint32_t 	param=0; //P1+P2+P3 
	NFC_DataExTypeDef NFC_DataExStruct;
  uint8_t b,res;
	uint32_t cnt;
	
	//************** end of smartcard ****************
	param	=	(bP1<<16)|(bP2<<8)|bP3;
	switch(bINS)   // INS
  {
  case INS_SELECT_DEVICE:
		*APDURecvLen = 0;
    if(bP3 == 0x01)
    {
      if((bP1 == 0x00) && (bP2 == 0x00))
      {
        b = (*(APDUBuffer+5));        
        if((b == 0x11) || (b == 0x21) || (b == 0x31) || (b == CURRENT_DEVICE_CL))
        {                                                        
          stuInRegs.bCurrentDevice = (b&0x0F);
          if(b&0x01)  // Contact card.
          {
            CLCardPowerOff();
            bTimeoutOpen = 0;   // Suppose card will be inserted into slot again.
          }
					else
					{
							// 
					}
          SetStatusWords(APDUBuffer, 0x9000);
        }
        else
          SetStatusWords(APDUBuffer, 0x6F00);
      }
      else
        SetStatusWords(APDUBuffer, 0x6B00);
    }
    else
      SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;
    break;
  
  case INS_DIRECT_READ_REGISTER:
			FM320_GetReg(bP2,&res);
			*APDUBuffer	= res;
			SetStatusWords(APDUBuffer+1, 0x9000);
			*APDURecvLen = 3;
		break;
	
	case INS_DIRECT_WRITE_REGISTER:
			FM320_SetReg(bP2,CMD1);
			SetStatusWords(APDUBuffer, 0x9000);
			*APDURecvLen =2;
		break;
		
  case INS_CL_OPERATION:			
		*APDURecvLen=0;
		switch(param)
		{
			//MI_FieldON  or MI_Field OFF    
			case 0x010001:
				*APDURecvLen = 0;
				if(CMD1)
					{
						FM320_POWERON;
//						FM320_NRSTPD_CLR;
//						for(cnt=0;cnt<10000;cnt++);
//						FM320_NRSTPD_SET;
						CLCardInit();
						FM320_Initial_ReaderA();
					}
				else
					FM320_POWEROFF;				
				SetStatusWords(APDUBuffer, 0x9000);	
				*APDURecvLen = 2;	
				break;
			
			//ReqA
			case 0x010102 : 
				*APDURecvLen = 0;
				//FM320_POWERON;
				//CLCardInit();
				//FM320_Initial_ReaderA();
				res=ReaderA_Request(RF_CMD_REQA);
				if(!res)
				{
					*APDUBuffer			=	CardA_Sel_Res.ATQA[0];
					*(APDUBuffer+1)	=	CardA_Sel_Res.ATQA[1];
					SetStatusWords(APDUBuffer+2, 0x9000);	
					*APDURecvLen = 4;
				}
					else
				{
					*APDUBuffer=0x6D;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;
				}
				InitCmdFlag=0;
				break;
			
			//WupA
			case 0x010A02:
				*APDURecvLen = 0;
				CLCardInit();
				res=ReaderA_Request(RF_CMD_WUPA);
				if(!res)
				{
					*APDUBuffer			=	CardA_Sel_Res.ATQA[0];
					*(APDUBuffer+1)	=	CardA_Sel_Res.ATQA[1];
					SetStatusWords(APDUBuffer+2, 0x9000);	
					*APDURecvLen = 4;
				}
				else
				{
					*APDUBuffer=0x6D;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;
				}			
				break;		
			
			//coll level 1
			case 0x010206	:
				*APDURecvLen = 0;
				res=ReaderA_AntiCol(0);
				if(!res)
				{
					*(APDUBuffer+4)=0;
					for(b=0;b<4;b++)
					{
						*(APDUBuffer+b)	=	CardA_Sel_Res.UID[b];
						*(APDUBuffer+4)^= CardA_Sel_Res.UID[b];
					}				
					SetStatusWords(APDUBuffer+5, 0x9000);
					*APDURecvLen = 7;
				}
				else 
				{
					*APDUBuffer=0x6D;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;
				}
				break;
				
			//SELECT LEVEL 1
			case 0x010301	:
				*APDURecvLen = 0;
				res=ReaderA_Select(0);
				if(!res)
				{
				(*APDUBuffer)=CardA_Sel_Res.SAK;
				SetStatusWords(APDUBuffer+1, 0x9000);
				*APDURecvLen = 3;
				}
				else
				{
					*APDUBuffer=0x6D;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;
				}
				break;
			
			//RATS
			case 0x010401:
				*APDURecvLen = 0;
				res=ReaderA_Rats(8,1);
				if(!res)
				{
					stuTCLParam.bBlockNum=0;
					if(!CardA_Sel_Res.ATSLEN)
					{
						*APDUBuffer=0x6D;
						*(APDUBuffer+1)	=0x00;
						*APDURecvLen=2;
					}
					else
					{
						for(b=0;b<CardA_Sel_Res.ATSLEN;b++)
						{
							*(APDUBuffer+b) = CardA_Sel_Res.ATS[b];
						}
							SetStatusWords(APDUBuffer+CardA_Sel_Res.ATSLEN, 0x9000);
							*APDURecvLen = CardA_Sel_Res.ATSLEN+2;
					}
				}
				else
				{
					*APDUBuffer=0x6D;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;
				}
				break;
				
			//MFAuth
			case 0x010E01:
				*APDURecvLen = 0;
				Write_FIFO(APDUSendLen-5,APDUBuffer+5);
				FM320_SetReg(COMMAND,CMD_AUTHENT);
				//Read MFCrypto1ON BIT
				FM320_GetReg(STATUS2,&res);
				cnt=10000;
				while(!(res&MFCrypto1On))
				{
					FM320_GetReg(STATUS2,&res);
					cnt--;
					if(!cnt) break;
				}				
				if(res&MFCrypto1On)
				{
					SetStatusWords(APDUBuffer, 0x9000);	
					*APDURecvLen = 2;	
				}
				else 
				{
					SetStatusWords(APDUBuffer, 0x6700);	
					*APDURecvLen = 2;	
				}
			break;
			
			//MI_RDBLOCK
			case 0x010501:
				*APDURecvLen = 0;
				APDUBuffer[0]=0x30;
				APDUBuffer[1]=CMD1;
				NFC_DataExStruct.pExBuf =	APDUBuffer;
				NFC_DataExStruct.nBytesToSend	  =	0x02;
				NFC_DataExStruct.nBytesReceived = 0x00;
				Command_Transceive(&NFC_DataExStruct);
				if(!NFC_DataExStruct.nBytesReceived)
				{
					SetStatusWords(APDUBuffer, 0x6700);	
					*APDURecvLen = 2;	
				}
				else
				{
					SetStatusWords(APDUBuffer+NFC_DataExStruct.nBytesReceived, 0x9000);
					*APDURecvLen = NFC_DataExStruct.nBytesReceived+2;
				}
				break;
				
				//MI_WriteBlock
			case 0x010611:
				*APDURecvLen = 0;
				//part A
				APDUBuffer[0]=0xA0;
				APDUBuffer[1]=CMD1;
				NFC_DataExStruct.pExBuf =	APDUBuffer;
				NFC_DataExStruct.nBytesToSend	  =	0x02;
				NFC_DataExStruct.nBytesReceived = 0x00;
				Command_Transceive(&NFC_DataExStruct);
				//part B
				if(!NFC_DataExStruct.nBytesReceived) //NACK
				{
					SetStatusWords(APDUBuffer, 0x6700);	
					*APDURecvLen = 2;	
				}
				else
				{
					NFC_DataExStruct.pExBuf 				=	APDUBuffer+6;
					NFC_DataExStruct.nBytesToSend	  =	0x10;
					NFC_DataExStruct.nBytesReceived = 0x00;
					Command_Transceive(&NFC_DataExStruct);
					if(NFC_DataExStruct.pExBuf[0]!=0x0A)
					{
						SetStatusWords(APDUBuffer, 0x6700);	
						*APDURecvLen = 2;	
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x9000);
						*APDURecvLen =2;
					}
				}
			break;
		
			//MI_Restore
			case 0x011301:		
			//MI_DECBLOCK
			case 0x011205:
			//MI_INCBLOCK
			case 0x011105:
				*APDURecvLen = 0;
				//part A
				APDUBuffer[0]=0xC1;
				APDUBuffer[1]=CMD1;
				NFC_DataExStruct.pExBuf =	APDUBuffer;
				NFC_DataExStruct.nBytesToSend	  =	0x02;
				NFC_DataExStruct.nBytesReceived = 0x00;
				Command_Transceive(&NFC_DataExStruct);

				if(!NFC_DataExStruct.nBytesReceived) //NACK
				{
					SetStatusWords(APDUBuffer, 0x6700);	
					*APDURecvLen = 2;	
				}
				//part B
				else
				{
					NFC_DataExStruct.pExBuf 				=	APDUBuffer+6;
					NFC_DataExStruct.nBytesToSend	  =	0x04;
					NFC_DataExStruct.nBytesReceived = 0x00;
					Command_Transceive(&NFC_DataExStruct);
					if(!NFC_DataExStruct.nBytesReceived)
					{
						SetStatusWords(APDUBuffer, 0x6700);	
						*APDURecvLen = 2;	
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x9000);
						*APDURecvLen =2;
					}
				}			
			break;
			
			//MI_Transfer
			case 0x011401:
				*APDURecvLen = 0;
				APDUBuffer[0]=0xB0;
				APDUBuffer[1]=CMD1;
				NFC_DataExStruct.pExBuf =	APDUBuffer;
				NFC_DataExStruct.nBytesToSend	  =	0x02;
				NFC_DataExStruct.nBytesReceived = 0x00;
				Command_Transceive(&NFC_DataExStruct);
				if(NFC_DataExStruct.pExBuf[0]!=0x0A)
				{
					SetStatusWords(APDUBuffer, 0x6700);	
					*APDURecvLen = 2;	
				}
				else
				{
					SetStatusWords(APDUBuffer+NFC_DataExStruct.nBytesReceived, 0x9000);
					*APDURecvLen = NFC_DataExStruct.nBytesReceived+2;
				}
			break;
			
				
			default:
					*APDURecvLen = 0;
					SetStatusWords(APDUBuffer, 0x9000);	
					*APDURecvLen = 2;	
				break;
		
		}
		
		break;
	
  case INS_CT_OPERATION:
		*APDURecvLen = 0;
		switch(param)
		{
			//Init TDA
			case 0x010001 :
						*APDURecvLen = 0;
						SC_PowerCmd(DISABLE);
						state=SC_POWER_ON;
						apdu_commands.Header.CLA = 0x00;
						apdu_commands.Header.INS = SC_GET_A2R;
						apdu_commands.Header.P1 = 0x00;
						apdu_commands.Header.P2 = 0x00;
						apdu_commands.Body.LC = 0x00;
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_RESET_LOW
						SC_PowerCmd(DISABLE);
						SetStatusWords(APDUBuffer, 0x9000);				
						*APDURecvLen = 2;			
						break;
			//Cold  Reset
			
			case 0x010101	:	
						*APDURecvLen = 0;
						SC_PowerCmd(DISABLE);
						state=SC_POWER_ON;
						apdu_commands.Header.CLA = 0x00;
						apdu_commands.Header.INS = SC_GET_A2R;
						apdu_commands.Header.P1 = 0x00;
						apdu_commands.Header.P2 = 0x00;
						apdu_commands.Body.LC = 0x00;
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_RESET_LOW
						//config the voltage 
						if(CMD1 == CARD_VCC_5V)
							SC_VoltageConfig(SC_Voltage_5V);
						else if(CMD1 == CARD_VCC_18V)
							SC_VoltageConfig(SC_Voltage_18V);
						else 
							SC_VoltageConfig(SC_Voltage_3V);
						
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_ACTIVE,ATR
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_ACTIVE_ON_T0 or SC_POWER_OFF													

						if(state==SC_ACTIVE_ON_T0)
						{
							for(b=0;b<SC_A2R.Tlength+SC_A2R.Hlength+2;b++)
							{
								APDUBuffer[b]=SC_ATR_Table[b];
							}
							SetStatusWords(APDUBuffer+b, 0x9000);							
							*APDURecvLen = b+2;
							bIsCTon = (bool)TRUE;
							InitCmdFlag=0;	//初始化指令标志位
							InitCmd00Flag=0;
							InitCmd04Flag=0;
						}
						else 
						{
							SetStatusWords(APDUBuffer, 0x6D00);							
							*APDURecvLen = 2;							
						}					
						break;
						
			//warmreset
			case 0x010201:
						*APDURecvLen = 0;
						SC_PowerCmd(DISABLE);
						state=SC_POWER_ON;
						apdu_commands.Header.CLA = 0x00;
						apdu_commands.Header.INS = SC_GET_A2R;
						apdu_commands.Header.P1 = 0x00;
						apdu_commands.Header.P2 = 0x00;
						apdu_commands.Body.LC = 0x00;
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_RESET_LOW
						//config the voltage 
						if(CMD1 == CARD_VCC_5V)
							SC_VoltageConfig(SC_Voltage_5V);
						else if(CMD1 == CARD_VCC_18V)
							SC_VoltageConfig(SC_Voltage_18V);
						else 
							SC_VoltageConfig(SC_Voltage_3V);
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_ACTIVE,ATR
						SC_Handler(&state,&apdu_commands,&apdu_responce);//state=SC_ACTIVE_ON_T0 or SC_POWER_OFF													
						if(state==SC_ACTIVE_ON_T0)
						{
							for(b=0;b<SC_A2R.Tlength+SC_A2R.Hlength+2;b++)
							{
								APDUBuffer[b]=SC_ATR_Table[b];
							}
							SetStatusWords(APDUBuffer+b, 0x9000);							
							*APDURecvLen = b+2;
							bIsCTon = (bool)TRUE;	
							InitCmdFlag=0;
						}
						else 
						{
							SetStatusWords(APDUBuffer, 0x6D00);							
							*APDURecvLen = 2;							
						}					
						break;
			default:
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x9000);	
						*APDURecvLen = 2;
						break;
		}
		break;					
	case INS_DATA_TRANSMIT:
		*APDURecvLen = 0;
		//TypeA Select
		if(param==0x010102)
		{
			FM320_POWERON;
			CLCardInit();
			FM320_Initial_ReaderA();
			SetStatusWords(APDUBuffer, 0x9000);	
			*APDURecvLen = 2;	
		}
		
		//TypeB Select
		if(param==0x010B03)
		{
			FM320_POWERON;
			CLCardInit();
			FM320_Initial_ReaderB();
			SetStatusWords(APDUBuffer, 0x9000);	
			*APDURecvLen = 2;	
		}
		
		//ContactCard
		if(stuInRegs.bCurrentDevice==CURRENT_DEVICE_CT)	
		{
			if(CMD2==0x01) //初始化01指令
			{
				InitCmdFlag=1;
				res=ContactCardInitCmd(APDUBuffer+5,APDUBuffer[4],APDURecvLen);
				if(res!=CT_T0_OK)
				{
					SetStatusWords(APDUBuffer, 0x6D00);
					*APDURecvLen = 2;
				}
			}
			else if(InitCmdFlag)
			{
				res=ContactCardInitCmd(APDUBuffer+5,APDUBuffer[4],APDURecvLen);
				if(res!=CT_T0_OK)
				{
					SetStatusWords(APDUBuffer, 0x6D00);
					*APDURecvLen = 2;
				}
				
			}
			else
				//bP3 is len
			{
				res=ContactCardT0APDU(APDUBuffer+5,APDUBuffer[4],APDURecvLen);
				if(res!=CT_T0_OK)
					SetStatusWords(APDUBuffer+5, 0x6D00);	
			}
			
			for(b=0;b<*APDURecvLen;b++)
			{
				APDUBuffer[b]=APDUBuffer[b+5];
			}
		}
		
		//ContactlessCard
		if(stuInRegs.bCurrentDevice==CURRENT_DEVICE_CL)
		{
			//CRC and parity
			//PS:FM320 parity can be disable and enable,but can't be select odd or even
			
			//No Parity 
			if((bP1&0xF0)==0x20)
				FM320_ModifyReg(MANUALRCV,BIT4,1);
			else
				FM320_ModifyReg(MANUALRCV,BIT4,0);
			
			//CRC 
			if((bP1&0x0F)==0x00) //NO CRC
			{
				FM320_ModifyReg(TXMODE,BIT7,0);
				FM320_ModifyReg(RXMODE,BIT7,0);
			}
			else if((bP1&0x0F)==0x02) //RX CRC
			{
				FM320_ModifyReg(TXMODE,BIT7,0);
				FM320_ModifyReg(RXMODE,BIT7,1);
			}
			else if((bP1&0x0F)==0x03) //TX CRC
			{
				FM320_ModifyReg(TXMODE,BIT7,1);
				FM320_ModifyReg(RXMODE,BIT7,0);
			}
			else
			{
				FM320_ModifyReg(TXMODE,BIT7,1);
				FM320_ModifyReg(RXMODE,BIT7,1);
			}
			
			if(CMD2==0x31) //进入非接触初始化指令流程
			{
				InitCmdFlag=1;
				ContactlessCardInitCmd(APDUBuffer+5,APDUBuffer[4],APDURecvLen);
			}
			else if(InitCmdFlag)
			{
				ContactlessCardInitCmd(APDUBuffer+5,APDUBuffer[4],APDURecvLen);
			}
			else		
				//CMD1 is Len
				CLTCLAPDU(APDUBuffer+5,APDUBuffer[4],APDURecvLen);
						
			for(b=0;b<*APDURecvLen;b++)
			{
				APDUBuffer[b]=APDUBuffer[b+5];
			}
		}	
		break;
	
  case INS_SEND_PPSA:
		*APDURecvLen = 0;
		//CL PPS
		if(param==0x000001)
		{
				if(CLCardPPS(CMD1)==CL_TCL_OK)
				{
					SetStatusWords(APDUBuffer, 0x9000);	
					*APDURecvLen = 2;	
				}
				else
				{
					SetStatusWords(APDUBuffer, 0x6D00);
					*APDURecvLen = 2;
				}

		}
		//CT PPS
		if(param==0x000101)
		{
			if(ContactCardPPS(CMD1)==CT_T0_OK) 
			{	
				SetStatusWords(APDUBuffer, 0x9000);	
				*APDURecvLen = 2;	
			}
			else
			{
				SetStatusWords(APDUBuffer, 0x6D00);
				*APDURecvLen = 2;
			}
		}	
    break;
	
	case INS_DATA_RECEIVE:  //C0指令
		*APDURecvLen = 0;
		APDUBuffer[0]=0x00;
		if(InitCmdFlag)
		{
			res=ContactCardInitCmd(APDUBuffer,APDUSendLen,APDURecvLen);
			if(res!=CT_T0_OK)
			{
				SetStatusWords(APDUBuffer, 0x6D00);
				*APDURecvLen = 2;
			}
		}
		else
		{ 
			res=ContactCardT0APDU(APDUBuffer,APDUSendLen,APDURecvLen);//jump to 0x00c000xy		
			if(res!=CT_T0_OK)
			{
				SetStatusWords(APDUBuffer, 0x6D00);
			}
		}
		break;
	
  default:
		*APDURecvLen = 0;
		SetStatusWords(APDUBuffer, 0x9000);	
		*APDURecvLen = 2;		
    break;

	}
}
/*******************************************************************************
* Function Name  : TIM3InterruptHandler.
* Description    : TIM3 interrupt handler.
* Input          : None 
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3InterruptHandler(void)
{}

