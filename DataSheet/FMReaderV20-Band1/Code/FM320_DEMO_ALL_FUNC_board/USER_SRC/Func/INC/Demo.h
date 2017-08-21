
/** @file FM175xx_demo.h
    @brief FM175xx_SPI DEMO����
    @version 0.10 
    
    @author tyw
*/
#ifndef DEMO_H
#define DEMO_H

#include "stm32f10x_type.h"

#define DEBUG_00A4	1
#define DEBUG_APDU	1

#define BUF_LENTH	1024

//#ifndef TRUE
//	#define TRUE				1	//��
//#endif

//#ifndef FALSE
//	#define FALSE				0	//��
//#endif

#define ON        		1
#define OFF		       	0

#define FUNC_RATS		0
#define FUNC_PPSS 		1
#define FUNC_I_BLOCK 	2
#define FUNC_R_BLOCK 	3
#define FUNC_S_BLOCK		4
#define FUNC_PDTP		5

//�ǽ�apdu demo������ض���
#define FM175xx_PDTP_FSDI		(FSDI-8)
#define FM175xx_APDU_FSDI		(FSDI-1-lcid)

#define APDU_PROC_DONE	0
#define APDU_PROC_WTX	1
#define APDU_PROC_DEMO	2

#define CLA_DEMO_PROC			0x20

#define INS_DEMO_PLUS			0x10
#define INS_DEMO_MINUS			0x11
#define INS_DEMO_MULTIPLY		0x12
#define INS_DEMO_DEVIDE			0x13
#define INS_DEMO_WRITE			0x30
#define INS_DEMO_READ			0x31


extern u8 STA_RF;		//��״̬

extern u8 sendBuf[300];
extern u32 sendLen;

extern u8 testBuf[300];
extern u32 testLen;

extern u8 RFdataBuf[BUF_LENTH];		//�ǽӴ������ݻ���
extern u32 RFdataLen;					//�ǽӴ����������ݳ���

extern u8 RFapduDemo	;

extern u8 CTdataBuf[BUF_LENTH];		//�Ӵ���(spi)���ݻ���
extern u32 CTdataLen;					//�Ӵ����������ݳ���
extern u32 CTdataIndex;					//�Ӵ����������ݳ���(�ѷ���)
extern u8 CTdataFlag;


extern u8 FlagFirstFrame;			//��Ƭ��֡��ʶ
extern u8 FlagErrIrq ;

extern u8 RATS;
extern u8 ATS[5];
extern u32 FSDI;		//-4֡����PCD
extern u8 CID;
extern u8 block_num;
extern u8 lcid;			//����cid����֡�ĳ���ƫ��
extern u8 fwt;						//ms��λ

u8 FM175xx_WriteFIFO(u8 *ibuf,u32 ilen);
u8 FM175xx_ReadFIFO(u32 ilen,u8 *rbuf);

u8 FM175xx_RFDataTx(u32 ilen,u8 *ibuf);
u32 FM175xx_RFDataRx(u8 *rbuf);

void FM175xx_Resp_WTX(void);

u8 FM175xx_APDU_Proc(u8 *rbuf, u32 rlen);

void FM175xx_DEMO_Proc(void);
void FM175xx_Card_APDU(void);

#endif


