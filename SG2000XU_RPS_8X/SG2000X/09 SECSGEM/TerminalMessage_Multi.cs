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
    public partial class frmTM2 : Form
    {
        frmGEM sgfrmGEM;

        //public TerminalMessage_Multi()
        //{
        //    InitializeComponent();
        //}

        public frmTM2(frmGEM _frmGEM)
        {
            InitializeComponent();
            sgfrmGEM = _frmGEM;
            Update_Timer.Start();
            if (CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) // 200625 lks
            {
                this.btn_Local.Enabled = false;
            }

            // 2021.10.18 SungTae Start : [수정] Multi-LOT 관련
            //List_Title(1);
            List_Title(1, 1);   // Current LOT
            List_Title(2, 1);   // Next LOT
            // 2021.10.18 SungTae End
        }

        private void List_Title(int nLotCnt, int nCount)
        {
            string[] stitle = new string[4];

            stitle[0] = "No";
            stitle[1] = "LOT ID";
            stitle[2] = "Carrier ID";
            stitle[3] = "Status";

            if (nLotCnt == 1)
            {
                grdDataView.ColumnCount = 4;
                grdDataView.RowCount = nCount;

                grdDataView[0, 0].Value = stitle[0];
                grdDataView[1, 0].Value = stitle[1];
                grdDataView[2, 0].Value = stitle[2];
                grdDataView[3, 0].Value = stitle[3];
            }

            if(nLotCnt == 2)
            {
                grdDataView_N.ColumnCount = 4;
                grdDataView_N.RowCount = nCount;

                grdDataView_N[0, 0].Value = stitle[0];
                grdDataView_N[1, 0].Value = stitle[1];
                grdDataView_N[2, 0].Value = stitle[2];
                grdDataView_N[3, 0].Value = stitle[3];
            }
        }

        #region button event
        private void btnHide_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            this.Hide();
        }
        #endregion button event

        public void ShowGrid(SECSGEM.ManagerLot pMLOT, string sLotName = "", int nLotCnt = 0)
        {
            int iCnt = 0;
            int i;

            if (grdDataView.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    ShowGrid(pMLOT);
                });
            }
            else
            {
                if (pMLOT == null || (int)pMLOT.Count <= 0 || nLotCnt == 0)
                {
                    return;
                }

                string[] stitle = new string[4];
                string[] sdata = new string[4];

                int iNumber = 0;
                int iStatus = 0;

                iCnt = (int)pMLOT.Count;

                if(nLotCnt == 1)
                {
                    grdDataView.RowCount = iCnt + 1;

                    for (i = 0; i < iCnt; i++)
                    {
                        iNumber = i + 1;
                        iStatus = 0;
                        
                        sdata[0] = iNumber.ToString();
                        sdata[1] = sLotName;
                        sdata[2] = pMLOT.GetCarrierID(i);

                        iStatus = pMLOT.GetCarrierStatus(i);

                        if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Idle)
                        {
                            sdata[3] = "Not read";
                            grdDataView[3, iNumber].Style.BackColor = Color.Gray;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Read)
                        {
                            sdata[3] = "Read";
                            grdDataView[3, iNumber].Style.BackColor = Color.Yellow;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Validate)
                        {
                            sdata[3] = "Validation";
                            grdDataView[3, iNumber].Style.BackColor = Color.Blue;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Start)
                        {
                            sdata[3] = "Start";
                            grdDataView[3, iNumber].Style.BackColor = Color.Green;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.End)
                        {
                            sdata[3] = "End";
                            grdDataView[3, iNumber].Style.BackColor = Color.Aqua;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Reject)
                        {
                            sdata[3] = "Reject";
                            grdDataView[3, iNumber].Style.BackColor = Color.Red;
                        }
                        else
                        {
                            //----------------------------------------
                            // 존재하지 않거나 맞지 않는 Status
                        }

                        grdDataView[0, iNumber].Value = sdata[0];
                        grdDataView[1, iNumber].Value = sdata[1];
                        grdDataView[2, iNumber].Value = sdata[2];
                        grdDataView[3, iNumber].Value = sdata[3];
                    }
                }

                if(nLotCnt == 2)
                {
                    grdDataView_N.RowCount = iCnt + 1;

                    for (i = 0; i < iCnt; i++)
                    {
                        iNumber = i + 1;
                        iStatus = 0;

                        sdata[0] = iNumber.ToString();
                        sdata[1] = sLotName;
                        sdata[2] = pMLOT.GetCarrierID(i);

                        iStatus = pMLOT.GetCarrierStatus(i);

                        if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Idle)
                        {
                            sdata[3] = "Not read";
                            grdDataView_N[3, iNumber].Style.BackColor = Color.Gray;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Read)
                        {
                            sdata[3] = "Read";
                            grdDataView_N[3, iNumber].Style.BackColor = Color.Yellow;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Validate)
                        {
                            sdata[3] = "Validation";
                            grdDataView_N[3, iNumber].Style.BackColor = Color.Blue;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Start)
                        {
                            sdata[3] = "Start";
                            grdDataView_N[3, iNumber].Style.BackColor = Color.Green;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.End)
                        {
                            sdata[3] = "End";
                            grdDataView_N[3, iNumber].Style.BackColor = Color.Aqua;
                        }
                        else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Reject)
                        {
                            sdata[3] = "Reject";
                            grdDataView_N[3, iNumber].Style.BackColor = Color.Red;
                        }
                        else
                        {
                            //----------------------------------------
                            // 존재하지 않거나 맞지 않는 Status
                        }

                        grdDataView_N[0, iNumber].Value = sdata[0];
                        grdDataView_N[1, iNumber].Value = sdata[1];
                        grdDataView_N[2, iNumber].Value = sdata[2];
                        grdDataView_N[3, iNumber].Value = sdata[3];
                    }
                }
            }

            //List_Title(iCnt + 1);
            List_Title(nLotCnt, iCnt + 1);
        }

        #region function
        public void AddString(string sMsg)
        {
            int nLineCnt = 0;
            string stime = string.Empty;
            
            DateTime dtNow = DateTime.Now;
            stime = dtNow.ToString("[yyyy-MM-dd HH:mm:ss.fff]");

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
            
            sMsg = string.Format("{0} {1} \n", stime, sMsg);

            Log_List.AppendText(sMsg);
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

        private void button3_Click(object sender, EventArgs e)
        {

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
            if (Log_List.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Display();
                });
            }
            else
            {
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
                    Display_Hide();
                });
            }
            else
            {
                Hide();
            }
        }

        private void Button_Display()
        {
            btn_Disc_Conn.Text      = GEM.strA_Conn[CData.GemForm.m_iConnState];
            btn_Disc_Conn.BackColor = GEM.clrA_RC[CData.GemForm.m_iConnState];

            btn_Remot_Sts.Text      = GEM.strA_Cont[CData.GemForm.m_iCtrlState];
            btn_Remot_Sts.BackColor = GEM.clrA_RYC[CData.GemForm.m_iCtrlState];

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
                btn_Offline.Text        = string.Format("On Line Sts");
                btn_Remot_Sts.Text      = string.Format("Remote Status");
                btn_Remot_Sts.BackColor = Color.DarkGreen;

                this.btn_Remote.Enabled = false;
                this.btn_Local.Enabled  = true;
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
            sgfrmGEM.mgem.GoOnlineLocal();
        }

        private void btnRemote_Click(object sender, EventArgs e)
        {
            sgfrmGEM.mgem.GoOnlineRemote();
        }
    }
}