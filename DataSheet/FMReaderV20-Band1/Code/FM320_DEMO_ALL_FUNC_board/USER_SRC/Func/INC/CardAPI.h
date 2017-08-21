/************************************************************/
/*�ļ�����CardAPI.h											*/
/*��  ��������Ȫ    	20130715								*/
/*ע  �⣺���ļ�����FM175xx���п�������غ���					*/
//����ֵ˵����0����ȷִ�У�others�����ִ���
/************************************************************/

#ifndef _CARDAPI_H
#define _CARDAPI_H

#include "Global.h"
#include "RegCtrl.h"
#include "ReaderAPI.h"

#define  CARD_DATA_BUF_LEN 255       //FIFO SIZE:255

extern uchar Card_Data_Len;
extern uchar Card_Data_Buf[CARD_DATA_BUF_LEN];

extern uchar Card_UID_Buf[25];
extern uchar LCID;			//��Ƭ���߼���ַ

extern uchar FM175xx_Initial_Card(void);

extern uchar FM175xx_Card_Config(uchar* configbuf);
extern uchar FM175xx_Card_AutoColl(void);

extern uchar FM175xx_Card_Pro(void);
void FM17550_Send_ATQA(unsigned char *atqa);

#endif

