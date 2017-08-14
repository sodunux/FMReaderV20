#include "includes.h"

uint16_t const FrameSize[9] = {16, 24, 32, 40, 48, 64, 96, 128, 256};
TCLParam stuTCLParam;


/*******************************************************************************
* Function Name  : CLCardInit.
* Description    : Variables of contactless card initialization.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void CLCardInit(void)
{
  uint8_t b;
  //初始化结构体
  for(b=0; b<2; b++)
  stuTCLParam.abATQ[b] = 0;
  stuTCLParam.bATQLen = 0;  
  stuTCLParam.bUIDLevel = 0;

  for(b=0; b<12; b++)
  	stuTCLParam.abUID[b] = 0;
  stuTCLParam.bSak = 0;

  for(b=0; b<32; b++)
  	stuTCLParam.abATS[b] = 0;
  stuTCLParam.iATSLen = 0;

  stuTCLParam.bFSCI = 8;  
  stuTCLParam.bDRC = 0;
  stuTCLParam.bDSC = 0;
  stuTCLParam.bFWI = 12; //4
  stuTCLParam.lFWT = 1<<12;	//16
  stuTCLParam.bSFGI = 8;
  stuTCLParam.bNADEn = 0;
  stuTCLParam.bCIDEn = 1;

  stuTCLParam.bINFLen = 254;
  stuTCLParam.bBlockNum = 0;
  //PRTCL=0;
	for(b=0;b<2;b++)
	{
		CardA_Sel_Res.ATQA[b]=0;
	}
	for(b=0;b<12;b++)
	{
		CardA_Sel_Res.UID[b]=0;
	}
	for(b=0;b<32;b++)
	{
		CardA_Sel_Res.ATS[b]=0;
	}
	CardA_Sel_Res.ATSLEN=0;
	CardA_Sel_Res.SAK=0;

  CLCardRegInit();
}

/*******************************************************************************
* Function Name  : CLCardReset.
* Description    : Contactless card reset process.
* Input          : None
* Output         : APDUBuffer   : ATR bytes received are here.
                   APDURecvLen  : How many bytes received.
* Return         : CL_TCL_OK, 
                   CL_TCL_ATQ_ERROR, 
                   CL_TCL_UID_ERROR,
                   CL_TCL_ATS_ERROR,
                   CL_TCL_RFIC_ERROR
*******************************************************************************/
uint8_t CLCardReset(uint8_t *APDUBuffer, uint16_t *APDURecvLen)
{
	
  int8_t    bStatus;
  uint8_t   b;
  uint8_t   bT0;
  uint8_t 	i;
  CLCardInit();  
	
  /* Reset RFIC */
  bStatus = FM320_Initial_ReaderA();
  if(bStatus != CL_TCL_OK)
    return CL_TCL_RFIC_ERROR;

  /* Get ATQA */  
  bStatus =ReaderA_Request(PICC_REQIDL);
  if(bStatus != CL_TCL_OK)
    return CL_TCL_ATQ_ERROR;
  else
  	{
	  stuTCLParam.abATQ[0]=CardA_Sel_Res.ATQA[0];
	  stuTCLParam.abATQ[1]=CardA_Sel_Res.ATQA[1];
	  stuTCLParam.bUIDLevel = (stuTCLParam.abATQ[1]>>6)+1;
  	}

  
  /* Anticollision 1 */
  bStatus=ReaderA_AntiCol(0);
  //bStatus = Mf500PiccCascAnticoll(PICC_ANTICOLL1, 0, &(stuTCLParam.abUID[0]));
  if(bStatus != CL_TCL_OK)
    return CL_TCL_UID_ERROR;
  else
  	{
  		for(i=0;i<4;i++)
			{
				stuTCLParam.abUID[i]=CardA_Sel_Res.UID[i];
			}		
  	}

  /* Selection 1 */
  bStatus=ReaderA_Select(0);
  //bStatus = Mf500PiccCascSelect(PICC_ANTICOLL1, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak));
  if(bStatus != CL_TCL_OK)
    return CL_TCL_UID_ERROR;
  else
  	stuTCLParam.bSak=CardA_Sel_Res.SAK;
  	
  if(stuTCLParam.bUIDLevel > 1)
  {
    /* Anticollision 2 */
	 bStatus=ReaderA_AntiCol(1);
    //bStatus = Mf500PiccCascAnticoll(PICC_ANTICOLL2, 0, &(stuTCLParam.abUID[4]));
    if(bStatus != CL_TCL_OK)
    	return CL_TCL_UID_ERROR;
	else
		{
			for(i=4;i<8;i++)
			{
				stuTCLParam.abUID[i]=CardA_Sel_Res.UID[i];
			}	
		}

    /* Selection 2 */
	bStatus=ReaderA_Select(0);
    //bStatus = Mf500PiccCascSelect(PICC_ANTICOLL2, &(stuTCLParam.abUID[4]), &(stuTCLParam.bSak));
    if(bStatus != CL_TCL_OK)
      	return CL_TCL_UID_ERROR;
	else
		stuTCLParam.bSak=CardA_Sel_Res.SAK;		
  }

  if(stuTCLParam.bUIDLevel > 2)
  {
    /* Anticollision 3 */
		bStatus=ReaderA_AntiCol(2);
    //bStatus = Mf500PiccCascAnticoll(PICC_ANTICOLL3, 0, &(stuTCLParam.abUID[8]));
    if(bStatus != CL_TCL_OK)
      return CL_TCL_UID_ERROR;
	else
		{
			for(i=8;i<12;i++)
			{
				stuTCLParam.abUID[i]=CardA_Sel_Res.UID[i];
			}	
		}

    /* Selection 3 */    
	  bStatus=ReaderA_Select(2);
   // bStatus = Mf500PiccCascSelect(PICC_ANTICOLL3, &(stuTCLParam.abUID[8]), &(stuTCLParam.bSak));
    if(bStatus != CL_TCL_OK)
      return CL_TCL_UID_ERROR; 
	else
		stuTCLParam.bSak=CardA_Sel_Res.SAK;
  }

  /* RATS */  

  bStatus=ReaderA_Rats(0x08,0x00);
  if(bStatus != CL_TCL_OK)
    return CL_TCL_ATS_ERROR;
  else
  	{
  		stuTCLParam.iATSLen=CardA_Sel_Res.ATSLEN;
			for(i=0;i<stuTCLParam.iATSLen;i++)
			{
				APDUBuffer[i]=CardA_Sel_Res.ATS[i];
			}
  	}

  /* Parser ATS*/                  
  memcpy(&(stuTCLParam.abATS[0]), APDUBuffer, stuTCLParam.iATSLen);
  bT0 = (*(APDUBuffer+1));
  stuTCLParam.bFSCI = (bT0&0x0F);
  if(bT0&0x10)
  {
    b = (*(APDUBuffer+2));
    stuTCLParam.bDRC = (b&0x0F);
    stuTCLParam.bDSC = ((b&0xF0)>>4);
  }
  if(bT0&0x20)
  {
    b = (*(APDUBuffer+3));
    stuTCLParam.bSFGI = (b&0x0F);
    stuTCLParam.bFWI = ((b&0xF0)>>4);
    stuTCLParam.lFWT = 1;
    stuTCLParam.lFWT <<= stuTCLParam.bFWI;
  }
  if(bT0&0x40)
  {
    b = (*(APDUBuffer+4));
    if(b&0x01)
      stuTCLParam.bNADEn = 1;
    else
      stuTCLParam.bNADEn = 0;      
    if(b&0x02)
      stuTCLParam.bCIDEn = 1;
    else
      stuTCLParam.bCIDEn = 0;
  }
  /* Information field length */
  // stuTCLParam.bINFLen = 254;
  stuTCLParam.bINFLen = 253;
  // stuTCLParam.bINFLen = 29;
  if(stuTCLParam.bFSCI < 8)
    stuTCLParam.bINFLen = (uint8_t)(FrameSize[stuTCLParam.bFSCI]-3);  // 1 PCB byte + 2 CRC bytes
  stuTCLParam.bINFLen -= stuTCLParam.bNADEn;
  stuTCLParam.bINFLen -= stuTCLParam.bCIDEn;

  /* Orgnize ATR */
  *APDUBuffer = 0x3B;
  *(APDUBuffer+1) = 0x96;
  *(APDUBuffer+2) = 0x11;
  *(APDUBuffer+3) = 0x80;
  *(APDUBuffer+4) = 0x01;
  *(APDUBuffer+5) = 'F';
  *(APDUBuffer+6) = 'M';
  *(APDUBuffer+7) = '3';
  *(APDUBuffer+8) = '4';
  *(APDUBuffer+9) = '9';
  *(APDUBuffer+10) = '6';
  if(stuTCLParam.bDSC&0x04)
    *(APDUBuffer+2) = 0x18;
  else if(stuTCLParam.bDSC&0x02)
    *(APDUBuffer+2) = 0x94;
  else if(stuTCLParam.bDSC&0x01)
    *(APDUBuffer+2) = 0x13;
  *(APDUBuffer+11) = 0;
  for(b=1; b<11; b++)
    *(APDUBuffer+11) ^= *(APDUBuffer+b);
  *APDURecvLen = 12;
  return CL_TCL_OK;
}

/*******************************************************************************
* Function Name  : CLCardPPS.
* Description    : Contactless card PPS process.
* Input          : PPS1 : 0x11 ---- 106kbps
                          0x13 ---- 212kbps
                          0x94 ---- 424kbps
                          0x18 ---- 848kbps
* Output         : None
* Return         : CL_TCL_OK, 
                   CL_TCL_PPS_ERROR                
*******************************************************************************/
uint8_t CLCardPPS(uint8_t PPS1)
{
	uint8_t DRI,DSI;
	uint8_t addr,regdata,res;
	uint8_t Buffer[10];
	
	NFC_DataExTypeDef NFC_DataExStruct;
	
	DRI=PPS1&0x03; 				//PCD->PICC
	DSI=(PPS1&0x0c)>>2;		//PICC->PCD
	
	Buffer[0]=0xD1; 			//PPSS
	Buffer[1]=0x11;      	//PPS0
	Buffer[2]=PPS1;
	NFC_DataExStruct.pExBuf=Buffer; 
	NFC_DataExStruct.nBytesToSend=3;
	NFC_DataExStruct.nBytesReceived=0;
	bStatus=Command_Transceive(&NFC_DataExStruct);
	if(!NFC_DataExStruct.nBytesReceived)
		return CL_TCL_PPS_ERROR;
	if(NFC_DataExStruct.pExBuf[0]==(0xD1))
	{
		addr = TXMODE;
		res = FM320_GetReg(addr,&regdata);//读取TXMODE寄存器数据
		if(res != true)
			return FM175XX_REG_ERR;
		regdata = (regdata&0xcf)|(DRI<<4);
		res = FM320_SetReg(addr,regdata); //设置DR PCD-PICC
		if(res != true)
			return FM175XX_REG_ERR;
		
		addr = RXMODE;
		res = FM320_GetReg(addr,&regdata);//读取RXMODE寄存器数据
		if(res != true)
			return FM175XX_REG_ERR;		
		regdata = (regdata&0xcf)|(DSI<<4);
		res = FM320_SetReg(addr,regdata);  //设置DS  PICC-PCD
		if(res != true)
			return FM175XX_REG_ERR;		
		
		return CL_TCL_OK;	
	}
	else
		return CL_TCL_PPS_ERROR;
}

#define BLOCK_TYPE_I     0x10
#define BLOCK_TYPE_RACK  0x21
#define BLOCK_TYPE_RNAK  0x22
#define BLOCK_TYPE_SWTX  0x31

#define ERROR_COUNTER_MAX 0x03


/*******************************************************************************
* Function Name  : ContactlessCardInitCmd.
* Description    : Contactless card Init APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.
                   APDURecvLen  : How many bytes received from card.
* Output         : None
* Return         : OK 		return 0
                   Failed return 1,                   
*******************************************************************************/
uint8_t ContactlessCardInitCmd(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	//0x30 0x31 0x32 0x33 0x34 0x35 0xA0 0xA1 0xA2	
//	uint8_t 	bCLA =	APDUBuffer[0];
//	uint8_t   bINS =  APDUBuffer[1];
//  uint8_t   bP1 = 	APDUBuffer[2];
//  uint8_t   bP2 = 	APDUBuffer[3];
//  uint16_t  bP3 =   APDUBuffer[4];
//	

	return 0;
	
}

/*******************************************************************************
* Function Name  : CLTCLAPDU.
* Description    : Contactless card APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.          
* Output         : APDURecvLen  : How many bytes received from card.
* Return         : CL_TCL_OK, 
                   CL_TCL_APDU_ERROR                
*******************************************************************************/
uint8_t CLTCLAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
  int8_t  	bStatus;
  uint16_t  iSendOffset = 0;
  uint16_t  iSendLen;
  uint8_t   bINFSize;
  uint8_t   bSendBlockType;
  uint8_t   bWTXValue;
  uint16_t  iRecvOffset = 0;
  uint16_t  iRecvLen;
  uint8_t   b;
  uint16_t  iTemp;
  uint8_t   bErrCnt = 0;
  NFC_DataExTypeDef NFC_DataExStruct;//FM320
	
  /* Orgnize first I-Block */
  bSendBlockType = BLOCK_TYPE_I;
	
	stuTCLParam.bINFLen=254;
	stuTCLParam.bCIDEn=0x01;
	stuTCLParam.bNADEn=0;
//	stuTCLParam.bBlockNum=0;
	
ORGNIZE_BLOCK:
  /* Information field */
                                                                                iSendLen = 1;
  if(stuTCLParam.bCIDEn)
    iSendLen++;
  if(bSendBlockType == BLOCK_TYPE_I)
  {
    if(stuTCLParam.bNADEn)
    iSendLen++;
    bINFSize = stuTCLParam.bINFLen;
    if((APDUSendLen-iSendOffset) > stuTCLParam.bINFLen)
      stuTCLParam.bChain = 1;
    else
    {
      bINFSize = (uint8_t)(APDUSendLen-iSendOffset);
      stuTCLParam.bChain = 0;
    }    
    memcpy(abTPDUBuffer+iSendLen, APDUBuffer+iSendOffset, bINFSize);
    abTPDUBuffer[0] = 0x02;
    if(stuTCLParam.bBlockNum)
      abTPDUBuffer[0] |= 0x01;
    if(stuTCLParam.bNADEn)
      abTPDUBuffer[0] |= 0x04;
    if(stuTCLParam.bCIDEn)
      abTPDUBuffer[0] |= 0x08;
    if(stuTCLParam.bChain)
      abTPDUBuffer[0] |= 0x10;
    /* CID and NAD */
    if(iSendLen == 3)  // CID and NAD
    {
      abTPDUBuffer[1] = 0x01;
      abTPDUBuffer[2] = 0x00;
    }
    else if(iSendLen == 2)
      abTPDUBuffer[1] = 0x01;
  }
  else if(bSendBlockType == BLOCK_TYPE_SWTX)
  {
    abTPDUBuffer[iSendLen] = bWTXValue;
    bINFSize = 1;
    /* PCB and CID */
    abTPDUBuffer[0] = 0xF2;
    if(stuTCLParam.bCIDEn)
    {
      abTPDUBuffer[0] |= 0x08;
      abTPDUBuffer[1] = 0x00;
    }
  }
  else
  {
    /* PCB */
    abTPDUBuffer[0] = 0xA2;
    bINFSize = 0;
    if(stuTCLParam.bBlockNum)
      abTPDUBuffer[0] |= 0x01;
    if(bSendBlockType == BLOCK_TYPE_RNAK)
      abTPDUBuffer[0] |= 0x10;
    if(stuTCLParam.bCIDEn)
      abTPDUBuffer[0] |= 0x08;      
    if(iSendLen == 3)  // CID and NAD
    {
      abTPDUBuffer[1] = 0x01;//abTPDUBuffer[1] = 0x00; modifyed by gxb 2014-10-28 ,ack cid use 1.
      abTPDUBuffer[2] = 0x00;
    }
    else if(iSendLen == 2)
      abTPDUBuffer[1] = 0x00;
  }
  iSendLen += (uint16_t)bINFSize;
  //iSendLen += 2;  // Two CRC bytes.

//  stuTCLParam.lFWT = (1<<14);
//	abTPDUBuffer,abBuffer


	NFC_DataExStruct.pExBuf=abTPDUBuffer;
	NFC_DataExStruct.nBytesToSend=iSendLen;
	bStatus=Command_Transceive(&NFC_DataExStruct);	
 // !bStatus = Mf500PiccExchangeBlock(abTPDUBuffer, iSendLen, abBuffer, &iRecvLen, 1, stuTCLParam.lFWT);
  for(b=0;b<NFC_DataExStruct.nBytesReceived;b++)
	{
		abBuffer[b]=NFC_DataExStruct.pExBuf[b];		
	}
	iRecvLen=NFC_DataExStruct.nBytesReceived;
	
	if(bStatus != CL_TCL_OK)
  {
    bErrCnt++;
    if(bErrCnt >= ERROR_COUNTER_MAX) 
      return CL_TCL_APDU_ERROR;

    bSendBlockType = BLOCK_TYPE_RNAK;
    goto ORGNIZE_BLOCK;
  }    
  
  /* Parser received block  */
  bErrCnt = 0;
  b = abBuffer[0];
  if((b&0xF0) == 0xF0)  // S-Block WTX
  {
    if(b&0x08)
      bWTXValue = abBuffer[2];
    else
      bWTXValue = abBuffer[1];  
    bSendBlockType = BLOCK_TYPE_SWTX;
    goto ORGNIZE_BLOCK;
  }
  else if((b&0xC0) == 0x00) // I-Block
  {
    if((b&0x01) != stuTCLParam.bBlockNum)
      return CL_TCL_APDU_ERROR;
    /* information field */ 
//    iRecvLen -= 2;  // CRC bytes are excluded.
    if(iRecvLen == 0)
      return CL_TCL_APDU_ERROR;      
    iTemp = 1;      // PCB is always
    if(b&0x08)
      iTemp++;
    if(b&0x04)      
      iTemp++;
		//copy received data to ApduBuffer
    memcpy(APDUBuffer+iRecvOffset, abBuffer+iTemp, iRecvLen-iTemp);
    iRecvOffset += (iRecvLen-iTemp);
    /* Change block number */
    stuTCLParam.bBlockNum++;
    stuTCLParam.bBlockNum &= 0x01;
    /* Check chain */
    if(b&0x10)
    {
      bSendBlockType = BLOCK_TYPE_RACK;
      goto ORGNIZE_BLOCK;
    }
  }
  else if((b&0xE0) == 0xA0)
  {
    /* Only RACK could be sent by card */
    if((b&0x01) == stuTCLParam.bBlockNum)
    {
      /* Change block number */
      stuTCLParam.bBlockNum++;
      stuTCLParam.bBlockNum &= 0x01;  
      /* Change offset */
      if(stuTCLParam.bChain)
        iSendOffset += stuTCLParam.bINFLen;
      else
        return CL_TCL_APDU_ERROR;
    }
    
    /* Transmit I Block. If block number is equal, transmit a new block.
       If not, retransmit last block.
    */
    bSendBlockType = BLOCK_TYPE_I;
    goto ORGNIZE_BLOCK;    
  }
  *APDURecvLen = iRecvOffset;
  return CL_TCL_OK;
}

/*******************************************************************************
* Function Name  : CLCardRegInit.
* Description    : Variables of contactless card initialization.
* Input          : None
* Output         : TRUE, FALSE
* Return         : None
*******************************************************************************/
uint8_t CLCardRegInit(void)
{
	u8 res;
	res=FM320_ResetAllReg();//复位FM320所有的寄存器
	FM320_Initial_ReaderA();
	if(res==FALSE)
		return FALSE;
	else 
		return TRUE;
}

/*******************************************************************************
* Function Name  : CLCardPowerOff.
* Description    : Contactless card power off process.
* Input          : None
* Output         : None
* Return         : None                
*******************************************************************************/
void CLCardPowerOff(void)
{
	FM320_POWEROFF;//FM320硬件复位功能
	//FM320_SPD();//启动FM320软件复位功能。
}
