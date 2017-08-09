/* /////////////////////////////////////////////////////////////////////////////////////////////////
//                     Copyright (c) FMSH
///////////////////////////////////////////////////////////////////////////////////////////////// */

#ifndef LPCD_REGCTRL_H
#define LPCD_REGCTRL_H
extern unsigned char GetReg_Ext(unsigned char ExtRegAddr,unsigned char* ExtRegData);
extern unsigned char SetReg_Ext(unsigned char ExtRegAddr,unsigned char ExtRegData);
extern unsigned char ModifyReg_Ext(unsigned char ExtRegAddr,unsigned char mask,unsigned char set);
#endif

