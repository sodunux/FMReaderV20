/******************** (C) COPYRIGHT 2007 STMicroelectronics ********************
* File Name          : stm32f10x_it.c
* Author             : MCD Application Team
* Version            : V1.0
* Date               : 10/08/2007
* Description        : Main Interrupt Service Routines.
*                      This file can be used to describe all the exceptions 
*                      subroutines that may occur within user application.
*                      When an interrupt happens, the software will branch 
*                      automatically to the corresponding routine.
*                      The following routines are all empty, user can write code 
*                      for exceptions handlers and peripherals IRQ interrupts.
********************************************************************************
* THE PRESENT SOFTWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH SOFTWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Includes ------------------------------------------------------------------*/
#include "stm32f10x_it.h"
#include "define.h"
#include "Global.h"
#include "Uart.h"
#include "Test.h"
#include "FMReg.h"
#include "RegCtrl.h"
/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : NMIException
* Description    : This function handles NMI exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void NMIException(void)
{
}

/*******************************************************************************
* Function Name  : HardFaultException
* Description    : This function handles Hard Fault exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void HardFaultException(void)
{
  /* Go to infinite loop when Hard Fault exception occurs */
  while (1)
  {
  }
}

/*******************************************************************************
* Function Name  : MemManageException
* Description    : This function handles Memory Manage exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void MemManageException(void)
{
  /* Go to infinite loop when Memory Manage exception occurs */
  while (1)
  {
  }
}

/*******************************************************************************
* Function Name  : BusFaultException
* Description    : This function handles Bus Fault exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void BusFaultException(void)
{
  /* Go to infinite loop when Bus Fault exception occurs */
  while (1)
  {
  }
}

/*******************************************************************************
* Function Name  : UsageFaultException
* Description    : This function handles Usage Fault exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void UsageFaultException(void)
{
  /* Go to infinite loop when Usage Fault exception occurs */
  while (1)
  {
  }
}

/*******************************************************************************
* Function Name  : DebugMonitor
* Description    : This function handles Debug Monitor exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DebugMonitor(void)
{
}

/*******************************************************************************
* Function Name  : SVCHandler
* Description    : This function handles SVCall exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SVCHandler(void)
{
}

/*******************************************************************************
* Function Name  : PendSVC
* Description    : This function handles PendSVC exception.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void PendSVC(void)
{
}

/*******************************************************************************
* Function Name  : SysTickHandler
* Description    : This function handles SysTick Handler.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SysTick_Handler(void)
{
	if(SysTick_GetFlagStatus(SysTick_FLAG_COUNT))
	//if(SysTick->CALIB&0x10000)
	{
		SysTick_CounterCmd(SysTick_Counter_Clear);//当前值寄存器清除 
//		if(GLED->ODR & LED_ARM_WORKING)
//		{
//			LED_ARM_WORKING_ON;	   //Arm Working
//		}
//		else
//		{
//			LED_ARM_WORKING_OFF;
//		}

		Sys_Timer_Count++;
	}
}

/*******************************************************************************
* Function Name  : WWDG_IRQHandler
* Description    : This function handles WWDG interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void WWDG_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : PVD_IRQHandler
* Description    : This function handles PVD interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void PVD_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : TAMPER_IRQHandler
* Description    : This function handles Tamper interrupt request. 
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TAMPER_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : RTC_IRQHandler
* Description    : This function handles RTC global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void RTC_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : FLASH_IRQHandler
* Description    : This function handles Flash interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void FLASH_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : RCC_IRQHandler
* Description    : This function handles RCC interrupt request. 
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void RCC_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : EXTI0_IRQHandler
* Description    : This function handles External interrupt Line 0 request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI0_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : EXTI1_IRQHandler
* Description    : This function handles External interrupt Line 1 request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI1_IRQHandler(void)
{
//	EXTI_ClearITPendingBit(EXTI_Line1);
//	if(Polling_A_OC == true)		//如果Polling处于打开状态
//	{
//		Event_Flag = TRUE;
//		Event_Polling_A  = TRUE;     //polling 有卡响应
//	}
}

/*******************************************************************************
* Function Name  : EXTI2_IRQHandler
* Description    : This function handles External interrupt Line 2 request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI2_IRQHandler(void)
{
// 	if(GPIO_ReadOutputDataBit(GPIOC,LED_BUTTON_PIN) == Bit_SET)		 //LED闪烁
//		LED_BUTTON_ON;
//	else 
//		LED_BUTTON_OFF;
//	if(EXTI_GetITStatus(EXTI_Line2)!= RESET)		  //SWIO1  //CARD_A
//	{
//    	EXTI_ClearITPendingBit(EXTI_Line2);
//		Event_Flag = TRUE;
//		Event_CardA_Func = TRUE;
//    }
}

/*******************************************************************************
* Function Name  : EXTI3_IRQHandler
* Description    : This function handles External interrupt Line 3 request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI3_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : EXTI4_IRQHandler
* Description    : This function handles External interrupt Line 4 request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI4_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel1_IRQHandler
* Description    : This function handles DMA Stream 1 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel1_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel2_IRQHandler
* Description    : This function handles DMA Stream 2 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel2_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel3_IRQHandler
* Description    : This function handles DMA Stream 3 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel3_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel4_IRQHandler
* Description    : This function handles DMA Stream 4 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel4_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel5_IRQHandler
* Description    : This function handles DMA Stream 5 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel5_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel6_IRQHandler
* Description    : This function handles DMA Stream 6 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel6_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : DMAChannel7_IRQHandler
* Description    : This function handles DMA Stream 7 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void DMAChannel7_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : ADC_IRQHandler
* Description    : This function handles ADC global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void ADC_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : USB_HP_CAN_TX_IRQHandler
* Description    : This function handles USB High Priority or CAN TX interrupts 
*                  requests.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USB_HP_CAN_TX_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : USB_LP_CAN_RX0_IRQHandler
* Description    : This function handles USB Low Priority or CAN RX0 interrupts 
*                  requests.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USB_LP_CAN_RX0_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : CAN_RX1_IRQHandler
* Description    : This function handles CAN RX1 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void CAN_RX1_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : CAN_SCE_IRQHandler
* Description    : This function handles CAN SCE interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void CAN_SCE_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : EXTI9_5_IRQHandler
* Description    : This function handles External lines 9 to 5 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI9_5_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : TIM1_BRK_IRQHandler
* Description    : This function handles TIM1 Break interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1_BRK_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : TIM1_UP_IRQHandler
* Description    : This function handles TIM1 overflow and update interrupt 
*                  request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1_UP_IRQHandler(void)
{
	TIM1_ClearFlag(TIM1_FLAG_Update);
	
//	if(GLED->ODR & LED2)
//	{
//		LED3_ON;
//		LED2_OFF;
//	}
//	else
//	{
//		LED2_ON;
//		LED3_OFF;
//	}
	Event_Flag = TRUE;
	Event_ARM_TIMER_IRQ = TRUE;

	TIM1_SetCounter(10000);
	TIM1_Cmd(ENABLE);	
}

/*******************************************************************************
* Function Name  : TIM1_TRG_COM_IRQHandler
* Description    : This function handles TIM1 Trigger and commutation interrupts 
*                  requests.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1_TRG_COM_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : TIM1_CC_IRQHandler
* Description    : This function handles TIM1 capture compare interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM1_CC_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : TIM2_IRQHandler
* Description    : This function handles TIM2 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM2_IRQHandler(void)
{
	if(TIM_GetITStatus(TIM2 , TIM_IT_Update) == SET)
	{
 		TIM_ClearITPendingBit(TIM2 , TIM_FLAG_Update);
		//if(GOSC10K->ODR & OSC10KOUT)
		//   GPIO_ResetBits(GOSC10K, OSC10KOUT);
		//else
		//   GPIO_SetBits(GOSC10K, OSC10KOUT);
		FM175XX_Time_Out = true;
	}
}

/*******************************************************************************
* Function Name  : TIM3_IRQHandler
* Description    : This function handles TIM3 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : TIM4_IRQHandler
* Description    : This function handles TIM4 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TIM4_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : I2C1_EV_IRQHandler
* Description    : This function handles I2C1 Event interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void I2C1_EV_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : I2C1_ER_IRQHandler
* Description    : This function handles I2C1 Error interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void I2C1_ER_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : I2C2_EV_IRQHandler
* Description    : This function handles I2C2 Event interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void I2C2_EV_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : I2C2_ER_IRQHandler
* Description    : This function handles I2C2 Error interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void I2C2_ER_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : SPI1_IRQHandler
* Description    : This function handles SPI1 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SPI1_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : SPI2_IRQHandler
* Description    : This function handles SPI2 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SPI2_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : USART1_IRQHandler
* Description    : This function handles USART1 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USART1_IRQHandler(void)
{
	  u8 temp;

	  if(USART_GetITStatus(USART1, USART_IT_RXNE) != RESET)
	  {
		temp=(u8)USART1->DR;							//读出数据	

	    USART_ClearITPendingBit(USART1, USART_IT_RXNE);	// Clear the USART1 Receive interrupt	
		if(UART_AUTO_TRANSMIT)
		{
			USART2->DR = (u16)temp;
			return;
		}						
		switch(UART_Com1_Para.RecvStatus)
		{
			case 0:
//				if(temp==0x55)							//接到握手信号
//				{
//					mDelay(100);
//					USART1->DR = (u16)temp;
//					if(GLED->ODR & LED_SYS_STATUS)
//					{
//						LED_SYS_STATUS_ON;	   //Arm Working
//					}
//					else
//					{
//						LED_SYS_STATUS_OFF;
//					}	
//				}				
				/*else */if(temp==STX)			   			//接到帧头		
				{	
					UART_Com1_Para.RecvStatus++;						//状态机加1 
				 	UART_Com1_Para.Recv_Index = 0;
				}
				else
					UART_Com1_Para.RecvStatus = 0;	
				break;
			case 1:							//读取CMD_Numb
				UART1_DATA_BUF[0] = temp;
				UART_Com1_Para.RecvStatus++;				
				break;
			case 2:							//读取CMD
				UART1_DATA_BUF[1] = temp;
				UART_Com1_Para.RecvStatus++;				
				break;					
			case 3:						   	//读取Lengh						
				UART1_DATA_BUF[2] = temp;
				UART_Com1_Para.Recv_Len = temp;
				UART_Com1_Para.RecvStatus++;
				break;
			case 4:							//读取Data & BCC
				if(UART_Com1_Para.Recv_Index<UART_Com1_Para.Recv_Len+1)
				{					
					UART1_DATA_BUF[UART_Com1_Para.Recv_Index+3] = temp;
					UART_Com1_Para.Recv_Index++;
				}
				if(UART_Com1_Para.Recv_Index == UART_Com1_Para.Recv_Len+1)	 //数据接收完毕;
				{
					UART_Com1_Para.RecvStatus++;          
				}										
				break;
			case 5:						//读取ETX
				UART_Com1_Para.RecvStatus++;
				if(temp == ETX)
				{
					UART_Com1_Para.RecvStatus++;
					UART_Com1_Para.RecvOKFLAG = TRUE;
					Event_Flag = TRUE;							   //通知主程序，有事件要处理
				}
				else				    //帧格式错误
				{
					UART_Com1_Para.RecvStatus = 0;
				}
				break;
			case 6:	  	//帧处理中，不再处理
				break;
			default:    //错误状态，返回IDLE
				UART_Com1_Para.RecvStatus=0;
				break;
	 	 }
	  }

	  if(USART_GetITStatus(USART1, USART_IT_TC) != RESET)
	  {   
		    USART_ClearITPendingBit(USART1, USART_IT_TC);
		                 
			if(UART_Com1_Para.Send_Index < UART_Com1_Para.Send_Len)			//发送缓冲区是否空
			{ 
					USART1->DR = (u16)UART1_DATA_BUF[UART_Com1_Para.Send_Index++];	
			}
			else
				UART_Com1_Para.Send_Index = UART_Com1_Para.Send_Len = 0;		
	  }
}


/*******************************************************************************
* Function Name  : USART2_IRQHandler
* Description    : This function handles USART2 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USART2_IRQHandler(void)
{
	  u8 temp;

	  if(USART_GetITStatus(USART2, USART_IT_RXNE) != RESET)
	  {
	  	if(UART_AUTO_TRANSMIT)
	  	{
			temp=(u8)USART2->DR;							//读出数据	
			USART1->DR = (u16)temp;
	    	USART_ClearITPendingBit(USART2, USART_IT_RXNE);	// Clear the USART1 Receive interrupt
			return;
		}
	  }

	  if(USART_GetITStatus(USART2, USART_IT_TC) != RESET)
	  {   
		    USART_ClearITPendingBit(USART2, USART_IT_TC);
		                 
			if(UART_Com3_Para.Send_Index < UART_Com3_Para.Send_Len)			//发送缓冲区是否空
			{ 
					USART2->DR = (u16)UART2_DATA_BUF[UART_Com3_Para.Send_Index++];	
			}
			else
				UART_Com3_Para.Send_Index = UART_Com3_Para.Send_Len = 0;		
	  }
}

/*******************************************************************************
* Function Name  : USART3_IRQHandler
* Description    : This function handles USART3 global interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USART3_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : EXTI15_10_IRQHandler
* Description    : This function handles External lines 15 to 10 interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void EXTI15_10_IRQHandler(void)
{
	uchar temp;
	if(EXTI_GetITStatus(EXTI_Line12)!= RESET)		  //R/C
	{
		
		EXTI_ClearITPendingBit(EXTI_Line12);

		Event_Flag = TRUE;
		Event_FM175XX_IRQ = TRUE;
		GetReg(FIFOLEVEL,&temp);
		//if(FM175XX_CurFunc == FM175XX_FUNC_CARD)
		//{
		//	ModifyReg(COMMIEN,PHCS_BFL_JBIT_RXI | PHCS_BFL_JBIT_HIALERTI,0);
		//}
		//GetReg(COMMIRQ,&temp);
		//SetReg(COMMIRQ,temp);			//清Timer中断
		//只要有中断唤醒，因为之后有寄存器操作，可能在LPCD模式下，先确保NRSTPD为高电平
		//NRSTPD_CTRL(1);			  //NRST
		//mDelay(2);				  //延时2ms，让晶振稳定
		if(LpcdFunc == 1)		 		//LTS 20140319 只由涉及到LPCD模式才启动中断
		//原来只要产生中断就会有NRSTPD为0到1的动作，目前改法还是不严密	
		//这个对原来程序可能有影响，需要修改
		{
			NRSTPD_CTRL(1);			  //NRST
		}
		mDelay(2);				  //延时2ms，让晶振稳定
    }
}

/*******************************************************************************
* Function Name  : RTCAlarm_IRQHandler
* Description    : This function handles RTC Alarm interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void RTCAlarm_IRQHandler(void)
{
}

/*******************************************************************************
* Function Name  : USBWakeUp_IRQHandler
* Description    : This function handles USB WakeUp interrupt request.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USBWakeUp_IRQHandler(void)
{
}

/******************* (C) COPYRIGHT 2007 STMicroelectronics *****END OF FILE****/
