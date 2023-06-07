namespace SG2000X
{
    partial class vwEqu_15IV2
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
            this.lbx_OfpLog = new System.Windows.Forms.ListBox();
            this.lbx_OnpLog = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_OfpIP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_OfpPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pnl_IV2ONP = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_OnpIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_OnpPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnl_IV2Manual = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_Send = new System.Windows.Forms.TextBox();
            this.btn_Text = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdb_OnpOfp = new System.Windows.Forms.RadioButton();
            this.rdb_Ofp = new System.Windows.Forms.RadioButton();
            this.rdb_Onp = new System.Windows.Forms.RadioButton();
            this.label62 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.pnl_IV2ONP.SuspendLayout();
            this.pnl_IV2Manual.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbx_OfpLog
            // 
            this.lbx_OfpLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F);
            this.lbx_OfpLog.FormattingEnabled = true;
            this.lbx_OfpLog.HorizontalScrollbar = true;
            this.lbx_OfpLog.Location = new System.Drawing.Point(318, 520);
            this.lbx_OfpLog.Name = "lbx_OfpLog";
            this.lbx_OfpLog.Size = new System.Drawing.Size(700, 342);
            this.lbx_OfpLog.TabIndex = 36;
            // 
            // lbx_OnpLog
            // 
            this.lbx_OnpLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbx_OnpLog.FormattingEnabled = true;
            this.lbx_OnpLog.HorizontalScrollbar = true;
            this.lbx_OnpLog.ItemHeight = 12;
            this.lbx_OnpLog.Location = new System.Drawing.Point(318, 172);
            this.lbx_OnpLog.Name = "lbx_OnpLog";
            this.lbx_OnpLog.Size = new System.Drawing.Size(700, 340);
            this.lbx_OnpLog.TabIndex = 35;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txt_OfpIP);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txt_OfpPort);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(11, 520);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 342);
            this.panel1.TabIndex = 34;
            this.panel1.Tag = "0";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label4.Location = new System.Drawing.Point(90, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "IP : ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_OfpIP
            // 
            this.txt_OfpIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txt_OfpIP.Location = new System.Drawing.Point(145, 65);
            this.txt_OfpIP.Name = "txt_OfpIP";
            this.txt_OfpIP.Size = new System.Drawing.Size(127, 20);
            this.txt_OfpIP.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label5.Location = new System.Drawing.Point(90, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Port : ";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_OfpPort
            // 
            this.txt_OfpPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txt_OfpPort.Location = new System.Drawing.Point(145, 40);
            this.txt_OfpPort.Name = "txt_OfpPort";
            this.txt_OfpPort.Size = new System.Drawing.Size(127, 20);
            this.txt_OfpPort.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(1, 2);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label6.Size = new System.Drawing.Size(270, 25);
            this.label6.TabIndex = 3;
            this.label6.Text = "OffLoader Picker IV2 Setting";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnl_IV2ONP
            // 
            this.pnl_IV2ONP.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_IV2ONP.Controls.Add(this.label3);
            this.pnl_IV2ONP.Controls.Add(this.txt_OnpIP);
            this.pnl_IV2ONP.Controls.Add(this.label2);
            this.pnl_IV2ONP.Controls.Add(this.txt_OnpPort);
            this.pnl_IV2ONP.Controls.Add(this.label1);
            this.pnl_IV2ONP.Location = new System.Drawing.Point(11, 172);
            this.pnl_IV2ONP.Name = "pnl_IV2ONP";
            this.pnl_IV2ONP.Size = new System.Drawing.Size(300, 340);
            this.pnl_IV2ONP.TabIndex = 33;
            this.pnl_IV2ONP.Tag = "0";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label3.Location = new System.Drawing.Point(90, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "IP : ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_OnpIP
            // 
            this.txt_OnpIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txt_OnpIP.Location = new System.Drawing.Point(145, 65);
            this.txt_OnpIP.Name = "txt_OnpIP";
            this.txt_OnpIP.Size = new System.Drawing.Size(127, 20);
            this.txt_OnpIP.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label2.Location = new System.Drawing.Point(90, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port : ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_OnpPort
            // 
            this.txt_OnpPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txt_OnpPort.Location = new System.Drawing.Point(145, 40);
            this.txt_OnpPort.Name = "txt_OnpPort";
            this.txt_OnpPort.Size = new System.Drawing.Size(127, 20);
            this.txt_OnpPort.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(1, 2);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(270, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "OnLoader Picker IV2 Setting";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnl_IV2Manual
            // 
            this.pnl_IV2Manual.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_IV2Manual.Controls.Add(this.groupBox2);
            this.pnl_IV2Manual.Controls.Add(this.button1);
            this.pnl_IV2Manual.Controls.Add(this.btn_Disconnect);
            this.pnl_IV2Manual.Controls.Add(this.btn_Save);
            this.pnl_IV2Manual.Controls.Add(this.groupBox1);
            this.pnl_IV2Manual.Controls.Add(this.label62);
            this.pnl_IV2Manual.Location = new System.Drawing.Point(12, 14);
            this.pnl_IV2Manual.Name = "pnl_IV2Manual";
            this.pnl_IV2Manual.Size = new System.Drawing.Size(1006, 150);
            this.pnl_IV2Manual.TabIndex = 32;
            this.pnl_IV2Manual.Tag = "0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_Send);
            this.groupBox2.Controls.Add(this.btn_Text);
            this.groupBox2.Location = new System.Drawing.Point(308, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 56);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            // 
            // txt_Send
            // 
            this.txt_Send.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txt_Send.Location = new System.Drawing.Point(100, 22);
            this.txt_Send.Name = "txt_Send";
            this.txt_Send.Size = new System.Drawing.Size(145, 20);
            this.txt_Send.TabIndex = 7;
            // 
            // btn_Text
            // 
            this.btn_Text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Text.FlatAppearance.BorderSize = 0;
            this.btn_Text.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Text.Font = new System.Drawing.Font("Arial Black", 9F);
            this.btn_Text.ForeColor = System.Drawing.Color.White;
            this.btn_Text.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Text.Location = new System.Drawing.Point(5, 16);
            this.btn_Text.Name = "btn_Text";
            this.btn_Text.Size = new System.Drawing.Size(90, 30);
            this.btn_Text.TabIndex = 316;
            this.btn_Text.Tag = "0";
            this.btn_Text.Text = "Send Text";
            this.btn_Text.UseVisualStyleBackColor = false;
            this.btn_Text.Click += new System.EventHandler(this.SendMsg_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Black", 9F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(313, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 30);
            this.button1.TabIndex = 315;
            this.button1.Tag = "0";
            this.button1.Text = "Trigger";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Tri_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Disconnect.FlatAppearance.BorderSize = 0;
            this.btn_Disconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Disconnect.Font = new System.Drawing.Font("Arial Black", 12F);
            this.btn_Disconnect.ForeColor = System.Drawing.Color.White;
            this.btn_Disconnect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Disconnect.Location = new System.Drawing.Point(850, 45);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(150, 30);
            this.btn_Disconnect.TabIndex = 314;
            this.btn_Disconnect.Tag = "0";
            this.btn_Disconnect.Text = "Disconnect";
            this.btn_Disconnect.UseVisualStyleBackColor = false;
            this.btn_Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.Font = new System.Drawing.Font("Arial Black", 12F);
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(850, 80);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(150, 65);
            this.btn_Save.TabIndex = 313;
            this.btn_Save.Tag = "0";
            this.btn_Save.Text = "SAVE";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdb_OnpOfp);
            this.groupBox1.Controls.Add(this.rdb_Ofp);
            this.groupBox1.Controls.Add(this.rdb_Onp);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.groupBox1.Location = new System.Drawing.Point(10, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 103);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            // 
            // rdb_OnpOfp
            // 
            this.rdb_OnpOfp.AutoSize = true;
            this.rdb_OnpOfp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rdb_OnpOfp.Location = new System.Drawing.Point(6, 71);
            this.rdb_OnpOfp.Name = "rdb_OnpOfp";
            this.rdb_OnpOfp.Size = new System.Drawing.Size(128, 17);
            this.rdb_OnpOfp.TabIndex = 31;
            this.rdb_OnpOfp.Tag = "OnpOfp";
            this.rdb_OnpOfp.Text = "On, Off Loader Picker";
            this.rdb_OnpOfp.UseVisualStyleBackColor = true;
            this.rdb_OnpOfp.CheckedChanged += new System.EventHandler(this.ManualCheckedChange);
            // 
            // rdb_Ofp
            // 
            this.rdb_Ofp.AutoSize = true;
            this.rdb_Ofp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rdb_Ofp.Location = new System.Drawing.Point(6, 48);
            this.rdb_Ofp.Name = "rdb_Ofp";
            this.rdb_Ofp.Size = new System.Drawing.Size(105, 17);
            this.rdb_Ofp.TabIndex = 30;
            this.rdb_Ofp.Tag = "Ofp";
            this.rdb_Ofp.Text = "OffLoader Picker";
            this.rdb_Ofp.UseVisualStyleBackColor = true;
            this.rdb_Ofp.CheckedChanged += new System.EventHandler(this.ManualCheckedChange);
            // 
            // rdb_Onp
            // 
            this.rdb_Onp.AutoSize = true;
            this.rdb_Onp.Checked = true;
            this.rdb_Onp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.rdb_Onp.Location = new System.Drawing.Point(6, 25);
            this.rdb_Onp.Name = "rdb_Onp";
            this.rdb_Onp.Size = new System.Drawing.Size(105, 17);
            this.rdb_Onp.TabIndex = 29;
            this.rdb_Onp.TabStop = true;
            this.rdb_Onp.Tag = "Onp";
            this.rdb_Onp.Text = "OnLoader Picker";
            this.rdb_Onp.UseVisualStyleBackColor = true;
            this.rdb_Onp.CheckedChanged += new System.EventHandler(this.ManualCheckedChange);
            // 
            // label62
            // 
            this.label62.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label62.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label62.ForeColor = System.Drawing.Color.White;
            this.label62.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label62.Location = new System.Drawing.Point(1, 2);
            this.label62.Name = "label62";
            this.label62.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label62.Size = new System.Drawing.Size(974, 25);
            this.label62.TabIndex = 3;
            this.label62.Text = "Manual";
            this.label62.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vwEqu_15IV2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lbx_OfpLog);
            this.Controls.Add(this.lbx_OnpLog);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnl_IV2ONP);
            this.Controls.Add(this.pnl_IV2Manual);
            this.Name = "vwEqu_15IV2";
            this.Size = new System.Drawing.Size(1030, 900);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnl_IV2ONP.ResumeLayout(false);
            this.pnl_IV2ONP.PerformLayout();
            this.pnl_IV2Manual.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbx_OfpLog;
        private System.Windows.Forms.ListBox lbx_OnpLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_OfpIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_OfpPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnl_IV2ONP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_OnpIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_OnpPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnl_IV2Manual;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txt_Send;
        private System.Windows.Forms.Button btn_Text;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdb_OnpOfp;
        private System.Windows.Forms.RadioButton rdb_Ofp;
        private System.Windows.Forms.RadioButton rdb_Onp;
        private System.Windows.Forms.Label label62;
    }
}
