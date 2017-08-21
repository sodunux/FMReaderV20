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
/* 	主要功能:														 */
/* 		1.实现LPCD相关API函数										 */
/* 	编制:罗挺松 													 */
/* 	编制时间:2013年7月18日											 */
/* 																	 */
/*********************************************************************/
#include "lpcd_reg_private.h"
#include "lpcd_reg_public.h"
#include "lpcd_config_private.h"
#include "lpcd_config_public.h"
#include "lpcd_regctrl_private.h"
#include "lpcd_api_public.h"
#include "global.h"
#include "BasicAPi.h"
#include "ReaderAPI.h"
#include "oled.h"

#define IF_ERR_THEN_RETURN if (ret == FALSE) return FALSE
#define CHARGE_CAP_MIN 0x0	//实际并联电容值最小
#define CHARGE_CAP_MAX 0x7F	//实际电容值最大

//********************************************************************
//内部函数列表
//********************************************************************
unsigned char LpOscCalibra(void);
unsigned char WaitForLpcdIrq(unsigned char IrqType);
extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
unsigned char CalibraReadPulseWidth(unsigned char *PulseWidth);
unsigned char LpcdAuxSelect(uchar OpenClose);



//********************************************************************
//调教变量列表
//********************************************************************
//unsigned char PulseClkDivK ;
unsigned char ChargeCurrent ;//3bit  //由样片测试指标决定，在config文件中设定
unsigned char LpcdGainReduce;//2bit
unsigned char LpcdGainAmplify;//3bit
unsigned char ChargeCap;//7bit 

//unsigned char Timer1Cfg;//4bit
//unsigned char Timer2Cfg;//5bit
//unsigned char Timer3Cfg;//5bit

//unsigned char PulseWidthFullScale;	//T3下满幅PulseWidth宽度
//unsigned char PulseWidthThreshold;	//检测宽度，设置成相对值
//unsigned char PulseThreshold_L;		//LPCD脉宽低阈值
//unsigned char PulseThreshold_H;		//LPCD脉宽高阈值
//unsigned char PulseWidthCenter;		//LPCD脉宽中心点
unsigned char LpcdPulseWidth[10];	//Lpcd脉宽宽度，用于误触发判断


//***********************************************
//函数名称：LpcdInitCalibra()
//函数功能：初始化调教
//入口参数：uchar *CalibraFlag 调教标志，是否需要调教
//出口参数：uchar  TRUE：调教成功   FALSE:调教失败
//函数类型：private
//***********************************************
unsigned char  LpcdInitCalibra(unsigned char *CalibraFlag)
{
	unsigned char ret;
	unsigned char PulseWidth;				//LPCD脉宽宽度
	unsigned char GainCalibraFlag;			//增益调教结果
	unsigned char PulseWidth_Pre;			//脉宽宽度的前一个值
	unsigned char LpcdCwp;					//PMOS驱动配置
	unsigned char LpcdCwn;					//PMOS驱动配置
	unsigned char RegData;

	//初始化配置变量
	ChargeCurrent = CHARGE_CURRENT ; //由样片测试指标决定，在config文件中设定
 	LpcdGainReduce = 0x3;			//1x
 	LpcdGainAmplify = 0x0;			//1x
	ChargeCap = 0;
	
	//配置增益
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
	IF_ERR_THEN_RETURN;

	//配置充电电容和充电电容
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)>>1)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;

	//V02 增加 CalibVmidEn时能
	ret = SetReg_Ext(BFL_JREG_MISC,BFL_JBIT_CALIB_VMID_EN);
	IF_ERR_THEN_RETURN;
	
	//恢复缺省发射驱动能力配置，如果放大倍数不够，则需要增大发射功率
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LPCD_CWN<<3)+LPCD_CWP));
	IF_ERR_THEN_RETURN;
	//备份PMOS和NMOS配置
	ret = GetReg_Ext(BFL_JREG_LPCDCTRL2,&RegData);
	IF_ERR_THEN_RETURN;
	LpcdCwp = RegData & 0x07;
	LpcdCwn = (RegData & 0x08) >> 3;

	//Timer1Cfg配置
	ret = SetReg_Ext(BFL_JREG_TIMER1CFG,(PulseClkDivK<<4)+Timer1Cfg);
	IF_ERR_THEN_RETURN;

	//Timer2Cfg配置
	ret = SetReg_Ext(BFL_JREG_TIMER2CFG,Timer2Cfg);
	IF_ERR_THEN_RETURN;

	//Timer3Cfg配置
	ret = SetReg_Ext(BFL_JREG_TIMER3CFG,Timer3Cfg);
	IF_ERR_THEN_RETURN;

	//电容值配置最小
	ChargeCap = CHARGE_CAP_MIN;
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)>>1)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;
	ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
	IF_ERR_THEN_RETURN;

	//调教读取当前脉宽宽度
	ret = CalibraReadPulseWidth(&PulseWidth);
	IF_ERR_THEN_RETURN;

	//缺省增益不需要调教
	GainCalibraFlag = TRUE;

	//判断是否脉宽太窄，如果太窄lpcd_gain衰减
	if  (PulseWidth < PulseWidthCenter)
	{
		//增益需要调教
		GainCalibraFlag = FALSE;
		while(1)
		{
			//如果当前已经是最小增益，调教失败
			if (LpcdGainReduce == 0)
			{
				GainCalibraFlag = FALSE;
				break;
			}
			//继续衰减
			LpcdGainReduce --; 
			 
			//配置增益
			ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
			IF_ERR_THEN_RETURN;

			//调教读取当前脉宽宽度
			ret = CalibraReadPulseWidth(&PulseWidth);
			IF_ERR_THEN_RETURN;
			
			//调教成功，把中心点移到中心点右侧
			if (PulseWidth >PulseWidthCenter)
			{
				GainCalibraFlag = TRUE;
				break;
			}
		}
	}
	else
	{
		//电容值配置最大
		ChargeCap = CHARGE_CAP_MAX;
		ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
		IF_ERR_THEN_RETURN;
		ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
		IF_ERR_THEN_RETURN;

		//调教读取当前脉宽宽度
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		
		//调教成功标志初始化
		GainCalibraFlag = TRUE;
		
		//判断是否脉宽太宽，如果太宽lpcd_gain放大
		if (PulseWidth > PulseWidthCenter)
		{
			//增益需要调教
			GainCalibraFlag = FALSE;
			while(1)
			{
				//如果当前已经是最大增益，换一种方式增大发射功率
				if (LpcdGainAmplify == 0x7)
				{
					//如果放大到最大增益，依然不够，考虑增大发射功率
					if ((LpcdCwp == 0x07) || (LPCD_CALIBRA_REDUCE_PMOS == 0)) 
					{	
						//输出驱动能力已经最大，调教失败	
						GainCalibraFlag = FALSE;
						break;
					}
					else
					{
						LpcdCwp ++;
						LpcdCwn = 0;
						ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LpcdCwn<<3)+LpcdCwp));
						IF_ERR_THEN_RETURN;
					}
					
				}
				else//继续放大
				{
					LpcdGainAmplify++;  
				}
				//配置增益
				ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
				IF_ERR_THEN_RETURN;
				
				

				//调教读取当前脉宽宽度
				ret = CalibraReadPulseWidth(&PulseWidth);
				IF_ERR_THEN_RETURN;
				
				ret = GetReg_Ext(BFL_JREG_LPCDCTRL4,&RegData);
				IF_ERR_THEN_RETURN;
				
				//调教成功，把中心点移到中心点左侧
				if (PulseWidth < PulseWidthCenter)
				{
					GainCalibraFlag = TRUE;
					break;
				}
			}

		}
	}
	//如果增益调教失败，则失败
	if (GainCalibraFlag == FALSE)
	{		
		(*CalibraFlag) = FALSE;
		return PulseWidth;	   //调教失败返回脉宽
	}
	//扫描电容值，找到合适的穿越中心点的配置
	(*CalibraFlag) = FALSE;
	//备份脉宽宽度
	PulseWidth_Pre = PulseWidth;
	//扫描充电电容
	for(ChargeCap = CHARGE_CAP_MIN;ChargeCap < CHARGE_CAP_MAX;ChargeCap++)
	{
		//配置电容值
		ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
		IF_ERR_THEN_RETURN;
		ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
		IF_ERR_THEN_RETURN;

		//备份脉宽宽度
		PulseWidth_Pre = PulseWidth;
		//调教读取当前脉宽宽度
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		//==============================================
		//算法一
		//==============================================
		//如果脉宽开始比中心值小，因为开始比中心值大
		if (PulseWidth < PulseWidthCenter)
		{
			//调教成功
			(*CalibraFlag) = TRUE;
			//与前一个调教电容，判断哪个脉宽宽度更接近中心点
			if((PulseWidthCenter-PulseWidth) < (PulseWidth_Pre-PulseWidthCenter))
			{
				//直接用当前值作为中心点
				PulseWidthCenter = PulseWidth;
			}
			else
			{
				//直接用当前值作为中心点
				PulseWidthCenter = PulseWidth_Pre;
				//充电电容采用之前的充电电容
				ChargeCap--;
				//重新配置电容值
				ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
				IF_ERR_THEN_RETURN;
				ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
				IF_ERR_THEN_RETURN;

			}
			//微调阈值
			PulseThreshold_H = PulseWidthCenter + PulseWidthThreshold;
			PulseThreshold_L= PulseWidthCenter - PulseWidthThreshold;
			//-----------------------------------------------------------------
			//PulseThreshold_L1寄存器
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
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

			break;
		}
	}
	if (GainCalibraFlag == FALSE)
	{		
		(*CalibraFlag) = FALSE;
		//V02 增加 调教结束关闭CalibVmidEn
		ret = ModifyReg_Ext(BFL_JREG_MISC,BFL_JBIT_CALIB_VMID_EN,0);
		IF_ERR_THEN_RETURN;
		return PulseWidth;	   //调教失败返回脉宽
	}
	
	//V02 增加 调教结束关闭CalibVmidEn
	ret = ModifyReg_Ext(BFL_JREG_MISC,BFL_JBIT_CALIB_VMID_EN,0);
	IF_ERR_THEN_RETURN;
	return TRUE;
}

//***********************************************
//函数名称：LpcdPeriodCalibra()
//函数功能：定时调教，主要通过调节电容，调整中心点
//入口参数：uchar *CalibraFlag 调教标志，是否需要调教
//出口参数：uchar  TRUE：寄存器读写成功   FALSE:寄存器读写失败
//***********************************************
/*
unsigned char  LpcdPeriodCalibra(unsigned char * CalibraFlag)
{
	unsigned char ret;
	// unsigned char LpcdPeriodCalibraResult;	//LPCD调教成功
	unsigned char PulseWidth;			//LPCD脉宽宽度

	//调教读取当前脉宽宽度
	ret = CalibraReadPulseWidth(&PulseWidth);
	IF_ERR_THEN_RETURN;
	//脉宽宽度在中心点附近可以无需调教，退出
	if ((PulseWidth < (PulseWidthCenter + PULSE_CENTER_RANGE))  
		| (PulseWidth > (PulseWidthCenter - PULSE_CENTER_RANGE))) 
	{
			(*CalibraFlag) = TRUE;
			return True;
	}
	//扫描电容值，找到合适的穿越中心点的配置
	(*CalibraFlag) = FALSE;
	for(ChargeCap = CHARGE_CAP_MIN;ChargeCap < CHARGE_CAP_MAX;ChargeCap++)
	{
		//配置电容值
		#if  (NB1117_FPGA == 1)
		ret = NB1117V02_ChargeConfig(LpcdGainReduce,ChargeCap,ChargeCurrent);
		IF_ERR_THEN_RETURN;
		#else
		ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
		IF_ERR_THEN_RETURN;
		ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
		IF_ERR_THEN_RETURN;
		#endif
		//调教读取当前脉宽宽度
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		//脉宽宽度在中心点附近可以结束调教
		if ((PulseWidth < (PulseWidthCenter + PULSE_CENTER_RANGE))  
			| (PulseWidth > (PulseWidthCenter - PULSE_CENTER_RANGE))) 
		{
			(*CalibraFlag) = TRUE;
			//微调中心点
			if (PulseWidth < (PulseWidthCenter))
			{
				PulseWidthCenter =  PulseWidthCenter + (PulseWidthCenter - PulseWidth);
			}
			else
			{
				PulseWidthCenter =  PulseWidthCenter - (PulseWidth - PulseWidthCenter);
			}
			//微调阈值
			PulseThreshold_H = PulseWidthCenter + PulseWidthThreshold;
			PulseThreshold_L= PulseWidthCenter - PulseWidthThreshold;
			//-----------------------------------------------------------------
			//PulseThreshold_L1寄存器
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
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

			break;
		}
	}
	return TRUE;
}
*/
//***********************************************
//函数名称：LpcdErrorTrigCalibra()
//函数功能：误触发调教,request失败之后调用，判断是否进行参数微调
//入口参数：uchar *CalibraFlag 调教标志，是否需要调教
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//***********************************************
/*
unsigned char  LpcdErrorTrigCalibra(unsigned char * CalibraFlag)
{	
	//--------------------------------------------------------
	//进入card_in_irq中断之后就读取过plusewidth，存放在LpcdPulseWidth[0]
	//完成request之后，寻卡失败后调用误触发函数
	//--------------------------------------------------------
	unsigned char ret;
	unsigned char PulseWidth;
	unsigned char i;
	//初始化成LPCD模式
	ret = LpcdRegisterInit();
	IF_ERR_THEN_RETURN;
	//采集1S的脉宽宽度
	for (i=1;i<10;i++)
	{
		//调教读取当前脉宽宽度
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		//保存脉宽宽度
		LpcdPulseWidth[i] = PulseWidth;

		//延时100ms
		mDelay(100);	
	}
	*CalibraFlag = FALSE;
	//可以根据算法对LpcdPulseWidth数组进行分析，判断是否是噪声或者温度漂移
	//目前用简单算法判断
	if (LpcdPulseWidth[9] > LpcdPulseWidth[0]) 
	{
		if ((LpcdPulseWidth[9] - LpcdPulseWidth[0]) < ERR_TRIG_JUDGE_RANGE)
		{
			//不是异类卡或者金属物进场，属于噪声或者温漂误触发，需要调教
			*CalibraFlag = TRUE;
		}
	}
	else
	{
		if ((LpcdPulseWidth[0] - LpcdPulseWidth[9]) < ERR_TRIG_JUDGE_RANGE)
		{
			//不是异类卡或者金属物进场，属于噪声或者温漂误触发，需要调教
			*CalibraFlag = TRUE;
		}
	}
	//属于噪声或者温漂误触发，需要调教
	if (*CalibraFlag)
	{
		//如果触发条件是上溢出
		if(LpcdPulseWidth[9] > PulseThreshold_H)
		{
			PulseThreshold_H = PulseThreshold_H + (LpcdPulseWidth[9] - PulseThreshold_H);
		}
		//如果触发条件是下溢出
		else if(LpcdPulseWidth[9] < PulseThreshold_L)
		{
			PulseThreshold_L= PulseThreshold_L + (PulseThreshold_L - LpcdPulseWidth[9]);
		}
		//-----------------------------------------------------------------
		//PulseThreshold_L1寄存器
		//-----------------------------------------------------------------
		ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
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
	}	
	return TRUE;	
	
}
*/
//***********************************************
//函数名称：LpOscCalibra()
//函数功能：低功耗环振初始化
//入口参数：
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//***********************************************
/*
unsigned char  LpOscCalibra(void)
{
	unsigned char ret;
	
	unsigned char ExtRegData;
	unsigned char LpOscCount;
	//复位LPCD相关寄存器
	//ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	//IF_ERR_THEN_RETURN;
	//使能环振，使能调教
	ret = SetReg_Ext(BFL_JREG_LPCD_TEST1,BFL_JBIT_LP_OSC10K_EN+BFL_JBIT_LP_OSC10K_EN);
	IF_ERR_THEN_RETURN;
	//判断调教时候结束
	ret = WaitForLpcdIrq(BFL_JBIT_LP10K_TESTOK_IRQ);
	IF_ERR_THEN_RETURN;
	//读取LP计数值
	ret = GetReg_Ext(BFL_JREG_LP_CLK_CNT2, &ExtRegData);
	LpOscCount = (ExtRegData << 6);
	ret = GetReg_Ext(BFL_JREG_LP_CLK_CNT1, &ExtRegData);
	LpOscCount += ExtRegData;

	//根据LpOscCount进行调教计算 Timer1Cfg
	//如果环振高电平宽度过宽，意味着频率过慢，Timer1Cfg需要减小
	if ((LpOscCount - LP10K_CNT_DEFAULT) > LP10K_CALIBRA_THRESHOLD)
	{
		 Timer1Cfg--;
	}
	//如果环振高电平宽度过窄，意味着频率过快，Timer1Cfg需要增加
	else if ((LP10K_CNT_DEFAULT - LpOscCount) > LP10K_CALIBRA_THRESHOLD)
	{
		 Timer1Cfg++;
	}
	return TRUE;	
}
*/
//***********************************************
//函数名称：WaitForLpcdIrq()
//函数功能：等待LPCD中断
//入口参数：IrqType
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//***********************************************
unsigned char WaitForLpcdIrq(unsigned char IrqType)
{
	unsigned char ExtRegData;
	unsigned char ret;
	unsigned char TimeOutCount;
	
	TimeOutCount = 0;
	ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&ExtRegData);
	//debug
	if (ret == 0)
	{
		ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&ExtRegData);
		ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&ExtRegData);
		ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&ExtRegData);
		ret =1;
	}
	IF_ERR_THEN_RETURN;
	while ((ExtRegData & IrqType) != IrqType)
	{
		ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&ExtRegData);
			//debug
		if (ret == 0)
		{
			ret =1;
		}
		IF_ERR_THEN_RETURN;
		//超时退出
		mDelay(1);	//延时10ms
		TimeOutCount++;
		if  (TimeOutCount > IRQ_TIMEOUT)  
		{
			return FALSE; //150ms 超时退出
		}
	}
	return TRUE; 
	
}

//***********************************************
//函数名称：LpcdCardIn_IRQHandler()
//函数功能：卡片进场服务程序
//入口参数：
//出口参数：
//函数类型：private
//***********************************************
unsigned char LpcdReset(void)
{
	unsigned char ret;
	//复位LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//复位放开LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//因为每次唤醒，测试寄存器会被复位
	ret = LpcdAuxSelect(ON & LPCD_AUX_EN);					//开启AUX观测通道
	IF_ERR_THEN_RETURN;
	
	return TRUE;
}

//***********************************************
//函数名称：ReadLpcdPulseWidth()
//函数功能：读取LPCD脉宽宽度
//入口参数：uchar *PulseWidth
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//函数类型：public
//***********************************************
unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth)
{
	unsigned char ExtRegData;
	unsigned char ret;
	unsigned char PulseTemp;

	//读取宽度信息
	*PulseWidth = 0;
	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH2,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp = ((ExtRegData & 0x3) << 6);

	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH1,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp += (ExtRegData & 0x3F);
    *PulseWidth = PulseTemp;

	//复位LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//复位放开LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;

	return TRUE;
}
//***********************************************
//函数名称：ReadLpcdPulseWidth()
//函数功能： 读取LPCD脉宽宽度
//入口参数：uchar *PulseWidth
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//函数类型：private
//***********************************************
/*
unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth)
{
	unsigned char ExtRegData;
	unsigned char ret;
	unsigned char PulseTemp;

	//读取宽度信息
	*PulseWidth = 0;
	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH2,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp = ((ExtRegData & 0x3) << 6);

	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH1,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp += (ExtRegData & 0x3F);
    *PulseWidth = PulseTemp;

	//复位LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//复位放开LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;

	return TRUE;
}
*/


//***********************************************
//函数名称：CalibraReadPulseWidth()
//函数功能： 调教并读取LPCD脉宽宽度
//入口参数：uchar *PulseWidth
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//函数类型：private
//***********************************************
unsigned char CalibraReadPulseWidth(unsigned char *PulseWidth)
{
	//使能调教模式
	unsigned char ret;
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_CALIBRA_EN);
	IF_ERR_THEN_RETURN;
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_CALIBRA_EN);
	IF_ERR_THEN_RETURN;
	mDelay(1);
	//等待调教结束中断
	ret = WaitForLpcdIrq(BFL_JBIT_CALIB_IRQ);
	//debug
	if (ret == 0)
	{
		ret =1;
	}
	IF_ERR_THEN_RETURN;
	//关闭调教模式
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_CALIBRA_EN);
	IF_ERR_THEN_RETURN;
	//读取脉宽宽度
	ret = ReadLpcdPulseWidth(PulseWidth);
	IF_ERR_THEN_RETURN;
	return TRUE;
}
//***********************************************
//函数名称：LpcdCardIn_IRQHandler()
//函数功能：卡片进场服务程序
//入口参数：
//出口参数：
//函数类型：private
//***********************************************
/*
unsigned char LpcdReset(void)
{
	unsigned char ret;
	//复位LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//复位放开LPCD寄存器
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//因为每次唤醒，测试寄存器会被复位
	ret = LpcdAuxSelect(ON & LPCD_AUX_EN);					//开启AUX观测通道
	IF_ERR_THEN_RETURN;
	
	return TRUE;
}
*/

//***********************************************
//函数名称：LpcdAuxSelect()
//函数功能：LPCD AUX通道选择  aux1=vp_lpcd aux2=vdem_lpcd
//入口参数：uchar TRUE:开启 FALSE ：关闭
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//***********************************************

uchar LpcdAuxSelect(uchar OpenClose)
{

    unsigned char RegAddr,RegData;
    unsigned char RegAddrLpcd,RegDataLpcd;
    unsigned char ret;

    unsigned char mask,set;

	uchar T3Tout = 0;  //D1
	uchar LpcdOut = 1; //D2
	uchar OscEnTout = 0;  //D3
	uchar OscClkTout = 0; //D4
	uchar T1Tout = 0;  //D5

	if ((OpenClose == TRUE) && (LPCD_AUX_EN))//打开AUX通道,配置文件有权限关闭
	{
	
	     //------------------------------------------
	     //disable dac
	     //en_b_dac=1
	     //a39_7=1;
	     //------------------------------------------
	     RegAddr = 0x39;
	     mask = BIT7;
	     set = 1;
	     ret = ModifyReg(RegAddr,mask,set);
		 IF_ERR_THEN_RETURN;
	     //------------------------------------------
	
	     //------------------------------------------
	     //disable rst_follow
	     //rst_follow=0
	     //a3C_1=0;
	     //------------------------------------------
	     RegAddr = 0x3C;
	     mask = BIT1;
	     set = 0;
	     ret = ModifyReg(RegAddr,mask,set);
		 IF_ERR_THEN_RETURN;
	     //------------------------------------------
	
	     //------------------------------------------
	     //disable  bypass_follow
	     //bypass_follow1=0
	     //a3D_1=0;
	     //bypass_follow1=0
	     //a3D_2=0;
	     //by_follow=0
	     //a3D_0=0
	     //------------------------------------------
	     RegAddr = 0x3D;
	     mask = BIT2 | BIT1 | BIT0;
	     set = 0;
	     ret = ModifyReg(RegAddr,mask,set);
		 IF_ERR_THEN_RETURN;
	     //------------------------------------------
	
	     //enable follow
	     //en_b_follow1=0
	     //a3E_1 = 0
	     //en_b_follow2=0
	     //a3E_0 = 0
	     //------------------------------------------
	     RegAddr = 0x3E;
	     mask =  BIT1 | BIT0;
	     set = 0;
	     ret = ModifyReg(RegAddr,mask,set);
		 IF_ERR_THEN_RETURN;
	     //------------------------------------------
	
		 //close analogtest channel
		 //analogtest = 0
		 //a38 0 
	     RegAddr = ANALOGTEST;
		 RegData = 0;
		 ret = SetReg(RegAddr,RegData);
		 IF_ERR_THEN_RETURN;	     
		 
		 RegAddrLpcd  = BFL_JREG_AUX1_TEST;
	     RegDataLpcd = 0x01 ; //vp_lpcd
		 ret = SetReg_Ext(RegAddrLpcd,RegDataLpcd);
		 IF_ERR_THEN_RETURN;
								
		 RegAddrLpcd  = BFL_JREG_AUX2_TEST;
	     RegDataLpcd = 0x02 ; //vdem_lpcd
		 ret = SetReg_Ext(RegAddrLpcd,RegDataLpcd);
		 IF_ERR_THEN_RETURN;
		//------------------------------------------

		//D1=T3
		//D2=LpcdOut
		RegAddr = BFL_JREG_LPCD_TEST1;
        ret = GetReg_Ext(RegAddr,&RegData);
        RegData &= ~0x08;
        ret = SetReg_Ext(RegAddr,RegData);
		IF_ERR_THEN_RETURN;

		RegData = ((T3Tout<<4)+(LpcdOut<<3)+(OscEnTout<<2)+(OscClkTout<<1)+T1Tout);
		RegAddr = BFL_JREG_LPCD_TEST2;
        ret = SetReg_Ext(RegAddr,RegData);
		IF_ERR_THEN_RETURN;

	}
	else
	{
		 RegAddrLpcd  = BFL_JREG_AUX1_TEST;
	     RegDataLpcd = 0x0 ;
		 ret = SetReg_Ext(RegAddrLpcd,RegDataLpcd);
		 IF_ERR_THEN_RETURN;
	
	
		 RegAddrLpcd  = BFL_JREG_AUX2_TEST;
	     RegDataLpcd = 0x0 ;
		 ret = SetReg_Ext(RegAddrLpcd,RegDataLpcd);
		 IF_ERR_THEN_RETURN;

		 RegAddr = BFL_JREG_LPCD_TEST1;
         RegData = 0x0;
         ret = SetReg_Ext(RegAddr,RegData);
		 IF_ERR_THEN_RETURN;

		 RegData = 0x0;
		 RegAddr = BFL_JREG_LPCD_TEST2;
         ret = SetReg_Ext(RegAddr,RegData);
		 IF_ERR_THEN_RETURN;
	}
	return TRUE;
}	
//***********************************************
//函数名称：LpcdAutoTest()
//函数功能：LPCD 自动测试，采集脉宽，会故意在无卡状态下检测到卡片
//入口参数：uchar 
//出口参数：uchar  TRUE：读取成功   FALSE:失败
//函数类型：private
//***********************************************	
uchar LpcdAutoTest(void)
{
	unsigned char ret;
	unsigned char regdata;



	//-----------------------------------------------------------------
	//Default Value
	//-----------------------------------------------------------------
	LpcdGainAmplify = 0x0;
	LpcdGainReduce = 0x3;
	ChargeCap = 0x14;
	ChargeCurrent = 0x0;
	PulseClkDivK = 0x01;
	PulseWidthCenter = 0x5A;
	//PulseThreshold_H = 0x5F;
	//PulseThreshold_L = 0x55;
	PulseThreshold_L = 0;
	PulseThreshold_H = 0;//需要故意产生中断
	Timer1Cfg = 1;//300ms 最小时间间隔

	//-----------------------------------------------------------------
	
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
	//ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,PulseClkDivK<<4+LPCD_TX2RFEN<<3+LPCD_CWN<<2+LPCD_CWP);
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LPCD_CWN<<3)+LPCD_CWP));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//LpcdCtrl3寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL3,LPCD_MODE<<3);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//LpcdCtrl4寄存器  与RegisterInit函数不同
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
    IF_ERR_THEN_RETURN;
	
	//-----------------------------------------------------------------
	//ChargeCurrent寄存器  与RegisterInit函数不同
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//ChargeCap寄存器  与RegisterInit函数不同
	//-----------------------------------------------------------------	
	ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
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
	return TRUE;
}
//***********************************************
//函数名称：LpcdLoadCalibraValue()
//函数功能：加载缺省调教参数
//入口参数：uchar *CalibraFlag 调教标志，是否需要调教
//出口参数：uchar  TRUE：调教成功   FALSE:调教失败
//***********************************************
/*
unsigned char  LpcdLoadCalibraValue(unsigned char *CalibraFlag)
{
	unsigned char ret;
	//-----------------------------------------------------------------
	//Default Value
	//-----------------------------------------------------------------
	LpcdGainAmplify = 0x0;
	LpcdGainReduce = 0x3;
	ChargeCap = 0x14;
	ChargeCurrent = 0x0;
	PulseClkDivK = 0x01;
	PulseWidthCenter = 0x5A;
	PulseThreshold_H = 0x5F;
	PulseThreshold_L = 0x55;
	
	
	

	//-----------------------------------------------------------------
	//配置增益
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//配置电容和充电电流
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)>>1)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer1Cfg配置
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER1CFG,(PulseClkDivK<<4)+Timer1Cfg);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer2Cfg配置
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER2CFG,Timer2Cfg);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer3Cfg配置
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER3CFG,Timer3Cfg);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//PulseThreshold_L1寄存器
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
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

	return TRUE;
}
*/
