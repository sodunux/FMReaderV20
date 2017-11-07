#ifndef __IIC_H__
#define __IIC_H__

#include "includes.h"

#define IIC_PORT			GPIOB
#define IIC_SCL				GPIO_Pin_6
#define IIC_SDA				GPIO_Pin_7
#define SCL_H				IIC_PORT->BSRR	= IIC_SCL
#define SCL_L				IIC_PORT->BRR 	= IIC_SCL
#define SDA_H				IIC_PORT->BSRR	= IIC_SDA
#define SDA_L				IIC_PORT->BRR 	= IIC_SDA
#define SCL_READ			IIC_PORT->IDR	& IIC_SCL
#define SDA_READ			IIC_PORT->IDR	& IIC_SDA

void IIC_Delay(void);
bool IIC_Start();
void IIC_Stop();
void IIC_Ack();
void IIC_NoAck();
bool IIC_WaitAck();
void IIC_SendByte(u8 sendbyte);
u8 IIC_ReadByte();
bool IIC_SendData(u8 addr,u8 *senddata,u8 len);
bool IIC_ReadData(u8 addr,u8 *readbuf,u8 len);

#endif