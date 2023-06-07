namespace SG2000X
{
    partial class frmSph
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
            this.lbl_Title = new System.Windows.Forms.Label();
            this.lbl_Ver = new System.Windows.Forms.Label();
            this.lbl_Txt = new System.Windows.Forms.Label();
            this.lbl_Copy = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            this.lbl_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lbl_Title.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.White;
            this.lbl_Title.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_Title.Location = new System.Drawing.Point(20, 20);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(466, 60);
            this.lbl_Title.TabIndex = 5;
            this.lbl_Title.Text = "SG - 2000X";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Ver
            // 
            this.lbl_Ver.AutoSize = true;
            this.lbl_Ver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lbl_Ver.Font = new System.Drawing.Font("Arial Unicode MS", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Ver.ForeColor = System.Drawing.Color.White;
            this.lbl_Ver.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_Ver.Location = new System.Drawing.Point(25, 70);
            this.lbl_Ver.Name = "lbl_Ver";
            this.lbl_Ver.Size = new System.Drawing.Size(76, 21);
            this.lbl_Ver.TabIndex = 6;
            this.lbl_Ver.Text = "Version ";
            this.lbl_Ver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Txt
            // 
            this.lbl_Txt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lbl_Txt.Font = new System.Drawing.Font("Arial Unicode MS", 12F);
            this.lbl_Txt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lbl_Txt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_Txt.Location = new System.Drawing.Point(140, 230);
            this.lbl_Txt.Name = "lbl_Txt";
            this.lbl_Txt.Size = new System.Drawing.Size(445, 25);
            this.lbl_Txt.TabIndex = 7;
            this.lbl_Txt.Text = "-";
            this.lbl_Txt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_Copy
            // 
            this.lbl_Copy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lbl_Copy.Font = new System.Drawing.Font("Arial Unicode MS", 10F);
            this.lbl_Copy.ForeColor = System.Drawing.Color.White;
            this.lbl_Copy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_Copy.Location = new System.Drawing.Point(140, 255);
            this.lbl_Copy.Name = "lbl_Copy";
            this.lbl_Copy.Size = new System.Drawing.Size(445, 25);
            this.lbl_Copy.TabIndex = 8;
            this.lbl_Copy.Text = "-";
            this.lbl_Copy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmSph
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.ClientSize = new System.Drawing.Size(600, 300);
            this.Controls.Add(this.lbl_Copy);
            this.Controls.Add(this.lbl_Txt);
            this.Controls.Add(this.lbl_Ver);
            this.Controls.Add(this.lbl_Title);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmSph";
            this.Text = "frmSph";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label lbl_Ver;
        private System.Windows.Forms.Label lbl_Txt;
        private System.Windows.Forms.Label lbl_Copy;
    }
}