#ifndef LPCD_API_PRIVATE_H
#define LPCD_API_PRIVATE_H


extern unsigned char LpcdInitCalibra(unsigned char *CalibraFlag);
extern uchar LpcdAuxSelect(uchar OpenClose);
extern uchar LpcdAutoTest(void);
extern unsigned char LpcdReset(void);
extern unsigned char ReadLpcdPulseWidth(unsigned char *PulseWidth);
#endif


