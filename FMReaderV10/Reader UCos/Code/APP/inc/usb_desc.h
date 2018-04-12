/******************** (C) COPYRIGHT 2010 STMicroelectronics ********************
* File Name          : usb_desc.h
* Author             : MCD Application Team
* Version            : V3.2.1
* Date               : 07/05/2010
* Description        : Descriptor Header for Virtual COM Port Device
********************************************************************************
* THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __USB_DESC_H
#define __USB_DESC_H

/* Includes ------------------------------------------------------------------*/
/* Exported types ------------------------------------------------------------*/
/* Exported constants --------------------------------------------------------*/
/* Exported macro ------------------------------------------------------------*/
/* Exported define -----------------------------------------------------------*/
#define USB_DEVICE_DESCRIPTOR_TYPE              0x01
#define USB_CONFIGURATION_DESCRIPTOR_TYPE       0x02
#define USB_STRING_DESCRIPTOR_TYPE              0x03
#define USB_INTERFACE_DESCRIPTOR_TYPE           0x04
#define USB_ENDPOINT_DESCRIPTOR_TYPE            0x05

#define CCID_READER_BULK_SIZE              64
#define CCID_READER_INT_SIZE               8

#define CCID_READER_SIZE_DEVICE_DESC        18
#define CCID_READER_SIZE_CONFIG_DESC        93
#define CCID_READER_SIZE_STRING_LANGID      4
#define CCID_READER_SIZE_STRING_VENDOR      10
#define CCID_READER_SIZE_STRING_PRODUCT     40
#define CCID_READER_SIZE_STRING_SERIAL      26

#define STANDARD_ENDPOINT_DESC_SIZE             0x09

/* Exported functions ------------------------------------------------------- */
extern const uint8_t CCID_READER_DeviceDescriptor[CCID_READER_SIZE_DEVICE_DESC];
extern const uint8_t CCID_READER_ConfigDescriptor[CCID_READER_SIZE_CONFIG_DESC];

extern const uint8_t CCID_READER_StringLangID[CCID_READER_SIZE_STRING_LANGID];
extern const uint8_t CCID_READER_StringVendor[CCID_READER_SIZE_STRING_VENDOR];
extern const uint8_t CCID_READER_StringProduct[CCID_READER_SIZE_STRING_PRODUCT];

#endif /* __USB_DESC_H */
/******************* (C) COPYRIGHT 2010 STMicroelectronics *****END OF FILE****/
