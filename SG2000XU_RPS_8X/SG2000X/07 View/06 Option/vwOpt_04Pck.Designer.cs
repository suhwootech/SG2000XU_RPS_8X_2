namespace SG2000X
{
    partial class vwOpt_04Pck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwOpt_04Pck));
            this.pnl_PadCleanTime = new System.Windows.Forms.Panel();
            this.lbl_OfpPadCleanTime_U = new System.Windows.Forms.Label();
            this.txt_OfpPadCleanTime = new System.Windows.Forms.TextBox();
            this.lbl_OfpPadCleanTime_T = new System.Windows.Forms.Label();
            this.label277 = new System.Windows.Forms.Label();
            this.panel45 = new System.Windows.Forms.Panel();
            this.label121 = new System.Windows.Forms.Label();
            this.label152 = new System.Windows.Forms.Label();
            this.label158 = new System.Windows.Forms.Label();
            this.txt_OnpRinseTime_L = new System.Windows.Forms.TextBox();
            this.txt_OnpRinseTime_R = new System.Windows.Forms.TextBox();
            this.label245 = new System.Windows.Forms.Label();
            this.label246 = new System.Windows.Forms.Label();
            this.panel51 = new System.Windows.Forms.Panel();
            this.label299 = new System.Windows.Forms.Label();
            this.label300 = new System.Windows.Forms.Label();
            this.txt_PlaceDelay = new System.Windows.Forms.TextBox();
            this.label301 = new System.Windows.Forms.Label();
            this.label302 = new System.Windows.Forms.Label();
            this.txt_PlaceGap = new System.Windows.Forms.TextBox();
            this.label303 = new System.Windows.Forms.Label();
            this.label304 = new System.Windows.Forms.Label();
            this.txt_PickDelay = new System.Windows.Forms.TextBox();
            this.label305 = new System.Windows.Forms.Label();
            this.label306 = new System.Windows.Forms.Label();
            this.label307 = new System.Windows.Forms.Label();
            this.txt_PickGap = new System.Windows.Forms.TextBox();
            this.panel43 = new System.Windows.Forms.Panel();
            this.ckb_OfpCoverDryerSkip = new System.Windows.Forms.CheckBox();
            this.ckb_OfpUseCenterPos = new System.Windows.Forms.CheckBox();
            this.ckb_OfpBtmCleanSkip = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdb_ClnModeRotation = new System.Windows.Forms.RadioButton();
            this.rdb_ClnModeBasic = new System.Windows.Forms.RadioButton();
            this.pnl_PadCleanTime.SuspendLayout();
            this.panel45.SuspendLayout();
            this.panel51.SuspendLayout();
            this.panel43.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_PadCleanTime
            // 
            this.pnl_PadCleanTime.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_PadCleanTime.Controls.Add(this.groupBox3);
            this.pnl_PadCleanTime.Controls.Add(this.lbl_OfpPadCleanTime_U);
            this.pnl_PadCleanTime.Controls.Add(this.txt_OfpPadCleanTime);
            this.pnl_PadCleanTime.Controls.Add(this.lbl_OfpPadCleanTime_T);
            this.pnl_PadCleanTime.Controls.Add(this.label277);
            resources.ApplyResources(this.pnl_PadCleanTime, "pnl_PadCleanTime");
            this.pnl_PadCleanTime.Name = "pnl_PadCleanTime";
            this.pnl_PadCleanTime.Tag = "0";
            // 
            // lbl_OfpPadCleanTime_U
            // 
            resources.ApplyResources(this.lbl_OfpPadCleanTime_U, "lbl_OfpPadCleanTime_U");
            this.lbl_OfpPadCleanTime_U.ForeColor = System.Drawing.Color.Black;
            this.lbl_OfpPadCleanTime_U.Name = "lbl_OfpPadCleanTime_U";
            // 
            // txt_OfpPadCleanTime
            // 
            this.txt_OfpPadCleanTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_OfpPadCleanTime, "txt_OfpPadCleanTime");
            this.txt_OfpPadCleanTime.Name = "txt_OfpPadCleanTime";
            this.txt_OfpPadCleanTime.Tag = "0";
            // 
            // lbl_OfpPadCleanTime_T
            // 
            resources.ApplyResources(this.lbl_OfpPadCleanTime_T, "lbl_OfpPadCleanTime_T");
            this.lbl_OfpPadCleanTime_T.ForeColor = System.Drawing.Color.Black;
            this.lbl_OfpPadCleanTime_T.Name = "lbl_OfpPadCleanTime_T";
            // 
            // label277
            // 
            this.label277.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label277, "label277");
            this.label277.ForeColor = System.Drawing.Color.White;
            this.label277.Name = "label277";
            // 
            // panel45
            // 
            this.panel45.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel45.Controls.Add(this.label121);
            this.panel45.Controls.Add(this.label152);
            this.panel45.Controls.Add(this.label158);
            this.panel45.Controls.Add(this.txt_OnpRinseTime_L);
            this.panel45.Controls.Add(this.txt_OnpRinseTime_R);
            this.panel45.Controls.Add(this.label245);
            this.panel45.Controls.Add(this.label246);
            resources.ApplyResources(this.panel45, "panel45");
            this.panel45.Name = "panel45";
            this.panel45.Tag = "0";
            // 
            // label121
            // 
            resources.ApplyResources(this.label121, "label121");
            this.label121.ForeColor = System.Drawing.Color.Black;
            this.label121.Name = "label121";
            // 
            // label152
            // 
            resources.ApplyResources(this.label152, "label152");
            this.label152.ForeColor = System.Drawing.Color.Black;
            this.label152.Name = "label152";
            // 
            // label158
            // 
            resources.ApplyResources(this.label158, "label158");
            this.label158.ForeColor = System.Drawing.Color.Black;
            this.label158.Name = "label158";
            // 
            // txt_OnpRinseTime_L
            // 
            this.txt_OnpRinseTime_L.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_OnpRinseTime_L, "txt_OnpRinseTime_L");
            this.txt_OnpRinseTime_L.Name = "txt_OnpRinseTime_L";
            this.txt_OnpRinseTime_L.Tag = "0";
            // 
            // txt_OnpRinseTime_R
            // 
            this.txt_OnpRinseTime_R.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_OnpRinseTime_R, "txt_OnpRinseTime_R");
            this.txt_OnpRinseTime_R.Name = "txt_OnpRinseTime_R";
            this.txt_OnpRinseTime_R.Tag = "0";
            // 
            // label245
            // 
            resources.ApplyResources(this.label245, "label245");
            this.label245.ForeColor = System.Drawing.Color.Black;
            this.label245.Name = "label245";
            // 
            // label246
            // 
            this.label246.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label246, "label246");
            this.label246.ForeColor = System.Drawing.Color.White;
            this.label246.Name = "label246";
            // 
            // panel51
            // 
            this.panel51.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel51.Controls.Add(this.label299);
            this.panel51.Controls.Add(this.label300);
            this.panel51.Controls.Add(this.txt_PlaceDelay);
            this.panel51.Controls.Add(this.label301);
            this.panel51.Controls.Add(this.label302);
            this.panel51.Controls.Add(this.txt_PlaceGap);
            this.panel51.Controls.Add(this.label303);
            this.panel51.Controls.Add(this.label304);
            this.panel51.Controls.Add(this.txt_PickDelay);
            this.panel51.Controls.Add(this.label305);
            this.panel51.Controls.Add(this.label306);
            this.panel51.Controls.Add(this.label307);
            this.panel51.Controls.Add(this.txt_PickGap);
            resources.ApplyResources(this.panel51, "panel51");
            this.panel51.Name = "panel51";
            this.panel51.Tag = "0";
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
            // txt_PlaceDelay
            // 
            this.txt_PlaceDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PlaceDelay, "txt_PlaceDelay");
            this.txt_PlaceDelay.Name = "txt_PlaceDelay";
            this.txt_PlaceDelay.Tag = "0";
            // 
            // label301
            // 
            resources.ApplyResources(this.label301, "label301");
            this.label301.ForeColor = System.Drawing.Color.Black;
            this.label301.Name = "label301";
            // 
            // label302
            // 
            resources.ApplyResources(this.label302, "label302");
            this.label302.ForeColor = System.Drawing.Color.Black;
            this.label302.Name = "label302";
            // 
            // txt_PlaceGap
            // 
            this.txt_PlaceGap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PlaceGap, "txt_PlaceGap");
            this.txt_PlaceGap.Name = "txt_PlaceGap";
            this.txt_PlaceGap.Tag = "0";
            // 
            // label303
            // 
            resources.ApplyResources(this.label303, "label303");
            this.label303.ForeColor = System.Drawing.Color.Black;
            this.label303.Name = "label303";
            // 
            // label304
            // 
            resources.ApplyResources(this.label304, "label304");
            this.label304.ForeColor = System.Drawing.Color.Black;
            this.label304.Name = "label304";
            // 
            // txt_PickDelay
            // 
            this.txt_PickDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PickDelay, "txt_PickDelay");
            this.txt_PickDelay.Name = "txt_PickDelay";
            this.txt_PickDelay.Tag = "0";
            // 
            // label305
            // 
            this.label305.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label305, "label305");
            this.label305.ForeColor = System.Drawing.Color.White;
            this.label305.Name = "label305";
            // 
            // label306
            // 
            resources.ApplyResources(this.label306, "label306");
            this.label306.ForeColor = System.Drawing.Color.Black;
            this.label306.Name = "label306";
            // 
            // label307
            // 
            resources.ApplyResources(this.label307, "label307");
            this.label307.ForeColor = System.Drawing.Color.Black;
            this.label307.Name = "label307";
            // 
            // txt_PickGap
            // 
            this.txt_PickGap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PickGap, "txt_PickGap");
            this.txt_PickGap.Name = "txt_PickGap";
            this.txt_PickGap.Tag = "0";
            // 
            // panel43
            // 
            this.panel43.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel43.Controls.Add(this.ckb_OfpCoverDryerSkip);
            this.panel43.Controls.Add(this.ckb_OfpUseCenterPos);
            this.panel43.Controls.Add(this.ckb_OfpBtmCleanSkip);
            this.panel43.Controls.Add(this.label15);
            resources.ApplyResources(this.panel43, "panel43");
            this.panel43.Name = "panel43";
            this.panel43.Tag = "0";
            // 
            // ckb_OfpCoverDryerSkip
            // 
            resources.ApplyResources(this.ckb_OfpCoverDryerSkip, "ckb_OfpCoverDryerSkip");
            this.ckb_OfpCoverDryerSkip.Name = "ckb_OfpCoverDryerSkip";
            this.ckb_OfpCoverDryerSkip.UseVisualStyleBackColor = true;
            // 
            // ckb_OfpUseCenterPos
            // 
            resources.ApplyResources(this.ckb_OfpUseCenterPos, "ckb_OfpUseCenterPos");
            this.ckb_OfpUseCenterPos.Name = "ckb_OfpUseCenterPos";
            this.ckb_OfpUseCenterPos.UseVisualStyleBackColor = true;
            // 
            // ckb_OfpBtmCleanSkip
            // 
            resources.ApplyResources(this.ckb_OfpBtmCleanSkip, "ckb_OfpBtmCleanSkip");
            this.ckb_OfpBtmCleanSkip.Name = "ckb_OfpBtmCleanSkip";
            this.ckb_OfpBtmCleanSkip.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label15, "label15");
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Name = "label15";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdb_ClnModeRotation);
            this.groupBox3.Controls.Add(this.rdb_ClnModeBasic);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // rdb_ClnModeRotation
            // 
            resources.ApplyResources(this.rdb_ClnModeRotation, "rdb_ClnModeRotation");
            this.rdb_ClnModeRotation.Name = "rdb_ClnModeRotation";
            this.rdb_ClnModeRotation.UseVisualStyleBackColor = true;
            // 
            // rdb_ClnModeBasic
            // 
            this.rdb_ClnModeBasic.Checked = true;
            resources.ApplyResources(this.rdb_ClnModeBasic, "rdb_ClnModeBasic");
            this.rdb_ClnModeBasic.Name = "rdb_ClnModeBasic";
            this.rdb_ClnModeBasic.TabStop = true;
            this.rdb_ClnModeBasic.UseVisualStyleBackColor = true;
            // 
            // vwOpt_04Pck
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel43);
            this.Controls.Add(this.panel51);
            this.Controls.Add(this.pnl_PadCleanTime);
            this.Controls.Add(this.panel45);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwOpt_04Pck";
            this.pnl_PadCleanTime.ResumeLayout(false);
            this.pnl_PadCleanTime.PerformLayout();
            this.panel45.ResumeLayout(false);
            this.panel45.PerformLayout();
            this.panel51.ResumeLayout(false);
            this.panel51.PerformLayout();
            this.panel43.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_PadCleanTime;
        private System.Windows.Forms.Label lbl_OfpPadCleanTime_U;
        private System.Windows.Forms.TextBox txt_OfpPadCleanTime;
        private System.Windows.Forms.Label lbl_OfpPadCleanTime_T;
        private System.Windows.Forms.Label label277;
        private System.Windows.Forms.Panel panel45;
        private System.Windows.Forms.Label label121;
        private System.Windows.Forms.Label label152;
        private System.Windows.Forms.Label label158;
        private System.Windows.Forms.TextBox txt_OnpRinseTime_L;
        private System.Windows.Forms.TextBox txt_OnpRinseTime_R;
        private System.Windows.Forms.Label label245;
        private System.Windows.Forms.Label label246;
        private System.Windows.Forms.Panel panel51;
        private System.Windows.Forms.Label label299;
        private System.Windows.Forms.Label label300;
        private System.Windows.Forms.TextBox txt_PlaceDelay;
        private System.Windows.Forms.Label label301;
        private System.Windows.Forms.Label label302;
        private System.Windows.Forms.TextBox txt_PlaceGap;
        private System.Windows.Forms.Label label303;
        private System.Windows.Forms.Label label304;
        private System.Windows.Forms.TextBox txt_PickDelay;
        private System.Windows.Forms.Label label305;
        private System.Windows.Forms.Label label306;
        private System.Windows.Forms.Label label307;
        private System.Windows.Forms.TextBox txt_PickGap;
        private System.Windows.Forms.Panel panel43;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox ckb_OfpBtmCleanSkip;
        private System.Windows.Forms.CheckBox ckb_OfpUseCenterPos;
        private System.Windows.Forms.CheckBox ckb_OfpCoverDryerSkip;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdb_ClnModeRotation;
        private System.Windows.Forms.RadioButton rdb_ClnModeBasic;
    }
}
