/*
 * Card.cs: Smart card (SCARDHANDLE)
 * 
 * Copyright (c) 2006-2008 Andreas Faerber
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace PcSc
{
	public class SmartCard : MarshalByRefObject, IDisposable
	{
        protected IntPtr card;
		protected SmartCardDisposition dispose_disposition = SmartCardDisposition.Leave;
        int PRTCL;	 			//0表示现在的17寄存器设置为TypeA;1表示TypeB
        string receive = "";
        string send = "";
        int CurrentInterface = 0; //1为CL 2为CT,0为开机状态
        public int isFMV02 = 0; //Add for FMReaderV20
        public int auth_falg = 0;//CNN 20170703
        //card activation相关_cjn20151020
        public int UID_level = 0;
        public int enable_SAK = 0;
        public int card_selected_flag = 0;
        public int select_error_flag = 0;
        public string UID1 = "";
        public string UID2 = "";
        public string UID3 = "";
        public string SAK1 = "";
        public string SAK2 = "";
        public string SAK3 = "";
        public string last_SAK = "";
        public string CID = "";
        public int UID_numbers = 0;
        public int check_BCC = 0;
        public string check_BCC_result = "";

        //type B variables start
        public int ExtREQBSupported = 0;
        public string PUPI = "";
        public string AppData = "";
        public string ProtocolInfo = "";
        public string AFI = "";
        public string BRC = "";  //Bit Rate Capability
        public string MFS = "";  //Max Frame Size
        public string PT = "";   //Protocol Type
        public string FWI = "";
        public string ADC_FO = "";
        public string SFGI = "";
        public byte Param1 = 0;
        public byte Param2 = 0;
        public byte Param3 = 0;
        public byte Param4 = 0;
        public string TypeB_INF = "";
        public int enSOF_n, enEOF_n, EOF_type, EGT, SOF_type;
        public bool TypeBFrameFlag = true;
        //type B variables end
        public string display = "";
        byte[] uid = new byte[4];
        byte[] uid2 = new byte[4];//CNN 20170703定义参与认证的UID信息是选择第一重还是第二重
//        public string GetLastError = "";
        //编程相关全局变量定义
        //Prog_struct	g_ProgParam;
        uint[] g_EncryptS0=
	        {0xC, 0x1, 0xA, 0xF, 0x9, 0x2, 0x6, 0x8, 0x0, 0xD, 0x3, 0x4, 0xE, 0x7, 0x5, 0xB};
        uint[] g_EncryptS1 =
	        {0x9, 0xE, 0xF, 0x5, 0x2, 0x8, 0xC, 0x3, 0x7, 0x0, 0x4, 0xA, 0x1, 0xD, 0xB, 0x6};
        uint[] g_EncryptS2 =
	        {0xe, 0x4, 0xd, 0x1, 0x2, 0xf, 0xb, 0x8, 0x3, 0xa, 0x6, 0xc, 0x5, 0x9, 0x0, 0x7};
        uint[] g_EncryptS3 =
	        {0x4, 0x1, 0xE, 0x8, 0xD, 0x6, 0x2, 0xB, 0xF, 0xC, 0x9, 0x7, 0x3, 0xA, 0x5, 0x0};
        uint[] g_EncryptS4 =
	        {0xe, 0x3, 0x4, 0x8, 0x1, 0xC, 0xA, 0xF, 0x7, 0xD, 0x9, 0x6, 0xB, 0x2, 0x0, 0x5};
        uint[] g_EncryptS5 =
	        {0xF, 0x1, 0x6, 0xC, 0x0, 0xE, 0x5, 0xB, 0x3, 0xA, 0xD, 0x7, 0x9, 0x4, 0x2, 0x8};



		internal SmartCard(IntPtr card) {
			this.card = card;
		}

		~SmartCard() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
            try
            {
                int ret = SCardDisconnect(card, dispose_disposition);
                if (ret != 0)
                    throw SmartCardContext.ToException(ret);
            }
            catch (Exception )
            { 
               
            }
				//Console.WriteLine(SmartCardContext.ToException(ret).Message);
		}
		
		public SmartCardDisposition Disposition {
			get {
				return dispose_disposition;
			}
			set {
				dispose_disposition = value;
			}
		}
		
		public SmartCardState GetStatus() {
			uint readerLen = 0;
			int ret = SCardGetStatus(card, IntPtr.Zero, ref readerLen, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (ret != 0)
				throw SmartCardContext.ToException(ret);
			IntPtr readerPtr = Marshal.AllocHGlobal((int)readerLen);
			SmartCardState state;
			SmartCardProtocols protocol;
			uint atrLen = 0;
			ret = SCardGetStatus(card, readerPtr, ref readerLen, out state, out protocol, IntPtr.Zero, out atrLen);
			if (ret != 0)
				throw SmartCardContext.ToException(ret);
			return state;
		}

		public int Transmit(byte[] sendBuffer, byte[] receiveBuffer) {
			SmartCardIORequest sendPci = SmartCardIORequest.T1;
			SmartCardIORequest recvPci = SmartCardIORequest.T1;
            uint temp = sendPci.len;
			uint len = (uint)receiveBuffer.Length;
			IntPtr ptr = Marshal.AllocHGlobal((int)len);
			try {
				int ret = SCardTransmit(card, ref sendPci, sendBuffer, (uint)sendBuffer.Length, ref recvPci, ptr, ref len);
				if (ret != 0)
                {
                    //throw SmartCardContext.ToException(ret);
                    receive = "PC/SC communication time out! Recv: " + ret;   //2014-04-04 尝试不抛出异常
                    //                    Marshal.FreeHGlobal(ptr);
                    return 0;
                }
				//Console.Write("Transmit received {0} bytes: ", len);
				Marshal.Copy(ptr, receiveBuffer, 0, (int)len);
				//Console.WriteLine(BitConverter.ToString(receiveBuffer, 0, (int)len));
				return (int)len;
			} finally {
				Marshal.FreeHGlobal(ptr);
			}
		}
        
        public int SendCommand(string send, out string receive)
        {
			SmartCardIORequest sendPci = SmartCardIORequest.T1;
			SmartCardIORequest recvPci = SmartCardIORequest.T1;
            byte[] sendBuffer       = new byte[300];
            byte[] receiveBuffer    = new byte[300];

            sendBuffer = strToHexByte(send);

            uint len = (uint)receiveBuffer.Length;
            IntPtr ptr = Marshal.AllocHGlobal((int)len);
            try
            {
                int ret = SCardTransmit(card, ref sendPci, sendBuffer, (uint)sendBuffer.Length, ref recvPci, ptr, ref len);
                if (ret != 0)
                {
                    //throw SmartCardContext.ToException(ret);
                    receive = "PC/SC communication time out! Recv: " + ret;   //2014-04-04 尝试不抛出异常
                    //                    Marshal.FreeHGlobal(ptr);
                    return 0;
                }
                //Console.Write("Transmit received {0} bytes: ", len);
                Marshal.Copy(ptr, receiveBuffer, 0, (int)len);
                //Console.WriteLine(BitConverter.ToString(receiveBuffer, 0, (int)len));
                receive = byteToHexStr((int)len, receiveBuffer);
                return (int)len;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardDisconnect(IntPtr card, SmartCardDisposition disposition);

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardTransmit(IntPtr card, [MarshalAs(UnmanagedType.Struct)] ref SmartCardIORequest sendPci, [MarshalAs(UnmanagedType.LPArray)] byte[] sendBuffer, uint sendLen, [MarshalAs(UnmanagedType.Struct)] ref SmartCardIORequest recvPci, IntPtr recvBuffer, ref uint recvLen);

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardGetStatus(IntPtr card, IntPtr readerName, ref uint readerLength, IntPtr state, IntPtr protocol, IntPtr atr, IntPtr atrLength);

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardGetStatus(IntPtr card, IntPtr readerName, ref uint readerLength, out SmartCardState state, out SmartCardProtocols protocol, IntPtr atr, out uint atrLength);


        /// <summary>
        /// strToHexByte(string hexString) 将hexstring转化为16进制表示，比如55AA 转化为0x55,0xAA
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;  //如果最后不足两位，最后添“0”。
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
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
        
        /// <summary>
        /// AddSpaceString(string str)    在string里加入空格便于显示
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddSpaceString(string str)
        {
            string returnStr = "";
            for (int i = 0; i < str.Length; i+=2)
                returnStr += str.Substring(i, 2) + " ";
            return returnStr;
        }
        /// <summary>
        /// DeleteSpaceString(string hexString)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteSpaceString(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString; //如果最后不足两位，最后添“0”。

            return hexString;
        }


        /// <summary>
        /// 延时函数
        /// </summary>
        /// <param name="delayTime">需要延时多少秒</param>
        /// <returns></returns>
        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }
        //获取当前时间
        public virtual int CurrentTime(out string StrReceived)
        {
            DateTime now = DateTime.Now;
            StrReceived = now.ToString();
            return 0;
        }

        /// <summary>
        /// REQA(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int REQA(out string StrReceived)
        {
            Set_FM1715_reg(0x1E, 0x01, out display);        //CJN20151216 force PCD receiver always being switched on

            UID_level = 1;      //start from single level
            card_selected_flag = 0;
            select_error_flag = 0;
            receive = "";
            send = SmartCardCmd.MI_REQA;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error Code " + receive;
                return 1;
            }
            else
            {
                 StrReceived = receive.Substring(0, 4) ;
                 return 0;
            }

        }

        public virtual int Active(string SAK, out string StrReceived)   //CJN20151028添加_带参数SAK取代全局变量last_SAK
        {
            string tmpStr = "";
            display = "";
            int rlt;

            rlt = REQA(out display);
            if (rlt == 1)
            {
               StrReceived = "NO REQA"; 
                return 1;
            }
            tmpStr += "ATQA：" + display + "\t";
            do
            {
                AntiColl_lv(UID_level, out display);
                if (UID_level == 1)
                {
                    UID1 = display;
                }
                else if (UID_level == 2)
                {
                    UID2 = display;
                }
                else
                {
                    UID3 = display;
                }
                if (check_BCC == 1)     //insert BCC check results
                    tmpStr += "UID: " + display + check_BCC_result + "\t";
                else
                    tmpStr += "UID: " + display + "\t";
                check_BCC_result = "";
                Select(UID_level, out display);
                tmpStr += "Sak：" + display;
                if (UID_level == 1)
                {
                    SAK1 = display;
                }
                else if (UID_level == 2)
                {
                    SAK2 = display;
                }
                else
                {
                    SAK3 = display;
                }
                if ((display != SAK)&&(UID_level < 3))
                {
                    UID_level++;
                    tmpStr += "\n\t\t\t";
                }
                else    //三重防冲突以后一定完成选卡
                {
                    if (display == SAK)
                    {
                        tmpStr += "\t选卡完成";
                    }
                    else
                    {
                        tmpStr += "\t选卡失败，预设的SAK是" + SAK;
                    }
                    card_selected_flag = 1;
                }
            } while (card_selected_flag == 0);
            UID_numbers = UID_level;
            StrReceived = tmpStr;
            return 0;
        }

        public virtual int Active(out string StrReceived)   //CJN20151028 不带参数，使用GUI上设置的last_SAK
        {
            string tmpStr = "";
            display = "";
            int rlt;   

            rlt = REQA(out display);
            if (rlt == 1)
            {
                StrReceived = "NO REQA";
                return 1;
            }
            tmpStr += "ATQA：" + display + "\t";
            do
            {
                AntiColl_lv(UID_level, out display);
                if (UID_level == 1)
                {
                    UID1 = display;
                }
                else if (UID_level == 2)
                {
                    UID2 = display;
                }
                else
                {
                    UID3 = display;
                }
                if (check_BCC == 1)     //insert BCC check results
                    tmpStr += "UID: " + display + check_BCC_result + "\t";
                else
                    tmpStr += "UID: " + display + "\t";
                check_BCC_result = "";
                Select(UID_level, out display);
                tmpStr += "Sak：" + display;
                if (UID_level == 1)
                {
                    SAK1 = display;
                }
                else if (UID_level == 2)
                {
                    SAK2 = display;
                }
                else
                {
                    SAK3 = display;
                }
                if (enable_SAK == 1)
                {
                    if ((display != last_SAK) && (UID_level < 3))
                    {
                        UID_level++;
                        tmpStr += "\n\t\t\t";
                    }
                    else    //三重防冲突以后一定完成选卡
                    {
                        if (display == last_SAK)
                        {
                            tmpStr += "\t选卡完成";
                            UID_level = 1;           //选卡完成后，UID_level复位
                        }
                        else
                        {
                            tmpStr += "\t选卡失败，预设的SAK是" + last_SAK;
                        }
                        card_selected_flag = 1;
                    }
                }
                else
                {
                    card_selected_flag = 1;
                }
            } while (card_selected_flag == 0);
            UID_numbers = UID_level;
            StrReceived = tmpStr;
            return 0;
        }

        public virtual int WUPA(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_WUPA;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, 4);
                return 0;
            }

        }

        public virtual int InitVal(string BlockAddr, string BlockData, out string StrReceived)
        {
            byte[] Data = new byte[16];
            UInt32 Value;
            uint i;

            Value = Convert.ToUInt32(BlockData);
            for (i = 0; i < 4; i++)
            {
                Data[i] = (byte)Value;
                Data[4 + i] = (byte)~Data[i];
                Data[8 + i] = Data[i];
                Value >>= 8;
            }
            Data[12] = Data[14] = (byte)Convert.ToSByte(BlockAddr, 16);
            Data[13] = Data[15] = (byte)~Data[12];

            send = SmartCardCmd.MI_WRITEBLOCK + BlockAddr;
            for (i = 0; i < 16; i++)
            {
                send += DeleteSpaceString(Data[i].ToString("X"));
            }

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int ReadVal(string BlockAddr, out string StrReceived)
        {
            byte[] Data = new byte[4];
            uint i, j;
            UInt32 str;

            send = SmartCardCmd.MI_RDBLOCK + BlockAddr;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                str = Convert.ToUInt32(receive.Substring(0, 8), 16);
                for (i = 0; i < 4; i++)
                {
                    Data[i] = (byte)str;
                    str >>= 8;
                }
                str = 0;
                for (i = 0; i < 4; i++)
                {
                    str <<= 8;
                    str |= Data[i];
                }
                StrReceived = str.ToString();
                return 0;
            }
        }

        public virtual int INCVAL(string BlockAddr, string IncData, out string StrReceived)     // 2014.3.6 Hong
        {
            UInt16 i, j;

            //byte[] Data = new byte[16];
            //UInt32 Value;
            //uint i;
            //Value = Convert.ToUInt32(BlockData);
            //for (i = 0; i < 4; i++)
            //{
            //    Data[i] = (byte)Value;
            //    Data[4 + i] = (byte)~Data[i];
            //    Data[8 + i] = Data[i];
            //    Value >>= 8;
            //}
            //Data[12] = Data[14] = (byte)Convert.ToSByte(BlockAddr, 16);
            //Data[13] = Data[15] = (byte)~Data[12];



            if (IncData.Length < 8)
            {
                j = (UInt16)IncData.Length;
                for (i = 0; i < (8 - j); i++)
                    IncData += "0";
            }
            send = SmartCardCmd.MI_IncBlock;
            send += BlockAddr + IncData;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int DECVAL(string BlockAddr, string DecData, out string StrReceived)     // 2014.3.6 Hong
        {
            UInt16 i, j;

            if (DecData.Length < 8)
            {
                j = (UInt16)DecData.Length;
                for (i = 0; i < (8 - j); i++)
                    DecData += "0";
            }
            send = SmartCardCmd.MI_DecBlock;
            send += BlockAddr + DecData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int RESTORE(string BlockAddr, out string StrReceived)                    // 2014.3.6 Hong
        {
            send = SmartCardCmd.MI_Restore + BlockAddr;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                //do
                //{
                //    send = SmartCardCmd.TR_CL_HEAD + "03" + "09" + "04";
                //    send += "00000000";
                //    SendCommand(send, out receive);
                //    if ((receive.Substring((receive.Length - 4), 4) != "9000"))
                //    {
                //        StrReceived = "Succeeded";
                //        return 1;
                //    }
                //} while (receive.Substring((receive.Length - 4), 4) != "9000");
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int TRANSFER(string BlockAddr, out string StrReceived)                   // 2014.3.6 Hong
        {
            send = SmartCardCmd.MI_Transfer + BlockAddr;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public virtual int LOADKEY(string AuthKeys, out string StrReceived)
        {            
            byte tmp, ln, hn;
            byte[] keybuffer = new byte[15];
            byte[] Fm17_buffer = new byte[15];
            int i;        
                        
            keybuffer = strToHexByte(AuthKeys);
            for (i = 0; i < 6; i++)			//密码转换成FM17密码格式
            {
                ln = (byte)(keybuffer[i] & 0x0f);	//低4位
                hn = (byte)(keybuffer[i] >> 4);		//高4位
                Fm17_buffer[i * 2 + 1] = (byte)((~ln << 4) | ln);
                Fm17_buffer[i * 2] = (byte)((~hn << 4) | hn);
            }
            Set_FM1715_reg(0x09, 0x09, out StrReceived);
            Set_FM1715_reg(0x01, 0x00, out StrReceived);

            for (i = 0; i < 12; i++)			//
            {
                Set_FM1715_reg(0x02, Fm17_buffer[i], out StrReceived);
            }
            Set_FM1715_reg(0x01, 0x19, out StrReceived);
            Read_FM1715_reg(0x01, out StrReceived, out tmp);
            if (tmp != 0)
            {
                StrReceived = "Error";
                return 1;	 //LoadKey出错返回
            }
            Read_FM1715_reg(0x0A, out StrReceived, out tmp);
            if ((tmp & 0x40) != 0)
            {
                StrReceived = "Error";
                return 2;	 //LoadKey出错返回
            }
            StrReceived = "Succeeded";
            return 0;	 //LoadKey正确返回            

        }

        public virtual int MiAUTH(string KeyMode, string AuthBlockAddr,string key, out string StrReceived) 
        {
            receive = "";
            send = SmartCardCmd.MI_Authent + KeyMode + AuthBlockAddr+key;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }


        }
        public virtual int AUTH(byte AuthType, byte KeyMode, string AuthBlockAddr, out string StrReceived)
        {
            byte tmp, block;

            if (uid == null)
            {
                StrReceived = "Error";
                return 3;
            }
           
            block = strToHexByte(AuthBlockAddr)[0];            

            Set_FM1715_reg(0x09, 0x09, out StrReceived);
            Set_FM1715_reg(0x01, 0x00, out StrReceived);

            Set_FM1715_reg(0x31, AuthType, out StrReceived);
            Set_FM1715_reg(0x02, KeyMode, out StrReceived);
            Set_FM1715_reg(0x02, block, out StrReceived);
            if (auth_falg == 2)//CNN 20170703 compatible with NXP MIFARE Classic EV1
            {
                Set_FM1715_reg(0x02, uid2[0], out StrReceived);
                Set_FM1715_reg(0x02, uid2[1], out StrReceived);
                Set_FM1715_reg(0x02, uid2[2], out StrReceived);
                Set_FM1715_reg(0x02, uid2[3], out StrReceived);
            }
            else//原始配置
            {
                Set_FM1715_reg(0x02, uid[0], out StrReceived);
                Set_FM1715_reg(0x02, uid[1], out StrReceived);
                Set_FM1715_reg(0x02, uid[2], out StrReceived);
                Set_FM1715_reg(0x02, uid[3], out StrReceived);
            }

            Set_FM1715_reg(0x01, 0x0C, out StrReceived);
           // Delay(1);
            Read_FM1715_reg(0x01, out StrReceived, out tmp);
            if (tmp != 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 1;	 //MIF_Authen出错返回
            }
            Read_FM1715_reg(0x0A, out StrReceived, out tmp);
            if ((tmp & 0x60) != 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 2;	 //MIF_Authen出错返回
            }

            Set_FM1715_reg(0x01, 0x14, out StrReceived);
            //Delay(1);
            Read_FM1715_reg(0x01, out StrReceived, out tmp);
            if (tmp != 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 3;	 //MIF_Authen出错返回
            }

            Read_FM1715_reg(0x09, out StrReceived, out tmp);
            if ((tmp & 0x08) == 0)
            {
                Set_FM1715_reg(0x09, 0x01, out StrReceived);
                StrReceived = "Error";
                return 4;	 //MIF_Authen出错返回
            }

            StrReceived = "Succeeded";
            return 0;

        }

        public virtual int HALT(out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_HALT;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }

        }

        public virtual int RATS(out string StrReceived) //CID forced to 1
        {
            send = SmartCardCmd.MI_RATS;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, (receive.Length - 4));
                return 0;
            }
        }

        public virtual int RATS(string card_cid, out string StrReceived)
        {
            while (card_cid.Length < 2)
            {
                card_cid = "0" + card_cid;
            }
            if (card_cid.Length > 2)
                card_cid = card_cid.Substring((card_cid.Length - 2), 2);
            send = SmartCardCmd.MI_RATS1 + card_cid;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, (receive.Length - 4));
                return 0;
            }
        }

        /// <summary>
        /// AntiColl_lv(1, ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int AntiColl_lv(int level, out string StrReceived)
        {
            receive = "";
            switch(level)
            {
                case 1:
                send = SmartCardCmd.MI_ANTICOLL;
                break;
                case 2:
                send = SmartCardCmd.MI_ANTICOLL2;
                break;
                case 3:
                send = SmartCardCmd.MI_ANTICOLL3;
                break;
                default:
                send = SmartCardCmd.MI_ANTICOLL;
                break;
            }
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 2) != "90")
            {
                StrReceived = "Error";// + receive;
                select_error_flag = 1;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0,10);
                //CJN20151203 only cascade level 1 of UID is taken into consideration during AUTH
                if (level == 1)
                {
                    uid = strToHexByte(StrReceived);        
                }
                //END CJN20151203
                if (level == 2)//CNN 20170703 compatible with NXP MIFARE Classic EV1
                {
                    uid2 = strToHexByte(StrReceived);
                }
                //END CNN20170703

                if (receive.Substring((receive.Length - 4), 4) == "9001")

                {
                    check_BCC_result = "(BCC " + level + " Failed)";
                }
                else
                {
                    check_BCC_result = "(BCC " + level + " Right)";
                }
                return 0;
            }
        }
        /// <summary>
        /// Select(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int Select(int level, out string StrReceived)
        {
            receive = "";
            if (check_BCC == 0)
            {
                switch (level)
                {
                    case 1:
                        send = SmartCardCmd.MI_SEL;
                        break;
                    case 2:
                        send = SmartCardCmd.MI_SEL2;
                        break;
                    case 3:
                        send = SmartCardCmd.MI_SEL3;
                        break;
                    default:
                        send = SmartCardCmd.MI_SEL;
                        break;
                }
            }
            else
            {
                switch (level)
                {
                    case 1:
                        send = SmartCardCmd.MI_SEL_CHECK_BCC;
                        break;
                    case 2:
                        send = SmartCardCmd.MI_SEL_CHECK_BCC2;
                        break;
                    case 3:
                        send = SmartCardCmd.MI_SEL_CHECK_BCC3;
                        break;
                    default:
                        send = SmartCardCmd.MI_SEL_CHECK_BCC;
                        break;
                }
            }
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {

                select_error_flag = 1;
                if (receive.Substring((receive.Length - 4), 2) == "6D")
                    StrReceived = "UID unmatched";
                else
                    StrReceived = "Error code: " + receive;
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }
        /// <summary>
        /// Read_FM1715_reg( string regAddr,out string regData)
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <returns></returns>
        public int Read_FM1715_reg(string regAddr, out string StrReceived, out byte regData)
        {
            //string regAddr_s;
            //regAddr_s = regAddr.ToString("X2");
            
            send = "FF0F02";
            send = send + regAddr + "01";
            SendCommand(send, out receive);
            
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        public int Read_FM1715_reg(byte regAddr, out string StrReceived, out byte regData)
        {
            string regAddr_s;
            regAddr_s = regAddr.ToString("X2");

            send = "FF0F02";
            send = send + regAddr_s + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error" ;
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        /// <summary>
        ///  Read_TDA8007_reg( regAddr, out  regData)
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <returns></returns>
        public int Read_TDA8007_reg(string regAddr, out string StrReceived,out byte regData)
        {
            //string regAddr_s;
            //regAddr_s = regAddr.ToString("X2");
            
            send = "FF0F01";
            send = send + regAddr + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2),16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        public int Read_TDA8007_reg(byte regAddr, out string StrReceived, out byte regData)
        {
            string regAddr_s;
            regAddr_s = regAddr.ToString("X2");

            send = "FF0F01";
            send = send + regAddr_s + "01";
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                regData = 0xFF;
                return 1;
            }
            else
            {
                regData = Convert.ToByte(receive.Substring(0, 2), 16);
                StrReceived = receive.Substring(0, 2);
                return 0;
            }
        }

        public int Init_TDA8007(out string StrReceived)
        {
            send = SmartCardCmd.CT_INITTDA;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) == "9000")
            {
                StrReceived = "Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "Error";
                return 1;
            }
             
        }

        /// <summary>
        /// Set_FM1715_reg( regAddr, regData)
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <returns></returns>
        public int Set_FM1715_reg(string regAddr, string regData, out string StrReceived)
        {
            //string regAddr_s, regData_s;
            //regAddr_s = regAddr.ToString("X2");
            //regData_s = regData.ToString("X2");
            
            send = "FF1102";
            send += regAddr + "01";
            send += regData;

            SendCommand(send, out receive);
            
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public int Set_FM1715_reg(byte regAddr, byte regData, out string StrReceived)
        {
            string regAddr_s, regData_s;
            regAddr_s = regAddr.ToString("X2");
            regData_s = regData.ToString("X2");

            send = "FF1102";
            send += regAddr_s + "01";
            send += regData_s;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }

        public int Set_TDA8007_reg(string regAddr, string regData, out string StrReceived)
        {
            //string regAddr_s, regData_s;
            //regAddr_s = regAddr.ToString("X2");
            //regData_s = regData.ToString("X2");

            send = "FF1101";
            send += regAddr + "01";
            send += regData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        /// <summary>
        /// Set_TDA8007_reg
        /// </summary>
        /// <param name="regAddr"></param>
        /// <param name="regData"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public int Set_TDA8007_reg(byte regAddr, byte regData, out string StrReceived)
        {
            string regAddr_s, regData_s;
            regAddr_s = regAddr.ToString("X2");
            regData_s = regData.ToString("X2");

            send = "FF1101";
            send += regAddr_s + "01";
            send += regData_s;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        public virtual int Set_TDA8007_regbit(string regAddr, int bitsel, int bitdata, out string StrReceived)
        {
            byte regData;
            int i,j=1;
            string strReceived, regData_Str;
            i = Read_TDA8007_reg(regAddr, out strReceived, out regData);
            if (bitdata == 1)            
                regData |= (byte)(0x01 << bitsel);          
            else
                regData &= (byte)~(0x01 << bitsel);
            regData_Str = regData.ToString("X2");
            if (i == 0)
            {
                j = Set_TDA8007_reg(regAddr, regData_Str, out strReceived);
            }
            else
            {
                StrReceived = "Read_TDA8007 Error";
                return 1;
            }
            if (j == 0)
            {
                StrReceived = "Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "Error";
                return 1;
            }
        }
        
        /// <summary>
        /// ReadBlock( string BlockAddr ,ref string StrReceived)
        /// </summary>
        /// <param name="BlockAddr"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int ReadBlock(string BlockAddr, out string StrReceived)
        {
            send = SmartCardCmd.MI_RDBLOCK;
            send += BlockAddr;

            SendCommand(send, out receive);
           
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, receive.Length - 4);
                return 0;
            }
        }
        /// <summary>
        /// WriteBlock(string BlockAddr ,string BlockData)
        /// </summary>
        /// <param name="BlockAddr"></param>
        /// <param name="BlockData"></param>
        /// <returns></returns>
        public virtual int WriteBlock(string BlockAddr, string BlockData, out string StrReceived)
        {

            send = SmartCardCmd.MI_WRITEBLOCK;
            send += BlockAddr + BlockData;

            SendCommand(send, out receive);
          
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        public virtual int Write128Bytes(string BlockAddr, string BlockData, out string StrReceived)
        {

            send = SmartCardCmd.MI_WRITE128BYTES;
            send += BlockAddr + BlockData;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
           
        }
        /// <summary>
        /// SelectCL(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        public virtual void SelectCL(out string StrReceived)
        {
;
            send = SmartCardCmd.SEL_CL;
            SendCommand(send, out StrReceived);

            return;
        }
        /// <summary>
        /// void SelectCT(ref string StrReceived)
        /// </summary>
        /// <param name="StrReceived"></param>
        public virtual void SelectCT(out string StrReceived)
        {

            send = SmartCardCmd.SEL_CT_30V;
            SendCommand(send, out StrReceived);

            return;
        }

        public virtual int SendAPDUCL(string sendData, out string StrReceived)
        {
            string sendData_noSpace;
            byte crc_l, crc_h, errflag;
            string CRC_L, CRC_H, ErrFlag, Display = "";

            if (CurrentInterface != 1)  //界面切换
            {
                SelectCL(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CL interface select Error";
                    return 1;
                }
                CurrentInterface = 1;
            }
            sendData_noSpace = DeleteSpaceString(sendData);

            send = sendData_noSpace;

            SendCommand(send, out receive);
            if (receive.Length != 0)
            {
                if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                {
                    StrReceived = receive;//.Substring((receive.Length - 4), 4);
                    return 0;
                }

                if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
                {
                    //if (receive.Substring((receive.Length - 2), 2) == "00")
                    //{
                    //    StrReceived = "No Data Received ";
                    //    return 1;
                    //}
                    do
                    {
                        if (receive.Substring((receive.Length - 2), 2) == "00")//cnn 取多余255长度的数据
                            send = SmartCardCmd.TR_GETDATA_NORM + "FF";
                        else
                            send = SmartCardCmd.TR_GETDATA_NORM + receive.Substring((receive.Length - 2), 2);//最多256个
                        //send = SmartCardCmd.TR_GETDATA_NORM + receive.Substring((receive.Length - 2), 2);//最多256个
                        SendCommand(send, out receive);
                        if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                        {
                            StrReceived = "Get Data Error";
                            return 1;
                        }

                        StrReceived = receive.Substring(0, receive.Length - 4);//多带两byte "00"剔除掉 CRC无法读出 一般一次性取出

                    } while (receive.Substring((receive.Length - 4), 4) != "9000");

                }
                else
                {
                    StrReceived = receive;
                }
            }
            else
            {
                Read_FM1715_reg(0x0A, out ErrFlag, out errflag);
                Read_FM1715_reg(0x0D, out CRC_L, out crc_l);
                Read_FM1715_reg(0x0E, out CRC_H, out crc_h);
                if ((errflag & 0x0F) != 0x00)
                {//PICC data error, look up in the RxErr register
                    if ((errflag & 0x08) == 0x08)
                    {
                        Display = "PICC CRC Error:" + CRC_L + CRC_H;
                    }

                    if ((errflag & 0x04) == 0x04)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC SOF error";
                    }
                    if ((errflag & 0x02) == 0x02)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC parity error";
                    }
                    StrReceived = Display;
                    return 0;
                }
                else
                {
                    StrReceived = "Unknown Send Error";
                    return 1;
                }
            }
            return 0;
        }

        public virtual int SendAPDUCT(string sendData, out string StrReceived)
        {
            string sendData_noSpace;

            if (CurrentInterface != 2)  //界面切换
            {

                SelectCT(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CL interface select Error";
                    return 1;
                }
                CurrentInterface = 2;
            }
            sendData_noSpace = DeleteSpaceString(sendData);

            send = sendData_noSpace;

            StrReceived = "";
            SendCommand(send, out receive);

            if (receive.Length != 0)
            {
                if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                {
                    StrReceived = receive;//.Substring((receive.Length - 4), 4);
                    return 0;
                }

                if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
                {
                    //if (receive.Substring((receive.Length - 2), 2) == "00")
                    //{
                    //    StrReceived = "No Data Received ";
                    //    return 1;
                    //}
                    do
                    {
                        if (receive.Substring((receive.Length - 2), 2) == "00")//cnn 取多余255长度的数据
                            send = SmartCardCmd.TR_GETDATA_NORM + "FF";
                        else
                            send = SmartCardCmd.TR_GETDATA_NORM + receive.Substring((receive.Length - 2), 2);//最多256个
                        SendCommand(send, out receive);
                        if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                        {
                            StrReceived = "Get Data Error";
                            return 1;
                        }

                        StrReceived += receive.Substring(0, receive.Length - 4);//多带两byte "00"剔除掉 CRC无法读出 假设一般一次性取出

                    } while (receive.Substring((receive.Length - 4), 4) != "9000");

                }
                else
                {
                    StrReceived = receive;
                }
            }
            else
            {
                StrReceived = "Send Error";
                return 1;
             }
            return 0;
        }
        /// <summary>
        /// TransceiveCL(string sendData,string CRC_En,string TimeOut,ref string StrReceived)
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="CRC_En"></param>
        /// <param name="TimeOut"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int TransceiveCL(string sendData, string CRC_En, string TimeOut, out string StrReceived)
        {
            string sendData_noSpace, len;
            byte crc_l, crc_h, errflag;
            string CRC_L, CRC_H, ErrFlag, Display="";

            if (CurrentInterface != 1)  //接触非接界面切换
            {
                SelectCL(out receive);
                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CL interface select Error";
                    return 1;
                }
                CurrentInterface = 1;
            }
            sendData_noSpace = DeleteSpaceString(sendData);
            len = (sendData_noSpace.Length/2).ToString("X2");

            send = SmartCardCmd.TR_CL_HEAD + CRC_En + TimeOut + len;
            send += sendData_noSpace;

            SendCommand(send, out receive);
            if (receive.Length != 0)
            {

                if (isFMV02 == 0)
                {
                    if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                    {
                        StrReceived = receive.Substring((receive.Length - 4), 4);
                        return 0;
                    }
                }
                else
                {
                    StrReceived = receive; //FMReaderV20
                    return 0;

                }

                if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
                {
                    if (receive.Substring((receive.Length - 2), 2) == "00")
                    {
                        StrReceived = "No Data Received ";
                        return 1;
                    }
                    do
                    {
                        if (receive.Substring((receive.Length - 2), 2) == "00")//cnn 取多余255长度的数据
                            send = SmartCardCmd.TR_GETDATA + "FF";
                        else
                            send = SmartCardCmd.TR_GETDATA + receive.Substring((receive.Length - 2), 2);//最多256个
                       // send = SmartCardCmd.TR_GETDATA + receive.Substring((receive.Length - 2), 2);//最多256个
                        SendCommand(send, out receive);
                        if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                        {
                            StrReceived = "Get Data Error";
                            return 1;
                        }

                        StrReceived = receive.Substring(0, receive.Length - 4);//CRC在卡机中以0000代替，未真正去取，此处假设一般一次性取出全部数据

                    } while (receive.Substring((receive.Length - 4), 4) != "9000");

                }
                else
                {
                    StrReceived = receive;
                }
            }
            else
            {
                Read_FM1715_reg(0x0A, out ErrFlag, out errflag);
                Read_FM1715_reg(0x0D, out CRC_L, out crc_l);
                Read_FM1715_reg(0x0E, out CRC_H, out crc_h);
                if((errflag&0x0F) != 0x00)
                {//PICC data error, look up in the RxErr register
                    if ((errflag & 0x08) == 0x08)
                    {
                        Display = "PICC CRC Error:" + CRC_L + CRC_H;
                    }

                    if ((errflag & 0x04) == 0x04)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC SOF error";
                    }
                    if ((errflag & 0x02) == 0x02)
                    {
                        Display += "\n\t\t\t\t\t" + "PICC parity error";
                    }
                    StrReceived = Display;
                    return 0;
                }
                else
                {
                    StrReceived = "Unknown Send Error";
                    return 1;
                }
            }
            return 0;
        }

        public virtual int SendReceiveCL(string sendData, out string StrReceived)//简化，固定带CRC，timeout固定
        {
            int ret = TransceiveCL(sendData, "01", "09", out StrReceived);
            return ret;
        }

        public virtual int SendRATS(out string ATR)
        {
            int ret = SendReceiveCL("E081", out ATR);
            return ret;
        }



       


        /// <summary>
        /// TransceiveCT(string sendData,string TimeOut, ref string StrReceived)
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="TimeOut"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        /// 
        public virtual int TransceiveCT(string sendData, string TimeOut, out string StrReceived)
        {
            string sendData_noSpace, len;

            if (CurrentInterface != 2)
            {
                SelectCT(out receive);

                if (receive.Substring((receive.Length - 4), 4) != "9000") //选择CL通道
                {
                    StrReceived = "CT interface select Error";
                    return 1;
                }
                CurrentInterface = 2;
            }

            sendData_noSpace = DeleteSpaceString(sendData);
            if (sendData_noSpace.Length / 2 == 256)            //CNN 添加init模块  256字节的写操作          
                len = "00";           
            else
                len = (sendData_noSpace.Length / 2).ToString("X2");
            
            send = SmartCardCmd.TR_CT_HEAD + TimeOut + len;
            send += sendData_noSpace;

            SendCommand(send, out receive);

            if (isFMV02 == 0)
            {
                if (receive.Substring((receive.Length - 4), 4) == "9000") //无等待延时，需手动去取数据
                {
                    StrReceived = receive.Substring((receive.Length - 4), 4);
                    return 0;
                }
            }
            else
            {
                StrReceived = receive; //FMReaderV20
                return 0;
               
            }
               
            
            if (receive.Substring((receive.Length - 4), 2) == "61")//有数据返回，取出到dataReceive中 SW：61XX
            {
                if (receive.Substring((receive.Length - 2), 2) == "00")
                {
                    StrReceived = "No Data Received";
                    return 1;
                }
                do
                {
                    if (receive.Substring((receive.Length - 2), 2) == "00")//cnn 取多余255长度的数据
                        send = SmartCardCmd.TR_GETDATA + "FF";
                    else
                        send = SmartCardCmd.TR_GETDATA + receive.Substring((receive.Length - 2), 2);//最多256个
                  //  send = SmartCardCmd.TR_GETDATA + receive.Substring((receive.Length - 2), 2);
                    SendCommand(send, out receive);
                    if ((receive.Substring((receive.Length - 4), 4) != "9000") && (receive.Substring((receive.Length - 4), 2) != "61"))
                    {
                        StrReceived = "Get Data Error";
                        return 1;
                    }

                    StrReceived = receive.Substring(0, receive.Length - 4);//CRC在卡机中以0000代替，未真正去取，此处假设一般一次性取出全部数据

                } while (receive.Substring((receive.Length - 4), 4) != "9000");

            }            
            else
            {
                StrReceived = "Send Error";
                return 1;
            }
            return 0;
        }
        public virtual int PPS_CL(string PPS1, out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_PPS_CL + PPS1;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else if (receive.Substring((receive.Length - 4), 4) == "9000")
            {
                StrReceived = "PPS Exchange Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "卡片没有响应";
                return 0;
            }

        }
        public virtual int PPS_CT(string PPS1, out string StrReceived)
        {
            receive = "";
            send = SmartCardCmd.MI_PPS_CT + PPS1;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";// +receive;
                return 1;
            }
            else
            {
                StrReceived = "PPS Exchange Succeeded";
                return 0;
            }

        }      
        /// <summary>
        /// Cold_Reset(string voltage ,ref string StrReceived)
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int Cold_Reset(string voltage, out string StrReceived)
        {
            send = SmartCardCmd.CT_COLDRESET + voltage;
            SendCommand(send, out receive);
            
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, receive.Length - 4);
                return 0;
            }
        }
        /// <summary>
        /// Warm_Reset(string voltage, ref string StrReceived)
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="StrReceived"></param>
        /// <returns></returns>
        public virtual int Warm_Reset(string voltage, out string StrReceived)
        {
            send = SmartCardCmd.CT_WARMRESET + voltage;
            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive.Substring(0, receive.Length - 4);
                return 0;
            }
        }
        /// <summary>
        /// SetField(int OnOff)
        /// </summary>
        /// <param name="OnOff"></param>
        /// <returns></returns>
        public int SetField(int OnOff, out string StrReturned)
        {
            if (OnOff == 0)
                send = SmartCardCmd.MI_FieldON + "00";
            else
                send = SmartCardCmd.MI_FieldON + "01";

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReturned = "Error";
                return 1;
            }
            else
            {
                if (OnOff == 0)
                    StrReturned = "Off";
                else
                    StrReturned = "On";
                return 0;
            }
        }

        //C函数声明;
        [DllImport("CRC16.dll", EntryPoint = "?ComputeCrc@@YAXHPADHPAE1@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ComputeCrc(int CRCType, byte[] Data, int Length, byte[] TransmitFirst, byte[] TransmitSecond);
        //void ComputeCrc(int CRCType, char *Data, int Length, BYTE *TransmitFirst, BYTE *TransmitSecond);
        

        public virtual void CRC16(int CRCType, string strCRC_SEED, string strInData, out string CRC_RESULT)
        {

            //int CRC_SEED;
            string strCRC16;
            byte[] InData, wCRClow, wCRChigh;

            wCRClow = strToHexByte(strCRC_SEED);
            wCRChigh = strToHexByte(strCRC_SEED);
            InData = strToHexByte(strInData);

            ComputeCrc(CRCType, InData, strInData.Length / 2, wCRClow, wCRChigh);
            strCRC16 = byteToHexStr(1, wCRClow) + byteToHexStr(1, wCRChigh);
            CRC_RESULT = strCRC16;
        }



        public virtual int SPI_Send(string StrSend, string lenToReceive, out string StrReceived)
        {
            int len;
            receive = "";
            send = DeleteSpaceString(StrSend);
            len = send.Length / 2;

            send = SPIandI2Ccmd.SPI_SEND + len.ToString("X2") + lenToReceive + send;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive;
                return 0;
            }
        }
        public virtual int I2C_Send(string StrSend, string lenToReceive, out string StrReceived)
        {
            int len;
            receive = "";
            send = DeleteSpaceString(StrSend);
            len = send.Length / 2;

            send = SPIandI2Ccmd.I2C_SEND + len.ToString("X2") + lenToReceive + send;

            SendCommand(send, out receive);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = receive;
                return 0;
            }
        }

        public virtual int TypeAinit(out string StrReceived)
        {
            string strSend, strReceived;
            PRTCL = 0;//TypeA

            strSend = SmartCardCmd.CLA_INIT;    //"FF09010102";
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        //TYPE B functions defined here
        public virtual int TypeBinit(out string StrReceived)
        {
            //string strSend;
            PRTCL = 1;      //TypeB
            Set_FM1715_reg("11", "4B", out StrReceived);        //不输出100%ASK
            Set_FM1715_reg("12", "3F", out StrReceived);        //发射脚TX1 和TX2 发射天线的电导,最大3F
            Set_FM1715_reg("13", "03", out StrReceived);        //设置调制深度
            Set_FM1715_reg("14", "20", out StrReceived);        //编码模式和时钟频率设置为typeB

            Set_FM1715_reg("19", "73", out StrReceived);        //restore default value
            Set_FM1715_reg("1a", "19", out StrReceived);        //decoder set to typeB
            Set_FM1715_reg("1c", "00", out StrReceived);        //bit解码阈值
            Set_FM1715_reg("1D", "1E", out StrReceived);        //控制BPSK解调——restore default value
            Set_FM1715_reg("22", "2c", out StrReceived);        //选择数据校验种类和模式
            //Set_FM1715_reg("22", "26", out StrReceived);        //选择数据校验种类和模式
            Set_FM1715_reg("23", "ff", out StrReceived);        //CRCB预设值LSB
            Set_FM1715_reg("24", "ff", out StrReceived);        //CRCB预设值MSB
            Set_FM1715_reg("26", "02", out StrReceived);        //Miller 编码并调制过的内部信号
            Set_FM1715_reg("3A", "04", out StrReceived);        //select analog test mode

            if (StrReceived == "Error")
            {
                return 1;
            }
            else
            {
                StrReceived = "Succeeded";
                return 0;
            }
        }
        public virtual byte TypeBFraming(int enSOF_n, int enEOF_n, int EOF_type, int EGT, int SOF_type, out string StrReceived)
        {
            int intREG17;
            intREG17 = 0;
            intREG17 = intREG17 +
                (enSOF_n << 7) +        //0：包含帧头 1：不含帧头
                (enEOF_n << 6) +        //0：包含帧尾 1：不含帧尾
                (EOF_type << 5) +       //EOF格式——0：10etu low 1：11etu low
                (EGT << 2) +            //0到7etu范围char spacing
                SOF_type;               //SOF格式——00：10low + 2high；01：10low + 3high；10：11low+2high；11：11low+3high
            Set_FM1715_reg(0x17, (byte)(intREG17 & 0xFF), out StrReceived);        //SOF:11 low + 3 high, 1 egt; EOF: 11, no Tx EOF & SOF
            if (StrReceived == "Error")
            {
                return (byte)(intREG17 & 0xFF);
            }
            else
            {
                StrReceived = "Succeeded";
                return (byte)(intREG17 & 0xFF);
            }
        }

        public virtual void ParseATQB(string ATQB)
        {
            string rcvAFI;
            PUPI = ATQB.Substring(2, 8);
            AppData = ATQB.Substring(10, 8);
            ProtocolInfo = ATQB.Substring(18, 6);
            rcvAFI = ATQB.Substring(10, 2);
            //Protocol Information Field
            BRC = ATQB.Substring(18, 2);
            MFS = ATQB.Substring(20, 1);
            PT = ATQB.Substring(21, 1);
            FWI = ATQB.Substring(22, 1);
            ADC_FO = ATQB.Substring(23, 1);
            if (ATQB.Length >24)
            {
                SFGI = ATQB.Substring(24, 1);
            }
        }

        public virtual int REQB(out string StrReceived)         //REQA with AFI and PARAM = 0x00
        {
            string strSend, strReceived;
            PRTCL = 1;      //TypeB

            strSend = "FF09010B03050000";   //select type B, and send REQB:AFI = 0x00, PARAM = 0x00
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                
                return 0;
            }
        }

        public virtual int REQB(string AFI, string PARAM, out string StrReceived)   //Also called as WUPB function
        {

            string strSend, strReceived;
            PRTCL = 1;      //TypeB
            //REQB method 1——cnn
            strSend = SmartCardCmd.MI_REQB + AFI + PARAM;
            SendCommand(strSend, out strReceived);

            //REQB method 2——直接发送数据
            //strSend = "FF09010B0305" + AFI + PARAM;
            //SendReceiveCL(strSend, out strReceived);        //20160408错误的CRC_B
            //strSend = "05" + AFI + PARAM;       //直接发送指令的方式
            //TransceiveCL(strSend + "71FF", "02", "09", out strReceived);      //正确的CRC_B

            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else if (strReceived.Substring((strReceived.Length - 4), 4) == "6100")
            {
                StrReceived = "NO ATQB";
                return 1;
            }
            else if (strReceived.Substring((strReceived.Length - 4), 4) == "9000")
            {
                StrReceived = strReceived.Substring(0, (strReceived.Length - 4));
                return 0;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }
        }

        public virtual int SlotMARKER(string N, out string StrReceived)
        {
            string strSend, strReceived;
            PRTCL = 1;      //TypeB

            strSend = "FF09010B01" + N + "5";       //APn
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else if (strReceived.Substring((strReceived.Length - 4), 4) == "6100")
            {
                StrReceived = "NO ATQB";
                return 1;
            }
            else
            {
                StrReceived = strReceived;

                return 0;
            }
        }

        public virtual int ATTRIB(string inPUPI, string Parameters, out string StrReceived)       //WITH INPUT
        {
            string strSend, strReceived, strlen;
            int len;
            PRTCL = 1;      //TypeB

            strSend = "1D" + inPUPI + Parameters;// +TypeB_INF;
            len = strSend.Length;
            strlen = len.ToString("X2");
            strSend = "FF09010B" + strlen + strSend;    //Append header
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else if (strReceived.Substring((strReceived.Length - 4), 4) == "6100")
            {
                StrReceived = "NO RESPONSE";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }
        }

        public virtual int ATTRIB(out string StrReceived)
        {
            string strSend, strReceived, strlen;
            int len;
            PRTCL = 1;      //TypeB
            Param1 = 0;
            Param2 = 0;
            Param3 = 0;
            Param4 = 0;
            strSend = "1D" + PUPI + Param1 + Param2 + Param3 + Param4;// +TypeB_INF;
            len = strSend.Length;
            strlen = len.ToString("X2");
            strSend = "FF09010B" + strlen + strSend;    //Append header
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }
        }

        public virtual int WUPB(out string StrReceived)         //WUPB with AFI and PARAM = 0x00
        {
            string strSend, strReceived;
            PRTCL = 1;      //TypeB

            strSend = "FF09010B03050008";   //WUPB:AFI = 0x00, PARAM = 0x08
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }
        }

        public virtual int HLTB(string inPUPI, out string StrReceived)
        {
            string strSend, strReceived;
            PRTCL = 1;      //TypeB

            strSend = "FF09010B0550" + inPUPI;      //inPUPI should be 4 bytes size
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else if (strReceived.Substring((strReceived.Length - 4), 4) == "6100")
            {
                StrReceived = "NO RESPONSE";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }
        }

        public virtual int HLTB(out string StrReceived)
        {
            string strSend, strReceived;
            PRTCL = 1;      //TypeB

            strSend = "FF09010B0550" + PUPI;
            SendCommand(strSend, out strReceived);
            if (strReceived.Substring((strReceived.Length - 4), 4) == "61FE")
            {
                StrReceived = "Error";
                return 1;
            }
            else if (strReceived.Substring((strReceived.Length - 4), 4) == "6100")
            {
                StrReceived = "NO RESPONSE";
                return 1;
            }
            else
            {
                StrReceived = strReceived;
                return 0;
            }
        }

        public virtual int ReadFileByte(String fileName, out int datalength, out byte[] data)
        {
            //以独占方式打开一个文件
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            //创建一个Byte用来存放读取到的文件内容
            data = new Byte[fs.Length];
            datalength = data.Length;
            //定义变量存储初始读取位置
            int offset = 0;
            //定义变量存储当前数据剩余未读的长度
            int remaining = data.Length / 2;
            try
            {
                while (remaining > 0)
                {
                    int read = fs.Read(data, offset, remaining);
                    if (read <= 0)
                        return 1;
                        //throw new EndOfStreamException("文件读取到" + read.ToString() + "失败！");
                    // 减少剩余的字节数
                    remaining -= read;
                    // 增加偏移量
                    offset += read;
                }
            }
            catch
            {
                data = null;
            }
            fs.Dispose();
            return 0;
        }

        public static void Hex2Hexbin(byte[] bHex, byte[] bHexbin, int nLen)
        {
            byte c;
            for (int i = 0; i < nLen; i++)
            {
                c = Convert.ToByte((bHex[i] >> 4) & 0x0f);
                if (c < 0x0a)
                {
                    bHexbin[2 * i] = Convert.ToByte(c + 0x30);
                }
                else
                {
                    bHexbin[2 * i] = Convert.ToByte(c + 0x37);
                }
                c = Convert.ToByte(bHex[i] & 0x0f);
                if (c < 0x0a)
                {
                    bHexbin[2 * i + 1] = Convert.ToByte(c + 0x30);
                }
                else
                {
                    bHexbin[2 * i + 1] = Convert.ToByte(c + 0x37);
                }
            }
        }
        public virtual int Hex2Hexbin(byte[] bHex, int nLen, out byte[] bHexbin)
        {
            bHexbin = new byte[nLen * 2];
            Hex2Hexbin(bHex, bHexbin, nLen);
            return 0;
        }
        public virtual int DataEncryptPro(uint btEncryptOpt, uint P_Data, uint btAddr, out uint C_Data)
        {
            //string C_Data;
            if (btEncryptOpt == 0)
            {
                C_Data = P_Data;
            }
            else if (btEncryptOpt == 1)
            {		//加密
                DataEncrypt(1, P_Data, btAddr, out C_Data);
            }
            else
            {		//解密
                DataDeEncrypt(1, P_Data, btAddr, out C_Data);
            }
            return 0;
        }
        //---------------------------------------------------------------------------
        public virtual int DataEncrypt(int bWrEncrypt, uint P_Data, uint btAddr, out uint C_Data)		//编程数据加密函数
        {
            uint P_Data1, A_Key;

            if (bWrEncrypt == 0)
            {
                C_Data = P_Data;
                return 1;
            }

            A_Key = (g_EncryptS1[(btAddr & 0xF0) >> 4] << 4) | g_EncryptS0[(btAddr & 0x0F)];
            P_Data1 = A_Key ^ P_Data;
            C_Data = (g_EncryptS3[(P_Data1 & 0xF0) >> 4] << 4) | g_EncryptS2[(P_Data1 & 0x0F)];

            return 0;
        }

        //---------------------------------------------------------------------------
        public virtual int DataDeEncrypt(int bRdEncrypt, uint P_Data, uint btAddr, out uint C_Data)	//编程数据解密函数
        {
            uint P_Data1,A_Key;

            if (bRdEncrypt == 0)
            {
                C_Data = P_Data;
                return 1;
            }

            A_Key = (g_EncryptS1[(btAddr & 0xF0) >> 4] << 4) | g_EncryptS0[(btAddr & 0x0F)];
            P_Data1 = (g_EncryptS5[(P_Data & 0xF0) >> 4] << 4) | g_EncryptS4[(P_Data & 0x0F)];
            C_Data = A_Key ^ P_Data1;

            return 0;
        }
        public virtual int Get_timeCL(out string StrTime)
        {
            string tempStr;
            byte[] buf1;
            Int64 templ1, templ2;
            float TemplMs;

            SendAPDUCL("0040000008", out tempStr);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrTime = "Error";
                return 1;
            }
            else
            {
                buf1 = strToHexByte(tempStr);

                templ1 = buf1[2] * 0x100 + buf1[3];
                templ1 = 0x7FFF - templ1;
                templ2 = (buf1[6] * 0x100 + buf1[7]) * 0x8000;
                templ1 += templ2;
                TemplMs = (float)(templ1 / 105.9375);
                StrTime = TemplMs + "ms";
                return 0;
            }
        }
        public virtual int Get_timeCT(out string StrTime)
        {
            string tempStr;
            byte[] buf1;
            Int64 templ1, templ2;
            float TemplMs;

            SendAPDUCT("0040000008", out tempStr);

            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrTime = "Error";
                return 1;
            }
            else
            {
                buf1 = strToHexByte(tempStr);

                templ1 = buf1[0] * 0x100 + buf1[1];
                templ1 = 0x7FFF - templ1;
                templ2 = (buf1[4] * 0x100 + buf1[5]) * 0x8000;
                templ1 += templ2;
                TemplMs = (float)(templ1 / 3579.5);
                StrTime = TemplMs + "ms";
                return 0;
            }
        }

        public virtual int ProgExt_CwdCheck(int InterfaceSel, int FMXX_Sel, out int ExtStartAddr, out int prog_extand_mode, out int ExtOpt, out int prog_mem_type, out int EEmax, out int LIBsize, out int cwd14, out string STR)	//编程段后扩展，控制字检测EE OR RAM,EE大小或者RAM大小
        {
            string cwd0, cwd10, cwd11, cwd13, block0;
            int temp_cwd0, temp_cwd10, temp_cwd11, temp_cwd13;
            ExtStartAddr = 160 * 1024;
            ExtOpt = 0;        //EE  OR RAM
            prog_mem_type = 3;//ROM  OR  EE
            prog_extand_mode = 2;//段后扩展或整体扩展
            EEmax = 80 * 1024;
            cwd11 = "";
            LIBsize = 0;
            cwd14 = 0x94;

            try
            {
                if (InterfaceSel == 0)//非接
                {
                    TransceiveCL("32A0", "01", "09", out block0);
                    if (block0 == "00000000000000000000000000000000")
                    {
                        STR = "ok";                        
                    }
                    else
                    {
                        STR = "error";
                        return 0;
                    }
                    //TransceiveCL("3180", "01", "09", out block0);
                    ReadBlock("00", out block0);
                    cwd0 = block0.Substring(0, 2);
                    cwd10 = block0.Substring(20, 2);
                    cwd11 = block0.Substring(22, 2);
                    cwd13 = block0.Substring(26, 2);
                    cwd14 = strToHexByte(block0.Substring(28, 2))[0];

                }
                else
                {
                    //TransceiveCT("0001000000", "02", out block0);
                    TransceiveCT("0002010000", "02", out block0);
                    if (block0 == "9000")
                    {
                        STR = "ok";
                    }
                    else
                    {
                        STR = "error";
                        return 0;
                    }
                    TransceiveCT("0004000010", "02", out block0);
                    cwd0 = block0.Substring(2, 2);
                    cwd10 = block0.Substring(22, 2);
                    cwd11 = block0.Substring(24, 2);
                    cwd13 = block0.Substring(28, 2);
                    cwd14 = strToHexByte(block0.Substring(30, 2))[0];

                }
                temp_cwd0 = strToHexByte(cwd0)[0] & 0x60;
                temp_cwd10 = strToHexByte(cwd10)[0] & 0x03;
                temp_cwd11 = strToHexByte(cwd11)[0];
                temp_cwd13 = strToHexByte(cwd13)[0] & 0x7F;
                if ((strToHexByte(cwd10)[0] & 0x04) == 0x00)
                {
                    prog_mem_type = 0;
                }
                else
                    prog_mem_type = 1;
               
                if (FMXX_Sel == 1)
                {
                    temp_cwd10 = strToHexByte(cwd10)[0] & 0x08;
                }

                LIBsize = temp_cwd11 & 0xF8; // LIB区不为0，CNN 20130410          

                if ((temp_cwd0 & 0x20) == 0x20)//程序扩展选择段后扩展
                {
                    prog_extand_mode = 1;//段后扩展               
                }
                else
                    prog_extand_mode = 0;

                if ((temp_cwd0 & 0x40) == 0x00)//选择EE
                {
                    ExtOpt = 0;
                    if (FMXX_Sel == 1)
                    {
                        if (temp_cwd10 == 0x00)
                        {
                            EEmax = 40 * 1024;
                            ExtStartAddr = 40 * 1024 - temp_cwd13 * 1024;
                        }
                        else 
                        {
                            EEmax = 80 * 1024;
                            ExtStartAddr = 80 * 1024 - temp_cwd13 * 1024;
                        }
                    }
                    else
                    {
                        if (temp_cwd10 == 0x00)
                        {
                            EEmax = 40 * 1024;
                            ExtStartAddr = 40 * 1024 - temp_cwd13 * 1024;
                        }
                        else if (temp_cwd10 == 0x01)
                        {
                            EEmax = 80 * 1024;
                            ExtStartAddr = 80 * 1024 - temp_cwd13 * 1024;
                        }
                        else
                        {
                            EEmax = 160 * 1024;
                            ExtStartAddr = 160 * 1024 - temp_cwd13 * 1024;
                        }
                    }
                }
                else if ((temp_cwd0 & 0x40) == 0x40)//选择Ram
                {
                    ExtOpt = 1;
                    if (temp_cwd13 > 7)
                        temp_cwd13 = 7;
                    ExtStartAddr = 0x2000 - temp_cwd13 * 1024;
                    EEmax = 8 * 1024; ;
                }
                return 0;
            }
            catch (Exception ex)
            {
                STR = "error";
                return 1;
            }
        }
	}
}
