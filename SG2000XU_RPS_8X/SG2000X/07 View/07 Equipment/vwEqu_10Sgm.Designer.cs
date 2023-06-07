namespace SG2000X
{
    partial class vwEqu_10Sgm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_10Sgm));
            this.panel15 = new System.Windows.Forms.Panel();
            this.btn10_FileSearch = new System.Windows.Forms.Button();
            this.cmb10_Rcp = new System.Windows.Forms.ComboBox();
            this.btn10_RcpAdd = new System.Windows.Forms.Button();
            this.btn10_RcpEdit = new System.Windows.Forms.Button();
            this.btn10_RcpDel = new System.Windows.Forms.Button();
            this.btn10_RcpChange = new System.Windows.Forms.Button();
            this.txt10_RcpAdd = new System.Windows.Forms.TextBox();
            this.label149 = new System.Windows.Forms.Label();
            this.panel14 = new System.Windows.Forms.Panel();
            this.txt10_Alr = new System.Windows.Forms.TextBox();
            this.btn10_AlrReset = new System.Windows.Forms.Button();
            this.btn10_AlrSet = new System.Windows.Forms.Button();
            this.label138 = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.btn10_StripBase = new System.Windows.Forms.Button();
            this.btn10_LotBase = new System.Windows.Forms.Button();
            this.label134 = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.btn10_Down = new System.Windows.Forms.Button();
            this.btn10_Run = new System.Windows.Forms.Button();
            this.btn10_Ready = new System.Windows.Forms.Button();
            this.btn10_Idle = new System.Windows.Forms.Button();
            this.label148 = new System.Windows.Forms.Label();
            this.label146 = new System.Windows.Forms.Label();
            this.label145 = new System.Windows.Forms.Label();
            this.label143 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.btn10_LotEnd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn10_StripUnload = new System.Windows.Forms.Button();
            this.label154 = new System.Windows.Forms.Label();
            this.btn10_StripReadFail = new System.Windows.Forms.Button();
            this.btn10_StripIdKey = new System.Windows.Forms.Button();
            this.btn10_StripIdRead = new System.Windows.Forms.Button();
            this.txt10_StripID = new System.Windows.Forms.TextBox();
            this.label155 = new System.Windows.Forms.Label();
            this.btn10_StripLoad = new System.Windows.Forms.Button();
            this.txt10_StripNo = new System.Windows.Forms.TextBox();
            this.label157 = new System.Windows.Forms.Label();
            this.btn10_LotStart = new System.Windows.Forms.Button();
            this.label152 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt10_LotName = new System.Windows.Forms.TextBox();
            this.label151 = new System.Windows.Forms.Label();
            this.btn10_LotIdKey = new System.Windows.Forms.Button();
            this.txt10_MgzCnt = new System.Windows.Forms.TextBox();
            this.label139 = new System.Windows.Forms.Label();
            this.btn10_LotIdRead = new System.Windows.Forms.Button();
            this.txt10_DevID = new System.Windows.Forms.TextBox();
            this.label140 = new System.Windows.Forms.Label();
            this.label142 = new System.Windows.Forms.Label();
            this.panel15.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel11.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel15
            // 
            resources.ApplyResources(this.panel15, "panel15");
            this.panel15.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel15.Controls.Add(this.btn10_FileSearch);
            this.panel15.Controls.Add(this.cmb10_Rcp);
            this.panel15.Controls.Add(this.btn10_RcpAdd);
            this.panel15.Controls.Add(this.btn10_RcpEdit);
            this.panel15.Controls.Add(this.btn10_RcpDel);
            this.panel15.Controls.Add(this.btn10_RcpChange);
            this.panel15.Controls.Add(this.txt10_RcpAdd);
            this.panel15.Controls.Add(this.label149);
            this.panel15.Name = "panel15";
            // 
            // btn10_FileSearch
            // 
            resources.ApplyResources(this.btn10_FileSearch, "btn10_FileSearch");
            this.btn10_FileSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_FileSearch.ForeColor = System.Drawing.Color.White;
            this.btn10_FileSearch.Name = "btn10_FileSearch";
            this.btn10_FileSearch.Tag = "";
            this.btn10_FileSearch.UseVisualStyleBackColor = false;
            // 
            // cmb10_Rcp
            // 
            resources.ApplyResources(this.cmb10_Rcp, "cmb10_Rcp");
            this.cmb10_Rcp.FormattingEnabled = true;
            this.cmb10_Rcp.Name = "cmb10_Rcp";
            // 
            // btn10_RcpAdd
            // 
            resources.ApplyResources(this.btn10_RcpAdd, "btn10_RcpAdd");
            this.btn10_RcpAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_RcpAdd.ForeColor = System.Drawing.Color.White;
            this.btn10_RcpAdd.Name = "btn10_RcpAdd";
            this.btn10_RcpAdd.Tag = "";
            this.btn10_RcpAdd.UseVisualStyleBackColor = false;
            // 
            // btn10_RcpEdit
            // 
            resources.ApplyResources(this.btn10_RcpEdit, "btn10_RcpEdit");
            this.btn10_RcpEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_RcpEdit.ForeColor = System.Drawing.Color.White;
            this.btn10_RcpEdit.Name = "btn10_RcpEdit";
            this.btn10_RcpEdit.Tag = "";
            this.btn10_RcpEdit.UseVisualStyleBackColor = false;
            // 
            // btn10_RcpDel
            // 
            resources.ApplyResources(this.btn10_RcpDel, "btn10_RcpDel");
            this.btn10_RcpDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_RcpDel.ForeColor = System.Drawing.Color.White;
            this.btn10_RcpDel.Name = "btn10_RcpDel";
            this.btn10_RcpDel.Tag = "";
            this.btn10_RcpDel.UseVisualStyleBackColor = false;
            // 
            // btn10_RcpChange
            // 
            resources.ApplyResources(this.btn10_RcpChange, "btn10_RcpChange");
            this.btn10_RcpChange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_RcpChange.ForeColor = System.Drawing.Color.White;
            this.btn10_RcpChange.Name = "btn10_RcpChange";
            this.btn10_RcpChange.Tag = "";
            this.btn10_RcpChange.UseVisualStyleBackColor = false;
            // 
            // txt10_RcpAdd
            // 
            resources.ApplyResources(this.txt10_RcpAdd, "txt10_RcpAdd");
            this.txt10_RcpAdd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_RcpAdd.Name = "txt10_RcpAdd";
            // 
            // label149
            // 
            resources.ApplyResources(this.label149, "label149");
            this.label149.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label149.ForeColor = System.Drawing.Color.White;
            this.label149.Name = "label149";
            // 
            // panel14
            // 
            resources.ApplyResources(this.panel14, "panel14");
            this.panel14.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel14.Controls.Add(this.txt10_Alr);
            this.panel14.Controls.Add(this.btn10_AlrReset);
            this.panel14.Controls.Add(this.btn10_AlrSet);
            this.panel14.Controls.Add(this.label138);
            this.panel14.Name = "panel14";
            // 
            // txt10_Alr
            // 
            resources.ApplyResources(this.txt10_Alr, "txt10_Alr");
            this.txt10_Alr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_Alr.Name = "txt10_Alr";
            // 
            // btn10_AlrReset
            // 
            resources.ApplyResources(this.btn10_AlrReset, "btn10_AlrReset");
            this.btn10_AlrReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_AlrReset.ForeColor = System.Drawing.Color.White;
            this.btn10_AlrReset.Name = "btn10_AlrReset";
            this.btn10_AlrReset.Tag = "";
            this.btn10_AlrReset.UseVisualStyleBackColor = false;
            // 
            // btn10_AlrSet
            // 
            resources.ApplyResources(this.btn10_AlrSet, "btn10_AlrSet");
            this.btn10_AlrSet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_AlrSet.ForeColor = System.Drawing.Color.White;
            this.btn10_AlrSet.Name = "btn10_AlrSet";
            this.btn10_AlrSet.Tag = "";
            this.btn10_AlrSet.UseVisualStyleBackColor = false;
            // 
            // label138
            // 
            resources.ApplyResources(this.label138, "label138");
            this.label138.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label138.ForeColor = System.Drawing.Color.White;
            this.label138.Name = "label138";
            // 
            // panel13
            // 
            resources.ApplyResources(this.panel13, "panel13");
            this.panel13.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel13.Controls.Add(this.btn10_StripBase);
            this.panel13.Controls.Add(this.btn10_LotBase);
            this.panel13.Controls.Add(this.label134);
            this.panel13.Name = "panel13";
            // 
            // btn10_StripBase
            // 
            resources.ApplyResources(this.btn10_StripBase, "btn10_StripBase");
            this.btn10_StripBase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_StripBase.ForeColor = System.Drawing.Color.White;
            this.btn10_StripBase.Name = "btn10_StripBase";
            this.btn10_StripBase.Tag = "";
            this.btn10_StripBase.UseVisualStyleBackColor = false;
            // 
            // btn10_LotBase
            // 
            resources.ApplyResources(this.btn10_LotBase, "btn10_LotBase");
            this.btn10_LotBase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_LotBase.ForeColor = System.Drawing.Color.White;
            this.btn10_LotBase.Name = "btn10_LotBase";
            this.btn10_LotBase.Tag = "";
            this.btn10_LotBase.UseVisualStyleBackColor = false;
            // 
            // label134
            // 
            resources.ApplyResources(this.label134, "label134");
            this.label134.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label134.ForeColor = System.Drawing.Color.White;
            this.label134.Name = "label134";
            // 
            // panel12
            // 
            resources.ApplyResources(this.panel12, "panel12");
            this.panel12.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel12.Controls.Add(this.btn10_Down);
            this.panel12.Controls.Add(this.btn10_Run);
            this.panel12.Controls.Add(this.btn10_Ready);
            this.panel12.Controls.Add(this.btn10_Idle);
            this.panel12.Controls.Add(this.label148);
            this.panel12.Name = "panel12";
            // 
            // btn10_Down
            // 
            resources.ApplyResources(this.btn10_Down, "btn10_Down");
            this.btn10_Down.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_Down.ForeColor = System.Drawing.Color.White;
            this.btn10_Down.Name = "btn10_Down";
            this.btn10_Down.Tag = "";
            this.btn10_Down.UseVisualStyleBackColor = false;
            // 
            // btn10_Run
            // 
            resources.ApplyResources(this.btn10_Run, "btn10_Run");
            this.btn10_Run.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_Run.ForeColor = System.Drawing.Color.White;
            this.btn10_Run.Name = "btn10_Run";
            this.btn10_Run.Tag = "";
            this.btn10_Run.UseVisualStyleBackColor = false;
            // 
            // btn10_Ready
            // 
            resources.ApplyResources(this.btn10_Ready, "btn10_Ready");
            this.btn10_Ready.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_Ready.ForeColor = System.Drawing.Color.White;
            this.btn10_Ready.Name = "btn10_Ready";
            this.btn10_Ready.Tag = "";
            this.btn10_Ready.UseVisualStyleBackColor = false;
            // 
            // btn10_Idle
            // 
            resources.ApplyResources(this.btn10_Idle, "btn10_Idle");
            this.btn10_Idle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_Idle.ForeColor = System.Drawing.Color.White;
            this.btn10_Idle.Name = "btn10_Idle";
            this.btn10_Idle.Tag = "";
            this.btn10_Idle.UseVisualStyleBackColor = false;
            // 
            // label148
            // 
            resources.ApplyResources(this.label148, "label148");
            this.label148.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label148.ForeColor = System.Drawing.Color.White;
            this.label148.Name = "label148";
            // 
            // label146
            // 
            resources.ApplyResources(this.label146, "label146");
            this.label146.BackColor = System.Drawing.SystemColors.Control;
            this.label146.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label146.Name = "label146";
            // 
            // label145
            // 
            resources.ApplyResources(this.label145, "label145");
            this.label145.BackColor = System.Drawing.SystemColors.Control;
            this.label145.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label145.Name = "label145";
            // 
            // label143
            // 
            resources.ApplyResources(this.label143, "label143");
            this.label143.BackColor = System.Drawing.SystemColors.Control;
            this.label143.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label143.Name = "label143";
            // 
            // panel11
            // 
            resources.ApplyResources(this.panel11, "panel11");
            this.panel11.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel11.Controls.Add(this.btn10_LotEnd);
            this.panel11.Controls.Add(this.groupBox2);
            this.panel11.Controls.Add(this.btn10_LotStart);
            this.panel11.Controls.Add(this.label152);
            this.panel11.Controls.Add(this.groupBox1);
            this.panel11.Controls.Add(this.label142);
            this.panel11.Name = "panel11";
            // 
            // btn10_LotEnd
            // 
            resources.ApplyResources(this.btn10_LotEnd, "btn10_LotEnd");
            this.btn10_LotEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_LotEnd.ForeColor = System.Drawing.Color.White;
            this.btn10_LotEnd.Name = "btn10_LotEnd";
            this.btn10_LotEnd.Tag = "";
            this.btn10_LotEnd.UseVisualStyleBackColor = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.btn10_StripUnload);
            this.groupBox2.Controls.Add(this.label154);
            this.groupBox2.Controls.Add(this.btn10_StripReadFail);
            this.groupBox2.Controls.Add(this.btn10_StripIdKey);
            this.groupBox2.Controls.Add(this.btn10_StripIdRead);
            this.groupBox2.Controls.Add(this.txt10_StripID);
            this.groupBox2.Controls.Add(this.label155);
            this.groupBox2.Controls.Add(this.btn10_StripLoad);
            this.groupBox2.Controls.Add(this.txt10_StripNo);
            this.groupBox2.Controls.Add(this.label157);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btn10_StripUnload
            // 
            resources.ApplyResources(this.btn10_StripUnload, "btn10_StripUnload");
            this.btn10_StripUnload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_StripUnload.ForeColor = System.Drawing.Color.White;
            this.btn10_StripUnload.Name = "btn10_StripUnload";
            this.btn10_StripUnload.Tag = "";
            this.btn10_StripUnload.UseVisualStyleBackColor = false;
            // 
            // label154
            // 
            resources.ApplyResources(this.label154, "label154");
            this.label154.ForeColor = System.Drawing.Color.Red;
            this.label154.Name = "label154";
            // 
            // btn10_StripReadFail
            // 
            resources.ApplyResources(this.btn10_StripReadFail, "btn10_StripReadFail");
            this.btn10_StripReadFail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_StripReadFail.ForeColor = System.Drawing.Color.White;
            this.btn10_StripReadFail.Name = "btn10_StripReadFail";
            this.btn10_StripReadFail.Tag = "";
            this.btn10_StripReadFail.UseVisualStyleBackColor = false;
            // 
            // btn10_StripIdKey
            // 
            resources.ApplyResources(this.btn10_StripIdKey, "btn10_StripIdKey");
            this.btn10_StripIdKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_StripIdKey.ForeColor = System.Drawing.Color.White;
            this.btn10_StripIdKey.Name = "btn10_StripIdKey";
            this.btn10_StripIdKey.Tag = "";
            this.btn10_StripIdKey.UseVisualStyleBackColor = false;
            // 
            // btn10_StripIdRead
            // 
            resources.ApplyResources(this.btn10_StripIdRead, "btn10_StripIdRead");
            this.btn10_StripIdRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_StripIdRead.ForeColor = System.Drawing.Color.White;
            this.btn10_StripIdRead.Name = "btn10_StripIdRead";
            this.btn10_StripIdRead.Tag = "";
            this.btn10_StripIdRead.UseVisualStyleBackColor = false;
            // 
            // txt10_StripID
            // 
            resources.ApplyResources(this.txt10_StripID, "txt10_StripID");
            this.txt10_StripID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_StripID.Name = "txt10_StripID";
            // 
            // label155
            // 
            resources.ApplyResources(this.label155, "label155");
            this.label155.Name = "label155";
            // 
            // btn10_StripLoad
            // 
            resources.ApplyResources(this.btn10_StripLoad, "btn10_StripLoad");
            this.btn10_StripLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_StripLoad.ForeColor = System.Drawing.Color.White;
            this.btn10_StripLoad.Name = "btn10_StripLoad";
            this.btn10_StripLoad.Tag = "";
            this.btn10_StripLoad.UseVisualStyleBackColor = false;
            // 
            // txt10_StripNo
            // 
            resources.ApplyResources(this.txt10_StripNo, "txt10_StripNo");
            this.txt10_StripNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_StripNo.Name = "txt10_StripNo";
            // 
            // label157
            // 
            resources.ApplyResources(this.label157, "label157");
            this.label157.Name = "label157";
            // 
            // btn10_LotStart
            // 
            resources.ApplyResources(this.btn10_LotStart, "btn10_LotStart");
            this.btn10_LotStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_LotStart.ForeColor = System.Drawing.Color.White;
            this.btn10_LotStart.Name = "btn10_LotStart";
            this.btn10_LotStart.Tag = "";
            this.btn10_LotStart.UseVisualStyleBackColor = false;
            // 
            // label152
            // 
            resources.ApplyResources(this.label152, "label152");
            this.label152.ForeColor = System.Drawing.Color.Red;
            this.label152.Name = "label152";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.txt10_LotName);
            this.groupBox1.Controls.Add(this.label151);
            this.groupBox1.Controls.Add(this.btn10_LotIdKey);
            this.groupBox1.Controls.Add(this.txt10_MgzCnt);
            this.groupBox1.Controls.Add(this.label139);
            this.groupBox1.Controls.Add(this.btn10_LotIdRead);
            this.groupBox1.Controls.Add(this.txt10_DevID);
            this.groupBox1.Controls.Add(this.label140);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txt10_LotName
            // 
            resources.ApplyResources(this.txt10_LotName, "txt10_LotName");
            this.txt10_LotName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_LotName.Name = "txt10_LotName";
            // 
            // label151
            // 
            resources.ApplyResources(this.label151, "label151");
            this.label151.Name = "label151";
            // 
            // btn10_LotIdKey
            // 
            resources.ApplyResources(this.btn10_LotIdKey, "btn10_LotIdKey");
            this.btn10_LotIdKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_LotIdKey.ForeColor = System.Drawing.Color.White;
            this.btn10_LotIdKey.Name = "btn10_LotIdKey";
            this.btn10_LotIdKey.Tag = "";
            this.btn10_LotIdKey.UseVisualStyleBackColor = false;
            // 
            // txt10_MgzCnt
            // 
            resources.ApplyResources(this.txt10_MgzCnt, "txt10_MgzCnt");
            this.txt10_MgzCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_MgzCnt.Name = "txt10_MgzCnt";
            // 
            // label139
            // 
            resources.ApplyResources(this.label139, "label139");
            this.label139.Name = "label139";
            // 
            // btn10_LotIdRead
            // 
            resources.ApplyResources(this.btn10_LotIdRead, "btn10_LotIdRead");
            this.btn10_LotIdRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn10_LotIdRead.ForeColor = System.Drawing.Color.White;
            this.btn10_LotIdRead.Name = "btn10_LotIdRead";
            this.btn10_LotIdRead.Tag = "";
            this.btn10_LotIdRead.UseVisualStyleBackColor = false;
            // 
            // txt10_DevID
            // 
            resources.ApplyResources(this.txt10_DevID, "txt10_DevID");
            this.txt10_DevID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10_DevID.Name = "txt10_DevID";
            // 
            // label140
            // 
            resources.ApplyResources(this.label140, "label140");
            this.label140.Name = "label140";
            // 
            // label142
            // 
            resources.ApplyResources(this.label142, "label142");
            this.label142.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.label142.ForeColor = System.Drawing.Color.White;
            this.label142.Name = "label142";
            // 
            // vwEqu_10Sgm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel15);
            this.Controls.Add(this.panel14);
            this.Controls.Add(this.panel13);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.label146);
            this.Controls.Add(this.label145);
            this.Controls.Add(this.label143);
            this.Controls.Add(this.panel11);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_10Sgm";
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.panel13.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Button btn10_FileSearch;
        private System.Windows.Forms.ComboBox cmb10_Rcp;
        private System.Windows.Forms.Button btn10_RcpAdd;
        private System.Windows.Forms.Button btn10_RcpEdit;
        private System.Windows.Forms.Button btn10_RcpDel;
        private System.Windows.Forms.Button btn10_RcpChange;
        private System.Windows.Forms.TextBox txt10_RcpAdd;
        private System.Windows.Forms.Label label149;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.TextBox txt10_Alr;
        private System.Windows.Forms.Button btn10_AlrReset;
        private System.Windows.Forms.Button btn10_AlrSet;
        private System.Windows.Forms.Label label138;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Button btn10_StripBase;
        private System.Windows.Forms.Button btn10_LotBase;
        private System.Windows.Forms.Label label134;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button btn10_Down;
        private System.Windows.Forms.Button btn10_Run;
        private System.Windows.Forms.Button btn10_Ready;
        private System.Windows.Forms.Button btn10_Idle;
        private System.Windows.Forms.Label label148;
        private System.Windows.Forms.Label label146;
        private System.Windows.Forms.Label label145;
        private System.Windows.Forms.Label label143;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Button btn10_LotEnd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn10_StripUnload;
        private System.Windows.Forms.Label label154;
        private System.Windows.Forms.Button btn10_StripReadFail;
        private System.Windows.Forms.Button btn10_StripIdKey;
        private System.Windows.Forms.Button btn10_StripIdRead;
        private System.Windows.Forms.TextBox txt10_StripID;
        private System.Windows.Forms.Label label155;
        private System.Windows.Forms.Button btn10_StripLoad;
        private System.Windows.Forms.TextBox txt10_StripNo;
        private System.Windows.Forms.Label label157;
        private System.Windows.Forms.Button btn10_LotStart;
        private System.Windows.Forms.Label label152;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt10_LotName;
        private System.Windows.Forms.Label label151;
        private System.Windows.Forms.Button btn10_LotIdKey;
        private System.Windows.Forms.TextBox txt10_MgzCnt;
        private System.Windows.Forms.Label label139;
        private System.Windows.Forms.Button btn10_LotIdRead;
        private System.Windows.Forms.TextBox txt10_DevID;
        private System.Windows.Forms.Label label140;
        private System.Windows.Forms.Label label142;
    }
}
