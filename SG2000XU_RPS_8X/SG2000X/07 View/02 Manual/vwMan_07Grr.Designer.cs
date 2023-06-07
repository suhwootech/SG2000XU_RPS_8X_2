using Extended;

namespace SG2000X
{
    partial class vwMan_07Grr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwMan_07Grr));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblGrR_TSetter = new System.Windows.Forms.Label();
            this.btn_WaterKnife = new System.Windows.Forms.Button();
            this.panel30 = new System.Windows.Forms.Panel();
            this.lblGrR_AAvg = new System.Windows.Forms.Label();
            this.lblGrR_BAvg = new System.Windows.Forms.Label();
            this.lblGrR_AMax = new System.Windows.Forms.Label();
            this.lblGrR_AMin = new System.Windows.Forms.Label();
            this.lblGrR_BMin = new System.Windows.Forms.Label();
            this.lblGrR_BMax = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lbl_ZPos = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lbl_ZStat = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lbl_YPos = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.lbl_YStat = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lbl_XPos = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lbl_XStat = new System.Windows.Forms.Label();
            this.ckbGrR_TcW = new System.Windows.Forms.CheckBox();
            this.ckbGrR_TcAKn = new System.Windows.Forms.CheckBox();
            this.ckbGrR_TcWKn = new System.Windows.Forms.CheckBox();
            this.ckbGrR_TcDn = new System.Windows.Forms.CheckBox();
            this.lblGrR_BtmFlw = new System.Windows.Forms.Label();
            this.lblGrR_GrdFlw = new System.Windows.Forms.Label();
            this.lblGrR_SplPcw = new System.Windows.Forms.Label();
            this.lblGrR_SplZig = new System.Windows.Forms.Label();
            this.lblGrR_PrbAMP = new System.Windows.Forms.Label();
            this.lblGrR_TcFlw = new System.Windows.Forms.Label();
            this.lblGrR_TcDn = new System.Windows.Forms.Label();
            this.lblGrR_TblFlw = new System.Windows.Forms.Label();
            this.lbl_TblVac = new System.Windows.Forms.Label();
            this.btn_Grd = new System.Windows.Forms.Button();
            this.tabGrR_StripInsp = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlGrR_R12 = new System.Windows.Forms.Panel();
            this.lblR_R12Num = new System.Windows.Forms.Label();
            this.lblGrR_R12ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R12One = new System.Windows.Forms.Label();
            this.lblGrR_R12 = new System.Windows.Forms.Label();
            this.lblGrR_R12ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R12CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R12Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R12Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R12Spl = new System.Windows.Forms.Label();
            this.lblGrR_R12Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R11 = new System.Windows.Forms.Panel();
            this.lblR_R11Num = new System.Windows.Forms.Label();
            this.lblGrR_R11ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R11One = new System.Windows.Forms.Label();
            this.lblGrR_R11 = new System.Windows.Forms.Label();
            this.lblGrR_R11ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R11CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R11Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R11Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R11Spl = new System.Windows.Forms.Label();
            this.lblGrR_R11Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R10 = new System.Windows.Forms.Panel();
            this.lblR_R10Num = new System.Windows.Forms.Label();
            this.lblGrR_R10ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R10One = new System.Windows.Forms.Label();
            this.lblGrR_R10 = new System.Windows.Forms.Label();
            this.lblGrR_R10ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R10CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R10Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R10Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R10Spl = new System.Windows.Forms.Label();
            this.lblGrR_R10Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R9 = new System.Windows.Forms.Panel();
            this.lblR_R9Num = new System.Windows.Forms.Label();
            this.lblGrR_R9ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R9One = new System.Windows.Forms.Label();
            this.lblGrR_R9 = new System.Windows.Forms.Label();
            this.lblGrR_R9ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R9CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R9Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R9Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R9Spl = new System.Windows.Forms.Label();
            this.lblGrR_R9Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R8 = new System.Windows.Forms.Panel();
            this.lblR_R8Num = new System.Windows.Forms.Label();
            this.lblGrR_R8ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R8One = new System.Windows.Forms.Label();
            this.lblGrR_R8 = new System.Windows.Forms.Label();
            this.lblGrR_R8ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R8CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R8Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R8Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R8Spl = new System.Windows.Forms.Label();
            this.lblGrR_R8Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R7 = new System.Windows.Forms.Panel();
            this.lblR_R7Num = new System.Windows.Forms.Label();
            this.lblGrR_R7ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R7One = new System.Windows.Forms.Label();
            this.lblGrR_R7 = new System.Windows.Forms.Label();
            this.lblGrR_R7ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R7CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R7Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R7Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R7Spl = new System.Windows.Forms.Label();
            this.lblGrR_R7Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R6 = new System.Windows.Forms.Panel();
            this.lblR_R6Num = new System.Windows.Forms.Label();
            this.lblGrR_R6ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R6One = new System.Windows.Forms.Label();
            this.lblGrR_R6 = new System.Windows.Forms.Label();
            this.lblGrR_R6ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R6CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R6Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R6Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R6Spl = new System.Windows.Forms.Label();
            this.lblGrR_R6Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R5 = new System.Windows.Forms.Panel();
            this.lblR_R5Num = new System.Windows.Forms.Label();
            this.lblGrR_R5ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R5One = new System.Windows.Forms.Label();
            this.lblGrR_R5 = new System.Windows.Forms.Label();
            this.lblGrR_R5ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R5CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R5Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R5Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R5Spl = new System.Windows.Forms.Label();
            this.lblGrR_R5Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R4 = new System.Windows.Forms.Panel();
            this.lblR_R4Num = new System.Windows.Forms.Label();
            this.lblGrR_R4ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R4One = new System.Windows.Forms.Label();
            this.lblGrR_R4 = new System.Windows.Forms.Label();
            this.lblGrR_R4ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R4CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R4Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R4Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R4Spl = new System.Windows.Forms.Label();
            this.lblGrR_R4Tar = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.pnlGrR_Fi = new System.Windows.Forms.Panel();
            this.lblR_FiNum = new System.Windows.Forms.Label();
            this.lblGrR_FiReCnt = new System.Windows.Forms.Label();
            this.lblGrR_FiOne = new System.Windows.Forms.Label();
            this.lblGrR_Fi = new System.Windows.Forms.Label();
            this.lblGrR_FiToTh = new System.Windows.Forms.Label();
            this.lblGrR_FiCyTh = new System.Windows.Forms.Label();
            this.lblGrR_FiTbl = new System.Windows.Forms.Label();
            this.lblGrR_FiCnt = new System.Windows.Forms.Label();
            this.lblGrR_FiSpl = new System.Windows.Forms.Label();
            this.lblGrR_FiTar = new System.Windows.Forms.Label();
            this.pnlGrR_R3 = new System.Windows.Forms.Panel();
            this.lblR_R3Num = new System.Windows.Forms.Label();
            this.lblGrR_R3ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R3One = new System.Windows.Forms.Label();
            this.lblGrR_R3 = new System.Windows.Forms.Label();
            this.lblGrR_R3ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R3CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R3Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R3Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R3Spl = new System.Windows.Forms.Label();
            this.lblGrR_R3Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R2 = new System.Windows.Forms.Panel();
            this.lblR_R2Num = new System.Windows.Forms.Label();
            this.lblGrR_R2ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R2One = new System.Windows.Forms.Label();
            this.lblGrR_R2 = new System.Windows.Forms.Label();
            this.lblGrR_R2ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R2CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R2Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R2Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R2Spl = new System.Windows.Forms.Label();
            this.lblGrR_R2Tar = new System.Windows.Forms.Label();
            this.pnlGrR_R1 = new System.Windows.Forms.Panel();
            this.lblR_R1Num = new System.Windows.Forms.Label();
            this.lblGrR_R1ReCnt = new System.Windows.Forms.Label();
            this.lblGrR_R1One = new System.Windows.Forms.Label();
            this.lblGrR_R1 = new System.Windows.Forms.Label();
            this.lblGrR_R1ToTh = new System.Windows.Forms.Label();
            this.lblGrR_R1CyTh = new System.Windows.Forms.Label();
            this.lblGrR_R1Tbl = new System.Windows.Forms.Label();
            this.lblGrR_R1Cnt = new System.Windows.Forms.Label();
            this.lblGrR_R1Spl = new System.Windows.Forms.Label();
            this.lblGrR_R1Tar = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblGrR_Mod = new System.Windows.Forms.Label();
            this.lblGrR_ModT = new System.Windows.Forms.Label();
            this.tpGrR_Bf = new System.Windows.Forms.TabPage();
            this.dgvGrR_Bf = new ExDataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpGrR_Af = new System.Windows.Forms.TabPage();
            this.dgvGrR_Af = new ExDataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lbl6_DrsAir = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel37 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.lbl6_UDrs2Tot = new System.Windows.Forms.Label();
            this.lbl6_UDrs2Cyl = new System.Windows.Forms.Label();
            this.lbl6_UDrs2Tbl = new System.Windows.Forms.Label();
            this.lbl6_UDrs2Cnt = new System.Windows.Forms.Label();
            this.lbl6_UDrs2Spl = new System.Windows.Forms.Label();
            this.panel39 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.lbl6_UDrs1Tot = new System.Windows.Forms.Label();
            this.lbl6_UDrs1Cyl = new System.Windows.Forms.Label();
            this.lbl6_UDrs1Tbl = new System.Windows.Forms.Label();
            this.lbl6_UDrs1Cnt = new System.Windows.Forms.Label();
            this.lbl6_UDrs1Spl = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.btn_Drs = new System.Windows.Forms.Button();
            this.ckb_Ejt = new System.Windows.Forms.CheckBox();
            this.ckbGrR_Wtr = new System.Windows.Forms.CheckBox();
            this.ckb_Vac = new System.Windows.Forms.CheckBox();
            this.btnGrR_StInsp = new System.Windows.Forms.Button();
            this.btnGrR_DrInsp = new System.Windows.Forms.Button();
            this.btnGrR_WhInsp = new System.Windows.Forms.Button();
            this.btn_H = new System.Windows.Forms.Button();
            this.btn_Wait = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_TbClean = new System.Windows.Forms.Button();
            this.btn_Bcr = new System.Windows.Forms.Button();
            this.lblGrR_Bcr = new System.Windows.Forms.Label();
            this.lbl_Unit4 = new System.Windows.Forms.Label();
            this.lbl_Unit3 = new System.Windows.Forms.Label();
            this.lbl_Unit2 = new System.Windows.Forms.Label();
            this.lbl_Unit1 = new System.Windows.Forms.Label();
            this.panel22 = new System.Windows.Forms.Panel();
            this.lbl_DrsTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_GrdTime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.panel30.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabGrR_StripInsp.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnlGrR_R12.SuspendLayout();
            this.pnlGrR_R11.SuspendLayout();
            this.pnlGrR_R10.SuspendLayout();
            this.pnlGrR_R9.SuspendLayout();
            this.pnlGrR_R8.SuspendLayout();
            this.pnlGrR_R7.SuspendLayout();
            this.pnlGrR_R6.SuspendLayout();
            this.pnlGrR_R5.SuspendLayout();
            this.pnlGrR_R4.SuspendLayout();
            this.pnlGrR_Fi.SuspendLayout();
            this.pnlGrR_R3.SuspendLayout();
            this.pnlGrR_R2.SuspendLayout();
            this.pnlGrR_R1.SuspendLayout();
            this.tpGrR_Bf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrR_Bf)).BeginInit();
            this.tpGrR_Af.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrR_Af)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.panel37.SuspendLayout();
            this.panel39.SuspendLayout();
            this.panel22.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGrR_TSetter
            // 
            this.lblGrR_TSetter.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_TSetter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_TSetter, "lblGrR_TSetter");
            this.lblGrR_TSetter.Name = "lblGrR_TSetter";
            this.lblGrR_TSetter.Tag = "Tool Setter";
            this.lblGrR_TSetter.BackColorChanged += new System.EventHandler(this.lblIO_B_BackColorChanged);
            // 
            // btn_WaterKnife
            // 
            this.btn_WaterKnife.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_WaterKnife.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_WaterKnife, "btn_WaterKnife");
            this.btn_WaterKnife.ForeColor = System.Drawing.Color.White;
            this.btn_WaterKnife.Name = "btn_WaterKnife";
            this.btn_WaterKnife.Tag = "GRR_Water_Knife";
            this.btn_WaterKnife.UseVisualStyleBackColor = false;
            this.btn_WaterKnife.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // panel30
            // 
            this.panel30.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel30.Controls.Add(this.lblGrR_AAvg);
            this.panel30.Controls.Add(this.lblGrR_BAvg);
            this.panel30.Controls.Add(this.lblGrR_AMax);
            this.panel30.Controls.Add(this.lblGrR_AMin);
            this.panel30.Controls.Add(this.lblGrR_BMin);
            this.panel30.Controls.Add(this.lblGrR_BMax);
            this.panel30.Controls.Add(this.label49);
            this.panel30.Controls.Add(this.label61);
            this.panel30.Controls.Add(this.label60);
            this.panel30.Controls.Add(this.label59);
            this.panel30.Controls.Add(this.label57);
            this.panel30.Controls.Add(this.label56);
            resources.ApplyResources(this.panel30, "panel30");
            this.panel30.Name = "panel30";
            this.panel30.Tag = "1";
            // 
            // lblGrR_AAvg
            // 
            this.lblGrR_AAvg.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.lblGrR_AAvg, "lblGrR_AAvg");
            this.lblGrR_AAvg.Name = "lblGrR_AAvg";
            // 
            // lblGrR_BAvg
            // 
            this.lblGrR_BAvg.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.lblGrR_BAvg, "lblGrR_BAvg");
            this.lblGrR_BAvg.Name = "lblGrR_BAvg";
            // 
            // lblGrR_AMax
            // 
            this.lblGrR_AMax.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.lblGrR_AMax, "lblGrR_AMax");
            this.lblGrR_AMax.Name = "lblGrR_AMax";
            // 
            // lblGrR_AMin
            // 
            this.lblGrR_AMin.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.lblGrR_AMin, "lblGrR_AMin");
            this.lblGrR_AMin.Name = "lblGrR_AMin";
            // 
            // lblGrR_BMin
            // 
            this.lblGrR_BMin.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.lblGrR_BMin, "lblGrR_BMin");
            this.lblGrR_BMin.Name = "lblGrR_BMin";
            // 
            // lblGrR_BMax
            // 
            this.lblGrR_BMax.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.lblGrR_BMax, "lblGrR_BMax");
            this.lblGrR_BMax.Name = "lblGrR_BMax";
            // 
            // label49
            // 
            this.label49.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label49, "label49");
            this.label49.ForeColor = System.Drawing.Color.White;
            this.label49.Name = "label49";
            // 
            // label61
            // 
            this.label61.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.label61, "label61");
            this.label61.ForeColor = System.Drawing.Color.White;
            this.label61.Name = "label61";
            // 
            // label60
            // 
            this.label60.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.label60, "label60");
            this.label60.ForeColor = System.Drawing.Color.White;
            this.label60.Name = "label60";
            // 
            // label59
            // 
            this.label59.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.label59, "label59");
            this.label59.Name = "label59";
            // 
            // label57
            // 
            this.label57.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.label57, "label57");
            this.label57.Name = "label57";
            // 
            // label56
            // 
            this.label56.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.label56, "label56");
            this.label56.Name = "label56";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LightGray;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.lbl_ZPos);
            this.panel4.Controls.Add(this.label21);
            this.panel4.Controls.Add(this.lbl_ZStat);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            this.panel4.Tag = "0";
            // 
            // lbl_ZPos
            // 
            this.lbl_ZPos.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lbl_ZPos, "lbl_ZPos");
            this.lbl_ZPos.Name = "lbl_ZPos";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // lbl_ZStat
            // 
            this.lbl_ZStat.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_ZStat, "lbl_ZStat");
            this.lbl_ZStat.Name = "lbl_ZStat";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.LightGray;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.lbl_YPos);
            this.panel5.Controls.Add(this.label26);
            this.panel5.Controls.Add(this.lbl_YStat);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            this.panel5.Tag = "0";
            // 
            // lbl_YPos
            // 
            this.lbl_YPos.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lbl_YPos, "lbl_YPos");
            this.lbl_YPos.Name = "lbl_YPos";
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // lbl_YStat
            // 
            this.lbl_YStat.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_YStat, "lbl_YStat");
            this.lbl_YStat.Name = "lbl_YStat";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.LightGray;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.lbl_XPos);
            this.panel6.Controls.Add(this.label30);
            this.panel6.Controls.Add(this.lbl_XStat);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            this.panel6.Tag = "0";
            // 
            // lbl_XPos
            // 
            this.lbl_XPos.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lbl_XPos, "lbl_XPos");
            this.lbl_XPos.Name = "lbl_XPos";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // lbl_XStat
            // 
            this.lbl_XStat.BackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.lbl_XStat, "lbl_XStat");
            this.lbl_XStat.Name = "lbl_XStat";
            // 
            // ckbGrR_TcW
            // 
            resources.ApplyResources(this.ckbGrR_TcW, "ckbGrR_TcW");
            this.ckbGrR_TcW.BackColor = System.Drawing.Color.LightGray;
            this.ckbGrR_TcW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbGrR_TcW.FlatAppearance.BorderSize = 2;
            this.ckbGrR_TcW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbGrR_TcW.ForeColor = System.Drawing.Color.Black;
            this.ckbGrR_TcW.Name = "ckbGrR_TcW";
            this.ckbGrR_TcW.Tag = "6";
            this.ckbGrR_TcW.UseVisualStyleBackColor = false;
            this.ckbGrR_TcW.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckbGrR_TcW.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckbGrR_TcAKn
            // 
            resources.ApplyResources(this.ckbGrR_TcAKn, "ckbGrR_TcAKn");
            this.ckbGrR_TcAKn.BackColor = System.Drawing.Color.LightGray;
            this.ckbGrR_TcAKn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbGrR_TcAKn.FlatAppearance.BorderSize = 2;
            this.ckbGrR_TcAKn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbGrR_TcAKn.ForeColor = System.Drawing.Color.Black;
            this.ckbGrR_TcAKn.Name = "ckbGrR_TcAKn";
            this.ckbGrR_TcAKn.Tag = "5";
            this.ckbGrR_TcAKn.UseVisualStyleBackColor = false;
            this.ckbGrR_TcAKn.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckbGrR_TcAKn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckbGrR_TcWKn
            // 
            resources.ApplyResources(this.ckbGrR_TcWKn, "ckbGrR_TcWKn");
            this.ckbGrR_TcWKn.BackColor = System.Drawing.Color.LightGray;
            this.ckbGrR_TcWKn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbGrR_TcWKn.FlatAppearance.BorderSize = 2;
            this.ckbGrR_TcWKn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbGrR_TcWKn.ForeColor = System.Drawing.Color.Black;
            this.ckbGrR_TcWKn.Name = "ckbGrR_TcWKn";
            this.ckbGrR_TcWKn.Tag = "4";
            this.ckbGrR_TcWKn.UseVisualStyleBackColor = false;
            this.ckbGrR_TcWKn.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckbGrR_TcWKn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckbGrR_TcDn
            // 
            resources.ApplyResources(this.ckbGrR_TcDn, "ckbGrR_TcDn");
            this.ckbGrR_TcDn.BackColor = System.Drawing.Color.LightGray;
            this.ckbGrR_TcDn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbGrR_TcDn.FlatAppearance.BorderSize = 2;
            this.ckbGrR_TcDn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbGrR_TcDn.ForeColor = System.Drawing.Color.Black;
            this.ckbGrR_TcDn.Name = "ckbGrR_TcDn";
            this.ckbGrR_TcDn.Tag = "3";
            this.ckbGrR_TcDn.UseVisualStyleBackColor = false;
            this.ckbGrR_TcDn.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckbGrR_TcDn.Click += new System.EventHandler(this.ckb_Click);
            // 
            // lblGrR_BtmFlw
            // 
            this.lblGrR_BtmFlw.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_BtmFlw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_BtmFlw, "lblGrR_BtmFlw");
            this.lblGrR_BtmFlw.Name = "lblGrR_BtmFlw";
            this.lblGrR_BtmFlw.Tag = "Bottom Water$Flow";
            this.lblGrR_BtmFlw.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblGrR_GrdFlw
            // 
            this.lblGrR_GrdFlw.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_GrdFlw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_GrdFlw, "lblGrR_GrdFlw");
            this.lblGrR_GrdFlw.Name = "lblGrR_GrdFlw";
            this.lblGrR_GrdFlw.Tag = "Grinding Water$Flow";
            this.lblGrR_GrdFlw.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblGrR_SplPcw
            // 
            this.lblGrR_SplPcw.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_SplPcw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_SplPcw, "lblGrR_SplPcw");
            this.lblGrR_SplPcw.Name = "lblGrR_SplPcw";
            this.lblGrR_SplPcw.Tag = "Spindle PCW$Flow";
            this.lblGrR_SplPcw.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblGrR_SplZig
            // 
            this.lblGrR_SplZig.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_SplZig.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_SplZig, "lblGrR_SplZig");
            this.lblGrR_SplZig.Name = "lblGrR_SplZig";
            this.lblGrR_SplZig.Tag = "Spindle Jig$";
            this.lblGrR_SplZig.BackColorChanged += new System.EventHandler(this.lblIO_B_BackColorChanged);
            // 
            // lblGrR_PrbAMP
            // 
            this.lblGrR_PrbAMP.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_PrbAMP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_PrbAMP, "lblGrR_PrbAMP");
            this.lblGrR_PrbAMP.Name = "lblGrR_PrbAMP";
            this.lblGrR_PrbAMP.Tag = "Probe AMP$Out";
            this.lblGrR_PrbAMP.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblGrR_TcFlw
            // 
            this.lblGrR_TcFlw.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_TcFlw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_TcFlw, "lblGrR_TcFlw");
            this.lblGrR_TcFlw.Name = "lblGrR_TcFlw";
            this.lblGrR_TcFlw.Tag = "Top Cleaner$Water Flow";
            this.lblGrR_TcFlw.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblGrR_TcDn
            // 
            this.lblGrR_TcDn.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_TcDn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_TcDn, "lblGrR_TcDn");
            this.lblGrR_TcDn.Name = "lblGrR_TcDn";
            this.lblGrR_TcDn.Tag = "Top Cleaner$Down";
            this.lblGrR_TcDn.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lblGrR_TblFlw
            // 
            this.lblGrR_TblFlw.BackColor = System.Drawing.Color.LightGray;
            this.lblGrR_TblFlw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_TblFlw, "lblGrR_TblFlw");
            this.lblGrR_TblFlw.Name = "lblGrR_TblFlw";
            this.lblGrR_TblFlw.Tag = "Table Water$Flow";
            this.lblGrR_TblFlw.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // lbl_TblVac
            // 
            this.lbl_TblVac.BackColor = System.Drawing.Color.LightGray;
            this.lbl_TblVac.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_TblVac, "lbl_TblVac");
            this.lbl_TblVac.Name = "lbl_TblVac";
            this.lbl_TblVac.Tag = "Table Vacuum$";
            this.lbl_TblVac.BackColorChanged += new System.EventHandler(this.lblIO_A_BackColorChanged);
            // 
            // btn_Grd
            // 
            this.btn_Grd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Grd.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Grd, "btn_Grd");
            this.btn_Grd.ForeColor = System.Drawing.Color.White;
            this.btn_Grd.Name = "btn_Grd";
            this.btn_Grd.Tag = "GRR_Manual_GrdBcr";
            this.btn_Grd.UseVisualStyleBackColor = false;
            this.btn_Grd.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // tabGrR_StripInsp
            // 
            this.tabGrR_StripInsp.Controls.Add(this.tabPage2);
            this.tabGrR_StripInsp.Controls.Add(this.tpGrR_Bf);
            this.tabGrR_StripInsp.Controls.Add(this.tpGrR_Af);
            this.tabGrR_StripInsp.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tabGrR_StripInsp, "tabGrR_StripInsp");
            this.tabGrR_StripInsp.Multiline = true;
            this.tabGrR_StripInsp.Name = "tabGrR_StripInsp";
            this.tabGrR_StripInsp.SelectedIndex = 0;
            this.tabGrR_StripInsp.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pnlGrR_R12);
            this.tabPage2.Controls.Add(this.pnlGrR_R11);
            this.tabPage2.Controls.Add(this.pnlGrR_R10);
            this.tabPage2.Controls.Add(this.pnlGrR_R9);
            this.tabPage2.Controls.Add(this.pnlGrR_R8);
            this.tabPage2.Controls.Add(this.pnlGrR_R7);
            this.tabPage2.Controls.Add(this.pnlGrR_R6);
            this.tabPage2.Controls.Add(this.pnlGrR_R5);
            this.tabPage2.Controls.Add(this.pnlGrR_R4);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.pnlGrR_Fi);
            this.tabPage2.Controls.Add(this.pnlGrR_R3);
            this.tabPage2.Controls.Add(this.pnlGrR_R2);
            this.tabPage2.Controls.Add(this.pnlGrR_R1);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.lblGrR_Mod);
            this.tabPage2.Controls.Add(this.lblGrR_ModT);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlGrR_R12
            // 
            this.pnlGrR_R12.Controls.Add(this.lblR_R12Num);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12ReCnt);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12One);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12ToTh);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12CyTh);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12Tbl);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12Cnt);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12Spl);
            this.pnlGrR_R12.Controls.Add(this.lblGrR_R12Tar);
            resources.ApplyResources(this.pnlGrR_R12, "pnlGrR_R12");
            this.pnlGrR_R12.Name = "pnlGrR_R12";
            // 
            // lblR_R12Num
            // 
            this.lblR_R12Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R12Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R12Num, "lblR_R12Num");
            this.lblR_R12Num.Name = "lblR_R12Num";
            // 
            // lblGrR_R12ReCnt
            // 
            this.lblGrR_R12ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12ReCnt, "lblGrR_R12ReCnt");
            this.lblGrR_R12ReCnt.Name = "lblGrR_R12ReCnt";
            // 
            // lblGrR_R12One
            // 
            this.lblGrR_R12One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12One, "lblGrR_R12One");
            this.lblGrR_R12One.Name = "lblGrR_R12One";
            // 
            // lblGrR_R12
            // 
            this.lblGrR_R12.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12, "lblGrR_R12");
            this.lblGrR_R12.Name = "lblGrR_R12";
            // 
            // lblGrR_R12ToTh
            // 
            this.lblGrR_R12ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12ToTh, "lblGrR_R12ToTh");
            this.lblGrR_R12ToTh.Name = "lblGrR_R12ToTh";
            // 
            // lblGrR_R12CyTh
            // 
            this.lblGrR_R12CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12CyTh, "lblGrR_R12CyTh");
            this.lblGrR_R12CyTh.Name = "lblGrR_R12CyTh";
            // 
            // lblGrR_R12Tbl
            // 
            this.lblGrR_R12Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12Tbl, "lblGrR_R12Tbl");
            this.lblGrR_R12Tbl.Name = "lblGrR_R12Tbl";
            // 
            // lblGrR_R12Cnt
            // 
            this.lblGrR_R12Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12Cnt, "lblGrR_R12Cnt");
            this.lblGrR_R12Cnt.Name = "lblGrR_R12Cnt";
            // 
            // lblGrR_R12Spl
            // 
            this.lblGrR_R12Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12Spl, "lblGrR_R12Spl");
            this.lblGrR_R12Spl.Name = "lblGrR_R12Spl";
            // 
            // lblGrR_R12Tar
            // 
            this.lblGrR_R12Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R12Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R12Tar, "lblGrR_R12Tar");
            this.lblGrR_R12Tar.Name = "lblGrR_R12Tar";
            // 
            // pnlGrR_R11
            // 
            this.pnlGrR_R11.Controls.Add(this.lblR_R11Num);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11ReCnt);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11One);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11ToTh);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11CyTh);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11Tbl);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11Cnt);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11Spl);
            this.pnlGrR_R11.Controls.Add(this.lblGrR_R11Tar);
            resources.ApplyResources(this.pnlGrR_R11, "pnlGrR_R11");
            this.pnlGrR_R11.Name = "pnlGrR_R11";
            // 
            // lblR_R11Num
            // 
            this.lblR_R11Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R11Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R11Num, "lblR_R11Num");
            this.lblR_R11Num.Name = "lblR_R11Num";
            // 
            // lblGrR_R11ReCnt
            // 
            this.lblGrR_R11ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11ReCnt, "lblGrR_R11ReCnt");
            this.lblGrR_R11ReCnt.Name = "lblGrR_R11ReCnt";
            // 
            // lblGrR_R11One
            // 
            this.lblGrR_R11One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11One, "lblGrR_R11One");
            this.lblGrR_R11One.Name = "lblGrR_R11One";
            // 
            // lblGrR_R11
            // 
            this.lblGrR_R11.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11, "lblGrR_R11");
            this.lblGrR_R11.Name = "lblGrR_R11";
            // 
            // lblGrR_R11ToTh
            // 
            this.lblGrR_R11ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11ToTh, "lblGrR_R11ToTh");
            this.lblGrR_R11ToTh.Name = "lblGrR_R11ToTh";
            // 
            // lblGrR_R11CyTh
            // 
            this.lblGrR_R11CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11CyTh, "lblGrR_R11CyTh");
            this.lblGrR_R11CyTh.Name = "lblGrR_R11CyTh";
            // 
            // lblGrR_R11Tbl
            // 
            this.lblGrR_R11Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11Tbl, "lblGrR_R11Tbl");
            this.lblGrR_R11Tbl.Name = "lblGrR_R11Tbl";
            // 
            // lblGrR_R11Cnt
            // 
            this.lblGrR_R11Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11Cnt, "lblGrR_R11Cnt");
            this.lblGrR_R11Cnt.Name = "lblGrR_R11Cnt";
            // 
            // lblGrR_R11Spl
            // 
            this.lblGrR_R11Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11Spl, "lblGrR_R11Spl");
            this.lblGrR_R11Spl.Name = "lblGrR_R11Spl";
            // 
            // lblGrR_R11Tar
            // 
            this.lblGrR_R11Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R11Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R11Tar, "lblGrR_R11Tar");
            this.lblGrR_R11Tar.Name = "lblGrR_R11Tar";
            // 
            // pnlGrR_R10
            // 
            this.pnlGrR_R10.Controls.Add(this.lblR_R10Num);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10ReCnt);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10One);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10ToTh);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10CyTh);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10Tbl);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10Cnt);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10Spl);
            this.pnlGrR_R10.Controls.Add(this.lblGrR_R10Tar);
            resources.ApplyResources(this.pnlGrR_R10, "pnlGrR_R10");
            this.pnlGrR_R10.Name = "pnlGrR_R10";
            // 
            // lblR_R10Num
            // 
            this.lblR_R10Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R10Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R10Num, "lblR_R10Num");
            this.lblR_R10Num.Name = "lblR_R10Num";
            // 
            // lblGrR_R10ReCnt
            // 
            this.lblGrR_R10ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10ReCnt, "lblGrR_R10ReCnt");
            this.lblGrR_R10ReCnt.Name = "lblGrR_R10ReCnt";
            // 
            // lblGrR_R10One
            // 
            this.lblGrR_R10One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10One, "lblGrR_R10One");
            this.lblGrR_R10One.Name = "lblGrR_R10One";
            // 
            // lblGrR_R10
            // 
            this.lblGrR_R10.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10, "lblGrR_R10");
            this.lblGrR_R10.Name = "lblGrR_R10";
            // 
            // lblGrR_R10ToTh
            // 
            this.lblGrR_R10ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10ToTh, "lblGrR_R10ToTh");
            this.lblGrR_R10ToTh.Name = "lblGrR_R10ToTh";
            // 
            // lblGrR_R10CyTh
            // 
            this.lblGrR_R10CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10CyTh, "lblGrR_R10CyTh");
            this.lblGrR_R10CyTh.Name = "lblGrR_R10CyTh";
            // 
            // lblGrR_R10Tbl
            // 
            this.lblGrR_R10Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10Tbl, "lblGrR_R10Tbl");
            this.lblGrR_R10Tbl.Name = "lblGrR_R10Tbl";
            // 
            // lblGrR_R10Cnt
            // 
            this.lblGrR_R10Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10Cnt, "lblGrR_R10Cnt");
            this.lblGrR_R10Cnt.Name = "lblGrR_R10Cnt";
            // 
            // lblGrR_R10Spl
            // 
            this.lblGrR_R10Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10Spl, "lblGrR_R10Spl");
            this.lblGrR_R10Spl.Name = "lblGrR_R10Spl";
            // 
            // lblGrR_R10Tar
            // 
            this.lblGrR_R10Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R10Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R10Tar, "lblGrR_R10Tar");
            this.lblGrR_R10Tar.Name = "lblGrR_R10Tar";
            // 
            // pnlGrR_R9
            // 
            this.pnlGrR_R9.Controls.Add(this.lblR_R9Num);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9ReCnt);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9One);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9ToTh);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9CyTh);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9Tbl);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9Cnt);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9Spl);
            this.pnlGrR_R9.Controls.Add(this.lblGrR_R9Tar);
            resources.ApplyResources(this.pnlGrR_R9, "pnlGrR_R9");
            this.pnlGrR_R9.Name = "pnlGrR_R9";
            // 
            // lblR_R9Num
            // 
            this.lblR_R9Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R9Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R9Num, "lblR_R9Num");
            this.lblR_R9Num.Name = "lblR_R9Num";
            // 
            // lblGrR_R9ReCnt
            // 
            this.lblGrR_R9ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9ReCnt, "lblGrR_R9ReCnt");
            this.lblGrR_R9ReCnt.Name = "lblGrR_R9ReCnt";
            // 
            // lblGrR_R9One
            // 
            this.lblGrR_R9One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9One, "lblGrR_R9One");
            this.lblGrR_R9One.Name = "lblGrR_R9One";
            // 
            // lblGrR_R9
            // 
            this.lblGrR_R9.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9, "lblGrR_R9");
            this.lblGrR_R9.Name = "lblGrR_R9";
            // 
            // lblGrR_R9ToTh
            // 
            this.lblGrR_R9ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9ToTh, "lblGrR_R9ToTh");
            this.lblGrR_R9ToTh.Name = "lblGrR_R9ToTh";
            // 
            // lblGrR_R9CyTh
            // 
            this.lblGrR_R9CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9CyTh, "lblGrR_R9CyTh");
            this.lblGrR_R9CyTh.Name = "lblGrR_R9CyTh";
            // 
            // lblGrR_R9Tbl
            // 
            this.lblGrR_R9Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9Tbl, "lblGrR_R9Tbl");
            this.lblGrR_R9Tbl.Name = "lblGrR_R9Tbl";
            // 
            // lblGrR_R9Cnt
            // 
            this.lblGrR_R9Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9Cnt, "lblGrR_R9Cnt");
            this.lblGrR_R9Cnt.Name = "lblGrR_R9Cnt";
            // 
            // lblGrR_R9Spl
            // 
            this.lblGrR_R9Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9Spl, "lblGrR_R9Spl");
            this.lblGrR_R9Spl.Name = "lblGrR_R9Spl";
            // 
            // lblGrR_R9Tar
            // 
            this.lblGrR_R9Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R9Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R9Tar, "lblGrR_R9Tar");
            this.lblGrR_R9Tar.Name = "lblGrR_R9Tar";
            // 
            // pnlGrR_R8
            // 
            this.pnlGrR_R8.Controls.Add(this.lblR_R8Num);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8ReCnt);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8One);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8ToTh);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8CyTh);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8Tbl);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8Cnt);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8Spl);
            this.pnlGrR_R8.Controls.Add(this.lblGrR_R8Tar);
            resources.ApplyResources(this.pnlGrR_R8, "pnlGrR_R8");
            this.pnlGrR_R8.Name = "pnlGrR_R8";
            // 
            // lblR_R8Num
            // 
            this.lblR_R8Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R8Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R8Num, "lblR_R8Num");
            this.lblR_R8Num.Name = "lblR_R8Num";
            // 
            // lblGrR_R8ReCnt
            // 
            this.lblGrR_R8ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8ReCnt, "lblGrR_R8ReCnt");
            this.lblGrR_R8ReCnt.Name = "lblGrR_R8ReCnt";
            // 
            // lblGrR_R8One
            // 
            this.lblGrR_R8One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8One, "lblGrR_R8One");
            this.lblGrR_R8One.Name = "lblGrR_R8One";
            // 
            // lblGrR_R8
            // 
            this.lblGrR_R8.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8, "lblGrR_R8");
            this.lblGrR_R8.Name = "lblGrR_R8";
            // 
            // lblGrR_R8ToTh
            // 
            this.lblGrR_R8ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8ToTh, "lblGrR_R8ToTh");
            this.lblGrR_R8ToTh.Name = "lblGrR_R8ToTh";
            // 
            // lblGrR_R8CyTh
            // 
            this.lblGrR_R8CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8CyTh, "lblGrR_R8CyTh");
            this.lblGrR_R8CyTh.Name = "lblGrR_R8CyTh";
            // 
            // lblGrR_R8Tbl
            // 
            this.lblGrR_R8Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8Tbl, "lblGrR_R8Tbl");
            this.lblGrR_R8Tbl.Name = "lblGrR_R8Tbl";
            // 
            // lblGrR_R8Cnt
            // 
            this.lblGrR_R8Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8Cnt, "lblGrR_R8Cnt");
            this.lblGrR_R8Cnt.Name = "lblGrR_R8Cnt";
            // 
            // lblGrR_R8Spl
            // 
            this.lblGrR_R8Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8Spl, "lblGrR_R8Spl");
            this.lblGrR_R8Spl.Name = "lblGrR_R8Spl";
            // 
            // lblGrR_R8Tar
            // 
            this.lblGrR_R8Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R8Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R8Tar, "lblGrR_R8Tar");
            this.lblGrR_R8Tar.Name = "lblGrR_R8Tar";
            // 
            // pnlGrR_R7
            // 
            this.pnlGrR_R7.Controls.Add(this.lblR_R7Num);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7ReCnt);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7One);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7ToTh);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7CyTh);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7Tbl);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7Cnt);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7Spl);
            this.pnlGrR_R7.Controls.Add(this.lblGrR_R7Tar);
            resources.ApplyResources(this.pnlGrR_R7, "pnlGrR_R7");
            this.pnlGrR_R7.Name = "pnlGrR_R7";
            // 
            // lblR_R7Num
            // 
            this.lblR_R7Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R7Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R7Num, "lblR_R7Num");
            this.lblR_R7Num.Name = "lblR_R7Num";
            // 
            // lblGrR_R7ReCnt
            // 
            this.lblGrR_R7ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7ReCnt, "lblGrR_R7ReCnt");
            this.lblGrR_R7ReCnt.Name = "lblGrR_R7ReCnt";
            // 
            // lblGrR_R7One
            // 
            this.lblGrR_R7One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7One, "lblGrR_R7One");
            this.lblGrR_R7One.Name = "lblGrR_R7One";
            // 
            // lblGrR_R7
            // 
            this.lblGrR_R7.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7, "lblGrR_R7");
            this.lblGrR_R7.Name = "lblGrR_R7";
            // 
            // lblGrR_R7ToTh
            // 
            this.lblGrR_R7ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7ToTh, "lblGrR_R7ToTh");
            this.lblGrR_R7ToTh.Name = "lblGrR_R7ToTh";
            // 
            // lblGrR_R7CyTh
            // 
            this.lblGrR_R7CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7CyTh, "lblGrR_R7CyTh");
            this.lblGrR_R7CyTh.Name = "lblGrR_R7CyTh";
            // 
            // lblGrR_R7Tbl
            // 
            this.lblGrR_R7Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7Tbl, "lblGrR_R7Tbl");
            this.lblGrR_R7Tbl.Name = "lblGrR_R7Tbl";
            // 
            // lblGrR_R7Cnt
            // 
            this.lblGrR_R7Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7Cnt, "lblGrR_R7Cnt");
            this.lblGrR_R7Cnt.Name = "lblGrR_R7Cnt";
            // 
            // lblGrR_R7Spl
            // 
            this.lblGrR_R7Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7Spl, "lblGrR_R7Spl");
            this.lblGrR_R7Spl.Name = "lblGrR_R7Spl";
            // 
            // lblGrR_R7Tar
            // 
            this.lblGrR_R7Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R7Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R7Tar, "lblGrR_R7Tar");
            this.lblGrR_R7Tar.Name = "lblGrR_R7Tar";
            // 
            // pnlGrR_R6
            // 
            this.pnlGrR_R6.Controls.Add(this.lblR_R6Num);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6ReCnt);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6One);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6ToTh);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6CyTh);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6Tbl);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6Cnt);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6Spl);
            this.pnlGrR_R6.Controls.Add(this.lblGrR_R6Tar);
            resources.ApplyResources(this.pnlGrR_R6, "pnlGrR_R6");
            this.pnlGrR_R6.Name = "pnlGrR_R6";
            // 
            // lblR_R6Num
            // 
            this.lblR_R6Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R6Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R6Num, "lblR_R6Num");
            this.lblR_R6Num.Name = "lblR_R6Num";
            // 
            // lblGrR_R6ReCnt
            // 
            this.lblGrR_R6ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6ReCnt, "lblGrR_R6ReCnt");
            this.lblGrR_R6ReCnt.Name = "lblGrR_R6ReCnt";
            // 
            // lblGrR_R6One
            // 
            this.lblGrR_R6One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6One, "lblGrR_R6One");
            this.lblGrR_R6One.Name = "lblGrR_R6One";
            // 
            // lblGrR_R6
            // 
            this.lblGrR_R6.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6, "lblGrR_R6");
            this.lblGrR_R6.Name = "lblGrR_R6";
            // 
            // lblGrR_R6ToTh
            // 
            this.lblGrR_R6ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6ToTh, "lblGrR_R6ToTh");
            this.lblGrR_R6ToTh.Name = "lblGrR_R6ToTh";
            // 
            // lblGrR_R6CyTh
            // 
            this.lblGrR_R6CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6CyTh, "lblGrR_R6CyTh");
            this.lblGrR_R6CyTh.Name = "lblGrR_R6CyTh";
            // 
            // lblGrR_R6Tbl
            // 
            this.lblGrR_R6Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6Tbl, "lblGrR_R6Tbl");
            this.lblGrR_R6Tbl.Name = "lblGrR_R6Tbl";
            // 
            // lblGrR_R6Cnt
            // 
            this.lblGrR_R6Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6Cnt, "lblGrR_R6Cnt");
            this.lblGrR_R6Cnt.Name = "lblGrR_R6Cnt";
            // 
            // lblGrR_R6Spl
            // 
            this.lblGrR_R6Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6Spl, "lblGrR_R6Spl");
            this.lblGrR_R6Spl.Name = "lblGrR_R6Spl";
            // 
            // lblGrR_R6Tar
            // 
            this.lblGrR_R6Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R6Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R6Tar, "lblGrR_R6Tar");
            this.lblGrR_R6Tar.Name = "lblGrR_R6Tar";
            // 
            // pnlGrR_R5
            // 
            this.pnlGrR_R5.Controls.Add(this.lblR_R5Num);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5ReCnt);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5One);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5ToTh);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5CyTh);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5Tbl);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5Cnt);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5Spl);
            this.pnlGrR_R5.Controls.Add(this.lblGrR_R5Tar);
            resources.ApplyResources(this.pnlGrR_R5, "pnlGrR_R5");
            this.pnlGrR_R5.Name = "pnlGrR_R5";
            // 
            // lblR_R5Num
            // 
            this.lblR_R5Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R5Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R5Num, "lblR_R5Num");
            this.lblR_R5Num.Name = "lblR_R5Num";
            // 
            // lblGrR_R5ReCnt
            // 
            this.lblGrR_R5ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5ReCnt, "lblGrR_R5ReCnt");
            this.lblGrR_R5ReCnt.Name = "lblGrR_R5ReCnt";
            // 
            // lblGrR_R5One
            // 
            this.lblGrR_R5One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5One, "lblGrR_R5One");
            this.lblGrR_R5One.Name = "lblGrR_R5One";
            // 
            // lblGrR_R5
            // 
            this.lblGrR_R5.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5, "lblGrR_R5");
            this.lblGrR_R5.Name = "lblGrR_R5";
            // 
            // lblGrR_R5ToTh
            // 
            this.lblGrR_R5ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5ToTh, "lblGrR_R5ToTh");
            this.lblGrR_R5ToTh.Name = "lblGrR_R5ToTh";
            // 
            // lblGrR_R5CyTh
            // 
            this.lblGrR_R5CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5CyTh, "lblGrR_R5CyTh");
            this.lblGrR_R5CyTh.Name = "lblGrR_R5CyTh";
            // 
            // lblGrR_R5Tbl
            // 
            this.lblGrR_R5Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5Tbl, "lblGrR_R5Tbl");
            this.lblGrR_R5Tbl.Name = "lblGrR_R5Tbl";
            // 
            // lblGrR_R5Cnt
            // 
            this.lblGrR_R5Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5Cnt, "lblGrR_R5Cnt");
            this.lblGrR_R5Cnt.Name = "lblGrR_R5Cnt";
            // 
            // lblGrR_R5Spl
            // 
            this.lblGrR_R5Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5Spl, "lblGrR_R5Spl");
            this.lblGrR_R5Spl.Name = "lblGrR_R5Spl";
            // 
            // lblGrR_R5Tar
            // 
            this.lblGrR_R5Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R5Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R5Tar, "lblGrR_R5Tar");
            this.lblGrR_R5Tar.Name = "lblGrR_R5Tar";
            // 
            // pnlGrR_R4
            // 
            this.pnlGrR_R4.Controls.Add(this.lblR_R4Num);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4ReCnt);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4One);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4ToTh);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4CyTh);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4Tbl);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4Cnt);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4Spl);
            this.pnlGrR_R4.Controls.Add(this.lblGrR_R4Tar);
            resources.ApplyResources(this.pnlGrR_R4, "pnlGrR_R4");
            this.pnlGrR_R4.Name = "pnlGrR_R4";
            // 
            // lblR_R4Num
            // 
            this.lblR_R4Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R4Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R4Num, "lblR_R4Num");
            this.lblR_R4Num.Name = "lblR_R4Num";
            // 
            // lblGrR_R4ReCnt
            // 
            this.lblGrR_R4ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4ReCnt, "lblGrR_R4ReCnt");
            this.lblGrR_R4ReCnt.Name = "lblGrR_R4ReCnt";
            // 
            // lblGrR_R4One
            // 
            this.lblGrR_R4One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4One, "lblGrR_R4One");
            this.lblGrR_R4One.Name = "lblGrR_R4One";
            // 
            // lblGrR_R4
            // 
            this.lblGrR_R4.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4, "lblGrR_R4");
            this.lblGrR_R4.Name = "lblGrR_R4";
            // 
            // lblGrR_R4ToTh
            // 
            this.lblGrR_R4ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4ToTh, "lblGrR_R4ToTh");
            this.lblGrR_R4ToTh.Name = "lblGrR_R4ToTh";
            // 
            // lblGrR_R4CyTh
            // 
            this.lblGrR_R4CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4CyTh, "lblGrR_R4CyTh");
            this.lblGrR_R4CyTh.Name = "lblGrR_R4CyTh";
            // 
            // lblGrR_R4Tbl
            // 
            this.lblGrR_R4Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4Tbl, "lblGrR_R4Tbl");
            this.lblGrR_R4Tbl.Name = "lblGrR_R4Tbl";
            // 
            // lblGrR_R4Cnt
            // 
            this.lblGrR_R4Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4Cnt, "lblGrR_R4Cnt");
            this.lblGrR_R4Cnt.Name = "lblGrR_R4Cnt";
            // 
            // lblGrR_R4Spl
            // 
            this.lblGrR_R4Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4Spl, "lblGrR_R4Spl");
            this.lblGrR_R4Spl.Name = "lblGrR_R4Spl";
            // 
            // lblGrR_R4Tar
            // 
            this.lblGrR_R4Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R4Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R4Tar, "lblGrR_R4Tar");
            this.lblGrR_R4Tar.Name = "lblGrR_R4Tar";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // pnlGrR_Fi
            // 
            this.pnlGrR_Fi.Controls.Add(this.lblR_FiNum);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiReCnt);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiOne);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_Fi);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiToTh);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiCyTh);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiTbl);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiCnt);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiSpl);
            this.pnlGrR_Fi.Controls.Add(this.lblGrR_FiTar);
            resources.ApplyResources(this.pnlGrR_Fi, "pnlGrR_Fi");
            this.pnlGrR_Fi.Name = "pnlGrR_Fi";
            // 
            // lblR_FiNum
            // 
            this.lblR_FiNum.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_FiNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_FiNum, "lblR_FiNum");
            this.lblR_FiNum.Name = "lblR_FiNum";
            // 
            // lblGrR_FiReCnt
            // 
            this.lblGrR_FiReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiReCnt, "lblGrR_FiReCnt");
            this.lblGrR_FiReCnt.Name = "lblGrR_FiReCnt";
            // 
            // lblGrR_FiOne
            // 
            this.lblGrR_FiOne.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiOne.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiOne, "lblGrR_FiOne");
            this.lblGrR_FiOne.Name = "lblGrR_FiOne";
            // 
            // lblGrR_Fi
            // 
            this.lblGrR_Fi.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_Fi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_Fi, "lblGrR_Fi");
            this.lblGrR_Fi.Name = "lblGrR_Fi";
            // 
            // lblGrR_FiToTh
            // 
            this.lblGrR_FiToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiToTh, "lblGrR_FiToTh");
            this.lblGrR_FiToTh.Name = "lblGrR_FiToTh";
            // 
            // lblGrR_FiCyTh
            // 
            this.lblGrR_FiCyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiCyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiCyTh, "lblGrR_FiCyTh");
            this.lblGrR_FiCyTh.Name = "lblGrR_FiCyTh";
            // 
            // lblGrR_FiTbl
            // 
            this.lblGrR_FiTbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiTbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiTbl, "lblGrR_FiTbl");
            this.lblGrR_FiTbl.Name = "lblGrR_FiTbl";
            // 
            // lblGrR_FiCnt
            // 
            this.lblGrR_FiCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiCnt, "lblGrR_FiCnt");
            this.lblGrR_FiCnt.Name = "lblGrR_FiCnt";
            // 
            // lblGrR_FiSpl
            // 
            this.lblGrR_FiSpl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiSpl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiSpl, "lblGrR_FiSpl");
            this.lblGrR_FiSpl.Name = "lblGrR_FiSpl";
            // 
            // lblGrR_FiTar
            // 
            this.lblGrR_FiTar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_FiTar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_FiTar, "lblGrR_FiTar");
            this.lblGrR_FiTar.Name = "lblGrR_FiTar";
            // 
            // pnlGrR_R3
            // 
            this.pnlGrR_R3.Controls.Add(this.lblR_R3Num);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3ReCnt);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3One);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3ToTh);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3CyTh);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3Tbl);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3Cnt);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3Spl);
            this.pnlGrR_R3.Controls.Add(this.lblGrR_R3Tar);
            resources.ApplyResources(this.pnlGrR_R3, "pnlGrR_R3");
            this.pnlGrR_R3.Name = "pnlGrR_R3";
            // 
            // lblR_R3Num
            // 
            this.lblR_R3Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R3Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R3Num, "lblR_R3Num");
            this.lblR_R3Num.Name = "lblR_R3Num";
            // 
            // lblGrR_R3ReCnt
            // 
            this.lblGrR_R3ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3ReCnt, "lblGrR_R3ReCnt");
            this.lblGrR_R3ReCnt.Name = "lblGrR_R3ReCnt";
            // 
            // lblGrR_R3One
            // 
            this.lblGrR_R3One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3One, "lblGrR_R3One");
            this.lblGrR_R3One.Name = "lblGrR_R3One";
            // 
            // lblGrR_R3
            // 
            this.lblGrR_R3.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3, "lblGrR_R3");
            this.lblGrR_R3.Name = "lblGrR_R3";
            // 
            // lblGrR_R3ToTh
            // 
            this.lblGrR_R3ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3ToTh, "lblGrR_R3ToTh");
            this.lblGrR_R3ToTh.Name = "lblGrR_R3ToTh";
            // 
            // lblGrR_R3CyTh
            // 
            this.lblGrR_R3CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3CyTh, "lblGrR_R3CyTh");
            this.lblGrR_R3CyTh.Name = "lblGrR_R3CyTh";
            // 
            // lblGrR_R3Tbl
            // 
            this.lblGrR_R3Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3Tbl, "lblGrR_R3Tbl");
            this.lblGrR_R3Tbl.Name = "lblGrR_R3Tbl";
            // 
            // lblGrR_R3Cnt
            // 
            this.lblGrR_R3Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3Cnt, "lblGrR_R3Cnt");
            this.lblGrR_R3Cnt.Name = "lblGrR_R3Cnt";
            // 
            // lblGrR_R3Spl
            // 
            this.lblGrR_R3Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3Spl, "lblGrR_R3Spl");
            this.lblGrR_R3Spl.Name = "lblGrR_R3Spl";
            // 
            // lblGrR_R3Tar
            // 
            this.lblGrR_R3Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R3Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R3Tar, "lblGrR_R3Tar");
            this.lblGrR_R3Tar.Name = "lblGrR_R3Tar";
            // 
            // pnlGrR_R2
            // 
            this.pnlGrR_R2.Controls.Add(this.lblR_R2Num);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2ReCnt);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2One);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2ToTh);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2CyTh);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2Tbl);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2Cnt);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2Spl);
            this.pnlGrR_R2.Controls.Add(this.lblGrR_R2Tar);
            resources.ApplyResources(this.pnlGrR_R2, "pnlGrR_R2");
            this.pnlGrR_R2.Name = "pnlGrR_R2";
            // 
            // lblR_R2Num
            // 
            this.lblR_R2Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R2Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R2Num, "lblR_R2Num");
            this.lblR_R2Num.Name = "lblR_R2Num";
            // 
            // lblGrR_R2ReCnt
            // 
            this.lblGrR_R2ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2ReCnt, "lblGrR_R2ReCnt");
            this.lblGrR_R2ReCnt.Name = "lblGrR_R2ReCnt";
            // 
            // lblGrR_R2One
            // 
            this.lblGrR_R2One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2One, "lblGrR_R2One");
            this.lblGrR_R2One.Name = "lblGrR_R2One";
            // 
            // lblGrR_R2
            // 
            this.lblGrR_R2.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2, "lblGrR_R2");
            this.lblGrR_R2.Name = "lblGrR_R2";
            // 
            // lblGrR_R2ToTh
            // 
            this.lblGrR_R2ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2ToTh, "lblGrR_R2ToTh");
            this.lblGrR_R2ToTh.Name = "lblGrR_R2ToTh";
            // 
            // lblGrR_R2CyTh
            // 
            this.lblGrR_R2CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2CyTh, "lblGrR_R2CyTh");
            this.lblGrR_R2CyTh.Name = "lblGrR_R2CyTh";
            // 
            // lblGrR_R2Tbl
            // 
            this.lblGrR_R2Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2Tbl, "lblGrR_R2Tbl");
            this.lblGrR_R2Tbl.Name = "lblGrR_R2Tbl";
            // 
            // lblGrR_R2Cnt
            // 
            this.lblGrR_R2Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2Cnt, "lblGrR_R2Cnt");
            this.lblGrR_R2Cnt.Name = "lblGrR_R2Cnt";
            // 
            // lblGrR_R2Spl
            // 
            this.lblGrR_R2Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2Spl, "lblGrR_R2Spl");
            this.lblGrR_R2Spl.Name = "lblGrR_R2Spl";
            // 
            // lblGrR_R2Tar
            // 
            this.lblGrR_R2Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R2Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R2Tar, "lblGrR_R2Tar");
            this.lblGrR_R2Tar.Name = "lblGrR_R2Tar";
            // 
            // pnlGrR_R1
            // 
            this.pnlGrR_R1.Controls.Add(this.lblR_R1Num);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1ReCnt);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1One);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1ToTh);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1CyTh);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1Tbl);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1Cnt);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1Spl);
            this.pnlGrR_R1.Controls.Add(this.lblGrR_R1Tar);
            resources.ApplyResources(this.pnlGrR_R1, "pnlGrR_R1");
            this.pnlGrR_R1.Name = "pnlGrR_R1";
            // 
            // lblR_R1Num
            // 
            this.lblR_R1Num.BackColor = System.Drawing.Color.Gainsboro;
            this.lblR_R1Num.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblR_R1Num, "lblR_R1Num");
            this.lblR_R1Num.Name = "lblR_R1Num";
            // 
            // lblGrR_R1ReCnt
            // 
            this.lblGrR_R1ReCnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1ReCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1ReCnt, "lblGrR_R1ReCnt");
            this.lblGrR_R1ReCnt.Name = "lblGrR_R1ReCnt";
            // 
            // lblGrR_R1One
            // 
            this.lblGrR_R1One.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1One.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1One, "lblGrR_R1One");
            this.lblGrR_R1One.Name = "lblGrR_R1One";
            // 
            // lblGrR_R1
            // 
            this.lblGrR_R1.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1, "lblGrR_R1");
            this.lblGrR_R1.Name = "lblGrR_R1";
            // 
            // lblGrR_R1ToTh
            // 
            this.lblGrR_R1ToTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1ToTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1ToTh, "lblGrR_R1ToTh");
            this.lblGrR_R1ToTh.Name = "lblGrR_R1ToTh";
            // 
            // lblGrR_R1CyTh
            // 
            this.lblGrR_R1CyTh.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1CyTh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1CyTh, "lblGrR_R1CyTh");
            this.lblGrR_R1CyTh.Name = "lblGrR_R1CyTh";
            // 
            // lblGrR_R1Tbl
            // 
            this.lblGrR_R1Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1Tbl, "lblGrR_R1Tbl");
            this.lblGrR_R1Tbl.Name = "lblGrR_R1Tbl";
            // 
            // lblGrR_R1Cnt
            // 
            this.lblGrR_R1Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1Cnt, "lblGrR_R1Cnt");
            this.lblGrR_R1Cnt.Name = "lblGrR_R1Cnt";
            // 
            // lblGrR_R1Spl
            // 
            this.lblGrR_R1Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1Spl, "lblGrR_R1Spl");
            this.lblGrR_R1Spl.Name = "lblGrR_R1Spl";
            // 
            // lblGrR_R1Tar
            // 
            this.lblGrR_R1Tar.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_R1Tar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_R1Tar, "lblGrR_R1Tar");
            this.lblGrR_R1Tar.Name = "lblGrR_R1Tar";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // lblGrR_Mod
            // 
            this.lblGrR_Mod.BackColor = System.Drawing.Color.Gainsboro;
            this.lblGrR_Mod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lblGrR_Mod, "lblGrR_Mod");
            this.lblGrR_Mod.Name = "lblGrR_Mod";
            // 
            // lblGrR_ModT
            // 
            resources.ApplyResources(this.lblGrR_ModT, "lblGrR_ModT");
            this.lblGrR_ModT.Name = "lblGrR_ModT";
            // 
            // tpGrR_Bf
            // 
            this.tpGrR_Bf.BackColor = System.Drawing.Color.White;
            this.tpGrR_Bf.Controls.Add(this.dgvGrR_Bf);
            resources.ApplyResources(this.tpGrR_Bf, "tpGrR_Bf");
            this.tpGrR_Bf.Name = "tpGrR_Bf";
            this.tpGrR_Bf.Tag = "0";
            // 
            // dgvGrR_Bf
            // 
            this.dgvGrR_Bf.AllowUserToAddRows = false;
            this.dgvGrR_Bf.AllowUserToDeleteRows = false;
            this.dgvGrR_Bf.AllowUserToResizeColumns = false;
            this.dgvGrR_Bf.AllowUserToResizeRows = false;
            this.dgvGrR_Bf.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGrR_Bf.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvGrR_Bf.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvGrR_Bf.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dgvGrR_Bf, "dgvGrR_Bf");
            this.dgvGrR_Bf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvGrR_Bf.ColumnHeadersVisible = false;
            this.dgvGrR_Bf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.dgvGrR_Bf.Name = "dgvGrR_Bf";
            this.dgvGrR_Bf.ReadOnly = true;
            this.dgvGrR_Bf.RowHeadersVisible = false;
            this.dgvGrR_Bf.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 7F);
            this.dgvGrR_Bf.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvGrR_Bf.RowTemplate.ReadOnly = true;
            this.dgvGrR_Bf.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvGrR_Bf.Tag = "0";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // tpGrR_Af
            // 
            this.tpGrR_Af.BackColor = System.Drawing.Color.White;
            this.tpGrR_Af.Controls.Add(this.dgvGrR_Af);
            resources.ApplyResources(this.tpGrR_Af, "tpGrR_Af");
            this.tpGrR_Af.Name = "tpGrR_Af";
            this.tpGrR_Af.Tag = "1";
            // 
            // dgvGrR_Af
            // 
            this.dgvGrR_Af.AllowUserToAddRows = false;
            this.dgvGrR_Af.AllowUserToDeleteRows = false;
            this.dgvGrR_Af.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGrR_Af.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvGrR_Af.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvGrR_Af.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.dgvGrR_Af, "dgvGrR_Af");
            this.dgvGrR_Af.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvGrR_Af.ColumnHeadersVisible = false;
            this.dgvGrR_Af.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3});
            this.dgvGrR_Af.Name = "dgvGrR_Af";
            this.dgvGrR_Af.ReadOnly = true;
            this.dgvGrR_Af.RowHeadersVisible = false;
            this.dgvGrR_Af.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Consolas", 7F);
            this.dgvGrR_Af.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvGrR_Af.RowTemplate.ReadOnly = true;
            this.dgvGrR_Af.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvGrR_Af.Tag = "0";
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Consolas", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lbl6_DrsAir);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.panel37);
            this.tabPage4.Controls.Add(this.panel39);
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.label18);
            this.tabPage4.Controls.Add(this.label24);
            this.tabPage4.Controls.Add(this.label50);
            this.tabPage4.Controls.Add(this.label51);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lbl6_DrsAir
            // 
            this.lbl6_DrsAir.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_DrsAir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_DrsAir, "lbl6_DrsAir");
            this.lbl6_DrsAir.Name = "lbl6_DrsAir";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel37
            // 
            this.panel37.Controls.Add(this.label12);
            this.panel37.Controls.Add(this.lbl6_UDrs2Tot);
            this.panel37.Controls.Add(this.lbl6_UDrs2Cyl);
            this.panel37.Controls.Add(this.lbl6_UDrs2Tbl);
            this.panel37.Controls.Add(this.lbl6_UDrs2Cnt);
            this.panel37.Controls.Add(this.lbl6_UDrs2Spl);
            resources.ApplyResources(this.panel37, "panel37");
            this.panel37.Name = "panel37";
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Gainsboro;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // lbl6_UDrs2Tot
            // 
            this.lbl6_UDrs2Tot.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs2Tot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs2Tot, "lbl6_UDrs2Tot");
            this.lbl6_UDrs2Tot.Name = "lbl6_UDrs2Tot";
            // 
            // lbl6_UDrs2Cyl
            // 
            this.lbl6_UDrs2Cyl.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs2Cyl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs2Cyl, "lbl6_UDrs2Cyl");
            this.lbl6_UDrs2Cyl.Name = "lbl6_UDrs2Cyl";
            // 
            // lbl6_UDrs2Tbl
            // 
            this.lbl6_UDrs2Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs2Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs2Tbl, "lbl6_UDrs2Tbl");
            this.lbl6_UDrs2Tbl.Name = "lbl6_UDrs2Tbl";
            // 
            // lbl6_UDrs2Cnt
            // 
            this.lbl6_UDrs2Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs2Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs2Cnt, "lbl6_UDrs2Cnt");
            this.lbl6_UDrs2Cnt.Name = "lbl6_UDrs2Cnt";
            // 
            // lbl6_UDrs2Spl
            // 
            this.lbl6_UDrs2Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs2Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs2Spl, "lbl6_UDrs2Spl");
            this.lbl6_UDrs2Spl.Name = "lbl6_UDrs2Spl";
            // 
            // panel39
            // 
            this.panel39.Controls.Add(this.label14);
            this.panel39.Controls.Add(this.lbl6_UDrs1Tot);
            this.panel39.Controls.Add(this.lbl6_UDrs1Cyl);
            this.panel39.Controls.Add(this.lbl6_UDrs1Tbl);
            this.panel39.Controls.Add(this.lbl6_UDrs1Cnt);
            this.panel39.Controls.Add(this.lbl6_UDrs1Spl);
            resources.ApplyResources(this.panel39, "panel39");
            this.panel39.Name = "panel39";
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.Gainsboro;
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // lbl6_UDrs1Tot
            // 
            this.lbl6_UDrs1Tot.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs1Tot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs1Tot, "lbl6_UDrs1Tot");
            this.lbl6_UDrs1Tot.Name = "lbl6_UDrs1Tot";
            // 
            // lbl6_UDrs1Cyl
            // 
            this.lbl6_UDrs1Cyl.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs1Cyl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs1Cyl, "lbl6_UDrs1Cyl");
            this.lbl6_UDrs1Cyl.Name = "lbl6_UDrs1Cyl";
            // 
            // lbl6_UDrs1Tbl
            // 
            this.lbl6_UDrs1Tbl.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs1Tbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs1Tbl, "lbl6_UDrs1Tbl");
            this.lbl6_UDrs1Tbl.Name = "lbl6_UDrs1Tbl";
            // 
            // lbl6_UDrs1Cnt
            // 
            this.lbl6_UDrs1Cnt.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs1Cnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs1Cnt, "lbl6_UDrs1Cnt");
            this.lbl6_UDrs1Cnt.Name = "lbl6_UDrs1Cnt";
            // 
            // lbl6_UDrs1Spl
            // 
            this.lbl6_UDrs1Spl.BackColor = System.Drawing.Color.Gainsboro;
            this.lbl6_UDrs1Spl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl6_UDrs1Spl, "lbl6_UDrs1Spl");
            this.lbl6_UDrs1Spl.Name = "lbl6_UDrs1Spl";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label50
            // 
            resources.ApplyResources(this.label50, "label50");
            this.label50.Name = "label50";
            // 
            // label51
            // 
            resources.ApplyResources(this.label51, "label51");
            this.label51.Name = "label51";
            // 
            // btn_Drs
            // 
            this.btn_Drs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Drs.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Drs, "btn_Drs");
            this.btn_Drs.ForeColor = System.Drawing.Color.White;
            this.btn_Drs.Name = "btn_Drs";
            this.btn_Drs.Tag = "GRR_Dressing";
            this.btn_Drs.UseVisualStyleBackColor = false;
            this.btn_Drs.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // ckb_Ejt
            // 
            resources.ApplyResources(this.ckb_Ejt, "ckb_Ejt");
            this.ckb_Ejt.BackColor = System.Drawing.Color.LightGray;
            this.ckb_Ejt.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_Ejt.FlatAppearance.BorderSize = 2;
            this.ckb_Ejt.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_Ejt.ForeColor = System.Drawing.Color.Black;
            this.ckb_Ejt.Name = "ckb_Ejt";
            this.ckb_Ejt.Tag = "2";
            this.ckb_Ejt.UseVisualStyleBackColor = false;
            this.ckb_Ejt.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckb_Ejt.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckbGrR_Wtr
            // 
            resources.ApplyResources(this.ckbGrR_Wtr, "ckbGrR_Wtr");
            this.ckbGrR_Wtr.BackColor = System.Drawing.Color.LightGray;
            this.ckbGrR_Wtr.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckbGrR_Wtr.FlatAppearance.BorderSize = 2;
            this.ckbGrR_Wtr.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckbGrR_Wtr.ForeColor = System.Drawing.Color.Black;
            this.ckbGrR_Wtr.Name = "ckbGrR_Wtr";
            this.ckbGrR_Wtr.Tag = "1";
            this.ckbGrR_Wtr.UseVisualStyleBackColor = false;
            this.ckbGrR_Wtr.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckbGrR_Wtr.Click += new System.EventHandler(this.ckb_Click);
            // 
            // ckb_Vac
            // 
            resources.ApplyResources(this.ckb_Vac, "ckb_Vac");
            this.ckb_Vac.BackColor = System.Drawing.Color.LightGray;
            this.ckb_Vac.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckb_Vac.FlatAppearance.BorderSize = 2;
            this.ckb_Vac.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.ckb_Vac.ForeColor = System.Drawing.Color.Black;
            this.ckb_Vac.Name = "ckb_Vac";
            this.ckb_Vac.Tag = "0";
            this.ckb_Vac.UseVisualStyleBackColor = false;
            this.ckb_Vac.CheckedChanged += new System.EventHandler(this.ckbGrd_CheckedChanged);
            this.ckb_Vac.Click += new System.EventHandler(this.ckb_Click);
            // 
            // btnGrR_StInsp
            // 
            this.btnGrR_StInsp.BackColor = System.Drawing.Color.Indigo;
            this.btnGrR_StInsp.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnGrR_StInsp, "btnGrR_StInsp");
            this.btnGrR_StInsp.ForeColor = System.Drawing.Color.White;
            this.btnGrR_StInsp.Image = global::SG2000X.Properties.Resources.Inspection64;
            this.btnGrR_StInsp.Name = "btnGrR_StInsp";
            this.btnGrR_StInsp.Tag = "GRR_Strip_Measure";
            this.btnGrR_StInsp.UseVisualStyleBackColor = false;
            this.btnGrR_StInsp.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // btnGrR_DrInsp
            // 
            this.btnGrR_DrInsp.BackColor = System.Drawing.Color.Indigo;
            this.btnGrR_DrInsp.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btnGrR_DrInsp, "btnGrR_DrInsp");
            this.btnGrR_DrInsp.ForeColor = System.Drawing.Color.White;
            this.btnGrR_DrInsp.Image = global::SG2000X.Properties.Resources.Inspection64;
            this.btnGrR_DrInsp.Name = "btnGrR_DrInsp";
            this.btnGrR_DrInsp.Tag = "GRR_Dresser_Measure";
            this.btnGrR_DrInsp.UseVisualStyleBackColor = false;
            this.btnGrR_DrInsp.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // btnGrR_WhInsp
            // 
            this.btnGrR_WhInsp.BackColor = System.Drawing.Color.Indigo;
            resources.ApplyResources(this.btnGrR_WhInsp, "btnGrR_WhInsp");
            this.btnGrR_WhInsp.FlatAppearance.BorderSize = 0;
            this.btnGrR_WhInsp.ForeColor = System.Drawing.Color.White;
            this.btnGrR_WhInsp.Image = global::SG2000X.Properties.Resources.Inspection64;
            this.btnGrR_WhInsp.Name = "btnGrR_WhInsp";
            this.btnGrR_WhInsp.Tag = "GRR_Wheel_Measure";
            this.btnGrR_WhInsp.UseVisualStyleBackColor = false;
            this.btnGrR_WhInsp.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // btn_H
            // 
            this.btn_H.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_H.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_H, "btn_H");
            this.btn_H.ForeColor = System.Drawing.Color.White;
            this.btn_H.Image = global::SG2000X.Properties.Resources.Home64;
            this.btn_H.Name = "btn_H";
            this.btn_H.Tag = "GRR_Home";
            this.btn_H.UseVisualStyleBackColor = false;
            this.btn_H.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // btn_Wait
            // 
            this.btn_Wait.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_Wait.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Wait, "btn_Wait");
            this.btn_Wait.ForeColor = System.Drawing.Color.White;
            this.btn_Wait.Image = global::SG2000X.Properties.Resources.Wait64;
            this.btn_Wait.Name = "btn_Wait";
            this.btn_Wait.Tag = "GRR_Wait";
            this.btn_Wait.UseVisualStyleBackColor = false;
            this.btn_Wait.Click += new System.EventHandler(this.btnMan_Cycle_Click);
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
            // btn_TbClean
            // 
            this.btn_TbClean.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_TbClean.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_TbClean, "btn_TbClean");
            this.btn_TbClean.ForeColor = System.Drawing.Color.White;
            this.btn_TbClean.Name = "btn_TbClean";
            this.btn_TbClean.Tag = "GRR_Table_Clean";
            this.btn_TbClean.UseVisualStyleBackColor = false;
            this.btn_TbClean.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // btn_Bcr
            // 
            this.btn_Bcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Bcr.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Bcr, "btn_Bcr");
            this.btn_Bcr.ForeColor = System.Drawing.Color.White;
            this.btn_Bcr.Name = "btn_Bcr";
            this.btn_Bcr.Tag = "GRR_Manual_Bcr";
            this.btn_Bcr.UseVisualStyleBackColor = false;
            this.btn_Bcr.Click += new System.EventHandler(this.btnMan_Cycle_Click);
            // 
            // lblGrR_Bcr
            // 
            this.lblGrR_Bcr.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblGrR_Bcr, "lblGrR_Bcr");
            this.lblGrR_Bcr.ForeColor = System.Drawing.Color.Black;
            this.lblGrR_Bcr.Name = "lblGrR_Bcr";
            // 
            // lbl_Unit4
            // 
            this.lbl_Unit4.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit4, "lbl_Unit4");
            this.lbl_Unit4.Name = "lbl_Unit4";
            this.lbl_Unit4.Tag = "";
            // 
            // lbl_Unit3
            // 
            this.lbl_Unit3.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit3, "lbl_Unit3");
            this.lbl_Unit3.Name = "lbl_Unit3";
            this.lbl_Unit3.Tag = "";
            // 
            // lbl_Unit2
            // 
            this.lbl_Unit2.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit2, "lbl_Unit2");
            this.lbl_Unit2.Name = "lbl_Unit2";
            this.lbl_Unit2.Tag = "";
            // 
            // lbl_Unit1
            // 
            this.lbl_Unit1.BackColor = System.Drawing.Color.LightGray;
            this.lbl_Unit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Unit1, "lbl_Unit1");
            this.lbl_Unit1.Name = "lbl_Unit1";
            this.lbl_Unit1.Tag = "";
            // 
            // panel22
            // 
            this.panel22.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel22.Controls.Add(this.lbl_DrsTime);
            this.panel22.Controls.Add(this.label1);
            this.panel22.Controls.Add(this.lbl_GrdTime);
            this.panel22.Controls.Add(this.label6);
            this.panel22.Controls.Add(this.label46);
            resources.ApplyResources(this.panel22, "panel22");
            this.panel22.Name = "panel22";
            this.panel22.Tag = "1";
            // 
            // lbl_DrsTime
            // 
            this.lbl_DrsTime.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.lbl_DrsTime, "lbl_DrsTime");
            this.lbl_DrsTime.Name = "lbl_DrsTime";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // lbl_GrdTime
            // 
            this.lbl_GrdTime.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.lbl_GrdTime, "lbl_GrdTime");
            this.lbl_GrdTime.Name = "lbl_GrdTime";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            // 
            // label46
            // 
            this.label46.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label46, "label46");
            this.label46.ForeColor = System.Drawing.Color.White;
            this.label46.Name = "label46";
            // 
            // vwMan_07Grr
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel22);
            this.Controls.Add(this.lbl_Unit4);
            this.Controls.Add(this.lbl_Unit3);
            this.Controls.Add(this.lbl_Unit2);
            this.Controls.Add(this.lbl_Unit1);
            this.Controls.Add(this.lblGrR_Bcr);
            this.Controls.Add(this.btn_Bcr);
            this.Controls.Add(this.btn_TbClean);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.lblGrR_TSetter);
            this.Controls.Add(this.btn_WaterKnife);
            this.Controls.Add(this.panel30);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.ckbGrR_TcW);
            this.Controls.Add(this.ckbGrR_TcAKn);
            this.Controls.Add(this.ckbGrR_TcWKn);
            this.Controls.Add(this.ckbGrR_TcDn);
            this.Controls.Add(this.lblGrR_BtmFlw);
            this.Controls.Add(this.lblGrR_GrdFlw);
            this.Controls.Add(this.lblGrR_SplPcw);
            this.Controls.Add(this.lblGrR_SplZig);
            this.Controls.Add(this.lblGrR_PrbAMP);
            this.Controls.Add(this.lblGrR_TcFlw);
            this.Controls.Add(this.lblGrR_TcDn);
            this.Controls.Add(this.lblGrR_TblFlw);
            this.Controls.Add(this.lbl_TblVac);
            this.Controls.Add(this.btn_Grd);
            this.Controls.Add(this.tabGrR_StripInsp);
            this.Controls.Add(this.btn_Drs);
            this.Controls.Add(this.ckb_Ejt);
            this.Controls.Add(this.ckbGrR_Wtr);
            this.Controls.Add(this.ckb_Vac);
            this.Controls.Add(this.btnGrR_StInsp);
            this.Controls.Add(this.btnGrR_DrInsp);
            this.Controls.Add(this.btnGrR_WhInsp);
            this.Controls.Add(this.btn_H);
            this.Controls.Add(this.btn_Wait);
            resources.ApplyResources(this, "$this");
            this.Name = "vwMan_07Grr";
            this.panel30.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.tabGrR_StripInsp.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnlGrR_R12.ResumeLayout(false);
            this.pnlGrR_R11.ResumeLayout(false);
            this.pnlGrR_R10.ResumeLayout(false);
            this.pnlGrR_R9.ResumeLayout(false);
            this.pnlGrR_R8.ResumeLayout(false);
            this.pnlGrR_R7.ResumeLayout(false);
            this.pnlGrR_R6.ResumeLayout(false);
            this.pnlGrR_R5.ResumeLayout(false);
            this.pnlGrR_R4.ResumeLayout(false);
            this.pnlGrR_Fi.ResumeLayout(false);
            this.pnlGrR_R3.ResumeLayout(false);
            this.pnlGrR_R2.ResumeLayout(false);
            this.pnlGrR_R1.ResumeLayout(false);
            this.tpGrR_Bf.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrR_Bf)).EndInit();
            this.tpGrR_Af.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrR_Af)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.panel37.ResumeLayout(false);
            this.panel39.ResumeLayout(false);
            this.panel22.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblGrR_TSetter;
        private System.Windows.Forms.Button btn_WaterKnife;
        private System.Windows.Forms.Panel panel30;
        private System.Windows.Forms.Label lblGrR_AAvg;
        private System.Windows.Forms.Label lblGrR_BAvg;
        private System.Windows.Forms.Label lblGrR_AMax;
        private System.Windows.Forms.Label lblGrR_AMin;
        private System.Windows.Forms.Label lblGrR_BMin;
        private System.Windows.Forms.Label lblGrR_BMax;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbl_ZPos;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lbl_ZStat;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lbl_YPos;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lbl_YStat;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label lbl_XPos;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label lbl_XStat;
        private System.Windows.Forms.CheckBox ckbGrR_TcW;
        private System.Windows.Forms.CheckBox ckbGrR_TcAKn;
        private System.Windows.Forms.CheckBox ckbGrR_TcWKn;
        private System.Windows.Forms.CheckBox ckbGrR_TcDn;
        private System.Windows.Forms.Label lblGrR_BtmFlw;
        private System.Windows.Forms.Label lblGrR_GrdFlw;
        private System.Windows.Forms.Label lblGrR_SplPcw;
        private System.Windows.Forms.Label lblGrR_SplZig;
        private System.Windows.Forms.Label lblGrR_PrbAMP;
        private System.Windows.Forms.Label lblGrR_TcFlw;
        private System.Windows.Forms.Label lblGrR_TcDn;
        private System.Windows.Forms.Label lblGrR_TblFlw;
        private System.Windows.Forms.Label lbl_TblVac;
        private System.Windows.Forms.Button btn_Grd;
        private System.Windows.Forms.TabControl tabGrR_StripInsp;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel pnlGrR_Fi;
        private System.Windows.Forms.Label lblR_FiNum;
        private System.Windows.Forms.Label lblGrR_FiReCnt;
        private System.Windows.Forms.Label lblGrR_FiOne;
        private System.Windows.Forms.Label lblGrR_Fi;
        private System.Windows.Forms.Label lblGrR_FiToTh;
        private System.Windows.Forms.Label lblGrR_FiCyTh;
        private System.Windows.Forms.Label lblGrR_FiTbl;
        private System.Windows.Forms.Label lblGrR_FiCnt;
        private System.Windows.Forms.Label lblGrR_FiSpl;
        private System.Windows.Forms.Label lblGrR_FiTar;
        private System.Windows.Forms.Panel pnlGrR_R3;
        private System.Windows.Forms.Label lblR_R3Num;
        private System.Windows.Forms.Label lblGrR_R3ReCnt;
        private System.Windows.Forms.Label lblGrR_R3One;
        private System.Windows.Forms.Label lblGrR_R3;
        private System.Windows.Forms.Label lblGrR_R3ToTh;
        private System.Windows.Forms.Label lblGrR_R3CyTh;
        private System.Windows.Forms.Label lblGrR_R3Tbl;
        private System.Windows.Forms.Label lblGrR_R3Cnt;
        private System.Windows.Forms.Label lblGrR_R3Spl;
        private System.Windows.Forms.Label lblGrR_R3Tar;
        private System.Windows.Forms.Panel pnlGrR_R2;
        private System.Windows.Forms.Label lblR_R2Num;
        private System.Windows.Forms.Label lblGrR_R2ReCnt;
        private System.Windows.Forms.Label lblGrR_R2One;
        private System.Windows.Forms.Label lblGrR_R2;
        private System.Windows.Forms.Label lblGrR_R2ToTh;
        private System.Windows.Forms.Label lblGrR_R2CyTh;
        private System.Windows.Forms.Label lblGrR_R2Tbl;
        private System.Windows.Forms.Label lblGrR_R2Cnt;
        private System.Windows.Forms.Label lblGrR_R2Spl;
        private System.Windows.Forms.Label lblGrR_R2Tar;
        private System.Windows.Forms.Panel pnlGrR_R1;
        private System.Windows.Forms.Label lblR_R1Num;
        private System.Windows.Forms.Label lblGrR_R1ReCnt;
        private System.Windows.Forms.Label lblGrR_R1One;
        private System.Windows.Forms.Label lblGrR_R1;
        private System.Windows.Forms.Label lblGrR_R1ToTh;
        private System.Windows.Forms.Label lblGrR_R1CyTh;
        private System.Windows.Forms.Label lblGrR_R1Tbl;
        private System.Windows.Forms.Label lblGrR_R1Cnt;
        private System.Windows.Forms.Label lblGrR_R1Spl;
        private System.Windows.Forms.Label lblGrR_R1Tar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblGrR_Mod;
        private System.Windows.Forms.Label lblGrR_ModT;
        private System.Windows.Forms.TabPage tpGrR_Bf;
        private ExDataGridView dgvGrR_Bf;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.TabPage tpGrR_Af;
        private ExDataGridView dgvGrR_Af;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lbl6_DrsAir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel37;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbl6_UDrs2Tot;
        private System.Windows.Forms.Label lbl6_UDrs2Cyl;
        private System.Windows.Forms.Label lbl6_UDrs2Tbl;
        private System.Windows.Forms.Label lbl6_UDrs2Cnt;
        private System.Windows.Forms.Label lbl6_UDrs2Spl;
        private System.Windows.Forms.Panel panel39;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbl6_UDrs1Tot;
        private System.Windows.Forms.Label lbl6_UDrs1Cyl;
        private System.Windows.Forms.Label lbl6_UDrs1Tbl;
        private System.Windows.Forms.Label lbl6_UDrs1Cnt;
        private System.Windows.Forms.Label lbl6_UDrs1Spl;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Button btn_Drs;
        private System.Windows.Forms.CheckBox ckb_Ejt;
        private System.Windows.Forms.CheckBox ckbGrR_Wtr;
        private System.Windows.Forms.CheckBox ckb_Vac;
        private System.Windows.Forms.Button btnGrR_StInsp;
        private System.Windows.Forms.Button btnGrR_DrInsp;
        private System.Windows.Forms.Button btnGrR_WhInsp;
        private System.Windows.Forms.Button btn_H;
        private System.Windows.Forms.Button btn_Wait;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Button btn_TbClean;
        private System.Windows.Forms.Button btn_Bcr;
        private System.Windows.Forms.Label lblGrR_Bcr;
        private System.Windows.Forms.Label lbl_Unit4;
        private System.Windows.Forms.Label lbl_Unit3;
        private System.Windows.Forms.Label lbl_Unit2;
        private System.Windows.Forms.Label lbl_Unit1;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.Label lbl_DrsTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_GrdTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Panel pnlGrR_R12;
        private System.Windows.Forms.Label lblR_R12Num;
        private System.Windows.Forms.Label lblGrR_R12ReCnt;
        private System.Windows.Forms.Label lblGrR_R12One;
        private System.Windows.Forms.Label lblGrR_R12;
        private System.Windows.Forms.Label lblGrR_R12ToTh;
        private System.Windows.Forms.Label lblGrR_R12CyTh;
        private System.Windows.Forms.Label lblGrR_R12Tbl;
        private System.Windows.Forms.Label lblGrR_R12Cnt;
        private System.Windows.Forms.Label lblGrR_R12Spl;
        private System.Windows.Forms.Label lblGrR_R12Tar;
        private System.Windows.Forms.Panel pnlGrR_R11;
        private System.Windows.Forms.Label lblR_R11Num;
        private System.Windows.Forms.Label lblGrR_R11ReCnt;
        private System.Windows.Forms.Label lblGrR_R11One;
        private System.Windows.Forms.Label lblGrR_R11;
        private System.Windows.Forms.Label lblGrR_R11ToTh;
        private System.Windows.Forms.Label lblGrR_R11CyTh;
        private System.Windows.Forms.Label lblGrR_R11Tbl;
        private System.Windows.Forms.Label lblGrR_R11Cnt;
        private System.Windows.Forms.Label lblGrR_R11Spl;
        private System.Windows.Forms.Label lblGrR_R11Tar;
        private System.Windows.Forms.Panel pnlGrR_R10;
        private System.Windows.Forms.Label lblR_R10Num;
        private System.Windows.Forms.Label lblGrR_R10ReCnt;
        private System.Windows.Forms.Label lblGrR_R10One;
        private System.Windows.Forms.Label lblGrR_R10;
        private System.Windows.Forms.Label lblGrR_R10ToTh;
        private System.Windows.Forms.Label lblGrR_R10CyTh;
        private System.Windows.Forms.Label lblGrR_R10Tbl;
        private System.Windows.Forms.Label lblGrR_R10Cnt;
        private System.Windows.Forms.Label lblGrR_R10Spl;
        private System.Windows.Forms.Label lblGrR_R10Tar;
        private System.Windows.Forms.Panel pnlGrR_R9;
        private System.Windows.Forms.Label lblR_R9Num;
        private System.Windows.Forms.Label lblGrR_R9ReCnt;
        private System.Windows.Forms.Label lblGrR_R9One;
        private System.Windows.Forms.Label lblGrR_R9;
        private System.Windows.Forms.Label lblGrR_R9ToTh;
        private System.Windows.Forms.Label lblGrR_R9CyTh;
        private System.Windows.Forms.Label lblGrR_R9Tbl;
        private System.Windows.Forms.Label lblGrR_R9Cnt;
        private System.Windows.Forms.Label lblGrR_R9Spl;
        private System.Windows.Forms.Label lblGrR_R9Tar;
        private System.Windows.Forms.Panel pnlGrR_R8;
        private System.Windows.Forms.Label lblR_R8Num;
        private System.Windows.Forms.Label lblGrR_R8ReCnt;
        private System.Windows.Forms.Label lblGrR_R8One;
        private System.Windows.Forms.Label lblGrR_R8;
        private System.Windows.Forms.Label lblGrR_R8ToTh;
        private System.Windows.Forms.Label lblGrR_R8CyTh;
        private System.Windows.Forms.Label lblGrR_R8Tbl;
        private System.Windows.Forms.Label lblGrR_R8Cnt;
        private System.Windows.Forms.Label lblGrR_R8Spl;
        private System.Windows.Forms.Label lblGrR_R8Tar;
        private System.Windows.Forms.Panel pnlGrR_R7;
        private System.Windows.Forms.Label lblR_R7Num;
        private System.Windows.Forms.Label lblGrR_R7ReCnt;
        private System.Windows.Forms.Label lblGrR_R7One;
        private System.Windows.Forms.Label lblGrR_R7;
        private System.Windows.Forms.Label lblGrR_R7ToTh;
        private System.Windows.Forms.Label lblGrR_R7CyTh;
        private System.Windows.Forms.Label lblGrR_R7Tbl;
        private System.Windows.Forms.Label lblGrR_R7Cnt;
        private System.Windows.Forms.Label lblGrR_R7Spl;
        private System.Windows.Forms.Label lblGrR_R7Tar;
        private System.Windows.Forms.Panel pnlGrR_R6;
        private System.Windows.Forms.Label lblR_R6Num;
        private System.Windows.Forms.Label lblGrR_R6ReCnt;
        private System.Windows.Forms.Label lblGrR_R6One;
        private System.Windows.Forms.Label lblGrR_R6;
        private System.Windows.Forms.Label lblGrR_R6ToTh;
        private System.Windows.Forms.Label lblGrR_R6CyTh;
        private System.Windows.Forms.Label lblGrR_R6Tbl;
        private System.Windows.Forms.Label lblGrR_R6Cnt;
        private System.Windows.Forms.Label lblGrR_R6Spl;
        private System.Windows.Forms.Label lblGrR_R6Tar;
        private System.Windows.Forms.Panel pnlGrR_R5;
        private System.Windows.Forms.Label lblR_R5Num;
        private System.Windows.Forms.Label lblGrR_R5ReCnt;
        private System.Windows.Forms.Label lblGrR_R5One;
        private System.Windows.Forms.Label lblGrR_R5;
        private System.Windows.Forms.Label lblGrR_R5ToTh;
        private System.Windows.Forms.Label lblGrR_R5CyTh;
        private System.Windows.Forms.Label lblGrR_R5Tbl;
        private System.Windows.Forms.Label lblGrR_R5Cnt;
        private System.Windows.Forms.Label lblGrR_R5Spl;
        private System.Windows.Forms.Label lblGrR_R5Tar;
        private System.Windows.Forms.Panel pnlGrR_R4;
        private System.Windows.Forms.Label lblR_R4Num;
        private System.Windows.Forms.Label lblGrR_R4ReCnt;
        private System.Windows.Forms.Label lblGrR_R4One;
        private System.Windows.Forms.Label lblGrR_R4;
        private System.Windows.Forms.Label lblGrR_R4ToTh;
        private System.Windows.Forms.Label lblGrR_R4CyTh;
        private System.Windows.Forms.Label lblGrR_R4Tbl;
        private System.Windows.Forms.Label lblGrR_R4Cnt;
        private System.Windows.Forms.Label lblGrR_R4Spl;
        private System.Windows.Forms.Label lblGrR_R4Tar;
    }
}
