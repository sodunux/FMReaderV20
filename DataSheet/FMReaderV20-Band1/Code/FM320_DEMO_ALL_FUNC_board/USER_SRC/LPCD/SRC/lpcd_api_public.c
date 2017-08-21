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
/* 	��Ҫ����:           											 */
/* 		1.ʵ��LPCD���API����										 */
/* 	����:��ͦ�� 													 */
/* 	����ʱ��:2014��6��   											 */
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
//�ڲ������б�
//********************************************************************
unsigned char LpcdParamInit(void);
unsigned char LpcdRegisterInit(void);

//********************************************************************
//�ⲿ�����б�
//********************************************************************
extern unsigned char LpcdReset(void);
extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
extern unsigned char LpcdInitCalibra(unsigned char *CalibraFlag);
extern unsigned char LpcdAuxSelect(unsigned char OpenClose);

//********************************************************************
//�ڲ������б�
//********************************************************************
unsigned char PulseClkDivK ;
unsigned char Timer1Cfg;			//4bit,�����ʱ��
unsigned char Timer2Cfg;			//5bit,���׼��ʱ��
unsigned char Timer3Cfg;			//5bit,������ʱ��

unsigned char PulseWidthFullScale;	//T3������PulseWidth���
unsigned char PulseWidthThreshold;	//����ȣ����ó����ֵ
unsigned char PulseThreshold_L;		//LPCD�������ֵ
unsigned char PulseThreshold_H;		//LPCD�������ֵ
unsigned char PulseWidthCenter;		//LPCD�������ĵ�

unsigned char LpcdDetectTimes;		//������;
unsigned char LpcdThresholdRatio;	//���������
unsigned char LpcdAutoWakeUpEn;		//�Զ�����ʹ��
unsigned char LpcdAutoWakeUpCFG;	//�Զ�����ʱ������

//***********************************************
//�������ƣ�NRSTPD_CTRL()
//�������ܣ�NRSTPD���ƺ���
//��ڲ�����
//���ڲ�����
//***********************************************
void NRSTPD_CTRL(unsigned char ctrltype)
{
	//====================================================
	//���²����û�����NRSTPD��MCU�������ӹ�ϵ�޸�
	//====================================================
	if(ctrltype == 0x0)
		GPIO_ResetBits(GNRST,NRST);
	else
		GPIO_SetBits(GNRST,NRST);	
}

//***********************************************
//�������ƣ�LpcdParamInit()
//�������ܣ�LPCD������ʼ��
//��ڲ�����
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//�������ͣ�public
//***********************************************
unsigned char LpcdParamInit()
{
	unsigned char LpcdCfgBug[20];
	unsigned char *ptrBuf;
	
	Flash_Read_Lpcd_Cfg(LpcdCfgBug);								//��Flash���ȡ������Ϣ��
	ptrBuf = LpcdCfgBug;
	if((*(ptrBuf++) == 0x55)&&(*(ptrBuf++) == 0xAA))				//Flash����������Ϣ
	{
		Timer1Cfg = *(ptrBuf++);
		if(15 < Timer1Cfg)											//������Timer����������;
			Timer1Cfg = TIMER1_CFG;
			
		Timer2Cfg = TIMER2_CFG;
		
		Timer3Cfg = *(ptrBuf++);
		if(31 < Timer3Cfg)											//������Timer����������;
			Timer3Cfg = TIMER3_CFG;
			
		LpcdDetectTimes = *(ptrBuf++);
		if((0 != LpcdDetectTimes) && (1 != LpcdDetectTimes))		//flash�쳣
			LpcdDetectTimes = LPCD_AUTO_DETECT_TIMES;
		
		LpcdThresholdRatio = *(ptrBuf++);
		if((2 > LpcdThresholdRatio) || (5 < LpcdThresholdRatio))	//flash�쳣
			LpcdThresholdRatio = LPCD_THRESHOLD_RATIO;
			
		LpcdAutoWakeUpEn = *(ptrBuf++);
			
		LpcdAutoWakeUpCFG = *(ptrBuf++);
		if(7 < LpcdAutoWakeUpCFG)									//flash�쳣
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
	
	if (Timer3Cfg > 0xF)			//Timer3Cfg�õ�5bit������ֻ��ѡ��16��Ƶ
	{
		PulseClkDivK = 2;			//16��Ƶ
		PulseWidthFullScale =  ((Timer3Cfg - 1)<<3);
		PulseWidthCenter = (PulseWidthFullScale >>1);
		PulseWidthThreshold = (PulseWidthFullScale >> LpcdThresholdRatio);
	}
	else if(Timer3Cfg > 0x7)		//Timer3Cfg�õ�4bit������ֻ��ѡ��8��Ƶ
	{
		PulseClkDivK = 1;			//8��Ƶ
		PulseWidthFullScale =  ((Timer3Cfg - 1)<<4);
		PulseWidthCenter = (PulseWidthFullScale >>1);
		PulseWidthThreshold = (PulseWidthFullScale >> LpcdThresholdRatio);
	}
	else 
	{
		PulseClkDivK = 0;			//4��Ƶ
		PulseWidthFullScale =  ((Timer3Cfg - 1)<<5);
		PulseWidthCenter = (PulseWidthFullScale >>1);
		PulseWidthThreshold = (PulseWidthFullScale >> LpcdThresholdRatio);
	}

	PulseThreshold_H = PulseWidthCenter + PulseWidthThreshold;
	PulseThreshold_L= PulseWidthCenter - PulseWidthThreshold;

	return TRUE;
}
//***********************************************
//�������ƣ�LpcdRegisterInit()
//�������ܣ�LPCD�Ĵ�����ʼ��
//��ڲ�����
//���ڲ�����uchar  TRUE����ʼ���ɹ�   FALSE:��ʼ��ʧ��
//�������ͣ�public
//***********************************************
unsigned char LpcdRegisterInit(void)
{
	unsigned char ret;
	unsigned char regdata;
	
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
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL2,((LPCD_TX2RFEN<<4)+(LPCD_CWN<<3)+LPCD_CWP));
	IF_ERR_THEN_RETURN;
	//-----------------------------------------------------------------
	//LpcdCtrl3�Ĵ��� ��λֵ����000���üĴ�������
	//-----------------------------------------------------------------
	ret = SetReg_Ext(BFL_JREG_LPCDCTRL3,LPCD_MODE<<3);
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
	
	//-----------------------------------------------------------------
	//Auto_Wup_Cfg�Ĵ���
	//-----------------------------------------------------------------
	ret=SetReg_Ext(BFL_JREG_AUTO_WUP_CFG,((LpcdAutoWakeUpEn<<3) + LpcdAutoWakeUpCFG ));
	IF_ERR_THEN_RETURN;
		
	return TRUE;
}
//***********************************************
//�������ƣ�LpcdInitialize()
//�������ܣ�LPCD��ʼ������
//��ڲ�������
//���ڲ��������̳ɹ���־
//�������ͣ�public
//***********************************************
unsigned char LpcdInitialize(void)
{
	unsigned char CalibraFlag;
	LpcdParamInit();					//LPCD������ʼ��
	LpcdRegisterInit();       			//LPCD�Ĵ�����ʼ��
	LpcdAuxSelect(ON);					//����AUX�۲�ͨ��
	LpcdInitCalibra(&CalibraFlag);		//LPCD��ʼ������
	return CalibraFlag;
}
//***********************************************
//�������ƣ�LpcdCardIn_IRQHandler()
//�������ܣ���Ƭ�����������
//��ڲ�����
//���ڲ�����
//�������ͣ�public
//***********************************************
void LpcdCardIn_IRQHandler(void)
{
	unsigned char PulseWidth;
	unsigned char lpcd_irq;
	unsigned char ret;
	unsigned char Card_In = False;
	//-----------------------------------------------------------------
	//��ȡ�жϱ�־
	//-----------------------------------------------------------------
	ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
	//if (lpcd_irq != 0x01)
	//{
	//	lpcd_irq = 0x01;
	//}
	
	//-----------------------------------------------------------------
	//��ȡ�����Ϣ
	//-----------------------------------------------------------------
	ReadLpcdPulseWidth(&PulseWidth);
	//��λLPCD
	ret = LpcdReset();
	
	//LTS 20140701
	ret = GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
	if (lpcd_irq != 0x00)
	{
		lpcd_irq = 0x00;
	}
	
	
	//��Ҫ��ʱ�������п������һ���Ĵ���д����
//LTS20140701	mDelay(100);
	
	//====================================================
	//���²����û����Ա�д��⵽��Ƭ�ĺ�������
	//====================================================
	//�û�����ʼ
	LED_ARM_WORKING_OFF;

	//if(LpcdFunc == 0)		//����LPCD����
	//{
		OLED_ShowNum(65,STING_SIZE*2,(PulseWidthCenter & 0xFF),2,STING_SIZE);
		OLED_ShowNum(79,STING_SIZE*2,(PulseThreshold_L & 0xFF),2,STING_SIZE);
		OLED_ShowNum(93,STING_SIZE*2,(PulseThreshold_H & 0xFF),2,STING_SIZE);
		OLED_ShowNum(107,STING_SIZE*2,(PulseWidth & 0xFF),2,STING_SIZE);
		OLED_Refresh_Gram();	
	//}
	if(LpcdFunc == 1) //����ʾ�������
	{	
		
		FM175xx_Initial_ReaderA();
		
		if(ReaderA_Request(RF_CMD_REQA))
		{
			if(ReaderA_AntiCol(0))					//һ�ط���ͻ
			{
				if(ReaderA_Select(0))
				{
					//LTS20140701��ʱ�޸�
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
	
	//�û��������
	//====================================================
	//LTS 20140701
	
	//FieldOff();
	//LpcdReset();
	//GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
	
	//mDelay(1000);
	
	NRSTPD_CTRL(0);								//��������LPCDģʽ
	mDelay(10);
	LED_ARM_WORKING_OFF;

}
//***********************************************
//�������ƣ�LpcdAutoWup_IRQHandler()
//�������ܣ��Զ����ѵ��̷������
//��ڲ�����
//���ڲ�����
//***********************************************
void LpcdAutoWup_IRQHandler(void)
{
 	/*
	unsigned char PulseWidth;
	LED2_ON;
	ReadLpcdPulseWidth(&PulseWidth);
	//��λLPCD�Ĵ���
	SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_CLR+BFL_JBIT_LPCD_RSTN);
	//��λ�ſ�LPCD�Ĵ���
	SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_BIT_CTRL_SET+BFL_JBIT_LPCD_RSTN);
	//��Ҫ��ʱ�������п������һ���Ĵ���д����
	//mDelay(100);
	DisplayBuffer[0]=(PulseWidthCenter>> 4) & 0x0F;
	DisplayBuffer[1]=PulseWidthCenter & 0x0F;;
	DisplayBuffer[2]=(PulseWidth>> 4) & 0x0F;
	DisplayBuffer[3]=PulseWidth & 0x0F;;
	PS7219_ShowBuf(DisplayBuffer);

	NRSTPD_CTRL(0);		//��������LPCDģʽ
	LED2_OFF;
	*/
}


