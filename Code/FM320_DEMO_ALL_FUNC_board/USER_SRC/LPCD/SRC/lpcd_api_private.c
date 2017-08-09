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
/* 	FM175xx LPCD API������											 */
/* 	��Ҫ����:														 */
/* 		1.ʵ��LPCD���API����										 */
/* 	����:��ͦ�� 													 */
/* 	����ʱ��:2013��7��18��											 */
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
#define CHARGE_CAP_MIN 0x0	//ʵ�ʲ�������ֵ��С
#define CHARGE_CAP_MAX 0x7F	//ʵ�ʵ���ֵ���

//********************************************************************
//�ڲ������б�
//********************************************************************
unsigned char LpOscCalibra(void);
unsigned char WaitForLpcdIrq(unsigned char IrqType);
extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
unsigned char CalibraReadPulseWidth(unsigned char *PulseWidth);
unsigned char LpcdAuxSelect(uchar OpenClose);



//********************************************************************
//���̱����б�
//********************************************************************
//unsigned char PulseClkDivK ;
unsigned char ChargeCurrent ;//3bit  //����Ƭ����ָ���������config�ļ����趨
unsigned char LpcdGainReduce;//2bit
unsigned char LpcdGainAmplify;//3bit
unsigned char ChargeCap;//7bit 

//unsigned char Timer1Cfg;//4bit
//unsigned char Timer2Cfg;//5bit
//unsigned char Timer3Cfg;//5bit

//unsigned char PulseWidthFullScale;	//T3������PulseWidth���
//unsigned char PulseWidthThreshold;	//����ȣ����ó����ֵ
//unsigned char PulseThreshold_L;		//LPCD�������ֵ
//unsigned char PulseThreshold_H;		//LPCD�������ֵ
//unsigned char PulseWidthCenter;		//LPCD�������ĵ�
unsigned char LpcdPulseWidth[10];	//Lpcd�����ȣ������󴥷��ж�


//***********************************************
//�������ƣ�LpcdInitCalibra()
//�������ܣ���ʼ������
//��ڲ�����uchar *CalibraFlag ���̱�־���Ƿ���Ҫ����
//���ڲ�����uchar  TRUE�����̳ɹ�   FALSE:����ʧ��
//�������ͣ�private
//***********************************************
unsigned char  LpcdInitCalibra(unsigned char *CalibraFlag)
{
	unsigned char ret;
	unsigned char PulseWidth;				//LPCD������
	unsigned char GainCalibraFlag;			//������̽��
	unsigned char PulseWidth_Pre;			//�����ȵ�ǰһ��ֵ
	unsigned char LpcdCwp;					//PMOS��������
	unsigned char LpcdCwn;					//PMOS��������
	unsigned char RegData;

	//��ʼ�����ñ���
	ChargeCurrent = CHARGE_CURRENT ; //����Ƭ����ָ���������config�ļ����趨
 	LpcdGainReduce = 0x3;			//1x
 	LpcdGainAmplify = 0x0;			//1x
	ChargeCap = 0;
	
	//��������
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
	IF_ERR_THEN_RETURN;

	//���ó����ݺͳ�����
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)>>1)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;

	//V02 ���� CalibVmidEnʱ��
	ret = SetReg_Ext(BFL_JREG_MISC,BFL_JBIT_CALIB_VMID_EN);
	IF_ERR_THEN_RETURN;
	
	//�ָ�ȱʡ���������������ã�����Ŵ�������������Ҫ�����书��
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LPCD_CWN<<3)+LPCD_CWP));
	IF_ERR_THEN_RETURN;
	//����PMOS��NMOS����
	ret = GetReg_Ext(BFL_JREG_LPCDCTRL2,&RegData);
	IF_ERR_THEN_RETURN;
	LpcdCwp = RegData & 0x07;
	LpcdCwn = (RegData & 0x08) >> 3;

	//Timer1Cfg����
	ret = SetReg_Ext(BFL_JREG_TIMER1CFG,(PulseClkDivK<<4)+Timer1Cfg);
	IF_ERR_THEN_RETURN;

	//Timer2Cfg����
	ret = SetReg_Ext(BFL_JREG_TIMER2CFG,Timer2Cfg);
	IF_ERR_THEN_RETURN;

	//Timer3Cfg����
	ret = SetReg_Ext(BFL_JREG_TIMER3CFG,Timer3Cfg);
	IF_ERR_THEN_RETURN;

	//����ֵ������С
	ChargeCap = CHARGE_CAP_MIN;
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)>>1)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;
	ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
	IF_ERR_THEN_RETURN;

	//���̶�ȡ��ǰ������
	ret = CalibraReadPulseWidth(&PulseWidth);
	IF_ERR_THEN_RETURN;

	//ȱʡ���治��Ҫ����
	GainCalibraFlag = TRUE;

	//�ж��Ƿ�����̫խ�����̫խlpcd_gain˥��
	if  (PulseWidth < PulseWidthCenter)
	{
		//������Ҫ����
		GainCalibraFlag = FALSE;
		while(1)
		{
			//�����ǰ�Ѿ�����С���棬����ʧ��
			if (LpcdGainReduce == 0)
			{
				GainCalibraFlag = FALSE;
				break;
			}
			//����˥��
			LpcdGainReduce --; 
			 
			//��������
			ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
			IF_ERR_THEN_RETURN;

			//���̶�ȡ��ǰ������
			ret = CalibraReadPulseWidth(&PulseWidth);
			IF_ERR_THEN_RETURN;
			
			//���̳ɹ��������ĵ��Ƶ����ĵ��Ҳ�
			if (PulseWidth >PulseWidthCenter)
			{
				GainCalibraFlag = TRUE;
				break;
			}
		}
	}
	else
	{
		//����ֵ�������
		ChargeCap = CHARGE_CAP_MAX;
		ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
		IF_ERR_THEN_RETURN;
		ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
		IF_ERR_THEN_RETURN;

		//���̶�ȡ��ǰ������
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		
		//���̳ɹ���־��ʼ��
		GainCalibraFlag = TRUE;
		
		//�ж��Ƿ�����̫�����̫��lpcd_gain�Ŵ�
		if (PulseWidth > PulseWidthCenter)
		{
			//������Ҫ����
			GainCalibraFlag = FALSE;
			while(1)
			{
				//�����ǰ�Ѿ���������棬��һ�ַ�ʽ�����书��
				if (LpcdGainAmplify == 0x7)
				{
					//����Ŵ�������棬��Ȼ���������������书��
					if ((LpcdCwp == 0x07) || (LPCD_CALIBRA_REDUCE_PMOS == 0)) 
					{	
						//������������Ѿ���󣬵���ʧ��	
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
				else//�����Ŵ�
				{
					LpcdGainAmplify++;  
				}
				//��������
				ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
				IF_ERR_THEN_RETURN;
				
				

				//���̶�ȡ��ǰ������
				ret = CalibraReadPulseWidth(&PulseWidth);
				IF_ERR_THEN_RETURN;
				
				ret = GetReg_Ext(BFL_JREG_LPCDCTRL4,&RegData);
				IF_ERR_THEN_RETURN;
				
				//���̳ɹ��������ĵ��Ƶ����ĵ����
				if (PulseWidth < PulseWidthCenter)
				{
					GainCalibraFlag = TRUE;
					break;
				}
			}

		}
	}
	//����������ʧ�ܣ���ʧ��
	if (GainCalibraFlag == FALSE)
	{		
		(*CalibraFlag) = FALSE;
		return PulseWidth;	   //����ʧ�ܷ�������
	}
	//ɨ�����ֵ���ҵ����ʵĴ�Խ���ĵ������
	(*CalibraFlag) = FALSE;
	//����������
	PulseWidth_Pre = PulseWidth;
	//ɨ�������
	for(ChargeCap = CHARGE_CAP_MIN;ChargeCap < CHARGE_CAP_MAX;ChargeCap++)
	{
		//���õ���ֵ
		ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
		IF_ERR_THEN_RETURN;
		ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
		IF_ERR_THEN_RETURN;

		//����������
		PulseWidth_Pre = PulseWidth;
		//���̶�ȡ��ǰ������
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		//==============================================
		//�㷨һ
		//==============================================
		//�������ʼ������ֵС����Ϊ��ʼ������ֵ��
		if (PulseWidth < PulseWidthCenter)
		{
			//���̳ɹ�
			(*CalibraFlag) = TRUE;
			//��ǰһ�����̵��ݣ��ж��ĸ������ȸ��ӽ����ĵ�
			if((PulseWidthCenter-PulseWidth) < (PulseWidth_Pre-PulseWidthCenter))
			{
				//ֱ���õ�ǰֵ��Ϊ���ĵ�
				PulseWidthCenter = PulseWidth;
			}
			else
			{
				//ֱ���õ�ǰֵ��Ϊ���ĵ�
				PulseWidthCenter = PulseWidth_Pre;
				//�����ݲ���֮ǰ�ĳ�����
				ChargeCap--;
				//�������õ���ֵ
				ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
				IF_ERR_THEN_RETURN;
				ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
				IF_ERR_THEN_RETURN;

			}
			//΢����ֵ
			PulseThreshold_H = PulseWidthCenter + PulseWidthThreshold;
			PulseThreshold_L= PulseWidthCenter - PulseWidthThreshold;
			//-----------------------------------------------------------------
			//PulseThreshold_L1�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
			IF_ERR_THEN_RETURN;
			//-----------------------------------------------------------------
			//PulseThreshold_L2�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L2,(PulseThreshold_L>>6));
			IF_ERR_THEN_RETURN;
			//-----------------------------------------------------------------
			//PulseThreshold_H1�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H1,(PulseThreshold_H& 0x3F));
			IF_ERR_THEN_RETURN;
			//-----------------------------------------------------------------
			//PulseThreshold_H2�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H2,(PulseThreshold_H>>6));
			IF_ERR_THEN_RETURN;

			break;
		}
	}
	if (GainCalibraFlag == FALSE)
	{		
		(*CalibraFlag) = FALSE;
		//V02 ���� ���̽����ر�CalibVmidEn
		ret = ModifyReg_Ext(BFL_JREG_MISC,BFL_JBIT_CALIB_VMID_EN,0);
		IF_ERR_THEN_RETURN;
		return PulseWidth;	   //����ʧ�ܷ�������
	}
	
	//V02 ���� ���̽����ر�CalibVmidEn
	ret = ModifyReg_Ext(BFL_JREG_MISC,BFL_JBIT_CALIB_VMID_EN,0);
	IF_ERR_THEN_RETURN;
	return TRUE;
}

//***********************************************
//�������ƣ�LpcdPeriodCalibra()
//�������ܣ���ʱ���̣���Ҫͨ�����ڵ��ݣ��������ĵ�
//��ڲ�����uchar *CalibraFlag ���̱�־���Ƿ���Ҫ����
//���ڲ�����uchar  TRUE���Ĵ�����д�ɹ�   FALSE:�Ĵ�����дʧ��
//***********************************************
/*
unsigned char  LpcdPeriodCalibra(unsigned char * CalibraFlag)
{
	unsigned char ret;
	// unsigned char LpcdPeriodCalibraResult;	//LPCD���̳ɹ�
	unsigned char PulseWidth;			//LPCD������

	//���̶�ȡ��ǰ������
	ret = CalibraReadPulseWidth(&PulseWidth);
	IF_ERR_THEN_RETURN;
	//�����������ĵ㸽������������̣��˳�
	if ((PulseWidth < (PulseWidthCenter + PULSE_CENTER_RANGE))  
		| (PulseWidth > (PulseWidthCenter - PULSE_CENTER_RANGE))) 
	{
			(*CalibraFlag) = TRUE;
			return True;
	}
	//ɨ�����ֵ���ҵ����ʵĴ�Խ���ĵ������
	(*CalibraFlag) = FALSE;
	for(ChargeCap = CHARGE_CAP_MIN;ChargeCap < CHARGE_CAP_MAX;ChargeCap++)
	{
		//���õ���ֵ
		#if  (NB1117_FPGA == 1)
		ret = NB1117V02_ChargeConfig(LpcdGainReduce,ChargeCap,ChargeCurrent);
		IF_ERR_THEN_RETURN;
		#else
		ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
		IF_ERR_THEN_RETURN;
		ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
		IF_ERR_THEN_RETURN;
		#endif
		//���̶�ȡ��ǰ������
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		//�����������ĵ㸽�����Խ�������
		if ((PulseWidth < (PulseWidthCenter + PULSE_CENTER_RANGE))  
			| (PulseWidth > (PulseWidthCenter - PULSE_CENTER_RANGE))) 
		{
			(*CalibraFlag) = TRUE;
			//΢�����ĵ�
			if (PulseWidth < (PulseWidthCenter))
			{
				PulseWidthCenter =  PulseWidthCenter + (PulseWidthCenter - PulseWidth);
			}
			else
			{
				PulseWidthCenter =  PulseWidthCenter - (PulseWidth - PulseWidthCenter);
			}
			//΢����ֵ
			PulseThreshold_H = PulseWidthCenter + PulseWidthThreshold;
			PulseThreshold_L= PulseWidthCenter - PulseWidthThreshold;
			//-----------------------------------------------------------------
			//PulseThreshold_L1�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
			IF_ERR_THEN_RETURN;
			//-----------------------------------------------------------------
			//PulseThreshold_L2�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L2,(PulseThreshold_L>>6));
			IF_ERR_THEN_RETURN;
			//-----------------------------------------------------------------
			//PulseThreshold_H1�Ĵ���
			//-----------------------------------------------------------------
			ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H1,(PulseThreshold_H& 0x3F));
			IF_ERR_THEN_RETURN;
			//-----------------------------------------------------------------
			//PulseThreshold_H2�Ĵ���
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
//�������ƣ�LpcdErrorTrigCalibra()
//�������ܣ��󴥷�����,requestʧ��֮����ã��ж��Ƿ���в���΢��
//��ڲ�����uchar *CalibraFlag ���̱�־���Ƿ���Ҫ����
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
/*
unsigned char  LpcdErrorTrigCalibra(unsigned char * CalibraFlag)
{	
	//--------------------------------------------------------
	//����card_in_irq�ж�֮��Ͷ�ȡ��plusewidth�������LpcdPulseWidth[0]
	//���request֮��Ѱ��ʧ�ܺ�����󴥷�����
	//--------------------------------------------------------
	unsigned char ret;
	unsigned char PulseWidth;
	unsigned char i;
	//��ʼ����LPCDģʽ
	ret = LpcdRegisterInit();
	IF_ERR_THEN_RETURN;
	//�ɼ�1S��������
	for (i=1;i<10;i++)
	{
		//���̶�ȡ��ǰ������
		ret = CalibraReadPulseWidth(&PulseWidth);
		IF_ERR_THEN_RETURN;
		//����������
		LpcdPulseWidth[i] = PulseWidth;

		//��ʱ100ms
		mDelay(100);	
	}
	*CalibraFlag = FALSE;
	//���Ը����㷨��LpcdPulseWidth������з������ж��Ƿ������������¶�Ư��
	//Ŀǰ�ü��㷨�ж�
	if (LpcdPulseWidth[9] > LpcdPulseWidth[0]) 
	{
		if ((LpcdPulseWidth[9] - LpcdPulseWidth[0]) < ERR_TRIG_JUDGE_RANGE)
		{
			//�������࿨���߽������������������������Ư�󴥷�����Ҫ����
			*CalibraFlag = TRUE;
		}
	}
	else
	{
		if ((LpcdPulseWidth[0] - LpcdPulseWidth[9]) < ERR_TRIG_JUDGE_RANGE)
		{
			//�������࿨���߽������������������������Ư�󴥷�����Ҫ����
			*CalibraFlag = TRUE;
		}
	}
	//��������������Ư�󴥷�����Ҫ����
	if (*CalibraFlag)
	{
		//������������������
		if(LpcdPulseWidth[9] > PulseThreshold_H)
		{
			PulseThreshold_H = PulseThreshold_H + (LpcdPulseWidth[9] - PulseThreshold_H);
		}
		//������������������
		else if(LpcdPulseWidth[9] < PulseThreshold_L)
		{
			PulseThreshold_L= PulseThreshold_L + (PulseThreshold_L - LpcdPulseWidth[9]);
		}
		//-----------------------------------------------------------------
		//PulseThreshold_L1�Ĵ���
		//-----------------------------------------------------------------
		ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
		IF_ERR_THEN_RETURN;
		//-----------------------------------------------------------------
		//PulseThreshold_L2�Ĵ���
		//-----------------------------------------------------------------
		ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L2,(PulseThreshold_L>>6));
		IF_ERR_THEN_RETURN;
		//-----------------------------------------------------------------
		//PulseThreshold_H1�Ĵ���
		//-----------------------------------------------------------------
		ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H1,(PulseThreshold_H& 0x3F));
		IF_ERR_THEN_RETURN;
		//-----------------------------------------------------------------
		//PulseThreshold_H2�Ĵ���
		//-----------------------------------------------------------------
		ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H2,(PulseThreshold_H>>6));
		IF_ERR_THEN_RETURN;
	}	
	return TRUE;	
	
}
*/
//***********************************************
//�������ƣ�LpOscCalibra()
//�������ܣ��͹��Ļ����ʼ��
//��ڲ�����
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
/*
unsigned char  LpOscCalibra(void)
{
	unsigned char ret;
	
	unsigned char ExtRegData;
	unsigned char LpOscCount;
	//��λLPCD��ؼĴ���
	//ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	//IF_ERR_THEN_RETURN;
	//ʹ�ܻ���ʹ�ܵ���
	ret = SetReg_Ext(BFL_JREG_LPCD_TEST1,BFL_JBIT_LP_OSC10K_EN+BFL_JBIT_LP_OSC10K_EN);
	IF_ERR_THEN_RETURN;
	//�жϵ���ʱ�����
	ret = WaitForLpcdIrq(BFL_JBIT_LP10K_TESTOK_IRQ);
	IF_ERR_THEN_RETURN;
	//��ȡLP����ֵ
	ret = GetReg_Ext(BFL_JREG_LP_CLK_CNT2, &ExtRegData);
	LpOscCount = (ExtRegData << 6);
	ret = GetReg_Ext(BFL_JREG_LP_CLK_CNT1, &ExtRegData);
	LpOscCount += ExtRegData;

	//����LpOscCount���е��̼��� Timer1Cfg
	//�������ߵ�ƽ��ȹ�����ζ��Ƶ�ʹ�����Timer1Cfg��Ҫ��С
	if ((LpOscCount - LP10K_CNT_DEFAULT) > LP10K_CALIBRA_THRESHOLD)
	{
		 Timer1Cfg--;
	}
	//�������ߵ�ƽ��ȹ�խ����ζ��Ƶ�ʹ��죬Timer1Cfg��Ҫ����
	else if ((LP10K_CNT_DEFAULT - LpOscCount) > LP10K_CALIBRA_THRESHOLD)
	{
		 Timer1Cfg++;
	}
	return TRUE;	
}
*/
//***********************************************
//�������ƣ�WaitForLpcdIrq()
//�������ܣ��ȴ�LPCD�ж�
//��ڲ�����IrqType
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
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
		//��ʱ�˳�
		mDelay(1);	//��ʱ10ms
		TimeOutCount++;
		if  (TimeOutCount > IRQ_TIMEOUT)  
		{
			return FALSE; //150ms ��ʱ�˳�
		}
	}
	return TRUE; 
	
}

//***********************************************
//�������ƣ�LpcdCardIn_IRQHandler()
//�������ܣ���Ƭ�����������
//��ڲ�����
//���ڲ�����
//�������ͣ�private
//***********************************************
unsigned char LpcdReset(void)
{
	unsigned char ret;
	//��λLPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��λ�ſ�LPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��Ϊÿ�λ��ѣ����ԼĴ����ᱻ��λ
	ret = LpcdAuxSelect(ON & LPCD_AUX_EN);					//����AUX�۲�ͨ��
	IF_ERR_THEN_RETURN;
	
	return TRUE;
}

//***********************************************
//�������ƣ�ReadLpcdPulseWidth()
//�������ܣ���ȡLPCD������
//��ڲ�����uchar *PulseWidth
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//�������ͣ�public
//***********************************************
unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth)
{
	unsigned char ExtRegData;
	unsigned char ret;
	unsigned char PulseTemp;

	//��ȡ�����Ϣ
	*PulseWidth = 0;
	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH2,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp = ((ExtRegData & 0x3) << 6);

	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH1,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp += (ExtRegData & 0x3F);
    *PulseWidth = PulseTemp;

	//��λLPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��λ�ſ�LPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;

	return TRUE;
}
//***********************************************
//�������ƣ�ReadLpcdPulseWidth()
//�������ܣ� ��ȡLPCD������
//��ڲ�����uchar *PulseWidth
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//�������ͣ�private
//***********************************************
/*
unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth)
{
	unsigned char ExtRegData;
	unsigned char ret;
	unsigned char PulseTemp;

	//��ȡ�����Ϣ
	*PulseWidth = 0;
	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH2,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp = ((ExtRegData & 0x3) << 6);

	ret = GetReg_Ext(BFL_JREG_PULSEWIDTH1,&ExtRegData);
	IF_ERR_THEN_RETURN;
	PulseTemp += (ExtRegData & 0x3F);
    *PulseWidth = PulseTemp;

	//��λLPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��λ�ſ�LPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;

	return TRUE;
}
*/


//***********************************************
//�������ƣ�CalibraReadPulseWidth()
//�������ܣ� ���̲���ȡLPCD������
//��ڲ�����uchar *PulseWidth
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//�������ͣ�private
//***********************************************
unsigned char CalibraReadPulseWidth(unsigned char *PulseWidth)
{
	//ʹ�ܵ���ģʽ
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
	//�ȴ����̽����ж�
	ret = WaitForLpcdIrq(BFL_JBIT_CALIB_IRQ);
	//debug
	if (ret == 0)
	{
		ret =1;
	}
	IF_ERR_THEN_RETURN;
	//�رյ���ģʽ
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_CALIBRA_EN);
	IF_ERR_THEN_RETURN;
	//��ȡ������
	ret = ReadLpcdPulseWidth(PulseWidth);
	IF_ERR_THEN_RETURN;
	return TRUE;
}
//***********************************************
//�������ƣ�LpcdCardIn_IRQHandler()
//�������ܣ���Ƭ�����������
//��ڲ�����
//���ڲ�����
//�������ͣ�private
//***********************************************
/*
unsigned char LpcdReset(void)
{
	unsigned char ret;
	//��λLPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��λ�ſ�LPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��Ϊÿ�λ��ѣ����ԼĴ����ᱻ��λ
	ret = LpcdAuxSelect(ON & LPCD_AUX_EN);					//����AUX�۲�ͨ��
	IF_ERR_THEN_RETURN;
	
	return TRUE;
}
*/

//***********************************************
//�������ƣ�LpcdAuxSelect()
//�������ܣ�LPCD AUXͨ��ѡ��  aux1=vp_lpcd aux2=vdem_lpcd
//��ڲ�����uchar TRUE:���� FALSE ���ر�
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
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

	if ((OpenClose == TRUE) && (LPCD_AUX_EN))//��AUXͨ��,�����ļ���Ȩ�޹ر�
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
//�������ƣ�LpcdAutoTest()
//�������ܣ�LPCD �Զ����ԣ��ɼ�������������޿�״̬�¼�⵽��Ƭ
//��ڲ�����uchar 
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//�������ͣ�private
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
	PulseThreshold_H = 0;//��Ҫ��������ж�
	Timer1Cfg = 1;//300ms ��Сʱ����

	//-----------------------------------------------------------------
	
	//-----------------------------------------------------------------
	//�ж�����
	//-----------------------------------------------------------------
	regdata = COMMIEN_DEF;			 //�ж�����
	ret = SetReg(COMMIEN,regdata);
	IF_ERR_THEN_RETURN;

	regdata = DIVIEN_DEF;			 
	ret = SetReg(DIVIEN,regdata);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------

	//-----------------------------------------------------------------
	//LpcdCtr1�Ĵ���
	//-----------------------------------------------------------------
	//��λLPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//��λ�ſ�LPCD�Ĵ���
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	IF_ERR_THEN_RETURN;
	//ʹ��LPCD����
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_EN);
	IF_ERR_THEN_RETURN;
	//����LPCD�ж�
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,(LPCD_IE<<5)+BFL_JBIT_LPCD_IE);
	IF_ERR_THEN_RETURN;	
	//���ý���������
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL1,(LpcdDetectTimes<<5)+BFL_JBIT_LPCD_CMP_1);
	IF_ERR_THEN_RETURN;	
	//-----------------------------------------------------------------
	//LpcdCtrl2�Ĵ���
	//-----------------------------------------------------------------
	//ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,PulseClkDivK<<4+LPCD_TX2RFEN<<3+LPCD_CWN<<2+LPCD_CWP);
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LPCD_CWN<<3)+LPCD_CWP));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//LpcdCtrl3�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL3,LPCD_MODE<<3);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//LpcdCtrl4�Ĵ���  ��RegisterInit������ͬ
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
    IF_ERR_THEN_RETURN;
	
	//-----------------------------------------------------------------
	//ChargeCurrent�Ĵ���  ��RegisterInit������ͬ
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)<<5)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//ChargeCap�Ĵ���  ��RegisterInit������ͬ
	//-----------------------------------------------------------------	
	ret = SetReg_Ext(BFL_JREG_CHARGECAP,ChargeCap&0x3F);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer1Cfg�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER1CFG,(PulseClkDivK<<4)+Timer1Cfg);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//Timer2Cfg�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER2CFG,Timer2Cfg);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//Timer3Cfg�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER3CFG,Timer3Cfg);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//VmidBdCfg�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_VMIDBDCFG,VMID_BG_CFG);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//AutoWupCfg�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_AUTO_WUP_CFG,(LpcdAutoWakeUpEn<<3)+LpcdAutoWakeUpCFG);
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_L1�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L & 0x3F));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_L2�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L2,(PulseThreshold_L>>6));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_H1�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H1,(PulseThreshold_H& 0x3F));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_H2�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H2,(PulseThreshold_H>>6));
	IF_ERR_THEN_RETURN;
	return TRUE;
}
//***********************************************
//�������ƣ�LpcdLoadCalibraValue()
//�������ܣ�����ȱʡ���̲���
//��ڲ�����uchar *CalibraFlag ���̱�־���Ƿ���Ҫ����
//���ڲ�����uchar  TRUE�����̳ɹ�   FALSE:����ʧ��
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
	//��������
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL4,((LpcdGainAmplify << 2) + LpcdGainReduce));
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//���õ��ݺͳ�����
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_CHARGECURRENT,((ChargeCap&0x40)>>1)+ChargeCurrent&0x7);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer1Cfg����
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER1CFG,(PulseClkDivK<<4)+Timer1Cfg);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer2Cfg����
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER2CFG,Timer2Cfg);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//Timer3Cfg����
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_TIMER3CFG,Timer3Cfg);
	IF_ERR_THEN_RETURN;

	//-----------------------------------------------------------------
	//PulseThreshold_L1�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L1,(PulseThreshold_L& 0x3F));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_L2�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_L2,(PulseThreshold_L>>6));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_H1�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H1,(PulseThreshold_H& 0x3F));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//PulseThreshold_H2�Ĵ���
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_PULSE_THRESHOLD_H2,(PulseThreshold_H>>6));
	IF_ERR_THEN_RETURN;

	return TRUE;
}
*/
