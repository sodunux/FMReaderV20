/*******��Ҫ�������º���******/
//FM175xx�Ļ�����������Ĵ�����λ��SPD,��дFIFO��
#ifndef _BasicAPI_H
#define _BasicAPI_H

#include "Global.h"

typedef struct
{
	u8  Cmd;                 				// �������
	u8  nBytesToSend;        				// ��Ҫ���͵��ֽ���
	u8  nBytesReceived;      				// �ѽ��յ��ֽ���
	u8  nBitsReceived;       				// �ѽ��յ�λ��
  u8  WaitForComm;
  u8  WaitForDiv;
  u8  AllCommIrq;
	u8  AllDivIrq;
	u8  Status;
	u8  *pExBuf;							// �������ݻ�����	
	u8  collPos;             				// ��ײλ��
} NFC_DataExTypeDef;

extern uchar FM175xx_ResetAllReg(void);
extern uchar FM175xx_SPD(void);
extern uchar FM175xx_Initial(void);
extern uchar FM175xx_IRQ_Pro(uchar tag_com_irq,uchar tag_div_irq);

extern uchar Write_FIFO(uchar fflen,uchar* ffbuf);  

extern uchar Command_Transceive(NFC_DataExTypeDef* NFC_DataExStruct);

extern unsigned char LpcdEnBak;
void Quit_AUXSET(void);
void AUX2_SET(unsigned char AuxSel);
void AUX1_SET(unsigned char AuxSel);

#endif
