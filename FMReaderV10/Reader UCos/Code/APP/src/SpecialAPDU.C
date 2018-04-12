#include "includes.h"
#include "CCID.h"
#include "SpecialAPDU.h" 
#include "SPI_IIC.h" 
#include "timer1.h"

#ifdef CONTACT_CARD_ENABLE
  #include "ContactCard.h"
  #include "TDA8007.h"
#endif
#ifdef CONTACTLESS_CARD_ENABLE
  #include "PICCCmdConst.h"
  #include "ContactlessCard.h"
  #include "FM1715.h"
  #include "ricreg.h"
  #include "MfErrNo.h"
#endif                

#define INDEX_INS 0x01
#define INDEX_P1  0x02
#define INDEX_P2  0x03
#define INDEX_P3  0x04
#define INDEX_DATA  0x05

InternalRegister stuInRegs;
uint8_t abIntBuffer[1024*4];    // Internal Buffer.
uint8_t InitCmd04Flag=0;
uint8_t InitCmd00Flag=0;

uint8_t I2C_Addr_Len;		// 2013.9.11	Hong

extern uint16_t const FrameSize[9];
extern bool bTimeOut;
extern uint32_t i2cEventGroup[512];
extern  unsigned char PRTCL;	 			//0表示现在的17寄存器设置为TypeA;1表示TypeB
extern  uint8_t CLcard17regInit(void);
extern void TDA8007ReadAllRegs(void);
/*******************************************************************************
* Function Name  : InRegsInit.
* Description    : Initialize member of stuInRegs.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void InRegsInit(void)
{
  stuInRegs.bCurrentDevice = CURRENT_DEVICE_TDA8007;
  // stuInRegs.bCurrentDevice = CURRENT_DEVICE_FM1715;
  stuInRegs.bATRSource = ATR_SOURCE_CARD;  
  stuInRegs.lTriggleDelay = 0;
  stuInRegs.lTriggleWidth = 0;
  stuInRegs.bTriggleStatus = 0;
  stuInRegs.bCounterStatus = COUNTER_DISABLE;
  stuInRegs.lCounterValue = 0;
  stuInRegs.bTmoStatus = TMO_STANDARD;
  stuInRegs.bVoltage = CARD_VCC_3V;
  stuInRegs.bCardPresent = SA_CARD_ABSENT;
  stuInRegs.bIsPoweredOnCT = 1;//0;
}

/*******************************************************************************
* Function Name  : SetStatusWords.
* Description    : Put status words into array.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   SW : Two bytes status words.              
* Output         : None
* Return         : None                   
*******************************************************************************/
void SetStatusWords(uint8_t *APDUBuffer, uint16_t SW)
{
  *APDUBuffer = (uint8_t)((SW>>8)&0xFF);
  *(APDUBuffer+1) = (uint8_t)(SW&0xFF);
}

/*******************************************************************************
* Function Name  : PWMOutConfig.
* Description    : Config the PWM output of MCU, using TIM2, output pin is PA1(CH2)
* Input          : ClkFrq : Clock frequency.
* Output         : None
* Return         : None
*******************************************************************************/
void PWMOutConfig(uint32_t ClkFrq)
{
  uint16_t  TimerPeriod;
  uint16_t  Channel2Pulse;
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
  TIM_OCInitTypeDef  TIM_OCInitStructure;
   
  /* Compute the value to be set in ARR regiter to generate signal frequency at 17.57 Khz */
  TimerPeriod = (SystemCoreClock / ClkFrq ) - 1;
  /* Compute CCR1 value to generate a duty cycle at 50% for channel 1 and 1N */
  Channel2Pulse = (uint16_t) (((uint32_t) 5 * (TimerPeriod - 1)) / 10);

  /* Time Base configuration */
  TIM_TimeBaseStructure.TIM_Prescaler = 0;
  TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
  TIM_TimeBaseStructure.TIM_Period = TimerPeriod;
  TIM_TimeBaseStructure.TIM_ClockDivision = 0;
  TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
  TIM_TimeBaseInit(TIM2, &TIM_TimeBaseStructure);

  /* Channel 1, 2,3 and 4 Configuration in PWM mode */
  TIM_OCInitStructure.TIM_OCMode = TIM_OCMode_PWM2;
  TIM_OCInitStructure.TIM_OutputState = TIM_OutputState_Enable;
  TIM_OCInitStructure.TIM_OutputNState = TIM_OutputNState_Disable;
  TIM_OCInitStructure.TIM_Pulse = Channel2Pulse;
  TIM_OCInitStructure.TIM_OCPolarity = TIM_OCPolarity_Low;
  TIM_OCInitStructure.TIM_OCNPolarity = TIM_OCNPolarity_High;
  TIM_OCInitStructure.TIM_OCIdleState = TIM_OCIdleState_Set;
  TIM_OCInitStructure.TIM_OCNIdleState = TIM_OCIdleState_Reset;

  TIM_OC2Init(TIM2, &TIM_OCInitStructure);

  /* TIM2 counter enable */
  TIM_Cmd(TIM2, ENABLE);

  /* TIM2 Main Output Enable */
  TIM_CtrlPWMOutputs(TIM2, ENABLE);
}

/*******************************************************************************
* Function Name  : TIM3Config.
* Description    : Config the TIM3 of MCU used for delay and width timer.
* Input          : ClkFrq : Clock frequency.
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3Config(void)
{
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
  TIM_OCInitTypeDef  TIM_OCInitStructure;
  /* ---------------------------------------------------------------
    TIM2 Configuration: Output Compare Timing Mode:
    TIM2 counter clock at 1 MHz
  --------------------------------------------------------------- */  

  /* Time base configuration */
  TIM_TimeBaseStructure.TIM_Period = 65535;
  TIM_TimeBaseStructure.TIM_Prescaler = 0;
  TIM_TimeBaseStructure.TIM_ClockDivision = 0;
  TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;

  TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure);

  /* Prescaler configuration */
  TIM_PrescalerConfig(TIM3, 71, TIM_PSCReloadMode_Immediate);

  /* Output Compare Timing Mode configuration: Channel1 */
  TIM_OCInitStructure.TIM_OCMode = TIM_OCMode_Timing;
  TIM_OCInitStructure.TIM_OutputState = TIM_OutputState_Enable;
  if(stuInRegs.iTimerCyc)
    TIM_OCInitStructure.TIM_Pulse = 65535;
  else
    TIM_OCInitStructure.TIM_Pulse = stuInRegs.iTimerLast;
  TIM_OCInitStructure.TIM_OCPolarity = TIM_OCPolarity_High;

  TIM_OC1Init(TIM3, &TIM_OCInitStructure);

  TIM_OC1PreloadConfig(TIM3, TIM_OCPreload_Disable);
}

/*******************************************************************************
* Function Name  : TIM3Start.
* Description    : Start the TIM3 of MCU used for delay and width timer.
* Input          : None.
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3Start(void)
{
  /* TIM IT enable */
  TIM_ITConfig(TIM3, TIM_IT_CC1, ENABLE);

  /* TIM2 enable counter */
  TIM_Cmd(TIM3, ENABLE);
}

extern  uint8_t bTimeoutOpen;
/*******************************************************************************
* Function Name  : SpecialAPDU.
* Description    : Special APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.
                   APDURecvLen  : How many bytes received from card.
* Output         : None
* Return         : None                   
*******************************************************************************/
void SpecialAPDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	uint8_t   bINS = *(APDUBuffer+INDEX_INS);
	uint8_t   bP1 = *(APDUBuffer+INDEX_P1);
	uint8_t   bP2 = *(APDUBuffer+INDEX_P2);
	uint16_t   bP3 = *(APDUBuffer+INDEX_P3);		
	uint8_t   CMD1 = (*(APDUBuffer+INDEX_DATA));
	uint8_t   CMD2 = (*(APDUBuffer+INDEX_DATA+1));
	uint8_t   CMD3 = (*(APDUBuffer+INDEX_DATA+2));
	uint8_t   CMD4 = (*(APDUBuffer+INDEX_DATA+3));
	uint8_t   CMD5 = (*(APDUBuffer+INDEX_DATA+4));
	uint32_t  lTemp;
	uint16_t  iTemp;
	uint8_t   bTemp;
	uint32_t  iNum,i;			
	uint8_t   b;
	int16_t	bStatus;		

	uint8_t	bT0;

	switch(bINS)   // INS
	{
		case INS_SELECT_DEVICE:
			if(bP3 == 0x01)
			{
				if((bP1 == 0x00) && (bP2 == 0x00))
				{
					b = (*(APDUBuffer+5));        
					if((b == 0x11) || (b == 0x21) || (b == 0x31) || (b == CURRENT_DEVICE_FM1715))
					{                                                        
						stuInRegs.bCurrentDevice = (b&0x0F);
						if(b&0x01)  // Contact card.
						{
							//           stuInRegs.bVoltage = ((b>>4)&0x0F);
							//           CLCardPowerOff();
							bTimeoutOpen = 0;   // Suppose card will be inserted into slot again.
						}
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					SetStatusWords(APDUBuffer, 0x6F00);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;
			break;

#ifdef CONTACT_CARD_ENABLE
		case INS_SELECT_CLOCK:
			if((bP1 == 0x00) && (bP2 > 0) && (bP2 < 3))
			{
				if((bP2 == 1) && (bP3 == 4))    // PWM output configuration
				{ 
					TDA8007WriteReg(RegClockConfiguration, 0x07);   // Internal Clock
					while(TDA8007ReadReg(RegMixedStatus)&0x80 == 0);

					lTemp = 0;
					for(b=0; b<4; b++)
					{
						lTemp <<= 8;
						lTemp |= (*(APDUBuffer+INDEX_DATA+b));
					}
					TIM_CtrlPWMOutputs(TIM2, DISABLE);   // Disables PWM output
					TIM_Cmd(TIM2, DISABLE);
					PWMOutConfig(lTemp);

					for(lTemp=10000; lTemp>0; lTemp--);
					TDA8007WriteReg(RegClockConfiguration, 0x00);   // External Clock
					while(TDA8007ReadReg(RegMixedStatus)&0x80 == 1);

					SetStatusWords(APDUBuffer, 0x9000);
				}
				else if((bP2 == 2) && (bP3 == 1)) // TDA8007 internal register configuration
				{
					b = (*(APDUBuffer+INDEX_DATA));
					TDA8007WriteReg(RegClockConfiguration, b);
					if(b&0x04)  // Fext -> Fint
					{
						while(TDA8007ReadReg(RegMixedStatus)&0x80 == 0);
					}
					else
					{
						while(TDA8007ReadReg(RegMixedStatus)&0x80 == 1);
					}
					SetStatusWords(APDUBuffer, 0x9000);
				}
				else
					SetStatusWords(APDUBuffer, 0x6700);  
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);  
			*APDURecvLen = 2;
			break;
#endif

		case INS_TRIGGLE_LEVEL:
			if(bP3 == 0x01)
			{
				if((bP1 > 0) && (bP1 < 5) && (bP2 == 0))
				{
					if((*(APDUBuffer+INDEX_DATA)) < 0x02)
					{
						if(bP1 == 0x01)
							GPIO_WriteBit(TRIG_PORT, TRIG_PIN_1, (BitAction)(*(APDUBuffer+5)));
						else if(bP1 == 0x02)
							GPIO_WriteBit(TRIG_PORT, TRIG_PIN_2, (BitAction)(*(APDUBuffer+5)));
						else if(bP1 == 0x03)
							GPIO_WriteBit(TRIG_PORT, TRIG_PIN_3, (BitAction)(*(APDUBuffer+5)));
						else if(bP1 == 0x04)
							GPIO_WriteBit(TRIG_PORT, TRIG_PIN_4, (BitAction)(*(APDUBuffer+5)));
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
						SetStatusWords(APDUBuffer, 0x6F00);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;
			break;

		case INS_TRIGGLE_DELAY_AND_WIDTH:
			if(bP3 == 8)
			{
				if((bP1 == 1) && (bP2 == 0))  // Only GPIO0 could be used in this function.
				{
					lTemp = 0;
					for(b=0; b<4; b++)
					{
						lTemp <<= 8;
						lTemp |= (*(APDUBuffer+5+b));
					} 
					stuInRegs.lTriggleDelay = lTemp;

					lTemp = 0;
					for(b=4; b<8; b++)
					{
						lTemp <<= 8;
						lTemp |= (*(APDUBuffer+5+b));
					}
					stuInRegs.lTriggleWidth = lTemp;

					if(stuInRegs.lTriggleDelay == 0)
					{
						stuInRegs.iTimerCyc = (uint16_t)(stuInRegs.lTriggleWidth >> 16);
						stuInRegs.iTimerLast = (uint16_t)(stuInRegs.lTriggleWidth);
						stuInRegs.bTriggleStatus = TRIG_STATUS_WIDTH;
					}
					else
					{
						stuInRegs.iTimerCyc = (uint16_t)(stuInRegs.lTriggleDelay >> 16);
						stuInRegs.iTimerLast = (uint16_t)(stuInRegs.lTriggleDelay);
						stuInRegs.bTriggleStatus = TRIG_STATUS_DELAY;
					}
					TIM3Config();
					GPIO_WriteBit(TRIG_PORT, TRIG_PIN_2, (BitAction)(1 - GPIO_ReadOutputDataBit(TRIG_PORT, TRIG_PIN_2)));
					SetStatusWords(APDUBuffer, 0x9000);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;
			break;

		case INS_DATA_TRANSMIT:
			*APDURecvLen = 0;
			if(!bP3 && (InitCmd00Flag==0x01)) 	//CNN 添加init模块  256字节的写操作
			{
				InitCmd00Flag=0x00;
				bP3=0x100;
			}			
			if(bP3 > 0)
			{
				/* Contact Card */
				if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_TDA8007)
				{		        
					if((bP1 == 0)&&((bP2 == 1) || (bP2 == 2) || (bP2 == 3)))
					{
						/* Card is present */
						if((TDA8007ReadReg(RegMixedStatus)& CARD_PRESENT)&&stuInRegs.bIsPoweredOnCT)
						{
							/* Counter is ordered*/
							if(stuInRegs.bCounterStatus == COUNTER_ENABLE_ORDER)
							{
								stuInRegs.bCounterStatus = COUNTER_ENABLE;
								TIM3Start();																							
							}

							iTPDUBufferSendCnt = 5;	  			  
							iTPDUBufferSendLen = APDUSendLen;
							iTPDUBufferRecvCnt = 0;
							iTPDUBufferRecvLen = 0;
							iTPDUBufferRecvCntPrev = 0;
							stuInRegs.iIntBufferOffset = 0;
							stuInRegs.iIntBufferLen = 0;

SEND_DATA:
							/* Non-standard timeout counter is needed */
							stuInRegs.bTmoStatus = bP2;

							bT0Status = T0_STATUS_SEND_BYTES;
							if(bP3 > 1)
								TDA8007SetUCR1Bit(BIT_INDEX_TR, 1); 		// To transmit.
							else
							{
								TDA8007SetUCR1Bit(BIT_INDEX_LCT, 1);
								TDA8007SetUCR1Bit(BIT_INDEX_TR, 1);
							}
							while((bT0Status&0x01) == T0_STATUS_SEND);
							bT0Status = T0_STATUS_RECV_BYTES;
							bIsTimeout = 0;
							if(!CMD1 && (CMD2==0x85) && !CMD3 && !CMD4)                                           //cnn 120911 WTX
							{  
								TDA8007WriteReg(RegGuardTime, 0x00);//CNN 修改，不增加额外的2个ETU		 
								while(bIsTimeout == 0)	
								{  
									if(iTPDUBufferRecvCntPrev != iTPDUBufferRecvCnt) 
									{
										if(abTPDUBuffer[iTPDUBufferRecvCntPrev]==0x00)	  
										{							  
											TDA8007SetTmo(15);			 
											//  for(i=9200;i>0;i--)  ;	   
											TDA8007StartTmo();	   
											while(bIsTimeout == 0);                       
											// TDA8007WriteReg(RegUartConfiguration1, 0x11);   

											*(APDUBuffer+INDEX_DATA+5)=0x00;  
											*(APDUBuffer+INDEX_DATA+6)=0xE3;		 
											*(APDUBuffer+INDEX_DATA+7)=0x01;																	
											*(APDUBuffer+INDEX_DATA+8)=0x01;		
											*(APDUBuffer+INDEX_DATA+9)=0x94;	   
											*(APDUBuffer+INDEX_DATA+10)=0xB8;	   
											iTPDUBufferSendCnt = 10;			 
											iTPDUBufferSendLen = 16;		   
											iTPDUBufferRecvCnt = 0;		  
											iTPDUBufferRecvLen = 6;		 
											iTPDUBufferRecvCntPrev = 0;	 
											bIsTimeout =0;	  
											goto     SEND_DATA;                      

										}	 
										else	  
											INC_TPDU_CNT(iTPDUBufferRecvCntPrev);	  
									}											
								}   
							}


							if(stuInRegs.bTmoStatus != TMO_NO_LIMIT)
							{
								while(bIsTimeout == 0)
								{
									if(iTPDUBufferRecvCntPrev != iTPDUBufferRecvCnt)
									{
										b = abTPDUBuffer[iTPDUBufferRecvCntPrev];
										INC_TPDU_CNT(iTPDUBufferRecvCntPrev);
										abIntBuffer[stuInRegs.iIntBufferLen] = b;
										stuInRegs.iIntBufferLen++;
									}
								}	  				 
								/* Non-standard timeout counter is needed */
								if(bP2 == 2)
									stuInRegs.bTmoStatus = TMO_STANDARD;

								/* Counter is ordered*/
								if(stuInRegs.bCounterStatus == COUNTER_ENABLE)
								{
									TIM_Cmd(TIM3, DISABLE);
									TIM_ITConfig(TIM3, TIM_IT_CC1, DISABLE);                         
									stuInRegs.bCounterStatus = COUNTER_ENABLE_ORDER;
								}

								bIsTimeout = 0;
								if(!CMD1 && !CMD2 && !CMD5)		                              	 //cnn 120911 00
								{
									InitCmd00Flag=1;
								}					
								if(!CMD1 && CMD2==0x04)	 //Init 读命令0004 	 cnn
								{
									InitCmd04Flag=1;
									stuInRegs.iIntBufferLen=stuInRegs.iIntBufferLen-3; //去掉读出的数据04头  和9000
								}

								if(stuInRegs.iIntBufferLen< 256)	   
									SetStatusWords(APDUBuffer, 0x6100|((uint8_t)(stuInRegs.iIntBufferLen)));				
								else
									SetStatusWords(APDUBuffer, 0x61FF);
							}
							else                                      
								SetStatusWords(APDUBuffer, 0x9000);              
						}
						else
						SetStatusWords(APDUBuffer, 0x6F00);  
					}
					else
						SetStatusWords(APDUBuffer, 0x6B00);       
				}
				/* Contactless Card */
				else if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_FM1715)
				{
					if(bP2==0x0B)												  // cnn 2012 5 15  TypeB
					{
						PRTCL=1;												//0表示现在的17寄存器设置为TypeA;1表示TypeB
						b=CLcard17regInit();	
						if(b!=CL_TCL_OK)
						{
							SetStatusWords(APDUBuffer, 0x61FE);	
							break;
						}  		
					}
					else if(bP2 == 0x01)											// 2013.12.13	Hong
						PRTCL=0;	
					if((bP1 <= 0x23) && (bP2 < 17))		//add parity control CJN20150807
					{
						stuInRegs.bCardPresent = SA_CARD_PRESENT	;	           // cnn 2012 5 11			debug TypeB
						/* Card is present */
						if(stuInRegs.bCardPresent == SA_CARD_PRESENT)
						{
							stuInRegs.iIntBufferOffset = 0;
							stuInRegs.iIntBufferLen = 0;

							/* Counter is ordered*/
							if(stuInRegs.bCounterStatus == COUNTER_ENABLE_ORDER)
							{
								stuInRegs.bCounterStatus = COUNTER_ENABLE;
								TIM3Start();																							
							}

							if(bP2 == 0)  // No wait for receiving.
							{
								stuInRegs.bTmoStatus = TMO_NO_LIMIT;
							}

							iNum = APDUSendLen-5;
							for(i=0; i<iNum; i++)
								abTPDUBuffer[i] = (*(APDUBuffer+5+i));
							//            if(bP1)
							//              iNum += 2;
							b = Mf500PiccExchangeBlock(abTPDUBuffer, iNum, 
							abIntBuffer, &iTemp, 
							bP1, (1<<bP2));

							/* Counter is ordered*/
							if(stuInRegs.bCounterStatus == COUNTER_ENABLE)
							{
								TIM_Cmd(TIM3, DISABLE);
								TIM_ITConfig(TIM3, TIM_IT_CC1, DISABLE);                         
								stuInRegs.bCounterStatus = COUNTER_ENABLE_ORDER;
							}

							if(b == MI_OK)
							{
								if(bP2 == 0)  // No wait for receiving.
								{
									SetStatusWords(APDUBuffer, 0x9000);
								}
								else
								{
									stuInRegs.iIntBufferLen = iTemp;
									if(stuInRegs.iIntBufferLen < 256)
										SetStatusWords(APDUBuffer, 0x6100|((uint8_t)(stuInRegs.iIntBufferLen)));			
									else
										SetStatusWords(APDUBuffer, 0x61FF);
								}
							}
							else
								SetStatusWords(APDUBuffer, 0x6100);  
						}
						else
							SetStatusWords(APDUBuffer, 0x6F00);
					}
					else
						SetStatusWords(APDUBuffer, 0x6B00);
				}
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);    
			*APDURecvLen += 2;      
			break;

		case INS_DATA_RECEIVE:
   
			*APDURecvLen = 0;
			if((bP1 == 0)&&(bP2 == 0))
			{
				/* Contactless card */
				if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_FM1715)
				{
					/* No waiting time */
					if(stuInRegs.bTmoStatus == TMO_NO_LIMIT)
					{
						PcdStopRecvCmd(abIntBuffer, &iTemp);
						stuInRegs.iIntBufferLen = iTemp;
					}
				}

				if(stuInRegs.bTmoStatus == TMO_NO_LIMIT)
				{
					stuInRegs.bTmoStatus = TMO_STANDARD;          
				}
				if(InitCmd04Flag==1)		//CNN 修改 
				{
					stuInRegs.iIntBufferLen=stuInRegs.iIntBufferLen+3;
					bP3=bP3+3;
					InitCmd04Flag=0;
				}
				iTemp = stuInRegs.iIntBufferLen - stuInRegs.iIntBufferOffset;
				if(bP3 == 0)
					iNum = 256;
				else
					iNum = stuInRegs.iIntBufferLen;		 					    

				if(iTemp >= iNum)  
				{
					for(i=0; i<iNum; i++)
					{
						*(APDUBuffer+i) = abIntBuffer[stuInRegs.iIntBufferOffset++];
					}
					*APDURecvLen = iNum;

					iTemp -= iNum;
					if(iTemp == 0)
						SetStatusWords(APDUBuffer+iNum, 0x9000);          
					else if(iTemp < 255)        
						SetStatusWords(APDUBuffer+iNum, (0x6100|(uint8_t)(iTemp)));
					else
						SetStatusWords(APDUBuffer+iNum, 0x61FF);
				}
				else
					SetStatusWords(APDUBuffer, 0x6700);        
			}			
			else
				SetStatusWords(APDUBuffer, 0x6B00);
			*APDURecvLen += 2;  
			break;

		case INS_ATR_SOURCE:
			*APDURecvLen = 0;
			if(bP3 == 0x01)
			{
				if((bP1 == 0x00) && (bP2 == 0x00))
				{
					*APDUBuffer = stuInRegs.bATRSource;
					SetStatusWords(APDUBuffer+1, 0x9000);
					*APDURecvLen = 1;
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen += 2;
			break;

#ifdef CONTACTLESS_CARD_ENABLE
		case INS_FM_DEBUG_SIGNAL:
			if(bP3 == 0x01)
			{
				if((bP1 == 0)&&(bP2 == 0x01))
				{
					if(*(APDUBuffer+INDEX_DATA)<0x06)
					{
						WriteRC(RegMfOutSelect, (*(APDUBuffer+INDEX_DATA)));
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
						SetStatusWords(APDUBuffer, 0x6F00);
				}
				else if((bP1 == 0)&&(bP2 == 0x02))
				{
					if(*(APDUBuffer+INDEX_DATA)<0x0D)
					{
						WriteRC(RegTestAnaSelect, (*(APDUBuffer+INDEX_DATA)));
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
						SetStatusWords(APDUBuffer, 0x6F00);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;  
			break;
#endif

		case INS_DIRECT_READ_REGISTER:
			*APDURecvLen = 0;
			if(bP3 == 0x01)
			{
				if(bP1 == 0x01) 
				{
#ifdef CONTACT_CARD_ENABLE
					if(bP2 < 0x10)
					{
						*APDUBuffer = TDA8007ReadReg(bP2);
						SetStatusWords(APDUBuffer+1, 0x9000);
						*APDURecvLen = 1;
					}
					else
#endif  // #ifdef CONTACT_CARD_ENABLE
					SetStatusWords(APDUBuffer, 0x6B00);        
				}
				else if(bP1 == 0x02)
				{
#ifdef CONTACTLESS_CARD_ENABLE
					if(bP2 < 0x40)
					{
						*APDUBuffer = ReadRC(bP2);
						SetStatusWords(APDUBuffer+1, 0x9000);
						*APDURecvLen = 1;
					}
					else
#endif  // #ifdef CONTACTLESS_CARD_ENABLE
						SetStatusWords(APDUBuffer, 0x6B00);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen += 2;
			break;

		case INS_DIRECT_WRITE_REGISTER:
			if(bP3 == 0x01)
			{
				if(bP1 == 0x01) 
				{
#ifdef CONTACT_CARD_ENABLE
					if(bP2 < 0x10)
					{
						TDA8007WriteReg(bP2, *(APDUBuffer+INDEX_DATA));
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
#endif  // #ifdef CONTACT_CARD_ENABLE
						SetStatusWords(APDUBuffer, 0x6B00);        
				}
				else if(bP1 == 0x02)
				{
#ifdef CONTACTLESS_CARD_ENABLE
					if(bP2 < 0x40)
					{
						WriteRC(bP2, *(APDUBuffer+INDEX_DATA));
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
#endif  // #ifdef CONTACTLESS_CARD_ENABLE
						SetStatusWords(APDUBuffer, 0x6B00);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;
			break;

		case INS_COUNTER_START:
			if(bP3 == 0)
			{
				if((bP1 == 0)&&(bP2 == 0))
				{
					stuInRegs.bCounterStatus = COUNTER_ENABLE_ORDER;
					stuInRegs.lCounterValue = 0;
					TIM3Config(); 
					SetStatusWords(APDUBuffer, 0x9000);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);  
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen = 2;      
			break;

		case INS_COUNTER_STOP:
			*APDURecvLen = 0;
			if(bP3 == 4)
			{
				if((bP1 == 0)&&(bP2 == 0))
				{        
					TIM_Cmd(TIM3, DISABLE);
					TIM_ITConfig(TIM3, TIM_IT_CC1, DISABLE);
					stuInRegs.lCounterValue += TIM_GetCounter(TIM3);
					stuInRegs.bCounterStatus = COUNTER_DISABLE;
					lTemp = stuInRegs.lCounterValue;
					for(b=0; b<4; b++)
					{
						*(APDUBuffer+3-b) = (uint8_t)(lTemp);
						lTemp >>= 8;
					}                     
					*APDURecvLen = 4;
					SetStatusWords(APDUBuffer+4, 0x9000);
				}
				else
					SetStatusWords(APDUBuffer, 0x6B00);  
			}
			else
				SetStatusWords(APDUBuffer, 0x6700);
			*APDURecvLen += 2;      
			break;

		case INS_GET_TYPEA_PARAM:
			*APDURecvLen = 0;
			if((bP1 == 0) && (bP2 > 0) && (bP2 < 5))
			{
				if(bP2 == 1)  // ATQA
				{
					if(bP3 == 2)
					{
						*(APDUBuffer) = stuTCLParam.abATQ[0];
						*(APDUBuffer+1) = stuTCLParam.abATQ[1];
						*APDURecvLen = 2;
						SetStatusWords(APDUBuffer+2, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6702);        
					}
				}
				else if(bP2 == 2) // UID
				{
					if((stuTCLParam.bUIDLevel == 1) && (bP3 == 4))
					{ 
						for(b=0; b<4; b++)
							*(APDUBuffer+b) = stuTCLParam.abUID[b];         
						*APDURecvLen = 4;          
						SetStatusWords(APDUBuffer+4, 0x9000);          
					}
					else if((stuTCLParam.bUIDLevel == 2) && (bP3 == 7))
					{
						for(b=0; b<4; b++)
							*(APDUBuffer+b) = stuTCLParam.abUID[b];         
						for(b=5; b<8; b++)
							*(APDUBuffer+b-1) = stuTCLParam.abUID[b];         
						*APDURecvLen = 7;           
						SetStatusWords(APDUBuffer+7, 0x9000);         
					}
					else if((stuTCLParam.bUIDLevel == 3) && (bP3 == 10))
					{
						for(b=0; b<4; b++)
							*(APDUBuffer+b) = stuTCLParam.abUID[b];         
						for(b=5; b<8; b++)
							*(APDUBuffer+b-1) = stuTCLParam.abUID[b];         
						for(b=9; b<12; b++)
							*(APDUBuffer+b-2) = stuTCLParam.abUID[b];         
						*APDURecvLen = 10; 
						SetStatusWords(APDUBuffer+10, 0x9000);
					}
					else
					{
						if(stuTCLParam.bUIDLevel == 1)    
							SetStatusWords(APDUBuffer, 0x6704);            
						else if(stuTCLParam.bUIDLevel == 2)    
							SetStatusWords(APDUBuffer, 0x6707);
						else if(stuTCLParam.bUIDLevel == 3)    
							SetStatusWords(APDUBuffer, 0x670A);
					}

				}
				else if(bP2 == 3)  // SAK
				{
					if(bP3 == 1)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6701);
					}
				}
				if(bP2 == 4) // ATS
				{
					if(bP3 == (uint8_t)stuTCLParam.iATSLen)
					{
						for(b=0; b<bP3; b++)
							*(APDUBuffer+b) = stuTCLParam.abATS[b];
						*APDURecvLen = bP3;
						SetStatusWords(APDUBuffer+bP3, 0x9000);
					}
					else
						SetStatusWords(APDUBuffer, (0x6700+stuTCLParam.iATSLen));          
				}  
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);
			*APDURecvLen += 2;
			break;
		case INS_CL_OPERATION:				//////////////////////
			*APDURecvLen = 0;
			if(bP1 == 0x01)
			{
				if(bP2 == 0x00)//开关场
				{
					if(APDUBuffer[5] == 0x00)
					{
						CLCardPowerOff();		   
						bStatus = MI_OK;
					}
					else
					{ 
						CLCardPowerOff();
						bStatus = Mf500PcdConfig();
					}
					//	  	ClearBitMask(RegTxControl,0x03);   
					if(bStatus == MI_OK)
						SetStatusWords(APDUBuffer, 0x9000);
					else
						SetStatusWords(APDUBuffer, 0x6D00);		
				}
	  			else if(bP2 == 0x01)//REQA
				{
					CLCardInit();
					bStatus = Mf500PiccRequest(PICC_REQIDL, &(stuTCLParam.abATQ[0]));
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.abATQ[0];
						*(APDUBuffer+1) = stuTCLParam.abATQ[1];
						*APDURecvLen = 2;
						SetStatusWords(APDUBuffer+2, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D01);
					}
				}
	  			else if(bP2 == 0x02)//AntiColl 1
				{
					bStatus = Mf500PiccAnticollWithBCC(PICC_ANTICOLL1, &(stuTCLParam.abUID[0])); //自己添加的发送93 20并且无论bcc多错都回发4字节加bcc
					if((bStatus == MI_OK) || (bStatus == MI_SERNRERR))
					{

						for(b=0; b<5; b++)
							*(APDUBuffer+b) = stuTCLParam.abUID[b];         
						*APDURecvLen = 5;
						if(bStatus == MI_SERNRERR)
							SetStatusWords(APDUBuffer+5, 0x9001);//BCC校验出错
						else
							SetStatusWords(APDUBuffer+5, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D02);
					}
				} 
				else if(bP2 == 0x22)//AntiColl 2
				{
					bStatus = Mf500PiccAnticollWithBCC(PICC_ANTICOLL2, &(stuTCLParam.abUID[0]));
					if((bStatus == MI_OK) || (bStatus == MI_SERNRERR))
					{
						for(b=0; b<5; b++)
							*(APDUBuffer+b) = stuTCLParam.abUID[b];         
						*APDURecvLen = 5;
						if(bStatus == MI_SERNRERR)
							SetStatusWords(APDUBuffer+5, 0x9001);//BCC error
						else
							SetStatusWords(APDUBuffer+5, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D22);
					}
				}
				else if(bP2 == 0x32)//AntiColl 3
				{
					bStatus = Mf500PiccAnticollWithBCC(PICC_ANTICOLL3, &(stuTCLParam.abUID[0]));
					if((bStatus == MI_OK) || (bStatus == MI_SERNRERR))
					{
						for(b=0; b<5; b++)
							*(APDUBuffer+b) = stuTCLParam.abUID[b];         
						*APDURecvLen = 5;
						if(bStatus == MI_SERNRERR)
							SetStatusWords(APDUBuffer+5, 0x9001);//BCC error
						else
							SetStatusWords(APDUBuffer+5, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D32);
					}
				}
				else if(bP2 == 0x03)//sel 1
				{
					bStatus = Mf500PiccCascSelect(PICC_ANTICOLL1, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 1);
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						stuInRegs.bCardPresent = SA_CARD_PRESENT;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x6D03);
					}
				}
				else if(bP2 == 0x23)//sel 2
				{
					bStatus = Mf500PiccCascSelect(PICC_ANTICOLL2, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 1);
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						stuInRegs.bCardPresent = SA_CARD_PRESENT;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x6D23);
					}
				}
				else if(bP2 == 0x33)//sel 3
				{
					bStatus = Mf500PiccCascSelect(PICC_ANTICOLL3, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 1);
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						stuInRegs.bCardPresent = SA_CARD_PRESENT;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x6D33);
					}
				}
				else if(bP2 == 0x83)//sel 1 2015.08.17 by CJN: without check BCC
				{
					bStatus = Mf500PiccCascSelect(PICC_ANTICOLL1, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 0);
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						stuInRegs.bCardPresent = SA_CARD_PRESENT;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x6D83);
					}
				}
				else if(bP2 == 0xA3)//sel 2 2015.08.17 by CJN: without check BCC
				{
					bStatus = Mf500PiccCascSelect(PICC_ANTICOLL2, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 0);
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						stuInRegs.bCardPresent = SA_CARD_PRESENT;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x6DA3);
					}
				}
				else if(bP2 == 0xB3)//sel 3 2015.08.17 by CJN: without check BCC
				{
					bStatus = Mf500PiccCascSelect(PICC_ANTICOLL3, &(stuTCLParam.abUID[0]), &(stuTCLParam.bSak), 0);
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.bSak;
						*APDURecvLen = 1;
						stuInRegs.bCardPresent = SA_CARD_PRESENT;
						SetStatusWords(APDUBuffer+1, 0x9000);
					}
					else
					{
						*APDURecvLen = 0;
						SetStatusWords(APDUBuffer, 0x6DB3);
					}
				}
				else if(bP2 == 0x04)
				{
					/* RATS */  
					abTPDUBuffer[0] = 0xE0;
					abTPDUBuffer[1] = 0x81;	  // CID = 1
					stuTCLParam.lFWT = 1<<12;
					bStatus = Mf500PiccExchangeBlock(abTPDUBuffer, 2, 
					                              APDUBuffer, &(stuTCLParam.iATSLen), 
					                              1, stuTCLParam.lFWT);
					if(bStatus == MI_OK)
					{
						/* Parser ATS*/                  
						//memcpy(&(stuTCLParam.abATS[0]), APDUBuffer, stuTCLParam.iATSLen);
						*APDURecvLen = stuTCLParam.iATSLen;					

						bT0 = (*(APDUBuffer+1));
						stuTCLParam.bFSCI = (bT0&0x0F);
						if(bT0&0x10)
						{
						b = (*(APDUBuffer+2));
						stuTCLParam.bDRC = (b&0x0F);
						stuTCLParam.bDSC = ((b&0xF0)>>4);
						}
						if(bT0&0x20)
						{
						b = (*(APDUBuffer+3));
						stuTCLParam.bSFGI = (b&0x0F);
						stuTCLParam.bFWI = ((b&0xF0)>>4);
						stuTCLParam.lFWT = 1;
						stuTCLParam.lFWT <<= stuTCLParam.bFWI;

						stuTCLParam.lFWT = 1<<12;//此处固定最长超时了 2013-04-28

						}
						if(bT0&0x40)
						{
						b = (*(APDUBuffer+4));
						if(b&0x01)
						  stuTCLParam.bNADEn = 1;
						else
						  stuTCLParam.bNADEn = 0;      
						if(b&0x02)
						  stuTCLParam.bCIDEn = 1;
						else
						  stuTCLParam.bCIDEn = 0;
						}
						/* Information field length */
						// stuTCLParam.bINFLen = 254;
						stuTCLParam.bINFLen = 253;
						// stuTCLParam.bINFLen = 29;
						if(stuTCLParam.bFSCI < 8)
						stuTCLParam.bINFLen = (uint8_t)(FrameSize[stuTCLParam.bFSCI]-3);  // 1 PCB byte + 2 CRC bytes
						stuTCLParam.bINFLen -= stuTCLParam.bNADEn;
						stuTCLParam.bINFLen -= stuTCLParam.bCIDEn;

						SetStatusWords(APDUBuffer+stuTCLParam.iATSLen, 0x9000); 

					}
				  	else
					{
						SetStatusWords(APDUBuffer, 0x6D03);
					}
				}
     	 			else if(bP2 == 0x05)//MIReadE2
				{
					bStatus = Mf500PiccRead(APDUBuffer[5],APDUBuffer);
					if(bStatus == MI_OK)
					{
						*APDURecvLen = 0x10;
						SetStatusWords(APDUBuffer+0x10, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D04);
					}
				}
				else if(bP2 == 0x06)//MIWriteE2
				{
					bStatus = Mf500PiccWrite(APDUBuffer[5],APDUBuffer+6);
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D05);
					}
				}
				else if(bP2 == 0x0C)//MIWriteE2_128BYTES
				{
					bStatus = Mf500PiccWrite128(APDUBuffer[5],APDUBuffer+6);
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D05);
					}
				}
				else if(bP2 == 0x07)//halt
				{
					bStatus = Mf500PiccHalt();
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer,0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer,0x6D06);
					}
				}
				else if(bP2 == 0x08)//band rate
				{
					bStatus = Mf500PcdSetAttrib(APDUBuffer[5], APDUBuffer[6]);	  //(0,0)~106K (1,1)~212K (2,2)~424K (3,3)~848K
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D08);
					}
				}
				else if(bP2 == 0x0B)//Type B select
				{
					PRTCL = 1;		 // Type B 
					b=CLcard17regInit();	
					if(b!=CL_TCL_OK)
					{
						SetStatusWords(APDUBuffer, 0x61FE);	
						break;
					}  	
					abTPDUBuffer[0] = PICC_REQB_APf;
					abTPDUBuffer[1] = 0x00;	  
					abTPDUBuffer[2] = 0x00;	  
					bStatus = Mf500PiccReqB(abTPDUBuffer,5,APDUBuffer,&(stuTCLParam.iREQBLen));

					if(bStatus == MI_OK)
					{
						*APDURecvLen = stuTCLParam.iREQBLen;
						SetStatusWords(APDUBuffer+stuTCLParam.iREQBLen, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D09);
					}
				}
				else if(bP2 == 0x0A)//WupA
				{
					CLCardInit();
					bStatus = Mf500PiccRequest(PICC_REQALL, &(stuTCLParam.abATQ[0]));
					if(bStatus == MI_OK)
					{
						*(APDUBuffer) = stuTCLParam.abATQ[0];
						*(APDUBuffer+1) = stuTCLParam.abATQ[1];
						*APDURecvLen = 2;
						SetStatusWords(APDUBuffer+2, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D0A);
					}
				}
				else if(bP2 == 0x11)		// Block Inc,	2014.3.6	Hong
				{
					bStatus = Mf500Picc_Inc_Dec(PICC_INCREMENT,*(APDUBuffer+5),4,APDUBuffer+6);
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D11);
					}
				}
				else if(bP2 == 0x12)		// Block Dec,	2014.3.6	Hong
				{
					bStatus = Mf500Picc_Inc_Dec(PICC_DECREMENT,*(APDUBuffer+5),4,APDUBuffer+6);
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D12);
					}
				}
				else if(bP2 == 0x13)		// Restore,	2014.3.6	Hong
				{
					bStatus = Mf500Picc_Res_Tra(PICC_RESTORE,*(APDUBuffer+5));
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D13);
					}
				}
				else if(bP2 == 0x14)		// Transfer,	2014.3.6	Hong
				{
					bStatus = Mf500Picc_Res_Tra(PICC_TRANSFER,*(APDUBuffer+5));
					if(bStatus == MI_OK)
					{
						SetStatusWords(APDUBuffer, 0x9000);
					}
					else
					{
						SetStatusWords(APDUBuffer, 0x6D14);
					}
				}
				else
        				SetStatusWords(APDUBuffer, 0x6700);  
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);
			*APDURecvLen += 2;
			break;

		case INS_CT_OPERATION:				
			*APDURecvLen = 0;	  
			if(bP1 == 0x01)
			{
				if(bP2 == 0x00)
				{	  

					//			TDA8007_Pow_Config(DISABLE);
					//			lTemp =50000;
					//			while(lTemp--);
					//			TDA8007_Pow_Config(ENABLE);
					ContactCardPowerOff();
					stuInRegs.bIsPoweredOnCT = 1;// 0;
					SetStatusWords(APDUBuffer, 0x9000);

				}
				else if(bP2 == 0x01)// Cold Reset
				{
					ContactCardPowerOff();
					ContactCardPowerOff();
					ContactCardPowerOff();
					ContactCardPowerOff();
					ContactCardPowerOff();
					ContactCardPowerOff();
					if(TDA8007ReadReg(RegMixedStatus)& CARD_PRESENT)
						bStatus = ContactCardReset(APDUBuffer[5], APDUBuffer, APDURecvLen); //发送的P3后第一个数据位电压，返回的数据为ATR
					else
						bStatus = CT_T0_ATR_TIMEOUT;
					if(bStatus == CT_T0_OK)
					{
						SetStatusWords(APDUBuffer+*APDURecvLen, 0x9000);
						stuInRegs.bIsPoweredOnCT = 1;
					}
					else
					{
						*APDURecvLen = 0x01;
						APDUBuffer[0] = bStatus;  		    	
						SetStatusWords(APDUBuffer+1, 0x6D01);
					}
					//	TDA8007ReadAllRegs();
					//	stuInRegs.bIsPoweredOnCT +=1;	 //for debug
				}
				else if(bP2 == 0x02)//warm reset
				{
					if(TDA8007ReadReg(RegMixedStatus)& CARD_PRESENT)
						bStatus = ContactCardReset(4, APDUBuffer, APDURecvLen); //发送的P3后第一个数据位电压，返回的数据为ATR
					else
						bStatus = CT_T0_ATR_TIMEOUT;
					if(bStatus == CT_T0_OK)
					{
						SetStatusWords(APDUBuffer+*APDURecvLen, 0x9000);
					}
					else
					{
						*APDURecvLen = 0x01;
						APDUBuffer[0] = bStatus;  
						SetStatusWords(APDUBuffer+1, 0x6D02);
					}
				}
				else if(bP2 == 0x03)
				{

				}
				else
					SetStatusWords(APDUBuffer, 0x6700);	
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);
			*APDURecvLen += 2;
			break;

		case INS_SPI_OPERATION:
			*APDURecvLen = 0;

			if(bP1==0)
			{
				if(bP2!=0)		
				{
					if(bP3!=0)//bP3为需要接受的数据
					{
						SPI1_CS_L();
			
						SPI_Send(APDUBuffer+5,bP2);
			/*		SPI1_Rx_Idx = 0;
						SPI_I2S_ITConfig(SPI1, SPI_I2S_IT_RXNE, ENABLE);
			*/		bIsTimeout = 0;
						TIM1Config(1); // 1ms
						TIM1Start();
						while(bIsTimeout==0);  //发送接收间的延时
			
						SPI_Receive(APDUBuffer,bP3);			
						
						SPI1_CS_H();
			
						SetStatusWords(APDUBuffer+bP3,0x9000);
						*APDURecvLen = bP3;			
					}
					else
					{
						SPI1_CS_L();
						
						SPI_Send(APDUBuffer+5,bP2);
						
						SPI1_CS_H();				
						
						SetStatusWords(APDUBuffer,0x9000);
			
					}
				}
				else
					SetStatusWords(APDUBuffer, 0x6700);//bP2错误		
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);	//bP1出错
			*APDURecvLen +=2;
			break;
		case INS_I2C_OPERATION:
			*APDURecvLen = 0;
			if(!bP1)
			{
				I2C_Addr_Len = 0;
				if(bP2)
				{
					if(!*(APDUBuffer+6))
					{
						if(bP3)
						{

							bIsTimeout = 0;
							TIM1Config(10000); //1000ms
							TIM1Start();
							I2C_Send(APDUBuffer+5,APDUBuffer+7,bP2-2); //第一个为地址。后面是数据

							I2C_Receive(APDUBuffer+5,APDUBuffer,bP3);

							//				I2C_ReadByte(APDUBuffer,bP3,*(APDUBuffer+5));
							SetStatusWords(APDUBuffer+bP3,0x9000);
							*APDURecvLen = bP3;			

						}
						else
						{
							I2C_Send(APDUBuffer+5,APDUBuffer+7,bP2-2);
							I2C_Stop();
							SetStatusWords(APDUBuffer+bP3,0x9000);
							*APDURecvLen = bP3;
						}
					}
					else
						goto	AAA;
				}
				else
					SetStatusWords(APDUBuffer, 0x6700);//bP2错误		
			}
/*	else if(bP1==1)
	{
		I2C_ReadBytes24(APDUBuffer,bP3,*(APDUBuffer+5));
		SetStatusWords(APDUBuffer+bP3,0x9000);
		*APDURecvLen = bP3;		
			}	*/			
			else
				SetStatusWords(APDUBuffer, 0x6B00);	//bP1出错
			*APDURecvLen += 2;
			break;
		case INS_10BitI2C_OPERATION:
			*APDURecvLen = 0;
			
			if(!bP1)
			{
				if(bP2)
				{
AAA:
					I2C_Addr_Len = 1;
					if(bP3)
					{
						bIsTimeout = 0;
						TIM1Config(10000); //1000ms
						TIM1Start();
						I2C_Send(APDUBuffer+5,APDUBuffer+7,bP2-2); //第一个为地址。后面是数据
						
						I2C_Receive(APDUBuffer+5,APDUBuffer,bP3);

						SetStatusWords(APDUBuffer+bP3,0x9000);
						*APDURecvLen = bP3;			
					}
					else
					{
						I2C_Send(APDUBuffer+5,APDUBuffer+7,bP2-2);
						I2C_Stop();
						SetStatusWords(APDUBuffer+bP3,0x9000);
						*APDURecvLen = bP3;
					}
				}
				else
					SetStatusWords(APDUBuffer, 0x6700);//bP2错误		
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);	//bP1出错
			*APDURecvLen += 2;
			break;
//////////////////////////////////////////////////////////////////////////////
		case INS_SEND_PPSA:
			if((bP1 == 0) && (bP2 == 0))
			{
				if(bP3 == 0x01)
				{
					b=(*(APDUBuffer+INDEX_DATA));
					//修改非接PPS，使pps1从0x00到0x0F都可以切换。	  cnn 20131104
					if((b < 0x10))
					{
						bTemp = CLCardPPS(b);
						if(bTemp == CL_TCL_OK)
							SetStatusWords(APDUBuffer, 0x9000);  
						else
							SetStatusWords(APDUBuffer, 0x6500);
					}
					else
						SetStatusWords(APDUBuffer, 0x6F00);   
					/*if((b == 0x11)||(b == 0x13)||(b == 0x94)||(b == 0x18))
					{
					bTemp = CLCardPPS(b);
					if(bTemp == CL_TCL_OK)
					SetStatusWords(APDUBuffer, 0x9000);  
					else
					SetStatusWords(APDUBuffer, 0x6500);
					}
					else
					SetStatusWords(APDUBuffer, 0x6F00);         */ 

				}
				else
					SetStatusWords(APDUBuffer, 0x6700);  
			}
			else if((bP1 == 0) && (bP2 == 1)) 
			{
				if(bP3 == 0x01)
				{
				b=(*(APDUBuffer+INDEX_DATA));

				bTemp = ContactCardPPS(b);
					if(bTemp == CT_T0_OK)
						SetStatusWords(APDUBuffer, 0x9000);  
					else
						SetStatusWords(APDUBuffer, 0x6500);
				/*
				if((b == 0x11)||(b == 0x13)||(b == 0x94)||(b == 0x18)||(b == 0x95)||(b == 0x96))          
				{
				bTemp = ContactCardPPS(b);
				if(bTemp == CT_T0_OK)
				SetStatusWords(APDUBuffer, 0x9000);  
				else
				SetStatusWords(APDUBuffer, 0x6500);
				}
				else
				SetStatusWords(APDUBuffer, 0x6F00);          
				*/
				}
				else
					SetStatusWords(APDUBuffer, 0x6700);  	
			}
			else
				SetStatusWords(APDUBuffer, 0x6B00);
			*APDURecvLen = 2; 
			break;

		default:
			SetStatusWords(APDUBuffer, 0x6D00);
			*APDURecvLen = 2;
			break;
	}
}

/*******************************************************************************
* Function Name  : TIM3InterruptHandler.
* Description    : TIM3 interrupt handler.
* Input          : None 
* Output         : None
* Return         : None
*******************************************************************************/
void TIM3InterruptHandler(void)
{
  if(TIM_GetITStatus(TIM3, TIM_IT_CC1) != RESET)
  {
    TIM_ClearITPendingBit(TIM3, TIM_IT_CC1);
    /* Counter function */
    if(stuInRegs.bCounterStatus == COUNTER_ENABLE)
    {
      if(stuInRegs.lCounterValue)
        stuInRegs.lCounterValue += 65536;
      else
        stuInRegs.lCounterValue++;
    }
    /* Delay and width function */
    else
    {
      if(stuInRegs.iTimerCyc)
      {
        stuInRegs.iTimerCyc--;
        if(stuInRegs.iTimerCyc == 0)
        {
          if(stuInRegs.iTimerLast)
            TIM_SetCompare1(TIM3, TIM_GetCapture1(TIM3) + stuInRegs.iTimerLast);
          else
            goto TIMER_OVER;
        }
      }
      else
      {
  TIMER_OVER:
        if(stuInRegs.bTriggleStatus == TRIG_STATUS_DELAY)
        {
          stuInRegs.bTriggleStatus = TRIG_STATUS_WIDTH;
          stuInRegs.iTimerCyc = (uint16_t)(stuInRegs.lTriggleWidth >> 16);
          stuInRegs.iTimerLast = (uint16_t)(stuInRegs.lTriggleWidth);
          if(stuInRegs.iTimerCyc)
            TIM_SetCompare1(TIM3, TIM_GetCapture1(TIM3));   // 65536  
          else if(stuInRegs.iTimerLast)
            TIM_SetCompare1(TIM3, TIM_GetCapture1(TIM3) + stuInRegs.iTimerLast);         
  
        }
        else
        {
          TIM_Cmd(TIM3, DISABLE);
          TIM_ITConfig(TIM3, TIM_IT_CC1, DISABLE);
          stuInRegs.bTriggleStatus = 0;
        }
        GPIO_WriteBit(TRIG_PORT, TRIG_PIN_1, (BitAction)(1 - GPIO_ReadOutputDataBit(TRIG_PORT, TRIG_PIN_1)));  
      }
    }
  }
}
