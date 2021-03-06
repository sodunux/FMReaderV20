﻿using System;
namespace PcSc
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            try
            {

                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (Exception )
            {
            }
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.main_note = new System.Windows.Forms.RichTextBox();
            this.main_note_cMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl_main = new System.Windows.Forms.TabControl();
            this.tabPage_CL = new System.Windows.Forms.TabPage();
            this.btnWordXOR = new System.Windows.Forms.Button();
            this.XOR_button = new System.Windows.Forms.Button();
            this.M1_Visible_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox_MIFARE = new System.Windows.Forms.GroupBox();
            this.auth_uidlevel_textBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.AuthBlock_textBox = new System.Windows.Forms.TextBox();
            this.buttonRESTORE = new System.Windows.Forms.Button();
            this.buttonTRANSFER = new System.Windows.Forms.Button();
            this.buttonINCValue = new System.Windows.Forms.Button();
            this.buttonDECValue = new System.Windows.Forms.Button();
            this.AuthType_comboBox = new System.Windows.Forms.ComboBox();
            this.buttonReadValue = new System.Windows.Forms.Button();
            this.label66 = new System.Windows.Forms.Label();
            this.KeyMode_comboBox = new System.Windows.Forms.ComboBox();
            this.AuthKey_textBox = new System.Windows.Forms.TextBox();
            this.buttonWriteValue = new System.Windows.Forms.Button();
            this.Ldkey_button = new System.Windows.Forms.Button();
            this.Auth_button = new System.Windows.Forms.Button();
            this.buttonReadBlock = new System.Windows.Forms.Button();
            this.buttonWriteBlock = new System.Windows.Forms.Button();
            this.BlockAddr_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Send_Selected_Command_CL_button = new System.Windows.Forms.Button();
            this.Open_CMDlog_button = new System.Windows.Forms.Button();
            this.Access_bits_textBox = new System.Windows.Forms.TextBox();
            this.parse_access_bits_button = new System.Windows.Forms.Button();
            this.label84 = new System.Windows.Forms.Label();
            this.label83 = new System.Windows.Forms.Label();
            this.label82 = new System.Windows.Forms.Label();
            this.label81 = new System.Windows.Forms.Label();
            this.label80 = new System.Windows.Forms.Label();
            this.label79 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.C3_3_textBox = new System.Windows.Forms.TextBox();
            this.C2_3_textBox = new System.Windows.Forms.TextBox();
            this.C1_3_textBox = new System.Windows.Forms.TextBox();
            this.C3_2_textBox = new System.Windows.Forms.TextBox();
            this.C2_2_textBox = new System.Windows.Forms.TextBox();
            this.C1_2_textBox = new System.Windows.Forms.TextBox();
            this.C3_1_textBox = new System.Windows.Forms.TextBox();
            this.C2_1_textBox = new System.Windows.Forms.TextBox();
            this.C1_1_textBox = new System.Windows.Forms.TextBox();
            this.C3_0_textBox = new System.Windows.Forms.TextBox();
            this.C2_0_textBox = new System.Windows.Forms.TextBox();
            this.C1_0_textBox = new System.Windows.Forms.TextBox();
            this.Cal_access_bits_button = new System.Windows.Forms.Button();
            this.buttonASCIIToString = new System.Windows.Forms.Button();
            this.buttonStringToASCII = new System.Windows.Forms.Button();
            this.button_AppendCRC16 = new System.Windows.Forms.Button();
            this.label75 = new System.Windows.Forms.Label();
            this.textBox_CRC = new System.Windows.Forms.TextBox();
            this.button_CRC = new System.Windows.Forms.Button();
            this.cla_parity_groupbox = new System.Windows.Forms.GroupBox();
            this.cla_no_parity_cfg = new System.Windows.Forms.RadioButton();
            this.cla_even_parity_cfg = new System.Windows.Forms.RadioButton();
            this.cla_odd_parity_cfg = new System.Windows.Forms.RadioButton();
            this.textBox_APDU_Le_CL = new System.Windows.Forms.TextBox();
            this.LeText = new System.Windows.Forms.Label();
            this.GetReaders_bnt = new System.Windows.Forms.Button();
            this.readers_cbox = new System.Windows.Forms.ComboBox();
            this.label57 = new System.Windows.Forms.Label();
            this.flash_isp_comSelect = new System.Windows.Forms.ComboBox();
            this.version_check = new System.Windows.Forms.Button();
            this.label51 = new System.Windows.Forms.Label();
            this.ResFreqCacu_L_btn = new System.Windows.Forms.Button();
            this.ResFreqCacu_C_btn = new System.Windows.Forms.Button();
            this.ResFreqCacu_F_btn = new System.Windows.Forms.Button();
            this.ResonFreqCaculate_F_tB = new System.Windows.Forms.TextBox();
            this.ResonFreqCaculate_L_tB = new System.Windows.Forms.TextBox();
            this.ResonFreqCaculate_C_tB = new System.Windows.Forms.TextBox();
            this.CLTypeSelect_groupBox = new System.Windows.Forms.GroupBox();
            this.TypeBSel_radioButton = new System.Windows.Forms.RadioButton();
            this.TypeASel_radioButton = new System.Windows.Forms.RadioButton();
            this.TransceiveDataCL_listBox = new System.Windows.Forms.ListBox();
            this.AddCMDtoListBox_CL_btn = new System.Windows.Forms.Button();
            this.checkBox_APDUmodeCL = new System.Windows.Forms.CheckBox();
            this.label_APDU_CL = new System.Windows.Forms.Label();
            this.textBox_APDU_P3_CL = new System.Windows.Forms.TextBox();
            this.textBox_APDU_P2_CL = new System.Windows.Forms.TextBox();
            this.textBox_APDU_P1_CL = new System.Windows.Forms.TextBox();
            this.textBox_APDU_INS_CL = new System.Windows.Forms.TextBox();
            this.textBox_APDU_CLA_CL = new System.Windows.Forms.TextBox();
            this.CardType_Select = new System.Windows.Forms.ComboBox();
            this.Field_ON = new System.Windows.Forms.Button();
            this.ConnectReader = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.TimeOut_textBox = new System.Windows.Forms.TextBox();
            this.Reset17 = new System.Windows.Forms.Button();
            this.CRC_gropbox = new System.Windows.Forms.GroupBox();
            this.tx_crc_cfg = new System.Windows.Forms.RadioButton();
            this.rx_crc_cfg = new System.Windows.Forms.RadioButton();
            this.no_crc_cfg = new System.Windows.Forms.RadioButton();
            this.all_crc_cfg = new System.Windows.Forms.RadioButton();
            this.TransceiveCL = new System.Windows.Forms.Button();
            this.groupBox_FM17Reg = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Read17Reg = new System.Windows.Forms.Button();
            this.Write17Reg = new System.Windows.Forms.Button();
            this.RegAddr_17_textBox = new System.Windows.Forms.TextBox();
            this.RegData_17_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox_CLA_CONTROL = new System.Windows.Forms.GroupBox();
            this.VHBR_PPS_checkBox = new System.Windows.Forms.CheckBox();
            this.textBox_PPSCL_DRI = new System.Windows.Forms.TextBox();
            this.label_DSI = new System.Windows.Forms.Label();
            this.label_DRI = new System.Windows.Forms.Label();
            this.label_CID = new System.Windows.Forms.Label();
            this.textBox_CID = new System.Windows.Forms.TextBox();
            this.checkBox_last_SAK = new System.Windows.Forms.CheckBox();
            this.checkBox_check_BCC = new System.Windows.Forms.CheckBox();
            this.MI_Deselect = new System.Windows.Forms.Button();
            this.Active = new System.Windows.Forms.Button();
            this.MI_REQA = new System.Windows.Forms.Button();
            this.MI_AntiColl = new System.Windows.Forms.Button();
            this.MI_Select = new System.Windows.Forms.Button();
            this.button_RATS = new System.Windows.Forms.Button();
            this.pps_exchange_CL_btn = new System.Windows.Forms.Button();
            this.label_UID1 = new System.Windows.Forms.Label();
            this.WupA = new System.Windows.Forms.Button();
            this.label_UID2 = new System.Windows.Forms.Label();
            this.Halt = new System.Windows.Forms.Button();
            this.label_UID3 = new System.Windows.Forms.Label();
            this.label_SAK1 = new System.Windows.Forms.Label();
            this.label_SAK2 = new System.Windows.Forms.Label();
            this.label_SAK3 = new System.Windows.Forms.Label();
            this.textBox_UID1 = new System.Windows.Forms.TextBox();
            this.textBox_UID2 = new System.Windows.Forms.TextBox();
            this.textBox_UID3 = new System.Windows.Forms.TextBox();
            this.textBox_SAK1 = new System.Windows.Forms.TextBox();
            this.textBox_SAK2 = new System.Windows.Forms.TextBox();
            this.textBox_SAK3 = new System.Windows.Forms.TextBox();
            this.textBox_last_SAK = new System.Windows.Forms.TextBox();
            this.textBox_PPSCL_DSI = new System.Windows.Forms.TextBox();
            this.groupBox_CLB_CONTROL = new System.Windows.Forms.GroupBox();
            this.comboBox_CLB_EOF = new System.Windows.Forms.ComboBox();
            this.comboBox_CLB_SOF = new System.Windows.Forms.ComboBox();
            this.label_EGT = new System.Windows.Forms.Label();
            this.textBox_EGT = new System.Windows.Forms.TextBox();
            this.button_TypeBFraming = new System.Windows.Forms.Button();
            this.checkBox_CLB_EOF = new System.Windows.Forms.CheckBox();
            this.checkBox_CLB_SOF = new System.Windows.Forms.CheckBox();
            this.label_parameters = new System.Windows.Forms.Label();
            this.textBox_Parameters = new System.Windows.Forms.TextBox();
            this.label_PUPI = new System.Windows.Forms.Label();
            this.textBox_PUPI = new System.Windows.Forms.TextBox();
            this.checkBox_ExtREQB = new System.Windows.Forms.CheckBox();
            this.label_Slots = new System.Windows.Forms.Label();
            this.comboBox_N = new System.Windows.Forms.ComboBox();
            this.textBox_DRI_CLB = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label_AFI = new System.Windows.Forms.Label();
            this.textBox_AFI = new System.Windows.Forms.TextBox();
            this.button_Deselect_CLB = new System.Windows.Forms.Button();
            this.button_REQB = new System.Windows.Forms.Button();
            this.button_SlotMARKER = new System.Windows.Forms.Button();
            this.button_ATTRIB = new System.Windows.Forms.Button();
            this.button_PPS_CLB = new System.Windows.Forms.Button();
            this.button_WUPB = new System.Windows.Forms.Button();
            this.button_HLTB = new System.Windows.Forms.Button();
            this.textBox_SlotNumber = new System.Windows.Forms.TextBox();
            this.textBox_DSI_CLB = new System.Windows.Forms.TextBox();
            this.tabPage_CT = new System.Windows.Forms.TabPage();
            this.btnWriteUID = new System.Windows.Forms.Button();
            this.btnReadUID = new System.Windows.Forms.Button();
            this.Send_Slected_Command_button = new System.Windows.Forms.Button();
            this.CT_OPEN_CMDLOG_button = new System.Windows.Forms.Button();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.aux_s = new System.Windows.Forms.ComboBox();
            this.label89 = new System.Windows.Forms.Label();
            this.button14 = new System.Windows.Forms.Button();
            this.trim_wr = new System.Windows.Forms.CheckBox();
            this.trim8m = new System.Windows.Forms.Button();
            this.text_0B_P3 = new System.Windows.Forms.TextBox();
            this.ins54 = new System.Windows.Forms.GroupBox();
            this.wr3e = new System.Windows.Forms.RadioButton();
            this.rd05 = new System.Windows.Forms.RadioButton();
            this.CT_347_STOPMCU = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.r347 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.FreqRead = new System.Windows.Forms.Button();
            this.init_AUTH_55 = new System.Windows.Forms.Button();
            this.text_0A_P2 = new System.Windows.Forms.TextBox();
            this.text_0A_P1 = new System.Windows.Forms.TextBox();
            this.init_CRC_0A = new System.Windows.Forms.Button();
            this.label59 = new System.Windows.Forms.Label();
            this.PPS_exchange_ct_tbox = new System.Windows.Forms.TextBox();
            this.PPS_exchange_ct_btn = new System.Windows.Forms.Button();
            this.label56 = new System.Windows.Forms.Label();
            this.text_0B_P2 = new System.Windows.Forms.TextBox();
            this.text_0B_P1 = new System.Windows.Forms.TextBox();
            this.Init_Trim_0B = new System.Windows.Forms.Button();
            this.text_03_P2 = new System.Windows.Forms.TextBox();
            this.text_03_P1 = new System.Windows.Forms.TextBox();
            this.Init_ewEEtime_03 = new System.Windows.Forms.Button();
            this.text_08_P1 = new System.Windows.Forms.TextBox();
            this.Init_CPUStart_08 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.text_01_P1 = new System.Windows.Forms.TextBox();
            this.text_04_len = new System.Windows.Forms.TextBox();
            this.text_00_len = new System.Windows.Forms.TextBox();
            this.text_02_P2 = new System.Windows.Forms.TextBox();
            this.text_04_P2 = new System.Windows.Forms.TextBox();
            this.text_00_P2 = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.text_02_P1 = new System.Windows.Forms.TextBox();
            this.text_04_P1 = new System.Windows.Forms.TextBox();
            this.text_00_P1 = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.Init_Rdee_04 = new System.Windows.Forms.Button();
            this.Init_EEopt_02 = new System.Windows.Forms.Button();
            this.Init_01 = new System.Windows.Forms.Button();
            this.Init_Wree_00 = new System.Windows.Forms.Button();
            this.Parity_groupBox = new System.Windows.Forms.GroupBox();
            this.parity_odd_rdBtn = new System.Windows.Forms.RadioButton();
            this.parity_even_rdBtn = new System.Windows.Forms.RadioButton();
            this.TransceiveDataCT_listBox = new System.Windows.Forms.ListBox();
            this.AddCMDtoListBox_CT_btn = new System.Windows.Forms.Button();
            this.textBox_APDU_P3_CT = new System.Windows.Forms.TextBox();
            this.textBox_APDU_P2_CT = new System.Windows.Forms.TextBox();
            this.textBox_APDU_P1_CT = new System.Windows.Forms.TextBox();
            this.textBox_APDU_INS_CT = new System.Windows.Forms.TextBox();
            this.textBox_APDU_CLA_CT = new System.Windows.Forms.TextBox();
            this.checkBox_APDUmodeCT = new System.Windows.Forms.CheckBox();
            this.label_APDU_CT = new System.Windows.Forms.Label();
            this.TR_CT_TimeOut = new System.Windows.Forms.GroupBox();
            this.CT_TimeOut_NoTimeout = new System.Windows.Forms.RadioButton();
            this.CT_TimeOut_100ETU = new System.Windows.Forms.RadioButton();
            this.CT_TimeOut_std = new System.Windows.Forms.RadioButton();
            this.Clock_Stop_Btn = new System.Windows.Forms.Button();
            this.CST_select = new System.Windows.Forms.GroupBox();
            this.CST_Low = new System.Windows.Forms.RadioButton();
            this.CST_High = new System.Windows.Forms.RadioButton();
            this.TransceiveCT = new System.Windows.Forms.Button();
            this.VoltageSel = new System.Windows.Forms.GroupBox();
            this.Volt18 = new System.Windows.Forms.RadioButton();
            this.Volt30 = new System.Windows.Forms.RadioButton();
            this.Volt50 = new System.Windows.Forms.RadioButton();
            this.Warm_Reset = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.RegData_TDA_textBox = new System.Windows.Forms.TextBox();
            this.RegAddr_TDA_textBox = new System.Windows.Forms.TextBox();
            this.WriteTDAReg = new System.Windows.Forms.Button();
            this.ReadTDAReg = new System.Windows.Forms.Button();
            this.Cold_Reset = new System.Windows.Forms.Button();
            this.Init_TDA8007 = new System.Windows.Forms.Button();
            this.tabPage_SPI_I2C = new System.Windows.Forms.TabPage();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.TransceiveDataSPI_I2C_listBox = new System.Windows.Forms.ListBox();
            this.AddtoListBox_SPI_I2C_btn = new System.Windows.Forms.Button();
            this.I2C_groupBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.I2C_LenToReceive_textBox = new System.Windows.Forms.TextBox();
            this.I2C_Send_btn = new System.Windows.Forms.Button();
            this.SPI_groupBox = new System.Windows.Forms.GroupBox();
            this.SPI_Send_btn = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.SPI_LenToReceive_textBox = new System.Windows.Forms.TextBox();
            this.tabPage_Script1 = new System.Windows.Forms.TabPage();
            this.button_script_reload = new System.Windows.Forms.Button();
            this.StopRunScr_simple = new System.Windows.Forms.Button();
            this.RunScr_simple = new System.Windows.Forms.Button();
            this.RunScrStep_simple = new System.Windows.Forms.Button();
            this.RstScrScope = new System.Windows.Forms.Button();
            this.Open_ScriptFile = new System.Windows.Forms.Button();
            this.dataGridView_Script = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Commands = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CompareData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReturnData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Results = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_Script = new System.Windows.Forms.TabPage();
            this.Script_backgroundWorker_progressBar = new System.Windows.Forms.ProgressBar();
            this.Stop_RunScript_btn = new System.Windows.Forms.Button();
            this.Scripts_code = new System.Windows.Forms.RichTextBox();
            this.Run_Script_btn = new System.Windows.Forms.Button();
            this.Open_Scr_dir = new System.Windows.Forms.Button();
            this.Scripts_list = new System.Windows.Forms.ListBox();
            this.lighttest = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.UID_modify = new System.Windows.Forms.CheckBox();
            this.checkClockTrim = new System.Windows.Forms.CheckBox();
            this.cmdNBFaka = new System.Windows.Forms.Button();
            this.NVM_encrypt = new System.Windows.Forms.CheckBox();
            this.fm347_cos = new System.Windows.Forms.CheckBox();
            this.NVM_init = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cc_dip = new System.Windows.Forms.RadioButton();
            this.cc_card = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cc_347 = new System.Windows.Forms.RadioButton();
            this.mp300_347 = new System.Windows.Forms.RadioButton();
            this.N_uid = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cos = new System.Windows.Forms.RadioButton();
            this.init = new System.Windows.Forms.RadioButton();
            this.auto_delay = new System.Windows.Forms.CheckBox();
            this.d_cos = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.temp_retain = new System.Windows.Forms.CheckBox();
            this.wr_config = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.high_temp = new System.Windows.Forms.RadioButton();
            this.low_temp = new System.Windows.Forms.RadioButton();
            this.temp_fm347 = new System.Windows.Forms.Button();
            this.user_lock_button = new System.Windows.Forms.Button();
            this.userfor_passport_button = new System.Windows.Forms.Button();
            this.cfg_and_progfor_passport_button = new System.Windows.Forms.Button();
            this.trimfor_passport_button = new System.Windows.Forms.Button();
            this.groupBox20 = new System.Windows.Forms.GroupBox();
            this.record_num = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.record_interface = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.record_card = new System.Windows.Forms.TextBox();
            this.label86 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.record_name = new System.Windows.Forms.TextBox();
            this.button13 = new System.Windows.Forms.Button();
            this.FM336_Endurance_button = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.bkpr = new System.Windows.Forms.RadioButton();
            this.bkpw = new System.Windows.Forms.RadioButton();
            this.bkp = new System.Windows.Forms.Button();
            this.auto_freq_test = new System.Windows.Forms.Button();
            this.cfgroup = new System.Windows.Forms.GroupBox();
            this.rston = new System.Windows.Forms.CheckBox();
            this.checkrlt = new System.Windows.Forms.CheckBox();
            this.cfwconfig = new System.Windows.Forms.CheckBox();
            this.cfw = new System.Windows.Forms.GroupBox();
            this.cfw_agn = new System.Windows.Forms.RadioButton();
            this.cfw_rnd = new System.Windows.Forms.RadioButton();
            this.prog_cos = new System.Windows.Forms.CheckBox();
            this.user_lock = new System.Windows.Forms.CheckBox();
            this.cfg = new System.Windows.Forms.GroupBox();
            this.seload = new System.Windows.Forms.RadioButton();
            this.fuload = new System.Windows.Forms.RadioButton();
            this.sjxm = new System.Windows.Forms.GroupBox();
            this.zc = new System.Windows.Forms.RadioButton();
            this.bank326 = new System.Windows.Forms.RadioButton();
            this.gm = new System.Windows.Forms.RadioButton();
            this.bright = new System.Windows.Forms.RadioButton();
            this.bank = new System.Windows.Forms.RadioButton();
            this.ty = new System.Windows.Forms.RadioButton();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.Scl = new System.Windows.Forms.RadioButton();
            this.Sct = new System.Windows.Forms.RadioButton();
            this.button8 = new System.Windows.Forms.Button();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.ITct = new System.Windows.Forms.RadioButton();
            this.ITcl = new System.Windows.Forms.RadioButton();
            this.label74 = new System.Windows.Forms.Label();
            this.ADcos = new System.Windows.Forms.TextBox();
            this.label73 = new System.Windows.Forms.Label();
            this.Akey = new System.Windows.Forms.TextBox();
            this.brst = new System.Windows.Forms.Button();
            this.temp_select = new System.Windows.Forms.ComboBox();
            this.FM294_str = new System.Windows.Forms.Button();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.FM336 = new System.Windows.Forms.RadioButton();
            this.FM309 = new System.Windows.Forms.RadioButton();
            this.FM326 = new System.Windows.Forms.RadioButton();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.init_n = new System.Windows.Forms.RadioButton();
            this.init_y = new System.Windows.Forms.RadioButton();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.temp_rlt = new System.Windows.Forms.RichTextBox();
            this.temp_start = new System.Windows.Forms.Button();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.tabPage_Program = new System.Windows.Forms.TabPage();
            this.retry = new System.Windows.Forms.CheckBox();
            this.pre_prog = new System.Windows.Forms.CheckBox();
            this.sec_addr = new System.Windows.Forms.ComboBox();
            this.chkReadVerify = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.FM347_radioButton = new System.Windows.Forms.RadioButton();
            this.FM326_radioButton = new System.Windows.Forms.RadioButton();
            this.FM302_radioButton = new System.Windows.Forms.RadioButton();
            this.FM350_radioButton = new System.Windows.Forms.RadioButton();
            this.FM274_radioButton = new System.Windows.Forms.RadioButton();
            this.FM294_radioButton = new System.Windows.Forms.RadioButton();
            this.FM349_radioButton = new System.Windows.Forms.RadioButton();
            this.FM309_radioButton = new System.Windows.Forms.RadioButton();
            this.FM336_radioButton = new System.Windows.Forms.RadioButton();
            this.groupBox_chips = new System.Windows.Forms.GroupBox();
            this.comboBox_ChipName = new System.Windows.Forms.ComboBox();
            this.label72 = new System.Windows.Forms.Label();
            this.label71 = new System.Windows.Forms.Label();
            this.WR_TIME = new System.Windows.Forms.TextBox();
            this.ER_TIME = new System.Windows.Forms.TextBox();
            this.label69 = new System.Windows.Forms.Label();
            this.ProgEEOffsetAddr = new System.Windows.Forms.TextBox();
            this.label67 = new System.Windows.Forms.Label();
            this.groupBox_350_Cfg = new System.Windows.Forms.GroupBox();
            this.label78 = new System.Windows.Forms.Label();
            this.label_terase_Calc = new System.Windows.Forms.Label();
            this.label_terase_unit = new System.Windows.Forms.Label();
            this.label_prepg_Calc = new System.Windows.Forms.Label();
            this.label76 = new System.Windows.Forms.Label();
            this.label_tprog_Calc = new System.Windows.Forms.Label();
            this.textBox_terase = new System.Windows.Forms.TextBox();
            this.textBox_prepg = new System.Windows.Forms.TextBox();
            this.label_terase = new System.Windows.Forms.Label();
            this.label_tprog_unit = new System.Windows.Forms.Label();
            this.label_tprog = new System.Windows.Forms.Label();
            this.textBox_tprog = new System.Windows.Forms.TextBox();
            this.label68 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.EE_patch_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.radioButton_0xFF = new System.Windows.Forms.RadioButton();
            this.radioButton_0x00 = new System.Windows.Forms.RadioButton();
            this.radioButton_0xB9 = new System.Windows.Forms.RadioButton();
            this.flash_check = new System.Windows.Forms.CheckBox();
            this.BufMove_checkBox = new System.Windows.Forms.CheckBox();
            this.checkBox_CTHighBaud = new System.Windows.Forms.CheckBox();
            this.Prog_Move = new System.Windows.Forms.Button();
            this.Card_init_Button = new System.Windows.Forms.Button();
            this.dest_label1 = new System.Windows.Forms.Label();
            this.ProgDestAddr = new System.Windows.Forms.TextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.key_richTextBox = new System.Windows.Forms.RichTextBox();
            this.keyl_label = new System.Windows.Forms.Label();
            this.InitData_comboBox = new System.Windows.Forms.ComboBox();
            this.Auth_checkBox = new System.Windows.Forms.CheckBox();
            this.label61 = new System.Windows.Forms.Label();
            this.lib_Size = new System.Windows.Forms.TextBox();
            this.label60 = new System.Windows.Forms.Label();
            this.flash_isp = new System.Windows.Forms.CheckBox();
            this.MEM_EE = new System.Windows.Forms.CheckBox();
            this.MEM_ROM = new System.Windows.Forms.CheckBox();
            this.SaveAs_verify = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.prog_extand_mode1 = new System.Windows.Forms.CheckBox();
            this.prog_extand_memRAM = new System.Windows.Forms.CheckBox();
            this.prog_extand_memEE = new System.Windows.Forms.CheckBox();
            this.prog_extand_mode0 = new System.Windows.Forms.CheckBox();
            this.SaveAsStart = new System.Windows.Forms.TextBox();
            this.SaveAsEnd = new System.Windows.Forms.TextBox();
            this.SaveAS_Button = new System.Windows.Forms.Button();
            this.A1Program = new System.Windows.Forms.CheckBox();
            this.ProgEE_progressBar = new System.Windows.Forms.ProgressBar();
            this.label47 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.Patch_Size = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.ProgEEStartAddr = new System.Windows.Forms.TextBox();
            this.ProgEEEndAddr = new System.Windows.Forms.TextBox();
            this.InitEEdata_Button = new System.Windows.Forms.Button();
            this.ProgEE_Button = new System.Windows.Forms.Button();
            this.OpenFile_Button = new System.Windows.Forms.Button();
            this.MemTypeSelGrop = new System.Windows.Forms.GroupBox();
            this.RAMtype_radiobutton = new System.Windows.Forms.RadioButton();
            this.PROG_EEtype_radiobutton = new System.Windows.Forms.RadioButton();
            this.DATA_EEtype_radiobutton = new System.Windows.Forms.RadioButton();
            this.groupBoxOperation = new System.Windows.Forms.GroupBox();
            this.textBox_LibSize = new System.Windows.Forms.TextBox();
            this.label_LibSizeUnit = new System.Windows.Forms.Label();
            this.checkBox_SeperateLibProg = new System.Windows.Forms.CheckBox();
            this.comboBox_CTBaud = new System.Windows.Forms.ComboBox();
            this.InitCRC16_texbox = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.SaveReadbuf = new System.Windows.Forms.CheckBox();
            this.ReadEEdata_Button = new System.Windows.Forms.Button();
            this.label54 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.CB_NoErase = new System.Windows.Forms.CheckBox();
            this.checkBox_350ChipErase = new System.Windows.Forms.CheckBox();
            this.checkBox_ignore9800 = new System.Windows.Forms.CheckBox();
            this.interface_sel_grpbox = new System.Windows.Forms.GroupBox();
            this.CL_Interface = new System.Windows.Forms.RadioButton();
            this.CT_Interface = new System.Windows.Forms.RadioButton();
            this.progEEdataGridView = new System.Windows.Forms.DataGridView();
            this.EE_Addr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EE_Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.Trim_init = new System.Windows.Forms.CheckBox();
            this.Chip_comboBox = new System.Windows.Forms.ComboBox();
            this.FreqRead_radioButton = new System.Windows.Forms.RadioButton();
            this.FreqTrim_radioButton = new System.Windows.Forms.RadioButton();
            this.DfsCFG_comboBox1 = new System.Windows.Forms.ComboBox();
            this.freq_cos_check = new System.Windows.Forms.CheckBox();
            this.Freq_trim_button = new System.Windows.Forms.Button();
            this.FMXX_config = new System.Windows.Forms.TabPage();
            this.label62 = new System.Windows.Forms.Label();
            this.keylegth_label = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.CWD_auth_checkbox = new System.Windows.Forms.CheckBox();
            this.CfgWordWrite_chkbox = new System.Windows.Forms.CheckBox();
            this.DetectCfgWord_btn = new System.Windows.Forms.Button();
            this.CfgWord_Parse_btn = new System.Windows.Forms.Button();
            this.CfgWd_Gen_button = new System.Windows.Forms.Button();
            this.CfgWord_dataGridView = new System.Windows.Forms.DataGridView();
            this.new_config = new System.Windows.Forms.TabPage();
            this.CWD_CMP_checkBox = new System.Windows.Forms.CheckBox();
            this.button12 = new System.Windows.Forms.Button();
            this.CB_DisturbDoubleCheck = new System.Windows.Forms.CheckBox();
            this.label70 = new System.Windows.Forms.Label();
            this.nvrDisturbParam = new System.Windows.Forms.TextBox();
            this.CB_AutoReverse = new System.Windows.Forms.CheckBox();
            this.saveascoe__button = new System.Windows.Forms.Button();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.button_ReverseOption = new System.Windows.Forms.Button();
            this.CB_EnableOption = new System.Windows.Forms.CheckBox();
            this.CB_NVRALL_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR6_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR5_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR4_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR3_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR2_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR1_SEL = new System.Windows.Forms.CheckBox();
            this.CB_NVR0_SEL = new System.Windows.Forms.CheckBox();
            this.CB_WordRegEn = new System.Windows.Forms.CheckBox();
            this.FMXX_SEL_CFGcomboBox = new System.Windows.Forms.ComboBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.CL_newcfg_radioButton = new System.Windows.Forms.RadioButton();
            this.CT_newcfg_radioButton = new System.Windows.Forms.RadioButton();
            this.WriteCWD_button = new System.Windows.Forms.Button();
            this.GenerateCWD_button = new System.Windows.Forms.Button();
            this.AnalysisCWD_button = new System.Windows.Forms.Button();
            this.ReadCWD_button = new System.Windows.Forms.Button();
            this.cfg_dataGridView = new System.Windows.Forms.DataGridView();
            this.folderBrowserDialog_Scr = new System.Windows.Forms.FolderBrowserDialog();
            this.SendData_textBox = new System.Windows.Forms.TextBox();
            this.SendData_label = new System.Windows.Forms.Label();
            this.openFileDialog_Scr = new System.Windows.Forms.OpenFileDialog();
            this.SimpleScript_backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.Script_backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.Hints_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.swp_slave_autorx_timer = new System.Windows.Forms.Timer(this.components);
            this.timswp_master_autorx = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog_Prog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip_CLIterface = new System.Windows.Forms.ToolTip(this.components);
            this.main_note_cMS.SuspendLayout();
            this.tabControl_main.SuspendLayout();
            this.tabPage_CL.SuspendLayout();
            this.groupBox_MIFARE.SuspendLayout();
            this.cla_parity_groupbox.SuspendLayout();
            this.CLTypeSelect_groupBox.SuspendLayout();
            this.CRC_gropbox.SuspendLayout();
            this.groupBox_FM17Reg.SuspendLayout();
            this.groupBox_CLA_CONTROL.SuspendLayout();
            this.groupBox_CLB_CONTROL.SuspendLayout();
            this.tabPage_CT.SuspendLayout();
            this.groupBox21.SuspendLayout();
            this.ins54.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.Parity_groupBox.SuspendLayout();
            this.TR_CT_TimeOut.SuspendLayout();
            this.CST_select.SuspendLayout();
            this.VoltageSel.SuspendLayout();
            this.tabPage_SPI_I2C.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.I2C_groupBox.SuspendLayout();
            this.SPI_groupBox.SuspendLayout();
            this.tabPage_Script1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Script)).BeginInit();
            this.tabPage_Script.SuspendLayout();
            this.lighttest.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox20.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.cfgroup.SuspendLayout();
            this.cfw.SuspendLayout();
            this.cfg.SuspendLayout();
            this.sjxm.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.tabPage_Program.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox_chips.SuspendLayout();
            this.groupBox_350_Cfg.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.MemTypeSelGrop.SuspendLayout();
            this.groupBoxOperation.SuspendLayout();
            this.interface_sel_grpbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progEEdataGridView)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.FMXX_config.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CfgWord_dataGridView)).BeginInit();
            this.new_config.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cfg_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // main_note
            // 
            this.main_note.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.main_note.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.main_note.ContextMenuStrip = this.main_note_cMS;
            this.main_note.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.main_note.Location = new System.Drawing.Point(0, 470);
            this.main_note.Name = "main_note";
            this.main_note.Size = new System.Drawing.Size(961, 256);
            this.main_note.TabIndex = 2;
            this.main_note.Text = "";
            this.main_note.TextChanged += new System.EventHandler(this.main_note_TextChanged);
            // 
            // main_note_cMS
            // 
            this.main_note_cMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.main_note_cMS.Name = "main_note_cMS";
            this.main_note_cMS.Size = new System.Drawing.Size(107, 26);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // tabControl_main
            // 
            this.tabControl_main.Controls.Add(this.tabPage_CL);
            this.tabControl_main.Controls.Add(this.tabPage_CT);
            this.tabControl_main.Controls.Add(this.tabPage_SPI_I2C);
            this.tabControl_main.Controls.Add(this.tabPage_Script1);
            this.tabControl_main.Controls.Add(this.tabPage_Script);
            this.tabControl_main.Controls.Add(this.lighttest);
            this.tabControl_main.Controls.Add(this.tabPage_Program);
            this.tabControl_main.Controls.Add(this.FMXX_config);
            this.tabControl_main.Controls.Add(this.new_config);
            this.tabControl_main.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl_main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_main.Name = "tabControl_main";
            this.tabControl_main.SelectedIndex = 0;
            this.tabControl_main.Size = new System.Drawing.Size(964, 432);
            this.tabControl_main.TabIndex = 14;
            // 
            // tabPage_CL
            // 
            this.tabPage_CL.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_CL.Controls.Add(this.btnWordXOR);
            this.tabPage_CL.Controls.Add(this.XOR_button);
            this.tabPage_CL.Controls.Add(this.M1_Visible_checkBox);
            this.tabPage_CL.Controls.Add(this.groupBox_MIFARE);
            this.tabPage_CL.Controls.Add(this.Send_Selected_Command_CL_button);
            this.tabPage_CL.Controls.Add(this.Open_CMDlog_button);
            this.tabPage_CL.Controls.Add(this.Access_bits_textBox);
            this.tabPage_CL.Controls.Add(this.parse_access_bits_button);
            this.tabPage_CL.Controls.Add(this.label84);
            this.tabPage_CL.Controls.Add(this.label83);
            this.tabPage_CL.Controls.Add(this.label82);
            this.tabPage_CL.Controls.Add(this.label81);
            this.tabPage_CL.Controls.Add(this.label80);
            this.tabPage_CL.Controls.Add(this.label79);
            this.tabPage_CL.Controls.Add(this.label77);
            this.tabPage_CL.Controls.Add(this.C3_3_textBox);
            this.tabPage_CL.Controls.Add(this.C2_3_textBox);
            this.tabPage_CL.Controls.Add(this.C1_3_textBox);
            this.tabPage_CL.Controls.Add(this.C3_2_textBox);
            this.tabPage_CL.Controls.Add(this.C2_2_textBox);
            this.tabPage_CL.Controls.Add(this.C1_2_textBox);
            this.tabPage_CL.Controls.Add(this.C3_1_textBox);
            this.tabPage_CL.Controls.Add(this.C2_1_textBox);
            this.tabPage_CL.Controls.Add(this.C1_1_textBox);
            this.tabPage_CL.Controls.Add(this.C3_0_textBox);
            this.tabPage_CL.Controls.Add(this.C2_0_textBox);
            this.tabPage_CL.Controls.Add(this.C1_0_textBox);
            this.tabPage_CL.Controls.Add(this.Cal_access_bits_button);
            this.tabPage_CL.Controls.Add(this.buttonASCIIToString);
            this.tabPage_CL.Controls.Add(this.buttonStringToASCII);
            this.tabPage_CL.Controls.Add(this.button_AppendCRC16);
            this.tabPage_CL.Controls.Add(this.label75);
            this.tabPage_CL.Controls.Add(this.textBox_CRC);
            this.tabPage_CL.Controls.Add(this.button_CRC);
            this.tabPage_CL.Controls.Add(this.cla_parity_groupbox);
            this.tabPage_CL.Controls.Add(this.textBox_APDU_Le_CL);
            this.tabPage_CL.Controls.Add(this.LeText);
            this.tabPage_CL.Controls.Add(this.GetReaders_bnt);
            this.tabPage_CL.Controls.Add(this.readers_cbox);
            this.tabPage_CL.Controls.Add(this.label57);
            this.tabPage_CL.Controls.Add(this.flash_isp_comSelect);
            this.tabPage_CL.Controls.Add(this.version_check);
            this.tabPage_CL.Controls.Add(this.label51);
            this.tabPage_CL.Controls.Add(this.ResFreqCacu_L_btn);
            this.tabPage_CL.Controls.Add(this.ResFreqCacu_C_btn);
            this.tabPage_CL.Controls.Add(this.ResFreqCacu_F_btn);
            this.tabPage_CL.Controls.Add(this.ResonFreqCaculate_F_tB);
            this.tabPage_CL.Controls.Add(this.ResonFreqCaculate_L_tB);
            this.tabPage_CL.Controls.Add(this.ResonFreqCaculate_C_tB);
            this.tabPage_CL.Controls.Add(this.CLTypeSelect_groupBox);
            this.tabPage_CL.Controls.Add(this.TransceiveDataCL_listBox);
            this.tabPage_CL.Controls.Add(this.AddCMDtoListBox_CL_btn);
            this.tabPage_CL.Controls.Add(this.checkBox_APDUmodeCL);
            this.tabPage_CL.Controls.Add(this.label_APDU_CL);
            this.tabPage_CL.Controls.Add(this.textBox_APDU_P3_CL);
            this.tabPage_CL.Controls.Add(this.textBox_APDU_P2_CL);
            this.tabPage_CL.Controls.Add(this.textBox_APDU_P1_CL);
            this.tabPage_CL.Controls.Add(this.textBox_APDU_INS_CL);
            this.tabPage_CL.Controls.Add(this.textBox_APDU_CLA_CL);
            this.tabPage_CL.Controls.Add(this.CardType_Select);
            this.tabPage_CL.Controls.Add(this.Field_ON);
            this.tabPage_CL.Controls.Add(this.ConnectReader);
            this.tabPage_CL.Controls.Add(this.label5);
            this.tabPage_CL.Controls.Add(this.TimeOut_textBox);
            this.tabPage_CL.Controls.Add(this.Reset17);
            this.tabPage_CL.Controls.Add(this.CRC_gropbox);
            this.tabPage_CL.Controls.Add(this.TransceiveCL);
            this.tabPage_CL.Controls.Add(this.groupBox_FM17Reg);
            this.tabPage_CL.Controls.Add(this.groupBox_CLA_CONTROL);
            this.tabPage_CL.Controls.Add(this.groupBox_CLB_CONTROL);
            this.tabPage_CL.Location = new System.Drawing.Point(4, 22);
            this.tabPage_CL.Name = "tabPage_CL";
            this.tabPage_CL.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_CL.Size = new System.Drawing.Size(956, 406);
            this.tabPage_CL.TabIndex = 0;
            this.tabPage_CL.Text = "非接触接口";
            this.tabPage_CL.DoubleClick += new System.EventHandler(this.blkaddrDblClick);
            // 
            // btnWordXOR
            // 
            this.btnWordXOR.Location = new System.Drawing.Point(725, 368);
            this.btnWordXOR.Name = "btnWordXOR";
            this.btnWordXOR.Size = new System.Drawing.Size(75, 23);
            this.btnWordXOR.TabIndex = 181;
            this.btnWordXOR.Text = "按字异或";
            this.btnWordXOR.UseVisualStyleBackColor = true;
            this.btnWordXOR.Click += new System.EventHandler(this.btnWordXOR_Click);
            // 
            // XOR_button
            // 
            this.XOR_button.Location = new System.Drawing.Point(725, 339);
            this.XOR_button.Name = "XOR_button";
            this.XOR_button.Size = new System.Drawing.Size(75, 23);
            this.XOR_button.TabIndex = 180;
            this.XOR_button.Text = "异或";
            this.XOR_button.UseVisualStyleBackColor = true;
            this.XOR_button.Click += new System.EventHandler(this.XOR_button_Click);
            // 
            // M1_Visible_checkBox
            // 
            this.M1_Visible_checkBox.AutoSize = true;
            this.M1_Visible_checkBox.Location = new System.Drawing.Point(328, 101);
            this.M1_Visible_checkBox.Name = "M1_Visible_checkBox";
            this.M1_Visible_checkBox.Size = new System.Drawing.Size(84, 16);
            this.M1_Visible_checkBox.TabIndex = 179;
            this.M1_Visible_checkBox.Text = "逻辑加密卡";
            this.M1_Visible_checkBox.UseVisualStyleBackColor = true;
            this.M1_Visible_checkBox.CheckedChanged += new System.EventHandler(this.M1_Visible_checkBox_CheckedChanged);
            // 
            // groupBox_MIFARE
            // 
            this.groupBox_MIFARE.Controls.Add(this.auth_uidlevel_textBox);
            this.groupBox_MIFARE.Controls.Add(this.label11);
            this.groupBox_MIFARE.Controls.Add(this.label10);
            this.groupBox_MIFARE.Controls.Add(this.AuthBlock_textBox);
            this.groupBox_MIFARE.Controls.Add(this.buttonRESTORE);
            this.groupBox_MIFARE.Controls.Add(this.buttonTRANSFER);
            this.groupBox_MIFARE.Controls.Add(this.buttonINCValue);
            this.groupBox_MIFARE.Controls.Add(this.buttonDECValue);
            this.groupBox_MIFARE.Controls.Add(this.AuthType_comboBox);
            this.groupBox_MIFARE.Controls.Add(this.buttonReadValue);
            this.groupBox_MIFARE.Controls.Add(this.label66);
            this.groupBox_MIFARE.Controls.Add(this.KeyMode_comboBox);
            this.groupBox_MIFARE.Controls.Add(this.AuthKey_textBox);
            this.groupBox_MIFARE.Controls.Add(this.buttonWriteValue);
            this.groupBox_MIFARE.Controls.Add(this.Ldkey_button);
            this.groupBox_MIFARE.Controls.Add(this.Auth_button);
            this.groupBox_MIFARE.Controls.Add(this.buttonReadBlock);
            this.groupBox_MIFARE.Controls.Add(this.buttonWriteBlock);
            this.groupBox_MIFARE.Controls.Add(this.BlockAddr_textBox);
            this.groupBox_MIFARE.Controls.Add(this.label4);
            this.groupBox_MIFARE.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_MIFARE.Location = new System.Drawing.Point(319, 2);
            this.groupBox_MIFARE.Name = "groupBox_MIFARE";
            this.groupBox_MIFARE.Size = new System.Drawing.Size(433, 93);
            this.groupBox_MIFARE.TabIndex = 105;
            this.groupBox_MIFARE.TabStop = false;
            this.groupBox_MIFARE.Text = "逻辑加密卡认证和操作";
            this.groupBox_MIFARE.Visible = false;
            // 
            // auth_uidlevel_textBox
            // 
            this.auth_uidlevel_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.auth_uidlevel_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.auth_uidlevel_textBox.Location = new System.Drawing.Point(165, 42);
            this.auth_uidlevel_textBox.MaxLength = 2;
            this.auth_uidlevel_textBox.Name = "auth_uidlevel_textBox";
            this.auth_uidlevel_textBox.Size = new System.Drawing.Size(26, 23);
            this.auth_uidlevel_textBox.TabIndex = 119;
            this.auth_uidlevel_textBox.Text = "1";
            this.auth_uidlevel_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(157, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 14);
            this.label11.TabIndex = 118;
            this.label11.Text = "level";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(159, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 14);
            this.label10.TabIndex = 117;
            this.label10.Text = "认证";
            // 
            // AuthBlock_textBox
            // 
            this.AuthBlock_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.AuthBlock_textBox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AuthBlock_textBox.Location = new System.Drawing.Point(21, 16);
            this.AuthBlock_textBox.MaxLength = 2;
            this.AuthBlock_textBox.Name = "AuthBlock_textBox";
            this.AuthBlock_textBox.Size = new System.Drawing.Size(22, 21);
            this.AuthBlock_textBox.TabIndex = 102;
            this.AuthBlock_textBox.Text = "00";
            this.AuthBlock_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AuthBlock_textBox.DoubleClick += new System.EventHandler(this.blkaddrDblClick);
            // 
            // buttonRESTORE
            // 
            this.buttonRESTORE.Location = new System.Drawing.Point(167, 67);
            this.buttonRESTORE.Name = "buttonRESTORE";
            this.buttonRESTORE.Size = new System.Drawing.Size(64, 20);
            this.buttonRESTORE.TabIndex = 115;
            this.buttonRESTORE.Text = "RESTORE";
            this.buttonRESTORE.UseVisualStyleBackColor = true;
            this.buttonRESTORE.Click += new System.EventHandler(this.buttonRESTORE_Click);
            // 
            // buttonTRANSFER
            // 
            this.buttonTRANSFER.Location = new System.Drawing.Point(233, 67);
            this.buttonTRANSFER.Name = "buttonTRANSFER";
            this.buttonTRANSFER.Size = new System.Drawing.Size(65, 20);
            this.buttonTRANSFER.TabIndex = 116;
            this.buttonTRANSFER.Text = "TRANSFER";
            this.buttonTRANSFER.UseVisualStyleBackColor = true;
            this.buttonTRANSFER.Click += new System.EventHandler(this.buttonTRANSFER_Click);
            // 
            // buttonINCValue
            // 
            this.buttonINCValue.Location = new System.Drawing.Point(299, 67);
            this.buttonINCValue.Name = "buttonINCValue";
            this.buttonINCValue.Size = new System.Drawing.Size(63, 20);
            this.buttonINCValue.TabIndex = 113;
            this.buttonINCValue.Text = "INCValue";
            this.buttonINCValue.UseVisualStyleBackColor = true;
            this.buttonINCValue.Click += new System.EventHandler(this.IncValue_Click);
            // 
            // buttonDECValue
            // 
            this.buttonDECValue.Location = new System.Drawing.Point(364, 67);
            this.buttonDECValue.Name = "buttonDECValue";
            this.buttonDECValue.Size = new System.Drawing.Size(62, 20);
            this.buttonDECValue.TabIndex = 114;
            this.buttonDECValue.Text = "DECValue";
            this.buttonDECValue.UseVisualStyleBackColor = true;
            this.buttonDECValue.Click += new System.EventHandler(this.DecValue_Click);
            // 
            // AuthType_comboBox
            // 
            this.AuthType_comboBox.FormattingEnabled = true;
            this.AuthType_comboBox.Items.AddRange(new object[] {
            "Mifare",
            "SH"});
            this.AuthType_comboBox.Location = new System.Drawing.Point(45, 17);
            this.AuthType_comboBox.Name = "AuthType_comboBox";
            this.AuthType_comboBox.Size = new System.Drawing.Size(60, 20);
            this.AuthType_comboBox.TabIndex = 104;
            this.AuthType_comboBox.Text = "Mifare";
            // 
            // buttonReadValue
            // 
            this.buttonReadValue.Location = new System.Drawing.Point(336, 13);
            this.buttonReadValue.Name = "buttonReadValue";
            this.buttonReadValue.Size = new System.Drawing.Size(90, 24);
            this.buttonReadValue.TabIndex = 108;
            this.buttonReadValue.Text = "ReadValue";
            this.buttonReadValue.UseVisualStyleBackColor = true;
            this.buttonReadValue.Click += new System.EventHandler(this.ReadValue_Click);
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label66.Location = new System.Drawing.Point(4, 20);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(17, 12);
            this.label66.TabIndex = 103;
            this.label66.Text = "0x";
            // 
            // KeyMode_comboBox
            // 
            this.KeyMode_comboBox.FormattingEnabled = true;
            this.KeyMode_comboBox.Items.AddRange(new object[] {
            "KeyA",
            "KeyB"});
            this.KeyMode_comboBox.Location = new System.Drawing.Point(108, 17);
            this.KeyMode_comboBox.Name = "KeyMode_comboBox";
            this.KeyMode_comboBox.Size = new System.Drawing.Size(49, 20);
            this.KeyMode_comboBox.TabIndex = 101;
            this.KeyMode_comboBox.Text = "KeyA";
            // 
            // AuthKey_textBox
            // 
            this.AuthKey_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.AuthKey_textBox.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AuthKey_textBox.Location = new System.Drawing.Point(5, 39);
            this.AuthKey_textBox.MaxLength = 22;
            this.AuthKey_textBox.Name = "AuthKey_textBox";
            this.AuthKey_textBox.Size = new System.Drawing.Size(152, 24);
            this.AuthKey_textBox.TabIndex = 100;
            this.AuthKey_textBox.Text = "FF FF FF FF FF FF";
            this.AuthKey_textBox.TextChanged += new System.EventHandler(this.AuthKey_textBox_TextChanged);
            // 
            // buttonWriteValue
            // 
            this.buttonWriteValue.Location = new System.Drawing.Point(336, 40);
            this.buttonWriteValue.Name = "buttonWriteValue";
            this.buttonWriteValue.Size = new System.Drawing.Size(90, 25);
            this.buttonWriteValue.TabIndex = 109;
            this.buttonWriteValue.Text = "WriteValue";
            this.buttonWriteValue.UseVisualStyleBackColor = true;
            this.buttonWriteValue.Click += new System.EventHandler(this.InitValue_Click);
            // 
            // Ldkey_button
            // 
            this.Ldkey_button.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Ldkey_button.Location = new System.Drawing.Point(6, 63);
            this.Ldkey_button.Name = "Ldkey_button";
            this.Ldkey_button.Size = new System.Drawing.Size(75, 23);
            this.Ldkey_button.TabIndex = 99;
            this.Ldkey_button.Text = "LoadKey";
            this.Ldkey_button.UseVisualStyleBackColor = true;
            this.Ldkey_button.Click += new System.EventHandler(this.Ldkey_button_Click);
            // 
            // Auth_button
            // 
            this.Auth_button.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Auth_button.Location = new System.Drawing.Point(83, 64);
            this.Auth_button.Name = "Auth_button";
            this.Auth_button.Size = new System.Drawing.Size(75, 23);
            this.Auth_button.TabIndex = 98;
            this.Auth_button.Text = "Auth";
            this.Auth_button.UseVisualStyleBackColor = true;
            this.Auth_button.Click += new System.EventHandler(this.Auth_button_Click);
            // 
            // buttonReadBlock
            // 
            this.buttonReadBlock.Location = new System.Drawing.Point(198, 13);
            this.buttonReadBlock.Name = "buttonReadBlock";
            this.buttonReadBlock.Size = new System.Drawing.Size(87, 24);
            this.buttonReadBlock.TabIndex = 23;
            this.buttonReadBlock.Text = "ReadBlock";
            this.buttonReadBlock.UseVisualStyleBackColor = true;
            this.buttonReadBlock.Click += new System.EventHandler(this.ReadBlock_Click);
            // 
            // buttonWriteBlock
            // 
            this.buttonWriteBlock.Location = new System.Drawing.Point(198, 40);
            this.buttonWriteBlock.Name = "buttonWriteBlock";
            this.buttonWriteBlock.Size = new System.Drawing.Size(87, 25);
            this.buttonWriteBlock.TabIndex = 22;
            this.buttonWriteBlock.Text = "WriteBlock";
            this.buttonWriteBlock.UseVisualStyleBackColor = true;
            this.buttonWriteBlock.Click += new System.EventHandler(this.WriteBlock_Click);
            // 
            // BlockAddr_textBox
            // 
            this.BlockAddr_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.BlockAddr_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BlockAddr_textBox.Location = new System.Drawing.Point(306, 31);
            this.BlockAddr_textBox.MaxLength = 2;
            this.BlockAddr_textBox.Name = "BlockAddr_textBox";
            this.BlockAddr_textBox.Size = new System.Drawing.Size(26, 23);
            this.BlockAddr_textBox.TabIndex = 30;
            this.BlockAddr_textBox.Text = "00";
            this.BlockAddr_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.BlockAddr_textBox.DoubleClick += new System.EventHandler(this.blkaddrDblClick);
            this.BlockAddr_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BlockAddr_KeyPress);
            this.BlockAddr_textBox.Leave += new System.EventHandler(this.BlockAddr_textBox_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(284, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 14);
            this.label4.TabIndex = 32;
            this.label4.Text = "0x";
            // 
            // Send_Selected_Command_CL_button
            // 
            this.Send_Selected_Command_CL_button.Location = new System.Drawing.Point(349, 256);
            this.Send_Selected_Command_CL_button.Name = "Send_Selected_Command_CL_button";
            this.Send_Selected_Command_CL_button.Size = new System.Drawing.Size(75, 44);
            this.Send_Selected_Command_CL_button.TabIndex = 178;
            this.Send_Selected_Command_CL_button.Text = "顺序执行";
            this.Send_Selected_Command_CL_button.UseVisualStyleBackColor = true;
            this.Send_Selected_Command_CL_button.Click += new System.EventHandler(this.Send_Slected_Command_button_Click);
            // 
            // Open_CMDlog_button
            // 
            this.Open_CMDlog_button.Location = new System.Drawing.Point(349, 305);
            this.Open_CMDlog_button.Name = "Open_CMDlog_button";
            this.Open_CMDlog_button.Size = new System.Drawing.Size(75, 45);
            this.Open_CMDlog_button.TabIndex = 177;
            this.Open_CMDlog_button.Text = "OPEN";
            this.Open_CMDlog_button.UseVisualStyleBackColor = true;
            this.Open_CMDlog_button.Click += new System.EventHandler(this.Open_CMDlog_button_Click);
            // 
            // Access_bits_textBox
            // 
            this.Access_bits_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.Access_bits_textBox.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Access_bits_textBox.Location = new System.Drawing.Point(806, 189);
            this.Access_bits_textBox.MaxLength = 22;
            this.Access_bits_textBox.Name = "Access_bits_textBox";
            this.Access_bits_textBox.Size = new System.Drawing.Size(78, 24);
            this.Access_bits_textBox.TabIndex = 176;
            this.Access_bits_textBox.Text = "FF078069";
            // 
            // parse_access_bits_button
            // 
            this.parse_access_bits_button.Location = new System.Drawing.Point(809, 151);
            this.parse_access_bits_button.Name = "parse_access_bits_button";
            this.parse_access_bits_button.Size = new System.Drawing.Size(75, 23);
            this.parse_access_bits_button.TabIndex = 175;
            this.parse_access_bits_button.Text = "解析权限字";
            this.parse_access_bits_button.UseVisualStyleBackColor = true;
            this.parse_access_bits_button.Click += new System.EventHandler(this.parse_access_bits_button_Click);
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label84.Location = new System.Drawing.Point(779, 107);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(21, 14);
            this.label84.TabIndex = 174;
            this.label84.Text = "C3";
            // 
            // label83
            // 
            this.label83.AutoSize = true;
            this.label83.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label83.Location = new System.Drawing.Point(754, 107);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(21, 14);
            this.label83.TabIndex = 173;
            this.label83.Text = "C2";
            // 
            // label82
            // 
            this.label82.AutoSize = true;
            this.label82.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label82.Location = new System.Drawing.Point(728, 107);
            this.label82.Name = "label82";
            this.label82.Size = new System.Drawing.Size(21, 14);
            this.label82.TabIndex = 172;
            this.label82.Text = "C1";
            // 
            // label81
            // 
            this.label81.AutoSize = true;
            this.label81.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label81.Location = new System.Drawing.Point(705, 195);
            this.label81.Name = "label81";
            this.label81.Size = new System.Drawing.Size(14, 14);
            this.label81.TabIndex = 171;
            this.label81.Text = "3";
            // 
            // label80
            // 
            this.label80.AutoSize = true;
            this.label80.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label80.Location = new System.Drawing.Point(705, 175);
            this.label80.Name = "label80";
            this.label80.Size = new System.Drawing.Size(14, 14);
            this.label80.TabIndex = 170;
            this.label80.Text = "2";
            // 
            // label79
            // 
            this.label79.AutoSize = true;
            this.label79.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label79.Location = new System.Drawing.Point(705, 153);
            this.label79.Name = "label79";
            this.label79.Size = new System.Drawing.Size(14, 14);
            this.label79.TabIndex = 169;
            this.label79.Text = "1";
            // 
            // label77
            // 
            this.label77.AutoSize = true;
            this.label77.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label77.Location = new System.Drawing.Point(705, 127);
            this.label77.Name = "label77";
            this.label77.Size = new System.Drawing.Size(14, 14);
            this.label77.TabIndex = 115;
            this.label77.Text = "0";
            // 
            // C3_3_textBox
            // 
            this.C3_3_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C3_3_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C3_3_textBox.Location = new System.Drawing.Point(777, 190);
            this.C3_3_textBox.MaxLength = 1;
            this.C3_3_textBox.Name = "C3_3_textBox";
            this.C3_3_textBox.Size = new System.Drawing.Size(26, 23);
            this.C3_3_textBox.TabIndex = 168;
            this.C3_3_textBox.Text = "0";
            this.C3_3_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C2_3_textBox
            // 
            this.C2_3_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C2_3_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C2_3_textBox.Location = new System.Drawing.Point(751, 190);
            this.C2_3_textBox.MaxLength = 1;
            this.C2_3_textBox.Name = "C2_3_textBox";
            this.C2_3_textBox.Size = new System.Drawing.Size(26, 23);
            this.C2_3_textBox.TabIndex = 167;
            this.C2_3_textBox.Text = "0";
            this.C2_3_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C1_3_textBox
            // 
            this.C1_3_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C1_3_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C1_3_textBox.Location = new System.Drawing.Point(725, 190);
            this.C1_3_textBox.MaxLength = 1;
            this.C1_3_textBox.Name = "C1_3_textBox";
            this.C1_3_textBox.Size = new System.Drawing.Size(26, 23);
            this.C1_3_textBox.TabIndex = 166;
            this.C1_3_textBox.Text = "0";
            this.C1_3_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C3_2_textBox
            // 
            this.C3_2_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C3_2_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C3_2_textBox.Location = new System.Drawing.Point(777, 168);
            this.C3_2_textBox.MaxLength = 1;
            this.C3_2_textBox.Name = "C3_2_textBox";
            this.C3_2_textBox.Size = new System.Drawing.Size(26, 23);
            this.C3_2_textBox.TabIndex = 165;
            this.C3_2_textBox.Text = "0";
            this.C3_2_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C2_2_textBox
            // 
            this.C2_2_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C2_2_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C2_2_textBox.Location = new System.Drawing.Point(751, 168);
            this.C2_2_textBox.MaxLength = 1;
            this.C2_2_textBox.Name = "C2_2_textBox";
            this.C2_2_textBox.Size = new System.Drawing.Size(26, 23);
            this.C2_2_textBox.TabIndex = 164;
            this.C2_2_textBox.Text = "0";
            this.C2_2_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C1_2_textBox
            // 
            this.C1_2_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C1_2_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C1_2_textBox.Location = new System.Drawing.Point(725, 168);
            this.C1_2_textBox.MaxLength = 1;
            this.C1_2_textBox.Name = "C1_2_textBox";
            this.C1_2_textBox.Size = new System.Drawing.Size(26, 23);
            this.C1_2_textBox.TabIndex = 163;
            this.C1_2_textBox.Text = "0";
            this.C1_2_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C3_1_textBox
            // 
            this.C3_1_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C3_1_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C3_1_textBox.Location = new System.Drawing.Point(777, 145);
            this.C3_1_textBox.MaxLength = 1;
            this.C3_1_textBox.Name = "C3_1_textBox";
            this.C3_1_textBox.Size = new System.Drawing.Size(26, 23);
            this.C3_1_textBox.TabIndex = 162;
            this.C3_1_textBox.Text = "0";
            this.C3_1_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C2_1_textBox
            // 
            this.C2_1_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C2_1_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C2_1_textBox.Location = new System.Drawing.Point(751, 145);
            this.C2_1_textBox.MaxLength = 1;
            this.C2_1_textBox.Name = "C2_1_textBox";
            this.C2_1_textBox.Size = new System.Drawing.Size(26, 23);
            this.C2_1_textBox.TabIndex = 161;
            this.C2_1_textBox.Text = "0";
            this.C2_1_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C1_1_textBox
            // 
            this.C1_1_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C1_1_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C1_1_textBox.Location = new System.Drawing.Point(725, 145);
            this.C1_1_textBox.MaxLength = 1;
            this.C1_1_textBox.Name = "C1_1_textBox";
            this.C1_1_textBox.Size = new System.Drawing.Size(26, 23);
            this.C1_1_textBox.TabIndex = 160;
            this.C1_1_textBox.Text = "0";
            this.C1_1_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C3_0_textBox
            // 
            this.C3_0_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C3_0_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C3_0_textBox.Location = new System.Drawing.Point(777, 123);
            this.C3_0_textBox.MaxLength = 1;
            this.C3_0_textBox.Name = "C3_0_textBox";
            this.C3_0_textBox.Size = new System.Drawing.Size(26, 23);
            this.C3_0_textBox.TabIndex = 159;
            this.C3_0_textBox.Text = "0";
            this.C3_0_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C2_0_textBox
            // 
            this.C2_0_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C2_0_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C2_0_textBox.Location = new System.Drawing.Point(751, 123);
            this.C2_0_textBox.MaxLength = 1;
            this.C2_0_textBox.Name = "C2_0_textBox";
            this.C2_0_textBox.Size = new System.Drawing.Size(26, 23);
            this.C2_0_textBox.TabIndex = 158;
            this.C2_0_textBox.Text = "0";
            this.C2_0_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // C1_0_textBox
            // 
            this.C1_0_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.C1_0_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.C1_0_textBox.Location = new System.Drawing.Point(725, 123);
            this.C1_0_textBox.MaxLength = 1;
            this.C1_0_textBox.Name = "C1_0_textBox";
            this.C1_0_textBox.Size = new System.Drawing.Size(26, 23);
            this.C1_0_textBox.TabIndex = 157;
            this.C1_0_textBox.Text = "0";
            this.C1_0_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Cal_access_bits_button
            // 
            this.Cal_access_bits_button.Location = new System.Drawing.Point(809, 127);
            this.Cal_access_bits_button.Name = "Cal_access_bits_button";
            this.Cal_access_bits_button.Size = new System.Drawing.Size(75, 23);
            this.Cal_access_bits_button.TabIndex = 115;
            this.Cal_access_bits_button.Text = "生成权限字";
            this.Cal_access_bits_button.UseVisualStyleBackColor = true;
            this.Cal_access_bits_button.Click += new System.EventHandler(this.Cal_access_bits_button_Click);
            // 
            // buttonASCIIToString
            // 
            this.buttonASCIIToString.Location = new System.Drawing.Point(725, 248);
            this.buttonASCIIToString.Name = "buttonASCIIToString";
            this.buttonASCIIToString.Size = new System.Drawing.Size(75, 21);
            this.buttonASCIIToString.TabIndex = 156;
            this.buttonASCIIToString.Text = "ASCII-Str";
            this.buttonASCIIToString.UseVisualStyleBackColor = true;
            this.buttonASCIIToString.Click += new System.EventHandler(this.buttonASCIIToString_Click);
            // 
            // buttonStringToASCII
            // 
            this.buttonStringToASCII.Location = new System.Drawing.Point(725, 270);
            this.buttonStringToASCII.Name = "buttonStringToASCII";
            this.buttonStringToASCII.Size = new System.Drawing.Size(75, 21);
            this.buttonStringToASCII.TabIndex = 155;
            this.buttonStringToASCII.Text = "Str-ASCII";
            this.buttonStringToASCII.UseVisualStyleBackColor = true;
            this.buttonStringToASCII.Click += new System.EventHandler(this.buttonStringToASCII_Click);
            // 
            // button_AppendCRC16
            // 
            this.button_AppendCRC16.Location = new System.Drawing.Point(725, 291);
            this.button_AppendCRC16.Name = "button_AppendCRC16";
            this.button_AppendCRC16.Size = new System.Drawing.Size(75, 23);
            this.button_AppendCRC16.TabIndex = 154;
            this.button_AppendCRC16.Text = "Append CRC";
            this.button_AppendCRC16.UseVisualStyleBackColor = true;
            this.button_AppendCRC16.Click += new System.EventHandler(this.button_AppendCRC16_Click);
            // 
            // label75
            // 
            this.label75.AutoSize = true;
            this.label75.Location = new System.Drawing.Point(723, 225);
            this.label75.Name = "label75";
            this.label75.Size = new System.Drawing.Size(47, 12);
            this.label75.TabIndex = 153;
            this.label75.Text = "Init 0x";
            // 
            // textBox_CRC
            // 
            this.textBox_CRC.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_CRC.Location = new System.Drawing.Point(769, 221);
            this.textBox_CRC.MaxLength = 4;
            this.textBox_CRC.Name = "textBox_CRC";
            this.textBox_CRC.Size = new System.Drawing.Size(34, 23);
            this.textBox_CRC.TabIndex = 152;
            this.textBox_CRC.Text = "6363";
            this.textBox_CRC.TextChanged += new System.EventHandler(this.textBox_CRC_TextChanged);
            this.textBox_CRC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_CRC_KeyPress);
            this.textBox_CRC.Leave += new System.EventHandler(this.textBox_CRC_Leave);
            // 
            // button_CRC
            // 
            this.button_CRC.Location = new System.Drawing.Point(725, 315);
            this.button_CRC.Name = "button_CRC";
            this.button_CRC.Size = new System.Drawing.Size(75, 23);
            this.button_CRC.TabIndex = 151;
            this.button_CRC.Text = "Calc CRC16";
            this.button_CRC.UseVisualStyleBackColor = true;
            this.button_CRC.Click += new System.EventHandler(this.button_CRC_Click);
            this.button_CRC.MouseEnter += new System.EventHandler(this.button_CRC_MouseEnter);
            this.button_CRC.MouseLeave += new System.EventHandler(this.button_CRC_MouseLeave);
            // 
            // cla_parity_groupbox
            // 
            this.cla_parity_groupbox.Controls.Add(this.cla_no_parity_cfg);
            this.cla_parity_groupbox.Controls.Add(this.cla_even_parity_cfg);
            this.cla_parity_groupbox.Controls.Add(this.cla_odd_parity_cfg);
            this.cla_parity_groupbox.Location = new System.Drawing.Point(261, 304);
            this.cla_parity_groupbox.Name = "cla_parity_groupbox";
            this.cla_parity_groupbox.Size = new System.Drawing.Size(79, 62);
            this.cla_parity_groupbox.TabIndex = 147;
            this.cla_parity_groupbox.TabStop = false;
            this.cla_parity_groupbox.Text = "Tx Parity";
            // 
            // cla_no_parity_cfg
            // 
            this.cla_no_parity_cfg.AutoSize = true;
            this.cla_no_parity_cfg.Location = new System.Drawing.Point(6, 43);
            this.cla_no_parity_cfg.Name = "cla_no_parity_cfg";
            this.cla_no_parity_cfg.Size = new System.Drawing.Size(35, 16);
            this.cla_no_parity_cfg.TabIndex = 3;
            this.cla_no_parity_cfg.Text = "NO";
            this.cla_no_parity_cfg.UseVisualStyleBackColor = true;
            // 
            // cla_even_parity_cfg
            // 
            this.cla_even_parity_cfg.AutoSize = true;
            this.cla_even_parity_cfg.Location = new System.Drawing.Point(6, 28);
            this.cla_even_parity_cfg.Name = "cla_even_parity_cfg";
            this.cla_even_parity_cfg.Size = new System.Drawing.Size(47, 16);
            this.cla_even_parity_cfg.TabIndex = 1;
            this.cla_even_parity_cfg.Text = "EVEN";
            this.cla_even_parity_cfg.UseVisualStyleBackColor = true;
            // 
            // cla_odd_parity_cfg
            // 
            this.cla_odd_parity_cfg.AutoSize = true;
            this.cla_odd_parity_cfg.Checked = true;
            this.cla_odd_parity_cfg.Location = new System.Drawing.Point(6, 13);
            this.cla_odd_parity_cfg.Name = "cla_odd_parity_cfg";
            this.cla_odd_parity_cfg.Size = new System.Drawing.Size(41, 16);
            this.cla_odd_parity_cfg.TabIndex = 0;
            this.cla_odd_parity_cfg.TabStop = true;
            this.cla_odd_parity_cfg.Text = "ODD";
            this.cla_odd_parity_cfg.UseVisualStyleBackColor = true;
            // 
            // textBox_APDU_Le_CL
            // 
            this.textBox_APDU_Le_CL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_Le_CL.Location = new System.Drawing.Point(185, 373);
            this.textBox_APDU_Le_CL.MaxLength = 2;
            this.textBox_APDU_Le_CL.Name = "textBox_APDU_Le_CL";
            this.textBox_APDU_Le_CL.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_Le_CL.TabIndex = 111;
            this.textBox_APDU_Le_CL.Text = "00";
            this.textBox_APDU_Le_CL.TextChanged += new System.EventHandler(this.textBox_APDU_Le_CL_TextChanged);
            this.textBox_APDU_Le_CL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_Le_KeyPress);
            this.textBox_APDU_Le_CL.Leave += new System.EventHandler(this.textBox_APDU_Le_CL_Leave);
            // 
            // LeText
            // 
            this.LeText.AutoSize = true;
            this.LeText.Location = new System.Drawing.Point(188, 358);
            this.LeText.Name = "LeText";
            this.LeText.Size = new System.Drawing.Size(17, 12);
            this.LeText.TabIndex = 110;
            this.LeText.Text = "Le";
            // 
            // GetReaders_bnt
            // 
            this.GetReaders_bnt.Location = new System.Drawing.Point(763, 36);
            this.GetReaders_bnt.Name = "GetReaders_bnt";
            this.GetReaders_bnt.Size = new System.Drawing.Size(55, 36);
            this.GetReaders_bnt.TabIndex = 107;
            this.GetReaders_bnt.Text = "Get Readers";
            this.GetReaders_bnt.UseVisualStyleBackColor = true;
            this.GetReaders_bnt.Click += new System.EventHandler(this.GetReaders_bnt_Click);
            // 
            // readers_cbox
            // 
            this.readers_cbox.FormattingEnabled = true;
            this.readers_cbox.Location = new System.Drawing.Point(763, 11);
            this.readers_cbox.Name = "readers_cbox";
            this.readers_cbox.Size = new System.Drawing.Size(187, 20);
            this.readers_cbox.TabIndex = 106;
            this.readers_cbox.Text = "no readers";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(867, 106);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(83, 12);
            this.label57.TabIndex = 92;
            this.label57.Text = "Flash编程串口";
            // 
            // flash_isp_comSelect
            // 
            this.flash_isp_comSelect.Font = new System.Drawing.Font("宋体", 10F);
            this.flash_isp_comSelect.FormattingEnabled = true;
            this.flash_isp_comSelect.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14"});
            this.flash_isp_comSelect.Location = new System.Drawing.Point(890, 128);
            this.flash_isp_comSelect.Name = "flash_isp_comSelect";
            this.flash_isp_comSelect.Size = new System.Drawing.Size(58, 21);
            this.flash_isp_comSelect.TabIndex = 91;
            this.flash_isp_comSelect.Text = "COM1";
            // 
            // version_check
            // 
            this.version_check.Location = new System.Drawing.Point(824, 256);
            this.version_check.Name = "version_check";
            this.version_check.Size = new System.Drawing.Size(127, 23);
            this.version_check.TabIndex = 90;
            this.version_check.Text = "软件版本信息";
            this.version_check.UseVisualStyleBackColor = true;
            this.version_check.Click += new System.EventHandler(this.version_check_Click);
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label51.Location = new System.Drawing.Point(822, 302);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(113, 12);
            this.label51.TabIndex = 68;
            this.label51.Text = "谐振频率相关计算：";
            // 
            // ResFreqCacu_L_btn
            // 
            this.ResFreqCacu_L_btn.Location = new System.Drawing.Point(824, 344);
            this.ResFreqCacu_L_btn.Name = "ResFreqCacu_L_btn";
            this.ResFreqCacu_L_btn.Size = new System.Drawing.Size(75, 23);
            this.ResFreqCacu_L_btn.TabIndex = 67;
            this.ResFreqCacu_L_btn.Text = "电感(uH)";
            this.ResFreqCacu_L_btn.UseVisualStyleBackColor = true;
            this.ResFreqCacu_L_btn.Click += new System.EventHandler(this.ResFreqCacu_L_btn_Click);
            // 
            // ResFreqCacu_C_btn
            // 
            this.ResFreqCacu_C_btn.Location = new System.Drawing.Point(824, 317);
            this.ResFreqCacu_C_btn.Name = "ResFreqCacu_C_btn";
            this.ResFreqCacu_C_btn.Size = new System.Drawing.Size(75, 23);
            this.ResFreqCacu_C_btn.TabIndex = 66;
            this.ResFreqCacu_C_btn.Text = "电容(pF)";
            this.ResFreqCacu_C_btn.UseVisualStyleBackColor = true;
            this.ResFreqCacu_C_btn.Click += new System.EventHandler(this.ResFreqCacu_C_btn_Click);
            // 
            // ResFreqCacu_F_btn
            // 
            this.ResFreqCacu_F_btn.Location = new System.Drawing.Point(824, 371);
            this.ResFreqCacu_F_btn.Name = "ResFreqCacu_F_btn";
            this.ResFreqCacu_F_btn.Size = new System.Drawing.Size(75, 23);
            this.ResFreqCacu_F_btn.TabIndex = 65;
            this.ResFreqCacu_F_btn.Text = "频率(MHz)";
            this.ResFreqCacu_F_btn.UseVisualStyleBackColor = true;
            this.ResFreqCacu_F_btn.Click += new System.EventHandler(this.ResFreqCacu_F_btn_Click);
            // 
            // ResonFreqCaculate_F_tB
            // 
            this.ResonFreqCaculate_F_tB.Location = new System.Drawing.Point(905, 373);
            this.ResonFreqCaculate_F_tB.MaxLength = 5;
            this.ResonFreqCaculate_F_tB.Name = "ResonFreqCaculate_F_tB";
            this.ResonFreqCaculate_F_tB.Size = new System.Drawing.Size(46, 21);
            this.ResonFreqCaculate_F_tB.TabIndex = 64;
            // 
            // ResonFreqCaculate_L_tB
            // 
            this.ResonFreqCaculate_L_tB.Location = new System.Drawing.Point(905, 346);
            this.ResonFreqCaculate_L_tB.MaxLength = 5;
            this.ResonFreqCaculate_L_tB.Name = "ResonFreqCaculate_L_tB";
            this.ResonFreqCaculate_L_tB.Size = new System.Drawing.Size(46, 21);
            this.ResonFreqCaculate_L_tB.TabIndex = 63;
            this.ResonFreqCaculate_L_tB.Text = "4";
            // 
            // ResonFreqCaculate_C_tB
            // 
            this.ResonFreqCaculate_C_tB.Location = new System.Drawing.Point(905, 318);
            this.ResonFreqCaculate_C_tB.MaxLength = 5;
            this.ResonFreqCaculate_C_tB.Name = "ResonFreqCaculate_C_tB";
            this.ResonFreqCaculate_C_tB.Size = new System.Drawing.Size(46, 21);
            this.ResonFreqCaculate_C_tB.TabIndex = 62;
            this.ResonFreqCaculate_C_tB.Text = "20";
            // 
            // CLTypeSelect_groupBox
            // 
            this.CLTypeSelect_groupBox.Controls.Add(this.TypeBSel_radioButton);
            this.CLTypeSelect_groupBox.Controls.Add(this.TypeASel_radioButton);
            this.CLTypeSelect_groupBox.Location = new System.Drawing.Point(325, 128);
            this.CLTypeSelect_groupBox.Name = "CLTypeSelect_groupBox";
            this.CLTypeSelect_groupBox.Size = new System.Drawing.Size(90, 60);
            this.CLTypeSelect_groupBox.TabIndex = 51;
            this.CLTypeSelect_groupBox.TabStop = false;
            this.CLTypeSelect_groupBox.Text = "Type Select";
            // 
            // TypeBSel_radioButton
            // 
            this.TypeBSel_radioButton.AutoSize = true;
            this.TypeBSel_radioButton.Location = new System.Drawing.Point(13, 37);
            this.TypeBSel_radioButton.Name = "TypeBSel_radioButton";
            this.TypeBSel_radioButton.Size = new System.Drawing.Size(53, 16);
            this.TypeBSel_radioButton.TabIndex = 1;
            this.TypeBSel_radioButton.Text = "TypeB";
            this.TypeBSel_radioButton.UseVisualStyleBackColor = true;
            this.TypeBSel_radioButton.CheckedChanged += new System.EventHandler(this.TypeBSel_radioButton_CheckedChanged);
            // 
            // TypeASel_radioButton
            // 
            this.TypeASel_radioButton.AutoSize = true;
            this.TypeASel_radioButton.Checked = true;
            this.TypeASel_radioButton.Location = new System.Drawing.Point(13, 16);
            this.TypeASel_radioButton.Name = "TypeASel_radioButton";
            this.TypeASel_radioButton.Size = new System.Drawing.Size(53, 16);
            this.TypeASel_radioButton.TabIndex = 0;
            this.TypeASel_radioButton.TabStop = true;
            this.TypeASel_radioButton.Text = "TypeA";
            this.TypeASel_radioButton.UseVisualStyleBackColor = true;
            this.TypeASel_radioButton.CheckedChanged += new System.EventHandler(this.TypeASel_radioButton_CheckedChanged);
            // 
            // TransceiveDataCL_listBox
            // 
            this.TransceiveDataCL_listBox.FormattingEnabled = true;
            this.TransceiveDataCL_listBox.ItemHeight = 12;
            this.TransceiveDataCL_listBox.Items.AddRange(new object[] {
            "E031:RATS",
            "3280:DATA_EE",
            "32C0:PROG_EE",
            "32A0:安全区",
            "3180:",
            "0084000008",
            "000102030405060708090A0B0C0D0E0F",
            "00000000000000000000000000000000",
            "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "55555555555555555555555555555555",
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"});
            this.TransceiveDataCL_listBox.Location = new System.Drawing.Point(432, 13);
            this.TransceiveDataCL_listBox.Name = "TransceiveDataCL_listBox";
            this.TransceiveDataCL_listBox.ScrollAlwaysVisible = true;
            this.TransceiveDataCL_listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.TransceiveDataCL_listBox.Size = new System.Drawing.Size(263, 388);
            this.TransceiveDataCL_listBox.TabIndex = 37;
            this.TransceiveDataCL_listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TransceiveDataCL_listBox_MouseClick);
            this.TransceiveDataCL_listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransceiveDataCL_listBox_KeyDown);
            // 
            // AddCMDtoListBox_CL_btn
            // 
            this.AddCMDtoListBox_CL_btn.Location = new System.Drawing.Point(349, 354);
            this.AddCMDtoListBox_CL_btn.Name = "AddCMDtoListBox_CL_btn";
            this.AddCMDtoListBox_CL_btn.Size = new System.Drawing.Size(75, 44);
            this.AddCMDtoListBox_CL_btn.TabIndex = 49;
            this.AddCMDtoListBox_CL_btn.Text = "Add CMD List";
            this.AddCMDtoListBox_CL_btn.UseVisualStyleBackColor = true;
            this.AddCMDtoListBox_CL_btn.Click += new System.EventHandler(this.AddCMDtoListBox_CL_btn_Click);
            // 
            // checkBox_APDUmodeCL
            // 
            this.checkBox_APDUmodeCL.AutoSize = true;
            this.checkBox_APDUmodeCL.Location = new System.Drawing.Point(17, 295);
            this.checkBox_APDUmodeCL.Name = "checkBox_APDUmodeCL";
            this.checkBox_APDUmodeCL.Size = new System.Drawing.Size(78, 16);
            this.checkBox_APDUmodeCL.TabIndex = 47;
            this.checkBox_APDUmodeCL.Text = "APDU mode";
            this.checkBox_APDUmodeCL.UseVisualStyleBackColor = true;
            this.checkBox_APDUmodeCL.CheckedChanged += new System.EventHandler(this.checkBox_APDU_mode_CheckedChanged);
            // 
            // label_APDU_CL
            // 
            this.label_APDU_CL.AutoSize = true;
            this.label_APDU_CL.Enabled = false;
            this.label_APDU_CL.Location = new System.Drawing.Point(15, 358);
            this.label_APDU_CL.Name = "label_APDU_CL";
            this.label_APDU_CL.Size = new System.Drawing.Size(149, 12);
            this.label_APDU_CL.TabIndex = 46;
            this.label_APDU_CL.Text = "CLA   INS   P1   P2   P3";
            // 
            // textBox_APDU_P3_CL
            // 
            this.textBox_APDU_P3_CL.Enabled = false;
            this.textBox_APDU_P3_CL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_P3_CL.Location = new System.Drawing.Point(141, 373);
            this.textBox_APDU_P3_CL.MaxLength = 2;
            this.textBox_APDU_P3_CL.Name = "textBox_APDU_P3_CL";
            this.textBox_APDU_P3_CL.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_P3_CL.TabIndex = 45;
            this.textBox_APDU_P3_CL.Text = "08";
            this.textBox_APDU_P3_CL.TextChanged += new System.EventHandler(this.textBox_APDU_P3_TextChanged);
            this.textBox_APDU_P3_CL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_P3_KeyPress);
            this.textBox_APDU_P3_CL.Leave += new System.EventHandler(this.textBox_APDU_P3_CL_Leave);
            // 
            // textBox_APDU_P2_CL
            // 
            this.textBox_APDU_P2_CL.Enabled = false;
            this.textBox_APDU_P2_CL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_P2_CL.Location = new System.Drawing.Point(110, 373);
            this.textBox_APDU_P2_CL.MaxLength = 2;
            this.textBox_APDU_P2_CL.Name = "textBox_APDU_P2_CL";
            this.textBox_APDU_P2_CL.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_P2_CL.TabIndex = 44;
            this.textBox_APDU_P2_CL.Text = "00";
            this.textBox_APDU_P2_CL.TextChanged += new System.EventHandler(this.textBox_APDU_P2_TextChanged);
            this.textBox_APDU_P2_CL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_P2_KeyPress);
            this.textBox_APDU_P2_CL.Leave += new System.EventHandler(this.textBox_APDU_P2_CL_Leave);
            // 
            // textBox_APDU_P1_CL
            // 
            this.textBox_APDU_P1_CL.Enabled = false;
            this.textBox_APDU_P1_CL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_P1_CL.Location = new System.Drawing.Point(79, 373);
            this.textBox_APDU_P1_CL.MaxLength = 2;
            this.textBox_APDU_P1_CL.Name = "textBox_APDU_P1_CL";
            this.textBox_APDU_P1_CL.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_P1_CL.TabIndex = 43;
            this.textBox_APDU_P1_CL.Text = "00";
            this.textBox_APDU_P1_CL.TextChanged += new System.EventHandler(this.textBox_APDU_P1_TextChanged);
            this.textBox_APDU_P1_CL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_P1_KeyPress);
            this.textBox_APDU_P1_CL.Leave += new System.EventHandler(this.textBox_APDU_P1_CL_Leave);
            // 
            // textBox_APDU_INS_CL
            // 
            this.textBox_APDU_INS_CL.Enabled = false;
            this.textBox_APDU_INS_CL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_INS_CL.Location = new System.Drawing.Point(48, 373);
            this.textBox_APDU_INS_CL.MaxLength = 2;
            this.textBox_APDU_INS_CL.Name = "textBox_APDU_INS_CL";
            this.textBox_APDU_INS_CL.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_INS_CL.TabIndex = 42;
            this.textBox_APDU_INS_CL.Text = "84";
            this.textBox_APDU_INS_CL.TextChanged += new System.EventHandler(this.textBox_APDU_INS_TextChanged);
            this.textBox_APDU_INS_CL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_INS_KeyPress);
            this.textBox_APDU_INS_CL.Leave += new System.EventHandler(this.textBox_APDU_INS_CL_Leave);
            // 
            // textBox_APDU_CLA_CL
            // 
            this.textBox_APDU_CLA_CL.Enabled = false;
            this.textBox_APDU_CLA_CL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_CLA_CL.Location = new System.Drawing.Point(17, 373);
            this.textBox_APDU_CLA_CL.MaxLength = 2;
            this.textBox_APDU_CLA_CL.Name = "textBox_APDU_CLA_CL";
            this.textBox_APDU_CLA_CL.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_CLA_CL.TabIndex = 41;
            this.textBox_APDU_CLA_CL.Text = "00";
            this.textBox_APDU_CLA_CL.TextChanged += new System.EventHandler(this.textBox_APDU_CLA_TextChanged);
            this.textBox_APDU_CLA_CL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_CLA_KeyPress);
            this.textBox_APDU_CLA_CL.Leave += new System.EventHandler(this.textBox_APDU_CLA_CL_Leave);
            // 
            // CardType_Select
            // 
            this.CardType_Select.FormattingEnabled = true;
            this.CardType_Select.Items.AddRange(new object[] {
            "FM309",
            "FM293",
            "FM274"});
            this.CardType_Select.Location = new System.Drawing.Point(763, 75);
            this.CardType_Select.Name = "CardType_Select";
            this.CardType_Select.Size = new System.Drawing.Size(187, 20);
            this.CardType_Select.TabIndex = 40;
            this.CardType_Select.Text = "FM309";
            // 
            // Field_ON
            // 
            this.Field_ON.Location = new System.Drawing.Point(17, 41);
            this.Field_ON.Name = "Field_ON";
            this.Field_ON.Size = new System.Drawing.Size(75, 30);
            this.Field_ON.TabIndex = 39;
            this.Field_ON.Text = "Field ON";
            this.Field_ON.UseVisualStyleBackColor = true;
            this.Field_ON.Click += new System.EventHandler(this.Field_ON_Click);
            this.Field_ON.MouseEnter += new System.EventHandler(this.Field_ON_MouseEnter);
            this.Field_ON.MouseLeave += new System.EventHandler(this.Field_ON_MouseLeave);
            // 
            // ConnectReader
            // 
            this.ConnectReader.Location = new System.Drawing.Point(824, 36);
            this.ConnectReader.Name = "ConnectReader";
            this.ConnectReader.Size = new System.Drawing.Size(126, 36);
            this.ConnectReader.TabIndex = 38;
            this.ConnectReader.Text = "Connect Reader";
            this.ConnectReader.UseVisualStyleBackColor = true;
            this.ConnectReader.Click += new System.EventHandler(this.ConnectReader_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(216, 357);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 35;
            this.label5.Text = "超时";
            // 
            // TimeOut_textBox
            // 
            this.TimeOut_textBox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TimeOut_textBox.Location = new System.Drawing.Point(218, 374);
            this.TimeOut_textBox.MaxLength = 2;
            this.TimeOut_textBox.Name = "TimeOut_textBox";
            this.TimeOut_textBox.Size = new System.Drawing.Size(25, 21);
            this.TimeOut_textBox.TabIndex = 34;
            this.TimeOut_textBox.Text = "09";
            this.TimeOut_textBox.TextChanged += new System.EventHandler(this.TimeOut_textBox_TextChanged);
            // 
            // Reset17
            // 
            this.Reset17.Location = new System.Drawing.Point(17, 7);
            this.Reset17.Name = "Reset17";
            this.Reset17.Size = new System.Drawing.Size(75, 32);
            this.Reset17.TabIndex = 14;
            this.Reset17.Text = "Reset17";
            this.Reset17.UseVisualStyleBackColor = true;
            this.Reset17.Click += new System.EventHandler(this.Reset17_Click);
            // 
            // CRC_gropbox
            // 
            this.CRC_gropbox.Controls.Add(this.tx_crc_cfg);
            this.CRC_gropbox.Controls.Add(this.rx_crc_cfg);
            this.CRC_gropbox.Controls.Add(this.no_crc_cfg);
            this.CRC_gropbox.Controls.Add(this.all_crc_cfg);
            this.CRC_gropbox.Location = new System.Drawing.Point(122, 304);
            this.CRC_gropbox.Name = "CRC_gropbox";
            this.CRC_gropbox.Size = new System.Drawing.Size(135, 50);
            this.CRC_gropbox.TabIndex = 93;
            this.CRC_gropbox.TabStop = false;
            this.CRC_gropbox.Text = "CRC Select";
            // 
            // tx_crc_cfg
            // 
            this.tx_crc_cfg.AutoSize = true;
            this.tx_crc_cfg.Location = new System.Drawing.Point(74, 29);
            this.tx_crc_cfg.Name = "tx_crc_cfg";
            this.tx_crc_cfg.Size = new System.Drawing.Size(59, 16);
            this.tx_crc_cfg.TabIndex = 3;
            this.tx_crc_cfg.Text = "TX_CRC";
            this.tx_crc_cfg.UseVisualStyleBackColor = true;
            // 
            // rx_crc_cfg
            // 
            this.rx_crc_cfg.AutoSize = true;
            this.rx_crc_cfg.Location = new System.Drawing.Point(74, 12);
            this.rx_crc_cfg.Name = "rx_crc_cfg";
            this.rx_crc_cfg.Size = new System.Drawing.Size(59, 16);
            this.rx_crc_cfg.TabIndex = 2;
            this.rx_crc_cfg.Text = "RX_CRC";
            this.rx_crc_cfg.UseVisualStyleBackColor = true;
            // 
            // no_crc_cfg
            // 
            this.no_crc_cfg.AutoSize = true;
            this.no_crc_cfg.Location = new System.Drawing.Point(6, 29);
            this.no_crc_cfg.Name = "no_crc_cfg";
            this.no_crc_cfg.Size = new System.Drawing.Size(59, 16);
            this.no_crc_cfg.TabIndex = 1;
            this.no_crc_cfg.Text = "NO_CRC";
            this.no_crc_cfg.UseVisualStyleBackColor = true;
            // 
            // all_crc_cfg
            // 
            this.all_crc_cfg.AutoSize = true;
            this.all_crc_cfg.Checked = true;
            this.all_crc_cfg.Location = new System.Drawing.Point(6, 12);
            this.all_crc_cfg.Name = "all_crc_cfg";
            this.all_crc_cfg.Size = new System.Drawing.Size(65, 16);
            this.all_crc_cfg.TabIndex = 0;
            this.all_crc_cfg.TabStop = true;
            this.all_crc_cfg.Text = "ALL_CRC";
            this.all_crc_cfg.UseVisualStyleBackColor = true;
            // 
            // TransceiveCL
            // 
            this.TransceiveCL.Location = new System.Drawing.Point(15, 312);
            this.TransceiveCL.Name = "TransceiveCL";
            this.TransceiveCL.Size = new System.Drawing.Size(104, 42);
            this.TransceiveCL.TabIndex = 29;
            this.TransceiveCL.Text = "Direct Send";
            this.TransceiveCL.UseVisualStyleBackColor = true;
            this.TransceiveCL.Click += new System.EventHandler(this.TransceiveCL_Click);
            // 
            // groupBox_FM17Reg
            // 
            this.groupBox_FM17Reg.Controls.Add(this.label1);
            this.groupBox_FM17Reg.Controls.Add(this.Read17Reg);
            this.groupBox_FM17Reg.Controls.Add(this.Write17Reg);
            this.groupBox_FM17Reg.Controls.Add(this.RegAddr_17_textBox);
            this.groupBox_FM17Reg.Controls.Add(this.RegData_17_textBox);
            this.groupBox_FM17Reg.Controls.Add(this.label2);
            this.groupBox_FM17Reg.Location = new System.Drawing.Point(108, 5);
            this.groupBox_FM17Reg.Name = "groupBox_FM17Reg";
            this.groupBox_FM17Reg.Size = new System.Drawing.Size(188, 66);
            this.groupBox_FM17Reg.TabIndex = 150;
            this.groupBox_FM17Reg.TabStop = false;
            this.groupBox_FM17Reg.Text = "FM17 Reg";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 14);
            this.label1.TabIndex = 27;
            this.label1.Text = "Addr 0x";
            // 
            // Read17Reg
            // 
            this.Read17Reg.Location = new System.Drawing.Point(102, 11);
            this.Read17Reg.Name = "Read17Reg";
            this.Read17Reg.Size = new System.Drawing.Size(75, 23);
            this.Read17Reg.TabIndex = 20;
            this.Read17Reg.Text = "Read17Reg";
            this.Read17Reg.UseVisualStyleBackColor = true;
            this.Read17Reg.Click += new System.EventHandler(this.Read17Reg_Click);
            // 
            // Write17Reg
            // 
            this.Write17Reg.Location = new System.Drawing.Point(102, 38);
            this.Write17Reg.Name = "Write17Reg";
            this.Write17Reg.Size = new System.Drawing.Size(75, 24);
            this.Write17Reg.TabIndex = 21;
            this.Write17Reg.Text = "Write17Reg";
            this.Write17Reg.UseVisualStyleBackColor = true;
            this.Write17Reg.Click += new System.EventHandler(this.Write17Reg_Click);
            // 
            // RegAddr_17_textBox
            // 
            this.RegAddr_17_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.RegAddr_17_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RegAddr_17_textBox.Location = new System.Drawing.Point(65, 12);
            this.RegAddr_17_textBox.MaxLength = 2;
            this.RegAddr_17_textBox.Name = "RegAddr_17_textBox";
            this.RegAddr_17_textBox.Size = new System.Drawing.Size(22, 23);
            this.RegAddr_17_textBox.TabIndex = 24;
            this.RegAddr_17_textBox.Text = "11";
            this.RegAddr_17_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegAddr_17_KeyPress);
            // 
            // RegData_17_textBox
            // 
            this.RegData_17_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.RegData_17_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RegData_17_textBox.Location = new System.Drawing.Point(65, 38);
            this.RegData_17_textBox.MaxLength = 2;
            this.RegData_17_textBox.Name = "RegData_17_textBox";
            this.RegData_17_textBox.Size = new System.Drawing.Size(22, 23);
            this.RegData_17_textBox.TabIndex = 25;
            this.RegData_17_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegData_17_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(9, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 14);
            this.label2.TabIndex = 26;
            this.label2.Text = "Data 0x";
            // 
            // groupBox_CLA_CONTROL
            // 
            this.groupBox_CLA_CONTROL.Controls.Add(this.VHBR_PPS_checkBox);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_PPSCL_DRI);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_DSI);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_DRI);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_CID);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_CID);
            this.groupBox_CLA_CONTROL.Controls.Add(this.checkBox_last_SAK);
            this.groupBox_CLA_CONTROL.Controls.Add(this.checkBox_check_BCC);
            this.groupBox_CLA_CONTROL.Controls.Add(this.MI_Deselect);
            this.groupBox_CLA_CONTROL.Controls.Add(this.Active);
            this.groupBox_CLA_CONTROL.Controls.Add(this.MI_REQA);
            this.groupBox_CLA_CONTROL.Controls.Add(this.MI_AntiColl);
            this.groupBox_CLA_CONTROL.Controls.Add(this.MI_Select);
            this.groupBox_CLA_CONTROL.Controls.Add(this.button_RATS);
            this.groupBox_CLA_CONTROL.Controls.Add(this.pps_exchange_CL_btn);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_UID1);
            this.groupBox_CLA_CONTROL.Controls.Add(this.WupA);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_UID2);
            this.groupBox_CLA_CONTROL.Controls.Add(this.Halt);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_UID3);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_SAK1);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_SAK2);
            this.groupBox_CLA_CONTROL.Controls.Add(this.label_SAK3);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_UID1);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_UID2);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_UID3);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_SAK1);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_SAK2);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_SAK3);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_last_SAK);
            this.groupBox_CLA_CONTROL.Controls.Add(this.textBox_PPSCL_DSI);
            this.groupBox_CLA_CONTROL.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_CLA_CONTROL.Location = new System.Drawing.Point(6, 75);
            this.groupBox_CLA_CONTROL.Name = "groupBox_CLA_CONTROL";
            this.groupBox_CLA_CONTROL.Size = new System.Drawing.Size(312, 216);
            this.groupBox_CLA_CONTROL.TabIndex = 52;
            this.groupBox_CLA_CONTROL.TabStop = false;
            this.groupBox_CLA_CONTROL.Text = "Type A Activation";
            // 
            // VHBR_PPS_checkBox
            // 
            this.VHBR_PPS_checkBox.AutoSize = true;
            this.VHBR_PPS_checkBox.Location = new System.Drawing.Point(95, 195);
            this.VHBR_PPS_checkBox.Name = "VHBR_PPS_checkBox";
            this.VHBR_PPS_checkBox.Size = new System.Drawing.Size(82, 18);
            this.VHBR_PPS_checkBox.TabIndex = 180;
            this.VHBR_PPS_checkBox.Text = "VHBR_PPS";
            this.VHBR_PPS_checkBox.UseVisualStyleBackColor = true;
            this.VHBR_PPS_checkBox.CheckedChanged += new System.EventHandler(this.VHBR_PPS_checkBox_CheckedChanged);
            // 
            // textBox_PPSCL_DRI
            // 
            this.textBox_PPSCL_DRI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_PPSCL_DRI.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PPSCL_DRI.Location = new System.Drawing.Point(74, 149);
            this.textBox_PPSCL_DRI.MaxLength = 2;
            this.textBox_PPSCL_DRI.Name = "textBox_PPSCL_DRI";
            this.textBox_PPSCL_DRI.Size = new System.Drawing.Size(18, 21);
            this.textBox_PPSCL_DRI.TabIndex = 96;
            this.textBox_PPSCL_DRI.Text = "0";
            // 
            // label_DSI
            // 
            this.label_DSI.AutoSize = true;
            this.label_DSI.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_DSI.Location = new System.Drawing.Point(9, 152);
            this.label_DSI.Name = "label_DSI";
            this.label_DSI.Size = new System.Drawing.Size(23, 12);
            this.label_DSI.TabIndex = 95;
            this.label_DSI.Text = "DSI";
            // 
            // label_DRI
            // 
            this.label_DRI.AutoSize = true;
            this.label_DRI.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_DRI.Location = new System.Drawing.Point(50, 152);
            this.label_DRI.Name = "label_DRI";
            this.label_DRI.Size = new System.Drawing.Size(23, 12);
            this.label_DRI.TabIndex = 97;
            this.label_DRI.Text = "DRI";
            // 
            // label_CID
            // 
            this.label_CID.AutoSize = true;
            this.label_CID.Font = new System.Drawing.Font("宋体", 9F);
            this.label_CID.Location = new System.Drawing.Point(9, 120);
            this.label_CID.Name = "label_CID";
            this.label_CID.Size = new System.Drawing.Size(23, 12);
            this.label_CID.TabIndex = 148;
            this.label_CID.Text = "CID";
            // 
            // textBox_CID
            // 
            this.textBox_CID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_CID.Font = new System.Drawing.Font("宋体", 10F);
            this.textBox_CID.Location = new System.Drawing.Point(32, 116);
            this.textBox_CID.MaxLength = 1;
            this.textBox_CID.Name = "textBox_CID";
            this.textBox_CID.Size = new System.Drawing.Size(16, 23);
            this.textBox_CID.TabIndex = 148;
            this.textBox_CID.Text = "1";
            this.textBox_CID.TextChanged += new System.EventHandler(this.textBox_CID_TextChanged);
            // 
            // checkBox_last_SAK
            // 
            this.checkBox_last_SAK.AutoSize = true;
            this.checkBox_last_SAK.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_last_SAK.Location = new System.Drawing.Point(185, 157);
            this.checkBox_last_SAK.Name = "checkBox_last_SAK";
            this.checkBox_last_SAK.Size = new System.Drawing.Size(72, 16);
            this.checkBox_last_SAK.TabIndex = 148;
            this.checkBox_last_SAK.Text = "Last SAK";
            this.checkBox_last_SAK.UseVisualStyleBackColor = true;
            this.checkBox_last_SAK.CheckedChanged += new System.EventHandler(this.checkBox_last_SAK_CheckedChanged);
            // 
            // checkBox_check_BCC
            // 
            this.checkBox_check_BCC.AutoSize = true;
            this.checkBox_check_BCC.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_check_BCC.Location = new System.Drawing.Point(185, 176);
            this.checkBox_check_BCC.Name = "checkBox_check_BCC";
            this.checkBox_check_BCC.Size = new System.Drawing.Size(78, 16);
            this.checkBox_check_BCC.TabIndex = 146;
            this.checkBox_check_BCC.Text = "Check BCC";
            this.checkBox_check_BCC.UseVisualStyleBackColor = true;
            // 
            // MI_Deselect
            // 
            this.MI_Deselect.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MI_Deselect.Location = new System.Drawing.Point(102, 171);
            this.MI_Deselect.Name = "MI_Deselect";
            this.MI_Deselect.Size = new System.Drawing.Size(61, 23);
            this.MI_Deselect.TabIndex = 112;
            this.MI_Deselect.Text = "Deselect";
            this.MI_Deselect.UseVisualStyleBackColor = true;
            this.MI_Deselect.Click += new System.EventHandler(this.MI_Deselect_Click);
            // 
            // Active
            // 
            this.Active.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Active.Location = new System.Drawing.Point(11, 21);
            this.Active.Name = "Active";
            this.Active.Size = new System.Drawing.Size(75, 41);
            this.Active.TabIndex = 19;
            this.Active.Text = "Active";
            this.Active.UseVisualStyleBackColor = true;
            this.Active.Click += new System.EventHandler(this.Active_Click);
            // 
            // MI_REQA
            // 
            this.MI_REQA.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MI_REQA.Location = new System.Drawing.Point(102, 21);
            this.MI_REQA.Name = "MI_REQA";
            this.MI_REQA.Size = new System.Drawing.Size(61, 23);
            this.MI_REQA.TabIndex = 16;
            this.MI_REQA.Text = "REQA";
            this.MI_REQA.UseVisualStyleBackColor = true;
            this.MI_REQA.Click += new System.EventHandler(this.MI_REQA_Click);
            // 
            // MI_AntiColl
            // 
            this.MI_AntiColl.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MI_AntiColl.Location = new System.Drawing.Point(102, 49);
            this.MI_AntiColl.Name = "MI_AntiColl";
            this.MI_AntiColl.Size = new System.Drawing.Size(61, 23);
            this.MI_AntiColl.TabIndex = 17;
            this.MI_AntiColl.Text = "AntiColl";
            this.MI_AntiColl.UseVisualStyleBackColor = true;
            this.MI_AntiColl.Click += new System.EventHandler(this.MI_AntiColl_Click);
            // 
            // MI_Select
            // 
            this.MI_Select.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MI_Select.Location = new System.Drawing.Point(102, 73);
            this.MI_Select.Name = "MI_Select";
            this.MI_Select.Size = new System.Drawing.Size(61, 23);
            this.MI_Select.TabIndex = 18;
            this.MI_Select.Text = "Select";
            this.MI_Select.UseVisualStyleBackColor = true;
            this.MI_Select.Click += new System.EventHandler(this.MI_Select_Click);
            // 
            // button_RATS
            // 
            this.button_RATS.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_RATS.Location = new System.Drawing.Point(11, 68);
            this.button_RATS.Name = "button_RATS";
            this.button_RATS.Size = new System.Drawing.Size(75, 43);
            this.button_RATS.TabIndex = 50;
            this.button_RATS.Text = "RATS";
            this.button_RATS.UseVisualStyleBackColor = true;
            this.button_RATS.Click += new System.EventHandler(this.button_RATS_Click);
            // 
            // pps_exchange_CL_btn
            // 
            this.pps_exchange_CL_btn.Location = new System.Drawing.Point(11, 174);
            this.pps_exchange_CL_btn.Name = "pps_exchange_CL_btn";
            this.pps_exchange_CL_btn.Size = new System.Drawing.Size(75, 36);
            this.pps_exchange_CL_btn.TabIndex = 88;
            this.pps_exchange_CL_btn.Text = "PPS";
            this.pps_exchange_CL_btn.UseVisualStyleBackColor = true;
            this.pps_exchange_CL_btn.Click += new System.EventHandler(this.pps_exchange_CL_btn_Click);
            // 
            // label_UID1
            // 
            this.label_UID1.AutoSize = true;
            this.label_UID1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_UID1.Location = new System.Drawing.Point(168, 26);
            this.label_UID1.Name = "label_UID1";
            this.label_UID1.Size = new System.Drawing.Size(47, 12);
            this.label_UID1.TabIndex = 133;
            this.label_UID1.Text = "Lv1 UID";
            // 
            // WupA
            // 
            this.WupA.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.WupA.Location = new System.Drawing.Point(102, 142);
            this.WupA.Name = "WupA";
            this.WupA.Size = new System.Drawing.Size(61, 23);
            this.WupA.TabIndex = 58;
            this.WupA.Text = "WUPA";
            this.WupA.UseVisualStyleBackColor = true;
            this.WupA.Click += new System.EventHandler(this.WupA_Click);
            // 
            // label_UID2
            // 
            this.label_UID2.AutoSize = true;
            this.label_UID2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_UID2.Location = new System.Drawing.Point(168, 67);
            this.label_UID2.Name = "label_UID2";
            this.label_UID2.Size = new System.Drawing.Size(47, 12);
            this.label_UID2.TabIndex = 137;
            this.label_UID2.Text = "Lv2 UID";
            // 
            // Halt
            // 
            this.Halt.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Halt.Location = new System.Drawing.Point(103, 113);
            this.Halt.Name = "Halt";
            this.Halt.Size = new System.Drawing.Size(61, 23);
            this.Halt.TabIndex = 57;
            this.Halt.Text = "HLTA";
            this.Halt.UseVisualStyleBackColor = true;
            this.Halt.Click += new System.EventHandler(this.Halt_Click);
            // 
            // label_UID3
            // 
            this.label_UID3.AutoSize = true;
            this.label_UID3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_UID3.Location = new System.Drawing.Point(168, 110);
            this.label_UID3.Name = "label_UID3";
            this.label_UID3.Size = new System.Drawing.Size(47, 12);
            this.label_UID3.TabIndex = 141;
            this.label_UID3.Text = "Lv3 UID";
            // 
            // label_SAK1
            // 
            this.label_SAK1.AutoSize = true;
            this.label_SAK1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_SAK1.Location = new System.Drawing.Point(283, 26);
            this.label_SAK1.Name = "label_SAK1";
            this.label_SAK1.Size = new System.Drawing.Size(23, 12);
            this.label_SAK1.TabIndex = 135;
            this.label_SAK1.Text = "SAK";
            // 
            // label_SAK2
            // 
            this.label_SAK2.AutoSize = true;
            this.label_SAK2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_SAK2.Location = new System.Drawing.Point(283, 67);
            this.label_SAK2.Name = "label_SAK2";
            this.label_SAK2.Size = new System.Drawing.Size(23, 12);
            this.label_SAK2.TabIndex = 139;
            this.label_SAK2.Text = "SAK";
            // 
            // label_SAK3
            // 
            this.label_SAK3.AutoSize = true;
            this.label_SAK3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_SAK3.Location = new System.Drawing.Point(283, 110);
            this.label_SAK3.Name = "label_SAK3";
            this.label_SAK3.Size = new System.Drawing.Size(23, 12);
            this.label_SAK3.TabIndex = 143;
            this.label_SAK3.Text = "SAK";
            // 
            // textBox_UID1
            // 
            this.textBox_UID1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_UID1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UID1.Location = new System.Drawing.Point(185, 41);
            this.textBox_UID1.MaxLength = 2;
            this.textBox_UID1.Name = "textBox_UID1";
            this.textBox_UID1.ReadOnly = true;
            this.textBox_UID1.Size = new System.Drawing.Size(82, 23);
            this.textBox_UID1.TabIndex = 132;
            this.textBox_UID1.Text = "----------";
            this.textBox_UID1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_UID2
            // 
            this.textBox_UID2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_UID2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UID2.Location = new System.Drawing.Point(185, 83);
            this.textBox_UID2.MaxLength = 2;
            this.textBox_UID2.Name = "textBox_UID2";
            this.textBox_UID2.ReadOnly = true;
            this.textBox_UID2.Size = new System.Drawing.Size(81, 23);
            this.textBox_UID2.TabIndex = 136;
            this.textBox_UID2.Text = "----------";
            this.textBox_UID2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_UID3
            // 
            this.textBox_UID3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_UID3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UID3.Location = new System.Drawing.Point(185, 125);
            this.textBox_UID3.MaxLength = 2;
            this.textBox_UID3.Name = "textBox_UID3";
            this.textBox_UID3.ReadOnly = true;
            this.textBox_UID3.Size = new System.Drawing.Size(81, 23);
            this.textBox_UID3.TabIndex = 140;
            this.textBox_UID3.Text = "----------";
            this.textBox_UID3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_SAK1
            // 
            this.textBox_SAK1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_SAK1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SAK1.Location = new System.Drawing.Point(281, 41);
            this.textBox_SAK1.MaxLength = 2;
            this.textBox_SAK1.Name = "textBox_SAK1";
            this.textBox_SAK1.ReadOnly = true;
            this.textBox_SAK1.Size = new System.Drawing.Size(25, 23);
            this.textBox_SAK1.TabIndex = 134;
            this.textBox_SAK1.Text = "--";
            this.textBox_SAK1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_SAK2
            // 
            this.textBox_SAK2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_SAK2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SAK2.Location = new System.Drawing.Point(281, 83);
            this.textBox_SAK2.MaxLength = 2;
            this.textBox_SAK2.Name = "textBox_SAK2";
            this.textBox_SAK2.ReadOnly = true;
            this.textBox_SAK2.Size = new System.Drawing.Size(25, 23);
            this.textBox_SAK2.TabIndex = 138;
            this.textBox_SAK2.Text = "--";
            this.textBox_SAK2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_SAK3
            // 
            this.textBox_SAK3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_SAK3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SAK3.Location = new System.Drawing.Point(281, 125);
            this.textBox_SAK3.MaxLength = 2;
            this.textBox_SAK3.Name = "textBox_SAK3";
            this.textBox_SAK3.ReadOnly = true;
            this.textBox_SAK3.Size = new System.Drawing.Size(25, 23);
            this.textBox_SAK3.TabIndex = 142;
            this.textBox_SAK3.Text = "--";
            this.textBox_SAK3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_last_SAK
            // 
            this.textBox_last_SAK.Enabled = false;
            this.textBox_last_SAK.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_last_SAK.Location = new System.Drawing.Point(281, 161);
            this.textBox_last_SAK.MaxLength = 2;
            this.textBox_last_SAK.Name = "textBox_last_SAK";
            this.textBox_last_SAK.Size = new System.Drawing.Size(24, 23);
            this.textBox_last_SAK.TabIndex = 145;
            this.textBox_last_SAK.Text = "28";
            this.textBox_last_SAK.TextChanged += new System.EventHandler(this.textBox_last_SAK_TextChanged);
            // 
            // textBox_PPSCL_DSI
            // 
            this.textBox_PPSCL_DSI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_PPSCL_DSI.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PPSCL_DSI.Location = new System.Drawing.Point(32, 149);
            this.textBox_PPSCL_DSI.MaxLength = 2;
            this.textBox_PPSCL_DSI.Name = "textBox_PPSCL_DSI";
            this.textBox_PPSCL_DSI.Size = new System.Drawing.Size(18, 21);
            this.textBox_PPSCL_DSI.TabIndex = 94;
            this.textBox_PPSCL_DSI.Text = "0";
            // 
            // groupBox_CLB_CONTROL
            // 
            this.groupBox_CLB_CONTROL.Controls.Add(this.comboBox_CLB_EOF);
            this.groupBox_CLB_CONTROL.Controls.Add(this.comboBox_CLB_SOF);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label_EGT);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_EGT);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_TypeBFraming);
            this.groupBox_CLB_CONTROL.Controls.Add(this.checkBox_CLB_EOF);
            this.groupBox_CLB_CONTROL.Controls.Add(this.checkBox_CLB_SOF);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label_parameters);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_Parameters);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label_PUPI);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_PUPI);
            this.groupBox_CLB_CONTROL.Controls.Add(this.checkBox_ExtREQB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label_Slots);
            this.groupBox_CLB_CONTROL.Controls.Add(this.comboBox_N);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_DRI_CLB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label63);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label65);
            this.groupBox_CLB_CONTROL.Controls.Add(this.label_AFI);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_AFI);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_Deselect_CLB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_REQB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_SlotMARKER);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_ATTRIB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_PPS_CLB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_WUPB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.button_HLTB);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_SlotNumber);
            this.groupBox_CLB_CONTROL.Controls.Add(this.textBox_DSI_CLB);
            this.groupBox_CLB_CONTROL.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_CLB_CONTROL.Location = new System.Drawing.Point(6, 75);
            this.groupBox_CLB_CONTROL.Name = "groupBox_CLB_CONTROL";
            this.groupBox_CLB_CONTROL.Size = new System.Drawing.Size(309, 216);
            this.groupBox_CLB_CONTROL.TabIndex = 149;
            this.groupBox_CLB_CONTROL.TabStop = false;
            this.groupBox_CLB_CONTROL.Text = "Type B Activation";
            this.groupBox_CLB_CONTROL.Visible = false;
            // 
            // comboBox_CLB_EOF
            // 
            this.comboBox_CLB_EOF.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBox_CLB_EOF.FormattingEnabled = true;
            this.comboBox_CLB_EOF.Items.AddRange(new object[] {
            "10low",
            "11low"});
            this.comboBox_CLB_EOF.Location = new System.Drawing.Point(209, 140);
            this.comboBox_CLB_EOF.Name = "comboBox_CLB_EOF";
            this.comboBox_CLB_EOF.Size = new System.Drawing.Size(89, 20);
            this.comboBox_CLB_EOF.TabIndex = 163;
            this.comboBox_CLB_EOF.Text = "10low";
            this.comboBox_CLB_EOF.SelectedIndexChanged += new System.EventHandler(this.comboBox_CLB_EOF_SelectedIndexChanged);
            // 
            // comboBox_CLB_SOF
            // 
            this.comboBox_CLB_SOF.Font = new System.Drawing.Font("宋体", 9F);
            this.comboBox_CLB_SOF.FormattingEnabled = true;
            this.comboBox_CLB_SOF.Items.AddRange(new object[] {
            "10low+2high",
            "10low+3high",
            "11low+2high",
            "11low+3high"});
            this.comboBox_CLB_SOF.Location = new System.Drawing.Point(209, 118);
            this.comboBox_CLB_SOF.Name = "comboBox_CLB_SOF";
            this.comboBox_CLB_SOF.Size = new System.Drawing.Size(89, 20);
            this.comboBox_CLB_SOF.TabIndex = 161;
            this.comboBox_CLB_SOF.Text = "10low+2high";
            this.comboBox_CLB_SOF.SelectedIndexChanged += new System.EventHandler(this.comboBox_CLB_SOF_SelectedIndexChanged);
            // 
            // label_EGT
            // 
            this.label_EGT.AutoSize = true;
            this.label_EGT.Font = new System.Drawing.Font("宋体", 10F);
            this.label_EGT.Location = new System.Drawing.Point(169, 167);
            this.label_EGT.Name = "label_EGT";
            this.label_EGT.Size = new System.Drawing.Size(49, 14);
            this.label_EGT.TabIndex = 168;
            this.label_EGT.Text = "EGT Ox";
            // 
            // textBox_EGT
            // 
            this.textBox_EGT.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_EGT.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_EGT.Location = new System.Drawing.Point(218, 164);
            this.textBox_EGT.MaxLength = 1;
            this.textBox_EGT.Name = "textBox_EGT";
            this.textBox_EGT.Size = new System.Drawing.Size(17, 23);
            this.textBox_EGT.TabIndex = 167;
            this.textBox_EGT.Text = "0";
            this.textBox_EGT.TextChanged += new System.EventHandler(this.textBox_EGT_TextChanged);
            // 
            // button_TypeBFraming
            // 
            this.button_TypeBFraming.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_TypeBFraming.Location = new System.Drawing.Point(102, 190);
            this.button_TypeBFraming.Name = "button_TypeBFraming";
            this.button_TypeBFraming.Size = new System.Drawing.Size(89, 23);
            this.button_TypeBFraming.TabIndex = 166;
            this.button_TypeBFraming.Text = "Type B Frame";
            this.button_TypeBFraming.UseVisualStyleBackColor = true;
            this.button_TypeBFraming.Click += new System.EventHandler(this.button_TypeBFraming_Click);
            this.button_TypeBFraming.MouseEnter += new System.EventHandler(this.button_TypeBFraming_MouseEnter);
            this.button_TypeBFraming.MouseLeave += new System.EventHandler(this.button_TypeBFraming_MouseLeave);
            // 
            // checkBox_CLB_EOF
            // 
            this.checkBox_CLB_EOF.AutoSize = true;
            this.checkBox_CLB_EOF.Checked = true;
            this.checkBox_CLB_EOF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CLB_EOF.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_CLB_EOF.Location = new System.Drawing.Point(170, 144);
            this.checkBox_CLB_EOF.Name = "checkBox_CLB_EOF";
            this.checkBox_CLB_EOF.Size = new System.Drawing.Size(42, 16);
            this.checkBox_CLB_EOF.TabIndex = 165;
            this.checkBox_CLB_EOF.Text = "EOF";
            this.checkBox_CLB_EOF.UseVisualStyleBackColor = true;
            this.checkBox_CLB_EOF.CheckedChanged += new System.EventHandler(this.checkBox_CLB_EOF_CheckedChanged);
            // 
            // checkBox_CLB_SOF
            // 
            this.checkBox_CLB_SOF.AutoSize = true;
            this.checkBox_CLB_SOF.Checked = true;
            this.checkBox_CLB_SOF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CLB_SOF.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_CLB_SOF.Location = new System.Drawing.Point(170, 121);
            this.checkBox_CLB_SOF.Name = "checkBox_CLB_SOF";
            this.checkBox_CLB_SOF.Size = new System.Drawing.Size(42, 16);
            this.checkBox_CLB_SOF.TabIndex = 164;
            this.checkBox_CLB_SOF.Text = "SOF";
            this.checkBox_CLB_SOF.UseVisualStyleBackColor = true;
            this.checkBox_CLB_SOF.CheckedChanged += new System.EventHandler(this.checkBox_CLB_SOF_CheckedChanged);
            // 
            // label_parameters
            // 
            this.label_parameters.AutoSize = true;
            this.label_parameters.Font = new System.Drawing.Font("宋体", 10F);
            this.label_parameters.Location = new System.Drawing.Point(114, 92);
            this.label_parameters.Name = "label_parameters";
            this.label_parameters.Size = new System.Drawing.Size(63, 14);
            this.label_parameters.TabIndex = 158;
            this.label_parameters.Text = "Param 0x";
            // 
            // textBox_Parameters
            // 
            this.textBox_Parameters.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_Parameters.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Parameters.Location = new System.Drawing.Point(179, 90);
            this.textBox_Parameters.MaxLength = 8;
            this.textBox_Parameters.Name = "textBox_Parameters";
            this.textBox_Parameters.Size = new System.Drawing.Size(66, 23);
            this.textBox_Parameters.TabIndex = 157;
            this.textBox_Parameters.Text = "00000000";
            this.textBox_Parameters.TextChanged += new System.EventHandler(this.textBox_Parameters_TextChanged);
            this.textBox_Parameters.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_Parameters_KeyPress);
            this.textBox_Parameters.Leave += new System.EventHandler(this.textBox_Parameters_Leave);
            // 
            // label_PUPI
            // 
            this.label_PUPI.AutoSize = true;
            this.label_PUPI.Font = new System.Drawing.Font("宋体", 10F);
            this.label_PUPI.Location = new System.Drawing.Point(121, 66);
            this.label_PUPI.Name = "label_PUPI";
            this.label_PUPI.Size = new System.Drawing.Size(56, 14);
            this.label_PUPI.TabIndex = 156;
            this.label_PUPI.Text = "PUPI 0x";
            // 
            // textBox_PUPI
            // 
            this.textBox_PUPI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_PUPI.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PUPI.Location = new System.Drawing.Point(179, 62);
            this.textBox_PUPI.MaxLength = 8;
            this.textBox_PUPI.Name = "textBox_PUPI";
            this.textBox_PUPI.Size = new System.Drawing.Size(66, 23);
            this.textBox_PUPI.TabIndex = 155;
            this.textBox_PUPI.Text = "00000000";
            this.textBox_PUPI.TextChanged += new System.EventHandler(this.textBox_PUPI_TextChanged);
            this.textBox_PUPI.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_PUPI_KeyPress);
            this.textBox_PUPI.Leave += new System.EventHandler(this.textBox_PUPI_Leave);
            // 
            // checkBox_ExtREQB
            // 
            this.checkBox_ExtREQB.AutoSize = true;
            this.checkBox_ExtREQB.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_ExtREQB.Location = new System.Drawing.Point(102, 46);
            this.checkBox_ExtREQB.Name = "checkBox_ExtREQB";
            this.checkBox_ExtREQB.Size = new System.Drawing.Size(132, 16);
            this.checkBox_ExtREQB.TabIndex = 153;
            this.checkBox_ExtREQB.Text = "Extended REQB/WUPB";
            this.checkBox_ExtREQB.UseVisualStyleBackColor = true;
            // 
            // label_Slots
            // 
            this.label_Slots.AutoSize = true;
            this.label_Slots.Font = new System.Drawing.Font("宋体", 9F);
            this.label_Slots.Location = new System.Drawing.Point(155, 23);
            this.label_Slots.Name = "label_Slots";
            this.label_Slots.Size = new System.Drawing.Size(35, 12);
            this.label_Slots.TabIndex = 152;
            this.label_Slots.Text = "Slots";
            // 
            // comboBox_N
            // 
            this.comboBox_N.FormattingEnabled = true;
            this.comboBox_N.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16"});
            this.comboBox_N.Location = new System.Drawing.Point(191, 20);
            this.comboBox_N.Name = "comboBox_N";
            this.comboBox_N.Size = new System.Drawing.Size(40, 21);
            this.comboBox_N.TabIndex = 151;
            this.comboBox_N.Text = "1";
            // 
            // textBox_DRI_CLB
            // 
            this.textBox_DRI_CLB.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_DRI_CLB.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_DRI_CLB.Location = new System.Drawing.Point(74, 156);
            this.textBox_DRI_CLB.MaxLength = 2;
            this.textBox_DRI_CLB.Name = "textBox_DRI_CLB";
            this.textBox_DRI_CLB.Size = new System.Drawing.Size(16, 21);
            this.textBox_DRI_CLB.TabIndex = 96;
            this.textBox_DRI_CLB.Text = "0";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label63.Location = new System.Drawing.Point(9, 159);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(23, 12);
            this.label63.TabIndex = 95;
            this.label63.Text = "DSI";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label65.Location = new System.Drawing.Point(50, 159);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(23, 12);
            this.label65.TabIndex = 97;
            this.label65.Text = "DRI";
            // 
            // label_AFI
            // 
            this.label_AFI.AutoSize = true;
            this.label_AFI.Font = new System.Drawing.Font("宋体", 9F);
            this.label_AFI.Location = new System.Drawing.Point(101, 23);
            this.label_AFI.Name = "label_AFI";
            this.label_AFI.Size = new System.Drawing.Size(23, 12);
            this.label_AFI.TabIndex = 148;
            this.label_AFI.Text = "AFI";
            // 
            // textBox_AFI
            // 
            this.textBox_AFI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_AFI.Font = new System.Drawing.Font("宋体", 10F);
            this.textBox_AFI.Location = new System.Drawing.Point(125, 20);
            this.textBox_AFI.MaxLength = 2;
            this.textBox_AFI.Name = "textBox_AFI";
            this.textBox_AFI.Size = new System.Drawing.Size(22, 23);
            this.textBox_AFI.TabIndex = 148;
            this.textBox_AFI.Text = "00";
            this.textBox_AFI.TextChanged += new System.EventHandler(this.textBox_AFI_TextChanged);
            this.textBox_AFI.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_AFI_KeyPress);
            this.textBox_AFI.Leave += new System.EventHandler(this.textBox_AFI_Leave);
            // 
            // button_Deselect_CLB
            // 
            this.button_Deselect_CLB.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Deselect_CLB.Location = new System.Drawing.Point(102, 164);
            this.button_Deselect_CLB.Name = "button_Deselect_CLB";
            this.button_Deselect_CLB.Size = new System.Drawing.Size(61, 23);
            this.button_Deselect_CLB.TabIndex = 112;
            this.button_Deselect_CLB.Text = "Deselect";
            this.button_Deselect_CLB.UseVisualStyleBackColor = true;
            this.button_Deselect_CLB.Click += new System.EventHandler(this.button_Deselect_CLB_Click);
            // 
            // button_REQB
            // 
            this.button_REQB.Font = new System.Drawing.Font("宋体", 10F);
            this.button_REQB.Location = new System.Drawing.Point(11, 19);
            this.button_REQB.Name = "button_REQB";
            this.button_REQB.Size = new System.Drawing.Size(75, 45);
            this.button_REQB.TabIndex = 16;
            this.button_REQB.Text = "REQB";
            this.button_REQB.UseVisualStyleBackColor = true;
            this.button_REQB.Click += new System.EventHandler(this.button_REQB_Click);
            // 
            // button_SlotMARKER
            // 
            this.button_SlotMARKER.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_SlotMARKER.Location = new System.Drawing.Point(11, 70);
            this.button_SlotMARKER.Name = "button_SlotMARKER";
            this.button_SlotMARKER.Size = new System.Drawing.Size(87, 23);
            this.button_SlotMARKER.TabIndex = 17;
            this.button_SlotMARKER.Text = "Slot_MARKER";
            this.button_SlotMARKER.UseVisualStyleBackColor = true;
            this.button_SlotMARKER.Click += new System.EventHandler(this.button_SlotMARKER_Click);
            // 
            // button_ATTRIB
            // 
            this.button_ATTRIB.Font = new System.Drawing.Font("宋体", 10F);
            this.button_ATTRIB.Location = new System.Drawing.Point(12, 101);
            this.button_ATTRIB.Name = "button_ATTRIB";
            this.button_ATTRIB.Size = new System.Drawing.Size(74, 44);
            this.button_ATTRIB.TabIndex = 18;
            this.button_ATTRIB.Text = "ATTRIB";
            this.button_ATTRIB.UseVisualStyleBackColor = true;
            this.button_ATTRIB.Click += new System.EventHandler(this.button_ATTRIB_Click);
            // 
            // button_PPS_CLB
            // 
            this.button_PPS_CLB.Location = new System.Drawing.Point(12, 183);
            this.button_PPS_CLB.Name = "button_PPS_CLB";
            this.button_PPS_CLB.Size = new System.Drawing.Size(75, 30);
            this.button_PPS_CLB.TabIndex = 88;
            this.button_PPS_CLB.Text = "PPS";
            this.button_PPS_CLB.UseVisualStyleBackColor = true;
            this.button_PPS_CLB.Click += new System.EventHandler(this.button_PPS_CLB_Click);
            // 
            // button_WUPB
            // 
            this.button_WUPB.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_WUPB.Location = new System.Drawing.Point(102, 138);
            this.button_WUPB.Name = "button_WUPB";
            this.button_WUPB.Size = new System.Drawing.Size(61, 23);
            this.button_WUPB.TabIndex = 58;
            this.button_WUPB.Text = "WUPB";
            this.button_WUPB.UseVisualStyleBackColor = true;
            this.button_WUPB.Click += new System.EventHandler(this.button_WUPB_Click);
            // 
            // button_HLTB
            // 
            this.button_HLTB.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_HLTB.Location = new System.Drawing.Point(102, 111);
            this.button_HLTB.Name = "button_HLTB";
            this.button_HLTB.Size = new System.Drawing.Size(61, 23);
            this.button_HLTB.TabIndex = 57;
            this.button_HLTB.Text = "HLTB";
            this.button_HLTB.UseVisualStyleBackColor = true;
            this.button_HLTB.Click += new System.EventHandler(this.button_HLTB_Click);
            // 
            // textBox_SlotNumber
            // 
            this.textBox_SlotNumber.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_SlotNumber.Location = new System.Drawing.Point(100, 70);
            this.textBox_SlotNumber.MaxLength = 1;
            this.textBox_SlotNumber.Name = "textBox_SlotNumber";
            this.textBox_SlotNumber.Size = new System.Drawing.Size(13, 23);
            this.textBox_SlotNumber.TabIndex = 145;
            this.textBox_SlotNumber.Text = "0";
            this.textBox_SlotNumber.TextChanged += new System.EventHandler(this.textBox_SlotNumber_TextChanged);
            // 
            // textBox_DSI_CLB
            // 
            this.textBox_DSI_CLB.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_DSI_CLB.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_DSI_CLB.Location = new System.Drawing.Point(32, 156);
            this.textBox_DSI_CLB.MaxLength = 2;
            this.textBox_DSI_CLB.Name = "textBox_DSI_CLB";
            this.textBox_DSI_CLB.Size = new System.Drawing.Size(16, 21);
            this.textBox_DSI_CLB.TabIndex = 94;
            this.textBox_DSI_CLB.Text = "0";
            // 
            // tabPage_CT
            // 
            this.tabPage_CT.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_CT.Controls.Add(this.btnWriteUID);
            this.tabPage_CT.Controls.Add(this.btnReadUID);
            this.tabPage_CT.Controls.Add(this.Send_Slected_Command_button);
            this.tabPage_CT.Controls.Add(this.CT_OPEN_CMDLOG_button);
            this.tabPage_CT.Controls.Add(this.groupBox21);
            this.tabPage_CT.Controls.Add(this.trim_wr);
            this.tabPage_CT.Controls.Add(this.trim8m);
            this.tabPage_CT.Controls.Add(this.text_0B_P3);
            this.tabPage_CT.Controls.Add(this.ins54);
            this.tabPage_CT.Controls.Add(this.CT_347_STOPMCU);
            this.tabPage_CT.Controls.Add(this.groupBox10);
            this.tabPage_CT.Controls.Add(this.FreqRead);
            this.tabPage_CT.Controls.Add(this.init_AUTH_55);
            this.tabPage_CT.Controls.Add(this.text_0A_P2);
            this.tabPage_CT.Controls.Add(this.text_0A_P1);
            this.tabPage_CT.Controls.Add(this.init_CRC_0A);
            this.tabPage_CT.Controls.Add(this.label59);
            this.tabPage_CT.Controls.Add(this.PPS_exchange_ct_tbox);
            this.tabPage_CT.Controls.Add(this.PPS_exchange_ct_btn);
            this.tabPage_CT.Controls.Add(this.label56);
            this.tabPage_CT.Controls.Add(this.text_0B_P2);
            this.tabPage_CT.Controls.Add(this.text_0B_P1);
            this.tabPage_CT.Controls.Add(this.Init_Trim_0B);
            this.tabPage_CT.Controls.Add(this.text_03_P2);
            this.tabPage_CT.Controls.Add(this.text_03_P1);
            this.tabPage_CT.Controls.Add(this.Init_ewEEtime_03);
            this.tabPage_CT.Controls.Add(this.text_08_P1);
            this.tabPage_CT.Controls.Add(this.Init_CPUStart_08);
            this.tabPage_CT.Controls.Add(this.button3);
            this.tabPage_CT.Controls.Add(this.text_01_P1);
            this.tabPage_CT.Controls.Add(this.text_04_len);
            this.tabPage_CT.Controls.Add(this.text_00_len);
            this.tabPage_CT.Controls.Add(this.text_02_P2);
            this.tabPage_CT.Controls.Add(this.text_04_P2);
            this.tabPage_CT.Controls.Add(this.text_00_P2);
            this.tabPage_CT.Controls.Add(this.label50);
            this.tabPage_CT.Controls.Add(this.label49);
            this.tabPage_CT.Controls.Add(this.text_02_P1);
            this.tabPage_CT.Controls.Add(this.text_04_P1);
            this.tabPage_CT.Controls.Add(this.text_00_P1);
            this.tabPage_CT.Controls.Add(this.label48);
            this.tabPage_CT.Controls.Add(this.Init_Rdee_04);
            this.tabPage_CT.Controls.Add(this.Init_EEopt_02);
            this.tabPage_CT.Controls.Add(this.Init_01);
            this.tabPage_CT.Controls.Add(this.Init_Wree_00);
            this.tabPage_CT.Controls.Add(this.Parity_groupBox);
            this.tabPage_CT.Controls.Add(this.TransceiveDataCT_listBox);
            this.tabPage_CT.Controls.Add(this.AddCMDtoListBox_CT_btn);
            this.tabPage_CT.Controls.Add(this.textBox_APDU_P3_CT);
            this.tabPage_CT.Controls.Add(this.textBox_APDU_P2_CT);
            this.tabPage_CT.Controls.Add(this.textBox_APDU_P1_CT);
            this.tabPage_CT.Controls.Add(this.textBox_APDU_INS_CT);
            this.tabPage_CT.Controls.Add(this.textBox_APDU_CLA_CT);
            this.tabPage_CT.Controls.Add(this.checkBox_APDUmodeCT);
            this.tabPage_CT.Controls.Add(this.label_APDU_CT);
            this.tabPage_CT.Controls.Add(this.TR_CT_TimeOut);
            this.tabPage_CT.Controls.Add(this.Clock_Stop_Btn);
            this.tabPage_CT.Controls.Add(this.CST_select);
            this.tabPage_CT.Controls.Add(this.TransceiveCT);
            this.tabPage_CT.Controls.Add(this.VoltageSel);
            this.tabPage_CT.Controls.Add(this.Warm_Reset);
            this.tabPage_CT.Controls.Add(this.label6);
            this.tabPage_CT.Controls.Add(this.label7);
            this.tabPage_CT.Controls.Add(this.RegData_TDA_textBox);
            this.tabPage_CT.Controls.Add(this.RegAddr_TDA_textBox);
            this.tabPage_CT.Controls.Add(this.WriteTDAReg);
            this.tabPage_CT.Controls.Add(this.ReadTDAReg);
            this.tabPage_CT.Controls.Add(this.Cold_Reset);
            this.tabPage_CT.Controls.Add(this.Init_TDA8007);
            this.tabPage_CT.Location = new System.Drawing.Point(4, 22);
            this.tabPage_CT.Name = "tabPage_CT";
            this.tabPage_CT.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_CT.Size = new System.Drawing.Size(956, 406);
            this.tabPage_CT.TabIndex = 1;
            this.tabPage_CT.Text = "接触接口";
            // 
            // btnWriteUID
            // 
            this.btnWriteUID.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnWriteUID.Location = new System.Drawing.Point(657, 323);
            this.btnWriteUID.Name = "btnWriteUID";
            this.btnWriteUID.Size = new System.Drawing.Size(77, 27);
            this.btnWriteUID.TabIndex = 105;
            this.btnWriteUID.Text = "Write UID";
            this.btnWriteUID.UseVisualStyleBackColor = true;
            this.btnWriteUID.Visible = false;
            this.btnWriteUID.Click += new System.EventHandler(this.btnWriteUID_Click);
            // 
            // btnReadUID
            // 
            this.btnReadUID.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReadUID.Location = new System.Drawing.Point(657, 294);
            this.btnReadUID.Name = "btnReadUID";
            this.btnReadUID.Size = new System.Drawing.Size(77, 27);
            this.btnReadUID.TabIndex = 104;
            this.btnReadUID.Text = "Read UID";
            this.btnReadUID.UseVisualStyleBackColor = true;
            this.btnReadUID.Visible = false;
            this.btnReadUID.Click += new System.EventHandler(this.btnReadUID_Click);
            // 
            // Send_Slected_Command_button
            // 
            this.Send_Slected_Command_button.Location = new System.Drawing.Point(562, 260);
            this.Send_Slected_Command_button.Name = "Send_Slected_Command_button";
            this.Send_Slected_Command_button.Size = new System.Drawing.Size(89, 42);
            this.Send_Slected_Command_button.TabIndex = 103;
            this.Send_Slected_Command_button.Text = "顺序执行";
            this.toolTip_CLIterface.SetToolTip(this.Send_Slected_Command_button, "选中多行，可顺序执行");
            this.Send_Slected_Command_button.UseVisualStyleBackColor = true;
            this.Send_Slected_Command_button.Click += new System.EventHandler(this.Send_Slected_Command_button_Click);
            // 
            // CT_OPEN_CMDLOG_button
            // 
            this.CT_OPEN_CMDLOG_button.Location = new System.Drawing.Point(562, 308);
            this.CT_OPEN_CMDLOG_button.Name = "CT_OPEN_CMDLOG_button";
            this.CT_OPEN_CMDLOG_button.Size = new System.Drawing.Size(89, 42);
            this.CT_OPEN_CMDLOG_button.TabIndex = 102;
            this.CT_OPEN_CMDLOG_button.Text = "OPEN";
            this.CT_OPEN_CMDLOG_button.UseVisualStyleBackColor = true;
            this.CT_OPEN_CMDLOG_button.Click += new System.EventHandler(this.Open_CMDlog_button_Click);
            // 
            // groupBox21
            // 
            this.groupBox21.Controls.Add(this.aux_s);
            this.groupBox21.Controls.Add(this.label89);
            this.groupBox21.Controls.Add(this.button14);
            this.groupBox21.Location = new System.Drawing.Point(623, 129);
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.Size = new System.Drawing.Size(117, 90);
            this.groupBox21.TabIndex = 101;
            this.groupBox21.TabStop = false;
            this.groupBox21.Text = "AUX_sel";
            this.groupBox21.Visible = false;
            // 
            // aux_s
            // 
            this.aux_s.FormattingEnabled = true;
            this.aux_s.Items.AddRange(new object[] {
            "verf1p2",
            "vref_flash",
            "vdet_vb",
            "vdd12",
            "vdd12_ana",
            "vdd16",
            "vdd20",
            "vbias1p2",
            "vtemp",
            "vref_htm",
            "vref_ltm"});
            this.aux_s.Location = new System.Drawing.Point(7, 14);
            this.aux_s.Name = "aux_s";
            this.aux_s.Size = new System.Drawing.Size(103, 20);
            this.aux_s.TabIndex = 100;
            this.aux_s.Text = "输出电压选择";
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(18, 35);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(83, 12);
            this.label89.TabIndex = 100;
            this.label89.Text = "观测PA7-GPIO4";
            // 
            // button14
            // 
            this.button14.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button14.Location = new System.Drawing.Point(14, 51);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(90, 34);
            this.button14.TabIndex = 99;
            this.button14.Text = "AUX_out";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // trim_wr
            // 
            this.trim_wr.AutoSize = true;
            this.trim_wr.Location = new System.Drawing.Point(656, 112);
            this.trim_wr.Name = "trim_wr";
            this.trim_wr.Size = new System.Drawing.Size(96, 16);
            this.trim_wr.TabIndex = 98;
            this.trim_wr.Text = "trim值写入CW";
            this.trim_wr.UseVisualStyleBackColor = true;
            this.trim_wr.Visible = false;
            // 
            // trim8m
            // 
            this.trim8m.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.trim8m.Location = new System.Drawing.Point(656, 69);
            this.trim8m.Name = "trim8m";
            this.trim8m.Size = new System.Drawing.Size(77, 37);
            this.trim8m.TabIndex = 97;
            this.trim8m.Text = "8M trim";
            this.trim8m.UseVisualStyleBackColor = true;
            this.trim8m.Visible = false;
            this.trim8m.Click += new System.EventHandler(this.trim8m_Click);
            // 
            // text_0B_P3
            // 
            this.text_0B_P3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_0B_P3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_0B_P3.Location = new System.Drawing.Point(924, 258);
            this.text_0B_P3.MaxLength = 2;
            this.text_0B_P3.Name = "text_0B_P3";
            this.text_0B_P3.Size = new System.Drawing.Size(25, 23);
            this.text_0B_P3.TabIndex = 96;
            this.text_0B_P3.Text = "10";
            this.text_0B_P3.Visible = false;
            // 
            // ins54
            // 
            this.ins54.Controls.Add(this.wr3e);
            this.ins54.Controls.Add(this.rd05);
            this.ins54.Location = new System.Drawing.Point(685, 228);
            this.ins54.Name = "ins54";
            this.ins54.Size = new System.Drawing.Size(55, 43);
            this.ins54.TabIndex = 95;
            this.ins54.TabStop = false;
            this.ins54.Text = "ins54";
            this.ins54.Visible = false;
            // 
            // wr3e
            // 
            this.wr3e.AutoSize = true;
            this.wr3e.Location = new System.Drawing.Point(4, 25);
            this.wr3e.Name = "wr3e";
            this.wr3e.Size = new System.Drawing.Size(53, 16);
            this.wr3e.TabIndex = 1;
            this.wr3e.TabStop = true;
            this.wr3e.Text = "wr 3E";
            this.wr3e.UseVisualStyleBackColor = true;
            // 
            // rd05
            // 
            this.rd05.AutoSize = true;
            this.rd05.Location = new System.Drawing.Point(4, 10);
            this.rd05.Name = "rd05";
            this.rd05.Size = new System.Drawing.Size(53, 16);
            this.rd05.TabIndex = 0;
            this.rd05.TabStop = true;
            this.rd05.Text = "rd 05";
            this.rd05.UseVisualStyleBackColor = true;
            // 
            // CT_347_STOPMCU
            // 
            this.CT_347_STOPMCU.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CT_347_STOPMCU.Location = new System.Drawing.Point(656, 28);
            this.CT_347_STOPMCU.Name = "CT_347_STOPMCU";
            this.CT_347_STOPMCU.Size = new System.Drawing.Size(77, 32);
            this.CT_347_STOPMCU.TabIndex = 92;
            this.CT_347_STOPMCU.Text = "STOP MCU";
            this.CT_347_STOPMCU.UseVisualStyleBackColor = true;
            this.CT_347_STOPMCU.Visible = false;
            this.CT_347_STOPMCU.Click += new System.EventHandler(this.CT_347_STOPMCU_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.r347);
            this.groupBox10.Controls.Add(this.radioButton1);
            this.groupBox10.Controls.Add(this.radioButton2);
            this.groupBox10.Location = new System.Drawing.Point(751, 6);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(94, 61);
            this.groupBox10.TabIndex = 91;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "项目名";
            // 
            // r347
            // 
            this.r347.AutoSize = true;
            this.r347.Location = new System.Drawing.Point(6, 26);
            this.r347.Name = "r347";
            this.r347.Size = new System.Drawing.Size(41, 16);
            this.r347.TabIndex = 2;
            this.r347.Text = "347";
            this.r347.UseVisualStyleBackColor = true;
            this.r347.CheckedChanged += new System.EventHandler(this.r347_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(47, 12);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(41, 16);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.Text = "336";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 12);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(41, 16);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "309";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // FreqRead
            // 
            this.FreqRead.Location = new System.Drawing.Point(657, 353);
            this.FreqRead.Name = "FreqRead";
            this.FreqRead.Size = new System.Drawing.Size(67, 41);
            this.FreqRead.TabIndex = 90;
            this.FreqRead.Text = "trim指令只读频率";
            this.FreqRead.UseVisualStyleBackColor = true;
            this.FreqRead.Click += new System.EventHandler(this.FreqRead_Click);
            // 
            // init_AUTH_55
            // 
            this.init_AUTH_55.Location = new System.Drawing.Point(751, 364);
            this.init_AUTH_55.Name = "init_AUTH_55";
            this.init_AUTH_55.Size = new System.Drawing.Size(86, 30);
            this.init_AUTH_55.TabIndex = 89;
            this.init_AUTH_55.Text = "认证 55";
            this.init_AUTH_55.UseVisualStyleBackColor = true;
            this.init_AUTH_55.Click += new System.EventHandler(this.init_AUTH_55_Click);
            // 
            // text_0A_P2
            // 
            this.text_0A_P2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_0A_P2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_0A_P2.Location = new System.Drawing.Point(885, 293);
            this.text_0A_P2.MaxLength = 2;
            this.text_0A_P2.Name = "text_0A_P2";
            this.text_0A_P2.Size = new System.Drawing.Size(25, 23);
            this.text_0A_P2.TabIndex = 88;
            this.text_0A_P2.Text = "01";
            // 
            // text_0A_P1
            // 
            this.text_0A_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_0A_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_0A_P1.Location = new System.Drawing.Point(843, 293);
            this.text_0A_P1.MaxLength = 2;
            this.text_0A_P1.Name = "text_0A_P1";
            this.text_0A_P1.Size = new System.Drawing.Size(25, 23);
            this.text_0A_P1.TabIndex = 87;
            this.text_0A_P1.Text = "00";
            // 
            // init_CRC_0A
            // 
            this.init_CRC_0A.Location = new System.Drawing.Point(751, 290);
            this.init_CRC_0A.Name = "init_CRC_0A";
            this.init_CRC_0A.Size = new System.Drawing.Size(86, 30);
            this.init_CRC_0A.TabIndex = 86;
            this.init_CRC_0A.Text = "取CRC 0A";
            this.init_CRC_0A.UseVisualStyleBackColor = true;
            this.init_CRC_0A.Click += new System.EventHandler(this.init_CRC_0A_Click);
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label59.Location = new System.Drawing.Point(21, 165);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(42, 14);
            this.label59.TabIndex = 85;
            this.label59.Text = "PPS1:";
            // 
            // PPS_exchange_ct_tbox
            // 
            this.PPS_exchange_ct_tbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.PPS_exchange_ct_tbox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PPS_exchange_ct_tbox.Location = new System.Drawing.Point(63, 160);
            this.PPS_exchange_ct_tbox.MaxLength = 2;
            this.PPS_exchange_ct_tbox.Name = "PPS_exchange_ct_tbox";
            this.PPS_exchange_ct_tbox.Size = new System.Drawing.Size(29, 23);
            this.PPS_exchange_ct_tbox.TabIndex = 84;
            this.PPS_exchange_ct_tbox.Text = "18";
            // 
            // PPS_exchange_ct_btn
            // 
            this.PPS_exchange_ct_btn.Location = new System.Drawing.Point(18, 186);
            this.PPS_exchange_ct_btn.Name = "PPS_exchange_ct_btn";
            this.PPS_exchange_ct_btn.Size = new System.Drawing.Size(77, 27);
            this.PPS_exchange_ct_btn.TabIndex = 83;
            this.PPS_exchange_ct_btn.Text = "PPS";
            this.PPS_exchange_ct_btn.UseVisualStyleBackColor = true;
            this.PPS_exchange_ct_btn.Click += new System.EventHandler(this.PPS_exchange_ct_btn_Click);
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(842, 171);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(71, 12);
            this.label56.TabIndex = 77;
            this.label56.Text = "只指向前64K";
            // 
            // text_0B_P2
            // 
            this.text_0B_P2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_0B_P2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_0B_P2.Location = new System.Drawing.Point(885, 257);
            this.text_0B_P2.MaxLength = 2;
            this.text_0B_P2.Name = "text_0B_P2";
            this.text_0B_P2.Size = new System.Drawing.Size(25, 23);
            this.text_0B_P2.TabIndex = 76;
            this.text_0B_P2.Text = "05";
            // 
            // text_0B_P1
            // 
            this.text_0B_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_0B_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_0B_P1.Location = new System.Drawing.Point(843, 257);
            this.text_0B_P1.MaxLength = 2;
            this.text_0B_P1.Name = "text_0B_P1";
            this.text_0B_P1.Size = new System.Drawing.Size(25, 23);
            this.text_0B_P1.TabIndex = 75;
            this.text_0B_P1.Text = "81";
            // 
            // Init_Trim_0B
            // 
            this.Init_Trim_0B.Location = new System.Drawing.Point(751, 254);
            this.Init_Trim_0B.Name = "Init_Trim_0B";
            this.Init_Trim_0B.Size = new System.Drawing.Size(86, 30);
            this.Init_Trim_0B.TabIndex = 74;
            this.Init_Trim_0B.Text = "调校 0B";
            this.Init_Trim_0B.UseVisualStyleBackColor = true;
            this.Init_Trim_0B.Click += new System.EventHandler(this.Init_Trim_0B_Click);
            // 
            // text_03_P2
            // 
            this.text_03_P2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_03_P2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_03_P2.Location = new System.Drawing.Point(885, 221);
            this.text_03_P2.MaxLength = 2;
            this.text_03_P2.Name = "text_03_P2";
            this.text_03_P2.Size = new System.Drawing.Size(25, 23);
            this.text_03_P2.TabIndex = 73;
            this.text_03_P2.Text = "07";
            this.Hints_toolTip.SetToolTip(this.text_03_P2, "EEPROM擦写模式配置\r\nEEPROM写时间配置");
            // 
            // text_03_P1
            // 
            this.text_03_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_03_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_03_P1.Location = new System.Drawing.Point(843, 221);
            this.text_03_P1.MaxLength = 2;
            this.text_03_P1.Name = "text_03_P1";
            this.text_03_P1.Size = new System.Drawing.Size(25, 23);
            this.text_03_P1.TabIndex = 72;
            this.text_03_P1.Text = "07";
            this.Hints_toolTip.SetToolTip(this.text_03_P1, "EEPROM擦时间配置");
            // 
            // Init_ewEEtime_03
            // 
            this.Init_ewEEtime_03.Location = new System.Drawing.Point(751, 218);
            this.Init_ewEEtime_03.Name = "Init_ewEEtime_03";
            this.Init_ewEEtime_03.Size = new System.Drawing.Size(86, 30);
            this.Init_ewEEtime_03.TabIndex = 71;
            this.Init_ewEEtime_03.Text = "擦写EE 03";
            this.Init_ewEEtime_03.UseVisualStyleBackColor = true;
            this.Init_ewEEtime_03.Click += new System.EventHandler(this.Init_ewEEtime_03_Click);
            // 
            // text_08_P1
            // 
            this.text_08_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_08_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_08_P1.Location = new System.Drawing.Point(843, 329);
            this.text_08_P1.MaxLength = 2;
            this.text_08_P1.Name = "text_08_P1";
            this.text_08_P1.Size = new System.Drawing.Size(25, 23);
            this.text_08_P1.TabIndex = 70;
            this.text_08_P1.Text = "04";
            // 
            // Init_CPUStart_08
            // 
            this.Init_CPUStart_08.Location = new System.Drawing.Point(751, 326);
            this.Init_CPUStart_08.Name = "Init_CPUStart_08";
            this.Init_CPUStart_08.Size = new System.Drawing.Size(86, 30);
            this.Init_CPUStart_08.TabIndex = 69;
            this.Init_CPUStart_08.Text = "启动cpu 08";
            this.Init_CPUStart_08.UseVisualStyleBackColor = true;
            this.Init_CPUStart_08.Click += new System.EventHandler(this.Init_CPUStart_08_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(851, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 44);
            this.button3.TabIndex = 68;
            this.button3.Text = "Connect Reader";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ConnectReader_Click);
            // 
            // text_01_P1
            // 
            this.text_01_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_01_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_01_P1.Location = new System.Drawing.Point(843, 74);
            this.text_01_P1.MaxLength = 2;
            this.text_01_P1.Name = "text_01_P1";
            this.text_01_P1.Size = new System.Drawing.Size(25, 23);
            this.text_01_P1.TabIndex = 67;
            this.text_01_P1.Text = "00";
            this.Hints_toolTip.SetToolTip(this.text_01_P1, "FIDI参数设置");
            this.text_01_P1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_01_P1_KeyPress);
            // 
            // text_04_len
            // 
            this.text_04_len.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_04_len.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_04_len.Location = new System.Drawing.Point(927, 148);
            this.text_04_len.MaxLength = 2;
            this.text_04_len.Name = "text_04_len";
            this.text_04_len.Size = new System.Drawing.Size(25, 23);
            this.text_04_len.TabIndex = 66;
            this.text_04_len.Text = "10";
            // 
            // text_00_len
            // 
            this.text_00_len.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_00_len.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_00_len.Location = new System.Drawing.Point(927, 184);
            this.text_00_len.MaxLength = 2;
            this.text_00_len.Name = "text_00_len";
            this.text_00_len.Size = new System.Drawing.Size(25, 23);
            this.text_00_len.TabIndex = 66;
            this.text_00_len.Text = "10";
            // 
            // text_02_P2
            // 
            this.text_02_P2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_02_P2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_02_P2.Location = new System.Drawing.Point(885, 109);
            this.text_02_P2.MaxLength = 2;
            this.text_02_P2.Name = "text_02_P2";
            this.text_02_P2.Size = new System.Drawing.Size(25, 23);
            this.text_02_P2.TabIndex = 65;
            this.text_02_P2.Text = "00";
            this.Hints_toolTip.SetToolTip(this.text_02_P2, "电压拉偏\r\n读模式配置");
            this.text_02_P2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_02_P2_KeyPress);
            // 
            // text_04_P2
            // 
            this.text_04_P2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_04_P2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_04_P2.Location = new System.Drawing.Point(886, 148);
            this.text_04_P2.MaxLength = 2;
            this.text_04_P2.Name = "text_04_P2";
            this.text_04_P2.Size = new System.Drawing.Size(25, 23);
            this.text_04_P2.TabIndex = 65;
            this.text_04_P2.Text = "00";
            this.text_04_P2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_04_P2_KeyPress);
            // 
            // text_00_P2
            // 
            this.text_00_P2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_00_P2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_00_P2.Location = new System.Drawing.Point(886, 184);
            this.text_00_P2.MaxLength = 2;
            this.text_00_P2.Name = "text_00_P2";
            this.text_00_P2.Size = new System.Drawing.Size(25, 23);
            this.text_00_P2.TabIndex = 65;
            this.text_00_P2.Text = "00";
            this.text_00_P2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_00_P2_KeyPress);
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(927, 53);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(23, 12);
            this.label50.TabIndex = 64;
            this.label50.Text = "LEN";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(893, 55);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(17, 12);
            this.label49.TabIndex = 63;
            this.label49.Text = "P2";
            // 
            // text_02_P1
            // 
            this.text_02_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_02_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_02_P1.Location = new System.Drawing.Point(843, 109);
            this.text_02_P1.MaxLength = 2;
            this.text_02_P1.Name = "text_02_P1";
            this.text_02_P1.Size = new System.Drawing.Size(25, 23);
            this.text_02_P1.TabIndex = 62;
            this.text_02_P1.Text = "00";
            this.Hints_toolTip.SetToolTip(this.text_02_P1, "00：数据EE\r\n01：安全区\r\n02：程序区\r\n03：lib区\r\n04：8Kram");
            this.text_02_P1.TextChanged += new System.EventHandler(this.text_02_P1_TextChanged);
            this.text_02_P1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_02_P1_KeyPress);
            // 
            // text_04_P1
            // 
            this.text_04_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_04_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_04_P1.Location = new System.Drawing.Point(844, 148);
            this.text_04_P1.MaxLength = 2;
            this.text_04_P1.Name = "text_04_P1";
            this.text_04_P1.Size = new System.Drawing.Size(25, 23);
            this.text_04_P1.TabIndex = 62;
            this.text_04_P1.Text = "00";
            this.text_04_P1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_04_P1_KeyPress);
            // 
            // text_00_P1
            // 
            this.text_00_P1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.text_00_P1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.text_00_P1.Location = new System.Drawing.Point(844, 184);
            this.text_00_P1.MaxLength = 2;
            this.text_00_P1.Name = "text_00_P1";
            this.text_00_P1.Size = new System.Drawing.Size(25, 23);
            this.text_00_P1.TabIndex = 62;
            this.text_00_P1.Text = "00";
            this.text_00_P1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text_00_P1_KeyPress);
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(849, 53);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(17, 12);
            this.label48.TabIndex = 61;
            this.label48.Text = "P1";
            // 
            // Init_Rdee_04
            // 
            this.Init_Rdee_04.Location = new System.Drawing.Point(751, 144);
            this.Init_Rdee_04.Name = "Init_Rdee_04";
            this.Init_Rdee_04.Size = new System.Drawing.Size(86, 30);
            this.Init_Rdee_04.TabIndex = 60;
            this.Init_Rdee_04.Text = "RDEE 04";
            this.Init_Rdee_04.UseVisualStyleBackColor = true;
            this.Init_Rdee_04.Click += new System.EventHandler(this.Init_Rdee_04_Click);
            // 
            // Init_EEopt_02
            // 
            this.Init_EEopt_02.Location = new System.Drawing.Point(751, 105);
            this.Init_EEopt_02.Name = "Init_EEopt_02";
            this.Init_EEopt_02.Size = new System.Drawing.Size(86, 30);
            this.Init_EEopt_02.TabIndex = 59;
            this.Init_EEopt_02.Text = "EEopt 02";
            this.Init_EEopt_02.UseVisualStyleBackColor = true;
            this.Init_EEopt_02.Click += new System.EventHandler(this.Init_EEopt_02_Click);
            // 
            // Init_01
            // 
            this.Init_01.Location = new System.Drawing.Point(751, 70);
            this.Init_01.Name = "Init_01";
            this.Init_01.Size = new System.Drawing.Size(86, 30);
            this.Init_01.TabIndex = 58;
            this.Init_01.Text = "初始化 01";
            this.Init_01.UseVisualStyleBackColor = true;
            this.Init_01.Click += new System.EventHandler(this.Init_01_Click);
            // 
            // Init_Wree_00
            // 
            this.Init_Wree_00.Location = new System.Drawing.Point(751, 180);
            this.Init_Wree_00.Name = "Init_Wree_00";
            this.Init_Wree_00.Size = new System.Drawing.Size(86, 30);
            this.Init_Wree_00.TabIndex = 57;
            this.Init_Wree_00.Text = "WREE 00";
            this.Init_Wree_00.UseVisualStyleBackColor = true;
            this.Init_Wree_00.Click += new System.EventHandler(this.Init_Wree_00_Click);
            // 
            // Parity_groupBox
            // 
            this.Parity_groupBox.Controls.Add(this.parity_odd_rdBtn);
            this.Parity_groupBox.Controls.Add(this.parity_even_rdBtn);
            this.Parity_groupBox.Location = new System.Drawing.Point(201, 158);
            this.Parity_groupBox.Name = "Parity_groupBox";
            this.Parity_groupBox.Size = new System.Drawing.Size(80, 52);
            this.Parity_groupBox.TabIndex = 56;
            this.Parity_groupBox.TabStop = false;
            this.Parity_groupBox.Text = "Parity";
            // 
            // parity_odd_rdBtn
            // 
            this.parity_odd_rdBtn.AutoSize = true;
            this.parity_odd_rdBtn.Location = new System.Drawing.Point(6, 29);
            this.parity_odd_rdBtn.Name = "parity_odd_rdBtn";
            this.parity_odd_rdBtn.Size = new System.Drawing.Size(41, 16);
            this.parity_odd_rdBtn.TabIndex = 1;
            this.parity_odd_rdBtn.Text = "odd";
            this.parity_odd_rdBtn.UseVisualStyleBackColor = true;
            this.parity_odd_rdBtn.CheckedChanged += new System.EventHandler(this.parity_odd_rdBtn_CheckedChanged);
            // 
            // parity_even_rdBtn
            // 
            this.parity_even_rdBtn.AutoSize = true;
            this.parity_even_rdBtn.Checked = true;
            this.parity_even_rdBtn.Location = new System.Drawing.Point(6, 13);
            this.parity_even_rdBtn.Name = "parity_even_rdBtn";
            this.parity_even_rdBtn.Size = new System.Drawing.Size(47, 16);
            this.parity_even_rdBtn.TabIndex = 0;
            this.parity_even_rdBtn.TabStop = true;
            this.parity_even_rdBtn.Text = "even";
            this.parity_even_rdBtn.UseVisualStyleBackColor = true;
            this.parity_even_rdBtn.CheckedChanged += new System.EventHandler(this.parity_even_rdBtn_CheckedChanged);
            // 
            // TransceiveDataCT_listBox
            // 
            this.TransceiveDataCT_listBox.FormattingEnabled = true;
            this.TransceiveDataCT_listBox.ItemHeight = 12;
            this.TransceiveDataCT_listBox.Items.AddRange(new object[] {
            "0084000008",
            "0051000004",
            "00F48001",
            "0085000006",
            "000102030405060708090A0B0C0D0E0F",
            "00000000000000000000000000000000",
            "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "55555555555555555555555555555555",
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"});
            this.TransceiveDataCT_listBox.Location = new System.Drawing.Point(303, 12);
            this.TransceiveDataCT_listBox.Name = "TransceiveDataCT_listBox";
            this.TransceiveDataCT_listBox.ScrollAlwaysVisible = true;
            this.TransceiveDataCT_listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.TransceiveDataCT_listBox.Size = new System.Drawing.Size(255, 388);
            this.TransceiveDataCT_listBox.TabIndex = 54;
            this.TransceiveDataCT_listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TransceiveDataCT_listBox_MouseClick);
            this.TransceiveDataCT_listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TransceiveDataCT_listBox_KeyDown);
            // 
            // AddCMDtoListBox_CT_btn
            // 
            this.AddCMDtoListBox_CT_btn.Location = new System.Drawing.Point(562, 352);
            this.AddCMDtoListBox_CT_btn.Name = "AddCMDtoListBox_CT_btn";
            this.AddCMDtoListBox_CT_btn.Size = new System.Drawing.Size(89, 42);
            this.AddCMDtoListBox_CT_btn.TabIndex = 55;
            this.AddCMDtoListBox_CT_btn.Text = "Add CMD List";
            this.AddCMDtoListBox_CT_btn.UseVisualStyleBackColor = true;
            this.AddCMDtoListBox_CT_btn.Click += new System.EventHandler(this.AddCMDtoListBox_CT_btn_Click);
            // 
            // textBox_APDU_P3_CT
            // 
            this.textBox_APDU_P3_CT.Enabled = false;
            this.textBox_APDU_P3_CT.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_P3_CT.Location = new System.Drawing.Point(134, 377);
            this.textBox_APDU_P3_CT.MaxLength = 2;
            this.textBox_APDU_P3_CT.Name = "textBox_APDU_P3_CT";
            this.textBox_APDU_P3_CT.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_P3_CT.TabIndex = 53;
            this.textBox_APDU_P3_CT.Text = "08";
            this.textBox_APDU_P3_CT.TextChanged += new System.EventHandler(this.textBox_APDU_P3_CT_TextChanged);
            this.textBox_APDU_P3_CT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_P3_CT_KeyPress);
            this.textBox_APDU_P3_CT.Leave += new System.EventHandler(this.textBox_APDU_P3_CT_Leave);
            // 
            // textBox_APDU_P2_CT
            // 
            this.textBox_APDU_P2_CT.Enabled = false;
            this.textBox_APDU_P2_CT.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_P2_CT.Location = new System.Drawing.Point(103, 377);
            this.textBox_APDU_P2_CT.MaxLength = 2;
            this.textBox_APDU_P2_CT.Name = "textBox_APDU_P2_CT";
            this.textBox_APDU_P2_CT.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_P2_CT.TabIndex = 52;
            this.textBox_APDU_P2_CT.Text = "00";
            this.textBox_APDU_P2_CT.TextChanged += new System.EventHandler(this.textBox_APDU_P2_CT_TextChanged);
            this.textBox_APDU_P2_CT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_P2_CT_KeyPress);
            this.textBox_APDU_P2_CT.Leave += new System.EventHandler(this.textBox_APDU_P2_CT_Leave);
            // 
            // textBox_APDU_P1_CT
            // 
            this.textBox_APDU_P1_CT.Enabled = false;
            this.textBox_APDU_P1_CT.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_P1_CT.Location = new System.Drawing.Point(72, 377);
            this.textBox_APDU_P1_CT.MaxLength = 2;
            this.textBox_APDU_P1_CT.Name = "textBox_APDU_P1_CT";
            this.textBox_APDU_P1_CT.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_P1_CT.TabIndex = 51;
            this.textBox_APDU_P1_CT.Text = "00";
            this.textBox_APDU_P1_CT.TextChanged += new System.EventHandler(this.textBox_APDU_P1_CT_TextChanged);
            this.textBox_APDU_P1_CT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_P1_CT_KeyPress);
            this.textBox_APDU_P1_CT.Leave += new System.EventHandler(this.textBox_APDU_P1_CT_Leave);
            // 
            // textBox_APDU_INS_CT
            // 
            this.textBox_APDU_INS_CT.Enabled = false;
            this.textBox_APDU_INS_CT.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_INS_CT.Location = new System.Drawing.Point(41, 377);
            this.textBox_APDU_INS_CT.MaxLength = 2;
            this.textBox_APDU_INS_CT.Name = "textBox_APDU_INS_CT";
            this.textBox_APDU_INS_CT.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_INS_CT.TabIndex = 50;
            this.textBox_APDU_INS_CT.Text = "51";
            this.textBox_APDU_INS_CT.TextChanged += new System.EventHandler(this.textBox_APDU_INS_CT_TextChanged);
            this.textBox_APDU_INS_CT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_INS_CT_KeyPress);
            this.textBox_APDU_INS_CT.Leave += new System.EventHandler(this.textBox_APDU_INS_CT_Leave);
            // 
            // textBox_APDU_CLA_CT
            // 
            this.textBox_APDU_CLA_CT.Enabled = false;
            this.textBox_APDU_CLA_CT.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_APDU_CLA_CT.Location = new System.Drawing.Point(10, 377);
            this.textBox_APDU_CLA_CT.MaxLength = 2;
            this.textBox_APDU_CLA_CT.Name = "textBox_APDU_CLA_CT";
            this.textBox_APDU_CLA_CT.Size = new System.Drawing.Size(25, 23);
            this.textBox_APDU_CLA_CT.TabIndex = 49;
            this.textBox_APDU_CLA_CT.Text = "00";
            this.textBox_APDU_CLA_CT.TextChanged += new System.EventHandler(this.textBox_APDU_CLA_CT_TextChanged);
            this.textBox_APDU_CLA_CT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_APDU_CLA_CT_KeyPress);
            this.textBox_APDU_CLA_CT.Leave += new System.EventHandler(this.textBox_APDU_CLA_CT_Leave);
            // 
            // checkBox_APDUmodeCT
            // 
            this.checkBox_APDUmodeCT.AutoSize = true;
            this.checkBox_APDUmodeCT.Location = new System.Drawing.Point(19, 294);
            this.checkBox_APDUmodeCT.Name = "checkBox_APDUmodeCT";
            this.checkBox_APDUmodeCT.Size = new System.Drawing.Size(78, 16);
            this.checkBox_APDUmodeCT.TabIndex = 48;
            this.checkBox_APDUmodeCT.Text = "APDU mode";
            this.checkBox_APDUmodeCT.UseVisualStyleBackColor = true;
            this.checkBox_APDUmodeCT.CheckedChanged += new System.EventHandler(this.checkBox_APDUmodeCT_CheckedChanged);
            // 
            // label_APDU_CT
            // 
            this.label_APDU_CT.AutoSize = true;
            this.label_APDU_CT.Enabled = false;
            this.label_APDU_CT.Location = new System.Drawing.Point(8, 364);
            this.label_APDU_CT.Name = "label_APDU_CT";
            this.label_APDU_CT.Size = new System.Drawing.Size(149, 12);
            this.label_APDU_CT.TabIndex = 47;
            this.label_APDU_CT.Text = "CLA   INS   P1   P2   P3";
            // 
            // TR_CT_TimeOut
            // 
            this.TR_CT_TimeOut.Controls.Add(this.CT_TimeOut_NoTimeout);
            this.TR_CT_TimeOut.Controls.Add(this.CT_TimeOut_100ETU);
            this.TR_CT_TimeOut.Controls.Add(this.CT_TimeOut_std);
            this.TR_CT_TimeOut.Location = new System.Drawing.Point(170, 311);
            this.TR_CT_TimeOut.Name = "TR_CT_TimeOut";
            this.TR_CT_TimeOut.Size = new System.Drawing.Size(121, 89);
            this.TR_CT_TimeOut.TabIndex = 40;
            this.TR_CT_TimeOut.TabStop = false;
            this.TR_CT_TimeOut.Text = "直接发送超时选项";
            // 
            // CT_TimeOut_NoTimeout
            // 
            this.CT_TimeOut_NoTimeout.AutoSize = true;
            this.CT_TimeOut_NoTimeout.Location = new System.Drawing.Point(6, 56);
            this.CT_TimeOut_NoTimeout.Name = "CT_TimeOut_NoTimeout";
            this.CT_TimeOut_NoTimeout.Size = new System.Drawing.Size(83, 16);
            this.CT_TimeOut_NoTimeout.TabIndex = 2;
            this.CT_TimeOut_NoTimeout.Text = "无超时时间";
            this.CT_TimeOut_NoTimeout.UseVisualStyleBackColor = true;
            // 
            // CT_TimeOut_100ETU
            // 
            this.CT_TimeOut_100ETU.AutoSize = true;
            this.CT_TimeOut_100ETU.Checked = true;
            this.CT_TimeOut_100ETU.Location = new System.Drawing.Point(6, 38);
            this.CT_TimeOut_100ETU.Name = "CT_TimeOut_100ETU";
            this.CT_TimeOut_100ETU.Size = new System.Drawing.Size(83, 16);
            this.CT_TimeOut_100ETU.TabIndex = 1;
            this.CT_TimeOut_100ETU.TabStop = true;
            this.CT_TimeOut_100ETU.Text = "100ETU超时";
            this.CT_TimeOut_100ETU.UseVisualStyleBackColor = true;
            // 
            // CT_TimeOut_std
            // 
            this.CT_TimeOut_std.AutoSize = true;
            this.CT_TimeOut_std.Location = new System.Drawing.Point(6, 20);
            this.CT_TimeOut_std.Name = "CT_TimeOut_std";
            this.CT_TimeOut_std.Size = new System.Drawing.Size(95, 16);
            this.CT_TimeOut_std.TabIndex = 0;
            this.CT_TimeOut_std.Text = "标准超时时间";
            this.CT_TimeOut_std.UseVisualStyleBackColor = true;
            // 
            // Clock_Stop_Btn
            // 
            this.Clock_Stop_Btn.Location = new System.Drawing.Point(18, 248);
            this.Clock_Stop_Btn.Name = "Clock_Stop_Btn";
            this.Clock_Stop_Btn.Size = new System.Drawing.Size(77, 23);
            this.Clock_Stop_Btn.TabIndex = 39;
            this.Clock_Stop_Btn.Text = "ClockStop";
            this.Clock_Stop_Btn.UseVisualStyleBackColor = true;
            this.Clock_Stop_Btn.Click += new System.EventHandler(this.Clock_Stop_Btn_Click);
            // 
            // CST_select
            // 
            this.CST_select.Controls.Add(this.CST_Low);
            this.CST_select.Controls.Add(this.CST_High);
            this.CST_select.Location = new System.Drawing.Point(202, 96);
            this.CST_select.Name = "CST_select";
            this.CST_select.Size = new System.Drawing.Size(79, 53);
            this.CST_select.TabIndex = 38;
            this.CST_select.TabStop = false;
            this.CST_select.Text = "停时钟状态";
            // 
            // CST_Low
            // 
            this.CST_Low.AutoSize = true;
            this.CST_Low.Checked = true;
            this.CST_Low.Location = new System.Drawing.Point(6, 30);
            this.CST_Low.Name = "CST_Low";
            this.CST_Low.Size = new System.Drawing.Size(41, 16);
            this.CST_Low.TabIndex = 1;
            this.CST_Low.TabStop = true;
            this.CST_Low.Text = "Low";
            this.CST_Low.UseVisualStyleBackColor = true;
            // 
            // CST_High
            // 
            this.CST_High.AutoSize = true;
            this.CST_High.Location = new System.Drawing.Point(7, 14);
            this.CST_High.Name = "CST_High";
            this.CST_High.Size = new System.Drawing.Size(47, 16);
            this.CST_High.TabIndex = 0;
            this.CST_High.Text = "High";
            this.CST_High.UseVisualStyleBackColor = true;
            // 
            // TransceiveCT
            // 
            this.TransceiveCT.Location = new System.Drawing.Point(18, 311);
            this.TransceiveCT.Name = "TransceiveCT";
            this.TransceiveCT.Size = new System.Drawing.Size(124, 50);
            this.TransceiveCT.TabIndex = 37;
            this.TransceiveCT.Text = "Direct Send";
            this.TransceiveCT.UseVisualStyleBackColor = true;
            this.TransceiveCT.Click += new System.EventHandler(this.TransceiveCT_Click);
            // 
            // VoltageSel
            // 
            this.VoltageSel.Controls.Add(this.Volt18);
            this.VoltageSel.Controls.Add(this.Volt30);
            this.VoltageSel.Controls.Add(this.Volt50);
            this.VoltageSel.Location = new System.Drawing.Point(18, 6);
            this.VoltageSel.Name = "VoltageSel";
            this.VoltageSel.Size = new System.Drawing.Size(77, 90);
            this.VoltageSel.TabIndex = 36;
            this.VoltageSel.TabStop = false;
            this.VoltageSel.Text = "CT电压";
            // 
            // Volt18
            // 
            this.Volt18.AutoSize = true;
            this.Volt18.Location = new System.Drawing.Point(6, 64);
            this.Volt18.Name = "Volt18";
            this.Volt18.Size = new System.Drawing.Size(47, 16);
            this.Volt18.TabIndex = 37;
            this.Volt18.Text = "1.8V";
            this.Volt18.UseVisualStyleBackColor = true;
            this.Volt18.Click += new System.EventHandler(this.VoltageSel_click);
            // 
            // Volt30
            // 
            this.Volt30.AutoSize = true;
            this.Volt30.Checked = true;
            this.Volt30.Location = new System.Drawing.Point(6, 42);
            this.Volt30.Name = "Volt30";
            this.Volt30.Size = new System.Drawing.Size(47, 16);
            this.Volt30.TabIndex = 36;
            this.Volt30.TabStop = true;
            this.Volt30.Text = "3.0V";
            this.Volt30.UseVisualStyleBackColor = true;
            this.Volt30.Click += new System.EventHandler(this.VoltageSel_click);
            // 
            // Volt50
            // 
            this.Volt50.AutoSize = true;
            this.Volt50.Location = new System.Drawing.Point(6, 20);
            this.Volt50.Name = "Volt50";
            this.Volt50.Size = new System.Drawing.Size(47, 16);
            this.Volt50.TabIndex = 35;
            this.Volt50.Text = "5.0V";
            this.Volt50.UseVisualStyleBackColor = true;
            this.Volt50.Click += new System.EventHandler(this.VoltageSel_click);
            // 
            // Warm_Reset
            // 
            this.Warm_Reset.Location = new System.Drawing.Point(18, 218);
            this.Warm_Reset.Name = "Warm_Reset";
            this.Warm_Reset.Size = new System.Drawing.Size(77, 23);
            this.Warm_Reset.TabIndex = 34;
            this.Warm_Reset.Text = "Warm Reset";
            this.Warm_Reset.UseVisualStyleBackColor = true;
            this.Warm_Reset.Click += new System.EventHandler(this.Warm_Reset_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(114, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 14);
            this.label6.TabIndex = 32;
            this.label6.Text = "Data 0x";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(114, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 33;
            this.label7.Text = "Addr 0x";
            // 
            // RegData_TDA_textBox
            // 
            this.RegData_TDA_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.RegData_TDA_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RegData_TDA_textBox.Location = new System.Drawing.Point(170, 52);
            this.RegData_TDA_textBox.MaxLength = 2;
            this.RegData_TDA_textBox.Name = "RegData_TDA_textBox";
            this.RegData_TDA_textBox.Size = new System.Drawing.Size(29, 23);
            this.RegData_TDA_textBox.TabIndex = 31;
            this.RegData_TDA_textBox.Text = "00";
            // 
            // RegAddr_TDA_textBox
            // 
            this.RegAddr_TDA_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.RegAddr_TDA_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RegAddr_TDA_textBox.Location = new System.Drawing.Point(170, 23);
            this.RegAddr_TDA_textBox.MaxLength = 2;
            this.RegAddr_TDA_textBox.Name = "RegAddr_TDA_textBox";
            this.RegAddr_TDA_textBox.Size = new System.Drawing.Size(29, 23);
            this.RegAddr_TDA_textBox.TabIndex = 30;
            this.RegAddr_TDA_textBox.Text = "00";
            // 
            // WriteTDAReg
            // 
            this.WriteTDAReg.Location = new System.Drawing.Point(206, 52);
            this.WriteTDAReg.Name = "WriteTDAReg";
            this.WriteTDAReg.Size = new System.Drawing.Size(75, 23);
            this.WriteTDAReg.TabIndex = 29;
            this.WriteTDAReg.Text = "WriteReg";
            this.WriteTDAReg.UseVisualStyleBackColor = true;
            this.WriteTDAReg.Click += new System.EventHandler(this.WriteTDAReg_Click);
            // 
            // ReadTDAReg
            // 
            this.ReadTDAReg.Location = new System.Drawing.Point(206, 22);
            this.ReadTDAReg.Name = "ReadTDAReg";
            this.ReadTDAReg.Size = new System.Drawing.Size(75, 23);
            this.ReadTDAReg.TabIndex = 28;
            this.ReadTDAReg.Text = "ReadReg";
            this.ReadTDAReg.UseVisualStyleBackColor = true;
            this.ReadTDAReg.Click += new System.EventHandler(this.ReadTDAReg_Click);
            // 
            // Cold_Reset
            // 
            this.Cold_Reset.Location = new System.Drawing.Point(18, 131);
            this.Cold_Reset.Name = "Cold_Reset";
            this.Cold_Reset.Size = new System.Drawing.Size(77, 23);
            this.Cold_Reset.TabIndex = 1;
            this.Cold_Reset.Text = "Cold Reset";
            this.Cold_Reset.UseVisualStyleBackColor = true;
            this.Cold_Reset.Click += new System.EventHandler(this.Cold_Reset_Click);
            // 
            // Init_TDA8007
            // 
            this.Init_TDA8007.Location = new System.Drawing.Point(18, 102);
            this.Init_TDA8007.Name = "Init_TDA8007";
            this.Init_TDA8007.Size = new System.Drawing.Size(77, 23);
            this.Init_TDA8007.TabIndex = 0;
            this.Init_TDA8007.Text = "Init TDA";
            this.Init_TDA8007.UseVisualStyleBackColor = true;
            this.Init_TDA8007.Click += new System.EventHandler(this.Init_TDA8007_Click);
            // 
            // tabPage_SPI_I2C
            // 
            this.tabPage_SPI_I2C.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_SPI_I2C.Controls.Add(this.groupBox14);
            this.tabPage_SPI_I2C.Controls.Add(this.TransceiveDataSPI_I2C_listBox);
            this.tabPage_SPI_I2C.Controls.Add(this.AddtoListBox_SPI_I2C_btn);
            this.tabPage_SPI_I2C.Controls.Add(this.I2C_groupBox);
            this.tabPage_SPI_I2C.Controls.Add(this.SPI_groupBox);
            this.tabPage_SPI_I2C.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SPI_I2C.Name = "tabPage_SPI_I2C";
            this.tabPage_SPI_I2C.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_SPI_I2C.Size = new System.Drawing.Size(956, 406);
            this.tabPage_SPI_I2C.TabIndex = 6;
            this.tabPage_SPI_I2C.Text = "其他接口";
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.comboBox1);
            this.groupBox14.Controls.Add(this.button9);
            this.groupBox14.Controls.Add(this.button7);
            this.groupBox14.Location = new System.Drawing.Point(24, 281);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(162, 110);
            this.groupBox14.TabIndex = 93;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "UART";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("宋体", 10F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "115200",
            "57600",
            "19200",
            "9600",
            "4800"});
            this.comboBox1.Location = new System.Drawing.Point(70, 30);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(73, 21);
            this.comboBox1.TabIndex = 92;
            this.comboBox1.Text = "115200";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(16, 26);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(49, 29);
            this.button9.TabIndex = 66;
            this.button9.Text = "打开";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(16, 61);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(127, 34);
            this.button7.TabIndex = 62;
            this.button7.Text = "发送";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // TransceiveDataSPI_I2C_listBox
            // 
            this.TransceiveDataSPI_I2C_listBox.FormattingEnabled = true;
            this.TransceiveDataSPI_I2C_listBox.ItemHeight = 12;
            this.TransceiveDataSPI_I2C_listBox.Items.AddRange(new object[] {
            "000102030405060708090A0B0C0D0E0F",
            "00000000000000000000000000000000",
            "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "55555555555555555555555555555555",
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"});
            this.TransceiveDataSPI_I2C_listBox.Location = new System.Drawing.Point(301, 243);
            this.TransceiveDataSPI_I2C_listBox.Name = "TransceiveDataSPI_I2C_listBox";
            this.TransceiveDataSPI_I2C_listBox.ScrollAlwaysVisible = true;
            this.TransceiveDataSPI_I2C_listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.TransceiveDataSPI_I2C_listBox.Size = new System.Drawing.Size(277, 148);
            this.TransceiveDataSPI_I2C_listBox.TabIndex = 60;
            this.TransceiveDataSPI_I2C_listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TransceiveDataSPI_I2C_listBox_MouseClick);
            // 
            // AddtoListBox_SPI_I2C_btn
            // 
            this.AddtoListBox_SPI_I2C_btn.Location = new System.Drawing.Point(584, 365);
            this.AddtoListBox_SPI_I2C_btn.Name = "AddtoListBox_SPI_I2C_btn";
            this.AddtoListBox_SPI_I2C_btn.Size = new System.Drawing.Size(79, 26);
            this.AddtoListBox_SPI_I2C_btn.TabIndex = 61;
            this.AddtoListBox_SPI_I2C_btn.Text = "Add to List";
            this.AddtoListBox_SPI_I2C_btn.UseVisualStyleBackColor = true;
            this.AddtoListBox_SPI_I2C_btn.Click += new System.EventHandler(this.AddtoListBox_SPI_I2C_btn_Click);
            // 
            // I2C_groupBox
            // 
            this.I2C_groupBox.Controls.Add(this.label9);
            this.I2C_groupBox.Controls.Add(this.I2C_LenToReceive_textBox);
            this.I2C_groupBox.Controls.Add(this.I2C_Send_btn);
            this.I2C_groupBox.Location = new System.Drawing.Point(663, 6);
            this.I2C_groupBox.Name = "I2C_groupBox";
            this.I2C_groupBox.Size = new System.Drawing.Size(244, 236);
            this.I2C_groupBox.TabIndex = 59;
            this.I2C_groupBox.TabStop = false;
            this.I2C_groupBox.Text = "I2C";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(27, 208);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 16);
            this.label9.TabIndex = 62;
            this.label9.Text = "ReceiveLen:";
            // 
            // I2C_LenToReceive_textBox
            // 
            this.I2C_LenToReceive_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.I2C_LenToReceive_textBox.Location = new System.Drawing.Point(129, 207);
            this.I2C_LenToReceive_textBox.MaxLength = 2;
            this.I2C_LenToReceive_textBox.Name = "I2C_LenToReceive_textBox";
            this.I2C_LenToReceive_textBox.Size = new System.Drawing.Size(23, 23);
            this.I2C_LenToReceive_textBox.TabIndex = 61;
            this.I2C_LenToReceive_textBox.Text = "00";
            this.I2C_LenToReceive_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.I2C_LenToReceive_textBox_KeyPress);
            // 
            // I2C_Send_btn
            // 
            this.I2C_Send_btn.Location = new System.Drawing.Point(30, 154);
            this.I2C_Send_btn.Name = "I2C_Send_btn";
            this.I2C_Send_btn.Size = new System.Drawing.Size(122, 42);
            this.I2C_Send_btn.TabIndex = 56;
            this.I2C_Send_btn.Text = "I2C_Send";
            this.I2C_Send_btn.UseVisualStyleBackColor = true;
            this.I2C_Send_btn.Click += new System.EventHandler(this.I2C_Send_btn_Click);
            // 
            // SPI_groupBox
            // 
            this.SPI_groupBox.Controls.Add(this.SPI_Send_btn);
            this.SPI_groupBox.Controls.Add(this.label8);
            this.SPI_groupBox.Controls.Add(this.SPI_LenToReceive_textBox);
            this.SPI_groupBox.Location = new System.Drawing.Point(8, 6);
            this.SPI_groupBox.Name = "SPI_groupBox";
            this.SPI_groupBox.Size = new System.Drawing.Size(271, 250);
            this.SPI_groupBox.TabIndex = 58;
            this.SPI_groupBox.TabStop = false;
            this.SPI_groupBox.Text = "SPI";
            // 
            // SPI_Send_btn
            // 
            this.SPI_Send_btn.Location = new System.Drawing.Point(16, 175);
            this.SPI_Send_btn.Name = "SPI_Send_btn";
            this.SPI_Send_btn.Size = new System.Drawing.Size(125, 40);
            this.SPI_Send_btn.TabIndex = 58;
            this.SPI_Send_btn.Text = "SPI_Send";
            this.SPI_Send_btn.UseVisualStyleBackColor = true;
            this.SPI_Send_btn.Click += new System.EventHandler(this.SPI_Send_btn_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(16, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 16);
            this.label8.TabIndex = 60;
            this.label8.Text = "ReceiveLen:";
            // 
            // SPI_LenToReceive_textBox
            // 
            this.SPI_LenToReceive_textBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SPI_LenToReceive_textBox.Location = new System.Drawing.Point(118, 219);
            this.SPI_LenToReceive_textBox.MaxLength = 2;
            this.SPI_LenToReceive_textBox.Name = "SPI_LenToReceive_textBox";
            this.SPI_LenToReceive_textBox.Size = new System.Drawing.Size(23, 23);
            this.SPI_LenToReceive_textBox.TabIndex = 59;
            this.SPI_LenToReceive_textBox.Text = "00";
            this.SPI_LenToReceive_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SPI_LenToReceive_textBox_KeyPress);
            // 
            // tabPage_Script1
            // 
            this.tabPage_Script1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_Script1.Controls.Add(this.button_script_reload);
            this.tabPage_Script1.Controls.Add(this.StopRunScr_simple);
            this.tabPage_Script1.Controls.Add(this.RunScr_simple);
            this.tabPage_Script1.Controls.Add(this.RunScrStep_simple);
            this.tabPage_Script1.Controls.Add(this.RstScrScope);
            this.tabPage_Script1.Controls.Add(this.Open_ScriptFile);
            this.tabPage_Script1.Controls.Add(this.dataGridView_Script);
            this.tabPage_Script1.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Script1.Name = "tabPage_Script1";
            this.tabPage_Script1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Script1.Size = new System.Drawing.Size(956, 406);
            this.tabPage_Script1.TabIndex = 4;
            this.tabPage_Script1.Text = "简单脚本";
            // 
            // button_script_reload
            // 
            this.button_script_reload.Location = new System.Drawing.Point(6, 97);
            this.button_script_reload.Name = "button_script_reload";
            this.button_script_reload.Size = new System.Drawing.Size(75, 25);
            this.button_script_reload.TabIndex = 15;
            this.button_script_reload.Text = "刷新脚本";
            this.button_script_reload.UseVisualStyleBackColor = true;
            this.button_script_reload.Click += new System.EventHandler(this.button_script_reload_Click);
            // 
            // StopRunScr_simple
            // 
            this.StopRunScr_simple.Location = new System.Drawing.Point(6, 280);
            this.StopRunScr_simple.Name = "StopRunScr_simple";
            this.StopRunScr_simple.Size = new System.Drawing.Size(75, 40);
            this.StopRunScr_simple.TabIndex = 14;
            this.StopRunScr_simple.Text = "停止运行";
            this.StopRunScr_simple.UseVisualStyleBackColor = true;
            this.StopRunScr_simple.Click += new System.EventHandler(this.StopRunScr_simple_Click);
            // 
            // RunScr_simple
            // 
            this.RunScr_simple.Location = new System.Drawing.Point(6, 224);
            this.RunScr_simple.Name = "RunScr_simple";
            this.RunScr_simple.Size = new System.Drawing.Size(75, 40);
            this.RunScr_simple.TabIndex = 13;
            this.RunScr_simple.Text = "连续运行";
            this.RunScr_simple.UseVisualStyleBackColor = true;
            this.RunScr_simple.Click += new System.EventHandler(this.RunScr_simple_Click);
            // 
            // RunScrStep_simple
            // 
            this.RunScrStep_simple.Location = new System.Drawing.Point(6, 150);
            this.RunScrStep_simple.Name = "RunScrStep_simple";
            this.RunScrStep_simple.Size = new System.Drawing.Size(75, 40);
            this.RunScrStep_simple.TabIndex = 11;
            this.RunScrStep_simple.Text = "单步运行";
            this.RunScrStep_simple.UseVisualStyleBackColor = true;
            this.RunScrStep_simple.Click += new System.EventHandler(this.RunScrStep_simple_Click);
            // 
            // RstScrScope
            // 
            this.RstScrScope.Location = new System.Drawing.Point(6, 57);
            this.RstScrScope.Name = "RstScrScope";
            this.RstScrScope.Size = new System.Drawing.Size(75, 25);
            this.RstScrScope.TabIndex = 10;
            this.RstScrScope.Text = "复位脚本";
            this.RstScrScope.UseVisualStyleBackColor = true;
            this.RstScrScope.Click += new System.EventHandler(this.RstScrScope_Click);
            // 
            // Open_ScriptFile
            // 
            this.Open_ScriptFile.Location = new System.Drawing.Point(6, 21);
            this.Open_ScriptFile.Name = "Open_ScriptFile";
            this.Open_ScriptFile.Size = new System.Drawing.Size(75, 25);
            this.Open_ScriptFile.TabIndex = 9;
            this.Open_ScriptFile.Text = "打开脚本";
            this.Open_ScriptFile.UseVisualStyleBackColor = true;
            this.Open_ScriptFile.Click += new System.EventHandler(this.Open_ScriptFile_Click);
            // 
            // dataGridView_Script
            // 
            this.dataGridView_Script.AllowUserToResizeRows = false;
            this.dataGridView_Script.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Script.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Script.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Script.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.Commands,
            this.CompareData,
            this.ReturnData,
            this.Results});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Script.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Script.Location = new System.Drawing.Point(90, 3);
            this.dataGridView_Script.Name = "dataGridView_Script";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.NullValue = "1";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Script.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_Script.RowHeadersWidth = 47;
            this.dataGridView_Script.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView_Script.RowTemplate.Height = 23;
            this.dataGridView_Script.Size = new System.Drawing.Size(867, 400);
            this.dataGridView_Script.TabIndex = 8;
            this.dataGridView_Script.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Script_RowHeaderMouseClick);
            this.dataGridView_Script.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_Script_RowHeaderMouseDoubleClick);
            // 
            // Index
            // 
            this.Index.HeaderText = "Idx";
            this.Index.Name = "Index";
            this.Index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Index.Width = 30;
            // 
            // Commands
            // 
            this.Commands.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Commands.HeaderText = "Commands";
            this.Commands.Name = "Commands";
            this.Commands.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Commands.Width = 310;
            // 
            // CompareData
            // 
            this.CompareData.HeaderText = "CompareData";
            this.CompareData.Name = "CompareData";
            this.CompareData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CompareData.Width = 200;
            // 
            // ReturnData
            // 
            this.ReturnData.HeaderText = "ReturnData";
            this.ReturnData.Name = "ReturnData";
            this.ReturnData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ReturnData.Width = 200;
            // 
            // Results
            // 
            this.Results.HeaderText = "Results";
            this.Results.Name = "Results";
            this.Results.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Results.Width = 74;
            // 
            // tabPage_Script
            // 
            this.tabPage_Script.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_Script.Controls.Add(this.Script_backgroundWorker_progressBar);
            this.tabPage_Script.Controls.Add(this.Stop_RunScript_btn);
            this.tabPage_Script.Controls.Add(this.Scripts_code);
            this.tabPage_Script.Controls.Add(this.Run_Script_btn);
            this.tabPage_Script.Controls.Add(this.Open_Scr_dir);
            this.tabPage_Script.Controls.Add(this.Scripts_list);
            this.tabPage_Script.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Script.Name = "tabPage_Script";
            this.tabPage_Script.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Script.Size = new System.Drawing.Size(956, 406);
            this.tabPage_Script.TabIndex = 3;
            this.tabPage_Script.Text = "复杂脚本";
            // 
            // Script_backgroundWorker_progressBar
            // 
            this.Script_backgroundWorker_progressBar.Location = new System.Drawing.Point(413, 380);
            this.Script_backgroundWorker_progressBar.Name = "Script_backgroundWorker_progressBar";
            this.Script_backgroundWorker_progressBar.Size = new System.Drawing.Size(161, 18);
            this.Script_backgroundWorker_progressBar.TabIndex = 7;
            // 
            // Stop_RunScript_btn
            // 
            this.Stop_RunScript_btn.Location = new System.Drawing.Point(283, 378);
            this.Stop_RunScript_btn.Name = "Stop_RunScript_btn";
            this.Stop_RunScript_btn.Size = new System.Drawing.Size(90, 23);
            this.Stop_RunScript_btn.TabIndex = 6;
            this.Stop_RunScript_btn.Text = "终止脚本运行";
            this.Stop_RunScript_btn.UseVisualStyleBackColor = true;
            this.Stop_RunScript_btn.Click += new System.EventHandler(this.Stop_RunScript_btn_Click);
            // 
            // Scripts_code
            // 
            this.Scripts_code.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Scripts_code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Scripts_code.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Scripts_code.Location = new System.Drawing.Point(158, 0);
            this.Scripts_code.Name = "Scripts_code";
            this.Scripts_code.Size = new System.Drawing.Size(798, 376);
            this.Scripts_code.TabIndex = 5;
            this.Scripts_code.Text = "";
            this.Scripts_code.WordWrap = false;
            // 
            // Run_Script_btn
            // 
            this.Run_Script_btn.Location = new System.Drawing.Point(185, 378);
            this.Run_Script_btn.Name = "Run_Script_btn";
            this.Run_Script_btn.Size = new System.Drawing.Size(92, 23);
            this.Run_Script_btn.TabIndex = 4;
            this.Run_Script_btn.Text = "运行Py脚本";
            this.Run_Script_btn.UseVisualStyleBackColor = true;
            this.Run_Script_btn.Click += new System.EventHandler(this.Run_Script_btn_Click);
            // 
            // Open_Scr_dir
            // 
            this.Open_Scr_dir.Location = new System.Drawing.Point(7, 377);
            this.Open_Scr_dir.Name = "Open_Scr_dir";
            this.Open_Scr_dir.Size = new System.Drawing.Size(142, 23);
            this.Open_Scr_dir.TabIndex = 2;
            this.Open_Scr_dir.Text = "打开目录";
            this.Open_Scr_dir.UseVisualStyleBackColor = true;
            this.Open_Scr_dir.Click += new System.EventHandler(this.Open_Scr_dir_Click);
            // 
            // Scripts_list
            // 
            this.Scripts_list.FormattingEnabled = true;
            this.Scripts_list.HorizontalScrollbar = true;
            this.Scripts_list.ItemHeight = 12;
            this.Scripts_list.Location = new System.Drawing.Point(6, 0);
            this.Scripts_list.Name = "Scripts_list";
            this.Scripts_list.Size = new System.Drawing.Size(146, 376);
            this.Scripts_list.TabIndex = 0;
            // 
            // lighttest
            // 
            this.lighttest.Controls.Add(this.groupBox5);
            this.lighttest.Controls.Add(this.groupBox2);
            this.lighttest.Controls.Add(this.auto_delay);
            this.lighttest.Controls.Add(this.d_cos);
            this.lighttest.Controls.Add(this.button1);
            this.lighttest.Controls.Add(this.temp_retain);
            this.lighttest.Controls.Add(this.wr_config);
            this.lighttest.Controls.Add(this.groupBox1);
            this.lighttest.Controls.Add(this.temp_fm347);
            this.lighttest.Controls.Add(this.user_lock_button);
            this.lighttest.Controls.Add(this.userfor_passport_button);
            this.lighttest.Controls.Add(this.cfg_and_progfor_passport_button);
            this.lighttest.Controls.Add(this.trimfor_passport_button);
            this.lighttest.Controls.Add(this.groupBox20);
            this.lighttest.Controls.Add(this.button13);
            this.lighttest.Controls.Add(this.FM336_Endurance_button);
            this.lighttest.Controls.Add(this.groupBox8);
            this.lighttest.Controls.Add(this.bkp);
            this.lighttest.Controls.Add(this.auto_freq_test);
            this.lighttest.Controls.Add(this.cfgroup);
            this.lighttest.Controls.Add(this.cfg);
            this.lighttest.Controls.Add(this.sjxm);
            this.lighttest.Controls.Add(this.button11);
            this.lighttest.Controls.Add(this.button10);
            this.lighttest.Controls.Add(this.groupBox19);
            this.lighttest.Controls.Add(this.button8);
            this.lighttest.Controls.Add(this.groupBox18);
            this.lighttest.Controls.Add(this.label74);
            this.lighttest.Controls.Add(this.ADcos);
            this.lighttest.Controls.Add(this.label73);
            this.lighttest.Controls.Add(this.Akey);
            this.lighttest.Controls.Add(this.brst);
            this.lighttest.Controls.Add(this.temp_select);
            this.lighttest.Controls.Add(this.FM294_str);
            this.lighttest.Controls.Add(this.groupBox17);
            this.lighttest.Controls.Add(this.groupBox16);
            this.lighttest.Controls.Add(this.groupBox11);
            this.lighttest.Controls.Add(this.button6);
            this.lighttest.Controls.Add(this.button5);
            this.lighttest.Controls.Add(this.temp_rlt);
            this.lighttest.Controls.Add(this.temp_start);
            this.lighttest.Controls.Add(this.groupBox12);
            this.lighttest.Location = new System.Drawing.Point(4, 22);
            this.lighttest.Name = "lighttest";
            this.lighttest.Padding = new System.Windows.Forms.Padding(3);
            this.lighttest.Size = new System.Drawing.Size(956, 406);
            this.lighttest.TabIndex = 8;
            this.lighttest.Text = "安全筛卡";
            this.lighttest.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.UID_modify);
            this.groupBox5.Controls.Add(this.checkClockTrim);
            this.groupBox5.Controls.Add(this.cmdNBFaka);
            this.groupBox5.Controls.Add(this.NVM_encrypt);
            this.groupBox5.Controls.Add(this.fm347_cos);
            this.groupBox5.Controls.Add(this.NVM_init);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Controls.Add(this.groupBox4);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.groupBox3);
            this.groupBox5.Controls.Add(this.N_uid);
            this.groupBox5.Location = new System.Drawing.Point(586, 153);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(232, 170);
            this.groupBox5.TabIndex = 108;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "FM347送检";
            // 
            // UID_modify
            // 
            this.UID_modify.AutoSize = true;
            this.UID_modify.Location = new System.Drawing.Point(97, 141);
            this.UID_modify.Name = "UID_modify";
            this.UID_modify.Size = new System.Drawing.Size(66, 16);
            this.UID_modify.TabIndex = 113;
            this.UID_modify.Text = "改UID错";
            this.UID_modify.UseVisualStyleBackColor = true;
            // 
            // checkClockTrim
            // 
            this.checkClockTrim.AutoSize = true;
            this.checkClockTrim.Location = new System.Drawing.Point(97, 119);
            this.checkClockTrim.Name = "checkClockTrim";
            this.checkClockTrim.Size = new System.Drawing.Size(72, 16);
            this.checkClockTrim.TabIndex = 112;
            this.checkClockTrim.Text = "时钟调校";
            this.checkClockTrim.UseVisualStyleBackColor = true;
            // 
            // cmdNBFaka
            // 
            this.cmdNBFaka.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmdNBFaka.Location = new System.Drawing.Point(16, 119);
            this.cmdNBFaka.Name = "cmdNBFaka";
            this.cmdNBFaka.Size = new System.Drawing.Size(70, 45);
            this.cmdNBFaka.TabIndex = 111;
            this.cmdNBFaka.Text = "内部发卡初始化";
            this.cmdNBFaka.UseVisualStyleBackColor = true;
            this.cmdNBFaka.Click += new System.EventHandler(this.cmdNBFaka_Click);
            // 
            // NVM_encrypt
            // 
            this.NVM_encrypt.AutoSize = true;
            this.NVM_encrypt.Location = new System.Drawing.Point(139, 59);
            this.NVM_encrypt.Name = "NVM_encrypt";
            this.NVM_encrypt.Size = new System.Drawing.Size(90, 16);
            this.NVM_encrypt.TabIndex = 110;
            this.NVM_encrypt.Text = "关闭NVM加密";
            this.NVM_encrypt.UseVisualStyleBackColor = true;
            // 
            // fm347_cos
            // 
            this.fm347_cos.AutoSize = true;
            this.fm347_cos.Location = new System.Drawing.Point(64, 72);
            this.fm347_cos.Name = "fm347_cos";
            this.fm347_cos.Size = new System.Drawing.Size(66, 16);
            this.fm347_cos.TabIndex = 109;
            this.fm347_cos.Text = "COS下载";
            this.fm347_cos.UseVisualStyleBackColor = true;
            // 
            // NVM_init
            // 
            this.NVM_init.AutoSize = true;
            this.NVM_init.Location = new System.Drawing.Point(64, 58);
            this.NVM_init.Name = "NVM_init";
            this.NVM_init.Size = new System.Drawing.Size(78, 16);
            this.NVM_init.TabIndex = 108;
            this.NVM_init.Text = "NVM初始化";
            this.NVM_init.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(6, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 46);
            this.button2.TabIndex = 103;
            this.button2.Text = "FM347 初始化";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cc_dip);
            this.groupBox4.Controls.Add(this.cc_card);
            this.groupBox4.Location = new System.Drawing.Point(172, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(56, 49);
            this.groupBox4.TabIndex = 105;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "样品";
            // 
            // cc_dip
            // 
            this.cc_dip.AutoSize = true;
            this.cc_dip.Location = new System.Drawing.Point(6, 27);
            this.cc_dip.Name = "cc_dip";
            this.cc_dip.Size = new System.Drawing.Size(41, 16);
            this.cc_dip.TabIndex = 1;
            this.cc_dip.Text = "DIP";
            this.cc_dip.UseVisualStyleBackColor = true;
            // 
            // cc_card
            // 
            this.cc_card.AutoSize = true;
            this.cc_card.Checked = true;
            this.cc_card.Location = new System.Drawing.Point(6, 12);
            this.cc_card.Name = "cc_card";
            this.cc_card.Size = new System.Drawing.Size(47, 16);
            this.cc_card.TabIndex = 0;
            this.cc_card.TabStop = true;
            this.cc_card.Text = "白卡";
            this.cc_card.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 107;
            this.label3.Text = "3位UID编号";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cc_347);
            this.groupBox3.Controls.Add(this.mp300_347);
            this.groupBox3.Location = new System.Drawing.Point(71, 11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(98, 44);
            this.groupBox3.TabIndex = 104;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "送检项目";
            // 
            // cc_347
            // 
            this.cc_347.AutoSize = true;
            this.cc_347.Checked = true;
            this.cc_347.Location = new System.Drawing.Point(6, 27);
            this.cc_347.Name = "cc_347";
            this.cc_347.Size = new System.Drawing.Size(59, 16);
            this.cc_347.TabIndex = 1;
            this.cc_347.TabStop = true;
            this.cc_347.Text = "CC送检";
            this.cc_347.UseVisualStyleBackColor = true;
            // 
            // mp300_347
            // 
            this.mp300_347.AutoSize = true;
            this.mp300_347.Location = new System.Drawing.Point(6, 12);
            this.mp300_347.Name = "mp300_347";
            this.mp300_347.Size = new System.Drawing.Size(71, 16);
            this.mp300_347.TabIndex = 0;
            this.mp300_347.Text = "内部安全";
            this.mp300_347.UseVisualStyleBackColor = true;
            // 
            // N_uid
            // 
            this.N_uid.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.N_uid.ForeColor = System.Drawing.Color.Red;
            this.N_uid.Location = new System.Drawing.Point(19, 62);
            this.N_uid.Name = "N_uid";
            this.N_uid.Size = new System.Drawing.Size(34, 26);
            this.N_uid.TabIndex = 106;
            this.N_uid.Text = "编号";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cos);
            this.groupBox2.Controls.Add(this.init);
            this.groupBox2.Location = new System.Drawing.Point(225, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(55, 46);
            this.groupBox2.TabIndex = 102;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "方式";
            // 
            // cos
            // 
            this.cos.AutoSize = true;
            this.cos.Checked = true;
            this.cos.Location = new System.Drawing.Point(6, 28);
            this.cos.Name = "cos";
            this.cos.Size = new System.Drawing.Size(41, 16);
            this.cos.TabIndex = 1;
            this.cos.TabStop = true;
            this.cos.Text = "COS";
            this.cos.UseVisualStyleBackColor = true;
            // 
            // init
            // 
            this.init.AutoSize = true;
            this.init.Location = new System.Drawing.Point(6, 12);
            this.init.Name = "init";
            this.init.Size = new System.Drawing.Size(47, 16);
            this.init.TabIndex = 0;
            this.init.Text = "init";
            this.init.UseVisualStyleBackColor = true;
            // 
            // auto_delay
            // 
            this.auto_delay.AutoSize = true;
            this.auto_delay.Checked = true;
            this.auto_delay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.auto_delay.Location = new System.Drawing.Point(286, 212);
            this.auto_delay.Name = "auto_delay";
            this.auto_delay.Size = new System.Drawing.Size(72, 16);
            this.auto_delay.TabIndex = 101;
            this.auto_delay.Text = "自动延时";
            this.auto_delay.UseVisualStyleBackColor = true;
            // 
            // d_cos
            // 
            this.d_cos.AutoSize = true;
            this.d_cos.Checked = true;
            this.d_cos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.d_cos.Location = new System.Drawing.Point(286, 196);
            this.d_cos.Name = "d_cos";
            this.d_cos.Size = new System.Drawing.Size(66, 16);
            this.d_cos.TabIndex = 100;
            this.d_cos.Text = "COS下载";
            this.d_cos.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(87, 176);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 43);
            this.button1.TabIndex = 99;
            this.button1.Text = "FM347 cos检验";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // temp_retain
            // 
            this.temp_retain.AutoSize = true;
            this.temp_retain.Location = new System.Drawing.Point(286, 181);
            this.temp_retain.Name = "temp_retain";
            this.temp_retain.Size = new System.Drawing.Size(108, 16);
            this.temp_retain.TabIndex = 98;
            this.temp_retain.Text = "保留上次的结果";
            this.temp_retain.UseVisualStyleBackColor = true;
            // 
            // wr_config
            // 
            this.wr_config.AutoSize = true;
            this.wr_config.Checked = true;
            this.wr_config.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wr_config.Location = new System.Drawing.Point(286, 166);
            this.wr_config.Name = "wr_config";
            this.wr_config.Size = new System.Drawing.Size(108, 16);
            this.wr_config.TabIndex = 97;
            this.wr_config.Text = "结果写入配置字";
            this.wr_config.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.high_temp);
            this.groupBox1.Controls.Add(this.low_temp);
            this.groupBox1.Location = new System.Drawing.Point(166, 169);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(53, 50);
            this.groupBox1.TabIndex = 96;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "温度";
            // 
            // high_temp
            // 
            this.high_temp.AutoSize = true;
            this.high_temp.Location = new System.Drawing.Point(6, 29);
            this.high_temp.Name = "high_temp";
            this.high_temp.Size = new System.Drawing.Size(47, 16);
            this.high_temp.TabIndex = 98;
            this.high_temp.Text = "高温";
            this.high_temp.UseVisualStyleBackColor = true;
            // 
            // low_temp
            // 
            this.low_temp.AutoSize = true;
            this.low_temp.Checked = true;
            this.low_temp.Location = new System.Drawing.Point(6, 13);
            this.low_temp.Name = "low_temp";
            this.low_temp.Size = new System.Drawing.Size(47, 16);
            this.low_temp.TabIndex = 97;
            this.low_temp.TabStop = true;
            this.low_temp.Text = "低温";
            this.low_temp.UseVisualStyleBackColor = true;
            // 
            // temp_fm347
            // 
            this.temp_fm347.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.temp_fm347.Location = new System.Drawing.Point(21, 176);
            this.temp_fm347.Name = "temp_fm347";
            this.temp_fm347.Size = new System.Drawing.Size(60, 44);
            this.temp_fm347.TabIndex = 95;
            this.temp_fm347.Text = "FM347 挑卡";
            this.temp_fm347.UseVisualStyleBackColor = true;
            this.temp_fm347.Click += new System.EventHandler(this.temp_fm347_Click);
            // 
            // user_lock_button
            // 
            this.user_lock_button.Location = new System.Drawing.Point(277, 351);
            this.user_lock_button.Name = "user_lock_button";
            this.user_lock_button.Size = new System.Drawing.Size(93, 43);
            this.user_lock_button.TabIndex = 94;
            this.user_lock_button.Text = "一所正式lock";
            this.user_lock_button.UseVisualStyleBackColor = true;
            this.user_lock_button.Click += new System.EventHandler(this.user_lock_button_Click);
            // 
            // userfor_passport_button
            // 
            this.userfor_passport_button.Location = new System.Drawing.Point(166, 351);
            this.userfor_passport_button.Name = "userfor_passport_button";
            this.userfor_passport_button.Size = new System.Drawing.Size(93, 43);
            this.userfor_passport_button.TabIndex = 93;
            this.userfor_passport_button.Text = "一所user";
            this.userfor_passport_button.UseVisualStyleBackColor = true;
            this.userfor_passport_button.Click += new System.EventHandler(this.userfor_passport_button_Click);
            // 
            // cfg_and_progfor_passport_button
            // 
            this.cfg_and_progfor_passport_button.Location = new System.Drawing.Point(225, 302);
            this.cfg_and_progfor_passport_button.Name = "cfg_and_progfor_passport_button";
            this.cfg_and_progfor_passport_button.Size = new System.Drawing.Size(93, 43);
            this.cfg_and_progfor_passport_button.TabIndex = 92;
            this.cfg_and_progfor_passport_button.Text = "一所cfg和prog";
            this.cfg_and_progfor_passport_button.UseVisualStyleBackColor = true;
            this.cfg_and_progfor_passport_button.Click += new System.EventHandler(this.cfg_and_progfor_passport_button_Click);
            // 
            // trimfor_passport_button
            // 
            this.trimfor_passport_button.Location = new System.Drawing.Point(225, 253);
            this.trimfor_passport_button.Name = "trimfor_passport_button";
            this.trimfor_passport_button.Size = new System.Drawing.Size(93, 43);
            this.trimfor_passport_button.TabIndex = 91;
            this.trimfor_passport_button.Text = "一所trim";
            this.trimfor_passport_button.UseVisualStyleBackColor = true;
            this.trimfor_passport_button.Click += new System.EventHandler(this.trimfor_passport_button_Click);
            // 
            // groupBox20
            // 
            this.groupBox20.Controls.Add(this.record_num);
            this.groupBox20.Controls.Add(this.label88);
            this.groupBox20.Controls.Add(this.record_interface);
            this.groupBox20.Controls.Add(this.label87);
            this.groupBox20.Controls.Add(this.record_card);
            this.groupBox20.Controls.Add(this.label86);
            this.groupBox20.Controls.Add(this.label85);
            this.groupBox20.Controls.Add(this.record_name);
            this.groupBox20.Location = new System.Drawing.Point(824, 245);
            this.groupBox20.Name = "groupBox20";
            this.groupBox20.Size = new System.Drawing.Size(126, 115);
            this.groupBox20.TabIndex = 90;
            this.groupBox20.TabStop = false;
            this.groupBox20.Text = "送检记录";
            // 
            // record_num
            // 
            this.record_num.Location = new System.Drawing.Point(54, 84);
            this.record_num.Name = "record_num";
            this.record_num.Size = new System.Drawing.Size(62, 21);
            this.record_num.TabIndex = 7;
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Location = new System.Drawing.Point(1, 89);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(53, 12);
            this.label88.TabIndex = 6;
            this.label88.Text = "送检数量";
            // 
            // record_interface
            // 
            this.record_interface.Location = new System.Drawing.Point(54, 60);
            this.record_interface.Name = "record_interface";
            this.record_interface.Size = new System.Drawing.Size(62, 21);
            this.record_interface.TabIndex = 5;
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(1, 66);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(53, 12);
            this.label87.TabIndex = 4;
            this.label87.Text = "接口类型";
            // 
            // record_card
            // 
            this.record_card.Location = new System.Drawing.Point(53, 35);
            this.record_card.Name = "record_card";
            this.record_card.Size = new System.Drawing.Size(62, 21);
            this.record_card.TabIndex = 3;
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(1, 42);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(53, 12);
            this.label86.TabIndex = 2;
            this.label86.Text = "卡片版本";
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(6, 18);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(41, 12);
            this.label85.TabIndex = 1;
            this.label85.Text = "操作员";
            // 
            // record_name
            // 
            this.record_name.Location = new System.Drawing.Point(53, 13);
            this.record_name.Name = "record_name";
            this.record_name.Size = new System.Drawing.Size(62, 21);
            this.record_name.TabIndex = 0;
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.White;
            this.button13.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button13.ForeColor = System.Drawing.Color.Blue;
            this.button13.Location = new System.Drawing.Point(841, 208);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(93, 32);
            this.button13.TabIndex = 89;
            this.button13.Text = "送检情况记录";
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // FM336_Endurance_button
            // 
            this.FM336_Endurance_button.Location = new System.Drawing.Point(653, 329);
            this.FM336_Endurance_button.Name = "FM336_Endurance_button";
            this.FM336_Endurance_button.Size = new System.Drawing.Size(108, 46);
            this.FM336_Endurance_button.TabIndex = 88;
            this.FM336_Endurance_button.Text = "FM336擦写";
            this.FM336_Endurance_button.UseVisualStyleBackColor = true;
            this.FM336_Endurance_button.Click += new System.EventHandler(this.FM336_Endurance_button_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.bkpr);
            this.groupBox8.Controls.Add(this.bkpw);
            this.groupBox8.Location = new System.Drawing.Point(277, 108);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(55, 55);
            this.groupBox8.TabIndex = 87;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "BK/RE";
            // 
            // bkpr
            // 
            this.bkpr.AutoSize = true;
            this.bkpr.Location = new System.Drawing.Point(6, 32);
            this.bkpr.Name = "bkpr";
            this.bkpr.Size = new System.Drawing.Size(47, 16);
            this.bkpr.TabIndex = 1;
            this.bkpr.TabStop = true;
            this.bkpr.Text = "恢复";
            this.bkpr.UseVisualStyleBackColor = true;
            // 
            // bkpw
            // 
            this.bkpw.AutoSize = true;
            this.bkpw.Location = new System.Drawing.Point(6, 15);
            this.bkpw.Name = "bkpw";
            this.bkpw.Size = new System.Drawing.Size(47, 16);
            this.bkpw.TabIndex = 0;
            this.bkpw.TabStop = true;
            this.bkpw.Text = "备份";
            this.bkpw.UseVisualStyleBackColor = true;
            // 
            // bkp
            // 
            this.bkp.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bkp.Location = new System.Drawing.Point(191, 110);
            this.bkp.Name = "bkp";
            this.bkp.Size = new System.Drawing.Size(83, 57);
            this.bkp.TabIndex = 86;
            this.bkp.Text = "温度配置备份/恢复";
            this.bkp.UseVisualStyleBackColor = true;
            this.bkp.Click += new System.EventHandler(this.bkp_Click);
            // 
            // auto_freq_test
            // 
            this.auto_freq_test.Location = new System.Drawing.Point(24, 351);
            this.auto_freq_test.Name = "auto_freq_test";
            this.auto_freq_test.Size = new System.Drawing.Size(105, 47);
            this.auto_freq_test.TabIndex = 85;
            this.auto_freq_test.Text = "FM336升频测试";
            this.auto_freq_test.UseVisualStyleBackColor = true;
            this.auto_freq_test.Click += new System.EventHandler(this.auto_freq_test_Click);
            // 
            // cfgroup
            // 
            this.cfgroup.Controls.Add(this.rston);
            this.cfgroup.Controls.Add(this.checkrlt);
            this.cfgroup.Controls.Add(this.cfwconfig);
            this.cfgroup.Controls.Add(this.cfw);
            this.cfgroup.Controls.Add(this.prog_cos);
            this.cfgroup.Controls.Add(this.user_lock);
            this.cfgroup.Cursor = System.Windows.Forms.Cursors.Default;
            this.cfgroup.Location = new System.Drawing.Point(832, 60);
            this.cfgroup.Name = "cfgroup";
            this.cfgroup.Size = new System.Drawing.Size(115, 137);
            this.cfgroup.TabIndex = 84;
            this.cfgroup.TabStop = false;
            this.cfgroup.Text = "配置项目";
            // 
            // rston
            // 
            this.rston.AutoSize = true;
            this.rston.Location = new System.Drawing.Point(9, 89);
            this.rston.Name = "rston";
            this.rston.Size = new System.Drawing.Size(72, 16);
            this.rston.TabIndex = 84;
            this.rston.Text = "开启复位";
            this.rston.UseVisualStyleBackColor = true;
            // 
            // checkrlt
            // 
            this.checkrlt.AutoSize = true;
            this.checkrlt.Location = new System.Drawing.Point(9, 119);
            this.checkrlt.Name = "checkrlt";
            this.checkrlt.Size = new System.Drawing.Size(96, 16);
            this.checkrlt.TabIndex = 83;
            this.checkrlt.Text = "接口应答检验";
            this.checkrlt.UseVisualStyleBackColor = true;
            // 
            // cfwconfig
            // 
            this.cfwconfig.AutoSize = true;
            this.cfwconfig.Location = new System.Drawing.Point(7, 14);
            this.cfwconfig.Name = "cfwconfig";
            this.cfwconfig.Size = new System.Drawing.Size(84, 16);
            this.cfwconfig.TabIndex = 82;
            this.cfwconfig.Text = "配置控制字";
            this.cfwconfig.UseVisualStyleBackColor = true;
            // 
            // cfw
            // 
            this.cfw.Controls.Add(this.cfw_agn);
            this.cfw.Controls.Add(this.cfw_rnd);
            this.cfw.Location = new System.Drawing.Point(4, 30);
            this.cfw.Name = "cfw";
            this.cfw.Size = new System.Drawing.Size(99, 43);
            this.cfw.TabIndex = 81;
            this.cfw.TabStop = false;
            this.cfw.Text = "控制字修改";
            // 
            // cfw_agn
            // 
            this.cfw_agn.AutoSize = true;
            this.cfw_agn.Checked = true;
            this.cfw_agn.Location = new System.Drawing.Point(4, 25);
            this.cfw_agn.Name = "cfw_agn";
            this.cfw_agn.Size = new System.Drawing.Size(95, 16);
            this.cfw_agn.TabIndex = 1;
            this.cfw_agn.TabStop = true;
            this.cfw_agn.Text = "控制字再修改";
            this.cfw_agn.UseVisualStyleBackColor = true;
            // 
            // cfw_rnd
            // 
            this.cfw_rnd.AutoSize = true;
            this.cfw_rnd.Location = new System.Drawing.Point(4, 11);
            this.cfw_rnd.Name = "cfw_rnd";
            this.cfw_rnd.Size = new System.Drawing.Size(71, 16);
            this.cfw_rnd.TabIndex = 0;
            this.cfw_rnd.Text = "反码扰乱";
            this.cfw_rnd.UseVisualStyleBackColor = true;
            // 
            // prog_cos
            // 
            this.prog_cos.AutoSize = true;
            this.prog_cos.Checked = true;
            this.prog_cos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.prog_cos.Location = new System.Drawing.Point(9, 74);
            this.prog_cos.Name = "prog_cos";
            this.prog_cos.Size = new System.Drawing.Size(66, 16);
            this.prog_cos.TabIndex = 67;
            this.prog_cos.Text = "下载COS";
            this.prog_cos.UseVisualStyleBackColor = true;
            this.prog_cos.CheckedChanged += new System.EventHandler(this.prog_cos_CheckedChanged);
            // 
            // user_lock
            // 
            this.user_lock.AutoSize = true;
            this.user_lock.Location = new System.Drawing.Point(9, 104);
            this.user_lock.Name = "user_lock";
            this.user_lock.Size = new System.Drawing.Size(84, 16);
            this.user_lock.TabIndex = 68;
            this.user_lock.Text = "用户及LOCK";
            this.user_lock.UseVisualStyleBackColor = true;
            this.user_lock.CheckedChanged += new System.EventHandler(this.user_lock_CheckedChanged);
            // 
            // cfg
            // 
            this.cfg.Controls.Add(this.seload);
            this.cfg.Controls.Add(this.fuload);
            this.cfg.Location = new System.Drawing.Point(754, 91);
            this.cfg.Name = "cfg";
            this.cfg.Size = new System.Drawing.Size(73, 47);
            this.cfg.TabIndex = 83;
            this.cfg.TabStop = false;
            this.cfg.Text = "配置方式";
            // 
            // seload
            // 
            this.seload.AutoSize = true;
            this.seload.Checked = true;
            this.seload.Location = new System.Drawing.Point(3, 25);
            this.seload.Name = "seload";
            this.seload.Size = new System.Drawing.Size(71, 16);
            this.seload.TabIndex = 1;
            this.seload.TabStop = true;
            this.seload.Text = "选择配置";
            this.seload.UseVisualStyleBackColor = true;
            this.seload.CheckedChanged += new System.EventHandler(this.seload_CheckedChanged);
            // 
            // fuload
            // 
            this.fuload.AutoSize = true;
            this.fuload.Location = new System.Drawing.Point(3, 11);
            this.fuload.Name = "fuload";
            this.fuload.Size = new System.Drawing.Size(71, 16);
            this.fuload.TabIndex = 0;
            this.fuload.Text = "一键配置";
            this.fuload.UseVisualStyleBackColor = true;
            this.fuload.CheckedChanged += new System.EventHandler(this.fuload_CheckedChanged);
            // 
            // sjxm
            // 
            this.sjxm.Controls.Add(this.zc);
            this.sjxm.Controls.Add(this.bank326);
            this.sjxm.Controls.Add(this.gm);
            this.sjxm.Controls.Add(this.bright);
            this.sjxm.Controls.Add(this.bank);
            this.sjxm.Controls.Add(this.ty);
            this.sjxm.Location = new System.Drawing.Point(660, 62);
            this.sjxm.Name = "sjxm";
            this.sjxm.Size = new System.Drawing.Size(91, 90);
            this.sjxm.TabIndex = 82;
            this.sjxm.TabStop = false;
            this.sjxm.Text = "336送检项目";
            // 
            // zc
            // 
            this.zc.AutoSize = true;
            this.zc.Location = new System.Drawing.Point(46, 25);
            this.zc.Name = "zc";
            this.zc.Size = new System.Drawing.Size(47, 16);
            this.zc.TabIndex = 5;
            this.zc.TabStop = true;
            this.zc.Text = "总参";
            this.zc.UseVisualStyleBackColor = true;
            this.zc.CheckedChanged += new System.EventHandler(this.zc_CheckedChanged);
            // 
            // bank326
            // 
            this.bank326.AutoSize = true;
            this.bank326.Location = new System.Drawing.Point(2, 68);
            this.bank326.Name = "bank326";
            this.bank326.Size = new System.Drawing.Size(65, 16);
            this.bank326.TabIndex = 4;
            this.bank326.TabStop = true;
            this.bank326.Text = "326银检";
            this.bank326.UseVisualStyleBackColor = true;
            this.bank326.CheckedChanged += new System.EventHandler(this.bank326_CheckedChanged);
            // 
            // gm
            // 
            this.gm.AutoSize = true;
            this.gm.Location = new System.Drawing.Point(2, 54);
            this.gm.Name = "gm";
            this.gm.Size = new System.Drawing.Size(47, 16);
            this.gm.TabIndex = 3;
            this.gm.TabStop = true;
            this.gm.Text = "国密";
            this.gm.UseVisualStyleBackColor = true;
            // 
            // bright
            // 
            this.bright.AutoSize = true;
            this.bright.Checked = true;
            this.bright.Location = new System.Drawing.Point(2, 10);
            this.bright.Name = "bright";
            this.bright.Size = new System.Drawing.Size(89, 16);
            this.bright.TabIndex = 2;
            this.bright.TabStop = true;
            this.bright.Text = "brightsight";
            this.bright.UseVisualStyleBackColor = true;
            this.bright.CheckedChanged += new System.EventHandler(this.bright_CheckedChanged);
            // 
            // bank
            // 
            this.bank.AutoSize = true;
            this.bank.Location = new System.Drawing.Point(2, 25);
            this.bank.Name = "bank";
            this.bank.Size = new System.Drawing.Size(47, 16);
            this.bank.TabIndex = 1;
            this.bank.Text = "银检";
            this.bank.UseVisualStyleBackColor = true;
            this.bank.CheckedChanged += new System.EventHandler(this.bank_CheckedChanged);
            // 
            // ty
            // 
            this.ty.AutoSize = true;
            this.ty.Location = new System.Drawing.Point(2, 39);
            this.ty.Name = "ty";
            this.ty.Size = new System.Drawing.Size(47, 16);
            this.ty.TabIndex = 0;
            this.ty.Text = "天语";
            this.ty.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button11.Location = new System.Drawing.Point(24, 287);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(105, 54);
            this.button11.TabIndex = 78;
            this.button11.Text = "温度trim\r\n理论与实测";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(400, 147);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(115, 48);
            this.button10.TabIndex = 76;
            this.button10.Text = "brightsight温度";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // groupBox19
            // 
            this.groupBox19.Controls.Add(this.Scl);
            this.groupBox19.Controls.Add(this.Sct);
            this.groupBox19.Location = new System.Drawing.Point(138, 114);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.Size = new System.Drawing.Size(47, 49);
            this.groupBox19.TabIndex = 75;
            this.groupBox19.TabStop = false;
            this.groupBox19.Text = "接口";
            // 
            // Scl
            // 
            this.Scl.AutoSize = true;
            this.Scl.Location = new System.Drawing.Point(6, 12);
            this.Scl.Name = "Scl";
            this.Scl.Size = new System.Drawing.Size(35, 16);
            this.Scl.TabIndex = 1;
            this.Scl.Text = "CL";
            this.Scl.UseVisualStyleBackColor = true;
            // 
            // Sct
            // 
            this.Sct.AutoSize = true;
            this.Sct.Checked = true;
            this.Sct.Location = new System.Drawing.Point(6, 27);
            this.Sct.Name = "Sct";
            this.Sct.Size = new System.Drawing.Size(35, 16);
            this.Sct.TabIndex = 0;
            this.Sct.TabStop = true;
            this.Sct.Text = "CT";
            this.Sct.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(24, 245);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(96, 34);
            this.button8.TabIndex = 74;
            this.button8.Text = "光误报警检测";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // groupBox18
            // 
            this.groupBox18.Controls.Add(this.ITct);
            this.groupBox18.Controls.Add(this.ITcl);
            this.groupBox18.Location = new System.Drawing.Point(753, 62);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.Size = new System.Drawing.Size(74, 26);
            this.groupBox18.TabIndex = 73;
            this.groupBox18.TabStop = false;
            this.groupBox18.Text = "下载接口";
            // 
            // ITct
            // 
            this.ITct.AutoSize = true;
            this.ITct.Location = new System.Drawing.Point(37, 11);
            this.ITct.Name = "ITct";
            this.ITct.Size = new System.Drawing.Size(35, 16);
            this.ITct.TabIndex = 1;
            this.ITct.Text = "CT";
            this.ITct.UseVisualStyleBackColor = true;
            // 
            // ITcl
            // 
            this.ITcl.AutoSize = true;
            this.ITcl.Checked = true;
            this.ITcl.Location = new System.Drawing.Point(4, 11);
            this.ITcl.Name = "ITcl";
            this.ITcl.Size = new System.Drawing.Size(35, 16);
            this.ITcl.TabIndex = 0;
            this.ITcl.TabStop = true;
            this.ITcl.Text = "CL";
            this.ITcl.UseVisualStyleBackColor = true;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Location = new System.Drawing.Point(903, 42);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(47, 12);
            this.label74.TabIndex = 72;
            this.label74.Text = "COS地址";
            // 
            // ADcos
            // 
            this.ADcos.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ADcos.ForeColor = System.Drawing.Color.Black;
            this.ADcos.Location = new System.Drawing.Point(574, 36);
            this.ADcos.Name = "ADcos";
            this.ADcos.Size = new System.Drawing.Size(319, 23);
            this.ADcos.TabIndex = 71;
            this.ADcos.Text = ".\\\\FM1280_EMVCo_COS_V170.hex";
            // 
            // label73
            // 
            this.label73.AutoSize = true;
            this.label73.Location = new System.Drawing.Point(897, 10);
            this.label73.Name = "label73";
            this.label73.Size = new System.Drawing.Size(53, 12);
            this.label73.TabIndex = 70;
            this.label73.Text = "认证密钥";
            // 
            // Akey
            // 
            this.Akey.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Akey.ForeColor = System.Drawing.Color.Maroon;
            this.Akey.Location = new System.Drawing.Point(574, 4);
            this.Akey.Name = "Akey";
            this.Akey.Size = new System.Drawing.Size(319, 26);
            this.Akey.TabIndex = 69;
            this.Akey.Text = "772B7F0C1CCDF4F8417C57DEBC06635E";
            // 
            // brst
            // 
            this.brst.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.brst.Location = new System.Drawing.Point(593, 62);
            this.brst.Name = "brst";
            this.brst.Size = new System.Drawing.Size(63, 61);
            this.brst.TabIndex = 65;
            this.brst.Text = "送检配置";
            this.brst.UseVisualStyleBackColor = true;
            this.brst.Click += new System.EventHandler(this.brst_Click);
            // 
            // temp_select
            // 
            this.temp_select.FormattingEnabled = true;
            this.temp_select.Items.AddRange(new object[] {
            "-40℃报警 (低温报警范围 -30℃~-40℃)",
            "-29℃不报 (低温报警范围 -30℃~-40℃)",
            "110℃报警  (高温报警范围 100℃~110℃)",
            "99℃不报  (高温报警范围 100℃~110℃)"});
            this.temp_select.Location = new System.Drawing.Point(33, 10);
            this.temp_select.Name = "temp_select";
            this.temp_select.Size = new System.Drawing.Size(251, 20);
            this.temp_select.TabIndex = 64;
            this.temp_select.Tag = "";
            this.temp_select.Text = "高低温筛卡范围选择";
            // 
            // FM294_str
            // 
            this.FM294_str.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FM294_str.Location = new System.Drawing.Point(33, 124);
            this.FM294_str.Name = "FM294_str";
            this.FM294_str.Size = new System.Drawing.Size(96, 43);
            this.FM294_str.TabIndex = 63;
            this.FM294_str.Text = "FM294_CL";
            this.FM294_str.UseVisualStyleBackColor = true;
            this.FM294_str.Click += new System.EventHandler(this.FM294_str_Click);
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.FM336);
            this.groupBox17.Controls.Add(this.FM309);
            this.groupBox17.Controls.Add(this.FM326);
            this.groupBox17.Location = new System.Drawing.Point(141, 37);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(60, 69);
            this.groupBox17.TabIndex = 62;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "类型";
            // 
            // FM336
            // 
            this.FM336.AutoSize = true;
            this.FM336.Checked = true;
            this.FM336.Location = new System.Drawing.Point(6, 47);
            this.FM336.Name = "FM336";
            this.FM336.Size = new System.Drawing.Size(53, 16);
            this.FM336.TabIndex = 56;
            this.FM336.TabStop = true;
            this.FM336.Text = "FM336";
            this.FM336.UseVisualStyleBackColor = true;
            // 
            // FM309
            // 
            this.FM309.AutoSize = true;
            this.FM309.Location = new System.Drawing.Point(6, 14);
            this.FM309.Name = "FM309";
            this.FM309.Size = new System.Drawing.Size(53, 16);
            this.FM309.TabIndex = 52;
            this.FM309.Text = "FM309";
            this.FM309.UseVisualStyleBackColor = true;
            // 
            // FM326
            // 
            this.FM326.AutoSize = true;
            this.FM326.Location = new System.Drawing.Point(6, 31);
            this.FM326.Name = "FM326";
            this.FM326.Size = new System.Drawing.Size(53, 16);
            this.FM326.TabIndex = 53;
            this.FM326.Text = "FM326";
            this.FM326.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.init_n);
            this.groupBox16.Controls.Add(this.init_y);
            this.groupBox16.Location = new System.Drawing.Point(290, 6);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(92, 31);
            this.groupBox16.TabIndex = 60;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "控制字初始化";
            // 
            // init_n
            // 
            this.init_n.AutoSize = true;
            this.init_n.Location = new System.Drawing.Point(47, 12);
            this.init_n.Name = "init_n";
            this.init_n.Size = new System.Drawing.Size(35, 16);
            this.init_n.TabIndex = 61;
            this.init_n.TabStop = true;
            this.init_n.Text = "否";
            this.init_n.UseVisualStyleBackColor = true;
            // 
            // init_y
            // 
            this.init_y.AutoSize = true;
            this.init_y.Location = new System.Drawing.Point(6, 12);
            this.init_y.Name = "init_y";
            this.init_y.Size = new System.Drawing.Size(35, 16);
            this.init_y.TabIndex = 61;
            this.init_y.TabStop = true;
            this.init_y.Text = "是";
            this.init_y.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.radioButton7);
            this.groupBox11.Controls.Add(this.radioButton6);
            this.groupBox11.Location = new System.Drawing.Point(487, 203);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(82, 73);
            this.groupBox11.TabIndex = 57;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "接口";
            // 
            // radioButton7
            // 
            this.radioButton7.AutoSize = true;
            this.radioButton7.Location = new System.Drawing.Point(16, 48);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(47, 16);
            this.radioButton7.TabIndex = 53;
            this.radioButton7.Text = "接触";
            this.radioButton7.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Checked = true;
            this.radioButton6.Location = new System.Drawing.Point(16, 18);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(47, 16);
            this.radioButton6.TabIndex = 52;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "非接";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(409, 294);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(66, 43);
            this.button6.TabIndex = 3;
            this.button6.Text = "COS_test";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(405, 223);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(72, 46);
            this.button5.TabIndex = 2;
            this.button5.Text = "启动";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // temp_rlt
            // 
            this.temp_rlt.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.temp_rlt.ForeColor = System.Drawing.Color.Blue;
            this.temp_rlt.Location = new System.Drawing.Point(225, 44);
            this.temp_rlt.Name = "temp_rlt";
            this.temp_rlt.Size = new System.Drawing.Size(244, 58);
            this.temp_rlt.TabIndex = 1;
            this.temp_rlt.Text = "挑卡结果";
            // 
            // temp_start
            // 
            this.temp_start.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.temp_start.Location = new System.Drawing.Point(33, 37);
            this.temp_start.Name = "temp_start";
            this.temp_start.Size = new System.Drawing.Size(96, 82);
            this.temp_start.TabIndex = 0;
            this.temp_start.Text = "FM309\r\nFM326\r\nFM336";
            this.temp_start.UseVisualStyleBackColor = true;
            this.temp_start.Click += new System.EventHandler(this.temp_start_Click);
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.radioButton5);
            this.groupBox12.Controls.Add(this.radioButton4);
            this.groupBox12.Controls.Add(this.radioButton3);
            this.groupBox12.Location = new System.Drawing.Point(481, 282);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(106, 94);
            this.groupBox12.TabIndex = 58;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "功能选择";
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Checked = true;
            this.radioButton5.Location = new System.Drawing.Point(6, 25);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(89, 16);
            this.radioButton5.TabIndex = 59;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "ETC程序下载";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(6, 47);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(95, 16);
            this.radioButton4.TabIndex = 58;
            this.radioButton4.Text = "基本功能测试";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(6, 69);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(83, 16);
            this.radioButton3.TabIndex = 57;
            this.radioButton3.Text = "EE功能测试";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // tabPage_Program
            // 
            this.tabPage_Program.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_Program.Controls.Add(this.retry);
            this.tabPage_Program.Controls.Add(this.pre_prog);
            this.tabPage_Program.Controls.Add(this.sec_addr);
            this.tabPage_Program.Controls.Add(this.chkReadVerify);
            this.tabPage_Program.Controls.Add(this.groupBox6);
            this.tabPage_Program.Controls.Add(this.groupBox_chips);
            this.tabPage_Program.Controls.Add(this.label72);
            this.tabPage_Program.Controls.Add(this.label71);
            this.tabPage_Program.Controls.Add(this.WR_TIME);
            this.tabPage_Program.Controls.Add(this.ER_TIME);
            this.tabPage_Program.Controls.Add(this.label69);
            this.tabPage_Program.Controls.Add(this.ProgEEOffsetAddr);
            this.tabPage_Program.Controls.Add(this.label67);
            this.tabPage_Program.Controls.Add(this.groupBox_350_Cfg);
            this.tabPage_Program.Controls.Add(this.label68);
            this.tabPage_Program.Controls.Add(this.textBox1);
            this.tabPage_Program.Controls.Add(this.EE_patch_checkBox);
            this.tabPage_Program.Controls.Add(this.groupBox13);
            this.tabPage_Program.Controls.Add(this.flash_check);
            this.tabPage_Program.Controls.Add(this.BufMove_checkBox);
            this.tabPage_Program.Controls.Add(this.checkBox_CTHighBaud);
            this.tabPage_Program.Controls.Add(this.Prog_Move);
            this.tabPage_Program.Controls.Add(this.Card_init_Button);
            this.tabPage_Program.Controls.Add(this.dest_label1);
            this.tabPage_Program.Controls.Add(this.ProgDestAddr);
            this.tabPage_Program.Controls.Add(this.label64);
            this.tabPage_Program.Controls.Add(this.key_richTextBox);
            this.tabPage_Program.Controls.Add(this.keyl_label);
            this.tabPage_Program.Controls.Add(this.InitData_comboBox);
            this.tabPage_Program.Controls.Add(this.Auth_checkBox);
            this.tabPage_Program.Controls.Add(this.label61);
            this.tabPage_Program.Controls.Add(this.lib_Size);
            this.tabPage_Program.Controls.Add(this.label60);
            this.tabPage_Program.Controls.Add(this.flash_isp);
            this.tabPage_Program.Controls.Add(this.MEM_EE);
            this.tabPage_Program.Controls.Add(this.MEM_ROM);
            this.tabPage_Program.Controls.Add(this.SaveAs_verify);
            this.tabPage_Program.Controls.Add(this.button4);
            this.tabPage_Program.Controls.Add(this.prog_extand_mode1);
            this.tabPage_Program.Controls.Add(this.prog_extand_memRAM);
            this.tabPage_Program.Controls.Add(this.prog_extand_memEE);
            this.tabPage_Program.Controls.Add(this.prog_extand_mode0);
            this.tabPage_Program.Controls.Add(this.SaveAsStart);
            this.tabPage_Program.Controls.Add(this.SaveAsEnd);
            this.tabPage_Program.Controls.Add(this.SaveAS_Button);
            this.tabPage_Program.Controls.Add(this.A1Program);
            this.tabPage_Program.Controls.Add(this.ProgEE_progressBar);
            this.tabPage_Program.Controls.Add(this.label47);
            this.tabPage_Program.Controls.Add(this.label55);
            this.tabPage_Program.Controls.Add(this.label52);
            this.tabPage_Program.Controls.Add(this.Patch_Size);
            this.tabPage_Program.Controls.Add(this.label46);
            this.tabPage_Program.Controls.Add(this.ProgEEStartAddr);
            this.tabPage_Program.Controls.Add(this.ProgEEEndAddr);
            this.tabPage_Program.Controls.Add(this.InitEEdata_Button);
            this.tabPage_Program.Controls.Add(this.ProgEE_Button);
            this.tabPage_Program.Controls.Add(this.OpenFile_Button);
            this.tabPage_Program.Controls.Add(this.MemTypeSelGrop);
            this.tabPage_Program.Controls.Add(this.groupBoxOperation);
            this.tabPage_Program.Controls.Add(this.interface_sel_grpbox);
            this.tabPage_Program.Controls.Add(this.progEEdataGridView);
            this.tabPage_Program.Controls.Add(this.groupBox7);
            this.tabPage_Program.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Program.Name = "tabPage_Program";
            this.tabPage_Program.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Program.Size = new System.Drawing.Size(956, 406);
            this.tabPage_Program.TabIndex = 2;
            this.tabPage_Program.Text = "编程操作";
            // 
            // retry
            // 
            this.retry.AutoSize = true;
            this.retry.Location = new System.Drawing.Point(83, 125);
            this.retry.Name = "retry";
            this.retry.Size = new System.Drawing.Size(84, 16);
            this.retry.TabIndex = 81;
            this.retry.Text = "retry 编程";
            this.retry.UseVisualStyleBackColor = true;
            // 
            // pre_prog
            // 
            this.pre_prog.AutoSize = true;
            this.pre_prog.Location = new System.Drawing.Point(83, 107);
            this.pre_prog.Name = "pre_prog";
            this.pre_prog.Size = new System.Drawing.Size(72, 16);
            this.pre_prog.TabIndex = 80;
            this.pre_prog.Text = "pre_prog";
            this.pre_prog.UseVisualStyleBackColor = true;
            // 
            // sec_addr
            // 
            this.sec_addr.FormattingEnabled = true;
            this.sec_addr.Items.AddRange(new object[] {
            "程序 用户程序区-00",
            "程序 bootload区-50",
            "程序 firmware区-70",
            "数据 通用数据区-B0",
            "数据 敏感数据区-D0",
            "数据 逻辑加密区-DA",
            "RAM 补丁程序区-30",
            "RAM CPU缓存区-80",
            "RAM 非接缓存区-AF"});
            this.sec_addr.Location = new System.Drawing.Point(82, 83);
            this.sec_addr.Name = "sec_addr";
            this.sec_addr.Size = new System.Drawing.Size(121, 20);
            this.sec_addr.TabIndex = 79;
            this.sec_addr.Text = "段地址";
            this.sec_addr.SelectedIndexChanged += new System.EventHandler(this.sec_addr_SelectedIndexChanged);
            // 
            // chkReadVerify
            // 
            this.chkReadVerify.AutoSize = true;
            this.chkReadVerify.Location = new System.Drawing.Point(60, 231);
            this.chkReadVerify.Name = "chkReadVerify";
            this.chkReadVerify.Size = new System.Drawing.Size(48, 16);
            this.chkReadVerify.TabIndex = 15;
            this.chkReadVerify.Text = "校验";
            this.chkReadVerify.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.FM347_radioButton);
            this.groupBox6.Controls.Add(this.FM326_radioButton);
            this.groupBox6.Controls.Add(this.FM302_radioButton);
            this.groupBox6.Controls.Add(this.FM350_radioButton);
            this.groupBox6.Controls.Add(this.FM274_radioButton);
            this.groupBox6.Controls.Add(this.FM294_radioButton);
            this.groupBox6.Controls.Add(this.FM349_radioButton);
            this.groupBox6.Controls.Add(this.FM309_radioButton);
            this.groupBox6.Controls.Add(this.FM336_radioButton);
            this.groupBox6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox6.Location = new System.Drawing.Point(535, 105);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(53, 177);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "隐藏面板";
            this.groupBox6.Visible = false;
            // 
            // FM347_radioButton
            // 
            this.FM347_radioButton.AutoSize = true;
            this.FM347_radioButton.Location = new System.Drawing.Point(4, 144);
            this.FM347_radioButton.Name = "FM347_radioButton";
            this.FM347_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM347_radioButton.TabIndex = 80;
            this.FM347_radioButton.Text = "347";
            this.FM347_radioButton.UseVisualStyleBackColor = true;
            // 
            // FM326_radioButton
            // 
            this.FM326_radioButton.AutoSize = true;
            this.FM326_radioButton.Location = new System.Drawing.Point(4, 49);
            this.FM326_radioButton.Name = "FM326_radioButton";
            this.FM326_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM326_radioButton.TabIndex = 79;
            this.FM326_radioButton.Text = "326";
            this.FM326_radioButton.UseVisualStyleBackColor = true;
            // 
            // FM302_radioButton
            // 
            this.FM302_radioButton.AutoSize = true;
            this.FM302_radioButton.Location = new System.Drawing.Point(4, 128);
            this.FM302_radioButton.Name = "FM302_radioButton";
            this.FM302_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM302_radioButton.TabIndex = 78;
            this.FM302_radioButton.Text = "302";
            this.FM302_radioButton.UseVisualStyleBackColor = true;
            // 
            // FM350_radioButton
            // 
            this.FM350_radioButton.AutoSize = true;
            this.FM350_radioButton.Location = new System.Drawing.Point(4, 80);
            this.FM350_radioButton.Name = "FM350_radioButton";
            this.FM350_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM350_radioButton.TabIndex = 77;
            this.FM350_radioButton.Text = "350";
            this.FM350_radioButton.UseVisualStyleBackColor = true;
            this.FM350_radioButton.CheckedChanged += new System.EventHandler(this.FM350_radioButton_CheckedChanged);
            // 
            // FM274_radioButton
            // 
            this.FM274_radioButton.AutoSize = true;
            this.FM274_radioButton.Location = new System.Drawing.Point(4, 112);
            this.FM274_radioButton.Name = "FM274_radioButton";
            this.FM274_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM274_radioButton.TabIndex = 76;
            this.FM274_radioButton.Text = "274";
            this.FM274_radioButton.UseVisualStyleBackColor = true;
            // 
            // FM294_radioButton
            // 
            this.FM294_radioButton.AutoSize = true;
            this.FM294_radioButton.Location = new System.Drawing.Point(4, 96);
            this.FM294_radioButton.Name = "FM294_radioButton";
            this.FM294_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM294_radioButton.TabIndex = 75;
            this.FM294_radioButton.Text = "294";
            this.FM294_radioButton.UseVisualStyleBackColor = true;
            // 
            // FM349_radioButton
            // 
            this.FM349_radioButton.AutoSize = true;
            this.FM349_radioButton.Location = new System.Drawing.Point(4, 65);
            this.FM349_radioButton.Name = "FM349_radioButton";
            this.FM349_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM349_radioButton.TabIndex = 3;
            this.FM349_radioButton.Text = "349";
            this.FM349_radioButton.UseVisualStyleBackColor = true;
            this.FM349_radioButton.CheckedChanged += new System.EventHandler(this.FM349_radioButton_CheckedChanged);
            // 
            // FM309_radioButton
            // 
            this.FM309_radioButton.AutoSize = true;
            this.FM309_radioButton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FM309_radioButton.Location = new System.Drawing.Point(4, 17);
            this.FM309_radioButton.Name = "FM309_radioButton";
            this.FM309_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM309_radioButton.TabIndex = 2;
            this.FM309_radioButton.Text = "309";
            this.FM309_radioButton.UseVisualStyleBackColor = true;
            // 
            // FM336_radioButton
            // 
            this.FM336_radioButton.AutoSize = true;
            this.FM336_radioButton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FM336_radioButton.Location = new System.Drawing.Point(4, 33);
            this.FM336_radioButton.Name = "FM336_radioButton";
            this.FM336_radioButton.Size = new System.Drawing.Size(46, 18);
            this.FM336_radioButton.TabIndex = 1;
            this.FM336_radioButton.Text = "336";
            this.FM336_radioButton.UseVisualStyleBackColor = true;
            this.FM336_radioButton.CheckedChanged += new System.EventHandler(this.FM336_radioButton_CheckedChanged);
            // 
            // groupBox_chips
            // 
            this.groupBox_chips.Controls.Add(this.comboBox_ChipName);
            this.groupBox_chips.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_chips.Location = new System.Drawing.Point(74, 35);
            this.groupBox_chips.Name = "groupBox_chips";
            this.groupBox_chips.Size = new System.Drawing.Size(83, 42);
            this.groupBox_chips.TabIndex = 78;
            this.groupBox_chips.TabStop = false;
            this.groupBox_chips.Text = "项目";
            // 
            // comboBox_ChipName
            // 
            this.comboBox_ChipName.FormattingEnabled = true;
            this.comboBox_ChipName.Items.AddRange(new object[] {
            "FM309",
            "FM326",
            "FM336",
            "FM349",
            "FM350",
            "FM294",
            "FM274",
            "FM302",
            "FM347"});
            this.comboBox_ChipName.Location = new System.Drawing.Point(8, 17);
            this.comboBox_ChipName.Name = "comboBox_ChipName";
            this.comboBox_ChipName.Size = new System.Drawing.Size(71, 22);
            this.comboBox_ChipName.TabIndex = 75;
            this.comboBox_ChipName.Text = "请选择";
            this.comboBox_ChipName.SelectedIndexChanged += new System.EventHandler(this.comboBox_ChipName_SelectedIndexChanged);
            // 
            // label72
            // 
            this.label72.AutoSize = true;
            this.label72.Location = new System.Drawing.Point(241, 317);
            this.label72.Name = "label72";
            this.label72.Size = new System.Drawing.Size(41, 12);
            this.label72.TabIndex = 73;
            this.label72.Text = "写时间";
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Location = new System.Drawing.Point(199, 317);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(41, 12);
            this.label71.TabIndex = 72;
            this.label71.Text = "擦时间";
            // 
            // WR_TIME
            // 
            this.WR_TIME.Location = new System.Drawing.Point(242, 331);
            this.WR_TIME.Name = "WR_TIME";
            this.WR_TIME.Size = new System.Drawing.Size(38, 21);
            this.WR_TIME.TabIndex = 71;
            this.WR_TIME.Text = "E7";
            this.WR_TIME.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // ER_TIME
            // 
            this.ER_TIME.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ER_TIME.Location = new System.Drawing.Point(200, 331);
            this.ER_TIME.Name = "ER_TIME";
            this.ER_TIME.Size = new System.Drawing.Size(36, 21);
            this.ER_TIME.TabIndex = 70;
            this.ER_TIME.Text = "07";
            this.ER_TIME.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(205, 188);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(77, 12);
            this.label69.TabIndex = 69;
            this.label69.Text = "偏移地址：0x";
            // 
            // ProgEEOffsetAddr
            // 
            this.ProgEEOffsetAddr.Location = new System.Drawing.Point(287, 185);
            this.ProgEEOffsetAddr.MaxLength = 6;
            this.ProgEEOffsetAddr.Name = "ProgEEOffsetAddr";
            this.ProgEEOffsetAddr.Size = new System.Drawing.Size(51, 21);
            this.ProgEEOffsetAddr.TabIndex = 68;
            this.ProgEEOffsetAddr.Text = "000000";
            this.ProgEEOffsetAddr.TextChanged += new System.EventHandler(this.ProgEEOffsetAddr_TextChanged);
            this.ProgEEOffsetAddr.Leave += new System.EventHandler(this.ProgEEOffsetAddr_Leave);
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(298, 67);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(17, 12);
            this.label67.TabIndex = 66;
            this.label67.Text = "0x";
            this.label67.Visible = false;
            // 
            // groupBox_350_Cfg
            // 
            this.groupBox_350_Cfg.Controls.Add(this.label78);
            this.groupBox_350_Cfg.Controls.Add(this.label_terase_Calc);
            this.groupBox_350_Cfg.Controls.Add(this.label_terase_unit);
            this.groupBox_350_Cfg.Controls.Add(this.label_prepg_Calc);
            this.groupBox_350_Cfg.Controls.Add(this.label76);
            this.groupBox_350_Cfg.Controls.Add(this.label_tprog_Calc);
            this.groupBox_350_Cfg.Controls.Add(this.textBox_terase);
            this.groupBox_350_Cfg.Controls.Add(this.textBox_prepg);
            this.groupBox_350_Cfg.Controls.Add(this.label_terase);
            this.groupBox_350_Cfg.Controls.Add(this.label_tprog_unit);
            this.groupBox_350_Cfg.Controls.Add(this.label_tprog);
            this.groupBox_350_Cfg.Controls.Add(this.textBox_tprog);
            this.groupBox_350_Cfg.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_350_Cfg.Location = new System.Drawing.Point(198, 208);
            this.groupBox_350_Cfg.Name = "groupBox_350_Cfg";
            this.groupBox_350_Cfg.Size = new System.Drawing.Size(140, 93);
            this.groupBox_350_Cfg.TabIndex = 5;
            this.groupBox_350_Cfg.TabStop = false;
            this.groupBox_350_Cfg.Text = "擦写时间配置";
            this.groupBox_350_Cfg.Visible = false;
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Location = new System.Drawing.Point(122, 70);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(17, 12);
            this.label78.TabIndex = 90;
            this.label78.Text = "us";
            // 
            // label_terase_Calc
            // 
            this.label_terase_Calc.AutoSize = true;
            this.label_terase_Calc.Location = new System.Drawing.Point(81, 47);
            this.label_terase_Calc.Name = "label_terase_Calc";
            this.label_terase_Calc.Size = new System.Drawing.Size(41, 12);
            this.label_terase_Calc.TabIndex = 85;
            this.label_terase_Calc.Text = "*0.064";
            // 
            // label_terase_unit
            // 
            this.label_terase_unit.AutoSize = true;
            this.label_terase_unit.Location = new System.Drawing.Point(121, 46);
            this.label_terase_unit.Name = "label_terase_unit";
            this.label_terase_unit.Size = new System.Drawing.Size(17, 12);
            this.label_terase_unit.TabIndex = 86;
            this.label_terase_unit.Text = "ms";
            // 
            // label_prepg_Calc
            // 
            this.label_prepg_Calc.AutoSize = true;
            this.label_prepg_Calc.Location = new System.Drawing.Point(81, 71);
            this.label_prepg_Calc.Name = "label_prepg_Calc";
            this.label_prepg_Calc.Size = new System.Drawing.Size(41, 12);
            this.label_prepg_Calc.TabIndex = 89;
            this.label_prepg_Calc.Text = "*0.125";
            // 
            // label76
            // 
            this.label76.AutoSize = true;
            this.label76.Location = new System.Drawing.Point(5, 70);
            this.label76.Name = "label76";
            this.label76.Size = new System.Drawing.Size(53, 12);
            this.label76.TabIndex = 88;
            this.label76.Text = "prepg 0x";
            // 
            // label_tprog_Calc
            // 
            this.label_tprog_Calc.AutoSize = true;
            this.label_tprog_Calc.Location = new System.Drawing.Point(81, 23);
            this.label_tprog_Calc.Name = "label_tprog_Calc";
            this.label_tprog_Calc.Size = new System.Drawing.Size(41, 12);
            this.label_tprog_Calc.TabIndex = 81;
            this.label_tprog_Calc.Text = "*0.125";
            // 
            // textBox_terase
            // 
            this.textBox_terase.Location = new System.Drawing.Point(60, 43);
            this.textBox_terase.Name = "textBox_terase";
            this.textBox_terase.Size = new System.Drawing.Size(20, 21);
            this.textBox_terase.TabIndex = 84;
            this.textBox_terase.Text = "10";
            this.textBox_terase.TextChanged += new System.EventHandler(this.textBox_terase_TextChanged);
            this.textBox_terase.Leave += new System.EventHandler(this.textBox_terase_Leave);
            // 
            // textBox_prepg
            // 
            this.textBox_prepg.Location = new System.Drawing.Point(60, 67);
            this.textBox_prepg.Name = "textBox_prepg";
            this.textBox_prepg.Size = new System.Drawing.Size(20, 21);
            this.textBox_prepg.TabIndex = 87;
            this.textBox_prepg.Text = "0D";
            this.textBox_prepg.TextChanged += new System.EventHandler(this.textBox_prepg_TextChanged);
            // 
            // label_terase
            // 
            this.label_terase.AutoSize = true;
            this.label_terase.Location = new System.Drawing.Point(2, 46);
            this.label_terase.Name = "label_terase";
            this.label_terase.Size = new System.Drawing.Size(59, 12);
            this.label_terase.TabIndex = 83;
            this.label_terase.Text = "terase 0x";
            // 
            // label_tprog_unit
            // 
            this.label_tprog_unit.AutoSize = true;
            this.label_tprog_unit.Location = new System.Drawing.Point(121, 23);
            this.label_tprog_unit.Name = "label_tprog_unit";
            this.label_tprog_unit.Size = new System.Drawing.Size(17, 12);
            this.label_tprog_unit.TabIndex = 82;
            this.label_tprog_unit.Text = "us";
            // 
            // label_tprog
            // 
            this.label_tprog.AutoSize = true;
            this.label_tprog.Location = new System.Drawing.Point(2, 22);
            this.label_tprog.Name = "label_tprog";
            this.label_tprog.Size = new System.Drawing.Size(53, 12);
            this.label_tprog.TabIndex = 80;
            this.label_tprog.Text = "tprog 0x";
            // 
            // textBox_tprog
            // 
            this.textBox_tprog.Location = new System.Drawing.Point(60, 19);
            this.textBox_tprog.Name = "textBox_tprog";
            this.textBox_tprog.Size = new System.Drawing.Size(20, 21);
            this.textBox_tprog.TabIndex = 79;
            this.textBox_tprog.Text = "1C";
            this.textBox_tprog.TextChanged += new System.EventHandler(this.textBox_tprog_TextChanged);
            this.textBox_tprog.Leave += new System.EventHandler(this.textBox_tprog_Leave);
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(305, 50);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(35, 12);
            this.label68.TabIndex = 65;
            this.label68.Text = "Patch";
            this.label68.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(314, 64);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(27, 21);
            this.textBox1.TabIndex = 64;
            this.textBox1.Text = "10";
            this.textBox1.Visible = false;
            // 
            // EE_patch_checkBox
            // 
            this.EE_patch_checkBox.AutoSize = true;
            this.EE_patch_checkBox.Enabled = false;
            this.EE_patch_checkBox.Location = new System.Drawing.Point(210, 66);
            this.EE_patch_checkBox.Name = "EE_patch_checkBox";
            this.EE_patch_checkBox.Size = new System.Drawing.Size(96, 16);
            this.EE_patch_checkBox.TabIndex = 63;
            this.EE_patch_checkBox.Text = "补丁正反校验";
            this.EE_patch_checkBox.UseVisualStyleBackColor = true;
            this.EE_patch_checkBox.CheckedChanged += new System.EventHandler(this.EE_patch_checkBox_CheckedChanged);
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.radioButton_0xFF);
            this.groupBox13.Controls.Add(this.radioButton_0x00);
            this.groupBox13.Controls.Add(this.radioButton_0xB9);
            this.groupBox13.Location = new System.Drawing.Point(5, 80);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(74, 67);
            this.groupBox13.TabIndex = 62;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "背景数据";
            // 
            // radioButton_0xFF
            // 
            this.radioButton_0xFF.AutoSize = true;
            this.radioButton_0xFF.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton_0xFF.Location = new System.Drawing.Point(9, 46);
            this.radioButton_0xFF.Name = "radioButton_0xFF";
            this.radioButton_0xFF.Size = new System.Drawing.Size(53, 18);
            this.radioButton_0xFF.TabIndex = 3;
            this.radioButton_0xFF.Text = "0xFF";
            this.radioButton_0xFF.UseVisualStyleBackColor = true;
            // 
            // radioButton_0x00
            // 
            this.radioButton_0x00.AutoSize = true;
            this.radioButton_0x00.Checked = true;
            this.radioButton_0x00.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton_0x00.Location = new System.Drawing.Point(9, 14);
            this.radioButton_0x00.Name = "radioButton_0x00";
            this.radioButton_0x00.Size = new System.Drawing.Size(53, 18);
            this.radioButton_0x00.TabIndex = 2;
            this.radioButton_0x00.TabStop = true;
            this.radioButton_0x00.Text = "0x00";
            this.radioButton_0x00.UseVisualStyleBackColor = true;
            // 
            // radioButton_0xB9
            // 
            this.radioButton_0xB9.AutoSize = true;
            this.radioButton_0xB9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton_0xB9.Location = new System.Drawing.Point(9, 30);
            this.radioButton_0xB9.Name = "radioButton_0xB9";
            this.radioButton_0xB9.Size = new System.Drawing.Size(53, 18);
            this.radioButton_0xB9.TabIndex = 1;
            this.radioButton_0xB9.Text = "0xB9";
            this.radioButton_0xB9.UseVisualStyleBackColor = true;
            // 
            // flash_check
            // 
            this.flash_check.AutoSize = true;
            this.flash_check.BackColor = System.Drawing.SystemColors.Control;
            this.flash_check.Checked = true;
            this.flash_check.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flash_check.Location = new System.Drawing.Point(210, 49);
            this.flash_check.Name = "flash_check";
            this.flash_check.Size = new System.Drawing.Size(78, 16);
            this.flash_check.TabIndex = 61;
            this.flash_check.Text = "flash校验";
            this.flash_check.UseVisualStyleBackColor = false;
            this.flash_check.CheckedChanged += new System.EventHandler(this.checkBox10_CheckedChanged);
            // 
            // BufMove_checkBox
            // 
            this.BufMove_checkBox.AutoSize = true;
            this.BufMove_checkBox.Location = new System.Drawing.Point(210, 83);
            this.BufMove_checkBox.Name = "BufMove_checkBox";
            this.BufMove_checkBox.Size = new System.Drawing.Size(84, 16);
            this.BufMove_checkBox.TabIndex = 60;
            this.BufMove_checkBox.Text = "缓冲区搬移";
            this.BufMove_checkBox.UseVisualStyleBackColor = true;
            this.BufMove_checkBox.CheckedChanged += new System.EventHandler(this.BufMove_checkBox_CheckedChanged);
            // 
            // checkBox_CTHighBaud
            // 
            this.checkBox_CTHighBaud.AutoSize = true;
            this.checkBox_CTHighBaud.Enabled = false;
            this.checkBox_CTHighBaud.Location = new System.Drawing.Point(128, 197);
            this.checkBox_CTHighBaud.Name = "checkBox_CTHighBaud";
            this.checkBox_CTHighBaud.Size = new System.Drawing.Size(60, 16);
            this.checkBox_CTHighBaud.TabIndex = 59;
            this.checkBox_CTHighBaud.Text = "CT高速";
            this.checkBox_CTHighBaud.UseVisualStyleBackColor = true;
            this.checkBox_CTHighBaud.Visible = false;
            this.checkBox_CTHighBaud.CheckedChanged += new System.EventHandler(this.checkBox_CTHighBaud_CheckedChanged);
            // 
            // Prog_Move
            // 
            this.Prog_Move.Location = new System.Drawing.Point(301, 87);
            this.Prog_Move.Name = "Prog_Move";
            this.Prog_Move.Size = new System.Drawing.Size(38, 23);
            this.Prog_Move.TabIndex = 58;
            this.Prog_Move.Text = "搬移";
            this.Prog_Move.UseVisualStyleBackColor = true;
            this.Prog_Move.Visible = false;
            this.Prog_Move.Click += new System.EventHandler(this.Prog_Move_Click);
            // 
            // Card_init_Button
            // 
            this.Card_init_Button.Location = new System.Drawing.Point(284, 323);
            this.Card_init_Button.Name = "Card_init_Button";
            this.Card_init_Button.Size = new System.Drawing.Size(57, 24);
            this.Card_init_Button.TabIndex = 46;
            this.Card_init_Button.Text = "银检COS";
            this.Card_init_Button.UseVisualStyleBackColor = true;
            this.Card_init_Button.Click += new System.EventHandler(this.Card_init_Button_Click);
            // 
            // dest_label1
            // 
            this.dest_label1.AutoSize = true;
            this.dest_label1.Location = new System.Drawing.Point(205, 121);
            this.dest_label1.Name = "dest_label1";
            this.dest_label1.Size = new System.Drawing.Size(77, 12);
            this.dest_label1.TabIndex = 56;
            this.dest_label1.Text = "目标地址：0x";
            this.dest_label1.Visible = false;
            // 
            // ProgDestAddr
            // 
            this.ProgDestAddr.Location = new System.Drawing.Point(287, 118);
            this.ProgDestAddr.Name = "ProgDestAddr";
            this.ProgDestAddr.Size = new System.Drawing.Size(51, 21);
            this.ProgDestAddr.TabIndex = 55;
            this.ProgDestAddr.Text = "000000";
            this.ProgDestAddr.Visible = false;
            this.ProgDestAddr.TextChanged += new System.EventHandler(this.ProgDestAddr_TextChanged);
            this.ProgDestAddr.Leave += new System.EventHandler(this.ProgDestAddr_Leave);
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label64.Location = new System.Drawing.Point(5, 361);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(113, 12);
            this.label64.TabIndex = 54;
            this.label64.Text = "初始化指令认证密钥";
            this.label64.Visible = false;
            // 
            // key_richTextBox
            // 
            this.key_richTextBox.Location = new System.Drawing.Point(122, 357);
            this.key_richTextBox.Name = "key_richTextBox";
            this.key_richTextBox.Size = new System.Drawing.Size(206, 21);
            this.key_richTextBox.TabIndex = 53;
            this.key_richTextBox.Text = "00000000000000000000000000000000";
            this.key_richTextBox.Visible = false;
            this.key_richTextBox.TextChanged += new System.EventHandler(this.key_richTextBox_TextChanged);
            // 
            // keyl_label
            // 
            this.keyl_label.AutoSize = true;
            this.keyl_label.Location = new System.Drawing.Point(324, 357);
            this.keyl_label.Name = "keyl_label";
            this.keyl_label.Size = new System.Drawing.Size(17, 12);
            this.keyl_label.TabIndex = 52;
            this.keyl_label.Text = "16";
            this.keyl_label.Visible = false;
            // 
            // InitData_comboBox
            // 
            this.InitData_comboBox.FormattingEnabled = true;
            this.InitData_comboBox.Items.AddRange(new object[] {
            "00",
            "FF",
            "55",
            "AA",
            "升序",
            "降序",
            "伪随机-奇",
            "伪随机-偶",
            "B9",
            "80FE"});
            this.InitData_comboBox.Location = new System.Drawing.Point(95, 326);
            this.InitData_comboBox.Name = "InitData_comboBox";
            this.InitData_comboBox.Size = new System.Drawing.Size(47, 20);
            this.InitData_comboBox.TabIndex = 50;
            this.InitData_comboBox.Text = "00";
            // 
            // Auth_checkBox
            // 
            this.Auth_checkBox.AutoSize = true;
            this.Auth_checkBox.Location = new System.Drawing.Point(128, 167);
            this.Auth_checkBox.Name = "Auth_checkBox";
            this.Auth_checkBox.Size = new System.Drawing.Size(48, 16);
            this.Auth_checkBox.TabIndex = 49;
            this.Auth_checkBox.Text = "认证";
            this.Auth_checkBox.UseVisualStyleBackColor = true;
            this.Auth_checkBox.CheckedChanged += new System.EventHandler(this.Auth_checkBox_CheckedChanged);
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Enabled = false;
            this.label61.Location = new System.Drawing.Point(332, 10);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(11, 12);
            this.label61.TabIndex = 45;
            this.label61.Text = "K";
            // 
            // lib_Size
            // 
            this.lib_Size.Enabled = false;
            this.lib_Size.Location = new System.Drawing.Point(313, 26);
            this.lib_Size.Name = "lib_Size";
            this.lib_Size.Size = new System.Drawing.Size(20, 21);
            this.lib_Size.TabIndex = 44;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Enabled = false;
            this.label60.Location = new System.Drawing.Point(289, 29);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(23, 12);
            this.label60.TabIndex = 43;
            this.label60.Text = "Lib";
            // 
            // flash_isp
            // 
            this.flash_isp.AutoSize = true;
            this.flash_isp.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flash_isp.Location = new System.Drawing.Point(210, 33);
            this.flash_isp.Name = "flash_isp";
            this.flash_isp.Size = new System.Drawing.Size(78, 16);
            this.flash_isp.TabIndex = 42;
            this.flash_isp.Text = "flash编程";
            this.Hints_toolTip.SetToolTip(this.flash_isp, "用于FM309验证板上flash的编程");
            this.flash_isp.UseVisualStyleBackColor = true;
            this.flash_isp.CheckedChanged += new System.EventHandler(this.flash_isp_CheckedChanged);
            // 
            // MEM_EE
            // 
            this.MEM_EE.AutoSize = true;
            this.MEM_EE.Enabled = false;
            this.MEM_EE.Location = new System.Drawing.Point(86, 19);
            this.MEM_EE.Name = "MEM_EE";
            this.MEM_EE.Size = new System.Drawing.Size(60, 16);
            this.MEM_EE.TabIndex = 38;
            this.MEM_EE.Text = "EE存储";
            this.MEM_EE.UseVisualStyleBackColor = true;
            // 
            // MEM_ROM
            // 
            this.MEM_ROM.AutoSize = true;
            this.MEM_ROM.Enabled = false;
            this.MEM_ROM.Location = new System.Drawing.Point(86, 3);
            this.MEM_ROM.Name = "MEM_ROM";
            this.MEM_ROM.Size = new System.Drawing.Size(66, 16);
            this.MEM_ROM.TabIndex = 37;
            this.MEM_ROM.Text = "ROM存储";
            this.MEM_ROM.UseVisualStyleBackColor = true;
            // 
            // SaveAs_verify
            // 
            this.SaveAs_verify.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SaveAs_verify.Location = new System.Drawing.Point(12, 295);
            this.SaveAs_verify.Name = "SaveAs_verify";
            this.SaveAs_verify.Size = new System.Drawing.Size(46, 25);
            this.SaveAs_verify.TabIndex = 36;
            this.SaveAs_verify.Text = "CRC16";
            this.SaveAs_verify.UseVisualStyleBackColor = true;
            this.SaveAs_verify.Click += new System.EventHandler(this.SaveAs_verify_Button_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(145, 328);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(43, 21);
            this.button4.TabIndex = 35;
            this.button4.Text = "全擦";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // prog_extand_mode1
            // 
            this.prog_extand_mode1.AutoSize = true;
            this.prog_extand_mode1.Enabled = false;
            this.prog_extand_mode1.Location = new System.Drawing.Point(192, 19);
            this.prog_extand_mode1.Name = "prog_extand_mode1";
            this.prog_extand_mode1.Size = new System.Drawing.Size(72, 16);
            this.prog_extand_mode1.TabIndex = 34;
            this.prog_extand_mode1.Text = "段后扩展";
            this.prog_extand_mode1.UseVisualStyleBackColor = true;
            // 
            // prog_extand_memRAM
            // 
            this.prog_extand_memRAM.AutoSize = true;
            this.prog_extand_memRAM.Enabled = false;
            this.prog_extand_memRAM.Location = new System.Drawing.Point(152, 19);
            this.prog_extand_memRAM.Name = "prog_extand_memRAM";
            this.prog_extand_memRAM.Size = new System.Drawing.Size(42, 16);
            this.prog_extand_memRAM.TabIndex = 34;
            this.prog_extand_memRAM.Text = "RAM";
            this.prog_extand_memRAM.UseVisualStyleBackColor = true;
            // 
            // prog_extand_memEE
            // 
            this.prog_extand_memEE.AutoSize = true;
            this.prog_extand_memEE.Enabled = false;
            this.prog_extand_memEE.Location = new System.Drawing.Point(152, 3);
            this.prog_extand_memEE.Name = "prog_extand_memEE";
            this.prog_extand_memEE.Size = new System.Drawing.Size(36, 16);
            this.prog_extand_memEE.TabIndex = 34;
            this.prog_extand_memEE.Text = "EE";
            this.prog_extand_memEE.UseVisualStyleBackColor = true;
            // 
            // prog_extand_mode0
            // 
            this.prog_extand_mode0.AutoSize = true;
            this.prog_extand_mode0.Enabled = false;
            this.prog_extand_mode0.Location = new System.Drawing.Point(192, 3);
            this.prog_extand_mode0.Name = "prog_extand_mode0";
            this.prog_extand_mode0.Size = new System.Drawing.Size(72, 16);
            this.prog_extand_mode0.TabIndex = 34;
            this.prog_extand_mode0.Text = "整体扩展";
            this.prog_extand_mode0.UseVisualStyleBackColor = true;
            // 
            // SaveAsStart
            // 
            this.SaveAsStart.Location = new System.Drawing.Point(83, 280);
            this.SaveAsStart.Name = "SaveAsStart";
            this.SaveAsStart.Size = new System.Drawing.Size(43, 21);
            this.SaveAsStart.TabIndex = 31;
            this.SaveAsStart.Text = "000000";
            // 
            // SaveAsEnd
            // 
            this.SaveAsEnd.Location = new System.Drawing.Point(143, 280);
            this.SaveAsEnd.Name = "SaveAsEnd";
            this.SaveAsEnd.Size = new System.Drawing.Size(43, 21);
            this.SaveAsEnd.TabIndex = 29;
            this.SaveAsEnd.Text = "0001FF";
            // 
            // SaveAS_Button
            // 
            this.SaveAS_Button.Location = new System.Drawing.Point(12, 271);
            this.SaveAS_Button.Name = "SaveAS_Button";
            this.SaveAS_Button.Size = new System.Drawing.Size(46, 22);
            this.SaveAS_Button.TabIndex = 28;
            this.SaveAS_Button.Text = "另存";
            this.SaveAS_Button.UseVisualStyleBackColor = true;
            this.SaveAS_Button.Click += new System.EventHandler(this.SaveAS_Button_Click);
            // 
            // A1Program
            // 
            this.A1Program.AutoSize = true;
            this.A1Program.Checked = true;
            this.A1Program.CheckState = System.Windows.Forms.CheckState.Checked;
            this.A1Program.Location = new System.Drawing.Point(128, 182);
            this.A1Program.Name = "A1Program";
            this.A1Program.Size = new System.Drawing.Size(36, 16);
            this.A1Program.TabIndex = 24;
            this.A1Program.Text = "A1";
            this.A1Program.UseVisualStyleBackColor = true;
            // 
            // ProgEE_progressBar
            // 
            this.ProgEE_progressBar.Location = new System.Drawing.Point(7, 379);
            this.ProgEE_progressBar.Name = "ProgEE_progressBar";
            this.ProgEE_progressBar.Size = new System.Drawing.Size(321, 22);
            this.ProgEE_progressBar.TabIndex = 23;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(205, 166);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(77, 12);
            this.label47.TabIndex = 20;
            this.label47.Text = "结束地址：0x";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Enabled = false;
            this.label55.Location = new System.Drawing.Point(332, 30);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(11, 12);
            this.label55.TabIndex = 19;
            this.label55.Text = "K";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Enabled = false;
            this.label52.Location = new System.Drawing.Point(279, 10);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(35, 12);
            this.label52.TabIndex = 19;
            this.label52.Text = "Patch";
            // 
            // Patch_Size
            // 
            this.Patch_Size.Enabled = false;
            this.Patch_Size.Location = new System.Drawing.Point(314, 4);
            this.Patch_Size.Name = "Patch_Size";
            this.Patch_Size.Size = new System.Drawing.Size(20, 21);
            this.Patch_Size.TabIndex = 18;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(205, 144);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(77, 12);
            this.label46.TabIndex = 19;
            this.label46.Text = "起始地址：0x";
            // 
            // ProgEEStartAddr
            // 
            this.ProgEEStartAddr.Location = new System.Drawing.Point(287, 140);
            this.ProgEEStartAddr.Name = "ProgEEStartAddr";
            this.ProgEEStartAddr.Size = new System.Drawing.Size(51, 21);
            this.ProgEEStartAddr.TabIndex = 18;
            this.ProgEEStartAddr.Text = "000000";
            this.ProgEEStartAddr.TextChanged += new System.EventHandler(this.ProgEEStartAddr_TextChanged);
            this.ProgEEStartAddr.Leave += new System.EventHandler(this.ProgEEStartAddr_Leave);
            // 
            // ProgEEEndAddr
            // 
            this.ProgEEEndAddr.Location = new System.Drawing.Point(287, 162);
            this.ProgEEEndAddr.Name = "ProgEEEndAddr";
            this.ProgEEEndAddr.Size = new System.Drawing.Size(51, 21);
            this.ProgEEEndAddr.TabIndex = 17;
            this.ProgEEEndAddr.Text = "03FFFF";
            this.ProgEEEndAddr.TextChanged += new System.EventHandler(this.ProgEEEndAddr_TextChanged);
            this.ProgEEEndAddr.Leave += new System.EventHandler(this.ProgEEEndAddr_Leave);
            // 
            // InitEEdata_Button
            // 
            this.InitEEdata_Button.Location = new System.Drawing.Point(12, 321);
            this.InitEEdata_Button.Name = "InitEEdata_Button";
            this.InitEEdata_Button.Size = new System.Drawing.Size(78, 28);
            this.InitEEdata_Button.TabIndex = 7;
            this.InitEEdata_Button.Text = "初始缓冲区";
            this.InitEEdata_Button.UseVisualStyleBackColor = true;
            this.InitEEdata_Button.Click += new System.EventHandler(this.InitEEdata_Button_Click);
            // 
            // ProgEE_Button
            // 
            this.ProgEE_Button.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ProgEE_Button.Location = new System.Drawing.Point(59, 171);
            this.ProgEE_Button.Name = "ProgEE_Button";
            this.ProgEE_Button.Size = new System.Drawing.Size(62, 44);
            this.ProgEE_Button.TabIndex = 6;
            this.ProgEE_Button.Text = "编程";
            this.ProgEE_Button.UseVisualStyleBackColor = true;
            this.ProgEE_Button.Click += new System.EventHandler(this.ProgEE_Button_Click);
            // 
            // OpenFile_Button
            // 
            this.OpenFile_Button.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.OpenFile_Button.Location = new System.Drawing.Point(12, 171);
            this.OpenFile_Button.Name = "OpenFile_Button";
            this.OpenFile_Button.Size = new System.Drawing.Size(46, 35);
            this.OpenFile_Button.TabIndex = 5;
            this.OpenFile_Button.Text = "打开文件";
            this.OpenFile_Button.UseVisualStyleBackColor = true;
            this.OpenFile_Button.Click += new System.EventHandler(this.OpenFile_Button_Click);
            // 
            // MemTypeSelGrop
            // 
            this.MemTypeSelGrop.Controls.Add(this.RAMtype_radiobutton);
            this.MemTypeSelGrop.Controls.Add(this.PROG_EEtype_radiobutton);
            this.MemTypeSelGrop.Controls.Add(this.DATA_EEtype_radiobutton);
            this.MemTypeSelGrop.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MemTypeSelGrop.Location = new System.Drawing.Point(5, 3);
            this.MemTypeSelGrop.Name = "MemTypeSelGrop";
            this.MemTypeSelGrop.Size = new System.Drawing.Size(69, 75);
            this.MemTypeSelGrop.TabIndex = 3;
            this.MemTypeSelGrop.TabStop = false;
            this.MemTypeSelGrop.Text = "NVM类型";
            // 
            // RAMtype_radiobutton
            // 
            this.RAMtype_radiobutton.AutoSize = true;
            this.RAMtype_radiobutton.Enabled = false;
            this.RAMtype_radiobutton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RAMtype_radiobutton.Location = new System.Drawing.Point(9, 51);
            this.RAMtype_radiobutton.Name = "RAMtype_radiobutton";
            this.RAMtype_radiobutton.Size = new System.Drawing.Size(46, 18);
            this.RAMtype_radiobutton.TabIndex = 3;
            this.RAMtype_radiobutton.Text = "RAM";
            this.RAMtype_radiobutton.UseVisualStyleBackColor = true;
            this.RAMtype_radiobutton.CheckedChanged += new System.EventHandler(this.RAMtype_radiobutton_CheckedChanged);
            // 
            // PROG_EEtype_radiobutton
            // 
            this.PROG_EEtype_radiobutton.AutoSize = true;
            this.PROG_EEtype_radiobutton.Checked = true;
            this.PROG_EEtype_radiobutton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PROG_EEtype_radiobutton.Location = new System.Drawing.Point(9, 16);
            this.PROG_EEtype_radiobutton.Name = "PROG_EEtype_radiobutton";
            this.PROG_EEtype_radiobutton.Size = new System.Drawing.Size(53, 18);
            this.PROG_EEtype_radiobutton.TabIndex = 2;
            this.PROG_EEtype_radiobutton.TabStop = true;
            this.PROG_EEtype_radiobutton.Text = "程序";
            this.PROG_EEtype_radiobutton.UseVisualStyleBackColor = true;
            this.PROG_EEtype_radiobutton.CheckedChanged += new System.EventHandler(this.PROG_EEtype_radiobutton_CheckedChanged);
            // 
            // DATA_EEtype_radiobutton
            // 
            this.DATA_EEtype_radiobutton.AutoSize = true;
            this.DATA_EEtype_radiobutton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DATA_EEtype_radiobutton.Location = new System.Drawing.Point(9, 34);
            this.DATA_EEtype_radiobutton.Name = "DATA_EEtype_radiobutton";
            this.DATA_EEtype_radiobutton.Size = new System.Drawing.Size(53, 18);
            this.DATA_EEtype_radiobutton.TabIndex = 1;
            this.DATA_EEtype_radiobutton.Text = "数据";
            this.DATA_EEtype_radiobutton.UseVisualStyleBackColor = true;
            this.DATA_EEtype_radiobutton.CheckedChanged += new System.EventHandler(this.DATA_EEtype_radiobutton_CheckedChanged);
            // 
            // groupBoxOperation
            // 
            this.groupBoxOperation.Controls.Add(this.textBox_LibSize);
            this.groupBoxOperation.Controls.Add(this.label_LibSizeUnit);
            this.groupBoxOperation.Controls.Add(this.checkBox_SeperateLibProg);
            this.groupBoxOperation.Controls.Add(this.comboBox_CTBaud);
            this.groupBoxOperation.Controls.Add(this.InitCRC16_texbox);
            this.groupBoxOperation.Controls.Add(this.label58);
            this.groupBoxOperation.Controls.Add(this.SaveReadbuf);
            this.groupBoxOperation.Controls.Add(this.ReadEEdata_Button);
            this.groupBoxOperation.Controls.Add(this.label54);
            this.groupBoxOperation.Controls.Add(this.label53);
            this.groupBoxOperation.Controls.Add(this.CB_NoErase);
            this.groupBoxOperation.Controls.Add(this.checkBox_350ChipErase);
            this.groupBoxOperation.Controls.Add(this.checkBox_ignore9800);
            this.groupBoxOperation.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBoxOperation.Location = new System.Drawing.Point(5, 156);
            this.groupBoxOperation.Name = "groupBoxOperation";
            this.groupBoxOperation.Size = new System.Drawing.Size(191, 196);
            this.groupBoxOperation.TabIndex = 63;
            this.groupBoxOperation.TabStop = false;
            this.groupBoxOperation.Text = "读写操作";
            // 
            // textBox_LibSize
            // 
            this.textBox_LibSize.Enabled = false;
            this.textBox_LibSize.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_LibSize.Location = new System.Drawing.Point(140, 101);
            this.textBox_LibSize.Name = "textBox_LibSize";
            this.textBox_LibSize.Size = new System.Drawing.Size(19, 21);
            this.textBox_LibSize.TabIndex = 79;
            this.textBox_LibSize.Text = "40";
            this.textBox_LibSize.Visible = false;
            this.textBox_LibSize.TextChanged += new System.EventHandler(this.textBox_LibSize_TextChanged);
            this.textBox_LibSize.Leave += new System.EventHandler(this.textBox_LibSize_Leave);
            // 
            // label_LibSizeUnit
            // 
            this.label_LibSizeUnit.AutoSize = true;
            this.label_LibSizeUnit.Enabled = false;
            this.label_LibSizeUnit.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_LibSizeUnit.Location = new System.Drawing.Point(160, 104);
            this.label_LibSizeUnit.Name = "label_LibSizeUnit";
            this.label_LibSizeUnit.Size = new System.Drawing.Size(11, 12);
            this.label_LibSizeUnit.TabIndex = 79;
            this.label_LibSizeUnit.Text = "K";
            this.label_LibSizeUnit.Visible = false;
            // 
            // checkBox_SeperateLibProg
            // 
            this.checkBox_SeperateLibProg.AutoSize = true;
            this.checkBox_SeperateLibProg.Enabled = false;
            this.checkBox_SeperateLibProg.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_SeperateLibProg.Location = new System.Drawing.Point(55, 105);
            this.checkBox_SeperateLibProg.Name = "checkBox_SeperateLibProg";
            this.checkBox_SeperateLibProg.Size = new System.Drawing.Size(84, 16);
            this.checkBox_SeperateLibProg.TabIndex = 80;
            this.checkBox_SeperateLibProg.Text = "LibSize 0x";
            this.checkBox_SeperateLibProg.UseVisualStyleBackColor = true;
            this.checkBox_SeperateLibProg.Visible = false;
            this.checkBox_SeperateLibProg.CheckedChanged += new System.EventHandler(this.checkBox_SeperateLibProg_CheckedChanged);
            // 
            // comboBox_CTBaud
            // 
            this.comboBox_CTBaud.Enabled = false;
            this.comboBox_CTBaud.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_CTBaud.FormattingEnabled = true;
            this.comboBox_CTBaud.Items.AddRange(new object[] {
            "57600",
            "115200"});
            this.comboBox_CTBaud.Location = new System.Drawing.Point(121, 57);
            this.comboBox_CTBaud.Name = "comboBox_CTBaud";
            this.comboBox_CTBaud.Size = new System.Drawing.Size(61, 20);
            this.comboBox_CTBaud.TabIndex = 75;
            this.comboBox_CTBaud.Text = "57600";
            this.comboBox_CTBaud.Visible = false;
            // 
            // InitCRC16_texbox
            // 
            this.InitCRC16_texbox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.InitCRC16_texbox.Location = new System.Drawing.Point(147, 147);
            this.InitCRC16_texbox.Name = "InitCRC16_texbox";
            this.InitCRC16_texbox.Size = new System.Drawing.Size(33, 21);
            this.InitCRC16_texbox.TabIndex = 39;
            this.InitCRC16_texbox.Text = "6363";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label58.Location = new System.Drawing.Point(77, 152);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(71, 12);
            this.label58.TabIndex = 40;
            this.label58.Text = "CRC16初值：";
            // 
            // SaveReadbuf
            // 
            this.SaveReadbuf.AutoSize = true;
            this.SaveReadbuf.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SaveReadbuf.Location = new System.Drawing.Point(55, 60);
            this.SaveReadbuf.Name = "SaveReadbuf";
            this.SaveReadbuf.Size = new System.Drawing.Size(48, 16);
            this.SaveReadbuf.TabIndex = 27;
            this.SaveReadbuf.Text = "保存";
            this.SaveReadbuf.UseVisualStyleBackColor = true;
            // 
            // ReadEEdata_Button
            // 
            this.ReadEEdata_Button.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ReadEEdata_Button.Location = new System.Drawing.Point(6, 53);
            this.ReadEEdata_Button.Name = "ReadEEdata_Button";
            this.ReadEEdata_Button.Size = new System.Drawing.Size(46, 51);
            this.ReadEEdata_Button.TabIndex = 8;
            this.ReadEEdata_Button.Text = "读取";
            this.ReadEEdata_Button.UseVisualStyleBackColor = true;
            this.ReadEEdata_Button.Click += new System.EventHandler(this.ReadEEdata_Button_Click);
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(59, 126);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(21, 14);
            this.label54.TabIndex = 32;
            this.label54.Text = "0x";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(122, 132);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(14, 14);
            this.label53.TabIndex = 33;
            this.label53.Text = "~";
            // 
            // CB_NoErase
            // 
            this.CB_NoErase.AutoSize = true;
            this.CB_NoErase.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CB_NoErase.Location = new System.Drawing.Point(123, 78);
            this.CB_NoErase.Name = "CB_NoErase";
            this.CB_NoErase.Size = new System.Drawing.Size(48, 16);
            this.CB_NoErase.TabIndex = 67;
            this.CB_NoErase.Text = "不擦";
            this.CB_NoErase.UseVisualStyleBackColor = true;
            this.CB_NoErase.Visible = false;
            // 
            // checkBox_350ChipErase
            // 
            this.checkBox_350ChipErase.AutoSize = true;
            this.checkBox_350ChipErase.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_350ChipErase.Location = new System.Drawing.Point(123, 77);
            this.checkBox_350ChipErase.Name = "checkBox_350ChipErase";
            this.checkBox_350ChipErase.Size = new System.Drawing.Size(48, 16);
            this.checkBox_350ChipErase.TabIndex = 74;
            this.checkBox_350ChipErase.Text = "全擦";
            this.checkBox_350ChipErase.UseVisualStyleBackColor = true;
            this.checkBox_350ChipErase.Visible = false;
            // 
            // checkBox_ignore9800
            // 
            this.checkBox_ignore9800.AutoSize = true;
            this.checkBox_ignore9800.Enabled = false;
            this.checkBox_ignore9800.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_ignore9800.Location = new System.Drawing.Point(55, 90);
            this.checkBox_ignore9800.Name = "checkBox_ignore9800";
            this.checkBox_ignore9800.Size = new System.Drawing.Size(72, 16);
            this.checkBox_ignore9800.TabIndex = 79;
            this.checkBox_ignore9800.Text = "忽略9800";
            this.checkBox_ignore9800.UseVisualStyleBackColor = true;
            this.checkBox_ignore9800.Visible = false;
            this.checkBox_ignore9800.CheckedChanged += new System.EventHandler(this.checkBox_ignore9800_CheckedChanged);
            // 
            // interface_sel_grpbox
            // 
            this.interface_sel_grpbox.Controls.Add(this.CL_Interface);
            this.interface_sel_grpbox.Controls.Add(this.CT_Interface);
            this.interface_sel_grpbox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.interface_sel_grpbox.Location = new System.Drawing.Point(160, 35);
            this.interface_sel_grpbox.Name = "interface_sel_grpbox";
            this.interface_sel_grpbox.Size = new System.Drawing.Size(46, 48);
            this.interface_sel_grpbox.TabIndex = 4;
            this.interface_sel_grpbox.TabStop = false;
            this.interface_sel_grpbox.Text = "接口";
            // 
            // CL_Interface
            // 
            this.CL_Interface.AutoSize = true;
            this.CL_Interface.Checked = true;
            this.CL_Interface.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CL_Interface.Location = new System.Drawing.Point(6, 12);
            this.CL_Interface.Name = "CL_Interface";
            this.CL_Interface.Size = new System.Drawing.Size(39, 18);
            this.CL_Interface.TabIndex = 2;
            this.CL_Interface.TabStop = true;
            this.CL_Interface.Text = "CL";
            this.CL_Interface.UseVisualStyleBackColor = true;
            this.CL_Interface.CheckedChanged += new System.EventHandler(this.CL_Interface_CheckedChanged);
            // 
            // CT_Interface
            // 
            this.CT_Interface.AutoSize = true;
            this.CT_Interface.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CT_Interface.Location = new System.Drawing.Point(6, 28);
            this.CT_Interface.Name = "CT_Interface";
            this.CT_Interface.Size = new System.Drawing.Size(39, 18);
            this.CT_Interface.TabIndex = 1;
            this.CT_Interface.Text = "CT";
            this.CT_Interface.UseVisualStyleBackColor = true;
            this.CT_Interface.CheckedChanged += new System.EventHandler(this.CT_Interface_CheckedChanged);
            // 
            // progEEdataGridView
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.progEEdataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.progEEdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.progEEdataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EE_Addr,
            this.EE_Data});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.progEEdataGridView.DefaultCellStyle = dataGridViewCellStyle5;
            this.progEEdataGridView.Location = new System.Drawing.Point(345, 9);
            this.progEEdataGridView.Name = "progEEdataGridView";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.progEEdataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.progEEdataGridView.RowTemplate.Height = 23;
            this.progEEdataGridView.Size = new System.Drawing.Size(611, 395);
            this.progEEdataGridView.TabIndex = 0;
            this.progEEdataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.progEEdataGridView_CellEndEdit);
            // 
            // EE_Addr
            // 
            this.EE_Addr.HeaderText = "地址";
            this.EE_Addr.Name = "EE_Addr";
            this.EE_Addr.Width = 85;
            // 
            // EE_Data
            // 
            this.EE_Data.HeaderText = "数据";
            this.EE_Data.Name = "EE_Data";
            this.EE_Data.Width = 468;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.Trim_init);
            this.groupBox7.Controls.Add(this.Chip_comboBox);
            this.groupBox7.Controls.Add(this.FreqRead_radioButton);
            this.groupBox7.Controls.Add(this.FreqTrim_radioButton);
            this.groupBox7.Controls.Add(this.DfsCFG_comboBox1);
            this.groupBox7.Controls.Add(this.freq_cos_check);
            this.groupBox7.Controls.Add(this.Freq_trim_button);
            this.groupBox7.Location = new System.Drawing.Point(196, 205);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(141, 109);
            this.groupBox7.TabIndex = 48;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Trim or Read";
            // 
            // Trim_init
            // 
            this.Trim_init.AutoSize = true;
            this.Trim_init.Location = new System.Drawing.Point(10, 42);
            this.Trim_init.Name = "Trim_init";
            this.Trim_init.Size = new System.Drawing.Size(78, 16);
            this.Trim_init.TabIndex = 74;
            this.Trim_init.Text = "Trim_init";
            this.Trim_init.UseVisualStyleBackColor = true;
            // 
            // Chip_comboBox
            // 
            this.Chip_comboBox.FormattingEnabled = true;
            this.Chip_comboBox.Items.AddRange(new object[] {
            "FM309",
            "FM326",
            "FM336"});
            this.Chip_comboBox.Location = new System.Drawing.Point(63, 59);
            this.Chip_comboBox.Name = "Chip_comboBox";
            this.Chip_comboBox.Size = new System.Drawing.Size(53, 20);
            this.Chip_comboBox.TabIndex = 57;
            this.Chip_comboBox.Text = "FM309";
            this.Chip_comboBox.SelectedIndexChanged += new System.EventHandler(this.Chip_comboBox_SelectedIndexChanged);
            // 
            // FreqRead_radioButton
            // 
            this.FreqRead_radioButton.AutoSize = true;
            this.FreqRead_radioButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FreqRead_radioButton.Location = new System.Drawing.Point(92, 36);
            this.FreqRead_radioButton.Name = "FreqRead_radioButton";
            this.FreqRead_radioButton.Size = new System.Drawing.Size(47, 16);
            this.FreqRead_radioButton.TabIndex = 56;
            this.FreqRead_radioButton.Text = "读取";
            this.FreqRead_radioButton.UseVisualStyleBackColor = true;
            // 
            // FreqTrim_radioButton
            // 
            this.FreqTrim_radioButton.AutoSize = true;
            this.FreqTrim_radioButton.Checked = true;
            this.FreqTrim_radioButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FreqTrim_radioButton.Location = new System.Drawing.Point(92, 16);
            this.FreqTrim_radioButton.Name = "FreqTrim_radioButton";
            this.FreqTrim_radioButton.Size = new System.Drawing.Size(47, 16);
            this.FreqTrim_radioButton.TabIndex = 3;
            this.FreqTrim_radioButton.TabStop = true;
            this.FreqTrim_radioButton.Text = "校准";
            this.FreqTrim_radioButton.UseVisualStyleBackColor = true;
            // 
            // DfsCFG_comboBox1
            // 
            this.DfsCFG_comboBox1.FormattingEnabled = true;
            this.DfsCFG_comboBox1.Items.AddRange(new object[] {
            "[0]1.5MHz",
            "[1]2.9MHz",
            "[2]4.2MHz",
            "[3]5.6MHz",
            "[4]6.9MHz",
            "[5]8.1MHz",
            "[6]9.4MHz",
            "[7]10.7MHz",
            "[8]11.9MHz",
            "[9]13.2MHz",
            "[A]14.3MHz",
            "[B]15.5MHz",
            "[C]16.7MHz",
            "[D]17.8MHz",
            "[E]19.0MHz",
            "[F]20.1MHz",
            "[10]21.2MHz",
            "[11]22.4MHz",
            "[12]23.5MHz",
            "[13]24.6MHz",
            "[14]25.6MHz",
            "[15]26.7MHz",
            "[16]27.7MHz",
            "[17]28.8MHz",
            "[18]29.9MHz",
            "[19]30.9MHz",
            "[1A]31.9MHz",
            "[1B]32.9MHz",
            "[1C]33.9MHz",
            "[1D]35.2MHz",
            "[1E]36.3MHz",
            "[1F]37.1MHz"});
            this.DfsCFG_comboBox1.Location = new System.Drawing.Point(9, 82);
            this.DfsCFG_comboBox1.Name = "DfsCFG_comboBox1";
            this.DfsCFG_comboBox1.Size = new System.Drawing.Size(107, 20);
            this.DfsCFG_comboBox1.TabIndex = 55;
            this.DfsCFG_comboBox1.Text = "[B]15.5MHz";
            // 
            // freq_cos_check
            // 
            this.freq_cos_check.AutoSize = true;
            this.freq_cos_check.Location = new System.Drawing.Point(10, 61);
            this.freq_cos_check.Name = "freq_cos_check";
            this.freq_cos_check.Size = new System.Drawing.Size(54, 16);
            this.freq_cos_check.TabIndex = 55;
            this.freq_cos_check.Text = "有COS";
            this.freq_cos_check.UseVisualStyleBackColor = true;
            // 
            // Freq_trim_button
            // 
            this.Freq_trim_button.Location = new System.Drawing.Point(9, 17);
            this.Freq_trim_button.Name = "Freq_trim_button";
            this.Freq_trim_button.Size = new System.Drawing.Size(79, 24);
            this.Freq_trim_button.TabIndex = 55;
            this.Freq_trim_button.Text = "频率";
            this.Freq_trim_button.UseVisualStyleBackColor = true;
            this.Freq_trim_button.Click += new System.EventHandler(this.Freq_trim_button_Click);
            // 
            // FMXX_config
            // 
            this.FMXX_config.BackColor = System.Drawing.SystemColors.Control;
            this.FMXX_config.Controls.Add(this.label62);
            this.FMXX_config.Controls.Add(this.keylegth_label);
            this.FMXX_config.Controls.Add(this.richTextBox1);
            this.FMXX_config.Controls.Add(this.CWD_auth_checkbox);
            this.FMXX_config.Controls.Add(this.CfgWordWrite_chkbox);
            this.FMXX_config.Controls.Add(this.DetectCfgWord_btn);
            this.FMXX_config.Controls.Add(this.CfgWord_Parse_btn);
            this.FMXX_config.Controls.Add(this.CfgWd_Gen_button);
            this.FMXX_config.Controls.Add(this.CfgWord_dataGridView);
            this.FMXX_config.Location = new System.Drawing.Point(4, 22);
            this.FMXX_config.Name = "FMXX_config";
            this.FMXX_config.Padding = new System.Windows.Forms.Padding(3);
            this.FMXX_config.Size = new System.Drawing.Size(956, 406);
            this.FMXX_config.TabIndex = 5;
            this.FMXX_config.Text = "FM309(old)";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label62.Location = new System.Drawing.Point(805, 187);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(90, 12);
            this.label62.TabIndex = 69;
            this.label62.Text = "下方输入密钥:";
            this.label62.Visible = false;
            // 
            // keylegth_label
            // 
            this.keylegth_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.keylegth_label.AutoSize = true;
            this.keylegth_label.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.keylegth_label.Location = new System.Drawing.Point(921, 187);
            this.keylegth_label.Name = "keylegth_label";
            this.keylegth_label.Size = new System.Drawing.Size(17, 12);
            this.keylegth_label.TabIndex = 39;
            this.keylegth_label.Text = "16";
            this.keylegth_label.Visible = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(807, 202);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(143, 38);
            this.richTextBox1.TabIndex = 68;
            this.richTextBox1.Text = "00000000000000000000000000000000";
            this.richTextBox1.Visible = false;
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // CWD_auth_checkbox
            // 
            this.CWD_auth_checkbox.AutoSize = true;
            this.CWD_auth_checkbox.Location = new System.Drawing.Point(805, 168);
            this.CWD_auth_checkbox.Name = "CWD_auth_checkbox";
            this.CWD_auth_checkbox.Size = new System.Drawing.Size(48, 16);
            this.CWD_auth_checkbox.TabIndex = 66;
            this.CWD_auth_checkbox.Text = "认证";
            this.CWD_auth_checkbox.UseVisualStyleBackColor = true;
            this.CWD_auth_checkbox.CheckedChanged += new System.EventHandler(this.CWD_auth_checkbox_CheckedChanged);
            // 
            // CfgWordWrite_chkbox
            // 
            this.CfgWordWrite_chkbox.AutoSize = true;
            this.CfgWordWrite_chkbox.Location = new System.Drawing.Point(805, 338);
            this.CfgWordWrite_chkbox.Name = "CfgWordWrite_chkbox";
            this.CfgWordWrite_chkbox.Size = new System.Drawing.Size(108, 16);
            this.CfgWordWrite_chkbox.TabIndex = 65;
            this.CfgWordWrite_chkbox.Text = "生成的同时写入";
            this.CfgWordWrite_chkbox.UseVisualStyleBackColor = true;
            // 
            // DetectCfgWord_btn
            // 
            this.DetectCfgWord_btn.Location = new System.Drawing.Point(805, 246);
            this.DetectCfgWord_btn.Name = "DetectCfgWord_btn";
            this.DetectCfgWord_btn.Size = new System.Drawing.Size(146, 40);
            this.DetectCfgWord_btn.TabIndex = 64;
            this.DetectCfgWord_btn.Text = "检测控制字";
            this.DetectCfgWord_btn.UseVisualStyleBackColor = true;
            this.DetectCfgWord_btn.Click += new System.EventHandler(this.DetectCfgWord_btn_Click);
            // 
            // CfgWord_Parse_btn
            // 
            this.CfgWord_Parse_btn.Location = new System.Drawing.Point(805, 292);
            this.CfgWord_Parse_btn.Name = "CfgWord_Parse_btn";
            this.CfgWord_Parse_btn.Size = new System.Drawing.Size(146, 40);
            this.CfgWord_Parse_btn.TabIndex = 63;
            this.CfgWord_Parse_btn.Text = "解析控制字";
            this.CfgWord_Parse_btn.UseVisualStyleBackColor = true;
            this.CfgWord_Parse_btn.Click += new System.EventHandler(this.CfgWord_Parse_btn_Click);
            // 
            // CfgWd_Gen_button
            // 
            this.CfgWd_Gen_button.Location = new System.Drawing.Point(805, 360);
            this.CfgWd_Gen_button.Name = "CfgWd_Gen_button";
            this.CfgWd_Gen_button.Size = new System.Drawing.Size(146, 40);
            this.CfgWd_Gen_button.TabIndex = 62;
            this.CfgWd_Gen_button.Text = "生成控制字";
            this.CfgWd_Gen_button.UseVisualStyleBackColor = true;
            this.CfgWd_Gen_button.Click += new System.EventHandler(this.CfgWd_Gen_button_Click);
            // 
            // CfgWord_dataGridView
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CfgWord_dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.CfgWord_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.CfgWord_dataGridView.DefaultCellStyle = dataGridViewCellStyle8;
            this.CfgWord_dataGridView.Location = new System.Drawing.Point(6, 6);
            this.CfgWord_dataGridView.Name = "CfgWord_dataGridView";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CfgWord_dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.CfgWord_dataGridView.RowTemplate.Height = 23;
            this.CfgWord_dataGridView.Size = new System.Drawing.Size(793, 394);
            this.CfgWord_dataGridView.TabIndex = 61;
            this.CfgWord_dataGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.CfgWord_dataGridView_ColumnWidthChanged);
            this.CfgWord_dataGridView.CurrentCellChanged += new System.EventHandler(this.CfgWord_dataGridView_CurrentCellChanged);
            this.CfgWord_dataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.CfgWord_dataGridView_Scroll);
            // 
            // new_config
            // 
            this.new_config.Controls.Add(this.CWD_CMP_checkBox);
            this.new_config.Controls.Add(this.button12);
            this.new_config.Controls.Add(this.CB_DisturbDoubleCheck);
            this.new_config.Controls.Add(this.label70);
            this.new_config.Controls.Add(this.nvrDisturbParam);
            this.new_config.Controls.Add(this.CB_AutoReverse);
            this.new_config.Controls.Add(this.saveascoe__button);
            this.new_config.Controls.Add(this.groupBox15);
            this.new_config.Controls.Add(this.CB_WordRegEn);
            this.new_config.Controls.Add(this.FMXX_SEL_CFGcomboBox);
            this.new_config.Controls.Add(this.groupBox9);
            this.new_config.Controls.Add(this.WriteCWD_button);
            this.new_config.Controls.Add(this.GenerateCWD_button);
            this.new_config.Controls.Add(this.AnalysisCWD_button);
            this.new_config.Controls.Add(this.ReadCWD_button);
            this.new_config.Controls.Add(this.cfg_dataGridView);
            this.new_config.Location = new System.Drawing.Point(4, 22);
            this.new_config.Name = "new_config";
            this.new_config.Padding = new System.Windows.Forms.Padding(3);
            this.new_config.Size = new System.Drawing.Size(956, 406);
            this.new_config.TabIndex = 9;
            this.new_config.Text = "配置信息";
            this.new_config.UseVisualStyleBackColor = true;
            // 
            // CWD_CMP_checkBox
            // 
            this.CWD_CMP_checkBox.AutoSize = true;
            this.CWD_CMP_checkBox.Location = new System.Drawing.Point(892, 299);
            this.CWD_CMP_checkBox.Name = "CWD_CMP_checkBox";
            this.CWD_CMP_checkBox.Size = new System.Drawing.Size(66, 16);
            this.CWD_CMP_checkBox.TabIndex = 14;
            this.CWD_CMP_checkBox.Text = "CWD比较";
            this.CWD_CMP_checkBox.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(893, 7);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(62, 35);
            this.button12.TabIndex = 69;
            this.button12.Text = "Connect Reader";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.ConnectReader_Click);
            // 
            // CB_DisturbDoubleCheck
            // 
            this.CB_DisturbDoubleCheck.AutoSize = true;
            this.CB_DisturbDoubleCheck.Enabled = false;
            this.CB_DisturbDoubleCheck.Location = new System.Drawing.Point(816, 100);
            this.CB_DisturbDoubleCheck.Name = "CB_DisturbDoubleCheck";
            this.CB_DisturbDoubleCheck.Size = new System.Drawing.Size(15, 14);
            this.CB_DisturbDoubleCheck.TabIndex = 14;
            this.CB_DisturbDoubleCheck.UseVisualStyleBackColor = true;
            this.CB_DisturbDoubleCheck.CheckedChanged += new System.EventHandler(this.CB_DisturbDoubleCheck_CheckedChanged);
            // 
            // label70
            // 
            this.label70.AutoSize = true;
            this.label70.Location = new System.Drawing.Point(757, 100);
            this.label70.Name = "label70";
            this.label70.Size = new System.Drawing.Size(59, 12);
            this.label70.TabIndex = 13;
            this.label70.Text = "扰乱因子:";
            // 
            // nvrDisturbParam
            // 
            this.nvrDisturbParam.Enabled = false;
            this.nvrDisturbParam.Location = new System.Drawing.Point(832, 96);
            this.nvrDisturbParam.MaxLength = 8;
            this.nvrDisturbParam.Name = "nvrDisturbParam";
            this.nvrDisturbParam.Size = new System.Drawing.Size(59, 21);
            this.nvrDisturbParam.TabIndex = 12;
            this.nvrDisturbParam.Text = "00000000";
            // 
            // CB_AutoReverse
            // 
            this.CB_AutoReverse.AutoSize = true;
            this.CB_AutoReverse.Enabled = false;
            this.CB_AutoReverse.Location = new System.Drawing.Point(845, 75);
            this.CB_AutoReverse.Name = "CB_AutoReverse";
            this.CB_AutoReverse.Size = new System.Drawing.Size(72, 16);
            this.CB_AutoReverse.TabIndex = 11;
            this.CB_AutoReverse.TabStop = false;
            this.CB_AutoReverse.Text = "自动反码";
            this.CB_AutoReverse.UseVisualStyleBackColor = true;
            this.CB_AutoReverse.CheckedChanged += new System.EventHandler(this.CB_AutoReverse_CheckedChanged);
            // 
            // saveascoe__button
            // 
            this.saveascoe__button.Location = new System.Drawing.Point(898, 365);
            this.saveascoe__button.Name = "saveascoe__button";
            this.saveascoe__button.Size = new System.Drawing.Size(50, 33);
            this.saveascoe__button.TabIndex = 10;
            this.saveascoe__button.Text = "另存COE";
            this.saveascoe__button.UseVisualStyleBackColor = true;
            this.saveascoe__button.Click += new System.EventHandler(this.saveascoe__button_Click);
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.button_ReverseOption);
            this.groupBox15.Controls.Add(this.CB_EnableOption);
            this.groupBox15.Controls.Add(this.CB_NVRALL_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR6_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR5_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR4_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR3_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR2_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR1_SEL);
            this.groupBox15.Controls.Add(this.CB_NVR0_SEL);
            this.groupBox15.Location = new System.Drawing.Point(752, 118);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(139, 129);
            this.groupBox15.TabIndex = 9;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "NVR_Sel";
            // 
            // button_ReverseOption
            // 
            this.button_ReverseOption.Enabled = false;
            this.button_ReverseOption.Location = new System.Drawing.Point(81, 101);
            this.button_ReverseOption.Name = "button_ReverseOption";
            this.button_ReverseOption.Size = new System.Drawing.Size(41, 22);
            this.button_ReverseOption.TabIndex = 13;
            this.button_ReverseOption.Text = "取反";
            this.button_ReverseOption.UseVisualStyleBackColor = true;
            this.button_ReverseOption.Click += new System.EventHandler(this.button_ReverseOption_Click);
            // 
            // CB_EnableOption
            // 
            this.CB_EnableOption.AutoSize = true;
            this.CB_EnableOption.Enabled = false;
            this.CB_EnableOption.Location = new System.Drawing.Point(12, 105);
            this.CB_EnableOption.Name = "CB_EnableOption";
            this.CB_EnableOption.Size = new System.Drawing.Size(72, 16);
            this.CB_EnableOption.TabIndex = 12;
            this.CB_EnableOption.Text = "开启选项";
            this.CB_EnableOption.UseVisualStyleBackColor = true;
            this.CB_EnableOption.CheckedChanged += new System.EventHandler(this.CB_EnableOption_CheckedChanged);
            this.CB_EnableOption.Click += new System.EventHandler(this.CB_EnableOption_Click);
            // 
            // CB_NVRALL_SEL
            // 
            this.CB_NVRALL_SEL.AutoSize = true;
            this.CB_NVRALL_SEL.Enabled = false;
            this.CB_NVRALL_SEL.Location = new System.Drawing.Point(80, 80);
            this.CB_NVRALL_SEL.Name = "CB_NVRALL_SEL";
            this.CB_NVRALL_SEL.Size = new System.Drawing.Size(42, 16);
            this.CB_NVRALL_SEL.TabIndex = 11;
            this.CB_NVRALL_SEL.Text = "ALL";
            this.CB_NVRALL_SEL.UseVisualStyleBackColor = true;
            this.CB_NVRALL_SEL.Click += new System.EventHandler(this.CB_NVRALL_SEL_Click);
            // 
            // CB_NVR6_SEL
            // 
            this.CB_NVR6_SEL.AutoSize = true;
            this.CB_NVR6_SEL.Enabled = false;
            this.CB_NVR6_SEL.Location = new System.Drawing.Point(12, 80);
            this.CB_NVR6_SEL.Name = "CB_NVR6_SEL";
            this.CB_NVR6_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR6_SEL.TabIndex = 10;
            this.CB_NVR6_SEL.Text = "NVR6";
            this.CB_NVR6_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR6_SEL.Click += new System.EventHandler(this.CB_NVR6_SEL_Click);
            // 
            // CB_NVR5_SEL
            // 
            this.CB_NVR5_SEL.AutoSize = true;
            this.CB_NVR5_SEL.Enabled = false;
            this.CB_NVR5_SEL.Location = new System.Drawing.Point(80, 60);
            this.CB_NVR5_SEL.Name = "CB_NVR5_SEL";
            this.CB_NVR5_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR5_SEL.TabIndex = 5;
            this.CB_NVR5_SEL.Text = "NVR5";
            this.CB_NVR5_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR5_SEL.Click += new System.EventHandler(this.CB_NVR5_SEL_Click);
            // 
            // CB_NVR4_SEL
            // 
            this.CB_NVR4_SEL.AutoSize = true;
            this.CB_NVR4_SEL.Enabled = false;
            this.CB_NVR4_SEL.Location = new System.Drawing.Point(12, 60);
            this.CB_NVR4_SEL.Name = "CB_NVR4_SEL";
            this.CB_NVR4_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR4_SEL.TabIndex = 4;
            this.CB_NVR4_SEL.Text = "NVR4";
            this.CB_NVR4_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR4_SEL.Click += new System.EventHandler(this.CB_NVR4_SEL_Click);
            // 
            // CB_NVR3_SEL
            // 
            this.CB_NVR3_SEL.AutoSize = true;
            this.CB_NVR3_SEL.Enabled = false;
            this.CB_NVR3_SEL.Location = new System.Drawing.Point(80, 40);
            this.CB_NVR3_SEL.Name = "CB_NVR3_SEL";
            this.CB_NVR3_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR3_SEL.TabIndex = 3;
            this.CB_NVR3_SEL.Text = "NVR3";
            this.CB_NVR3_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR3_SEL.Click += new System.EventHandler(this.CB_NVR3_SEL_Click);
            // 
            // CB_NVR2_SEL
            // 
            this.CB_NVR2_SEL.AutoSize = true;
            this.CB_NVR2_SEL.Enabled = false;
            this.CB_NVR2_SEL.Location = new System.Drawing.Point(12, 40);
            this.CB_NVR2_SEL.Name = "CB_NVR2_SEL";
            this.CB_NVR2_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR2_SEL.TabIndex = 2;
            this.CB_NVR2_SEL.Text = "NVR2";
            this.CB_NVR2_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR2_SEL.Click += new System.EventHandler(this.CB_NVR2_SEL_Click);
            // 
            // CB_NVR1_SEL
            // 
            this.CB_NVR1_SEL.AutoSize = true;
            this.CB_NVR1_SEL.Enabled = false;
            this.CB_NVR1_SEL.Location = new System.Drawing.Point(80, 20);
            this.CB_NVR1_SEL.Name = "CB_NVR1_SEL";
            this.CB_NVR1_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR1_SEL.TabIndex = 1;
            this.CB_NVR1_SEL.Text = "NVR1";
            this.CB_NVR1_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR1_SEL.Click += new System.EventHandler(this.CB_NVR1_SEL_Click);
            // 
            // CB_NVR0_SEL
            // 
            this.CB_NVR0_SEL.AutoSize = true;
            this.CB_NVR0_SEL.Enabled = false;
            this.CB_NVR0_SEL.Location = new System.Drawing.Point(12, 20);
            this.CB_NVR0_SEL.Name = "CB_NVR0_SEL";
            this.CB_NVR0_SEL.Size = new System.Drawing.Size(48, 16);
            this.CB_NVR0_SEL.TabIndex = 0;
            this.CB_NVR0_SEL.Text = "NVR0";
            this.CB_NVR0_SEL.UseVisualStyleBackColor = true;
            this.CB_NVR0_SEL.Click += new System.EventHandler(this.CB_NVR0_SEL_Click);
            // 
            // CB_WordRegEn
            // 
            this.CB_WordRegEn.AutoSize = true;
            this.CB_WordRegEn.Enabled = false;
            this.CB_WordRegEn.Location = new System.Drawing.Point(760, 75);
            this.CB_WordRegEn.Name = "CB_WordRegEn";
            this.CB_WordRegEn.Size = new System.Drawing.Size(78, 16);
            this.CB_WordRegEn.TabIndex = 8;
            this.CB_WordRegEn.Text = "WordRegEn";
            this.CB_WordRegEn.UseVisualStyleBackColor = true;
            // 
            // FMXX_SEL_CFGcomboBox
            // 
            this.FMXX_SEL_CFGcomboBox.BackColor = System.Drawing.SystemColors.Window;
            this.FMXX_SEL_CFGcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FMXX_SEL_CFGcomboBox.FormattingEnabled = true;
            this.FMXX_SEL_CFGcomboBox.Items.AddRange(new object[] {
            "FM336",
            "FM326",
            "FM349",
            "FM347",
            "FM294",
            "FM274",
            "FM302",
            "FM309",
            "FM349_NEW",
            "FM349_Security",
            "FM336_NEW"});
            this.FMXX_SEL_CFGcomboBox.Location = new System.Drawing.Point(754, 51);
            this.FMXX_SEL_CFGcomboBox.Name = "FMXX_SEL_CFGcomboBox";
            this.FMXX_SEL_CFGcomboBox.Size = new System.Drawing.Size(137, 20);
            this.FMXX_SEL_CFGcomboBox.TabIndex = 7;
            this.FMXX_SEL_CFGcomboBox.SelectedIndexChanged += new System.EventHandler(this.FMXX_SEL_CFGcomboBox_SelectedIndexChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.CL_newcfg_radioButton);
            this.groupBox9.Controls.Add(this.CT_newcfg_radioButton);
            this.groupBox9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox9.Location = new System.Drawing.Point(754, 0);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(137, 42);
            this.groupBox9.TabIndex = 6;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "接口类型";
            // 
            // CL_newcfg_radioButton
            // 
            this.CL_newcfg_radioButton.AutoSize = true;
            this.CL_newcfg_radioButton.Checked = true;
            this.CL_newcfg_radioButton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CL_newcfg_radioButton.Location = new System.Drawing.Point(10, 18);
            this.CL_newcfg_radioButton.Name = "CL_newcfg_radioButton";
            this.CL_newcfg_radioButton.Size = new System.Drawing.Size(53, 18);
            this.CL_newcfg_radioButton.TabIndex = 2;
            this.CL_newcfg_radioButton.TabStop = true;
            this.CL_newcfg_radioButton.Text = "非接";
            this.CL_newcfg_radioButton.UseVisualStyleBackColor = true;
            // 
            // CT_newcfg_radioButton
            // 
            this.CT_newcfg_radioButton.AutoSize = true;
            this.CT_newcfg_radioButton.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CT_newcfg_radioButton.Location = new System.Drawing.Point(78, 18);
            this.CT_newcfg_radioButton.Name = "CT_newcfg_radioButton";
            this.CT_newcfg_radioButton.Size = new System.Drawing.Size(53, 18);
            this.CT_newcfg_radioButton.TabIndex = 1;
            this.CT_newcfg_radioButton.Text = "接触";
            this.CT_newcfg_radioButton.UseVisualStyleBackColor = true;
            // 
            // WriteCWD_button
            // 
            this.WriteCWD_button.Location = new System.Drawing.Point(752, 328);
            this.WriteCWD_button.Name = "WriteCWD_button";
            this.WriteCWD_button.Size = new System.Drawing.Size(139, 33);
            this.WriteCWD_button.TabIndex = 2;
            this.WriteCWD_button.Text = "写控制字";
            this.WriteCWD_button.UseVisualStyleBackColor = true;
            this.WriteCWD_button.Click += new System.EventHandler(this.WriteCWD_button_Click);
            // 
            // GenerateCWD_button
            // 
            this.GenerateCWD_button.Location = new System.Drawing.Point(752, 365);
            this.GenerateCWD_button.Name = "GenerateCWD_button";
            this.GenerateCWD_button.Size = new System.Drawing.Size(139, 33);
            this.GenerateCWD_button.TabIndex = 1;
            this.GenerateCWD_button.Text = "生成控制字";
            this.GenerateCWD_button.UseVisualStyleBackColor = true;
            this.GenerateCWD_button.Click += new System.EventHandler(this.GenerateCWD_button_Click);
            // 
            // AnalysisCWD_button
            // 
            this.AnalysisCWD_button.Location = new System.Drawing.Point(752, 290);
            this.AnalysisCWD_button.Name = "AnalysisCWD_button";
            this.AnalysisCWD_button.Size = new System.Drawing.Size(139, 33);
            this.AnalysisCWD_button.TabIndex = 1;
            this.AnalysisCWD_button.Text = "解析控制字";
            this.AnalysisCWD_button.UseVisualStyleBackColor = true;
            this.AnalysisCWD_button.Click += new System.EventHandler(this.AnalysisCWD_button_Click);
            // 
            // ReadCWD_button
            // 
            this.ReadCWD_button.Location = new System.Drawing.Point(752, 253);
            this.ReadCWD_button.Name = "ReadCWD_button";
            this.ReadCWD_button.Size = new System.Drawing.Size(139, 33);
            this.ReadCWD_button.TabIndex = 1;
            this.ReadCWD_button.Text = "读控制字";
            this.ReadCWD_button.UseVisualStyleBackColor = true;
            this.ReadCWD_button.Click += new System.EventHandler(this.ReadCWD_button_Click);
            // 
            // cfg_dataGridView
            // 
            this.cfg_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cfg_dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.cfg_dataGridView.Location = new System.Drawing.Point(28, 3);
            this.cfg_dataGridView.Name = "cfg_dataGridView";
            this.cfg_dataGridView.RowTemplate.Height = 23;
            this.cfg_dataGridView.Size = new System.Drawing.Size(690, 395);
            this.cfg_dataGridView.TabIndex = 0;
            this.cfg_dataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.cfg_dataGridView_CellBeginEdit);
            this.cfg_dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.cfg_dataGridView_CellEndEdit);
            this.cfg_dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.cfg_dataGridView_CellEnter);
            this.cfg_dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.cfg_dataGridView_CellValueChanged);
            this.cfg_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.cfg_dataGridView_CurrentCellDirtyStateChanged);
            this.cfg_dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.cfg_dataGridView_DataError);
            this.cfg_dataGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.cfg_dataGridView_RowPrePaint);
            // 
            // folderBrowserDialog_Scr
            // 
            this.folderBrowserDialog_Scr.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // SendData_textBox
            // 
            this.SendData_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SendData_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.SendData_textBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SendData_textBox.Location = new System.Drawing.Point(4, 438);
            this.SendData_textBox.Name = "SendData_textBox";
            this.SendData_textBox.Size = new System.Drawing.Size(860, 26);
            this.SendData_textBox.TabIndex = 15;
            this.SendData_textBox.TextChanged += new System.EventHandler(this.SendData_textBox_TextChanged);
            this.SendData_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SendData_textBox_KeyDown);
            this.SendData_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SendData_KeyPress);
            // 
            // SendData_label
            // 
            this.SendData_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SendData_label.AutoSize = true;
            this.SendData_label.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SendData_label.Location = new System.Drawing.Point(887, 441);
            this.SendData_label.Name = "SendData_label";
            this.SendData_label.Size = new System.Drawing.Size(26, 16);
            this.SendData_label.TabIndex = 38;
            this.SendData_label.Text = "00";
            // 
            // SimpleScript_backgroundWorker
            // 
            this.SimpleScript_backgroundWorker.WorkerReportsProgress = true;
            this.SimpleScript_backgroundWorker.WorkerSupportsCancellation = true;
            this.SimpleScript_backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SimpleScript_backgroundWorker_DoWork);
            this.SimpleScript_backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SimpleScript_backgroundWorker_RunWorkerCompleted);
            // 
            // Script_backgroundWorker
            // 
            this.Script_backgroundWorker.WorkerReportsProgress = true;
            this.Script_backgroundWorker.WorkerSupportsCancellation = true;
            this.Script_backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Script_backgroundWorker_DoWork);
            this.Script_backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Script_backgroundWorker_ProgressChanged);
            this.Script_backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Script_backgroundWorker_RunWorkerCompleted);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "FmReader";
            this.notifyIcon1.Visible = true;
            // 
            // toolTip_CLIterface
            // 
            this.toolTip_CLIterface.AutoPopDelay = 5000;
            this.toolTip_CLIterface.InitialDelay = 500;
            this.toolTip_CLIterface.ReshowDelay = 500;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 722);
            this.Controls.Add(this.tabControl_main);
            this.Controls.Add(this.main_note);
            this.Controls.Add(this.SendData_textBox);
            this.Controls.Add(this.SendData_label);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FMreader";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.main_note_cMS.ResumeLayout(false);
            this.tabControl_main.ResumeLayout(false);
            this.tabPage_CL.ResumeLayout(false);
            this.tabPage_CL.PerformLayout();
            this.groupBox_MIFARE.ResumeLayout(false);
            this.groupBox_MIFARE.PerformLayout();
            this.cla_parity_groupbox.ResumeLayout(false);
            this.cla_parity_groupbox.PerformLayout();
            this.CLTypeSelect_groupBox.ResumeLayout(false);
            this.CLTypeSelect_groupBox.PerformLayout();
            this.CRC_gropbox.ResumeLayout(false);
            this.CRC_gropbox.PerformLayout();
            this.groupBox_FM17Reg.ResumeLayout(false);
            this.groupBox_FM17Reg.PerformLayout();
            this.groupBox_CLA_CONTROL.ResumeLayout(false);
            this.groupBox_CLA_CONTROL.PerformLayout();
            this.groupBox_CLB_CONTROL.ResumeLayout(false);
            this.groupBox_CLB_CONTROL.PerformLayout();
            this.tabPage_CT.ResumeLayout(false);
            this.tabPage_CT.PerformLayout();
            this.groupBox21.ResumeLayout(false);
            this.groupBox21.PerformLayout();
            this.ins54.ResumeLayout(false);
            this.ins54.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.Parity_groupBox.ResumeLayout(false);
            this.Parity_groupBox.PerformLayout();
            this.TR_CT_TimeOut.ResumeLayout(false);
            this.TR_CT_TimeOut.PerformLayout();
            this.CST_select.ResumeLayout(false);
            this.CST_select.PerformLayout();
            this.VoltageSel.ResumeLayout(false);
            this.VoltageSel.PerformLayout();
            this.tabPage_SPI_I2C.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.I2C_groupBox.ResumeLayout(false);
            this.I2C_groupBox.PerformLayout();
            this.SPI_groupBox.ResumeLayout(false);
            this.SPI_groupBox.PerformLayout();
            this.tabPage_Script1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Script)).EndInit();
            this.tabPage_Script.ResumeLayout(false);
            this.lighttest.ResumeLayout(false);
            this.lighttest.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.cfgroup.ResumeLayout(false);
            this.cfgroup.PerformLayout();
            this.cfw.ResumeLayout(false);
            this.cfw.PerformLayout();
            this.cfg.ResumeLayout(false);
            this.cfg.PerformLayout();
            this.sjxm.ResumeLayout(false);
            this.sjxm.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.tabPage_Program.ResumeLayout(false);
            this.tabPage_Program.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox_chips.ResumeLayout(false);
            this.groupBox_350_Cfg.ResumeLayout(false);
            this.groupBox_350_Cfg.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.MemTypeSelGrop.ResumeLayout(false);
            this.MemTypeSelGrop.PerformLayout();
            this.groupBoxOperation.ResumeLayout(false);
            this.groupBoxOperation.PerformLayout();
            this.interface_sel_grpbox.ResumeLayout(false);
            this.interface_sel_grpbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progEEdataGridView)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.FMXX_config.ResumeLayout(false);
            this.FMXX_config.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CfgWord_dataGridView)).EndInit();
            this.new_config.ResumeLayout(false);
            this.new_config.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cfg_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl_main;
        private System.Windows.Forms.TabPage tabPage_CL;
        private System.Windows.Forms.TabPage tabPage_CT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox RegData_17_textBox;
        private System.Windows.Forms.TextBox RegAddr_17_textBox;
        private System.Windows.Forms.Button buttonReadBlock;
        private System.Windows.Forms.Button buttonWriteBlock;
        private System.Windows.Forms.Button Write17Reg;
        private System.Windows.Forms.Button Read17Reg;
        private System.Windows.Forms.Button Active;
        private System.Windows.Forms.Button MI_Select;
        private System.Windows.Forms.Button MI_AntiColl;
        private System.Windows.Forms.Button MI_REQA;
        private System.Windows.Forms.Button Reset17;
        private System.Windows.Forms.Button TransceiveCL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox BlockAddr_textBox;
        private System.Windows.Forms.TextBox TimeOut_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox TransceiveDataCL_listBox;
        private System.Windows.Forms.Button Init_TDA8007;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox RegData_TDA_textBox;
        private System.Windows.Forms.TextBox RegAddr_TDA_textBox;
        private System.Windows.Forms.Button WriteTDAReg;
        private System.Windows.Forms.Button ReadTDAReg;
        private System.Windows.Forms.Button Cold_Reset;
        private System.Windows.Forms.Button Warm_Reset;
        private System.Windows.Forms.GroupBox VoltageSel;
        private System.Windows.Forms.RadioButton Volt18;
        private System.Windows.Forms.RadioButton Volt30;
        private System.Windows.Forms.RadioButton Volt50;
        private System.Windows.Forms.Button ConnectReader;
        private System.Windows.Forms.Button Field_ON;
        private System.Windows.Forms.Button TransceiveCT;
        private System.Windows.Forms.GroupBox CST_select;
        private System.Windows.Forms.RadioButton CST_Low;
        private System.Windows.Forms.RadioButton CST_High;
        private System.Windows.Forms.Button Clock_Stop_Btn;
        private System.Windows.Forms.TabPage tabPage_Program;
        private System.Windows.Forms.GroupBox TR_CT_TimeOut;
        private System.Windows.Forms.RadioButton CT_TimeOut_NoTimeout;
        private System.Windows.Forms.RadioButton CT_TimeOut_100ETU;
        private System.Windows.Forms.RadioButton CT_TimeOut_std;
        private System.Windows.Forms.ComboBox CardType_Select;
        private System.Windows.Forms.TabPage tabPage_Script;
        private System.Windows.Forms.ListBox Scripts_list;
        private System.Windows.Forms.Button Open_Scr_dir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_Scr;
        public System.Windows.Forms.RichTextBox main_note;
        private System.Windows.Forms.Button Run_Script_btn;
        private System.Windows.Forms.TextBox SendData_textBox;
        private System.Windows.Forms.Label SendData_label;
        private System.Windows.Forms.TabPage tabPage_Script1;
        private System.Windows.Forms.DataGridView dataGridView_Script;
        private System.Windows.Forms.TabPage FMXX_config;
        private System.Windows.Forms.Button Open_ScriptFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog_Scr;
        private System.Windows.Forms.Label label_APDU_CL;
        private System.Windows.Forms.TextBox textBox_APDU_P3_CL;
        private System.Windows.Forms.TextBox textBox_APDU_P2_CL;
        private System.Windows.Forms.TextBox textBox_APDU_P1_CL;
        private System.Windows.Forms.TextBox textBox_APDU_INS_CL;
        private System.Windows.Forms.TextBox textBox_APDU_CLA_CL;
        private System.Windows.Forms.CheckBox checkBox_APDUmodeCL;
        private System.Windows.Forms.Button RstScrScope;
        private System.Windows.Forms.Button RunScrStep_simple;
        public System.ComponentModel.BackgroundWorker SimpleScript_backgroundWorker;
        private System.Windows.Forms.Button RunScr_simple;
        private System.Windows.Forms.Button StopRunScr_simple;
        public System.ComponentModel.BackgroundWorker Script_backgroundWorker;
        private System.Windows.Forms.Button Stop_RunScript_btn;
        private System.Windows.Forms.ContextMenuStrip main_note_cMS;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        public System.Windows.Forms.RichTextBox Scripts_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Commands;
        private System.Windows.Forms.DataGridViewTextBoxColumn CompareData;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReturnData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Results;
        private System.Windows.Forms.Button AddCMDtoListBox_CL_btn;
        private System.Windows.Forms.Label label_APDU_CT;
        private System.Windows.Forms.CheckBox checkBox_APDUmodeCT;
        private System.Windows.Forms.TextBox textBox_APDU_P3_CT;
        private System.Windows.Forms.TextBox textBox_APDU_P2_CT;
        private System.Windows.Forms.TextBox textBox_APDU_P1_CT;
        private System.Windows.Forms.TextBox textBox_APDU_INS_CT;
        private System.Windows.Forms.TextBox textBox_APDU_CLA_CT;
        private System.Windows.Forms.Button button_RATS;
        private System.Windows.Forms.TabPage tabPage_SPI_I2C;
        private System.Windows.Forms.GroupBox SPI_groupBox;
        private System.Windows.Forms.Button SPI_Send_btn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox SPI_LenToReceive_textBox;
        private System.Windows.Forms.GroupBox I2C_groupBox;
        private System.Windows.Forms.Button I2C_Send_btn;
        private System.Windows.Forms.ListBox TransceiveDataSPI_I2C_listBox;
        private System.Windows.Forms.Button AddtoListBox_SPI_I2C_btn;
        private System.Windows.Forms.ListBox TransceiveDataCT_listBox;
        private System.Windows.Forms.Button AddCMDtoListBox_CT_btn;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox I2C_LenToReceive_textBox;
        private System.Windows.Forms.GroupBox CLTypeSelect_groupBox;
        private System.Windows.Forms.RadioButton TypeBSel_radioButton;
        private System.Windows.Forms.RadioButton TypeASel_radioButton;
        private System.Windows.Forms.GroupBox Parity_groupBox;
        private System.Windows.Forms.RadioButton parity_odd_rdBtn;
        private System.Windows.Forms.RadioButton parity_even_rdBtn;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolTip Hints_toolTip;
        private System.Windows.Forms.ProgressBar Script_backgroundWorker_progressBar;
        private System.Windows.Forms.Button Halt;
        private System.Windows.Forms.Button WupA;
        private System.Windows.Forms.Timer swp_slave_autorx_timer;
        private System.Windows.Forms.Timer timswp_master_autorx;
        private System.Windows.Forms.DataGridView progEEdataGridView;
        private System.Windows.Forms.GroupBox MemTypeSelGrop;
        private System.Windows.Forms.RadioButton PROG_EEtype_radiobutton;
        private System.Windows.Forms.RadioButton DATA_EEtype_radiobutton;
        private System.Windows.Forms.GroupBox interface_sel_grpbox;
        private System.Windows.Forms.RadioButton CL_Interface;
        private System.Windows.Forms.RadioButton CT_Interface;
        private System.Windows.Forms.Button OpenFile_Button;
        private System.Windows.Forms.Button ReadEEdata_Button;
        private System.Windows.Forms.Button InitEEdata_Button;
        private System.Windows.Forms.Button ProgEE_Button;
        private System.Windows.Forms.CheckBox chkReadVerify;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TextBox ProgEEStartAddr;
        private System.Windows.Forms.TextBox ProgEEEndAddr;
        private System.Windows.Forms.ProgressBar ProgEE_progressBar;
        private System.Windows.Forms.CheckBox A1Program;
        private System.Windows.Forms.Button Init_Rdee_04;
        private System.Windows.Forms.Button Init_EEopt_02;
        private System.Windows.Forms.Button Init_01;
        private System.Windows.Forms.Button Init_Wree_00;
        private System.Windows.Forms.TextBox text_01_P1;
        private System.Windows.Forms.TextBox text_04_len;
        private System.Windows.Forms.TextBox text_00_len;
        private System.Windows.Forms.TextBox text_02_P2;
        private System.Windows.Forms.TextBox text_04_P2;
        private System.Windows.Forms.TextBox text_00_P2;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox text_02_P1;
        private System.Windows.Forms.TextBox text_04_P1;
        private System.Windows.Forms.TextBox text_00_P1;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.CheckBox CfgWordWrite_chkbox;
        private System.Windows.Forms.Button DetectCfgWord_btn;
        private System.Windows.Forms.Button CfgWord_Parse_btn;
        private System.Windows.Forms.Button CfgWd_Gen_button;
        private System.Windows.Forms.DataGridView CfgWord_dataGridView;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Button ResFreqCacu_L_btn;
        private System.Windows.Forms.Button ResFreqCacu_C_btn;
        private System.Windows.Forms.Button ResFreqCacu_F_btn;
        private System.Windows.Forms.TextBox ResonFreqCaculate_F_tB;
        private System.Windows.Forms.TextBox ResonFreqCaculate_L_tB;
        private System.Windows.Forms.TextBox ResonFreqCaculate_C_tB;
        private System.Windows.Forms.RadioButton RAMtype_radiobutton;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox text_08_P1;
        private System.Windows.Forms.Button Init_CPUStart_08;
        private System.Windows.Forms.TextBox text_03_P2;
        private System.Windows.Forms.TextBox text_03_P1;
        private System.Windows.Forms.Button Init_ewEEtime_03;
        private System.Windows.Forms.TextBox text_0B_P2;
        private System.Windows.Forms.TextBox text_0B_P1;
        private System.Windows.Forms.Button Init_Trim_0B;
        private System.Windows.Forms.Button SaveAS_Button;
        private System.Windows.Forms.CheckBox SaveReadbuf;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.TextBox SaveAsStart;
        private System.Windows.Forms.TextBox SaveAsEnd;
        private System.Windows.Forms.CheckBox prog_extand_mode1;
        private System.Windows.Forms.CheckBox prog_extand_mode0;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox Patch_Size;
        private System.Windows.Forms.CheckBox prog_extand_memRAM;
        private System.Windows.Forms.CheckBox prog_extand_memEE;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Button SaveAs_verify;
        private System.Windows.Forms.CheckBox MEM_EE;
        private System.Windows.Forms.CheckBox MEM_ROM;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.TextBox InitCRC16_texbox;
        private System.Windows.Forms.Button pps_exchange_CL_btn;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TextBox PPS_exchange_ct_tbox;
        private System.Windows.Forms.Button PPS_exchange_ct_btn;
        private System.Windows.Forms.Button version_check;
        private System.Windows.Forms.CheckBox flash_isp;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.ComboBox flash_isp_comSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialog_Prog;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.TextBox lib_Size;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.Button Card_init_Button;
        private System.Windows.Forms.GroupBox CRC_gropbox;
        private System.Windows.Forms.RadioButton tx_crc_cfg;
        private System.Windows.Forms.RadioButton rx_crc_cfg;
        private System.Windows.Forms.RadioButton no_crc_cfg;
        private System.Windows.Forms.RadioButton all_crc_cfg;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox Auth_checkBox;
        private System.Windows.Forms.TextBox text_0A_P2;
        private System.Windows.Forms.TextBox text_0A_P1;
        private System.Windows.Forms.Button init_CRC_0A;
        private System.Windows.Forms.Button init_AUTH_55;
        private System.Windows.Forms.CheckBox CWD_auth_checkbox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label keylegth_label;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.ComboBox InitData_comboBox;
        private System.Windows.Forms.Label keyl_label;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.RichTextBox key_richTextBox;
        private System.Windows.Forms.TabPage lighttest;
        private System.Windows.Forms.Button temp_start;
        private System.Windows.Forms.RichTextBox temp_rlt;
        private System.Windows.Forms.Button Freq_trim_button;
        private System.Windows.Forms.TextBox textBox_PPSCL_DSI;
        private System.Windows.Forms.Label label_DRI;
        private System.Windows.Forms.TextBox textBox_PPSCL_DRI;
        private System.Windows.Forms.Label label_DSI;
        private System.Windows.Forms.ComboBox KeyMode_comboBox;
        private System.Windows.Forms.TextBox AuthKey_textBox;
        private System.Windows.Forms.Button Ldkey_button;
        private System.Windows.Forms.Button Auth_button;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.TextBox AuthBlock_textBox;
        private System.Windows.Forms.ComboBox AuthType_comboBox;
        private System.Windows.Forms.GroupBox groupBox_MIFARE;
        private System.Windows.Forms.CheckBox freq_cos_check;
        private System.Windows.Forms.ComboBox DfsCFG_comboBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.DataGridViewTextBoxColumn EE_Addr;
        private System.Windows.Forms.DataGridViewTextBoxColumn EE_Data;
        private System.Windows.Forms.Label dest_label1;
        private System.Windows.Forms.TextBox ProgDestAddr;
        private System.Windows.Forms.Button FreqRead;
        private System.Windows.Forms.Button Prog_Move;
        private System.Windows.Forms.CheckBox checkBox_CTHighBaud;
        private System.Windows.Forms.CheckBox BufMove_checkBox;
        private System.Windows.Forms.RadioButton FreqRead_radioButton;
        private System.Windows.Forms.RadioButton FreqTrim_radioButton;
        private System.Windows.Forms.RadioButton FM326;
        private System.Windows.Forms.RadioButton FM309;
        private System.Windows.Forms.TabPage new_config;
        private System.Windows.Forms.Button GenerateCWD_button;
        private System.Windows.Forms.Button AnalysisCWD_button;
        private System.Windows.Forms.Button ReadCWD_button;
        private System.Windows.Forms.DataGridView cfg_dataGridView;
        private System.Windows.Forms.Button WriteCWD_button;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton CL_newcfg_radioButton;
        private System.Windows.Forms.RadioButton CT_newcfg_radioButton;
        private System.Windows.Forms.ComboBox FMXX_SEL_CFGcomboBox;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton FM309_radioButton;
        private System.Windows.Forms.RadioButton FM336_radioButton;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.CheckBox flash_check;
        private System.Windows.Forms.RadioButton radioButton_0xB9;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.RadioButton radioButton_0x00;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.RadioButton init_n;
        private System.Windows.Forms.RadioButton init_y;
        private System.Windows.Forms.Button FM294_str;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.ComboBox readers_cbox;
        private System.Windows.Forms.Button GetReaders_bnt;
        private System.Windows.Forms.ComboBox Chip_comboBox;
        private System.Windows.Forms.ComboBox temp_select;
        private System.Windows.Forms.CheckBox EE_patch_checkBox;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton FM336;
        private System.Windows.Forms.CheckBox CB_WordRegEn;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.CheckBox CB_NVR1_SEL;
        private System.Windows.Forms.CheckBox CB_NVR0_SEL;
        private System.Windows.Forms.CheckBox CB_NVRALL_SEL;
        private System.Windows.Forms.CheckBox CB_NVR6_SEL;
        private System.Windows.Forms.CheckBox CB_NVR5_SEL;
        private System.Windows.Forms.CheckBox CB_NVR4_SEL;
        private System.Windows.Forms.CheckBox CB_NVR3_SEL;
        private System.Windows.Forms.CheckBox CB_NVR2_SEL;
        private System.Windows.Forms.RadioButton FM349_radioButton;
        private System.Windows.Forms.Button saveascoe__button;
        private System.Windows.Forms.CheckBox CB_AutoReverse;
        private System.Windows.Forms.CheckBox CB_NoErase;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.TextBox ProgEEOffsetAddr;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.TextBox nvrDisturbParam;
        private System.Windows.Forms.CheckBox CB_DisturbDoubleCheck;
        private System.Windows.Forms.Button button_script_reload;
        private System.Windows.Forms.Button buttonReadValue;
        private System.Windows.Forms.Button buttonWriteValue;
        private System.Windows.Forms.TextBox textBox_APDU_Le_CL;
        private System.Windows.Forms.Label LeText;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.TextBox WR_TIME;
        private System.Windows.Forms.TextBox ER_TIME;
        private System.Windows.Forms.Button brst;
        private System.Windows.Forms.CheckBox user_lock;
        private System.Windows.Forms.CheckBox prog_cos;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.TextBox Akey;
        private System.Windows.Forms.TextBox ADcos;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.GroupBox groupBox18;
        private System.Windows.Forms.RadioButton ITct;
        private System.Windows.Forms.RadioButton ITcl;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.GroupBox groupBox19;
        private System.Windows.Forms.RadioButton Scl;
        private System.Windows.Forms.RadioButton Sct;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.CheckBox Trim_init;
        private System.Windows.Forms.RadioButton radioButton_0xFF;
        private System.Windows.Forms.Button MI_Deselect;
        private System.Windows.Forms.Button buttonINCValue;
        private System.Windows.Forms.Button buttonDECValue;
        private System.Windows.Forms.RadioButton FM294_radioButton;
        private System.Windows.Forms.RadioButton FM274_radioButton;
        private System.Windows.Forms.Label label_SAK3;
        public System.Windows.Forms.TextBox textBox_SAK3;
        private System.Windows.Forms.Label label_UID3;
        public System.Windows.Forms.TextBox textBox_UID3;
        private System.Windows.Forms.TextBox textBox_last_SAK;
        private System.Windows.Forms.Label label_SAK2;
        public System.Windows.Forms.TextBox textBox_SAK2;
        private System.Windows.Forms.Label label_UID2;
        public System.Windows.Forms.TextBox textBox_UID2;
        private System.Windows.Forms.Label label_SAK1;
        public System.Windows.Forms.TextBox textBox_SAK1;
        private System.Windows.Forms.Label label_UID1;
        public System.Windows.Forms.TextBox textBox_UID1;
        public System.Windows.Forms.CheckBox checkBox_check_BCC;
        private System.Windows.Forms.GroupBox cla_parity_groupbox;
        private System.Windows.Forms.RadioButton cla_no_parity_cfg;
        private System.Windows.Forms.RadioButton cla_even_parity_cfg;
        private System.Windows.Forms.RadioButton cla_odd_parity_cfg;
        private System.Windows.Forms.GroupBox groupBox_CLA_CONTROL;
        private System.Windows.Forms.CheckBox checkBox_last_SAK;
        private System.Windows.Forms.RadioButton FM350_radioButton;
        private System.Windows.Forms.CheckBox checkBox_350ChipErase;
        private System.Windows.Forms.ComboBox comboBox_ChipName;
        private System.Windows.Forms.GroupBox groupBox_chips;
        private System.Windows.Forms.GroupBox groupBoxOperation;
        private System.Windows.Forms.ComboBox comboBox_CTBaud;
        private System.Windows.Forms.GroupBox cfw;
        private System.Windows.Forms.RadioButton cfw_agn;
        private System.Windows.Forms.RadioButton cfw_rnd;
        private System.Windows.Forms.GroupBox cfgroup;
        private System.Windows.Forms.GroupBox cfg;
        private System.Windows.Forms.RadioButton seload;
        private System.Windows.Forms.RadioButton fuload;
        private System.Windows.Forms.GroupBox sjxm;
        private System.Windows.Forms.RadioButton bright;
        private System.Windows.Forms.RadioButton bank;
        private System.Windows.Forms.RadioButton ty;
        private System.Windows.Forms.CheckBox cfwconfig;
        private System.Windows.Forms.CheckBox checkrlt;
        private System.Windows.Forms.CheckBox checkBox_ignore9800;
        private System.Windows.Forms.CheckBox checkBox_SeperateLibProg;
        private System.Windows.Forms.GroupBox groupBox_350_Cfg;
        private System.Windows.Forms.Label label_tprog;
        private System.Windows.Forms.TextBox textBox_tprog;
        private System.Windows.Forms.Label label_tprog_unit;
        private System.Windows.Forms.TextBox textBox_terase;
        private System.Windows.Forms.Label label_terase;
        private System.Windows.Forms.Label label_terase_unit;
        private System.Windows.Forms.TextBox textBox_LibSize;
        private System.Windows.Forms.Label label_LibSizeUnit;
        private System.Windows.Forms.CheckBox rston;
        private System.Windows.Forms.TextBox textBox_CID;
        private System.Windows.Forms.Label label_CID;
        private System.Windows.Forms.GroupBox groupBox_CLB_CONTROL;
        private System.Windows.Forms.TextBox textBox_DRI_CLB;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label_AFI;
        private System.Windows.Forms.TextBox textBox_AFI;
        private System.Windows.Forms.Button button_Deselect_CLB;
        private System.Windows.Forms.Button button_REQB;
        private System.Windows.Forms.Button button_SlotMARKER;
        private System.Windows.Forms.Button button_ATTRIB;
        private System.Windows.Forms.Button button_PPS_CLB;
        private System.Windows.Forms.Button button_WUPB;
        private System.Windows.Forms.Button button_HLTB;
        private System.Windows.Forms.TextBox textBox_SlotNumber;
        private System.Windows.Forms.TextBox textBox_DSI_CLB;
        private System.Windows.Forms.Label label_Slots;
        private System.Windows.Forms.ComboBox comboBox_N;
        private System.Windows.Forms.CheckBox checkBox_ExtREQB;
        private System.Windows.Forms.Label label_PUPI;
        private System.Windows.Forms.TextBox textBox_PUPI;
        private System.Windows.Forms.Label label_parameters;
        private System.Windows.Forms.TextBox textBox_Parameters;
        private System.Windows.Forms.GroupBox groupBox_FM17Reg;
        private System.Windows.Forms.TextBox textBox_CRC;
        private System.Windows.Forms.Button button_CRC;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.CheckBox checkBox_CLB_EOF;
        private System.Windows.Forms.CheckBox checkBox_CLB_SOF;
        private System.Windows.Forms.ComboBox comboBox_CLB_EOF;
        private System.Windows.Forms.ComboBox comboBox_CLB_SOF;
        private System.Windows.Forms.ToolTip toolTip_CLIterface;
        private System.Windows.Forms.Button button_TypeBFraming;
        private System.Windows.Forms.Label label_EGT;
        private System.Windows.Forms.TextBox textBox_EGT;
        private System.Windows.Forms.Button button_AppendCRC16;
        private System.Windows.Forms.CheckBox CB_EnableOption;
        private System.Windows.Forms.Button button_ReverseOption;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.RadioButton FM302_radioButton;
        private System.Windows.Forms.RadioButton FM326_radioButton;
        private System.Windows.Forms.RadioButton r347;
        private System.Windows.Forms.RadioButton FM347_radioButton;
        private System.Windows.Forms.Button CT_347_STOPMCU;
        private System.Windows.Forms.RadioButton gm;
        private System.Windows.Forms.Button auto_freq_test;
        private System.Windows.Forms.Button buttonStringToASCII;
        private System.Windows.Forms.Button buttonASCIIToString;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.Label label_prepg_Calc;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.TextBox textBox_prepg;
        private System.Windows.Forms.Label label_terase_Calc;
        private System.Windows.Forms.Label label_tprog_Calc;
        private System.Windows.Forms.CheckBox retry;
        private System.Windows.Forms.CheckBox pre_prog;
        private System.Windows.Forms.ComboBox sec_addr;
        private System.Windows.Forms.TextBox text_0B_P3;
        private System.Windows.Forms.GroupBox ins54;
        private System.Windows.Forms.RadioButton wr3e;
        private System.Windows.Forms.RadioButton rd05;
        private System.Windows.Forms.RadioButton bank326;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.Label label83;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.Label label81;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.TextBox C3_3_textBox;
        private System.Windows.Forms.TextBox C2_3_textBox;
        private System.Windows.Forms.TextBox C1_3_textBox;
        private System.Windows.Forms.TextBox C3_2_textBox;
        private System.Windows.Forms.TextBox C2_2_textBox;
        private System.Windows.Forms.TextBox C1_2_textBox;
        private System.Windows.Forms.TextBox C3_1_textBox;
        private System.Windows.Forms.TextBox C2_1_textBox;
        private System.Windows.Forms.TextBox C1_1_textBox;
        private System.Windows.Forms.TextBox C3_0_textBox;
        private System.Windows.Forms.TextBox C2_0_textBox;
        private System.Windows.Forms.TextBox C1_0_textBox;
        private System.Windows.Forms.Button Cal_access_bits_button;
        private System.Windows.Forms.Button parse_access_bits_button;
        private System.Windows.Forms.TextBox Access_bits_textBox;
        private System.Windows.Forms.Button buttonRESTORE;
        private System.Windows.Forms.Button buttonTRANSFER;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton bkpr;
        private System.Windows.Forms.RadioButton bkpw;
        private System.Windows.Forms.Button bkp;
        private System.Windows.Forms.CheckBox CWD_CMP_checkBox;
        private System.Windows.Forms.Button FM336_Endurance_button;
        private System.Windows.Forms.RadioButton zc;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.GroupBox groupBox20;
        private System.Windows.Forms.Label label86;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.TextBox record_name;
        private System.Windows.Forms.TextBox record_interface;
        private System.Windows.Forms.Label label87;
        private System.Windows.Forms.TextBox record_card;
        private System.Windows.Forms.TextBox record_num;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.Button user_lock_button;
        private System.Windows.Forms.Button userfor_passport_button;
        private System.Windows.Forms.Button cfg_and_progfor_passport_button;
        private System.Windows.Forms.Button trimfor_passport_button;
        private System.Windows.Forms.Button trim8m;
        private System.Windows.Forms.CheckBox trim_wr;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.GroupBox groupBox21;
        private System.Windows.Forms.ComboBox aux_s;
        private System.Windows.Forms.Button Open_CMDlog_button;
        private System.Windows.Forms.Button CT_OPEN_CMDLOG_button;
        private System.Windows.Forms.Button Send_Slected_Command_button;
        private System.Windows.Forms.Button Send_Selected_Command_CL_button;
        private System.Windows.Forms.CheckBox M1_Visible_checkBox;
        private System.Windows.Forms.CheckBox VHBR_PPS_checkBox;
        private System.Windows.Forms.Button XOR_button;
        private System.Windows.Forms.Button btnReadUID;
        private System.Windows.Forms.Button btnWriteUID;
        private System.Windows.Forms.CheckBox wr_config;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton high_temp;
        private System.Windows.Forms.RadioButton low_temp;
        private System.Windows.Forms.Button temp_fm347;
        private System.Windows.Forms.CheckBox temp_retain;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox d_cos;
        private System.Windows.Forms.CheckBox auto_delay;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton cos;
        private System.Windows.Forms.RadioButton init;
        private System.Windows.Forms.Button btnWordXOR;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton cc_347;
        private System.Windows.Forms.RadioButton mp300_347;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox N_uid;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton cc_dip;
        private System.Windows.Forms.RadioButton cc_card;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox NVM_init;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox fm347_cos;
        private System.Windows.Forms.TextBox auth_uidlevel_textBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox NVM_encrypt;
        private System.Windows.Forms.CheckBox checkClockTrim;
        private System.Windows.Forms.Button cmdNBFaka;
        private System.Windows.Forms.CheckBox UID_modify;
    }
}

