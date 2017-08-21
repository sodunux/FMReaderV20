#include "global.h"
#include "stm32f10x_flash.h"

ErrorStatus 	   GlobalErrorStatus;

GPIO_InitTypeDef   GPIO_InitStructure;
USART_InitTypeDef  USART_InitStructure;
UART_Com_Para_Def  UART_Com1_Para;
UART_Com_Para_Def  UART_Com3_Para;
NVIC_InitTypeDef   NVIC_InitStructure;

bool Event_Flag           = FALSE;		    //是否有事件要处理
bool Event_FM175xx_IRQ      = FALSE;			//FM175xx IRQ是否被置起，IRQ的上升沿该中断起来
bool Event_ARM_TIMER_IRQ  = FALSE;			//ARM超时中断，用来指示ARM在工作

bool Test_LPCD_AutoWakeUp = false;	//AutoWakeUp测试标志；
u16  intAutoWakeSec = 0;			//AutoWakeUp的计数值，单位为S

//u8 Card_Config_Buf[25];
u8 User_Info[10];
u8 Lpcd_Cfg[20];
u32	Sys_Timer_Count = 0;

u8 UART1_DATA_BUF[UART1_BUF_LEN];
u8 UART2_DATA_BUF[UART2_BUF_LEN];

uchar Debug_Buf[DEBUG_BUF_LEN];
uchar Debug_Point = 0;

uchar FM175xx_INTERFACE = FM175xx_INTF_DEF;
uchar FM175xx_CurFunc = 0x0;       	        //当前功能为空
//uchar Card_Sel = 0x0;

bool FM175xx_Time_Out = FALSE;
bool FM175xx_Card_Processing = FALSE;
bool FM175xx_Card_Actived    = FALSE;

uchar DisplayBuffer[8]="LPCDinit";

uchar LpcdFunc = 0;	
uchar LpcdEn = 0;	
uchar DoorKey[4];

uchar pbcmd = 0;
vu32 time3tick = 0;
bool adc_data_ready = 0;

void UART_Com_Para_Init(UART_Com_Para_Def* UART_Com_Para)
{	
		u16 i;

		UART_Com_Para->Send_Len = 0;
		UART_Com_Para->Send_Index = 0;
		UART_Com_Para->Recv_Len = 0;
		UART_Com_Para->Recv_Index = 0; 
		UART_Com_Para->RecvStatus = 0;			//接收状态字
		UART_Com_Para->RecvOKFLAG = FALSE;		//接收完毕状

		for(i=0;i<UART1_BUF_LEN;i++)
			UART1_DATA_BUF[i] = 0;
}

void mDelay(u16 ms)			//实测1ms
{
	u32	i;
	u16 j;
	for(j=0;j<ms;j++)
	{
		for(i=0;i<8100;i++)
			;
	}
}

void uDelay(u16 us)			//实测1us
{
	u32	i;
	u16 j;
	for(j=0;j<us;j++)
	{
		for(i=0;i<81;i++)
			;
	}
}

uchar FM175xx_Intf_Detect(void)
{
	uchar io_sel = 0;
	
	if(GIOSEL->IDR & IOSEL0)
	{
		io_sel += 0x01;
	}
	if(GIOSEL->IDR & IOSEL1)
	{
		io_sel += 0x02;
	}
	
	if(io_sel == 0x3)
	{
		FM175xx_INTERFACE = FM175xx_INTF_I2C;
	}
	else if(io_sel == 0x02)
	{
		FM175xx_INTERFACE = FM175xx_INTF_SPI;
	}
	else 
	{
		FM175xx_INTERFACE = FM175xx_INTF_UART;
	}
	return FM175xx_INTERFACE;
}

uchar Flash_Write_Card_UID(uchar *uid)
{
	uchar i;
	u16	  temp;
	FLASH_Unlock();
	FLASH_ClearFlag(FLASH_FLAG_BSY | FLASH_FLAG_EOP | FLASH_FLAG_PGERR | FLASH_FLAG_WRPRTERR);	
	FLASH_ErasePage(FLASH_CARD_UID_START);//
	for(i=0;i<25;i++)
	{
		temp = (uint)(*(uid+i*2))+(((uint)*(uid+i*2+1))<<8);
		FLASH_ProgramHalfWord(FLASH_CARD_UID_START+i*2, temp);
	}
	temp = *(uid+24);
	FLASH_ProgramHalfWord(FLASH_CARD_UID_START+24, temp);
	
	FLASH_Lock();
	return 0x00;
}

uchar Flash_Read_Card_UID(uchar *uid)
{
	uchar i;
	FLASH_Unlock();
	for(i=0;i<25;i++)
	{
		*(uid+i) = *(u8*)(FLASH_CARD_UID_START+i);
	}
	FLASH_Lock();
	return 0x00;
}

uchar Flash_Write_UserInfo(u8* userinfo)
{
	uchar i;
	u16	  temp;
	FLASH_Unlock();
	FLASH_ClearFlag(FLASH_FLAG_BSY | FLASH_FLAG_EOP | FLASH_FLAG_PGERR | FLASH_FLAG_WRPRTERR);	
	FLASH_ErasePage(FLASH_USER_INFO_START);//
	for(i=0;i<5;i++)
	{
		temp = (uint)(*(userinfo+i*2))+(((uint)*(userinfo+i*2+1))<<8);
		FLASH_ProgramHalfWord(FLASH_USER_INFO_START+i*2, temp);
	}
	FLASH_Lock();
	return 0x00;
}

uchar Flash_Read_UserInfo(u8* userinfo)
{
	uchar i;
	FLASH_Unlock();
	for(i=0;i<10;i++)
	{
		*(userinfo+i) = *(u8*)(FLASH_USER_INFO_START+i);
	}
	FLASH_Lock();
	return 0x00;
}


uchar Flash_Write_Lpcd_Cfg(u8* lpcd_cfg)
{
	uchar i;
	u16	  temp;
	FLASH_Unlock();
	FLASH_ClearFlag(FLASH_FLAG_BSY | FLASH_FLAG_EOP | FLASH_FLAG_PGERR | FLASH_FLAG_WRPRTERR);	
	FLASH_ErasePage(FLASH_LPCD_CFG_START);//
	for(i=0;i<5;i++)
	{
		temp = (uint)(*(lpcd_cfg+i*2))+(((uint)*(lpcd_cfg+i*2+1))<<8);
		FLASH_ProgramHalfWord(FLASH_LPCD_CFG_START+i*2, temp);
	}
	FLASH_Lock();
	return 0x00;
}

uchar Flash_Read_Lpcd_Cfg(u8* lpcd_cfg)
{
	uchar i;
	FLASH_Unlock();
	for(i=0;i<20;i++)
	{
		*(lpcd_cfg+i) = *(u8*)(FLASH_LPCD_CFG_START+i);
	}
	FLASH_Lock();
	return 0x00;
}
/**************************Key Scan*************************************/
/**                                                                    */ 
/**                                                                     */
/**                                                                    */
/***********************************************************************/
uchar Keyscan(void)
{
	u8 data1 = 0;
	u8 data2 = 0;
	u8  data3 = 0;
	data1 = (u8)((0xF000&GPIO_ReadInputData(GPIOD))>>12);
	mDelay(1);
	data2 = (u8)((0xF000&GPIO_ReadInputData(GPIOD))>>12);
	mDelay(10);
	data3 =	(u8)((0xF000&GPIO_ReadInputData(GPIOD))>>12);
	if((data1 == data2)&&(data1 != 0x0F)&&(data3 == 0x0F))
	{
		return data2;
		
	}else
	{
		return 0;
	
	}
}
