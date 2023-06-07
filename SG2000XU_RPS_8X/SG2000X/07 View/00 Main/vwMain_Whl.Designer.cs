namespace SG2000X
{ 
    partial class vwMain_Whl
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
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Can = new System.Windows.Forms.Button();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.cmb_WhlL = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_WhlR = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Ok
            // 
            this.btn_Ok.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Ok.FlatAppearance.BorderSize = 0;
            this.btn_Ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Ok.Font = new System.Drawing.Font("Arial Black", 13F, System.Drawing.FontStyle.Bold);
            this.btn_Ok.ForeColor = System.Drawing.Color.White;
            this.btn_Ok.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Ok.Location = new System.Drawing.Point(735, 489);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(135, 80);
            this.btn_Ok.TabIndex = 45;
            this.btn_Ok.Tag = "";
            this.btn_Ok.Text = "OK";
            this.btn_Ok.UseVisualStyleBackColor = false;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Can
            // 
            this.btn_Can.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Can.FlatAppearance.BorderSize = 0;
            this.btn_Can.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Can.Font = new System.Drawing.Font("Arial Black", 13F, System.Drawing.FontStyle.Bold);
            this.btn_Can.ForeColor = System.Drawing.Color.White;
            this.btn_Can.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Can.Location = new System.Drawing.Point(905, 489);
            this.btn_Can.Name = "btn_Can";
            this.btn_Can.Size = new System.Drawing.Size(135, 80);
            this.btn_Can.TabIndex = 44;
            this.btn_Can.Tag = "";
            this.btn_Can.Text = "CANCEL";
            this.btn_Can.UseVisualStyleBackColor = false;
            this.btn_Can.Click += new System.EventHandler(this.btn_Can_Click);
            // 
            // lbl_Title
            // 
            this.lbl_Title.BackColor = System.Drawing.Color.White;
            this.lbl_Title.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.Black;
            this.lbl_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Title.Location = new System.Drawing.Point(0, 300);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Padding = new System.Windows.Forms.Padding(300, 0, 0, 0);
            this.lbl_Title.Size = new System.Drawing.Size(1280, 50);
            this.lbl_Title.TabIndex = 43;
            this.lbl_Title.Text = "Wheel File";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmb_WhlL
            // 
            this.cmb_WhlL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_WhlL.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.cmb_WhlL.FormattingEnabled = true;
            this.cmb_WhlL.ItemHeight = 31;
            this.cmb_WhlL.Location = new System.Drawing.Point(445, 370);
            this.cmb_WhlL.Name = "cmb_WhlL";
            this.cmb_WhlL.Size = new System.Drawing.Size(595, 39);
            this.cmb_WhlL.TabIndex = 220;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(0, 370);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 39);
            this.label2.TabIndex = 219;
            this.label2.Text = "Left";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmb_WhlR
            // 
            this.cmb_WhlR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_WhlR.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold);
            this.cmb_WhlR.FormattingEnabled = true;
            this.cmb_WhlR.ItemHeight = 31;
            this.cmb_WhlR.Location = new System.Drawing.Point(445, 430);
            this.cmb_WhlR.Name = "cmb_WhlR";
            this.cmb_WhlR.Size = new System.Drawing.Size(595, 39);
            this.cmb_WhlR.TabIndex = 222;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(0, 430);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 39);
            this.label1.TabIndex = 221;
            this.label1.Text = "Right";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // vwMain_Whl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightSalmon;
            this.Controls.Add(this.cmb_WhlR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmb_WhlL);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.btn_Can);
            this.Controls.Add(this.lbl_Title);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "vwMain_Whl";
            this.Size = new System.Drawing.Size(1280, 1020);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Can;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.ComboBox cmb_WhlL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_WhlR;
        private System.Windows.Forms.Label label1;
    }
}
