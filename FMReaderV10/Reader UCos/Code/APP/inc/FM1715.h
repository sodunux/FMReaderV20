/*
*         Copyright (c), Philips Semiconductors Gratkorn / Austria
*
*                     (C)PHILIPS Electronics N.V.2000
*       All rights are reserved. Reproduction in whole or in part is 
*      prohibited without the written consent of the copyright owner.
*  Philips reserves the right to make changes without notice at any time.
* Philips makes no warranty, expressed, implied or statutory, including but
* not limited to any implied warranty of merchantability or fitness for any
*particular purpose, or that the use will not infringe any third party patent,
* copyright or trademark. Philips must not be liable for any loss or damage
*                          arising from its use.
*/

#ifndef MFRC500_H
#define MFRC500_H

/*
#define READER_NCS_SET();         do{GPIO_WriteBit(FM1715_NCS_PORT, FM1715_NCS_PIN, Bit_SET);}while(0);
#define READER_NCS_RESET();       do{GPIO_WriteBit(FM1715_NCS_PORT, FM1715_NCS_PIN, Bit_RESET);}while(0);
#define READER_NRD_SET();         do{GPIO_WriteBit(FM1715_NRD_PORT, FM1715_NRD_PIN, Bit_SET);}while(0);
#define READER_NRD_RESET();       do{GPIO_WriteBit(FM1715_NRD_PORT, FM1715_NRD_PIN, Bit_RESET);}while(0);
#define READER_NWR_SET();         do{GPIO_WriteBit(FM1715_NWR_PORT, FM1715_NWR_PIN, Bit_SET);}while(0);
#define READER_NWR_RESET();       do{GPIO_WriteBit(FM1715_NWR_PORT, FM1715_NWR_PIN, Bit_RESET);}while(0);
#define READER_ALE_SET();         do{GPIO_WriteBit(FM1715_ALE_PORT, FM1715_ALE_PIN, Bit_SET);}while(0);
#define READER_ALE_RESET();       do{GPIO_WriteBit(FM1715_ALE_PORT, FM1715_ALE_PIN, Bit_RESET);}while(0);
#define READER_RST_SET();         do{GPIO_WriteBit(FM1715_RST_PORT, FM1715_RST_PIN, Bit_SET);}while(0);
#define READER_RST_RESET();       do{GPIO_WriteBit(FM1715_RST_PORT, FM1715_RST_PIN, Bit_RESET);}while(0);
*/
#define READER_NCS_SET();         do{FM1715_NCS_PORT->BSRR = FM1715_NCS_PIN;}while(0);
#define READER_NCS_RESET();       do{FM1715_NCS_PORT->BRR = FM1715_NCS_PIN;}while(0);
#define READER_NRD_SET();         do{FM1715_NRD_PORT->BSRR = FM1715_NRD_PIN;}while(0);
#define READER_NRD_RESET();       do{FM1715_NRD_PORT->BRR = FM1715_NRD_PIN;}while(0);
#define READER_NWR_SET();         do{FM1715_NWR_PORT->BSRR = FM1715_NWR_PIN;}while(0);
#define READER_NWR_RESET();       do{FM1715_NWR_PORT->BRR = FM1715_NWR_PIN;}while(0);
#define READER_ALE_SET();         do{FM1715_ALE_PORT->BSRR = FM1715_ALE_PIN;}while(0);
#define READER_ALE_RESET();       do{FM1715_ALE_PORT->BRR = FM1715_ALE_PIN;}while(0);
#define READER_RST_SET();         do{FM1715_RST_PORT->BSRR = FM1715_RST_PIN;}while(0);
#define READER_RST_RESET();       do{FM1715_RST_PORT->BRR = FM1715_RST_PIN;}while(0);

void SleepMs(unsigned short ms);
void SleepUs(unsigned short us);


// PCD Configuration
signed char Mf500PcdConfig(void);

// Active Antenna Slave Configuration of the MF RC500.
signed char Mf500ActiveAntennaSlaveConfig(void);

// Active Antenna Master Configuration of the MF RC 500
signed char Mf500ActiveAntennaMasterConfig(void);

// Set default attributes for the baudrate divider
signed char Mf500PcdSetDefaultAttrib(void);

// Set attributes for the baudrate divider 
signed char Mf500PcdSetAttrib(unsigned char DSI,
                           unsigned char DRI);

// Get transmission properties of the PCD
signed char Mf500PcdGetAttrib(unsigned char *FSCImax,
                          unsigned char *FSDImax,
                          unsigned char *DSsupp,
                          unsigned char *DRsupp,
                          unsigned char *DREQDS);

// PICC Request command
signed char Mf500PiccRequest(unsigned char req_code, 
                       unsigned char *atq);
                       
// PICC Request command for ISO 14443 A-4 Command set
signed char Mf500PiccCommonRequest(unsigned char req_code, 
                             unsigned char *atq);  

// PICC Anticollision Command
signed char Mf500PiccAnticoll (unsigned char bcnt,
                         unsigned char *snr);

// PICC Cascaded Anticollision Command
signed char Mf500PiccCascAnticoll (unsigned char select_code,
                             unsigned char bcnt,
                             unsigned char *snr);                     
signed char Mf500PiccAnticollWithBCC (unsigned char select_code,
                         	 unsigned char *snr); 
// PICC Select Command
signed char Mf500PiccSelect(unsigned char *snr, 
                      unsigned char *sak);

// PICC Select Command
signed char Mf500PiccCascSelect(unsigned char select_code, 
                             unsigned char *snr, 
                             unsigned char *sak,
                             unsigned char check_BCC); 

// Activation of a PICC in IDLE mode
signed char Mf500PiccActivateIdle(unsigned char br,
                           unsigned char *atq, 
                           unsigned char *sak, 
                           unsigned char *uid, 
                           unsigned char *uid_len);

// Activation of all PICC's in the RF field
signed char Mf500PiccActivateWakeup(unsigned char br,
                             unsigned char *atq, 
                             unsigned char *sak,
                             unsigned char *uid, 
                             unsigned char uid_len);

// MIFARE®  Authentication
signed char Mf500PiccAuth(unsigned char auth_mode, 
                      unsigned char key_sector, 
                      unsigned char block);   
  
// MIFARE ® Authentication with  keys stored  in the MF RC 500's EEPROM.
signed char Mf500PiccAuthE2( unsigned char auth_mode, 
                         unsigned char *snr,      
                         unsigned char key_sector,
                         unsigned char block); 

// Authentication Key Coding
signed char Mf500HostCodeKey(unsigned char *uncoded, 
                         unsigned char *coded); 

// Key Loading into the MF RC500's EEPROM.
signed char Mf500PcdLoadKeyE2(unsigned char key_type,
                          unsigned char sector,
                          unsigned char *uncoded_keys); 
                     
// Authentication with direct key loading form the microcontroller
signed char Mf500PiccAuthKey(unsigned char auth_mode,
                         unsigned char *snr,   
                         unsigned char *keys,  
                         unsigned char sector); 

// PICC Inc/Dec
signed char Mf500Picc_Inc_Dec(u8 cmd,u8 addr,u8 datalen,u8 *data);

// PICC Restore/Transfer
signed char Mf500Picc_Res_Tra(u8 cmd,u8 addr);
                     
// PICC Read Block
signed char Mf500PiccRead(unsigned char addr,  
                       unsigned char* data);

// PICC Read Block of variable length
signed char Mf500PiccCommonRead(unsigned char cmd,
                             unsigned char addr,  
                             unsigned char datalen,
                             unsigned char *data);
                  
// PICC Write Block
signed char Mf500PiccWrite(unsigned char addr,
                        unsigned char *data);

// PICC Write 4 Byte Block
signed char Mf500PiccWrite4(unsigned char addr,
                         unsigned char *data);
                       
// PICC Write Block of variable length
signed char Mf500PiccCommonWrite(unsigned char cmd,
                              unsigned char addr,
                              unsigned char datalen,
                              unsigned char *data);

// PICC Value Block Operation
signed char Mf500PiccValue(unsigned char dd_mode, 
                       unsigned char addr, 
                       unsigned char *value,
                       unsigned char trans_addr);

// PICC Value Block Operation for Cards with automatic transfer
signed char Mf500PiccValueDebit(unsigned char dd_mode, 
                             unsigned char addr, 
                             unsigned char *value);

// Exchange Data Blocks PCD --> PICC --> PCD
signed char Mf500PiccExchangeBlock(unsigned char *send_data,
                               unsigned short send_bytelen,
                               unsigned char *rec_data,  
                               unsigned short *rec_bytelen,
                               unsigned char append_crc, 
                               unsigned long timeout );                  

// PICC Halt
signed char Mf500PiccHalt(void);

// Reset the reader ic 
signed char PcdReset(void);

// Exchange Data Stream PCD --> PICC --> PCD
signed char ExchangeByteStream(unsigned char Cmd,
                            unsigned char *send_data,
                            unsigned short send_bytelen,
                            unsigned char *rec_data,  
                            unsigned short *rec_bytelen);

// Set RF communication timeout 
signed char PcdSetTmo(unsigned long numberOfEtus);

// Read Serial Number from Reader IC
signed char PcdGetSnr(unsigned char *snr);

// Read EEPROM Memory Block
signed char PcdReadE2(unsigned short startaddr,  
                   unsigned char length,
                   unsigned char* data);

// Writes data to the reader IC's EEPROM blocks.
signed char PcdWriteE2(  unsigned short startaddr,
                      unsigned char length,
                      unsigned char* data);

// Turns ON/OFF RF field
signed char PcdRfReset(unsigned short ms);  

void SingleResponseIsr(void); 

signed char  PcdStopRecvCmd(unsigned char *buffer, unsigned short *RecvLen);

void WriteRC(unsigned char Address, unsigned char value);
unsigned char ReadRC(unsigned char Address);

void ReadAllRegs(void);
signed char Mf500PiccReqB(unsigned char *send_data,
                        unsigned short send_bytelen,
                        unsigned char *rec_data,  
                        unsigned short *rec_bytelen);
signed char Mf500PiccWrite128( unsigned char addr,
                  unsigned char *data)	 ;
#endif
