#ifndef _CONTACTLESS_CARD_H_
#define _CONTACTLESS_CARD_H_
#include "stm32f10x.h"
typedef struct
{
  uint8_t bNADEn;
  uint8_t bCIDEn;
  uint8_t bINFLen;    // Max information field Length in one block.
  uint8_t bBlockNum;  // Block number
  uint8_t bChain;     // Chain
} TCLParam;

/* Function Result */
#define CL_TCL_OK   0
#define CL_TCL_ATQ_ERROR  0x01
#define CL_TCL_UID_ERROR  0x02
#define CL_TCL_ATS_ERROR  0x03
#define CL_TCL_PPS_ERROR  0x04
#define CL_TCL_APDU_ERROR 0x05
#define CL_TCL_RFIC_ERROR   0x06
#define FALSE 0
#define TRUE (!FALSE)

#define BLOCK_TYPE_I     0x10
#define BLOCK_TYPE_RACK  0x21
#define BLOCK_TYPE_RNAK  0x22
#define BLOCK_TYPE_SWTX  0x31
#define ERROR_COUNTER_MAX 0x03
extern TCLParam stuTCLParam;

void CLCardInit(void);
uint8_t CLCardPPS(uint8_t PPS1);
uint8_t CLTCLAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
uint8_t ContactlessCardInitCmd(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
void CLCardPowerOff(void);


#endif
