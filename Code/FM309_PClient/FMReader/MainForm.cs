using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using PcSc;
using System.Runtime.InteropServices;
// IronPython import;
using System.Collections.ObjectModel;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO.Ports;
using System.Xml;
using System.Xml.Linq;
using System.Linq;


namespace PcSc
{
    public partial class MainForm : Form
    {

        SmartCardContext FM12_Reader_Context = new SmartCardContext();
        SmartCard FM12XX_Card;
        swp FM309_SWP = new swp();
        flash_isp FM309_flash_isp = new flash_isp();
        FM309card FM309card;


        private ComboBox[] CfgWord_combobox = new ComboBox[150];
        private CheckBox[] CfgWord_checkbox = new CheckBox[50];
        private byte[] CfgWord_Mask = new byte[150];

        bool Cfg_Parse = false;
        bool initVal_done = false;
        int i, j;

        string ScriptDirectory = Application.StartupPath + ".\\Scripts\\aa.309";
        string HexDirectory = Application.StartupPath + ".\\prog\\aa.hex";
        string CMD_LOG_Directory = Application.StartupPath + ".\\CMD_CL_LOG.txt";
        string CfgFileName = "\\cfg_336.xml";
        int main_note_idx = 0;
        string receive = "";
        string display = "";
        string ScriptDir = "";
        int open_cmdlog_flag = 0;
        string fm347_TypeAInfo="";
        //        uint ProgFileMaxLen = 1024 * 448;
        uint ProgFileMaxLen = 1024 * 610;
        uint ProgLen_350 = 384 * 1024;
        volatile uint LibSizeFM350;
        //uint ProgLibLen_350 = 64 * 1024;
        volatile int HasLib350_flag;
        volatile int FullProgFile350_flag = 0;
        double FM350_tprog;
        double FM350_terase;
        int ProgFileLenth;
        int nRowcount;
        int dataGridView_flag = 2;
        int ProgOrReadEE_flag;
        int ProgOrReadEE_FF_flag = 1;
        int ProgOrReadEE_80_flag = 1;
        int ProgOrReadEE_81_flag = 1;
        int ProgOrReadEE_82_flag = 1;
        int ProgOrReadEE_83_flag = 1;
        int ProgOrReadEE_84_flag = 1;
        int ProgOrReadEE_85_flag = 1;
        int ProgOrReadEE_86_flag = 1;
        int ProgOrReadEE_90_flag = 1;
        int ProgOrReadEE_EXT_flag = 0;
        int SoftwareStartFlag = 0;
        int open_flag = 0;
        int SaveAs_flag = 0;
        int temp_cwd3;
        int nProgRamStartTemp = 0;

        bool fm347_data_erase = false; 
        //        byte[] Progfilebuf = new byte[1024 * 448];      //最大文件256k+127K+LIB
        //        byte[] Readfilebuf = new byte[1024 * 448];      //最大文件256k
        //        byte[] Testbuf = new byte[2048 * 640];      //最大文件256k
        byte[] Progfilebuf = new byte[1024 * 610];      //最大文件256k+127K+LIB
        byte[] Readfilebuf = new byte[1024 * 610];      //最大文件256k
        byte[] Testbuf = new byte[2048 * 900];      //最大文件256k
        byte[] hexfiledata, bin_data;
        List<String> ScriptData = new List<String>();

        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public const int WM_VSCROLL = 0x0115;
        public const int SB_LINEDOWN = 7;
        string[] ListReaders;
        //       Progfilebuf = new byte[ProgFileMaxLen];

        public struct ScriptVariable
        {
            public int scrCodeNum;
            public int scrCodeIndex;
            public string scrCode;
            public string[] code;
            //....TODO
        }

        ScriptEngine engine;
        ScriptScope scope;


        public MainForm()
        {
            InitializeComponent();
            InitializeScript();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public string DecimalToBinary(int decimalNum)
        {
            string binaryNum = Convert.ToString(decimalNum, 2);
            int j = binaryNum.Length;
            if (j < 8)
            {
                for (int i = 0; i < 8 - j; i++)
                {
                    binaryNum = '0' + binaryNum;
                }
            }
            return binaryNum;
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

        private void InitializeScript()
        {

            //           this.tabControl.Focus();

            engine = Python.CreateEngine(); // ScriptEngine
            scope = engine.CreateScope();   // ScriptScope

            ScriptRuntime runtime = engine.Runtime;
            runtime.LoadAssembly(typeof(String).Assembly);
            runtime.LoadAssembly(typeof(Uri).Assembly);

            try
            {
                //导入脚本文件
                string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string ScriptDir = Path.Combine(rootDir, "Python Scripts");
                InitializeScript(ScriptDir);
            }
            catch
            {
            }
        }

        private void InitializeScript(string ScriptDir)
        {
            //string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string ScriptDir = Path.Combine(folderName, "Scripts");
            //List<String> ScriptData = new List<String>();
            ScriptData.Clear();

            foreach (string ScrFiles in Directory.GetFiles(ScriptDir, "*.py", SearchOption.AllDirectories))
            {
                string name = Path.GetFileNameWithoutExtension(ScrFiles);
                string data = File.ReadAllText(ScrFiles);
                Scripts_list.Items.Add(name);
                ScriptData.Add(data);

            }
            GC.Collect();
            Scripts_list.SelectedIndexChanged += delegate(object sender, EventArgs e)
            {
                Scripts_code.Text = ScriptData[Scripts_list.SelectedIndex];
                Scripts_code.SelectAll();
                Scripts_code.SelectionColor = Color.Black;

            };
        }
        /*        private void Run_Script_Click(object sender, EventArgs e)   //按照整个文件来跑Python脚本
                {
                    display = "";

                    scope.SetVariable("Card", FM12XX_Card);
                    scope.SetVariable("this", this);
                    scope.SetVariable("msg", display);

           

                    string ScrCode = Scripts_code.Text;
                    try
                    {
                        ScriptSource source = engine.CreateScriptSourceFromString(ScrCode, SourceCodeKind.Statements);
                        source.Execute(scope);

                        string str = scope.GetVariable<string>("msg");
                        if (str != "")
                            DisplayMessageLine(str);
                    }
                    catch (Exception ex)
                    {
                        ExceptionOperations eo;
                        eo = engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(ex);

                        MessageBox.Show(error, "There was an Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                }
        */
        private void Script_backgroundWorker_DoWork(object sender, DoWorkEventArgs e) //复杂脚本运行
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            display = "";
            scope.SetVariable("card", FM12XX_Card);
            scope.SetVariable("this", this);
            scope.SetVariable("msg", display);

            int ProgressPercentage;

            string ScrCode = "";
            string str;
            string nextLineStr = "";
            string[] codeArray = Scripts_code.Lines;
            int countLine = codeArray.Length;
            int lineStart = 0;
            int lineEnd = 0;
            //TODO  添加按行执行的脚本程序 单行解析的时候，python定义的函数处理有问题
            //foreach (string str in Scripts_code.Lines)

            for (countLine = 0; countLine < codeArray.Length; countLine++)
            {
                if (bw.CancellationPending) //线程结束就停止
                {
                    e.Cancel = true;
                    return;
                }
                ProgressPercentage = (countLine + 1) * 100 / codeArray.Length;
                bw.ReportProgress(ProgressPercentage);//Script_backgroundWorker_progressBar

                str = codeArray[countLine];
                if (countLine < codeArray.Length - 1)
                    nextLineStr = codeArray[countLine + 1];
                else
                    nextLineStr = "";

                if (str == "")
                {
                    continue;
                }
                else if (str.Substring(0, 1) == "#")
                {
                    lineEnd = Scripts_code.Text.IndexOf("\n", lineStart);
                    Scripts_code.Select(lineStart, lineEnd);
                    lineStart = lineEnd + 1;
                    //Scripts_code.SelectionColor = Color.Gray;
                    continue;
                }

                //if (nextLineStr != "" && nextLineStr.Substring(0, 2) == "  ") //函数定义或者if判断，循环等中间不要有空行
                if ((nextLineStr != "") && ((nextLineStr.Substring(0, 2) == "  ") || (nextLineStr.Substring(0, 4) == "else")))//函数定义或者if判断，循环等中间不要有空行
                {
                    ScrCode += str + "\r\n";

                    lineEnd = Scripts_code.Text.IndexOf("\n", lineStart);
                    Scripts_code.Select(lineStart, lineEnd);
                    lineStart = lineEnd + 1;
                    //Scripts_code.SelectionColor = Color.Blue;

                    continue;
                }

                ScrCode += str + "\r\n";
                try
                {
                    ScriptSource source = engine.CreateScriptSourceFromString(ScrCode, SourceCodeKind.Statements);
                    source.Execute(scope);
                    ScrCode = "";

                    lineEnd = Scripts_code.Text.IndexOf("\n", lineStart);
                    Scripts_code.Select(lineStart, lineEnd);
                    lineStart = lineEnd + 1;
                    //Scripts_code.SelectionColor = Color.Blue;
                    //Scripts_code.SelectionBackColor = Color.Goldenrod;

                    string strDisplay = scope.GetVariable<string>("msg");
                    if (strDisplay != "")
                        DisplayMessageLine(strDisplay);
                    strDisplay = "";
                    scope.SetVariable("msg", "");
                }
                catch (Exception ex)
                {
                    //                    ExceptionOperations eo;
                    //                    eo = engine.GetService<ExceptionOperations>();
                    //                    string error = eo.FormatException(ex);
                    DisplayMessageLine("Script Error : " + ex.Message);

                    /*                    MessageBox.Show(error, "There was an Error",
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Error);
                    */
                    return;
                }
            }
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void Script_backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Script_backgroundWorker_progressBar.Value = e.ProgressPercentage;
        }

        private void Script_backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DisplayMessageLine("脚本运行完成");
            Script_backgroundWorker_progressBar.Value = 100;
            //Scripts_code.Enabled = true;
        }


        private void Run_Script_btn_Click(object sender, EventArgs e)
        {
            if ((Script_backgroundWorker.IsBusy) | (SimpleScript_backgroundWorker.IsBusy))
            {
                //MessageBox.Show("脚本正在运行，请耐心等待", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayMessageLine("脚本运行中，请停止后再开始！");
                return;
            }
            else
            {
                Script_backgroundWorker_progressBar.Value = 0;
                Script_backgroundWorker.RunWorkerAsync();
                //Scripts_code.Enabled = false;
            }
        }
        private void Stop_RunScript_btn_Click(object sender, EventArgs e)
        {
            //if (Script_backgroundWorker.IsBusy)
            Script_backgroundWorker.CancelAsync();
        }

        public void DisplayMessageLine(string display)
        {
            string str;
            if (main_note_idx >= 500)
            {
                main_note_idx = 0;
                main_note.Clear();
            }
            main_note_idx++;
            str = Convert.ToString(main_note_idx) + ">\t" + display + "\n";
            main_note.AppendText(str);
        }
        private void main_note_TextChanged(object sender, EventArgs e)
        {
            this.main_note.Select(this.main_note.Text.Length, 0);
            this.main_note.ScrollToCaret();
        }

        /// <summary>
        /// DeleteSpaceString(string hexString)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteSpaceString(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            hexString = hexString.Replace("\t", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString; //如果最后不足两位，最后添“0”。

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
        private void RegAddr_17_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void RegData_17_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        public void KeyFilter(KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && e.KeyChar != 0x20 && (e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'a' || e.KeyChar > 'f') && (e.KeyChar < 'A' || e.KeyChar > 'F')
                && e.KeyChar != 0x01 && e.KeyChar != 0x03 && e.KeyChar != 0x16 && e.KeyChar != 0x18)
            {                                                   //keyChar:01->Ctrl+A   03->Ctrl+C
                //keyChar:16->Ctrl+V   18->Ctrl+X
                e.Handled = true;

            }
        }
        private void SendData_KeyPress(object sender, KeyPressEventArgs e)
        {
            TransceiveDataCL_listBox.SelectedItem = null;
            TransceiveDataCT_listBox.SelectedItem = null;
            KeyFilter(e);
        }

        private void BlockAddr_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        public bool IsFMReaderV2() //判断是否为FMReader新卡机   
        {
            if (readers_cbox.Text.Contains("FMSH Reader V2."))
            {
                FM12XX_Card.isFMV02 = 1;
                return true;
            }

            else 
            {
                FM12XX_Card.isFMV02 = 0;
                return false;
            }
               
        }
 

        string[] getReaders()
        {
           string[] ListReaders;
            
            ListReaders = FM12_Reader_Context.GetReaders();
            readers_cbox.Items.Clear();
            foreach (string reader in ListReaders)
            {
                readers_cbox.Items.Add(reader);
            }
            readers_cbox.Text = readers_cbox.Items[0].ToString();

            return ListReaders;
        }
        private void GetReaders_bnt_Click(object sender, EventArgs e)
        {
            try
            {
                ListReaders = getReaders();
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                return;
            }
        }

        private void ConnectReader_Click(object sender, EventArgs e)
        {
            string reader;
            string FMSH_readers = "";
            string strReceived;
            try
            {
                IntPtr cardPtr;

                reader = readers_cbox.Text;
                if (reader.Contains("FMSH Reader"))
                {
                    FMSH_readers = reader;
                }
                /*
                foreach (string reader in ListReaders)
                {
                    if (reader.Contains("FMSH Reader"))
                    {
                        FMSH_readers = reader;
                        break;
                    }
                }*/

                FM12_Reader_Context.Connect(reader, SmartCardShare.Shared, SmartCardProtocols.T0T1, out cardPtr);

                //FM12_Reader_Context.Connect("FMSH NB1026 Reader V3.1.1 0", SmartCardShare.Shared, SmartCardProtocols.T0T1, out cardPtr);
                if (CardType_Select.SelectedItem.ToString() == "FM274")
                {
                    FM12XX_Card = new FM274card(cardPtr);
                }
                else if (CardType_Select.SelectedItem.ToString() == "FM309")
                {
                    FM12XX_Card = new FM309card(cardPtr);
                }
                else
                {
                    FM12XX_Card = new SmartCard(cardPtr);
                }
                IsFMReaderV2(); //Check FMReaderV20

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                return;
            }
            display = "Connect Reader: " + reader + "\t\tSucceeded";
            this.Text = reader;
            DisplayMessageLine(display);
            if (SoftwareStartFlag == 1)
            {
                if (TypeASel_radioButton.Checked == true)
                {
                    FM12XX_Card.TypeAinit(out strReceived);
                    if (strReceived == "Error")
                    {
                        DisplayMessageLine("Switch to 14443 TypeA:\tFailed!\nTypeA Set FM1715Reg:\tFailed!");
                    }
                    else
                    {
                        DisplayMessageLine("Switch to 14443 TypeA:\tSucceeded!");
                    }
                }
                else
                {
                    FM12XX_Card.TypeBinit(out strReceived);
                    DisplayMessageLine("Switch to 14443 TypeB:\t" + strReceived + "!");
                }
            }
            SoftwareStartFlag = 1;
            /*
            display = "ConnectReader: " + CardType_Select.SelectedItem.ToString() + "\t\tSucceeded";
            this.Text = CardType_Select.SelectedItem.ToString() + " Card";
            DisplayMessageLine(display);
            DisplayMessageLine("Reader version:\t\t" + FMSH_readers.Substring(0, FMSH_readers.Length - 1));
            */
        }

        private void Reset17_Click(object sender, EventArgs e)
        {
            try
            {
                int i = FM12XX_Card.SetField(0, out display);
                display = "SetField     \t\t\t" + display;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void MI_REQA_Click(object sender, EventArgs e)
        {
            try
            {
                textBox_UID1.Text = "----------";
                textBox_SAK1.Text = "--";
                textBox_UID2.Text = "----------";
                textBox_SAK2.Text = "--";
                textBox_UID3.Text = "----------";
                textBox_SAK3.Text = "--";

                FM12XX_Card.REQA(out display);

                display = "ATQA：  \t\t\t" + display;

                DisplayMessageLine(display);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void MI_AntiColl_Click(object sender, EventArgs e)
        {
            try
            {
                FM12XX_Card.AntiColl_lv(FM12XX_Card.UID_level, out display);
                if (FM12XX_Card.UID_level == 1)
                {
                    textBox_UID1.Text = "----------";
                    textBox_SAK1.Text = "--";
                    textBox_UID2.Text = "----------";
                    textBox_SAK2.Text = "--";
                    textBox_UID3.Text = "----------";
                    textBox_SAK3.Text = "--";
                    textBox_UID1.Text = display;
                }
                else if (FM12XX_Card.UID_level == 2)
                {
                    textBox_UID2.Text = "----------";
                    textBox_SAK2.Text = "--";
                    textBox_UID3.Text = "----------";
                    textBox_SAK3.Text = "--";
                    textBox_UID2.Text = display;
                }
                else if (FM12XX_Card.UID_level == 3)
                {
                    textBox_UID3.Text = "----------";
                    textBox_SAK3.Text = "--";
                    textBox_UID3.Text = display;
                }
                display = "UID" + FM12XX_Card.UID_level + "：  \t\t\t" + display;
                if (checkBox_check_BCC.Checked == true)
                {
                    display += FM12XX_Card.check_BCC_result;
                    FM12XX_Card.check_BCC_result = "";
                }
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void MI_Select_Click(object sender, EventArgs e)
        {
            string tmp;
            tmp = "";
            FM12XX_Card.enable_SAK = (checkBox_last_SAK.Checked) ? 1 : 0;
            try
            {
               FM12XX_Card.Select(FM12XX_Card.UID_level, out display);
                if (FM12XX_Card.UID_level == 1)
                {
                    textBox_SAK1.Text = display;
                }
                else if (FM12XX_Card.UID_level == 2)
                {
                    textBox_SAK2.Text = display;
                }
                else if (FM12XX_Card.UID_level == 3)
                {
                    textBox_SAK3.Text = display;
                }

                if (FM12XX_Card.enable_SAK == 1)
                {
                    if ((display != FM12XX_Card.last_SAK) && (FM12XX_Card.UID_level < 3))
                    {
                        FM12XX_Card.UID_level++;
                    }
                    else
                    {
                        if (display == FM12XX_Card.last_SAK)
                        {
                            tmp = "选卡完成";
                            FM12XX_Card.UID_level = 1;           //选卡完成后，UID_level复位
                        }
                        else
                        {
                            tmp = "选卡失败，预设的SAK是" + FM12XX_Card.last_SAK;
                        }
                        FM12XX_Card.card_selected_flag = 1;     //无论最后一个SAK是什么都认为选卡完成
                    }
                }
                else
                {
                    FM12XX_Card.card_selected_flag = 1;
                }

                display = "Sak：  \t\t\t" + display;
                if (FM12XX_Card.card_selected_flag == 1)
                {
                    display += "\t" + tmp;
                }
                DisplayMessageLine(display);
                //display = "";
                //display = "Card：  \t\t\tUID level " + FM12XX_Card.UID_level + " is complete";
                //DisplayMessageLine(display);
                //if (FM12XX_Card.card_selected_flag == 1)
                //{
                //    FM12XX_Card.UID_level = 1;
                    //display = "Card：  \t\t\tSAK not 0x04 means selection complete";
                    //DisplayMessageLine(display);
                //}
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Active_Click(object sender, EventArgs e)
        {
            display = "";
            FM12XX_Card.last_SAK = textBox_last_SAK.Text;
            FM12XX_Card.enable_SAK = (checkBox_last_SAK.Checked) ? 1 : 0;
            try
            {
                FM12XX_Card.Active(out display);
                DisplayMessageLine(display);
                if (FM12XX_Card.UID_numbers > 0)
                {
                    textBox_UID1.Text = FM12XX_Card.UID1;
                    textBox_SAK1.Text = FM12XX_Card.SAK1;
                }
                else
                {
                    textBox_UID1.Text = "----------";
                    textBox_SAK1.Text = "--";
                    textBox_UID2.Text = "----------";
                    textBox_SAK2.Text = "--";
                    textBox_UID3.Text = "----------";
                    textBox_SAK3.Text = "--";
                    return; 
                }
                if (FM12XX_Card.UID_numbers > 1)
                {
                    textBox_UID2.Text = FM12XX_Card.UID2;
                    textBox_SAK2.Text = FM12XX_Card.SAK2;
                }
                else
                {
                    textBox_UID2.Text = "----------";
                    textBox_SAK2.Text = "--";
                    textBox_UID3.Text = "----------";
                    textBox_SAK3.Text = "--";
                    return;
                }
                if (FM12XX_Card.UID_numbers > 2)
                {
                    textBox_UID3.Text = FM12XX_Card.UID3;
                    textBox_SAK3.Text = FM12XX_Card.SAK3;
                }
                else
                {
                    textBox_UID3.Text = "----------";
                    textBox_SAK3.Text = "--";
                    return;
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Read17Reg_Click(object sender, EventArgs e)
        {
            string regAddrStr_17, regDataStr_17, strReceived;
            byte regData;

            while (RegAddr_17_textBox.Text.Length < 2)
            {
                RegAddr_17_textBox.Text = "0" + RegAddr_17_textBox.Text;
            }
            //regAddr = Convert.ToByte(regAddrStr_17, 16);
            regAddrStr_17 = RegAddr_17_textBox.Text;

            try
            {
                int i = FM12XX_Card.Read_FM1715_reg(regAddrStr_17, out strReceived, out regData);
                regDataStr_17 = regData.ToString("X2");

                if (i == 0)
                    RegData_17_textBox.Text = regDataStr_17;
                else
                    RegData_17_textBox.Text = "ER";
                display = "Read 17 Reg 0x" + regAddrStr_17 + ":    \t" + strReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Write17Reg_Click(object sender, EventArgs e)
        {

            string regAddrStr_17, regDataStr_17, StrReceived;
            // byte regAddr, regData;
            try
            {

                while (RegAddr_17_textBox.Text.Length < 2)
                {
                    RegAddr_17_textBox.Text = "0" + RegAddr_17_textBox.Text;
                }
                regAddrStr_17 = RegAddr_17_textBox.Text;

                while (RegData_17_textBox.Text.Length < 2)
                {
                    RegData_17_textBox.Text = "0" + RegData_17_textBox.Text;
                }
                regDataStr_17 = RegData_17_textBox.Text;
                //regAddr = Convert.ToByte(regAddrStr_17, 16);
                //regData = Convert.ToByte(regDataStr_17, 16);


                FM12XX_Card.Set_FM1715_reg(regAddrStr_17, regDataStr_17, out StrReceived);
                display = "Write FM17 Reg 0x" + regAddrStr_17 + ": \t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }


        private void TransceiveCL_Click(object sender, EventArgs e)
        {
            string data, crc_parity_cfg, timeout, APDU_data = "", P3;

            data = DeleteSpaceString(SendData_textBox.Text);
            if (checkBox_APDUmodeCL.Checked == true)
            {
                if (data != "")
                {
                    P3 = (data.Length / 2).ToString("X2");
                    textBox_APDU_P3_CL.Text = P3;
                }
                if (textBox_APDU_Le_CL.Text == "00")
                {
                    APDU_data = textBox_APDU_CLA_CL.Text + textBox_APDU_INS_CL.Text + textBox_APDU_P1_CL.Text + textBox_APDU_P2_CL.Text + " " + textBox_APDU_P3_CL.Text + " " + data;
                }
                else
                {
                    APDU_data = textBox_APDU_CLA_CL.Text + textBox_APDU_INS_CL.Text + textBox_APDU_P1_CL.Text + textBox_APDU_P2_CL.Text + " " + textBox_APDU_P3_CL.Text + " " + data + " " + textBox_APDU_Le_CL.Text;
                }
            }
            //in direct send mode CRC and parity can be manipulated——CJN20150807
            //check parity mode
            if (cla_odd_parity_cfg.Checked)
                crc_parity_cfg = "0";
            else if (cla_even_parity_cfg.Checked)
                crc_parity_cfg = "1";
            else if (cla_no_parity_cfg.Checked)
                crc_parity_cfg = "2";
            else
                crc_parity_cfg = "0";          //default set_up

            //check CRC mode
            if (all_crc_cfg.Checked)
                crc_parity_cfg += "1";
            else if (no_crc_cfg.Checked)
                crc_parity_cfg += "0";
            else if (rx_crc_cfg.Checked)
                crc_parity_cfg += "2";
            else if (tx_crc_cfg.Checked)
                crc_parity_cfg += "3";
            else
                crc_parity_cfg += "1";

            timeout = TimeOut_textBox.Text;
            if (timeout.Length < 2)
            {
                timeout = "0" + timeout;
                TimeOut_textBox.Text = timeout;
            }

            try
            {
                if (checkBox_APDUmodeCL.Checked == false)
                {
                    DisplayMessageLine("Data Send:    \t->\t" + data);
                    int i = FM12XX_Card.TransceiveCL(data, crc_parity_cfg, timeout, out receive);
                }
                else
                {
                    DisplayMessageLine("Data Send:    \t->\t" + APDU_data);
                    int i = FM12XX_Card.SendAPDUCL(APDU_data, out receive);
                }

                display = "Data Received:\t<-\t" + receive;

                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }

        }

        private void ReadBlock_Click(object sender, EventArgs e)
        {
            string block_addr, StrReceived = "";
            display = "";

            while (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
            block_addr = BlockAddr_textBox.Text;

            try
            {
                int i = FM12XX_Card.ReadBlock(block_addr, out StrReceived);

                display = "ReadBlock " + block_addr + ":\t\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }

        }

        private void WriteBlock_Click(object sender, EventArgs e)
        {
            string block_addr, block_data, StrReceived;


            if (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
            block_addr = BlockAddr_textBox.Text;

            block_data = DeleteSpaceString(SendData_textBox.Text);
            if (block_data.Length != 32)
            {
                display = "WriteBlock:      \t\t数据长度错误！";
                DisplayMessageLine(display);
                return;
            }

            try
            {
                FM12XX_Card.WriteBlock(block_addr, block_data, out StrReceived);
                display = "WriteBlock " + block_addr + ":   \t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void SendData_textBox_TextChanged(object sender, EventArgs e)
        {
            string str = SendData_textBox.Text.Replace(" ", "");
            float i = str.Length / 2F;
            SendData_label.Text = " " + i.ToString() + " Byte";
        }

        private void Init_TDA8007_Click(object sender, EventArgs e)
        {
            string strReceived;
            parity_even_rdBtn.Checked = true;
            fm347_TypeAInfo = "";
            try
            {
                /*int i = FM12XX_Card.SetField(0, out strReceived);
                display = "Set17Field     \t\t\t" + strReceived;
                DisplayMessageLine(display);*/
                FM12XX_Card.Set_TDA8007_reg(TDA8007Reg.GTR, 0x00, out strReceived);
                DisplayMessageLine("Set EGT : 00");

                FM12XX_Card.Init_TDA8007(out strReceived);
                display = "Init TDA8007:\t\t\t" + strReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void ReadTDAReg_Click(object sender, EventArgs e)
        {
            string regAddrStr, regDataStr, strReceived;
            byte regData;
            try
            {
                while (RegAddr_TDA_textBox.Text.Length < 2)
                {
                    RegAddr_TDA_textBox.Text = "0" + RegAddr_TDA_textBox.Text;
                }
                //regAddr = Convert.ToByte(regAddrStr, 16);
                regAddrStr = RegAddr_TDA_textBox.Text;

                int i = FM12XX_Card.Read_TDA8007_reg(regAddrStr, out strReceived, out regData);
                regDataStr = regData.ToString("X2");
                if (i == 0)
                {
                    RegData_TDA_textBox.Text = regDataStr;
                }
                else
                {
                    RegData_TDA_textBox.Text = "ER";
                }
                display = "Read TDA8007 Reg 0x" + regAddrStr + ": \t" + strReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void WriteTDAReg_Click(object sender, EventArgs e)
        {
            string regAddrStr, regDataStr, strReceived;
            //byte regAddr, regData;
            while (RegAddr_TDA_textBox.Text.Length < 2)
            {
                RegAddr_TDA_textBox.Text = "0" + RegAddr_TDA_textBox.Text;
            }
            regAddrStr = RegAddr_TDA_textBox.Text;

            while (RegData_TDA_textBox.Text.Length < 2)
            {
                RegData_TDA_textBox.Text = "0" + RegData_TDA_textBox.Text;
            }
            regDataStr = RegData_TDA_textBox.Text;
            //regAddr = Convert.ToByte(regAddrStr, 16);
            //regData = Convert.ToByte(regDataStr, 16);

            try
            {
                FM12XX_Card.Set_TDA8007_reg(regAddrStr, regDataStr, out strReceived);
                display = "Write TDA8007 Reg 0x" + regAddrStr + ": \t" + strReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Cold_Reset_Click(object sender, EventArgs e)
        {
            string strReceived = "", volt = "";
            if (Volt30.Checked)
                volt = "02";
            else if (Volt50.Checked)
                volt = "01";
            else if (Volt18.Checked)
                volt = "03";
            fm347_TypeAInfo = "";
            try
            {
                int i = FM12XX_Card.Cold_Reset(volt, out strReceived);

                display = "ColdReset ATR:    \t\t" + strReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Field_ON_Click(object sender, EventArgs e)
        {
            string StrReceived;
            try
            {
                if (checkBox_check_BCC.Checked == true)     //initialization
                    FM12XX_Card.check_BCC = 1;
                else
                    FM12XX_Card.check_BCC = 0;
                FM12XX_Card.SetField(1, out display);
                display = "SetField     \t\t\t" + display;
                DisplayMessageLine(display);

                FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out StrReceived);        //MFOUT = Miller编码并调制过的内部信号
                display = "Write 17Reg 0x26(0x02)" + ": \t" + StrReceived;
                DisplayMessageLine(display);
////bailiang fm336test 2014.10.1
//                FM12XX_Card.Set_FM1715_reg(0x3D, 0xF4, out StrReceived);
//                display = "Write 17Reg 0x3D(0xF4)" + ": \t" + StrReceived;
//                DisplayMessageLine(display);
                //数据发送后，接收器等待RxWait 定义的bit 时钟数，在这段时间内，Rx 上收到的任何信号都被忽略
                FM12XX_Card.Set_FM1715_reg(0x21, 0x06, out StrReceived);        
                display = "Write 17Reg 0x21(0x06)" + ": \t" + StrReceived;
                DisplayMessageLine(display);
                FM12XX_Card.Set_FM1715_reg(0x1E, 0x01, out StrReceived);        //解码控制源，01-时钟开启接收器，02-接收器在接收数据前自动打开和接受数据后自动关闭，用于节省功耗（该功能有缺陷）
                display = "Write 17Reg 0x1E(0x41)" + ": \t" + StrReceived;
                DisplayMessageLine(display);
///bailiang fm336test 2014.10.1
                if (TypeBSel_radioButton.Checked == true)
                    FM12XX_Card.TypeBinit(out display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Warm_Reset_Click(object sender, EventArgs e)
        {
            string strReceived = "", volt = "";
            if (Volt30.Checked)
                volt = "02";
            else if (Volt50.Checked)
                volt = "01";
            else if (Volt18.Checked)
                volt = "03";
            try
            {
                int i = FM12XX_Card.Warm_Reset(volt, out strReceived);

                display = "WarmReset ATR:     \t\t" + strReceived;

                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Clock_Stop_Btn_Click(object sender, EventArgs e)
        {
            bool clkSta = true;
            byte data;
            string strReceived;
            try
            {
                if (Clock_Stop_Btn.Text == "ClockStop")
                {
                    clkSta = true;
                    Clock_Stop_Btn.Text = "ClockResume";
                }
                else if (Clock_Stop_Btn.Text == "ClockResume")
                {
                    clkSta = false;
                    Clock_Stop_Btn.Text = "ClockStop";
                }

                FM12XX_Card.Read_TDA8007_reg(TDA8007Reg_Str.CCR, out strReceived, out data);
                if (CST_High.Checked)
                    data |= 0x20;
                if (CST_Low.Checked)
                    data &= 0xDF;
                if (!clkSta)
                {
                    data &= 0xEF;
                    FM12XX_Card.Set_TDA8007_reg(TDA8007Reg_Str.CCR, data.ToString("X2"), out strReceived);
                    DisplayMessageLine("Clock: \t\t\tResumed");
                }
                else
                {
                    data |= 0x10;
                    FM12XX_Card.Set_TDA8007_reg(TDA8007Reg_Str.CCR, data.ToString("X2"), out strReceived);
                    DisplayMessageLine("Clock: \t\t\tStopped");
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void TransceiveCT_Click(object sender, EventArgs e)
        {
            string data, timeout, APDU_data = "", P3;
            timeout = "01";
            data = SendData_textBox.Text;

            if (CT_TimeOut_std.Checked)
                timeout = "01";
            if (CT_TimeOut_100ETU.Checked)
                timeout = "02";
            if (CT_TimeOut_NoTimeout.Checked)
                timeout = "03";

            data = DeleteSpaceString(SendData_textBox.Text);
            if (checkBox_APDUmodeCT.Checked == true)
            {
                if (data != "")
                {
                    P3 = (data.Length / 2).ToString("X2");
                    textBox_APDU_P3_CT.Text = P3;
                }
                APDU_data = textBox_APDU_CLA_CT.Text + textBox_APDU_INS_CT.Text + textBox_APDU_P1_CT.Text + textBox_APDU_P2_CT.Text + " " + textBox_APDU_P3_CT.Text + " " + data;
            }

            try
            {
                if (checkBox_APDUmodeCT.Checked == false)
                {
                    DisplayMessageLine("Data Send:    \t->\t" + DeleteSpaceString(data));
                    int i = FM12XX_Card.TransceiveCT(data, timeout, out receive);

                }
                else
                {
                    DisplayMessageLine("Data Send:    \t->\t" + APDU_data);
                    int i = FM12XX_Card.SendAPDUCT(APDU_data, out receive);
                }
                display = "Data Received:  \t<-\t" + receive;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void Open_Scr_dir_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog_Scr.ShowDialog();
            if (result == DialogResult.OK)
            {
                ScriptDir = folderBrowserDialog_Scr.SelectedPath;
                Scripts_list.Items.Clear();
                InitializeScript(ScriptDir);       //TODO  此处调用此函数，里面的委托函数会出错。 2011-05-17                      

            }

        }

        private void Open_ScriptFile_Click(object sender, EventArgs e)
        {

            string[] ScrCode, CompareData, RawCode, ControlCode;
            string tempStr, compStr;
            int i = 0, count = 0, position_CompareData = 0;

            if (SimpleScript_backgroundWorker.IsBusy)
            {
                //                MessageBox.Show("脚本正在运行，请耐心等待", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayMessageLine("脚本运行中，请停止后再开始！");
                return;
            }
            openFileDialog_Prog.Reset();
            openFileDialog_Scr.Reset();
            openFileDialog_Scr = new OpenFileDialog();
            openFileDialog_Scr.InitialDirectory = ScriptDirectory.Substring(0, ScriptDirectory.LastIndexOf("\\"));
            openFileDialog_Scr.RestoreDirectory = true;

            if (dataGridView_Script.RowCount <= 1 || dataGridView_flag == 0 || dataGridView_flag == 2)
            {
                dataGridView_flag = 1;
                dataGridView_Script.Rows.Clear();
            }
            openFileDialog_Scr.Filter = "309 Script files|*.309|fmsh Script files|*.fm|python files|*.py|All files|*.*";
            if (openFileDialog_Scr.ShowDialog() == DialogResult.OK)
            {
                ScriptDirectory = openFileDialog_Scr.FileName;

                System.Text.Encoding.GetEncoding("gb2312");
                RawCode = File.ReadAllLines(openFileDialog_Scr.FileName, System.Text.Encoding.Default);
                ScrCode = new string[RawCode.Length];
                CompareData = new string[RawCode.Length];
                ControlCode = new string[RawCode.Length];

                for (i = 0; i < RawCode.Length; i++)
                {

                    tempStr = RawCode[i];
                    if (tempStr == "")
                    {
                        continue;
                    }
                    position_CompareData = tempStr.IndexOf("@");

                    if (position_CompareData > 0)
                    {
                        ScrCode[count] = tempStr.Substring(0, position_CompareData);
                        compStr = tempStr.Substring(position_CompareData + 1, tempStr.Length - position_CompareData - 1);
                        if (compStr.Contains("pause"))
                        {
                            ControlCode[count] = "●";
                            CompareData[count] = "";
                        }
                        else
                        {
                            compStr = compStr.Split(new Char[] { '#' })[0]; //添加注释
                            CompareData[count] = DeleteSpaceString(compStr);
                            ControlCode[count] = "";
                        }
                        count++;
                    }
                    else
                    {
                        if (tempStr.IndexOf("#") > 0) //添加注释
                            tempStr = tempStr.Split(new Char[] { '#' })[0];
                        ScrCode[count] = tempStr;
                        CompareData[count] = "";
                        ControlCode[count] = "";
                        count++;
                    }
                }
                dataGridView_Script.RowCount = count;

                for (i = 0; i < count; i++)
                {


                    dataGridView_Script.Rows[i].HeaderCell.Value = ControlCode[i];
                    dataGridView_Script["Index", i].Value = i.ToString();
                    dataGridView_Script["Commands", i].Value = ScrCode[i];
                    dataGridView_Script["CompareData", i].Value = CompareData[i];
                }

                RstScrScope_Click(null, EventArgs.Empty);
                ConnectReader_Click(null, EventArgs.Empty);
            }

        }

        private void textBox_APDU_CLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
            TransceiveDataCL_listBox.SelectedItem = null;
        }

        private void textBox_APDU_INS_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
            TransceiveDataCL_listBox.SelectedItem = null;
        }

        private void textBox_APDU_P1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
            TransceiveDataCL_listBox.SelectedItem = null;
        }

        private void textBox_APDU_P2_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
            TransceiveDataCL_listBox.SelectedItem = null;
        }

        private void textBox_APDU_P3_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
            TransceiveDataCL_listBox.SelectedItem = null;
        }

        private void textBox_APDU_Le_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
            TransceiveDataCL_listBox.SelectedItem = null;
        }

        private void checkBox_APDU_mode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_APDUmodeCL.Checked)
            {
                label_APDU_CL.Enabled = true;
                textBox_APDU_CLA_CL.Enabled = true;
                textBox_APDU_INS_CL.Enabled = true;
                textBox_APDU_P1_CL.Enabled = true;
                textBox_APDU_P2_CL.Enabled = true;
                textBox_APDU_P3_CL.Enabled = true;
                textBox_APDU_Le_CL.Enabled = true;
                CRC_gropbox.Enabled = false;
                cla_parity_groupbox.Enabled = false;
                TimeOut_textBox.Enabled = false;
                TransceiveCL.Text = "Send APDU";
            }
            else
            {
                label_APDU_CL.Enabled = false;
                textBox_APDU_CLA_CL.Enabled = false;
                textBox_APDU_INS_CL.Enabled = false;
                textBox_APDU_P1_CL.Enabled = false;
                textBox_APDU_P2_CL.Enabled = false;
                textBox_APDU_P3_CL.Enabled = false;
                textBox_APDU_Le_CL.Enabled = false;
                CRC_gropbox.Enabled = true;
                cla_parity_groupbox.Enabled = true;
                TimeOut_textBox.Enabled = true;
                TransceiveCL.Text = "Direct Send";
            }
        }

        private void textBox_APDU_CLA_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_CLA_CL.Text.Length == 2)
            {
                textBox_APDU_INS_CL.Focus();
            }
        }

        private void textBox_APDU_INS_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_INS_CL.Text.Length == 2)
            {
                textBox_APDU_P1_CL.Focus();
            }
        }

        private void textBox_APDU_P1_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_P1_CL.Text.Length == 2)
            {
                textBox_APDU_P2_CL.Focus();
            }
        }

        private void textBox_APDU_P2_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_P2_CL.Text.Length == 2)
            {
                textBox_APDU_P3_CL.Focus();
            }
        }

        private void textBox_APDU_P3_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_P3_CL.Text.Length == 2)
            {
                SendData_textBox.Focus();
            }
        }

        private void textBox_APDU_Le_CL_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_Le_CL.Text.Length == 2)
            {
                SendData_textBox.Focus();
            }
        }

        private void RstScrScope_Click(object sender, EventArgs e)
        {
            if (SimpleScript_backgroundWorker.IsBusy)
            {
                //                MessageBox.Show("脚本正在运行，请耐心等待", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayMessageLine("脚本运行中，请停止后再开始！");
                return;
            }
            for (int i = 0; i < dataGridView_Script.RowCount; i++)
            {
                dataGridView_Script["ReturnData", i].Value = "";
                dataGridView_Script["Results", i].Value = "";
            }
            dataGridView_Script.Rows[0].Selected = true;
            dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
            //ConnectReader_Click(null, EventArgs.Empty);
        }


        private void dataGridView_Script_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dataGridView_Script_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (((string)dataGridView_Script.CurrentRow.HeaderCell.Value == "") | ((string)dataGridView_Script.CurrentRow.HeaderCell.Value == null))
            {
                //dataGridView_Script.CurrentRow.HeaderCell.Style.ForeColor = Color.Red;
                //               dataGridView_Script.CurrentRow.HeaderCell.Style.SelectionForeColor = Color.Red;
                dataGridView_Script.CurrentRow.HeaderCell.Value = "●";
            }
            else
            {
                dataGridView_Script.CurrentRow.HeaderCell.Value = "";
            }
        }


        private void RunScrStep_simple_Click(object sender, EventArgs e)
        {
            display = "";
            object retVariable = new object();
            scope.SetVariable("card", FM12XX_Card);
            scope.SetVariable("this", this);
            scope.SetVariable("ret", display);
            scope.SetVariable("retVariable", retVariable);

            string ScrCode = "";
            string ScrCodeRaw = "";
            string PyGetVarHead = "retVariable = ";
            string CompareData = "";
            string Get_retVariable_FirstVar = "[1]";
            //        string Get_retVariable_SecondVar = "[2]";

            int CurrentRowSel, CountSum;

            if ((SimpleScript_backgroundWorker.IsBusy) | (Script_backgroundWorker.IsBusy))
            {
                //                MessageBox.Show("脚本正在运行，请耐心等待", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayMessageLine("脚本运行中，请停止后再开始！");
                return;
            }

            if (dataGridView_Script.CurrentCell == null)
                return;
            CurrentRowSel = dataGridView_Script.CurrentCell.RowIndex;
            CountSum = dataGridView_Script.Rows.Count;


            ScrCodeRaw = (String)dataGridView_Script["Commands", CurrentRowSel].Value;
            CompareData = (String)dataGridView_Script["CompareData", CurrentRowSel].Value;

            if ((ScrCodeRaw == "") || (ScrCodeRaw == null) || (ScrCodeRaw.Substring(0, 1) == "#"))
            {
                if (CurrentRowSel + 1 == CountSum)
                {
                    dataGridView_Script.Rows[0].Selected = true;
                    dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
                }
                else
                {
                    dataGridView_Script.Rows[CurrentRowSel + 1].Selected = true;
                    dataGridView_Script.CurrentCell = dataGridView_Script.Rows[CurrentRowSel + 1].Cells["Commands"];
                }
                return;
            }
            else if (ScrCodeRaw.Substring(0, 4) == "card")//卡类操作，要求有返回值
            {
                ScrCode = PyGetVarHead + ScrCodeRaw + Get_retVariable_FirstVar + "\r\n";
            }
            else
            {
                ScrCode = ScrCodeRaw + "\r\n";
            }
            try
            {
                ScriptSource source = engine.CreateScriptSourceFromString(ScrCode, SourceCodeKind.Statements);
                source.Execute(scope);
                ScrCode = "";

                if (CurrentRowSel + 1 == CountSum)
                {
                    //dataGridView_Script.Rows[0].Selected = true;
                    //dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
                    dataGridView_Script.Rows[CurrentRowSel].Selected = true;
                    dataGridView_Script.CurrentCell = dataGridView_Script.Rows[CurrentRowSel].Cells["Commands"];
                    DisplayMessageLine("已执行到脚本末尾！");
                }
                else
                {
                    dataGridView_Script.Rows[CurrentRowSel + 1].Selected = true;
                    dataGridView_Script.CurrentCell = dataGridView_Script.Rows[CurrentRowSel + 1].Cells["Commands"];
                }

                // System.Threading.Thread.Sleep(100);

                object obj = scope.GetVariable("retVariable");
                if ((obj != null) && (obj.GetType() == typeof(string)))
                {
                    display = obj.ToString();
                    if (display != "")
                    {
                        //DisplayMessageLine(display);
                        dataGridView_Script["ReturnData", CurrentRowSel].Value = display;
                        dataGridView_Script["Results", CurrentRowSel].Value = "数据返回";
                        dataGridView_Script["Results", CurrentRowSel].Style.ForeColor = Color.Black;
                        display = "";
                    }
                }
                else
                {
                    dataGridView_Script["ReturnData", CurrentRowSel].Value = "";
                    dataGridView_Script["Results", CurrentRowSel].Value = "";
                    display = "";
                }


                if ((CompareData != "") && (CompareData != null))
                {

                    if (DeleteSpaceString(CompareData).ToUpper() == DeleteSpaceString((String)dataGridView_Script["ReturnData", CurrentRowSel].Value).ToUpper())
                    {
                        dataGridView_Script["Results", CurrentRowSel].Value = "比较成功";
                        dataGridView_Script["Results", CurrentRowSel].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        dataGridView_Script["Results", CurrentRowSel].Value = "比较失败";
                        dataGridView_Script["Results", CurrentRowSel].Style.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                /*                    ExceptionOperations eo;
                                    eo = engine.GetService<ExceptionOperations>();
                                    string error = eo.FormatException(ex);
                                    */
                dataGridView_Script["Results", CurrentRowSel].Value = ex.Message;
                dataGridView_Script["Results", CurrentRowSel].Style.ForeColor = Color.Red;

                if (CurrentRowSel + 1 == CountSum)
                {
                    //dataGridView_Script.Rows[0].Selected = true;
                    //dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
                    dataGridView_Script.Rows[CurrentRowSel].Selected = true;
                    dataGridView_Script.CurrentCell = dataGridView_Script.Rows[CurrentRowSel].Cells["Commands"];
                }
                else
                {
                    dataGridView_Script.Rows[CurrentRowSel + 1].Selected = true;
                    dataGridView_Script.CurrentCell = dataGridView_Script.Rows[CurrentRowSel + 1].Cells["Commands"];
                }
                return;
            }

        }

        private void RunScr_simple_Click(object sender, EventArgs e)
        {
            if ((SimpleScript_backgroundWorker.IsBusy) | (Script_backgroundWorker.IsBusy))
            {
                //                MessageBox.Show("脚本正在运行，请耐心等待", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayMessageLine("脚本运行中，请停止后再开始！");
                return;
            }
            else
            {
                SimpleScript_backgroundWorker.RunWorkerAsync();
                dataGridView_Script.Enabled = false;
            }
        }
        private void StopRunScr_simple_Click(object sender, EventArgs e)
        {
            SimpleScript_backgroundWorker.CancelAsync();
        }


        private void SimpleScript_backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DisplayMessageLine("脚本运行完成");
            dataGridView_Script.Enabled = true;
        }

        private void SimpleScript_backgroundWorker_DoWork(object sender, DoWorkEventArgs e) //连续运行脚本的后台程序
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            bool valReturn = false;
            display = "";
            object retVariable = new object();
            scope.SetVariable("card", FM12XX_Card);
            scope.SetVariable("this", this);
            scope.SetVariable("ret", display);
            scope.SetVariable("retVariable", retVariable);

            string ScrCode = "";
            string ScrCodeRaw = "";
            string PyGetVarHead = "retVariable = ";
            string CompareData = "";
            string Get_retVariable_FirstVar = "[1]";
            string Scr_test = "";
            //        string Get_retVariable_SecondVar = "[2]";

            int CurrentRowSel, BreakPointRow, CountSum;
            int countLine, i;
            int LineTemp, countfor = 1;

            //TODO  添加按行执行的脚本程序 单行解析的时候，python定义的函数处理有问题
            //foreach (string str in Scripts_code.Lines)

            /*                for (i = 0; i < dataGridView_Script.Rows.Count; i++)
                            {
                                string testValue1 = (String)dataGridView_Script["Commands", i].Value; //取值
                                DisplayMessageLine(testValue1);
                            }*/
            if (dataGridView_Script.CurrentCell == null)
                return;
            CurrentRowSel = dataGridView_Script.CurrentCell.RowIndex;
            CountSum = dataGridView_Script.Rows.Count;
            LineTemp = CurrentRowSel;

            i = CurrentRowSel + 1;
            while (i < CountSum)
            {
                if ((String)dataGridView_Script.Rows[i].HeaderCell.Value == "●")
                    break;
                i++;
            }

            BreakPointRow = i;

            for (countLine = CurrentRowSel; countLine < BreakPointRow; countLine++)  //循环执行脚本
            {

                if (bw.CancellationPending) //如果中断线程了即可停止运行
                {
                    e.Cancel = true;
                    return;
                }

                ScrCodeRaw = (String)dataGridView_Script["Commands", countLine].Value;
                ScrCodeRaw = ScrCodeRaw.Trim();                      //删除该行前后的空格  20121008
                CompareData = (String)dataGridView_Script["CompareData", countLine].Value;

                if ((ScrCodeRaw == "") || (ScrCodeRaw == null) || (ScrCodeRaw.Substring(0, 1) == "#"))
                {
                    if (countLine + 1 == CountSum)
                    {
                        dataGridView_Script.Rows[0].Selected = true;
                        dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
                    }
                    else
                    {
                        dataGridView_Script.Rows[countLine + 1].Selected = true;
                        dataGridView_Script.CurrentCell = dataGridView_Script.Rows[countLine + 1].Cells["Commands"];
                    }
                    Thread.Sleep(60);
                    continue;
                }
                else if (ScrCodeRaw.Substring(0, 4) == "card")//卡类操作，要求有返回值
                {
                    ScrCode = PyGetVarHead + ScrCodeRaw + Get_retVariable_FirstVar + "\r\n";
                    valReturn = true;
                }
                else if (ScrCodeRaw.Substring(0, 3) == "for")//for循环  20121008
                {
                    LineTemp = countLine;
                    continue;
                }
                else if (ScrCodeRaw.Substring(0, 5) == "count")//count计数  20121008
                {
                    dataGridView_Script["ReturnData", countLine].Value = countfor;
                    dataGridView_Script["Results", countLine].Style.ForeColor = Color.Black;
                    if (countfor.ToString("D6") == CompareData.ToString().PadLeft(6, '0'))
                    {
                        countfor = 1;
                    }
                    else
                    {
                        countLine = LineTemp;
                        countfor++;
                    }
                    continue;
                }
                else if (ScrCodeRaw.Substring(0, 5) == "NoRev")//直接发送，无返回值   20121008
                {
                    Scr_test = ScrCodeRaw.Substring(8, ScrCodeRaw.Length - 9);
                    try
                    {
                        if (ScrCodeRaw.Substring(5, 2) == "CT")
                        {
                            FM12XX_Card.SendAPDUCT(Scr_test, out receive);
                        }
                        else
                            FM12XX_Card.SendAPDUCL(Scr_test, out receive);
                    }
                    catch (Exception ex)
                    {
                        dataGridView_Script["ReturnData", countLine].Value = "无返回";
                        dataGridView_Script["Results", countLine].Value = "正常返回";
                        dataGridView_Script["Results", countLine].Style.ForeColor = Color.Black;
                        continue;
                    }
                    continue;
                }
                else
                {
                    ScrCode = ScrCodeRaw + "\r\n";
                }

                try
                {
                    ScriptSource source = engine.CreateScriptSourceFromString(ScrCode, SourceCodeKind.Statements);
                    source.Execute(scope);
                    ScrCode = "";
                    ScrCodeRaw = "";

                    Thread.Sleep(40);

                    if (valReturn)
                    {
                        object obj = scope.GetVariable("retVariable");
                        if ((obj != null) && (obj.GetType() == typeof(string)))
                        {
                            display = obj.ToString();
                            if (display != "")
                            {
                                //DisplayMessageLine(display);
                                dataGridView_Script["ReturnData", countLine].Value = display;
                                dataGridView_Script["Results", countLine].Value = "数据返回";
                                dataGridView_Script["Results", countLine].Style.ForeColor = Color.Black;
                                display = "";
                            }
                        }
                    }
                    else
                    {
                        dataGridView_Script["ReturnData", countLine].Value = "";
                        dataGridView_Script["Results", countLine].Value = "";
                        display = "";
                    }
                    valReturn = false;

                    if ((CompareData != "") && (CompareData != null))
                    {

                        if (DeleteSpaceString(CompareData).ToUpper() == DeleteSpaceString((String)dataGridView_Script["ReturnData", countLine].Value).ToUpper())
                        {
                            dataGridView_Script["Results", countLine].Value = "比较成功";
                            dataGridView_Script["Results", countLine].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            dataGridView_Script["Results", countLine].Value = "比较失败";
                            dataGridView_Script["Results", countLine].Style.ForeColor = Color.Red;
                            SimpleScript_backgroundWorker.CancelAsync();
                        }
                    }


                    if (countLine + 1 == CountSum)
                    {
                        //dataGridView_Script.Rows[0].Selected = true;
                        //dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
                        dataGridView_Script.Rows[countLine].Selected = true;
                        dataGridView_Script.CurrentCell = dataGridView_Script.Rows[countLine].Cells["Commands"];
                    }
                    else
                    {
                        dataGridView_Script.Rows[countLine + 1].Selected = true;
                        dataGridView_Script.CurrentCell = dataGridView_Script.Rows[countLine + 1].Cells["Commands"];
                    }
                }

                catch (Exception ex)
                {
                    /*ExceptionOperations eo;
                    eo = engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    */
                    //DisplayMessageLine("Script Error : " + ex.Message);

                    dataGridView_Script["Results", countLine].Value = ex.Message;
                    dataGridView_Script["Results", countLine].Style.ForeColor = Color.Red;
                    SimpleScript_backgroundWorker.CancelAsync();
                    if (countLine + 1 == CountSum)
                    {
                        //dataGridView_Script.Rows[0].Selected = true;
                        //dataGridView_Script.CurrentCell = dataGridView_Script.Rows[0].Cells["Commands"];
                        dataGridView_Script.Rows[countLine].Selected = true;
                        dataGridView_Script.CurrentCell = dataGridView_Script.Rows[countLine].Cells["Commands"];
                    }
                    else
                    {
                        dataGridView_Script.Rows[countLine + 1].Selected = true;
                        dataGridView_Script.CurrentCell = dataGridView_Script.Rows[countLine + 1].Cells["Commands"];
                    }
                    return;
                }
            }
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            main_note.Text = "";
            main_note_idx = 0;
        }

        //直接调用按钮的事件函数的方法  Active_Click(this, new EventArgs());

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Normal && MainForm.ActiveForm != null)
            if (MainForm.ActiveForm != null)
                SendData_label.Location = new Point(MainForm.ActiveForm.Width - 100, SendData_label.Location.Y);
        }

        private void AddCMDtoListBox_CL_btn_Click(object sender, EventArgs e)
        {
            string getCommend, P3, Le;
            string data = DeleteSpaceString(SendData_textBox.Text);

            try
            {
                if (open_cmdlog_flag == 0)
                    CMD_LOG_Directory=".\\CMD_CL_LOG.txt";

                StreamWriter sw = new StreamWriter(CMD_LOG_Directory,true);    //打开命令记录文档

//              StreamWriter sw = new StreamWriter(".\\CMD_CL_LOG.txt",true);    //打开命令记录文档


                if (checkBox_APDUmodeCL.Checked == false)
                {
                    if (SendData_textBox.Text != "")
                    {
                        TransceiveDataCL_listBox.Items.Add(SendData_textBox.Text);
                        SendMessage(TransceiveDataCL_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);

                        sw.WriteLine(SendData_textBox.Text);            //新增命令

                    }
                }
                else
                {
                    if (data != "")
                    {
                        P3 = (data.Length / 2).ToString("X2");
                        textBox_APDU_P3_CL.Text = P3;
                    }
                    if (textBox_APDU_Le_CL.Text == "00")
                    {
                        getCommend = textBox_APDU_CLA_CL.Text + textBox_APDU_INS_CL.Text + textBox_APDU_P1_CL.Text + textBox_APDU_P2_CL.Text + textBox_APDU_P3_CL.Text + SendData_textBox.Text;
                    }
                    else
                    {
                        getCommend = textBox_APDU_CLA_CL.Text + textBox_APDU_INS_CL.Text + textBox_APDU_P1_CL.Text + textBox_APDU_P2_CL.Text + textBox_APDU_P3_CL.Text + SendData_textBox.Text + textBox_APDU_Le_CL.Text;
                    }
                    if (getCommend.Length >= 10)
                    {
                        TransceiveDataCL_listBox.Items.Add(getCommend);
                        SendMessage(TransceiveDataCL_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);

                        sw.WriteLine(getCommend);            //新增命令
                    }
                    else
                    {
                        DisplayMessageLine("APDU格式不对");
                    }
                }
                sw.Close();                                     //关闭文档
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }
        private void Open_CMDlog_button_Click(object sender, EventArgs e)
        {
            string line;
            openFileDialog_Prog = null;
            openFileDialog_Prog = new OpenFileDialog();
            openFileDialog_Prog.Reset();
            openFileDialog_Scr.Reset();
            openFileDialog_Prog.InitialDirectory = CMD_LOG_Directory.Substring(0, CMD_LOG_Directory.LastIndexOf("\\"));// ;
            openFileDialog_Prog.RestoreDirectory = true;
           
            openFileDialog_Prog.Filter = " txt files (*.txt)|*.txt;|All files (*.*)|*.*";

            if (openFileDialog_Prog.ShowDialog() == DialogResult.OK)
            {                
                CMD_LOG_Directory = openFileDialog_Prog.FileName;
                if (tabControl_main.SelectedIndex == 0)
                {
                    TransceiveDataCL_listBox.Items.Clear();
                }
                else
                {
                    TransceiveDataCT_listBox.Items.Clear();
                }
                StreamReader sr = new StreamReader(CMD_LOG_Directory, System.Text.Encoding.GetEncoding("gb2312"));
                while ((line = sr.ReadLine()) != null)
                {
                    if (tabControl_main.SelectedIndex == 0)
                    {
                        TransceiveDataCL_listBox.Items.Add(line);
                    }
                    else
                    {
                        TransceiveDataCT_listBox.Items.Add(line);
                    }
                }
                sr.Close();
                open_cmdlog_flag = 1;
            }
        }
        private void Send_Slected_Command_button_Click(object sender, EventArgs e)
        {
            string getCommend="";
            int count=0;
            if (tabControl_main.SelectedIndex == 0)
                count = TransceiveDataCL_listBox.SelectedItems.Count;
            else if(tabControl_main.SelectedIndex == 1)
                count = TransceiveDataCT_listBox.SelectedItems.Count;

            for (int i = 0; i < count; i++)
            {
                if (tabControl_main.SelectedIndex == 0)
                {
                    getCommend = TransceiveDataCL_listBox.SelectedItems[i].ToString().Split(new Char[] { ':' })[0];
                    getCommend = DeleteSpaceString(getCommend);
                    if (checkBox_APDUmodeCL.Checked)
                    {
                        if (getCommend.Length >= 10)
                        {
                            textBox_APDU_CLA_CL.Text = getCommend.Substring(0, 2);
                            textBox_APDU_INS_CL.Text = getCommend.Substring(2, 2);
                            textBox_APDU_P1_CL.Text = getCommend.Substring(4, 2);
                            textBox_APDU_P2_CL.Text = getCommend.Substring(6, 2);
                            textBox_APDU_P3_CL.Text = getCommend.Substring(8, 2);
                            SendData_textBox.Text = getCommend.Substring(10, getCommend.Length - 10);
                        }
                        else
                        {
                            DisplayMessageLine("APDU指令的长度要大于5byte");
                        }
                    }
                    else
                    {
                        SendData_textBox.Text = getCommend;
                    }

                    TransceiveCL_Click(null, EventArgs.Empty);

                }
                else if (tabControl_main.SelectedIndex == 1)
                {
                    getCommend = TransceiveDataCT_listBox.SelectedItems[i].ToString().Split(new Char[] { ':' })[0];
                    getCommend = DeleteSpaceString(getCommend);
                    if (checkBox_APDUmodeCT.Checked)
                    {
                        if (getCommend.Length >= 10)
                        {
                            textBox_APDU_CLA_CT.Text = getCommend.Substring(0, 2);
                            textBox_APDU_INS_CT.Text = getCommend.Substring(2, 2);
                            textBox_APDU_P1_CT.Text = getCommend.Substring(4, 2);
                            textBox_APDU_P2_CT.Text = getCommend.Substring(6, 2);
                            textBox_APDU_P3_CT.Text = getCommend.Substring(8, 2);
                            SendData_textBox.Text = getCommend.Substring(10, getCommend.Length - 10);
                        }
                        else
                        {
                            DisplayMessageLine("APDU指令的长度要大于5byte");
                        }
                    }
                    else
                    {
                        SendData_textBox.Text = getCommend;
                    }

                    TransceiveCT_Click(null, EventArgs.Empty);
                }
            }
        }
        private void TransceiveDataCL_listBox_MouseClick(object sender, MouseEventArgs e)
        {
            string getCommend;
            if (TransceiveDataCL_listBox.SelectedItem != null)
            {
                getCommend = TransceiveDataCL_listBox.SelectedItem.ToString().Split(new Char[] { ':' })[0];
                getCommend = DeleteSpaceString(getCommend);
                if (checkBox_APDUmodeCL.Checked)
                {
                    if (getCommend.Length >= 10)
                    {
                        textBox_APDU_CLA_CL.Text = getCommend.Substring(0, 2);
                        textBox_APDU_INS_CL.Text = getCommend.Substring(2, 2);
                        textBox_APDU_P1_CL.Text = getCommend.Substring(4, 2);
                        textBox_APDU_P2_CL.Text = getCommend.Substring(6, 2);
                        textBox_APDU_P3_CL.Text = getCommend.Substring(8, 2);
                        if ((Convert.ToInt32(textBox_APDU_P3_CL.Text, 16) == ((getCommend.Length - 10) / 2)) || ((getCommend.Length - 10) == 0))
                        {//case 1 2 3
                            SendData_textBox.Text = getCommend.Substring(10, getCommend.Length - 10);
                            textBox_APDU_Le_CL.Text = "00";
                        }
                        else
                        {//case 4
                            SendData_textBox.Text = getCommend.Substring(10, getCommend.Length - 12);
                            textBox_APDU_Le_CL.Text = getCommend.Substring(getCommend.Length - 2, 2);
                        }
                    }
                    else
                    {
                        DisplayMessageLine("APDU指令的长度要大于5byte");
                    }
                }
                else
                {
                    SendData_textBox.Text = getCommend;
                }
                TransceiveDataCL_listBox.Focus();
            }

        }

        private void textBox_APDU_CLA_CT_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_CLA_CT.Text.Length == 2)
            {
                textBox_APDU_INS_CT.Focus();
            }
        }
        private void textBox_APDU_INS_CT_TextChanged(object sender, EventArgs e)
        {

            if (textBox_APDU_INS_CT.Text.Length == 2)
            {
                textBox_APDU_P1_CT.Focus();
            }

        }
        private void textBox_APDU_P1_CT_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_P1_CT.Text.Length == 2)
            {
                textBox_APDU_P2_CT.Focus();
            }
        }

        private void textBox_APDU_P2_CT_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_P2_CT.Text.Length == 2)
            {
                textBox_APDU_P3_CT.Focus();
            }
        }

        private void textBox_APDU_P3_CT_TextChanged(object sender, EventArgs e)
        {
            if (textBox_APDU_P3_CT.Text.Length == 2)
            {
                SendData_textBox.Focus();
            }
        }

        private void checkBox_APDUmodeCT_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_APDUmodeCT.Checked)
            {
                label_APDU_CT.Enabled = true;
                textBox_APDU_CLA_CT.Enabled = true;
                textBox_APDU_INS_CT.Enabled = true;
                textBox_APDU_P1_CT.Enabled = true;
                textBox_APDU_P2_CT.Enabled = true;
                textBox_APDU_P3_CT.Enabled = true;
                TransceiveCT.Text = "Send APDU";
            }
            else
            {
                label_APDU_CT.Enabled = false;
                textBox_APDU_CLA_CT.Enabled = false;
                textBox_APDU_INS_CT.Enabled = false;
                textBox_APDU_P1_CT.Enabled = false;
                textBox_APDU_P2_CT.Enabled = false;
                textBox_APDU_P3_CT.Enabled = false;
                TransceiveCT.Text = "Direct Send";
            }
        }

        private void button_RATS_Click(object sender, EventArgs e)
        {
            try
            {
                FM12XX_Card.CID = textBox_CID.Text;
                //FM12XX_Card.RATS(out display);
                FM12XX_Card.RATS(FM12XX_Card.CID, out display);         //user defined CID
                display = "ATS： " + display;
                DisplayMessageLine(display);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void SPI_Send_btn_Click(object sender, EventArgs e)
        {
            while (SPI_LenToReceive_textBox.Text.Length < 2)
            {
                SPI_LenToReceive_textBox.Text = "0" + SPI_LenToReceive_textBox.Text;
            }

            try
            {
                FM12XX_Card.SPI_Send(SendData_textBox.Text, SPI_LenToReceive_textBox.Text, out display);
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void I2C_Send_btn_Click(object sender, EventArgs e)
        {
            while (I2C_LenToReceive_textBox.Text.Length < 2)
            {
                I2C_LenToReceive_textBox.Text = "0" + I2C_LenToReceive_textBox.Text;
            }
            try
            {
                FM12XX_Card.I2C_Send(SendData_textBox.Text, I2C_LenToReceive_textBox.Text, out display);
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void TransceiveDataSPI_I2C_listBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (TransceiveDataSPI_I2C_listBox.SelectedItem != null)
                SendData_textBox.Text = TransceiveDataSPI_I2C_listBox.SelectedItem.ToString();
        }

        private void AddtoListBox_SPI_I2C_btn_Click(object sender, EventArgs e)
        {
            if (SendData_textBox.Text != "")
            {
                TransceiveDataSPI_I2C_listBox.Items.Add(SendData_textBox.Text);
                //TransceiveDataSPI_I2C_listBox.SelectedItem = TransceiveDataSPI_I2C_listBox.Items[TransceiveDataSPI_I2C_listBox.Items.Count - 1];
                SendMessage(TransceiveDataSPI_I2C_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
            }
        }

        private void TransceiveDataCT_listBox_MouseClick(object sender, MouseEventArgs e)
        {
            string getCommend;
            if (TransceiveDataCT_listBox.SelectedItem != null)
            {               
                getCommend = TransceiveDataCT_listBox.SelectedItem.ToString().Split(new Char[] { ':' })[0];
                getCommend = DeleteSpaceString(getCommend);
                if (checkBox_APDUmodeCT.Checked)
                {
                    if (getCommend.Length >= 10)
                    {
                        textBox_APDU_CLA_CT.Text = getCommend.Substring(0, 2);
                        textBox_APDU_INS_CT.Text = getCommend.Substring(2, 2);
                        textBox_APDU_P1_CT.Text = getCommend.Substring(4, 2);
                        textBox_APDU_P2_CT.Text = getCommend.Substring(6, 2);
                        textBox_APDU_P3_CT.Text = getCommend.Substring(8, 2);
                        SendData_textBox.Text = getCommend.Substring(10, getCommend.Length - 10);
                    }
                    else
                    {
                        DisplayMessageLine("APDU指令的长度要大于5byte");
                    }
                }
                else
                {
                    SendData_textBox.Text = getCommend;
                }
                TransceiveDataCT_listBox.Focus();
            }
        }

        private void AddCMDtoListBox_CT_btn_Click(object sender, EventArgs e)
        {
            string getCommend, P3;
            string data = DeleteSpaceString(SendData_textBox.Text);
            try
            {
                if (open_cmdlog_flag == 0)
                    CMD_LOG_Directory = ".\\CMD_CT_LOG.txt";
                StreamWriter sw = new StreamWriter(CMD_LOG_Directory, true);    //打开命令记录文档
//                StreamWriter sw = new StreamWriter(".\\CMD_CT_LOG.txt", true);    //打开命令记录文档
                if (checkBox_APDUmodeCT.Checked == false)
                {
                    if (SendData_textBox.Text != "")
                    {
                        TransceiveDataCT_listBox.Items.Add(SendData_textBox.Text);
                        SendMessage(TransceiveDataCT_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
                        sw.WriteLine(SendData_textBox.Text);            //新增命令
                    }
                }
                else
                {
                    if (data != "")
                    {
                        P3 = (data.Length / 2).ToString("X2");
                        textBox_APDU_P3_CT.Text = P3;
                    }
                    getCommend = textBox_APDU_CLA_CT.Text + textBox_APDU_INS_CT.Text + textBox_APDU_P1_CT.Text + textBox_APDU_P2_CT.Text + textBox_APDU_P3_CT.Text + SendData_textBox.Text;
                    if (getCommend.Length >= 10)
                    {
                        TransceiveDataCT_listBox.Items.Add(getCommend);
                        SendMessage(TransceiveDataCT_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
                        sw.WriteLine(getCommend);                //新增命令
                    }
                    else
                    {
                        DisplayMessageLine("APDU格式不对");
                    }
                     
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void SPI_LenToReceive_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void I2C_LenToReceive_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void textBox_APDU_CLA_CT_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_CLA_CT.Text.Length < 2)
            {
                textBox_APDU_CLA_CT.Text = "0" + textBox_APDU_CLA_CT.Text;
            }
        }

        private void textBox_APDU_INS_CT_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_INS_CT.Text.Length < 2)
            {
                textBox_APDU_INS_CT.Text = "0" + textBox_APDU_INS_CT.Text;
            }
        }

        private void textBox_APDU_P1_CT_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_P1_CT.Text.Length < 2)
            {
                textBox_APDU_P1_CT.Text = "0" + textBox_APDU_P1_CT.Text;
            }
        }

        private void textBox_APDU_P2_CT_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_P2_CT.Text.Length < 2)
            {
                textBox_APDU_P2_CT.Text = "0" + textBox_APDU_P2_CT.Text;
            }
        }

        private void textBox_APDU_P3_CT_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_P3_CT.Text.Length < 2)
            {
                textBox_APDU_P3_CT.Text = "0" + textBox_APDU_P3_CT.Text;
            }
        }

        private void textBox_APDU_CLA_CL_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_CLA_CL.Text.Length < 2)
            {
                textBox_APDU_CLA_CL.Text = "0" + textBox_APDU_CLA_CL.Text;
            }
        }

        private void textBox_APDU_INS_CL_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_INS_CL.Text.Length < 2)
            {
                textBox_APDU_INS_CL.Text = "0" + textBox_APDU_INS_CL.Text;
            }
        }

        private void textBox_APDU_P1_CL_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_P1_CL.Text.Length < 2)
            {
                textBox_APDU_P1_CL.Text = "0" + textBox_APDU_P1_CL.Text;
            }
        }

        private void textBox_APDU_P2_CL_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_P2_CL.Text.Length < 2)
            {
                textBox_APDU_P2_CL.Text = "0" + textBox_APDU_P2_CL.Text;
            }
        }

        private void textBox_APDU_P3_CL_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_P3_CL.Text.Length < 2)
            {
                textBox_APDU_P3_CL.Text = "0" + textBox_APDU_P3_CL.Text;
            }
        }

        private void textBox_APDU_Le_CL_Leave(object sender, EventArgs e)
        {
            while (textBox_APDU_Le_CL.Text.Length < 2)
            {
                textBox_APDU_Le_CL.Text = "0" + textBox_APDU_Le_CL.Text;
            }
        }
        private void textBox_last_SAK_Leave(object sender, EventArgs e)
        {
            while (textBox_last_SAK.Text.Length < 2)
            {
                textBox_last_SAK.Text = "0" + textBox_last_SAK.Text;
            }
        }
        private void BlockAddr_textBox_Leave(object sender, EventArgs e)
        {
            while (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
        }

        private void parity_even_rdBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (parity_even_rdBtn.Checked == false)
            {
                return;
            }

            byte regData;
            int i;
            string strReceived;
            i = FM12XX_Card.Read_TDA8007_reg(TDA8007Reg.UCR1, out strReceived, out regData);
            if (i == 0)
            {
                regData &= 0xBF;
                i = FM12XX_Card.Set_TDA8007_reg(TDA8007Reg.UCR1, regData, out strReceived);
            }

            if (i != 0)
            {
                display = "Set TDA8007Reg Faild";
                DisplayMessageLine(display);
            }
            else
            {
                display = "Set Parity to even";
                DisplayMessageLine(display);
            }
        }

        private void parity_odd_rdBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (parity_odd_rdBtn.Checked == false)
            {
                return;
            }

            byte regData;
            int i;
            string strReceived;
            i = FM12XX_Card.Read_TDA8007_reg(TDA8007Reg.UCR1, out strReceived, out regData);
            if (i == 0)
            {
                regData |= 0x40;
                i = FM12XX_Card.Set_TDA8007_reg(TDA8007Reg.UCR1, regData, out strReceived);
            }

            if (i != 0)
            {
                display = "Set TDA8007Reg Faild";
                DisplayMessageLine(display);
            }
            else
            {
                display = "Set Parity to odd";
                DisplayMessageLine(display);
            }
        }


        private void blkaddrDblClick(object sender, EventArgs e)
        {
            byte[] blkno;
            uint blk;
            string blknoo;

            try
            {
                //blkno = Convert.ToUInt32(BlockAddr_textBox.Text);                 
                blkno = strToHexByte(BlockAddr_textBox.Text);
                blk = (uint)(blkno[0]);
                blk = blk + 0x01;
                blknoo = blk.ToString("X2");
                BlockAddr_textBox.Text = blknoo;
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }

        }
        private void TypeASel_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string strReceived;
                if (TypeASel_radioButton.Checked == false)
                {
                    groupBox_CLA_CONTROL.Visible = false;
                    groupBox_MIFARE.Visible = false;
                    groupBox_CLB_CONTROL.Visible = true;
                    textBox_CRC.Text = "FFFF";
                    return;
                }
                else
                {
                    groupBox_CLB_CONTROL.Visible = false;
                    groupBox_CLA_CONTROL.Visible = true;
                    groupBox_MIFARE.Visible = true;
                    textBox_CRC.Text = "6363";
                    FM12XX_Card.TypeAinit(out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "Switch to 14443 TypeA:\tFailed!\nTypeA Set FM1715Reg:\tFailed!";
                        DisplayMessageLine(display);
                    }
                    else
                    {
                        display = "Switch to 14443 TypeA:\tSucceeded!";
                        DisplayMessageLine(display);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void TypeBSel_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string StrReceived;
                string msg = "";
                if (TypeBSel_radioButton.Checked == false)
                {
                    groupBox_CLB_CONTROL.Visible = false;
                    groupBox_CLA_CONTROL.Visible = true;
                    groupBox_MIFARE.Visible = true;
                    textBox_CRC.Text = "6363";
                    return;
                }
                else
                {
                    groupBox_CLA_CONTROL.Visible = false;
                    groupBox_MIFARE.Visible = false;
                    groupBox_CLB_CONTROL.Visible = true;
                    textBox_CRC.Text = "FFFF";
                    //TYPE B config frame
                    if (FM12XX_Card.TypeBFrameFlag)
                    {
                        FM12XX_Card.enSOF_n = (checkBox_CLB_SOF.Checked == true) ? 0 : 1;
                        FM12XX_Card.enEOF_n = (checkBox_CLB_EOF.Checked == true) ? 0 : 1;
                        FM12XX_Card.EOF_type = comboBox_CLB_EOF.SelectedIndex;
                        FM12XX_Card.SOF_type = comboBox_CLB_SOF.SelectedIndex;
                        FM12XX_Card.EGT = (int)strToHexByte(textBox_EGT.Text)[0];

                        FM12XX_Card.TypeBFraming(FM12XX_Card.enSOF_n, FM12XX_Card.enEOF_n, FM12XX_Card.EOF_type, FM12XX_Card.EGT, FM12XX_Card.SOF_type, out StrReceived);
                        if (StrReceived == "Succeeded")
                        {
                            if (checkBox_CLB_SOF.Checked == true)
                                msg += ("Has SOF: " + comboBox_CLB_SOF.SelectedItem + ";\t");
                            if (checkBox_CLB_EOF.Checked == true)
                                msg += ("Has EOF: " + comboBox_CLB_SOF.SelectedItem + ";\t");
                            msg += ("EGT: " + FM12XX_Card.EGT + " etus");
                            msg = "Set PCD Type B frame format:\t" + msg;
                        }
                        else if (StrReceived == "Error")
                        {
                            msg = "Set PCD Type B frame format:\tFailed";
                        }
                        FM12XX_Card.TypeBFrameFlag = false;
                    }
                    FM12XX_Card.TypeBinit(out StrReceived);
                    DisplayMessageLine("Switch to 14443 TypeB:\t" + StrReceived + "!");
                    if(msg != "")
                        DisplayMessageLine(msg);
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }


        private void WupA_Click(object sender, EventArgs e)
        {
            try
            {
                FM12XX_Card.WUPA(out display);

                display = "ATQA：  \t\t\t" + display;

                DisplayMessageLine(display);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Halt_Click(object sender, EventArgs e)
        {
            try
            {
                FM12XX_Card.HALT(out display);

                display = "Halt：  \t\t\t" + display;

                DisplayMessageLine(display);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        
        public int Flash_isp(uint nProgStartAddr, uint nProgEndAddr, uint nProgOffsetAddr)//, out string strReceived)
        {
            uint ProgBlockNumi, nProgBlockSum, nAddr, nProgLastOneLen, nProgLastOneAddr;
            int i, rtn;
            int baud_r;
            string WriteAddr, WriteData, strOut, ReadData;
            byte[] readbuf = new byte[0x80];
            baud_r = 115200;

            rtn = FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 0, out strOut);

            DisplayMessageLine(strOut);
            if (rtn == 0)
            {
                FM309_flash_isp.uart_bandrate_init();
                FM309_flash_isp.flash_isp_start_end();
                DisplayMessageLine("flash在线下载流程开始");
            }
            else
            {
                return 1;
            }
            
            nProgBlockSum = (nProgEndAddr - nProgStartAddr) / 0x80;

            nProgLastOneLen = (nProgEndAddr - nProgStartAddr) % 0x80 + 1;
            nProgLastOneAddr = nProgEndAddr - (nProgEndAddr - nProgStartAddr) % 0x80;
            if (flash_check.Checked)
            {
                ProgEE_progressBar.Maximum = (int)nProgBlockSum * 2;
            }
            else
            {
                ProgEE_progressBar.Maximum = (int)nProgBlockSum;
            }
            ProgEE_progressBar.Value = 0;

            if (CB_NoErase.Checked == true)
            {
                DisplayMessageLine("flash未擦除，直接进行写入......");
            }
            else
            {
                DisplayMessageLine("开始擦除flash......");
                rtn = FM309_flash_isp.falsh_chip_erase(out strOut);
                if (rtn != 0)
                {
                    DisplayMessageLine("擦除flash失败");
                    FM309_flash_isp.flash_isp_start_end();
                    DisplayMessageLine("flash在线下载流程结束");
                    FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                    DisplayMessageLine(strOut);
                    return 1;
                }
                DisplayMessageLine("擦除flash完成");
            }

            DisplayMessageLine("开始写入flash......");
            for (ProgBlockNumi = 0; ProgBlockNumi < nProgBlockSum; ProgBlockNumi += 1)
            {
                nAddr = nProgStartAddr + ProgBlockNumi * 0x80;
                WriteAddr = (nAddr + nProgOffsetAddr).ToString("X6");
                WriteData = "";

                for (i = 0; i < 0x80; i++)
                {
                    WriteData += Progfilebuf[nAddr + i].ToString("X2");
                }
                rtn = FM309_flash_isp.flash_write(WriteAddr, "80", WriteData, out strOut);
                if (rtn != 0)
                {
                    DisplayMessageLine(strOut);
                    FM309_flash_isp.flash_isp_start_end();
                    DisplayMessageLine("flash在线下载流程结束");
                    FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                    DisplayMessageLine(strOut);
                    return 1;
                }
                ProgEE_progressBar.Value = (int)ProgBlockNumi + 1;
            }
            if (nProgLastOneLen != 0)
            {
                WriteData = "";
                for (i = 0; i < nProgLastOneLen; i++)
                {
                    WriteData += Progfilebuf[nProgLastOneAddr + i].ToString("X2");
                }
                WriteAddr = (nProgOffsetAddr + nProgLastOneAddr).ToString("X6");
                rtn = FM309_flash_isp.flash_write(WriteAddr, ((byte)nProgLastOneLen).ToString("X2"), WriteData, out strOut);
                if (rtn != 0)
                {
                    DisplayMessageLine(strOut);
                    FM309_flash_isp.flash_isp_start_end();
                    DisplayMessageLine("flash在线下载流程结束");
                    FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                    DisplayMessageLine(strOut);
                    return 1;
                }
            }
            DisplayMessageLine("写入flash完成");
            ///////////////////////////////////////校验////
            if (flash_check.Checked)
            {
                DisplayMessageLine("开始读出校验......");
                for (ProgBlockNumi = 0; ProgBlockNumi < nProgBlockSum; ProgBlockNumi += 1)
                {
                    nAddr = nProgStartAddr + ProgBlockNumi * 0x80;
                    WriteAddr = (nProgOffsetAddr+nAddr).ToString("X6");
                    rtn = FM309_flash_isp.flash_read(WriteAddr, "80", out strOut);
                    if (rtn != 0)
                    {
                        DisplayMessageLine(strOut);
                        FM309_flash_isp.flash_isp_start_end();
                        DisplayMessageLine("flash在线下载流程结束");
                        FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                        DisplayMessageLine(strOut);
                        return 1;
                    }
                    for (i = 0; i < 0x80; i++)
                    {
                        WriteData = Progfilebuf[nAddr + i].ToString("X2");
                        ReadData = strOut.Substring(i * 2, 2);
                        if (WriteData != ReadData)
                        {
                            DisplayMessageLine("校验失败 地址：" + (nAddr + nProgOffsetAddr + i).ToString("X2") + "  WriteData:" + WriteData + "  ReadData:" + ReadData);
                            FM309_flash_isp.flash_isp_start_end();
                            DisplayMessageLine("flash在线下载流程结束");
                            FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                            DisplayMessageLine(strOut);
                            return 1;
                        }
                    }
                    ProgEE_progressBar.Value = (int)ProgBlockNumi + 1 + (int)nProgBlockSum;
                }

                if (nProgLastOneLen != 0)
                {
                    WriteAddr = (nProgOffsetAddr+nProgLastOneAddr).ToString("X6");

                    rtn = FM309_flash_isp.flash_read(WriteAddr, ((byte)nProgLastOneLen).ToString("X2"), out strOut);
                    if (rtn != 0)
                    {
                        DisplayMessageLine(strOut);
                        FM309_flash_isp.flash_isp_start_end();
                        DisplayMessageLine("flash在线下载流程结束");
                        FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                        DisplayMessageLine(strOut);
                        return 1;
                    }
                    for (i = 0; i < nProgLastOneLen; i++)
                    {
                        WriteData = Progfilebuf[nProgLastOneAddr + i].ToString("X2");
                        ReadData = strOut.Substring(i * 2, 2);
                        if (WriteData != ReadData)
                        {
                            DisplayMessageLine("校验失败：" + (nProgLastOneAddr + nProgOffsetAddr + i).ToString("X2") + "  WriteData:" + WriteData + "  ReadData:" + ReadData);
                            FM309_flash_isp.flash_isp_start_end();
                            DisplayMessageLine("flash在线下载流程结束");
                            FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
                            DisplayMessageLine(strOut);
                            return 1;
                        }

                    }

                }

                DisplayMessageLine("读出校验通过");
            }
            ///////////////////////////////////////校验//////////////////////
            FM309_flash_isp.flash_isp_start_end();
            DisplayMessageLine("flash在线下载流程结束");
            rtn = FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baud_r, 1, out strOut);
            DisplayMessageLine(strOut);
            return 0;
        }

        private void ProgEE_Button_Click(object sender, EventArgs e)
        {
            string StartEEAddr, EndEEAddr, OffsetEEAddr, strReceived;
            uint EEtype, nTemp1, nTemp2, nTemp3, tmp, cnt2k = 0, cnt2b = 0;
            byte[] endaddr, startaddr, offsetaddr, temp = { 0, 0 };
            int nRet = 1;
            uint nProgStartBlock = 0;
            uint nProgEndBlock = 0;

            ProgOrReadEE_flag = 0;//EE编程

            if (PROG_EEtype_radiobutton.Checked == true)
                EEtype = 0; // 程序EE
            else
                EEtype = 1;

            /*if (chkWREncrypt.Checked == true)
                btEncryptOpt = 0;
            else
                btEncryptOpt = 1;*/

            try
            {
                StartEEAddr = ProgEEStartAddr.Text;
                EndEEAddr = ProgEEEndAddr.Text;
                OffsetEEAddr = ProgEEOffsetAddr.Text;
                for (int i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
                {
                    StartEEAddr = "0" + StartEEAddr;
                }
                for (int i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
                {
                    EndEEAddr = "0" + EndEEAddr;
                }
                for (int i = 0; i < 6 - ProgEEOffsetAddr.Text.Length; i++)
                {
                    OffsetEEAddr = "0" + OffsetEEAddr;
                }
                ProgEEStartAddr.Text = StartEEAddr;
                ProgEEEndAddr.Text = EndEEAddr;
                ProgEEOffsetAddr.Text = OffsetEEAddr;
                startaddr = new byte[3];
                endaddr = new byte[3];
                offsetaddr = new byte[3];
                startaddr = strToHexByte(StartEEAddr);
                endaddr = strToHexByte(EndEEAddr);
                offsetaddr = strToHexByte(OffsetEEAddr);
                nTemp1 = (uint)(startaddr[2] + startaddr[1] * 256 + startaddr[0] * 65536);  //start addr value
                nTemp2 = (uint)(endaddr[2] + endaddr[1] * 256 + endaddr[0] * 65536);        //end addr value
                nTemp3 = (uint)(offsetaddr[2] + offsetaddr[1] * 256 + offsetaddr[0] * 65536);   //offset addr value
                if (interface_sel_grpbox.Enabled)
                {
                    if (CL_Interface.Checked)
                    {
                        if (A1Program.Checked == true)
                        {
                            //cla,A1命令,最小单位128BYTES
                            nProgStartBlock = nTemp1 / 128;	//开始页
                            nProgEndBlock = nTemp2 / 128;  	//结束页

                            if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true)
                            {
                                nProgStartBlock = nTemp1 / 16;	//开始扇区
                                nProgEndBlock = nTemp2 / 16;	//结束扇区                            
                            }
                        }
                        else
                        {
                            //cla,最小单位16BYTES
                            nProgStartBlock = nTemp1 / 16;	//开始扇区
                            nProgEndBlock = nTemp2 / 16;	//结束扇区
                        }
                    }
                    else    //CT interface prog
                    {
                        if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true)
                        {
                            //ct,最小单位64 BYTES
                            nProgStartBlock = nTemp1 / 64;	//开始页
                            nProgEndBlock = nTemp2 / 64;	//结束页
                        }
                        else if (FM347_radioButton.Checked == true)
                        {
                            FM12XX_Card.Cold_Reset("02", out strReceived);
                            FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                            nRet = FM12XX_Card.TransceiveCT("0004000C04", "02", out strReceived);
                            if (nRet == 1)
                            {
                                DisplayMessageLine("读配置字失败！");
                                return;
                            }
                            temp[0] = (byte)(strToHexByte(strReceived.Substring(4, 2))[0] << 1);
                            temp[0] &= 0x3F;
                            temp[1] = strToHexByte(strReceived.Substring(6, 2))[0];
                            if (temp[1] >= 0x80)
                                temp[0]++;
                            if (PROG_EEtype_radiobutton.Checked == true)
                            {
                                nProgStartBlock = nTemp1 / 0x800;	//开始页
                                if (nTemp2 + nTemp3 > 0x058000)  //ct,最小单位256 bytes
                                {
                                    cnt2k = (0x058000 - nTemp1 - nTemp3) / 0x800 + 1;
                                    cnt2b = (nTemp2 + nTemp3 - 0x058000) / 0x100 + 1;
                                    DisplayMessageLine("程序段在后160k空间，有： " + cnt2b.ToString("x3") + " 个sector（256 Bytes）");
                                }
                                else                 //ct,最小单位2k bytes
                                {
                                    cnt2k = (nTemp2 - nTemp1) / 0x800 + 1;
                                    cnt2b = 0;
                                }
                            }
                            else if (DATA_EEtype_radiobutton.Checked == true)
                            {
                                if (temp[0] >= 0x2C) //ct,最小单位256 bytes
                                {
                                    nProgStartBlock = nTemp1 / 0x100;	//开始页          
                                    cnt2k = 0;
                                    cnt2b = (nTemp2 - nTemp1) / 0x100 + 1;
                                }
                                else if ((0x2C - temp[0]) * 32 >= (nTemp2 - nTemp1 + nTemp3) / 0x100 + 1) //ct,最小单位2k bytes
                                {
                                    nProgStartBlock = nTemp1 / 0x800;	//开始页          
                                    cnt2k = (nTemp2 - nTemp1) / 0x800 + 1;
                                    cnt2b = 0;
                                    DisplayMessageLine("数据段全在前352k空间，有： " + cnt2k.ToString("x3") + " 个sector（2k Bytes）");
                                }
                                else  //ct,最小单位2k bytes
                                {
                                    nProgStartBlock = nTemp1 / 0x800;	//开始页
                                    cnt2k = (uint)(0x2C - temp[0]) * 4;
                                    if (cnt2k >= nTemp3 / 0x800)
                                    {
                                        cnt2k -= nTemp3 / 0x800;
                                        cnt2b = (nTemp2 - nTemp1) / 0x100 + 1 - cnt2k * 8;
                                    }
                                    else
                                    {
                                        cnt2k = 0;
                                        cnt2b = (nTemp2 - nTemp1) / 0x100 + 1;
                                    }
                                    DisplayMessageLine("数据段在前352k空间，有： " + cnt2k.ToString("x3") + " 个sector（2k Bytes）");
                                }
                            }
                            else
                            {
                                nProgStartBlock = nTemp1 / 0x100;	//开始页          
                                cnt2k = 0;
                                cnt2b = (nTemp2 - nTemp1) / 0x100 + 1;
                            }
                        }
                        else
                        {
                            //ct,最小单位128YTES
                            nProgStartBlock = nTemp1 / 128;	//开始页
                            nProgEndBlock = nTemp2 / 128;  	//结束页
                        }
                    }
                    //Reset17_Click(null, EventArgs.Empty);
                    //Init_TDA8007_Click(null, EventArgs.Empty);
                    if (CL_Interface.Checked)
                    {	//cla编程接口
                        DisplayMessageLine("已选择CLA接口...");
                        if (FM349_radioButton.Checked == true)
                        {
                            DisplayMessageLine("FM349编程启动...");
                            nRet = ProgOrReadEEInCLA349(nProgStartBlock, nProgEndBlock, nTemp3);
                            FM12XX_Card.SetField(0, out display);
                            display = "SetField     \t\t\t" + display;
                            DisplayMessageLine(display);
                        }
                        else if (FM350_radioButton.Checked == true)
                        {
                            DisplayMessageLine("FM350编程启动...");
                            nRet = ProgOrReadEEInCLA350(nProgStartBlock, nProgEndBlock, nTemp3);
                            FM12XX_Card.SetField(0, out display);
                            display = "SetField     \t\t\t" + display;
                            DisplayMessageLine(display);
                        }
                        else if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true || FM302_radioButton.Checked == true)
                        {
                            nRet = ProgOrReadEEInCLA294(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                        }
                        else
                        {
                            nRet = ProgOrReadEEInCLA(EEtype, nProgStartBlock * 8, nProgEndBlock * 8, out  strReceived);
                            FM12XX_Card.SetField(0, out display);
                            display = "SetField     \t\t\t" + display;
                            DisplayMessageLine(display);
                        }
                    }
                    else
                    {	//ct编程接口
                        DisplayMessageLine("已选择CT接口...");
                        if (FM349_radioButton.Checked == true)
                        {
                            DisplayMessageLine("FM349编程启动...");

                            nRet = ProgOrReadEEInCT349(nProgStartBlock, nProgEndBlock, nTemp3);
                            FM12XX_Card.Init_TDA8007(out strReceived);
                            display = "Init TDA8007\t\t\t\t" + strReceived;
                            DisplayMessageLine(display);
                        }
                        else if (FM350_radioButton.Checked == true)
                        {
                            DisplayMessageLine("FM350编程启动...");

                            nRet = ProgOrReadEEInCT350(nProgStartBlock, nProgEndBlock, nTemp3);
                            FM12XX_Card.Init_TDA8007(out strReceived);
                            display = "Init TDA8007\t\t\t\t" + strReceived;
                            DisplayMessageLine(display);
                        }
                        else if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true)
                            nRet = ProgOrReadEEInCT294(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                        else if (FM347_radioButton.Checked)
                            nRet = ProgOrReadEEInCT347(nProgStartBlock, cnt2k, cnt2b, nTemp3);
                        else
                        {
                            nRet = ProgOrReadEEInCT(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                            //    FM12XX_Card.Init_TDA8007(out strReceived);       //下载程序后不下电    2015.12.7 zhujunhao
                            display = "Init TDA8007\t\t\t\t" + strReceived;
                            DisplayMessageLine(display);
                        }
                    }
                    if (nRet == 0)
                    {
                        display = "编程结束   \t\t\tSucceeded ";
                        DisplayMessageLine(display);
                    }
                    else
                    {
                        display = "编程失败!   ";
                        DisplayMessageLine(display);
                        FM12XX_Card.Init_TDA8007(out strReceived);
                    }
                }
                else
                {
                    DisplayMessageLine("flash编程开始");
                    //FM12XX_Card.SetField(0, out display);
                    //FM12XX_Card.SetField(1, out display);
                    nRet = Flash_isp(nTemp1, nTemp2, nTemp3);//, out string strReceived)
                    //FM12XX_Card.SetField(0, out display);
                    if (nRet == 0)
                    {
                        display = "flash编程结束";
                        DisplayMessageLine(display);
                    }
                    else
                    {
                        display = "flash编程失败!";
                        DisplayMessageLine(display);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }

        }
        public virtual int ProgOrReadEEInCT(uint EEtype, uint nProgStartBlock, uint nProgEndBlock, out string strReceived)
        {
            int result, FMXX_SEL;
            uint i, j, ProgNumi, nProgSum, buf_nAddr, temp, Ext_i, ProgStartBlock_temp, nAddr, nAddr_base, buffer_start_addr;
            int Ext_addr_change, ExtOpt, prog_mem_type, EEmax, prog_extand_mode, LIBsize, cmp1, cmp2, cwd14;
            byte[] buff;
            string sendbuf, STR, auth_key;
            int write_length;
            strReceived = "";
            nAddr = 0;
            buf_nAddr = 0;
            buff = new byte[0x103];
            string DestAddr;
            uint nTemp3 = 0;
            byte[] destaddr;
            uint nDestBlock = 0;
            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;
            nAddr_base = 0;

            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            //if (FM326_radioButton.Checked == false)
            //{
            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.Cold_Reset("02", out strReceived); //3v
            //}
            if (ProgOrReadEE_flag == 0) //编程
            {
                if(FM326_radioButton.Checked == true)
                    FM12XX_Card.Cold_Reset("03", out strReceived); //1.8v
                else
                    FM12XX_Card.Cold_Reset("02", out strReceived); //3v
                if (strReceived == "Error")
                {
                    display = "Cold Reset失败 ";
                    DisplayMessageLine(display);
                    //return 1; //CNN 20130325 如果卡没有ATR回发  继续执行
                }
                Delay(1);
            }
            if (checkBox_CTHighBaud.Checked)
            {
                result = FM12XX_Card.TransceiveCT("0001180000", "02", out strReceived);//切换到31分频
                //初始化          
                // result = FM12XX_Card.TransceiveCT("0001950000", "02", out strReceived);//切换到32分频
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到32分频
                //result = FM12XX_Card.Set_TDA8007_reg("03", "21", out strReceived);
                //result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);

                //切换到31分频
                result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);

                DisplayMessageLine("切换到115200波特率");
            }
            else
            {
                result = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);//切换到32分频
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
            }

            //INITIAL EEPROM命令               
            if (Auth_checkBox.Checked)
            {
                auth_key = key_richTextBox.Text.Replace(" ", "");
                if ((auth_key == "") || (auth_key.Length != 32))
                {
                    display = "认证的密钥长度不对！！！   \t\t";
                    DisplayMessageLine(display);
                    return 1;
                }
                FM12XX_Card.TransceiveCT("0055000010", "02", out strReceived);//CNN 添加认证，使认证时也能配置控制字
                int k = FM12XX_Card.TransceiveCT(auth_key, "02", out strReceived);//LOAD密钥
                if (k != 0)
                {
                    display = "认证:   \t\t" + "错误！！";
                    DisplayMessageLine(display);
                    return 1;
                }
                display = "认证:   \t\t" + "通过";
                DisplayMessageLine(display);
            }
            if (FM336_radioButton.Checked)
            {
                FMXX_SEL = 1;
            }
            else
                FMXX_SEL = 0;

            FM12XX_Card.ProgExt_CwdCheck(1, FMXX_SEL, out Ext_addr_change, out prog_extand_mode, out ExtOpt, out prog_mem_type, out EEmax, out LIBsize, out cwd14, out STR);//返回的是数据EE max            
            if (STR == "error")
            {
                display = "初始化指令出错！！！请查看是否需要认证？？或有其他错误……   \t\t";
                DisplayMessageLine(display);
                return 1;
            }

            if (RAMtype_radiobutton.Checked)//数据EE-0x00,程序EE-0x02,RAM-0x04
            {
                sendbuf = "0002040000";
                if (((EEmax - Ext_addr_change) / 1024) > 7)
                {
                    display = "RAM补丁区超出7K！！将按照7K进行。";
                    DisplayMessageLine(display);
                    Ext_addr_change = 1 * 1024;
                }
                EEmax = 8 * 1024;
                display = "RAM补丁区为" + ((EEmax - Ext_addr_change) / 1024).ToString() + "K" + "起始地址：" + (8 * 1024 - (EEmax - Ext_addr_change)).ToString("X6");
                DisplayMessageLine(display);
                if (ProgOrReadEE_flag == 0 && nProgEndBlock > (EEmax - Ext_addr_change) / 128)
                {
                    if (MessageBox.Show("程序太大，超出RAM补丁区！！是否继续编程？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return 1;
                    }
                }
            }
            else if (DATA_EEtype_radiobutton.Checked)
            {
                sendbuf = "0002000300";                //修改数据EEPROM读取默认为快读模式。 2015.05.21 zhujunhao
            }
            else
            {
                sendbuf = "0002020000";
            }
            cmp1 = (EEmax - Ext_addr_change) / 1024;
            Patch_Size.Text = ((EEmax - Ext_addr_change) / 1024).ToString();
            lib_Size.Text = ((LIBsize >> 3) * 2).ToString();
            ProgStartBlock_temp = 0;
            if (PROG_EEtype_radiobutton.Checked)
            {
                if (ExtOpt == 0)
                {
                    prog_extand_memEE.Checked = true;
                }
                else
                {
                    prog_extand_memRAM.Checked = true;
                }

                if (DATA_EEtype_radiobutton.Checked == true || RAMtype_radiobutton.Checked == true)//非程序EE 不支持扩展 CNN20130221
                {
                    prog_extand_mode = 1;
                }
                if (prog_mem_type == 0)
                    MEM_ROM.Checked = true;
                else
                    MEM_EE.Checked = true;

                if (prog_extand_mode == 1)//段后扩展
                {
                    ProgOrReadEE_EXT_flag = 1;
                    prog_extand_mode1.Checked = true;
                    if (FM336_radioButton.Checked)
                    {
                        // EEmax = 0x60000;
                        if ((cwd14 & 0x10) == 0x00)//不使能ROM奇偶校验 384k
                        {
                            EEmax = 0x60000;
                        }
                        else if ((cwd14 & 0x08) == 0x00)//1位校验 336k
                        {
                            EEmax = 0x54000;
                        }
                        else if ((cwd14 & 0x08) == 0x08)//4位校验 256k
                        {
                            EEmax = 0x40000;
                        }
                        else
                            EEmax = 0x60000;
                    }
                    else
                    {
                        if (prog_mem_type == 0 && PROG_EEtype_radiobutton.Checked)//程序EE
                        {
                            EEmax = 0x40000;
                        }
                        else
                        {
                            EEmax = 0x3C000;
                        }
                    }
                }

                else//整体扩展
                {
                    prog_extand_mode0.Checked = true;
                    if (ProgOrReadEE_flag == 0 || chkReadVerify.Checked)
                    {
                        ProgStartBlock_temp = (uint)(Ext_addr_change / 128);
                        if (Patch_Size.Text == "0")
                        {
                            ProgStartBlock_temp = 0;
                            sendbuf = "0002020000";
                            display = "Patch区为0K。整体扩展无意义！！现在程序起始地址在程序区：逻辑地址0xFF0000";
                        }
                        else if (ExtOpt == 0 && PROG_EEtype_radiobutton.Checked)//EE
                        {
                            sendbuf = "0002000000";
                            display = "整体扩展：起始地址扩展到逻辑地址" + (Ext_addr_change + 0x010000).ToString("X6");
                        }
                        else//RAM
                        {
                            sendbuf = "0002040000";
                            display = "整体扩展：起始地址扩展到逻辑地址" + (Ext_addr_change).ToString("X6");
                        }
                        DisplayMessageLine(display);
                    }
                }
            }


            result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (result == 1)
            {
                display = "INITIAL EE 失败";
                DisplayMessageLine(display);
                return 1;
            }

            //ProgStartBlock_temp = nProgStartBlock;          

            i = 0;
            Ext_i = 0;

            buffer_start_addr = nProgStartBlock;

            if (ProgOrReadEE_flag == 1)
            {
                nProgSum = (nProgSum + 1) / 2;
                nProgStartBlock = nProgStartBlock / 2;
                ProgStartBlock_temp = ProgStartBlock_temp / 2;
                ProgEE_progressBar.Maximum = (int)nProgSum;
            }
            if (ProgOrReadEE_flag == 0 && RAMtype_radiobutton.Checked == true)
            {
                nProgSum = (nProgSum + 1) / 2;
                write_length = 256;			//编程字节数 RAM最多编程256个字节    

                nProgStartBlock = (uint)((8 * 1024 - (EEmax - Ext_addr_change)) / 128);

                nProgStartBlock = nProgStartBlock / 2;
                ProgStartBlock_temp = ProgStartBlock_temp / 2;
                buffer_start_addr = buffer_start_addr / 2;
                ProgEE_progressBar.Maximum = (int)nProgSum;
            }
            else
            {
                write_length = 0x80;			//编程字节数
            }
            result = 1;

            /* if ((RAMtype_radiobutton.Checked == true) && (PatchSize_comboBox.Enabled == true) && (ProgOrReadEE_flag == 0))
             {
                 if (ProgOrReadEE_flag == 0)
                 {
                     result = FM12XX_Card.TransceiveCT("0008020000", "02", out strReceived);
                 }               
                               
                 if (result == 1)
                 {
                     display = "7816接口改时钟 失败";
                     DisplayMessageLine(display);
                     return 1;
                 }
             }
             else
                 result = FM12XX_Card.TransceiveCT("0008000000", "02", out strReceived);*/


            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                if (ProgOrReadEE_flag == 0 && PROG_EEtype_radiobutton.Checked && Ext_i == 0)
                {
                    if ((ProgOrReadEE_FF_flag == 0) && (ProgNumi * 128 > 0x000000) && (ProgNumi * 128 < 0x010000) && (ProgNumi < nProgSum))
                    {
                        i = 0x200;
                        ProgNumi = i;
                        // ProgOrReadEE_FF_flag = 1;
                    }
                    if ((ProgOrReadEE_80_flag == 0) && (ProgNumi * 128 > 0x00FFFF) && (ProgNumi * 128 < 0x020000) && (ProgNumi < nProgSum))
                    {
                        i = 0x400;
                        ProgNumi = i;
                        // ProgOrReadEE_80_flag = 1;
                    }
                    if ((ProgOrReadEE_81_flag == 0) && (ProgNumi * 128 > 0x01FFFF) && (ProgNumi * 128 < 0x030000) && (ProgNumi < nProgSum))
                    {
                        i = 0x600;
                        ProgNumi = i;
                        //ProgOrReadEE_81_flag = 1;
                    }
                    if (FM336_radioButton.Checked)
                    {
                        if ((ProgOrReadEE_82_flag == 0) && (ProgNumi * 128 > 0x02FFFF) && (ProgNumi * 128 < 0x040000) && (ProgNumi < nProgSum))
                        {
                            i = 0x800;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_83_flag == 0) && (ProgNumi * 128 > 0x03FFFF) && (ProgNumi * 128 < 0x050000) && (ProgNumi < nProgSum))
                        {
                            i = 0xA00;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_84_flag == 0) && (ProgNumi * 128 > 0x04FFFF) && (ProgNumi * 128 < EEmax - 2) && (ProgNumi < nProgSum))
                        {
                            //i = 0xC00;
                            i = ((uint)(EEmax) / 0x100) * 2;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_85_flag == 0) && (ProgNumi * 128 > 0x05FFFF) && (ProgNumi * 128 < 0x070000) && (ProgNumi < nProgSum))
                        {
                            i = 0xE00;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_86_flag == 0) && (ProgNumi * 128 > 0x06FFFF) && (ProgNumi * 128 < 0x080000) && (ProgNumi < nProgSum))
                        {
                            i = 0x1000;
                            ProgNumi = i;
                        }

                    }
                    else
                    {
                        if ((ProgOrReadEE_82_flag == 0) && (ProgNumi * 128 > 0x02FFFF) && (ProgNumi * 128 < EEmax - 2) && (ProgNumi < nProgSum))
                        {
                            if (prog_mem_type == 0)
                            {
                                i = 0x800;
                            }
                            else
                                i = 0x780;
                            //i = 0x800;                        
                            ProgNumi = i;
                            //ProgOrReadEE_82_flag = 1;
                        }
                        if ((ProgOrReadEE_83_flag == 0) && (ProgNumi * 128 > 0x03FFFF) && (ProgNumi * 128 < 0x050000) && (ProgNumi < nProgSum))
                        {
                            i = 0xA00;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_84_flag == 0) && (ProgNumi * 128 > 0x04FFFF) && (ProgNumi * 128 < 0x060000) && (ProgNumi < nProgSum))
                        {
                            i = 0xC00;
                            ProgNumi = i;
                        }
                    }
                }
                if (ProgOrReadEE_EXT_flag == 1 && (((ProgNumi + nProgStartBlock) * 128 > EEmax - 1) || (ProgStartBlock_temp * 128 > EEmax - 1)) && PROG_EEtype_radiobutton.Checked && (ProgNumi < nProgSum))
                { //CNN 20130325 添加：如果起始地址大于EEmax,则切换到段后扩展补丁区    
                    cmp2 = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) - (EEmax - 1);
                    cmp2 = cmp2 / 1024;
                    if (Ext_i == 0)
                    {
                        Ext_i = 1;
                        //FM12XX_Card.ProgExt_CwdCheck(1, out Ext_addr_change, out ExtOpt, out prog_mem_type);                            

                        display = "程序空间已满，将进行段后扩展!";
                        DisplayMessageLine(display);
                        if (cmp1 < cmp2)
                        {
                            display = "待扩展的数据大小为" + (cmp2).ToString() + "K,补丁区为" + Patch_Size.Text + "K,不满足扩展需要！";
                            if (MessageBox.Show(display + "是否继续编程？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return 1;
                            }
                        }
                        if (ExtOpt == 1)
                            display = "将切换到段后扩展地址(逻辑地址)" + (Ext_addr_change).ToString("X6");
                        else
                            display = "将切换到段后扩展地址(逻辑地址)" + (Ext_addr_change + 0x010000).ToString("X6");
                        DisplayMessageLine(display);

                        if (Ext_addr_change < 0)
                        {
                            display = "EE地址切换到段后扩展地址错误，请检查控制字EE大小和补丁区配置！！";
                            DisplayMessageLine(display);
                            return 1;
                        }

                        //选择段后扩展数据EE
                        if (ExtOpt == 1)
                            result = FM12XX_Card.TransceiveCT("0002040000", "02", out strReceived);
                        else
                            result = FM12XX_Card.TransceiveCT("0002000000", "02", out strReceived);
                        if (result == 1)
                        {
                            display = "选择段后扩展数据EE或者RAM出错";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        //ProgStartBlock_temp = (uint)(ProgStartBlock_temp * 128 - EEmax + Ext_addr_change) / 128;//CNN 20130328
                        ProgStartBlock_temp = (uint)(Ext_addr_change) / 128;//CNN 20131230
                        i = 0;
                    }
                    //Ext_i++;
                }
                // nAddr_temp

                if (ProgOrReadEE_flag == 0)//编程
                {
                                  
                   if (prog_mem_type == 0 && PROG_EEtype_radiobutton.Checked && prog_extand_mode == 1)
                    {
                        if (MessageBox.Show("程序是ROM存储，是否继续编程？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                        }
                        else
                            return 1;      
                       /*  display = "无权限对ROM空间编程，请检查配置字……   \t\t";
                        DisplayMessageLine(display);
                        return 1;*/
                    }
                    //WR EEPROM命令

                    buf_nAddr = (buffer_start_addr + ProgNumi) * (uint)write_length;

                    nAddr = nAddr_base + (ProgStartBlock_temp + nProgStartBlock + i) * (uint)write_length;

                    if (FM336_radioButton.Checked)
                        temp = nAddr / 8;
                    else
                        temp = nAddr / 4;
                    buff[0] = 0x00;
                    buff[1] = 0x00;			//INS = 0x00,WRITE EEPROM命令
                    buff[2] = (byte)(temp / 0x100);	//地址高位
                    buff[3] = (byte)(temp % 0x100);	//地址低位
                    buff[4] = 0x80;			//编程字节数 
                    if (RAMtype_radiobutton.Checked == true)
                    {
                        buff[4] = 0x00;			//编程字节数 RAM最多编程256个字节
                    }

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if ((result != 0x00) || (buff[0] != 0x00))
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < write_length; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                        //btAddr = (nAddr + j) % 0x100;
                        //buff[j] = DataEncryptPro(g_ProgParam.btEncryptOpt, temp, btAddr);
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (result != 0x00)
                    {//编程出错
                        // strReceived = "EE编程出错" + (nProgStartBlock + i).ToString("X6");
                        strReceived = "EE编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    i++;
                    ProgEE_progressBar.Value = (int)ProgNumi;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                    }
                }


                else//读取EE操作
                {
                    //读取操作                      
                    nAddr = nAddr_base + (ProgStartBlock_temp + nProgStartBlock + i) * 0x100;

                    if (FM336_radioButton.Checked)
                        temp = nAddr / 8;
                    else
                        temp = nAddr / 4;
                    buff[0] = 0x00;
                    buff[1] = 0x04;			//INS = 0x04,READ EEPROM命令
                    buff[2] = (byte)(temp / 0x100);	//地址高位
                    buff[3] = (byte)(temp % 0x100);	//地址低位
                    buff[4] = 0x00;			//读字节数Lc=00表示读取256个字节

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    /*//RD Encrypt begin 
                    for(j=0;j<BLOCKSIZE;j++)
                    {
                        buff[j] = DataEncryptPro(g_ProgParam.btEncryptOpt, buff[j], btAddr*0x10+j);
                    }
                    //RD Encrypt begin */

                    for (j = 0; j < 0x100; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        //nAddr = (nProgStartBlock + ProgNumi) * 0x100 + j;                        
                        Readfilebuf[ProgNumi * 0x100 + j] = buff[j + 1];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < 0x100; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * 0x100 + j;
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[ProgNumi * 0x100 + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < 0x100; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        buf_nAddr = (nProgStartBlock + ProgNumi) * 0x100 + j;
                        if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("读取完毕");
                            break;
                        }
                        Progfilebuf[buf_nAddr] = Readfilebuf[ProgNumi * 0x100 + j];
                    }
                    i++;
                    ProgEE_progressBar.Value = (int)ProgNumi;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                    }

                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            result = FM12XX_Card.TransceiveCT("0009530000", "02", out strReceived);//切换到372分频
            if (result == 1)
            {
                display = "修改传输波特率失败";
                DisplayMessageLine(display);
                return 1;
            }
            result = FM12XX_Card.Set_TDA8007_reg("02", "0C", out strReceived);
            if (result == 1)
            {
                display = "修改传输波特率失败";
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                display = "修改传输波特率为9600";
                DisplayMessageLine(display);
            }

            /*if ((RAMtype_radiobutton.Checked == true) && (ProgOrReadEE_flag == 0))
            {
            }
            else
            {
                FM12XX_Card.Init_TDA8007(out strReceived);
                if (strReceived == "Error")
                {
                    display = "Init TDA8007失败 ";
                    DisplayMessageLine(display);
                    return 1;
                }
                else
                {
                    display = "Init TDA8007\t ";
                    DisplayMessageLine(display);
                }
            }*/
            return 0;
        }
        public virtual int ProgOrReadEEInCLA(uint EEtype, uint nProgStartBlock, uint nProgEndBlock, out string strReceived)
        {
            int result, FMXX_SEL;
            uint i, j, Ext_i, ProgNumi, nProgSum, nAddr, changeEE, nStep, ProgStartBlock_temp, nAddr_temp, buffer_start_addr, lib_startaddr;
            byte[] buff;
            int Ext_addr_change, ExtOpt, prog_mem_type, EEmax, prog_extand_mode, LIBsize, cmp1, cmp2, cwd14;
            string sendbuf, BlockAddr, BlockData, auth_key, STR;
            uint temp, Block_temp;//C_Data, 
            string DestAddr;
            uint nTemp3 = 0;
            byte[] destaddr;
            uint nDestBlock = 0;

            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            ProgStartBlock_temp = nProgStartBlock;//nProgStartBlock用于起始地址   ProgStartBlock_temp用于地址切换
            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            //进度条
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_ROM.Checked = false;
            MEM_EE.Checked = false;


            FM12XX_Card.Init_TDA8007(out strReceived);
            for (i = 0; i < ProgFileMaxLen; i++)
            {
                Readfilebuf[i] = 0x00;		//缓冲区初始化为0x00
            }

            FM12XX_Card.SetField(1, out strReceived);
            if (strReceived == "Error")
            {
                display = "SetField         \t\tFaild";
                DisplayMessageLine(display);
                return 1;
            }

            buff = new byte[0x90];
            //Active
            //WriteReg(TxControl,0x5b);
            FM12XX_Card.Set_FM1715_reg(FM1715Reg_Str.TxControl, "5b", out strReceived);

            //FM12XX_Card.REQA(out strReceived);
            //FM12XX_Card.AntiColl_lv(1, out strReceived);
            //FM12XX_Card.Select(out strReceived);
            FM12XX_Card.Active(out display);
            if (strReceived.Contains("Error"))
            {
                display = "选卡错误 ！";
                DisplayMessageLine(display);
                return 1;
            }

            if (Auth_checkBox.Checked)
            {
                auth_key = key_richTextBox.Text.Replace(" ", "");
                if ((auth_key == "") || (auth_key.Length != 32))
                {
                    display = "认证的密钥长度不对！！！   \t\t";
                    DisplayMessageLine(display);
                    return 1;
                }
                FM12XX_Card.TransceiveCL("A200", "03", "09", out receive);//CNN 添加认证，使认证时也能配置控制字
                int k = FM12XX_Card.TransceiveCL(auth_key, "03", "09", out receive);//LOAD密钥
                if (k != 0)
                {
                    display = "认证:   \t\t" + "错误！！";
                    DisplayMessageLine(display);
                    return 1;
                }
                display = "认证:   \t\t" + "通过";
                DisplayMessageLine(display);
            }
            if (FM336_radioButton.Checked)
            {
                FMXX_SEL = 1;
            }
            else
                FMXX_SEL = 0;
  
            FM12XX_Card.ProgExt_CwdCheck(0, FMXX_SEL, out Ext_addr_change, out prog_extand_mode, out ExtOpt, out prog_mem_type, out EEmax, out LIBsize, out cwd14, out STR);//返回的是数据EE max
            if (STR == "error")
            {
                display = "初始化指令出错！！！请查看是否需要认证？？或有其他错误……   \t\t";
                DisplayMessageLine(display);
                return 1;
            }
            cmp1 = (EEmax - Ext_addr_change) / 1024;
            Patch_Size.Text = cmp1.ToString();
            lib_Size.Text = ((LIBsize >> 3) * 2).ToString();
            if (prog_mem_type == 0 && PROG_EEtype_radiobutton.Checked && prog_extand_mode == 1)
            {
                if (MessageBox.Show("程序是ROM存储，是否继续编程？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                }
                else
                    return 1;
                /*display = "无权限对ROM空间编程，请检查配置字……   \t\t";
                DisplayMessageLine(display);
                return 1;*/
            }
            if (ExtOpt == 0)
            {
                prog_extand_memEE.Checked = true;
            }
            else
            {
                display = "非接不支持RAM扩展！！！！！";
                DisplayMessageLine(display);
                return 1;
            }
            if (DATA_EEtype_radiobutton.Checked == true || RAMtype_radiobutton.Checked == true)//非程序EE 不支持扩展 CNN20130221
            {
                prog_extand_mode = 1;
            }
            if (prog_mem_type == 0)
                MEM_ROM.Checked = true;
            else
                MEM_EE.Checked = true;

            if (prog_extand_mode == 1)//段后扩展
            {
                ProgOrReadEE_EXT_flag = 1;
                prog_extand_mode1.Checked = true;
                if (FM336_radioButton.Checked)
                {
                    //EEmax = 0x60000;
                    if ((cwd14 & 0x10) == 0x00)//不使能ROM奇偶校验 384k
                    {
                        EEmax = 0x60000;
                    }
                    else if ((cwd14 & 0x08) == 0x00)//1位校验 336k
                    {
                        EEmax = 0x54000;
                    }
                    else if ((cwd14 & 0x08) == 0x08)//4位校验 256k
                    {
                        EEmax = 0x40000;
                    }
                    else
                        EEmax = 0x60000;
                }
                else
                {
                    if (prog_mem_type == 0 && PROG_EEtype_radiobutton.Checked)
                    {
                        EEmax = 0x40000;//程序rom max                    
                    }
                    else
                    {
                        EEmax = 0x3C000;
                    }
                }
            }
            else//整体扩展
            {
                prog_extand_mode0.Checked = true;
                if (ProgOrReadEE_flag == 0 || chkReadVerify.Checked)
                {
                    if (Patch_Size.Text == "0")
                    {
                        ProgStartBlock_temp = 0;
                        EEtype = 0;
                        display = "Patch区为0K。整体扩展无意义！！现在程序起始地址在程序区：逻辑地址0xFF0000";
                    }
                    else
                    {
                        ProgStartBlock_temp = (uint)(Ext_addr_change / 16);
                        EEtype = 1;
                        display = "整体扩展：起始地址扩展到逻辑地址" + (Ext_addr_change + 0x010000).ToString("X6");
                    }
                    DisplayMessageLine(display);
                }
            }

            //选择编程EE
            buff[0] = 0x32;
            buff[1] = (EEtype == 0) ? Convert.ToByte(0xC0) : Convert.ToByte(0x80);

            sendbuf = byteToHexStr(2, buff);
            result = FM12XX_Card.SendReceiveCL(sendbuf, out strReceived);            
            if (result == 1)
            {
                strReceived = buff[0].ToString("X2") + "命令出错";
                display = strReceived;
                DisplayMessageLine(display);
                return 1;
            }
            //nRet=ActiveA(buff);

            changeEE = 0;
            i = 0;
            Ext_i = 0;

            buff[0] = Convert.ToByte(0x31);
            temp = ProgStartBlock_temp / 0x100;

            //buff[1] = (EEtype == 0) ? Convert.ToByte(0x80 + temp) : Convert.ToByte(0x90 + temp);
            //buff[1] = Convert.ToByte(0x80 + temp);
            buff[1] = Convert.ToByte(0x80 + temp);

            sendbuf = byteToHexStr(2, buff);
            result = FM12XX_Card.SendReceiveCL(sendbuf, out strReceived);
            if (result == 1)
            {
                strReceived = buff[0].ToString("X2") + "命令出错";
                display = strReceived;
                DisplayMessageLine(display);
                return 1;
            }

            if (A1Program.Checked == true)
            {		//A1命令,写128字节
                nStep = 8;
            }
            else
            {
                nStep = 1;
            }
            if (ProgOrReadEE_flag == 1)//读取EE
            {
                nStep = 1;
            }

            buffer_start_addr = nProgStartBlock;

            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi += nStep)
            {
                if (ProgOrReadEE_flag == 0 && PROG_EEtype_radiobutton.Checked && Ext_i == 0)
                {
                    if ((ProgOrReadEE_FF_flag == 0) && (ProgNumi * 16 > 0x000000) && (ProgNumi * 16 < 0x010000) && (ProgNumi < nProgSum))
                    {
                        i = 0x1000;
                        ProgNumi = i;
                    }
                    if ((ProgOrReadEE_80_flag == 0) && (ProgNumi * 16 > 0x00FFFF) && (ProgNumi * 16 < 0x020000) && (ProgNumi < nProgSum))
                    {
                        i = 0x2000;
                        ProgNumi = i;
                    }
                    if ((ProgOrReadEE_81_flag == 0) && (ProgNumi * 16 > 0x01FFFF) && (ProgNumi * 16 < 0x030000) && (ProgNumi < nProgSum))
                    {
                        i = 0x3000;
                        ProgNumi = i;
                        //ProgOrReadEE_81_flag = 2;
                    }
                    if (FM336_radioButton.Checked)
                    {
                        if ((ProgOrReadEE_82_flag == 0) && (ProgNumi * 16 > 0x02FFFF) && (ProgNumi * 16 < 0x040000) && (ProgNumi < nProgSum))
                        {
                            i = 0x4000;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_83_flag == 0) && (ProgNumi * 16 > 0x03FFFF) && (ProgNumi * 16 < 0x050000) && (ProgNumi < nProgSum))
                        {
                            i = 0x5000;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_84_flag == 0) && (ProgNumi * 16 > 0x04FFFF) && (ProgNumi * 16 < EEmax - 2) && (ProgNumi < nProgSum))
                        {
                            //i = 0x6000;
                            i = (uint)(EEmax) / 0x10;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_85_flag == 0) && (ProgNumi * 16 > 0x05FFFF) && (ProgNumi * 16 < 0x070000) && (ProgNumi < nProgSum) && (nProgSum > 0x7000))
                        {
                            i = 0x7000;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_86_flag == 0) && (ProgNumi * 16 > 0x06FFFF) && (ProgNumi * 16 < 0x080000) && (ProgNumi < nProgSum))
                        {
                            i = 0x8000;
                            ProgNumi = i;
                        }

                    }
                    else
                    {
                        if ((ProgOrReadEE_82_flag == 0) && (ProgNumi * 16 > 0x02FFFF) && (ProgNumi * 16 < EEmax - 2) && (ProgNumi < nProgSum))
                        {
                            if (prog_mem_type == 0)
                            {
                                i = 0x4000;
                            }
                            else
                                i = 0x3C00;
                            ProgNumi = i;
                            //ProgOrReadEE_82_flag = 2;                      

                        }

                        if ((ProgOrReadEE_83_flag == 0) && (ProgNumi * 16 > EEmax - 2) && (ProgNumi * 16 < 0x050000) && (ProgNumi < nProgSum) && (nProgSum > 0x5000))
                        {
                            i = 0x5000;
                            ProgNumi = i;
                        }
                        if ((ProgOrReadEE_84_flag == 0) && (ProgNumi * 16 > 0x04FFFF) && (ProgNumi * 16 < 0x060000) && (ProgNumi < nProgSum))
                        {
                            i = 0x6000;
                            ProgNumi = i;
                        }
                    }
                    /*if ((ProgOrReadEE_90_flag == 0) && (ProgNumi * 16 > 0x05FFFF) && (ProgNumi * 16 < 0x070000) && (ProgNumi < nProgSum))
                    {
                        i = 0x7000;
                        ProgNumi = i;
                    }*/
                }
                if (FM336_radioButton.Checked)
                {
                    lib_startaddr = 0x07FFFF;
                }
                else
                {
                    lib_startaddr = 0x05FFFF;
                }
                if (ProgOrReadEE_90_flag == 1 && (((ProgNumi + nProgStartBlock) * 16 > lib_startaddr) || (nProgStartBlock * 16 > lib_startaddr)) && PROG_EEtype_radiobutton.Checked && (ProgNumi < nProgSum))// lib 20130410 CNN
                {
                    ProgOrReadEE_90_flag = 2;
                    if (LIBsize != 0)//lib不为0
                    {
                        result = FM12XX_Card.SendReceiveCL("32E0", out strReceived);
                        if (result == 1)
                        {
                            strReceived = "32命令切换编程地址到LIB区出错";
                            display = strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }
                        ProgStartBlock_temp = 0;
                        i = 0;
                        changeEE = 1;
                        display = "编程地址切换到LIB区";
                        DisplayMessageLine(display);
                    }
                    else
                    {
                        display = "程序要LIB区大小为0，无法将相应数据进行lib区编程";
                        DisplayMessageLine(display);
                        return 1;
                    }
                }
                //打开程序时，lib区数据是放在普通程序后面的，因此这边用lib_startaddr作为界线
                if (ProgOrReadEE_EXT_flag == 1 && (((ProgNumi + nProgStartBlock) * 16 > EEmax - 1) || (nProgStartBlock * 16 > EEmax - 1)) && PROG_EEtype_radiobutton.Checked && (ProgNumi < nProgSum) && (((ProgNumi + nProgStartBlock) * 16 < lib_startaddr + 1) && (ProgStartBlock_temp * 16 < lib_startaddr + 1)))
                {     //CNN 20130325 添加：如果起始地址大于EEmax,则切换到段后扩展补丁区                       
                    cmp2 = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) - (EEmax - 1);
                    cmp2 = cmp2 / 1024;
                    if (Ext_i == 0)
                    {
                        display = "程序空间已满，将进行段后扩展!";
                        DisplayMessageLine(display);
                        if (cmp1 < cmp2)
                        {
                            display = "待扩展的数据大小为" + (cmp2).ToString() + "K,补丁区为" + Patch_Size.Text + "K,不满足扩展需要！";
                            if (MessageBox.Show(display + "是否继续编程？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                return 1;
                            }
                        }
                        display = "将切换到段后扩展地址" + (Ext_addr_change + 0x010000).ToString("X6");
                        DisplayMessageLine(display);
                        changeEE = 1;
                        Ext_i = 1;
                        if (ExtOpt == 1)//RAM
                        {
                            display = "EE地址切换到段后扩展选择了RAM，请选择CT编程！！";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        //ProgStartBlock_temp = (uint)(ProgStartBlock_temp * 16 - EEmax + Ext_addr_change) / 16;//CNN 20130328
                        ProgStartBlock_temp = (uint)(Ext_addr_change) / 16;//CNN 20131230
                        //选择段后扩展数据EE
                        buff[0] = 0x32;
                        buff[1] = Convert.ToByte(0x80);
                        sendbuf = byteToHexStr(2, buff);
                        result = FM12XX_Card.SendReceiveCL(sendbuf, out strReceived);
                        if (result == 1)
                        {
                            strReceived = "32命令“选择段后扩展数据EE”出错";
                            display = strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }

                        i = 0;
                    }

                    // i = Ext_i;
                    //Ext_i = Ext_i + nStep;
                }

                if (((ProgStartBlock_temp + i) % 0x100 == 0) || (changeEE == 1))
                {		//高位地址线切换
                    temp = (ProgStartBlock_temp + i) / 0x100;
                    if ((i != 0) || (changeEE == 1))
                    {
                        changeEE = 0;
                        buff[0] = Convert.ToByte(0x31);
                        buff[1] = Convert.ToByte(0x80 + temp);
                        sendbuf = byteToHexStr(2, buff);
                        result = FM12XX_Card.SendReceiveCL(sendbuf, out strReceived);
                        if (result == 1)
                        {
                            strReceived = "EE地址切换出错" + (ProgStartBlock_temp + i).ToString("X6");
                            display = strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }
                        else
                        {
                            display = "EE地址切换  \t\t\tSucceeded " + "31" + (temp + 0x80).ToString("X2");
                            DisplayMessageLine(display);
                        }

                    }
                }
                Block_temp = ProgStartBlock_temp % 0x100;
                nAddr_temp = (buffer_start_addr + ProgNumi) * 0x10;
                if (ProgOrReadEE_flag == 0)//编程
                {
                    //编程操作   
                    if (A1Program.Checked == true)
                    {		//A1命令,写128字节
                        for (j = 0; j < 0x80; j++)
                        {
                            nAddr = nAddr_temp + j;
                            temp = Progfilebuf[nAddr];
                            buff[j] = Progfilebuf[nAddr];
                        }
                        BlockAddr = ((nProgStartBlock + Block_temp + i) % 0x100).ToString("X2");
                        BlockData = byteToHexStr(0x80, buff);// buff.ToString();
                        result = FM12XX_Card.Write128Bytes(BlockAddr, BlockData, out strReceived);
                    }
                    else
                    {
                        for (j = 0; j < 0x10; j++)
                        {
                            nAddr = nAddr_temp + j;
                            temp = Progfilebuf[nAddr];
                            buff[j] = Progfilebuf[nAddr];
                        }
                        BlockAddr = ((nProgStartBlock + Block_temp + i) % 0x100).ToString("X2");
                        BlockData = byteToHexStr(0x10, buff);// buff.ToString();
                        result = FM12XX_Card.WriteBlock(BlockAddr, BlockData, out strReceived);
                    }

                    if (result == 1)
                    {//编程出错
                        strReceived = "编程出错" + (nProgStartBlock + Block_temp + i).ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                }


                else//读取EE操作
                {
                    //读取操作                    
                    BlockAddr = ((nProgStartBlock + i) % 0x100).ToString("X2");
                    result = FM12XX_Card.ReadBlock(BlockAddr, out strReceived);
                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + (nProgStartBlock + i).ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    /*//RD Encrypt begin 
                    for(j=0;j<BLOCKSIZE;j++)
                    {
                        buff[j] = DataEncryptPro(g_ProgParam.btEncryptOpt, buff[j], btAddr*0x10+j);
                    }
                    //RD Encrypt begin */

                    for (j = 0; j < 0x10; j++)
                    {
                        nAddr = nAddr_temp + j;
                        Readfilebuf[nAddr] = buff[j];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < 0x10; j++)
                        {
                            nAddr = nAddr_temp + j;
                            if (nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[nAddr] != Progfilebuf[nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + nAddr.ToString("X6");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < 0x10; j++)
                    {
                        nAddr = nAddr_temp + j;
                        if (nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("读取完毕");
                            break;
                        }
                        Progfilebuf[nAddr] = Readfilebuf[nAddr];
                    }
                }
                ProgEE_progressBar.Value = (int)ProgNumi;
                i = i + nStep;

            }
            if (ProgOrReadEE_flag == 1)
            {
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            FM12XX_Card.SetField(0, out strReceived);
            if (strReceived == "Error")
            {
                display = "ResetField       \t\tFaild";
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                display = "ResetField       \t\tSucceeded";
                DisplayMessageLine(display);
            }
            return 0;
        }

        public virtual int ProgOrReadEEInCLA294(uint EEtype, uint nProgStartBlock, uint nProgEndBlock, out string strReceived)
        {
            uint i, j, ProgNumi, nProgSum, buf_nAddr, nAddr;
            string sendbuf;
            byte[] buff;
            buff = new byte[0x20];
            byte[] init;
            init = new byte[0x02];
            int write_length, read_length;
            Boolean k=true;

            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            write_length = 0x10; // 写操作长度0x10
            read_length = 0x10; // 读操作长度0x10

            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            //setup communication channel
            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            FM12XX_Card.SetField(1, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            FM12XX_Card.Active(out display);
            if ((strReceived == "Error") || (strReceived == "NO REQA"))
            {
                display = "CLA选卡失败";
                DisplayMessageLine(display);
            }
            Delay(1);

            if (EEtype == 0) //程序EE
            {
                init[0] = 0x31;
                if(FM294_radioButton.Checked == true)
                    init[1] = 0x90;
                else if(FM274_radioButton.Checked == true)
                    init[1] = 0xC0;
                else
                    init[1] = 0x90;
            }
            else            //数据EE
            {
                init[0] = 0x31;
                if(FM294_radioButton.Checked == true)
                    init[1] = 0x80;
                else if (FM274_radioButton.Checked == true)
                    init[1] = 0xA0; 
                else
                    init[1] = 0x8A; 
            }
            //Programming
            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {                
                if (ProgOrReadEE_flag == 0)     //仅编程，不读出
                {
                    buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length;
                    nAddr = buf_nAddr;
                    if (k)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                        init[1] += (byte)(nAddr/0x1000); 
                        sendbuf = byteToHexStr(2, init);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived.Length != 32)
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        init[1]++;
                        k = false;
                    }
                    else if (((nProgStartBlock % 0x100) + ProgNumi) % 0x100 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                        sendbuf = byteToHexStr(2, init);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived.Length != 32)
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        init[1] += 1;    
                    }
                     //一个block——16bytes的编程操作
                    //if (A1Program.Checked == true)
                    buff[0] = 0xA0;
                    buff[1] = (byte)(((nProgStartBlock % 0x100) + ProgNumi) % 0x100);
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if (strReceived != "0A")
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X4");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < 0x10; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if (strReceived != "0A")
                    {//编程出错
                        strReceived = "EE编程出错" + nAddr.ToString("X4");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    //ProgEE_progressBar.Value = (int)ProgNumi + 1;

                }
                else//读取EE操作
                {
                    nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length;
                    if (k)
                    {             
                        display = "正在读取地址：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                        init[1] += (byte)(nAddr / 0x1000); 
                        sendbuf = byteToHexStr(2, init);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived.Length != 32)
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        init[1] ++;
                        k = false;
                    }
                    else if (((nProgStartBlock % 0x100) + ProgNumi) % 0x100 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                        sendbuf = byteToHexStr(2, init);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived.Length != 32)
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        init[1] ++;
                    }
                    //单次读操作
                    buff[0] = 0x30;
                    buff[1] = (byte)(((nProgStartBlock%0x100) + ProgNumi) % 0x100);
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    //for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    //{
                    //    Readfilebuf[ProgNumi * read_length + j] = buff[j];
                    //}
     
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < read_length; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                            Readfilebuf[ProgNumi * read_length + j] = buff[j];                     
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[ProgNumi * read_length + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X4");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[ProgNumi * read_length + j].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    else
                    {
                        for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                            Readfilebuf[ProgNumi * read_length + j] = buff[j];
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            Progfilebuf[buf_nAddr] = Readfilebuf[ProgNumi * read_length + j];
                        }
                    }
                    //display = "当前写入文件的源地址：" + nAddr.ToString("X6");
                    //DisplayMessageLine(display);                   
                }
                ProgEE_progressBar.Value = (int)ProgNumi + 1;
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1]*256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            return 0;
        }

        public virtual int ProgOrReadEEInCT294(uint EEtype, uint nProgStartBlock, uint nProgEndBlock, out string strReceived)
        {
            uint i, j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base;
            string auth_key, sendbuf;
            int result;
            byte[] buff;
            buff = new byte[0x103];
            int write_length;
            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            write_length = 0x40;

            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.Cold_Reset("02", out strReceived);
            if (strReceived == "Error")
            {
                display = "Cold Reset失败 ";
                DisplayMessageLine(display);
            }
            Delay(1);
            if (checkBox_CTHighBaud.Checked)
            {
                result = FM12XX_Card.TransceiveCT("0001180000", "02", out strReceived);//切换到31分频
                //初始化          
                // result = FM12XX_Card.TransceiveCT("0001950000", "02", out strReceived);//切换到32分频
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到32分频
                //result = FM12XX_Card.Set_TDA8007_reg("03", "21", out strReceived);
                //result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);

                //切换到31分频
                result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);

                DisplayMessageLine("切换到115200波特率");
            }
            else
            {
                result = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);//9600
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到31分频
                result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);
                DisplayMessageLine("切换到115200波特率");
            }

            //INITIAL EEPROM命令               
            if (Auth_checkBox.Checked)
            {
                auth_key = key_richTextBox.Text.Replace(" ", "");
                if ((auth_key == "") || (auth_key.Length != 32))
                {
                    display = "认证的密钥长度不对！！！   \t\t";
                    DisplayMessageLine(display);
                    return 1;
                }
                FM12XX_Card.TransceiveCT("0055000010", "02", out strReceived);//CNN 添加认证，使认证时也能配置控制字
                int k = FM12XX_Card.TransceiveCT(auth_key, "02", out strReceived);//LOAD密钥
                if (k != 0)
                {
                    display = "认证:   \t\t" + "错误！！";
                    DisplayMessageLine(display);
                    return 1;
                }
                display = "认证:   \t\t" + "通过";
                DisplayMessageLine(display);
            }

            if (EEtype == 0) //prog
            {
                if (FM294_radioButton.Checked == true)
                    sendbuf = "0002010000";
                else
                    sendbuf = "00022D0100";
            }
            else //EE
            {
                if (FM294_radioButton.Checked == true)
                    sendbuf = "0002000000";
                else
                    sendbuf = "00022D0000";            
            }

            FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (strReceived != "029000")
            {
                display = "编程模式配置出错，返回：" + strReceived;
                DisplayMessageLine(display);
                return 1;
            }
            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length;
                nAddr = buf_nAddr;               
                if (ProgOrReadEE_flag == 0)//编程
                {
                    buff[0] = 0x00;
                    buff[1] = 0x00;			//INS = 0x00,WRITE EEPROM命令
                    buff[2] = (byte)(nAddr >> 0x08);	//地址高位
                    buff[3] = (byte)(nAddr);	//地址低位
                    buff[4] = 0x40;

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (result != 0x00)
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X4");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < 0x40; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (result != 0x00)
                    {//编程出错
                        strReceived = "EE编程出错" + nAddr.ToString("X4");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    if (strReceived != "9000")
                    {
                        display = "返回：" + strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    ProgEE_progressBar.Value = (int)ProgNumi + 1;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                    }
                }
                else//读取EE操作
                {
                    //读取操作                      
                    buff[0] = 0x00;
                    buff[1] = 0x04;			//INS = 0x04,READ EEPROM命令
                    buff[2] = (byte)(nAddr >> 0x08);	//地址高位
                    buff[3] = (byte)(nAddr);	//地址低位
                    buff[4] = 0x40;			//读字节数Lc=0x40表示读取64个字节

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X4");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < write_length; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        Readfilebuf[ProgNumi * write_length + j] = buff[j + 1];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < write_length; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length + j;
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[ProgNumi * write_length + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X4");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < write_length; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length + j;
                        if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("读取完毕");
                            break;
                        }
                        Progfilebuf[buf_nAddr] = Readfilebuf[ProgNumi * (uint)write_length + j];
                    }

                    ProgEE_progressBar.Value = (int)ProgNumi + 1;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                    }

                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            return 0;
        }

        public virtual int nvmporgCT347(uint count, string addstart, uint addbase, uint addend, byte[] filebuff, byte[] readbuf, int type, out string strReceived)
        {
            string addrnvm = "", sendbuf;
            uint buf_nAddr, addr, tmp;
            int result, cnt;
            byte[] buff;
            buff = new byte[0x103];

            if (fm347_data_erase == false)
            {
                switch (type)
                {
                    case 1: cnt = 8;    //sector 2k per prog 256 bytes
                        break;
                    case 2: cnt = 1;    //sector 256bytes per prog 256 bytes
                        break;
                    default: cnt = 8;    //sector 2k per prog 256 bytes
                        break;
                }

                buf_nAddr = Convert.ToUInt32(addstart, 16);
                for (i = 0; i < cnt; i++) //initial ram 80
                {
                    addr = buf_nAddr + (uint)(i * 0x100);
                    FM12XX_Card.TransceiveCT("000D800000", "02", out strReceived);
                    sendbuf = "003C" + i.ToString("X2") + "0000";
                    FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    tmp = addr - addbase;
                    for (j = 0; j < 0x100; j++)
                    {
                        if ((tmp + j) > addend)     //保留之前的内容           
                            buff[j] = readbuf[tmp + j];
                        else                        //要改写的内容 
                            buff[j] = filebuff[tmp + j];
                    }
                    sendbuf = byteToHexStr(0x100, buff);
                    FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                }
            }
            tmp = 0x00000045;
            if (pre_prog.Checked == true)
                tmp |= 0x00000400;
            if (retry.Checked == true)
                tmp |= 0x00001000;
            if (CB_NoErase.Checked == true)
                tmp &= 0xFFFFFFFB;
            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
            FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
            FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

            FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
            if (type == 1) //2k
                FM12XX_Card.TransceiveCT("00800000" + addstart + "000007FF" + tmp.ToString("X8"), "02", out strReceived);
            else
                FM12XX_Card.TransceiveCT("00800000" + addstart + "000000FF" + tmp.ToString("X8"), "02", out strReceived);

            FM12XX_Card.TransceiveCT("000D" + addstart.Substring(2, 2) + "0000", "02", out strReceived); //NVM
            FM12XX_Card.TransceiveCT("0004" + addstart.Substring(4, 4) + "01", "02", out strReceived); // NVM

            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
            FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010
            result = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
            if (result != 0)
            {
                if (type == 1) //2k
                    DisplayMessageLine("2k-Bytes  sector " + count.ToString("X2") + "-- " + addrnvm + " 编程:\t<-\t error!");
                else
                    DisplayMessageLine("256-Bytes sector " + count.ToString("X2") + "-- " + addrnvm + " 编程:\t<-\t error!");
                return 1;
            }
            FM12XX_Card.TransceiveCT("0004020004", "02", out strReceived); // NVM nvm_op_status E00200
            if ((strToHexByte(strReceived.Substring(8, 2))[0] & 0x08) == 0)
            {
                if (type == 1) //2k
                    DisplayMessageLine("2k-Bytes  sector " + count.ToString("X2") + "-- " + addrnvm + " 编程:\t<-\t" + "Successed");
                else
                    DisplayMessageLine("256-Bytes sector " + count.ToString("X2") + "-- " + addrnvm + " 编程:\t<-\t" + "Successed");
            }
            else
            {
                if (type == 1) //2k
                    DisplayMessageLine("2k-Bytes  sector " + count.ToString("X2") + "-- " + addrnvm + " 编程:\t<-\t" + "擦写校验失败");
                else
                    DisplayMessageLine("256-Bytes sector " + count.ToString("X2") + "-- " + addrnvm + " 编程:\t<-\t" + "擦写校验失败");
                return 1;
            }

            ProgEE_progressBar.Value = (int)(count + 1);

            strReceived = addrnvm;
            return 0;
        }


        public virtual int ProgOrReadEEInCT347(uint nProgStartBlock, uint cnt2k, uint cnt2b, uint nProgOffsetAddr)
        {
            uint i, j, cnt = 0, t, ProgNumi, nProgSum, buf_nAddr = 0, nAddr, nAddr_base, nAddr_end = 0;
            string strReceived, sendbuf, saddrnvm, addr, strtmp;
            int result;
            byte[] buff;
            buff = new byte[0x103];

            //进度条
            nProgSum = cnt2k + cnt2b;
            strReceived = "";
            if (ProgOrReadEE_flag == 0 && cnt2b == 0)
                cnt = 8;
            else
                cnt = 1;

            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            if (RAMtype_radiobutton.Checked == false)
            {
                FM12XX_Card.SetField(0, out display);
                DisplayMessageLine("SetField     \t\t\t" + display);
                FM12XX_Card.Init_TDA8007(out strReceived);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                    DisplayMessageLine("Cold Reset失败 ");
                else
                    DisplayMessageLine("Cold ATR:    \t\t" + strReceived);
                FM12XX_Card.Set_TDA8007_reg("05", "00", out strReceived);  //PCD BGT = 2.5etu
                if (checkBox_CTHighBaud.Checked)
                {
                    result = FM12XX_Card.TransceiveCT("0001950000", "02", out strReceived);//切换到32分频
                    if (result == 1)
                    {
                        DisplayMessageLine("ERROR：7816接口初始化 失败");
                        return 1;
                    }
                    result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);
                    DisplayMessageLine("切换到111875波特率");
                }
                else
                {
                    result = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    if (result == 1)
                    {
                        DisplayMessageLine("ERROR：7816接口初始化 失败");
                        return 1;
                    }
                }
            }
            switch (sec_addr.SelectedIndex)
            {
                case 0: //程序 用户程序区-00
                    nAddr_base = nProgOffsetAddr;
                    break;
                case 1: //程序 bootload区-50
                    nAddr_base = 0x500000 + nProgOffsetAddr;
                    break;
                case 2: //程序 firmware区-70
                    nAddr_base = 0x700000 + nProgOffsetAddr;
                    break;
                case 3: //数据 通用数据区-B0
                    nAddr_base = 0xB00000 + nProgOffsetAddr;
                    break;
                case 4: //数据 敏感数据区-D0
                    nAddr_base = 0xD00000 + nProgOffsetAddr;
                    break;
                case 5: //数据 逻辑加密区-DA
                    nAddr_base = 0xDA0000 + nProgOffsetAddr;
                    break;
                case 6: //RAM 补丁程序区-30
                    nAddr_base = 0x300000 + nProgOffsetAddr;
                    break;
                case 7: //RAM CPU缓存区-80
                    nAddr_base = 0x800000 + nProgOffsetAddr;
                    break;
                case 8: //RAM 非接缓存区-AF
                    nAddr_base = 0xAF0000 + nProgOffsetAddr;
                    break;
                default:
                    nAddr_base = nProgOffsetAddr;
                    break;
            }
            nAddr_end = Convert.ToUInt32(ProgEEEndAddr.Text, 16);
            ProgFileLenth = (int)nAddr_end + 1;
            if (ProgOrReadEE_flag == 0 && RAMtype_radiobutton.Checked == false && ((ProgEEEndAddr.Text.Substring(4, 2) != "FF" && cnt == 1) || (ProgEEEndAddr.Text.Substring(3, 3) != "7FF" && ProgEEEndAddr.Text.Substring(3, 3) != "FFF" && cnt == 8)))  //编程前保留未擦写区域
            {
                saddrnvm = (nAddr_end + nAddr_base).ToString("X8");
                result = FM12XX_Card.TransceiveCT("000D" + saddrnvm.Substring(2, 2) + "0000", "02", out strReceived); //NVM
                if (result == 1)
                {
                    DisplayMessageLine("ERROR -- 7816接口初始化 失败");
                    return 1;
                }
                i = Convert.ToUInt16(saddrnvm.Substring(5, 1), 16);
                if (cnt2b == 0 && i <= 0x07)
                    addr = saddrnvm.Substring(4, 1) + "0";
                else if (cnt2b == 0 && i > 0x07)
                    addr = saddrnvm.Substring(4, 1) + "8";
                else
                    addr = saddrnvm.Substring(4, 2);
                for (j = 0; j < cnt; j++)
                {
                    sendbuf = (Convert.ToUInt16(addr, 16) + j).ToString("X2");
                    //strtmp = saddrnvm.Substring(2, 2) + sendbuf + "00";
                    strtmp = sendbuf + "00";
                    buf_nAddr = Convert.ToUInt16(strtmp, 16);
                    DisplayMessageLine("预保留地址：  " + strtmp);
                    result = FM12XX_Card.TransceiveCT("0004" + sendbuf + "0000", "02", out strReceived);
                    if (result == 1)
                    {
                        DisplayMessageLine("读取失败！");
                        return 1;
                    }
                    else
                        buff = strToHexByte(strReceived);
                    for (i = 0; i < 0x100; i++)
                        Readfilebuf[buf_nAddr + i] = buff[i + 1];
                }
            }
            if ((ProgOrReadEE_flag == 0) && (RAMtype_radiobutton.Checked == false))
            {
                //配置芯片擦写时间　for FM347
                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("003C020408", "02", out strReceived);
                if (retry.Checked == true)
                    result = FM12XX_Card.TransceiveCT("0000" + textBox_prepg.Text + textBox_tprog.Text + "007D" + textBox_terase.Text + "32", "02", out strReceived);
                else
                    result = FM12XX_Card.TransceiveCT("0000" + textBox_tprog.Text + textBox_tprog.Text + "007D0C" + textBox_terase.Text, "02", out strReceived);
                if (result == 1)
                {
                    DisplayMessageLine("ERROR：init_ctrl接口配置nvm擦写时间失败!");
                    return 1;
                }
                else
                    DisplayMessageLine("INFO：FM347擦写时间配置成 Tprog " + label_tprog_Calc.Text + "us, Terase " + label_terase_Calc.Text + "ms");
            }
            if (ProgOrReadEE_flag == 0)//编程
            {
                fm347_data_erase = true;
                for (i = 1; i < ProgFileLenth; i++)
                {
                    if (Progfilebuf[0] != Progfilebuf[i])
                    {
                        fm347_data_erase = false;
                        break;
                    }
                }
                if (fm347_data_erase == true)
                {
                    for (i = 0; i < 8; i++) //initial ram 80
                    {
                        FM12XX_Card.TransceiveCT("000D800000", "02", out strReceived);
                        sendbuf = "003C" + i.ToString("X2") + "0000";
                        FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                        for (j = 0; j < 0x100; j++)
                        {
                            buff[j] = Progfilebuf[0];
                        }
                        sendbuf = byteToHexStr(0x100, buff);
                        FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    }
                }
            }

            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                //buf_nAddr = (ProgNumi + nProgStartBlock) * (uint)write_length + nAddr_base;
                if (ProgNumi <= cnt2k && cnt2k > 0)
                    buf_nAddr = (ProgNumi + nProgStartBlock) * 0x800 + nAddr_base;
                else if (ProgNumi == 0)
                    buf_nAddr = (ProgNumi + nProgStartBlock) * 0x100 + nAddr_base;
                else
                    buf_nAddr += 0x100;

                saddrnvm = buf_nAddr.ToString("X8");
                if (ProgOrReadEE_flag == 0)//编程
                {
                    if (ProgNumi < cnt2k)
                    {
                        result = nvmporgCT347(ProgNumi, saddrnvm, nAddr_base, nAddr_end, Progfilebuf, Readfilebuf, 1, out strReceived); //sector2k
                        if (result == 1)
                            return 1;
                    }
                    else if (RAMtype_radiobutton.Checked != true)
                    {
                        result = nvmporgCT347(ProgNumi, saddrnvm, nAddr_base, nAddr_end, Progfilebuf, Readfilebuf, 2, out strReceived); //sector 256bytes
                        if (result == 1)
                            return 1;
                    }
                    else
                    {
                        sendbuf = "000D" + saddrnvm.Substring(2, 2) + "0000";
                        FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                        t = buf_nAddr - nAddr_base;
                        i = nAddr_end / 0x100;
                        if (nAddr_end % 0x100 == 0xFF)
                            i++;
                        if ((t / 0x100) < i)
                        {
                            sendbuf = "003C" + saddrnvm.Substring(4, 4) + "00";
                            FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                            for (j = 0; j < 0x100; j++)
                                buff[j] = Progfilebuf[t + j];
                            sendbuf = byteToHexStr(0x100, buff);
                        }
                        else
                        {
                            i = 4 - (uint)ProgFileLenth % 4;
                            if (i < 4) //不足4字节，补0
                                for (j = 0; j < i; j++)
                                    Progfilebuf[ProgFileLenth++] = 0;

                            sendbuf = "003C" + saddrnvm.Substring(4, 4) + ProgFileLenth.ToString("X6").Substring(4, 2);
                            FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                            for (j = 0; j < ProgFileLenth % 0x100; j++)
                                buff[j] = Progfilebuf[t + j];
                            sendbuf = byteToHexStr(ProgFileLenth % 0x100, buff);
                        }
                        result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                        DisplayMessageLine("写RAM地址:\t<-\t" + saddrnvm + "\t" + strReceived);
                        if (result == 1)
                            return 1;
                        ProgEE_progressBar.Value = (int)(ProgNumi + 1);
                    }
                }
                else//读取EE操作
                {
                    result = FM12XX_Card.TransceiveCT("000D" + saddrnvm.Substring(2, 2) + "0000", "02", out strReceived); //NVM
                    if (result == 1)
                    {
                        DisplayMessageLine("ERROR -- 7816接口初始化 失败");
                        return 1;
                    }

                    addr = buf_nAddr.ToString("X6");
                    DisplayMessageLine("正在读取地址：  " + addr);
                    sendbuf = "0004" + addr.Substring(2, 4) + "00";
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (result == 1)
                    {
                        DisplayMessageLine("读取失败！");
                        return 1;
                    }
                    else
                        buff = strToHexByte(strReceived);
                    for (j = 0; j < 0x100; j++) //第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        nAddr = buf_nAddr + j - nAddr_base;
                        Readfilebuf[nAddr] = buff[j + 1];
                        if (chkReadVerify.Checked)
                        {
                            if (nAddr > nAddr_end)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[nAddr] != Progfilebuf[nAddr])
                            {
                                DisplayMessageLine("读EE校验错误" + "\t" + "错误地址: 0x" + nAddr.ToString("X6"));
                                display = "读出内容: 0x" + Readfilebuf[nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                        else
                        {
                            if (nAddr > nAddr_end)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            Progfilebuf[nAddr] = Readfilebuf[nAddr];
                        }
                    }
                    ProgEE_progressBar.Value = (int)(ProgNumi + 1);
                }
            }

            if (ProgOrReadEE_flag == 1)
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            return 0;
        }



        public virtual int ChipEraseInCT350(uint nProgStartAddr)
        {
            uint j;
            string strReceived, sendbuf;
            byte[] buff;
            buff = new byte[0x103];
            int result;


            // 配置成全擦模式——请确认已经启动Initial ctrl模块
            FM12XX_Card.TransceiveCT("0002040000", "02", out strReceived);
            if (strReceived != "9000")
            {
                display = "全擦模式配置出错，返回：" + strReceived;
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                DisplayMessageLine("全擦模式配置成功");

                // 启动一个block——128bytes的编程操作来完成全擦
                buff[0] = 0x00;
                buff[1] = 0x00;			//INS = 0x00,WRITE EEPROM命令
                buff[2] = (byte)(nProgStartAddr >> 0x10);	//地址高位
                buff[3] = (byte)(nProgStartAddr >> 0x08);	//地址低位
                buff[4] = (byte)(nProgStartAddr);

                sendbuf = byteToHexStr(5, buff);
                result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                if ((result != 0x00) || (buff[0] != 0x00))
                {//启动编程出错
                    strReceived = "全擦时，指令0x00出错！";
                    DisplayMessageLine(strReceived);
                    return 1;
                }
                for (j = 0; j < 0x80; j++)
                {
                    buff[j] = Progfilebuf[j];
                }
                sendbuf = byteToHexStr(0x80, buff);
                result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                if (result != 0x00)
                {//编程出错
                    strReceived = "全擦时，写128bytes数据出错！";
                    DisplayMessageLine(strReceived);
                    return 1;
                }
                DisplayMessageLine("全擦结束！地址：" + nProgStartAddr.ToString("X6"));
                return 0;
            }
        }

        public virtual int SwitchBaudRateCT350(out string strReceived)
        {
            string strRegTDA8007;
            int result;
            byte bRegTDA8007;

            switch (comboBox_CTBaud.Text)
            {
            case "57600":
                result = FM12XX_Card.TransceiveCT("0001380000", "02", out strReceived);//DI/FI = 12/2 = 6
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到31分频
                result = FM12XX_Card.Set_TDA8007_reg("02", "02", out strReceived);
                break;
            case "115200":
                result = FM12XX_Card.TransceiveCT("0001950000", "02", out strReceived);//DI/FI = 16*372/512
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到32分频
                FM12XX_Card.Read_TDA8007_reg("03", out strReceived, out bRegTDA8007);
                bRegTDA8007 |= 0x01;
                strRegTDA8007 = bRegTDA8007.ToString("X2");
                result = FM12XX_Card.Set_TDA8007_reg("03", strRegTDA8007, out strReceived);
                result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);
                break;
            default:
                result = FM12XX_Card.TransceiveCT("0001380000", "02", out strReceived);//切换到31分频
                if (result == 1)
                {
                    display = "7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到31分频
                result = FM12XX_Card.Set_TDA8007_reg("02", "02", out strReceived);
                break;
            }
            return 0;
        }

        public virtual int RestartProg_CT350(out string strReceived)
        {
            int result;

            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.Cold_Reset("02", out strReceived);
            if (strReceived == "Error")
            {
                display = "Cold Reset失败 ";
                DisplayMessageLine(display);
            }
            Delay(1);
            if (checkBox_CTHighBaud.Checked)
            {
                result = SwitchBaudRateCT350(out strReceived);
                if (result == 1)    //config baud rate error
                    return 1;
                display = "切换到" + comboBox_CTBaud.Text + "波特率...";
                DisplayMessageLine(display);
            }
            //配置芯片擦写时间　for FM349/350
            result = FM12XX_Card.TransceiveCT("0003" + textBox_tprog.Text + textBox_terase.Text + "00", "02", out strReceived);
            if (strReceived != "9000")
            {
                display = "ERROR：7816接口配置nvm擦写时间 失败";
                DisplayMessageLine(display);
                return 1;
            }
            else
                DisplayMessageLine("INFO：FM350擦写时间配置成 Tprog " + label_tprog_Calc.Text + "us, Terase " + label_terase_Calc.Text + "ms");

            //配置（编程+校验）模式
            FM12XX_Card.TransceiveCT("0002090000", "02", out strReceived);
            if (strReceived != "9000")
            {
                display = "ERROR：编程模式配置出错，返回：" + strReceived;
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                DisplayMessageLine("INFO：配置成（编程+校验）模式成功...");
            }

            return 0;
        }

        public virtual int ProgOrReadEEInCT350(uint nProgStartBlock, uint nProgEndBlock, uint nProgOffsetAddr)
        {
            /*
                        uint i, j, ProgNumi, nProgSum, buf_nAddr, temp, Ext_i, ProgStartBlock_temp, nAddr, nAddr_base, buffer_start_addr;
                        int Ext_addr_change, ExtOpt, prog_mem_type, EEmax, prog_extand_mode, LIBsize, cmp1, cmp2, cwd14;
                        byte[] buff;
                        string sendbuf, STR, auth_key;
                        int write_length;
                        strReceived = "";
                        nAddr = 0;
                        buf_nAddr = 0;
                        buff = new byte[0x103];
                        string DestAddr;
                        uint nTemp3 = 0;
                        byte[] destaddr;
                        uint nDestBlock = 0;
            */
            uint i, j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base, nAddr_liboffset = 0;
            string strReceived, auth_key, sendbuf;
            int result, progonly_flag, err9800count, lib_start_flag = 0;
            byte[] buff;
            buff = new byte[0x103];
            int write_length;

            err9800count = 0;
            progonly_flag = 0;
            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            if (checkBox_SeperateLibProg.Checked == true)
            {
                if ((FullProgFile350_flag == 1) && (HasLib350_flag == 1))
                {
                    //DisplayMessageLine((nProgEndBlock).ToString("X6"));
                    //DisplayMessageLine((LibSizeFM350/128).ToString("X6"));
                    nAddr_liboffset = 0x400000 - (0x60000 - LibSizeFM350);
                    //DisplayMessageLine("Lib offset is" + (nAddr_liboffset / 1024) + "K");
                    DisplayMessageLine("INFO：配置lib区大小为" + (LibSizeFM350 / 1024) + "K");
                }
                else
                {
                    DisplayMessageLine("INFO：开始执行不带Lib的常规编程");
                }
            }

            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.Cold_Reset("02", out strReceived);
            if (strReceived == "Error")
            {
                display = "ERROR：Cold Reset失败 ";
                DisplayMessageLine(display);
            }
            else
            {
                DisplayMessageLine("ColdReset ATR:    \t\t" + strReceived);
            }
            Delay(1);
            if (checkBox_CTHighBaud.Checked)
            {
                result = SwitchBaudRateCT350(out strReceived);
                if (result == 1)    //config baud rate error
                    return 1;
                display = "切换到" + comboBox_CTBaud.Text + "波特率...";
                DisplayMessageLine(display);
            }
            else
            {
                result = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);//切换到32分频
                if (result == 1)
                {
                    display = "ERROR：7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
            }

            //INITIAL EEPROM命令               
            if (Auth_checkBox.Checked)
            {
                auth_key = key_richTextBox.Text.Replace(" ", "");
                if ((auth_key == "") || (auth_key.Length != 32))
                {
                    display = "WARN：认证的密钥长度不对！！！   \t\t";
                    DisplayMessageLine(display);
                    return 1;
                }
                FM12XX_Card.TransceiveCT("0055000010", "02", out strReceived);//CNN 添加认证，使认证时也能配置控制字
                int k = FM12XX_Card.TransceiveCT(auth_key, "02", out strReceived);//LOAD密钥
                if (k != 0)
                {
                    display = "ERROR：认证:   \t\t" + "错误！！";
                    DisplayMessageLine(display);
                    return 1;
                }
                display = "认证:   \t\t" + "通过";
                DisplayMessageLine(display);
            }

            if (DATA_EEtype_radiobutton.Checked == true)
            {
                nAddr_base = 0xB00000 + nProgOffsetAddr;
            }
            else if (RAMtype_radiobutton.Checked == true)
            {
                nAddr_base = 0x800000 + nProgOffsetAddr;
            }
            else
            {
                nAddr_base = nProgOffsetAddr;
            }

            write_length = 0x80; // 写操作长度固定为0x80
            if ((ProgOrReadEE_flag == 0) && (RAMtype_radiobutton.Checked == false))
            {
                //配置芯片擦写时间　for FM349/350
                result = FM12XX_Card.TransceiveCT("0003" + textBox_tprog.Text + textBox_terase.Text + "00", "02", out strReceived);
                if (strReceived != "9000")
                {
                    display = "ERROR：7816接口配置nvm擦写时间 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                else
                    DisplayMessageLine("INFO：FM350擦写时间配置成 Tprog " + label_tprog_Calc.Text + "us, Terase " + label_terase_Calc.Text + "ms");

                if (checkBox_350ChipErase.Checked == true)
                {
                    //全擦操作
                    result = ChipEraseInCT350(nAddr_base);
                    if (result == 1)
                    {
                        display = "ERROR：全擦失败，结束编程！";
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        //配置（编程+校验）模式
                        FM12XX_Card.TransceiveCT("0002090000", "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            display = "ERROR：编程模式配置出错，返回：" + strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }
                        else
                        {
                            DisplayMessageLine("INFO：配置成（编程+校验）模式成功");
                        }
                        DisplayMessageLine("INFO：全擦成功，开始写入数据...");
                    }
                }
                else//不带全擦的操作
                {
                    if (PROG_EEtype_radiobutton.Checked == false)       //数据NVM页大小为128bytes和349一致
                    {
                        //配置（带片擦的编程+校验）模式
                        FM12XX_Card.TransceiveCT("00020B0000", "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            display = "ERROR：编程模式配置出错，返回：" + strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }
                        else
                        {
                            DisplayMessageLine("INFO：配置成（带页擦的编程+校验）模式成功");
                        }
                    }
                    progonly_flag = 0;
                }
            }
            //DEBUG cjn
            //DisplayMessageLine(nProgStartBlock.ToString("X6"));
            //DisplayMessageLine(nProgEndBlock.ToString("X6"));

            if (ProgOrReadEE_flag == 0)//编程
            {
                for (ProgNumi = nProgStartBlock; ProgNumi < nProgEndBlock + 1; ProgNumi++)
                {
                    //config prog mode aligning 512byte boundary FOR PROG NVM ONLY --CJN20151203
                    if (checkBox_350ChipErase.Checked == false)
                    {
                        if (PROG_EEtype_radiobutton.Checked == true)
                        {
                            if ((ProgNumi & 3) == 0)  //every 512 bytes 1st sector
                            {
                                // 带页擦的编程 + 校验
                                FM12XX_Card.TransceiveCT("00020B0000", "02", out strReceived);
                                if (strReceived != "9000")
                                {
                                    display = "ERROR：编程模式配置（带页擦的编程 + 校验）出错，返回：" + strReceived;
                                    DisplayMessageLine(display);
                                    return 1;
                                }
                                progonly_flag = 0;
                            }
                            else if ((ProgNumi & 3) == 1)       //2nd sector
                            {
                                // 编程 + 校验
                                FM12XX_Card.TransceiveCT("0002090000", "02", out strReceived);
                                if (strReceived != "9000")
                                {
                                    display = "ERROR：编程模式配置（编程 + 校验）出错，返回：" + strReceived;
                                    DisplayMessageLine(display);
                                    return 1;
                                }
                                progonly_flag = 1;
                            }
                            else
                            //3rd and 4th sector
                            {
                                if (progonly_flag == 0)
                                {
                                    // 编程 + 校验
                                    FM12XX_Card.TransceiveCT("0002090000", "02", out strReceived);
                                    if (strReceived != "9000")
                                    {
                                        display = "ERROR：编程模式配置出错（编程 + 校验），返回：" + strReceived;
                                        DisplayMessageLine(display);
                                        return 1;
                                    }
                                    progonly_flag = 1;
                                }
                            }
                        }
                    }
                    buf_nAddr = ProgNumi * (uint)write_length;
                    nAddr = nAddr_base + buf_nAddr;

                    if (lib_start_flag == 1)        //normal prog complete
                    {
                        lib_start_flag = 0;
                        if (nAddr_liboffset > 0)
                        {
                            lib_start_flag = 0;
                            nAddr_base += nAddr_liboffset;
                            nAddr += nAddr_liboffset;
                            DisplayMessageLine("INFO：启动编程lib区：" + nAddr.ToString("X6") + "...");
                            //result = RestartProg_CT350(out strReceived);
                            //if (result == 1)
                            //{
                            //    DisplayMessageLine("ERROR：初始化失败！");
                            //    return 1;
                            //}
                        }
                    }


                    buff[0] = 0x00;
                    buff[1] = 0x00;			//INS = 0x00,WRITE EEPROM命令
                    buff[2] = (byte)(nAddr >> 0x10);	//地址高位
                    buff[3] = (byte)(nAddr >> 0x08);	//地址低位
                    buff[4] = (byte)(nAddr);

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if ((result != 0x00) || (buff[0] != 0x00))
                    {//启动编程出错
                        strReceived = "ERROR：启动编程指令0x00出错，地址：" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < 0x80; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                        
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");
                        //display = "正在编程地址：" + (ProgNumi*128).ToString("X6");
                        DisplayMessageLine(display);
                    }
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (result != 0x00)
                    {//编程出错
                        strReceived = "ERROR：写128bytes数据出错，地址：" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    if (strReceived != "9000")
                    {
                        if (checkBox_ignore9800.Checked == true)
                        {
                            display = "页：" + nAddr.ToString("X6") + "编程校验出错，结束后请单独读出确认！！";
                            err9800count++;
                            if (err9800count > 50)
                            {
                                DisplayMessageLine("ERROR：过多校验错误，编程强制终止!");
                                return 1;
                            }
                            DisplayMessageLine(display);
                        }
                        else
                        {
                            display = "返回：" + strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }
                    }
                    if (nAddr_liboffset > 0)
                    {
                        if (ProgNumi == nProgEndBlock - (LibSizeFM350/128))
                        {
                            DisplayMessageLine("INFO：Normal段结束：" + (nAddr + 127).ToString("X6"));
                            lib_start_flag = 1;
                        }
                    }
                    ProgEE_progressBar.Value = (int)ProgNumi + 1 - (int)nProgStartBlock;
                }
            }
            else//读取EE操作
            {
                for (ProgNumi = nProgStartBlock; ProgNumi < nProgEndBlock + 1; ProgNumi++)
                {
                    //读取操作                      
                    nAddr = nAddr_base + ProgNumi * (uint)write_length;
                    buff[0] = 0x00;
                    buff[1] = 0x0D;
                    buff[2] = (byte)(nAddr >> 0x10);
                    buff[3] = 0x00;
                    buff[4] = 0x00;
                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (strReceived == "Error")
                    {
                        display = "读取数据0D错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }

                    buff[0] = 0x00;
                    buff[1] = 0x04;			//INS = 0x04,READ EEPROM命令
                    buff[2] = (byte)(nAddr >> 0x08);	//地址高位
                    buff[3] = (byte)(nAddr);	//地址低位
                    buff[4] = 0x80;			//读字节数Lc=00表示读取256个字节 修改为0x80

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < write_length; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        Readfilebuf[(ProgNumi - nProgStartBlock) * write_length + j] = buff[j + 1];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < write_length; j++)
                        {
                            buf_nAddr = ProgNumi * (uint)write_length + j;
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[(ProgNumi - nProgStartBlock)* write_length + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读FLASH校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < write_length; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        buf_nAddr = ProgNumi * (uint)write_length + j;
                        if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("读取完毕");
                            break;
                        }
                        Progfilebuf[buf_nAddr] = Readfilebuf[(ProgNumi - nProgStartBlock) * (uint)write_length + j];
                    }

                    ProgEE_progressBar.Value = (int)ProgNumi + 1 - (int)nProgStartBlock;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                    }
                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            else
            {
                display = "共有" + err9800count.ToString("D2") + "处编程校验错误（9800）";
                DisplayMessageLine(display);
            }

            result = FM12XX_Card.TransceiveCT("0009530000", "02", out strReceived);//切换到372分频
            if (result == 1)
            {
                display = "修改传输波特率失败";
                DisplayMessageLine(display);
                return 1;
            }
            result = FM12XX_Card.Set_TDA8007_reg("02", "0C", out strReceived);
            if (result == 1)
            {
                display = "修改传输波特率失败";
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                display = "修改传输波特率为9600";
                DisplayMessageLine(display);
            }


            return 0;
        }


        public virtual int ProgOrReadEEInCT349(uint nProgStartBlock, uint nProgEndBlock, uint nProgOffsetAddr)
        {
/*
            uint i, j, ProgNumi, nProgSum, buf_nAddr, temp, Ext_i, ProgStartBlock_temp, nAddr, nAddr_base, buffer_start_addr;
            int Ext_addr_change, ExtOpt, prog_mem_type, EEmax, prog_extand_mode, LIBsize, cmp1, cmp2, cwd14;
            byte[] buff;
            string sendbuf, STR, auth_key;
            int write_length;
            strReceived = "";
            nAddr = 0;
            buf_nAddr = 0;
            buff = new byte[0x103];
            string DestAddr;
            uint nTemp3 = 0;
            byte[] destaddr;
            uint nDestBlock = 0;
*/
            uint i, j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base;
            string strReceived, auth_key, sendbuf;
            int result;
            byte[] buff;
            buff = new byte[0x103];
            int write_length;
            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.Cold_Reset("02", out strReceived);
            if (strReceived == "Error")
            {
                display = "Cold Reset失败 ";
                DisplayMessageLine(display);
            }
            else
            {
                DisplayMessageLine("Cold ATR:    \t\t" + strReceived);
            }
            Delay(1);
            if (checkBox_CTHighBaud.Checked)
            {
 //               result = FM12XX_Card.TransceiveCT("0001380000", "02", out strReceived);//切换到31分频
                //初始化          
                 result = FM12XX_Card.TransceiveCT("0001950000", "02", out strReceived);//切换到32分频
                if (result == 1)
                {
                    display = "ERROR：7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                //切换到32分频
                result = FM12XX_Card.Set_TDA8007_reg("03", "21", out strReceived);
                result = FM12XX_Card.Set_TDA8007_reg("02", "01", out strReceived);

                //切换到31分频
 //               result = FM12XX_Card.Set_TDA8007_reg("02", "02", out strReceived);

                DisplayMessageLine("切换到115200波特率");
                //DisplayMessageLine("切换到57600波特率");
            }
            else
            {
                result = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);//切换到32分频
                if (result == 1)
                {
                    display = "ERROR：7816接口初始化 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
            }

            //INITIAL EEPROM命令               
            if (Auth_checkBox.Checked)
            {
                auth_key = key_richTextBox.Text.Replace(" ", "");
                if ((auth_key == "") || (auth_key.Length != 32))
                {
                    display = "认证的密钥长度不对！！！   \t\t";
                    DisplayMessageLine(display);
                    return 1;
                }
                FM12XX_Card.TransceiveCT("0055000010", "02", out strReceived);//CNN 添加认证，使认证时也能配置控制字
                int k = FM12XX_Card.TransceiveCT(auth_key, "02", out strReceived);//LOAD密钥
                if (k != 0)
                {
                    display = "ERROR：认证:   \t\t" + "错误！！";
                    DisplayMessageLine(display);
                    return 1;
                }
                display = "认证:   \t\t" + "通过";
                DisplayMessageLine(display);
            }

            if (DATA_EEtype_radiobutton.Checked == true)
            {
                nAddr_base = 0xB00000 + nProgOffsetAddr;
            }
            else if (RAMtype_radiobutton.Checked == true)
            {
                nAddr_base = 0x800000 + nProgOffsetAddr;
            }
            else
            {
                nAddr_base = nProgOffsetAddr;
            }

            if ((ProgOrReadEE_flag == 0) && (RAMtype_radiobutton.Checked == false))
            {
                //配置芯片擦写时间　for FM349/350
                result = FM12XX_Card.TransceiveCT("0003" + textBox_tprog.Text + textBox_terase.Text + "00", "02", out strReceived);
                if (strReceived != "9000")
                {
                    display = "ERROR：7816接口配置nvm擦写时间 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                else
                    DisplayMessageLine("INFO：FM349擦写时间配置成 Tprog " + label_tprog_Calc.Text + "us, Terase " + label_terase_Calc.Text + "ms");


                write_length = 0x80; // 写操作长度0x80
                // 带片擦的编程模式 + 校验
                FM12XX_Card.TransceiveCT("00020B0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    display = "编程模式配置出错，返回：" + strReceived;
                    DisplayMessageLine(display);
                    return 1;
                }
            }
            else
            {
                write_length = 0x80;
    //            write_length = 0x100; // 读操作长度0x100 
   //             nProgSum = (nProgSum + 1) / 2;
   //             nProgStartBlock = nProgStartBlock / 2;
   //             ProgEE_progressBar.Maximum = (int)nProgSum;
            }

            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                if (ProgOrReadEE_flag == 0)//编程
                {
                    buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length;
                    nAddr = nAddr_base + buf_nAddr;

                    buff[0] = 0x00;
                    buff[1] = 0x00;			//INS = 0x00,WRITE EEPROM命令
                    buff[2] = (byte)(nAddr>>0x10);	//地址高位
                    buff[3] = (byte)(nAddr>>0x08);	//地址低位
                    buff[4] = (byte)(nAddr);

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if ((result != 0x00) || (buff[0] != 0x00))
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < 0x80; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (result != 0x00)
                    {//编程出错
                        strReceived = "EE编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    if (strReceived != "9000")
                    {
                        display = "返回："+strReceived;
                        DisplayMessageLine(display);
                        return 1;     
                    }
                    ProgEE_progressBar.Value = (int)ProgNumi+1;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                    }
                }
                else//读取EE操作
                {
                    //读取操作                      
                    nAddr = nAddr_base + (nProgStartBlock + ProgNumi) * (uint)write_length;
                    buff[0] = 0x00;
                    buff[1] = 0x0D;
                    buff[2] = (byte)(nAddr >> 0x10);
                    buff[3] = 0x00;
                    buff[4] = 0x00;
                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (strReceived == "Error")
                    {
                        display = "读取数据0D错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }

                    buff[0] = 0x00;
                    buff[1] = 0x04;			//INS = 0x04,READ EEPROM命令
                    buff[2] = (byte)(nAddr >> 0x08);	//地址高位
                    buff[3] = (byte)(nAddr);	//地址低位
                    buff[4] = 0x80;			//读字节数Lc=00表示读取256个字节 修改为0x80

                    sendbuf = byteToHexStr(5, buff);
                    result = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < write_length; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        Readfilebuf[ProgNumi * write_length + j] = buff[j + 1];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < write_length; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length + j;
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[ProgNumi * write_length + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < write_length; j++)//第一个字节为0X04 从第二个字节开始放入BUFF
                    {
                        buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length + j;
                        if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("读取完毕");
                            break;
                        }
                        Progfilebuf[buf_nAddr] = Readfilebuf[ProgNumi * (uint)write_length + j];
                    }

                    ProgEE_progressBar.Value = (int)ProgNumi+1;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                    }

                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            result = FM12XX_Card.TransceiveCT("0009530000", "02", out strReceived);//切换到372分频
            if (result == 1)
            {
                display = "修改传输波特率失败";
                DisplayMessageLine(display);
                return 1;
            }
            result = FM12XX_Card.Set_TDA8007_reg("02", "0C", out strReceived);
            if (result == 1)
            {
                display = "修改传输波特率失败";
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                display = "修改传输波特率为9600";
                DisplayMessageLine(display);
            }


            return 0;
        }

        public virtual int ChipEraseInCLA350(uint nProgStartBlock)
        {
            uint j;
            string strReceived, sendbuf;
            byte[] buff;
            buff = new byte[0x103];

            // 配置成全擦模式——请确认选卡已经完成
            FM12XX_Card.TransceiveCL("3384", "01", "09", out strReceived);
            if (strReceived != "00000000000000000000000000000000")
            {
                display = "编程模式配置出错，返回：" + strReceived;
                DisplayMessageLine(display);
                return 1;
            }
            else
            {
                display = "全擦模式配置成功";
                DisplayMessageLine(display);

                buff[0] = 0x31;
                buff[1] = (byte)(nProgStartBlock * 128 >> 20);
                buff[1] += 0x80;
                sendbuf = byteToHexStr(2, buff);
                FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "0x31初始化指令错误";
                    DisplayMessageLine(display);
                    return 1;
                }
                buff[0] = 0x32;
                buff[1] = (byte)(nProgStartBlock * 128 >> 12);
                sendbuf = byteToHexStr(2, buff);
                FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "0x32初始化指令错误";
                    DisplayMessageLine(display);
                    return 1;
                }
                // 启动一个block——128bytes的编程操作来完成全擦
                buff[0] = 0xA1;
                buff[1] = (byte)(nProgStartBlock * 128 >> 4);     //addr[11:5]
                sendbuf = byteToHexStr(2, buff);
                FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                if ((strReceived != "0A") || (buff[0] != 0xA1))
                {//启动编程出错
                    strReceived = "全擦时，指令0xA1出错！";
                    display = strReceived;
                    DisplayMessageLine(display);
                    return 1;
                }
                for (j = 0; j < 0x80; j++)
                {
                    buff[j] = Progfilebuf[j];
                }
                sendbuf = byteToHexStr(0x80, buff);
                FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                if (strReceived != "0A")
                {//编程出错
                    strReceived = "全擦时，写128bytes数据出错！";
                    display = strReceived;
                    DisplayMessageLine(display);
                    return 1;
                }
                DisplayMessageLine("全擦结束！");
                return 0;
            }
        }

        public virtual int ProgOrReadEEInCLA350(uint nProgStartBlock, uint nProgEndBlock, uint nProgOffsetAddr)
        {
            uint i, j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base;    //nProgPageNum
            string strReceived, sendbuf;//auth_key,strAddr_base
            int result, progonly_flag = 0;
            byte[] buff, tprog, terase;
            buff = new byte[0x103];
            int write_length, read_length;

            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            write_length = 0x80; // 写操作长度0x80 = 128 bytes
            read_length = 0x10; // 读操作长度0x10 = 16 bytes

            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            //setup communication channel
            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            FM12XX_Card.SetField(1, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            FM12XX_Card.Active(out display);
            if ((strReceived == "Error") || (strReceived == "NO REQA"))
            {
                display = "FM350 CLA选卡失败";
                DisplayMessageLine(display);
            }
            Delay(1);

            if (DATA_EEtype_radiobutton.Checked == true)        //如果勾选了数据NVM
            {
                nAddr_base = 0xB00000 + nProgOffsetAddr;
            }
            else if (RAMtype_radiobutton.Checked == true)       //如果勾选了RAM
            {
                nAddr_base = 0x800000 + nProgOffsetAddr;
            }
            else
            {
                nAddr_base = nProgOffsetAddr;       //写入程序NVM
            }

            if ((ProgOrReadEE_flag == 0) && (RAMtype_radiobutton.Checked == false)) //无论是否512对齐，第一个page都配置成页擦
            {
                //配置芯片擦写时间　for FM349/350
                tprog = strToHexByte(textBox_tprog.Text);
                tprog[0] |= 0x80;
                result = FM12XX_Card.TransceiveCL("34" + tprog[0], "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "ERROR：非接配置nvm编程时间 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                terase = strToHexByte(textBox_terase.Text);
                terase[0] |= 0x80;
                result = FM12XX_Card.TransceiveCL("35" + terase[0], "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "ERROR：非接配置nvm擦除时间 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                DisplayMessageLine("INFO：FM350擦写时间配置成 Tprog " + label_tprog_Calc.Text + "us, Terase " + label_terase_Calc.Text + "ms");

                if (checkBox_350ChipErase.Checked == true)
                {
                    //全擦
                    result = ChipEraseInCLA350(nProgStartBlock);
                    if (result == 1)
                    {
                        display = "ERROR：全擦失败，结束编程";
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        //带校验的编程
                        FM12XX_Card.TransceiveCL("3389", "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "ERROR：编程模式配置出错，返回：" + strReceived;
                            DisplayMessageLine(display);
                            return 1;
                        }
                        else
                        {
                            DisplayMessageLine("INFO：已经配置成（编程+校验）模式");
                        }
                        DisplayMessageLine("全擦成功，开始写入数据...");
                    }
                }
                else
                {
                    //// 页擦+编程 模式
                    //FM12XX_Card.TransceiveCL("3383", "01", "09", out strReceived);
                    //if (strReceived != "00000000000000000000000000000000")
                    //{
                    //    display = "编程模式配置出错，返回：" + strReceived;
                    //    DisplayMessageLine(display);
                    //    return 1;
                    //}
                    //else
                    //{
                    //    display = "编程模式配置成功";
                    //    DisplayMessageLine(display);
                    //}
                    progonly_flag = 0;
                }
            }

            //Programming
            for (ProgNumi = nProgStartBlock; ProgNumi < nProgEndBlock + 1; ProgNumi++)
            {
                if (ProgOrReadEE_flag == 0)     //仅编程，不读出
                {
                    buf_nAddr = ProgNumi * (uint)write_length;
                    nAddr = nAddr_base + buf_nAddr;

                    if (checkBox_350ChipErase.Checked == false)
                    {
                        if ((ProgNumi & 3) == 0)    //every 512 bytes___1st sector
                        {
                            // 带片擦的编程模式
                            FM12XX_Card.TransceiveCL("3383", "01", "09", out strReceived);
                            if (strReceived != "00000000000000000000000000000000")
                            {
                                display = "编程模式配置出错，返回：" + strReceived;
                                DisplayMessageLine(display);
                                return 1;
                            }
                            progonly_flag = 0;
                        }
                        else if ((ProgNumi & 3) == 1)       //2nd sector
                        {
                            // 编程模式only
                            FM12XX_Card.TransceiveCL("3381", "01", "09", out strReceived);
                            if (strReceived != "00000000000000000000000000000000")
                            {
                                display = "编程模式配置出错，返回：" + strReceived;
                                DisplayMessageLine(display);
                                return 1;
                            }
                            progonly_flag = 1;
                        }
                        else         //3rd and 4th sector
                        {
                            if (progonly_flag == 0)
                            {
                                // 编程模式only
                                FM12XX_Card.TransceiveCL("3381", "01", "09", out strReceived);
                                if (strReceived != "00000000000000000000000000000000")
                                {
                                    display = "编程模式配置出错，返回：" + strReceived;
                                    DisplayMessageLine(display);
                                    return 1;
                                }
                                progonly_flag = 1;
                            }
                        }
                    }
                    

                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                    }

                    // 一个block——128bytes的编程操作
                    buff[0] = 0xA1;
                    buff[1] = (byte)(nAddr >> 4);     //addr[11:5]
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if ((strReceived != "0A") || (buff[0] != 0xA1))
                    {//启动编程出错
                        strReceived = "启动编程指令0xA1出错，地址：" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < 0x80; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if (strReceived != "0A")
                    {//编程出错
                        strReceived = "写128bytes数据出错，地址：" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    ProgEE_progressBar.Value = (int)ProgNumi + 1 - (int)nProgStartBlock;

                }
                else//读取FLASH操作
                {
                    //读取操作                      
                    nAddr = nAddr_base + ProgNumi * (uint)read_length;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                    }
                    //单次读操作
                    buff[0] = 0x30;
                    buff[1] = (byte)(nAddr >> 0x04);
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        Readfilebuf[(ProgNumi - nProgStartBlock) * read_length + j] = buff[j];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < read_length; j++)
                        {
                            buf_nAddr = ProgNumi * (uint)read_length + j;
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[(ProgNumi - nProgStartBlock) * read_length + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读FLASH校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        buf_nAddr = ProgNumi * (uint)read_length + j;
                        if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("FM350读取FLASH完毕");
                            break;
                        }
                        Progfilebuf[buf_nAddr] = Readfilebuf[(ProgNumi - nProgStartBlock) * (uint)read_length + j];
                    }
                    //display = "当前写入文件的源地址：" + nAddr.ToString("X6");
                    //DisplayMessageLine(display);
                    ProgEE_progressBar.Value = (int)ProgNumi + 1 - (int)nProgStartBlock;

                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            return 0;
        }

        public virtual int ProgOrReadEEInCLA349(uint nProgStartBlock, uint nProgEndBlock, uint nProgOffsetAddr)
        {
            uint i, j, ProgNumi, nProgSum, buf_nAddr, nAddr, nAddr_base, nProgPageNum;
            string strReceived, auth_key, sendbuf, strAddr_base;
            int result;
            byte[] buff, tprog, terase;
            buff = new byte[0x103];
            int write_length, read_length;

            //进度条
            nProgSum = nProgEndBlock - nProgStartBlock + 1;
            write_length = 0x80; // 写操作长度0x80 = 128 bytes
            read_length = 0x10; // 读操作长度0x10 = 16 bytes

            strReceived = "";
            ProgEE_progressBar.Maximum = (int)nProgSum;
            ProgEE_progressBar.Value = 0;
            prog_extand_mode0.Checked = false;
            prog_extand_mode1.Checked = false;
            prog_extand_memEE.Checked = false;
            prog_extand_memRAM.Checked = false;
            MEM_EE.Checked = false;
            MEM_ROM.Checked = false;

            //setup communication channel
            FM12XX_Card.Init_TDA8007(out strReceived);
            Delay(1);
            if (strReceived == "Error")
            {
                display = "Init TDA8007失败 ";
                DisplayMessageLine(display);
                return 1;
            }
            FM12XX_Card.SetField(0, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            FM12XX_Card.SetField(1, out display);
            display = "SetField     \t\t\t" + display;
            DisplayMessageLine(display);
            FM12XX_Card.Active(out display);
            if ((strReceived == "Error") || (strReceived == "NO REQA"))
            {
                display = "CLA选卡失败";
                DisplayMessageLine(display);
            }
            Delay(1);

            if (DATA_EEtype_radiobutton.Checked == true)        //如果勾选了数据NVM
            {
                nAddr_base = 0xB00000 + nProgOffsetAddr;
            }
            else if (RAMtype_radiobutton.Checked == true)       //如果勾选了RAM
            {
                nAddr_base = 0x800000 + nProgOffsetAddr;
            }
            else
            {
                nAddr_base = nProgOffsetAddr;       //写入程序NVM
            }

            if ((ProgOrReadEE_flag == 0) && (RAMtype_radiobutton.Checked == false))
            {
                //配置芯片擦写时间　for FM349/350
                tprog = strToHexByte(textBox_tprog.Text);
                tprog[0] |= 0x80;
                result = FM12XX_Card.TransceiveCL("34" + tprog[0], "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "ERROR：非接配置nvm编程时间 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                terase = strToHexByte(textBox_terase.Text);
                terase[0] |= 0x80;
                result = FM12XX_Card.TransceiveCL("35" + terase[0], "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "ERROR：非接配置nvm擦除时间 失败";
                    DisplayMessageLine(display);
                    return 1;
                }
                DisplayMessageLine("INFO：FM349擦写时间配置成 Tprog " + label_tprog_Calc.Text + "us, Terase " + label_terase_Calc.Text + "ms");

                // 带片擦的编程模式
                FM12XX_Card.TransceiveCL("3383", "01", "09", out strReceived);
                if (strReceived != "00000000000000000000000000000000")
                {
                    display = "编程模式配置出错，返回：" + strReceived;
                    DisplayMessageLine(display);
                    return 1;
                }
                else
                {
                    display = "编程模式配置成功";
                    DisplayMessageLine(display);
                }
            }

            //Programming
            for (ProgNumi = 0; ProgNumi < nProgSum; ProgNumi++)
            {
                if (ProgOrReadEE_flag == 0)     //仅编程，不读出
                {
                    buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)write_length;
                    nAddr = nAddr_base + buf_nAddr;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在编程地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                    }
                    // 一个block——128bytes的编程操作
                    buff[0] = 0xA1;
                    buff[1] = (byte)(nAddr >> 4);     //addr[11:5]
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if ((strReceived != "0A") || (buff[0] != 0xA1))
                    {//启动编程出错
                        strReceived = "EE启动编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    for (j = 0; j < 0x80; j++)
                    {
                        buff[j] = Progfilebuf[buf_nAddr + j];
                    }
                    sendbuf = byteToHexStr(write_length, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "03", "09", out strReceived);
                    if (strReceived != "0A")
                    {//编程出错
                        strReceived = "EE编程出错" + nAddr.ToString("X6");
                        display = strReceived;
                        DisplayMessageLine(display);
                        return 1;
                    }
                    ProgEE_progressBar.Value = (int)ProgNumi + 1;

                }
                else//读取EE操作
                {
                    //读取操作                      
                    nAddr = nAddr_base + (nProgStartBlock + ProgNumi) * (uint)read_length;
                    if (nAddr % 0x1000 == 0)
                    {
                        display = "正在读取地址：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);

                        buff[0] = 0x31;
                        buff[1] = (byte)(nAddr >> 20);
                        buff[1] += 0x80;
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x31初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                        buff[0] = 0x32;
                        buff[1] = (byte)(nAddr >> 12);
                        sendbuf = byteToHexStr(2, buff);
                        FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            display = "0x32初始化指令错误";
                            DisplayMessageLine(display);
                            return 1;
                        }
                    }
                    //单次读操作
                    buff[0] = 0x30;
                    buff[1] = (byte)(nAddr >> 0x04);
                    sendbuf = byteToHexStr(2, buff);
                    FM12XX_Card.TransceiveCL(sendbuf, "01", "09", out strReceived);

                    if (strReceived == "Error")
                    {
                        display = "读取数据错误：" + nAddr.ToString("X6");
                        DisplayMessageLine(display);
                        return 1;
                    }
                    else
                    {
                        buff = strToHexByte(strReceived);
                    }

                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        Readfilebuf[ProgNumi * read_length + j] = buff[j];
                    }
                    if (chkReadVerify.Checked)
                    {
                        for (j = 0; j < read_length; j++)
                        {
                            buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                            if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                            {
                                DisplayMessageLine("读取完毕");
                                break;
                            }
                            if (Readfilebuf[ProgNumi * read_length + j] != Progfilebuf[buf_nAddr])
                            {
                                display = "读EE校验错误" + "\t" + "错误地址: 0x" + buf_nAddr.ToString("X6");
                                DisplayMessageLine(display);
                                display = "读出内容: 0x" + Readfilebuf[buf_nAddr].ToString("X2") + "\t" + "期望内容: 0x" + Progfilebuf[buf_nAddr].ToString("X2");
                                DisplayMessageLine(display);
                                return 1;
                            }
                        }
                    }
                    for (j = 0; j < read_length; j++)//第一个字节开始放入BUFF
                    {
                        buf_nAddr = (nProgStartBlock + ProgNumi) * (uint)read_length + j;
                        if (buf_nAddr > (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1)
                        {
                            DisplayMessageLine("读取完毕");
                            break;
                        }
                        Progfilebuf[buf_nAddr] = Readfilebuf[ProgNumi * (uint)read_length + j];
                    }
                    //display = "当前写入文件的源地址：" + nAddr.ToString("X6");
                    //DisplayMessageLine(display);
                    ProgEE_progressBar.Value = (int)ProgNumi + 1;

                }
            }
            if (ProgOrReadEE_flag == 1)
            {
                //ProgFileLenth = (int)(nProgSum)*0x100;
                ProgFileLenth = (strToHexByte(ProgEEEndAddr.Text)[2] + strToHexByte(ProgEEEndAddr.Text)[1] * 256 + strToHexByte(ProgEEEndAddr.Text)[0] * 65536) + 1;
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            }
            return 0;
        }

        private void OpenFile_Button_Click(object sender, EventArgs e)
        {
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            int i;
            int patch_size=0;
            int addr = 0;
            int hex_addr_change;
            byte[] bytedata;
            int startaddr;
            //int Ext_addr_change, ExtOpt, prog_mem_type, EEmax;
            //            StreamReader read = new StreamReader(openFileDialog_Scr.FileName);

            ProgFileLenth = 0;
            open_flag = 1;
            hex_addr_change = 0;
            openFileDialog_Prog = null;
            openFileDialog_Prog = new OpenFileDialog();

            openFileDialog_Prog.Reset();
            openFileDialog_Scr.Reset();
            openFileDialog_Prog.InitialDirectory = HexDirectory.Substring(0, HexDirectory.LastIndexOf("\\"));// ;
            openFileDialog_Prog.RestoreDirectory = true;
            if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)//CNN修改 
            {
                dataGridView_flag = 0;
                progEEdataGridView.Rows.Clear();
            }
            ProgEEStartAddr.Text = "000000";
            openFileDialog_Prog.Filter = "hex files (*.hex;*.bin)|*.bin;*.hex|All files (*.*)|*.*";

            if (radioButton_0xB9.Checked)
            {
                for (i = 0; i < ProgFileMaxLen; i++)
                {
                    Progfilebuf[i] = 0xB9;		//缓冲区初始化为0xB9   CNN 应系统部要求 20140804
                }
            }
            else if (radioButton_0xFF.Checked)
            {
                for (i = 0; i < ProgFileMaxLen; i++)
                {
                    Progfilebuf[i] = 0xFF;		//缓冲区初始化为0xFF   YSL 应系统部要求 20150720
                }
            }
            else
            {
                for (i = 0; i < ProgFileMaxLen; i++)
                {
                    Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                }
            }

            if (openFileDialog_Prog.ShowDialog() == DialogResult.OK)
            {
                HexDirectory = openFileDialog_Prog.FileName;
                tempStr =HexDirectory.Substring(HexDirectory.Length-4,4);      //取文件后缀
                tempStr = tempStr.ToUpper();
                //修改为了支持BIN文件格式,by yanshouli
                if (/*(openFileDialog_Prog.FilterIndex == 0x01) ||*/ (tempStr == ".HEX"))       //HEX文件格式   
                {
                    //temp = HexDirectory.LastIndexOf("\\");
                    //HexDirectory = HexDirectory.Substring(0, temp);
                    RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
                    hexfiledata = new byte[RawCode.Length];
                    bin_data = new byte[RawCode.Length];

                    StartEEAddr = ProgEEStartAddr.Text;
                    EndEEAddr = ProgEEEndAddr.Text;
                    for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
                    {
                        StartEEAddr = "0" + StartEEAddr;
                    }
                    for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
                    {
                        EndEEAddr = "0" + EndEEAddr;
                    }
                    ProgEEStartAddr.Text = StartEEAddr;
                    ProgEEEndAddr.Text = EndEEAddr;

                    if (radioButton_0xB9.Checked)
                    {
                        for (i = 0; i < ProgFileMaxLen; i++)
                        {
                            Progfilebuf[i] = 0xB9;		//缓冲区初始化为0xB9   CNN 应系统部要求 20140804
                        }
                    }
                    else
                    {
                        for (i = 0; i < ProgFileMaxLen; i++)
                        {
                            Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                        }
                    }
                    ProgOrReadEE_FF_flag = 0;
                    ProgOrReadEE_80_flag = 0;
                    ProgOrReadEE_81_flag = 0;
                    ProgOrReadEE_82_flag = 0;
                    ProgOrReadEE_83_flag = 0;
                    ProgOrReadEE_84_flag = 0;
                    ProgOrReadEE_85_flag = 0;
                    ProgOrReadEE_86_flag = 0;
                    ProgOrReadEE_90_flag = 0;

                    for (i = 0; i < RawCode.Length; i++)
                    {
                        szHex = "";
                        tempStr = RawCode[i];
                        if (tempStr == "")
                        {
                            continue;
                        }
                        if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                        {

                            if (tempStr.Substring(1, 8) == "00000001")//数据结束
                            {
                                break;
                            }
                            if (tempStr.Substring(7, 2) == "02") // 段偏移
                            {
                                hex_addr_change = Convert.ToUInt16(tempStr.Substring(9, 4), 16) << 0x4;
                                continue;
                            }
                            if (tempStr.Substring(7, 2) == "04")//段地址
                            {
                                if ((FM349_radioButton.Checked)||(FM350_radioButton.Checked))
                                {
                                    // remove ram segment
                                    if (tempStr.Substring(11, 2) == "80")//段地址
                                    {
                                        DisplayMessageLine("RAM段已经移除！！！请确认");
                                        break;
                                    }
                                }
                                else if (FM336_radioButton.Checked)
                                {
                                    if (tempStr.Substring(11, 2) == "80")//段地址
                                    {
                                        hex_addr_change = 0x010000;
                                        ProgOrReadEE_80_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "81")//段地址
                                    {
                                        hex_addr_change = 0x020000;
                                        ProgOrReadEE_81_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "82")//段地址
                                    {
                                        hex_addr_change = 0x030000;
                                        ProgOrReadEE_82_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "83")//段地址
                                    {
                                        hex_addr_change = 0x040000;
                                        ProgOrReadEE_83_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "84")//段地址
                                    {
                                        hex_addr_change = 0x050000;
                                        ProgOrReadEE_84_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "FF")//段地址
                                    {
                                        hex_addr_change = 0x000000;
                                        ProgOrReadEE_FF_flag = 1;
                                    }
                                    //打开程序时，lib区数据是放在普通程序后面的，
                                    else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                                    {
                                        hex_addr_change = 0x080000;
                                        ProgOrReadEE_90_flag = 1;
                                    }
                                    else//段后扩展地址
                                    {
                                        ProgOrReadEE_EXT_flag = 1;
                                        if (tempStr.Substring(11, 2) == "85")//段地址
                                        {
                                            hex_addr_change = 0x060000;
                                            ProgOrReadEE_85_flag = 1;
                                        }
                                        else if (tempStr.Substring(11, 2) == "86")//段地址
                                        {
                                            hex_addr_change = 0x070000;
                                            ProgOrReadEE_86_flag = 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tempStr.Substring(11, 2) == "80")//段地址
                                    {
                                        hex_addr_change = 0x010000;
                                        ProgOrReadEE_80_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "81")//段地址
                                    {
                                        hex_addr_change = 0x020000;
                                        ProgOrReadEE_81_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "82")//段地址
                                    {
                                        hex_addr_change = 0x030000;
                                        ProgOrReadEE_82_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "FF")//段地址
                                    {
                                        hex_addr_change = 0x000000;
                                        ProgOrReadEE_FF_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                                    {
                                        hex_addr_change = 0x060000;
                                        ProgOrReadEE_90_flag = 1;
                                    }
                                    else//段后扩展地址
                                    {
                                        ProgOrReadEE_EXT_flag = 1;
                                        if (tempStr.Substring(11, 2) == "83")//段地址
                                        {
                                            hex_addr_change = 0x040000;
                                            ProgOrReadEE_83_flag = 1;
                                        }
                                        else if (tempStr.Substring(11, 2) == "84")//段地址
                                        {
                                            hex_addr_change = 0x050000;
                                            ProgOrReadEE_84_flag = 1;
                                        }
                                        /* else if (tempStr.Substring(11, 2) == "85")//段地址
                                         {
                                             hex_addr_change = 0x060000;                                    
                                         }   */
                                    }
                                }
                                // addrStr = tempStr.Substring(11, 2);
                                // hex_addr_change = (int)(strToHexByte(addrStr)[0] * 65536);
                            }
                            else if (tempStr.Substring(7, 2) == "05")
                            {//05 类型字段处理,解决打开部分HEX文件出错的现象,修改by yanshouli 
                            }
                            else if (tempStr.Substring(7, 2) == "03")
                            { // start segment address record 
                            }
                            else
                            {
                                ProgOrReadEE_FF_flag = 1;
                                datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                                addrStr = tempStr.Substring(3, 4);//记录该行的地址
                                szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                                bytedata = strToHexByte(szHex);
                                addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                                if (addr >= ProgFileLenth)
                                {
                                    ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                                }
                                for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                                {
                                    Progfilebuf[addr + j] = bytedata[j];
                                }
                            }
                            // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                        }
                    }
                    ProgFileLenth = ((ProgFileLenth+3)/4)*4;    //文件大小取整到4的倍数,by yanshouli 
                }
                else if (/*(openFileDialog_Prog.FilterIndex == 0x02)||*/ (tempStr == ".BIN"))       //BIN文件格式
                {   //修改支持BIN文件格式,by yanshouli
                    FileStream s2 = File.OpenRead(openFileDialog_Prog.FileName);
                    ProgFileLenth = (int)s2.Length;
                    s2.Read(Progfilebuf, 0, ProgFileLenth);
                    s2.Close();
                    ProgFileLenth = ((ProgFileLenth + 3) / 4) * 4;
                }
               
                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);

                if (EE_patch_checkBox.Checked)  //cnn 20141022  for FM336patch区正反码校验功能
                {
                    if (textBox1.Text == "")
                    {
                        DisplayMessageLine("请输入正确的补丁大小！！！");
                        return;
                    }
                    patch_size = strToHexByte(textBox1.Text)[0] * 1024;
                    if (patch_size < ProgFileLenth)
                    {
                        DisplayMessageLine("补丁小于程序大小，请输入正确的补丁值！！！");
                        return;
                    }

                    for (i = 0; i < ProgFileLenth; i++)
                        Progfilebuf[patch_size + i] = (byte)(~Progfilebuf[i]);

                    ProgFileLenth = patch_size * 2;
                    ProgEEStartAddr.Text = "000000";
                    ProgEEEndAddr.Text = (ProgFileLenth - 1).ToString("X6");
                    ProgDestAddr.Text = (0x14000 - patch_size * 2).ToString("X6");
                    Prog_Move_Click(null, EventArgs.Empty);
                    ProgEEStartAddr.Text = ProgDestAddr.Text;
                    ProgEEEndAddr.Text = "014000";
                    DATA_EEtype_radiobutton.Checked = true;
                    DisplayMessageLine("提醒：EE类型已选择数据EE");
                    DisplayMessageLine("现程序大小为：" + ProgFileLenth / 1024.000 + "K");
                }
            }
            openFileDialog_Prog.Dispose();
        }

        public virtual void ShowProgFileBuffer(int ProgFileLenth, byte[] data)
        {
            int i, j, len;

            int nProgAddr1, nProgAddr, Addr_temp, nProgAddrOffset;
            int nProgEndTemp;
            int nRowData;
            String strProgData;

            nProgAddr1 = strToHexByte(ProgEEStartAddr.Text)[2] + strToHexByte(ProgEEStartAddr.Text)[1] * 256 + strToHexByte(ProgEEStartAddr.Text)[0] * 65536;
            nProgAddrOffset = strToHexByte(ProgEEOffsetAddr.Text)[2] + strToHexByte(ProgEEOffsetAddr.Text)[1] * 256 + strToHexByte(ProgEEOffsetAddr.Text)[0] * 65536;

            Addr_temp = nProgAddr1;

            if ((ProgFileLenth - Addr_temp) % 16 == 0)
            {
                nRowData = (ProgFileLenth - Addr_temp) / 16;
            }
            else
            {
                nRowData = (ProgFileLenth - Addr_temp) / 16 + 1;
            }
            nRowcount = nRowData;
            for (i = 0; i < nRowData; i++)
            {
                progEEdataGridView.RowCount = nRowData;
                nProgAddr = nProgAddr1 + 0x10 * i + nProgAddrOffset;

                progEEdataGridView["EE_Addr", i].Value = nProgAddr.ToString("X6");
                strProgData = "";
                len = 0x10;
                for (j = 0; j < len; j++)
                {
                    strProgData = strProgData + data[Addr_temp + i * len + j].ToString("X2") + " ";
                }
                progEEdataGridView["EE_Data", i].Value = strProgData;
            }
            //nProgEndTemp = ProgFileLenth + nProgRamStartTemp;  
            nProgEndTemp = ProgFileLenth - 1;
            ProgEEEndAddr.Text = (nProgEndTemp + nProgAddr1 - Addr_temp).ToString("X6");

            if ((RAMtype_radiobutton.Checked == true))
            {
                //nProgEndTemp = ProgFileLenth;
                // ProgEEEndAddr.Text = (nProgEndTemp - 1).ToString("X6");
                if (nProgEndTemp > 0x2000)
                {
                    display = "程序超出RAM范围……";
                    DisplayMessageLine(display);
                    return;
                }
            }

            //根据项目显示程序NVM空间
            if ((FM350_radioButton.Checked == true) &&　(PROG_EEtype_radiobutton.Checked == true))
            {
                //DisplayMessageLine("INFO：FM350程序NVM为" + ProgLen_350 / 1024.000 + "K（" + ProgBnNLen_350 / 1024.000 + "K boot&normal + " + ProgLibLen_350 / 1024.000 + "K lib）");
                if (ProgFileLenth > ProgLen_350)
                {
                    FullProgFile350_flag = 0;
                    HasLib350_flag = 0;
                    display = "ERROR：程序大小超出范围...";
                    DisplayMessageLine(display);
                    return;
                }
                else if (ProgFileLenth < ProgLen_350)
                {
                    FullProgFile350_flag = 0;
                    if (HasLib350_flag == 1)
                        HasLib350_flag = 0;
                    DisplayMessageLine("WARN：程序文件不带Lib段，将执行常规编程...");
                }
                else
                {
                    FullProgFile350_flag = 1;
                    HasLib350_flag = (checkBox_SeperateLibProg.Checked == true) ? 1 : 0;
                }

                display = "原程序大小为：" + ProgFileLenth / 1024.000 + "K" + "/" + ProgLen_350 / 1024.000 + "K";
                DisplayMessageLine(display);
            }
            else
            {
                HasLib350_flag = 0;
                display = "原程序大小为：" + ProgFileLenth / 1024.000 + "K";
                DisplayMessageLine(display);
            }
            
            if (DATA_EEtype_radiobutton.Checked && nProgEndTemp > 0x28000)
            {
                if (flash_isp.Checked == false)
                {
                    display = "程序超出160K数据EE范围……";
                    DisplayMessageLine(display);
                    return;
                }
            }
            /*if (PROG_EEtype_radiobutton.Checked && nProgEndTemp > 0x40000)
            {
                display = "程序出256K超程序EE范围……";
                DisplayMessageLine(display);
                display = "将进行程序扩展，请检查控制字0配置是否符合您的要求……";
                DisplayMessageLine(display);
                //return;
            }    */
            nProgRamStartTemp = 0;
            if (SaveReadbuf.Checked && ProgOrReadEE_flag == 1)
            {
                SaveFileBuffer(ProgEEStartAddr.Text, ProgEEEndAddr.Text, Progfilebuf);
            }
            
        }

        public virtual void CRC16_verify(string SaveStartAddr, string SaveEndAddr, byte[] data, int wcrc, out int result_crc)
        {
            int nProgAddr1, nProgAddr2, DataLength, nRow, btBCC = 0, i, j;
            nProgAddr1 = strToHexByte(SaveStartAddr)[2] + strToHexByte(SaveStartAddr)[1] * 256 + strToHexByte(SaveStartAddr)[0] * 65536;
            nProgAddr2 = strToHexByte(SaveEndAddr)[2] + strToHexByte(SaveEndAddr)[1] * 256 + strToHexByte(SaveEndAddr)[0] * 65536;
            if (nProgAddr2 > nProgAddr1)
            {
                DataLength = nProgAddr2 - nProgAddr1 + 1;
            }
            else
            {
                display = "起止地址错误……";
                DisplayMessageLine(display);
                result_crc = 0;
                return;
            }
            //wcrc = 0x6363;
            for (i = nProgAddr1; i < nProgAddr1 + DataLength; i++)
            {
                wcrc = wcrc ^ data[i];
                for (j = 0; j < 8; j++)
                {
                    if ((wcrc & 0x0001) == 0x0001)
                        wcrc = (wcrc >> 1) ^ 0x8408;
                    else
                        wcrc = wcrc >> 1;
                }
            }
            result_crc = wcrc;
        }

        public virtual void SaveFileBuffer(string SaveStartAddr, string SaveEndAddr, byte[] data)
        {
            string Temp = "", strData;
            string name = "";
            int nProgAddr1, nProgAddr2, SaveLength, nRow, btBCC = 0, printAddr, segmentAddr, segmentBCC;
            int FileBinMode=0;
            StreamWriter Savebuf;
            if (SaveAs_flag == 1)
            {
                //SaveFileDialog类
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = Application.StartupPath + ".\\log";
                //过滤器
                //save.Filter = "hex files (*.hex)|*.hex|All files (*.*)|*.*";
                save.Filter = "hex files (*.hex)|*.hex|bin files(*.bin)|*.bin|All files (*.*)|*.*";
                //显示
                if (save.ShowDialog() == DialogResult.OK)
                {
                    name = save.FileName;
                    if (save.FilterIndex == 0x02)       //BIN文件格式 
                    {
                        FileBinMode = 1;   
                        // File.WriteAllBytes(name,,);
                    }
                    else
                    {

                        FileInfo info = new FileInfo(name);
                        FileStream fs = info.Create();
                        fs.Close();
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                FileStream fs = new FileStream(Application.StartupPath + ".\\log" + ".\\Readbuf.hex", FileMode.Create, FileAccess.Write);//创建写入文件                                     
                fs.Close();
            }
            nProgAddr1 = strToHexByte(SaveStartAddr)[2] + strToHexByte(SaveStartAddr)[1] * 256 + strToHexByte(SaveStartAddr)[0] * 65536;
            nProgAddr2 = strToHexByte(SaveEndAddr)[2] + strToHexByte(SaveEndAddr)[1] * 256 + strToHexByte(SaveEndAddr)[0] * 65536;
            if (nProgAddr2 > nProgAddr1)
            {
                SaveLength = nProgAddr2 - nProgAddr1;
            }
            else
            {
                display = "保存的起止地址错误……";
                DisplayMessageLine(display);
                return;
            }
            if (FileBinMode == 1)
            {

                FileStream fs = File.Create(name);
                for (int i = 0; i < SaveLength+1; i++)
                {
                    fs.WriteByte(data[nProgAddr1 + i]);
                }
                
                fs.Close();
                return;
            }
            if (SaveLength % 16 == 0)
            {
                nRow = (SaveLength) / 16;
            }
            else
            {
                nRow = (SaveLength) / 16 + 1;
            }
            if (FM349_radioButton.Checked)
            {
                segmentAddr = ((nProgAddr1 >> 0x04) & 0xF000);
                segmentBCC = 256 - (0x02 + 0x02 + (segmentAddr / 256) + (segmentAddr % 256)) % 256;
                Temp = ":02000002" + segmentAddr.ToString("X4") + segmentBCC.ToString("X2");
            }
            else
            {
                if (nProgAddr1 < 0x010000)
                {
                    Temp = ":0200000400FFFB";
                }
                else if (nProgAddr1 < 0x020000)
                {
                    Temp = ":0200000400807A";
                }
                else if (nProgAddr1 < 0x030000)
                {
                    Temp = ":02000004008179";
                }
                else if (nProgAddr1 < 0x040000)
                {
                    Temp = ":02000004008278";
                }
                else if (nProgAddr1 < 0x050000)
                {
                    Temp = ":02000004008377";
                }
                else if (nProgAddr1 < 0x060000)
                {
                    Temp = ":02000004008476";
                }
                else if (nProgAddr1 < 0x070000)
                {
                    Temp = ":02000004008575";
                }
                else if (nProgAddr1 < 0x080000)
                {
                    Temp = ":02000004008674";
                }
                else if (nProgAddr1 > 0x07FFFF)
                {
                    Temp = ":0200000400906A";
                }
            }

            if (SaveAs_flag == 1)
            {
                Savebuf = File.AppendText(name);
            }
            else
            {
                Savebuf = File.AppendText(Application.StartupPath + ".\\log" + ".\\Readbuf.hex");
            }
            for (int i = 0; i < nRow; i++)
            {

                printAddr = nProgAddr1 + 0x10 * i;

                btBCC = (0x10 + printAddr / 256) % 256;
                btBCC = (btBCC + printAddr % 256) % 256;
                strData = "";
                for (int j = 0; j < 0x10; j++)
                {
                    strData = strData + data[nProgAddr1 + i * 0x10 + j].ToString("X2") + " ";
                    btBCC = (btBCC + data[nProgAddr1 + i * 0x10 + j]) % 256;
                }
                btBCC = (0x100 - btBCC) % 0x100;
                if (i == 0)
                {
                    Savebuf.WriteLine(Temp);
                }
                else
                {
                    if (FM349_radioButton.Checked)
                    {
                        if ((printAddr & 0xFFFF) == 0)
                        {
                            segmentAddr = ((printAddr >> 0x04) & 0xF000);
                            segmentBCC = 256 - (0x02 + 0x02 + (segmentAddr / 256) + (segmentAddr % 256)) % 256;
                            Temp = ":02000002" + segmentAddr.ToString("X4") + segmentBCC.ToString("X2");
                            Savebuf.WriteLine(Temp);
                        }
                    }
                    else
                    {
                        if (printAddr == 0x000000)
                        {
                            Savebuf.WriteLine(":0200000400FFFB");
                        }
                        else if (printAddr == 0x010000)
                        {
                            Savebuf.WriteLine(":0200000400807A");
                        }
                        else if (printAddr == 0x020000)
                        {
                            Savebuf.WriteLine(":02000004008179");
                        }
                        else if (printAddr == 0x030000)
                        {
                            Savebuf.WriteLine(":02000004008278");
                        }
                        else if (printAddr == 0x040000)
                        {
                            Savebuf.WriteLine(":02000004008377");
                        }
                        else if (printAddr == 0x050000)
                        {
                            Savebuf.WriteLine(":02000004008476");
                        }
                        else if (printAddr == 0x060000)
                        {
                            Savebuf.WriteLine(":02000004008575");
                        }
                        else if (printAddr == 0x070000)
                        {
                            Savebuf.WriteLine(":02000004008674");
                        }
                        else if (printAddr == 0x080000)
                        {
                            Savebuf.WriteLine(":0200000400906A");
                        }
                    }
                }
                Savebuf.WriteLine(":10" + (printAddr % 0x10000).ToString("X4") + "00" + DeleteSpaceString(strData) + btBCC.ToString("X2"));
                if (i == nRow - 1)
                {
                    Savebuf.WriteLine(":00000001FF");
                }
            }
            Savebuf.Close();

            SaveAs_flag = 0;
        }


        private void ReadEEdata_Button_Click(object sender, EventArgs e)
        {
            string StartEEAddr, EndEEAddr, OffsetEEAddr, strReceived;
            uint EEtype, nTemp1, nTemp2, nTemp3, cnt2k = 0, cnt2b = 0;
            byte[] endaddr, startaddr, offsetaddr;
            int nRet = 1;
            uint nProgStartBlock = 0;
            uint nProgEndBlock = 0;
            ProgOrReadEE_flag = 1;//EE读取
            if (PROG_EEtype_radiobutton.Checked == true)
                EEtype = 0;
            else
                EEtype = 1;

            /*if (chkWREncrypt.Checked == true)
                btEncryptOpt = 0;
            else
                btEncryptOpt = 1;*/

            StartEEAddr = ProgEEStartAddr.Text;
            EndEEAddr = ProgEEEndAddr.Text;
            OffsetEEAddr = ProgEEOffsetAddr.Text;
            for (int i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (int i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            for (int i = 0; i < 6 - ProgEEOffsetAddr.Text.Length; i++)
            {
                OffsetEEAddr = "0" + OffsetEEAddr;
            }
            ProgEEStartAddr.Text = StartEEAddr;
            ProgEEEndAddr.Text = EndEEAddr;
            ProgEEOffsetAddr.Text = OffsetEEAddr;
            startaddr = new byte[3];
            endaddr = new byte[3];
            offsetaddr = new byte[3];
            startaddr = strToHexByte(StartEEAddr);
            endaddr = strToHexByte(EndEEAddr);
            offsetaddr = strToHexByte(OffsetEEAddr);
            nTemp1 = (uint)(startaddr[2] + startaddr[1] * 256 + startaddr[0] * 65536);
            nTemp2 = (uint)(endaddr[2] + endaddr[1] * 256 + endaddr[0] * 65536);
            nTemp3 = (uint)(offsetaddr[2] + offsetaddr[1] * 256 + offsetaddr[0] * 65536);
            //A1Program.Checked = false;

            if (CL_Interface.Checked)
            {
                //ct,最小单位16 bytes
                nProgStartBlock = nTemp1 / 16;	//开始页
                nProgEndBlock = nTemp2 / 16;  	//结束页
            }
            else
            {
                if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true)
                {
                    //ct,最小单位64 BYTES
                    nProgStartBlock = nTemp1 / 64;	//开始页
                    nProgEndBlock = nTemp2 / 64;  	//结束页
                }//ct,最小单位128YTES
                else if (FM347_radioButton.Checked == true)
                {
                    //ct,最小单位256 bytes
                    nProgStartBlock = nTemp1 / 0x100;	//开始页
                    cnt2b = (nTemp2 - nTemp1) / 0x100 + 1;  	//结束页    
                    cnt2k = 0;
                }
                else
                {
                    //ct,最小单位128 bytes
                    nProgStartBlock = nTemp1 / 128;	//开始页
                    nProgEndBlock = nTemp2 / 128;  	//结束页
                }
            }
            try
            {

                //Reset17_Click(null, EventArgs.Empty);
                //Init_TDA8007_Click(null, EventArgs.Empty);
                if (CL_Interface.Checked)
                {	//cla编程接口
                    DisplayMessageLine("已选择CLA接口...");
                    if (FM349_radioButton.Checked == true)
                    {
                        DisplayMessageLine("开始读取FM349的NVM...");
                        nRet = ProgOrReadEEInCLA349(nProgStartBlock, nProgEndBlock, nTemp3);
                    }
                    else if (FM350_radioButton.Checked == true)
                    {
                        DisplayMessageLine("开始读取FM350的NVM...");
                        nRet = ProgOrReadEEInCLA350(nProgStartBlock, nProgEndBlock, nTemp3);
                    }
                    else if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true || FM302_radioButton.Checked == true)
                    {
                        nRet = ProgOrReadEEInCLA294(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                    }
                    else
                    {
                        nRet = ProgOrReadEEInCLA(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                    }
                }
                else
                {	//ct编程接口
                    DisplayMessageLine("已选择CT接口...");
                    if (FM349_radioButton.Checked == true)
                    {
                        DisplayMessageLine("开始读取FM349的NVM...");
                        nRet = ProgOrReadEEInCT349(nProgStartBlock, nProgEndBlock, nTemp3);
                    }
                    else if (FM350_radioButton.Checked == true)
                    {
                        DisplayMessageLine("开始读取FM350的NVM...");
                        nRet = ProgOrReadEEInCT350(nProgStartBlock, nProgEndBlock, nTemp3);
                    }
                    else if (FM294_radioButton.Checked == true || FM274_radioButton.Checked == true)
                    {
                        nRet = ProgOrReadEEInCT294(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                    }
                    else if (FM347_radioButton.Checked)
                        nRet = ProgOrReadEEInCT347(nProgStartBlock, cnt2k, cnt2b, nTemp3);
                    else
                    {
                        nRet = ProgOrReadEEInCT(EEtype, nProgStartBlock, nProgEndBlock, out  strReceived);
                    }
                }
                if (nRet == 0)
                {
                    if (chkReadVerify.Checked)
                    {
                        display = "校验正确   \t\t\tSucceeded ";
                        DisplayMessageLine(display);
                    }
                    else
                    {
                        display = "读取结束   \t\t\tSucceeded ";
                        DisplayMessageLine(display);
                    }
                }
                else
                {
                    display = "读取失败!   ";
                    DisplayMessageLine(display);
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }


        private void InitEEdata_Button_Click(object sender, EventArgs e)
        {
            string StartEEAddr, EndEEAddr;
            int i, j, datalenth,k;
            int[] Debug_Buf = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ProgFileLenth = (int)ProgFileMaxLen;
            if (DATA_EEtype_radiobutton.Checked)
            {
                ProgFileLenth = 1024 * 160;
            }
            //            dataGridView_flag = 0;
            progEEdataGridView.Rows.Clear();
            //if (FM302_radioButton.Checked == true)
            //    StartEEAddr = "00A000";
            //else
               StartEEAddr = "000000";
            EndEEAddr = ProgEEEndAddr.Text;
            for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            ProgEEStartAddr.Text = StartEEAddr;
            ProgEEEndAddr.Text = EndEEAddr;
            //EndAddr = strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536;
            
            // 初始化缓冲区根据结束地址变化 hj 2015-03-03
            ProgFileLenth = 1 + strToHexByte(EndEEAddr)[2] + strToHexByte(EndEEAddr)[1] * 256 + strToHexByte(EndEEAddr)[0] * 65536;

            datalenth = ProgFileLenth;

            if (InitData_comboBox.SelectedIndex == 0)
            {
                for (i = 0; i < datalenth; i++)
                {
                    Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                }
            }
            if (InitData_comboBox.SelectedIndex == 1)
            {
                for (i = 0; i < datalenth; i++)
                {
                    Progfilebuf[i] = 0xFF;		//缓冲区初始化为0xFF
                }
            }
            if (InitData_comboBox.SelectedIndex == 2)
            {
                for (i = 0; i < datalenth; i++)
                {
                    Progfilebuf[i] = 0x55;		//缓冲区初始化为0x55
                }
            }
            if (InitData_comboBox.SelectedIndex == 3)
            {
                for (i = 0; i < datalenth; i++)
                {
                    Progfilebuf[i] = 0xAA;		//缓冲区初始化为0xAA
                }
            }
            if (InitData_comboBox.SelectedIndex == 4)
            {
                for (i = 0; i < datalenth; i++)
                {
                    //Progfilebuf[i] = (byte)(i%0x100);	//缓冲区初始化为升序00~FF
                    Progfilebuf[i] = (byte)((i) % 0x100 + (i) / 0x100);	//缓冲区初始化为升序
                }
            }
            if (InitData_comboBox.SelectedIndex == 5)
            {
                for (i = 0; i < datalenth; i++)
                {
                    j = i / 128;
                    //Progfilebuf[i] = (byte)(i%0x100);	//缓冲区初始化为升序00~FF
                    //Progfilebuf[i] = (byte)((0xFF - i) % 0x100 + (0xFF - i) / 0x100);	//缓冲区初始化为升序
                    Progfilebuf[i] = (byte)(j + 1);	//缓冲区初始化为升序00~FF
                }
            }
            ///////////增加初始化伪随机序列----zhujunhao//////////////////
            Debug_Buf[0] = 0x00;
            Debug_Buf[1] = 0x0F;
            Debug_Buf[2] = 0x33;
            Debug_Buf[3] = 0x55;
            Debug_Buf[4] = 0xAA;
            Debug_Buf[5] = 0xCC;
            Debug_Buf[6] = 0xF0;
            Debug_Buf[7] = 0xFF;
            Debug_Buf[8] = 0x0F;
            Debug_Buf[9] = 0x00;
            Debug_Buf[10] = 0x55;
            Debug_Buf[11] = 0x33;
            Debug_Buf[12] = 0xCC;
            Debug_Buf[13] = 0xAA;
            Debug_Buf[14] = 0xFF;
            Debug_Buf[15] = 0xF0;
            if (InitData_comboBox.SelectedIndex == 6)
            {
                for (i = 0; i < datalenth; i++)
                {
                    j = (i / 128) % 8;
                    k = i % 8;
                    Progfilebuf[i] = (byte)Debug_Buf[(j + k) % 8];	////奇循环数据
                }
            }

            if (InitData_comboBox.SelectedIndex == 7)
            {
                for (i = 0; i < datalenth; i++)
                {
                    j = (i / 128) % 8;
                    k = i % 8;
                    Progfilebuf[i] = (byte)Debug_Buf[(j + k) % 8 + 8];	///偶循环数据
                }
            }

            if (InitData_comboBox.SelectedIndex == 8)
            {
                for (i = 0; i < datalenth; i++)
                {
                    Progfilebuf[i] = 0xB9;		//缓冲区初始化为0xB9
                }
            }

            Debug_Buf[16] = 0x80;
            Debug_Buf[17] = 0xFE;
            if (InitData_comboBox.SelectedIndex == 9)
            {
                for (i = 0; i < datalenth; i++)
                {
                    Progfilebuf[i] = (byte)Debug_Buf[16 + (i%2)];		//缓冲区初始化为0x80FE
                }
            }
            /////////////////////////////////////////////////////////////
            ProgOrReadEE_FF_flag = 1;
            ProgOrReadEE_80_flag = 1;
            ProgOrReadEE_81_flag = 1;
            ProgOrReadEE_82_flag = 1;
            ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
        }

        private void Init_CPUStart_08_Click(object sender, EventArgs e)
        {

            string sendbuf, strReceived;
            if (r347.Checked == true)
                sendbuf = "0040000002";
            else
                sendbuf = "0008" + text_08_P1.Text + "0000";
            int i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (i == 0)
            {
                display = "Data Received:  \t<-\t" + strReceived;
                DisplayMessageLine(display);
            }
            else
            {
                display = "Data Received:  \t<-\t" + "ERROR";
                DisplayMessageLine(display);
                return;
            }


        }

        private void Init_01_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived;

            //FM12XX_Card.Set_TDA8007_reg(TDA8007Reg.GTR, 0x01, out strReceived);
            //DisplayMessageLine("Set EGT : 01");

            sendbuf = "0001" + text_01_P1.Text + "0000";

            int i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (i == 0)
            {
                display = "Data Received:  \t<-\t" + strReceived;
                DisplayMessageLine(display);
            }
            else
            {
                display = "Data Received:  \t<-\t" + "ERROR";
                DisplayMessageLine(display);
                return;
            }

        }

        private void Init_EEopt_02_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived;

            if (r347.Checked == true)
                sendbuf = "000D" + text_02_P1.Text + text_02_P2.Text + "00";
            else
                sendbuf = "0002" + text_02_P1.Text + text_02_P2.Text + "00";

            int i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (i == 0)
            {
                display = "Data Received:  \t<-\t" + strReceived;
                DisplayMessageLine(display);
            }
            else
            {
                display = "Data Received:  \t<-\t" + "ERROR";
                DisplayMessageLine(display);
                return;
            }

        }

        private void Init_ewEEtime_03_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived, sendbuf_data;

            if (r347.Checked == true)
                sendbuf = "003D" + text_03_P1.Text + text_03_P2.Text + "10";
            else
                sendbuf = "0003" + text_03_P1.Text + text_03_P2.Text + "00";

            int i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (r347.Checked == true)
            {
                sendbuf_data = DeleteSpaceString(SendData_textBox.Text);
                display = "Data Received:\t<-\t" + "Step1:\t" + strReceived;
                DisplayMessageLine(display);
                if (i == 1)
                {
                    return;
                }
                i = FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);
                display = "Data Received:\t<-\t" + "Step2:\t" + strReceived;
                DisplayMessageLine(display);
                if (i == 1)
                {
                    return;
                }
            }
            else
            {
                if (i == 0)
                {
                    display = "Data Received:  \t<-\t" + strReceived;
                    DisplayMessageLine(display);
                }
                else
                {
                    display = "Data Received:  \t<-\t" + "ERROR";
                    DisplayMessageLine(display);
                    return;
                }
            }
        }
        private void Init_Rdee_04_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived;
            if (r347.Checked)
                sendbuf = "0004" + (strToHexByte(text_04_P1.Text)[0] * 256 + strToHexByte(text_04_P2.Text)[0]).ToString("X4") + text_04_len.Text;
            else if (radioButton1.Checked)
                sendbuf = "0004" + (((strToHexByte(text_04_P1.Text)[0] * 256 + strToHexByte(text_04_P2.Text)[0])) / 8).ToString("X4") + text_04_len.Text;
            else
                sendbuf = "0004" + (((strToHexByte(text_04_P1.Text)[0] * 256 + strToHexByte(text_04_P2.Text)[0])) / 4).ToString("X4") + text_04_len.Text;


            int i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (i == 0)
            {
                display = "Data Received:  \t<-\t" + strReceived;
                DisplayMessageLine(display);
            }
            else
            {
                display = "Data Received:  \t<-\t" + "ERROR";
                DisplayMessageLine(display);
                return;
            }
        }

        private void Init_Trim_0B_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived;
            int i;

            if (r347.Checked == true && wr3e.Checked == true)
            {
                sendbuf = "003E" + text_0B_P1.Text + text_0B_P2.Text + text_0B_P3.Text;
                i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                if (i != 0)
                {
                    DisplayMessageLine("Data Received:  \t<-\t" + "ERROR");
                    return;
                }
                sendbuf = DeleteSpaceString(SendData_textBox.Text);
            }
            else if (r347.Checked == true && rd05.Checked == true)
                sendbuf = "0005" + text_0B_P1.Text + text_0B_P2.Text + text_0B_P3.Text;
            else
                sendbuf = "000B" + text_0B_P1.Text + text_0B_P2.Text + "00";

            i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (i == 0)
            {
                display = "Data Received:  \t<-\t" + strReceived;
                DisplayMessageLine(display);
            }
            else
            {
                display = "Data Received:  \t<-\t" + "ERROR";
                DisplayMessageLine(display);
                return;
            }
        }

        public void Init_Wree_00_Click(object sender, EventArgs e)
        {
            string sendbuf, sendbuf_data, strReceived, saddr, saddrnvm = "", slen = "";
            int i, nadd, padd, plen, len = 1;
            bool nvm = false;
            byte tmp;

            nadd = strToHexByte(text_00_P1.Text)[0] * 256 + strToHexByte(text_00_P2.Text)[0];
            saddr = nadd.ToString("X4");
            slen = text_00_len.Text;
            tmp = strToHexByte(text_02_P1.Text)[0];
            sendbuf_data = DeleteSpaceString(SendData_textBox.Text);
            if ((sendbuf_data.Length / 2 != Convert.ToByte(text_00_len.Text, 16)) && text_00_len.Text != "00")
            {
                display = "数据长度错误！";
                DisplayMessageLine(display);
                return;
            }
            if (r347.Checked)
            {
                if ((tmp >= 0xE0 && tmp <= 0xFF) || tmp == 0x30 || tmp == 0x80 || tmp == 0x9E || tmp == 0x9F || tmp == 0xA0 || tmp == 0xA8 || tmp == 0xAF)
                {
                    plen = strToHexByte(slen)[0];
                    len = 4 - plen % 4;
                    if (len < 4)
                        for (i = 0; i < len; i++)
                        {
                            sendbuf_data += "00";
                            plen++;
                        }
                    slen = plen.ToString("X2");
                    nvm = false; //RAM&REG
                }
                else   //NVM              
                {
                    //if (tmp == 0xDE) //NVR     
                    //    len = 0x64;
                    //else
                    //    len = 0x80;
                    len = sendbuf_data.Length / 2;
                    if (len != 256)
                    {
                        slen = len.ToString("X8");
                        //tlen = Convert.ToByte(slen.Substring(6,2), 16);
                        FM12XX_Card.TransceiveCT("000D" + text_02_P1.Text + "0000", "02", out strReceived);
                        sendbuf = "0004" + text_00_P1.Text + "0000";
                        FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived); //bak sector                        
                        padd = strToHexByte(text_00_P2.Text)[0];
                        plen = strToHexByte(text_00_len.Text)[0];
                        sendbuf = strReceived.Substring((padd + plen + 1) * 2, (256 - padd - plen) * 2);
                        sendbuf_data = strReceived.Substring(2, padd * 2) + sendbuf_data + sendbuf;
                    }
                    saddrnvm = "00" + text_02_P1.Text + text_00_P1.Text + "00";

                    nvm = true;
                }
            }
            if (nvm)
            {
                FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                //sendbuf = "003C0000" + text_00_len.Text;
                sendbuf = "003C000000";
                //sendbuf = "003C0000"+slen.Substring(6,2);
                FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived); //cl_ram 0000
                i = FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);
                if (i == 1)
                {
                    DisplayMessageLine("step_1: 写cl_ram\t<-\t error!");
                    return;
                }
                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                i = FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);
                if (i == 1)
                {
                    DisplayMessageLine("step_2: 配置NVM_op_start\t<-\t error!");
                    return;
                }
                //slen = (len-1).ToString("X8");
                FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                //i = FM12XX_Card.TransceiveCT("00AF0000" + saddrnvm + slen + "00000005", "02", out strReceived);
                i = FM12XX_Card.TransceiveCT("00AF0000" + saddrnvm + "000000FF" + "00000045", "02", out strReceived);
                if (i == 1)
                {
                    DisplayMessageLine("step_3: 配置相关寄存器\t<-\t error!");
                    return;
                }
                FM12XX_Card.TransceiveCT("000D" + text_02_P1.Text + "0000", "02", out strReceived); //NVM
                i = FM12XX_Card.TransceiveCT("0004" + saddr + "01", "02", out strReceived); // NVM
                if (i == 1)
                {
                    DisplayMessageLine("step_4: 读目标地址\t<-\t error!");
                    return;
                }
                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010
                i = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                if (i != 0)
                {
                    DisplayMessageLine("step_5: NVM擦写\t<-\t error!");
                    return;
                }
                FM12XX_Card.TransceiveCT("0004020004", "02", out strReceived); // NVM nvm_op_status E00200
                if ((strToHexByte(strReceived.Substring(8, 2))[0] & 0x08) == 0)
                    DisplayMessageLine("NVM擦写:\t<-\t" + "Successed");
                else
                    DisplayMessageLine("NVM擦写:\t<-\t" + "擦写校验失败");

                FM12XX_Card.TransceiveCT("000D" + text_02_P1.Text + "0000", "02", out strReceived); //NVM
            }
            else
            {
                if (r347.Checked == true)
                    sendbuf = "003C" + saddr + slen;
                else if (radioButton1.Checked) //336
                    sendbuf = "0000" + (nadd / 8).ToString("X4") + slen;
                else
                    sendbuf = "0000" + (nadd / 4).ToString("X4") + slen;

                i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                display = "Data Received:\t<-\t" + "Step1:\t" + strReceived;
                DisplayMessageLine(display);
                if (i == 1)
                {
                    return;
                }
                i = FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);
                display = "Data Received:\t<-\t" + "Step2:\t" + strReceived;
                DisplayMessageLine(display);
                if (i == 1)
                {
                    return;
                }
            }
        }

        private void DataGridView_Reload(object sender, EventArgs e)
        {

            tabControl_main.SelectedIndex = 7;
            CfgWord_dataGridView.Focus();

            BindCfgwd();

            BindData();

            // 将下拉列表框加入到DataGridView控件中
            for (i = 0; i < CfgWord_combobox.Length; i++)
            {
                CfgWord_combobox[i].Visible = true;
                CfgWord_combobox[i].Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
                // CfgWord_combobox[i].SelectedIndexChanged += new EventHandler(CfgWord_combobox_SelectedIndexChanged);
                CfgWord_combobox[i].VisibleChanged += new EventHandler(CfgWord_combobox_VisibleChanged);
                CfgWord_dataGridView.Controls.Add(CfgWord_combobox[i]);
                CfgWord_combobox[i].Visible = false;
            }
            SetDefaultCfg();
            for (i = 0; i < CfgWord_combobox.Length; i++)
            {
                CfgWord_combobox[i].SelectedIndexChanged += new EventHandler(CfgWord_combobox_SelectedIndexChanged);
            }


            for (i = 0; i < CfgWord_checkbox.Length; i++)
            {
                CfgWord_checkbox[i] = new CheckBox();
                CfgWord_checkbox[i].Checked = false;
                CfgWord_checkbox[i].Text = "可写";
                CfgWord_checkbox[i].Visible = false;
                CfgWord_checkbox[i].CheckedChanged += new EventHandler(CfgWord_checkbox_CheckedChanged);
                CfgWord_dataGridView.Controls.Add(CfgWord_checkbox[i]);
            }

            //设置下拉框默认选项
            // SetDefaultCfg();
            /*for (i = 0; i < CfgWord_combobox.Length; i++)
            {
                CfgWord_combobox[i].Visible = true;
                CfgWord_combobox[i].Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
                CfgWord_combobox[i].SelectedIndexChanged += new EventHandler(CfgWord_combobox_SelectedIndexChanged);
                CfgWord_combobox[i].VisibleChanged += new EventHandler(CfgWord_combobox_VisibleChanged);
                CfgWord_dataGridView.Controls.Add(CfgWord_combobox[i]);
                CfgWord_combobox[i].Visible = false;
            }*/
            //SetDefaultCfg();

            CfgWord_dataGridView.CellEndEdit += new DataGridViewCellEventHandler(CfgWord_dataGridView_CellEndEdit);
            ConnectReader_Click(null, EventArgs.Empty);

        }



        ////////////FM309ConfigWord///////////////////////////////////////////////////////////////////////////////////////
        private void MainForm_Load(object sender, EventArgs e)
        {
            string CMD_bak;

            Hints_toolTip.SetToolTip(this.ConnectReader, "连接卡机");
            Hints_toolTip.SetToolTip(this.RunScrStep_simple, "从当前选中行单步运行脚本");
            Hints_toolTip.SetToolTip(this.RunScr_simple, "从当前选中行开始连续运行脚本");
            Hints_toolTip.SetToolTip(this.Send_Selected_Command_CL_button, "可选中多行，顺序执行所选指令");

            tabControl_main.SelectedIndex = 7;
            CfgWord_dataGridView.Focus();

            BindCfgwd();
            BindData();
            // 将下拉列表框加入到DataGridView控件中
            for (i = 0; i < CfgWord_combobox.Length; i++)
            {
                CfgWord_combobox[i].Visible = true;
                CfgWord_combobox[i].Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
                CfgWord_combobox[i].SelectedIndexChanged += new EventHandler(CfgWord_combobox_SelectedIndexChanged);
                CfgWord_combobox[i].VisibleChanged += new EventHandler(CfgWord_combobox_VisibleChanged);
                CfgWord_dataGridView.Controls.Add(CfgWord_combobox[i]);
                CfgWord_combobox[i].Visible = false;
            }
            for (i = 0; i < CfgWord_checkbox.Length; i++)
            {
                CfgWord_checkbox[i] = new CheckBox();
                CfgWord_checkbox[i].Checked = false;
                CfgWord_checkbox[i].Text = "可写";
                CfgWord_checkbox[i].Visible = false;
                CfgWord_checkbox[i].CheckedChanged += new EventHandler(CfgWord_checkbox_CheckedChanged);
                CfgWord_dataGridView.Controls.Add(CfgWord_checkbox[i]);
            }

            //设置下拉框默认选项309 old
            SetDefaultCfg();

            CfgWord_dataGridView.CellEndEdit += new DataGridViewCellEventHandler(CfgWord_dataGridView_CellEndEdit);


            tabControl_main.SelectedIndex = 0;
            GetReaders_bnt_Click(null, EventArgs.Empty);
            ConnectReader_Click(null, EventArgs.Empty);
            checkBox_APDUmodeCL.Checked = true;
            checkBox_APDUmodeCT.Checked = true;

            //从CMD_LOG.txt中读入历史指令
            //CL
            try
            {
                StreamReader sr = new StreamReader(".\\CMD_CL_LOG.txt",System.Text.Encoding.GetEncoding("gb2312"));   //打开命令记录文档
                TransceiveDataCL_listBox.Items.Clear();                 //清空列表，从指令记录文档读入。
                CMD_bak = sr.ReadLine();
                while (CMD_bak != null)
                {
                    TransceiveDataCL_listBox.Items.Add(CMD_bak);
                    SendMessage(TransceiveDataCL_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
                    CMD_bak = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);

                int volume = TransceiveDataCL_listBox.Items.Count;
                StreamWriter sw = new StreamWriter(".\\CMD_CL_LOG.txt");
                for (int i = 0; i < volume; i++)
                {
                    sw.WriteLine(TransceiveDataCL_listBox.Items[i].ToString());
                }
                sw.Close();
                DisplayMessageLine("已自动创建CMD_CL_LOG.txt文件用于记录指令。");
                //MessageBox.Show(ex.Message);
            }
            //CT
            try
            {
                StreamReader sr = new StreamReader(".\\CMD_CT_LOG.txt",System.Text.Encoding.GetEncoding("gb2312"));   //打开命令记录文档
                TransceiveDataCT_listBox.Items.Clear();                 //清空列表，从指令记录文档读入。
                CMD_bak = sr.ReadLine();
                while (CMD_bak != null)
                {
                    TransceiveDataCT_listBox.Items.Add(CMD_bak);
                    SendMessage(TransceiveDataCT_listBox.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
                    CMD_bak = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);

                int volume = TransceiveDataCT_listBox.Items.Count;
                StreamWriter sw = new StreamWriter(".\\CMD_CT_LOG.txt");
                for (int i = 0; i < volume; i++)
                {
                    sw.WriteLine(TransceiveDataCT_listBox.Items[i].ToString());
                }
                sw.Close();
                DisplayMessageLine("已自动创建CMD_CT_LOG.txt文件用于记录指令。");
                //MessageBox.Show(ex.Message);
            }
            tabControl_main.TabPages.Remove(this.FMXX_config);//cnn 隐藏FM309 old配置界面 20170502

        }

        private void CfgWord_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Cfg_Parse == true)
                return;
            if (((CheckBox)sender).Checked == true)
            {
                ((CheckBox)sender).Text = "不可写";
            }
            else
            {
                ((CheckBox)sender).Text = "可写";
            }
            if (initVal_done)
            {
                CfgWord_dataGridView.CurrentCell.Value = ((CheckBox)sender).Text;
                j = (CfgWord_dataGridView.CurrentCell.RowIndex - 74) % 8;

                if (((CheckBox)sender).Text == "可写")
                    CfgWord_dataGridView[2, CfgWord_dataGridView.CurrentCell.RowIndex].Value = "00";
                else
                    CfgWord_dataGridView[2, CfgWord_dataGridView.CurrentCell.RowIndex].Value = (01 << j).ToString("X2");

            }
            ((CheckBox)sender).Visible = false;

        }


        private void CfgWord_Parse_btn_Click(object sender, EventArgs e)
        {
            string data;
            byte[] CfgWord = new byte[32];
            int CfgWord_length;
            data = DeleteSpaceString(SendData_textBox.Text);

            if (data.Length == 0)
                return;

            Cfg_Parse = true;

            try
            {
                for (i = 0; i < data.Length; i = i + 2)
                {
                    CfgWord[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);

                }

                CfgWord_length = i / 2;
                if (CfgWord_length > 32)
                    CfgWord_length = 32;
                DisplayMessageLine("解析字节数： " + CfgWord_length.ToString());
                for (i = 0; i < 6; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[0] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 6; i < 11; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[1] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 11; i < 19; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[2] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 19; i < 23; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[3] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();

                }
                for (i = 23; i < 29; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[4] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();

                }
                for (i = 29; i < 36; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[5] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 36; i < 44; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[6] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }

                CfgWord_dataGridView[1, 44].Value = CfgWord[7].ToString("X2");
                CfgWord_dataGridView[2, 44].Value = CfgWord[7].ToString("X2");
                CfgWord_dataGridView[1, 45].Value = CfgWord[8].ToString("X2");
                CfgWord_dataGridView[2, 45].Value = CfgWord[8].ToString("X2");

                for (i = 46; i < 48; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[9] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 48; i < 52; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[10] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 52; i < 54; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[11] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 54; i < 57; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[12] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }

                CfgWord_dataGridView[1, 57].Value = (CfgWord[13] & CfgWord_Mask[57]).ToString("X2");
                CfgWord_dataGridView[2, 57].Value = (CfgWord[13] & CfgWord_Mask[57]).ToString("X2");

                CfgWord_combobox[58].SelectedValue = (CfgWord[13] & CfgWord_Mask[58]).ToString("X2");
                CfgWord_dataGridView[1, 58].Value = CfgWord_combobox[58].Text;
                CfgWord_dataGridView[2, 58].Value = CfgWord_combobox[58].SelectedValue.ToString();

                for (i = 59; i < 66; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[14] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 66; i < 67; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[15] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 67; i < 69; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[16] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 69; i < 71; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[17] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 71; i < 74; i++)
                {
                    CfgWord_combobox[i].SelectedValue = (CfgWord[18] & CfgWord_Mask[i]).ToString("X2");
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                for (i = 74; i < 82; i++)
                {
                    CfgWord_checkbox[i - 74].Checked = ((CfgWord[19] & CfgWord_Mask[i]) != 0x00);
                    if (CfgWord_checkbox[i - 74].Checked)
                    {
                        CfgWord_checkbox[i - 74].Text = "不可写";
                        CfgWord_dataGridView[1, i].Value = "不可写";
                        CfgWord_dataGridView[2, i].Value = CfgWord_Mask[i].ToString("X2");
                    }
                    else
                    {
                        CfgWord_checkbox[i - 74].Text = "可写";
                        CfgWord_dataGridView[1, i].Value = "可写";
                        CfgWord_dataGridView[2, i].Value = "00";
                    }
                }
                for (i = 82; i < 90; i++)
                {
                    CfgWord_checkbox[i - 74].Checked = ((CfgWord[20] & CfgWord_Mask[i]) != 0x00);
                    if (CfgWord_checkbox[i - 74].Checked)
                    {
                        CfgWord_checkbox[i - 74].Text = "不可写";
                        CfgWord_dataGridView[1, i].Value = "不可写";
                        CfgWord_dataGridView[2, i].Value = CfgWord_Mask[i].ToString("X2");
                    }
                    else
                    {
                        CfgWord_checkbox[i - 74].Text = "可写";
                        CfgWord_dataGridView[1, i].Value = "可写";
                        CfgWord_dataGridView[2, i].Value = "00";
                    }
                }

                for (i = 90; i < 98; i++)
                {
                    CfgWord_checkbox[i - 74].Checked = ((CfgWord[21] & CfgWord_Mask[i]) != 0x00);
                    if (CfgWord_checkbox[i - 74].Checked)
                    {
                        CfgWord_checkbox[i - 74].Text = "不可写";
                        CfgWord_dataGridView[1, i].Value = "不可写";
                        CfgWord_dataGridView[2, i].Value = CfgWord_Mask[i].ToString("X2");
                    }
                    else
                    {
                        CfgWord_checkbox[i - 74].Text = "可写";
                        CfgWord_dataGridView[1, i].Value = "可写";
                        CfgWord_dataGridView[2, i].Value = "00";
                    }
                }
                for (i = 98; i < 106; i++)
                {
                    CfgWord_checkbox[i - 74].Checked = ((CfgWord[22] & CfgWord_Mask[i]) != 0x00);
                    if (CfgWord_checkbox[i - 74].Checked)
                    {
                        CfgWord_checkbox[i - 74].Text = "不可写";
                        CfgWord_dataGridView[1, i].Value = "不可写";
                        CfgWord_dataGridView[2, i].Value = CfgWord_Mask[i].ToString("X2");
                    }
                    else
                    {
                        CfgWord_checkbox[i - 74].Text = "可写";
                        CfgWord_dataGridView[1, i].Value = "可写";
                        CfgWord_dataGridView[2, i].Value = "00";
                    }
                }
                for (i = 106; i < 114; i++)
                {
                    CfgWord_checkbox[i - 74].Checked = ((CfgWord[23] & CfgWord_Mask[i]) != 0x00);
                    if (CfgWord_checkbox[i - 74].Checked)
                    {
                        CfgWord_checkbox[i - 74].Text = "不可写";
                        CfgWord_dataGridView[1, i].Value = "不可写";
                        CfgWord_dataGridView[2, i].Value = CfgWord_Mask[i].ToString("X2");
                    }
                    else
                    {
                        CfgWord_checkbox[i - 74].Text = "可写";
                        CfgWord_dataGridView[1, i].Value = "可写";
                        CfgWord_dataGridView[2, i].Value = "00";
                    }
                }
                for (i = 114; i < 122; i++)
                {
                    CfgWord_checkbox[i - 74].Checked = ((CfgWord[24] & CfgWord_Mask[i]) != 0x00);
                    if (CfgWord_checkbox[i - 74].Checked)
                    {
                        CfgWord_checkbox[i - 74].Text = "不可写";
                        CfgWord_dataGridView[1, i].Value = "不可写";
                        CfgWord_dataGridView[2, i].Value = CfgWord_Mask[i].ToString("X2");
                    }
                    else
                    {
                        CfgWord_checkbox[i - 74].Text = "可写";
                        CfgWord_dataGridView[1, i].Value = "可写";
                        CfgWord_dataGridView[2, i].Value = "00";
                    }
                }
                CfgWord_dataGridView[1, 122].Value = CfgWord[25].ToString("X2");
                CfgWord_dataGridView[2, 122].Value = CfgWord[25].ToString("X2");

                CfgWord_dataGridView[1, 123].Value = CfgWord[26].ToString("X2");
                CfgWord_dataGridView[2, 123].Value = CfgWord[26].ToString("X2");

                CfgWord_dataGridView[1, 124].Value = CfgWord[27].ToString("X2");
                CfgWord_dataGridView[2, 124].Value = CfgWord[27].ToString("X2");

                CfgWord_dataGridView[1, 125].Value = CfgWord[28].ToString("X2");
                CfgWord_dataGridView[2, 125].Value = CfgWord[28].ToString("X2");

                CfgWord_dataGridView[1, 126].Value = CfgWord[29].ToString("X2");
                CfgWord_dataGridView[2, 126].Value = CfgWord[29].ToString("X2");

                CfgWord_dataGridView[1, 127].Value = CfgWord[30].ToString("X2");
                CfgWord_dataGridView[2, 127].Value = CfgWord[30].ToString("X2");

                CfgWord_dataGridView[1, 128].Value = CfgWord[31].ToString("X2");
                CfgWord_dataGridView[2, 128].Value = CfgWord[31].ToString("X2");


            }
            catch (Exception ex)
            {
                //    DisplayMessageLine(ex.Message);
            }

            Cfg_Parse = false;

        }


        private void CfgWd_Gen_button_Click(object sender, EventArgs e)
        {
            string CfgWord_Text = "";
            string First16Byte = "", Second16Byte = "";
            byte cfg_cw = 0x00;
            byte cfg_tmp = 0x00;
            string auth_key;

            try
            {
                for (i = 0; i < 6; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text = cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 6; i < 11; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 11; i < 19; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 19; i < 23; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 23; i < 29; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 29; i < 36; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 36; i < 44; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 44; i < 45; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 45; i < 46; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 46; i < 48; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 48; i < 52; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 52; i < 54; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 54; i < 57; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 57; i < 59; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 59; i < 66; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 66; i < 67; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");

                SendData_textBox.Text = CfgWord_Text;

                //DisplayMessageLine(CfgWord_Text);
                First16Byte = CfgWord_Text;

                CfgWord_Text = "";

                cfg_cw = 0x00;
                for (i = 67; i < 69; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 69; i < 71; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 71; i < 74; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 74; i < 82; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 82; i < 90; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 90; i < 98; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 98; i < 106; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 106; i < 114; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");
                cfg_cw = 0x00;
                for (i = 114; i < 122; i++)
                {
                    cfg_tmp = Convert.ToByte(CfgWord_dataGridView.Rows[i].Cells[2].Value.ToString(), 16);
                    cfg_cw += cfg_tmp;
                }
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[122].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[123].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[124].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[125].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[126].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[127].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                cfg_cw = Convert.ToByte(CfgWord_dataGridView.Rows[128].Cells[2].Value.ToString(), 16);
                CfgWord_Text += cfg_cw.ToString("X2");

                SendData_textBox.Text += CfgWord_Text;
                Second16Byte = CfgWord_Text;

                //DisplayMessageLine(CfgWord_Text);

                if (CfgWordWrite_chkbox.Checked)
                {
                    Field_ON_Click(null, EventArgs.Empty);
                    Active_Click(null, EventArgs.Empty);

                    if (CWD_auth_checkbox.Checked)
                    {
                        auth_key = richTextBox1.Text.Replace(" ", "");
                        if ((auth_key == "") || (auth_key.Length != 32))
                        {
                            display = "认证的密钥长度不对！！！   \t\t";
                            DisplayMessageLine(display);
                            return;
                        }
                        FM12XX_Card.TransceiveCL("A200", "03", "09", out receive);//CNN 添加认证，使认证时也能配置控制字
                        int k = FM12XX_Card.TransceiveCL(auth_key, "03", "09", out receive);//LOAD密钥
                        if (k != 0)
                        {
                            display = "认证:   \t\t" + "错误！！";
                            DisplayMessageLine(display);
                            return;
                        }
                        display = "认证:   \t\t" + "通过";
                        DisplayMessageLine(display);
                    }
                    FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);

                    FM12XX_Card.WriteBlock("00", First16Byte, out receive);
                    display = "WriteBlock 00:   \t\t" + receive;
                    DisplayMessageLine(display);

                    FM12XX_Card.WriteBlock("01", Second16Byte, out receive);
                    display = "WriteBlock 01:   \t\t" + receive;
                    DisplayMessageLine(display);
                    Reset17_Click(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                return;
            }

        }


        // 改变DataGridView列宽时将下拉列表框设为不可见
        private void CfgWord_dataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (initVal_done)
                this.CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Visible = false;
        }

        private void CfgWord_combobox_VisibleChanged(object sender, EventArgs e)
        {
            if (initVal_done == true)
                return;
        }

        // 当用户点击到这一列时单元格显示下拉列表框或者checkbox
        private void CfgWord_dataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if ((this.CfgWord_dataGridView.CurrentCell.ColumnIndex == 1) && (CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Items.Count > 1))
                {
                    Rectangle rect = CfgWord_dataGridView.GetCellDisplayRectangle(CfgWord_dataGridView.CurrentCell.ColumnIndex, CfgWord_dataGridView.CurrentCell.RowIndex, false);

                    CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Left = rect.Left;
                    CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Top = rect.Top;
                    CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Width = rect.Width;
                    CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Height = rect.Height;
                    for (i = 0; i < CfgWord_combobox.Length; i++)
                    {
                        if (i == CfgWord_dataGridView.CurrentCell.RowIndex)
                            CfgWord_combobox[i].Visible = true;
                        else
                            CfgWord_combobox[i].Visible = false;
                    }
                }
                else if ((this.CfgWord_dataGridView.CurrentCell.ColumnIndex == 1) && (CfgWord_dataGridView.CurrentCell.RowIndex >= 74) && (CfgWord_dataGridView.CurrentCell.RowIndex < 122))
                {
                    Rectangle rect = CfgWord_dataGridView.GetCellDisplayRectangle(CfgWord_dataGridView.CurrentCell.ColumnIndex, CfgWord_dataGridView.CurrentCell.RowIndex, false);

                    CfgWord_checkbox[CfgWord_dataGridView.CurrentCell.RowIndex - 74].Left = rect.Left;
                    CfgWord_checkbox[CfgWord_dataGridView.CurrentCell.RowIndex - 74].Top = rect.Top;
                    CfgWord_checkbox[CfgWord_dataGridView.CurrentCell.RowIndex - 74].Width = rect.Width;
                    CfgWord_checkbox[CfgWord_dataGridView.CurrentCell.RowIndex - 74].Height = rect.Height;
                    for (i = 0; i < CfgWord_checkbox.Length; i++)
                    {
                        if (i == CfgWord_dataGridView.CurrentCell.RowIndex - 74)
                            CfgWord_checkbox[i].Visible = true;
                        else
                            CfgWord_checkbox[i].Visible = false;
                    }
                }
                else
                {
                    for (int i = 0; i < CfgWord_combobox.Length; i++)
                    {
                        CfgWord_combobox[i].Visible = false;
                    }
                    for (i = 0; i < CfgWord_checkbox.Length; i++)
                    {
                        CfgWord_checkbox[i].Visible = false;
                    }

                }
            }
            catch
            {
            }
        }

        void CfgWord_dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            byte cfgValue;

            if ((this.CfgWord_dataGridView.CurrentCell.ColumnIndex == 1) && (CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Items.Count < 2))
            {
                try
                {
                    if (CfgWord_dataGridView.CurrentCell.RowIndex == 56)
                    {
                        cfgValue = Convert.ToByte(CfgWord_dataGridView.CurrentCell.Value.ToString(), 16);
                        if (cfgValue > 0x7F)
                        {
                            CfgWord_dataGridView[2, CfgWord_dataGridView.CurrentCell.RowIndex].Value = "需小于7F";
                            return;
                        }
                    }
                    cfgValue = Convert.ToByte(CfgWord_dataGridView.CurrentCell.Value.ToString(), 16);
                    CfgWord_dataGridView[2, CfgWord_dataGridView.CurrentCell.RowIndex].Value = CfgWord_dataGridView.CurrentCell.Value.ToString().ToUpper();
                }
                catch
                {
                    CfgWord_dataGridView[2, CfgWord_dataGridView.CurrentCell.RowIndex].Value = "需16进制数";
                }
            }
        }



        // 当用户选择下拉列表框时改变DataGridView单元格的内容
        private void CfgWord_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (initVal_done && !Cfg_Parse)
            {
                CfgWord_dataGridView.CurrentCell.Value = ((ComboBox)sender).Text;
                CfgWord_dataGridView[2, CfgWord_dataGridView.CurrentCell.RowIndex].Value = ((ComboBox)sender).SelectedValue.ToString();
            }

        }

        // 滚动DataGridView时将下拉列表框设为不可见
        private void CfgWord_dataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (CfgWord_dataGridView.CurrentCell.RowIndex < 75)
                this.CfgWord_combobox[CfgWord_dataGridView.CurrentCell.RowIndex].Visible = false;
            else if ((CfgWord_dataGridView.CurrentCell.RowIndex >= 75) && (CfgWord_dataGridView.CurrentCell.RowIndex < 129))
            {
                this.CfgWord_checkbox[CfgWord_dataGridView.CurrentCell.RowIndex - 74].Visible = false;
            }

        }

        /// <summary>
        /// 绑定控制字下拉列表框
        /// </summary>
        private void BindCfgwd()
        {
            int temp;
            for (int i = 0; i < CfgWord_combobox.Length; i++)
            {
                CfgWord_combobox[i] = new ComboBox();
                CfgWord_combobox[i].ValueMember = "Value";
                CfgWord_combobox[i].DisplayMember = "Name";
                CfgWord_combobox[i].DropDownStyle = ComboBoxStyle.DropDownList;
            }


            //CW0/////0
            DataTable dtUID = new DataTable();
            dtUID.Columns.Add("Name");
            dtUID.Columns.Add("Value");
            DataRow dr_data;
            dr_data = dtUID.NewRow(); dr_data[0] = "一重UID"; dr_data[1] = "00"; dtUID.Rows.Add(dr_data);
            dr_data = dtUID.NewRow(); dr_data[0] = "二重UID"; dr_data[1] = "01"; dtUID.Rows.Add(dr_data);
            dr_data = dtUID.NewRow(); dr_data[0] = "三重UID"; dr_data[1] = "02"; dtUID.Rows.Add(dr_data);
            dr_data = dtUID.NewRow(); dr_data[0] = "(RFU)一重UID"; dr_data[1] = "03"; dtUID.Rows.Add(dr_data);
            CfgWord_combobox[0].DataSource = dtUID;
            CfgWord_Mask[0] = 0x03;
            //CW0/////1
            DataTable dtCLinterface = new DataTable();
            dtCLinterface.Columns.Add("Name");
            dtCLinterface.Columns.Add("Value");
            dr_data = dtCLinterface.NewRow(); dr_data[0] = "禁用非接触接口"; dr_data[1] = "00"; dtCLinterface.Rows.Add(dr_data);
            dr_data = dtCLinterface.NewRow(); dr_data[0] = "使用非接触接口"; dr_data[1] = "04"; dtCLinterface.Rows.Add(dr_data);
            CfgWord_combobox[1].DataSource = dtCLinterface;
            CfgWord_Mask[1] = 0x04;
            //CW0/////2
            DataTable dtCLtype = new DataTable();
            dtCLtype.Columns.Add("Name");
            dtCLtype.Columns.Add("Value");
            dr_data = dtCLtype.NewRow(); dr_data[0] = "非接触TypeA"; dr_data[1] = "08"; dtCLtype.Rows.Add(dr_data);
            dr_data = dtCLtype.NewRow(); dr_data[0] = "非接触typeB"; dr_data[1] = "10"; dtCLtype.Rows.Add(dr_data);
            dr_data = dtCLtype.NewRow(); dr_data[0] = "typeA+typeB"; dr_data[1] = "18"; dtCLtype.Rows.Add(dr_data);
            dr_data = dtCLtype.NewRow(); dr_data[0] = "RFU（typeA+typeB）"; dr_data[1] = "00"; dtCLtype.Rows.Add(dr_data);
            CfgWord_combobox[2].DataSource = dtCLtype;
            CfgWord_Mask[2] = 0x18;
            //CW0/////3
            DataTable dtProg_ext_mode = new DataTable();
            dtProg_ext_mode.Columns.Add("Name");
            dtProg_ext_mode.Columns.Add("Value");
            dr_data = dtProg_ext_mode.NewRow(); dr_data[0] = "整体扩展"; dr_data[1] = "00"; dtProg_ext_mode.Rows.Add(dr_data);
            dr_data = dtProg_ext_mode.NewRow(); dr_data[0] = "段后扩展"; dr_data[1] = "20"; dtProg_ext_mode.Rows.Add(dr_data);
            CfgWord_combobox[3].DataSource = dtProg_ext_mode;
            CfgWord_Mask[3] = 0x20;
            //CW0/////4
            DataTable dtProg_start_mem = new DataTable();
            dtProg_start_mem.Columns.Add("Name");
            dtProg_start_mem.Columns.Add("Value");
            dr_data = dtProg_start_mem.NewRow(); dr_data[0] = "EEPROM启动"; dr_data[1] = "00"; dtProg_start_mem.Rows.Add(dr_data);
            dr_data = dtProg_start_mem.NewRow(); dr_data[0] = "RAM启动"; dr_data[1] = "40"; dtProg_start_mem.Rows.Add(dr_data);
            CfgWord_combobox[4].DataSource = dtProg_start_mem;
            CfgWord_Mask[4] = 0x40;
            //CW0/////5
            DataTable dtCard_type = new DataTable();
            dtCard_type.Columns.Add("Name");
            dtCard_type.Columns.Add("Value");
            dr_data = dtCard_type.NewRow(); dr_data[0] = "纯CPU卡"; dr_data[1] = "00"; dtCard_type.Rows.Add(dr_data);
            dr_data = dtCard_type.NewRow(); dr_data[0] = "CPU+逻辑加密卡"; dr_data[1] = "80"; dtCard_type.Rows.Add(dr_data);
            CfgWord_combobox[5].DataSource = dtCard_type;
            CfgWord_Mask[5] = 0x80;
            //CW1/////6
            DataTable dtlogicCardType = new DataTable();
            dtlogicCardType.Columns.Add("Name");
            dtlogicCardType.Columns.Add("Value");
            dr_data = dtlogicCardType.NewRow(); dr_data[0] = "普通逻辑加密卡"; dr_data[1] = "00"; dtlogicCardType.Rows.Add(dr_data);
            dr_data = dtlogicCardType.NewRow(); dr_data[0] = "兼容s70逻辑加密卡"; dr_data[1] = "01"; dtlogicCardType.Rows.Add(dr_data);
            CfgWord_combobox[6].DataSource = dtlogicCardType;
            CfgWord_Mask[6] = 0x01;
            //CW1/////7
            DataTable dtlogicEncryType = new DataTable();
            dtlogicEncryType.Columns.Add("Name");
            dtlogicEncryType.Columns.Add("Value");
            dr_data = dtlogicEncryType.NewRow(); dr_data[0] = "MIFARE"; dr_data[1] = "00"; dtlogicEncryType.Rows.Add(dr_data);
            dr_data = dtlogicEncryType.NewRow(); dr_data[0] = "SH"; dr_data[1] = "02"; dtlogicEncryType.Rows.Add(dr_data);
            CfgWord_combobox[7].DataSource = dtlogicEncryType;
            CfgWord_Mask[7] = 0x02;
            //CW1/////8
            DataTable dtlogicMemSize = new DataTable();
            dtlogicMemSize.Columns.Add("Name");
            dtlogicMemSize.Columns.Add("Value");
            dr_data = dtlogicMemSize.NewRow(); dr_data[0] = "1K逻辑加密区"; dr_data[1] = "00"; dtlogicMemSize.Rows.Add(dr_data);
            dr_data = dtlogicMemSize.NewRow(); dr_data[0] = "4K逻辑加密区"; dr_data[1] = "04"; dtlogicMemSize.Rows.Add(dr_data);
            CfgWord_combobox[8].DataSource = dtlogicMemSize;
            CfgWord_Mask[8] = 0x04;
            //CW1/////9
            DataTable dtlogicCardNum = new DataTable();
            dtlogicCardNum.Columns.Add("Name");
            dtlogicCardNum.Columns.Add("Value");
            dr_data = dtlogicCardNum.NewRow(); dr_data[0] = "无扩展(1张)"; dr_data[1] = "00"; dtlogicCardNum.Rows.Add(dr_data);
            dr_data = dtlogicCardNum.NewRow(); dr_data[0] = "扩展出2张"; dr_data[1] = "08"; dtlogicCardNum.Rows.Add(dr_data);
            dr_data = dtlogicCardNum.NewRow(); dr_data[0] = "扩展出4张"; dr_data[1] = "10"; dtlogicCardNum.Rows.Add(dr_data);
            dr_data = dtlogicCardNum.NewRow(); dr_data[0] = "扩展出8张"; dr_data[1] = "18"; dtlogicCardNum.Rows.Add(dr_data);
            CfgWord_combobox[9].DataSource = dtlogicCardNum;
            CfgWord_Mask[9] = 0x18;
            //CW1/////10
            DataTable dtMmuModeSel = new DataTable();
            dtMmuModeSel.Columns.Add("Name");
            dtMmuModeSel.Columns.Add("Value");
            dr_data = dtMmuModeSel.NewRow(); dr_data[0] = "MMU不区分SM、AM"; dr_data[1] = "00"; dtMmuModeSel.Rows.Add(dr_data);
            dr_data = dtMmuModeSel.NewRow(); dr_data[0] = "MMU区分SM、AM"; dr_data[1] = "20"; dtMmuModeSel.Rows.Add(dr_data);
            CfgWord_combobox[10].DataSource = dtMmuModeSel;
            CfgWord_Mask[10] = 0x20;
            temp = 0;


            //CW2/////11
            DataTable dtFreDetEn = new DataTable();
            dtFreDetEn.Columns.Add("Name");
            dtFreDetEn.Columns.Add("Value");
            dr_data = dtFreDetEn.NewRow(); dr_data[0] = "频率检测关闭"; dr_data[1] = "00"; dtFreDetEn.Rows.Add(dr_data);
            dr_data = dtFreDetEn.NewRow(); dr_data[0] = "频率检测开启"; dr_data[1] = "01"; dtFreDetEn.Rows.Add(dr_data);
            CfgWord_combobox[11 + temp].DataSource = dtFreDetEn;
            CfgWord_Mask[11 + temp] = 0x01;
            //CW2/////12
            DataTable dtLightDetEn = new DataTable();
            dtLightDetEn.Columns.Add("Name");
            dtLightDetEn.Columns.Add("Value");
            dr_data = dtLightDetEn.NewRow(); dr_data[0] = "光检测关闭"; dr_data[1] = "00"; dtLightDetEn.Rows.Add(dr_data);
            dr_data = dtLightDetEn.NewRow(); dr_data[0] = "光检测开启"; dr_data[1] = "02"; dtLightDetEn.Rows.Add(dr_data);
            CfgWord_combobox[12 + temp].DataSource = dtLightDetEn;
            CfgWord_Mask[12 + temp] = 0x02;
            //CW2/////13
            DataTable dtShieldDetEn = new DataTable();
            dtShieldDetEn.Columns.Add("Name");
            dtShieldDetEn.Columns.Add("Value");
            dr_data = dtShieldDetEn.NewRow(); dr_data[0] = "屏蔽层检测关闭"; dr_data[1] = "00"; dtShieldDetEn.Rows.Add(dr_data);
            dr_data = dtShieldDetEn.NewRow(); dr_data[0] = "屏蔽层检测开启"; dr_data[1] = "04"; dtShieldDetEn.Rows.Add(dr_data);
            CfgWord_combobox[13 + temp].DataSource = dtShieldDetEn;
            CfgWord_Mask[13 + temp] = 0x04;
            //CW2/////14
            DataTable dtTempDetEn = new DataTable();
            dtTempDetEn.Columns.Add("Name");
            dtTempDetEn.Columns.Add("Value");
            dr_data = dtTempDetEn.NewRow(); dr_data[0] = "温度检测关闭"; dr_data[1] = "00"; dtTempDetEn.Rows.Add(dr_data);
            dr_data = dtTempDetEn.NewRow(); dr_data[0] = "温度检测开启"; dr_data[1] = "08"; dtTempDetEn.Rows.Add(dr_data);
            CfgWord_combobox[14 + temp].DataSource = dtTempDetEn;
            CfgWord_Mask[14 + temp] = 0x08;
            //CW2/////15
            DataTable dtGlitchDetEn = new DataTable();
            dtGlitchDetEn.Columns.Add("Name");
            dtGlitchDetEn.Columns.Add("Value");
            dr_data = dtGlitchDetEn.NewRow(); dr_data[0] = "电压检测关闭"; dr_data[1] = "00"; dtGlitchDetEn.Rows.Add(dr_data);
            dr_data = dtGlitchDetEn.NewRow(); dr_data[0] = "电压检测开启"; dr_data[1] = "10"; dtGlitchDetEn.Rows.Add(dr_data);
            CfgWord_combobox[15 + temp].DataSource = dtGlitchDetEn;
            CfgWord_Mask[15 + temp] = 0x10;
            //CW2/////16
            DataTable dtSecRstEn = new DataTable();
            dtSecRstEn.Columns.Add("Name");
            dtSecRstEn.Columns.Add("Value");
            dr_data = dtSecRstEn.NewRow(); dr_data[0] = "安全检测不产生复位"; dr_data[1] = "00"; dtSecRstEn.Rows.Add(dr_data);
            dr_data = dtSecRstEn.NewRow(); dr_data[0] = "安全检测产生复位"; dr_data[1] = "20"; dtSecRstEn.Rows.Add(dr_data);
            CfgWord_combobox[16 + temp].DataSource = dtSecRstEn;
            CfgWord_Mask[16 + temp] = 0x20;
            //CW2/////17
            DataTable dtShieldMode = new DataTable();
            dtShieldMode.Columns.Add("Name");
            dtShieldMode.Columns.Add("Value");
            dr_data = dtShieldMode.NewRow(); dr_data[0] = "静态Shield"; dr_data[1] = "00"; dtShieldMode.Rows.Add(dr_data);
            dr_data = dtShieldMode.NewRow(); dr_data[0] = "动态Shield"; dr_data[1] = "40"; dtShieldMode.Rows.Add(dr_data);
            CfgWord_combobox[17 + temp].DataSource = dtShieldMode;
            CfgWord_Mask[17 + temp] = 0x40;
            //CW2/////18
            DataTable dtCtCpuStopEn = new DataTable();
            dtCtCpuStopEn.Columns.Add("Name");
            dtCtCpuStopEn.Columns.Add("Value");
            dr_data = dtCtCpuStopEn.NewRow(); dr_data[0] = "CT接口不停止CPU时钟"; dr_data[1] = "00"; dtCtCpuStopEn.Rows.Add(dr_data);
            dr_data = dtCtCpuStopEn.NewRow(); dr_data[0] = "CT接口停止CPU时钟"; dr_data[1] = "80"; dtCtCpuStopEn.Rows.Add(dr_data);
            CfgWord_combobox[18 + temp].DataSource = dtCtCpuStopEn;
            CfgWord_Mask[18 + temp] = 0x80;
            //CW3/////19
            DataTable dtLowTempCfg = new DataTable();
            dtLowTempCfg.Columns.Add("Name");
            dtLowTempCfg.Columns.Add("Value");
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b000"; dr_data[1] = "00"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b001"; dr_data[1] = "01"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b010"; dr_data[1] = "02"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b011"; dr_data[1] = "03"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b100"; dr_data[1] = "04"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b101"; dr_data[1] = "05"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b110"; dr_data[1] = "06"; dtLowTempCfg.Rows.Add(dr_data);
            dr_data = dtLowTempCfg.NewRow(); dr_data[0] = "3'b111"; dr_data[1] = "07"; dtLowTempCfg.Rows.Add(dr_data);
            CfgWord_combobox[19 + temp].DataSource = dtLowTempCfg;
            CfgWord_Mask[19 + temp] = 0x07;
            //CW3/////20
            DataTable dtHighTempCfg = new DataTable();
            dtHighTempCfg.Columns.Add("Name");
            dtHighTempCfg.Columns.Add("Value");
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b000"; dr_data[1] = "00"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b001"; dr_data[1] = "08"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b010"; dr_data[1] = "10"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b011"; dr_data[1] = "18"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b100"; dr_data[1] = "20"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b101"; dr_data[1] = "28"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b110"; dr_data[1] = "30"; dtHighTempCfg.Rows.Add(dr_data);
            dr_data = dtHighTempCfg.NewRow(); dr_data[0] = "3'b111"; dr_data[1] = "38"; dtHighTempCfg.Rows.Add(dr_data);
            CfgWord_combobox[20 + temp].DataSource = dtHighTempCfg;
            CfgWord_Mask[20 + temp] = 0x38;

            //CW3/////21
            temp = 0;
            DataTable dtClaAtqaSakSel = new DataTable();
            dtClaAtqaSakSel.Columns.Add("Name");
            dtClaAtqaSakSel.Columns.Add("Value");
            dr_data = dtClaAtqaSakSel.NewRow(); dr_data[0] = "ATQA、SAK来自EEPROM"; dr_data[1] = "00"; dtClaAtqaSakSel.Rows.Add(dr_data);
            dr_data = dtClaAtqaSakSel.NewRow(); dr_data[0] = "ATQA、SAK来自寄存器"; dr_data[1] = "40"; dtClaAtqaSakSel.Rows.Add(dr_data);
            CfgWord_combobox[21].DataSource = dtClaAtqaSakSel;
            CfgWord_Mask[21] = 0x40;
            //CW3/////22
            DataTable dtUserUidEn = new DataTable();
            dtUserUidEn.Columns.Add("Name");
            dtUserUidEn.Columns.Add("Value");
            dr_data = dtUserUidEn.NewRow(); dr_data[0] = "不使用用户UID"; dr_data[1] = "00"; dtUserUidEn.Rows.Add(dr_data);
            dr_data = dtUserUidEn.NewRow(); dr_data[0] = "使用用户UID"; dr_data[1] = "80"; dtUserUidEn.Rows.Add(dr_data);
            CfgWord_combobox[22].DataSource = dtUserUidEn;
            CfgWord_Mask[22] = 0x80;

            //CW4/////23
            DataTable dtRandSampFrq = new DataTable();
            dtRandSampFrq.Columns.Add("Name");
            dtRandSampFrq.Columns.Add("Value");
            dr_data = dtRandSampFrq.NewRow(); dr_data[0] = "1MHz"; dr_data[1] = "00"; dtRandSampFrq.Rows.Add(dr_data);
            dr_data = dtRandSampFrq.NewRow(); dr_data[0] = "2MHz"; dr_data[1] = "01"; dtRandSampFrq.Rows.Add(dr_data);
            dr_data = dtRandSampFrq.NewRow(); dr_data[0] = "4MHz"; dr_data[1] = "02"; dtRandSampFrq.Rows.Add(dr_data);
            dr_data = dtRandSampFrq.NewRow(); dr_data[0] = "RFU(4MHz)"; dr_data[1] = "03"; dtRandSampFrq.Rows.Add(dr_data);
            CfgWord_combobox[23 + temp].DataSource = dtRandSampFrq;
            CfgWord_Mask[23 + temp] = 0x03;
            //CW4/////24
            DataTable dtGaloisNumber = new DataTable();
            dtGaloisNumber.Columns.Add("Name");
            dtGaloisNumber.Columns.Add("Value");
            dr_data = dtGaloisNumber.NewRow(); dr_data[0] = "一个Galois"; dr_data[1] = "00"; dtGaloisNumber.Rows.Add(dr_data);
            dr_data = dtGaloisNumber.NewRow(); dr_data[0] = "二个Galois"; dr_data[1] = "04"; dtGaloisNumber.Rows.Add(dr_data);
            dr_data = dtGaloisNumber.NewRow(); dr_data[0] = "三个Galois"; dr_data[1] = "08"; dtGaloisNumber.Rows.Add(dr_data);
            dr_data = dtGaloisNumber.NewRow(); dr_data[0] = "四个Galois"; dr_data[1] = "0C"; dtGaloisNumber.Rows.Add(dr_data);
            CfgWord_combobox[24 + temp].DataSource = dtGaloisNumber;
            CfgWord_Mask[24 + temp] = 0x0C;
            //CW4/////25
            DataTable dtFibonacciEn = new DataTable();
            dtFibonacciEn.Columns.Add("Name");
            dtFibonacciEn.Columns.Add("Value");
            dr_data = dtFibonacciEn.NewRow(); dr_data[0] = "Fibonacci禁止"; dr_data[1] = "00"; dtFibonacciEn.Rows.Add(dr_data);
            dr_data = dtFibonacciEn.NewRow(); dr_data[0] = "Fibonacci使能"; dr_data[1] = "10"; dtFibonacciEn.Rows.Add(dr_data);
            CfgWord_combobox[25 + temp].DataSource = dtFibonacciEn;
            CfgWord_Mask[25 + temp] = 0x10;
            //CW4/////26
            DataTable dtInitCmdEn = new DataTable();
            dtInitCmdEn.Columns.Add("Name");
            dtInitCmdEn.Columns.Add("Value");
            dr_data = dtInitCmdEn.NewRow(); dr_data[0] = "初始化指令无效"; dr_data[1] = "00"; dtInitCmdEn.Rows.Add(dr_data);
            dr_data = dtInitCmdEn.NewRow(); dr_data[0] = "初始化指令有效"; dr_data[1] = "20"; dtInitCmdEn.Rows.Add(dr_data);
            CfgWord_combobox[26 + temp].DataSource = dtInitCmdEn;
            CfgWord_Mask[26 + temp] = 0x20;
            //CW4/////27
            DataTable dtInitRdNonencEn = new DataTable();
            dtInitRdNonencEn.Columns.Add("Name");
            dtInitRdNonencEn.Columns.Add("Value");
            dr_data = dtInitRdNonencEn.NewRow(); dr_data[0] = "初始化指令不能读取ROM"; dr_data[1] = "00"; dtInitRdNonencEn.Rows.Add(dr_data);
            dr_data = dtInitRdNonencEn.NewRow(); dr_data[0] = "初始化指令可以读取ROM"; dr_data[1] = "40"; dtInitRdNonencEn.Rows.Add(dr_data);
            CfgWord_combobox[27 + temp].DataSource = dtInitRdNonencEn;
            CfgWord_Mask[27 + temp] = 0x40;
            //CW4/////28
            DataTable dtInitRdEncryEn = new DataTable();
            dtInitRdEncryEn.Columns.Add("Name");
            dtInitRdEncryEn.Columns.Add("Value");
            dr_data = dtInitRdEncryEn.NewRow(); dr_data[0] = "初始化指令不能读取encry_lib"; dr_data[1] = "00"; dtInitRdEncryEn.Rows.Add(dr_data);
            dr_data = dtInitRdEncryEn.NewRow(); dr_data[0] = "初始化指令可以读取encry_lib"; dr_data[1] = "80"; dtInitRdEncryEn.Rows.Add(dr_data);
            CfgWord_combobox[28 + temp].DataSource = dtInitRdEncryEn;
            CfgWord_Mask[28 + temp] = 0x80;
            //CW5/////29
            DataTable dtCtEn = new DataTable();
            dtCtEn.Columns.Add("Name");
            dtCtEn.Columns.Add("Value");
            dr_data = dtCtEn.NewRow(); dr_data[0] = "接触接口关闭"; dr_data[1] = "00"; dtCtEn.Rows.Add(dr_data);
            dr_data = dtCtEn.NewRow(); dr_data[0] = "接触接口打开"; dr_data[1] = "02"; dtCtEn.Rows.Add(dr_data);
            CfgWord_combobox[29 + temp].DataSource = dtCtEn;
            CfgWord_Mask[29 + temp] = 0x02;
            //CW5/////30
            DataTable dtSpiEn = new DataTable();
            dtSpiEn.Columns.Add("Name");
            dtSpiEn.Columns.Add("Value");
            dr_data = dtSpiEn.NewRow(); dr_data[0] = "SPI接口关闭"; dr_data[1] = "00"; dtSpiEn.Rows.Add(dr_data);
            dr_data = dtSpiEn.NewRow(); dr_data[0] = "SPI接口打开"; dr_data[1] = "04"; dtSpiEn.Rows.Add(dr_data);
            CfgWord_combobox[30 + temp].DataSource = dtSpiEn;
            CfgWord_Mask[30 + temp] = 0x04;
            //CW5/////31
            DataTable dtI2CEn = new DataTable();
            dtI2CEn.Columns.Add("Name");
            dtI2CEn.Columns.Add("Value");
            dr_data = dtI2CEn.NewRow(); dr_data[0] = "I2C接口关闭"; dr_data[1] = "00"; dtI2CEn.Rows.Add(dr_data);
            dr_data = dtI2CEn.NewRow(); dr_data[0] = "I2C接口打开"; dr_data[1] = "08"; dtI2CEn.Rows.Add(dr_data);
            CfgWord_combobox[31 + temp].DataSource = dtI2CEn;
            CfgWord_Mask[31 + temp] = 0x08;
            //CW5/////32
            DataTable dtUartEn = new DataTable();
            dtUartEn.Columns.Add("Name");
            dtUartEn.Columns.Add("Value");
            dr_data = dtUartEn.NewRow(); dr_data[0] = "UART接口关闭"; dr_data[1] = "00"; dtUartEn.Rows.Add(dr_data);
            dr_data = dtUartEn.NewRow(); dr_data[0] = "UART接口打开"; dr_data[1] = "10"; dtUartEn.Rows.Add(dr_data);
            CfgWord_combobox[32 + temp].DataSource = dtUartEn;
            CfgWord_Mask[32 + temp] = 0x10;


            //CW5/////33
            temp = 0;
            DataTable dtSwpEn = new DataTable();
            dtSwpEn.Columns.Add("Name");
            dtSwpEn.Columns.Add("Value");
            dr_data = dtSwpEn.NewRow(); dr_data[0] = "SWP接口关闭"; dr_data[1] = "00"; dtSwpEn.Rows.Add(dr_data);
            dr_data = dtSwpEn.NewRow(); dr_data[0] = "SWP接口打开"; dr_data[1] = "20"; dtSwpEn.Rows.Add(dr_data);
            CfgWord_combobox[33].DataSource = dtSwpEn;
            CfgWord_Mask[33] = 0x20;

            //CW5/////34
            DataTable dtEswpEn = new DataTable();
            dtEswpEn.Columns.Add("Name");
            dtEswpEn.Columns.Add("Value");
            dr_data = dtEswpEn.NewRow(); dr_data[0] = "eSWP接口关闭"; dr_data[1] = "00"; dtEswpEn.Rows.Add(dr_data);
            dr_data = dtEswpEn.NewRow(); dr_data[0] = "eSWP接口打开"; dr_data[1] = "40"; dtEswpEn.Rows.Add(dr_data);
            CfgWord_combobox[34 + temp].DataSource = dtEswpEn;
            CfgWord_Mask[34 + temp] = 0x40;
            //CW5/////35
            DataTable dtS2CEn = new DataTable();
            dtS2CEn.Columns.Add("Name");
            dtS2CEn.Columns.Add("Value");
            dr_data = dtS2CEn.NewRow(); dr_data[0] = "S2C接口关闭"; dr_data[1] = "00"; dtS2CEn.Rows.Add(dr_data);
            dr_data = dtS2CEn.NewRow(); dr_data[0] = "S2C接口打开"; dr_data[1] = "80"; dtS2CEn.Rows.Add(dr_data);
            CfgWord_combobox[35 + temp].DataSource = dtS2CEn;
            CfgWord_Mask[35 + temp] = 0x80;
            //CW6/////36
            DataTable dtRaeEn = new DataTable();
            dtRaeEn.Columns.Add("Name");
            dtRaeEn.Columns.Add("Value");
            dr_data = dtRaeEn.NewRow(); dr_data[0] = "不支持RSA/ECC算法"; dr_data[1] = "00"; dtRaeEn.Rows.Add(dr_data);
            dr_data = dtRaeEn.NewRow(); dr_data[0] = "支持RSA/ECC算法"; dr_data[1] = "01"; dtRaeEn.Rows.Add(dr_data);
            CfgWord_combobox[36 + temp].DataSource = dtRaeEn;
            CfgWord_Mask[36 + temp] = 0x01;
            //CW6/////37
            DataTable dtDesEn = new DataTable();
            dtDesEn.Columns.Add("Name");
            dtDesEn.Columns.Add("Value");
            dr_data = dtDesEn.NewRow(); dr_data[0] = "不支持DES运算"; dr_data[1] = "00"; dtDesEn.Rows.Add(dr_data);
            dr_data = dtDesEn.NewRow(); dr_data[0] = "支持DES算法"; dr_data[1] = "02"; dtDesEn.Rows.Add(dr_data);
            CfgWord_combobox[37 + temp].DataSource = dtDesEn;
            CfgWord_Mask[37 + temp] = 0x02;


            //CW6/////38
            temp = 0;
            DataTable dtAesEn = new DataTable();
            dtAesEn.Columns.Add("Name");
            dtAesEn.Columns.Add("Value");
            dr_data = dtAesEn.NewRow(); dr_data[0] = "不支持AES运算"; dr_data[1] = "00"; dtAesEn.Rows.Add(dr_data);
            dr_data = dtAesEn.NewRow(); dr_data[0] = "支持AES算法"; dr_data[1] = "04"; dtAesEn.Rows.Add(dr_data);
            CfgWord_combobox[38].DataSource = dtAesEn;
            CfgWord_Mask[38] = 0x04;

            //CW6/////39
            DataTable dtSsf33En = new DataTable();
            dtSsf33En.Columns.Add("Name");
            dtSsf33En.Columns.Add("Value");
            dr_data = dtSsf33En.NewRow(); dr_data[0] = "不支持SSF33运算"; dr_data[1] = "00"; dtSsf33En.Rows.Add(dr_data);
            dr_data = dtSsf33En.NewRow(); dr_data[0] = "支持SSF33算法"; dr_data[1] = "08"; dtSsf33En.Rows.Add(dr_data);
            CfgWord_combobox[39].DataSource = dtSsf33En;
            CfgWord_Mask[39] = 0x08;
            //CW6/////40
            DataTable dtSm1En = new DataTable();
            dtSm1En.Columns.Add("Name");
            dtSm1En.Columns.Add("Value");
            dr_data = dtSm1En.NewRow(); dr_data[0] = "不支持SM1运算"; dr_data[1] = "00"; dtSm1En.Rows.Add(dr_data);
            dr_data = dtSm1En.NewRow(); dr_data[0] = "支持SM1算法"; dr_data[1] = "10"; dtSm1En.Rows.Add(dr_data);
            CfgWord_combobox[40].DataSource = dtSm1En;
            CfgWord_Mask[40] = 0x10;
            //CW6/////38
            temp = 0;
            //CW6/////41
            DataTable dtSm7En = new DataTable();
            dtSm7En.Columns.Add("Name");
            dtSm7En.Columns.Add("Value");
            dr_data = dtSm7En.NewRow(); dr_data[0] = "不支持SM7运算"; dr_data[1] = "00"; dtSm7En.Rows.Add(dr_data);
            dr_data = dtSm7En.NewRow(); dr_data[0] = "支持SM7算法"; dr_data[1] = "20"; dtSm7En.Rows.Add(dr_data);
            CfgWord_combobox[41].DataSource = dtSm7En;
            CfgWord_Mask[41] = 0x20;

            //CW6/////42
            DataTable dtAcesSpecSecuEn = new DataTable();
            dtAcesSpecSecuEn.Columns.Add("Name");
            dtAcesSpecSecuEn.Columns.Add("Value");
            dr_data = dtAcesSpecSecuEn.NewRow(); dr_data[0] = "只有Encry_Lib区可访问安全区特殊数据区"; dr_data[1] = "00"; dtAcesSpecSecuEn.Rows.Add(dr_data);
            dr_data = dtAcesSpecSecuEn.NewRow(); dr_data[0] = "所有程序均可访问安全区特殊数据区"; dr_data[1] = "40"; dtAcesSpecSecuEn.Rows.Add(dr_data);
            CfgWord_combobox[42].DataSource = dtAcesSpecSecuEn;
            CfgWord_Mask[42] = 0x40;
            //CW6/////43
            DataTable dtAuthEn = new DataTable();
            dtAuthEn.Columns.Add("Name");
            dtAuthEn.Columns.Add("Value");
            dr_data = dtAuthEn.NewRow(); dr_data[0] = "初始化指令不需要认证"; dr_data[1] = "00"; dtAuthEn.Rows.Add(dr_data);
            dr_data = dtAuthEn.NewRow(); dr_data[0] = "初始化指令需要认证"; dr_data[1] = "80"; dtAuthEn.Rows.Add(dr_data);
            CfgWord_combobox[43].DataSource = dtAuthEn;
            CfgWord_Mask[43] = 0x80;
            //CW7/////43 数据EE扰乱因子低位/////不可选择
            DataTable dtDataEeEncryLowBits = new DataTable();
            dtDataEeEncryLowBits.Columns.Add("Name");
            dtDataEeEncryLowBits.Columns.Add("Value");
            dr_data = dtDataEeEncryLowBits.NewRow(); dr_data[0] = "00"; dr_data[1] = "00"; dtDataEeEncryLowBits.Rows.Add(dr_data);
            CfgWord_combobox[44].DataSource = dtDataEeEncryLowBits;
            CfgWord_Mask[44] = 0xFF;
            //CW8/////44 数据EE扰乱因子高位/////不可选择
            DataTable dtDataEeEncryHighBits = new DataTable();
            dtDataEeEncryHighBits.Columns.Add("Name");
            dtDataEeEncryHighBits.Columns.Add("Value");
            dr_data = dtDataEeEncryHighBits.NewRow(); dr_data[0] = "00"; dr_data[1] = "00"; dtDataEeEncryHighBits.Rows.Add(dr_data);
            CfgWord_combobox[45].DataSource = dtDataEeEncryHighBits;
            CfgWord_Mask[45] = 0xFF;
            //CW9/////45 非接触下dfs最大频率设置
            DataTable dtClMaxDfsCfg = new DataTable();
            dtClMaxDfsCfg.Columns.Add("Name");
            dtClMaxDfsCfg.Columns.Add("Value");
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "1.5MHz"; dr_data[1] = "00"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "2.9MHz"; dr_data[1] = "01"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "4.2MHz"; dr_data[1] = "02"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "5.6MHz"; dr_data[1] = "03"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "6.9MHz"; dr_data[1] = "04"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "8.1MHz"; dr_data[1] = "05"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "9.4MHz"; dr_data[1] = "06"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "10.7MHz"; dr_data[1] = "07"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "11.9MHz"; dr_data[1] = "08"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "13.2MHz"; dr_data[1] = "09"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "14.3MHz"; dr_data[1] = "0A"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "15.5MHz"; dr_data[1] = "0B"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "16.7MHz"; dr_data[1] = "0C"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "17.8MHz"; dr_data[1] = "0D"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "19.0MHz"; dr_data[1] = "0E"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "20.1MHz"; dr_data[1] = "0F"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "21.2MHz"; dr_data[1] = "10"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "22.4MHz"; dr_data[1] = "11"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "23.5MHz"; dr_data[1] = "12"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "24.6MHz"; dr_data[1] = "13"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "25.6MHz"; dr_data[1] = "14"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "26.7MHz"; dr_data[1] = "15"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "27.7MHz"; dr_data[1] = "16"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "28.8MHz"; dr_data[1] = "17"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "29.9MHz"; dr_data[1] = "18"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "30.9MHz"; dr_data[1] = "19"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "31.9MHz"; dr_data[1] = "1A"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "32.9MHz"; dr_data[1] = "1B"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "33.9MHz"; dr_data[1] = "1C"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "35.2MHz"; dr_data[1] = "1D"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "36.3MHz"; dr_data[1] = "1E"; dtClMaxDfsCfg.Rows.Add(dr_data);
            dr_data = dtClMaxDfsCfg.NewRow(); dr_data[0] = "37.1MHz"; dr_data[1] = "1F"; dtClMaxDfsCfg.Rows.Add(dr_data);
            CfgWord_combobox[46].DataSource = dtClMaxDfsCfg;
            CfgWord_Mask[46] = 0x1F;
            //CW9/////46
            DataTable dtClWorkVolt = new DataTable();
            dtClWorkVolt.Columns.Add("Name");
            dtClWorkVolt.Columns.Add("Value");
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "RFU(2.50V)"; dr_data[1] = "00"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "2.50V"; dr_data[1] = "20"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "2.75V"; dr_data[1] = "40"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "3.00V"; dr_data[1] = "60"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "3.25V"; dr_data[1] = "80"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "3.50V"; dr_data[1] = "A0"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "3.75V"; dr_data[1] = "C0"; dtClWorkVolt.Rows.Add(dr_data);
            dr_data = dtClWorkVolt.NewRow(); dr_data[0] = "4.00V"; dr_data[1] = "E0"; dtClWorkVolt.Rows.Add(dr_data);
            CfgWord_combobox[47].DataSource = dtClWorkVolt;
            CfgWord_Mask[47] = 0xE0;
            //CW10/////47
            DataTable dtDataMemSize = new DataTable();
            dtDataMemSize.Columns.Add("Name");
            dtDataMemSize.Columns.Add("Value");
            dr_data = dtDataMemSize.NewRow(); dr_data[0] = "40K"; dr_data[1] = "00"; dtDataMemSize.Rows.Add(dr_data);
            dr_data = dtDataMemSize.NewRow(); dr_data[0] = "80K"; dr_data[1] = "01"; dtDataMemSize.Rows.Add(dr_data);
            dr_data = dtDataMemSize.NewRow(); dr_data[0] = "160K"; dr_data[1] = "02"; dtDataMemSize.Rows.Add(dr_data);
            dr_data = dtDataMemSize.NewRow(); dr_data[0] = "160K"; dr_data[1] = "03"; dtDataMemSize.Rows.Add(dr_data);
            CfgWord_combobox[48].DataSource = dtDataMemSize;
            CfgWord_Mask[48] = 0x03;
            //CW10/////48
            DataTable dtProgMemType = new DataTable();
            dtProgMemType.Columns.Add("Name");
            dtProgMemType.Columns.Add("Value");
            dr_data = dtProgMemType.NewRow(); dr_data[0] = "ROM"; dr_data[1] = "00"; dtProgMemType.Rows.Add(dr_data);
            dr_data = dtProgMemType.NewRow(); dr_data[0] = "EEPROM"; dr_data[1] = "04"; dtProgMemType.Rows.Add(dr_data);
            CfgWord_combobox[49].DataSource = dtProgMemType;
            CfgWord_Mask[49] = 0x04;
            //CW10/////49
            DataTable dtLightDetCfg = new DataTable();
            dtLightDetCfg.Columns.Add("Name");
            dtLightDetCfg.Columns.Add("Value");
            /*dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0000"; dr_data[1] = "00"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0001"; dr_data[1] = "08"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0010"; dr_data[1] = "10"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0011"; dr_data[1] = "18"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0100"; dr_data[1] = "20"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0101"; dr_data[1] = "28"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0110"; dr_data[1] = "30"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "0111"; dr_data[1] = "38"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1000"; dr_data[1] = "40"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1001"; dr_data[1] = "48"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1010"; dr_data[1] = "50"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1011"; dr_data[1] = "58"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1100"; dr_data[1] = "60"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1101"; dr_data[1] = "68"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1110"; dr_data[1] = "70"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "1111"; dr_data[1] = "78"; dtLightDetCfg.Rows.Add(dr_data);*/
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "最低补偿电流"; dr_data[1] = "00"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "较低补偿电流"; dr_data[1] = "18"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "较高补偿电流"; dr_data[1] = "28"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "最高补偿电流"; dr_data[1] = "48"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "00"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "08"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "10"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "20"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "30"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "38"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "40"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "50"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "58"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "60"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "68"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "70"; dtLightDetCfg.Rows.Add(dr_data);
            dr_data = dtLightDetCfg.NewRow(); dr_data[0] = "RFU"; dr_data[1] = "78"; dtLightDetCfg.Rows.Add(dr_data);
            CfgWord_combobox[50].DataSource = dtLightDetCfg;
            CfgWord_Mask[50] = 0x78;
            //CW10/////50
            DataTable dtSecuAutoInvEn = new DataTable();
            dtSecuAutoInvEn.Columns.Add("Name");
            dtSecuAutoInvEn.Columns.Add("Value");
            dr_data = dtSecuAutoInvEn.NewRow(); dr_data[0] = "手动写入安全区校验码"; dr_data[1] = "00"; dtSecuAutoInvEn.Rows.Add(dr_data);
            dr_data = dtSecuAutoInvEn.NewRow(); dr_data[0] = "自动写入安全区校验码"; dr_data[1] = "80"; dtSecuAutoInvEn.Rows.Add(dr_data);
            CfgWord_combobox[51].DataSource = dtSecuAutoInvEn;
            CfgWord_Mask[51] = 0x80;
            //CW11/////51
            DataTable dtDemoLvlSel = new DataTable();
            dtDemoLvlSel.Columns.Add("Name");
            dtDemoLvlSel.Columns.Add("Value");
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "000"; dr_data[1] = "00"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "001"; dr_data[1] = "01"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "010"; dr_data[1] = "02"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "011"; dr_data[1] = "03"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "100"; dr_data[1] = "04"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "101"; dr_data[1] = "05"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "110"; dr_data[1] = "06"; dtDemoLvlSel.Rows.Add(dr_data);
            dr_data = dtDemoLvlSel.NewRow(); dr_data[0] = "111"; dr_data[1] = "07"; dtDemoLvlSel.Rows.Add(dr_data);
            CfgWord_combobox[52].DataSource = dtDemoLvlSel;
            CfgWord_Mask[52] = 0x07;
            //CW11/////52
            DataTable dtLibSizeSel = new DataTable();
            dtLibSizeSel.Columns.Add("Name");
            dtLibSizeSel.Columns.Add("Value");
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "0K"; dr_data[1] = "00"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "2K"; dr_data[1] = "08"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "4K"; dr_data[1] = "10"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "6K"; dr_data[1] = "18"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "8K"; dr_data[1] = "20"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "10K"; dr_data[1] = "28"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "12K"; dr_data[1] = "30"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "14K"; dr_data[1] = "38"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "16K"; dr_data[1] = "40"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "18K"; dr_data[1] = "48"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "20K"; dr_data[1] = "50"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "22K"; dr_data[1] = "58"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "24K"; dr_data[1] = "60"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "26K"; dr_data[1] = "68"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "28K"; dr_data[1] = "70"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "30K"; dr_data[1] = "78"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "32K"; dr_data[1] = "80"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "34K"; dr_data[1] = "88"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "36K"; dr_data[1] = "90"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "38K"; dr_data[1] = "98"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "40K"; dr_data[1] = "A0"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "42K"; dr_data[1] = "A8"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "44K"; dr_data[1] = "B0"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "46K"; dr_data[1] = "B8"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "48K"; dr_data[1] = "C0"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "50K"; dr_data[1] = "C8"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "52K"; dr_data[1] = "D0"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "54K"; dr_data[1] = "D8"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "56K"; dr_data[1] = "E0"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "58K"; dr_data[1] = "E8"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "60K"; dr_data[1] = "F0"; dtLibSizeSel.Rows.Add(dr_data);
            dr_data = dtLibSizeSel.NewRow(); dr_data[0] = "62K"; dr_data[1] = "F8"; dtLibSizeSel.Rows.Add(dr_data);
            CfgWord_combobox[53].DataSource = dtLibSizeSel;
            CfgWord_Mask[53] = 0xF8;
            //CW12/////53
            DataTable dtLowFreqEeAccCfg = new DataTable();
            dtLowFreqEeAccCfg.Columns.Add("Name");
            dtLowFreqEeAccCfg.Columns.Add("Value");
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "000"; dr_data[1] = "00"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "001"; dr_data[1] = "01"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "010"; dr_data[1] = "02"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "011"; dr_data[1] = "03"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "100"; dr_data[1] = "04"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "101"; dr_data[1] = "05"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "110"; dr_data[1] = "06"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtLowFreqEeAccCfg.NewRow(); dr_data[0] = "111"; dr_data[1] = "07"; dtLowFreqEeAccCfg.Rows.Add(dr_data);
            CfgWord_combobox[54].DataSource = dtLowFreqEeAccCfg;
            CfgWord_Mask[54] = 0x07;
            //CW12/////54
            DataTable dtHighFreqEeAccCfg = new DataTable();
            dtHighFreqEeAccCfg.Columns.Add("Name");
            dtHighFreqEeAccCfg.Columns.Add("Value");
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "000"; dr_data[1] = "00"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "001"; dr_data[1] = "08"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "010"; dr_data[1] = "10"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "011"; dr_data[1] = "18"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "100"; dr_data[1] = "20"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "101"; dr_data[1] = "28"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "110"; dr_data[1] = "30"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            dr_data = dtHighFreqEeAccCfg.NewRow(); dr_data[0] = "111"; dr_data[1] = "38"; dtHighFreqEeAccCfg.Rows.Add(dr_data);
            CfgWord_combobox[55].DataSource = dtHighFreqEeAccCfg;
            CfgWord_Mask[55] = 0x38;
            //CW12/////55
            DataTable dtDigitVoltCfg = new DataTable();
            dtDigitVoltCfg.Columns.Add("Name");
            dtDigitVoltCfg.Columns.Add("Value");
            dr_data = dtDigitVoltCfg.NewRow(); dr_data[0] = "1.5V"; dr_data[1] = "00"; dtDigitVoltCfg.Rows.Add(dr_data);
            dr_data = dtDigitVoltCfg.NewRow(); dr_data[0] = "1.0V"; dr_data[1] = "40"; dtDigitVoltCfg.Rows.Add(dr_data);
            dr_data = dtDigitVoltCfg.NewRow(); dr_data[0] = "1.2V"; dr_data[1] = "80"; dtDigitVoltCfg.Rows.Add(dr_data);
            dr_data = dtDigitVoltCfg.NewRow(); dr_data[0] = "0.8V"; dr_data[1] = "C0"; dtDigitVoltCfg.Rows.Add(dr_data);
            CfgWord_combobox[56].DataSource = dtDigitVoltCfg;
            CfgWord_Mask[56] = 0xC0;
            //CW13/////56
            DataTable dtPatchSize = new DataTable();
            dtPatchSize.Columns.Add("Name");
            dtPatchSize.Columns.Add("Value");
            dr_data = dtPatchSize.NewRow(); dr_data[0] = "00"; dr_data[1] = "00"; dtPatchSize.Rows.Add(dr_data);
            CfgWord_combobox[57].DataSource = dtPatchSize;
            CfgWord_Mask[57] = 0x7F;

            //CW13/////57
            temp = 0;
            DataTable dtS2cModeSel = new DataTable();
            dtS2cModeSel.Columns.Add("Name");
            dtS2cModeSel.Columns.Add("Value");
            dr_data = dtS2cModeSel.NewRow(); dr_data[0] = "适合FM295通讯"; dr_data[1] = "00"; dtS2cModeSel.Rows.Add(dr_data);
            dr_data = dtS2cModeSel.NewRow(); dr_data[0] = "适合标准S2C"; dr_data[1] = "80"; dtS2cModeSel.Rows.Add(dr_data);
            CfgWord_combobox[58].DataSource = dtS2cModeSel;
            CfgWord_Mask[58] = 0x80;

            //CW14/////58
            DataTable dtRamEncryEn = new DataTable();
            dtRamEncryEn.Columns.Add("Name");
            dtRamEncryEn.Columns.Add("Value");
            dr_data = dtRamEncryEn.NewRow(); dr_data[0] = "不加密"; dr_data[1] = "00"; dtRamEncryEn.Rows.Add(dr_data);
            dr_data = dtRamEncryEn.NewRow(); dr_data[0] = "加密"; dr_data[1] = "01"; dtRamEncryEn.Rows.Add(dr_data);
            CfgWord_combobox[59 - temp].DataSource = dtRamEncryEn;
            CfgWord_Mask[59 - temp] = 0x01;
            //CW14/////59
            DataTable dtDataMenEncryEn = new DataTable();
            dtDataMenEncryEn.Columns.Add("Name");
            dtDataMenEncryEn.Columns.Add("Value");
            dr_data = dtDataMenEncryEn.NewRow(); dr_data[0] = "不加密"; dr_data[1] = "00"; dtDataMenEncryEn.Rows.Add(dr_data);
            dr_data = dtDataMenEncryEn.NewRow(); dr_data[0] = "加密"; dr_data[1] = "02"; dtDataMenEncryEn.Rows.Add(dr_data);
            CfgWord_combobox[60 - temp].DataSource = dtDataMenEncryEn;
            CfgWord_Mask[60 - temp] = 0x02;
            //CW14/////60
            DataTable dtProgMenEncryEn = new DataTable();
            dtProgMenEncryEn.Columns.Add("Name");
            dtProgMenEncryEn.Columns.Add("Value");
            dr_data = dtProgMenEncryEn.NewRow(); dr_data[0] = "不加密"; dr_data[1] = "00"; dtProgMenEncryEn.Rows.Add(dr_data);
            dr_data = dtProgMenEncryEn.NewRow(); dr_data[0] = "加密"; dr_data[1] = "04"; dtProgMenEncryEn.Rows.Add(dr_data);
            CfgWord_combobox[61 - temp].DataSource = dtProgMenEncryEn;
            CfgWord_Mask[61 - temp] = 0x04;
            //CW14/////61
            DataTable dtDataMenEncryCfg = new DataTable();
            dtDataMenEncryCfg.Columns.Add("Name");
            dtDataMenEncryCfg.Columns.Add("Value");
            dr_data = dtDataMenEncryCfg.NewRow(); dr_data[0] = "1轮加密"; dr_data[1] = "00"; dtDataMenEncryCfg.Rows.Add(dr_data);
            dr_data = dtDataMenEncryCfg.NewRow(); dr_data[0] = "2轮加密"; dr_data[1] = "08"; dtDataMenEncryCfg.Rows.Add(dr_data);
            dr_data = dtDataMenEncryCfg.NewRow(); dr_data[0] = "3轮加密"; dr_data[1] = "10"; dtDataMenEncryCfg.Rows.Add(dr_data);
            dr_data = dtDataMenEncryCfg.NewRow(); dr_data[0] = "4轮加密"; dr_data[1] = "18"; dtDataMenEncryCfg.Rows.Add(dr_data);
            CfgWord_combobox[62 - temp].DataSource = dtDataMenEncryCfg;
            CfgWord_Mask[62 - temp] = 0x18;
            //CW14/////63
            DataTable dtEeRamParityChkEn = new DataTable();
            dtEeRamParityChkEn.Columns.Add("Name");
            dtEeRamParityChkEn.Columns.Add("Value");
            dr_data = dtEeRamParityChkEn.NewRow(); dr_data[0] = "不校验"; dr_data[1] = "00"; dtEeRamParityChkEn.Rows.Add(dr_data);
            dr_data = dtEeRamParityChkEn.NewRow(); dr_data[0] = "校验"; dr_data[1] = "20"; dtEeRamParityChkEn.Rows.Add(dr_data);
            CfgWord_combobox[63 - temp].DataSource = dtEeRamParityChkEn;
            CfgWord_Mask[63 - temp] = 0x20;
            //CW14/////64
            DataTable dtRomParityChkEn = new DataTable();
            dtRomParityChkEn.Columns.Add("Name");
            dtRomParityChkEn.Columns.Add("Value");
            dr_data = dtRomParityChkEn.NewRow(); dr_data[0] = "不校验"; dr_data[1] = "00"; dtRomParityChkEn.Rows.Add(dr_data);
            dr_data = dtRomParityChkEn.NewRow(); dr_data[0] = "校验"; dr_data[1] = "40"; dtRomParityChkEn.Rows.Add(dr_data);
            CfgWord_combobox[64 - temp].DataSource = dtRomParityChkEn;
            CfgWord_Mask[64 - temp] = 0x40;
            //CW14/////65
            DataTable dtSboxNumber = new DataTable();
            dtSboxNumber.Columns.Add("Name");
            dtSboxNumber.Columns.Add("Value");
            dr_data = dtSboxNumber.NewRow(); dr_data[0] = "2个"; dr_data[1] = "00"; dtSboxNumber.Rows.Add(dr_data);
            dr_data = dtSboxNumber.NewRow(); dr_data[0] = "1个"; dr_data[1] = "80"; dtSboxNumber.Rows.Add(dr_data);
            CfgWord_combobox[65 - temp].DataSource = dtSboxNumber;
            CfgWord_Mask[65 - temp] = 0x80;

            //CW15/////66
            DataTable dtVDD15Trim = new DataTable();
            dtVDD15Trim.Columns.Add("Name");
            dtVDD15Trim.Columns.Add("Value");
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "000"; dr_data[1] = "00"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "001"; dr_data[1] = "01"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "010"; dr_data[1] = "02"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "011"; dr_data[1] = "03"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "100"; dr_data[1] = "04"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "101"; dr_data[1] = "05"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "110"; dr_data[1] = "06"; dtVDD15Trim.Rows.Add(dr_data);
            dr_data = dtVDD15Trim.NewRow(); dr_data[0] = "111"; dr_data[1] = "07"; dtVDD15Trim.Rows.Add(dr_data);
            CfgWord_combobox[66 - temp].DataSource = dtVDD15Trim;
            CfgWord_Mask[66 - temp] = 0x07;

            temp = 0;

            //CW16/////67
            DataTable dtDfsFreqTrim = new DataTable();
            dtDfsFreqTrim.Columns.Add("Name");
            dtDfsFreqTrim.Columns.Add("Value");
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0000"; dr_data[1] = "00"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0001"; dr_data[1] = "01"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0010"; dr_data[1] = "02"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0011"; dr_data[1] = "03"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0100"; dr_data[1] = "04"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0101"; dr_data[1] = "05"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0110"; dr_data[1] = "06"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "0111"; dr_data[1] = "07"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1000"; dr_data[1] = "08"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1001"; dr_data[1] = "09"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1010"; dr_data[1] = "0A"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1011"; dr_data[1] = "0B"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1100"; dr_data[1] = "0C"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1101"; dr_data[1] = "0D"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1110"; dr_data[1] = "0E"; dtDfsFreqTrim.Rows.Add(dr_data);
            dr_data = dtDfsFreqTrim.NewRow(); dr_data[0] = "1111"; dr_data[1] = "0F"; dtDfsFreqTrim.Rows.Add(dr_data);
            CfgWord_combobox[67].DataSource = dtDfsFreqTrim;
            CfgWord_Mask[67] = 0x0F;


            //CW16/////68
            temp = 0;
            DataTable dtOsc4MTrim = new DataTable();
            dtOsc4MTrim.Columns.Add("Name");
            dtOsc4MTrim.Columns.Add("Value");
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "000"; dr_data[1] = "00"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "001"; dr_data[1] = "10"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "010"; dr_data[1] = "20"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "011"; dr_data[1] = "30"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "100"; dr_data[1] = "40"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "101"; dr_data[1] = "50"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "110"; dr_data[1] = "60"; dtOsc4MTrim.Rows.Add(dr_data);
            dr_data = dtOsc4MTrim.NewRow(); dr_data[0] = "111"; dr_data[1] = "70"; dtOsc4MTrim.Rows.Add(dr_data);
            CfgWord_combobox[68].DataSource = dtOsc4MTrim;
            CfgWord_Mask[68] = 0x70;

            //CW17/////69
            DataTable dtSwpCurrtTrim = new DataTable();
            dtSwpCurrtTrim.Columns.Add("Name");
            dtSwpCurrtTrim.Columns.Add("Value");
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0000"; dr_data[1] = "00"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0001"; dr_data[1] = "01"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0010"; dr_data[1] = "02"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0011"; dr_data[1] = "03"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0100"; dr_data[1] = "04"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0101"; dr_data[1] = "05"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0110"; dr_data[1] = "06"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "0111"; dr_data[1] = "07"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1000"; dr_data[1] = "08"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1001"; dr_data[1] = "09"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1010"; dr_data[1] = "0A"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1011"; dr_data[1] = "0B"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1100"; dr_data[1] = "0C"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1101"; dr_data[1] = "0D"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1110"; dr_data[1] = "0E"; dtSwpCurrtTrim.Rows.Add(dr_data);
            dr_data = dtSwpCurrtTrim.NewRow(); dr_data[0] = "1111"; dr_data[1] = "0F"; dtSwpCurrtTrim.Rows.Add(dr_data);
            CfgWord_combobox[69].DataSource = dtSwpCurrtTrim;
            CfgWord_Mask[69] = 0x0F;
            //CW17/////70
            DataTable dtVDD15_2Trim = new DataTable();
            dtVDD15_2Trim.Columns.Add("Name");
            dtVDD15_2Trim.Columns.Add("Value");
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "000"; dr_data[1] = "00"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "001"; dr_data[1] = "10"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "010"; dr_data[1] = "20"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "011"; dr_data[1] = "30"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "100"; dr_data[1] = "40"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "101"; dr_data[1] = "50"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "110"; dr_data[1] = "60"; dtVDD15_2Trim.Rows.Add(dr_data);
            dr_data = dtVDD15_2Trim.NewRow(); dr_data[0] = "111"; dr_data[1] = "70"; dtVDD15_2Trim.Rows.Add(dr_data);
            CfgWord_combobox[70].DataSource = dtVDD15_2Trim;
            CfgWord_Mask[70] = 0x70;
            //CW18/////71
            DataTable dtClCpuWorkMode = new DataTable();
            dtClCpuWorkMode.Columns.Add("Name");
            dtClCpuWorkMode.Columns.Add("Value");
            dr_data = dtClCpuWorkMode.NewRow(); dr_data[0] = "非接收发停CPU，电源供电时不停"; dr_data[1] = "00"; dtClCpuWorkMode.Rows.Add(dr_data);
            dr_data = dtClCpuWorkMode.NewRow(); dr_data[0] = "强制非接收发运算时停CPU"; dr_data[1] = "01"; dtClCpuWorkMode.Rows.Add(dr_data);
            dr_data = dtClCpuWorkMode.NewRow(); dr_data[0] = "强制非接收发运算时不停CPU"; dr_data[1] = "02"; dtClCpuWorkMode.Rows.Add(dr_data);
            dr_data = dtClCpuWorkMode.NewRow(); dr_data[0] = "非接收发时有调制信号时停时钟，未调制时有系统时钟。"; dr_data[1] = "03"; dtClCpuWorkMode.Rows.Add(dr_data);
            CfgWord_combobox[71].DataSource = dtClCpuWorkMode;
            CfgWord_Mask[71] = 0x03;
            //CW18/////72
            DataTable dtDemoModeSel = new DataTable();
            dtDemoModeSel.Columns.Add("Name");
            dtDemoModeSel.Columns.Add("Value");
            dr_data = dtDemoModeSel.NewRow(); dr_data[0] = "解调电路0"; dr_data[1] = "00"; dtDemoModeSel.Rows.Add(dr_data);
            dr_data = dtDemoModeSel.NewRow(); dr_data[0] = "解调电路1"; dr_data[1] = "04"; dtDemoModeSel.Rows.Add(dr_data);
            CfgWord_combobox[72].DataSource = dtDemoModeSel;
            CfgWord_Mask[72] = 0x04;
            //CW18/////73
            DataTable dtShieldSel = new DataTable();
            dtShieldSel.Columns.Add("Name");
            dtShieldSel.Columns.Add("Value");
            dr_data = dtShieldSel.NewRow(); dr_data[0] = "400Khz的时钟频率"; dr_data[1] = "00"; dtShieldSel.Rows.Add(dr_data);
            dr_data = dtShieldSel.NewRow(); dr_data[0] = "100Khz的时钟频率"; dr_data[1] = "08"; dtShieldSel.Rows.Add(dr_data);
            dr_data = dtShieldSel.NewRow(); dr_data[0] = "50Khz的时钟频率"; dr_data[1] = "10"; dtShieldSel.Rows.Add(dr_data);
            dr_data = dtShieldSel.NewRow(); dr_data[0] = "400Khz的时钟频率"; dr_data[1] = "18"; dtShieldSel.Rows.Add(dr_data);
            CfgWord_combobox[73].DataSource = dtShieldSel;
            CfgWord_Mask[73] = 0x18;
            temp = 0;

            CfgWord_Mask[74 + temp] = 0x01;
            CfgWord_Mask[75 + temp] = 0x02;
            CfgWord_Mask[76 + temp] = 0x04;
            CfgWord_Mask[77 + temp] = 0x08;
            CfgWord_Mask[78 + temp] = 0x10;
            CfgWord_Mask[79 + temp] = 0x20;
            CfgWord_Mask[80 + temp] = 0x40;
            CfgWord_Mask[81 + temp] = 0x80;

            CfgWord_Mask[82 + temp] = 0x01;
            CfgWord_Mask[83 + temp] = 0x02;
            CfgWord_Mask[84 + temp] = 0x04;
            CfgWord_Mask[85 + temp] = 0x08;
            CfgWord_Mask[86 + temp] = 0x10;
            CfgWord_Mask[87 + temp] = 0x20;
            CfgWord_Mask[88 + temp] = 0x40;
            CfgWord_Mask[89 + temp] = 0x80;

            CfgWord_Mask[90 + temp] = 0x01;
            CfgWord_Mask[91 + temp] = 0x02;
            CfgWord_Mask[92 + temp] = 0x04;
            CfgWord_Mask[93 + temp] = 0x08;
            CfgWord_Mask[94 + temp] = 0x10;
            CfgWord_Mask[95 + temp] = 0x20;
            CfgWord_Mask[96 + temp] = 0x40;
            CfgWord_Mask[97 + temp] = 0x80;

            CfgWord_Mask[98 + temp] = 0x01;
            CfgWord_Mask[99 + temp] = 0x02;
            CfgWord_Mask[100 + temp] = 0x04;
            CfgWord_Mask[101 + temp] = 0x08; ;
            CfgWord_Mask[102 + temp] = 0x10;
            CfgWord_Mask[103 + temp] = 0x20;
            CfgWord_Mask[104 + temp] = 0x40;
            CfgWord_Mask[105 + temp] = 0x80;

            CfgWord_Mask[106 + temp] = 0x01;
            CfgWord_Mask[107 + temp] = 0x02;
            CfgWord_Mask[108 + temp] = 0x04;
            CfgWord_Mask[109 + temp] = 0x08;
            CfgWord_Mask[110 + temp] = 0x10;
            CfgWord_Mask[111 + temp] = 0x20;
            CfgWord_Mask[112 + temp] = 0x40;
            CfgWord_Mask[113 + temp] = 0x80;

            CfgWord_Mask[114 + temp] = 0x01;
            CfgWord_Mask[115 + temp] = 0x02;
            CfgWord_Mask[116 + temp] = 0x04;
            CfgWord_Mask[117 + temp] = 0x08;
            CfgWord_Mask[118 + temp] = 0x10;
            CfgWord_Mask[119 + temp] = 0x20;
            CfgWord_Mask[120 + temp] = 0x40;
            CfgWord_Mask[121 + temp] = 0x80;
            /*
                        //LW1/////72
                        DataTable dtlockWord_1 = new DataTable();
                        dtlockWord_1.Columns.Add("Name");
                        dtlockWord_1.Columns.Add("Value");
                        dr_data = dtlockWord_1.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_1.Rows.Add(dr_data);
                        dr_data = dtlockWord_1.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "01"; dtlockWord_1.Rows.Add(dr_data);

                        DataTable dtlockWord_2 = new DataTable();
                        dtlockWord_2.Columns.Add("Name");
                        dtlockWord_2.Columns.Add("Value");
                        dr_data = dtlockWord_2.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_2.Rows.Add(dr_data);
                        dr_data = dtlockWord_2.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "02"; dtlockWord_2.Rows.Add(dr_data);

                        DataTable dtlockWord_3 = new DataTable();
                        dtlockWord_3.Columns.Add("Name");
                        dtlockWord_3.Columns.Add("Value");
                        dr_data = dtlockWord_3.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_3.Rows.Add(dr_data);
                        dr_data = dtlockWord_3.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "04"; dtlockWord_3.Rows.Add(dr_data);

                        DataTable dtlockWord_4 = new DataTable();
                        dtlockWord_4.Columns.Add("Name");
                        dtlockWord_4.Columns.Add("Value");
                        dr_data = dtlockWord_4.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_4.Rows.Add(dr_data);
                        dr_data = dtlockWord_4.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "08"; dtlockWord_4.Rows.Add(dr_data);

                        DataTable dtlockWord_5 = new DataTable();
                        dtlockWord_5.Columns.Add("Name");
                        dtlockWord_5.Columns.Add("Value");
                        dr_data = dtlockWord_5.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_5.Rows.Add(dr_data);
                        dr_data = dtlockWord_5.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "10"; dtlockWord_5.Rows.Add(dr_data);

                        DataTable dtlockWord_6 = new DataTable();
                        dtlockWord_6.Columns.Add("Name");
                        dtlockWord_6.Columns.Add("Value");
                        dr_data = dtlockWord_6.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_6.Rows.Add(dr_data);
                        dr_data = dtlockWord_6.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "20"; dtlockWord_6.Rows.Add(dr_data);

                        DataTable dtlockWord_7 = new DataTable();
                        dtlockWord_7.Columns.Add("Name");
                        dtlockWord_7.Columns.Add("Value");
                        dr_data = dtlockWord_7.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_7.Rows.Add(dr_data);
                        dr_data = dtlockWord_7.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "40"; dtlockWord_7.Rows.Add(dr_data);

                        DataTable dtlockWord_8 = new DataTable();
                        dtlockWord_8.Columns.Add("Name");
                        dtlockWord_8.Columns.Add("Value");
                        dr_data = dtlockWord_8.NewRow(); dr_data[0] = "可写  "; dr_data[1] = "00"; dtlockWord_8.Rows.Add(dr_data);
                        dr_data = dtlockWord_8.NewRow(); dr_data[0] = "不可写"; dr_data[1] = "80"; dtlockWord_8.Rows.Add(dr_data);

                        //LW0/////72~79
                        CfgWord_combobox[72].DataSource = dtlockWord_1;
                        CfgWord_combobox[73].DataSource = dtlockWord_2;
                        CfgWord_combobox[74].DataSource = dtlockWord_3;
                        CfgWord_combobox[75].DataSource = dtlockWord_4;
                        CfgWord_combobox[76].DataSource = dtlockWord_5;
                        CfgWord_combobox[77].DataSource = dtlockWord_6;
                        CfgWord_combobox[78].DataSource = dtlockWord_7;
                        CfgWord_combobox[79].DataSource = dtlockWord_8;

                        //LW1/////80~87
                        CfgWord_combobox[80].DataSource = dtlockWord_1;
                        CfgWord_combobox[81].DataSource = dtlockWord_2;
                        CfgWord_combobox[82].DataSource = dtlockWord_3;
                        CfgWord_combobox[83].DataSource = dtlockWord_4;
                        CfgWord_combobox[84].DataSource = dtlockWord_5;
                        CfgWord_combobox[85].DataSource = dtlockWord_6;
                        CfgWord_combobox[86].DataSource = dtlockWord_7;
                        CfgWord_combobox[87].DataSource = dtlockWord_8;

                        //LW2/////88~95
                        CfgWord_combobox[88].DataSource = dtlockWord_1;
                        CfgWord_combobox[89].DataSource = dtlockWord_2;
                        CfgWord_combobox[90].DataSource = dtlockWord_3;
                        CfgWord_combobox[91].DataSource = dtlockWord_4;
                        CfgWord_combobox[92].DataSource = dtlockWord_5;
                        CfgWord_combobox[93].DataSource = dtlockWord_6;
                        CfgWord_combobox[94].DataSource = dtlockWord_7;
                        CfgWord_combobox[95].DataSource = dtlockWord_8;

                        //LW3/////96~103
                        CfgWord_combobox[96].DataSource = dtlockWord_1;
                        CfgWord_combobox[97].DataSource = dtlockWord_2;
                        CfgWord_combobox[98].DataSource = dtlockWord_3;
                        CfgWord_combobox[99].DataSource = dtlockWord_4;
                        CfgWord_combobox[100].DataSource = dtlockWord_5;
                        CfgWord_combobox[101].DataSource = dtlockWord_6;
                        CfgWord_combobox[102].DataSource = dtlockWord_7;
                        CfgWord_combobox[103].DataSource = dtlockWord_8;
                             
                        //LW4/////104~111
                        CfgWord_combobox[104].DataSource = dtlockWord_1;
                        CfgWord_combobox[105].DataSource = dtlockWord_2;
                        CfgWord_combobox[106].DataSource = dtlockWord_3;
                        CfgWord_combobox[107].DataSource = dtlockWord_4;
                        CfgWord_combobox[108].DataSource = dtlockWord_5;
                        CfgWord_combobox[109].DataSource = dtlockWord_6;
                        CfgWord_combobox[110].DataSource = dtlockWord_7;
                        CfgWord_combobox[111].DataSource = dtlockWord_8;
                             
                        //LW5/////112~118
                        CfgWord_combobox[112].DataSource = dtlockWord_1;
                        CfgWord_combobox[113].DataSource = dtlockWord_2;
                        CfgWord_combobox[114].DataSource = dtlockWord_3;
                        CfgWord_combobox[115].DataSource = dtlockWord_4;
                        CfgWord_combobox[116].DataSource = dtlockWord_5;
                        CfgWord_combobox[117].DataSource = dtlockWord_6;
                        CfgWord_combobox[118].DataSource = dtlockWord_7;

                        //MW_RFU//119~122
                        DataTable dtModeWordRFU = new DataTable();
                        dtModeWordRFU.Columns.Add("Name");
                        dtModeWordRFU.Columns.Add("Value");
                        dr_data = dtModeWordRFU.NewRow(); dr_data[0] = "00"; dr_data[1] = "00"; dtModeWordRFU.Rows.Add(dr_data);
                        CfgWord_combobox[119].DataSource = dtModeWordRFU;
                        CfgWord_combobox[120].DataSource = dtModeWordRFU;
                        CfgWord_combobox[121].DataSource = dtModeWordRFU;
                        CfgWord_combobox[122].DataSource = dtModeWordRFU;

                        //MW4~5//123
                        DataTable dtProgValue = new DataTable();
                        dtProgValue.Columns.Add("Name");
                        dtProgValue.Columns.Add("Value");
                        dr_data = dtProgValue.NewRow(); dr_data[0] = "55"; dr_data[1] = "55"; dtProgValue.Rows.Add(dr_data);
                        CfgWord_combobox[123].DataSource = dtProgValue;

                        //MW4~5//124
                        DataTable dtUserValue = new DataTable();
                        dtUserValue.Columns.Add("Name");
                        dtUserValue.Columns.Add("Value");
                        dr_data = dtUserValue.NewRow(); dr_data[0] = "00"; dr_data[1] = "00"; dtUserValue.Rows.Add(dr_data);
                        CfgWord_combobox[124].DataSource = dtUserValue;

                        //MW4~5//123
                        DataTable dtModeEn = new DataTable();
                        dtModeEn.Columns.Add("Name");
                        dtModeEn.Columns.Add("Value");
                        dr_data = dtModeEn.NewRow(); dr_data[0] = "A5"; dr_data[1] = "A5"; dtModeEn.Rows.Add(dr_data);
                        CfgWord_combobox[125].DataSource = dtModeEn;
                        */
        }

        private void SetDefaultCfg()
        {
            ///////////Config word 0//////////////////////////////
            CfgWord_combobox[0].SelectedIndex = 0;//UID_level1
            CfgWord_combobox[1].SelectedIndex = 1;//CL ensable
            CfgWord_combobox[2].SelectedIndex = 2;//typeA+typeB
            CfgWord_combobox[3].SelectedIndex = 0;//整体扩展
            CfgWord_combobox[4].SelectedIndex = 0;//EEPROM
            CfgWord_combobox[5].SelectedIndex = 1;//CPU+逻辑加密卡
            ///////////Config word 1//////////////////////////////
            CfgWord_combobox[6].SelectedIndex = 0;//普通逻辑加密卡
            CfgWord_combobox[7].SelectedIndex = 0;//MIFARE
            CfgWord_combobox[8].SelectedIndex = 0;//1K
            CfgWord_combobox[9].SelectedIndex = 0;//无扩展
            CfgWord_combobox[10].SelectedIndex = 0;//不区分SM、AM
            ///////////Config word 2//////////////////////////////
            CfgWord_combobox[11].SelectedIndex = 0;//频率检测关闭
            CfgWord_combobox[12].SelectedIndex = 0;//光检测关闭
            CfgWord_combobox[13].SelectedIndex = 0;//屏蔽层检测关闭
            CfgWord_combobox[14].SelectedIndex = 0;//温度检测关闭
            CfgWord_combobox[15].SelectedIndex = 0;//glitch检测关闭
            CfgWord_combobox[16].SelectedIndex = 1;//安全检测不产生复位
            CfgWord_combobox[17].SelectedIndex = 1;//静态Shield
            CfgWord_combobox[18].SelectedIndex = 0;//CT接口不停止CPU时钟
            ///////////Config word 3//////////////////////////////
            CfgWord_combobox[19].SelectedIndex = 0;//低温检测配置000
            CfgWord_combobox[20].SelectedIndex = 0;//高温检测配置000
            CfgWord_combobox[21].SelectedIndex = 0;//ATQA、SAK来自EEPROM
            CfgWord_combobox[22].SelectedIndex = 0;//禁止使用用户UID
            ///////////Config word 4//////////////////////////////
            CfgWord_combobox[23].SelectedIndex = 2;//4MHz
            CfgWord_combobox[24].SelectedIndex = 0;//一个galois
            CfgWord_combobox[25].SelectedIndex = 0;//禁用Fibonacci
            CfgWord_combobox[26].SelectedIndex = 1;//初始化指令有效
            CfgWord_combobox[27].SelectedIndex = 1;//初始化指令可以读encry_lib外的ROM
            CfgWord_combobox[28].SelectedIndex = 1;//初始化指令可以读取encry_lib
            ///////////Config word 5//////////////////////////////
            CfgWord_combobox[29].SelectedIndex = 1;//CT有效
            CfgWord_combobox[30].SelectedIndex = 0;//SPI接口关闭
            CfgWord_combobox[31].SelectedIndex = 0;//I2C接口关闭
            CfgWord_combobox[32].SelectedIndex = 0;//UART接口关闭
            CfgWord_combobox[33].SelectedIndex = 0;//SWP接口关闭
            CfgWord_combobox[34].SelectedIndex = 0;//ESWP接口关闭
            CfgWord_combobox[35].SelectedIndex = 0;//S2C接口关闭
            ///////////Config word 6//////////////////////////////
            CfgWord_combobox[36].SelectedIndex = 1;//支持RAE运算
            CfgWord_combobox[37].SelectedIndex = 1;//支持DES运算
            CfgWord_combobox[38].SelectedIndex = 1;//支持AES运算
            CfgWord_combobox[39].SelectedIndex = 1;//支持SSF33运算
            CfgWord_combobox[40].SelectedIndex = 1;//支持SM1运算
            CfgWord_combobox[41].SelectedIndex = 1;//支持SM7运算
            CfgWord_combobox[42].SelectedIndex = 1;//安全区特殊数据区访问权限定义
            CfgWord_combobox[43].SelectedIndex = 0;//初始化指令不需要认证
            ///////////Config word 7//////////////////////////////
            CfgWord_combobox[44].SelectedIndex = 0;//数据EE扰乱因子低位
            ///////////Config word 8//////////////////////////////
            CfgWord_combobox[45].SelectedIndex = 0;//数据EE扰乱因子高位
            ///////////Config word 9//////////////////////////////
            CfgWord_combobox[46].SelectedIndex = 03;//非接触dfs最大时钟频率
            CfgWord_combobox[47].SelectedIndex = 6;//非接触工作电压
            ///////////Config word 10//////////////////////////////
            CfgWord_combobox[48].SelectedIndex = 1;//80K
            CfgWord_combobox[49].SelectedIndex = 1;//EEPROM
            CfgWord_combobox[50].SelectedIndex = 0;//0000
            CfgWord_combobox[51].SelectedIndex = 1;//自动写入校验码
            ///////////Config word 11//////////////////////////////
            CfgWord_combobox[52].SelectedIndex = 0;//000
            CfgWord_combobox[53].SelectedIndex = 0;//没有lib区
            ///////////Config word 12//////////////////////////////
            CfgWord_combobox[54].SelectedIndex = 0;//000
            CfgWord_combobox[55].SelectedIndex = 0;//000
            CfgWord_combobox[56].SelectedIndex = 0;//1.5V
            ///////////Config word 13//////////////////////////////
            CfgWord_combobox[57].SelectedIndex = 0;//没有Patch区
            CfgWord_combobox[58].SelectedIndex = 1;//标准S2C
            ///////////Config word 14//////////////////////////////
            CfgWord_combobox[59].SelectedIndex = 0;//Ram不加密
            CfgWord_combobox[60].SelectedIndex = 0;//数据存储器不加密
            CfgWord_combobox[61].SelectedIndex = 0;//程序存储器不加密
            CfgWord_combobox[62].SelectedIndex = 0;//数据存储器加密1轮加密
            CfgWord_combobox[63].SelectedIndex = 0;//EEPROM/RAM不校验
            CfgWord_combobox[64].SelectedIndex = 0;//ROM奇偶校验不校验
            CfgWord_combobox[65].SelectedIndex = 0;//加密的Sbox个数：2个
            ///////////Config word 15//////////////////////////////
            CfgWord_combobox[66].SelectedIndex = 0;//vdd15trim            
            ///////////Config word 16//////////////////////////////
            CfgWord_combobox[67].SelectedIndex = 0;//dfs_trim
            CfgWord_combobox[68].SelectedIndex = 0;//osc4M_trim
            ///////////Config word 17//////////////////////////////
            CfgWord_combobox[69].SelectedIndex = 0;//swp_current_trim
            CfgWord_combobox[70].SelectedIndex = 0;//vdd15_2trim
            ///////////Config word 18//////////////////////////////
            CfgWord_combobox[71].SelectedIndex = 0;//强制停CPU时钟
            CfgWord_combobox[72].SelectedIndex = 0;//公司原有解调电路
            CfgWord_combobox[73].SelectedIndex = 0;//400Khz的时钟频率
            ///////////Lock word 0~5 ///////////////均默认可写  



            initVal_done = true;
            for (int i = 0; i < this.CfgWord_dataGridView.Rows.Count; i++)
            {
                if (i < 74)
                {
                    if (CfgWord_combobox[i].SelectedValue == null)
                        break;
                    CfgWord_dataGridView[1, i].Value = CfgWord_combobox[i].Text;
                    CfgWord_dataGridView[2, i].Value = CfgWord_combobox[i].SelectedValue.ToString();
                }
                else if ((i >= 74) && (i < 122))
                {
                    CfgWord_dataGridView[1, i].Value = CfgWord_checkbox[i - 74].Text;
                    CfgWord_dataGridView[2, i].Value = "00";

                }
            }

            CfgWord_dataGridView[1, 122].Value = "00";
            CfgWord_dataGridView[2, 122].Value = "00";
            CfgWord_dataGridView[1, 123].Value = "00";
            CfgWord_dataGridView[2, 123].Value = "00";
            CfgWord_dataGridView[1, 124].Value = "00";
            CfgWord_dataGridView[2, 124].Value = "00";
            CfgWord_dataGridView[1, 125].Value = "00";
            CfgWord_dataGridView[2, 125].Value = "00";

            CfgWord_dataGridView[1, 126].Value = "55";//ProgValue
            CfgWord_dataGridView[2, 126].Value = "55";

            CfgWord_dataGridView[1, 127].Value = "00";//UserValue
            CfgWord_dataGridView[2, 127].Value = "00";

            CfgWord_dataGridView[1, 128].Value = "A5";//ModeEn
            CfgWord_dataGridView[2, 128].Value = "A5";

        }

        /// <summary>
        /// 为避免连接数据库，这里手工构造数据表，实际应用中应从数据库中获取
        /// </summary>
        private void BindData()
        {
            int temp;

            DataTable dtData = new DataTable();
            dtData.Columns.Add("contex");
            dtData.Columns.Add("cfg");
            dtData.Columns.Add("data");
            DataRow drData;

            //cw0
            drData = dtData.NewRow(); drData[0] = "UID重数"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "CL接口使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "非接触界面类型"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "程序扩展模式"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "启动存储器选择"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "卡类型"; dtData.Rows.Add(drData);
            //cw1
            drData = dtData.NewRow(); drData[0] = "逻辑加密卡类型"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "逻辑加密卡加密类型"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "逻辑加密卡数据区大小"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "扩展逻辑加密卡数量"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "MMU模式选择"; dtData.Rows.Add(drData);

            //cw2
            drData = dtData.NewRow(); drData[0] = "频率检测"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "光检测"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "金属屏蔽检测"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "温度检测"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "电压检测"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "安全模块复位使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "屏蔽层模式"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "CT接口下CPU时钟控制"; dtData.Rows.Add(drData);
            //cw3
            drData = dtData.NewRow(); drData[0] = "低温检测配置"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "高温检测配置"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "ATQA、SAK数据来源选择"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "用户UID使能"; dtData.Rows.Add(drData);

            //cw4
            drData = dtData.NewRow(); drData[0] = "随机数采样频率选择"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "真随机源个数"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "fibonacci随机数源使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化指令使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化指令读取出lib外ROM使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化指令读取encry_lib使能"; dtData.Rows.Add(drData);
            //cw5
            drData = dtData.NewRow(); drData[0] = "接触接口使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "SPI接口使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "I2C接口使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "Uart接口使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "SWP接口使能（仅309）"; dtData.Rows.Add(drData);

            drData = dtData.NewRow(); drData[0] = "eSWP接口使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "S2C接口使能"; dtData.Rows.Add(drData);
            //cw6
            drData = dtData.NewRow(); drData[0] = "RSA/ECC算法使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "DES算法使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "AES算法使能（仅309）"; dtData.Rows.Add(drData);

            drData = dtData.NewRow(); drData[0] = "SSF33算法使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "SM1算法使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "SM7算法使能（仅309）"; dtData.Rows.Add(drData);

            drData = dtData.NewRow(); drData[0] = "安全区特殊数据区访问权限定义"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化指令认证"; dtData.Rows.Add(drData);
            //cw7
            drData = dtData.NewRow(); drData[0] = "数据EE加密因子低8位"; dtData.Rows.Add(drData);
            //cw8
            drData = dtData.NewRow(); drData[0] = "数据EE加密因子高8位"; dtData.Rows.Add(drData);
            //cw9
            drData = dtData.NewRow(); drData[0] = "非接触DFS最大频率"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "非接触工作电压"; dtData.Rows.Add(drData);
            //cw10
            drData = dtData.NewRow(); drData[0] = "数据存储器大小（仅309）"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "程序存储器类型"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "电流补偿大小配置"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "安全区校验码"; dtData.Rows.Add(drData);
            //cw11
            drData = dtData.NewRow(); drData[0] = "解调电路灵敏度配置"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "Lib区大小选择"; dtData.Rows.Add(drData);
            //cw12
            drData = dtData.NewRow(); drData[0] = "低频区EEPROM访问周期配置"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "高频区EEPROM访问周期配置"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "数字电路工作电压配置（仅309）"; dtData.Rows.Add(drData);
            //cw13
            drData = dtData.NewRow(); drData[0] = "Patch区空间大小"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "S2C工作模式选择（仅309）"; dtData.Rows.Add(drData);

            //cw14
            drData = dtData.NewRow(); drData[0] = "RAM数据加密使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "数据存储器加密使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "程序存储器加密使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "数据存储器加密轮数配置（仅309）"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "EE/RAM奇偶校验使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "ROM奇偶校验使能"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "加密的SBOX个数选择（仅309）"; dtData.Rows.Add(drData);
            //cw15
            drData = dtData.NewRow(); drData[0] = "VDD15电压TRIM值（仅309）"; dtData.Rows.Add(drData);

            //cw16
            drData = dtData.NewRow(); drData[0] = "可配置环振DFS频率TRIM"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "4M环振频率TRIM"; dtData.Rows.Add(drData);
            //cw17
            drData = dtData.NewRow(); drData[0] = "SWP电流TRIM值（仅309）"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "VDD15_2电压TRIM值（仅309）"; dtData.Rows.Add(drData);
            //cw18
            drData = dtData.NewRow(); drData[0] = "非接触CPU工作模式"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "调制解调电路选择"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "动态shield工作时钟频率选择"; dtData.Rows.Add(drData);

            //lw0
            drData = dtData.NewRow(); drData[0] = "UID重数                控制字0的bit1~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "非接触接口配置         控制字0的bit4~2	"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "程序扩展模式           控制字0的bit5"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "程序扩展存储器类型     控制字0的bit6"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "卡类型                 控制字0的bit7"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "安全配置               控制字2的bit6~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "认证使能               控制字6的bit7"; dtData.Rows.Add(drData);

            drData = dtData.NewRow(); drData[0] = "CT接口CPU时钟关闭      控制字2的bit7"; dtData.Rows.Add(drData);
            //lw1
            drData = dtData.NewRow(); drData[0] = "M1卡配置               控制字1的bit4~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "MMU管理模式            控制字1的bit5"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "ATQA/SAK源配置         控制字3的bit6"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "用户UID使能            控制字3的bit7"; dtData.Rows.Add(drData);

            drData = dtData.NewRow(); drData[0] = "非接触接口CPU时钟模式  控制字18的bit7~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "接触接口使能           控制字5的bit1"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "通用接触接口配置       控制字5的bit4~2"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "通用非接触接口配置     控制字5的bit7~5"; dtData.Rows.Add(drData);
            //lw2
            drData = dtData.NewRow(); drData[0] = "温度检测配置           控制字3的bit5~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "真随机数配置           控制字4的bit4~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "反码校验字节自动写配置 控制字10的bit7"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化命令使能         控制字4的bit5"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化命令读加密函数库 控制字4的bit7"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "初始化命令读通用程序区 控制字4的bit6"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "芯片操作配置           控制字15/16/17的bit7~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "芯片制造商配置         控制字10的6~0、11的7~0"; dtData.Rows.Add(drData);

            //lw3

            drData = dtData.NewRow(); drData[0] = "SM7使能                控制字6的bit5"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "DES使能                控制字6的bit1"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "SSF33使能              控制字6的bit3"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "PAE使能                控制字6的bit0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "SM1使能                控制字6的bit4"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "AES使能                控制字6的bit2"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "数据EEPROM加密密钥     控制字7/8的bit7~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "数据总线加密和奇偶校验 控制字14的bit7~0"; dtData.Rows.Add(drData);
            //lw4
            drData = dtData.NewRow(); drData[0] = "芯片第一重UID          TypeA_info_word区域的byte0~5、byte8~13、byte16~21"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "ATQA                   TypeA_info_word区域的byte22~23"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "建设部认证码           chip_inf_word区域的byte4~7"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "小号信息               chip_inf_word区域的byte0~3"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "控制字RFU位            控制字5的bit0、6的bit6、1的bit7~6"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "信息区中的RFU位        TypeA_info_word区域的byte6~7 byte14~15、chip_inf_word区域中的byte8~19"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "CL接口电压频率配置     控制字9的bit7~0	"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "访问周期和数字电压配置 控制字12的bit7~0"; dtData.Rows.Add(drData);
            //lw5
            drData = dtData.NewRow(); drData[0] = "编程模式字             mode_word区域中的byte4"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "用户模式字             mode_word区域中的byte5"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "模式使能字             mode_word区域中的byte6"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "校验字节地址加密密钥   Check_word_encry_key中的byte0~3"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "JCA配置字              JCA_config_word区域中的byte0~31"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "控制字中的完整RFU字节  mode_word区域中的byte0~3"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "Patch和S2C模式的配置   控制字13的bit7~0"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "特殊信息区             特殊信息区中共32byte内容"; dtData.Rows.Add(drData);

            //RFU
            drData = dtData.NewRow(); drData[0] = "RFU"; dtData.Rows.Add(drData);

            drData = dtData.NewRow(); drData[0] = "RFU"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "RFU"; dtData.Rows.Add(drData);
            drData = dtData.NewRow(); drData[0] = "RFU"; dtData.Rows.Add(drData);
            //Prog_value
            drData = dtData.NewRow(); drData[0] = "Prog模式字"; dtData.Rows.Add(drData);
            //user_value
            drData = dtData.NewRow(); drData[0] = "User模式字"; dtData.Rows.Add(drData);
            //Mode_en
            drData = dtData.NewRow(); drData[0] = "模式使能字"; dtData.Rows.Add(drData);




            CfgWord_dataGridView.DataSource = dtData;

            CfgWord_dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            CfgWord_dataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            CfgWord_dataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            CfgWord_dataGridView.Columns[0].Width = 250;
            CfgWord_dataGridView.Columns[1].Width = 300;
            CfgWord_dataGridView.Columns[2].Width = 120;
            CfgWord_dataGridView.RowHeadersWidth = 100;

            CfgWord_dataGridView.AllowUserToResizeRows = false;
            CfgWord_dataGridView.AllowUserToAddRows = false;
            CfgWord_dataGridView.Columns[1].DefaultCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);

            for (int i = 0; i < this.CfgWord_dataGridView.Rows.Count; i++)
            {
                if (i < 6)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW0";
                }
                else if (i < 11)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW1";
                }
                else if (i < 19)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW2";
                }
                else if (i < 23)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW3";
                }
                else if (i < 29)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW4";
                }
                else if (i < 36)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW5";
                }
                else if (i < 44)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW6";
                }
                else if (i < 45)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW7";
                }
                else if (i < 46)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW8";
                }
                else if (i < 48)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW9";
                }
                else if (i < 52)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW10";
                }
                else if (i < 54)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW11";
                }
                else if (i < 57)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW12";
                }
                else if (i < 59)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW13";
                }
                else if (i < 66)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW14";
                }
                else if (i < 67)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW15";
                }
                else if (i < 69)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW16";
                }
                else if (i < 71)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW17";
                }
                else if (i < 74)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "CW18";
                }
                else if (i < 82)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "Lock0";
                }
                else if (i < 90)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "Lock1";
                }
                else if (i < 98)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "Lock2";
                }
                else if (i < 106)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "Lock3";
                }
                else if (i < 114)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "Lock4";
                }
                else if (i < 122)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "Lock5";
                }
                else if (i < 129)
                {
                    CfgWord_dataGridView.Rows[i].HeaderCell.Value = "ModeWord";
                }
            }


            CfgWord_dataGridView.Columns[0].ReadOnly = true;
            CfgWord_dataGridView.Columns[2].ReadOnly = true;
        }

        private void DetectCfgWord_btn_Click(object sender, EventArgs e)
        {
            string auth_key;
            try
            {
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);

                if (CWD_auth_checkbox.Checked)
                {
                    auth_key = richTextBox1.Text.Replace(" ", "");
                    if ((auth_key == "") || (auth_key.Length != 32))
                    {
                        display = "认证的密钥长度不对！！！   \t\t";
                        DisplayMessageLine(display);
                        return;
                    }
                    FM12XX_Card.TransceiveCL("A200", "03", "09", out receive);//CNN 添加认证，使认证时也能配置控制字
                    int k = FM12XX_Card.TransceiveCL(auth_key, "03", "09", out receive);//LOAD密钥
                    if (k != 0)
                    {
                        display = "认证:   \t\t" + "错误！！";
                        DisplayMessageLine(display);
                        return;
                    }
                    display = "认证:   \t\t" + "通过";
                    DisplayMessageLine(display);
                }
                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                SendData_textBox.Text = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                SendData_textBox.Text += receive;

                Reset17_Click(null, EventArgs.Empty);
                CfgWord_Parse_btn_Click(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void ResFreqCacu_F_btn_Click(object sender, EventArgs e)
        {
            double RF_C, RF_L, RF_F;
            try
            {
                RF_C = Convert.ToSingle(ResonFreqCaculate_C_tB.Text);
                RF_L = Convert.ToSingle(ResonFreqCaculate_L_tB.Text);

                RF_F = 1000 / (Math.PI * 2 * (Math.Sqrt(RF_C * RF_L)));

                ResonFreqCaculate_F_tB.Text = RF_F.ToString("F2");
            }
            catch
            {
                ResonFreqCaculate_F_tB.Text = "error";
            }
        }

        private void ResFreqCacu_L_btn_Click(object sender, EventArgs e)
        {
            double RF_C, RF_L, RF_F;
            try
            {
                RF_C = Convert.ToSingle(ResonFreqCaculate_C_tB.Text);
                RF_F = Convert.ToSingle(ResonFreqCaculate_F_tB.Text);

                RF_L = Math.Pow((1000 / (2 * Math.PI * RF_F)), 2) / RF_C;

                ResonFreqCaculate_L_tB.Text = RF_L.ToString("F2");
            }
            catch
            {
                ResonFreqCaculate_L_tB.Text = "error";
            }
        }

        private void ResFreqCacu_C_btn_Click(object sender, EventArgs e)
        {
            double RF_C, RF_L, RF_F;
            try
            {
                RF_L = Convert.ToSingle(ResonFreqCaculate_L_tB.Text);
                RF_F = Convert.ToSingle(ResonFreqCaculate_F_tB.Text);

                RF_C = Math.Pow((1000 / (2 * Math.PI * RF_F)), 2) / RF_L;

                ResonFreqCaculate_C_tB.Text = RF_C.ToString("F2");
            }
            catch
            {
                ResonFreqCaculate_C_tB.Text = "error";
            }
        }

        private void RAMtype_radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            if (FM350_radioButton.Checked == true)
            {
                checkBox_SeperateLibProg.Enabled = false;
                checkBox_SeperateLibProg.Checked = false;
            }
            /*PatchSize_comboBox.Enabled = true;
            PatchSize_comboBox.Text = "7K";
            ProgEEStartAddr.Text = "000400";
            nProgRamStartTemp = 0x0400;
            if (open_flag == 1)
            {
                ProgEEEndAddr.Text = (ProgFileLenth + 0x40 - 1).ToString("X6");
            }*/
        }

        private void CT_Interface_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_CTHighBaud.Checked = false;
            if (CT_Interface.Checked == true)
            {
                RAMtype_radiobutton.Enabled = true;
                checkBox_CTHighBaud.Enabled = true;
                checkBox_CTHighBaud.Visible = true;
                comboBox_CTBaud.Visible = true;
                comboBox_CTBaud.Enabled = false;
                checkBox_ignore9800.Enabled = true;
                checkBox_ignore9800.Visible = true;
            }
            else
            {
                RAMtype_radiobutton.Enabled = false;
                checkBox_CTHighBaud.Enabled = false;
                checkBox_CTHighBaud.Visible = false;
                comboBox_CTBaud.Visible = false;
                comboBox_CTBaud.Enabled = false;
                checkBox_ignore9800.Enabled = false;
                checkBox_ignore9800.Visible = false;
            }
        }

        private void PROG_EEtype_radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            if (FM350_radioButton.Checked == true)
            {
                checkBox_SeperateLibProg.Enabled = true;
                checkBox_SeperateLibProg.Checked = false;
            }
            /*ProgEEStartAddr.Text = "000000";            
            if (open_flag == 1)
            {
                ProgEEEndAddr.Text = (ProgFileLenth - 1).ToString("X6");
            }*/

        }

        private void DATA_EEtype_radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            if (FM350_radioButton.Checked == true)
            {
                checkBox_SeperateLibProg.Checked = false;
                checkBox_SeperateLibProg.Enabled = false;

            }
            /* ProgEEStartAddr.Text = "000000";
             if (open_flag == 1)
             {
                 ProgEEEndAddr.Text = (ProgFileLenth - 1).ToString("X6");
             }*/
        }

        private void CL_Interface_CheckedChanged(object sender, EventArgs e)
        {
            if (CL_Interface.Checked == true)
            {
                RAMtype_radiobutton.Enabled = false;
                checkBox_CTHighBaud.Enabled = false;
                checkBox_CTHighBaud.Visible = false;
                comboBox_CTBaud.Visible = false;
            }
            else
            {
                RAMtype_radiobutton.Enabled = true;
                checkBox_CTHighBaud.Enabled = true;
                checkBox_CTHighBaud.Visible = true;
            }
            /* ProgEEStartAddr.Text = "000000";
             if (open_flag == 1)
             {
                 ProgEEEndAddr.Text = (ProgFileLenth - 1).ToString("X6");
             }*/
        }

        private void text_01_P1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void text_02_P1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void text_02_P2_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void text_04_P1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void text_00_P1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void text_00_P2_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void text_04_P2_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void SaveAS_Button_Click(object sender, EventArgs e)
        {
            string StartEEAddr, EndEEAddr;
            SaveAs_flag = 1;
            StartEEAddr = SaveAsStart.Text;
            EndEEAddr = SaveAsEnd.Text;
            for (int i = 0; i < 6 - SaveAsStart.Text.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (int i = 0; i < 6 - SaveAsEnd.Text.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            SaveAsStart.Text = StartEEAddr;
            SaveAsEnd.Text = EndEEAddr;
            SaveFileBuffer(SaveAsStart.Text, SaveAsEnd.Text, Progfilebuf);
            if (SaveAs_flag == 0)
            {
                display = "保存成功！";
                DisplayMessageLine(display);
                return;
            }
        }

        private void SaveAs_verify_Button_Click(object sender, EventArgs e)
        {
            string StartEEAddr, EndEEAddr;
            int wcrc, result_crc;
            byte[] crc = new byte[2];
            StartEEAddr = SaveAsStart.Text;
            EndEEAddr = SaveAsEnd.Text;
            for (int i = 0; i < 6 - SaveAsStart.Text.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (int i = 0; i < 6 - SaveAsEnd.Text.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            SaveAsStart.Text = StartEEAddr;
            SaveAsEnd.Text = EndEEAddr;
            wcrc = strToHexByte(InitCRC16_texbox.Text)[0] * 256 + strToHexByte(InitCRC16_texbox.Text)[1];
            CRC16_verify(StartEEAddr, EndEEAddr, Progfilebuf, wcrc, out result_crc);
            crc[1] = (byte)(result_crc % 0x100);
            crc[0] = (byte)(result_crc / 0x100);
            InitCRC16_texbox.Text = byteToHexStr(2, crc);
            display = "软件计算的CRC:  \t\t" + (result_crc % 0x10000).ToString("X4") + "  (0x" + SaveAsStart.Text + "—" + "0x" + SaveAsEnd.Text + ")";
            DisplayMessageLine(display);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strReceived, data, auth_key;
            int i, j;
            PROG_EEtype_radiobutton.Checked = false;
            DATA_EEtype_radiobutton.Checked = true;
            if (InitData_comboBox.SelectedIndex == 0)
            {
                data = "00000000000000000000000000000000";
            }
            else if (InitData_comboBox.SelectedIndex == 1)
            {
                data = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
            }
            else if (InitData_comboBox.SelectedIndex == 2)
            {
                data = "55555555555555555555555555555555";
            }
            else if (InitData_comboBox.SelectedIndex == 3)
            {
                data = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            }
            else if (InitData_comboBox.SelectedIndex == 4)
            {
                data = "000102030405060708090A0B0C0D0E0F";
            }
            else if (InitData_comboBox.SelectedIndex == 5)
            {
                data = "0F0E0D0C0B0A09080706050403020100";
            }
            else
            {
                data = "00000000000000000000000000000000";
            }
            if (MessageBox.Show("是否对EE进行全擦？请选择EE", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (CL_Interface.Checked)
                {
                    FM12XX_Card.Init_TDA8007(out strReceived);
                    FM12XX_Card.SetField(1, out strReceived);
                    FM12XX_Card.Set_FM1715_reg(FM1715Reg_Str.TxControl, "5b", out strReceived);
                    FM12XX_Card.REQA(out strReceived);
                    FM12XX_Card.AntiColl_lv(1, out strReceived);
                    FM12XX_Card.Select(1, out strReceived);

                    if (Auth_checkBox.Checked)
                    {
                        auth_key = key_richTextBox.Text.Replace(" ", "");
                        if ((auth_key == "") || (auth_key.Length != 32))
                        {
                            display = "认证的密钥长度不对！！！   \t\t";
                            DisplayMessageLine(display);
                            return;
                        }
                        FM12XX_Card.TransceiveCL("A200", "03", "09", out receive);//CNN 添加认证，使认证时也能配置控制字
                        int k = FM12XX_Card.TransceiveCL(auth_key, "03", "09", out receive);//LOAD密钥
                        if (k != 0)
                        {
                            display = "认证:   \t\t" + "错误！！";
                            DisplayMessageLine(display);
                            return;
                        }
                        display = "认证:   \t\t" + "通过";
                        DisplayMessageLine(display);
                    }

                    if (PROG_EEtype_radiobutton.Checked)
                        i = FM12XX_Card.TransceiveCL("32DC", "01", "09", out strReceived);
                    else
                        i = FM12XX_Card.TransceiveCL("329C", "01", "09", out strReceived);
                    j = FM12XX_Card.WriteBlock("00", data, out strReceived);
                }
                else
                {
                    FM12XX_Card.SetField(0, out display);
                    FM12XX_Card.Init_TDA8007(out strReceived);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    i = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    if (FM347_radioButton.Checked == true)
                    {
                        FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                        FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                        FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

                        FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                        FM12XX_Card.TransceiveCT("00800000" + "00000000" + "00000004" + "00000010", "02", out strReceived);

                        FM12XX_Card.TransceiveCT("000D000000", "02", out strReceived); //NVM
                        FM12XX_Card.TransceiveCT("0004000001", "02", out strReceived); // NVM

                        FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                        FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010
                        j = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                    }
                    else
                    {
                        if (Auth_checkBox.Checked)
                        {
                            auth_key = key_richTextBox.Text.Replace(" ", "");
                            if ((auth_key == "") || (auth_key.Length != 32))
                            {
                                display = "认证的密钥长度不对！！！   \t\t";
                                DisplayMessageLine(display);
                                return;
                            }
                            FM12XX_Card.TransceiveCT("0055000010", "02", out strReceived);//CNN 添加认证，使认证时也能配置控制字
                            int k = FM12XX_Card.TransceiveCT(auth_key, "02", out strReceived);//LOAD密钥
                            if (k != 0)
                            {
                                display = "认证:   \t\t" + "错误！！";
                                DisplayMessageLine(display);
                                return;
                            }
                            display = "认证:   \t\t" + "通过";
                            DisplayMessageLine(display);
                        }
                        if (PROG_EEtype_radiobutton.Checked)
                            i = FM12XX_Card.TransceiveCT("0002020000", "02", out strReceived);
                        else
                            i = FM12XX_Card.TransceiveCT("0002000000", "02", out strReceived);
                        //         i = FM12XX_Card.TransceiveCT("000307E700", "02", out strReceived);
                        i = FM12XX_Card.TransceiveCT("0003" + ER_TIME.Text + WR_TIME.Text + "00", "02", out strReceived);  //zhujunhao,20150317
                        j = FM12XX_Card.TransceiveCT("0000000010", "02", out strReceived);
                        j = FM12XX_Card.TransceiveCT(data, "02", out strReceived);
                    }
                }

                if (i == 1 || j == 1)
                {
                    display = "擦写错误";
                    DisplayMessageLine(display);
                    return;
                }
                else
                {
                    display = "擦写完成";
                    DisplayMessageLine(display);
                }

            }
        }

        private void TransceiveDataCL_listBox_KeyDown(object sender, KeyEventArgs e)
        {
            int count;
            int volume;
            if (e.KeyCode == Keys.Delete)
            {
                count = TransceiveDataCL_listBox.SelectedItems.Count;
                for (int i = 0; i < count; i++)
                {
                    this.TransceiveDataCL_listBox.Items.RemoveAt(this.TransceiveDataCL_listBox.SelectedIndex);//cnn
                     //this.TransceiveDataCL_listBox.Items.Remove(this.TransceiveDataCL_listBox.SelectedItem);
               }
                //从CMD_LOG文件中删除选中的指令
                volume = TransceiveDataCL_listBox.Items.Count;
                try
                {
                    if (open_cmdlog_flag == 0)
                        CMD_LOG_Directory = ".\\CMD_CL_LOG.txt";
                    StreamWriter sw = new StreamWriter(CMD_LOG_Directory);

//                    StreamWriter sw = new StreamWriter(".\\CMD_CL_LOG.txt");
                    for (int i = 0; i < volume; i++)
                    {
                        sw.WriteLine(TransceiveDataCL_listBox.Items[i].ToString());           
                    }
                    sw.Close();                                   
                }
                catch (Exception ex)
                {
                    DisplayMessageLine(ex.Message);
                    //MessageBox.Show(ex.Message);
                }          
            }
        }

        private void TransceiveDataCT_listBox_KeyDown(object sender, KeyEventArgs e)
        {
            int count;
            int volume;
            if (e.KeyCode == Keys.Delete)
            {
                count = TransceiveDataCT_listBox.SelectedItems.Count;
                for (int i = 0; i < count; i++)
                {
                    this.TransceiveDataCT_listBox.Items.RemoveAt(this.TransceiveDataCT_listBox.SelectedIndex);//cnn
                    //this.TransceiveDataCT_listBox.Items.Remove(this.TransceiveDataCT_listBox.SelectedItem);
                }
                //从CMD_LOG文件中删除选中的指令
                volume = TransceiveDataCT_listBox.Items.Count;
                try
                {
                    if (open_cmdlog_flag == 0)
                        CMD_LOG_Directory = ".\\CMD_CT_LOG.txt";
                    StreamWriter sw = new StreamWriter(CMD_LOG_Directory);
                    //StreamWriter sw = new StreamWriter(".\\CMD_CT_LOG.txt");
                    for (int i = 0; i < volume; i++)
                    {
                        sw.WriteLine(TransceiveDataCT_listBox.Items[i].ToString());
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    DisplayMessageLine(ex.Message);
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        private void pps_exchange_CL_btn_Click(object sender, EventArgs e)
        {
            string pps1, receive;
            int pps1_DSI, pps1_DRI;
            try
            {
                /*if (PPS_exchange_cl_cbox.Text == "106K")
                    pps1 = "11";//---106K
                else if (PPS_exchange_cl_cbox.Text == "212K")
                    pps1 = "13";// ---- 212kbps
                else if (PPS_exchange_cl_cbox.Text == "424K")
                    pps1 = "94";// ---- 424kbps
                else if (PPS_exchange_cl_cbox.Text == "848K")
                    pps1 = "18";// ---- 848kbps
                else
                    pps1 = "11";*/
                if (VHBR_PPS_checkBox.Checked)
                {                   
                    FM12XX_Card.TransceiveCL("F0A002A100", "01", "09", out receive);
                    if (receive.Substring(0, 10) != "F0A00DA20B")
                    {
                        DisplayMessageLine("VHBR波特率请求帧回发错误！返回值：" + receive);
                        return;
                    }
                    DisplayMessageLine("下行可支持：" + receive.Substring(14,2));
                    DisplayMessageLine("上行可支持：" + receive.Substring(22,2));
                    pps1_DSI = strToHexByte(textBox_PPSCL_DSI.Text)[0];
                    pps1_DRI = strToHexByte(textBox_PPSCL_DRI.Text)[0];
                    FM12XX_Card.TransceiveCL("F0A00DA30B 8302" + pps1_DRI.ToString("X2") + "00 8402" + pps1_DSI.ToString("X2") + "00 850100", "01", "09", out receive);
                   // FM12XX_Card.SendAPDUCL("F0A00DA30B 8302" + pps1_DRI + "00 8402" +pps1_DSI+ "00 850100", out receive);
                    if (receive != "F0A002A400")
                    {
                        DisplayMessageLine("波特率切换错误！返回值：" + receive);
                        return;
                    }
                    else
                        DisplayMessageLine("PPS Response:\t\t\t" + "PPS Exchange Succeeded");
                    switch (pps1_DRI)
                    {
                        case 0x2://212
                            FM12XX_Card.Set_FM1715_reg(0x14, 0x11, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x15, 0x09, out receive);
                            break;
                        case 0x4://424
                            FM12XX_Card.Set_FM1715_reg(0x14, 0x09, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x15, 0x04, out receive);
                            break;
                        case 0x8://848
                            FM12XX_Card.Set_FM1715_reg(0x14, 0x01, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x15, 0x01, out receive);
                            break;
                        default://106
                            FM12XX_Card.Set_FM1715_reg(0x14, 0x19, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x15, 0x13, out receive);
                            break;
                    }
                    switch (pps1_DSI)
                    {
                        case 0x2://212
                            FM12XX_Card.Set_FM1715_reg(0x19, 0x53, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1A, 0x09, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1C, 0xFF, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1D, 0x0C, out receive);
                            break;
                        case 0x4://424
                            FM12XX_Card.Set_FM1715_reg(0x19, 0x33, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1A, 0x09, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1C, 0xFF, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1D, 0x0C, out receive);
                            break;
                        case 0x8://848
                        case 0x10:
                        case 0x20:
                        case 0x30:
                            FM12XX_Card.Set_FM1715_reg(0x19, 0x13, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1A, 0x09, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1C, 0xFF, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1D, 0x0C, out receive);
                            break;
                        default://106
                            FM12XX_Card.Set_FM1715_reg(0x19, 0x73, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1A, 0x08, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1C, 0xFF, out receive);
                            FM12XX_Card.Set_FM1715_reg(0x1D, 0x00, out receive);
                            break;
                    }
                }
                else
                {
                    pps1_DSI = strToHexByte(textBox_PPSCL_DSI.Text)[0];
                    pps1_DRI = strToHexByte(textBox_PPSCL_DRI.Text)[0];
                    pps1 = ((pps1_DSI << 2) + pps1_DRI).ToString("X2");

                    FM12XX_Card.PPS_CL(pps1, out display);

                    display = "PPS Response:\t\t\t" + display;

                    DisplayMessageLine(display);
                }

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void PPS_exchange_ct_btn_Click(object sender, EventArgs e)
        {
            string pps1;
            try
            {
                while (PPS_exchange_ct_tbox.Text.Length < 2)
                {
                    PPS_exchange_ct_tbox.Text = "0" + PPS_exchange_ct_tbox.Text;
                }
                pps1 = PPS_exchange_ct_tbox.Text;

                FM12XX_Card.PPS_CT(pps1, out display);

                display = "PPS Response： " + display;

                DisplayMessageLine(display);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }

        }

        private void version_check_Click(object sender, EventArgs e)
        {
            DisplayMessageLine("v1.00  2013-01-01   FM309流片前的最后固定版本。");
            DisplayMessageLine("v1.01  2013-03-05   增加flash_isp功能。可对FM309验证板上的flash1进行编程校验。（高学兵）");
            DisplayMessageLine("v1.02  2013-03-06   增加完善add CMD list功能对APDU之类的支持，并且可以使用:添加注释。（高学兵）");
            DisplayMessageLine("v1.03  2013-03-19   增加简单脚本中命令行可添加注释的功能，注释格式为在行末用'#'开头的内容，但是在打开脚本后不予以显示。(高学兵)");
            DisplayMessageLine("v1.04  2013-03-20   修改APDU之类，将明确显示返回码，如：6A81等等。（高学兵）");
            DisplayMessageLine("v1.05  2013-03-22   修改脚本文件打开和Hex文件打开时可以记住上一次的打开文件的路径。（高学兵）");
            DisplayMessageLine("v1.06  2013-03-25   程序接口访问补丁区，由硬件自动指向修改为由上位机指向，（陈楠楠）");
            DisplayMessageLine("v1.07  2013-03-25   修改如果卡没有ATR回发，不影响CT接口的程序下载，（陈楠楠）");
            DisplayMessageLine("v1.08  2013-03-28   修改清BUF的最大长度，修改补丁区的编程问题，（陈楠楠）");
            DisplayMessageLine("v1.09  2013-04-10   添加非接对LIB区的编程功能，（陈楠楠）");
            DisplayMessageLine("v1.10  2013-04-15   添加针对送检白卡的一键下载功能。（陈楠楠）");
            DisplayMessageLine("v1.11  2013-04-15   修改上位机控制字解析和生成时lock位出错的问题和电压检测的描述位。（高学兵）");
            DisplayMessageLine("v1.12  2013-04-27   添加卡机非接触透明传输时可以任意配置带不带CRC，并且修改相应的传输函数，去除编程等时的认证操作。（高学兵）");
            DisplayMessageLine("v1.13  2013-04-28   修改白卡下载的流程，适用与COS下载，COS版本( FM309Cos_ROM_BCTC_V06.hex )。（陈楠楠）");
            DisplayMessageLine("v1.14  2013-05-01   增加CWD按钮，修改控制字，改高温配置为3'b110。（闫守礼）");
            DisplayMessageLine("v1.15  2013-05-31   修改编程界面的布局，增加编程模式的配置。（陈楠楠）");
            DisplayMessageLine("v1.16  2013-06-08   调整界面，修正小bug，添加脚本打开时的过滤，控制字中的动静态shield反过来了。（高学兵）");
            DisplayMessageLine("v1.17  2013-07-05   修改接触接口进入init接口时的egt为1，即init接口通讯是增加egt，由于FM309V04的限制做的修改（高学兵）");
            DisplayMessageLine("v1.18  2013-08-05   添加编程和控制字部分的认证功能（陈楠楠）");
            DisplayMessageLine("v1.19  2013-10-23   添加低温筛卡功能。（杨松川）");
            DisplayMessageLine("v1.20  2013-10-30   添加银检下载COS筛卡功能，白卡按钮更新为银检COS，去掉CWD按钮。\n\t\t\t\tCOS版本(FM1280_BCTC_COS_V0207.hex,CRC=3923 (0x000000—0x00DE5E))。（闫守礼）");
            DisplayMessageLine("v1.21  2013-10-30   添加接触接口环振频率校准功能，需配合频率校准COS使用。（朱军浩）");
            DisplayMessageLine("v1.22  2013-11-01   修改下载银检COS的控制字内容，高温配置为110。（闫守礼）");
            DisplayMessageLine("v1.23  2013-11-04   修改非接PPS的切换，DSI和DRI分别配置，对应的卡机版本V1.3-20131104。（陈楠楠）");
            DisplayMessageLine("v1.24  2013-11-08   添加逻辑加密卡认证功能。（陈楠楠）");
            DisplayMessageLine("v1.25  2013-11-16   配置字部分添加FM336的配置选项。（陈楠楠）");
            DisplayMessageLine("v1.26  2013-11-19   更新FM336的配置选项。（陈楠楠）");
            DisplayMessageLine("v1.27  2013-11-21   修改频率校准流程，FM309卡可不做控制字配置，校准完后，其他配置字保持不变。（朱军浩）");
            DisplayMessageLine("v1.28  2013-11-28   配合新的频率校准COS修改流程，可针对不同的档位进行校准，并增加是否下载程序选项。（朱军浩）");
            DisplayMessageLine("v1.29  2013-11-29   修改FM309频率校准修改配置字把模式误改为用户模式的错误。（朱军浩）");
            DisplayMessageLine("v1.30  2013-12-10   增加环振频率校准配置下拉菜单，增加显示当前校准后频率。（朱军浩）");
            DisplayMessageLine("v1.31  2013-12-12   更新FM336配置字选项。（陈楠楠）");
            DisplayMessageLine("v1.32  2013-12-17   修改频率校准时，非接编程出现选卡错误的问题。（朱军浩）");
            DisplayMessageLine("v1.33  2013-12-25   增加直接读取内部环振频率功能。（朱军浩）");
            DisplayMessageLine("v1.34  2014-01-23   增加编程内容搬移功能。（朱军浩）");
            DisplayMessageLine("v1.35  2014-02-13   编程界面删减一些东西，添加CT高速下载选项。（陈楠楠）");
            DisplayMessageLine("v1.36  2014-02-28   修正配置字部分DFS频率对应关系，光检测配置修改为电流补偿相关配置。（陈楠楠）");
            DisplayMessageLine("v1.37  2014-03-04   优化频率校准功能，COS改为在RAM里面执行；程序大小4K；只读频率按钮改为不做写动作和比较，只读频率。（陈楠楠）");
            DisplayMessageLine("v1.38  2014-03-11   频率校准改用单步发送命令，且校准时配置字关闭安全监测及复位功能，关闭存储器加密以及奇偶校验，校准完成后恢复配置字（朱军浩）");
            DisplayMessageLine("v1.39  2014-03-12   频率校准按钮自动载入校准COS（朱军浩）");
            DisplayMessageLine("v1.40  2014-03-18   修改FM309低温筛卡写入的控制字对bandgap的影响，增加FM326的低温筛卡选项。（杨松川）");
            DisplayMessageLine("v1.41  2014-03-27   更正FM326低温筛卡控制字，使能低温报警。（杨松川）");
            DisplayMessageLine("v1.42  2014-04-04   修改PCSC通讯函数，更改异常抛出机制，普通通讯错误不再抛出异常，将直接显示出错误，利于复杂脚本运行。并且修改部分复杂脚本的功能（高学兵）");
            DisplayMessageLine("v1.43  2014-04-21   增加配置新项，通过从运行文件目录下读取xml文件，进行配置字的更新（陈楠楠）");
            DisplayMessageLine("v1.44  2014-06-20   添加针对FM336项目的整体扩展和段后（85、86段）扩展程序（陈楠楠）");
            DisplayMessageLine("v1.45  2014-07-10（世界杯半决赛版）  修改编程页面下的另存功能bug，之前的另存功能速度太慢了，修改后秒存（高学兵）");
            DisplayMessageLine("v1.46  2014-07-15   修改编程流程（编程前下电）；FM336控制字解析去掉空格；算法计算时间显示改为浮点类型（闫守礼）");
            DisplayMessageLine("v1.47  2014-07-24   修改另存功能的地址范围；低温筛卡界面添加一些一键测试功能（陈楠楠）");
            DisplayMessageLine("v1.48  2014-07-25   增加FLASH下载校验选择选项（朱军浩）");
            DisplayMessageLine("v1.49  2014-07-28   修改FLASH下载流程，加快下载速度（闫守礼）");
            DisplayMessageLine("v1.50  2014-08-01   修改打开脚本和打开hex文件时不能分别记住上次打开脚本和hex的bug（高学兵）");
            DisplayMessageLine("v1.51  2014-08-12   添加编程时背景数据选项；添加“其他接口”里UART基本通讯功能（陈楠楠）");
            DisplayMessageLine("v1.52  2014-09-01   修改安全筛卡,增加高温筛卡（杨松川）");
            DisplayMessageLine("v1.53  2014-09-10   修改高温筛卡bug及更新FM326配置文件（杨松川）");
            DisplayMessageLine("v1.54  2014-09-22   增加FM294非接触低温筛卡（杨松川）");
            DisplayMessageLine("v1.55  2014-09-25   修改FM294非接触低温筛卡（杨松川）");
            DisplayMessageLine("v1.56  2014-09-26   修改FM309高温筛卡（杨松川）");
            DisplayMessageLine("v1.57  2014-09-28   修改FM309高温筛卡初始化控制字高温配置（杨松川）");
            DisplayMessageLine("v1.58  2014-10-08   修改ROM存储时，编程方式（陈楠楠）");
            DisplayMessageLine("v1.59  2014-10-10   增加非接下的频率校准方式，需配合相应的COS使用。增加指令记忆功能。（朱军浩）");
            DisplayMessageLine("v1.60  2014-10-11   重新整理上位机，修改ROM存储时，段后扩展编程方式。（陈楠楠）");
            DisplayMessageLine("v1.61  2014-10-14   修改个小bug。（陈楠楠）");
            DisplayMessageLine("v1.62  2014-10-17   增加FM309和FM326的非接触高低温筛卡（杨松川）");
            DisplayMessageLine("v1.63  2014-10-22   添加FM336补丁区正反码校验选项（陈楠楠）");
            DisplayMessageLine("v1.64  2014-10-23   FM336频率校准时，关闭CW20的复位使能（朱军浩）");
            DisplayMessageLine("v1.65  2014-10-30   修正针对于回发“6100”的自动取数功能（高学兵）");
            DisplayMessageLine("v1.66  2014-11-06   增加FM336接触高低温筛卡（杨松川）");
            DisplayMessageLine("v1.67  2014-11-13   增加支持BIN文件格式读写，HEX 05 字段处理，文件大小扩展取整到4的倍数(闫守礼)");
            DisplayMessageLine("v1.68  2014-12-09   增加芯片配置下拉类型，加入FM349，控制字解析暂时没做(闫守礼)");
            DisplayMessageLine("v1.69  2015-01-06   增加新版配置对FM349的支持(胡杰)");
            DisplayMessageLine("v1.70  2015-01-15   增加编程操作打开Hex文件对扩展段地址记录\"02\"的支持,FM349程序大于64K时候会用到;(胡杰)");
            DisplayMessageLine("v1.71  2015-01-16   修改编程操作读取和编程操作，增加对FM349的处理，修改测试中...(胡杰)");
            DisplayMessageLine("v1.72  2015-01-20   修改新版配置对FM349处理中解析控制字的字节序处理(胡杰)");
            DisplayMessageLine("v1.73  2015-01-22   增加新版配置(FM349)生成带Parity校验的COE文件功能(胡杰)");
            DisplayMessageLine("v1.74  2015-01-22   增加新版配置(FM349)生成控制字时自动反码功能,仅在扰乱字为全0时有用(胡杰)");
            DisplayMessageLine("v1.75  2015-01-27   去掉新版配置生成COE时(FM349)Parity校验0xFF的特殊处理，相应的RFU改回全0(胡杰)");
            DisplayMessageLine("v1.75  2015-02-05   增加初始化Endurance数据内容(朱军浩)");
            DisplayMessageLine("v1.76  2015-02-04   编程操作增加偏移地址和不擦选项，FM349编程和读取简单测试，CT下可用(胡杰)");
            DisplayMessageLine("v1.77  2015-02-05   增加新版配置(FM349)生成配置字时扰乱因子的设置，需要修改扰乱因子时可用于生成配置字(胡杰)");
            DisplayMessageLine("v1.78  2015-02-09   修改新版配置(FM349)非接读取和写入配置字的支持，已经可以用(陈剑南)");
            DisplayMessageLine("v1.79  2015-02-12   增加简单脚本中的刷新脚本功能");
            DisplayMessageLine("v1.80  2015-03-13   修改安全筛卡等待时间为30s（杨松川）");
            DisplayMessageLine("v1.81  2015-03-17   增加全擦时的擦写参数控制（朱军浩）");
            DisplayMessageLine("v1.82  2015-03-24   修改FM336恒流源开关说明。“0”为开，“1”为关（朱军浩）");
            DisplayMessageLine("v1.83  2015-03-26   完成brightsight安全筛卡，保留4M及DFS环振trim值，添加认证密钥初始化（杨松川）");
            DisplayMessageLine("v1.84  2015-03-27   brightsight安全筛卡，修改class C的LOCK位不使能，增加COS地址及下载接口选择（杨松川）");
            DisplayMessageLine("v1.85  2015-03-31   修改编程操作读取HEX文件时扩展记录02和04的处理(守礼&胡杰)");
            DisplayMessageLine("v1.86  2015-04-01   修改高低温安全筛卡初始化仅修改相关控制字(杨松川)");
            DisplayMessageLine("v1.87  2015-04-07   优化高低温筛卡，增加brightsight温度筛选(杨松川)");
            DisplayMessageLine("v1.88  2015-04-22   调整温度筛选时间(杨松川)");
            DisplayMessageLine("v1.89  2015-04-27   增加温度trim理论与实测(杨松川)");
            DisplayMessageLine("v1.90  2015-04-28   添加FM350非接编程程序NVM；添加-3流程一些接收错误的判断(陈剑南)");
            DisplayMessageLine("v1.91  2015-05-18   非接APDU增加Le field；增加M1 INCVAL、DECVAL、RESTORE、TRANSFER等指令(需要卡机支持)，并且添加ReadVal和WriteVal按钮(陈剑南)");
            DisplayMessageLine("v1.92  2015-05-21   修改编程操作里，读取数据EEPROM为快读模式。(朱军浩)");
            DisplayMessageLine("v1.93  2015-06-19   增加FM274和FM294的控制字的配置读写界面。(杨松川)");
            DisplayMessageLine("v1.94  2015-07-01   频率校准处增加还原trim值读频率选项。(朱军浩)");
			DisplayMessageLine("v1.95  2015-06-29   更新FM349非接读写EE函数(陈剑南)");
            DisplayMessageLine("v1.96  2015-07-20   增加FM302控制字配置读写界面(杨松川)");
			DisplayMessageLine("v1.97  2015-07-08   增加编程操作更改界面数据同步更新到内部缓存(胡杰)");
            DisplayMessageLine("v1.98  2015-07-20   增加缓冲区初始化背景为0xFF(yanshouli)");
            DisplayMessageLine("v1.99  2015-07-21   增加非接触式Deselct功能,增加349HEX格式另存功能(胡杰)");
            DisplayMessageLine("v1.100  2015-09-10   增加294/274的编程和读取功能(杨松川)");
            DisplayMessageLine("v1.101  2015-10-20   添加非接CLA模块Direct Send Mode下面parity的控制(需要配合20150807或以上版本的卡机)(陈剑南)");
            DisplayMessageLine("v1.102  2015-10-20   添加非接CLA模块Active和Select指令“check BCC”选项，勾选以后会检查接收到UID的BCC，SELECT指令的BCC也由卡机计算(需要配合20150817或以上版本的卡机)(陈剑南)");
            DisplayMessageLine("v1.103  2015-10-20   修改非接-3选卡的指令，能够识别设置的最后一个SAK（默认0x20），判断是否完成选卡，并且在textBox中显示(需要配合20150817或以上版本的卡机)(陈剑南)");
            DisplayMessageLine("v1.104  2015-11-09   更新FM350 接触&非接 读写FLASH的操作流程(陈剑南)");
            DisplayMessageLine("v1.105  2015-11-13   安全筛卡增加银检配置(杨松川)");
            DisplayMessageLine("v1.106  2015-11-17   送检配置增加一键下载功能(杨松川)");
            DisplayMessageLine("v1.107  2015-11-17   FM350 CT编程时可选忽略9800出错，之后再通过读取判断是否正确写入(陈剑南)");
            DisplayMessageLine("v1.108  2015-11-19   FM350 CT编程时可选“LibSize”将程序文件lib段自动编程到lib区(0x40_0000)(陈剑南)");
            DisplayMessageLine("v1.109  2015-11-20   FM350擦写时间tprog、terase可以通过上位机进行配置(陈剑南)");
            DisplayMessageLine("v1.110  2015-12-03   修复：M1卡带多重UID时候，AUTH指令出错的bug(陈剑南)");
            DisplayMessageLine("v1.111  2015-12-03   修复：FM350编程数据NVM的bug(陈剑南)");
            DisplayMessageLine("v1.112  2015-12-04   更新了银检安全下载配置(杨松川)");
            DisplayMessageLine("v1.113  2015-12-07   FMF309、326、336下载程序后不下电（朱军浩）");
            DisplayMessageLine("v1.114  2015-12-09   读取或者打开文件之前，如果有偏移地址，会按照偏移后地址在右侧显示数据（陈剑南）");
            DisplayMessageLine("v1.115  2015-12-16   修复部分卡机界面按钮操作不能激活的问题（陈剑南）");
            DisplayMessageLine("v1.116  2016-01-08   Tab“配置信息”中选中“FM349”以后将不再选中全部控制字（陈剑南）");
            DisplayMessageLine("v1.117  2016-01-13   支持不同CID，同样兼容旧卡机（固定为1）（陈剑南）");
            DisplayMessageLine("v1.118  2016-01-29   增加：软件计算CRC-A和CRC-B，并且添加到发送数据框后（陈剑南）");
            DisplayMessageLine("v1.119  2016-02-19   修复：脚本RATS()函数不能使用问题；增加：RATS(strCID)可在脚本中使用（陈剑南）");
            DisplayMessageLine("v1.120  2016-02-19   增加：TypeB卡片操作界面，修改TypeA和TypeB界面切换时的FM17寄存器操作（陈剑南）");
            DisplayMessageLine("v1.121  2016-04-11   修改：TypeB卡操作（陈剑南）");
            DisplayMessageLine("v1.122  2016-04-12   “FM349_NEW”控制字增加一列选项，可以锁定控制字当前值，需使用新版\\cfg_349New.xml（陈剑南）");
            DisplayMessageLine("v1.123  2016-05-13   配置字增加FM347.xml(陈楠楠）");
            DisplayMessageLine("v1.124  2016-05-23   增加FM302编程以及FM326在1.8v下编程及读取校验不下电（杨松川）");
            DisplayMessageLine("v1.125  2016-06-03   增加FM347大端模式的配置字解析（杨松川）");
            DisplayMessageLine("v1.126  2016-06-13   增加FM347配置字的读/解析/生成以及手动配置自动完成对位（杨松川）");
            DisplayMessageLine("v1.127  2016-07-06   增加FM347的CT接口下，配置字的擦写以及程序/数据的编程（杨松川）");
            DisplayMessageLine("v1.128  2016-07-11   增加生成80FE的数据背景。（朱军浩）");
            DisplayMessageLine("v1.129  2016-07-18   添加回车键直接发送命令的功能。（陈楠楠）");
            DisplayMessageLine("v1.130  2016-07-08   修改下位机并增加FM347的256bytes写，并提升NVM的编程速度（杨松川）");
            DisplayMessageLine("v1.131  2016-07-25   编程RAM改为RAM_patch区，修改bug并补足4bytes写（杨松川）");
            DisplayMessageLine("v1.132  2016-07-27   修改下位机并增加FM347停CPU特殊按键（杨松川）");
            DisplayMessageLine("v1.133  2016-11-01   安全筛选页增加FM336升频检测功能（朱军浩）");
            DisplayMessageLine("v1.134  2016-11-10   选卡完成后UID_level复位（朱军浩）");
            DisplayMessageLine("v1.135  2016-12-09   完成FM347在小黑屋所有功能的移植（杨松川）");
            DisplayMessageLine("v1.136  2016-12-09   完成FM326银检送检一键式功能移植（杨松川）");
            DisplayMessageLine("v1.137  2016-12-13   非接界面增加M1卡权限字的生成和解析按钮（陈楠楠）");
            DisplayMessageLine("v1.138  2017-01-10   非接界面增加M1卡RESTORE/TRANSFER按钮，并且根据高学兵整理的最新配置字更新了cfg_349New.xml（陈剑南）");
            DisplayMessageLine("v1.139  2017-01-18   增加温度筛卡配置字备份和恢复功能以及336筛卡时关掉安全复位（杨松川）");
            DisplayMessageLine("v1.140  2017-02-07   1,控制字解析部分添加控制字的比较功能;2,添加脚本里取当前时间的命令card.CurrentTime()（陈楠楠）");
            DisplayMessageLine("v1.141  2017-02-10   修改温度筛卡bug（杨松川）");
            DisplayMessageLine("v1.142  2017-02-20   修改347默认波特率编程的bug（杨松川）");
            DisplayMessageLine("v1.143  2017-02-22   增加总参送检一键式配置（杨松川）");
            DisplayMessageLine("v1.144  2017-03-03   安全筛卡界面增加一所送检一键式配置（陈楠楠）");
            DisplayMessageLine("v1.145  2017-03-20   1、FM347、FM349如果修改配置字，会以红色字体区分出来，2、修复（手写）配置字不能被锁定的问题，3、RESTORE指令第二阶段由卡机COS完成（陈剑南）");
            DisplayMessageLine("v1.146  2017-03-21   1、对配置字改动特效进行了调整，2、增加了“FM349_Security”的配置(默认开启报警复位)，“FM349_NEW”默认不开启报警复位（陈剑南）");
            DisplayMessageLine("v1.147  2017-03-29   修改FM347编程备份的bug（杨松川）");
            DisplayMessageLine("v1.148  2017-04-14   增加FM347的8M环振的trim功能（杨松川）");
            DisplayMessageLine("v1.149  2017-04-17   增加FM347的Vdet_vb的trim功能（杨松川）");
            DisplayMessageLine("v1.150  2017-04-21   将Vdet_vb的trim改为AUX输出功能（杨松川）");
            DisplayMessageLine("v1.151  2017-04-26   FM347的RAM编程后读RAM不下电（杨松川）");
            DisplayMessageLine("v1.152  2017-05-02   1，添加获取数据长度超出255字节时的逻辑；2，CMD_List添加open文件的功能和顺序执行功能；3，非接触界面重新排列了一下,并添加“异或”小按钮；4，PPS按钮添加支持VHBR PPS切换指令的功能；5，隐藏“FM309 old配置”界面（陈楠楠）");
            DisplayMessageLine("v1.153  2017-05-03   修正FM347的8M环振trim的bug（杨松川）");
            DisplayMessageLine("v1.154  2017-05-05   新增FM347接触init接口读取和改写ATQA/UID/SAK数据的两个按钮（陈剑南）");
            DisplayMessageLine("v1.155  2017-05-10   修正FM336接触编程的问题。（朱军浩）");
            DisplayMessageLine("v1.156  2017-05-17   新增FM347一键式init_ctrl指令温度筛卡。（杨松川）");
            DisplayMessageLine("v1.157  2017-05-18   新增FM347一键式COS验证的温度筛卡。（杨松川）");
            DisplayMessageLine("v1.158  2017-05-23   新增按照字计算异或值。（闫守礼）");
            DisplayMessageLine("v1.159  2017-05-24   新增FM347的CC认证一键式下载。（杨松川）");
            DisplayMessageLine("v1.160  2017-07-03   兼容 NXP MIFARE Classic EV1，认证时可选UID为第一重还是第二重。（陈楠楠）");
            DisplayMessageLine("v1.161  2017-07-13   新增FM347的CC认证关闭NVM加密选项。（杨松川）");
            DisplayMessageLine("v1.162  2017-08-02   增加FM347内部发卡初始化一键式。（杨松川）");
            DisplayMessageLine("v1.163  2017-08-22   新增对新卡机的支持，主要对TransceiveCL、TransceiveCT和M1认证进行修改。（严鹏飞）");

        }


        private void textBox_APDU_CLA_CT_KeyPress(object sender, KeyPressEventArgs e)
        {
            TransceiveDataCT_listBox.SelectedItem = null;
            KeyFilter(e);
        }

        private void textBox_APDU_INS_CT_KeyPress(object sender, KeyPressEventArgs e)
        {
            TransceiveDataCT_listBox.SelectedItem = null;
            KeyFilter(e);
        }

        private void textBox_APDU_P1_CT_KeyPress(object sender, KeyPressEventArgs e)
        {
            TransceiveDataCT_listBox.SelectedItem = null;
            KeyFilter(e);
        }

        private void textBox_APDU_P2_CT_KeyPress(object sender, KeyPressEventArgs e)
        {
            TransceiveDataCT_listBox.SelectedItem = null;
            KeyFilter(e);
        }

        private void textBox_APDU_P3_CT_KeyPress(object sender, KeyPressEventArgs e)
        {
            TransceiveDataCT_listBox.SelectedItem = null;
            KeyFilter(e);
        }

        private void flash_isp_CheckedChanged(object sender, EventArgs e)
        {
            if (flash_isp.Checked)
            {
                DisplayMessageLine("请确保串口已连接！flash编程操作将包括整个芯片擦除、编程、校验。全自动的哦～～亲");
                MemTypeSelGrop.Enabled = false;
                interface_sel_grpbox.Enabled = false;
                groupBox6.Enabled = false;
                A1Program.Enabled = false;
                button4.Enabled = false;
                ReadEEdata_Button.Enabled = false;
                //SaveAS_Button.Enabled = false;
                //SaveAs_verify.Enabled = false;
                SaveReadbuf.Enabled = false;
                chkReadVerify.Enabled = false;
                CB_NoErase.Visible = true;
                checkBox_350ChipErase.Visible = false;
                checkBox_350ChipErase.Enabled = false;
                groupBox_chips.Enabled = false;
                checkBox_CTHighBaud.Enabled = false;
                comboBox_CTBaud.Enabled = false;
            }
            else
            {
                MemTypeSelGrop.Enabled = true;
                interface_sel_grpbox.Enabled = true;
                groupBox6.Enabled = true;
                A1Program.Enabled = true;
                button4.Enabled = true;
                ReadEEdata_Button.Enabled = true;
                SaveAS_Button.Enabled = true;
                SaveAs_verify.Enabled = true;
                SaveReadbuf.Enabled = true;
                chkReadVerify.Enabled = true;
                CB_NoErase.Visible = false;
                groupBox_chips.Enabled = true;
                if (FM350_radioButton.Checked == true)
                {
                    checkBox_350ChipErase.Visible = true;
                    checkBox_350ChipErase.Enabled = true;
                }
                checkBox_CTHighBaud.Enabled = true;
                comboBox_CTBaud.Enabled = true;
            }
        }

        private void Card_init_Button_Click(object sender, EventArgs e)
        {
            string First16Byte, Second16Byte, cwd3, bytedatacwd7, cwd8, cwd16, tmpStr, uid_Temp, Str, strReceived;
            int temp_cwd3, temp_cwd16;
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            int i;
            int addr = 0;
            int hex_addr_change;
            byte[] bytedata;
            int startaddr;

            tmpStr = "";
            try
            {
                main_note_idx = 0;
                main_note.Clear();
                ConnectReader_Click(null, EventArgs.Empty);

                FM12XX_Card.Init_TDA8007(out strReceived);
                Field_ON_Click(null, EventArgs.Empty);
                //Active_Click(null, EventArgs.Empty);
                FM12XX_Card.REQA(out strReceived);
                if (strReceived != "0400")
                {

                    Reset17_Click(null, EventArgs.Empty);
                    //   return;  //yanshouli,非接触接口出错，退出

                    Init_TDA8007_Click(null, EventArgs.Empty);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived == "Error")
                    {
                        display = "Cold Reset失败 ";
                        DisplayMessageLine(display);
                        //return 1; //CNN 20130325 如果卡没有ATR回发  继续执行
                    }
                    FM12XX_Card.TransceiveCT("0001000000", "02", out receive);
                    FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                    FM12XX_Card.TransceiveCT("0004000010", "02", out receive);
                    First16Byte = receive.Substring(2, 4) + "00" + receive.Substring(8, 26);
                    FM12XX_Card.TransceiveCT("0000000010", "02", out receive);
                    FM12XX_Card.TransceiveCT(First16Byte, "02", out strReceived);
                    if (strReceived != "9000")
                    {
                        display = "CT接口关闭频率检测配置字错误:    \t\t";
                        DisplayMessageLine(display);
                        return;
                    }
                }

                FM12XX_Card.Init_TDA8007(out strReceived);
                Reset17_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                //Active_Click(null, EventArgs.Empty);
                FM12XX_Card.REQA(out display);
                tmpStr += "ATQA：" + display + "\t";
                FM12XX_Card.AntiColl_lv(1, out uid_Temp);
                tmpStr += "UID：" + uid_Temp + "\t";
                FM12XX_Card.Select(1, out display);
                tmpStr += "Sak：" + display;
                DisplayMessageLine(tmpStr);

                display = "COS版本:   \t\t" + "FM309Cos_ROM_BCTC_V07.hex";
                DisplayMessageLine(display);
                /*FM12XX_Card.TransceiveCL("A200", "01", "09", out receive);//CNN 添加认证，使认证时也能配置控制字
                i = FM12XX_Card.TransceiveCL("00000000000000000000000000000000", "01", "09", out receive);//LOAD密钥
                if (i != 0)
                {
                    display = "认证:   \t\t" + receive;
                    DisplayMessageLine(display);
                    return;
                }*/
                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                cwd3 = receive.Substring(6, 2);
                //cwd7 = receive.Substring(14, 2);
                //cwd8 = receive.Substring(16, 2);
                SendData_textBox.Text = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                cwd16 = receive.Substring(0, 2);        //yanshl,2013-10-30
                SendData_textBox.Text += receive;

                //修改配置字                               
                // 写入修改后的配置字   
                //temp_cwd3 = (strToHexByte(cwd3)[0] & 0x07)|0x18; //取低三位(低温配置值)，高温配置固定为011(中间值)
                temp_cwd3 = (strToHexByte(cwd3)[0] & 0x07) | 0x30; //取低三位(低温配置值)，高温配置固定为110(中间值)

                //  写EEPROM的加密因子（cwd7，cwd8）UID的低2字节
                First16Byte = "0C0000" + temp_cwd3.ToString("X2") + "EC027F" + uid_Temp.Substring(0, 4) + "6F81031940A700";
                //First16Byte = "1C00409BE0027F00006FB90099108403";
                FM12XX_Card.WriteBlock("00", First16Byte, out receive);
                display = "WriteBlock 00:   \t\t" + receive;
                DisplayMessageLine(display);

                temp_cwd16 = strToHexByte(cwd16)[0];
                Second16Byte = temp_cwd16.ToString("X2") + "0700000000000000000000005555A5";       //保留频率trim值,yanshl,2013-10-30
                FM12XX_Card.WriteBlock("01", Second16Byte, out receive);

                display = "WriteBlock 01:   \t\t" + receive;
                DisplayMessageLine(display);
                Delay(1);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                Second16Byte = receive;
                SendData_textBox.Text = First16Byte + Second16Byte;
                display = "配置字检测:   \t\t" + First16Byte + Second16Byte;
                DisplayMessageLine(display);
                Delay(1);
                ////初始化安全区内容  刘丹，改到下载完程序后再初始化， by yanshouli
                //FM12XX_Card.WriteBlock("18", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out receive);
                //FM12XX_Card.WriteBlock("19", "0000FFFFFFFFFFFFFFFFFFFFFFFFFFFF", out receive);
                //FM12XX_Card.WriteBlock("1A", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out receive);

                //下载程序  
                //OpenFile_Button_Click(null, EventArgs.Empty);     

                ProgFileLenth = 0;
                open_flag = 1;
                hex_addr_change = 0;
                openFileDialog_Prog = null;
                openFileDialog_Prog = new OpenFileDialog();

                openFileDialog_Prog.InitialDirectory = HexDirectory;
                openFileDialog_Prog.Reset();
                openFileDialog_Scr.Reset();

                if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)//CNN修改 
                {
                    dataGridView_flag = 0;
                    progEEdataGridView.Rows.Clear();
                }
                //openFileDialog_Prog.RestoreDirectory = true;
                //openFileDialog_Prog.FileName = ".\\FM309Cos_ROM_BCTC_V07.hex";              
                openFileDialog_Prog.FileName = ".\\FM1280_BCTC_COS_V0207.hex";
                // if (openFileDialog_Prog.ShowDialog() == DialogResult.OK)
                // {                  
                HexDirectory = openFileDialog_Prog.FileName;
                //temp = HexDirectory.LastIndexOf("\\");
                //HexDirectory = HexDirectory.Substring(0, temp);
                RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
                hexfiledata = new byte[RawCode.Length];
                bin_data = new byte[RawCode.Length];


                StartEEAddr = ProgEEStartAddr.Text;
                EndEEAddr = ProgEEEndAddr.Text;
                for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
                {
                    StartEEAddr = "0" + StartEEAddr;
                }
                for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
                {
                    EndEEAddr = "0" + EndEEAddr;
                }
                ProgEEStartAddr.Text = StartEEAddr;
                ProgEEEndAddr.Text = EndEEAddr;

                for (i = 0; i < ProgFileMaxLen; i++)
                {
                    Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                }
                ProgOrReadEE_FF_flag = 0;
                ProgOrReadEE_80_flag = 0;
                ProgOrReadEE_81_flag = 0;
                ProgOrReadEE_82_flag = 0;
                ProgOrReadEE_83_flag = 0;
                ProgOrReadEE_84_flag = 0;
                ProgOrReadEE_90_flag = 0;

                for (i = 0; i < RawCode.Length; i++)
                {
                    szHex = "";
                    tempStr = RawCode[i];
                    if (tempStr == "")
                    {
                        continue;
                    }
                    if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                    {

                        if (tempStr.Substring(1, 8) == "00000001")//数据结束
                        {
                            break;
                        }
                        if (tempStr.Substring(7, 2) == "04")//段地址
                        {
                            if (tempStr.Substring(11, 2) == "80")//段地址
                            {
                                hex_addr_change = 0x010000;
                                ProgOrReadEE_80_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "81")//段地址
                            {
                                hex_addr_change = 0x020000;
                                ProgOrReadEE_81_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "82")//段地址
                            {
                                hex_addr_change = 0x030000;
                                ProgOrReadEE_82_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "FF")//段地址
                            {
                                hex_addr_change = 0x000000;
                                ProgOrReadEE_FF_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                            {
                                hex_addr_change = 0x060000;
                                ProgOrReadEE_90_flag = 1;
                            }
                            else//段后扩展地址
                            {
                                ProgOrReadEE_EXT_flag = 1;
                                if (tempStr.Substring(11, 2) == "83")//段地址
                                {
                                    hex_addr_change = 0x040000;
                                    ProgOrReadEE_83_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "84")//段地址
                                {
                                    hex_addr_change = 0x050000;
                                    ProgOrReadEE_84_flag = 1;
                                }
                                /* else if (tempStr.Substring(11, 2) == "85")//段地址
                                 {
                                     hex_addr_change = 0x060000;                                    
                                 }   */
                            }
                            // addrStr = tempStr.Substring(11, 2);
                            // hex_addr_change = (int)(strToHexByte(addrStr)[0] * 65536);
                        }
                        else
                        {
                            ProgOrReadEE_FF_flag = 1;
                            datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                            addrStr = tempStr.Substring(3, 4);//记录该行的地址
                            szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                            bytedata = strToHexByte(szHex);
                            addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                            if (addr >= ProgFileLenth)
                            {
                                ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                            }
                            for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                            {
                                Progfilebuf[addr + j] = bytedata[j];
                            }
                        }
                        // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                    }
                }

                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
                // }
                openFileDialog_Prog.Dispose();
                //编程
                Reset17_Click(null, EventArgs.Empty);
                Init_TDA8007_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.TransceiveCL("3180", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                Second16Byte = receive;
                SendData_textBox.Text = First16Byte + Second16Byte;
                display = "配置字检测:   \t\t" + First16Byte + Second16Byte;
                DisplayMessageLine(display);
                ProgEE_Button_Click(null, EventArgs.Empty);

                //修改控制字
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.TransceiveCL("3180", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = "0C007E" + temp_cwd3.ToString("X2") + "EC027F" + uid_Temp.Substring(0, 4) + "6F81031940A700";

                FM12XX_Card.WriteBlock("00", First16Byte, out receive);

                if (receive == "Succeeded")
                    display = "控制字打开安全复位:   \t\t" + "成功";
                else
                    display = "控制字打开安全复位:   \t\t" + "错误！！！！！！！！！！！！";
                DisplayMessageLine(display);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                Second16Byte = receive;
                SendData_textBox.Text = First16Byte + Second16Byte;
                display = "配置字检测:   \t\t" + First16Byte + Second16Byte;
                DisplayMessageLine(display);

                Delay(1);
                //初始化安全区内容（刘丹），改到下载完程序后再初始化， by yanshouli
                FM12XX_Card.WriteBlock("18", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out receive);
                FM12XX_Card.WriteBlock("19", "0000FFFFFFFFFFFFFFFFFFFFFFFFFFFF", out receive);
                FM12XX_Card.WriteBlock("1A", "FF96FFFFFFFFFFFFFFFFFFA578556677", out receive);//关闭审计功能

                Reset17_Click(null, EventArgs.Empty);
                //crc 校验 刘丹
                Reset17_Click(null, EventArgs.Empty);
                Init_TDA8007_Click(null, EventArgs.Empty);
                //Cold_Reset_Click(null, EventArgs.Empty);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                {
                    ConnectReader_Click(null, EventArgs.Empty);
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                    Cold_Reset_Click(null, EventArgs.Empty);
                    FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                    FM12XX_Card.TransceiveCT("0000000010", "02", out strReceived);
                    Str = "0C0000" + temp_cwd3.ToString("X2") + "EC027F" + uid_Temp.Substring(0, 4) + "6F81031940A700";
                    FM12XX_Card.TransceiveCT(Str, "02", out strReceived);
                    if (strReceived == "9000")
                    {
                        display = "点击《白卡》按钮，重新下载COS:    \t\t";
                        DisplayMessageLine(display);
                        return;
                    }
                    else
                    {
                        display = "配置字修改错误:    \t\t";
                        DisplayMessageLine(display);
                        return;
                    }
                }
                else
                {
                    display = "ColdReset ATR:    \t\t" + strReceived;
                    DisplayMessageLine(display);
                }

                FM12XX_Card.SendAPDUCT("0088DE5F02", out receive); //OLD 0088CA0402
                if (receive == "39239000")//old crc=8AE7
                    Str = "正确";
                else
                    Str = "错误";
                display = "接触接口取 COS CRC:    \t\t\t" + receive + "\t" + Str;
                DisplayMessageLine(display);

                //非接接口取CRC
                Init_TDA8007_Click(null, EventArgs.Empty);
                Reset17_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                FM12XX_Card.RATS(out display);
                FM12XX_Card.SendAPDUCL("0088DE5F02", out receive); //OLD 0088CA0402
                if (receive == "39239000")//old crc=8AE7
                    Str = "正确";
                else
                    Str = "错误";
                display = "非接接口取 COS CRC:    \t\t\t" + receive + "\t" + Str;
                DisplayMessageLine(display);
                Reset17_Click(null, EventArgs.Empty);
                Init_TDA8007_Click(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }
        /*
        private void butCwd_Click(object sender, EventArgs e)
        {
            string First16Byte, Second16Byte, cwd3, cwd7, cwd8, tmpStr, uid_Temp, Str, strReceived,strReadCWD16Byte;
            int temp_cwd3;
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            int i;
            int addr = 0;
            int hex_addr_change;
            byte[] bytedata;
            int startaddr;

            tmpStr = "";
            try
            {
                main_note_idx = 0;
                main_note.Clear();
                ConnectReader_Click(null, EventArgs.Empty);

                FM12XX_Card.Init_TDA8007(out strReceived);
                Field_ON_Click(null, EventArgs.Empty);
                //Active_Click(null, EventArgs.Empty);
                FM12XX_Card.REQA(out strReceived);
                if (strReceived != "0400")
                {
                    Reset17_Click(null, EventArgs.Empty);
                    return;  //yanshouli,非接触接口出错，退出
                }

                FM12XX_Card.Init_TDA8007(out strReceived);
                Reset17_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                //Active_Click(null, EventArgs.Empty);
                FM12XX_Card.REQA(out display);
                tmpStr += "ATQA：" + display + "\t";
                FM12XX_Card.AntiColl_lv(1, out uid_Temp);
                tmpStr += "UID：" + uid_Temp + "\t";
                FM12XX_Card.Select(out display);
                tmpStr += "Sak：" + display;
                DisplayMessageLine(tmpStr);

                display = "修改控制字";
                DisplayMessageLine(display);

                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                cwd3 = receive.Substring(6, 2); //取CWD
                strReadCWD16Byte = receive.Substring(0, 32);
                //cwd7 = receive.Substring(14, 2);
                //cwd8 = receive.Substring(16, 2);
                SendData_textBox.Text = receive;

                //修改配置字                               
                // 写入修改后的配置字   
                //temp_cwd3 = (strToHexByte(cwd3)[0] & 0x07)|0x18; //取低三位(低温配置值)，高温配置固定为011(中间值)
                temp_cwd3 = (strToHexByte(cwd3)[0] & 0xC7) | 0x30; //取低三位(低温配置值)，高温配置固定为011(中间值)

                //  写EEPROM的加密因子（cwd7，cwd8）UID的低2字节
                First16Byte = strReadCWD16Byte.Substring(0, 6) + temp_cwd3.ToString("X2") + strReadCWD16Byte.Substring(8, 6) + uid_Temp.Substring(0, 4) + strReadCWD16Byte.Substring(18, 14);
                //First16Byte = "1C00409BE0027F00006FB90099108403";
                FM12XX_Card.WriteBlock("00", First16Byte, out receive);
                display = "WriteBlock 00:   \t\t" + receive;
                DisplayMessageLine(display);
                display = "原配置字:=" + strReadCWD16Byte;
                DisplayMessageLine(display);
                display = "新配置字:=" + First16Byte;
                DisplayMessageLine(display);

                Delay(1);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                Second16Byte = receive;
                SendData_textBox.Text = First16Byte + Second16Byte;
                display = "配置字检测:   \t\t" + First16Byte + Second16Byte;
                DisplayMessageLine(display);
                Delay(1);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }
        */

        private void init_CRC_0A_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived;
            if (r347.Checked == true)
                sendbuf = "0041" + text_0A_P1.Text + text_0A_P2.Text + "00";
            else
                sendbuf = "000A" + text_0A_P1.Text + text_0A_P2.Text + "02";

            int i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
            if (i == 0)
            {
                display = "Data Received:  \t<-\t" + strReceived;
                DisplayMessageLine(display);
            }
            else
            {
                display = "Data Received:  \t<-\t" + "ERROR";
                DisplayMessageLine(display);
                return;
            }
        }


        private void init_AUTH_55_Click(object sender, EventArgs e)
        {
            string auth_key, strReceived;
            int k;

            if (r347.Checked == true)
            {
                FM12XX_Card.Cold_Reset("02", out strReceived);
                k = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("01初始化指令:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("01初始化指令:   \t\t" + strReceived);
                k = FM12XX_Card.TransceiveCT("0054000010", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-key校验-step1:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-key校验-step1:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("DAA1DBB7EA53EDDBA72876EDAB143B76", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-key校验-step2:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-key校验-step2:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("0054000000", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-NVM全擦:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-NVM全擦:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("0054000064", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW0擦写-setp1:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW0擦写-setp1:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("8DC634CA77850000811A000000160000C00489008204100492037F030000000055AAA55A  00000000 111111112222222233333333444444445555555566666666 7239CB35887AFFFF7EE5FFFFFFE9FFFF3FFB76FF7DFBEFFB6DFC80FCFFFFFFFFAA555AA5", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW0擦写-setp2:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW0擦写-setp2:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("0054010048", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW1擦写-setp1:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW1擦写-setp1:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("000000000000000000000000 000155550002AAAA 0000000000000000000000000000000000000000 00000000000000000000000000000000E63041A037280400 900201FF00000000", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW1擦写-setp2:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW1擦写-setp2:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("0054060064", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW6擦写-setp1:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW6擦写-setp1:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("8DC634CA77850000811A000000160000C00489008204100492037F030000000055AAA55A  00000000 111111112222222233333333444444445555555566666666 7239CB35887AFFFF7EE5FFFFFFE9FFFF3FFB76FF7DFBEFFB6DFC80FCFFFFFFFFAA555AA5", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW6擦写-setp2:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW6擦写-setp2:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("0054070104", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW7擦写-setp1:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW7擦写-setp1:   \t\t" + strReceived);

                k = FM12XX_Card.TransceiveCT("00000000", "02", out strReceived);
                if (k != 0)
                {
                    DisplayMessageLine("54指令-CW7擦写-setp2:   \t\t" + "错误！！");
                    return;
                }
                else
                    DisplayMessageLine("54指令-CW7擦写-setp2:   \t\t" + strReceived);
                DisplayMessageLine("54指令 Success");
                return;
            }
            else
            {
                auth_key = SendData_textBox.Text.Replace(" ", "");
                if ((auth_key == "") || (auth_key.Length != 32))
                {
                    display = "认证的密钥长度不对！！！   \t\t";
                    DisplayMessageLine(display);
                    return;
                }
                FM12XX_Card.TransceiveCT("0055000010", "02", out strReceived);//CNN 添加认证，使认证时也能配置控制字
                k = FM12XX_Card.TransceiveCT(auth_key, "02", out strReceived);//LOAD密钥

                if (k != 0)
                {
                    display = "认证:   \t\t" + "错误！！";
                    DisplayMessageLine(display);
                    return;
                }
                display = "认证:   \t\t" + "通过";
                DisplayMessageLine(display);
                return;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string str = richTextBox1.Text.Replace(" ", "");
            float i = str.Length / 2F;
            keylegth_label.Text = i.ToString();
        }

        private void CWD_auth_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (CWD_auth_checkbox.Checked)
            {
                richTextBox1.Visible = true;
                keylegth_label.Visible = true;
                label62.Visible = true;
            }
            else
            {
                richTextBox1.Visible = false;
                keylegth_label.Visible = false;
                label62.Visible = false;
            }
        }

        private void Auth_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Auth_checkBox.Checked)
            {
                key_richTextBox.Visible = true;
                keyl_label.Visible = true;
                label64.Visible = true;
            }
            else
            {
                key_richTextBox.Visible = false;
                keyl_label.Visible = false;
                label64.Visible = false;
            }

        }

        private void key_richTextBox_TextChanged(object sender, EventArgs e)
        {
            string str = key_richTextBox.Text.Replace(" ", "");
            float i = str.Length / 2F;
            keyl_label.Text = i.ToString();
        }

        private void VoltageSel_click(object sender, EventArgs e)
        {
            string strReceived, volt = "";

            if (Volt50.Checked)
                volt = "05";
            else if (Volt18.Checked)
                volt = "0D";
            else if (Volt30.Checked)
                volt = "07";
            else
                return;
            try
            {
                FM12XX_Card.Set_TDA8007_reg("07", volt, out strReceived);
                display = "Reader VCC 切换" + ": \t" + strReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }


        }

        private int ATR_check(int sel, out string str_rlt)
        {
            string strReceived, str_tmp;

            FM12XX_Card.SetField(0, out display);
            FM12XX_Card.Init_TDA8007(out display);
            switch (sel)
            {
                case 1: str_tmp = "01";
                    break;
                case 2: str_tmp = "02";
                    break;
                default: str_tmp = "03";
                    break;                       
            }
            FM12XX_Card.Cold_Reset(str_tmp, out strReceived);
            if (strReceived == "Error")
            {
                FM12XX_Card.Cold_Reset(str_tmp, out strReceived);
                if (strReceived == "Error")
                {
                    str_rlt = " -- NO ATR twice! --";
                    return 1;
                }
            }
            str_rlt = strReceived;
            return 0;
        }

        private int ACTIVE_check(out string str_rlt)
        {
            FM12XX_Card.Init_TDA8007(out display);
            FM12XX_Card.SetField(0, out display);
            FM12XX_Card.Active(out display);
            if (display == "NO REQA")
            {
                FM12XX_Card.Active(out display);
                if (display == "NO REQA")
                {
                    str_rlt = " -- NO REQA twice --";
                    return 1;
                }
            }
            str_rlt = display;
            return 0;
        }

       private int temp_write(int num)
        {
            string strReceived;
            string temp_str;
            byte[] temp_rlt;
            byte tmp_add, tmp_and;

            if (temp_select.SelectedIndex == 0)  //低温
            {
                tmp_and = 0x38;
                switch (num)
                {
                    case 1: tmp_add = 0x07;
                        break;
                    case 2: tmp_add = 0x06;
                        break;
                    case 3: tmp_add = 0x05;
                        break;
                    case 4: tmp_add = 0x04;
                        break;
                    case 5: tmp_add = 0x00;
                        break;
                    case 6: tmp_add = 0x01;
                        break;
                    case 7: tmp_add = 0x02;
                        break;
                    case 8: tmp_add = 0x03;
                        break;
                    default: tmp_add = 0x07;
                        break;
                }
            }
            else  //高温
            {
                tmp_and = 0x07;
                switch (num)
                {
                    case 1: tmp_add = 0x18;
                        break;
                    case 2: tmp_add = 0x10;
                        break;
                    case 3: tmp_add = 0x38;
                        break;
                    case 4: tmp_add = 0x30;
                        break;
                    case 5: tmp_add = 0x28;
                        break;
                    case 6: tmp_add = 0x20;
                        break;
                    case 7: tmp_add = 0x00;
                        break;
                    case 8: tmp_add = 0x08;
                        break;
                    default: tmp_add = 0x18;
                        break;
                }                
            }

            if (Sct.Checked == true)
            {
                if (ATR_check(2, out receive) == 1)
                {
                    DisplayMessageLine(receive);
                    return 1;
                } 
                FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                FM12XX_Card.TransceiveCT("0004000004", "02", out receive);
                temp_rlt = strToHexByte(receive.Substring(2, 8));
                temp_rlt[3] &= tmp_and;
                temp_rlt[3] += tmp_add;
                temp_str = byteToHexStr(4, temp_rlt);
                FM12XX_Card.TransceiveCT("0000000004", "02", out receive);
                FM12XX_Card.TransceiveCT(temp_str, "02", out receive);
                FM12XX_Card.Init_TDA8007(out display);
            }
            else
            {
                if (ACTIVE_check(out strReceived) == 1)
                {
                    DisplayMessageLine(strReceived);
                    return 1;
                }
                FM12XX_Card.SendReceiveCL("32A0", out strReceived);
                FM12XX_Card.ReadBlock("00", out strReceived);
                temp_rlt = strToHexByte(strReceived);
                temp_rlt[3] &= tmp_and;
                temp_rlt[3] += tmp_add;
                temp_str = byteToHexStr(16, temp_rlt);
                FM12XX_Card.WriteBlock("00", temp_str, out strReceived);
                FM12XX_Card.SetField(0, out display);
            }
            return 0;
        }

       int temp_sel()
       {
           int i, j;
           byte[] rlt;
           if (Sct.Checked == true)
           {
               if (ATR_check(2, out receive) == 1)
               {
                   DisplayMessageLine(receive);
                   return 2;
               }
               for (i = 0, j = 0; i < 3; i++)
               {
                   if (FM336.Checked == true)
                       FM12XX_Card.SendAPDUCT("0051000004 00F601 01", out receive);
                   else
                       FM12XX_Card.SendAPDUCT("0051000004 00F603 01", out receive);
                   rlt = strToHexByte(receive);
                   if ((rlt[0] & 0x10) == 0x10)
                       j++;
                   if (FM336.Checked == true)
                       FM12XX_Card.SendAPDUCT("0053000005 00F607 01 00", out receive);
                   else
                       FM12XX_Card.SendAPDUCT("0053000005 00F603 01 00", out receive);
               }
               if (FM336.Checked == true)
                   FM12XX_Card.SendAPDUCT("0051000004 00F601 01", out receive);
               else
                   FM12XX_Card.SendAPDUCT("0051000004 00F603 01", out receive);
               DisplayMessageLine("该档位报警值 --> " + receive);
               FM12XX_Card.Init_TDA8007(out display);
           }
           else
           {
               if (ACTIVE_check(out receive) == 1)
               {
                   DisplayMessageLine(receive);
                   return 2;
               }
               FM12XX_Card.RATS(out display);
               for (i = 0, j = 0; i < 3; i++)
               {
                   if (FM336.Checked == true)
                       FM12XX_Card.SendAPDUCL("0051000004 00F601 01", out receive);
                   else
                       FM12XX_Card.SendAPDUCL("0051000004 00F603 01", out receive);
                   rlt = strToHexByte(receive);
                   if ((rlt[0] & 0x10) == 0x10)
                       j++;
                   if (FM336.Checked == true)
                       FM12XX_Card.SendAPDUCL("0053000005 00F607 01 00", out receive);
                   else
                       FM12XX_Card.SendAPDUCL("0053000005 00F603 01 00", out receive);
               }
               if (FM336.Checked == true)
                   FM12XX_Card.SendAPDUCL("0051000004 00F601 01", out receive);
               else
                   FM12XX_Card.SendAPDUCL("0051000004 00F603 01", out receive);
               DisplayMessageLine("该档位报警值 --> " + receive.Substring(0, 2));
               FM12XX_Card.SetField(0, out display);
           }
           if (j > 1)
               return 1;
           else
               return 0;
       }

        private void program(string file_addr, int flag, bool zf, int patch_size)
        {
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            int i;
            int addr = 0;
            int hex_addr_change;
            byte[] bytedata;

            //下载程序  
            ProgFileLenth = 0;
            open_flag = 1;
            hex_addr_change = 0;
            openFileDialog_Prog = null;
            openFileDialog_Prog = new OpenFileDialog();

            openFileDialog_Prog.InitialDirectory = HexDirectory;
            openFileDialog_Prog.Reset();
            openFileDialog_Scr.Reset();

            if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)//CNN修改 
            {
                dataGridView_flag = 0;
                progEEdataGridView.Rows.Clear();
            }



            openFileDialog_Prog.FileName = file_addr;
            HexDirectory = openFileDialog_Prog.FileName;
            tempStr = HexDirectory.Substring(HexDirectory.Length - 4, 4);      //取文件后缀
            tempStr = tempStr.ToUpper();
            //修改为了支持BIN文件格式,by yanshouli
            if (tempStr == ".HEX")       //HEX文件格式   
            {
                HexDirectory = openFileDialog_Prog.FileName;
                RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
                hexfiledata = new byte[RawCode.Length];
                bin_data = new byte[RawCode.Length];

                StartEEAddr = ProgEEStartAddr.Text;
                EndEEAddr = ProgEEEndAddr.Text;
                for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
                {
                    StartEEAddr = "0" + StartEEAddr;
                }
                for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
                {
                    EndEEAddr = "0" + EndEEAddr;
                }
                ProgEEStartAddr.Text = StartEEAddr;
                ProgEEEndAddr.Text = EndEEAddr;

                for (i = 0; i < ProgFileMaxLen; i++)
                {
                    Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                }
                ProgOrReadEE_FF_flag = 0;
                ProgOrReadEE_80_flag = 0;
                ProgOrReadEE_81_flag = 0;
                ProgOrReadEE_82_flag = 0;
                ProgOrReadEE_83_flag = 0;
                ProgOrReadEE_84_flag = 0;
                ProgOrReadEE_90_flag = 0;

                for (i = 0; i < RawCode.Length; i++)
                {
                    szHex = "";
                    tempStr = RawCode[i];
                    if (tempStr == "")
                    {
                        continue;
                    }
                    if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                    {

                        if (tempStr.Substring(1, 8) == "00000001")//数据结束
                        {
                            break;
                        }
                        if (tempStr.Substring(7, 2) == "02") // 段偏移
                        {
                            hex_addr_change = Convert.ToUInt16(tempStr.Substring(9, 4), 16) << 0x4;
                            continue;
                        }
                        if (tempStr.Substring(7, 2) == "04")//段地址
                        {
                            if ((FM349_radioButton.Checked) || (FM350_radioButton.Checked))
                            {
                                // remove ram segment
                                if (tempStr.Substring(11, 2) == "80")//段地址
                                {
                                    DisplayMessageLine("RAM段已经移除！！！请确认");
                                    break;
                                }
                            }
                            else if (FM336_radioButton.Checked)
                            {
                                if (tempStr.Substring(11, 2) == "80")//段地址
                                {
                                    hex_addr_change = 0x010000;
                                    ProgOrReadEE_80_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "81")//段地址
                                {
                                    hex_addr_change = 0x020000;
                                    ProgOrReadEE_81_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "82")//段地址
                                {
                                    hex_addr_change = 0x030000;
                                    ProgOrReadEE_82_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "83")//段地址
                                {
                                    hex_addr_change = 0x040000;
                                    ProgOrReadEE_83_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "84")//段地址
                                {
                                    hex_addr_change = 0x050000;
                                    ProgOrReadEE_84_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "FF")//段地址
                                {
                                    hex_addr_change = 0x000000;
                                    ProgOrReadEE_FF_flag = 1;
                                }
                                //打开程序时，lib区数据是放在普通程序后面的，
                                else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                                {
                                    hex_addr_change = 0x080000;
                                    ProgOrReadEE_90_flag = 1;
                                }
                                else//段后扩展地址
                                {
                                    ProgOrReadEE_EXT_flag = 1;
                                    if (tempStr.Substring(11, 2) == "85")//段地址
                                    {
                                        hex_addr_change = 0x060000;
                                        ProgOrReadEE_85_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "86")//段地址
                                    {
                                        hex_addr_change = 0x070000;
                                        ProgOrReadEE_86_flag = 1;
                                    }
                                }
                            }
                            else
                            {
                                if (tempStr.Substring(11, 2) == "80")//段地址
                                {
                                    hex_addr_change = 0x010000;
                                    ProgOrReadEE_80_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "81")//段地址
                                {
                                    hex_addr_change = 0x020000;
                                    ProgOrReadEE_81_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "82")//段地址
                                {
                                    hex_addr_change = 0x030000;
                                    ProgOrReadEE_82_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "FF")//段地址
                                {
                                    hex_addr_change = 0x000000;
                                    ProgOrReadEE_FF_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                                {
                                    hex_addr_change = 0x060000;
                                    ProgOrReadEE_90_flag = 1;
                                }
                                else//段后扩展地址
                                {
                                    ProgOrReadEE_EXT_flag = 1;
                                    if (tempStr.Substring(11, 2) == "83")//段地址
                                    {
                                        hex_addr_change = 0x040000;
                                        ProgOrReadEE_83_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "84")//段地址
                                    {
                                        hex_addr_change = 0x050000;
                                        ProgOrReadEE_84_flag = 1;
                                    }
                                    /* else if (tempStr.Substring(11, 2) == "85")//段地址
                                        {
                                            hex_addr_change = 0x060000;                                    
                                        }   */
                                }
                            }
                            // addrStr = tempStr.Substring(11, 2);
                            // hex_addr_change = (int)(strToHexByte(addrStr)[0] * 65536);
                        }
                        else if (tempStr.Substring(7, 2) == "05")
                        {//05 类型字段处理,解决打开部分HEX文件出错的现象,修改by yanshouli 
                        }
                        else if (tempStr.Substring(7, 2) == "03")
                        { // start segment address record 
                        }
                        else
                        {
                            ProgOrReadEE_FF_flag = 1;
                            datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                            addrStr = tempStr.Substring(3, 4);//记录该行的地址
                            szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                            bytedata = strToHexByte(szHex);
                            addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                            if (addr >= ProgFileLenth)
                            {
                                ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                            }
                            for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                            {
                                Progfilebuf[addr + j] = bytedata[j];
                            }
                        }
                        // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                    }
                }
                ProgFileLenth = ((ProgFileLenth + 3) / 4) * 4;    //文件大小取整到4的倍数,by yanshouli 
            }
            else if (tempStr == ".BIN")       //BIN文件格式
            {   //修改支持BIN文件格式,by yanshouli
                FileStream s2 = File.OpenRead(openFileDialog_Prog.FileName);
                ProgFileLenth = (int)s2.Length;
                s2.Read(Progfilebuf, 0, ProgFileLenth);
                s2.Close();
                ProgFileLenth = ((ProgFileLenth + 3) / 4) * 4;
            } 
          
            ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            openFileDialog_Prog.Dispose();
            if (flag == 1)
            {
                CL_Interface.Checked = false;
                CT_Interface.Checked = true;
            }
            else
            {
                CL_Interface.Checked = true;
                CT_Interface.Checked = false;
            }

            if (zf)  // patch区正反码校验功能
            {

                patch_size *= 1024;
                if (patch_size < ProgFileLenth)
                {
                    DisplayMessageLine("补丁小于程序大小，请输入正确的补丁值！！！");
                    return;
                }

                for (i = 0; i < ProgFileLenth; i++)
                    Progfilebuf[patch_size + i] = (byte)(~Progfilebuf[i]);

                ProgFileLenth = patch_size * 2;
                ProgEEStartAddr.Text = "000000";
                ProgEEEndAddr.Text = (ProgFileLenth - 1).ToString("X6");
                ProgDestAddr.Text = (0x14000 - patch_size * 2).ToString("X6");
                Prog_Move_Click(null, EventArgs.Empty);
                ProgEEStartAddr.Text = ProgDestAddr.Text;
                ProgEEEndAddr.Text = "014000";
                DATA_EEtype_radiobutton.Checked = true;
                DisplayMessageLine("提醒：EE类型已选择数据EE");
                DisplayMessageLine("现程序大小为：" + ProgFileLenth / 1024.000 + "K");
            }

            ProgEE_Button_Click(null, EventArgs.Empty);
        }

       private void temp_start_Click(object sender, EventArgs e)
        {
            string strReceived;
            string tempStr, tempStr1;
            int i, tmp, rlt;
            byte[] temp_all;
            bool check;

            strReceived = "";
            tmp = 0;
            temp_rlt.Text = "请耐心等待……";
            FM12XX_Card.SetField(0, out display);
            //FM12XX_Card.Set_FM1715_reg("12", "02", out display); //将非接触的场强降到最小，避免误报警
            FM12XX_Card.Init_TDA8007(out display);

            if (init_y.Checked == true)
            {
                if (Sct.Checked == true)
                {
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived == "Error")
                        DisplayMessageLine("Cold Reset失败 ");
                    else
                        DisplayMessageLine("Cold Reset成功 ");
                    FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                    if (receive != "9000")
                    {
                        DisplayMessageLine("初始化指令失效 ");
                        tmp = 2;
                        goto wait;
                    }
                    FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                    FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                    temp_all = strToHexByte(receive.Substring(2, 64));
                    temp_all[0] &= 0x9F; //整体扩展，EE做程序扩展区
                    temp_all[2] |= 0x08; //开启温度报警
                    temp_all[3] = 0x1F; //-10℃，60℃ 最低档位
                    temp_all[13] = 0x20; //32k patch区
                    if (FM336.Checked == true)
                    {
                        temp_all[10] &= 0xDF; //关闭硬件断点使能
                        temp_all[26] = 00;  //关闭安全复位
                    }
                    tempStr = byteToHexStr(32, temp_all);
                    FM12XX_Card.TransceiveCT("0000000020", "02", out receive);
                    FM12XX_Card.TransceiveCT(tempStr, "02", out receive);
                    if (readers_cbox.Text.Substring(0, 16) != "FMSH Reader V2.0")  //新版卡机写操作无返回，再接着读会出错
                    {
                        FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                        FM12XX_Card.Init_TDA8007(out display);
                        if (tempStr != receive.Substring(2, 64))
                        {
                            DisplayMessageLine("写控制字失败 ");
                            tmp = 3;
                            goto wait;
                        }
                    }
                    DisplayMessageLine("写控制字成功 ");
                }
                else if (Scl.Checked == true)
                {
                    if (ACTIVE_check(out receive) == 1)
                    {
                        DisplayMessageLine(receive);
                        tmp = 5;
                        goto wait;
                    }
                    DisplayMessageLine(receive);
                    FM12XX_Card.SendReceiveCL("32A0", out strReceived);
                    FM12XX_Card.ReadBlock("00", out strReceived);
                    tempStr = strReceived;
                    FM12XX_Card.ReadBlock("01", out strReceived);
                    tempStr += strReceived;
                    temp_all = strToHexByte(tempStr);
                    temp_all[0] &= 0x9F; //整体扩展，EE做程序扩展区
                    temp_all[2] |= 0x08; //开启温度报警
                    temp_all[3] = 0x1F; //-10℃，60℃ 最低档位
                    temp_all[13] = 0x20; //32k patch区
                    if (FM336.Checked == true)
                    {
                        temp_all[10] &= 0xDF; //关闭硬件断点使能
                        temp_all[26] = 00;  //关闭安全复位
                    }
                    tempStr = byteToHexStr(32, temp_all);
                    FM12XX_Card.WriteBlock("00", tempStr.Substring(0, 32), out strReceived);
                    FM12XX_Card.WriteBlock("01", tempStr.Substring(32, 32), out strReceived);
                    FM12XX_Card.ReadBlock("00", out strReceived);
                    tempStr1 = strReceived;
                    FM12XX_Card.ReadBlock("01", out strReceived);
                    tempStr1 += strReceived;
                    FM12XX_Card.SetField(0, out display);
                    if (tempStr != tempStr1)
                    {
                        DisplayMessageLine("写控制字失败 ");
                        tmp = 3;
                        goto wait;
                    }
                    DisplayMessageLine("写控制字成功 ");
                }

            }
            else
            {
                DisplayMessageLine("跳过控制字初始化——保留上次温度trim值 ");
                if (temp_select.SelectedIndex == 1 || temp_select.SelectedIndex == 3)
                    goto over_program;
            }

            //下载程序  
            if (FM309.Checked == true)
            {
                FM309_radioButton.Checked = true;
                tempStr = ".\\FM309Cos.hex";
            }
            else if (FM326.Checked == true)
            {
                FM309_radioButton.Checked = true;
                tempStr = ".\\FM326Cos.hex";
            }
            else
            {
                FM336_radioButton.Checked = true;
                tempStr = ".\\FM336Cos.hex";
            }

            if (Sct.Checked == true)
                tmp = 1;
            else
                tmp = 0;
            check = false;
            program(tempStr, tmp, check, 0);
            if (display == "编程失败!   ")
            {
                tmp = 8;
                goto wait;
            }
            //FM12XX_Card.Set_FM1715_reg("12", "02", out display); //将非接触的场强降到最小，避免误报警
            if (temp_select.SelectedIndex == 1 || temp_select.SelectedIndex == 3)
                goto over_program;

            if (Sct.Checked == true)
            {
                if (ATR_check(2, out receive) == 1)
                {
                    DisplayMessageLine(receive);
                    tmp = 10;
                    goto wait;
                }
            }
            else
            {
                if (ACTIVE_check(out receive) == 1)
                {
                    DisplayMessageLine(receive);
                    tmp = 9;
                    goto wait;
                }
                FM12XX_Card.RATS(out display);
                if (display == "Error")
                { 
                    tmp = 10;
                    goto wait;                    
                }
            }
            FM12XX_Card.SetField(0, out display);
            FM12XX_Card.Init_TDA8007(out display);

            DisplayMessageLine("延时30秒…… ");
            Delay(30);
            DisplayMessageLine("延时结束 ");

            if (temp_select.SelectedIndex == 0)
                DisplayMessageLine("低温筛卡开始 ");
            else 
                DisplayMessageLine("高温筛卡开始 ");

            for (i = 1; i < 9; i++ )
            {
                rlt = temp_sel();
                if (rlt == 1 && i < 8)
                {
                    if (temp_write(i + 1) == 1)
                    {
                        tmp = 6;
                        break;
                    }
                }
                else if (rlt == 0)
                    break;
                else
                { 
                        tmp = 99;
                        DisplayMessageLine("-- 最高档仍然报警！ --");
                        goto wait;                    
                }
            }
            if (i == 1)
            { 
                tmp = 99;
                DisplayMessageLine("-- 最低档位不报警！ --");
            }
            else if(i <= 8)
            { 
                tmp = 1;
                if (temp_write(i - 1) == 1)
                {
                    tmp = 6;
                    DisplayMessageLine("报警档位修改失败需要改小一档！" ); 
                }
                else
                    DisplayMessageLine(" 报警档位 --> " + (i-1).ToString("X2"));            
            }

        over_program: if (temp_select.SelectedIndex == 1 || temp_select.SelectedIndex == 3)
            {
                DisplayMessageLine("延时45秒……");
                Delay(45);
                DisplayMessageLine("延时结束");
                i = temp_sel();
                if (i == 0)
                    tmp = 1;
                else if (i == 1)
                    tmp = 99;
                else
                    tmp = 7;
            }
        wait: FM12XX_Card.Init_TDA8007(out display);
            FM12XX_Card.SetField(0, out display);
            switch(tmp)
            {
                case 1: temp_rlt.Text = "恭喜，卡片合格！";
                    bkre(); //增加了温度配置备份
                    break;
                case 2: temp_rlt.Text = "初始化指令失效，请检查卡片是否插好或重换新卡！";
                    break;
                case 3: temp_rlt.Text = "控制字修改失败，请重试或换新卡！";
                    break;
                case 4: temp_rlt.Text = "升温再判断！";
                    break;
                case 5: temp_rlt.Text = "选卡失败，请检查是否有感应线圈或重换新卡！";
                    break;
                case 6: temp_rlt.Text = "调整档位时芯片失灵！";
                    break;
                case 7: temp_rlt.Text = "检验时芯片失灵！";
                    break;
                case 8: temp_rlt.Text = "芯片编程失败！";
                    break;
                case 9:  temp_rlt.Text = "程序下载后在高低温下无法正常选卡！";
                    break;
                case 10:  temp_rlt.Text = "CPU无法正常运行！";
                    break;
                default: temp_rlt.Text = "很遗憾，不合格！";
                    break;            
            }              
            DisplayMessageLine("------------程序运行结束------------- ");
        }

        private void Freq_trim_button_Click(object sender, EventArgs e)
        {
            string sendbuf, strReceived, sendbuf_data, APDU_data = "", freq_trim, CFG_WORD, trim_info, dfs_stv, dfs_cfg, CFW0_BAK, CFW2_BAK, CFW12_BAK, CFW13_BAK, CFW14_BAK, First16Byte; //备份需要修改的控制字
            string byte0, byte1, byte2, byte3, byte4, byte5, byte6, byte7;
            string strlen;
            int trimflag;
            int frq_ref;
            int interface_flag = 2;//CL CT 编程按钮check  标志  0：CL   1：CT
            int Chip_Type = 0;     //芯片类型 标识 0：FM309； 1：FM326； 2：FM336；
            double dfs_dev, FourM_dev, dfs_value, FourM_value, dfs_t, ref_frq;

            int hex_addr_change;
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            byte[] bytedata;
            int addr = 0;
            try
            {
                switch (Chip_comboBox.Text)
                {
                    case "FM309": Chip_Type = 0; break;
                    case "FM326": Chip_Type = 1; break;
                    case "FM336": Chip_Type = 2; break;
                    default: Chip_Type = 0; break;
                }

                if (CT_Interface.Checked)
                {
                    interface_flag = 1;//接触
                    ref_frq = 3.58;
                }
                else
                {
                    interface_flag = 0;//非接 
                    ref_frq = 13.56;
                }

                switch (DfsCFG_comboBox1.Text)
                {
                    case "[0]1.5MHz": dfs_cfg = "00"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "1.5MHz"; dfs_t = 1.5; break;
                    case "[1]2.9MHz": dfs_cfg = "01"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "2.9MHz"; dfs_t = 2.9; break;
                    case "[2]4.2MHz": dfs_cfg = "02"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "4.2MHz"; dfs_t = 4.2; break;
                    case "[3]5.6MHz": dfs_cfg = "03"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "5.6MHz"; dfs_t = 5.6; break;
                    case "[4]6.9MHz": dfs_cfg = "04"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "6.9MHz"; dfs_t = 6.9; break;
                    case "[5]8.1MHz": dfs_cfg = "05"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "8.1MHz"; dfs_t = 8.1; break;
                    case "[6]9.4MHz": dfs_cfg = "06"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "9.4MHz"; dfs_t = 9.4; break;
                    case "[7]10.7MHz": dfs_cfg = "07"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "10.7MHz"; dfs_t = 10.7; break;
                    case "[8]11.9MHz": dfs_cfg = "08"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "11.9MHz"; dfs_t = 11.9; break;
                    case "[9]13.2MHz": dfs_cfg = "09"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "13.2MHz"; dfs_t = 13.2; break;
                    case "[A]14.3MHz": dfs_cfg = "0A"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "14.3MHz"; dfs_t = 14.3; break;
                    case "[B]15.5MHz": dfs_cfg = "0B"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "15.5MHz"; dfs_t = 15.5; break;
                    case "[C]16.7MHz": dfs_cfg = "0C"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "16.7MHz"; dfs_t = 16.7; break;
                    case "[D]17.8MHz": dfs_cfg = "0D"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x8f; dfs_stv = "17.8MHz"; dfs_t = 17.8; break;
                    case "[E]19.0MHz": dfs_cfg = "0E"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x8f; dfs_stv = "19.0MHz"; dfs_t = 19.0; break;
                    case "[F]20.1MHz": dfs_cfg = "0F"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x8f; dfs_stv = "20.1MHz"; dfs_t = 20.1; break;
                    case "[10]21.2MHz": dfs_cfg = "10"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x4f; dfs_stv = "21.2MHz"; dfs_t = 21.2; break;
                    case "[11]22.4MHz": dfs_cfg = "11"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x4f; dfs_stv = "22.4MHz"; dfs_t = 22.4; break;
                    case "[12]23.5MHz": dfs_cfg = "12"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x4f; dfs_stv = "23.5MHz"; dfs_t = 23.5; break;
                    case "[13]24.6MHz": dfs_cfg = "13"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x4f; dfs_stv = "24.6MHz"; dfs_t = 24.6; break;
                    case "[14]25.6MHz": dfs_cfg = "14"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x4f; dfs_stv = "25.6MHz"; dfs_t = 25.6; break;
                    case "[15]26.7MHz": dfs_cfg = "15"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x4f; dfs_stv = "26.7MHz"; dfs_t = 26.7; break;
                    case "[16]27.7MHz": dfs_cfg = "16"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x4f; dfs_stv = "27.7MHz"; dfs_t = 27.7; break;
                    case "[17]28.8MHz": dfs_cfg = "17"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x4f; dfs_stv = "28.8MHz"; dfs_t = 28.8; break;
                    case "[18]29.9MHz": dfs_cfg = "18"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "29.9MHz"; dfs_t = 29.9; break;
                    case "[19]30.9MHz": dfs_cfg = "19"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "30.9MHz"; dfs_t = 30.9; break;
                    case "[1A]31.9MHz": dfs_cfg = "1A"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "31.9MHz"; dfs_t = 31.9; break;
                    case "[1B]32.9MHz": dfs_cfg = "1B"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "32.9MHz"; dfs_t = 32.9; break;
                    case "[1C]33.9MHz": dfs_cfg = "1C"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "33.9MHz"; dfs_t = 33.9; break;
                    case "[1D]35.2MHz": dfs_cfg = "1D"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "35.2MHz"; dfs_t = 35.2; break;
                    case "[1E]36.3MHz": dfs_cfg = "1E"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "36.3MHz"; dfs_t = 36.3; break;
                    case "[1F]37.1MHz": dfs_cfg = "1F"; if (interface_flag == 1) frq_ref = 0x0f; else frq_ref = 0x3f; dfs_stv = "37.1MHz"; dfs_t = 37.1; break;
                    default:
                        frq_ref = 0x00;
                        dfs_stv = "";
                        display = "CFG输入错误！ 有效输入范围为0x00~0x1F 。";
                        DisplayMessageLine(display);
                        Init_TDA8007_Click(null, null);
                        return;
                        break;
                }

                if (interface_flag == 1) //接触界面校准
                {//////////////////////////////////读取控制字，备份,并写配置字为整体RAM补丁模式/////////CNN 20140304////////////////////
                    Reset17_Click(null, EventArgs.Empty);
                    FM12XX_Card.Init_TDA8007(out strReceived);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived == "Error")
                    {
                        display = "Cold Reset失败 ";
                        DisplayMessageLine(display);
                    }
                    //发送01指令
                    sendbuf = "0001000000";
                    FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (strReceived != "9000")
                    {
                        DisplayMessageLine("01初始化指令出错!!!");
                        return;
                    }

                    //发送02指令
                    sendbuf = "0002010000";
                    i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (strReceived != "9000")
                    {
                        display = "02初始化指令出错!!!";
                        DisplayMessageLine(display);
                        return;
                    }

                    //发送04读指令
                    sendbuf = "0004000020";
                    i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (i != 0)
                    {
                        display = "04读指令出错!!!";
                        DisplayMessageLine(display);
                        return;
                    }
                    CFG_WORD = strReceived.Substring(2, 64);
                    DisplayMessageLine("备份控制字： " + CFG_WORD);
                }
                else   //非接界面校准
                {
                    Reset17_Click(null, EventArgs.Empty);
                    FM12XX_Card.Init_TDA8007(out strReceived);
                    FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                    FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                    FM12XX_Card.REQA(out display);
                    if (display == "Error")
                    {
                        DisplayMessageLine("REQA Error!");
                        return;
                    }
                    FM12XX_Card.AntiColl_lv(1, out display);
                    if (display == "Error")
                    {
                        DisplayMessageLine("AntiColl Error!");
                        return;
                    }
                    FM12XX_Card.Select(1, out display);
                    if (display == "Error")
                    {
                        DisplayMessageLine("Select Error!");
                        return;
                    }
                    i = FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                    i = FM12XX_Card.ReadBlock("00", out strReceived);
                    CFG_WORD = strReceived.Substring(0, 32);
                    i = FM12XX_Card.ReadBlock("01", out strReceived);
                    CFG_WORD = CFG_WORD + strReceived.Substring(0, 32);
                    DisplayMessageLine("备份控制字： "+CFG_WORD);
                }
                SendData_textBox.Text = CFG_WORD;
                CFW0_BAK = CFG_WORD.Substring(0, 2);   //备份需要修改的控制字
                CFW2_BAK = CFG_WORD.Substring(4, 2);
                CFW12_BAK = CFG_WORD.Substring(24, 2);
                CFW13_BAK = CFG_WORD.Substring(26, 2);
                CFW14_BAK = CFG_WORD.Substring(28, 2);

                /////接触为RAM下载后不下电，直接切换到CPU 执行程序//// CNN 20140304///////////      
                if (freq_cos_check.Checked != true)
                {
                    if (interface_flag == 1) //接触界面校准
                    {
                        FM12XX_Card.TransceiveCT("0004000020", "02", out First16Byte);
                        if (Trim_init.Checked == true)  //trim值都恢复到初始值。
                        {
                            First16Byte = ((strToHexByte(CFW0_BAK)[0] & 0x9F) | 0x40).ToString("X2") + First16Byte.Substring(4, 2) + "00" + First16Byte.Substring(8, 20) + ((strToHexByte(CFW13_BAK)[0] & 0x80) | 0x07).ToString("X2") + "00" + "003702"
                                + First16Byte.Substring(38, 16) + "00" + First16Byte.Substring(56, 10); //关闭安全监测以及加密奇偶校验  zhujunhao 2014.3.11//  //关闭各种异常复位使能  zhujunhao 2014.10.23//

                        }
                        else
                        {
                            First16Byte = ((strToHexByte(CFW0_BAK)[0] & 0x9F) | 0x40).ToString("X2") + First16Byte.Substring(4, 2) + "00" + First16Byte.Substring(8, 20) + ((strToHexByte(CFW13_BAK)[0] & 0x80) | 0x07).ToString("X2") + "00" + First16Byte.Substring(32, 2)
                                + First16Byte.Substring(34, 20) + "00" + First16Byte.Substring(56, 10); //关闭安全监测以及加密奇偶校验  zhujunhao 2014.3.11//  //关闭各种异常复位使能  zhujunhao 2014.10.23//
                        } 
                        FM12XX_Card.TransceiveCT("0000000020", "02", out strReceived);
                        FM12XX_Card.TransceiveCT(First16Byte, "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            display = "配置字修改 RAM 打补丁  失败！！";
                            DisplayMessageLine(display);
                            return;
                        }
                        //发送04读指令
                        sendbuf = "0004000020";
                        i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                        if (i == 0)
                        {
                            display = "Data Received:  \t<-\t" + strReceived;
                            DisplayMessageLine(display);
                        }
                        else
                        {
                            display = "Data Received:  \t<-\t" + "ERROR";
                            DisplayMessageLine(display);
                            return;
                        }

                        //if (CT_Interface.Checked == false)
                        //{
                        //    CL_Interface.Checked = false;
                        //    CT_Interface.Checked = true;
                        //    interface_flag = 1;//接触
                        //}
                        Init_TDA8007_Click(null, null);
                    }
                    else                    //非接界面校准，EEPROM补丁
                    {
                        i = FM12XX_Card.ReadBlock("00", out First16Byte);
                        if (Trim_init.Checked == true)  //trim值都恢复到初始值。
                        {
                            First16Byte = ((strToHexByte(CFW0_BAK)[0] & 0x9F) | 0x00).ToString("X2") + First16Byte.Substring(2, 2) + "00" + First16Byte.Substring(6, 20) + ((strToHexByte(CFW13_BAK)[0] & 0x80) | 0x10).ToString("X2") + "00" + "00"; //关闭安全监测以及加密奇偶校验  EEPROM 16K补丁//
                        }
                        else
                        {
                            First16Byte = ((strToHexByte(CFW0_BAK)[0] & 0x9F) | 0x00).ToString("X2") + First16Byte.Substring(2, 2) + "00" + First16Byte.Substring(6, 20) + ((strToHexByte(CFW13_BAK)[0] & 0x80) | 0x10).ToString("X2") + "00" + First16Byte.Substring(30, 2); //关闭安全监测以及加密奇偶校验  EEPROM 16K补丁//
                        }
                        i = FM12XX_Card.WriteBlock("00", First16Byte, out strReceived);
                        if (i == 1)
                        {
                            display = "配置字修改 EEPROM 打补丁  失败！！";
                            DisplayMessageLine(display);
                            return;
                        }
                        i = FM12XX_Card.ReadBlock("00", out strReceived);
                        if (i == 0)
                        {
                            display = "EEPROM PATCH CFW 1:  \t<-\t" + strReceived;
                            DisplayMessageLine(display);
                        }
                        else
                        {
                            display = "Data Received:  \t<-\t" + "ERROR";
                            DisplayMessageLine(display);
                            return;
                        }
                        //关各种异常复位使能 2014.10.23 zhujunhao
                        i = FM12XX_Card.ReadBlock("01", out First16Byte);
                        if (Trim_init.Checked == true)  //trim值都恢复到初始值。
                        {
                            First16Byte = "3702" + First16Byte.Substring(4, 16) + "00" + First16Byte.Substring(22, 10);
                        }
                        else
                        {
                            First16Byte = First16Byte.Substring(0, 20) + "00" + First16Byte.Substring(22, 10);
                        }
                        i = FM12XX_Card.WriteBlock("01", First16Byte, out strReceived);
                        if (i == 1)
                        {
                            display = "配置字修改 EEPROM 打补丁  失败！！";
                            DisplayMessageLine(display);
                            return;
                        }
                        i = FM12XX_Card.ReadBlock("01", out strReceived);
                        if (i == 0)
                        {
                            display = "EEPROM PATCH CFW 2:  \t<-\t" + strReceived;
                            DisplayMessageLine(display);
                        }
                        else
                        {
                            display = "Data Received:  \t<-\t" + "ERROR";
                            DisplayMessageLine(display);
                            return;
                        }
                        Reset17_Click(null, EventArgs.Empty);
                    }

                    //打开文件//////////////zhujunhao  2014.3.12////////
                    ProgFileLenth = 0;
                    open_flag = 1;
                    hex_addr_change = 0;
                    openFileDialog_Prog = null;
                    openFileDialog_Prog = new OpenFileDialog();

                    openFileDialog_Prog.InitialDirectory = HexDirectory;
                    openFileDialog_Prog.Reset();
                    openFileDialog_Scr.Reset();

                    if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)//CNN修改 
                    {
                        dataGridView_flag = 0;
                        progEEdataGridView.Rows.Clear();
                    }
                    //openFileDialog_Prog.RestoreDirectory = true;
                    //openFileDialog_Prog.FileName = ".\\FM309Cos_ROM_BCTC_V07.hex";
                    switch (Chip_Type)
                    {
                        case 0: openFileDialog_Prog.FileName = ".\\FM309_TRIM.hex"; break;
                        case 1: openFileDialog_Prog.FileName = ".\\FM309_TRIM.hex"; ; break;
                        case 2: if (interface_flag == 1) openFileDialog_Prog.FileName = ".\\FM336_TRIM_CT.hex"; else openFileDialog_Prog.FileName = ".\\FM336_TRIM_CL.hex"; break;
                        default: openFileDialog_Prog.FileName = ".\\FM309_TRIM.hex"; ; break;
                    }              
                    //openFileDialog_Prog.FileName = ".\\FM309Cos_frq_cal_v3.hex";
                    // if (openFileDialog_Prog.ShowDialog() == DialogResult.OK)
                    // {                  
                    HexDirectory = openFileDialog_Prog.FileName;
                    //temp = HexDirectory.LastIndexOf("\\");
                    //HexDirectory = HexDirectory.Substring(0, temp);
                    RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
                    hexfiledata = new byte[RawCode.Length];
                    bin_data = new byte[RawCode.Length];


                    StartEEAddr = ProgEEStartAddr.Text;
                    EndEEAddr = ProgEEEndAddr.Text;
                    for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
                    {
                        StartEEAddr = "0" + StartEEAddr;
                    }
                    for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
                    {
                        EndEEAddr = "0" + EndEEAddr;
                    }
                    ProgEEStartAddr.Text = StartEEAddr;
                    ProgEEEndAddr.Text = EndEEAddr;

                    for (i = 0; i < ProgFileMaxLen; i++)
                    {
                        Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                    }
                    ProgOrReadEE_FF_flag = 0;
                    ProgOrReadEE_80_flag = 0;
                    ProgOrReadEE_81_flag = 0;
                    ProgOrReadEE_82_flag = 0;
                    ProgOrReadEE_83_flag = 0;
                    ProgOrReadEE_84_flag = 0;
                    ProgOrReadEE_90_flag = 0;

                    for (i = 0; i < RawCode.Length; i++)
                    {
                        szHex = "";
                        tempStr = RawCode[i];
                        if (tempStr == "")
                        {
                            continue;
                        }
                        if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                        {

                            if (tempStr.Substring(1, 8) == "00000001")//数据结束
                            {
                                break;
                            }
                            if (tempStr.Substring(7, 2) == "04")//段地址
                            {
                                if (tempStr.Substring(11, 2) == "80")//段地址
                                {
                                    hex_addr_change = 0x010000;
                                    ProgOrReadEE_80_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "81")//段地址
                                {
                                    hex_addr_change = 0x020000;
                                    ProgOrReadEE_81_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "82")//段地址
                                {
                                    hex_addr_change = 0x030000;
                                    ProgOrReadEE_82_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "FF")//段地址
                                {
                                    hex_addr_change = 0x000000;
                                    ProgOrReadEE_FF_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                                {
                                    hex_addr_change = 0x060000;
                                    ProgOrReadEE_90_flag = 1;
                                }
                                else//段后扩展地址
                                {
                                    ProgOrReadEE_EXT_flag = 1;
                                    if (tempStr.Substring(11, 2) == "83")//段地址
                                    {
                                        hex_addr_change = 0x040000;
                                        ProgOrReadEE_83_flag = 1;
                                    }
                                    else if (tempStr.Substring(11, 2) == "84")//段地址
                                    {
                                        hex_addr_change = 0x050000;
                                        ProgOrReadEE_84_flag = 1;
                                    }
                                    /* else if (tempStr.Substring(11, 2) == "85")//段地址
                                     {
                                         hex_addr_change = 0x060000;                                    
                                     }   */
                                }
                                // addrStr = tempStr.Substring(11, 2);
                                // hex_addr_change = (int)(strToHexByte(addrStr)[0] * 65536);
                            }
                            else
                            {
                                ProgOrReadEE_FF_flag = 1;
                                datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                                addrStr = tempStr.Substring(3, 4);//记录该行的地址
                                szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                                bytedata = strToHexByte(szHex);
                                addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                                if (addr >= ProgFileLenth)
                                {
                                    ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                                }
                                for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                                {
                                    Progfilebuf[addr + j] = bytedata[j];
                                }
                            }
                            // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                        }
                    }

                    ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
                    // }
                    openFileDialog_Prog.Dispose();

                    //编程/
                    ProgEE_Button_Click(null, null);
                    //启动CPU/
                    if (interface_flag == 1)
                        FM12XX_Card.TransceiveCT("0008050000", "02", out strReceived);
                    else
                    {
                        FM12XX_Card.Init_TDA8007(out strReceived);
                        Reset17_Click(null, EventArgs.Empty);
                        FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                        FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                        FM12XX_Card.REQA(out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("REQA Error!");
                            return;
                        }
                        FM12XX_Card.AntiColl_lv(1, out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("AntiColl Error!");
                            return;
                        }
                        FM12XX_Card.Select(1, out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("Select Error!");
                            return;
                        }
                        i = FM12XX_Card.TransceiveCL("E031", "01", "09", out strReceived);
                    }
                }
                else         //已有trim COS，直接通过接触或者非接指令启动CPU
                {
                    if (interface_flag == 1)
                    {
                        //Init
                        Init_TDA8007_Click(null, null);
                        //Cold reset
                        Cold_Reset_Click(null, null);
                    }
                    else
                    {
                        FM12XX_Card.Init_TDA8007(out strReceived);
                        Reset17_Click(null, EventArgs.Empty);
                        FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                        FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                        FM12XX_Card.REQA(out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("REQA Error!");
                            return;
                        }
                        FM12XX_Card.AntiColl_lv(1, out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("AntiColl Error!");
                            return;
                        }
                        FM12XX_Card.Select(1, out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("Select Error!");
                            return;
                        }
                        i = FM12XX_Card.TransceiveCL("E031", "01", "09", out strReceived); //RATS
                    }
                }
                //校准指令
                //FD CMD
                if (FreqTrim_radioButton.Checked == true)//校准
                {
                    if(interface_flag == 1)
                        APDU_data = "00" + "FD" + dfs_cfg + "00" + " " + "08" + " " + "";
                    else
                        APDU_data = "00" + "FD" + dfs_cfg + "02" + " " + "08" + " " + "";
                }
                else//读取
                {
                    if (interface_flag == 1)//zjh
                        APDU_data = "00" + "FD" + dfs_cfg + "10" + " " + "08" + " " + "";
                    else
                        APDU_data = "00" + "FD" + dfs_cfg + "12" + " " + "08" + " " + "";
                }
                if (interface_flag == 1) //接触界面校准
                {
                    //i = FM12XX_Card.SendAPDUCT(APDU_data, out receive);
                    //改为单步发送。 2014.3.11 zhujunhao//
                    i = FM12XX_Card.TransceiveCT(APDU_data, "02", out receive);
                    if (receive.Substring(0, 2) != "61")
                    {
                        display = "校准指令回复错误，请重试！";
                        DisplayMessageLine(display);
                        //////////////////恢复控制字////////////////zhujunhao 20140311///////////
                        Init_TDA8007_Click(null, EventArgs.Empty);
                        Cold_Reset_Click(null, EventArgs.Empty);
                        FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("0000000020", "02", out strReceived);
                        FM12XX_Card.TransceiveCT(CFG_WORD, "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            DisplayMessageLine("恢复控制字   失败！！");
                        }
                        ////////////////////////////////////////////////////////
                        return;
                    }
                    strlen = receive.Substring(2, 2);
                    APDU_data = "00C00000" + strlen;
                    i = FM12XX_Card.TransceiveCT(APDU_data, "02", out receive);
                    if (receive.Substring(0, 2) != "C0")
                    {
                        display = "取数据错误，请重试！";
                        DisplayMessageLine(display);
                        //////////////////恢复控制字////////////////zhujunhao 20140311///////////
                        Init_TDA8007_Click(null, EventArgs.Empty);
                        Cold_Reset_Click(null, EventArgs.Empty);
                        FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("0000000020", "02", out strReceived);
                        FM12XX_Card.TransceiveCT(CFG_WORD, "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            DisplayMessageLine("恢复控制字   失败！！");
                        }
                        ////////////////////////////////////////////////////////
                        return;
                    }
                    receive = receive.Substring(2, strToHexByte(strlen)[0] * 2);
                    //改为单步发送。 2014.3.11 zhujunhao//
                }
                else //非接界面校准
                {
                    i = FM12XX_Card.SendAPDUCL(APDU_data, out receive);
                }
                trim_info = receive;
                display = "Data Received:  \t<-\t" + receive;
                DisplayMessageLine(display);
                //校准值
                byte0 = trim_info.Substring(0, 2);
                if (byte0 != "5A")
                {
                    display = "回复信息错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    DisplayMessageLine(display);
                    //////////////////恢复控制字////////////////CNN 20140304///////////
                    if (interface_flag == 1)
                    {
                        Init_TDA8007_Click(null, EventArgs.Empty);
                        Cold_Reset_Click(null, EventArgs.Empty);
                        FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("0000000020", "02", out strReceived);
                        FM12XX_Card.TransceiveCT(CFG_WORD, "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            DisplayMessageLine("恢复控制字   失败！！");
                        }
                        ////////////////////////////////////////////////////////
                        return;
                    }
                    else
                    {
                        Reset17_Click(null, EventArgs.Empty);
                        FM12XX_Card.Init_TDA8007(out strReceived);
                        FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                        FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                        FM12XX_Card.REQA(out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("REQA Error!");
                            return;
                        }
                        FM12XX_Card.AntiColl_lv(1, out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("AntiColl Error!");
                            return;
                        }
                        FM12XX_Card.Select(1, out display);
                        if (display == "Error")
                        {
                            DisplayMessageLine("Select Error!");
                            return;
                        }
                        i = FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                        i = FM12XX_Card.WriteBlock("00", CFG_WORD.Substring(0,32), out strReceived);
                        i = FM12XX_Card.WriteBlock("01", CFG_WORD.Substring(32,32), out strReceived);
                        if (i == 1)
                        {
                            display = "恢复控制字   失败！！";
                            DisplayMessageLine(display);
                            return;
                        }                        
                    }
                }
                byte1 = trim_info.Substring(2, 2);
                byte2 = trim_info.Substring(4, 2);
                byte3 = trim_info.Substring(6, 2);
                byte4 = trim_info.Substring(8, 2);
                byte5 = trim_info.Substring(10, 2);
                byte6 = trim_info.Substring(12, 2);
                byte7 = trim_info.Substring(14, 2);
                freq_trim = byte1;

                if (Trim_init.Checked == false)  //trim值都恢复到初始值。
                    CFG_WORD = CFG_WORD.Substring(0, 32) + freq_trim + CFG_WORD.Substring(34, 30);
                if (interface_flag == 1)
                {
                    //////////////////恢复控制字////////////////CNN 20140304///////////
                    Init_TDA8007_Click(null, EventArgs.Empty);
                    Cold_Reset_Click(null, EventArgs.Empty);
                    FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                    FM12XX_Card.TransceiveCT("0000000020", "02", out strReceived);
                    FM12XX_Card.TransceiveCT(CFG_WORD, "02", out strReceived);
                    if (strReceived != "9000")
                    {
                        DisplayMessageLine("恢复控制字   失败！！");
                    }
                    //发送04读指令
                    sendbuf = "0004000020";
                    i = FM12XX_Card.TransceiveCT(sendbuf, "02", out strReceived);
                    if (strReceived.Substring(2, 64) != CFG_WORD)
                    {
                        DisplayMessageLine("恢复控制字   失败！！");
                        DisplayMessageLine("原始控制字：" + CFG_WORD);
                        DisplayMessageLine("目前控制字：" + strReceived.Substring(2, 64));
                        return;
                    }
                }
                else
                {
                    Reset17_Click(null, EventArgs.Empty);
                    FM12XX_Card.Init_TDA8007(out strReceived);
                    FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                    FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                    FM12XX_Card.REQA(out display);
                    if (display == "Error")
                    {
                        DisplayMessageLine("REQA Error!");
                        return;
                    }
                    FM12XX_Card.AntiColl_lv(1, out display);
                    if (display == "Error")
                    {
                        DisplayMessageLine("AntiColl Error!");
                        return;
                    }
                    FM12XX_Card.Select(1, out display);
                    if (display == "Error")
                    {
                        DisplayMessageLine("Select Error!");
                        return;
                    }
                    i = FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                    i = FM12XX_Card.WriteBlock("00", CFG_WORD.Substring(0, 32), out strReceived);
                    i = FM12XX_Card.WriteBlock("01", CFG_WORD.Substring(32, 32), out strReceived);
                    if (i == 1)
                    {
                        display = "恢复控制字   失败！！";
                        DisplayMessageLine(display);
                        return;
                    }
                    //读控制字校验
                    i = FM12XX_Card.ReadBlock("00", out strReceived);
                    display = strReceived.Substring(0, 32);
                    i = FM12XX_Card.ReadBlock("01", out strReceived);
                    display = display + strReceived.Substring(0, 32);
                    if (display != CFG_WORD)
                    {
                        DisplayMessageLine("恢复控制字   失败！！");
                        DisplayMessageLine("原始控制字：" + CFG_WORD);
                        DisplayMessageLine("目前控制字：" + display);
                        return;
                    }
  
                }

                if (FreqTrim_radioButton.Checked == true)//校准
                {
                    ////////////////////////计算频率偏差///////////////////////
                    dfs_dev = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
                    if(interface_flag==1)
                        FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (47 * 256 + 0xff);
                    else
                        FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);
                    dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
                    FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
                    //dfs_dev, FourM_dev, dfs_value, FourM_value;
                    switch (byte2)
                    {
                        case "10":
                            dfs_value = dfs_t + dfs_dev;
                            display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F5") + "MHz 。";
                            break;
                        case "01":
                            dfs_value = dfs_t - dfs_dev;
                            display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                            break;
                        case "FF":
                            display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前档位实际最低频率为" + dfs_value.ToString("F4") + "MHz 。";
                            break;
                        default:
                            display = "DFS环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                            break;
                    }
                    DisplayMessageLine(display);
                    switch (byte5)
                    {
                        case "10":
                            FourM_value = 4.04 + FourM_dev;
                            display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                            break;
                        case "01":
                            FourM_value = 4.04 - FourM_dev;
                            display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                            break;
                        case "FF":
                            display = "4M环振频率过高，标准频率为4.04MHz， " + "当前实际最低频率为" + FourM_value.ToString("F4") + "MHz 。";
                            break;
                        default:
                            display = "4M环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                            break;
                    }
                }
                else
                {
                    ////////////////////////计算频率偏差///////////////////////
                    dfs_value = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
                    if (interface_flag == 1)
                        FourM_value = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (47 * 256 + 0xff);
                    else
                        FourM_value = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);

                    //dfs_dev, FourM_dev, dfs_value, FourM_value;
                    switch (byte2)
                    {
                        case "10":
                            dfs_dev = dfs_value - dfs_t;
                            display = "DFS环振当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F4") + "MHz 。";
                            break;
                        case "01":
                            dfs_dev = dfs_t - dfs_value;
                            display = "DFS环振当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                            break;
                        case "FF":
                            dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
                            dfs_dev = dfs_value - dfs_t;
                            display = "DFS环振当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F4") + "MHz 。";
                            //                     display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前频率为" + dfs_value.ToString("F4") + "MHz 。";
                            break;
                        default:
                            display = "DFS环振频率读取回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                            break;
                    }
                    DisplayMessageLine(display);
                    switch (byte5)
                    {
                        case "10":
                            FourM_dev = FourM_value - 4.04;
                            display = "4M环振当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                            break;
                        case "01":
                            FourM_dev = 4.04 - FourM_value;
                            display = "4M环振当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                            break;
                        case "FF":
                            FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
                            FourM_dev = FourM_value - 4.04;
                            display = "4M环振当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                            //                    display = "4M环振频率过高，标准频率为4.04MHz， " + "当前频率为" + FourM_value.ToString("F4") + "MHz 。";
                            break;
                        default:
                            display = "4M环振频率读取回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                            break;
                    }
                }
                DisplayMessageLine(display);

                Init_TDA8007_Click(null, null);
                Reset17_Click(null, EventArgs.Empty);
                //if (interface_flag == 1)
                //{
                //    CL_Interface.Checked = true;
                //    CT_Interface.Checked = false;
                //    interface_flag = 0;//非接触
                //}

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }


        private void Auth_button_Click(object sender, EventArgs e)
        {
            if (IsFMReaderV2() == false)
            {
                string StrReceived, AuthBlockAddr;
                byte AuthType, KeyMode;
                int result;
                if (auth_uidlevel_textBox.Text == "2")
                    FM12XX_Card.auth_falg = 2;
                else
                    FM12XX_Card.auth_falg = 0;
                display = "";
                AuthType = (byte)(AuthType_comboBox.SelectedIndex == 1 ? 0x01 : 0x00);
                KeyMode = (byte)(KeyMode_comboBox.SelectedIndex == 1 ? 0x61 : 0x60);
                AuthBlockAddr = AuthBlock_textBox.Text;
                result = FM12XX_Card.AUTH(AuthType, KeyMode, AuthBlockAddr, out StrReceived);
                if (result != 0)
                    display = result.ToString();
                display = "认证:  \t<-\t" + StrReceived + display;
                display = AuthType_comboBox.SelectedIndex == 1 ? "SH" : "Mifare" + display;
                DisplayMessageLine(display);
            }
            else 
            {
                string keymode,AuthBlockAddr,StrReceived,key;
                string command,display;
                if (KeyMode_comboBox.SelectedIndex == 1)
                    keymode ="01";
                else
                    keymode = "00";
                AuthBlockAddr = AuthBlock_textBox.Text;
                key =AuthKey_textBox.Text;


      
                command = SmartCardCmd.MI_Authent + keymode + AuthBlockAddr + key;

                FM12XX_Card.SendCommand(DeleteSpaceString(command), out StrReceived);
                if (StrReceived == "9000")
                    display = "Authent: Success!";
                else
                    display = "Authen: Failed!";

                DisplayMessageLine(display);
                   
            }
        }

        private void Ldkey_button_Click(object sender, EventArgs e)
        {

            if (IsFMReaderV2() == false) //Old FMReader
            {
                string AuthKeys, StrReceived;
                int result;
                display = "";
                AuthKeys = AuthKey_textBox.Text;
                result = FM12XX_Card.LOADKEY(AuthKeys, out StrReceived);
                if (result != 0)
                    display = result.ToString();
                display = "加载密钥:  \t<-\t" + StrReceived + display;
                DisplayMessageLine(display);
            }
            else 
            {
                //Do Nothing
                DisplayMessageLine("加载密钥: Success");
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            string First16Byte, Second16Byte, cwd0, cwd13, cwd7, cwd8, cwd16, tmpStr, uid_Temp, Str, strReceived;
            int temp_cwd3, temp_cwd16;
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            int i;
            int addr = 0;
            int hex_addr_change = 0;
            byte[] bytedata;
            int startaddr;

            tmpStr = "";

            if (radioButton7.Checked)
            {
                CT_Interface.Checked = true;
            }
            else
            {
                CL_Interface.Checked = true;
            }


            if (CL_Interface.Checked)
            {
                FM12XX_Card.Init_TDA8007(out strReceived);
                Reset17_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = receive;
                //cwd0 = receive.Substring(0, 2);
                //cwd13 = receive.Substring(26, 2);
                SendData_textBox.Text = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                Second16Byte = receive;
                // cwd16 = receive.Substring(0, 2);        //yanshl,2013-10-30
                SendData_textBox.Text += receive;
                if (radioButton3.Checked)
                {
                    First16Byte = "4C" + First16Byte.Substring(2, 18) + "B9" + First16Byte.Substring(22, 4) + "07" + First16Byte.Substring(28, 4);//改光检测07

                }
                else
                {
                    // First16Byte = "0C" + First16Byte.Substring(2, 18) + "81" + First16Byte.Substring(22, 4) + "20" + First16Byte.Substring(28, 4);//改光检测00
                    First16Byte = "0C" + First16Byte.Substring(2, 18) + "B9" + First16Byte.Substring(22, 4) + "20" + First16Byte.Substring(28, 4);//改光检测07
                    // First16Byte = "0C" + First16Byte.Substring(2, 24) + "20" + First16Byte.Substring(28, 4);//改光检测保留默认
                }

                FM12XX_Card.WriteBlock("00", First16Byte, out receive);
                display = "WriteBlock 00:   \t\t" + receive;
                DisplayMessageLine(display);

                FM12XX_Card.WriteBlock("01", Second16Byte, out receive);
                display = "WriteBlock 01:   \t\t" + receive;
                DisplayMessageLine(display);


                Reset17_Click(null, EventArgs.Empty);
                Init_TDA8007_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCL("32A0", "01", "09", out receive);
                FM12XX_Card.TransceiveCL("3180", "01", "09", out receive);
                FM12XX_Card.ReadBlock("00", out receive);
                First16Byte = receive;
                FM12XX_Card.ReadBlock("01", out receive);
                Second16Byte = receive;
                SendData_textBox.Text = First16Byte + Second16Byte;
                display = "配置字检测:   \t\t" + First16Byte + Second16Byte;
                DisplayMessageLine(display);
            }
            else
            {
                Reset17_Click(null, EventArgs.Empty);
                FM12XX_Card.Init_TDA8007(out strReceived);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                {
                    display = "Cold Reset失败 ";
                    DisplayMessageLine(display);
                    //return 1; //CNN 20130325 如果卡没有ATR回发  继续执行
                }
                Delay(1);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                if (strReceived == "9000")
                {
                }
                else
                {
                    DisplayMessageLine("初始化接口错误！！");
                    return;
                }
                FM12XX_Card.TransceiveCT("0004000010", "02", out First16Byte);
                FM12XX_Card.TransceiveCT("0004000410", "02", out Second16Byte);
                SendData_textBox.Text = First16Byte.Substring(2, 32) + Second16Byte.Substring(2, 32);
                if (radioButton3.Checked)
                {
                    First16Byte = "4C" + First16Byte.Substring(4, 18) + "B9" + First16Byte.Substring(24, 4) + "07" + First16Byte.Substring(30, 4);//改光检测07

                }
                else
                {
                    First16Byte = "0C" + First16Byte.Substring(4, 18) + "B9" + First16Byte.Substring(24, 4) + "20" + First16Byte.Substring(30, 4);//改光检测07
                }
                FM12XX_Card.TransceiveCT("0000000010", "02", out strReceived);
                FM12XX_Card.TransceiveCT(First16Byte, "02", out strReceived);
                if (strReceived == "9000")
                {
                    receive = "成功";
                }
                else
                {
                    receive = "失败";
                }
                display = "WriteBlock 00:   \t\t" + receive;
                DisplayMessageLine(display);


                Reset17_Click(null, EventArgs.Empty);
                FM12XX_Card.Init_TDA8007(out strReceived);
                Cold_Reset_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0004000010", "02", out First16Byte);
                FM12XX_Card.TransceiveCT("0004000410", "02", out Second16Byte);
                SendData_textBox.Text = First16Byte + Second16Byte;
                display = "配置字检测:   \t\t" + First16Byte + Second16Byte;
                DisplayMessageLine(display);

            }
            ProgEEStartAddr.Text = "000000";
            ProgEEEndAddr.Text = "0000ff";
            ProgFileLenth = 0;
            if (radioButton3.Checked)
            {
                openFileDialog_Prog.FileName = Application.StartupPath + "\\prog\\FM309CosBist_EE_TEST_20130507.hex";

            }
            else if (radioButton4.Checked)
            {
                openFileDialog_Prog.FileName = Application.StartupPath + "\\prog\\FM309CosBist_ENCRYPT_TEST_090004.hex";

            }
            else if (radioButton5.Checked)
            {
                openFileDialog_Prog.FileName = Application.StartupPath + "\\prog\\FM309_ETC_PSAM.hex";
            }
            HexDirectory = openFileDialog_Prog.FileName;
            RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
            hexfiledata = new byte[RawCode.Length];
            bin_data = new byte[RawCode.Length];

            StartEEAddr = ProgEEStartAddr.Text;
            EndEEAddr = ProgEEEndAddr.Text;
            for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            ProgEEStartAddr.Text = StartEEAddr;
            ProgEEEndAddr.Text = EndEEAddr;

            for (i = 0; i < ProgFileMaxLen; i++)
            {
                Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
            }
            ProgOrReadEE_FF_flag = 0;
            ProgOrReadEE_80_flag = 0;
            ProgOrReadEE_81_flag = 0;
            ProgOrReadEE_82_flag = 0;
            ProgOrReadEE_83_flag = 0;
            ProgOrReadEE_84_flag = 0;
            ProgOrReadEE_90_flag = 0;

            for (i = 0; i < RawCode.Length; i++)
            {
                szHex = "";
                tempStr = RawCode[i];
                if (tempStr == "")
                {
                    continue;
                }
                if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                {

                    if (tempStr.Substring(1, 8) == "00000001")//数据结束
                    {
                        break;
                    }
                    if (tempStr.Substring(7, 2) == "04")//段地址
                    {
                        if (tempStr.Substring(11, 2) == "80")//段地址
                        {
                            hex_addr_change = 0x010000;
                            ProgOrReadEE_80_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "81")//段地址
                        {
                            hex_addr_change = 0x020000;
                            ProgOrReadEE_81_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "82")//段地址
                        {
                            hex_addr_change = 0x030000;
                            ProgOrReadEE_82_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "FF")//段地址
                        {
                            hex_addr_change = 0x000000;
                            ProgOrReadEE_FF_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                        {
                            hex_addr_change = 0x060000;
                            ProgOrReadEE_90_flag = 1;
                        }
                        else//段后扩展地址
                        {
                            ProgOrReadEE_EXT_flag = 1;
                            if (tempStr.Substring(11, 2) == "83")//段地址
                            {
                                hex_addr_change = 0x040000;
                                ProgOrReadEE_83_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "84")//段地址
                            {
                                hex_addr_change = 0x050000;
                                ProgOrReadEE_84_flag = 1;
                            }
                        }
                    }
                    else
                    {
                        ProgOrReadEE_FF_flag = 1;
                        datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                        addrStr = tempStr.Substring(3, 4);//记录该行的地址
                        szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                        bytedata = strToHexByte(szHex);
                        addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                        if (addr >= ProgFileLenth)
                        {
                            ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                        }
                        for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                        {
                            Progfilebuf[addr + j] = bytedata[j];
                        }
                    }
                    // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                }
            }
            ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            openFileDialog_Prog.Dispose();
            //编程 
            if (radioButton3.Checked)
            {
                CT_Interface.Checked = true;
            }
            else
            {
                CL_Interface.Checked = true;
            }
            ProgEE_Button_Click(null, EventArgs.Empty);
            if (radioButton3.Checked)
            {
                FM12XX_Card.TransceiveCT("0008050000", "02", out strReceived);
                if (strReceived != "90003B630000903344")
                {
                    DisplayMessageLine("程序出错！！");
                    return;
                }
                // EE 全空间写0xFF   
                FM12XX_Card.TransceiveCT("00 10 05 00 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 00 02", "02", out strReceived);
                if (strReceived != "107D669000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }

                //EE 偶55,奇AA                                
                FM12XX_Card.TransceiveCT("00 10 01 00 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 00 02", "02", out strReceived);
                if (strReceived != "1091C19000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                //EE 偶AA,奇55                               
                FM12XX_Card.TransceiveCT("00 10 02 00 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 00 02", "02", out strReceived);
                if (strReceived != "10ED4D9000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                //EE 地址译码前40K 擦、写时间分别1*128us       
                FM12XX_Card.TransceiveCT("00 10 03 01 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 01 02", "02", out strReceived);
                if (strReceived != "1064AB9000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                // EE 地址译码后40K 擦、写时间分别1*128us       
                FM12XX_Card.TransceiveCT("00 10 03 81 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 02 02", "02", out strReceived);
                if (strReceived != "107D8E9000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                FM12XX_Card.TransceiveCT("00 10 07 00 02", "02", out strReceived);
                if (strReceived != "10A9129000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                // EE 页内译码                                
                FM12XX_Card.TransceiveCT("00 10 04 00 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 03 02", "02", out strReceived);
                if (strReceived != "1000009000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                // EE 全空间写0x00   
                FM12XX_Card.TransceiveCT("00 10 00 00 02", "02", out strReceived);
                FM12XX_Card.TransceiveCT("00 10 07 00 02", "02", out strReceived);
                if (strReceived != "1001EA9000")
                {
                    DisplayMessageLine("EE出错！！");
                    return;
                }
                DisplayMessageLine("Succeeded~~~~~~~~~~~~~~~~~~~~~~~~");

            }
            else if (radioButton4.Checked)
            {
                Reset17_Click(null, EventArgs.Empty);
                FM12XX_Card.Init_TDA8007(out strReceived);
                Cold_Reset_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCT("0020000005", "02", out strReceived);
                if (strReceived != "2001E9314CB59000")
                {
                    DisplayMessageLine("算法出错！！");
                    return;
                }
                FM12XX_Card.TransceiveCT("0021000005", "02", out strReceived);
                if (strReceived != "2101561CCDB89000")
                {
                    DisplayMessageLine("算法出错！！");
                    return;
                }
                FM12XX_Card.TransceiveCT("0022000005", "02", out strReceived);
                if (strReceived != "221F6EF78E7A9000")
                {
                    DisplayMessageLine("算法出错！！");
                    return;
                }
                FM12XX_Card.Init_TDA8007(out strReceived);
                Reset17_Click(null, EventArgs.Empty);
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                button_RATS_Click(null, EventArgs.Empty);
                FM12XX_Card.SendAPDUCL("0020000005", out strReceived);
                if (strReceived != "01E9314CB59000")
                {
                    DisplayMessageLine("算法出错！！");
                    return;
                }
                FM12XX_Card.SendAPDUCL("0021000005", out strReceived);
                if (strReceived != "01561CCDB89000")
                {
                    DisplayMessageLine("算法出错！！");
                    return;
                }
                FM12XX_Card.SendAPDUCL("0022000005", out strReceived);
                if (strReceived != "1F6EF78E7A9000")
                {
                    DisplayMessageLine("算法出错！！");
                    return;
                }
                DisplayMessageLine("Succeeded~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }
            else
            {
                DisplayMessageLine("请用MP300进行电流测试~~~");

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Reset17_Click(null, EventArgs.Empty);
            Init_TDA8007_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            button_RATS_Click(null, EventArgs.Empty);
            Reset17_Click(null, EventArgs.Empty);
            Init_TDA8007_Click(null, EventArgs.Empty);
            Cold_Reset_Click(null, EventArgs.Empty);
        }

        private void BufMove_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BufMove_checkBox.Checked)
            {
                ProgDestAddr.Visible = true;
                dest_label1.Visible = true;
                Prog_Move.Visible = true;
            }
            else
            {
                ProgDestAddr.Visible = false;
                dest_label1.Visible = false;
                Prog_Move.Visible = false;
            }
        }


        private void FreqRead_Click(object sender, EventArgs e)
        {
            string APDU_data = "", freq_trim, trim_info, dfs_stv, dfs_cfg; //备份需要修改的控制字
            string byte0, byte1, byte2, byte3, byte4, byte5, byte6, byte7;
            int frq_ref;
            double dfs_dev, FourM_dev, dfs_value, FourM_value, dfs_t;
            try
            {
                APDU_data = "00FD000100";   //根据COS算法  frq_ref应配置成0x2f=47
                frq_ref = 0x2f;
                i = FM12XX_Card.SendAPDUCT(APDU_data, out receive);
                trim_info = receive;
                display = "Data Received:  \t<-\t" + receive;
                DisplayMessageLine(display);
                //写入校准值
                byte0 = trim_info.Substring(0, 2);
                if (byte0 != "5A")
                {
                    display = "回复信息错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    DisplayMessageLine(display);
                    Init_TDA8007_Click(null, null);
                    return;
                }
                byte1 = trim_info.Substring(2, 2);
                byte2 = trim_info.Substring(4, 2);
                byte3 = trim_info.Substring(6, 2);
                byte4 = trim_info.Substring(8, 2);
                byte5 = trim_info.Substring(10, 2);
                byte6 = trim_info.Substring(12, 2);
                byte7 = trim_info.Substring(14, 2);
                freq_trim = byte1;
                ////////////////////////计算频率偏差///////////////////////

                dfs_value = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * 3.58) / (frq_ref * 256 + 0xff);
                FourM_value = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * 3.58) / (47 * 256 + 0xff);

                //dfs_dev, FourM_dev, dfs_value, FourM_value;
                switch (byte2)
                {
                    case "10":
                    case "01":
                        display = "DFS环振当前频率为" + dfs_value.ToString("F4") + "MHz";
                        break;
                    case "FF":
                        dfs_value = (3.58 * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
                        display = "DFS环振当前频率为" + dfs_value.ToString("F4") + "MHz";
                        break;
                    default:
                        display = "DFS环振频率读取回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                        break;
                }
                DisplayMessageLine(display);
                switch (byte5)
                {
                    case "10":
                    case "01":
                        FourM_dev = FourM_value - 4.04;
                        display = "4M环振当前频率为" + FourM_value.ToString("F4") + "MHz";
                        break;
                    case "FF":
                        FourM_value = (3.58 * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
                        display = "4M环振当前频率为" + FourM_value.ToString("F4") + "MHz";
                        break;
                    default:
                        display = "4M环振频率读取回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                        break;
                }
                DisplayMessageLine(display);

                //                Init_TDA8007_Click(null, null);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        private void Prog_Move_Click(object sender, EventArgs e)
        {
            string StartAddr;
            uint nTemp1 = 0;
            byte[] startaddr;
            string EndAddr;
            uint nTemp2 = 0;
            byte[] endaddr;
            string DestAddr;
            uint nTemp3 = 0;
            byte[] destaddr;
            //            byte[] Progfilebuftmp = new byte[1024 * 448];      //最大文件256k+127K+LIB
            byte[] Progfilebuftmp = new byte[1024 * 610];      //最大文件256k+127K+LIB
            uint MoveLength;

            int i, j, len;
            int nProgAddr1, nProgAddr, Addr_temp;
            int nProgEndTemp;
            int nRowData;
            String strProgData;

            StartAddr = ProgEEStartAddr.Text;
            for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
            {
                StartAddr = "0" + StartAddr;
            }
            ProgEEStartAddr.Text = StartAddr;
            startaddr = new byte[3];
            startaddr = (strToHexByte(ProgEEStartAddr.Text));
            nTemp1 = (uint)(startaddr[2] + startaddr[1] * 256 + startaddr[0] * 65536);

            EndAddr = ProgEEEndAddr.Text;
            for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
            {
                EndAddr = "0" + EndAddr;
            }
            ProgEEEndAddr.Text = EndAddr;
            endaddr = new byte[3];
            endaddr = (strToHexByte(ProgEEEndAddr.Text));
            nTemp2 = (uint)(endaddr[2] + endaddr[1] * 256 + endaddr[0] * 65536);

            DestAddr = ProgDestAddr.Text;
            for (i = 0; i < 6 - ProgDestAddr.Text.Length; i++)
            {
                DestAddr = "0" + DestAddr;
            }
            ProgDestAddr.Text = DestAddr;
            destaddr = new byte[3];
            destaddr = (strToHexByte(ProgDestAddr.Text));
            nTemp3 = (uint)(destaddr[2] + destaddr[1] * 256 + destaddr[0] * 65536);

            MoveLength = nTemp2 - nTemp1 + 1;

            for (i = 0; i < MoveLength; i++)
            {
                Progfilebuftmp[nTemp3 + i] = Progfilebuf[nTemp1 + i];
            }
            for (i = 0; i < MoveLength; i++)
            {
                Progfilebuf[nTemp3 + i] = Progfilebuftmp[nTemp3 + i];
            }
            if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)
            {
                dataGridView_flag = 0;
                progEEdataGridView.Rows.Clear();
            }

            nProgAddr1 = strToHexByte(ProgDestAddr.Text)[2] + strToHexByte(ProgDestAddr.Text)[1] * 256 + strToHexByte(ProgDestAddr.Text)[0] * 65536;

            Addr_temp = nProgAddr1;

            if (MoveLength % 16 == 0)
            {
                nRowData = (int)MoveLength / 16;
            }
            else
            {
                nRowData = (int)MoveLength / 16 + 1;
            }
            nRowcount = nRowData;
            for (i = 0; i < nRowData; i++)
            {
                progEEdataGridView.RowCount = nRowData;
                nProgAddr = nProgAddr1 + 0x10 * i;

                progEEdataGridView["EE_Addr", i].Value = nProgAddr.ToString("X6");
                strProgData = "";
                len = 0x10;
                for (j = 0; j < len; j++)
                {
                    strProgData = strProgData + Progfilebuf[Addr_temp + i * len + j].ToString("X2") + " ";
                }
                progEEdataGridView["EE_Data", i].Value = strProgData;
            }

        }

        private void FM349_ReverseOption()
        {
            Int32 rowcounter;
            j = cfg_dataGridView.RowCount;
            try
            {
                for (rowcounter = 0; rowcounter < j-1; rowcounter++)
                {
                    if (cfg_dataGridView[4, rowcounter].EditedFormattedValue.ToString() == "--")
                        cfg_dataGridView[4, rowcounter].Value = "锁定";
                    //if (cfg_dataGridView.Rows[rowcounter].Cells[4].Value.ToString() == "--")
                    //    cfg_dataGridView[4, rowcounter].Value = "锁定";
                    else
                        cfg_dataGridView[4, rowcounter].Value = "--";
                }
                cfg_dataGridView.Refresh();
                cfg_dataGridView.EndEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Load_NEW_cfg_dataGridView()
        {
            string cc = "空";
            Int32 rowcounter = 0;
            int flag = 0;
            int flag_dd = 0;
            string strData = "00";
            string strItem = "00";
            string strOption = "";
            string ggggg = "00";
            XmlDocument dom = new XmlDocument();
            switch (FMXX_SEL_CFGcomboBox.SelectedIndex)
            {
                case 0: CfgFileName = "\\cfg_336.xml";
                    break;
                case 1: CfgFileName = "\\cfg_326.xml";
                    break;                
                case 2: CfgFileName = "\\cfg_349.xml";
                    break;
                case 3: CfgFileName = "\\cfg_347.xml";
                    break;
                case 4: CfgFileName = "\\cfg_294.xml"; 
                    break;
                case 5: CfgFileName = "\\cfg_274.xml";
                    break;
                case 6: CfgFileName = "\\cfg_302.xml";
                    break;
                case 7: CfgFileName = "\\cfg_309.xml";
                    break;
                case 8: CfgFileName = "\\cfg_349New.xml";
                    break;
                case 9: CfgFileName = "\\cfg_349Security.xml";
                    break;
                case 10: CfgFileName = "\\cfg_336_new.xml";
                    break;
                default: CfgFileName = "\\cfg_336.xml";
                    break;
            }
            if (!(System.IO.File.Exists(Application.StartupPath + CfgFileName)))                  //检查所要操作的xml文件是否存在
            {
                DisplayMessageLine("运行目录下的配置文件" + CfgFileName + "文件不存在!请置入");
                return;
            }
            DataTable dtRelation = new DataTable();
            cfg_dataGridView.DataSource = dtRelation;
            dtRelation.Columns.Add("bit", typeof(string));
            dtRelation.Columns.Add("条目", typeof(string));
            dtRelation.Columns.Add("配置选项", typeof(string));
            dtRelation.Columns.Add("数据", typeof(string));
            if(CB_EnableOption.Checked == true)
                dtRelation.Columns.Add("选项", typeof(string));

            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + CfgFileName);    //加载Xml文件  
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList configNodes = rootElem.GetElementsByTagName("config"); //获取config子节点集合 
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    flag = 0;
                    DataGridViewComboBoxCell cb = new DataGridViewComboBoxCell();
                    DataGridViewComboBoxCell cOption = new DataGridViewComboBoxCell();
                    // DataGridViewCell cc = new DataGridViewCell();
                    //DataGridViewComboBoxCell cc = new DataGridViewComboBoxCell();
                    string strCwd = ((XmlElement)node).GetAttribute("CWD");   //获取CWD属性值

                    // 349处理，添加NVR判断，需要CWD属性统一命名0_CW开头表示NVR0区的配置字
                    if ((CB_NVR0_SEL.Checked == false) && (strCwd.Contains("0_CW"))) continue;
                    if ((CB_NVR1_SEL.Checked == false) && (strCwd.Contains("1_CW"))) continue;
                    if ((CB_NVR2_SEL.Checked == false) && (strCwd.Contains("2_CW"))) continue;
                    if ((CB_NVR3_SEL.Checked == false) && (strCwd.Contains("3_CW"))) continue;
                    if ((CB_NVR4_SEL.Checked == false) && (strCwd.Contains("4_CW"))) continue;
                    if ((CB_NVR5_SEL.Checked == false) && (strCwd.Contains("5_CW"))) continue;
                    if ((CB_NVR6_SEL.Checked == false) && (strCwd.Contains("6_CW"))) continue;

                    string strContex = ((XmlElement)node).GetAttribute("contex");   //获取CWD属性值  
                    if (strContex.Substring(1, 2) == "手写")
                    {
                        flag_dd = 1;
                    }
                    string strBit = ((XmlElement)node).GetAttribute("bit");   //获取bit位数                                 
                    XmlNodeList subItemNodes = ((XmlElement)node).GetElementsByTagName("item");  //获取item子XmlElement集合 
                    XmlNodeList subItemOptions = ((XmlElement)node).GetElementsByTagName("option");//获取option子XmlElement集合
                    j = subItemNodes.Count;
                    foreach (XmlElement element in subItemNodes)
                    {
                        strItem = ((XmlElement)element).GetAttribute("cfgitem");
                        if (flag_dd == 1)
                        {
                            ggggg = strItem;
                        }
                        cb.Items.Add(strItem);
                        if (flag == 0)
                        {
                            XmlNodeList dataNodes = element.ChildNodes;
                            j = dataNodes.Count;
                            if (j > 0)
                            {
                                strData = dataNodes[j - 1].InnerText;
                            }
                            else
                            {
                                strData = "无效数据";
                            }
                            cc = strData;
                            flag = 1;
                        }
                    }
                    if (CB_EnableOption.Checked == true)
                    {
                        foreach (XmlElement element in subItemOptions)
                        {
                            strOption = ((XmlElement)element).GetAttribute("optionname");
                            cOption.Items.Add(strOption);
                        }
                    }

                    dtRelation.Rows.Add(new object[] { strBit, strContex });
                    cfg_dataGridView.Rows[rowcounter].HeaderCell.Value = strCwd;//set row header
                    if (flag_dd == 1)
                    {
                        cfg_dataGridView.Rows[rowcounter].Cells[2].Value = ggggg;
                        flag_dd = 0;
                    }
                    else
                    {
                        cfg_dataGridView.Rows[rowcounter].Cells[2] = cb;
                        cfg_dataGridView.Rows[rowcounter].Cells[2].Value = cb.Items.ToString();
                    }
                    cfg_dataGridView.Rows[rowcounter].Cells[3].Value = cc;
                    if (CB_EnableOption.Checked == true)
                    {
                        cfg_dataGridView.Rows[rowcounter].Cells[4] = cOption;
                        cfg_dataGridView.Rows[rowcounter].Cells[4].Value = cOption.Items.ToString();
                    }
                    rowcounter++;

                }
                cfg_dataGridView.RowHeadersWidth = 70;
                cfg_dataGridView.Columns[0].Width = 47;
                cfg_dataGridView.Columns[1].Width = 245;
                cfg_dataGridView.Columns[2].Width = 244;
                cfg_dataGridView.Columns[3].Width = 65;
                if (CB_EnableOption.Checked == true)
                {
                    cfg_dataGridView.Columns[1].Width = 222;
                    cfg_dataGridView.Columns[2].Width = 215;
                    cfg_dataGridView.Columns[4].Width = 52;
                }
                cfg_dataGridView.Columns[0].ReadOnly = true;
                cfg_dataGridView.Columns[1].ReadOnly = true;
                cfg_dataGridView.Refresh();
                cfg_dataGridView.EndEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReadCWD_button_Click(object sender, EventArgs e)
        {
            string block0;
            string block1;
            string strReceived;
            if (cfg_dataGridView.RowCount == 0)
            {
                Load_NEW_cfg_dataGridView();
            }
            try
            {
                if (CT_newcfg_radioButton.Checked) //接触模式下读控制字
                {
                    ConnectReader_Click(null, EventArgs.Empty);
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                    Cold_Reset_Click(null, EventArgs.Empty);
                    FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    if (strReceived == "9000" || strReceived == "019000")
                    {
                        strReceived = "ok";
                    }
                    else
                    {
                        strReceived = "error";
                        DisplayMessageLine("初始化指令出错!!!!!!!");
                        return;
                    }
                    if (CB_WordRegEn.Checked == false)
                    {
                        if (FMXX_SEL_CFGcomboBox.Text == "FM274")
                        {
                            FM12XX_Card.Set_TDA8007_reg(0x02, 0x01, out strReceived); //修改通讯波特率115200
                            FM12XX_Card.TransceiveCT("00022D0200", "02", out strReceived);
                        }
                        else if (FMXX_SEL_CFGcomboBox.Text == "FM294")
                        {
                            FM12XX_Card.Set_TDA8007_reg(0x02, 0x01, out strReceived); //修改通讯波特率115200
                            FM12XX_Card.TransceiveCT("0002020000", "02", out strReceived);
                        }
                        else
                            FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                        if (strReceived != "9000" && strReceived != "029000")
                        {
                            DisplayMessageLine("初始化指令出错!!!!!!!");
                            return;
                        }
                        if (FMXX_SEL_CFGcomboBox.Text == "FM274" || FMXX_SEL_CFGcomboBox.Text == "FM294")
                        {
                            FM12XX_Card.TransceiveCT("0004000010", "02", out block1);
                            block0 = block1.Substring(2, 32);
                        }
                        else
                        {
                            FM12XX_Card.TransceiveCT("0004000020", "02", out block0);
                            block1 = block0.Substring(2, 64);
                            block0 = "";
                        }
                        //FM12XX_Card.TransceiveCT("0004000410", "02", out block1);

                    }
                    else if (FMXX_SEL_CFGcomboBox.Text == "FM347")
                    {
                        FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            DisplayMessageLine("地址高位配置出错!!!!!!!");
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                        FM12XX_Card.TransceiveCT("0004010C08", "02", out block1);
                        block1 = block0.Substring(2, 72) + block1.Substring(2, 16);
                        block0 = "";
                    }
                    else // for fm349
                    {
                        // 配置0x04/0x0A逻辑地址高8位
                        FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            DisplayMessageLine("地址高位配置出错!!!!!!!");
                            return;
                        }
                        block0 = "";
                        block1 = "";
                        for (int i = 0; i < 7; i++)
                        {
                            string cmd_data = "0004" + (0x80 * i).ToString("X4") + "80";
                            if (((CheckBox)this.Controls.Find("CB_NVR" + i + "_SEL", true)[0]).Checked == true)
                            {
                                FM12XX_Card.TransceiveCT(cmd_data, "02", out block0);
                                block0 = block0.Substring(2, 256);
                                block1 = block1 + block0;
                            }
                        }
                        block0 = "";
                    }
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                }
                else //非接读取控制字
                {
                    ConnectReader_Click(null, EventArgs.Empty);
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                    Field_ON_Click(null, EventArgs.Empty);
                    Active_Click(null, EventArgs.Empty);

                    if (CB_WordRegEn.Checked == false)
                    {
                        if (FMXX_SEL_CFGcomboBox.Text == "FM294")
                            FM12XX_Card.TransceiveCL("31A0", "01", "09", out strReceived);
                        else if (FMXX_SEL_CFGcomboBox.Text == "FM274")
                            FM12XX_Card.TransceiveCL("31B0", "01", "09", out strReceived);
                        else if (FMXX_SEL_CFGcomboBox.Text == "FM302")
                            FM12XX_Card.TransceiveCL("3190", "01", "09", out strReceived);
                        else
                            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                        if (strReceived == "00000000000000000000000000000000")
                        {
                            strReceived = "ok";
                        }
                        else
                        {
                            strReceived = "error";
                            DisplayMessageLine("初始化指令出错!!!!!!!!!");
                            return;
                        }
                        //TransceiveCL("3180", "01", "09", out block0);
                        FM12XX_Card.ReadBlock("00", out block0);
                        FM12XX_Card.ReadBlock("01", out block1);
                    }
                    else if (FMXX_SEL_CFGcomboBox.Text == "FM347")
                    {
                        // 配置逻辑地址的bit23~bit20
                        FM12XX_Card.TransceiveCL("318D", "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            DisplayMessageLine("扩展地址指令0x31失败，安全区可能需要认证");
                            return;
                        }
                        // 配置逻辑地址的bit19~bit12
                        FM12XX_Card.TransceiveCL("32E0", "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            DisplayMessageLine("扩展地址指令0x32失败，安全区可能需要认证");
                            return;
                        }
                        FM12XX_Card.ReadBlock("00", out block0);
                        FM12XX_Card.ReadBlock("01", out block1);
                        block0 += block1;
                        FM12XX_Card.ReadBlock("02", out block1);
                        block1 = block1.Substring(0, 8);
                    }
                    else // for fm349 
                    {
                        block0 = "";
                        block1 = "";
                        // 配置逻辑地址的bit23~bit20
                        FM12XX_Card.TransceiveCL("318D", "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            DisplayMessageLine("扩展地址指令0x31失败，安全区可能需要认证");
                            return;
                        }
                        // 配置逻辑地址的bit19~bit12
                        FM12XX_Card.TransceiveCL("32E0", "01", "09", out strReceived);
                        if (strReceived != "00000000000000000000000000000000")
                        {
                            DisplayMessageLine("扩展地址指令0x32失败，安全区可能需要认证");
                            return;
                        }
                        for (int i = 0; i < 7; i++)
                        {
                            if (((CheckBox)this.Controls.Find("CB_NVR" + i + "_SEL", true)[0]).Checked == true)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    //FM12XX_Card.ReadBlock((i*8+j).ToString("X2"), out block0);
                                    FM12XX_Card.TransceiveCL("30" + (0x08 * i + j).ToString("X2"), "01", "09", out block0);
                                    if (block0 == "Error")
                                    {
                                        DisplayMessageLine("NVR" + i + "_block" + j + "控制字读取出错");
                                        return;
                                    }
                                    else if (block0 == "No Data Received")
                                    {
                                        DisplayMessageLine("NVR" + i + "_block" + j + "控制字读取出错");
                                        return;
                                    }
                                    block1 = block1 + block0;
                                }
                            }
                        }
                        block0 = "";
                    }
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                }
                SendData_textBox.Text = block0 + block1;
                if (FMXX_SEL_CFGcomboBox.Text == "FM274" || FMXX_SEL_CFGcomboBox.Text == "FM294" || FMXX_SEL_CFGcomboBox.Text == "FM302")
                    SendData_textBox.Text = block0;
                DisplayMessageLine("控制字读取成功");
                AnalysisCWD_button_Click(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AnalysisCWD_button_Click(object sender, EventArgs e)
        {
            //解析
            string tt = "0";
            int tmp, j = 0;
            string cc = "";
            string bb = "";
            //Int32 rowcounter=0;
            string strData = "00";
            string strItem = "00";
            if (!(System.IO.File.Exists(Application.StartupPath + CfgFileName)))  //检查所要操作的xml文件是否存在
            {
                DisplayMessageLine("运行目录下的配置文件" + CfgFileName + "文件不存在!请置入");
                return;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + CfgFileName);    //加载Xml文件  
            XmlElement rootElem = doc.DocumentElement;   //获取根节点  
            XmlNodeList configNodes = rootElem.GetElementsByTagName("config"); //获取config子节点集合 
            //string ffff = Convert.ToString(this.dataGridView1.CurrentCell.Value);
            //更新数据         

            try
            {
                if (CB_WordRegEn.Checked == false)
                {
                    for (int k = 0; k < cfg_dataGridView.RowCount - 1; k++)
                    {
                        string strBit = cfg_dataGridView[0, k].Value.ToString();
                        int strbitstart = strToHexByte(strBit.Substring(0, 1))[0];
                        int strbitend = strToHexByte(strBit.Substring(2, 1))[0];
                        int bitnum = strbitend - strbitstart + 1;
                        if (k == 0)
                        {
                            tt = (DeleteSpaceString(SendData_textBox.Text)).Substring(2 * j, 2);
                            bb = DecimalToBinary(strToHexByte(tt)[0]);
                        }
                        else
                        {
                            if ((cfg_dataGridView.Rows[k].HeaderCell.Value.ToString()) != (cfg_dataGridView.Rows[k - 1].HeaderCell.Value.ToString()))
                            {
                                j++;
                                if (FMXX_SEL_CFGcomboBox.Text == "FM274" || FMXX_SEL_CFGcomboBox.Text == "FM294" || FMXX_SEL_CFGcomboBox.Text == "FM302")
                                    tt = (DeleteSpaceString(SendData_textBox.Text)).Substring(4 * j, 2);
                                else
                                    tt = (DeleteSpaceString(SendData_textBox.Text)).Substring(2 * j, 2);
                                bb = DecimalToBinary(strToHexByte(tt)[0]);
                            }
                        }

                        string ffff = (Convert.ToInt32((bb.Substring(7 - strbitend, bitnum)), 2) << strbitstart).ToString("X2");   //获取bit位对应的值 

                        //dataGridView1[2, j].Value = tt;
                        cc = "";
                        if (cfg_dataGridView.Rows[k].Cells[1].Value.ToString().Substring(1, 2) == "手写")
                        {
                            cfg_dataGridView.Rows[k].Cells[2].Value = ffff;
                            cfg_dataGridView.Rows[k].Cells[3].Value = ffff;
                        }
                        else
                        {
                            foreach (XmlNode node in configNodes)
                            {
                                string strCwd = ((XmlElement)node).GetAttribute("CWD");   //获取CWD属性值 
                                XmlNodeList subItemNodes = ((XmlElement)node).GetElementsByTagName("item");  //获取item子XmlElement集合                      
                                if (cfg_dataGridView.Rows[k].HeaderCell.Value.ToString() == strCwd)
                                {
                                    foreach (XmlElement element in subItemNodes)
                                    {
                                        strItem = ((XmlElement)element).GetAttribute("cfgitem");

                                        XmlNodeList dataNodes = element.ChildNodes;
                                        i = dataNodes.Count;
                                        if (i > 0)
                                        {
                                            //strsex = sexNodes[i - 2].InnerText;
                                            strData = dataNodes[i - 1].InnerText;
                                        }
                                        else
                                        {
                                            strData = "无效数据";
                                        }

                                        if (ffff == strData)
                                        {
                                            cc = strItem;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (cfg_dataGridView.Rows[k].Cells[3].Value.ToString() != ffff)
                            {
                                cfg_dataGridView.Rows[k].Cells[2].Value = cc;
                                cfg_dataGridView.Rows[k].Cells[3].Value = ffff;
                                if (CWD_CMP_checkBox.Checked)
                                {
                                    cfg_dataGridView.Rows[k].Cells[3].Style.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                cfg_dataGridView.Rows[k].Cells[3].Style.ForeColor = Color.Black;
                            }
                        }
                        // dataGridView1.Refresh();

                    }
                }
                else // for fm349/fm347
                {

                    string tt_temp = DeleteSpaceString(SendData_textBox.Text);
                    if (((FMXX_SEL_CFGcomboBox.Text == "FM349") || (FMXX_SEL_CFGcomboBox.Text == "FM349_New") || (FMXX_SEL_CFGcomboBox.Text == "FM349_Security")) 
                        && (CalNVRCount() * 256) != tt_temp.Length)
                    {
                        MessageBox.Show("控制字位数不对");
                        return;
                    }
                    for (int k = 0; k < cfg_dataGridView.RowCount - 1; k++)
                    {
                        string strBit = cfg_dataGridView[0, k].Value.ToString();
                        int index = strBit.IndexOf('~');
                        int strbitstart = strToHexByte(strBit.Substring(0, index))[0];
                        int strbitend = strToHexByte(strBit.Substring(index + 1, strBit.Length - 1 - index))[0];
                        int bitnum = strbitend - strbitstart + 1;
                        if (k == 0)
                        {
                            if (FMXX_SEL_CFGcomboBox.Text == "FM347")   //大端模式
                                tt = tt_temp.Substring(8 * j, 2) + tt_temp.Substring(8 * j + 2, 2) + tt_temp.Substring(8 * j + 4, 2) + tt_temp.Substring(8 * j + 6, 2);
                            else                                        //小端模式
                                tt = tt_temp.Substring(8 * j + 6, 2) + tt_temp.Substring(8 * j + 4, 2) + tt_temp.Substring(8 * j + 2, 2) + tt_temp.Substring(8 * j, 2);
                            bb = Convert.ToString(Convert.ToUInt32(tt, 16), 2);
                            int len = bb.Length;
                            if (len < 32)
                            {
                                for (int i = 0; i < 32 - len; i++)
                                {
                                    bb = '0' + bb;
                                }
                            }
                        }
                        else
                        {
                            if ((cfg_dataGridView.Rows[k].HeaderCell.Value.ToString()) != (cfg_dataGridView.Rows[k - 1].HeaderCell.Value.ToString()))
                            {
                                j++;
                                if (FMXX_SEL_CFGcomboBox.Text == "FM347")   //大端模式
                                    tt = tt_temp.Substring(8 * j, 2) + tt_temp.Substring(8 * j + 2, 2) + tt_temp.Substring(8 * j + 4, 2) + tt_temp.Substring(8 * j + 6, 2);
                                else                                        //小端模式
                                    tt = tt_temp.Substring(8 * j + 6, 2) + tt_temp.Substring(8 * j + 4, 2) + tt_temp.Substring(8 * j + 2, 2) + tt_temp.Substring(8 * j, 2);
                                //        tt = (DeleteSpaceString(SendData_textBox.Text)).Substring(8 * j, 8);
                                bb = Convert.ToString(Convert.ToUInt32(tt, 16), 2);
                                int len = bb.Length;
                                if (len < 32)
                                {
                                    for (int i = 0; i < 32 - len; i++)
                                    {
                                        bb = '0' + bb;
                                    }
                                }
                            }
                        }

                        string ffff = (Convert.ToUInt32((bb.Substring(31 - strbitend, bitnum)), 2) << strbitstart).ToString("X8");   //获取bit位对应的值 
                        //if (FMXX_SEL_CFGcomboBox.Text == "FM347")   //大端模式
                        //    ffff = ffff.Substring(0, 2) + ffff.Substring(2, 2) + ffff.Substring(4, 2) + ffff.Substring(6, 2);
                        if (FMXX_SEL_CFGcomboBox.Text != "FM347")   //小端模式
                            ffff = ffff.Substring(6, 2) + ffff.Substring(4, 2) + ffff.Substring(2, 2) + ffff.Substring(0, 2);

                        //dataGridView1[2, j].Value = tt;
                        cc = "";
                        if (cfg_dataGridView.Rows[k].Cells[1].Value.ToString().Substring(1, 2) == "手写")
                        {
                            string manualInput;
                            if (FMXX_SEL_CFGcomboBox.Text == "FM347")
                            {
                                tmp = bitnum / 4;
                                if ((bitnum % 4) != 0)
                                    tmp++;
                                //cfg_dataGridView.Rows[k].Cells[2].Value
                                manualInput = (Convert.ToUInt32(ffff, 16) >> strbitstart).ToString("X8").Substring(8 - tmp, tmp);
                            }
                            else
                                manualInput = ffff;
                            
                            if (CB_EnableOption.Checked == true)
                            {
                                if (cfg_dataGridView.Rows[k].Cells[4].EditedFormattedValue.ToString() != "锁定")
                                {
                                    cfg_dataGridView.Rows[k].Cells[2].Value = manualInput;
                                    cfg_dataGridView.Rows[k].Cells[3].Value = ffff;
                                }
                                else//if locked, then compare and highlight different value
                                {
                                    if(manualInput!= cfg_dataGridView.Rows[k].Cells[2].EditedFormattedValue.ToString())
                                    {
                                        cfg_dataGridView.Rows[k].DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }
                            }
                            else
                            {
                                cfg_dataGridView.Rows[k].Cells[2].Value = manualInput;
                                cfg_dataGridView.Rows[k].Cells[3].Value = ffff;
                            }
                            cfg_dataGridView.Rows[k].Cells[2].Tag = manualInput;
                            cfg_dataGridView.Rows[k].Cells[3].Tag = ffff;
                        }
                        else
                        {
                            foreach (XmlNode node in configNodes)
                            {
                                string strCwd = ((XmlElement)node).GetAttribute("CWD");   //获取CWD属性值 
                                string strContext = ((XmlElement)node).GetAttribute("contex");
                                XmlNodeList subItemNodes = ((XmlElement)node).GetElementsByTagName("item");  //获取item子XmlElement集合                      
                                if ((cfg_dataGridView.Rows[k].HeaderCell.Value.ToString() == strCwd)
                                    && (cfg_dataGridView.Rows[k].Cells[1].Value.ToString() == strContext))
                                {
                                    foreach (XmlElement element in subItemNodes)
                                    {
                                        strItem = ((XmlElement)element).GetAttribute("cfgitem");

                                        XmlNodeList dataNodes = element.ChildNodes;
                                        i = dataNodes.Count;
                                        if (i > 0)
                                        {
                                            //strsex = sexNodes[i - 2].InnerText;
                                            strData = dataNodes[i - 1].InnerText;
                                        }
                                        else
                                        {
                                            strData = "无效数据";
                                        }

                                        if (ffff == strData)
                                        {
                                            cc = strItem;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (CB_EnableOption.Checked == true)
                            {
                                if (cfg_dataGridView.Rows[k].Cells[4].EditedFormattedValue.ToString() != "锁定")
                                {
                                    cfg_dataGridView.Rows[k].Cells[2].Value = cc;
                                    cfg_dataGridView.Rows[k].Cells[3].Value = ffff;
                                }
                                else//if locked, then compare and highlight different value
                                {
                                    if (cc != cfg_dataGridView.Rows[k].Cells[2].EditedFormattedValue.ToString())
                                    {
                                        cfg_dataGridView.Rows[k].DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }
                            }
                            else
                            {
                                cfg_dataGridView.Rows[k].Cells[2].Value = cc;
                                cfg_dataGridView.Rows[k].Cells[3].Value = ffff;
                            }
                            cfg_dataGridView.Rows[k].Cells[2].Tag = cc;
                            cfg_dataGridView.Rows[k].Cells[3].Tag = ffff;
                        }
                        // dataGridView1.Refresh();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // fm349
        private int CalNVRCount()
        { 
            int count = 0;
            for(int i=0; i<7; i++)
            {
                if (((CheckBox)this.Controls.Find("CB_NVR" + i + "_SEL", true)[0]).Checked == true)
                {
                    count++;
                }
            }
            return count;
        }

        private void GenerateCWD_button_Click(object sender, EventArgs e)
        {
            int index, strbitstart, strbitend, bitnum, flag = 0;
            long tt;
            byte[] tmp;
            string strBit, tmp_str = "";
            string tt1 = "";
            string tt2 = "";
            //string tt0 = "0";
            string tt0 = "";
            string strcwd = "";
            string tostring_format = "";
            if (CB_WordRegEn.Checked)
                tostring_format = "X8";
            else
                tostring_format = "X2";
            for (int k = 0; k < cfg_dataGridView.RowCount; k++)
            {
                if (cfg_dataGridView.Rows[k].HeaderCell.Value == null)
                {
                    strcwd += tt0;
                    break;
                }
                if (k == 0)
                {
                    if (FMXX_SEL_CFGcomboBox.Text == "FM347" && (cfg_dataGridView.Rows[k].Cells[1].Value.ToString().Substring(1, 2) == "手写"))   //仅取有效位数
                    {
                        strBit = cfg_dataGridView[0, k].Value.ToString();
                        index = strBit.IndexOf('~');
                        strbitstart = strToHexByte(strBit.Substring(0, index))[0];
                        strbitend = strToHexByte(strBit.Substring(index + 1, strBit.Length - 1 - index))[0];
                        bitnum = strbitend - strbitstart + 1;
                        flag = 1;
                        flag <<= bitnum;
                        tt0 = (Convert.ToUInt32(cfg_dataGridView[2, k].Value.ToString(), 16) % flag).ToString(tostring_format);
                        cfg_dataGridView.Rows[k].Cells[3].Value = tt0;
                    }
                    else
                        tt0 = Convert.ToUInt32(cfg_dataGridView[3, k].Value.ToString(), 16).ToString(tostring_format);
                }
                else
                {
                    if ((cfg_dataGridView.Rows[k].HeaderCell.Value.ToString()) == (cfg_dataGridView.Rows[k - 1].HeaderCell.Value.ToString()))
                    {
                        if (FMXX_SEL_CFGcomboBox.Text == "FM347" && (cfg_dataGridView.Rows[k].Cells[1].Value.ToString().Substring(1, 2) == "手写"))  //仅取有效位数
                        {
                            strBit = cfg_dataGridView[0, k].Value.ToString();
                            index = strBit.IndexOf('~');
                            strbitstart = strToHexByte(strBit.Substring(0, index))[0];
                            strbitend = strToHexByte(strBit.Substring(index + 1, strBit.Length - 1 - index))[0];
                            bitnum = strbitend - strbitstart + 1;
                            flag = 1;
                            flag <<= bitnum;
                            strBit = cfg_dataGridView[0, k - 1].Value.ToString();
                            index = strBit.IndexOf('~');
                            strbitend = strToHexByte(strBit.Substring(index + 1, strBit.Length - 1 - index))[0];
                            tt = (Convert.ToUInt32(cfg_dataGridView[2, k].Value.ToString(), 16) % flag) << (strbitend + 1);
                            tt0 = (Convert.ToUInt32(tt0, 16) | tt).ToString(tostring_format);
                            cfg_dataGridView.Rows[k].Cells[3].Value = tt.ToString(tostring_format);
                        }
                        else
                            tt0 = (Convert.ToUInt32(tt0, 16) | Convert.ToUInt32(cfg_dataGridView[3, k].Value.ToString(), 16)).ToString(tostring_format);
                    }
                    else
                    {
                        strcwd += tt0;
                        if (FMXX_SEL_CFGcomboBox.Text == "FM347" && (cfg_dataGridView.Rows[k].Cells[1].Value.ToString().Substring(1, 2) == "手写"))   //仅取有效位数
                        {
                            strBit = cfg_dataGridView[0, k].Value.ToString();
                            index = strBit.IndexOf('~');
                            strbitstart = strToHexByte(strBit.Substring(0, index))[0];
                            strbitend = strToHexByte(strBit.Substring(index + 1, strBit.Length - 1 - index))[0];
                            bitnum = strbitend - strbitstart + 1;
                            flag = 1;
                            flag <<= bitnum;
                            if (strbitstart == 0)
                                tt0 = (Convert.ToUInt32(cfg_dataGridView[2, k].Value.ToString(), 16) % flag).ToString(tostring_format);
                            else
                                tt0 = ((Convert.ToUInt32(cfg_dataGridView[2, k].Value.ToString(), 16) % flag) << strbitstart).ToString(tostring_format);
                            cfg_dataGridView.Rows[k].Cells[3].Value = tt0;
                        }
                        else
                            tt0 = Convert.ToUInt32(cfg_dataGridView[3, k].Value.ToString(), 16).ToString(tostring_format);
                    }
                }
                /*
                tt2 = "";
                tt1 = "";
                
                if (k == 0)
                {
                    tt0 = strToHexByte(cfg_dataGridView[3, k].Value.ToString())[0].ToString("X2");
                }
                else
                {
                    if (cfg_dataGridView.Rows[k].HeaderCell.Value == null)
                    {
                        break;
                    }
                    if ((cfg_dataGridView.Rows[k].HeaderCell.Value.ToString()) == (cfg_dataGridView.Rows[k - 1].HeaderCell.Value.ToString()))
                    {
                        tt0 = (strToHexByte(tt0)[0] + strToHexByte(cfg_dataGridView[3, k].Value.ToString())[0]).ToString("X2");
                        flag = 1;
                    }
                    else if (cfg_dataGridView.Rows[k + 1].HeaderCell.Value == null)
                    {
                        if (flag == 1)
                        {
                            tt2 = tt0;
                            flag = 0;
                        }
                        tt1 = strToHexByte(cfg_dataGridView[3, k].Value.ToString())[0].ToString("X2");
                        strcwd += tt2;
                        strcwd += tt1;
                        break;
                    }
                    else if (cfg_dataGridView.Rows[k].HeaderCell.Value.ToString() == cfg_dataGridView.Rows[k + 1].HeaderCell.Value.ToString())
                    {
                        if (flag == 1)
                        {
                            tt2 = tt0;
                            flag = 0;
                        }
                        tt0 = strToHexByte(cfg_dataGridView[3, k].Value.ToString())[0].ToString("X2");
                    }
                    else
                    {
                        if (flag == 1)
                        {
                            tt2 = tt0;
                            flag = 0;
                        }
                        tt1 = strToHexByte(cfg_dataGridView[3, k].Value.ToString())[0].ToString("X2");
                    }
                }
                strcwd += tt2;
                strcwd += tt1;
                 */
            }

            // fm349 NVR0 ~ NVR5自动计算反码，扰乱字为全0时可用
            if ((CB_WordRegEn.Checked == true) && (CB_AutoReverse.Checked == true))
            {
                int count = CalNVRCount();
                if ((count * 256) != strcwd.Length)
                {
                    MessageBox.Show("位数不对");
                    SendData_textBox.Text = strcwd;
                    return;
                }

                uint nvr_disturb = 0;
                int disturb_offset = 0;
                if (CB_DisturbDoubleCheck.Checked == true)
                {
                    nvr_disturb = Convert.ToUInt32(nvrDisturbParam.Text, 16);
                    if ((nvr_disturb & 0xC0FFFFFF) != 0)
                    {
                        DisplayMessageLine("扰乱因子无效，此次结果未扰乱！！");
                    }
                    nvr_disturb = nvr_disturb >> 0x18;
                    if ((((nvr_disturb ^ (nvr_disturb >> 0x03)) & 0x07) != 0x07) && (nvr_disturb != 0))
                    {
                        DisplayMessageLine("扰乱因子反码校验失败，此次结果未扰乱！！");
                    }

                    // 检查是否处于不自动写入反码检验模式
                    // 检查扰乱字配置和软件设置是否一致                 
                    if (CB_NVR0_SEL.Checked == true)
                    {
                        // 检测0_CW12中自动反码校验是否使能
                        if ((Convert.ToUInt32(strcwd.Substring(12 * 8, 8), 16) & 0x00000100) != 0)
                        {
                            MessageBox.Show("当前为自动写入反码校验模式，如果修改扰乱字可能导致错误，请确认！");
                        }
                        disturb_offset++;
                    }
                    else
                    {
                        MessageBox.Show("没有NVR0的信息，无法检测是否未使能自动写入反码校验模式，如果要修改扰乱字，请确认！");
                    }
                    if (CB_NVR1_SEL.Checked == true)
                    {
                        disturb_offset++;
                    }
                    if (CB_NVR2_SEL.Checked == true)
                    {
                        if (Convert.ToUInt32(strcwd.Substring(disturb_offset * 256 + 16 * 8, 8), 16) != (nvr_disturb << 0x18))
                        {
                            MessageBox.Show("检测到配置字2_CW16检验字地址扰乱因子与生成配置不一致，请确认");
                        }
                    }
                    else
                    {
                        MessageBox.Show("没有NVR2的信息，无法检测扰乱字与配置是否一致，如果要修改扰乱字，请确认！");
                    }
                    nvr_disturb = nvr_disturb & 0x07;
                }
                else
                {
                    if ((CB_NVR0_SEL.Checked == true) && (CB_NVR2_SEL.Checked == true))
                    {
                        if (CB_NVR1_SEL.Checked == true) disturb_offset = 2; else disturb_offset = 1;
                        if (((Convert.ToUInt32(strcwd.Substring(12 * 8, 8), 16) & 0x00000100) == 0) &&
                            (Convert.ToUInt32(strcwd.Substring(disturb_offset * 256 + 16 * 8, 8), 16) != 0))
                        {
                            MessageBox.Show("检测到当前为不自动写入反码校验模式，并且扰乱字不为0，请确认");
                        }
                    }
                }

                // 如果NVR6被选中，则减少一个count，因为NVR6没有反码
                if (CB_NVR6_SEL.Checked == true) count--;
                uint nvr_current = 0;
                for (int i = 0; i < count; i++)
                {
                    string temp;
                    uint nvr_disturb_n;
                    while (nvr_current < 6)
                    {
                        nvr_current++;
                        if (((CheckBox)this.Controls.Find("CB_NVR" + (nvr_current - 1) + "_SEL", true)[0]).Checked == true)
                            break;
                    }
                    nvr_disturb_n = nvr_disturb == 0 ? 0 : nvr_disturb ^ (nvr_current - 1);
                    for (int j = 0; j < 8; j++)
                    {
                        temp = (Convert.ToUInt32(strcwd.Substring(i * 256 + 64 + j * 8, 8), 16) ^ 0xFFFFFFFF).ToString("X8");
                        //     DisplayMessageLine(temp);
                        strcwd = strcwd.Remove(i * 256 + (j ^ (int)nvr_disturb_n) * 8, 8).Insert(i * 256 + (j ^ (int)nvr_disturb_n) * 8, temp);
                    }
                }
                SendData_textBox.Text = strcwd;
                AnalysisCWD_button_Click(null, EventArgs.Empty);
            }
            if (FMXX_SEL_CFGcomboBox.Text == "FM274" || FMXX_SEL_CFGcomboBox.Text == "FM294" || FMXX_SEL_CFGcomboBox.Text == "FM302")
            {
                tmp = strToHexByte(strcwd + strcwd);
                j = strcwd.Length / 2;
                for (i = j - 1; i > 0; i--)
                {
                    tmp[2 * i] = tmp[i];
                    tmp[2 * i + 1] = tmp[i];
                    tmp[2 * i + 1] ^= 0xFF;
                }
                tmp[1] = tmp[0];
                tmp[1] ^= 0xFF;
                strcwd = byteToHexStr(2 * j, tmp);
                if (FMXX_SEL_CFGcomboBox.Text == "FM274")
                    strcwd = strcwd + "55AA000000000000";
                else if (FMXX_SEL_CFGcomboBox.Text == "FM302")
                    strcwd = strcwd + "00000000000000000000";

            }
            SendData_textBox.Text = strcwd;
        }

        private void cfg_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string cc = "";
            //Int32 rowcounter=0;
            string strData = "00";
            string strItem = "00";

            try
            {
                if (!(System.IO.File.Exists(Application.StartupPath + CfgFileName)))                  //检查所要操作的xml文件是否存在
                {
                    DisplayMessageLine("运行目录下的配置文件" + CfgFileName + "文件不存在!请置入");
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(Application.StartupPath + CfgFileName);    //加载Xml文件  
                XmlElement rootElem = doc.DocumentElement;   //获取根节点  
                XmlNodeList configNodes = rootElem.GetElementsByTagName("config"); //获取person子节点集合 
                string ffff = Convert.ToString(this.cfg_dataGridView.CurrentCell.Value);
                if (ffff == "")
                {
                    return;
                }
                if (this.cfg_dataGridView.CurrentCell.ColumnIndex == 2)
                {
                    foreach (XmlNode node in configNodes)
                    {
                        string strCwd = ((XmlElement)node).GetAttribute("CWD");   //获取name属性值 
                        string strContex = ((XmlElement)node).GetAttribute("contex");   //获取name属性值 
                        XmlNodeList subItemNodes = ((XmlElement)node).GetElementsByTagName("item");  //获取age子XmlElement集合                      
                        if (cfg_dataGridView.Rows[this.cfg_dataGridView.CurrentCell.RowIndex].Cells[1].Value.ToString() == strContex)
                        {
                            foreach (XmlElement element in subItemNodes)
                            {
                                strItem = ((XmlElement)element).GetAttribute("cfgitem");
                                if (ffff == strItem)
                                {
                                    XmlNodeList dataNodes = element.ChildNodes;
                                    j = dataNodes.Count;
                                    if (j > 0)
                                    {
                                        strData = dataNodes[j - 1].InnerText;
                                    }
                                    else
                                    {
                                        strData = "无效数据";
                                    }
                                    cc = strData;
                                    cfg_dataGridView.Rows[this.cfg_dataGridView.CurrentCell.RowIndex].Cells[3].Value = cc;
                                    break;
                                }
                            }
                            break;
                        }


                        if (cfg_dataGridView.Rows[this.cfg_dataGridView.CurrentCell.RowIndex].Cells[1].Value.ToString().Substring(1, 2) == "手写")
                        {
                            cfg_dataGridView.Rows[this.cfg_dataGridView.CurrentCell.RowIndex].Cells[3].Value = cfg_dataGridView.CurrentCell.Value;
                            break;
                            //cfg_dataGridView[3, k].Value = cfg_dataGridView[2, k].Value.ToString();
                        }
                        /*else
                        {
                            cfg_dataGridView.Rows[this.cfg_dataGridView.CurrentCell.RowIndex].Cells[3].Value = cc;
                        }*/

                        //}


                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void cfg_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
        private void cfg_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (cfg_dataGridView.IsCurrentCellDirty == true)
            {
                cfg_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            cfg_dataGridView.EndEdit();
        }

        private void WriteCWD_button_Click(object sender, EventArgs e)
        {
            string block0;
            string block1 = "";
            string strReceived, sendbuf_data;

            GenerateCWD_button_Click(null, EventArgs.Empty);
            try
            {
                block0 = (SendData_textBox.Text).Substring(0, 32);
                if (FMXX_SEL_CFGcomboBox.Text != "FM274" && FMXX_SEL_CFGcomboBox.Text != "FM294" && FMXX_SEL_CFGcomboBox.Text != "FM302")
                    block1 = (SendData_textBox.Text).Substring(32, 32);
                if (CT_newcfg_radioButton.Checked)
                {
                    ConnectReader_Click(null, EventArgs.Empty);
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                    Cold_Reset_Click(null, EventArgs.Empty);
                    FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    if (strReceived == "9000" || strReceived == "019000")
                        strReceived = "ok";
                    else
                    {
                        strReceived = "error";
                        DisplayMessageLine("初始化指令出错");
                        return;
                    }
                    if (CB_WordRegEn.Checked == false)
                    {
                        if (FMXX_SEL_CFGcomboBox.Text == "FM274")
                        {
                            FM12XX_Card.Set_TDA8007_reg(0x02, 0x01, out strReceived); //修改通讯波特率115200
                            FM12XX_Card.TransceiveCT("00022D0200", "02", out strReceived);
                        }
                        else if (FMXX_SEL_CFGcomboBox.Text == "FM294")
                        {
                            FM12XX_Card.Set_TDA8007_reg(0x02, 0x01, out strReceived); //修改通讯波特率115200
                            FM12XX_Card.TransceiveCT("0002020000", "02", out strReceived);
                        }
                        else
                            FM12XX_Card.TransceiveCT("0002010000", "02", out strReceived);
                        if (FMXX_SEL_CFGcomboBox.Text == "FM274" || FMXX_SEL_CFGcomboBox.Text == "FM294")
                            FM12XX_Card.TransceiveCT("0000000010", "02", out strReceived);
                        else
                            FM12XX_Card.TransceiveCT("0000000020", "02", out strReceived);
                        FM12XX_Card.TransceiveCT(SendData_textBox.Text, "02", out strReceived);
                        if (strReceived == "9000")
                        {
                            DisplayMessageLine("控制字写入正确");
                        }
                        else
                        {
                            DisplayMessageLine("控制字写入失败");
                            return;
                        }
                    }
                    else
                    {
                        if (FMXX_SEL_CFGcomboBox.Text == "FM347")
                        {
                            sendbuf_data = SendData_textBox.Text.Substring(0, 72); //config word
                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                            FM12XX_Card.TransceiveCT("0004000064", "02", out strReceived); //bak sector                                             
                            sendbuf_data += strReceived.Substring(74, 128);

                            FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                            FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
                            FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);

                            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                            FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                            FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

                            FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                            FM12XX_Card.TransceiveCT("00AF0000" + "00DE0000" + "00000063" + "00000045", "02", out strReceived);

                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
                            FM12XX_Card.TransceiveCT("0004000001", "02", out strReceived); // NVM

                            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                            FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
                            int i = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                            if (i == 0)
                                DisplayMessageLine("写入配置字:\t<-\t" + "Successed");
                            else
                            {
                                DisplayMessageLine("写入配置字:\t<-\t" + "ERROR -- " + strReceived);
                                return;
                            }
                            sendbuf_data = SendData_textBox.Text.Substring(72, 16); //LCOK word
                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                            FM12XX_Card.TransceiveCT("0004010048", "02", out strReceived); //bak sector                                             
                            sendbuf_data = strReceived.Substring(2, 24) + sendbuf_data + strReceived.Substring(42, 104);

                            FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                            FM12XX_Card.TransceiveCT("003C000048", "02", out strReceived); //cl_ram 0000
                            FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);

                            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                            FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                            FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

                            FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                            FM12XX_Card.TransceiveCT("00AF0000" + "00DE0100" + "00000047" + "00000045", "02", out strReceived);

                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
                            FM12XX_Card.TransceiveCT("0004010001", "02", out strReceived); // NVM

                            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                            FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
                            i = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                            if (i == 0)
                                DisplayMessageLine("写入LOCK字:\t<-\t" + "Successed");
                            else
                            {
                                DisplayMessageLine("写入LOCK字:\t<-\t" + "ERROR -- " + strReceived);
                                return;
                            }
                            sendbuf_data = SendData_textBox.Text.Substring(0, 72); //config word
                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                            FM12XX_Card.TransceiveCT("0004060064", "02", out strReceived); //bak sector                                             
                            sendbuf_data += strReceived.Substring(74, 128);

                            FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                            FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
                            FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);

                            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                            FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                            FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

                            FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                            FM12XX_Card.TransceiveCT("00AF0000" + "00DE0600" + "00000063" + "00000045", "02", out strReceived);

                            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
                            FM12XX_Card.TransceiveCT("0004060001", "02", out strReceived); // NVM

                            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                            FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
                            i = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                            if (i == 0)
                                DisplayMessageLine("写入备份区:\t<-\t" + "Successed");
                            else
                            {
                                DisplayMessageLine("写入备份区:\t<-\t" + "ERROR -- " + strReceived);
                                return;
                            }
                        }
                        else
                        {
                            int nvr_count = 0;
                            string nvr_data;
                            // 带片擦的编程模式
                            FM12XX_Card.TransceiveCT("0002030000", "02", out strReceived);
                            for (int i = 0; i < 7; i++)
                            {
                                string cmd_data = "0000DE" + (0x80 * i).ToString("X4");
                                if (((CheckBox)this.Controls.Find("CB_NVR" + i + "_SEL", true)[0]).Checked == true)
                                {
                                    FM12XX_Card.TransceiveCT(cmd_data, "02", out strReceived);
                                    nvr_data = SendData_textBox.Text.Substring(nvr_count * 256, 256);
                                    FM12XX_Card.TransceiveCT(nvr_data, "02", out strReceived);
                                    if (strReceived == "9000")
                                    {
                                        DisplayMessageLine("NVR" + i + "控制字写入正确");
                                    }
                                    else if (strReceived == "9200")
                                    {
                                        //如果出现安全报警，读取当前NVR，判断是否写入
                                        FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                                        if (strReceived != "9000")
                                        {
                                            DisplayMessageLine("NVR" + i + "安全报警，定位高地址出错");
                                            return;
                                        }
                                        FM12XX_Card.TransceiveCT("0004" + (0x80 * i).ToString("X4") + "80", "02", out strReceived);
                                        if (strReceived == ("04" + nvr_data + "9000"))
                                        {
                                            DisplayMessageLine("NVR" + i + "安全报警，对比后，控制字写入成功");
                                        }
                                        else
                                        {
                                            DisplayMessageLine("NVR" + i + "安全报警，对比后，控制字写入不正确");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        DisplayMessageLine("NVR" + i + "控制字写入失败");
                                        return;
                                    }
                                    nvr_count++;
                                }
                            }
                        }
                    }
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                }
                else  //非接写操作
                {
                    ConnectReader_Click(null, EventArgs.Empty);
                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                    Field_ON_Click(null, EventArgs.Empty);
                    Active_Click(null, EventArgs.Empty);
                    if (FM12XX_Card.select_error_flag == 1)
                    {
                        DisplayMessageLine("选卡失败，请重新选卡!!!!!!!!!");
                        return;
                    }
                    if (CB_WordRegEn.Checked == false)
                    {
                        if (FMXX_SEL_CFGcomboBox.Text == "FM274")
                            FM12XX_Card.TransceiveCL("31B0", "01", "09", out strReceived);
                        else if (FMXX_SEL_CFGcomboBox.Text == "FM294")
                            FM12XX_Card.TransceiveCL("31A0", "01", "09", out strReceived);
                        else if (FMXX_SEL_CFGcomboBox.Text == "FM302")
                            FM12XX_Card.TransceiveCL("3190", "01", "09", out strReceived);
                        else
                            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                        if (strReceived == "00000000000000000000000000000000")
                        {
                            strReceived = "ok";
                        }
                        else
                        {
                            strReceived = "error";
                            DisplayMessageLine("初始化指令出错");
                            return;
                        }
                        //TransceiveCL("3180", "01", "09", out block0);
                        if (FMXX_SEL_CFGcomboBox.Text == "FM274" || FMXX_SEL_CFGcomboBox.Text == "FM294" || FMXX_SEL_CFGcomboBox.Text == "FM302")
                            FM12XX_Card.WriteBlock("00", block0, out strReceived);
                        else
                        {
                            FM12XX_Card.WriteBlock("00", block0, out strReceived);
                            FM12XX_Card.WriteBlock("01", block1, out strReceived);
                        }
                        if (strReceived == "Succeeded")
                        {
                            DisplayMessageLine("控制字写入正确");
                        }
                        else
                        {
                            DisplayMessageLine("控制字写入失败");
                            return;
                        }
                    }
                    else // for fm349
                    {
                        // 带片擦的编程模式,程序FLASH退出DWD模式
                        //FM12XX_Card.TransceiveCL("33C3", "01", "09", out strReceived);
                        block0 = "";
                        // block1 = "";
                        //    // 配置逻辑地址的bit23~bit20
                        //    FM12XX_Card.SendReceiveCL("31" + "8D", out strReceived);
                        //    // 配置逻辑地址的bit19~bit12
                        //    FM12XX_Card.SendReceiveCL("32" + "E0", out strReceived);
                        //    int nvr_count = 0;
                        //    for (int i = 0; i < 7; i++)
                        //    {
                        //        if (((CheckBox)this.Controls.Find("CB_NVR" + i + "_SEL", true)[0]).Checked == true)
                        //        {
                        //            for (int j = 0; j < 8; j++)
                        //            {
                        //                block0 = SendData_textBox.Text.Substring((nvr_count + j) * 32, 32);
                        //                FM12XX_Card.WriteBlock((i*8+j).ToString("X2"), block0, out strReceived);
                        //                if (strReceived != "Succeeded")
                        //                {
                        //                    DisplayMessageLine("NVR"+i+"_block"+j+"控制字写入失败");
                        //                    return;
                        //                }
                        //            }
                        //            nvr_count += 0x08;
                        //        }
                        //    }
                        //    DisplayMessageLine("控制字写入成功");
                        //}
                        //使用A1指令进行写块操作。
                        int nvr_count = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            // 配置逻辑地址的bit23~bit20
                            FM12XX_Card.TransceiveCL("318D", "01", "09", out strReceived);
                            if (strReceived != "00000000000000000000000000000000")
                            {
                                DisplayMessageLine("扩展地址指令0x31失败，安全区可能需要认证");
                                return;
                            }
                            // 配置逻辑地址的bit19~bit12
                            FM12XX_Card.TransceiveCL("32E0", "01", "09", out strReceived);
                            if (strReceived != "00000000000000000000000000000000")
                            {
                                DisplayMessageLine("扩展地址指令0x32失败，安全区可能需要认证");
                                return;
                            }
                            if (((CheckBox)this.Controls.Find("CB_NVR" + i + "_SEL", true)[0]).Checked == true)
                            {
                                string cmd_data_sector = "A1" + (0x08 * i).ToString("X2");
                                FM12XX_Card.TransceiveCL(cmd_data_sector, "03", "09", out strReceived); //Only TX CRC En
                                if (strReceived == "04")
                                {
                                    DisplayMessageLine("NVR" + i + "权限检测失败，无法下发数据");
                                    return;
                                }
                                block0 = SendData_textBox.Text.Substring(nvr_count * 256, 256);
                                FM12XX_Card.TransceiveCL(block0, "03", "09", out strReceived);//Only TX CRC en
                                if (strReceived == "0A")
                                {
                                    DisplayMessageLine("NVR" + i + "控制字写入成功");
                                }
                                else if (strReceived == "04")
                                {
                                    DisplayMessageLine("NVR" + i + "控制字写入失败，或数据校验错误");
                                    return;
                                }
                                else
                                {
                                    DisplayMessageLine("NVR" + i + "控制字写入失败，写指令无响应");
                                    return;
                                }
                                nvr_count++;
                            }
                        }
                        DisplayMessageLine("控制字写入成功");
                    }

                    Reset17_Click(null, EventArgs.Empty);
                    Init_TDA8007_Click(null, EventArgs.Empty);
                }
                //     SendData_textBox.Text = block0 + block1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void FMXX_SEL_CFGcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // fm349
            if ((FMXX_SEL_CFGcomboBox.Text=="FM349")||(FMXX_SEL_CFGcomboBox.Text=="FM349_NEW")||(FMXX_SEL_CFGcomboBox.Text == "FM349_Security"))
            {
                CB_WordRegEn.Checked = true;
                CB_NVRALL_SEL.Checked = false;
                CB_AutoReverse.Enabled = true;
                CB_NVR0_SEL.Enabled = true;
                CB_NVR1_SEL.Enabled = true;
                CB_NVR2_SEL.Enabled = true;
                CB_NVR3_SEL.Enabled = true;
                CB_NVR4_SEL.Enabled = true;
                CB_NVR5_SEL.Enabled = true;
                CB_NVR6_SEL.Enabled = true;
                CB_NVRALL_SEL.Enabled = true;
                CB_AutoReverse.Checked = true;
                if ((FMXX_SEL_CFGcomboBox.Text == "FM349_NEW") || (FMXX_SEL_CFGcomboBox.Text == "FM349_Security"))
                    CB_EnableOption.Enabled = true;
                else
                {
                    CB_EnableOption.Enabled = false;
                    CB_EnableOption.Checked = false;
                }
                CB_NVRALL_SEL_Click(null, EventArgs.Empty);
                DisplayMessageLine("请注意配置字数据顺序为：byte0 byte1 byte2 byte3！！");
            }
            else
            {
                CB_WordRegEn.Checked = false;
                CB_AutoReverse.Enabled = false;
                CB_NVR0_SEL.Enabled = false;
                CB_NVR1_SEL.Enabled = false;
                CB_NVR2_SEL.Enabled = false;
                CB_NVR3_SEL.Enabled = false;
                CB_NVR4_SEL.Enabled = false;
                CB_NVR5_SEL.Enabled = false;
                CB_NVR6_SEL.Enabled = false;
                CB_NVRALL_SEL.Enabled = false;
                CB_AutoReverse.Checked = false;
                CB_NVRALL_SEL.Checked = false;
                CB_EnableOption.Enabled = false;
                CB_EnableOption.Checked = false;               
//                Load_NEW_cfg_dataGridView();               
            }
            if (FMXX_SEL_CFGcomboBox.Text=="FM347")           
            {
                CB_WordRegEn.Checked = true;
                DisplayMessageLine("请注意配置字数据顺序为：byte3 byte2 byte1 byte0！！");
            }
             Load_NEW_cfg_dataGridView();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            /*if (radioButton1.Checked)
                label56.Text = "只指向前128K";
            else
                label56.Text = "只指向前64K";*/
            Init_EEopt_02.Text = "EEopt 02";
            Init_Rdee_04.Text = "RDEE 04";
            Init_Wree_00.Text = "WREE 00";
            Init_ewEEtime_03.Text = "擦写EE 03";
            Init_CPUStart_08.Text = "启动cpu 08";
            Init_Trim_0B.Text = "调校 0B";
            init_CRC_0A.Text = "取CRC 0A";
            init_AUTH_55.Text = "认证 55";
            text_0B_P3.Visible = false;
            CT_347_STOPMCU.Visible = false;
            text_08_P1.Visible = true;
            ins54.Visible = false;
            trim8m.Visible = false;
            trim_wr.Visible = false;
            groupBox21.Visible = false;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string strOut;
            try
            {
                FM309_flash_isp.Uart_send(SendData_textBox.Text.ToString());
                if (SendData_textBox.Text.ToString() != "78")
                {
                    FM309_flash_isp.Uart_Rev_test(out strOut);
                    SendData_textBox.Text = strOut;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string strOut;
            int commstate, baudrate;
            if (button9.Text == "打开")
            {
                commstate = 0;
                button9.Text = "关闭";
            }
            else
            {
                commstate = 1;
                button9.Text = "打开";
            }
            switch (comboBox1.SelectedItem.ToString())
            {
                case "57600": baudrate = 57600; break;
                case "19200": baudrate = 19200; break;
                case "9600": baudrate = 9600; break;
                case "4800": baudrate = 4800; break;
                default: baudrate = 115200; break;
            }
            FM309_flash_isp.CommOpen(flash_isp_comSelect.SelectedItem.ToString(), baudrate, commstate, out strOut);
            DisplayMessageLine(strOut);
        }

        private void FM294_str_Click(object sender, EventArgs e)
        {
            string strReceived;
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex;
            int i, j, t, tmp;
            int addr = 0;
            byte[] bytedata;
            byte[] uid;
            int result;
            byte[] buff;
            string BlockAddr, BlockData;

            strReceived = "";
            tmp = 0;
            temp_rlt.Text = "请耐心等待……";
            FM12XX_Card.SetField(0, out display);
            FM12XX_Card.Init_TDA8007(out display);

            FM12XX_Card.Active(out display);
            DisplayMessageLine("Active --> "+display);
            FM12XX_Card.SendReceiveCL("31A0", out strReceived);
            FM12XX_Card.ReadBlock("00", out strReceived);
            uid = strToHexByte(strReceived);         
            uid[10] = 0x7F;  //-35℃报警，85℃报警。
            uid[11] = 0x80;
            tempStr = byteToHexStr(16, uid);
            FM12XX_Card.WriteBlock("00", tempStr, out strReceived);
            FM12XX_Card.SetField(0, out display);

            ProgFileLenth = 0;
            open_flag = 1;
            openFileDialog_Prog = null;
            openFileDialog_Prog = new OpenFileDialog();
            openFileDialog_Prog.InitialDirectory = HexDirectory;
            openFileDialog_Prog.Reset();
            openFileDialog_Scr.Reset();
            openFileDialog_Prog.FileName = ".\\FM294Cos.hex";
            HexDirectory = openFileDialog_Prog.FileName;
            RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
            hexfiledata = new byte[RawCode.Length];
            bin_data = new byte[RawCode.Length];

            for (i = 0; i < RawCode.Length; i++)
            {
                szHex = "";
                tempStr = RawCode[i];
                datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                addrStr = tempStr.Substring(3, 4);//记录该行的地址
                szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                bytedata = strToHexByte(szHex);
                addr = (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                if (addr >= ProgFileLenth)
                {
                    ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                }
                for (j = 0; j < strToHexByte(datacountStr)[0]; j++)
                {
                    Progfilebuf[addr + j] = bytedata[j];
                }

            }

            buff = uid;
            j = (ProgFileLenth / 0x10) - 0x100 + 1;
            DisplayMessageLine("CL编程切开始……");
            temp_rlt.Text = "programing! please wait……";
            FM12XX_Card.Active(out display);
            DisplayMessageLine(display);
            FM12XX_Card.SendReceiveCL("3190", out strReceived);
            for (i = 0; i <= 0xFF; i++)
            {
                for (t = 0; t < 16; t++)
                {
                    tmp = t + i * 16;
                    buff[t] = Progfilebuf[tmp];
                }
                BlockAddr = (i).ToString("X2");
                BlockData = byteToHexStr(0x10, buff);
                result = FM12XX_Card.WriteBlock(BlockAddr, BlockData, out strReceived);
                if (result == 1)        //编程出错
                {
                    strReceived = "编程出错" + (i).ToString("X2");
                    display = strReceived;
                    DisplayMessageLine(display);
                    return;
                }
            }
            display = "CL编程切换地址 3191……";
            DisplayMessageLine(display);
            FM12XX_Card.SendReceiveCL("3191", out strReceived);
            for (i = 0; i <= j; i++)
            {
                for (t = 0; t < 16; t++)
                {
                    tmp = t + i * 16 + 0x1000;
                    buff[t] = Progfilebuf[tmp];
                }
                BlockAddr = (i).ToString("X2");
                BlockData = byteToHexStr(0x10, buff);
                result = FM12XX_Card.WriteBlock(BlockAddr, BlockData, out strReceived);
                if (result == 1)        //编程出错
                {
                    strReceived = "编程出错" + (0x100 + i).ToString("X4");
                    display = strReceived;
                    DisplayMessageLine(display);
                    return;
                }
            }
            DisplayMessageLine("CL编程成功！");

            FM12XX_Card.SetField(0, out display);
            FM12XX_Card.Active(out display);
            DisplayMessageLine("Active --> " + display);
            FM12XX_Card.RATS(out display);
            DisplayMessageLine("RATS --> " + display);

            //DisplayMessageLine("写F019--00");
            //FM12XX_Card.SendReceiveCL("5319F00100", out strReceived);
            //DisplayMessageLine(strReceived);

            DisplayMessageLine("写F018--08");    //开启温度检测
            FM12XX_Card.SendReceiveCL("5318F00108", out strReceived);
            DisplayMessageLine(strReceived);

            //DisplayMessageLine("写F010--80");
            //FM12XX_Card.SendReceiveCL("5310F00180", out strReceived);
            //DisplayMessageLine(strReceived);

            DisplayMessageLine("延时30妙 ");
            Delay(30);
            DisplayMessageLine("延时结束 ");

            FM12XX_Card.SendReceiveCL("511AF001", out strReceived);
            DisplayMessageLine("读F01A -->" + strReceived);
            buff = strToHexByte(strReceived);
            if (buff[2] == 0x08)
                temp_rlt.Text = "挑卡成功，感谢主！";
            else
                temp_rlt.Text = "挑卡失败，忍耐等候！";
            DisplayMessageLine("挑卡结束，请更换卡片");
            FM12XX_Card.SetField(0, out display);
             
        }

        private void Chip_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (Chip_comboBox.Text)
                {
                    case "FM336": FM336_radioButton.Checked = true; FM309_radioButton.Checked = false; break;
                    default: FM336_radioButton.Checked = false; FM309_radioButton.Checked = true; break;
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void FM336_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (FM336_radioButton.Checked)
                EE_patch_checkBox.Enabled = true;
            else
                EE_patch_checkBox.Enabled = false;
        }

        private void EE_patch_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            DisplayMessageLine("请注意控制字补丁值和编程时补丁值大小一致！");
            if (EE_patch_checkBox.Checked)
            {
                label68.Visible = true;
                textBox1.Visible = true;
                label67.Visible = true;
            }
            else
            {
                label68.Visible = false;
                textBox1.Visible = false;
                label67.Visible = false;
            }
        }

        private void CB_NVRSEL_CheckedChanged_Process()
        {
            if ((CB_NVR0_SEL.Checked == true) && (CB_NVR1_SEL.Checked == true)
                && (CB_NVR2_SEL.Checked == true) && (CB_NVR3_SEL.Checked == true)
                && (CB_NVR4_SEL.Checked == true) && (CB_NVR5_SEL.Checked == true)
                && (CB_NVR6_SEL.Checked == true))
            {
                CB_NVRALL_SEL.Checked = true;
            }
            else
            {
                CB_NVRALL_SEL.Checked = false;
            }
            Load_NEW_cfg_dataGridView();
            //if(CB_EnableOption.Checked == true)
            //    FM349_ReverseOption();
            //20160412因为不知道为什么第一次加载DataGridView，Row[i].Cells[4].Value没有初始化过，所以这么做
        }

        private void CB_NVRALL_SEL_Click(object sender, EventArgs e)
        {
            if (CB_NVRALL_SEL.Checked == true)
            {
                CB_NVR0_SEL.Checked = true;
                CB_NVR1_SEL.Checked = true;
                CB_NVR2_SEL.Checked = true;
                CB_NVR3_SEL.Checked = true;
                CB_NVR4_SEL.Checked = true;
                CB_NVR5_SEL.Checked = true;
                CB_NVR6_SEL.Checked = true;
            }
            else
            {
                CB_NVR0_SEL.Checked = false;
                CB_NVR1_SEL.Checked = false;
                CB_NVR2_SEL.Checked = false;
                CB_NVR3_SEL.Checked = false;
                CB_NVR4_SEL.Checked = false;
                CB_NVR5_SEL.Checked = false;
                CB_NVR6_SEL.Checked = false;
            }
            Load_NEW_cfg_dataGridView();
            //if (CB_EnableOption.Checked == true)
            //    FM349_ReverseOption();
            //20160412因为不知道为什么第一次加载DataGridView，Row[i].Cells[4].Value没有初始化过，所以这么做
        }

        private void CB_NVR0_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_NVR1_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_NVR2_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_NVR3_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_NVR4_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_NVR5_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_NVR6_SEL_Click(object sender, EventArgs e)
        {
            CB_NVRSEL_CheckedChanged_Process();
        }

        private void CB_EnableOption_Click(object sender, EventArgs e)
        {
            if (CB_EnableOption.Checked == true)
                button_ReverseOption.Enabled = true;
            else
                button_ReverseOption.Enabled = false;
            Load_NEW_cfg_dataGridView();
        }

        private byte OddParityCheck(byte b)
        {
            return (byte)(((b ^ (b >> 1) ^ (b >> 2) ^ (b >> 3) ^ (b >> 4) ^ (b >> 5) ^ (b >> 6) ^ (b >> 7)) & 0x01)^0x01);
        }

        private void saveascoe__button_Click(object sender, EventArgs e)
        {
            string name = "";
            StreamWriter savebuf;
            //SaveFileDialog类
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = Application.StartupPath;
            //过滤器
            save.Filter = "coe file (*.coe)|*.coe|All files (*.*)|*.*";
            //显示
            if (save.ShowDialog() == DialogResult.OK)
            {
                name = save.FileName;
                FileInfo info = new FileInfo(name);
                FileStream fs = info.Create();
                fs.Close();
            }
            else
            {
                return;
            }
            savebuf = File.AppendText(name);
            savebuf.WriteLine("Memory_Initialization_Radix = 16;");
            savebuf.WriteLine("Memory_Initialization_Vector =");

            string tt_temp = DeleteSpaceString(SendData_textBox.Text);
            string tt = "";
            int text_length = tt_temp.Length;
            text_length = text_length / 8;
            byte b0;
            byte b1;
            byte b2;
            byte b3;
            for (i = 0; i < text_length; i++)
            {
                tt = tt_temp.Substring(i*8, 2);
                b0 = Convert.ToByte(tt_temp.Substring(i * 8 + 0, 2), 16);
                b1 = Convert.ToByte(tt_temp.Substring(i * 8 + 2, 2), 16);
                b2 = Convert.ToByte(tt_temp.Substring(i * 8 + 4, 2), 16);
                b3 = Convert.ToByte(tt_temp.Substring(i * 8 + 6, 2), 16);
                tt = (OddParityCheck(b0) + (byte)(b1 << 1)).ToString("X2") + tt;
                tt = (((b1 & 0x80) >> 7) + (OddParityCheck(b1) << 1) + (byte)(b2 << 2)).ToString("X2") + tt;
                tt = (((b2 & 0xC0) >> 6) + (OddParityCheck(b2) << 2) + (byte)(b3 << 3)).ToString("X2") + tt;
                tt = (((b3 & 0xE0) >> 5) + (OddParityCheck(b3) << 3)).ToString("X1") + tt;
                if (i != text_length - 1) tt = tt + ","; else tt = tt + ";";
                savebuf.WriteLine(tt);
            }
            savebuf.Close();
        }

        private void CB_AutoReverse_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_AutoReverse.Checked == true)
            {
                CB_DisturbDoubleCheck.Enabled = true;
            }
            else
            {
                CB_DisturbDoubleCheck.Enabled = false;
            }
        }

        private void CB_DisturbDoubleCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_DisturbDoubleCheck.Checked == true)
            {
                nvrDisturbParam.Enabled = true;
            }
            else
            {
                nvrDisturbParam.Enabled = false;
            }
        }


        private void button_script_reload_Click(object sender, EventArgs e)
        {
            string[] ScrCode, CompareData, RawCode, ControlCode;
            string tempStr, compStr;
            int i = 0, count = 0, position_CompareData = 0;

            if (SimpleScript_backgroundWorker.IsBusy)
            {
                //                MessageBox.Show("脚本正在运行，请耐心等待", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayMessageLine("脚本运行中，请停止后再开始！");
                return;
            }

            if (dataGridView_Script.RowCount <= 1 || dataGridView_flag == 0 || dataGridView_flag == 2)
            {
                dataGridView_flag = 1;
                dataGridView_Script.Rows.Clear();
            }

        //    ScriptDirectory = openFileDialog_Scr.FileName;

            System.Text.Encoding.GetEncoding("gb2312");
            RawCode = File.ReadAllLines(ScriptDirectory, System.Text.Encoding.Default);
            ScrCode = new string[RawCode.Length];
            CompareData = new string[RawCode.Length];
            ControlCode = new string[RawCode.Length];

            for (i = 0; i < RawCode.Length; i++)
            {

                tempStr = RawCode[i];
                if (tempStr == "")
                {
                    continue;
                }
                position_CompareData = tempStr.IndexOf("@");

                if (position_CompareData > 0)
                {
                    ScrCode[count] = tempStr.Substring(0, position_CompareData);
                    compStr = tempStr.Substring(position_CompareData + 1, tempStr.Length - position_CompareData - 1);
                    if (compStr.Contains("pause"))
                    {
                        ControlCode[count] = "●";
                        CompareData[count] = "";
                    }
                    else
                    {
                        compStr = compStr.Split(new Char[] { '#' })[0]; //添加注释
                        CompareData[count] = DeleteSpaceString(compStr);
                        ControlCode[count] = "";
                    }
                    count++;
                }
                else
                {
                    if (tempStr.IndexOf("#") > 0) //添加注释
                        tempStr = tempStr.Split(new Char[] { '#' })[0];
                    ScrCode[count] = tempStr;
                    CompareData[count] = "";
                    ControlCode[count] = "";
                    count++;
                }
            }
            dataGridView_Script.RowCount = count;

            for (i = 0; i < count; i++)
            {
                dataGridView_Script.Rows[i].HeaderCell.Value = ControlCode[i];
                dataGridView_Script["Index", i].Value = i.ToString();
                dataGridView_Script["Commands", i].Value = ScrCode[i];
                dataGridView_Script["CompareData", i].Value = CompareData[i];
            }

//            RstScrScope_Click(null, EventArgs.Empty);
//            ConnectReader_Click(null, EventArgs.Empty);

        }


        private void InitValue_Click(object sender, EventArgs e)
        {
            string block_addr, block_data, StrReceived;

            if (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
            block_addr = BlockAddr_textBox.Text;

            block_data = DeleteSpaceString(SendData_textBox.Text);
            //if (block_data.Length != 32)
            //{
            //    display = "WriteBlock:      \t\t数据长度错误！";
            //    DisplayMessageLine(display);
            //    return;
            //}

            try
            {
                FM12XX_Card.InitVal(block_addr, block_data, out StrReceived);
                display = "WriteBlock " + block_addr + ":   \t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void ReadValue_Click(object sender, EventArgs e)
        {
            string block_addr, StrReceived = "";
            display = "";

            while (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
            block_addr = BlockAddr_textBox.Text;

            try
            {
                int i = FM12XX_Card.ReadVal(block_addr, out StrReceived);

                display = "ReadBlock " + block_addr + ":\t\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }


        private void text_02_P1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private string cfw_addr_rand(string cfw, string dist0, string dist1)  //控制字反码扰乱 zhujunhao 2015.3.25
        {
            string cfw_rd = "";
            string tmp = "";
            byte dist0_bin;
            byte dist1_bin;
            int temp1, temp2, temp3;
            int[] cfw_num;
            int[] cfw_num_rd;
            cfw_num = new int[32];
            cfw_num_rd = new int[32];
            int i;
            dist0_bin = Convert.ToByte(dist0, 16);
            dist1_bin = Convert.ToByte(dist1, 16);

            temp1 = (dist0_bin % 32) ^ (dist1_bin % 32);
            if (temp1 % 2 == 1)
                temp2 = ((int)(dist1_bin / 32)) * 4 + (int)(dist0_bin / 64);
            else
                temp2 = (((int)(dist1_bin / 32)) % 4) * 8 + (int)(dist0_bin / 32);
            temp3 = temp1 ^ temp2;
            for (i = 0; i < 32; i++)
            {
                tmp = cfw.Substring(i * 2, 2);
                cfw_num[i] = Convert.ToByte(tmp, 16);
                cfw_num_rd[temp3 ^ i] = 255 - cfw_num[i];  //取反码，地址重排
            }
            for (i = 0; i < 32; i++)
                cfw_rd = cfw_rd + cfw_num_rd[i].ToString("X2");
//            DisplayMessageLine(cfw_rd);
            return (string)(cfw_rd);
        }

        private void eeprogram(string file_addr, int flag)
        {
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            int i;
            int addr = 0;
            int hex_addr_change;
            byte[] bytedata;

            //下载程序  
            ProgFileLenth = 0;
            open_flag = 1;
            hex_addr_change = 0;
            openFileDialog_Prog = null;
            openFileDialog_Prog = new OpenFileDialog();

            openFileDialog_Prog.InitialDirectory = HexDirectory;
            openFileDialog_Prog.Reset();
            openFileDialog_Scr.Reset();

            if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)//CNN修改 
            {
                dataGridView_flag = 0;
                progEEdataGridView.Rows.Clear();
            }
            openFileDialog_Prog.FileName = file_addr;
            HexDirectory = openFileDialog_Prog.FileName;
            RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
            hexfiledata = new byte[RawCode.Length];
            bin_data = new byte[RawCode.Length];

            StartEEAddr = ProgEEStartAddr.Text;
            EndEEAddr = ProgEEEndAddr.Text;
            for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
            {
                StartEEAddr = "0" + StartEEAddr;
            }
            for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
            {
                EndEEAddr = "0" + EndEEAddr;
            }
            ProgEEStartAddr.Text = StartEEAddr;
            ProgEEEndAddr.Text = EndEEAddr;

            for (i = 0; i < ProgFileMaxLen; i++)
            {
                Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
            }
            ProgOrReadEE_FF_flag = 0;
            ProgOrReadEE_80_flag = 0;
            ProgOrReadEE_81_flag = 0;
            ProgOrReadEE_82_flag = 0;
            ProgOrReadEE_83_flag = 0;
            ProgOrReadEE_84_flag = 0;
            ProgOrReadEE_90_flag = 0;

            for (i = 0; i < RawCode.Length; i++)
            {
                szHex = "";
                tempStr = RawCode[i];
                if (tempStr == "")
                {
                    continue;
                }
                if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                {

                    if (tempStr.Substring(1, 8) == "00000001")//数据结束
                    {
                        break;
                    }
                    if (tempStr.Substring(7, 2) == "04")//段地址
                    {
                        if (tempStr.Substring(11, 2) == "80")//段地址
                        {
                            hex_addr_change = 0x010000;
                            ProgOrReadEE_80_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "81")//段地址
                        {
                            hex_addr_change = 0x020000;
                            ProgOrReadEE_81_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "82")//段地址
                        {
                            hex_addr_change = 0x030000;
                            ProgOrReadEE_82_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "FF")//段地址
                        {
                            hex_addr_change = 0x000000;
                            ProgOrReadEE_FF_flag = 1;
                        }
                        else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                        {
                            hex_addr_change = 0x060000;
                            ProgOrReadEE_90_flag = 1;
                        }
                        else//段后扩展地址
                        {
                            ProgOrReadEE_EXT_flag = 1;
                            if (tempStr.Substring(11, 2) == "83")//段地址
                            {
                                hex_addr_change = 0x040000;
                                ProgOrReadEE_83_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "84")//段地址
                            {
                                hex_addr_change = 0x050000;
                                ProgOrReadEE_84_flag = 1;
                            }
                            /* else if (tempStr.Substring(11, 2) == "85")//段地址
                             {
                                 hex_addr_change = 0x060000;                                    
                             }   */
                        }
                        // addrStr = tempStr.Substring(11, 2);
                        // hex_addr_change = (int)(strToHexByte(addrStr)[0] * 65536);
                    }
                    else
                    {
                        ProgOrReadEE_FF_flag = 1;
                        datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                        addrStr = tempStr.Substring(3, 4);//记录该行的地址
                        szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                        bytedata = strToHexByte(szHex);
                        addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                        if (addr >= ProgFileLenth)
                        {
                            ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                        }
                        for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                        {
                            Progfilebuf[addr + j] = bytedata[j];
                        }
                    }
                    // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                }
            }

            ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
            openFileDialog_Prog.Dispose();
            if (flag == 1)
            {
                CL_Interface.Checked = false;
                CT_Interface.Checked = true;
            }
            else
            {
                CL_Interface.Checked = true;
                CT_Interface.Checked = false;
            }
            ProgDestAddr.Text = "002000";
            ProgEEStartAddr.Text = "020000";
            Prog_Move_Click(null, EventArgs.Empty);
            ProgEEStartAddr.Text = "002000";
            ProgEEEndAddr.Text = "003FFF";
            ProgEE_Button_Click(null, EventArgs.Empty);
        }

        private void brst_Click(object sender, EventArgs e)
        {
            string strReceived, temp_str, str_l, str_2, str_rlt;
            byte[] temp_all = {0}, temp_uid, temp_ref;
            bool zfcheck;
            int ITflag, result;
            if (cfwconfig.Checked)
            {
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                    DisplayMessageLine("NO ATR!");
                FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                if (receive != "9000")
                {
                    DisplayMessageLine("初始化指令失效--停止");
                    return;
                }
                FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                temp_all = strToHexByte(receive.Substring(2, 64));
                if (bank326.Checked == false)
                    FM12XX_Card.TransceiveCT("0004000605", "02", out receive);
                else
                    FM12XX_Card.TransceiveCT("0004000C05", "02", out receive);
                temp_uid = strToHexByte(receive.Substring(2, 10));
                temp_str = "";
                if (gm.Checked)
                    temp_str = "00609FE52E227B0000A3A8013C40FF00660425000000000000940004005500A5";                
                if (ty.Checked)
                    temp_str = "20601F0B2E227B0000A3AC003C00F7007704240000000000009B0006005500A5";
                if (bank.Checked)
                    temp_str = "00609FE52E22630000A3A8013C40FF00660425000000000000940004005500A5"; //寄存器奇偶校验安全检测关闭,开启SM4
                if (zc.Checked)
                    temp_str = "00609FE52E22430000A3A8013C40FF00660425000000000000940004005500A5"; //寄存器奇偶校验安全检测关闭,关闭SM4,关闭正反码校验                
                if (bright.Checked)
                    temp_str = "0060BFBF2E22430000A3A8003C20FF007704250000000000009B0006005500A5"; //安全检测复位关闭，都没有lock
                if (bank326.Checked)
                    temp_str = "24001F0AEE025F0000E580072B202790A60024000000000000000000005555A5"; //关掉寄存器奇偶校验位
                temp_ref = strToHexByte(temp_str);

                temp_ref[7] = temp_uid[0]; //EEPROM 密钥1
                temp_ref[8] = temp_uid[1]; //EEPROM 密钥2
                if (bank326.Checked == false)
                    temp_ref[3] = temp_all[3]; //保存温度配置值
                else
                {
                    temp_all[3] &= 0x3F;
                    temp_ref[3] |= temp_all[3]; //保存温度配置值
                }
                if (bank326.Checked == false)
                    temp_ref[15] = temp_all[15]; //保存vdd15及verf偏置电压trim值
                temp_ref[16] = temp_all[16]; //保存4M及DFS环振trim值

                temp_str = byteToHexStr(32, temp_ref);
                FM12XX_Card.TransceiveCT("0000000020", "02", out receive);
                FM12XX_Card.TransceiveCT(temp_str, "02", out receive);               
                DisplayMessageLine("密钥1： " + temp_ref[7].ToString("X2"));
                DisplayMessageLine("密钥2： " + temp_ref[8].ToString("X2"));
                //-------写认证密钥------------
                if (bank326.Checked == false)
                    FM12XX_Card.TransceiveCT("0000000A10", "02", out receive);
                else
                    FM12XX_Card.TransceiveCT("0000001410", "02", out receive);
                FM12XX_Card.TransceiveCT(Akey.Text, "02", out receive);
                DisplayMessageLine("控制字 + EEPROM密钥 + 认证密钥，初始化完成！");
                FM12XX_Card.Init_TDA8007(out display);
                if (cfw_rnd.Checked)
                {
                    //--------关闭自动写反码---------
                    FM12XX_Card.Init_TDA8007(out display);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived == "Error")
                        DisplayMessageLine("NO ATR!");
                    FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                    if (receive != "9000")
                    {
                        DisplayMessageLine("初始化指令失效--停止");
                        return;
                    }
                    FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                    FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                    temp_all = strToHexByte(receive.Substring(2, 64));
                    temp_all[6] &= 0xBF;       //自动反码 关闭
                    strReceived = byteToHexStr(32, temp_all);
                    FM12XX_Card.TransceiveCT("0000000020", "02", out receive);
                    FM12XX_Card.TransceiveCT(strReceived, "02", out receive);
                    //--------写扰乱因子并开启自动写反码---------
                    FM12XX_Card.Init_TDA8007(out display);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived == "Error")
                        DisplayMessageLine("NO ATR--写扰乱因子及反码!");
                    FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                    if (receive != "9000")
                    {
                        DisplayMessageLine("初始化指令失效，未写扰乱因子--停止");
                        return;
                    }
                    FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                    if (bank326.Checked == false)
                        FM12XX_Card.TransceiveCT("0004000908", "02", out receive);
                    else
                        FM12XX_Card.TransceiveCT("0004001208", "02", out receive);
                    temp_all = strToHexByte(receive.Substring(2, 16));
                    temp_all[4] = temp_uid[1];
                    temp_uid[1] ^= 0xff;
                    temp_all[5] = temp_uid[1];
                    temp_all[6] = temp_uid[0];
                    temp_uid[0] ^= 0xff;
                    temp_all[7] = temp_uid[0];
                    if (bank326.Checked == false)
                        FM12XX_Card.TransceiveCT("0000000908", "02", out receive);
                    else
                        FM12XX_Card.TransceiveCT("0004001208", "02", out receive);
                    temp_str = byteToHexStr(8, temp_all);
                    FM12XX_Card.TransceiveCT(temp_str, "02", out receive);
                    if (bank326.Checked == false)
                        temp_ref[6] |= 0x40;       //自动反码 打开
                    else
                        temp_ref[10] |= 0x80;       //自动反码 打开
                    temp_str = byteToHexStr(32, temp_ref);
                    FM12XX_Card.TransceiveCT("0000000020", "02", out receive);
                    FM12XX_Card.TransceiveCT(temp_str, "02", out receive);

                    str_l = temp_all[4].ToString("X2");
                    str_2 = temp_all[6].ToString("X2");
                    DisplayMessageLine("扰乱因子1： " + str_l);
                    DisplayMessageLine("扰乱因子2： " + str_2);
                    str_rlt = cfw_addr_rand(temp_str, str_l, str_2);
                    if (bank326.Checked == false)
                        FM12XX_Card.TransceiveCT("0000000C20", "02", out receive);
                    else
                        FM12XX_Card.TransceiveCT("0000001820", "02", out receive);
                    FM12XX_Card.TransceiveCT(str_rlt, "02", out receive);
                    FM12XX_Card.Init_TDA8007(out display);
                    DisplayMessageLine("反码扰乱始化完成！");
                }
            }
            if (prog_cos.Checked)
            {                
                FM336_radioButton.Checked = true;

                if (ITcl.Checked == true)
                {
                    ITflag = 0;
                    FM12XX_Card.Init_TDA8007(out display);
                    if (ACTIVE_check(out receive) == 1)
                    {
                        DisplayMessageLine(receive);
                        return;
                    }
                    DisplayMessageLine(receive);
                    FM12XX_Card.SendReceiveCL("32A0", out strReceived);
                    if (zc.Checked == true)
                    {
                        FM12XX_Card.WriteBlock("18", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
                        FM12XX_Card.ReadBlock("18", out strReceived);
                        if (strReceived != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                        {
                            DisplayMessageLine("初始化block18失败！");
                            return;
                        }
                        FM12XX_Card.WriteBlock("19", "0000FFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
                        FM12XX_Card.ReadBlock("19", out strReceived);
                        if (strReceived != "0000FFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                        {
                            DisplayMessageLine("初始化block19失败！");
                            return;
                        }
                        FM12XX_Card.WriteBlock("1A", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
                        FM12XX_Card.ReadBlock("1A", out strReceived);
                        if (strReceived != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                        {
                            DisplayMessageLine("初始化block1A失败！");
                            return;
                        }
                        PROG_EEtype_radiobutton.Checked = true;
                        FM12XX_Card.ReadBlock("00", out strReceived);
                        temp_all = strToHexByte(strReceived);
                        FM12XX_Card.SetField(0, out display);
                        DisplayMessageLine("非接触开始下载COS……");
                    }
                    else if (bank326.Checked == true)
                    {
                        DATA_EEtype_radiobutton.Checked = true;
                        FM12XX_Card.WriteBlock("18", "FF00817E000000000000000000000000", out strReceived);
                        FM12XX_Card.ReadBlock("18", out strReceived);
                        if (strReceived != "FF00817E000000000000000000000000")
                        {
                            DisplayMessageLine("初始化block18失败！");
                            return;
                        }
                        FM12XX_Card.WriteBlock("19", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
                        FM12XX_Card.ReadBlock("19", out strReceived);
                        if (strReceived != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                        {
                            DisplayMessageLine("初始化block19失败！");
                            return;
                        }
                        FM12XX_Card.WriteBlock("1A", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
                        FM12XX_Card.ReadBlock("1A", out strReceived);
                        if (strReceived != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                        {
                            DisplayMessageLine("初始化block1A失败！");
                            return;
                        }
                        FM12XX_Card.SetField(0, out display);
                        DisplayMessageLine("非接触开始下载COS_patch……");
                    }
                    else
                    {
                        PROG_EEtype_radiobutton.Checked = true;
                        FM12XX_Card.ReadBlock("00", out strReceived);
                        temp_all = strToHexByte(strReceived);
                        FM12XX_Card.SetField(0, out display);
                        DisplayMessageLine("非接触开始下载COS……");
                    }
                }
                else
                {
                    ITflag = 1;
                    checkBox_CTHighBaud.Checked = true;
                    FM12XX_Card.Init_TDA8007(out display);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived == "Error")
                        DisplayMessageLine("NO ATR!");
                    FM12XX_Card.TransceiveCT("0001008100", "02", out receive); 
                    if (receive != "9000")
                    {
                        DisplayMessageLine("初始化指令失效--停止");
                        return;
                    }
                    if (zc.Checked == true)
                    {
                        FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                        FM12XX_Card.TransceiveCT("0000003010", "02", out receive);
                        FM12XX_Card.TransceiveCT("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004003010", "02", out receive);
                        if (receive != "04FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9000")
                        {
                            DisplayMessageLine("初始化block18失败！receive: " + receive);
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0000003210", "02", out receive);
                        FM12XX_Card.TransceiveCT("0000FFFFFFFFFFFFFFFFFFFFFFFFFFFF", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004003210", "02", out receive);
                        if (receive != "040000FFFFFFFFFFFFFFFFFFFFFFFFFFFF9000")
                        {
                            DisplayMessageLine("初始化block19失败！receive: " + receive);
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0000003410", "02", out receive);
                        FM12XX_Card.TransceiveCT("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004003410", "02", out receive);
                        if (receive != "04FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9000")
                        {
                            DisplayMessageLine("初始化block1A失败！receive: " + receive);
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                        temp_all = strToHexByte(receive.Substring(2, 64));
                        FM12XX_Card.Init_TDA8007(out display);
                        DisplayMessageLine("接触开始下载COS……");
                    }
                    else if (bank326.Checked == true)
                    {
                        FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                        FM12XX_Card.TransceiveCT("0000006010", "02", out receive);
                        FM12XX_Card.TransceiveCT("FF00817E000000000000000000000000", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004006010", "02", out receive);
                        if (receive != "04FF00817E0000000000000000000000009000")
                        {
                            DisplayMessageLine("初始化block18失败！receive: " + receive);
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0000006410", "02", out receive);
                        FM12XX_Card.TransceiveCT("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004006410", "02", out receive);
                        if (receive != "04FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9000")
                        {
                            DisplayMessageLine("初始化block19失败！receive: " + receive);
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0000006810", "02", out receive);
                        FM12XX_Card.TransceiveCT("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004006810", "02", out receive);
                        if (receive != "04FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9000")
                        {
                            DisplayMessageLine("初始化block1A失败！receive: " + receive);
                            return;
                        }
                        FM12XX_Card.Init_TDA8007(out display);
                        DisplayMessageLine("接触开始下载COS_patch……");
                    }
                    else
                    {
                        FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                        FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                        temp_all = strToHexByte(receive.Substring(2, 64));
                        FM12XX_Card.Init_TDA8007(out display);
                        DisplayMessageLine("接触开始下载COS……");
                    }
                }
                if (bank326.Checked == true)
                {
                    strReceived = ADcos.Text;
                    eeprogram(strReceived, ITflag);
                }
                else
                {
                    if ((temp_all[2] & 0x40) != 0) //CW2-bit6 正反码校验
                        zfcheck = true;
                    else
                        zfcheck = false;
                    program(ADcos.Text, ITflag, zfcheck, temp_all[13]);
                }
            }

            if (rston.Checked)
            {
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                {
                    DisplayMessageLine("COS无法运行--停止!");
                    return;
                }
                FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                if (receive != "9000")
                {
                    DisplayMessageLine("初始化指令失效，未开启复位--停止 ");
                    return;
                }
                FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                FM12XX_Card.TransceiveCT("0004000605", "02", out receive);
                temp_uid = strToHexByte(receive.Substring(2, 10));
                FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                temp_all = strToHexByte(receive.Substring(2, 64));
                temp_all[7] = temp_uid[0]; //EEPROM 密钥1
                temp_all[8] = temp_uid[1]; //EEPROM 密钥2
                if (bright.Checked)
                {
                    //-----开启数字sensor复位---------    
                    temp_all[3] |= 0x80;
                    //-----开启安全复位---------
                    temp_all[26] = 0xFF;
                }
                if (bank.Checked || zc.Checked)
                {
                    //-----开启模拟和数字sensor复位---------    
                    temp_all[3] |= 0xC0;
                    //-----开启安全复位---------
                    temp_all[26] = 0xDF;
                }
                temp_str = byteToHexStr(32, temp_all);
                FM12XX_Card.TransceiveCT("0000000020", "02", out receive);
                FM12XX_Card.TransceiveCT(temp_str, "02", out receive);
                DisplayMessageLine("开启复位！ ");
                FM12XX_Card.Init_TDA8007(out display);
            }

            if (user_lock.Checked)
            {
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                {
                    DisplayMessageLine("COS无法运行--停止!");
                    return;
                }
                FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
                if (receive != "9000")
                {
                    DisplayMessageLine("初始化指令失效，未lock--停止 ");
                    return;
                }
                FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
                FM12XX_Card.TransceiveCT("0004000605", "02", out receive);
                temp_uid = strToHexByte(receive.Substring(2, 10));
                FM12XX_Card.TransceiveCT("0004000020", "02", out receive);
                temp_all = strToHexByte(receive.Substring(2, 64));
                temp_all[7] = temp_uid[0]; //EEPROM 密钥1
                temp_all[8] = temp_uid[1]; //EEPROM 密钥2
                if (bright.Checked)
                {
                    //----- LOCK位---------                    
                    temp_all[19] = 0x1F;
                    temp_all[20] = 0x1F;
                    temp_all[21] = 0x7A;  //class C不LOCK
                    temp_all[22] = 0xFF;
                    temp_all[23] = 0x7F;
                    temp_all[24] = 0x2F;
                }
                if (bank.Checked || zc.Checked)
                {
                    //---关闭初始化指令----                    
                    temp_all[4] &= 0xDF;
                    //----开启认证指令-----                    
                    temp_all[6] |= 0x80; 
                    //----- LOCK位---------                    
                    temp_all[19] = 0xFF;
                    temp_all[20] = 0xFF;
                    temp_all[21] = 0xFF;  
                    temp_all[22] = 0xFF;
                    temp_all[23] = 0x7F;
                    temp_all[24] = 0x6F;
                }
                    //-----用户模式---------
                    temp_all[29] = 0x5A;
                    temp_all[30] = 0xAA;
                    temp_all[31] = 0xA5;
                temp_str = byteToHexStr(32, temp_all);
                FM12XX_Card.TransceiveCT("0000000020", "02", out receive);
                FM12XX_Card.TransceiveCT(temp_str, "02", out receive);
                DisplayMessageLine("进入用户模式并开启LOCK！ ");
                FM12XX_Card.Init_TDA8007(out display);
            }
            if (checkrlt.Checked)
            {
                //---------检验------------    
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived == "Error")
                    DisplayMessageLine("接触无ATR--停止!");
                else
                    DisplayMessageLine("ATR-->  " + strReceived);
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.SetField(0, out display);
                FM12XX_Card.Active(out display);
                DisplayMessageLine("Active --> " + display);
                FM12XX_Card.RATS(out display);
                DisplayMessageLine("RATS --> " + display);
                FM12XX_Card.SetField(0, out display);
                DisplayMessageLine("--------END--------");
            }
        }

        private void skip_cfw_rnd_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void prog_cos_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void user_lock_CheckedChanged(object sender, EventArgs e)
        {

        }

        private int alarm_check(int face, out int rlta, out int rltb)
        {
            int i, j, light_cnt, vdd_cnt, evod_cnt, mem_cnt, temp_cnt, shield_cnt, freq_cnt, check_cnt, dapr_cnt, aury_cnt, selchk_cnt;
            byte[] rlt, rlt_2;

            light_cnt = 0;
            vdd_cnt = 0;
            evod_cnt = 0;
            mem_cnt = 0;
            temp_cnt = 0;
            shield_cnt = 0;
            freq_cnt = 0;
            check_cnt = 0;
            dapr_cnt = 0;
            aury_cnt = 0;
            selchk_cnt = 0;

            for (i = 0; i < 3; i++)
            {
                FM12XX_Card.SendAPDUCT("1020 F601 0B", out receive);
                rlt = strToHexByte(receive);
                if (rlt[0] == 0)
                    break;
                if ((rlt[0] & 0xF0) == 0)
                {
                    if ((rlt[0] & 0x01) == 0x01)
                        selchk_cnt++;

                }
                if ((rlt[0] & 0x04) == 0x04)
                        light_cnt++;
                if (((rlt[0] & 0x10) == 0x10))
                    temp_cnt++;
                if (((rlt[0] & 0x20) == 0x20))
                    vdd_cnt++;
                if ((rlt[0] & 0x40) == 0x40)
                    evod_cnt++;
                if (((rlt[0] & 0x80) == 0x80))
                {
                    mem_cnt++;
                    FM12XX_Card.SendAPDUCT("0051000004 00F60B 01", out receive);
                    rlt_2 = strToHexByte(receive);
                    if ((rlt_2[0] & 0x01) == 0x01)
                        aury_cnt++;
                    if ((rlt_2[0] & 0x02) == 0x01)
                        dapr_cnt++;
                }
                if ((rlt[0] & 0x08) == 0x08)
                    shield_cnt++;
                if (((rlt[0] & 0x02) == 0x02))
                    freq_cnt++;
                if (((rlt[0] & 0x01) == 0x01))
                {
                    check_cnt++;
                    FM12XX_Card.SendAPDUCT("005300000B 00F602 07 00000000000000", out receive);
                }
            }
            rlta = 0;
            rltb = 0;
            return 0;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            string  temp_str;
            


            if (ATR_check(1, out temp_str) == 1) //5V
            {
                DisplayMessageLine("接触5v  " + temp_str);
                goto p3v;
            }
            //alarm_check(int face, out int rlta, out int rlt b);
p3v:        if (ATR_check(2, out temp_str) == 1) //3V
            {
                DisplayMessageLine("接触3v  " + temp_str);
                goto pcl;
            }
pcl:

            //if (light_cnt != 0)
            //    DisplayMessageLine("接触5v -- 光报警");
            //if (temp_cnt != 0)
            //    DisplayMessageLine("接触5v -- 温度报警");
            //if (vdd_cnt != 0)
            //    DisplayMessageLine("接触5v -- 电压报警");
            //if (evod_cnt != 0)
            //    DisplayMessageLine("接触5v -- 寄存器奇偶校验报警");
            //if (mem_cnt != 0)
            //{
            //    if (dapr_cnt != 0)
            //        DisplayMessageLine("接触5v -- 读出数据奇偶校验报警");
            //    if (aury_cnt != 0)
            //        DisplayMessageLine("接触5v -- 访问权限报警");
            //}
            //if (shield_cnt != 0)
            //    DisplayMessageLine("接触5v -- 金属屏蔽层报警");
            //if (freq_cnt != 0)
            //    DisplayMessageLine("接触5v -- 频率检测报警");
            //if (check_cnt > 1)
            //    DisplayMessageLine("接触5v -- 自检测报警");
            FM12XX_Card.Init_TDA8007(out display);
            DisplayMessageLine("-------------end--------------- ");
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string temp_str;
            byte[] tmp;
            
            FM12XX_Card.SetField(0, out display);


            if (ATR_check(1, out temp_str) == 1) //5V
            {
                
                DisplayMessageLine("接触5v  " + temp_str);
                goto p3v;
                
            }
            FM12XX_Card.SendAPDUCT("1020F60101 ", out receive);
            tmp = strToHexByte(receive);
            if (tmp[0] != 0)
            {
                DisplayMessageLine("接触5v读寄存器失败！");
                DisplayMessageLine("-------------end--------------- ");
                return;
            }
p3v:       if (ATR_check(2, out temp_str) == 1) //3V
            {
                
                DisplayMessageLine("接触3v  " + temp_str);
                goto pCL;
                
            }
            FM12XX_Card.SendAPDUCT("1020F60101 ", out receive);
            tmp = strToHexByte(receive);
            if (tmp[0] != 0)
            {
                DisplayMessageLine("接触3v读寄存器失败！");
                DisplayMessageLine("-------------end--------------- ");
                return;
            }
pCL:        FM12XX_Card.Init_TDA8007(out display);
            if(ACTIVE_check(out temp_str) == 1)
                DisplayMessageLine(temp_str);
            DisplayMessageLine("接触5v/3v和非接触都能正常工作！ ");
            DisplayMessageLine("-------------end--------------- ");
            return;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int j, rel_l, rel_h;
            int[] temp_v = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] thy_l = { 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] thy_h = { 0, 0, 0, 0, 0, 0, 0, 0 };
            string str_UID, str_ctrl, str10, str11, str12, strl ="", strh="";            
            byte[] tmp;

            try
            {
                FM12XX_Card.SetField(0, out display);
                FM12XX_Card.REQA(out display);
                FM12XX_Card.AntiColl_lv(1, out str_UID);
                FM12XX_Card.Select(1, out display);
                FM12XX_Card.SendReceiveCL("32A0", out display);
                FM12XX_Card.ReadBlock("00", out str_ctrl);
                FM12XX_Card.ReadBlock("10", out str10);
                FM12XX_Card.ReadBlock("11", out str11);
                FM12XX_Card.ReadBlock("12", out str12);
                FM12XX_Card.SetField(0, out display);
                tmp = strToHexByte(str_ctrl);
                tmp[3] &= 0x3F;
                rel_h = tmp[3] / 8;
                rel_l = tmp[3] % 8;
                switch (rel_h)
                {
                    case 0: strh = "000";
                        break;
                    case 1: strh = "001";
                        break;
                    case 2: strh = "010";
                        break;
                    case 3: strh = "011";
                        break;
                    case 4: strh = "100";
                        break;
                    case 5: strh = "101";
                        break;
                    case 6: strh = "110";
                        break;
                    case 7: strh = "111";
                        break;
                }
                switch (rel_l)
                {
                    case 0: strl = "000";
                        break;
                    case 1: strl = "001";
                        break;
                    case 2: strl = "010";
                        break;
                    case 3: strl = "011";
                        break;
                    case 4: strl = "100";
                        break;
                    case 5: strl = "101";
                        break;
                    case 6: strl = "110";
                        break;
                    case 7: strl = "111";
                        break;
                }
                tmp = strToHexByte(str10 + str11 + str12);
                for (j = 0; j < 24; j++)
                    temp_v[j] = tmp[j * 2] * 256 + tmp[j * 2 + 1];
                for (j = 0; j < 8; j++)
                {
                    thy_l[j] = 25 - ((temp_v[3 * j + 1] - temp_v[3 * j]) / 1.91);
                    thy_h[j] = 25 - ((temp_v[3 * j + 2] - temp_v[3 * j]) / 1.96);
                }
                
                StreamWriter memo = new StreamWriter(".\\Vbe_Vfl_Vfh.txt", true);    //打开记录文档
                memo.WriteLine("#-------UID --> " + str_UID + "-------#");            		//新增表头
                memo.WriteLine("#---trim----Vbe----Vref_l----Vref_h----理论值L----理论值H----#"); //新增表头
                memo.WriteLine("    000     " + temp_v[0].ToString() + "     " + temp_v[1].ToString() + "       " + temp_v[2].ToString() + "      " + thy_l[0].ToString("f1") + "       " + thy_h[0].ToString("f1"));
                memo.WriteLine("    001     " + temp_v[3].ToString() + "     " + temp_v[4].ToString() + "       " + temp_v[5].ToString() + "      " + thy_l[1].ToString("f1") + "       " + thy_h[1].ToString("f1"));
                memo.WriteLine("    010     " + temp_v[6].ToString() + "     " + temp_v[7].ToString() + "       " + temp_v[8].ToString() + "      " + thy_l[2].ToString("f1") + "       " + thy_h[2].ToString("f1"));
                memo.WriteLine("    011     " + temp_v[9].ToString() + "     " + temp_v[10].ToString() + "       " + temp_v[11].ToString() + "      " + thy_l[3].ToString("f1") + "       " + thy_h[3].ToString("f1"));
                memo.WriteLine("    100     " + temp_v[12].ToString() + "     " + temp_v[13].ToString() + "       " + temp_v[14].ToString() + "      " + thy_l[4].ToString("f1") + "       " + thy_h[4].ToString("f1"));
                memo.WriteLine("    101     " + temp_v[15].ToString() + "     " + temp_v[16].ToString() + "       " + temp_v[17].ToString() + "      " + thy_l[5].ToString("f1") + "       " + thy_h[5].ToString("f1"));
                memo.WriteLine("    110     " + temp_v[18].ToString() + "     " + temp_v[19].ToString() + "       " + temp_v[20].ToString() + "      " + thy_l[6].ToString("f1") + "       " + thy_h[6].ToString("f1"));
                memo.WriteLine("    111     " + temp_v[21].ToString() + "     " + temp_v[22].ToString() + "       " + temp_v[23].ToString() + "      " + thy_l[7].ToString("f1") + "       " + thy_h[7].ToString("f1"));
                memo.WriteLine("#---实测L----实测H----#");
                memo.WriteLine("     " + strl + "     " + strh);
                memo.Close();
                DisplayMessageLine("-------result in Vbe_Vfl_Vfh.txt-------");
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void progEEdataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int i, j, len;
            int nRowData;
            String strProgData;

            nRowData = progEEdataGridView.RowCount;
            if (nRowData == 0) return;
            ProgFileLenth = nRowData * 16;
            if (ProgFileLenth >= 1024 * 610)
            {
                display = "数据超出范围……";
                DisplayMessageLine(display);
                return;
            }

            for (i = 0; i < nRowData; i++)
            {
                if (progEEdataGridView["EE_Data", i].Value != null)
                {
                    strProgData = progEEdataGridView["EE_Data", i].Value.ToString();
                    strProgData = DeleteSpaceString(strProgData);
                    len = strProgData.Length;
                    if (len >= 0x20) len = 0x20;
                    for (j = 0; j < len / 2; j++)
                    {
                        Progfilebuf[i * 0x10 + j] = (byte)Convert.ToInt32(strProgData.Substring(j * 2, 2), 16);
                    }
                    for (; j < 0x10; j++)
                    {
                        Progfilebuf[i * 0x10 + j] = 0;
                    }
                }
                else
                {
                    len = 0;
                    for (j = 0; j < 0x10; j++)
                    {
                        Progfilebuf[i * 0x10 + j] = 0;
                    }
                }
            }
            ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
        }

        private void MI_Deselect_Click(object sender, EventArgs e)
        {
            string data = "CA0" + textBox_CID.Text;
            string crc_cfg = "01"; // all crc
            string timeout = TimeOut_textBox.Text;
            if (timeout.Length < 2)
            {
                timeout = "0" + timeout;
                TimeOut_textBox.Text = timeout;
            }
            try
            {
                DisplayMessageLine("Data Send:    \t->\t" + data);
                int i = FM12XX_Card.TransceiveCL(data, crc_cfg, timeout, out receive);

                display = "Data Received:\t<-\t" + receive;

                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void IncValue_Click(object sender, EventArgs e)
        {
            string block_addr, IncData, StrReceived = "";
            display = "";

            while (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
            block_addr = BlockAddr_textBox.Text;
            IncData = DeleteSpaceString(SendData_textBox.Text);
            try
            {
                int i = FM12XX_Card.INCVAL(block_addr, IncData, out StrReceived);

                display = "IncValue " + IncData + ":\t\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void DecValue_Click(object sender, EventArgs e)
        {
            string block_addr, DecData, StrReceived = "";
            display = "";

            while (BlockAddr_textBox.Text.Length < 2)
            {
                BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
            }
            block_addr = BlockAddr_textBox.Text;
            DecData = DeleteSpaceString(SendData_textBox.Text);
            try
            {
                int i = FM12XX_Card.DECVAL(block_addr, DecData, out StrReceived);

                display = "IncValue " + DecData + ":\t\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void textBox_last_SAK_TextChanged(object sender, EventArgs e)
        {
            if (textBox_last_SAK.Text.Length == 2)
            {
                textBox_last_SAK.Focus();
                FM12XX_Card.last_SAK = textBox_last_SAK.Text;
            }
        }

        private void checkBox_check_BCC_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_check_BCC.Checked == true)
            {
                FM12XX_Card.check_BCC = 1;
            }
            else
            {
                FM12XX_Card.check_BCC = 0;
            }
        }

        private void ProgEEEndAddr_Leave(object sender, EventArgs e)
        {
            while (ProgEEEndAddr.Text.Length < 6)
            {
                ProgEEEndAddr.Text = "0" + ProgEEEndAddr.Text;
            }
        }
        private void ProgEEEndAddr_TextChanged(object sender, EventArgs e)
        {
            if (ProgEEEndAddr.Text.Length == 6)
            {
                ProgEE_Button.Focus();
            }
        }

        private void ProgEEOffsetAddr_TextChanged(object sender, EventArgs e)
        {
            if (ProgEEOffsetAddr.Text.Length == 6)
            {
                ProgEE_Button.Focus();
            }
        }
        private void ProgEEOffsetAddr_Leave(object sender, EventArgs e)
        {
            while (ProgEEOffsetAddr.Text.Length < 6)
            {
                ProgEEOffsetAddr.Text = "0" + ProgEEOffsetAddr.Text;
            }
        }

        private void ProgDestAddr_TextChanged(object sender, EventArgs e)
        {
            if (ProgDestAddr.Text.Length == 6)
            {
                ProgEE_Button.Focus();
            }
        }
        private void ProgDestAddr_Leave(object sender, EventArgs e)
        {
            while (ProgDestAddr.Text.Length < 6)
            {
                ProgDestAddr.Text = "0" + ProgDestAddr.Text;
            }
        }

        private void ProgEEStartAddr_TextChanged(object sender, EventArgs e)
        {
            if (ProgEEStartAddr.Text.Length == 6)
            {
                ProgEE_Button.Focus();
            }
        }
        private void ProgEEStartAddr_Leave(object sender, EventArgs e)
        {
            while (ProgEEStartAddr.Text.Length < 6)
            {
                ProgEEStartAddr.Text = "0" + ProgEEStartAddr.Text;
            }
        }

        private void checkBox_last_SAK_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_last_SAK.Checked)
            {
                textBox_last_SAK.Enabled = true;
                FM12XX_Card.enable_SAK = 1;
            }
            else
            {
                textBox_last_SAK.Enabled = false;
                FM12XX_Card.enable_SAK = 0;
            }
        }

        private void FM350_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (FM350_radioButton.Checked == true)
            {
                checkBox_350ChipErase.Visible = true;
                checkBox_350ChipErase.Enabled = true;
                DisplayMessageLine("若编程FM350请保证起始地址512bytes对齐！");
                checkBox_SeperateLibProg.Visible = true;
                groupBox7.Visible = false;
                groupBox_350_Cfg.Visible = true;
                textBox_LibSize.Visible = true;
                label_LibSizeUnit.Visible = true;
                if (PROG_EEtype_radiobutton.Checked == true)
                    checkBox_SeperateLibProg.Enabled = true;
                else
                    checkBox_SeperateLibProg.Enabled = false;
            }
            else
            {
                checkBox_350ChipErase.Visible = false;
                checkBox_350ChipErase.Enabled = false;
                checkBox_SeperateLibProg.Visible = false;
                checkBox_SeperateLibProg.Enabled = false;
                groupBox7.Visible = true;
                groupBox_350_Cfg.Visible = false;
                textBox_LibSize.Visible = false;
                textBox_LibSize.Enabled = false;
                label_LibSizeUnit.Visible = false;
            }
        }

        private void comboBox_ChipName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox_ChipName.Text)
                {
                    case "FM309":
                        DisplayMessageLine("已选择FM309芯片");
                        FM309_radioButton.Checked = true;
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM326":
                        DisplayMessageLine("已选择FM326芯片");
                        FM326_radioButton.Checked = true;
                        ProgEEStartAddr.Text = "000000";
                        ProgEEEndAddr.Text = "009FFF";
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM336":
                        DisplayMessageLine("已选择FM336芯片");
                        FM336_radioButton.Checked = true;
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM349":
                        DisplayMessageLine("已选择FM349芯片");
                        FM349_radioButton.Checked = true;
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM350":
                        DisplayMessageLine("已选择FM350芯片");
                        FM350_radioButton.Checked = true;
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM294":
                        DisplayMessageLine("已选择FM294芯片");
                        FM294_radioButton.Checked = true;
                        ProgEEStartAddr.Text = "000000";
                        ProgEEEndAddr.Text = "007FFF";
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM274":
                        DisplayMessageLine("已选择FM274芯片");
                        FM274_radioButton.Checked = true;
                        ProgEEStartAddr.Text = "000000";
                        ProgEEEndAddr.Text = "003FFF";
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM302":
                        DisplayMessageLine("已选择FM302芯片");
                        FM302_radioButton.Checked = true;
                        ProgEEStartAddr.Text = "000000";
                        ProgEEEndAddr.Text = "001FFF";
                        A1Program.Checked = false;
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                    case "FM347":
                        DisplayMessageLine("已选择FM347芯片");
                        FM347_radioButton.Checked = true;
                        CT_Interface.Checked = true;
                        checkBox_CTHighBaud.Checked = true;
                        sec_addr.Visible = true;
                        retry.Visible = true;
                        groupBox_350_Cfg.Visible = true;
                        groupBox7.Visible = false;
                        textBox_terase.Text = "32";
                        textBox_tprog.Text = "47";
                        pre_prog.Visible = true;
                        CB_NoErase.Visible = true;
                        break;
                    default:        //FM309 chosen by default
                        DisplayMessageLine("已选择FM309芯片");
                        FM309_radioButton.Checked = true;
                        sec_addr.Visible = false;
                        retry.Visible = false;
                        groupBox_350_Cfg.Visible = false;
                        groupBox7.Visible = true;
                        textBox_terase.Text = "10";
                        textBox_tprog.Text = "1C";
                        pre_prog.Visible = false;
                        CB_NoErase.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void checkBox_CTHighBaud_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_CTHighBaud.Checked == true)
            {
                comboBox_CTBaud.Enabled = true;
            }
            else
            {
                comboBox_CTBaud.Enabled = false;
            }
        }

        private void fuload_CheckedChanged(object sender, EventArgs e)
        {
            cfwconfig.Checked = true;
            cfw_rnd.Checked = true;
            cfw_agn.Checked = false;
            prog_cos.Checked = true;
            rston.Checked = true;
            user_lock.Checked = true;
            checkrlt.Checked = true;
            cfgroup.Enabled = false;

        }

        private void seload_CheckedChanged(object sender, EventArgs e)
        {
            cfgroup.Enabled = true;
            cfw_rnd.Checked = false;
            cfw_agn.Checked = true;
            rston.Checked = false;
            user_lock.Checked = false;
        }

        private void checkBox_ignore9800_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ignore9800.Checked == true)
            {
                DisplayMessageLine("INFO：带校验的编程出错将被忽略！");
            }
        }

        private void checkBox_SeperateLibProg_CheckedChanged(object sender, EventArgs e)
        {
            byte[] libsize;
            if (checkBox_SeperateLibProg.Checked == true)
            {
                DisplayMessageLine("INFO：请确认配置的LibSize和NVR配置的一致");
                HasLib350_flag = 1;
                textBox_LibSize.Enabled = true;
                label_LibSizeUnit.Enabled = true;
                libsize = strToHexByte(textBox_LibSize.Text);
                LibSizeFM350 = (uint)libsize[0] * 1024;
            }
            else
            {
                HasLib350_flag = 0;
                textBox_LibSize.Enabled = false;
                label_LibSizeUnit.Enabled = false;
                LibSizeFM350 = 0;
            }
        }

        private void FM349_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (FM349_radioButton.Checked == true)
            {
                groupBox7.Visible = false;
                groupBox_350_Cfg.Visible = true;
            }
            else
            {
                groupBox7.Visible = true;
                groupBox_350_Cfg.Visible = false;
            }
        }

        private void textBox_tprog_TextChanged(object sender, EventArgs e)
        {
            byte[] tprog;
            double tmp;
            if (FM347_radioButton.Checked == true)
            {
                if (textBox_tprog.Text.Length == 2)
                {
                    tmp = (Convert.ToUInt16(textBox_tprog.Text, 16) + 1) * 0.125;
                    label_tprog_Calc.Text = tmp.ToString("F2");
                    if (tmp > 16)
                    {
                        label_tprog_Calc.Text = 16.ToString("F2");
                        textBox_tprog.Text = "7F";
                        DisplayMessageLine("FM347最大编程时间配置为0x7F = 16us");
                    }


                    ProgEE_Button.Focus();
                }
                else if (textBox_tprog.Text.Length > 2)
                {
                    textBox_tprog.Text = "7F";
                }
            }
            else
            {
                if (textBox_tprog.Text.Length == 2)
                {
                    tprog = strToHexByte(textBox_tprog.Text);
                    FM350_tprog = 0.25 * ((double)tprog[0]);
                    if (FM350_tprog == 0)
                    {
                        textBox_tprog.Text = "01";
                    }
                    else if (FM350_tprog > 15.75)
                    {
                        textBox_tprog.Text = "3F";
                        DisplayMessageLine("FM350最大编程时间配置为0x3F = 15.75us");
                    }
                    label_tprog_Calc.Text = FM350_tprog.ToString("F2");

                    ProgEE_Button.Focus();
                }
                else if (textBox_tprog.Text.Length > 2)
                {
                    textBox_tprog.Text = "3F";
                }
            }
        }
        private void textBox_tprog_Leave(object sender, EventArgs e)
        {
            while (textBox_tprog.Text.Length < 2)
            {
                textBox_tprog.Text = "0" + textBox_tprog.Text;
            }
        }

        private void textBox_terase_TextChanged(object sender, EventArgs e)
        {
            byte[] terase;
            double tmp, revf;
            string max;

            if (FM347_radioButton.Checked == true)
            {
                if (retry.Checked == true)
                {
                    revf = 2.048;
                    max = "1F";
                }
                else
                {
                    revf = 8.192;
                    max = "7F";
                }
                if (textBox_terase.Text.Length == 2)
                {
                    tmp = (Convert.ToUInt16(textBox_terase.Text, 16) + 1) * 0.064;
                    label_terase_Calc.Text = tmp.ToString("F3");
                    if (tmp > revf)
                    {
                        label_terase_Calc.Text = revf.ToString("F3");
                        textBox_terase.Text = max;
                        DisplayMessageLine("FM347最大擦除时间配置为" + max + " = " + label_terase_Calc.Text + "ms");
                    }
                    ProgEE_Button.Focus();
                }
                else if (textBox_terase.Text.Length > 2)
                {
                    textBox_terase.Text = max;
                }
            }
            else
            {
                if (textBox_terase.Text.Length == 2)
                {
                    terase = strToHexByte(textBox_terase.Text);
                    FM350_terase = 0.128 * ((double)terase[0]);
                    if (FM350_terase == 0)
                    {
                        textBox_terase.Text = "01";
                    }
                    else if (FM350_terase > 8.064)
                    {
                        textBox_terase.Text = "3F";
                        DisplayMessageLine("FM350最大擦除时间配置为0x3F = 8.064ms");
                    }
                    label_terase_Calc.Text = FM350_terase.ToString("F3");

                    ProgEE_Button.Focus();
                }
                else if (textBox_terase.Text.Length > 2)
                {
                    textBox_terase.Text = "3F";
                }
            }
        }
        private void textBox_terase_Leave(object sender, EventArgs e)
        {
            while (textBox_terase.Text.Length < 2)
            {
                textBox_terase.Text = "0" + textBox_terase.Text;
            }
        }

        private void textBox_LibSize_TextChanged(object sender, EventArgs e)
        {
            byte[] libsize;
            if (textBox_LibSize.Text.Length == 2)
            {
                libsize = strToHexByte(textBox_LibSize.Text);
                if (libsize[0] > 0x7F)
                {
                    textBox_LibSize.Text = "7F";
                    DisplayMessageLine("WARN：FM350最大可配置lib区为127Kbyte");
                }
                else if (libsize[0] == 0x00)
                {
                    //lib段长度强制设置为大于0
                    textBox_LibSize.Text = "01";
                    DisplayMessageLine("WARN：勾选以后，lib区大小不能为0");
                }
                else
                {
                    libsize = strToHexByte(textBox_LibSize.Text);
                    LibSizeFM350 = (uint)libsize[0] * 1024;     //get lib size
                }

                ProgEE_Button.Focus();
            }
            else if (textBox_terase.Text.Length > 2)
            {
                textBox_terase.Text = "7F";
                DisplayMessageLine("WARN：FM350最大可配置lib区为127Kbyte");
                textBox_LibSize.Text = "7F";
            }
        }

        private void textBox_LibSize_Leave(object sender, EventArgs e)
        {
            while (textBox_LibSize.Text.Length < 2)
            {
                textBox_LibSize.Text = "0" + textBox_LibSize.Text;
            }
        }

        private void textBox_CID_TextChanged(object sender, EventArgs e)
        {
            if (textBox_CID.Text.Length == 1)
                button_RATS.Focus();
        }

        /* TYPE B control buttons */
        private void GUI_ParseATQB(string ATQB)
        {
            FM12XX_Card.ParseATQB(ATQB);
        }

        private void button_REQB_Click(object sender, EventArgs e)
        {
            string strReceived, PARAM, ExtREQB;
            try
            {
                FM12XX_Card.PUPI = "00000000";
                if (checkBox_ExtREQB.Checked == true)
                {
                    ExtREQB = "1";
                    FM12XX_Card.ExtREQBSupported = 1;
                }
                else
                {
                    ExtREQB = "0";
                    FM12XX_Card.ExtREQBSupported = 0;
                }
                switch (comboBox_N.Text)
                {
                    case "1":
                        PARAM = "0";   //N = 1;
                        break;
                    case "2":
                        PARAM = "1";
                        break;
                    case "4":
                        PARAM = "2";
                        break;
                    case "8":
                        PARAM = "3";
                        break;
                    case "16":
                        PARAM = "4";
                        break;
                    default:
                        PARAM = "0";
                        break;
                }
                PARAM = ExtREQB + PARAM;
                FM12XX_Card.AFI = textBox_AFI.Text;

                DisplayMessageLine("REQB:\t\t\t<-\t05" + textBox_AFI.Text + PARAM);      //debug purpose

                FM12XX_Card.REQB(textBox_AFI.Text, PARAM, out strReceived);     //REQB send
                if (strReceived == "Error")
                {
                    DisplayMessageLine("ERROR: REQB Failed！");
                }
                else
                {
                    DisplayMessageLine("ATQB:\t\t\t->\t" + strReceived);
                    if ((strReceived.Length == 24) || (strReceived.Length == 26))
                    {
                        FM12XX_Card.ParseATQB(strReceived);
                        textBox_PUPI.Text = FM12XX_Card.PUPI;
                        display = "INFO:\tPUPI:\t\t\t" + textBox_PUPI.Text + "\n";
                        display += "\t\tApplication Data:\t" + FM12XX_Card.AppData + "\n";
                        display += "\t\tProtocol Info:\t" + FM12XX_Card.ProtocolInfo;
                        DisplayMessageLine(display);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void button_WUPB_Click(object sender, EventArgs e)
        {
            string strReceived, PARAM, ExtREQB;
            try
            {
                if (checkBox_ExtREQB.Checked == true)
                {
                    ExtREQB = "1";
                    FM12XX_Card.ExtREQBSupported = 1;
                }
                else
                {
                    ExtREQB = "0";
                    FM12XX_Card.ExtREQBSupported = 0;
                }
                switch (comboBox_N.Text)
                {
                    case "1":
                        PARAM = "8";   //N = 1;
                        break;
                    case "2":
                        PARAM = "9";
                        break;
                    case "4":
                        PARAM = "A";
                        break;
                    case "8":
                        PARAM = "B";
                        break;
                    case "16":
                        PARAM = "C";
                        break;
                    default:
                        PARAM = "8";
                        break;
                }
                PARAM = ExtREQB + PARAM;
                FM12XX_Card.AFI = textBox_AFI.Text;

                DisplayMessageLine("WUPB:\t\t\t\t05" + textBox_AFI.Text + PARAM);      //debug purpose
                FM12XX_Card.REQB(textBox_AFI.Text, PARAM, out strReceived);     //REQB send
                if (strReceived == "Error")
                {
                    display = "ERROR: WUPB Failed！";
                    DisplayMessageLine(display);
                }
                else if (strReceived == "NO ATQB")
                {
                    DisplayMessageLine("ERROR:\t\t\t\t" + strReceived);
                }
                else
                {
                    display = "ATQB:\t\t\t\t" + strReceived;
                    DisplayMessageLine(display);
                    GUI_ParseATQB(strReceived);
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void textBox_AFI_TextChanged(object sender, EventArgs e)
        {
            if (textBox_AFI.Text.Length == 2)
            {
                button_REQB.Focus();
            }
        }

        private void textBox_AFI_Leave(object sender, EventArgs e)
        {
            while (textBox_AFI.Text.Length < 2)
            {
                textBox_AFI.Text = "0" + textBox_AFI.Text;
            }
        }

        private void textBox_SlotNumber_TextChanged(object sender, EventArgs e)
        {
            if (textBox_SlotNumber.Text.Length == 1)
                button_SlotMARKER.Focus();
        }

        private void button_SlotMARKER_Click(object sender, EventArgs e)
        {
            string StrReceived;
            FM12XX_Card.PUPI = "00000000";
            DisplayMessageLine("Slot-MARKER:\t\t\t" + textBox_SlotNumber.Text+"5");
            FM12XX_Card.SlotMARKER(textBox_SlotNumber.Text, out StrReceived);
            if (StrReceived == "Error")
            {
                display = "ERROR:\t\t\t\tConfigure FM17XX register failed!";
                DisplayMessageLine(display);
            }
            else
            {
                display = "ATQB:\t\t\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
        }

        private void button_ActiveTypeB_Click(object sender, EventArgs e)
        {
            string StrReceived;

        }

        private void button_ATTRIB_Click(object sender, EventArgs e)
        {
            string StrReceived;

            DisplayMessageLine("ATTRIB:\t\t\t" + "1D" + textBox_PUPI.Text + textBox_Parameters.Text);
            FM12XX_Card.ATTRIB(textBox_PUPI.Text, textBox_Parameters.Text, out StrReceived);
            if (StrReceived == "Error")
            {
                display = "ERROR:\t\t\t\tConfigure FM17XX register failed!";
                DisplayMessageLine(display);
            }
            else
            {
                display = "Answer to ATTRIB:\t\t" + StrReceived;
                DisplayMessageLine(display);
            }

        }

        private void textBox_AFI_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void button_HLTB_Click(object sender, EventArgs e)
        {
            string StrReceived;
            DisplayMessageLine("HLTB:\t\t\t\t"+"50" + textBox_PUPI.Text);
            FM12XX_Card.HLTB(textBox_PUPI.Text, out StrReceived);
            if (StrReceived == "Error")
            {
                display = "ERROR:\t\t\t\tConfigure FM17XX register failed!";
                DisplayMessageLine(display);
            }
            else
            {
                if (StrReceived == "00")
                {
                    DisplayMessageLine("HALT Card[" + textBox_PUPI.Text + "]:\t\tSucceeded!");
                }
                else
                {
                    DisplayMessageLine("HALT Card[" + textBox_PUPI.Text + "]:\t\tFailed!(" + StrReceived + ")");
                }
            }
        }

        private void textBox_PUPI_TextChanged(object sender, EventArgs e)
        {
            if (textBox_PUPI.Text.Length == 8)
            {
                button_ATTRIB.Focus();
            }
        }

        private void textBox_PUPI_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void textBox_Parameters_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void textBox_Parameters_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Parameters.Text.Length == 8)
            {
                button_ATTRIB.Focus();
            }
        }

        private void textBox_PUPI_Leave(object sender, EventArgs e)
        {
            while (textBox_PUPI.Text.Length < 8)
            {
                textBox_PUPI.Text = textBox_PUPI.Text + "0";
            }
        }

        private void textBox_Parameters_Leave(object sender, EventArgs e)
        {
            while (textBox_Parameters.Text.Length < 8)
            {
                textBox_Parameters.Text = textBox_Parameters.Text + "0";
            }
        }

        private void button_Deselect_CLB_Click(object sender, EventArgs e)
        {
            string data = "CA0" + textBox_Parameters.Text.Substring(7, 1);
            string crc_cfg = "01"; // all crc
            string timeout = TimeOut_textBox.Text;
            if (timeout.Length < 2)
            {
                timeout = "0" + timeout;
                TimeOut_textBox.Text = timeout;
            }
            try
            {
                DisplayMessageLine("Data Send:    \t->\t" + data);
                int i = FM12XX_Card.TransceiveCL(data, crc_cfg, timeout, out receive);

                display = "Data Received:\t<-\t" + receive;

                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void button_PPS_CLB_Click(object sender, EventArgs e)
        {
            string pps1;
            int pps1_DSI, pps1_DRI;
            try
            {
                /*if (PPS_exchange_cl_cbox.Text == "106K")
                    pps1 = "11";//---106K
                else if (PPS_exchange_cl_cbox.Text == "212K")
                    pps1 = "13";// ---- 212kbps
                else if (PPS_exchange_cl_cbox.Text == "424K")
                    pps1 = "94";// ---- 424kbps
                else if (PPS_exchange_cl_cbox.Text == "848K")
                    pps1 = "18";// ---- 848kbps
                else
                    pps1 = "11";*/

                pps1_DSI = strToHexByte(textBox_DSI_CLB.Text)[0];
                pps1_DRI = strToHexByte(textBox_DRI_CLB.Text)[0];
                pps1 = ((pps1_DSI << 2) + pps1_DRI).ToString("X2");

                FM12XX_Card.PPS_CL(pps1, out display);

                display = "PPS Response:\t\t\t" + display;

                DisplayMessageLine(display);

            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void textBox_CRC_TextChanged(object sender, EventArgs e)
        {
            if (textBox_CRC.Text.Length == 4)
            {
                button_CRC.Focus();
            }
        }

        private void textBox_CRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyFilter(e);
        }

        private void textBox_CRC_Leave(object sender, EventArgs e)
        {
            while (textBox_CRC.Text.Length < 4)
                textBox_CRC.Text = "0" + textBox_CRC.Text;
        }


        private void button_CRC_Click(object sender, EventArgs e)
        {
            try
            {
                string strCRC_type, strCRC16_result;
                int CRCType;

                if (textBox_CRC.Text == "6363")
                {
                    strCRC_type = "A";
                    CRCType = 1;
                }
                else if ((textBox_CRC.Text == "FFFF") || (textBox_CRC.Text == "ffff"))
                {
                    strCRC_type = "B";
                    CRCType = 2;
                }
                else
                {
                    strCRC_type = "unknown";
                    CRCType = 3;
                }
                FM12XX_Card.CRC16(CRCType, textBox_CRC.Text, SendData_textBox.Text, out strCRC16_result);
                DisplayMessageLine("CRC " + strCRC_type + " result: " + strCRC16_result);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                return;
            }

        }

        private void Field_ON_MouseEnter(object sender, EventArgs e)
        {
            string display;
            display = "可以通过修改0x12寄存器来改变发射脚TX1和TX2发射天线的电导，达到修改场强的目的。";
            display += "\nReg: CwConductance\nAddr: 0x12\nBIT0~3: trim level(如果是0关场)\nBIT4~5: cfg level\nBIT6~7: RFU";
            this.toolTip_CLIterface.SetToolTip(this.Field_ON, display);
        }

        private void Field_ON_MouseLeave(object sender, EventArgs e)
        {
            this.toolTip_CLIterface.RemoveAll();
        }

        private void button_TypeBFraming_Click(object sender, EventArgs e)
        {
            string StrReceived, msg;
            msg = "";
            FM12XX_Card.enSOF_n = (checkBox_CLB_SOF.Checked == true) ? 0 : 1;
            FM12XX_Card.enEOF_n = (checkBox_CLB_EOF.Checked == true) ? 0 : 1;
            FM12XX_Card.EOF_type = comboBox_CLB_EOF.SelectedIndex;
            FM12XX_Card.SOF_type = comboBox_CLB_SOF.SelectedIndex;
            FM12XX_Card.EGT = (int)strToHexByte(textBox_EGT.Text)[0];

            FM12XX_Card.TypeBFraming(FM12XX_Card.enSOF_n, FM12XX_Card.enEOF_n, FM12XX_Card.EOF_type, FM12XX_Card.EGT, FM12XX_Card.SOF_type, out StrReceived);
            if (StrReceived == "Succeeded")
            {
                if (checkBox_CLB_SOF.Checked == true)
                    msg += ("Has SOF: " + comboBox_CLB_SOF.SelectedItem + ";\t");
                if (checkBox_CLB_EOF.Checked == true)
                    msg += ("Has EOF: " + comboBox_CLB_SOF.SelectedItem + ";\t");
                msg += ("EGT: " + FM12XX_Card.EGT + " etus");
                DisplayMessageLine("Set PCD Type B frame format:\t" + msg);
            }
            else if (StrReceived == "Error")
            {
                DisplayMessageLine("Set PCD Type B frame format:\tFailed");
            }
            FM12XX_Card.TypeBFrameFlag = false;
        }

        private void textBox_EGT_TextChanged(object sender, EventArgs e)
        {
            if (textBox_EGT.Text.Length == 1)
            {
                if ((int)strToHexByte(textBox_EGT.Text)[0] > 7)
                {
                    textBox_EGT.Text = "7";
                }
                button_TypeBFraming.Focus();
            }
        }

        private void button_TypeBFraming_MouseEnter(object sender, EventArgs e)
        {
            display = "配置FM17的0x17寄存器来改变Type B Frame。";
            this.toolTip_CLIterface.SetToolTip(this.button_TypeBFraming, display);
        }

        private void button_TypeBFraming_MouseLeave(object sender, EventArgs e)
        {
            this.toolTip_CLIterface.RemoveAll();
        }

        private void button_CRC_MouseEnter(object sender, EventArgs e)
        {
            display = "1.填入CRC16初始值(0x6363——CRC_A，0xFFFF——CRC_B)，2.在下列数据框内输入数据，3.点击按钮\n";
            this.toolTip_CLIterface.SetToolTip(this.button_CRC, display);
        }

        private void button_CRC_MouseLeave(object sender, EventArgs e)
        {
            this.toolTip_CLIterface.RemoveAll();
        }

        private void button_AppendCRC16_Click(object sender, EventArgs e)
        {
            try
            {
                string strCRC16_result;//strCRC_type,
                int CRCType;

                if (textBox_CRC.Text == "6363")
                {
                    //strCRC_type = "A";
                    CRCType = 1;
                }
                else if ((textBox_CRC.Text == "FFFF") || (textBox_CRC.Text == "ffff"))
                {
                    //strCRC_type = "B";
                    CRCType = 2;
                }
                else
                {
                    //strCRC_type = "unknown";
                    CRCType = 3;
                }
                FM12XX_Card.CRC16(CRCType, textBox_CRC.Text, SendData_textBox.Text, out strCRC16_result);
                SendData_textBox.Text += strCRC16_result;
                //DisplayMessageLine("CRC " + strCRC_type + " result: " + strCRC16_result);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                return;
            }
        }

        private void checkBox_CLB_SOF_CheckedChanged(object sender, EventArgs e)
        {
            FM12XX_Card.TypeBFrameFlag = true;
        }

        private void comboBox_CLB_SOF_SelectedIndexChanged(object sender, EventArgs e)
        {
            FM12XX_Card.TypeBFrameFlag = true;
        }

        private void checkBox_CLB_EOF_CheckedChanged(object sender, EventArgs e)
        {
            FM12XX_Card.TypeBFrameFlag = true;
        }

        private void comboBox_CLB_EOF_SelectedIndexChanged(object sender, EventArgs e)
        {
            FM12XX_Card.TypeBFrameFlag = true;
        }

        private void button_ReverseOption_Click(object sender, EventArgs e)
        {
            FM349_ReverseOption();
        }

        private void CB_EnableOption_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_EnableOption.Checked == true)
                button_ReverseOption.Enabled = true;
            else
                button_ReverseOption.Enabled = false;
        }

        private void r347_CheckedChanged(object sender, EventArgs e)
        {
            Init_EEopt_02.Text = "High8addr-0D";
            Init_Rdee_04.Text = "RD-04";
            Init_Wree_00.Text = "WR_ram/nvm";
            Init_ewEEtime_03.Text = "WR-3D";
            Init_CPUStart_08.Text = "RD-efuse";
            Init_Trim_0B.Text = "54WR-3E";
            init_CRC_0A.Text = "WR-efuse";
            init_AUTH_55.Text = "ORG-54";
            CT_347_STOPMCU.Visible = true;
            text_0B_P3.Visible = true;
            text_08_P1.Visible = false;
            ins54.Visible = true;
            trim8m.Visible = true;
            trim_wr.Visible = true;
            groupBox21.Visible = true;
            btnReadUID.Visible = true;
            btnWriteUID.Visible = true;
            fm347_TypeAInfo = "";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Init_EEopt_02.Text = "EEopt 02";
            Init_Rdee_04.Text = "RDEE 04";
            Init_Wree_00.Text = "WREE 00";
            Init_ewEEtime_03.Text = "擦写EE 03";
            Init_CPUStart_08.Text = "启动cpu 08";
            Init_Trim_0B.Text = "调校 0B";
            init_CRC_0A.Text = "取CRC 0A";
            init_AUTH_55.Text = "认证 55";
            CT_347_STOPMCU.Visible = false;
            text_0B_P3.Visible = false;
            text_08_P1.Visible = true;
            ins54.Visible = false;
            trim8m.Visible = false;
            trim_wr.Visible = false;
            groupBox21.Visible = false;
            btnReadUID.Visible = false;
            btnWriteUID.Visible = false;
        }

        private void SendData_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) //cnn
            {
                if(tabControl_main.SelectedIndex == 0)               
                    TransceiveCL_Click(null, EventArgs.Empty);
                else if (tabControl_main.SelectedIndex == 1)
                    TransceiveCT_Click(null, EventArgs.Empty);
                
            }
        }

        private void buttonStringToASCII_Click(object sender, EventArgs e)
        {
            byte[] tmp;
            string ASCIIString = "";
            tmp = Encoding.ASCII.GetBytes(SendData_textBox.Text);
            foreach (byte item in tmp)
            {
                ASCIIString += item;
            }
            DisplayMessageLine("ASCII string:\t" + ASCIIString);
        }

        private void buttonASCIIToString_Click(object sender, EventArgs e)
        {
            if (SendData_textBox.Text.Length % 2 != 0)
            {
                DisplayMessageLine("Error: Invalid length, length of ASCII string should be even");
                return;
            }
            string HexString = "";
            byte[] valueArray = new byte[SendData_textBox.Text.Length / 2];

            for(int i = 0; i< SendData_textBox.Text.Length; i+=2)
            {
                valueArray[i / 2] = byte.Parse(SendData_textBox.Text.Substring(i, 2));
            }
            //foreach(int value in valueArray)
            //{
            //    HexString += (char)value;
            //    //HexString += Convert.ToChar(value);
            //}
            HexString = Encoding.ASCII.GetString(valueArray);
            DisplayMessageLine("Hex string:\t" + HexString);
        }

        private void CT_347_STOPMCU_Click(object sender, EventArgs e)
        {
            string send;

            send = SmartCardCmd.CT_347_STOPMCU;
            FM12XX_Card.SendCommand(send, out receive);
            if (receive == "AFAFAFAF")
                DisplayMessageLine("virtual ATR -- >\t" + "in 50 etu receive nothing!");
            else
                DisplayMessageLine("virtual ATR -- >\t" + receive);
        }

        private void auto_freq_test_Click(object sender, EventArgs e)
        {
            string freq_up_flag, current_cfg;
            string strReceived, CFG_WORD, CFW0_BAK, CFW2_BAK, CFW12_BAK, CFW13_BAK, CFW14_BAK, First16Byte; //备份需要修改的控制字
            int hex_addr_change;
            string[] RawCode;
            string tempStr, datacountStr, addrStr, szHex, StartEEAddr, EndEEAddr;
            byte[] bytedata;
            int addr = 0;
            int count, tmp;
            try
            {
                Reset17_Click(null, EventArgs.Empty);
                FM12XX_Card.Init_TDA8007(out strReceived);
                FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                FM12XX_Card.REQA(out display);
                if (display == "Error")
                {
                    DisplayMessageLine("REQA Error!");
                    return;
                }
                FM12XX_Card.AntiColl_lv(1, out display);
                if (display == "Error")
                {
                    DisplayMessageLine("AntiColl Error!");
                    return;
                }
                FM12XX_Card.Select(1, out display);
                if (display == "Error")
                {
                    DisplayMessageLine("Select Error!");
                    return;
                }
                i = FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                i = FM12XX_Card.ReadBlock("00", out strReceived);
                CFG_WORD = strReceived.Substring(0, 32);
                i = FM12XX_Card.ReadBlock("01", out strReceived);
                CFG_WORD = CFG_WORD + strReceived.Substring(0, 32);
                DisplayMessageLine("备份控制字： " + CFG_WORD);

                SendData_textBox.Text = CFG_WORD;
                CFW0_BAK = CFG_WORD.Substring(0, 2);   //备份需要修改的控制字
                CFW2_BAK = CFG_WORD.Substring(4, 2);
                CFW12_BAK = CFG_WORD.Substring(24, 2);
                CFW13_BAK = CFG_WORD.Substring(26, 2);
                CFW14_BAK = CFG_WORD.Substring(28, 2);
                //修改控制字                
                i = FM12XX_Card.ReadBlock("00", out First16Byte);
                First16Byte = ((strToHexByte(CFW0_BAK)[0] & 0x9F) | 0x00).ToString("X2") + First16Byte.Substring(2, 2) + "00" + First16Byte.Substring(6, 20) + ((strToHexByte(CFW13_BAK)[0] & 0x80) | 0x10).ToString("X2") + "00" + First16Byte.Substring(30, 2); //关闭安全监测以及加密奇偶校验  EEPROM 16K补丁//
                i = FM12XX_Card.WriteBlock("00", First16Byte, out strReceived);
                if (i == 1)
                {
                    display = "配置字修改 EEPROM 打补丁  失败！！";
                    DisplayMessageLine(display);
                    return;
                }
                i = FM12XX_Card.ReadBlock("00", out strReceived);
                if (i == 0)
                {
                    display = "EEPROM PATCH CFW 1:  \t<-\t" + strReceived;
                    DisplayMessageLine(display);
                }
                else
                {
                    display = "Data Received:  \t<-\t" + "ERROR";
                    DisplayMessageLine(display);
                    return;
                }
                //关各种异常复位使能 2014.10.23 zhujunhao
                i = FM12XX_Card.ReadBlock("01", out First16Byte);
                First16Byte = First16Byte.Substring(0, 20) + "00" + First16Byte.Substring(22, 10);
                i = FM12XX_Card.WriteBlock("01", First16Byte, out strReceived);
                if (i == 1)
                {
                    display = "配置字修改 EEPROM 打补丁  失败！！";
                    DisplayMessageLine(display);
                    return;
                }
                i = FM12XX_Card.ReadBlock("01", out strReceived);
                if (i == 0)
                {
                    display = "EEPROM PATCH CFW 2:  \t<-\t" + strReceived;
                    DisplayMessageLine(display);
                }
                else
                {
                    display = "Data Received:  \t<-\t" + "ERROR";
                    DisplayMessageLine(display);
                    return;
                }
                Reset17_Click(null, EventArgs.Empty);
                //下载测试COS
                //打开文件//////////////zhujunhao  2014.3.12////////
                ProgFileLenth = 0;
                open_flag = 1;
                hex_addr_change = 0;
                openFileDialog_Prog = null;
                openFileDialog_Prog = new OpenFileDialog();

                openFileDialog_Prog.InitialDirectory = HexDirectory;
                openFileDialog_Prog.Reset();
                openFileDialog_Scr.Reset();

                if (progEEdataGridView.RowCount <= 1 || dataGridView_flag == 1 || dataGridView_flag == 2)//CNN修改 
                {
                    dataGridView_flag = 0;
                    progEEdataGridView.Rows.Clear();
                }
                //openFileDialog_Prog.RestoreDirectory = true;
                //openFileDialog_Prog.FileName = ".\\FM309Cos_ROM_BCTC_V07.hex";
                openFileDialog_Prog.FileName = ".\\FM336CosBist_auto_freq_test.hex";

                //openFileDialog_Prog.FileName = ".\\FM309Cos_frq_cal_v3.hex";
                // if (openFileDialog_Prog.ShowDialog() == DialogResult.OK)
                // {                  
                HexDirectory = openFileDialog_Prog.FileName;
                //temp = HexDirectory.LastIndexOf("\\");
                //HexDirectory = HexDirectory.Substring(0, temp);
                RawCode = File.ReadAllLines(openFileDialog_Prog.FileName);
                hexfiledata = new byte[RawCode.Length];
                bin_data = new byte[RawCode.Length];


                StartEEAddr = ProgEEStartAddr.Text;
                EndEEAddr = ProgEEEndAddr.Text;
                for (i = 0; i < 6 - ProgEEStartAddr.Text.Length; i++)
                {
                    StartEEAddr = "0" + StartEEAddr;
                }
                for (i = 0; i < 6 - ProgEEEndAddr.Text.Length; i++)
                {
                    EndEEAddr = "0" + EndEEAddr;
                }
                ProgEEStartAddr.Text = StartEEAddr;
                ProgEEEndAddr.Text = EndEEAddr;

                for (i = 0; i < ProgFileMaxLen; i++)
                {
                    Progfilebuf[i] = 0x00;		//缓冲区初始化为0x00
                }
                ProgOrReadEE_FF_flag = 0;
                ProgOrReadEE_80_flag = 0;
                ProgOrReadEE_81_flag = 0;
                ProgOrReadEE_82_flag = 0;
                ProgOrReadEE_83_flag = 0;
                ProgOrReadEE_84_flag = 0;
                ProgOrReadEE_90_flag = 0;

                for (i = 0; i < RawCode.Length; i++)
                {
                    szHex = "";
                    tempStr = RawCode[i];
                    if (tempStr == "")
                    {
                        continue;
                    }
                    if (tempStr.Substring(0, 1) == ":") //判断第1字符是否是:
                    {

                        if (tempStr.Substring(1, 8) == "00000001")//数据结束
                        {
                            break;
                        }
                        if (tempStr.Substring(7, 2) == "04")//段地址
                        {
                            if (tempStr.Substring(11, 2) == "80")//段地址
                            {
                                hex_addr_change = 0x010000;
                                ProgOrReadEE_80_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "81")//段地址
                            {
                                hex_addr_change = 0x020000;
                                ProgOrReadEE_81_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "82")//段地址
                            {
                                hex_addr_change = 0x030000;
                                ProgOrReadEE_82_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "FF")//段地址
                            {
                                hex_addr_change = 0x000000;
                                ProgOrReadEE_FF_flag = 1;
                            }
                            else if (tempStr.Substring(11, 2) == "90")//LIB地址  CNN 20130410
                            {
                                hex_addr_change = 0x060000;
                                ProgOrReadEE_90_flag = 1;
                            }
                            else//段后扩展地址
                            {
                                ProgOrReadEE_EXT_flag = 1;
                                if (tempStr.Substring(11, 2) == "83")//段地址
                                {
                                    hex_addr_change = 0x040000;
                                    ProgOrReadEE_83_flag = 1;
                                }
                                else if (tempStr.Substring(11, 2) == "84")//段地址
                                {
                                    hex_addr_change = 0x050000;
                                    ProgOrReadEE_84_flag = 1;
                                }
                                /* else if (tempStr.Substring(11, 2) == "85")//段地址
                                 {
                                     hex_addr_change = 0x060000;                                    
                                 }   */
                            }
                            // addrStr = tempStr.Substring(11, 2);
                            // hex_addr_change = (int)(strToHexByte(addrStr)[0] * 65536);
                        }
                        else
                        {
                            ProgOrReadEE_FF_flag = 1;
                            datacountStr = tempStr.Substring(1, 2);//记录该行的字节个数
                            addrStr = tempStr.Substring(3, 4);//记录该行的地址
                            szHex += tempStr.Substring(9, tempStr.Length - 11); //读取数据
                            bytedata = strToHexByte(szHex);
                            addr = hex_addr_change + (int)(strToHexByte(addrStr)[1]) + (int)(strToHexByte(addrStr)[0] * 256);

                            if (addr >= ProgFileLenth)
                            {
                                ProgFileLenth = addr + strToHexByte(datacountStr)[0];
                            }
                            for (int j = 0; j < strToHexByte(datacountStr)[0]; j++)
                            {
                                Progfilebuf[addr + j] = bytedata[j];
                            }
                        }
                        // bytecount = bytecount + strToHexByte(datacountStr)[0] + addr;//记录总字节个数
                    }
                }

                ShowProgFileBuffer(ProgFileLenth, Progfilebuf);
                // }
                openFileDialog_Prog.Dispose();

                //编程/
                PROG_EEtype_radiobutton.Checked = true;
                CL_Interface.Checked = true;
                FM336_radioButton.Checked = true;
                ProgEE_Button_Click(null, null);
                //启动CPU    
                FM12XX_Card.Init_TDA8007(out strReceived);
                Reset17_Click(null, EventArgs.Empty);
                FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                FM12XX_Card.REQA(out display);
                if (display == "Error")
                {
                    DisplayMessageLine("REQA Error!");
                    return;
                }
                FM12XX_Card.AntiColl_lv(1, out display);
                if (display == "Error")
                {
                    DisplayMessageLine("AntiColl Error!");
                    return;
                }
                FM12XX_Card.Select(1, out display);
                if (display == "Error")
                {
                    DisplayMessageLine("Select Error!");
                    return;
                }
                i = FM12XX_Card.TransceiveCL("E081", "01", "09", out strReceived);
                freq_up_flag = "卡片可升频~";
                current_cfg = "升档正常！";
                for (count = 1; count < 5; count++)
                {

                    i = FM12XX_Card.TransceiveCL("0A010015F0A801", "01", "09", out strReceived);
                    freq_up_flag = strReceived.Substring(4, 2);
                    if (((int)(strToHexByte(freq_up_flag)[0] & 0x01)) == 0)
                    {
                        freq_up_flag = "卡片不能升频！";
                        break;
                    }
                    else
                    {
                        freq_up_flag = "卡片可升频~";
                    }
                }
                for (count = 1; count < 5; count++)
                {
                    i = FM12XX_Card.TransceiveCL("0B010015F41301", "01", "09", out strReceived);
                    current_cfg = strReceived.Substring(4, 2);
                    if (((int)(strToHexByte(current_cfg)[0] & 0x07)) < 2)
                    {
                        current_cfg = "档位偏低，请检查卡机场强是否过小！";
                        break;
                    }
                    else
                    {
                        current_cfg = "升档正常！";
                    }
                }
                DisplayMessageLine("升频标志及电流源档位： " + freq_up_flag + current_cfg);
                //恢复控制字

                Reset17_Click(null, EventArgs.Empty);
                FM12XX_Card.Init_TDA8007(out strReceived);
                FM12XX_Card.Set_FM1715_reg(0x26, 0x02, out display);
                FM12XX_Card.Set_FM1715_reg(0x3A, 0x05, out display);
                FM12XX_Card.REQA(out display);
                if (display == "Error")
                {
                    DisplayMessageLine("REQA Error!");
                    return;
                }
                FM12XX_Card.AntiColl_lv(1, out display);
                if (display == "Error")
                {
                    DisplayMessageLine("AntiColl Error!");
                    return;
                }
                FM12XX_Card.Select(1, out display);
                if (display == "Error")
                {
                    DisplayMessageLine("Select Error!");
                    return;
                }
                i = FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
                i = FM12XX_Card.WriteBlock("00", CFG_WORD.Substring(0, 32), out strReceived);
                i = FM12XX_Card.WriteBlock("01", CFG_WORD.Substring(32, 32), out strReceived);
                if (i == 1)
                {
                    display = "恢复控制字   失败！！";
                    DisplayMessageLine(display);
                    return;
                }
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }                                              
        }

        private void textBox_prepg_TextChanged(object sender, EventArgs e)
        {
            double tmp;

            if (textBox_prepg.Text.Length == 2)
            {
                tmp = (Convert.ToUInt16(textBox_prepg.Text, 16) + 1) * 0.125;
                label_prepg_Calc.Text = tmp.ToString("F3");
                if (tmp > 4)
                {
                    label_prepg_Calc.Text = 4.ToString("F3");
                    textBox_prepg.Text = "1F";
                    DisplayMessageLine("FM347最大擦除时间配置为 0x1F = 4us");
                }
                ProgEE_Button.Focus();
            }
            else if (textBox_prepg.Text.Length > 2)
            {
                textBox_prepg.Text = "1F";
            }
        }

        private void sec_addr_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (sec_addr.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                    PROG_EEtype_radiobutton.Checked = true;
                    break;
                case 3:
                case 4:
                case 5:
                    DATA_EEtype_radiobutton.Checked = true;
                    break;
                case 6:
                case 7:
                case 8:
                    RAMtype_radiobutton.Checked = true;
                    break;
                default:
                    PROG_EEtype_radiobutton.Checked = true;
                    break;
            }
        }

        private void bright_CheckedChanged(object sender, EventArgs e)
        {
            ADcos.Text = ".\\FM1280_EMVCo_COS_V220.hex";
        }

        private void bank_CheckedChanged(object sender, EventArgs e)
        {
            ADcos.Text = ".\\FM1280_COS_V211.hex";
        }

        private void bank326_CheckedChanged(object sender, EventArgs e)
        {
            ADcos.Text = ".\\FM326Cos_patch.hex";
        }

        private void buttonRESTORE_Click(object sender, EventArgs e)
        {
            string block_addr, StrReceived = "";
            display = "";
            try
            {
                while (BlockAddr_textBox.Text.Length < 2)
                {
                    BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
                }
                block_addr = BlockAddr_textBox.Text;
                int i = FM12XX_Card.RESTORE(block_addr,out StrReceived);

                display = "RESTORE [" + block_addr + "]:\t\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void buttonTRANSFER_Click(object sender, EventArgs e)
        {
            string block_addr, StrReceived = "";
            display = "";
            try
            {
                while (BlockAddr_textBox.Text.Length < 2)
                {
                    BlockAddr_textBox.Text = "0" + BlockAddr_textBox.Text;
                }
                block_addr = BlockAddr_textBox.Text;
                int i = FM12XX_Card.TRANSFER(block_addr, out StrReceived);

                display = "TRANSFER [" + block_addr + "]:\t\t" + StrReceived;
                DisplayMessageLine(display);
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void Cal_access_bits_button_Click(object sender, EventArgs e)
        {
            byte C3_3, C3_2, C3_1, C3_0,C2_3, C2_2, C2_1, C2_0,C1_3, C1_2, C1_1, C1_0;
            byte byte6, byte7, byte8, byte9;
            C1_0 = Convert.ToByte(C1_0_textBox.Text);
            C2_0 = Convert.ToByte(C2_0_textBox.Text);
            C3_0 = Convert.ToByte(C3_0_textBox.Text);
            C1_1 = Convert.ToByte(C1_1_textBox.Text);
            C2_1 = Convert.ToByte(C2_1_textBox.Text);
            C3_1 = Convert.ToByte(C3_1_textBox.Text);
            C1_2 = Convert.ToByte(C1_2_textBox.Text);
            C2_2 = Convert.ToByte(C2_2_textBox.Text);
            C3_2 = Convert.ToByte(C3_2_textBox.Text);
            C1_3 = Convert.ToByte(C1_3_textBox.Text);
            C2_3 = Convert.ToByte(C2_3_textBox.Text);
            C3_3 = Convert.ToByte(C3_3_textBox.Text);

            byte6 = (byte)(((~C2_3) & 0x01) << 7 | ((~C2_2) & 0x01) << 6 | ((~C2_1) & 0x01) << 5 | ((~C2_0) & 0x01) << 4 | ((~C1_3) & 0x01) << 3 | ((~C1_2) & 0x01) << 2 | ((~C1_1) & 0x01) << 1 | ((~C1_0) & 0x01));
            byte7 = (byte)((C1_3 & 0x01) << 7 | (C1_2 & 0x01) << 6 | (C1_1 & 0x01) << 5 | (C1_0 & 0x01) << 4 | ((~C3_3) & 0x01) << 3 | ((~C3_2) & 0x01) << 2 | ((~C3_1) & 0x01) << 1 | ((~C3_0) & 0x01));
            byte8 = (byte)((C3_3 & 0x01) << 7 | (C3_2 & 0x01) << 6 | (C3_1 & 0x01) << 5 | (C3_0 & 0x01) << 4 | (C2_3 & 0x01) << 3 | (C2_2 & 0x01) << 2 | (C2_1 & 0x01) << 1 | (C2_0 & 0x01));
            byte9 = 0x69;
           // DisplayMessageLine("生成的M1权限字为: " + byte6.ToString("X2") + byte7.ToString("X2") + byte8.ToString("X2") + byte9.ToString("X2"));
            Access_bits_textBox.Text = byte6.ToString("X2") + byte7.ToString("X2") + byte8.ToString("X2") + byte9.ToString("X2");
        }

        private void parse_access_bits_button_Click(object sender, EventArgs e)
        {
            byte byte6, byte7, byte8, byte9;
            byte[] access_byte_buf = strToHexByte(Access_bits_textBox.Text);
            
            byte6=access_byte_buf[0];
            byte7=access_byte_buf[1];
            byte8=access_byte_buf[2];
            byte9=access_byte_buf[3];

            if (((~byte7 >> 4) & 0x0F) != (byte6 & 0x0F))
            {
                DisplayMessageLine("待解析数据（C1）正反码输入错误！！");
                return;
            }
            if (((~byte6 >> 4) & 0x0F) != (byte8 & 0x0F))
            {
                DisplayMessageLine("待解析数据（C2）正反码输入错误！！");
                return;
            }
            if (((~byte8 >> 4) & 0x0F) != (byte7 & 0x0F))
            {
                DisplayMessageLine("待解析数据（C3）正反码输入错误！！");
                return;
            }

            C1_0_textBox.Text  =((byte7&0x10)>>4).ToString("X1");
            C2_0_textBox.Text  =(byte8&0x01).ToString("X1");
            C3_0_textBox.Text  =((byte8&0x10)>>4).ToString("X1");
            C1_1_textBox.Text  =((byte7&0x20)>>5).ToString("X1");
            C2_1_textBox.Text  =((byte8&0x02)>>1).ToString("X1");
            C3_1_textBox.Text  =((byte8&0x20)>>5).ToString("X1");
            C1_2_textBox.Text  =((byte7&0x40)>>6).ToString("X1");
            C2_2_textBox.Text  =((byte8&0x04)>>2).ToString("X1");
            C3_2_textBox.Text  =((byte8&0x40)>>6).ToString("X1");
            C1_3_textBox.Text  =((byte7&0x80)>>7).ToString("X1");
            C2_3_textBox.Text  =((byte8&0x08)>>3).ToString("X1");
            C3_3_textBox.Text  =((byte8&0x80)>>7).ToString("X1");
            
       }

void bkre( )
{
    string strReceived;
    string tempStr;
    byte tmp, flag;
    byte[] temp_all;

    FM12XX_Card.SetField(0, out display);
    FM12XX_Card.Init_TDA8007(out display);
    if (bkpw.Checked == true)
    {
        if (Scl.Checked == true)
        {
            if (ACTIVE_check(out receive) == 1)
            {
                DisplayMessageLine(receive);
                FM12XX_Card.SetField(0, out display);
                return;
            }
            DisplayMessageLine(receive);
            FM12XX_Card.SendReceiveCL("32A0", out strReceived);
            FM12XX_Card.ReadBlock("00", out strReceived);
            temp_all = strToHexByte(strReceived);
            tmp = temp_all[3];
            FM12XX_Card.ReadBlock("10", out strReceived);
            temp_all = strToHexByte(strReceived);
            temp_all[0] = tmp;
            temp_all[1] = 0x69;
            tempStr = byteToHexStr(16, temp_all);
            FM12XX_Card.WriteBlock("10", tempStr, out strReceived);
            FM12XX_Card.ReadBlock("10", out strReceived);
            FM12XX_Card.SetField(0, out display);
            if (tempStr != strReceived)
            {
                DisplayMessageLine("备份失败 ");
                FM12XX_Card.SetField(0, out display);
                return;
            }
            DisplayMessageLine("备份成功 ");
        }
        else
        {
            FM12XX_Card.Cold_Reset("02", out strReceived);
            if (strReceived == "Error")
                DisplayMessageLine("Cold Reset失败 ");
            else
                DisplayMessageLine("Cold Reset成功 ");
            FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
            if (receive != "9000")
            {
                DisplayMessageLine("初始化指令失效 ");
                FM12XX_Card.Init_TDA8007(out display);
                return;
            }
            FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
            FM12XX_Card.TransceiveCT("0004000010", "02", out receive);
            temp_all = strToHexByte(receive.Substring(2, 32));
            tmp = temp_all[3];
            if (FM326.Checked == false)
                FM12XX_Card.TransceiveCT("0004002010", "02", out receive);
            else
                FM12XX_Card.TransceiveCT("0004004010", "02", out receive);
            temp_all = strToHexByte(receive.Substring(2, 32));
            temp_all[0] = tmp;
            temp_all[1] = 0x69;
            tempStr = byteToHexStr(16, temp_all);
            if (FM326.Checked == false)
                FM12XX_Card.TransceiveCT("0000002010", "02", out receive);
            else
                FM12XX_Card.TransceiveCT("0000004010", "02", out receive);
            FM12XX_Card.TransceiveCT(tempStr, "02", out receive);
            if (FM326.Checked == false)
                FM12XX_Card.TransceiveCT("0004002010", "02", out receive);
            else
                FM12XX_Card.TransceiveCT("0004004010", "02", out receive);
            FM12XX_Card.Init_TDA8007(out display);
            if (tempStr != receive.Substring(2, 32))
            {
                DisplayMessageLine("备份失败 ");
                FM12XX_Card.Init_TDA8007(out display);
                return;
            }
            DisplayMessageLine("备份成功 ");
        }
    }
    else
    {
        if (Scl.Checked == true)
        {
            if (ACTIVE_check(out receive) == 1)
            {
                DisplayMessageLine(receive);
                FM12XX_Card.SetField(0, out display);
                return;
            }
            DisplayMessageLine(receive);
            FM12XX_Card.SendReceiveCL("32A0", out strReceived);
            FM12XX_Card.ReadBlock("10", out strReceived);
            temp_all = strToHexByte(strReceived);
            tmp = temp_all[0];
            flag = temp_all[1];
            FM12XX_Card.ReadBlock("00", out strReceived);
            temp_all = strToHexByte(strReceived);
            if (flag == 0x69)
            {
                temp_all[3] &= 0xC0;
                tmp &= 0x3F;
                temp_all[3] |= tmp;
            }
            else
            {
                MessageBox.Show("温度配置没有备份");
                FM12XX_Card.SetField(0, out display);
                return;
            }
            tempStr = byteToHexStr(16, temp_all);
            FM12XX_Card.WriteBlock("00", tempStr, out strReceived);
            FM12XX_Card.ReadBlock("00", out strReceived);
            FM12XX_Card.SetField(0, out display);
            if (tempStr != strReceived)
            {
                DisplayMessageLine("恢复失败 ");
                FM12XX_Card.SetField(0, out display);
                return;
            }
            DisplayMessageLine("恢复成功 ");
        }
        else
        {
            FM12XX_Card.Cold_Reset("02", out strReceived);
            if (strReceived == "Error")
                DisplayMessageLine("Cold Reset失败 ");
            else
                DisplayMessageLine("Cold Reset成功 ");
            FM12XX_Card.TransceiveCT("0001008100", "02", out receive);
            if (receive != "9000")
            {
                DisplayMessageLine("初始化指令失效 ");
                FM12XX_Card.Init_TDA8007(out display);
                return;
            }
            FM12XX_Card.TransceiveCT("0002010000", "02", out receive);
            if (FM326.Checked == false)
                FM12XX_Card.TransceiveCT("0004002010", "02", out receive);
            else
                FM12XX_Card.TransceiveCT("0004004010", "02", out receive);
            temp_all = strToHexByte(receive.Substring(2, 32));
            tmp = temp_all[0];
            flag = temp_all[1];
            FM12XX_Card.TransceiveCT("0004000010", "02", out receive);
            temp_all = strToHexByte(receive.Substring(2, 32));
            if (flag == 0x69)
            {
                temp_all[3] &= 0xC0;
                tmp &= 0x3F;
                temp_all[3] |= tmp;
            }
            else
            {
                MessageBox.Show("温度配置没有备份");
                FM12XX_Card.Init_TDA8007(out display);
                return;
            }
            tempStr = byteToHexStr(16, temp_all);
            FM12XX_Card.TransceiveCT("0000000010", "02", out receive);
            FM12XX_Card.TransceiveCT(tempStr, "02", out receive);
            FM12XX_Card.TransceiveCT("0004000010", "02", out receive);
            FM12XX_Card.Init_TDA8007(out display);
            if (tempStr != receive.Substring(2, 32))
            {
                DisplayMessageLine("恢复失败 ");
                FM12XX_Card.Init_TDA8007(out display);
                return;
            }
            DisplayMessageLine("恢复成功 ");
        }
    }
    FM12XX_Card.SetField(0, out display);
    FM12XX_Card.Init_TDA8007(out display);    
}

        private void bkp_Click(object sender, EventArgs e)
        {
            bkre();
        }

        private void FM336_Endurance_button_Click(object sender, EventArgs e)
        {
            string BlockAddr, BlockData, strReceived;
            byte[] buff;
            int j;
            uint addr;
            buff = new byte[0x80];
            //A1命令,写128字节
            for (j = 0; j < 0x80; j++)
            {               
                buff[j] = 0x00;
            }
            BlockData = byteToHexStr(0x80, buff);// buff.ToString();
            try
            {
                Field_ON_Click(null, EventArgs.Empty);
                Active_Click(null, EventArgs.Empty);
                FM12XX_Card.TransceiveCL("3280", "01", "09", out receive);//选择数据EE
                FM12XX_Card.TransceiveCL("3180", "01", "09", out receive);//选择数据EE
                addr = 0;
                FM12XX_Card.CurrentTime(out display);
                DisplayMessageLine(display);

                while (addr < 0x80)
                {
                    BlockAddr = addr.ToString("X2");
                    FM12XX_Card.Write128Bytes(BlockAddr, BlockData, out strReceived);
                    addr = addr + 8;
                }
                FM12XX_Card.CurrentTime(out display);
                DisplayMessageLine(display);


                FM12XX_Card.CurrentTime(out display);
                DisplayMessageLine(display);
                while (addr < 0x80)
                {
                    FM12XX_Card.TransceiveCL("3387", "01", "09", out receive);//擦1，将写时间配置为0
                    FM12XX_Card.TransceiveCL("3480", "01", "09", out receive);//擦1，将写时间配置为0
                    BlockAddr = addr.ToString("X2");
                    FM12XX_Card.Write128Bytes(BlockAddr, BlockData, out strReceived);
                    addr = addr + 8;
                }
                addr = 0;
                while (addr < 0x80)
                {
                    FM12XX_Card.TransceiveCL("3380", "01", "09", out receive);//写0，将擦时间配置为0
                    FM12XX_Card.TransceiveCL("3487", "01", "09", out receive);//写0，将擦时间配置为0
                    BlockAddr = addr.ToString("X2");
                    FM12XX_Card.Write128Bytes(BlockAddr, BlockData, out strReceived);
                    addr = addr + 8;
                }
                FM12XX_Card.CurrentTime(out display);
                DisplayMessageLine(display);
                DisplayMessageLine("测试结束！！");
            }
            catch (Exception ex)
            {
                DisplayMessageLine(ex.Message);
            }
        }

        private void zc_CheckedChanged(object sender, EventArgs e)
        {
            ADcos.Text = ".\\FM1280_COS_V210_zc.hex";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string name;
            if(MessageBox.Show("是否记录送检信息,请确认相关信息是否填写完毕", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (bright.Checked)
                    name = "brightsight 送检";
                else if (bank.Checked)
                    name = "银检 送检";
                else if (zc.Checked)
                    name = "总参 送检";
                else if (ty.Checked)
                    name = "天语 送检";
                else if (gm.Checked)
                    name = "国密 送检";
                else
                    name = "326银检 送检";
               
                StreamWriter memo = new StreamWriter(".\\送检记录.txt", true);    //打开记录文档
                memo.WriteLine("----------送检项目： "+ name +"----------");
                memo.WriteLine("送检时间:  "+DateTime.Now);   	
                memo.WriteLine("操作员:  "+record_name.Text);                 
                memo.WriteLine("卡片版本:  "+record_card.Text);
                memo.WriteLine("接口类型:  "+record_interface.Text);
                memo.WriteLine("送检数量:  "+record_num.Text);
                memo.WriteLine("----------送检项目： "+ name +"----------");
                memo.WriteLine("\n");
                memo.Close();
                DisplayMessageLine("记录完毕");
            }
        }

        private void trimfor_passport_button_Click(object sender, EventArgs e)
        {
            string byte2, byte3, byte4, byte5, byte6, byte7, dfs_stv, CFG_WORD1, CFG_WORD2, strReceived, dfs04_trim, dfs09_trim, dfs0B_trim, dfs0F_trim, dfs07_trim,block_data;
            double dfs_dev, FourM_dev, dfs_value, FourM_value, dfs_t, ref_frq;
            int frq_ref;


            //case "[4]6.9MHz": dfs_cfg = "04"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0xff; dfs_stv = "6.9MHz"; dfs_t = 6.9; break;
            //case "[9]13.2MHz": dfs_cfg = "09"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "13.2MHz"; dfs_t = 13.2; break;
            //case "[B]15.5MHz": dfs_cfg = "0B"; if (interface_flag == 1) frq_ref = 0x2f; else frq_ref = 0x8f; dfs_stv = "15.5MHz"; dfs_t = 15.5; break;
            //case "[F]20.1MHz": dfs_cfg = "0F"; if (interface_flag == 1) frq_ref = 0x1f; else frq_ref = 0x8f; dfs_stv = "20.1MHz"; dfs_t = 20.1; break;




            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            block_data = "00" + CFG_WORD1.Substring(2, 18) +"29"+CFG_WORD1.Substring(22, 4) +"10" + CFG_WORD1.Substring(28, 4);
            DisplayMessageLine("CFG_WORD1:  " + block_data);
            FM12XX_Card.WriteBlock("00", block_data, out strReceived);

            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            block_data = CFG_WORD2.Substring(0, 22) + "08" + CFG_WORD2.Substring(24, 8);
            DisplayMessageLine("CFG_WORD2:  " + block_data);
            FM12XX_Card.WriteBlock("01", block_data, out strReceived);

            PROG_EEtype_radiobutton.Checked = true;
            comboBox_ChipName.SelectedIndex = 2;
            OpenFile_Button_Click(null, EventArgs.Empty);
            ProgEE_Button_Click(null, EventArgs.Empty);




            ref_frq = 13.56;
            Init_TDA8007_Click(null, EventArgs.Empty);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.RATS(out display);
            FM12XX_Card.SendAPDUCL("00FD090208", out receive);
            DisplayMessageLine(receive);
            frq_ref = 0x8f;
            dfs_stv = "13.2MHz";
            dfs_t = 13.2;
            if (receive.Substring(0, 2) != "5A")
            {
                DisplayMessageLine("trim指令回发错误！！");
                return;
            }
            dfs09_trim = receive.Substring(2, 2);
            byte2 = receive.Substring(4, 2);
            byte3 = receive.Substring(6, 2);
            byte4 = receive.Substring(8, 2);
            byte5 = receive.Substring(10, 2);
            byte6 = receive.Substring(12, 2);
            byte7 = receive.Substring(14, 2);




            ////////////////////////计算频率偏差///////////////////////
            dfs_dev = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
            FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);
            dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
            FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
            //dfs_dev, FourM_dev, dfs_value, FourM_value;
            switch (byte2)
            {
                case "10":
                    dfs_value = dfs_t + dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F5") + "MHz 。";
                    break;
                case "01":
                    dfs_value = dfs_t - dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前档位实际最低频率为" + dfs_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "DFS环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            switch (byte5)
            {
                case "10":
                    FourM_value = 4.04 + FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "01":
                    FourM_value = 4.04 - FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "4M环振频率过高，标准频率为4.04MHz， " + "当前实际最低频率为" + FourM_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "4M环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            FM12XX_Card.SendAPDUCL("00FD0B0208", out receive);
            DisplayMessageLine(receive);
            frq_ref = 0x8f;
            dfs_stv = "15.5MHz";
            dfs_t = 15.5;
            if (receive.Substring(0, 2) != "5A")
            {
                DisplayMessageLine("trim指令回发错误！！");
                return;
            }
            dfs0B_trim = receive.Substring(2, 2);
            byte2 = receive.Substring(4, 2);
            byte3 = receive.Substring(6, 2);
            byte4 = receive.Substring(8, 2);
            byte5 = receive.Substring(10, 2);
            byte6 = receive.Substring(12, 2);
            byte7 = receive.Substring(14, 2);


            ////////////////////////计算频率偏差///////////////////////
            dfs_dev = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
            FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);
            dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
            FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
            //dfs_dev, FourM_dev, dfs_value, FourM_value;
            switch (byte2)
            {
                case "10":
                    dfs_value = dfs_t + dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F5") + "MHz 。";
                    break;
                case "01":
                    dfs_value = dfs_t - dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前档位实际最低频率为" + dfs_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "DFS环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            switch (byte5)
            {
                case "10":
                    FourM_value = 4.04 + FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "01":
                    FourM_value = 4.04 - FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "4M环振频率过高，标准频率为4.04MHz， " + "当前实际最低频率为" + FourM_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "4M环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            FM12XX_Card.SendAPDUCL("00FD0F0208", out receive);
            DisplayMessageLine(receive);
            frq_ref = 0x8f;
            dfs_stv = "20.1MHz";
            dfs_t = 20.1;
            if (receive.Substring(0, 2) != "5A")
            {
                DisplayMessageLine("trim指令回发错误！！");
                return;
            }
            dfs0F_trim = receive.Substring(2, 2);
            byte2 = receive.Substring(4, 2);
            byte3 = receive.Substring(6, 2);
            byte4 = receive.Substring(8, 2);
            byte5 = receive.Substring(10, 2);
            byte6 = receive.Substring(12, 2);
            byte7 = receive.Substring(14, 2);




            ////////////////////////计算频率偏差///////////////////////
            dfs_dev = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
            FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);
            dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
            FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
            //dfs_dev, FourM_dev, dfs_value, FourM_value;
            switch (byte2)
            {
                case "10":
                    dfs_value = dfs_t + dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F5") + "MHz 。";
                    break;
                case "01":
                    dfs_value = dfs_t - dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前档位实际最低频率为" + dfs_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "DFS环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            switch (byte5)
            {
                case "10":
                    FourM_value = 4.04 + FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "01":
                    FourM_value = 4.04 - FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "4M环振频率过高，标准频率为4.04MHz， " + "当前实际最低频率为" + FourM_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "4M环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);

            FM12XX_Card.SendAPDUCL("00FD070208", out receive);
            DisplayMessageLine(receive);
            frq_ref = 0xff;
            dfs_stv = "10.7MHz";
            dfs_t = 10.7;
            if (receive.Substring(0, 2) != "5A")
            {
                DisplayMessageLine("trim指令回发错误！！");
                return;
            }
            dfs07_trim = receive.Substring(2, 2);
            byte2 = receive.Substring(4, 2);
            byte3 = receive.Substring(6, 2);
            byte4 = receive.Substring(8, 2);
            byte5 = receive.Substring(10, 2);
            byte6 = receive.Substring(12, 2);
            byte7 = receive.Substring(14, 2);




            ////////////////////////计算频率偏差///////////////////////
            dfs_dev = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
            FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);
            dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
            FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
            //dfs_dev, FourM_dev, dfs_value, FourM_value;
            switch (byte2)
            {
                case "10":
                    dfs_value = dfs_t + dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F5") + "MHz 。";
                    break;
                case "01":
                    dfs_value = dfs_t - dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前档位实际最低频率为" + dfs_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "DFS环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            switch (byte5)
            {
                case "10":
                    FourM_value = 4.04 + FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "01":
                    FourM_value = 4.04 - FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "4M环振频率过高，标准频率为4.04MHz， " + "当前实际最低频率为" + FourM_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "4M环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);


            FM12XX_Card.SendAPDUCL("00FD040208", out receive);
            DisplayMessageLine(receive);
            frq_ref = 0xff;
            dfs_stv = "6.9MHz";
            dfs_t = 6.9;
            if (receive.Substring(0, 2) != "5A")
            {
                DisplayMessageLine("trim指令回发错误！！");
                return;
            }
            dfs04_trim = receive.Substring(2, 2);
            byte2 = receive.Substring(4, 2);
            byte3 = receive.Substring(6, 2);
            byte4 = receive.Substring(8, 2);
            byte5 = receive.Substring(10, 2);
            byte6 = receive.Substring(12, 2);
            byte7 = receive.Substring(14, 2);


            ////////////////////////计算频率偏差///////////////////////
            dfs_dev = ((strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]) * ref_frq) / (frq_ref * 256 + 0xff);
            FourM_dev = ((strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]) * ref_frq) / (255 * 256 + 0xff);
            dfs_value = (ref_frq * 65535) / (strToHexByte(byte3)[0] * 256 + strToHexByte(byte4)[0]);
            FourM_value = (ref_frq * 65535) / (strToHexByte(byte6)[0] * 256 + strToHexByte(byte7)[0]);
            //dfs_dev, FourM_dev, dfs_value, FourM_value;
            switch (byte2)
            {
                case "10":
                    dfs_value = dfs_t + dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏高 " + dfs_dev.ToString("F5") + "MHz 。";
                    break;
                case "01":
                    dfs_value = dfs_t - dfs_dev;
                    display = "DFS环振校准完成，当前频率为" + dfs_value.ToString("F4") + "MHz，比标准频率" + dfs_stv + "偏低 " + dfs_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "DFS环振频率过高，标准频率为" + dfs_stv + "， " + "当前档位实际最低频率为" + dfs_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "DFS环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            switch (byte5)
            {
                case "10":
                    FourM_value = 4.04 + FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏高 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "01":
                    FourM_value = 4.04 - FourM_dev;
                    display = "4M环振校准完成，当前频率为" + FourM_value.ToString("F4") + "MHz，比标准频率4.04MHz偏低 " + FourM_dev.ToString("F4") + "MHz 。";
                    break;
                case "FF":
                    display = "4M环振频率过高，标准频率为4.04MHz， " + "当前实际最低频率为" + FourM_value.ToString("F4") + "MHz 。";
                    break;
                default:
                    display = "4M环振校准回发错误，请检查芯片是否能正常工作，且载入了频率校准的COS！";
                    break;
            }
            DisplayMessageLine(display);
            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);
            block_data = dfs0F_trim + dfs07_trim + "AA00" + dfs04_trim + dfs0B_trim + dfs09_trim + "000000000000000000";
            FM12XX_Card.WriteBlock("1F", block_data, out strReceived);
            FM12XX_Card.ReadBlock("1F", out block_data); ;
            DisplayMessageLine("1F0:  " + block_data);
        }

        private void cfg_and_progfor_passport_button_Click(object sender, EventArgs e)
        {
            string strReceived, tmpStr, uid_Temp, CFG_WORD1, CFG_WORD2, block_data;
            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            //Active_Click(null, EventArgs.Empty);
            FM12XX_Card.REQA(out display);
            tmpStr = "";
            tmpStr += "ATQA：" + display + "\t";
            FM12XX_Card.AntiColl_lv(1, out uid_Temp);
            tmpStr += "UID：" + uid_Temp + "\t";
            FM12XX_Card.Select(1, out display);
            tmpStr += "Sak：" + display;
            DisplayMessageLine(tmpStr);
            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);

            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);
            block_data = "20601E" + CFG_WORD1.Substring(6, 2) + "2E227B" + uid_Temp.Substring(0, 4) + "A42B003C0297" + CFG_WORD1.Substring(30, 2);
            DisplayMessageLine("CFG_WORD1:  " + block_data);
            FM12XX_Card.WriteBlock("00", block_data, out strReceived);
            block_data = CFG_WORD2.Substring(0, 2) + "0702000000000000900E01005555A5";
            DisplayMessageLine("CFG_WORD2:  " + block_data);
            FM12XX_Card.WriteBlock("01", block_data, out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);


            DATA_EEtype_radiobutton.Checked = true;
            comboBox_ChipName.SelectedIndex = 2;
            OpenFile_Button_Click(null, EventArgs.Empty);
            ProgEE_Button_Click(null, EventArgs.Empty);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);
            FM12XX_Card.WriteBlock("04", "503335380080033602157D7400FF00FF", out strReceived);
            FM12XX_Card.WriteBlock("05", "00000000000000000000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("08", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("09", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("0A", "00000000000000000000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("0B", "00000000000000000000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("0C", "00000000000000000000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("0D", "00000000000000000000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("10", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("11", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("12", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("13", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("14", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("15", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("16", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("17", "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out strReceived);
            FM12XX_Card.WriteBlock("18", "FF000000000000000000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("19", "FC696907F6000000000080094F008009", out strReceived);
            FM12XX_Card.WriteBlock("1A", "A1008006230080978D008005D3008005", out strReceived);
            FM12XX_Card.WriteBlock("1B", "3B0080084C00FF000000800319008024", out strReceived);
            FM12XX_Card.WriteBlock("1C", "5200000000008441B000000000000000", out strReceived);
            FM12XX_Card.WriteBlock("1D", "A5550B0B0B55050B0577B0400909000F", out strReceived);
            FM12XX_Card.WriteBlock("1E", "0F073C00000000000000000000000000", out strReceived);

            FM12XX_Card.ReadBlock("04", out display);
            DisplayMessageLine("04: " + display);
            FM12XX_Card.ReadBlock("18", out display);
            DisplayMessageLine("18: " + display);
            FM12XX_Card.ReadBlock("19", out display);
            DisplayMessageLine("19: " + display);
            FM12XX_Card.ReadBlock("1A", out display);
            DisplayMessageLine("1A: " + display);
            FM12XX_Card.ReadBlock("1B", out display);
            DisplayMessageLine("1B: " + display);
            FM12XX_Card.ReadBlock("1C", out display);
            DisplayMessageLine("1C: " + display);
            FM12XX_Card.ReadBlock("1D", out display);
            DisplayMessageLine("1D: " + display);
            FM12XX_Card.ReadBlock("1E", out display);
            DisplayMessageLine("1E: " + display);
            FM12XX_Card.ReadBlock("1F", out display);
            DisplayMessageLine("1F: " + display);
            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.RATS(out display);
            DisplayMessageLine("RATS:  " + display);
        }

        private void cfg_dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            cfg_dataGridView.CurrentCell.Style.BackColor = Color.LightGray;
            //MessageBox.Show(cfg_dataGridView.Rows[e.RowIndex].Tag.ToString());
        }

        private void cfg_dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            cfg_dataGridView.CurrentCell.Style.BackColor = Color.Empty;

        }

        private void cfg_dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if (cfg_dataGridView.CurrentCell.Tag == null)
            //    return;
            ////MessageBox.Show(cfg_dataGridView.CurrentCell.FormattedValue.ToString());
            //if (cfg_dataGridView.CurrentCell.FormattedValue.ToString() != cfg_dataGridView.CurrentCell.Tag.ToString())
            //{
            //    cfg_dataGridView.CurrentCell.Style.ForeColor = Color.Red;
            //}
            //else
            //{
            //    cfg_dataGridView.CurrentCell.Style.ForeColor = Color.Black;
            //}
        }


        private void cfg_dataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (cfg_dataGridView.Rows[e.RowIndex].Cells[2].Tag == null)
                return;
            //MessageBox.Show(cfg_dataGridView.CurrentCell.FormattedValue.ToString());
            if (cfg_dataGridView.Rows[e.RowIndex].Cells[2].FormattedValue.ToString() != cfg_dataGridView.Rows[e.RowIndex].Cells[2].Tag.ToString())
            {
                cfg_dataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
            }
            else
            {
                cfg_dataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void userfor_passport_button_Click(object sender, EventArgs e)
        {
            string strReceived, tmpStr, uid_Temp, CFG_WORD1, CFG_WORD2, block_data;
            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            //Active_Click(null, EventArgs.Empty);
            FM12XX_Card.REQA(out display);
            tmpStr = "";
            tmpStr += "ATQA：" + display + "\t";
            FM12XX_Card.AntiColl_lv(1, out uid_Temp);
            tmpStr += "UID：" + uid_Temp + "\t";
            FM12XX_Card.Select(1, out display);
            tmpStr += "Sak：" + display;
            DisplayMessageLine(tmpStr);
            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);
            block_data = CFG_WORD1.Substring(0, 8) + "0E" + CFG_WORD1.Substring(10, 4) + uid_Temp.Substring(0, 4) + CFG_WORD1.Substring(18, 14);
            DisplayMessageLine("CFG_WORD1:  " + block_data);
            FM12XX_Card.WriteBlock("00", block_data, out strReceived);
            block_data = CFG_WORD2.Substring(0, 26) + "5AAAA5";
            DisplayMessageLine("CFG_WORD2:  " + block_data);
            FM12XX_Card.WriteBlock("01", block_data, out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.RATS(out display);
            DisplayMessageLine("RATS:  " + display);
        }

        private void user_lock_button_Click(object sender, EventArgs e)
        {
            string strReceived, tmpStr, uid_Temp, CFG_WORD1, CFG_WORD2, block_data;

            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.RATS(out display);
            DisplayMessageLine("RATS:  " + display);
            FM12XX_Card.SendAPDUCL("0069000000", out receive);
            DisplayMessageLine("已切回编程模式！" + receive);

            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            Active_Click(null, EventArgs.Empty);
            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);

            FM12XX_Card.WriteBlock("19", "FC696903F6000000000080094F008009", out strReceived);
            FM12XX_Card.ReadBlock("19", out display);
            DisplayMessageLine("已关闭模式切换补丁，block19: " + display);



            FM12XX_Card.Init_TDA8007(out strReceived);
            Reset17_Click(null, EventArgs.Empty);
            Field_ON_Click(null, EventArgs.Empty);
            //Active_Click(null, EventArgs.Empty);
            FM12XX_Card.REQA(out display);
            tmpStr = "";
            tmpStr += "ATQA：" + display + "\t";
            FM12XX_Card.AntiColl_lv(1, out uid_Temp);
            tmpStr += "UID：" + uid_Temp + "\t";
            FM12XX_Card.Select(1, out display);
            tmpStr += "Sak：" + display;
            DisplayMessageLine(tmpStr);

            FM12XX_Card.TransceiveCL("32A0", "01", "09", out strReceived);
            FM12XX_Card.TransceiveCL("3180", "01", "09", out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);
            block_data = CFG_WORD1.Substring(0, 8) + "0E" + CFG_WORD1.Substring(10, 4) + uid_Temp.Substring(0, 4) + CFG_WORD1.Substring(18, 14);
            DisplayMessageLine("CFG_WORD1:  " + block_data);
            FM12XX_Card.WriteBlock("00", block_data, out strReceived);
            block_data = CFG_WORD2.Substring(0, 6) + "FFFFBFDFBFFF900E01005AAAA5";
            DisplayMessageLine("CFG_WORD2:  " + block_data);
            FM12XX_Card.WriteBlock("01", block_data, out strReceived);
            FM12XX_Card.ReadBlock("00", out CFG_WORD1); ;
            FM12XX_Card.ReadBlock("01", out CFG_WORD2); ;
            DisplayMessageLine(CFG_WORD1);
            DisplayMessageLine(CFG_WORD2);
        }

        public virtual int freq_trim( )
        {
            string strReceived, rlt8, rlt358;
            int k;

            FM12XX_Card.TransceiveCT("000DE80000", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003C010004", "02", out strReceived);
            FM12XX_Card.TransceiveCT("80010003", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003C070004", "02", out strReceived);
            FM12XX_Card.TransceiveCT("00560011", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003C071804", "02", out strReceived);
            FM12XX_Card.TransceiveCT("0000FFFF", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003C071C04", "02", out strReceived);
            FM12XX_Card.TransceiveCT("00003FFF", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003D070810", "02", out strReceived);
            FM12XX_Card.TransceiveCT("0000001100E807140000001000000010", "02", out strReceived);
            FM12XX_Card.TransceiveCT("0004072804", "02", out rlt8);
            FM12XX_Card.TransceiveCT("0004072C04", "02", out rlt358);
            //Init_TDA8007_Click(null, EventArgs.Empty);
            k = Convert.ToUInt16(rlt8.Substring(6,4), 16);
            return k;
        }

        //public virtual int config8m(int flag)
        //{
        //    string strReceived, sendbuf_data;
        //    int k, h2, l2;
        //    byte[] temp;

        //    FM12XX_Card.Cold_Reset("02", out strReceived);
        //    k = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
        //    if (k != 0)
        //    {
        //        DisplayMessageLine("01初始化指令:   \t\t" + "错误！！");
        //        return 0;
        //    }          
        //    FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
        //    FM12XX_Card.TransceiveCT("0004000024", "02", out strReceived);
        //    //sendbuf_data = strReceived.Substring(2, 72);                   
        //    temp = strToHexByte(strReceived.Substring(2, 72));                  //config word
        //    h2 = temp[2] % 4;
        //    l2 = temp[3] / 64;
        //    k = h2 + l2;
        //    if ((k == 6) || (k == 0))
        //        return 16;
        //    if (flag == 1)
        //    {
        //        if (l2 == 3)
        //        {
        //            temp[3] &= 0x3F;
        //            temp[2]++;
        //        }
        //        else
        //            temp[3] += 64;
        //    }
        //    else
        //    {
        //        if (l2 == 0)
        //        {
        //            temp[2]--;
        //            temp[3] |= 0xC0;
        //        }
        //        else
        //            temp[3] -= 64;
        //    }
        //    sendbuf_data = byteToHexStr(36, temp);                          //new config word
            
        //    FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
        //    FM12XX_Card.TransceiveCT("0004000064", "02", out strReceived); //bak sector                                             
        //    sendbuf_data += strReceived.Substring(74, 128);

        //    FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
        //    FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
        //    FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);

        //    FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
        //    FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
        //    FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

        //    FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
        //    FM12XX_Card.TransceiveCT("00AF0000" + "00DE0000" + "00000063" + "00000045", "02", out strReceived);

        //    FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
        //    FM12XX_Card.TransceiveCT("0004000001", "02", out strReceived); // NVM

        //    FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
        //    FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
        //    k = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
        //    if (k == 0)
        //        DisplayMessageLine("写入配置字:\t<-\t" + "Successed");
        //    else
        //    {
        //        DisplayMessageLine("写入配置字:\t<-\t" + "ERROR -- " + strReceived);
        //        return 1;
        //    }

        //    FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
        //    FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
        //    FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);

        //    FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
        //    FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
        //    FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

        //    FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
        //    FM12XX_Card.TransceiveCT("00AF0000" + "00DE0600" + "00000063" + "00000045", "02", out strReceived);

        //    FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
        //    FM12XX_Card.TransceiveCT("0004060001", "02", out strReceived); // NVM

        //    FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
        //    FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
        //    k = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
        //    //Init_TDA8007_Click(null, EventArgs.Empty);
        //    if (k == 0)
        //        DisplayMessageLine("写入备份区:\t<-\t" + "Successed");
        //    else
        //    {
        //        DisplayMessageLine("写入备份区:\t<-\t" + "ERROR -- " + strReceived);
        //        return 1;
        //    }
        //    return 2;
        //}

        public virtual int reg8m(int flag)
        {
            string strReceived, sendbuf_data;
            int k, h2, l2;
            byte[] temp;

            FM12XX_Card.TransceiveCT("000DE80000", "02", out strReceived);
            FM12XX_Card.TransceiveCT("0004000004", "02", out strReceived);               
            temp = strToHexByte(strReceived.Substring(2, 8));                  //config word
            h2 = temp[2] % 4;
            l2 = temp[3] / 64;
            k = h2 + l2;
            if ((k == 6) && (flag == 1))
                return 16;
            if ((k == 0) && (flag == 0))
                return 0;
            if (flag == 1)
            {
                if (l2 == 3)
                {
                    temp[3] &= 0x3F;
                    temp[2]++;
                }
                else
                    temp[3] += 64;
            }
            else
            {
                if (l2 == 0)
                {
                    temp[2]--;
                    temp[3] |= 0xC0;
                }
                else
                    temp[3] -= 64;
            }
            sendbuf_data = byteToHexStr(4, temp);                          //new config word
            FM12XX_Card.TransceiveCT("003C000004", "02", out strReceived);
            FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);
            return 2;
        }

        private void trim8m_Click(object sender, EventArgs e)
        {
            string strReceived, str_cofig;
            int result, k, cnt, cnt1, cnt0;
            double g1, g2;

            FM12XX_Card.Cold_Reset("02", out strReceived);
            k = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
            if (k != 0)
            {
                DisplayMessageLine("01初始化指令:   \t\t" + "错误！！");
                return;
            }
            else
                DisplayMessageLine("01初始化指令:   \t\t" + strReceived);

            if (trim_wr.Checked == false)
            {
                k = freq_trim();
                g1 = ((double)k / 16383) * 3.58;
                DisplayMessageLine("8M 当前频率:   \t\t" + g1.ToString("F2"));
            }
            else
            {
                result = 0; cnt = 0; cnt1 = 0; cnt0 = 0; g1 = 0; g2 = 0;               
                while (result == 0)
                {
                    k = freq_trim();
                    g1 = ((double)k / 16383) * 3.58;
                    DisplayMessageLine("8M tirm -->   \t\t" + g1.ToString("F2"));
                    if (g1 < 8)
                    {
                        cnt = reg8m(1);
                        cnt1++;
                    }
                    else
                    {
                        cnt = reg8m(0);
                        cnt0++;
                    }
                   
                    if ((cnt == 16) && (g1 < 8))
                    {
                        result = 1;
                        DisplayMessageLine("8M trim 已无档位可调！");
                    }
                    else if((cnt == 0) && (g1 > 8))
                    {
                        result = 1;
                        DisplayMessageLine("8M trim 已无档位可调！");
                    }
                    else if ((cnt1 > 0) && (cnt0 > 0))
                    {
                        k = freq_trim();
                        g2 = ((double)k / 16383) * 3.58;
                        result = 2;
                    }
                }
                if (result == 1)
                    DisplayMessageLine("8M tirm 完成:   \t\t" + g1.ToString("F2"));
                else if (Math.Abs(g2-8) > Math.Abs(g1-8))
                {
                    if (g2 > 8)
                        reg8m(0);
                    else
                        reg8m(1);
                    k = freq_trim();
                    g1 = ((double)k / 16383) * 3.58;
                    DisplayMessageLine("8M tirm 完成:   \t\t" + g1.ToString("F2"));
                }
                else
                    DisplayMessageLine("8M tirm 完成:   \t\t" + g2.ToString("F2"));

                FM12XX_Card.TransceiveCT("000DE80000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0004000004", "02", out strReceived);
                str_cofig = strReceived.Substring(2, 8);                  //config word
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0004000064", "02", out strReceived);
                str_cofig += strReceived.Substring(10, 192);

                FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
                FM12XX_Card.TransceiveCT(str_cofig, "02", out strReceived);

                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

                FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                FM12XX_Card.TransceiveCT("00AF0000" + "00DE0000" + "00000063" + "00000045", "02", out strReceived);

                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
                FM12XX_Card.TransceiveCT("0004000001", "02", out strReceived); // NVM

                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
                k = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                if (k == 0)
                    DisplayMessageLine("写入配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("写入配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
                FM12XX_Card.TransceiveCT(str_cofig, "02", out strReceived);

                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
                FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

                FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
                FM12XX_Card.TransceiveCT("00AF0000" + "00DE0600" + "00000063" + "00000045", "02", out strReceived);

                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
                FM12XX_Card.TransceiveCT("0004060001", "02", out strReceived); // NVM

                FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
                FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
                k = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
                Init_TDA8007_Click(null, EventArgs.Empty);
                if (k == 0)
                    DisplayMessageLine("写入备份区:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("写入备份区:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string strReceived, temp, name;
            int k;

            switch (aux_s.SelectedIndex)
            {
                case 0: temp = "0000040000000006"; //verf1p2  
                    name = "verf1p2";
                    break;
                case 1: temp = "0000070000000006"; //vref_flash
                    name = "vref_flash";                    
                    break;
                case 2: temp = "0000080000000006"; //vdet_vb
                    name = "vdet_vb";   
                    break;
                case 3: temp = "0000000000000006"; //vdd12
                    name = "vdd12";   
                    break;
                case 4: temp = "0000010000000006"; //vdd12_ana
                    name = "vdd12_ana"; 
                    break;
                case 5: temp = "0000030000000006"; //vdd16
                    name = "vdd16"; 
                    break;
                case 6: temp = "0000020000000006"; //vdd20
                    name = "vdd20"; 
                    break;
                case 7: temp = "0000060000000006"; //vbias1p2
                    name = "vbias1p2"; 
                    break;
                case 8: temp = "0000050000000006"; //vtemp
                    name = "vtemp"; 
                    break;
                case 9: temp = "0000090000000006"; //vref_htm
                    name = "vref_htm"; 
                    break;
                case 10: temp = "00000A0000000006"; //vref_ltm
                    name = "vref_ltm"; 
                    break;
                default: temp = "0000040000000006"; //verf1p2
                    name = "verf1p2"; 
                    break;
            }
            FM12XX_Card.Cold_Reset("02", out strReceived);
            k = FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
            if (k != 0)
            {
                DisplayMessageLine("01初始化指令:   \t\t" + "错误！！");
                return;
            }
            else
                DisplayMessageLine("01初始化指令:   \t\t" + strReceived);
            FM12XX_Card.TransceiveCT("000DE90000", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003C000404", "02", out strReceived);
            FM12XX_Card.TransceiveCT("055919B9", "02", out strReceived);
            FM12XX_Card.TransceiveCT("000DE80000", "02", out strReceived);
            FM12XX_Card.TransceiveCT("003C0A8008", "02", out strReceived);
            FM12XX_Card.TransceiveCT(temp, "02", out strReceived);
            DisplayMessageLine("AUX - PA7(GPIO4)的电压值即为 " + name + " 的电压");
        }

        private void M1_Visible_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (M1_Visible_checkBox.Checked)
            {
                groupBox_MIFARE.Visible = true;
                TransceiveDataCL_listBox.Location = new Point(432, 97);
                TransceiveDataCL_listBox.Height = 304;
            }
            else
            {
                groupBox_MIFARE.Visible = false;
                TransceiveDataCL_listBox.Location = new Point(432, 13);
                TransceiveDataCL_listBox.Height = 388;
            }
        }

        private void btnReadUID_Click(object sender, EventArgs e)
        {
            string strReceived, typeAInfo,volt="02";
            int uid1_index = 0x38;
            int sak1_index = 0x3D;
            int atqa_index = 0x3E;
            int uid2_index = 0x28;
            int sak2_index = 0x2D;
            int uid3_index = 0x2E;
            int sak3_index = 0x33;
            if (r347.Checked == true)
            {
                if (Volt30.Checked)
                    volt = "02";
                else if (Volt50.Checked)
                    volt = "01";
                else if (Volt18.Checked)
                    volt = "03";
                FM12XX_Card.Init_TDA8007(out strReceived);
                FM12XX_Card.Cold_Reset(volt, out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0004010000", "02", out strReceived);
                fm347_TypeAInfo = strReceived.Substring(2, 0x200);
                DisplayMessageLine(fm347_TypeAInfo);
                typeAInfo = strReceived.Substring(uid1_index * 2 + 2, 10);
                typeAInfo += "  ";
                typeAInfo += strReceived.Substring(uid2_index * 2 + 2, 10);
                typeAInfo += "  ";
                typeAInfo += strReceived.Substring(uid3_index * 2 + 2, 10);
                typeAInfo += "  ";
                typeAInfo += strReceived.Substring(sak1_index * 2 + 2, 2);
                typeAInfo += "  ";
                typeAInfo += strReceived.Substring(sak2_index * 2 + 2, 2);
                typeAInfo += "  ";
                typeAInfo += strReceived.Substring(sak3_index * 2 + 2, 2);
                typeAInfo += "  ";
                typeAInfo += strReceived.Substring(atqa_index * 2 + 2, 4);

                SendData_textBox.Text = typeAInfo;

            }
            else
            {
                DisplayMessageLine("Cmd is only applicable to FM347");
                return;
            }

        }

        private void btnWriteUID_Click(object sender, EventArgs e)
        {
            if (SendData_textBox.Text.Length != 52 || fm347_TypeAInfo == "" || fm347_TypeAInfo.Length != 0x200)
            {
                MessageBox.Show("Invalid UID info length or format, please press Read UID button first.");
                return;
            }
            string strReceived,nvr1Data, typeAInfo1, typeAInfo2, volt = "02";
            int uid1_index = 0x38;
            int sak1_index = 0x3D;
            int atqa_index = 0x3E;
            int uid2_index = 0x28;
            int sak2_index = 0x2D;
            int uid3_index = 0x2E;
            int sak3_index = 0x33;
            typeAInfo1 = SendData_textBox.Text.Substring(12, 10) //uid2
                + SendData_textBox.Text.Substring(40, 2)//sak2
                + SendData_textBox.Text.Substring(24, 10)//uid3
                + SendData_textBox.Text.Substring(44, 2);//sak3
            typeAInfo2 = SendData_textBox.Text.Substring(0, 10)//uid1
                + SendData_textBox.Text.Substring(36, 2)//sak1
                + SendData_textBox.Text.Substring(48, 4);//atqa
            nvr1Data = fm347_TypeAInfo.Substring(0, uid2_index * 2)
                + typeAInfo1
                + fm347_TypeAInfo.Substring(0x34 * 2, 8)
                + typeAInfo2
                + fm347_TypeAInfo.Substring(128);
            if(nvr1Data.Length!=0x200)
            {
                MessageBox.Show("Date generated has invalid length!");
                return;
            }
            //DisplayMessageLine(nvr1Data);
            text_00_P1.Text = "01";
            text_00_P2.Text = "00";
            text_00_len.Text = "00";
            text_02_P1.Text = "DE";
            text_04_P1.Text = "01";
            text_04_P2.Text = "00";
            text_04_len.Text = "00";
            DisplayMessageLine("Updating type A info: " + SendData_textBox.Text + " ...");
            SendData_textBox.Text = nvr1Data;
            Init_Wree_00_Click(sender, e);


        }

        private void VHBR_PPS_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (VHBR_PPS_checkBox.Checked)
                pps_exchange_CL_btn.Text = "VHBR PPS";
            else
                pps_exchange_CL_btn.Text = "PPS";

        }

        private void XOR_button_Click(object sender, EventArgs e)//CNN
        {
            byte[] data;
            int xor_data,i,length;
            data = strToHexByte(DeleteSpaceString(SendData_textBox.Text));
            length=data.Length;
            xor_data = data[0];
            for (i = 1; i < length;i++ )
                xor_data = xor_data ^ data[i];
            DisplayMessageLine(xor_data.ToString("X2"));
        }

        private int fm347_config_wr(string sendbuf_data, string sect, bool flag)  // sendbuf_data 36-bytes
        {
            string strReceived;
            if (flag)  //100-BYTE
            {
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0004000064", "02", out strReceived); //bak sector                                             
                sendbuf_data += strReceived.Substring(74, 128);
                FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                FM12XX_Card.TransceiveCT("003C000064", "02", out strReceived); //cl_ram 0000
                FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);
            }
            else  //256-BYTE
            {
                FM12XX_Card.TransceiveCT("000DAF0000", "02", out strReceived); //cl_ram AF
                FM12XX_Card.TransceiveCT("003C000000", "02", out strReceived); //cl_ram 0000
                FM12XX_Card.TransceiveCT(sendbuf_data, "02", out strReceived);
            }

            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
            FM12XX_Card.TransceiveCT("003C001004", "02", out strReceived); // NVM_op_start 0010
            FM12XX_Card.TransceiveCT("55AAAA55", "02", out strReceived);

            FM12XX_Card.TransceiveCT("003C000010", "02", out strReceived); // NVM_op_src_strat_add,NVM_op_des_strat_add,NVM_op_length,NVM_op_mode
            if(flag)
                FM12XX_Card.TransceiveCT("00AF0000" + "00DE"+sect+"00" + "00000063" + "00000045", "02", out strReceived);
            else
                FM12XX_Card.TransceiveCT("00AF0000" + "00DE"+sect+"00" + "000000FF" + "00000045", "02", out strReceived);

            FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived); //NVM
            FM12XX_Card.TransceiveCT("0004" + sect + "0001", "02", out strReceived); // NVM

            FM12XX_Card.TransceiveCT("000DE00000", "02", out strReceived); //NVM_op_start E0
            FM12XX_Card.TransceiveCT("003D001010", "02", out strReceived); // NVM_op_start 0010                
            int i = FM12XX_Card.TransceiveCT("5AA5A55A" + "00E00200" + "00000001" + "00000001", "02", out strReceived);
            return i;
        }

        private int temp_check()
        {
            string strReceived, block;
            byte alarm;
            byte[] temp;
            if (init.Checked)
            {
                FM12XX_Card.TransceiveCT("0004040804", "02", out strReceived);
                block = strReceived.Substring(2, 8);
            }
            else
            {
                FM12XX_Card.SendAPDUCT("0053010008 E80408 04 00000000", out strReceived);
                FM12XX_Card.SendAPDUCT("0051010004 E80408 04", out strReceived);
                block = strReceived;
            }
            temp = strToHexByte(block);
            if(low_temp.Checked == true)
                alarm = 0x04;
            else
                alarm = 0x08;
            if((temp[2] & alarm) == 0)
                return 0;
            else 
                return 1;
        }

        private void temp_wr()
        { 
            string strReceived, data;
            byte value;
            byte[] temp;

            if (init.Checked)
            {
                FM12XX_Card.TransceiveCT("0004000404", "02", out strReceived);
                data = strReceived.Substring(2, 8);
            }
            else
            {
                FM12XX_Card.SendAPDUCT("0051010004 E80004 04", out strReceived);
                data = strReceived;
            }
            temp = strToHexByte(data);
            if (low_temp.Checked == true)
                value = 1;
            else
                value = 16;
            temp[0] -= value;
            data = byteToHexStr(4, temp); //new temp word
            if (init.Checked)
            {
                FM12XX_Card.TransceiveCT("003C000404", "02", out strReceived);
                FM12XX_Card.TransceiveCT(data, "02", out strReceived);
            }
            else
                FM12XX_Card.SendAPDUCT("0053010008 E80004 04" + data, out receive);
        }

        private void btnWordXOR_Click(object sender, EventArgs e)
        {
            string strOpdata, str1;
            byte[] strOp1 = new byte[4];
            int xor_data, i, length, wordLen;
            uint nTemp1,nXOR;

            strOpdata = DeleteSpaceString(SendData_textBox.Text);
            length = strOpdata.Length;
            if (length < 8)
            {
                MessageBox.Show("输入数据必须至少一个word!");
                return;
            }
            if ((length & 0x07) != 0x00)
            {
                MessageBox.Show("输入数据必须是word对齐!");
                return;
            }
           
            wordLen = length / 8;
            nXOR = 0;
            for (i = 0; i < wordLen; i++)
            {
                //xor_data = xor_data ^ data[i];
                str1 = strOpdata.Substring(i * 8, 8);
                strOp1 = strToHexByte(str1);
                nTemp1 = (uint)(strOp1[3] + strOp1[2] * 0x100 + strOp1[1] * 0x10000 + strOp1[0] * 0x1000000);
                nXOR ^= nTemp1;
            }
            DisplayMessageLine(nXOR.ToString("X8"));

        }

        private void temp_fm347_Click(object sender, EventArgs e)
        {
            string strReceived, block0, sendbuf_data;
            byte[] temp_all;
            int k, i, j;

            temp_rlt.Text = "耐心等候，挑卡中……";
            if (init.Checked)
            {
                FM12XX_Card.SetField(0, out display);
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                temp_all = strToHexByte(block0.Substring(2, 72));
                if (temp_retain.Checked == false)
                    temp_all[4] = 0x99; //温度报警第10档
                temp_all[7] = 0xFF; //打开所有安全报警
                temp_all[10] = 0x00; //关闭所有复位
                temp_all[11] = 0x00; //关闭所有复位
                temp_all[24] |= 0x80; //Vbe模式
                sendbuf_data = byteToHexStr(36, temp_all); //new config word
                i = fm347_config_wr(sendbuf_data, "00", true);
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DE80000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                temp_rlt.Text = "耐心等候，挑卡中……";
                if (auto_delay.Checked)
                {
                    DisplayMessageLine("延时50秒…… ");
                    Delay(50);
                    DisplayMessageLine("延时结束 ");
                }

                if (low_temp.Checked == true)
                    DisplayMessageLine("低温筛卡开始 ");
                else
                    DisplayMessageLine("高温筛卡开始 ");
            }
            else
            {               
                FM12XX_Card.SetField(0, out display);
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                if (strReceived != "9000" && strReceived != "019000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                temp_all = strToHexByte(block0.Substring(2, 72));
                if (temp_retain.Checked == false)
                    temp_all[4] = 0x99; //温度报警设为第10档——实际温度 -30℃/100℃
                temp_all[7] = 0xFF; //打开所有安全报警
                temp_all[9] &= 0xFD; //NVM BOOT启动
                temp_all[9] |= 0x01; //NVM BOOT启动
                temp_all[10] = 0x00; //关闭所有复位
                temp_all[11] = 0x00; //关闭所有复位
                temp_all[24] |= 0x80; //Vbe模式
                sendbuf_data = byteToHexStr(36, temp_all); //new config word
                i = fm347_config_wr(sendbuf_data, "00", true);
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }
                FM12XX_Card.Init_TDA8007(out display);
                if (d_cos.Checked)
                {
                    FM347_radioButton.Checked = true;
                    CT_Interface.Checked = true;
                    checkBox_CTHighBaud.Checked = true;
                    PROG_EEtype_radiobutton.Checked = true;
                    sec_addr.SelectedIndex = 1; //NVM BOOT 50段
                    textBox_terase.Text = "32";
                    textBox_tprog.Text = "47";

                    program(".\\fm347_cos.hex", 1, false, 0);
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived != "3BD895024064464D53485F333437")
                    {
                        DisplayMessageLine("无ATR！！！ 1.重试 2.接触良好？3.换卡");
                        return;
                    }
                    if (auto_delay.Checked)
                    {
                        DisplayMessageLine("延时40秒…… ");
                        Delay(40);
                        DisplayMessageLine("延时结束 ");
                    }
                }
                else
                {
                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived != "3BD895024064464D53485F333437")
                    {
                        DisplayMessageLine("无ATR！！！  1.重试 2.接触良好？3.换卡");
                        return;
                    }
                    DisplayMessageLine("挑卡开始！！！");
                    if (auto_delay.Checked)
                    {
                        DisplayMessageLine("延时50秒…… ");
                        Delay(50);
                        DisplayMessageLine("延时结束 ");
                    }
                }
                if (low_temp.Checked == true)
                    DisplayMessageLine("低温筛卡开始 ");
                else
                    DisplayMessageLine("高温筛卡开始 ");
            }
            for(k=10; k>0; k--)
            {
                i = temp_check();
                if (i == 0)
                {
                    DisplayMessageLine("第 " +  k.ToString("X2") + " 档   ——>   不报警 ");
                    if (k > 1)
                        temp_wr();
                    else if (wr_config.Checked == true)
                    {
                        if (init.Checked)
                        {
                            FM12XX_Card.TransceiveCT("0004000024", "02", out strReceived);
                            j = fm347_config_wr(strReceived.Substring(2, 72),"00", true);
                        }
                        else
                        {
                            FM12XX_Card.SendAPDUCT("0051010004 E80000 24", out receive);
                            FM12XX_Card.Init_TDA8007(out display);
                            FM12XX_Card.Cold_Reset("02", out strReceived);
                            FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                            j = fm347_config_wr(receive, "00", true);
                        }
                        if (j == 0)
                            DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                        else
                        {
                            DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                            return;
                        }
                        temp_rlt.Text = "很遗憾，挑卡失败！";
                    }
                    else
                        temp_rlt.Text = "很遗憾，挑卡失败！";
                }
                else
                {
                    DisplayMessageLine("报警档位 ——> " + k.ToString("X2"));
                    if (wr_config.Checked == true)
                    {
                        if (init.Checked)
                        {
                            FM12XX_Card.TransceiveCT("0004000024", "02", out strReceived);
                            j = fm347_config_wr(strReceived.Substring(2, 72),"00", true);
                        }
                        else
                        {
                            FM12XX_Card.SendAPDUCT("0051010004 E80000 24", out receive);
                            FM12XX_Card.Init_TDA8007(out display);
                            FM12XX_Card.Cold_Reset("02", out strReceived);
                            FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                            j = fm347_config_wr(receive,"00", true);
                        }
                        if (j == 0)
                            DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                        else
                        {
                            DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                            return;
                        }                   
                    }
                    temp_rlt.Text = "恭喜，卡片合格！";
                    break;
                }    
            }
            FM12XX_Card.Init_TDA8007(out display);
            DisplayMessageLine("------------------end-------------------");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strReceived, block0;
            byte[] temp_all;
            int i, j;

            temp_rlt.Text = "耐心等候，挑卡中……";
            if (d_cos.Checked)
            {
                FM12XX_Card.Init_TDA8007(out display);
                FM347_radioButton.Checked = true;
                CT_Interface.Checked = true;
                checkBox_CTHighBaud.Checked = true;
                PROG_EEtype_radiobutton.Checked = true;
                sec_addr.SelectedIndex = 1; //NVM BOOT 50段
                textBox_terase.Text = "32";
                textBox_tprog.Text = "47";

                program(".\\fm347_cos.hex", 1, false, 0);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived != "3BD895024064464D53485F333437")
                {
                    DisplayMessageLine("无ATR！！！ 1.重试 2.接触良好？3.换卡");
                    return;
                }
                if (auto_delay.Checked)
                {
                    DisplayMessageLine("延时35秒…… ");
                    Delay(35);
                    DisplayMessageLine("延时结束 ");
                }
            }
            else
            {
                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived != "3BD895024064464D53485F333437")
                {
                    DisplayMessageLine("无ATR！！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                DisplayMessageLine("挑卡开始！！！");
                if (auto_delay.Checked)
                {
                    DisplayMessageLine("延时50秒…… ");
                    Delay(50);
                    DisplayMessageLine("延时结束 ");
                }
            }

            FM12XX_Card.SendAPDUCT("0053010008 E80408 04 00000000", out receive);
            FM12XX_Card.SendAPDUCT("0051010004 E80408 04", out receive);
            temp_all = strToHexByte(receive);
            if ((temp_all[2] & 0x0C) != 0)
            {
                FM12XX_Card.SendAPDUCT("0051010004 E80004 04", out receive);
                temp_all = strToHexByte(receive);
                temp_all[0] += 16; //增加一档
                block0 = byteToHexStr(4, temp_all);
                FM12XX_Card.SendAPDUCT("0053010008 E80004 04" + block0, out receive);
                for (i = 0, j = 0; i < 10; i++)
                {
                    FM12XX_Card.SendAPDUCT("0053010008 E80408 04 00000000", out receive);
                    FM12XX_Card.SendAPDUCT("0051010004 E80408 04", out receive);
                    temp_all = strToHexByte(receive);
                    if ((temp_all[2] & 0x0C) != 0)
                        j++;
                }
                if (j > 3)
                {
                    temp_rlt.Text = "增加一档仍报警，挑卡失败！";
                    DisplayMessageLine("------------------end-------------------");
                }
                else
                {
                    temp_rlt.Text = "恭喜，卡片合格！";
                    DisplayMessageLine("------------------end-------------------");
                }
            }
            else
            {
                temp_rlt.Text = "不报警，挑卡失败！";
                DisplayMessageLine("------------------end-------------------");
            }
            FM12XX_Card.Init_TDA8007(out display);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strReceived, block0, block1, block2, sendbuf_data, UID;
            byte[] temp_all, temp_new;
            int k, i, j;

            if (NVM_init.Checked)
            { 
                //------------00,06 sector-------------
                FM12XX_Card.SetField(0, out display);
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                temp_all = strToHexByte(block0.Substring(2, 72));
                if (mp300_347.Checked)
                    block1 = "89A643E4778DF90881DA2200001602F7C0048940C3041004920B7F0700027C0055AAA55A";
                else if (cc_347.Checked)
                    block1 = "89A643E477C9FDFD1FD8FFFF00160207D004894082041004DF2B630300027C0055AAA55A";
                else
                    block1 = block0;
                temp_new = strToHexByte(block1);
                temp_new[0] = temp_all[0]; //保留电源trim值   
                temp_new[1] = temp_all[1]; //保留电源trim值   
                temp_new[2] = temp_all[2]; //保留电源trim值                  
                temp_new[3] = temp_all[3]; //保留电源trim值            
                temp_new[4] = temp_all[4]; //保留温度报警档位
                if (NVM_encrypt.Checked)
                    temp_new[6] &= 0xFB;  //关掉NVM加密
                temp_all[17] &=0x08;       //保留LDO12_ct_lptrim[0] CW4-bit19
                temp_new[17] |= temp_all[17];
                temp_all[18] &=0x80;       //保留LDO12_ct_lptrim[1] CW4-bit15
                temp_new[18] |= temp_all[18];
                temp_all[23] &=0x08;       //保留LDO16_cl_lptrim[0] CW5-bit3
                temp_new[23] |= temp_all[23];
                temp_all[29] &=0x08;       //保留LDO16_cl_lptrim[1] CW7-bit19
                temp_new[29] |= temp_all[29];
                temp_new[11] = 0;         //关闭所有安全复位
                if (NVM_init.Checked)
                    temp_new[14] &= 0x80;     //NVM的firm大小设为0

                sendbuf_data = byteToHexStr(36, temp_new); //new config word
                sendbuf_data += "000000380000000000000000" + "00000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";

                i = fm347_config_wr(sendbuf_data, "00", false);
                DisplayMessageLine("------------00 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                i = fm347_config_wr(sendbuf_data, "06", false);
                DisplayMessageLine("------------06 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }
                FM12XX_Card.Init_TDA8007(out display);
                //------------00,06 sector-------------

                //------------01,02,03,04,05,07 sector-------------
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                if (cc_card.Checked)
                    UID = "C" + N_uid.Text + "00";
                else
                    UID = "D" + N_uid.Text + "00";
                temp_all = strToHexByte(UID);
    
                for (i = 0, k = 0, j = 128; i < 8; i++)
                {
                    if ((temp_all[0] & (0x01 << i)) != 0)
                        k += j;
                    j /= 2;                   
                }
                temp_new[1] = (byte)k;
                for (i = 0, k = 0, j = 128; i < 8; i++)
                {
                    if ((temp_all[1] & (0x01 << i)) != 0)
                        k += j;
                    j /= 2;   
                }
                temp_new[0] = (byte)k;

                block0 = byteToHexStr(2, temp_new); //UID由LSB转成MSB
                temp_all[2] = (byte)(0x34 ^ 0x71 ^ temp_all[0] ^ temp_all[1]); //UID的异或值
                UID = UID.Substring(0, 4);
                block2 = byteToHexStr(3, temp_all); 
                temp_all[0] ^= 0XFF;
                temp_all[1] ^= 0XFF;
                block1 = byteToHexStr(2, temp_all); //UID的反码
            
                sendbuf_data = "0541" + block0 + UID + block1 + "06F838C800015555";
                sendbuf_data += "0002AAAAFFFFFFFFFFFFFFFFFFFFFFFF" + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFF3471" + block2 + "280400" + "90FF0101FFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + "464D534800414E4A0001000112800005";
                sendbuf_data += "0001000100030103000100000160FF01" + "01FFFFFF54111CFAFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFF5A96A569FFFFFFFFFFFFFFFF";

                i = fm347_config_wr(sendbuf_data, "01", false);
                DisplayMessageLine("------------01 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                sendbuf_data = "55AAAA555A5A000000000000000000003F3F040400020000000000340555FFFE";
                sendbuf_data += "00000D47007D0C3200007FEF28F4001B40C0C0C0C0C08080C0C0C0C04040C0C0";
                sendbuf_data += "C0000000C0C0C0C0C0C040C00000000000000000000000000000000000000000";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "7777777774447744777774474477777700000000000000000000000000000000";
                sendbuf_data += "076F00C50000000000000000000000000A06030C0A06030C0A06030C0A06030C";
                sendbuf_data += "0000200000002000000020000000200000002000000020000000200000002000";
                sendbuf_data += "00000000000000000000000000000000000000000000000000000000EACDEC9C";

                i = fm347_config_wr(sendbuf_data, "02", false);
                DisplayMessageLine("------------02 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                sendbuf_data = "A55A699600000000FFFFFFFF006969010010FFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFF010010FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF010010FFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFF010010FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF010010FFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFF000000000000000000000000000000000011";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "000000000000000000000000000000000000000000000000000000004BCDEE68";

                i = fm347_config_wr(sendbuf_data, "03", false);
                DisplayMessageLine("------------03 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                sendbuf_data = "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";

                i = fm347_config_wr(sendbuf_data, "04", false);
                DisplayMessageLine("------------04 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                i = fm347_config_wr(sendbuf_data, "05", false);
                DisplayMessageLine("------------05 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                i = fm347_config_wr(sendbuf_data, "07", false);
                DisplayMessageLine("------------07 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                FM12XX_Card.Init_TDA8007(out display);
                //------------01,02,03,04,05,07 sector-------------

                FM347_radioButton.Checked = true;
                CT_Interface.Checked = true;
                checkBox_CTHighBaud.Checked = true;
                PROG_EEtype_radiobutton.Checked = true;
                sec_addr.SelectedIndex = 0; //NVM user 00段
                textBox_terase.Text = "32";
                textBox_tprog.Text = "47";
                InitData_comboBox.SelectedIndex = 0;
                ProgEEEndAddr.Text = "03FFFF"; //256k
                InitEEdata_Button_Click(null, EventArgs.Empty);
                ProgEE_Button_Click(null, EventArgs.Empty);

                DisplayMessageLine("配置NVM的firm区大小！");
                //------------00,06 sector-------------
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                temp_all = strToHexByte(block0.Substring(2, 72));
                temp_all[14] = 0x02;     //NVM的firm大小设为8k
                sendbuf_data = byteToHexStr(36, temp_all); //new config word
                sendbuf_data += "000000380000000000000000" + "00000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";

                i = fm347_config_wr(sendbuf_data, "00", false);
                DisplayMessageLine("------------00 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                i = fm347_config_wr(sendbuf_data, "06", false);
                DisplayMessageLine("------------06 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }
                //------------00,06 sector-------------
                temp_all = strToHexByte("0" + N_uid.Text);
                k = (int)(temp_all[1] + temp_all[0] * 256);
                k++;
                N_uid.Text = k.ToString("X3");

                DisplayMessageLine("NVM初始化完成！");
            }
            if (fm347_cos.Checked)
            {
                FM347_radioButton.Checked = true;
                CT_Interface.Checked = true;
                checkBox_CTHighBaud.Checked = true;
                PROG_EEtype_radiobutton.Checked = true;
                sec_addr.SelectedIndex = 0; //NVM user 00段
                textBox_terase.Text = "32";
                textBox_tprog.Text = "47";
                ADcos.Text = ".\\FM1280_V50B.hex";
                program(".\\FM1280_V50B.hex", 1, false, 0);
                sec_addr.SelectedIndex = 2; //NVM firmware 70段
                ADcos.Text = ".\\fm347_firmware_nvm_V50B.bin";
                program(".\\fm347_firmware_nvm_V50B.bin", 1, false, 0);

                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived != "3BD69502406431323830050B")
                {
                    DisplayMessageLine("无ATR！！！ 1.重试 2.接触良好？3.换卡");
                    return;
                }
                else
                {
                    if (cc_347.Checked)
                    {
                        DisplayMessageLine("打开所有安全复位！");
                        //------------00,06 sector-------------
                        FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                        FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                        if (strReceived != "9000")
                        {
                            DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                            return;
                        }
                        FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                        temp_all = strToHexByte(block0.Substring(2, 72));
                        temp_all[11] = 0xFF; //打开所有安全复位
                        sendbuf_data = byteToHexStr(36, temp_all); //new config word
                        sendbuf_data += "000000380000000000000000" + "00000000000000000000000000000000";
                        sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                        sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                        sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                        sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                        sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                        sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";

                        i = fm347_config_wr(sendbuf_data, "00", false);
                        DisplayMessageLine("------------00 sector-------------");
                        if (i == 0)
                            DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                        else
                        {
                            DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                            return;
                        }

                        i = fm347_config_wr(sendbuf_data, "06", false);
                        DisplayMessageLine("------------06 sector-------------");
                        if (i == 0)
                            DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                        else
                        {
                            DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                            return;
                        }
                        //------------00,06 sector-------------
                    }
                    FM12XX_Card.Init_TDA8007(out display);
                    DisplayMessageLine("FM347 CC认证 初始化配置 成功！");
                    DisplayMessageLine("------------------end-------------------");
                }
            }
        }

        private void cmdNBFaka_Click(object sender, EventArgs e)
        {
            string strReceived, StrDisplay, block0, block1, block2, sendbuf_data, UID;
            byte[] temp_all, temp_new;
            byte[] ClockTrim;
            byte clkTrim8M, clkTrimDFS;
            UInt32 TempL;
            int k, i, j;

            if (NVM_init.Checked)
            {
                //------------00,06 sector-------------
                FM12XX_Card.SetField(0, out display);
                FM12XX_Card.Init_TDA8007(out display);
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                temp_all = strToHexByte(block0.Substring(2, 72));
                block1 = block0.Substring(2, 72);
                temp_new = strToHexByte(block1);
                temp_new[6] = (byte)((UInt32)temp_all[6] | 0x88);     //CW1.B,CW1.F
                temp_new[18] = (byte)(((UInt32)temp_all[18] & 0xE1) | 0x10);     //CW4.c~9=1000
                temp_new[19] = (byte)(((UInt32)temp_all[19] & 0xF0) | 0x01);     //CW4.3~0=0001
                temp_new[25] = (byte)(((UInt32)temp_all[25] & 0xF3) | 0x08);     //CW6.13H~12H=10

                sendbuf_data = byteToHexStr(36, temp_new); //new config word
                sendbuf_data += "000000380000000000000000" + "00000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";

                i = fm347_config_wr(sendbuf_data, "00", false);
                DisplayMessageLine("------------00 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                i = fm347_config_wr(sendbuf_data, "06", false);
                DisplayMessageLine("------------06 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }
                FM12XX_Card.Init_TDA8007(out display);
                //------------00,06 sector-------------

                //------------01,02 sector-------------
                FM12XX_Card.Cold_Reset("02", out strReceived);
                FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                if (strReceived != "9000")
                {
                    DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                    return;
                }
                if (UID_modify.Checked)
                {
                    FM12XX_Card.TransceiveCT("0004015005", "02", out block0);
                    temp_all = strToHexByte(block0.Substring(2, 10));
                    block2 = byteToHexStr(5, temp_all);
                }
                else
                {
                    if (cc_card.Checked)
                        UID = "B" + N_uid.Text + "00";
                    else
                        UID = "0" + N_uid.Text + "00";

                    temp_all = strToHexByte(UID);

                    temp_all[2] = (byte)(0x34 ^ 0x72 ^ temp_all[0] ^ temp_all[1]); //UID的异或值
                    UID = UID.Substring(0, 4);
                    block2 = "3472" + byteToHexStr(3, temp_all);
                }

                sendbuf_data = "0541FFFFFFFFFFFF06F838C8000155550002AAAAFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" + block2 + "200800";
                sendbuf_data += "90FF0101FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

                i = fm347_config_wr(sendbuf_data, "01", false);
                DisplayMessageLine("------------01 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                sendbuf_data = "55AAAA555A5A000000000000000000003F3F040400020000000000340555FFFE";
                sendbuf_data += "00000D47007D0C3200007FEF28F4001B40C0C0C0C0C08080C0C0C0C04040C0C0";
                sendbuf_data += "C0000000C0C0C0C0C0C040C00000000000000000000000000000000000000000";
                sendbuf_data += "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                sendbuf_data += "7777777774447744777774474477777700000000000000000000000000000000";
                sendbuf_data += "076F00C50000000000000000000000000A06030C0A06030C0A06030C0A06030C";
                sendbuf_data += "0000200000002000000020000000200000002000000020000000200000002000";
                sendbuf_data += "00000000000000000000000000000000000000000000000000000000EACDEC9C";

                i = fm347_config_wr(sendbuf_data, "02", false);
                DisplayMessageLine("------------02 sector-------------");
                if (i == 0)
                    DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                else
                {
                    DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                    return;
                }

                FM12XX_Card.Init_TDA8007(out display);
                //------------01,02 sector-------------
                if (UID_modify.Checked == false)
                {
                    temp_all = strToHexByte("0" + N_uid.Text);
                    k = (int)(temp_all[1] + temp_all[0] * 256);
                    k++;
                    N_uid.Text = k.ToString("X3");
                }
                DisplayMessageLine("NVM初始化完成！");
            }
            if (checkClockTrim.Checked)
            {
                //Debug 
                /*clkTrimDFS = 0x33;
                clkTrim8M = 0x0A;
                StrDisplay = "调校值:DFS - " + clkTrimDFS.ToString("X2") + "H,8M - " + clkTrim8M.ToString("X2") +"H";
                DisplayMessageLine(StrDisplay);
                return; */
                //Debug end 

                FM347_radioButton.Checked = true;
                CT_Interface.Checked = true;
                checkBox_CTHighBaud.Checked = true;
                PROG_EEtype_radiobutton.Checked = true;
                sec_addr.SelectedIndex = 1; //NVM boot 50段
                textBox_terase.Text = "32";
                textBox_tprog.Text = "47";
                ADcos.Text = ".\\fm347_freq_trim.hex";
                program(".\\fm347_freq_trim.hex", 1, false, 0);

                FM12XX_Card.Cold_Reset("02", out strReceived);
                if (strReceived != "3BD895024064464D53485F333437")
                {
                    DisplayMessageLine("无ATR！！！ 1.重试 2.接触良好？3.换卡");
                    return;
                }
                else
                {
                    DisplayMessageLine("DFS 环振时钟调校！");
                    FM12XX_Card.TransceiveCT("00FD120009", "02", out strReceived);
                    temp_all = strToHexByte(strReceived.Substring(0, 4));
                    if ((temp_all[0] != 0xFD) || (temp_all[1] != 0x5A))
                    {
                        DisplayMessageLine("时钟调校指令出错！！");
                        return;
                    }
                    ClockTrim = strToHexByte(strReceived.Substring(4, 16));
                    clkTrimDFS = ClockTrim[0];
                    clkTrim8M = ClockTrim[7];
                    StrDisplay = "调校值:DFS - " + clkTrimDFS.ToString("X2") + "H,8M - " + clkTrim8M.ToString("X2") + "H";
                    DisplayMessageLine(StrDisplay);

                    FM12XX_Card.Cold_Reset("02", out strReceived);
                    if (strReceived != "3BD895024064464D53485F333437")
                    {
                        DisplayMessageLine("无ATR！！！ 1.重试 2.接触良好？3.换卡");
                        return;
                    }
                    //------------00,06 sector-------------
                    FM12XX_Card.TransceiveCT("0001000000", "02", out strReceived);
                    FM12XX_Card.TransceiveCT("000DDE0000", "02", out strReceived);
                    if (strReceived != "9000")
                    {
                        DisplayMessageLine("初始化指令出错！！  1.重试 2.接触良好？3.换卡");
                        return;
                    }

                    FM12XX_Card.TransceiveCT("0004000024", "02", out block0);
                    temp_all = strToHexByte(block0.Substring(2, 72));
                    block1 = block0.Substring(2, 72);
                    temp_new = strToHexByte(block1);
                    TempL = (UInt32)(temp_all[0] * 0x1000000 + temp_all[1] * 0x10000 + temp_all[2] * 0x100 + temp_all[3]);
                    TempL = (UInt32)((TempL & 0xFFFFFC3F) | ((UInt32)clkTrim8M << 6));   //CW0.[9~6]=
                    temp_new[0] = (byte)((TempL & 0xFF000000) >> 24);
                    temp_new[1] = (byte)((TempL & 0x00FF0000) >> 16);
                    temp_new[2] = (byte)((TempL & 0x0000FF00) >> 8);
                    temp_new[3] = (byte)(TempL & 0x000000FF);
                    TempL = (UInt32)(temp_all[20] * 0x1000000 + temp_all[21] * 0x10000 + temp_all[22] * 0x100 + temp_all[23]);
                    TempL = (UInt32)((TempL & 0x03FFFFFF) | ((UInt32)clkTrimDFS << 26));   //CW5.[1FH~1AH]
                    TempL = (UInt32)((TempL & 0xFC0FFFFF) | ((UInt32)clkTrimDFS << 20));   //CW5.[1FH~14H]=
                    temp_new[20] = (byte)((TempL & 0xFF000000) >> 24);
                    temp_new[21] = (byte)((TempL & 0x00FF0000) >> 16);
                    temp_new[22] = (byte)((TempL & 0x0000FF00) >> 8);
                    temp_new[23] = (byte)(TempL & 0x000000FF);

                    sendbuf_data = byteToHexStr(36, temp_new); //new config word
                    sendbuf_data += "000000380000000000000000" + "00000000000000000000000000000000";
                    sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                    sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                    sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                    sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                    sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";
                    sendbuf_data += "0000000000000000000000000000000000000000000000000000000000000000";

                    i = fm347_config_wr(sendbuf_data, "00", false);
                    DisplayMessageLine("------------00 sector-------------");
                    if (i == 0)
                        DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                    else
                    {
                        DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                        return;
                    }

                    i = fm347_config_wr(sendbuf_data, "06", false);
                    DisplayMessageLine("------------06 sector-------------");
                    if (i == 0)
                        DisplayMessageLine("改写配置字:\t<-\t" + "Successed");
                    else
                    {
                        DisplayMessageLine("改写配置字:\t<-\t" + "ERROR -- " + strReceived);
                        return;
                    }
                    //------------00,06 sector-------------

                    FM12XX_Card.Init_TDA8007(out display);
                    DisplayMessageLine("FM347 内部发卡 初始化配置 成功！");
                    DisplayMessageLine("------------------end-------------------");
                }
            }
        }

        private void AuthKey_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TimeOut_textBox_TextChanged(object sender, EventArgs e)
        {

        }
             
	}
}
             
