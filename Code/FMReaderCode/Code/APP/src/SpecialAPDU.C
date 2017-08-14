#include "includes.h"               

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


void SpecApduCL(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	uint8_t   bINS = *(APDUBuffer+INDEX_INS);
  uint8_t   bP1 =  *(APDUBuffer+INDEX_P1);
  uint8_t   bP2 =  *(APDUBuffer+INDEX_P2);
  uint16_t  bP3 =  *(APDUBuffer+INDEX_P3);		
  uint8_t   *pData = (APDUBuffer+INDEX_DATA);
	uint32_t iParam=(bP1<<16)|(bP2<<8)|(bP3<<0);
	SetStatusWords(APDUBuffer, 0x9000);	
	*APDURecvLen = 2;	

	switch(iParam)
	{
		case 0x010001: //MI_FieldON
			break;
		case 0x010102: //ReqA
			break;
		case 0x010206://MI_ANTICOLL1
			break;
		case 0x012206://MI_ANTICOLL2
			break;
		case 0x013206://MI_ANTICOLL3
			break;
		case 0x010301://MI_SEL1
			break;
		case 0x012301://MI_SEL2
			break;
		case 0x013301://MI_SEL3
			break;
		case 0x010401://Rats
			break;
		case 0x010501://MI_RDBLOCK
			break;
		case 0x010611://MI_WRITEBLOCK
			break;
		case 0x010701://MI_HALT
			break;
		case 0x010801://MI_BAUDRATE
			break;
		case 0x010B03://MI_REQB
			break;
		case 0x010A02://MI_WUPA
			break;
		case 0x011105://MI_IncBlock
			break;
		case 0x011205: //MI_DecBlock 
			break;
		case 0x011301://MI_Restore
			break;
		case 0x011401: //MI_Transfer
			break;
		default:
			break;
	}
	
	
}

void SpecApduCT(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	uint8_t   bINS = *(APDUBuffer+INDEX_INS);
  uint8_t   bP1 =  *(APDUBuffer+INDEX_P1);
  uint8_t   bP2 =  *(APDUBuffer+INDEX_P2);
  uint16_t  bP3 =  *(APDUBuffer+INDEX_P3);		
  uint8_t   *pData = (APDUBuffer+INDEX_DATA);
	uint32_t iParam=(bP1<<16)|(bP2<<8)|(bP3<<0);
	SetStatusWords(APDUBuffer, 0x9000);	
	*APDURecvLen = 2;	
	
	switch(iParam)
	{
		case 0x010001: //InitCT
			break;
		case 0x010101: //ColdReset
			break;
		case 0x010201: //WarmReset
			break;
		default:
			break;
	}
}

void DataTransmit(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	uint8_t   bINS = *(APDUBuffer+INDEX_INS);
  uint8_t   bP1 =  *(APDUBuffer+INDEX_P1);
  uint8_t   bP2 =  *(APDUBuffer+INDEX_P2);
  uint16_t  bP3 =  *(APDUBuffer+INDEX_P3);		
  uint8_t   *pData = (APDUBuffer+INDEX_DATA);
	uint32_t iParam=(bP1<<16)|(bP2<<8)|(bP3<<0);
	SetStatusWords(APDUBuffer, 0x9000);	
	*APDURecvLen = 2;
	
	
}

void DataReceive(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	uint8_t   bINS = *(APDUBuffer+INDEX_INS);
  uint8_t   bP1 =  *(APDUBuffer+INDEX_P1);
  uint8_t   bP2 =  *(APDUBuffer+INDEX_P2);
  uint16_t  bP3 =  *(APDUBuffer+INDEX_P3);		
  uint8_t   *pData = (APDUBuffer+INDEX_DATA);
	uint32_t iParam=(bP1<<16)|(bP2<<8)|(bP3<<0);
	uint8_t ret;
	SetStatusWords(APDUBuffer, 0x9000);	
	*APDURecvLen=2;
		
}

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
  uint8_t   *pData = (APDUBuffer+INDEX_DATA);
	
	uint32_t iParam=(bP1<<16)|(bP2<<8)|(bP3<<0);
	APDURecvLen=0;
	
	switch(bINS)
	{
		case INS_SELECT_DEVICE:
			if(iParam==0x000001)
			{
				if((pData[0]&0x0F)==0x02) //CL
					stuInRegs.bCurrentDevice=CURRENT_DEVICE_CL;
				else //CT
					stuInRegs.bCurrentDevice=CURRENT_DEVICE_CT;
				SetStatusWords(APDUBuffer, 0x9000);	
				*APDURecvLen = 2;			
			}
			else
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;			
			}
			break;			
		case INS_READ_REGISTER_CL:
			FM320_GetReg(bP2,APDUBuffer);
			SetStatusWords(APDUBuffer+1, 0x9000);
			*APDURecvLen = 3;
			break;
		case INS_WRITE_REGISTER_CL:
			FM320_SetReg(bP2,pData[0]);
			SetStatusWords(APDUBuffer+1, 0x9000);
			*APDURecvLen = 3;		
			break;
		case INS_CL_OPERATION:
			SpecApduCL(APDUBuffer,APDUSendLen,APDURecvLen);
			break;
		case INS_CT_OPERATION:
			SpecApduCT(APDUBuffer,APDUSendLen,APDURecvLen);
			break;
		case INS_DATA_TRANSMIT:
			DataTransmit(APDUBuffer,APDUSendLen,APDURecvLen);
			break;
		case INS_SEND_PPSA:
			if(iParam==0x000001) //CL
			{
				if(CLCardPPS(pData[0])==CL_TCL_OK)
				{
					SetStatusWords(APDUBuffer, 0x9000);	
					*APDURecvLen = 2;	
				}
				else
				{
					SetStatusWords(APDUBuffer, 0x6700);
					*APDURecvLen = 2;
				}
			}
			else if(iParam==0x000101)//CT PPS
			{
				if(ContactCardPPS(pData[0])==CT_T0_OK) 
				{	
					SetStatusWords(APDUBuffer, 0x9000);	
					*APDURecvLen = 2;	
				}
				else
				{
					SetStatusWords(APDUBuffer, 0x6700);
					*APDURecvLen = 2;
				}
			}
			else 
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;
			}
			break;			
		case INS_DATA_RECEIVE:
			DataReceive(APDUBuffer,APDUSendLen,APDURecvLen);
			break;		
		default:
			SetStatusWords(APDUBuffer, 0x9000);	
			*APDURecvLen = 2;		
			break;		
	}	
		
}


