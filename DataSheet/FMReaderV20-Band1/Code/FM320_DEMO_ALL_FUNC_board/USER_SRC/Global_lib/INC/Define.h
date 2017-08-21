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
/* 	ARM所用变量宏定义													 */
/* 	主要功能:														 */
/* 		1.程序中产量定义											 */
/* 	编制:赵清泉														 */
/* 	编制时间:2012年5月16日											 */
/* 																	 */
/*********************************************************************/

#ifndef _DEFINE_H_
#define _DEFINE_H_


/* Includes ------------------------------------------------------------------*/
#include "stm32f10x_map.h"

//<<< Use Configuration Wizard in Context Menu >>>
// <h> 初始化选项

//     <e0> USART1使能选项	
//	   </e>	  <i>"USB转UART，用于控制ARM"	
//     <e1> USART3使能选项  <i>"UART3，用于控制FM175xx"
//	   </e> 
//     <e2> I2C使能选项  <i>"I2C，用于控制FM175xx"
//	   </e>

//     <e3>    SPI2使能选项
//	   </e>

//     <e4>    USB使能选项
//	   </e>

//     <o5.0..18> UART1 波特率选择  <9600=>9600
//			      <19200=>19200               <38400=>38400
//			      <57600=>57600               <115200=>115200

//     <o6.0..18> UART2 波特率选择  <9600=>9600
//			      <19200=>19200               <38400=>38400
//			      <57600=>57600               <115200=>115200

//     <o7.0..7> FM175xx接口方式选择  <1=>UART接口
//			      <2=>I2C           <3=>SPI接口
//			      <4=>R/W线分开Data/Addr分开       <5=>R/W线分开Data/Addr复用
//			      <6=>R/W线复用Data/Addr分开       <7=>R/W线复用Data/Addr复用

//     <e8>FM175xx Uart透传模式
//	   </e>

//	   <S0.60>版本信息<i>"串口吐出的信息"
//	   <S1.60>硬件版本信息
// </h>	

// <h> FM175xx IRQ使能选项
//     <e9.7> IRQ INV使能与否，如果勾选，IRQ脚将与IRQ寄存器位相反	
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
//     <e10.7>IRQPushPul;如果勾选，IRQ为CMOS输出，否则，为开漏输出	
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

// <h> FM175xx 功能选项
//     <e11.7> ReaderA功能选项(bit7)	
//	   </e>
//     <e11.6> ReaderB功能选项(bit6)	
//	   </e>
//     <e11.5> ReaderF功能选项(bit5)	
//	   </e>
//     <e11.4> CardA功能选项(bit4)	
//	   </e>
//     <e11.3> CardF功能选项(bit3)	
//	   </e>
//     <e11.2> S2C_外卡功能选项(bit2)	
//	   </e>
//     <e11.1> S2C_内卡功能选项(bit1)	
//	   </e>
//     <e11.0> LPCD功能选项(bit0)	
//	   </e>

// </h>

// <h> FM175xx 测试选项
//     <e12.7> LPCD测试调校选项(bit7)	
//	   </e>
//     <e12.6> ReaderB功能选项(bit6)	
//	   </e>
//     <e12.5> ReaderF功能选项(bit5)	
//	   </e>
//     <e12.4> CardA功能选项(bit4)	
//	   </e>
//     <e12.3> CardF功能选项(bit3)	
//	   </e>
//     <e12.2> S2C_外卡功能选项(bit2)	
//	   </e>
//     <e12.1> S2C_内卡功能选项(bit1)	
//	   </e>
//     <e11.0> LPCD功能选项(bit0)	
//	   </e>
// </h>

//     <o13.0..7> FM175xx 封装形式  <02=>QFN48封装
//			      <01=>QFN40封装              <00=>QFN32封装

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


//     <o7.0..7> FM175xx接口方式选择  <1=>UART接口
//			      <2=>I2C           <3=>SPI接口
//			      <4=>R/W线分开Data/Addr分开       <5=>R/W线分开Data/Addr复用
//			      <6=>R/W线复用Data/Addr分开       <7=>R/W线复用Data/Addr复用

#define FM175xx_INTF_UART		0x01
#define FM175xx_INTF_I2C		0x02               
#define FM175xx_INTF_SPI		0x03
#define FM175xx_INTF_PAR1		0x04
#define FM175xx_INTF_PAR2		0x05
#define FM175xx_INTF_PAR3		0x06
#define FM175xx_INTF_PAR4		0x07

//;帧的常数
//通信字节码定义
#define 	STX 			0x02		//开始字符
#define 	ETX 			0x03		//结束字符字符
//#define 	DLE				0x10		//握手字符

//#define DEBOULE_FRE

#ifndef true
	#define true				TRUE	//真
#endif
#ifndef false
	#define false				FALSE	//假
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

#define UART1_BUF_LEN		0x0400	   //UART1 BUFFER 长度
#define UART2_BUF_LEN		0x80	   //UART1 BUFFER 长度

#define DEBUG_BUF_LEN		0x80		//Debug信息区长度

#define FM175xx_FUNC_CARD     0x10 	   //TypeA 卡功能
#define FM175xx_FUNC_ES2CA    0x12	   //S2CA 外卡功能
#define FM175xx_FUNC_IS2CA    0x13	   //S2CA 内卡功能
#define FM175xx_FUNC_READERA  0x20	   //TypeA Reader功能
#define FM175xx_FUNC_READERB  0x21	   //TypeB Reader功能
#define FM175xx_FUNC_READERF  0x22	   //TypeF Reader功能

#define FM175xx_Card_Buf_Len  0x80	   //卡数据缓冲区长度定义

//******************* 通信命令码定义********************
#define	 USUCCESS                       0x01    //操作成功的回复
#define  UERROR							0x02	//错做失败的回复
//0x00~0x0F 用于主控芯片的设置
#define  CMD_MCU_CHG_UART_BAUD			0x08	//改变MCU的Uart通讯波特率

//0x10~0x1F 用于接口以及接口属性切换
#define  CMD_FM175xx_CHG_INTF				0x10	//切换接口类型
#define  CMD_FM175xx_CHG_UART_BAUD		0x11	//切换UART通讯速率(ARM端)
#define  CMD_FM175xx_CHG_SPI_BAUD		    0x12	//切换SPI通讯速率 (ARM端)
#define  CMD_FM175xx_CHG_I2C_BAUD		    0x13	//切换I2C通讯速率 (ARM端)
#define  CMD_FM175xx_CHG_I2C_ADDR		    0x14	//切换I2C地址 (ARM端)
#define  CMD_ARM_CHG_TIMER_GATE			0x15	//切换Timer的Gate信号值

//0x20~0x2F LPCD相关
#define  CMD_FM175xx_NRSTPD_CTRL          0x20    //NRSTPD控制
#define  CMD_FM175xx_ENTER_LPCD           0x21    //进入LPCD模式
#define  CMD_FM175xx_LPCD_AutoWakeCtrl    0x22    //启动AutoWake测试:
												//这里仅仅是配置AutoWakeUp值，并将NRSTPD拉低，Timer等配置，请上位机进行
#define  CMD_FM175xx_Get_AutoWake_Timer   0x23    //读取AutoWakeUp计数值

//0x50~0x5F 用于寄存器操作
#define  CMD_RD_REG      				0x51    //读xdata
#define  CMD_WR_REG      				0x53    //写xdata
#define  CMD_I2C_RD_FIFO				0x54	//I2C接口读FIFO
#define  CMD_I2C_WR_FIFO				0x55	//I2C接口写FIFO
#define  CMD_SPI_WR_FIFO				0x57	//SPI接口写FIFO
#define  CMD_RD_FIFO				    0x58	//读FIFO
#define  CMD_WR_FIFO				    0x59	//写FIFO

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

#define  CMD_READER_TRANSCEIVE          0x80    //启动Reader发送然后接收数据

#define  CMD_AD_DATA_TRANSCEIVE              0x90              //ADC data tranceive
#define  CMD_AD_DATA_RETRANSCEIVE              0x91
#define  CMD_AD_DATA_READY                     0x92

#define  CMD_SYS_HANDON					0xF0	//握手命令
#define  CMD_SYS_HARD_INFO				0xF1	//硬件版本信息
#define  CMD_SYS_GET_CFG				0xF2	//配置信息：硬件接口，速率；
#define  CMD_CFG_SET_CARD_UID			0xF6
#define  CMD_CFG_GET_CARD_UID			0xF7
#define  CMD_CFG_SET_USERIFO			0xF8
#define  CMD_CFG_GET_USERIFO			0xF9
#define  CMD_CFG_SET_LPCD_CFG		    0xFA
#define  CMD_CFG_GET_LPCD_CFG		    0xFB
#define  CMD_GET_SYS_INTO				0xFD	//获取下位机配置信息(如时钟信息，接口类型)
#define  CMD_DEBUG_INFO					0xFE    //调试信息
#define  VERSION_INFO					0xFF	//软件版本信息

//-----------------------------------------------------------------------------
#define	BCC_OK					0
#define BCC_ERR					1

//------------------------------------------------------------------------------
#define FM175xx_DEBUG_OK            0x0
#define FM175xx_COMFRM_ERR          0x1        //上位机通讯命令帧格式错误
#define FM175xx_COMBCC_ERR          0x2        //上位机通讯命令帧的BCC错误
#define FM175xx_REG_ERR             0x10       //寄存器读写错误
#define FM175xx_FIFO_ERR			  0x11       //FIFO错误：FIFOLevel与写入FIFO数据长度不一致等
#define FM175xx_CMD_ERR             0x20       //FM175xx Command命令错误
#define FM175xx_RECV_ERR            0x21       //FM175xx 数据接收错误

typedef struct
{
	u8 Send_Len;
	u8 Send_Index;
	u8 Recv_Len;
	u8 Recv_Index; 
	u8 RecvStatus;			//接收状态字
	bool RecvOKFLAG;		//接收完毕状态
} UART_Com_Para_Def;

//******************* ARM 管脚定义区 **************************
//FM175xx
//注意：ALE管脚比较特殊：Uart模式时，FPGA顶层应该将FM175xx的ALE连到USART2_Rx
//I2C时，FM175xx的ALE连到I2C_SDA(PA0); SPI时，连接到SPI1_NSS（PA4）
//并口时候，按下面定义连接（PA8)
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

//FM175xx ADDR0~ADDR5	 各种接口模式下，FPGA连接关系不变
#define GADDR					GPIOC
#define A0						GPIO_Pin_11			//PA0
#define A1						GPIO_Pin_10			//PA1

//FM175xx DATA0~DATA7	 各种接口模式下，FPGA连接关系需要改变，注意；
//并口模式下：FM175xx连接D0~D7
//I2C 模式下：FM175xx的D0~D6连接ARM的D0~D6，D7连接ARM的I2C_SCL,即PA1
//Uart模式下：FM175xx的D0~D6连接ARM的D0~D6，D7连接ARM的USART2_TX,即PA2
//SPI 模式下：FM175xx的D0~D4连接ARM的D0~D4，D5连ARM的SPI1_SCK(PA5),D6连MOSI(PA6),D7连MISO(PA7)
#define GDATA					GPIOC
#define D5						GPIO_Pin_5			//PC5	   //SPI_SCK   I2C_ADDR_1
#define D6						GPIO_Pin_6			//PC6	   //SPI_MOSI  I2C_ADDR_0
#define D4            GPIO_Pin_4                             //I2C_ADDR_2
//FM175xx I2C管脚借口定义
#define GI2C					GPIOB
#define I2C_SDA                 GPIO_Pin_7
#define I2C_SCL					GPIO_Pin_6

#define GFUNC					GPIOC
#define FUNC0					GPIO_Pin_13
#define FUNC1					GPIO_Pin_14


//SPI1 NSS (PA.04)、SCK(PA.05)、MISO(PA.06)、MOSI(PA.07) as alternate function push-pull  
#define GNSS					GPIOB
#define NSS_PIN					GPIO_Pin_12
#define	NSS_CLR					GNSS->BRR = NSS_PIN			//Clear NSS
#define	NSS_SET					GNSS->BSRR = NSS_PIN		//Set NSS to 1

//IO_SEL ：用于FPGA切换顶层连接关系
#define GIOSEL					  GPIOA
#define IOSEL0                    GPIO_Pin_7
#define IOSEL1					  GPIO_Pin_6

#define GNFCMODE					GPIOA
#define NFCMODE						GPIO_Pin_0

#define GLPCD					  GPIOA
#define LPCD                      GPIO_Pin_1

#define GBUZZ					 GPIOC
#define BUZZ                     GPIO_Pin_14
#define BUZZ_ON					(GBUZZ->BSRR = BUZZ)		//开蜂鸣器
#define BUZZ_OFF				(GBUZZ->BRR = BUZZ)			//关蜂鸣器

//LED控制：PD2
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
#define LED_ARM_WORKING_OFF		(GLED->BSRR = LED_ARM_WORKING)			//LED0亮  
#define LED_ARM_WORKING_ON		(GLED->BRR = LED_ARM_WORKING)			//LED0灭



#define Enable_TIM1_IT			  TIM1_ITConfig(TIM1_IT_Update,ENABLE)	//使能TIM1总中断
#define Disable_TIM1_IT			  TIM1_ITConfig(TIM1_IT_Update,DISABLE)	//使能TIM1总中断

#define	 ON					  1
#define  OFF                  0

//-----------------------------------------------------------------------------

#endif
