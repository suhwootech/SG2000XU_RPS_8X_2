using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_01Wmx : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwEqu_01Wmx()
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
            if (CData.WMX)
            {
                #region WMX Engine Stat
                wmxCLRLibrary.PEState eStat = CWmx.It.Stat.EngineStatus;
                if (eStat < wmxCLRLibrary.PEState.Communicating)
                {
                    lblE1_ComStat.BackColor = Color.LightGray;
                    lblE1_ComStat.Text = "Offline";

                    lblE1_EngStat.BackColor = Color.Lime;
                    lblE1_EngStat.Text = "Running";
                }
                else if (eStat == wmxCLRLibrary.PEState.Communicating)
                {
                    lblE1_ComStat.BackColor = Color.Lime;
                    lblE1_ComStat.Text = "Online";

                    lblE1_EngStat.BackColor = Color.Lime;
                    lblE1_EngStat.Text = "Running";
                }
                else if (eStat == wmxCLRLibrary.PEState.Shutdown)
                {
                    lblE1_ComStat.BackColor = Color.LightGray;
                    lblE1_ComStat.Text = "Offline";

                    lblE1_EngStat.BackColor = Color.LightGray;
                    lblE1_EngStat.Text = "Shutdown";
                }
                else
                {
                    lblE1_ComStat.BackColor = Color.LightGray;
                    lblE1_ComStat.Text = "Offline";

                    lblE1_EngStat.BackColor = Color.Red;
                    lblE1_EngStat.Text = "Error";
                }
                #endregion

                btnE1_Hot.Enabled = CWmx.It.IsLostSlave();

                for (int i = 0; i <= CWmx.It.SLAVE_COUNT; i++)
                {
                    if (CWmx.It.EcInfo.Slaves[i].NumOfAxes == 1)
                    {
                        uint iAlias = CWmx.It.EcInfo.Slaves[i].Alias;
                        Panel mPnl = (Panel)this.Controls["pnlE1_Ax" + iAlias];
                        mPnl.Controls["lblE1_Ax" + iAlias + "_ID"].Text = "ID : " + iAlias;
                        if (CWmx.It.EcInfo.Slaves[i].Offline)
                        {
                            mPnl.BackColor = Color.LightGray;
                            mPnl.Controls["lblE1_Ax" + iAlias + "_Op"].Text = "Offline";
                        }
                        else
                        {
                            mPnl.BackColor = Color.Lime;
                            mPnl.Controls["lblE1_Ax" + iAlias + "_Op"].Text = CWmx.It.EcInfo.Slaves[i].State.ToString();
                        }
                    }
                }
            }
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
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            // Onloader picker Y축 표시 여부
            if (CDataOption.CurEqu == eEquType.Pikcer)
            { pnlE1_Ax3.Visible = true; }
            else
            { pnlE1_Ax3.Visible = false; }

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

        private void btnE1_Hot_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CWmx.It.ELib.StartHotconnect();
            GU.Delay(500);
        }
    }
}
