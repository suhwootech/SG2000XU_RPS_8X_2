namespace SG2000X
{
    partial class vwEqu_07Twl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_07Twl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTw_List = new System.Windows.Forms.Panel();
            this.btnTw_Save = new System.Windows.Forms.Button();
            this.btnTw_Stop = new System.Windows.Forms.Button();
            this.btnTw_Start = new System.Windows.Forms.Button();
            this.pnlTw_B = new System.Windows.Forms.Panel();
            this.rdbTw_BzOff = new System.Windows.Forms.RadioButton();
            this.rdbTw_Bz3 = new System.Windows.Forms.RadioButton();
            this.rdbTw_Bz2 = new System.Windows.Forms.RadioButton();
            this.rdbTw_Bz1 = new System.Windows.Forms.RadioButton();
            this.lblTw_B_T = new System.Windows.Forms.Label();
            this.pnlTw_G = new System.Windows.Forms.Panel();
            this.rdbTw_GFck = new System.Windows.Forms.RadioButton();
            this.rdbTw_GOff = new System.Windows.Forms.RadioButton();
            this.rdbTw_GOn = new System.Windows.Forms.RadioButton();
            this.lblTw_G_T = new System.Windows.Forms.Label();
            this.pnlTw_Y = new System.Windows.Forms.Panel();
            this.rdbTw_YFck = new System.Windows.Forms.RadioButton();
            this.rdbTw_YOff = new System.Windows.Forms.RadioButton();
            this.rdbTw_YOn = new System.Windows.Forms.RadioButton();
            this.lblTw_Y_T = new System.Windows.Forms.Label();
            this.pnlTw_R = new System.Windows.Forms.Panel();
            this.rdbTw_RFck = new System.Windows.Forms.RadioButton();
            this.rdbTw_ROff = new System.Windows.Forms.RadioButton();
            this.rdbTw_ROn = new System.Windows.Forms.RadioButton();
            this.lblTw_R_T = new System.Windows.Forms.Label();
            this.dgvTw_List = new System.Windows.Forms.DataGridView();
            this.Col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTw_T = new System.Windows.Forms.Label();
            this.panel38 = new System.Windows.Forms.Panel();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lblO_IdleT_T = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.pnlTw_List.SuspendLayout();
            this.pnlTw_B.SuspendLayout();
            this.pnlTw_G.SuspendLayout();
            this.pnlTw_Y.SuspendLayout();
            this.pnlTw_R.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTw_List)).BeginInit();
            this.panel38.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTw_List
            // 
            resources.ApplyResources(this.pnlTw_List, "pnlTw_List");
            this.pnlTw_List.BackColor = System.Drawing.Color.White;
            this.pnlTw_List.Controls.Add(this.btnTw_Save);
            this.pnlTw_List.Controls.Add(this.btnTw_Stop);
            this.pnlTw_List.Controls.Add(this.btnTw_Start);
            this.pnlTw_List.Controls.Add(this.pnlTw_B);
            this.pnlTw_List.Controls.Add(this.pnlTw_G);
            this.pnlTw_List.Controls.Add(this.pnlTw_Y);
            this.pnlTw_List.Controls.Add(this.pnlTw_R);
            this.pnlTw_List.Controls.Add(this.dgvTw_List);
            this.pnlTw_List.Controls.Add(this.lblTw_T);
            this.pnlTw_List.Name = "pnlTw_List";
            this.pnlTw_List.Tag = "0";
            // 
            // btnTw_Save
            // 
            resources.ApplyResources(this.btnTw_Save, "btnTw_Save");
            this.btnTw_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnTw_Save.FlatAppearance.BorderSize = 0;
            this.btnTw_Save.ForeColor = System.Drawing.Color.White;
            this.btnTw_Save.Name = "btnTw_Save";
            this.btnTw_Save.Tag = "";
            this.btnTw_Save.UseVisualStyleBackColor = false;
            this.btnTw_Save.Click += new System.EventHandler(this.btnTw_Save_Click);
            // 
            // btnTw_Stop
            // 
            resources.ApplyResources(this.btnTw_Stop, "btnTw_Stop");
            this.btnTw_Stop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.btnTw_Stop.FlatAppearance.BorderSize = 0;
            this.btnTw_Stop.ForeColor = System.Drawing.Color.White;
            this.btnTw_Stop.Name = "btnTw_Stop";
            this.btnTw_Stop.Tag = "";
            this.btnTw_Stop.UseVisualStyleBackColor = false;
            this.btnTw_Stop.Click += new System.EventHandler(this.btnTw_Stop_Click);
            // 
            // btnTw_Start
            // 
            resources.ApplyResources(this.btnTw_Start, "btnTw_Start");
            this.btnTw_Start.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnTw_Start.FlatAppearance.BorderSize = 0;
            this.btnTw_Start.ForeColor = System.Drawing.Color.White;
            this.btnTw_Start.Name = "btnTw_Start";
            this.btnTw_Start.Tag = "";
            this.btnTw_Start.UseVisualStyleBackColor = false;
            this.btnTw_Start.Click += new System.EventHandler(this.btnTw_Start_Click);
            // 
            // pnlTw_B
            // 
            resources.ApplyResources(this.pnlTw_B, "pnlTw_B");
            this.pnlTw_B.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTw_B.Controls.Add(this.rdbTw_BzOff);
            this.pnlTw_B.Controls.Add(this.rdbTw_Bz3);
            this.pnlTw_B.Controls.Add(this.rdbTw_Bz2);
            this.pnlTw_B.Controls.Add(this.rdbTw_Bz1);
            this.pnlTw_B.Controls.Add(this.lblTw_B_T);
            this.pnlTw_B.Name = "pnlTw_B";
            this.pnlTw_B.Tag = "3";
            // 
            // rdbTw_BzOff
            // 
            resources.ApplyResources(this.rdbTw_BzOff, "rdbTw_BzOff");
            this.rdbTw_BzOff.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_BzOff.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_BzOff.FlatAppearance.BorderSize = 2;
            this.rdbTw_BzOff.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_BzOff.Name = "rdbTw_BzOff";
            this.rdbTw_BzOff.TabStop = true;
            this.rdbTw_BzOff.Tag = "0";
            this.rdbTw_BzOff.UseVisualStyleBackColor = false;
            // 
            // rdbTw_Bz3
            // 
            resources.ApplyResources(this.rdbTw_Bz3, "rdbTw_Bz3");
            this.rdbTw_Bz3.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_Bz3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_Bz3.FlatAppearance.BorderSize = 2;
            this.rdbTw_Bz3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_Bz3.Name = "rdbTw_Bz3";
            this.rdbTw_Bz3.TabStop = true;
            this.rdbTw_Bz3.Tag = "3";
            this.rdbTw_Bz3.UseVisualStyleBackColor = false;
            // 
            // rdbTw_Bz2
            // 
            resources.ApplyResources(this.rdbTw_Bz2, "rdbTw_Bz2");
            this.rdbTw_Bz2.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_Bz2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_Bz2.FlatAppearance.BorderSize = 2;
            this.rdbTw_Bz2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_Bz2.Name = "rdbTw_Bz2";
            this.rdbTw_Bz2.TabStop = true;
            this.rdbTw_Bz2.Tag = "2";
            this.rdbTw_Bz2.UseVisualStyleBackColor = false;
            // 
            // rdbTw_Bz1
            // 
            resources.ApplyResources(this.rdbTw_Bz1, "rdbTw_Bz1");
            this.rdbTw_Bz1.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_Bz1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_Bz1.FlatAppearance.BorderSize = 2;
            this.rdbTw_Bz1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_Bz1.Name = "rdbTw_Bz1";
            this.rdbTw_Bz1.TabStop = true;
            this.rdbTw_Bz1.Tag = "1";
            this.rdbTw_Bz1.UseVisualStyleBackColor = false;
            // 
            // lblTw_B_T
            // 
            resources.ApplyResources(this.lblTw_B_T, "lblTw_B_T");
            this.lblTw_B_T.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lblTw_B_T.ForeColor = System.Drawing.Color.White;
            this.lblTw_B_T.Name = "lblTw_B_T";
            // 
            // pnlTw_G
            // 
            resources.ApplyResources(this.pnlTw_G, "pnlTw_G");
            this.pnlTw_G.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTw_G.Controls.Add(this.rdbTw_GFck);
            this.pnlTw_G.Controls.Add(this.rdbTw_GOff);
            this.pnlTw_G.Controls.Add(this.rdbTw_GOn);
            this.pnlTw_G.Controls.Add(this.lblTw_G_T);
            this.pnlTw_G.Name = "pnlTw_G";
            this.pnlTw_G.Tag = "2";
            // 
            // rdbTw_GFck
            // 
            resources.ApplyResources(this.rdbTw_GFck, "rdbTw_GFck");
            this.rdbTw_GFck.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_GFck.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_GFck.FlatAppearance.BorderSize = 2;
            this.rdbTw_GFck.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_GFck.Name = "rdbTw_GFck";
            this.rdbTw_GFck.TabStop = true;
            this.rdbTw_GFck.Tag = "2";
            this.rdbTw_GFck.UseVisualStyleBackColor = false;
            // 
            // rdbTw_GOff
            // 
            resources.ApplyResources(this.rdbTw_GOff, "rdbTw_GOff");
            this.rdbTw_GOff.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_GOff.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_GOff.FlatAppearance.BorderSize = 2;
            this.rdbTw_GOff.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_GOff.Name = "rdbTw_GOff";
            this.rdbTw_GOff.TabStop = true;
            this.rdbTw_GOff.Tag = "0";
            this.rdbTw_GOff.UseVisualStyleBackColor = false;
            // 
            // rdbTw_GOn
            // 
            resources.ApplyResources(this.rdbTw_GOn, "rdbTw_GOn");
            this.rdbTw_GOn.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_GOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_GOn.FlatAppearance.BorderSize = 2;
            this.rdbTw_GOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_GOn.Name = "rdbTw_GOn";
            this.rdbTw_GOn.TabStop = true;
            this.rdbTw_GOn.Tag = "1";
            this.rdbTw_GOn.UseVisualStyleBackColor = false;
            // 
            // lblTw_G_T
            // 
            resources.ApplyResources(this.lblTw_G_T, "lblTw_G_T");
            this.lblTw_G_T.BackColor = System.Drawing.Color.Green;
            this.lblTw_G_T.ForeColor = System.Drawing.Color.White;
            this.lblTw_G_T.Name = "lblTw_G_T";
            // 
            // pnlTw_Y
            // 
            resources.ApplyResources(this.pnlTw_Y, "pnlTw_Y");
            this.pnlTw_Y.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTw_Y.Controls.Add(this.rdbTw_YFck);
            this.pnlTw_Y.Controls.Add(this.rdbTw_YOff);
            this.pnlTw_Y.Controls.Add(this.rdbTw_YOn);
            this.pnlTw_Y.Controls.Add(this.lblTw_Y_T);
            this.pnlTw_Y.Name = "pnlTw_Y";
            this.pnlTw_Y.Tag = "1";
            // 
            // rdbTw_YFck
            // 
            resources.ApplyResources(this.rdbTw_YFck, "rdbTw_YFck");
            this.rdbTw_YFck.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_YFck.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_YFck.FlatAppearance.BorderSize = 2;
            this.rdbTw_YFck.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_YFck.Name = "rdbTw_YFck";
            this.rdbTw_YFck.TabStop = true;
            this.rdbTw_YFck.Tag = "2";
            this.rdbTw_YFck.UseVisualStyleBackColor = false;
            // 
            // rdbTw_YOff
            // 
            resources.ApplyResources(this.rdbTw_YOff, "rdbTw_YOff");
            this.rdbTw_YOff.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_YOff.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_YOff.FlatAppearance.BorderSize = 2;
            this.rdbTw_YOff.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_YOff.Name = "rdbTw_YOff";
            this.rdbTw_YOff.TabStop = true;
            this.rdbTw_YOff.Tag = "0";
            this.rdbTw_YOff.UseVisualStyleBackColor = false;
            // 
            // rdbTw_YOn
            // 
            resources.ApplyResources(this.rdbTw_YOn, "rdbTw_YOn");
            this.rdbTw_YOn.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_YOn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_YOn.FlatAppearance.BorderSize = 2;
            this.rdbTw_YOn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_YOn.Name = "rdbTw_YOn";
            this.rdbTw_YOn.TabStop = true;
            this.rdbTw_YOn.Tag = "1";
            this.rdbTw_YOn.UseVisualStyleBackColor = false;
            // 
            // lblTw_Y_T
            // 
            resources.ApplyResources(this.lblTw_Y_T, "lblTw_Y_T");
            this.lblTw_Y_T.BackColor = System.Drawing.Color.Gold;
            this.lblTw_Y_T.ForeColor = System.Drawing.Color.Black;
            this.lblTw_Y_T.Name = "lblTw_Y_T";
            // 
            // pnlTw_R
            // 
            resources.ApplyResources(this.pnlTw_R, "pnlTw_R");
            this.pnlTw_R.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTw_R.Controls.Add(this.rdbTw_RFck);
            this.pnlTw_R.Controls.Add(this.rdbTw_ROff);
            this.pnlTw_R.Controls.Add(this.rdbTw_ROn);
            this.pnlTw_R.Controls.Add(this.lblTw_R_T);
            this.pnlTw_R.Name = "pnlTw_R";
            this.pnlTw_R.Tag = "0";
            // 
            // rdbTw_RFck
            // 
            resources.ApplyResources(this.rdbTw_RFck, "rdbTw_RFck");
            this.rdbTw_RFck.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_RFck.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_RFck.FlatAppearance.BorderSize = 2;
            this.rdbTw_RFck.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_RFck.Name = "rdbTw_RFck";
            this.rdbTw_RFck.TabStop = true;
            this.rdbTw_RFck.Tag = "2";
            this.rdbTw_RFck.UseVisualStyleBackColor = false;
            // 
            // rdbTw_ROff
            // 
            resources.ApplyResources(this.rdbTw_ROff, "rdbTw_ROff");
            this.rdbTw_ROff.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_ROff.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_ROff.FlatAppearance.BorderSize = 2;
            this.rdbTw_ROff.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_ROff.Name = "rdbTw_ROff";
            this.rdbTw_ROff.TabStop = true;
            this.rdbTw_ROff.Tag = "0";
            this.rdbTw_ROff.UseVisualStyleBackColor = false;
            // 
            // rdbTw_ROn
            // 
            resources.ApplyResources(this.rdbTw_ROn, "rdbTw_ROn");
            this.rdbTw_ROn.BackColor = System.Drawing.Color.LightGray;
            this.rdbTw_ROn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rdbTw_ROn.FlatAppearance.BorderSize = 2;
            this.rdbTw_ROn.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.rdbTw_ROn.Name = "rdbTw_ROn";
            this.rdbTw_ROn.TabStop = true;
            this.rdbTw_ROn.Tag = "1";
            this.rdbTw_ROn.UseVisualStyleBackColor = false;
            // 
            // lblTw_R_T
            // 
            resources.ApplyResources(this.lblTw_R_T, "lblTw_R_T");
            this.lblTw_R_T.BackColor = System.Drawing.Color.Firebrick;
            this.lblTw_R_T.ForeColor = System.Drawing.Color.White;
            this.lblTw_R_T.Name = "lblTw_R_T";
            // 
            // dgvTw_List
            // 
            resources.ApplyResources(this.dgvTw_List, "dgvTw_List");
            this.dgvTw_List.AllowUserToAddRows = false;
            this.dgvTw_List.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTw_List.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTw_List.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTw_List.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col1,
            this.Col2,
            this.Col3,
            this.Col4,
            this.Col5});
            this.dgvTw_List.Name = "dgvTw_List";
            this.dgvTw_List.ReadOnly = true;
            this.dgvTw_List.RowTemplate.Height = 23;
            this.dgvTw_List.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTw_List.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTw_List_CellClick);
            // 
            // Col1
            // 
            resources.ApplyResources(this.Col1, "Col1");
            this.Col1.Name = "Col1";
            this.Col1.ReadOnly = true;
            this.Col1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Col2
            // 
            resources.ApplyResources(this.Col2, "Col2");
            this.Col2.Name = "Col2";
            this.Col2.ReadOnly = true;
            this.Col2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Col3
            // 
            resources.ApplyResources(this.Col3, "Col3");
            this.Col3.Name = "Col3";
            this.Col3.ReadOnly = true;
            this.Col3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Col4
            // 
            resources.ApplyResources(this.Col4, "Col4");
            this.Col4.Name = "Col4";
            this.Col4.ReadOnly = true;
            this.Col4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Col5
            // 
            resources.ApplyResources(this.Col5, "Col5");
            this.Col5.Name = "Col5";
            this.Col5.ReadOnly = true;
            this.Col5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // lblTw_T
            // 
            resources.ApplyResources(this.lblTw_T, "lblTw_T");
            this.lblTw_T.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lblTw_T.ForeColor = System.Drawing.Color.White;
            this.lblTw_T.Name = "lblTw_T";
            // 
            // panel38
            // 
            resources.ApplyResources(this.panel38, "panel38");
            this.panel38.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel38.Controls.Add(this.label53);
            this.panel38.Controls.Add(this.label54);
            this.panel38.Controls.Add(this.label55);
            this.panel38.Controls.Add(this.label56);
            this.panel38.Controls.Add(this.label57);
            this.panel38.Controls.Add(this.label48);
            this.panel38.Controls.Add(this.label49);
            this.panel38.Controls.Add(this.label50);
            this.panel38.Controls.Add(this.label51);
            this.panel38.Controls.Add(this.label52);
            this.panel38.Controls.Add(this.label43);
            this.panel38.Controls.Add(this.label44);
            this.panel38.Controls.Add(this.label45);
            this.panel38.Controls.Add(this.label46);
            this.panel38.Controls.Add(this.label47);
            this.panel38.Controls.Add(this.label37);
            this.panel38.Controls.Add(this.label38);
            this.panel38.Controls.Add(this.label40);
            this.panel38.Controls.Add(this.label41);
            this.panel38.Controls.Add(this.label42);
            this.panel38.Controls.Add(this.label36);
            this.panel38.Controls.Add(this.label34);
            this.panel38.Controls.Add(this.label33);
            this.panel38.Controls.Add(this.label31);
            this.panel38.Controls.Add(this.lblO_IdleT_T);
            this.panel38.Controls.Add(this.label32);
            this.panel38.Name = "panel38";
            this.panel38.Tag = "0";
            // 
            // label53
            // 
            resources.ApplyResources(this.label53, "label53");
            this.label53.BackColor = System.Drawing.Color.Silver;
            this.label53.ForeColor = System.Drawing.Color.Black;
            this.label53.Name = "label53";
            // 
            // label54
            // 
            resources.ApplyResources(this.label54, "label54");
            this.label54.BackColor = System.Drawing.Color.Silver;
            this.label54.ForeColor = System.Drawing.Color.Black;
            this.label54.Name = "label54";
            // 
            // label55
            // 
            resources.ApplyResources(this.label55, "label55");
            this.label55.BackColor = System.Drawing.Color.Silver;
            this.label55.ForeColor = System.Drawing.Color.Black;
            this.label55.Name = "label55";
            // 
            // label56
            // 
            resources.ApplyResources(this.label56, "label56");
            this.label56.BackColor = System.Drawing.Color.Silver;
            this.label56.ForeColor = System.Drawing.Color.Black;
            this.label56.Name = "label56";
            // 
            // label57
            // 
            resources.ApplyResources(this.label57, "label57");
            this.label57.BackColor = System.Drawing.Color.Silver;
            this.label57.ForeColor = System.Drawing.Color.Black;
            this.label57.Name = "label57";
            // 
            // label48
            // 
            resources.ApplyResources(this.label48, "label48");
            this.label48.BackColor = System.Drawing.Color.LightGray;
            this.label48.ForeColor = System.Drawing.Color.Black;
            this.label48.Name = "label48";
            // 
            // label49
            // 
            resources.ApplyResources(this.label49, "label49");
            this.label49.BackColor = System.Drawing.Color.LightGray;
            this.label49.ForeColor = System.Drawing.Color.Black;
            this.label49.Name = "label49";
            // 
            // label50
            // 
            resources.ApplyResources(this.label50, "label50");
            this.label50.BackColor = System.Drawing.Color.LightGray;
            this.label50.ForeColor = System.Drawing.Color.Black;
            this.label50.Name = "label50";
            // 
            // label51
            // 
            resources.ApplyResources(this.label51, "label51");
            this.label51.BackColor = System.Drawing.Color.LightGray;
            this.label51.ForeColor = System.Drawing.Color.Black;
            this.label51.Name = "label51";
            // 
            // label52
            // 
            resources.ApplyResources(this.label52, "label52");
            this.label52.BackColor = System.Drawing.Color.LightGray;
            this.label52.ForeColor = System.Drawing.Color.Black;
            this.label52.Name = "label52";
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.BackColor = System.Drawing.Color.Silver;
            this.label43.ForeColor = System.Drawing.Color.Black;
            this.label43.Name = "label43";
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.BackColor = System.Drawing.Color.Silver;
            this.label44.ForeColor = System.Drawing.Color.Black;
            this.label44.Name = "label44";
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.BackColor = System.Drawing.Color.Silver;
            this.label45.ForeColor = System.Drawing.Color.Black;
            this.label45.Name = "label45";
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.BackColor = System.Drawing.Color.Silver;
            this.label46.ForeColor = System.Drawing.Color.Black;
            this.label46.Name = "label46";
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.BackColor = System.Drawing.Color.Silver;
            this.label47.ForeColor = System.Drawing.Color.Black;
            this.label47.Name = "label47";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.BackColor = System.Drawing.Color.LightGray;
            this.label37.ForeColor = System.Drawing.Color.Black;
            this.label37.Name = "label37";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.BackColor = System.Drawing.Color.LightGray;
            this.label38.ForeColor = System.Drawing.Color.Black;
            this.label38.Name = "label38";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.BackColor = System.Drawing.Color.LightGray;
            this.label40.ForeColor = System.Drawing.Color.Black;
            this.label40.Name = "label40";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.BackColor = System.Drawing.Color.LightGray;
            this.label41.ForeColor = System.Drawing.Color.Black;
            this.label41.Name = "label41";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.BackColor = System.Drawing.Color.LightGray;
            this.label42.ForeColor = System.Drawing.Color.Black;
            this.label42.Name = "label42";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.BackColor = System.Drawing.Color.DimGray;
            this.label36.ForeColor = System.Drawing.Color.Black;
            this.label36.Name = "label36";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.BackColor = System.Drawing.Color.DimGray;
            this.label34.ForeColor = System.Drawing.Color.Black;
            this.label34.Name = "label34";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.BackColor = System.Drawing.Color.DimGray;
            this.label33.ForeColor = System.Drawing.Color.Black;
            this.label33.Name = "label33";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.BackColor = System.Drawing.Color.DimGray;
            this.label31.ForeColor = System.Drawing.Color.Black;
            this.label31.Name = "label31";
            // 
            // lblO_IdleT_T
            // 
            resources.ApplyResources(this.lblO_IdleT_T, "lblO_IdleT_T");
            this.lblO_IdleT_T.BackColor = System.Drawing.Color.DimGray;
            this.lblO_IdleT_T.ForeColor = System.Drawing.Color.Black;
            this.lblO_IdleT_T.Name = "lblO_IdleT_T";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label32.ForeColor = System.Drawing.Color.White;
            this.label32.Name = "label32";
            // 
            // vwEqu_07Twl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlTw_List);
            this.Controls.Add(this.panel38);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_07Twl";
            this.pnlTw_List.ResumeLayout(false);
            this.pnlTw_B.ResumeLayout(false);
            this.pnlTw_G.ResumeLayout(false);
            this.pnlTw_Y.ResumeLayout(false);
            this.pnlTw_R.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTw_List)).EndInit();
            this.panel38.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTw_List;
        private System.Windows.Forms.Button btnTw_Save;
        private System.Windows.Forms.Button btnTw_Stop;
        private System.Windows.Forms.Button btnTw_Start;
        private System.Windows.Forms.Panel pnlTw_B;
        private System.Windows.Forms.RadioButton rdbTw_BzOff;
        private System.Windows.Forms.RadioButton rdbTw_Bz3;
        private System.Windows.Forms.RadioButton rdbTw_Bz2;
        private System.Windows.Forms.RadioButton rdbTw_Bz1;
        private System.Windows.Forms.Label lblTw_B_T;
        private System.Windows.Forms.Panel pnlTw_G;
        private System.Windows.Forms.RadioButton rdbTw_GFck;
        private System.Windows.Forms.RadioButton rdbTw_GOff;
        private System.Windows.Forms.RadioButton rdbTw_GOn;
        private System.Windows.Forms.Label lblTw_G_T;
        private System.Windows.Forms.Panel pnlTw_Y;
        private System.Windows.Forms.RadioButton rdbTw_YFck;
        private System.Windows.Forms.RadioButton rdbTw_YOff;
        private System.Windows.Forms.RadioButton rdbTw_YOn;
        private System.Windows.Forms.Label lblTw_Y_T;
        private System.Windows.Forms.Panel pnlTw_R;
        private System.Windows.Forms.RadioButton rdbTw_RFck;
        private System.Windows.Forms.RadioButton rdbTw_ROff;
        private System.Windows.Forms.RadioButton rdbTw_ROn;
        private System.Windows.Forms.Label lblTw_R_T;
        private System.Windows.Forms.DataGridView dgvTw_List;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col5;
        private System.Windows.Forms.Label lblTw_T;
        private System.Windows.Forms.Panel panel38;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label lblO_IdleT_T;
        private System.Windows.Forms.Label label32;
    }
}
