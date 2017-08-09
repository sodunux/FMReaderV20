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
/* 	Uart通讯相关函数											     */
/* 	主要功能:														 */
/* 		1.实现Uart相关通讯											 */
/* 	编制:赵清泉														 */
/* 	编制时间:2012年5月24日											 */
/* 																	 */
/*********************************************************************/
#ifndef _UART_H_
#define _UART_H_

#include "Define.h"

extern void Uart_Proc(void);
extern void SendMsgWelcome(void);
extern void ProFrame(u8 nFrmType);

extern void Uart2_Start_Send(void);
extern void Uart2Frame(u8 nFrmType);
#endif

