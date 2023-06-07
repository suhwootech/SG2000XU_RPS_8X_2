namespace SG2000X
{
    partial class vwOpt_06Lvs
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
			System.Windows.Forms.TabPage tp4_Wheel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwOpt_06Lvs));
			this.cbLv_WhlDrsChange = new System.Windows.Forms.ComboBox();
			this.cbLv_WhlChange = new System.Windows.Forms.ComboBox();
			this.label264 = new System.Windows.Forms.Label();
			this.label258 = new System.Windows.Forms.Label();
			this.cbLv_WhlSave = new System.Windows.Forms.ComboBox();
			this.label225 = new System.Windows.Forms.Label();
			this.cbLv_WhlDel = new System.Windows.Forms.ComboBox();
			this.label226 = new System.Windows.Forms.Label();
			this.cbLv_WhlSaveAs = new System.Windows.Forms.ComboBox();
			this.label227 = new System.Windows.Forms.Label();
			this.cbLv_WhlNew = new System.Windows.Forms.ComboBox();
			this.label228 = new System.Windows.Forms.Label();
			this.tap_Level = new System.Windows.Forms.TabControl();
			this.tp1_Auto = new System.Windows.Forms.TabPage();
			this.cbLv_Exit = new System.Windows.Forms.ComboBox();
			this.label184 = new System.Windows.Forms.Label();
			this.cbLv_Util = new System.Windows.Forms.ComboBox();
			this.label185 = new System.Windows.Forms.Label();
			this.cbLv_Option = new System.Windows.Forms.ComboBox();
			this.label188 = new System.Windows.Forms.Label();
			this.cbLv_Spc = new System.Windows.Forms.ComboBox();
			this.label190 = new System.Windows.Forms.Label();
			this.cbLv_Wheel = new System.Windows.Forms.ComboBox();
			this.label183 = new System.Windows.Forms.Label();
			this.cbLv_Device = new System.Windows.Forms.ComboBox();
			this.label182 = new System.Windows.Forms.Label();
			this.cbLv_Manual = new System.Windows.Forms.ComboBox();
			this.label181 = new System.Windows.Forms.Label();
			this.tp2_Manual = new System.Windows.Forms.TabPage();
			this.cbLv_AllSvOff = new System.Windows.Forms.ComboBox();
			this.label288 = new System.Windows.Forms.Label();
			this.cbLv_AllSvOn = new System.Windows.Forms.ComboBox();
			this.label289 = new System.Windows.Forms.Label();
			this.cbLv_WarmSet = new System.Windows.Forms.ComboBox();
			this.label206 = new System.Windows.Forms.Label();
			this.cbLv_OfL = new System.Windows.Forms.ComboBox();
			this.label203 = new System.Windows.Forms.Label();
			this.cbLv_Dry = new System.Windows.Forms.ComboBox();
			this.label205 = new System.Windows.Forms.Label();
			this.cbLv_Ofp = new System.Windows.Forms.ComboBox();
			this.label179 = new System.Windows.Forms.Label();
			this.cbLv_Grr = new System.Windows.Forms.ComboBox();
			this.label191 = new System.Windows.Forms.Label();
			this.cbLv_Grd = new System.Windows.Forms.ComboBox();
			this.label192 = new System.Windows.Forms.Label();
			this.cbLv_GrL = new System.Windows.Forms.ComboBox();
			this.label193 = new System.Windows.Forms.Label();
			this.cbLv_Onp = new System.Windows.Forms.ComboBox();
			this.label195 = new System.Windows.Forms.Label();
			this.cbLv_Inr = new System.Windows.Forms.ComboBox();
			this.label199 = new System.Windows.Forms.Label();
			this.cbLv_OnL = new System.Windows.Forms.ComboBox();
			this.label202 = new System.Windows.Forms.Label();
			this.tp3_Device = new System.Windows.Forms.TabPage();
			this.cbLv_DvPosView = new System.Windows.Forms.ComboBox();
			this.label286 = new System.Windows.Forms.Label();
			this.cbLv_DvParaEnable = new System.Windows.Forms.ComboBox();
			this.label287 = new System.Windows.Forms.Label();
			this.cbLv_DvSave = new System.Windows.Forms.ComboBox();
			this.label207 = new System.Windows.Forms.Label();
			this.cbLv_DvCurrent = new System.Windows.Forms.ComboBox();
			this.label208 = new System.Windows.Forms.Label();
			this.cbLv_DvLoad = new System.Windows.Forms.ComboBox();
			this.label209 = new System.Windows.Forms.Label();
			this.cbLv_DvDel = new System.Windows.Forms.ComboBox();
			this.label211 = new System.Windows.Forms.Label();
			this.cbLv_DvSaveAs = new System.Windows.Forms.ComboBox();
			this.label212 = new System.Windows.Forms.Label();
			this.cbLv_DvNew = new System.Windows.Forms.ComboBox();
			this.label214 = new System.Windows.Forms.Label();
			this.cbLv_GpDel = new System.Windows.Forms.ComboBox();
			this.label215 = new System.Windows.Forms.Label();
			this.cbLv_GpSaveAs = new System.Windows.Forms.ComboBox();
			this.label217 = new System.Windows.Forms.Label();
			this.cbLv_GpNew = new System.Windows.Forms.ComboBox();
			this.label218 = new System.Windows.Forms.Label();
			this.tp5_Spc = new System.Windows.Forms.TabPage();
			this.cbLv_SpcErrHisSave = new System.Windows.Forms.ComboBox();
			this.label230 = new System.Windows.Forms.Label();
			this.cbLv_SpcErrHisView = new System.Windows.Forms.ComboBox();
			this.label229 = new System.Windows.Forms.Label();
			this.cbLv_SpcGpSave = new System.Windows.Forms.ComboBox();
			this.label223 = new System.Windows.Forms.Label();
			this.cbLv_SpcErrHis = new System.Windows.Forms.ComboBox();
			this.label222 = new System.Windows.Forms.Label();
			this.cbLv_SpcErrList = new System.Windows.Forms.ComboBox();
			this.label220 = new System.Windows.Forms.Label();
			this.tp6_Option = new System.Windows.Forms.TabPage();
			this.cbLv_OptTbGrd = new System.Windows.Forms.ComboBox();
			this.label221 = new System.Windows.Forms.Label();
			this.cbLv_OptGrind = new System.Windows.Forms.ComboBox();
			this.cbLv_OptPicker = new System.Windows.Forms.ComboBox();
			this.cbLv_OptRailDry = new System.Windows.Forms.ComboBox();
			this.cbLv_OptLoader = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cbLv_OptSysPos = new System.Windows.Forms.ComboBox();
			this.label231 = new System.Windows.Forms.Label();
			this.tp7_Util = new System.Windows.Forms.TabPage();
			this.cbLv_UtilRepeat = new System.Windows.Forms.ComboBox();
			this.label240 = new System.Windows.Forms.Label();
			this.cbLv_UtilBcr = new System.Windows.Forms.ComboBox();
			this.label233 = new System.Windows.Forms.Label();
			this.cbLv_UtilTw = new System.Windows.Forms.ComboBox();
			this.label234 = new System.Windows.Forms.Label();
			this.cbLv_UtilPrb = new System.Windows.Forms.ComboBox();
			this.label235 = new System.Windows.Forms.Label();
			this.cbLv_UtilOut = new System.Windows.Forms.ComboBox();
			this.label236 = new System.Windows.Forms.Label();
			this.cbLv_UtilIn = new System.Windows.Forms.ComboBox();
			this.label237 = new System.Windows.Forms.Label();
			this.cbLv_UtilSpd = new System.Windows.Forms.ComboBox();
			this.label238 = new System.Windows.Forms.Label();
			this.cbLv_UtilMot = new System.Windows.Forms.ComboBox();
			this.label239 = new System.Windows.Forms.Label();
			this.tp8_OpManual = new System.Windows.Forms.TabPage();
			this.cbLv_OpStripExistEdit = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbLv_OpDrsPos = new System.Windows.Forms.ComboBox();
			this.label290 = new System.Windows.Forms.Label();
			this.cbLv_OptMnt = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cbLv_OptGeneral = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			tp4_Wheel = new System.Windows.Forms.TabPage();
			tp4_Wheel.SuspendLayout();
			this.tap_Level.SuspendLayout();
			this.tp1_Auto.SuspendLayout();
			this.tp2_Manual.SuspendLayout();
			this.tp3_Device.SuspendLayout();
			this.tp5_Spc.SuspendLayout();
			this.tp6_Option.SuspendLayout();
			this.tp7_Util.SuspendLayout();
			this.tp8_OpManual.SuspendLayout();
			this.SuspendLayout();
			// 
			// tp4_Wheel
			// 
			tp4_Wheel.BackColor = System.Drawing.Color.White;
			tp4_Wheel.Controls.Add(this.cbLv_WhlDrsChange);
			tp4_Wheel.Controls.Add(this.cbLv_WhlChange);
			tp4_Wheel.Controls.Add(this.label264);
			tp4_Wheel.Controls.Add(this.label258);
			tp4_Wheel.Controls.Add(this.cbLv_WhlSave);
			tp4_Wheel.Controls.Add(this.label225);
			tp4_Wheel.Controls.Add(this.cbLv_WhlDel);
			tp4_Wheel.Controls.Add(this.label226);
			tp4_Wheel.Controls.Add(this.cbLv_WhlSaveAs);
			tp4_Wheel.Controls.Add(this.label227);
			tp4_Wheel.Controls.Add(this.cbLv_WhlNew);
			tp4_Wheel.Controls.Add(this.label228);
			resources.ApplyResources(tp4_Wheel, "tp4_Wheel");
			tp4_Wheel.Name = "tp4_Wheel";
			// 
			// cbLv_WhlDrsChange
			// 
			this.cbLv_WhlDrsChange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WhlDrsChange, "cbLv_WhlDrsChange");
			this.cbLv_WhlDrsChange.FormattingEnabled = true;
			this.cbLv_WhlDrsChange.Items.AddRange(new object[] {
            resources.GetString("cbLv_WhlDrsChange.Items"),
            resources.GetString("cbLv_WhlDrsChange.Items1"),
            resources.GetString("cbLv_WhlDrsChange.Items2")});
			this.cbLv_WhlDrsChange.Name = "cbLv_WhlDrsChange";
			// 
			// cbLv_WhlChange
			// 
			this.cbLv_WhlChange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WhlChange, "cbLv_WhlChange");
			this.cbLv_WhlChange.FormattingEnabled = true;
			this.cbLv_WhlChange.Items.AddRange(new object[] {
            resources.GetString("cbLv_WhlChange.Items"),
            resources.GetString("cbLv_WhlChange.Items1"),
            resources.GetString("cbLv_WhlChange.Items2")});
			this.cbLv_WhlChange.Name = "cbLv_WhlChange";
			// 
			// label264
			// 
			this.label264.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label264, "label264");
			this.label264.ForeColor = System.Drawing.Color.Black;
			this.label264.Name = "label264";
			// 
			// label258
			// 
			this.label258.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label258, "label258");
			this.label258.ForeColor = System.Drawing.Color.Black;
			this.label258.Name = "label258";
			// 
			// cbLv_WhlSave
			// 
			this.cbLv_WhlSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WhlSave, "cbLv_WhlSave");
			this.cbLv_WhlSave.FormattingEnabled = true;
			this.cbLv_WhlSave.Items.AddRange(new object[] {
            resources.GetString("cbLv_WhlSave.Items"),
            resources.GetString("cbLv_WhlSave.Items1"),
            resources.GetString("cbLv_WhlSave.Items2")});
			this.cbLv_WhlSave.Name = "cbLv_WhlSave";
			// 
			// label225
			// 
			this.label225.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label225, "label225");
			this.label225.ForeColor = System.Drawing.Color.Black;
			this.label225.Name = "label225";
			// 
			// cbLv_WhlDel
			// 
			this.cbLv_WhlDel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WhlDel, "cbLv_WhlDel");
			this.cbLv_WhlDel.FormattingEnabled = true;
			this.cbLv_WhlDel.Items.AddRange(new object[] {
            resources.GetString("cbLv_WhlDel.Items"),
            resources.GetString("cbLv_WhlDel.Items1"),
            resources.GetString("cbLv_WhlDel.Items2")});
			this.cbLv_WhlDel.Name = "cbLv_WhlDel";
			// 
			// label226
			// 
			this.label226.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label226, "label226");
			this.label226.ForeColor = System.Drawing.Color.Black;
			this.label226.Name = "label226";
			// 
			// cbLv_WhlSaveAs
			// 
			this.cbLv_WhlSaveAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WhlSaveAs, "cbLv_WhlSaveAs");
			this.cbLv_WhlSaveAs.FormattingEnabled = true;
			this.cbLv_WhlSaveAs.Items.AddRange(new object[] {
            resources.GetString("cbLv_WhlSaveAs.Items"),
            resources.GetString("cbLv_WhlSaveAs.Items1"),
            resources.GetString("cbLv_WhlSaveAs.Items2")});
			this.cbLv_WhlSaveAs.Name = "cbLv_WhlSaveAs";
			// 
			// label227
			// 
			this.label227.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label227, "label227");
			this.label227.ForeColor = System.Drawing.Color.Black;
			this.label227.Name = "label227";
			// 
			// cbLv_WhlNew
			// 
			this.cbLv_WhlNew.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WhlNew, "cbLv_WhlNew");
			this.cbLv_WhlNew.FormattingEnabled = true;
			this.cbLv_WhlNew.Items.AddRange(new object[] {
            resources.GetString("cbLv_WhlNew.Items"),
            resources.GetString("cbLv_WhlNew.Items1"),
            resources.GetString("cbLv_WhlNew.Items2")});
			this.cbLv_WhlNew.Name = "cbLv_WhlNew";
			// 
			// label228
			// 
			this.label228.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label228, "label228");
			this.label228.ForeColor = System.Drawing.Color.Black;
			this.label228.Name = "label228";
			// 
			// tap_Level
			// 
			this.tap_Level.Controls.Add(this.tp1_Auto);
			this.tap_Level.Controls.Add(this.tp2_Manual);
			this.tap_Level.Controls.Add(this.tp3_Device);
			this.tap_Level.Controls.Add(tp4_Wheel);
			this.tap_Level.Controls.Add(this.tp5_Spc);
			this.tap_Level.Controls.Add(this.tp6_Option);
			this.tap_Level.Controls.Add(this.tp7_Util);
			this.tap_Level.Controls.Add(this.tp8_OpManual);
			resources.ApplyResources(this.tap_Level, "tap_Level");
			this.tap_Level.Name = "tap_Level";
			this.tap_Level.SelectedIndex = 0;
			this.tap_Level.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			// 
			// tp1_Auto
			// 
			this.tp1_Auto.BackColor = System.Drawing.Color.White;
			this.tp1_Auto.Controls.Add(this.cbLv_Exit);
			this.tp1_Auto.Controls.Add(this.label184);
			this.tp1_Auto.Controls.Add(this.cbLv_Util);
			this.tp1_Auto.Controls.Add(this.label185);
			this.tp1_Auto.Controls.Add(this.cbLv_Option);
			this.tp1_Auto.Controls.Add(this.label188);
			this.tp1_Auto.Controls.Add(this.cbLv_Spc);
			this.tp1_Auto.Controls.Add(this.label190);
			this.tp1_Auto.Controls.Add(this.cbLv_Wheel);
			this.tp1_Auto.Controls.Add(this.label183);
			this.tp1_Auto.Controls.Add(this.cbLv_Device);
			this.tp1_Auto.Controls.Add(this.label182);
			this.tp1_Auto.Controls.Add(this.cbLv_Manual);
			this.tp1_Auto.Controls.Add(this.label181);
			resources.ApplyResources(this.tp1_Auto, "tp1_Auto");
			this.tp1_Auto.Name = "tp1_Auto";
			// 
			// cbLv_Exit
			// 
			this.cbLv_Exit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Exit, "cbLv_Exit");
			this.cbLv_Exit.FormattingEnabled = true;
			this.cbLv_Exit.Items.AddRange(new object[] {
            resources.GetString("cbLv_Exit.Items"),
            resources.GetString("cbLv_Exit.Items1"),
            resources.GetString("cbLv_Exit.Items2")});
			this.cbLv_Exit.Name = "cbLv_Exit";
			// 
			// label184
			// 
			this.label184.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label184, "label184");
			this.label184.ForeColor = System.Drawing.Color.Black;
			this.label184.Name = "label184";
			// 
			// cbLv_Util
			// 
			this.cbLv_Util.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Util, "cbLv_Util");
			this.cbLv_Util.FormattingEnabled = true;
			this.cbLv_Util.Items.AddRange(new object[] {
            resources.GetString("cbLv_Util.Items"),
            resources.GetString("cbLv_Util.Items1"),
            resources.GetString("cbLv_Util.Items2")});
			this.cbLv_Util.Name = "cbLv_Util";
			// 
			// label185
			// 
			this.label185.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label185, "label185");
			this.label185.ForeColor = System.Drawing.Color.Black;
			this.label185.Name = "label185";
			// 
			// cbLv_Option
			// 
			this.cbLv_Option.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Option, "cbLv_Option");
			this.cbLv_Option.FormattingEnabled = true;
			this.cbLv_Option.Items.AddRange(new object[] {
            resources.GetString("cbLv_Option.Items"),
            resources.GetString("cbLv_Option.Items1"),
            resources.GetString("cbLv_Option.Items2")});
			this.cbLv_Option.Name = "cbLv_Option";
			// 
			// label188
			// 
			this.label188.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label188, "label188");
			this.label188.ForeColor = System.Drawing.Color.Black;
			this.label188.Name = "label188";
			// 
			// cbLv_Spc
			// 
			this.cbLv_Spc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Spc, "cbLv_Spc");
			this.cbLv_Spc.FormattingEnabled = true;
			this.cbLv_Spc.Items.AddRange(new object[] {
            resources.GetString("cbLv_Spc.Items"),
            resources.GetString("cbLv_Spc.Items1"),
            resources.GetString("cbLv_Spc.Items2")});
			this.cbLv_Spc.Name = "cbLv_Spc";
			// 
			// label190
			// 
			this.label190.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label190, "label190");
			this.label190.ForeColor = System.Drawing.Color.Black;
			this.label190.Name = "label190";
			// 
			// cbLv_Wheel
			// 
			this.cbLv_Wheel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Wheel, "cbLv_Wheel");
			this.cbLv_Wheel.FormattingEnabled = true;
			this.cbLv_Wheel.Items.AddRange(new object[] {
            resources.GetString("cbLv_Wheel.Items"),
            resources.GetString("cbLv_Wheel.Items1"),
            resources.GetString("cbLv_Wheel.Items2")});
			this.cbLv_Wheel.Name = "cbLv_Wheel";
			// 
			// label183
			// 
			this.label183.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label183, "label183");
			this.label183.ForeColor = System.Drawing.Color.Black;
			this.label183.Name = "label183";
			// 
			// cbLv_Device
			// 
			this.cbLv_Device.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Device, "cbLv_Device");
			this.cbLv_Device.FormattingEnabled = true;
			this.cbLv_Device.Items.AddRange(new object[] {
            resources.GetString("cbLv_Device.Items"),
            resources.GetString("cbLv_Device.Items1"),
            resources.GetString("cbLv_Device.Items2")});
			this.cbLv_Device.Name = "cbLv_Device";
			// 
			// label182
			// 
			this.label182.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label182, "label182");
			this.label182.ForeColor = System.Drawing.Color.Black;
			this.label182.Name = "label182";
			// 
			// cbLv_Manual
			// 
			this.cbLv_Manual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Manual, "cbLv_Manual");
			this.cbLv_Manual.FormattingEnabled = true;
			this.cbLv_Manual.Items.AddRange(new object[] {
            resources.GetString("cbLv_Manual.Items"),
            resources.GetString("cbLv_Manual.Items1"),
            resources.GetString("cbLv_Manual.Items2")});
			this.cbLv_Manual.Name = "cbLv_Manual";
			// 
			// label181
			// 
			this.label181.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label181, "label181");
			this.label181.ForeColor = System.Drawing.Color.Black;
			this.label181.Name = "label181";
			// 
			// tp2_Manual
			// 
			this.tp2_Manual.BackColor = System.Drawing.Color.White;
			this.tp2_Manual.Controls.Add(this.cbLv_AllSvOff);
			this.tp2_Manual.Controls.Add(this.label288);
			this.tp2_Manual.Controls.Add(this.cbLv_AllSvOn);
			this.tp2_Manual.Controls.Add(this.label289);
			this.tp2_Manual.Controls.Add(this.cbLv_WarmSet);
			this.tp2_Manual.Controls.Add(this.label206);
			this.tp2_Manual.Controls.Add(this.cbLv_OfL);
			this.tp2_Manual.Controls.Add(this.label203);
			this.tp2_Manual.Controls.Add(this.cbLv_Dry);
			this.tp2_Manual.Controls.Add(this.label205);
			this.tp2_Manual.Controls.Add(this.cbLv_Ofp);
			this.tp2_Manual.Controls.Add(this.label179);
			this.tp2_Manual.Controls.Add(this.cbLv_Grr);
			this.tp2_Manual.Controls.Add(this.label191);
			this.tp2_Manual.Controls.Add(this.cbLv_Grd);
			this.tp2_Manual.Controls.Add(this.label192);
			this.tp2_Manual.Controls.Add(this.cbLv_GrL);
			this.tp2_Manual.Controls.Add(this.label193);
			this.tp2_Manual.Controls.Add(this.cbLv_Onp);
			this.tp2_Manual.Controls.Add(this.label195);
			this.tp2_Manual.Controls.Add(this.cbLv_Inr);
			this.tp2_Manual.Controls.Add(this.label199);
			this.tp2_Manual.Controls.Add(this.cbLv_OnL);
			this.tp2_Manual.Controls.Add(this.label202);
			resources.ApplyResources(this.tp2_Manual, "tp2_Manual");
			this.tp2_Manual.Name = "tp2_Manual";
			// 
			// cbLv_AllSvOff
			// 
			this.cbLv_AllSvOff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_AllSvOff, "cbLv_AllSvOff");
			this.cbLv_AllSvOff.FormattingEnabled = true;
			this.cbLv_AllSvOff.Items.AddRange(new object[] {
            resources.GetString("cbLv_AllSvOff.Items"),
            resources.GetString("cbLv_AllSvOff.Items1"),
            resources.GetString("cbLv_AllSvOff.Items2")});
			this.cbLv_AllSvOff.Name = "cbLv_AllSvOff";
			// 
			// label288
			// 
			this.label288.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label288, "label288");
			this.label288.ForeColor = System.Drawing.Color.Black;
			this.label288.Name = "label288";
			// 
			// cbLv_AllSvOn
			// 
			this.cbLv_AllSvOn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_AllSvOn, "cbLv_AllSvOn");
			this.cbLv_AllSvOn.FormattingEnabled = true;
			this.cbLv_AllSvOn.Items.AddRange(new object[] {
            resources.GetString("cbLv_AllSvOn.Items"),
            resources.GetString("cbLv_AllSvOn.Items1"),
            resources.GetString("cbLv_AllSvOn.Items2")});
			this.cbLv_AllSvOn.Name = "cbLv_AllSvOn";
			// 
			// label289
			// 
			this.label289.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label289, "label289");
			this.label289.ForeColor = System.Drawing.Color.Black;
			this.label289.Name = "label289";
			// 
			// cbLv_WarmSet
			// 
			this.cbLv_WarmSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_WarmSet, "cbLv_WarmSet");
			this.cbLv_WarmSet.FormattingEnabled = true;
			this.cbLv_WarmSet.Items.AddRange(new object[] {
            resources.GetString("cbLv_WarmSet.Items"),
            resources.GetString("cbLv_WarmSet.Items1"),
            resources.GetString("cbLv_WarmSet.Items2")});
			this.cbLv_WarmSet.Name = "cbLv_WarmSet";
			// 
			// label206
			// 
			this.label206.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label206, "label206");
			this.label206.ForeColor = System.Drawing.Color.Black;
			this.label206.Name = "label206";
			// 
			// cbLv_OfL
			// 
			this.cbLv_OfL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OfL, "cbLv_OfL");
			this.cbLv_OfL.FormattingEnabled = true;
			this.cbLv_OfL.Items.AddRange(new object[] {
            resources.GetString("cbLv_OfL.Items"),
            resources.GetString("cbLv_OfL.Items1"),
            resources.GetString("cbLv_OfL.Items2")});
			this.cbLv_OfL.Name = "cbLv_OfL";
			// 
			// label203
			// 
			this.label203.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label203, "label203");
			this.label203.ForeColor = System.Drawing.Color.Black;
			this.label203.Name = "label203";
			// 
			// cbLv_Dry
			// 
			this.cbLv_Dry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Dry, "cbLv_Dry");
			this.cbLv_Dry.FormattingEnabled = true;
			this.cbLv_Dry.Items.AddRange(new object[] {
            resources.GetString("cbLv_Dry.Items"),
            resources.GetString("cbLv_Dry.Items1"),
            resources.GetString("cbLv_Dry.Items2")});
			this.cbLv_Dry.Name = "cbLv_Dry";
			// 
			// label205
			// 
			this.label205.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label205, "label205");
			this.label205.ForeColor = System.Drawing.Color.Black;
			this.label205.Name = "label205";
			// 
			// cbLv_Ofp
			// 
			this.cbLv_Ofp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Ofp, "cbLv_Ofp");
			this.cbLv_Ofp.FormattingEnabled = true;
			this.cbLv_Ofp.Items.AddRange(new object[] {
            resources.GetString("cbLv_Ofp.Items"),
            resources.GetString("cbLv_Ofp.Items1"),
            resources.GetString("cbLv_Ofp.Items2")});
			this.cbLv_Ofp.Name = "cbLv_Ofp";
			// 
			// label179
			// 
			this.label179.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label179, "label179");
			this.label179.ForeColor = System.Drawing.Color.Black;
			this.label179.Name = "label179";
			// 
			// cbLv_Grr
			// 
			this.cbLv_Grr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Grr, "cbLv_Grr");
			this.cbLv_Grr.FormattingEnabled = true;
			this.cbLv_Grr.Items.AddRange(new object[] {
            resources.GetString("cbLv_Grr.Items"),
            resources.GetString("cbLv_Grr.Items1"),
            resources.GetString("cbLv_Grr.Items2")});
			this.cbLv_Grr.Name = "cbLv_Grr";
			// 
			// label191
			// 
			this.label191.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label191, "label191");
			this.label191.ForeColor = System.Drawing.Color.Black;
			this.label191.Name = "label191";
			// 
			// cbLv_Grd
			// 
			this.cbLv_Grd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Grd, "cbLv_Grd");
			this.cbLv_Grd.FormattingEnabled = true;
			this.cbLv_Grd.Items.AddRange(new object[] {
            resources.GetString("cbLv_Grd.Items"),
            resources.GetString("cbLv_Grd.Items1"),
            resources.GetString("cbLv_Grd.Items2")});
			this.cbLv_Grd.Name = "cbLv_Grd";
			// 
			// label192
			// 
			this.label192.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label192, "label192");
			this.label192.ForeColor = System.Drawing.Color.Black;
			this.label192.Name = "label192";
			// 
			// cbLv_GrL
			// 
			this.cbLv_GrL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_GrL, "cbLv_GrL");
			this.cbLv_GrL.FormattingEnabled = true;
			this.cbLv_GrL.Items.AddRange(new object[] {
            resources.GetString("cbLv_GrL.Items"),
            resources.GetString("cbLv_GrL.Items1"),
            resources.GetString("cbLv_GrL.Items2")});
			this.cbLv_GrL.Name = "cbLv_GrL";
			// 
			// label193
			// 
			this.label193.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label193, "label193");
			this.label193.ForeColor = System.Drawing.Color.Black;
			this.label193.Name = "label193";
			// 
			// cbLv_Onp
			// 
			this.cbLv_Onp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Onp, "cbLv_Onp");
			this.cbLv_Onp.FormattingEnabled = true;
			this.cbLv_Onp.Items.AddRange(new object[] {
            resources.GetString("cbLv_Onp.Items"),
            resources.GetString("cbLv_Onp.Items1"),
            resources.GetString("cbLv_Onp.Items2")});
			this.cbLv_Onp.Name = "cbLv_Onp";
			// 
			// label195
			// 
			this.label195.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label195, "label195");
			this.label195.ForeColor = System.Drawing.Color.Black;
			this.label195.Name = "label195";
			// 
			// cbLv_Inr
			// 
			this.cbLv_Inr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_Inr, "cbLv_Inr");
			this.cbLv_Inr.FormattingEnabled = true;
			this.cbLv_Inr.Items.AddRange(new object[] {
            resources.GetString("cbLv_Inr.Items"),
            resources.GetString("cbLv_Inr.Items1"),
            resources.GetString("cbLv_Inr.Items2")});
			this.cbLv_Inr.Name = "cbLv_Inr";
			// 
			// label199
			// 
			this.label199.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label199, "label199");
			this.label199.ForeColor = System.Drawing.Color.Black;
			this.label199.Name = "label199";
			// 
			// cbLv_OnL
			// 
			this.cbLv_OnL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OnL, "cbLv_OnL");
			this.cbLv_OnL.FormattingEnabled = true;
			this.cbLv_OnL.Items.AddRange(new object[] {
            resources.GetString("cbLv_OnL.Items"),
            resources.GetString("cbLv_OnL.Items1"),
            resources.GetString("cbLv_OnL.Items2")});
			this.cbLv_OnL.Name = "cbLv_OnL";
			// 
			// label202
			// 
			this.label202.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label202, "label202");
			this.label202.ForeColor = System.Drawing.Color.Black;
			this.label202.Name = "label202";
			// 
			// tp3_Device
			// 
			this.tp3_Device.BackColor = System.Drawing.Color.White;
			this.tp3_Device.Controls.Add(this.cbLv_DvPosView);
			this.tp3_Device.Controls.Add(this.label286);
			this.tp3_Device.Controls.Add(this.cbLv_DvParaEnable);
			this.tp3_Device.Controls.Add(this.label287);
			this.tp3_Device.Controls.Add(this.cbLv_DvSave);
			this.tp3_Device.Controls.Add(this.label207);
			this.tp3_Device.Controls.Add(this.cbLv_DvCurrent);
			this.tp3_Device.Controls.Add(this.label208);
			this.tp3_Device.Controls.Add(this.cbLv_DvLoad);
			this.tp3_Device.Controls.Add(this.label209);
			this.tp3_Device.Controls.Add(this.cbLv_DvDel);
			this.tp3_Device.Controls.Add(this.label211);
			this.tp3_Device.Controls.Add(this.cbLv_DvSaveAs);
			this.tp3_Device.Controls.Add(this.label212);
			this.tp3_Device.Controls.Add(this.cbLv_DvNew);
			this.tp3_Device.Controls.Add(this.label214);
			this.tp3_Device.Controls.Add(this.cbLv_GpDel);
			this.tp3_Device.Controls.Add(this.label215);
			this.tp3_Device.Controls.Add(this.cbLv_GpSaveAs);
			this.tp3_Device.Controls.Add(this.label217);
			this.tp3_Device.Controls.Add(this.cbLv_GpNew);
			this.tp3_Device.Controls.Add(this.label218);
			resources.ApplyResources(this.tp3_Device, "tp3_Device");
			this.tp3_Device.Name = "tp3_Device";
			// 
			// cbLv_DvPosView
			// 
			this.cbLv_DvPosView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvPosView, "cbLv_DvPosView");
			this.cbLv_DvPosView.FormattingEnabled = true;
			this.cbLv_DvPosView.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvPosView.Items"),
            resources.GetString("cbLv_DvPosView.Items1"),
            resources.GetString("cbLv_DvPosView.Items2")});
			this.cbLv_DvPosView.Name = "cbLv_DvPosView";
			// 
			// label286
			// 
			this.label286.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label286, "label286");
			this.label286.ForeColor = System.Drawing.Color.Black;
			this.label286.Name = "label286";
			// 
			// cbLv_DvParaEnable
			// 
			this.cbLv_DvParaEnable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvParaEnable, "cbLv_DvParaEnable");
			this.cbLv_DvParaEnable.FormattingEnabled = true;
			this.cbLv_DvParaEnable.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvParaEnable.Items"),
            resources.GetString("cbLv_DvParaEnable.Items1"),
            resources.GetString("cbLv_DvParaEnable.Items2")});
			this.cbLv_DvParaEnable.Name = "cbLv_DvParaEnable";
			// 
			// label287
			// 
			this.label287.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label287, "label287");
			this.label287.ForeColor = System.Drawing.Color.Black;
			this.label287.Name = "label287";
			// 
			// cbLv_DvSave
			// 
			this.cbLv_DvSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvSave, "cbLv_DvSave");
			this.cbLv_DvSave.FormattingEnabled = true;
			this.cbLv_DvSave.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvSave.Items"),
            resources.GetString("cbLv_DvSave.Items1"),
            resources.GetString("cbLv_DvSave.Items2")});
			this.cbLv_DvSave.Name = "cbLv_DvSave";
			// 
			// label207
			// 
			this.label207.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label207, "label207");
			this.label207.ForeColor = System.Drawing.Color.Black;
			this.label207.Name = "label207";
			// 
			// cbLv_DvCurrent
			// 
			this.cbLv_DvCurrent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvCurrent, "cbLv_DvCurrent");
			this.cbLv_DvCurrent.FormattingEnabled = true;
			this.cbLv_DvCurrent.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvCurrent.Items"),
            resources.GetString("cbLv_DvCurrent.Items1"),
            resources.GetString("cbLv_DvCurrent.Items2")});
			this.cbLv_DvCurrent.Name = "cbLv_DvCurrent";
			// 
			// label208
			// 
			this.label208.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label208, "label208");
			this.label208.ForeColor = System.Drawing.Color.Black;
			this.label208.Name = "label208";
			// 
			// cbLv_DvLoad
			// 
			this.cbLv_DvLoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvLoad, "cbLv_DvLoad");
			this.cbLv_DvLoad.FormattingEnabled = true;
			this.cbLv_DvLoad.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvLoad.Items"),
            resources.GetString("cbLv_DvLoad.Items1"),
            resources.GetString("cbLv_DvLoad.Items2")});
			this.cbLv_DvLoad.Name = "cbLv_DvLoad";
			// 
			// label209
			// 
			this.label209.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label209, "label209");
			this.label209.ForeColor = System.Drawing.Color.Black;
			this.label209.Name = "label209";
			// 
			// cbLv_DvDel
			// 
			this.cbLv_DvDel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvDel, "cbLv_DvDel");
			this.cbLv_DvDel.FormattingEnabled = true;
			this.cbLv_DvDel.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvDel.Items"),
            resources.GetString("cbLv_DvDel.Items1"),
            resources.GetString("cbLv_DvDel.Items2")});
			this.cbLv_DvDel.Name = "cbLv_DvDel";
			// 
			// label211
			// 
			this.label211.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label211, "label211");
			this.label211.ForeColor = System.Drawing.Color.Black;
			this.label211.Name = "label211";
			// 
			// cbLv_DvSaveAs
			// 
			this.cbLv_DvSaveAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvSaveAs, "cbLv_DvSaveAs");
			this.cbLv_DvSaveAs.FormattingEnabled = true;
			this.cbLv_DvSaveAs.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvSaveAs.Items"),
            resources.GetString("cbLv_DvSaveAs.Items1"),
            resources.GetString("cbLv_DvSaveAs.Items2")});
			this.cbLv_DvSaveAs.Name = "cbLv_DvSaveAs";
			// 
			// label212
			// 
			this.label212.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label212, "label212");
			this.label212.ForeColor = System.Drawing.Color.Black;
			this.label212.Name = "label212";
			// 
			// cbLv_DvNew
			// 
			this.cbLv_DvNew.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_DvNew, "cbLv_DvNew");
			this.cbLv_DvNew.FormattingEnabled = true;
			this.cbLv_DvNew.Items.AddRange(new object[] {
            resources.GetString("cbLv_DvNew.Items"),
            resources.GetString("cbLv_DvNew.Items1"),
            resources.GetString("cbLv_DvNew.Items2")});
			this.cbLv_DvNew.Name = "cbLv_DvNew";
			// 
			// label214
			// 
			this.label214.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label214, "label214");
			this.label214.ForeColor = System.Drawing.Color.Black;
			this.label214.Name = "label214";
			// 
			// cbLv_GpDel
			// 
			this.cbLv_GpDel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_GpDel, "cbLv_GpDel");
			this.cbLv_GpDel.FormattingEnabled = true;
			this.cbLv_GpDel.Items.AddRange(new object[] {
            resources.GetString("cbLv_GpDel.Items"),
            resources.GetString("cbLv_GpDel.Items1"),
            resources.GetString("cbLv_GpDel.Items2")});
			this.cbLv_GpDel.Name = "cbLv_GpDel";
			// 
			// label215
			// 
			this.label215.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label215, "label215");
			this.label215.ForeColor = System.Drawing.Color.Black;
			this.label215.Name = "label215";
			// 
			// cbLv_GpSaveAs
			// 
			this.cbLv_GpSaveAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_GpSaveAs, "cbLv_GpSaveAs");
			this.cbLv_GpSaveAs.FormattingEnabled = true;
			this.cbLv_GpSaveAs.Items.AddRange(new object[] {
            resources.GetString("cbLv_GpSaveAs.Items"),
            resources.GetString("cbLv_GpSaveAs.Items1"),
            resources.GetString("cbLv_GpSaveAs.Items2")});
			this.cbLv_GpSaveAs.Name = "cbLv_GpSaveAs";
			// 
			// label217
			// 
			this.label217.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label217, "label217");
			this.label217.ForeColor = System.Drawing.Color.Black;
			this.label217.Name = "label217";
			// 
			// cbLv_GpNew
			// 
			this.cbLv_GpNew.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_GpNew, "cbLv_GpNew");
			this.cbLv_GpNew.FormattingEnabled = true;
			this.cbLv_GpNew.Items.AddRange(new object[] {
            resources.GetString("cbLv_GpNew.Items"),
            resources.GetString("cbLv_GpNew.Items1"),
            resources.GetString("cbLv_GpNew.Items2")});
			this.cbLv_GpNew.Name = "cbLv_GpNew";
			// 
			// label218
			// 
			this.label218.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label218, "label218");
			this.label218.ForeColor = System.Drawing.Color.Black;
			this.label218.Name = "label218";
			// 
			// tp5_Spc
			// 
			this.tp5_Spc.BackColor = System.Drawing.Color.White;
			this.tp5_Spc.Controls.Add(this.cbLv_SpcErrHisSave);
			this.tp5_Spc.Controls.Add(this.label230);
			this.tp5_Spc.Controls.Add(this.cbLv_SpcErrHisView);
			this.tp5_Spc.Controls.Add(this.label229);
			this.tp5_Spc.Controls.Add(this.cbLv_SpcGpSave);
			this.tp5_Spc.Controls.Add(this.label223);
			this.tp5_Spc.Controls.Add(this.cbLv_SpcErrHis);
			this.tp5_Spc.Controls.Add(this.label222);
			this.tp5_Spc.Controls.Add(this.cbLv_SpcErrList);
			this.tp5_Spc.Controls.Add(this.label220);
			resources.ApplyResources(this.tp5_Spc, "tp5_Spc");
			this.tp5_Spc.Name = "tp5_Spc";
			// 
			// cbLv_SpcErrHisSave
			// 
			this.cbLv_SpcErrHisSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_SpcErrHisSave, "cbLv_SpcErrHisSave");
			this.cbLv_SpcErrHisSave.FormattingEnabled = true;
			this.cbLv_SpcErrHisSave.Items.AddRange(new object[] {
            resources.GetString("cbLv_SpcErrHisSave.Items"),
            resources.GetString("cbLv_SpcErrHisSave.Items1"),
            resources.GetString("cbLv_SpcErrHisSave.Items2")});
			this.cbLv_SpcErrHisSave.Name = "cbLv_SpcErrHisSave";
			// 
			// label230
			// 
			this.label230.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label230, "label230");
			this.label230.ForeColor = System.Drawing.Color.Black;
			this.label230.Name = "label230";
			// 
			// cbLv_SpcErrHisView
			// 
			this.cbLv_SpcErrHisView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_SpcErrHisView, "cbLv_SpcErrHisView");
			this.cbLv_SpcErrHisView.FormattingEnabled = true;
			this.cbLv_SpcErrHisView.Items.AddRange(new object[] {
            resources.GetString("cbLv_SpcErrHisView.Items"),
            resources.GetString("cbLv_SpcErrHisView.Items1"),
            resources.GetString("cbLv_SpcErrHisView.Items2")});
			this.cbLv_SpcErrHisView.Name = "cbLv_SpcErrHisView";
			// 
			// label229
			// 
			this.label229.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label229, "label229");
			this.label229.ForeColor = System.Drawing.Color.Black;
			this.label229.Name = "label229";
			// 
			// cbLv_SpcGpSave
			// 
			this.cbLv_SpcGpSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_SpcGpSave, "cbLv_SpcGpSave");
			this.cbLv_SpcGpSave.FormattingEnabled = true;
			this.cbLv_SpcGpSave.Items.AddRange(new object[] {
            resources.GetString("cbLv_SpcGpSave.Items"),
            resources.GetString("cbLv_SpcGpSave.Items1"),
            resources.GetString("cbLv_SpcGpSave.Items2")});
			this.cbLv_SpcGpSave.Name = "cbLv_SpcGpSave";
			// 
			// label223
			// 
			this.label223.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label223, "label223");
			this.label223.ForeColor = System.Drawing.Color.Black;
			this.label223.Name = "label223";
			// 
			// cbLv_SpcErrHis
			// 
			this.cbLv_SpcErrHis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_SpcErrHis, "cbLv_SpcErrHis");
			this.cbLv_SpcErrHis.FormattingEnabled = true;
			this.cbLv_SpcErrHis.Items.AddRange(new object[] {
            resources.GetString("cbLv_SpcErrHis.Items"),
            resources.GetString("cbLv_SpcErrHis.Items1"),
            resources.GetString("cbLv_SpcErrHis.Items2")});
			this.cbLv_SpcErrHis.Name = "cbLv_SpcErrHis";
			// 
			// label222
			// 
			this.label222.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label222, "label222");
			this.label222.ForeColor = System.Drawing.Color.Black;
			this.label222.Name = "label222";
			// 
			// cbLv_SpcErrList
			// 
			this.cbLv_SpcErrList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_SpcErrList, "cbLv_SpcErrList");
			this.cbLv_SpcErrList.FormattingEnabled = true;
			this.cbLv_SpcErrList.Items.AddRange(new object[] {
            resources.GetString("cbLv_SpcErrList.Items"),
            resources.GetString("cbLv_SpcErrList.Items1"),
            resources.GetString("cbLv_SpcErrList.Items2")});
			this.cbLv_SpcErrList.Name = "cbLv_SpcErrList";
			// 
			// label220
			// 
			this.label220.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label220, "label220");
			this.label220.ForeColor = System.Drawing.Color.Black;
			this.label220.Name = "label220";
			// 
			// tp6_Option
			// 
			this.tp6_Option.BackColor = System.Drawing.Color.White;
			this.tp6_Option.Controls.Add(this.cbLv_OptGeneral);
			this.tp6_Option.Controls.Add(this.label7);
			this.tp6_Option.Controls.Add(this.cbLv_OptMnt);
			this.tp6_Option.Controls.Add(this.label6);
			this.tp6_Option.Controls.Add(this.cbLv_OptTbGrd);
			this.tp6_Option.Controls.Add(this.label221);
			this.tp6_Option.Controls.Add(this.cbLv_OptGrind);
			this.tp6_Option.Controls.Add(this.cbLv_OptPicker);
			this.tp6_Option.Controls.Add(this.cbLv_OptRailDry);
			this.tp6_Option.Controls.Add(this.cbLv_OptLoader);
			this.tp6_Option.Controls.Add(this.label5);
			this.tp6_Option.Controls.Add(this.label4);
			this.tp6_Option.Controls.Add(this.label3);
			this.tp6_Option.Controls.Add(this.label2);
			this.tp6_Option.Controls.Add(this.cbLv_OptSysPos);
			this.tp6_Option.Controls.Add(this.label231);
			resources.ApplyResources(this.tp6_Option, "tp6_Option");
			this.tp6_Option.Name = "tp6_Option";
			// 
			// cbLv_OptTbGrd
			// 
			this.cbLv_OptTbGrd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptTbGrd, "cbLv_OptTbGrd");
			this.cbLv_OptTbGrd.FormattingEnabled = true;
			this.cbLv_OptTbGrd.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptTbGrd.Items"),
            resources.GetString("cbLv_OptTbGrd.Items1"),
            resources.GetString("cbLv_OptTbGrd.Items2")});
			this.cbLv_OptTbGrd.Name = "cbLv_OptTbGrd";
			// 
			// label221
			// 
			this.label221.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label221, "label221");
			this.label221.ForeColor = System.Drawing.Color.Black;
			this.label221.Name = "label221";
			// 
			// cbLv_OptGrind
			// 
			this.cbLv_OptGrind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptGrind, "cbLv_OptGrind");
			this.cbLv_OptGrind.FormattingEnabled = true;
			this.cbLv_OptGrind.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptGrind.Items"),
            resources.GetString("cbLv_OptGrind.Items1"),
            resources.GetString("cbLv_OptGrind.Items2")});
			this.cbLv_OptGrind.Name = "cbLv_OptGrind";
			// 
			// cbLv_OptPicker
			// 
			this.cbLv_OptPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptPicker, "cbLv_OptPicker");
			this.cbLv_OptPicker.FormattingEnabled = true;
			this.cbLv_OptPicker.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptPicker.Items"),
            resources.GetString("cbLv_OptPicker.Items1"),
            resources.GetString("cbLv_OptPicker.Items2")});
			this.cbLv_OptPicker.Name = "cbLv_OptPicker";
			// 
			// cbLv_OptRailDry
			// 
			this.cbLv_OptRailDry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptRailDry, "cbLv_OptRailDry");
			this.cbLv_OptRailDry.FormattingEnabled = true;
			this.cbLv_OptRailDry.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptRailDry.Items"),
            resources.GetString("cbLv_OptRailDry.Items1"),
            resources.GetString("cbLv_OptRailDry.Items2")});
			this.cbLv_OptRailDry.Name = "cbLv_OptRailDry";
			// 
			// cbLv_OptLoader
			// 
			this.cbLv_OptLoader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptLoader, "cbLv_OptLoader");
			this.cbLv_OptLoader.FormattingEnabled = true;
			this.cbLv_OptLoader.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptLoader.Items"),
            resources.GetString("cbLv_OptLoader.Items1"),
            resources.GetString("cbLv_OptLoader.Items2")});
			this.cbLv_OptLoader.Name = "cbLv_OptLoader";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label5, "label5");
			this.label5.ForeColor = System.Drawing.Color.Black;
			this.label5.Name = "label5";
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label4, "label4");
			this.label4.ForeColor = System.Drawing.Color.Black;
			this.label4.Name = "label4";
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Name = "label3";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Name = "label2";
			// 
			// cbLv_OptSysPos
			// 
			this.cbLv_OptSysPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptSysPos, "cbLv_OptSysPos");
			this.cbLv_OptSysPos.FormattingEnabled = true;
			this.cbLv_OptSysPos.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptSysPos.Items"),
            resources.GetString("cbLv_OptSysPos.Items1"),
            resources.GetString("cbLv_OptSysPos.Items2")});
			this.cbLv_OptSysPos.Name = "cbLv_OptSysPos";
			// 
			// label231
			// 
			this.label231.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label231, "label231");
			this.label231.ForeColor = System.Drawing.Color.Black;
			this.label231.Name = "label231";
			// 
			// tp7_Util
			// 
			this.tp7_Util.BackColor = System.Drawing.Color.White;
			this.tp7_Util.Controls.Add(this.cbLv_UtilRepeat);
			this.tp7_Util.Controls.Add(this.label240);
			this.tp7_Util.Controls.Add(this.cbLv_UtilBcr);
			this.tp7_Util.Controls.Add(this.label233);
			this.tp7_Util.Controls.Add(this.cbLv_UtilTw);
			this.tp7_Util.Controls.Add(this.label234);
			this.tp7_Util.Controls.Add(this.cbLv_UtilPrb);
			this.tp7_Util.Controls.Add(this.label235);
			this.tp7_Util.Controls.Add(this.cbLv_UtilOut);
			this.tp7_Util.Controls.Add(this.label236);
			this.tp7_Util.Controls.Add(this.cbLv_UtilIn);
			this.tp7_Util.Controls.Add(this.label237);
			this.tp7_Util.Controls.Add(this.cbLv_UtilSpd);
			this.tp7_Util.Controls.Add(this.label238);
			this.tp7_Util.Controls.Add(this.cbLv_UtilMot);
			this.tp7_Util.Controls.Add(this.label239);
			resources.ApplyResources(this.tp7_Util, "tp7_Util");
			this.tp7_Util.Name = "tp7_Util";
			// 
			// cbLv_UtilRepeat
			// 
			this.cbLv_UtilRepeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilRepeat, "cbLv_UtilRepeat");
			this.cbLv_UtilRepeat.FormattingEnabled = true;
			this.cbLv_UtilRepeat.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilRepeat.Items"),
            resources.GetString("cbLv_UtilRepeat.Items1"),
            resources.GetString("cbLv_UtilRepeat.Items2")});
			this.cbLv_UtilRepeat.Name = "cbLv_UtilRepeat";
			// 
			// label240
			// 
			this.label240.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label240, "label240");
			this.label240.ForeColor = System.Drawing.Color.Black;
			this.label240.Name = "label240";
			// 
			// cbLv_UtilBcr
			// 
			this.cbLv_UtilBcr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilBcr, "cbLv_UtilBcr");
			this.cbLv_UtilBcr.FormattingEnabled = true;
			this.cbLv_UtilBcr.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilBcr.Items"),
            resources.GetString("cbLv_UtilBcr.Items1"),
            resources.GetString("cbLv_UtilBcr.Items2")});
			this.cbLv_UtilBcr.Name = "cbLv_UtilBcr";
			// 
			// label233
			// 
			this.label233.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label233, "label233");
			this.label233.ForeColor = System.Drawing.Color.Black;
			this.label233.Name = "label233";
			// 
			// cbLv_UtilTw
			// 
			this.cbLv_UtilTw.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilTw, "cbLv_UtilTw");
			this.cbLv_UtilTw.FormattingEnabled = true;
			this.cbLv_UtilTw.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilTw.Items"),
            resources.GetString("cbLv_UtilTw.Items1"),
            resources.GetString("cbLv_UtilTw.Items2")});
			this.cbLv_UtilTw.Name = "cbLv_UtilTw";
			// 
			// label234
			// 
			this.label234.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label234, "label234");
			this.label234.ForeColor = System.Drawing.Color.Black;
			this.label234.Name = "label234";
			// 
			// cbLv_UtilPrb
			// 
			this.cbLv_UtilPrb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilPrb, "cbLv_UtilPrb");
			this.cbLv_UtilPrb.FormattingEnabled = true;
			this.cbLv_UtilPrb.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilPrb.Items"),
            resources.GetString("cbLv_UtilPrb.Items1"),
            resources.GetString("cbLv_UtilPrb.Items2")});
			this.cbLv_UtilPrb.Name = "cbLv_UtilPrb";
			// 
			// label235
			// 
			this.label235.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label235, "label235");
			this.label235.ForeColor = System.Drawing.Color.Black;
			this.label235.Name = "label235";
			// 
			// cbLv_UtilOut
			// 
			this.cbLv_UtilOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilOut, "cbLv_UtilOut");
			this.cbLv_UtilOut.FormattingEnabled = true;
			this.cbLv_UtilOut.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilOut.Items"),
            resources.GetString("cbLv_UtilOut.Items1"),
            resources.GetString("cbLv_UtilOut.Items2")});
			this.cbLv_UtilOut.Name = "cbLv_UtilOut";
			// 
			// label236
			// 
			this.label236.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label236, "label236");
			this.label236.ForeColor = System.Drawing.Color.Black;
			this.label236.Name = "label236";
			// 
			// cbLv_UtilIn
			// 
			this.cbLv_UtilIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilIn, "cbLv_UtilIn");
			this.cbLv_UtilIn.FormattingEnabled = true;
			this.cbLv_UtilIn.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilIn.Items"),
            resources.GetString("cbLv_UtilIn.Items1"),
            resources.GetString("cbLv_UtilIn.Items2")});
			this.cbLv_UtilIn.Name = "cbLv_UtilIn";
			// 
			// label237
			// 
			this.label237.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label237, "label237");
			this.label237.ForeColor = System.Drawing.Color.Black;
			this.label237.Name = "label237";
			// 
			// cbLv_UtilSpd
			// 
			this.cbLv_UtilSpd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilSpd, "cbLv_UtilSpd");
			this.cbLv_UtilSpd.FormattingEnabled = true;
			this.cbLv_UtilSpd.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilSpd.Items"),
            resources.GetString("cbLv_UtilSpd.Items1"),
            resources.GetString("cbLv_UtilSpd.Items2")});
			this.cbLv_UtilSpd.Name = "cbLv_UtilSpd";
			// 
			// label238
			// 
			this.label238.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label238, "label238");
			this.label238.ForeColor = System.Drawing.Color.Black;
			this.label238.Name = "label238";
			// 
			// cbLv_UtilMot
			// 
			this.cbLv_UtilMot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_UtilMot, "cbLv_UtilMot");
			this.cbLv_UtilMot.FormattingEnabled = true;
			this.cbLv_UtilMot.Items.AddRange(new object[] {
            resources.GetString("cbLv_UtilMot.Items"),
            resources.GetString("cbLv_UtilMot.Items1"),
            resources.GetString("cbLv_UtilMot.Items2")});
			this.cbLv_UtilMot.Name = "cbLv_UtilMot";
			// 
			// label239
			// 
			this.label239.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label239, "label239");
			this.label239.ForeColor = System.Drawing.Color.Black;
			this.label239.Name = "label239";
			// 
			// tp8_OpManual
			// 
			this.tp8_OpManual.BackColor = System.Drawing.Color.White;
			this.tp8_OpManual.Controls.Add(this.cbLv_OpStripExistEdit);
			this.tp8_OpManual.Controls.Add(this.label1);
			this.tp8_OpManual.Controls.Add(this.cbLv_OpDrsPos);
			this.tp8_OpManual.Controls.Add(this.label290);
			resources.ApplyResources(this.tp8_OpManual, "tp8_OpManual");
			this.tp8_OpManual.Name = "tp8_OpManual";
			// 
			// cbLv_OpStripExistEdit
			// 
			this.cbLv_OpStripExistEdit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OpStripExistEdit, "cbLv_OpStripExistEdit");
			this.cbLv_OpStripExistEdit.FormattingEnabled = true;
			this.cbLv_OpStripExistEdit.Items.AddRange(new object[] {
            resources.GetString("cbLv_OpStripExistEdit.Items"),
            resources.GetString("cbLv_OpStripExistEdit.Items1"),
            resources.GetString("cbLv_OpStripExistEdit.Items2")});
			this.cbLv_OpStripExistEdit.Name = "cbLv_OpStripExistEdit";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Name = "label1";
			// 
			// cbLv_OpDrsPos
			// 
			this.cbLv_OpDrsPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OpDrsPos, "cbLv_OpDrsPos");
			this.cbLv_OpDrsPos.FormattingEnabled = true;
			this.cbLv_OpDrsPos.Items.AddRange(new object[] {
            resources.GetString("cbLv_OpDrsPos.Items"),
            resources.GetString("cbLv_OpDrsPos.Items1"),
            resources.GetString("cbLv_OpDrsPos.Items2")});
			this.cbLv_OpDrsPos.Name = "cbLv_OpDrsPos";
			// 
			// label290
			// 
			this.label290.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label290, "label290");
			this.label290.ForeColor = System.Drawing.Color.Black;
			this.label290.Name = "label290";
			// 
			// cbLv_OptMnt
			// 
			this.cbLv_OptMnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptMnt, "cbLv_OptMnt");
			this.cbLv_OptMnt.FormattingEnabled = true;
			this.cbLv_OptMnt.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptMnt.Items"),
            resources.GetString("cbLv_OptMnt.Items1"),
            resources.GetString("cbLv_OptMnt.Items2")});
			this.cbLv_OptMnt.Name = "cbLv_OptMnt";
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label6, "label6");
			this.label6.ForeColor = System.Drawing.Color.Black;
			this.label6.Name = "label6";
			// 
			// cbLv_OptGeneral
			// 
			this.cbLv_OptGeneral.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLv_OptGeneral, "cbLv_OptGeneral");
			this.cbLv_OptGeneral.FormattingEnabled = true;
			this.cbLv_OptGeneral.Items.AddRange(new object[] {
            resources.GetString("cbLv_OptGeneral.Items"),
            resources.GetString("cbLv_OptGeneral.Items1"),
            resources.GetString("cbLv_OptGeneral.Items2")});
			this.cbLv_OptGeneral.Name = "cbLv_OptGeneral";
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.Color.WhiteSmoke;
			resources.ApplyResources(this.label7, "label7");
			this.label7.ForeColor = System.Drawing.Color.Black;
			this.label7.Name = "label7";
			// 
			// vwOpt_06Lvs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.tap_Level);
			this.DoubleBuffered = true;
			resources.ApplyResources(this, "$this");
			this.ForeColor = System.Drawing.Color.Black;
			this.Name = "vwOpt_06Lvs";
			tp4_Wheel.ResumeLayout(false);
			this.tap_Level.ResumeLayout(false);
			this.tp1_Auto.ResumeLayout(false);
			this.tp2_Manual.ResumeLayout(false);
			this.tp3_Device.ResumeLayout(false);
			this.tp5_Spc.ResumeLayout(false);
			this.tp6_Option.ResumeLayout(false);
			this.tp7_Util.ResumeLayout(false);
			this.tp8_OpManual.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tap_Level;
        private System.Windows.Forms.TabPage tp1_Auto;
        private System.Windows.Forms.ComboBox cbLv_Exit;
        private System.Windows.Forms.Label label184;
        private System.Windows.Forms.ComboBox cbLv_Util;
        private System.Windows.Forms.Label label185;
        private System.Windows.Forms.ComboBox cbLv_Option;
        private System.Windows.Forms.Label label188;
        private System.Windows.Forms.ComboBox cbLv_Spc;
        private System.Windows.Forms.Label label190;
        private System.Windows.Forms.ComboBox cbLv_Wheel;
        private System.Windows.Forms.Label label183;
        private System.Windows.Forms.ComboBox cbLv_Device;
        private System.Windows.Forms.Label label182;
        private System.Windows.Forms.Label label181;
        private System.Windows.Forms.TabPage tp2_Manual;
        private System.Windows.Forms.ComboBox cbLv_AllSvOff;
        private System.Windows.Forms.Label label288;
        private System.Windows.Forms.ComboBox cbLv_AllSvOn;
        private System.Windows.Forms.Label label289;
        private System.Windows.Forms.ComboBox cbLv_WarmSet;
        private System.Windows.Forms.Label label206;
        private System.Windows.Forms.ComboBox cbLv_OfL;
        private System.Windows.Forms.Label label203;
        private System.Windows.Forms.ComboBox cbLv_Dry;
        private System.Windows.Forms.Label label205;
        private System.Windows.Forms.ComboBox cbLv_Ofp;
        private System.Windows.Forms.Label label179;
        private System.Windows.Forms.ComboBox cbLv_Grr;
        private System.Windows.Forms.Label label191;
        private System.Windows.Forms.ComboBox cbLv_Grd;
        private System.Windows.Forms.Label label192;
        private System.Windows.Forms.ComboBox cbLv_GrL;
        private System.Windows.Forms.Label label193;
        private System.Windows.Forms.ComboBox cbLv_Onp;
        private System.Windows.Forms.Label label195;
        private System.Windows.Forms.ComboBox cbLv_Inr;
        private System.Windows.Forms.Label label199;
        private System.Windows.Forms.ComboBox cbLv_OnL;
        private System.Windows.Forms.Label label202;
        private System.Windows.Forms.TabPage tp3_Device;
        private System.Windows.Forms.ComboBox cbLv_DvPosView;
        private System.Windows.Forms.Label label286;
        private System.Windows.Forms.ComboBox cbLv_DvParaEnable;
        private System.Windows.Forms.Label label287;
        private System.Windows.Forms.ComboBox cbLv_DvSave;
        private System.Windows.Forms.Label label207;
        private System.Windows.Forms.ComboBox cbLv_DvCurrent;
        private System.Windows.Forms.Label label208;
        private System.Windows.Forms.ComboBox cbLv_DvLoad;
        private System.Windows.Forms.Label label209;
        private System.Windows.Forms.ComboBox cbLv_DvDel;
        private System.Windows.Forms.Label label211;
        private System.Windows.Forms.ComboBox cbLv_DvSaveAs;
        private System.Windows.Forms.Label label212;
        private System.Windows.Forms.ComboBox cbLv_DvNew;
        private System.Windows.Forms.Label label214;
        private System.Windows.Forms.ComboBox cbLv_GpDel;
        private System.Windows.Forms.Label label215;
        private System.Windows.Forms.ComboBox cbLv_GpSaveAs;
        private System.Windows.Forms.Label label217;
        private System.Windows.Forms.ComboBox cbLv_GpNew;
        private System.Windows.Forms.Label label218;
        private System.Windows.Forms.ComboBox cbLv_WhlDrsChange;
        private System.Windows.Forms.ComboBox cbLv_WhlChange;
        private System.Windows.Forms.Label label264;
        private System.Windows.Forms.Label label258;
        private System.Windows.Forms.ComboBox cbLv_WhlSave;
        private System.Windows.Forms.Label label225;
        private System.Windows.Forms.ComboBox cbLv_WhlDel;
        private System.Windows.Forms.Label label226;
        private System.Windows.Forms.ComboBox cbLv_WhlSaveAs;
        private System.Windows.Forms.Label label227;
        private System.Windows.Forms.ComboBox cbLv_WhlNew;
        private System.Windows.Forms.Label label228;
        private System.Windows.Forms.TabPage tp5_Spc;
        private System.Windows.Forms.ComboBox cbLv_SpcErrHisSave;
        private System.Windows.Forms.Label label230;
        private System.Windows.Forms.ComboBox cbLv_SpcErrHisView;
        private System.Windows.Forms.Label label229;
        private System.Windows.Forms.ComboBox cbLv_SpcGpSave;
        private System.Windows.Forms.Label label223;
        private System.Windows.Forms.ComboBox cbLv_SpcErrHis;
        private System.Windows.Forms.Label label222;
        private System.Windows.Forms.ComboBox cbLv_SpcErrList;
        private System.Windows.Forms.Label label220;
        private System.Windows.Forms.TabPage tp6_Option;
        private System.Windows.Forms.ComboBox cbLv_OptTbGrd;
        private System.Windows.Forms.Label label221;
        private System.Windows.Forms.ComboBox cbLv_OptSysPos;
        private System.Windows.Forms.Label label231;
        private System.Windows.Forms.TabPage tp7_Util;
        private System.Windows.Forms.ComboBox cbLv_UtilRepeat;
        private System.Windows.Forms.Label label240;
        private System.Windows.Forms.ComboBox cbLv_UtilBcr;
        private System.Windows.Forms.Label label233;
        private System.Windows.Forms.ComboBox cbLv_UtilTw;
        private System.Windows.Forms.Label label234;
        private System.Windows.Forms.ComboBox cbLv_UtilPrb;
        private System.Windows.Forms.Label label235;
        private System.Windows.Forms.ComboBox cbLv_UtilOut;
        private System.Windows.Forms.Label label236;
        private System.Windows.Forms.ComboBox cbLv_UtilIn;
        private System.Windows.Forms.Label label237;
        private System.Windows.Forms.ComboBox cbLv_UtilSpd;
        private System.Windows.Forms.Label label238;
        private System.Windows.Forms.ComboBox cbLv_UtilMot;
        private System.Windows.Forms.Label label239;
        private System.Windows.Forms.TabPage tp8_OpManual;
        private System.Windows.Forms.ComboBox cbLv_OpDrsPos;
        private System.Windows.Forms.Label label290;
        private System.Windows.Forms.ComboBox cbLv_Manual;
        private System.Windows.Forms.ComboBox cbLv_OpStripExistEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLv_OptGrind;
        private System.Windows.Forms.ComboBox cbLv_OptPicker;
        private System.Windows.Forms.ComboBox cbLv_OptRailDry;
        private System.Windows.Forms.ComboBox cbLv_OptLoader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbLv_OptMnt;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cbLv_OptGeneral;
		private System.Windows.Forms.Label label7;
	}
}
