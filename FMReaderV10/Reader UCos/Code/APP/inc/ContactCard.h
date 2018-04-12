#ifndef _CONTACT_CARD_H_
#define _CONTACT_CARD_H_

/* bT0Status Constants */
#define T0_STATUS_RECV              0x00
#define T0_STATUS_SEND              0x01
#define T0_STATUS_RECV_ATR          0x10
#define T0_STATUS_SEND_PPS_REQUEST  0x21
#define T0_STATUS_RECV_PPS_RESPONSE 0x22
#define T0_STATUS_SEND_APDU_HEAD    0x31
#define T0_STATUS_RECV_APDU_INS_1ST     0x32
#define T0_STATUS_SEND_APDU_DATA_1ST    0x33
#define T0_STATUS_RECV_APDU_INS     0x34
#define T0_STATUS_SEND_APDU_BODY    0x35
#define T0_STATUS_RECV_APDU_DATA    0x36
#define T0_STATUS_RECV_APDU_SW1     0x42
#define T0_STATUS_RECV_APDU_SW      0x38
#define T0_STATUS_SEND_APDU_ONE_BYTE    0x39
#define T0_STATUS_RECV_APDU_ONE_BYTE    0x3A

#define T0_STATUS_RECV_BYTES    0x40
#define T0_STATUS_SEND_BYTES    0x41

/* Function Result */
#define CT_T0_OK          0x00
#define CT_T0_NO_TS       0x01
#define CT_T0_ATR_TIMEOUT 0x02
#define CT_T0_TS_ERROR    0x03
#define CT_T0_TCK_ERROR   0x04

#define CT_T0_PPS_TIMEOUT 0x11
#define CT_T0_PPS_ERROR   0x12

#define CT_T0_APDU_LEN_ERROR  0x21
#define CT_T0_APDU_TIMEOUT  0x22

extern uint8_t volatile bT0Status;      // Reader will receive bytes or send bytes.
extern uint8_t volatile bIsTimeout;     // TDA8007 Timer is overflow or not.

#ifdef TDA8007_DEBUG
extern uint8_t  bIntCnt;
extern uint8_t  abTemps[32];
#endif

#define INC_TPDU_CNT(CNT);  do{CNT++; \
                               if(CNT == TPDU_BUFFER_SIZE){CNT = 0;}  \
                              }while(0);
#define DEC_TPDU_CNT(CNT);  do{if(CNT == 0) CNT = (TPDU_BUFFER_SIZE-1); \
                               else CNT--; \
                              }while(0);

uint8_t ContactCardReset(uint8_t PowerVolt, uint8_t *APDUBuffer, uint16_t *APDURecvLen);
uint8_t ContactCardPPS(uint8_t PPS1);
uint8_t ContactCardT0APDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen);
void ContactCardPowerOff(void);
void ContactCardInterruptHandler(void);
#endif
