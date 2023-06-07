using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SG2000X
{

    public partial class vmMan_13QcVision : UserControl
    {
        public static vmMan_13QcVision m_qcVision;
        private delegate void SafeCallDelegate(string text);

        public vmMan_13QcVision()
        {
            InitializeComponent();
            m_qcVision = this;
            CGxSocket.It.sIPAddress = CData.Opt.sQcServerIp;
            CGxSocket.It.nPortNo = Int32.Parse(CData.Opt.sQcServerPort);
        }

        static void doAction()
        {

        }

        private void connection_Click(object sender, EventArgs e)
        {
            if (!CGxSocket.It.IsConnected())
            {
                CGxSocket.It.Connect();
                addMsgList("Try CGxSocket.It.Connect();");
                addMsgList(string.Format("IP:{0} Port:{1}", CGxSocket.It.sIPAddress, CGxSocket.It.nPortNo));
            }
            else
            {
                addMsgList("Already Connected !!!");
            }
               
        }

        public void addMsgList(string sMsg)
        {
            if (!CData.Opt.bQcSimulation) return;

            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke((MethodInvoker)(() =>
                {
                    listBox1.Items.Add(sMsg);
                    if (listBox1.Items.Count > 35) listBox1.Items.RemoveAt(0);     
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;             
                }));
            }
            else
            {
                if (listBox1.Items.Count > 35)
                {
                    listBox1.Items.RemoveAt(0);
                }
                int nCnt = listBox1.Items.Add(sMsg);
            }
        }
        public void Open()
        {

        }
        public void Close()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("EQReadyQuery");
        }

        private void btnEQSendOut2_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("EQSendOut,Bypass," + txtEQSend.Text);
        }

        private void EQSendEnd_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("EQSendEnd");
        }

        private void btnEQAbort_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("EQAbortTransfer");
        }

        private void btnQCSendRequest_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("QCSendRequest");
        }

        private void btnAutoRun_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("AutoRun");
        }

        private void btnAutoStop_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("AutoStop");
        }

        private void btnErrorReset_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("ErrorReset");
        }

        private void btnLotEnd_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("LotEnd");
        }

        private void btnDoorLockOn_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("DoorLock,On");
        }

        private void btnDoorLockOff_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("DoorLock,Off");
        }

        private void btnEmgStop_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("EmgStop");
        }

        private void StatusQuery_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("StatusQuery");
        }

        private void btnResultQuery_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("ResultQuery");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (CGxSocket.It.IsConnected())
            {
                CGxSocket.It.CloseClient();
                addMsgList("Try CGxSocket.It.CloseClient();");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Run;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Out;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CGxSocket.It.SendMessage("EQAbortTransfer,ACK");
        }

        private void buttonStartSim_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            buttonStartSim.Enabled = false;
            CSQ7_Dry_SIM.It.startAutoRun();
        }

        private void buttonStripOut_Click(object sender, EventArgs e)
        {
            if (!CData.Opt.bQcSimulation) return;
            CSQ7_Dry_SIM.It.Func_AbnormalStripOUt();
        }
    }
}
