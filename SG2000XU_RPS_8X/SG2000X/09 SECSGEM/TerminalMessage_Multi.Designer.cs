namespace SG2000X
{
    partial class frmTM2
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.Log_List = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.Terminal_List = new System.Windows.Forms.RichTextBox();
            this.lblMsg = new System.Windows.Forms.Label();
            this.panel24 = new System.Windows.Forms.Panel();
            this.grdDataView_N = new System.Windows.Forms.DataGridView();
            this.grdDataView = new System.Windows.Forms.DataGridView();
            this.label82 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblConn_Status = new System.Windows.Forms.Label();
            this.btn_Remote = new System.Windows.Forms.Button();
            this.btn_Offline = new System.Windows.Forms.Button();
            this.btn_NotConn = new System.Windows.Forms.Button();
            this.btn_Remot_Sts = new System.Windows.Forms.Button();
            this.btn_Disc_Conn = new System.Windows.Forms.Button();
            this.btn_Local = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnHide = new System.Windows.Forms.Button();
            this.Update_Timer = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel24.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataView_N)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.Log_List);
            this.panel2.Location = new System.Drawing.Point(1, 528);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(507, 212);
            this.panel2.TabIndex = 368;
            this.panel2.Tag = "1";
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Magenta;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(1, 3);
            this.label11.Name = "label11";
            this.label11.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label11.Size = new System.Drawing.Size(212, 25);
            this.label11.TabIndex = 3;
            this.label11.Text = "SECSEM Message Log";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Log_List
            // 
            this.Log_List.BackColor = System.Drawing.SystemColors.GrayText;
            this.Log_List.Location = new System.Drawing.Point(3, 31);
            this.Log_List.Name = "Log_List";
            this.Log_List.ReadOnly = true;
            this.Log_List.Size = new System.Drawing.Size(500, 176);
            this.Log_List.TabIndex = 1;
            this.Log_List.Text = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.Terminal_List);
            this.panel1.Controls.Add(this.lblMsg);
            this.panel1.Location = new System.Drawing.Point(515, 528);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(507, 212);
            this.panel1.TabIndex = 369;
            this.panel1.Tag = "1";
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Navy;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(1, 3);
            this.label9.Name = "label9";
            this.label9.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label9.Size = new System.Drawing.Size(212, 25);
            this.label9.TabIndex = 3;
            this.label9.Text = "Terminal Message";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Terminal_List
            // 
            this.Terminal_List.BackColor = System.Drawing.SystemColors.Window;
            this.Terminal_List.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Terminal_List.ForeColor = System.Drawing.Color.Maroon;
            this.Terminal_List.Location = new System.Drawing.Point(3, 31);
            this.Terminal_List.Name = "Terminal_List";
            this.Terminal_List.ReadOnly = true;
            this.Terminal_List.Size = new System.Drawing.Size(500, 176);
            this.Terminal_List.TabIndex = 1;
            this.Terminal_List.Text = "";
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Gainsboro;
            this.lblMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMsg.Location = new System.Drawing.Point(344, 3);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(107, 25);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Visible = false;
            // 
            // panel24
            // 
            this.panel24.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel24.Controls.Add(this.grdDataView_N);
            this.panel24.Controls.Add(this.grdDataView);
            this.panel24.Controls.Add(this.label82);
            this.panel24.Location = new System.Drawing.Point(1, 1);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(1018, 521);
            this.panel24.TabIndex = 370;
            this.panel24.Tag = "1";
            // 
            // grdDataView_N
            // 
            this.grdDataView_N.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDataView_N.ColumnHeadersVisible = false;
            this.grdDataView_N.Location = new System.Drawing.Point(3, 272);
            this.grdDataView_N.Name = "grdDataView_N";
            this.grdDataView_N.RowHeadersVisible = false;
            this.grdDataView_N.RowTemplate.Height = 23;
            this.grdDataView_N.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.grdDataView_N.Size = new System.Drawing.Size(1010, 242);
            this.grdDataView_N.TabIndex = 370;
            // 
            // grdDataView
            // 
            this.grdDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDataView.ColumnHeadersVisible = false;
            this.grdDataView.Location = new System.Drawing.Point(3, 29);
            this.grdDataView.Name = "grdDataView";
            this.grdDataView.RowHeadersVisible = false;
            this.grdDataView.RowTemplate.Height = 23;
            this.grdDataView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.grdDataView.Size = new System.Drawing.Size(1010, 242);
            this.grdDataView.TabIndex = 370;
            // 
            // label82
            // 
            this.label82.BackColor = System.Drawing.Color.Gray;
            this.label82.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label82.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label82.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label82.Location = new System.Drawing.Point(1, 1);
            this.label82.Name = "label82";
            this.label82.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.label82.Size = new System.Drawing.Size(212, 25);
            this.label82.TabIndex = 3;
            this.label82.Text = "LOT Data";
            this.label82.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(802, 770);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 50);
            this.btnStop.TabIndex = 371;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // lblConn_Status
            // 
            this.lblConn_Status.BackColor = System.Drawing.Color.Tomato;
            this.lblConn_Status.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblConn_Status.Location = new System.Drawing.Point(300, 747);
            this.lblConn_Status.Name = "lblConn_Status";
            this.lblConn_Status.Size = new System.Drawing.Size(136, 21);
            this.lblConn_Status.TabIndex = 380;
            this.lblConn_Status.Text = "Disconnect";
            this.lblConn_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConn_Status.Visible = false;
            // 
            // btn_Remote
            // 
            this.btn_Remote.Location = new System.Drawing.Point(571, 799);
            this.btn_Remote.Name = "btn_Remote";
            this.btn_Remote.Size = new System.Drawing.Size(100, 25);
            this.btn_Remote.TabIndex = 372;
            this.btn_Remote.Text = "Remote";
            this.btn_Remote.UseVisualStyleBackColor = true;
            // 
            // btn_Offline
            // 
            this.btn_Offline.Location = new System.Drawing.Point(236, 793);
            this.btn_Offline.Name = "btn_Offline";
            this.btn_Offline.Size = new System.Drawing.Size(28, 19);
            this.btn_Offline.TabIndex = 373;
            this.btn_Offline.Text = "OffLine";
            this.btn_Offline.UseVisualStyleBackColor = true;
            this.btn_Offline.Visible = false;
            // 
            // btn_NotConn
            // 
            this.btn_NotConn.Location = new System.Drawing.Point(236, 768);
            this.btn_NotConn.Name = "btn_NotConn";
            this.btn_NotConn.Size = new System.Drawing.Size(31, 19);
            this.btn_NotConn.TabIndex = 374;
            this.btn_NotConn.Text = "Not Comm";
            this.btn_NotConn.UseVisualStyleBackColor = true;
            this.btn_NotConn.Visible = false;
            // 
            // btn_Remot_Sts
            // 
            this.btn_Remot_Sts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Remot_Sts.Location = new System.Drawing.Point(312, 801);
            this.btn_Remot_Sts.Name = "btn_Remot_Sts";
            this.btn_Remot_Sts.Size = new System.Drawing.Size(110, 24);
            this.btn_Remot_Sts.TabIndex = 375;
            this.btn_Remot_Sts.Text = "Off Line";
            this.btn_Remot_Sts.UseVisualStyleBackColor = true;
            // 
            // btn_Disc_Conn
            // 
            this.btn_Disc_Conn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Disc_Conn.Location = new System.Drawing.Point(312, 771);
            this.btn_Disc_Conn.Name = "btn_Disc_Conn";
            this.btn_Disc_Conn.Size = new System.Drawing.Size(110, 24);
            this.btn_Disc_Conn.TabIndex = 376;
            this.btn_Disc_Conn.Text = "DisConn";
            this.btn_Disc_Conn.UseVisualStyleBackColor = true;
            // 
            // btn_Local
            // 
            this.btn_Local.Location = new System.Drawing.Point(571, 770);
            this.btn_Local.Name = "btn_Local";
            this.btn_Local.Size = new System.Drawing.Size(100, 25);
            this.btn_Local.TabIndex = 377;
            this.btn_Local.Text = "Local";
            this.btn_Local.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(696, 771);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 50);
            this.btnStart.TabIndex = 378;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(908, 770);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(100, 50);
            this.btnHide.TabIndex = 379;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            // 
            // Update_Timer
            // 
            this.Update_Timer.Interval = 500;
            // 
            // frmTM2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1024, 836);
            this.ControlBox = false;
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblConn_Status);
            this.Controls.Add(this.btn_Remote);
            this.Controls.Add(this.btn_Offline);
            this.Controls.Add(this.btn_NotConn);
            this.Controls.Add(this.btn_Remot_Sts);
            this.Controls.Add(this.btn_Disc_Conn);
            this.Controls.Add(this.btn_Local);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnHide);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmTM2";
            this.Text = "TerminalMessage_Multi";
            this.TopMost = true;
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel24.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDataView_N)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RichTextBox Log_List;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox Terminal_List;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.DataGridView grdDataView_N;
        private System.Windows.Forms.DataGridView grdDataView;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblConn_Status;
        private System.Windows.Forms.Button btn_Remote;
        private System.Windows.Forms.Button btn_Offline;
        private System.Windows.Forms.Button btn_NotConn;
        private System.Windows.Forms.Button btn_Remot_Sts;
        private System.Windows.Forms.Button btn_Disc_Conn;
        private System.Windows.Forms.Button btn_Local;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Timer Update_Timer;
    }
}