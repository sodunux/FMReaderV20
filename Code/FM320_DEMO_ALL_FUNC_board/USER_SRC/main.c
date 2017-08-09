/* /////////////////////////////////////////////////////////////////////////////////////////////////
//                     Copyright (c) FMSH
//                      (C)FMSH 2014
//         All rights are reserved. Reproduction in whole or in part is
//        prohibited without the written consent of the copyright owner.
//    Philips reserves the right to make changes without notice at any time.
//   Philips makes no warranty, expressed, implied or statutory, including but
//   not limited to any implied warranty of merchantability or fitness for any
//  particular purpose, or that the use will not infringe any third party patent,
//   copyright or trademark. Philips must not be liable for any loss or damage
//                            arising from its use.
///////////////////////////////////////////////////////////////////////////////////////////////// */

#include "global.h"
#include "DEVICE_CFG.H"
#include "UART.h"
#include "RegCtrl.h"
#include "I2C.h"
#include "BasicAPI.h"
#include "FMReg.h"
#include "lpcd_api_private.h"
#include "lpcd_api_public.h"
#include "lpcd_regctrl_private.h"
#include "lpcd_reg_private.h"
#include "lpcd_reg_public.h"
#include "ReaderAPI.h"
#include "CardAPI.h"
#include "oled.h"
#include "demo.h"

int main(void)
{	  
	uchar lpcd_irq;
	uchar CalibraFlag;
	uchar Mode;
	u32 ReaderA_Timer_Count;//Sys_Timer_Count
	u32 ReaderB_Timer_Count;
	u32 ReaderF_Timer_Count;
	u32 Card_Timer_Count;
	vu8  keycmd_tmp = 0;
	vu8  typeArq = 0;
	u8 RegData1;
	
	//u8 z;
	
	RCC_Config();		//时钟配置
	GPIO_CONFIG();		//GPIO配置
//	GPIO_SetBits(GPIOA, GPIO_Pin_1);
	GPIO_SetBits(GLED,LED1|LED2|LED3|LED4|LED5|LED6|LED7);
	//Rst for FPGA
	GPIO_ResetBits(GPIOB,GPIO_Pin_1);                    //FPGA_Rst = low
	mDelay(100);
	GPIO_SetBits(GPIOB,GPIO_Pin_1);                    //FPGA_Rst = high
	
	FM175xx_INTERFACE = FM175xx_Intf_Detect();
	//I2C_MODE = I2C_HSMode;
	FM175xx_SetInterface(FM175xx_INTERFACE);
	
	USART1_CONFIG();	//UART1配置
	if(UART3_EN_SEL)	
	{
		USART3_CONFIG();	//UART2配置
	}
	if(SPI2_EN_SEL)	
	{
		SPI_CONFIG();	//UART2配置
	}
	DMA_Configuration();
	EXTI_CONFIG();  	//外部中断配置
	Timer_Init_TIM1();
	Timer_Init_TIM2();  //1us 定时
	Timer_Init_TIM3();
	SysTick_Init();
	NVIC_CONFIG();
	UART_Com_Para_Init(&UART_Com1_Para);    //初始化Uart2的通讯参数
	if(UART3_EN_SEL)
	{
		UART_Com_Para_Init(&UART_Com3_Para);    //初始化Uart2的通讯参数
	}

	FM175xx_Initial();
	
  LED_ARM_WORKING_OFF;
	BUZZ_OFF;
	
	OLED_Init();
	ShowLogo();
	OLED_ShowString(56,STING_SIZE*0, "NB1411 Test");  	//ROW 3
	if(1 == Mode)
	{
		//LTS 删除，为了能够显示更多的信息量
		//OLED_ShowString(64,STING_SIZE*1,"AUTO");  //ROW 4
	}
	else
	{
		//OLED_ShowString(64,STING_SIZE*1,"CONTROL");  //ROW 4
	}
	
	OLED_Refresh_Gram();	
	
	Flash_Read_UserInfo(User_Info);
	Flash_Read_Card_UID(Card_UID_Buf);
	
	LpcdEn   = User_Info[4];
	LpcdFunc = User_Info[5];
	
	if(LpcdEn == 1)												//支持LPCD功能
	{
	
			CalibraFlag = LpcdInitialize();
			if (CalibraFlag == TRUE)						//如果调教成功，则亮灯
			{
				//LTS为了显示更多的信息
				//OLED_ShowString(64,STING_SIZE*2,"LpcdCalPass");  //ROW 2
				OLED_ShowString(56,STING_SIZE*1,"LpcdCalPass");  //ROW 2
				
				OLED_Refresh_Gram();
				NRSTPD_CTRL(0);										//进入LPCD模式
				Event_Flag = FALSE;								//
				Event_FM175xx_IRQ = FALSE;
			} 
			else
			{
				OLED_ShowString(56,STING_SIZE*1,"LpcdCalFail");  //ROW 2
				OLED_Refresh_Gram();
			}
	} 
	
//	if(FM175xx_INTERFACE == 1)		//Uart
//	{
//		SetReg(SERIALSPEED,0x3A);
//		USART_CHG_BAUD(USART3, 460800);	
//	}
	
	//BUZZ_OFF;
	ReaderA_Timer_Count = ReaderB_Timer_Count = ReaderF_Timer_Count = Card_Timer_Count = Sys_Timer_Count;
	/*****************TEST*******************/

//	for(z=0;z<10;z++)
//	{
//		UART1_DATA_BUF[z]=z;
	
//	}
//	DMA_Cmd(DMA_Channel4, ENABLE);
	
	
	/*****************END TEST********************/
	while(1)
	{	
		//for NB1411 test
		keycmd_tmp = Keyscan();
		if(keycmd_tmp != 0)
		{
			pbcmd = (~keycmd_tmp)&(0X0F);
			keycmd_tmp = 0;
		}
		switch(pbcmd)
		{
			case 0x01:
						{pbcmd=0; 
							AUX1_SET(16);
						GPIO_ResetBits(GLED,LED4);
						//GPIO_SetBits(GLED,LED5|LED6|LED7); 
						
						break;}
			case 0x02:
						{pbcmd=0; 
							AUX2_SET(17);
						GPIO_ResetBits(GLED,LED5);
						//GPIO_SetBits(GLED,LED4|LED6|LED7); 
						break;}
			case 0x04:
						{pbcmd=0; 
					//	typeArq = 1;
						//u8 ret;
							
							//GetReg(0x37,&RegData);
						
							//ret = GetReg(RegAddrLpcd,&RegDataLpcd);
							/*************************************************Enter HPD
							GetReg_Ext(0X03,&RegData1);
							RegData1 = RegData1|0x20;
							
							SetReg_Ext(0x03,RegData1);
							GetReg_Ext(0X03,&RegData1);	
							if(RegData1&0x20)
							{
								GPIO_ResetBits(GLED,LED1);
								GPIO_SetBits(GLED,LED2|LED3|LED4|LED5|LED6|LED7);
								GPIO_ResetBits(GNRST,NRST);
							}
							*/
							//ReaderA_Request(0x52);
							//Quit_AUXSET();
						//GPIO_ResetBits(GLED,LED6);
						//GPIO_SetBits(GLED,LED5|LED4|LED7); 
						break;}
			case 0x08:
						{pbcmd=0; 
						typeArq = 0;	
							
						GPIO_ResetBits(GLED,LED7);
						GPIO_SetBits(GLED,LED5|LED6|LED4); 
						break;}
			default:{pbcmd=0; 
						//GPIO_ResetBits(GLED,LED4);
						//GPIO_SetBits(GLED,LED4|LED5|LED6|LED7); 
						break;}
		}
		if(typeArq == 1)
		{
		//	ReaderA_Request(0x26);
			typeArq = 0;
		}
	///******************************************************	
		if(Event_Flag)
		{
			Event_Flag = FALSE;
			if (UART_Com1_Para.RecvOKFLAG == TRUE)		//有串行命令进入,串口收完数据
			{
				    Uart_Proc();
			}
			if (Event_FM175xx_IRQ == TRUE)
			{
				
					Event_FM175xx_IRQ = FALSE;
					
					if(LpcdEn == 1)												//支持LPCD功能
					{
						GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
						
						if ((lpcd_irq & BFL_JBIT_CARD_IN_IRQ) == BFL_JBIT_CARD_IN_IRQ )	
						{
							//BUZZ_ON;
							//mDelay(200);
							//BUZZ_OFF;	
							LpcdCardIn_IRQHandler();	
						}
						if ((lpcd_irq & BFL_JBIT_CARD_IN_IRQ) == BFL_JBIT_AUTO_WUP_IRQ )
						{
							//LpcdAutoWakeUp_IRQHandler();
						}	
						
						//如果定时唤醒，则定时调教
						/*
						if (lpcd_irq & BFL_JBIT_AUTO_WUP_IRQ)	
						{
							LpcdAutoWup_IRQHandler();	
						}
						*/
						//TIM1_Cmd(ENABLE);					

					}
					else
					{
						FM175xx_IRQ_Pro(PHCS_BFL_JBIT_HIALERTI | PHCS_BFL_JBIT_RXI|PHCS_BFL_JBIT_ERRI,PHCS_BFL_JBIT_MODEI);
						if(FM175xx_Card_Actived)
						{							
							FM175xx_Card_APDU();
						
							ModifyReg(COMMIEN,PHCS_BFL_JBIT_RXI | PHCS_BFL_JBIT_HIALERTI,SET);

							SetReg(COMMIRQ,PHCS_BFL_JBIT_RXI | PHCS_BFL_JBIT_HIALERTI );		//清除中断
							FM175xx_Card_Actived = false;
						} 
						
					}	
					
			}
			if(Event_ARM_TIMER_IRQ == TRUE)
			{
					Event_ARM_TIMER_IRQ = FALSE;
					if(GLED->ODR & LED_ARM_WORKING)
					{
						LED_ARM_WORKING_ON;	   //Arm Working
					}
					else
					{
						LED_ARM_WORKING_OFF;
					}
			}
		}	
		
		if(User_Info[0] == 0x1)				//ReaderA功能开启
		{
			if(Sys_Timer_Count > ReaderA_Timer_Count )// (ReaderA_Timer_Count+1))		//2s
			{
				OLED_ShowString(5,STING_SIZE*4+5,"RDER A");  	//ROW 4
				OLED_Refresh_Gram();
				FM175xx_Initial_ReaderA();
				if(ReaderA_Request(RF_CMD_REQA) == FM175XX_SUCCESS)
				{
					BUZZ_ON;
					mDelay(100);
					BUZZ_OFF;
					if(ReaderA_AntiCol(0) == FM175XX_SUCCESS)						//一重防冲突
					{
						OLED_ShowNum(65,STING_SIZE*4,CardA_Sel_Res.UID[0],2,STING_SIZE);
						OLED_ShowNum(79,STING_SIZE*4,CardA_Sel_Res.UID[1],2,STING_SIZE);
						OLED_ShowNum(93,STING_SIZE*4,CardA_Sel_Res.UID[2],2,STING_SIZE);
						OLED_ShowNum(107,STING_SIZE*4,CardA_Sel_Res.UID[3],2,STING_SIZE);
						OLED_Refresh_Gram();
					}
				}
				else
				{
					OLED_ShowString(65,STING_SIZE*4,"         ");
				}
				ReaderA_Timer_Count = Sys_Timer_Count;
			}
		}
		if(User_Info[1] == 0x1)				//ReaderB功能开启
		{

				if(Sys_Timer_Count > (ReaderB_Timer_Count+3))		//5ms
				{
					OLED_ShowString(5,STING_SIZE*4+5,"RDER B");  //ROW 4
					OLED_Refresh_Gram();
					FM175xx_Initial_ReaderB();
					ReaderB_Timer_Count = Sys_Timer_Count;
				}
		}
		if(User_Info[2] == 0x1)				//ReaderF功能开启
		{
				if(Sys_Timer_Count > (ReaderF_Timer_Count+3))		//5ms
				{
					OLED_ShowString(5,STING_SIZE*4+5,"RDER F");  //ROW 4
					OLED_Refresh_Gram();
					FM175xx_Initial_ReaderF();
					ReaderF_Timer_Count = Sys_Timer_Count;	
				}
				
		}
		if(User_Info[3] == 0x1)				//CARD功能开启
		{
				if(Sys_Timer_Count > (Card_Timer_Count+3))		//5ms
				{
					OLED_ShowString(5,STING_SIZE*4+5,"CARD P");  //ROW 4
					OLED_Refresh_Gram();
					if(User_Info[3] == 0x01)
					{
						FM175xx_Initial_Card();
						FM175xx_Card_Config(Card_UID_Buf);
						FM175xx_Card_AutoColl();
						User_Info[3]++;
					}
					Card_Timer_Count = Sys_Timer_Count;
					User_Info[3] = 0;
				}		
		}
	}
}









