namespace SG2000X
{
    partial class vwMan_06Grd
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
            // Dispose시 함수내의 모든 변수 해제
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_06Grd));
            this.ckbGRD_IOOn = new System.Windows.Forms.CheckBox();
            this.btnGrd_DrsReplace = new System.Windows.Forms.Button();
            this.btnGrd_H = new System.Windows.Forms.Button();
            this.btnGrd_Wait = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGRD_DualDressing = new System.Windows.Forms.Button();
            this.btnGRD_DualGrinding = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ckbGRD_IOOn
            // 
            resources.ApplyResources(this.ckbGRD_IOOn, "ckbGRD_IOOn");
            this.ckbGRD_IOOn.BackColor = System.Drawing.Color.LightGray;
            this.ckbGRD_IOOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbGRD_IOOn.FlatAppearance.BorderSize = 2;
            this.ckbGRD_IOOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbGRD_IOOn.ForeColor = System.Drawing.Color.Black;
            this.ckbGRD_IOOn.Name = "ckbGRD_IOOn";
            this.ckbGRD_IOOn.Tag = "7";
            this.ckbGRD_IOOn.UseVisualStyleBackColor = false;
            this.ckbGRD_IOOn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // btnGrd_DrsReplace
            // 
            this.btnGrd_DrsReplace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnGrd_DrsReplace.FlatAppearance.BorderSize = 0;
            this.btnGrd_DrsReplace.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btnGrd_DrsReplace.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.btnGrd_DrsReplace, "btnGrd_DrsReplace");
            this.btnGrd_DrsReplace.ForeColor = System.Drawing.Color.White;
            this.btnGrd_DrsReplace.Name = "btnGrd_DrsReplace";
            this.btnGrd_DrsReplace.Tag = "GRD_DRESSER_REPLACE";
            this.btnGrd_DrsReplace.UseVisualStyleBackColor = false;
            this.btnGrd_DrsReplace.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btnGrd_H
            // 
            this.btnGrd_H.BackColor = System.Drawing.Color.SteelBlue;
            this.btnGrd_H.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnGrd_H, "btnGrd_H");
            this.btnGrd_H.ForeColor = System.Drawing.Color.White;
            this.btnGrd_H.Image = global::SG2000X.Properties.Resources.Home64;
            this.btnGrd_H.Name = "btnGrd_H";
            this.btnGrd_H.Tag = "GRD_Home";
            this.btnGrd_H.UseVisualStyleBackColor = false;
            this.btnGrd_H.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btnGrd_Wait
            // 
            this.btnGrd_Wait.BackColor = System.Drawing.Color.SteelBlue;
            this.btnGrd_Wait.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnGrd_Wait, "btnGrd_Wait");
            this.btnGrd_Wait.ForeColor = System.Drawing.Color.White;
            this.btnGrd_Wait.Image = global::SG2000X.Properties.Resources.Wait64;
            this.btnGrd_Wait.Name = "btnGrd_Wait";
            this.btnGrd_Wait.Tag = "GRD_Wait";
            this.btnGrd_Wait.UseVisualStyleBackColor = false;
            this.btnGrd_Wait.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.button1, "button1");
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Name = "button1";
            this.button1.Tag = "GRD_BufferTest";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btnGRD_DualDressing
            // 
            this.btnGRD_DualDressing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnGRD_DualDressing.FlatAppearance.BorderSize = 0;
            this.btnGRD_DualDressing.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btnGRD_DualDressing.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.btnGRD_DualDressing, "btnGRD_DualDressing");
            this.btnGRD_DualDressing.ForeColor = System.Drawing.Color.White;
            this.btnGRD_DualDressing.Name = "btnGRD_DualDressing";
            this.btnGRD_DualDressing.Tag = "GRD_Dual_Dressing";
            this.btnGRD_DualDressing.UseVisualStyleBackColor = false;
            this.btnGRD_DualDressing.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btnGRD_DualGrinding
            // 
            this.btnGRD_DualGrinding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnGRD_DualGrinding.FlatAppearance.BorderSize = 0;
            this.btnGRD_DualGrinding.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btnGRD_DualGrinding.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.btnGRD_DualGrinding, "btnGRD_DualGrinding");
            this.btnGRD_DualGrinding.ForeColor = System.Drawing.Color.White;
            this.btnGRD_DualGrinding.Name = "btnGRD_DualGrinding";
            this.btnGRD_DualGrinding.Tag = "GRD_Dual_Grinding";
            this.btnGRD_DualGrinding.UseVisualStyleBackColor = false;
            this.btnGRD_DualGrinding.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // vwMan_06Grd
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnGRD_DualGrinding);
            this.Controls.Add(this.btnGRD_DualDressing);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ckbGRD_IOOn);
            this.Controls.Add(this.btnGrd_DrsReplace);
            this.Controls.Add(this.btnGrd_H);
            this.Controls.Add(this.btnGrd_Wait);
            resources.ApplyResources(this, "$this");
            this.Name = "vwMan_06Grd";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbGRD_IOOn;
        private System.Windows.Forms.Button btnGrd_DrsReplace;
        private System.Windows.Forms.Button btnGrd_H;
        private System.Windows.Forms.Button btnGrd_Wait;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnGRD_DualDressing;
        private System.Windows.Forms.Button btnGRD_DualGrinding;
    }
}
