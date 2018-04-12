using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcSc;

namespace PcSc
{
    public class FM309card : SmartCard
    {
        SmartCard FM12XX_Card;
        protected new IntPtr card;        

        public FM309card(IntPtr card)
            : base(card)
        {
            this.card = card;
        }
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
       

    }
}
