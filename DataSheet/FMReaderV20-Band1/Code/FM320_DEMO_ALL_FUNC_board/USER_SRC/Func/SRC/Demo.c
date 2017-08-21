/************************************************************
 Shanghai Fudan Microelectronics Group Company Limited
 
 file name:demo.c
 
 version:v0.10

 project:FM320
*************************************************************/

#include "global.h"
#include "String.h"
#include "DEVICE_CFG.H"
#include "UART.h"
#include "RegCtrl.h"
#include "I2C.h"
#include "BasicAPI.h"
#include "FMReg.h"
#include "ReaderAPI.h"
#include "CardAPI.h"
#include "oled.h"
#include "define.h"

#include "Demo.h"
 
u8 STA_RF = OFF;		//场状态

u8 RFdataBuf[BUF_LENTH];		//非接触端数据缓存
u32 RFdataLen=0;					//非接触缓存区数据长度

u8 RFapduReady = OFF; 			//非接触缓存区APDU数据已准备好待发送

u8 RFapduDemo	= OFF;


u8 CTdataBuf[BUF_LENTH];		//接触端(spi)数据缓存
u32 CTdataLen=0;					//接触缓存区数据长度
u32 CTdataIndex;					//接触缓存区数据长度(已发送)
u8 CTdataFlag = 0;


u8 sendBuf[300];
u32 sendLen;

u8 testBuf[300];
u32 testLen;

u8 FlagFirstFrame = ON;			//卡片首帧标识
u8 FlagErrIrq = OFF;

u32 FSDI = 64-2;		//-4帧长度PCD
u8 CID = 0;
u8 block_num = 1;
u8 lcid = 0;			//用于cid对于帧的长度偏移
u8 fwt = 3;						//ms单位(2~400us)

u8 f_pdtp_write = OFF;		//PDTP write 初始化标识
u8 f_pdtp_read = OFF;		//PDTP read 初始化标识

u32 len_pdtp_write = 0;		//PDTP 待写数据长度
u32 len_pdtp_read	= 0;		//PDTP 待写数据长度

u32 id_pdtp_write = 0;		//PDTP write packet id
u32 id_pdtp_read = 0;		//PDTP read packet id

u8 irq_data_ind = 0;		//非接数据接收终端标识
u8 irq_rxdone = 0;
u8 irq_txdone = 0;

u8 RATS = 0xE0;
u8 ATS[5] = {0x05,0x72,0xF7,0x60,0x02};			//ats:0572F76002

void cpu_to_be32p(u8*p, u32 n)
{
	p[0] = (u8)(n>>24);
	p[1] = (u8)(n>>16);
	p[2] = (u8)(n>>8);
	p[3] = (u8)(n);
}

/*********************************************************************************************************
** 函数名称:	FM175xx_Check_FIFOcnt
** 函数功能:	查320fifo有多少字节（<=64）
** 输入参数:	无
** 输出参数:	无
** 返回值: 		fifo长度
*********************************************************************************************************/
u8 FM175xx_Chk_FIFOcnt(void)
{
	u8 res,regdata;
	
	res = GetReg(FIFOLEVEL,&regdata);
	if(res != TRUE)
		return 0;
	regdata &= 0x7F;

	return regdata;
}

/*********************************************************************************************************
** 函数名称:	FM175xx_WriteFIFO
** 函数功能:	写FIFO
** 输入参数:		ilen:写数据长度(<=64 fifo空间)
** 					*ibuf:写的数据
** 输出参数:		无
** 返回值:			是否读取成功
*********************************************************************************************************/
u8 FM175xx_WriteFIFO(u8 *ibuf,u32 ilen)
{	
	u8 res;
	u32 i;
				
	for(i=0;i<ilen;i++)				 //Buf首字节为数据长度，不再回发
	{
		res = SetReg(FIFODATA,ibuf[i]);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}

	return FM175xx_DEBUG_OK;
}

/*********************************************************************************************************
** 函数名称:	ReadFIFO
** 函数功能:	发送NAK帧
** 输入参数:		*ilen:待读取的数据长度
** 输出参数:		*rbuf:读取的数据
** 返回值:			是否读取成功
*********************************************************************************************************/
u8 FM175xx_ReadFIFO(u32 ilen,u8 *rbuf)
{
	u32 i;
	u8 res;
	
	for(i=0;i<ilen;i++)
	{
		res = GetReg(FIFODATA,&rbuf[i]);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}	
	
	return FM175xx_DEBUG_OK;
}


/*********************************************************************************************************
** 函数名称:	FM175xx_RFDataTx
** 函数功能:	RF数据回发
** 输入参数:		ilen:回发数据长度
** 					*ibuf:回发的数据
** 输出参数:		无
** 返回值:			是否读取成功
*********************************************************************************************************/
u8 FM175xx_RFDataTx(u32 ilen,u8 *ibuf)
{
	u8 res,regdata;
	u32 slen;
	u8 *sbuf;
	
	slen = ilen;
	sbuf = ibuf;
	
	SetReg(FIFOLEVEL,0x80);		//清FIFO

	if(slen <= 64)
	{
		FM175xx_WriteFIFO(sbuf,slen);		//write fifo	有多少发多少
		slen = 0;
	}
	else
	{
		FM175xx_WriteFIFO(sbuf,64);			//write fifo	先发64字节进fifo
		
		slen -= 64;		//待发长度－32
		sbuf += 64;		//待发数据指针+32
	}
		
	res = SetReg(COMMIRQ,PHCS_BFL_JBIT_TXI);		//清除TXI中断
	if(res != TRUE)
		return FM175xx_REG_ERR;

	res = SetReg(COMMAND,CMD_TRANSMIT);	   			//启动发送
	if(res != TRUE)
		return FM175xx_REG_ERR;

	while(slen>0)
	{		
		if(FM175xx_Chk_FIFOcnt()<=32)
		{
			if(slen<=32)
			{
				FM175xx_WriteFIFO(sbuf,slen);			//write fifo	剩余的数据
				slen = 0;				
			}
			else
			{
				FM175xx_WriteFIFO(sbuf,32);			//write fifo	先发32字节进fifo
				
				slen -= 32; 	//待发长度-32
				sbuf += 32; 	//待发数据指针+32
			}
		}
	}

	do
	{
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}
	while(FM175xx_Card_Processing && !(regdata&PHCS_BFL_JBIT_TXI));	//等待发送结束
		
	res = SetReg(COMMAND,CMD_RECEIVE);	 //进入接收状态
	if(res != TRUE)
		return FM175xx_REG_ERR;	
		
	SetReg(FIFOLEVEL,0x80);		//清FIFO
	
	FM175xx_Card_Processing = FALSE;
	
	return FM175xx_DEBUG_OK;
}

/*********************************************************************************************************
** 函数名称:	FM175xx_RFDataRx
** 函数功能:	写FIFO
** 输入参数:		rbuf:读取数据
** 输出参数:		无
** 返回值:			读取的数据长度
*********************************************************************************************************/
u32 FM175xx_RFDataRx(u8 *rbuf)
{
	u8 res,regdata;
	u32 rlen,temp;

	rlen = 0;
	temp = 0;
	
	FM175xx_Card_Processing = TRUE;
	
	do
	{	
		if(FM175xx_Chk_FIFOcnt() >= 32)	//查fifo是否到32字节
		{
			FM175xx_ReadFIFO(32,&rbuf[rlen]);		//渐满之后读32字节
					
			rlen += 32;
		}
		
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}
	while(!(regdata & PHCS_BFL_JBIT_RXI));

	temp = FM175xx_Chk_FIFOcnt();	//接收完全之后，查fifo有多少字节

	FM175xx_ReadFIFO(temp,&rbuf[rlen]);		//读最后的数据
	rlen += temp;
		
	return rlen;
}

/*********************************************************************************************************
** 函数名称:	SetRatsCfg
** 函数功能:	配置卡片rats相关数据
** 输入参数:		rats 参数
** 输出参数:		无
** 返回值:			无
*********************************************************************************************************/
void FM175xx_SetRatsCfg(u8 rats)
{
	u8 temp;
	
	CID = rats & 0x0F;
	temp = (rats >> 4) & 0x0F;
	
	if(temp < 5)
		FSDI = 8*(temp+2);
	else if((temp >= 5)&&(temp <= 7))
		FSDI = 32*(temp-3);
	else
		FSDI = 256;

	FSDI -= 2;	//减去2字节EDC
	block_num = 0x01;	//初始为1，调用前，一直为上一帧的块号
}

/*********************************************************************************************************
** 函数名称:	FM175xx_SetSpeed
** 函数功能:	配置卡片bps
** 输入参数:		rats 参数
** 输出参数:		无
** 返回值:			无
*********************************************************************************************************/
u8 FM175xx_SetSpeed(u8 pps1)
{
	u8 res,regdata;
	u8 dsi,dri;
	
	
	//发送
	dsi = (pps1 << 2) & 0x30;
	
	res = GetReg(TXMODE,&regdata);
	if(res != TRUE)
		return res;
	
	regdata = regdata & ~(BIT6|BIT5|BIT4);
	regdata |= dsi;					//BIT6~4 = bps
	
	res = SetReg(TXMODE,regdata);
	if(res != TRUE)
		return res;
	
	//发送
	dri = (pps1 << 4) & 0x30;
	
	res = GetReg(RXMODE,&regdata);
	if(res != TRUE)
		return res;
	
	regdata = regdata & ~(BIT6|BIT5|BIT4);
	regdata |= dri;					//BIT6~4 = bps
	
	res = SetReg(RXMODE,regdata);
	if(res != TRUE)
		return res;
	
	return FM175xx_DEBUG_OK;
}



/*********************************************************************************************************
** 函数名称:	FM175xx_Resp_WTX
** 函数功能:	fm320响应wtx，有数据则直接答复
** 输入参数:		无
** 输出参数:		无
** 返回值:			1:需要开启wtx中断，0:结束
*********************************************************************************************************/
void FM175xx_Resp_WTX(void)
{
	//if(CTdataLen > 0)	//有数据，回I_BLOCK
	if(CTdataFlag == 1)
	{
		CTdataIndex = 0;			//已读取的数据长度为0
			
		if(CTdataLen > FM175xx_APDU_FSDI) 	//带链接发送
		{			
			sendBuf[lcid] = CID;
			sendBuf[0] = 0x12 | (lcid << 3) | block_num;						
			memcpy(&sendBuf[1+lcid],&CTdataBuf[0],FM175xx_APDU_FSDI);
	
			CTdataIndex += FM175xx_APDU_FSDI;
			sendLen = FSDI;
				
			CTdataLen -= FM175xx_APDU_FSDI;
		}
		else
		{		
			sendBuf[lcid] = CID;
			sendBuf[0] = 0x02 | (lcid << 3) | block_num;						
			memcpy(&sendBuf[1+lcid],&CTdataBuf[0],CTdataLen);
	
			sendLen = CTdataLen+1+lcid;
				
			CTdataLen = 0;
			CTdataFlag = 0;
		}
	
		FM175xx_RFDataTx(sendLen,sendBuf);
		
		memcpy(testBuf,sendBuf,sendLen);
		testLen = sendLen;
	}
	else
	{		
		sendBuf[lcid] = CID;
		sendBuf[0] = 0xF2 | (lcid << 3);
		sendBuf[1 + lcid] = 0x08;

		sendLen = 2+lcid;
		
		FM175xx_RFDataTx( sendLen,sendBuf);		//发wts
	}	
}


/*********************************************************************************************************
** 函数名称:	FM175xx_FIFO_func
** 函数功能:	判断FIFO数据的功能
** 输入参数:		ibuf
** 输出参数:		无
** 返回值:			协议模式
*********************************************************************************************************/
u8 FM175xx_FIFO_func(u8 ibuf)
{		
	if( ibuf == 0xE0 )			//处理Rats指令	
	{
		return FUNC_RATS;	
	}
	
	if( (ibuf&0xF0) == 0xD0 ) 	//处理pps
	{
		return	FUNC_PPSS;	
	}
	
	if( (ibuf&0xE2) == 0x02)		//处理I_BLOCK
	{
		return	FUNC_I_BLOCK;	
	}
	
	if( (ibuf&0xE6) == 0xA2)		//处理R_BLOCK		
	{
		return	FUNC_R_BLOCK;	
	}
	
	if((ibuf&0xC7) == 0xC2)		//处理S_BLOCK	
	{
		return	FUNC_S_BLOCK;	
	}
	
	else 
		return	FUNC_S_BLOCK;
}



/*********************************************************************************************************
** 函数名称:	FM175xx_APDU_Proc
** 函数功能:	320非接处理
** 输入参数:		rbuf:接收到的数据
** 					rlen:接收的数据长度
** 输出参数:		无
** 返回值:			无
*********************************************************************************************************/
u8 FM175xx_APDU_Proc(u8 *rbuf, u32 rlen)
{
	u8 sta;
	u32 ctrl;		//帧头	
	//register u32 _r15;
	
	ctrl = FM175xx_FIFO_func(rbuf[0]);
	
	switch(ctrl)
	{
		case FUNC_RATS:		//处理Rats指令	
			FM175xx_SetRatsCfg(rbuf[1]);
		
			memcpy(sendBuf,ATS,5);
			sendLen = 5;
			FM175xx_RFDataTx(sendLen,sendBuf);
		break;

		case FUNC_PPSS:		//处理pps
			if(rbuf[1] == 0x11)
			{
				sta = FM175xx_SetSpeed(rbuf[2]);				
				
				if(sta != FM175xx_DEBUG_OK)
				{
					break;
				}
			}	

			sendBuf[0] = 0xD0 | CID ;	
			sendLen = 1;
			FM175xx_RFDataTx(sendLen,sendBuf); 
		break;

		case FUNC_I_BLOCK:	//处理I_BLOCK						
			if(rbuf[0] & 0x08)	 		//cid跟随
			{
				if(rbuf[1]== CID)		lcid = 1;		//是否是其邋CID
				else		break;
			}
			
			if((rbuf[0]& 0x01)== block_num)	//与上一帧block相同，重发上一帧
			{
				FM175xx_RFDataTx(sendLen,sendBuf);								
				break;
			}
			
#if DEBUG_APDU == 1
			if(RFapduReady == ON)		RFdataLen = 0;	
#else
			if(RFapduReady == ON)		break;
#endif
			
			block_num ^= 0x01;		//收到正确I_block,block翻转

			if(rlen == 1)		//收到空I_BLOCK，回空I_BLOCK
			{
				sendBuf[lcid] = CID;
				sendBuf[0] = 0x02 | (lcid << 3) | block_num;												
				
				sendLen = 1+lcid;
				
				FM175xx_RFDataTx(sendLen,sendBuf);
				return APDU_PROC_DONE;
			}

			//代替上位机处理00 a4指令
			if((rlen>=5)&&(memcmp(&rbuf[1+lcid],(u8*)"\x00\xA4",2) == 0))
			{			
				sendBuf[lcid] = CID;
				sendBuf[0] = 0x02 | (lcid << 3) | block_num;						
			
				memcpy(&sendBuf[1+lcid],(u8*)"\x6A\x82",2);
				sendLen = 3+lcid;
			
				FM175xx_RFDataTx(sendLen,sendBuf); 
				return APDU_PROC_DONE;				
			}

			//printf("rlen RFdataLen %02x %02x \n",rlen,RFdataLen);
			memcpy(&RFdataBuf[RFdataLen],&rbuf[1+lcid],( rlen - 1 -lcid));				
			RFdataLen += ( rlen - 1 -lcid);
			//printf("rlen RFdataLen %02x %02x \n",rlen,RFdataLen);
			
			if(rbuf[0] & 0x10)		//带链接
			{				
			
				sendBuf[lcid] = CID;
				sendBuf[0] = 0xA2 | (lcid << 3) | block_num;
				
				sendLen = 1+lcid;
				FM175xx_RFDataTx(sendLen,sendBuf); 	//ack确认

				RFapduReady = OFF;
			}
			else		//接收到完整数据，答复
			{
				
				sendBuf[lcid] = CID;
				sendBuf[0] = 0xF2 | (lcid << 3);				
				sendBuf[1 + lcid] = 0x08;
				
				sendLen = 2+lcid;
				FM175xx_RFDataTx(sendLen,sendBuf);		//发wts
			
				if(RFdataBuf[0] == CLA_DEMO_PROC)		//判断条件，是进入上位机处理协议，还是DEMO处理
				{
					RFapduDemo = ON;
					RFapduReady = OFF;
				}
				else
				{
					RFapduDemo = OFF;
					RFapduReady = ON;
				}
			}	
		break;
		
		case FUNC_R_BLOCK:	//处理R_BLOCK	
			if(rbuf[0] & 0x08)	 		//cid跟随
			{
				if(rbuf[1]== CID)		lcid = 1;		//是否是其邋CID
				else						break;
			}

			if((rbuf[0]& 0x01)== block_num)	//与上一帧block相同，重发上一帧
			{
				FM175xx_RFDataTx(sendLen,sendBuf);
//				overwrite_pit0();		//计时器重新计数
				break;
			}
			else			//处理NAK和ACK
			{
				if(rbuf[0]& 0x10)		//nak,回ack
				{
					sendBuf[lcid] = CID;
					sendBuf[0] = 0xA2 | (lcid << 3) | block_num;					
					sendLen = 1+lcid;
					FM175xx_RFDataTx(sendLen,sendBuf); 	//ack确认
				}
				else			//处理ack后链接帧
				{
					block_num ^= 0x01;		//收到正确ack,block翻转
				
					if(CTdataLen > FM175xx_APDU_FSDI)		//带链接发送
					{
						sendBuf[lcid] = CID;
						sendBuf[0] = 0x12 | (lcid << 3) | block_num;						
						
						memcpy(&sendBuf[1+lcid],&CTdataBuf[CTdataIndex],FM175xx_APDU_FSDI);

						CTdataIndex += FM175xx_APDU_FSDI;
						CTdataLen -= FM175xx_APDU_FSDI;
						sendLen = FSDI;
					}
					else
					{
						sendBuf[lcid] = CID;
						sendBuf[0] = 0x02 | (lcid << 3) | block_num;												
						memcpy(&sendBuf[1+lcid],&CTdataBuf[CTdataIndex],CTdataLen);

						CTdataLen = 0;
						CTdataFlag = 0;
						sendLen = CTdataLen+1+lcid;
					}
					
					FM175xx_RFDataTx(sendLen,sendBuf);
				}
			}
		break;
		
		case FUNC_S_BLOCK:	//处理S_BLOCK			
			if(rbuf[0] & 0x08)	 		//cid跟随
			{
				if(rbuf[1]== CID)		lcid = 1;		//是否是其邋CID
				else						break;
			}

			if((rbuf[0] & 0x30) == 0x00)		//Deselect	,暂时只应答，不作功能性操作
			{							
				sendBuf[lcid] = CID;
				sendBuf[0] = 0xC2 | (lcid << 3) ;				
				sendLen = 1+lcid;
				FM175xx_RFDataTx(sendLen,sendBuf); 	//Deselect	
				
				//SetReg(COMMAND,CMD_SOFT_RESET);
//				FM175xx_Initial_Card();
				FM175xx_Card_AutoColl();
				//FM175xx_Initial();
			}				
			else if((rbuf[0] & 0x30) == 0x30)		//wtx
			{
				if(RFapduDemo == ON)
				{
					RFapduDemo = OFF;
					return APDU_PROC_DEMO;
				}
			
				if(CTdataFlag == 1)
				{
					CTdataIndex = 0;			//已读取的数据长度为0
						
					if(CTdataLen > FM175xx_APDU_FSDI) 	//带链接发送
					{			
						sendBuf[lcid] = CID;
						sendBuf[0] = 0x12 | (lcid << 3) | block_num;						
						memcpy(&sendBuf[1+lcid],&CTdataBuf[0],FM175xx_APDU_FSDI);
				
						CTdataIndex += FM175xx_APDU_FSDI;
						sendLen = FSDI;
							
						CTdataLen -= FM175xx_APDU_FSDI;
					}
					else
					{		
						sendBuf[lcid] = CID;
						sendBuf[0] = 0x02 | (lcid << 3) | block_num;						
						memcpy(&sendBuf[1+lcid],&CTdataBuf[0],CTdataLen);
				
						sendLen = CTdataLen+1+lcid;
							
						CTdataLen = 0;
						CTdataFlag = 0;
					}
				
					FM175xx_RFDataTx(sendLen,sendBuf);
					
					memcpy(testBuf,sendBuf,sendLen);
					testLen = sendLen;
				}
				else
				{
					return APDU_PROC_WTX;
				}
				//overwrite_pit0();		//计时器重新计数
			}
		break;
		
		default:
		break;
	}
	
	return APDU_PROC_DONE;
}


/*********************************************************************************************************
** 函数名称:	FM175xx_DEMO_Proc
** 函数功能:	320处理DEMO流程，运算/存读数据
** 输入参数:		无
** 输出参数:		无
** 返回值:			无
*********************************************************************************************************/
void FM175xx_DEMO_Proc(void)
{
	u8 ins,p1,p2;

	u32 flen = 0;
	u8 file[16] ={0};

	int res = 0;	//计算结果

	u32 slen = 0;
	u8 sbuf[17] = {0};

	
	ins = RFdataBuf[1];
	p1 = RFdataBuf[2];
	p2 = RFdataBuf[3];
	
	switch(ins) 
	{
		case INS_DEMO_PLUS: 		//加法运算
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1+p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins错
				slen = 2;
			}
		break;
			
		case INS_DEMO_MINUS:		//减法运算
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1-p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins错
				slen = 2;
			}
		break;
			
		case INS_DEMO_MULTIPLY: //乘法运算		
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1*p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins错
				slen = 2;
			}
		break;
			
		case INS_DEMO_DEVIDE:		//除法运算
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1/p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins错
				slen = 2;
			}
		break;
		
		case INS_DEMO_WRITE:		//写文件		
			flen = RFdataBuf[4];
			file[0] = flen;
			memcpy(&file[1],&RFdataBuf[5],flen);
		
			OLED_Init();
			ShowLogo();
			OLED_ShowString(64,STING_SIZE*0, "DEMO 1.8");		//ROW 3			
			OLED_ShowString(64,STING_SIZE*1, "DEMO SHOW:");		//ROW 4
			OLED_ShowString(64,STING_SIZE*2, &file[1]);		//ROW 5
			OLED_Refresh_Gram();	
		
			memcpy(sbuf,(u8*)"\x90\x00",2); 
			slen = 2;
		break;
		
//		case INS_DEMO_READ: 	//读文件
//		break;
		
		default:
			memcpy(sbuf,(u8*)"\x6D\x00",2); //ins错
			slen = 2;
		break;
	}
	
	sendBuf[lcid] = CID;
	sendBuf[0] = 0x02 | (lcid << 3) | block_num;												
	memcpy(&sendBuf[1+lcid],sbuf,slen);
		
	sendLen = slen+1+lcid;
	
	FM175xx_RFDataTx(sendLen,sendBuf);
	
	RFdataLen = 0;
}


void FM175xx_Card_APDU(void)
{
	u8 sta = 0;
	u8 rfBuf[300]={0x00};			//
	u32 rfLen = 0;
		
	rfLen = FM175xx_RFDataRx(rfBuf);		//
	
	if(rfLen > 0)
	{
		ModifyReg(COMMIEN,PHCS_BFL_JBIT_RXI | PHCS_BFL_JBIT_HIALERTI| PHCS_BFL_JBIT_ERRI,CLR);
		
		sta = FM175xx_APDU_Proc(rfBuf, rfLen);
				
		if(sta == APDU_PROC_DEMO)
		{
			FM175xx_DEMO_Proc();	
		}			
	}	
}







