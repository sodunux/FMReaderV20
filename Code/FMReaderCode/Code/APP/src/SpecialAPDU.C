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
	MiAuthTpeDef mifauth;
	uint8_t ret,i;
	u32 value;
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
			if(pData[0]) //开场
			{
				FM320_POWERON;
				ret=FM320_Initial_ReaderA();
				if(ret)
				{
					SetStatusWords(APDUBuffer, 0x6F00);	
					*APDURecvLen = 2;						
				}					
			}
			else 
				FM320_POWEROFF;
			break;
		case 0x010102: //ReqA
			ret=FM320_ReaderA_Request(PICC_REQIDL);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_AntiCol(0);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_AntiCol(1);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_AntiCol(2);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_Select(0);	
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_Select(1);	
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_Select(2);	
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			ret=FM320_ReaderA_Rats(8,1);
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
			ret=FM320_ReaderA_HALT();
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);	
				*APDURecvLen = 2;						
			}	
			break;
		case 0x010B03://MI_REQB
				//TODO
			break;
		case 0x010A02://MI_WUPA
			ret=FM320_ReaderA_Request(PICC_REQALL);
			if(ret)
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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
			
		case 0x010901://MI_Authent add by yanpengfei (FF18010901+keytype+block+key)
			if(APDUSendLen!=13)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;				
			}
			else
			{
				if(pData[0]==0)
					mifauth.keytype=KEYA;
				else
					mifauth.keytype=KEYB;
				mifauth.block=pData[1];
				for(i=0;i<6;i++)
					mifauth.key[i]=pData[2+i];
				for(i=0;i<4;i++)
					mifauth.UID[i]=CardA_Sel_Res.UID[i];
				ret=FM320_Authent(mifauth);	
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;		
				}
			}
			break;	
		case 0x010501://MI_RDBLOCK
			if(APDUSendLen!=6)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;		
			}
			else
			{
				ret=FM320_MiRead(pData[0],APDUBuffer);
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;						
				}
				else
				{
					APDUBuffer[16]=0x90;
					APDUBuffer[17]=0x00;
					*APDURecvLen = 18;	
				}
			}	
			break;
		case 0x010611://MI_WRITEBLOCK
			if(APDUSendLen!=22)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;		
			}
			else
			{
				ret=FM320_MiWrite(pData[0],APDUBuffer+6);
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;					
				}
				else
				{
					APDUBuffer[0]=0x90;
					APDUBuffer[1]=0x00;
					*APDURecvLen = 2;	
				}
			}
			break;
		case 0x011105://MI_IncBlock
			if(APDUSendLen!=10)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;		
			}
			else
			{
				value=(pData[1]<<24)|(pData[2]<<16)|(pData[3]<<8)|(pData[4]);
				ret=FM320_MiIncrement(pData[0],value);
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;					
				}
				else
				{
					APDUBuffer[0]=0x90;
					APDUBuffer[1]=0x00;
					*APDURecvLen = 2;	
				}
			}
			break;
		case 0x011205: //MI_DecBlock 
			if(APDUSendLen!=10)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;		
			}
			else
			{
				value=(pData[1]<<24)|(pData[2]<<16)|(pData[3]<<8)|(pData[4]);
				ret=FM320_MiDecrement(pData[0],value);
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;					
				}
				else
				{
					APDUBuffer[0]=0x90;
					APDUBuffer[1]=0x00;
					*APDURecvLen = 2;	
				}
			}
			break;
		case 0x011301://MI_Restore
			if(APDUSendLen!=6)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;		
			}
			else
			{
				ret=FM320_MiRestore(pData[0]);
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;					
				}
				else
				{
					APDUBuffer[0]=0x90;
					APDUBuffer[1]=0x00;
					*APDURecvLen = 2;	
				}
			}			
			break;
		case 0x011401: //MI_Transfer
			if(APDUSendLen!=6)
			{
				*APDUBuffer=0x67;
				*(APDUBuffer+1)	=0x00;
				*APDURecvLen=2;		
			}
			else
			{
				ret=FM320_MiTransfer(pData[0]);
				if(ret)
				{
					*APDUBuffer=0x67;
					*(APDUBuffer+1)	=0x00;
					*APDURecvLen=2;					
				}
				else
				{
					APDUBuffer[0]=0x90;
					APDUBuffer[1]=0x00;
					*APDURecvLen = 2;	
				}
			}
			break;
		default:
			break;
	}
}


void SpecApduCT(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	uint8_t b;
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
		case 0x010101: //ColdReset
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
			if(pData[0] == CARD_VCC_5V)
				SC_VoltageConfig(SC_Voltage_5V);
			else if(pData[0] == CARD_VCC_18V)
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
		case 0x010201: //WarmReset
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
			if(pData[0] == CARD_VCC_5V)
				SC_VoltageConfig(SC_Voltage_5V);
			else if(pData[0] == CARD_VCC_18V)
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
}

void DataTransmit(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	u8 ret=0,i;
	u8 recevLen=0;
	u8 tx_crc,rx_crc;
	uint8_t   bINS = *(APDUBuffer+INDEX_INS);
  uint8_t   bP1 =  *(APDUBuffer+INDEX_P1);
  uint8_t   bP2 =  *(APDUBuffer+INDEX_P2);
  uint16_t  bP3 =  *(APDUBuffer+INDEX_P3);		
  uint8_t   *pData = (APDUBuffer+INDEX_DATA);
	uint32_t iParam=(bP1<<16)|(bP2<<8)|(bP3<<0);
	SetStatusWords(APDUBuffer, 0x9000);	
	*APDURecvLen = 2;
	
	if(stuInRegs.bCurrentDevice==CURRENT_DEVICE_CL)
	{
		if(APDUSendLen>5)
		{
			if(bP3!=(APDUSendLen-5))
			{
				SetStatusWords(APDUBuffer, 0x6F00);	
				*APDURecvLen = 2;
			}
			else
			{
				switch(bP1)
				{
					case 0x00: //disable tx,rx crc
						tx_crc=0;
						rx_crc=0;
						break;
					case 0x02: //rx crc enable
						tx_crc=0;
						rx_crc=1;
						break;
					case 0x03: //tx_crc_enable
						tx_crc=1;
						rx_crc=0;						
						break;
					default: //all crc enable
						tx_crc=1;
						rx_crc=1;						
						break;
				}
				ret=FM320_DirectSend(pData,bP3,&recevLen,tx_crc,rx_crc);
				for(i=0;i<recevLen;i++)
					APDUBuffer[i]=pData[i];
				SetStatusWords(APDUBuffer+recevLen,0x9000);	
				*APDURecvLen = recevLen+2;			
			}
	}
		else
		{
			SetStatusWords(APDUBuffer, 0x6F00);	
			*APDURecvLen = 2;
		}
	}
	else if(stuInRegs.bCurrentDevice==CURRENT_DEVICE_CT)
	{
		//TODO
		
		
	}
	
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
				SetStatusWords(APDUBuffer, 0x6F00);
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
					SetStatusWords(APDUBuffer, 0x6F00);
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
					SetStatusWords(APDUBuffer, 0x6F00);
					*APDURecvLen = 2;
				}
			}
			else 
			{
				SetStatusWords(APDUBuffer, 0x6F00);
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


