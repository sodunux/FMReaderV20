/******************** (C) COPYRIGHT 2010 STMicroelectronics ********************
* File Name          : usb_prop.h
* Author             : MCD Application Team
* Version            : V3.2.1
* Date               : 07/05/2010
* Description        : All processing related to Virtual COM Port Demo (Endpoint 0)
********************************************************************************
* THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __usb_prop_H
#define __usb_prop_H

/* Includes ------------------------------------------------------------------*/
/* Exported types ------------------------------------------------------------*/
typedef struct
{
  uint32_t bitrate;
  uint8_t format;
  uint8_t paritytype;
  uint8_t datatype;
}LINE_CODING;

/* Exported constants --------------------------------------------------------*/
/* Exported macro ------------------------------------------------------------*/
/* Exported define -----------------------------------------------------------*/

#define CCID_READER_GetConfiguration          NOP_Process
//#define CCID_READER_SetConfiguration          NOP_Process
#define CCID_READER_GetInterface              NOP_Process
#define CCID_READER_SetInterface              NOP_Process
#define CCID_READER_GetStatus                 NOP_Process
#define CCID_READER_ClearFeature              NOP_Process
#define CCID_READER_SetEndPointFeature        NOP_Process
#define CCID_READER_SetDeviceFeature          NOP_Process
//#define CCID_READER_SetDeviceAddress          NOP_Process

#define ABORT                       0x01
#define GET_CLOCK_FREQUENCIES       0x02
#define GET_DATA_RATES              0x03

/* Exported functions ------------------------------------------------------- */
void CCID_READER_init(void);
void CCID_READER_Reset(void);
void CCID_READER_SetConfiguration(void);
void CCID_READER_SetDeviceAddress (void);
void CCID_READER_Status_In (void);
void CCID_READER_Status_Out (void);
RESULT CCID_READER_Data_Setup(uint8_t);
RESULT CCID_READER_NoData_Setup(uint8_t);
RESULT CCID_READER_Get_Interface_Setting(uint8_t Interface, uint8_t AlternateSetting);
uint8_t *CCID_READER_GetDeviceDescriptor(uint16_t );
uint8_t *CCID_READER_GetConfigDescriptor(uint16_t);
uint8_t *CCID_READER_GetStringDescriptor(uint16_t);

uint8_t *CCID_READER_GetClockFrequencies(uint16_t Length);
uint8_t *CCID_READER_GetDataRates(uint16_t Length);

#endif /* __usb_prop_H */

/******************* (C) COPYRIGHT 2010 STMicroelectronics *****END OF FILE****/

