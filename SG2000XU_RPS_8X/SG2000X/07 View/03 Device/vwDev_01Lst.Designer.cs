namespace SG2000X
{
    partial class vwDev_01Lst
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwDev_01Lst));
            this.pnlM_Dev = new System.Windows.Forms.Panel();
            this.btnM_DCur = new System.Windows.Forms.Button();
            this.btnM_DDel = new System.Windows.Forms.Button();
            this.btnM_DApp = new System.Windows.Forms.Button();
            this.btnM_DCopy = new System.Windows.Forms.Button();
            this.btnM_DNew = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbxM_Dev = new System.Windows.Forms.ListBox();
            this.pnlM_Grp = new System.Windows.Forms.Panel();
            this.btnM_GDel = new System.Windows.Forms.Button();
            this.btnM_GCopy = new System.Windows.Forms.Button();
            this.btnM_GNew = new System.Windows.Forms.Button();
            this.label39 = new System.Windows.Forms.Label();
            this.lbxM_Grp = new System.Windows.Forms.ListBox();
            this.btnM_DBcr = new System.Windows.Forms.Button();
            this.pnlM_Dev.SuspendLayout();
            this.pnlM_Grp.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlM_Dev
            // 
            this.pnlM_Dev.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlM_Dev.Controls.Add(this.btnM_DBcr);
            this.pnlM_Dev.Controls.Add(this.btnM_DCur);
            this.pnlM_Dev.Controls.Add(this.btnM_DDel);
            this.pnlM_Dev.Controls.Add(this.btnM_DApp);
            this.pnlM_Dev.Controls.Add(this.btnM_DCopy);
            this.pnlM_Dev.Controls.Add(this.btnM_DNew);
            this.pnlM_Dev.Controls.Add(this.label2);
            this.pnlM_Dev.Controls.Add(this.lbxM_Dev);
            resources.ApplyResources(this.pnlM_Dev, "pnlM_Dev");
            this.pnlM_Dev.Name = "pnlM_Dev";
            // 
            // btnM_DCur
            // 
            this.btnM_DCur.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_DCur.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_DCur, "btnM_DCur");
            this.btnM_DCur.ForeColor = System.Drawing.Color.White;
            this.btnM_DCur.Name = "btnM_DCur";
            this.btnM_DCur.UseVisualStyleBackColor = false;
            this.btnM_DCur.Click += new System.EventHandler(this.btnM_DCur_Click);
            // 
            // btnM_DDel
            // 
            this.btnM_DDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_DDel.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_DDel, "btnM_DDel");
            this.btnM_DDel.ForeColor = System.Drawing.Color.White;
            this.btnM_DDel.Name = "btnM_DDel";
            this.btnM_DDel.UseVisualStyleBackColor = false;
            this.btnM_DDel.Click += new System.EventHandler(this.btnM_RDel_Click);
            // 
            // btnM_DApp
            // 
            this.btnM_DApp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_DApp.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_DApp, "btnM_DApp");
            this.btnM_DApp.ForeColor = System.Drawing.Color.White;
            this.btnM_DApp.Name = "btnM_DApp";
            this.btnM_DApp.UseVisualStyleBackColor = false;
            this.btnM_DApp.Click += new System.EventHandler(this.btnM_RApp_Click);
            // 
            // btnM_DCopy
            // 
            this.btnM_DCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_DCopy.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_DCopy, "btnM_DCopy");
            this.btnM_DCopy.ForeColor = System.Drawing.Color.White;
            this.btnM_DCopy.Name = "btnM_DCopy";
            this.btnM_DCopy.UseVisualStyleBackColor = false;
            this.btnM_DCopy.Click += new System.EventHandler(this.btnM_RCopy_Click);
            // 
            // btnM_DNew
            // 
            this.btnM_DNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_DNew.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_DNew, "btnM_DNew");
            this.btnM_DNew.ForeColor = System.Drawing.Color.White;
            this.btnM_DNew.Name = "btnM_DNew";
            this.btnM_DNew.UseVisualStyleBackColor = false;
            this.btnM_DNew.Click += new System.EventHandler(this.btnM_RNew_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // lbxM_Dev
            // 
            this.lbxM_Dev.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.lbxM_Dev, "lbxM_Dev");
            this.lbxM_Dev.Name = "lbxM_Dev";
            this.lbxM_Dev.TabStop = false;
            this.lbxM_Dev.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbx_DrawItem);
            this.lbxM_Dev.SelectedIndexChanged += new System.EventHandler(this.libM_Rcp_SelectedIndexChanged);
            // 
            // pnlM_Grp
            // 
            this.pnlM_Grp.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlM_Grp.Controls.Add(this.btnM_GDel);
            this.pnlM_Grp.Controls.Add(this.btnM_GCopy);
            this.pnlM_Grp.Controls.Add(this.btnM_GNew);
            this.pnlM_Grp.Controls.Add(this.label39);
            this.pnlM_Grp.Controls.Add(this.lbxM_Grp);
            resources.ApplyResources(this.pnlM_Grp, "pnlM_Grp");
            this.pnlM_Grp.Name = "pnlM_Grp";
            // 
            // btnM_GDel
            // 
            this.btnM_GDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_GDel.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_GDel, "btnM_GDel");
            this.btnM_GDel.ForeColor = System.Drawing.Color.White;
            this.btnM_GDel.Name = "btnM_GDel";
            this.btnM_GDel.UseVisualStyleBackColor = false;
            this.btnM_GDel.Click += new System.EventHandler(this.btnM_GDel_Click);
            // 
            // btnM_GCopy
            // 
            this.btnM_GCopy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_GCopy.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_GCopy, "btnM_GCopy");
            this.btnM_GCopy.ForeColor = System.Drawing.Color.White;
            this.btnM_GCopy.Name = "btnM_GCopy";
            this.btnM_GCopy.UseVisualStyleBackColor = false;
            this.btnM_GCopy.Click += new System.EventHandler(this.btnM_GCopy_Click);
            // 
            // btnM_GNew
            // 
            this.btnM_GNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_GNew.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_GNew, "btnM_GNew");
            this.btnM_GNew.ForeColor = System.Drawing.Color.White;
            this.btnM_GNew.Name = "btnM_GNew";
            this.btnM_GNew.UseVisualStyleBackColor = false;
            this.btnM_GNew.Click += new System.EventHandler(this.btnM_GNew_Click);
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label39, "label39");
            this.label39.ForeColor = System.Drawing.Color.White;
            this.label39.Name = "label39";
            // 
            // lbxM_Grp
            // 
            this.lbxM_Grp.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.lbxM_Grp, "lbxM_Grp");
            this.lbxM_Grp.Name = "lbxM_Grp";
            this.lbxM_Grp.TabStop = false;
            this.lbxM_Grp.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbx_DrawItem);
            this.lbxM_Grp.SelectedIndexChanged += new System.EventHandler(this.lbxM_Grp_SelectedIndexChanged);
            // 
            // btnM_DBcr
            // 
            this.btnM_DBcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnM_DBcr.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnM_DBcr, "btnM_DBcr");
            this.btnM_DBcr.ForeColor = System.Drawing.Color.White;
            this.btnM_DBcr.Name = "btnM_DBcr";
            this.btnM_DBcr.UseVisualStyleBackColor = false;
            this.btnM_DBcr.Click += new System.EventHandler(this.btnM_DBcr_Click);
            // 
            // vwDev_01Lst
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlM_Dev);
            this.Controls.Add(this.pnlM_Grp);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.Name = "vwDev_01Lst";
            this.pnlM_Dev.ResumeLayout(false);
            this.pnlM_Grp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlM_Dev;
        private System.Windows.Forms.Button btnM_DCur;
        private System.Windows.Forms.Button btnM_DDel;
        private System.Windows.Forms.Button btnM_DApp;
        private System.Windows.Forms.Button btnM_DCopy;
        private System.Windows.Forms.Button btnM_DNew;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbxM_Dev;
        private System.Windows.Forms.Panel pnlM_Grp;
        private System.Windows.Forms.Button btnM_GDel;
        private System.Windows.Forms.Button btnM_GCopy;
        private System.Windows.Forms.Button btnM_GNew;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ListBox lbxM_Grp;
        private System.Windows.Forms.Button btnM_DBcr;
    }
}
