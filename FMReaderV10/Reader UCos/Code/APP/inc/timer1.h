#ifndef _TIM1_
#define _TIM1_

extern void LEDShow(uint8_t Index, BitAction BitVal);
extern bool bTimeOut;

extern void TIM1Config(uint32_t Counter);
extern void TIM1Start(void);
extern void TIM1Stop(void);
extern void TIM1InterruptHandler(void);

#endif
