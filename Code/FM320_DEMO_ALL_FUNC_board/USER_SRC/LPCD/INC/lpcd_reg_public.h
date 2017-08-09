/* /////////////////////////////////////////////////////////////////////////////////////////////////
//                     Copyright (c) FMSH
///////////////////////////////////////////////////////////////////////////////////////////////// */

#ifndef LPCD_REG_PUBLIC_H
#define LPCD_REG_PUBLIC_H

//============================================================================
#define		BFL_JREG_EXT_REG_ENTRANCE		0x0F	//ext register entrance
//============================================================================
#define		BFL_JBIT_EXT_REG_WR_ADDR		0X40	//wrire address cycle
#define		BFL_JBIT_EXT_REG_RD_ADDR		0X80	//read address cycle
#define		BFL_JBIT_EXT_REG_WR_DATA		0XC0	//write data cycle
#define		BFL_JBIT_EXT_REG_RD_DATA		0X00	//read data cycle


//============================================================================
#define		BFL_JREG_LVD					0x1D	//Low Votage Detect register
//============================================================================
#define		BFL_JBIT_LVD_CTRL_24			0x03	//Low Voltage Detect of 2.4V
#define		BFL_JBIT_LVD_CTRL_21			0X02	//Low Voltage Detect of 2.1V
#define		BFL_JBIT_LVD_CTRL_19			0x01	//Low Voltage Detect of 1.9V
#define		BFL_JBIT_LVD_CTRL_DISABLE		0x00	//Disable Low Voltage Detect 
//----------------------------------------------------------------------------
#define		BFL_JBIT_LVD_IE_ENABLE			0x04	//Enable IE of Low Voltage Detect
#define		BFL_JBIT_LVD_IE_DISBLE			0x00	//Disable IE of Low Voltage Detect
//----------------------------------------------------------------------------
#define		BFL_JBIT_LVD_EFF_RST			0x00	//if Low Voltage Detect,reset digital
#define		BFL_JBIT_LVD_EFF_IRQ			0x08	//if Low Voltage Detect,generate irq
//----------------------------------------------------------------------------
#define		BFL_JBIT_LVD_IRQ				0x10	//Irq Status of Low Voltage Detect
//============================================================================

//============================================================================
#define		BFL_JREG_LPCDCTRL1				0x01	//Lpcd Ctrl register1
//============================================================================
#define		BFL_JBIT_LPCD_EN				0x01	//enble LPCD
#define		BFL_JBIT_LPCD_RSTN				0X02	//lpcd reset
#define		BFL_JBIT_LPCD_CALIBRA_EN		0x04	//into lpcd calibra mode
#define		BFL_JBIT_LPCD_CMP_1				0x08	//Compare times 1 or 3
#define		BFL_JBIT_LPCD_IE				0x10	//Enable LPCD IE
#define		BFL_JBIT_BIT_CTRL_SET			0x20	//Lpcd register Bit ctrl set bit
#define		BFL_JBIT_BIT_CTRL_CLR			0x00	//Lpcd register Bit ctrl clear bit
//============================================================================

//============================================================================
#define		BFL_JREG_LPCDCTRL2				0x02	//Lpcd Ctrl register2
//============================================================================
#define		BFL_JBIT_LPCD_CWP_180			0x00	//LPCD mode,CWGsP=180 
#define		BFL_JBIT_LPCD_CWP_90			0x01	//LPCD mode,CWGsP=90 
#define		BFL_JBIT_LPCD_CWP_46			0x02	//LPCD mode,CWGsP=46
#define		BFL_JBIT_LPCD_CWP_23			0x03	//LPCD mode,CWGsP=23
#define		BFL_JBIT_LPCD_CWP_12			0x04	//LPCD mode,CWGsP=12
#define		BFL_JBIT_LPCD_CWP_6				0x05	//LPCD mode,CWGsP=6
#define		BFL_JBIT_LPCD_CWP_3				0x06	//LPCD mode,CWGsP=3
#define		BFL_JBIT_LPCD_CWP_1				0x07	//LPCD mode,CWGsP=1
//----------------------------------------------------------------------------
#define		BFL_JBIT_LPCD_CWN_46			0x00	//LPCD mode,CWGsN=46
#define		BFL_JBIT_LPCD_CWN_23			0x08	//LPCD mode,CWGsN=23
//----------------------------------------------------------------------------
#define		BFL_JBIT_LPCD_TX2_ENABLE		0x10	//LPCD mode,TX2 enable
#define		BFL_JBIT_LPCD_TX2_DISABLE		0x00	//LPCD mode,TX2 disable   
//============================================================================


//============================================================================
#define		BFL_JREG_LPCDCTRL3				0x03	//Lpcd Ctrl register3
//============================================================================
#define		BFL_JBIT_LPCD_MODE_0			0x00	//lpcd mode 0
#define		BFL_JBIT_LPCD_MODE_1			0x08	//lpcd mode 1
#define		BFL_JBIT_LPCD_MODE_2			0x10	//lpcd mode 2

//============================================================================


//============================================================================
#define		BFL_JREG_TIMER1CFG				0x07	//Timer1Cfg[3:0] register 
//============================================================================

//============================================================================
#define		BFL_JREG_TIMER2CFG				0x08	//Timer2Cfg[4:0] register 
//============================================================================

//============================================================================
#define		BFL_JREG_TIMER3CFG				0x09	//Timer2Cfg[4:0] register 
//============================================================================

//============================================================================
#define		BFL_JREG_VMIDBDCFG				0x0A	//VmidBdCfg[4:0] register 
//============================================================================

//============================================================================
#define		BFL_JREG_AUTO_WUP_CFG			0x0B	//Auto_Wup_Cfg register 
//============================================================================
#define		BFL_JBIT_AUTO_WUP_ENABLE		0x08 	//Enable Lpcd Auto wakeup
#define		BFL_JBIT_AUTO_WUP_DISABLE		0x00 	//Disable Lpcd Auto wakeup
//----------------------------------------------------------------------------
#define		BFL_JBIT_AUTO_WUP_CFG_15MIN		0x00	//Auto Wakeup after 15 mins
#define		BFL_JBIT_AUTO_WUP_CFG_30MIN		0x01	//Auto Wakeup after 30 mins
#define		BFL_JBIT_AUTO_WUP_CFG_1HOUR		0x02	//Auto Wakeup after 1 hour
#define		BFL_JBIT_AUTO_WUP_CFG_2HOUR		0x03	//Auto Wakeup after 1.7 hours
#define		BFL_JBIT_AUTO_WUP_CFG_4HOUR		0x04	//Auto Wakeup after 3.6 hours
#define		BFL_JBIT_AUTO_WUP_CFG_7HOUR		0x05	//Auto Wakeup after 7.2 hours
#define		BFL_JBIT_AUTO_WUP_CFG_15HOUR	0x06	//Auto Wakeup after 15 hours
#define		BFL_JBIT_AUTO_WUP_CFG_30HOUR	0x07	//Auto Wakeup after 30 hours
//============================================================================

//============================================================================
#define		BFL_JREG_PULSEWIDTH1			0x0C	//PulseWidth[5:0] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_PULSEWIDTH2			0x0D	//PulseWidth[7:6] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_PULSE_THRESHOLD_L1		0x0E	//PulseThreshold_L[5:0] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_PULSE_THRESHOLD_L2		0x0F	//PulseThreshold_L[7:6] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_PULSE_THRESHOLD_H1		0x10	//PulseThreshold_H[5:0] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_PULSE_THRESHOLD_H2		0x11	//PulseThreshold_H[7:6] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_LPCD_STATUS			0x12	//LpcdStatus Register 
//============================================================================
#define		BFL_JBIT_CARD_IN_IRQ			0x01	//irq of card in
//#define		BFL_JBIT_LPCD23_IRQ				0x02	//irq of LPCD 23 end
//#define		BFL_JBIT_CALIB_IRQ				0x04	//irq of calib end
//#define		BFL_JBIT_LP10K_TESTOK_IRQ		0x08	//irq of lp osc 10K ok
#define		BFL_JBIT_AUTO_WUP_IRQ			0x10	//irq of Auto wake up 
//============================================================================


#endif 

/* E.O.F. */
