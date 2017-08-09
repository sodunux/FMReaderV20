
/**********************************************
* IO口模拟I2C的相关定义以及函数
*
*
*
************************************************/
#ifndef _I2C_H_
#define _I2C_H_

#define  I2C_FSMode		0//I2C 快速模式
#define  I2C_HSMode		1//I2C 高速模式3.4MHz
#define  I2C_MASTERCODE	0x08		//高速模式下的MasterCode

#define  I2C_ADDR_DEF			0X28  // 0x2F; V1.0中使用   //0x28  原始

#define I2C_SDA_ON			  	  (GI2C->BSRR = I2C_SDA)			//GPIO_SetBits(GPIOB,I2C_SDA)  
#define I2C_SDA_OFF			  	  (GI2C->BRR = I2C_SDA)				//GPIO_ResetBits(GPIOB,I2C_SDA)
#define I2C_SCL_ON			  	  (GI2C->BSRR = I2C_SCL)     		//GPIO_SetBits(GPIOB,I2C_SCL)
#define I2C_SCL_OFF			  	  (GI2C->BRR = I2C_SCL)				//GPIO_ResetBits(GPIOB,I2C_SCL)


#define Get_SDA_Status			  ((GI2C->IDR & I2C_SDA)?1:0) 		//GPIO_ReadInputDataBit(GI2C,I2C_SDA)

extern uchar SDA_DELAY;  		   //SDA数据保持时间
extern uchar SCL_DELAY;	 		   //SCL沿变之后，延时SCL_DELAY，SDA再变
extern uchar STA_DELAY;	           //SDA在产生STA时序后，SCL保持STA_DELAY
extern uchar STO_DELAY;	 		   //SDA在产生STO时序后，SCL保持STA_DELAY

extern uchar I2C_MODE;
extern uchar I2C_ADDR;

extern void I2C_START(void); 
extern void I2C_START_HS(void);
extern void I2C_STOP(void); 

extern void I2C_SEND_DATA(u8 i2cdata);
extern void I2C_SEND_ACK(u8 ack);

extern u8 I2C_RECV_DATA(void);
extern u8 I2C_RECV_ACK(void);

extern u8 I2C_Write_FIFO(u8* buflen,u8* dstbuf);  //I2C接口连续写FIFO
extern u8 I2C_Read_FIFO(u8* fflen,u8* ffbuf);	  //I2C接口连续读FIFO

extern void I2C_SEND_DATA_HS(u8 i2cdata);
extern u8   I2C_RECV_DATA_HS(void);
extern u8   I2C_RECV_ACK_HS(void);

#endif
