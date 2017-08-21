/*********************************************************************
*                                                                    *
*   Copyright (c) 2010 Shanghai FuDan MicroElectronic Inc, Ltd.      *
*   All rights reserved. Licensed Software Material.                 *
*                                                                    *
*   Unauthorized use, duplication, or distribution is strictly       *
*   prohibited by law.                                               *
*                                                                    *
**********************************************************************
*********************************************************************/

/*********************************************************************/
/* 	FM295 Uart通讯相关函数											 */
/* 	主要功能:														 */
/* 		1.实现Uart相关通讯											 */
/* 	编制:赵清泉														 */
/* 	编制时间:2012年5月24日											 */
/* 																	 */
/*********************************************************************/

#include "stm32f10x_lib.h"
#include "define.h"
#include "global.h"
#include "RegCtrl.h"
#include "lpcd_regctrl_private.h"
#include "Device_cfg.h"
#include "I2C.H"
#include "BasicAPI.h"
#include "lpcd_reg_private.h"
#include "lpcd_reg_public.h"
#include "lpcd_api_public.h"
#include "ReaderAPI.H"
#include "CardAPI.H"


vu8 Uart1TXMode;
vu8 datatranstime =0;
/****************************************************************/
/* 函数名：ProFrame												*/
/* 功能说明：Uart串口通信回复数据处理							*/
/* 入口参数：nFrmType:组帧类型									*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void ProFrame(u8 nFrmType) 
{
	u8 	tempType;
	u8  tempData;
	u8  i;
	tempType = nFrmType;
	switch (tempType)
	{
		case USUCCESS:							//正确响应
			UART1_DATA_BUF[1] = 0; 				//COMM_STATUS=0,命令正确
			UART1_DATA_BUF[2] = 0x0;			//LEN=0
			break;
		case UERROR:							//正确响应
			UART1_DATA_BUF[1] = 0xFF; 			//COMM_STATUS≠0,命令错误
			UART1_DATA_BUF[2] = 0x01;			//LEN=0
			UART1_DATA_BUF[3] = 0xFF;			//LEN=1
			break;
		case CMD_FM175xx_Get_AutoWake_Timer:
			UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
			UART1_DATA_BUF[2] = 0x02;			//LEN=2
			UART1_DATA_BUF[3] = intAutoWakeSec/0x100;	//高8位
			UART1_DATA_BUF[4] = intAutoWakeSec%0x100;	//低8位
			break;
		case CMD_GET_SYS_INTO:
			tempData = UART1_DATA_BUF[3];		//INFO类型
			UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
			if(tempData == 0x10)				//SYS CLK
			{
				UART1_DATA_BUF[2] = 0x03;			//LEN=0
				UART1_DATA_BUF[3] = 108;			//108MHz
				UART1_DATA_BUF[4] = 54;				//APB1CLK 2分频
				UART1_DATA_BUF[5] = 108;			//APB2CLK 1分频
			}
			else if(tempData == 0x11)		 	    //目前接口类型
			{
				UART1_DATA_BUF[2] = 0x01;			   //LEN=0
				UART1_DATA_BUF[3] = FM175xx_INTERFACE;   //144MHz
			}	
			break;	
		case CMD_READER_TRANSCEIVE:
			UART1_DATA_BUF[1] = 0x0;
			for(i=0;i<RF_Data_Len;i++)
			{
				UART1_DATA_BUF[i+3]=*(RF_Data_Buf+i);
			}
			UART1_DATA_BUF[2] = RF_Data_Len;
			break;	
		case VERSION_INFO:						//上电测试发送的字符串
			UART1_DATA_BUF[1] = 0x00; 			//Debug信息
			for(i=0;;i++)
			{
				UART1_DATA_BUF[i+3]=*(VersionInfo+i);
				if(*(VersionInfo+i) == '\0')
					break;
			}
			UART1_DATA_BUF[2] = i+1;
			break;
		case CMD_SYS_HANDON:
			UART1_DATA_BUF[1] = 0x0;
			UART1_DATA_BUF[2] = 0x02;			//LEN=1
			UART1_DATA_BUF[3] = 0x55;			//HAND INFO
			UART1_DATA_BUF[4] = 0xAA;			//HAND INFO
			break;
		case CMD_SYS_HARD_INFO:
			UART1_DATA_BUF[1] = 0x00; 			//Debug信息
			for(i=0;;i++)
			{
				UART1_DATA_BUF[i+3]=*(HardInfo+i);
				if(*(HardInfo+i) == '\0')
					break;
			}
			UART1_DATA_BUF[2] = i+1;
			break;
		case CMD_SYS_GET_CFG:
			UART1_DATA_BUF[1] = 0x0;
			UART1_DATA_BUF[2] = 0x01;			//LEN=1
			UART1_DATA_BUF[3] = FM175xx_INTERFACE;			//INTF
			break;
		case CMD_DEBUG_INFO:
			UART1_DATA_BUF[1] = 0x00; 			//Debug信息
			for(i=0;i<Debug_Point;i++)
			{
				UART1_DATA_BUF[i+3]=*(Debug_Buf+i);
			}
			Debug_Point = 0;
			UART1_DATA_BUF[2] = i;
			break;	
		case CMD_CFG_GET_CARD_UID:
			UART1_DATA_BUF[1] = 0x00; 			//Debug信息
			for(i=0;i<25;i++)
			{
				UART1_DATA_BUF[i+3]=*(Card_UID_Buf+i);
			}
			UART1_DATA_BUF[2] = i;
			break;
		case CMD_CFG_GET_USERIFO:
			UART1_DATA_BUF[1] = 0x00; 			//Debug信息
			for(i=0;i<10;i++)
			{
				UART1_DATA_BUF[i+3]=*(User_Info+i);
			}
			UART1_DATA_BUF[2] = i;
			break;
		case CMD_CFG_GET_LPCD_CFG:
			UART1_DATA_BUF[1] = 0x00; 			//Debug信息
			for(i=0;i<20;i++)
			{
				UART1_DATA_BUF[i+3]=*(Lpcd_Cfg+i);
			}
			UART1_DATA_BUF[2] = i;
			break;
		default:
			break;
	}
}

/****************************************************************/
/* 函数名：MakeFrameBCC											*/
/* 功能说明：生成发送数据缓冲区tUart.TX_Buf的BCC校验码			*/
/* 入口参数：tUart.TX_Buf,UART1_DATA_BUF[2]=len 					*/
/* 出口参数：N/A	UART1_DATA_BUF[len+3]=BCC						*/
/* 时间:														*/
/* 作者:														*/
/****************************************************************/
void MakeFrameBCC(u8 *ptrUartDataBuf)
{
	u8	i;
	u8	btBCC;
	
	btBCC = *ptrUartDataBuf^*(ptrUartDataBuf+1)^*(ptrUartDataBuf+2);
	for(i=0;i<*(ptrUartDataBuf+2);i++)
	{
		btBCC ^= *(ptrUartDataBuf+3+i);
	}
	i = *(ptrUartDataBuf+2);
	*(ptrUartDataBuf+3+i) = btBCC;
	*(ptrUartDataBuf+4+i) = ETX;
	if(ptrUartDataBuf == UART1_DATA_BUF)
		UART_Com1_Para.Send_Len = i+5;
	else if(ptrUartDataBuf == UART2_DATA_BUF)
		UART_Com3_Para.Send_Len = i+5;
}
/****************************************************************/
/* 函数名：CheckFrameBCC										*/
/* 功能说明：检查接收数据缓冲区tUart.TX_Buf的BCC校验码			*/
/* 入口参数：tUart.TX_Buf,tUart.TX_Buf[2]=len 					*/
/* 出口参数：BCC_OK:校验正确;BCC_ERR校验错误					*/
/* 时间:														*/
/* 作者:														*/
/****************************************************************/
u8 CheckFrameBCC(void)
{
	u8	i;
	u8	btBCC;
	
	btBCC = UART1_DATA_BUF[0]^UART1_DATA_BUF[1]^UART1_DATA_BUF[2];
	for(i=0;i<=UART1_DATA_BUF[2];i++)
	{
		btBCC ^= UART1_DATA_BUF[3+i];
	}
	if (btBCC == 0)
	{
		return BCC_OK;
	}
	else
	{
		return BCC_ERR;
	}
}
/****************************************************************/
/* 函数名：UartStartSend										*/
/* 功能说明：Uart串口通信启动发送操作							*/
/* 入口参数：N/A												*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void UartStartSend()
{ 
	u16 DMATXleng = 0;
//	USART1->DR = (u16)STX;				//发送STX数据
	UART1_DATA_BUF[0] = 0x02;              //STX
	DMA_SetCurrDataCounter(DMA_Channel4, 1);  //(u16)UART_Com1_Para.Send_Len);
	DMA_Cmd(DMA_Channel4, ENABLE);
//	DMA_Cmd(DMA_Channel4, DISABLE);
	UART1_DATA_BUF[0] = 0x01;              //01  
	
//	MakeFrameBCC(UART1_DATA_BUF);						//计算BCC及发送长度
	//启动发送数据
//	tUart.SendStatus = 0xFF;			//树发送标志
	//UART_Com1_Para.Send_Index = 0;
//	tUart.bSendOverFlag = FALSE;		//清发送数据结束标志
	if(Uart1TXMode == 1)
	{
		DMATXleng = (u16)(*(UART1_DATA_BUF+3));
		DMATXleng = (DMATXleng <<8);
		DMATXleng +=  (u16)(*(UART1_DATA_BUF+2));
		DMATXleng +=5;
		//test
		//if(datatranstime == 5)
		//		UART1_DATA_BUF[3] = 0x04;
		
	}else
	{
		MakeFrameBCC(UART1_DATA_BUF);						//计算BCC及发送长度
		DMATXleng = (u16)(UART_Com1_Para.Send_Len);
	}
	DMA_SetCurrDataCounter(DMA_Channel4, DMATXleng);  //(u16)UART_Com1_Para.Send_Len);
	DMA_Cmd(DMA_Channel4, ENABLE);
	//USART1->DR = (u16)STX;				//发送STX数据
}

/****************************************************************/
/* 函数名：Comm_Proc											*/
/* 功能说明：串口通信解帧处理函数								*/
/* 入口参数：N/A												*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void Comm_Proc() 
{
	u8 tempCMD;
	u8 res;
	u8 regdata;
	u8 regaddr;
	u8 tempData;
	u16 tempU16;
	u16 i;
	u32 tempU32;
	u8 atqatest[2] = {0x04,0x00};
	NFC_DataExTypeDef NFC_DataExStruct;

	if (CheckFrameBCC() == BCC_ERR)
	{
		return;
	}
	tempCMD = UART1_DATA_BUF[1];			//命令字
	Uart1TXMode = 0;
	switch (tempCMD)
	{
		case VERSION_INFO:
			ProFrame(VERSION_INFO);
			break;
		case CMD_GET_SYS_INTO:
			ProFrame(CMD_GET_SYS_INTO);
			break;
		case CMD_DEBUG_INFO:
			ProFrame(CMD_DEBUG_INFO);
			break;
		case CMD_SYS_HANDON:
			ProFrame(CMD_SYS_HANDON);
			break;
		case CMD_SYS_HARD_INFO:
			ProFrame(CMD_SYS_HARD_INFO);
			break;
		case CMD_SYS_GET_CFG:
			ProFrame(CMD_SYS_GET_CFG);
			break;
		case CMD_FM175xx_CHG_INTF:
			FM175xx_INTERFACE = UART1_DATA_BUF[3];		//接口类型
			FM175xx_SetInterface(FM175xx_INTERFACE);
			ProFrame(USUCCESS);
			break;
		case CMD_FM175xx_CHG_UART_BAUD:
			tempU32 = ((u32)UART1_DATA_BUF[3]<<24)+((u32)UART1_DATA_BUF[4]<<16)+((u32)UART1_DATA_BUF[5]<<8)+((u32)UART1_DATA_BUF[6]);		//波特率类型
			USART_CHG_BAUD(USART3, tempU32);
			ProFrame(USUCCESS);	
			break;
		case CMD_FM175xx_CHG_SPI_BAUD:
			tempData = UART1_DATA_BUF[3];
			tempU16 = SPI2->CR1; 
			tempU16 &= 0xFFC7; 
			tempU16 |= (u16)(tempData<<3);
			SPI2->CR1 = tempU16;
			ProFrame(USUCCESS);
			break;
		case CMD_FM175xx_CHG_I2C_BAUD:	  
			tempData = UART1_DATA_BUF[3];
			if(tempData == 0)
				I2C_MODE = I2C_HSMode;
			else 
			{
				I2C_MODE = I2C_FSMode;
				SDA_DELAY = tempData;		 //SDA数据保持时间
				SCL_DELAY = tempData;		 //SCL沿变之后，延时SCL_DELAY，SDA再变
				STA_DELAY = tempData;        //SDA在产生STA时序后，SCL保持STA_DELAY
				STO_DELAY = tempData;		 //SDA在产生STO时序后，SCL保持STA_DELAY
			}
			ProFrame(USUCCESS);
			break;
		case CMD_FM175xx_CHG_I2C_ADDR:	  
			tempData = UART1_DATA_BUF[3];
			I2C_ADDR = UART1_DATA_BUF[4];
			FM175xx_SetInterface(tempData);
			ProFrame(USUCCESS);
			break;
		case CMD_ARM_CHG_TIMER_GATE:
			tempData = UART1_DATA_BUF[3];	   //Gated Sig Type:NONE/SIGIN/A3/AUX
			regdata  = UART1_DATA_BUF[4];	   //HIGH Level Or Low Level
			if(tempData == 0x01)	  			   //SIGIN
			{
				;	
			}
			else if(tempData == 0x02)	  			   //AUX
			{
				;
			}
			ProFrame(USUCCESS);				
			break;
		case CMD_FM175xx_NRSTPD_CTRL:
			tempData = UART1_DATA_BUF[3];
			NRSTPD_CTRL(tempData);
			ProFrame(USUCCESS);
			break;
		case CMD_FM175xx_ENTER_LPCD:
			res = SetReg_Ext(BFL_JREG_LPCDCTRL1,BFL_JBIT_LPCD_EN+BFL_JBIT_LPCD_RSTN+BFL_JBIT_BIT_CTRL_SET);
			for(tempU16=4810;tempU16>0;tempU16--);
			NRSTPD_CTRL(0);			  //NRST
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				ProFrame(USUCCESS);
			}
			break;
		case CMD_FM175xx_LPCD_AutoWakeCtrl:
        	res = SetReg_Ext(BFL_JREG_AUTO_WUP_CFG,UART1_DATA_BUF[3]);
			for(tempU16=4810;tempU16>0;tempU16--);
			NRSTPD_CTRL(0);			  //NRST
			intAutoWakeSec = 0;
			Test_LPCD_AutoWakeUp = true;
		    if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				ProFrame(USUCCESS);
			}
			break;
		case CMD_FM175xx_Get_AutoWake_Timer:	  //读取AutoWakeUp值
			ProFrame(CMD_FM175xx_Get_AutoWake_Timer);
			break;
			
		case CMD_RD_REG:
			regaddr = UART1_DATA_BUF[4];
			if(regaddr & BIT6)                      //读扩展寄存器？
			{
				res = GetReg_Ext((regaddr&0x3F),&regdata);
			}
			else
			{
				res = GetReg(regaddr,&regdata);
			}
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				UART1_DATA_BUF[1] = 0x00; 			//COMM_STATUS=0,命令正确
				UART1_DATA_BUF[2] = 0x01;			//LEN=0
				UART1_DATA_BUF[3] = regdata;		//Data
			}
			break;
		case CMD_WR_REG:
			regaddr = UART1_DATA_BUF[4];
			regdata = UART1_DATA_BUF[5];
			if(regaddr & BIT6)                      //写扩展寄存器？
			{
				res = SetReg_Ext((regaddr&0x3F),regdata);
			}
			else
			{
				res = SetReg(regaddr,regdata);
			}
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				ProFrame(USUCCESS);
			}
			break;
		case CMD_I2C_RD_FIFO:
			res = I2C_Read_FIFO(&UART1_DATA_BUF[2],&UART1_DATA_BUF[3]);
			UART1_DATA_BUF[1] = 0; 	   //SUCESS
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_I2C_WR_FIFO:
			res = I2C_Write_FIFO(&UART1_DATA_BUF[2],&UART1_DATA_BUF[3]);
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				ProFrame(USUCCESS);
			}
			break;
		case CMD_SPI_WR_FIFO:
			res = SPI_Write_FIFO(UART1_DATA_BUF[2],&UART1_DATA_BUF[3]);
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				ProFrame(USUCCESS);
			}
			break;
		case CMD_WR_FIFO:
			res = Write_FIFO(UART1_DATA_BUF[2],&UART1_DATA_BUF[3]);
			if(res != TRUE)
			{
				ProFrame(UERROR);
			}
			else
			{
				ProFrame(USUCCESS);
			}
			break;
		case CMD_READER_TRANSCEIVE:
			NFC_DataExStruct.pExBuf = &UART1_DATA_BUF[3];
			NFC_DataExStruct.nBytesToSend = UART1_DATA_BUF[2];
			res = Command_Transceive(&NFC_DataExStruct);
			RF_Data_Len = NFC_DataExStruct.nBytesReceived;
			for(tempData=0;tempData<RF_Data_Len;tempData++)
			{
				*(RF_Data_Buf+tempData) = *(NFC_DataExStruct.pExBuf+tempData);
			}
			ProFrame(CMD_READER_TRANSCEIVE);
			break;
		case CMD_READERA_INITIAL:
			res = FM175xx_Initial_ReaderA();
			if(FM175XX_SUCCESS == res)
				ProFrame(USUCCESS);
			else
				ProFrame(UERROR);
			break;			
		case CMD_READERA_REQUEST:
			if(ReaderA_Request(RF_CMD_WUPA) == FM175XX_SUCCESS)                         //wake up
			{
				UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
				UART1_DATA_BUF[2] = 0x02;			//LEN=2
				UART1_DATA_BUF[3] = CardA_Sel_Res.ATQA[0];	//高8位
				UART1_DATA_BUF[4] = CardA_Sel_Res.ATQA[1];	//低8位
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_READERA_ANTICOLL:
			if(ReaderA_AntiCol(UART1_DATA_BUF[3]) == FM175XX_SUCCESS)
			{
				UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
				UART1_DATA_BUF[2] = 0x04;			//LEN=2
				UART1_DATA_BUF[3] = CardA_Sel_Res.UID[0];			//UID0
				UART1_DATA_BUF[4] = CardA_Sel_Res.UID[1];			//UID1
				UART1_DATA_BUF[5] = CardA_Sel_Res.UID[2];			//UID2
				UART1_DATA_BUF[6] = CardA_Sel_Res.UID[3];			//UID3
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_READERA_SELECT:
			if(ReaderA_Select(UART1_DATA_BUF[3]) == FM175XX_SUCCESS)
			{
				UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
				UART1_DATA_BUF[2] = 0x01;			//LEN=1
				UART1_DATA_BUF[3] = CardA_Sel_Res.SAK;			//SAK
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_READERA_RATS:
			break;
		case CMD_READERA_PPS:
			break;
		case CMD_READERB_INITIAL:
			res = FM175xx_Initial_ReaderB();
			if(FM175XX_SUCCESS == res)
				ProFrame(USUCCESS);
			else
				ProFrame(UERROR);
			break;
		case CMD_READERB_REQUEST:
			
			break;
		case CMD_CARD_AUTOSENDATQA:                               //for NB1411 
			 
			 FM17550_Send_ATQA(atqatest); 
		
			 ProFrame(USUCCESS);
			break;
		case CMD_CARD_INITIAL:
			res = FM175xx_Initial_Card();
			if(FM175XX_SUCCESS == res)
				ProFrame(USUCCESS);
			else
				ProFrame(UERROR);
			break;
		case CMD_CARD_CONFIG:
			res = FM175xx_Card_Config(&UART1_DATA_BUF[3]);
			if(FM175XX_SUCCESS == res)
				ProFrame(USUCCESS);
			else
				ProFrame(UERROR);
			break;
		case CMD_READERF_INITIAL:
			res = FM175xx_Initial_ReaderF();
			if(FM175XX_SUCCESS == res)
				ProFrame(USUCCESS);
			else
				ProFrame(UERROR);
			break;
		case CMD_READERF_POLL_REQ:
			if(ReaderF_Polling(UART1_DATA_BUF[3],&UART1_DATA_BUF[3])  == FM175XX_SUCCESS)
			{
				UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
				UART1_DATA_BUF[2] = 0x13;			//LEN=2
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_READERF_POLL_RES:
			break;
		case CMD_CFG_SET_CARD_UID:
			if(UART1_DATA_BUF[2] != 25)
			{
				ProFrame(UERROR);
			}
			else if(!Flash_Write_Card_UID(&UART1_DATA_BUF[3]))
			{
				ProFrame(USUCCESS);
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_CFG_GET_CARD_UID:
			if(!Flash_Read_Card_UID(Card_UID_Buf))
			{
				ProFrame(CMD_CFG_GET_CARD_UID);
			}
			else
			{
				ProFrame(UERROR);
			}	
			break;
		case CMD_CFG_SET_USERIFO:
			if(UART1_DATA_BUF[2] != 10)
			{
				ProFrame(UERROR);
			}
			else if(!Flash_Write_UserInfo(&UART1_DATA_BUF[3]))
			{
				ProFrame(USUCCESS);
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_CFG_GET_USERIFO:
			if(!Flash_Read_UserInfo(User_Info))
			{
				ProFrame(CMD_CFG_GET_USERIFO);
			}
			else
			{
				ProFrame(UERROR);
			}	
			break;
		case CMD_CFG_SET_LPCD_CFG:
			if(UART1_DATA_BUF[2] != 20)
			{
				ProFrame(UERROR);
			}
			else if(!Flash_Write_Lpcd_Cfg(&UART1_DATA_BUF[3]))
			{
				ProFrame(USUCCESS);
			}
			else
			{
				ProFrame(UERROR);
			}
			break;
		case CMD_CFG_GET_LPCD_CFG:
			if(!Flash_Read_Lpcd_Cfg(Lpcd_Cfg))
			{
				ProFrame(CMD_CFG_GET_LPCD_CFG);
			}
			else
			{
				ProFrame(UERROR);
			}	
			break;
		case CMD_AD_DATA_TRANSCEIVE:
		{
			
			Uart1TXMode = 1;
			if(datatranstime >= 9)
			{
				UART1_DATA_BUF[1] = 0x06;     //end
				datatranstime = 0;
			}else
			{
			UART1_DATA_BUF[1] = 0x07; 			// not end
			 datatranstime ++;
			}
			UART1_DATA_BUF[2] = 0xE8;			//LEN  low
			UART1_DATA_BUF[3] = 0x03;			//LEN high
			
			for(i=0;i<1000;i++)
			{
				UART1_DATA_BUF[4+i] = i;
			}
			UART1_DATA_BUF[1004] = 0x03;
			
			break;
		}
		case CMD_AD_DATA_RETRANSCEIVE:
		{
			Uart1TXMode = 1;
			if(datatranstime >= 9)
			{
				UART1_DATA_BUF[1] = 0x06;     //end
				datatranstime = 0;
			}else
			{
			UART1_DATA_BUF[1] = 0x07; 			// not end
			datatranstime ++;
			}
			UART1_DATA_BUF[2] = 0xE8;			//LEN  low
			UART1_DATA_BUF[3] = 0x03;			//LEN high
			
			for(i=0;i<1000;i++)
			{
				UART1_DATA_BUF[4+i] = i;
			}
			UART1_DATA_BUF[1004] = 0x03;
			break;
		}
		case CMD_AD_DATA_READY:
		{
			UART1_DATA_BUF[1] = 0x0; 			//COMM_STATUS=0,命令正确
			UART1_DATA_BUF[2] = 0x01;			//LEN=1
			UART1_DATA_BUF[3] = 0x01;			//ready
			
			break;
		}
		default:
			break;

	}
}

/****************************************************************/
/* 函数名：Uart_Proc											*/
/* 功能说明：Uart串口通信解帧处理								*/
/* 入口参数：N/A												*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void Uart_Proc()
{		  
		Comm_Proc();						//通信命令处理
		
		UART_Com1_Para.RecvOKFLAG = FALSE; 			//清有命令处理标志状态
		UART_Com1_Para.Recv_Index = 0;				//接收指针清零
		UART_Com1_Para.RecvStatus = 0;				//接收状态复位为开始接收装态

		UartStartSend();					//启动发送数据
}

/****************************************************************/
/* 函数名：SendMsgWelcome										*/
/* 功能说明：Uart串口通信解帧处理								*/
/* 入口参数：N/A												*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void SendMsgWelcome(void)
{
	ProFrame(VERSION_INFO);

	UART_Com1_Para.RecvOKFLAG = FALSE; 			//清有命令处理标志状态
	UART_Com1_Para.Recv_Index = 0;				//接收指针清零
	UART_Com1_Para.RecvStatus = 0;				//接收状态复位为开始接收装态

	UartStartSend();					//启动发送数据
}

/****************************************************************/
/* 函数名：Uart2_Start_Send										*/
/* 功能说明：Uart串口通信解帧处理								*/
/* 入口参数：N/A												*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void Uart2_Start_Send(void)
{	
		UART_Com3_Para.RecvOKFLAG = FALSE; 			//清有命令处理标志状态
		UART_Com3_Para.Recv_Index = 0;				//接收指针清零
		UART_Com3_Para.RecvStatus = 0;				//接收状态复位为开始接收装态

		MakeFrameBCC(UART2_DATA_BUF);				//计算BCC及发送长度
		UART_Com1_Para.Send_Index = 0;
		USART2->DR = (u16)STX;						//发送STX数据
}

/****************************************************************/
/* 函数名：Uart2Frame												*/
/* 功能说明：Uart串口通信回复数据处理							*/
/* 入口参数：nFrmType:组帧类型									*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void Uart2Frame(u8 nFrmType) 
{
	u8 	tempType;
	tempType = nFrmType;
	switch (tempType)
	{				
		default:
			break;
	}
}
/****************************************************************/
/* 函数名：PushDebugInfo										*/
/* 功能说明：压栈debug信息										*/
/* 入口参数：需要压栈的内容										*/
/* 出口参数：N/A												*/
/* 时间:  														*/
/* 作者:  														*/
/****************************************************************/
void PushDebugInfo(uchar DebugInfo)
{
	if (Debug_Point < DEBUG_BUF_LEN)
	{
		Debug_Buf[Debug_Point] = DebugInfo;
		Debug_Point ++ ;
	}
	
}
