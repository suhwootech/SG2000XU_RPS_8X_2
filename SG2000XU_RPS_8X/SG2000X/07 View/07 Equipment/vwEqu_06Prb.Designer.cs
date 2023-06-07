namespace SG2000X
{
    partial class vwEqu_06Prb
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose시 함수내의 모든 변수 해제
            _Release();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_06Prb));
            this.pnlPr_InrPort = new System.Windows.Forms.Panel();
            this.ckb_InAir = new System.Windows.Forms.CheckBox();
            this.ckbPr_InDn = new System.Windows.Forms.CheckBox();
            this.btn_InRead = new System.Windows.Forms.Button();
            this.btn_InOpen = new System.Windows.Forms.Button();
            this.btn_InClose = new System.Windows.Forms.Button();
            this.lb_PortInr = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_InPort = new System.Windows.Forms.ComboBox();
            this.cmb_InBaud = new System.Windows.Forms.ComboBox();
            this.lb_BaudInr = new System.Windows.Forms.Label();
            this.txtPr_InrLog = new System.Windows.Forms.RichTextBox();
            this.pnlPr_RPort = new System.Windows.Forms.Panel();
            this.R_Z_Pos = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txt_RY = new System.Windows.Forms.TextBox();
            this.lb_RY = new System.Windows.Forms.Label();
            this.txt_RX = new System.Windows.Forms.TextBox();
            this.lb_RX = new System.Windows.Forms.Label();
            this.txtU_RCalCheckCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtS_PrbTestCountR = new Extended.ExTextBox();
            this.txtS_UpErrRetryTmoutR = new Extended.ExTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtS_VacOffDelayR = new Extended.ExTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtS_AfterOffDelayR = new Extended.ExTextBox();
            this.txtS_VacOnTimeR = new Extended.ExTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_RCheck = new System.Windows.Forms.Button();
            this.ckb_RAir = new System.Windows.Forms.CheckBox();
            this.ckb_RDn = new System.Windows.Forms.CheckBox();
            this.lbl_RAmp = new System.Windows.Forms.Label();
            this.btn_RRepeat = new System.Windows.Forms.Button();
            this.btn_RAuto = new System.Windows.Forms.Button();
            this.btn_RRead = new System.Windows.Forms.Button();
            this.btn_ROpen = new System.Windows.Forms.Button();
            this.btn_RClose = new System.Windows.Forms.Button();
            this.lb_PortR = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_RPort = new System.Windows.Forms.ComboBox();
            this.cmb_RBaud = new System.Windows.Forms.ComboBox();
            this.lb_BaudR = new System.Windows.Forms.Label();
            this.txtPr_RLog = new System.Windows.Forms.RichTextBox();
            this.lb_BaudL = new System.Windows.Forms.Label();
            this.txtPr_LLog = new System.Windows.Forms.RichTextBox();
            this.cmb_LBaud = new System.Windows.Forms.ComboBox();
            this.lb_PortL = new System.Windows.Forms.Label();
            this.cmb_LPort = new System.Windows.Forms.ComboBox();
            this.label366 = new System.Windows.Forms.Label();
            this.btn_LClose = new System.Windows.Forms.Button();
            this.btn_LOpen = new System.Windows.Forms.Button();
            this.btn_LRead = new System.Windows.Forms.Button();
            this.btn_LAuto = new System.Windows.Forms.Button();
            this.btn_LRepeat = new System.Windows.Forms.Button();
            this.lbl_LAmp = new System.Windows.Forms.Label();
            this.ckb_LDn = new System.Windows.Forms.CheckBox();
            this.ckb_LAir = new System.Windows.Forms.CheckBox();
            this.btn_LCheck = new System.Windows.Forms.Button();
            this.btn_LPrbTestStop = new System.Windows.Forms.Button();
            this.lbl_VacOnTimeTitle = new System.Windows.Forms.Label();
            this.txt_VacOnTimeUnit = new System.Windows.Forms.Label();
            this.lbl_AfterOffDelayTitle = new System.Windows.Forms.Label();
            this.txt_AfterOffDelayUnit = new System.Windows.Forms.Label();
            this.lbl_VacOffDelayTitle = new System.Windows.Forms.Label();
            this.txt_VacOffDelayUnit = new System.Windows.Forms.Label();
            this.lbl_UpErrRetryTmout = new System.Windows.Forms.Label();
            this.lbl_UpErrRetryTmoutUnit = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtU_LCalCheckCount = new System.Windows.Forms.TextBox();
            this.pnlPr_LPort = new System.Windows.Forms.Panel();
            this.lbl_Z = new System.Windows.Forms.Label();
            this.lbl_Y = new System.Windows.Forms.Label();
            this.L_Z_Pos = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.btn_LPrbT1 = new System.Windows.Forms.Button();
            this.txt_LY = new System.Windows.Forms.TextBox();
            this.lb_LY = new System.Windows.Forms.Label();
            this.txt_LX = new System.Windows.Forms.TextBox();
            this.lb_LX = new System.Windows.Forms.Label();
            this.lbl_X = new System.Windows.Forms.Label();
            this.txtS_PrbTestCount = new Extended.ExTextBox();
            this.txtS_UpErrRetryTmout = new Extended.ExTextBox();
            this.txtS_VacOffDelay = new Extended.ExTextBox();
            this.txtS_AfterOffDelay = new Extended.ExTextBox();
            this.txtS_VacOnTime = new Extended.ExTextBox();
            this.pnlPr_InrPort.SuspendLayout();
            this.pnlPr_RPort.SuspendLayout();
            this.pnlPr_LPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPr_InrPort
            // 
            this.pnlPr_InrPort.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPr_InrPort.Controls.Add(this.ckb_InAir);
            this.pnlPr_InrPort.Controls.Add(this.ckbPr_InDn);
            this.pnlPr_InrPort.Controls.Add(this.btn_InRead);
            this.pnlPr_InrPort.Controls.Add(this.btn_InOpen);
            this.pnlPr_InrPort.Controls.Add(this.btn_InClose);
            this.pnlPr_InrPort.Controls.Add(this.lb_PortInr);
            this.pnlPr_InrPort.Controls.Add(this.label2);
            this.pnlPr_InrPort.Controls.Add(this.cmb_InPort);
            this.pnlPr_InrPort.Controls.Add(this.cmb_InBaud);
            this.pnlPr_InrPort.Controls.Add(this.lb_BaudInr);
            this.pnlPr_InrPort.Controls.Add(this.txtPr_InrLog);
            resources.ApplyResources(this.pnlPr_InrPort, "pnlPr_InrPort");
            this.pnlPr_InrPort.Name = "pnlPr_InrPort";
            // 
            // ckb_InAir
            // 
            resources.ApplyResources(this.ckb_InAir, "ckb_InAir");
            this.ckb_InAir.BackColor = System.Drawing.Color.White;
            this.ckb_InAir.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_InAir.FlatAppearance.BorderSize = 2;
            this.ckb_InAir.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_InAir.Name = "ckb_InAir";
            this.ckb_InAir.Tag = "31";
            this.ckb_InAir.UseVisualStyleBackColor = false;
            this.ckb_InAir.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckbPr_InDn
            // 
            resources.ApplyResources(this.ckbPr_InDn, "ckbPr_InDn");
            this.ckbPr_InDn.BackColor = System.Drawing.Color.White;
            this.ckbPr_InDn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbPr_InDn.FlatAppearance.BorderSize = 2;
            this.ckbPr_InDn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbPr_InDn.Name = "ckbPr_InDn";
            this.ckbPr_InDn.Tag = "30";
            this.ckbPr_InDn.UseVisualStyleBackColor = false;
            this.ckbPr_InDn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // btn_InRead
            // 
            this.btn_InRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_InRead.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_InRead, "btn_InRead");
            this.btn_InRead.ForeColor = System.Drawing.Color.White;
            this.btn_InRead.Name = "btn_InRead";
            this.btn_InRead.Tag = "0";
            this.btn_InRead.UseVisualStyleBackColor = false;
            this.btn_InRead.Click += new System.EventHandler(this.btnPr_InrRead_Click);
            // 
            // btn_InOpen
            // 
            this.btn_InOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_InOpen.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_InOpen, "btn_InOpen");
            this.btn_InOpen.ForeColor = System.Drawing.Color.White;
            this.btn_InOpen.Name = "btn_InOpen";
            this.btn_InOpen.Tag = "2";
            this.btn_InOpen.UseVisualStyleBackColor = false;
            this.btn_InOpen.Click += new System.EventHandler(this.btnPr_Open_Click);
            // 
            // btn_InClose
            // 
            this.btn_InClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_InClose.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_InClose, "btn_InClose");
            this.btn_InClose.ForeColor = System.Drawing.Color.White;
            this.btn_InClose.Name = "btn_InClose";
            this.btn_InClose.Tag = "2";
            this.btn_InClose.UseVisualStyleBackColor = false;
            this.btn_InClose.Click += new System.EventHandler(this.btnPr_Close_Click);
            // 
            // lb_PortInr
            // 
            resources.ApplyResources(this.lb_PortInr, "lb_PortInr");
            this.lb_PortInr.Name = "lb_PortInr";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // cmb_InPort
            // 
            this.cmb_InPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_InPort.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_InPort, "cmb_InPort");
            this.cmb_InPort.Name = "cmb_InPort";
            // 
            // cmb_InBaud
            // 
            this.cmb_InBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_InBaud.FormattingEnabled = true;
            this.cmb_InBaud.Items.AddRange(new object[] {
            resources.GetString("cmb_InBaud.Items"),
            resources.GetString("cmb_InBaud.Items1"),
            resources.GetString("cmb_InBaud.Items2"),
            resources.GetString("cmb_InBaud.Items3"),
            resources.GetString("cmb_InBaud.Items4"),
            resources.GetString("cmb_InBaud.Items5"),
            resources.GetString("cmb_InBaud.Items6"),
            resources.GetString("cmb_InBaud.Items7"),
            resources.GetString("cmb_InBaud.Items8"),
            resources.GetString("cmb_InBaud.Items9"),
            resources.GetString("cmb_InBaud.Items10"),
            resources.GetString("cmb_InBaud.Items11"),
            resources.GetString("cmb_InBaud.Items12"),
            resources.GetString("cmb_InBaud.Items13")});
            resources.ApplyResources(this.cmb_InBaud, "cmb_InBaud");
            this.cmb_InBaud.Name = "cmb_InBaud";
            // 
            // lb_BaudInr
            // 
            resources.ApplyResources(this.lb_BaudInr, "lb_BaudInr");
            this.lb_BaudInr.Name = "lb_BaudInr";
            // 
            // txtPr_InrLog
            // 
            this.txtPr_InrLog.BackColor = System.Drawing.Color.LightGray;
            this.txtPr_InrLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.txtPr_InrLog, "txtPr_InrLog");
            this.txtPr_InrLog.Name = "txtPr_InrLog";
            this.txtPr_InrLog.ReadOnly = true;
            // 
            // pnlPr_RPort
            // 
            this.pnlPr_RPort.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPr_RPort.Controls.Add(this.R_Z_Pos);
            this.pnlPr_RPort.Controls.Add(this.button4);
            this.pnlPr_RPort.Controls.Add(this.label16);
            this.pnlPr_RPort.Controls.Add(this.btn_Save);
            this.pnlPr_RPort.Controls.Add(this.button2);
            this.pnlPr_RPort.Controls.Add(this.txt_RY);
            this.pnlPr_RPort.Controls.Add(this.lb_RY);
            this.pnlPr_RPort.Controls.Add(this.txt_RX);
            this.pnlPr_RPort.Controls.Add(this.lb_RX);
            this.pnlPr_RPort.Controls.Add(this.txtU_RCalCheckCount);
            this.pnlPr_RPort.Controls.Add(this.label4);
            this.pnlPr_RPort.Controls.Add(this.label6);
            this.pnlPr_RPort.Controls.Add(this.txtS_PrbTestCountR);
            this.pnlPr_RPort.Controls.Add(this.txtS_UpErrRetryTmoutR);
            this.pnlPr_RPort.Controls.Add(this.label7);
            this.pnlPr_RPort.Controls.Add(this.label8);
            this.pnlPr_RPort.Controls.Add(this.txtS_VacOffDelayR);
            this.pnlPr_RPort.Controls.Add(this.label9);
            this.pnlPr_RPort.Controls.Add(this.label10);
            this.pnlPr_RPort.Controls.Add(this.txtS_AfterOffDelayR);
            this.pnlPr_RPort.Controls.Add(this.txtS_VacOnTimeR);
            this.pnlPr_RPort.Controls.Add(this.label11);
            this.pnlPr_RPort.Controls.Add(this.label12);
            this.pnlPr_RPort.Controls.Add(this.label13);
            this.pnlPr_RPort.Controls.Add(this.label14);
            this.pnlPr_RPort.Controls.Add(this.button1);
            this.pnlPr_RPort.Controls.Add(this.btn_RCheck);
            this.pnlPr_RPort.Controls.Add(this.ckb_RAir);
            this.pnlPr_RPort.Controls.Add(this.ckb_RDn);
            this.pnlPr_RPort.Controls.Add(this.lbl_RAmp);
            this.pnlPr_RPort.Controls.Add(this.btn_RRepeat);
            this.pnlPr_RPort.Controls.Add(this.btn_RAuto);
            this.pnlPr_RPort.Controls.Add(this.btn_RRead);
            this.pnlPr_RPort.Controls.Add(this.btn_ROpen);
            this.pnlPr_RPort.Controls.Add(this.btn_RClose);
            this.pnlPr_RPort.Controls.Add(this.lb_PortR);
            this.pnlPr_RPort.Controls.Add(this.label1);
            this.pnlPr_RPort.Controls.Add(this.cmb_RPort);
            this.pnlPr_RPort.Controls.Add(this.cmb_RBaud);
            this.pnlPr_RPort.Controls.Add(this.lb_BaudR);
            this.pnlPr_RPort.Controls.Add(this.txtPr_RLog);
            resources.ApplyResources(this.pnlPr_RPort, "pnlPr_RPort");
            this.pnlPr_RPort.Name = "pnlPr_RPort";
            // 
            // R_Z_Pos
            // 
            this.R_Z_Pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.R_Z_Pos, "R_Z_Pos");
            this.R_Z_Pos.Name = "R_Z_Pos";
            this.R_Z_Pos.Tag = "0";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.DarkGreen;
            this.button4.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button4, "button4");
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Name = "button4";
            this.button4.Tag = "1";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.btn_RPrbmT1_Click);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Name = "label16";
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Save.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Tag = "0";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btnPr_Save_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkGreen;
            this.button2.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button2, "button2");
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Name = "button2";
            this.button2.Tag = "1";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txt_RY
            // 
            this.txt_RY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_RY, "txt_RY");
            this.txt_RY.Name = "txt_RY";
            this.txt_RY.Tag = "0";
            // 
            // lb_RY
            // 
            resources.ApplyResources(this.lb_RY, "lb_RY");
            this.lb_RY.ForeColor = System.Drawing.Color.Black;
            this.lb_RY.Name = "lb_RY";
            // 
            // txt_RX
            // 
            this.txt_RX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_RX, "txt_RX");
            this.txt_RX.Name = "txt_RX";
            this.txt_RX.Tag = "0";
            // 
            // lb_RX
            // 
            resources.ApplyResources(this.lb_RX, "lb_RX");
            this.lb_RX.ForeColor = System.Drawing.Color.Black;
            this.lb_RX.Name = "lb_RX";
            // 
            // txtU_RCalCheckCount
            // 
            resources.ApplyResources(this.txtU_RCalCheckCount, "txtU_RCalCheckCount");
            this.txtU_RCalCheckCount.Name = "txtU_RCalCheckCount";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // txtS_PrbTestCountR
            // 
            this.txtS_PrbTestCountR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_PrbTestCountR.ExMax = 9999999D;
            this.txtS_PrbTestCountR.ExMin = 1D;
            this.txtS_PrbTestCountR.ExSigned = Extended.ExSign.Signed;
            this.txtS_PrbTestCountR.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_PrbTestCountR, "txtS_PrbTestCountR");
            this.txtS_PrbTestCountR.Name = "txtS_PrbTestCountR";
            this.txtS_PrbTestCountR.Tag = "0";
            // 
            // txtS_UpErrRetryTmoutR
            // 
            this.txtS_UpErrRetryTmoutR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_UpErrRetryTmoutR.ExMax = 99999D;
            this.txtS_UpErrRetryTmoutR.ExMin = 0D;
            this.txtS_UpErrRetryTmoutR.ExSigned = Extended.ExSign.Signed;
            this.txtS_UpErrRetryTmoutR.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_UpErrRetryTmoutR, "txtS_UpErrRetryTmoutR");
            this.txtS_UpErrRetryTmoutR.Name = "txtS_UpErrRetryTmoutR";
            this.txtS_UpErrRetryTmoutR.Tag = "0";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtS_VacOffDelayR
            // 
            this.txtS_VacOffDelayR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_VacOffDelayR.ExMax = 9999D;
            this.txtS_VacOffDelayR.ExMin = 0D;
            this.txtS_VacOffDelayR.ExSigned = Extended.ExSign.Signed;
            this.txtS_VacOffDelayR.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_VacOffDelayR, "txtS_VacOffDelayR");
            this.txtS_VacOffDelayR.Name = "txtS_VacOffDelayR";
            this.txtS_VacOffDelayR.Tag = "0";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // txtS_AfterOffDelayR
            // 
            this.txtS_AfterOffDelayR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_AfterOffDelayR.ExMax = 9999D;
            this.txtS_AfterOffDelayR.ExMin = 0D;
            this.txtS_AfterOffDelayR.ExSigned = Extended.ExSign.Signed;
            this.txtS_AfterOffDelayR.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_AfterOffDelayR, "txtS_AfterOffDelayR");
            this.txtS_AfterOffDelayR.Name = "txtS_AfterOffDelayR";
            this.txtS_AfterOffDelayR.Tag = "0";
            // 
            // txtS_VacOnTimeR
            // 
            this.txtS_VacOnTimeR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_VacOnTimeR.ExMax = 9999D;
            this.txtS_VacOnTimeR.ExMin = 0D;
            this.txtS_VacOnTimeR.ExSigned = Extended.ExSign.Signed;
            this.txtS_VacOnTimeR.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_VacOnTimeR, "txtS_VacOnTimeR");
            this.txtS_VacOnTimeR.Name = "txtS_VacOnTimeR";
            this.txtS_VacOnTimeR.Tag = "0";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Crimson;
            this.button1.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button1, "button1");
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Name = "button1";
            this.button1.Tag = "0";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_RCheck
            // 
            this.btn_RCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_RCheck.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_RCheck, "btn_RCheck");
            this.btn_RCheck.ForeColor = System.Drawing.Color.White;
            this.btn_RCheck.Name = "btn_RCheck";
            this.btn_RCheck.Tag = "GRR_Probe_Calibration_Check";
            this.btn_RCheck.UseVisualStyleBackColor = false;
            this.btn_RCheck.Click += new System.EventHandler(this.btnPr_RCheck_Click);
            // 
            // ckb_RAir
            // 
            resources.ApplyResources(this.ckb_RAir, "ckb_RAir");
            this.ckb_RAir.BackColor = System.Drawing.Color.White;
            this.ckb_RAir.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_RAir.FlatAppearance.BorderSize = 2;
            this.ckb_RAir.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_RAir.Name = "ckb_RAir";
            this.ckb_RAir.Tag = "81";
            this.ckb_RAir.UseVisualStyleBackColor = false;
            this.ckb_RAir.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckb_RDn
            // 
            resources.ApplyResources(this.ckb_RDn, "ckb_RDn");
            this.ckb_RDn.BackColor = System.Drawing.Color.White;
            this.ckb_RDn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_RDn.FlatAppearance.BorderSize = 2;
            this.ckb_RDn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_RDn.Name = "ckb_RDn";
            this.ckb_RDn.Tag = "80";
            this.ckb_RDn.UseVisualStyleBackColor = false;
            this.ckb_RDn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // lbl_RAmp
            // 
            this.lbl_RAmp.BackColor = System.Drawing.Color.LightGray;
            this.lbl_RAmp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_RAmp, "lbl_RAmp");
            this.lbl_RAmp.Name = "lbl_RAmp";
            // 
            // btn_RRepeat
            // 
            this.btn_RRepeat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_RRepeat.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_RRepeat, "btn_RRepeat");
            this.btn_RRepeat.ForeColor = System.Drawing.Color.White;
            this.btn_RRepeat.Name = "btn_RRepeat";
            this.btn_RRepeat.Tag = "0";
            this.btn_RRepeat.UseVisualStyleBackColor = false;
            this.btn_RRepeat.Click += new System.EventHandler(this.btn_RRepeat_Click);
            // 
            // btn_RAuto
            // 
            this.btn_RAuto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_RAuto.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_RAuto, "btn_RAuto");
            this.btn_RAuto.ForeColor = System.Drawing.Color.White;
            this.btn_RAuto.Name = "btn_RAuto";
            this.btn_RAuto.Tag = "GRR_Probe_Auto_Calibration";
            this.btn_RAuto.UseVisualStyleBackColor = false;
            this.btn_RAuto.Click += new System.EventHandler(this.btnPr_RAuto_Click);
            // 
            // btn_RRead
            // 
            this.btn_RRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_RRead.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_RRead, "btn_RRead");
            this.btn_RRead.ForeColor = System.Drawing.Color.White;
            this.btn_RRead.Name = "btn_RRead";
            this.btn_RRead.Tag = "0";
            this.btn_RRead.UseVisualStyleBackColor = false;
            this.btn_RRead.Click += new System.EventHandler(this.btnPr_RRead_Click);
            // 
            // btn_ROpen
            // 
            this.btn_ROpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_ROpen.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_ROpen, "btn_ROpen");
            this.btn_ROpen.ForeColor = System.Drawing.Color.White;
            this.btn_ROpen.Name = "btn_ROpen";
            this.btn_ROpen.Tag = "1";
            this.btn_ROpen.UseVisualStyleBackColor = false;
            this.btn_ROpen.Click += new System.EventHandler(this.btnPr_Open_Click);
            // 
            // btn_RClose
            // 
            this.btn_RClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_RClose.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_RClose, "btn_RClose");
            this.btn_RClose.ForeColor = System.Drawing.Color.White;
            this.btn_RClose.Name = "btn_RClose";
            this.btn_RClose.Tag = "1";
            this.btn_RClose.UseVisualStyleBackColor = false;
            this.btn_RClose.Click += new System.EventHandler(this.btnPr_Close_Click);
            // 
            // lb_PortR
            // 
            resources.ApplyResources(this.lb_PortR, "lb_PortR");
            this.lb_PortR.Name = "lb_PortR";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // cmb_RPort
            // 
            this.cmb_RPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_RPort.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_RPort, "cmb_RPort");
            this.cmb_RPort.Name = "cmb_RPort";
            // 
            // cmb_RBaud
            // 
            this.cmb_RBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_RBaud.FormattingEnabled = true;
            this.cmb_RBaud.Items.AddRange(new object[] {
            resources.GetString("cmb_RBaud.Items"),
            resources.GetString("cmb_RBaud.Items1"),
            resources.GetString("cmb_RBaud.Items2"),
            resources.GetString("cmb_RBaud.Items3"),
            resources.GetString("cmb_RBaud.Items4"),
            resources.GetString("cmb_RBaud.Items5"),
            resources.GetString("cmb_RBaud.Items6"),
            resources.GetString("cmb_RBaud.Items7"),
            resources.GetString("cmb_RBaud.Items8"),
            resources.GetString("cmb_RBaud.Items9"),
            resources.GetString("cmb_RBaud.Items10"),
            resources.GetString("cmb_RBaud.Items11"),
            resources.GetString("cmb_RBaud.Items12"),
            resources.GetString("cmb_RBaud.Items13")});
            resources.ApplyResources(this.cmb_RBaud, "cmb_RBaud");
            this.cmb_RBaud.Name = "cmb_RBaud";
            // 
            // lb_BaudR
            // 
            resources.ApplyResources(this.lb_BaudR, "lb_BaudR");
            this.lb_BaudR.Name = "lb_BaudR";
            // 
            // txtPr_RLog
            // 
            this.txtPr_RLog.BackColor = System.Drawing.Color.LightGray;
            this.txtPr_RLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.txtPr_RLog, "txtPr_RLog");
            this.txtPr_RLog.Name = "txtPr_RLog";
            this.txtPr_RLog.ReadOnly = true;
            // 
            // lb_BaudL
            // 
            resources.ApplyResources(this.lb_BaudL, "lb_BaudL");
            this.lb_BaudL.Name = "lb_BaudL";
            // 
            // txtPr_LLog
            // 
            this.txtPr_LLog.BackColor = System.Drawing.Color.LightGray;
            this.txtPr_LLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.txtPr_LLog, "txtPr_LLog");
            this.txtPr_LLog.Name = "txtPr_LLog";
            this.txtPr_LLog.ReadOnly = true;
            // 
            // cmb_LBaud
            // 
            this.cmb_LBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_LBaud.FormattingEnabled = true;
            this.cmb_LBaud.Items.AddRange(new object[] {
            resources.GetString("cmb_LBaud.Items"),
            resources.GetString("cmb_LBaud.Items1"),
            resources.GetString("cmb_LBaud.Items2"),
            resources.GetString("cmb_LBaud.Items3"),
            resources.GetString("cmb_LBaud.Items4"),
            resources.GetString("cmb_LBaud.Items5"),
            resources.GetString("cmb_LBaud.Items6"),
            resources.GetString("cmb_LBaud.Items7"),
            resources.GetString("cmb_LBaud.Items8"),
            resources.GetString("cmb_LBaud.Items9"),
            resources.GetString("cmb_LBaud.Items10"),
            resources.GetString("cmb_LBaud.Items11"),
            resources.GetString("cmb_LBaud.Items12"),
            resources.GetString("cmb_LBaud.Items13")});
            resources.ApplyResources(this.cmb_LBaud, "cmb_LBaud");
            this.cmb_LBaud.Name = "cmb_LBaud";
            // 
            // lb_PortL
            // 
            resources.ApplyResources(this.lb_PortL, "lb_PortL");
            this.lb_PortL.Name = "lb_PortL";
            // 
            // cmb_LPort
            // 
            this.cmb_LPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_LPort.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_LPort, "cmb_LPort");
            this.cmb_LPort.Name = "cmb_LPort";
            // 
            // label366
            // 
            this.label366.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label366, "label366");
            this.label366.ForeColor = System.Drawing.Color.White;
            this.label366.Name = "label366";
            // 
            // btn_LClose
            // 
            this.btn_LClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_LClose.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LClose, "btn_LClose");
            this.btn_LClose.ForeColor = System.Drawing.Color.White;
            this.btn_LClose.Name = "btn_LClose";
            this.btn_LClose.Tag = "0";
            this.btn_LClose.UseVisualStyleBackColor = false;
            this.btn_LClose.Click += new System.EventHandler(this.btnPr_Close_Click);
            // 
            // btn_LOpen
            // 
            this.btn_LOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_LOpen.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LOpen, "btn_LOpen");
            this.btn_LOpen.ForeColor = System.Drawing.Color.White;
            this.btn_LOpen.Name = "btn_LOpen";
            this.btn_LOpen.Tag = "0";
            this.btn_LOpen.UseVisualStyleBackColor = false;
            this.btn_LOpen.Click += new System.EventHandler(this.btnPr_Open_Click);
            // 
            // btn_LRead
            // 
            this.btn_LRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_LRead.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LRead, "btn_LRead");
            this.btn_LRead.ForeColor = System.Drawing.Color.White;
            this.btn_LRead.Name = "btn_LRead";
            this.btn_LRead.Tag = "0";
            this.btn_LRead.UseVisualStyleBackColor = false;
            this.btn_LRead.Click += new System.EventHandler(this.btnPr_LRead_Click);
            // 
            // btn_LAuto
            // 
            this.btn_LAuto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_LAuto.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LAuto, "btn_LAuto");
            this.btn_LAuto.ForeColor = System.Drawing.Color.White;
            this.btn_LAuto.Name = "btn_LAuto";
            this.btn_LAuto.Tag = "GRL_Probe_Auto_Calibration";
            this.btn_LAuto.UseVisualStyleBackColor = false;
            this.btn_LAuto.Click += new System.EventHandler(this.btnPr_LAuto_Click);
            // 
            // btn_LRepeat
            // 
            this.btn_LRepeat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_LRepeat.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LRepeat, "btn_LRepeat");
            this.btn_LRepeat.ForeColor = System.Drawing.Color.White;
            this.btn_LRepeat.Name = "btn_LRepeat";
            this.btn_LRepeat.Tag = "0";
            this.btn_LRepeat.UseVisualStyleBackColor = false;
            this.btn_LRepeat.Click += new System.EventHandler(this.btn_LRepeat_Click);
            // 
            // lbl_LAmp
            // 
            this.lbl_LAmp.BackColor = System.Drawing.Color.LightGray;
            this.lbl_LAmp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_LAmp, "lbl_LAmp");
            this.lbl_LAmp.Name = "lbl_LAmp";
            // 
            // ckb_LDn
            // 
            resources.ApplyResources(this.ckb_LDn, "ckb_LDn");
            this.ckb_LDn.BackColor = System.Drawing.Color.White;
            this.ckb_LDn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_LDn.FlatAppearance.BorderSize = 2;
            this.ckb_LDn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_LDn.Name = "ckb_LDn";
            this.ckb_LDn.Tag = "56";
            this.ckb_LDn.UseVisualStyleBackColor = false;
            this.ckb_LDn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckb_LAir
            // 
            resources.ApplyResources(this.ckb_LAir, "ckb_LAir");
            this.ckb_LAir.BackColor = System.Drawing.Color.White;
            this.ckb_LAir.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_LAir.FlatAppearance.BorderSize = 2;
            this.ckb_LAir.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_LAir.Name = "ckb_LAir";
            this.ckb_LAir.Tag = "57";
            this.ckb_LAir.UseVisualStyleBackColor = false;
            this.ckb_LAir.Click += new System.EventHandler(this.ckb_Click);
            // 
            // btn_LCheck
            // 
            this.btn_LCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_LCheck.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LCheck, "btn_LCheck");
            this.btn_LCheck.ForeColor = System.Drawing.Color.White;
            this.btn_LCheck.Name = "btn_LCheck";
            this.btn_LCheck.Tag = "GRL_Probe_Calibration_Check";
            this.btn_LCheck.UseVisualStyleBackColor = false;
            this.btn_LCheck.Click += new System.EventHandler(this.btnPr_LCheck_Click);
            // 
            // btn_LPrbTestStop
            // 
            this.btn_LPrbTestStop.BackColor = System.Drawing.Color.Crimson;
            this.btn_LPrbTestStop.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LPrbTestStop, "btn_LPrbTestStop");
            this.btn_LPrbTestStop.ForeColor = System.Drawing.Color.White;
            this.btn_LPrbTestStop.Name = "btn_LPrbTestStop";
            this.btn_LPrbTestStop.Tag = "0";
            this.btn_LPrbTestStop.UseVisualStyleBackColor = false;
            this.btn_LPrbTestStop.Click += new System.EventHandler(this.btn_LPrbTestStop_Click);
            // 
            // lbl_VacOnTimeTitle
            // 
            resources.ApplyResources(this.lbl_VacOnTimeTitle, "lbl_VacOnTimeTitle");
            this.lbl_VacOnTimeTitle.Name = "lbl_VacOnTimeTitle";
            // 
            // txt_VacOnTimeUnit
            // 
            resources.ApplyResources(this.txt_VacOnTimeUnit, "txt_VacOnTimeUnit");
            this.txt_VacOnTimeUnit.ForeColor = System.Drawing.Color.Black;
            this.txt_VacOnTimeUnit.Name = "txt_VacOnTimeUnit";
            // 
            // lbl_AfterOffDelayTitle
            // 
            resources.ApplyResources(this.lbl_AfterOffDelayTitle, "lbl_AfterOffDelayTitle");
            this.lbl_AfterOffDelayTitle.Name = "lbl_AfterOffDelayTitle";
            // 
            // txt_AfterOffDelayUnit
            // 
            resources.ApplyResources(this.txt_AfterOffDelayUnit, "txt_AfterOffDelayUnit");
            this.txt_AfterOffDelayUnit.ForeColor = System.Drawing.Color.Black;
            this.txt_AfterOffDelayUnit.Name = "txt_AfterOffDelayUnit";
            // 
            // lbl_VacOffDelayTitle
            // 
            resources.ApplyResources(this.lbl_VacOffDelayTitle, "lbl_VacOffDelayTitle");
            this.lbl_VacOffDelayTitle.Name = "lbl_VacOffDelayTitle";
            // 
            // txt_VacOffDelayUnit
            // 
            resources.ApplyResources(this.txt_VacOffDelayUnit, "txt_VacOffDelayUnit");
            this.txt_VacOffDelayUnit.ForeColor = System.Drawing.Color.Black;
            this.txt_VacOffDelayUnit.Name = "txt_VacOffDelayUnit";
            // 
            // lbl_UpErrRetryTmout
            // 
            resources.ApplyResources(this.lbl_UpErrRetryTmout, "lbl_UpErrRetryTmout");
            this.lbl_UpErrRetryTmout.Name = "lbl_UpErrRetryTmout";
            // 
            // lbl_UpErrRetryTmoutUnit
            // 
            resources.ApplyResources(this.lbl_UpErrRetryTmoutUnit, "lbl_UpErrRetryTmoutUnit");
            this.lbl_UpErrRetryTmoutUnit.ForeColor = System.Drawing.Color.Black;
            this.lbl_UpErrRetryTmoutUnit.Name = "lbl_UpErrRetryTmoutUnit";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtU_LCalCheckCount
            // 
            resources.ApplyResources(this.txtU_LCalCheckCount, "txtU_LCalCheckCount");
            this.txtU_LCalCheckCount.Name = "txtU_LCalCheckCount";
            // 
            // pnlPr_LPort
            // 
            this.pnlPr_LPort.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPr_LPort.Controls.Add(this.lbl_Z);
            this.pnlPr_LPort.Controls.Add(this.lbl_Y);
            this.pnlPr_LPort.Controls.Add(this.L_Z_Pos);
            this.pnlPr_LPort.Controls.Add(this.label15);
            this.pnlPr_LPort.Controls.Add(this.button3);
            this.pnlPr_LPort.Controls.Add(this.btn_LPrbT1);
            this.pnlPr_LPort.Controls.Add(this.txt_LY);
            this.pnlPr_LPort.Controls.Add(this.lb_LY);
            this.pnlPr_LPort.Controls.Add(this.txt_LX);
            this.pnlPr_LPort.Controls.Add(this.lb_LX);
            this.pnlPr_LPort.Controls.Add(this.txtU_LCalCheckCount);
            this.pnlPr_LPort.Controls.Add(this.lbl_X);
            this.pnlPr_LPort.Controls.Add(this.label3);
            this.pnlPr_LPort.Controls.Add(this.label5);
            this.pnlPr_LPort.Controls.Add(this.txtS_PrbTestCount);
            this.pnlPr_LPort.Controls.Add(this.txtS_UpErrRetryTmout);
            this.pnlPr_LPort.Controls.Add(this.lbl_UpErrRetryTmoutUnit);
            this.pnlPr_LPort.Controls.Add(this.lbl_UpErrRetryTmout);
            this.pnlPr_LPort.Controls.Add(this.txtS_VacOffDelay);
            this.pnlPr_LPort.Controls.Add(this.txt_VacOffDelayUnit);
            this.pnlPr_LPort.Controls.Add(this.lbl_VacOffDelayTitle);
            this.pnlPr_LPort.Controls.Add(this.txtS_AfterOffDelay);
            this.pnlPr_LPort.Controls.Add(this.txtS_VacOnTime);
            this.pnlPr_LPort.Controls.Add(this.txt_AfterOffDelayUnit);
            this.pnlPr_LPort.Controls.Add(this.lbl_AfterOffDelayTitle);
            this.pnlPr_LPort.Controls.Add(this.txt_VacOnTimeUnit);
            this.pnlPr_LPort.Controls.Add(this.lbl_VacOnTimeTitle);
            this.pnlPr_LPort.Controls.Add(this.btn_LPrbTestStop);
            this.pnlPr_LPort.Controls.Add(this.btn_LCheck);
            this.pnlPr_LPort.Controls.Add(this.ckb_LAir);
            this.pnlPr_LPort.Controls.Add(this.ckb_LDn);
            this.pnlPr_LPort.Controls.Add(this.lbl_LAmp);
            this.pnlPr_LPort.Controls.Add(this.btn_LRepeat);
            this.pnlPr_LPort.Controls.Add(this.btn_LAuto);
            this.pnlPr_LPort.Controls.Add(this.btn_LRead);
            this.pnlPr_LPort.Controls.Add(this.btn_LOpen);
            this.pnlPr_LPort.Controls.Add(this.btn_LClose);
            this.pnlPr_LPort.Controls.Add(this.label366);
            this.pnlPr_LPort.Controls.Add(this.cmb_LPort);
            this.pnlPr_LPort.Controls.Add(this.lb_PortL);
            this.pnlPr_LPort.Controls.Add(this.cmb_LBaud);
            this.pnlPr_LPort.Controls.Add(this.txtPr_LLog);
            this.pnlPr_LPort.Controls.Add(this.lb_BaudL);
            resources.ApplyResources(this.pnlPr_LPort, "pnlPr_LPort");
            this.pnlPr_LPort.Name = "pnlPr_LPort";
            // 
            // lbl_Z
            // 
            resources.ApplyResources(this.lbl_Z, "lbl_Z");
            this.lbl_Z.Name = "lbl_Z";
            // 
            // lbl_Y
            // 
            resources.ApplyResources(this.lbl_Y, "lbl_Y");
            this.lbl_Y.Name = "lbl_Y";
            // 
            // L_Z_Pos
            // 
            this.L_Z_Pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.L_Z_Pos, "L_Z_Pos");
            this.L_Z_Pos.Name = "L_Z_Pos";
            this.L_Z_Pos.Tag = "0";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Name = "label15";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.DarkGreen;
            this.button3.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button3, "button3");
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Name = "button3";
            this.button3.Tag = "1";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.btn_LPrbmT1_Click);
            // 
            // btn_LPrbT1
            // 
            this.btn_LPrbT1.BackColor = System.Drawing.Color.DarkGreen;
            this.btn_LPrbT1.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_LPrbT1, "btn_LPrbT1");
            this.btn_LPrbT1.ForeColor = System.Drawing.Color.White;
            this.btn_LPrbT1.Name = "btn_LPrbT1";
            this.btn_LPrbT1.Tag = "1";
            this.btn_LPrbT1.UseVisualStyleBackColor = false;
            this.btn_LPrbT1.Click += new System.EventHandler(this.btn_LPrbT1_Click);
            // 
            // txt_LY
            // 
            this.txt_LY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_LY, "txt_LY");
            this.txt_LY.Name = "txt_LY";
            this.txt_LY.Tag = "0";
            // 
            // lb_LY
            // 
            resources.ApplyResources(this.lb_LY, "lb_LY");
            this.lb_LY.ForeColor = System.Drawing.Color.Black;
            this.lb_LY.Name = "lb_LY";
            // 
            // txt_LX
            // 
            this.txt_LX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_LX, "txt_LX");
            this.txt_LX.Name = "txt_LX";
            this.txt_LX.Tag = "0";
            // 
            // lb_LX
            // 
            resources.ApplyResources(this.lb_LX, "lb_LX");
            this.lb_LX.ForeColor = System.Drawing.Color.Black;
            this.lb_LX.Name = "lb_LX";
            // 
            // lbl_X
            // 
            resources.ApplyResources(this.lbl_X, "lbl_X");
            this.lbl_X.Name = "lbl_X";
            // 
            // txtS_PrbTestCount
            // 
            this.txtS_PrbTestCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_PrbTestCount.ExMax = 9999999D;
            this.txtS_PrbTestCount.ExMin = 1D;
            this.txtS_PrbTestCount.ExSigned = Extended.ExSign.Signed;
            this.txtS_PrbTestCount.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_PrbTestCount, "txtS_PrbTestCount");
            this.txtS_PrbTestCount.Name = "txtS_PrbTestCount";
            this.txtS_PrbTestCount.Tag = "0";
            // 
            // txtS_UpErrRetryTmout
            // 
            this.txtS_UpErrRetryTmout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_UpErrRetryTmout.ExMax = 99999D;
            this.txtS_UpErrRetryTmout.ExMin = 0D;
            this.txtS_UpErrRetryTmout.ExSigned = Extended.ExSign.Signed;
            this.txtS_UpErrRetryTmout.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_UpErrRetryTmout, "txtS_UpErrRetryTmout");
            this.txtS_UpErrRetryTmout.Name = "txtS_UpErrRetryTmout";
            this.txtS_UpErrRetryTmout.Tag = "0";
            // 
            // txtS_VacOffDelay
            // 
            this.txtS_VacOffDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_VacOffDelay.ExMax = 9999D;
            this.txtS_VacOffDelay.ExMin = 0D;
            this.txtS_VacOffDelay.ExSigned = Extended.ExSign.Signed;
            this.txtS_VacOffDelay.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_VacOffDelay, "txtS_VacOffDelay");
            this.txtS_VacOffDelay.Name = "txtS_VacOffDelay";
            this.txtS_VacOffDelay.Tag = "0";
            // 
            // txtS_AfterOffDelay
            // 
            this.txtS_AfterOffDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_AfterOffDelay.ExMax = 9999D;
            this.txtS_AfterOffDelay.ExMin = 0D;
            this.txtS_AfterOffDelay.ExSigned = Extended.ExSign.Signed;
            this.txtS_AfterOffDelay.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_AfterOffDelay, "txtS_AfterOffDelay");
            this.txtS_AfterOffDelay.Name = "txtS_AfterOffDelay";
            this.txtS_AfterOffDelay.Tag = "0";
            // 
            // txtS_VacOnTime
            // 
            this.txtS_VacOnTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtS_VacOnTime.ExMax = 9999D;
            this.txtS_VacOnTime.ExMin = 0D;
            this.txtS_VacOnTime.ExSigned = Extended.ExSign.Signed;
            this.txtS_VacOnTime.ExType = Extended.ExTxtType.Double;
            resources.ApplyResources(this.txtS_VacOnTime, "txtS_VacOnTime");
            this.txtS_VacOnTime.Name = "txtS_VacOnTime";
            this.txtS_VacOnTime.Tag = "0";
            // 
            // vwEqu_06Prb
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlPr_InrPort);
            this.Controls.Add(this.pnlPr_RPort);
            this.Controls.Add(this.pnlPr_LPort);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_06Prb";
            this.pnlPr_InrPort.ResumeLayout(false);
            this.pnlPr_RPort.ResumeLayout(false);
            this.pnlPr_RPort.PerformLayout();
            this.pnlPr_LPort.ResumeLayout(false);
            this.pnlPr_LPort.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPr_InrPort;
        private System.Windows.Forms.Label lb_PortInr;
        private System.Windows.Forms.ComboBox cmb_InPort;
        private System.Windows.Forms.ComboBox cmb_InBaud;
        private System.Windows.Forms.Label lb_BaudInr;
        private System.Windows.Forms.RichTextBox txtPr_InrLog;
        private System.Windows.Forms.Panel pnlPr_RPort;
        private System.Windows.Forms.Label lb_PortR;
        private System.Windows.Forms.ComboBox cmb_RPort;
        private System.Windows.Forms.ComboBox cmb_RBaud;
        private System.Windows.Forms.Label lb_BaudR;
        private System.Windows.Forms.RichTextBox txtPr_RLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_InOpen;
        private System.Windows.Forms.Button btn_InClose;
        private System.Windows.Forms.Button btn_ROpen;
        private System.Windows.Forms.Button btn_RClose;
        private System.Windows.Forms.Button btn_InRead;
        private System.Windows.Forms.Button btn_RRead;
        private System.Windows.Forms.Button btn_RRepeat;
        private System.Windows.Forms.Button btn_RAuto;
        private System.Windows.Forms.CheckBox ckbPr_InDn;
        private System.Windows.Forms.CheckBox ckb_RDn;
        private System.Windows.Forms.Label lbl_RAmp;
        private System.Windows.Forms.CheckBox ckb_RAir;
        private System.Windows.Forms.CheckBox ckb_InAir;
        private System.Windows.Forms.Button btn_RCheck;
        private System.Windows.Forms.Label label6;
        private Extended.ExTextBox txtS_PrbTestCountR;
        private Extended.ExTextBox txtS_UpErrRetryTmoutR;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private Extended.ExTextBox txtS_VacOffDelayR;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private Extended.ExTextBox txtS_AfterOffDelayR;
        private Extended.ExTextBox txtS_VacOnTimeR;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtU_RCalCheckCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_BaudL;
        private System.Windows.Forms.RichTextBox txtPr_LLog;
        private System.Windows.Forms.ComboBox cmb_LBaud;
        private System.Windows.Forms.Label lb_PortL;
        private System.Windows.Forms.ComboBox cmb_LPort;
        private System.Windows.Forms.Label label366;
        private System.Windows.Forms.Button btn_LClose;
        private System.Windows.Forms.Button btn_LOpen;
        private System.Windows.Forms.Button btn_LRead;
        private System.Windows.Forms.Button btn_LAuto;
        private System.Windows.Forms.Button btn_LRepeat;
        private System.Windows.Forms.Label lbl_LAmp;
        private System.Windows.Forms.CheckBox ckb_LDn;
        private System.Windows.Forms.CheckBox ckb_LAir;
        private System.Windows.Forms.Button btn_LCheck;
        private System.Windows.Forms.Button btn_LPrbTestStop;
        private System.Windows.Forms.Label lbl_VacOnTimeTitle;
        private System.Windows.Forms.Label txt_VacOnTimeUnit;
        private System.Windows.Forms.Label lbl_AfterOffDelayTitle;
        private System.Windows.Forms.Label txt_AfterOffDelayUnit;
        private Extended.ExTextBox txtS_VacOnTime;
        private Extended.ExTextBox txtS_AfterOffDelay;
        private System.Windows.Forms.Label lbl_VacOffDelayTitle;
        private System.Windows.Forms.Label txt_VacOffDelayUnit;
        private Extended.ExTextBox txtS_VacOffDelay;
        private System.Windows.Forms.Label lbl_UpErrRetryTmout;
        private System.Windows.Forms.Label lbl_UpErrRetryTmoutUnit;
        private Extended.ExTextBox txtS_UpErrRetryTmout;
        private Extended.ExTextBox txtS_PrbTestCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtU_LCalCheckCount;
        private System.Windows.Forms.Panel pnlPr_LPort;
        private System.Windows.Forms.TextBox txt_LY;
        private System.Windows.Forms.Label lb_LY;
        private System.Windows.Forms.TextBox txt_LX;
        private System.Windows.Forms.Label lb_LX;
        private System.Windows.Forms.TextBox txt_RY;
        private System.Windows.Forms.Label lb_RY;
        private System.Windows.Forms.TextBox txt_RX;
        private System.Windows.Forms.Label lb_RX;
        private System.Windows.Forms.TextBox R_Z_Pos;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox L_Z_Pos;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btn_LPrbT1;
        private System.Windows.Forms.Label lbl_Z;
        private System.Windows.Forms.Label lbl_Y;
        private System.Windows.Forms.Label lbl_X;
    }
}
