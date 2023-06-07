namespace SG2000X
{
    partial class vwMan_03Inr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_03Inr));
            this.btnInr_Ori = new System.Windows.Forms.Button();
            this.btnInr_Bcr = new System.Windows.Forms.Button();
            this.ckbInr_IOOn = new System.Windows.Forms.CheckBox();
            this.pl_Dynamic = new System.Windows.Forms.Panel();
            this.lblM_PcbTtv = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.lblM_PcbMax = new System.Windows.Forms.Label();
            this.lblM_Pcb1 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.lblM_PcbMean = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.lblM_Pcb2 = new System.Windows.Forms.Label();
            this.lblM_Pcb3 = new System.Windows.Forms.Label();
            this.pnlInr_Bcr = new System.Windows.Forms.Panel();
            this.label68 = new System.Windows.Forms.Label();
            this.lbl_BcrTitle = new System.Windows.Forms.Label();
            this.lblInr_Bcr = new System.Windows.Forms.Label();
            this.ckb_PrbAir = new System.Windows.Forms.CheckBox();
            this.ckb_PrbDn = new System.Windows.Forms.CheckBox();
            this.btnInr_Dynamic = new System.Windows.Forms.Button();
            this.panel11 = new System.Windows.Forms.Panel();
            this.lblInr_YPos = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.lblInr_YStat = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.lblInr_XPos = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.lblInr_XStat = new System.Windows.Forms.Label();
            this.ckbInr_LiftUp = new System.Windows.Forms.CheckBox();
            this.ckbInr_GripClpOn = new System.Windows.Forms.CheckBox();
            this.lblInr_LiftU = new System.Windows.Forms.Label();
            this.lbl_GripClp = new System.Windows.Forms.Label();
            this.lblInr_GripD = new System.Windows.Forms.Label();
            this.lblInr_Algn = new System.Windows.Forms.Label();
            this.lbl_StripBtm = new System.Windows.Forms.Label();
            this.lblInr_StripIn = new System.Windows.Forms.Label();
            this.btnInr_Algn = new System.Windows.Forms.Button();
            this.ckbInr_AlgnF = new System.Windows.Forms.CheckBox();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn2_Home = new System.Windows.Forms.Button();
            this.btn2_Wait = new System.Windows.Forms.Button();
            this.lbl_Unit4 = new System.Windows.Forms.Label();
            this.lbl_Unit3 = new System.Windows.Forms.Label();
            this.lbl_Unit2 = new System.Windows.Forms.Label();
            this.lbl_Unit1 = new System.Windows.Forms.Label();
            this.lbl_GripClpR = new System.Windows.Forms.Label();
            this.lbl_UTilt = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblM_Pcb4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblM_Pcb5 = new System.Windows.Forms.Label();
            this.pl_Dynamic.SuspendLayout();
            this.pnlInr_Bcr.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel12.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInr_Ori
            // 
            this.btnInr_Ori.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnInr_Ori.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnInr_Ori, "btnInr_Ori");
            this.btnInr_Ori.ForeColor = System.Drawing.Color.White;
            this.btnInr_Ori.Name = "btnInr_Ori";
            this.btnInr_Ori.Tag = "INR_CheckOri";
            this.btnInr_Ori.UseVisualStyleBackColor = false;
            this.btnInr_Ori.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btnInr_Bcr
            // 
            this.btnInr_Bcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnInr_Bcr.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnInr_Bcr, "btnInr_Bcr");
            this.btnInr_Bcr.ForeColor = System.Drawing.Color.White;
            this.btnInr_Bcr.Name = "btnInr_Bcr";
            this.btnInr_Bcr.Tag = "INR_CheckBcr";
            this.btnInr_Bcr.UseVisualStyleBackColor = false;
            this.btnInr_Bcr.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // ckbInr_IOOn
            // 
            resources.ApplyResources(this.ckbInr_IOOn, "ckbInr_IOOn");
            this.ckbInr_IOOn.BackColor = System.Drawing.Color.LightGray;
            this.ckbInr_IOOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbInr_IOOn.FlatAppearance.BorderSize = 2;
            this.ckbInr_IOOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbInr_IOOn.ForeColor = System.Drawing.Color.Black;
            this.ckbInr_IOOn.Name = "ckbInr_IOOn";
            this.ckbInr_IOOn.Tag = "5";
            this.ckbInr_IOOn.UseVisualStyleBackColor = false;
            this.ckbInr_IOOn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckbInr_IOOn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // pl_Dynamic
            // 
            this.pl_Dynamic.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pl_Dynamic.Controls.Add(this.label3);
            this.pl_Dynamic.Controls.Add(this.lblM_Pcb5);
            this.pl_Dynamic.Controls.Add(this.label2);
            this.pl_Dynamic.Controls.Add(this.lblM_Pcb4);
            this.pl_Dynamic.Controls.Add(this.lblM_PcbTtv);
            this.pl_Dynamic.Controls.Add(this.label69);
            this.pl_Dynamic.Controls.Add(this.label1);
            this.pl_Dynamic.Controls.Add(this.label45);
            this.pl_Dynamic.Controls.Add(this.lblM_PcbMax);
            this.pl_Dynamic.Controls.Add(this.lblM_Pcb1);
            this.pl_Dynamic.Controls.Add(this.label33);
            this.pl_Dynamic.Controls.Add(this.label42);
            this.pl_Dynamic.Controls.Add(this.lblM_PcbMean);
            this.pl_Dynamic.Controls.Add(this.label41);
            this.pl_Dynamic.Controls.Add(this.label37);
            this.pl_Dynamic.Controls.Add(this.lblM_Pcb2);
            this.pl_Dynamic.Controls.Add(this.lblM_Pcb3);
            resources.ApplyResources(this.pl_Dynamic, "pl_Dynamic");
            this.pl_Dynamic.Name = "pl_Dynamic";
            this.pl_Dynamic.Tag = "1";
            // 
            // lblM_PcbTtv
            // 
            this.lblM_PcbTtv.BackColor = System.Drawing.Color.LightGray;
            this.lblM_PcbTtv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_PcbTtv, "lblM_PcbTtv");
            this.lblM_PcbTtv.Name = "lblM_PcbTtv";
            // 
            // label69
            // 
            this.label69.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label69, "label69");
            this.label69.ForeColor = System.Drawing.Color.White;
            this.label69.Name = "label69";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.Name = "label45";
            // 
            // lblM_PcbMax
            // 
            this.lblM_PcbMax.BackColor = System.Drawing.Color.LightGray;
            this.lblM_PcbMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_PcbMax, "lblM_PcbMax");
            this.lblM_PcbMax.Name = "lblM_PcbMax";
            // 
            // lblM_Pcb1
            // 
            this.lblM_Pcb1.BackColor = System.Drawing.Color.LightGray;
            this.lblM_Pcb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_Pcb1, "lblM_Pcb1");
            this.lblM_Pcb1.Name = "lblM_Pcb1";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // lblM_PcbMean
            // 
            this.lblM_PcbMean.BackColor = System.Drawing.Color.LightGray;
            this.lblM_PcbMean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_PcbMean, "lblM_PcbMean");
            this.lblM_PcbMean.Name = "lblM_PcbMean";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // lblM_Pcb2
            // 
            this.lblM_Pcb2.BackColor = System.Drawing.Color.LightGray;
            this.lblM_Pcb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_Pcb2, "lblM_Pcb2");
            this.lblM_Pcb2.Name = "lblM_Pcb2";
            // 
            // lblM_Pcb3
            // 
            this.lblM_Pcb3.BackColor = System.Drawing.Color.LightGray;
            this.lblM_Pcb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_Pcb3, "lblM_Pcb3");
            this.lblM_Pcb3.Name = "lblM_Pcb3";
            // 
            // pnlInr_Bcr
            // 
            this.pnlInr_Bcr.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlInr_Bcr.Controls.Add(this.label68);
            this.pnlInr_Bcr.Controls.Add(this.lbl_BcrTitle);
            this.pnlInr_Bcr.Controls.Add(this.lblInr_Bcr);
            resources.ApplyResources(this.pnlInr_Bcr, "pnlInr_Bcr");
            this.pnlInr_Bcr.Name = "pnlInr_Bcr";
            this.pnlInr_Bcr.Tag = "1";
            // 
            // label68
            // 
            this.label68.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label68, "label68");
            this.label68.ForeColor = System.Drawing.Color.White;
            this.label68.Name = "label68";
            // 
            // lbl_BcrTitle
            // 
            resources.ApplyResources(this.lbl_BcrTitle, "lbl_BcrTitle");
            this.lbl_BcrTitle.Name = "lbl_BcrTitle";
            // 
            // lblInr_Bcr
            // 
            this.lblInr_Bcr.BackColor = System.Drawing.Color.LightGray;
            this.lblInr_Bcr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblInr_Bcr, "lblInr_Bcr");
            this.lblInr_Bcr.Name = "lblInr_Bcr";
            // 
            // ckb_PrbAir
            // 
            resources.ApplyResources(this.ckb_PrbAir, "ckb_PrbAir");
            this.ckb_PrbAir.BackColor = System.Drawing.Color.LightGray;
            this.ckb_PrbAir.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_PrbAir.FlatAppearance.BorderSize = 2;
            this.ckb_PrbAir.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_PrbAir.ForeColor = System.Drawing.Color.Black;
            this.ckb_PrbAir.Name = "ckb_PrbAir";
            this.ckb_PrbAir.Tag = "4";
            this.ckb_PrbAir.UseVisualStyleBackColor = false;
            this.ckb_PrbAir.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckb_PrbAir.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckb_PrbDn
            // 
            resources.ApplyResources(this.ckb_PrbDn, "ckb_PrbDn");
            this.ckb_PrbDn.BackColor = System.Drawing.Color.LightGray;
            this.ckb_PrbDn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_PrbDn.FlatAppearance.BorderSize = 2;
            this.ckb_PrbDn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_PrbDn.ForeColor = System.Drawing.Color.Black;
            this.ckb_PrbDn.Name = "ckb_PrbDn";
            this.ckb_PrbDn.Tag = "3";
            this.ckb_PrbDn.UseVisualStyleBackColor = false;
            this.ckb_PrbDn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckb_PrbDn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // btnInr_Dynamic
            // 
            this.btnInr_Dynamic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnInr_Dynamic.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnInr_Dynamic, "btnInr_Dynamic");
            this.btnInr_Dynamic.ForeColor = System.Drawing.Color.White;
            this.btnInr_Dynamic.Name = "btnInr_Dynamic";
            this.btnInr_Dynamic.Tag = "INR_CheckDynamic";
            this.btnInr_Dynamic.UseVisualStyleBackColor = false;
            this.btnInr_Dynamic.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.LightGray;
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Controls.Add(this.lblInr_YPos);
            this.panel11.Controls.Add(this.label29);
            this.panel11.Controls.Add(this.lblInr_YStat);
            resources.ApplyResources(this.panel11, "panel11");
            this.panel11.Name = "panel11";
            this.panel11.Tag = "0";
            // 
            // lblInr_YPos
            // 
            this.lblInr_YPos.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblInr_YPos, "lblInr_YPos");
            this.lblInr_YPos.Name = "lblInr_YPos";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // lblInr_YStat
            // 
            this.lblInr_YStat.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lblInr_YStat, "lblInr_YStat");
            this.lblInr_YStat.Name = "lblInr_YStat";
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.LightGray;
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.lblInr_XPos);
            this.panel12.Controls.Add(this.label34);
            this.panel12.Controls.Add(this.lblInr_XStat);
            resources.ApplyResources(this.panel12, "panel12");
            this.panel12.Name = "panel12";
            this.panel12.Tag = "0";
            // 
            // lblInr_XPos
            // 
            this.lblInr_XPos.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblInr_XPos, "lblInr_XPos");
            this.lblInr_XPos.Name = "lblInr_XPos";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // lblInr_XStat
            // 
            this.lblInr_XStat.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lblInr_XStat, "lblInr_XStat");
            this.lblInr_XStat.Name = "lblInr_XStat";
            // 
            // ckbInr_LiftUp
            // 
            resources.ApplyResources(this.ckbInr_LiftUp, "ckbInr_LiftUp");
            this.ckbInr_LiftUp.BackColor = System.Drawing.Color.LightGray;
            this.ckbInr_LiftUp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbInr_LiftUp.FlatAppearance.BorderSize = 2;
            this.ckbInr_LiftUp.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbInr_LiftUp.ForeColor = System.Drawing.Color.Black;
            this.ckbInr_LiftUp.Name = "ckbInr_LiftUp";
            this.ckbInr_LiftUp.Tag = "2";
            this.ckbInr_LiftUp.UseVisualStyleBackColor = false;
            this.ckbInr_LiftUp.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckbInr_LiftUp.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckbInr_GripClpOn
            // 
            resources.ApplyResources(this.ckbInr_GripClpOn, "ckbInr_GripClpOn");
            this.ckbInr_GripClpOn.BackColor = System.Drawing.Color.LightGray;
            this.ckbInr_GripClpOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbInr_GripClpOn.FlatAppearance.BorderSize = 2;
            this.ckbInr_GripClpOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbInr_GripClpOn.ForeColor = System.Drawing.Color.Black;
            this.ckbInr_GripClpOn.Name = "ckbInr_GripClpOn";
            this.ckbInr_GripClpOn.Tag = "1";
            this.ckbInr_GripClpOn.UseVisualStyleBackColor = false;
            this.ckbInr_GripClpOn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckbInr_GripClpOn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // lblInr_LiftU
            // 
            this.lblInr_LiftU.BackColor = System.Drawing.Color.LightGray;
            this.lblInr_LiftU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblInr_LiftU, "lblInr_LiftU");
            this.lblInr_LiftU.Name = "lblInr_LiftU";
            this.lblInr_LiftU.Tag = "Lift$Down/Up";
            // 
            // lbl_GripClp
            // 
            this.lbl_GripClp.BackColor = System.Drawing.Color.LightGray;
            this.lbl_GripClp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_GripClp, "lbl_GripClp");
            this.lbl_GripClp.Name = "lbl_GripClp";
            this.lbl_GripClp.Tag = "Gripper$Unclamp/Clamp";
            // 
            // lblInr_GripD
            // 
            this.lblInr_GripD.BackColor = System.Drawing.Color.LightGray;
            this.lblInr_GripD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblInr_GripD, "lblInr_GripD");
            this.lblInr_GripD.Name = "lblInr_GripD";
            this.lblInr_GripD.Tag = "Gripper Strip$Detect";
            this.lblInr_GripD.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblInr_Algn
            // 
            this.lblInr_Algn.BackColor = System.Drawing.Color.LightGray;
            this.lblInr_Algn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblInr_Algn, "lblInr_Algn");
            this.lblInr_Algn.Name = "lblInr_Algn";
            this.lblInr_Algn.Tag = "Align Guide$Backward/Forward";
            // 
            // lbl_StripBtm
            // 
            this.lbl_StripBtm.BackColor = System.Drawing.Color.LightGray;
            this.lbl_StripBtm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_StripBtm, "lbl_StripBtm");
            this.lbl_StripBtm.Name = "lbl_StripBtm";
            this.lbl_StripBtm.Tag = "Strip Bottom$Detect";
            this.lbl_StripBtm.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblInr_StripIn
            // 
            this.lblInr_StripIn.BackColor = System.Drawing.Color.LightGray;
            this.lblInr_StripIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblInr_StripIn, "lblInr_StripIn");
            this.lblInr_StripIn.Name = "lblInr_StripIn";
            this.lblInr_StripIn.Tag = "Strip In$Detect";
            this.lblInr_StripIn.BackColorChanged += new System.EventHandler(this.lblIO_B_BackColorChanged);
            // 
            // btnInr_Algn
            // 
            this.btnInr_Algn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnInr_Algn.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnInr_Algn, "btnInr_Algn");
            this.btnInr_Algn.ForeColor = System.Drawing.Color.White;
            this.btnInr_Algn.Name = "btnInr_Algn";
            this.btnInr_Algn.Tag = "INR_Align";
            this.btnInr_Algn.UseVisualStyleBackColor = false;
            this.btnInr_Algn.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // ckbInr_AlgnF
            // 
            resources.ApplyResources(this.ckbInr_AlgnF, "ckbInr_AlgnF");
            this.ckbInr_AlgnF.BackColor = System.Drawing.Color.LightGray;
            this.ckbInr_AlgnF.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbInr_AlgnF.FlatAppearance.BorderSize = 2;
            this.ckbInr_AlgnF.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbInr_AlgnF.ForeColor = System.Drawing.Color.Black;
            this.ckbInr_AlgnF.Name = "ckbInr_AlgnF";
            this.ckbInr_AlgnF.Tag = "0";
            this.ckbInr_AlgnF.UseVisualStyleBackColor = false;
            this.ckbInr_AlgnF.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            this.ckbInr_AlgnF.Click += new System.EventHandler(this.ckb_Click);
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
            // btn2_Home
            // 
            this.btn2_Home.BackColor = System.Drawing.Color.SteelBlue;
            this.btn2_Home.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn2_Home, "btn2_Home");
            this.btn2_Home.ForeColor = System.Drawing.Color.White;
            this.btn2_Home.Image = global::SG2000X.Properties.Resources.Home64;
            this.btn2_Home.Name = "btn2_Home";
            this.btn2_Home.Tag = "INR_Home";
            this.btn2_Home.UseVisualStyleBackColor = false;
            this.btn2_Home.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // btn2_Wait
            // 
            this.btn2_Wait.BackColor = System.Drawing.Color.SteelBlue;
            this.btn2_Wait.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn2_Wait, "btn2_Wait");
            this.btn2_Wait.ForeColor = System.Drawing.Color.White;
            this.btn2_Wait.Image = global::SG2000X.Properties.Resources.Wait64;
            this.btn2_Wait.Name = "btn2_Wait";
            this.btn2_Wait.Tag = "INR_Wait";
            this.btn2_Wait.UseVisualStyleBackColor = false;
            this.btn2_Wait.Click += new System.EventHandler(this.btn_Cycle_Click);
            // 
            // lbl_Unit4
            // 
            this.lbl_Unit4.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit4, "lbl_Unit4");
            this.lbl_Unit4.Name = "lbl_Unit4";
            this.lbl_Unit4.Tag = "Unit #4";
            this.lbl_Unit4.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lbl_Unit3
            // 
            this.lbl_Unit3.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit3, "lbl_Unit3");
            this.lbl_Unit3.Name = "lbl_Unit3";
            this.lbl_Unit3.Tag = "Unit #3";
            this.lbl_Unit3.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lbl_Unit2
            // 
            this.lbl_Unit2.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit2, "lbl_Unit2");
            this.lbl_Unit2.Name = "lbl_Unit2";
            this.lbl_Unit2.Tag = "Unit #2";
            this.lbl_Unit2.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lbl_Unit1
            // 
            this.lbl_Unit1.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit1, "lbl_Unit1");
            this.lbl_Unit1.Name = "lbl_Unit1";
            this.lbl_Unit1.Tag = "Unit #1";
            this.lbl_Unit1.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lbl_GripClpR
            // 
            this.lbl_GripClpR.BackColor = System.Drawing.Color.LightGray;
            this.lbl_GripClpR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_GripClpR, "lbl_GripClpR");
            this.lbl_GripClpR.Name = "lbl_GripClpR";
            this.lbl_GripClpR.Tag = "Gripper Rear$Unclamp/Clamp";
            // 
            // lbl_UTilt
            // 
            this.lbl_UTilt.BackColor = System.Drawing.Color.LightGray;
            this.lbl_UTilt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_UTilt, "lbl_UTilt");
            this.lbl_UTilt.Name = "lbl_UTilt";
            this.lbl_UTilt.Tag = "Unit Tilt$";
            this.lbl_UTilt.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblM_Pcb4
            // 
            this.lblM_Pcb4.BackColor = System.Drawing.Color.LightGray;
            this.lblM_Pcb4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_Pcb4, "lblM_Pcb4");
            this.lblM_Pcb4.Name = "lblM_Pcb4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblM_Pcb5
            // 
            this.lblM_Pcb5.BackColor = System.Drawing.Color.LightGray;
            this.lblM_Pcb5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblM_Pcb5, "lblM_Pcb5");
            this.lblM_Pcb5.Name = "lblM_Pcb5";
            // 
            // vwMan_03Inr
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lbl_UTilt);
            this.Controls.Add(this.lbl_Unit4);
            this.Controls.Add(this.lbl_Unit3);
            this.Controls.Add(this.lbl_Unit2);
            this.Controls.Add(this.lbl_Unit1);
            this.Controls.Add(this.lbl_GripClpR);
            this.Controls.Add(this.btnInr_Ori);
            this.Controls.Add(this.btnInr_Bcr);
            this.Controls.Add(this.ckbInr_IOOn);
            this.Controls.Add(this.pl_Dynamic);
            this.Controls.Add(this.pnlInr_Bcr);
            this.Controls.Add(this.ckb_PrbAir);
            this.Controls.Add(this.ckb_PrbDn);
            this.Controls.Add(this.btnInr_Dynamic);
            this.Controls.Add(this.panel11);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.ckbInr_LiftUp);
            this.Controls.Add(this.ckbInr_GripClpOn);
            this.Controls.Add(this.lblInr_LiftU);
            this.Controls.Add(this.lbl_GripClp);
            this.Controls.Add(this.lblInr_GripD);
            this.Controls.Add(this.lblInr_Algn);
            this.Controls.Add(this.lbl_StripBtm);
            this.Controls.Add(this.lblInr_StripIn);
            this.Controls.Add(this.btnInr_Algn);
            this.Controls.Add(this.ckbInr_AlgnF);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn2_Home);
            this.Controls.Add(this.btn2_Wait);
            resources.ApplyResources(this, "$this");
            this.Name = "vwMan_03Inr";
            this.pl_Dynamic.ResumeLayout(false);
            this.pnlInr_Bcr.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInr_Ori;
        private System.Windows.Forms.Button btnInr_Bcr;
        private System.Windows.Forms.CheckBox ckbInr_IOOn;
        private System.Windows.Forms.Panel pl_Dynamic;
        private System.Windows.Forms.Label lblM_PcbTtv;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label lblM_PcbMax;
        private System.Windows.Forms.Label lblM_Pcb1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label lblM_PcbMean;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label lblM_Pcb2;
        private System.Windows.Forms.Label lblM_Pcb3;
        private System.Windows.Forms.Panel pnlInr_Bcr;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.Label lbl_BcrTitle;
        private System.Windows.Forms.Label lblInr_Bcr;
        private System.Windows.Forms.CheckBox ckb_PrbAir;
        private System.Windows.Forms.CheckBox ckb_PrbDn;
        private System.Windows.Forms.Button btnInr_Dynamic;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label lblInr_YPos;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label lblInr_YStat;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Label lblInr_XPos;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label lblInr_XStat;
        private System.Windows.Forms.CheckBox ckbInr_LiftUp;
        private System.Windows.Forms.CheckBox ckbInr_GripClpOn;
        private System.Windows.Forms.Label lblInr_LiftU;
        private System.Windows.Forms.Label lbl_GripClp;
        private System.Windows.Forms.Label lblInr_GripD;
        private System.Windows.Forms.Label lblInr_Algn;
        private System.Windows.Forms.Label lbl_StripBtm;
        private System.Windows.Forms.Label lblInr_StripIn;
        private System.Windows.Forms.Button btnInr_Algn;
        private System.Windows.Forms.CheckBox ckbInr_AlgnF;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Button btn2_Home;
        private System.Windows.Forms.Button btn2_Wait;
        private System.Windows.Forms.Label lbl_Unit4;
        private System.Windows.Forms.Label lbl_Unit3;
        private System.Windows.Forms.Label lbl_Unit2;
        private System.Windows.Forms.Label lbl_Unit1;
        private System.Windows.Forms.Label lbl_GripClpR;
        private System.Windows.Forms.Label lbl_UTilt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblM_Pcb5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblM_Pcb4;
    }
}
