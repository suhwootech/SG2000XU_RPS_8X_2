namespace SG2000X
{
    partial class vwMan_04Onp
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_04Onp));
			this.ckbOnP_Clean = new System.Windows.Forms.CheckBox();
			this.lblOnP_CleanR = new System.Windows.Forms.Label();
			this.lblOnP_CleanL = new System.Windows.Forms.Label();
			this.btnOnP_Clean = new System.Windows.Forms.Button();
			this.pnlOnp_Bcr = new System.Windows.Forms.Panel();
			this.label52 = new System.Windows.Forms.Label();
			this.label55 = new System.Windows.Forms.Label();
			this.lblOnp_Bcr = new System.Windows.Forms.Label();
			this.pnlOnp_Y = new System.Windows.Forms.Panel();
			this.lblOnp_YPos = new System.Windows.Forms.Label();
			this.label54 = new System.Windows.Forms.Label();
			this.lblOnp_YStat = new System.Windows.Forms.Label();
			this.btnOnp_Ori = new System.Windows.Forms.Button();
			this.btnOnp_Bcr = new System.Windows.Forms.Button();
			this.panel10 = new System.Windows.Forms.Panel();
			this.lblOnp_ZPos = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.lblOnp_ZStat = new System.Windows.Forms.Label();
			this.panel14 = new System.Windows.Forms.Panel();
			this.lblOnp_XPos = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.lblOnp_XStat = new System.Windows.Forms.Label();
			this.ckbOnP_Drn = new System.Windows.Forms.CheckBox();
			this.lblOnP_LdCell = new System.Windows.Forms.Label();
			this.lblOnP_Vac = new System.Windows.Forms.Label();
			this.lblOnP_R90 = new System.Windows.Forms.Label();
			this.lblOnP_R0 = new System.Windows.Forms.Label();
			this.ckbOnP_R0 = new System.Windows.Forms.CheckBox();
			this.ckbOnP_Ejt = new System.Windows.Forms.CheckBox();
			this.btnOnP_PlaceR = new System.Windows.Forms.Button();
			this.btnOnP_PlaceL = new System.Windows.Forms.Button();
			this.btnOnP_PickLTbl = new System.Windows.Forms.Button();
			this.btnOnP_PickRail = new System.Windows.Forms.Button();
			this.ckbOnP_Vac = new System.Windows.Forms.CheckBox();
			this.button5 = new System.Windows.Forms.Button();
			this.btnOnP_H = new System.Windows.Forms.Button();
			this.btnOnP_Wait = new System.Windows.Forms.Button();
			this.btnOnP_BcrOri = new System.Windows.Forms.Button();
			this.tab_CtrlONP = new System.Windows.Forms.TabControl();
			this.tab_PagePickerX = new System.Windows.Forms.TabPage();
			this.btn_PickerXRightPlace = new System.Windows.Forms.Button();
			this.btn_PickerXLeftPlace = new System.Windows.Forms.Button();
			this.btn_PickerXRailPick = new System.Windows.Forms.Button();
			this.btn_PickerXWait = new System.Windows.Forms.Button();
			this.tab_PagePickerZ = new System.Windows.Forms.TabPage();
			this.btn_PickerZRailPick = new System.Windows.Forms.Button();
			this.btn_PickerZTblPickPlace = new System.Windows.Forms.Button();
			this.btn_PickerZWait = new System.Windows.Forms.Button();
			this.btnOnP_Unit_Check_IV2 = new System.Windows.Forms.Button();
			this.lblOnP_VacFree = new System.Windows.Forms.Label();
			this.pnlOnp_Bcr.SuspendLayout();
			this.pnlOnp_Y.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel14.SuspendLayout();
			this.tab_CtrlONP.SuspendLayout();
			this.tab_PagePickerX.SuspendLayout();
			this.tab_PagePickerZ.SuspendLayout();
			this.SuspendLayout();
			// 
			// ckbOnP_Clean
			// 
			resources.ApplyResources(this.ckbOnP_Clean, "ckbOnP_Clean");
			this.ckbOnP_Clean.BackColor = System.Drawing.Color.LightGray;
			this.ckbOnP_Clean.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOnP_Clean.FlatAppearance.BorderSize = 2;
			this.ckbOnP_Clean.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOnP_Clean.ForeColor = System.Drawing.Color.Black;
			this.ckbOnP_Clean.Name = "ckbOnP_Clean";
			this.ckbOnP_Clean.Tag = "4";
			this.ckbOnP_Clean.UseVisualStyleBackColor = false;
			this.ckbOnP_Clean.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOnP_Clean.Click += new System.EventHandler(this.ckb_Click);
			// 
			// lblOnP_CleanR
			// 
			this.lblOnP_CleanR.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_CleanR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_CleanR, "lblOnP_CleanR");
			this.lblOnP_CleanR.Name = "lblOnP_CleanR";
			this.lblOnP_CleanR.Tag = "Cleaner Right$";
			// 
			// lblOnP_CleanL
			// 
			this.lblOnP_CleanL.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_CleanL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_CleanL, "lblOnP_CleanL");
			this.lblOnP_CleanL.Name = "lblOnP_CleanL";
			this.lblOnP_CleanL.Tag = "Cleaner Left$";
			// 
			// btnOnP_Clean
			// 
			this.btnOnP_Clean.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_Clean.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_Clean, "btnOnP_Clean");
			this.btnOnP_Clean.ForeColor = System.Drawing.Color.White;
			this.btnOnP_Clean.Name = "btnOnP_Clean";
			this.btnOnP_Clean.Tag = "ONP_Clean";
			this.btnOnP_Clean.UseVisualStyleBackColor = false;
			this.btnOnP_Clean.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// pnlOnp_Bcr
			// 
			this.pnlOnp_Bcr.BackColor = System.Drawing.Color.WhiteSmoke;
			this.pnlOnp_Bcr.Controls.Add(this.label52);
			this.pnlOnp_Bcr.Controls.Add(this.label55);
			this.pnlOnp_Bcr.Controls.Add(this.lblOnp_Bcr);
			resources.ApplyResources(this.pnlOnp_Bcr, "pnlOnp_Bcr");
			this.pnlOnp_Bcr.Name = "pnlOnp_Bcr";
			this.pnlOnp_Bcr.Tag = "1";
			// 
			// label52
			// 
			this.label52.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
			resources.ApplyResources(this.label52, "label52");
			this.label52.ForeColor = System.Drawing.Color.White;
			this.label52.Name = "label52";
			// 
			// label55
			// 
			resources.ApplyResources(this.label55, "label55");
			this.label55.Name = "label55";
			// 
			// lblOnp_Bcr
			// 
			this.lblOnp_Bcr.BackColor = System.Drawing.Color.LightGray;
			this.lblOnp_Bcr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnp_Bcr, "lblOnp_Bcr");
			this.lblOnp_Bcr.Name = "lblOnp_Bcr";
			// 
			// pnlOnp_Y
			// 
			this.pnlOnp_Y.BackColor = System.Drawing.Color.LightGray;
			this.pnlOnp_Y.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlOnp_Y.Controls.Add(this.lblOnp_YPos);
			this.pnlOnp_Y.Controls.Add(this.label54);
			this.pnlOnp_Y.Controls.Add(this.lblOnp_YStat);
			resources.ApplyResources(this.pnlOnp_Y, "pnlOnp_Y");
			this.pnlOnp_Y.Name = "pnlOnp_Y";
			this.pnlOnp_Y.Tag = "0";
			// 
			// lblOnp_YPos
			// 
			this.lblOnp_YPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblOnp_YPos, "lblOnp_YPos");
			this.lblOnp_YPos.Name = "lblOnp_YPos";
			// 
			// label54
			// 
			resources.ApplyResources(this.label54, "label54");
			this.label54.Name = "label54";
			// 
			// lblOnp_YStat
			// 
			this.lblOnp_YStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lblOnp_YStat, "lblOnp_YStat");
			this.lblOnp_YStat.Name = "lblOnp_YStat";
			// 
			// btnOnp_Ori
			// 
			this.btnOnp_Ori.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnp_Ori.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnp_Ori, "btnOnp_Ori");
			this.btnOnp_Ori.ForeColor = System.Drawing.Color.White;
			this.btnOnp_Ori.Name = "btnOnp_Ori";
			this.btnOnp_Ori.Tag = "INR_OriReady";
			this.btnOnp_Ori.UseVisualStyleBackColor = false;
			this.btnOnp_Ori.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// btnOnp_Bcr
			// 
			this.btnOnp_Bcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnp_Bcr.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnp_Bcr, "btnOnp_Bcr");
			this.btnOnp_Bcr.ForeColor = System.Drawing.Color.White;
			this.btnOnp_Bcr.Name = "btnOnp_Bcr";
			this.btnOnp_Bcr.Tag = "INR_BcrReady";
			this.btnOnp_Bcr.UseVisualStyleBackColor = false;
			this.btnOnp_Bcr.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// panel10
			// 
			this.panel10.BackColor = System.Drawing.Color.LightGray;
			this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel10.Controls.Add(this.lblOnp_ZPos);
			this.panel10.Controls.Add(this.label19);
			this.panel10.Controls.Add(this.lblOnp_ZStat);
			resources.ApplyResources(this.panel10, "panel10");
			this.panel10.Name = "panel10";
			this.panel10.Tag = "0";
			// 
			// lblOnp_ZPos
			// 
			this.lblOnp_ZPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblOnp_ZPos, "lblOnp_ZPos");
			this.lblOnp_ZPos.Name = "lblOnp_ZPos";
			// 
			// label19
			// 
			resources.ApplyResources(this.label19, "label19");
			this.label19.Name = "label19";
			// 
			// lblOnp_ZStat
			// 
			this.lblOnp_ZStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lblOnp_ZStat, "lblOnp_ZStat");
			this.lblOnp_ZStat.Name = "lblOnp_ZStat";
			// 
			// panel14
			// 
			this.panel14.BackColor = System.Drawing.Color.LightGray;
			this.panel14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel14.Controls.Add(this.lblOnp_XPos);
			this.panel14.Controls.Add(this.label36);
			this.panel14.Controls.Add(this.lblOnp_XStat);
			resources.ApplyResources(this.panel14, "panel14");
			this.panel14.Name = "panel14";
			this.panel14.Tag = "0";
			// 
			// lblOnp_XPos
			// 
			this.lblOnp_XPos.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.lblOnp_XPos, "lblOnp_XPos");
			this.lblOnp_XPos.Name = "lblOnp_XPos";
			// 
			// label36
			// 
			resources.ApplyResources(this.label36, "label36");
			this.label36.Name = "label36";
			// 
			// lblOnp_XStat
			// 
			this.lblOnp_XStat.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lblOnp_XStat, "lblOnp_XStat");
			this.lblOnp_XStat.Name = "lblOnp_XStat";
			// 
			// ckbOnP_Drn
			// 
			resources.ApplyResources(this.ckbOnP_Drn, "ckbOnP_Drn");
			this.ckbOnP_Drn.BackColor = System.Drawing.Color.LightGray;
			this.ckbOnP_Drn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOnP_Drn.FlatAppearance.BorderSize = 2;
			this.ckbOnP_Drn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOnP_Drn.ForeColor = System.Drawing.Color.Black;
			this.ckbOnP_Drn.Name = "ckbOnP_Drn";
			this.ckbOnP_Drn.Tag = "2";
			this.ckbOnP_Drn.UseVisualStyleBackColor = false;
			this.ckbOnP_Drn.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOnP_Drn.Click += new System.EventHandler(this.ckb_Click);
			// 
			// lblOnP_LdCell
			// 
			this.lblOnP_LdCell.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_LdCell.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_LdCell, "lblOnP_LdCell");
			this.lblOnP_LdCell.Name = "lblOnP_LdCell";
			this.lblOnP_LdCell.Tag = "Load Cell$";
			this.lblOnP_LdCell.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOnP_Vac
			// 
			this.lblOnP_Vac.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_Vac.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_Vac, "lblOnP_Vac");
			this.lblOnP_Vac.Name = "lblOnP_Vac";
			this.lblOnP_Vac.Tag = "Vacuum$";
			this.lblOnP_Vac.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOnP_R90
			// 
			this.lblOnP_R90.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_R90.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_R90, "lblOnP_R90");
			this.lblOnP_R90.Name = "lblOnP_R90";
			this.lblOnP_R90.Tag = "Rotate 90º$";
			this.lblOnP_R90.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// lblOnP_R0
			// 
			this.lblOnP_R0.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_R0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_R0, "lblOnP_R0");
			this.lblOnP_R0.Name = "lblOnP_R0";
			this.lblOnP_R0.Tag = "Rotate 0º$";
			this.lblOnP_R0.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
			// 
			// ckbOnP_R0
			// 
			resources.ApplyResources(this.ckbOnP_R0, "ckbOnP_R0");
			this.ckbOnP_R0.BackColor = System.Drawing.Color.LightGray;
			this.ckbOnP_R0.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOnP_R0.FlatAppearance.BorderSize = 2;
			this.ckbOnP_R0.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOnP_R0.ForeColor = System.Drawing.Color.Black;
			this.ckbOnP_R0.Name = "ckbOnP_R0";
			this.ckbOnP_R0.Tag = "3";
			this.ckbOnP_R0.UseVisualStyleBackColor = false;
			this.ckbOnP_R0.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOnP_R0.Click += new System.EventHandler(this.ckb_Click);
			// 
			// ckbOnP_Ejt
			// 
			resources.ApplyResources(this.ckbOnP_Ejt, "ckbOnP_Ejt");
			this.ckbOnP_Ejt.BackColor = System.Drawing.Color.LightGray;
			this.ckbOnP_Ejt.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOnP_Ejt.FlatAppearance.BorderSize = 2;
			this.ckbOnP_Ejt.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOnP_Ejt.ForeColor = System.Drawing.Color.Black;
			this.ckbOnP_Ejt.Name = "ckbOnP_Ejt";
			this.ckbOnP_Ejt.Tag = "1";
			this.ckbOnP_Ejt.UseVisualStyleBackColor = false;
			this.ckbOnP_Ejt.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOnP_Ejt.Click += new System.EventHandler(this.ckb_Click);
			// 
			// btnOnP_PlaceR
			// 
			this.btnOnP_PlaceR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_PlaceR.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_PlaceR, "btnOnP_PlaceR");
			this.btnOnP_PlaceR.ForeColor = System.Drawing.Color.White;
			this.btnOnP_PlaceR.Name = "btnOnP_PlaceR";
			this.btnOnP_PlaceR.Tag = "ONP_PlaceTbR";
			this.btnOnP_PlaceR.UseVisualStyleBackColor = false;
			this.btnOnP_PlaceR.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// btnOnP_PlaceL
			// 
			this.btnOnP_PlaceL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_PlaceL.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_PlaceL, "btnOnP_PlaceL");
			this.btnOnP_PlaceL.ForeColor = System.Drawing.Color.White;
			this.btnOnP_PlaceL.Name = "btnOnP_PlaceL";
			this.btnOnP_PlaceL.Tag = "ONP_PlaceTbL";
			this.btnOnP_PlaceL.UseVisualStyleBackColor = false;
			this.btnOnP_PlaceL.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// btnOnP_PickLTbl
			// 
			this.btnOnP_PickLTbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_PickLTbl.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_PickLTbl, "btnOnP_PickLTbl");
			this.btnOnP_PickLTbl.ForeColor = System.Drawing.Color.White;
			this.btnOnP_PickLTbl.Name = "btnOnP_PickLTbl";
			this.btnOnP_PickLTbl.Tag = "ONP_PickTbL";
			this.btnOnP_PickLTbl.UseVisualStyleBackColor = false;
			this.btnOnP_PickLTbl.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// btnOnP_PickRail
			// 
			this.btnOnP_PickRail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_PickRail.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_PickRail, "btnOnP_PickRail");
			this.btnOnP_PickRail.ForeColor = System.Drawing.Color.White;
			this.btnOnP_PickRail.Name = "btnOnP_PickRail";
			this.btnOnP_PickRail.Tag = "ONP_Pick";
			this.btnOnP_PickRail.UseVisualStyleBackColor = false;
			this.btnOnP_PickRail.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// ckbOnP_Vac
			// 
			resources.ApplyResources(this.ckbOnP_Vac, "ckbOnP_Vac");
			this.ckbOnP_Vac.BackColor = System.Drawing.Color.LightGray;
			this.ckbOnP_Vac.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.ckbOnP_Vac.FlatAppearance.BorderSize = 2;
			this.ckbOnP_Vac.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
			this.ckbOnP_Vac.ForeColor = System.Drawing.Color.Black;
			this.ckbOnP_Vac.Name = "ckbOnP_Vac";
			this.ckbOnP_Vac.Tag = "0";
			this.ckbOnP_Vac.UseVisualStyleBackColor = false;
			this.ckbOnP_Vac.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
			this.ckbOnP_Vac.Click += new System.EventHandler(this.ckb_Click);
			// 
			// button5
			// 
			this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
			this.button5.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.button5, "button5");
			this.button5.ForeColor = System.Drawing.Color.White;
			this.button5.Image = global::SG2000X.Properties.Resources.Stop64;
			this.button5.Name = "button5";
			this.button5.Tag = "";
			this.button5.UseVisualStyleBackColor = false;
			this.button5.Click += new System.EventHandler(this.btn_Stop_Click);
			// 
			// btnOnP_H
			// 
			this.btnOnP_H.BackColor = System.Drawing.Color.SteelBlue;
			this.btnOnP_H.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_H, "btnOnP_H");
			this.btnOnP_H.ForeColor = System.Drawing.Color.White;
			this.btnOnP_H.Image = global::SG2000X.Properties.Resources.Home64;
			this.btnOnP_H.Name = "btnOnP_H";
			this.btnOnP_H.Tag = "ONP_Home";
			this.btnOnP_H.UseVisualStyleBackColor = false;
			this.btnOnP_H.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// btnOnP_Wait
			// 
			this.btnOnP_Wait.BackColor = System.Drawing.Color.SteelBlue;
			this.btnOnP_Wait.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_Wait, "btnOnP_Wait");
			this.btnOnP_Wait.ForeColor = System.Drawing.Color.White;
			this.btnOnP_Wait.Image = global::SG2000X.Properties.Resources.Wait64;
			this.btnOnP_Wait.Name = "btnOnP_Wait";
			this.btnOnP_Wait.Tag = "ONP_Wait";
			this.btnOnP_Wait.UseVisualStyleBackColor = false;
			this.btnOnP_Wait.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// btnOnP_BcrOri
			// 
			this.btnOnP_BcrOri.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_BcrOri.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_BcrOri, "btnOnP_BcrOri");
			this.btnOnP_BcrOri.ForeColor = System.Drawing.Color.White;
			this.btnOnP_BcrOri.Name = "btnOnP_BcrOri";
			this.btnOnP_BcrOri.Tag = "INR_CheckBcrOri";
			this.btnOnP_BcrOri.UseVisualStyleBackColor = false;
			this.btnOnP_BcrOri.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// tab_CtrlONP
			// 
			this.tab_CtrlONP.Controls.Add(this.tab_PagePickerX);
			this.tab_CtrlONP.Controls.Add(this.tab_PagePickerZ);
			resources.ApplyResources(this.tab_CtrlONP, "tab_CtrlONP");
			this.tab_CtrlONP.Name = "tab_CtrlONP";
			this.tab_CtrlONP.SelectedIndex = 0;
			// 
			// tab_PagePickerX
			// 
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXRightPlace);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXLeftPlace);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXRailPick);
			this.tab_PagePickerX.Controls.Add(this.btn_PickerXWait);
			resources.ApplyResources(this.tab_PagePickerX, "tab_PagePickerX");
			this.tab_PagePickerX.Name = "tab_PagePickerX";
			this.tab_PagePickerX.UseVisualStyleBackColor = true;
			// 
			// btn_PickerXRightPlace
			// 
			this.btn_PickerXRightPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXRightPlace.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXRightPlace, "btn_PickerXRightPlace");
			this.btn_PickerXRightPlace.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXRightPlace.Name = "btn_PickerXRightPlace";
			this.btn_PickerXRightPlace.Tag = "";
			this.btn_PickerXRightPlace.UseVisualStyleBackColor = false;
			this.btn_PickerXRightPlace.Click += new System.EventHandler(this.btn_PickerXRightPlace_Click);
			// 
			// btn_PickerXLeftPlace
			// 
			this.btn_PickerXLeftPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXLeftPlace.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXLeftPlace, "btn_PickerXLeftPlace");
			this.btn_PickerXLeftPlace.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXLeftPlace.Name = "btn_PickerXLeftPlace";
			this.btn_PickerXLeftPlace.Tag = "";
			this.btn_PickerXLeftPlace.UseVisualStyleBackColor = false;
			this.btn_PickerXLeftPlace.Click += new System.EventHandler(this.btn_PickerXLeftPlace_Click);
			// 
			// btn_PickerXRailPick
			// 
			this.btn_PickerXRailPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerXRailPick.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerXRailPick, "btn_PickerXRailPick");
			this.btn_PickerXRailPick.ForeColor = System.Drawing.Color.White;
			this.btn_PickerXRailPick.Name = "btn_PickerXRailPick";
			this.btn_PickerXRailPick.Tag = "";
			this.btn_PickerXRailPick.UseVisualStyleBackColor = false;
			this.btn_PickerXRailPick.Click += new System.EventHandler(this.btn_PickerXRailPick_Click);
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
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZRailPick);
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZTblPickPlace);
			this.tab_PagePickerZ.Controls.Add(this.btn_PickerZWait);
			resources.ApplyResources(this.tab_PagePickerZ, "tab_PagePickerZ");
			this.tab_PagePickerZ.Name = "tab_PagePickerZ";
			this.tab_PagePickerZ.UseVisualStyleBackColor = true;
			// 
			// btn_PickerZRailPick
			// 
			this.btn_PickerZRailPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btn_PickerZRailPick.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btn_PickerZRailPick, "btn_PickerZRailPick");
			this.btn_PickerZRailPick.ForeColor = System.Drawing.Color.White;
			this.btn_PickerZRailPick.Name = "btn_PickerZRailPick";
			this.btn_PickerZRailPick.Tag = "";
			this.btn_PickerZRailPick.UseVisualStyleBackColor = false;
			this.btn_PickerZRailPick.Click += new System.EventHandler(this.btn_PickerZRailPick_Click);
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
			// btnOnP_Unit_Check_IV2
			// 
			this.btnOnP_Unit_Check_IV2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.btnOnP_Unit_Check_IV2.FlatAppearance.BorderSize = 0;
			resources.ApplyResources(this.btnOnP_Unit_Check_IV2, "btnOnP_Unit_Check_IV2");
			this.btnOnP_Unit_Check_IV2.ForeColor = System.Drawing.Color.White;
			this.btnOnP_Unit_Check_IV2.Name = "btnOnP_Unit_Check_IV2";
			this.btnOnP_Unit_Check_IV2.Tag = "ONP_IV2";
			this.btnOnP_Unit_Check_IV2.UseVisualStyleBackColor = false;
			this.btnOnP_Unit_Check_IV2.Click += new System.EventHandler(this.btnMan_Cycle_Click);
			// 
			// lblOnP_VacFree
			// 
			this.lblOnP_VacFree.BackColor = System.Drawing.Color.LightGray;
			this.lblOnP_VacFree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblOnP_VacFree, "lblOnP_VacFree");
			this.lblOnP_VacFree.Name = "lblOnP_VacFree";
			this.lblOnP_VacFree.Tag = "Vacuum Free$";
			// 
			// vwMan_04Onp
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.lblOnP_VacFree);
			this.Controls.Add(this.btnOnP_Unit_Check_IV2);
			this.Controls.Add(this.tab_CtrlONP);
			this.Controls.Add(this.btnOnP_BcrOri);
			this.Controls.Add(this.ckbOnP_Clean);
			this.Controls.Add(this.lblOnP_CleanR);
			this.Controls.Add(this.lblOnP_CleanL);
			this.Controls.Add(this.btnOnP_Clean);
			this.Controls.Add(this.pnlOnp_Bcr);
			this.Controls.Add(this.pnlOnp_Y);
			this.Controls.Add(this.btnOnp_Ori);
			this.Controls.Add(this.btnOnp_Bcr);
			this.Controls.Add(this.panel10);
			this.Controls.Add(this.panel14);
			this.Controls.Add(this.ckbOnP_Drn);
			this.Controls.Add(this.lblOnP_LdCell);
			this.Controls.Add(this.lblOnP_Vac);
			this.Controls.Add(this.lblOnP_R90);
			this.Controls.Add(this.lblOnP_R0);
			this.Controls.Add(this.ckbOnP_R0);
			this.Controls.Add(this.ckbOnP_Ejt);
			this.Controls.Add(this.btnOnP_PlaceR);
			this.Controls.Add(this.btnOnP_PlaceL);
			this.Controls.Add(this.btnOnP_PickLTbl);
			this.Controls.Add(this.btnOnP_PickRail);
			this.Controls.Add(this.ckbOnP_Vac);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.btnOnP_H);
			this.Controls.Add(this.btnOnP_Wait);
			resources.ApplyResources(this, "$this");
			this.Name = "vwMan_04Onp";
			this.pnlOnp_Bcr.ResumeLayout(false);
			this.pnlOnp_Y.ResumeLayout(false);
			this.panel10.ResumeLayout(false);
			this.panel14.ResumeLayout(false);
			this.tab_CtrlONP.ResumeLayout(false);
			this.tab_PagePickerX.ResumeLayout(false);
			this.tab_PagePickerZ.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbOnP_Clean;
        private System.Windows.Forms.Label lblOnP_CleanR;
        private System.Windows.Forms.Label lblOnP_CleanL;
        private System.Windows.Forms.Button btnOnP_Clean;
        private System.Windows.Forms.Panel pnlOnp_Bcr;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label lblOnp_Bcr;
        private System.Windows.Forms.Panel pnlOnp_Y;
        private System.Windows.Forms.Label lblOnp_YPos;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label lblOnp_YStat;
        private System.Windows.Forms.Button btnOnp_Ori;
        private System.Windows.Forms.Button btnOnp_Bcr;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label lblOnp_ZPos;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblOnp_ZStat;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Label lblOnp_XPos;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label lblOnp_XStat;
        private System.Windows.Forms.CheckBox ckbOnP_Drn;
        private System.Windows.Forms.Label lblOnP_LdCell;
        private System.Windows.Forms.Label lblOnP_Vac;
        private System.Windows.Forms.Label lblOnP_R90;
        private System.Windows.Forms.Label lblOnP_R0;
        private System.Windows.Forms.CheckBox ckbOnP_R0;
        private System.Windows.Forms.CheckBox ckbOnP_Ejt;
        private System.Windows.Forms.Button btnOnP_PlaceR;
        private System.Windows.Forms.Button btnOnP_PlaceL;
        private System.Windows.Forms.Button btnOnP_PickLTbl;
        private System.Windows.Forms.Button btnOnP_PickRail;
        private System.Windows.Forms.CheckBox ckbOnP_Vac;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btnOnP_H;
        private System.Windows.Forms.Button btnOnP_Wait;
        private System.Windows.Forms.Button btnOnP_BcrOri;
        private System.Windows.Forms.TabControl tab_CtrlONP;
        private System.Windows.Forms.TabPage tab_PagePickerX;
        private System.Windows.Forms.Button btn_PickerXRightPlace;
        private System.Windows.Forms.Button btn_PickerXLeftPlace;
        private System.Windows.Forms.Button btn_PickerXRailPick;
        private System.Windows.Forms.Button btn_PickerXWait;
        private System.Windows.Forms.TabPage tab_PagePickerZ;
        private System.Windows.Forms.Button btn_PickerZRailPick;
        private System.Windows.Forms.Button btn_PickerZTblPickPlace;
        private System.Windows.Forms.Button btn_PickerZWait;
        private System.Windows.Forms.Button btnOnP_Unit_Check_IV2;
		private System.Windows.Forms.Label lblOnP_VacFree;
	}
}
