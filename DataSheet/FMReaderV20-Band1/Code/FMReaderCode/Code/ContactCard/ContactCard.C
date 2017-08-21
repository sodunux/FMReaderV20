#include "includes.h"
	
/*******************************************************************************
* Function Name  : ContactCardPPS.
* Description    : Contact card PPS process.
* Input          : PPS1  : Equal to TA1 byte of ATR.
* Output         : None
* Return         : CT_T0_OK, 
                   CT_T0_PPS_TIMEOUT
*******************************************************************************/
uint8_t ContactCardPPS(uint8_t PPS1)
{
		u8 res;
		res=SC_PPS(PPS1);
		if(res)
			return CT_T0_OK;
		else
     return CT_T0_PPS_TIMEOUT;
}

/*******************************************************************************
* Function Name  : ContactCardT0APDU.
* Description    : Contact card APDU process.
* Input          : APDUBuffer   : APDU bytes are stored here.
                   APDUSendLen  : How many bytes will be sent to card.
                   APDURecvLen  : How many bytes received from card.
* Output         : None
* Return         : CT_T0_OK,
                   CT_T0_APDU_LEN_ERROR,                   
*******************************************************************************/
uint8_t ContactCardT0APDU(uint8_t *APDUBuffer, uint16_t APDUSendLen, uint16_t *APDURecvLen)
{
	u8 b;
	if(APDUSendLen < 4)
    	return CT_T0_APDU_LEN_ERROR;
	apdu_commands.Header.CLA=APDUBuffer[0];
	apdu_commands.Header.INS=APDUBuffer[1];
	apdu_commands.Header.P1	=APDUBuffer[2];
	apdu_commands.Header.P2	=APDUBuffer[3];

	if(APDUSendLen == 4)	//LC=0,LE!=0;	
  	{	
		apdu_commands.Body.LC=0;
		apdu_commands.Body.LE=0;
  	}
  
  	else if(APDUSendLen==5) 
  	{
		apdu_commands.Body.LE=APDUBuffer[4];
		apdu_commands.Body.LC=0; 
  	}
	
  	else if(APDUSendLen>5) //LC != 0
  	{
		apdu_commands.Body.LC=APDUBuffer[4];	
		
		if(apdu_commands.Body.LC==(APDUSendLen-5))  //LE=0
		{
			for(b=0;b<(APDUSendLen-5);b++)
			{
				apdu_commands.Body.Data[b]=APDUBuffer[5+b];
			}	
			apdu_commands.Body.LE=0;
		}
		
		else if(apdu_commands.Body.LC==(APDUSendLen-6)) //LE != 0
		{
			for(b=0;b<(APDUSendLen-6);b++)
			{
				apdu_commands.Body.Data[b]=APDUBuffer[5+b];
			}	
			apdu_commands.Body.LE=APDUBuffer[5+b];
		}
		else
			return CT_T0_APDU_LEN_ERROR;
  }
	 
    /* Send APDU header */
	SC_ApduExchange(&apdu_commands,&apdu_responce);//state=SC_ACTIVE_ON_T0	
	if(((apdu_responce.SW1&0x60)!=0x60)&&((apdu_responce.SW1&0x90)!=0x90))
		return CT_T0_APDU_TIMEOUT;
	else 
	{
		if((apdu_commands.Body.LE)&&(apdu_commands.Body.LC==0))
		{
			for(b=0;b<apdu_commands.Body.LE;b++)
				APDUBuffer[b]	=	apdu_responce.Data[b];
			APDUBuffer[b]		=	apdu_responce.SW1;
			APDUBuffer[b+1]		=	apdu_responce.SW2;
			*APDURecvLen=b+2;
		}
		else 
		{
			APDUBuffer[0]=apdu_responce.SW1;
			APDUBuffer[1]=apdu_responce.SW2;
			*APDURecvLen=2;
		}
		return CT_T0_OK;
	}	
}




