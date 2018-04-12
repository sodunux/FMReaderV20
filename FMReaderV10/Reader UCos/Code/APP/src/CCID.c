

#include "usb_lib.h"
#include "stdint.h"
#include "CCID.h"
#include "TDA8007.h"

uint8_t   volatile bReaderStatus;
uint8_t   bCardSlotStatus;

uint8_t   abBuffer[CCID_BUFFER_SIZE];
uint16_t  iBufferRecvLen, iBufferRecvCnt;
uint16_t  iBufferSendLen, iBufferSendCnt;
uint8_t   abAPDUBuffer[APDU_BUFFER_SIZE];
uint16_t  iAPDUBufferSendLen, iAPDUBufferSendCnt;
uint16_t  iAPDUBufferRecvLen, iAPDUBufferRecvCnt;
uint8_t   abTPDUBuffer[TPDU_BUFFER_SIZE];
uint16_t  volatile iTPDUBufferRecvLen, iTPDUBufferRecvCnt, iTPDUBufferRecvCntPrev, iTPDUBufferRecvPos;
uint16_t  volatile iTPDUBufferSendLen, iTPDUBufferSendCnt;

/* Clock frequency is 3.58M */
uint8_t const CCID_READER_CLOCK_FREQUENCIES[4]={0xFC, 0x0D, 0x00, 0x00};
/* Data rates are 9600, 57600, 115200, 230400, 460800 */
/* uint8_t const CCID_READER_DATA_RATES[4]={0x80, 0x25, 0x00, 0x00}; */
uint8_t const CCID_READER_DATA_RATES[16]={0x80, 0x25, 0x00, 0x00, // 9600
                                          0x00, 0x96, 0x00, 0x00, // 38400
                                          0x00, 0xE1, 0x00, 0x00, // 57600
                                          0x00, 0xC2, 0x01, 0x00  // 115200                                   
                                         };
/*
uint8_t const CCID_READER_DATA_RATES[20]={0x80, 0x25, 0x00, 0x00, // 9600
                                          0x00, 0xE1, 0x00, 0x00, // 57600
                                          0x00, 0xC2, 0x01, 0x00, // 115200
                                          0x00, 0x84, 0x03, 0x00, // 230400
                                          0x00, 0x08, 0x07, 0x00, // 460800
                                         };
*/
/* PC_to_RDR_XXX */
  /* Common fields */
uint8_t  bMessageType;       // Index = 0
uint32_t dwLength;           // Index = 1
uint8_t  bSlot;              // Index = 5
uint8_t  bSeq;               // Index = 6
uint8_t  bPrevSeq;
  /* Special fields */
    /* PC_to_RDR_Power_On */
uint8_t  bPowerSelect;       // Index = 7
    /* PC_to_RDR_SetParameters */
uint8_t  bProtocolNum;       // Index = 7 									    
      /* bProtocolNum = 0 */
uint8_t  bmFindexDindex;     // Index = 10
uint8_t  bmTCCKST0;          // Index = 11
uint8_t  bGuardTimeT0;       // Index = 12
uint8_t  bWaitingIntegerT0;  // Index = 13
uint8_t  bClockStop;         // Index = 14
      /* bProtocolNum = 1 */
uint8_t  bmFindexDindex;     // Index = 10
uint8_t  bmTCCKST1;          // Index = 11
uint8_t  bGuardTimeT1;       // Index = 12
uint8_t  bWaitingIntegerT1;  // Index = 13
uint8_t  bClockStop;         // Index = 14
uint8_t  bIFSC;              // Index = 15
uint8_t  bNadValue;          // Index = 16
    /* PC_to_RDR_XfrBlock */
uint8_t  bBWI;               // Index = 7
uint16_t wLevelParameter;    // Index = 8
/* RDR_to_PC_XXX */
  /* Common fields */
uint8_t  bStatus;            // Index = 7
uint8_t  bError;             // Index = 8


/*******************************************************************************
* Function Name  : IsValidSeq.
* Description    : Check the sequence number of command is valid or not.
* Input          : None
* Output         : None
* Return         : CCID_OK, CCID_NOK.
*******************************************************************************/
uint8_t IsValidSeq(void)
{
	uint8_t  b;

	if(bPrevSeq == 0xFF)
	{
		if(bSeq)  
			b = CCID_NOK;
		else      
			b = CCID_OK;
	}
	else
	{
		if(bSeq == (bPrevSeq+1))  
			b = CCID_OK;
		else      
			b = CCID_NOK;
	}
	bPrevSeq = bSeq;
	return b;
}

/*******************************************************************************
* Function Name  : CCIDInit.
* Description    : Initialize the variables used in CCID module.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void CCIDInit(void)
{
	bPrevSeq = 0;
	iBufferRecvCnt = 0;
	iBufferRecvLen = 0;
	iBufferSendCnt = 0;
	iBufferSendLen = 0;
	bReaderStatus = RDS_IDLE;
}

/*******************************************************************************
* Function Name  : CCIDParser.
* Description    : Parser CCID package to determine which action should be done.
                   APDU is also be seperated from this function.
* Input          : CCIDBuffer : CCID package received from USB.
* Output         : APDUBuffer : APDU seperated from CCID package is stored here.
                   APDULength : How many bytes are included in APDU.
* Return         : ACT_POWER_ON_5V, ACT_POWER_ON_3V, ACT_POWER_ON_18V,
                   ACT_POWER_OFF,
                   ACT_APDU_EXCHANGE,
                   CCID_NOK.
*******************************************************************************/
uint8_t CCIDParser(uint8_t* CCIDBuffer, uint8_t* APDUBuffer, uint16_t *APDULength)
{
	uint8_t  b;
	uint32_t l;
	uint8_t  bReturn = CCID_OK;

	bMessageType = (*CCIDBuffer);
	bSlot = (*(CCIDBuffer+5));
	bSeq = (*(CCIDBuffer+6));

	if(IsValidSeq() == CCID_NOK)
		return CCID_NOK;

	switch(bMessageType)  
	{
		case PC_to_RDR_IccPowerOn:
			bPowerSelect = (*(CCIDBuffer+7)); 
			bReturn = (ACT_POWER_ON | bPowerSelect);           
			break;
		case PC_to_RDR_IccPowerOff:
			bReturn = ACT_POWER_OFF;
			break;
		case PC_to_RDR_SetParameters:
			bProtocolNum = (*(CCIDBuffer+7));
			bmFindexDindex = (*(CCIDBuffer+10));
			bClockStop = (*(CCIDBuffer+14));
			if(bProtocolNum == 0)     // T=0
			{
				bmTCCKST0 = (*(CCIDBuffer+11));    
				bGuardTimeT0 = (*(CCIDBuffer+12));
				bWaitingIntegerT0 = (*(CCIDBuffer+13));
			}
			else if(bProtocolNum == 1) // T=1
			{
				bmTCCKST1 = (*(CCIDBuffer+11));    
				bGuardTimeT1 = (*(CCIDBuffer+12));
				bWaitingIntegerT1 = (*(CCIDBuffer+13));
				bIFSC = (*(CCIDBuffer+15));
				bNadValue = (*(CCIDBuffer+16));
			}
			else
				return CCID_NOK;
			bReturn = ACT_SET_PARAM;
			break;
		case PC_to_RDR_XfrBlock:
			dwLength = 0;
			for(b=4; b>0; b--)
			{
				dwLength <<= 8;
				dwLength |= (*(CCIDBuffer+b));
			}

			for(l=0; l<dwLength; l++)
			{
				*(APDUBuffer+l) = (*(CCIDBuffer+10+l));
			}

			if(dwLength == 4)
			{
				*(CCIDBuffer+14) = 0;
				dwLength++;
			}
			*APDULength = (uint16_t)dwLength;
			bReturn = ACT_APDU_EXCHANGE;
			break;
		case PC_to_RDR_SetDataRateAndClockFrequency:  // Why send this command on this version?
			bReturn = CCID_OK;
			break;
		default:
			bReturn = CCID_NOK;
			break;
	}
	return bReturn;
}

/*******************************************************************************
* Function Name  : CCIDOrgnize.
* Description    : Orgnize CCID package to send to PC.
* Input          : APDUBuffer : APDU seperated from CCID package is stored here.
                   APDULength : How many bytes are included in APDU.
* Output         : CCIDBuffer : CCID package received from USB.
                   CCIDLength : How many bytes are included in CCID package.
* Return         : None.
*******************************************************************************/
void CCIDOrgnize(uint8_t* APDUBuffer, uint16_t APDULength, uint8_t* CCIDBuffer, uint16_t *CCIDLength)
{
	uint32_t l;

	switch(bMessageType)
	{
		case PC_to_RDR_IccPowerOn:
		case PC_to_RDR_XfrBlock:
		case PC_to_RDR_Secure:
			bMessageType = RDR_to_PC_DataBlock;
			break;
		case PC_to_RDR_IccPowerOff:
		case PC_to_RDR_GetSlotStatus:
		case PC_to_RDR_IccClock:
		case PC_to_RDR_T0APDU:
		case PC_to_RDR_Mechanical:
		case PC_to_RDR_Abort:
			bMessageType = RDR_to_PC_SlotStatus;
			break;
		case PC_to_RDR_GetParameters:
		case PC_to_RDR_ResetParameters:
		case PC_to_RDR_SetParameters:
			bMessageType = RDR_to_PC_Parameters;
			break;
		case PC_to_RDR_Escape:
			bMessageType = RDR_to_PC_Escape;
			break;
		case PC_to_RDR_SetDataRateAndClockFrequency:
			bMessageType = RDR_to_PC_DataRateAndClockFrequecncy;
			break;  
	}

	if((bStatus&0xC0) == CCID_COMMAND_STATUS_NO_ERROR)
	{    
		if(bMessageType == RDR_to_PC_DataBlock)
		{
			*(CCIDBuffer+9) = 0x00;         // bChainParameter
			dwLength = (uint32_t)APDULength;
			if(dwLength)      
			{
				for(l=0; l<dwLength; l++)
				{
					*(CCIDBuffer+10+l) = (*(APDUBuffer+l));
				}  
			}
		}
		else if(bMessageType == RDR_to_PC_SlotStatus)
		{
			*(CCIDBuffer+9) = 0x00;         // bClcokStatus      
			dwLength = 0;  
		}
		else if(bMessageType == RDR_to_PC_Parameters)
		{
			*(CCIDBuffer+9) = bProtocolNum; // bProtocolNum
			if(bProtocolNum == 0)
				dwLength = 5;
			else if(bProtocolNum == 1)
				dwLength = 7;
		}
		else if(bMessageType == RDR_to_PC_DataRateAndClockFrequecncy)
		{
			*(CCIDBuffer+9) = 0;
			dwLength = 8;  
		}
	}
	else
	{
		*(CCIDBuffer+9) = 0;
		dwLength = 0;       
	}

	*(CCIDBuffer) = bMessageType;
	*(CCIDBuffer+1) = (uint8_t)(dwLength&0xFF);
	*(CCIDBuffer+2) = (uint8_t)((dwLength&0xFF00)>>8);
	*(CCIDBuffer+3) = (uint8_t)((dwLength&0xFF0000)>>16);
	*(CCIDBuffer+4) = (uint8_t)((dwLength&0xFF000000)>>24);
	*(CCIDBuffer+5) = bSlot;
	*(CCIDBuffer+6) = bSeq;
	*(CCIDBuffer+7) = bStatus;
	*(CCIDBuffer+8) = bError;
	*CCIDLength = 10+(uint16_t)dwLength;
}

/*******************************************************************************
* Function Name  : CCIDCardSlotChange.
* Description    : Notify PC that card slot status has changed.
* Input          : None
* Output         : None
* Return         : None.
*******************************************************************************/
void CCIDCardSlotChange(void)
{
	abBuffer[0] = 0x50;
	abBuffer[1] = (0x02|bCardSlotStatus);
	iBufferSendCnt = 0;
	iBufferSendLen = 2;

	UserToPMABufferCopy(abBuffer, ENDP3_TXADDR, iBufferSendLen);
	SetEPTxCount(ENDP3, iBufferSendLen);
	SetEPTxValid(ENDP3);
}

/*******************************************************************************
* Function Name  : CCIDCardSlotCheck.
* Description    : Check the contact card is present or not.
* Input          : None
* Output         : None
* Return         : CCID_CARD_SLOT_PRESENT,
                   CCID_CARD_SLOT_ABSENT
*******************************************************************************/
uint8_t CCIDCardSlotCheck(void)
{
	uint8_t b;

	b = TDA8007ReadReg(RegMixedStatus);
	if((b&0x04) == CARD_PRESENT)
	{
		return CCID_CARD_SLOT_PRESENT;
	}
	else
	{
		return CCID_CARD_SLOT_ABSENT;
	}
}
/******************************************************************************************/
