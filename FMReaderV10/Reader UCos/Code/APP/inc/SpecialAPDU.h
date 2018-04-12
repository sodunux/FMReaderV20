#ifndef _SPECIAL_APDU_H_
#define _SPECIAL_APDU_H_
 
#include "stdint.h"

/* INS constants */
#define INS_SELECT_DEVICE 0x01
#define INS_SELECT_CLOCK  0x03
#define INS_TRIGGLE_LEVEL 0x05
#define INS_TRIGGLE_DELAY_AND_WIDTH 0x07
#define INS_DATA_TRANSMIT 0x09
#define INS_DATA_RECEIVE  0xC0
#define INS_ATR_SOURCE    0x0B
#define INS_FM_DEBUG_SIGNAL 0x0D
#define INS_DIRECT_READ_REGISTER  0x0F
#define INS_DIRECT_WRITE_REGISTER 0x11
#define INS_COUNTER_START   0x13
#define INS_COUNTER_STOP   0x15
#define INS_GET_TYPEA_PARAM 0x17
#define INS_CL_OPERATION 	0x18
#define INS_CT_OPERATION	0x19
#define INS_SPI_OPERATION	0x1A
#define INS_I2C_OPERATION	0x1B
#define INS_10BitI2C_OPERATION	0x1E

//add 24xx opration, zhangpei
#define INS_I2C_READ24XX	0x1C
#define INS_I2C_WRITE24XX	0x1D
//#define INS_I2C_READ_FM327	0x1E
//#define INS_I2C_WRITE_FM327	0x1F

#define INS_SEND_PPSA    0xFF

/* bCurrentDevice constants */
#define CURRENT_DEVICE_TDA8007  0x01
#define CURRENT_DEVICE_FM1715   0x02
/* bATRSource constants */
#define ATR_SOURCE_CARD         0x01
#define ATR_SOURCE_VIRTUAL      0x02

/* bTriggleStatus constants*/
#define TRIG_STATUS_DELAY       0x01
#define TRIG_STATUS_WIDTH       0x02 

/* bCounterValue constants */
#define COUNTER_ENABLE  0x01
#define COUNTER_ENABLE_ORDER  0x02
#define COUNTER_DISABLE  0x00

/* Timerout Status */
#define TMO_STANDARD  0x01
#define TMO_NONE_STANDARD  0x02
#define TMO_NO_LIMIT  0x03

/* Card Present */
#define SA_CARD_PRESENT  0x01
#define SA_CARD_ABSENT   0x00

typedef struct
{
  uint8_t bCurrentDevice;
  uint8_t bATRSource;
  uint8_t bTriggleStatus;   // TRIG_STATUS_DELAY or TRIG_STATUS_PULSE
  uint8_t bCounterStatus;
  uint32_t  lTriggleDelay;  // Unit is us
  uint32_t  lTriggleWidth;  // Unit is us
  uint16_t  iTimerCyc;      // How many 65536 counter.
  uint16_t  iTimerLast;     // Counter value less than 65536
  uint32_t  iIntBufferLen;
  uint32_t  iIntBufferOffset;
  uint32_t  lCounterValue;
  uint8_t bTmoStatus;       // Non-Standard timeout counter is needed.
  uint8_t bVoltage;         // Voltage supplied to contact card.
  uint8_t bCardPresent;
  uint8_t	bIsPoweredOnCT;	// CT Card is powered on
} InternalRegister;   
extern InternalRegister stuInRegs;

extern uint8_t abIntBuffer[1024*4];    // Internal Buffer.

void InRegsInit(void);
void PWMOutConfig(uint32_t ClkFrq);
void TIM3Start(void);
void SpecialAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
void TIM3InterruptHandler(void);
#endif
