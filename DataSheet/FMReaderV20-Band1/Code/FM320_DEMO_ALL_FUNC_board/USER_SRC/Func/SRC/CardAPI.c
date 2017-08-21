#include "FMReg.h"
#include "CardAPI.h"
#include "BasicApi.h"

uchar Card_Data_Len;
uchar Card_Data_Buf[CARD_DATA_BUF_LEN];

uchar Card_UID_Buf[25] = {0x10,0x04,0x11,0x22,0x33,0x20,
						  0x01,0xFE,
						  0xF0,0xF1,0xF2,0xF3,0xF4,0xF5,
						  0x80,0x81,0x82,0x83,0x84,0x85,0x86,0x87,
						  0xAA,0xBB,0xFE};

uchar LCID = 0;			//卡片的逻辑地址

//*******************************************************
//函数名称：FM175xx_Initial_Card
//函数功能：卡片初始化
//入口参数：NA
//出口参数：0x00 成功   others:失败
//********************************************************
uchar FM175xx_Initial_Card(void)
{
	uchar regdata;
	uchar res;
				
	res = SetReg(CONTROL,0x00);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	res = SetReg(TXMODE,PHCS_BFL_JBIT_TXCRCEN);// Activate CRC
	if(res != True)
		return FM175xx_REG_ERR;
	
	res = SetReg(RXMODE,PHCS_BFL_JBIT_RXCRCEN);// Activate CRC
	if(res != True)
			return FM175xx_REG_ERR;

	regdata = 0x85;			//比较稳定bit7~bit4 MinLevel bit2~bit0 CollLevel:unCare here
	res = SetReg(RXTRESHOLD,regdata);	// MinLevel/ColLevel
	if(res != True)
			return FM175xx_REG_ERR;
		
	regdata = 0x61;
	res = SetReg(DEMOD,regdata);		// DemodReg
	if(res != True)
			return FM175xx_REG_ERR;

	regdata = 0x92;
	res = SetReg(MIFARE,regdata);		// MifareReg
	if(res != True)
			return FM175xx_REG_ERR;

	regdata = 0x2F;
	res = SetReg(GSNLOADMOD,regdata);//调节卡下拉深度：bit7~bit4:未调制时的负载；bit3~bit0:调制时的负载；值越大拉的越深
	if(res != True)
			return FM175xx_REG_ERR;

	regdata = 0x87;					//使用场恢复时钟
	res = SetReg(TXBITPHASE,regdata);
	if(res != True)
			return FM175xx_REG_ERR;

	regdata = 0x59;
	res = SetReg(RFCFG,regdata);		//Bit6~Bit4:RxGain
	if(res != True)
			return FM175xx_REG_ERR;			
	
	res = SetReg(WATERLEVEL,0x20);	//设置FIFOWater Level
	if(res != true)
		return FM175xx_REG_ERR;
		
	res = SetReg(COMMIEN,PHCS_BFL_JBIT_IRQINV|PHCS_BFL_JBIT_RXI|PHCS_BFL_JBIT_ERRI|PHCS_BFL_JBIT_HIALERTI);	//中断为低电平触发
	if(res != true)
		return FM175xx_REG_ERR;
		
	res = SetReg(DIVIEN,PHCS_BFL_JBIT_IRQPUSHPULL);	//
	if(res != true)
		return FM175xx_REG_ERR;
		
	res = ModifyReg(MODE,PHCS_BFL_JBIT_MODEDETOFF,CLR);		//清除ModeDetOff位，启动模式检测
	if(res != True)
			return FM175xx_REG_ERR;
			
	res = ModifyReg(CONTROL,PHCS_BFL_JBIT_INITIATOR,CLR);		//清除Initiator位，进入卡模式
	if(res != True)
			return FM175xx_REG_ERR;

	FM175xx_CurFunc = FM175xx_FUNC_CARD;

	return FM175xx_DEBUG_OK;
}

//*******************************************************
//函数名称：FM175xx_Card_Config
//函数功能：卡片UID配置
//入口参数：configbuf：25Bytes卡片UID缓冲区
//出口参数：0x00 成功   others:失败
//********************************************************
uchar FM175xx_Card_Config(uchar* configbuf)
{
	uchar regdata;
	uchar res;
	uchar i;

	res = SetReg(FIFOLEVEL,PHCS_BFL_JBIT_FLUSHFIFO);	//Flush	FIFO
	if(res != True)
		return FM175xx_REG_ERR;
	
	Write_FIFO(25,configbuf);							//配置信息
	
	res = GetReg(FIFOLEVEL,&regdata);
	if(res != TRUE)
		return FM175xx_REG_ERR;

	if((regdata&0x7F) != 25)
		return FM175xx_FIFO_ERR;

	res = SetReg(COMMAND,CMD_CONFIGURE);
	if(res != True)
			return FM175xx_REG_ERR;

	regdata = 0xFF;
	i = 0; 
	while((regdata & CMD_MASK) && i<100)    //等待CMD变为CMD_IDLE（0x0)
	{
		res = GetReg(COMMAND,&regdata);
		if(res != TRUE)
			return FM175xx_REG_ERR;
		mDelay(1);			//1ms
		i++;
	}
	if(i==100)							    //超时退出
		return FM175xx_CMD_ERR;

	return FM175xx_DEBUG_OK;
}

//*******************************************************
//函数名称：FM175xx_Card_Config
//函数功能：卡片UID配置
//入口参数：configbuf：25Bytes卡片UID缓冲区
//出口参数：0x00 成功   others:失败
//********************************************************
uchar FM175xx_Card_AutoColl(void)
{
	uchar regdata;
	uchar res;

	res = GetReg(COMMAND,&regdata);
	if(res != TRUE)
			return FM175xx_REG_ERR;
	
	regdata = (regdata&CMD_MASK)|CMD_AUTOCOLL;
	res = SetReg(COMMAND,regdata);
	if(res != TRUE)
			return FM175xx_REG_ERR;
		
	return FM175xx_DEBUG_OK;
}

uchar Card_Receive_Data(uchar* dlen,uchar* dbuf)
{
	uchar res;
	uchar i;
	uchar timeout = 0;
	uchar regdata;
	
	*dlen = 0;
	while(1)
	{
		res = GetReg(FIFOLEVEL,&regdata);
		if(res != true)
			return FM175xx_REG_ERR;
		regdata &= 0x7F;
		
		if(regdata == 0)
		{
			timeout++;
			if(timeout>200)
				return false;
		}
		else
		{
			timeout = 0;
			for(i=0;i<regdata;i++)
			{
				res = GetReg(FIFODATA,dbuf+i+(*dlen));
				if(res != true)
					return FM175xx_REG_ERR;
			}
			*dlen += regdata;
		}
		
		res = GetReg(COMMIRQ,&regdata);
		if(res != true)
			return FM175xx_REG_ERR;
		if(regdata & PHCS_BFL_JBIT_RXI)
		{
			res = GetReg(FIFOLEVEL,&regdata);
			if(res != true)
				return FM175xx_REG_ERR;
			regdata &= 0x7F;
			for(i=0;i<regdata;i++)
			{
				res = GetReg(FIFODATA,dbuf+i+(*dlen));
				if(res != true)
					return FM175xx_REG_ERR;
			}
			*dlen += regdata;
			break;
		}
	}
	return true;
}

uchar Card_Data_Transmit(uchar slen,uchar* sbuf)
{
	uchar res;
	uchar tmpSendLen;
	uchar regdata;
	uchar i;
	
	res = SetReg(COMMIRQ,PHCS_BFL_JBIT_TXI);		//清除TXI中断
	if(res != true)
		return FM175xx_REG_ERR;
		
	if(slen >0x40)
	{
		Write_FIFO(0x40,sbuf);
		tmpSendLen = 0x40;
	}
	else
	{
		Write_FIFO(slen,sbuf);
		tmpSendLen = slen;
	}

	res = SetReg(COMMAND,CMD_TRANSMIT);	   			//启动发送
	if(res != true)
		return FM175xx_REG_ERR;
	
	res = GetReg(FIFOLEVEL,&regdata);
	if(res != true)
		return FM175xx_REG_ERR;
	while(tmpSendLen < slen)
	{
		regdata &= 0x3F;
		if(regdata<0x3D)
		{
			res = SetReg(FIFODATA,*(sbuf+tmpSendLen));
			if(res != true)
				return FM175xx_REG_ERR;
			tmpSendLen++;
		}
		res = GetReg(FIFOLEVEL,&regdata);
		if(res != true)
			return FM175xx_REG_ERR;
	}
	
	res = GetReg(COMMIRQ,&regdata);
	if(res != true)
		return FM175xx_REG_ERR;
	while(FM175xx_Card_Processing && (!(regdata&PHCS_BFL_JBIT_TXI)))	//等待发送结束
	{
		res = GetReg(COMMIRQ,&regdata);
		if(res != true)
			return FM175xx_REG_ERR;
	}
	
	return true;
}

uchar Card_Data_Pro(uchar rlen,uchar *rbuf)
{
	uchar res;
	uchar i;
	uchar ppsPara = 0;
	uchar regdata;
	uchar slen;
	
	if((rlen == 0x02) && (*rbuf == 0xE0))			//Rats
	{
		slen = 5;
		LCID = *(rbuf+1)&0x0F;			//-4协议的卡片逻辑地址
		
		*(rbuf+0) = 0x05;
		*(rbuf+1) = 0x72;
		*(rbuf+2) = 0x76;
		*(rbuf+3) = 0x60;
		*(rbuf+4) = 0x02;
	}
	else if((rlen == 0x03) && ((*rbuf & 0xF0) == 0xD0))			//PPS
	{
		if((*rbuf&0x0F) == LCID) //CID PASS
		{
			slen = 1;
			
			if(*(rbuf+1) == 0x11)
			{
				ppsPara = *(rbuf+2);  
			}
		}
		else
		{
			slen = 5;
			*(rbuf+0) = 'I';
			*(rbuf+1) = 'D';
			*(rbuf+2) = 'E';
			*(rbuf+3) = 'r';
			*(rbuf+4) = 'r';
		}
	}
	else
	{
		slen = rlen;
		for(i=1;i<rlen;i++)
		{
			*(rbuf+i) = *(rbuf+i)+1;
		}
	}
	
	res = Card_Data_Transmit(slen,rbuf);
	
	if(ppsPara != 0x0)
	{
		res = GetReg(TXMODE,&regdata);
		if(res != true)
			return FM175xx_REG_ERR;
		 regdata &= 0x8F;
		switch(ppsPara & 0x0C)	//PICC Send Baud
		{
			case 0x04:						//212k
				regdata |= 0x10;
				break;
			case 0x08:						//848k
				regdata |= 0x20;
				break;
			default:
				break;	
		}
		res = SetReg(TXMODE,regdata);
		if(res != true)
			return FM175xx_REG_ERR;
		
		res = GetReg(RXMODE,&regdata);
		if(res != true)
			return FM175xx_REG_ERR;
		 regdata &= 0x8F;
		switch(ppsPara & 0x03)	//PICC Recv Baud
		{
			case 0x01:						//212k
				regdata |= 0x10;
				break;
			case 0x02:						//848k
				regdata |= 0x20;
				break;
			default:
				break;
		}
		res = SetReg(RXMODE,regdata);
		if(res != true)
			return FM175xx_REG_ERR;
	}
	
	return true;
}


uchar FM175xx_Card_Pro(void)
{
	uchar res;
	uint i;
	uchar regdata;
	u8 tmpSendLen;
	u8 ppsPara = 0;
	
	FM175xx_Card_Processing = true;
	
	res = Card_Receive_Data(&Card_Data_Len,Card_Data_Buf);
	if(res != true)
		return FM175xx_RECV_ERR;

	res = ModifyReg(COMMIEN,PHCS_BFL_JBIT_RXI | PHCS_BFL_JBIT_HIALERTI,0);
	if(res != true)
			return FM175xx_REG_ERR;
	
	Card_Data_Pro(Card_Data_Len,Card_Data_Buf);		//数据解析发送
	
	res = SetReg(FIFOLEVEL,0x80);		//Flush FIFO
	if(res != true)
		return FM175xx_REG_ERR;
		
	res = GetReg(COMMIRQ,&regdata);		//Clear IRQ
	if(res != true)
		return FM175xx_REG_ERR;
		
	res = SetReg(COMMIRQ,regdata);
	if(res != true)
		return FM175xx_REG_ERR;
	
   res = SetReg(COMMAND,CMD_RECEIVE);	 //进入接收状态
   if(res != true)
		return FM175xx_REG_ERR;	
		
	FM175xx_Card_Processing = false;
	
	return FM175xx_DEBUG_OK; 
}


void FM17550_Send_ATQA(unsigned char *atqa)
{	
	uchar regdata = 0;
	Write_FIFO(2,atqa);                  //Write_FIFO(2,atqa);
	SetReg(TXMODE,0x00);                 //Write_Reg(TxModeReg,0x00);//0x12 关闭发送CRC
	ModifyReg(MANUALRCV,BIT4,CLR);       //Clear_BitMask(MfRxReg,0x10);//打开奇偶校验
	SetReg(BITFRAMING,0x00);             //Write_Reg(BitFramingReg,0x00);
	SetReg(COMMAND,0x04);                //Write_Reg(CommandReg,Transmit);
	do{
		GetReg(COMMAND,&regdata);
		
	}
	while(regdata == 0x04);              //while(Read_Reg(CommandReg)==Transmit);
	return;		
}
