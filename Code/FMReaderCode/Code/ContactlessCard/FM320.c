#include "FM320.h"

const u8 RF_CMD_ANTICOL[3] = {0x93,0x95,0x97} ;
const u8 RF_CMD_SELECT[3] = {0x93,0x95,0x97} ;
struct TypeACardResponse CardA_Sel_Res;

u8 RF_Data_Buf[RF_DATA_BUF_LEN];

void FM320_mDelay(u16 ms)			//实测1ms
{
	u32	i;
	u16 j;
	for(j=0;j<ms;j++)
	{
		for(i=0;i<8100;i++)
			;
	}
}

void FM320_uDelay(u16 us)			//实测1us
{
	u32	i;
	u16 j;
	for(j=0;j<us;j++)
	{
		for(i=0;i<81;i++)
			;
	}
}


void FM320_SPIConfig(void)
{
  SPI_InitTypeDef 		SPI_InitStructure;
	GPIO_InitTypeDef   	GPIO_InitStructure;	
	//enable the SPI Clock
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_SPI1|RCC_APB2Periph_GPIOA|RCC_APB2Periph_AFIO,ENABLE);
	//config SPI GPIO
	//FM320_NRSTPD
	GPIO_InitStructure.GPIO_Pin = FM320_NRSTPD;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;
  GPIO_Init(FM320_NRSTPD_PORT, &GPIO_InitStructure);//NRSTPD
	//FM320_IRQ
	GPIO_InitStructure.GPIO_Pin = FM320_IRQ;
	GPIO_InitStructure.GPIO_Mode=GPIO_Mode_IN_FLOATING;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;
	GPIO_Init(FM320_IRQ_PORT,&GPIO_InitStructure);//FM320_IRQ
	
	// Configure SPI1 NSS (PA.4)、SCK(PA.5)、MISO(PA.6)、MOSI(PA.7) as alternate function push-pull  
  GPIO_InitStructure.GPIO_Pin = FM320_MISO | FM320_MOSI | FM320_SCK; 
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz; 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP; 
	GPIO_Init(FM320_SCK_PORT, &GPIO_InitStructure);  
	//	FM320_NSS 
  GPIO_InitStructure.GPIO_Pin = FM320_NSS;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz; 
  GPIO_Init(FM320_NSS_PORT, &GPIO_InitStructure);

  /* SPI1 Config: SPI1 Master*/
  SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
  SPI_InitStructure.SPI_Mode = SPI_Mode_Master;
  SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;
  SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;
  SPI_InitStructure.SPI_CPHA = SPI_CPHA_1Edge;
  SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;
  SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_16;
  SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;
  SPI_InitStructure.SPI_CRCPolynomial = 7;
  SPI_Init(SPI1, &SPI_InitStructure);
  /* Enable SPI1 */
	FM320_NSS_CLR;
	FM320_POWERON;
  SPI_Cmd(SPI1, ENABLE);
}

//***********************************************
//函数名称：FM320_Get_Reg(u8 addr,u8* regdata)
//函数功能：通过SPI接口读寄存器
//入口参数：addr:目标寄存器地址   regdata:读取的值
//出口参数：u8  true：读取成功   false:失败
//***********************************************
bool FM320_GetReg(u8 addr,u8* regdata)
{	
	u16 i;

	FM320_NSS_CLR;
	__nop();__nop();
	
	addr = addr << 1;	 	   
	addr |= 0x80;
	SPI_I2S_SendData(SPI1,addr);   /* Send SPI1 data */ 
		
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) != RESET)
		{
			SPI_I2S_ClearFlag(SPI1,SPI_I2S_FLAG_RXNE);	
			*regdata = SPI_I2S_ReceiveData(SPI1);		 /* Read SPI1 received data */
			break;
		}
		FM320_uDelay(1);
	}
	if(i >= SPI_TIME_OUT)
	{
		FM320_NSS_SET;
		return false;
	}
	
	SPI_I2S_SendData(SPI1,0x00);   /* Send SPI1 data */
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) != RESET)
		{
			SPI_I2S_ClearFlag(SPI1,SPI_I2S_FLAG_RXNE);	
			*regdata = SPI_I2S_ReceiveData(SPI1);		 /* Read SPI1 received data */
			FM320_NSS_SET;
			return true;
		}
		FM320_uDelay(1);
	}
		
	FM320_NSS_SET;
	return false;
}

//***********************************************
//函数名称：FM320_Set_Reg(u8 addr,u8* regdata)
//函数功能：写寄存器
//入口参数：addr:目标寄存器地址   regdata:要写入的值
//出口参数：u8  true：写成功   false:写失败
//***********************************************
bool FM320_SetReg(u8 addr,u8 regdata)
{
	
	u16 i;
	u8 idat = 0;
	
	FM320_NSS_CLR;
	__nop();__nop();
	
	addr = addr << 1;	 	   
	addr &= 0x7F;
	SPI_I2S_SendData(SPI1,addr);		/* Send SPI1 data */ 
	
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) != RESET)
		{
			SPI_I2S_ClearFlag(SPI1,SPI_I2S_FLAG_RXNE);	
			idat = SPI_I2S_ReceiveData(SPI1);		 /* Read SPI1 received data */
			break;
		}
		FM320_uDelay(1);
	}
	if(i >= SPI_TIME_OUT)
	{
		FM320_NSS_SET;
		return false;
	}
	
	SPI_I2S_SendData(SPI1,regdata);		/* Send SPI1 data */
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) != RESET)
		{
			SPI_I2S_ClearFlag(SPI1,SPI_I2S_FLAG_RXNE);	
			idat = SPI_I2S_ReceiveData(SPI1);		 /* Read SPI1 received data */
			break;
		}
		FM320_uDelay(1);
	}
	if(i >= SPI_TIME_OUT)
	{
		FM320_NSS_SET;
		return false;
	}
	
	FM320_NSS_SET;
	return true;
}

//*******************************************************
//函数名称：FM320_ModifyReg(u8 addr,u8* mask,u8 set)
//函数功能：写寄存器
//入口参数：addr:目标寄存器地址   mask:要改变的位  
//         set:  0:标志的位清零   其它:标志的位置起
//出口参数：u8  true：写成功   false:写失败
//********************************************************
bool FM320_ModifyReg(u8 addr,u8 mask,u8 set)
{
	u8 res;
	u8 regdata;
	
	res = FM320_GetReg(addr,&regdata);
	if(res == true)
	{
		if(set)
		{
			regdata |= mask;
		}
		else
		{
			regdata &= ~mask;
		}
	}
	else
		return res;

	res = FM320_SetReg(addr,regdata);
	return res;
}

//***********************************************
//函数名称：FM320_PPS
//函数功能：PPS操作
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_PPS(u8 PPS1)
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
	res = FM320_ModifyReg(TXMODE,PHCS_BFL_JBIT_TXCRCEN,SET);
	if(res != true)
		return FM175XX_REG_ERR;
	res = FM320_ModifyReg(RXMODE,PHCS_BFL_JBIT_RXCRCEN,SET);
	if(res != true)
		return FM175XX_REG_ERR;
	
	bStatus=FM320_Command_Transceive(&NFC_DataExStruct);
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


//***********************************************
//函数名称：FM320_Authent
//函数功能：认证Mifare卡
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_Authent(MiAuthTpeDef miauth)
{
	u8 res,regdata;
	u8 fifo[12];
	res = FM320_SetReg(FIFOLEVEL,PHCS_BFL_JBIT_FLUSHFIFO);	//FlushFIFO
	if(res != true)
		return FM175XX_REG_ERR;
	fifo[0]=miauth.keytype;
	fifo[1]=miauth.block;
	fifo[2]=miauth.key[0];
	fifo[3]=miauth.key[1];
	fifo[4]=miauth.key[2];
	fifo[5]=miauth.key[3];
	fifo[6]=miauth.key[4];	
	fifo[7]=miauth.key[5];	
	fifo[8]=miauth.UID[0];
	fifo[9]=miauth.UID[1];	
	fifo[10]=miauth.UID[2];
	fifo[11]=miauth.UID[3];
	res = FM320_Write_FIFO(12,fifo);
	if(res != true)
		return FM175XX_REG_ERR;	
	res = FM320_SetReg(COMMAND,CMD_AUTHENT);
	if(res != true)
		return FM175XX_REG_ERR;	
	FM320_mDelay(10);
	res=FM320_GetReg(STATUS2,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;	
	if(!(regdata&BIT3))
	{
		FM320_SetReg(COMMAND,CMD_IDLE);
		return FM175XX_REG_ERR;
	}
	else
		return FM175XX_SUCCESS;	
}

//***********************************************
//函数名称：FM320_MiRead
//函数功能：Mifare Read
//入口参数：block号；读出的数据(长度要大于 16)
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_MiRead(u8 block,u8 *readbuf)
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0x30;
	fifo[1]=block;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(NFC_DataExStruct.nBytesReceived==0)
		return FM175XX_REG_ERR;
	for(i=0;i<16;i++)
	{
		readbuf[i]=fifo[i];
	}
	return FM175XX_SUCCESS;	
}

//***********************************************
//函数名称：FM320_MiWrite
//函数功能：Mifare Read
//入口参数：block号；写入的数据(长度要大于 16)
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_MiWrite(u8 block,u8 *writebuf)
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0xA0;
	fifo[1]=block;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(!NFC_DataExStruct.nBytesReceived)
		return FM175XX_REG_ERR;
	else 
	{
		NFC_DataExStruct.pExBuf =	writebuf;
		NFC_DataExStruct.nBytesToSend	  =	0x10;
		NFC_DataExStruct.nBytesReceived = 0x00;
		FM320_Command_Transceive(&NFC_DataExStruct);
		if(NFC_DataExStruct.pExBuf[0]!=0x0A)
			return FM175XX_REG_ERR;
		else 
			return FM175XX_SUCCESS;
	}
	
}

//***********************************************
//函数名称：FM320_MiIncrement
//函数功能：Increment
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_MiIncrement(u8 block,u32 incdat)
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0xC1;
	fifo[1]=block;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(!NFC_DataExStruct.nBytesReceived)
		return FM175XX_REG_ERR;
	else 
	{
		fifo[0]=(u8)incdat;
		fifo[1]=(u8)(incdat>>8);
		fifo[2]=(u8)(incdat>>16);
		fifo[3]=(u8)(incdat>>24);		
		NFC_DataExStruct.pExBuf =	fifo;
		NFC_DataExStruct.nBytesToSend	  =	0x04;
		NFC_DataExStruct.nBytesReceived = 0x00;
		FM320_Command_Transceive(&NFC_DataExStruct);
		return FM175XX_SUCCESS;
	}
}


//***********************************************
//函数名称：FM320_MiDecrement
//函数功能：FM320_MiDecrement
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_MiDecrement(u8 block,u32 decdat)
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0xC0;
	fifo[1]=block;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(!NFC_DataExStruct.nBytesReceived)
		return FM175XX_REG_ERR;
	else 
	{
		fifo[0]=(u8)decdat;
		fifo[1]=(u8)(decdat>>8);
		fifo[2]=(u8)(decdat>>16);
		fifo[3]=(u8)(decdat>>24);		
		NFC_DataExStruct.pExBuf =	fifo;
		NFC_DataExStruct.nBytesToSend	  =	0x04;
		NFC_DataExStruct.nBytesReceived = 0x00;
		FM320_Command_Transceive(&NFC_DataExStruct);
		return FM175XX_SUCCESS;
	}
	
}
//***********************************************
//函数名称：FM320_MiRestore
//函数功能：FM320_MiRestore
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_MiRestore(u8 block)
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0xC2;
	fifo[1]=block;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(!NFC_DataExStruct.nBytesReceived)
		return FM175XX_REG_ERR;
	else 
	{
		fifo[0]=0x00;
		fifo[1]=0x00;
		fifo[2]=0x00;
		fifo[3]=0x00;		
		NFC_DataExStruct.pExBuf =	fifo;
		NFC_DataExStruct.nBytesToSend	  =	0x04;
		NFC_DataExStruct.nBytesReceived = 0x00;
		FM320_Command_Transceive(&NFC_DataExStruct);
		return FM175XX_SUCCESS;
	}
}

//***********************************************
//函数名称：FM320_MiTransfer
//函数功能：FM320_MiTransfer
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_MiTransfer(u8 block)
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0xB0;
	fifo[1]=block;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(NFC_DataExStruct.pExBuf[0]!=0x0A)
		return FM175XX_REG_ERR;
	else 
		return FM175XX_SUCCESS;
}



//***********************************************
//函数名称：ReadeA_HALT
//函数功能：发送HALT命令
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_ReaderA_HALT()
{
	u8 fifo[16],i;
	NFC_DataExTypeDef NFC_DataExStruct;
	fifo[0]=0x50;
	fifo[1]=0x00;
	NFC_DataExStruct.pExBuf =	fifo;
	NFC_DataExStruct.nBytesToSend	  =	0x02;
	NFC_DataExStruct.nBytesReceived = 0x00;
	FM320_Command_Transceive(&NFC_DataExStruct);
	if(NFC_DataExStruct.nBytesReceived)
		return FM175XX_REG_ERR;
	else 
		return FM175XX_SUCCESS;
}



//***********************************************
//函数名称：FM320_RateSel
//函数功能：修改发送和接受速率
//入口参数：txrata 发送速率，rxrate 接受速率
//					0:106K;1:212K;2:424K;3:848K;
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_RateSel(u8 txrate,u8 rxrate)
{
	u8 regdata;
	u8 res;
	if((txrate<4)&&(rxrate<4))
	{
		res=FM320_GetReg(TXMODE,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		regdata=(regdata&0x8F)|(txrate<<4);
		res=FM320_SetReg(TXMODE,regdata);
		if(res != true)
			return FM175XX_REG_ERR;		
		
		res=FM320_GetReg(RXMODE,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		regdata=(regdata&0x8F)|(rxrate<<4);
		res=FM320_SetReg(RXMODE,regdata);
		if(res != true)
			return FM175XX_REG_ERR;				
		
		return FM175XX_SUCCESS;
	}
	else 
		return FM175XX_TRANS_ERR;
	
}

//***********************************************
//函数名称：FM320_Enter_Idle
//函数功能：进入IDLE(取消当前命令)
//入口参数：NA
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_Enter_Idle()
{
	u8 regdata,res;
	
	res=FM320_GetReg(COMMAND,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata=regdata&0xF0;
	res=FM320_SetReg(COMMAND,regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	return FM175XX_SUCCESS;
}

//***********************************************
//函数名称：FM175xx_Initial_ReaderA
//函数功能：ReaderA初始化
//入口参数：NA
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_Initial_ReaderA(void)
{
	u8 addr,mask,set,regdata;
	u8 res;

	addr = STATUS2;
	mask = BIT3;
	set  = 0;
	res = FM320_ModifyReg(addr,mask,set); //关闭加解密开关
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXMODE;
	res = FM320_GetReg(addr,&regdata);//读取TXMODE寄存器数据
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);	//保留TxMix和InvMod的数据
	regdata |= BIT7;									//Enable CRC generation 
	res = FM320_SetReg(addr,regdata);	//106K并且在模式Mifare(ISO14443协议)
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RXMODE;
	res = FM320_GetReg(addr,&regdata);//读取RXMODE寄存器数据
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);	//保持自动忽略非法数据帧和自动重复接受配置
	regdata |= BIT7;
	res = FM320_SetReg(addr,regdata); //允许接受的时候进行CRC运算
	FM320_GetReg(addr,&regdata);	
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXCONTROL;
	res = FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
//	regdata=0x80;
	regdata = (regdata | BIT1 |BIT0);//TX1和TX2的PIN脚上输出13.56M的载波
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
		
	addr = MODWIDTH;								//设置调制宽度
	regdata = 0x26;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RXTRESHOLD; //设置最低的解调电平
	regdata = 0x55;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXBITPHASE;	//设置
	res = FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & BIT7;	//保持解调时钟的驱动源
	regdata |= 0x0F;					//设置发送前的等待时间
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = GSN;//设置天线电感参数
	regdata = 0xF4;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	
	addr = CWGSP;//设置天线电感参数
	regdata = 0x3F;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = GSNLOADMOD;   //设置NDriver天线参数
	regdata = 0xF4;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = CONTROL;
	regdata = 0x10;
	res = FM320_SetReg(addr,regdata);//FM320作为发起者
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RFCFG;                //接收器放大倍数和接收灵敏度
	regdata = 0x38;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXAUTO;	//设置天线参数
	res = FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata | BIT6 | BIT2 | BIT1 | BIT0;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;  
	

	return FM175XX_SUCCESS;
}

//***********************************************
//函数名称：FM320_Initial_ReaderB
//函数功能：ReaderB初始化
//入口参数：NA
//出口参数：u8  true：成功   false:失败
//***********************************************
u8 FM320_Initial_ReaderB(void)
{
	u8 mask,set,regdata;
	u8 res;

	mask = BIT3;
	set  = 0;
	res = FM320_ModifyReg(STATUS2,mask,set);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_GetReg(TXMODE,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= (BIT7|BIT1|BIT0);					//BIT1~0 = 2'b11:ISO/IEC 14443B
	res = FM320_SetReg(TXMODE,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_GetReg(RXMODE,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);
	regdata |= (BIT7|BIT1|BIT0);					//BIT1~0 = 2'b11:ISO/IEC 14443B
	res = FM320_SetReg(RXMODE,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_GetReg(TXAUTO,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (~(BIT3|BIT7));
	res = FM320_SetReg(TXAUTO,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0x55;
	res = FM320_SetReg(RXTRESHOLD,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_GetReg(TXBITPHASE,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & BIT7;
	regdata |= 0x0F;
	res = FM320_SetReg(TXBITPHASE,regdata);
	if(res != true)
		return FM175XX_REG_ERR;
				    
	regdata = 0xF4;
	res = FM320_SetReg(GSN,regdata);  //☆☆☆☆☆低四位改变调制深度，数值越小，调制深度越小；
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0xF3;
	res = FM320_SetReg(GSNLOADMOD,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0x10;
	res = FM320_SetReg(CONTROL,regdata);
	if(res != true)
		return FM175XX_REG_ERR;
               
	regdata = 0x48;
	res = FM320_SetReg(RFCFG,regdata);//接收器放大倍数和接收灵敏度
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_GetReg(TXAUTO,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata &= ~BIT6;				//☆☆☆☆☆Disable  Force100ASK
	regdata = regdata | BIT2 | BIT1 | BIT0;
	res = FM320_SetReg(TXAUTO,regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	return FM175XX_SUCCESS;
}

//***********************************************
//函数名称：FM320_ReaderA_Request
//函数功能：ReqA
//入口参数：Type：ReqA：0x26 WupA：0x52
//出口参数：u8  0：成功   others:失败
//***********************************************
u8 FM320_ReaderA_Request(u8 type)
{
	u8 regdata;
	u8 res;
	u8 irq;
	u8 dly = 0;
	
	res = FM320_ModifyReg(STATUS2,BIT3,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(MODE,BIT6,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0x80;
	res = FM320_SetReg(COLL,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(TXMODE,BIT7,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(RXMODE,BIT7,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(RXMODE,BIT3,SET);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = CMD_TRANSCEIVE;
	res = FM320_SetReg(COMMAND,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

//	res = FM320_ModifyReg(COMMIEN,BIT5,SET);//RXI/TIMERI/ERRI
//	if(res != true)
//		return FM175XX_REG_ERR;
	irq = BIT5; //BIT5|BIT0|BIT1;
	

	regdata = 0x80;
	res = FM320_SetReg(FIFOLEVEL,regdata);	//清零FIFO的读写指针
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = type;
	res = FM320_SetReg(FIFODATA,regdata);		//发送指令头 0x26 或 0x52
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0x87;				//BIT7:STARTSEND Bit2~0 LastByte发送7bits
	res = FM320_SetReg(BITFRAMING,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0;
//配置FM320中断
	while(!(regdata & irq) && dly<100)
	{
		res = FM320_GetReg(COMMIRQ,&regdata);
		if(res != true)
		{
			return FM175XX_REG_ERR;
		}
		FM320_mDelay(10);			//0.1ms
		dly++;
	}	
//	res = FM320_ModifyReg(COMMIEN,BIT5|BIT0|BIT1,CLR);//清零中断使能
//	if(res != true)
//		return FM175XX_REG_ERR;

	res = FM320_GetReg(FIFOLEVEL,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	
	if(regdata == 2)
	{
		res = FM320_GetReg(FIFODATA,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		CardA_Sel_Res.ATQA[0] = regdata;

		res = FM320_GetReg(FIFODATA,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		CardA_Sel_Res.ATQA[1] = regdata;
	}
	else
	{
		regdata = 0x7F;                                    //added for nb1411
		res = FM320_SetReg(COMMIRQ,regdata);                     //
		if(res != true)                                    //
				return FM175XX_REG_ERR;                            //added for nb1411
		return FM175XX_RECV_TIMEOUT;
	}
	regdata = 0x7F;
	res = FM320_SetReg(COMMIRQ,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(FIFOLEVEL,BIT7,SET);
	if(res != true)
		return FM175XX_REG_ERR;

	return FM175XX_SUCCESS;
	
}

//*************************************
//函数  名：FM320_ReaderA_AntiCol
//入口参数：size:防冲突等级，包括0、1、2
//出口参数：u8:0:OK  others：错误
//*************************************
u8 FM320_ReaderA_AntiCol(u8 size)
{
	u8 regdata;
	u8 res;
	u8 i;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;

	res = FM320_ModifyReg(TXMODE,PHCS_BFL_JBIT_TXCRCEN,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(RXMODE,PHCS_BFL_JBIT_RXCRCEN,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0x00;
	res = FM320_SetReg(COLL,regdata);
	if(res != true)
		return FM175XX_REG_ERR;
				
	regdata = 0x00;				//8Bits数据全发
	res = FM320_SetReg(BITFRAMING,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	if(size>2)
		size = 0;
	*(NFC_DataExStruct.pExBuf+0) = RF_CMD_ANTICOL[size];   //防冲突等级
	*(NFC_DataExStruct.pExBuf+1) = 0x20;
	NFC_DataExStruct.nBytesToSend = 2;						//发送长度：2

	res = FM320_Command_Transceive(&NFC_DataExStruct);  

	res = FM320_SetReg(COLL,0x80);
	if(res != true)
		return FM175XX_REG_ERR;

	if((res != true)||(NFC_DataExStruct.nBytesReceived != 0x5))
		return FM175XX_FRAME_ERR;
	res = 0;
	for(i=0;i<4;i++) 
	{
		//CardA_Sel_Res.UID[size*4+i] = *(NFC_DataExStruct.pExBuf+i) ;
		CardA_Sel_Res.UID[size*3+i] = *(NFC_DataExStruct.pExBuf+i) ;
		res ^= *(NFC_DataExStruct.pExBuf+i);
	}
	if(res != *(NFC_DataExStruct.pExBuf+4))	//BCC
		return  FM175XX_BCC_ERR;

	return FM175XX_SUCCESS;
}

//*************************************
//函数  名：FM320_ReaderA_Select
//入口参数：size:防冲突等级，包括0、1、2
//出口参数：u8:0:OK  others：错误
//*************************************
u8 FM320_ReaderA_Select(u8 size)
{
	u8 addr,mask,set/*,regdata*/;
	u8 res;
	u8 i;
	
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	
	addr = TXMODE;
	mask = BIT7;
	set  = 1;
	res = FM320_ModifyReg(addr,mask,set);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RXMODE;
	mask = BIT7;
	set  = 1;
	res = FM320_ModifyReg(addr,mask,set);
	if(res != true)
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


	res = FM320_Command_Transceive(&NFC_DataExStruct);
	if(NFC_DataExStruct.nBytesReceived == 1)
		CardA_Sel_Res.SAK = *(NFC_DataExStruct.pExBuf);
	else
		return FM175XX_TRANS_ERR;
		
	return FM175XX_SUCCESS;
}

//*************************************
//函数  名：FM320_ReaderA_Rats
//入口参数：FSDI：Reader的帧长度最大值：2^x；CID:0~14,Card的逻辑地址；
//出口参数：u8:0:OK  others：错误
//*************************************
u8 FM320_ReaderA_Rats(u8 FSDI,u8 CCID)
{
	u8 res;	
	u8 i;
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	*(NFC_DataExStruct.pExBuf+0) = 0xE0;
	*(NFC_DataExStruct.pExBuf+1) = ((FSDI&0x0F)<<4) + (CCID&0x0F);   					//
	NFC_DataExStruct.nBytesToSend = 2;

	res = FM320_Command_Transceive(&NFC_DataExStruct);

	CardA_Sel_Res.ATSLEN=NFC_DataExStruct.nBytesReceived;

	for(i=0;i<CardA_Sel_Res.ATSLEN;i++)
	{
		CardA_Sel_Res.ATS[i]=NFC_DataExStruct.pExBuf[i];
	}	
	return res;
}

//*******************************************************
//函数名称：FM320_ResetAllReg
//函数功能：复位FM175xx寄存器
//出口参数：u8  true：写成功   false:写失败
//********************************************************
u8 FM320_ResetAllReg(void)
{
	u8 addr,regdata;
	u8 res;

	addr = COMMAND;	
	regdata = CMD_SOFT_RESET;//软件重启，内部Buffer的配置项不变，所有寄存器复位为默认值
	res = FM320_SetReg(addr,regdata);
	if(res != true)
		return FM175xx_REG_ERR;

	return FM175xx_DEBUG_OK;
}

//*******************************************************
//函数名称：FM320_Write_FIFO
//函数功能：写FIFO
//入口参数：fflen:写入长度；
//         ffbuf:数据缓冲区；
//出口参数：u8  true：写成功   false:写失败
//********************************************************
bool FM320_Write_FIFO(u8 fflen,u8* ffbuf)
{
	u8  i;
	u8 res;
	if(fflen > 0x40)
		return false;
	for(i=0;i<fflen;i++)
	{
		res = FM320_SetReg(0x09,*(ffbuf+i));
		if(res != true)
			return res;
	}
	return true;	
}


/******************************************************************************/
//函数名：FM320_Command_Transceive
//入口参数：
//			u8* sendbuf:发送数据缓冲区	u8 sendlen:发送数据长度
//			u8* recvbuf:接收数据缓冲区	u8* recvlen:接收到的数据长度
//出口参数：u8 0:发送接收流程正常	0x50：寄存器读写错误 
/******************************************************************************/
u8 FM320_Command_Transceive(NFC_DataExTypeDef* NFC_DataExStruct)
{
	u8 regdata;
	u8 i;
	u8 res;
	u32 dly = 0;
	res = FM320_ModifyReg(COMMIRQ,PHCS_BFL_JBIT_TXI|PHCS_BFL_JBIT_TXI|PHCS_BFL_JBIT_ERRI,SET);
	
	res = FM320_SetReg(COMMAND,CMD_TRANSCEIVE);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_ModifyReg(COMMIEN,PHCS_BFL_JBIT_TXI,SET);
	if(res != true)
		return FM175XX_REG_ERR;
	NFC_DataExStruct->WaitForComm = PHCS_BFL_JBIT_TXI;
		
	res = FM320_SetReg(FIFOLEVEL,PHCS_BFL_JBIT_FLUSHFIFO);	//FlushFIFO
	if(res != true)
		return FM175XX_REG_ERR;

	if(NFC_DataExStruct->nBytesToSend > 0x40) //FIFO maxlen=64Bytes
	{
		res = FM320_Write_FIFO(0x40,NFC_DataExStruct->pExBuf);
		if(res != true)
				return FM175XX_REG_ERR;
	}
	else
	{
		res = FM320_Write_FIFO(NFC_DataExStruct->nBytesToSend,NFC_DataExStruct->pExBuf);
		if(res != true)
				return FM175XX_REG_ERR;
	}

	res = FM320_ModifyReg(BITFRAMING,PHCS_BFL_JBIT_STARTSEND,SET);
	if(res != true)
		return FM175XX_REG_ERR;

	if(NFC_DataExStruct->nBytesToSend > 0x40)
	{		
		res = FM320_SetReg(WATERLEVEL,0x08);  //waterlevel设置
		if(res != true)
			return FM175XX_REG_ERR;
						
		for(i=0x40;i<NFC_DataExStruct->nBytesToSend;)		   //数据没有发完
		{
			res = FM320_GetReg(STATUS1,&regdata);
			if(res != true)
				return FM175XX_REG_ERR;

			if(!(regdata&0x02))	 //HiAlert没有置起
			{
					regdata = *(NFC_DataExStruct->pExBuf+i);
					res = FM320_SetReg(FIFODATA,regdata);
					if(res != true)
						return FM175XX_REG_ERR;
					i++;
			}	
		}
		
	}
	
	regdata = 0;
	while(!(regdata & NFC_DataExStruct->WaitForComm) && dly<1000)		//等待发送结束 20ms
	{
		res = FM320_GetReg(COMMIRQ,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		FM320_mDelay(1);			//0.1ms
		dly++;
	}
	if(dly == 255)
		return FM175XX_TRANS_ERR;


	res = FM320_ModifyReg(COMMIEN,PHCS_BFL_JBIT_TXI,CLR);
	if(res != true)
		return FM175XX_REG_ERR;

	NFC_DataExStruct->WaitForComm =  PHCS_BFL_JBIT_RXI|PHCS_BFL_JBIT_ERRI;
	res = FM320_ModifyReg(COMMIEN,NFC_DataExStruct->WaitForComm,SET);
	if(res != true)
		return FM175XX_REG_ERR;
	
	NFC_DataExStruct->AllCommIrq = 0;   								//临时用来储存irq
	NFC_DataExStruct->nBytesReceived  = 0;
	dly = 0;
	while(dly<1000)		//等待接收结束 20ms		 //使用dly缓存irq寄存器值
	{
		res = FM320_GetReg(FIFOLEVEL,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		regdata &= 0x7F;
			
		if(regdata != 0)	//data coming
		{
			for(i=0;i<regdata;i++)
			{
				res = FM320_GetReg(FIFODATA,NFC_DataExStruct->pExBuf+i+NFC_DataExStruct->nBytesReceived);
				if(res != true)
					return FM175XX_REG_ERR;
			} 
			NFC_DataExStruct->nBytesReceived += regdata;
		}
		res = FM320_GetReg(COMMIRQ,&NFC_DataExStruct->AllCommIrq);	  //使用tmpdata缓存irq寄存器值
		if(res != true)
			return FM175XX_REG_ERR;
		if(!(NFC_DataExStruct->AllCommIrq & NFC_DataExStruct->WaitForComm))
		{
			FM320_mDelay(1);			//0.1ms
			dly++;
		}
		else
		{
			if(dly > 997)
				dly++;
			else
			{
				FM320_mDelay(1);
				dly = 998;
			}
		}
			
	}
	return FM175XX_SUCCESS;
}
