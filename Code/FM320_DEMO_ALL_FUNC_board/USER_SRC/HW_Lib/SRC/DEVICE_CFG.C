/* Includes ------------------------------------------------------------------*/
//#include "DEVICE_CFG.h"
//#include "stm32f10x_rcc.h"

#include "stm32f10x_lib.h"
#include "define.h"
#include "global.h"


void USART_CHG_BAUD(USART_TypeDef* USARTx, u32 baud)
{
  USART_InitStructure.USART_BaudRate = baud;
  USART_InitStructure.USART_WordLength = USART_WordLength_8b;
  USART_InitStructure.USART_StopBits = USART_StopBits_1;
  USART_InitStructure.USART_Parity = USART_Parity_No;
  USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
  USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;
  USART_Init(USARTx, &USART_InitStructure);	
}

/*******************************************************************************
* Function Name  : RCC_Config
* Description    : Configures Main system clocks & power
* Input          : None.
* Return         : None.
*******************************************************************************/
void RCC_Config(void)
{
  u32 Clk_Enable_Sel1,Clk_Enable_Sel2;

  /* RCC system reset(for debug purpose) */
  RCC_DeInit();

  /* Enable HSE */
  RCC_HSEConfig(RCC_HSE_ON);	   //使能外部晶振时钟
	//RCC_HSICmd(ENABLE); 


  /* Wait till HSE is ready */
  GlobalErrorStatus = RCC_WaitForHSEStartUp();		//等待外部晶振时钟稳定

  if (GlobalErrorStatus == SUCCESS)				    //外部晶振时钟准备好
  {
    	/* Enable Prefetch Buffer */
    	FLASH_PrefetchBufferCmd(FLASH_PrefetchBuffer_Enable);
    	/* Flash 2 wait state */
    	FLASH_SetLatency(FLASH_Latency_2);
 
    	/* HCLK = SYSCLK */
    	RCC_HCLKConfig(RCC_SYSCLK_Div1); 
    	/* PCLK2 = HCLK */					   
    	RCC_PCLK2Config(RCC_HCLK_Div1); 	   //APB2 CLK : 72MHz
    	/* PCLK1 = HCLK/2 */
    	RCC_PCLK1Config(RCC_HCLK_Div2);		   //APB1 CLK ：36MHz
		
		/* PLLCLK = 9 * 8MHz= 72 MHz */ 
		//#define HSE_Value    ((u32)8000000) /* Value of the External oscillator in Hz  8MHZ*/	
		//stm32f10x_conf.h里面要在这里一并修改，切记！！！
    	#ifdef DEBOULE_FRE
		RCC_PLLConfig(RCC_PLLSource_HSE_Div1, RCC_PLLMul_8);  //ARM 外部晶振时钟为8，不分频作为PLL输入，12倍频作为PLL输出
		#else
		RCC_PLLConfig(RCC_PLLSource_HSE_Div1, RCC_PLLMul_9);  //ARM 外部晶振时钟为8，不分频作为PLL输入，9倍频作为PLL输出
		#endif
		  	
		RCC_PLLCmd(ENABLE);	    /* Enable PLL */ 
    	while (RCC_GetFlagStatus(RCC_FLAG_PLLRDY) == RESET){} //等待PLL准备好

    	RCC_SYSCLKConfig(RCC_SYSCLKSource_PLLCLK);	/* Select PLL as system clock source */

    	while(RCC_GetSYSCLKSource() != 0x08){}		/* Wait till PLL is used as system clock source */
	
		Clk_Enable_Sel2 = 0;
		if(UART1_EN_SEL)
			Clk_Enable_Sel2  |= RCC_APB2Periph_USART1;	
//		if(SPI1_EN_SEL)
//			Clk_Enable_Sel2  |= RCC_APB2Periph_SPI1;				
		Clk_Enable_Sel2 |= (RCC_APB2Periph_GPIOA | RCC_APB2Periph_GPIOB | RCC_APB2Periph_TIM1 |\
							RCC_APB2Periph_GPIOC |RCC_APB2Periph_GPIOD | RCC_APB2Periph_GPIOE | RCC_APB2Periph_AFIO);

		Clk_Enable_Sel1 = 0;
		if(UART3_EN_SEL)
			Clk_Enable_Sel1  |= RCC_APB1Periph_USART3;
		if(I2C_EN_SEL)
			Clk_Enable_Sel1  |= RCC_APB1Periph_I2C2;
		if(USB_EN_SEL)
			Clk_Enable_Sel1  |= RCC_APB1Periph_USB;	
		Clk_Enable_Sel1  |= RCC_APB1Periph_SPI2;

		RCC_APB2PeriphClockCmd(Clk_Enable_Sel2, ENABLE);		//打开APB2下的外围模块时钟
    RCC_APB1PeriphClockCmd(Clk_Enable_Sel1, ENABLE);		//打开APB1下的外围模块时钟
		RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO, ENABLE);			//复用功能时钟打开
		RCC_AHBPeriphClockCmd(RCC_AHBPeriph_DMA, ENABLE);
  }
  else		 //外部晶振没有工作....
  { 
  		/* If HSE fails to start-up, the application will have wrong clock configuration.
       	User can add here some code to deal with this error */    

    	/* Go to infinite loop */
    	while (1)
    	{
    	}
  }
}


/*******************************************************************************
* Function Name  : GPIO_CONFIG
* Description    : Configures GPIO
* Input          : None.
* Return         : None.
*******************************************************************************/
void GPIO_CONFIG(void)
{
	//Configure PC.00~07、PC.08~13 as Output push-pull 
	//PC0~7:FM175xx D0~D7    PC8~PC13:FM175xx A0~A5
  	GPIO_InitStructure.GPIO_Pin = /*D0 | D1 | D2 | D3 | D4 | */D5 | D6/* | D7*/; 
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  	GPIO_Init(GDATA, &GPIO_InitStructure);
	
		GPIO_InitStructure.GPIO_Pin = BUZZ;
  	GPIO_Init(GBUZZ, &GPIO_InitStructure);
	
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_1;
		GPIO_InitStructure.GPIO_Speed = GPIO_Speed_2MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
		GPIO_Init(GPIOB, &GPIO_InitStructure);
		
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_12|GPIO_Pin_13|GPIO_Pin_14|GPIO_Pin_15;
		//GPIO_InitStructure.GPIO_Speed = GPIO_Speed_2MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
		GPIO_Init(GPIOD, &GPIO_InitStructure);
	
	
	
	
		//mcu 2 fpga data bus
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_8|GPIO_Pin_9|GPIO_Pin_10|GPIO_Pin_11|GPIO_Pin_12|GPIO_Pin_13|GPIO_Pin_14|GPIO_Pin_15;
		//GPIO_InitStructure.GPIO_Speed = GPIO_Speed_2MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
		GPIO_Init(GPIOE, &GPIO_InitStructure);
		//mcu2fpga clk
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_7; 
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  	GPIO_Init(GPIOE, &GPIO_InitStructure);
		//mcu2fpga ready
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_0; 
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
		GPIO_Init(GPIOA, &GPIO_InitStructure);
		
		//mcu2fpga start
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_1; 
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  	GPIO_Init(GPIOA, &GPIO_InitStructure);
		
		
		
	//Configure PC.00~07、PC.08~13 as Output push-pull 
	// PC8~PC13:FM175xx A0~A5
  	GPIO_InitStructure.GPIO_Pin = A0 | A1/* | A2 | A3 | A4 | A5*/; 
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  	GPIO_Init(GADDR, &GPIO_InitStructure);

	GPIO_InitStructure.GPIO_Pin = NRST;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;
  	GPIO_Init(GNRST, &GPIO_InitStructure);
	
	// Configure PB.12(IRQ) as FLOATING INPUT	
  	GPIO_InitStructure.GPIO_Pin = IRQ;
  	//lts delete 20130827 GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU; //lts add 20130827
  	GPIO_Init(GIRQ, &GPIO_InitStructure);

	//Configure PD.00(IO_Sel0)、PD.01(IO_Sel0)、PD.02(IO_Sel0)as In_Floating
  	GPIO_InitStructure.GPIO_Pin = IOSEL0 | IOSEL1;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
  	GPIO_Init(GIOSEL, &GPIO_InitStructure);

	GPIO_InitStructure.GPIO_Pin = NFCMODE;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
  	GPIO_Init(GNFCMODE, &GPIO_InitStructure);
	
//	GPIO_InitStructure.GPIO_Pin = LPCD;
//  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
//  	GPIO_Init(GLPCD, &GPIO_InitStructure);

	//Configure PD.00(IO_Sel0)、PD.01(IO_Sel0)、PD.02(IO_Sel0)as Output push-pull 
  	GPIO_InitStructure.GPIO_Pin = FUNC0 | FUNC1;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_2MHz;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  	GPIO_Init(GFUNC, &GPIO_InitStructure);
	
//	//PA8作为系统时钟的输出
//	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_8;					
//	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
//	GPIO_InitStructure.GPIO_Speed= GPIO_Speed_50MHz;
//	GPIO_Init(GPIOA, &GPIO_InitStructure);

	/*LED灯控制：PD2*/
	GPIO_InitStructure.GPIO_Pin = LED0|LED1|LED2|LED3|LED4|LED5|LED6|LED7;					
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
	GPIO_InitStructure.GPIO_Speed= GPIO_Speed_2MHz;
	GPIO_Init(GLED, &GPIO_InitStructure);

	/*SPI2 Configuration*/
	// Configure SPI2 NSS (PB.12)、SCK(PB.13)、MISO(PB.14)、MOSI(PB.15) as alternate function push-pull  
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15; 
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz; 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP; 
	GPIO_Init(GPIOB, &GPIO_InitStructure);  

  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_12;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz; 
  	GPIO_Init(GPIOB, &GPIO_InitStructure);

	/*USART1 Configuration*/
	// Configure USART1 Tx (PA.09) as alternate function push-pull 
  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;	  //115200的最高波特率情况下，10MHz的IO速度；其实2MHz也就够了
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
  	GPIO_Init(GPIOA, &GPIO_InitStructure);
  	// Configure USART1 Rx (PA.10) as input floating
  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;	
	GPIO_Init(GPIOA, &GPIO_InitStructure);
	
	/*USART3 Configuration*/
	// Configure USART2 Tx (PA.02) as alternate function push-pull 
  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_10MHz;	  //115200的最高波特率情况下，10MHz的IO速度；其实2MHz也就够了
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
  	GPIO_Init(GPIOB, &GPIO_InitStructure);
  	// Configure USART2 Rx (PA.03) as input floating
  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU;
  	GPIO_Init(GPIOB, &GPIO_InitStructure);
	
	/*I2C2 Configuration*/
	// Configure I2C2 SCL (PD.3) as alternate function OD
  	GPIO_InitStructure.GPIO_Pin = I2C_SCL;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;	  //400k最高波特率情况下，50MHz的IO速度；
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_OD;
  	GPIO_Init(GI2C, &GPIO_InitStructure);
  	// Configure I2C2 SDA  (PD.4) as alternate function OD
  	GPIO_InitStructure.GPIO_Pin = I2C_SDA;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;	  //400k最高波特率情况下，50MHz的IO速度；
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_OD;
  	GPIO_Init(GI2C, &GPIO_InitStructure);

}

/*******************************************************************************
* Function Name  : EXTI_CONFIG
* Description    : Configures EXTI 外部中断配置
* Input          : None.
* Return         : None.
*******************************************************************************/
void EXTI_CONFIG(void)
{
	 EXTI_InitTypeDef  EXTI_InitStructure;
	 EXTI_DeInit();		  //EXTI初始化
	 GPIO_EXTILineConfig(GPIO_PortSourceGPIOC,GPIO_PinSource12);	//PC12设置到EXTI12上

		EXTI_DeInit();
		EXTI_InitStructure.EXTI_Line = EXTI_Line12; 
		EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt; 
		EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling;			//EXTI_Trigger_Falling; 
		EXTI_InitStructure.EXTI_LineCmd = ENABLE;  
		EXTI_Init(&EXTI_InitStructure);
	
	



//	EXTI_GenerateSWInterrupt(EXTI_Line2 | EXTI_Line5 | EXTI_Line6 | EXTI_Line7|\
//								   EXTI_Line8 | EXTI_Line9 | EXTI_Line12);



}
/*******************************************************************************
* Function Name  : NVIC_CONFIG
* Description    : Configures NVIC 
* Input          : None.
* Return         : None.
*******************************************************************************/
void NVIC_CONFIG(void)
{
		NVIC_DeInit();		//中断优先级初始化

	 NVIC_SetVectorTable(NVIC_VectTab_FLASH, 0x0); 
	 NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);	//抢占优先级：0/1  从优先级0-7

	 /* Enable the USART1 Interrupt */
	 NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQChannel;
	 NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
	 NVIC_InitStructure.NVIC_IRQChannelSubPriority = 1;
	 NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	 NVIC_Init(&NVIC_InitStructure);

	
	if(UART3_EN_SEL && UART_AUTO_TRANSMIT)	
	{
		/* Enable the USART2 Interrupt */
	 	NVIC_InitStructure.NVIC_IRQChannel = USART3_IRQChannel;
	 	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
	 	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;
	 	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	 	NVIC_Init(&NVIC_InitStructure);
	}

	 NVIC_InitStructure.NVIC_IRQChannel = EXTI15_10_IRQChannel; 	//中断通道　　
	 NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;		//强占优先级　　
	 NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0; 			//次优先级　　
	 NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 				//通道中断使能　　
	 NVIC_Init(&NVIC_InitStructure); 								//初始化中断


			//Timer1 中断使能
	NVIC_InitStructure.NVIC_IRQChannel = TIM1_UP_IRQChannel;	//更新事件
  	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;	//抢占优先级
  	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 3;			//响应优先级
  	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;				//允许中断
  	NVIC_Init(&NVIC_InitStructure);

	NVIC_InitStructure.NVIC_IRQChannel = TIM2_IRQChannel;//指定中断源
	//NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;// 指定响应优先级别1
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;	//抢占优先级
  	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 4;			//响应优先级
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
		NVIC_InitStructure.NVIC_IRQChannel = TIM3_IRQChannel;//指定中断源
	//NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;// 指定响应优先级别1
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;	//抢占优先级
  	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 5;			//响应优先级
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
	NVIC_SystemHandlerPriorityConfig(SystemHandler_SysTick, 1, 5);
	
	
	//	 NVIC_PriorityGroupConfig(NVIC_PriorityGroup_3);	
		NVIC_InitStructure.NVIC_IRQChannel = DMAChannel4_IRQChannel;
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
    NVIC_Init(&NVIC_InitStructure);
}
/*******************************************************************************
* Function Name  : USART1_CONFIG
* Description    : Configures USART1     UART1配置
* Input          : None.
* Return         : None.
*******************************************************************************/
void USART1_CONFIG(void)
{
  /* USART1 configuration ----------------------------------------------------*/
  /* USART1 configured as follow:
        - BaudRate = 9600 baud  
        - Word Length = 8 Bits
        - Two Stop Bit
        - Odd parity
        - Hardware flow control disabled (RTS and CTS signals)
        - Receive and transmit enabled
        - USART Clock disabled
        - USART CPOL: Clock is active low
        - USART CPHA: Data is captured on the second edge 
        - USART LastBit: The clock pulse of the last data bit is not output to 
                         the SCLK pin
  */
  USART_DeInit(USART1);	   //复位
 
  /* Configure the USART1 */
  USART_InitStructure.USART_BaudRate = UART1_BaudRate;
  USART_InitStructure.USART_WordLength = USART_WordLength_8b;
  USART_InitStructure.USART_StopBits = USART_StopBits_1;
  USART_InitStructure.USART_Parity = USART_Parity_No;
  USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
  USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;
  USART_Init(USART1, &USART_InitStructure);			  
 
  USART_Cmd(USART1, ENABLE);
	
// USART_ITConfig(USART1, USART_IT_TC, ENABLE);     //发送完成中断
  USART_ITConfig(USART1, USART_IT_RXNE, ENABLE);   //接收完成中断
}
/*******************************************************************************
* Function Name  : USART2_CONFIG
* Description    : Configures USART2     UART2配置
* Input          : None.
* Return         : None.
*******************************************************************************/
void USART3_CONFIG(void)
{
  USART_DeInit(USART3);	   //复位
 
  /* Configure the USART2 */
  USART_InitStructure.USART_BaudRate = UART3_BaudRate;
  USART_InitStructure.USART_WordLength = USART_WordLength_8b;
  USART_InitStructure.USART_StopBits = USART_StopBits_2;
  USART_InitStructure.USART_Parity = USART_Parity_No;
  USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
  USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;
  USART_Init(USART3, &USART_InitStructure);			  
  
  USART_Cmd(USART3, ENABLE);
    
  USART_ITConfig(USART3, USART_IT_TC, ENABLE);     //发送完成中断
  //if(UART_AUTO_TRANSMIT) 						   //发送采用中断模式，接收采用查询模式，
  USART_ITConfig(USART3, USART_IT_RXNE, ENABLE);   //只有自动收发模式下，才启用中断模式接收
}

void Timer_Init_TIM1(void)
{
	TIM1_TimeBaseInitTypeDef TIM1_TimeBaseStructure;

	TIM1_DeInit();

	TIM1_TimeBaseStructure.TIM1_Period = 10000;				//计数到10000产生更新事件  自动重装载数值
	TIM1_TimeBaseStructure.TIM1_Prescaler = 7199;			//APB2进行11200分频0.1ms 计数器的时钟频率等于f/(PSC[15:0]+1)。  
	TIM1_TimeBaseStructure.TIM1_ClockDivision = TIM1_CKD_DIV1  ;
	TIM1_TimeBaseStructure.TIM1_CounterMode = TIM1_CounterMode_Down;
	TIM1_TimeBaseStructure.TIM1_RepetitionCounter = 0;		//重复计价次数
	TIM1_TimeBaseInit(&TIM1_TimeBaseStructure);
	
	TIM1_ClearFlag(TIM1_FLAG_Update);							//清除中断，以免一启用就中断
	TIM1_SetCounter(10000);										//设置计数器初值	1s
	Enable_TIM1_IT;
	TIM1_Cmd(ENABLE);											//TIM1使能工作  使能或者失能TIM1外设 
//	mDelay(1);
}

void Timer_Init_TIM2(void)
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_DeInit(TIM2);
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM2, ENABLE);

	TIM_TimeBaseStructure.TIM_Period=5;                   //ARR的值 5us
	TIM_TimeBaseStructure.TIM_Prescaler=72-1;			  //1us @112Mhz
	TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up; //向上计数模式
	TIM_TimeBaseInit(TIM2, &TIM_TimeBaseStructure);
	TIM_ITConfig(TIM2,TIM_IT_Update,ENABLE);
	//TIM_Cmd(TIM2, ENABLE); //开启时钟
}

void Timer_Init_TIM3(void)
{
	TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
	TIM_DeInit(TIM3);
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3, ENABLE);

	TIM_TimeBaseStructure.TIM_Period=359;                   //ARR的值 5us
	TIM_TimeBaseStructure.TIM_Prescaler=0;			  //
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode=TIM_CounterMode_Up; //向上计数模式
	TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure);
	TIM_ClearFlag(TIM2, TIM_FLAG_Update);
	TIM_ARRPreloadConfig(TIM3, DISABLE);
	TIM_ITConfig(TIM3,TIM_IT_Update,ENABLE);
	TIM_Cmd(TIM3, ENABLE); //开启时钟
}

void SPI_CONFIG(void)
{
  SPI_InitTypeDef SPI_InitStructure;
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
  SPI_Init(SPI2, &SPI_InitStructure);

  /* Enable SPI1 */
  SPI_Cmd(SPI2, ENABLE);
}

void SysTick_Init(void)
{
   u32 tickReload;
   SysTick_CLKSourceConfig(SysTick_CLKSource_HCLK_Div8);   // HCLK/8作为Systick时钟，12MHz
   tickReload = 11999*500;// 500ms lts 11999*1000;			//1s
   SysTick_SetReload(tickReload);  //重装载寄存器1S
   SysTick_ITConfig(ENABLE); 	   //Systick中断使能位
   SysTick_CounterCmd(SysTick_Counter_Clear);//当前值寄存器清除 
   SysTick_CounterCmd(SysTick_Counter_Enable); //Systick 计数使能  
}

void DMA_Configuration(void)
{
    DMA_InitTypeDef DMA_InitStructure;
	//DMA_InitTypeDef
    
    DMA_DeInit(DMA_Channel4);
    DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)(&USART1->DR);//(USART1_BASE + 0x04);
    DMA_InitStructure.DMA_MemoryBaseAddr = (u32)UART1_DATA_BUF;
    DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralDST;
    DMA_InitStructure.DMA_BufferSize =1024;
    DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;
    DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;
    DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;
    DMA_InitStructure.DMA_MemoryDataSize = DMA_PeripheralDataSize_Byte;
    DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;  //DMA_Mode_Circular;
    DMA_InitStructure.DMA_Priority = DMA_Priority_High;
    DMA_InitStructure.DMA_M2M = DMA_M2M_Disable;
    DMA_Init(DMA_Channel4, &DMA_InitStructure);

		USART_DMACmd(USART1, USART_DMAReq_Tx, ENABLE);           //DMA
	
	  DMA_ITConfig(DMA_Channel4,DMA_IT_TC,ENABLE);

	  DMA_Cmd(DMA_Channel4, DISABLE);
}


