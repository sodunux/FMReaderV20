
#ifndef __OLED_H
#define __OLED_H			  	 
//#include "sys.h"
#include "stdlib.h"	  
#include "STM32F10x_TYPE.H"
//////////////////////////////////////////////////////////////////////////////////	 
//imodule	
//////////////////////////////////////////////////////////////////////////////////	

//OLED模式设置
//0:4线串行模式
//1:并行8080模式
#define OLED_MODE 0

#define STRING_SIZE_1206	12
#define STRING_SIZE_1608	16
#define STING_SIZE			STRING_SIZE_1206
		    						  
//-----------------OLED端口定义----------------  					   

//RST
#define OLED_RST_Clr() GPIO_ResetBits(GPIOE,GPIO_Pin_2)
#define OLED_RST_Set() GPIO_SetBits(GPIOE,GPIO_Pin_2)
//DC
#define OLED_RS_Clr() GPIO_ResetBits(GPIOE,GPIO_Pin_3)
#define OLED_RS_Set() GPIO_SetBits(GPIOE,GPIO_Pin_3)
//DO
#define OLED_SCLK_Clr() GPIO_ResetBits(GPIOE,GPIO_Pin_0)
#define OLED_SCLK_Set() GPIO_SetBits(GPIOE,GPIO_Pin_0)
//DI
#define OLED_SDIN_Clr() GPIO_ResetBits(GPIOE,GPIO_Pin_1)
#define OLED_SDIN_Set() GPIO_SetBits(GPIOE,GPIO_Pin_1)

#define OLED_CS_Clr()	GPIO_ResetBits(GPIOE,GPIO_Pin_4)		//目前悬空，有问题
#define OLED_CS_Set()	GPIO_SetBits(GPIOE,GPIO_Pin_4)

 		     
#define OLED_CMD  0	//写命令
#define OLED_DATA 1	//写数据
//OLED控制用函数
void OLED_WR_Byte(u8 dat,u8 cmd);	    
void OLED_Display_On(void);
void OLED_Display_Off(void);
void OLED_Refresh_Gram(void);		   
							   		    
void OLED_Init(void);
void OLED_Clear(void);
void OLED_DrawPoint(u8 x,u8 y,u8 t);
void OLED_Fill(u8 x1,u8 y1,u8 x2,u8 y2,u8 dot);
void OLED_ShowChar(u8 x,u8 y,u8 chr,u8 size,u8 mode);
void OLED_ShowNum(u8 x,u8 y,u32 num,u8 len,u8 size);
void OLED_ShowString(u8 x,u8 y,const u8 *p);	
void ShowLogo(void);

#endif  
	 



