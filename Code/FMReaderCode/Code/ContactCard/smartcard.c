/******************** (C) COPYRIGHT 2007 STMicroelectronics ********************
* File Name          : smartcard.c
* Author             : MCD Application Team
* Version            : V1.0
* Date               : 10/08/2007
* Description        : This file provides all the Smartcard firmware functions.
********************************************************************************
* THE PRESENT SOFTWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH SOFTWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Includes ------------------------------------------------------------------*/
#include "smartcard.h"

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* Global variables definition and initialization ----------------------------*/
SC_ATR SC_A2R;
u8 SC_ATR_Table[40];
static vu8 SCData = 0;
static u32 F_Table[16] = {0, 372, 558, 744, 1116, 1488, 1860, 0,
                          0, 512, 768, 1024, 1536, 2048, 0, 0};
static u32 D_Table[8] = {0, 1, 2, 4, 8, 16, 0, 0};

SC_State state;
SC_ADPU_Commands apdu_commands;
SC_ADPU_Responce apdu_responce;

/* Private function prototypes -----------------------------------------------*/
/* Transport Layer -----------------------------------------------------------*/
/*--------------APDU-----------*/
static void SC_SendData(SC_ADPU_Commands *SC_ADPU, SC_ADPU_Responce *SC_ResponceStatus);

/*------------ ATR ------------*/
static void SC_AnswerReq(SC_State *SCState, u8 *card, u8 length);  /* Ask ATR */
static u8 SC_decode_Answer2reset(u8 *card);  /* Decode ATR */

/* Physical Port Layer -------------------------------------------------------*/


/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : SC_Handler
* Description    : Handles all Smartcard states and serves to send and receive all
*                  communication data between Smartcard and reader.
* Input          : - SCState: pointer to an SC_State enumeration that will contain
*                    the Smartcard state.
*                  - SC_ADPU: pointer to an SC_ADPU_Commands structure that will be
*                    initialized.  
*                  - SC_Response: pointer to a SC_ADPU_Responce structure which will
*                    be initialized.
* Output         : None
* Return         : None
*******************************************************************************/
void SC_Handler(SC_State *SCState, SC_ADPU_Commands *SC_ADPU, SC_ADPU_Responce *SC_Response)
{
  u32 i = 0;

  switch(*SCState)
  {
    case SC_POWER_ON:
      if (SC_ADPU->Header.INS == SC_GET_A2R)
      {
        /* Smartcard intialization ------------------------------------------*/
        SC_Init();

        /* Reset Data from SC buffer -----------------------------------------*/
        for (i = 0; i < 40; i++)
        {
          SC_ATR_Table[i] = 0;
        }
        
        /* Reset SC_A2R Structure --------------------------------------------*/
        SC_A2R.TS = 0;
        SC_A2R.T0 = 0;
        for (i = 0; i < SETUP_LENGTH; i++)
        {
          SC_A2R.T[i] = 0;
        }
        for (i = 0; i < HIST_LENGTH; i++)
        {
          SC_A2R.H[i] = 0;
        }
        SC_A2R.Tlength = 0;
        SC_A2R.Hlength = 0;
        
        /* Next State --------------------------------------------------------*/
        *SCState = SC_RESET_LOW;
      }
    break;

    case SC_RESET_LOW:
      if(SC_ADPU->Header.INS == SC_GET_A2R)
      {
        /* If card is detected then Power ON, Card Reset and wait for an answer) */
        if (SC_Detect())
        {
          while(((*SCState) != SC_POWER_OFF) && ((*SCState) != SC_ACTIVE))
          {
            SC_AnswerReq(SCState, &SC_ATR_Table[0], 20); /* Check for answer to reset */
          }
        }
        else
        {
          (*SCState) = SC_POWER_OFF;
        } 
      }
    break;

    case SC_ACTIVE:
      if (SC_ADPU->Header.INS == SC_GET_A2R)
      {
        if(SC_decode_Answer2reset(&SC_ATR_Table[0]) == T0_PROTOCOL)
        {
          (*SCState) = SC_ACTIVE_ON_T0;
        }
        else
        {
          (*SCState) = SC_POWER_OFF; 
        }
      }
    break;

    case SC_ACTIVE_ON_T0:
      SC_SendData(SC_ADPU, SC_Response);
    break;

    case SC_POWER_OFF:
      //SC_DeInit(); /* Disable Smartcard interface */
    break;

    default: (*SCState) = SC_POWER_OFF;
  }
}

/*******************************************************************************
* Function Name  : SC_PowerCmd
* Description    : Enables or disables the power VCC to the Smartcard.
* Input          : NewState: new state of the Smartcard power supply. 
*                  This parameter can be: ENABLE(VCC=1) or DISABLE(VCC=0).
* Output         : None
* Return         : None
*******************************************************************************/
void SC_PowerCmd(FunctionalState NewState)
{
  if(NewState == ENABLE)
  {
    GPIO_ResetBits(SC_CMDVCC_PORT, SC_CMDVCC);
  }
  else
  {
    GPIO_SetBits(SC_CMDVCC_PORT, SC_CMDVCC);
  } 
}

/*******************************************************************************
* Function Name  : SC_Reset
* Description    : Sets or clears the Smartcard reset pin.
* Input          : - ResetState: this parameter specifies the state of the Smartcard 
*                    reset pin.
*                    BitVal must be one of the BitAction enum values:
*                       - Bit_RESET: to clear the port pin.
*                       - Bit_SET: to set the port pin.
* Output         : None
* Return         : None
*******************************************************************************/
void SC_Reset(BitAction ResetState)
{
  GPIO_WriteBit(SC_RESET_PORT, SC_RESET, ResetState);
}

/*******************************************************************************
* Function Name  : SC_ParityErrorHandler
* Description    : Resends the byte that failed to be received (by the Smartcard)
*                  correctly.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SC_ParityErrorHandler(void)
{
  USART_SendData(USART3, SCData);
  while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
  {
  } 
}

/*******************************************************************************
* Function Name  : SC_PTSConfig
* Description    : Configures the IO speed (BaudRate) communication.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SC_PTSConfig(void)
{
  RCC_ClocksTypeDef RCC_ClocksStatus;
  u32 workingbaudrate = 0, apbclock = 0;
  u8 locData = 0, PTSConfirmStatus = 1;
  USART_InitTypeDef USART_InitStructure;

  /* Reconfigure the USART Baud Rate -------------------------------------------*/
  RCC_GetClocksFreq(&RCC_ClocksStatus);
  apbclock = RCC_ClocksStatus.PCLK1_Frequency;
  apbclock /= ((USART3->GTPR & (u16)0x00FF) * 2);

  /* Enable the DMA Receive (Set DMAR bit only) to enable interrupt generation
     in case of a framing error FE */  
  USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);
  
  if((SC_A2R.T0 & (u8)0x10) == 0x10)
  {
    if(SC_A2R.T[0] != 0x11)
    {
      /* Send PTSS */
      SCData = 0xFF;
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Send PTS0 */
      SCData = 0x10;
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Send PTS1 */
      SCData = SC_A2R.T[0]; 
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Send PCK */
      SCData = (u8)0xFF^(u8)0x10^(u8)SC_A2R.T[0]; 
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Disable the DMA Receive (Reset DMAR bit only) */  
      USART_DMACmd(USART3, USART_DMAReq_Rx, DISABLE);
   
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != 0xFF)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != 0x10)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != SC_A2R.T[0])
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != ((u8)0xFF^(u8)0x10^(u8)SC_A2R.T[0]))
        {
           PTSConfirmStatus = 0x00;
        }
      }
      else
      {
        PTSConfirmStatus = 0x00;
      }
      /* PTS Confirm */
      if(PTSConfirmStatus == 0x01)
      {
        workingbaudrate = apbclock * D_Table[(SC_A2R.T[0] & (u8)0x0F)];
        workingbaudrate /= F_Table[((SC_A2R.T[0] >> 4) & (u8)0x0F)];
        USART_StructInit(&USART_InitStructure);
        USART_InitStructure.USART_BaudRate = workingbaudrate;
        USART_InitStructure.USART_WordLength = USART_WordLength_9b;
        USART_InitStructure.USART_StopBits = USART_StopBits_1_5;
        USART_InitStructure.USART_Parity = USART_Parity_Even;
        USART_InitStructure.USART_Clock = USART_Clock_Enable;
        USART_Init(USART3, &USART_InitStructure);
      }
    }
  }  
}

/*******************************************************************************
* Function Name  : SC_PPS
* Description    : Configures the IO speed (BaudRate) communication.
* Input          : PPS1 
* Output         : 0x00 Failed, 0x01 Succeed
* Return         : None
*******************************************************************************/
u8 SC_PPS(u8 PPS1)
{
	RCC_ClocksTypeDef RCC_ClocksStatus;
  u32 workingbaudrate = 0, apbclock = 0;
  u8 locData = 0, PTSConfirmStatus = 1;
  USART_InitTypeDef USART_InitStructure;
  
  /* Reconfigure the USART Baud Rate -------------------------------------------*/
  RCC_GetClocksFreq(&RCC_ClocksStatus);
  apbclock = RCC_ClocksStatus.PCLK1_Frequency;
  apbclock /= ((USART3->GTPR & (u16)0x00FF) * 2);

  /* Enable the DMA Receive (Set DMAR bit only) to enable interrupt generation
     in case of a framing error FE */  
  USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);
  
  if(PPS1!= 0x11)
    {
      /* Send PTSS */
      SCData = 0xFF;
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Send PTS0 */
      SCData = 0x10;
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Send PTS1 */
      SCData = PPS1; 
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      } 

      /* Send PCK */
      SCData = (u8)0xFF^(u8)0x10^(u8)PPS1; 
      USART_SendData(USART3, SCData);
      while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
      {
      }

      /* Disable the DMA Receive (Reset DMAR bit only) */  
      USART_DMACmd(USART3, USART_DMAReq_Rx, DISABLE);
   
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != 0xFF)
        {
           PTSConfirmStatus = 0x00;
        }
      }
			if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != 0xFF)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != 0x10)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != PPS1)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
        if(locData != ((u8)0xFF^(u8)0x10^(u8)PPS1))
        {
           PTSConfirmStatus = 0x00;
        }
      }
      else
      {
        PTSConfirmStatus = 0x00;
      }
      /* PTS Confirm */
      if(PTSConfirmStatus == 0x01)
      {
        workingbaudrate = apbclock * D_Table[(PPS1 & (u8)0x0F)];
        workingbaudrate /= F_Table[((PPS1 >> 4) & (u8)0x0F)];
        USART_StructInit(&USART_InitStructure);
        USART_InitStructure.USART_BaudRate = workingbaudrate;
        USART_InitStructure.USART_WordLength = USART_WordLength_9b;
        USART_InitStructure.USART_StopBits = USART_StopBits_1_5;
        USART_InitStructure.USART_Parity = USART_Parity_Even;
        USART_InitStructure.USART_Clock = USART_Clock_Enable;
        USART_Init(USART3, &USART_InitStructure);
				return 0x01;
      }
			else 
				return 0x00;
    }
	return 0x01;		
}

/*******************************************************************************
* Function Name  : SC_DirectSendData
* Description    : Manages the Smartcard transport layer: send APDU commands and
*                  receives the APDU responce.
* Input          : - SC_ADPU: pointer to a SC_ADPU_Commands structure which 
*                    will be initialized.  
*                  - SC_Response: pointer to a SC_ADPU_Responce structure which 
*                    will be initialized.
* Output         : None
* Return         : None
*******************************************************************************/
void SC_DirectSendData(u8 *buffer,u8 sendlen,u8 recvlen)
{
	u32 i=0;
	u8 locData=0;
	USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);
	if(buffer[4]>0)
	{
	//Send Data to SC
	for(i=0;i<sendlen;i++)
		{
			SCData=buffer[i];
			USART_SendData(USART3, SCData);
			while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET){} 			
		}
  	/* Flush the USART3 DR */
 (void)USART_ReceiveData(USART3);
	//Receive Data from SC
	
	for(i = 0; i < recvlen; i++)
    {
      if(USART_ByteReceive(&locData, SC_Receive_Timeout) == SUCCESS)
      {
        buffer[i] = locData;
      }
    }
		buffer[i]=0x90;
		buffer[i+1]=0x00;
		recvlen=i+2;		
	}
	else 
	{
		buffer[0]=0x67;
		buffer[1]=0x00;
		recvlen=0x02;	
	}
	
}	
	






/*******************************************************************************
* Function Name  : SC_SendData
* Description    : Manages the Smartcard transport layer: send APDU commands and
*                  receives the APDU responce.
* Input          : - SC_ADPU: pointer to a SC_ADPU_Commands structure which 
*                    will be initialized.  
*                  - SC_Response: pointer to a SC_ADPU_Responce structure which 
*                    will be initialized.
* Output         : None
* Return         : None
*******************************************************************************/
void SC_SendData(SC_ADPU_Commands *SC_ADPU, SC_ADPU_Responce *SC_ResponceStatus)
{
  u32 i = 0;
  u8 locData = 0;

  /* Reset responce buffer ---------------------------------------------------*/
  for(i = 0; i < LCmax; i++)
  {
    SC_ResponceStatus->Data[i] = 0;
  }
  
  SC_ResponceStatus->SW1 = 0;
  SC_ResponceStatus->SW2 = 0;

  /* Enable the DMA Receive (Set DMAR bit only) to enable interrupt generation
     in case of a framing error FE */  
  USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);

  /* Send header -------------------------------------------------------------*/
  SCData = SC_ADPU->Header.CLA;
  USART_SendData(USART3, SCData);
  while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
  {		
  }
		
  SCData = SC_ADPU->Header.INS;
  USART_SendData(USART3, SCData);
  while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
  {
  }
	
  SCData = SC_ADPU->Header.P1;
  USART_SendData(USART3, SCData);
  while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
  {
  }  
  SCData = SC_ADPU->Header.P2;
  USART_SendData(USART3, SCData);
  while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
  {
  }   
  /* Send body length to/from SC ---------------------------------------------*/
  if(SC_ADPU->Body.LC)
  {
    SCData = SC_ADPU->Body.LC;
    USART_SendData(USART3, SCData);
    while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
    {
    }
  }
  else if(SC_ADPU->Body.LE)
  { 
    SCData = SC_ADPU->Body.LE;
    USART_SendData(USART3, SCData);
    while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
    {
    }   
  }
	else
	{
		SCData = 0x00;
    USART_SendData(USART3, SCData);
    while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
    {
    }
	}

  /* Flush the USART3 DR */
  (void)USART_ReceiveData(USART3);
//	USART_ByteReceive(&locData, SC_Receive_Timeout*2);


  /* --------------------------------------------------------
    Wait Procedure byte from card:
    1 - ACK
    2 - NULL
    3 - SW1; SW2
   -------------------------------------------------------- */
	
  if((USART_ByteReceive(&locData, SC_Receive_Timeout*10)) == SUCCESS)
  {
    if(((locData & (u8)0xF0) == 0x60) || ((locData & (u8)0xF0) == 0x90))
    {
      /* SW1 received */
      SC_ResponceStatus->SW1 = locData;
      if((USART_ByteReceive(&locData, SC_Receive_Timeout)) == SUCCESS)
      {
      /* SW2 received */
        SC_ResponceStatus->SW2 = locData;
      }
    }
		
    else if (((locData & (u8)0xFE) == (((u8)~(SC_ADPU->Header.INS)) & (u8)0xFE))||((locData & (u8)0xFE) == (SC_ADPU->Header.INS & (u8)0xFE)))
    {
      SC_ResponceStatus->Data[0] = locData;/* ACK received */
    }
  }

  /* If no status bytes received ---------------------------------------------*/
  if(SC_ResponceStatus->SW1 == 0x00)
  {
    /* Send body data to SC--------------------------------------------------*/
    if (SC_ADPU->Body.LC)
    {
      for(i = 0; i < SC_ADPU->Body.LC; i++)
      {
        SCData = SC_ADPU->Body.Data[i];        
        USART_SendData(USART3, SCData);
        while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET)
        {
        } 
      }
      /* Flush the USART3 DR */
      (void)USART_ReceiveData(USART3);
      /* Disable the DMA Receive (Reset DMAR bit only) */  
      USART_DMACmd(USART3, USART_DMAReq_Rx, DISABLE);
    }
    /* Or receive body data from SC ------------------------------------------*/
    else if (SC_ADPU->Body.LE)
    {
      for(i = 0; i < SC_ADPU->Body.LE; i++)
      {
        if(USART_ByteReceive(&locData, SC_Receive_Timeout) == SUCCESS)
        {
          SC_ResponceStatus->Data[i] = locData;
        }
      }
    }
    /* Wait SW1 --------------------------------------------------------------*/
    i = 0;
    while(i < 10)
    {
      if(USART_ByteReceive(&locData, SC_Receive_Timeout) == SUCCESS)
      {
        SC_ResponceStatus->SW1 = locData;
        i = 11;
      }
      else
      {
        i++;
      }
    }
    /* Wait SW2 ------------------------------------------------------------*/   
    i = 0;
    while(i < 10)
    {
      if(USART_ByteReceive(&locData, SC_Receive_Timeout) == SUCCESS)
      {
        SC_ResponceStatus->SW2 = locData;
        i = 11;
      }
      else
      {
        i++;
      }
    }
  }
}

/*******************************************************************************
* Function Name  : SC_AnswerReq
* Description    : Requests the reset answer from card.
* Input          : - SCState: pointer to an SC_State enumeration that will contain
*                    the Smartcard state.
*                  - card: pointer to a buffer which will contain the card ATR.
*                  - length: maximum ATR length
* Output         : None
* Return         : None
*******************************************************************************/
void SC_AnswerReq(SC_State *SCstate, u8 *card, u8 length)
{
  u8 Data = 0;
  u32 i = 0;
	u8 volatile *initvalue=card;

  switch(*SCstate)
  {
    case SC_RESET_LOW:
      /* Check responce with reset low ---------------------------------------*/
      for (i = 0; i < 1; i++)
      {
        if((USART_ByteReceive(&Data, SC_Receive_Timeout)) == SUCCESS)
        {
          card[i] = Data;
        }
      }
//      if(initvalue[0])
//      {
//        (*SCstate) = SC_ACTIVE;
//        SC_Reset(Bit_SET);
//      }
//      else
//      {
//        (*SCstate) = SC_RESET_HIGH;
//      }
//		(*SCstate) = SC_RESET_HIGH;
//    break;

//    case SC_RESET_HIGH:
      /* Check responce with reset high --------------------------------------*/
      
			SC_Reset(Bit_SET); /* Reset High */
			//(void)USART_ReceiveData(USART3);
			if((USART_ByteReceive(&Data, SC_Receive_Timeout*20)) == SUCCESS)
        {
          card[0] = Data; /* Receive data for timeout = SC_Receive_Timeout */
        }
			card=card+1;//第一个byte已经接受完成，第一个BYTE的等待时间设置的长一点。
      while(length--)
      {
        if((USART_ByteReceive(&Data, SC_Receive_Timeout)) == SUCCESS)
        {
          *card++ = Data; /* Receive data for timeout = SC_Receive_Timeout */
        }       
      }
			
      if(initvalue[0])
      {
        (*SCstate) = SC_ACTIVE;
      }
      else
      {
        (*SCstate) = SC_POWER_OFF;
      }
    break;

    case SC_ACTIVE:
    break;
    
    case SC_POWER_OFF:
      /* Close Connection if no answer received ------------------------------*/
      SC_Reset(Bit_SET); /* Reset high - a bit is used as level shifter from 3.3 to 5 V */
      SC_PowerCmd(DISABLE);
    break;

    default:
      (*SCstate) = SC_RESET_LOW;
  }
}

/*******************************************************************************
* Function Name  : SC_decode_Answer2reset
* Description    : Decodes the Answer to reset received from card.
* Input          : - Card: pointer to the buffer containing the card ATR.
* Output         : None
* Return         : None
*******************************************************************************/
u8 SC_decode_Answer2reset(u8 *card)
{
  u32 i = 0, flag = 0, buf = 0, protocol = 0;

  SC_A2R.TS = card[0];  /* Initial character */
  SC_A2R.T0 = card[1];  /* Format character */

  SC_A2R.Hlength = SC_A2R.T0 & (u8)0x0F;

  if ((SC_A2R.T0 & (u8)0x80) == 0x80)
  {
    flag = 1;
  }

  for (i = 0; i < 4; i++)
  {
    SC_A2R.Tlength = SC_A2R.Tlength + (((SC_A2R.T0 & (u8)0xF0) >> (4 + i)) & (u8)0x1);
  }

  for (i = 0; i < SC_A2R.Tlength; i++)
  {
    SC_A2R.T[i] = card[i + 2];
  }

  protocol = SC_A2R.T[SC_A2R.Tlength - 1] & (u8)0x0F;

  while (flag)
  {
    if ((SC_A2R.T[SC_A2R.Tlength - 1] & (u8)0x80) == 0x80)
    {
      flag = 1;
    }
    else
    {
      flag = 0;
    }

    buf = SC_A2R.Tlength;
    SC_A2R.Tlength = 0;

    for (i = 0; i < 4; i++)
    {
      SC_A2R.Tlength = SC_A2R.Tlength + (((SC_A2R.T[buf - 1] & (u8)0xF0) >> (4 + i)) & (u8)0x1);
    }
	
    for (i = 0;i < SC_A2R.Tlength; i++)
    {
      SC_A2R.T[buf + i] = card[i + 2 + buf];
    }
    SC_A2R.Tlength += (u8)buf;
  }

  for (i = 0; i < SC_A2R.Hlength; i++)
  {
    SC_A2R.H[i] = card[i + 2 + SC_A2R.Tlength];
  }

  return (u8)protocol;
}

/*******************************************************************************
* Function Name  : SC_Init
* Description    : Initializes all peripheral used for Smartcard interface.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SC_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStructure;
  USART_InitTypeDef USART_InitStructure;
  
  /* Enable GPIOB, GPIOD and GPIOE clocks */
  RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB | RCC_APB2Periph_GPIOD |
                         RCC_APB2Periph_GPIOC, ENABLE);
                         
  /* Enable USART3 clock */
  RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART3, ENABLE);
                           
  /* Configure USART3 CK(PB.12) as alternate function push-pull */
  GPIO_InitStructure.GPIO_Pin = SC_CLK;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  GPIO_Init(SC_CLK_PORT, &GPIO_InitStructure);
  
  /* Configure USART3 Tx (PB.10) as alternate function open-drain */
  GPIO_InitStructure.GPIO_Pin = SC_IO;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_OD;
  GPIO_Init(SC_IO_PORT, &GPIO_InitStructure);

  /* Configure Smartcard Reset  */
  GPIO_InitStructure.GPIO_Pin = SC_RESET;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(SC_RESET_PORT, &GPIO_InitStructure);

  /* Configure Smartcard 3/5V  */
  GPIO_InitStructure.GPIO_Pin = SC_3_5V;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(SC_3_5V_PORT, &GPIO_InitStructure);

  /* Configure Smartcard 1.8V  */
  GPIO_InitStructure.GPIO_Pin = SC_18V;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(SC_18V_PORT, &GPIO_InitStructure);
	
	  /* Configure Smartcard CLKDIV1  */
  GPIO_InitStructure.GPIO_Pin = SC_CLKDIV_1;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(SC_CLKDIV_1_PORT, &GPIO_InitStructure);
	
	  /* Configure Smartcard CLKDIV2 */
  GPIO_InitStructure.GPIO_Pin = SC_CLKDIV_2;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(SC_CLKDIV_2_PORT, &GPIO_InitStructure);

  /* Configure Smartcard CMDVCC  */
  GPIO_InitStructure.GPIO_Pin = SC_CMDVCC;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(SC_CMDVCC_PORT, &GPIO_InitStructure);
	SC_PowerCmd(DISABLE);//VCC=0
	
	/* Configure Smartcard OFFN  */
  GPIO_InitStructure.GPIO_Pin = SC_OFF;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
  GPIO_Init(SC_OFF_PORT, &GPIO_InitStructure);
    
/* USART3 configuration ------------------------------------------------------*/
  /* USART3 configured as follow:
        - Word Length = 9 Bits
        - 0.5 Stop Bit
        - Even parity
        - BaudRate = 12096 baud
        - Hardware flow control disabled (RTS and CTS signals)
        - Tx and Rx enabled
        - USART Clock enabled
  */

  /* USART Clock set to 3.6 MHz (PCLK1 (36 MHZ) / 10) */
  USART_SetPrescaler(USART3, 0x05);
	//USART_SetPrescaler(USART3, 0x1e);
	
  /* USART Guard Time set to 16 Bit */
  USART_SetGuardTime(USART3, 0);
  
  USART_StructInit(&USART_InitStructure);
	//USART_InitStructure.USART_BaudRate = 1613;
  USART_InitStructure.USART_BaudRate = 9677;
	//	USART_InitStructure.USART_BaudRate = 116129;
  USART_InitStructure.USART_WordLength = USART_WordLength_9b;
  USART_InitStructure.USART_StopBits = USART_StopBits_1_5;
  USART_InitStructure.USART_Parity = USART_Parity_Even;
  USART_InitStructure.USART_Clock = USART_Clock_Enable;  
  USART_Init(USART3, &USART_InitStructure);

  /* Enable the USART3 Parity Error Interrupt */
  USART_ITConfig(USART3, USART_IT_PE, ENABLE);

  /* Enable the USART3 Framing Error Interrupt */
  USART_ITConfig(USART3, USART_IT_ERR, ENABLE);

  /* Enable USART3 */
  USART_Cmd(USART3, ENABLE);

  /* Enable the NACK Transmission */
  USART_SmartCardNACKCmd(USART3, ENABLE);

  /* Enable the Smartcard Interface */
  USART_SmartCardCmd(USART3, ENABLE);
  
  /* Set RSTIN LOW */  
  SC_Reset(Bit_RESET);
	
	/* Set  CLKDIV=div1 */
	GPIO_SetBits(SC_CLKDIV_1_PORT,SC_CLKDIV_1);		//CLK div=1
	GPIO_ResetBits(SC_CLKDIV_2_PORT,SC_CLKDIV_2);	//CLK div=1
	 
  /* Select 3V */ 
  SC_VoltageConfig(SC_Voltage_3V);
  	
	SC_PowerCmd(ENABLE);//VCC=1
		
}

/*******************************************************************************
* Function Name  : SC_DeInit
* Description    : Deinitializes all ressources used by the Smartcard interface.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SC_DeInit(void)
{
  /* Disable CMDVCCN */
  SC_PowerCmd(DISABLE);                     
  /* Disable USART3 clock */
  RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART3, DISABLE);
}

/*******************************************************************************
* Function Name  : SC_VoltageConfig
* Description    : Configures the card power voltage.
* Input          : - SC_Voltage: specifies the card power voltage.
*                    This parameter can be one of the following values:
*                        - SC_Voltage_5V: 5V cards.
*                        - SC_Voltage_3V: 3V cards.
* Output         : None
* Return         : None
*******************************************************************************/
/*******************************************************************************
//					Command					EN5V3VN						EN1V8VN
//					Deep ShutDown					0									0
//					Vcc=1.8V							1									0
//					Vcc=3V								0									1
//					Vcc=5V								1									1
//
********************************************************************************/
void SC_VoltageConfig(u32 SC_Voltage)
{
  if(SC_Voltage == SC_Voltage_5V)
  {
    /* Select Smartcard 5V */  
    GPIO_SetBits(SC_18V_PORT, SC_18V);		
    GPIO_SetBits(SC_3_5V_PORT, SC_3_5V);
		
  }
	
  else if(SC_Voltage==SC_Voltage_3V)
  {
    /* Select Smartcard 3V */  
    GPIO_SetBits(SC_18V_PORT, SC_18V);	    
    GPIO_ResetBits(SC_3_5V_PORT, SC_3_5V);
  } 
	else
	{
		/*Select Smartcard 1.8V*/
		GPIO_SetBits(SC_3_5V_PORT, SC_3_5V);
		GPIO_ResetBits(SC_18V_PORT, SC_18V);	
		
	}
}

/*******************************************************************************
* Function Name  : SC_Detect
* Description    : Detects whether the Smartcard is present or not.
* Input          : None. 
* Output         : None.
* Return         : 1 - Smartcard inserted
*                  0 - Smartcard not inserted
*******************************************************************************/
u8 SC_Detect(void)
{
  return 1;
}

/*******************************************************************************
* Function Name  : USART_ByteReceive
* Description    : Receives a new data while the time out not elapsed.
* Input          : None
* Output         : None
* Return         : An ErrorStatus enumuration value:
*                         - SUCCESS: New data has been received
*                         - ERROR: time out was elapsed and no further data is 
*                                  received
*******************************************************************************/
ErrorStatus USART_ByteReceive(u8 *Data, u32 TimeOut)
{
  u32 Counter = 0;

  while((USART_GetFlagStatus(USART3, USART_FLAG_RXNE) == RESET) && (Counter != TimeOut))
  {
    Counter++;
  }

  if(Counter != TimeOut)
  {
    *Data = (u8)USART_ReceiveData(USART3);
    return SUCCESS;    
  }
  else 
  {
    return ERROR;
  }
}

/******************* (C) COPYRIGHT 2007 STMicroelectronics *****END OF FILE****/
