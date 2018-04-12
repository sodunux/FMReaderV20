#include "SPI_IIC.h"
#include "includes.h"
#include "ContactCard.h"

#define SCL_H         GPIOB->BSRR = GPIO_Pin_10
#define SCL_L         GPIOB->BRR  = GPIO_Pin_10
   
#define SDA_H         GPIOB->BSRR = GPIO_Pin_11
#define SDA_L         GPIOB->BRR  = GPIO_Pin_11

#define SCL_read      GPIOB->IDR  & GPIO_Pin_10
#define SDA_read      GPIOB->IDR  & GPIO_Pin_11

#define Dummy_Byte 		0x5A

uint8_t 	I2C2_SLAVE_ADDRESS7 = 0xA0;
uint32_t 	I2C2_ClockSpeed = 400000;
uint8_t		I2C2_Buffer_Tx[I2C2_BufferSize];
uint8_t		I2C2_Buffer_Rx[I2C2_BufferSize];
//uint8_t		SPI1_Buffer_Tx[SPI1_BufferSize];
//uint8_t		SPI1_Buffer_Rx[SPI1_BufferSize];
uint8_t		I2C2_SendLen = 0;

uint8_t		sEEDataNum;

extern uint8_t I2C_Addr_Len;


uint8_t volatile 	I2C2_Tx_Idx = 0, I2C2_Rx_Idx = 0, I2C2_PEC_Value = 0;
uint8_t volatile	SPI1_Tx_Idx = 0, SPI1_Rx_Idx = 0;

/* Standard ASCII 6x8 font */
const char font6x8[] = {
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00,// sp
  0x00, 0x00, 0x00, 0x2f, 0x00, 0x00,// !
  0x00, 0x00, 0x07, 0x00, 0x07, 0x00,// "
  0x00, 0x14, 0x7f, 0x14, 0x7f, 0x14,// #
  0x00, 0x24, 0x2a, 0x7f, 0x2a, 0x12,// $
  0x00, 0x62, 0x64, 0x08, 0x13, 0x23,// %
  0x00, 0x36, 0x49, 0x55, 0x22, 0x50,// &
  0x00, 0x00, 0x05, 0x03, 0x00, 0x00,// '
  0x00, 0x00, 0x1c, 0x22, 0x41, 0x00,// (
  0x00, 0x00, 0x41, 0x22, 0x1c, 0x00,// )
  0x00, 0x14, 0x08, 0x3E, 0x08, 0x14,// *
  0x00, 0x08, 0x08, 0x3E, 0x08, 0x08,// +
  0x00, 0x00, 0x00, 0xA0, 0x60, 0x00,// ,
  0x00, 0x08, 0x08, 0x08, 0x08, 0x08,// -
  0x00, 0x00, 0x60, 0x60, 0x00, 0x00,// .
  0x00, 0x20, 0x10, 0x08, 0x04, 0x02,// /
  0x00, 0x3E, 0x51, 0x49, 0x45, 0x3E,// 0
  0x00, 0x00, 0x42, 0x7F, 0x40, 0x00,// 1
  0x00, 0x42, 0x61, 0x51, 0x49, 0x46,// 2
  0x00, 0x21, 0x41, 0x45, 0x4B, 0x31,// 3
  0x00, 0x18, 0x14, 0x12, 0x7F, 0x10,// 4
  0x00, 0x27, 0x45, 0x45, 0x45, 0x39,// 5
  0x00, 0x3C, 0x4A, 0x49, 0x49, 0x30,// 6
  0x00, 0x01, 0x71, 0x09, 0x05, 0x03,// 7
  0x00, 0x36, 0x49, 0x49, 0x49, 0x36,// 8
  0x00, 0x06, 0x49, 0x49, 0x29, 0x1E,// 9
  0x00, 0x00, 0x36, 0x36, 0x00, 0x00,// :
  0x00, 0x00, 0x56, 0x36, 0x00, 0x00,// ;
  0x00, 0x08, 0x14, 0x22, 0x41, 0x00,// <
  0x00, 0x14, 0x14, 0x14, 0x14, 0x14,// =
  0x00, 0x00, 0x41, 0x22, 0x14, 0x08,// >
  0x00, 0x02, 0x01, 0x51, 0x09, 0x06,// ?
  0x00, 0x32, 0x49, 0x59, 0x51, 0x3E,// @
  0x00, 0x7C, 0x12, 0x11, 0x12, 0x7C,// A
  0x00, 0x7F, 0x49, 0x49, 0x49, 0x36,// B
  0x00, 0x3E, 0x41, 0x41, 0x41, 0x22,// C
  0x00, 0x7F, 0x41, 0x41, 0x22, 0x1C,// D
  0x00, 0x7F, 0x49, 0x49, 0x49, 0x41,// E
  0x00, 0x7F, 0x09, 0x09, 0x09, 0x01,// F
  0x00, 0x3E, 0x41, 0x49, 0x49, 0x7A,// G
  0x00, 0x7F, 0x08, 0x08, 0x08, 0x7F,// H
  0x00, 0x00, 0x41, 0x7F, 0x41, 0x00,// I
  0x00, 0x20, 0x40, 0x41, 0x3F, 0x01,// J
  0x00, 0x7F, 0x08, 0x14, 0x22, 0x41,// K
  0x00, 0x7F, 0x40, 0x40, 0x40, 0x40,// L
  0x00, 0x7F, 0x02, 0x0C, 0x02, 0x7F,// M
  0x00, 0x7F, 0x04, 0x08, 0x10, 0x7F,// N
  0x00, 0x3E, 0x41, 0x41, 0x41, 0x3E,// O
  0x00, 0x7F, 0x09, 0x09, 0x09, 0x06,// P
  0x00, 0x3E, 0x41, 0x51, 0x21, 0x5E,// Q
  0x00, 0x7F, 0x09, 0x19, 0x29, 0x46,// R
  0x00, 0x46, 0x49, 0x49, 0x49, 0x31,// S
  0x00, 0x01, 0x01, 0x7F, 0x01, 0x01,// T
  0x00, 0x3F, 0x40, 0x40, 0x40, 0x3F,// U
  0x00, 0x1F, 0x20, 0x40, 0x20, 0x1F,// V
  0x00, 0x3F, 0x40, 0x38, 0x40, 0x3F,// W
  0x00, 0x63, 0x14, 0x08, 0x14, 0x63,// X
  0x00, 0x07, 0x08, 0x70, 0x08, 0x07,// Y
  0x00, 0x61, 0x51, 0x49, 0x45, 0x43,// Z
  0x00, 0x00, 0x7F, 0x41, 0x41, 0x00,// [
  0x00, 0x55, 0x2A, 0x55, 0x2A, 0x55,// 55
  0x00, 0x00, 0x41, 0x41, 0x7F, 0x00,// ]
  0x00, 0x04, 0x02, 0x01, 0x02, 0x04,// ^
  0x00, 0x40, 0x40, 0x40, 0x40, 0x40,// _
  0x00, 0x00, 0x01, 0x02, 0x04, 0x00,// '
  0x00, 0x20, 0x54, 0x54, 0x54, 0x78,// a
  0x00, 0x7F, 0x48, 0x44, 0x44, 0x38,// b
  0x00, 0x38, 0x44, 0x44, 0x44, 0x20,// c
  0x00, 0x38, 0x44, 0x44, 0x48, 0x7F,// d
  0x00, 0x38, 0x54, 0x54, 0x54, 0x18,// e
  0x00, 0x08, 0x7E, 0x09, 0x01, 0x02,// f
  0x00, 0x18, 0xA4, 0xA4, 0xA4, 0x7C,// g
  0x00, 0x7F, 0x08, 0x04, 0x04, 0x78,// h
  0x00, 0x00, 0x44, 0x7D, 0x40, 0x00,// i
  0x00, 0x40, 0x80, 0x84, 0x7D, 0x00,// j
  0x00, 0x7F, 0x10, 0x28, 0x44, 0x00,// k
  0x00, 0x00, 0x41, 0x7F, 0x40, 0x00,// l
  0x00, 0x7C, 0x04, 0x18, 0x04, 0x78,// m
  0x00, 0x7C, 0x08, 0x04, 0x04, 0x78,// n
  0x00, 0x38, 0x44, 0x44, 0x44, 0x38,// o
  0x00, 0xFC, 0x24, 0x24, 0x24, 0x18,// p
  0x00, 0x18, 0x24, 0x24, 0x18, 0xFC,// q
  0x00, 0x7C, 0x08, 0x04, 0x04, 0x08,// r
  0x00, 0x48, 0x54, 0x54, 0x54, 0x20,// s
  0x00, 0x04, 0x3F, 0x44, 0x40, 0x20,// t
  0x00, 0x3C, 0x40, 0x40, 0x20, 0x7C,// u
  0x00, 0x1C, 0x20, 0x40, 0x20, 0x1C,// v
  0x00, 0x3C, 0x40, 0x30, 0x40, 0x3C,// w
  0x00, 0x44, 0x28, 0x10, 0x28, 0x44,// x
  0x00, 0x1C, 0xA0, 0xA0, 0xA0, 0x7C,// y
  0x00, 0x44, 0x64, 0x54, 0x4C, 0x44,// z
  0x14, 0x14, 0x14, 0x14, 0x14, 0x14,// horiz lines
};


void SPI_Send(uint8_t *SPI1_SendBuffer, uint16_t SPI1_SendLen)
{
	SPI1_Tx_Idx = 0;

	while(SPI1_Tx_Idx<SPI1_SendLen)
  	{ 
	    /* Wait for SPI1 Tx buffer empty */ 
	    while(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_TXE)==RESET);
	    /* Send SPI1 data */ 
	    SPI_I2S_SendData(SPI1, *(SPI1_SendBuffer+SPI1_Tx_Idx));	
			SPI1_Tx_Idx++;

// 2013.10.8	Hong:		
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();

		while(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) == RESET);
		SPI_I2S_ReceiveData(SPI1);
	}
}
u8 SPI_SendByte(u8 byte) //get Byte returned
{
  /* Loop while DR register in not emplty */
  while(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_TXE) == RESET);

  /* Send byte through the SPI1 peripheral */
  SPI_I2S_SendData(SPI1, byte);

  /* Wait to receive a byte */
  while(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) == RESET);

  /* Return the byte read from the SPI bus */
  return  SPI_I2S_ReceiveData(SPI1);
}

void SPI_Receive(u8 *ReceiveBuffer, u16 ReceiveLen)
{

	while(ReceiveLen--)
	{
		*ReceiveBuffer++ = SPI_SendByte(Dummy_Byte);
// 2013.10.8	Hong:
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();
	}
	
}


void I2C_StartSend(void)
{
	I2C_Cmd(I2C2, ENABLE);
	I2C_ITConfig(I2C2, I2C_IT_EVT,ENABLE);// | I2C_IT_BUF, ENABLE);
	while(I2C2->SR2&0x02);
	I2C_GenerateSTART(I2C2, ENABLE);

}

void I2C_HW_Send(uint8_t *I2C_SendBuffer, uint16_t ByteToSend)
{
 while(I2C2->SR2&0x02)
 {
  	if(bIsTimeout==1)
		return;
 };
 /* Send START condition */
  I2C_GenerateSTART(I2C2, ENABLE);
  
  /* Test on EV5 and clear it */
  while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_MODE_SELECT))
  {
  	if(bIsTimeout==1)
		return;
  }; 
  
  /* Send address for write */
  I2C_Send7bitAddress(I2C2, I2C2_SLAVE_ADDRESS7, I2C_Direction_Transmitter);

  /* Test on EV6 and clear it */
  while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED))
  {
	if(bIsTimeout==1)
		return;  	
  };  

  /* While there is data to be Send */
  while(ByteToSend--)  
  {
    /* Send the current byte */
    I2C_SendData(I2C2, *I2C_SendBuffer); 

    /* Point to the next byte to be written */
    I2C_SendBuffer++; 
  
    /* Test on EV8 and clear it */
    while (!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_BYTE_TRANSMITTED))
	{
		if(bIsTimeout==1)
			return;  			
	};
  }

  /* Send STOP condition */
  I2C_GenerateSTOP(I2C2, ENABLE);

  return;
}

void ReadCurByte(uint8_t *DataByte)
{
	 while(I2C2->SR2&0x02)
	 {
	  	if(bIsTimeout==1)
			return;
	 };

	I2C_AcknowledgeConfig(I2C2, DISABLE);

	I2C_GenerateSTART(I2C2, ENABLE);
  /* Test on EV5 and clear it */
	while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_MODE_SELECT))
	{
		if(bIsTimeout==1)
			return;
	};
	
	I2C_Send7bitAddress(I2C2, I2C2_SLAVE_ADDRESS7, I2C_Direction_Receiver);
	/* Test on EV6 and clear it */
	while(!I2C_CheckEvent(I2C1, I2C_EVENT_MASTER_RECEIVER_MODE_SELECTED))
	{
		if(bIsTimeout==1)
			return;  	
	};		

	while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_BYTE_RECEIVED))
	{
		if(bIsTimeout==1)
			return; 
	};

	I2C_GenerateSTOP(I2C2, ENABLE);

	*DataByte = I2C_ReceiveData(I2C2);
	
	I2C_AcknowledgeConfig(I2C2, ENABLE);
}

/*******************************************************************************
* Function Name  : I2C_EE_BufferRead
* Description    : Reads a block of data from the EEPROM.
* Input          : - pBuffer : pointer to the buffer that receives the data read 
*                    from the EEPROM.
*                  - ReadAddr : EEPROM's internal address to read from.
*                  - NumByteToRead : number of bytes to read from the EEPROM.
* Output         : None
* Return         : None
*******************************************************************************/
void I2C_EE_BufferRead(uint8_t* pBuffer, uint8_t ReadAddr, uint16_t NumByteToRead)
{  
  /* Send START condition */
  I2C_GenerateSTART(I2C2, ENABLE);
  
  /* Test on EV5 and clear it */
  while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_MODE_SELECT))
  {
  	if(bIsTimeout==1)
		return;
  };
  
  /* In the case of a single data transfer disable ACK before reading the data */
  if(NumByteToRead==1) 
  {
    I2C_AcknowledgeConfig(I2C2, DISABLE);
  }
  
  /* Send EEPROM address for write */
  I2C_Send7bitAddress(I2C2, I2C2_SLAVE_ADDRESS7, I2C_Direction_Transmitter);

  /* Test on EV6 and clear it */
  while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED))
  {
	if(bIsTimeout==1)
		return;
  };
  
  /* Clear EV6 by setting again the PE bit */
  I2C_Cmd(I2C2, ENABLE);

  /* Send the EEPROM's internal address to write to */
  I2C_SendData(I2C2, ReadAddr);  

  /* Test on EV8 and clear it */
  while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_BYTE_TRANSMITTED))
  {
  	if(bIsTimeout==1)
		return;
  };
  
  /* Send STRAT condition a second time */  
  I2C_GenerateSTART(I2C1, ENABLE);
  
  /* Test on EV5 and clear it */
  while(!I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_MODE_SELECT))
  {
  	if(bIsTimeout==1)
		return;
  };
  
  /* Send EEPROM address for read */
  I2C_Send7bitAddress(I2C2, I2C2_SLAVE_ADDRESS7, I2C_Direction_Receiver);
  
  /* Test on EV6 and clear it */
  while(!I2C_CheckEvent(I2C1, I2C_EVENT_MASTER_RECEIVER_MODE_SELECTED))
  {
  	if(bIsTimeout==1)
		return;  	
  };
  
  /* While there is data to be read */
  while(NumByteToRead)  
  {
    /* Test on EV7 and clear it */
    if(I2C_CheckEvent(I2C2, I2C_EVENT_MASTER_BYTE_RECEIVED))  
    {
      if(NumByteToRead == 2)
      {
        /* Disable Acknowledgement */
        I2C_AcknowledgeConfig(I2C2, DISABLE);
      }

      if(NumByteToRead == 1)
      {
        /* Send STOP Condition */
        I2C_GenerateSTOP(I2C2, ENABLE);
      }
      
      /* Read a byte from the EEPROM */
      *pBuffer = I2C_ReceiveData(I2C2);

      /* Point to the next location where the byte read will be saved */
      pBuffer++; 
      
      /* Decrement the read bytes counter */
      NumByteToRead--;    
    }   
  }

  /* Enable Acknowledgement to be ready for another reception */
  I2C_AcknowledgeConfig(I2C2, ENABLE);
}

void sFLASH_Init(void)
{
	SPI_InitTypeDef  SPI_InitStructure;
	SPI1_CS_H();
	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;
	SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;
	SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;
	SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;
	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;
	SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_64; // 72M/16=4.5M
	SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;
	SPI_InitStructure.SPI_CRCPolynomial = 7;
	SPI_Init(sFLASH_SPI, &SPI_InitStructure);
	SPI_Cmd(sFLASH_SPI, ENABLE);
}

void I2C_GPIO_Config(void)
{
  GPIO_InitTypeDef  GPIO_InitStructure; 
  /* Configure I2C1 pins: SCL and SDA */
  GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_10 | GPIO_Pin_11;
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_OD;  
  GPIO_Init(GPIOB, &GPIO_InitStructure);
}

void I2C_delay(void)
{	
	uint8_t volatile i=40;//40; //这里延时可以优化速度	，经测试最低到5还能写入
	while(i) 
	{ 
	 	i--; 
	} 
}
void I2C_delay_1(void)
{	
	uint8_t i=200; 
	while(i) 
	{ 
		i--; 
	} 
}

bool I2C_Start(void)
{
	I2C_delay_1();
	I2C_delay_1();
	I2C_delay_1();
	SDA_H;
	I2C_delay();
	SCL_H;
	I2C_delay();
	if(!SDA_read)
		return FALSE;	//SDA线为低电平则总线忙,退出
//	if(!SCL_read)
	{
		while(!SCL_read)			//等待设备释放SCL	,2013.9.11		Hong
		{
			SCL_H;
			I2C_delay();
		}
		I2C_delay();
	}
	SDA_L;
	I2C_delay();
	if(SDA_read) 
		return FALSE;	//SDA线为高电平则总线出错,退出
	SDA_L;
	I2C_delay();
	return TRUE;
}

void I2C_Stop(void)
{
	SCL_L;
	I2C_delay();
	SDA_L;
	I2C_delay();
	SCL_H;
	I2C_delay();
	SDA_H;
	I2C_delay();
}

void I2C_Ack(void)
{	
	SCL_L;
	I2C_delay();
	SDA_L;
	I2C_delay();
	SCL_H;
	I2C_delay();
	SCL_L;
	I2C_delay();
}


void I2C_NoAck(void)
{	
	SCL_L;
	I2C_delay();
	SDA_H;
	I2C_delay();
	SCL_H;
	I2C_delay();
	SCL_L;
	I2C_delay();
}


bool I2C_WaitAck(void) 	 //返回为:=1有ACK,=0无ACK
{
	SCL_L;
	I2C_delay();
	SDA_H;			
	I2C_delay();
	SCL_H;
	I2C_delay();
	if(SDA_read)
	{
	      SCL_L;
	      return FALSE;
	}
	SCL_L;
	return TRUE;
}

static void I2C_SendByte(uint8_t SendByte) //数据从高位到低位//
{
//	uint8_t i=8;
	
//----------------2013.9.11---------Hong----------Start------------
	uint8_t i=7;
	SCL_L;
	I2C_delay();
	if(SendByte&0x80)
		SDA_H;  
	else 
		SDA_L;   
	SendByte<<=1;
	SCL_H;
	I2C_delay();
//等待设备释放SCL
//	if(!SCL_read)		//等待设备释放SCL	,2013.9.11		Hong
	{
		while(!SCL_read)
		{
			SCL_H;
			I2C_delay();
		}
	}
	I2C_delay();
	I2C_delay();
//----------------2013.9.11---------Hong----------End-------------	*/
    while(i--)
    {
        SCL_L;
        I2C_delay();
      if(SendByte&0x80)
        SDA_H;  
      else 
        SDA_L;   
        SendByte<<=1;
        I2C_delay();
		SCL_H;
        I2C_delay();
    }
    SCL_L;
}

static uint8_t I2C_ReceiveByte(void)  //数据从高位到低位//
{ 
	uint8_t i;
	uint8_t ReceiveByte=0;

	SDA_H;				
	I2C_delay();
//2013.9.27		Hong	等待Slave释放Clcok:
	SCL_L;
	I2C_delay();
	SCL_H;
	I2C_delay();	
	while(!SCL_read)
	{
		SCL_H;
		I2C_delay();	
	}
	I2C_delay();	
	if(SDA_read)
	{
		ReceiveByte|=0x01;
	}	
	for(i=0; i<7; i++)
	{
		ReceiveByte<<=1;      
		SCL_L;
		I2C_delay();
		SCL_H;
		I2C_delay();	
		if(SDA_read)
		{
			ReceiveByte|=0x01;
		}
	}
	SCL_L;
	return ReceiveByte;
}

bool I2C_Send(u8 *address, u8* pBuffer, u8 length)
{
	if(!I2C_Start())
		return FALSE;
	if(!I2C_Addr_Len)
		I2C_SendByte(*address<<1);		//address
	else									// 2013.9.11	Hong
	{
		I2C_SendByte(((*(address+1) << 1 )|0xf0) & 0xf6);
		I2C_WaitAck();
		I2C_delay();
		I2C_delay();
		I2C_SendByte(*address);
	}
	if(!I2C_WaitAck())
	{
		I2C_Stop(); 
		return FALSE;
	}
  
	while(length--)
	{
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();
		I2C_SendByte(* pBuffer);
		I2C_WaitAck();
		pBuffer++;
	}
//	I2C_Stop(); 
	return TRUE;
}
bool I2C_Receive(u8 *address, u8* pBuffer, u8 length)
{
	if(!I2C_Start()) 
		return FALSE;
	if(!I2C_Addr_Len)
		I2C_SendByte((*address << 1) | 0x01);		//address
	else											// 2013.9.11	Hong
	{
//	10bit地址在重复start位后只发一byte地址
		I2C_SendByte(((*(address+1) << 1 )|0xf1) & 0xf7);
/*		I2C_WaitAck();
		I2C_delay();
		I2C_delay();
		I2C_SendByte(*address);	*/
	}
	
	if(!I2C_WaitAck())	
	{
		I2C_Stop();
		return FALSE;
	}
	while(length)
	{
//保证byte间足够延时用于slave处理数据
		I2C_delay_1();
		I2C_delay_1();
		I2C_delay_1();
		*pBuffer = I2C_ReceiveByte();
		if(length==1)
			I2C_NoAck();
		else
			I2C_Ack();	
		pBuffer++;
		length--;
	}
	I2C_Stop();
	return TRUE;

}

u8 I2C_ReadOneByte24(void)
{
	u8 tmp;
	if(!I2C_Start()) return FALSE;
	I2C_SendByte(0xA1);
	if(!I2C_WaitAck()) {I2C_Stop(); return FALSE;}
	tmp = I2C_ReceiveByte();
	I2C_Stop();
	return tmp;
}

bool I2C_ReadBytes24(uint8_t* pBuffer,uint8_t length, uint8_t ReadAddress)
{		
    if(!I2C_Start())return FALSE;
    I2C_SendByte(0xA0);//	器件地址 
    if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
    I2C_SendByte(ReadAddress);   //设置低起始地址      
    I2C_WaitAck();
    I2C_Start();
    I2C_SendByte(0xA1);
    I2C_WaitAck();
    while(length)
    {
      *pBuffer = I2C_ReceiveByte();
      if(length == 1)I2C_NoAck();
      else I2C_Ack(); 
      pBuffer++;
      length--;
    }
    I2C_Stop();
    return TRUE;
}


//××××××××××××××××××××××OLED SH1106×××××××××××//

/**********************************************
// IIC Write Command
**********************************************/
bool Write_1106_Command(unsigned char IIC_Command)
{
   if(!I2C_Start())return FALSE;

   I2C_SendByte(0x78);            //Slave address,SA0=0
   if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
   I2C_SendByte(0x00);			//write command
   if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
   I2C_SendByte(IIC_Command);
   if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
   I2C_Stop();
   return TRUE;
}
/**********************************************
// IIC Write Data
**********************************************/
bool Write_1106_Data(unsigned char IIC_Data)
{
   if(!I2C_Start())return FALSE;

   I2C_SendByte(0x78);
   if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
   I2C_SendByte(0x40);			//写数据——发送此命令之后，IIC_Stop之前，发送的所有的byte均为数据。
   if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
   I2C_SendByte(IIC_Data);
   if(!I2C_WaitAck()){I2C_Stop(); return FALSE;}
   I2C_Stop();
   return TRUE;
}

void Initial_LY096BG30(void)
{
	Write_1106_Command(0xAE);    /*display off*/

	Write_1106_Command(0x02);    /*set lower column address 0x02 */
	Write_1106_Command(0x10);    /*set higher column address*/

	Write_1106_Command(0x40);    /*set display start line*/

	Write_1106_Command(0xB0);    /*set page address*/

	Write_1106_Command(0x81);    /*对比度设置*/
	Write_1106_Command(0xFF);    /*0~255，值越大，屏幕越亮*/

	Write_1106_Command(0xA1);    /*0xA1 -- 正常显示，0xA0 -- x轴反向*/

	Write_1106_Command(0xA6);    /*A6 -- 黑底点阵字;A7 -- 显示对象翻转*/

	Write_1106_Command(0xA8);    /*multiplex ratio*/
	Write_1106_Command(0x3F);    /*duty = 1/64*/

	Write_1106_Command(0xAD);    /*set charge pump enable*/
	Write_1106_Command(0x8B);    /*0x8B -- 内部供电，0x8a -- 外供VCC*/

	Write_1106_Command(0x30);    /*0X30---0X33  set VPP   8V */

	Write_1106_Command(0xC8);    /*Com scan direction   0XC8 */

	Write_1106_Command(0xD3);    /* set display offset */
	Write_1106_Command(0x00);

	Write_1106_Command(0xD5);    /* set osc division */
	Write_1106_Command(0x80);

	Write_1106_Command(0xD9);    /*set pre-charge period*/
	Write_1106_Command(0x1F);	  /*0x22*/

	Write_1106_Command(0xDA);    /*set COM pins*/
	Write_1106_Command(0x12);

	Write_1106_Command(0xDB);    /*set vcomh*/
	Write_1106_Command(0x40);

	Write_1106_Command(0xAF);    /*display ON*/ 
}

/********************************************
// fill_Picture
********************************************/
void fill_picture(unsigned char fill_Data)
{
	unsigned char m,n;
	for(m=0;m<8;m++)
	{
		Write_1106_Command(0xb0+m);		//page0-page1
		Write_1106_Command(0x00);		//low column start address
		Write_1106_Command(0x10);		//high column start address
		for(n=0;n<132;n++)
			{
				Write_1106_Data(fill_Data);
			}
	}
}
//设置起始点坐标
void SetPos(unsigned char x, unsigned char y)
{
  Write_1106_Command(0xb0+y);
  Write_1106_Command(((x&0xf0)>>4)|0x10);//|0x10
  Write_1106_Command((x&0x0f)|0x01);//|0x01
}

void SetDisplay(unsigned char x, unsigned char y, const char ch[])
{
  unsigned char c,i,j=0;
  while(ch[j] != '\0')
  {
    c = ch[j] - 32;
    if(x>126)
    {
      x=0;
      y++;
    }
    SetPos(x,y);
    for(i=0;i<6;i++)
    {
      Write_1106_Data(font6x8[c*6+i]);
    }
    x += 6;
    j++;
  }
}





