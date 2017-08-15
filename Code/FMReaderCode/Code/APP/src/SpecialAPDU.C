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
	uint8_t ret,i;
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
			if(pData[0]) //¿ª³¡
			{
				FM320_POWERON;
				ret=FM320_Initial_ReaderA();
				if(ret)
				{
					SetStatusWords(APDUBuffer, 0x6700);	
					*APDURecvLen = 2;						
				}					
			}
			else 
				FM320_POWEROFF;
			break;
		case 0x010102: //ReqA
			ret=ReaderA_Request(PICC_REQIDL);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;	
			}
				
			else
			{
				APDUBuffer[0]=CardA_Sel_Res.ATQA[0];
				APDUBuffer[1]=CardA_Sel_Res.ATQA[1];
				APDUBuffer[2]=0x90;
				APDUBuffer[3]=0x00;
				*APDURecvLen = 4;	
			}
			break;
		case 0x010206://MI_ANTICOLL1
			ret=ReaderA_AntiCol(0);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;	
			}
			else	
			{
				for(i=0;i<4;i++)
				{
					APDUBuffer[i]=CardA_Sel_Res.UID[i];
					
				}
				APDUBuffer[4]=APDUBuffer[0]^APDUBuffer[1]^APDUBuffer[2]^APDUBuffer[3];
				APDUBuffer[5]=0x90;
				APDUBuffer[6]=0x00;
				*APDURecvLen = 7;			
			}
			break;
		case 0x012206://MI_ANTICOLL2
			ret=ReaderA_AntiCol(1);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;	
			}
			else	
			{
				for(i=0;i<4;i++)
				APDUBuffer[i]=CardA_Sel_Res.UID[i+3];
				
				APDUBuffer[4]=APDUBuffer[0]^APDUBuffer[1]^APDUBuffer[2]^APDUBuffer[3];
				APDUBuffer[5]=0x90;
				APDUBuffer[6]=0x00;
				*APDURecvLen = 7;			
			}			
			break;
		case 0x013206://MI_ANTICOLL3
			ret=ReaderA_AntiCol(2);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;	
			}
			else	
			{
				for(i=0;i<4;i++)
				APDUBuffer[i]=CardA_Sel_Res.UID[i+6];
				APDUBuffer[4]=APDUBuffer[0]^APDUBuffer[1]^APDUBuffer[2]^APDUBuffer[3];
				APDUBuffer[5]=0x90;
				APDUBuffer[6]=0x00;
				*APDURecvLen = 7;			
			}				
			break;
		case 0x010301://MI_SEL1
			ret=ReaderA_Select(0);	
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;						
			}
			else 
			{
				APDUBuffer[0]=CardA_Sel_Res.SAK;
				APDUBuffer[1]=0x90;
				APDUBuffer[2]=0x00;
				*APDURecvLen = 3;					
			}
			break;
		case 0x012301://MI_SEL2
			ret=ReaderA_Select(1);	
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;						
			}
			else 
			{
				APDUBuffer[0]=CardA_Sel_Res.SAK;
				APDUBuffer[1]=0x90;
				APDUBuffer[2]=0x00;
				*APDURecvLen = 3;					
			}
			break;
		case 0x013301://MI_SEL3
			ret=ReaderA_Select(2);	
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;						
			}
			else 
			{
				APDUBuffer[0]=CardA_Sel_Res.SAK;
				APDUBuffer[1]=0x90;
				APDUBuffer[2]=0x00;
				*APDURecvLen = 3;					
			}
			break;
			
		case 0x010401://Rats
			ret=ReaderA_Rats(8,1);
			if(!ret)
			{
				if(!CardA_Sel_Res.ATSLEN)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;
				}
				else
				{
					for(i=0;i<CardA_Sel_Res.ATSLEN;i++)
					{
						*(APDUBuffer+i) = CardA_Sel_Res.ATS[i];
					}
						SetStatusWords(APDUBuffer+CardA_Sel_Res.ATSLEN, 0x9000);
						*APDURecvLen = CardA_Sel_Res.ATSLEN+2;
				}
			}
			else
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;
			}			
			break;

		case 0x010701://MI_HALT
			ret=ReaderA_HALT();
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);	
				*APDURecvLen = 2;						
			}	
			break;
		case 0x010B03://MI_REQB
				//TODO
			break;
		case 0x010A02://MI_WUPA
			ret=ReaderA_Request(PICC_REQALL);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6700);
				*APDURecvLen = 2;	
			}
				
			else
			{
				APDUBuffer[0]=CardA_Sel_Res.ATQA[0];
				APDUBuffer[1]=CardA_Sel_Res.ATQA[1];
				APDUBuffer[2]=0x90;
				APDUBuffer[3]=0x00;
				*APDURecvLen = 4;	
			}
			break;
			
		case 0x010501://MI_RDBLOCK
			//TODO
			break;
		case 0x010611://MI_WRITEBLOCK
			//TODO
			break;
		case 0x011105://MI_IncBlock
			//TODO
			break;
		case 0x011205: //MI_DecBlock 
			//TODO
			break;
		case 0x011301://MI_Restore
			//TODO
			break;
		case 0x011401: //MI_Transfer
			//TODO
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
	*APDURecvLen=0;
	
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


