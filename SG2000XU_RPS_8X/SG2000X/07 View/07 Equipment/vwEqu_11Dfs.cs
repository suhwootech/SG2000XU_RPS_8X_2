using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_11Dfs : UserControl
    {
        //20190617 ghk_dfserver
        private TextBox[,] tx;
        private int m_iRow = 0;

        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwEqu_11Dfs()
        {
            if ((int)ELang.English == CData.Opt.iSelLan)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)ELang.Korea == CData.Opt.iSelLan)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)ELang.China == CData.Opt.iSelLan)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            string sTemp = "";

            if (CDf.It.CheckCon())
            {
                btnDf_Con.ForeColor = Color.Lime;
                if (btnDf_Con.Text == "CONNECT")
                { _SaveLog("Connected"); }

                btnDf_Con.Text = "DISCONNECT";
            }
            else
            {
                btnDf_Con.ForeColor = Color.Red;
                if (btnDf_Con.Text == "DISCONNECT")
                { _SaveLog("DisConnected"); }
                btnDf_Con.Text = "CONNECT";
            }

            if (CDf.It.sLog != "")
            {
                sTemp = "<<  ";
                sTemp += CDf.It.sLog;

                _SaveLog(sTemp);
                CDf.It.sLog = "";
            }
            txtDf_RowNum.Text = m_iRow.ToString();
        }

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
            // 타이머에 대한 모든 리소스 해제
            if (m_tmUpdt != null)
            {
                Close();
                m_tmUpdt.Dispose();
                m_tmUpdt = null;
            }
        }

        /// <summary>
        /// 데이터를 화면에 출력
        /// </summary>
        private void _Set()
        {
            txtDf_Ip.Text = CData.dfInfo.sIp;
            txtDf_Port.Text = CData.dfInfo.iPort.ToString();
            txtDf_Id.Text = CData.dfInfo.sId;
        }

        /// <summary>
        /// 화면에 값을 데이터로 저장
        /// </summary>
        private void _Get()
        {
            CData.dfInfo.sIp = txtDf_Ip.Text;
            int.TryParse(txtDf_Port.Text, out CData.dfInfo.iPort);
            CData.dfInfo.sId = txtDf_Id.Text;
        }

        /// <summary>
        /// Manual view에 조작 로그 저장 함수
        /// </summary>
        /// <param name="sMsg"></param>
        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.OPL, string.Format("{0}.cs {1}() {2} Lv:{3}", sCls, sMth, sMsg, CData.Lev));
        }

        private void _SaveLog(string sLog)
        {
            string sTemp = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "  ";
            sTemp += sLog;
            lib_Df_Log.Items.Insert(0, sTemp);
        }
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            _Set();

            //20191029 ghk_dfserver_notuse_df
            if (CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {//Df 사용시 DF 측정 안할경우
                lbDf_Pcb1    .Visible = false;
                txtDf_Pcb1   .Visible = false;
                lbDf_Pcb2    .Visible = false;
                txtDf_Pcb2   .Visible = false;
                lbDf_Pcb3    .Visible = false;
                txtDf_Pcb3   .Visible = false;
                btnDf_SendPcb.Visible = false;
                lbDf_PcbUse  .Visible = false;
                cmbDf_PcbUse .Visible = false;
            }

            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled) { m_tmUpdt.Start(); }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled) { m_tmUpdt.Stop(); }
        }
        #endregion

        private void btnDf_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;

            _Get();

            iRet = CDf.It.Save();
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "Dfserver config file save fail");
            }
            else
            {
                CMsg.Show(eMsg.Notice, "Notice", "Dfserver config file save success");
            }
            CDf.It.DisConnectServer();            
        }

        private void btnDf_Con_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            if (!CDf.It.CheckCon())
            {
                CDf.It.ConnectServer();
            }
            else
            {
                CDf.It.DisConnectServer();
            }            
        }

        private void btnDf_SendId_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;
            CDf.It.SendID();        
        }

        private void btnDf_SendReady_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";

            CDf.It.SendReady();

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + ",Ready;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendLotOpen_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";

            CData.dfInfo.sLotName = txtDf_LotName.Text;
            CData.dfInfo.sGl = cmbDf_GrdMode.Items[cmbDf_GrdMode.SelectedIndex].ToString();
            CDf.It.SendLotOpen();

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + ",LotOpen," + CData.dfInfo.sLotName + "," + CData.dfInfo.sGl + ";";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendLotExist_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";

            CDf.It.SendLotExist();

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + ",LotExist;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendLotEnd_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";

            CDf.It.SendLotEnd();

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + ",LotEnd;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendInrBcr_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";

            CData.dfInfo.sBcr = txtDf_BcrId.Text;

            CDf.It.SendInrBcr();

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + ",InrailBCR," + CData.dfInfo.sBcr + ";";
            _SaveLog(sTemp);
        }

        private void btnDf_SendRetBcr_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";

            CDf.It.SendResultBcr();

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + ",ResultBCR;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendGrdReady_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";
            string sWay = "";

            sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();

            CDf.It.SendGrdReady(sWay);

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + "," + sWay + ",GrdReady;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendBfStart_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;
            CData.dfInfo.sBcr = txtDf_BcrId.Text;
            CData.GRLDfData.sBcr = txtDf_BcrId.Text;
            CData.GRRDfData.sBcr = txtDf_BcrId.Text;

            string sTemp = "";
            string sWay = "";

            sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();

            CDf.It.SendBfStart(sWay);

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + "," + sWay + ",BfStart," + CData.dfInfo.sBcr + ";";
            _SaveLog(sTemp);

            foreach (Control iTem in pnlDf_Row.Controls.OfType<TextBox>())
            {
                iTem.BackColor = Color.White;
            }

            m_iRow = 0;            
        }

        private void btnDf_SendBfEnd_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";
            string sWay = "";

            sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();

            CDf.It.SendBfEnd(sWay);

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + "," + sWay + ",BfEnd;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendAfStart_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;
            CData.dfInfo.sBcr = txtDf_BcrId.Text;
            CData.GRLDfData.sBcr = txtDf_BcrId.Text;
            CData.GRRDfData.sBcr = txtDf_BcrId.Text;

            string sTemp = "";
            string sWay = "";

            sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();

            CDf.It.SendAfStart(sWay);

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + "," + sWay + ",AfStart," + CData.dfInfo.sBcr + ";";
            _SaveLog(sTemp);
            m_iRow = 0;

            foreach (Control iTem in pnlDf_Row.Controls.OfType<TextBox>())
            {
                iTem.BackColor = Color.White;
            }            
        }

        private void btnDf_SendAfEnd_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";
            string sWay = "";

            sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();

            CDf.It.SendAfEnd(sWay);

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + "," + sWay + ",AfEnd;";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendPcb_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            string sTemp = "";
            string sWay = "";

            sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();

            CData.GRLDfData.dPcb1 = double.Parse(txtDf_Pcb1.Text);
            CData.GRLDfData.dPcb2 = double.Parse(txtDf_Pcb2.Text);
            CData.GRLDfData.dPcb3 = double.Parse(txtDf_Pcb3.Text);
            CData.GRLDfData.sPcbUse = cmbDf_PcbUse.Items[cmbDf_PcbUse.SelectedIndex].ToString();

            CData.GRRDfData.dPcb1 = double.Parse(txtDf_Pcb1.Text);
            CData.GRRDfData.dPcb2 = double.Parse(txtDf_Pcb2.Text);
            CData.GRRDfData.dPcb3 = double.Parse(txtDf_Pcb3.Text);
            CData.GRRDfData.sPcbUse = cmbDf_PcbUse.Items[cmbDf_PcbUse.SelectedIndex].ToString();

            CDf.It.SendPcbData(sWay);

            sTemp = ">>  ";
            sTemp += "@" + CData.dfInfo.sId + "," + sWay + ",PCB," + CData.GRLDfData.dPcb1 + "," + CData.GRLDfData.dPcb2 + "," + CData.GRLDfData.dPcb3 + "," + CData.GRLDfData.sPcbUse + ";";
            _SaveLog(sTemp);            
        }

        private void btnDf_SendPos_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRow = 0;
            int iCol = 0;
            int iPnlCnt = 0;
            int iX = 0;
            int iY = 0;

            iRow = int.Parse(txtDf_Row.Text);
            iCol = int.Parse(txtDf_Col.Text);

            iPnlCnt = pnlDf_Row.Controls.Count;
            if (iPnlCnt > 0)
            {
                for (int i = 0; i < iPnlCnt; i++)
                {
                    pnlDf_Row.Controls.RemoveAt(0);
                }
            }

            tx = new TextBox[iCol, iRow];
            for (int r = 0; r < iRow; r++)
            {
                for (int c = 0; c < iCol; c++)
                {
                    tx[c, r] = new TextBox();
                    tx[c, r].Size = new Size(50, 26);
                    iX = tx[c, r].Width;
                    iY = tx[c, r].Height;
                    tx[c, r].Location = new Point(iX * c, iY * r);

                    pnlDf_Row.Controls.Add(tx[c, r]);
                }
            }            
        }

        private void btnDf_SendRowData_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.dfInfo.sId = txtDf_Id.Text;

            int iRow = 0;
            int iCol = 0;
            int iRowNum = 0;
            string sRowData = "";
            string sTemp = "";
            string sWay = "";

            iRow = int.Parse(txtDf_Row.Text);
            iCol = int.Parse(txtDf_Col.Text);

            if (m_iRow < iRow)
            {
                for (int c = 0; c < iCol; c++)
                {
                    if (tx[c, m_iRow].Text == "")
                    { sRowData += "."; }
                    else
                    { sRowData += tx[c, m_iRow].Text; }

                    tx[c, m_iRow].BackColor = Color.Red;
                    if (c < (iCol - 1))
                    { sRowData += ","; }
                }

                sWay = cmbDf_GrdWay.Items[cmbDf_GrdWay.SelectedIndex].ToString();
                iRowNum = int.Parse(txtDf_RowNum.Text);

                CData.GRLDfData.sRow = "R" + iRowNum.ToString("D2");
                CData.GRRDfData.sRow = "R" + iRowNum.ToString("D2");

                CDf.It.SendGrdRowData(sWay, sRowData);
                sTemp = ">>  ";
                sTemp += "@" + CData.dfInfo.sId + "," + sWay + "," + CData.GRLDfData.sRow + "," + sRowData + ";";
                _SaveLog(sTemp);

                m_iRow++;
            }
            else
            {
                _SaveLog("Out of Index");
            }            
        }
    }
}
