/* /////////////////////////////////////////////////////////////////////////////////////////////////
//                     Copyright (c) FMSH
///////////////////////////////////////////////////////////////////////////////////////////////// */

#ifndef LPCD_REG_PRIVATE_H
#define LPCD_REG_PRIVATE_H




//============================================================================
#define		BFL_JREG_LPCDCTRL4				0x04	//Lpcd Ctrl register4
//============================================================================
//#define		BFL_JBIT_LPCD_GAIN_0			0x00	//Lpcd Gain Level 0: 1
//#define		BFL_JBIT_LPCD_GAIN_1			0x08	//Lpcd Gain Level 1: 1/2
//#define		BFL_JBIT_LPCD_GAIN_2			0x10	//Lpcd Gain Level 2: 1/3
//#define		BFL_JBIT_LPCD_GAIN_3			0x18	//Lpcd Gain Level 3: 2


//============================================================================

//============================================================================
#define		BFL_JREG_CHARGECURRENT			0x05	//Charge Current register
//============================================================================
#define		BFL_JBIT_CHARGECURRENT_0		0x00	//Charge current level 0
#define		BFL_JBIT_CHARGECURRENT_1		0x01	//Charge current level 1
#define		BFL_JBIT_CHARGECURRENT_2		0x02	//Charge current level 2
#define		BFL_JBIT_CHARGECURRENT_3		0x03	//Charge current level 3
#define		BFL_JBIT_CHARGECURRENT_4		0x04	//Charge current level 4
#define		BFL_JBIT_CHARGECURRENT_5		0x05	//Charge current level 5
#define		BFL_JBIT_CHARGECURRENT_6		0x06	//Charge current level 6
#define		BFL_JBIT_CHARGECURRENT_7		0x07	//Charge current level 7
//----------------------------------------------------------------------------
#define		BFL_JBIT_CHARGEGAP_MSB_H		0x20	//Lpcd Chargegap MSB=H
#define		BFL_JBIT_CHARGEGAP_MSB_L		0x00	//Lpcd Chargegap MSB=L
//============================================================================

//============================================================================
#define		BFL_JREG_CHARGECAP				0x06	//ChargeCap register 
//============================================================================

//============================================================================
#define		BFL_JREG_LPCD_STATUS			0x12	//LpcdStatus Register 
//============================================================================
#define		BFL_JBIT_CARD_IN_IRQ			0x01	//irq of card in
#define		BFL_JBIT_LPCD23_IRQ				0x02	//irq of LPCD 23 end
#define		BFL_JBIT_CALIB_IRQ				0x04	//irq of calib end
#define		BFL_JBIT_LP10K_TESTOK_IRQ		0x08	//irq of lp osc 10K ok
#define		BFL_JBIT_AUTO_WUP_IRQ			0x10	//irq of Auto wake up 
//============================================================================

//============================================================================
#define		BFL_JREG_AUX1_TEST				0x13	//Aux1 select Register 
//============================================================================
#define		BFL_JBIT_AUX1_CLOSE				0x00	//close aux1 out
#define		BFL_JBIT_AUX1_VDEM_LPCD			0x01	//vdem of lpcd
#define		BFL_JBIT_AUX1_VP_LPCD			0x02	//voltage of charecap
//============================================================================

//============================================================================
#define		BFL_JREG_AUX2_TEST				0x14	//Aux2 select Register 
//============================================================================
#define		BFL_JBIT_AUX2_CLOSE				0x00	//close aux1 out
#define		BFL_JBIT_AUX2_VDEM_LPCD			0x01	//vdem of lpcd
#define		BFL_JBIT_AUX2_VP_LPCD			0x02	//voltage of charecap
//============================================================================

//============================================================================
#define		BFL_JREG_LPCD_TEST1				0x15	//LPCD test1 Register 
//============================================================================
#define		BFL_JBIT_LP_OSC10K_EN			0x01	//enable lp osc10k
#define		BFL_JBIT_LP_OSC_CALIBRA_EN		0x02	//enable lp osc10k calibra mode
#define		BFL_JBIT_LP_CURR_TEST			0x04	//enable lp t1 current test
#define		BFL_JBIT_LPCD_TEST2_LPCD_OUT	0x08	//lpcd_test2[3]:LPCD_OUT 
//============================================================================

//============================================================================
#define		BFL_JREG_LPCD_TEST2				0x16	//LPCD test2 Register 
//============================================================================
#define		BFL_JBIT_T1_OUT_EN				0x01	//D5:T1_OUT
#define		BFL_JBIT_OSCCLK_OUT_EN			0x02	//D4:OSC_CLK_OUT
#define		BFL_JBIT_OSCEN_OUT_EN			0x04	//D3:OSC_EN
#define		BFL_JBIT_LP_CLK_LPCD_OUT_EN		0x08	//D2:LP_CLK or LPCD_OUT
#define		BFL_JBIT_T3_OUT_EN				0x10	//D1:T3_OUT
//============================================================================		

//============================================================================
#define		BFL_JREG_LP_CLK_CNT1			0x17	//lp_clk_cnt[5:0] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_LP_CLK_CNT2			0x18	//lp_clk_cnt[7:6] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_VERSIONREG2			0x19	//VersionReg2[1:0] Register 
//============================================================================

//============================================================================
#define		BFL_JREG_IRQ_BAK				0x1A	//Irq bak Register 
//============================================================================
#define		BFL_JBIT_IRQ_INV_BAK			0x01	//Irq Inv backup
#define		BFL_JBIT_IRQ_PUSHPULL_BAK		0x02	//Irq pushpull backup
//============================================================================


//============================================================================
#define		BFL_JREG_LPCD_TEST3				0x1B	//LPCD TEST3 Register 
//============================================================================
#define		BFL_JBIT_LPCD_TESTEN			0x01	//lPCD test mode
#define		BFL_JBIT_AWUP_TSEL				0x02	//Auto wakeup test mode
#define		BFL_JBIT_RNG_MODE_SEL			0x04	//RNG  mode sel
#define		BFL_JBIT_USE_RET				0x08	//use retention mode
//============================================================================

//============================================================================
#define		BFL_JREG_MISC				      0x1C	//LPCD Misc Register 
//============================================================================
#define		BFL_JBIT_CALIB_VMID_EN			0x01	//lPCD test mode
#define		BFL_JBIT_AMP_EN_SEL				  0x04	//LPCD amp en select

//============================================================================

#endif 

/* E.O.F. */
