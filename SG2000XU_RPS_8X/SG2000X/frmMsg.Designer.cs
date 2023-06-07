namespace SG2000X
{
    partial class frmMsg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMsg));
			this.lbl_Title = new System.Windows.Forms.Label();
			this.btn_Can = new System.Windows.Forms.Button();
			this.btn_Ok = new System.Windows.Forms.Button();
			this.tipName = new System.Windows.Forms.ToolTip(this.components);
			this.lbl_Msg = new System.Windows.Forms.Label();
			this.lblRestTime = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lbl_Title
			// 
			this.lbl_Title.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lbl_Title, "lbl_Title");
			this.lbl_Title.ForeColor = System.Drawing.Color.Black;
			this.lbl_Title.Name = "lbl_Title";
			// 
			// btn_Can
			// 
			this.btn_Can.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_Can.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_Can, "btn_Can");
			this.btn_Can.ForeColor = System.Drawing.Color.White;
			this.btn_Can.Name = "btn_Can";
			this.btn_Can.Tag = "";
			this.btn_Can.UseVisualStyleBackColor = false;
			this.btn_Can.Click += new System.EventHandler(this.btn_Can_Click);
			// 
			// btn_Ok
			// 
			this.btn_Ok.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_Ok.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_Ok, "btn_Ok");
			this.btn_Ok.ForeColor = System.Drawing.Color.White;
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Tag = "";
			this.btn_Ok.UseVisualStyleBackColor = false;
			this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
			// 
			// tipName
			// 
			this.tipName.ForeColor = System.Drawing.Color.Red;
			this.tipName.ShowAlways = true;
			// 
			// lbl_Msg
			// 
			this.lbl_Msg.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lbl_Msg, "lbl_Msg");
			this.lbl_Msg.ForeColor = System.Drawing.Color.Black;
			this.lbl_Msg.Name = "lbl_Msg";
			// 
			// lblRestTime
			// 
			resources.ApplyResources(this.lblRestTime, "lblRestTime");
			this.lblRestTime.Name = "lblRestTime";
			// 
			// frmMsg
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(123)))), ((int)(((byte)(135)))));
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblRestTime);
			this.Controls.Add(this.lbl_Msg);
			this.Controls.Add(this.btn_Ok);
			this.Controls.Add(this.btn_Can);
			this.Controls.Add(this.lbl_Title);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmMsg";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Button btn_Can;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.ToolTip tipName;
        private System.Windows.Forms.Label lbl_Msg;
		private System.Windows.Forms.Label lblRestTime;
	}
}