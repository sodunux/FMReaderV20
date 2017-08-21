/*********************************************************************
*                                                                    *
*   Copyright (c) 2010 Shanghai FuDan MicroElectronic Inc, Ltd.      *
*   All rights reserved. Licensed Software Material.                 *
*                                                                    *
*   Unauthorized use, duplication, or distribution is strictly       *
*   prohibited by law.                                               *
*                                                                    *
**********************************************************************
*********************************************************************/

/*********************************************************************/
/* 	ARM���ñ����궨��													 */
/* 	��Ҫ����:														 */
/* 		1.�����в�������											 */
/* 	����:����Ȫ														 */
/* 	����ʱ��:2012��5��16��											 */
/* 																	 */
/*********************************************************************/

#ifndef _DEFINE_H_
#define _DEFINE_H_


/* Includes ------------------------------------------------------------------*/
#include "stm32f10x_map.h"

//<<< Use Configuration Wizard in Context Menu >>>
// <h> ��ʼ��ѡ��

//     <e0> USART1ʹ��ѡ��	
//	   </e>	  <i>"USBתUART�����ڿ���ARM"	
//     <e1> USART3ʹ��ѡ��  <i>"UART3�����ڿ���FM175xx"
//	   </e> 
//     <e2> I2Cʹ��ѡ��  <i>"I2C�����ڿ���FM175xx"
//	   </e>

//     <e3>    SPI2ʹ��ѡ��
//	   </e>

//     <e4>    USBʹ��ѡ��
//	   </e>

//     <o5.0..18> UART1 ������ѡ��  <9600=>9600
//			      <19200=>19200               <38400=>38400
//			      <57600=>57600               <115200=>115200

//     <o6.0..18> UART2 ������ѡ��  <9600=>9600
//			      <19200=>19200               <38400=>38400
//			      <57600=>57600               <115200=>115200

//     <o7.0..7> FM175xx�ӿڷ�ʽѡ��  <1=>UART�ӿ�
//			      <2=>I2C           <3=>SPI�ӿ�
//			      <4=>R/W�߷ֿ�Data/Addr�ֿ�       <5=>R/W�߷ֿ�Data/Addr����
//			      <6=>R/W�߸���Data/Addr�ֿ�       <7=>R/W�߸���Data/Addr����

//     <e8>FM175xx Uart͸��ģʽ
//	   </e>

//	   <S0.60>�汾��Ϣ<i>"�����³�����Ϣ"
//	   <S1.60>Ӳ���汾��Ϣ
// </h>	

// <h> FM175xx IRQʹ��ѡ��
//     <e9.7> IRQ INVʹ����������ѡ��IRQ�Ž���IRQ�Ĵ���λ�෴	
//	   </e>
//     <e9.6> TxIEn	
//	   </e>  
//     <e9.5> RxIEn	
//	   </e>
//     <e9.4> IdleIEn	
//	   </e>
//     <e9.3> HiAlertIEn	
//	   </e>
//     <e9.2> LoAlertIEn	
//	   </e>
//     <e9.1> ErrIEn	
//	   </e>
//     <e9.0> TimerIEn	
//	   </e>
//     <e10.7>IRQPushPul;�����ѡ��IRQΪCMOS���������Ϊ��©���	
//	   </e>
//     <e10.4> SiginActIEn	
//	   </e>
//     <e10.3> ModeIEn	
//	   </e>
//     <e10.2> CRCIEn	
//	   </e>
//     <e10.1> RfOnIEn	
//	   </e>
//     <e10.0> RfOffIEn	
//	   </e>
// </h>

// <h> FM175xx ����ѡ��
//     <e11.7> ReaderA����ѡ��(bit7)	
//	   </e>
//     <e11.6> ReaderB����ѡ��(bit6)	
//	   </e>
//     <e11.5> ReaderF����ѡ��(bit5)	
//	   </e>
//     <e11.4> CardA����ѡ��(bit4)	
//	   </e>
//     <e11.3> CardF����ѡ��(bit3)	
//	   </e>
//     <e11.2> S2C_�⿨����ѡ��(bit2)	
//	   </e>
//     <e11.1> S2C_�ڿ�����ѡ��(bit1)	
//	   </e>
//     <e11.0> LPCD����ѡ��(bit0)	
//	   </e>

// </h>

// <h> FM175xx ����ѡ��
//     <e12.7> LPCD���Ե�Уѡ��(bit7)	
//	   </e>
//     <e12.6> ReaderB����ѡ��(bit6)	
//	   </e>
//     <e12.5> ReaderF����ѡ��(bit5)	
//	   </e>
//     <e12.4> CardA����ѡ��(bit4)	
//	   </e>
//     <e12.3> CardF����ѡ��(bit3)	
//	   </e>
//     <e12.2> S2C_�⿨����ѡ��(bit2)	
//	   </e>
//     <e12.1> S2C_�ڿ�����ѡ��(bit1)	
//	   </e>
//     <e11.0> LPCD����ѡ��(bit0)	
//	   </e>
// </h>

//     <o13.0..7> FM175xx ��װ��ʽ  <02=>QFN48��װ
//			      <01=>QFN40��װ              <00=>QFN32��װ

#define  UART1_EN_SEL        0x01
#define  UART3_EN_SEL  	     0x01
#define	 I2C_EN_SEL		     0x00
#define  SPI2_EN_SEL         0x01
#define  USB_EN_SEL          0x00
#define  UART1_BaudRate      115200
#define  UART3_BaudRate      9600
#define	 FM175xx_INTF_DEF      0x01
#define  UART_AUTO_TRANSMIT  0x00
#define  VersionInfo	     "FM175xx DEMO Pro V1.1"
#define  HardInfo			 "FM175xx DEMO Board V1.1 "	
#define  COMMIEN_DEF          0x80
#define  DIVIEN_DEF           0x80

#define  FM175xx_FUNC_SEL       0x00
#define  FM175xx_TEST_SEL       0x00

#define  FM175xx_PACKAGE		  0x1
//<<< end of configuration section >>>


//     <o7.0..7> FM175xx�ӿڷ�ʽѡ��  <1=>UART�ӿ�
//			      <2=>I2C           <3=>SPI�ӿ�
//			      <4=>R/W�߷ֿ�Data/Addr�ֿ�       <5=>R/W�߷ֿ�Data/Addr����
//			      <6=>R/W�߸���Data/Addr�ֿ�       <7=>R/W�߸���Data/Addr����

#define FM175xx_INTF_UART		0x01
#define FM175xx_INTF_I2C		0x02               
#define FM175xx_INTF_SPI		0x03
#define FM175xx_INTF_PAR1		0x04
#define FM175xx_INTF_PAR2		0x05
#define FM175xx_INTF_PAR3		0x06
#define FM175xx_INTF_PAR4		0x07

//;֡�ĳ���
//ͨ���ֽ��붨��
#define 	STX 			0x02		//��ʼ�ַ�
#define 	ETX 			0x03		//�����ַ��ַ�
//#define 	DLE				0x10		//�����ַ�

//#define DEBOULE_FRE

#ifndef true
	#define true				TRUE	//��
#endif
#ifndef false
	#define false				FALSE	//��
#endif
#ifndef True
	#define True				TRUE
	#define False				FALSE
#endif

#ifndef uchar
	#define uchar 				u8
	#define uint				u16
#endif


//Page 0 0x0800 0000 - 0x0800 03FF 1 Kbyte
//Page 1 0x0800 0400 - 0x0800 07FF 1 Kbyte
//Page 2 0x0800 0800 - 0x0800 0BFF 1 Kbyte
//Page 3 0x0800 0C00 - 0x0800 0FFF 1 Kbyte
//Page 4 0x0800 1000 - 0x0800 13FF 1 Kbyte
//.
//.
//.
//.
//.
//.
//.
//.
//.
//Page 127 0x0801 FC00 - 0x0801 FFFF 1 Kbyte
#define FLASH_CARD_UID_PAGE		125
#define FLASH_CARD_UID_START	0x801F400
#define FLASH_USER_INFO_PAGE	126
#define FLASH_USER_INFO_START	0x801F800
#define FLASH_LPCD_CFG_PAGE		127
#define FLASH_LPCD_CFG_START	0x801FC00


#define BIT0               0x01
#define BIT1               0x02
#define BIT2               0x04
#define BIT3               0x08
#define BIT4               0x10
#define BIT5               0x20
#define BIT6               0x40
#define BIT7               0x80

#define UART1_BUF_LEN		0x0400	   //UART1 BUFFER ����
#define UART2_BUF_LEN		0x80	   //UART1 BUFFER ����

#define DEBUG_BUF_LEN		0x80		//Debug��Ϣ������

#define FM175xx_FUNC_CARD     0x10 	   //TypeA ������
#define FM175xx_FUNC_ES2CA    0x12	   //S2CA �⿨����
#define FM175xx_FUNC_IS2CA    0x13	   //S2CA �ڿ�����
#define FM175xx_FUNC_READERA  0x20	   //TypeA Reader����
#define FM175xx_FUNC_READERB  0x21	   //TypeB Reader����
#define FM175xx_FUNC_READERF  0x22	   //TypeF Reader����

#define FM175xx_Card_Buf_Len  0x80	   //�����ݻ��������ȶ���

//******************* ͨ�������붨��********************
#define	 USUCCESS                       0x01    //�����ɹ��Ļظ�
#define  UERROR							0x02	//����ʧ�ܵĻظ�
//0x00~0x0F ��������оƬ������
#define  CMD_MCU_CHG_UART_BAUD			0x08	//�ı�MCU��UartͨѶ������

//0x10~0x1F ���ڽӿ��Լ��ӿ������л�
#define  CMD_FM175xx_CHG_INTF				0x10	//�л��ӿ�����
#define  CMD_FM175xx_CHG_UART_BAUD		0x11	//�л�UARTͨѶ����(ARM��)
#define  CMD_FM175xx_CHG_SPI_BAUD		    0x12	//�л�SPIͨѶ���� (ARM��)
#define  CMD_FM175xx_CHG_I2C_BAUD		    0x13	//�л�I2CͨѶ���� (ARM��)
#define  CMD_FM175xx_CHG_I2C_ADDR		    0x14	//�л�I2C��ַ (ARM��)
#define  CMD_ARM_CHG_TIMER_GATE			0x15	//�л�Timer��Gate�ź�ֵ

//0x20~0x2F LPCD���
#define  CMD_FM175xx_NRSTPD_CTRL          0x20    //NRSTPD����
#define  CMD_FM175xx_ENTER_LPCD           0x21    //����LPCDģʽ
#define  CMD_FM175xx_LPCD_AutoWakeCtrl    0x22    //����AutoWake����:
												//�������������AutoWakeUpֵ������NRSTPD���ͣ�Timer�����ã�����λ������
#define  CMD_FM175xx_Get_AutoWake_Timer   0x23    //��ȡAutoWakeUp����ֵ

//0x50~0x5F ���ڼĴ�������
#define  CMD_RD_REG      				0x51    //��xdata
#define  CMD_WR_REG      				0x53    //дxdata
#define  CMD_I2C_RD_FIFO				0x54	//I2C�ӿڶ�FIFO
#define  CMD_I2C_WR_FIFO				0x55	//I2C�ӿ�дFIFO
#define  CMD_SPI_WR_FIFO				0x57	//SPI�ӿ�дFIFO
#define  CMD_RD_FIFO				    0x58	//��FIFO
#define  CMD_WR_FIFO				    0x59	//дFIFO

#define  CMD_READERA_INITIAL			0x60
#define  CMD_READERA_REQUEST			0x61
#define  CMD_READERA_ANTICOLL			0x62
#define  CMD_READERA_SELECT 			0x63
#define  CMD_READERA_RATS	 			0x64
#define  CMD_READERA_PPS	 			0x65

#define  CMD_READERB_INITIAL			0x68
#define  CMD_READERB_REQUEST			0x69

#define  CMD_READERF_INITIAL			0x70
#define  CMD_READERF_POLL_REQ			0x71
#define  CMD_READERF_POLL_RES			0x72

#define  CMD_CARD_AUTOSENDATQA               0x77                          //for NB1411
#define  CMD_CARD_INITIAL               0x78
#define  CMD_CARD_CONFIG                0x79

#define  CMD_READER_TRANSCEIVE          0x80    //����Reader����Ȼ���������

#define  CMD_AD_DATA_TRANSCEIVE              0x90              //ADC data tranceive
#define  CMD_AD_DATA_RETRANSCEIVE              0x91
#define  CMD_AD_DATA_READY                     0x92

#define  CMD_SYS_HANDON					0xF0	//��������
#define  CMD_SYS_HARD_INFO				0xF1	//Ӳ���汾��Ϣ
#define  CMD_SYS_GET_CFG				0xF2	//������Ϣ��Ӳ���ӿڣ����ʣ�
#define  CMD_CFG_SET_CARD_UID			0xF6
#define  CMD_CFG_GET_CARD_UID			0xF7
#define  CMD_CFG_SET_USERIFO			0xF8
#define  CMD_CFG_GET_USERIFO			0xF9
#define  CMD_CFG_SET_LPCD_CFG		    0xFA
#define  CMD_CFG_GET_LPCD_CFG		    0xFB
#define  CMD_GET_SYS_INTO				0xFD	//��ȡ��λ��������Ϣ(��ʱ����Ϣ���ӿ�����)
#define  CMD_DEBUG_INFO					0xFE    //������Ϣ
#define  VERSION_INFO					0xFF	//����汾��Ϣ

//-----------------------------------------------------------------------------
#define	BCC_OK					0
#define BCC_ERR					1

//------------------------------------------------------------------------------
#define FM175xx_DEBUG_OK            0x0
#define FM175xx_COMFRM_ERR          0x1        //��λ��ͨѶ����֡��ʽ����
#define FM175xx_COMBCC_ERR          0x2        //��λ��ͨѶ����֡��BCC����
#define FM175xx_REG_ERR             0x10       //�Ĵ�����д����
#define FM175xx_FIFO_ERR			  0x11       //FIFO����FIFOLevel��д��FIFO���ݳ��Ȳ�һ�µ�
#define FM175xx_CMD_ERR             0x20       //FM175xx Command�������
#define FM175xx_RECV_ERR            0x21       //FM175xx ���ݽ��մ���

typedef struct
{
	u8 Send_Len;
	u8 Send_Index;
	u8 Recv_Len;
	u8 Recv_Index; 
	u8 RecvStatus;			//����״̬��
	bool RecvOKFLAG;		//�������״̬
} UART_Com_Para_Def;

//******************* ARM �ܽŶ����� **************************
//FM175xx
//ע�⣺ALE�ܽűȽ����⣺Uartģʽʱ��FPGA����Ӧ�ý�FM175xx��ALE����USART2_Rx
//I2Cʱ��FM175xx��ALE����I2C_SDA(PA0); SPIʱ�����ӵ�SPI1_NSS��PA4��
//����ʱ�򣬰����涨�����ӣ�PA8)
#define GALE					GPIOC
#define ALE						GPIO_Pin_8			//PB8

#define GNRD					GPIOC
#define NRD						GPIO_Pin_11			//PB11

#define GNWR					GPIOC
#define NWR						GPIO_Pin_10			//PB10

#define GNCS					GPIOC
#define NCS						GPIO_Pin_9			//PC9

#define GIRQ					GPIOC
#define IRQ						GPIO_Pin_12			//PC12

#define GNRST					GPIOC
#define NRST					GPIO_Pin_15			//PB13

//FM175xx ADDR0~ADDR5	 ���ֽӿ�ģʽ�£�FPGA���ӹ�ϵ����
#define GADDR					GPIOC
#define A0						GPIO_Pin_11			//PA0
#define A1						GPIO_Pin_10			//PA1

//FM175xx DATA0~DATA7	 ���ֽӿ�ģʽ�£�FPGA���ӹ�ϵ��Ҫ�ı䣬ע�⣻
//����ģʽ�£�FM175xx����D0~D7
//I2C ģʽ�£�FM175xx��D0~D6����ARM��D0~D6��D7����ARM��I2C_SCL,��PA1
//Uartģʽ�£�FM175xx��D0~D6����ARM��D0~D6��D7����ARM��USART2_TX,��PA2
//SPI ģʽ�£�FM175xx��D0~D4����ARM��D0~D4��D5��ARM��SPI1_SCK(PA5),D6��MOSI(PA6),D7��MISO(PA7)
#define GDATA					GPIOC
#define D5						GPIO_Pin_5			//PC5	   //SPI_SCK   I2C_ADDR_1
#define D6						GPIO_Pin_6			//PC6	   //SPI_MOSI  I2C_ADDR_0
#define D4            GPIO_Pin_4                             //I2C_ADDR_2
//FM175xx I2C�ܽŽ�ڶ���
#define GI2C					GPIOB
#define I2C_SDA                 GPIO_Pin_7
#define I2C_SCL					GPIO_Pin_6

#define GFUNC					GPIOC
#define FUNC0					GPIO_Pin_13
#define FUNC1					GPIO_Pin_14


//SPI1 NSS (PA.04)��SCK(PA.05)��MISO(PA.06)��MOSI(PA.07) as alternate function push-pull  
#define GNSS					GPIOB
#define NSS_PIN					GPIO_Pin_12
#define	NSS_CLR					GNSS->BRR = NSS_PIN			//Clear NSS
#define	NSS_SET					GNSS->BSRR = NSS_PIN		//Set NSS to 1

//IO_SEL ������FPGA�л��������ӹ�ϵ
#define GIOSEL					  GPIOA
#define IOSEL0                    GPIO_Pin_7
#define IOSEL1					  GPIO_Pin_6

#define GNFCMODE					GPIOA
#define NFCMODE						GPIO_Pin_0

#define GLPCD					  GPIOA
#define LPCD                      GPIO_Pin_1

#define GBUZZ					 GPIOC
#define BUZZ                     GPIO_Pin_14
#define BUZZ_ON					(GBUZZ->BSRR = BUZZ)		//��������
#define BUZZ_OFF				(GBUZZ->BRR = BUZZ)			//�ط�����

//LED���ƣ�PD2
#define GLED                    GPIOD		   
#define LED0                    GPIO_Pin_7
#define LED1                    GPIO_Pin_6
#define LED2                    GPIO_Pin_5
#define LED3                    GPIO_Pin_4
#define LED4                    GPIO_Pin_3
#define LED5                    GPIO_Pin_2
#define LED6                    GPIO_Pin_1
#define LED7                    GPIO_Pin_0



#define LED_ARM_WORKING			LED0
#define LED_ARM_WORKING_OFF		(GLED->BSRR = LED_ARM_WORKING)			//LED0��  
#define LED_ARM_WORKING_ON		(GLED->BRR = LED_ARM_WORKING)			//LED0��



#define Enable_TIM1_IT			  TIM1_ITConfig(TIM1_IT_Update,ENABLE)	//ʹ��TIM1���ж�
#define Disable_TIM1_IT			  TIM1_ITConfig(TIM1_IT_Update,DISABLE)	//ʹ��TIM1���ж�

#define	 ON					  1
#define  OFF                  0

//-----------------------------------------------------------------------------

#endif
