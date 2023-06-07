namespace SG2000X
{
    partial class vwDev
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
            this.Release();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwDev));
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.btn_Save = new System.Windows.Forms.Button();
            this.rdbMn_Set = new System.Windows.Forms.RadioButton();
            this.rdbMn_Parm = new System.Windows.Forms.RadioButton();
            this.rdbMn_List = new System.Windows.Forms.RadioButton();
            this.pnl_Base = new System.Windows.Forms.Panel();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenu
            // 
            this.pnlMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.pnlMenu.Controls.Add(this.btn_Save);
            this.pnlMenu.Controls.Add(this.rdbMn_Set);
            this.pnlMenu.Controls.Add(this.rdbMn_Parm);
            this.pnlMenu.Controls.Add(this.rdbMn_List);
            resources.ApplyResources(this.pnlMenu, "pnlMenu");
            this.pnlMenu.Name = "pnlMenu";
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btn_Save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.ForeColor = System.Drawing.Color.Black;
            this.btn_Save.Image = global::SG2000X.Properties.Resources.Save64_Black;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // rdbMn_Set
            // 
            resources.ApplyResources(this.rdbMn_Set, "rdbMn_Set");
            this.rdbMn_Set.FlatAppearance.BorderSize = 0;
            this.rdbMn_Set.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Set.Name = "rdbMn_Set";
            this.rdbMn_Set.Tag = "3";
            this.rdbMn_Set.UseVisualStyleBackColor = true;
            this.rdbMn_Set.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_Parm
            // 
            resources.ApplyResources(this.rdbMn_Parm, "rdbMn_Parm");
            this.rdbMn_Parm.FlatAppearance.BorderSize = 0;
            this.rdbMn_Parm.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_Parm.Name = "rdbMn_Parm";
            this.rdbMn_Parm.Tag = "2";
            this.rdbMn_Parm.UseVisualStyleBackColor = true;
            this.rdbMn_Parm.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // rdbMn_List
            // 
            resources.ApplyResources(this.rdbMn_List, "rdbMn_List");
            this.rdbMn_List.Checked = true;
            this.rdbMn_List.FlatAppearance.BorderSize = 0;
            this.rdbMn_List.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.rdbMn_List.Name = "rdbMn_List";
            this.rdbMn_List.TabStop = true;
            this.rdbMn_List.Tag = "1";
            this.rdbMn_List.UseVisualStyleBackColor = true;
            this.rdbMn_List.CheckedChanged += new System.EventHandler(this.rdbMn_CheckedChanged);
            // 
            // pnl_Base
            // 
            this.pnl_Base.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pnl_Base, "pnl_Base");
            this.pnl_Base.Name = "pnl_Base";
            // 
            // vwDev2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnl_Base);
            this.Controls.Add(this.pnlMenu);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.Name = "vwDev2";
            this.pnlMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.RadioButton rdbMn_Set;
        private System.Windows.Forms.RadioButton rdbMn_Parm;
        private System.Windows.Forms.RadioButton rdbMn_List;
        private System.Windows.Forms.Panel pnl_Base;
    }
}
