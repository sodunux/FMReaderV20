/******************** (C) COPYRIGHT 2010 STMicroelectronics ********************
* File Name          : usb_desc.c
* Author             : MCD Application Team
* Version            : V3.2.1
* Date               : 07/05/2010
* Description        : Descriptors for CCID Reader
********************************************************************************
* THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Includes ------------------------------------------------------------------*/
#include "usb_lib.h"
#include "usb_desc.h"

/* USB Standard Device Descriptor */
const uint8_t CCID_READER_DeviceDescriptor[] =
  {
    0x12,   /* bLength */
    USB_DEVICE_DESCRIPTOR_TYPE,     /* bDescriptorType */
    0x00,
    0x02,   /* bcdUSB = 2.00 */
    0x0B,   /* bDeviceClass: Smart Card Device Class */
    0x00,   /* bDeviceSubClass */
    0x00,   /* bDeviceProtocol */
    0x40,   /* bMaxPacketSize0 */
    0x85,
    0x04,   /* idVendor = 0x0485 */
    0x26,
    0x10,   /* idProduct = 0x1026 */
    0x10,
    0x01,   /* bcdDevice = 1.10 */
    1,              /* Index of string descriptor describing manufacturer */
    2,              /* Index of string descriptor describing product */
    0,              /* Index of string descriptor describing the device's serial number */
    0x01    /* bNumConfigurations */
  };

const uint8_t CCID_READER_ConfigDescriptor[] =
  {
    /*Configuation Descriptor*/
    0x09,   /* bLength: Configuation Descriptor size */
    USB_CONFIGURATION_DESCRIPTOR_TYPE,      /* bDescriptorType: Configuration */
    CCID_READER_SIZE_CONFIG_DESC,       /* wTotalLength:no of returned bytes */
    0x00,
    0x01,   /* bNumInterfaces: 1 interface */
    0x01,   /* bConfigurationValue: Configuration value */
    0x00,   /* iConfiguration: Index of string descriptor describing the configuration */
    0xC0,   /* bmAttributes: self powered */
    0x64,   /* MaxPower 100*2=200 mA */
    /*Interface Descriptor*/
    0x09,   /* bLength: Interface Descriptor size */
    USB_INTERFACE_DESCRIPTOR_TYPE,  /* bDescriptorType: Interface */
    0x00,   /* bInterfaceNumber: Number of Interface */
    0x00,   /* bAlternateSetting: Alternate setting */
    0x03,   /* bNumEndpoints: Three endpoints used */
    0x0B,   /* bInterfaceClass: Smart Card Device Class */
    0x00,   /* bInterfaceSubClass: None */
    0x00,   /* bInterfaceProtocol: CCID */
    0x00,   /* iInterface: 0 */
    /* Smart Card Device Class Descriptor*/
    0x36,   /* bLength: Smart Card Device Class Descriptor size */
    0x21,   /* bDescriptorType:*/
    0x10,
    0x01,   /* bcdCCID: 1.10*/
    0x00,   /* bMaxSlotIndex: */
    0x07,   /* bVoltageSupport: 0x01 5.0V
                                0x02 3.0V
                                0x04 1.8V */
    0x03,
    0x00,
    0x00,
    0x00,   /* dwProtocol: 0x00000001 T=0 
                           0x00000002 T=1 */
    0xFC,
    0x0D,
    0x00,
    0x00,   /* dwDefaultClock: 3580kHz */
    0xFC,
    0x0D,
    0x00,
    0x00,   /* dwMaximumClock: 3580kHz */
    0x01,   /* bNumClockSupported: 1 */
    0x80,
    0x25,
    0x00,
    0x00,   /* dwDataRate: 9600bps */
    0x00,
    0xC2,
    0x01,
    0x00,   /* dwMaxDataRate: 115200bps */
    0x04,   /* bNumDataratesSupported: 4 */
    0x00,
    0x01,
    0x00,
    0x00,   /* dwMaxIFSD: 256 */
    0x00,
    0x00,
    0x00,
    0x00,   /* dwSynchProtocols: 0*/
    0x00,
    0x00,
    0x00,
    0x00,   /* dwMechanical: 0*/
    0xB2,
    0x00,
    0x02,
    0x00,   /* dwFeature: Short APDU Exchange */
    0x0F,
    0x01,
    0x00,
    0x00,   /* dwMaxCCIDMessageLength: 271 */
    0xFF,   /* bClassGetResponse: echo */
    0xFF,   /* bClassEnvelope: echo */
    0x00,
    0x00,   /* wLcdLayout: No LCD */
    0x00,   /* bPINSupport: No support */
    0x01,   /* bMaxCCIDBusySlots: 1 */
    /*Endpoint 1 Descriptor*/
    0x07,   /* bLength: Endpoint Descriptor size */
    USB_ENDPOINT_DESCRIPTOR_TYPE,   /* bDescriptorType: Endpoint */
    0x01,   /* bEndpointAddress: (OUT1) */
    0x02,   /* bmAttributes: Bulk */
    CCID_READER_BULK_SIZE,      /* wMaxPacketSize: */
    0x00,
    0xFF,   /* bInterval: */        
    /*Endpoint 2 Descriptor*/
    0x07,   /* bLength: Endpoint Descriptor size */
    USB_ENDPOINT_DESCRIPTOR_TYPE,   /* bDescriptorType: Endpoint */
    0x82,   /* bEndpointAddress: (IN2) */
    0x02,   /* bmAttributes: Bulk */
    CCID_READER_BULK_SIZE,      /* wMaxPacketSize: */
    0x00,
    0xFF,   /* bInterval: */
    /*Endpoint 3 Descriptor*/
    0x07,   /* bLength: Endpoint Descriptor size */
    USB_ENDPOINT_DESCRIPTOR_TYPE,   /* bDescriptorType: Endpoint */
    0x83,   /* bEndpointAddress: (IN3) */
    0x03,   /* bmAttributes: Interrupt */
    CCID_READER_INT_SIZE,             /* wMaxPacketSize: */
    0x00,
    0x0A,   /* bInterval: 10ms */
  };

/* USB String Descriptors */
const uint8_t CCID_READER_StringLangID[CCID_READER_SIZE_STRING_LANGID] =
  {
    CCID_READER_SIZE_STRING_LANGID,
    USB_STRING_DESCRIPTOR_TYPE,
    0x09,
    0x04 /* LangID = 0x0409: U.S. English */
  };

const uint8_t CCID_READER_StringVendor[CCID_READER_SIZE_STRING_VENDOR] =
  {
    CCID_READER_SIZE_STRING_VENDOR,     /* Size of Vendor string */
    USB_STRING_DESCRIPTOR_TYPE,             /* bDescriptorType*/
    /* Manufacturer: "FMSH" */
    'F', 0, 'M', 0, 'S', 0, 'H', 0
  };

const uint8_t CCID_READER_StringProduct[CCID_READER_SIZE_STRING_PRODUCT] =
  {
    CCID_READER_SIZE_STRING_PRODUCT,          /* bLength */
    USB_STRING_DESCRIPTOR_TYPE,        /* bDescriptorType */
    /* Product name: "FM309 Reader V1.5" */
    'R', 0, 'e', 0, 'a', 0, 'd', 0, 'e', 0, 'r', 0, ' ', 0,	//Reader
	'V', 0, '1', 0, '.', 0, '5', 0, '_', 0, '_', 0,			//V1.5__
	'1', 0, '5', 0, '1', 0, '0', 0, '2', 0, '7', 0			//151022  (date)
	/*
	//reader version  V1.4
	//2013-04-10 gaoxuebing。以后更新版本号的时候修改此处的最后两行行几个引号中的数字，和日期
	//
  */};

/******************* (C) COPYRIGHT 2010 STMicroelectronics *****END OF FILE****/
