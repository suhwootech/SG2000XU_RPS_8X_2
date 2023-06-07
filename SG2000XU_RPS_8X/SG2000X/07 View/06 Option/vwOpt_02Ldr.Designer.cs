namespace SG2000X
{
    partial class vwOpt_02Ldr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwOpt_02Ldr));
            this.panel52 = new System.Windows.Forms.Panel();
            this.ckb_MgzFirstTopSlotOff = new System.Windows.Forms.CheckBox();
            this.label247 = new System.Windows.Forms.Label();
            this.ckb_MgzFirstTopSlot = new System.Windows.Forms.CheckBox();
            this.label377 = new System.Windows.Forms.Label();
            this.ckb_MgzMatchingOn = new System.Windows.Forms.CheckBox();
            this.label154 = new System.Windows.Forms.Label();
            this.panel48 = new System.Windows.Forms.Panel();
            this.cmb_Emit = new System.Windows.Forms.ComboBox();
            this.label308 = new System.Windows.Forms.Label();
            this.label148 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.txt_EmptySlotCnt = new System.Windows.Forms.TextBox();
            this.label98 = new System.Windows.Forms.Label();
            this.label105 = new System.Windows.Forms.Label();
            this.pnl_RfidUse = new System.Windows.Forms.Panel();
            this.ckb_OflRfidUse = new System.Windows.Forms.CheckBox();
            this.ckb_OnlRfidUse = new System.Windows.Forms.CheckBox();
            this.lbl_RFID_T = new System.Windows.Forms.Label();
            this.ckb_MgzUnloadType = new System.Windows.Forms.CheckBox();
            this.panel52.SuspendLayout();
            this.panel48.SuspendLayout();
            this.pnl_RfidUse.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel52
            // 
            this.panel52.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel52.Controls.Add(this.ckb_MgzFirstTopSlotOff);
            this.panel52.Controls.Add(this.label247);
            this.panel52.Controls.Add(this.ckb_MgzFirstTopSlot);
            this.panel52.Controls.Add(this.label377);
            this.panel52.Controls.Add(this.ckb_MgzUnloadType);
            this.panel52.Controls.Add(this.ckb_MgzMatchingOn);
            this.panel52.Controls.Add(this.label154);
            resources.ApplyResources(this.panel52, "panel52");
            this.panel52.Name = "panel52";
            this.panel52.Tag = "0";
            // 
            // ckb_MgzFirstTopSlotOff
            // 
            resources.ApplyResources(this.ckb_MgzFirstTopSlotOff, "ckb_MgzFirstTopSlotOff");
            this.ckb_MgzFirstTopSlotOff.Name = "ckb_MgzFirstTopSlotOff";
            this.ckb_MgzFirstTopSlotOff.UseVisualStyleBackColor = true;
            // 
            // label247
            // 
            resources.ApplyResources(this.label247, "label247");
            this.label247.ForeColor = System.Drawing.Color.Red;
            this.label247.Name = "label247";
            // 
            // ckb_MgzFirstTopSlot
            // 
            resources.ApplyResources(this.ckb_MgzFirstTopSlot, "ckb_MgzFirstTopSlot");
            this.ckb_MgzFirstTopSlot.Name = "ckb_MgzFirstTopSlot";
            this.ckb_MgzFirstTopSlot.UseVisualStyleBackColor = true;
            // 
            // label377
            // 
            resources.ApplyResources(this.label377, "label377");
            this.label377.ForeColor = System.Drawing.Color.Red;
            this.label377.Name = "label377";
            // 
            // ckb_MgzMatchingOn
            // 
            resources.ApplyResources(this.ckb_MgzMatchingOn, "ckb_MgzMatchingOn");
            this.ckb_MgzMatchingOn.Name = "ckb_MgzMatchingOn";
            this.ckb_MgzMatchingOn.UseVisualStyleBackColor = true;
            // 
            // label154
            // 
            this.label154.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label154, "label154");
            this.label154.ForeColor = System.Drawing.Color.White;
            this.label154.Name = "label154";
            // 
            // panel48
            // 
            this.panel48.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel48.Controls.Add(this.cmb_Emit);
            this.panel48.Controls.Add(this.label308);
            this.panel48.Controls.Add(this.label148);
            this.panel48.Controls.Add(this.label84);
            this.panel48.Controls.Add(this.txt_EmptySlotCnt);
            this.panel48.Controls.Add(this.label98);
            this.panel48.Controls.Add(this.label105);
            resources.ApplyResources(this.panel48, "panel48");
            this.panel48.Name = "panel48";
            this.panel48.Tag = "0";
            // 
            // cmb_Emit
            // 
            this.cmb_Emit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmb_Emit, "cmb_Emit");
            this.cmb_Emit.FormattingEnabled = true;
            this.cmb_Emit.Items.AddRange(new object[] {
            resources.GetString("cmb_Emit.Items"),
            resources.GetString("cmb_Emit.Items1")});
            this.cmb_Emit.Name = "cmb_Emit";
            // 
            // label308
            // 
            this.label308.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.label308, "label308");
            this.label308.ForeColor = System.Drawing.Color.Black;
            this.label308.Name = "label308";
            // 
            // label148
            // 
            resources.ApplyResources(this.label148, "label148");
            this.label148.ForeColor = System.Drawing.Color.Red;
            this.label148.Name = "label148";
            // 
            // label84
            // 
            resources.ApplyResources(this.label84, "label84");
            this.label84.ForeColor = System.Drawing.Color.Black;
            this.label84.Name = "label84";
            // 
            // txt_EmptySlotCnt
            // 
            this.txt_EmptySlotCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_EmptySlotCnt, "txt_EmptySlotCnt");
            this.txt_EmptySlotCnt.Name = "txt_EmptySlotCnt";
            this.txt_EmptySlotCnt.Tag = "0";
            // 
            // label98
            // 
            resources.ApplyResources(this.label98, "label98");
            this.label98.ForeColor = System.Drawing.Color.Black;
            this.label98.Name = "label98";
            // 
            // label105
            // 
            this.label105.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.label105, "label105");
            this.label105.ForeColor = System.Drawing.Color.White;
            this.label105.Name = "label105";
            // 
            // pnl_RfidUse
            // 
            this.pnl_RfidUse.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_RfidUse.Controls.Add(this.ckb_OflRfidUse);
            this.pnl_RfidUse.Controls.Add(this.ckb_OnlRfidUse);
            this.pnl_RfidUse.Controls.Add(this.lbl_RFID_T);
            resources.ApplyResources(this.pnl_RfidUse, "pnl_RfidUse");
            this.pnl_RfidUse.Name = "pnl_RfidUse";
            this.pnl_RfidUse.Tag = "0";
            // 
            // ckb_OflRfidUse
            // 
            resources.ApplyResources(this.ckb_OflRfidUse, "ckb_OflRfidUse");
            this.ckb_OflRfidUse.Name = "ckb_OflRfidUse";
            this.ckb_OflRfidUse.UseVisualStyleBackColor = true;
            // 
            // ckb_OnlRfidUse
            // 
            resources.ApplyResources(this.ckb_OnlRfidUse, "ckb_OnlRfidUse");
            this.ckb_OnlRfidUse.Name = "ckb_OnlRfidUse";
            this.ckb_OnlRfidUse.UseVisualStyleBackColor = true;
            // 
            // lbl_RFID_T
            // 
            this.lbl_RFID_T.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.lbl_RFID_T, "lbl_RFID_T");
            this.lbl_RFID_T.ForeColor = System.Drawing.Color.White;
            this.lbl_RFID_T.Name = "lbl_RFID_T";
            // 
            // ckb_MgzUnloadType
            // 
            resources.ApplyResources(this.ckb_MgzUnloadType, "ckb_MgzUnloadType");
            this.ckb_MgzUnloadType.Name = "ckb_MgzUnloadType";
            this.ckb_MgzUnloadType.UseVisualStyleBackColor = true;
            // 
            // vwOpt_02Ldr
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnl_RfidUse);
            this.Controls.Add(this.panel48);
            this.Controls.Add(this.panel52);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwOpt_02Ldr";
            this.panel52.ResumeLayout(false);
            this.panel48.ResumeLayout(false);
            this.panel48.PerformLayout();
            this.pnl_RfidUse.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel52;
        private System.Windows.Forms.CheckBox ckb_MgzFirstTopSlotOff;
        private System.Windows.Forms.Label label247;
        private System.Windows.Forms.CheckBox ckb_MgzFirstTopSlot;
        private System.Windows.Forms.Label label377;
        private System.Windows.Forms.CheckBox ckb_MgzMatchingOn;
        private System.Windows.Forms.Label label154;
        private System.Windows.Forms.Panel panel48;
        private System.Windows.Forms.ComboBox cmb_Emit;
        private System.Windows.Forms.Label label308;
        private System.Windows.Forms.Label label148;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.TextBox txt_EmptySlotCnt;
        private System.Windows.Forms.Label label98;
        private System.Windows.Forms.Label label105;
        private System.Windows.Forms.Panel pnl_RfidUse;
        private System.Windows.Forms.CheckBox ckb_OflRfidUse;
        private System.Windows.Forms.CheckBox ckb_OnlRfidUse;
        private System.Windows.Forms.Label lbl_RFID_T;
        private System.Windows.Forms.CheckBox ckb_MgzUnloadType;
    }
}
