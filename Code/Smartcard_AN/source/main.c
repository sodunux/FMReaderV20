/******************** (C) COPYRIGHT 2007 STMicroelectronics ********************
* File Name          : main.c
* Author             : MCD Application Team
* Version            : V1.0
* Date               : 10/08/2007
* Description        : Main program body.
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
/* Directories & Files ID */
uc8 MasterRoot[2] = {0x3F, 0x00};
uc8 GSMDir[2] = {0x7F, 0x20};
uc8 ICCID[2] = {0x2F, 0xE2};
uc8 IMSI[2] = {0x6F, 0x07};
uc8 CHV1[8] = {'0', '0', '0', '0', '0', '0', '0', '0'};

/* APDU Transport Structures */
SC_ADPU_Commands SC_ADPU;
SC_ADPU_Responce SC_Responce;
vu32 TimingDelay = 0;
static volatile ErrorStatus HSEStartUpStatus = SUCCESS;
vu32 CardInserted = 0;
u32 CHV1Status = 0, i = 0;
vu8 ICCID_Content[10] = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
vu8 IMSI_Content[9] = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

/* Private function prototypes -----------------------------------------------*/
static void RCC_Configuration(void);
static void NVIC_Configuration(void);
static void EXTI_Configuration(void);
static void GPIO_Configuration(void);
void SysTick_Config(void);
void Delay(u32 nCount);

/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : main
* Description    : Main program.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
int main(void)
{
  SC_State SCState = SC_POWER_OFF;

#ifdef DEBUG
  debug();
#endif
     
  /* System Clocks Configuration */
  RCC_Configuration();
       
  /* NVIC configuration */
  NVIC_Configuration();

  /* SysTick configuration */
  SysTick_Config();

  /* Configure the GPIO ports */
  GPIO_Configuration();

  /* Configure the EXTI Controller */
  EXTI_Configuration();
  
/*-------------------------------- Idle task ---------------------------------*/
  while(1)
  {
    /* Loop while no Smartcard is detected */  
    while(CardInserted == 0)
    {
    }
       
    /* Start SC Demo ---------------------------------------------------------*/
    
    /* Wait A2R --------------------------------------------------------------*/
    SCState = SC_POWER_ON;

    SC_ADPU.Header.CLA = 0x00;
    SC_ADPU.Header.INS = SC_GET_A2R;
    SC_ADPU.Header.P1 = 0x00;
    SC_ADPU.Header.P2 = 0x00;
    SC_ADPU.Body.LC = 0x00;
   
    while(SCState != SC_ACTIVE_ON_T0) 
    {
      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Apply the Procedure Type Selection (PTS) */
    SC_PTSConfig();

    /* Inserts delay(400ms) for Smartcard clock resynchronisation */
    Delay(40);

    /* Select MF -------------------------------------------------------------*/    
    SC_ADPU.Header.CLA = SC_CLA_GSM11;
    SC_ADPU.Header.INS = SC_SELECT_FILE;
    SC_ADPU.Header.P1 = 0x00;
    SC_ADPU.Header.P2 = 0x00;
    SC_ADPU.Body.LC = 0x02;

    for(i = 0; i < SC_ADPU.Body.LC; i++)
    {
      SC_ADPU.Body.Data[i] = MasterRoot[i];
    }
    while(i < LCmax) 
    {    
      SC_ADPU.Body.Data[i++] = 0;
    }
    SC_ADPU.Body.LE = 0;

    SC_Handler(&SCState, &SC_ADPU, &SC_Responce);

    /* Get Response on MF ----------------------------------------------------*/    
    if(SC_Responce.SW1 == SC_DF_SELECTED)
    {
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_GET_RESPONCE;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x00;
      SC_ADPU.Body.LE = SC_Responce.SW2;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Select ICCID ----------------------------------------------------------*/    
    if(((SC_Responce.SW1 << 8) | (SC_Responce.SW2)) == SC_OP_TERMINATED)
    {
      /* Check if the CHV1 is enabled */   
      if((SC_Responce.Data[13] & 0x80) == 0x00)
      {
        CHV1Status = 0x01;
      }
      /* Send APDU Command for ICCID selection */
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_SELECT_FILE;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x02;

      for(i = 0; i < SC_ADPU.Body.LC; i++)
      {
        SC_ADPU.Body.Data[i] = ICCID[i];
      }
      while(i < LCmax) 
      {    
        SC_ADPU.Body.Data[i++] = 0;
      }
      SC_ADPU.Body.LE = 0;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Read Binary in ICCID --------------------------------------------------*/
    if(SC_Responce.SW1 == SC_EF_SELECTED)
    {
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_READ_BINARY;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x00;

      SC_ADPU.Body.LE = 10;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Select GSMDir ---------------------------------------------------------*/    
    if(((SC_Responce.SW1 << 8) | (SC_Responce.SW2)) == SC_OP_TERMINATED)
    {
      /* Copy the ICCID File content into ICCID_Content buffer */
      for(i = 0; i < SC_ADPU.Body.LE; i++)
      {
        ICCID_Content[i] =  SC_Responce.Data[i];
      }
      /* Send APDU Command for GSMDir selection */ 
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_SELECT_FILE;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x02;

      for(i = 0; i < SC_ADPU.Body.LC; i++)
      {
        SC_ADPU.Body.Data[i] = GSMDir[i];
      }
      while(i < LCmax) 
      {    
        SC_ADPU.Body.Data[i++] = 0;
      }
      SC_ADPU.Body.LE = 0;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Select IMSI -----------------------------------------------------------*/    
    if(SC_Responce.SW1 == SC_DF_SELECTED)
    {
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_SELECT_FILE;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x02;

      for(i = 0; i < SC_ADPU.Body.LC; i++)
      {
        SC_ADPU.Body.Data[i] = IMSI[i];
      }
      while(i < LCmax) 
      {    
        SC_ADPU.Body.Data[i++] = 0;
      }
      SC_ADPU.Body.LE = 0;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Get Response on IMSI File ---------------------------------------------*/    
    if(SC_Responce.SW1 == SC_EF_SELECTED)
    {
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_GET_RESPONCE;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x00;
      SC_ADPU.Body.LE = SC_Responce.SW2;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }

    /* Read Binary in IMSI ---------------------------------------------------*/
    if(CHV1Status == 0x00)
    {
      if(((SC_Responce.SW1 << 8) | (SC_Responce.SW2)) == SC_OP_TERMINATED)
      {
        /* Enable CHV1 (PIN1) ------------------------------------------------*/
        SC_ADPU.Header.CLA = SC_CLA_GSM11;
        SC_ADPU.Header.INS = SC_ENABLE;
        SC_ADPU.Header.P1 = 0x00;
        SC_ADPU.Header.P2 = 0x01;
        SC_ADPU.Body.LC = 0x08;
 
        for(i = 0; i < SC_ADPU.Body.LC; i++)
        {
          SC_ADPU.Body.Data[i] = CHV1[i];
        }
        while(i < LCmax) 
        {    
          SC_ADPU.Body.Data[i++] = 0;
        }
        SC_ADPU.Body.LE = 0;

        SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
      }
    }
    else
    {
      if(((SC_Responce.SW1 << 8) | (SC_Responce.SW2)) == SC_OP_TERMINATED)
      {
        /* Verify CHV1 (PIN1) ------------------------------------------------*/
        SC_ADPU.Header.CLA = SC_CLA_GSM11;
        SC_ADPU.Header.INS = SC_VERIFY;
        SC_ADPU.Header.P1 = 0x00;
        SC_ADPU.Header.P2 = 0x01;
        SC_ADPU.Body.LC = 0x08;

        for(i = 0; i < SC_ADPU.Body.LC; i++)
        {
          SC_ADPU.Body.Data[i] = CHV1[i];
        }
        while(i < LCmax) 
        {    
          SC_ADPU.Body.Data[i++] = 0;
        }
        SC_ADPU.Body.LE = 0;

        SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
      }
    }
    /* Read Binary in IMSI ---------------------------------------------------*/
    if(((SC_Responce.SW1 << 8) | (SC_Responce.SW2)) == SC_OP_TERMINATED)
    {
      SC_ADPU.Header.CLA = SC_CLA_GSM11;
      SC_ADPU.Header.INS = SC_READ_BINARY;
      SC_ADPU.Header.P1 = 0x00;
      SC_ADPU.Header.P2 = 0x00;
      SC_ADPU.Body.LC = 0x00;

      SC_ADPU.Body.LE = 9;

      SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    }
    if(((SC_Responce.SW1 << 8) | (SC_Responce.SW2)) == SC_OP_TERMINATED)
    {
      /* Copy the IMSI File content into IMSI_Content buffer */
      for(i = 0; i < SC_ADPU.Body.LE; i++)
      {
        IMSI_Content[i] =  SC_Responce.Data[i];
      }
    }
    /* Disable the Smartcard interface */
    SCState = SC_POWER_OFF;
    SC_Handler(&SCState, &SC_ADPU, &SC_Responce);
    CardInserted = 0;
  }
}

/*******************************************************************************
* Function Name  : RCC_Configuration
* Description    : Configures the different system clocks.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
static void RCC_Configuration(void)
{   
  /* RCC system reset(for debug purpose) */
  RCC_DeInit();
 
  /* Enable HSE */
  RCC_HSEConfig(RCC_HSE_ON);
 
  /* Wait till HSE is ready */
  HSEStartUpStatus = RCC_WaitForHSEStartUp();
 
  if(HSEStartUpStatus == SUCCESS)
  {
    /* Enable Prefetch Buffer */
    FLASH_PrefetchBufferCmd(FLASH_PrefetchBuffer_Enable);

    /* Flash 2 wait state */
    FLASH_SetLatency(FLASH_Latency_2);
    
    /* HCLK = SYSCLK */
    RCC_HCLKConfig(RCC_SYSCLK_Div1); 
  
    /* PCLK2 = HCLK */
    RCC_PCLK2Config(RCC_HCLK_Div1); 
 
    /* PCLK1 = HCLK/2 */
    RCC_PCLK1Config(RCC_HCLK_Div2);
 
    /* PLLCLK = 8MHz * 9 = 72 MHz */
    RCC_PLLConfig(RCC_PLLSource_HSE_Div1, RCC_PLLMul_9);
 
    /* Enable PLL */ 
    RCC_PLLCmd(ENABLE);
 
    /* Wait till PLL is ready */
    while(RCC_GetFlagStatus(RCC_FLAG_PLLRDY) == RESET)
    {
    }
 
    /* Select PLL as system clock source */
    RCC_SYSCLKConfig(RCC_SYSCLKSource_PLLCLK);
 
    /* Wait till PLL is used as system clock source */
    while(RCC_GetSYSCLKSource() != 0x08)
    {
    }
  }
  /* Enable GPIOC, GPIOE and AFIO clocks */
  RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC | RCC_APB2Periph_GPIOE |
                         RCC_APB2Periph_AFIO, ENABLE);
}

/*******************************************************************************
* Function Name  : NVIC_Configuration
* Description    : Configures the nested vectored interrupt controller.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
static void NVIC_Configuration(void)
{
  NVIC_InitTypeDef NVIC_InitStructure;

#ifdef  VECT_TAB_RAM  
  /* Set the Vector Table base location at 0x20000000 */ 
  NVIC_SetVectorTable(NVIC_VectTab_RAM, 0x0); 
#else  /* VECT_TAB_FLASH  */
  /* Set the Vector Table base location at 0x08000000 */ 
  NVIC_SetVectorTable(NVIC_VectTab_FLASH, 0x0);   
#endif
  
  /* Configure one bit for preemption priority */
  NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);
  
  /* Clear the EXTI15_10_IRQChannel Pending Bit */
  NVIC_ClearIRQChannelPendingBit(EXTI15_10_IRQChannel);

  NVIC_InitStructure.NVIC_IRQChannel = EXTI15_10_IRQChannel;
  NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
  NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
  NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
  NVIC_Init(&NVIC_InitStructure);

  /* Enable the USART3 Interrupt */
  NVIC_InitStructure.NVIC_IRQChannel = USART3_IRQChannel;
  NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
  NVIC_Init(&NVIC_InitStructure);
}

/*******************************************************************************
* Function Name  : EXTI_Configuration
* Description    : Configures the External Interrupts controller.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
static void EXTI_Configuration(void)
{
  EXTI_InitTypeDef EXTI_InitStructure;

  /* Smartcard OFF (PE.14) */
  GPIO_EXTILineConfig(GPIO_PortSourceGPIOE, GPIO_PinSource14);

    /* Clear EXTI Line 14 Pending Bit */
  EXTI_ClearITPendingBit(EXTI_Line14);
  
  EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt;
  EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Rising;
  EXTI_InitStructure.EXTI_Line = EXTI_Line14;
  EXTI_InitStructure.EXTI_LineCmd = ENABLE;
  EXTI_Init(&EXTI_InitStructure);
}

/*******************************************************************************
* Function Name  : GPIO_Configuration
* Description    : Configures the different GPIO ports.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
static void GPIO_Configuration(void)
{
  GPIO_InitTypeDef GPIO_InitStructure;
    
  /* Configure PC.06, PC.07, PC.08 and PC.09 as output push-pull */
  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6 | GPIO_Pin_7 | GPIO_Pin_8 | GPIO_Pin_9;
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
  GPIO_Init(GPIOC, &GPIO_InitStructure);

  /* Configure Smartcard OFF  */
  GPIO_InitStructure.GPIO_Pin = SC_OFF;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
  GPIO_Init(GPIOE, &GPIO_InitStructure);
}

/*******************************************************************************
* Function Name  : SysTick_Config
* Description    : Configure a SysTick Base time to 10 ms.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void SysTick_Config(void)
{
  /* Configure HCLK clock as SysTick clock source */
  SysTick_CLKSourceConfig(SysTick_CLKSource_HCLK);
 
  /* SysTick interrupt each 100 Hz with HCLK equal to 72MHz */
  SysTick_SetReload(720000);

  /* Enable the SysTick Interrupt */
  SysTick_ITConfig(ENABLE);
}

/*******************************************************************************
* Function Name  : Delay
* Description    : Inserts a delay time.
* Input          : nCount: specifies the delay time length (time base 10 ms).
* Output         : None
* Return         : None
*******************************************************************************/
void Delay(u32 nCount)
{
  TimingDelay = nCount;

  /* Enable the SysTick Counter */
  SysTick_CounterCmd(SysTick_Counter_Enable);
  
  while(TimingDelay != 0)
  {
  }

  /* Disable the SysTick Counter */
  SysTick_CounterCmd(SysTick_Counter_Disable);

  /* Clear the SysTick Counter */
  SysTick_CounterCmd(SysTick_Counter_Clear);
}

#ifdef  DEBUG
/*******************************************************************************
* Function Name  : assert_failed
* Description    : Reports the name of the source file and the source line number
*                  where the assert_param error has occurred.
* Input          : - file: pointer to the source file name
*                  - line: assert_param error line source number
* Output         : None
* Return         : None
*******************************************************************************/
void assert_failed(u8* file, u32 line)
{ 
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */

  /* Infinite loop */
  while (1)
  {
  }
}
#endif

/******************* (C) COPYRIGHT 2007 STMicroelectronics *****END OF FILE****/
