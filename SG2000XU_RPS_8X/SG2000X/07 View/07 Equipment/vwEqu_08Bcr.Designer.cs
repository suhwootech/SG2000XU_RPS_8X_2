namespace SG2000X
{
    partial class vwEqu_08Bcr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_08Bcr));
            this.lbl_Rfid = new System.Windows.Forms.Label();
            this.btn_BtmRfid = new System.Windows.Forms.Button();
            this.btn_TopRfid = new System.Windows.Forms.Button();
            this.lbl_Ocr = new System.Windows.Forms.Label();
            this.lbl_OcrTitle = new System.Windows.Forms.Label();
            this.btn_Bcr = new System.Windows.Forms.Button();
            this.lbl_Bcr = new System.Windows.Forms.Label();
            this.lbl_BcrTitle = new System.Windows.Forms.Label();
            this.btn_OnlRfid = new System.Windows.Forms.Button();
            this.lbl_OnlRfid = new System.Windows.Forms.Label();
            this.lbl_OriTitle = new System.Windows.Forms.Label();
            this.lbl_Ori = new System.Windows.Forms.Label();
            this.btn_Ori = new System.Windows.Forms.Button();
            this.btn_VisAuto = new System.Windows.Forms.Button();
            this.btn_VisUiAuto = new System.Windows.Forms.Button();
            this.btn_VisUiTrain = new System.Windows.Forms.Button();
            this.lbl_VisProcessTitle = new System.Windows.Forms.Label();
            this.lbl_VisProcess = new System.Windows.Forms.Label();
            this.lbl_VisConnTitle = new System.Windows.Forms.Label();
            this.lbl_VisConn = new System.Windows.Forms.Label();
            this.lbl_VisULevelTitle = new System.Windows.Forms.Label();
            this.lbl_VisULevel = new System.Windows.Forms.Label();
            this.lbl_VisUIModeTitle = new System.Windows.Forms.Label();
            this.lbl_VisUIMode = new System.Windows.Forms.Label();
            this.lbl_VisStatusTitle = new System.Windows.Forms.Label();
            this.lbl_VisStatus = new System.Windows.Forms.Label();
            this.pnl_OnL_Bcr = new System.Windows.Forms.Panel();
            this.lbl_OnL_BcrTcp_Title = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrTcp = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrBcr_Title = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrBcr = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrErr_Title = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrErr = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrCmdResult_Title = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrCmdResult = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrCmd_Title = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrCmd = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrState_Title = new System.Windows.Forms.Label();
            this.lbl_OnL_BcrState = new System.Windows.Forms.Label();
            this.btn_OnL_BCLR = new System.Windows.Forms.Button();
            this.btn_OnL_LOFF = new System.Windows.Forms.Button();
            this.btn_OnL_LON = new System.Windows.Forms.Button();
            this.btn_OnL_BcrConnect = new System.Windows.Forms.Button();
            this.lbl_OnL_Bcr_Title = new System.Windows.Forms.Label();
            this.pnl_OffL_Bcr = new System.Windows.Forms.Panel();
            this.lbl_OffL_BcrTcp_Title = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrTcp = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrBcr_Title = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrBcr = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrErr_Title = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrErr = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrCmdResult_Title = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrCmdResult = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrCmd_Title = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrCmd = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrState_Title = new System.Windows.Forms.Label();
            this.lbl_OffL_BcrState = new System.Windows.Forms.Label();
            this.btn_OffL_BCLR = new System.Windows.Forms.Button();
            this.btn_OffL_LOFF = new System.Windows.Forms.Button();
            this.btn_OffL_LON = new System.Windows.Forms.Button();
            this.btn_OffL_BcrConnect = new System.Windows.Forms.Button();
            this.lbl_OffL_Bcr_Title = new System.Windows.Forms.Label();
            this.btn_BCROnly = new System.Windows.Forms.Button();
            this.btn_OCROnly = new System.Windows.Forms.Button();
            this.btn_BCROCR = new System.Windows.Forms.Button();
            this.pnl_OnL_Bcr.SuspendLayout();
            this.pnl_OffL_Bcr.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Rfid
            // 
            this.lbl_Rfid.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_Rfid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Rfid, "lbl_Rfid");
            this.lbl_Rfid.Name = "lbl_Rfid";
            // 
            // btn_BtmRfid
            // 
            this.btn_BtmRfid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_BtmRfid.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_BtmRfid, "btn_BtmRfid");
            this.btn_BtmRfid.ForeColor = System.Drawing.Color.Lime;
            this.btn_BtmRfid.Name = "btn_BtmRfid";
            this.btn_BtmRfid.Tag = "0";
            this.btn_BtmRfid.UseVisualStyleBackColor = false;
            this.btn_BtmRfid.Click += new System.EventHandler(this.btn_BtmRfid_Click);
            // 
            // btn_TopRfid
            // 
            this.btn_TopRfid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_TopRfid.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_TopRfid, "btn_TopRfid");
            this.btn_TopRfid.ForeColor = System.Drawing.Color.Lime;
            this.btn_TopRfid.Name = "btn_TopRfid";
            this.btn_TopRfid.Tag = "0";
            this.btn_TopRfid.UseVisualStyleBackColor = false;
            this.btn_TopRfid.Click += new System.EventHandler(this.btn_TopRfid_Click);
            // 
            // lbl_Ocr
            // 
            this.lbl_Ocr.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_Ocr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Ocr, "lbl_Ocr");
            this.lbl_Ocr.Name = "lbl_Ocr";
            // 
            // lbl_OcrTitle
            // 
            resources.ApplyResources(this.lbl_OcrTitle, "lbl_OcrTitle");
            this.lbl_OcrTitle.Name = "lbl_OcrTitle";
            // 
            // btn_Bcr
            // 
            this.btn_Bcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Bcr.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Bcr, "btn_Bcr");
            this.btn_Bcr.ForeColor = System.Drawing.Color.Lime;
            this.btn_Bcr.Name = "btn_Bcr";
            this.btn_Bcr.Tag = "0";
            this.btn_Bcr.UseVisualStyleBackColor = false;
            this.btn_Bcr.Click += new System.EventHandler(this.btn_Bcr_Click);
            // 
            // lbl_Bcr
            // 
            this.lbl_Bcr.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_Bcr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Bcr, "lbl_Bcr");
            this.lbl_Bcr.Name = "lbl_Bcr";
            // 
            // lbl_BcrTitle
            // 
            resources.ApplyResources(this.lbl_BcrTitle, "lbl_BcrTitle");
            this.lbl_BcrTitle.Name = "lbl_BcrTitle";
            // 
            // btn_OnlRfid
            // 
            this.btn_OnlRfid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OnlRfid.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OnlRfid, "btn_OnlRfid");
            this.btn_OnlRfid.ForeColor = System.Drawing.Color.Lime;
            this.btn_OnlRfid.Name = "btn_OnlRfid";
            this.btn_OnlRfid.Tag = "0";
            this.btn_OnlRfid.UseVisualStyleBackColor = false;
            this.btn_OnlRfid.Click += new System.EventHandler(this.btn_OnlRfid_Click);
            // 
            // lbl_OnlRfid
            // 
            this.lbl_OnlRfid.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnlRfid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnlRfid, "lbl_OnlRfid");
            this.lbl_OnlRfid.Name = "lbl_OnlRfid";
            // 
            // lbl_OriTitle
            // 
            resources.ApplyResources(this.lbl_OriTitle, "lbl_OriTitle");
            this.lbl_OriTitle.Name = "lbl_OriTitle";
            // 
            // lbl_Ori
            // 
            this.lbl_Ori.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_Ori.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_Ori, "lbl_Ori");
            this.lbl_Ori.Name = "lbl_Ori";
            // 
            // btn_Ori
            // 
            this.btn_Ori.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Ori.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_Ori, "btn_Ori");
            this.btn_Ori.ForeColor = System.Drawing.Color.Lime;
            this.btn_Ori.Name = "btn_Ori";
            this.btn_Ori.Tag = "0";
            this.btn_Ori.UseVisualStyleBackColor = false;
            this.btn_Ori.Click += new System.EventHandler(this.btn_Ori_Click);
            // 
            // btn_VisAuto
            // 
            this.btn_VisAuto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_VisAuto.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_VisAuto, "btn_VisAuto");
            this.btn_VisAuto.ForeColor = System.Drawing.Color.Lime;
            this.btn_VisAuto.Name = "btn_VisAuto";
            this.btn_VisAuto.Tag = "0";
            this.btn_VisAuto.UseVisualStyleBackColor = false;
            this.btn_VisAuto.Click += new System.EventHandler(this.btn_VisAuto_Click);
            // 
            // btn_VisUiAuto
            // 
            this.btn_VisUiAuto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_VisUiAuto.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_VisUiAuto, "btn_VisUiAuto");
            this.btn_VisUiAuto.ForeColor = System.Drawing.Color.Lime;
            this.btn_VisUiAuto.Name = "btn_VisUiAuto";
            this.btn_VisUiAuto.Tag = "0";
            this.btn_VisUiAuto.UseVisualStyleBackColor = false;
            this.btn_VisUiAuto.Click += new System.EventHandler(this.btn_VisUiAuto_Click);
            // 
            // btn_VisUiTrain
            // 
            this.btn_VisUiTrain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_VisUiTrain.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_VisUiTrain, "btn_VisUiTrain");
            this.btn_VisUiTrain.ForeColor = System.Drawing.Color.Lime;
            this.btn_VisUiTrain.Name = "btn_VisUiTrain";
            this.btn_VisUiTrain.Tag = "0";
            this.btn_VisUiTrain.UseVisualStyleBackColor = false;
            this.btn_VisUiTrain.Click += new System.EventHandler(this.btn_VisUiTrain_Click);
            // 
            // lbl_VisProcessTitle
            // 
            resources.ApplyResources(this.lbl_VisProcessTitle, "lbl_VisProcessTitle");
            this.lbl_VisProcessTitle.Name = "lbl_VisProcessTitle";
            // 
            // lbl_VisProcess
            // 
            this.lbl_VisProcess.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_VisProcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_VisProcess, "lbl_VisProcess");
            this.lbl_VisProcess.Name = "lbl_VisProcess";
            // 
            // lbl_VisConnTitle
            // 
            resources.ApplyResources(this.lbl_VisConnTitle, "lbl_VisConnTitle");
            this.lbl_VisConnTitle.Name = "lbl_VisConnTitle";
            // 
            // lbl_VisConn
            // 
            this.lbl_VisConn.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_VisConn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_VisConn, "lbl_VisConn");
            this.lbl_VisConn.Name = "lbl_VisConn";
            // 
            // lbl_VisULevelTitle
            // 
            resources.ApplyResources(this.lbl_VisULevelTitle, "lbl_VisULevelTitle");
            this.lbl_VisULevelTitle.Name = "lbl_VisULevelTitle";
            // 
            // lbl_VisULevel
            // 
            this.lbl_VisULevel.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_VisULevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_VisULevel, "lbl_VisULevel");
            this.lbl_VisULevel.Name = "lbl_VisULevel";
            // 
            // lbl_VisUIModeTitle
            // 
            resources.ApplyResources(this.lbl_VisUIModeTitle, "lbl_VisUIModeTitle");
            this.lbl_VisUIModeTitle.Name = "lbl_VisUIModeTitle";
            // 
            // lbl_VisUIMode
            // 
            this.lbl_VisUIMode.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_VisUIMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_VisUIMode, "lbl_VisUIMode");
            this.lbl_VisUIMode.Name = "lbl_VisUIMode";
            // 
            // lbl_VisStatusTitle
            // 
            resources.ApplyResources(this.lbl_VisStatusTitle, "lbl_VisStatusTitle");
            this.lbl_VisStatusTitle.Name = "lbl_VisStatusTitle";
            // 
            // lbl_VisStatus
            // 
            this.lbl_VisStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_VisStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_VisStatus, "lbl_VisStatus");
            this.lbl_VisStatus.Name = "lbl_VisStatus";
            // 
            // pnl_OnL_Bcr
            // 
            this.pnl_OnL_Bcr.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrTcp_Title);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrTcp);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrBcr_Title);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrBcr);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrErr_Title);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrErr);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrCmdResult_Title);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrCmdResult);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrCmd_Title);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrCmd);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrState_Title);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_BcrState);
            this.pnl_OnL_Bcr.Controls.Add(this.btn_OnL_BCLR);
            this.pnl_OnL_Bcr.Controls.Add(this.btn_OnL_LOFF);
            this.pnl_OnL_Bcr.Controls.Add(this.btn_OnL_LON);
            this.pnl_OnL_Bcr.Controls.Add(this.btn_OnL_BcrConnect);
            this.pnl_OnL_Bcr.Controls.Add(this.lbl_OnL_Bcr_Title);
            resources.ApplyResources(this.pnl_OnL_Bcr, "pnl_OnL_Bcr");
            this.pnl_OnL_Bcr.Name = "pnl_OnL_Bcr";
            this.pnl_OnL_Bcr.Tag = "0";
            // 
            // lbl_OnL_BcrTcp_Title
            // 
            resources.ApplyResources(this.lbl_OnL_BcrTcp_Title, "lbl_OnL_BcrTcp_Title");
            this.lbl_OnL_BcrTcp_Title.Name = "lbl_OnL_BcrTcp_Title";
            // 
            // lbl_OnL_BcrTcp
            // 
            this.lbl_OnL_BcrTcp.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnL_BcrTcp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnL_BcrTcp, "lbl_OnL_BcrTcp");
            this.lbl_OnL_BcrTcp.Name = "lbl_OnL_BcrTcp";
            // 
            // lbl_OnL_BcrBcr_Title
            // 
            resources.ApplyResources(this.lbl_OnL_BcrBcr_Title, "lbl_OnL_BcrBcr_Title");
            this.lbl_OnL_BcrBcr_Title.Name = "lbl_OnL_BcrBcr_Title";
            // 
            // lbl_OnL_BcrBcr
            // 
            this.lbl_OnL_BcrBcr.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnL_BcrBcr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnL_BcrBcr, "lbl_OnL_BcrBcr");
            this.lbl_OnL_BcrBcr.Name = "lbl_OnL_BcrBcr";
            // 
            // lbl_OnL_BcrErr_Title
            // 
            resources.ApplyResources(this.lbl_OnL_BcrErr_Title, "lbl_OnL_BcrErr_Title");
            this.lbl_OnL_BcrErr_Title.Name = "lbl_OnL_BcrErr_Title";
            // 
            // lbl_OnL_BcrErr
            // 
            this.lbl_OnL_BcrErr.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnL_BcrErr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnL_BcrErr, "lbl_OnL_BcrErr");
            this.lbl_OnL_BcrErr.Name = "lbl_OnL_BcrErr";
            // 
            // lbl_OnL_BcrCmdResult_Title
            // 
            resources.ApplyResources(this.lbl_OnL_BcrCmdResult_Title, "lbl_OnL_BcrCmdResult_Title");
            this.lbl_OnL_BcrCmdResult_Title.Name = "lbl_OnL_BcrCmdResult_Title";
            // 
            // lbl_OnL_BcrCmdResult
            // 
            this.lbl_OnL_BcrCmdResult.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnL_BcrCmdResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnL_BcrCmdResult, "lbl_OnL_BcrCmdResult");
            this.lbl_OnL_BcrCmdResult.Name = "lbl_OnL_BcrCmdResult";
            // 
            // lbl_OnL_BcrCmd_Title
            // 
            resources.ApplyResources(this.lbl_OnL_BcrCmd_Title, "lbl_OnL_BcrCmd_Title");
            this.lbl_OnL_BcrCmd_Title.Name = "lbl_OnL_BcrCmd_Title";
            // 
            // lbl_OnL_BcrCmd
            // 
            this.lbl_OnL_BcrCmd.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnL_BcrCmd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnL_BcrCmd, "lbl_OnL_BcrCmd");
            this.lbl_OnL_BcrCmd.Name = "lbl_OnL_BcrCmd";
            // 
            // lbl_OnL_BcrState_Title
            // 
            resources.ApplyResources(this.lbl_OnL_BcrState_Title, "lbl_OnL_BcrState_Title");
            this.lbl_OnL_BcrState_Title.Name = "lbl_OnL_BcrState_Title";
            // 
            // lbl_OnL_BcrState
            // 
            this.lbl_OnL_BcrState.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OnL_BcrState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OnL_BcrState, "lbl_OnL_BcrState");
            this.lbl_OnL_BcrState.Name = "lbl_OnL_BcrState";
            // 
            // btn_OnL_BCLR
            // 
            this.btn_OnL_BCLR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OnL_BCLR.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OnL_BCLR, "btn_OnL_BCLR");
            this.btn_OnL_BCLR.ForeColor = System.Drawing.Color.Lime;
            this.btn_OnL_BCLR.Name = "btn_OnL_BCLR";
            this.btn_OnL_BCLR.Tag = "0";
            this.btn_OnL_BCLR.UseVisualStyleBackColor = false;
            this.btn_OnL_BCLR.Click += new System.EventHandler(this.btn_OnL_BCLR_Click);
            // 
            // btn_OnL_LOFF
            // 
            this.btn_OnL_LOFF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OnL_LOFF.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OnL_LOFF, "btn_OnL_LOFF");
            this.btn_OnL_LOFF.ForeColor = System.Drawing.Color.Lime;
            this.btn_OnL_LOFF.Name = "btn_OnL_LOFF";
            this.btn_OnL_LOFF.Tag = "0";
            this.btn_OnL_LOFF.UseVisualStyleBackColor = false;
            this.btn_OnL_LOFF.Click += new System.EventHandler(this.btn_OnL_LOFF_Click);
            // 
            // btn_OnL_LON
            // 
            this.btn_OnL_LON.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OnL_LON.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OnL_LON, "btn_OnL_LON");
            this.btn_OnL_LON.ForeColor = System.Drawing.Color.Lime;
            this.btn_OnL_LON.Name = "btn_OnL_LON";
            this.btn_OnL_LON.Tag = "0";
            this.btn_OnL_LON.UseVisualStyleBackColor = false;
            this.btn_OnL_LON.Click += new System.EventHandler(this.btn_OnL_LON_Click);
            // 
            // btn_OnL_BcrConnect
            // 
            this.btn_OnL_BcrConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OnL_BcrConnect.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OnL_BcrConnect, "btn_OnL_BcrConnect");
            this.btn_OnL_BcrConnect.ForeColor = System.Drawing.Color.Lime;
            this.btn_OnL_BcrConnect.Name = "btn_OnL_BcrConnect";
            this.btn_OnL_BcrConnect.Tag = "0";
            this.btn_OnL_BcrConnect.UseVisualStyleBackColor = false;
            this.btn_OnL_BcrConnect.Click += new System.EventHandler(this.btn_OnL_BcrConnect_Click);
            // 
            // lbl_OnL_Bcr_Title
            // 
            this.lbl_OnL_Bcr_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.lbl_OnL_Bcr_Title, "lbl_OnL_Bcr_Title");
            this.lbl_OnL_Bcr_Title.ForeColor = System.Drawing.Color.White;
            this.lbl_OnL_Bcr_Title.Name = "lbl_OnL_Bcr_Title";
            // 
            // pnl_OffL_Bcr
            // 
            this.pnl_OffL_Bcr.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrTcp_Title);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrTcp);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrBcr_Title);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrBcr);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrErr_Title);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrErr);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrCmdResult_Title);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrCmdResult);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrCmd_Title);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrCmd);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrState_Title);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_BcrState);
            this.pnl_OffL_Bcr.Controls.Add(this.btn_OffL_BCLR);
            this.pnl_OffL_Bcr.Controls.Add(this.btn_OffL_LOFF);
            this.pnl_OffL_Bcr.Controls.Add(this.btn_OffL_LON);
            this.pnl_OffL_Bcr.Controls.Add(this.btn_OffL_BcrConnect);
            this.pnl_OffL_Bcr.Controls.Add(this.lbl_OffL_Bcr_Title);
            resources.ApplyResources(this.pnl_OffL_Bcr, "pnl_OffL_Bcr");
            this.pnl_OffL_Bcr.Name = "pnl_OffL_Bcr";
            this.pnl_OffL_Bcr.Tag = "0";
            // 
            // lbl_OffL_BcrTcp_Title
            // 
            resources.ApplyResources(this.lbl_OffL_BcrTcp_Title, "lbl_OffL_BcrTcp_Title");
            this.lbl_OffL_BcrTcp_Title.Name = "lbl_OffL_BcrTcp_Title";
            // 
            // lbl_OffL_BcrTcp
            // 
            this.lbl_OffL_BcrTcp.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OffL_BcrTcp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffL_BcrTcp, "lbl_OffL_BcrTcp");
            this.lbl_OffL_BcrTcp.Name = "lbl_OffL_BcrTcp";
            // 
            // lbl_OffL_BcrBcr_Title
            // 
            resources.ApplyResources(this.lbl_OffL_BcrBcr_Title, "lbl_OffL_BcrBcr_Title");
            this.lbl_OffL_BcrBcr_Title.Name = "lbl_OffL_BcrBcr_Title";
            // 
            // lbl_OffL_BcrBcr
            // 
            this.lbl_OffL_BcrBcr.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OffL_BcrBcr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffL_BcrBcr, "lbl_OffL_BcrBcr");
            this.lbl_OffL_BcrBcr.Name = "lbl_OffL_BcrBcr";
            // 
            // lbl_OffL_BcrErr_Title
            // 
            resources.ApplyResources(this.lbl_OffL_BcrErr_Title, "lbl_OffL_BcrErr_Title");
            this.lbl_OffL_BcrErr_Title.Name = "lbl_OffL_BcrErr_Title";
            // 
            // lbl_OffL_BcrErr
            // 
            this.lbl_OffL_BcrErr.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OffL_BcrErr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffL_BcrErr, "lbl_OffL_BcrErr");
            this.lbl_OffL_BcrErr.Name = "lbl_OffL_BcrErr";
            // 
            // lbl_OffL_BcrCmdResult_Title
            // 
            resources.ApplyResources(this.lbl_OffL_BcrCmdResult_Title, "lbl_OffL_BcrCmdResult_Title");
            this.lbl_OffL_BcrCmdResult_Title.Name = "lbl_OffL_BcrCmdResult_Title";
            // 
            // lbl_OffL_BcrCmdResult
            // 
            this.lbl_OffL_BcrCmdResult.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OffL_BcrCmdResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffL_BcrCmdResult, "lbl_OffL_BcrCmdResult");
            this.lbl_OffL_BcrCmdResult.Name = "lbl_OffL_BcrCmdResult";
            // 
            // lbl_OffL_BcrCmd_Title
            // 
            resources.ApplyResources(this.lbl_OffL_BcrCmd_Title, "lbl_OffL_BcrCmd_Title");
            this.lbl_OffL_BcrCmd_Title.Name = "lbl_OffL_BcrCmd_Title";
            // 
            // lbl_OffL_BcrCmd
            // 
            this.lbl_OffL_BcrCmd.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OffL_BcrCmd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffL_BcrCmd, "lbl_OffL_BcrCmd");
            this.lbl_OffL_BcrCmd.Name = "lbl_OffL_BcrCmd";
            // 
            // lbl_OffL_BcrState_Title
            // 
            resources.ApplyResources(this.lbl_OffL_BcrState_Title, "lbl_OffL_BcrState_Title");
            this.lbl_OffL_BcrState_Title.Name = "lbl_OffL_BcrState_Title";
            // 
            // lbl_OffL_BcrState
            // 
            this.lbl_OffL_BcrState.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_OffL_BcrState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.lbl_OffL_BcrState, "lbl_OffL_BcrState");
            this.lbl_OffL_BcrState.Name = "lbl_OffL_BcrState";
            // 
            // btn_OffL_BCLR
            // 
            this.btn_OffL_BCLR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OffL_BCLR.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OffL_BCLR, "btn_OffL_BCLR");
            this.btn_OffL_BCLR.ForeColor = System.Drawing.Color.Lime;
            this.btn_OffL_BCLR.Name = "btn_OffL_BCLR";
            this.btn_OffL_BCLR.Tag = "0";
            this.btn_OffL_BCLR.UseVisualStyleBackColor = false;
            this.btn_OffL_BCLR.Click += new System.EventHandler(this.btn_OffL_BCLR_Click);
            // 
            // btn_OffL_LOFF
            // 
            this.btn_OffL_LOFF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OffL_LOFF.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OffL_LOFF, "btn_OffL_LOFF");
            this.btn_OffL_LOFF.ForeColor = System.Drawing.Color.Lime;
            this.btn_OffL_LOFF.Name = "btn_OffL_LOFF";
            this.btn_OffL_LOFF.Tag = "0";
            this.btn_OffL_LOFF.UseVisualStyleBackColor = false;
            this.btn_OffL_LOFF.Click += new System.EventHandler(this.btn_OffL_LOFF_Click);
            // 
            // btn_OffL_LON
            // 
            this.btn_OffL_LON.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OffL_LON.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OffL_LON, "btn_OffL_LON");
            this.btn_OffL_LON.ForeColor = System.Drawing.Color.Lime;
            this.btn_OffL_LON.Name = "btn_OffL_LON";
            this.btn_OffL_LON.Tag = "0";
            this.btn_OffL_LON.UseVisualStyleBackColor = false;
            this.btn_OffL_LON.Click += new System.EventHandler(this.btn_OffL_LON_Click);
            // 
            // btn_OffL_BcrConnect
            // 
            this.btn_OffL_BcrConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OffL_BcrConnect.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OffL_BcrConnect, "btn_OffL_BcrConnect");
            this.btn_OffL_BcrConnect.ForeColor = System.Drawing.Color.Lime;
            this.btn_OffL_BcrConnect.Name = "btn_OffL_BcrConnect";
            this.btn_OffL_BcrConnect.Tag = "0";
            this.btn_OffL_BcrConnect.UseVisualStyleBackColor = false;
            this.btn_OffL_BcrConnect.Click += new System.EventHandler(this.btn_OffL_BcrConnect_Click);
            // 
            // lbl_OffL_Bcr_Title
            // 
            this.lbl_OffL_Bcr_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            resources.ApplyResources(this.lbl_OffL_Bcr_Title, "lbl_OffL_Bcr_Title");
            this.lbl_OffL_Bcr_Title.ForeColor = System.Drawing.Color.White;
            this.lbl_OffL_Bcr_Title.Name = "lbl_OffL_Bcr_Title";
            // 
            // btn_BCROnly
            // 
            this.btn_BCROnly.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_BCROnly.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_BCROnly, "btn_BCROnly");
            this.btn_BCROnly.ForeColor = System.Drawing.Color.Lime;
            this.btn_BCROnly.Name = "btn_BCROnly";
            this.btn_BCROnly.Tag = "0";
            this.btn_BCROnly.UseVisualStyleBackColor = false;
            this.btn_BCROnly.Click += new System.EventHandler(this.btn_BCROnly_Click);
            // 
            // btn_OCROnly
            // 
            this.btn_OCROnly.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_OCROnly.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_OCROnly, "btn_OCROnly");
            this.btn_OCROnly.ForeColor = System.Drawing.Color.Lime;
            this.btn_OCROnly.Name = "btn_OCROnly";
            this.btn_OCROnly.Tag = "0";
            this.btn_OCROnly.UseVisualStyleBackColor = false;
            this.btn_OCROnly.Click += new System.EventHandler(this.btn_OCROnly_Click);
            // 
            // btn_BCROCR
            // 
            this.btn_BCROCR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_BCROCR.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.btn_BCROCR, "btn_BCROCR");
            this.btn_BCROCR.ForeColor = System.Drawing.Color.Lime;
            this.btn_BCROCR.Name = "btn_BCROCR";
            this.btn_BCROCR.Tag = "0";
            this.btn_BCROCR.UseVisualStyleBackColor = false;
            this.btn_BCROCR.Click += new System.EventHandler(this.btn_BCROCR_Click);
            // 
            // vwEqu_08Bcr
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btn_BCROCR);
            this.Controls.Add(this.btn_OCROnly);
            this.Controls.Add(this.btn_BCROnly);
            this.Controls.Add(this.pnl_OffL_Bcr);
            this.Controls.Add(this.pnl_OnL_Bcr);
            this.Controls.Add(this.lbl_VisStatusTitle);
            this.Controls.Add(this.lbl_VisStatus);
            this.Controls.Add(this.lbl_VisUIModeTitle);
            this.Controls.Add(this.lbl_VisUIMode);
            this.Controls.Add(this.lbl_VisULevelTitle);
            this.Controls.Add(this.lbl_VisULevel);
            this.Controls.Add(this.lbl_VisConnTitle);
            this.Controls.Add(this.lbl_VisConn);
            this.Controls.Add(this.lbl_VisProcessTitle);
            this.Controls.Add(this.lbl_VisProcess);
            this.Controls.Add(this.btn_VisUiTrain);
            this.Controls.Add(this.btn_VisUiAuto);
            this.Controls.Add(this.btn_VisAuto);
            this.Controls.Add(this.lbl_OriTitle);
            this.Controls.Add(this.lbl_Ori);
            this.Controls.Add(this.btn_Ori);
            this.Controls.Add(this.lbl_OnlRfid);
            this.Controls.Add(this.btn_OnlRfid);
            this.Controls.Add(this.lbl_Rfid);
            this.Controls.Add(this.btn_BtmRfid);
            this.Controls.Add(this.btn_TopRfid);
            this.Controls.Add(this.lbl_Ocr);
            this.Controls.Add(this.lbl_OcrTitle);
            this.Controls.Add(this.btn_Bcr);
            this.Controls.Add(this.lbl_Bcr);
            this.Controls.Add(this.lbl_BcrTitle);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_08Bcr";
            this.pnl_OnL_Bcr.ResumeLayout(false);
            this.pnl_OffL_Bcr.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_Rfid;
        private System.Windows.Forms.Button btn_BtmRfid;
        private System.Windows.Forms.Button btn_TopRfid;
        private System.Windows.Forms.Label lbl_Ocr;
        private System.Windows.Forms.Label lbl_OcrTitle;
        private System.Windows.Forms.Button btn_Bcr;
        private System.Windows.Forms.Label lbl_Bcr;
        private System.Windows.Forms.Label lbl_BcrTitle;
        private System.Windows.Forms.Button btn_OnlRfid;
        private System.Windows.Forms.Label lbl_OnlRfid;
        private System.Windows.Forms.Label lbl_OriTitle;
        private System.Windows.Forms.Label lbl_Ori;
        private System.Windows.Forms.Button btn_Ori;
        private System.Windows.Forms.Button btn_VisAuto;
        private System.Windows.Forms.Button btn_VisUiAuto;
        private System.Windows.Forms.Button btn_VisUiTrain;
        private System.Windows.Forms.Label lbl_VisProcessTitle;
        private System.Windows.Forms.Label lbl_VisProcess;
        private System.Windows.Forms.Label lbl_VisConnTitle;
        private System.Windows.Forms.Label lbl_VisConn;
        private System.Windows.Forms.Label lbl_VisULevelTitle;
        private System.Windows.Forms.Label lbl_VisULevel;
        private System.Windows.Forms.Label lbl_VisUIModeTitle;
        private System.Windows.Forms.Label lbl_VisUIMode;
        private System.Windows.Forms.Label lbl_VisStatusTitle;
        private System.Windows.Forms.Label lbl_VisStatus;
        private System.Windows.Forms.Panel pnl_OnL_Bcr;
        private System.Windows.Forms.Label lbl_OnL_BcrTcp_Title;
        private System.Windows.Forms.Label lbl_OnL_BcrTcp;
        private System.Windows.Forms.Label lbl_OnL_BcrBcr_Title;
        private System.Windows.Forms.Label lbl_OnL_BcrBcr;
        private System.Windows.Forms.Label lbl_OnL_BcrErr_Title;
        private System.Windows.Forms.Label lbl_OnL_BcrErr;
        private System.Windows.Forms.Label lbl_OnL_BcrCmdResult_Title;
        private System.Windows.Forms.Label lbl_OnL_BcrCmdResult;
        private System.Windows.Forms.Label lbl_OnL_BcrCmd_Title;
        private System.Windows.Forms.Label lbl_OnL_BcrCmd;
        private System.Windows.Forms.Label lbl_OnL_BcrState_Title;
        private System.Windows.Forms.Label lbl_OnL_BcrState;
        private System.Windows.Forms.Button btn_OnL_BCLR;
        private System.Windows.Forms.Button btn_OnL_LOFF;
        private System.Windows.Forms.Button btn_OnL_LON;
        private System.Windows.Forms.Button btn_OnL_BcrConnect;
        private System.Windows.Forms.Label lbl_OnL_Bcr_Title;
        private System.Windows.Forms.Panel pnl_OffL_Bcr;
        private System.Windows.Forms.Label lbl_OffL_BcrTcp_Title;
        private System.Windows.Forms.Label lbl_OffL_BcrTcp;
        private System.Windows.Forms.Label lbl_OffL_BcrBcr_Title;
        private System.Windows.Forms.Label lbl_OffL_BcrBcr;
        private System.Windows.Forms.Label lbl_OffL_BcrErr_Title;
        private System.Windows.Forms.Label lbl_OffL_BcrErr;
        private System.Windows.Forms.Label lbl_OffL_BcrCmdResult_Title;
        private System.Windows.Forms.Label lbl_OffL_BcrCmdResult;
        private System.Windows.Forms.Label lbl_OffL_BcrCmd_Title;
        private System.Windows.Forms.Label lbl_OffL_BcrCmd;
        private System.Windows.Forms.Label lbl_OffL_BcrState_Title;
        private System.Windows.Forms.Label lbl_OffL_BcrState;
        private System.Windows.Forms.Button btn_OffL_BCLR;
        private System.Windows.Forms.Button btn_OffL_LOFF;
        private System.Windows.Forms.Button btn_OffL_LON;
        private System.Windows.Forms.Button btn_OffL_BcrConnect;
        private System.Windows.Forms.Label lbl_OffL_Bcr_Title;
        private System.Windows.Forms.Button btn_BCROnly;
        private System.Windows.Forms.Button btn_OCROnly;
        private System.Windows.Forms.Button btn_BCROCR;
    }
}
