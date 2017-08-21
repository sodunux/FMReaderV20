/******************** (C) COPYRIGHT 2015 fmsh ********************
* File Name          : AD_DATA_Process.c
* Author             : Sean Chen
* Version            : V1.0
* Date               : 30/06/2015
* Description        : This file provides all the ADC data processing functions.
********************************************************************************/

#include "AD_DATA_Process.h"
#include "stm32f10x_lib.h"

vu8 AD_ON = 0;                     //0: do not open adc , 1: open adc

/*******************************************************************************
* Function Name  : AD_Init
* Description    : 
*                  
* Input          : 
*                  
*                    
*                    
* Output         : None
* Return         : None
*******************************************************************************/
void AD_Init(void)
{
		GPIO_ResetBits(GPIOA, GPIO_Pin_1);        //set start line low
		GPIO_ResetBits(GPIOE,GPIO_Pin_7);         //set clk line low
		
	
	
}


/*******************************************************************************
* Function Name  : AD_START
* Description    : 
*                  
* Input          : 
*                  
*                    
*                    
* Output         : None
* Return         : None
*******************************************************************************/
void AD_START(u16 time)
{
		TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
		GPIO_SetBits(GPIOA, GPIO_Pin_1);        //set start line high
		TIM_TimeBaseStructure.TIM_Period=time-1; 
		TIM_TimeBaseInit(TIM2, &TIM_TimeBaseStructure);	
		TIM_Cmd(TIM2, ENABLE);
	
	
}

/*******************************************************************************
* Function Name  : AD_READY_CHECK
* Description    : 
*                  
* Input          : 
*                  
*                    
*                    
* Output         : None
* Return         : None
*******************************************************************************/
bool AD_READY_CHECK(void)
{
	
	
	
	
	
	
	
	 return 0;
}







