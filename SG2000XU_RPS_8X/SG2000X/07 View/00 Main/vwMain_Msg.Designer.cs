namespace SG2000X
{ 
    partial class vwMain_Msg
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
            this.lbl_Note = new System.Windows.Forms.Label();
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
            this.btn_Ok.Location = new System.Drawing.Point(735, 700);
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
            this.btn_Can.Location = new System.Drawing.Point(905, 700);
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
            this.lbl_Title.Text = "Title";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Note
            // 
            this.lbl_Note.BackColor = System.Drawing.Color.White;
            this.lbl_Note.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_Note.ForeColor = System.Drawing.Color.Black;
            this.lbl_Note.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Note.Location = new System.Drawing.Point(0, 370);
            this.lbl_Note.Name = "lbl_Note";
            this.lbl_Note.Padding = new System.Windows.Forms.Padding(350, 0, 0, 0);
            this.lbl_Note.Size = new System.Drawing.Size(1280, 300);
            this.lbl_Note.TabIndex = 219;
            this.lbl_Note.Text = "note";
            this.lbl_Note.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vwMain_Msg
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(123)))), ((int)(((byte)(135)))));
            this.Controls.Add(this.lbl_Note);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.btn_Can);
            this.Controls.Add(this.lbl_Title);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "vwMain_Msg";
            this.Size = new System.Drawing.Size(1280, 1020);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Can;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label lbl_Note;
    }
}
