#include "Global.h"
#include "RegCtrl.h"
#include "FMReg.h"
#include "Define.h"
#include "CardAPI.h"
#include "BasicAPI.h"
#include "lpcd_regctrl_private.h"
#include "lpcd_reg_private.h"
#include "lpcd_reg_public.h"
#include "lpcd_api_public.h"

unsigned char LpcdEnBak;

//复位全部寄存器
uchar FM175xx_ResetAllReg(void)
{
	uchar addr,regdata;
	uchar res;

	addr = COMMAND;	
	regdata = CMD_SOFT_RESET;
	res = SetReg(addr,regdata);
	if(res != true)
		return FM175xx_REG_ERR;

	return FM175xx_DEBUG_OK;
}

//During soft power-down, all register values, the FIFO buffer  content 
//and the configuration keep their current contents.
uchar FM175xx_SPD(void)
{
	uchar addr,mask,set;
	uchar res;

	addr = COMMAND;
	mask = BIT4;
	set  = 1;
	res = ModifyReg(addr,mask,set);
	if(res != true)
		return FM175xx_REG_ERR;
	
	return FM175xx_DEBUG_OK;
}

//FM175xx 初始化
uchar FM175xx_Initial(void)
{
	uchar regdata,res;
	FM175xx_ResetAllReg();
	if((FM175xx_FUNC_SEL&0x10) || (FM175xx_FUNC_SEL & 0x08))          //CardA初始化选项
	{ 			
		res = FM175xx_Initial_Card();
		if(res != FM175xx_DEBUG_OK)
			return res;
		res = FM175xx_Card_Config(Card_UID_Buf);
		if(res != FM175xx_DEBUG_OK)
			return res;
		res = FM175xx_Card_AutoColl();
		if(res != FM175xx_DEBUG_OK)
			return res;
	}
	
	regdata = 0x20;			 //WaterLevel，收到一半数据时候，起中断
	res = SetReg(WATERLEVEL,regdata);
	if(res != true)
		return FM175xx_REG_ERR;
		
	return FM175xx_DEBUG_OK;
}

uchar FM175xx_IRQ_Pro(uchar tag_com_irq,uchar tag_div_irq)
{
	uchar res;
	uchar reg_com_irq,reg_div_irq;
	uint i;
	uchar lpcd_irq;

	if(Test_LPCD_AutoWakeUp)//LPCD 模式
	{
			NRSTPD_CTRL(1);			 //NRST
			for(i=8100;i>0;i--);     //等待晶振起振
			for(i=8100;i>0;i--);
			res = GetReg_Ext(BFL_JREG_LPCD_STATUS,&lpcd_irq);
			if(res != true)
				return FM175xx_REG_ERR;
			if(lpcd_irq & BFL_JBIT_AUTO_WUP_IRQ)		//自动唤醒中断
			{
				Test_LPCD_AutoWakeUp = false;	
			}
	}
	else
	{
		res = GetReg(COMMIRQ,&reg_com_irq);
		if(res != true)
			return FM175xx_REG_ERR;
		reg_com_irq &= tag_com_irq;
		res = GetReg(DIVIRQ,&reg_div_irq);
		if(res != true)
			return FM175xx_REG_ERR;
		reg_div_irq	&= tag_div_irq;
	
//		res = SetReg(COMMIRQ,reg_com_irq);
//		if(res != true)
//			return FM175xx_REG_ERR;
		reg_com_irq &= tag_com_irq;
//		res = SetReg(DIVIRQ,reg_div_irq);
//		if(res != true)
//			return FM175xx_REG_ERR;

		if(FM175xx_CurFunc == FM175xx_FUNC_CARD)
		{
			if(reg_com_irq & PHCS_BFL_JBIT_TXI)	
			{
				;
			}	 
			if((reg_com_irq & PHCS_BFL_JBIT_RXI)|(reg_com_irq & PHCS_BFL_JBIT_HIALERTI))	//	
			{
				FM175xx_Card_Actived = TRUE;	
			} 
			if(reg_com_irq & PHCS_BFL_JBIT_ERRI)	
			{
				;
			} 			
		}
	}
	return FM175xx_DEBUG_OK;
}


//*******************************************************
//函数名称：Write_FIFO
//函数功能：写FIFO
//入口参数：fflen:写入长度；
//         ffbuf:数据缓冲区；
//出口参数：uchar  TRUE：写成功   FALSE:写失败
//********************************************************
uchar Write_FIFO(uchar fflen,uchar* ffbuf)
{
	uchar  i;
	uchar res;
	if(fflen > 0x40)
		return false;
		
	for(i=0;i<fflen;i++)
	{
		res = SetReg(0x09,*(ffbuf+i));
		if(res != true)
			return res;
	}
	return true;	
}

/******************************************************************************/
//函数名：Command_Transceive
//入口参数：
//			uchar* sendbuf:发送数据缓冲区	uchar sendlen:发送数据长度
//			uchar* recvbuf:接收数据缓冲区	uchar* recvlen:接收到的数据长度
//出口参数：uchar 0:发送接收流程正常	0x50：寄存器读写错误 
/******************************************************************************/
uchar Command_Transceive(NFC_DataExTypeDef* NFC_DataExStruct)
{
	uchar regdata;
	uchar i;
	uchar res;
	uint dly = 0;
	res = ModifyReg(COMMIRQ,PHCS_BFL_JBIT_TXI|PHCS_BFL_JBIT_TXI|PHCS_BFL_JBIT_ERRI,SET);
	
	res = SetReg(COMMAND,CMD_TRANSCEIVE);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	res = ModifyReg(COMMIEN,PHCS_BFL_JBIT_TXI,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	NFC_DataExStruct->WaitForComm = PHCS_BFL_JBIT_TXI;
		
	res = SetReg(FIFOLEVEL,PHCS_BFL_JBIT_FLUSHFIFO);	//FlushFIFO
	if(res != TRUE)
		return FM175XX_REG_ERR;

	if(NFC_DataExStruct->nBytesToSend > 0x40)
	{
		res = Write_FIFO(0x40,NFC_DataExStruct->pExBuf);
		if(res != TRUE)
				return FM175XX_REG_ERR;
	}
	else
	{
		res = Write_FIFO(NFC_DataExStruct->nBytesToSend,NFC_DataExStruct->pExBuf);
		if(res != TRUE)
				return FM175XX_REG_ERR;
	}

	res = ModifyReg(BITFRAMING,PHCS_BFL_JBIT_STARTSEND,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	if(NFC_DataExStruct->nBytesToSend > 0x40)
	{		
		res = SetReg(WATERLEVEL,0x08);  //waterlevel设置
		if(res != TRUE)
			return FM175XX_REG_ERR;
						
		for(i=0x40;i<NFC_DataExStruct->nBytesToSend;)		   //数据没有发完
		{
			res = GetReg(STATUS1,&regdata);
			if(res != TRUE)
				return FM175XX_REG_ERR;

			if(!(regdata&0x02))	 //HiAlert没有置起
			{
					regdata = *(NFC_DataExStruct->pExBuf+i);
					res = SetReg(FIFODATA,regdata);
					if(res != TRUE)
						return FM175XX_REG_ERR;
					i++;
			}	
		}
		
	}
	
	regdata = 0;
	while(!(regdata & NFC_DataExStruct->WaitForComm) && dly<1000)		//等待发送结束 20ms
	{
		res = GetReg(COMMIRQ,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		mDelay(1);			//0.1ms
		dly++;
	}
	if(dly == 255)
		return FM175XX_TRANS_ERR;


	res = ModifyReg(COMMIEN,PHCS_BFL_JBIT_TXI,CLR);
	if(res != TRUE)
		return FM175XX_REG_ERR;

	NFC_DataExStruct->WaitForComm =  PHCS_BFL_JBIT_RXI|PHCS_BFL_JBIT_ERRI;
	res = ModifyReg(COMMIEN,NFC_DataExStruct->WaitForComm,SET);
	if(res != TRUE)
		return FM175XX_REG_ERR;
	
	NFC_DataExStruct->AllCommIrq = 0;   								//临时用来储存irq
	NFC_DataExStruct->nBytesReceived  = 0;
	dly = 0;
	while(dly<1000)		//等待接收结束 20ms		 //使用dly缓存irq寄存器值
	{
		res = GetReg(FIFOLEVEL,&regdata);
		if(res != TRUE)
			return FM175XX_REG_ERR;
		regdata &= 0x7F;
			
		if(regdata != 0)	//data coming
		{
			for(i=0;i<regdata;i++)
			{
				res = GetReg(FIFODATA,NFC_DataExStruct->pExBuf+i+NFC_DataExStruct->nBytesReceived);
				if(res != TRUE)
					return FM175XX_REG_ERR;
			} 
			NFC_DataExStruct->nBytesReceived += regdata;
		}
		res = GetReg(COMMIRQ,&NFC_DataExStruct->AllCommIrq);	  //使用tmpdata缓存irq寄存器值
		if(res != TRUE)
			return FM175XX_REG_ERR;
		if(!(NFC_DataExStruct->AllCommIrq & NFC_DataExStruct->WaitForComm))
		{
			mDelay(1);			//0.1ms
			dly++;
		}
		else
		{
			if(dly > 997)
				dly++;
			else
			{
				mDelay(1);
				dly = 998;
			}
		}
			
	}
	return FM175XX_SUCCESS;
}

void AUX1_SET(unsigned char AuxSel)
{
	 // unsigned char AuxSel;
    unsigned char RegAddr,RegData;
    unsigned char RegAddrLpcd,RegDataLpcd;
    unsigned char ret;

    unsigned char mask,res,set;

   // AuxSel = rdoAnalogAuxSel1->ItemIndex;

    if ((AuxSel>15)&&(AuxSel<21)) //??????    16-20
    {
         //------------------------------------------
         //disable dac
         //en_b_dac=1
         //a39_7=1;
         //------------------------------------------
         RegAddr = 0x39;
         mask = BIT7;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable rst_follow
         //rst_follow=0
         //a3C_1=0;
         //------------------------------------------
         RegAddr = 0x3C;
         mask = BIT1;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable  bypass_follow
         //bypass_follow1=0
         //a3D_1=0;
         //bypass_follow1=0
         //a3D_2=0;
         //by_follow=0
         //a3D_0=0
         //------------------------------------------
         RegAddr = 0x3D;
         mask = BIT2 | BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //enable follow
         //en_b_follow1=0
         //a3E_1 = 0
         //en_b_follow2=0
         //a3E_0 = 0
         //------------------------------------------
         RegAddr = 0x3E;
         mask =  BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------
    }
    else  if (AuxSel<16) //DAC????    0-15
    {
         //------------------------------------------
         //enable dac
         //en_b_dac=0
         //a39_7=0;
         //------------------------------------------
         RegAddr = 0x39;
         mask = BIT7;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //enable rst_follow
         //rst_follow=1
         //a3C_1=1;
         //------------------------------------------
         RegAddr = 0x3C;
         mask = BIT1;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //enble  bypass_follow
         //bypass_follow1=1
         //a3D_1=1;
         //bypass_follow1=1
         //a3D_2=1;
         //by_follow=1
         //a3D_0=1
         //------------------------------------------
         RegAddr = 0x3D;
         mask = BIT2 | BIT1 | BIT0;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //disable follow
         //en_b_follow1=1
         //a3E_1 = 1
         //en_b_follow2=1
         //a3E_0 = 1
         //------------------------------------------
         RegAddr = 0x3E;
         mask =  BIT1 | BIT0;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------
    }
    else    //LPCD????
    {
                //------------------------------------------
         //disable dac
         //en_b_dac=1
         //a39_7=1;
         //------------------------------------------
         RegAddr = 0x39;
         mask = BIT7;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable rst_follow
         //rst_follow=0
         //a3C_1=0;
         //------------------------------------------
         RegAddr = 0x3C;
         mask = BIT1;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable  bypass_follow
         //bypass_follow1=0
         //a3D_1=0;
         //bypass_follow1=0
         //a3D_2=0;
         //by_follow=0
         //a3D_0=0
         //------------------------------------------
         RegAddr = 0x3D;
         mask = BIT2 | BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //enable follow
         //en_b_follow1=0
         //a3E_1 = 0
         //en_b_follow2=0
         //a3E_0 = 0
         //------------------------------------------
         RegAddr = 0x3E;
         mask =  BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------


        //lpcd_en=1
        RegAddr = BFL_JREG_LPCDCTRL1;
        res = GetReg(RegAddr,&RegData);
        if ((RegData & BFL_JBIT_LPCD_EN) == BFL_JBIT_LPCD_EN)
        {
            LpcdEnBak=1;//????LPCD_EN
        }
        else
        {
            LpcdEnBak=0;//????LPCD_EN,?????????
        }
        RegData = BFL_JBIT_BIT_CTRL_SET + BFL_JBIT_LPCD_EN+BFL_JBIT_LPCD_RSTN+BFL_JBIT_LPCD_CALIBRA_EN;
        res = SetReg(RegAddr,RegData);
    }

    //if (AuxSel<21)
    //{
        RegAddr = ANALOGTEST;
        ret = GetReg(RegAddr,&RegData);
    //}
    //else
    //{
        RegAddrLpcd  = 0x13;
       // ret = GetReg(RegAddrLpcd,&RegDataLpcd);
		   GetReg_Ext(0x13,&RegDataLpcd);
    //}



    switch(AuxSel)
    {
        case 0x0:RegData = 0 ;RegDataLpcd = 0 ;break;//all close
        case 0x1:RegData = (RegData & 0x0F) + 0x10 ;RegDataLpcd = 0 ;break;
        case 0x2:RegData = (RegData & 0x0F) + 0x20 ;RegDataLpcd = 0 ;break;
        case 0x3:RegData = (RegData & 0x0F) + 0x30 ;RegDataLpcd = 0 ;break;
        case 0x4:RegData = (RegData & 0x0F) + 0x40 ;RegDataLpcd = 0 ;break;
        case 0x5:RegData = (RegData & 0x0F) + 0x50 ;RegDataLpcd = 0 ;break;
        case 0x6:RegData = (RegData & 0x0F) + 0x60 ;RegDataLpcd = 0 ;break;
        case 0x7:RegData = (RegData & 0x0F) + 0x70 ;RegDataLpcd = 0 ;break;
        case 0x8:RegData = (RegData & 0x0F) + 0x80 ;RegDataLpcd = 0 ;break;
        case 0x9:RegData = (RegData & 0x0F) + 0x90 ;RegDataLpcd = 0 ;break;
        case 0xA:RegData = (RegData & 0x0F) + 0xA0 ;RegDataLpcd = 0 ;break;
        case 0xB:RegData = (RegData & 0x0F) + 0xB0 ;RegDataLpcd = 0 ;break;
        case 0xC:RegData = (RegData & 0x0F) + 0xC0 ;RegDataLpcd = 0 ;break;
        case 0xD:RegData = (RegData & 0x0F) + 0xD0 ;RegDataLpcd = 0 ;break;
        case 0xE:RegData = (RegData & 0x0F) + 0xE0 ;RegDataLpcd = 0 ;break;
        case 0xF:RegData = (RegData & 0x0F) + 0xF0 ;RegDataLpcd = 0 ;break;
        //-----------------------------------------------
        case 0x10:RegData = (RegData & 0x0F) + 0xA0 ;RegDataLpcd = 0 ;break;
        case 0x11:RegData = (RegData & 0x0F) + 0xB0 ;RegDataLpcd = 0 ;break;
        case 0x12:RegData = (RegData & 0x0F) + 0xC0 ;RegDataLpcd = 0 ;break;
        case 0x13:RegData = (RegData & 0x0F) + 0x90 ;RegDataLpcd = 0 ;break;
        case 0x14:RegData = (RegData & 0x0F) + 0x80 ;RegDataLpcd = 0 ;break;
        //-----------------------------------------------
        case 0x15:RegDataLpcd = 0x03 ;RegData = 0 ;break;  //vdd_osc
        case 0x16:RegDataLpcd = 0x04 ;RegData = 0 ;break;  //oscin_out
        case 0x17:RegDataLpcd = 0x05 ;RegData = 0 ;break;  //ldo_mode_sel
        case 0x18:RegDataLpcd = 0x06 ;RegData = 0 ;break;  //enb_ldo15_delay
        case 0x19:RegDataLpcd = 0x01 ;RegData = 0 ;break;  //vp_lpcd
        case 0x1A:RegDataLpcd = 0x02 ;RegData = 0 ;break;  //vdem_lpcd
        default:break;
    }
     ret = SetReg(RegAddr,RegData);
     //ret = SetReg(RegAddrLpcd,RegDataLpcd);
			SetReg_Ext(0x13,RegDataLpcd);
	
}


void AUX2_SET(unsigned char AuxSel)
{
	  unsigned char RegAddr,RegData;
    unsigned char RegAddrLpcd,RegDataLpcd;
    unsigned char ret;

    unsigned char mask,res,set;

   // AuxSel = rdoAnalogAuxSel2->ItemIndex;

    if ((AuxSel>15)&&(AuxSel<21)) //??????
    {
         //------------------------------------------
         //disable dac
         //en_b_dac=1
         //a39_7=1;
         //------------------------------------------
         RegAddr = 0x39;
         mask = BIT7;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable rst_follow
         //rst_follow=0
         //a3C_1=0;
         //------------------------------------------
         RegAddr = 0x3C;
         mask = BIT1;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable  bypass_follow
         //bypass_follow1=0
         //a3D_1=0;
         //bypass_follow1=0
         //a3D_2=0;
         //by_follow=0
         //a3D_0=0
         //------------------------------------------
         RegAddr = 0x3D;
         mask = BIT2 | BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //enable follow
         //en_b_follow1=0
         //a3E_1 = 0
         //en_b_follow2=0
         //a3E_0 = 0
         //------------------------------------------
         RegAddr = 0x3E;
         mask =  BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------
    }
    else  if(AuxSel < 16)  //DAC????
    {
         //------------------------------------------
         //enable dac
         //en_b_dac=0
         //a39_7=0;
         //------------------------------------------
         RegAddr = 0x39;
         mask = BIT7;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //enable rst_follow
         //rst_follow=1
         //a3C_1=1;
         //------------------------------------------
         RegAddr = 0x3C;
         mask = BIT1;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //enble  bypass_follow
         //bypass_follow1=1
         //a3D_1=1;
         //bypass_follow1=1
         //a3D_2=1;
         //by_follow=1
         //a3D_0=1
         //------------------------------------------
         RegAddr = 0x3D;
         mask = BIT2 | BIT1 | BIT0;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //disable follow
         //en_b_follow1=1
         //a3E_1 = 1
         //en_b_follow2=1
         //a3E_0 = 1
         //------------------------------------------
         RegAddr = 0x3E;
         mask =  BIT1 | BIT0;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------
    }
    else    //LPCD????
    {
         //------------------------------------------
         //disable dac
         //en_b_dac=1
         //a39_7=1;
         //------------------------------------------
         RegAddr = 0x39;
         mask = BIT7;
         set = 1;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable rst_follow
         //rst_follow=0
         //a3C_1=0;
         //------------------------------------------
         RegAddr = 0x3C;
         mask = BIT1;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //------------------------------------------
         //disable  bypass_follow
         //bypass_follow1=0
         //a3D_1=0;
         //bypass_follow1=0
         //a3D_2=0;
         //by_follow=0
         //a3D_0=0
         //------------------------------------------
         RegAddr = 0x3D;
         mask = BIT2 | BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------

         //enable follow
         //en_b_follow1=0
         //a3E_1 = 0
         //en_b_follow2=0
         //a3E_0 = 0
         //------------------------------------------
         RegAddr = 0x3E;
         mask =  BIT1 | BIT0;
         set = 0;
         res = ModifyReg(RegAddr,mask,set);
         //------------------------------------------
         
        //lpcd_en=1   calibra_en
        RegAddr = BFL_JREG_LPCDCTRL1;
        res = GetReg(RegAddr,&RegData);
        if ((RegData & BFL_JBIT_LPCD_EN) == BFL_JBIT_LPCD_EN)
        {
            LpcdEnBak=1;//????LPCD_EN
        }
        else
        {
            LpcdEnBak=0;//????LPCD_EN,?????????
        }
        RegData = BFL_JBIT_BIT_CTRL_SET + BFL_JBIT_LPCD_EN+BFL_JBIT_LPCD_RSTN+BFL_JBIT_LPCD_CALIBRA_EN;
        res = SetReg(RegAddr,RegData);
    }

    //if (AuxSel<21)
    //{
        RegAddr = ANALOGTEST;
        ret = GetReg(RegAddr,&RegData);
    //}
    //else
    //{
        RegAddrLpcd  = 0X14;
       // ret = GetReg(RegAddrLpcd,&RegDataLpcd);
				
				GetReg_Ext(0x14,&RegDataLpcd);	
    //}


    switch(AuxSel)
    {
        case 0x0:RegData = 0 ;RegDataLpcd = 0 ;break;//all close
        case 0x1:RegData = (RegData & 0xF0) + 0x01 ;RegDataLpcd = 0 ;break;
        case 0x2:RegData = (RegData & 0xF0) + 0x02 ;RegDataLpcd = 0 ;break;
        case 0x3:RegData = (RegData & 0xF0) + 0x03 ;RegDataLpcd = 0 ;break;
        case 0x4:RegData = (RegData & 0xF0) + 0x04 ;RegDataLpcd = 0 ;break;
        case 0x5:RegData = (RegData & 0xF0) + 0x05 ;RegDataLpcd = 0 ;break;
        case 0x6:RegData = (RegData & 0xF0) + 0x06 ;RegDataLpcd = 0 ;break;
        case 0x7:RegData = (RegData & 0xF0) + 0x07 ;RegDataLpcd = 0 ;break;
        case 0x8:RegData = (RegData & 0xF0) + 0x08 ;RegDataLpcd = 0 ;break;
        case 0x9:RegData = (RegData & 0xF0) + 0x09 ;RegDataLpcd = 0 ;break;
        case 0xA:RegData = (RegData & 0xF0) + 0x0A ;RegDataLpcd = 0 ;break;
        case 0xB:RegData = (RegData & 0xF0) + 0x0B ;RegDataLpcd = 0 ;break;
        case 0xC:RegData = (RegData & 0xF0) + 0x0C ;RegDataLpcd = 0 ;break;
        case 0xD:RegData = (RegData & 0xF0) + 0x0D ;RegDataLpcd = 0 ;break;
        case 0xE:RegData = (RegData & 0xF0) + 0x0E ;RegDataLpcd = 0 ;break;
        case 0xF:RegData = (RegData & 0xF0) + 0x0F ;RegDataLpcd = 0 ;break;
        case 0x10:RegData = (RegData & 0xF0) + 0x0A ;RegDataLpcd = 0 ;break;
        case 0x11:RegData = (RegData & 0xF0) + 0x0B ;RegDataLpcd = 0 ;break;
        case 0x12:RegData = (RegData & 0xF0) + 0x0C ;RegDataLpcd = 0 ;break;
        case 0x13:RegData = (RegData & 0xF0) + 0x09 ;RegDataLpcd = 0 ;break;
        case 0x14:RegData = (RegData & 0xF0) + 0x08 ;RegDataLpcd = 0 ;break;
        //---------------------------------------------------
        case 0x15:RegDataLpcd = 0x01 ;RegData = 0;break;  //vp_lpcd
        case 0x16:RegDataLpcd = 0x02 ;RegData = 0;break;  //vdem_lpcd
        case 0x17:RegDataLpcd = 0x03 ;RegData = 0;break;  //vb2_ldo15
        case 0x18:RegDataLpcd = 0x04 ;RegData = 0;break;  //vbn_lpcd
        case 0x19:RegDataLpcd = 0x05 ;RegData = 0;break;  //ldo_mode_ctrl
        default:break;
    }

    ret = SetReg(RegAddr,RegData);
		SetReg_Ext(0x14,RegDataLpcd);
    //ret = SetReg(RegAddrLpcd,RegDataLpcd);
	
	
	
	
}

void Quit_AUXSET(void)
{
	   uchar RegAddr,res,RegData;
     RegAddr = 0x53;
     res = GetReg(RegAddr,&RegData);
     if (LpcdEnBak == 0)//??????,????
     {
        //RegData = BFL_JBIT_BIT_CTRL_CLR + BFL_JBIT_LPCD_EN;
        RegData = BFL_JBIT_BIT_CTRL_CLR + BFL_JBIT_LPCD_EN+BFL_JBIT_LPCD_CALIBRA_EN;
        res = SetReg(RegAddr,RegData);
        res = SetReg(RegAddr,RegData);
     }
	
	
}




