namespace SG2000X
{
    partial class vwSPC_Radar
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_RadarList = new System.Windows.Forms.DataGridView();
            this.col_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_RadarFunction = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_Content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_SDCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_CDCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_ResetCount = new System.Windows.Forms.Button();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Page_1 = new System.Windows.Forms.Button();
            this.btn_Page_101 = new System.Windows.Forms.Button();
            this.btn_Page_201 = new System.Windows.Forms.Button();
            this.btn_Page_301 = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_RadarList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_RadarList
            // 
            this.dgv_RadarList.AllowUserToAddRows = false;
            this.dgv_RadarList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_RadarList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_RadarList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_RadarList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Code,
            this.col_RadarFunction,
            this.col_Content,
            this.col_SDCount,
            this.col_CDCount});
            this.dgv_RadarList.Location = new System.Drawing.Point(9, 9);
            this.dgv_RadarList.MultiSelect = false;
            this.dgv_RadarList.Name = "dgv_RadarList";
            this.dgv_RadarList.RowHeadersVisible = false;
            this.dgv_RadarList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_RadarList.RowTemplate.Height = 23;
            this.dgv_RadarList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_RadarList.Size = new System.Drawing.Size(820, 613);
            this.dgv_RadarList.TabIndex = 256;
            this.dgv_RadarList.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgv_RadarList_CurrentCeelDirtyStateChanged);
            this.dgv_RadarList.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgv_RadarList_EditingControlShowing);
            this.dgv_RadarList.SelectionChanged += new System.EventHandler(this.dgv_RadarList_SelectionChanged);
            this.dgv_RadarList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgv_RadarList_KeyPress);
            // 
            // col_Code
            // 
            this.col_Code.HeaderText = "Code";
            this.col_Code.Name = "col_Code";
            this.col_Code.ReadOnly = true;
            this.col_Code.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Code.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.col_Code.Width = 50;
            // 
            // col_RadarFunction
            // 
            this.col_RadarFunction.HeaderText = "Radar Function";
            this.col_RadarFunction.Name = "col_RadarFunction";
            this.col_RadarFunction.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // col_Content
            // 
            this.col_Content.HeaderText = "Content";
            this.col_Content.Name = "col_Content";
            this.col_Content.ReadOnly = true;
            this.col_Content.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Content.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.col_Content.Width = 450;
            // 
            // col_SDCount
            // 
            this.col_SDCount.HeaderText = "Set Data Count";
            this.col_SDCount.Name = "col_SDCount";
            this.col_SDCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_SDCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_CDCount
            // 
            this.col_CDCount.HeaderText = "Current Data Count";
            this.col_CDCount.Name = "col_CDCount";
            this.col_CDCount.ReadOnly = true;
            this.col_CDCount.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_CDCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btn_ResetCount
            // 
            this.btn_ResetCount.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_ResetCount.FlatAppearance.BorderSize = 0;
            this.btn_ResetCount.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btn_ResetCount.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btn_ResetCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ResetCount.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btn_ResetCount.ForeColor = System.Drawing.Color.White;
            this.btn_ResetCount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_ResetCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_ResetCount.Location = new System.Drawing.Point(456, 686);
            this.btn_ResetCount.Name = "btn_ResetCount";
            this.btn_ResetCount.Padding = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.btn_ResetCount.Size = new System.Drawing.Size(150, 70);
            this.btn_ResetCount.TabIndex = 259;
            this.btn_ResetCount.Tag = "";
            this.btn_ResetCount.Text = "RESET\r\nCOUNT";
            this.btn_ResetCount.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btn_ResetCount.UseVisualStyleBackColor = false;
            this.btn_ResetCount.Click += new System.EventHandler(this.btn_ResetCount_Click);
            // 
            // btn_Exit
            // 
            this.btn_Exit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(56)))), ((int)(((byte)(80)))));
            this.btn_Exit.FlatAppearance.BorderSize = 0;
            this.btn_Exit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.btn_Exit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.IndianRed;
            this.btn_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Exit.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Exit.ForeColor = System.Drawing.Color.White;
            this.btn_Exit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Exit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Exit.Location = new System.Drawing.Point(678, 686);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Padding = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.btn_Exit.Size = new System.Drawing.Size(150, 70);
            this.btn_Exit.TabIndex = 258;
            this.btn_Exit.Tag = "";
            this.btn_Exit.Text = "EXIT";
            this.btn_Exit.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btn_Exit.UseVisualStyleBackColor = false;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.ForestGreen;
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGreen;
            this.btn_Save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LimeGreen;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(12, 686);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Padding = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.btn_Save.Size = new System.Drawing.Size(150, 70);
            this.btn_Save.TabIndex = 257;
            this.btn_Save.Tag = "";
            this.btn_Save.Text = "SAVE";
            this.btn_Save.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Page_1
            // 
            this.btn_Page_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Page_1.FlatAppearance.BorderSize = 0;
            this.btn_Page_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Page_1.Font = new System.Drawing.Font("Arial Black", 10F);
            this.btn_Page_1.ForeColor = System.Drawing.Color.White;
            this.btn_Page_1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Page_1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Page_1.Location = new System.Drawing.Point(144, 634);
            this.btn_Page_1.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.btn_Page_1.Name = "btn_Page_1";
            this.btn_Page_1.Padding = new System.Windows.Forms.Padding(10, 5, 5, 0);
            this.btn_Page_1.Size = new System.Drawing.Size(120, 40);
            this.btn_Page_1.TabIndex = 354;
            this.btn_Page_1.Tag = "0";
            this.btn_Page_1.Text = "1 ~ 100";
            this.btn_Page_1.UseVisualStyleBackColor = false;
            this.btn_Page_1.Click += new System.EventHandler(this.btn_Page_Click);
            // 
            // btn_Page_101
            // 
            this.btn_Page_101.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Page_101.FlatAppearance.BorderSize = 0;
            this.btn_Page_101.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Page_101.Font = new System.Drawing.Font("Arial Black", 10F);
            this.btn_Page_101.ForeColor = System.Drawing.Color.White;
            this.btn_Page_101.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Page_101.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Page_101.Location = new System.Drawing.Point(287, 634);
            this.btn_Page_101.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.btn_Page_101.Name = "btn_Page_101";
            this.btn_Page_101.Padding = new System.Windows.Forms.Padding(10, 5, 5, 0);
            this.btn_Page_101.Size = new System.Drawing.Size(120, 40);
            this.btn_Page_101.TabIndex = 355;
            this.btn_Page_101.Tag = "100";
            this.btn_Page_101.Text = "101 ~ 200";
            this.btn_Page_101.UseVisualStyleBackColor = false;
            this.btn_Page_101.Click += new System.EventHandler(this.btn_Page_Click);
            // 
            // btn_Page_201
            // 
            this.btn_Page_201.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Page_201.FlatAppearance.BorderSize = 0;
            this.btn_Page_201.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Page_201.Font = new System.Drawing.Font("Arial Black", 10F);
            this.btn_Page_201.ForeColor = System.Drawing.Color.White;
            this.btn_Page_201.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Page_201.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Page_201.Location = new System.Drawing.Point(430, 634);
            this.btn_Page_201.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.btn_Page_201.Name = "btn_Page_201";
            this.btn_Page_201.Padding = new System.Windows.Forms.Padding(10, 5, 5, 0);
            this.btn_Page_201.Size = new System.Drawing.Size(120, 40);
            this.btn_Page_201.TabIndex = 356;
            this.btn_Page_201.Tag = "200";
            this.btn_Page_201.Text = "201 ~ 300";
            this.btn_Page_201.UseVisualStyleBackColor = false;
            this.btn_Page_201.Click += new System.EventHandler(this.btn_Page_Click);
            // 
            // btn_Page_301
            // 
            this.btn_Page_301.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btn_Page_301.FlatAppearance.BorderSize = 0;
            this.btn_Page_301.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Page_301.Font = new System.Drawing.Font("Arial Black", 10F);
            this.btn_Page_301.ForeColor = System.Drawing.Color.White;
            this.btn_Page_301.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Page_301.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Page_301.Location = new System.Drawing.Point(573, 634);
            this.btn_Page_301.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.btn_Page_301.Name = "btn_Page_301";
            this.btn_Page_301.Padding = new System.Windows.Forms.Padding(10, 5, 5, 0);
            this.btn_Page_301.Size = new System.Drawing.Size(120, 40);
            this.btn_Page_301.TabIndex = 357;
            this.btn_Page_301.Tag = "300";
            this.btn_Page_301.Text = "301 ~";
            this.btn_Page_301.UseVisualStyleBackColor = false;
            this.btn_Page_301.Click += new System.EventHandler(this.btn_Page_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.Yellow;
            this.btn_Cancel.FlatAppearance.BorderSize = 0;
            this.btn_Cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btn_Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Cancel.ForeColor = System.Drawing.Color.Black;
            this.btn_Cancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(234, 686);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Padding = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.btn_Cancel.Size = new System.Drawing.Size(150, 70);
            this.btn_Cancel.TabIndex = 358;
            this.btn_Cancel.Tag = "";
            this.btn_Cancel.Text = "CANCEL";
            this.btn_Cancel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // vwSPC_Radar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(839, 761);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Page_301);
            this.Controls.Add(this.btn_Page_201);
            this.Controls.Add(this.btn_Page_101);
            this.Controls.Add(this.btn_Page_1);
            this.Controls.Add(this.btn_ResetCount);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.dgv_RadarList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "vwSPC_Radar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Radar Error List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.vwSPC_Radar_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_RadarList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_RadarList;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Code;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_RadarFunction;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Content;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_SDCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_CDCount;
        private System.Windows.Forms.Button btn_ResetCount;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Page_1;
        private System.Windows.Forms.Button btn_Page_101;
        private System.Windows.Forms.Button btn_Page_201;
        private System.Windows.Forms.Button btn_Page_301;
        private System.Windows.Forms.Button btn_Cancel;
    }
}