#ifndef _CCID_H_
#define _CCID_H_

/* PC to Reader Message */
#define PC_to_RDR_IccPowerOn      0x62
#define PC_to_RDR_IccPowerOff     0x63
#define PC_to_RDR_GetSlotStatus   0x65
#define PC_to_RDR_XfrBlock        0x6F
#define PC_to_RDR_GetParameters   0x6C
#define PC_to_RDR_ResetParameters 0x6D
#define PC_to_RDR_SetParameters   0x61
#define PC_to_RDR_Escape          0x6B
#define PC_to_RDR_IccClock        0x6E
#define PC_to_RDR_T0APDU          0x6A
#define PC_to_RDR_Secure          0x69
#define PC_to_RDR_Mechanical      0x71
#define PC_to_RDR_Abort           0x72
#define PC_to_RDR_SetDataRateAndClockFrequency  \
                                  0x73
/* Reader to PC Message */
#define RDR_to_PC_DataBlock       0x80
#define RDR_to_PC_SlotStatus      0x81
#define RDR_to_PC_Parameters      0x82
#define RDR_to_PC_Escape          0x83
#define RDR_to_PC_DataRateAndClockFrequecncy  \
                                  0x84

/* Action parsered from CCID package */
#define ACT_POWER_ON              0x10
#define ACT_POWER_ON_5V           0x11
#define ACT_POWER_ON_3V           0x12
#define ACT_POWER_ON_18           0x13
#define ACT_SET_PARAM             0x40
#define ACT_POWER_OFF             0x20
#define ACT_APDU_EXCHANGE         0x30

/* Return value of Check functions */
#define CCID_OK                   0x01
#define CCID_NOK                  0x00

/* RDR_to_PC_XXX */
  /* bStatus constants */
    /* bmCommandStatus */
#define CCID_COMMAND_STATUS_NO_ERROR  0x00
#define CCID_COMMAND_STATUS_FAIL      0x40
#define CCID_COMMAND_STATUS_TIMEOUT   0x80
    /* bmICCStatus */
#define CCID_ICC_STATUS_PRES_ACT      0x00
#define CCID_ICC_STATUS_PRES_INACT    0x01
#define CCID_ICC_STATUS_ABSENT        0x02
  /* bError */
#define CCID_ERROR_CMD_ABORT          (-1)
#define CCID_ERROR_ICC_MUTE           (-2)
#define CCID_ERROR_XFR_PARITY_ERROR   (-3)
#define CCID_ERROR_XFR_OVERRUN        (-4)
#define CCID_ERROR_HW_ERROR           (-5)
#define CCID_ERROR_BAD_ATR_TS         (-8)
#define CCID_ERROR_BAD_ATR_TCK        (-9)
#define CCID_ERROR_ICC_PROTOCOL_NOT_SUPPORTED   \
                                      (-10)
#define CCID_ERROR_ICC_CLASS_NOT_SUPPORTED      \
                                      (-11)     
#define CCID_ERROR_PROCEDURE_BYTE_CONFLICT      \
                                      (-12)     
#define CCID_ERROR_DEACTIVATED_PROTOCOL         \
                                      (-13)
#define CCID_ERROR_BUSY_WITH_AUTO_SEQUENCE      \
                                      (-14)
#define CCID_ERROR_PIN_TIMEOUT        (-16)
#define CCID_ERROR_PIN_CANCELLED      (-17)
#define CCID_ERROR_CMD_SLOT_BUSY     (-32)
#define CCID_ERROR_USER_DEFINED       (-64)

#define CCID_BUFFER_SIZE  271
#define APDU_BUFFER_SIZE  264
#define TPDU_BUFFER_SIZE  261

/* Constants of bReaderStatus */
#define RDS_IDLE    0x00
#define RDS_PARSER  0x10
#define RDS_ACTION  0x20
#define RDS_ORGNIZE 0x30
#define RDS_SENDING 0x31

/* Constants of bCardSlotStatus */
#define CCID_CARD_SLOT_ABSENT  0x00
#define CCID_CARD_SLOT_PRESENT 0x01

/* Card presnt contants */
  /* Normal Close */
#define CARD_PRESENT  0x04
#define CARD_ABSENT  0x00
  /* Normal Open */
// #define CARD_PRESENT  0x00
// #define CARD_ABSENT  0x04

extern uint8_t const CCID_READER_CLOCK_FREQUENCIES[4];
extern uint8_t const CCID_READER_DATA_RATES[16];
/* RDR_to_PC_XXX */
extern uint8_t  bStatus;            // Index = 7
extern uint8_t  bError;             // Index = 8

extern uint8_t   volatile bReaderStatus;
extern uint8_t   bCardSlotStatus;

extern uint8_t   abBuffer[CCID_BUFFER_SIZE];
extern uint16_t  iBufferRecvLen, iBufferRecvCnt;
extern uint16_t  iBufferSendLen, iBufferSendCnt;
extern uint8_t   abAPDUBuffer[APDU_BUFFER_SIZE];
extern uint16_t  iAPDUBufferSendLen, iAPDUBufferSendCnt;
extern uint16_t  iAPDUBufferRecvLen, iAPDUBufferRecvCnt;
extern uint8_t   abTPDUBuffer[TPDU_BUFFER_SIZE];
extern uint16_t  volatile iTPDUBufferRecvLen, iTPDUBufferRecvCnt, iTPDUBufferRecvCntPrev, iTPDUBufferRecvPos;
extern uint16_t  volatile iTPDUBufferSendLen, iTPDUBufferSendCnt;

extern uint8_t  bPowerSelect;       // Index = 7
extern uint8_t  bmFindexDindex;     // Index = 10


void CCIDInit(void);
uint8_t CCIDParser(uint8_t* CCIDBuffer, uint8_t* APDUBuffer, uint16_t *APDULength);
void CCIDOrgnize(uint8_t* APDUBuffer, uint16_t APDULength, uint8_t* CCIDBuffer, uint16_t *CCIDLength);
void CCIDCardSlotChange(void);
uint8_t CCIDCardSlotCheck(void);
#endif
