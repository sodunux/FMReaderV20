#include "RegCtrl.h"
#include "I2C.h"

//***********************************************
//�������ƣ�Uart_Get_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��Uart�ӿڶ��Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:��ȡ��ֵ
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
uchar Uart_Get_Reg(uchar addr,uchar* regdata)
{
	u16  i;

	addr |= 0x80; 
	USART3->DR = (u16)addr;	
	for(i=0;i<UART_TIME_OUT;i++)
	{
		if(USART_GetITStatus(USART3, USART_IT_TC) != RESET)
			USART_ClearITPendingBit(USART3, USART_IT_TC);
	
		if(USART_GetFlagStatus(USART3, USART_FLAG_RXNE) != RESET)
		{
			*regdata = (u8)USART3->DR;//��������	
			USART_ClearFlag(USART3, USART_FLAG_RXNE);	
			return REG_OPT_SUCCESS;
		}
		uDelay(1);
	}
	
	return REG_OPT_NO_RESPONSE;
}

//***********************************************
//�������ƣ�Uart_Set_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��Uart�ӿ�д�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:Ҫд���ֵ
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//***********************************************
uchar Uart_Set_Reg(uchar addr,uchar regdata)
{
	uchar recvlen;
	u8 temp;
	u16 i;
	u8 	datasend;

	datasend = 0x0;
	recvlen = 0; 
	temp = 0;
	
	addr &= 0x3F;
	USART_ClearFlag(USART3, USART_FLAG_TC);
	USART3->DR = (u16)addr;
	
	for(i=0;i<UART_TIME_OUT;i++)
	{
		if((USART_GetFlagStatus(USART3, USART_FLAG_TC) != RESET ) && (datasend == 0))
		{
			USART_ClearFlag(USART3, USART_FLAG_TC);
			USART3->DR = (u16)regdata;
			datasend = 1;
			if(recvlen == 1)
				return REG_OPT_SUCCESS;
		}
		
		if(USART_GetFlagStatus(USART3, USART_FLAG_RXNE) != RESET)
		{
			temp = (u8)USART3->DR;	//�������صĵ�ַ����
			USART_ClearFlag(USART3, USART_FLAG_RXNE);
			
			if(temp == addr)
				recvlen = 1;
			else
				return REG_OPT_ERROR_RESPONSE;
		
			if(datasend == 1)
				return REG_OPT_SUCCESS;
		}
		uDelay(1);
	}				
	
	return REG_OPT_NO_RESPONSE;
}


//***********************************************
//�������ƣ�HS_I2C_Get_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��I2C�ӿڵ�High Speedģʽ���Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:��ȡ��ֵ
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
uchar HS_I2C_Get_Reg(uchar addr,uchar* regdata)
{
 	u8    temp = 0;

	I2C_START();
	
	I2C_SEND_DATA(I2C_MASTERCODE);		//MasterCode
	temp = I2C_RECV_ACK();				//NACK
//	if(temp != 0)
//	{
//		I2C_STOP();
//		return REG_OPT_I2C_MCODE_ERROR;
//	}
	
	I2C_START();					    //ReStart
	
	I2C_SEND_DATA_HS(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK_HS();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_SLV_ADDR_ERROR;
	}
	

	I2C_SEND_DATA_HS(addr&0x3F);		//Addr
	temp = I2C_RECV_ACK_HS();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_DATA_SEND_ERROR;
	}

	I2C_START();
	
	I2C_SEND_DATA_HS((I2C_ADDR<<1)+1);		//Addr
	temp = I2C_RECV_ACK_HS();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_SLV_ADDR_ERROR;
	}
	
	*regdata = I2C_RECV_DATA_HS();	
	I2C_SEND_ACK(1);		//NACK

	I2C_STOP();
	
	return REG_OPT_SUCCESS;
}

//***********************************************
//�������ƣ�HS_I2C_Set_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��I2C�ӿڵ�High Speedģʽд�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:Ҫд���ֵ
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//***********************************************
uchar HS_I2C_Set_Reg(uchar addr,uchar regdata)
{
	u8 temp = 0;
	
	I2C_START();

	I2C_SEND_DATA(I2C_MASTERCODE);		//MasterCode
	temp = I2C_RECV_ACK();				//NACK
//	if(temp != 0)
//	{
//		I2C_STOP();
//		return I2C_MASTERCODE;
//	}
	
	I2C_START();					    //ReStart

	I2C_SEND_DATA_HS(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK_HS();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_SLV_ADDR_ERROR;
	}

	I2C_SEND_DATA_HS(addr&0x3F);		//Addr
	temp = I2C_RECV_ACK_HS();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_DATA_SEND_ERROR;
	}

	I2C_SEND_DATA_HS(regdata);		//data
	temp = I2C_RECV_ACK_HS();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_DATA_SEND_ERROR;
	}

	I2C_STOP();
			
	return REG_OPT_SUCCESS;
}


//***********************************************
//�������ƣ�FS_I2C_Get_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��I2C�ӿڵ�Fast Speedģʽ���Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:��ȡ��ֵ
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
uchar FS_I2C_Get_Reg(uchar addr,uchar* regdata)
{
 	u8    temp;

	I2C_START();
	
	I2C_SEND_DATA(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_SLV_ADDR_ERROR;
	}
	
	I2C_SEND_DATA(addr&0x3F);		//Addr
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_DATA_SEND_ERROR;
	}

	I2C_START();

	I2C_SEND_DATA((I2C_ADDR<<1)+1);		//Addr
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_SLV_ADDR_ERROR;
	}
	
	*regdata = I2C_RECV_DATA();
	I2C_SEND_ACK(1);		//NACK

	I2C_STOP();
	
	return REG_OPT_SUCCESS;
}

//***********************************************
//�������ƣ�FS_I2C_Set_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��I2C�ӿڵ�Fast Speedģʽд�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:Ҫд���ֵ
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//***********************************************
uchar FS_I2C_Set_Reg(uchar addr,uchar regdata)
{
	u8 temp = 0;

	I2C_START();

	I2C_SEND_DATA(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_SLV_ADDR_ERROR;
	}
	
	I2C_SEND_DATA(addr&0x3F);		//Addr
	temp = I2C_RECV_ACK();	
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_DATA_SEND_ERROR;
	}

	I2C_SEND_DATA(regdata);		//data
	temp = I2C_RECV_ACK();
	
	if(temp != 0)
	{
		I2C_STOP();
		return REG_OPT_I2C_DATA_SEND_ERROR;
	}

	I2C_STOP();
	
	return REG_OPT_SUCCESS;
}


//***********************************************
//�������ƣ�SPI_Get_Reg(uchar addr,uchar* regdata)
//�������ܣ�ͨ��SPI�ӿڶ��Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:��ȡ��ֵ
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
uchar SPI_Get_Reg(uchar addr,uchar* regdata)
{	
	u16 i;

	NSS_CLR;
	__nop();__nop();
	
	addr = addr << 1;	 	   
	addr |= 0x80;
	SPI_SendData(SPI2,addr);   /* Send SPI1 data */ 
		
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_GetFlagStatus(SPI2, SPI_FLAG_RXNE) != RESET)
		{
			SPI_ClearFlag(SPI2,SPI_FLAG_RXNE);	
			*regdata = SPI_ReceiveData(SPI2);		 /* Read SPI1 received data */
			break;
		}
		uDelay(1);
	}
	if(i >= SPI_TIME_OUT)
	{
		NSS_SET;
		return REG_OPT_SPI_ERROR;
	}
	
	SPI_SendData(SPI2,0x00);   /* Send SPI1 data */
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_GetFlagStatus(SPI2, SPI_FLAG_RXNE) != RESET)
		{
			SPI_ClearFlag(SPI2,SPI_FLAG_RXNE);	
			*regdata = SPI_ReceiveData(SPI2);		 /* Read SPI1 received data */
			NSS_SET;
			return REG_OPT_SUCCESS;
		}
		uDelay(1);
	}
		
	NSS_SET;
	return REG_OPT_SPI_ERROR;
}

//***********************************************
//�������ƣ�SPI_Set_Reg(uchar addr,uchar* regdata)
//�������ܣ�д�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:Ҫд���ֵ
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//***********************************************
uchar SPI_Set_Reg(uchar addr,uchar regdata)
{
	u16 i;
	uchar temp = 0;
	
	NSS_CLR;
	__nop();__nop();
	
	addr = addr << 1;	 	   
	addr &= 0x7F;
	SPI_SendData(SPI2,addr);		/* Send SPI2 data */ 
	
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_GetFlagStatus(SPI2, SPI_FLAG_RXNE) != RESET)
		{
			SPI_ClearFlag(SPI2,SPI_FLAG_RXNE);	
			temp = SPI_ReceiveData(SPI2);		 /* Read SPI1 received data */
			break;
		}
		uDelay(1);
	}
	if(i >= SPI_TIME_OUT)
	{
		NSS_SET;
		return REG_OPT_SPI_ERROR;
	}
	
	SPI_SendData(SPI2,regdata);		/* Send SPI2 data */
	for(i=0;i<SPI_TIME_OUT;i++)
	{
		if(SPI_GetFlagStatus(SPI2, SPI_FLAG_RXNE) != RESET)
		{
			SPI_ClearFlag(SPI2,SPI_FLAG_RXNE);	
			temp = SPI_ReceiveData(SPI2);		 /* Read SPI1 received data */
			break;
		}
		uDelay(1);
	}
	if(i >= SPI_TIME_OUT)
	{
		NSS_SET;
		return REG_OPT_SPI_ERROR;
	}
	
	NSS_SET;
	return REG_OPT_SUCCESS;
}



//***********************************************
//�������ƣ�FM175xx_SetInterface(uchar interface)
//�������ܣ�����FM175xx�Ľӿ����ͣ���ARM����
//��ڲ������ӿ����ͣ�FM175xx_INTERFACE
//���ڲ�����N/A
//***********************************************
void  FM175xx_SetInterface(uchar intf)
{	
	GPIO_ResetBits(GNRST,NRST);					//NRST����Ϊ0,FM175xx���ڸ�λ״̬
	switch(intf)
	{
		case 1:		//uart
			GPIO_ResetBits(GADDR,A1|A0);	//PA5~PA0 000000
			break;
		case 2:		//I2C
			GPIO_SetBits(GADDR,A1);		//PA1 1
			GPIO_ResetBits(GADDR,A0);
											//EA=0, bit6~bit3=0101 bit2~bit0=(ADR_0, ADR_1, ADR_2)
											//EA=1, ADR_0 to ADR_6 = D6~D0	   A0��ΪI2C��EA
			GPIO_ResetBits(GDATA,D5|D6|D4);	//D6~D5 2'b00
			if(I2C_ADDR&0x02)
				GPIO_SetBits(GDATA,D5);
			if(I2C_ADDR&0x01)
				GPIO_SetBits(GDATA,D6);								
			if(I2C_ADDR&0x04)
				GPIO_SetBits(GDATA,D4);
			
			GPIO_SetBits(GI2C,I2C_SDA);					//
			GPIO_SetBits(GI2C,I2C_SCL);					//
			break;
		case 0x20:								//I2C EA = 0
			GPIO_SetBits(GIOSEL,IOSEL0);
		
			GPIO_ResetBits(GADDR,A0);			//PA5~PA2 0000
			GPIO_SetBits(GADDR,A1);				//PA1 1
												//EA=0, bit6~bit3=0101 bit2~bit0=(ADR_0, ADR_1, ADR_2)
												//EA=1, ADR_0 to ADR_6 = D6~D0	   A0��ΪI2C��EA
			GPIO_ResetBits(GDATA,D5|D6);		//D6~D5 2'b00

			if(I2C_ADDR&0x02)
				GPIO_SetBits(GDATA,D5);
			if(I2C_ADDR&0x01)
				GPIO_SetBits(GDATA,D6);								

			GPIO_SetBits(GI2C,I2C_SDA);
			GPIO_SetBits(GI2C,I2C_SCL);
			break;
		case 3:		//SPI			
			GPIO_ResetBits(GADDR,A1);		//PA5~PA1 000000
			GPIO_SetBits(GADDR,A0);			//PA0 1
			
			NSS_SET;						//NSS
			break;
		default:	//uart
			break;
	}
	GPIO_SetBits(GNRST,NRST);				//NRST����Ϊ1,FM175xx��λ�ſ�
	mDelay(2);								//NRST�ſ���Լ1ms���ң�оƬ�����ȶ�.�ڴ�֮ǰ��Ҫ�����Ĵ���.
}


//***********************************************
//�������ƣ�GetReg(uchar addr,uchar* regdata)
//�������ܣ���ȡ�Ĵ���ֵ
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:��ȡ��ֵ
//���ڲ�����uchar  TRUE����ȡ�ɹ�   FALSE:ʧ��
//***********************************************
uchar GetReg(uchar addr,uchar* regdata)
{
	uchar res;

	switch(FM175xx_INTERFACE)
	{
		case 1:		//uart
			res = Uart_Get_Reg(addr,regdata);
			break;
		case 2:		//I2C
			if(I2C_MODE == I2C_HSMode)
			{
				res = HS_I2C_Get_Reg(addr,regdata);
			}
			else
			{
				res =FS_I2C_Get_Reg(addr,regdata);
			}
			break;
		case 3:		//SPI
   			res = SPI_Get_Reg(addr,regdata);
			break;
		default:	//uart
			break;
	}
	if(res == REG_OPT_SUCCESS)
		return true;
	else
		return false;
}

//***********************************************
//�������ƣ�SetReg(uchar addr,uchar* regdata)
//�������ܣ�д�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   regdata:Ҫд���ֵ
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//***********************************************
uchar SetReg(uchar addr,uchar regdata)
{
	uchar res;
	
	switch(FM175xx_INTERFACE)
	{
		case 1:		//uart
			res = Uart_Set_Reg(addr,regdata);				
			break;
		case 2:		//I2C
			if(I2C_MODE == I2C_HSMode)
			{
				res = HS_I2C_Set_Reg(addr,regdata);	
			}
			else
			{
				res = FS_I2C_Set_Reg(addr,regdata);	
			}
			break;
		case 3:		//SPI
			res = SPI_Set_Reg(addr,regdata);	
			break;
		default:	//uart
			break;
	}

	if(res == REG_OPT_SUCCESS)
		return true;
	else
		return false;
}

//*******************************************************
//�������ƣ�ModifyReg(uchar addr,uchar* mask,uchar set)
//�������ܣ�д�Ĵ���
//��ڲ�����addr:Ŀ��Ĵ�����ַ   mask:Ҫ�ı��λ  
//         set:  0:��־��λ����   ����:��־��λ����
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//********************************************************
uchar ModifyReg(uchar addr,uchar mask,uchar set)
{
	uchar res;
	uchar regdata;
	
	res = GetReg(addr,&regdata);
	if(res == true)
	{
		if(set)
		{
			regdata |= mask;
		}
		else
		{
			regdata &= ~mask;
		}
	}
	else
		return res;

	res = SetReg(addr,regdata);
	return res;
}

//*******************************************************
//�������ƣ�SPI_Write_FIFO
//�������ܣ�ͨ��SPI�ӿ�дFIFO
//��ڲ�����fflen:д�볤�ȣ�
//         ffbuf:���ݻ�������
//���ڲ�����uchar  TRUE��д�ɹ�   FALSE:дʧ��
//********************************************************
u8 SPI_Write_FIFO(u8 fflen,u8* ffbuf)  //SPI�ӿ�����дFIFO ����д�ĵ�ַ�̶�����ˣ�ֻ����дFIFO
{
	u8  i;
	u8 regdata;
	u16 j;

	NSS_CLR;
	SPI_SendData(SPI2,0x12);   /* Send FIFO addr 0x09<<1 */ 
	while(SPI_GetFlagStatus(SPI2, SPI_FLAG_RXNE)==RESET);
	regdata = SPI_ReceiveData(SPI2);		 /* not care data */
	for(i=0;i<fflen;i++)
	{
		regdata = *(ffbuf+i);	  //RegData_i
		SPI_SendData(SPI2,regdata);   /* Send addr_i i��1*/
		
		for(j=0;j<SPI_TIME_OUT;j++)
		{
			if(SPI_GetFlagStatus(SPI2, SPI_FLAG_RXNE) != RESET)
			{
				SPI_ClearFlag(SPI2,SPI_FLAG_RXNE);	
				SPI_ReceiveData(SPI2);		 /* Read SPI1 received data */
				break;
			}
		}
		if(j >= SPI_TIME_OUT)
		{
			NSS_SET;
			return REG_OPT_SPI_ERROR;
		}
		
    	regdata = SPI_ReceiveData(SPI2);		 /* not care data */
	}
	
	NSS_SET;
	return true;
}














