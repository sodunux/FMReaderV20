///////////////////////////////////////////////////////////////////////////////
//    Copyright (c), Philips Semiconductors Gratkorn
//
//                  (C)PHILIPS Electronics N.V.2000
//                     All rights are reserved. 
//  Philips reserves the right to make changes without notice at any time.
// Philips makes no warranty, expressed, implied or statutory, including but
// not limited to any implied warranty of merchantibility or fitness for any
//particular purpose, or that the use will not infringe any third party patent,
// copyright or trademark. Philips must not be liable for any loss or damage
//                          arising from its use.
///////////////////////////////////////////////////////////////////////////////
#include <string.h>
#include <stdio.h>
#include "includes.h"
#include "RICReg.h"
#include "FM1715.h"
#include "PICCCmdConst.h"
#include "MfErrNo.h"


#include "stm32f10x_tim.h"

#include "SpecialAPDU.h"

/* 
* Projekt: MF EV X00 Firmware
*
* $Workfile:: MfRc500uC.c                                               $ 
* $Modtime:: 24.06.02 15:41                                             $ 
* $Author:: Hb                                                          $
* $Revision:: 8                                                         $
*
* 
* This library modul is written for a C166 microcontroller derivative.
* The source can be ported to other platforms very easily. 
* In our case the reader module is 
* connected via memory mapped io at base address 0x100000.
* The interrupt pin of the reader IC is assumed to be connected to 
* the fast external interrupt pin INT0# (active low) and the reset
* pin of the reader IC should be connected to a dedicated port pin
* (Port: P1 Pin: 9).
* In this configuration, a reset of the reader module is independend
* from the reset of the microcontroller.
* All protocoll 
* relevant timing constraints are generated
* by the internal timer of the reader module.
* 
* Some explanations to the programming method of this library.
* There are three kind of functions coded in this module.
*
*  ---- internal functions, which have no prototypes in a header
*       file. This kind of functions are not intended to be used 
*       outside of this file
*  ---- commands, which are intended for the reader module itself
*  ---- commands, which are intended for any tag in the rf field.
*       These commands are send to the reader and the reader module
*       transmitts the data to the rf interface.
*
* Commands for the reader and for the tag have the appropriate 
* prefix (PCD for Proximity Coupled Device or reader module
* PICC for Proximity Integrated Circuit Card or tag)
* and their protypes are defined in the header file.
* Certainly, each command for a PICC consists of an instruction to the PCD. 
* Therefore
* the function PcdSingleResponseCmd is very important for the understanding
* of the communication.
* 
* The basic functionality is provided by the interrupt service
* routine (SingleResponseCmd), which closely works together with the function
* PcdSingleResponseCmd. All kinds of interrupts are serviced by the 
* same ISR. 
*/

///////////////////////////////////////////////////////////////////////////////
//             M O D U L   V A R I A B L E S 
///////////////////////////////////////////////////////////////////////////////

// interrupt vector number for interrupt of the RIC
// select the appropriate value corresponding to the microcontroller in use
#define READER_INT       // e. g. 0x18 
// disable reader interrupt
// select the appropriate value corresponding to the microcontroller in use
#define READER_INT_DISABLE();     do{EXTI_Configuration_FM1715(DISABLE);}while(0);
                              // e. g. EXICON &= ~0x0002
// enable reader interrupt
// select the appropriate value corresponding to the microcontroller in use
#define READER_INT_ENABLE();      do{EXTI_Configuration_FM1715(ENABLE);}while(0);
                              // e. g. EXICON |= 0x0002

// initialize reset pin and change port direction

/* ISO14443 Support Properties
* Some of the protokoll functions of ISO14443 needs information about
* the capability of the reader device, which are provided by this
* constants.
*/
//{
#define TCLFSDSNDMAX   8   ///< max. frame size send
#define TCLFSDRECMAX   8   ///< max. frame size rcv
#define TCLDSMAX       3   ///< max. baudrate divider PICC --> PCD
#define TCLDRMAX       3   ///< max. baudrate divider PCD --> PICC

#define TCLDSDFLT      0   ///< default baudrate divider PICC --> PCD
#define TCLDRDFLT      0   ///< default baudrate divider PCD --> PICC
//}

/* ISR communication structures
* All parameters are passed to the ISR via this structure.
*/
//{
// struct definition for a communication channel between function and ISR
typedef struct 
         {
            unsigned char  cmd;           //!< command code 
            signed char    status;        //!< communication status
            unsigned short nBytesSent;    //!< how many bytes already sent
            unsigned short nBytesToSend;  //!< how many bytes to send
            unsigned short nBytesReceived;//!< how many bytes received
            unsigned long  nBitsReceived; //!< how many bits received
            unsigned char  irqSource;     //!< which interrupts have occured
            unsigned char  collPos;       /*!< at which position occured a
                                          collision*/
            unsigned char  errFlags;      //!< error flags
            unsigned char  saveErrorState;//!< accumulated error flags for
                                          //!< multiple responses
            unsigned char  RxAlignWA;     //!< workaround for RxAlign = 7
            unsigned char  DisableDF;     //!< disable disturbance filter
         } MfCmdInfo;

// Convinience function for initialising the communication structure.
#define ResetInfo(info)    \
            info.cmd            = 0; \
            info.status         = MI_OK;\
            info.irqSource      = 0; \
            info.nBytesSent     = 0; \
            info.nBytesToSend   = 0; \
            info.nBytesReceived = 0; \
            info.nBitsReceived  = 0; \
            info.collPos        = 0; \
            info.errFlags       = 0; \
            info.saveErrorState = 0; \
            info.RxAlignWA      = 0; \
            info.DisableDF      = 0;

// In order to exchange some values between the ISR and the calling function,
// a struct is provided. 
volatile MfCmdInfo     MInfo;                  

// communication info stucture
static   volatile MfCmdInfo     *MpIsrInfo = 0; 
// ISR send buffer
static   volatile unsigned char *MpIsrOut = 0; 
// ISR receive buffer
static   volatile unsigned char *MpIsrIn = 0;   
//}


// storage of the last selected serial number including check byte.
// For multi level serial numbers, only the first 4 bytes are stored.
unsigned char MLastSelectedSnr[5];


// storage buffer for receive and transmit routines
//{
#define MEMORY_BUFFER_SIZE    300
unsigned char MemPool[MEMORY_BUFFER_SIZE];

unsigned char *MSndBuffer = MemPool; // pointer to the transmit buffer
unsigned char *MRcvBuffer = MemPool; // pointer to the receive buffer
//}

/* Higher Baudrate Control
* attention: RegDecoderControl is modified in CascAnticoll
* Because of standard baudrate usage during anticollision, the 
* register can be written. For general purpose usage, only some bits 
* should be set.         
*
* Please pay attention, that the location of the configuration array is
* in ROM space, that means that on 16 bit microcontroller the access 
* should be word aligned.
*/
//{
typedef struct 
         {
            unsigned short  SubCarrierPulses; ///< RegRxControl1
            unsigned short  RxCoding;         ///< RegDecoderControl
            unsigned short  RxThreshold;      ///< RegRxThreshold
            unsigned short  BPSKDemControl;   ///< RegBPSKDemControl
         } t_DSCfg;

typedef struct 
         {
            unsigned short  CoderRate;        ///< RegCoderControl
            unsigned short  ModWidth;         ///< RegModWidth
         } t_DRCfg;
     
const t_DSCfg  MDSCfg[8] = {{0x73,0x08,0xFF,0x00}     // Manchaster 106 kBaud	 
                            ,{0x53,0x09,0xFF,0x0C}     // BPSK 212 kBaud		 
                            ,{0x33,0x09,0xFF,0x0C}     // BPSK 424 kBaud		 
                            ,{0x13,0x09,0xFF,0x0C}     // BPSK 848 kBaud	
							,{0x73,0x19,0xFF,0x00}     // typeB BPSK 106 kBaud	 	   cnn
                            ,{0x53,0x19,0xFF,0x0C}     // typeB BPSK 212 kBaud		 
                            ,{0x33,0x19,0xFF,0x0C}     // typeB BPSK 424 kBaud		 
                            ,{0x13,0x19,0xFF,0x0C}};   // typeB BPSK 848 kBaud
								 
//const t_DRCfg  MDRCfg[4] = {{0x19,0x13}          // Miller 106 kBaud
//                            ,{0x11,0x07}          // Miller 212 kBaud
//                            ,{0x09,0x03}          // Miller 424 kBaud
//                            ,{0x01,0x01}};        // Miller 848 kBaud
const t_DRCfg  MDRCfg[8] = {{0x19,0x13}          // Miller 106 kBaud
                            ,{0x11,0x09}          // Miller 212 kBaud
                            ,{0x09,0x04}          // Miller 424 kBaud
                            ,{0x01,0x01}		  // Miller 848 kBaud
							,{0x20,0x13}         // NRZ typeB  106 kBaud                 cnn
						    ,{0x18,0x13}		 // NRZ typeB  212 kBaud
							,{0x10,0x13}         // NRZ typeB  424 kBaud                 
						    ,{0x08,0x13}};         // NRZ typeB  848 kBaud   


unsigned char PRTCL;	 			//0±íÊ¾ÏÖÔÚµÄ17¼Ä´æÆ÷ÉèÖÃÎªTypeA;1±íÊ¾TypeB
// data send baudrate divider PICC --> PCD
static unsigned char MDSI = TCLDSDFLT;    

// data send baudrate divider PCD --> PICC
static unsigned char MDRI = TCLDRDFLT;   
//}

extern uint8_t   bSendBlockType;

/*******************************************************************************
* Function Name  : TIM4Config.
* Description    : Config the TIM4 of MCU used for delay and width timer.
* Input          : TimeBase : 1=1us, 1000=1ms.
                   Counter  : How many timebase unit should be counted. 
* Output         : None
* Return         : None
*******************************************************************************/
void TIM4Config(uint16_t  TimeBase, uint16_t Counter)
{
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
  TIM_OCInitTypeDef  TIM_OCInitStructure;
  /* ---------------------------------------------------------------
    TIM4 Configuration: Output Compare Timing Mode:
    TIM4 counter clock at 1 MHz
  --------------------------------------------------------------- */  

  /* Time base configuration */
  TIM_TimeBaseStructure.TIM_Period = 65535;
  TIM_TimeBaseStructure.TIM_Prescaler = 0;
  TIM_TimeBaseStructure.TIM_ClockDivision = 0;
  TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;

  TIM_TimeBaseInit(TIM4, &TIM_TimeBaseStructure);

  /* Prescaler configuration */
  if(TimeBase == 1)
    TIM_PrescalerConfig(TIM4, 71, TIM_PSCReloadMode_Immediate);
  else if(TimeBase == 1000)
    TIM_PrescalerConfig(TIM4, 35999, TIM_PSCReloadMode_Immediate);    

  /* Output Compare Timing Mode configuration: Channel1 */
  TIM_OCInitStructure.TIM_OCMode = TIM_OCMode_Timing;
  TIM_OCInitStructure.TIM_OutputState = TIM_OutputState_Enable;
  if(TimeBase == 1)
    TIM_OCInitStructure.TIM_Pulse = Counter;
  else if(TimeBase == 1000)
    TIM_OCInitStructure.TIM_Pulse = Counter;
  TIM_OCInitStructure.TIM_OCPolarity = TIM_OCPolarity_High;

  TIM_OC1Init(TIM4, &TIM_OCInitStructure);

  TIM_OC1PreloadConfig(TIM4, TIM_OCPreload_Disable);

  TIM_ITConfig(TIM4, TIM_IT_CC1, ENABLE);
  TIM_Cmd(TIM4, ENABLE);
}

/*******************************************************************************
* Function Name  : WriteRawRC.
* Description    : Write FM1715 register.
* Input          : Addr : register address
                   Val  : register value
* Output         : None
* Return         : None
*******************************************************************************/
void WriteRawRC(unsigned char Addr, unsigned char Val)
{
  unsigned char b;
  uint16_t  i;
  
  i = (uint16_t)FM1715_DATA_PORT->ODR;
  GPIO_Write(FM1715_DATA_PORT, (i&0x00FF)|(Addr<<8));
  for(b=0; b<3; b++);
  READER_ALE_SET();
  for(b=0; b<3; b++);
  READER_ALE_RESET();

  READER_NCS_RESET();
  i = (uint16_t)FM1715_DATA_PORT->ODR;
  GPIO_Write(FM1715_DATA_PORT, (i&0x00FF)|(Val<<8));
  for(b=0; b<3; b++);
  READER_NWR_RESET();
  for(b=0; b<3; b++);

  READER_NWR_SET();
  READER_NCS_SET();
}

/*******************************************************************************
* Function Name  : ReadRawRC.
* Description    : Read FM1715 register.
* Input          : Addr : register address
* Output         : None
* Return         : register value
*******************************************************************************/
unsigned char ReadRawRC(unsigned char Addr)
{
  unsigned char b;
  uint16_t  i;
  
  i = (uint16_t)FM1715_DATA_PORT->ODR;
  GPIO_Write(FM1715_DATA_PORT, (i&0x00FF)|(Addr<<8));
  for(b=0; b<3; b++);
  READER_ALE_SET();
  for(b=0; b<3; b++);
  READER_ALE_RESET();

  READER_NCS_RESET();
  for(b=0; b<3; b++);
  
  TDA8007_DATA_PORT->CRH = 0x88888888;
  TDA8007_DATA_PORT->BSRR = 0x0000FF00;  
  READER_NRD_RESET();
  for(b=0; b<3; b++);
  b = (uint8_t)((uint16_t)(FM1715_DATA_PORT->IDR)>>8);

  READER_NRD_SET();
  READER_NCS_SET();  
  TDA8007_DATA_PORT->CRH = 0x33333333;

  return b;
}

//! Open Reader IC Connection
/*!
* -o  none
* return: MI_OK
*         other    error opening reader ic channel
*
* Open and initialize communication channel to the reader module
*/
char OpenRC(void);

//! Write one byte to the reader IC address space
/*!
* -o  address  (IN) reader ic register address
* -o  value    (IN) 8 bit value
* return: none
*
* This function determines the necessary page address of the 
* reader module and writes the page number to the page 
* register and the value to the specified address.
*/
void WriteRC(unsigned char Address, unsigned char value);

//! Write one byte to the reader IC address space
/*
* -o  address   (IN) reader IC register address
* return: value    8 bit data, read from the reader ic address space
*
* This function determines the necessary page address of the 
* reader module and writes the page number to the page 
* register and reads the value from the specified address.
*/
unsigned char ReadRC(unsigned char Address);

//! Close reader IC communication channel
/*!
* -o  none
* return: none
*
* Closing the communication channel to the reader module
*/
void CloseRC(void);

// In case of a parallel connection, the address space of the reader module
// needs to be mapped to the address space of the microcontroller. Therefore
// a base address is reserved.
///////////////////////////////////////////////////////////////////////////////
//                 Open Reader Communication
///////////////////////////////////////////////////////////////////////////////
char OpenRC(void)
{
   signed char status = MI_OK;
   READER_RST_RESET(); 
   return status;
}

///////////////////////////////////////////////////////////////////////////////
//                 Close Reader Communication
///////////////////////////////////////////////////////////////////////////////
void CloseRC(void)
{
}

///////////////////////////////////////////////////////////////////////////////
//          G E N E R I C    W R I T E
///////////////////////////////////////////////////////////////////////////////
void WriteRC(unsigned char Address, unsigned char value)
{
  WriteRawRC(RegPage, 0x80|(Address>>3));
  WriteRawRC(Address,value);              // write value at the specified 
                                           // address
}

///////////////////////////////////////////////////////////////////////////////
//          G E N E R I C    R E A D
///////////////////////////////////////////////////////////////////////////////
unsigned char ReadRC(unsigned char Address)
{
  WriteRawRC(RegPage, 0x80|(Address>>3));
  return ReadRawRC(Address);              // read value at the specified 
}  

///////////////////////////////////////////////////////////////////////////////
//             Prototypes for local functions 
///////////////////////////////////////////////////////////////////////////////

///  Internal Authentication State Switch
/*!
* -o  auth_mode (IN) 
*                  enum: selects master key A or master key B 
*                   - PICC_AUTHENT1A
*                   - PICC_AUTHENT1B 
*                  
* -o  *snr      (IN) 
*                  4 byte serial number of the card, which should be 
*                  authenticated
* -o  sector (IN) Range [0..15] 
*               specifies the key RAM address 
*               from which the keys should be taken
* return: enum:
*          - MI_OK
*          - CCE
*          - MI_BITCOUNTERR  wrong number of bits received
*          - MI_AUTHERR      wrong keys for selected card
*          - MI_KEYERR       error while loading keys
*         
* 
* Internal authentication state function.
*/
signed char Mf500PiccAuthState(unsigned char auth_mode,// PICC_AUTHENT1A, PICC_AUTHENT1B
                       unsigned char *snr,    // 4 byte serial number
                       unsigned char sector); // 0 <= sector <= 15  
                                            // sector address for authentication

/// Write Baudrate Divider
/*!
* -o     none
* return:   MI_OK
*
* Write function for baudrate divider and PCD properties
*/
signed char  Mf500PcdWriteAttrib(void);

/* ISR Communication Structure
* Data, which have to be passed between ISR and other functions are
* colleted within one structure. 
*/ 
//{
// ISR for Single Response Commands 
/*
* -o  none
* return: none
*
* This function is a central routine in the communication chain between
* PCD and PICC. 
*/ 
//interrupt (READER_INT) 
void SingleResponseIsr(void);

// Command issuer for Single Response Commands 
/*
* -o  cmd  (IN)
*             Command type
*             enum:
*              - PCD_IDLE
*              - PCD_WRITEE2
*              - PCD_READE2
*              - PCD_LOADCONFIG
*              - PCD_LOADKEYE2
*              - PCD_AUTHENT1
*              - PCD_CALCCRC
*              - PCD_AUTHENT2
*              - PCD_RECEIVE
*              - PCD_LOADKEY
*              - PCD_TRANSMIT
*              - PCD_TRANSCEIVE
*             
*       send  (IN)
*             byte stream of variable length, which should be send to
*             the PICC, the length of stream has to be specified
*             in the info - structure
*       rcv   (OUT) 
*             byte stream of variable length, which was received 
*             from the PICC. The length can be obtained from the
*             info - structure
*       info  (OUT)
*             communication and status structure
* return: enum:
*          - MI_OK               operation without error
*          - MI_NOTAGERR         no tag in rf field
*          - MI_ACCESSTIMEOUT    RIC is not responding in time
*          - MI_COLLERR          collision in during rf data transfer
*          - MI_PARITYERR        parity error while receiving data
*          - MI_FRAMINGERR       framing error - start bit not valid
*          - MI_OVFLERR          FIFO overflow - to many data bytes
*          - MI_CRCERR           CRC error of received data
*          - MI_NY_IMPLEMENTED   internal error - source not identified
*         
*
* This function provides the central interface to the reader module.
* Depending on the "cmd"-value, all necessary interrupts are enabled
* and the communication is started. While the processing is done by
* the reader module, this function waits for its completion.
*
* It's notable, that the data in the send byte stream is written 
* to the FIFO of the reader module by the ISR itself. Immediate after 
* enabling the interrupts, the LoAlert interrupt is activated.
*
* The ISR writes the data to the FIFO. This function is not directly involved
* in writing or fetching data from FIFO, all work is done by the 
* corresponding ISR.After command completion, the error status is evaluated and 
* returned to the calling function.
*/ 
signed char PcdSingleResponseCmd(unsigned char cmd,
                volatile unsigned char* send, 
                volatile unsigned char* rcv,
                volatile MfCmdInfo *info);

/// Basic Register definitions
/*!
* return: none
*/
signed char PcdBasicRegisterConfiguration(void);

/// Set Reader IC Register Bit
/*!
* -o  reg  (IN)
*             register address
* -o  mask (IN)
*             Bit mask to set
* return:     none
*              
* This function performs a read - modify - write sequence
* on the specified register. All bits with a 1 in the mask
* are set - all other bits keep their original value.
*/
void SetBitMask(unsigned char reg,unsigned char mask);

/// Clear Reader IC Register Bit
/*!
* -o  reg  (IN)
*             register address
* -o  mask (IN)
*             Bit mask to clear
* return: none
*              
* This function performs a read - modify - write sequence
* on the specified register. All bits with a 1 in the mask
* are cleared - all other bits keep their original value.
*/
void ClearBitMask(unsigned char reg,unsigned char mask);

/// Flush remaining data from the FIFO
/*!
* -o  none
* return: none
*              
* This function erases  all remaining data in the MF RC 500's FIFO .
* Before writing new data or starting a new command, all remaining data 
* from former  commands should be deleted.
*/
void FlushFIFO(void);

/// Sleep several milliseconds
/*
The implementation of this function depends heavily
on the microcontroller in use. The measurement need not to be
very accurate. Only make sure, that the periode is not shorter, than
the required one.
*/
void SleepMs(unsigned short ms)
{
  TIM4Config(1000, ms);
  while(TIM_GetITStatus(TIM4, TIM_IT_CC1) == RESET);
  TIM_ClearITPendingBit(TIM4, TIM_IT_CC1);
  TIM_ITConfig(TIM4, TIM_IT_CC1, DISABLE);
  TIM_Cmd(TIM4, DISABLE);

  TIM4Config(1000, ms);
  while(TIM_GetITStatus(TIM4, TIM_IT_CC1) == RESET);
  TIM_ClearITPendingBit(TIM4, TIM_IT_CC1);
  TIM_ITConfig(TIM4, TIM_IT_CC1, DISABLE);
  TIM_Cmd(TIM4, DISABLE);
}

/// Sleep several microseconds
/*
The implementation of this function depends heavily
on the microcontroller in use. The measurement need not to be
very accurate. Only make sure, that the periode is not shorter, than
the required one.
*/
void SleepUs(unsigned short us)
{
  TIM4Config(1, us);
  while(TIM_GetITStatus(TIM4, TIM_IT_CC1) == RESET);
  TIM_ClearITPendingBit(TIM4, TIM_IT_CC1);
  TIM_ITConfig(TIM4, TIM_IT_CC1, DISABLE);
  TIM_Cmd(TIM4, DISABLE);
}


///////////////////////////////////////////////////////////////////////
//      M I F A R E   M O D U L E   C O N F I G U R A T I O N
///////////////////////////////////////////////////////////////////////
signed char Mf500PcdConfig(void)
{
   signed char status = MI_RESETERR;
      
   status = PcdReset();

   if (status == MI_OK)
   {
     if ((status = PcdBasicRegisterConfiguration()) == MI_OK);
     {
	    if(PRTCL == 1)               // ÈôTypeB MDSI+4×ªµ½Êý×éTypeBµÄ¶¨Òå      cnn
		{
			MDSI = MDSI+4;
			MDRI = MDSI+4;  
		}
        Mf500PcdWriteAttrib(); // write current modulation parameters
        PcdRfReset(1); // Rf - reset and enable output driver   
        SleepMs(10); 
     }
   }
   return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   R E M O T E   A N T E N N A
//  Configuration of slave module
///////////////////////////////////////////////////////////////////////
signed char Mf500ActiveAntennaSlaveConfig(void)
{
   char status = MI_OK;

   FlushFIFO();    // empty FIFO
   ResetInfo(MInfo);   
   MSndBuffer[0] = 0x10; // addr low byte
   MSndBuffer[1] = 0x00; // addr high byte

   MSndBuffer[2] = 0x00; // Page
   MSndBuffer[3] = 0x7B; // RegTxControl modsource 11,InvTx2,Tx2RFEn,TX1RFEn
   MSndBuffer[4] = 0x3F; // RegCwConductance
   MSndBuffer[5] = 0x3F; // RFU13
   MSndBuffer[6] = 0x19; // RFU14
   MSndBuffer[7] = 0x13; // RegModWidth     
   MSndBuffer[8] = 0x00; // RFU16
   MSndBuffer[9] = 0x00; // RFU17
 
   MSndBuffer[10] = 0x00; // Page
   MSndBuffer[11] = 0x73; // RegRxControl1 
   MSndBuffer[12] = 0x08; // RegDecoderControl
   MSndBuffer[13] = 0x6c; // RegBitPhase     
   MSndBuffer[14] = 0xFF; // RegRxThreshold  
   MSndBuffer[15] = 0x00; // RegBPSKDemControl
   MSndBuffer[16] = 0x00; // RegRxControl2   
   MSndBuffer[17] = 0x00; // RegClockQControl

   MSndBuffer[18] = 0x00; // Page
   MSndBuffer[19] = 0x06; // RegRxWait
   MSndBuffer[20] = 0x03; // RegChannelRedundancy
   MSndBuffer[21] = 0x63; // RegCRCPresetLSB    
   MSndBuffer[22] = 0x63; // RegCRCPresetMSB    
   MSndBuffer[23] = 0x0;  // RFU25
   MSndBuffer[24] = 0x04; // RegMfOutSelect enable mfout = manchester HT
   MSndBuffer[25] = 0x00; // RFU27
     
   // PAGE 5      FIFO, Timer and IRQ-Pin Configuration
   MSndBuffer[26] = 0x00; // Page
   MSndBuffer[27] = 0x08; // RegFIFOLevel       
   MSndBuffer[28] = 0x07; // RegTimerClock      
   MSndBuffer[29] = 0x06; // RegTimerControl    
   MSndBuffer[30] = 0x0A; // RegTimerReload     
   MSndBuffer[31] = 0x02; // RegIRqPinConfig    
   MSndBuffer[32] = 0x00; // RFU    
   MSndBuffer[33] = 0x00; // RFU
   MInfo.nBytesToSend   = 34;
         
   status = PcdSingleResponseCmd(PCD_WRITEE2,
                   MSndBuffer,
                   MRcvBuffer,
                   &MInfo); // write e2
   return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   R E M O T E   A N T E N N A
//  Configuration of master module
///////////////////////////////////////////////////////////////////////
signed char Mf500ActiveAntennaMasterConfig(void)
{
   char status = MI_OK;

   WriteRC(RegRxControl2,0x42);
   WriteRC(RegTxControl,0x10);
   WriteRC(RegBitPhase,0x11);
   WriteRC(RegMfOutSelect,0x02);

   return status;
}     

uint8_t bCLIntCnt;   
uint8_t abCLIntSrc[100];
///////////////////////////////////////////////////////////////////////
//          M I F A R E    R E Q U E S T 
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccRequest(unsigned char req_code, // request code ALL = 0x52 
                                           // or IDLE = 0x26 
                   unsigned char *atq)     // answer to request
{
//  for(bCLIntCnt=0; bCLIntCnt<100; bCLIntCnt++)
//    abCLIntSrc[bCLIntCnt] = 0;
//  bCLIntCnt = 0;  

  return Mf500PiccCommonRequest(req_code,atq);
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   C O M M O N   R E Q U E S T 
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccCommonRequest(unsigned char req_code, 
                            unsigned char *atq)
{
	signed char status = MI_OK;

	//************* initialize ******************************
	if ((status = Mf500PcdSetDefaultAttrib()) == MI_OK)
	{

		// PcdSetTmo(60);
		PcdSetTmo(120);

		WriteRC(RegChannelRedundancy,0x03); // RxCRC and TxCRC disable, parity enable
		ClearBitMask(RegControl,0x08);      // disable crypto 1 unit   
		WriteRC(RegBitFraming,0x07);        // set TxLastBits to 7 

		//      ReadAllRegs();

		ResetInfo(MInfo);   
		MSndBuffer[0] = req_code;
		MInfo.nBytesToSend   = 1;   
		MInfo.DisableDF = 1;
		status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
		                 MSndBuffer,
		                 MRcvBuffer,
		                 &MInfo);
		if ((status == MI_OK) && (MInfo.nBitsReceived != 16)) // 2 bytes expected
		{
		 	status = MI_BITCOUNTERR;
		} 
		if ((status == MI_COLLERR) && (MInfo.nBitsReceived == 16)) //
		 	status = MI_OK; // all received tag-types are combined to the 16 bit
		 
		// in any case, copy received data to output - for debugging reasons
		if (MInfo.nBytesReceived >= 2)
		{
		 	memcpy(atq,MRcvBuffer,2);      
		}
		else
		{
			if (MInfo.nBytesReceived == 1)
				atq[0] = MRcvBuffer[0];
			else
				atq[0] = 0x00;
			atq[1] = 0x00;
		}
	}
	//   ReadAllRegs();
	return status; 
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E    A N T I C O L L I S I O N
// for standard select
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccAnticoll (unsigned char bcnt,
                     unsigned char *snr)
{
	return Mf500PiccCascAnticoll(0x93,bcnt,snr); // first cascade level
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E    A N T I C O L L I S I O N
// for extended serial numbers
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccCascAnticoll (unsigned char select_code,
                         unsigned char bcnt,       
                         unsigned char *snr)       
{
	signed char  status = MI_OK;
	char  snr_in[4];         // copy of the input parameter snr
	char  nbytes = 0;        // how many bytes received
	char  nbits = 0;         // how many bits received
	char  complete = 0;      // complete snr recived
	short i        = 0;
	char  byteOffset = 0;
	unsigned char snr_crc;   // check byte calculation
	unsigned char snr_check;
	unsigned char dummyShift1;       // dummy byte for snr shift
	unsigned char dummyShift2;       // dummy byte for snr shift   
 
	//************* Initialisierung ******************************
	if ((status = Mf500PcdSetDefaultAttrib()) == MI_OK)
	{
		PcdSetTmo(106);

		memcpy(snr_in,snr,4);   

		WriteRC(RegDecoderControl,0x28); // ZeroAfterColl aktivieren   
		ClearBitMask(RegControl,0x08);    // disable crypto 1 unit

		//************** Anticollision Loop ***************************
		complete=0;
		while (!complete && (status == MI_OK) )
		{
			// if there is a communication problem on the RF interface, bcnt 
			// could be larger than 32 - folowing loops will be defective.
			if (bcnt > 32)
			{
				status = MI_WRONG_PARAMETER_VALUE;
				continue;
			}
			ResetInfo(MInfo);
			MInfo.cmd = select_code;   // pass command flag to ISR        
			MInfo.DisableDF = 1;
			WriteRC(RegChannelRedundancy,0x03); // RxCRC and TxCRC disable, parity enable
			nbits = bcnt % 8;   // remaining number of bits
			if (nbits)
			{
				WriteRC(RegBitFraming,nbits << 4 | nbits); // TxLastBits/RxAlign auf nb_bi
				nbytes = bcnt / 8 + 1;   
				// number of bytes known

				// in order to solve an inconsistancy in the anticollision sequence
				// (will be solved soon), the case of 7 bits has to be treated in a
				// separate way
				if (nbits == 7 )
				{
					MInfo.RxAlignWA = 1;
					MInfo.nBitsReceived = 7; // set flag for 7 bit anticoll, which is evaluated
					// in the ISRnBitsReceived        
					WriteRC(RegBitFraming,nbits); // reset RxAlign to zero
				}
			} 
			else
			{
				nbytes = bcnt / 8;
			}

			MSndBuffer[0] = select_code;
			MSndBuffer[1] = 0x20 + ((bcnt/8) << 4) + nbits; //number of bytes send

			for (i = 0; i < nbytes; i++)  // Sende Buffer beschreiben
			{
				MSndBuffer[i + 2] = snr_in[i];
			}
			MInfo.nBytesToSend   = 2 + nbytes;    

			status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
								MSndBuffer,
								MRcvBuffer,
								&MInfo);

			// in order to solve an inconsistancy in the anticollision sequence
			// (will be solved soon), the case of 7 bits has to be treated in a
			// separate way 
			if (MInfo.RxAlignWA)
			{
				// reorder received bits
				dummyShift1 = 0x00;
				for (i = 0; i < MInfo.nBytesReceived; i++)
				{
					dummyShift2 = MRcvBuffer[i];
					MRcvBuffer[i] = (dummyShift1 >> (i+1)) | (MRcvBuffer[i] << (7-i));
					dummyShift1 = dummyShift2;
				}
				MInfo.nBitsReceived -= MInfo.nBytesReceived; // subtract received parity bits
				// recalculation of collision position
				if ( MInfo.collPos ) 
					MInfo.collPos += 7 - (MInfo.collPos + 6) / 9;
			}

			if ( status == MI_OK || status == MI_COLLERR)    // no other occured
			{
				byteOffset = 0;
				if ( nbits != 0 )           // last byte was not complete
				{
					snr_in[nbytes - 1] = snr_in[nbytes - 1] | MRcvBuffer[0];
					byteOffset = 1;
				}
				for ( i =0; i < (4 - nbytes); i++)     
				{
					snr_in[nbytes + i] = MRcvBuffer[i + byteOffset];
				}
				// R e s p o n s e   P r o c e s s i n g   
				if ( MInfo.nBitsReceived != (40 - bcnt) ) // not 5 bytes answered
				{
					status = MI_BITCOUNTERR;
				} 
				else 
				{
					if (status != MI_COLLERR ) // no error and no collision
					{
						// SerCh check
						snr_crc = snr_in[0] ^ snr_in[1] ^ snr_in[2] ^ snr_in[3];
						snr_check = MRcvBuffer[MInfo.nBytesReceived - 1];
						if (snr_crc != snr_check)
						{
							status = MI_SERNRERR;
						} 
						else   
						{
							complete = 1;
						}
					}
					else                   // collision occured
					{
						bcnt = bcnt + MInfo.collPos - nbits;
						status = MI_OK;
					}
				}
			}
		}
	}
	// transfer snr_in to snr - even in case of an error - for 
	// debugging reasons
	memcpy(snr,snr_in,4);

	//----------------------Einstellungen aus Initialisierung ruecksetzen 
	ClearBitMask(RegDecoderControl,0x20); // ZeroAfterColl disable

	return status;  
}

//add by gaoxuebing
signed char Mf500PiccAnticollWithBCC (unsigned char select_code,
                         unsigned char *snr)       
{
	signed char  status = MI_OK;
	char  snr_in[5];         // copy of the input parameter snr
	char  nbytes = 0;        // how many bytes received
	short i        = 0;
	char  byteOffset = 0;
	unsigned char snr_crc;   // check byte calculation
	unsigned char snr_check;

	//************* Initialisierung ******************************
	if ((status = Mf500PcdSetDefaultAttrib()) == MI_OK)
	{
		PcdSetTmo(106);

		memcpy(snr_in,snr,4);   

		WriteRC(RegDecoderControl,0x28); // ZeroAfterColl aktivieren   
		ClearBitMask(RegControl,0x08);    // disable crypto 1 unit

		//************** Anticollision Loop ***************************

		// if there is a communication problem on the RF interface, bcnt 
		// could be larger than 32 - folowing loops will be defective.

		ResetInfo(MInfo);
		MInfo.cmd = select_code;   // pass command flag to ISR        
		MInfo.DisableDF = 1;
		WriteRC(RegChannelRedundancy,0x03); // RxCRC and TxCRC disable, parity enable


		MSndBuffer[0] = select_code;
		MSndBuffer[1] = 0x20; //number of bytes send
		      

		MInfo.nBytesToSend   = 2;    

		status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
				                MSndBuffer,
				                MRcvBuffer,
				                &MInfo);
		if(status == MI_OK)
		{
			byteOffset = 0;

			for ( i =0; i < 5; i++)     
			{
			   	snr_in[nbytes + i] = MRcvBuffer[i + byteOffset];
			}


			// SerCh check
			snr_crc = snr_in[0] ^ snr_in[1] ^ snr_in[2] ^ snr_in[3];
			snr_check = MRcvBuffer[MInfo.nBytesReceived - 1];
			if (snr_crc != snr_check)
			{
				status = MI_SERNRERR;
			} 
			else   
			{
				status = MI_OK;
			}

		 }
	}
	// transfer snr_in to snr - even in case of an error - for 
	// debugging reasons
	memcpy(snr,snr_in,5);

	//----------------------Einstellungen aus Initialisierung ruecksetzen 
	ClearBitMask(RegDecoderControl,0x20); // ZeroAfterColl disable

	return status;  
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E    S E L E C T 
// for std. select
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccSelect(unsigned char *snr, 
                  unsigned char *sak)
{
	return Mf500PiccCascSelect(0x93,snr,sak, 1); // first cascade level
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E    C A S C A D E D   S E L E C T 
//  for extended serial number
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccCascSelect(unsigned char select_code, 
                        unsigned char *snr,
                        unsigned char *sak,
						unsigned char check_BCC)
{
	signed char   status = MI_OK; 

	if ((status = Mf500PcdSetDefaultAttrib()) == MI_OK)
	{
		PcdSetTmo(106);

		WriteRC(RegChannelRedundancy,0x0F); // RxCRC,TxCRC, Parity enable
		ClearBitMask(RegControl,0x08);    // disable crypto 1 unit

		//************* Cmd Sequence ********************************** 
		ResetInfo(MInfo);   
		MSndBuffer[0] = select_code;
		MSndBuffer[1] = 0x70;         // number of bytes send

		//2015.8.17 by CJN
		 if(check_BCC == 0)	//no check BCC
		 {
			 memcpy(MSndBuffer + 2,snr,5);
		 }
		 else		//send UID with right BCC
		 {
			 memcpy(MSndBuffer + 2,snr,4);
			 MSndBuffer[6] = MSndBuffer[2] 
				                      ^ MSndBuffer[3] 
				                      ^ MSndBuffer[4] 
				                      ^ MSndBuffer[5];
		 }
		MInfo.nBytesToSend   = 7;
		MInfo.DisableDF = 1;
		status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                          MSndBuffer,
                          MRcvBuffer,
                          &MInfo);
   
		*sak = 0;   
		if (status == MI_OK)    // no timeout occured
		{
			if (MInfo.nBitsReceived != 8)    // last byte is not complete
			{
				status = MI_BITCOUNTERR;
			}
			else
			{
				memcpy(MLastSelectedSnr,snr,4);            
			}
		}
		// copy received data in any case - for debugging reasons
		*sak = MRcvBuffer[0];
	}
	return status;
}

///////////////////////////////////////////////////////////////////////
//       M I F A R E   P I C C   A C T I V A T I O N    S E Q E N C E
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccActivateIdle(unsigned char br,
                           unsigned char *atq, 
                           unsigned char *sak, 
                           unsigned char *uid, 
                           unsigned char *uid_len)
{
  unsigned char cascade_level;
  unsigned char sel_code;
  unsigned char uid_index;
  signed char status;
  unsigned char cmdASEL;

  *uid_len      = 0;

  //call activation with def. divs
  status = Mf500PcdSetDefaultAttrib();
  if (status == MI_OK)
  {
     status = Mf500PiccCommonRequest(PICC_REQIDL,atq);
  }
  if (status == MI_OK)
  {
     if((atq[0] & 0x1F) == 0x00) // check lower 5 bits, for tag-type
                                 // all tags within this 5 bits have to
                                 // provide a bitwise anticollision
     {
        status = MI_NOBITWISEANTICOLL;
     }
  }
  if (status == MI_OK)
  {
      //Get UID in 1 - 3 levels (standard, [double], [triple] )
      //-------
      switch(br)
      {
         case 0: cmdASEL = PICC_ANTICOLL1; break;
         case 1: cmdASEL = PICC_ANTICOLL11; break;
         case 2: cmdASEL = PICC_ANTICOLL12; break;
         case 3: cmdASEL = PICC_ANTICOLL13; break;
         default:
              status = MI_BAUDRATE_NOT_SUPPORTED; break;
      }
  }
  if (status == MI_OK)
  {
      cascade_level = 0;
      uid_index     = 0;
      do
      {
        //Select code depends on cascade level
        sel_code   = cmdASEL + (2 * cascade_level);
        cmdASEL = PICC_ANTICOLL1; // reset anticollistion level for calculation
        //ANTICOLLISION
        status = Mf500PiccCascAnticoll(sel_code, 0, &uid[uid_index]);
        //SELECT
        if (status == MI_OK)
        {
           status = Mf500PiccCascSelect(sel_code, &uid[uid_index], sak, 1);
           if (status == MI_OK)
           {
              cascade_level++;

              //we differ cascaded and uncascaded UIDs
              if (*sak & 0x04) // if cascaded, bit 2 is set in answer to select
              {
                 //this UID is cascaded, remove the cascaded tag that is
                 //0x88 as first of the 4 byte received
                 memmove(&uid[uid_index], &uid[uid_index + 1], 3);
                 uid_index += 3;
                 *uid_len += 3;
              }
              else
              {
                 //this UID is not cascaded -> the length is 4 bytes
                 uid_index += 4;
                 *uid_len += 4;
              }
           }
        }
      }
      while((status == MI_OK)        // error status
            && (*sak & 0x04)         // no further cascade level
            && (cascade_level < 3)); // highest cascade level is reached
   }
   if (status == MI_OK)
   {
      //Exit function, if cascade level is triple and sak indicates another
      //cascase level.
      if ((cascade_level == 3) && (*sak & 0x04))
      {
         *uid_len = 0;
         status = MI_CASCLEVEX;
      }
      Mf500PcdSetAttrib(br,br);
   }
   return (status);
}

///////////////////////////////////////////////////////////////////////
//       M I F A R E   P I C C   A C T I V A T I O N    S E Q E N C E
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccActivateWakeup(unsigned char br,
                             unsigned char *atq, 
                             unsigned char *sak,
                             unsigned char *uid, 
                             unsigned char uid_len)
{
   unsigned char cascade_level;
   unsigned char uid_index;
   unsigned char tmpuid[4];
   unsigned char sel_code;
   unsigned char cmdASEL;
   signed char   status;

   //call activation with def. divs
   status = Mf500PcdSetDefaultAttrib();
   if (status == MI_OK)
   {
      status = Mf500PiccCommonRequest(PICC_REQALL,atq);
   }
   if (status == MI_OK)
   {
      if ((atq[0] & 0x1F) == 0x00) // check lower 5 bits, for tag-type
                                   // all tags within this 5 bits have to
                                   // provide a bitwise anticollision
      {
         status = MI_NOBITWISEANTICOLL;
      }
   }
   if (status == MI_OK)
   {
      //Get UID in 1 - 3 levels (standard, [double], [triple] )
      //-------
      switch(br)
      {
         case 0: cmdASEL = PICC_ANTICOLL1; break;
         case 1: cmdASEL = PICC_ANTICOLL11; break;
         case 2: cmdASEL = PICC_ANTICOLL12; break;
         case 3: cmdASEL = PICC_ANTICOLL13; break;
         default:
              status = MI_BAUDRATE_NOT_SUPPORTED; break;
      }
   }
   if (status == MI_OK)
   {
      //Select UID in up to 3 cascade levels (standard, [double], [triple] )
      //------------------------------------
      cascade_level = 0;
      uid_index     = 0;
      tmpuid[0] = 0x88;     //first byte of cascaded UIDs is 0x88 (cascaded tag)

      do
      {
        sel_code   = cmdASEL + (2 * cascade_level);
        cmdASEL = PICC_ANTICOLL1; // reset anticollistion level for calculation
        //get the next UID part if we need to cascade
        if((uid_len - uid_index) > 4)
        {
          //ok, we need to cascade the UID
          memcpy(&tmpuid[1], &uid[uid_index], 3);
          uid_index += 3;
        }
        else
        {
          //ah, how nice. no need to cascade
          memcpy(tmpuid, &uid[uid_index], 4);
          uid_index += 4;
        }

        status = Mf500PiccCascSelect(sel_code, tmpuid, sak, 1);

        if(status == MI_OK)
        {
          cascade_level++;
        }
      }
      while((status == MI_OK )    // error occured
            && (*sak & 0x04)       // no further cascade level
            && ((uid_index + 1) < uid_len) // all bytes of snr sent
            && (cascade_level < 3)); // highest cascade level reached
   }
   if ( status == MI_OK) 
   {
      //Exit function, if UID length is not of expected length
      if ((uid_index) != uid_len)
      {
         status =  MI_SERNRERR ;
      }
   }
   if (status == MI_OK)
   {
      //Exit function, if cascade level is triple and sak indicates another
      //cascase level.
      if ((cascade_level == 3) && (*sak & 0x04))
      {
         status = MI_SERNRERR;
      }
   }
   return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E      A U T H E N T I C A T I O N
//   calling compatible version    
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccAuth(unsigned char key_type,    // PICC_AUTHENT1A or PICC_AUTHENT1B
                   unsigned char key_addr,    // key address in reader storage
                   unsigned char block)       // block number which should be 
                                              // authenticated
{
   char            status = MI_OK;

   status = Mf500PiccAuthE2(  key_type,
                              MLastSelectedSnr,
                              key_addr,
                              block);
   return status;
}

///////////////////////////////////////////////////////////////////////
//                  A U T H E N T I C A T I O N   
//             W I T H   K E Y S   F R O M   E 2 P R O M
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccAuthE2(   unsigned char auth_mode,   // PICC_AUTHENT1A or PICC_AUTHENT1B
                     unsigned char *snr,        // 4 bytes card serial number
                     unsigned char key_sector,  // 0 <= key_sector <= 15                     
                     unsigned char block)      //  0 <= block <= 256
{
   char status = MI_OK;
   // eeprom address calculation
   // 0x80 ... offset
   // key_sector ... sector
   // 0x18 ... 2 * 12 = 24 = 0x18
   unsigned short e2addr = 0x80 + key_sector * 0x18;
   unsigned char *e2addrbuf = (unsigned char*)&e2addr;
   
   PcdSetTmo(106);
   if (auth_mode == PICC_AUTHENT1B)
      e2addr += 12; // key B offset   
   FlushFIFO();    // empty FIFO
   ResetInfo(MInfo);

   memcpy(MSndBuffer,e2addrbuf,2); // write low and high byte of address
   MInfo.nBytesToSend   = 2;
    // write load command
   if ((status=PcdSingleResponseCmd(PCD_LOADKEYE2,MSndBuffer,MRcvBuffer,&MInfo)) == MI_OK)
   {      
      // execute authentication
      status = Mf500PiccAuthState(auth_mode,snr,block);  
   }
   return status;
}                        

///////////////////////////////////////////////////////////////////////
//                      C O D E   K E Y S  
///////////////////////////////////////////////////////////////////////
signed char Mf500HostCodeKey(  unsigned char *uncoded, // 6 bytes key value uncoded
                     unsigned char *coded)   // 12 bytes key value coded
{
   signed char status = MI_OK;
   unsigned char cnt = 0;
   unsigned char ln  = 0;     // low nibble
   unsigned char hn  = 0;     // high nibble
   
   for (cnt = 0; cnt < 6; cnt++)
   {
      ln = uncoded[cnt] & 0x0F;
      hn = uncoded[cnt] >> 4;
      coded[cnt * 2 + 1]     =  (~ln << 4) | ln;
      coded[cnt * 2 ] =  (~hn << 4) | hn;
   }
   return status;
}

///////////////////////////////////////////////////////////////////////
//                  A U T H E N T I C A T I O N   
//             W I T H   P R O V I D E D   K E Y S
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccAuthKey(  unsigned char auth_mode,
                     unsigned char *snr,       
                     unsigned char *keys,      
                     unsigned char block)      
{
   char status     = MI_OK;
   
   PcdSetTmo(106);
   FlushFIFO();    // empty FIFO
   ResetInfo(MInfo);
   memcpy(MSndBuffer,keys,12);                  // write 12 bytes of the key
   MInfo.nBytesToSend = 12;
    // write load command
   if ((status=PcdSingleResponseCmd(PCD_LOADKEY,MSndBuffer,MRcvBuffer,&MInfo)) == MI_OK)
   {      
      // execute authentication
      status = Mf500PiccAuthState(auth_mode,snr,block); 
   }
   return status;
}

///////////////////////////////////////////////////////////////////////
//        S T O R E   K E Y S   I N   E E P R O M
///////////////////////////////////////////////////////////////////////
signed char Mf500PcdLoadKeyE2(unsigned char key_type,
                       unsigned char sector,
                       unsigned char *uncoded_keys)
{
   // eeprom address calculation
   // 0x80 ... offset
   // key_sector ... sector
   // 0x18 ... 2 * 12 = 24 = 0x18
   signed char status = MI_OK;
   unsigned short e2addr = 0x80 + sector * 0x18;
   unsigned char coded_keys[12];

   if (key_type == PICC_AUTHENT1B)
      e2addr += 12; // key B offset   
   if ((status = Mf500HostCodeKey(uncoded_keys,coded_keys)) == MI_OK)
      status = PcdWriteE2(  e2addr,12,coded_keys);
   return status;
}                       
                          
///////////////////////////////////////////////////////////////////////
//        A U T H E N T I C A T I O N   S T A T E S
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccAuthState(   unsigned char auth_mode,
                        unsigned char *snr,
                        unsigned char block)
{
   signed char status = MI_OK;
   
   WriteRC(RegChannelRedundancy,0x07); // RxCRC disable,TxCRC, Parity enable

   PcdSetTmo(150);
   MSndBuffer[0] = auth_mode;        // write authentication command

   MSndBuffer[1] = block;    // write block number for authentication
   memcpy(MSndBuffer + 2,snr,4); // write 4 bytes card serial number 
   ResetInfo(MInfo);
   MInfo.nBytesToSend = 6;
   if ((status = PcdSingleResponseCmd(PCD_AUTHENT1,
                            MSndBuffer,
                            MRcvBuffer,
                            &MInfo)) == MI_OK)
   {
      if (ReadRC(RegSecondaryStatus) & 0x07) // RxLastBits muß leer sein
      {
         status = MI_BITCOUNTERR;
      }
      else
      {
         WriteRC(RegChannelRedundancy,0x03); // RxCRC,TxCRC disable, Parity enable
         ResetInfo(MInfo);
         MInfo.nBytesToSend = 0;
         if ((status = PcdSingleResponseCmd(PCD_AUTHENT2,
                                  MSndBuffer,
                                  MRcvBuffer,
                                  &MInfo)) == MI_OK) 
         {
            if ( ReadRC(RegControl) & 0x08 ) // Crypto1 activated
            {
                status = MI_OK;
            }
            else
            {
                status = MI_AUTHERR;
            }
         }
      }
   }
   return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   INC/DEC   
///////////////////////////////////////////////////////////////////////
signed char Mf500Picc_Inc_Dec(u8 cmd,u8 addr,u8 datalen,u8 *data)
{
	signed char status = MI_OK;
	u8 bitsExpected;

	// ************* Cmd Sequence ********************************** 
	PcdSetTmo(1000);     // long timeout 

	WriteRC(RegChannelRedundancy,0x07); //  TxCRC, Parity enable
	 
	ResetInfo(MInfo);   
	MSndBuffer[0] = cmd;        // Write command code
	MSndBuffer[1] = addr;
	MInfo.nBytesToSend   = 2;
	status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
	                         MSndBuffer,
	                         MRcvBuffer,
	                         &MInfo);

	if (status != MI_NOTAGERR)   // no timeout error
	{
		if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
		{
			// upper nibble should be equal to lower nibble, otherwise 
			// there is a coding error on card side.
			if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
				bitsExpected = 8;
			else
			{
				bitsExpected = 0;
			}
		}
		else
			bitsExpected = 4;  
		if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
		{
			if (bitsExpected == 0)
				status = MI_CODINGERR;
			else
				status = MI_BITCOUNTERR;
		}
		else                     // 4 bit received
		{
			MRcvBuffer[0] &= 0x0f; // mask out upper nibble
			switch(MRcvBuffer[0])
			{
				case 0x00: 
					status = MI_NOTAUTHERR;
					break;
				case 0x0a:
					status = MI_OK;
					break;
				default:
					status = MI_CODEERR;
					break;
			}
		}
	}

   	if ( status == MI_OK)
	{
		ResetInfo(MInfo);   
		memcpy(MSndBuffer,data,datalen);
		MInfo.nBytesToSend   = datalen;
		status = PcdSingleResponseCmd(PCD_TRANSMIT,
					            MSndBuffer,
					            MRcvBuffer,
					            &MInfo);      
	}
	if ( status == MI_OK)
	{
		status = Mf500Picc_Res_Tra(PICC_TRANSFER,addr);
	}
	return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   RESTORE/TRANSFER   
///////////////////////////////////////////////////////////////////////
signed char Mf500Picc_Res_Tra(u8 cmd,u8 addr)
{
	signed char status = MI_OK;
	u8 bitsExpected;
	
	// ************* Cmd Sequence ********************************** 
	PcdSetTmo(1000);     // long timeout 

	WriteRC(RegChannelRedundancy,0x07); //  TxCRC, Parity enable
	 
	ResetInfo(MInfo);   
	MSndBuffer[0] = cmd;        // Write command code
	MSndBuffer[1] = addr;
	MInfo.nBytesToSend   = 2;
	status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
	                         MSndBuffer,
	                         MRcvBuffer,
	                         &MInfo);

	if (status != MI_NOTAGERR)   // no timeout error
	{
		if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
		{
			// upper nibble should be equal to lower nibble, otherwise 
			// there is a coding error on card side.
			if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
				bitsExpected = 8;
			else
			{
				bitsExpected = 0;
			}
		}
		else
			bitsExpected = 4;  
		if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
		{
			if (bitsExpected == 0)
				status = MI_CODINGERR;
			else
				status = MI_BITCOUNTERR;
		}
		else                     // 4 bit received
		{
			MRcvBuffer[0] &= 0x0f; // mask out upper nibble
			switch(MRcvBuffer[0])
			{
				case 0x00: 
					status = MI_NOTAUTHERR;
					break;
				case 0x0a:
					status = MI_OK;
					break;
				default:
					status = MI_CODEERR;
					break;
			}
		}
	}
	return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   R E A D   
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccRead(  unsigned char addr,
                  unsigned char* data)
{
   return Mf500PiccCommonRead(PICC_READ16,addr,16,data);
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   R E A D   
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccCommonRead(  unsigned char cmd,
                           unsigned char addr,
                           unsigned char datalen,
                           unsigned char *data)
{
   signed char status = MI_OK;
   signed char bitsExpected;
   char i;


   FlushFIFO();    // empty FIFO

   PcdSetTmo(640);  // long timeout 

   WriteRC(RegChannelRedundancy,0x0F); // RxCRC, TxCRC, Parity enable
   
   // ************* Cmd Sequence ********************************** 
   ResetInfo(MInfo);   
   MSndBuffer[0] = cmd;   // read command code
   MSndBuffer[1] = addr;
   MInfo.nBytesToSend   = 2;   
   status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                       MSndBuffer,
                       MRcvBuffer,
                       &MInfo);

   if (status != MI_OK)
   {
      if (status != MI_NOTAGERR ) // no timeout occured
      {
         if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
         {
            // upper nibble should be equal to lower nibble, otherwise 
            // there is a coding error on card side.
            if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
               bitsExpected = 8;
            else
            {
               status = MI_CODINGERR;
               bitsExpected = 0; // data is copied to output
            }
         }
         else
            bitsExpected = 4;  
         if (MInfo.nBitsReceived == bitsExpected)  // NACK
         {
             MRcvBuffer[0] &= 0x0f;  // mask out upper nibble
             if ((MRcvBuffer[0] & 0x0a) == 0)
             {
                status = MI_NOTAUTHERR;
             }
             else
             {
                status = MI_CODEERR;
             }
          }
          else
          {
             // return data - even if an error occured - for debugging reasons
             if (MInfo.nBytesReceived >= datalen)
                memcpy(data,MRcvBuffer,datalen);
             else
             {
                memcpy(data,MRcvBuffer,MInfo.nBytesReceived);
                for (i = MInfo.nBytesReceived; i < datalen; i++)
                {
                   data[i] = 0x00;
                }
             }
          }
             
      }
      else
         memcpy(data,"0000000000000000",datalen); // in case of an error initialise 
                                             // data
   }
   else   // Response Processing
   {
      if (MInfo.nBytesReceived != datalen)
      {
         status = MI_BYTECOUNTERR;
         // return data, even if an error occured
         if (MInfo.nBytesReceived >= datalen)
            memcpy(data,MRcvBuffer,datalen);
         else
         {
            memcpy(data,MRcvBuffer,MInfo.nBytesReceived);             
            for (i = MInfo.nBytesReceived; i < datalen; i++)
            {
               data[i] = 0x00;
            }
         }   
      }
      else
      {
         memcpy(data,MRcvBuffer,datalen);
      }
   }
   return status; 
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   W R I T E     
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccWrite( unsigned char addr,
                  unsigned char *data)
{
   return Mf500PiccCommonWrite(PICC_WRITE16,addr,16,data);
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   W R I T E     
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccWrite128( unsigned char addr,
                  unsigned char *data)
{
   return Mf500PiccCommonWrite(PICC_WRITE128,addr,128,data);
}
///////////////////////////////////////////////////////////////////////
//          M I F A R E   W R I T E     
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccWrite4( unsigned char addr,
                      unsigned char *data)
{
   unsigned char bitsExpected;
   signed char status = MI_OK;

     // ************* Cmd Sequence ********************************** 
   PcdSetTmo(1000);     // long timeout 

   WriteRC(RegChannelRedundancy,0x07); //  TxCRC, Parity enable
     
   ResetInfo(MInfo);   
   MSndBuffer[0] = PICC_WRITE4;  // Write command code
   MSndBuffer[1] = addr;
   memcpy(MSndBuffer + 2,data,4);
   MInfo.nBytesToSend   = 6;
   status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                         MSndBuffer,
                         MRcvBuffer,
                         &MInfo);

   if (status != MI_NOTAGERR)   // no timeout error
   {
      if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
      {
         // upper nibble should be equal to lower nibble, otherwise 
         // there is a coding error on card side.
         if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
            bitsExpected = 8;
         else
         {
            bitsExpected = 0;
         }
      }
      else
         bitsExpected = 4;  
      if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
      {
         if (bitsExpected == 0)
            status = MI_CODINGERR;
         else
            status = MI_BITCOUNTERR;
      }
      else                     // 4 bit received
      {
         MRcvBuffer[0] &= 0x0f; // mask out upper nibble
         switch(MRcvBuffer[0])
         {
            case 0x00: 
               status = MI_NOTAUTHERR;
               break;
            case 0x0a:
               status = MI_OK;
               break;
            default:
               status = MI_CODEERR;
               break;
         }
      }
   }
   return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E   W R I T E     
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccCommonWrite( unsigned char cmd,
                           unsigned char addr,
                           unsigned char datalen,
                           unsigned char *data)
{
   unsigned char bitsExpected;
   signed char status = MI_OK;

     // ************* Cmd Sequence ********************************** 
   PcdSetTmo(1000);     // long timeout 

   WriteRC(RegChannelRedundancy,0x07); //  TxCRC, Parity enable
     
   ResetInfo(MInfo);   
   MSndBuffer[0] = cmd;        // Write command code
   MSndBuffer[1] = addr;
   MInfo.nBytesToSend   = 2;
   status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                         MSndBuffer,
                         MRcvBuffer,
                         &MInfo);

   if (status != MI_NOTAGERR)   // no timeout error
   {
      if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
      {
         // upper nibble should be equal to lower nibble, otherwise 
         // there is a coding error on card side.
         if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
            bitsExpected = 8;
         else
         {
            bitsExpected = 0;
         }
      }
      else
         bitsExpected = 4;  
      if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
      {
         if (bitsExpected == 0)
            status = MI_CODINGERR;
         else
            status = MI_BITCOUNTERR;
      }
      else                     // 4 bit received
      {
         MRcvBuffer[0] &= 0x0f; // mask out upper nibble
         switch(MRcvBuffer[0])
         {
            case 0x00: 
               status = MI_NOTAUTHERR;
               break;
            case 0x0a:
               status = MI_OK;
               break;
            default:
               status = MI_CODEERR;
               break;
         }
      }
   }

   if ( status == MI_OK)
   {
      ResetInfo(MInfo);   
      memcpy(MSndBuffer,data,datalen);
      MInfo.nBytesToSend   = datalen;
      status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                            MSndBuffer,
                            MRcvBuffer,
                            &MInfo);
        
      if (status != MI_NOTAGERR)    // no timeout occured
      {
         if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
         {
            // upper nibble should be equal to lower nibble, otherwise 
            // there is a coding error on card side.
            if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
               bitsExpected = 8;
            else
            {
               bitsExpected = 0;
            }
         }
         else
            bitsExpected = 4;  
         if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
         {
            if (bitsExpected == 0)
               status = MI_CODINGERR;
            else
               status = MI_BITCOUNTERR;
         }
         else                     // 4 bit received
         {
            MRcvBuffer[0] &= 0x0f; // mask out upper nibble
            switch(MRcvBuffer[0])
            {
               case 0x00: 
                  status = MI_WRITEERR;
                  break;
               case 0x0a:
                  status = MI_OK;
                  break;
               default:
                  status = MI_CODEERR;
                  break;
            }
         }
      }        
   }
   return status;
}


///////////////////////////////////////////////////////////////////////
//                V A L U E   M A N I P U L A T I O N 
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccValue(unsigned char dd_mode, 
                   unsigned char addr, 
                   unsigned char *value,
                   unsigned char trans_addr)
{
   unsigned char bitsExpected;
   signed char status = MI_OK;

   PcdSetTmo(106);
   
   WriteRC(RegChannelRedundancy,0x07); // TxCRC, Parity enable
   
   // ************* Cmd Sequence ********************************** 
   ResetInfo(MInfo);   
   MSndBuffer[0] = dd_mode;        // Inc,Dec command code
   MSndBuffer[1] = addr;
   MInfo.nBytesToSend   = 2;
   status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                       MSndBuffer,
                       MRcvBuffer,
                       &MInfo);

   if (status != MI_NOTAGERR)   // no timeout error
   {
        if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
        {
           // upper nibble should be equal to lower nibble, otherwise 
           // there is a coding error on card side.
           if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
              bitsExpected = 8;
           else
           {
              bitsExpected = 0;
           }
        }
        else
           bitsExpected = 4;  
        if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
        {
           if (bitsExpected == 0)
              status = MI_CODINGERR;
           else
              status = MI_BITCOUNTERR;
        }
        else                     // 4 bit received
        {
           MRcvBuffer[0] &= 0x0f; // mask out upper nibble
           switch(MRcvBuffer[0])
           {
              case 0x00: 
                 status = MI_NOTAUTHERR;
                 break;
              case 0x0a:
                 status = MI_OK;
                 break;
              case 0x01:
                 status = MI_VALERR;
                 break;
              default:
                 status = MI_CODEERR;
                 break;
           }
        }
     }

     if ( status == MI_OK)
     {
        PcdSetTmo(1000);     // long timeout 

        ResetInfo(MInfo);   
        memcpy(MSndBuffer,value,4);
        MInfo.nBytesToSend   = 4;
        status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                            MSndBuffer,
                            MRcvBuffer,
                            &MInfo);
        
        if (status == MI_OK)    // no timeout occured
        {
            if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
            {
               // upper nibble should be equal to lower nibble, otherwise 
               // there is a coding error on card side.
               if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
                  bitsExpected = 8;
               else
               {
                  bitsExpected = 0;
               }
            }
            else
               bitsExpected = 4;  
            if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
            {
               if (bitsExpected == 0)
                  status = MI_CODINGERR;
               else
                  status = MI_BITCOUNTERR;
            }
            else                     // 4 bit received
            {
               MRcvBuffer[0] &= 0x0f; // mask out upper nibble
               switch(MRcvBuffer[0])
               {
                  case 0x00: 
                     status = MI_NOTAUTHERR;
                     break;
                  case 0x01:
                     status = MI_VALERR;
                     break;
                  default:
                     status = MI_CODEERR;
                     break;
               }
            }
         }        
		else if (status == MI_NOTAGERR )
            status = MI_OK;  // no response after 4 byte value - 
                             // transfer command has to follow
     }
     if ( status == MI_OK)
     {
        ResetInfo(MInfo);   
        MSndBuffer[0] = PICC_TRANSFER;        // transfer command code
        MSndBuffer[1] = trans_addr;
        MInfo.nBytesToSend   = 2;
        status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                            MSndBuffer,
                            MRcvBuffer,
                            &MInfo);
        
        if (status != MI_NOTAGERR)    // timeout occured
        {
            if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
            {
               // upper nibble should be equal to lower nibble, otherwise 
               // there is a coding error on card side.
               if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
                  bitsExpected = 8;
               else
               {
                  bitsExpected = 0;
               }
            }
            else
               bitsExpected = 4;  
            if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
            {
               if (bitsExpected == 0)
                  status = MI_CODINGERR;
               else
                  status = MI_BITCOUNTERR;
            }
            else                     // 4 bit received
            {
               MRcvBuffer[0] &= 0x0f; // mask out upper nibble
               switch(MRcvBuffer[0])
               {
                  case 0x00: 
                     status = MI_NOTAUTHERR;
                     break;
                  case 0x0a:
                     status = MI_OK;
                     break;
                  case 0x01:
                     status = MI_VALERR;
                     break;
                  default:
                     status = MI_CODEERR;
                     break;
               }
            }
        }        
     }
   return status;
}

///////////////////////////////////////////////////////////////////////
//   V A L U E   M A N I P U L A T I O N   W I T H   B A C K U P
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccValueDebit(unsigned char dd_mode, 
                         unsigned char addr, 
                         unsigned char *value)
{
   unsigned char bitsExpected;
   signed char status = MI_OK;
   
   PcdSetTmo(106);
   
   WriteRC(RegChannelRedundancy,0x07); // RxCRC,TxCRC, Parity enable   
   
   ResetInfo(MInfo);   
   MSndBuffer[0] = dd_mode;        // Inc,Dec command code
   MSndBuffer[1] = addr;
   MInfo.nBytesToSend   = 2;
   status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                       MSndBuffer,
                       MRcvBuffer,
                       &MInfo);

   if (status != MI_NOTAGERR)   // no timeout error
   {
        if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
        {
           // upper nibble should be equal to lower nibble, otherwise 
           // there is a coding error on card side.
           if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
              bitsExpected = 8;
           else
           {
              bitsExpected = 0;
           }
        }
        else
           bitsExpected = 4;  
        if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
        {
           if (bitsExpected == 0)
              status = MI_CODINGERR;
           else
              status = MI_BITCOUNTERR;
        }
        else                     // 4 bit received
        {
           MRcvBuffer[0] &= 0x0f; // mask out upper nibble
           switch(MRcvBuffer[0])
           {
              case 0x00: 
                 status = MI_NOTAUTHERR;
                 break;
              case 0x0a:
                 status = MI_OK;
                 break;
              case 0x01:
                 status = MI_VALERR;
                 break;
              default:
                 status = MI_CODEERR;
                 break;
           }
        }
     }

     if ( status == MI_OK)
     {
        PcdSetTmo(1000);     // long timeout 

        ResetInfo(MInfo);   
        memcpy(MSndBuffer,value,4);
        MInfo.nBytesToSend   = 4;
        status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                            MSndBuffer,
                            MRcvBuffer,
                            &MInfo);
        
        if (status == MI_OK)    // no timeout occured
        {
            if (MInfo.nBitsReceived == 8 && (ReadRC(RegDecoderControl) & 0x01))
            {
               // upper nibble should be equal to lower nibble, otherwise 
               // there is a coding error on card side.
               if ((MRcvBuffer[0] & 0x0f) == ((MRcvBuffer[0] >> 4) & 0x0f))
                  bitsExpected = 8;
               else
               {
                  bitsExpected = 0;
               }
            }
            else
               bitsExpected = 4;  
            if (MInfo.nBitsReceived != bitsExpected)  // ACK / NACK expected
            {
               if (bitsExpected == 0)
                  status = MI_CODINGERR;
               else
                  status = MI_BITCOUNTERR;
            }
            else                     // 4 bit received
            {
               MRcvBuffer[0] &= 0x0f; // mask out upper nibble
               switch(MRcvBuffer[0])
               {
                  case 0x00: 
                     status = MI_NOTAUTHERR;
                     break;
                  case 0x0a:
                     status = MI_OK;
                     break;
                  case 0x05:
                  case 0x01:
                     status = MI_VALERR;
                     break;
                  default:
                     status = MI_CODEERR;
                     break;
               }
            }
        }        
     }

   return status;
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E     H A L T
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccHalt(void)
{
   signed char status = MI_CODEERR;

   PcdSetTmo(106);
   
   WriteRC(RegChannelRedundancy,0x07); // RxCRC,TxCRC, Parity enable
   
   // ************* Cmd Sequence ********************************** 
   ResetInfo(MInfo);   
   MSndBuffer[0] = PICC_HALT ;     // Halt command code
   MSndBuffer[1] = 0x00;         // dummy address
   MInfo.nBytesToSend   = 2;
   status = PcdSingleResponseCmd(PCD_TRANSCEIVE,
                       MSndBuffer,
                       MRcvBuffer,
                       &MInfo);   
   if (status)
   {
     // timeout error ==> no NAK received ==> OK
     if (status == MI_NOTAGERR || status == MI_ACCESSTIMEOUT)
        status = MI_OK;
   }
    //reset command register - no response from tag
   WriteRC(RegCommand,PCD_IDLE);
   return status; 
}

///////////////////////////////////////////////////////////////////////
//          M I F A R E     C L B
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccReqB(unsigned char *send_data,
                        unsigned short send_bytelen,
                        unsigned char *rec_data,  
                        unsigned short *rec_bytelen)
{
   signed char status = MI_OK;
   unsigned char append_crc=1;
   PcdSetTmo(3300);
   

      if (append_crc)
   {
      // RxCRC and TxCRC enable, parity enable
      WriteRC(RegChannelRedundancy,0x2C); 		 // TypeB crcËã·¨ÇÐ»»crc-B
      send_bytelen -= 2;
   }
   else
   {
      // RxCRC and TxCRC disable, parity enable
      WriteRC(RegChannelRedundancy,0x03); 
   }
  
   status = ExchangeByteStream(PCD_TRANSCEIVE,
                               send_data,
                               send_bytelen,
                               rec_data,
                               rec_bytelen);
   // even if an error occured, the data should be
   // returned - for debugging reasons

   if (append_crc)
   {
      *rec_bytelen += 2; // for two CRC bytes
      rec_data[*rec_bytelen - 2] = 0x00;
      rec_data[*rec_bytelen - 1] = 0x00;
   }

	if (status)
   {
     // timeout error ==> no NAK received ==> OK
 //    if (status == MI_NOTAGERR || status == MI_ACCESSTIMEOUT)
       if (status == MI_NOTAGERR) 
	   status = MI_OK;
	
   }
	return status; 
}

//////////////////////////////////////////////////////////////////////
//        S E T   D E F A U L T    C O M M   A T T R I B S
///////////////////////////////////////////////////////////////////////
signed char Mf500PcdSetDefaultAttrib(void)
{
   char   status = MI_OK;

   // switch to 106 kBaud (default)
   // if last reader receive baud rate is different to the default value
   // or last reader send baud rate is different to the default value
   if ((MDSI != TCLDSDFLT) || (MDRI != TCLDRDFLT))
   {
      status = Mf500PcdSetAttrib(TCLDSDFLT,TCLDRDFLT);
   }
   return status;
}

//////////////////////////////////////////////////////////////////////
//        G E T   C O M M    A B I L I T I E S 
///////////////////////////////////////////////////////////////////////
signed char Mf500PcdGetAttrib(unsigned char *FSCImax,
                      unsigned char *FSDImax,
                      unsigned char *DSsupp,
                      unsigned char *DRsupp,
                      unsigned char *DREQDS)
{
    *FSCImax = TCLFSDSNDMAX; // max. frame size send
    *FSDImax = TCLFSDRECMAX; // max. frame size rcv
    *DSsupp  = TCLDSMAX;     // max. baudrate PICC --> PCD
    *DRsupp  = TCLDRMAX;     // max. baudrate PCD --> PICC
    *DREQDS  = 0x00; // different send and receive baudrates are
                     // possible
    return(MI_OK);
}

//////////////////////////////////////////////////////////////////////
//        S E T   C O M M    P A R A M E T E R S 
///////////////////////////////////////////////////////////////////////
signed char Mf500PcdSetAttrib(unsigned char DSI,
                      unsigned char DRI)
{
   signed char   status = MI_OK;
  
  // Return error, if adjusted baudrate not supported by PCD
  if ( ( DSI > TCLDSMAX ) || ( DRI > TCLDRMAX ) )
  {
    status = MI_BAUDRATE_NOT_SUPPORTED ;
  }
  else
  {
    if(PRTCL == 1)               // cnn
	{
    MDSI = DSI+4;
    MDRI = DRI+4;  
	}
	else
	{
	MDSI = DSI;
    MDRI = DRI;
	}
    Mf500PcdWriteAttrib();
  }
  return status;
}

//////////////////////////////////////////////////////////////////////
//        W R I T E   C O M M    P A R A M E T E R S 
///////////////////////////////////////////////////////////////////////
signed char Mf500PcdWriteAttrib(void)
{
    char   status = MI_OK;

    // adjust baudrate and pauselength of reader
    WriteRC(RegBPSKDemControl,0x0e);   // RegBPSKDemControl  

    // set reader send baudrate
    WriteRC(RegCoderControl,MDRCfg[MDRI].CoderRate);
    WriteRC(RegModWidth,MDRCfg[MDRI].ModWidth);
    
    // set reader receive baudrate
    WriteRC(RegRxControl1,MDSCfg[MDSI].SubCarrierPulses);
    WriteRC(RegDecoderControl,MDSCfg[MDSI].RxCoding);      
    WriteRC(RegRxThreshold,MDSCfg[MDSI].RxThreshold);
    WriteRC(RegBPSKDemControl,MDSCfg[MDSI].BPSKDemControl);
    return(status);
}
 
//////////////////////////////////////////////////////////////////////
//           P I C C   E X C H A N G E   B L O C K
///////////////////////////////////////////////////////////////////////
signed char Mf500PiccExchangeBlock(unsigned char *send_data,
                           unsigned short send_bytelen,
                           unsigned char *rec_data,  
                           unsigned short *rec_bytelen,
                           unsigned char parity_crc, 
                           unsigned long timeout )
{
   char   status = MI_OK;
//	append_crc : 0 :RxCRC=0,TXCRC=0
//	append_crc : 1 :RxCRC=1,TXCRC=1
//	append_crc : 2 :RxCRC=1,TXCRC=0
//	append_crc : 3 :RxCRC=0,TXCRC=1
	unsigned char append_crc, parity_cfg;
	append_crc = parity_crc & 0x0F;
	parity_cfg = parity_crc & 0xF0;
	
	if (append_crc == 1) //	append_crc : 1 :RxCRC,TXCRC 
	{
		// RxCRC and TxCRC enable, parity enable
		if(PRTCL != 1)  					 // 2012 5 15  cnn ÐÞ¸ÄTypeBÊ±CRC-B²»±»ÐÞ¸Ä¡£
		{
//			WriteRC(RegChannelRedundancy,0x0F); 
			// 2014.1.10	Hong,ÓÃÓÚ²âÊÔCLA parity bit,
			// ·ÇAPDU modeÏÂ²âÊÔ
			if((bSendBlockType == 0x10) && !(*send_data & 0xe0))		// I Block
			{
				if(*(send_data+2) == 0x03)	
					WriteRC(RegChannelRedundancy,0x0c); 
				else if(*(send_data+2) == 0x02)
					WriteRC(RegChannelRedundancy,0x0d); 
				else if(*(send_data+2) == 0x01)
					WriteRC(RegChannelRedundancy,0x0e); 
				else
					WriteRC(RegChannelRedundancy,0x0F); 
				//*(send_data+2) = 0x00;		//20150714 CJN should not force the CLA to 0x00
			}
			else		//add parity control   by CJN20150807
			{
				if(parity_cfg == 0)	//ODD parity_default set up
					WriteRC(RegChannelRedundancy,0x0F);
				else if(parity_cfg == 0x10)	//even parity
					WriteRC(RegChannelRedundancy,0x0D);
				else if(parity_cfg == 0x20)		//no parity
					WriteRC(RegChannelRedundancy,0x0C);
				else		//invalid value
					WriteRC(RegChannelRedundancy,0x0F);
			}
		}
		else
		{			// 2013.12.12	Hong
			WriteRC(RegChannelRedundancy,0x2C); 
		//      send_bytelen -= 2; //2013-04-26
		}
	}
	else if(append_crc == 2) //	append_crc : 2 :RxCRC
	{
		if(PRTCL != 1)  					 // 2012 5 15  cnn ÐÞ¸ÄTypeBÊ±CRC-B²»±»ÐÞ¸Ä¡
		{		//add parity control   by CJN20150807
			if(parity_cfg == 0)	//ODD parity_default set up
				WriteRC(RegChannelRedundancy,0x0B);
			else if(parity_cfg == 0x10)	//even parity
				WriteRC(RegChannelRedundancy,0x09);
			else if(parity_cfg == 0x20)		//no parity
				WriteRC(RegChannelRedundancy,0x08);
			else		//invalid value
				WriteRC(RegChannelRedundancy,0x0B);
		}
		else					// 2013.12.12	Hong
			WriteRC(RegChannelRedundancy,0x28); 
	//      send_bytelen -= 2; //2013-04-26
	}
	else if(append_crc == 3) //	append_crc : 3 :TxCRC
	{
		if(PRTCL != 1)  					 // 2012 5 15  cnn ÐÞ¸ÄTypeBÊ±CRC-B²»±»ÐÞ¸Ä¡
		{		//add parity control   by CJN20150807
			if(parity_cfg == 0)	//ODD parity_default set up
				WriteRC(RegChannelRedundancy,0x07);
			else if(parity_cfg == 0x10)	//even parity
				WriteRC(RegChannelRedundancy,0x05);
			else if(parity_cfg == 0x20)		//no parity
				WriteRC(RegChannelRedundancy,0x04);
			else		//invalid value
				WriteRC(RegChannelRedundancy,0x07);
		}
		else					// 2013.12.12	Hong
			WriteRC(RegChannelRedundancy,0x24); 
		//      send_bytelen -= 2;  //2013-04-26
	}
	else //	append_crc : 4 :No CRC
	{
	  // RxCRC and TxCRC disable, parity enable
		if(PRTCL != 1)
		{		//add parity control   by CJN20150807
			if(parity_cfg == 0)	//ODD parity_default set up
				WriteRC(RegChannelRedundancy,0x03);
			else if(parity_cfg == 0x10)	//even parity
				WriteRC(RegChannelRedundancy,0x01);
			else if(parity_cfg == 0x20)		//no parity
				WriteRC(RegChannelRedundancy,0x00);
			else		//invalid value
				WriteRC(RegChannelRedundancy,0x03);
		}
		else					// 2013.12.12	Hong
			WriteRC(RegChannelRedundancy,0x20); 
	}

	if(stuInRegs.bTmoStatus != TMO_NO_LIMIT)    // Standard timeout setting.	
	 	PcdSetTmo(timeout);

   status = ExchangeByteStream(PCD_TRANSCEIVE,
                               send_data,
                               send_bytelen,
                               rec_data,
                               rec_bytelen);
   // even if an error occured, the data should be
   // returned - for debugging reasons
  /*
   if ((append_crc==1)||(append_crc==2))
   {
      *rec_bytelen += 2; // for two CRC bytes
      rec_data[*rec_bytelen - 2] = 0x00;
      rec_data[*rec_bytelen - 1] = 0x00;
   }
   */
   return status;
}                      

///////////////////////////////////////////////////////////////////////
//         S e t   T i m e o u t   L E N G T H
///////////////////////////////////////////////////////////////////////
signed char PcdSetTmo(unsigned long tmoLength)
{
    unsigned char prescale = 12;  // 4096 division. 1 unit is 302us.
    unsigned long reload = tmoLength;

	while (reload > 255)
	{
		prescale++;
		reload = reload >> 1; // division by 2
	}
	// if one of the higher bits are set, the prescaler is set
	// to the largest value
	if (prescale > 0x15)
	{
		prescale = 0x15;
		reload   = 0xFF;
	}
/*	if (prescale > 0x18)		// 2014.2.13	Hong,for FWT max test
	{
		prescale = 0x18;
		reload   = 0xFF;
	}	*/

    // fcnt = 13560000 / (2^prescale)
    // T = (reload - counter) / fcnt
    WriteRC(RegTimerClock,prescale); // TAutoRestart=0,TPrescale=prescale
    WriteRC(RegTimerReload,reload);// TReloadVal = reload
    
    return MI_OK;
}

///////////////////////////////////////////////////////////////////////
//      M I F A R E   M O D U L E   R E S E T 
///////////////////////////////////////////////////////////////////////
signed char PcdReset(void)
{
   signed char status = MI_OK;

   SleepMs(100);        // wait after POR    
   READER_RST_SET();    // reset reader IC
   SleepMs(100);        // wait
   READER_RST_RESET();  // clear reset pin
   SleepUs(100);
   

//   WriteRawRC(RegPage, 0);
   // wait until reset command recognized
   while (((ReadRawRC(RegCommand) & 0x3F) != 0x3F));

   // while reset sequence in progress
   while ((ReadRawRC(RegCommand) & 0x3F));

   WriteRawRC(RegPage,0x80); // Dummy access in order to determine the bus 
                             // configuration
   // necessary read access 
   // after first write access, the returned value
   // should be zero ==> interface recognized
   if (ReadRawRC(RegCommand) != 0x00)
   {                           
       status = MI_INTERFACEERR;
   }
   
   return status;
}

///////////////////////////////////////////////////////////////////////
//           E X C H A N G E   B Y T E   S T R E A M
///////////////////////////////////////////////////////////////////////
signed char ExchangeByteStream(unsigned char Cmd,
                        unsigned char *send_data,
                        unsigned short send_bytelen,
                        unsigned char *rec_data,  
                        unsigned short *rec_bytelen)
{
   signed char status = MI_OK;

   FlushFIFO();    // empty FIFO
   ResetInfo(MInfo); // initialise ISR Info structure

   if (send_bytelen > 0)
   {
      memcpy(MSndBuffer,send_data,send_bytelen); // write n bytes 
      MInfo.nBytesToSend = send_bytelen;
      // write load command
      status = PcdSingleResponseCmd(Cmd,
                      MSndBuffer,
                      MRcvBuffer,
                      &MInfo);
      *rec_bytelen = MInfo.nBytesReceived;
      // copy data to output, even in case of an error
      if (*rec_bytelen)
      {
         memcpy(rec_data,MRcvBuffer,*rec_bytelen);
      }
   }
   else
   {
      status = MI_WRONG_PARAMETER_VALUE;
   }
   return status;
}                                   

///////////////////////////////////////////////////////////////////////
//          R E A D   S N R   O F   R E A D E R   I C
///////////////////////////////////////////////////////////////////////
signed char PcdGetSnr(unsigned char* snr)
{
   signed char status;
   
   status = PcdReadE2(0x08,0x04,snr);
   return status;
}


///////////////////////////////////////////////////////////////////////
//          E E P R O M   R E A D   
///////////////////////////////////////////////////////////////////////
signed char PcdReadE2(unsigned short startaddr,
               unsigned char length,
               unsigned char* data)
{
   char status = MI_OK;

   PcdSetTmo(3400);
   // ************* Cmd Sequence ********************************** 
   ResetInfo(MInfo);   
   MSndBuffer[0] = startaddr & 0xFF;
   MSndBuffer[1] = (startaddr >> 8) & 0xFF;
   MSndBuffer[2] = length;
   MInfo.nBytesToSend   = 3;
   status = PcdSingleResponseCmd(PCD_READE2,
                         MSndBuffer,
                         MRcvBuffer,
                         &MInfo);
   // return data, even in case of an error - for debugging reasons                         
   if (MInfo.nBytesReceived >= length)
      memcpy(data,MRcvBuffer,length);     
   else
      memcpy(data,MRcvBuffer,MInfo.nBytesReceived);
   return status ;
}


///////////////////////////////////////////////////////////////////////
//          E E P R O M   W R I T E 
///////////////////////////////////////////////////////////////////////
signed char PcdWriteE2(  unsigned short startaddr,
                      unsigned char length,
                      unsigned char* data)
{
   char status = MI_OK;

   PcdSetTmo(6800);
     // ************* Cmd Sequence ********************************** 
   ResetInfo(MInfo);   
   MSndBuffer[0] = startaddr & 0xFF;
   MSndBuffer[1] = (startaddr >> 8) & 0xFF;
   memcpy(MSndBuffer + 2,data,length);

   MInfo.nBytesToSend   = length + 2;
         
   status = PcdSingleResponseCmd(PCD_WRITEE2,
                   MSndBuffer,
                   MRcvBuffer,
                   &MInfo); // write e2
   return status;
}   

//////////////////////////////////////////////////////////////////////
//                 R E S E T 
///////////////////////////////////////////////////////////////////////
signed char PcdRfReset(unsigned short ms)
{
   char   status = MI_OK;

   ClearBitMask(RegTxControl,0x03);  // Tx2RF-En, Tx1RF-En disablen
   if (ms > 0)
   {
      SleepMs(ms);                      // Delay for 1 ms 
      SetBitMask(RegTxControl,0x03);    // Tx2RF-En, Tx1RF-En enable
   }
   return status;
}



//////////////////////////////////////////////////////////////////////
//   S E T   A   B I T   M A S K 
///////////////////////////////////////////////////////////////////////
void SetBitMask(unsigned char reg,unsigned char mask) // 
{
   char   tmp    = 0x0;

   tmp = ReadRC(reg);
   WriteRC(reg,tmp | mask);  // set bit mask
}

//////////////////////////////////////////////////////////////////////
//   C L E A R   A   B I T   M A S K 
///////////////////////////////////////////////////////////////////////
void ClearBitMask(unsigned char reg,unsigned char mask) // 
{
   char   tmp    = 0x0;

   tmp = ReadRC(reg);
   WriteRC(reg,tmp & ~mask);  // clear bit mask
}

///////////////////////////////////////////////////////////////////////
//                  F L U S H    F I F O
///////////////////////////////////////////////////////////////////////
void FlushFIFO(void)
{  
   SetBitMask(RegControl,0x01);
}

///////////////////////////////////////////////////////////////////////////////
//                       Interrupt Handler RIC
///////////////////////////////////////////////////////////////////////////////
//interrupt (READER_INT)
void SingleResponseIsr(void)
{
   static unsigned char  irqBits;
   static unsigned char  irqMask;            
   static unsigned char  oldPageSelect;
   static unsigned char  nbytes;
   static unsigned char  cnt;

   if (MpIsrInfo && MpIsrOut && MpIsrIn)  // transfer pointers have to be set
                                          // correctly
   {
      oldPageSelect = ReadRawRC(RegPage); // save old page select 
                                          // Attention: ReadRC cannnot be
                                          // used because of the internal
                                          // write sequence to the page 
                                          // reg
      MpIsrInfo->errFlags = ReadRC(RegErrorFlag) & 0x0F; // save error state
      WriteRawRC(RegPage,0x80);           // select page 0 for ISR
      while( (ReadRawRC(RegPrimaryStatus) & 0x08)) // loop while IRQ pending
      {
         irqMask = ReadRawRC(RegInterruptEn); // read enabled interrupts
         // read pending interrupts
         irqBits = ReadRawRC(RegInterruptRq) & irqMask;

//         abCLIntSrc[bCLIntCnt++] = irqBits;

         MpIsrInfo->irqSource |= irqBits; // save pending interrupts

         //************ LoAlertIRQ ******************
         if (irqBits & 0x01)    // LoAlert
         {  
            nbytes = DEF_FIFO_LENGTH - ReadRawRC(RegFIFOLength);
            // less bytes to send, than space in FIFO
            if ((MpIsrInfo->nBytesToSend - MpIsrInfo->nBytesSent) <= nbytes)
            {
             nbytes = MpIsrInfo->nBytesToSend - MpIsrInfo->nBytesSent;
             WriteRawRC(RegInterruptEn,0x01); // disable LoAlert IRQ
            }
            // write remaining data to the FIFO
            for ( cnt = 0;cnt < nbytes;cnt++)
            {
               WriteRawRC(RegFIFOData,MpIsrOut[MpIsrInfo->nBytesSent]);
               MpIsrInfo->nBytesSent++;
            }
            WriteRawRC(RegInterruptRq,0x01);  // reset IRQ bit
            WriteRawRC(RegInterruptEn,0x02);  // disable HiAlert IRQ
                                              // I cannot find where enable the HiAlert IRQ.
         }
      
         //************* TxIRQ Handling **************
         if (irqBits & 0x10)       // TxIRQ
         {
            WriteRawRC(RegInterruptRq,0x10);    // reset IRQ bit 
            WriteRawRC(RegInterruptEn,0x82);    // enable HiAlert Irq for
                                                // response
            if (MpIsrInfo->RxAlignWA) // if cmd is anticollision and 7 bits are known
            {                                    // switch off parity generation
               WriteRC(RegChannelRedundancy,0x02); // RxCRC and TxCRC disable, parity disable               
               WriteRawRC(RegPage,0x00);  // reset page address
            }
//            ReadAllRegs();
         }
         //**************** RxIRQ Handling *******************************
         if (irqBits & 0x08) // RxIRQ - possible End of response processing
         {
             // no error or collision during
            if (MpIsrInfo->DisableDF || (MpIsrInfo->errFlags == 0x00))
            {
               WriteRawRC(RegCommand,0x00); // cancel current command
               irqBits |= 0x04; // set idle flag in order to signal the end of
                                // processing. For single reponse processing, this
                                // flag is already set.
            }
            else // error occured - flush data and continue receiving
            {
               MpIsrInfo->saveErrorState = MpIsrInfo->errFlags; // save error state
               MpIsrInfo->errFlags = 0; // reset error flags for next receiption
               WriteRC(RegControl,0x01);
               WriteRawRC(RegPage,0x00);
               MpIsrInfo->nBytesReceived = 0x00;
               irqBits &= ~0x08; // clear interrupt request
               WriteRawRC(RegInterruptRq,0x08);
            }
         }

         //************* HiAlertIRQ or RxIRQ Handling ******************
         if (irqBits & 0x0E) // HiAlert, Idle or valid RxIRQ
         {
            // read some bytes ( length of FIFO queue)              
            // into the receive buffer
            nbytes = ReadRawRC(RegFIFOLength);
            // read date from the FIFO and store them in the receive buffer
            do
            {
                for ( cnt = 0; cnt < nbytes; cnt++)               
                {
                   // accept no more data, than reserved memory space
                   if (MpIsrInfo->nBytesReceived < MEMORY_BUFFER_SIZE) 
                   {
                      MpIsrIn[MpIsrInfo->nBytesReceived] = ReadRawRC(RegFIFOData);
                      MpIsrInfo->nBytesReceived++;
                   }
                   else
                   {
                      MpIsrInfo->status = MI_RECBUF_OVERFLOW;
                      break;
                   }
                }
                // check for remaining bytes
                nbytes = ReadRawRC(RegFIFOLength);
            }
            while (MpIsrInfo->status == MI_OK && nbytes > 0);
            WriteRawRC(RegInterruptRq,0x0A & irqBits);  
                                       // reset IRQ bit - idle irq will
                                       // be deleted in a seperate section
         }   

         //************* additional HiAlertIRQ Handling ***
         if (irqBits & 0x02)
         {
            // if highAlertIRQ is pending and the receiver is still
            // running, then the timeout counter should be stopped,
            // otherwise a timeout could occure while receiving 
            // correct data
            if ((ReadRawRC(RegPrimaryStatus) & 0x70) == 0x70)
            {      
               WriteRawRC(RegPage,0x81); // switch Page register
               cnt = ReadRawRC(RegControl); // read control register
               WriteRawRC(RegControl,cnt|0x04); // stop reader IC timer
                                                // write modified register
               WriteRawRC(RegPage,0x00); // reset Page Register
            }
         }

         //************** additional IdleIRQ Handling ******
         if (irqBits & 0x04)     // Idle IRQ
         {
            WriteRawRC(RegInterruptEn,0x20); // disable Timer IRQ
            WriteRawRC(RegInterruptRq,0x24); // disable Timer IRQ request
            irqBits &= ~0x20;   // clear Timer IRQ in local var
            MpIsrInfo->irqSource &= ~0x20; // clear Timer IRQ in info var
                                        // when idle received, then cancel
                                        // timeout
            MpIsrInfo->irqSource |= 0x04; // set idle-flag in case of valid rx-irq
            // status should still be MI_OK
            // no error - only used for wake up
         }
       
         //************* TimerIRQ Handling ***********
         if (irqBits & 0x20)       // timer IRQ
         {
            WriteRawRC(RegInterruptRq,0x20); // reset IRQ bit 
            // only if no other error occured
            if (MpIsrInfo->status == MI_OK)
            {
               MpIsrInfo->status = MI_NOTAGERR; // timeout error
                                                // otherwise ignore the interrupt
            }
         }
         
      }
      WriteRawRC(RegPage,oldPageSelect | 0x80);
   }
}

unsigned char waterLevelBackup;
unsigned char validErrorFlags = 0x1F; 
//////////////////////////////////////////////////////////////////////
//       W R I T E   A   P C D   C O M M A N D 
///////////////////////////////////////////////////////////////////////
signed char  PcdSingleResponseCmd(unsigned char cmd,
               volatile unsigned char* send, 
               volatile unsigned char* rcv,
               volatile MfCmdInfo *info)
{     
   signed char   status    = MI_OK;
   unsigned char lastBits;
//   unsigned char rxMultiple = 0x00;

   unsigned char irqEn     = 0x00;
   unsigned char waitFor   = 0x00;
   unsigned char timerCtl  = 0x06;

   unsigned char  b;   

   WriteRC(RegInterruptEn,0x7F); // disable all interrupts
   WriteRC(RegInterruptRq,0x7F); // reset interrupt requests

   // please pay attention to the sequence of following commands
   // at first empty the FIFO
   // second wait for probably e2 programming in progress
   // third cancel command
   //
   // ATTENTION: the guard timer must not expire earlier than 10 ms
   FlushFIFO();            // flush FIFO buffer

   // wait until e2 programming is finished
   while (((ReadRC(RegSecondaryStatus) & 0x40) == 0));

   WriteRC(RegCommand,PCD_IDLE); // terminate probably running command

   // Set water level to the default value (see. 'PcdBasicRegisterConfiguration()')   
   waterLevelBackup = ReadRC(RegFIFOLevel);
   WriteRC(RegFIFOLevel, 0x18);
   
   // save info structures to module pointers
   MpIsrInfo = info;  
   MpIsrOut  = send;
   MpIsrIn   = rcv;

   info->irqSource = 0x0; // reset interrupt flags

   READER_INT_ENABLE();

   // depending on the command code, appropriate interrupts are enabled (irqEn)
   // and the commit interrupt is choosen (waitFor).
   switch(cmd)
   {
      case PCD_IDLE:                   // nothing else required
         irqEn = 0x00;
         waitFor = 0x00;
         break;
      case PCD_WRITEE2:                // LoAlert and TxIRq
         timerCtl = 0x00;              // start and stop timer manually            
         irqEn = 0x11;
         waitFor = 0x10;
		 validErrorFlags &= ~0x08;     // hb 20.06.2001 don't check CRC errors         
         break;
      case PCD_READE2:                 // HiAlert, LoAlert and IdleIRq
         timerCtl = 0x00;              // start and stop timer manually            
         irqEn = 0x07;
         waitFor = 0x04;
		 validErrorFlags &= ~0x08;     // wu 15.05.2001 don't check CRC errors
         break;
      case PCD_LOADKEYE2:              // IdleIRq and LoAlert
         timerCtl = 0x00;              // start and stop timer manually            
		 validErrorFlags &= ~0x08;     // hb 20.06.2001 don't check CRC errors               
         irqEn = 0x05;
         waitFor = 0x04;
         break;
      case PCD_LOADCONFIG:             // IdleIRq and LoAlert
         timerCtl = 0x00;              // start and stop timer manually            
         irqEn = 0x05;
         waitFor = 0x04;
         break;
      case PCD_AUTHENT1:               // IdleIRq and LoAlert
         irqEn = 0x05;
         waitFor = 0x04;
         break;
      case PCD_CALCCRC:                // LoAlert and TxIRq
         timerCtl = 0x00;              // start and stop timer manually            
         irqEn = 0x11;
         waitFor = 0x10;
         break;
      case PCD_AUTHENT2:               // IdleIRq
         irqEn = 0x04;
         waitFor = 0x04;
         break;
      case PCD_RECEIVE:                // HiAlert and IdleIRq
         info->nBitsReceived = -(ReadRC(RegBitFraming) >> 4);      
         timerCtl = 0x04;              // start timer manually and stop
                                       // with first bit received
         irqEn = 0x06;
         waitFor = 0x04;
         break;
      case PCD_LOADKEY:                // IdleIRq
         timerCtl = 0x00;              // start and stop timer manually            
         irqEn = 0x05;
         waitFor = 0x04;
         break;
      case PCD_TRANSMIT:               // LoAlert and IdleIRq
         timerCtl = 0x00;              // start and stop timer manually            
         irqEn = 0x05;
         waitFor = 0x04;
         break;
      case PCD_TRANSCEIVE:             // TxIrq, RxIrq, IdleIRq and LoAlert
         info->nBitsReceived = -(ReadRC(RegBitFraming) >> 4);
         irqEn = 0x3D;
         waitFor = 0x04;
         break;
      default:
         status = MI_UNKNOWN_COMMAND;
   }        
   if (status == MI_OK)
   {

//      rxMultiple = ReadRC(RegDecoderControl) & 0x40;
//      if (!rxMultiple)
//         SetBitMask(RegDecoderControl,0x40);

      
      // Initialize uC Timer for global Timeout management
      if(stuInRegs.bTmoStatus == TMO_NO_LIMIT)
      {
        timerCtl = 0x04;  
      }
      else
      {
        irqEn |= 0x20;                        // always enable timout irq
        waitFor |= 0x20;                      // always wait for timeout 
      }

      WriteRC(RegInterruptEn,irqEn | 0x80);  //necessary interrupts are enabled // count up from 1
      b = ReadRC(RegInterruptEn);
//      WriteRC(RegInterruptEn, 0x02);	                                     	//2012 5 14 cnn debug  ×¢ÊÍµô
//      b = ReadRC(RegInterruptEn);
	  b = b;  //to kill warning
      
      WriteRC(RegTimerControl,timerCtl);

      if(stuInRegs.bTmoStatus != TMO_NO_LIMIT)
      {
        if (~timerCtl & 0x02) // if no start condition could be detected, then
                                // start timer manually
        {
           SetBitMask(RegControl,0x02);
        }
      }

      WriteRC(RegCommand,cmd);               //start command   
      
      // wait for commmand completion
      // a command is completed, if the corresponding interrupt occurs
      // or a timeout is signaled  

      if(stuInRegs.bTmoStatus == TMO_NO_LIMIT)
      {
        return  status;   
      }
      else
      {
        while (!(MpIsrInfo->irqSource & waitFor));
      }
                                         // wait for cmd completion or timeout

      SetBitMask(RegControl,0x04);         // stop timer now
      WriteRC(RegInterruptEn,0x7F);          // disable all interrupts
      WriteRC(RegInterruptRq,0x7F);          // clear all interrupt requests
      
      WriteRC(RegCommand,PCD_IDLE);          // reset command register

      status = MpIsrInfo->status;          // set status
      if (MpIsrInfo->irqSource & 0x20) // if timeout expired - look at old error state
         MpIsrInfo->errFlags |= MpIsrInfo->saveErrorState;
      MpIsrInfo->errFlags &=  validErrorFlags;
      if (MpIsrInfo->errFlags) // error occured
      {
         if (MpIsrInfo->errFlags & 0x01)   // collision detected
         {
            info->collPos = ReadRC(RegCollPos); // read collision position
            status = MI_COLLERR;
         }
         else
         {
            if (MpIsrInfo->errFlags & 0x02)   // parity error
            {
               status = MI_PARITYERR;
            }
         }
         if (MpIsrInfo->errFlags & 0x04)   // framing error
         {
            status = MI_FRAMINGERR;
         }
			else if (MpIsrInfo->errFlags & 0x10)   // FIFO overflow
         {
            FlushFIFO();
            status = MI_OVFLERR;
         }
			else if (MpIsrInfo->errFlags & 0x08) // CRC error
         {
            status = MI_CRCERR;
         }
 			else if (status == MI_OK)
            status = MI_NY_IMPLEMENTED;
         // key error occures always, because of 
         // missing crypto 1 keys loaded
      }
      // if the last command was TRANSCEIVE, the number of 
      // received bits must be calculated - even if an error occured
      if (cmd == PCD_TRANSCEIVE || cmd == PCD_RECEIVE)
      {
         // number of bits in the last byte
         lastBits = ReadRC(RegSecondaryStatus) & 0x07;
         if (lastBits)
            info->nBitsReceived += (info->nBytesReceived-1) * 8 + lastBits;
         else
            info->nBitsReceived += info->nBytesReceived * 8;
      }
      
//      if (!rxMultiple)
//         ClearBitMask(RegDecoderControl,0x40);
   }

   READER_INT_DISABLE();

   MpIsrInfo = 0;         // reset interface variables for ISR
   MpIsrOut  = 0;
   MpIsrIn   = 0; 

   // restore the previous value for the FIFO water level
   WriteRC(RegFIFOLevel, waterLevelBackup);

   return status;
}   

signed char  PcdStopRecvCmd(unsigned char *buffer, unsigned short *RecvLen)
{
  signed char   status    = MI_OK;

  WriteRC(RegInterruptEn,0x7F);          // disable all interrupts
  WriteRC(RegInterruptRq,0x7F);          // clear all interrupt requests
  
  WriteRC(RegCommand,PCD_IDLE);          // reset command register

  status = MpIsrInfo->status;          // set status  

  if (MpIsrInfo->irqSource & 0x20) // if timeout expired - look at old error state
     MpIsrInfo->errFlags |= MpIsrInfo->saveErrorState;
  MpIsrInfo->errFlags &=  validErrorFlags;
  if (MpIsrInfo->errFlags) // error occured
  {
     if (MpIsrInfo->errFlags & 0x01)   // collision detected
     {
        MInfo.collPos = ReadRC(RegCollPos); // read collision position
        status = MI_COLLERR;
     }
     else
     {
        if (MpIsrInfo->errFlags & 0x02)   // parity error
        {
           status = MI_PARITYERR;
        }
     }
     if (MpIsrInfo->errFlags & 0x04)   // framing error
     {
        status = MI_FRAMINGERR;
     }
		else if (MpIsrInfo->errFlags & 0x10)   // FIFO overflow
     {
        FlushFIFO();
        status = MI_OVFLERR;
     }
 		else if (MpIsrInfo->errFlags & 0x08) // CRC error
     {
        status = MI_CRCERR;
     }
		else if (status == MI_OK)
        status = MI_NY_IMPLEMENTED;
     // key error occures always, because of 
     // missing crypto 1 keys loaded
  }

  READER_INT_DISABLE();

  MpIsrInfo = 0;         // reset interface variables for ISR
  MpIsrOut  = 0;
  MpIsrIn   = 0; 
  
  // restore the previous value for the FIFO water level
  WriteRC(RegFIFOLevel, waterLevelBackup);

  memcpy(buffer, MRcvBuffer, MInfo.nBytesReceived);
  *(buffer + MInfo.nBytesReceived) = 0;
  *(buffer + MInfo.nBytesReceived + 1) = 0;
  *RecvLen =  MInfo.nBytesReceived + 2;
  return status;
}

/////////////////////////////////////////////////////////////////////
//          B A S I C   R E G I S T E R   S E T T I N G S
///////////////////////////////////////////////////////////////////////
signed char PcdBasicRegisterConfiguration(void)
{
  // test clock Q calibration - value in the range of 0x46 expected
  WriteRC(RegClockQControl,0x0);
  WriteRC(RegClockQControl,0x40);
  SleepUs(100);  // wait approximately 100 us - calibration in progress
  ClearBitMask(RegClockQControl,0x40); // clear bit ClkQCalib for 
                                       // further calibration
  // enable auto power down
  WriteRC(RegRxControl2,0x01);
  WriteRC(RegIRqPinConfig,0x03); // interrupt active low enable
  WriteRC(RegRxWait,0x03);
  WriteRC(RegMfOutSelect,0x05);

//  ReadAllRegs();
  return MI_OK;
}    
/*
void ReadAllRegs(void)
{
  uint8_t abRegs[64];
  uint8_t b;
  
  for(b=0; b<64; b++)
    abRegs[b] = ReadRC(b);
}
*/
