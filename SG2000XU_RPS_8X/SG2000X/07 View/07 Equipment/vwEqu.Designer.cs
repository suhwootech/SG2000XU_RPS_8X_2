namespace SG2000X
{
    partial class vwEqu
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
            _Release();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu));
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.rdbMn_IV2 = new System.Windows.Forms.RadioButton();
            this.rdbMn_Tm8 = new System.Windows.Forms.RadioButton();
            this.rdb_CBM = new System.Windows.Forms.RadioButton();
            this.rdbMn_Light = new System.Windows.Forms.RadioButton();
            this.rdbMn_Df = new System.Windows.Forms.RadioButton();
            this.rdbMn_SGem = new System.Windows.Forms.RadioButton();
            this.rdbMn_Repeat = new System.Windows.Forms.RadioButton();
            this.rdbMn_Bcr = new System.Windows.Forms.RadioButton();
            this.rdbMn_TwL = new System.Windows.Forms.RadioButton();
            this.rdbMn_Spl = new System.Windows.Forms.RadioButton();
            this.rdbMn_Prb = new System.Windows.Forms.RadioButton();
            this.rdbMn_Out = new System.Windows.Forms.RadioButton();
            this.rdbMn_In = new System.Windows.Forms.RadioButton();
            this.rdbMn_Mot = new System.Windows.Forms.RadioButton();
            this.rdbMn_Equ = new System.Windows.Forms.RadioButton();
            this.pnl_Base = new System.Windows.Forms.Panel();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenu
            // 
            this.pnlMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.pnlMenu.Controls.Add(this.rdbMn_IV2);
            this.pnlMenu.Controls.Add(this.rdbMn_Tm8);
            this.pnlMenu.Controls.Add(this.rdb_CBM);
            this.pnlMenu.Controls.Add(this.rdbMn_Light);
            this.pnlMenu.Controls.Add(this.rdbMn_Df);
            this.pnlMenu.Controls.Add(this.rdbMn_SGem);
            this.pnlMenu.Controls.Add(this.rdbMn_Repeat);
            this.pnlMenu.Controls.Add(this.rdbMn_Bcr);
            this.pnlMenu.Controls.Add(this.rdbMn_TwL);
            this.pnlMenu.Controls.Add(this.rdbMn_Spl);
            this.pnlMenu.Controls.Add(this.rdbMn_Prb);
            this.pnlMenu.Controls.Add(this.rdbMn_Out);
            this.pnlMenu.Controls.Add(this.rdbMn_In);
            this.pnlMenu.Controls.Add(this.rdbMn_Mot);
            this.pnlMenu.Controls.Add(this.rdbMn_Equ);
            resources.ApplyResources(this.pnlMenu, "pnlMenu");
            this.pnlMenu.Name = "pnlMenu";
            // 
            // rdbMn_IV2
            // 
            resources.ApplyResources(this.rdbMn_IV2, "rdbMn_IV2");
            this.rdbMn_IV2.FlatAppearance.BorderSize = 0;
            this.rdbMn_IV2.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_IV2.Name = "rdbMn_IV2";
            this.rdbMn_IV2.Tag = "15";
            this.rdbMn_IV2.UseVisualStyleBackColor = true;
            this.rdbMn_IV2.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Tm8
            // 
            resources.ApplyResources(this.rdbMn_Tm8, "rdbMn_Tm8");
            this.rdbMn_Tm8.FlatAppearance.BorderSize = 0;
            this.rdbMn_Tm8.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Tm8.Name = "rdbMn_Tm8";
            this.rdbMn_Tm8.Tag = "14";
            this.rdbMn_Tm8.UseVisualStyleBackColor = true;
            this.rdbMn_Tm8.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdb_CBM
            // 
            resources.ApplyResources(this.rdb_CBM, "rdb_CBM");
            this.rdb_CBM.FlatAppearance.BorderSize = 0;
            this.rdb_CBM.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdb_CBM.Name = "rdb_CBM";
            this.rdb_CBM.Tag = "13";
            this.rdb_CBM.UseVisualStyleBackColor = true;
            this.rdb_CBM.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Light
            // 
            resources.ApplyResources(this.rdbMn_Light, "rdbMn_Light");
            this.rdbMn_Light.FlatAppearance.BorderSize = 0;
            this.rdbMn_Light.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Light.Name = "rdbMn_Light";
            this.rdbMn_Light.Tag = "12";
            this.rdbMn_Light.UseVisualStyleBackColor = true;
            this.rdbMn_Light.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Df
            // 
            resources.ApplyResources(this.rdbMn_Df, "rdbMn_Df");
            this.rdbMn_Df.FlatAppearance.BorderSize = 0;
            this.rdbMn_Df.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Df.Name = "rdbMn_Df";
            this.rdbMn_Df.Tag = "11";
            this.rdbMn_Df.UseVisualStyleBackColor = true;
            this.rdbMn_Df.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_SGem
            // 
            resources.ApplyResources(this.rdbMn_SGem, "rdbMn_SGem");
            this.rdbMn_SGem.FlatAppearance.BorderSize = 0;
            this.rdbMn_SGem.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_SGem.Name = "rdbMn_SGem";
            this.rdbMn_SGem.Tag = "10";
            this.rdbMn_SGem.UseVisualStyleBackColor = true;
            this.rdbMn_SGem.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Repeat
            // 
            resources.ApplyResources(this.rdbMn_Repeat, "rdbMn_Repeat");
            this.rdbMn_Repeat.FlatAppearance.BorderSize = 0;
            this.rdbMn_Repeat.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Repeat.Name = "rdbMn_Repeat";
            this.rdbMn_Repeat.Tag = "9";
            this.rdbMn_Repeat.UseVisualStyleBackColor = true;
            this.rdbMn_Repeat.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Bcr
            // 
            resources.ApplyResources(this.rdbMn_Bcr, "rdbMn_Bcr");
            this.rdbMn_Bcr.FlatAppearance.BorderSize = 0;
            this.rdbMn_Bcr.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Bcr.Name = "rdbMn_Bcr";
            this.rdbMn_Bcr.Tag = "8";
            this.rdbMn_Bcr.UseVisualStyleBackColor = true;
            this.rdbMn_Bcr.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_TwL
            // 
            resources.ApplyResources(this.rdbMn_TwL, "rdbMn_TwL");
            this.rdbMn_TwL.FlatAppearance.BorderSize = 0;
            this.rdbMn_TwL.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_TwL.Name = "rdbMn_TwL";
            this.rdbMn_TwL.Tag = "7";
            this.rdbMn_TwL.UseVisualStyleBackColor = true;
            this.rdbMn_TwL.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Spl
            // 
            resources.ApplyResources(this.rdbMn_Spl, "rdbMn_Spl");
            this.rdbMn_Spl.FlatAppearance.BorderSize = 0;
            this.rdbMn_Spl.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Spl.Name = "rdbMn_Spl";
            this.rdbMn_Spl.Tag = "3";
            this.rdbMn_Spl.UseVisualStyleBackColor = true;
            this.rdbMn_Spl.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Prb
            // 
            resources.ApplyResources(this.rdbMn_Prb, "rdbMn_Prb");
            this.rdbMn_Prb.FlatAppearance.BorderSize = 0;
            this.rdbMn_Prb.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Prb.Name = "rdbMn_Prb";
            this.rdbMn_Prb.Tag = "6";
            this.rdbMn_Prb.UseVisualStyleBackColor = true;
            this.rdbMn_Prb.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Out
            // 
            resources.ApplyResources(this.rdbMn_Out, "rdbMn_Out");
            this.rdbMn_Out.FlatAppearance.BorderSize = 0;
            this.rdbMn_Out.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Out.Name = "rdbMn_Out";
            this.rdbMn_Out.Tag = "5";
            this.rdbMn_Out.UseVisualStyleBackColor = true;
            this.rdbMn_Out.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_In
            // 
            resources.ApplyResources(this.rdbMn_In, "rdbMn_In");
            this.rdbMn_In.FlatAppearance.BorderSize = 0;
            this.rdbMn_In.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_In.Name = "rdbMn_In";
            this.rdbMn_In.Tag = "4";
            this.rdbMn_In.UseVisualStyleBackColor = true;
            this.rdbMn_In.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Mot
            // 
            resources.ApplyResources(this.rdbMn_Mot, "rdbMn_Mot");
            this.rdbMn_Mot.FlatAppearance.BorderSize = 0;
            this.rdbMn_Mot.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Mot.Name = "rdbMn_Mot";
            this.rdbMn_Mot.Tag = "2";
            this.rdbMn_Mot.UseVisualStyleBackColor = true;
            this.rdbMn_Mot.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Equ
            // 
            resources.ApplyResources(this.rdbMn_Equ, "rdbMn_Equ");
            this.rdbMn_Equ.Checked = true;
            this.rdbMn_Equ.FlatAppearance.BorderSize = 0;
            this.rdbMn_Equ.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Equ.Name = "rdbMn_Equ";
            this.rdbMn_Equ.TabStop = true;
            this.rdbMn_Equ.Tag = "1";
            this.rdbMn_Equ.UseVisualStyleBackColor = true;
            this.rdbMn_Equ.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // pnl_Base
            // 
            resources.ApplyResources(this.pnl_Base, "pnl_Base");
            this.pnl_Base.Name = "pnl_Base";
            // 
            // vwEqu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnl_Base);
            this.Controls.Add(this.pnlMenu);
            resources.ApplyResources(this, "$this");
            this.Name = "vwEqu";
            this.pnlMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.RadioButton rdbMn_Spl;
        private System.Windows.Forms.RadioButton rdbMn_Prb;
        private System.Windows.Forms.RadioButton rdbMn_Out;
        private System.Windows.Forms.RadioButton rdbMn_In;
        private System.Windows.Forms.RadioButton rdbMn_Mot;
        private System.Windows.Forms.RadioButton rdbMn_Equ;
        private System.Windows.Forms.RadioButton rdbMn_TwL;
        private System.Windows.Forms.RadioButton rdbMn_Bcr;
        private System.Windows.Forms.RadioButton rdbMn_Repeat;
        private System.Windows.Forms.RadioButton rdbMn_SGem;
        private System.Windows.Forms.RadioButton rdbMn_Df;
        private System.Windows.Forms.Panel pnl_Base;
        private System.Windows.Forms.RadioButton rdbMn_Light;
        private System.Windows.Forms.RadioButton rdb_CBM;
        private System.Windows.Forms.RadioButton rdbMn_Tm8;
        private System.Windows.Forms.RadioButton rdbMn_IV2;
    }
}
