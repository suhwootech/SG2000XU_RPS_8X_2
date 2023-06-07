namespace SG2000X
{
    partial class vmMan_13QcVision
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
            this.connection = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtEQSend = new System.Windows.Forms.TextBox();
            this.btnResultQuery = new System.Windows.Forms.Button();
            this.StatusQuery = new System.Windows.Forms.Button();
            this.btnEmgStop = new System.Windows.Forms.Button();
            this.btnDoorLockOff = new System.Windows.Forms.Button();
            this.btnDoorLockOn = new System.Windows.Forms.Button();
            this.btnLotEnd = new System.Windows.Forms.Button();
            this.btnErrorReset = new System.Windows.Forms.Button();
            this.btnAutoStop = new System.Windows.Forms.Button();
            this.btnAutoRun = new System.Windows.Forms.Button();
            this.btnQCSendRequest = new System.Windows.Forms.Button();
            this.btnEQAbort = new System.Windows.Forms.Button();
            this.EQSendEnd = new System.Windows.Forms.Button();
            this.btnEQSendOut2 = new System.Windows.Forms.Button();
            this.btnEQSendOut1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.buttonStartSim = new System.Windows.Forms.Button();
            this.buttonStripOut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connection
            // 
            this.connection.Location = new System.Drawing.Point(36, 36);
            this.connection.Name = "connection";
            this.connection.Size = new System.Drawing.Size(125, 42);
            this.connection.TabIndex = 0;
            this.connection.Text = "connection";
            this.connection.UseVisualStyleBackColor = true;
            this.connection.Click += new System.EventHandler(this.connection_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(469, 22);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(404, 580);
            this.listBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(22, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 44);
            this.button1.TabIndex = 2;
            this.button1.Text = "EQReadyQuery";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtEQSend
            // 
            this.txtEQSend.Location = new System.Drawing.Point(22, 159);
            this.txtEQSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtEQSend.Name = "txtEQSend";
            this.txtEQSend.Size = new System.Drawing.Size(158, 22);
            this.txtEQSend.TabIndex = 22;
            this.txtEQSend.Text = "Lot1,Mgz1,Slot1,SlotOK,Group1,Deive1,Strip1";
            // 
            // btnResultQuery
            // 
            this.btnResultQuery.Location = new System.Drawing.Point(199, 427);
            this.btnResultQuery.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnResultQuery.Name = "btnResultQuery";
            this.btnResultQuery.Size = new System.Drawing.Size(109, 31);
            this.btnResultQuery.TabIndex = 37;
            this.btnResultQuery.Text = "ResultQuery";
            this.btnResultQuery.UseVisualStyleBackColor = true;
            this.btnResultQuery.Click += new System.EventHandler(this.btnResultQuery_Click);
            // 
            // StatusQuery
            // 
            this.StatusQuery.Location = new System.Drawing.Point(200, 387);
            this.StatusQuery.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StatusQuery.Name = "StatusQuery";
            this.StatusQuery.Size = new System.Drawing.Size(109, 31);
            this.StatusQuery.TabIndex = 36;
            this.StatusQuery.Text = "StatusQuery";
            this.StatusQuery.UseVisualStyleBackColor = true;
            this.StatusQuery.Click += new System.EventHandler(this.StatusQuery_Click);
            // 
            // btnEmgStop
            // 
            this.btnEmgStop.Location = new System.Drawing.Point(200, 348);
            this.btnEmgStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEmgStop.Name = "btnEmgStop";
            this.btnEmgStop.Size = new System.Drawing.Size(109, 31);
            this.btnEmgStop.TabIndex = 35;
            this.btnEmgStop.Text = "EmgStop";
            this.btnEmgStop.UseVisualStyleBackColor = true;
            this.btnEmgStop.Click += new System.EventHandler(this.btnEmgStop_Click);
            // 
            // btnDoorLockOff
            // 
            this.btnDoorLockOff.Location = new System.Drawing.Point(200, 310);
            this.btnDoorLockOff.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDoorLockOff.Name = "btnDoorLockOff";
            this.btnDoorLockOff.Size = new System.Drawing.Size(109, 31);
            this.btnDoorLockOff.TabIndex = 34;
            this.btnDoorLockOff.Text = "DoorLock Off";
            this.btnDoorLockOff.UseVisualStyleBackColor = true;
            this.btnDoorLockOff.Click += new System.EventHandler(this.btnDoorLockOff_Click);
            // 
            // btnDoorLockOn
            // 
            this.btnDoorLockOn.Location = new System.Drawing.Point(200, 267);
            this.btnDoorLockOn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDoorLockOn.Name = "btnDoorLockOn";
            this.btnDoorLockOn.Size = new System.Drawing.Size(109, 31);
            this.btnDoorLockOn.TabIndex = 33;
            this.btnDoorLockOn.Text = "DoorLock On";
            this.btnDoorLockOn.UseVisualStyleBackColor = true;
            this.btnDoorLockOn.Click += new System.EventHandler(this.btnDoorLockOn_Click);
            // 
            // btnLotEnd
            // 
            this.btnLotEnd.Location = new System.Drawing.Point(200, 228);
            this.btnLotEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLotEnd.Name = "btnLotEnd";
            this.btnLotEnd.Size = new System.Drawing.Size(109, 31);
            this.btnLotEnd.TabIndex = 32;
            this.btnLotEnd.Text = "LotEnd";
            this.btnLotEnd.UseVisualStyleBackColor = true;
            this.btnLotEnd.Click += new System.EventHandler(this.btnLotEnd_Click);
            // 
            // btnErrorReset
            // 
            this.btnErrorReset.Location = new System.Drawing.Point(200, 190);
            this.btnErrorReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnErrorReset.Name = "btnErrorReset";
            this.btnErrorReset.Size = new System.Drawing.Size(109, 31);
            this.btnErrorReset.TabIndex = 31;
            this.btnErrorReset.Text = "ErrorReset";
            this.btnErrorReset.UseVisualStyleBackColor = true;
            this.btnErrorReset.Click += new System.EventHandler(this.btnErrorReset_Click);
            // 
            // btnAutoStop
            // 
            this.btnAutoStop.Location = new System.Drawing.Point(200, 151);
            this.btnAutoStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoStop.Name = "btnAutoStop";
            this.btnAutoStop.Size = new System.Drawing.Size(109, 31);
            this.btnAutoStop.TabIndex = 30;
            this.btnAutoStop.Text = "AutoStop";
            this.btnAutoStop.UseVisualStyleBackColor = true;
            this.btnAutoStop.Click += new System.EventHandler(this.btnAutoStop_Click);
            // 
            // btnAutoRun
            // 
            this.btnAutoRun.Location = new System.Drawing.Point(200, 112);
            this.btnAutoRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoRun.Name = "btnAutoRun";
            this.btnAutoRun.Size = new System.Drawing.Size(109, 31);
            this.btnAutoRun.TabIndex = 29;
            this.btnAutoRun.Text = "AutoRun";
            this.btnAutoRun.UseVisualStyleBackColor = true;
            this.btnAutoRun.Click += new System.EventHandler(this.btnAutoRun_Click);
            // 
            // btnQCSendRequest
            // 
            this.btnQCSendRequest.Location = new System.Drawing.Point(22, 322);
            this.btnQCSendRequest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnQCSendRequest.Name = "btnQCSendRequest";
            this.btnQCSendRequest.Size = new System.Drawing.Size(161, 31);
            this.btnQCSendRequest.TabIndex = 28;
            this.btnQCSendRequest.Text = "QCSendRequest";
            this.btnQCSendRequest.UseVisualStyleBackColor = true;
            this.btnQCSendRequest.Click += new System.EventHandler(this.btnQCSendRequest_Click);
            // 
            // btnEQAbort
            // 
            this.btnEQAbort.Location = new System.Drawing.Point(19, 282);
            this.btnEQAbort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEQAbort.Name = "btnEQAbort";
            this.btnEQAbort.Size = new System.Drawing.Size(161, 31);
            this.btnEQAbort.TabIndex = 27;
            this.btnEQAbort.Text = "EQAbortTransfer";
            this.btnEQAbort.UseVisualStyleBackColor = true;
            this.btnEQAbort.Click += new System.EventHandler(this.btnEQAbort_Click);
            // 
            // EQSendEnd
            // 
            this.EQSendEnd.Location = new System.Drawing.Point(19, 244);
            this.EQSendEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.EQSendEnd.Name = "EQSendEnd";
            this.EQSendEnd.Size = new System.Drawing.Size(161, 31);
            this.EQSendEnd.TabIndex = 26;
            this.EQSendEnd.Text = "EQSendEnd";
            this.EQSendEnd.UseVisualStyleBackColor = true;
            this.EQSendEnd.Click += new System.EventHandler(this.EQSendEnd_Click);
            // 
            // btnEQSendOut2
            // 
            this.btnEQSendOut2.Location = new System.Drawing.Point(22, 200);
            this.btnEQSendOut2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEQSendOut2.Name = "btnEQSendOut2";
            this.btnEQSendOut2.Size = new System.Drawing.Size(161, 31);
            this.btnEQSendOut2.TabIndex = 25;
            this.btnEQSendOut2.Text = "SendOut,Test";
            this.btnEQSendOut2.UseVisualStyleBackColor = true;
            this.btnEQSendOut2.Click += new System.EventHandler(this.btnEQSendOut2_Click);
            // 
            // btnEQSendOut1
            // 
            this.btnEQSendOut1.Location = new System.Drawing.Point(192, -60);
            this.btnEQSendOut1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEQSendOut1.Name = "btnEQSendOut1";
            this.btnEQSendOut1.Size = new System.Drawing.Size(161, 31);
            this.btnEQSendOut1.TabIndex = 24;
            this.btnEQSendOut1.Text = "SendOut,Bypass";
            this.btnEQSendOut1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(196, 39);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(142, 38);
            this.button2.TabIndex = 38;
            this.button2.Text = "DisConnection";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(337, 112);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(126, 44);
            this.button3.TabIndex = 39;
            this.button3.Text = "Dry RUN";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(334, 177);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(128, 43);
            this.button4.TabIndex = 40;
            this.button4.Text = "Dry Out";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(334, 265);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(127, 47);
            this.button5.TabIndex = 41;
            this.button5.Text = "EQ Abort Tr";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // buttonStartSim
            // 
            this.buttonStartSim.Location = new System.Drawing.Point(61, 532);
            this.buttonStartSim.Name = "buttonStartSim";
            this.buttonStartSim.Size = new System.Drawing.Size(177, 69);
            this.buttonStartSim.TabIndex = 42;
            this.buttonStartSim.Text = "Start Simulation";
            this.buttonStartSim.UseVisualStyleBackColor = true;
            this.buttonStartSim.Click += new System.EventHandler(this.buttonStartSim_Click);
            // 
            // buttonStripOut
            // 
            this.buttonStripOut.Location = new System.Drawing.Point(313, 532);
            this.buttonStripOut.Name = "buttonStripOut";
            this.buttonStripOut.Size = new System.Drawing.Size(120, 60);
            this.buttonStripOut.TabIndex = 43;
            this.buttonStripOut.Text = "Strip Out";
            this.buttonStripOut.UseVisualStyleBackColor = true;
            this.buttonStripOut.Click += new System.EventHandler(this.buttonStripOut_Click);
            // 
            // vmMan_13QcVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonStripOut);
            this.Controls.Add(this.buttonStartSim);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnResultQuery);
            this.Controls.Add(this.StatusQuery);
            this.Controls.Add(this.btnEmgStop);
            this.Controls.Add(this.btnDoorLockOff);
            this.Controls.Add(this.btnDoorLockOn);
            this.Controls.Add(this.btnLotEnd);
            this.Controls.Add(this.btnErrorReset);
            this.Controls.Add(this.btnAutoStop);
            this.Controls.Add(this.btnAutoRun);
            this.Controls.Add(this.btnQCSendRequest);
            this.Controls.Add(this.btnEQAbort);
            this.Controls.Add(this.EQSendEnd);
            this.Controls.Add(this.btnEQSendOut2);
            this.Controls.Add(this.btnEQSendOut1);
            this.Controls.Add(this.txtEQSend);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.connection);
            this.Name = "vmMan_13QcVision";
            this.Size = new System.Drawing.Size(898, 630);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connection;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtEQSend;
        private System.Windows.Forms.Button btnResultQuery;
        private System.Windows.Forms.Button StatusQuery;
        private System.Windows.Forms.Button btnEmgStop;
        private System.Windows.Forms.Button btnDoorLockOff;
        private System.Windows.Forms.Button btnDoorLockOn;
        private System.Windows.Forms.Button btnLotEnd;
        private System.Windows.Forms.Button btnErrorReset;
        private System.Windows.Forms.Button btnAutoStop;
        private System.Windows.Forms.Button btnAutoRun;
        private System.Windows.Forms.Button btnQCSendRequest;
        private System.Windows.Forms.Button btnEQAbort;
        private System.Windows.Forms.Button EQSendEnd;
        private System.Windows.Forms.Button btnEQSendOut2;
        private System.Windows.Forms.Button btnEQSendOut1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button buttonStartSim;
        private System.Windows.Forms.Button buttonStripOut;
    }
}
