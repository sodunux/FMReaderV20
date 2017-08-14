#ifndef _CONTACTLESS_CARD_H_
#define _CONTACTLESS_CARD_H_
#include "stm32f10x.h"
typedef struct
{
  uint8_t abATQ[2];
  uint8_t bATQLen;
  uint8_t bUIDLevel;
  uint8_t abUID[12];
  uint8_t bSak;

  uint8_t abATS[32];  // Maybe 256-2 bytes max.
  uint16_t iATSLen;
  uint8_t bFSCI;
  uint8_t bDRC;
  uint8_t bDSC;
  uint8_t bFWI;
  uint8_t bSFGI;   
  uint8_t bNADEn;
  uint8_t bCIDEn;
  uint32_t  lFWT;
  uint16_t iREQBLen;
  
  uint8_t bINFLen;    // Max information field Length in one block.
  uint8_t bBlockNum;  // Block number
  uint8_t bChain;     // Chain
} TCLParam;
extern TCLParam stuTCLParam;

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

uint8_t CLCardReset(uint8_t *APDUBuffer, uint16_t *APDURecvLen);
uint8_t CLCardPPS(uint8_t PPS1);
uint8_t CLTCLAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
uint8_t ContactlessCardInitCmd(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
void CLCardPowerOff(void);
void CLCardInit(void);
uint8_t CLCardRegInit(void);

#endif
