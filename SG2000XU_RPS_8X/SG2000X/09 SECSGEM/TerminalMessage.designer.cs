namespace SG2000X
{
    partial class frmTM
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
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label82 = new System.Windows.Forms.Label();
            this.panel24 = new System.Windows.Forms.Panel();
            this.grdDataView = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Terminal_List = new System.Windows.Forms.RichTextBox();
            this.lblConn_Status = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.Log_List = new System.Windows.Forms.RichTextBox();
            this.Update_Timer = new System.Windows.Forms.Timer(this.components);
            this.btn_Local = new System.Windows.Forms.Button();
            this.btn_Remote = new System.Windows.Forms.Button();
            this.btn_Disc_Conn = new System.Windows.Forms.Button();
            this.btn_Offline = new System.Windows.Forms.Button();
            this.btn_NotConn = new System.Windows.Forms.Button();
            this.btn_Remot_Sts = new System.Windows.Forms.Button();
            this.panel24.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Gainsboro;
            this.lblMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMsg.Location = new System.Drawing.Point(544, 3);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(107, 25);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Visible = false;
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(686, 749);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(100, 50);
            this.btnHide.TabIndex = 1;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(474, 750);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 50);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(580, 749);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 50);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
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
            // panel24
            // 
            this.panel24.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel24.Controls.Add(this.grdDataView);
            this.panel24.Controls.Add(this.label82);
            this.panel24.Location = new System.Drawing.Point(10, 5);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(802, 278);
            this.panel24.TabIndex = 367;
            this.panel24.Tag = "1";
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
            this.grdDataView.Size = new System.Drawing.Size(794, 242);
            this.grdDataView.TabIndex = 370;
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.Terminal_List);
            this.panel1.Controls.Add(this.lblMsg);
            this.panel1.Location = new System.Drawing.Point(10, 506);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(802, 212);
            this.panel1.TabIndex = 367;
            this.panel1.Tag = "1";
            // 
            // Terminal_List
            // 
            this.Terminal_List.BackColor = System.Drawing.SystemColors.Window;
            this.Terminal_List.Font = new System.Drawing.Font("Gulim", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Terminal_List.ForeColor = System.Drawing.Color.Maroon;
            this.Terminal_List.Location = new System.Drawing.Point(3, 31);
            this.Terminal_List.Name = "Terminal_List";
            this.Terminal_List.ReadOnly = true;
            this.Terminal_List.Size = new System.Drawing.Size(794, 176);
            this.Terminal_List.TabIndex = 1;
            this.Terminal_List.Text = "";
            // 
            // lblConn_Status
            // 
            this.lblConn_Status.BackColor = System.Drawing.Color.Tomato;
            this.lblConn_Status.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblConn_Status.Location = new System.Drawing.Point(78, 726);
            this.lblConn_Status.Name = "lblConn_Status";
            this.lblConn_Status.Size = new System.Drawing.Size(136, 21);
            this.lblConn_Status.TabIndex = 369;
            this.lblConn_Status.Text = "Disconnect";
            this.lblConn_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConn_Status.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.Log_List);
            this.panel2.Location = new System.Drawing.Point(10, 288);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(802, 212);
            this.panel2.TabIndex = 367;
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
            this.Log_List.Size = new System.Drawing.Size(794, 176);
            this.Log_List.TabIndex = 1;
            this.Log_List.Text = "";
            // 
            // Update_Timer
            // 
            this.Update_Timer.Interval = 500;
            this.Update_Timer.Tick += new System.EventHandler(this.Update_Timer_Tick);
            // 
            // btn_Local
            // 
            this.btn_Local.Location = new System.Drawing.Point(349, 749);
            this.btn_Local.Name = "btn_Local";
            this.btn_Local.Size = new System.Drawing.Size(100, 25);
            this.btn_Local.TabIndex = 1;
            this.btn_Local.Text = "Local";
            this.btn_Local.UseVisualStyleBackColor = true;
            this.btn_Local.Click += new System.EventHandler(this.btn_Local_Click);
            // 
            // btn_Remote
            // 
            this.btn_Remote.Location = new System.Drawing.Point(349, 778);
            this.btn_Remote.Name = "btn_Remote";
            this.btn_Remote.Size = new System.Drawing.Size(100, 25);
            this.btn_Remote.TabIndex = 1;
            this.btn_Remote.Text = "Remote";
            this.btn_Remote.UseVisualStyleBackColor = true;
            this.btn_Remote.Click += new System.EventHandler(this.btnRemote_Click);
            // 
            // btn_Disc_Conn
            // 
            this.btn_Disc_Conn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Disc_Conn.Location = new System.Drawing.Point(90, 750);
            this.btn_Disc_Conn.Name = "btn_Disc_Conn";
            this.btn_Disc_Conn.Size = new System.Drawing.Size(110, 24);
            this.btn_Disc_Conn.TabIndex = 1;
            this.btn_Disc_Conn.Text = "DisConn";
            this.btn_Disc_Conn.UseVisualStyleBackColor = true;
            this.btn_Disc_Conn.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btn_Offline
            // 
            this.btn_Offline.Location = new System.Drawing.Point(14, 772);
            this.btn_Offline.Name = "btn_Offline";
            this.btn_Offline.Size = new System.Drawing.Size(28, 19);
            this.btn_Offline.TabIndex = 1;
            this.btn_Offline.Text = "OffLine";
            this.btn_Offline.UseVisualStyleBackColor = true;
            this.btn_Offline.Visible = false;
            this.btn_Offline.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btn_NotConn
            // 
            this.btn_NotConn.Location = new System.Drawing.Point(14, 747);
            this.btn_NotConn.Name = "btn_NotConn";
            this.btn_NotConn.Size = new System.Drawing.Size(31, 19);
            this.btn_NotConn.TabIndex = 1;
            this.btn_NotConn.Text = "Not Comm";
            this.btn_NotConn.UseVisualStyleBackColor = true;
            this.btn_NotConn.Visible = false;
            this.btn_NotConn.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btn_Remot_Sts
            // 
            this.btn_Remot_Sts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Remot_Sts.Location = new System.Drawing.Point(90, 780);
            this.btn_Remot_Sts.Name = "btn_Remot_Sts";
            this.btn_Remot_Sts.Size = new System.Drawing.Size(110, 24);
            this.btn_Remot_Sts.TabIndex = 1;
            this.btn_Remot_Sts.Text = "Off Line";
            this.btn_Remot_Sts.UseVisualStyleBackColor = true;
            this.btn_Remot_Sts.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // frmTM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(824, 836);
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
            this.Name = "frmTM";
            this.Text = "TerminalMessage";
            this.TopMost = true;
            this.panel24.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDataView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox Terminal_List;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RichTextBox Log_List;
        private System.Windows.Forms.Label lblConn_Status;
        private System.Windows.Forms.DataGridView grdDataView;
        private System.Windows.Forms.Timer Update_Timer;
        private System.Windows.Forms.Button btn_Local;
        private System.Windows.Forms.Button btn_Remote;
        private System.Windows.Forms.Button btn_Disc_Conn;
        private System.Windows.Forms.Button btn_Offline;
        private System.Windows.Forms.Button btn_NotConn;
        private System.Windows.Forms.Button btn_Remot_Sts;
    }
}