/*
*********************************************************************************************************
*                                     MICIRUM BOARD SUPPORT PACKAGE
*
*                             (c) Copyright 2007; Micrium, Inc.; Weston, FL
*
*               All rights reserved.  Protected by international copyright laws.
*               Knowledge of the source code may NOT be used to develop a similar product.
*               Please help us continue to provide the Embedded community with the finest
*               software available.  Your honesty is greatly appreciated.
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*
*                                        BOARD SUPPORT PACKAGE
*
*                                     ST Microelectronics STM32
*                                              with the
*                                   STM3210B-EVAL Evaluation Board
*
* Filename      : bsp.h
* Version       : V1.10
* Programmer(s) : BAN
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                                 MODULE
*
* Note(s) : (1) This header file is protected from multiple pre-processor inclusion through use of the
*               BSP present pre-processor macro definition.
*********************************************************************************************************
*/

#ifndef  BSP_PRESENT
#define  BSP_PRESENT


/*
*********************************************************************************************************
*                                              INCLUDE FILES
*********************************************************************************************************
*/


#include "stm32f10x.h"
#include "app_cfg.h"
#include "timer1.h"
#include "ucos_ii.h"


/*
*********************************************************************************************************
*                                               INT DEFINES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                             PERIPH DEFINES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                               DATA TYPES
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                            GLOBAL VARIABLES
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                                 MACRO'S
*********************************************************************************************************
*/
  /* USB disconnect pin */
#define USB_DISCONNECT_PORT GPIOC
#define USB_DISCONNECT_PIN  GPIO_Pin_8
  /* TDA8035 pins define in smartcard.h */



  /* LED pins */
#define LED_PORT         	GPIOC
#define LED_PIN_1        	GPIO_Pin_0
#define LED_PIN_2        	GPIO_Pin_1
#define LED_PIN_3        	GPIO_Pin_2
#define LED_PIN_4        	GPIO_Pin_3
  /* Triggle pins */
#define TRIG_PORT         	GPIOC
#define TRIG_PIN_1        	GPIO_Pin_4
#define TRIG_PIN_2        	GPIO_Pin_5
#define TRIG_PIN_3        	GPIO_Pin_6
#define TRIG_PIN_4        	GPIO_Pin_7


/*
*********************************************************************************************************
*                                           FUNCTION PROTOTYPES
*********************************************************************************************************
*/
void BSP_Init (void);
int SendChar (int ch);
int GetKey(void);
void LEDShow(INT8U Index, BitAction BitVal);
void USB_Cable_Config (FunctionalState NewState);
void Enter_LowPowerMode(void);
void Leave_LowPowerMode(void);

void Set_USBClock(void);
void SPI_Config(void);
void RCC_Config(void);
void GPIO_Config(void);
void IIC_Config(void);
void SPI_Config(void);

/*
*********************************************************************************************************
*                                           INTERRUPT SERVICES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                     PERIPHERAL POWER/CLOCK SERVICES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                              LED SERVICES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                               PB SERVICES
*********************************************************************************************************
*/



/*
*********************************************************************************************************
*                                              ADC SERVICES
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                           JOYSTICK SERVICES
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                               MODULE END
*********************************************************************************************************
*/

#endif                                                          /* End of module include.                               */
