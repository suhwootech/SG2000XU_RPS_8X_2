namespace SG2000X
{
    partial class vwMan_09Dry
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_09Dry));
			this.ckbDry_IOOn = new System.Windows.Forms.CheckBox();
			this.btn_DryWork = new System.Windows.Forms.Button();
			this.ckbDry_Flow = new System.Windows.Forms.CheckBox();
			this.ckbDry_Stick = new System.Windows.Forms.CheckBox();
			this.ckbDry_Btm = new System.Windows.Forms.CheckBox();
			this.ckbDry_Top = new System.Windows.Forms.CheckBox();
			this.panel17 = new System.Windows.Forms.Panel();
			this.lblDry_ZPos = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.lbl_ZStat = new System.Windows.Forms.Label();
			this.panel15 = new System.Windows.Forms.Panel();
			this.lblDry_RPos = new System.Windows.Forms.Label();
			this.lbl_RUnit = new System.Windows.Forms.Label();
			this.lbl_RStat = new System.Windows.Forms.Label();
			this.panel18 = new System.Windows.Forms.Panel();
			this.lblDry_XPos = new System.Windows.Forms.Label();
			this.label40 = new System.Windows.Forms.Label();
			this.lbl_XStat = new System.Windows.Forms.Label();
			this.btn_StripOut = new System.Windows.Forms.Button();
			this.btn_CkSafty = new System.Windows.Forms.Button();
			this.lblDry_BtmFlow = new System.Windows.Forms.Label();
			this.lbl_BtmTarget = new System.Windows.Forms.Label();
			this.lbl_BtmStandby = new System.Windows.Forms.Label();
			this.lbl_Lv2 = new System.Windows.Forms.Label();
			this.lbl_Lv1 = new System.Windows.Forms.Label();
			this.lblDry_StpOutD = new System.Windows.Forms.Label();
			this.lblDry_PushO = new System.Windows.Forms.Label();
			this.btn_Stop = new System.Windows.Forms.Button();
			this.btn_Home = new System.Windows.Forms.Button();
			this.btn_Wait = new System.Windows.Forms.Button();
			this.lbl_Unit4 = new System.Windows.Forms.Label();
			this.lbl_Unit3 = new System.Windows.Forms.Label();
			this.lbl_Unit2 = new System.Windows.Forms.Label();
			this.lbl_Unit1 = new System.Windows.Forms.Label();
			this.btn_WaterNozzle = new System.Windows.Forms.Button();
			this.ckbDry_BtmClnNzlMove = new System.Windows.Forms.CheckBox();
			this.ckbDry_BtmClnAirBlow = new System.Windows.Forms.CheckBox();
			this.lblDry_BtmClnNzlForward = new System.Windows.Forms.Label();
			this.lblDry_BtmClnNzlBackward = new System.Windows.Forms.Label();
			this.ckbDry_LevelAirBlow = new System.Windows.Forms.CheckBox();
			this.lbl_FrontClamp = new System.Windows.Forms.Label();
			this.lbl_RearUnClamp = new System.Windows.Forms.Label();
			this.lbl_RearClamp = new System.Windows.Forms.Label();
			this.lbl_FrontUnClamp = new System.Windows.Forms.Label();
			this.ckb_ClampOpen = new System.Windows.Forms.CheckBox();
			this.btn_Z_Up_Pos = new System.Windows.Forms.Button();
			this.panel17.SuspendLayout();
			this.panel15.SuspendLayout();
			this.panel18.SuspendLayout();
			this.SuspendLayout();
			// 
			// ckbDry_IOOn
			// 
			resources.ApplyResources(this.ckbDry_IOOn, "ckbDry_IOOn");
			this.ckbDry_IOOn.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_IOOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_IOOn.FlatAppearance.BorderSize = 2;
			this.ckbDry_IOOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_IOOn.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_IOOn.Name = "ckbDry_IOOn";
			this.ckbDry_IOOn.Tag = "4";
			this.ckbDry_IOOn.UseVisualStyleBackColor = false;
			this.ckbDry_IOOn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_IOOn.Click += new System.EventHandler(this.ckb_Click);
			// 
			// btn_DryWork
			// 
			this.btn_DryWork.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_DryWork.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_DryWork, "btn_DryWork");
			this.btn_DryWork.ForeColor = System.Drawing.Color.White;
			this.btn_DryWork.Name = "btn_DryWork";
			this.btn_DryWork.Tag = "DRY_Run";
			this.btn_DryWork.UseVisualStyleBackColor = false;
			this.btn_DryWork.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// ckbDry_Flow
			// 
			resources.ApplyResources(this.ckbDry_Flow, "ckbDry_Flow");
			this.ckbDry_Flow.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_Flow.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_Flow.FlatAppearance.BorderSize = 2;
			this.ckbDry_Flow.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_Flow.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_Flow.Name = "ckbDry_Flow";
			this.ckbDry_Flow.Tag = "3";
			this.ckbDry_Flow.UseVisualStyleBackColor = false;
			this.ckbDry_Flow.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_Flow.Click += new System.EventHandler(this.ckb_Click);
			// 
			// ckbDry_Stick
			// 
			resources.ApplyResources(this.ckbDry_Stick, "ckbDry_Stick");
			this.ckbDry_Stick.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_Stick.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_Stick.FlatAppearance.BorderSize = 2;
			this.ckbDry_Stick.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_Stick.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_Stick.Name = "ckbDry_Stick";
			this.ckbDry_Stick.Tag = "2";
			this.ckbDry_Stick.UseVisualStyleBackColor = false;
			this.ckbDry_Stick.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_Stick.Click += new System.EventHandler(this.ckb_Click);
			// 
			// ckbDry_Btm
			// 
			resources.ApplyResources(this.ckbDry_Btm, "ckbDry_Btm");
			this.ckbDry_Btm.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_Btm.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_Btm.FlatAppearance.BorderSize = 2;
			this.ckbDry_Btm.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_Btm.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_Btm.Name = "ckbDry_Btm";
			this.ckbDry_Btm.Tag = "1";
			this.ckbDry_Btm.UseVisualStyleBackColor = false;
			this.ckbDry_Btm.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_Btm.Click += new System.EventHandler(this.ckb_Click);
			// 
			// ckbDry_Top
			// 
			resources.ApplyResources(this.ckbDry_Top, "ckbDry_Top");
			this.ckbDry_Top.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_Top.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_Top.FlatAppearance.BorderSize = 2;
			this.ckbDry_Top.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_Top.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_Top.Name = "ckbDry_Top";
			this.ckbDry_Top.Tag = "0";
			this.ckbDry_Top.UseVisualStyleBackColor = false;
			this.ckbDry_Top.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_Top.Click += new System.EventHandler(this.ckb_Click);
			// 
			// panel17
			// 
			this.panel17.BackColor = System.Drawing.Color.LightGray;
			this.panel17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel17.Controls.Add(this.lblDry_ZPos);
			this.panel17.Controls.Add(this.label35);
			this.panel17.Controls.Add(this.lbl_ZStat);
			resources.ApplyResources(this.panel17, "panel17");
			this.panel17.Name = "panel17";
			this.panel17.Tag = "0";
			// 
			// lblDry_ZPos
			// 
			this.lblDry_ZPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblDry_ZPos, "lblDry_ZPos");
			this.lblDry_ZPos.Name = "lblDry_ZPos";
			// 
			// label35
			// 
			resources.ApplyResources(this.label35, "label35");
			this.label35.Name = "label35";
			// 
			// lbl_ZStat
			// 
			this.lbl_ZStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lbl_ZStat, "lbl_ZStat");
			this.lbl_ZStat.Name = "lbl_ZStat";
			// 
			// panel15
			// 
			this.panel15.BackColor = System.Drawing.Color.LightGray;
			this.panel15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel15.Controls.Add(this.lblDry_RPos);
			this.panel15.Controls.Add(this.lbl_RUnit);
			this.panel15.Controls.Add(this.lbl_RStat);
			resources.ApplyResources(this.panel15, "panel15");
			this.panel15.Name = "panel15";
			this.panel15.Tag = "0";
			// 
			// lblDry_RPos
			// 
			this.lblDry_RPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblDry_RPos, "lblDry_RPos");
			this.lblDry_RPos.Name = "lblDry_RPos";
			// 
			// lbl_RUnit
			// 
			resources.ApplyResources(this.lbl_RUnit, "lbl_RUnit");
			this.lbl_RUnit.Name = "lbl_RUnit";
			// 
			// lbl_RStat
			// 
			this.lbl_RStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lbl_RStat, "lbl_RStat");
			this.lbl_RStat.Name = "lbl_RStat";
			// 
			// panel18
			// 
			this.panel18.BackColor = System.Drawing.Color.LightGray;
			this.panel18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel18.Controls.Add(this.lblDry_XPos);
			this.panel18.Controls.Add(this.label40);
			this.panel18.Controls.Add(this.lbl_XStat);
			resources.ApplyResources(this.panel18, "panel18");
			this.panel18.Name = "panel18";
			this.panel18.Tag = "0";
			// 
			// lblDry_XPos
			// 
			this.lblDry_XPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblDry_XPos, "lblDry_XPos");
			this.lblDry_XPos.Name = "lblDry_XPos";
			// 
			// label40
			// 
			resources.ApplyResources(this.label40, "label40");
			this.label40.Name = "label40";
			// 
			// lbl_XStat
			// 
			this.lbl_XStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lbl_XStat, "lbl_XStat");
			this.lbl_XStat.Name = "lbl_XStat";
			// 
			// btn_StripOut
			// 
			this.btn_StripOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_StripOut.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_StripOut, "btn_StripOut");
			this.btn_StripOut.ForeColor = System.Drawing.Color.White;
			this.btn_StripOut.Name = "btn_StripOut";
			this.btn_StripOut.Tag = "DRY_Out";
			this.btn_StripOut.UseVisualStyleBackColor = false;
			this.btn_StripOut.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_CkSafty
			// 
			this.btn_CkSafty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_CkSafty.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_CkSafty, "btn_CkSafty");
			this.btn_CkSafty.ForeColor = System.Drawing.Color.White;
			this.btn_CkSafty.Name = "btn_CkSafty";
			this.btn_CkSafty.Tag = "DRY_CheckSensor";
			this.btn_CkSafty.UseVisualStyleBackColor = false;
			this.btn_CkSafty.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// lblDry_BtmFlow
			// 
			this.lblDry_BtmFlow.BackColor = System.Drawing.Color.LightGray;
			this.lblDry_BtmFlow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblDry_BtmFlow, "lblDry_BtmFlow");
			this.lblDry_BtmFlow.Name = "lblDry_BtmFlow";
			this.lblDry_BtmFlow.Tag = "Bottom Cleaner$Flow";
			this.lblDry_BtmFlow.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_BtmTarget
			// 
			this.lbl_BtmTarget.BackColor = System.Drawing.Color.LightGray;
			this.lbl_BtmTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_BtmTarget, "lbl_BtmTarget");
			this.lbl_BtmTarget.Name = "lbl_BtmTarget";
			this.lbl_BtmTarget.Tag = "Bottom Air Blow$Target Position";
			this.lbl_BtmTarget.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_BtmStandby
			// 
			this.lbl_BtmStandby.BackColor = System.Drawing.Color.LightGray;
			this.lbl_BtmStandby.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_BtmStandby, "lbl_BtmStandby");
			this.lbl_BtmStandby.Name = "lbl_BtmStandby";
			this.lbl_BtmStandby.Tag = "Bottom Air Blow$Standby Position";
			this.lbl_BtmStandby.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_Lv2
			// 
			this.lbl_Lv2.BackColor = System.Drawing.Color.LightGray;
			this.lbl_Lv2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_Lv2, "lbl_Lv2");
			this.lbl_Lv2.Name = "lbl_Lv2";
			this.lbl_Lv2.Tag = "Level Safty #2$";
			this.lbl_Lv2.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_Lv1
			// 
			this.lbl_Lv1.BackColor = System.Drawing.Color.LightGray;
			this.lbl_Lv1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_Lv1, "lbl_Lv1");
			this.lbl_Lv1.Name = "lbl_Lv1";
			this.lbl_Lv1.Tag = "Level Safty #1$";
			this.lbl_Lv1.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblDry_StpOutD
			// 
			this.lblDry_StpOutD.BackColor = System.Drawing.Color.LightGray;
			this.lblDry_StpOutD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblDry_StpOutD, "lblDry_StpOutD");
			this.lblDry_StpOutD.Name = "lblDry_StpOutD";
			this.lblDry_StpOutD.Tag = "Strip Out$Detect";
			this.lblDry_StpOutD.BackColorChanged += new System.EventHandler(this.lblIO_B_BackColorChanged);
			// 
			// lblDry_PushO
			// 
			this.lblDry_PushO.BackColor = System.Drawing.Color.LightGray;
			this.lblDry_PushO.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblDry_PushO, "lblDry_PushO");
			this.lblDry_PushO.Name = "lblDry_PushO";
			this.lblDry_PushO.Tag = "Pusher$Overload";
			this.lblDry_PushO.BackColorChanged += new System.EventHandler(this.lblIO_B_BackColorChanged);
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
			// btn_Home
			// 
			this.btn_Home.BackColor = System.Drawing.Color.SteelBlue;
			this.btn_Home.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_Home, "btn_Home");
			this.btn_Home.ForeColor = System.Drawing.Color.White;
			this.btn_Home.Image = global::SG2000X.Properties.Resources.Home64;
			this.btn_Home.Name = "btn_Home";
			this.btn_Home.Tag = "DRY_Home";
			this.btn_Home.UseVisualStyleBackColor = false;
			this.btn_Home.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_Wait
			// 
			this.btn_Wait.BackColor = System.Drawing.Color.SteelBlue;
			this.btn_Wait.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_Wait, "btn_Wait");
			this.btn_Wait.ForeColor = System.Drawing.Color.White;
			this.btn_Wait.Image = global::SG2000X.Properties.Resources.Wait64;
			this.btn_Wait.Name = "btn_Wait";
			this.btn_Wait.Tag = "DRY_Wait";
			this.btn_Wait.UseVisualStyleBackColor = false;
			this.btn_Wait.Click += new System.EventHandler(this.btn_Cycle_Click);
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
			// btn_WaterNozzle
			// 
			this.btn_WaterNozzle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_WaterNozzle.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_WaterNozzle, "btn_WaterNozzle");
			this.btn_WaterNozzle.ForeColor = System.Drawing.Color.White;
			this.btn_WaterNozzle.Name = "btn_WaterNozzle";
			this.btn_WaterNozzle.Tag = "DRY_WaterNozzle";
			this.btn_WaterNozzle.UseVisualStyleBackColor = false;
			this.btn_WaterNozzle.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// ckbDry_BtmClnNzlMove
			// 
			resources.ApplyResources(this.ckbDry_BtmClnNzlMove, "ckbDry_BtmClnNzlMove");
			this.ckbDry_BtmClnNzlMove.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_BtmClnNzlMove.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_BtmClnNzlMove.FlatAppearance.BorderSize = 2;
			this.ckbDry_BtmClnNzlMove.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_BtmClnNzlMove.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_BtmClnNzlMove.Name = "ckbDry_BtmClnNzlMove";
			this.ckbDry_BtmClnNzlMove.Tag = "5";
			this.ckbDry_BtmClnNzlMove.UseVisualStyleBackColor = false;
			this.ckbDry_BtmClnNzlMove.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_BtmClnNzlMove.Click += new System.EventHandler(this.ckb_Click);
			// 
			// ckbDry_BtmClnAirBlow
			// 
			resources.ApplyResources(this.ckbDry_BtmClnAirBlow, "ckbDry_BtmClnAirBlow");
			this.ckbDry_BtmClnAirBlow.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_BtmClnAirBlow.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_BtmClnAirBlow.FlatAppearance.BorderSize = 2;
			this.ckbDry_BtmClnAirBlow.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_BtmClnAirBlow.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_BtmClnAirBlow.Name = "ckbDry_BtmClnAirBlow";
			this.ckbDry_BtmClnAirBlow.Tag = "6";
			this.ckbDry_BtmClnAirBlow.UseVisualStyleBackColor = false;
			this.ckbDry_BtmClnAirBlow.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_BtmClnAirBlow.Click += new System.EventHandler(this.ckb_Click);
			// 
			// lblDry_BtmClnNzlForward
			// 
			this.lblDry_BtmClnNzlForward.BackColor = System.Drawing.Color.LightGray;
			this.lblDry_BtmClnNzlForward.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblDry_BtmClnNzlForward, "lblDry_BtmClnNzlForward");
			this.lblDry_BtmClnNzlForward.Name = "lblDry_BtmClnNzlForward";
			this.lblDry_BtmClnNzlForward.Tag = "Bottom Cleaner$Nozzle Forward";
			this.lblDry_BtmClnNzlForward.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblDry_BtmClnNzlBackward
			// 
			this.lblDry_BtmClnNzlBackward.BackColor = System.Drawing.Color.LightGray;
			this.lblDry_BtmClnNzlBackward.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblDry_BtmClnNzlBackward, "lblDry_BtmClnNzlBackward");
			this.lblDry_BtmClnNzlBackward.Name = "lblDry_BtmClnNzlBackward";
			this.lblDry_BtmClnNzlBackward.Tag = "Bottom Cleaner$Nozzle Backward";
			this.lblDry_BtmClnNzlBackward.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// ckbDry_LevelAirBlow
			// 
			resources.ApplyResources(this.ckbDry_LevelAirBlow, "ckbDry_LevelAirBlow");
			this.ckbDry_LevelAirBlow.BackColor = System.Drawing.Color.LightGray;
			this.ckbDry_LevelAirBlow.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbDry_LevelAirBlow.FlatAppearance.BorderSize = 2;
			this.ckbDry_LevelAirBlow.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbDry_LevelAirBlow.ForeColor = System.Drawing.Color.Black;
			this.ckbDry_LevelAirBlow.Name = "ckbDry_LevelAirBlow";
			this.ckbDry_LevelAirBlow.Tag = "7";
			this.ckbDry_LevelAirBlow.UseVisualStyleBackColor = false;
			this.ckbDry_LevelAirBlow.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbDry_LevelAirBlow.Click += new System.EventHandler(this.ckb_Click);
			// 
			// lbl_FrontClamp
			// 
			this.lbl_FrontClamp.BackColor = System.Drawing.Color.LightGray;
			this.lbl_FrontClamp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_FrontClamp, "lbl_FrontClamp");
			this.lbl_FrontClamp.Name = "lbl_FrontClamp";
			this.lbl_FrontClamp.Tag = "Front Strip Clamp$Clamp";
			this.lbl_FrontClamp.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_RearUnClamp
			// 
			this.lbl_RearUnClamp.BackColor = System.Drawing.Color.LightGray;
			this.lbl_RearUnClamp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_RearUnClamp, "lbl_RearUnClamp");
			this.lbl_RearUnClamp.Name = "lbl_RearUnClamp";
			this.lbl_RearUnClamp.Tag = "Rear Strip Clamp$Unclamp";
			this.lbl_RearUnClamp.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_RearClamp
			// 
			this.lbl_RearClamp.BackColor = System.Drawing.Color.LightGray;
			this.lbl_RearClamp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_RearClamp, "lbl_RearClamp");
			this.lbl_RearClamp.Name = "lbl_RearClamp";
			this.lbl_RearClamp.Tag = "Rear Strip Clamp$Clamp";
			this.lbl_RearClamp.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lbl_FrontUnClamp
			// 
			this.lbl_FrontUnClamp.BackColor = System.Drawing.Color.LightGray;
			this.lbl_FrontUnClamp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lbl_FrontUnClamp, "lbl_FrontUnClamp");
			this.lbl_FrontUnClamp.Name = "lbl_FrontUnClamp";
			this.lbl_FrontUnClamp.Tag = "Front Strip Clamp$Unclamp";
			this.lbl_FrontUnClamp.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// ckb_ClampOpen
			// 
			resources.ApplyResources(this.ckb_ClampOpen, "ckb_ClampOpen");
			this.ckb_ClampOpen.BackColor = System.Drawing.Color.LightGray;
			this.ckb_ClampOpen.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckb_ClampOpen.FlatAppearance.BorderSize = 2;
			this.ckb_ClampOpen.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckb_ClampOpen.ForeColor = System.Drawing.Color.Black;
			this.ckb_ClampOpen.Name = "ckb_ClampOpen";
			this.ckb_ClampOpen.Tag = "8";
			this.ckb_ClampOpen.UseVisualStyleBackColor = false;
			this.ckb_ClampOpen.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckb_ClampOpen.Click += new System.EventHandler(this.ckb_Click);
			// 
			// btn_Z_Up_Pos
			// 
			resources.ApplyResources(this.btn_Z_Up_Pos, "btn_Z_Up_Pos");
			this.btn_Z_Up_Pos.Name = "btn_Z_Up_Pos";
			this.btn_Z_Up_Pos.UseVisualStyleBackColor = true;
			this.btn_Z_Up_Pos.Click += new System.EventHandler(this.btn_Z_Up_Pos_Click);
			// 
			// vwMan_09Dry
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.btn_Z_Up_Pos);
			this.Controls.Add(this.ckb_ClampOpen);
			this.Controls.Add(this.lbl_RearUnClamp);
			this.Controls.Add(this.lbl_RearClamp);
			this.Controls.Add(this.lbl_FrontUnClamp);
			this.Controls.Add(this.lbl_FrontClamp);
			this.Controls.Add(this.ckbDry_LevelAirBlow);
			this.Controls.Add(this.ckbDry_BtmClnAirBlow);
			this.Controls.Add(this.ckbDry_BtmClnNzlMove);
			this.Controls.Add(this.btn_WaterNozzle);
			this.Controls.Add(this.lbl_Unit4);
			this.Controls.Add(this.lbl_Unit3);
			this.Controls.Add(this.lbl_Unit2);
			this.Controls.Add(this.lbl_Unit1);
			this.Controls.Add(this.ckbDry_IOOn);
			this.Controls.Add(this.btn_DryWork);
			this.Controls.Add(this.ckbDry_Flow);
			this.Controls.Add(this.ckbDry_Stick);
			this.Controls.Add(this.ckbDry_Btm);
			this.Controls.Add(this.ckbDry_Top);
			this.Controls.Add(this.panel17);
			this.Controls.Add(this.panel15);
			this.Controls.Add(this.panel18);
			this.Controls.Add(this.btn_StripOut);
			this.Controls.Add(this.btn_CkSafty);
			this.Controls.Add(this.lblDry_BtmClnNzlBackward);
			this.Controls.Add(this.lblDry_BtmClnNzlForward);
			this.Controls.Add(this.lblDry_BtmFlow);
			this.Controls.Add(this.lbl_BtmTarget);
			this.Controls.Add(this.lbl_BtmStandby);
			this.Controls.Add(this.lbl_Lv2);
			this.Controls.Add(this.lbl_Lv1);
			this.Controls.Add(this.lblDry_StpOutD);
			this.Controls.Add(this.lblDry_PushO);
			this.Controls.Add(this.btn_Stop);
			this.Controls.Add(this.btn_Home);
			this.Controls.Add(this.btn_Wait);
			resources.ApplyResources(this, "$this");
			this.Name = "vwMan_09Dry";
			this.panel17.ResumeLayout(false);
			this.panel15.ResumeLayout(false);
			this.panel18.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbDry_IOOn;
        private System.Windows.Forms.Button btn_DryWork;
        private System.Windows.Forms.CheckBox ckbDry_Flow;
        private System.Windows.Forms.CheckBox ckbDry_Stick;
        private System.Windows.Forms.CheckBox ckbDry_Btm;
        private System.Windows.Forms.CheckBox ckbDry_Top;
        private System.Windows.Forms.Panel panel17;
        private System.Windows.Forms.Label lblDry_ZPos;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label lbl_ZStat;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Label lblDry_RPos;
        private System.Windows.Forms.Label lbl_RUnit;
        private System.Windows.Forms.Label lbl_RStat;
        private System.Windows.Forms.Panel panel18;
        private System.Windows.Forms.Label lblDry_XPos;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label lbl_XStat;
        private System.Windows.Forms.Button btn_StripOut;
        private System.Windows.Forms.Button btn_CkSafty;
        private System.Windows.Forms.Label lblDry_BtmFlow;
        private System.Windows.Forms.Label lbl_BtmTarget;
        private System.Windows.Forms.Label lbl_BtmStandby;
        private System.Windows.Forms.Label lbl_Lv2;
        private System.Windows.Forms.Label lbl_Lv1;
        private System.Windows.Forms.Label lblDry_StpOutD;
        private System.Windows.Forms.Label lblDry_PushO;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Button btn_Home;
        private System.Windows.Forms.Button btn_Wait;
        private System.Windows.Forms.Label lbl_Unit4;
        private System.Windows.Forms.Label lbl_Unit3;
        private System.Windows.Forms.Label lbl_Unit2;
        private System.Windows.Forms.Label lbl_Unit1;
        private System.Windows.Forms.Button btn_WaterNozzle;
		private System.Windows.Forms.CheckBox ckbDry_BtmClnNzlMove;
		private System.Windows.Forms.CheckBox ckbDry_BtmClnAirBlow;
		private System.Windows.Forms.Label lblDry_BtmClnNzlForward;
		private System.Windows.Forms.Label lblDry_BtmClnNzlBackward;
		private System.Windows.Forms.CheckBox ckbDry_LevelAirBlow;
		private System.Windows.Forms.Label lbl_FrontClamp;
		private System.Windows.Forms.Label lbl_RearUnClamp;
		private System.Windows.Forms.Label lbl_RearClamp;
		private System.Windows.Forms.Label lbl_FrontUnClamp;
		private System.Windows.Forms.CheckBox ckb_ClampOpen;
        private System.Windows.Forms.Button btn_Z_Up_Pos;
    }
}
