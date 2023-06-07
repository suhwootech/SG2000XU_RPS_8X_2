namespace SG2000X
{
    partial class vwOpt_03Dry
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwOpt_03Dry));
			this.label313 = new System.Windows.Forms.Label();
			this.txt_SpgWtOnMin = new Extended.ExTextBox();
			this.lbl_SpgWtOnMin = new System.Windows.Forms.Label();
			this.label311 = new System.Windows.Forms.Label();
			this.txt_SpgWtOffMin = new Extended.ExTextBox();
			this.lbl_SpgWtOffMin = new System.Windows.Forms.Label();
			this.pnl_SpongeOnOff = new System.Windows.Forms.Panel();
			this.label7 = new System.Windows.Forms.Label();
			this.panel43 = new System.Windows.Forms.Panel();
			this.ckb_SpwSkip = new System.Windows.Forms.CheckBox();
			this.ckb_DryStickSkip = new System.Windows.Forms.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.pnl_DelayTime = new System.Windows.Forms.Panel();
			this.label299 = new System.Windows.Forms.Label();
			this.label300 = new System.Windows.Forms.Label();
			this.txt_ZUpStableDelay = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.ckb_SpgWtAutoOnOff = new System.Windows.Forms.CheckBox();
			this.pnl_SpongeOnOff.SuspendLayout();
			this.panel43.SuspendLayout();
			this.pnl_DelayTime.SuspendLayout();
			this.SuspendLayout();
			// 
			// label313
			// 
			resources.ApplyResources(this.label313, "label313");
			this.label313.ForeColor = System.Drawing.Color.Black;
			this.label313.Name = "label313";
			// 
			// txt_SpgWtOnMin
			// 
			this.txt_SpgWtOnMin.ExMax = 60D;
			this.txt_SpgWtOnMin.ExMin = 1D;
			this.txt_SpgWtOnMin.ExSigned = Extended.ExSign.Signed;
			this.txt_SpgWtOnMin.ExType = Extended.ExTxtType.Double;
			resources.ApplyResources(this.txt_SpgWtOnMin, "txt_SpgWtOnMin");
			this.txt_SpgWtOnMin.Name = "txt_SpgWtOnMin";
			// 
			// lbl_SpgWtOnMin
			// 
			resources.ApplyResources(this.lbl_SpgWtOnMin, "lbl_SpgWtOnMin");
			this.lbl_SpgWtOnMin.ForeColor = System.Drawing.Color.Black;
			this.lbl_SpgWtOnMin.Name = "lbl_SpgWtOnMin";
			// 
			// label311
			// 
			resources.ApplyResources(this.label311, "label311");
			this.label311.ForeColor = System.Drawing.Color.Black;
			this.label311.Name = "label311";
			// 
			// txt_SpgWtOffMin
			// 
			this.txt_SpgWtOffMin.ExMax = 300D;
			this.txt_SpgWtOffMin.ExMin = 1D;
			this.txt_SpgWtOffMin.ExSigned = Extended.ExSign.Signed;
			this.txt_SpgWtOffMin.ExType = Extended.ExTxtType.Double;
			resources.ApplyResources(this.txt_SpgWtOffMin, "txt_SpgWtOffMin");
			this.txt_SpgWtOffMin.Name = "txt_SpgWtOffMin";
			// 
			// lbl_SpgWtOffMin
			// 
			resources.ApplyResources(this.lbl_SpgWtOffMin, "lbl_SpgWtOffMin");
			this.lbl_SpgWtOffMin.ForeColor = System.Drawing.Color.Black;
			this.lbl_SpgWtOffMin.Name = "lbl_SpgWtOffMin";
			// 
			// pnl_SpongeOnOff
			// 
			this.pnl_SpongeOnOff.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnl_SpongeOnOff.Controls.Add(this.ckb_SpgWtAutoOnOff);
			this.pnl_SpongeOnOff.Controls.Add(this.label313);
			this.pnl_SpongeOnOff.Controls.Add(this.label7);
			this.pnl_SpongeOnOff.Controls.Add(this.txt_SpgWtOnMin);
			this.pnl_SpongeOnOff.Controls.Add(this.lbl_SpgWtOffMin);
			this.pnl_SpongeOnOff.Controls.Add(this.lbl_SpgWtOnMin);
			this.pnl_SpongeOnOff.Controls.Add(this.txt_SpgWtOffMin);
			this.pnl_SpongeOnOff.Controls.Add(this.label311);
			resources.ApplyResources(this.pnl_SpongeOnOff, "pnl_SpongeOnOff");
			this.pnl_SpongeOnOff.Name = "pnl_SpongeOnOff";
			this.pnl_SpongeOnOff.Tag = "0";
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
			resources.ApplyResources(this.label7, "label7");
			this.label7.ForeColor = System.Drawing.Color.White;
			this.label7.Name = "label7";
			// 
			// panel43
			// 
			this.panel43.BackColor = System.Drawing.Color.WhiteSmoke;
			this.panel43.Controls.Add(this.ckb_SpwSkip);
			this.panel43.Controls.Add(this.ckb_DryStickSkip);
			this.panel43.Controls.Add(this.label15);
			resources.ApplyResources(this.panel43, "panel43");
			this.panel43.Name = "panel43";
			this.panel43.Tag = "0";
			// 
			// ckb_SpwSkip
			// 
			this.ckb_SpwSkip.Checked = true;
			this.ckb_SpwSkip.CheckState = System.Windows.Forms.CheckState.Checked;
			resources.ApplyResources(this.ckb_SpwSkip, "ckb_SpwSkip");
			this.ckb_SpwSkip.Name = "ckb_SpwSkip";
			this.ckb_SpwSkip.UseVisualStyleBackColor = true;
			// 
			// ckb_DryStickSkip
			// 
			resources.ApplyResources(this.ckb_DryStickSkip, "ckb_DryStickSkip");
			this.ckb_DryStickSkip.Name = "ckb_DryStickSkip";
			this.ckb_DryStickSkip.UseVisualStyleBackColor = true;
			// 
			// label15
			// 
			this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
			resources.ApplyResources(this.label15, "label15");
			this.label15.ForeColor = System.Drawing.Color.White;
			this.label15.Name = "label15";
			// 
			// pnl_DelayTime
			// 
			this.pnl_DelayTime.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnl_DelayTime.Controls.Add(this.label299);
			this.pnl_DelayTime.Controls.Add(this.label300);
			this.pnl_DelayTime.Controls.Add(this.txt_ZUpStableDelay);
			this.pnl_DelayTime.Controls.Add(this.label2);
			resources.ApplyResources(this.pnl_DelayTime, "pnl_DelayTime");
			this.pnl_DelayTime.Name = "pnl_DelayTime";
			this.pnl_DelayTime.Tag = "0";
			// 
			// label299
			// 
			resources.ApplyResources(this.label299, "label299");
			this.label299.ForeColor = System.Drawing.Color.Black;
			this.label299.Name = "label299";
			// 
			// label300
			// 
			resources.ApplyResources(this.label300, "label300");
			this.label300.ForeColor = System.Drawing.Color.Black;
			this.label300.Name = "label300";
			// 
			// txt_ZUpStableDelay
			// 
			this.txt_ZUpStableDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.txt_ZUpStableDelay, "txt_ZUpStableDelay");
			this.txt_ZUpStableDelay.Name = "txt_ZUpStableDelay";
			this.txt_ZUpStableDelay.Tag = "0";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Name = "label2";
			// 
			// ckb_SpgWtAutoOnOff
			// 
			this.ckb_SpgWtAutoOnOff.Checked = true;
			this.ckb_SpgWtAutoOnOff.CheckState = System.Windows.Forms.CheckState.Checked;
			resources.ApplyResources(this.ckb_SpgWtAutoOnOff, "ckb_SpgWtAutoOnOff");
			this.ckb_SpgWtAutoOnOff.Name = "ckb_SpgWtAutoOnOff";
			this.ckb_SpgWtAutoOnOff.UseVisualStyleBackColor = true;
			// 
			// vwOpt_03Dry
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.pnl_DelayTime);
			this.Controls.Add(this.panel43);
			this.Controls.Add(this.pnl_SpongeOnOff);
			this.DoubleBuffered = true;
			resources.ApplyResources(this, "$this");
			this.ForeColor = System.Drawing.Color.Black;
			this.Name = "vwOpt_03Dry";
			this.pnl_SpongeOnOff.ResumeLayout(false);
			this.pnl_SpongeOnOff.PerformLayout();
			this.panel43.ResumeLayout(false);
			this.pnl_DelayTime.ResumeLayout(false);
			this.pnl_DelayTime.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label313;
        private Extended.ExTextBox txt_SpgWtOnMin;
        private System.Windows.Forms.Label lbl_SpgWtOnMin;
        private System.Windows.Forms.Label label311;
        private Extended.ExTextBox txt_SpgWtOffMin;
        private System.Windows.Forms.Label lbl_SpgWtOffMin;
        private System.Windows.Forms.Panel pnl_SpongeOnOff;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel43;
        private System.Windows.Forms.CheckBox ckb_SpwSkip;
        private System.Windows.Forms.CheckBox ckb_DryStickSkip;
        private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Panel pnl_DelayTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label299;
		private System.Windows.Forms.Label label300;
		private System.Windows.Forms.TextBox txt_ZUpStableDelay;
		private System.Windows.Forms.CheckBox ckb_SpgWtAutoOnOff;
	}
}
