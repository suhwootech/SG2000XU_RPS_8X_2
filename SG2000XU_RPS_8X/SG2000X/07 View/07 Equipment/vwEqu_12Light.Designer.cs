namespace SG2000X
{
    partial class vwEqu_12Light
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_12Light));
            this.label136 = new System.Windows.Forms.Label();
            this.btn_Open = new System.Windows.Forms.Button();
            this.txt_Log = new System.Windows.Forms.RichTextBox();
            this.pnl_Para = new System.Windows.Forms.Panel();
            this.lb_Port = new System.Windows.Forms.Label();
            this.cmb_Port = new System.Windows.Forms.ComboBox();
            this.ckb_Rts = new System.Windows.Forms.CheckBox();
            this.cmb_Baud = new System.Windows.Forms.ComboBox();
            this.lb_Baud = new System.Windows.Forms.Label();
            this.cmb_Parity = new System.Windows.Forms.ComboBox();
            this.lb_Stopbits = new System.Windows.Forms.Label();
            this.lb_Parity = new System.Windows.Forms.Label();
            this.cmb_SBits = new System.Windows.Forms.ComboBox();
            this.cmb_DBits = new System.Windows.Forms.ComboBox();
            this.lb_Databits = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.pnl_Port = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_Set2 = new System.Windows.Forms.Button();
            this.nud_Val2 = new System.Windows.Forms.NumericUpDown();
            this.btn_Off2 = new System.Windows.Forms.Button();
            this.btn_On2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_Set1 = new System.Windows.Forms.Button();
            this.nud_Val1 = new System.Windows.Forms.NumericUpDown();
            this.btn_Off1 = new System.Windows.Forms.Button();
            this.btn_On1 = new System.Windows.Forms.Button();
            this.pnl_Para.SuspendLayout();
            this.pnl_Port.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Val2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Val1)).BeginInit();
            this.SuspendLayout();
            // 
            // label136
            // 
            resources.ApplyResources(this.label136, "label136");
            this.label136.Name = "label136";
            // 
            // btn_Open
            // 
            resources.ApplyResources(this.btn_Open, "btn_Open");
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Tag = "2";
            this.btn_Open.UseVisualStyleBackColor = true;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // txt_Log
            // 
            this.txt_Log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_Log, "txt_Log");
            this.txt_Log.Name = "txt_Log";
            // 
            // pnl_Para
            // 
            this.pnl_Para.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_Para.Controls.Add(this.lb_Port);
            this.pnl_Para.Controls.Add(this.cmb_Port);
            this.pnl_Para.Controls.Add(this.ckb_Rts);
            this.pnl_Para.Controls.Add(this.cmb_Baud);
            this.pnl_Para.Controls.Add(this.lb_Baud);
            this.pnl_Para.Controls.Add(this.cmb_Parity);
            this.pnl_Para.Controls.Add(this.lb_Stopbits);
            this.pnl_Para.Controls.Add(this.lb_Parity);
            this.pnl_Para.Controls.Add(this.cmb_SBits);
            this.pnl_Para.Controls.Add(this.cmb_DBits);
            this.pnl_Para.Controls.Add(this.lb_Databits);
            resources.ApplyResources(this.pnl_Para, "pnl_Para");
            this.pnl_Para.Name = "pnl_Para";
            // 
            // lb_Port
            // 
            resources.ApplyResources(this.lb_Port, "lb_Port");
            this.lb_Port.Name = "lb_Port";
            // 
            // cmb_Port
            // 
            this.cmb_Port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Port.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_Port, "cmb_Port");
            this.cmb_Port.Name = "cmb_Port";
            // 
            // ckb_Rts
            // 
            resources.ApplyResources(this.ckb_Rts, "ckb_Rts");
            this.ckb_Rts.Name = "ckb_Rts";
            this.ckb_Rts.UseVisualStyleBackColor = true;
            // 
            // cmb_Baud
            // 
            this.cmb_Baud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Baud.FormattingEnabled = true;
            this.cmb_Baud.Items.AddRange(new object[] {
            resources.GetString("cmb_Baud.Items"),
            resources.GetString("cmb_Baud.Items1"),
            resources.GetString("cmb_Baud.Items2"),
            resources.GetString("cmb_Baud.Items3"),
            resources.GetString("cmb_Baud.Items4"),
            resources.GetString("cmb_Baud.Items5"),
            resources.GetString("cmb_Baud.Items6"),
            resources.GetString("cmb_Baud.Items7"),
            resources.GetString("cmb_Baud.Items8"),
            resources.GetString("cmb_Baud.Items9"),
            resources.GetString("cmb_Baud.Items10"),
            resources.GetString("cmb_Baud.Items11"),
            resources.GetString("cmb_Baud.Items12"),
            resources.GetString("cmb_Baud.Items13")});
            resources.ApplyResources(this.cmb_Baud, "cmb_Baud");
            this.cmb_Baud.Name = "cmb_Baud";
            // 
            // lb_Baud
            // 
            resources.ApplyResources(this.lb_Baud, "lb_Baud");
            this.lb_Baud.Name = "lb_Baud";
            // 
            // cmb_Parity
            // 
            this.cmb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Parity.FormattingEnabled = true;
            this.cmb_Parity.Items.AddRange(new object[] {
            resources.GetString("cmb_Parity.Items"),
            resources.GetString("cmb_Parity.Items1"),
            resources.GetString("cmb_Parity.Items2"),
            resources.GetString("cmb_Parity.Items3"),
            resources.GetString("cmb_Parity.Items4")});
            resources.ApplyResources(this.cmb_Parity, "cmb_Parity");
            this.cmb_Parity.Name = "cmb_Parity";
            // 
            // lb_Stopbits
            // 
            resources.ApplyResources(this.lb_Stopbits, "lb_Stopbits");
            this.lb_Stopbits.Name = "lb_Stopbits";
            // 
            // lb_Parity
            // 
            resources.ApplyResources(this.lb_Parity, "lb_Parity");
            this.lb_Parity.Name = "lb_Parity";
            // 
            // cmb_SBits
            // 
            this.cmb_SBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_SBits.FormattingEnabled = true;
            this.cmb_SBits.Items.AddRange(new object[] {
            resources.GetString("cmb_SBits.Items"),
            resources.GetString("cmb_SBits.Items1"),
            resources.GetString("cmb_SBits.Items2"),
            resources.GetString("cmb_SBits.Items3")});
            resources.ApplyResources(this.cmb_SBits, "cmb_SBits");
            this.cmb_SBits.Name = "cmb_SBits";
            // 
            // cmb_DBits
            // 
            this.cmb_DBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_DBits.FormattingEnabled = true;
            this.cmb_DBits.Items.AddRange(new object[] {
            resources.GetString("cmb_DBits.Items"),
            resources.GetString("cmb_DBits.Items1"),
            resources.GetString("cmb_DBits.Items2"),
            resources.GetString("cmb_DBits.Items3")});
            resources.ApplyResources(this.cmb_DBits, "cmb_DBits");
            this.cmb_DBits.Name = "cmb_DBits";
            // 
            // lb_Databits
            // 
            resources.ApplyResources(this.lb_Databits, "lb_Databits");
            this.lb_Databits.Name = "lb_Databits";
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Tag = "2";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // pnl_Port
            // 
            this.pnl_Port.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_Port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Port.Controls.Add(this.groupBox1);
            this.pnl_Port.Controls.Add(this.btn_Save);
            this.pnl_Port.Controls.Add(this.btn_Close);
            this.pnl_Port.Controls.Add(this.pnl_Para);
            this.pnl_Port.Controls.Add(this.txt_Log);
            this.pnl_Port.Controls.Add(this.btn_Open);
            this.pnl_Port.Controls.Add(this.label136);
            resources.ApplyResources(this.pnl_Port, "pnl_Port");
            this.pnl_Port.Name = "pnl_Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_Set2);
            this.groupBox3.Controls.Add(this.nud_Val2);
            this.groupBox3.Controls.Add(this.btn_Off2);
            this.groupBox3.Controls.Add(this.btn_On2);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // btn_Set2
            // 
            resources.ApplyResources(this.btn_Set2, "btn_Set2");
            this.btn_Set2.Name = "btn_Set2";
            this.btn_Set2.Tag = "2";
            this.btn_Set2.UseVisualStyleBackColor = true;
            this.btn_Set2.Click += new System.EventHandler(this.btn_Set2_Click);
            // 
            // nud_Val2
            // 
            resources.ApplyResources(this.nud_Val2, "nud_Val2");
            this.nud_Val2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_Val2.Name = "nud_Val2";
            this.nud_Val2.ValueChanged += new System.EventHandler(this.nud_Val2_ValueChanged);
            // 
            // btn_Off2
            // 
            resources.ApplyResources(this.btn_Off2, "btn_Off2");
            this.btn_Off2.Name = "btn_Off2";
            this.btn_Off2.Tag = "2";
            this.btn_Off2.UseVisualStyleBackColor = true;
            this.btn_Off2.Click += new System.EventHandler(this.btn_Off2_Click);
            // 
            // btn_On2
            // 
            resources.ApplyResources(this.btn_On2, "btn_On2");
            this.btn_On2.Name = "btn_On2";
            this.btn_On2.Tag = "2";
            this.btn_On2.UseVisualStyleBackColor = true;
            this.btn_On2.Click += new System.EventHandler(this.btn_On2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Set1);
            this.groupBox2.Controls.Add(this.nud_Val1);
            this.groupBox2.Controls.Add(this.btn_Off1);
            this.groupBox2.Controls.Add(this.btn_On1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btn_Set1
            // 
            resources.ApplyResources(this.btn_Set1, "btn_Set1");
            this.btn_Set1.Name = "btn_Set1";
            this.btn_Set1.Tag = "2";
            this.btn_Set1.UseVisualStyleBackColor = true;
            this.btn_Set1.Click += new System.EventHandler(this.btn_Set1_Click);
            // 
            // nud_Val1
            // 
            resources.ApplyResources(this.nud_Val1, "nud_Val1");
            this.nud_Val1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_Val1.Name = "nud_Val1";
            this.nud_Val1.ValueChanged += new System.EventHandler(this.nud_Val1_ValueChanged);
            // 
            // btn_Off1
            // 
            resources.ApplyResources(this.btn_Off1, "btn_Off1");
            this.btn_Off1.Name = "btn_Off1";
            this.btn_Off1.Tag = "2";
            this.btn_Off1.UseVisualStyleBackColor = true;
            this.btn_Off1.Click += new System.EventHandler(this.btn_Off1_Click);
            // 
            // btn_On1
            // 
            resources.ApplyResources(this.btn_On1, "btn_On1");
            this.btn_On1.Name = "btn_On1";
            this.btn_On1.Tag = "2";
            this.btn_On1.UseVisualStyleBackColor = true;
            this.btn_On1.Click += new System.EventHandler(this.btn_On1_Click);
            // 
            // vwEqu_12Light
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnl_Port);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_12Light";
            this.pnl_Para.ResumeLayout(false);
            this.pnl_Port.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nud_Val2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nud_Val1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label136;
        private System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.RichTextBox txt_Log;
        private System.Windows.Forms.Panel pnl_Para;
        private System.Windows.Forms.Label lb_Port;
        private System.Windows.Forms.ComboBox cmb_Port;
        private System.Windows.Forms.CheckBox ckb_Rts;
        private System.Windows.Forms.ComboBox cmb_Baud;
        private System.Windows.Forms.Label lb_Baud;
        private System.Windows.Forms.ComboBox cmb_Parity;
        private System.Windows.Forms.Label lb_Stopbits;
        private System.Windows.Forms.Label lb_Parity;
        private System.Windows.Forms.ComboBox cmb_SBits;
        private System.Windows.Forms.ComboBox cmb_DBits;
        private System.Windows.Forms.Label lb_Databits;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Panel pnl_Port;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_Set2;
        private System.Windows.Forms.NumericUpDown nud_Val2;
        private System.Windows.Forms.Button btn_Off2;
        private System.Windows.Forms.Button btn_On2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Set1;
        private System.Windows.Forms.NumericUpDown nud_Val1;
        private System.Windows.Forms.Button btn_Off1;
        private System.Windows.Forms.Button btn_On1;
    }
}
