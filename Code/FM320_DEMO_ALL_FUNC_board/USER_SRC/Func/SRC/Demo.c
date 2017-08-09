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
 
u8 STA_RF = OFF;		//��״̬

u8 RFdataBuf[BUF_LENTH];		//�ǽӴ������ݻ���
u32 RFdataLen=0;					//�ǽӴ����������ݳ���

u8 RFapduReady = OFF; 			//�ǽӴ�������APDU������׼���ô�����

u8 RFapduDemo	= OFF;


u8 CTdataBuf[BUF_LENTH];		//�Ӵ���(spi)���ݻ���
u32 CTdataLen=0;					//�Ӵ����������ݳ���
u32 CTdataIndex;					//�Ӵ����������ݳ���(�ѷ���)
u8 CTdataFlag = 0;


u8 sendBuf[300];
u32 sendLen;

u8 testBuf[300];
u32 testLen;

u8 FlagFirstFrame = ON;			//��Ƭ��֡��ʶ
u8 FlagErrIrq = OFF;

u32 FSDI = 64-2;		//-4֡����PCD
u8 CID = 0;
u8 block_num = 1;
u8 lcid = 0;			//����cid����֡�ĳ���ƫ��
u8 fwt = 3;						//ms��λ(2~400us)

u8 f_pdtp_write = OFF;		//PDTP write ��ʼ����ʶ
u8 f_pdtp_read = OFF;		//PDTP read ��ʼ����ʶ

u32 len_pdtp_write = 0;		//PDTP ��д���ݳ���
u32 len_pdtp_read	= 0;		//PDTP ��д���ݳ���

u32 id_pdtp_write = 0;		//PDTP write packet id
u32 id_pdtp_read = 0;		//PDTP read packet id

u8 irq_data_ind = 0;		//�ǽ����ݽ����ն˱�ʶ
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
** ��������:	FM175xx_Check_FIFOcnt
** ��������:	��320fifo�ж����ֽڣ�<=64��
** �������:	��
** �������:	��
** ����ֵ: 		fifo����
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
** ��������:	FM175xx_WriteFIFO
** ��������:	дFIFO
** �������:		ilen:д���ݳ���(<=64 fifo�ռ�)
** 					*ibuf:д������
** �������:		��
** ����ֵ:			�Ƿ��ȡ�ɹ�
*********************************************************************************************************/
u8 FM175xx_WriteFIFO(u8 *ibuf,u32 ilen)
{	
	u8 res;
	u32 i;
				
	for(i=0;i<ilen;i++)				 //Buf���ֽ�Ϊ���ݳ��ȣ����ٻط�
	{
		res = SetReg(FIFODATA,ibuf[i]);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}

	return FM175xx_DEBUG_OK;
}

/*********************************************************************************************************
** ��������:	ReadFIFO
** ��������:	����NAK֡
** �������:		*ilen:����ȡ�����ݳ���
** �������:		*rbuf:��ȡ������
** ����ֵ:			�Ƿ��ȡ�ɹ�
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
** ��������:	FM175xx_RFDataTx
** ��������:	RF���ݻط�
** �������:		ilen:�ط����ݳ���
** 					*ibuf:�ط�������
** �������:		��
** ����ֵ:			�Ƿ��ȡ�ɹ�
*********************************************************************************************************/
u8 FM175xx_RFDataTx(u32 ilen,u8 *ibuf)
{
	u8 res,regdata;
	u32 slen;
	u8 *sbuf;
	
	slen = ilen;
	sbuf = ibuf;
	
	SetReg(FIFOLEVEL,0x80);		//��FIFO

	if(slen <= 64)
	{
		FM175xx_WriteFIFO(sbuf,slen);		//write fifo	�ж��ٷ�����
		slen = 0;
	}
	else
	{
		FM175xx_WriteFIFO(sbuf,64);			//write fifo	�ȷ�64�ֽڽ�fifo
		
		slen -= 64;		//�������ȣ�32
		sbuf += 64;		//��������ָ��+32
	}
		
	res = SetReg(COMMIRQ,PHCS_BFL_JBIT_TXI);		//���TXI�ж�
	if(res != TRUE)
		return FM175xx_REG_ERR;

	res = SetReg(COMMAND,CMD_TRANSMIT);	   			//��������
	if(res != TRUE)
		return FM175xx_REG_ERR;

	while(slen>0)
	{		
		if(FM175xx_Chk_FIFOcnt()<=32)
		{
			if(slen<=32)
			{
				FM175xx_WriteFIFO(sbuf,slen);			//write fifo	ʣ�������
				slen = 0;				
			}
			else
			{
				FM175xx_WriteFIFO(sbuf,32);			//write fifo	�ȷ�32�ֽڽ�fifo
				
				slen -= 32; 	//��������-32
				sbuf += 32; 	//��������ָ��+32
			}
		}
	}

	do
	{
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}
	while(FM175xx_Card_Processing && !(regdata&PHCS_BFL_JBIT_TXI));	//�ȴ����ͽ���
		
	res = SetReg(COMMAND,CMD_RECEIVE);	 //�������״̬
	if(res != TRUE)
		return FM175xx_REG_ERR;	
		
	SetReg(FIFOLEVEL,0x80);		//��FIFO
	
	FM175xx_Card_Processing = FALSE;
	
	return FM175xx_DEBUG_OK;
}

/*********************************************************************************************************
** ��������:	FM175xx_RFDataRx
** ��������:	дFIFO
** �������:		rbuf:��ȡ����
** �������:		��
** ����ֵ:			��ȡ�����ݳ���
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
		if(FM175xx_Chk_FIFOcnt() >= 32)	//��fifo�Ƿ�32�ֽ�
		{
			FM175xx_ReadFIFO(32,&rbuf[rlen]);		//����֮���32�ֽ�
					
			rlen += 32;
		}
		
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
			return FM175xx_REG_ERR;
	}
	while(!(regdata & PHCS_BFL_JBIT_RXI));

	temp = FM175xx_Chk_FIFOcnt();	//������ȫ֮�󣬲�fifo�ж����ֽ�

	FM175xx_ReadFIFO(temp,&rbuf[rlen]);		//����������
	rlen += temp;
		
	return rlen;
}

/*********************************************************************************************************
** ��������:	SetRatsCfg
** ��������:	���ÿ�Ƭrats�������
** �������:		rats ����
** �������:		��
** ����ֵ:			��
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

	FSDI -= 2;	//��ȥ2�ֽ�EDC
	block_num = 0x01;	//��ʼΪ1������ǰ��һֱΪ��һ֡�Ŀ��
}

/*********************************************************************************************************
** ��������:	FM175xx_SetSpeed
** ��������:	���ÿ�Ƭbps
** �������:		rats ����
** �������:		��
** ����ֵ:			��
*********************************************************************************************************/
u8 FM175xx_SetSpeed(u8 pps1)
{
	u8 res,regdata;
	u8 dsi,dri;
	
	
	//����
	dsi = (pps1 << 2) & 0x30;
	
	res = GetReg(TXMODE,&regdata);
	if(res != TRUE)
		return res;
	
	regdata = regdata & ~(BIT6|BIT5|BIT4);
	regdata |= dsi;					//BIT6~4 = bps
	
	res = SetReg(TXMODE,regdata);
	if(res != TRUE)
		return res;
	
	//����
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
** ��������:	FM175xx_Resp_WTX
** ��������:	fm320��Ӧwtx����������ֱ�Ӵ�
** �������:		��
** �������:		��
** ����ֵ:			1:��Ҫ����wtx�жϣ�0:����
*********************************************************************************************************/
void FM175xx_Resp_WTX(void)
{
	//if(CTdataLen > 0)	//�����ݣ���I_BLOCK
	if(CTdataFlag == 1)
	{
		CTdataIndex = 0;			//�Ѷ�ȡ�����ݳ���Ϊ0
			
		if(CTdataLen > FM175xx_APDU_FSDI) 	//�����ӷ���
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
		
		FM175xx_RFDataTx( sendLen,sendBuf);		//��wts
	}	
}


/*********************************************************************************************************
** ��������:	FM175xx_FIFO_func
** ��������:	�ж�FIFO���ݵĹ���
** �������:		ibuf
** �������:		��
** ����ֵ:			Э��ģʽ
*********************************************************************************************************/
u8 FM175xx_FIFO_func(u8 ibuf)
{		
	if( ibuf == 0xE0 )			//����Ratsָ��	
	{
		return FUNC_RATS;	
	}
	
	if( (ibuf&0xF0) == 0xD0 ) 	//����pps
	{
		return	FUNC_PPSS;	
	}
	
	if( (ibuf&0xE2) == 0x02)		//����I_BLOCK
	{
		return	FUNC_I_BLOCK;	
	}
	
	if( (ibuf&0xE6) == 0xA2)		//����R_BLOCK		
	{
		return	FUNC_R_BLOCK;	
	}
	
	if((ibuf&0xC7) == 0xC2)		//����S_BLOCK	
	{
		return	FUNC_S_BLOCK;	
	}
	
	else 
		return	FUNC_S_BLOCK;
}



/*********************************************************************************************************
** ��������:	FM175xx_APDU_Proc
** ��������:	320�ǽӴ���
** �������:		rbuf:���յ�������
** 					rlen:���յ����ݳ���
** �������:		��
** ����ֵ:			��
*********************************************************************************************************/
u8 FM175xx_APDU_Proc(u8 *rbuf, u32 rlen)
{
	u8 sta;
	u32 ctrl;		//֡ͷ	
	//register u32 _r15;
	
	ctrl = FM175xx_FIFO_func(rbuf[0]);
	
	switch(ctrl)
	{
		case FUNC_RATS:		//����Ratsָ��	
			FM175xx_SetRatsCfg(rbuf[1]);
		
			memcpy(sendBuf,ATS,5);
			sendLen = 5;
			FM175xx_RFDataTx(sendLen,sendBuf);
		break;

		case FUNC_PPSS:		//����pps
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

		case FUNC_I_BLOCK:	//����I_BLOCK						
			if(rbuf[0] & 0x08)	 		//cid����
			{
				if(rbuf[1]== CID)		lcid = 1;		//�Ƿ�������CID
				else		break;
			}
			
			if((rbuf[0]& 0x01)== block_num)	//����һ֡block��ͬ���ط���һ֡
			{
				FM175xx_RFDataTx(sendLen,sendBuf);								
				break;
			}
			
#if DEBUG_APDU == 1
			if(RFapduReady == ON)		RFdataLen = 0;	
#else
			if(RFapduReady == ON)		break;
#endif
			
			block_num ^= 0x01;		//�յ���ȷI_block,block��ת

			if(rlen == 1)		//�յ���I_BLOCK���ؿ�I_BLOCK
			{
				sendBuf[lcid] = CID;
				sendBuf[0] = 0x02 | (lcid << 3) | block_num;												
				
				sendLen = 1+lcid;
				
				FM175xx_RFDataTx(sendLen,sendBuf);
				return APDU_PROC_DONE;
			}

			//������λ������00 a4ָ��
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
			
			if(rbuf[0] & 0x10)		//������
			{				
			
				sendBuf[lcid] = CID;
				sendBuf[0] = 0xA2 | (lcid << 3) | block_num;
				
				sendLen = 1+lcid;
				FM175xx_RFDataTx(sendLen,sendBuf); 	//ackȷ��

				RFapduReady = OFF;
			}
			else		//���յ��������ݣ���
			{
				
				sendBuf[lcid] = CID;
				sendBuf[0] = 0xF2 | (lcid << 3);				
				sendBuf[1 + lcid] = 0x08;
				
				sendLen = 2+lcid;
				FM175xx_RFDataTx(sendLen,sendBuf);		//��wts
			
				if(RFdataBuf[0] == CLA_DEMO_PROC)		//�ж��������ǽ�����λ������Э�飬����DEMO����
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
		
		case FUNC_R_BLOCK:	//����R_BLOCK	
			if(rbuf[0] & 0x08)	 		//cid����
			{
				if(rbuf[1]== CID)		lcid = 1;		//�Ƿ�������CID
				else						break;
			}

			if((rbuf[0]& 0x01)== block_num)	//����һ֡block��ͬ���ط���һ֡
			{
				FM175xx_RFDataTx(sendLen,sendBuf);
//				overwrite_pit0();		//��ʱ�����¼���
				break;
			}
			else			//����NAK��ACK
			{
				if(rbuf[0]& 0x10)		//nak,��ack
				{
					sendBuf[lcid] = CID;
					sendBuf[0] = 0xA2 | (lcid << 3) | block_num;					
					sendLen = 1+lcid;
					FM175xx_RFDataTx(sendLen,sendBuf); 	//ackȷ��
				}
				else			//����ack������֡
				{
					block_num ^= 0x01;		//�յ���ȷack,block��ת
				
					if(CTdataLen > FM175xx_APDU_FSDI)		//�����ӷ���
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
		
		case FUNC_S_BLOCK:	//����S_BLOCK			
			if(rbuf[0] & 0x08)	 		//cid����
			{
				if(rbuf[1]== CID)		lcid = 1;		//�Ƿ�������CID
				else						break;
			}

			if((rbuf[0] & 0x30) == 0x00)		//Deselect	,��ʱֻӦ�𣬲��������Բ���
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
					CTdataIndex = 0;			//�Ѷ�ȡ�����ݳ���Ϊ0
						
					if(CTdataLen > FM175xx_APDU_FSDI) 	//�����ӷ���
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
				//overwrite_pit0();		//��ʱ�����¼���
			}
		break;
		
		default:
		break;
	}
	
	return APDU_PROC_DONE;
}


/*********************************************************************************************************
** ��������:	FM175xx_DEMO_Proc
** ��������:	320����DEMO���̣�����/�������
** �������:		��
** �������:		��
** ����ֵ:			��
*********************************************************************************************************/
void FM175xx_DEMO_Proc(void)
{
	u8 ins,p1,p2;

	u32 flen = 0;
	u8 file[16] ={0};

	int res = 0;	//������

	u32 slen = 0;
	u8 sbuf[17] = {0};

	
	ins = RFdataBuf[1];
	p1 = RFdataBuf[2];
	p2 = RFdataBuf[3];
	
	switch(ins) 
	{
		case INS_DEMO_PLUS: 		//�ӷ�����
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1+p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins��
				slen = 2;
			}
		break;
			
		case INS_DEMO_MINUS:		//��������
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1-p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins��
				slen = 2;
			}
		break;
			
		case INS_DEMO_MULTIPLY: //�˷�����		
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1*p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins��
				slen = 2;
			}
		break;
			
		case INS_DEMO_DEVIDE:		//��������
			if((p1<=100)&&(p2<=100))
			{				
				res = (int)(p1/p2);
			
				cpu_to_be32p(sbuf,res);
				memcpy(&sbuf[4],(u8*)"\x90\x00",2);
				slen = 6;		
			}
			else
			{
				memcpy(sbuf,(u8*)"\x6A\x86",2); //ins��
				slen = 2;
			}
		break;
		
		case INS_DEMO_WRITE:		//д�ļ�		
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
		
//		case INS_DEMO_READ: 	//���ļ�
//		break;
		
		default:
			memcpy(sbuf,(u8*)"\x6D\x00",2); //ins��
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







