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

extern bool Event_Flag;		  		//�Ƿ����¼�Ҫ����
extern bool Event_FM175xx_IRQ;		//FM175xx IRQ�Ƿ�����IRQ�������ظ��ж�����
extern bool Event_ARM_TIMER_IRQ;

extern bool Test_LPCD_AutoWakeUp;	//AutoWakeUp���Ա�־��
extern u16  intAutoWakeSec;			//AutoWakeUp�ļ���ֵ����λΪS

extern uchar FM175xx_INTERFACE;		//�ӿ�����
extern uchar FM175xx_CurFunc;       	//��ǰ����

extern bool FM175xx_Time_Out;			//FM175xx ��ʱ��־
extern bool FM175xx_Card_Processing;  //��Ƭ�������ڴ�������
extern bool FM175xx_Card_Actived;		//��Ƭ�ѱ�����

extern uchar LpcdFunc;				//ʹ��LPCD���ܱ�־λ
extern uchar LpcdEn;	
extern uchar DoorKey[4];

extern vu8 datatranstime;
extern uchar pbcmd;
extern vu32 time3tick;
extern bool adc_data_ready;
//extern u8 Card_Config_Buf[25];		//FM175XX Card UID ���û�����
/***************************************/
//User_Info[0]		//�ѻ�ģʽ��ReaderA�������
//User_Info[1]		//�ѻ�ģʽ��ReaderB�������
//User_Info[2]		//�ѻ�ģʽ��ReaderF�������
//User_Info[3]		//�ѻ�ģʽ��Card�������
//User_Info[4]		//������
//User_Info[5]		//������
//User_Info[6]		//������
//User_Info[7]		//������
//User_Info[8]		//������
//User_Info[9]		//������
extern u8 User_Info[10];			//FM175XX �û��Զ������û�����
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
