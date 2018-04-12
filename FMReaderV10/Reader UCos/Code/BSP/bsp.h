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
#include "SPI_IIC.h"
#include "app_cfg.h"
#include "timer1.h"
#include "TDA8007.h"
#include "FM1715.h"
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
  /* TDA8007 pins */
#define TDA8007_DATA_PORT 	GPIOD
#define TDA8007_DATA_LSB  	GPIO_Pin_0
#define TDA8007_ADDR_PORT 	GPIOE
#define TDA8007_ADDR_LSB  	GPIO_Pin_0
#define TDA8007_NCS_PORT  	GPIOB
#define TDA8007_NCS_PIN   	GPIO_Pin_5
#define TDA8007_NRD_PORT  	GPIOB
#define TDA8007_NRD_PIN   	GPIO_Pin_6
#define TDA8007_NWR_PORT  	GPIOB
#define TDA8007_NWR_PIN   	GPIO_Pin_7
#define TDA8007_ALE_PORT  	GPIOB
#define TDA8007_ALE_PIN   	GPIO_Pin_8
#define TDA8007_INT_PORT  	GPIOB
#define TDA8007_INT_PIN   	GPIO_Pin_9
#define TDA8007_EXTI_PORT  	GPIO_PortSourceGPIOB
#define TDA8007_EXTI_PIN   	GPIO_PinSource9
#define TDA8007_PWM_PORT  	GPIOA
#define TDA8007_PWM_PIN   	GPIO_Pin_1
#define TDA8007_POWEN_PORT	GPIOE
#define TDA8007_POWEN_PIN	GPIO_Pin_4	
  /* FM1715 pins */
#define FM1715_DATA_PORT 	GPIOD
#define FM1715_DATA_LSB  	GPIO_Pin_8
#define FM1715_NCS_PORT  	GPIOC
#define FM1715_NCS_PIN   	GPIO_Pin_9
#define FM1715_NRD_PORT  	GPIOC
#define FM1715_NRD_PIN   	GPIO_Pin_10

#define I2C2_SCL_PORT  		GPIOB
#define I2C2_SCL_PIN   		GPIO_Pin_10
#define I2C2_SDA_PORT  		GPIOB
#define I2C2_SDA_PIN   		GPIO_Pin_11

#define SPI1_CS_PORT  		GPIOA
#define SPI1_CS_PIN   		GPIO_Pin_4	//NSS≤ª π”√
#define SPI1_SCK_PORT  		GPIOA
#define SPI1_SCK_PIN   		GPIO_Pin_5
#define SPI1_MISO_PORT  	GPIOA
#define SPI1_MISO_PIN   	GPIO_Pin_6
#define SPI1_MOSI_PORT  	GPIOA
#define SPI1_MOSI_PIN   	GPIO_Pin_7

#define BUZ_PORT  			GPIOE
#define BUZ_PIN   			GPIO_Pin_8
#define BUTTON_PORT  		GPIOE
#define BUTTON_PIN   		GPIO_Pin_9

#define FM1715_NWR_PORT  	GPIOB
#define FM1715_NWR_PIN   	GPIO_Pin_12
#define FM1715_ALE_PORT  	GPIOB
#define FM1715_ALE_PIN   	GPIO_Pin_13
#define FM1715_RST_PORT  	GPIOB
#define FM1715_RST_PIN   	GPIO_Pin_14
#define FM1715_INT_PORT  	GPIOB
#define FM1715_INT_PIN   	GPIO_Pin_15
#define FM1715_EXTI_PORT  	GPIO_PortSourceGPIOB
#define FM1715_EXTI_PIN   	GPIO_PinSource15
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
void  BSP_Init (void);
int SendChar (int ch);
int GetKey (void);
void LEDShow(INT8U Index, BitAction BitVal);
void USB_Cable_Config (FunctionalState NewState);
void Enter_LowPowerMode(void);
void Leave_LowPowerMode(void);
void EXTI_Configuration_FM1715(FunctionalState State);
void EXTI_Configuration_TDA8007(FunctionalState State);
void TDA8007_Pow_Config (FunctionalState NewState);
void Set_USBClock(void);
void SPI_Config(void);
void BUZZ(u8 ms);
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
