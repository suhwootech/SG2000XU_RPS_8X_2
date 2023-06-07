namespace SG2000X
{
    partial class frmLot
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.btn_Can = new System.Windows.Forms.Button();
            this.txt_Dev = new System.Windows.Forms.TextBox();
            this.btn_Open = new System.Windows.Forms.Button();
            this.lbl_Dev = new System.Windows.Forms.Label();
            this.lbl_Cnt = new System.Windows.Forms.Label();
            this.cmb_Cnt = new System.Windows.Forms.ComboBox();
            this.lbl_Lot = new System.Windows.Forms.Label();
            this.txt_Lot = new System.Windows.Forms.TextBox();
            this.lbl_LeftToolId = new System.Windows.Forms.Label();
            this.txt_LToolId = new System.Windows.Forms.TextBox();
            this.cmb_GrdLay = new System.Windows.Forms.ComboBox();
            this.lbl_GrdLay = new System.Windows.Forms.Label();
            this.timer_DfServer = new System.Windows.Forms.Timer(this.components);
            this.lbl_StripCount = new System.Windows.Forms.Label();
            this.txt_StripCount = new System.Windows.Forms.TextBox();
            this.lbl_RightToolId = new System.Windows.Forms.Label();
            this.txt_RToolId = new System.Windows.Forms.TextBox();
            this.lbl_LeftMT = new System.Windows.Forms.Label();
            this.lbl_RightMT = new System.Windows.Forms.Label();
            this.txt_LMTId = new System.Windows.Forms.TextBox();
            this.txt_RMTId = new System.Windows.Forms.TextBox();
            this.lbl_LToolId = new System.Windows.Forms.Label();
            this.lbl_RToolId = new System.Windows.Forms.Label();
            this.lbl_LDrsId = new System.Windows.Forms.Label();
            this.lbl_RDrsId = new System.Windows.Forms.Label();
            this.ckb_SetUpStripUse = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            this.lbl_Title.BackColor = System.Drawing.Color.White;
            this.lbl_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.Black;
            this.lbl_Title.Location = new System.Drawing.Point(0, 10);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Title.Size = new System.Drawing.Size(490, 30);
            this.lbl_Title.TabIndex = 4;
            this.lbl_Title.Text = "LOT Open";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_Can
            // 
            this.btn_Can.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Can.FlatAppearance.BorderSize = 0;
            this.btn_Can.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Can.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Can.ForeColor = System.Drawing.Color.White;
            this.btn_Can.Location = new System.Drawing.Point(390, 327);
            this.btn_Can.Name = "btn_Can";
            this.btn_Can.Size = new System.Drawing.Size(100, 50);
            this.btn_Can.TabIndex = 34;
            this.btn_Can.Tag = "";
            this.btn_Can.Text = "CANCEL";
            this.btn_Can.UseVisualStyleBackColor = false;
            this.btn_Can.Click += new System.EventHandler(this.btn_Can_Click);
            // 
            // txt_Dev
            // 
            this.txt_Dev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Dev.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txt_Dev.Location = new System.Drawing.Point(110, 50);
            this.txt_Dev.Name = "txt_Dev";
            this.txt_Dev.ReadOnly = true;
            this.txt_Dev.Size = new System.Drawing.Size(380, 23);
            this.txt_Dev.TabIndex = 36;
            this.txt_Dev.Tag = "0";
            // 
            // btn_Open
            // 
            this.btn_Open.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Open.FlatAppearance.BorderSize = 0;
            this.btn_Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Open.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Open.ForeColor = System.Drawing.Color.White;
            this.btn_Open.Location = new System.Drawing.Point(280, 327);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(100, 50);
            this.btn_Open.TabIndex = 37;
            this.btn_Open.Tag = "";
            this.btn_Open.Text = "OPEN";
            this.btn_Open.UseVisualStyleBackColor = false;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // lbl_Dev
            // 
            this.lbl_Dev.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Dev.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_Dev.ForeColor = System.Drawing.Color.Black;
            this.lbl_Dev.Location = new System.Drawing.Point(10, 50);
            this.lbl_Dev.Name = "lbl_Dev";
            this.lbl_Dev.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Dev.Size = new System.Drawing.Size(100, 25);
            this.lbl_Dev.TabIndex = 38;
            this.lbl_Dev.Text = "Device";
            this.lbl_Dev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_Cnt
            // 
            this.lbl_Cnt.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Cnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_Cnt.ForeColor = System.Drawing.Color.Black;
            this.lbl_Cnt.Location = new System.Drawing.Point(10, 85);
            this.lbl_Cnt.Name = "lbl_Cnt";
            this.lbl_Cnt.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Cnt.Size = new System.Drawing.Size(100, 25);
            this.lbl_Cnt.TabIndex = 39;
            this.lbl_Cnt.Text = "MGZ Count";
            this.lbl_Cnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmb_Cnt
            // 
            this.cmb_Cnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Cnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cmb_Cnt.FormattingEnabled = true;
            this.cmb_Cnt.Location = new System.Drawing.Point(110, 84);
            this.cmb_Cnt.Name = "cmb_Cnt";
            this.cmb_Cnt.Size = new System.Drawing.Size(110, 24);
            this.cmb_Cnt.TabIndex = 40;
            // 
            // lbl_Lot
            // 
            this.lbl_Lot.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Lot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_Lot.ForeColor = System.Drawing.Color.Black;
            this.lbl_Lot.Location = new System.Drawing.Point(10, 159);
            this.lbl_Lot.Name = "lbl_Lot";
            this.lbl_Lot.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Lot.Size = new System.Drawing.Size(100, 25);
            this.lbl_Lot.TabIndex = 42;
            this.lbl_Lot.Text = "LOT Name";
            this.lbl_Lot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_Lot
            // 
            this.txt_Lot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Lot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txt_Lot.Location = new System.Drawing.Point(110, 159);
            this.txt_Lot.Name = "txt_Lot";
            this.txt_Lot.Size = new System.Drawing.Size(380, 23);
            this.txt_Lot.TabIndex = 41;
            this.txt_Lot.Tag = "0";
            // 
            // lbl_LeftToolId
            // 
            this.lbl_LeftToolId.BackColor = System.Drawing.Color.Transparent;
            this.lbl_LeftToolId.ForeColor = System.Drawing.Color.Black;
            this.lbl_LeftToolId.Location = new System.Drawing.Point(10, 198);
            this.lbl_LeftToolId.Name = "lbl_LeftToolId";
            this.lbl_LeftToolId.Size = new System.Drawing.Size(100, 23);
            this.lbl_LeftToolId.TabIndex = 43;
            this.lbl_LeftToolId.Visible = false;
            // 
            // txt_LToolId
            // 
            this.txt_LToolId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_LToolId.Location = new System.Drawing.Point(179, 198);
            this.txt_LToolId.Name = "txt_LToolId";
            this.txt_LToolId.Size = new System.Drawing.Size(309, 21);
            this.txt_LToolId.TabIndex = 44;
            this.txt_LToolId.Tag = "0";
            this.txt_LToolId.Visible = false;
            // 
            // cmb_GrdLay
            // 
            this.cmb_GrdLay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_GrdLay.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cmb_GrdLay.FormattingEnabled = true;
            this.cmb_GrdLay.Items.AddRange(new object[] {
            "GL1",
            "GL2",
            "GL3"});
            this.cmb_GrdLay.Location = new System.Drawing.Point(380, 84);
            this.cmb_GrdLay.Name = "cmb_GrdLay";
            this.cmb_GrdLay.Size = new System.Drawing.Size(110, 24);
            this.cmb_GrdLay.TabIndex = 46;
            // 
            // lbl_GrdLay
            // 
            this.lbl_GrdLay.BackColor = System.Drawing.Color.Transparent;
            this.lbl_GrdLay.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_GrdLay.ForeColor = System.Drawing.Color.Black;
            this.lbl_GrdLay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_GrdLay.Location = new System.Drawing.Point(260, 85);
            this.lbl_GrdLay.Name = "lbl_GrdLay";
            this.lbl_GrdLay.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_GrdLay.Size = new System.Drawing.Size(120, 25);
            this.lbl_GrdLay.TabIndex = 45;
            this.lbl_GrdLay.Text = "GRD Layer";
            this.lbl_GrdLay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer_DfServer
            // 
            this.timer_DfServer.Interval = 500;
            this.timer_DfServer.Tick += new System.EventHandler(this.timer_DfServer_Tick);
            // 
            // lbl_StripCount
            // 
            this.lbl_StripCount.BackColor = System.Drawing.Color.Transparent;
            this.lbl_StripCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_StripCount.ForeColor = System.Drawing.Color.Black;
            this.lbl_StripCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_StripCount.Location = new System.Drawing.Point(10, 121);
            this.lbl_StripCount.Name = "lbl_StripCount";
            this.lbl_StripCount.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_StripCount.Size = new System.Drawing.Size(100, 25);
            this.lbl_StripCount.TabIndex = 50;
            this.lbl_StripCount.Text = "Strip Count";
            this.lbl_StripCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_StripCount.Visible = false;
            // 
            // txt_StripCount
            // 
            this.txt_StripCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_StripCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txt_StripCount.Location = new System.Drawing.Point(110, 121);
            this.txt_StripCount.Name = "txt_StripCount";
            this.txt_StripCount.Size = new System.Drawing.Size(170, 23);
            this.txt_StripCount.TabIndex = 49;
            this.txt_StripCount.Tag = "0";
            this.txt_StripCount.Text = "1";
            this.txt_StripCount.Visible = false;
            // 
            // lbl_RightToolId
            // 
            this.lbl_RightToolId.BackColor = System.Drawing.Color.Transparent;
            this.lbl_RightToolId.ForeColor = System.Drawing.Color.Black;
            this.lbl_RightToolId.Location = new System.Drawing.Point(12, 234);
            this.lbl_RightToolId.Name = "lbl_RightToolId";
            this.lbl_RightToolId.Size = new System.Drawing.Size(100, 23);
            this.lbl_RightToolId.TabIndex = 51;
            this.lbl_RightToolId.Visible = false;
            // 
            // txt_RToolId
            // 
            this.txt_RToolId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_RToolId.Location = new System.Drawing.Point(179, 231);
            this.txt_RToolId.Name = "txt_RToolId";
            this.txt_RToolId.Size = new System.Drawing.Size(309, 21);
            this.txt_RToolId.TabIndex = 52;
            this.txt_RToolId.Tag = "0";
            this.txt_RToolId.Visible = false;
            // 
            // lbl_LeftMT
            // 
            this.lbl_LeftMT.BackColor = System.Drawing.Color.Transparent;
            this.lbl_LeftMT.ForeColor = System.Drawing.Color.Black;
            this.lbl_LeftMT.Location = new System.Drawing.Point(12, 268);
            this.lbl_LeftMT.Name = "lbl_LeftMT";
            this.lbl_LeftMT.Size = new System.Drawing.Size(100, 23);
            this.lbl_LeftMT.TabIndex = 53;
            this.lbl_LeftMT.Visible = false;
            // 
            // lbl_RightMT
            // 
            this.lbl_RightMT.BackColor = System.Drawing.Color.Transparent;
            this.lbl_RightMT.ForeColor = System.Drawing.Color.Black;
            this.lbl_RightMT.Location = new System.Drawing.Point(12, 297);
            this.lbl_RightMT.Name = "lbl_RightMT";
            this.lbl_RightMT.Size = new System.Drawing.Size(100, 23);
            this.lbl_RightMT.TabIndex = 54;
            this.lbl_RightMT.Visible = false;
            // 
            // txt_LMTId
            // 
            this.txt_LMTId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_LMTId.Location = new System.Drawing.Point(179, 264);
            this.txt_LMTId.Name = "txt_LMTId";
            this.txt_LMTId.Size = new System.Drawing.Size(309, 21);
            this.txt_LMTId.TabIndex = 55;
            this.txt_LMTId.Tag = "0";
            this.txt_LMTId.Visible = false;
            // 
            // txt_RMTId
            // 
            this.txt_RMTId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_RMTId.Location = new System.Drawing.Point(179, 297);
            this.txt_RMTId.Name = "txt_RMTId";
            this.txt_RMTId.Size = new System.Drawing.Size(309, 21);
            this.txt_RMTId.TabIndex = 56;
            this.txt_RMTId.Tag = "0";
            this.txt_RMTId.Visible = false;
            // 
            // lbl_LToolId
            // 
            this.lbl_LToolId.BackColor = System.Drawing.Color.Transparent;
            this.lbl_LToolId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_LToolId.ForeColor = System.Drawing.Color.Black;
            this.lbl_LToolId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_LToolId.Location = new System.Drawing.Point(12, 198);
            this.lbl_LToolId.Name = "lbl_LToolId";
            this.lbl_LToolId.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_LToolId.Size = new System.Drawing.Size(143, 25);
            this.lbl_LToolId.TabIndex = 57;
            this.lbl_LToolId.Text = "Left Tool ID";
            this.lbl_LToolId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_LToolId.Visible = false;
            // 
            // lbl_RToolId
            // 
            this.lbl_RToolId.BackColor = System.Drawing.Color.Transparent;
            this.lbl_RToolId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_RToolId.ForeColor = System.Drawing.Color.Black;
            this.lbl_RToolId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_RToolId.Location = new System.Drawing.Point(12, 231);
            this.lbl_RToolId.Name = "lbl_RToolId";
            this.lbl_RToolId.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_RToolId.Size = new System.Drawing.Size(143, 25);
            this.lbl_RToolId.TabIndex = 58;
            this.lbl_RToolId.Text = "Right Tool ID";
            this.lbl_RToolId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_RToolId.Visible = false;
            // 
            // lbl_LDrsId
            // 
            this.lbl_LDrsId.BackColor = System.Drawing.Color.Transparent;
            this.lbl_LDrsId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_LDrsId.ForeColor = System.Drawing.Color.Black;
            this.lbl_LDrsId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_LDrsId.Location = new System.Drawing.Point(12, 264);
            this.lbl_LDrsId.Name = "lbl_LDrsId";
            this.lbl_LDrsId.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_LDrsId.Size = new System.Drawing.Size(143, 25);
            this.lbl_LDrsId.TabIndex = 59;
            this.lbl_LDrsId.Text = "Left Material ID";
            this.lbl_LDrsId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_LDrsId.Visible = false;
            // 
            // lbl_RDrsId
            // 
            this.lbl_RDrsId.BackColor = System.Drawing.Color.Transparent;
            this.lbl_RDrsId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_RDrsId.ForeColor = System.Drawing.Color.Black;
            this.lbl_RDrsId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_RDrsId.Location = new System.Drawing.Point(12, 297);
            this.lbl_RDrsId.Name = "lbl_RDrsId";
            this.lbl_RDrsId.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_RDrsId.Size = new System.Drawing.Size(143, 25);
            this.lbl_RDrsId.TabIndex = 60;
            this.lbl_RDrsId.Text = "Right Material ID";
            this.lbl_RDrsId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbl_RDrsId.Visible = false;
            // 
            // ckb_SetUpStripUse
            // 
            this.ckb_SetUpStripUse.BackColor = System.Drawing.Color.Transparent;
            this.ckb_SetUpStripUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckb_SetUpStripUse.ForeColor = System.Drawing.Color.Black;
            this.ckb_SetUpStripUse.Location = new System.Drawing.Point(294, 121);
            this.ckb_SetUpStripUse.Name = "ckb_SetUpStripUse";
            this.ckb_SetUpStripUse.Size = new System.Drawing.Size(194, 25);
            this.ckb_SetUpStripUse.TabIndex = 61;
            this.ckb_SetUpStripUse.Text = "Set Up Strip Use";
            this.ckb_SetUpStripUse.UseVisualStyleBackColor = false;
            // 
            // frmLot
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Orange;
            this.ClientSize = new System.Drawing.Size(500, 395);
            this.Controls.Add(this.ckb_SetUpStripUse);
            this.Controls.Add(this.lbl_RDrsId);
            this.Controls.Add(this.lbl_LDrsId);
            this.Controls.Add(this.lbl_RToolId);
            this.Controls.Add(this.lbl_LToolId);
            this.Controls.Add(this.txt_RMTId);
            this.Controls.Add(this.txt_LMTId);
            this.Controls.Add(this.lbl_RightMT);
            this.Controls.Add(this.lbl_LeftMT);
            this.Controls.Add(this.txt_RToolId);
            this.Controls.Add(this.lbl_RightToolId);
            this.Controls.Add(this.lbl_StripCount);
            this.Controls.Add(this.txt_StripCount);
            this.Controls.Add(this.cmb_GrdLay);
            this.Controls.Add(this.lbl_GrdLay);
            this.Controls.Add(this.txt_LToolId);
            this.Controls.Add(this.lbl_LeftToolId);
            this.Controls.Add(this.lbl_Lot);
            this.Controls.Add(this.txt_Lot);
            this.Controls.Add(this.cmb_Cnt);
            this.Controls.Add(this.lbl_Cnt);
            this.Controls.Add(this.lbl_Dev);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.btn_Can);
            this.Controls.Add(this.txt_Dev);
            this.Controls.Add(this.lbl_Title);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmPwd";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Button btn_Can;
        private System.Windows.Forms.TextBox txt_Dev;
        private System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.Label lbl_Dev;
        private System.Windows.Forms.Label lbl_Cnt;
        private System.Windows.Forms.ComboBox cmb_Cnt;
        private System.Windows.Forms.Label lbl_Lot;
        private System.Windows.Forms.TextBox txt_Lot;
        private System.Windows.Forms.Label lbl_LeftToolId;
        private System.Windows.Forms.TextBox txt_LToolId;
        private System.Windows.Forms.ComboBox cmb_GrdLay;
        private System.Windows.Forms.Label lbl_GrdLay;
        private System.Windows.Forms.Timer timer_DfServer;
        private System.Windows.Forms.Label lbl_StripCount;
        private System.Windows.Forms.TextBox txt_StripCount;
        private System.Windows.Forms.Label lbl_RightToolId;
        private System.Windows.Forms.TextBox txt_RToolId;
        private System.Windows.Forms.Label lbl_LeftMT;
        private System.Windows.Forms.Label lbl_RightMT;
        private System.Windows.Forms.TextBox txt_LMTId;
        private System.Windows.Forms.TextBox txt_RMTId;
        private System.Windows.Forms.Label lbl_LToolId;
        private System.Windows.Forms.Label lbl_RToolId;
        private System.Windows.Forms.Label lbl_LDrsId;
        private System.Windows.Forms.Label lbl_RDrsId;
        private System.Windows.Forms.CheckBox ckb_SetUpStripUse;
    }
}