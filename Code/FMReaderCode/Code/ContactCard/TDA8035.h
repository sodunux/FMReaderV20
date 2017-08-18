#ifndef __TDA8035_H
#define __TDA8035_H


#define SC_3_5V_PORT				GPIOD
#define SC_CLKDIV_2_PORT			GPIOD
#define SC_CLKDIV_1_PORT			GPIOD
#define SC_CMDVCC_PORT				GPIOD
#define SC_OFF_PORT					GPIOC
#define SC_RESET_PORT				GPIOC
#define SC_18V_PORT					GPIOC
#define SC_IO_PORT					GPIOB
#define SC_CLK_PORT					GPIOB

//Port D
#define SC_3_5V            GPIO_Pin_0   /* GPIOD Pin 0 */
#define SC_CLKDIV_2		   GPIO_Pin_1   /* GPIOD_Pin_1 */
#define SC_CLKDIV_1		   GPIO_Pin_2	  /* GPIOD_Pin_2 */
#define SC_CMDVCC          GPIO_Pin_3   /* GPIOD Pin 3 */

//Port C
#define SC_OFF   			GPIO_Pin_10  /* GPIOC Pin 10 */ 
#define SC_RESET 			GPIO_Pin_11  /* GPIOC Pin_11 */
#define SC_18V				GPIO_Pin_12	/* GPIOC_Pin_12 */

//Port B
#define SC_IO              GPIO_Pin_10  /* GPIOB Pin 10 */
#define SC_CLK             GPIO_Pin_12  /* GPIOB Pin 12 */

/* Smartcard Voltage */
#define SC_Voltage_5V      	0
#define SC_Voltage_3V      	1
#define SC_Voltage_18V		2 

void SC_VoltageConfig(u32 SC_Voltage);
void SC_PowerCmd(FunctionalState NewState);
void SC_Reset(BitAction ResetState);

void SC_Init(void);



#endif