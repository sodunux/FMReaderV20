#ifndef _TDA8007_H_
#define _TDA8007_H_

/* Register Addresses */
#define RegCardSelect           0
#define RegClockConfiguration   1
#define RegProgrammableDivider  2
#define RegUartConfiguration2   3
#define RegGuardTime            5
#define RegUartConfiguration1   6
#define RegPowerControl         7
#define RegTimeoutConfiguration 8
#define RegTimeout1             9
#define RegTimeout2             10
#define RegTimeout3             11
#define RegMixedStatus          12  // Read
#define RegFIFOControl          12  // Write
#define RegUartReceive          13  // Read
#define RegUartTransmit         13  // Write
#define RegUartStatus           14
#define RegHardwareStatus       15

/* Common Maro */
#define CARD_VCC_5V   1
#define CARD_VCC_3V   2
#define CARD_VCC_18V  3
#define CARD_VCC_WARM 4

#define CARD_CLK_EXT          0
#define CARD_CLK_EXT_DIV_2    1
#define CARD_CLK_EXT_DIV_4    2
#define CARD_CLK_EXT_DIV_8    3
#define CARD_CLK_INT_DIV_2    4

/* Basic Functions */
void TDA8007WriteReg(uint8_t Addr, uint8_t Val);
uint8_t TDA8007ReadReg(uint8_t Addr);

/* External Functions */
void TDA8007Init(void);
void TDA8007SetTmo(uint32_t TmoValue);
void TDA8007StartTmo(void);
void TDA8007StopTmo(void);
void TDA8007PowerOn(uint8_t CardVcc);
void TDA8007ResetPin(BitAction BitVal);
void TDA8007PowerOff(void);
void TDA8007PPS(uint8_t PPS1);
void TDA8007SwitchClock(uint8_t ClkConfig);
uint8_t TDA8007ReadFIFO(void);
void TDA8007WriteFIFO(uint8_t Value);

#ifdef TDA8007_DEBUG
void TDA8007ReadAllRegs(void);
#endif

extern uint8_t bTimeoutOpen;

#define BIT_INDEX_TR    3
#define BIT_INDEX_LCT   2
void TDA8007SetUCR1Bit(uint8_t BitIndex, uint8_t BitValue);

#endif
