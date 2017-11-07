
#include "IIC.h"




/*******************************************************************************
* Function Name  : IIC_Delay
* Description    : IIC Delay time
* Input          : None
* Output         : None
* Return         : None
****************************************************************************** */

void IIC_Delay(void)
{
		
   int i=1000;  
   while(i) 
   { 
     i--; 
   }  
}


/*******************************************************************************
* Function Name  : IIC_Start
* Description    : Master Start Simulation IIC Communication
* Input          : 1 or 2
* Output         : None
* Return         : Wheather	 Start
****************************************************************************** */
bool IIC_Start()
{
	SDA_H;
	SCL_H;
	IIC_Delay();
	if(!SDA_READ)return FALSE;	//SDA?????????,??
	SDA_L;
	IIC_Delay();
	if(SDA_READ) return FALSE;	//SDA??????????,??
	SDA_L;
	IIC_Delay();
	return TRUE;
}


/*******************************************************************************
* Function Name  : IIC_Stop
* Description    : Master Stop Simulation IIC Communication
* Input          : 1 or 2
* Output         : None
* Return         : None
****************************************************************************** */
void IIC_Stop()
{

	SCL_L;
	IIC_Delay();
	SDA_L;
	IIC_Delay();
	SCL_H;
	IIC_Delay();
	SDA_H;
	IIC_Delay();
} 

/*******************************************************************************
* Function Name  : IIC_Ack
* Description    : Master Send Acknowledge Signal
* Input          : 1 or 2
* Output         : None
* Return         : None
****************************************************************************** */
void IIC_Ack()
{	

	SCL_L;
	IIC_Delay();
	SDA_L;
	IIC_Delay();
	SCL_H;
	IIC_Delay();
	SCL_L;
	IIC_Delay(); 
	
}
/*******************************************************************************
* Function Name  : IIC_NoAck
* Description    : Master Send No Acknowledge Signal
* Input          : 1 or 2
* Output         : None
* Return         : None
****************************************************************************** */
void IIC_NoAck()
{

	SCL_L;
	IIC_Delay();
	SDA_H;
	IIC_Delay();
	SCL_H;
	IIC_Delay();
	SCL_L;
	IIC_Delay();
}

 

/*******************************************************************************
* Function Name  : IIC_WaitAck
* Description    : Master Reserive Slave Acknowledge Single
* Input          : 1 or 2
* Output         : None
* Return         : Wheather	 Reserive Slave Acknowledge Single
****************************************************************************** */
bool IIC_WaitAck() 	 
{

	SCL_L;
	IIC_Delay();
	SDA_H;			
	IIC_Delay();
	SCL_H;
	IIC_Delay();
	if(SDA_READ)
	{
      	SCL_L;
	  	IIC_Delay();
      	return FALSE;
	}
	SCL_L;
	IIC_Delay();
	return TRUE;



}


/*******************************************************************************
* Function Name  : IIC_SendByte
* Description    : Master Send a Byte to Slave
* Input          : IIC1 or IIC2,Will Send Date
* Output         : None
* Return         : None
****************************************************************************** */
void IIC_SendByte(u8 sendbyte) //MSB first
{
    u8 i=8;
    while(i--)
    {
      	SCL_L;
        	IIC_Delay();
     	if(sendbyte&0x80)
       		SDA_H;  
      	else 
        		SDA_L;   
       	sendbyte<<=1;
       	IIC_Delay();
				SCL_H;
       	IIC_Delay();
    }
    SCL_L;
   
}


/*******************************************************************************
* Function Name  : IIC_ReadByte
* Description    : Master Reserive a Byte From Slave
* Input          : 1 or 2
* Output         : None
* Return         : Date From Slave 
****************************************************************************** */
u8 IIC_ReadByte() 
{ 
    u8 i=8;
    u8 receivebyte=0;
   	SDA_H;				
   	while(i--)
   	{
     	receivebyte<<=1;      
     	SCL_L;
     	IIC_Delay();
	 	SCL_H;
     	IIC_Delay();	
     	if(SDA_READ)
     	{
       	receivebyte|=0x01;
     	}
   	}
   	SCL_L;  	
    return receivebyte;
} 

/*******************************************************************************
* Function Name  : IIC_SendData
* Description    : Send Data via IIC
* Input          : addr:0x00~0xFF
									 senddata:data buff address
									 len:senddata length
* Return         : true:
											opertion OK.
									 false:
											opertion failed.
*******************************************************************************/
bool IIC_SendData(u8 addr,u8 *senddata,u8 len)
{
	bool flag;
	u8 i,bccvalue;
	u32 j;
	u8 *ptr=senddata;	
	IIC_Start();
	IIC_SendByte(addr);
	flag=IIC_WaitAck();
	if(!flag) 
		return 0;
	for(i=0;i<len;i++)
	{
		for(j=0;j<1000;j++);
		IIC_SendByte(*ptr);
		flag=IIC_WaitAck();
		if(!flag) 
			return 0;
		ptr++;
	}	
	IIC_Stop();
	return 1;
	
}

/*******************************************************************************
* Function Name  : IIC_SendData
* Description    : Read Data via IIC
* Input          : addr:0x00~0xFF
									 Read:data buff address
									 len:senddata length
* Return         : true:
											opertion OK.
									 false:
											opertion failed.
*******************************************************************************/
bool IIC_ReadData(u8 addr,u8 *readbuf,u8 len)
{
	u8 *ptr=readbuf;
	u8 i;
	u32 j;
	bool flag;
	IIC_Start();
	IIC_SendByte(addr|0x01);
	flag=IIC_WaitAck();
	if(!flag) 
			return 0;
	for(i=0;i<(len-1);i++)
	{
		for(j=0;j<1000;j++);
		*ptr=IIC_ReadByte();
		IIC_Ack();
		ptr++;
	}
	for(j=0;j<1000;j++);	
	*ptr=IIC_ReadByte();
	IIC_NoAck();
	IIC_Stop();
	return 1;
}


