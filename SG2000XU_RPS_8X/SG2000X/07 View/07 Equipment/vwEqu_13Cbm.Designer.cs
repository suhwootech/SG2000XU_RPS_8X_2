namespace SG2000X
{
    partial class vwEqu_13Cbm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_13Cbm));
            this.btn_Save = new System.Windows.Forms.Button();
            this.tab_AutoView = new System.Windows.Forms.TabControl();
            this.tabPage_Cyl = new System.Windows.Forms.TabPage();
            this.pnlP_SrL = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textDelay_ms = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textRepeatNum = new System.Windows.Forms.TextBox();
            this.btnLogClear = new System.Windows.Forms.Button();
            this.btnAutoRunStop = new System.Windows.Forms.Button();
            this.btnAutoRunStart = new System.Windows.Forms.Button();
            this.butManual_SolOff = new System.Windows.Forms.Button();
            this.btnManual_SolOn = new System.Windows.Forms.Button();
            this.richCyldMeas = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbOut_02 = new System.Windows.Forms.Label();
            this.lbOut_01 = new System.Windows.Forms.Label();
            this.lbIn_02 = new System.Windows.Forms.Label();
            this.lbIn_01 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label205 = new System.Windows.Forms.Label();
            this.lblP_SrLTitle = new System.Windows.Forms.Label();
            this.listboxCylinder = new System.Windows.Forms.ListBox();
            this.label47 = new System.Windows.Forms.Label();
            this.tab_AutoView.SuspendLayout();
            this.tabPage_Cyl.SuspendLayout();
            this.pnlP_SrL.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // tab_AutoView
            // 
            this.tab_AutoView.Controls.Add(this.tabPage_Cyl);
            resources.ApplyResources(this.tab_AutoView, "tab_AutoView");
            this.tab_AutoView.Name = "tab_AutoView";
            this.tab_AutoView.SelectedIndex = 0;
            // 
            // tabPage_Cyl
            // 
            this.tabPage_Cyl.Controls.Add(this.pnlP_SrL);
            this.tabPage_Cyl.Controls.Add(this.listboxCylinder);
            this.tabPage_Cyl.Controls.Add(this.label47);
            resources.ApplyResources(this.tabPage_Cyl, "tabPage_Cyl");
            this.tabPage_Cyl.Name = "tabPage_Cyl";
            this.tabPage_Cyl.UseVisualStyleBackColor = true;
            // 
            // pnlP_SrL
            // 
            this.pnlP_SrL.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlP_SrL.Controls.Add(this.label3);
            this.pnlP_SrL.Controls.Add(this.textDelay_ms);
            this.pnlP_SrL.Controls.Add(this.label2);
            this.pnlP_SrL.Controls.Add(this.textRepeatNum);
            this.pnlP_SrL.Controls.Add(this.btnLogClear);
            this.pnlP_SrL.Controls.Add(this.btnAutoRunStop);
            this.pnlP_SrL.Controls.Add(this.btnAutoRunStart);
            this.pnlP_SrL.Controls.Add(this.butManual_SolOff);
            this.pnlP_SrL.Controls.Add(this.btnManual_SolOn);
            this.pnlP_SrL.Controls.Add(this.richCyldMeas);
            this.pnlP_SrL.Controls.Add(this.btn_Save);
            this.pnlP_SrL.Controls.Add(this.label1);
            this.pnlP_SrL.Controls.Add(this.lbOut_02);
            this.pnlP_SrL.Controls.Add(this.lbOut_01);
            this.pnlP_SrL.Controls.Add(this.lbIn_02);
            this.pnlP_SrL.Controls.Add(this.lbIn_01);
            this.pnlP_SrL.Controls.Add(this.label6);
            this.pnlP_SrL.Controls.Add(this.label205);
            this.pnlP_SrL.Controls.Add(this.lblP_SrLTitle);
            resources.ApplyResources(this.pnlP_SrL, "pnlP_SrL");
            this.pnlP_SrL.Name = "pnlP_SrL";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textDelay_ms
            // 
            resources.ApplyResources(this.textDelay_ms, "textDelay_ms");
            this.textDelay_ms.Name = "textDelay_ms";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textRepeatNum
            // 
            resources.ApplyResources(this.textRepeatNum, "textRepeatNum");
            this.textRepeatNum.Name = "textRepeatNum";
            // 
            // btnLogClear
            // 
            resources.ApplyResources(this.btnLogClear, "btnLogClear");
            this.btnLogClear.Name = "btnLogClear";
            this.btnLogClear.UseVisualStyleBackColor = true;
            this.btnLogClear.Click += new System.EventHandler(this.btnLogClear_Click);
            // 
            // btnAutoRunStop
            // 
            resources.ApplyResources(this.btnAutoRunStop, "btnAutoRunStop");
            this.btnAutoRunStop.Name = "btnAutoRunStop";
            this.btnAutoRunStop.UseVisualStyleBackColor = true;
            this.btnAutoRunStop.Click += new System.EventHandler(this.btnAutoRunStop_Click);
            // 
            // btnAutoRunStart
            // 
            resources.ApplyResources(this.btnAutoRunStart, "btnAutoRunStart");
            this.btnAutoRunStart.Name = "btnAutoRunStart";
            this.btnAutoRunStart.UseVisualStyleBackColor = true;
            this.btnAutoRunStart.Click += new System.EventHandler(this.btnAutoRunStart_Click);
            // 
            // butManual_SolOff
            // 
            resources.ApplyResources(this.butManual_SolOff, "butManual_SolOff");
            this.butManual_SolOff.Name = "butManual_SolOff";
            this.butManual_SolOff.UseVisualStyleBackColor = true;
            this.butManual_SolOff.Click += new System.EventHandler(this.btnManual_SolOff_Click);
            // 
            // btnManual_SolOn
            // 
            resources.ApplyResources(this.btnManual_SolOn, "btnManual_SolOn");
            this.btnManual_SolOn.Name = "btnManual_SolOn";
            this.btnManual_SolOn.UseVisualStyleBackColor = true;
            this.btnManual_SolOn.Click += new System.EventHandler(this.btnManual_SolOn_Click);
            // 
            // richCyldMeas
            // 
            this.richCyldMeas.BackColor = System.Drawing.Color.LightGray;
            this.richCyldMeas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.richCyldMeas, "richCyldMeas");
            this.richCyldMeas.Name = "richCyldMeas";
            this.richCyldMeas.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lbOut_02
            // 
            this.lbOut_02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbOut_02, "lbOut_02");
            this.lbOut_02.Name = "lbOut_02";
            // 
            // lbOut_01
            // 
            this.lbOut_01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbOut_01, "lbOut_01");
            this.lbOut_01.Name = "lbOut_01";
            // 
            // lbIn_02
            // 
            this.lbIn_02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbIn_02, "lbIn_02");
            this.lbIn_02.Name = "lbIn_02";
            // 
            // lbIn_01
            // 
            this.lbIn_01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbIn_01, "lbIn_01");
            this.lbIn_01.Name = "lbIn_01";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label205
            // 
            resources.ApplyResources(this.label205, "label205");
            this.label205.Name = "label205";
            // 
            // lblP_SrLTitle
            // 
            this.lblP_SrLTitle.BackColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.lblP_SrLTitle, "lblP_SrLTitle");
            this.lblP_SrLTitle.ForeColor = System.Drawing.Color.White;
            this.lblP_SrLTitle.Name = "lblP_SrLTitle";
            // 
            // listboxCylinder
            // 
            resources.ApplyResources(this.listboxCylinder, "listboxCylinder");
            this.listboxCylinder.Name = "listboxCylinder";
            this.listboxCylinder.TabStop = false;
            this.listboxCylinder.SelectedIndexChanged += new System.EventHandler(this.lbxCylList_SelectedIndexChanged);
            // 
            // label47
            // 
            this.label47.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label47, "label47");
            this.label47.ForeColor = System.Drawing.Color.White;
            this.label47.Name = "label47";
            // 
            // vwEqu_13Cbm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tab_AutoView);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_13Cbm";
            this.tab_AutoView.ResumeLayout(false);
            this.tabPage_Cyl.ResumeLayout(false);
            this.pnlP_SrL.ResumeLayout(false);
            this.pnlP_SrL.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TabControl tab_AutoView;
        private System.Windows.Forms.TabPage tabPage_Cyl;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.ListBox listboxCylinder;
        private System.Windows.Forms.Panel pnlP_SrL;
        private System.Windows.Forms.Label label205;
        private System.Windows.Forms.Label lblP_SrLTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbOut_02;
        private System.Windows.Forms.Label lbOut_01;
        private System.Windows.Forms.Label lbIn_02;
        private System.Windows.Forms.Label lbIn_01;
        private System.Windows.Forms.RichTextBox richCyldMeas;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnManual_SolOn;
        private System.Windows.Forms.TextBox textRepeatNum;
        private System.Windows.Forms.Button btnAutoRunStop;
        private System.Windows.Forms.Button btnAutoRunStart;
        private System.Windows.Forms.Button btnLogClear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textDelay_ms;
        private System.Windows.Forms.Button butManual_SolOff;
    }
}
