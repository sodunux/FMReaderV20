#include "includes.h"
#include "TDA8007.h"
#include "CCID.h"
#include "SpecialAPDU.h"
#include "ContactCard.h"

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
#define APDU_CASE_12  0x01
#define APDU_CASE_3   0x02

uint16_t const aiFI[16] = {372, 372, 558,  744, 1116, 1488, 1860, 0,
                             0, 512, 768, 1024, 1536, 2048,    0, 0};
uint8_t const abDI[16] = { 0,  1, 2, 4, 8, 16, 32, 64, 
                          12, 20, 0, 0, 0,  0,  0,  0};

uint8_t bATRSource;   // ATR comes from card or reader.
uint8_t volatile bT0Status;    // Reader will receive bytes or send bytes.
uint8_t bTA1;         // Communication rate parameter.
uint8_t bTC1;         // Extra Guard Time.
uint8_t bTC2;         // Work Waiting Time Integer.
uint8_t bProtocolT;   // T=0 or T=1.
uint32_t  lWwtInEtu;  // Work waiting time using ETU as unit.

uint8_t volatile bIsTimeout;    // TDA8007 Timer is overflow or not.
uint8_t volatile bIsParityErr;  // TDA8007 received enough parity error.
uint8_t volatile bRecvResult;   // Result of receiving bytes from card.

uint8_t bTAPos, bTBPos, bTCPos, bTDPos;   // TAi, TBi, TCi and TDi position in ATR.
uint8_t bATRCyc;      // Index value of TA, TB, TC and TD.
uint8_t bAPDUCase;    // Case 1, 2, 3
uint8_t bINS, bP3;    // INS and P3 of APDU Header

extern bool bIsCTon;

#ifdef TDA8007_DEBUG
uint8_t  bIntCnt;
uint8_t  abTemps[32];
#endif

/*******************************************************************************
* Function Name  : ContactCardInit.
* Description    : Initialize TDA8007 and contact card variables.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void ContactCardInit(void)
{
	TDA8007Init();
	bATRSource = 0;
	bT0Status = T0_STATUS_RECV_ATR;
	bTA1 = 0x11;
	bTC1 = 0;
	bTC2 = 10;
	lWwtInEtu = 9600;
	iTPDUBufferRecvCnt = 0;
	iTPDUBufferRecvCntPrev = 0;
	iTPDUBufferRecvLen = 0;
	iTPDUBufferSendCnt = 0;
	iTPDUBufferSendLen = 0;
	bRecvResult = 0;
	bProtocolT = 0;
	bTAPos = 0;
	bTBPos = 0;
	bTCPos = 0;
	bTDPos = 0;
	bATRCyc = 0;
#ifdef TDA8007_DEBUG
	bIntCnt = 0;
#endif
}

#define PARSER_TATBTCTD(T0TD, TEMP);   \
                                  do{   \
						TEMP = 0; \
						if(T0TD&0x10)  \
						{ \
							TEMP++; \
							bTAPos = (uint8_t)iTPDUBufferRecvCnt+TEMP;  \
						} \
						if(T0TD&0x20)  \
						{ \
							TEMP++; \
							bTBPos = (uint8_t)iTPDUBufferRecvCnt+TEMP;  \
						} \
						if(T0TD&0x40)  \
						{ \
							TEMP++; \
							bTCPos = (uint8_t)iTPDUBufferRecvCnt+TEMP;  \
						} \
						if(T0TD&0x80)  \
						{ \
							TEMP++; \
							bTDPos = (uint8_t)iTPDUBufferRecvCnt+TEMP;  \
						} \
                                 }while(0);

/*******************************************************************************
* Function Name  : ContactCardReset.
* Description    : Contact card reset process.
* Input          : PowerVolt  : Card Vcc voltage integer.
                                0x01 is 5V, 0x02 is 3V and 0x03 is 1.8V.                                 
* Output         : APDUBuffer   : ATR bytes received are here.
                   APDURecvLen  : How many bytes received.
* Return         : CT_T0_OK, 
                   CT_T0_NO_TS, 
                   CT_T0_ATR_TIMEOUT,
                   CT_T0_TS_ERROR,
                   CT_T0_TCK_ERROR.
*******************************************************************************/
uint8_t ContactCardReset(uint8_t PowerVolt, uint8_t *APDUBuffer, uint16_t *APDURecvLen)
{
	uint8_t bTemp;
	uint8_t bReturn;
	uint8_t b;

	/* Init device and variables */
	ContactCardInit();
	/* Initial before power on */
	bIsTimeout = 0;
	bIsParityErr = 0;
	/* Power on */
	if(PowerVolt != CARD_VCC_WARM)
	{
		TDA8007PowerOn(PowerVolt);
		EXTI_Configuration_TDA8007(ENABLE);
	}
	else
		TDA8007ResetPin(Bit_RESET);
		TDA8007WriteReg(RegUartConfiguration2,0x24);
		TDA8007WriteReg(RegUartConfiguration1,0x03);
		TDA8007SetTmo(10);        // Reset will be high at 100 ETU after card is powered on.
		TDA8007StartTmo();
	while(bIsTimeout == 0);  // Wait 10 ETU arrive. 
	bIsTimeout = 0;
	/* Initial before reset pin is high */
	TDA8007SetTmo(108);       // First byte timeout is 40000 clock cycles.
	/* Reset pin is high */
	TDA8007ResetPin(Bit_SET);
	TDA8007StartTmo();
	bRecvResult = 0;          // Receive result initial
	while((bIsTimeout == 0)&&(iTPDUBufferRecvCnt == 0));

	if(bIsTimeout)
	{
		bIsTimeout = 0;
		iTPDUBufferRecvLen = 0;
		return CT_T0_NO_TS;
	}  
	else if((abTPDUBuffer[0] != 0x3B)&&(abTPDUBuffer[0] != 0x3F))
	{
		TDA8007StopTmo();
		iTPDUBufferRecvLen = 0;
		return CT_T0_TS_ERROR;
	}
  
	iTPDUBufferRecvCntPrev++;
	iTPDUBufferRecvLen = 2;
	/* Wait ATR is completed */  
	while(bIsTimeout == 0)
	{
		if(iTPDUBufferRecvCntPrev < iTPDUBufferRecvCnt)
		{
			iTPDUBufferRecvCntPrev++;
			if(iTPDUBufferRecvCntPrev == 2)   // T0
			{
				if(abTPDUBuffer[1] == 0x00)
				{
					TDA8007StopTmo();
					return CT_T0_OK;  
				}
				else
				{
					iTPDUBufferRecvLen += (abTPDUBuffer[1]&0x0F);
					bATRCyc++;
					PARSER_TATBTCTD(abTPDUBuffer[1], bTemp);
					iTPDUBufferRecvLen += bTemp;
				} 
			}
			else if((uint8_t)iTPDUBufferRecvCntPrev == bTAPos)
			{
				if(bATRCyc == 1)
					bTA1 = abTPDUBuffer[iTPDUBufferRecvCntPrev-1];
			}
			else if((uint8_t)iTPDUBufferRecvCntPrev == bTCPos)
			{
				if(bATRCyc == 1)
					bTC1 = abTPDUBuffer[iTPDUBufferRecvCntPrev-1];
				else if(bATRCyc == 2)
					bTC2 = abTPDUBuffer[iTPDUBufferRecvCntPrev-1];
			}
			else if((uint8_t)iTPDUBufferRecvCntPrev == bTDPos)
			{
				bProtocolT = (abTPDUBuffer[iTPDUBufferRecvCntPrev-1]&0x0F);          
				bATRCyc++;
				bTAPos = 0;
				bTBPos = 0;
				bTCPos = 0;
				bTDPos = 0;
				PARSER_TATBTCTD(abTPDUBuffer[iTPDUBufferRecvCntPrev-1], bTemp);
				iTPDUBufferRecvLen += bTemp;      
				/* TCK is present or absent */
				if(bTDPos == 0)
				{
					if(bProtocolT)
						iTPDUBufferRecvLen++;
				}
			}

			if(iTPDUBufferRecvCntPrev == iTPDUBufferRecvLen)
			{            
				bReturn = CT_T0_OK;
				/* Check TCK value */
				if(bProtocolT)
				{
					bTemp = 0;
					for(b=1; b<(uint8_t)(iTPDUBufferRecvLen); b++)    
						bTemp ^= abTPDUBuffer[b];          
					if(bTemp)      
						bReturn = CT_T0_TCK_ERROR;
				}
				TDA8007StopTmo();
				break;
			}
		}
	}

	if(bIsTimeout)	  //gao 如果在比较之前产生了timeout而前面ATR正常收取了呢？虽然概率很小。
	{
		bIsTimeout = 0;
		iTPDUBufferRecvLen = 0;
		return CT_T0_ATR_TIMEOUT;  
	}
	else
	{
		if(bReturn == CT_T0_OK)
		{
			for(b=0; b<(uint8_t)iTPDUBufferRecvLen; b++)
				*(APDUBuffer+b) = abTPDUBuffer[b];
			*APDURecvLen = iTPDUBufferRecvLen;
			/* TC1 is set into TDA8007 */
			if(bTC1)
				TDA8007WriteReg(RegGuardTime, bTC1);
			/* TC2 configuration */
			if(bTC2)
				lWwtInEtu = 960*bTC2;

			stuInRegs.bATRSource = ATR_SOURCE_CARD; //gao
		}
		return bRecvResult;
	}
}

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
	uint8_t b;

	if(PPS1 == 0x11)
		return CT_T0_OK;
	/* Prepare PPS bytes */
	abAPDUBuffer[0] = 0xFF;
	abAPDUBuffer[1] = 0x10;
	abAPDUBuffer[2] = PPS1;
	abAPDUBuffer[3] = 0x00;
	for(b=0; b<3; b++)
		abAPDUBuffer[3] ^= abAPDUBuffer[b];

	/* Send bytes */  
	iTPDUBufferSendCnt = 0;
	iTPDUBufferSendLen = 4;
	bT0Status = T0_STATUS_SEND_PPS_REQUEST;
	TDA8007SetUCR1Bit(BIT_INDEX_TR, 1);   // To Transmit.
	//  TDA8007WriteFIFO(abTPDUBuffer[0]);
	while((bT0Status&0x01) == T0_STATUS_SEND);
	/* Receive bytes */  
	iTPDUBufferRecvCnt = 0;
	iTPDUBufferRecvLen = 4;
	bRecvResult = 0;
	while((iTPDUBufferRecvCnt < iTPDUBufferRecvLen) && (bIsTimeout == 0));

	if(bIsTimeout)
	{
		bIsTimeout = 0;
		iTPDUBufferRecvLen = 0;
		return CT_T0_PPS_TIMEOUT;
	}

	TDA8007StopTmo();
	for(b=0; b<4; b++)
	{
		if(abAPDUBuffer[b] != abTPDUBuffer[b])
		{
			return CT_T0_PPS_ERROR;
		}
	}
	/* Change PPS register */
	TDA8007PPS(abTPDUBuffer[2]);
	b = (abTPDUBuffer[2]&0x0F);
	b = abDI[b];
	lWwtInEtu *= b;
	return CT_T0_OK;    
}

/*******************************************************************************
* Function Name  : GetDeltaInTPDU.
* Description    : Calculate the delta value between two counter vaiables of TPDU.
* Input          : iFirstPos   : Maybe iTPDUBufferRecvPos.
                   iLastPos    : Maybe iTPDUBufferRecvCntPrev.                   
* Output         : None
* Return         : Delta value. 
*******************************************************************************/
uint16_t GetDeltaInTPDU(uint16_t iFirstPos, uint16_t iLastPos)
{
	uint16_t  i;

	if(iLastPos < iFirstPos)
		i = iLastPos + TPDU_BUFFER_SIZE - iFirstPos;
	else
		i = iLastPos - iFirstPos;

	return i;
}

#define SEND_BYTE();  do{TDA8007SetUCR1Bit(BIT_INDEX_TR, 1);}while(0);
#define SEND_BYTE_LCT();  do{TDA8007SetUCR1Bit(BIT_INDEX_LCT, 1);  \
                                 TDA8007SetUCR1Bit(BIT_INDEX_TR, 1); }while(0);  

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
	uint8_t   b;
	uint16_t  i;

	if(TDA8007ReadReg(RegMixedStatus)& CARD_PRESENT)
	{
		if(bIsCTon == FALSE)
		{
			return CT_T0_APDU_TIMEOUT;
		}

		/* Get APDU header */
		if(APDUSendLen < 4)
			return CT_T0_APDU_LEN_ERROR;
		else if(APDUSendLen == 4)
		{
			for(b=0; b<4; b++)
				abTPDUBuffer[b] = (*(APDUBuffer+b));
			abTPDUBuffer[b] = 0;
		}
		else
		{
			for(b=0; b<5; b++)
				abTPDUBuffer[b] = (*(APDUBuffer+b));
		}

		if(APDUSendLen <= 5)
			bAPDUCase = APDU_CASE_12;
		else
			bAPDUCase = APDU_CASE_3;

		bINS = abTPDUBuffer[1];
		bP3 = abTPDUBuffer[4];

		/* Send APDU header */
		iTPDUBufferSendCnt = 0;
		iTPDUBufferSendLen = 5;
		iTPDUBufferRecvCnt = 0;
		iTPDUBufferRecvLen = 0;
		iTPDUBufferRecvCntPrev = 0;
		iBufferRecvCnt = 0;
		bT0Status = T0_STATUS_SEND_APDU_HEAD;

		//#ifdef SPECIAL_APDU_ENABLE
		//  /* Triggle delay and width is ordered*/
		//  if(stuInRegs.bTriggleStatus > 0)
		//    TIM3Start();
		//#endif

		SEND_BYTE();     // To transmit.
		while((bT0Status&0x01) == T0_STATUS_SEND);
		/* Receive bytes */
		bT0Status = T0_STATUS_RECV_APDU_INS;
		bIsTimeout = 0;
RECEIVE_DATA:
		while(bIsTimeout == 0)
		{
			if(iTPDUBufferRecvCntPrev != iTPDUBufferRecvCnt)
			{
				b = abTPDUBuffer[iTPDUBufferRecvCntPrev];	
				switch(bT0Status)
				{
					case T0_STATUS_RECV_APDU_INS:
						if(b == 0x60)
						{
							INC_TPDU_CNT(iTPDUBufferRecvCntPrev);	 		  
						}				  	
						else if(((b&0xF0)==0x60) || ((b&0xF0)==0x90))
						{
							abBuffer[iBufferRecvCnt] = b;
							iBufferRecvCnt++;
							INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
							bT0Status = T0_STATUS_RECV_APDU_SW;
							iTPDUBufferRecvLen = 2;          
						}
						else if(b == bINS)
						{
							INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
							if(bAPDUCase == APDU_CASE_12) // Case 2
							{           
								iTPDUBufferRecvLen = bP3 - iTPDUBufferRecvLen;
								bT0Status = T0_STATUS_RECV_APDU_DATA;
							}
							else  // Case 3
							{
								TDA8007StopTmo();
								bT0Status = T0_STATUS_SEND_APDU_BODY;         
								iTPDUBufferSendLen = 5+bP3;
								iTPDUBufferRecvCnt = 0;
								iTPDUBufferRecvLen = 2;
								iTPDUBufferRecvCntPrev = 0;
								/* Wait 5 etu to transmit data */
								TDA8007SetTmo(5);
								TDA8007StartTmo();
								while(bIsTimeout == 0);
								bIsTimeout = 0;
								SEND_BYTE();
								while((bT0Status&0x01) == T0_STATUS_SEND);
								// bT0Status = T0_STATUS_RECV_APDU_SW;
								bT0Status = T0_STATUS_RECV_APDU_INS;
								goto RECEIVE_DATA;
							}  
						}
						else if((b^0xFF) == bINS)
						{
							INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
							if(bAPDUCase == APDU_CASE_12) // Case 2
							{          
								iTPDUBufferRecvLen ++;
								bT0Status = T0_STATUS_RECV_APDU_ONE_BYTE;
								goto RECEIVE_DATA;
							}
							else  // Case 3
							{
								TDA8007StopTmo();
								bT0Status = T0_STATUS_SEND_APDU_ONE_BYTE;         
								iTPDUBufferSendLen++;
								iTPDUBufferRecvCnt = 0;
								iTPDUBufferRecvLen = 1;
								iTPDUBufferRecvCntPrev = 0;
								SEND_BYTE_LCT();
								while((bT0Status&0x01) == T0_STATUS_SEND);
								bT0Status = T0_STATUS_RECV_APDU_INS;
								goto RECEIVE_DATA;
							}
						}
						break;

					case T0_STATUS_RECV_APDU_ONE_BYTE:
						abBuffer[iBufferRecvCnt] = b;
						iBufferRecvCnt++;
						INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
						bT0Status = T0_STATUS_RECV_APDU_INS;
						goto RECEIVE_DATA;
						//        break;

					case T0_STATUS_RECV_APDU_DATA:

						abBuffer[iBufferRecvCnt] = b;
						iBufferRecvCnt++;
						INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
						if(iBufferRecvCnt == iTPDUBufferRecvLen)   // Receive OK
						{          
							bT0Status = T0_STATUS_RECV_APDU_SW1;
							iTPDUBufferRecvLen += 2;
							goto RECEIVE_DATA;
						}	 
						break; 

					case T0_STATUS_RECV_APDU_SW1:      
						if(b == 0x60)
						{
							INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
						}								     									  
						else if(((b&0xF0)==0x60) || ((b&0xF0)==0x90))
						{
							abBuffer[iBufferRecvCnt] = b;
							iBufferRecvCnt++;
							INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
							bT0Status = T0_STATUS_RECV_APDU_SW;
						}
						break;

					case T0_STATUS_RECV_APDU_SW:
						abBuffer[iBufferRecvCnt] = b;
						iBufferRecvCnt++;
						INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
						if(iBufferRecvCnt == iTPDUBufferRecvLen)
						{
							TDA8007StopTmo();
							for(i=0; i<iTPDUBufferRecvLen; i++)
								*(APDUBuffer+i) = abBuffer[i];
							*APDURecvLen = iTPDUBufferRecvLen;   
							goto EXIT;
						}     
					break;
				}
			} 
		}

EXIT:
		if(bIsTimeout)
		{
			bIsTimeout = 0;
			iTPDUBufferRecvLen = 0;
			*APDURecvLen = 0;
			return CT_T0_APDU_TIMEOUT;
		}

		return CT_T0_OK;  // No meaning. Kill complier warning.
	}
	else
	{
		return CT_T0_APDU_TIMEOUT;		
	}
}

/*******************************************************************************
* Function Name  : ContactCardPowerOff.
* Description    : Contact card power off process.
* Input          : None
* Output         : None
* Return         : None                   
*******************************************************************************/
void ContactCardPowerOff(void)
{
	EXTI_Configuration_TDA8007(DISABLE);
	TDA8007PowerOff();
	TDA8007ResetPin(Bit_RESET);
}

extern void LEDShow(uint8_t Index, BitAction BitVal);
/*******************************************************************************
* Function Name  : ContactCardInterruptHandler.
* Description    : Contact card interrupt handler.
* Input          : None 
* Output         : None
* Return         : None
*******************************************************************************/
void ContactCardInterruptHandler(void)
{
	uint8_t  bIntSource;
	uint8_t  b;

	b = TDA8007ReadReg(RegMixedStatus);
	if((b&CARD_PRESENT) == CARD_ABSENT)
	{
		TDA8007PowerOff();
		stuInRegs.bIsPoweredOnCT = 1;//0; 

		bT0Status &= 0xFE;  // Change to receive.
		bIsTimeout = 1;  
		return;
	}

	bIntSource = TDA8007ReadReg(RegUartStatus);
  /* 
    In some cases, TDA8007 will generate interrupt signal but 
    the flags of internal register are all zero.
  */  /*
  if(bIntSource == 0) // this interrupt will cause reader in the dead loop reading fifo， 2013-04-10 gaoxuebing
  {
    if((bT0Status&0x01) == T0_STATUS_RECV)
    {
      LEDShow(3, Bit_RESET);

      b = TDA8007ReadFIFO();
      abTPDUBuffer[iTPDUBufferRecvCnt] = b;
      iTPDUBufferRecvCnt++;  
      if(iTPDUBufferRecvCnt == TPDU_BUFFER_SIZE)
        iTPDUBufferRecvCnt = 0;
      
      LEDShow(3, Bit_SET);

      if(stuInRegs.bTmoStatus != TMO_NO_LIMIT)
      {
        TDA8007StopTmo();
        // Non-standard timeout counter is needed 
        if(stuInRegs.bTmoStatus == TMO_NONE_STANDARD)
        {
          TDA8007SetTmo(100);
        }
        else
        {        
          TDA8007SetTmo(lWwtInEtu);
        }
        TDA8007StartTmo();
      }
      else
      {
        abIntBuffer[stuInRegs.iIntBufferLen] = b;
        stuInRegs.iIntBufferLen++;
      }
    }
  }		*/
#ifdef TDA8007_DEBUG   
  // abTemps[bIntCnt] = bIntSource;
#endif
	while(bIntSource & 0xE9)
	{
		if(bIntSource&0x01)  // UART FIFO interrupt
		{
			bIntSource &= 0xFE;
			if((bT0Status&0x01) == T0_STATUS_RECV)
			{
				LEDShow(3, Bit_RESET);

				b = TDA8007ReadFIFO();
				abTPDUBuffer[iTPDUBufferRecvCnt] = b;
				iTPDUBufferRecvCnt++;
				if(iTPDUBufferRecvCnt == TPDU_BUFFER_SIZE)
					iTPDUBufferRecvCnt = 0;

				LEDShow(3, Bit_SET);

				if(stuInRegs.bTmoStatus != TMO_NO_LIMIT)
				{
					TDA8007StopTmo();
					/* Non-standard timeout counter is needed */
					if(stuInRegs.bTmoStatus == TMO_NONE_STANDARD)
					{
						TDA8007SetTmo(100);
					}
					else
					{        
						TDA8007SetTmo(lWwtInEtu);
					}
					TDA8007StartTmo();
				}
				else
				{
					abIntBuffer[stuInRegs.iIntBufferLen] = b;
					stuInRegs.iIntBufferLen++;
				}
			}
			else if((bT0Status&0x01) == T0_STATUS_SEND)
			{
				if(iTPDUBufferSendCnt == (iTPDUBufferSendLen-1)) 
				{
					/* Last character to transmit */
					TDA8007SetUCR1Bit(BIT_INDEX_LCT, 1);  // Auto change to receive          
					bT0Status &= 0xFE;  // Change to receive.

					if(stuInRegs.bTmoStatus != TMO_NO_LIMIT)
					{
						/* Set timeout counter */
						TDA8007StopTmo();
						TDA8007SetTmo(lWwtInEtu);
						TDA8007StartTmo();
					}
				}
				TDA8007WriteFIFO(abAPDUBuffer[iTPDUBufferSendCnt]);
				iTPDUBufferSendCnt++;
			}
		}
		/*    else if(bIntSource&0xE0)  // Timeout interrupt	   //gao
		{
		bIntSource &= 0x1F;
		TDA8007StopTmo();
		bIsTimeout = 1;
		}*/
		else if(bIntSource&0x08)        
		{
			bIntSource &= 0xF7;
		}
	}
}
