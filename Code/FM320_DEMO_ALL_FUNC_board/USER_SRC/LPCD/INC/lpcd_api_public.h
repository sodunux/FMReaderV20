#ifndef LPCD_API_PUBLIC_H
#define LPCD_API_PUBLIC_H

extern unsigned char LpcdParamInit(void);
extern unsigned char LpcdRegisterInit(void);
extern unsigned char PulseWidthCenter;		//LPCD脉宽中心点
extern void LpcdCardIn_IRQHandler(void);
extern void LpcdAutoWup_IRQHandler(void);

extern unsigned char PulseClkDivK ;
extern unsigned char Timer1Cfg;//4bit
extern unsigned char Timer2Cfg;//5bit
extern unsigned char Timer3Cfg;//5bit

extern unsigned char PulseWidthFullScale;	//T3???ú·ùPulseWidth?í?è
extern unsigned char PulseWidthThreshold;	//?ì2a?í?è￡?éè??3é?à???μ
extern unsigned char PulseThreshold_L;		//LPCD???íμí?D?μ
extern unsigned char PulseThreshold_H;		//LPCD???í???D?μ
extern unsigned char PulseWidthCenter;		//LPCD???í?DD?μ?

extern unsigned char LpcdDetectTimes;		//检测次数;
extern unsigned char LpcdThresholdRatio;	//检测灵敏度
extern unsigned char LpcdAutoWakeUpEn;		//自动唤醒使能
extern unsigned char LpcdAutoWakeUpCFG;		//自动唤醒时间设置

extern void NRSTPD_CTRL(unsigned char ctrltype);

extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
extern unsigned char LpcdInitialize(void);

#endif


