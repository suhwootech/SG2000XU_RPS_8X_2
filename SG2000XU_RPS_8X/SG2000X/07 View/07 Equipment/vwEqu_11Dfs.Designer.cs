namespace SG2000X
{
    partial class vwEqu_11Dfs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_11Dfs));
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lbDf_PcbUse = new System.Windows.Forms.Label();
            this.cmbDf_PcbUse = new System.Windows.Forms.ComboBox();
            this.btnDf_SendRowData = new System.Windows.Forms.Button();
            this.txtDf_RowNum = new System.Windows.Forms.TextBox();
            this.lbDf_RowNum = new System.Windows.Forms.Label();
            this.btnDf_SendPos = new System.Windows.Forms.Button();
            this.txtDf_Col = new System.Windows.Forms.TextBox();
            this.lbDf_Col = new System.Windows.Forms.Label();
            this.txtDf_Row = new System.Windows.Forms.TextBox();
            this.lbDf_Row = new System.Windows.Forms.Label();
            this.btnDf_SendPcb = new System.Windows.Forms.Button();
            this.txtDf_Pcb3 = new System.Windows.Forms.TextBox();
            this.lbDf_Pcb3 = new System.Windows.Forms.Label();
            this.txtDf_Pcb2 = new System.Windows.Forms.TextBox();
            this.lbDf_Pcb2 = new System.Windows.Forms.Label();
            this.txtDf_Pcb1 = new System.Windows.Forms.TextBox();
            this.lbDf_Pcb1 = new System.Windows.Forms.Label();
            this.btnDf_SendAfEnd = new System.Windows.Forms.Button();
            this.btnDf_SendAfStart = new System.Windows.Forms.Button();
            this.btnDf_SendBfEnd = new System.Windows.Forms.Button();
            this.btnDf_SendBfStart = new System.Windows.Forms.Button();
            this.btnDf_SendGrdReady = new System.Windows.Forms.Button();
            this.pnlDf_Row = new System.Windows.Forms.Panel();
            this.lbDf_Way = new System.Windows.Forms.Label();
            this.cmbDf_GrdWay = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnDf_SendRetBcr = new System.Windows.Forms.Button();
            this.btnDf_SendInrBcr = new System.Windows.Forms.Button();
            this.txtDf_BcrId = new System.Windows.Forms.TextBox();
            this.lbDf_BcrId = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnDf_SendLotExist = new System.Windows.Forms.Button();
            this.btnDf_SendLotEnd = new System.Windows.Forms.Button();
            this.btnDf_SendLotOpen = new System.Windows.Forms.Button();
            this.lbDf_GrdMode = new System.Windows.Forms.Label();
            this.cmbDf_GrdMode = new System.Windows.Forms.ComboBox();
            this.txtDf_LotName = new System.Windows.Forms.TextBox();
            this.lbDf_LotName = new System.Windows.Forms.Label();
            this.btnDf_SendReady = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lib_Df_Log = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDf_Con = new System.Windows.Forms.Button();
            this.txtDf_Id = new System.Windows.Forms.TextBox();
            this.lbDf_Id = new System.Windows.Forms.Label();
            this.btnDf_Save = new System.Windows.Forms.Button();
            this.txtDf_Port = new System.Windows.Forms.TextBox();
            this.lbDf_Port = new System.Windows.Forms.Label();
            this.txtDf_Ip = new System.Windows.Forms.TextBox();
            this.lbDf_Ip = new System.Windows.Forms.Label();
            this.btnDf_SendId = new System.Windows.Forms.Button();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.lbDf_PcbUse);
            this.groupBox7.Controls.Add(this.cmbDf_PcbUse);
            this.groupBox7.Controls.Add(this.btnDf_SendRowData);
            this.groupBox7.Controls.Add(this.txtDf_RowNum);
            this.groupBox7.Controls.Add(this.lbDf_RowNum);
            this.groupBox7.Controls.Add(this.btnDf_SendPos);
            this.groupBox7.Controls.Add(this.txtDf_Col);
            this.groupBox7.Controls.Add(this.lbDf_Col);
            this.groupBox7.Controls.Add(this.txtDf_Row);
            this.groupBox7.Controls.Add(this.lbDf_Row);
            this.groupBox7.Controls.Add(this.btnDf_SendPcb);
            this.groupBox7.Controls.Add(this.txtDf_Pcb3);
            this.groupBox7.Controls.Add(this.lbDf_Pcb3);
            this.groupBox7.Controls.Add(this.txtDf_Pcb2);
            this.groupBox7.Controls.Add(this.lbDf_Pcb2);
            this.groupBox7.Controls.Add(this.txtDf_Pcb1);
            this.groupBox7.Controls.Add(this.lbDf_Pcb1);
            this.groupBox7.Controls.Add(this.btnDf_SendAfEnd);
            this.groupBox7.Controls.Add(this.btnDf_SendAfStart);
            this.groupBox7.Controls.Add(this.btnDf_SendBfEnd);
            this.groupBox7.Controls.Add(this.btnDf_SendBfStart);
            this.groupBox7.Controls.Add(this.btnDf_SendGrdReady);
            this.groupBox7.Controls.Add(this.pnlDf_Row);
            this.groupBox7.Controls.Add(this.lbDf_Way);
            this.groupBox7.Controls.Add(this.cmbDf_GrdWay);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // lbDf_PcbUse
            // 
            resources.ApplyResources(this.lbDf_PcbUse, "lbDf_PcbUse");
            this.lbDf_PcbUse.Name = "lbDf_PcbUse";
            // 
            // cmbDf_PcbUse
            // 
            resources.ApplyResources(this.cmbDf_PcbUse, "cmbDf_PcbUse");
            this.cmbDf_PcbUse.FormattingEnabled = true;
            this.cmbDf_PcbUse.Items.AddRange(new object[] {
            resources.GetString("cmbDf_PcbUse.Items"),
            resources.GetString("cmbDf_PcbUse.Items1"),
            resources.GetString("cmbDf_PcbUse.Items2")});
            this.cmbDf_PcbUse.Name = "cmbDf_PcbUse";
            // 
            // btnDf_SendRowData
            // 
            resources.ApplyResources(this.btnDf_SendRowData, "btnDf_SendRowData");
            this.btnDf_SendRowData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendRowData.FlatAppearance.BorderSize = 0;
            this.btnDf_SendRowData.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendRowData.Name = "btnDf_SendRowData";
            this.btnDf_SendRowData.Tag = "";
            this.btnDf_SendRowData.UseVisualStyleBackColor = false;
            this.btnDf_SendRowData.Click += new System.EventHandler(this.btnDf_SendRowData_Click);
            // 
            // txtDf_RowNum
            // 
            resources.ApplyResources(this.txtDf_RowNum, "txtDf_RowNum");
            this.txtDf_RowNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_RowNum.Name = "txtDf_RowNum";
            // 
            // lbDf_RowNum
            // 
            resources.ApplyResources(this.lbDf_RowNum, "lbDf_RowNum");
            this.lbDf_RowNum.Name = "lbDf_RowNum";
            // 
            // btnDf_SendPos
            // 
            resources.ApplyResources(this.btnDf_SendPos, "btnDf_SendPos");
            this.btnDf_SendPos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendPos.FlatAppearance.BorderSize = 0;
            this.btnDf_SendPos.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendPos.Name = "btnDf_SendPos";
            this.btnDf_SendPos.Tag = "";
            this.btnDf_SendPos.UseVisualStyleBackColor = false;
            this.btnDf_SendPos.Click += new System.EventHandler(this.btnDf_SendPos_Click);
            // 
            // txtDf_Col
            // 
            resources.ApplyResources(this.txtDf_Col, "txtDf_Col");
            this.txtDf_Col.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Col.Name = "txtDf_Col";
            // 
            // lbDf_Col
            // 
            resources.ApplyResources(this.lbDf_Col, "lbDf_Col");
            this.lbDf_Col.Name = "lbDf_Col";
            // 
            // txtDf_Row
            // 
            resources.ApplyResources(this.txtDf_Row, "txtDf_Row");
            this.txtDf_Row.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Row.Name = "txtDf_Row";
            // 
            // lbDf_Row
            // 
            resources.ApplyResources(this.lbDf_Row, "lbDf_Row");
            this.lbDf_Row.Name = "lbDf_Row";
            // 
            // btnDf_SendPcb
            // 
            resources.ApplyResources(this.btnDf_SendPcb, "btnDf_SendPcb");
            this.btnDf_SendPcb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendPcb.FlatAppearance.BorderSize = 0;
            this.btnDf_SendPcb.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendPcb.Name = "btnDf_SendPcb";
            this.btnDf_SendPcb.Tag = "";
            this.btnDf_SendPcb.UseVisualStyleBackColor = false;
            this.btnDf_SendPcb.Click += new System.EventHandler(this.btnDf_SendPcb_Click);
            // 
            // txtDf_Pcb3
            // 
            resources.ApplyResources(this.txtDf_Pcb3, "txtDf_Pcb3");
            this.txtDf_Pcb3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Pcb3.Name = "txtDf_Pcb3";
            // 
            // lbDf_Pcb3
            // 
            resources.ApplyResources(this.lbDf_Pcb3, "lbDf_Pcb3");
            this.lbDf_Pcb3.Name = "lbDf_Pcb3";
            // 
            // txtDf_Pcb2
            // 
            resources.ApplyResources(this.txtDf_Pcb2, "txtDf_Pcb2");
            this.txtDf_Pcb2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Pcb2.Name = "txtDf_Pcb2";
            // 
            // lbDf_Pcb2
            // 
            resources.ApplyResources(this.lbDf_Pcb2, "lbDf_Pcb2");
            this.lbDf_Pcb2.Name = "lbDf_Pcb2";
            // 
            // txtDf_Pcb1
            // 
            resources.ApplyResources(this.txtDf_Pcb1, "txtDf_Pcb1");
            this.txtDf_Pcb1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Pcb1.Name = "txtDf_Pcb1";
            // 
            // lbDf_Pcb1
            // 
            resources.ApplyResources(this.lbDf_Pcb1, "lbDf_Pcb1");
            this.lbDf_Pcb1.Name = "lbDf_Pcb1";
            // 
            // btnDf_SendAfEnd
            // 
            resources.ApplyResources(this.btnDf_SendAfEnd, "btnDf_SendAfEnd");
            this.btnDf_SendAfEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendAfEnd.FlatAppearance.BorderSize = 0;
            this.btnDf_SendAfEnd.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendAfEnd.Name = "btnDf_SendAfEnd";
            this.btnDf_SendAfEnd.Tag = "";
            this.btnDf_SendAfEnd.UseVisualStyleBackColor = false;
            this.btnDf_SendAfEnd.Click += new System.EventHandler(this.btnDf_SendAfEnd_Click);
            // 
            // btnDf_SendAfStart
            // 
            resources.ApplyResources(this.btnDf_SendAfStart, "btnDf_SendAfStart");
            this.btnDf_SendAfStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendAfStart.FlatAppearance.BorderSize = 0;
            this.btnDf_SendAfStart.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendAfStart.Name = "btnDf_SendAfStart";
            this.btnDf_SendAfStart.Tag = "";
            this.btnDf_SendAfStart.UseVisualStyleBackColor = false;
            this.btnDf_SendAfStart.Click += new System.EventHandler(this.btnDf_SendAfStart_Click);
            // 
            // btnDf_SendBfEnd
            // 
            resources.ApplyResources(this.btnDf_SendBfEnd, "btnDf_SendBfEnd");
            this.btnDf_SendBfEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendBfEnd.FlatAppearance.BorderSize = 0;
            this.btnDf_SendBfEnd.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendBfEnd.Name = "btnDf_SendBfEnd";
            this.btnDf_SendBfEnd.Tag = "";
            this.btnDf_SendBfEnd.UseVisualStyleBackColor = false;
            this.btnDf_SendBfEnd.Click += new System.EventHandler(this.btnDf_SendBfEnd_Click);
            // 
            // btnDf_SendBfStart
            // 
            resources.ApplyResources(this.btnDf_SendBfStart, "btnDf_SendBfStart");
            this.btnDf_SendBfStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendBfStart.FlatAppearance.BorderSize = 0;
            this.btnDf_SendBfStart.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendBfStart.Name = "btnDf_SendBfStart";
            this.btnDf_SendBfStart.Tag = "";
            this.btnDf_SendBfStart.UseVisualStyleBackColor = false;
            this.btnDf_SendBfStart.Click += new System.EventHandler(this.btnDf_SendBfStart_Click);
            // 
            // btnDf_SendGrdReady
            // 
            resources.ApplyResources(this.btnDf_SendGrdReady, "btnDf_SendGrdReady");
            this.btnDf_SendGrdReady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendGrdReady.FlatAppearance.BorderSize = 0;
            this.btnDf_SendGrdReady.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendGrdReady.Name = "btnDf_SendGrdReady";
            this.btnDf_SendGrdReady.Tag = "";
            this.btnDf_SendGrdReady.UseVisualStyleBackColor = false;
            this.btnDf_SendGrdReady.Click += new System.EventHandler(this.btnDf_SendGrdReady_Click);
            // 
            // pnlDf_Row
            // 
            resources.ApplyResources(this.pnlDf_Row, "pnlDf_Row");
            this.pnlDf_Row.BackColor = System.Drawing.Color.Gray;
            this.pnlDf_Row.Name = "pnlDf_Row";
            // 
            // lbDf_Way
            // 
            resources.ApplyResources(this.lbDf_Way, "lbDf_Way");
            this.lbDf_Way.Name = "lbDf_Way";
            // 
            // cmbDf_GrdWay
            // 
            resources.ApplyResources(this.cmbDf_GrdWay, "cmbDf_GrdWay");
            this.cmbDf_GrdWay.FormattingEnabled = true;
            this.cmbDf_GrdWay.Items.AddRange(new object[] {
            resources.GetString("cmbDf_GrdWay.Items"),
            resources.GetString("cmbDf_GrdWay.Items1")});
            this.cmbDf_GrdWay.Name = "cmbDf_GrdWay";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.btnDf_SendRetBcr);
            this.groupBox6.Controls.Add(this.btnDf_SendInrBcr);
            this.groupBox6.Controls.Add(this.txtDf_BcrId);
            this.groupBox6.Controls.Add(this.lbDf_BcrId);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // btnDf_SendRetBcr
            // 
            resources.ApplyResources(this.btnDf_SendRetBcr, "btnDf_SendRetBcr");
            this.btnDf_SendRetBcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendRetBcr.FlatAppearance.BorderSize = 0;
            this.btnDf_SendRetBcr.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendRetBcr.Name = "btnDf_SendRetBcr";
            this.btnDf_SendRetBcr.Tag = "";
            this.btnDf_SendRetBcr.UseVisualStyleBackColor = false;
            this.btnDf_SendRetBcr.Click += new System.EventHandler(this.btnDf_SendRetBcr_Click);
            // 
            // btnDf_SendInrBcr
            // 
            resources.ApplyResources(this.btnDf_SendInrBcr, "btnDf_SendInrBcr");
            this.btnDf_SendInrBcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendInrBcr.FlatAppearance.BorderSize = 0;
            this.btnDf_SendInrBcr.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendInrBcr.Name = "btnDf_SendInrBcr";
            this.btnDf_SendInrBcr.Tag = "";
            this.btnDf_SendInrBcr.UseVisualStyleBackColor = false;
            this.btnDf_SendInrBcr.Click += new System.EventHandler(this.btnDf_SendInrBcr_Click);
            // 
            // txtDf_BcrId
            // 
            resources.ApplyResources(this.txtDf_BcrId, "txtDf_BcrId");
            this.txtDf_BcrId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_BcrId.Name = "txtDf_BcrId";
            // 
            // lbDf_BcrId
            // 
            resources.ApplyResources(this.lbDf_BcrId, "lbDf_BcrId");
            this.lbDf_BcrId.Name = "lbDf_BcrId";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.btnDf_SendLotExist);
            this.groupBox5.Controls.Add(this.btnDf_SendLotEnd);
            this.groupBox5.Controls.Add(this.btnDf_SendLotOpen);
            this.groupBox5.Controls.Add(this.lbDf_GrdMode);
            this.groupBox5.Controls.Add(this.cmbDf_GrdMode);
            this.groupBox5.Controls.Add(this.txtDf_LotName);
            this.groupBox5.Controls.Add(this.lbDf_LotName);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // btnDf_SendLotExist
            // 
            resources.ApplyResources(this.btnDf_SendLotExist, "btnDf_SendLotExist");
            this.btnDf_SendLotExist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendLotExist.FlatAppearance.BorderSize = 0;
            this.btnDf_SendLotExist.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendLotExist.Name = "btnDf_SendLotExist";
            this.btnDf_SendLotExist.Tag = "";
            this.btnDf_SendLotExist.UseVisualStyleBackColor = false;
            this.btnDf_SendLotExist.Click += new System.EventHandler(this.btnDf_SendLotExist_Click);
            // 
            // btnDf_SendLotEnd
            // 
            resources.ApplyResources(this.btnDf_SendLotEnd, "btnDf_SendLotEnd");
            this.btnDf_SendLotEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendLotEnd.FlatAppearance.BorderSize = 0;
            this.btnDf_SendLotEnd.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendLotEnd.Name = "btnDf_SendLotEnd";
            this.btnDf_SendLotEnd.Tag = "";
            this.btnDf_SendLotEnd.UseVisualStyleBackColor = false;
            this.btnDf_SendLotEnd.Click += new System.EventHandler(this.btnDf_SendLotEnd_Click);
            // 
            // btnDf_SendLotOpen
            // 
            resources.ApplyResources(this.btnDf_SendLotOpen, "btnDf_SendLotOpen");
            this.btnDf_SendLotOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendLotOpen.FlatAppearance.BorderSize = 0;
            this.btnDf_SendLotOpen.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendLotOpen.Name = "btnDf_SendLotOpen";
            this.btnDf_SendLotOpen.Tag = "";
            this.btnDf_SendLotOpen.UseVisualStyleBackColor = false;
            this.btnDf_SendLotOpen.Click += new System.EventHandler(this.btnDf_SendLotOpen_Click);
            // 
            // lbDf_GrdMode
            // 
            resources.ApplyResources(this.lbDf_GrdMode, "lbDf_GrdMode");
            this.lbDf_GrdMode.Name = "lbDf_GrdMode";
            // 
            // cmbDf_GrdMode
            // 
            resources.ApplyResources(this.cmbDf_GrdMode, "cmbDf_GrdMode");
            this.cmbDf_GrdMode.FormattingEnabled = true;
            this.cmbDf_GrdMode.Items.AddRange(new object[] {
            resources.GetString("cmbDf_GrdMode.Items"),
            resources.GetString("cmbDf_GrdMode.Items1")});
            this.cmbDf_GrdMode.Name = "cmbDf_GrdMode";
            // 
            // txtDf_LotName
            // 
            resources.ApplyResources(this.txtDf_LotName, "txtDf_LotName");
            this.txtDf_LotName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_LotName.Name = "txtDf_LotName";
            // 
            // lbDf_LotName
            // 
            resources.ApplyResources(this.lbDf_LotName, "lbDf_LotName");
            this.lbDf_LotName.Name = "lbDf_LotName";
            // 
            // btnDf_SendReady
            // 
            resources.ApplyResources(this.btnDf_SendReady, "btnDf_SendReady");
            this.btnDf_SendReady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendReady.FlatAppearance.BorderSize = 0;
            this.btnDf_SendReady.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendReady.Name = "btnDf_SendReady";
            this.btnDf_SendReady.Tag = "";
            this.btnDf_SendReady.UseVisualStyleBackColor = false;
            this.btnDf_SendReady.Click += new System.EventHandler(this.btnDf_SendReady_Click);
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.lib_Df_Log);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // lib_Df_Log
            // 
            resources.ApplyResources(this.lib_Df_Log, "lib_Df_Log");
            this.lib_Df_Log.FormattingEnabled = true;
            this.lib_Df_Log.Name = "lib_Df_Log";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.btnDf_Con);
            this.groupBox3.Controls.Add(this.txtDf_Id);
            this.groupBox3.Controls.Add(this.lbDf_Id);
            this.groupBox3.Controls.Add(this.btnDf_Save);
            this.groupBox3.Controls.Add(this.txtDf_Port);
            this.groupBox3.Controls.Add(this.lbDf_Port);
            this.groupBox3.Controls.Add(this.txtDf_Ip);
            this.groupBox3.Controls.Add(this.lbDf_Ip);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // btnDf_Con
            // 
            resources.ApplyResources(this.btnDf_Con, "btnDf_Con");
            this.btnDf_Con.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_Con.FlatAppearance.BorderSize = 0;
            this.btnDf_Con.ForeColor = System.Drawing.Color.White;
            this.btnDf_Con.Name = "btnDf_Con";
            this.btnDf_Con.Tag = "";
            this.btnDf_Con.UseVisualStyleBackColor = false;
            this.btnDf_Con.Click += new System.EventHandler(this.btnDf_Con_Click);
            // 
            // txtDf_Id
            // 
            resources.ApplyResources(this.txtDf_Id, "txtDf_Id");
            this.txtDf_Id.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Id.Name = "txtDf_Id";
            // 
            // lbDf_Id
            // 
            resources.ApplyResources(this.lbDf_Id, "lbDf_Id");
            this.lbDf_Id.Name = "lbDf_Id";
            // 
            // btnDf_Save
            // 
            resources.ApplyResources(this.btnDf_Save, "btnDf_Save");
            this.btnDf_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_Save.FlatAppearance.BorderSize = 0;
            this.btnDf_Save.ForeColor = System.Drawing.Color.White;
            this.btnDf_Save.Name = "btnDf_Save";
            this.btnDf_Save.Tag = "";
            this.btnDf_Save.UseVisualStyleBackColor = false;
            this.btnDf_Save.Click += new System.EventHandler(this.btnDf_Save_Click);
            // 
            // txtDf_Port
            // 
            resources.ApplyResources(this.txtDf_Port, "txtDf_Port");
            this.txtDf_Port.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Port.Name = "txtDf_Port";
            // 
            // lbDf_Port
            // 
            resources.ApplyResources(this.lbDf_Port, "lbDf_Port");
            this.lbDf_Port.Name = "lbDf_Port";
            // 
            // txtDf_Ip
            // 
            resources.ApplyResources(this.txtDf_Ip, "txtDf_Ip");
            this.txtDf_Ip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDf_Ip.Name = "txtDf_Ip";
            // 
            // lbDf_Ip
            // 
            resources.ApplyResources(this.lbDf_Ip, "lbDf_Ip");
            this.lbDf_Ip.Name = "lbDf_Ip";
            // 
            // btnDf_SendId
            // 
            resources.ApplyResources(this.btnDf_SendId, "btnDf_SendId");
            this.btnDf_SendId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDf_SendId.FlatAppearance.BorderSize = 0;
            this.btnDf_SendId.ForeColor = System.Drawing.Color.White;
            this.btnDf_SendId.Name = "btnDf_SendId";
            this.btnDf_SendId.Tag = "";
            this.btnDf_SendId.UseVisualStyleBackColor = false;
            this.btnDf_SendId.Click += new System.EventHandler(this.btnDf_SendId_Click);
            // 
            // vwEqu_11Dfs
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnDf_SendId);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnDf_SendReady);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_11Dfs";
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label lbDf_PcbUse;
        private System.Windows.Forms.ComboBox cmbDf_PcbUse;
        private System.Windows.Forms.Button btnDf_SendRowData;
        private System.Windows.Forms.TextBox txtDf_RowNum;
        private System.Windows.Forms.Label lbDf_RowNum;
        private System.Windows.Forms.Button btnDf_SendPos;
        private System.Windows.Forms.TextBox txtDf_Col;
        private System.Windows.Forms.Label lbDf_Col;
        private System.Windows.Forms.TextBox txtDf_Row;
        private System.Windows.Forms.Label lbDf_Row;
        private System.Windows.Forms.Button btnDf_SendPcb;
        private System.Windows.Forms.TextBox txtDf_Pcb3;
        private System.Windows.Forms.Label lbDf_Pcb3;
        private System.Windows.Forms.TextBox txtDf_Pcb2;
        private System.Windows.Forms.Label lbDf_Pcb2;
        private System.Windows.Forms.TextBox txtDf_Pcb1;
        private System.Windows.Forms.Label lbDf_Pcb1;
        private System.Windows.Forms.Button btnDf_SendAfEnd;
        private System.Windows.Forms.Button btnDf_SendAfStart;
        private System.Windows.Forms.Button btnDf_SendBfEnd;
        private System.Windows.Forms.Button btnDf_SendBfStart;
        private System.Windows.Forms.Button btnDf_SendGrdReady;
        private System.Windows.Forms.Panel pnlDf_Row;
        private System.Windows.Forms.Label lbDf_Way;
        private System.Windows.Forms.ComboBox cmbDf_GrdWay;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnDf_SendRetBcr;
        private System.Windows.Forms.Button btnDf_SendInrBcr;
        private System.Windows.Forms.TextBox txtDf_BcrId;
        private System.Windows.Forms.Label lbDf_BcrId;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnDf_SendLotExist;
        private System.Windows.Forms.Button btnDf_SendLotEnd;
        private System.Windows.Forms.Button btnDf_SendLotOpen;
        private System.Windows.Forms.Label lbDf_GrdMode;
        private System.Windows.Forms.ComboBox cmbDf_GrdMode;
        private System.Windows.Forms.TextBox txtDf_LotName;
        private System.Windows.Forms.Label lbDf_LotName;
        private System.Windows.Forms.Button btnDf_SendReady;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lib_Df_Log;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDf_Con;
        private System.Windows.Forms.TextBox txtDf_Id;
        private System.Windows.Forms.Label lbDf_Id;
        private System.Windows.Forms.Button btnDf_Save;
        private System.Windows.Forms.TextBox txtDf_Port;
        private System.Windows.Forms.Label lbDf_Port;
        private System.Windows.Forms.TextBox txtDf_Ip;
        private System.Windows.Forms.Label lbDf_Ip;
        private System.Windows.Forms.Button btnDf_SendId;
    }
}
