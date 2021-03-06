/******************** (C) COPYRIGHT 2007 STMicroelectronics ********************
* File Name          : smartcard.h
* Author             : MCD Application Team
* Version            : V1.0
* Date               : 10/08/2007
* Description        : This file contains all the functions prototypes for the
*                      Smartcard firmware library.
********************************************************************************
* THE PRESENT SOFTWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH SOFTWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __SMARTCARD_H
#define __SMARTCARD_H

/* Includes ------------------------------------------------------------------*/
#include "stm32f10x.h"

/* Exported constants --------------------------------------------------------*/
#define T0_PROTOCOL        0x00  /* T0 protocol */
#define DIRECT             0x3B  /* Direct bit convention */
#define INDIRECT           0x3F  /* Indirect bit convention */
#define SETUP_LENGTH       20
#define HIST_LENGTH        20
#define LCmax              280

/* SC Tree Structure -----------------------------------------------------------
                              MasterFile
                           ________|___________
                          |        |           |
                        System   UserData     Note
------------------------------------------------------------------------------*/

/* SC ADPU Command: Operation Code -------------------------------------------*/
#define SC_CLA_GSM11       0xA0
/*------------------------ Data Area Management Commands ---------------------*/
#define SC_SELECT_FILE     0xA4
#define SC_GET_RESPONCE    0xC0
#define SC_STATUS          0xF2
#define SC_UPDATE_BINARY   0xD6
#define SC_READ_BINARY     0xB0
#define SC_WRITE_BINARY    0xD0
#define SC_UPDATE_RECORD   0xDC
#define SC_READ_RECORD     0xB2

/*-------------------------- Administrative Commands -------------------------*/ 
#define SC_CREATE_FILE     0xE0
/*-------------------------- Safety Management Commands ----------------------*/
#define SC_VERIFY          0x20
#define SC_CHANGE          0x24
#define SC_DISABLE         0x26
#define SC_ENABLE          0x28
#define SC_UNBLOCK         0x2C
#define SC_EXTERNAL_AUTH   0x82
#define SC_GET_CHALLENGE   0x84

/*-------------------------- Answer to reset Commands ------------------------*/ 
#define SC_GET_A2R         0x00

/* SC STATUS: Status Code ----------------------------------------------------*/
#define SC_EF_SELECTED     0x9F
#define SC_DF_SELECTED     0x9F
#define SC_OP_TERMINATED   0x9000

/* Smartcard Inteface GPIO pins */


#define SC_3_5V_PORT				GPIOD
#define SC_CLKDIV_2_PORT			GPIOD
#define SC_CLKDIV_1_PORT			GPIOD
#define SC_CMDVCC_PORT				GPIOD
#define SC_OFF_PORT					GPIOC
#define SC_RESET_PORT				GPIOC
#define SC_18V_PORT					GPIOC
#define SC_IO_PORT					GPIOB
#define SC_CLK_PORT					GPIOB

//Port D
#define SC_3_5V            GPIO_Pin_0   /* GPIOD Pin 0 */
#define SC_CLKDIV_2		   GPIO_Pin_1   /* GPIOD_Pin_1 */
#define SC_CLKDIV_1		   GPIO_Pin_2	  /* GPIOD_Pin_2 */
#define SC_CMDVCC          GPIO_Pin_3   /* GPIOD Pin 3 */

//Port C
#define SC_OFF             GPIO_Pin_10  /* GPIOC Pin 10 */ 
#define SC_RESET           GPIO_Pin_11  /* GPIOC Pin_11 */
#define SC_18V						 GPIO_Pin_12	/* GPIOC_Pin_12 */

//Port B
#define SC_IO              GPIO_Pin_10  /* GPIOB Pin 10 */
#define SC_CLK             GPIO_Pin_12  /* GPIOB Pin 12 */


/* Smartcard Voltage */
#define SC_Voltage_5V      0
#define SC_Voltage_3V      1
#define SC_Voltage_18V		 2 


#define SC_CLK_18M  0x01
#define SC_CLK_9M   0x02
#define SC_CLK_6M   0x03
#define SC_CLK_4P5M 0x04
#define SC_CLK_3P6M 0x05
#define SC_CLK_3M   0x06
#define APB1_FREQ  36000000

#define SC_CLK_STOP_H 0x01
#define SC_CLK_STOP_L 0x00
#define SC_CLK_ENABLE 0x01
#define SC_CLK_DISABLE 0x00

#define SC_SUCCESS  0
#define SC_ERROR    1


#define SC_PowerON   GPIO_ResetBits(SC_CMDVCC_PORT, SC_CMDVCC)
#define SC_PowerOFF  GPIO_SetBits(SC_CMDVCC_PORT, SC_CMDVCC)

#define SC_ResetON 	 GPIO_SetBits(SC_RESET_PORT, SC_RESET)
#define SC_ResetOFF  GPIO_ResetBits(SC_RESET_PORT, SC_RESET)

/* ATR structure - Answer To Reset -------------------------------------------*/
typedef struct
{
  u8 TS;               /* Bit Convention */
  u8 T0;               /* High nibble = Number of setup byte; low nibble = Number of historical byte */
  u8 T[SETUP_LENGTH];  /* Setup array */
  u8 H[HIST_LENGTH];   /* Historical array */
  u8 Tlength;          /* Setup array dimension */
  u8 Hlength;          /* Historical array dimension */
} SC_ATR;

/* ADPU-Header command structure ---------------------------------------------*/
typedef struct
{
  u8 CLA;  /* Command class */
  u8 INS;  /* Operation code */
  u8 P1;   /* Selection Mode */
  u8 P2;   /* Selection Option */
} SC_Header;

/* ADPU-Body command structure -----------------------------------------------*/
typedef struct 
{
  u8 LC;           /* Data field length */
  u8 Data[LCmax];  /* Command parameters */
  u16 LE;           /* Expected length of data to be returned */
} SC_Body;

/* ADPU Command structure ----------------------------------------------------*/
typedef struct
{
  SC_Header Header;
  SC_Body Body;
} SC_ADPU_Commands;

/* SC response structure -----------------------------------------------------*/
typedef struct
{
  u8 Data[LCmax];  /* Data returned from the card */
  u8 SW1;          /* Command Processing status */
  u8 SW2;          /* Command Processing qualification */
} SC_ADPU_Responce;

typedef struct
{
  u32 F; //ETU=F/D
  u8 D; 
  u8 FreqDiv; //CT CLK Frequecy (PCK1 Freq)/(PrescalerValue*2)
  u8 GuardTime; //number of ETU
  u32 AtrTimeout; //number of ETU timeout
  u32 WaitTimeout; //number of ETU timeout 
  u32 clk_cnt; //Restore CLK number,add by timer
}SC_TimTypeDef;


/* Exported macro ------------------------------------------------------------*/
/* Exported functions ------------------------------------------------------- */
/* APPLICATION LAYER ---------------------------------------------------------*/

void SC_mDelay(u32 ms);
void SC_uDelay(u32 us);
u8 SC_PPS(u8 PPS1);
void SC_Init(void);
void SC_DeInit(void);
void SC_VoltageConfig(u32 SC_Voltage);
void SC_ETUConfig();
void SC_ClkCmd(u8 stat);
void SC_SendByte(u8 dat);
u8 SC_RecvByte(u8 *dat,u32 TimeOut);
u8 SC_decode_Answer2reset(u8 *card);
u8 SC_ColdReset(u8 *atr,u8 *len);
u8 SC_WarmReset(u8 *atr,u8 *len);
u8 SC_DataTrancive(u8 *buffer,u8 sendlen,u8* recelen);
void SC_InterruptHandler(void);
void SC_ApduExchange(SC_ADPU_Commands *SC_ADPU, SC_ADPU_Responce *SC_ResponceStatus);
void SC_TimConfig(void);
extern SC_ATR SC_A2R;
extern u8 SC_ATR_Table[40];
extern SC_ADPU_Commands apdu_commands;
extern SC_ADPU_Responce apdu_responce;
extern SC_TimTypeDef sc_tim;
extern u32 Fi[16];
extern u8 Di[16];



#endif /* __SMARTCARD_H */

/******************* (C) COPYRIGHT 2007 STMicroelectronics *****END OF FILE****/
