#include "includes.h"
#include "TDA8007.h"
//#include "func_switch.h"
#include "timer1.h"
/*
#define NCS_LEVEL(BitVal);  do{GPIO_WriteBit(TDA8007_NCS_PORT, TDA8007_NCS_PIN, BitVal);}while(0);
#define NRD_LEVEL(BitVal);  do{GPIO_WriteBit(TDA8007_NRD_PORT, TDA8007_NRD_PIN, BitVal);}while(0);
#define NWR_LEVEL(BitVal);  do{GPIO_WriteBit(TDA8007_NWR_PORT, TDA8007_NWR_PIN, BitVal);}while(0);
#define ALE_LEVEL(BitVal);  do{GPIO_WriteBit(TDA8007_ALE_PORT, TDA8007_ALE_PIN, BitVal);}while(0);
#define ADDR_WRITE(Value);  do{GPIO_Write(TDA8007_ADDR_PORT, \
                                          ((GPIO_ReadOutputData(TDA8007_ADDR_PORT)&0xFFF0)|Value));}while(0);
#define DATA_READ(Value);   do{Value=(uint8_t)GPIO_ReadInputData(TDA8007_DATA_PORT);}while(0);
#define DATA_WRITE(Value);  do{GPIO_Write(TDA8007_DATA_PORT, Value);}while(0);
*/
#define NCS_LEVEL_SET();    do{TDA8007_NCS_PORT->BSRR = TDA8007_NCS_PIN;}while(0);
#define NCS_LEVEL_RESET();  do{TDA8007_NCS_PORT->BRR = TDA8007_NCS_PIN;}while(0);
#define NRD_LEVEL_SET();    do{TDA8007_NRD_PORT->BSRR = TDA8007_NRD_PIN;}while(0);
#define NRD_LEVEL_RESET();  do{TDA8007_NRD_PORT->BRR = TDA8007_NRD_PIN;}while(0);
#define NWR_LEVEL_SET();    do{TDA8007_NWR_PORT->BSRR = TDA8007_NWR_PIN;}while(0);
#define NWR_LEVEL_RESET();  do{TDA8007_NWR_PORT->BRR = TDA8007_NWR_PIN;}while(0);
#define ALE_LEVEL_SET();    do{TDA8007_ALE_PORT->BSRR = TDA8007_ALE_PIN;}while(0);
#define ALE_LEVEL_RESET();  do{TDA8007_ALE_PORT->BRR = TDA8007_ALE_PIN;}while(0);
#define ADDR_WRITE(Value);  do{TDA8007_ADDR_PORT->ODR = (((uint16_t)TDA8007_ADDR_PORT->ODR & 0xFFF0)|Value);}while(0);
#define DATA_READ(Value);   do{Value=(uint8_t)TDA8007_DATA_PORT->IDR;}while(0);
#define DATA_WRITE(Value);  do{TDA8007_DATA_PORT->ODR = (((uint16_t)TDA8007_DATA_PORT->ODR & 0xFF00)|Value);}while(0);
                               
extern void TIM4Config(uint16_t  TimeBase, uint16_t Counter);
extern void SleepMs(unsigned short ms);
extern void SleepUs(unsigned short us); 
extern bool bIsCTon; 

#ifdef TDA8007_DEBUG
uint8_t abRegs[16];
uint8_t bTimeoutOpen = 0;
#endif

/*******************************************************************************
* Function Name  : TDA8007WriteReg.
* Description    : Write TDA8007 register.
* Input          : Addr : register address
                   Val  : register value
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007WriteReg(uint8_t Addr, uint8_t Val)
{
  uint8_t b;

  NCS_LEVEL_SET();
  NRD_LEVEL_RESET();
  NWR_LEVEL_RESET();
  ADDR_WRITE(Addr&0x0F);

  TDA8007_DATA_PORT->CRL = 0x33333333;
  for(b=5; b>0; b--);
for(b=50; b>0; b--);//gao	

  DATA_WRITE(Val);
  for(b=3; b>0; b--);
for(b=30; b>0; b--);//gao

  NCS_LEVEL_RESET();
  for(b=6; b>0; b--);
for(b=60; b>0; b--);//gao
  
  NCS_LEVEL_SET();
  NWR_LEVEL_SET();
//  for(b=10; b>0; b--);
}

/*******************************************************************************
* Function Name  : TDA8007ReadReg.
* Description    : Read TDA8007 register.
* Input          : Addr : register address
* Output         : None
* Return         : register value
*******************************************************************************/
uint8_t TDA8007ReadReg(uint8_t Addr)
{
  uint8_t b;
  uint8_t bReturn;
  NCS_LEVEL_SET();
  NRD_LEVEL_SET();
  NWR_LEVEL_RESET();
  ADDR_WRITE(Addr&0x0F);
 
  TDA8007_DATA_PORT->CRL = 0x88888888;
  TDA8007_DATA_PORT->BSRR = 0x000000FF; 
  for(b=3; b>0; b--);

  NCS_LEVEL_RESET();
  for(b=6; b>0; b--);

  DATA_READ(bReturn);
  for(b=3; b>0; b--);

  NCS_LEVEL_SET();
  NWR_LEVEL_SET();
//  for(b=10; b>0; b--);  

  return bReturn;
}

#ifdef TDA8007_DEBUG
/*******************************************************************************
* Function Name  : TDA8007ReadAllRegs.
* Description    : Read TDA8007 register.
* Input          : Addr : register address
* Output         : None
* Return         : register value
*******************************************************************************/
void TDA8007ReadAllRegs(void)
{
  uint8_t b;

  for(b=0; b<16; b++)
    abRegs[b] = TDA8007ReadReg(b);    
}
#endif

/*******************************************************************************
* Function Name  : TDA8007Init.
* Description    : Initialize TDA8007 registers.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007Init(void)
{
  uint8_t b;
  uint8_t bTemp;

  bTemp = TDA8007ReadReg(RegCardSelect);
  bTemp &= 0xF0;
  TDA8007WriteReg(RegCardSelect, bTemp);    // Clear RIU bit to reset UART registers.
  for(b=10; b>0; b--);
  bTemp |= 0x09;
  TDA8007WriteReg(RegCardSelect, bTemp);    // Set RIU bit to operate UART regisets.
                                            // Select Card 1.

  TDA8007WriteReg(RegClockConfiguration, 0x00);   // Set Clock is Fxtal.   
  TDA8007WriteReg(RegProgrammableDivider, 0x0C);  // Set 1 ETU = 31*12 clk.
  TDA8007WriteReg(RegUartConfiguration1, 0x02);   // Software convention Setting.
  TDA8007WriteReg(RegUartConfiguration2, 0x20);   // Disbale auxiliary interrupt.
//   TDA8007WriteReg(RegGuardTime, 0x02);
 TDA8007WriteReg(RegGuardTime, 0x00);//CNN 修改，不增加额外的2个ETU
  TDA8007WriteReg(RegFIFOControl, 0x70);    // Parity counter is 7, FIFO length is 1.

#ifdef TDA8007_DEBUG
  TDA8007ReadAllRegs();
#endif
}

/*******************************************************************************
* Function Name  : TDA8007SetTmo.
* Description    : Set TDA8007 timeout process.
* Input          : TmoValue
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007SetTmo(uint32_t TmoValue)
{
	TIM1Config(TmoValue);
}

/*
void TDA8007SetTmo(uint32_t TmoValue)
{
  uint8_t b;
  
  if(bTimeoutOpen)
    TDA8007StopTmo();
  b = (uint8_t)(TmoValue&0xFF);
  TDA8007WriteReg(RegTimeout1, b);
  b = (uint8_t)((TmoValue&0xFF00)>>8);
  TDA8007WriteReg(RegTimeout2, b);
  b = (uint8_t)((TmoValue&0xFF0000)>>16);
  TDA8007WriteReg(RegTimeout3, b); 
}
*/
/*******************************************************************************
* Function Name  : TDA8007StartTmo.
* Description    : Start TDA8007 timeout counter.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007StartTmo(void)
{
//	bTimeoutOpen = 1;
	TIM1Start();
}


/*
void TDA8007StartTmo(void)
{
  while((TDA8007ReadReg(RegMixedStatus)&0x10) == 0);    // Check control ready bit.
  TDA8007WriteReg(RegTimeoutConfiguration, 0x68);
  bTimeoutOpen = 1;
}
*/
/*******************************************************************************
* Function Name  : TDA8007StopTmo.
* Description    : Stop TDA8007 timeout counter.
* Input          : TmoValue
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007StopTmo(void)
{
	TIM1Stop();
//	bTimeoutOpen = 0;
}
/*
void TDA8007StopTmo(void)
{
  while((TDA8007ReadReg(RegMixedStatus)&0x10) == 0);    // Check control ready bit.
  TDA8007WriteReg(RegTimeoutConfiguration, 0x00);
  bTimeoutOpen = 0;
}

*/

/*******************************************************************************
* Function Name  : TDA8007PowerOn.
* Description    : TDA8007 power on process.
* Input          : CardVcc  : The source voltage of card.
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007PowerOn(uint8_t CardVcc)
{
  TDA8007Init(); 
  if(CardVcc == CARD_VCC_5V) 
  {
    TDA8007WriteReg(RegPowerControl, 0x01);
  }
  else if(CardVcc == CARD_VCC_3V)
  {
    TDA8007WriteReg(RegPowerControl, 0x03);    
  }
  else if(CardVcc == CARD_VCC_18V)
  {
    TDA8007WriteReg(RegPowerControl, 0x09);
//    TDA8007WriteReg(RegPowerControl, 0x03);
//    TDA8007WriteReg(RegPowerControl, 0x01);
  }
  bIsCTon = TRUE;
#ifdef TDA8007_DEBUG
  TDA8007ReadAllRegs();
#endif
}

/*******************************************************************************
* Function Name  : TDA8007ResetPin.
* Description    : TDA8007 reset pin value.
* Input          : BitVal  : The logic value of reset pin.
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007ResetPin(BitAction BitVal)
{
  uint8_t b;

  b = TDA8007ReadReg(RegPowerControl);
  if(BitVal == Bit_RESET)
    b &= 0xFB;
  else
    b |= 0x04;
  TDA8007WriteReg(RegPowerControl, b);
}

/*******************************************************************************
* Function Name  : TDA8007PowerOff.
* Description    : TDA8007 power off process.
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007PowerOff(void)
{
  uint8_t b;
  if(bTimeoutOpen)
    TDA8007StopTmo();
  TDA8007WriteReg(RegCardSelect, 0x01);
  b =  TDA8007ReadReg(RegPowerControl);
  b = b&0xFE;
  TDA8007WriteReg(RegPowerControl, b);

  bIsCTon = FALSE;

//  TDA8007WriteReg(RegTimeoutConfiguration, 0);
//  TDA8007StopTmo();  
}

/*******************************************************************************
* Function Name  : TDA8007PPS.
* Description    : TDA8007 PPS process to change RegProgrammableDivider.
* Input          : PPS1  : PPS1 value in PPS request. PSC and PDR value is showed in TDA8007 spec
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007PPS(uint8_t PPS1)
{
  uint8_t b;

  b = TDA8007ReadReg(RegUartConfiguration2); //bit0 is the PSC config bit , 1 for PSC=32 , 0 for PSC=31

  if(((PPS1>>4)==0)||((PPS1>>4)==1))	 //Fi=0,1这一列的参数
  {
	b &= 0xFE;	  //reset PSC config bit for PSC = 31
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
	    TDA8007WriteReg(RegProgrammableDivider, 12/(1<<((PPS1&0x0F)-1)));
	else if((PPS1&0x0F)==8)
	    TDA8007WriteReg(RegProgrammableDivider, 1);
  }
  else if((PPS1>>4)==2)	 //Fi=2这一列的参数
  {
	b &= 0xFE;	  //reset PSC config bit for PSC = 31
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
	    TDA8007WriteReg(RegProgrammableDivider, 18/(1<<((PPS1&0x0F)-1)));
  }
  else if((PPS1>>4)==3)	 //Fi=3这一列的参数
  {
	b &= 0xFE;	  //reset PSC config bit for PSC = 31
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
	    TDA8007WriteReg(RegProgrammableDivider, 24/(1<<((PPS1&0x0F)-1)));
	else if((PPS1&0x0F)==8)
	    TDA8007WriteReg(RegProgrammableDivider, 2);
  }
  else if((PPS1>>4)==4)	 //Fi=4这一列的参数
  {
	b &= 0xFE;	  //reset PSC config bit for PSC = 31
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
	    TDA8007WriteReg(RegProgrammableDivider, 36/(1<<((PPS1&0x0F)-1)));
	else if((PPS1&0x0F)==8)
	    TDA8007WriteReg(RegProgrammableDivider, 3);
  }
  else if((PPS1>>4)==5)	 //Fi=5这一列的参数
  {
	b &= 0xFE;	  //reset PSC config bit for PSC = 31
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 48/(1<<((PPS1&0x0F)-1)));	
	else if((PPS1&0x0F)==8)
    	TDA8007WriteReg(RegProgrammableDivider, 4);	  //31*4 = 279 	
  }
  else if((PPS1>>4)==6)	 
  {
	b &= 0xFE;	  //reset PSC config bit for PSC = 31
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 60/(1<<((PPS1&0x0F)-1)));	 	
	else if((PPS1&0x0F)==8)
    	TDA8007WriteReg(RegProgrammableDivider, 5);	  
	else if((PPS1&0x0F)==9)
    	TDA8007WriteReg(RegProgrammableDivider, 3);	  	
  }
  else if((PPS1>>4)==9)	 //Fi=9这一列的参数
  {
	b |= 0x01;	  //set PSC config bit for PSC = 32
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 16/(1<<((PPS1&0x0F)-1)));	  	
  }
  else if((PPS1>>4)==10)	 //Fi=10这一列的参数
  {
	b |= 0x01;	  //set PSC config bit for PSC = 32
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 24/(1<<((PPS1&0x0F)-1)));	  //32*24/(1<<((PPS1&0x0F)-1))  	
	else if((PPS1&0x0F)==8)
    	TDA8007WriteReg(RegProgrammableDivider, 2);	   	
  }
  else if((PPS1>>4)==11)	 //Fi=11这一列的参数
  {
	b |= 0x01;	  //set PSC config bit for PSC = 32
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 32/(1<<((PPS1&0x0F)-1)));	  //32*32/(1<<((PPS1&0x0F)-1))  	
  }
  else if((PPS1>>4)==12)	 //Fi=11这一列的参数
  {
	b |= 0x01;	  //set PSC config bit for PSC = 32
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 48/(1<<((PPS1&0x0F)-1)));	  //32*48/(1<<((PPS1&0x0F)-1))  	
	else if((PPS1&0x0F)==8)
    	TDA8007WriteReg(RegProgrammableDivider, 4);	  //32*4 = 279 	
  }
  else if((PPS1>>4)==13)	 //Fi=13这一列的参数
  {
	b |= 0x01;	  //set PSC config bit for PSC = 32
    TDA8007WriteReg(RegUartConfiguration2, b);
	if((PPS1&0x0F)<8)
    	TDA8007WriteReg(RegProgrammableDivider, 64/(1<<((PPS1&0x0F)-1)));	  //32*64/(1<<((PPS1&0x0F)-1))  	
  }
}

/*******************************************************************************
* Function Name  : TDA8007SwitchClock.
* Description    : Change the clock source of TDA8007.
* Input          : ClkConfig  : Clock configuration of TDA8007.
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007SwitchClock(uint8_t ClkConfig)
{
  uint8_t b;

  b = TDA8007ReadReg(RegClockConfiguration);
  b &= 0xF8;
  b |= ClkConfig;
  TDA8007WriteReg(RegClockConfiguration, b);
  if(ClkConfig & CARD_CLK_INT_DIV_2)  // switch into internal clock
  {
    while((TDA8007ReadReg(RegMixedStatus)&0x80) == 0x80);
  }
  else  // switch into external clock
  {
    while((TDA8007ReadReg(RegMixedStatus)&0x80) == 0x00);
  }
}

/*******************************************************************************
* Function Name  : TDA8007ReadFIFO.
* Description    : Read the UART data register of TDA8007.
* Input          : None
* Output         : Value of UART data register.
* Return         : None
*******************************************************************************/
uint8_t TDA8007ReadFIFO(void)
{
  uint8_t b;

  while((TDA8007ReadReg(RegMixedStatus)&0x10) == 0);    // Check control ready bit.
  b = TDA8007ReadReg(RegUartReceive);
  return b;
}

/*******************************************************************************
* Function Name  : TDA8007WriteFIFO.
* Description    : Write the UART data register of TDA8007.
* Input          : Value  : Data will transmited. 
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007WriteFIFO(uint8_t Value)
{
  while((TDA8007ReadReg(RegMixedStatus)&0x10) == 0);    // Check control ready bit.
  TDA8007WriteReg(RegUartTransmit, Value);  
}

/*******************************************************************************
* Function Name  : TDA8007SetUCR1Bit.
* Description    : Write one bit of UART Configuration1 register of TDA8007.
* Input          : BitIndex  : Bit number of UCR1
                   BitValue  : 0 or 1
* Output         : None
* Return         : None
*******************************************************************************/
void TDA8007SetUCR1Bit(uint8_t BitIndex, uint8_t BitValue)
{
  uint8_t b;
  uint8_t bTemp = (1 << BitIndex);

  b = TDA8007ReadReg(RegUartConfiguration1);
  if(BitValue)
    b |= bTemp;
  else
    b &= (~bTemp);
  TDA8007WriteReg(RegUartConfiguration1, b);
}
