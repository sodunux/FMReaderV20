#include "stm32f10x_it.h"
#include "define.h"
#include "global.h"
#include "I2C.h"

//10    606kHz
//18    393kHz
uchar SDA_DELAY = 1;		 //SDA数据保持时间
uchar SCL_DELAY = 1;		 //SCL沿变之后，延时SCL_DELAY，SDA再变
uchar STA_DELAY = 1;         //SDA在产生STA时序后，SCL保持STA_DELAY
uchar STO_DELAY = 1;		 //SDA在产生STO时序后，SCL保持STA_DELAY

uchar I2C_MODE = I2C_FSMode;
uchar I2C_ADDR = I2C_ADDR_DEF;

/****************************************************************
 * 函数名：SET_SDA_IN											
 * 功能说明：将SDA脚设为输入状态			
 * 入口参数：N/A				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
void SET_SDA_IN(void)   
{ 
//  u32 currentmode = 0x00, pinpos = 0x00, pos = 0x00;
//  u32 tmpreg = 0x00, pinmask = 0x00;;
  u32 tmpreg = 0x00;    
//  GPIO_InitStructure.GPIO_Pin =  I2C_SDA;   
//  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU;   	  //FPGA板没有接上拉
//  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;   
//  GPIO_Init(GI2C, &GPIO_InitStructure); 

  /*---------------------------- GPIO CRL Configuration ------------------------*/
  /* Configure the eight low port pins */
     //currentmode = GPIO_Mode_IPU & ((u32)0x0F);
	 tmpreg = GI2C->CRL;

	 //pinpos = I2C_SDA;

     //pos = pinpos << 2;
     /* Clear the corresponding low control register bits */
     //pinmask = ((u32)0x0F) << pos;
     //tmpreg &= ~pinmask;

     /* Write the mode configuration in the corresponding bits */
     //tmpreg |= (currentmode << pos);

    

	tmpreg &= 0x0FFFFFFF;
	tmpreg |= 0x40000000;
	//GI2C->BSRR = I2C_SDA;	   //上拉
    GI2C->CRL = tmpreg;
	//GI2C->BSRR = I2C_SDA;	   //上拉
} 

/****************************************************************
 * 函数名：SET_SDA_OUT											
 * 功能说明：将SDA脚设为输出状态			
 * 入口参数：N/A				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
void SET_SDA_OUT(void)   
{   
//  GPIO_InitStructure.GPIO_Pin =  I2C_SDA;   
//  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;     //FPGA板没有接上拉
//  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;   
//  GPIO_Init(GI2C, &GPIO_InitStructure); 
 
 //  u32 currentmode = 0x00, pinpos = 0x00, pos = 0x00;
 //  u32 tmpreg = 0x00, pinmask = 0x00;
  u32 tmpreg = 0x00;    

  /*---------------------------- GPIO CRL Configuration ------------------------*/
  /* Configure the eight low port pins */
     //currentmode = GPIO_Mode_IPU & ((u32)0x0F);
	 tmpreg = GI2C->CRL;

	 //pinpos = I2C_SDA;

     //pos = pinpos << 2;
     /* Clear the corresponding low control register bits */
     //pinmask = ((u32)0x0F) << pos;
     //tmpreg &= ~pinmask;

     /* Write the mode configuration in the corresponding bits */
     //tmpreg |= (currentmode << pos);

	tmpreg &= 0x0FFFFFFF;
	tmpreg |= 0x70000000;
    GI2C->CRL = tmpreg; 
 	//GI2C->BSRR = I2C_SDA;	   //上拉
}
 
/****************************************************************
 * 函数名：I2C_START											
 * 功能说明：发送Start命令			
 * 入口参数：N/A				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
void I2C_START(void)
{
	u16 i;
	I2C_SCL_OFF;			//SCL = 0;
	I2C_SDA_ON;				//SDA = 1
	I2C_SCL_ON;				//SCL = 1;
	for(i=SCL_DELAY;i>0;i--);
	I2C_SDA_OFF;		    //SDA = 0;
	for(i=STA_DELAY;i>0;i--);
}
/****************************************************************
 * 函数名：I2C_START_HS											
 * 功能说明：发送Start命令			
 * 入口参数：N/A				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
void I2C_START_HS(void)
{
	I2C_SCL_OFF;	//SCL = 0;
	I2C_SDA_ON;		//SDA = 1
	I2C_SCL_ON;		//SCL = 1;
	__nop();__nop();
	I2C_SDA_OFF;	//SDA = 0;
	__nop();__nop();
}

/****************************************************************
 * 函数名：I2C_STOP											
 * 功能说明：发送Stop命令			
 * 入口参数：N/A				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
 void I2C_STOP(void)
{
	u16 i;

	I2C_SCL_OFF;
	I2C_SDA_OFF;
	I2C_SCL_ON;
	for(i=SCL_DELAY;i>0;i--);
	I2C_SDA_ON;
	for(i=STO_DELAY;i>0;i--);

}

/****************************************************************
 * 函数名：I2C_SEND_DATA											
 * 功能说明：I2C发送一个数据			
 * 入口参数：i2cdata：要发送的数据				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
void I2C_SEND_DATA(u8 i2cdata)
{
	u16 i;
	u16 j;
	u8 temp;

	for (i = 0; i < 8; i++)		// 8 位数据，High Bit First；
	{
     	I2C_SCL_OFF;
		for(j= SCL_DELAY;j>0;j--);
		temp = i2cdata & 0x80;
		if(temp == 0)
			I2C_SDA_OFF;
		else
			I2C_SDA_ON;
        i2cdata = i2cdata << 1;
        I2C_SCL_ON;
		for(j= SDA_DELAY;j>0;j--);
    }

	I2C_SCL_OFF;
	SET_SDA_IN();				//FM175xx I2C在SCL的下降沿开始回发ACK...
	//I2C_SDA_ON;				    //SDA SCL 复位；
}

void I2C_SEND_DATA_HS(u8 i2cdata)
{
	u8 bit0,bit1,bit2,bit3,bit4,bit5,bit6,bit7;

	bit7 = i2cdata & 0x80;
    I2C_SCL_OFF;
	if(bit7 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
    I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();	

	bit6 = i2cdata & 0x40;
	I2C_SCL_OFF;
	if(bit6 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
    I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	bit5 = i2cdata & 0x20;
	I2C_SCL_OFF;
	if(bit5 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
			
	I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	bit4 = i2cdata & 0x10;
	I2C_SCL_OFF;
	if(bit4 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
	I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	bit3 = i2cdata & 0x08;
	I2C_SCL_OFF;
	if(bit3 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
	I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	bit2 = i2cdata & 0x04;
	I2C_SCL_OFF;
	if(bit2 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
	I2C_SCL_ON;
    __nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	bit1 = i2cdata & 0x02;
	I2C_SCL_OFF;
	if(bit1 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
	I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	bit0 = i2cdata & 0x01;
	I2C_SCL_OFF;
	if(bit0 == 0)
			I2C_SDA_OFF;
	else
			I2C_SDA_ON;
	I2C_SCL_ON;
	__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();

	I2C_SCL_OFF;
	SET_SDA_IN();				//FM175xx I2C在SCL的下降沿开始回发ACK...
	//I2C_SDA_ON;				    //SDA SCL 复位；	
}

/****************************************************************
 * 函数名：I2C_RECV_ACK											
 * 功能说明：I2C接收ACK数据			
 * 入口参数：N/A				
 * 出口参数：ack:0/1						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
u8 I2C_RECV_ACK(void)
{
	u16 i;
	u8 ack;

//	I2C_SCL_OFF;
//	I2C_SDA_ON;
	
	//SET_SDA_IN();

	for(i= SCL_DELAY;i>0;i--);	 //留给Slave响应时间
	I2C_SCL_ON;
	for(i= SDA_DELAY;i>0;i--);	 
	ack = Get_SDA_Status;
	
	I2C_SCL_OFF;
	SET_SDA_OUT();
	//I2C_SDA_ON;
	return ack;
}

u8 I2C_RECV_ACK_HS(void)
{
	u8 ack;

//	I2C_SCL_OFF;
//	I2C_SDA_ON;
	
	//SET_SDA_IN();

	I2C_SCL_ON;
	__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();	 
	ack = Get_SDA_Status;
	
	I2C_SCL_OFF;
	//I2C_SDA_ON;
	SET_SDA_OUT();

	return ack;
}

/****************************************************************
 * 函数名：I2C_RECV_DATA											
 * 功能说明：I2C接收数据			
 * 入口参数：N/A				
 * 出口参数：i2c_data						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
u8 I2C_RECV_DATA(void)
{
	u16 i;
	u16 j;
	u8 i2cdata;
	u8 dsda;

	I2C_SCL_OFF;
	SET_SDA_IN();
	//I2C_SDA_ON;
	for(i= SCL_DELAY;i>0;i--);
	i2cdata = 0;
	for(i=8;i>0;i--)
	{	
		i2cdata = i2cdata <<1;
		I2C_SCL_ON;
		for(j= SDA_DELAY;j>0;j--);
		dsda = Get_SDA_Status;
		i2cdata += dsda;
		I2C_SCL_OFF;
		for(j= SCL_DELAY;j>0;j--); 
	}
	SET_SDA_OUT();
	return i2cdata;
}

u8 I2C_RECV_DATA_HS(void)
{
	u8 i2cdata;
	u8 bit0,bit1,bit2,bit3,bit4,bit5,bit6,bit7;

	I2C_SCL_OFF;
	SET_SDA_IN();
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();	
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit7 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();//__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit6 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit5 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit4 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit3 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit2 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit1 = Get_SDA_Status;

	I2C_SCL_OFF;
	__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	//__nop();__nop();__nop();__nop();__nop();__nop();__nop();__nop();
	I2C_SCL_ON;
	//__nop();__nop();__nop();__nop();__nop();__nop();
	bit0 = Get_SDA_Status;
	I2C_SCL_OFF;

	SET_SDA_OUT();
	i2cdata = (bit7<<7) + (bit6<<6)	+ (bit5<<5) + (bit4<<4) + (bit3<<3) + (bit2<<2) + (bit1<<1) + (bit0);
	return i2cdata;
}

/****************************************************************
 * 函数名：I2C_SEND_ACK											
 * 功能说明：I2C发送ACK响应			
 * 入口参数：ack:0/1				
 * 出口参数：N/A						
 * 时间: 2012 6 14														
 * 作者: 赵清泉														
 ****************************************************************/
void I2C_SEND_ACK(u8 ack)
{
	u16 i;

	I2C_SCL_OFF;
	for(i= SCL_DELAY;i>0;i--);
	if(ack == 0)
		I2C_SDA_OFF;
	else
		I2C_SDA_ON;

	for(i= SDA_DELAY;i>0;i--);
	I2C_SCL_ON;
}

u8 I2C_Write_FIFO(u8* buflen,u8* dstbuf)  //I2C接口连续写FIFO
{
	u8 temp;
	u8 i;

	I2C_START();
	I2C_SEND_DATA(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x1;
	}
	I2C_SEND_DATA(0x09);		   //FIFO Addr
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x2;
	}

	for(i=0;i<*buflen;i++)
	{
		I2C_SEND_DATA(*(dstbuf+i));		//data
		temp = I2C_RECV_ACK();
		if(temp != 0)
		{
			I2C_STOP();
			return 0x3;
		}
	}

	I2C_STOP();

	return true;
}
u8 I2C_Read_FIFO(u8* fflen,u8* ffbuf)	  //I2C接口连续读FIFO
{
 	u8  temp;
	u8  i;

	I2C_START();
	I2C_SEND_DATA(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x1;
	}
	I2C_SEND_DATA(0x0A);		//FIFO Len
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x2;
	}
	I2C_STOP();

	I2C_START();
	I2C_SEND_DATA((I2C_ADDR<<1)+1);		//Addr+R
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x3;
	}

	*fflen = I2C_RECV_DATA();		
	I2C_SEND_ACK(1);		//NACK

	I2C_STOP();

	I2C_START();
	I2C_SEND_DATA(I2C_ADDR<<1);		//Addr+W
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x11;
	}
	I2C_SEND_DATA(0x09);		//FIFO Len
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x12;
	}
	I2C_STOP();

	I2C_START();
	I2C_SEND_DATA((I2C_ADDR<<1)+1);		//Addr+R
	temp = I2C_RECV_ACK();
	if(temp != 0)
	{
		I2C_STOP();
		return 0x13;
	}

	 for(i=0;i<(*fflen-1);i++)
	 {
		*(ffbuf+i) = I2C_RECV_DATA();		
		I2C_SEND_ACK(0);		//ACK
	 }

	 *(ffbuf+i) = I2C_RECV_DATA();		
	 I2C_SEND_ACK(1);		//NACK

	I2C_STOP();

	return true;
}

