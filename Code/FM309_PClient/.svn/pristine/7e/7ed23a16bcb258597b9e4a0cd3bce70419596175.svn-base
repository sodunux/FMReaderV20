using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.IO;
using PcSc;

namespace PcSc
{
    public class swp
    {      
        
        public string display = "";
        int ComSlaveCmdCounter = 1, ComExtMcuCmdCounter = 1;
        int SlaveComRecvIndex, ExtMcuComRecvIndex;
        bool SlaveComRecvOkFlag, ExtMcuComRecvOkFlag ;
        byte[] ComSlaveRecvBufData ;
        byte[] ComExtMcuRecvBufData;
//        string commdebug;

       
        private SerialPort serialPort1 = new SerialPort();
        private SerialPort serialPort2 = new SerialPort();
        
        /*Delay 函数*/
        public static void Delay(int second)
        {
            int num = 0;
            bool flag = false;
            DateTime now = DateTime.Now;
            while (flag)
            {
                num++;
                flag = (now.AddSeconds(second) > DateTime.Now);
            }
        }
         /*Delay 函数*/
        public virtual int ComWaits(int seconds,int ComFlag)
        {
            for (int i = 0; i < seconds; i++)
            {
                Delay(1);
                if (((ComFlag == 0) && ExtMcuComRecvOkFlag) || ((ComFlag == 2) && SlaveComRecvOkFlag))
                    return 0;                //接收到回发数据，正确返回；      
            }
            return 1;        
        }

        public static string DeleteSpaceString(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            hexString = hexString.Replace("\t", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;  //如果最后不足两位，最后添“0”。

            return hexString;
        }

        public static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;  //如果最后不足两位，最后添“0”。
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// byteToHexStr(int len, byte[] bytes)将bytes数组中的前面len个元素转化为string
        /// </summary>
        /// <param name="len"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(int len, byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < len; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /*打开串口函数*/
        public virtual int CommOpen(string PortName, Int32 BaudRate, int ComSelect, int ComState, out string StrReceived)
        {
            if (ComSelect == 0)//ext mcu comm
            {
                if (ComState == 0)//打开串口
                {
                  
                    //serialPort1 = new SerialPort(PortName);            
                    serialPort1.PortName = PortName;
                    serialPort1.BaudRate = BaudRate;
                    serialPort1.Parity = Parity.None;
                    serialPort1.StopBits = StopBits.Two;
                    serialPort1.ReceivedBytesThreshold = 8;
                    //serialPort1.DataReceived += new SerialDataReceivedEventHandler(MasterCommDataReceived);
                    if (serialPort1.IsOpen)
                    {
                        StrReceived = "comm_err";
                        return 1;
                    }
                    serialPort1.Open();
                    
                }
                else//关闭串口
                {
                    serialPort1.Close();
                    
                }
            }
            else if (ComSelect == 1)// slave comm
            {
                if (ComState == 0)//打开串口
                {
                    //if (PortName==serialPort2.PortName)
                    //serialPort2 = new SerialPort(PortName)
                    serialPort2.PortName = PortName; ;                
                    serialPort2.BaudRate = BaudRate;
                    serialPort2.Parity = Parity.None;
                    serialPort2.StopBits = StopBits.Two;
                    serialPort2.ReceivedBytesThreshold = 8;
 //                   serialPort2.DataReceived += new SerialDataReceivedEventHandler(SlaveCommCommDataReceived);
                
                    if (serialPort2.IsOpen)
                    {
                        StrReceived = "comm_err";
                        return 1;
                    }
                    else
                    serialPort2.Open();                    
                    
                }
                else//关闭串口
                {
                    serialPort2.Close();                    
                }
            }
            else
            {
                StrReceived = "Error";
                return 1;
            }
            StrReceived = "Succeeded";
            return 0;
        }
        public void MasterCommDataReceived(out byte[] RecvBufData)//object sender, SerialDataReceivedEventArgs e)
        {
            int i, len, temrecvdatalen=0;
            byte readbyte, temp = 0;        
            int datalength;
            while(serialPort1.BytesToRead == 0); 
            Delay(80);
            if (serialPort1.BytesToRead != 0)	             // 是否有字符驻留在接收缓冲区等待被取出
            {
                len = serialPort1.BytesToRead;
                temrecvdatalen = ExtMcuComRecvIndex;
                if (ExtMcuComRecvIndex == 0)                   //帧的第一笔数据
               {
                    ComExtMcuRecvBufData = new byte[len + 2];
                    
                    for (i = 0; i < len; i++)
                    {
                        readbyte = strToHexByte(serialPort1.ReadByte().ToString("X2"))[0];                        
                        if (readbyte==0x02||temp == 0x02)
                        {
                           //第一Byte数据如果不是0x2，不收 
                           ComExtMcuRecvBufData[temrecvdatalen++] = readbyte;
                           temp = 0x02;
                        }
                    }
                    ExtMcuComRecvIndex = temrecvdatalen;
                }
                else
                {
                    ComExtMcuRecvBufData = new byte[temrecvdatalen+len];
                    for (i = 0; i < len; i++)
                    {
                        readbyte = strToHexByte(serialPort1.ReadByte().ToString("X2"))[0];                        
                        ComExtMcuRecvBufData[temrecvdatalen + i] = readbyte;
                    }
                    ExtMcuComRecvIndex += len;
                }
                
                if (ExtMcuComRecvIndex > 4)// 除去0x02 numberofCMD bcc 0x03之外 是否还有数据
                {
                    datalength = ComExtMcuRecvBufData[3];//数据长度
                    if (ExtMcuComRecvIndex >= datalength + 6)
                    {
                        ExtMcuComRecvOkFlag = true;             //接收数据结束,置起标志
                        ExtMcuComRecvIndex = 0;
                    }                    
                }
            }
            RecvBufData = ComExtMcuRecvBufData;

        }

        public void SlaveCommCommDataReceived(out byte[] RecvBufData)
        {
            int i, len, temrecvdatalen,temp = 0;
            byte readbyte;            
            int datalength;
            //while (serialPort2.BytesToRead == 0) ;   
            Delay(80);
            if (serialPort2.BytesToRead != 0)	             // 是否有字符驻留在接收缓冲区等待被取出
            {
                len = serialPort2.BytesToRead;
                temrecvdatalen = 0;               
                for (i = 0; i < len; i++)
                {
                    ComSlaveRecvBufData = new byte[len + 2];
                    readbyte = strToHexByte(serialPort2.ReadByte().ToString("X2"))[0];
                    if (readbyte == 0x02 || temp == 0x02)
                    {
                        //第一Byte数据如果不是0x2，不收 
                        ComSlaveRecvBufData[temrecvdatalen++] = readbyte;
                        temp = 0x02;
                    }                                      
                }
                SlaveComRecvIndex = temrecvdatalen;
                
                if (SlaveComRecvIndex > 4)// 除去0x02 numberofCMD bcc 0x03之外 是否还有数据
                {
                    datalength = ComSlaveRecvBufData[3];//数据长度
                    if (SlaveComRecvIndex == datalength + 6)
                    {
                        SlaveComRecvOkFlag = true;             //接收数据结束,置起标志
                        SlaveComRecvIndex = 0;
                    }
                    /*else if (SlaveComRecvIndex > datalength + 6)
                    {
                        display = "SWP SLAVE 收到数据：--错误帧或不完整的帧    ";
                        
                        DisplayMessageLine(display);
                        SlaveComRecvIndex = 0;
                        return;
                    }*/
                }
            }
            RecvBufData = ComSlaveRecvBufData;
        }

        public virtual int CheckComRecv(int CommHandle, out string str)
        {
            int RetCode, i;
            int bcc, len, cmdnumb = 0;
            byte[] ptrrecvbuf;
            if (CommHandle == 1)
            {
                ptrrecvbuf = new byte[ComSlaveRecvBufData.Length];
                ptrrecvbuf = ComSlaveRecvBufData;
                cmdnumb = ComSlaveCmdCounter - 1;
            }
            else
            {
                ptrrecvbuf = new byte[ComExtMcuRecvBufData.Length];
                ptrrecvbuf = ComExtMcuRecvBufData;
                cmdnumb = ComExtMcuCmdCounter - 1;
            }             
            
            len = ptrrecvbuf[3];

            if (ptrrecvbuf[0] != 0x02)         	                  //Read STX
            {
                str = "MIS_COMMERR";
                return 1;
            }
            else
            {
                if (ptrrecvbuf[1] != cmdnumb)  //Read SNR
                {
                    str = "MIS_COMMERR";
                    return 1;
                }
                else
                {
                    i = ptrrecvbuf[2];                       //错误信息
                    if (i != 0)
                    {
                        RetCode = ptrrecvbuf[2];
                        str = "MIS_CMDERR";
                        return (RetCode);
                    }
                    else
                    {
                        len += 0X03;
                        bcc = 0;
                        for (i = 0; i < len; i++)
                        {
                            bcc ^= ptrrecvbuf[i + 1];
                        }
                        if (ptrrecvbuf[len + 1] == bcc)
                        {
                            str = "MIS_OK";
                            return 0;
                        }
                        else
                        {
                            str = "MIS_CRCMERR";
                            return 1;
                        }

                    }
                }
            }
        }
        public virtual int Uart_send_cmd(int CommHandle, byte[] ptrSendBuf,out byte[] ptrRecvBuf)
        {
            int comhandle, bcc = 0;
            int returndata;
            int sendlen, i, datalen;
            string CheckResult;

            comhandle = CommHandle;
            sendlen = ptrSendBuf[1];       //发送长度
            byte[] SendbufData = new byte[sendlen+6];          
            SendbufData[0] = 0x02;            //握手字节  
            for (i = 0; i < sendlen + 2; i++)       /*S NUMB CMD LEN data BCC*/
            {
                 // len //
                SendbufData[i + 2] = ptrSendBuf[i];
            }

            if (comhandle == 1)//SWP com
            {
                SendbufData[1] = (byte)(ComSlaveCmdCounter++);
                bcc ^= (ComSlaveCmdCounter - 1);

                SlaveComRecvIndex = 0;
                SlaveComRecvOkFlag = false;
            }
            else//EXT MCU com
            {
                SendbufData[1] = (byte)(ComExtMcuCmdCounter++);//命令计数
                bcc ^= (ComExtMcuCmdCounter - 1);
                ExtMcuComRecvIndex = 0;
                ExtMcuComRecvOkFlag = false;
            }

            for (i = 0; i < sendlen + 2; i++)
            {
                bcc ^= ptrSendBuf[i];
            }
            SendbufData[sendlen + 4] = (byte)bcc;
            SendbufData[sendlen + 5] = 0x03; //ETX

            if (CommHandle == 1)
            {
                Uart_send(SendbufData, 1);
                returndata = ComWaits(20, 1);
                if (returndata == 0)
                {
                    returndata = CheckComRecv(1, out CheckResult);//CheckResult info for debug
                    datalen = ComSlaveRecvBufData[3] + 1;
                    ptrRecvBuf = new byte[datalen];
                    for (i = 0; i < datalen; i++)
                        ptrRecvBuf[i] = ComSlaveRecvBufData[i + 3];
                }
                else
                {
                    ptrRecvBuf = new byte[1];
                    ptrRecvBuf[0] = 0xFF; 
                }
                SlaveComRecvOkFlag = false;
            }
            else
            {
                //默认用外部MCU串口发送，无校验
                Uart_send(SendbufData, 0);
                returndata = ComWaits(10, 0);            //等待10s          
                if (returndata == 0)
                {
                    returndata = CheckComRecv(0, out CheckResult);// CheckResult info for debug
                    datalen = ComExtMcuRecvBufData[3] + 1;
                    ptrRecvBuf = new byte[datalen];
                    for (i = 0; i < datalen; i++)
                        ptrRecvBuf[i] = ComExtMcuRecvBufData[i + 3];
                }
                else
                {
                    ptrRecvBuf=new byte[1];
                    ptrRecvBuf[0] = 0xFF; 
                }
                ExtMcuComRecvOkFlag = false;
            }                              
            return returndata;
        }

        public virtual int FM309_SWP_SLAVE_Write_Register(string regAddr_s, string regData_s, out byte returndata)
        {
            int i,datalength;
            uint RegAddressLo, RegAddressHi, RegAddressMi;
            byte[] Sendbuf;
            byte[] Recvbuf = new byte[32];           
            //UInt32 regAddr;
            byte[] regData;

            if (serialPort1.IsOpen == false)
            {
                returndata = 0xFF;
                return 1;
            }
            //regAddr = Convert.ToUInt32(regAddr_s);
            regData_s = DeleteSpaceString(regData_s);
            regData = strToHexByte(regData_s);
            datalength = regData.Length;
            Sendbuf = new byte[datalength];

            if (regAddr_s.Length != 6)
            {
                for (i = 6 - regAddr_s.Length; i > 0; i--)
                {
                    regAddr_s = "0" + regAddr_s;
                }
            }
            RegAddressHi = (uint)strToHexByte(regAddr_s)[0];
            RegAddressMi = (uint)strToHexByte(regAddr_s)[1];
            RegAddressLo = (uint)strToHexByte(regAddr_s)[2];

            //RegAddressHi = regAddr / 0x10000;
            //RegAddressMi = (regAddr % 0x10000) / 0x100;
            //RegAddressLo = regAddr % 0x100;   
       
            Sendbuf[0] = 0x53;//CMD                 
            Sendbuf[1] = (byte)RegAddressHi;//H
            Sendbuf[2] = (byte)RegAddressMi;//M
            Sendbuf[3] = (byte)RegAddressLo;//L
            Sendbuf[4] = (byte)datalength;//LEN
            for (i = 0; i < datalength;i++ )
                Sendbuf[5+i] = regData[i];//DATA
            
            Uart_send_cmd(1, Sendbuf, out Recvbuf);
            returndata = Recvbuf[0];
            /*Recvbuf需要COS回应*/
            if (returndata == 0x01)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public virtual int FM309_SWP_SLAVE_Read_Register(string regAddr_s,int datalength,out byte[] regData)
        {
            uint RegAddressLo, RegAddressHi, RegAddressMi;            
            //UInt32 regAddr;
            byte[] Sendbuf = new byte[5];
            byte[] recvbuf = new byte[32];
            regData=new byte[datalength];
            //regAddr = Convert.ToUInt32(regAddr_s);
            if (regAddr_s.Length != 6)
            {
                for (int i = 6 - regAddr_s.Length; i > 0; i--)
                {
                    regAddr_s = "0" + regAddr_s;
                }
            }
            RegAddressHi = (uint)strToHexByte(regAddr_s)[0];
            RegAddressMi = (uint)strToHexByte(regAddr_s)[1];
            RegAddressLo = (uint)strToHexByte(regAddr_s)[2];
            //RegAddressHi = regAddr / 0x10000;
            //RegAddressMi = (regAddr % 0x10000) / 0x100;
            //RegAddressLo = regAddr % 0x100;

            Sendbuf[0] = 0x51;//CMD                 
            Sendbuf[1] = (byte)RegAddressHi;//H
            Sendbuf[2] = (byte)RegAddressMi;//M
            Sendbuf[3] = (byte)RegAddressLo;//L
            Sendbuf[4] = (byte)datalength;//LEN

            Uart_send_cmd(1, Sendbuf, out recvbuf);
            if (recvbuf[0] == 0xFF)
            {
                regData[0] = 0xFF;
                return 1;
            }
            for (int i = datalength; i > 0;i-- )
                regData[datalength-i] = recvbuf[datalength-i+1];//recvbuf[0]是len
            return 0;

            /*regData为读出的寄存器数据*/
        }
        public virtual int FM295_Read_Register(string regAddr_s, out byte regData)//一次只读一个字节
        {
            
            uint RegAddressLo, RegAddressHi;
            byte[] Sendbuf = new byte[5];
            byte[] recvbuf=new byte[32];

            RegAddressHi = (uint)strToHexByte(regAddr_s)[0];
            RegAddressLo = (uint)strToHexByte(regAddr_s)[1];
            //RegAddressHi = regAddr / 0x100;
            //RegAddressLo = regAddr % 0x100;

            Sendbuf[0] = 0x51;//CMD      
            Sendbuf[1] = 0x03;
            Sendbuf[2] = (byte)RegAddressLo;//L    
            Sendbuf[3] = (byte)RegAddressHi;//H
            Sendbuf[4] = 0x01;//LEN

            Uart_send_cmd(0, Sendbuf, out recvbuf);
            if (recvbuf[0] == 0xFF)
            {
                regData = 0xFF;
                return 1;
            }
            regData = recvbuf[1];//recvbuf[0]是len
            return 0;

            /*regData为读出的寄存器数据*/
        }
        public virtual int FM295_Write_Register(string regAddr_s, string RegData_s, out byte returndata)
        {
            uint RegAddressHi, RegAddressLo;
            int Result, datalength;
            //UInt32 regAddr;
            byte[] regData;
            byte[] Recvbuf = new byte[32];
            byte[] Sendbuf = new byte[6];            
            
            if (serialPort1.IsOpen == false)
            {
                returndata = 0xFF;
                return 1;
            }
            //regAddr = Convert.ToUInt32(regAddr_s);
            RegData_s = DeleteSpaceString(RegData_s);
            regData = strToHexByte(RegData_s);
            datalength = regData.Length;
            RegAddressHi = (uint)strToHexByte(regAddr_s)[0];
            RegAddressLo = (uint)strToHexByte(regAddr_s)[1];
            //RegAddressLo = regAddr % 0x100;
            //RegAddressHi = regAddr / 0x100;
            
            Sendbuf[0] = 0x53;
            Sendbuf[1] = 0x04;
            Sendbuf[2] = (byte)RegAddressLo;
            Sendbuf[3] = (byte)RegAddressHi;
            Sendbuf[4] = 0x01;
            Sendbuf[5] = regData[0];
            /*Recvbuf需要295COS回应*/
            Result = Uart_send_cmd(0,Sendbuf,out Recvbuf);    // (Recvbuf[0] == 0x07))//超时出错   
            if (Recvbuf[0] == 0xFF || Result == 0x01)
            {
                returndata = 0xFF;
                return 1;
            }
            returndata = Recvbuf[0]; 
            return 0;  
        }
/*       private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string tmpstr = "";
            for (int i = 0; i < serialPort2.BytesToRead; i++)
            {
                tmpstr += Convert.ToString(serialPort2.ReadByte(), 16) + " ";

            }
            tmpstr = tmpstr.ToUpper();
            safeAddtrText(tmpstr);
        }
        public delegate void _SafeAddtrTextCall(string text);
        private void safeAddtrText(string text)
        {
            if (this.InvokeRequired)
            {
                _SafeAddtrTextCall call =
                delegate(string s)
                {
                    SRxBuffer1.Text += s;
                };

                this.Invoke(call, text);
            }
            else
            {
                SRxBuffer1.Text += text;

            }
        }*/
         private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string tmpstr = "";
            for (int i = 0; i < serialPort2.BytesToRead; i++)
            {
                tmpstr += Convert.ToString(serialPort2.ReadByte(), 16) + " ";

            }
            tmpstr = tmpstr.ToUpper();
            //safeAddtrText(tmpstr);
        }
        
        public void Uart_send(byte[] SendData, int comhandle)//发送数据
        {
            byte[] RecvData;//comm接收到的数据 Debug使用
            if (comhandle == 1)
            {
                this.serialPort2.Write(SendData, 0, SendData.Length);
                Delay(50);
                SlaveCommCommDataReceived(out RecvData);
            }
            else
            {
                this.serialPort1.Write(SendData, 0, SendData.Length);
                //Delay(50);
                MasterCommDataReceived(out RecvData);               
            }
        }
        public void Uart_Rev(out byte[] RevData, int comhandle)//接收数据 
        {                      
            string revdata_Str=" ";
            
            if (comhandle == 1)
            {
                /*serialPort2.DataReceived += new SerialDataReceivedEventHandler(SlaveCommCommDataReceived);//Debug 测试SWP SlaveCommCommDataReceived函数 
                RevData = ComSlaveRecvBufData;*/
                for (int i = serialPort2.BytesToRead; i >0; i--)
                {                   
                    revdata_Str += serialPort2.ReadByte().ToString("X2") + " ";
                   
                }
                revdata_Str = DeleteSpaceString(revdata_Str);
                RevData = new byte[revdata_Str.Length/2];  
                RevData = strToHexByte(revdata_Str);
            }
            else
            {
                /*serialPort1.DataReceived += new SerialDataReceivedEventHandler(MasterCommDataReceived);//Debug 测试SWP SlaveCommCommDataReceived函数 
                RevData = ComExtMcuRecvBufData;*/
                for (int i = serialPort1.BytesToRead; i > 0; i--)
                {
                    revdata_Str += serialPort1.ReadByte().ToString("X2") + " "; 
                }
                revdata_Str = DeleteSpaceString(revdata_Str);
                RevData = new byte[revdata_Str.Length / 2]; 
                RevData = strToHexByte(revdata_Str);
            }
        }

        public virtual int SWP_SLAVE_FIFO_Receive(out int ptrProByte0, out int ptrProByte1, out int ptrProByte2, out int ptrProByte3, out byte[] SWP_RecvBuf0, out byte[] SWP_RecvBuf1, out byte[] SWP_RecvBuf2, out byte[] SWP_RecvBuf3)
        {
            string RegAddress;
            //string RegData="";
            byte[] RegData;
            byte Rev;
            int i,j;
            SWP_RecvBuf0 = new byte[32];
            SWP_RecvBuf1 = new byte[32];
            SWP_RecvBuf2 = new byte[32];
            SWP_RecvBuf3 = new byte[32];
            int swp_rx_lenth0;
            int swp_rx_lenth1;
            int swp_rx_lenth2;
            int swp_rx_lenth3;
            byte swp_irg_data, swp_rxram_st_data;
            string swp_irg_data_str;
            
            ptrProByte1 = 0;
            ptrProByte2 = 0;
            ptrProByte3 = 0;
            ptrProByte0 = 0;
            RegAddress = FM309Reg_Str.swp_irq;                       //读swp_irq寄存器判swp_rx_irq是否置1            
            j=FM309_SWP_SLAVE_Read_Register(RegAddress, 1,out RegData);
            if (j == 1)//读寄存器错误
                return 1;

            swp_irg_data = RegData[0];
            if ((swp_irg_data & 0x02) == 0x02)
            {
                RegAddress = FM309Reg_Str.swp_raram_st;                       //读swp_rxram_st 判那一块有数据
                j=FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);
                if (j == 1)
                    return 1;

                swp_rxram_st_data = RegData[0];
                if ((swp_rxram_st_data & 0x01) == 0x01)
                {
                    RegAddress = FM309Reg_Str.swp_rx_lenth0;               
                    j=FM309_SWP_SLAVE_Read_Register(RegAddress, 1,out RegData);//读swp_rx_lenth0 判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth0 = RegData[0];
                    RegAddress = FM309Reg_Str.Cl_swp_Rxram0;               //读Rambuf0 接收的数据
                    j = FM309_SWP_SLAVE_Read_Register(RegAddress, swp_rx_lenth0, out RegData); //读ram块0地址数据
                    if (j == 1)
                        return 1;
                    for (i = 0; i < swp_rx_lenth0; i++)				 //根据数据长度取数据放到缓存
                    {
                        SWP_RecvBuf0[i] = RegData[i];
                    }
                    ptrProByte0 = swp_rx_lenth0;

                }

                if ((swp_rxram_st_data & 0x02) == 0x02)
                {
                    RegAddress = FM309Reg_Str.swp_rx_lenth1;			   
                    j=FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);//读swp_rx_lenth1 判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth1 = RegData[0];
                    RegAddress = FM309Reg_Str.Cl_swp_Rxram1;               //读Rambuf1 接收的数据
                    
                    j=FM309_SWP_SLAVE_Read_Register(RegAddress,swp_rx_lenth1, out RegData); //读ram块0地址数据
                    if (j == 1)
                        return 1;
                    for (i = 0; i < swp_rx_lenth1; i++)				 //根据数据长度取数据放到缓存
                    {    
                        SWP_RecvBuf1[i] = RegData[i];                        
                    }
                    ptrProByte1 = swp_rx_lenth1;
                }

                if ((swp_rxram_st_data & 0x04) == 0x04)
                {
                    RegAddress = FM309Reg_Str.swp_rx_lenth2;			   
                    j=FM309_SWP_SLAVE_Read_Register(RegAddress, 1,out RegData);//读swp_rx_lenth2判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth2 = RegData[0];
                    RegAddress = FM309Reg_Str.Cl_swp_Rxram2;                            //读Rambuf2 接收的数据
                    j = FM309_SWP_SLAVE_Read_Register(RegAddress, swp_rx_lenth2, out RegData); //读ram块0地址数据
                    if (j == 1)
                        return 1;
                    for (i = 0; i < swp_rx_lenth2; i++)				 //根据数据长度取数据放到缓存
                    {
                        SWP_RecvBuf2[i] = RegData[i];
                    }
                    ptrProByte2 = swp_rx_lenth2;
                }
                if ((swp_rxram_st_data & 0x08) == 0x08)
                {
                    RegAddress = FM309Reg_Str.swp_rx_lenth3;			   //读swp_rx_lenth3 判数据长度
                    j=FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);//读swp_rx_lenth3判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth3 = RegData[0];
                    RegAddress = FM309Reg_Str.Cl_swp_Rxram3;                            //读Rambuf3 接收的数据
                    j = FM309_SWP_SLAVE_Read_Register(RegAddress, swp_rx_lenth3, out RegData); //读ram块0地址数据
                    if (j == 1)
                        return 1;
                    for (i = 0; i < swp_rx_lenth3; i++)				 //根据数据长度取数据放到缓存
                    {
                        SWP_RecvBuf3[i] = RegData[i];
                    }
                    ptrProByte3 = swp_rx_lenth3;


                }
                RegAddress = FM309Reg_Str.swp_irq;
                swp_irg_data_str = (swp_irg_data & 0xFD).ToString("X2");
                j = FM309_SWP_SLAVE_Write_Register(RegAddress, swp_irg_data_str, out Rev);
                if (j == 1)
                    return 1;                
            }
            else
            {
                return 1;
            }
            return 0;
        }

        public virtual int SWP_MASTER_FIFO_Receive(out int ptrProByte0, out int ptrProByte1, out int ptrProByte2, out int ptrProByte3, out byte[] SWP_RecvBuf0, out byte[] SWP_RecvBuf1, out byte[] SWP_RecvBuf2, out byte[] SWP_RecvBuf3)
        {
            string RegAddress;
            //string RegData="";
            byte RegData;
            byte Rev;
            int i, j;
            uint regadr_uint;
            SWP_RecvBuf0 = new byte[32];
            SWP_RecvBuf1 = new byte[32];
            SWP_RecvBuf2 = new byte[32];
            SWP_RecvBuf3 = new byte[32];
            int swp_rx_lenth0;
            int swp_rx_lenth1;
            int swp_rx_lenth2;
            int swp_rx_lenth3;
            byte swp_irg_data, swp_rxram_st_data;
            string swp_irg_data_str;

            ptrProByte1 = 0;
            ptrProByte2 = 0;
            ptrProByte3 = 0;
            ptrProByte0 = 0;
            RegAddress = FM295Reg_Str.swp_master_irq;                       //读swp_irq寄存器判swp_rx_irq是否置1            
            j = FM295_Read_Register(RegAddress, out RegData);
            if (j == 1)//读寄存器错误
                return 1;

            swp_irg_data = RegData;
            if ((swp_irg_data & 0x02) == 0x02)
            {
                RegAddress = FM295Reg_Str.swp_master_raram_st;                       //读swp_rxram_st 判那一块有数据
                j = FM295_Read_Register(RegAddress, out RegData);
                if (j == 1)
                    return 1;

                swp_rxram_st_data = RegData;
                if ((swp_rxram_st_data & 0x01) == 0x01)
                {
                    RegAddress = FM295Reg_Str.swp_master_rx_lenth0;
                    j = FM295_Read_Register(RegAddress, out RegData);//读swp_rx_lenth0 判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth0 = RegData;

                    regadr_uint = FM295Reg.swp_master_Rxram0;
                    RegAddress = FM295Reg_Str.swp_master_Rxram0;               //读Rambuf0 接收的数据                    
                    for (i = 0; i < swp_rx_lenth0; i++)				 //根据数据长度取数据放到缓存
                    {
                        j = FM295_Read_Register(RegAddress, out RegData); //读ram块0地址数据
                        if (j == 1)
                            return 1;
                        SWP_RecvBuf0[i] = RegData;
                        regadr_uint++;
                        RegAddress = regadr_uint.ToString("X4");
                    }
                    ptrProByte0 = swp_rx_lenth0;

                }

                if ((swp_rxram_st_data & 0x02) == 0x02)
                {
                    RegAddress = FM295Reg_Str.swp_master_rx_lenth1;
                    j = FM295_Read_Register(RegAddress, out RegData);//读swp_rx_lenth1 判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth1 = RegData;
                    regadr_uint = FM295Reg.swp_master_Rxram1;
                    RegAddress = FM295Reg_Str.swp_master_Rxram1;               //读Rambuf1 接收的数据                    
                    for (i = 0; i < swp_rx_lenth1; i++)				 //根据数据长度取数据放到缓存
                    {
                        j = FM295_Read_Register(RegAddress, out RegData); //读ram块1地址数据
                        if (j == 1)
                            return 1;
                        SWP_RecvBuf0[i] = RegData;
                        regadr_uint++;
                        RegAddress = regadr_uint.ToString("X4");
                    }
                    ptrProByte1 = swp_rx_lenth1;
                }

                if ((swp_rxram_st_data & 0x04) == 0x04)
                {
                    RegAddress = FM295Reg_Str.swp_master_rx_lenth2;
                    j = FM295_Read_Register(RegAddress, out RegData);//读swp_rx_lenth2判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth2 = RegData;
                    regadr_uint = FM295Reg.swp_master_Rxram2;
                    RegAddress = FM295Reg_Str.swp_master_Rxram2;               //读Rambuf2 接收的数据                    
                    for (i = 0; i < swp_rx_lenth2; i++)				 //根据数据长度取数据放到缓存
                    {
                        j = FM295_Read_Register(RegAddress, out RegData); //读ram块2地址数据
                        if (j == 1)
                            return 1;
                        SWP_RecvBuf0[i] = RegData;
                        regadr_uint++;
                        RegAddress = regadr_uint.ToString("X4");
                    }
                    ptrProByte2 = swp_rx_lenth2;
                }
                if ((swp_rxram_st_data & 0x08) == 0x08)
                {
                    RegAddress = FM295Reg_Str.swp_master_rx_lenth3;			   //读swp_rx_lenth3 判数据长度
                    j = FM295_Read_Register(RegAddress, out RegData);//读swp_rx_lenth3判数据长度
                    if (j == 1)
                        return 1;
                    swp_rx_lenth3 = RegData;
                    regadr_uint = FM295Reg.swp_master_Rxram3;
                    RegAddress = FM295Reg_Str.swp_master_Rxram3;               //读Rambuf3接收的数据                    
                    for (i = 0; i < swp_rx_lenth3; i++)				 //根据数据长度取数据放到缓存
                    {
                        j = FM295_Read_Register(RegAddress, out RegData); //读ram块3地址数据
                        if (j == 1)
                            return 1;
                        SWP_RecvBuf0[i] = RegData;
                        regadr_uint++;
                        RegAddress = regadr_uint.ToString("X4");
                    }
                    ptrProByte3 = swp_rx_lenth3;


                }
                RegAddress = FM295Reg_Str.swp_master_irq;
                swp_irg_data_str = (swp_irg_data & 0xFD).ToString("X2");
                j = FM295_Write_Register(RegAddress, swp_irg_data_str, out Rev);
                if (j == 1)
                    return 1;
            }
            else
            {
                return 1;
            }
            return 0;
        }

        public virtual int SWP_SLAVE_SendData_AllBuf(int Sendlen0, int Sendlen1, int Sendlen2, int Sendlen3, string SendBuf0, string SendBuf1, string SendBuf2, string SendBuf3)
        {
            string RegAddress;
            byte swp_tx_trigger_data,returndata;
            byte[] RegData;
            int i, j,h,k;          
            string Sendlen0_str = Sendlen0.ToString("X2");
            string Sendlen1_str = Sendlen1.ToString("X2");
            string Sendlen2_str = Sendlen2.ToString("X2");
            string Sendlen3_str = Sendlen3.ToString("X2");

            RegAddress = FM309Reg_Str.swp_tx_lenth0;//把要发送的数据长度 写入309寄存器。
            i = FM309_SWP_SLAVE_Write_Register(RegAddress, Sendlen0_str, out returndata);
            RegAddress = FM309Reg_Str.swp_tx_lenth1;
            j = FM309_SWP_SLAVE_Write_Register(RegAddress, Sendlen1_str, out returndata);
            RegAddress = FM309Reg_Str.swp_tx_lenth2;
            h = FM309_SWP_SLAVE_Write_Register(RegAddress, Sendlen2_str, out returndata);
            RegAddress = FM309Reg_Str.swp_tx_lenth3;
            k = FM309_SWP_SLAVE_Write_Register(RegAddress, Sendlen3_str, out returndata);
            if ((i == 1) || (j == 1) || (h == 1) || (k == 1))
                return 1;

            RegAddress = FM309Reg_Str.swp_ctrl;
            i = FM309_SWP_SLAVE_Write_Register(RegAddress, "01", out returndata);   //swp_ctrl寄存器写"0x00"  TX_EN=0
            j = FM309_SWP_SLAVE_Write_Register(RegAddress, "03",out returndata);   //swp_ctrl寄存器写"0x03"  TX_EN=1
            if ((i == 1) || (j == 1))
                return 1;

            RegAddress = FM309Reg_Str.swp_tx_trigger;                       //读swp_tx_trigger寄存器判是否可以启动发送            
            j = FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);
            if (j == 1)
                return 1;
            swp_tx_trigger_data = RegData[0];

            if ((swp_tx_trigger_data & 0x01) == 0x00)//可以启动发送Rambuf0
            {               
                RegAddress = FM309Reg_Str.Cl_swp_Txram0;
                
                j = FM309_SWP_SLAVE_Write_Register(RegAddress, SendBuf0,out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;                 
                
                RegAddress = FM309Reg_Str.swp_ctrl;
                j =FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x01);
                if ((j == 0) && ((RegData[0] & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM309_SWP_SLAVE_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"),out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            if ((swp_tx_trigger_data & 0x02) == 0x00)//可以启动发送Rambuf1
            {
                RegAddress = FM309Reg_Str.Cl_swp_Txram1;
                j = FM309_SWP_SLAVE_Write_Register(RegAddress, SendBuf1, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;    

                RegAddress = FM309Reg_Str.swp_ctrl;
                j = FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x02);
                if ((j == 0) && ((RegData[0] & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM309_SWP_SLAVE_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            if ((swp_tx_trigger_data & 0x04) == 0x00)//可以启动发送Rambuf2
            {
                RegAddress = FM309Reg_Str.Cl_swp_Txram2;
                j = FM309_SWP_SLAVE_Write_Register(RegAddress, SendBuf2, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;    
                RegAddress = FM309Reg_Str.swp_ctrl;
                j = FM309_SWP_SLAVE_Read_Register(RegAddress, 1,out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x04);
                if ((j == 0) && ((RegData[0] & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM309_SWP_SLAVE_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            if ((swp_tx_trigger_data & 0x08) == 0x00)//可以启动发送Rambuf3
            {
                RegAddress = FM309Reg_Str.Cl_swp_Txram3;
                j = FM309_SWP_SLAVE_Write_Register(RegAddress, SendBuf3, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;
                RegAddress = FM309Reg_Str.swp_ctrl;
                j = FM309_SWP_SLAVE_Read_Register(RegAddress,1, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x08);
                if ((j == 0) && ((RegData[0] & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM309_SWP_SLAVE_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            return 0;
        }
        public virtual int SWP_SLAVE_SendData_FirstBuf(int Sendlen0, string SendBuf0)
        {    
            string RegAddress;
            byte swp_tx_trigger_data;
            int i,j;
            byte returndata;
            byte[] RegData;
            string Sendlen0_str = Sendlen0.ToString("X2");

            RegAddress = FM309Reg_Str.swp_tx_lenth0;//把要发送的数据长度 写入309寄存器。
            i = FM309_SWP_SLAVE_Write_Register(RegAddress, Sendlen0_str, out returndata);           
            if (i == 1)
                return 1;

            RegAddress = FM309Reg_Str.swp_ctrl;
            i = FM309_SWP_SLAVE_Write_Register(RegAddress, "01", out returndata);   //swp_ctrl寄存器写"0x00"  TX_EN=0
            j = FM309_SWP_SLAVE_Write_Register(RegAddress, "03", out returndata);   //swp_ctrl寄存器写"0x03"  TX_EN=1
            if ((i == 1) || (j == 1))
                return 1;

            RegAddress = FM309Reg_Str.swp_tx_trigger;                       //读swp_tx_trigger寄存器判是否可以启动发送            
            j = FM309_SWP_SLAVE_Read_Register(RegAddress, 1, out RegData);
            if (j == 1)
                return 1;
            swp_tx_trigger_data = RegData[0];

            if ((swp_tx_trigger_data & 0x01) == 0x00)//可以启动发送Rambuf0
            {
                RegAddress = FM309Reg_Str.Cl_swp_Txram0;

                j = FM309_SWP_SLAVE_Write_Register(RegAddress, SendBuf0, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;

                RegAddress = FM309Reg_Str.swp_ctrl;
                j = FM309_SWP_SLAVE_Read_Register(RegAddress, 1, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x01);
                if ((j == 0) && ((RegData[0] & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM309_SWP_SLAVE_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }            
            return 0;
        }
        public virtual int SWP_MASTER_SendData_AllBuf(int Sendlen0, int Sendlen1, int Sendlen2, int Sendlen3, string SendBuf0, string SendBuf1, string SendBuf2, string SendBuf3)
        {
            string RegAddress;
            byte swp_tx_trigger_data, returndata;
            byte RegData;
            int i, j, h, k;
            string Sendlen0_str = Sendlen0.ToString("X2");
            string Sendlen1_str = Sendlen1.ToString("X2");
            string Sendlen2_str = Sendlen2.ToString("X2");
            string Sendlen3_str = Sendlen3.ToString("X2");

            RegAddress = FM295Reg_Str.swp_master_tx_lenth0;//把要发送的数据长度 写入309寄存器。
            i = FM295_Write_Register(RegAddress, Sendlen0_str, out returndata);
            RegAddress = FM295Reg_Str.swp_master_tx_lenth1;
            j = FM295_Write_Register(RegAddress, Sendlen1_str, out returndata);
            RegAddress = FM295Reg_Str.swp_master_tx_lenth2;
            h = FM295_Write_Register(RegAddress, Sendlen2_str, out returndata);
            RegAddress = FM295Reg_Str.swp_master_tx_lenth3;
            k = FM295_Write_Register(RegAddress, Sendlen3_str, out returndata);
            if ((i == 1) || (j == 1) || (h == 1) || (k == 1))
                return 1;

            RegAddress = FM295Reg_Str.swp_master_ctrl;
            i = FM295_Write_Register(RegAddress, "00", out returndata);   //swp_ctrl寄存器写"0x00"  TX_EN=0
            j = FM295_Write_Register(RegAddress, "01", out returndata);   //swp_ctrl寄存器写"0x01"  TX_EN=1
            if ((i == 1) || (j == 1))
                return 1;

            RegAddress = FM295Reg_Str.swp_master_tx_trigger;                       //读swp_tx_trigger寄存器判是否可以启动发送            
            j = FM295_Read_Register(RegAddress, out RegData);
            if (j == 1)
                return 1;
            swp_tx_trigger_data = RegData;

            if ((swp_tx_trigger_data & 0x01) == 0x00)//可以启动发送Rambuf0
            {
                RegAddress = FM295Reg_Str.swp_master_Txram0;

                j = FM295_Write_Register(RegAddress, SendBuf0, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;

                RegAddress = FM295Reg_Str.swp_master_ctrl;
                j = FM295_Read_Register(RegAddress, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x01);
                if ((j == 0) && ((RegData & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM295_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            if ((swp_tx_trigger_data & 0x02) == 0x00)//可以启动发送Rambuf1
            {
                RegAddress = FM295Reg_Str.swp_master_Txram1;
                j = FM295_Write_Register(RegAddress, SendBuf1, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;

                RegAddress = FM295Reg_Str.swp_master_ctrl;
                j = FM295_Read_Register(RegAddress, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x02);
                if ((j == 0) && ((RegData & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM295_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            if ((swp_tx_trigger_data & 0x04) == 0x00)//可以启动发送Rambuf2
            {
                RegAddress = FM295Reg_Str.swp_master_Txram2;
                j = FM295_Write_Register(RegAddress, SendBuf2, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;
                RegAddress = FM295Reg_Str.swp_master_ctrl;
                j = FM295_Read_Register(RegAddress, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x04);
                if ((j == 0) && ((RegData & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM295_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            if ((swp_tx_trigger_data & 0x08) == 0x00)//可以启动发送Rambuf3
            {
                RegAddress = FM295Reg_Str.swp_master_Txram3;
                j = FM295_Write_Register(RegAddress, SendBuf3, out returndata);//把要发送的数据写进Rambuf    
                if (j == 1)
                    return 1;
                RegAddress = FM295Reg_Str.swp_master_ctrl;
                j = FM295_Read_Register(RegAddress, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_tx_trigger_data = (byte)(swp_tx_trigger_data | 0x08);
                if ((j == 0) && ((RegData & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM295_Write_Register(RegAddress, swp_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            return 0;
        }
        public virtual int SWP_Master_SendData_FirstBuf(int Sendlen0, string SendBuf0)
        {
            string RegAddress;
            byte swp_master_tx_trigger_data;
            int i, j;
            byte returndata;
            byte RegData;
            string Sendlen0_str = Sendlen0.ToString("X2");

           

            RegAddress = FM295Reg_Str.swp_master_ctrl;
            i = FM295_Write_Register(RegAddress, "00", out returndata);   //swp_ctrl寄存器写"0x00"  TX_EN=0
            j = FM295_Write_Register(RegAddress, "01", out returndata);   //swp_ctrl寄存器写"0x01"  TX_EN=1
            if ((i == 1) || (j == 1))
                return 1;

            RegAddress = FM295Reg_Str.swp_master_tx_trigger;                       //读swp_tx_trigger寄存器判是否可以启动发送            
            j = FM295_Read_Register(RegAddress, out RegData);
            if (j == 1)
                return 1;
            swp_master_tx_trigger_data = RegData;

            if ((swp_master_tx_trigger_data & 0x01) == 0x00)//可以启动发送Rambuf0
            {
                RegAddress = FM295Reg_Str.swp_master_Txram0;

                RegAddress = FM295Reg_Str.swp_master_tx_lenth0;//把要发送的数据长度 写入295寄存器。
                i = FM295_Write_Register(RegAddress, Sendlen0_str, out returndata);                
                j = FM295_Write_Register(RegAddress, SendBuf0, out returndata);//把要发送的数据写进Rambuf    
                if ((i == 1) || (j == 1))
                    return 1;

                RegAddress = FM295Reg_Str.swp_master_ctrl;
                j = FM295_Read_Register(RegAddress, out RegData);   //读swp_ctrl寄存器查看SWP状态
                swp_master_tx_trigger_data = (byte)(swp_master_tx_trigger_data | 0x01);
                if ((j == 0) && ((RegData & 0x02) == 0x02))//如果SWP在suspend或active状态，则启动发送
                {
                    j = FM295_Write_Register(RegAddress, swp_master_tx_trigger_data.ToString("X2"), out returndata); //第0位置1，启动发送。对应发送RAM第0块地址0x1f~0x00
                    if (j == 1)
                        return 1;
                }
            }
            return 0;
        }

    }
}
