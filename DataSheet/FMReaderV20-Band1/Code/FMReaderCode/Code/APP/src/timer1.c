#include "stm32f10x_tim.h"
#include "stm32f10x.h"
#include "timer1.h"

extern void LEDShow(uint8_t Index, BitAction BitVal);
extern bool bTimeOut;
extern uint8_t volatile bIsTimeout;
/*******************************************************************************
* Function Name  : TIM1Config.
* Description    : Config the TIM1 of MCU used for count time with interrupt.
* Input          : Counter  : How many unit (0.1ms=100us) should be counted. 
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1Config(uint32_t count)
{
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;

	uint16_t period;	
  /* ---------------------------------------------------------------
    TIM4 Configuration: Output Compare Timing Mode:
    TIM4 counter clock at 1 MHz
  --------------------------------------------------------------- */  

  /* Time base configuration */
  //if(count<65536)
  TIM_DeInit(TIM1);    

  period = count%0x10000;
  TIM_TimeBaseStructure.TIM_Period = period;	
  TIM_TimeBaseStructure.TIM_Prescaler = 7199;
  TIM_PrescalerConfig(TIM1, 7199, TIM_PSCReloadMode_Immediate); 
  

  TIM_TimeBaseStructure.TIM_ClockDivision = 1;
  TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;

  TIM_TimeBaseInit(TIM1, &TIM_TimeBaseStructure);

  TIM_ClearFlag(TIM1,TIM_FLAG_Update);
  //TIM_ITConfig(TIM4, TIM_IT_CC1, ENABLE);
  //TIM_Cmd(TIM4, ENABLE);
}

/*******************************************************************************
* Function Name  : TIM3Start.
* Description    : Start the TIM3 of MCU used for delay and width timer.
* Input          : None.
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1Start(void)
{
  /* TIM IT enable */
  TIM_ITConfig(TIM1, TIM_IT_CC1, ENABLE);

  /* TIM2 enable counter */
  TIM_Cmd(TIM1, ENABLE);
}
/*******************************************************************************
* Function Name  : TIM3Start.
* Description    : Start the TIM3 of MCU used for delay and width timer.
* Input          : None.
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1Stop(void)
{
  TIM_Cmd(TIM1, DISABLE);
  TIM_ITConfig(TIM1, TIM_IT_CC1, DISABLE);  
}


/*******************************************************************************
* Function Name  : TIM3InterruptHandler.
* Description    : TIM3 interrupt handler.
* Input          : None 
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1InterruptHandler(void)
{
  if(TIM_GetITStatus(TIM1, TIM_IT_CC1) != RESET)
  {
    TIM_ClearITPendingBit(TIM1, TIM_IT_CC1);
    
	TIM_ITConfig(TIM1, TIM_IT_CC1, DISABLE);
	TIM_Cmd(TIM1, DISABLE);
	
	bIsTimeout = 1;
	
  }
}
