/*
*********************************************************************************************************
*                                              Main code
*
*
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*
*
* Filename      : app.c
* Version       : V1.00
* Programmer(s) : BAN
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                             INCLUDE FILES
*********************************************************************************************************
*/

#include "includes.h"


/*
*********************************************************************************************************
*                                            LOCAL DEFINES
*********************************************************************************************************
*/

/*
*********************************************************************************************************
*                                       LOCAL GLOBAL VARIABLES
*********************************************************************************************************
*/

static  OS_STK         	App_TaskStartStk[APP_TASK_START_STK_SIZE];
static  OS_STK         	App_TaskCCIDStk[APP_TASK_CCID_STK_SIZE];
static  OS_STK         	App_TaskBuzzStk[APP_TASK_BUZZ_STK_SIZE];
static  OS_STK			App_TaskLedCountStk[APP_TASK_LED_STK_SIZE]; 
						

//static  OS_EVENT      *App_UserIFMbox;
INT8U err;
INT8U LedCount=0;
uint8_t  volatile bUsbStatus = 0;
bool bTimeOut = FALSE;
bool bIsCTon = FALSE;
bool bIsCLon = FALSE;



OS_EVENT *buzz;
OS_EVENT *usb_Sem;




/*
*********************************************************************************************************
*                                      LOCAL FUNCTION PROTOTYPES
*********************************************************************************************************
*/
static  void  	App_TaskStart        	(void       *p_arg);
static  void  	App_TaskCCID       		(void       *p_arg);
static  void  	App_TaskBuzz          	(void       *p_arg);

static 	void 	App_TaskLedCount		(void 		*p_arg);


void CCID_handler(void);
/*
*********************************************************************************************************
*                                                main()
*
* Description : This is the standard entry point for C code.  It is assumed that your code will call
*               main() once you have performed all necessary initialization.
*
* Argument(s) : none.
*
* Return(s)   : none.
*********************************************************************************************************
*/

int  main (void)
{

    OSInit();                                                   /* Initialize "uC/OS-II, The Real-Time Kernel".         */

	buzz = OSSemCreate(0);
	usb_Sem= OSSemCreate(0);

    OSTaskCreate(
				App_TaskStart,
				(void*)0,
				&App_TaskStartStk[APP_TASK_START_STK_SIZE-1],
				APP_TASK_START_PRIO
				);
	/*
	OSTaskCreate ((void (*)(void *)) App_TaskStart, 
				  NULL, 
				  App_TaskStartStk,
				  APP_TASK_START_PRIO
				  );

	  */
 /*   os_err = OSTaskCreateExt((void (*)(void *)) App_TaskStart,                               
                             (void          * ) 0,
                             (OS_STK        * )&App_TaskStartStk[APP_TASK_START_STK_SIZE - 1],
                             (INT8U           ) APP_TASK_START_PRIO,
                             (INT16U          ) APP_TASK_START_PRIO,
                             (OS_STK        * )&App_TaskStartStk[0],
                             (INT32U          ) APP_TASK_START_STK_SIZE,
                             (void          * )0,
                             (INT16U          )(OS_TASK_OPT_STK_CLR | OS_TASK_OPT_STK_CHK));
   */
    OSStart();                                                  /* Start multitasking (i.e. give control to uC/OS-II).  */

    return (0);
}


/*
*********************************************************************************************************
*                                          App_TaskStart()
*
* Description : The startup task.  The uC/OS-II ticker should only be initialize once multitasking starts.
*
* Argument(s) : p_arg       Argument passed to 'App_TaskStart()' by 'OSTaskCreate()'.
*
* Return(s)   : none.
*
* Caller(s)   : This is a task.
*
* Note(s)     : none.
*********************************************************************************************************
*/

static  void  App_TaskStart (void *p_arg)
{


    (void)p_arg;

	BSP_Init();


//  	printf("<=======   uCos-ii STM32 Ports  =======>\r\n");

	OSStatInit(); 

	OSTaskCreate(
				(void (*)(void *)) App_TaskLedCount, 
				NULL, 
				&App_TaskLedCountStk[APP_TASK_LED_STK_SIZE-1],
				APP_TASK_LED_PRIO
				);	
	
	OSTaskCreate(
				(void (*)(void *)) App_TaskCCID, 
				NULL, 
				&App_TaskCCIDStk[APP_TASK_CCID_STK_SIZE-1],
				APP_TASK_CCID_PRIO
				);

	OSTaskCreate(
				(void (*)(void *)) App_TaskBuzz, 
			  	NULL, 
			  	&App_TaskBuzzStk[APP_TASK_BUZZ_STK_SIZE-1],
			  	APP_TASK_BUZZ_PRIO
			  	);

	while(1)
	{
	 OSTaskSuspend(OS_PRIO_SELF);
	}
}



/*
*********************************************************************************************************
*                                            App_TaskBuzz()
*
* Description : Monitor the state of the push buttons and passes messages to AppTaskUserIF()
*
* Argument(s) : p_arg       Argument passed to 'App_TaskBuzz()' by 'OSTaskCreate()'.
*
* Return(s)   : none.
*
* Caller(s)   : This is a task.
*
* Note(s)     : none.
*********************************************************************************************************
*/

static  void  App_TaskBuzz (void *p_arg)
{

    (void)p_arg;

   
    while (1) 
	{
		OSSemPend(buzz, 0, &err); //  等待信号量

		BUZZ(50);
        OSTimeDlyHMSM(0, 0, 0, 0);
    }
}

/*
*********************************************************************************************************
*                                            App_TaskLedCount()
*
* Description : LED Count 0~0x10.
*
* Argument(s) : p_arg       Argument passed to 'AApp_TaskLedCount()' by 'OSTaskCreate()'.
*
* Return(s)   : none.
*
* Caller(s)   : This is a task.
*
* Note(s)     : none.
*********************************************************************************************************
*/

static void App_TaskLedCount(void *p_arg)
{
	INT8U i,tmp;
	char ch[10];
	(void)p_arg;
	
	for (i=0;i<10;i++)
		ch[i] = 0x00;

	ch[0] = '0';ch[1] = 'x';


	while(1)
	{
		if(LedCount ==0x20)
		{
			err = OSSemPost(buzz); // 发送信号量
			LedCount = 0;
//		Write_1106_Command(0xAE);    /*display OFF*/ 

		}
		tmp = LedCount;
		if((tmp>>4)>9)
		{
			ch[2] = 65 + (tmp>>4) - 0x0A;
		}
		else
		{
			ch[2] = 48 + (tmp>>4);
		}
		if((tmp&0x0F)>9)
		{
			ch[3] = 65 + (tmp&0x0F) - 0x0A;
		}
		else
		{
			ch[3] = 48 + (tmp&0x0F);
		}
//		SetDisplay(0,4,ch);
//		printf("%d :",tmp);
		for(i=0; i<4; i++)
		{
			tmp = LedCount >> i;
			if(tmp & 0x01)
				LEDShow(i+1 , Bit_RESET);
			else
				LEDShow(i+1 , Bit_SET);
		}
		LedCount++;
//		printf("%d%%\r\n",OSCPUUsage);
		OSTimeDlyHMSM(0, 0, 1, 0);
	}

}

/*
*********************************************************************************************************
*                                            App_TaskCCID()
*
* Description : manage CCID conmunication.
*
* Argument(s) : p_arg       Argument passed to 'AApp_TaskCCID()' by 'OSTaskCreate()'.
*
* Return(s)   : none.
*
* Caller(s)   : This is a task.
*
* Note(s)     : none.
*********************************************************************************************************
*/

static  void  App_TaskCCID (void *p_arg)
{



    (void)p_arg;

    while (1) 
	{
		OSSemPend(usb_Sem, 0, &err); //  等待信号量
		
		CCID_handler();
	    OSTimeDlyHMSM(0, 0, 0, 20);
    }
}

void CCID_handler(void)
{
	u8 bTemp = 0;
	while(bReaderStatus != RDS_IDLE)
	{
		switch(bReaderStatus)      
		{
			case RDS_PARSER:
				iBufferRecvCnt = 0;
				bTemp = CCIDParser(abBuffer, abAPDUBuffer, &iAPDUBufferSendLen);
				if(bTemp == CCID_NOK)
				{
					bStatus = (CCID_COMMAND_STATUS_FAIL|CCID_ICC_STATUS_PRES_INACT);
					bError = (uint8_t)CCID_ERROR_USER_DEFINED;
					bReaderStatus = RDS_ORGNIZE;
				}
				else
					bReaderStatus = RDS_ACTION;        
				break;	
			//end of RDS_PARSER
			case RDS_ACTION:
				LEDShow(3, Bit_RESET);
				bStatus = 0;
				bError = 0;
				switch(bTemp & 0xF0)
				{
					case ACT_POWER_ON:
						//goto VIRTUAL_ATR;// gaoxuebing

						if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_TDA8007)
						{


							if(CCIDCardSlotCheck() == CCID_CARD_SLOT_PRESENT)        
							{
								//ContactCardPowerOff();

								bPowerSelect = stuInRegs.bVoltage;  // Supply voltage is configured by special function APDU.

								//bTemp = ContactCardReset(bPowerSelect, abAPDUBuffer, &iAPDUBufferRecvLen);
								bTemp = 0x02;
								if(bTemp != CT_T0_OK)
									goto VIRTUAL_ATR;
								else
									stuInRegs.bATRSource = ATR_SOURCE_CARD;  
							}
							else
								goto VIRTUAL_ATR;

						}
						else if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_FM1715)
						{
							CLCardPowerOff();
							bTemp = CLCardReset(abAPDUBuffer, &iAPDUBufferRecvLen);
							if((bTemp == CL_TCL_ATS_ERROR) || (bTemp == CL_TCL_OK))
								stuInRegs.bCardPresent = SA_CARD_PRESENT;
							else 
								stuInRegs.bCardPresent = SA_CARD_ABSENT;            
							if(bTemp != CL_TCL_OK)          
								goto VIRTUAL_ATR;
							else
								stuInRegs.bATRSource = ATR_SOURCE_CARD;
						}
						else
						{
						VIRTUAL_ATR:
							abAPDUBuffer[0] = 0x3B;        
							abAPDUBuffer[1] = 0x96;
							abAPDUBuffer[2] = 0x11;
							abAPDUBuffer[3] = 0x80;
							abAPDUBuffer[4] = 0x01;
							abAPDUBuffer[5] = 0x4E;
							abAPDUBuffer[6] = 0x42;
							abAPDUBuffer[7] = 0x31;
							abAPDUBuffer[8] = 0x30;
							abAPDUBuffer[9] = 0x32;
							abAPDUBuffer[10] = 0x36;
							abAPDUBuffer[11] = 0x0F;
							iAPDUBufferRecvLen = 12;
					/*          abAPDUBuffer[0] = 0x3B;        T1:	3B 96 11 80 01 4E 42 31 30 32 36 0F
							abAPDUBuffer[1] = 0x16;
							abAPDUBuffer[2] = 0x11;
							abAPDUBuffer[3] = 'N';
							abAPDUBuffer[4] = 'B';
							abAPDUBuffer[5] = '1';
							abAPDUBuffer[6] = '0';
							abAPDUBuffer[7] = '2';
							abAPDUBuffer[8] = '6';
							iAPDUBufferRecvLen = 9;	*/

							stuInRegs.bATRSource = ATR_SOURCE_VIRTUAL;

						}
						break;

					case ACT_POWER_OFF:
					/*    if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_TDA8007)		//gao
							ContactCardPowerOff();
						else if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_FM1715)
							CLCardPowerOff();
						*/          
						break;

					case ACT_APDU_EXCHANGE:
						/* Triggle delay and width is ordered*/
						if(stuInRegs.bTriggleStatus > 0)
							TIM3Start();

						if(abAPDUBuffer[0] == 0xFF)
						{
							SpecialAPDU(abAPDUBuffer, iAPDUBufferSendLen, &iAPDUBufferRecvLen);  
						}
						else
						{
							if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_TDA8007)
							{
								LEDShow(3, Bit_SET);
								if(CCIDCardSlotCheck() == CCID_CARD_SLOT_PRESENT)
								{
									/* Counter is ordered*/
									if(stuInRegs.bCounterStatus == COUNTER_ENABLE_ORDER)
									{
										stuInRegs.bCounterStatus = COUNTER_ENABLE;
										TIM3Start();                                              
									}

									if(stuInRegs.bATRSource == ATR_SOURCE_CARD)
										bTemp = ContactCardT0APDU(abAPDUBuffer, iAPDUBufferSendLen, &iAPDUBufferRecvLen);        
									else
										bTemp = CT_T0_APDU_TIMEOUT;

									if(stuInRegs.bCounterStatus == COUNTER_ENABLE)
									{
										TIM_Cmd(TIM3, DISABLE);
										TIM_ITConfig(TIM3, TIM_IT_CC1, DISABLE);                         
										stuInRegs.bCounterStatus = COUNTER_ENABLE_ORDER;
									}

								}
								else
									bTemp = CT_T0_ATR_TIMEOUT; 
								if(bTemp != CT_T0_OK)
								{
									iAPDUBufferRecvLen = 0;
									bStatus = CCID_COMMAND_STATUS_FAIL;
									bError = (uint8_t)CCID_ERROR_USER_DEFINED;
								}
								LEDShow(1, Bit_RESET);
							}
							else if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_FM1715)
							{
								LEDShow(2, Bit_SET);
								if(stuInRegs.bCardPresent == SA_CARD_PRESENT)
								{
									/* Counter is ordered*/
									if(stuInRegs.bCounterStatus == COUNTER_ENABLE_ORDER)
									{
										stuInRegs.bCounterStatus = COUNTER_ENABLE;
										TIM3Start();                                              
									}

									bTemp = CLTCLAPDU(abAPDUBuffer, iAPDUBufferSendLen, &iAPDUBufferRecvLen);            

									if(stuInRegs.bCounterStatus == COUNTER_ENABLE)
									{
										TIM_Cmd(TIM3, DISABLE);
										TIM_ITConfig(TIM3, TIM_IT_CC1, DISABLE);                         
										stuInRegs.bCounterStatus = COUNTER_ENABLE_ORDER;
									}
								}
								else
									bTemp = CL_TCL_APDU_ERROR;
								if(bTemp != CL_TCL_OK)
								{
									iAPDUBufferRecvLen = 0;
									bStatus = CCID_COMMAND_STATUS_FAIL;
									bError = (uint8_t)CCID_ERROR_USER_DEFINED;
								}
								LEDShow(2, Bit_RESET);
							}
						}

						break;

					case ACT_SET_PARAM:

						if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_TDA8007)
						{
							if(CCIDCardSlotCheck() == CCID_CARD_SLOT_PRESENT)
							{
								bTemp = ContactCardPPS(bmFindexDindex);                    
								// bTemp = CT_T0_OK;
								if(bTemp != CT_T0_OK)
								{
									bStatus = (CCID_COMMAND_STATUS_TIMEOUT | CCID_ICC_STATUS_PRES_ACT);
									bError = (uint8_t)CCID_ERROR_USER_DEFINED;
								}
							}
						}
						else if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_FM1715)
						{
							if(stuInRegs.bATRSource == ATR_SOURCE_CARD)
							{
								if(bmFindexDindex == 0x11)
									bTemp = CL_TCL_OK;
								else
									bTemp = CLCardPPS(bmFindexDindex);  

								bTemp = CL_TCL_OK;
								if(bTemp != CL_TCL_OK)
								{
									bStatus = (CCID_COMMAND_STATUS_TIMEOUT | CCID_ICC_STATUS_PRES_ACT);
									bError = (uint8_t)CCID_ERROR_USER_DEFINED;
								}
							}
						}

					break;
				}
				bReaderStatus = RDS_ORGNIZE;
				break;
			//end of RDS_ACTION
			case RDS_ORGNIZE:
				LEDShow(3, Bit_SET);
				CCIDOrgnize(abAPDUBuffer, iAPDUBufferRecvLen, abBuffer, &iBufferSendLen);
				iBufferSendCnt = 0;
				iBufferRecvCnt = 0;
				bReaderStatus = RDS_SENDING;
				EP2_IN_Callback();
				break;
			//end of RDS_ORGNIZE
			default:

				/*if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_TDA8007)
				{
					if(CCIDCardSlotCheck() != CCID_CARD_SLOT_PRESENT)
					{
						ContactCardPowerOff();
					}
				}      
				*/	
				break;    
		}
	}
}

/*
*********************************************************************************************************
*********************************************************************************************************
*                                          uC/OS-II APP HOOKS
*********************************************************************************************************
*********************************************************************************************************
*/

#if (OS_APP_HOOKS_EN > 0)
/*
*********************************************************************************************************
*                                      TASK CREATION HOOK (APPLICATION)
*
* Description : This function is called when a task is created.
*
* Argument(s) : ptcb   is a pointer to the task control block of the task being created.
*
* Note(s)     : (1) Interrupts are disabled during this call.
*********************************************************************************************************
*/

void  App_TaskCreateHook (OS_TCB *ptcb)
{
#if ((APP_OS_PROBE_EN   == DEF_ENABLED) && \
     (OS_PROBE_HOOKS_EN == DEF_ENABLED))
    OSProbe_TaskCreateHook(ptcb);
#endif
}

/*
*********************************************************************************************************
*                                    TASK DELETION HOOK (APPLICATION)
*
* Description : This function is called when a task is deleted.
*
* Argument(s) : ptcb   is a pointer to the task control block of the task being deleted.
*
* Note(s)     : (1) Interrupts are disabled during this call.
*********************************************************************************************************
*/

void  App_TaskDelHook (OS_TCB *ptcb)
{
    (void)ptcb;
}

/*
*********************************************************************************************************
*                                      IDLE TASK HOOK (APPLICATION)
*
* Description : This function is called by OSTaskIdleHook(), which is called by the idle task.  This hook
*               has been added to allow you to do such things as STOP the CPU to conserve power.
*
* Argument(s) : none.
*
* Note(s)     : (1) Interrupts are enabled during this call.
*********************************************************************************************************
*/

#if OS_VERSION >= 251
void  App_TaskIdleHook (void)
{
}
#endif

/*
*********************************************************************************************************
*                                        STATISTIC TASK HOOK (APPLICATION)
*
* Description : This function is called by OSTaskStatHook(), which is called every second by uC/OS-II's
*               statistics task.  This allows your application to add functionality to the statistics task.
*
* Argument(s) : none.
*********************************************************************************************************
*/

void  App_TaskStatHook (void)
{
}

/*
*********************************************************************************************************
*                                        TASK SWITCH HOOK (APPLICATION)
*
* Description : This function is called when a task switch is performed.  This allows you to perform other
*               operations during a context switch.
*
* Argument(s) : none.
*
* Note(s)     : (1) Interrupts are disabled during this call.
*
*               (2) It is assumed that the global pointer 'OSTCBHighRdy' points to the TCB of the task that
*                   will be 'switched in' (i.e. the highest priority task) and, 'OSTCBCur' points to the
*                  task being switched out (i.e. the preempted task).
*********************************************************************************************************
*/

#if OS_TASK_SW_HOOK_EN > 0
void  App_TaskSwHook (void)
{
#if ((APP_OS_PROBE_EN   == DEF_ENABLED) && \
     (OS_PROBE_HOOKS_EN == DEF_ENABLED))
    OSProbe_TaskSwHook();
#endif
}
#endif

/*
*********************************************************************************************************
*                                     OS_TCBInit() HOOK (APPLICATION)
*
* Description : This function is called by OSTCBInitHook(), which is called by OS_TCBInit() after setting
*               up most of the TCB.
*
* Argument(s) : ptcb    is a pointer to the TCB of the task being created.
*
* Note(s)     : (1) Interrupts may or may not be ENABLED during this call.
*********************************************************************************************************
*/

#if OS_VERSION >= 204
void  App_TCBInitHook (OS_TCB *ptcb)
{
    (void)ptcb;
}
#endif

/*
*********************************************************************************************************
*                                        TICK HOOK (APPLICATION)
*
* Description : This function is called every tick.
*
* Argument(s) : none.
*
* Note(s)     : (1) Interrupts may or may not be ENABLED during this call.
*********************************************************************************************************
*/

#if OS_TIME_TICK_HOOK_EN > 0
void  App_TimeTickHook (void)
{
#if ((APP_OS_PROBE_EN   == DEF_ENABLED) && \
     (OS_PROBE_HOOKS_EN == DEF_ENABLED))
    OSProbe_TickHook();
#endif
}
#endif
#endif
