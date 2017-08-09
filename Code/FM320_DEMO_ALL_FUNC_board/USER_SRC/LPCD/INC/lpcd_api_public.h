#ifndef LPCD_API_PUBLIC_H
#define LPCD_API_PUBLIC_H

extern unsigned char LpcdParamInit(void);
extern unsigned char LpcdRegisterInit(void);
extern unsigned char PulseWidthCenter;		//LPCD�������ĵ�
extern void LpcdCardIn_IRQHandler(void);
extern void LpcdAutoWup_IRQHandler(void);

extern unsigned char PulseClkDivK ;
extern unsigned char Timer1Cfg;//4bit
extern unsigned char Timer2Cfg;//5bit
extern unsigned char Timer3Cfg;//5bit

extern unsigned char PulseWidthFullScale;	//T3???������PulseWidth?��?��
extern unsigned char PulseWidthThreshold;	//?��2a?��?����?����??3��?��???��
extern unsigned char PulseThreshold_L;		//LPCD???���̨�?D?��
extern unsigned char PulseThreshold_H;		//LPCD???��???D?��
extern unsigned char PulseWidthCenter;		//LPCD???��?DD?��?

extern unsigned char LpcdDetectTimes;		//������;
extern unsigned char LpcdThresholdRatio;	//���������
extern unsigned char LpcdAutoWakeUpEn;		//�Զ�����ʹ��
extern unsigned char LpcdAutoWakeUpCFG;		//�Զ�����ʱ������

extern void NRSTPD_CTRL(unsigned char ctrltype);

extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
extern unsigned char LpcdInitialize(void);

#endif


