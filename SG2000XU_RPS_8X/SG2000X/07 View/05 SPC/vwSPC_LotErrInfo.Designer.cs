namespace SG2000X
{
    partial class vwSPC_LotErrInfo
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_LotErrorList = new System.Windows.Forms.DataGridView();
            this.col_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ErrMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.sfd_Chart = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_LotErrorList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_LotErrorList
            // 
            this.dgv_LotErrorList.AllowUserToAddRows = false;
            this.dgv_LotErrorList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_LotErrorList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_LotErrorList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_LotErrorList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Code,
            this.col_ErrMsg,
            this.col_Count,
            this.col_time});
            this.dgv_LotErrorList.Location = new System.Drawing.Point(9, 9);
            this.dgv_LotErrorList.MultiSelect = false;
            this.dgv_LotErrorList.Name = "dgv_LotErrorList";
            this.dgv_LotErrorList.RowHeadersVisible = false;
            this.dgv_LotErrorList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_LotErrorList.RowTemplate.Height = 23;
            this.dgv_LotErrorList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_LotErrorList.Size = new System.Drawing.Size(669, 613);
            this.dgv_LotErrorList.TabIndex = 257;
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
            // col_ErrMsg
            // 
            this.col_ErrMsg.HeaderText = "Err Message";
            this.col_ErrMsg.Name = "col_ErrMsg";
            this.col_ErrMsg.ReadOnly = true;
            this.col_ErrMsg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_ErrMsg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.col_ErrMsg.Width = 450;
            // 
            // col_Count
            // 
            this.col_Count.HeaderText = "Count";
            this.col_Count.Name = "col_Count";
            this.col_Count.ReadOnly = true;
            this.col_Count.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Count.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.col_Count.Width = 50;
            // 
            // col_time
            // 
            this.col_time.HeaderText = "Time";
            this.col_time.Name = "col_time";
            this.col_time.ReadOnly = true;
            this.col_time.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
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
            this.btn_Exit.Location = new System.Drawing.Point(528, 679);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Padding = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.btn_Exit.Size = new System.Drawing.Size(150, 70);
            this.btn_Exit.TabIndex = 259;
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
            this.btn_Save.Location = new System.Drawing.Point(9, 679);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Padding = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.btn_Save.Size = new System.Drawing.Size(150, 70);
            this.btn_Save.TabIndex = 260;
            this.btn_Save.Tag = "";
            this.btn_Save.Text = "Lot Info\r\nSAVE";
            this.btn_Save.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // sfd_Chart
            // 
            this.sfd_Chart.Filter = "CSV File|*.csv";
            this.sfd_Chart.Title = "Save a CSV File";
            // 
            // vwSPC_LotErrInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 761);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.dgv_LotErrorList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "vwSPC_LotErrInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LOT INFO ERROR";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_LotErrorList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_LotErrorList;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ErrMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Count;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_time;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.SaveFileDialog sfd_Chart;
    }
}