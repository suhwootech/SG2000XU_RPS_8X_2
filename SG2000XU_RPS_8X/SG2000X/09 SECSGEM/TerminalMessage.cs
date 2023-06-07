using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SECSGEM;
namespace SG2000X
{
    public partial class frmTM : Form
    {
        frmGEM sgfrmGEM;

        /*
        public frmTM()
        {
            InitializeComponent();
            //sgfrmGEM = _frmGEM;
        }
        */

        public frmTM(frmGEM _frmGEM)
        {
            InitializeComponent();
            sgfrmGEM = _frmGEM;
            Update_Timer.Start();
            if (CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) // 200625 lks
            {
                this.btn_Local.Enabled = false;
            }
            List_Title(1);
        }
        private void List_Title(int nCount)
        {
            grdDataView.ColumnCount = 3;
            grdDataView.RowCount = nCount;
            string[] stitle = new string[3];

            stitle[0] = "No";
            stitle[1] = "Carrier ID";
            stitle[2] = "Status";
            
            grdDataView[0, 0].Value = stitle[0];
            grdDataView[1, 0].Value = stitle[1];
            grdDataView[2, 0].Value = stitle[2];


        }

        #region button event
        private void btnHide_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            this.Hide();
        }
        #endregion button event

        public void ShowGrid(SECSGEM.ManagerLot pMLOT )
        {
            int iCnt = 0;
            if (grdDataView.InvokeRequired )
            {

                Invoke((MethodInvoker)delegate ()
                {
                    ShowGrid(pMLOT);
                });
            }
            else
            {
                if (pMLOT == null) return;
                // 2021.03.04 SungTae : Lot 수량이 0으로 Grid를 그릴 때 Hang-up 발생하여 조건 추가(LOCAL MODE일 경우)
                if ((int)pMLOT.Count <= 0) return;

                string[] stitle = new string[3];
                iCnt = (int)pMLOT.Count;
                
                grdDataView.RowCount = iCnt + 1;

                //iCnt = 10;

                for (int i = 0; i < iCnt; i++)
                {   
                    int iNumber = i + 1;
                    int iStatus = 0;
                    string[] sdata = new string[3];

                    sdata[0] = iNumber.ToString();
                    sdata[1] = pMLOT.GetCarrierID(i);
                    iStatus  = pMLOT.GetCarrierStatus(i);

                    if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Idle)
                    {
                        sdata[2] = "Not read";
                        grdDataView[2, iNumber].Style.BackColor = Color.Gray;
                    }
                    else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Read)
                    {
                        sdata[2] = "Read";
                        grdDataView[2, iNumber].Style.BackColor = Color.Yellow;
                    }
                    else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Validate)
                    {
                        sdata[2] = "Validation";
                        grdDataView[2, iNumber].Style.BackColor = Color.Blue;
                    }
                    else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Start)
                    {
                        sdata[2] = "Start";
                        grdDataView[2, iNumber].Style.BackColor = Color.Green;
                    }
                    else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.End)
                    {
                        sdata[2] = "End";
                        grdDataView[2, iNumber].Style.BackColor = Color.Aqua;
                    }
                    else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Reject)
                    {
                        sdata[2] = "Reject";
                        grdDataView[2, iNumber].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        //----------------------------------------
                        // 존재하지 않거나 맞지 않는 Status
                    }

                    grdDataView[0, iNumber].Value = sdata[0];
                    grdDataView[1, iNumber].Value = sdata[1];
                    grdDataView[2, iNumber].Value = sdata[2];
                }
            }

            List_Title(iCnt + 1);
        }

        #region function
        public void AddString(string sMsg)
        {

            int nLineCnt = 0;
            string stime = string.Empty;
            DateTime dtNow = DateTime.Now;

            stime = dtNow.ToString("[yyyy-MM-dd HH:mm:ss fff]");
            //lstLog.Items.Add(string.Format("{0}    {1}", stime, slog));
            sMsg = string.Format("{0} {1} \n", stime, sMsg);
            
            Terminal_List.AppendText(sMsg);
            Terminal_List.ScrollToCaret();

            nLineCnt = Terminal_List.Lines.Length;

            if (nLineCnt > 100)
            {
                int cnt = nLineCnt - 101;

                for (int i = 0; i < cnt; i++)
                {
                    DeleteLine(0);
                }
            }
        }

        private void DeleteLine(int a_line)
        {
            int start_index = Terminal_List.GetFirstCharIndexFromLine(a_line);
            int count = Terminal_List.Lines[a_line].Length;

            // Eat new line chars
            if (a_line < Terminal_List.Lines.Length - 1)
            {
                count += Terminal_List.GetFirstCharIndexFromLine(a_line + 1) 
                        - ((start_index + count - 1) + 1);
            }

            Terminal_List.Text = Terminal_List.Text.Remove(start_index, count);
        }
        #endregion function

        public void Log_Display(string sMsg)
        {
            int nLineCnt = 0;
            string stime = string.Empty;
            DateTime dtNow = DateTime.Now;

            stime = dtNow.ToString("[yyyy-MM-dd HH:mm:ss fff]");
            //lstLog.Items.Add(string.Format("{0}    {1}", stime, slog));
            sMsg = string.Format("{0} {1} \n", stime,sMsg);

            Log_List.AppendText(sMsg );
            Log_List.ScrollToCaret();

            nLineCnt = Log_List.Lines.Length;

            if (nLineCnt > 100)
            {
                int cnt = nLineCnt - 101;
                for (int i = 0; i < cnt; i++)
                {
                    Log_DeleteLine(0);
                }
            }
        }

        private void Log_DeleteLine(int a_line)
        {
            int start_index = Log_List.GetFirstCharIndexFromLine(a_line);
            int count = Log_List.Lines[a_line].Length;

            // Eat new line chars
            if (a_line < Log_List.Lines.Length - 1)
            {
                count += Log_List.GetFirstCharIndexFromLine(a_line + 1) -
                    ((start_index + count - 1) + 1);
            }

            Log_List.Text = Log_List.Text.Remove(start_index, count);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            sgfrmGEM.Start_GEM();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            sgfrmGEM.Stop_GEM();
        }


        private void Update_Timer_Tick(object sender, EventArgs e)
        {

            int nmsg_Cnt = CData.SecsGem_Data.qTerminal_Msg.Count;

            for (int i = 0; i < nmsg_Cnt; i++)
            {
                AddString(CData.SecsGem_Data.qTerminal_Msg.Dequeue());
            }

            nmsg_Cnt = CData.SecsGem_Data.qTerminal_Log.Count;

            for (int i = 0; i < nmsg_Cnt; i++)
            {
                Log_Display(CData.SecsGem_Data.qTerminal_Log.Dequeue());

            }

            Button_Display();
            //ShowGrid(CData.m_pHostLOT);
        }
        
        public void Display()
        {
//            if (this == null) return;
            if (Log_List.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    //Console.WriteLine("---- a0");
                    Display();
                });
            }
            else
            {
                //Console.WriteLine("---- a1");
                Button_Display();                   // 화면에 폼을 표시해주기 전에 버튼의 속성들을 설정해 준다.
                Show();
            }        
        }

        public void Display_Hide()
        {
            if (Log_List.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    //Console.WriteLine("---- a2");
                    Display_Hide();
                });
            }
            else
            {
                //Console.WriteLine("---- a3");
                Hide();
            }        
        }

        private void Button_Display()
        {// 20200430 LCY 수정
            //lblComm_Status.Text = GEM.strA_Comm[CData.GemForm.m_iComm];
            //lblComm_Status.BackColor = GEM.clrA_WRC[CData.GemForm.m_iComm];

            btn_Disc_Conn.Text      = GEM.strA_Conn[CData.GemForm.m_iConnState];
            btn_Disc_Conn.BackColor = GEM.clrA_RC[CData.GemForm.m_iConnState];

            btn_Remot_Sts.Text      = GEM.strA_Cont[CData.GemForm.m_iCtrlState];
            btn_Remot_Sts.BackColor = GEM.clrA_RYC[CData.GemForm.m_iCtrlState];

            // Skyworks VOC
            //2021-01-18, jhLee : 작업자 실수 방지를 위하여 Hide를 제외한 모든 버튼을 감추도록 한다.
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                btnStop.Visible         = false;
                btnStart.Visible        = false;
                btn_Disc_Conn.Visible   = false;
                btn_Remot_Sts.Visible   = false;
                lblConn_Status.Visible  = false;
                btn_NotConn.Visible     = false;
                btn_Offline.Visible     = false;
                btn_Local.Visible       = false;
                btn_Remote.Visible      = false;
            }

            //lblPS_Status.Text = GEM.strA_PS[CData.GemForm.m_iPS];
            //lblPS_Status.BackColor = GEM.clrA_YGCR[CData.GemForm.m_iPS];
            /*
            if (CData.SecsGem_Data.nConnect_Flag == 1)
            {
                //Console.WriteLine("---- 0");
                this.btn_Disc_Conn.Text = string.Format("Connect Sts");
                this.btn_Disc_Conn.BackColor = Color.DarkSeaGreen;
            }
            else // 연결 되지 않음
            {
                //Console.WriteLine("---- 1");
                this.btn_Disc_Conn.Text = string.Format("Dis Connect Sts");
                this.btn_Disc_Conn.BackColor = Color.DarkRed;
            }*/

            // 2021.10.18 SungTae : [수정] 코드 파악 용이하도록 변경 (1 -> (int)JSCK.eCont.Connected)
            if (CData.SecsGem_Data.nConnect_Flag != (int)JSCK.eConn.Connected/*1*/)
            {
                btn_Offline.Text        = string.Format("Off Line Sts");
                btn_Remot_Sts.Text      = string.Format("OffLine Status");
                btn_Remot_Sts.BackColor = Color.DarkRed;

                this.btn_Remote.Enabled = false;
                this.btn_Local.Enabled  = false;
                return;
            }

            // 2021.10.18 SungTae : [수정] 코드 파악 용이하도록 변경 (4 -> (int)JSCK.eCont.Local)
            if (CData.SecsGem_Data.nRemote_Flag == (int)JSCK.eCont.Local/*4*/)// Local
            {
                //Console.WriteLine("---- 2");
                btn_Offline.Text        = string.Format("On Line Sts");
                btn_Remot_Sts.Text      = string.Format("Local Status");
                btn_Remot_Sts.BackColor = Color.Yellow;

                this.btn_Remote.Enabled = true;
                this.btn_Local.Enabled  = false;
            }
            // 2021.10.18 SungTae : [수정] 코드 파악 용이하도록 변경 (5 -> (int)JSCK.eCont.Remote)
            else if (CData.SecsGem_Data.nRemote_Flag == (int)JSCK.eCont.Remote/*5*/)// Remote
            {
                //Console.WriteLine("---- 3");
                btn_Offline.Text = string.Format("On Line Sts");
                btn_Remot_Sts.Text = string.Format("Remote Status");
                btn_Remot_Sts.BackColor = Color.DarkGreen;

                this.btn_Remote.Enabled = false;
                this.btn_Local.Enabled = true;
            }
            else
            {
                //Console.WriteLine("---- 3");
                btn_Offline.Text        = string.Format("Off Line Sts");
                btn_Remot_Sts.Text      = string.Format("OffLine Status");
                btn_Remot_Sts.BackColor = Color.DarkRed;

                this.btn_Remote.Enabled = true;
                this.btn_Local.Enabled  = true;
            }

            if (CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200625 lks
            {
                this.btn_Local.Enabled = false;
            }
        }

        private void btn_Local_Click(object sender, EventArgs e)
        {
            sgfrmGEM.OnOnlineLocal();
        }

        private void btnRemote_Click(object sender, EventArgs e)
        {
            sgfrmGEM.OnOnlineRemote();
        }
    }
}
