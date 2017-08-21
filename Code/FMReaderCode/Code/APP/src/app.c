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

static  OS_STK      App_TaskStartStk[APP_TASK_START_STK_SIZE];
static  OS_STK      App_TaskCCIDStk[APP_TASK_CCID_STK_SIZE];
static  OS_STK		App_TaskLedCountStk[APP_TASK_LED_STK_SIZE]; 
						

//static  OS_EVENT      *App_UserIFMbox;
INT8U err;
INT8U LedCount=0;
uint8_t  volatile bUsbStatus = 0;
bool bTimeOut = (bool)FALSE;
bool bIsCTon = (bool)FALSE;




OS_EVENT *usb_Sem;




/*
*********************************************************************************************************
*                                      LOCAL FUNCTION PROTOTYPES
*********************************************************************************************************
*/
static  void  	App_TaskStart        	(void       *p_arg);
static  void  	App_TaskCCID       		(void       *p_arg);
static 	void 	App_TaskLedCount		(void 		*p_arg);


void CCID_handler(void);
//void DebugTDA8035(void);
//void DebugFM320(void);
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

  OSInit();/* Initialize "uC/OS-II, The Real-Time Kernel"*/
	usb_Sem= OSSemCreate(0);
  OSTaskCreate(
								App_TaskStart,
								(void*)0,
								&App_TaskStartStk[APP_TASK_START_STK_SIZE-1],
								APP_TASK_START_PRIO
								);
  OSStart();/* Start multitasking (i.e. give control to uC/OS-II)*/

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


	while(1)
	{
		OSTaskSuspend(OS_PRIO_SELF);
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
	(void)p_arg;
	while(1)
	{
		if(LedCount ==0x20)
		{
			LedCount = 0;
		}
		tmp = LedCount;
		for(i=0; i<4; i++)
		{
			tmp = LedCount >> i;
			if(tmp & 0x01)
				LEDShow(i+1 , Bit_RESET);
			else
				LEDShow(i+1 , Bit_SET);
		}
		LedCount++;
		OSTimeDlyHMSM(0, 0, 0, 500);
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
			if(bUsbStatus == 1) //解决了win10驱动不兼容问题 gaoxuebing 2016-05-18
			{
				bCardSlotStatus = CCID_CARD_SLOT_PRESENT;    
        CCIDCardSlotChange();           
        bUsbStatus++;
			}
			OSSemPend(usb_Sem, 0, &err); //  wait for signal usb_Sem
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
	    //RDS is short for Reader Status
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
				
	    case RDS_ACTION:
				LEDShow(3, Bit_RESET);
	      bStatus = 0;
	      bError = 0;
	      switch(bTemp & 0xF0)
	      {
					case ACT_POWER_ON:
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
	        break;
					case ACT_POWER_OFF:

						__nop();
					break;
				
					//指令交换的分支
					case ACT_APDU_EXCHANGE:	
						if(abAPDUBuffer[0] == 0xFF)
						{
							SpecialAPDU(abAPDUBuffer, iAPDUBufferSendLen, &iAPDUBufferRecvLen);  
						}
						else
						{ 
							LEDShow(3, Bit_SET);
							if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_CT)
								//bTemp=CL_TCL_OK;
								bTemp = ContactCardT0APDU(abAPDUBuffer, iAPDUBufferSendLen, &iAPDUBufferRecvLen);
							else if(stuInRegs.bCurrentDevice == CURRENT_DEVICE_CL)
								bTemp = CLTCLAPDU(abAPDUBuffer, iAPDUBufferSendLen, &iAPDUBufferRecvLen);
							else 
								bTemp=CL_TCL_OK;
							
							if(bTemp!=CL_TCL_OK)
								{
									iAPDUBufferRecvLen = 0;
									bStatus = CCID_COMMAND_STATUS_FAIL;
									bError = (uint8_t)CCID_ERROR_USER_DEFINED;
								}
							LEDShow(3, Bit_RESET);	            
						}
						break;
	
					case ACT_SET_PARAM:
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
