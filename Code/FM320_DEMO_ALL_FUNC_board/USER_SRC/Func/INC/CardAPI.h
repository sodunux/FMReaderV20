/************************************************************/
/*文件名：CardAPI.h											*/
/*制  作：赵清泉    	20130715								*/
/*注  意：本文件包含FM175xx所有卡操作相关函数					*/
//返回值说明：0：正确执行；others：各种错误；
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
extern uchar LCID;			//卡片的逻辑地址

extern uchar FM175xx_Initial_Card(void);

extern uchar FM175xx_Card_Config(uchar* configbuf);
extern uchar FM175xx_Card_AutoColl(void);

extern uchar FM175xx_Card_Pro(void);
void FM17550_Send_ATQA(unsigned char *atqa);

#endif

