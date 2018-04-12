using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.IO.Ports;
using System.IO;
using PcSc;

namespace PcSc
{
    public class flash_isp
    {
        byte[] sendBuf = new byte[256];
        private SerialPort serialPort1 = new SerialPort();
        public flash_isp()
        {

        }

        /*打开串口函数*/
        public virtual int CommOpen(string PortName, Int32 BaudRate, int ComState, out string StrReceived)
        {
            if (ComState == 0)//打开串口
            {

                serialPort1.PortName = PortName;
                serialPort1.BaudRate = BaudRate;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.ReceivedBytesThreshold = 8;
                if (serialPort1.IsOpen)
                {
                    StrReceived = "COM Port already open";
                    return 1;
                }
                serialPort1.Open();
                StrReceived = "Open COM:\t" + PortName + " " + BaudRate;
                return 0;
            }
            else//关闭串口
            {
                serialPort1.Close();
                StrReceived = "Close COM:\t" + PortName + " " + BaudRate;
                return 0;
            }
        }


        public void Uart_send(string SendData)//发送数据
        {
            byte[] sendBuffer = new byte[SendData.Length / 2];
            sendBuffer = strToHexByte(SendData);
            this.serialPort1.Write(sendBuffer, 0, sendBuffer.Length);
        }

        public int Uart_Rev(int len, out string StrReceived)//接收数据 
        {
            string revdata_Str = "";
            int cnt = 5000;
            while (cnt > 0)
            {
                if (serialPort1.BytesToRead >= len)
                    break;
                System.Threading.Thread.Sleep(1);
                cnt--;
            }
            if (cnt == 0)
            {
                StrReceived = "Uart receive data timeout";
                return 1;
            }
            for (int i = len; i > 0; i--)
            {
                revdata_Str += serialPort1.ReadByte().ToString("X2");

            }
            StrReceived = revdata_Str;
            return 0;
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

        public int uart_bandrate_init()
        {
            Uart_send("78");
            return 0;
        }

        public int flash_isp_start_end()
        {
            Uart_send("A0");
            return 0;
        }

        public int falsh_chip_erase(out string StrReceived)
        {
            string tmp;
            Uart_send("5A");

            Uart_Rev(1, out tmp);
            if (tmp == "55")
            {
                StrReceived = "Chip Erase\t\t\tSucceeded";
                return 0;
            }
            else
            {
                StrReceived = "Chip Erase\t\t\tFailed";
                return 1;
            }
        }
        public int flash_sector_erase(string addr, out string StrReceived)
        {
            string tmp;
            Uart_send("5B" + addr);
            Uart_Rev(1, out tmp);
            if (tmp == "55")
            {
                StrReceived = "Sector Erase\t\t\tSucceeded";
                return 0;
            }
            else
            {
                StrReceived = "Sector Erase\t\t\tFailed";
                return 1;
            }
        }
        public int flash_read(string addr, string len, out string StrReceived)
        {
            int rtn;
            StrReceived = "";
            Uart_send("A6" + addr + len);
            rtn = Uart_Rev(Convert.ToByte(len, 16), out StrReceived);
            return rtn;
        }

        public int flash_write(string addr, string len, string data, out string StrReceived)
        {
            StrReceived = "";
            if (data.Length / 2 != Convert.ToByte(len, 16))
            {
                StrReceived = "Data length error";
                return 1;
            }
            Uart_send("A5" + addr + len + data);
            Uart_Rev(1, out StrReceived);
            if (StrReceived == "55")
            {
                StrReceived = "Flash write Succeeded";
                return 0;
            }
            else
            {
                StrReceived = "Flash write Failed: 0x" + addr;
                return 1;
            }
        }
        public int Uart_Rev_test(out string StrReceived)//接收数据 
        {
            string revdata_Str = "";
            int cnt = 5000;
            while (cnt > 0)
            {
                if (serialPort1.BytesToRead >= 1)
                    break;
                System.Threading.Thread.Sleep(1);
                cnt--;
            }
            if (cnt == 0)
            {
                StrReceived = "Uart receive data timeout";
                return 1;
            }
            for (int i = serialPort1.BytesToRead; i > 0; i--)
            {
                revdata_Str += serialPort1.ReadByte().ToString("X2");

            }
            StrReceived = revdata_Str;
            return 0;
        }

    }
}
