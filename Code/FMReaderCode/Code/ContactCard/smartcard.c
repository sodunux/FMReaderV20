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

SC_ADPU_Commands apdu_commands;
SC_ADPU_Responce apdu_responce;
SC_TimTypeDef sc_tim;
u32 Fi[16]={372,372,558,744,1116,1488,2232,372,372,512,756,1024,1536,2048,372,372};
u8 Di[16]={1,1,2,4,8,16,32,1,12,20,1,1,1,1,1,1};
/* Private function prototypes -----------------------------------------------*/
/* Transport Layer -----------------------------------------------------------*/
/*--------------APDU-----------*/

/*------------ ATR ------------*/
u8 SC_decode_Answer2reset(u8 *card);  /* Decode ATR */

/* Physical Port Layer -------------------------------------------------------*/


/* Private functions ---------------------------------------------------------*/

void SC_uDelay(u32 us) //1us
{
	//72M 
	u32 i,j;
	for(j=0;j<us;j++)
	{
		for(i=0;i<81;i++)
			__nop();
	}
}


void SC_mDelay(u32 ms) //1ms
{
	//72M
	u32 i,j;
	for(j=0;j<ms;j++)
	{
		for(i=0;i<8100;i++)
			__nop();
	}
}




/*******************************************************************************
* Function Name  : SC_PPS
* Description    : Configures the IO speed (BaudRate) communication.
* Input          : PPS1 
* Output         : SC_SUCCESS, SC_ERROR
* Return         : None
*******************************************************************************/
u8 SC_PPS(u8 PPS1)
{
  u8 locData = 0, PTSConfirmStatus = 1;

  /* Enable the DMA Receive (Set DMAR bit only) to enable interrupt generation
     in case of a framing error FE */  
  USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);
  
  if(PPS1!= 0x11)
    {
      /* Send PTSS */
			SC_SendByte(0xFF);
      /* Send PTS0 */
			SC_SendByte(0x10);
      /* Send PTS1 */
			SC_SendByte(PPS1);			
      /* Send PCK */
			SC_SendByte(0xFF^0x10^PPS1);	

      /* Disable the DMA Receive (Reset DMAR bit only) */  
      USART_DMACmd(USART3, USART_DMAReq_Rx, DISABLE);
 
      if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
      {
        if(locData != 0xFF)
        {
           PTSConfirmStatus = 0x00;
        }
      }
			if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
      {
        if(locData != 0xFF)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
      {
        if(locData != 0x10)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
      {
        if(locData != PPS1)
        {
           PTSConfirmStatus = 0x00;
        }
      }
      if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
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
				sc_tim.D=Di[PPS1 & 0x0F];
				sc_tim.F=Fi[(PPS1 >> 4) & 0x0F];
				SC_ETUConfig();
				return SC_SUCCESS;
      }
			else 
				return SC_ERROR;
    }
	return SC_SUCCESS;		
}


	
void SC_ClkCmd(u8 stat)
{
	if(stat)
	{
		USART3->CR2&=USART_CR2_CLKEN;
	}	
	else
		USART3->CR2|=USART_CR2_CLKEN;
}


/*******************************************************************************
* Function Name  : SC_DataTrancive
* Description    : Send Data and Receive Data
* Input 		 : DataBuffer; sendlen,recevlen;
* Return         : SC_SUCCESS, SC_ERROR
*******************************************************************************/
u8 SC_DataTrancive(u8 *buffer,u8 sendlen,u8* recelen)
{
	u8 i;
	u8 ret,locData;
	
	USART_DMACmd(USART3, USART_DMAReq_Rx, ENABLE);
	for(i=0;i<sendlen;i++)
		SC_SendByte(buffer[i]);
	(void)USART_ReceiveData(USART3);
	
	ret=SC_RecvByte(&locData, sc_tim.WaitTimeout);
	if(ret==SC_ERROR)
		return SC_ERROR;
	else
	{
		buffer[0]=locData;
		for(i=1;i<*recelen;i++)
		{
			ret=SC_RecvByte(&locData, sc_tim.WaitTimeout);
			if(ret==SC_SUCCESS)
				buffer[i]=locData;
			else
			{
				*recelen=i;
				return SC_SUCCESS;
			}
		}
		return SC_SUCCESS;
	}

}

void SC_TimConfig(void)
{
		TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
		sc_tim.F=Fi[0];
  	sc_tim.D=Di[0];
  	sc_tim.FreqDiv=SC_CLK_3P6M;
  	sc_tim.GuardTime=0x02; //2etu
  	sc_tim.AtrTimeout=400; //400ETU
  	sc_tim.WaitTimeout=100; //9600ETU
  	sc_tim.clk_cnt=0;
		/*********************TIM3 timeout**********************/
		TIM_TimeBaseStructure.TIM_Period=(sc_tim.F/sc_tim.D)-1;
		TIM_TimeBaseStructure.TIM_Prescaler =(sc_tim.FreqDiv*4)-1;
		TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
		TIM_TimeBaseInit(TIM3, & TIM_TimeBaseStructure);
		TIM_ARRPreloadConfig(TIM3, ENABLE);
		TIM_ITConfig(TIM3,TIM_IT_Update,ENABLE);
		TIM_Cmd(TIM3,ENABLE);
	
}

void SC_Init(void)
{
  	GPIO_InitTypeDef GPIO_InitStructure;
  	USART_InitTypeDef USART_InitStructure;
		TIM_TimeBaseInitTypeDef TIM_TimeBaseStructure;
		
		sc_tim.F=Fi[0];
  	sc_tim.D=Di[0];
  	sc_tim.FreqDiv=SC_CLK_3P6M;
  	sc_tim.GuardTime=0x02; //2etu
  	sc_tim.AtrTimeout=400; //400ETU
  	sc_tim.WaitTimeout=100; //9600ETU
  	sc_tim.clk_cnt=0;

  	/* Enable GPIOB, GPIOD and GPIOE clocks */	
  	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB | RCC_APB2Periph_GPIOD | RCC_APB2Periph_GPIOC, ENABLE);
  	/* Enable USART3 clock */
  	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART3 | RCC_APB1Periph_TIM3, ENABLE);
  
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
		
		/* Configure Smartcard OFFN  */
  	GPIO_InitStructure.GPIO_Pin = SC_OFF;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
  	GPIO_Init(SC_OFF_PORT, &GPIO_InitStructure);
		
  	/* Configure Smartcard CMDVCC  */
  	GPIO_InitStructure.GPIO_Pin = SC_CMDVCC;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  	GPIO_Init(SC_CMDVCC_PORT, &GPIO_InitStructure);
	 
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
  	USART_SetPrescaler(USART3,sc_tim.FreqDiv);
  	/* USART Guard Time set to 2 Bit */
  	USART_SetGuardTime(USART3, sc_tim.GuardTime);  
  	USART_StructInit(&USART_InitStructure);
  	USART_InitStructure.USART_BaudRate = (APB1_FREQ/(sc_tim.FreqDiv*2))/(sc_tim.F/sc_tim.D);
  	USART_InitStructure.USART_WordLength = USART_WordLength_9b;
  	USART_InitStructure.USART_StopBits = USART_StopBits_1_5;
  	USART_InitStructure.USART_Parity = USART_Parity_Even;
  	USART_InitStructure.USART_Clock = USART_Clock_Enable;  
  	USART_Init(USART3, &USART_InitStructure);
  	/* Enable the USART3 Parity Error Interrupt */
  	USART_ITConfig(USART3, USART_IT_PE, ENABLE);
  	/* Enable the USART3 Framing Error Interrupt */
  	USART_ITConfig(USART3, USART_IT_ERR, ENABLE);
  	/* ENABLE USART3 */
  	USART_Cmd(USART3, ENABLE);	
  	/* Enable the NACK Transmission */
  	USART_SmartCardNACKCmd(USART3, ENABLE);
  	/* Enable the Smartcard Interface */
  	USART_SmartCardCmd(USART3, ENABLE);
		/*********************TIM3 timeout**********************/
//		TIM_TimeBaseStructure.TIM_Period=(sc_tim.F/sc_tim.D)-1;
//		TIM_TimeBaseStructure.TIM_Prescaler =(sc_tim.FreqDiv*4)-1;
//		TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
//		TIM_TimeBaseInit(TIM3, & TIM_TimeBaseStructure);
//		TIM_ARRPreloadConfig(TIM3, ENABLE);
//		TIM_ITConfig(TIM3,TIM_IT_Update,ENABLE);
//		TIM_Cmd(TIM3,ENABLE);

  	SC_VoltageConfig(SC_Voltage_3V);
  	/* Set  CLKDIV=div1 */
		GPIO_SetBits(SC_CLKDIV_1_PORT,SC_CLKDIV_1);		//CLK div=1
		GPIO_ResetBits(SC_CLKDIV_2_PORT,SC_CLKDIV_2);	//CLK div=1
		
		SC_ResetOFF;		
		SC_PowerOFF;
		SC_mDelay(100);
//		SC_PowerOFF;
//		SC_mDelay(100);
  	SC_VoltageConfig(SC_Voltage_3V);		
}



u8 SC_ColdReset(u8 *atr,u8 *len)
{	
	u8 ret,i;
	
  USART_InitTypeDef USART_InitStructure;
	sc_tim.F=Fi[0];
  sc_tim.D=Di[0];
  sc_tim.FreqDiv=SC_CLK_3P6M;
  sc_tim.GuardTime=0x02; //2etu
  sc_tim.AtrTimeout=400; //400ETU
  sc_tim.WaitTimeout=100; //100ETU
  sc_tim.clk_cnt=0;
	SC_ETUConfig();
	
	SC_mDelay(10);	
	SC_PowerOFF;
	SC_ResetOFF;
	SC_mDelay(10);	
	SC_PowerON;
	//SC_ClkCmd(1);
	SC_mDelay(5);
	SC_ResetON;	
	ret=SC_RecvByte(atr,sc_tim.AtrTimeout);
	if(ret==SC_ERROR)
		return SC_ERROR;
	else
	{
		for(i=1;i<*len;i++)
			SC_RecvByte(atr+i,20);

		SC_decode_Answer2reset(atr);
		if((SC_A2R.TS==0x3B)||(SC_A2R.TS==0x3F))
		{
			*len=SC_A2R.Hlength+SC_A2R.Tlength+2;
			return SC_SUCCESS;
		}
		else
		{
			return SC_ERROR;
		}


	}
}


u8 SC_WarmReset(u8 *atr,u8 *len)
{
	u8 ret,i;
	
  USART_InitTypeDef USART_InitStructure;
	sc_tim.F=Fi[0];
  sc_tim.D=Di[0];
  sc_tim.FreqDiv=SC_CLK_3P6M;
  sc_tim.GuardTime=0x02; //2etu
  sc_tim.AtrTimeout=400; //400ETU
  sc_tim.WaitTimeout=100; //9600ETU
  sc_tim.clk_cnt=0;
	SC_ETUConfig();
	
	SC_ResetOFF;
	SC_mDelay(5);
	SC_ResetON;	
	ret=SC_RecvByte(atr,sc_tim.AtrTimeout);		
	if(ret==SC_ERROR)
		return SC_ERROR;
	else
	{
		for(i=1;i<*len;i++)
			SC_RecvByte(atr+i,20);
		SC_decode_Answer2reset(atr);
		if((SC_A2R.TS==0x3B)||(SC_A2R.TS==0x3F))
		{
			*len=SC_A2R.Hlength+SC_A2R.Tlength+2;
			return SC_SUCCESS;
		}
		else
		{
			return SC_ERROR;
		}
	}
}



/*******************************************************************************
* Function Name  : SC_ApduExchange
* Description    : Manages the Smartcard transport layer: send APDU commands and
*                  receives the APDU responce.
* Input          : - SC_ADPU: pointer to a SC_ADPU_Commands structure which 
*                    will be initialized.  
*                  - SC_Response: pointer to a SC_ADPU_Responce structure which 
*                    will be initialized.
* Output         : None
* Return         : None
*******************************************************************************/
void SC_ApduExchange(SC_ADPU_Commands *SC_ADPU, SC_ADPU_Responce *SC_ResponceStatus)
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
	SC_SendByte(SC_ADPU->Header.CLA);
	SC_SendByte(SC_ADPU->Header.INS);	
	SC_SendByte(SC_ADPU->Header.P1);
	SC_SendByte(SC_ADPU->Header.P2);
	  
  /* Send body length to/from SC ---------------------------------------------*/
  if(SC_ADPU->Body.LC)
  {
		SC_SendByte(SC_ADPU->Body.LC);
  }
  else if(SC_ADPU->Body.LE)
  { 
		SC_SendByte(SC_ADPU->Body.LE); 
  }
	else
	{
		SC_SendByte(0x00); 
	}

  /* Flush the USART3 DR */
  (void)USART_ReceiveData(USART3);

  /* --------------------------------------------------------
    Wait Procedure byte from card:
    1 - ACK
    2 - NULL
    3 - SW1; SW2
   -------------------------------------------------------- */
	
  if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
  {
		while(locData==0x60)
		{
			locData=0;
			SC_RecvByte(&locData, sc_tim.WaitTimeout);
		}
					
    if(((locData & (u8)0xF0) == 0x60) || ((locData & (u8)0xF0) == 0x90))
    {
      /* SW1 received */			
      SC_ResponceStatus->SW1 = locData;
      if((SC_RecvByte(&locData, sc_tim.WaitTimeout)) == SC_SUCCESS)
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
				SC_SendByte(SC_ADPU->Body.Data[i]);  
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
        if(SC_RecvByte(&locData, sc_tim.WaitTimeout) == SC_SUCCESS)
        {
          SC_ResponceStatus->Data[i] = locData;
        }
      }
    }
	}
  /* Wait SW1 --------------------------------------------------------------*/
	while(locData==0x60)
	{
		locData=0;
		SC_RecvByte(&locData, sc_tim.WaitTimeout);
	}
	
	if(SC_RecvByte(&locData, sc_tim.WaitTimeout) == SC_SUCCESS)
	{
		SC_ResponceStatus->SW1 = locData;
	 
	}


/* Wait SW2 ------------------------------------------------------------*/   


	if(SC_RecvByte(&locData, sc_tim.WaitTimeout) == SC_SUCCESS)
	{
		SC_ResponceStatus->SW2 = locData;
	 
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
	SC_A2R.Hlength=0;
	SC_A2R.Tlength=0;
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



void SC_SendByte(u8 dat)
{
    USART_SendData(USART3, dat);
    while(USART_GetFlagStatus(USART3, USART_FLAG_TC) == RESET);
}

u8 SC_RecvByte(u8 *dat,u32 TimeOut)
{
	sc_tim.clk_cnt=0;
	while((USART_GetFlagStatus(USART3, USART_FLAG_RXNE) == RESET) && (sc_tim.clk_cnt < TimeOut));
	if(sc_tim.clk_cnt < TimeOut)
	{
		*dat = (u8)USART_ReceiveData(USART3);
		return SC_SUCCESS;
	}
	else 
		return SC_ERROR;
}



/*******************************************************************************
* Function Name  : SC_EtuConfig
* Description    : Configures ETU ?CLK Frequency and GuardTime
* Output         : None
* Return         : None
*******************************************************************************/

void SC_ETUConfig()
{
		USART_InitTypeDef USART_InitStructure;
		USART_SetPrescaler(USART3,sc_tim.FreqDiv);
		USART_SetGuardTime(USART3, sc_tim.GuardTime);  
  	USART_StructInit(&USART_InitStructure);
  	USART_InitStructure.USART_BaudRate = (APB1_FREQ/(sc_tim.FreqDiv*2))/(sc_tim.F/sc_tim.D);
  	USART_InitStructure.USART_WordLength = USART_WordLength_9b;
  	USART_InitStructure.USART_StopBits = USART_StopBits_1_5;
  	USART_InitStructure.USART_Parity = USART_Parity_Even;
  	USART_InitStructure.USART_Clock = USART_Clock_Enable;  
  	USART_Init(USART3, &USART_InitStructure);
}

/*******************************************************************************
* Function Name  : SC_InterruptHandler.
* Description    : TIM3 interrupt handler.
* Input          : None 
* Output         : None
* Return         : None
*******************************************************************************/
void SC_InterruptHandler(void)
{
	static u8 tmp;
	sc_tim.clk_cnt++;
	TIM_ClearFlag(TIM3,TIM_FLAG_Update|TIM_FLAG_CC1|TIM_FLAG_CC2|TIM_FLAG_CC3|TIM_FLAG_CC4);
	if((sc_tim.clk_cnt%100)==0)
	{
		if(tmp)
		{
			GPIO_SetBits(GPIOB, GPIO_Pin_7);
			tmp=0;
		}
		else 
		{
			GPIO_ResetBits(GPIOB, GPIO_Pin_7);
			tmp=1;			
		}
		
	}
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
  SC_PowerOFF;                    
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




/******************* (C) COPYRIGHT 2007 STMicroelectronics *****END OF FILE****/
