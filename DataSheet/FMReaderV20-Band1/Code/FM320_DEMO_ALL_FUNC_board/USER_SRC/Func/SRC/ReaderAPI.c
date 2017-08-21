/* /////////////////////////////////////////////////////////////////////////////////////////////////
//                     Copyright (c) FMSH
//                       (C)FMSH  2014
//         All rights are reserved. Reproduction in whole or in part is
//        prohibited without the written consent of the copyright owner.
//    Philips reserves the right to make changes without notice at any time.
//   Philips makes no warranty, expressed, implied or statutory, including but
//   not limited to any implied warranty of merchantability or fitness for any
//  particular purpose, or that the use will not infringe any third party patent,
//   copyright or trademark. Philips must not be liable for any loss or damage
//                            arising from its use.
///////////////////////////////////////////////////////////////////////////////////////////////// */

/*! 
 * \file ReaderAPI.c
 * History:
 *  ZQq:  2014.04 Intial Version.
 *
 */

#include "ReaderAPI.h"
#include "BasicAPI.h"



struct TypeACardResponse CardA_Sel_Res;

const uchar RF_CMD_ANTICOL[3] = {0x93,0x95,0x97} ;
const uchar RF_CMD_SELECT[3] = {0x93,0x95,0x97} ;

uchar RF_Data_Len;
uchar RF_Data_Buf[RF_DATA_BUF_LEN];


//***********************************************
//函数名称：FM175xx_Initial_ReaderA
//函数功能：ReaderA初始化
//入口参数：NA
//出口参数：uchar  0：成功   others:失败
//***********************************************
uchar FM175xx_Initial_ReaderA(void)
{
	// debuged 2015.4.3 replace masked below,function become true 
	/*
	uchar addr,mask,set,regdata;
	uchar res;

	addr = STATUS2;
	mask = BIT3;
	set  = 0;
	res = ModifyReg(addr,mask,set);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXMODE;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= BIT7;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RXMODE;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= BIT7;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXCONTROL;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = (regdata | BIT1 |BIT0);
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
		
	addr = MODWIDTH;
	regdata = 0x26;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RXTRESHOLD;
	regdata = 0x55;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXBITPHASE;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & BIT7;
	regdata |= 0x0F;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = GSN;
	regdata = 0xF4;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	addr = CWGSP;
	regdata = 0x3F;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = GSNLOADMOD;
	regdata = 0xF4;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = CONTROL;
	regdata = 0x10;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RFCFG;                //接收器放大倍数和接收灵敏度
	regdata = 0x38;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXAUTO;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata | BIT6 | BIT2 | BIT1 | BIT0;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	return FM175XX_SUCCESS;
	*/
	
	
	/************************************************************************************/
	uchar addr,mask,set,regdata;
	uchar res;

	addr = STATUS2;
	mask = BIT3;
	set  = 0;
	res = ModifyReg(addr,mask,set);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXMODE;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= BIT7;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RXMODE;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= BIT7;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXCONTROL;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = (regdata | BIT1 |BIT0);
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
		
	addr = MODWIDTH;
	regdata = 0x26;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RXTRESHOLD;
	regdata = 0x55;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXBITPHASE;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & BIT7;
	regdata |= 0x0F;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = GSN;
	regdata = 0xF4;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	addr = CWGSP;
	regdata = 0x3F;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = GSNLOADMOD;
	regdata = 0xF4;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = CONTROL;
	regdata = 0x10;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RFCFG;                //接收器放大倍数和接收灵敏度
	regdata = 0x38;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = TXAUTO;
	res = GetReg(addr,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata | BIT6 | BIT2 | BIT1 | BIT0;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;  
	/***********************add for nb1411***********************************/

	addr = TESTSEL2;  //0x32              
	regdata = 0x16;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

		addr = TESTPINEN;     //0x33           
	regdata = 0x3F;         //
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = 0X38;                
	regdata = 0xFF;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	addr = 0X31;                
	regdata = 0x00;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	addr = 0X04;                   //clear all interupts          
	regdata = 0x60;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	addr = 0X02;                
	regdata = 0x40;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	addr = 0X03;                
	regdata = 0x80;
	res = SetReg(addr,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	AUX1_SET(16);
						GPIO_ResetBits(GLED,LED4);
	AUX2_SET(17);
						GPIO_ResetBits(GLED,LED5);
	/***************end of add*****************************/
	return FM175XX_SUCCESS;
}

//***********************************************
//函数名称：FM175xx_Initial_ReaderB
//函数功能：ReaderB初始化
//入口参数：NA
//出口参数：uchar  0：成功   others:失败
//***********************************************
uchar FM175xx_Initial_ReaderB(void)
{
	uchar mask,set,regdata;
	uchar res;

	mask = BIT3;
	set  = 0;
	res = ModifyReg(STATUS2,mask,set);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = GetReg(TXMODE,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= (BIT7|BIT1|BIT0);					//BIT1~0 = 2'b11:ISO/IEC 14443B
	res = SetReg(TXMODE,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = GetReg(RXMODE,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= (BIT7|BIT1|BIT0);					//BIT1~0 = 2'b11:ISO/IEC 14443B
	res = SetReg(RXMODE,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = GetReg(TXAUTO,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & (~(BIT3|BIT7));
	res = SetReg(TXAUTO,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x55;
	res = SetReg(RXTRESHOLD,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = GetReg(TXBITPHASE,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata = regdata & BIT7;
	regdata |= 0x0F;
	res = SetReg(TXBITPHASE,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
				    
	regdata = 0xF4;
	res = SetReg(GSN,regdata);  //☆☆☆☆☆低四位改变调制深度，数值越小，调制深度越小；
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0xF3;
	res = SetReg(GSNLOADMOD,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x10;
	res = SetReg(CONTROL,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
               
	regdata = 0x48;
	res = SetReg(RFCFG,regdata);//接收器放大倍数和接收灵敏度
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = GetReg(TXAUTO,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	regdata &= ~BIT6;				//☆☆☆☆☆Disable  Force100ASK
	regdata = regdata | BIT2 | BIT1 | BIT0;
	res = SetReg(TXAUTO,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	return FM175XX_SUCCESS;
}

//***********************************************
//函数名称：FM175xx_Initial_ReaderF
//函数功能：ReaderF初始化
//入口参数：NA
//出口参数：uchar  0：成功   others:失败
//***********************************************
uchar FM175xx_Initial_ReaderF(void)
{
	uchar addr,regdata;
	uchar res;
	
	res = SetReg(CONTROL,PHCS_BFL_JBIT_INITIATOR);
	if(res != True)
		return FM175XX_REG_ERR;
	
	addr = TXMODE;
	regdata = 0x92;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = RXMODE;
	regdata = 0x96;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = RXTRESHOLD;
	regdata = 0x55;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = DEMOD;
	regdata = 0x41;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = RFCFG;
	regdata = 0x69;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = GSN;
	regdata = 0xF3;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = CWGSP;
	regdata = 0x3F;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = MODGSP;
	regdata = 0x11;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	addr = TXAUTO;
	regdata = 0x07;
	res = SetReg(addr,regdata);
	if(res != True)
			return FM175XX_REG_ERR;

	return FM175XX_SUCCESS;
}

//***********************************************
//函数名称：ReaderA_Request
//函数功能：ReqA
//入口参数：Type：ReqA：0x26 WupA：0x52
//出口参数：uchar  0：成功   others:失败
//***********************************************
uchar ReaderA_Request(uchar type)
{
		uchar regdata;
	uchar res;
	uchar irq;
	uchar dly = 0;
	
	res = ModifyReg(STATUS2,BIT3,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(MODE,BIT6,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x80;
	res = SetReg(COLL,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(TXMODE,BIT7,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(RXMODE,BIT7,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(RXMODE,BIT3,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = CMD_TRANSCEIVE;
	res = SetReg(COMMAND,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

//	res = ModifyReg(COMMIEN,BIT5,SET);//RXI/TIMERI/ERRI
//	if(res != TRUE)
//		return FM175XX_REG_ERR;
	irq = BIT5; //BIT5|BIT0|BIT1;

	regdata = 0x80;
	res = SetReg(FIFOLEVEL,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = type;
	res = SetReg(FIFODATA,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x87;				//BIT7:STARTSEND Bit2~0 发送7bits
	res = SetReg(BITFRAMING,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0;

	while(!(regdata & irq) && dly<100)
	{
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
		{
			return FM175XX_REG_ERR;
		}
		mDelay(10);			//0.1ms
		dly++;
	}
	
//	res = ModifyReg(COMMIEN,BIT5|BIT0|BIT1,CLR);//清零中断使能
//	if(res != TRUE)
//		return FM175XX_REG_ERR;

	res = GetReg(FIFOLEVEL,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	if(regdata == 2)
	{
		res = GetReg(FIFODATA,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		CardA_Sel_Res.ATQA[0] = regdata;

		res = GetReg(FIFODATA,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		CardA_Sel_Res.ATQA[1] = regdata;
	}
	else
	{
		regdata = 0x7F;                                    //added for nb1411
		res = SetReg(COMMIRQ,regdata);                     //
		if(res != TRUE)                                    //
				return FM175XX_REG_ERR;                            //added for nb1411
		return FM175XX_RECV_TIMEOUT;
	}
	regdata = 0x7F;
	res = SetReg(COMMIRQ,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(FIFOLEVEL,BIT7,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	return FM175XX_SUCCESS;
	/*
	uchar regdata;
	uchar res;
	uchar irq;
	uchar dly = 0;
	
	res = ModifyReg(STATUS2,BIT3,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(MODE,BIT6,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x80;
	res = SetReg(COLL,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(TXMODE,BIT7,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(RXMODE,BIT7,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(RXMODE,BIT3,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = CMD_TRANSCEIVE;
	res = SetReg(COMMAND,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

//	res = ModifyReg(COMMIEN,BIT5|BIT0|BIT1,SET);//RXI/TIMERI/ERRI
//	if(res != TRUE)
//		return FM175XX_REG_ERR;
	irq = BIT5;

	regdata = 0x80;
	res = SetReg(FIFOLEVEL,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = type;
	res = SetReg(FIFODATA,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x87;				//BIT7:STARTSEND Bit2~0 发送7bits
	res = SetReg(BITFRAMING,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0;
	mDelay(300);       //for nb1411
	
	while(!(regdata & irq) && dly<100)
	{
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		mDelay(10);			//0.1ms
		dly++;
	}
	
//	res = ModifyReg(COMMIEN,BIT5|BIT0|BIT1,CLR);//清零中断使能
//	if(res != TRUE)
//		return FM175XX_REG_ERR;

	res = GetReg(FIFOLEVEL,&regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	if(regdata == 2)
	{
		res = GetReg(FIFODATA,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		CardA_Sel_Res.ATQA[0] = regdata;

		res = GetReg(FIFODATA,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		CardA_Sel_Res.ATQA[1] = regdata;
	}
	else
	{
		
		
		regdata = 0x6F;
		res = SetReg(COMMIRQ,regdata);
		if(res != TRUE)
		return FM175XX_REG_ERR;
		
		return FM175XX_RECV_TIMEOUT;
	}
	regdata = 0x6F;
	res = SetReg(COMMIRQ,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(FIFOLEVEL,BIT7,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	return FM175XX_SUCCESS;
	*/
}

//*************************************
//函数  名：ReaderA_AntiCol
//入口参数：size:防冲突等级，包括0、1、2
//出口参数：uchar:0:OK  others：错误
//*************************************
uchar ReaderA_AntiCol(uchar size)
{
	uchar regdata;
	uchar res;
	uchar i;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;

	res = ModifyReg(TXMODE,PHCS_BFL_JBIT_TXCRCEN,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(RXMODE,PHCS_BFL_JBIT_RXCRCEN,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	regdata = 0x00;
	res = SetReg(COLL,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;
				
	regdata = 0x00;				//8Bits数据全发
	res = SetReg(BITFRAMING,regdata);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	if(size>2)
		size = 0;
	*(NFC_DataExStruct.pExBuf+0) = RF_CMD_ANTICOL[size];   //防冲突等级
	*(NFC_DataExStruct.pExBuf+1) = 0x20;
	NFC_DataExStruct.nBytesToSend = 2;						//发送长度：2

	res = Command_Transceive(&NFC_DataExStruct);

	res = SetReg(COLL,0x80);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	if((res != True)||(NFC_DataExStruct.nBytesReceived != 0x5))
		return FM175XX_FRAME_ERR;
	res = 0;
	for(i=0;i<4;i++)
	{
		CardA_Sel_Res.UID[size*3+i] = *(NFC_DataExStruct.pExBuf+i) ;
		res ^= *(NFC_DataExStruct.pExBuf+i);
	}
	if(res != *(NFC_DataExStruct.pExBuf+4))	//BCC
		return  FM175XX_BCC_ERR;

	return FM175XX_SUCCESS;
}

//*************************************
//函数  名：ReaderA_Select
//入口参数：size:防冲突等级，包括0、1、2
//出口参数：uchar:0:OK  others：错误
//*************************************
uchar ReaderA_Select(uchar size)
{
	uchar addr,mask,set/*,regdata*/;
	uchar res;
	uchar i;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	
	addr = TXMODE;
	mask = BIT7;
	set  = 1;
	res = ModifyReg(addr,mask,set);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	addr = RXMODE;
	mask = BIT7;
	set  = 1;
	res = ModifyReg(addr,mask,set);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	if(size>2)							 //最多三重防冲突
		size = 0;
	*(NFC_DataExStruct.pExBuf+0) = RF_CMD_ANTICOL[size];   //防冲突等级
	*(NFC_DataExStruct.pExBuf+1) = 0x70;   					//
	NFC_DataExStruct.nBytesToSend = 7;

	*(NFC_DataExStruct.pExBuf+2) = CardA_Sel_Res.UID[3*size];
	*(NFC_DataExStruct.pExBuf+3) = CardA_Sel_Res.UID[3*size+1];
	*(NFC_DataExStruct.pExBuf+4) = CardA_Sel_Res.UID[3*size+2];
	*(NFC_DataExStruct.pExBuf+5) = CardA_Sel_Res.UID[3*size+3];
	*(NFC_DataExStruct.pExBuf+6) = 0;
	for(i=2;i<6;i++)				 //BCC
	{
		*(NFC_DataExStruct.pExBuf+6) ^= *(NFC_DataExStruct.pExBuf+i);
	}


	res = Command_Transceive(&NFC_DataExStruct);
	if(NFC_DataExStruct.nBytesReceived == 1)
		CardA_Sel_Res.SAK = *(NFC_DataExStruct.pExBuf);
	else
		return FM175XX_TRANS_ERR;
		
	return FM175XX_SUCCESS;
}

//*************************************
//函数  名：ReaderA_Rats
//入口参数：FSDI：Reader的帧长度最大值：2^x；CID:0~14,Card的逻辑地址；
//出口参数：uchar:0:OK  others：错误
//*************************************
uchar ReaderA_Rats(uchar FSDI,uchar CCID)
{
	uchar addr,mask,set/*,regdata*/;
	uchar res;
	uchar i;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	
	*(NFC_DataExStruct.pExBuf+0) = 0xE0;
	*(NFC_DataExStruct.pExBuf+1) = ((FSDI&0x0F)<<4) + (CCID&0x0F);   					//
	NFC_DataExStruct.nBytesToSend = 2;

	res = Command_Transceive(&NFC_DataExStruct);
	return res;
}

uchar GetKey(uchar* Key)
{
	uchar addr,mask,set/*,regdata*/;
	uchar res;
	uchar i;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	//02 00 a4 04 00 05 f2 22 22 22 22 
	*(NFC_DataExStruct.pExBuf+0) = 0x02;
	*(NFC_DataExStruct.pExBuf+1) = 0x00;   					//
	*(NFC_DataExStruct.pExBuf+2) = 0xA4;   					//
	*(NFC_DataExStruct.pExBuf+3) = 0x04;   					//
	*(NFC_DataExStruct.pExBuf+4) = 0x00;   					//
	*(NFC_DataExStruct.pExBuf+5) = 0x05;   					//
	*(NFC_DataExStruct.pExBuf+6) = 0xF2;   					//
	*(NFC_DataExStruct.pExBuf+7) = 0x22;   					//
	*(NFC_DataExStruct.pExBuf+8) = 0x22;   					//
	*(NFC_DataExStruct.pExBuf+9) = 0x22;   					//
	*(NFC_DataExStruct.pExBuf+10) = 0x22;   				//
	NFC_DataExStruct.nBytesToSend = 11;

	res = Command_Transceive(&NFC_DataExStruct);
	if(NFC_DataExStruct.nBytesReceived == 0)
	{
		return False;
	}
	if((NFC_DataExStruct.nBytesReceived==2)&&(*(NFC_DataExStruct.pExBuf+0) == 0xF2)&&(*(NFC_DataExStruct.pExBuf+1) == 0x01))
	{
		uDelay(10);
		NFC_DataExStruct.nBytesToSend = 2;
		res = Command_Transceive(&NFC_DataExStruct);
	}
	if(NFC_DataExStruct.nBytesReceived == 7)
	{
		for(i=0;i<4;i++)
		{
			*(Key+i) = *(NFC_DataExStruct.pExBuf+i+1);
		}
		return True;
	}
	
	return False;
}
//*************************************
//函数  名：ReaderF_Polling
//入口参数：tsn:timer slot; polling buffer:卡回发的数据
//出口参数：uchar:0:OK  others：错误
//*************************************
uchar ReaderF_Polling(uchar tsn,uchar* pollingbufer)
{
	uchar res,i;
	uchar databuf[0x13] = {0x06,0x00,0xFF,0xFF,0x00,0x00};
	NFC_DataExTypeDef NFC_DataExStruct;
	
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	NFC_DataExStruct.nBytesToSend = 6;
	for(i=0;i<NFC_DataExStruct.nBytesToSend;i++)
	{
		*(NFC_DataExStruct.pExBuf+i) = *(databuf+i);
	}
	
	if((tsn != 0x00) && (tsn != 0x01) && (tsn != 0x03) && (tsn != 0x07) && (tsn != 0x0F))
		return False;				//tsn不符合协议规范
	else
		*(NFC_DataExStruct.pExBuf+5) = tsn;
	
	res = Command_Transceive(&NFC_DataExStruct);
	if(res != FM175XX_SUCCESS)
		return FM175XX_TRANS_ERR;
	else if(NFC_DataExStruct.nBytesReceived != 0x13)
		return FM175XX_FRAME_ERR;
	
	for(i=0;i<NFC_DataExStruct.nBytesReceived;i++)
	{
		*(pollingbufer+i) = *(NFC_DataExStruct.pExBuf+i);
	}

	return FM175XX_SUCCESS;
}

//*************************************
//函数  名：ReaderF_ReqResponse
//入口参数：nfcid2:目标卡的nfcid2（8Bytes）
//出口参数：uchar:0:OK  others：错误
//*************************************
uchar ReaderF_ReqResponse(uchar* fnfcid)
{
	uchar res;

	uchar i;
	uchar index;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	
	index = 1;
	*(NFC_DataExStruct.pExBuf+index) = 0x04;		 //CMD
	index++;
	for(i=0;i<8;i++)
	{
		*(NFC_DataExStruct.pExBuf+index) = *(fnfcid+i);
		index++;
	}
	*NFC_DataExStruct.pExBuf = index;              //LEN
	NFC_DataExStruct.nBytesToSend  = index;

	res = Command_Transceive(&NFC_DataExStruct);
	if(res != FM175XX_SUCCESS)
		return FM175XX_TRANS_ERR;

	if((NFC_DataExStruct.pExBuf[0] != 0xB) || (*(NFC_DataExStruct.pExBuf+1) != 0x5))
		return FM175XX_FRAME_ERR;
	else
	{
		for(i=0;i<8;i++)
		{
			if(*(NFC_DataExStruct.pExBuf+i+2) != *(fnfcid+i))
			{
				return FM175XX_FRAME_ERR;
			}
		}
	}

	return FM175XX_SUCCESS;
}



