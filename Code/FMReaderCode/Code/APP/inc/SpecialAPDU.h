#ifndef _SPECIAL_APDU_H_
#define _SPECIAL_APDU_H_
 
#include "stdint.h"
#include "stm32f10x.h"
/* INS constants */
#define INS_SELECT_DEVICE 0x01


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
#define INS_SEND_PPSA    0xFF

/* bCurrentDevice constants */
#define CURRENT_DEVICE_TDA8035  0x01
#define CURRENT_DEVICE_FM320   0x02
/* bATRSource constants */
#define ATR_SOURCE_CARD         0x01
#define ATR_SOURCE_VIRTUAL      0x02

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


#define CARD_VCC_5V   1
#define CARD_VCC_3V   2
#define CARD_VCC_18V  3
#define CARD_VCC_WARM 4

typedef struct
{
  uint8_t bCurrentDevice;
  uint8_t bATRSource;
  uint8_t bVoltage;         // Voltage supplied to contact card.
  uint8_t bCardPresent;
  uint8_t	bIsPoweredOnCT;	// CT Card is powered on
} InternalRegister;  

extern InternalRegister stuInRegs;

extern uint8_t abIntBuffer[1024*4];    // Internal Buffer.
extern bool bIsCTon;
void InRegsInit(void);
void PWMOutConfig(uint32_t ClkFrq);
void TIM3Start(void);
void SpecialAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
void TIM3InterruptHandler(void);
extern uint8_t InitCmd04Flag;
extern uint8_t InitCmd00Flag;
extern uint8_t InitCmdFlag;//0:APDU,1:Init
#endif
