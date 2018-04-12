/******************** (C) COPYRIGHT 2010 STMicroelectronics ********************
* File Name          : usb_prop.c
* Author             : MCD Application Team
* Version            : V3.2.1
* Date               : 07/05/2010
* Description        : All processing related to CCID Reader
********************************************************************************
* THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
* WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE TIME.
* AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY DIRECT,
* INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING FROM THE
* CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE CODING
* INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
*******************************************************************************/

/* Includes ------------------------------------------------------------------*/
#include "includes.h"
#include "CCID.h"

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* -------------------------------------------------------------------------- */
/*  Structures initializations */
/* -------------------------------------------------------------------------- */

DEVICE Device_Table =
  {
    EP_NUM,
    1
  };

DEVICE_PROP Device_Property =
  {
    CCID_READER_init,
    CCID_READER_Reset,
    CCID_READER_Status_In,
    CCID_READER_Status_Out,
    CCID_READER_Data_Setup,
    CCID_READER_NoData_Setup,
    CCID_READER_Get_Interface_Setting,
    CCID_READER_GetDeviceDescriptor,
    CCID_READER_GetConfigDescriptor,
    CCID_READER_GetStringDescriptor,
    0,
    0x40 /*MAX PACKET SIZE*/
  };

USER_STANDARD_REQUESTS User_Standard_Requests =
  {
    CCID_READER_GetConfiguration,
    CCID_READER_SetConfiguration,
    CCID_READER_GetInterface,
    CCID_READER_SetInterface,
    CCID_READER_GetStatus,
    CCID_READER_ClearFeature,
    CCID_READER_SetEndPointFeature,
    CCID_READER_SetDeviceFeature,
    CCID_READER_SetDeviceAddress
  };

ONE_DESCRIPTOR Device_Descriptor =
  {
    (uint8_t*)CCID_READER_DeviceDescriptor,
    CCID_READER_SIZE_DEVICE_DESC
  };

ONE_DESCRIPTOR Config_Descriptor =
  {
    (uint8_t*)CCID_READER_ConfigDescriptor,
    CCID_READER_SIZE_CONFIG_DESC
  };

ONE_DESCRIPTOR String_Descriptor[3] =
  {
    {(uint8_t*)CCID_READER_StringLangID, CCID_READER_SIZE_STRING_LANGID},
    {(uint8_t*)CCID_READER_StringVendor, CCID_READER_SIZE_STRING_VENDOR},
    {(uint8_t*)CCID_READER_StringProduct, CCID_READER_SIZE_STRING_PRODUCT},
  };

/* Extern variables ----------------------------------------------------------*/

/* Private function prototypes -----------------------------------------------*/
/* Extern function prototypes ------------------------------------------------*/
/* Private functions ---------------------------------------------------------*/
/*******************************************************************************
* Function Name  : CCID_READER_init.
* Description    : CCID Reader init routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CCID_READER_init(void)
{
  pInformation->Current_Configuration = 0;

  /* Connect the device */
  PowerOn();

  /* Perform basic device initialization operations */
  USB_SIL_Init();

  bDeviceState = UNCONNECTED;
}

/*******************************************************************************
* Function Name  : CCID_READER_Reset
* Description    : CCID_READER Mouse reset routine
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CCID_READER_Reset(void)
{
  /* Set CCID_READER DEVICE as not configured */
  pInformation->Current_Configuration = 0;

  /* Current Feature initialization */
  pInformation->Current_Feature = CCID_READER_ConfigDescriptor[7];

  /* Set CCID_READER DEVICE with the default Interface*/
  pInformation->Current_Interface = 0;

#ifdef STM32F10X_CL     
  /* EP0 is already configured by USB_SIL_Init() function */
  
  /* Init EP1 IN as Bulk endpoint */
  OTG_DEV_EP_Init(EP1_IN, OTG_DEV_EP_TYPE_BULK, CCID_READER_DATA_SIZE);
  
  /* Init EP2 IN as Interrupt endpoint */
  OTG_DEV_EP_Init(EP2_IN, OTG_DEV_EP_TYPE_INT, CCID_READER_INT_SIZE);

  /* Init EP3 OUT as Bulk endpoint */
  OTG_DEV_EP_Init(EP3_OUT, OTG_DEV_EP_TYPE_BULK, CCID_READER_DATA_SIZE);  
#else 

  SetBTABLE(BTABLE_ADDRESS);

  /* Initialize Endpoint 0 */
  SetEPType(ENDP0, EP_CONTROL);
  SetEPTxStatus(ENDP0, EP_TX_STALL);
  SetEPRxAddr(ENDP0, ENDP0_RXADDR);
  SetEPTxAddr(ENDP0, ENDP0_TXADDR);
  Clear_Status_Out(ENDP0);
  SetEPRxCount(ENDP0, Device_Property.MaxPacketSize);
  SetEPRxValid(ENDP0);

  /* Initialize Endpoint 1 */
  SetEPType(ENDP1, EP_BULK);
  SetEPRxAddr(ENDP1, ENDP1_RXADDR);
  SetEPRxCount(ENDP1, CCID_READER_BULK_SIZE);
  SetEPTxStatus(ENDP1, EP_TX_DIS);
  SetEPRxStatus(ENDP1, EP_RX_VALID);

  /* Initialize Endpoint 2 */
  SetEPType(ENDP2, EP_BULK);
  SetEPTxAddr(ENDP2, ENDP2_TXADDR);
  SetEPRxStatus(ENDP2, EP_RX_DIS);
  SetEPTxStatus(ENDP2, EP_TX_NAK);

  /* Initialize Endpoint 3 */
  SetEPType(ENDP3, EP_INTERRUPT);
  SetEPTxAddr(ENDP3, ENDP3_TXADDR);
  SetEPRxStatus(ENDP3, EP_RX_DIS);
  SetEPTxStatus(ENDP3, EP_TX_NAK);

  /* Set this device to response on default address */
  SetDeviceAddress(0);
#endif /* STM32F10X_CL */

  bDeviceState = ATTACHED;
}

/*******************************************************************************
* Function Name  : CCID_READER_SetConfiguration.
* Description    : Udpade the device state to configured.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CCID_READER_SetConfiguration(void)
{
  DEVICE_INFO *pInfo = &Device_Info;

  if (pInfo->Current_Configuration != 0)
  {
    /* Device configured */
    bDeviceState = CONFIGURED;
  }
}

/*******************************************************************************
* Function Name  : CCID_READER_SetConfiguration.
* Description    : Udpade the device state to addressed.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CCID_READER_SetDeviceAddress (void)
{
  bDeviceState = ADDRESSED;
}

/*******************************************************************************
* Function Name  : CCID_READER_Status_In.
* Description    : CCID Reader Status In Routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CCID_READER_Status_In(void)
{}

/*******************************************************************************
* Function Name  : CCID_READER_Status_Out
* Description    : CCID Reader Status OUT Routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CCID_READER_Status_Out(void)
{}

/*******************************************************************************
* Function Name  : CCID_READER_Data_Setup
* Description    : handle the data class specific requests
* Input          : Request Nb.
* Output         : None.
* Return         : USB_UNSUPPORT or USB_SUCCESS.
*******************************************************************************/
RESULT CCID_READER_Data_Setup(uint8_t RequestNo)
{
  uint8_t    *(*CopyRoutine)(uint16_t);

  CopyRoutine = NULL;

  /* Handle GET_CLOCK_FREQUECNIES and GET_DATA_RATE */
  if(Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
  {
    if(RequestNo == GET_CLOCK_FREQUENCIES)
    {
      CopyRoutine = CCID_READER_GetClockFrequencies;  
    }
    else if(RequestNo == GET_DATA_RATES)
    {
      CopyRoutine = CCID_READER_GetDataRates;  
    }
  }

  if (CopyRoutine == NULL)
  {
    return USB_UNSUPPORT;
  }

  pInformation->Ctrl_Info.CopyData = CopyRoutine;
  pInformation->Ctrl_Info.Usb_wOffset = 0;
  (*CopyRoutine)(0);
  return USB_SUCCESS;
}

/*******************************************************************************
* Function Name  : CCID_READER_NoData_Setup.
* Description    : handle the no data class specific requests.
* Input          : Request Nb.
* Output         : None.
* Return         : USB_UNSUPPORT or USB_SUCCESS.
*******************************************************************************/
RESULT CCID_READER_NoData_Setup(uint8_t RequestNo)
{

  if (Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
  {
    if (RequestNo == ABORT)
    {
      return USB_SUCCESS;
    }
  }

  return USB_UNSUPPORT;
}

/*******************************************************************************
* Function Name  : CCID_READER_GetDeviceDescriptor.
* Description    : Gets the device descriptor.
* Input          : Length.
* Output         : None.
* Return         : The address of the device descriptor.
*******************************************************************************/
uint8_t *CCID_READER_GetDeviceDescriptor(uint16_t Length)
{
  return Standard_GetDescriptorData(Length, &Device_Descriptor);
}

/*******************************************************************************
* Function Name  : CCID_READER_GetConfigDescriptor.
* Description    : get the configuration descriptor.
* Input          : Length.
* Output         : None.
* Return         : The address of the configuration descriptor.
*******************************************************************************/
uint8_t *CCID_READER_GetConfigDescriptor(uint16_t Length)
{
  return Standard_GetDescriptorData(Length, &Config_Descriptor);
}

/*******************************************************************************
* Function Name  : CCID_READER_GetStringDescriptor
* Description    : Gets the string descriptors according to the needed index
* Input          : Length.
* Output         : None.
* Return         : The address of the string descriptors.
*******************************************************************************/
uint8_t *CCID_READER_GetStringDescriptor(uint16_t Length)
{
  uint8_t wValue0 = pInformation->USBwValue0;
  if (wValue0 > 4)
  {
    return NULL;
  }
  else
  {
    return Standard_GetDescriptorData(Length, &String_Descriptor[wValue0]);
  }
}

/*******************************************************************************
* Function Name  : CCID_READER_Get_Interface_Setting.
* Description    : test the interface and the alternate setting according to the
*                  supported one.
* Input1         : uint8_t: Interface : interface number.
* Input2         : uint8_t: AlternateSetting : Alternate Setting number.
* Output         : None.
* Return         : The address of the string descriptors.
*******************************************************************************/
RESULT CCID_READER_Get_Interface_Setting(uint8_t Interface, uint8_t AlternateSetting)
{
  if (AlternateSetting > 0)
  {
    return USB_UNSUPPORT;
  }
  else if (Interface > 1)
  {
    return USB_UNSUPPORT;
  }
  return USB_SUCCESS;
}

/*******************************************************************************
* Function Name  : CCID_READER_GetClockFrequencies.
* Description    : Send the clock frequencies supported to the PC host.
* Input          : Length.
* Output         : None.
* Return         : Inecoding structure base address.
*******************************************************************************/
uint8_t *CCID_READER_GetClockFrequencies(uint16_t Length)
{
  if (Length == 0)
  {
    pInformation->Ctrl_Info.Usb_wLength = sizeof(CCID_READER_CLOCK_FREQUENCIES);
    return NULL;
  }
  return(uint8_t *)&CCID_READER_CLOCK_FREQUENCIES;
}

extern uint8_t bUsbStatus;
/*******************************************************************************
* Function Name  : CCID_READER_GetDataRate.
* Description    : Send the data rate supported to the PC host.
* Input          : Length.
* Output         : None.
* Return         : Inecoding structure base address.
*******************************************************************************/
uint8_t *CCID_READER_GetDataRates(uint16_t Length)
{
  if (Length == 0)
  {
    pInformation->Ctrl_Info.Usb_wLength = sizeof(CCID_READER_DATA_RATES);
    return NULL;
  }
  bUsbStatus++;
  return(uint8_t *)&CCID_READER_DATA_RATES;
}
/******************* (C) COPYRIGHT 2010 STMicroelectronics *****END OF FILE****/

