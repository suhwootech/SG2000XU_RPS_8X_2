namespace SG2000X
{ 
    partial class vwMain_Key
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
            this.btn_Keyin = new System.Windows.Forms.Button();
            this.btn_Err = new System.Windows.Forms.Button();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Key = new System.Windows.Forms.TextBox();
            this.lbl_Help = new System.Windows.Forms.Label();
            this.btn_Buzz = new System.Windows.Forms.Button();
            this.picb_OCRImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picb_OCRImg)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Keyin
            // 
            this.btn_Keyin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Keyin.FlatAppearance.BorderSize = 0;
            this.btn_Keyin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Keyin.Font = new System.Drawing.Font("Arial Black", 13F, System.Drawing.FontStyle.Bold);
            this.btn_Keyin.ForeColor = System.Drawing.Color.White;
            this.btn_Keyin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Keyin.Location = new System.Drawing.Point(890, 500);
            this.btn_Keyin.Name = "btn_Keyin";
            this.btn_Keyin.Size = new System.Drawing.Size(150, 90);
            this.btn_Keyin.TabIndex = 45;
            this.btn_Keyin.Tag = "";
            this.btn_Keyin.Text = "KEY-IN";
            this.btn_Keyin.UseVisualStyleBackColor = false;
            this.btn_Keyin.Click += new System.EventHandler(this.btn_Keyin_Click);
            // 
            // btn_Err
            // 
            this.btn_Err.BackColor = System.Drawing.Color.Red;
            this.btn_Err.FlatAppearance.BorderSize = 0;
            this.btn_Err.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.btn_Err.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            this.btn_Err.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Err.Font = new System.Drawing.Font("Arial Black", 13F, System.Drawing.FontStyle.Bold);
            this.btn_Err.ForeColor = System.Drawing.Color.White;
            this.btn_Err.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Err.Location = new System.Drawing.Point(446, 500);
            this.btn_Err.Name = "btn_Err";
            this.btn_Err.Size = new System.Drawing.Size(150, 90);
            this.btn_Err.TabIndex = 44;
            this.btn_Err.Tag = "";
            this.btn_Err.Text = "ERROR";
            this.btn_Err.UseVisualStyleBackColor = false;
            this.btn_Err.Click += new System.EventHandler(this.btn_Err_Click);
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
            this.lbl_Title.Text = "Strip ID Manual Key-in";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(0, 390);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 38);
            this.label2.TabIndex = 221;
            this.label2.Text = "Strip ID";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_Key
            // 
            this.txt_Key.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Key.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_Key.Location = new System.Drawing.Point(446, 390);
            this.txt_Key.Name = "txt_Key";
            this.txt_Key.Size = new System.Drawing.Size(594, 38);
            this.txt_Key.TabIndex = 223;
            // 
            // lbl_Help
            // 
            this.lbl_Help.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Help.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_Help.ForeColor = System.Drawing.Color.Red;
            this.lbl_Help.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Help.Location = new System.Drawing.Point(450, 435);
            this.lbl_Help.Name = "lbl_Help";
            this.lbl_Help.Size = new System.Drawing.Size(590, 30);
            this.lbl_Help.TabIndex = 224;
            this.lbl_Help.Text = "Strip ID";
            this.lbl_Help.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_Buzz
            // 
            this.btn_Buzz.BackColor = System.Drawing.Color.Yellow;
            this.btn_Buzz.FlatAppearance.BorderSize = 0;
            this.btn_Buzz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Buzz.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Buzz.ForeColor = System.Drawing.Color.Black;
            this.btn_Buzz.Image = global::SG2000X.Properties.Resources.mute1;
            this.btn_Buzz.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btn_Buzz.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Buzz.Location = new System.Drawing.Point(1090, 30);
            this.btn_Buzz.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
            this.btn_Buzz.Name = "btn_Buzz";
            this.btn_Buzz.Padding = new System.Windows.Forms.Padding(0, 10, 0, 3);
            this.btn_Buzz.Size = new System.Drawing.Size(150, 90);
            this.btn_Buzz.TabIndex = 225;
            this.btn_Buzz.Text = "BUZZ OFF";
            this.btn_Buzz.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btn_Buzz.UseVisualStyleBackColor = false;
            this.btn_Buzz.Click += new System.EventHandler(this.btn_Buzz_Click);
            // 
            // picb_OCRImg
            // 
            this.picb_OCRImg.Location = new System.Drawing.Point(185, 593);
            this.picb_OCRImg.Name = "picb_OCRImg";
            this.picb_OCRImg.Size = new System.Drawing.Size(916, 424);
            this.picb_OCRImg.TabIndex = 227;
            this.picb_OCRImg.TabStop = false;
            // 
            // vwMain_Key
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Orange;
            this.Controls.Add(this.picb_OCRImg);
            this.Controls.Add(this.btn_Buzz);
            this.Controls.Add(this.lbl_Help);
            this.Controls.Add(this.txt_Key);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_Keyin);
            this.Controls.Add(this.btn_Err);
            this.Controls.Add(this.lbl_Title);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "vwMain_Key";
            this.Size = new System.Drawing.Size(1280, 1020);
            ((System.ComponentModel.ISupportInitialize)(this.picb_OCRImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Keyin;
        private System.Windows.Forms.Button btn_Err;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Key;
        private System.Windows.Forms.Label lbl_Help;
        private System.Windows.Forms.Button btn_Buzz;
        private System.Windows.Forms.PictureBox picb_OCRImg;
    }
}
