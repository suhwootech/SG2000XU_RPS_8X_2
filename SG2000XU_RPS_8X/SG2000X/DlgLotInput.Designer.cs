namespace SG2000X
{
    partial class CDlgLotInput
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbl_Lot = new System.Windows.Forms.Label();
            this.txtLotName = new System.Windows.Forms.TextBox();
            this.lbl_StripCount = new System.Windows.Forms.Label();
            this.txtStripQty = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lbl_Cnt = new System.Windows.Forms.Label();
            this.cmbMgzQty = new System.Windows.Forms.ComboBox();
            this.gridLotList = new System.Windows.Forms.DataGridView();
            this.lblCaption1 = new System.Windows.Forms.Label();
            this.lblCaption2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnComplete = new System.Windows.Forms.Button();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnClearInform = new System.Windows.Forms.Button();
            this.ListNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LOTID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MgzQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StripQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InputCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridLotList)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            this.lbl_Title.BackColor = System.Drawing.Color.White;
            this.lbl_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.ForeColor = System.Drawing.Color.Black;
            this.lbl_Title.Location = new System.Drawing.Point(0, 10);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Title.Size = new System.Drawing.Size(490, 30);
            this.lbl_Title.TabIndex = 4;
            this.lbl_Title.Text = "Multi - LOT Information";
            this.lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(204, 429);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(159, 44);
            this.btnCancel.TabIndex = 34;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btn_Can_Click);
            // 
            // lbl_Lot
            // 
            this.lbl_Lot.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Lot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_Lot.ForeColor = System.Drawing.Color.Black;
            this.lbl_Lot.Location = new System.Drawing.Point(10, 341);
            this.lbl_Lot.Name = "lbl_Lot";
            this.lbl_Lot.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Lot.Size = new System.Drawing.Size(100, 25);
            this.lbl_Lot.TabIndex = 42;
            this.lbl_Lot.Text = "LOT Name";
            this.lbl_Lot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLotName
            // 
            this.txtLotName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLotName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtLotName.Location = new System.Drawing.Point(110, 341);
            this.txtLotName.Name = "txtLotName";
            this.txtLotName.Size = new System.Drawing.Size(253, 23);
            this.txtLotName.TabIndex = 41;
            this.txtLotName.Tag = "0";
            // 
            // lbl_StripCount
            // 
            this.lbl_StripCount.BackColor = System.Drawing.Color.Transparent;
            this.lbl_StripCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_StripCount.ForeColor = System.Drawing.Color.Black;
            this.lbl_StripCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_StripCount.Location = new System.Drawing.Point(9, 397);
            this.lbl_StripCount.Name = "lbl_StripCount";
            this.lbl_StripCount.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_StripCount.Size = new System.Drawing.Size(100, 25);
            this.lbl_StripCount.TabIndex = 50;
            this.lbl_StripCount.Text = "Strip Count";
            this.lbl_StripCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtStripQty
            // 
            this.txtStripQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStripQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtStripQty.Location = new System.Drawing.Point(109, 397);
            this.txtStripQty.Name = "txtStripQty";
            this.txtStripQty.Size = new System.Drawing.Size(92, 23);
            this.txtStripQty.TabIndex = 49;
            this.txtStripQty.Tag = "0";
            this.txtStripQty.Text = "0";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(369, 341);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(160, 75);
            this.btnAdd.TabIndex = 51;
            this.btnAdd.Tag = "";
            this.btnAdd.Text = "ADD";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lbl_Cnt
            // 
            this.lbl_Cnt.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Cnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_Cnt.ForeColor = System.Drawing.Color.Black;
            this.lbl_Cnt.Location = new System.Drawing.Point(9, 367);
            this.lbl_Cnt.Name = "lbl_Cnt";
            this.lbl_Cnt.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lbl_Cnt.Size = new System.Drawing.Size(100, 25);
            this.lbl_Cnt.TabIndex = 39;
            this.lbl_Cnt.Text = "MGZ Count";
            this.lbl_Cnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbMgzQty
            // 
            this.cmbMgzQty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMgzQty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMgzQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cmbMgzQty.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "9",
            "10"});
            this.cmbMgzQty.Location = new System.Drawing.Point(110, 370);
            this.cmbMgzQty.Name = "cmbMgzQty";
            this.cmbMgzQty.Size = new System.Drawing.Size(91, 24);
            this.cmbMgzQty.TabIndex = 52;
            this.cmbMgzQty.SelectedIndexChanged += new System.EventHandler(this.cmb_Cnt_SelectedIndexChanged);
            // 
            // gridLotList
            // 
            this.gridLotList.AllowUserToAddRows = false;
            this.gridLotList.AllowUserToDeleteRows = false;
            this.gridLotList.AllowUserToResizeColumns = false;
            this.gridLotList.AllowUserToResizeRows = false;
            this.gridLotList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridLotList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridLotList.ColumnHeadersHeight = 25;
            this.gridLotList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridLotList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ListNumber,
            this.LOTID,
            this.Mode,
            this.MgzQty,
            this.StripQty,
            this.State,
            this.InputCount,
            this.OutCount});
            this.gridLotList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridLotList.EnableHeadersVisualStyles = false;
            this.gridLotList.Location = new System.Drawing.Point(5, 77);
            this.gridLotList.MultiSelect = false;
            this.gridLotList.Name = "gridLotList";
            this.gridLotList.ReadOnly = true;
            this.gridLotList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridLotList.RowHeadersVisible = false;
            this.gridLotList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridLotList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLotList.ShowCellErrors = false;
            this.gridLotList.ShowCellToolTips = false;
            this.gridLotList.ShowEditingIcon = false;
            this.gridLotList.ShowRowErrors = false;
            this.gridLotList.Size = new System.Drawing.Size(526, 166);
            this.gridLotList.TabIndex = 53;
            // 
            // lblCaption1
            // 
            this.lblCaption1.BackColor = System.Drawing.Color.Transparent;
            this.lblCaption1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblCaption1.ForeColor = System.Drawing.Color.Black;
            this.lblCaption1.Location = new System.Drawing.Point(12, 47);
            this.lblCaption1.Name = "lblCaption1";
            this.lblCaption1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblCaption1.Size = new System.Drawing.Size(157, 25);
            this.lblCaption1.TabIndex = 54;
            this.lblCaption1.Text = "LOT Status";
            this.lblCaption1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCaption2
            // 
            this.lblCaption2.BackColor = System.Drawing.Color.White;
            this.lblCaption2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblCaption2.ForeColor = System.Drawing.Color.Black;
            this.lblCaption2.Location = new System.Drawing.Point(5, 303);
            this.lblCaption2.Name = "lblCaption2";
            this.lblCaption2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblCaption2.Size = new System.Drawing.Size(526, 25);
            this.lblCaption2.TabIndex = 55;
            this.lblCaption2.Text = "Input New LOT Information";
            this.lblCaption2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(369, 246);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(160, 35);
            this.btnDelete.TabIndex = 56;
            this.btnDelete.Tag = "";
            this.btnDelete.Text = "LOT Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnComplete
            // 
            this.btnComplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnComplete.FlatAppearance.BorderSize = 0;
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btnComplete.ForeColor = System.Drawing.Color.White;
            this.btnComplete.Location = new System.Drawing.Point(5, 246);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(160, 35);
            this.btnComplete.TabIndex = 57;
            this.btnComplete.Tag = "";
            this.btnComplete.Text = "Complete";
            this.btnComplete.UseVisualStyleBackColor = false;
            this.btnComplete.Visible = false;
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Interval = 500;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // btnClearInform
            // 
            this.btnClearInform.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnClearInform.FlatAppearance.BorderSize = 0;
            this.btnClearInform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearInform.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btnClearInform.ForeColor = System.Drawing.Color.White;
            this.btnClearInform.Location = new System.Drawing.Point(168, 246);
            this.btnClearInform.Name = "btnClearInform";
            this.btnClearInform.Size = new System.Drawing.Size(198, 35);
            this.btnClearInform.TabIndex = 58;
            this.btnClearInform.Tag = "";
            this.btnClearInform.Text = "Clear Product Data";
            this.btnClearInform.UseVisualStyleBackColor = false;
            this.btnClearInform.Click += new System.EventHandler(this.btnClearInform_Click);
            // 
            // ListNumber
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ListNumber.DefaultCellStyle = dataGridViewCellStyle2;
            this.ListNumber.HeaderText = "No";
            this.ListNumber.Name = "ListNumber";
            this.ListNumber.ReadOnly = true;
            this.ListNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ListNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ListNumber.Width = 25;
            // 
            // LOTID
            // 
            this.LOTID.HeaderText = "LOT ID";
            this.LOTID.Name = "LOTID";
            this.LOTID.ReadOnly = true;
            this.LOTID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LOTID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LOTID.Width = 180;
            // 
            // Mode
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Mode.DefaultCellStyle = dataGridViewCellStyle3;
            this.Mode.HeaderText = "Mode";
            this.Mode.Name = "Mode";
            this.Mode.ReadOnly = true;
            this.Mode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Mode.Width = 40;
            // 
            // MgzQty
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.MgzQty.DefaultCellStyle = dataGridViewCellStyle4;
            this.MgzQty.HeaderText = "Mgz Qty";
            this.MgzQty.Name = "MgzQty";
            this.MgzQty.ReadOnly = true;
            this.MgzQty.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MgzQty.Width = 60;
            // 
            // StripQty
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.StripQty.DefaultCellStyle = dataGridViewCellStyle5;
            this.StripQty.HeaderText = "Strip Qty";
            this.StripQty.Name = "StripQty";
            this.StripQty.ReadOnly = true;
            this.StripQty.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.StripQty.Width = 60;
            // 
            // State
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.State.DefaultCellStyle = dataGridViewCellStyle6;
            this.State.HeaderText = "State";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            this.State.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.State.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.State.Width = 50;
            // 
            // InputCount
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.InputCount.DefaultCellStyle = dataGridViewCellStyle7;
            this.InputCount.HeaderText = "In";
            this.InputCount.Name = "InputCount";
            this.InputCount.ReadOnly = true;
            this.InputCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.InputCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.InputCount.Width = 45;
            // 
            // OutCount
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.OutCount.DefaultCellStyle = dataGridViewCellStyle8;
            this.OutCount.HeaderText = "Out";
            this.OutCount.Name = "OutCount";
            this.OutCount.ReadOnly = true;
            this.OutCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.OutCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.OutCount.Width = 45;
            // 
            // CDlgLotInput
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Orange;
            this.ClientSize = new System.Drawing.Size(534, 478);
            this.Controls.Add(this.btnClearInform);
            this.Controls.Add(this.btnComplete);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblCaption2);
            this.Controls.Add(this.lblCaption1);
            this.Controls.Add(this.gridLotList);
            this.Controls.Add(this.cmbMgzQty);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lbl_StripCount);
            this.Controls.Add(this.txtStripQty);
            this.Controls.Add(this.lbl_Lot);
            this.Controls.Add(this.txtLotName);
            this.Controls.Add(this.lbl_Cnt);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbl_Title);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CDlgLotInput";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multi LOT Input";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CDlgLotInput_FormClosing);
            this.Load += new System.EventHandler(this.CDlgLotInput_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridLotList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbl_Lot;
        private System.Windows.Forms.TextBox txtLotName;
        private System.Windows.Forms.Label lbl_StripCount;
        private System.Windows.Forms.TextBox txtStripQty;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lbl_Cnt;
        private System.Windows.Forms.ComboBox cmbMgzQty;
        private System.Windows.Forms.DataGridView gridLotList;
        private System.Windows.Forms.Label lblCaption1;
        private System.Windows.Forms.Label lblCaption2;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.Button btnClearInform;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn LOTID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn MgzQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn StripQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn InputCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutCount;
    }
}