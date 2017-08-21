/* /////////////////////////////////////////////////////////////////////////////////////////////////
//                     Copyright (c) FMSH
///////////////////////////////////////////////////////////////////////////////////////////////// */

#ifndef _FMREG_H
#define _FMREG_H

#define  PAGE0           0x00    //Page register in page 0
#define  COMMAND         0x01    //Contains Command bits, PowerDown bit and bit to switch receiver off.
#define  COMMIEN         0x02    //Contains Communication interrupt enable bits andbit for Interrupt inversion.
#define  DIVIEN          0x03    //Contains RfOn, RfOff, CRC and Mode Interrupt enable and bit to switch Interrupt pin to PushPull mode.
#define  COMMIRQ         0x04    //Contains Communication interrupt request bits.
#define  DIVIRQ          0x05    //Contains RfOn, RfOff, CRC and Mode Interrupt request.
#define  REGERROR        0x06    //Contains Protocol, Parity, CRC, Collision, Buffer overflow, Temperature and RF error flags.
#define  STATUS1         0x07    //Contains status information about Lo- and HiAlert, RF-field on, Timer, Interrupt request and CRC status.
#define  STATUS2         0x08    //Contains information about internal states (Modemstate),Mifare states and possibility to switch Temperature sensor off.
#define  FIFODATA        0x09    //Gives access to FIFO. Writing to register increments theFIFO level (register 0x0A), reading decrements it.
#define  FIFOLEVEL       0x0A    //Contains the actual level of the FIFO.     
#define  WATERLEVEL      0x0B    //Contains the Waterlevel value for the FIFO 
#define  CONTROL         0x0C    //Contains information about last received bits, Initiator mode bit, bit to copy NFCID to FIFO and to Start and stopthe Timer unit.
#define  BITFRAMING      0x0D    //Contains information of last bits to send, to align received bits in FIFO and activate sending in Transceive*/
#define  COLL            0x0E    //Contains all necessary bits for Collission handling 
#define  RFU0F           0x0F    //Currently not used.                                 
                               
                               
#define  PAGE1           0x10    //Page register in page 1
#define  MODE            0x11    //Contains bits for auto wait on Rf, to detect SYNC byte in NFC mode and MSB first for CRC calculation
#define  TXMODE          0x12    //Contains Transmit Framing, Speed, CRC enable, bit for inverse mode and TXMix bit.                            
#define  RXMODE          0x13    //Contains Transmit Framing, Speed, CRC enable, bit for multiple receive and to filter errors.                 
#define  TXCONTROL       0x14    //Contains bits to activate and configure Tx1 and Tx2 and bit to activate 100% modulation.                      
#define  TXAUTO          0x15    //Contains bits to automatically switch on/off the Rf and to do the collission avoidance and the initial rf-on.
#define  TXSEL           0x16    //Contains SigoutSel, DriverSel and LoadModSel bits.
#define  RXSEL           0x17    //Contains UartSel and RxWait bits.                 
#define  RXTRESHOLD      0x18    //Contains MinLevel and CollLevel for detection.    
#define  DEMOD           0x19    //Contains bits for time constants, hysteresis and IQ demodulator settings. 
#define  FELICANFC       0x1A    //Contains bits for minimum FeliCa length received and for FeliCa syncronisation length.
#define  FELICANFC2      0x1B    //Contains bits for maximum FeliCa length received.      
#define  MIFARE          0x1C    //Contains Miller settings, TxWait settings and MIFARE halted mode bit.
#define  MANUALRCV       0x1D    //Currently not used.                          
#define  RFU1E           0x1E    //Currently not used.                          
#define  SERIALSPEED     0x1F    //Contains speed settings for serila interface.
                               
                               
#define  PAGE2           0x20    //Page register in page 2 
#define  CRCRESULT1      0x21    //Contains MSByte of CRC Result.                
#define  CRCRESULT2      0x22    //Contains LSByte of CRC Result.                
#define  GSNLOADMOD      0x23    //Contains the conductance and the modulation settings for the N-MOS transistor only for load modulation (See difference to PHCS_BFL_JREG_GSN!). 
#define  MODWIDTH        0x24    //Contains modulation width setting.                    
#define  TXBITPHASE      0x25    //Contains TxBitphase settings and receive clock change.
#define  RFCFG           0x26    //Contains sensitivity of Rf Level detector, the receiver gain factor and the RfLevelAmp.
#define  GSN             0x27    //Contains the conductance and the modulation settings for the N-MOS transistor during active modulation (no load modulation setting!).
#define  CWGSP           0x28    //Contains the conductance for the P-Mos transistor.    
#define  MODGSP          0x29    //Contains the modulation index for the PMos transistor.
#define  TMODE           0x2A    //Contains all settings for the timer and the highest 4 bits of the prescaler.
#define  TPRESCALER      0x2B    //Contais the lowest byte of the prescaler.   
#define  TRELOADHI       0x2C    //Contains the high byte of the reload value. 
#define  TRELOADLO       0x2D    //Contains the low byte of the reload value.  
#define  TCOUNTERVALHI   0x2E    //Contains the high byte of the counter value.
#define  TCOUNTERVALLO   0x2F    //Contains the low byte of the counter value. 
                               
                               
#define  PAGE3           0x30    //Page register in page 3
#define  TESTSEL1        0x31    //Test register                              
#define  TESTSEL2        0x32    //Test register                              
#define  TESTPINEN       0x33    //Test register                              
#define  TESTPINVALUE    0x34    //Test register                              
#define  TESTBUS         0x35    //Test register                              
#define  AUTOTEST        0x36    //Test register                              
#define  VERSION         0x37    //Contains the product number and the version .
#define  ANALOGTEST      0x38    //Test register                              
#define  TESTDAC1        0x39    //Test register                              
#define  TESTDAC2        0x3A    //Test register                              
#define  TESTADC         0x3B    //Test register                              
#define  ANALOGUETEST1   0x3C    //Test register                              
#define  ANALOGUETEST0   0x3D    //Test register                              
#define  ANALOGUETPD_A   0x3E    //Test register                              
#define  ANALOGUETPD_B   0x3F    //Test register                              

#define  CMD_MASK          0xF0
/* /////////////////////////////////////////////////////////////////////////////
 * Possible commands
 * ////////////////////////////////////////////////////////////////////////// */
#define     CMD_IDLE          0x00
#define     CMD_CONFIGURE     0x01 
#define     CMD_GEN_RAND_ID   0x02
#define     CMD_CALC_CRC      0x03 

//注意：如果启动Transmit命令时，数据已经在FIFO里了，数据会被立即发送；在启动Transmit命令后可以继续写数据到FIFO；
#define     CMD_TRANSMIT      0x04 

#define     CMD_NOCMDCHANGE   0x07 

//使能接收器电路；状态机会等待RxWait配置的时间后，接收器才真正开始工作；在接收过程中，也可以从FIFO里读取数据；
#define     CMD_RECEIVE       0x08 

//在TRANSCEIVE下，只有当StartSend置起后，数据才会被发送；
//作为读卡器时：将FIFO内的数据发送给卡片，然后等待RxWait后进入接收状态；
//作为卡片时： 等待收到数据后，进入Transmit状态，软件可以写将FIFO内的数据读取后再写数据进FIFO,然后启动发送（置起StartSend）；
#define     CMD_TRANSCEIVE    0x0C 

#define     CMD_AUTOCOLL      0x0D /*Activates automatic anticollision in Target mode. Data from Config command is used.
                       Remark: </strong>Activate CRC before,(Mifare does it's own settings for CRC)*/
#define     CMD_AUTHENT       0x0E /*Perform the card authentication using the Crypto1 algorithm.*/
#define     CMD_SOFT_RESET    0x0F /*Runs the Reset- and Initialisation Phase
                       Remark:This command can be activated by software, but only by a Power-On or Hard Reset*/

/*  Defintion for special transceive command, which uses only timeout to terminate!
 *  This is especially used for the FeliCa Polling command because there only a 
 *  timeout is valid to terminate for a slow interface!*/
#define     CMD_TRANSCEIVE_TO       0x8C

/* /////////////////////////////////////////////////////////////////////////////
 * Bit Definitions
 * ////////////////////////////////////////////////////////////////////////// */
/*name FM175xx Bit definitions of Page 0 */
/* Command Register							(01) */
#define     PHCS_BFL_JBIT_RCVOFF             0x20   /*Switches the receiver on/off. */
#define     PHCS_BFL_JBIT_POWERDOWN          0x10   /*Switches FM175xx to Power Down mode. */

/* CommIEn Register							(02) */
#define     PHCS_BFL_JBIT_IRQINV             0x80   /*Inverts the output of IRQ Pin. */

/* DivIEn Register							(03) */
#define     PHCS_BFL_JBIT_IRQPUSHPULL        0x80   /*Sets the IRQ pin to Push Pull mode. */

/* CommIEn and CommIrq Register         (02, 04) */
#define     PHCS_BFL_JBIT_TXI                0x40   /*Bit position for Transmit Interrupt Enable/Request.*/
#define     PHCS_BFL_JBIT_RXI                0x20   /*Bit position for Receive Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_IDLEI              0x10   /*Bit position for Idle Interrupt Enable/Request.    */
#define     PHCS_BFL_JBIT_HIALERTI           0x08   /*Bit position for HiAlert Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_LOALERTI           0x04   /*Bit position for LoAlert Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_ERRI               0x02   /*Bit position for Error Interrupt Enable/Request.   */
#define     PHCS_BFL_JBIT_TIMERI             0x01   /*Bit position for Timer Interrupt Enable/Request.   */

/* DivIEn and DivIrq Register           (03, 05) */
#define     PHCS_BFL_JBIT_SIGINACT           0x10   /*Bit position for SiginAct Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_MODEI              0x08   /*Bit position for Mode Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_CRCI               0x04   /*Bit position for CRC Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_RFONI              0x02   /*Bit position for RF On Interrupt Enable/Request. */
#define     PHCS_BFL_JBIT_RFOFFI             0x01   /*Bit position for RF Off Interrupt Enable/Request. */

/* CommIrq and DivIrq Register          (04, 05) */
#define     PHCS_BFL_JBIT_SET                0x80   /*Bit position to set/clear dedicated IRQ bits. */

/* Error Register 							(06) */
#define     PHCS_BFL_JBIT_WRERR              0x40   /*Bit position for Write Access Error. */
#define     PHCS_BFL_JBIT_TEMPERR            0x40   /*Bit position for Temerature Error. */
#define     PHCS_BFL_JBIT_RFERR              0x20   /*Bit position for RF Error. */
#define     PHCS_BFL_JBIT_BUFFEROVFL         0x10   /*Bit position for Buffer Overflow Error. */
#define     PHCS_BFL_JBIT_COLLERR            0x08   /*Bit position for Collision Error. */
#define     PHCS_BFL_JBIT_CRCERR             0x04   /*Bit position for CRC Error. */
#define     PHCS_BFL_JBIT_PARITYERR          0x02   /*Bit position for Parity Error. */
#define     PHCS_BFL_JBIT_PROTERR            0x01   /*Bit position for Protocol Error. */

/* Status 1 Register 						(07) */
#define     PHCS_BFL_JBIT_CRCOK              0x40   /*Bit position for status CRC OK. */
#define     PHCS_BFL_JBIT_CRCREADY           0x20   /*Bit position for status CRC Ready. */
#define     PHCS_BFL_JBIT_IRQ                0x10   /*Bit position for status IRQ is active. */
#define     PHCS_BFL_JBIT_TRUNNUNG           0x08   /*Bit position for status Timer is running. */
#define     PHCS_BFL_JBIT_RFON               0x04   /*Bit position for status RF is on/off. */
#define     PHCS_BFL_JBIT_HIALERT            0x02   /*Bit position for status HiAlert. */
#define     PHCS_BFL_JBIT_LOALERT            0x01   /*Bit position for status LoAlert. */

/* Status 2 Register				    	(08) */
#define     PHCS_BFL_JBIT_TEMPSENSOFF        0x80   /*Bit position to switch Temperture sensors on/off. */
#define     PHCS_BFL_JBIT_I2CFORCEHS         0x40   /*Bit position to forece High speed mode for I2C Interface. */
#define     PHCS_BFL_JBIT_MFSELECTED         0x10   /*Bit position for card status Mifare selected. */
#define     PHCS_BFL_JBIT_CRYPTO1ON          0x08   /*Bit position for reader status Crypto is on. */

/* FIFOLevel Register				    	(0A) */
#define     PHCS_BFL_JBIT_FLUSHFIFO          0x80   /*Clears FIFO buffer if set to 1 */

/* Control Register					    	(0C) */
#define     PHCS_BFL_JBIT_TSTOPNOW           0x80   /*Stops timer if set to 1. */
#define     PHCS_BFL_JBIT_TSTARTNOW          0x40   /*Starts timer if set to 1. */
#define     PHCS_BFL_JBIT_WRNFCIDTOFIFO      0x20   /*Copies internal stored NFCID3 to actual position of FIFO. */
#define     PHCS_BFL_JBIT_INITIATOR          0x10   /*Sets Initiator mode. */

/* BitFraming Register					    (0D) */
#define     PHCS_BFL_JBIT_STARTSEND          0x80   /*Starts transmission in transceive command if set to 1. */

/* BitFraming Register					    (0E) */
#define     PHCS_BFL_JBIT_VALUESAFTERCOLL    0x80   /*Activates mode to keep data after collision. */

/*name FM175xx Bit definitions of Page 1
 *  Below there are useful bit definition of the FM175xx register set of Page 1.*/
/* Mode Register							(11) */
#define     PHCS_BFL_JBIT_MSBFIRST           0x80   /*Sets CRC coprocessor with MSB first. */
#define     PHCS_BFL_JBIT_DETECTSYNC         0x40   /*Activate automatic sync detection for NFC 106kbps. */
#define     PHCS_BFL_JBIT_TXWAITRF           0x20   /*Tx waits until Rf is enabled until transmit is startet, else 
                                                transmit is started immideately. */
#define     PHCS_BFL_JBIT_RXWAITRF           0x10   /*Rx waits until Rf is enabled until receive is startet, else 
                                                receive is started immideately. */
#define     PHCS_BFL_JBIT_POLSIGIN           0x08   /*Inverts polarity of SiginActIrq, if bit is set to 1 IRQ occures
                                                when Sigin line is 0. */
#define     PHCS_BFL_JBIT_MODEDETOFF         0x04   /*Deactivates the ModeDetector during AutoAnticoll command. The settings
                                                of the register are valid only. */

/* TxMode Register							(12) */
#define     PHCS_BFL_JBIT_TXCRCEN            0x80  /*enables the CRC generation during data transmissio. */
#define     PHCS_BFL_JBIT_INVMOD             0x08   /*Activates inverted transmission mode. */
#define     PHCS_BFL_JBIT_TXMIX              0x04   /*Activates TXMix functionality. */

/* RxMode Register							(13) */
#define     PHCS_BFL_JBIT_RXCRCEN            0x80   /*enables the CRC generation during reception.. */
#define     PHCS_BFL_JBIT_RXNOERR            0x08   /*If 1, receiver does not receive less than 4 bits. */
#define     PHCS_BFL_JBIT_RXMULTIPLE         0x04   /*Activates reception mode for multiple responses. */

/* Definitions for Tx and Rx		    (12, 13) */
#define     PHCS_BFL_JBIT_106KBPS            0x00   /*Activates speed of 106kbps. */
#define     PHCS_BFL_JBIT_212KBPS            0x10   /*Activates speed of 212kbps. */
#define     PHCS_BFL_JBIT_424KBPS            0x20   /*Activates speed of 424kbps. */
#define     PHCS_BFL_JBIT_848KBPS            0x30   /*Activates speed of 848kbps. */
#define     PHCS_BFL_JBIT_1_6MBPS            0x40   /*Activates speed of 1.6Mbps. */
#define     PHCS_BFL_JBIT_3_2MBPS            0x50   /*Activates speed of 3_3Mbps. */

#define     PHCS_BFL_JBIT_MIFARE             0x00   /*Activates Mifare communication mode. */
#define     PHCS_BFL_JBIT_NFC                0x01   /*Activates NFC/Active communication mode. */
#define     PHCS_BFL_JBIT_FELICA             0x02   /*Activates FeliCa communication mode. */

#define     PHCS_BFL_JBIT_CRCEN              0x80   /*Activates transmit or receive CRC. */

/* TxControl Register						(14) */
#define     PHCS_BFL_JBIT_INVTX2ON           0x80   /*Inverts the Tx2 output if drivers are switched on. */
#define     PHCS_BFL_JBIT_INVTX1ON           0x40   /*Inverts the Tx1 output if drivers are switched on. */
#define     PHCS_BFL_JBIT_INVTX2OFF          0x20   /*Inverts the Tx2 output if drivers are switched off. */
#define     PHCS_BFL_JBIT_INVTX1OFF          0x10   /*Inverts the Tx1 output if drivers are switched off. */
#define     PHCS_BFL_JBIT_TX2CW              0x08   /*Does not modulate the Tx2 output, only constant wave. */
#define     PHCS_BFL_JBIT_CHECKRF            0x04   /*Does not activate the driver if an external RF is detected.
                                                Only valid in combination with PHCS_BFL_JBIT_TX2RFEN and PHCS_BFL_JBIT_TX1RFEN. */
#define     PHCS_BFL_JBIT_TX2RFEN            0x02   /*Switches the driver for Tx2 pin on. */
#define     PHCS_BFL_JBIT_TX1RFEN            0x01   /*Switches the driver for Tx1 pin on. */

/* TxAuto Register							(15) */
#define     PHCS_BFL_JBIT_AUTORFOFF          0x80   /*Switches the RF automatically off after transmission is finished. */
#define     PHCS_BFL_JBIT_FORCE100ASK        0x40   /*Activates 100%ASK mode independent of driver settings. */
#define     PHCS_BFL_JBIT_AUTOWAKEUP         0x20   /*Activates automatic wakeup of the FM175xx if set to 1. */
#define     PHCS_BFL_JBIT_CAON               0x08   /*Activates the automatic time jitter generation by switching 
                                                on the Rf field as defined in ECMA-340. */
#define     PHCS_BFL_JBIT_INITIALRFON        0x04   /*Activate the initial RF on procedure as defined iun ECMA-340. */
#define     PHCS_BFL_JBIT_TX2RFAUTOEN        0x02   /*Switches on the driver two automatically according to the 
                                                other settings. */
#define     PHCS_BFL_JBIT_TX1RFAUTOEN        0x01   /*Switches on the driver one automatically according to the 
                                                other settings. */

/* Demod Register 							(19) */
#define     PHCS_BFL_JBIT_FIXIQ              0x20   /*If set to 1 and the lower bit of AddIQ is set to 0, the receiving is fixed to I channel.
                                                If set to 1 and the lower bit of AddIQ is set to 1, the receiving is fixed to Q channel. */

/* Felica/NFC 2 Register 				    (1B) */
#define     PHCS_BFL_JBIT_WAITFORSELECTED    0x80   /*If this bit is set to one, only passive communication modes are possible.
                                                In any other case the AutoColl Statemachine does not exit. */
#define     PHCS_BFL_JBIT_FASTTIMESLOT       0x40   /*If this bit is set to one, the response time to the polling command is half as normal. */

/* Mifare Register 							(1C) */
#define     PHCS_BFL_JBIT_MFHALTED           0x04   /*Configures the internal state machine only to answer to
                                                Wakeup commands according to ISO 14443-3. */

/* RFU 0x1D Register 					    (1D) */
#define     PHCS_BFL_JBIT_PARITYDISABLE      0x10   /*Disables the parity generation and sending independent from the mode. */
#define     PHCS_BFL_JBIT_LARGEBWPLL         0x08   /* */
#define     PHCS_BFL_JBIT_MANUALHPCF         0x04   /* */
/*@}*/

/*! \name FM175xx Bit definitions of Page 2
 *  Below there are useful bit definition of the FM175xx register set.
 */
/* TxBitPhase Register 					    (25) */
#define     PHCS_BFL_JBIT_RCVCLKCHANGE       0x80   /*If 1 the receive clock may change between Rf and oscilator. */

/* RfCFG Register 							(26) */
#define     PHCS_BFL_JBIT_RFLEVELAMP         0x80   /*Activates the RF Level detector amplifier. */

/* TMode Register 							(2A) */
#define     PHCS_BFL_JBIT_TAUTO              0x80   /*Sets the Timer start/stop conditions to Auto mode. */
#define     PHCS_BFL_JBIT_TAUTORESTART       0x10   /*Restarts the timer automatically after finished
                                                counting down to 0. */
/*@}*/

/*! \name FM175xx Bit definitions of Page 3
 *  Below there are useful bit definition of the FM175xx register set.
 */
/* AutoTest Register 					    (36) */
#define     PHCS_BFL_JBIT_AMPRCV             0x40   /* */
/*@}*/


/* /////////////////////////////////////////////////////////////////////////////
 * Bitmask Definitions
 * ////////////////////////////////////////////////////////////////////////// */
/*! \name FM175xx Bitmask definitions
 *  Below there are some useful mask defintions for the FM175xx. All specified 
 *  bits are set to 1.
 */
/* Command register                 (0x01)*/
#define     PHCS_BFL_JMASK_COMMAND           0x0F   /*Bitmask for Command bits in Register PHCS_BFL_JREG_COMMAND. */
#define     PHCS_BFL_JMASK_COMMAND_INV       0xF0   /*Inverted bitmask of PHCS_BFL_JMASK_COMMAND. */

/* Waterlevel register              (0x0B)*/
#define     PHCS_BFL_JMASK_WATERLEVEL        0x3F   /*Bitmask for Waterlevel bits in register PHCS_BFL_JREG_WATERLEVEL. */

/* Control register                 (0x0C)*/
#define     PHCS_BFL_JMASK_RXBITS            0x07   /*Bitmask for RxLast bits in register PHCS_BFL_JREG_CONTROL. */

/* Mode register                    (0x11)*/
#define     PHCS_BFL_JMASK_CRCPRESET         0x03   /*Bitmask for CRCPreset bits in register PHCS_BFL_JREG_MODE. */

/* TxMode register                  (0x12, 0x13)*/
#define     PHCS_BFL_JMASK_SPEED             0x70   /*Bitmask for Tx/RxSpeed bits in register PHCS_BFL_JREG_TXMODE and PHCS_BFL_JREG_RXMODE. */
#define     PHCS_BFL_JMASK_FRAMING           0x03   /*Bitmask for Tx/RxFraming bits in register PHCS_BFL_JREG_TXMODE and PHCS_BFL_JREG_RXMODE. */

/* TxSel register                   (0x16)*/
#define     PHCS_BFL_JMASK_LOADMODSEL        0xC0   /*Bitmask for LoadModSel bits in register PHCS_BFL_JREG_TXSEL. */
#define     PHCS_BFL_JMASK_DRIVERSEL         0x30   /*Bitmask for DriverSel bits in register PHCS_BFL_JREG_TXSEL. */
#define     PHCS_BFL_JMASK_SIGOUTSEL         0x0F   /*Bitmask for SigoutSel bits in register PHCS_BFL_JREG_TXSEL. */

/* RxSel register                   (0x17)*/
#define     PHCS_BFL_JMASK_UARTSEL           0xC0   /*Bitmask for UartSel bits in register PHCS_BFL_JREG_RXSEL. */
#define     PHCS_BFL_JMASK_RXWAIT            0x3F   /*Bitmask for RxWait bits in register PHCS_BFL_JREG_RXSEL. */

/* RxThreshold register             (0x18)*/
#define     PHCS_BFL_JMASK_MINLEVEL          0xF0   /*Bitmask for MinLevel bits in register PHCS_BFL_JREG_RXTHRESHOLD. */
#define     PHCS_BFL_JMASK_COLLEVEL          0x07   /*Bitmask for CollLevel bits in register PHCS_BFL_JREG_RXTHRESHOLD. */

/* Demod register                   (0x19)*/
#define     PHCS_BFL_JMASK_ADDIQ             0xC0   /*Bitmask for ADDIQ bits in register PHCS_BFL_JREG_DEMOD. */
#define     PHCS_BFL_JMASK_TAURCV            0x0C   /*Bitmask for TauRcv bits in register PHCS_BFL_JREG_DEMOD. */
#define     PHCS_BFL_JMASK_TAUSYNC           0x03   /*Bitmask for TauSync bits in register PHCS_BFL_JREG_DEMOD. */

/* FeliCa / FeliCa2 register        (0x1A, 0x1B)*/
#define     PHCS_BFL_JMASK_FELICASYNCLEN     0xC0   /*Bitmask for FeliCaSyncLen bits in registers PHCS_BFL_JREG_FELICANFC. */
#define     PHCS_BFL_JMASK_FELICALEN         0x3F   /*Bitmask for FeliCaLenMin and FeliCaLenMax in 
                                                registers PHCS_BFL_JREG_FELICANFC and PHCS_BFL_JREG_FELICANFC2. */
/* Mifare register                  (0x1C)*/
#define     PHCS_BFL_JMASK_SENSMILLER        0xE0   /*Bitmask for SensMiller bits in register PHCS_BFL_JREG_MIFARE. */
#define     PHCS_BFL_JMASK_TAUMILLER         0x18   /*Bitmask for TauMiller bits in register PHCS_BFL_JREG_MIFARE. */
#define     PHCS_BFL_JMASK_TXWAIT            0x03   /*Bitmask for TxWait bits in register PHCS_BFL_JREG_MIFARE. */

/* Manual Rcv register				(0x1D)*/
#define     PHCS_BFL_JMASK_HPCF				0x03   /*Bitmask for HPCF filter adjustments. */

/* TxBitPhase register              (0x25)*/
#define     PHCS_BFL_JMASK_TXBITPHASE        0x7F   /*Bitmask for TxBitPhase bits in register PHCS_BFL_JREG_TXBITPHASE. */

/* RFCfg register                   (0x26)*/
#define     PHCS_BFL_JMASK_RXGAIN            0x70   /*Bitmask for RxGain bits in register PHCS_BFL_JREG_RFCFG. */
#define     PHCS_BFL_JMASK_RFLEVEL           0x0F   /*Bitmask for RfLevel bits in register PHCS_BFL_JREG_RFCFG. */

/* GsN register                     (0x27)*/
#define     PHCS_BFL_JMASK_CWGSN             0xF0   /*Bitmask for CWGsN bits in register PHCS_BFL_JREG_GSN. */
#define     PHCS_BFL_JMASK_MODGSN            0x0F   /*Bitmask for ModGsN bits in register PHCS_BFL_JREG_GSN. */

/* CWGsP register                   (0x28)*/
#define     PHCS_BFL_JMASK_CWGSP             0x3F   /*Bitmask for CWGsP bits in register PHCS_BFL_JREG_CWGSP. */

/* ModGsP register                  (0x29)*/
#define     PHCS_BFL_JMASK_MODGSP            0x3F   /*Bitmask for ModGsP bits in register PHCS_BFL_JREG_MODGSP. */

/* TMode register                   (0x2A)*/
#define     PHCS_BFL_JMASK_TGATED            0x60   /*Bitmask for TGated bits in register PHCS_BFL_JREG_TMODE. */
#define     PHCS_BFL_JMASK_TPRESCALER_HI     0x0F   /*Bitmask for TPrescalerHi bits in register PHCS_BFL_JREG_TMODE. */

#endif /* PHCSBFLHW1REG_H */
