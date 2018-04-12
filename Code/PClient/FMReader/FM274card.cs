using System;
using System.Collections.Generic;
using System.Text;
using PcSc;

namespace PcSc
{
    public class FM274card : SmartCard
    {

        protected new IntPtr card;
        string receive = "";
        string send = "";

        public FM274card(IntPtr card):base(card)
        {
            this.card = card;
        }
        public override int REQA(out string StrReceived)     //override的示意函数
        {
            receive = "";
            send = SmartCardCmd.MI_REQA;
            SendCommand(send, out receive);
            if (receive.Substring((receive.Length - 4), 4) != "9000")
            {
                StrReceived = "FM274 Err: " + receive;
                return 1;
            }
            else
            {
                StrReceived = "FM274_reqa: " + receive.Substring(0, 4);
                return 0;
            }

        }

    }
}