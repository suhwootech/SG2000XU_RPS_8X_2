using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan : UserControl
    {
        private int m_iPage = -1;

        private vwMan_01Man m_vw01Man = new vwMan_01Man();
        private vwMan_02Onl m_vw02Onl = new vwMan_02Onl();
        private vwMan_03Inr m_vw03Inr = new vwMan_03Inr();
        private vwMan_04Onp m_vw04Onp = new vwMan_04Onp();
        private vwMan_05Grl m_vw05Grl = new vwMan_05Grl();
        private vwMan_06Grd m_vw06Grd = new vwMan_06Grd();
        private vwMan_07Grr m_vw07Grr = new vwMan_07Grr();
        private vwMan_08Ofp m_vw08Ofp = new vwMan_08Ofp();
        private vwMan_09Dry m_vw09Dry = new vwMan_09Dry();
        private vwMan_10Ofl m_vw10Ofl = new vwMan_10Ofl();
        private vwMan_11Cnv m_vw11Cnv = new vwMan_11Cnv();
        public  vmMan_13QcVision m_vw13QcVision  = new vmMan_13QcVision();

        public vwMan()
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

            m_iPage = 1;
        }

        /// <summary>
        /// 화면에 출력될 때 실행
        /// 타이머 시작 및 현재 변수 출력
        /// </summary>
        public void Open()
        {
            _HideMenu();
            _Open();
        }

        /// <summary>
        /// 화면에서 꺼질 때 실행
        /// 타이머 스탑
        /// </summary>
        public void Close()
        {
            _Close();
        }

        public void Release()
        {
            m_vw01Man.Dispose();
            m_vw02Onl.Dispose();
            m_vw03Inr.Dispose();
            m_vw04Onp.Dispose();
            m_vw05Grl.Dispose();
            m_vw06Grd.Dispose();
            m_vw07Grr.Dispose();
            m_vw08Ofp.Dispose();
            m_vw09Dry.Dispose();
            m_vw10Ofl.Dispose();
            m_vw11Cnv.Dispose();
            m_vw13QcVision.Dispose();
        }

        private void rdb_Man_CheckedChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            RadioButton mRdb = sender as RadioButton;
            int iNext = int.Parse(mRdb.Tag.ToString());

            _SetLog(string.Format("N, {0} -> {1}", m_iPage, iNext));
            if (m_iPage != iNext)
            {
                _Close();

                m_iPage = iNext;

                _Open();
            }
        }


        private void _Open()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Man.Open();
                    pnl_Base.Controls.Add(m_vw01Man);
                    break;
                case 2:
                    m_vw02Onl.Open();
                    pnl_Base.Controls.Add(m_vw02Onl);
                    break;
                case 3:
                    m_vw03Inr.Open();
                    pnl_Base.Controls.Add(m_vw03Inr);
                    break;
                case 4:
                    m_vw04Onp.Open();
                    pnl_Base.Controls.Add(m_vw04Onp);
                    break;
                case 5:
                    m_vw05Grl.Open();
                    pnl_Base.Controls.Add(m_vw05Grl);
                    break;
                case 6:
                    m_vw06Grd.Open();
                    pnl_Base.Controls.Add(m_vw06Grd);
                    break;
                case 7:
                    m_vw07Grr.Open();
                    pnl_Base.Controls.Add(m_vw07Grr);
                    break;
                case 8:
                    m_vw08Ofp.Open();
                    pnl_Base.Controls.Add(m_vw08Ofp);
                    break;
                case 9:
                    m_vw09Dry.Open();
                    pnl_Base.Controls.Add(m_vw09Dry);
                    break;
                case 10:
                    m_vw10Ofl.Open();
                    pnl_Base.Controls.Add(m_vw10Ofl);
                    break;
                case 11:
                    m_vw11Cnv.Open();
                    pnl_Base.Controls.Add(m_vw11Cnv);
                    break;
                case 12:
                    m_vw13QcVision.Open();
                    pnl_Base.Controls.Add(m_vw13QcVision);
                    break;
            }
        }

        private void _Close()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Man.Close();
                    break;
                case 2:
                    m_vw02Onl.Close();
                    break;
                case 3:
                    m_vw03Inr.Close();
                    break;
                case 4:
                    m_vw04Onp.Close();
                    break;
                case 5:
                    m_vw05Grl.Close();
                    break;
                case 6:
                    m_vw06Grd.Close();
                    break;
                case 7:
                    m_vw07Grr.Close();
                    break;
                case 8:
                    m_vw08Ofp.Close();
                    break;
                case 9:
                    m_vw09Dry.Close();
                    break;
                case 10:
                    m_vw10Ofl.Close();
                    break;
                case 11:
                    m_vw11Cnv.Close();
                    break;
                case 12:
                    m_vw13QcVision.Close();
                    break;
            }

            pnl_Base.Controls.Clear();
        }

        private void _HideMenu()
        {
            ELv RetLv = CData.Lev;
            //20190309 ghk_level
            rdb_2Onl.Visible = (int)RetLv >= CData.Opt.iLvOnL;
            rdb_3Inr.Visible = (int)RetLv >= CData.Opt.iLvInr;
            rdb_4Onp.Visible = (int)RetLv >= CData.Opt.iLvOnp;
            rdb_5GrL.Visible = (int)RetLv >= CData.Opt.iLvGrL;
            rdb_6GrD.Visible = (int)RetLv >= CData.Opt.iLvGrd;
            rdb_7GrR.Visible = (int)RetLv >= CData.Opt.iLvGrr;
            rdb_8OfP.Visible = (int)RetLv >= CData.Opt.iLvOfp;
            rdb_9Dry.Visible = (int)RetLv >= CData.Opt.iLvDry;
            rdb_10OfL.Visible = (int)RetLv >= CData.Opt.iLvOfL;

            // 2021.02.09 SungTae : QC Vision을 현재 Skyworks만 사용하고 있어서 Company Option으로 관리
            if (CData.CurCompany == ECompany.SkyWorks)  rdb12QcVision.Visible = true;
            else                                        rdb12QcVision.Visible = false;
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

        public void InitPage()
        {
            rdb_1Man.Checked = true;
        }
    }
}
