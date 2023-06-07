namespace LicManager
{
    partial class LicenseManager
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.txtLicenseFilePath = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnFile = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.rtxtContents = new System.Windows.Forms.RichTextBox();
            this.digFile = new System.Windows.Forms.OpenFileDialog();
            this.cbIsSecureFile = new System.Windows.Forms.CheckBox();
            this.panelTop.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.cbIsSecureFile);
            this.panelTop.Controls.Add(this.txtLicenseFilePath);
            this.panelTop.Controls.Add(this.panel2);
            this.panelTop.Controls.Add(this.panel4);
            this.panelTop.Controls.Add(this.panel3);
            this.panelTop.Controls.Add(this.panel1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(778, 53);
            this.panelTop.TabIndex = 0;
            // 
            // txtLicenseFilePath
            // 
            this.txtLicenseFilePath.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtLicenseFilePath.Location = new System.Drawing.Point(10, 10);
            this.txtLicenseFilePath.Name = "txtLicenseFilePath";
            this.txtLicenseFilePath.ReadOnly = true;
            this.txtLicenseFilePath.Size = new System.Drawing.Size(470, 21);
            this.txtLicenseFilePath.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(10, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(470, 10);
            this.panel2.TabIndex = 5;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnFile);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(480, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(98, 53);
            this.panel4.TabIndex = 7;
            // 
            // btnFile
            // 
            this.btnFile.BackColor = System.Drawing.Color.DimGray;
            this.btnFile.Enabled = false;
            this.btnFile.ForeColor = System.Drawing.Color.White;
            this.btnFile.Location = new System.Drawing.Point(6, 7);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(75, 37);
            this.btnFile.TabIndex = 3;
            this.btnFile.Text = "FILE";
            this.btnFile.UseVisualStyleBackColor = false;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnSave);
            this.panel3.Controls.Add(this.btnLoad);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(578, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 53);
            this.panel3.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.DimGray;
            this.btnSave.Enabled = false;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(114, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 37);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.DimGray;
            this.btnLoad.Enabled = false;
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(26, 7);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 37);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "LOAD";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 53);
            this.panel1.TabIndex = 4;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.rtxtContents);
            this.panelBottom.Controls.Add(this.panel7);
            this.panelBottom.Controls.Add(this.panel5);
            this.panelBottom.Controls.Add(this.panel6);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(0, 53);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(778, 518);
            this.panelBottom.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(10, 508);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(758, 10);
            this.panel7.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(768, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(10, 518);
            this.panel5.TabIndex = 6;
            // 
            // panel6
            // 
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(10, 518);
            this.panel6.TabIndex = 5;
            // 
            // rtxtContents
            // 
            this.rtxtContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtContents.Location = new System.Drawing.Point(10, 0);
            this.rtxtContents.Margin = new System.Windows.Forms.Padding(5);
            this.rtxtContents.Name = "rtxtContents";
            this.rtxtContents.Size = new System.Drawing.Size(758, 508);
            this.rtxtContents.TabIndex = 0;
            this.rtxtContents.Text = "";
            this.rtxtContents.WordWrap = false;
            this.rtxtContents.TextChanged += new System.EventHandler(this.rtxtContents_TextChanged);
            // 
            // digFile
            // 
            this.digFile.FileName = "openFileDialog1";
            // 
            // cbIsSecureFile
            // 
            this.cbIsSecureFile.AutoSize = true;
            this.cbIsSecureFile.Checked = true;
            this.cbIsSecureFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsSecureFile.Location = new System.Drawing.Point(391, 35);
            this.cbIsSecureFile.Name = "cbIsSecureFile";
            this.cbIsSecureFile.Size = new System.Drawing.Size(84, 16);
            this.cbIsSecureFile.TabIndex = 8;
            this.cbIsSecureFile.Text = "SecureFile";
            this.cbIsSecureFile.UseVisualStyleBackColor = true;
            // 
            // LicenseManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 571);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 39);
            this.Name = "LicenseManager";
            this.Text = "SUHWOO LICENSE MANAGER";
            this.Load += new System.EventHandler(this.LicenseManager_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LicenseManager_KeyDown);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox txtLicenseFilePath;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.RichTextBox rtxtContents;
        private System.Windows.Forms.OpenFileDialog digFile;
        private System.Windows.Forms.CheckBox cbIsSecureFile;
    }
}

