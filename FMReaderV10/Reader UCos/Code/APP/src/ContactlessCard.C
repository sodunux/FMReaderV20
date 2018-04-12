#include "includes.h"
#include "FM1715.h"
#include "CCID.h"
#include "PICCCmdConst.h"
#include "MfErrNo.h"
#include "ContactlessCard.h"


uint16_t const FrameSize[9] = {16, 24, 32, 40, 48, 64, 96, 128, 256};
uint8_t   bSendBlockType;
TCLParam stuTCLParam;
extern  unsigned char PRTCL;	 			//0表示现在的17寄存器设置为TypeA;1表示TypeB
uint8_t CLcard17regInit(void);
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

	READER_NCS_SET();
	READER_NRD_SET();
	READER_NWR_SET();
	READER_ALE_RESET();
	READER_RST_RESET();        

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
	PRTCL=0;
	CLcard17regInit();
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
					  
	CLCardInit();  
	/* Reset RFIC */
	bStatus = Mf500PcdConfig();
	if(bStatus != MI_OK)
		return CL_TCL_RFIC_ERROR;

	/* Get ATQA */  
	bStatus = Mf500PiccRequest(PICC_REQIDL, &(stuTCLParam.abATQ[0]));
	if(bStatus != MI_OK)
		return CL_TCL_ATQ_ERROR;
	stuTCLParam.bUIDLevel = (stuTCLParam.abATQ[1]>>6)+1;
  
	/* Anticollision 1 */
	bStatus = Mf500PiccCascAnticoll(PICC_ANTICOLL1, 0, &(stuTCLParam.abUID[0]));
	if(bStatus != MI_OK)
		return CL_TCL_UID_ERROR;

	/* Selection 1 */
	bStatus = Mf500PiccCascSelect(PICC_ANTICOLL1, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 1);
	if(bStatus != MI_OK)
		return CL_TCL_UID_ERROR;

	if(stuTCLParam.bUIDLevel > 1)
	{
		/* Anticollision 2 */
		bStatus = Mf500PiccCascAnticoll(PICC_ANTICOLL2, 0, &(stuTCLParam.abUID[4]));
		if(bStatus != MI_OK)
		  	return CL_TCL_UID_ERROR;    

		/* Selection 2 */
		bStatus = Mf500PiccCascSelect(PICC_ANTICOLL2, &(stuTCLParam.abUID[4]), &(stuTCLParam.bSak), 1);
		if(bStatus != MI_OK)
		  	return CL_TCL_UID_ERROR;
	}

	if(stuTCLParam.bUIDLevel > 2)
	{
		/* Anticollision 3 */
		bStatus = Mf500PiccCascAnticoll(PICC_ANTICOLL3, 0, &(stuTCLParam.abUID[8]));
		if(bStatus != MI_OK)
		  	return CL_TCL_UID_ERROR;

		/* Selection 3 */    
		bStatus = Mf500PiccCascSelect(PICC_ANTICOLL3, &(stuTCLParam.abUID[8]), &(stuTCLParam.bSak), 1);
		if(bStatus != MI_OK)
	  		return CL_TCL_UID_ERROR;    
	}

	/* RATS */  
	abTPDUBuffer[0] = 0xE0;
	abTPDUBuffer[1] = 0x80;
	bStatus = Mf500PiccExchangeBlock(abTPDUBuffer, 2, 
	                              APDUBuffer, &(stuTCLParam.iATSLen), 
	                              1, stuTCLParam.lFWT);
	if(bStatus != MI_OK)
		return CL_TCL_ATS_ERROR;

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
	*(APDUBuffer+5) = 'N';
	*(APDUBuffer+6) = 'B';
	*(APDUBuffer+7) = '1';
	*(APDUBuffer+8) = '0';
	*(APDUBuffer+9) = '2';
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
  uint16_t  iTemp;
  uint16_t  PPS_DSI,PPS_DRI;

	abTPDUBuffer[0] = 0xD1;
	abTPDUBuffer[1] = 0x11;
	if(PPS1 < 0x10)				  //修改非接PPS，使pps1从0x00到0x0F都可以切换。	  cnn 20131104
		abTPDUBuffer[2] = PPS1;
	/*if(PPS1 == 0x13)  
	abTPDUBuffer[2] = 0x05;
	else if(PPS1 == 0x94)    
	abTPDUBuffer[2] = 0x0A;    
	else if(PPS1 == 0x18)
	abTPDUBuffer[2] = 0x0F;
	else
	abTPDUBuffer[2] = 0x00;	  */

	bStatus = Mf500PiccExchangeBlock(abTPDUBuffer, 3, 
	                              abAPDUBuffer, &iTemp, 
	                              1, stuTCLParam.lFWT);
	if(bStatus != MI_OK)
		return CL_TCL_PPS_ERROR;
  
	if((iTemp!=1) || (abAPDUBuffer[0] != 0xD1))
		return CL_TCL_PPS_ERROR;
	else
	{	
		PPS_DSI=(PPS1&0x0C)>>2;		  //修改非接PPS，使pps1从0x00到0x0F都可以切换。	 cnn 20131104	
		PPS_DRI=PPS1&0x03;   
		Mf500PcdSetAttrib(PPS_DSI, PPS_DRI);
		/*if(PPS1 == 0x13)
		  Mf500PcdSetAttrib(1, 1);
		else if(PPS1 == 0x94)
		  Mf500PcdSetAttrib(2, 2);
		else if(PPS1 == 0x18)
		  Mf500PcdSetAttrib(3, 3);	  */
	}
	return CL_TCL_OK;
}

#define BLOCK_TYPE_I     0x10
#define BLOCK_TYPE_RACK  0x21
#define BLOCK_TYPE_RNAK  0x22
#define BLOCK_TYPE_SWTX  0x31

#define ERROR_COUNTER_MAX 0x03
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
	int8_t  bStatus;
	uint16_t  iSendOffset = 0;
	uint16_t  iSendLen;
	uint8_t   bINFSize;
	uint8_t   bWTXValue;
	uint16_t  iRecvOffset = 0;
	uint16_t  iRecvLen;
	uint8_t   b;
	uint16_t  iTemp;
	uint8_t   bErrCnt = 0;

	/* Orgnize first I-Block */
	bSendBlockType = BLOCK_TYPE_I; 

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

		/* PCB */

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
	  		abTPDUBuffer[1] =  0x01;
	}
  	else if(bSendBlockType == BLOCK_TYPE_SWTX)
	{
		if(!(abBuffer[0] & 0x08))						// 2014.5.21	Hong, no CID
		{
			abTPDUBuffer[0] = 0xF2;
			iSendLen--;		//iSendLen = 1
			bINFSize = 1;
			abTPDUBuffer[iSendLen] = bWTXValue & (~0xc0);
		}
		else						//has CID
		{
			abTPDUBuffer[iSendLen] = bWTXValue & (~0xc0);			//iSendLen = 2
			bINFSize = 1;
			/* PCB and CID */
			abTPDUBuffer[0] = 0xF2;
			if(stuTCLParam.bCIDEn)
			{
				abTPDUBuffer[0] |= 0x08;
				abTPDUBuffer[1] = abBuffer[1];	//	2013.12.2	Hong		//2015.07.14 CID not forced to 0x00
				if((abBuffer[1] & 0xc0) == 0x80)		// 2014.5.21	Hong,
				{
					iSendLen++;	//iSendLen = 3
					abAPDUBuffer[3] = 0x80;
				}
			}
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
			abTPDUBuffer[1] = 0x01;//abTPDUBuffer[1] = 0x00; modifyed by gxb 2014-10-28 ，ack cid use 1.
			abTPDUBuffer[2] = 0x00;
		}
		else if(iSendLen == 2)
			abTPDUBuffer[1] = 0x01;  //abTPDUBuffer[1] = 0x00; modifyed by gxb 2014-10-28 ，ack cid use 1.
	}
	iSendLen += (uint16_t)bINFSize;
	//iSendLen += 2;  // Two CRC bytes.

	//  stuTCLParam.lFWT = (1<<14);

	bStatus = Mf500PiccExchangeBlock(abTPDUBuffer, iSendLen, abBuffer, &iRecvLen, 1, stuTCLParam.lFWT);
	if(bStatus != MI_OK)
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
* Function Name  : CL17regInit.
* Description    : Variables of contactless card initialization.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
uint8_t CLcard17regInit(void)
{
  if(0==PRTCL)									   //cnn
  {
	WriteRC(0x11,0x5B);
	WriteRC(0x14,0x19);
	WriteRC(0x1A,0x08);
	WriteRC(0x1C,0xFF);
	WriteRC(0x22,0x0F);
	WriteRC(0x23,0x63);
	WriteRC(0x24,0x63);	   
   /* 	 */
  }
  if(1==PRTCL)
  {
	WriteRC(0x11,0x4B);
	WriteRC(0x13,0x06);
	WriteRC(0x14,0x20);
	WriteRC(0x17,0x23);
	WriteRC(0x19,0x73);
	WriteRC(0x1A,0x19);
	WriteRC(0x1C,0x44);
	WriteRC(0x1D,0x1E);
	WriteRC(0x22,0x2C);
	WriteRC(0x23,0xFF);
	WriteRC(0x24,0xFF);	   
  }
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
  PcdRfReset(0);
}
