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
* Filename      : bsp.c
* Version       : V1.10
* Programmer(s) : BAN
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                             INCLUDE FILES
*********************************************************************************************************
*/

#define  BSP_MODULE
#include "includes.h"
#include "bsp.h"



//u32 SystemCoreClock;

/*
*********************************************************************************************************
*                                            LOCAL DEFINES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                           LOCAL CONSTANTS
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                          LOCAL DATA TYPES
*********************************************************************************************************
*/
volatile ErrorStatus HSEStartUpStatus = SUCCESS;
extern uint8_t bUsbStatus;
/*
*********************************************************************************************************
*                                            LOCAL TABLES
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                       LOCAL GLOBAL VARIABLES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                      LOCAL FUNCTION PROTOTYPES
*********************************************************************************************************
*/
void RCC_Config(void);
void SysTick_Configuration(void);
void GPIO_Config(void);
void NVIC_Config(void);
void USART1_Configuration(void);
void USART1_GPIO_Configuration(void);
extern void sFLASH_Init(void);

/*
*********************************************************************************************************
*                                     LOCAL CONFIGURATION ERRORS
*********************************************************************************************************
*/


/*
*********************************************************************************************************
*                                               BSP_Init()
*
* Description : Initialize the Board Support Package (BSP).
*
* Argument(s) : none.
*
* Return(s)   : none.
*
* Caller(s)   : Application.
*
* Note(s)     : (1) This function SHOULD be called before any other BSP function is called.
*********************************************************************************************************
*/

void  BSP_Init (void)
{
	u8 i;
	RCC_Config();
	SysTick_Configuration();
	GPIO_Config();
	SPI_Config();
	NVIC_Config();
	//USART1_GPIO_Configuration();
	//USART1_Configuration();
	//sFLASH_Init();

  Set_USBClock();
	USB_Init();

  	while(bUsbStatus < 1);

  	CCIDInit();
  	InRegsInit();

	for(i=1; i<5; i++)
	{
		LEDShow(i,Bit_SET);	
	}


	bCardSlotStatus = CCID_CARD_SLOT_PRESENT;
  	// bCardSlotStatus = CARD_SLOT_ABSENT;
  	CCIDCardSlotChange(); 
	CLCardPowerOff();
	Mf500PcdConfig();
	CLCardPowerOff();

}
/*
*********************************************************************************************************
*                                            SysTick_Configuration()
*
*********************************************************************************************************
*/
void SysTick_Configuration(void)
{
    RCC_ClocksTypeDef  RCC_Clocks;
    u32  cnts;
    RCC_GetClocksFreq(&RCC_Clocks);   
    cnts = (u32)RCC_Clocks.HCLK_Frequency/OS_TICKS_PER_SEC;
	SysTick_CLKSourceConfig(SysTick_CLKSource_HCLK);
	SysTick_Config(cnts);

/*	SystemCoreClock=(u32)RCC_Clocks.HCLK_Frequency;
    SysTick_SetReload(cnts);
    SysTick_CLKSourceConfig(SysTick_CLKSource_HCLK);
    SysTick_CounterCmd(SysTick_Counter_Enable);
    SysTick_ITConfig(ENABLE);
*/
}

/*******************************************************************************
* Function Name  : RCC_Config
* Description    : Configures Main system clocks & power
* Input          : None.
* Return         : None.
*******************************************************************************/
void RCC_Config(void)
{
  /* SYSCLK, HCLK, PCLK2 and PCLK1 configuration -----------------------------*/   
  /* RCC system reset(for debug purpose) */
  RCC_DeInit();

  /* Enable HSE */
  RCC_HSEConfig(RCC_HSE_ON);

  /* Wait till HSE is ready */
  HSEStartUpStatus = RCC_WaitForHSEStartUp();

  if (HSEStartUpStatus == SUCCESS)
  {
    /* Enable Prefetch Buffer */
    FLASH_PrefetchBufferCmd(FLASH_PrefetchBuffer_Enable);

    /* Flash 2 wait state */
    FLASH_SetLatency(FLASH_Latency_2);
 
    /* HCLK = SYSCLK */
    RCC_HCLKConfig(RCC_SYSCLK_Div1); 
  
    /* PCLK2 = HCLK */
    RCC_PCLK2Config(RCC_HCLK_Div1); 

    /* PCLK1 = HCLK/2 */
    RCC_PCLK1Config(RCC_HCLK_Div2);

    /* PLLCLK = 8MHz * 9 = 72 MHz */
    RCC_PLLConfig(RCC_PLLSource_HSE_Div1, RCC_PLLMul_9);

    /* Enable PLL */ 
    RCC_PLLCmd(ENABLE);

    /* Wait till PLL is ready */
    while (RCC_GetFlagStatus(RCC_FLAG_PLLRDY) == RESET)
    {
    }

    /* Select PLL as system clock source */
    RCC_SYSCLKConfig(RCC_SYSCLKSource_PLLCLK);

    /* Wait till PLL is used as system clock source */
    while(RCC_GetSYSCLKSource() != 0x08)
    {
    }
    RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART3|RCC_APB1Periph_TIM2|RCC_APB1Periph_TIM3|RCC_APB1Periph_TIM4, ENABLE);
 	RCC_APB2PeriphClockCmd(RCC_APB2Periph_TIM1|RCC_APB2Periph_SPI1,ENABLE);
  }
  else
  { /* If HSE fails to start-up, the application will have wrong clock configuration.
       User can add here some code to deal with this error */    

    /* Go to infinite loop */
    while (1)
    {
    }
  }
}


/*******************************************************************************
* Function Name  : GPIOPinxConfig
* Description    : Configures one GPIO pin used by MCU.
* Input          : GPIOx    : GPIO port.
                   GPIOPinx : GPIO pin.
                   Speed    : GPIO Pin speed.
                   Mode     : GPIO Pin mode.
* Return         : None.
*******************************************************************************/
void GPIOPinxConfig(GPIO_TypeDef* GPIOx, INT16U GPIOPinx, GPIOSpeed_TypeDef Speed, GPIOMode_TypeDef Mode)
{
	GPIO_InitTypeDef GPIO_InitStructure;
	
	GPIO_InitStructure.GPIO_Pin = GPIOPinx;
	GPIO_InitStructure.GPIO_Speed = Speed;
	GPIO_InitStructure.GPIO_Mode = Mode;
	GPIO_Init(GPIOx, &GPIO_InitStructure);      
}

/*******************************************************************************
* Function Name  : GPIOPinxyConfig
* Description    : Configures several GPIO pins used by MCU.
* Input          : GPIOx      : GPIO port.
                   GPIOPinLSB : Least GPIO pin.
                   Speed      : GPIO Pin speed.
                   Mode       : GPIO Pin mode.
                   Number     : How many pins will be configured.
* Return         : None.
*******************************************************************************/
void GPIOPinxyConfig(GPIO_TypeDef* GPIOx, INT16U GPIOPinLSB, GPIOSpeed_TypeDef Speed, GPIOMode_TypeDef Mode, INT8U Number)
{
	GPIO_InitTypeDef GPIO_InitStructure;
	INT16U  GPIOPinTemp;
	INT8U   b;
	
	GPIOPinTemp = GPIOPinLSB;
	GPIO_InitStructure.GPIO_Speed = Speed;
	GPIO_InitStructure.GPIO_Mode = Mode;
	for(b=0; b<Number; b++)
	{
		GPIO_InitStructure.GPIO_Pin = GPIOPinTemp;
		GPIO_Init(GPIOx, &GPIO_InitStructure);  
		GPIOPinTemp <<= 1;    
	}
}

/*******************************************************************************
* Function Name  : GPIO_Config
* Description    : Configures GPIOs used by MCU.
* Input          : None.
* Return         : None.
*******************************************************************************/
void GPIO_Config(void)
{
	
	u32  RCC_APB2Periph_GPIOx;
	
	/* All GPIO ports will be initialzed */
	RCC_APB2Periph_GPIOx = RCC_APB2Periph_GPIOA;
	RCC_APB2Periph_GPIOx |= RCC_APB2Periph_GPIOB;
	RCC_APB2Periph_GPIOx |= RCC_APB2Periph_GPIOC;
	RCC_APB2Periph_GPIOx |= RCC_APB2Periph_GPIOD;
	RCC_APB2Periph_GPIOx |= RCC_APB2Periph_GPIOE;
	
	/* Enable GPIO ports clock */
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOx | RCC_APB2Periph_AFIO, ENABLE);
	
	/* Configure pins */
	GPIOPinxConfig(USB_DISCONNECT_PORT, USB_DISCONNECT_PIN, GPIO_Speed_10MHz, GPIO_Mode_Out_OD);
	GPIO_SetBits(USB_DISCONNECT_PORT, USB_DISCONNECT_PIN);

	GPIOPinxyConfig(TDA8007_DATA_PORT, TDA8007_DATA_LSB, GPIO_Speed_50MHz, GPIO_Mode_Out_PP, 8);
	GPIOPinxyConfig(TDA8007_ADDR_PORT, TDA8007_ADDR_LSB, GPIO_Speed_50MHz, GPIO_Mode_Out_PP, 4);
	GPIOPinxConfig(TDA8007_NCS_PORT, TDA8007_NCS_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TDA8007_NRD_PORT, TDA8007_NRD_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TDA8007_NWR_PORT, TDA8007_NWR_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TDA8007_ALE_PORT, TDA8007_ALE_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TDA8007_INT_PORT, TDA8007_INT_PIN, GPIO_Speed_50MHz, GPIO_Mode_IPU);
	GPIOPinxConfig(TDA8007_PWM_PORT, TDA8007_PWM_PIN, GPIO_Speed_50MHz, GPIO_Mode_AF_PP);
	GPIOPinxConfig(TDA8007_POWEN_PORT, TDA8007_POWEN_PIN, GPIO_Speed_10MHz, GPIO_Mode_Out_OD);
	
	GPIOPinxConfig(BUZ_PORT, BUZ_PIN, GPIO_Speed_10MHz, GPIO_Mode_Out_OD);		//gao
	GPIO_SetBits(BUZ_PORT, BUZ_PIN);
	GPIOPinxConfig(BUTTON_PORT, BUTTON_PIN, GPIO_Speed_10MHz, GPIO_Mode_IPU);
	
	//  GPIOPinxConfig(I2C2_SCL_PORT, I2C2_SCL_PIN|I2C2_SDA_PIN, GPIO_Speed_50MHz, GPIO_Mode_AF_OD);
	
	GPIOPinxConfig(SPI1_CS_PORT, SPI1_CS_PIN, GPIO_Speed_10MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(SPI1_SCK_PORT, SPI1_SCK_PIN|SPI1_MOSI_PIN, GPIO_Speed_50MHz, GPIO_Mode_AF_PP);
	GPIOPinxConfig(SPI1_MISO_PORT, SPI1_MISO_PIN, GPIO_Speed_50MHz, GPIO_Mode_IN_FLOATING); //zhangpei
	
	GPIOPinxyConfig(FM1715_DATA_PORT, FM1715_DATA_LSB, GPIO_Speed_50MHz, GPIO_Mode_Out_PP, 8);
	GPIOPinxConfig(FM1715_NCS_PORT, FM1715_NCS_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(FM1715_NRD_PORT, FM1715_NRD_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(FM1715_NWR_PORT, FM1715_NWR_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(FM1715_ALE_PORT, FM1715_ALE_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(FM1715_RST_PORT, FM1715_RST_PIN, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	// GPIOPinxConfig(FM1715_INT_PORT, FM1715_INT_PIN, GPIO_Speed_10MHz, GPIO_Mode_AF_OD);
	GPIOPinxConfig(FM1715_INT_PORT, FM1715_INT_PIN, GPIO_Speed_50MHz, GPIO_Mode_IPU);
	
	//GPIO_SetBits(FM1715_INT_PORT, FM1715_INT_PIN);
	
	
	GPIOPinxConfig(LED_PORT, LED_PIN_1, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(LED_PORT, LED_PIN_2, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(LED_PORT, LED_PIN_3, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(LED_PORT, LED_PIN_4, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	
	GPIOPinxConfig(TRIG_PORT, TRIG_PIN_1, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TRIG_PORT, TRIG_PIN_2, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TRIG_PORT, TRIG_PIN_3, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);
	GPIOPinxConfig(TRIG_PORT, TRIG_PIN_4, GPIO_Speed_50MHz, GPIO_Mode_Out_PP);

	/* I2C GPIO config, zhangpei */
	I2C_GPIO_Config();

}

/*******************************************************************************
* Function Name  : LEDShow.
* Description    : LED pins level output.
* Input          : Index      :   LED Index 1~4.
                   BitAction  : Bit_RESET or Bit_SET.
* Output         : None.
* Return         : None.
*******************************************************************************/
void LEDShow(INT8U Index, BitAction BitVal)
{
	if(BitVal == Bit_SET)  
	{
		if(Index == 1)
		  	GPIO_SetBits(LED_PORT, LED_PIN_1);
		else if(Index == 2)
		  	GPIO_SetBits(LED_PORT, LED_PIN_2);      
		else if(Index == 3)
		  	GPIO_SetBits(LED_PORT, LED_PIN_3);
		else if(Index == 4)
		  	GPIO_SetBits(LED_PORT, LED_PIN_4);
	}
	else
	{
		if(Index == 1)
		  	GPIO_ResetBits(LED_PORT, LED_PIN_1);
		else if(Index == 2)
		  	GPIO_ResetBits(LED_PORT, LED_PIN_2);      
		else if(Index == 3)
		  	GPIO_ResetBits(LED_PORT, LED_PIN_3);
		else if(Index == 4)
		  	GPIO_ResetBits(LED_PORT, LED_PIN_4);  
	}
    
}
/*******************************************************************************
* Function Name  : SetBUZ
* Description    : SetBUZ status
* Input          : None.
* Return         : None.
*******************************************************************************/
void SetBUZ(u8 status)
{
	if(status == 0)
		GPIO_SetBits(BUZ_PORT, BUZ_PIN);	
	else
		GPIO_ResetBits(BUZ_PORT, BUZ_PIN);
}
/*******************************************************************************
* Function Name  : BUZZ
* Description    : BUZZ for (ms)ms
* Input          : ms.
* Return         : None.
*******************************************************************************/
void BUZZ(u8 ms)
{
	SetBUZ(1);
	OSTimeDlyHMSM(0,0,0,ms);
	SetBUZ(0);
}

/*******************************************************************************
* Function Name  : NVIC_Config
* Description    : Configures the interrupts
* Input          : None.
* Return         : None.
*******************************************************************************/
void NVIC_Config(void)
{

	/*  NVIC_InitTypeDef NVIC_InitStructure;
	
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);
	
	NVIC_InitStructure.NVIC_IRQChannel = SysTick_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	*/

	NVIC_InitTypeDef NVIC_InitStructure;
	
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);
	
	NVIC_InitStructure.NVIC_IRQChannel = USB_LP_CAN1_RX0_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);

	NVIC_InitStructure.NVIC_IRQChannel = EXTI9_5_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	NVIC_InitStructure.NVIC_IRQChannel = EXTI15_10_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 1;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	NVIC_InitStructure.NVIC_IRQChannel = TIM3_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 3;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	NVIC_InitStructure.NVIC_IRQChannel = TIM1_CC_IRQn;			 //gao
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 4;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	NVIC_InitStructure.NVIC_IRQChannel = SPI1_IRQn;			 //gao
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 5;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
/*	
	NVIC_InitStructure.NVIC_IRQChannel = I2C2_EV_IRQn;			 //gao
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 6;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
*/


	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);
	NVIC_SetPriority(SysTick_IRQn, 0);  


}

 //串口初始化函数
void USART1_Configuration(void)                      
{
	//串口参数初始化  
	USART_InitTypeDef USART_InitStructure;               
	//初始化参数设置
	USART_DeInit(USART1);
	USART_InitStructure.USART_BaudRate = 9600;                              //波特率9600
	USART_InitStructure.USART_WordLength = USART_WordLength_8b; //字长8位
	USART_InitStructure.USART_StopBits = USART_StopBits_1;                  // 1位停止字节
	USART_InitStructure.USART_Parity = USART_Parity_No;                    //无奇偶校验
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//无流控制
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;//打开Rx接收和Tx发送功能

	USART_Init(USART1, &USART_InitStructure);                                          //初始化
	USART_Cmd(USART1, ENABLE);                                                        //启动串口
}

void USART1_GPIO_Configuration(void)
{
	GPIO_InitTypeDef GPIO_InitStructure;
	// Configure USART1_Tx as alternate function push-pull 
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
	GPIO_Init(GPIOA, &GPIO_InitStructure);
	// Configure USART1_Rx as input floating 
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
	GPIO_Init(GPIOA, &GPIO_InitStructure); // 原来为usart3,改为usart1，以免和I2C2冲突，zhangpei  
}

/*******************************************************************************
* Function Name  : SPI_Config
* Description    : Configures SPI and Initializes SPI
* Input          : None.
* Return         : None.
* By 			 : gao
*******************************************************************************/
void SPI_Config(void)
{
	SPI_InitTypeDef SPI_InitStructure;
	/* SPI1 configuration ------------------------------------------------------*/
	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;
	SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;
	SPI_InitStructure.SPI_CPOL = SPI_CPOL_High;
	SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;
	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;
	SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_64;
	SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;
	SPI_InitStructure.SPI_CRCPolynomial = 7;
	SPI_Init(SPI1, &SPI_InitStructure);

	/* Enable SPI1 */
	SPI_Cmd(SPI1, ENABLE);
	SPI1_CS_H(); 
}
/*******************************************************************************
* Function Name  : USB_Cable_Config
* Description    : Software Connection/Disconnection of USB Cable
* Input          : None.
* Return         : Status
*******************************************************************************/
void USB_Cable_Config (FunctionalState NewState)
{
	if (NewState != DISABLE)
	{
		GPIO_ResetBits(USB_DISCONNECT_PORT, USB_DISCONNECT_PIN);
	}
	else
	{
		GPIO_SetBits(USB_DISCONNECT_PORT, USB_DISCONNECT_PIN);
	}
}
/*******************************************************************************
* Function Name  : Set_USBClock
* Description    : Configures USB Clock input (48MHz)
* Input          : None.
* Return         : None.
*******************************************************************************/
void Set_USBClock(void)
{
  /* Select USBCLK source */
  RCC_USBCLKConfig(RCC_USBCLKSource_PLLCLK_1Div5);
  
  /* Enable the USB clock */
  RCC_APB1PeriphClockCmd(RCC_APB1Periph_USB, ENABLE);
}

/*******************************************************************************
* Function Name  : Enter_LowPowerMode
* Description    : Power-off system clocks and power while entering suspend mode
* Input          : None.
* Return         : None.
*******************************************************************************/
void Enter_LowPowerMode(void)
{
	/* Set the device state to suspend */
	bDeviceState = SUSPENDED;		
}

/*******************************************************************************
* Function Name  : Leave_LowPowerMode
* Description    : Restores system clocks and power while exiting suspend mode
* Input          : None.
* Return         : None.
*******************************************************************************/
void Leave_LowPowerMode(void)
{
	DEVICE_INFO *pInfo = &Device_Info;
	
	/* Set the device state to the correct state */
	if (pInfo->Current_Configuration != 0)
	{
		/* Device configured */
		bDeviceState = CONFIGURED;
	}
	else
	{
		bDeviceState = ATTACHED;
	}
}


/*******************************************************************************
* Function Name : EXTI_Configuration_TDA8007.
* Description   : Configure the EXTI lines for device interrupt of TDA8007.
* Input         : State : Enable or Disable.
* Output        : None.
* Return value  : The direction value.
*******************************************************************************/
void EXTI_Configuration_TDA8007(FunctionalState State)
{
	EXTI_InitTypeDef EXTI_InitStructure;

	GPIO_EXTILineConfig(TDA8007_EXTI_PORT, TDA8007_EXTI_PIN);
	/* Configure Key EXTI line to generate an interrupt on rising & falling edges */  
	EXTI_InitStructure.EXTI_Line = TDA8007_INT_PIN;
	EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt;
	EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling;
	EXTI_InitStructure.EXTI_LineCmd = State;
	EXTI_Init(&EXTI_InitStructure);

	/* Clear the Key EXTI line pending bit */
	EXTI_ClearITPendingBit(TDA8007_INT_PIN);
}
/*******************************************************************************
* Function Name  : TDA8007_Pow_Config
* Description    : Software Connection/Disconnection of TDA8007
* Input          : None.
* Return         : Status
*******************************************************************************/
void TDA8007_Pow_Config (FunctionalState NewState)
{
	if (NewState != DISABLE)
	{
		GPIO_ResetBits(TDA8007_POWEN_PORT, TDA8007_POWEN_PIN);
	}
	else
	{
		GPIO_SetBits(TDA8007_POWEN_PORT, TDA8007_POWEN_PIN);
	}
}


/*******************************************************************************
* Function Name : EXTI_Configuration_FM1715.
* Description   : Configure the EXTI lines for device interrupt of FM1715.
* Input         : State : Enable or Disable.
* Output        : None.
* Return value  : The direction value.
*******************************************************************************/
void EXTI_Configuration_FM1715(FunctionalState State)
{
	EXTI_InitTypeDef EXTI_InitStructure;

	GPIO_EXTILineConfig(FM1715_EXTI_PORT, FM1715_EXTI_PIN);
	EXTI_InitStructure.EXTI_Line = FM1715_INT_PIN;
	EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt;
	EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling;
	EXTI_InitStructure.EXTI_LineCmd = State;
	EXTI_Init(&EXTI_InitStructure);

	EXTI_ClearITPendingBit(FM1715_INT_PIN);
}



int SendChar (int ch)  
{
	while (!(USART1->SR & USART_FLAG_TXE)); // USART1 可换成你程序中通信的串口
	USART1->DR = (ch & 0x1FF);
	return (ch);
}

int GetKey (void)  
{
	while (!(USART1->SR & USART_FLAG_RXNE));
	return ((int)(USART1->DR & 0x1FF));
}
