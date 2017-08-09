/*********************************************************************
*                                                                    *
*   Copyright (c) 2010 Shanghai FuDan MicroElectronic Inc, Ltd.      *
*   All rights reserved. Licensed Software Material.                 *
*                                                                    *
*   Unauthorized use, duplication, or distribution is strictly       *
*   prohibited by law.                                               *
*                                                                    *
**********************************************************************
*********************************************************************/
/*********************************************************************/
/* 	FM175xx LPCD API函数库											 */
/* 	主要功能:           											 */
/* 		1.实现LPCD相关API函数										 */
/* 	编制:罗挺松 													 */
/* 	编制时间:2014年6月   											 */
/* 																	 */
/*********************************************************************/
#include "lpcd_reg_private.h"
#include "lpcd_reg_public.h"
#include "lpcd_config_private.h"
#include "lpcd_config_public.h"
#include "lpcd_regctrl_private.h"
//#include "global.h"
//#include "BasicAPi.h"
#include "ReaderAPI.h"
#include "oled.h"
#include "global.h"

#define IF_ERR_THEN_RETURN if (ret == FALSE) return FALSE

//********************************************************************
//内部函数列表
//********************************************************************
unsigned char LpcdParamInit(void);
unsigned char LpcdRegisterInit(void);

//********************************************************************
//外部函数列表
//********************************************************************
extern unsigned char LpcdReset(void);
extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
extern unsigned char LpcdInitCalibra(unsigned char *CalibraFlag);
extern unsigned char LpcdAuxSelect(unsigned char OpenClose);

//********************************************************************
//内部变量列表
//********************************************************************
unsigned char PulseClkDivK ;
unsigned char Timer1Cfg;			//4bit,检测间隔时间
unsigned char Timer2Cfg;			//5bit,检测准备时间
unsigned char Timer3Cfg;			//5bit,检测采样时间

unsigned char PulseWidthFullScale;	//T3下满幅PulseWidth宽度
unsigned char PulseWidthThreshold;	//检测宽度，设置成相对值
unsigned char PulseThreshold_L;		//LPCD脉宽低阈值
unsigned char PulseThreshold_H;		//LPCD脉宽高阈值
unsigned char PulseWidthCenter;		//LPCD脉宽中心点

unsigned char LpcdDetectTimes;		//检测次数;
unsigned char LpcdThresholdRatio;	//检测灵敏度
unsigned char LpcdAutoWakeUpEn;		//自动唤醒使能
unsigned char LpcdAutoWakeUpCFG;	//自动唤醒时间设置

//***********************************************
//函数名称：NRSTPD_CTRL()
//函数功能：NRSTPD控制函数
//入口参数：
//出口参数：
//***********************************************
void NRSTPD_CTRL(unsigned char ctrltype)
{
	//====================================================
	//以下部分用户根据NRSTPD和MCU控制连接关系修改
	//====================================================
	if(ctrltype == 0x0)
		GPIO_ResetBits(GNRST,NRST);
	else
		GPIO_SetBits(GNRST,NRST);	
}

//***********************************************
//函数名称：LpcdParamInit()
//函数功能：LPCD参数初始化
//入口参数：
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//函数类型：public
//***********************************************
unsigned char LpcdParamInit()
{
	unsigned char LpcdCfgBug[20];
	unsigned char *ptrBuf;
	
	Flash_Read_Lpcd_Cfg(LpcdCfgBug);								//从Flash里读取配置信息；
	ptrBuf = LpcdCfgBug;
	if((*(ptrBuf++) == 0x55)&&(*(ptrBuf++) == 0xAA))				//Flash里有配置信息
	{
		Timer1Cfg = *(ptrBuf++);
		if(15 < Timer1Cfg)											//超出了Timer的配置量程;
			Timer1Cfg = TIMER1_CFG;
			
		Timer2Cfg = TIMER2_CFG;
		
		Timer3Cfg = *(ptrBuf++);
		if(31 < Timer3Cfg)											//超出了Timer的配置量程;
			Timer3Cfg = TIMER3_CFG;
			
		LpcdDetectTimes = *(ptrBuf++);
		if((0 != LpcdDetectTimes) && (1 != LpcdDetectTimes))		//flash异常
			LpcdDetectTimes = LPCD_AUTO_DETECT_TIMES;
		
		LpcdThresholdRatio = *(ptrBuf++);
		if((2 > LpcdThresholdRatio) || (5 < LpcdThresholdRatio))	//flash异常
			LpcdThresholdRatio = LPCD_THRESHOLD_RATIO;
			
		LpcdAutoWakeUpEn = *(ptrBuf++);
			
		LpcdAutoWakeUpCFG = *(ptrBuf++);
		if(7 < LpcdAutoWakeUpCFG)									//flash异常
			LpcdAutoWakeUpCFG = AUTO_WUP_CFG;
	}
	else
	{
		Timer1Cfg = TIMER1_CFG;	
		Timer2Cfg = TIMER2_CFG;
		Timer3Cfg = TIMER3_CFG;
		LpcdThresholdRatio = LPCD_THRESHOLD_RATIO;
		LpcdDetectTimes = LPCD_AUTO_DETECT_TIMES;
		LpcdAutoWakeUpEn = AUTO_WUP_EN;
		LpcdAutoWakeUpCFG = AUTO_WUP_CFG;
	}
	
	if (Timer3Cfg > 0xF)			//Timer3Cfg用到5bit，所以只能选择16分频
	{
		PulseClkDivK = 2;			//16分频
		PulseWidthFullScale =  ((Timer3Cfg - 1)<<3);
		PulseWidthCenter = (PulseWidthFullScale >>1);
		PulseWidthThreshold = (PulseWidthFullScale >> LpcdThresholdRatio);
	}
	else if(Timer3Cfg > 0x7)		//Timer3Cfg用到4bit，所以只能选择8分频
	{
		PulseClkDivK = 1;			//8分频
		PulseWidthFullScale =  ((Timer3Cfg - 1)<<4);
		PulseWidthCenter = (PulseWidthFullScale >>1);
		PulseWidthThreshold = (PulseWidthFullScale >> LpcdThresholdRatio);
	}
	else 
	{
		PulseClkDivK = 0;			//4分频
		PulseWidthFullScale =  ((Timer3Cfg - 1)<<5);
		PulseWidthCenter = (PulseWidthFullScale >>1);
		PulseWidthThreshold = (PulseWidthFullScale >> LpcdThresholdRatio);
	}

	PulseThreshold_H = PulseWidthCenter + PulseWidthThreshold;
	PulseThreshold_L= PulseWidthCenter - PulseWidthThreshold;

	return TRUE;
}
//***********************************************
//函数名称：LpcdRegisterInit()
//函数功能：LPCD寄存器初始化
//入口参数：
//出口参数：uchar  TRUE：初始化成功   FALSE:初始化失败
//函数类型：public
//***********************************************
unsigned char LpcdRegisterInit(void)
{
	unsigned char ret;
	unsigned char regdata;
	
	//-----------------------------------------------------------------
	//中断设置
	//-----------------------------------------------------------------
	regdata = COMMIEN_DEF;			 //中断设置
	ret = SetReg(COMMIEN,regdata);
	IF_ERR_THEN_RETURN;

	regdata = DIVIEN_DEF;			 
	ret = SetReg(DIVIEN,regdata);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//LpcdCtr1寄存器
	//-----------------------------------------------------------------
	//复位LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//复位放开LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//使能LPCD功能
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_EN);
	IF_ERR_THEN_RETURN;
	//配置LPCD中断
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,(LPCD_IE<<5)+BFL_JBIT_LPCD_IE);
	IF_ERR_THEN_RETURN;	
	//配置进场检测次数
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,(LpcdDetectTimes<<5)+BFL_JBIT_LPCD_CMP_1);
	IF_ERR_THEN_RETURN;	
	//-----------------------------------------------------------------
	//LpcdCtrl2寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LPCD_CWN<<3)+LPCD_CWP));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//LpcdCtrl3寄存器 复位值就是000，该寄存器屏蔽
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL3,LPCD_MODE<<3);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer1Cfg寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER1CFG,(PulseClkDivK<<4)+Timer1Cfg);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//Timer2Cfg寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER2CFG,Timer2Cfg);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//Timer3Cfg寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER3CFG,Timer3Cfg);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//VmidBdCfg寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_VMIDBDCFG,VMID_BG_CFG);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//AutoWupCfg寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_AUTO_WUP_CFG,(LpcdAutoWakeUpEn<<3)+LpcdAutoWakeUpCFG);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_L1寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L & 0x3F));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_L2寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L2,(PulseThreshold_L>>6));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_H1寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H1,(PulseThreshold_H& 0x3F));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_H2寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H2,(PulseThreshold_H>>6));
	IF_ERR_THEN_RETURN;
	
	//-----------------------------------------------------------------
	//Auto_Wup_Cfg寄存器
	//-----------------------------------------------------------------
	ret=SetReg_Ext(BFL_JREG_AUTO_WUP_CFG,((LpcdAutoWakeUpEn<<3) + LpcdAutoWakeUpCFG ));
	IF_ERR_THEN_RETURN;
		
	return TRUE;
}
//***********************************************
//函数名称：LpcdInitialize()
//函数功能：LPCD初始化程序
//入口参数：无
//出口参数：调教成功标志
//函数类型：public
//***********************************************
unsigned char LpcdInitialize(void)
{
	unsigned char CalibraFlag;
	LpcdParamInit();					//LPCD参数初始化
	LpcdRegisterInit();       			//LPCD寄存器初始化
	LpcdAuxSelect(ON);					//开启AUX观测通道
	LpcdInitCalibra(&CalibraFlag);		//LPCD初始化调教
	return CalibraFlag;
}
//***********************************************
//函数名称：LpcdCardIn_IRQHandler()
//函数功能：卡片进场服务程序
//入口参数：
//出口参数：
//函数类型：public
//***********************************************
void LpcdCardIn_IRQHandler(void)
{
	unsigned char PulseWidth;
	unsigned char lpcd_irq;
	unsigned char ret;
	unsigned char Card_In = False;
	//-----------------------------------------------------------------
	//读取中断标志
	//-----------------------------------------------------------------
	ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
	//if (lpcd_irq != 0x01)
	//{
	//	lpcd_irq = 0x01;
	//}
	
	//-----------------------------------------------------------------
	//读取宽度信息
	//-----------------------------------------------------------------
	ReadLpcdPulseWidth(&PulseWidth);
	//复位LPCD
	ret = LpcdReset();
	
	//LTS 20140701
	ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
	if (lpcd_irq != 0x00)
	{
		lpcd_irq = 0x00;
	}
	
	
	//需要延时，否则有可能最后一个寄存器写不进
//LTS20140701	mDelay(100);
	
	//====================================================
	//以下部分用户可以编写检测到卡片的后续程序
	//====================================================
	//用户程序开始
	LED_ARM_WORKING_OFF;

	//if(LpcdFunc == 0)		//仅仅LPCD测试
	//{
		OLED_ShowNum(65,STING_SIZE*2,(PulseWidthCenter & 0xFF),2,STING_SIZE);
		OLED_ShowNum(79,STING_SIZE*2,(PulseThreshold_L & 0xFF),2,STING_SIZE);
		OLED_ShowNum(93,STING_SIZE*2,(PulseThreshold_H & 0xFF),2,STING_SIZE);
		OLED_ShowNum(107,STING_SIZE*2,(PulseWidth & 0xFF),2,STING_SIZE);
		OLED_Refresh_Gram();	
	//}
	if(LpcdFunc == 1) //还演示读卡相关
	{	
		
		FM175xx_Initial_ReaderA();
		
		if(ReaderA_Request(RF_CMD_REQA))
		{
			if(ReaderA_AntiCol(0))					//一重防冲突
			{
				if(ReaderA_Select(0))
				{
					//LTS20140701临时修改
					BUZZ_ON;
					mDelay(200);
					BUZZ_OFF;	
					if(ReaderA_Rats(0x5,0x0))
					{
						//OLED_ShowString(10,STING_SIZE*4+5,"CardA   ");  	//ROW 4
						OLED_ShowNum(65,STING_SIZE*3,CardA_Sel_Res.UID[0],2,STING_SIZE);
						OLED_ShowNum(79,STING_SIZE*3,CardA_Sel_Res.UID[1],2,STING_SIZE);
						OLED_ShowNum(93,STING_SIZE*3,CardA_Sel_Res.UID[2],2,STING_SIZE);
						OLED_ShowNum(107,STING_SIZE*3,CardA_Sel_Res.UID[3],2,STING_SIZE);
						OLED_Refresh_Gram();
						
						if(GetKey(DoorKey))
						{
							OLED_ShowString(10,STING_SIZE*4+5,"DoorKey");  	//ROW 4
							OLED_ShowNum(65,STING_SIZE*4,DoorKey[0]-'0',2,STING_SIZE);
							OLED_ShowNum(79,STING_SIZE*4,DoorKey[1]-'0',2,STING_SIZE);
							OLED_ShowNum(93,STING_SIZE*4,DoorKey[2]-'0',2,STING_SIZE);
							OLED_ShowNum(107,STING_SIZE*4,DoorKey[3]-'0',2,STING_SIZE);
							OLED_Refresh_Gram();
						}
						Card_In = true;
					}
				}
			}
			
		}
		
		if(Card_In == False)
		{
			OLED_ShowString(65,STING_SIZE*3,"          ");			
			OLED_ShowString(10,STING_SIZE*4+5,"        ");  	//ROW 4
			OLED_ShowString(65,STING_SIZE*4,"          ");
			OLED_Refresh_Gram();
		}
		
	}
	
	//用户程序结束
	//====================================================
	//LTS 20140701
	
	//FieldOff();
	//LpcdReset();
	//GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
	
	//mDelay(1000);
	
	NRSTPD_CTRL(0);								//继续进入LPCD模式
	mDelay(10);
	LED_ARM_WORKING_OFF;

}
//***********************************************
//函数名称：LpcdAutoWup_IRQHandler()
//函数功能：自动唤醒调教服务程序
//入口参数：
//出口参数：
//***********************************************
void LpcdAutoWup_IRQHandler(void)
{
 	/*
	unsigned char PulseWidth;
	LED2_ON;
	ReadLpcdPulseWidth(&PulseWidth);
	//复位LPCD寄存器
	SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	//复位放开LPCD寄存器
	SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	//需要延时，否则有可能最后一个寄存器写不进
	//mDelay(100);
	DisplayBuffer[0]=(PulseWidthCenter>> 4) & 0x0F;
	DisplayBuffer[1]=PulseWidthCenter & 0x0F;;
	DisplayBuffer[2]=(PulseWidth>> 4) & 0x0F;
	DisplayBuffer[3]=PulseWidth & 0x0F;;
	PS7219_ShowBuf(DisplayBuffer);

	NRSTPD_CTRL(0);		//继续进入LPCD模式
	LED2_OFF;
	*/
}


