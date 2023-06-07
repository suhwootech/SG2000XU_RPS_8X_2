namespace SG2000X
{
    partial class vwMain_Err
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_RaderErrMsg = new System.Windows.Forms.Label();
            this.pnl_CloseWithPws = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_RaderClearPws = new System.Windows.Forms.TextBox();
            this.btn_Close_Pws = new System.Windows.Forms.Button();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.lbl_No = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Action = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_ImgTitle = new System.Windows.Forms.Label();
            this.pbx_Img = new System.Windows.Forms.PictureBox();
            this.btn_Close = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnl_CloseWithPws.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbx_Img)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lbl_RaderErrMsg);
            this.panel1.Controls.Add(this.pnl_CloseWithPws);
            this.panel1.Controls.Add(this.lbl_Name);
            this.panel1.Controls.Add(this.lbl_No);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txt_Action);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lbl_ImgTitle);
            this.panel1.Controls.Add(this.pbx_Img);
            this.panel1.Controls.Add(this.btn_Close);
            this.panel1.Location = new System.Drawing.Point(0, 160);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1280, 700);
            this.panel1.TabIndex = 3;
            // 
            // lbl_RaderErrMsg
            // 
            this.lbl_RaderErrMsg.BackColor = System.Drawing.Color.LightGray;
            this.lbl_RaderErrMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_RaderErrMsg.Location = new System.Drawing.Point(645, 565);
            this.lbl_RaderErrMsg.Name = "lbl_RaderErrMsg";
            this.lbl_RaderErrMsg.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_RaderErrMsg.Size = new System.Drawing.Size(585, 25);
            this.lbl_RaderErrMsg.TabIndex = 275;
            this.lbl_RaderErrMsg.Text = "-";
            this.lbl_RaderErrMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnl_CloseWithPws
            // 
            this.pnl_CloseWithPws.Controls.Add(this.label3);
            this.pnl_CloseWithPws.Controls.Add(this.txt_RaderClearPws);
            this.pnl_CloseWithPws.Controls.Add(this.btn_Close_Pws);
            this.pnl_CloseWithPws.Location = new System.Drawing.Point(645, 656);
            this.pnl_CloseWithPws.Name = "pnl_CloseWithPws";
            this.pnl_CloseWithPws.Size = new System.Drawing.Size(585, 55);
            this.pnl_CloseWithPws.TabIndex = 274;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(240, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 26);
            this.label3.TabIndex = 270;
            this.label3.Text = "Passward";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_RaderClearPws
            // 
            this.txt_RaderClearPws.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txt_RaderClearPws.Location = new System.Drawing.Point(325, 15);
            this.txt_RaderClearPws.Name = "txt_RaderClearPws";
            this.txt_RaderClearPws.PasswordChar = '*';
            this.txt_RaderClearPws.Size = new System.Drawing.Size(240, 26);
            this.txt_RaderClearPws.TabIndex = 2;
            // 
            // btn_Close_Pws
            // 
            this.btn_Close_Pws.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Close_Pws.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Close_Pws.Font = new System.Drawing.Font("Arial Black", 12F);
            this.btn_Close_Pws.ForeColor = System.Drawing.Color.White;
            this.btn_Close_Pws.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Close_Pws.Location = new System.Drawing.Point(0, 0);
            this.btn_Close_Pws.Name = "btn_Close_Pws";
            this.btn_Close_Pws.Size = new System.Drawing.Size(240, 55);
            this.btn_Close_Pws.TabIndex = 1;
            this.btn_Close_Pws.Text = "Clear && CLOSE";
            this.btn_Close_Pws.UseVisualStyleBackColor = false;
            this.btn_Close_Pws.Click += new System.EventHandler(this.btn_Close_Pws_Click);
            // 
            // lbl_Name
            // 
            this.lbl_Name.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Name.Location = new System.Drawing.Point(755, 80);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Name.Size = new System.Drawing.Size(475, 25);
            this.lbl_Name.TabIndex = 270;
            this.lbl_Name.Text = "-";
            this.lbl_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_No
            // 
            this.lbl_No.BackColor = System.Drawing.Color.LightGray;
            this.lbl_No.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_No.Location = new System.Drawing.Point(645, 80);
            this.lbl_No.Name = "lbl_No";
            this.lbl_No.Size = new System.Drawing.Size(105, 25);
            this.lbl_No.TabIndex = 269;
            this.lbl_No.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(645, 120);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(585, 25);
            this.label2.TabIndex = 268;
            this.label2.Text = "Error Action";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_Action
            // 
            this.txt_Action.BackColor = System.Drawing.Color.LightGray;
            this.txt_Action.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Action.Location = new System.Drawing.Point(645, 150);
            this.txt_Action.Multiline = true;
            this.txt_Action.Name = "txt_Action";
            this.txt_Action.ReadOnly = true;
            this.txt_Action.Size = new System.Drawing.Size(585, 410);
            this.txt_Action.TabIndex = 263;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(645, 50);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(585, 25);
            this.label1.TabIndex = 29;
            this.label1.Text = "Error Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_ImgTitle
            // 
            this.lbl_ImgTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.lbl_ImgTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_ImgTitle.ForeColor = System.Drawing.Color.White;
            this.lbl_ImgTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_ImgTitle.Location = new System.Drawing.Point(50, 50);
            this.lbl_ImgTitle.Name = "lbl_ImgTitle";
            this.lbl_ImgTitle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_ImgTitle.Size = new System.Drawing.Size(570, 25);
            this.lbl_ImgTitle.TabIndex = 28;
            this.lbl_ImgTitle.Text = "Error Image";
            this.lbl_ImgTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbx_Img
            // 
            this.pbx_Img.BackColor = System.Drawing.Color.Transparent;
            this.pbx_Img.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbx_Img.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbx_Img.Location = new System.Drawing.Point(50, 80);
            this.pbx_Img.Name = "pbx_Img";
            this.pbx_Img.Size = new System.Drawing.Size(570, 570);
            this.pbx_Img.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbx_Img.TabIndex = 27;
            this.pbx_Img.TabStop = false;
            // 
            // btn_Close
            // 
            this.btn_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Close.FlatAppearance.BorderSize = 0;
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Close.Font = new System.Drawing.Font("Arial Black", 12F);
            this.btn_Close.ForeColor = System.Drawing.Color.White;
            this.btn_Close.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Close.Location = new System.Drawing.Point(645, 595);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(585, 55);
            this.btn_Close.TabIndex = 26;
            this.btn_Close.Tag = "";
            this.btn_Close.Text = "CLOSE";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // vwMain_Err
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "vwMain_Err";
            this.Size = new System.Drawing.Size(1280, 1020);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnl_CloseWithPws.ResumeLayout(false);
            this.pnl_CloseWithPws.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbx_Img)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Action;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_ImgTitle;
        private System.Windows.Forms.PictureBox pbx_Img;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Label lbl_No;
        private System.Windows.Forms.Panel pnl_CloseWithPws;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_RaderClearPws;
        private System.Windows.Forms.Button btn_Close_Pws;
        private System.Windows.Forms.Label lbl_RaderErrMsg;
    }
}
