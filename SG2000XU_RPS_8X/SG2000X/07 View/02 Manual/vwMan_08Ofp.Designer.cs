namespace SG2000X
{
    partial class vwMan_08Ofp
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_08Ofp));
			this.panel13 = new System.Windows.Forms.Panel();
			this.lblOfp_ZPos = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.lblOfp_ZStat = new System.Windows.Forms.Label();
			this.panel16 = new System.Windows.Forms.Panel();
			this.lblOfp_XPos = new System.Windows.Forms.Label();
			this.label38 = new System.Windows.Forms.Label();
			this.lblOfp_XStat = new System.Windows.Forms.Label();
			this.ckbOfP_Drn = new System.Windows.Forms.CheckBox();
			this.lblOfP_LdCell = new System.Windows.Forms.Label();
			this.lblOfP_Vac = new System.Windows.Forms.Label();
			this.lblOfP_R90 = new System.Windows.Forms.Label();
			this.lblOfP_R0 = new System.Windows.Forms.Label();
			this.ckbOfP_R0 = new System.Windows.Forms.CheckBox();
			this.ckbOfP_Ejt = new System.Windows.Forms.CheckBox();
			this.btn_PickCl = new System.Windows.Forms.Button();
			this.btn_StripCl = new System.Windows.Forms.Button();
			this.btn_Place = new System.Windows.Forms.Button();
			this.btn_PickR = new System.Windows.Forms.Button();
			this.btn_PickL = new System.Windows.Forms.Button();
			this.ckbOfP_Vac = new System.Windows.Forms.CheckBox();
			this.btn_Stop = new System.Windows.Forms.Button();
			this.btn_H = new System.Windows.Forms.Button();
			this.btn_Wait = new System.Windows.Forms.Button();
			this.btn_PlaceR = new System.Windows.Forms.Button();
			this.tab_CtrlOFP = new System.Windows.Forms.TabControl();
			this.tab_PagePickerX = new System.Windows.Forms.TabPage();
			this.btn_PickerXDryPlace = new System.Windows.Forms.Button();
			this.btn_PickerXStripClnEd = new System.Windows.Forms.Button();
			this.btn_PickerXPickerClnEd = new System.Windows.Forms.Button();
			this.btn_PickerXStripClnSt = new System.Windows.Forms.Button();
			this.btn_PickerXPickerClnSt = new System.Windows.Forms.Button();
			this.btn_PickerXRightPick = new System.Windows.Forms.Button();
			this.btn_PickerXLeftPick = new System.Windows.Forms.Button();
			this.btn_PickerXWait = new System.Windows.Forms.Button();
			this.tab_PagePickerZ = new System.Windows.Forms.TabPage();
			this.btn_PickerZDryPlace = new System.Windows.Forms.Button();
			this.btn_PickerZClnStrip = new System.Windows.Forms.Button();
			this.btn_PickerZClnPicker = new System.Windows.Forms.Button();
			this.btn_PickerZTblPickPlace = new System.Windows.Forms.Button();
			this.btn_PickerZWait = new System.Windows.Forms.Button();
			this.lblOfP_IV2_Error = new System.Windows.Forms.Label();
			this.lblOfP_IV2_Busy = new System.Windows.Forms.Label();
			this.lblOfP_IV2_OK = new System.Windows.Forms.Label();
			this.btn_CoverDryer = new System.Windows.Forms.Button();
			this.btn_IV_DryClamp = new System.Windows.Forms.Button();
			this.lblOfP_CarrierDetect = new System.Windows.Forms.Label();
			this.lblOfP_ClampClose = new System.Windows.Forms.Label();
			this.lblOfP_ClampOpen = new System.Windows.Forms.Label();
			this.btnOfP_Cover_Place = new System.Windows.Forms.Button();
			this.btnOfP_Cover_Pick = new System.Windows.Forms.Button();
			this.btnOfP_Cover_Check_IV2 = new System.Windows.Forms.Button();
			this.btnOfP_Unit_Check_IV2 = new System.Windows.Forms.Button();
			this.ckbOfP_Clamp = new System.Windows.Forms.CheckBox();
			this.lblOfP_VacFree = new System.Windows.Forms.Label();
			this.btn_IV_DryTable = new System.Windows.Forms.Button();
			this.ckbOfP_IV_Trigger = new System.Windows.Forms.CheckBox();
			this.btn_BtmClnBrush = new System.Windows.Forms.Button();
			this.btn_StripNzlCyl_N1 = new System.Windows.Forms.Button();
			this.btn_StripNzlCyl_N2 = new System.Windows.Forms.Button();
			this.panel13.SuspendLayout();
			this.panel16.SuspendLayout();
			this.tab_CtrlOFP.SuspendLayout();
			this.tab_PagePickerX.SuspendLayout();
			this.tab_PagePickerZ.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel13
			// 
			this.panel13.BackColor = System.Drawing.Color.LightGray;
			this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel13.Controls.Add(this.lblOfp_ZPos);
			this.panel13.Controls.Add(this.label22);
			this.panel13.Controls.Add(this.lblOfp_ZStat);
			resources.ApplyResources(this.panel13, "panel13");
			this.panel13.Name = "panel13";
			this.panel13.Tag = "0";
			// 
			// lblOfp_ZPos
			// 
			this.lblOfp_ZPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblOfp_ZPos, "lblOfp_ZPos");
			this.lblOfp_ZPos.Name = "lblOfp_ZPos";
			// 
			// label22
			// 
			resources.ApplyResources(this.label22, "label22");
			this.label22.Name = "label22";
			// 
			// lblOfp_ZStat
			// 
			this.lblOfp_ZStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lblOfp_ZStat, "lblOfp_ZStat");
			this.lblOfp_ZStat.Name = "lblOfp_ZStat";
			// 
			// panel16
			// 
			this.panel16.BackColor = System.Drawing.Color.LightGray;
			this.panel16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel16.Controls.Add(this.lblOfp_XPos);
			this.panel16.Controls.Add(this.label38);
			this.panel16.Controls.Add(this.lblOfp_XStat);
			resources.ApplyResources(this.panel16, "panel16");
			this.panel16.Name = "panel16";
			this.panel16.Tag = "0";
			// 
			// lblOfp_XPos
			// 
			this.lblOfp_XPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblOfp_XPos, "lblOfp_XPos");
			this.lblOfp_XPos.Name = "lblOfp_XPos";
			// 
			// label38
			// 
			resources.ApplyResources(this.label38, "label38");
			this.label38.Name = "label38";
			// 
			// lblOfp_XStat
			// 
			this.lblOfp_XStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lblOfp_XStat, "lblOfp_XStat");
			this.lblOfp_XStat.Name = "lblOfp_XStat";
			// 
			// ckbOfP_Drn
			// 
			resources.ApplyResources(this.ckbOfP_Drn, "ckbOfP_Drn");
			this.ckbOfP_Drn.BackColor = System.Drawing.Color.LightGray;
			this.ckbOfP_Drn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOfP_Drn.FlatAppearance.BorderSize = 2;
			this.ckbOfP_Drn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOfP_Drn.ForeColor = System.Drawing.Color.Black;
			this.ckbOfP_Drn.Name = "ckbOfP_Drn";
			this.ckbOfP_Drn.Tag = "2";
			this.ckbOfP_Drn.UseVisualStyleBackColor = false;
			this.ckbOfP_Drn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOfP_Drn.Click += new System.EventHandler(this.ckb_Click);
			// 
			// lblOfP_LdCell
			// 
			this.lblOfP_LdCell.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_LdCell.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_LdCell, "lblOfP_LdCell");
			this.lblOfP_LdCell.Name = "lblOfP_LdCell";
			this.lblOfP_LdCell.Tag = "Load Cell$";
			this.lblOfP_LdCell.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOfP_Vac
			// 
			this.lblOfP_Vac.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_Vac.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_Vac, "lblOfP_Vac");
			this.lblOfP_Vac.Name = "lblOfP_Vac";
			this.lblOfP_Vac.Tag = "Vacuum$";
			this.lblOfP_Vac.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOfP_R90
			// 
			this.lblOfP_R90.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_R90.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_R90, "lblOfP_R90");
			this.lblOfP_R90.Name = "lblOfP_R90";
			this.lblOfP_R90.Tag = "Rotate 90º$";
			this.lblOfP_R90.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOfP_R0
			// 
			this.lblOfP_R0.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_R0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_R0, "lblOfP_R0");
			this.lblOfP_R0.Name = "lblOfP_R0";
			this.lblOfP_R0.Tag = "Rotate 0º$";
			this.lblOfP_R0.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// ckbOfP_R0
			// 
			resources.ApplyResources(this.ckbOfP_R0, "ckbOfP_R0");
			this.ckbOfP_R0.BackColor = System.Drawing.Color.LightGray;
			this.ckbOfP_R0.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOfP_R0.FlatAppearance.BorderSize = 2;
			this.ckbOfP_R0.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOfP_R0.ForeColor = System.Drawing.Color.Black;
			this.ckbOfP_R0.Name = "ckbOfP_R0";
			this.ckbOfP_R0.Tag = "3";
			this.ckbOfP_R0.UseVisualStyleBackColor = false;
			this.ckbOfP_R0.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOfP_R0.Click += new System.EventHandler(this.ckb_Click);
			// 
			// ckbOfP_Ejt
			// 
			resources.ApplyResources(this.ckbOfP_Ejt, "ckbOfP_Ejt");
			this.ckbOfP_Ejt.BackColor = System.Drawing.Color.LightGray;
			this.ckbOfP_Ejt.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOfP_Ejt.FlatAppearance.BorderSize = 2;
			this.ckbOfP_Ejt.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOfP_Ejt.ForeColor = System.Drawing.Color.Black;
			this.ckbOfP_Ejt.Name = "ckbOfP_Ejt";
			this.ckbOfP_Ejt.Tag = "1";
			this.ckbOfP_Ejt.UseVisualStyleBackColor = false;
			this.ckbOfP_Ejt.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOfP_Ejt.Click += new System.EventHandler(this.ckb_Click);
			// 
			// btn_PickCl
			// 
			this.btn_PickCl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickCl.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickCl, "btn_PickCl");
			this.btn_PickCl.ForeColor = System.Drawing.Color.White;
			this.btn_PickCl.Name = "btn_PickCl";
			this.btn_PickCl.Tag = "OFP_BtmClean";
			this.btn_PickCl.UseVisualStyleBackColor = false;
			this.btn_PickCl.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_StripCl
			// 
			this.btn_StripCl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_StripCl.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_StripCl, "btn_StripCl");
			this.btn_StripCl.ForeColor = System.Drawing.Color.White;
			this.btn_StripCl.Name = "btn_StripCl";
			this.btn_StripCl.Tag = "OFP_BtmCleanStrip";
			this.btn_StripCl.UseVisualStyleBackColor = false;
			this.btn_StripCl.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_Place
			// 
			this.btn_Place.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_Place.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_Place, "btn_Place");
			this.btn_Place.ForeColor = System.Drawing.Color.White;
			this.btn_Place.Name = "btn_Place";
			this.btn_Place.Tag = "OFP_Place";
			this.btn_Place.UseVisualStyleBackColor = false;
			this.btn_Place.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_PickR
			// 
			this.btn_PickR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickR.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickR, "btn_PickR");
			this.btn_PickR.ForeColor = System.Drawing.Color.White;
			this.btn_PickR.Name = "btn_PickR";
			this.btn_PickR.Tag = "OFP_PickTbR";
			this.btn_PickR.UseVisualStyleBackColor = false;
			this.btn_PickR.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_PickL
			// 
			this.btn_PickL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickL.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickL, "btn_PickL");
			this.btn_PickL.ForeColor = System.Drawing.Color.White;
			this.btn_PickL.Name = "btn_PickL";
			this.btn_PickL.Tag = "OFP_PickTbL";
			this.btn_PickL.UseVisualStyleBackColor = false;
			this.btn_PickL.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// ckbOfP_Vac
			// 
			resources.ApplyResources(this.ckbOfP_Vac, "ckbOfP_Vac");
			this.ckbOfP_Vac.BackColor = System.Drawing.Color.LightGray;
			this.ckbOfP_Vac.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOfP_Vac.FlatAppearance.BorderSize = 2;
			this.ckbOfP_Vac.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOfP_Vac.ForeColor = System.Drawing.Color.Black;
			this.ckbOfP_Vac.Name = "ckbOfP_Vac";
			this.ckbOfP_Vac.Tag = "0";
			this.ckbOfP_Vac.UseVisualStyleBackColor = false;
			this.ckbOfP_Vac.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOfP_Vac.Click += new System.EventHandler(this.ckb_Click);
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
			// btn_H
			// 
			this.btn_H.BackColor = System.Drawing.Color.SteelBlue;
			this.btn_H.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_H, "btn_H");
			this.btn_H.ForeColor = System.Drawing.Color.White;
			this.btn_H.Image = global::SG2000X.Properties.Resources.Home64;
			this.btn_H.Name = "btn_H";
			this.btn_H.Tag = "OFP_Home";
			this.btn_H.UseVisualStyleBackColor = false;
			this.btn_H.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_Wait
			// 
			this.btn_Wait.BackColor = System.Drawing.Color.SteelBlue;
			this.btn_Wait.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_Wait, "btn_Wait");
			this.btn_Wait.ForeColor = System.Drawing.Color.White;
			this.btn_Wait.Image = global::SG2000X.Properties.Resources.Wait64;
			this.btn_Wait.Name = "btn_Wait";
			this.btn_Wait.Tag = "OFP_Wait";
			this.btn_Wait.UseVisualStyleBackColor = false;
			this.btn_Wait.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_PlaceR
			// 
			this.btn_PlaceR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PlaceR.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PlaceR, "btn_PlaceR");
			this.btn_PlaceR.ForeColor = System.Drawing.Color.White;
			this.btn_PlaceR.Name = "btn_PlaceR";
			this.btn_PlaceR.Tag = "OFP_PlaceTbR";
			this.btn_PlaceR.UseVisualStyleBackColor = false;
			this.btn_PlaceR.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// tab_CtrlOFP
			// 
			this.tab_CtrlOFP.Controls.Add(this.tab_PagePickerX);
			this.tab_CtrlOFP.Controls.Add(this.tab_PagePickerZ);
			resources.ApplyResources(this.tab_CtrlOFP, "tab_CtrlOFP");
			this.tab_CtrlOFP.Name = "tab_CtrlOFP";
			this.tab_CtrlOFP.SelectedIndex = 0;
			// 
			// tab_PagePickerX
			// 
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXDryPlace);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXStripClnEd);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXPickerClnEd);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXStripClnSt);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXPickerClnSt);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXRightPick);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXLeftPick);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXWait);
			resources.ApplyResources(this.tab_PagePickerX, "tab_PagePickerX");
			this.tab_PagePickerX.Name = "tab_PagePickerX";
			this.tab_PagePickerX.UseVisualStyleBackColor = true;
			// 
			// btn_PickerXDryPlace
			// 
			this.btn_PickerXDryPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXDryPlace.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXDryPlace, "btn_PickerXDryPlace");
			this.btn_PickerXDryPlace.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXDryPlace.Name = "btn_PickerXDryPlace";
			this.btn_PickerXDryPlace.Tag = "";
			this.btn_PickerXDryPlace.UseVisualStyleBackColor = false;
			this.btn_PickerXDryPlace.Click += new System.EventHandler(this.btn_PickerXDryPlace_Click);
			// 
			// btn_PickerXStripClnEd
			// 
			this.btn_PickerXStripClnEd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXStripClnEd.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXStripClnEd, "btn_PickerXStripClnEd");
			this.btn_PickerXStripClnEd.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXStripClnEd.Name = "btn_PickerXStripClnEd";
			this.btn_PickerXStripClnEd.Tag = "";
			this.btn_PickerXStripClnEd.UseVisualStyleBackColor = false;
			this.btn_PickerXStripClnEd.Click += new System.EventHandler(this.btn_PickerXStripClnEd_Click);
			// 
			// btn_PickerXPickerClnEd
			// 
			this.btn_PickerXPickerClnEd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXPickerClnEd.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXPickerClnEd, "btn_PickerXPickerClnEd");
			this.btn_PickerXPickerClnEd.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXPickerClnEd.Name = "btn_PickerXPickerClnEd";
			this.btn_PickerXPickerClnEd.Tag = "";
			this.btn_PickerXPickerClnEd.UseVisualStyleBackColor = false;
			this.btn_PickerXPickerClnEd.Click += new System.EventHandler(this.btn_PickerXPickerClnEd_Click);
			// 
			// btn_PickerXStripClnSt
			// 
			this.btn_PickerXStripClnSt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXStripClnSt.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXStripClnSt, "btn_PickerXStripClnSt");
			this.btn_PickerXStripClnSt.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXStripClnSt.Name = "btn_PickerXStripClnSt";
			this.btn_PickerXStripClnSt.Tag = "";
			this.btn_PickerXStripClnSt.UseVisualStyleBackColor = false;
			this.btn_PickerXStripClnSt.Click += new System.EventHandler(this.btn_PickerXStripClnSt_Click);
			// 
			// btn_PickerXPickerClnSt
			// 
			this.btn_PickerXPickerClnSt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXPickerClnSt.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXPickerClnSt, "btn_PickerXPickerClnSt");
			this.btn_PickerXPickerClnSt.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXPickerClnSt.Name = "btn_PickerXPickerClnSt";
			this.btn_PickerXPickerClnSt.Tag = "";
			this.btn_PickerXPickerClnSt.UseVisualStyleBackColor = false;
			this.btn_PickerXPickerClnSt.Click += new System.EventHandler(this.btn_PickerXPickerClnSt_Click);
			// 
			// btn_PickerXRightPick
			// 
			this.btn_PickerXRightPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXRightPick.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXRightPick, "btn_PickerXRightPick");
			this.btn_PickerXRightPick.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXRightPick.Name = "btn_PickerXRightPick";
			this.btn_PickerXRightPick.Tag = "";
			this.btn_PickerXRightPick.UseVisualStyleBackColor = false;
			this.btn_PickerXRightPick.Click += new System.EventHandler(this.btn_PickerXRightPick_Click);
			// 
			// btn_PickerXLeftPick
			// 
			this.btn_PickerXLeftPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXLeftPick.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXLeftPick, "btn_PickerXLeftPick");
			this.btn_PickerXLeftPick.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXLeftPick.Name = "btn_PickerXLeftPick";
			this.btn_PickerXLeftPick.Tag = "";
			this.btn_PickerXLeftPick.UseVisualStyleBackColor = false;
			this.btn_PickerXLeftPick.Click += new System.EventHandler(this.btn_PickerXLeftPick_Click);
			// 
			// btn_PickerXWait
			// 
			this.btn_PickerXWait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXWait.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXWait, "btn_PickerXWait");
			this.btn_PickerXWait.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXWait.Name = "btn_PickerXWait";
			this.btn_PickerXWait.Tag = "";
			this.btn_PickerXWait.UseVisualStyleBackColor = false;
			this.btn_PickerXWait.Click += new System.EventHandler(this.btn_PickerXWait_Click);
			// 
			// tab_PagePickerZ
			// 
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZDryPlace);
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZClnStrip);
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZClnPicker);
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZTblPickPlace);
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZWait);
			resources.ApplyResources(this.tab_PagePickerZ, "tab_PagePickerZ");
			this.tab_PagePickerZ.Name = "tab_PagePickerZ";
			this.tab_PagePickerZ.UseVisualStyleBackColor = true;
			// 
			// btn_PickerZDryPlace
			// 
			this.btn_PickerZDryPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerZDryPlace.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerZDryPlace, "btn_PickerZDryPlace");
			this.btn_PickerZDryPlace.ForeColor = System.Drawing.Color.White;
			this.btn_PickerZDryPlace.Name = "btn_PickerZDryPlace";
			this.btn_PickerZDryPlace.Tag = "";
			this.btn_PickerZDryPlace.UseVisualStyleBackColor = false;
			this.btn_PickerZDryPlace.Click += new System.EventHandler(this.btn_PickerZDryPlace_Click);
			// 
			// btn_PickerZClnStrip
			// 
			this.btn_PickerZClnStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerZClnStrip.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerZClnStrip, "btn_PickerZClnStrip");
			this.btn_PickerZClnStrip.ForeColor = System.Drawing.Color.White;
			this.btn_PickerZClnStrip.Name = "btn_PickerZClnStrip";
			this.btn_PickerZClnStrip.Tag = "";
			this.btn_PickerZClnStrip.UseVisualStyleBackColor = false;
			this.btn_PickerZClnStrip.Click += new System.EventHandler(this.btn_PickerZClnStrip_Click);
			// 
			// btn_PickerZClnPicker
			// 
			this.btn_PickerZClnPicker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerZClnPicker.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerZClnPicker, "btn_PickerZClnPicker");
			this.btn_PickerZClnPicker.ForeColor = System.Drawing.Color.White;
			this.btn_PickerZClnPicker.Name = "btn_PickerZClnPicker";
			this.btn_PickerZClnPicker.Tag = "";
			this.btn_PickerZClnPicker.UseVisualStyleBackColor = false;
			this.btn_PickerZClnPicker.Click += new System.EventHandler(this.btn_PickerZClnPicker_Click);
			// 
			// btn_PickerZTblPickPlace
			// 
			this.btn_PickerZTblPickPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerZTblPickPlace.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerZTblPickPlace, "btn_PickerZTblPickPlace");
			this.btn_PickerZTblPickPlace.ForeColor = System.Drawing.Color.White;
			this.btn_PickerZTblPickPlace.Name = "btn_PickerZTblPickPlace";
			this.btn_PickerZTblPickPlace.Tag = "";
			this.btn_PickerZTblPickPlace.UseVisualStyleBackColor = false;
			this.btn_PickerZTblPickPlace.Click += new System.EventHandler(this.btn_PickerZTblPickPlace_Click);
			// 
			// btn_PickerZWait
			// 
			this.btn_PickerZWait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerZWait.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerZWait, "btn_PickerZWait");
			this.btn_PickerZWait.ForeColor = System.Drawing.Color.White;
			this.btn_PickerZWait.Name = "btn_PickerZWait";
			this.btn_PickerZWait.Tag = "";
			this.btn_PickerZWait.UseVisualStyleBackColor = false;
			this.btn_PickerZWait.Click += new System.EventHandler(this.btn_PickerZWait_Click);
			// 
			// lblOfP_IV2_Error
			// 
			this.lblOfP_IV2_Error.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_IV2_Error.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_IV2_Error, "lblOfP_IV2_Error");
			this.lblOfP_IV2_Error.Name = "lblOfP_IV2_Error";
			this.lblOfP_IV2_Error.Tag = "IV Error$";
			this.lblOfP_IV2_Error.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOfP_IV2_Busy
			// 
			this.lblOfP_IV2_Busy.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_IV2_Busy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_IV2_Busy, "lblOfP_IV2_Busy");
			this.lblOfP_IV2_Busy.Name = "lblOfP_IV2_Busy";
			this.lblOfP_IV2_Busy.Tag = "IV Busy$";
			this.lblOfP_IV2_Busy.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOfP_IV2_OK
			// 
			this.lblOfP_IV2_OK.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_IV2_OK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_IV2_OK, "lblOfP_IV2_OK");
			this.lblOfP_IV2_OK.Name = "lblOfP_IV2_OK";
			this.lblOfP_IV2_OK.Tag = "IV All OK$";
			this.lblOfP_IV2_OK.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// btn_CoverDryer
			// 
			this.btn_CoverDryer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_CoverDryer.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_CoverDryer, "btn_CoverDryer");
			this.btn_CoverDryer.ForeColor = System.Drawing.Color.White;
			this.btn_CoverDryer.Name = "btn_CoverDryer";
			this.btn_CoverDryer.Tag = "OFP_CoverDryer";
			this.btn_CoverDryer.UseVisualStyleBackColor = false;
			this.btn_CoverDryer.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_IV_DryClamp
			// 
			this.btn_IV_DryClamp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_IV_DryClamp.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_IV_DryClamp, "btn_IV_DryClamp");
			this.btn_IV_DryClamp.ForeColor = System.Drawing.Color.White;
			this.btn_IV_DryClamp.Name = "btn_IV_DryClamp";
			this.btn_IV_DryClamp.Tag = "OFP_IV_DryClamp";
			this.btn_IV_DryClamp.UseVisualStyleBackColor = false;
			this.btn_IV_DryClamp.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// lblOfP_CarrierDetect
			// 
			this.lblOfP_CarrierDetect.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_CarrierDetect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_CarrierDetect, "lblOfP_CarrierDetect");
			this.lblOfP_CarrierDetect.Name = "lblOfP_CarrierDetect";
			this.lblOfP_CarrierDetect.Tag = "Carrier Detect$";
			// 
			// lblOfP_ClampClose
			// 
			this.lblOfP_ClampClose.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_ClampClose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_ClampClose, "lblOfP_ClampClose");
			this.lblOfP_ClampClose.Name = "lblOfP_ClampClose";
			this.lblOfP_ClampClose.Tag = "Clamp Close$";
			this.lblOfP_ClampClose.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOfP_ClampOpen
			// 
			this.lblOfP_ClampOpen.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_ClampOpen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_ClampOpen, "lblOfP_ClampOpen");
			this.lblOfP_ClampOpen.Name = "lblOfP_ClampOpen";
			this.lblOfP_ClampOpen.Tag = "Clamp Open$";
			this.lblOfP_ClampOpen.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// btnOfP_Cover_Place
			// 
			this.btnOfP_Cover_Place.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOfP_Cover_Place.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOfP_Cover_Place, "btnOfP_Cover_Place");
			this.btnOfP_Cover_Place.ForeColor = System.Drawing.Color.White;
			this.btnOfP_Cover_Place.Name = "btnOfP_Cover_Place";
			this.btnOfP_Cover_Place.Tag = "OFP_CoverPlace";
			this.btnOfP_Cover_Place.UseVisualStyleBackColor = false;
			this.btnOfP_Cover_Place.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btnOfP_Cover_Pick
			// 
			this.btnOfP_Cover_Pick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOfP_Cover_Pick.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOfP_Cover_Pick, "btnOfP_Cover_Pick");
			this.btnOfP_Cover_Pick.ForeColor = System.Drawing.Color.White;
			this.btnOfP_Cover_Pick.Name = "btnOfP_Cover_Pick";
			this.btnOfP_Cover_Pick.Tag = "OFP_CoverPick";
			this.btnOfP_Cover_Pick.UseVisualStyleBackColor = false;
			this.btnOfP_Cover_Pick.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btnOfP_Cover_Check_IV2
			// 
			this.btnOfP_Cover_Check_IV2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOfP_Cover_Check_IV2.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOfP_Cover_Check_IV2, "btnOfP_Cover_Check_IV2");
			this.btnOfP_Cover_Check_IV2.ForeColor = System.Drawing.Color.White;
			this.btnOfP_Cover_Check_IV2.Name = "btnOfP_Cover_Check_IV2";
			this.btnOfP_Cover_Check_IV2.Tag = "OFP_CoverIV2";
			this.btnOfP_Cover_Check_IV2.UseVisualStyleBackColor = false;
			this.btnOfP_Cover_Check_IV2.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btnOfP_Unit_Check_IV2
			// 
			this.btnOfP_Unit_Check_IV2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOfP_Unit_Check_IV2.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOfP_Unit_Check_IV2, "btnOfP_Unit_Check_IV2");
			this.btnOfP_Unit_Check_IV2.ForeColor = System.Drawing.Color.White;
			this.btnOfP_Unit_Check_IV2.Name = "btnOfP_Unit_Check_IV2";
			this.btnOfP_Unit_Check_IV2.Tag = "OFP_IV2";
			this.btnOfP_Unit_Check_IV2.UseVisualStyleBackColor = false;
			this.btnOfP_Unit_Check_IV2.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// ckbOfP_Clamp
			// 
			resources.ApplyResources(this.ckbOfP_Clamp, "ckbOfP_Clamp");
			this.ckbOfP_Clamp.BackColor = System.Drawing.Color.LightGray;
			this.ckbOfP_Clamp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOfP_Clamp.FlatAppearance.BorderSize = 2;
			this.ckbOfP_Clamp.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOfP_Clamp.ForeColor = System.Drawing.Color.Black;
			this.ckbOfP_Clamp.Name = "ckbOfP_Clamp";
			this.ckbOfP_Clamp.Tag = "6";
			this.ckbOfP_Clamp.UseVisualStyleBackColor = false;
			this.ckbOfP_Clamp.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOfP_Clamp.Click += new System.EventHandler(this.ckb_Click);
			// 
			// lblOfP_VacFree
			// 
			this.lblOfP_VacFree.BackColor = System.Drawing.Color.LightGray;
			this.lblOfP_VacFree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOfP_VacFree, "lblOfP_VacFree");
			this.lblOfP_VacFree.Name = "lblOfP_VacFree";
			this.lblOfP_VacFree.Tag = "Vacuum Free$";
			this.lblOfP_VacFree.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// btn_IV_DryTable
			// 
			this.btn_IV_DryTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_IV_DryTable.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_IV_DryTable, "btn_IV_DryTable");
			this.btn_IV_DryTable.ForeColor = System.Drawing.Color.White;
			this.btn_IV_DryTable.Name = "btn_IV_DryTable";
			this.btn_IV_DryTable.Tag = "OFP_IV_DryTable";
			this.btn_IV_DryTable.UseVisualStyleBackColor = false;
			this.btn_IV_DryTable.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// ckbOfP_IV_Trigger
			// 
			resources.ApplyResources(this.ckbOfP_IV_Trigger, "ckbOfP_IV_Trigger");
			this.ckbOfP_IV_Trigger.BackColor = System.Drawing.Color.LightGray;
			this.ckbOfP_IV_Trigger.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOfP_IV_Trigger.FlatAppearance.BorderSize = 2;
			this.ckbOfP_IV_Trigger.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOfP_IV_Trigger.ForeColor = System.Drawing.Color.Black;
			this.ckbOfP_IV_Trigger.Name = "ckbOfP_IV_Trigger";
			this.ckbOfP_IV_Trigger.Tag = "4";
			this.ckbOfP_IV_Trigger.UseVisualStyleBackColor = false;
			this.ckbOfP_IV_Trigger.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOfP_IV_Trigger.Click += new System.EventHandler(this.ckb_Click);
			// 
			// btn_BtmClnBrush
			// 
			this.btn_BtmClnBrush.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_BtmClnBrush.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_BtmClnBrush, "btn_BtmClnBrush");
			this.btn_BtmClnBrush.ForeColor = System.Drawing.Color.White;
			this.btn_BtmClnBrush.Name = "btn_BtmClnBrush";
			this.btn_BtmClnBrush.Tag = "OFP_BtmCleanBrush";
			this.btn_BtmClnBrush.UseVisualStyleBackColor = false;
			this.btn_BtmClnBrush.Click += new System.EventHandler(this.btn_Cycle_Click);
			// 
			// btn_StripNzlCyl_N1
			// 
			this.btn_StripNzlCyl_N1.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.btn_StripNzlCyl_N1, "btn_StripNzlCyl_N1");
			this.btn_StripNzlCyl_N1.Name = "btn_StripNzlCyl_N1";
			this.btn_StripNzlCyl_N1.UseVisualStyleBackColor = false;
			this.btn_StripNzlCyl_N1.Click += new System.EventHandler(this.btn_StripNzlCyl_N1_Click);
			// 
			// btn_StripNzlCyl_N2
			// 
			this.btn_StripNzlCyl_N2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.btn_StripNzlCyl_N2, "btn_StripNzlCyl_N2");
			this.btn_StripNzlCyl_N2.Name = "btn_StripNzlCyl_N2";
			this.btn_StripNzlCyl_N2.UseVisualStyleBackColor = false;
			this.btn_StripNzlCyl_N2.Click += new System.EventHandler(this.btn_StripNzlCyl_N2_Click);
			// 
			// vwMan_08Ofp
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.btn_StripNzlCyl_N2);
			this.Controls.Add(this.btn_StripNzlCyl_N1);
			this.Controls.Add(this.btn_BtmClnBrush);
			this.Controls.Add(this.ckbOfP_IV_Trigger);
			this.Controls.Add(this.btn_IV_DryTable);
			this.Controls.Add(this.lblOfP_VacFree);
			this.Controls.Add(this.ckbOfP_Clamp);
			this.Controls.Add(this.btnOfP_Cover_Place);
			this.Controls.Add(this.btnOfP_Cover_Pick);
			this.Controls.Add(this.btnOfP_Cover_Check_IV2);
			this.Controls.Add(this.btnOfP_Unit_Check_IV2);
			this.Controls.Add(this.lblOfP_CarrierDetect);
			this.Controls.Add(this.lblOfP_ClampClose);
			this.Controls.Add(this.lblOfP_ClampOpen);
			this.Controls.Add(this.btn_CoverDryer);
			this.Controls.Add(this.btn_IV_DryClamp);
			this.Controls.Add(this.lblOfP_IV2_Error);
			this.Controls.Add(this.lblOfP_IV2_Busy);
			this.Controls.Add(this.lblOfP_IV2_OK);
			this.Controls.Add(this.tab_CtrlOFP);
			this.Controls.Add(this.btn_PlaceR);
			this.Controls.Add(this.panel13);
			this.Controls.Add(this.panel16);
			this.Controls.Add(this.ckbOfP_Drn);
			this.Controls.Add(this.lblOfP_LdCell);
			this.Controls.Add(this.lblOfP_Vac);
			this.Controls.Add(this.lblOfP_R90);
			this.Controls.Add(this.lblOfP_R0);
			this.Controls.Add(this.ckbOfP_R0);
			this.Controls.Add(this.ckbOfP_Ejt);
			this.Controls.Add(this.btn_PickCl);
			this.Controls.Add(this.btn_StripCl);
			this.Controls.Add(this.btn_Place);
			this.Controls.Add(this.btn_PickR);
			this.Controls.Add(this.btn_PickL);
			this.Controls.Add(this.ckbOfP_Vac);
			this.Controls.Add(this.btn_Stop);
			this.Controls.Add(this.btn_H);
			this.Controls.Add(this.btn_Wait);
			resources.ApplyResources(this, "$this");
			this.Name = "vwMan_08Ofp";
			this.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			this.panel13.ResumeLayout(false);
			this.panel16.ResumeLayout(false);
			this.tab_CtrlOFP.ResumeLayout(false);
			this.tab_PagePickerX.ResumeLayout(false);
			this.tab_PagePickerZ.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Label lblOfp_ZPos;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblOfp_ZStat;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Label lblOfp_XPos;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label lblOfp_XStat;
        private System.Windows.Forms.CheckBox ckbOfP_Drn;
        private System.Windows.Forms.Label lblOfP_LdCell;
        private System.Windows.Forms.Label lblOfP_Vac;
        private System.Windows.Forms.Label lblOfP_R90;
        private System.Windows.Forms.Label lblOfP_R0;
        private System.Windows.Forms.CheckBox ckbOfP_R0;
        private System.Windows.Forms.CheckBox ckbOfP_Ejt;
        private System.Windows.Forms.Button btn_PickCl;
        private System.Windows.Forms.Button btn_StripCl;
        private System.Windows.Forms.Button btn_Place;
        private System.Windows.Forms.Button btn_PickR;
        private System.Windows.Forms.Button btn_PickL;
        private System.Windows.Forms.CheckBox ckbOfP_Vac;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Button btn_H;
        private System.Windows.Forms.Button btn_Wait;
        private System.Windows.Forms.Button btn_PlaceR;
        private System.Windows.Forms.TabControl tab_CtrlOFP;
        private System.Windows.Forms.TabPage tab_PagePickerX;
        private System.Windows.Forms.TabPage tab_PagePickerZ;
        private System.Windows.Forms.Button btn_PickerXDryPlace;
        private System.Windows.Forms.Button btn_PickerXStripClnEd;
        private System.Windows.Forms.Button btn_PickerXPickerClnEd;
        private System.Windows.Forms.Button btn_PickerXStripClnSt;
        private System.Windows.Forms.Button btn_PickerXPickerClnSt;
        private System.Windows.Forms.Button btn_PickerXRightPick;
        private System.Windows.Forms.Button btn_PickerXLeftPick;
        private System.Windows.Forms.Button btn_PickerXWait;
        private System.Windows.Forms.Button btn_PickerZDryPlace;
        private System.Windows.Forms.Button btn_PickerZClnStrip;
        private System.Windows.Forms.Button btn_PickerZClnPicker;
        private System.Windows.Forms.Button btn_PickerZTblPickPlace;
        private System.Windows.Forms.Button btn_PickerZWait;
        private System.Windows.Forms.Label lblOfP_IV2_Error;
        private System.Windows.Forms.Label lblOfP_IV2_Busy;
        private System.Windows.Forms.Label lblOfP_IV2_OK;
        private System.Windows.Forms.Button btn_CoverDryer;
        private System.Windows.Forms.Button btn_IV_DryClamp;
        private System.Windows.Forms.Label lblOfP_CarrierDetect;
        private System.Windows.Forms.Label lblOfP_ClampClose;
        private System.Windows.Forms.Label lblOfP_ClampOpen;
        private System.Windows.Forms.Button btnOfP_Cover_Place;
        private System.Windows.Forms.Button btnOfP_Cover_Pick;
        private System.Windows.Forms.Button btnOfP_Cover_Check_IV2;
        private System.Windows.Forms.Button btnOfP_Unit_Check_IV2;
        private System.Windows.Forms.CheckBox ckbOfP_Clamp;
		private System.Windows.Forms.Label lblOfP_VacFree;
		private System.Windows.Forms.Button btn_IV_DryTable;
		private System.Windows.Forms.CheckBox ckbOfP_IV_Trigger;
		private System.Windows.Forms.Button btn_BtmClnBrush;
		private System.Windows.Forms.Button btn_StripNzlCyl_N1;
		private System.Windows.Forms.Button btn_StripNzlCyl_N2;
	}
}
