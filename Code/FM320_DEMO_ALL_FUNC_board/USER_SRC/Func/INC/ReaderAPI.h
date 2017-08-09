#ifndef _READERAPI_H_
#define _READERAPI_H_

#include "Global.h"
#include "RegCtrl.h"
#include "FMReg.h"

#define  SET		1
#define  CLR		0

#define  RF_CMD_REQA       0x26
#define  RF_CMD_WUPA	   0x52

#define	 RF_CMD_ANTICOL_1  0x93
#define	 RF_CMD_SELECT_1   0x93
#define	 RF_CMD_ANTICOL_2  0x95
#define	 RF_CMD_SELECT_2   0x95
#define	 RF_CMD_ANTICOL_3  0x97
#define	 RF_CMD_SELECT_3   0x97


#define  FM175XX_SUCCESS		0x00
#define  FM175XX_REG_ERR		0x50
#define  FM175XX_FIFO_ERR		0x51
#define  FM175XX_TRANS_ERR  	0x60
#define  FM175XX_RECV_TIMEOUT  	0x61

#define  FM175XX_FRAME_ERR		0x70
#define  FM175XX_BCC_ERR		0x71

struct TypeACardResponse
{
	uchar ATQA[2];
	uchar UID[12];
	uchar SAK;
};

extern const uchar RF_CMD_ANTICOL[3];
extern const uchar RF_CMD_SELECT[3];

#define  RF_DATA_BUF_LEN 255       //FIFO SIZE:255
extern uchar RF_Data_Len;
extern uchar RF_Data_Buf[RF_DATA_BUF_LEN];


extern struct TypeACardResponse CardA_Sel_Res;

extern uchar FM175xx_Initial_ReaderA(void);
extern uchar FM175xx_Initial_ReaderB(void);
extern uchar FM175xx_Initial_ReaderF(void);

extern uchar ReaderA_Request(uchar type);
extern uchar ReaderA_AntiCol(uchar size);
extern uchar ReaderA_Select(uchar size);
extern uchar ReaderA_Rats(uchar FSDI,uchar CCID);

extern uchar ReaderF_Polling(uchar tsn,uchar* pollingbufer);
extern uchar ReaderF_ReqResponse(uchar* fnfcid);
extern uchar Reader_Data_SendRecv(uchar* sendbuf,uchar sendlen,uchar* recvbuf,uchar* recvlen);

extern uchar GetKey(uchar* Key);

#endif

