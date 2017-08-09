/************************************************************/
/*文件名：RegCtrl.h											*/
/*制  作：赵清泉    20130715								*/
/*注  意：本文件包含FM175xx所有寄存器操作相关函数             */
//返回值说明：0(false)：错误；1(true)：正确；
/************************************************************/
#include "Global.h"
#include "define.h"

#define	UART_TIME_OUT		8000
#define	SPI_TIME_OUT		1000

#define REG_OPT_SUCCESS				0x00
#define REG_OPT_NO_RESPONSE			0x01
#define REG_OPT_ERROR_RESPONSE		0x02

#define REG_OPT_I2C_MCODE_ERROR		0x10
#define REG_OPT_I2C_SLV_ADDR_ERROR	0x11
#define REG_OPT_I2C_DATA_SEND_ERROR	0x12

#define REG_OPT_SPI_ERROR			0x13

extern   void  FM175xx_SetInterface(uchar intf);		      

extern   uchar GetReg(uchar addr,uchar* regdata);
extern   uchar SetReg(uchar addr,uchar regdata);
extern   uchar ModifyReg(uchar addr,uchar mask,uchar set);

extern u8 SPI_Write_FIFO(u8 fflen,u8* ffbuf);   //SPI接口连续写FIFO 由于写的地址固定，因此，只用于写FIFO     









