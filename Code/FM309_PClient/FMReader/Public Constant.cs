using System;

namespace PcSc
{
    static class SmartCardCmd
    {
        public const string SEL_CT_50V      =   "FF0100000111";
        public const string SEL_CT_30V	    =   "FF0100000121";
        public const string SEL_CT_18V	    =   "FF0100000131";
        public const string SEL_CL		    =   "FF0100000102";
        public const string CLA_INIT        =   "FF09010102";
        //public const string CLB_INIT = "FF09010202";

        public const string TR_CT_HEAD      =   "FF0900";	    //CLA,INS,P1
        public const string TR_CL_HEAD      =   "FF09";         //CLA INS

        public const string TR_GETDATA      =   "FFC00000";     //CLA,INS,P1,P2
        public const string TR_GETDATA_NORM =   "00C00000";     //CLA,INS,P1,P2

        public const string MI_FieldON      =   "FF18010001";	//CLA,INS,P1,P2,P3 后面加一个Byte的00是关场，01是开场
        public const string MI_REQA		    =   "FF18010102";	//CLA,INS,P1,P2,P3
        public const string MI_ANTICOLL	    =   "FF18010206";	//CLA,INS,P1,P2,P3
        public const string MI_ANTICOLL2    =   "FF18012206";	//CLA,INS,P1,P2,P3
        public const string MI_ANTICOLL3    =   "FF18013206";	//CLA,INS,P1,P2,P3
        public const string MI_SEL		    =   "FF18010301";	//CLA,INS,P1,P2,P3
        public const string MI_SEL_CHECK_BCC = "FF18018301";	//CLA,INS,P1,P2,P3
        public const string MI_SEL2         = "FF18012301";	//CLA,INS,P1,P2,P3
        public const string MI_SEL_CHECK_BCC2 = "FF1801A301";	//CLA,INS,P1,P2,P3
        public const string MI_SEL3         = "FF18013301";	//CLA,INS,P1,P2,P3
        public const string MI_SEL_CHECK_BCC3 = "FF1801B301";	//CLA,INS,P1,P2,P3
        public const string MI_RATS         =   "FF18010401";	//CLA,INS,P1,P2,P3
        public const string MI_RATS1        =  "FF180104";      //CLA,INS,P1,P2
        public const string MI_RDBLOCK      =   "FF18010501";	//CLA,INS,P1,P2,P3 
        public const string MI_WRITEBLOCK   =   "FF18010611";	//CLA,INS,P1,P2,P3 后面加一个byte的地址和16byte的数据
        public const string MI_WRITE128BYTES=   "FF18010C81";	//CLA,INS,P1,P2,P3 后面加一个byte的地址和128byte的数据
        public const string MI_HALT         =   "FF18010701";	//CLA,INS,P1,P2,P3
        public const string MI_BAUDRATE     =   "FF18010801";	//CLA,INS,P1,P2,P3
        public const string MI_REQB         =   "FF18010B02";	//CLB,INS,P1,P2,P3 //由09改为0B 原因是下位机程序处理会和TR_CL_HEAD产生冲突 cnn
        public const string MI_WUPA         =   "FF18010A02";	//CLA,INS,P1,P2,P3
        public const string MI_IncBlock     =   "FF18011105";	//CLA,INS,P1,P2,P3  后接一byte地址及4byte增加值
        public const string MI_DecBlock     =   "FF18011205";	//CLA,INS,P1,P2,P3  后接一byte地址及4byte减少值
        public const string MI_Restore      =   "FF18011301";	//CLA,INS,P1,P2,P3  后接一byte地址
        public const string MI_Transfer     =   "FF18011401";	//CLA,INS,P1,P2,P3  后接一byte地址
      
        public const string MI_Authent     =   "FF18010901";  //CLA,INS,P1,P2,P3+keytype(00,01)+block+key(6byte) add by yanpengfei 
       
        public const string MI_PPS_CL = "FFFF000001";	//CLB,INS,P1,P2,P3 后面加一个参数0x11 :106kbps .0x13：212kbps。0x94：424kbps。0x18：848kbps
        public const string MI_PPS_CT = "FFFF000101";	//CLB,INS,P1,P2,P3  0x11\0x13\0x94\0x18\0x95\0x96

        public const string CT_INITTDA      =   "FF19010001";   //
        public const string CT_COLDRESET    =   "FF19010101";   //CLA,INS,P1,P2,P3 后面加一个byte的电压配置
        public const string CT_WARMRESET    =   "FF19010201";   //CLA,INS,P1,P2,P3 后面加一个byte的电压配置
        public const string CT_347_STOPMCU  =   "FF20000000";   //CLA,INS,P1,P2,P3 
    }
    
    static class SPIandI2Ccmd
    {
        public const string SPI_SEND        =   "FF1A00";   //CLA,INS,P1 后面加P2为长度，P3为想要接收的数据，后面接发送的数据   
        public const string I2C_SEND        =   "FF1B00";   //CLA,INS,P1 后面加P2为长度，P3为想要接收的数据，后面接发送的数据
    }

    static class TDA8007Reg
    {
        public const byte CSR   = 0x00;    /*Card Select Register*/
        public const byte CCR   = 0x01;    /*Clock Configuration Register*/
        public const byte PDR   = 0x02;    /*Programmable Divider Register*/
        public const byte UCR2  = 0x03;    /*UART Configuration Register 2*/
        public const byte GTR   = 0x05;    /*Guard Time Register*/
        public const byte UCR1  = 0x06;    /*UART Configuration Register 1*/
        public const byte PCR   = 0x07;    /*Power Control Register*/
        public const byte TOC   = 0x08;    /*Time-Out Configuration register*/
        public const byte TOR1  = 0x09;    /*Time-Out Register 1*/
        public const byte TOR2  = 0x0A;    /*Time-Out Register 2*/
        public const byte TOR3  = 0x0B;    /*Time-Out Register 3*/
        public const byte MSR   = 0x0C;    /*Mixed Status Register*/
        public const byte FCR   = 0x0C;    /*FIFO Control Register*/
        public const byte URR   = 0x0D;    /*UART Receive Register*/
        public const byte UTR   = 0x0D;    /*UART Transmit Register*/
        public const byte USR   = 0x0E;    /*UART Status Register*/
        public const byte HSR   = 0x0F;   /*Hardware Status Register*/
    }

    static class TDA8007Reg_Str
    {
        public const string CSR     = "00";    /*Card Select Register*/
        public const string CCR     = "01";    /*Clock Configuration Register*/
        public const string PDR     = "02";    /*Programmable Divider Register*/
        public const string UCR2    = "03";    /*UART Configuration Register 2*/
        public const string GTR     = "05";    /*Guard Time Register*/
        public const string UCR1    = "06";    /*UART Configuration Register 1*/
        public const string PCR     = "07";    /*Power Control Register*/
        public const string TOC     = "08";    /*Time-Out Configuration register*/
        public const string TOR1    = "09";    /*Time-Out Register 1*/
        public const string TOR2    = "0A";    /*Time-Out Register 2*/
        public const string TOR3    = "0B";    /*Time-Out Register 3*/
        public const string MSR     = "0C";    /*Mixed Status Register*/
        public const string FCR     = "0C";    /*FIFO Control Register*/
        public const string URR     = "0D";    /*UART Receive Register*/
        public const string UTR     = "0D";    /*UART Transmit Register*/
        public const string USR     = "0E";    /*UART Status Register*/
        public const string HSR     = "0F";   /*Hardware Status Register*/
    }

    static class FM1715Reg
    {
        public const byte Page              = 0x00;            //页写寄存器
        public const byte Command           = 0x01;            //命令寄存器
        public const byte FIFO              = 0x02;            //64字节FIFO缓冲的输入输出寄存器
        public const byte PrimaryStatus     = 0x03;            //发射器，接收器及FIFO的状态
        public const byte FIFOLength        = 0x04;            //当前FIFO内字节数寄存器
        public const byte SecondaryStatus   = 0x05;            //各种状态寄存器2
        public const byte InterruptEn       = 0x06;            //中断使能/禁止寄存器
        public const byte InterruptRq       = 0x07;            //中断请求标识寄存器
        public const byte Control           = 0x09;            //控制寄存器
        public const byte ErrorFlag         = 0x0A;            //错误状态寄存器
        public const byte CollPos           = 0x0B;            //冲突检测寄存器
        public const byte TimerValue        = 0x0C;            //定时器当前值
        public const byte BitFraming        = 0x0F;            //位帧调整寄存器
        public const byte TxControl         = 0x11;            //发送控制寄存器
        public const byte CwConductance     = 0x12;            //选择发射脚TX1和TX2发射天线
        public const byte ModConductance    = 0x13;            //定义输出驱动阻抗
        public const byte CoderControl      = 0x14;            //定义编码模式和时钟频率
        public const byte ModWidth          = 0x15;            //选择调制Pulse的宽度
        public const byte TypeBFraming      = 0x17;            //定义ISO14443B帧格式
        public const byte RxControl1        = 0x19;            //定义接收器的SubC,LPF,Gain
        public const byte DecoderControl    = 0x1A;            //解码控制寄存器
        public const byte BitPhase          = 0x1B;            //定义发送器和接收器之间的时钟相位关系
        public const byte RxThreshold       = 0x1C;            //选择位解码器的阈值
        public const byte BPSKDemControl    = 0x1D;            //控制BPSK解调
        public const byte RxControl2        = 0x1E;            //解码控制及选择接收源
        public const byte ClockQControl     = 0x1F;            //Q通道时钟生成的控制和显示
        public const byte RxWait            = 0x21;            //选择发射和接收之间的时间间隔
        public const byte ChannelRedundancy = 0x22;            //RF通道检验模式设置寄存器
        public const byte CRCPresetLSB      = 0x23;
        public const byte CRCPresetMSB      = 0x24;
        public const byte MFOUTSelect       = 0x26;            //mf OUT 选择配置寄存器
        public const byte FIFOLevel         = 0x29;            //定义FIFO上下溢出报警的门限
        public const byte TimerClock        = 0x2A;            //定时器周期设置寄存器
        public const byte TimerControl      = 0x2B;            //定时器控制寄存器
        public const byte TimerReload       = 0x2C;            //定时器初值寄存器
        public const byte IRQPinConfig      = 0x2D;            //定义中断输出的有效电平
        public const byte AUTHType          = 0x31;            //认证算法选择寄存器
        public const byte TestAnaSelect     = 0x3A;            //选择模拟测试输出信号
        public const byte TestDigiSelect    = 0x3D;            //测试管脚配置寄存器
    }

    static class FM1715Reg_Str
    {
        public const string Page            = "00";            //页写寄存器
        public const string Command         = "01";            //命令寄存器
        public const string FIFO            = "02";            //64字节FIFO缓冲的输入输出寄存器
        public const string PrimaryStatus   = "03";            //发射器，接收器及FIFO的状态
        public const string FIFOLength      = "04";            //当前FIFO内字节数寄存器
        public const string SecondaryStatus = "05";            //各种状态寄存器2
        public const string InterruptEn     = "06";            //中断使能/禁止寄存器
        public const string InterruptRq     = "07";            //中断请求标识寄存器
        public const string Control         = "09";            //控制寄存器
        public const string ErrorFlag       = "0A";            //错误状态寄存器
        public const string CollPos         = "0B";            //冲突检测寄存器
        public const string TimerValue      = "0C";            //定时器当前值
        public const string BitFraming      = "0F";            //位帧调整寄存器
        public const string TxControl       = "11";            //发送控制寄存器
        public const string CwConductance   = "12";            //选择发射脚TX1和TX2发射天线
        public const string ModConductance  = "13";            //定义输出驱动阻抗
        public const string CoderControl    = "14";            //定义编码模式和时钟频率
        public const string ModWidth        = "15";            //选择调制Pulse的宽度
        public const string TypeBFraming    = "17";            //定义ISO14443B帧格式
        public const string RxControl1      = "19";            //定义接收器的SubC,LPF,Gain
        public const string DecoderControl  = "1A";            //解码控制寄存器
        public const string BitPhase        = "1B";            //定义发送器和接收器之间的时钟相位关系
        public const string RxThreshold     = "1C";            //选择位解码器的阈值
        public const string BPSKDemControl  = "1D";            //控制BPSK解调
        public const string RxControl2      = "1E";            //解码控制及选择接收源
        public const string ClockQControl   = "1F";            //Q通道时钟生成的控制和显示
        public const string RxWait          = "21";            //选择发射和接收之间的时间间隔
        public const string ChannelRedundancy = "22";            //RF通道检验模式设置寄存器
        public const string CRCPresetLSB    = "23";
        public const string CRCPresetMSB    = "24";
        public const string MFOUTSelect     = "26";            //mf OUT 选择配置寄存器
        public const string FIFOLevel       = "29";            //定义FIFO上下溢出报警的门限
        public const string TimerClock      = "2A";            //定时器周期设置寄存器
        public const string TimerControl    = "2B";            //定时器控制寄存器
        public const string TimerReload     = "2C";            //定时器初值寄存器
        public const string IRQPinConfig    = "2D";            //定义中断输出的有效电平
        public const string AUTHType        = "31";            //认证算法选择寄存器
        public const string TestAnaSelect   = "3A";            //选择模拟测试输出信号
        public const string TestDigiSelect  = "3D";            //测试管脚配置寄存器
    }
     static class FM309Reg
    {      
        public const UInt32 Cl_swp_Rxram0    =		0xF200;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Rxram1    =		0xF220;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Rxram2    =		0xF240;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Rxram3    = 		0xF260;	//非接触接口CL/SWP RAM（低144bytes）
        public const UInt32 Cl_swp_Txram0    =   	0xF290;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 Cl_swp_Txram1    =   	0xF2B0;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 Cl_swp_Txram2    =   	0xF2D0;	//非接触接口CL/SWP RAM（高144bytes）
        public const UInt32 Cl_swp_Txram3    =   	0xF2F0;	//非接触接口CL/SWP RAM（高144bytes）

        public const UInt32 swp_ctrl		=		0xF5C0;	//SWP控制寄存器
        public const UInt32 swp_tx_trigger	=	    0xF5C1;	//SWP发送触发寄存器
        public const UInt32 swp_tx_lenth0	=		0xF5C2;	//SWP第0块RAM发送数据长度寄存器
        public const UInt32 swp_tx_lenth1	=		0xF5C3;	//SWP第1块RAM发送数据长度寄存器
        public const UInt32 swp_tx_lenth2	=		0xF5C4;	//SWP第2块RAM发送数据长度寄存器
        public const UInt32 swp_tx_lenth3	=		0xF5C5;	//SWP第3块RAM发送数据长度寄存器
        public const UInt32 swp_raram_st	=		0xF5C6;	//SWP接收RAM状态寄存器
        public const UInt32 swp_rx_lenth0	=		0xF5C7;	//SWP第0块RAM接收数据长度寄存器
        public const UInt32 swp_rx_lenth1	=		0xF5C8;	//SWP第1块RAM接收数据长度寄存器
        public const UInt32 swp_rx_lenth2	=		0xF5C9;	//SWP第2块RAM接收数据长度寄存器
        public const UInt32 swp_rx_lenth3	=		0xF5CA;	//SWP第3块RAM接收数据长度寄存器
        public const UInt32 swp_status		=		0xF5CB;	//SWP状态寄存器
        public const UInt32 swp_crc_ctrl	=		0xF5CC;	//SWP CRC控制寄存器
        public const UInt32 swp_irq			=		0xF5CD;	//SWP中断请求寄存器
        public const UInt32 swp_rx_err1		=		0xF5CE;	//SWP接收错误寄存器
        public const UInt32 swp_rx_err2		=		0xF5CF;	//SWP接收错误寄存器
     }
     static class FM309Reg_Str
     {         
         public const string Cl_swp_Rxram0 = "00F200";	//非接触接口CL/SWP RAM（低144bytes）
         public const string Cl_swp_Rxram1 = "00F220";	//非接触接口CL/SWP RAM（低144bytes）
         public const string Cl_swp_Rxram2 = "00F240";	//非接触接口CL/SWP RAM（低144bytes）
         public const string Cl_swp_Rxram3 = "00F260";	//非接触接口CL/SWP RAM（低144bytes）
         public const string Cl_swp_Txram0 = "00F290";	//非接触接口CL/SWP RAM（高144bytes）
         public const string Cl_swp_Txram1 = "00F2B0";	//非接触接口CL/SWP RAM（高144bytes）
         public const string Cl_swp_Txram2 = "00F2D0";	//非接触接口CL/SWP RAM（高144bytes）
         public const string Cl_swp_Txram3 = "00F2F0";	//非接触接口CL/SWP RAM（高144bytes）
                      
         public const string swp_ctrl      = "00F5C0";	//SWP控制寄存器
         public const string swp_tx_trigger = "00F5C1";	//SWP发送触发寄存器
         public const string swp_tx_lenth0 = "00F5C2";	//SWP第0块RAM发送数据长度寄存器
         public const string swp_tx_lenth1 = "00F5C3";	//SWP第1块RAM发送数据长度寄存器
         public const string swp_tx_lenth2 = "00F5C4";	//SWP第2块RAM发送数据长度寄存器
         public const string swp_tx_lenth3 = "00F5C5";	//SWP第3块RAM发送数据长度寄存器
         public const string swp_raram_st = "00F5C6";	//SWP接收RAM状态寄存器
         public const string swp_rx_lenth0 = "00F5C7";	//SWP第0块RAM接收数据长度寄存器
         public const string swp_rx_lenth1 = "00F5C8";	//SWP第1块RAM接收数据长度寄存器
         public const string swp_rx_lenth2 = "00F5C9";	//SWP第2块RAM接收数据长度寄存器
         public const string swp_rx_lenth3 = "00F5CA";	//SWP第3块RAM接收数据长度寄存器
         public const string swp_status = "00F5CB";	//SWP状态寄存器
         public const string swp_crc_ctrl = "00F5CC";	//SWP CRC控制寄存器
         public const string swp_irq = "00F5CD";	//SWP中断请求寄存器
         public const string swp_rx_err1 = "00F5CE";	//SWP接收错误寄存器
         public const string swp_rx_err2 = "00F5CF";	//SWP接收错误寄存器
     }
     static class FM295Reg_Str
     {
         public const string swp_master_ctrl = "F220";	//swp_ctrl寄存器
         public const string swp_master_timer_ctrl = "F221";	//swp_timer_ctrl寄存器
         public const string swp_master_timer_l = "F222";	//SWP计时器数据低位寄存器
         public const string swp_master_timer_h = "F223";	//swp计时器数据高位
         public const string swp_master_tx_trigger = "F224";	//swp发送触发寄存器
         public const string swp_master_tx_lenth0 = "F225";	//swp第0块RAM发送数据长度寄存器
         public const string swp_master_tx_lenth1 = "F226";	//swp第1块RAM发送数据长度寄存器
         public const string swp_master_tx_lenth2 = "F227";	//swp第2块RAM发送数据长度寄存器
         public const string swp_master_tx_lenth3 = "F228";	//swp第3块RAM发送数据长度寄存器
         public const string swp_master_raram_st = "F229";	//swp接收RAM状态寄存器
         public const string swp_master_rx_lenth0 = "F22A";	//swp第0块RAM接收数据长度寄存器
         public const string swp_master_rx_lenth1 = "F22B";	//swp第1块RAM接收数据长度寄存器
         public const string swp_master_rx_lenth2 = "F22C";	//swp第2块RAM接收数据长度寄存器
         public const string swp_master_rx_lenth3 = "F22D";	//swp第3块RAM接收数据长度寄存器
         public const string swp_master_rx_baud = "F22E";	//swp发送波特率寄存器
         public const string swp_master_crc_ctrl = "F22F";	//swp CRC控制寄存器
         public const string swp_master_irq = "F230";	//swp模块中断请求寄存器
         public const string swp_master_rx_err1 = "F231";	//swp接收错误寄存器
         public const string swp_master_rx_err2 = "F232";	//swp接收错误寄存器
         public const string swp_master_txram_addr = "F233";	//swp发送RAM地址寄存器
         public const string swp_master_txram_data = "F234";	//swp发送RAM数据寄存器
         public const string swp_master_rxram_addr = "F235";	//swp接收RAM地址寄存器
         public const string swp_master_rxram_data = "F236";	//swp接收RAM数据寄存器
         public const string swp_master_swio_sel = "F340";

         public const string swp_master_Rxram0 = "F480";	//SWP RAM（高128bytes）
         public const string swp_master_Rxram1 = "F4A0";	//WP RAM（高128bytes）
         public const string swp_master_Rxram2 = "F4C0";	//SWP RAM（高128bytes）
         public const string swp_master_Rxram3 = "F4E0";	//SWP RAM（高128bytes）
         public const string swp_master_Txram0 = "F400";	//SWP RAM（低128bytes）
         public const string swp_master_Txram1 = "F420";	//SWP RAM（低128bytes）
         public const string swp_master_Txram2 = "F440";	//SWP RAM（低128bytes）
         public const string swp_master_Txram3 = "F460";	//SWP RAM（低128bytes）
     }
     static class FM295Reg
     {
         public const UInt32 swp_master_ctrl = 0xF220;	//swp_ctrl寄存器
         public const UInt32 swp_master_timer_ctrl = 0xF221;	//swp_timer_ctrl寄存器
         public const UInt32 swp_master_timer_l = 0xF222;	//SWP计时器数据低位寄存器
         public const UInt32 swp_master_timer_h = 0xF223;	//swp计时器数据高位
         public const UInt32 swp_master_tx_trigger = 0xF224;	//swp发送触发寄存器
         public const UInt32 swp_master_tx_lenth0 = 0xF225;	//swp第0块RAM发送数据长度寄存器
         public const UInt32 swp_master_tx_lenth1 = 0xF226;	//swp第1块RAM发送数据长度寄存器
         public const UInt32 swp_master_tx_lenth2 = 0xF227;	//swp第2块RAM发送数据长度寄存器
         public const UInt32 swp_master_tx_lenth3 = 0xF228;	//swp第3块RAM发送数据长度寄存器
         public const UInt32 swp_master_raram_st = 0xF229;	//swp接收RAM状态寄存器
         public const UInt32 swp_master_rx_lenth0 = 0xF22A;	//swp第0块RAM接收数据长度寄存器
         public const UInt32 swp_master_rx_lenth1 = 0xF22B;	//swp第1块RAM接收数据长度寄存器
         public const UInt32 swp_master_rx_lenth2 = 0xF22C;	//swp第2块RAM接收数据长度寄存器
         public const UInt32 swp_master_rx_lenth3 = 0xF22D;	//swp第3块RAM接收数据长度寄存器
         public const UInt32 swp_master_rx_baud = 0xF22E;	//swp发送波特率寄存器
         public const UInt32 swp_master_crc_ctrl = 0xF22F;	//swp CRC控制寄存器
         public const UInt32 swp_master_irq = 0xF230;	//swp模块中断请求寄存器
         public const UInt32 swp_master_rx_err1 = 0xF231;	//swp接收错误寄存器
         public const UInt32 swp_master_rx_err2 = 0xF232;	//swp接收错误寄存器
         public const UInt32 swp_master_txram_addr = 0xF233;	//swp发送RAM地址寄存器
         public const UInt32 swp_master_txram_data = 0xF234;	//swp发送RAM数据寄存器
         public const UInt32 swp_master_rxram_addr = 0xF235;	//swp接收RAM地址寄存器
         public const UInt32 swp_master_rxram_data = 0xF236;	//swp接收RAM数据寄存器
         public const UInt32 swp_master_swio_sel = 0xF340;

         public const UInt32 swp_master_Rxram0 = 0xF480;	//SWP RAM（高128bytes）
         public const UInt32 swp_master_Rxram1 = 0xF4A0;	//WP RAM（高128bytes）
         public const UInt32 swp_master_Rxram2 = 0xF4C0;	//SWP RAM（高128bytes）
         public const UInt32 swp_master_Rxram3 = 0xF4E0;	//SWP RAM（高128bytes）
         public const UInt32 swp_master_Txram0 = 0xF400;	//SWP RAM（低128bytes）
         public const UInt32 swp_master_Txram1 = 0xF420;	//SWP RAM（低128bytes）
         public const UInt32 swp_master_Txram2 = 0xF440;	//SWP RAM（低128bytes）
         public const UInt32 swp_master_Txram3 = 0xF460;	//SWP RAM（低128bytes）
     }

}
