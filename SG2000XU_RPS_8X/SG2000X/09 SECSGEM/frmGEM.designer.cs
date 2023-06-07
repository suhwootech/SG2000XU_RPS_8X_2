using Extended;

namespace SG2000X
{
    partial class frmGEM
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGEM));
			this.tmrGEM = new System.Windows.Forms.Timer(this.components);
			this.tmrSet = new System.Windows.Forms.Timer(this.components);
			this.pnlGEM_Base = new System.Windows.Forms.Panel();
			this.grpStrip = new System.Windows.Forms.GroupBox();
			this.btnStrip_LOTEnded = new System.Windows.Forms.Button();
			this.lblStrip_Proceed = new System.Windows.Forms.Label();
			this.txtStrip_Proceed = new System.Windows.Forms.TextBox();
			this.lblStrip_TotalUnit = new System.Windows.Forms.Label();
			this.txtStrip_TotalUnit = new System.Windows.Forms.TextBox();
			this.btnStrip_CarrierEnd1 = new System.Windows.Forms.Button();
			this.txtStrip_EndStrip = new System.Windows.Forms.TextBox();
			this.btnStrip_EndM = new System.Windows.Forms.Button();
			this.txtStrip_MiddleStrip = new System.Windows.Forms.TextBox();
			this.btnStrip_MiddleM = new System.Windows.Forms.Button();
			this.txtStrip_BeforeStrip = new System.Windows.Forms.TextBox();
			this.btnStrip_BeforeM = new System.Windows.Forms.Button();
			this.lblStrip_SlotNo = new System.Windows.Forms.Label();
			this.txtStrip_SlotNo = new System.Windows.Forms.TextBox();
			this.lblStrip_StripID = new System.Windows.Forms.Label();
			this.txtStrip_StripID = new System.Windows.Forms.TextBox();
			this.lblStrip_CarrierID = new System.Windows.Forms.Label();
			this.txtStrip_CarrierID = new System.Windows.Forms.TextBox();
			this.btnStrip_CarrierCreate = new System.Windows.Forms.Button();
			this.btnStrip_LOTVerified = new System.Windows.Forms.Button();
			this.btnStrip_CarrierStarted = new System.Windows.Forms.Button();
			this.btnStrip_ClearCarrierList = new System.Windows.Forms.Button();
			this.btnStrip_GetStripID = new System.Windows.Forms.Button();
			this.lblStrip_GoodNg = new System.Windows.Forms.Label();
			this.txtStrip_GoodNg = new System.Windows.Forms.TextBox();
			this.lblStrip_LOTID = new System.Windows.Forms.Label();
			this.btnStrip_CarrierVerifyRequest = new System.Windows.Forms.Button();
			this.txtStrip_LOTID = new System.Windows.Forms.TextBox();
			this.btnStrip_CarrierEnd2 = new System.Windows.Forms.Button();
			this.grpDevice = new System.Windows.Forms.GroupBox();
			this.txtDevice = new System.Windows.Forms.TextBox();
			this.btnDevice_Search = new System.Windows.Forms.Button();
			this.btnDevice_Add = new System.Windows.Forms.Button();
			this.btnDevice_Change = new System.Windows.Forms.Button();
			this.btnDevice_Delete = new System.Windows.Forms.Button();
			this.btnDevice_Edit = new System.Windows.Forms.Button();
			this.cboDevice = new System.Windows.Forms.ComboBox();
			this.grpAlarm = new System.Windows.Forms.GroupBox();
			this.btnAlarm_Reset = new System.Windows.Forms.Button();
			this.btnAlarm_Set = new System.Windows.Forms.Button();
			this.cboALID = new System.Windows.Forms.ComboBox();
			this.grpES = new System.Windows.Forms.GroupBox();
			this.radES_Down = new System.Windows.Forms.RadioButton();
			this.radES_Run = new System.Windows.Forms.RadioButton();
			this.radES_Ready = new System.Windows.Forms.RadioButton();
			this.radES_Idle = new System.Windows.Forms.RadioButton();
			this.grpVerification = new System.Windows.Forms.GroupBox();
			this.lblV_MGZID = new System.Windows.Forms.Label();
			this.txtV_MGZID = new System.Windows.Forms.TextBox();
			this.lblV_MGZPort = new System.Windows.Forms.Label();
			this.btnV_MGZVerifyRequest = new System.Windows.Forms.Button();
			this.txtV_MGZPort = new System.Windows.Forms.TextBox();
			this.lblV_MatID = new System.Windows.Forms.Label();
			this.txtV_MatID = new System.Windows.Forms.TextBox();
			this.lblV_MatLoc = new System.Windows.Forms.Label();
			this.btnV_MatVerifyRequest = new System.Windows.Forms.Button();
			this.txtV_MatLoc = new System.Windows.Forms.TextBox();
			this.lblV_ToolID = new System.Windows.Forms.Label();
			this.txtV_ToolID = new System.Windows.Forms.TextBox();
			this.lblV_ToolLoc = new System.Windows.Forms.Label();
			this.btnV_ToolVerifyRequest = new System.Windows.Forms.Button();
			this.txtV_ToolLoc = new System.Windows.Forms.TextBox();
			this.btnV_MGZEnded = new System.Windows.Forms.Button();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.pnlHSMS_Base = new System.Windows.Forms.Panel();
			this.lblComm_Status = new System.Windows.Forms.Label();
			this.listLog = new System.Windows.Forms.ListBox();
			this.btnMini = new System.Windows.Forms.Button();
			this.lblCont_Status = new System.Windows.Forms.Label();
			this.lblConn_Status = new System.Windows.Forms.Label();
			this.btnConn_Stop = new System.Windows.Forms.Button();
			this.btnCont_Local = new System.Windows.Forms.Button();
			this.btnCont_Remote = new System.Windows.Forms.Button();
			this.btnCont_Offline = new System.Windows.Forms.Button();
			this.btnConn_Start = new System.Windows.Forms.Button();
			this.grpHSMS = new System.Windows.Forms.GroupBox();
			this.lblHSMS_CRT = new System.Windows.Forms.Label();
			this.btnHSMS_Save = new System.Windows.Forms.Button();
			this.lblHSMS_T8 = new System.Windows.Forms.Label();
			this.lblHSMS_T7 = new System.Windows.Forms.Label();
			this.lblHSMS_T6 = new System.Windows.Forms.Label();
			this.lblHSMS_T5 = new System.Windows.Forms.Label();
			this.lblHSMS_T3 = new System.Windows.Forms.Label();
			this.lblHSMS_LT = new System.Windows.Forms.Label();
			this.lblHSMS_DID = new System.Windows.Forms.Label();
			this.lblHSMS_Port_Title = new System.Windows.Forms.Label();
			this.txtHSMS_T81 = new System.Windows.Forms.TextBox();
			this.txtHSMS_T71 = new System.Windows.Forms.TextBox();
			this.txtHSMS_CRT1 = new System.Windows.Forms.TextBox();
			this.txtHSMS_T51 = new System.Windows.Forms.TextBox();
			this.txtHSMS_DID1 = new System.Windows.Forms.TextBox();
			this.txtHSMS_T61 = new System.Windows.Forms.TextBox();
			this.txtHSMS_LT1 = new System.Windows.Forms.TextBox();
			this.txtHSMS_T31 = new System.Windows.Forms.TextBox();
			this.txtHSMS_Port1 = new System.Windows.Forms.TextBox();
			this.pnlGEM_Base.SuspendLayout();
			this.grpStrip.SuspendLayout();
			this.grpDevice.SuspendLayout();
			this.grpAlarm.SuspendLayout();
			this.grpES.SuspendLayout();
			this.grpVerification.SuspendLayout();
			this.pnlHSMS_Base.SuspendLayout();
			this.grpHSMS.SuspendLayout();
			this.SuspendLayout();
			// 
			// tmrGEM
			// 
			this.tmrGEM.Interval = 5000;
			this.tmrGEM.Tick += new System.EventHandler(this.tmrGEM_Tick);
			// 
			// tmrSet
			// 
			this.tmrSet.Interval = 1000;
			this.tmrSet.Tick += new System.EventHandler(this.tmrSet_Tick);
			// 
			// pnlGEM_Base
			// 
			this.pnlGEM_Base.BackColor = System.Drawing.Color.Wheat;
			this.pnlGEM_Base.Controls.Add(this.grpStrip);
			this.pnlGEM_Base.Controls.Add(this.grpDevice);
			this.pnlGEM_Base.Controls.Add(this.grpAlarm);
			this.pnlGEM_Base.Controls.Add(this.grpES);
			this.pnlGEM_Base.Location = new System.Drawing.Point(570, 10);
			this.pnlGEM_Base.Name = "pnlGEM_Base";
			this.pnlGEM_Base.Size = new System.Drawing.Size(510, 573);
			this.pnlGEM_Base.TabIndex = 2;
			// 
			// grpStrip
			// 
			this.grpStrip.Controls.Add(this.btnStrip_LOTEnded);
			this.grpStrip.Controls.Add(this.lblStrip_Proceed);
			this.grpStrip.Controls.Add(this.txtStrip_Proceed);
			this.grpStrip.Controls.Add(this.lblStrip_TotalUnit);
			this.grpStrip.Controls.Add(this.txtStrip_TotalUnit);
			this.grpStrip.Controls.Add(this.btnStrip_CarrierEnd1);
			this.grpStrip.Controls.Add(this.txtStrip_EndStrip);
			this.grpStrip.Controls.Add(this.btnStrip_EndM);
			this.grpStrip.Controls.Add(this.txtStrip_MiddleStrip);
			this.grpStrip.Controls.Add(this.btnStrip_MiddleM);
			this.grpStrip.Controls.Add(this.txtStrip_BeforeStrip);
			this.grpStrip.Controls.Add(this.btnStrip_BeforeM);
			this.grpStrip.Controls.Add(this.lblStrip_SlotNo);
			this.grpStrip.Controls.Add(this.txtStrip_SlotNo);
			this.grpStrip.Controls.Add(this.lblStrip_StripID);
			this.grpStrip.Controls.Add(this.txtStrip_StripID);
			this.grpStrip.Controls.Add(this.lblStrip_CarrierID);
			this.grpStrip.Controls.Add(this.txtStrip_CarrierID);
			this.grpStrip.Controls.Add(this.btnStrip_CarrierCreate);
			this.grpStrip.Controls.Add(this.btnStrip_LOTVerified);
			this.grpStrip.Controls.Add(this.btnStrip_CarrierStarted);
			this.grpStrip.Controls.Add(this.btnStrip_ClearCarrierList);
			this.grpStrip.Controls.Add(this.btnStrip_GetStripID);
			this.grpStrip.Controls.Add(this.lblStrip_GoodNg);
			this.grpStrip.Controls.Add(this.txtStrip_GoodNg);
			this.grpStrip.Controls.Add(this.lblStrip_LOTID);
			this.grpStrip.Controls.Add(this.btnStrip_CarrierVerifyRequest);
			this.grpStrip.Controls.Add(this.txtStrip_LOTID);
			this.grpStrip.Controls.Add(this.btnStrip_CarrierEnd2);
			this.grpStrip.Location = new System.Drawing.Point(10, 250);
			this.grpStrip.Name = "grpStrip";
			this.grpStrip.Size = new System.Drawing.Size(490, 330);
			this.grpStrip.TabIndex = 7;
			this.grpStrip.TabStop = false;
			this.grpStrip.Text = "Strip";
			// 
			// btnStrip_LOTEnded
			// 
			this.btnStrip_LOTEnded.Location = new System.Drawing.Point(370, 290);
			this.btnStrip_LOTEnded.Name = "btnStrip_LOTEnded";
			this.btnStrip_LOTEnded.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_LOTEnded.TabIndex = 21;
			this.btnStrip_LOTEnded.Text = "LOT Ended";
			this.btnStrip_LOTEnded.UseVisualStyleBackColor = true;
			this.btnStrip_LOTEnded.Click += new System.EventHandler(this.btnStrip_LOTEnded_Click);
			// 
			// lblStrip_Proceed
			// 
			this.lblStrip_Proceed.Location = new System.Drawing.Point(250, 260);
			this.lblStrip_Proceed.Name = "lblStrip_Proceed";
			this.lblStrip_Proceed.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_Proceed.TabIndex = 0;
			this.lblStrip_Proceed.Text = "Proceed";
			this.lblStrip_Proceed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtStrip_Proceed
			// 
			this.txtStrip_Proceed.Location = new System.Drawing.Point(370, 260);
			this.txtStrip_Proceed.Name = "txtStrip_Proceed";
			this.txtStrip_Proceed.Size = new System.Drawing.Size(100, 21);
			this.txtStrip_Proceed.TabIndex = 9;
			// 
			// lblStrip_TotalUnit
			// 
			this.lblStrip_TotalUnit.Location = new System.Drawing.Point(10, 260);
			this.lblStrip_TotalUnit.Name = "lblStrip_TotalUnit";
			this.lblStrip_TotalUnit.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_TotalUnit.TabIndex = 0;
			this.lblStrip_TotalUnit.Text = "Total Unit";
			this.lblStrip_TotalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtStrip_TotalUnit
			// 
			this.txtStrip_TotalUnit.Location = new System.Drawing.Point(130, 260);
			this.txtStrip_TotalUnit.Name = "txtStrip_TotalUnit";
			this.txtStrip_TotalUnit.Size = new System.Drawing.Size(100, 21);
			this.txtStrip_TotalUnit.TabIndex = 8;
			// 
			// btnStrip_CarrierEnd1
			// 
			this.btnStrip_CarrierEnd1.Location = new System.Drawing.Point(250, 230);
			this.btnStrip_CarrierEnd1.Name = "btnStrip_CarrierEnd1";
			this.btnStrip_CarrierEnd1.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_CarrierEnd1.TabIndex = 19;
			this.btnStrip_CarrierEnd1.Text = "Carrier End 1";
			this.btnStrip_CarrierEnd1.UseVisualStyleBackColor = true;
			this.btnStrip_CarrierEnd1.Click += new System.EventHandler(this.btnStrip_CarrierEnd1_Click);
			// 
			// txtStrip_EndStrip
			// 
			this.txtStrip_EndStrip.Location = new System.Drawing.Point(10, 200);
			this.txtStrip_EndStrip.Name = "txtStrip_EndStrip";
			this.txtStrip_EndStrip.Size = new System.Drawing.Size(340, 21);
			this.txtStrip_EndStrip.TabIndex = 6;
			// 
			// btnStrip_EndM
			// 
			this.btnStrip_EndM.Location = new System.Drawing.Point(370, 200);
			this.btnStrip_EndM.Name = "btnStrip_EndM";
			this.btnStrip_EndM.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_EndM.TabIndex = 18;
			this.btnStrip_EndM.Text = "End";
			this.btnStrip_EndM.UseVisualStyleBackColor = true;
			this.btnStrip_EndM.Click += new System.EventHandler(this.btnStrip_EndM_Click);
			// 
			// txtStrip_MiddleStrip
			// 
			this.txtStrip_MiddleStrip.Location = new System.Drawing.Point(10, 170);
			this.txtStrip_MiddleStrip.Name = "txtStrip_MiddleStrip";
			this.txtStrip_MiddleStrip.Size = new System.Drawing.Size(340, 21);
			this.txtStrip_MiddleStrip.TabIndex = 5;
			// 
			// btnStrip_MiddleM
			// 
			this.btnStrip_MiddleM.Location = new System.Drawing.Point(370, 170);
			this.btnStrip_MiddleM.Name = "btnStrip_MiddleM";
			this.btnStrip_MiddleM.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_MiddleM.TabIndex = 17;
			this.btnStrip_MiddleM.Text = "Middle";
			this.btnStrip_MiddleM.UseVisualStyleBackColor = true;
			this.btnStrip_MiddleM.Click += new System.EventHandler(this.btnStrip_MiddleM_Click);
			// 
			// txtStrip_BeforeStrip
			// 
			this.txtStrip_BeforeStrip.Location = new System.Drawing.Point(10, 140);
			this.txtStrip_BeforeStrip.Name = "txtStrip_BeforeStrip";
			this.txtStrip_BeforeStrip.Size = new System.Drawing.Size(340, 21);
			this.txtStrip_BeforeStrip.TabIndex = 4;
			// 
			// btnStrip_BeforeM
			// 
			this.btnStrip_BeforeM.Location = new System.Drawing.Point(370, 140);
			this.btnStrip_BeforeM.Name = "btnStrip_BeforeM";
			this.btnStrip_BeforeM.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_BeforeM.TabIndex = 16;
			this.btnStrip_BeforeM.Text = "Before";
			this.btnStrip_BeforeM.UseVisualStyleBackColor = true;
			this.btnStrip_BeforeM.Click += new System.EventHandler(this.btnStrip_BeforeM_Click);
			// 
			// lblStrip_SlotNo
			// 
			this.lblStrip_SlotNo.Location = new System.Drawing.Point(10, 80);
			this.lblStrip_SlotNo.Name = "lblStrip_SlotNo";
			this.lblStrip_SlotNo.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_SlotNo.TabIndex = 0;
			this.lblStrip_SlotNo.Text = "Slot No";
			this.lblStrip_SlotNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtStrip_SlotNo
			// 
			this.txtStrip_SlotNo.Location = new System.Drawing.Point(130, 80);
			this.txtStrip_SlotNo.Name = "txtStrip_SlotNo";
			this.txtStrip_SlotNo.Size = new System.Drawing.Size(100, 21);
			this.txtStrip_SlotNo.TabIndex = 3;
			// 
			// lblStrip_StripID
			// 
			this.lblStrip_StripID.Location = new System.Drawing.Point(10, 290);
			this.lblStrip_StripID.Name = "lblStrip_StripID";
			this.lblStrip_StripID.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_StripID.TabIndex = 0;
			this.lblStrip_StripID.Text = "Strip ID";
			this.lblStrip_StripID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtStrip_StripID
			// 
			this.txtStrip_StripID.Location = new System.Drawing.Point(130, 292);
			this.txtStrip_StripID.Name = "txtStrip_StripID";
			this.txtStrip_StripID.Size = new System.Drawing.Size(220, 21);
			this.txtStrip_StripID.TabIndex = 0;
			// 
			// lblStrip_CarrierID
			// 
			this.lblStrip_CarrierID.Location = new System.Drawing.Point(10, 20);
			this.lblStrip_CarrierID.Name = "lblStrip_CarrierID";
			this.lblStrip_CarrierID.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_CarrierID.TabIndex = 0;
			this.lblStrip_CarrierID.Text = "Carrier ID";
			this.lblStrip_CarrierID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtStrip_CarrierID
			// 
			this.txtStrip_CarrierID.Location = new System.Drawing.Point(130, 20);
			this.txtStrip_CarrierID.Name = "txtStrip_CarrierID";
			this.txtStrip_CarrierID.Size = new System.Drawing.Size(220, 21);
			this.txtStrip_CarrierID.TabIndex = 1;
			// 
			// btnStrip_CarrierCreate
			// 
			this.btnStrip_CarrierCreate.Location = new System.Drawing.Point(10, 110);
			this.btnStrip_CarrierCreate.Name = "btnStrip_CarrierCreate";
			this.btnStrip_CarrierCreate.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_CarrierCreate.TabIndex = 13;
			this.btnStrip_CarrierCreate.Text = "Carrier Create";
			this.btnStrip_CarrierCreate.UseVisualStyleBackColor = true;
			this.btnStrip_CarrierCreate.Click += new System.EventHandler(this.btnStrip_CarrierCreate_Click);
			// 
			// btnStrip_LOTVerified
			// 
			this.btnStrip_LOTVerified.Location = new System.Drawing.Point(250, 110);
			this.btnStrip_LOTVerified.Name = "btnStrip_LOTVerified";
			this.btnStrip_LOTVerified.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_LOTVerified.TabIndex = 15;
			this.btnStrip_LOTVerified.Text = "LOT Verified";
			this.btnStrip_LOTVerified.UseVisualStyleBackColor = true;
			this.btnStrip_LOTVerified.Click += new System.EventHandler(this.btnStrip_LOTVerified_Click);
			// 
			// btnStrip_CarrierStarted
			// 
			this.btnStrip_CarrierStarted.Location = new System.Drawing.Point(130, 110);
			this.btnStrip_CarrierStarted.Name = "btnStrip_CarrierStarted";
			this.btnStrip_CarrierStarted.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_CarrierStarted.TabIndex = 14;
			this.btnStrip_CarrierStarted.Text = "Carrier Started";
			this.btnStrip_CarrierStarted.UseVisualStyleBackColor = true;
			this.btnStrip_CarrierStarted.Click += new System.EventHandler(this.btnStrip_CarrierStarted_Click);
			// 
			// btnStrip_ClearCarrierList
			// 
			this.btnStrip_ClearCarrierList.Location = new System.Drawing.Point(370, 80);
			this.btnStrip_ClearCarrierList.Name = "btnStrip_ClearCarrierList";
			this.btnStrip_ClearCarrierList.Size = new System.Drawing.Size(100, 54);
			this.btnStrip_ClearCarrierList.TabIndex = 11;
			this.btnStrip_ClearCarrierList.Text = "Clear Carrier List";
			this.btnStrip_ClearCarrierList.UseVisualStyleBackColor = true;
			this.btnStrip_ClearCarrierList.Click += new System.EventHandler(this.btnStrip_ClearCarrierList_Click);
			// 
			// btnStrip_GetStripID
			// 
			this.btnStrip_GetStripID.Location = new System.Drawing.Point(250, 80);
			this.btnStrip_GetStripID.Name = "btnStrip_GetStripID";
			this.btnStrip_GetStripID.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_GetStripID.TabIndex = 12;
			this.btnStrip_GetStripID.Text = "Get Strip ID";
			this.btnStrip_GetStripID.UseVisualStyleBackColor = true;
			this.btnStrip_GetStripID.Click += new System.EventHandler(this.btnStrip_GetStripID_Click);
			// 
			// lblStrip_GoodNg
			// 
			this.lblStrip_GoodNg.Location = new System.Drawing.Point(10, 230);
			this.lblStrip_GoodNg.Name = "lblStrip_GoodNg";
			this.lblStrip_GoodNg.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_GoodNg.TabIndex = 0;
			this.lblStrip_GoodNg.Text = "Good / NG";
			this.lblStrip_GoodNg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtStrip_GoodNg
			// 
			this.txtStrip_GoodNg.Location = new System.Drawing.Point(130, 230);
			this.txtStrip_GoodNg.Name = "txtStrip_GoodNg";
			this.txtStrip_GoodNg.Size = new System.Drawing.Size(100, 21);
			this.txtStrip_GoodNg.TabIndex = 7;
			// 
			// lblStrip_LOTID
			// 
			this.lblStrip_LOTID.Location = new System.Drawing.Point(10, 50);
			this.lblStrip_LOTID.Name = "lblStrip_LOTID";
			this.lblStrip_LOTID.Size = new System.Drawing.Size(100, 23);
			this.lblStrip_LOTID.TabIndex = 0;
			this.lblStrip_LOTID.Text = "LOT ID";
			this.lblStrip_LOTID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnStrip_CarrierVerifyRequest
			// 
			this.btnStrip_CarrierVerifyRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnStrip_CarrierVerifyRequest.Location = new System.Drawing.Point(370, 20);
			this.btnStrip_CarrierVerifyRequest.Name = "btnStrip_CarrierVerifyRequest";
			this.btnStrip_CarrierVerifyRequest.Size = new System.Drawing.Size(100, 54);
			this.btnStrip_CarrierVerifyRequest.TabIndex = 10;
			this.btnStrip_CarrierVerifyRequest.Text = "Carrier Verify Request";
			this.btnStrip_CarrierVerifyRequest.UseVisualStyleBackColor = true;
			this.btnStrip_CarrierVerifyRequest.Click += new System.EventHandler(this.btnStrip_CarrierVerifyRequest_Click);
			// 
			// txtStrip_LOTID
			// 
			this.txtStrip_LOTID.Location = new System.Drawing.Point(130, 50);
			this.txtStrip_LOTID.Name = "txtStrip_LOTID";
			this.txtStrip_LOTID.Size = new System.Drawing.Size(220, 21);
			this.txtStrip_LOTID.TabIndex = 2;
			// 
			// btnStrip_CarrierEnd2
			// 
			this.btnStrip_CarrierEnd2.Location = new System.Drawing.Point(370, 230);
			this.btnStrip_CarrierEnd2.Name = "btnStrip_CarrierEnd2";
			this.btnStrip_CarrierEnd2.Size = new System.Drawing.Size(100, 23);
			this.btnStrip_CarrierEnd2.TabIndex = 20;
			this.btnStrip_CarrierEnd2.Text = "Carrier End 2";
			this.btnStrip_CarrierEnd2.UseVisualStyleBackColor = true;
			this.btnStrip_CarrierEnd2.Click += new System.EventHandler(this.btnStrip_CarrierEnd2_Click);
			// 
			// grpDevice
			// 
			this.grpDevice.Controls.Add(this.txtDevice);
			this.grpDevice.Controls.Add(this.btnDevice_Search);
			this.grpDevice.Controls.Add(this.btnDevice_Add);
			this.grpDevice.Controls.Add(this.btnDevice_Change);
			this.grpDevice.Controls.Add(this.btnDevice_Delete);
			this.grpDevice.Controls.Add(this.btnDevice_Edit);
			this.grpDevice.Controls.Add(this.cboDevice);
			this.grpDevice.Location = new System.Drawing.Point(10, 130);
			this.grpDevice.Name = "grpDevice";
			this.grpDevice.Size = new System.Drawing.Size(490, 110);
			this.grpDevice.TabIndex = 3;
			this.grpDevice.TabStop = false;
			this.grpDevice.Text = "Device";
			// 
			// txtDevice
			// 
			this.txtDevice.Location = new System.Drawing.Point(10, 50);
			this.txtDevice.Name = "txtDevice";
			this.txtDevice.Size = new System.Drawing.Size(340, 21);
			this.txtDevice.TabIndex = 2;
			this.txtDevice.Visible = false;
			// 
			// btnDevice_Search
			// 
			this.btnDevice_Search.Location = new System.Drawing.Point(370, 20);
			this.btnDevice_Search.Name = "btnDevice_Search";
			this.btnDevice_Search.Size = new System.Drawing.Size(100, 23);
			this.btnDevice_Search.TabIndex = 1;
			this.btnDevice_Search.Text = "Device Search";
			this.btnDevice_Search.UseVisualStyleBackColor = true;
			this.btnDevice_Search.Click += new System.EventHandler(this.btnDevice_Search_Click);
			// 
			// btnDevice_Add
			// 
			this.btnDevice_Add.Location = new System.Drawing.Point(10, 80);
			this.btnDevice_Add.Name = "btnDevice_Add";
			this.btnDevice_Add.Size = new System.Drawing.Size(100, 23);
			this.btnDevice_Add.TabIndex = 4;
			this.btnDevice_Add.Text = "Device Add";
			this.btnDevice_Add.UseVisualStyleBackColor = true;
			this.btnDevice_Add.Click += new System.EventHandler(this.btnDevice_Add_Click);
			// 
			// btnDevice_Change
			// 
			this.btnDevice_Change.Location = new System.Drawing.Point(370, 50);
			this.btnDevice_Change.Name = "btnDevice_Change";
			this.btnDevice_Change.Size = new System.Drawing.Size(100, 23);
			this.btnDevice_Change.TabIndex = 3;
			this.btnDevice_Change.Text = "Device Change";
			this.btnDevice_Change.UseVisualStyleBackColor = true;
			this.btnDevice_Change.Click += new System.EventHandler(this.btnDevice_Change_Click);
			// 
			// btnDevice_Delete
			// 
			this.btnDevice_Delete.Location = new System.Drawing.Point(130, 80);
			this.btnDevice_Delete.Name = "btnDevice_Delete";
			this.btnDevice_Delete.Size = new System.Drawing.Size(100, 23);
			this.btnDevice_Delete.TabIndex = 5;
			this.btnDevice_Delete.Text = "Device Delete";
			this.btnDevice_Delete.UseVisualStyleBackColor = true;
			this.btnDevice_Delete.Click += new System.EventHandler(this.btnDevice_Delete_Click);
			// 
			// btnDevice_Edit
			// 
			this.btnDevice_Edit.Location = new System.Drawing.Point(250, 80);
			this.btnDevice_Edit.Name = "btnDevice_Edit";
			this.btnDevice_Edit.Size = new System.Drawing.Size(100, 23);
			this.btnDevice_Edit.TabIndex = 6;
			this.btnDevice_Edit.Text = "Device Edit";
			this.btnDevice_Edit.UseVisualStyleBackColor = true;
			this.btnDevice_Edit.Click += new System.EventHandler(this.btnDevice_Edit_Click);
			// 
			// cboDevice
			// 
			this.cboDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboDevice.FormattingEnabled = true;
			this.cboDevice.Location = new System.Drawing.Point(10, 20);
			this.cboDevice.Name = "cboDevice";
			this.cboDevice.Size = new System.Drawing.Size(340, 23);
			this.cboDevice.TabIndex = 0;
			// 
			// grpAlarm
			// 
			this.grpAlarm.Controls.Add(this.btnAlarm_Reset);
			this.grpAlarm.Controls.Add(this.btnAlarm_Set);
			this.grpAlarm.Controls.Add(this.cboALID);
			this.grpAlarm.Location = new System.Drawing.Point(10, 70);
			this.grpAlarm.Name = "grpAlarm";
			this.grpAlarm.Size = new System.Drawing.Size(490, 50);
			this.grpAlarm.TabIndex = 2;
			this.grpAlarm.TabStop = false;
			this.grpAlarm.Text = "Alarm";
			// 
			// btnAlarm_Reset
			// 
			this.btnAlarm_Reset.Location = new System.Drawing.Point(370, 20);
			this.btnAlarm_Reset.Name = "btnAlarm_Reset";
			this.btnAlarm_Reset.Size = new System.Drawing.Size(100, 23);
			this.btnAlarm_Reset.TabIndex = 2;
			this.btnAlarm_Reset.Text = "Alarm Reset";
			this.btnAlarm_Reset.UseVisualStyleBackColor = true;
			this.btnAlarm_Reset.Click += new System.EventHandler(this.btnAlarm_Reset_Click);
			// 
			// btnAlarm_Set
			// 
			this.btnAlarm_Set.Location = new System.Drawing.Point(250, 20);
			this.btnAlarm_Set.Name = "btnAlarm_Set";
			this.btnAlarm_Set.Size = new System.Drawing.Size(100, 23);
			this.btnAlarm_Set.TabIndex = 1;
			this.btnAlarm_Set.Text = "Alarm Set";
			this.btnAlarm_Set.UseVisualStyleBackColor = true;
			this.btnAlarm_Set.Click += new System.EventHandler(this.btnAlarm_Set_Click);
			// 
			// cboALID
			// 
			this.cboALID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboALID.FormattingEnabled = true;
			this.cboALID.Location = new System.Drawing.Point(10, 20);
			this.cboALID.Name = "cboALID";
			this.cboALID.Size = new System.Drawing.Size(121, 23);
			this.cboALID.TabIndex = 0;
			// 
			// grpES
			// 
			this.grpES.Controls.Add(this.radES_Down);
			this.grpES.Controls.Add(this.radES_Run);
			this.grpES.Controls.Add(this.radES_Ready);
			this.grpES.Controls.Add(this.radES_Idle);
			this.grpES.Location = new System.Drawing.Point(10, 10);
			this.grpES.Name = "grpES";
			this.grpES.Size = new System.Drawing.Size(490, 50);
			this.grpES.TabIndex = 0;
			this.grpES.TabStop = false;
			this.grpES.Text = "Equipment Status";
			// 
			// radES_Down
			// 
			this.radES_Down.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radES_Down.Location = new System.Drawing.Point(370, 20);
			this.radES_Down.Name = "radES_Down";
			this.radES_Down.Size = new System.Drawing.Size(100, 24);
			this.radES_Down.TabIndex = 3;
			this.radES_Down.TabStop = true;
			this.radES_Down.Tag = "4";
			this.radES_Down.Text = "Down";
			this.radES_Down.UseVisualStyleBackColor = true;
			this.radES_Down.CheckedChanged += new System.EventHandler(this.radES_CheckedChanged);
			// 
			// radES_Run
			// 
			this.radES_Run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radES_Run.Location = new System.Drawing.Point(250, 20);
			this.radES_Run.Name = "radES_Run";
			this.radES_Run.Size = new System.Drawing.Size(100, 24);
			this.radES_Run.TabIndex = 2;
			this.radES_Run.TabStop = true;
			this.radES_Run.Tag = "2";
			this.radES_Run.Text = "Ready";
			this.radES_Run.UseVisualStyleBackColor = true;
			this.radES_Run.CheckedChanged += new System.EventHandler(this.radES_CheckedChanged);
			// 
			// radES_Ready
			// 
			this.radES_Ready.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radES_Ready.Location = new System.Drawing.Point(130, 20);
			this.radES_Ready.Name = "radES_Ready";
			this.radES_Ready.Size = new System.Drawing.Size(100, 24);
			this.radES_Ready.TabIndex = 1;
			this.radES_Ready.TabStop = true;
			this.radES_Ready.Tag = "1";
			this.radES_Ready.Text = "Run";
			this.radES_Ready.UseVisualStyleBackColor = true;
			this.radES_Ready.CheckedChanged += new System.EventHandler(this.radES_CheckedChanged);
			// 
			// radES_Idle
			// 
			this.radES_Idle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radES_Idle.Location = new System.Drawing.Point(10, 20);
			this.radES_Idle.Name = "radES_Idle";
			this.radES_Idle.Size = new System.Drawing.Size(100, 24);
			this.radES_Idle.TabIndex = 0;
			this.radES_Idle.TabStop = true;
			this.radES_Idle.Tag = "0";
			this.radES_Idle.Text = "Idle";
			this.radES_Idle.UseVisualStyleBackColor = true;
			this.radES_Idle.CheckedChanged += new System.EventHandler(this.radES_CheckedChanged);
			// 
			// grpVerification
			// 
			this.grpVerification.BackColor = System.Drawing.Color.Wheat;
			this.grpVerification.Controls.Add(this.lblV_MGZID);
			this.grpVerification.Controls.Add(this.txtV_MGZID);
			this.grpVerification.Controls.Add(this.lblV_MGZPort);
			this.grpVerification.Controls.Add(this.btnV_MGZVerifyRequest);
			this.grpVerification.Controls.Add(this.txtV_MGZPort);
			this.grpVerification.Controls.Add(this.lblV_MatID);
			this.grpVerification.Controls.Add(this.txtV_MatID);
			this.grpVerification.Controls.Add(this.lblV_MatLoc);
			this.grpVerification.Controls.Add(this.btnV_MatVerifyRequest);
			this.grpVerification.Controls.Add(this.txtV_MatLoc);
			this.grpVerification.Controls.Add(this.lblV_ToolID);
			this.grpVerification.Controls.Add(this.txtV_ToolID);
			this.grpVerification.Controls.Add(this.lblV_ToolLoc);
			this.grpVerification.Controls.Add(this.btnV_ToolVerifyRequest);
			this.grpVerification.Controls.Add(this.txtV_ToolLoc);
			this.grpVerification.Controls.Add(this.btnV_MGZEnded);
			this.grpVerification.Location = new System.Drawing.Point(80, 468);
			this.grpVerification.Name = "grpVerification";
			this.grpVerification.Size = new System.Drawing.Size(490, 205);
			this.grpVerification.TabIndex = 6;
			this.grpVerification.TabStop = false;
			this.grpVerification.Text = "Verification";
			// 
			// lblV_MGZID
			// 
			this.lblV_MGZID.Location = new System.Drawing.Point(10, 140);
			this.lblV_MGZID.Name = "lblV_MGZID";
			this.lblV_MGZID.Size = new System.Drawing.Size(100, 23);
			this.lblV_MGZID.TabIndex = 0;
			this.lblV_MGZID.Text = "MGZ ID";
			this.lblV_MGZID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtV_MGZID
			// 
			this.txtV_MGZID.Location = new System.Drawing.Point(130, 140);
			this.txtV_MGZID.Name = "txtV_MGZID";
			this.txtV_MGZID.Size = new System.Drawing.Size(220, 21);
			this.txtV_MGZID.TabIndex = 5;
			// 
			// lblV_MGZPort
			// 
			this.lblV_MGZPort.Location = new System.Drawing.Point(10, 170);
			this.lblV_MGZPort.Name = "lblV_MGZPort";
			this.lblV_MGZPort.Size = new System.Drawing.Size(100, 23);
			this.lblV_MGZPort.TabIndex = 0;
			this.lblV_MGZPort.Text = "MGZ Port";
			this.lblV_MGZPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnV_MGZVerifyRequest
			// 
			this.btnV_MGZVerifyRequest.Location = new System.Drawing.Point(370, 140);
			this.btnV_MGZVerifyRequest.Name = "btnV_MGZVerifyRequest";
			this.btnV_MGZVerifyRequest.Size = new System.Drawing.Size(100, 54);
			this.btnV_MGZVerifyRequest.TabIndex = 9;
			this.btnV_MGZVerifyRequest.Text = "MGZ Verify Request";
			this.btnV_MGZVerifyRequest.UseVisualStyleBackColor = true;
			this.btnV_MGZVerifyRequest.Click += new System.EventHandler(this.btnV_MGZVerifyRequest_Click);
			// 
			// txtV_MGZPort
			// 
			this.txtV_MGZPort.Location = new System.Drawing.Point(130, 170);
			this.txtV_MGZPort.Name = "txtV_MGZPort";
			this.txtV_MGZPort.Size = new System.Drawing.Size(100, 21);
			this.txtV_MGZPort.TabIndex = 6;
			// 
			// lblV_MatID
			// 
			this.lblV_MatID.Location = new System.Drawing.Point(10, 80);
			this.lblV_MatID.Name = "lblV_MatID";
			this.lblV_MatID.Size = new System.Drawing.Size(100, 23);
			this.lblV_MatID.TabIndex = 0;
			this.lblV_MatID.Text = "Mat ID";
			this.lblV_MatID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtV_MatID
			// 
			this.txtV_MatID.Location = new System.Drawing.Point(130, 80);
			this.txtV_MatID.Name = "txtV_MatID";
			this.txtV_MatID.Size = new System.Drawing.Size(220, 21);
			this.txtV_MatID.TabIndex = 3;
			// 
			// lblV_MatLoc
			// 
			this.lblV_MatLoc.Location = new System.Drawing.Point(10, 110);
			this.lblV_MatLoc.Name = "lblV_MatLoc";
			this.lblV_MatLoc.Size = new System.Drawing.Size(100, 23);
			this.lblV_MatLoc.TabIndex = 0;
			this.lblV_MatLoc.Text = "Mat Loc";
			this.lblV_MatLoc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnV_MatVerifyRequest
			// 
			this.btnV_MatVerifyRequest.Location = new System.Drawing.Point(370, 80);
			this.btnV_MatVerifyRequest.Name = "btnV_MatVerifyRequest";
			this.btnV_MatVerifyRequest.Size = new System.Drawing.Size(100, 54);
			this.btnV_MatVerifyRequest.TabIndex = 8;
			this.btnV_MatVerifyRequest.Text = "Mat Verify Request";
			this.btnV_MatVerifyRequest.UseVisualStyleBackColor = true;
			this.btnV_MatVerifyRequest.Click += new System.EventHandler(this.btnV_MatVerifyRequest_Click);
			// 
			// txtV_MatLoc
			// 
			this.txtV_MatLoc.Location = new System.Drawing.Point(130, 110);
			this.txtV_MatLoc.Name = "txtV_MatLoc";
			this.txtV_MatLoc.Size = new System.Drawing.Size(220, 21);
			this.txtV_MatLoc.TabIndex = 4;
			// 
			// lblV_ToolID
			// 
			this.lblV_ToolID.Location = new System.Drawing.Point(10, 20);
			this.lblV_ToolID.Name = "lblV_ToolID";
			this.lblV_ToolID.Size = new System.Drawing.Size(100, 23);
			this.lblV_ToolID.TabIndex = 0;
			this.lblV_ToolID.Text = "Tool ID";
			this.lblV_ToolID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtV_ToolID
			// 
			this.txtV_ToolID.Location = new System.Drawing.Point(130, 20);
			this.txtV_ToolID.Name = "txtV_ToolID";
			this.txtV_ToolID.Size = new System.Drawing.Size(220, 21);
			this.txtV_ToolID.TabIndex = 1;
			// 
			// lblV_ToolLoc
			// 
			this.lblV_ToolLoc.Location = new System.Drawing.Point(10, 50);
			this.lblV_ToolLoc.Name = "lblV_ToolLoc";
			this.lblV_ToolLoc.Size = new System.Drawing.Size(100, 23);
			this.lblV_ToolLoc.TabIndex = 0;
			this.lblV_ToolLoc.Text = "Tool Loc";
			this.lblV_ToolLoc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnV_ToolVerifyRequest
			// 
			this.btnV_ToolVerifyRequest.Location = new System.Drawing.Point(370, 20);
			this.btnV_ToolVerifyRequest.Name = "btnV_ToolVerifyRequest";
			this.btnV_ToolVerifyRequest.Size = new System.Drawing.Size(100, 54);
			this.btnV_ToolVerifyRequest.TabIndex = 7;
			this.btnV_ToolVerifyRequest.Text = "Tool Verify Request";
			this.btnV_ToolVerifyRequest.UseVisualStyleBackColor = true;
			this.btnV_ToolVerifyRequest.Click += new System.EventHandler(this.btnV_ToolVerifyRequest_Click);
			// 
			// txtV_ToolLoc
			// 
			this.txtV_ToolLoc.Location = new System.Drawing.Point(130, 50);
			this.txtV_ToolLoc.Name = "txtV_ToolLoc";
			this.txtV_ToolLoc.Size = new System.Drawing.Size(220, 21);
			this.txtV_ToolLoc.TabIndex = 2;
			// 
			// btnV_MGZEnded
			// 
			this.btnV_MGZEnded.Location = new System.Drawing.Point(250, 170);
			this.btnV_MGZEnded.Name = "btnV_MGZEnded";
			this.btnV_MGZEnded.Size = new System.Drawing.Size(100, 23);
			this.btnV_MGZEnded.TabIndex = 10;
			this.btnV_MGZEnded.Text = "MGZ Ended";
			this.btnV_MGZEnded.UseVisualStyleBackColor = true;
			this.btnV_MGZEnded.Click += new System.EventHandler(this.btnV_MGZEnded_Click);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "notifyIcon1";
			this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			// 
			// pnlHSMS_Base
			// 
			this.pnlHSMS_Base.BackColor = System.Drawing.Color.MistyRose;
			this.pnlHSMS_Base.Controls.Add(this.lblComm_Status);
			this.pnlHSMS_Base.Controls.Add(this.listLog);
			this.pnlHSMS_Base.Controls.Add(this.btnMini);
			this.pnlHSMS_Base.Controls.Add(this.lblCont_Status);
			this.pnlHSMS_Base.Controls.Add(this.lblConn_Status);
			this.pnlHSMS_Base.Controls.Add(this.btnConn_Stop);
			this.pnlHSMS_Base.Controls.Add(this.btnCont_Local);
			this.pnlHSMS_Base.Controls.Add(this.btnCont_Remote);
			this.pnlHSMS_Base.Controls.Add(this.btnCont_Offline);
			this.pnlHSMS_Base.Controls.Add(this.btnConn_Start);
			this.pnlHSMS_Base.Controls.Add(this.grpHSMS);
			this.pnlHSMS_Base.Location = new System.Drawing.Point(0, 7);
			this.pnlHSMS_Base.Name = "pnlHSMS_Base";
			this.pnlHSMS_Base.Size = new System.Drawing.Size(550, 455);
			this.pnlHSMS_Base.TabIndex = 7;
			// 
			// lblComm_Status
			// 
			this.lblComm_Status.BackColor = System.Drawing.Color.Tomato;
			this.lblComm_Status.Location = new System.Drawing.Point(10, 10);
			this.lblComm_Status.Name = "lblComm_Status";
			this.lblComm_Status.Size = new System.Drawing.Size(200, 23);
			this.lblComm_Status.TabIndex = 4;
			this.lblComm_Status.Text = "Not Communication";
			this.lblComm_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listLog
			// 
			this.listLog.FormattingEnabled = true;
			this.listLog.HorizontalScrollbar = true;
			this.listLog.ItemHeight = 15;
			this.listLog.Location = new System.Drawing.Point(220, 105);
			this.listLog.Name = "listLog";
			this.listLog.Size = new System.Drawing.Size(320, 334);
			this.listLog.TabIndex = 3;
			// 
			// btnMini
			// 
			this.btnMini.Location = new System.Drawing.Point(440, 10);
			this.btnMini.Name = "btnMini";
			this.btnMini.Size = new System.Drawing.Size(100, 23);
			this.btnMini.TabIndex = 0;
			this.btnMini.Text = "Minimization";
			this.btnMini.UseVisualStyleBackColor = true;
			this.btnMini.Click += new System.EventHandler(this.btnMini_Click_1);
			// 
			// lblCont_Status
			// 
			this.lblCont_Status.BackColor = System.Drawing.Color.Tomato;
			this.lblCont_Status.Location = new System.Drawing.Point(10, 70);
			this.lblCont_Status.Name = "lblCont_Status";
			this.lblCont_Status.Size = new System.Drawing.Size(200, 23);
			this.lblCont_Status.TabIndex = 1;
			this.lblCont_Status.Text = "Offline";
			this.lblCont_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblConn_Status
			// 
			this.lblConn_Status.BackColor = System.Drawing.Color.Tomato;
			this.lblConn_Status.Location = new System.Drawing.Point(10, 40);
			this.lblConn_Status.Name = "lblConn_Status";
			this.lblConn_Status.Size = new System.Drawing.Size(200, 23);
			this.lblConn_Status.TabIndex = 1;
			this.lblConn_Status.Text = "Disconnect";
			this.lblConn_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnConn_Stop
			// 
			this.btnConn_Stop.Enabled = false;
			this.btnConn_Stop.Location = new System.Drawing.Point(330, 40);
			this.btnConn_Stop.Name = "btnConn_Stop";
			this.btnConn_Stop.Size = new System.Drawing.Size(100, 23);
			this.btnConn_Stop.TabIndex = 0;
			this.btnConn_Stop.Text = "Stop";
			this.btnConn_Stop.UseVisualStyleBackColor = true;
			this.btnConn_Stop.Click += new System.EventHandler(this.btnConn_Stop_Click);
			// 
			// btnCont_Local
			// 
			this.btnCont_Local.Enabled = false;
			this.btnCont_Local.Location = new System.Drawing.Point(330, 70);
			this.btnCont_Local.Name = "btnCont_Local";
			this.btnCont_Local.Size = new System.Drawing.Size(100, 23);
			this.btnCont_Local.TabIndex = 0;
			this.btnCont_Local.Text = "Local";
			this.btnCont_Local.UseVisualStyleBackColor = true;
			this.btnCont_Local.Click += new System.EventHandler(this.btnCont_Local_Click_1);
			// 
			// btnCont_Remote
			// 
			this.btnCont_Remote.Enabled = false;
			this.btnCont_Remote.Location = new System.Drawing.Point(440, 70);
			this.btnCont_Remote.Name = "btnCont_Remote";
			this.btnCont_Remote.Size = new System.Drawing.Size(100, 23);
			this.btnCont_Remote.TabIndex = 0;
			this.btnCont_Remote.Text = "Remote";
			this.btnCont_Remote.UseVisualStyleBackColor = true;
			this.btnCont_Remote.Click += new System.EventHandler(this.btnCont_Remote_Click_1);
			// 
			// btnCont_Offline
			// 
			this.btnCont_Offline.Enabled = false;
			this.btnCont_Offline.Location = new System.Drawing.Point(220, 70);
			this.btnCont_Offline.Name = "btnCont_Offline";
			this.btnCont_Offline.Size = new System.Drawing.Size(100, 23);
			this.btnCont_Offline.TabIndex = 0;
			this.btnCont_Offline.Text = "Offline";
			this.btnCont_Offline.UseVisualStyleBackColor = true;
			this.btnCont_Offline.Click += new System.EventHandler(this.btnCont_Offline_Click_1);
			// 
			// btnConn_Start
			// 
			this.btnConn_Start.Location = new System.Drawing.Point(220, 40);
			this.btnConn_Start.Name = "btnConn_Start";
			this.btnConn_Start.Size = new System.Drawing.Size(100, 23);
			this.btnConn_Start.TabIndex = 0;
			this.btnConn_Start.Text = "Start";
			this.btnConn_Start.UseVisualStyleBackColor = true;
			this.btnConn_Start.Click += new System.EventHandler(this.btnConn_Start_Click);
			// 
			// grpHSMS
			// 
			this.grpHSMS.Controls.Add(this.lblHSMS_CRT);
			this.grpHSMS.Controls.Add(this.btnHSMS_Save);
			this.grpHSMS.Controls.Add(this.lblHSMS_T8);
			this.grpHSMS.Controls.Add(this.lblHSMS_T7);
			this.grpHSMS.Controls.Add(this.lblHSMS_T6);
			this.grpHSMS.Controls.Add(this.lblHSMS_T5);
			this.grpHSMS.Controls.Add(this.lblHSMS_T3);
			this.grpHSMS.Controls.Add(this.lblHSMS_LT);
			this.grpHSMS.Controls.Add(this.lblHSMS_DID);
			this.grpHSMS.Controls.Add(this.lblHSMS_Port_Title);
			this.grpHSMS.Controls.Add(this.txtHSMS_T81);
			this.grpHSMS.Controls.Add(this.txtHSMS_T71);
			this.grpHSMS.Controls.Add(this.txtHSMS_CRT1);
			this.grpHSMS.Controls.Add(this.txtHSMS_T51);
			this.grpHSMS.Controls.Add(this.txtHSMS_DID1);
			this.grpHSMS.Controls.Add(this.txtHSMS_T61);
			this.grpHSMS.Controls.Add(this.txtHSMS_LT1);
			this.grpHSMS.Controls.Add(this.txtHSMS_T31);
			this.grpHSMS.Controls.Add(this.txtHSMS_Port1);
			this.grpHSMS.Location = new System.Drawing.Point(10, 105);
			this.grpHSMS.Name = "grpHSMS";
			this.grpHSMS.Size = new System.Drawing.Size(200, 343);
			this.grpHSMS.TabIndex = 2;
			this.grpHSMS.TabStop = false;
			this.grpHSMS.Text = "HSMS II";
			// 
			// lblHSMS_CRT
			// 
			this.lblHSMS_CRT.Location = new System.Drawing.Point(10, 110);
			this.lblHSMS_CRT.Name = "lblHSMS_CRT";
			this.lblHSMS_CRT.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_CRT.TabIndex = 17;
			this.lblHSMS_CRT.Text = "Comm Request";
			this.lblHSMS_CRT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnHSMS_Save
			// 
			this.btnHSMS_Save.Location = new System.Drawing.Point(70, 300);
			this.btnHSMS_Save.Name = "btnHSMS_Save";
			this.btnHSMS_Save.Size = new System.Drawing.Size(100, 23);
			this.btnHSMS_Save.TabIndex = 10;
			this.btnHSMS_Save.Text = "Save";
			this.btnHSMS_Save.UseVisualStyleBackColor = true;
			// 
			// lblHSMS_T8
			// 
			this.lblHSMS_T8.Location = new System.Drawing.Point(10, 260);
			this.lblHSMS_T8.Name = "lblHSMS_T8";
			this.lblHSMS_T8.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_T8.TabIndex = 14;
			this.lblHSMS_T8.Text = "T8";
			this.lblHSMS_T8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_T7
			// 
			this.lblHSMS_T7.Location = new System.Drawing.Point(10, 230);
			this.lblHSMS_T7.Name = "lblHSMS_T7";
			this.lblHSMS_T7.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_T7.TabIndex = 12;
			this.lblHSMS_T7.Text = "T7";
			this.lblHSMS_T7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_T6
			// 
			this.lblHSMS_T6.Location = new System.Drawing.Point(10, 200);
			this.lblHSMS_T6.Name = "lblHSMS_T6";
			this.lblHSMS_T6.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_T6.TabIndex = 10;
			this.lblHSMS_T6.Text = "T6";
			this.lblHSMS_T6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_T5
			// 
			this.lblHSMS_T5.Location = new System.Drawing.Point(10, 170);
			this.lblHSMS_T5.Name = "lblHSMS_T5";
			this.lblHSMS_T5.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_T5.TabIndex = 8;
			this.lblHSMS_T5.Text = "T5";
			this.lblHSMS_T5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_T3
			// 
			this.lblHSMS_T3.Location = new System.Drawing.Point(10, 140);
			this.lblHSMS_T3.Name = "lblHSMS_T3";
			this.lblHSMS_T3.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_T3.TabIndex = 6;
			this.lblHSMS_T3.Text = "T3";
			this.lblHSMS_T3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_LT
			// 
			this.lblHSMS_LT.Location = new System.Drawing.Point(10, 80);
			this.lblHSMS_LT.Name = "lblHSMS_LT";
			this.lblHSMS_LT.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_LT.TabIndex = 4;
			this.lblHSMS_LT.Text = "Link Test";
			this.lblHSMS_LT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_DID
			// 
			this.lblHSMS_DID.Location = new System.Drawing.Point(10, 50);
			this.lblHSMS_DID.Name = "lblHSMS_DID";
			this.lblHSMS_DID.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_DID.TabIndex = 2;
			this.lblHSMS_DID.Text = "Device ID";
			this.lblHSMS_DID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHSMS_Port_Title
			// 
			this.lblHSMS_Port_Title.Location = new System.Drawing.Point(10, 20);
			this.lblHSMS_Port_Title.Name = "lblHSMS_Port_Title";
			this.lblHSMS_Port_Title.Size = new System.Drawing.Size(100, 23);
			this.lblHSMS_Port_Title.TabIndex = 0;
			this.lblHSMS_Port_Title.Text = "Port";
			this.lblHSMS_Port_Title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtHSMS_T81
			// 
			this.txtHSMS_T81.Location = new System.Drawing.Point(116, 260);
			this.txtHSMS_T81.Name = "txtHSMS_T81";
			this.txtHSMS_T81.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_T81.TabIndex = 4;
			this.txtHSMS_T81.Text = "30";
			this.txtHSMS_T81.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_T71
			// 
			this.txtHSMS_T71.Location = new System.Drawing.Point(116, 231);
			this.txtHSMS_T71.Name = "txtHSMS_T71";
			this.txtHSMS_T71.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_T71.TabIndex = 4;
			this.txtHSMS_T71.Text = "30";
			this.txtHSMS_T71.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_CRT1
			// 
			this.txtHSMS_CRT1.Location = new System.Drawing.Point(116, 111);
			this.txtHSMS_CRT1.Name = "txtHSMS_CRT1";
			this.txtHSMS_CRT1.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_CRT1.TabIndex = 4;
			this.txtHSMS_CRT1.Text = "5";
			this.txtHSMS_CRT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_T51
			// 
			this.txtHSMS_T51.Location = new System.Drawing.Point(116, 170);
			this.txtHSMS_T51.Name = "txtHSMS_T51";
			this.txtHSMS_T51.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_T51.TabIndex = 4;
			this.txtHSMS_T51.Text = "30";
			this.txtHSMS_T51.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_DID1
			// 
			this.txtHSMS_DID1.Location = new System.Drawing.Point(116, 50);
			this.txtHSMS_DID1.Name = "txtHSMS_DID1";
			this.txtHSMS_DID1.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_DID1.TabIndex = 4;
			this.txtHSMS_DID1.Text = "0";
			this.txtHSMS_DID1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_T61
			// 
			this.txtHSMS_T61.Location = new System.Drawing.Point(116, 202);
			this.txtHSMS_T61.Name = "txtHSMS_T61";
			this.txtHSMS_T61.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_T61.TabIndex = 4;
			this.txtHSMS_T61.Text = "30";
			this.txtHSMS_T61.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_LT1
			// 
			this.txtHSMS_LT1.Location = new System.Drawing.Point(116, 82);
			this.txtHSMS_LT1.Name = "txtHSMS_LT1";
			this.txtHSMS_LT1.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_LT1.TabIndex = 4;
			this.txtHSMS_LT1.Text = "60";
			this.txtHSMS_LT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_T31
			// 
			this.txtHSMS_T31.Location = new System.Drawing.Point(116, 141);
			this.txtHSMS_T31.Name = "txtHSMS_T31";
			this.txtHSMS_T31.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_T31.TabIndex = 4;
			this.txtHSMS_T31.Text = "15";
			this.txtHSMS_T31.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtHSMS_Port1
			// 
			this.txtHSMS_Port1.Location = new System.Drawing.Point(116, 21);
			this.txtHSMS_Port1.Name = "txtHSMS_Port1";
			this.txtHSMS_Port1.Size = new System.Drawing.Size(78, 21);
			this.txtHSMS_Port1.TabIndex = 4;
			this.txtHSMS_Port1.Text = "5000";
			this.txtHSMS_Port1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// frmGEM
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1059, 685);
			this.ControlBox = false;
			this.Controls.Add(this.pnlHSMS_Base);
			this.Controls.Add(this.grpVerification);
			this.Controls.Add(this.pnlGEM_Base);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmGEM";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "SECSGEM";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGEM_FormClosing);
			this.Load += new System.EventHandler(this.frmGEM_Load);
			this.Resize += new System.EventHandler(this.frmGEM_Resize);
			this.pnlGEM_Base.ResumeLayout(false);
			this.grpStrip.ResumeLayout(false);
			this.grpStrip.PerformLayout();
			this.grpDevice.ResumeLayout(false);
			this.grpDevice.PerformLayout();
			this.grpAlarm.ResumeLayout(false);
			this.grpES.ResumeLayout(false);
			this.grpVerification.ResumeLayout(false);
			this.grpVerification.PerformLayout();
			this.pnlHSMS_Base.ResumeLayout(false);
			this.grpHSMS.ResumeLayout(false);
			this.grpHSMS.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion
        private ExTextBox txtHSMS_T8;
        private ExTextBox txtHSMS_T7;
        private ExTextBox txtHSMS_T6;
        private ExTextBox txtHSMS_T5;
        private ExTextBox txtHSMS_T3;
        private ExTextBox txtHSMS_LT;
        private ExTextBox txtHSMS_DID;
        private ExTextBox txtHSMS_Port;
        
        private System.Windows.Forms.Timer tmrGEM;
        private System.Windows.Forms.Timer tmrSet;
        private System.Windows.Forms.Panel pnlGEM_Base;
        private System.Windows.Forms.GroupBox grpES;
        private System.Windows.Forms.RadioButton radES_Down;
        private System.Windows.Forms.RadioButton radES_Run;
        private System.Windows.Forms.RadioButton radES_Ready;
        private System.Windows.Forms.RadioButton radES_Idle;
        private System.Windows.Forms.GroupBox grpAlarm;
        private System.Windows.Forms.ComboBox cboALID;
        private System.Windows.Forms.Button btnAlarm_Reset;
        private System.Windows.Forms.Button btnAlarm_Set;
        private System.Windows.Forms.GroupBox grpDevice;
        private System.Windows.Forms.TextBox txtDevice;
        private System.Windows.Forms.Button btnDevice_Search;
        private System.Windows.Forms.Button btnDevice_Add;
        private System.Windows.Forms.Button btnDevice_Change;
        private System.Windows.Forms.Button btnDevice_Delete;
        private System.Windows.Forms.Button btnDevice_Edit;
        private System.Windows.Forms.ComboBox cboDevice;
        private ExTextBox txtHSMS_CRT;
        private System.Windows.Forms.GroupBox grpVerification;
        private System.Windows.Forms.Label lblV_MGZID;
        private System.Windows.Forms.TextBox txtV_MGZID;
        private System.Windows.Forms.Label lblV_MGZPort;
        private System.Windows.Forms.Button btnV_MGZVerifyRequest;
        private System.Windows.Forms.TextBox txtV_MGZPort;
        private System.Windows.Forms.Label lblV_MatID;
        private System.Windows.Forms.TextBox txtV_MatID;
        private System.Windows.Forms.Label lblV_MatLoc;
        private System.Windows.Forms.Button btnV_MatVerifyRequest;
        private System.Windows.Forms.TextBox txtV_MatLoc;
        private System.Windows.Forms.Label lblV_ToolID;
        private System.Windows.Forms.TextBox txtV_ToolID;
        private System.Windows.Forms.Label lblV_ToolLoc;
        private System.Windows.Forms.Button btnV_ToolVerifyRequest;
        private System.Windows.Forms.TextBox txtV_ToolLoc;
        private System.Windows.Forms.Button btnV_MGZEnded;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Panel pnlHSMS_Base;
        private System.Windows.Forms.Label lblComm_Status;
        private System.Windows.Forms.ListBox listLog;
        private System.Windows.Forms.Button btnMini;
        private System.Windows.Forms.Label lblCont_Status;
        private System.Windows.Forms.Label lblConn_Status;
        private System.Windows.Forms.Button btnConn_Stop;
        private System.Windows.Forms.Button btnCont_Local;
        private System.Windows.Forms.Button btnCont_Remote;
        private System.Windows.Forms.Button btnCont_Offline;
        private System.Windows.Forms.Button btnConn_Start;
        private System.Windows.Forms.GroupBox grpHSMS;
        private System.Windows.Forms.Label lblHSMS_CRT;
        private System.Windows.Forms.Button btnHSMS_Save;
        private System.Windows.Forms.Label lblHSMS_T8;
        private System.Windows.Forms.Label lblHSMS_T7;
        private System.Windows.Forms.Label lblHSMS_T6;
        private System.Windows.Forms.Label lblHSMS_T5;
        private System.Windows.Forms.Label lblHSMS_T3;
        private System.Windows.Forms.Label lblHSMS_LT;
        private System.Windows.Forms.Label lblHSMS_DID;
        private System.Windows.Forms.Label lblHSMS_Port_Title;
        private System.Windows.Forms.TextBox txtHSMS_Port1;
        private System.Windows.Forms.TextBox txtHSMS_T81;
        private System.Windows.Forms.TextBox txtHSMS_T71;
        private System.Windows.Forms.TextBox txtHSMS_CRT1;
        private System.Windows.Forms.TextBox txtHSMS_T51;
        private System.Windows.Forms.TextBox txtHSMS_DID1;
        private System.Windows.Forms.TextBox txtHSMS_T61;
        private System.Windows.Forms.TextBox txtHSMS_LT1;
        private System.Windows.Forms.TextBox txtHSMS_T31;
        private System.Windows.Forms.GroupBox grpStrip;
        private System.Windows.Forms.Button btnStrip_LOTEnded;
        private System.Windows.Forms.Label lblStrip_Proceed;
        private System.Windows.Forms.TextBox txtStrip_Proceed;
        private System.Windows.Forms.Label lblStrip_TotalUnit;
        private System.Windows.Forms.TextBox txtStrip_TotalUnit;
        private System.Windows.Forms.Button btnStrip_CarrierEnd1;
        private System.Windows.Forms.TextBox txtStrip_EndStrip;
        private System.Windows.Forms.Button btnStrip_EndM;
        private System.Windows.Forms.TextBox txtStrip_MiddleStrip;
        private System.Windows.Forms.Button btnStrip_MiddleM;
        private System.Windows.Forms.TextBox txtStrip_BeforeStrip;
        private System.Windows.Forms.Button btnStrip_BeforeM;
        private System.Windows.Forms.Label lblStrip_SlotNo;
        private System.Windows.Forms.TextBox txtStrip_SlotNo;
        private System.Windows.Forms.Label lblStrip_StripID;
        private System.Windows.Forms.TextBox txtStrip_StripID;
        private System.Windows.Forms.Label lblStrip_CarrierID;
        private System.Windows.Forms.TextBox txtStrip_CarrierID;
        private System.Windows.Forms.Button btnStrip_CarrierCreate;
        private System.Windows.Forms.Button btnStrip_LOTVerified;
        private System.Windows.Forms.Button btnStrip_CarrierStarted;
        private System.Windows.Forms.Button btnStrip_ClearCarrierList;
        private System.Windows.Forms.Button btnStrip_GetStripID;
        private System.Windows.Forms.Label lblStrip_GoodNg;
        private System.Windows.Forms.TextBox txtStrip_GoodNg;
        private System.Windows.Forms.Label lblStrip_LOTID;
        private System.Windows.Forms.Button btnStrip_CarrierVerifyRequest;
        private System.Windows.Forms.TextBox txtStrip_LOTID;
        private System.Windows.Forms.Button btnStrip_CarrierEnd2;
    }
}