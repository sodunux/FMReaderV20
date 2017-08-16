#include "includes.h"

uint16_t const FrameSize[9] = {16, 24, 32, 40, 48, 64, 96, 128, 256};



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
		return FM320_PPS(PPS1);
}

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
	TCLParam stuTCLParam;
	
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
	bStatus=FM320_Command_Transceive(&NFC_DataExStruct);	
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
* Function Name  : CLCardPowerOff.
* Description    : Contactless card power off process.
* Input          : None
* Output         : None
* Return         : None                
*******************************************************************************/
void CLCardPowerOff(void)
{
	FM320_POWEROFF;//FM320硬件复位功能
}
