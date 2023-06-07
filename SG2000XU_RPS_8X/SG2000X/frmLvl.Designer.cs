namespace SG2000X
{
    partial class frmLvl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLvl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.btnM = new System.Windows.Forms.Button();
            this.btnE = new System.Windows.Forms.Button();
            this.btnO = new System.Windows.Forms.Button();
            this.label35 = new System.Windows.Forms.Label();
            this.btn_AddUser = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnAddUser);
            this.panel1.Controls.Add(this.btnM);
            this.panel1.Controls.Add(this.btnE);
            this.panel1.Controls.Add(this.btnO);
            this.panel1.Controls.Add(this.label35);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btnAddUser
            // 
            resources.ApplyResources(this.btnAddUser, "btnAddUser");
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // btnM
            // 
            this.btnM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.btnM.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM, "btnM");
            this.btnM.ForeColor = System.Drawing.Color.Black;
            this.btnM.Image = global::SG2000X.Properties.Resources.Manager64;
            this.btnM.Name = "btnM";
            this.btnM.Tag = "2";
            this.btnM.UseVisualStyleBackColor = false;
            this.btnM.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnE
            // 
            this.btnE.BackColor = System.Drawing.Color.Orange;
            this.btnE.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnE, "btnE");
            this.btnE.ForeColor = System.Drawing.Color.Black;
            this.btnE.Image = global::SG2000X.Properties.Resources.Engineer64;
            this.btnE.Name = "btnE";
            this.btnE.Tag = "1";
            this.btnE.UseVisualStyleBackColor = false;
            this.btnE.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnO
            // 
            this.btnO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.btnO.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnO, "btnO");
            this.btnO.ForeColor = System.Drawing.Color.Black;
            this.btnO.Image = global::SG2000X.Properties.Resources.Operator64;
            this.btnO.Name = "btnO";
            this.btnO.Tag = "0";
            this.btnO.UseVisualStyleBackColor = false;
            this.btnO.Click += new System.EventHandler(this.btn_Click);
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(167)))), ((int)(((byte)(205)))));
            resources.ApplyResources(this.label35, "label35");
            this.label35.ForeColor = System.Drawing.Color.White;
            this.label35.Name = "label35";
            // 
            // btn_AddUser
            // 
            resources.ApplyResources(this.btnAddUser, "btnAddUser");
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // frmLvl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(123)))), ((int)(((byte)(135)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLvl";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Button btnO;
        private System.Windows.Forms.Button btnM;
        private System.Windows.Forms.Button btnE;
        private System.Windows.Forms.Button btn_AddUser;
        private System.Windows.Forms.Button btnAddUser;
    }
}