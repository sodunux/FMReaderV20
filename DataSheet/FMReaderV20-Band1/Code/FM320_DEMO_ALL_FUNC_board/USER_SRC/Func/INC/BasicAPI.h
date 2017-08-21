/*******主要包含以下函数******/
//FM175xx的基本操作，如寄存器复位，SPD,读写FIFO等
#ifndef _BasicAPI_H
#define _BasicAPI_H

#include "Global.h"

typedef struct
{
	u8  Cmd;                 				// 命令代码
	u8  nBytesToSend;        				// 将要发送的字节数
	u8  nBytesReceived;      				// 已接收的字节数
	u8  nBitsReceived;       				// 已接收的位数
  u8  WaitForComm;
  u8  WaitForDiv;
  u8  AllCommIrq;
	u8  AllDivIrq;
	u8  Status;
	u8  *pExBuf;							// 交互数据缓冲区	
	u8  collPos;             				// 碰撞位置
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
