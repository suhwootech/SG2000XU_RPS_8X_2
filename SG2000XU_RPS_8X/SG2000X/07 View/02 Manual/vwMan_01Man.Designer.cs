namespace SG2000X
{
    partial class vwMan_01Man
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_01Man));
            this.ckb_RLamp = new System.Windows.Forms.CheckBox();
            this.pnl_WmSave = new System.Windows.Forms.Panel();
            this.lbl_IdleT_T = new System.Windows.Forms.Label();
            this.txt_IdleTime = new System.Windows.Forms.TextBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_AllWOff = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_WmTitle = new System.Windows.Forms.Label();
            this.lbl_IdleTime = new System.Windows.Forms.Label();
            this.btn_Idle = new System.Windows.Forms.Button();
            this.lbl_GrrHD = new System.Windows.Forms.Label();
            this.lbl_GrlHD = new System.Windows.Forms.Label();
            this.lbl_DryHD = new System.Windows.Forms.Label();
            this.lbl_InrHD = new System.Windows.Forms.Label();
            this.lbl_OfpHD = new System.Windows.Forms.Label();
            this.lbl_OnpHD = new System.Windows.Forms.Label();
            this.lbl_OflHD = new System.Windows.Forms.Label();
            this.lbl_OnlHD = new System.Windows.Forms.Label();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_AllH = new System.Windows.Forms.Button();
            this.btn_SrvOff = new System.Windows.Forms.Button();
            this.btn_SrvOn = new System.Windows.Forms.Button();
            this.pnl_AutoWarm = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_AutoWarm = new System.Windows.Forms.Label();
            this.btn0_DoorUnLock = new System.Windows.Forms.Button();
            this.btn0_DoorLock = new System.Windows.Forms.Button();
            this.ckb_FWater = new System.Windows.Forms.CheckBox();
            this.pnl_WmSave.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnl_AutoWarm.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckb_RLamp
            // 
            resources.ApplyResources(this.ckb_RLamp, "ckb_RLamp");
            this.ckb_RLamp.BackColor = System.Drawing.Color.LightGray;
            this.ckb_RLamp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_RLamp.FlatAppearance.BorderSize = 2;
            this.ckb_RLamp.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_RLamp.ForeColor = System.Drawing.Color.Black;
            this.ckb_RLamp.Name = "ckb_RLamp";
            this.ckb_RLamp.Tag = "0";
            this.ckb_RLamp.UseVisualStyleBackColor = false;
            this.ckb_RLamp.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckb_RLamp.Click += new System.EventHandler(this.ckb_Click);
            // 
            // pnl_WmSave
            // 
            this.pnl_WmSave.BackColor = System.Drawing.Color.OrangeRed;
            this.pnl_WmSave.Controls.Add(this.lbl_IdleT_T);
            this.pnl_WmSave.Controls.Add(this.txt_IdleTime);
            this.pnl_WmSave.Controls.Add(this.btn_Save);
            resources.ApplyResources(this.pnl_WmSave, "pnl_WmSave");
            this.pnl_WmSave.Name = "pnl_WmSave";
            // 
            // lbl_IdleT_T
            // 
            this.lbl_IdleT_T.BackColor = System.Drawing.Color.OrangeRed;
            resources.ApplyResources(this.lbl_IdleT_T, "lbl_IdleT_T");
            this.lbl_IdleT_T.ForeColor = System.Drawing.Color.Black;
            this.lbl_IdleT_T.Name = "lbl_IdleT_T";
            this.lbl_IdleT_T.Tag = "Brown,Coral,OrangeRed";
            // 
            // txt_IdleTime
            // 
            this.txt_IdleTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_IdleTime, "txt_IdleTime");
            this.txt_IdleTime.Name = "txt_IdleTime";
            this.txt_IdleTime.Tag = "0";
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
            // btn_AllWOff
            // 
            this.btn_AllWOff.BackColor = System.Drawing.Color.Indigo;
            this.btn_AllWOff.FlatAppearance.BorderSize = 0;
            this.btn_AllWOff.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btn_AllWOff.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.btn_AllWOff, "btn_AllWOff");
            this.btn_AllWOff.ForeColor = System.Drawing.Color.White;
            this.btn_AllWOff.Name = "btn_AllWOff";
            this.btn_AllWOff.Tag = "Warm_Up";
            this.btn_AllWOff.UseVisualStyleBackColor = false;
            this.btn_AllWOff.Click += new System.EventHandler(this.btn_AllWaterOff_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.OrangeRed;
            this.panel1.Controls.Add(this.lbl_WmTitle);
            this.panel1.Controls.Add(this.lbl_IdleTime);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lbl_WmTitle
            // 
            this.lbl_WmTitle.BackColor = System.Drawing.Color.OrangeRed;
            resources.ApplyResources(this.lbl_WmTitle, "lbl_WmTitle");
            this.lbl_WmTitle.ForeColor = System.Drawing.Color.White;
            this.lbl_WmTitle.Name = "lbl_WmTitle";
            this.lbl_WmTitle.Tag = "Brown,Coral,OrangeRed";
            // 
            // lbl_IdleTime
            // 
            this.lbl_IdleTime.BackColor = System.Drawing.Color.OrangeRed;
            resources.ApplyResources(this.lbl_IdleTime, "lbl_IdleTime");
            this.lbl_IdleTime.ForeColor = System.Drawing.Color.White;
            this.lbl_IdleTime.Name = "lbl_IdleTime";
            this.lbl_IdleTime.Tag = "IDLE_RUNNING";
            // 
            // btn_Idle
            // 
            this.btn_Idle.BackColor = System.Drawing.Color.OrangeRed;
            this.btn_Idle.FlatAppearance.BorderSize = 0;
            this.btn_Idle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btn_Idle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.btn_Idle, "btn_Idle");
            this.btn_Idle.ForeColor = System.Drawing.Color.White;
            this.btn_Idle.Name = "btn_Idle";
            this.btn_Idle.Tag = "Warm_Up";
            this.btn_Idle.UseVisualStyleBackColor = false;
            this.btn_Idle.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // lbl_GrrHD
            // 
            this.lbl_GrrHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_GrrHD, "lbl_GrrHD");
            this.lbl_GrrHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_GrrHD.Name = "lbl_GrrHD";
            // 
            // lbl_GrlHD
            // 
            this.lbl_GrlHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_GrlHD, "lbl_GrlHD");
            this.lbl_GrlHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_GrlHD.Name = "lbl_GrlHD";
            // 
            // lbl_DryHD
            // 
            this.lbl_DryHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_DryHD, "lbl_DryHD");
            this.lbl_DryHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_DryHD.Name = "lbl_DryHD";
            // 
            // lbl_InrHD
            // 
            this.lbl_InrHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_InrHD, "lbl_InrHD");
            this.lbl_InrHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_InrHD.Name = "lbl_InrHD";
            // 
            // lbl_OfpHD
            // 
            this.lbl_OfpHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_OfpHD, "lbl_OfpHD");
            this.lbl_OfpHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_OfpHD.Name = "lbl_OfpHD";
            // 
            // lbl_OnpHD
            // 
            this.lbl_OnpHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_OnpHD, "lbl_OnpHD");
            this.lbl_OnpHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_OnpHD.Name = "lbl_OnpHD";
            // 
            // lbl_OflHD
            // 
            this.lbl_OflHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_OflHD, "lbl_OflHD");
            this.lbl_OflHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_OflHD.Name = "lbl_OflHD";
            // 
            // lbl_OnlHD
            // 
            this.lbl_OnlHD.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_OnlHD, "lbl_OnlHD");
            this.lbl_OnlHD.Image = global::SG2000X.Properties.Resources.home;
            this.lbl_OnlHD.Name = "lbl_OnlHD";
            // 
            // btn_Stop
            // 
            this.btn_Stop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.btn_Stop.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Stop, "btn_Stop");
            this.btn_Stop.ForeColor = System.Drawing.Color.White;
            this.btn_Stop.Image = global::SG2000X.Properties.Resources.Stop64;
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Tag = "";
            this.btn_Stop.UseVisualStyleBackColor = false;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_AllH
            // 
            this.btn_AllH.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_AllH.FlatAppearance.BorderSize = 0;
            this.btn_AllH.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Teal;
            this.btn_AllH.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumTurquoise;
            resources.ApplyResources(this.btn_AllH, "btn_AllH");
            this.btn_AllH.ForeColor = System.Drawing.Color.White;
            this.btn_AllH.Image = global::SG2000X.Properties.Resources.All_Home;
            this.btn_AllH.Name = "btn_AllH";
            this.btn_AllH.Tag = "ALL_Home";
            this.btn_AllH.UseVisualStyleBackColor = false;
            this.btn_AllH.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btn_SrvOff
            // 
            this.btn_SrvOff.BackColor = System.Drawing.Color.Firebrick;
            this.btn_SrvOff.FlatAppearance.BorderSize = 0;
            this.btn_SrvOff.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.btn_SrvOff.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            resources.ApplyResources(this.btn_SrvOff, "btn_SrvOff");
            this.btn_SrvOff.ForeColor = System.Drawing.Color.White;
            this.btn_SrvOff.Image = global::SG2000X.Properties.Resources.Power;
            this.btn_SrvOff.Name = "btn_SrvOff";
            this.btn_SrvOff.Tag = "ALL_Servo_Off";
            this.btn_SrvOff.UseVisualStyleBackColor = false;
            this.btn_SrvOff.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btn_SrvOn
            // 
            this.btn_SrvOn.BackColor = System.Drawing.Color.ForestGreen;
            this.btn_SrvOn.FlatAppearance.BorderSize = 0;
            this.btn_SrvOn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGreen;
            this.btn_SrvOn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LimeGreen;
            resources.ApplyResources(this.btn_SrvOn, "btn_SrvOn");
            this.btn_SrvOn.ForeColor = System.Drawing.Color.White;
            this.btn_SrvOn.Image = global::SG2000X.Properties.Resources.Power;
            this.btn_SrvOn.Name = "btn_SrvOn";
            this.btn_SrvOn.Tag = "ALL_Servo_On";
            this.btn_SrvOn.UseVisualStyleBackColor = false;
            this.btn_SrvOn.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // pnl_AutoWarm
            // 
            this.pnl_AutoWarm.BackColor = System.Drawing.Color.LightSalmon;
            this.pnl_AutoWarm.Controls.Add(this.label1);
            this.pnl_AutoWarm.Controls.Add(this.lbl_AutoWarm);
            resources.ApplyResources(this.pnl_AutoWarm, "pnl_AutoWarm");
            this.pnl_AutoWarm.Name = "pnl_AutoWarm";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightSalmon;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            this.label1.Tag = "Brown,Coral,OrangeRed";
            // 
            // lbl_AutoWarm
            // 
            this.lbl_AutoWarm.BackColor = System.Drawing.Color.LightSalmon;
            resources.ApplyResources(this.lbl_AutoWarm, "lbl_AutoWarm");
            this.lbl_AutoWarm.ForeColor = System.Drawing.Color.White;
            this.lbl_AutoWarm.Name = "lbl_AutoWarm";
            this.lbl_AutoWarm.Tag = "IDLE_RUNNING";
            // 
            // btn0_DoorUnLock
            // 
            this.btn0_DoorUnLock.BackColor = System.Drawing.Color.Brown;
            resources.ApplyResources(this.btn0_DoorUnLock, "btn0_DoorUnLock");
            this.btn0_DoorUnLock.FlatAppearance.BorderSize = 0;
            this.btn0_DoorUnLock.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btn0_DoorUnLock.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            this.btn0_DoorUnLock.ForeColor = System.Drawing.Color.White;
            this.btn0_DoorUnLock.Name = "btn0_DoorUnLock";
            this.btn0_DoorUnLock.Tag = "Warm_Up";
            this.btn0_DoorUnLock.UseVisualStyleBackColor = false;
            this.btn0_DoorUnLock.Click += new System.EventHandler(this.btn0_DoorUnLock_Click);
            // 
            // btn0_DoorLock
            // 
            this.btn0_DoorLock.BackColor = System.Drawing.Color.Brown;
            this.btn0_DoorLock.FlatAppearance.BorderSize = 0;
            this.btn0_DoorLock.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Brown;
            this.btn0_DoorLock.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Coral;
            resources.ApplyResources(this.btn0_DoorLock, "btn0_DoorLock");
            this.btn0_DoorLock.ForeColor = System.Drawing.Color.White;
            this.btn0_DoorLock.Name = "btn0_DoorLock";
            this.btn0_DoorLock.Tag = "Warm_Up";
            this.btn0_DoorLock.UseVisualStyleBackColor = false;
            this.btn0_DoorLock.Click += new System.EventHandler(this.btn0_DoorLock_Click);
            // 
            // ckb_FWater
            // 
            resources.ApplyResources(this.ckb_FWater, "ckb_FWater");
            this.ckb_FWater.BackColor = System.Drawing.Color.LightGray;
            this.ckb_FWater.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_FWater.FlatAppearance.BorderSize = 2;
            this.ckb_FWater.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_FWater.ForeColor = System.Drawing.Color.Black;
            this.ckb_FWater.Name = "ckb_FWater";
            this.ckb_FWater.Tag = "1";
            this.ckb_FWater.UseVisualStyleBackColor = false;
            this.ckb_FWater.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckb_FWater.Click += new System.EventHandler(this.ckb_Click);
            // 
            // vwMan_01Man
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ckb_FWater);
            this.Controls.Add(this.btn0_DoorUnLock);
            this.Controls.Add(this.btn0_DoorLock);
            this.Controls.Add(this.pnl_AutoWarm);
            this.Controls.Add(this.ckb_RLamp);
            this.Controls.Add(this.pnl_WmSave);
            this.Controls.Add(this.lbl_GrrHD);
            this.Controls.Add(this.lbl_GrlHD);
            this.Controls.Add(this.lbl_DryHD);
            this.Controls.Add(this.lbl_InrHD);
            this.Controls.Add(this.lbl_OfpHD);
            this.Controls.Add(this.lbl_OnpHD);
            this.Controls.Add(this.lbl_OflHD);
            this.Controls.Add(this.lbl_OnlHD);
            this.Controls.Add(this.btn_AllWOff);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_AllH);
            this.Controls.Add(this.btn_SrvOff);
            this.Controls.Add(this.btn_Idle);
            this.Controls.Add(this.btn_SrvOn);
            resources.ApplyResources(this, "$this");
            this.Name = "vwMan_01Man";
            this.pnl_WmSave.ResumeLayout(false);
            this.pnl_WmSave.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.pnl_AutoWarm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox ckb_RLamp;
        private System.Windows.Forms.Panel pnl_WmSave;
        private System.Windows.Forms.Label lbl_IdleT_T;
        private System.Windows.Forms.TextBox txt_IdleTime;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Label lbl_GrrHD;
        private System.Windows.Forms.Label lbl_GrlHD;
        private System.Windows.Forms.Label lbl_DryHD;
        private System.Windows.Forms.Label lbl_InrHD;
        private System.Windows.Forms.Label lbl_OfpHD;
        private System.Windows.Forms.Label lbl_OnpHD;
        private System.Windows.Forms.Label lbl_OflHD;
        private System.Windows.Forms.Label lbl_OnlHD;
        private System.Windows.Forms.Button btn_AllWOff;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_WmTitle;
        private System.Windows.Forms.Label lbl_IdleTime;
        private System.Windows.Forms.Button btn_AllH;
        private System.Windows.Forms.Button btn_SrvOff;
        private System.Windows.Forms.Button btn_Idle;
        private System.Windows.Forms.Button btn_SrvOn;
        private System.Windows.Forms.Panel pnl_AutoWarm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_AutoWarm;
        private System.Windows.Forms.Button btn0_DoorUnLock;
        private System.Windows.Forms.Button btn0_DoorLock;
        private System.Windows.Forms.CheckBox ckb_FWater;
    }
}
