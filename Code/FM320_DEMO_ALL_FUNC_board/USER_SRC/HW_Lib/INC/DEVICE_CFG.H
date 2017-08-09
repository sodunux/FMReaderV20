#ifndef _DEVICE_CFG_H
#define _DEVICE_CFG_H


extern void USART_CHG_BAUD(USART_TypeDef* USARTx, u32 baud);
extern void RCC_Config(void);
extern void GPIO_CONFIG(void);
extern void EXTI_CONFIG(void);
extern void USART1_CONFIG(void);
extern void USART3_CONFIG(void);
extern void Timer_Init_TIM1(void);
extern void Timer_Init_TIM2(void);
void Timer_Init_TIM3(void);
extern void SPI_CONFIG(void);
extern void SysTick_Init(void);
void DMA_Configuration(void);
void NVIC_CONFIG(void);



#endif