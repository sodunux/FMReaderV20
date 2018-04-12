/******************** (C) COPYRIGHT 2010 STMicroelectronics ********************
* File Name          : usb_endp.c
* Author             : MCD Application Team
* Version            : V3.2.1
* Date               : 07/05/2010
* Description        : Endpoint routines
********************************************************************************
* THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Includes ------------------------------------------------------------------*/
#include "includes.h"
#include "CCID.h"

extern OS_EVENT *usb_Sem;
/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : EP1_OUT_Callback
* Description    :
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void EP1_OUT_Callback (void)
{
	uint16_t i;
	uint32_t dwTemp;

	i = USB_SIL_Read(EP1_OUT, &abBuffer[iBufferRecvCnt]);
	iBufferRecvCnt += i;

	dwTemp = 0;
	for(i=4; i>0; i--)
	{
		dwTemp <<= 8;
		dwTemp |= abBuffer[i];
	}

	if((uint32_t)iBufferRecvCnt == (10+dwTemp))
	{
		bReaderStatus = RDS_PARSER;

		OSSemPost(usb_Sem); // ·¢ËÍÐÅºÅÁ¿
	}

	SetEPRxValid(ENDP1);
}

/*******************************************************************************
* Function Name  : EP2_IN_Callback
* Description    :
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void EP2_IN_Callback(void)
{
	uint16_t  iByteNum;

	if(bReaderStatus == RDS_SENDING)
	{
		iByteNum = iBufferSendLen - iBufferSendCnt;
		if(iByteNum >= CCID_READER_BULK_SIZE)
		iByteNum = CCID_READER_BULK_SIZE;    
		else
		{
			bReaderStatus = RDS_IDLE;
		}

		UserToPMABufferCopy(&abBuffer[iBufferSendCnt], ENDP2_TXADDR, iByteNum);
		iBufferSendCnt += iByteNum;
		SetEPTxCount(ENDP2, iByteNum);
		SetEPTxValid(ENDP2); 
	}
}
/******************* (C) COPYRIGHT 2010 STMicroelectronics *****END OF FILE****/

