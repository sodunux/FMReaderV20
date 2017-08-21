#ifndef __GLOBAL_H
#define __GLOBAL_H
#include "stm32f10x_lib.h"
#include "Define.h"

extern ErrorStatus        GlobalErrorStatus;
extern GPIO_InitTypeDef   GPIO_InitStructure;
extern USART_InitTypeDef  USART_InitStructure;
extern UART_Com_Para_Def  UART_Com1_Para;
extern UART_Com_Para_Def  UART_Com3_Para;
extern NVIC_InitTypeDef   NVIC_InitStructure;

extern bool Event_Flag;		  		//是否有事件要处理
extern bool Event_FM175xx_IRQ;		//FM175xx IRQ是否被置起，IRQ的上升沿该中断起来
extern bool Event_ARM_TIMER_IRQ;

extern bool Test_LPCD_AutoWakeUp;	//AutoWakeUp测试标志；
extern u16  intAutoWakeSec;			//AutoWakeUp的计数值，单位为S

extern uchar FM175xx_INTERFACE;		//接口类型
extern uchar FM175xx_CurFunc;       	//当前功能

extern bool FM175xx_Time_Out;			//FM175xx 超时标志
extern bool FM175xx_Card_Processing;  //卡片功能正在处理流程
extern bool FM175xx_Card_Actived;		//卡片已被激活

extern uchar LpcdFunc;				//使能LPCD功能标志位
extern uchar LpcdEn;	
extern uchar DoorKey[4];

extern vu8 datatranstime;
extern uchar pbcmd;
extern vu32 time3tick;
extern bool adc_data_ready;
//extern u8 Card_Config_Buf[25];		//FM175XX Card UID 配置缓冲区
/***************************************/
//User_Info[0]		//脱机模式下ReaderA开启与否
//User_Info[1]		//脱机模式下ReaderB开启与否
//User_Info[2]		//脱机模式下ReaderF开启与否
//User_Info[3]		//脱机模式下Card开启与否
//User_Info[4]		//待定义
//User_Info[5]		//待定义
//User_Info[6]		//待定义
//User_Info[7]		//待定义
//User_Info[8]		//待定义
//User_Info[9]		//待定义
extern u8 User_Info[10];			//FM175XX 用户自定义配置缓冲区
extern u8 Lpcd_Cfg[20];				

extern u32	Sys_Timer_Count;

extern u8 UART1_DATA_BUF[UART1_BUF_LEN]; 
extern u8 UART2_DATA_BUF[UART2_BUF_LEN];
extern uchar DisplayBuffer[8];

extern uchar Debug_Buf[DEBUG_BUF_LEN];
extern uchar Debug_Point;

extern vu8 Uart1TXMode;

extern void UART_Com_Para_Init(UART_Com_Para_Def* UART_Com_Para);
extern void mDelay(u16 ms);
extern void uDelay(u16 us);

extern uchar FM175xx_Intf_Detect(void);
extern uchar Flash_Write_Card_UID(uchar *uid);
extern uchar Flash_Read_Card_UID(uchar *uid);
extern uchar Flash_Write_UserInfo(u8* userinfo);
extern uchar Flash_Read_UserInfo(u8* userinfo);
extern uchar Flash_Write_Lpcd_Cfg(u8* lpcd_cfg);
extern uchar Flash_Read_Lpcd_Cfg(u8* lpcd_cfg);

uchar Keyscan(void);


#endif
