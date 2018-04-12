#ifndef _SPI_IIC_H_
#define _SPI_IIC_H_
 
/* Includes ------------------------------------------------------------------*/
#include "includes.h"
#include "stm32f10x_i2c.h"
#include "stm32f10x_spi.h"
#include "stm32f10x_it.h"


/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
#define I2C2_BufferSize		  512
#define SPI1_BufferSize		  512
#define sFLASH_SPI			  SPI1

#define SPI1_CS_H();    	SPI1_CS_PORT->BSRR = SPI1_CS_PIN;
#define SPI1_CS_L();  		SPI1_CS_PORT->BRR = SPI1_CS_PIN;

/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
extern uint8_t 		I2C2_SLAVE_ADDRESS7;
extern uint32_t 	I2C2_ClockSpeed;
extern uint8_t		I2C2_Buffer_Tx[];
extern uint8_t		I2C2_Buffer_Rx[];
//extern uint8_t		SPI1_Buffer_Tx[];
//extern uint8_t		SPI1_Buffer_Rx[];

extern uint8_t	I2C2_SendLen;

extern uint8_t volatile 	I2C2_Tx_Idx, I2C2_Rx_Idx, I2C2_PEC_Value;
extern uint8_t volatile		SPI1_Tx_Idx, SPI1_Rx_Idx;

void I2C_GPIO_Config(void);
void SPI_Send(uint8_t *SPI1_SendBuffer, uint16_t SPI1_SendLen);
void SPI_Receive(u8 *ReceiveBuffer, u16 ReceiveLen);


void I2C_HW_Send(uint8_t *I2C_SendBuffer, uint16_t ByteToSend);

void I2C_StartSend(void);
void I2C_EE_BufferRead(uint8_t* pBuffer, uint8_t ReadAddr, uint16_t NumByteToRead);
void ReadCurByte(uint8_t *DataByte);

void I2C_delay(void);
void I2C_delay_1(void);
void I2C_Stop(void); 
bool I2C_Send(u8 *address, u8* pBuffer, u8 length);
bool I2C_Receive(u8 *address, u8* pBuffer, u8 length);
void I2C_GPIO_Config(void);
u8 I2C_ReadOneByte24(void);
bool I2C_ReadBytes24(uint8_t* pBuffer,uint8_t length, uint8_t ReadAddress);


void fill_picture(unsigned char fill_Data);
void Initial_LY096BG30(void);
void SetDisplay(unsigned char x, unsigned char y, const char ch[]);
bool Write_1106_Command(unsigned char IIC_Command);





#endif
