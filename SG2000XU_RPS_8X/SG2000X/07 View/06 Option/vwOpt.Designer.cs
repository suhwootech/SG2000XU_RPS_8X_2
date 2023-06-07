namespace SG2000X
{
    partial class vwOpt
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
            Release();

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwOpt));
			this.pnlMenu = new System.Windows.Forms.Panel();
			this.rdb_Mnt = new System.Windows.Forms.RadioButton();
			this.rdb_TbG = new System.Windows.Forms.RadioButton();
			this.rdb_LvS = new System.Windows.Forms.RadioButton();
			this.rdb_Sys = new System.Windows.Forms.RadioButton();
			this.rdb_Grd = new System.Windows.Forms.RadioButton();
			this.rdb_Pck = new System.Windows.Forms.RadioButton();
			this.rdb_Dry = new System.Windows.Forms.RadioButton();
			this.rdb_Ldr = new System.Windows.Forms.RadioButton();
			this.btn_Save = new System.Windows.Forms.Button();
			this.rdb_Gen = new System.Windows.Forms.RadioButton();
			this.pnl_Base = new System.Windows.Forms.Panel();
			this.pnlMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMenu
			// 
			this.pnlMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
			this.pnlMenu.Controls.Add(this.rdb_Mnt);
			this.pnlMenu.Controls.Add(this.rdb_TbG);
			this.pnlMenu.Controls.Add(this.rdb_LvS);
			this.pnlMenu.Controls.Add(this.rdb_Sys);
			this.pnlMenu.Controls.Add(this.rdb_Grd);
			this.pnlMenu.Controls.Add(this.rdb_Pck);
			this.pnlMenu.Controls.Add(this.rdb_Dry);
			this.pnlMenu.Controls.Add(this.rdb_Ldr);
			this.pnlMenu.Controls.Add(this.btn_Save);
			this.pnlMenu.Controls.Add(this.rdb_Gen);
			resources.ApplyResources(this.pnlMenu, "pnlMenu");
			this.pnlMenu.Name = "pnlMenu";
			// 
			// rdb_Mnt
			// 
			resources.ApplyResources(this.rdb_Mnt, "rdb_Mnt");
			this.rdb_Mnt.FlatAppearance.BorderSize = 0;
			this.rdb_Mnt.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Mnt.Name = "rdb_Mnt";
			this.rdb_Mnt.Tag = "9";
			this.rdb_Mnt.UseVisualStyleBackColor = true;
			this.rdb_Mnt.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_TbG
			// 
			resources.ApplyResources(this.rdb_TbG, "rdb_TbG");
			this.rdb_TbG.FlatAppearance.BorderSize = 0;
			this.rdb_TbG.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_TbG.Name = "rdb_TbG";
			this.rdb_TbG.Tag = "8";
			this.rdb_TbG.UseVisualStyleBackColor = true;
			this.rdb_TbG.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_LvS
			// 
			resources.ApplyResources(this.rdb_LvS, "rdb_LvS");
			this.rdb_LvS.FlatAppearance.BorderSize = 0;
			this.rdb_LvS.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_LvS.Name = "rdb_LvS";
			this.rdb_LvS.Tag = "6";
			this.rdb_LvS.UseVisualStyleBackColor = true;
			this.rdb_LvS.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_Sys
			// 
			resources.ApplyResources(this.rdb_Sys, "rdb_Sys");
			this.rdb_Sys.FlatAppearance.BorderSize = 0;
			this.rdb_Sys.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Sys.Name = "rdb_Sys";
			this.rdb_Sys.Tag = "7";
			this.rdb_Sys.UseVisualStyleBackColor = true;
			this.rdb_Sys.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_Grd
			// 
			resources.ApplyResources(this.rdb_Grd, "rdb_Grd");
			this.rdb_Grd.FlatAppearance.BorderSize = 0;
			this.rdb_Grd.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Grd.Name = "rdb_Grd";
			this.rdb_Grd.Tag = "5";
			this.rdb_Grd.UseVisualStyleBackColor = true;
			this.rdb_Grd.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_Pck
			// 
			resources.ApplyResources(this.rdb_Pck, "rdb_Pck");
			this.rdb_Pck.FlatAppearance.BorderSize = 0;
			this.rdb_Pck.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Pck.Name = "rdb_Pck";
			this.rdb_Pck.Tag = "4";
			this.rdb_Pck.UseVisualStyleBackColor = true;
			this.rdb_Pck.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_Dry
			// 
			resources.ApplyResources(this.rdb_Dry, "rdb_Dry");
			this.rdb_Dry.FlatAppearance.BorderSize = 0;
			this.rdb_Dry.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Dry.Name = "rdb_Dry";
			this.rdb_Dry.Tag = "3";
			this.rdb_Dry.UseVisualStyleBackColor = true;
			this.rdb_Dry.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// rdb_Ldr
			// 
			resources.ApplyResources(this.rdb_Ldr, "rdb_Ldr");
			this.rdb_Ldr.FlatAppearance.BorderSize = 0;
			this.rdb_Ldr.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Ldr.Name = "rdb_Ldr";
			this.rdb_Ldr.Tag = "2";
			this.rdb_Ldr.UseVisualStyleBackColor = true;
			this.rdb_Ldr.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// btn_Save
			// 
			this.btn_Save.BackColor = System.Drawing.Color.Transparent;
			this.btn_Save.FlatAppearance.BorderSize = 0;
			this.btn_Save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this.btn_Save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.btn_Save, "btn_Save");
			this.btn_Save.Image = global::SG2000X.Properties.Resources.Save64_Black;
			this.btn_Save.Name = "btn_Save";
			this.btn_Save.UseVisualStyleBackColor = false;
			this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
			// 
			// rdb_Gen
			// 
			resources.ApplyResources(this.rdb_Gen, "rdb_Gen");
			this.rdb_Gen.Checked = true;
			this.rdb_Gen.FlatAppearance.BorderSize = 0;
			this.rdb_Gen.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this.rdb_Gen.Name = "rdb_Gen";
			this.rdb_Gen.TabStop = true;
			this.rdb_Gen.Tag = "1";
			this.rdb_Gen.UseVisualStyleBackColor = true;
			this.rdb_Gen.CheckedChanged += new System.EventHandler(this.rdb_CheckedChanged);
			// 
			// pnl_Base
			// 
			this.pnl_Base.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.pnl_Base, "pnl_Base");
			this.pnl_Base.Name = "pnl_Base";
			// 
			// vwOpt
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.pnl_Base);
			this.Controls.Add(this.pnlMenu);
			resources.ApplyResources(this, "$this");
			this.Name = "vwOpt";
			this.pnlMenu.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.RadioButton rdb_TbG;
        private System.Windows.Forms.RadioButton rdb_Sys;
        private System.Windows.Forms.RadioButton rdb_Gen;
        private System.Windows.Forms.RadioButton rdb_LvS;
        private System.Windows.Forms.Panel pnl_Base;
        private System.Windows.Forms.RadioButton rdb_Ldr;
        private System.Windows.Forms.RadioButton rdb_Grd;
        private System.Windows.Forms.RadioButton rdb_Pck;
        private System.Windows.Forms.RadioButton rdb_Dry;
		private System.Windows.Forms.RadioButton rdb_Mnt;
	}
}
