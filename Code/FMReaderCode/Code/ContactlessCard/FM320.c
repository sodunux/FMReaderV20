#include "FM320.h"

const u8 RF_CMD_ANTICOL[3] = {0x93,0x95,0x97} ;
const u8 RF_CMD_SELECT[3] = {0x93,0x95,0x97} ;
struct TypeACardResponse CardA_Sel_Res;

u8 RF_Data_Buf[RF_DATA_BUF_LEN];

void mDelay(u16 ms)			//ʵ��1ms
{
	u32	i;
	u16 j;
	for(j=0;j<ms;j++)
	{
		for(i=0;i<8100;i++)
			;
	}
}

void uDelay(u16 us)			//ʵ��1us
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
	
	// Configure SPI1 NSS (PA.4)��SCK(PA.5)��MISO(PA.6)��MOSI(PA.7) as alternate function push-pull  
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
//�������ƣ�FM320_Get_Reg(u8 addr,u8* regdata)
//�������ܣ�ͨ��SPI�ӿڶ��Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:��ȡ��ֵ
//���ڲ�����u8  true����ȡ�ɹ�   false:ʧ��
//***********************************************
u8 FM320_GetReg(u8 addr,u8* regdata)
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
		uDelay(1);
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
		uDelay(1);
	}
		
	FM320_NSS_SET;
	return false;
}

//***********************************************
//�������ƣ�FM320_Set_Reg(u8 addr,u8* regdata)
//�������ܣ�д�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:Ҫд���ֵ
//���ڲ�����u8  true��д�ɹ�   false:дʧ��
//***********************************************
u8 FM320_SetReg(u8 addr,u8 regdata)
{
	
	u16 i;
	u8 temp = 0;
	
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
			temp = SPI_I2S_ReceiveData(SPI1);		 /* Read SPI1 received data */
			break;
		}
		uDelay(1);
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
			temp = SPI_I2S_ReceiveData(SPI1);		 /* Read SPI1 received data */
			break;
		}
		uDelay(1);
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
//�������ƣ�FM320_ModifyReg(u8 addr,u8* mask,u8 set)
//�������ܣ�д�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   mask:Ҫ�ı��λ  
//         set:  0:��־��λ����   ����:��־��λ����
//���ڲ�����u8  true��д�ɹ�   false:дʧ��
//********************************************************
u8 FM320_ModifyReg(u8 addr,u8 mask,u8 set)
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
//�������ƣ�ReadeA_HALT
//�������ܣ�����HALT����
//���ڲ�����u8  0���ɹ�   others:ʧ��
//***********************************************
u8 ReaderA_HALT()
{
	u8 res;	
	u8 i;
	u8 halt[2]={0x50,0x00};
	
	res = FM320_SetReg(FIFOLEVEL,PHCS_BFL_JBIT_FLUSHFIFO);	//FlushFIFO
	if(res != true)
		return FM175XX_REG_ERR;
	res = FM320_ModifyReg(TXMODE,PHCS_BFL_JBIT_TXCRCEN,SET);
	if(res != true)
		return FM175XX_REG_ERR;
		
	res = Write_FIFO(0x02,halt);
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_SetReg(COMMAND,CMD_TRANSMIT);
	if(res != true)
		return FM175XX_REG_ERR;
	
	res = FM320_SetReg(BITFRAMING,0x80);//8Bits����ȫ��
	if(res != true)
		return FM175XX_REG_ERR;	
	

	return FM175XX_SUCCESS;
}



//***********************************************
//�������ƣ�RateSel
//�������ܣ��޸ķ��ͺͽ�������
//��ڲ�����txrata �������ʣ�rxrate ��������
//					0:106K;1:212K;2:424K;3:848K;
//���ڲ�����u8  0���ɹ�   others:ʧ��
//***********************************************
u8 RateSel(u8 txrate,u8 rxrate)
{
	u8 ret,regdata;
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
	
	return ret;
}

//***********************************************
//�������ƣ�FM320_Enter_Idle
//�������ܣ�����IDLE(ȡ����ǰ����)
//��ڲ�����NA
//���ڲ�����u8  0���ɹ�   others:ʧ��
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
//�������ƣ�FM175xx_Initial_ReaderA
//�������ܣ�ReaderA��ʼ��
//��ڲ�����NA
//���ڲ�����u8  0���ɹ�   others:ʧ��
//***********************************************
u8 FM320_Initial_ReaderA(void)
{
	u8 addr,mask,set,regdata;
	u8 res;

	addr = STATUS2;
	mask = BIT3;
	set  = 0;
	res = FM320_ModifyReg(addr,mask,set); //�رռӽ��ܿ���
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXMODE;
	res = FM320_GetReg(addr,&regdata);//��ȡTXMODE�Ĵ�������
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);	//����TxMix��InvMod������
	regdata |= BIT7;									//Enable CRC generation 
	res = FM320_SetReg(addr,regdata);	//106K������ģʽMifare(ISO14443Э��)
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RXMODE;
	res = FM320_GetReg(addr,&regdata);//��ȡRXMODE�Ĵ�������
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & (BIT3|BIT2);	//�����Զ����ԷǷ�����֡���Զ��ظ���������
	regdata |= BIT7;
	res = FM320_SetReg(addr,regdata); //������ܵ�ʱ�����CRC����
	FM320_GetReg(addr,&regdata);	
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXCONTROL;
	res = FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
//	regdata=0x80;
	regdata = (regdata | BIT1 |BIT0);//TX1��TX2��PIN�������13.56M���ز�
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
		
	addr = MODWIDTH;								//���õ��ƿ��
	regdata = 0x26;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RXTRESHOLD; //������͵Ľ����ƽ
	regdata = 0x55;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXBITPHASE;	//����
	res = FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata = regdata & BIT7;	//���ֽ��ʱ�ӵ�����Դ
	regdata |= 0x0F;					//���÷���ǰ�ĵȴ�ʱ��
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = GSN;//�������ߵ�в���
	regdata = 0xF4;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	
	addr = CWGSP;//�������ߵ�в���
	regdata = 0x3F;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = GSNLOADMOD;   //����NDriver���߲���
	regdata = 0xF4;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = CONTROL;
	regdata = 0x10;
	res = FM320_SetReg(addr,regdata);//FM320��Ϊ������
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = RFCFG;                //�������Ŵ����ͽ���������
	regdata = 0x38;
	res = FM320_SetReg(addr,regdata);
	FM320_GetReg(addr,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	addr = TXAUTO;	//�������߲���
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
//�������ƣ�FM320_Initial_ReaderB
//�������ܣ�ReaderB��ʼ��
//��ڲ�����NA
//���ڲ�����u8  true���ɹ�   false:ʧ��
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
	res = FM320_SetReg(GSN,regdata);  //���������λ�ı������ȣ���ֵԽС���������ԽС��
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
	res = FM320_SetReg(RFCFG,regdata);//�������Ŵ����ͽ���������
	if(res != true)
		return FM175XX_REG_ERR;

	res = FM320_GetReg(TXAUTO,&regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	regdata &= ~BIT6;				//������Disable  Force100ASK
	regdata = regdata | BIT2 | BIT1 | BIT0;
	res = FM320_SetReg(TXAUTO,regdata);
	if(res != true)
		return FM175XX_REG_ERR;
	return FM175XX_SUCCESS;
}

//***********************************************
//�������ƣ�ReaderA_Request
//�������ܣ�ReqA
//��ڲ�����Type��ReqA��0x26 WupA��0x52
//���ڲ�����u8  0���ɹ�   others:ʧ��
//***********************************************
u8 ReaderA_Request(u8 type)
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
	res = FM320_SetReg(FIFOLEVEL,regdata);	//����FIFO�Ķ�дָ��
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = type;
	res = FM320_SetReg(FIFODATA,regdata);		//����ָ��ͷ 0x26 �� 0x52
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0x87;				//BIT7:STARTSEND Bit2~0 LastByte����7bits
	res = FM320_SetReg(BITFRAMING,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	regdata = 0;
//����FM320�ж�
	while(!(regdata & irq) && dly<100)
	{
		res = FM320_GetReg(COMMIRQ,&regdata);
		if(res != true)
		{
			return FM175XX_REG_ERR;
		}
		mDelay(10);			//0.1ms
		dly++;
	}	
//	res = FM320_ModifyReg(COMMIEN,BIT5|BIT0|BIT1,CLR);//�����ж�ʹ��
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
//����  ����ReaderA_AntiCol
//��ڲ�����size:����ͻ�ȼ�������0��1��2
//���ڲ�����u8:0:OK  others������
//*************************************
u8 ReaderA_AntiCol(u8 size)
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
				
	regdata = 0x00;				//8Bits����ȫ��
	res = FM320_SetReg(BITFRAMING,regdata);
	if(res != true)
		return FM175XX_REG_ERR;

	if(size>2)
		size = 0;
	*(NFC_DataExStruct.pExBuf+0) = RF_CMD_ANTICOL[size];   //����ͻ�ȼ�
	*(NFC_DataExStruct.pExBuf+1) = 0x20;
	NFC_DataExStruct.nBytesToSend = 2;						//���ͳ��ȣ�2

	res = Command_Transceive(&NFC_DataExStruct);  

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
//����  ����ReaderA_Select
//��ڲ�����size:����ͻ�ȼ�������0��1��2
//���ڲ�����u8:0:OK  others������
//*************************************
u8 ReaderA_Select(u8 size)
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

	if(size>2)							 //������ط���ͻ
		size = 0;
	*(NFC_DataExStruct.pExBuf+0) = RF_CMD_ANTICOL[size];   //����ͻ�ȼ�
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
//����  ����ReaderA_Rats
//��ڲ�����FSDI��Reader��֡�������ֵ��2^x��CID:0~14,Card���߼���ַ��
//���ڲ�����u8:0:OK  others������
//*************************************
u8 ReaderA_Rats(u8 FSDI,u8 CCID)
{
	u8 res;	
	u8 i;
	NFC_DataExTypeDef NFC_DataExStruct;
	NFC_DataExStruct.pExBuf = RF_Data_Buf;
	*(NFC_DataExStruct.pExBuf+0) = 0xE0;
	*(NFC_DataExStruct.pExBuf+1) = ((FSDI&0x0F)<<4) + (CCID&0x0F);   					//
	NFC_DataExStruct.nBytesToSend = 2;

	res = Command_Transceive(&NFC_DataExStruct);

	CardA_Sel_Res.ATSLEN=NFC_DataExStruct.nBytesReceived;

	for(i=0;i<CardA_Sel_Res.ATSLEN;i++)
	{
		CardA_Sel_Res.ATS[i]=NFC_DataExStruct.pExBuf[i];
	}	
	return res;
}

//*******************************************************
//�������ƣ�FM320_ResetAllReg
//�������ܣ���λFM175xx�Ĵ���
//���ڲ�����u8  true��д�ɹ�   false:дʧ��
//********************************************************
u8 FM320_ResetAllReg(void)
{
	u8 addr,regdata;
	u8 res;

	addr = COMMAND;	
	regdata = CMD_SOFT_RESET;//����������ڲ�Buffer��������䣬���мĴ�����λΪĬ��ֵ
	res = FM320_SetReg(addr,regdata);
	if(res != true)
		return FM175xx_REG_ERR;

	return FM175xx_DEBUG_OK;
}

//*******************************************************
//�������ƣ�Write_FIFO
//�������ܣ�дFIFO
//��ڲ�����fflen:д�볤�ȣ�
//         ffbuf:���ݻ�������
//���ڲ�����u8  true��д�ɹ�   false:дʧ��
//********************************************************
u8 Write_FIFO(u8 fflen,u8* ffbuf)
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
//��������Command_Transceive
//��ڲ�����
//			u8* sendbuf:�������ݻ�����	u8 sendlen:�������ݳ���
//			u8* recvbuf:�������ݻ�����	u8* recvlen:���յ������ݳ���
//���ڲ�����u8 0:���ͽ�����������	0x50���Ĵ�����д���� 
/******************************************************************************/
u8 Command_Transceive(NFC_DataExTypeDef* NFC_DataExStruct)
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
		res = Write_FIFO(0x40,NFC_DataExStruct->pExBuf);
		if(res != true)
				return FM175XX_REG_ERR;
	}
	else
	{
		res = Write_FIFO(NFC_DataExStruct->nBytesToSend,NFC_DataExStruct->pExBuf);
		if(res != true)
				return FM175XX_REG_ERR;
	}

	res = FM320_ModifyReg(BITFRAMING,PHCS_BFL_JBIT_STARTSEND,SET);
	if(res != true)
		return FM175XX_REG_ERR;

	if(NFC_DataExStruct->nBytesToSend > 0x40)
	{		
		res = FM320_SetReg(WATERLEVEL,0x08);  //waterlevel����
		if(res != true)
			return FM175XX_REG_ERR;
						
		for(i=0x40;i<NFC_DataExStruct->nBytesToSend;)		   //����û�з���
		{
			res = FM320_GetReg(STATUS1,&regdata);
			if(res != true)
				return FM175XX_REG_ERR;

			if(!(regdata&0x02))	 //HiAlertû������
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
	while(!(regdata & NFC_DataExStruct->WaitForComm) && dly<1000)		//�ȴ����ͽ��� 20ms
	{
		res = FM320_GetReg(COMMIRQ,&regdata);
		if(res != true)
			return FM175XX_REG_ERR;
		mDelay(1);			//0.1ms
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
	
	NFC_DataExStruct->AllCommIrq = 0;   								//��ʱ��������irq
	NFC_DataExStruct->nBytesReceived  = 0;
	dly = 0;
	while(dly<1000)		//�ȴ����ս��� 20ms		 //ʹ��dly����irq�Ĵ���ֵ
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
		res = FM320_GetReg(COMMIRQ,&NFC_DataExStruct->AllCommIrq);	  //ʹ��tmpdata����irq�Ĵ���ֵ
		if(res != true)
			return FM175XX_REG_ERR;
		if(!(NFC_DataExStruct->AllCommIrq & NFC_DataExStruct->WaitForComm))
		{
			mDelay(1);			//0.1ms
			dly++;
		}
		else
		{
			if(dly > 997)
				dly++;
			else
			{
				mDelay(1);
				dly = 998;
			}
		}
			
	}
	return FM175XX_SUCCESS;
}
