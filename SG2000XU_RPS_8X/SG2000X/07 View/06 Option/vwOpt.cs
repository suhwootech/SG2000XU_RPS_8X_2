using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt : UserControl
    {
        private int m_iPage = 0;

        private vwOpt_01Gen m_vw01Gen = new vwOpt_01Gen();
        private vwOpt_02Ldr m_vw02Ldr = new vwOpt_02Ldr();
        private vwOpt_03Dry m_vw03Dry = new vwOpt_03Dry();
        private vwOpt_04Pck m_vw04Pck = new vwOpt_04Pck();
        private vwOpt_05Grd m_vw05Grd = new vwOpt_05Grd();
        private vwOpt_06Lvs m_vw06Lvs = new vwOpt_06Lvs();
        private vwOpt_07Sys m_vw07Sys = new vwOpt_07Sys();
        private vwOpt_08Tbl m_vw08Tbl = new vwOpt_08Tbl();
        private vwOpt_09Mnt m_vw09Mnt = new vwOpt_09Mnt();   // 2021.07.14 lhs

        public vwOpt()
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

            m_vw01Gen.Set();
            m_vw02Ldr.Set();
            m_vw03Dry.Set();
            m_vw04Pck.Set();
            m_vw05Grd.Set();
            m_vw06Lvs.Set();
            m_vw07Sys.Set();
            m_vw08Tbl.Set();
            m_vw09Mnt.Set(); // 2021.07.14 lhs

        }

        #region Basic method
        public void Open()
        {
            m_iPage = 1;
            _VwAdd();
            rdb_Gen.Checked = true;
            _HideMenu();
        }

        public void Close()
        {
            _VwClr();
        }

        public void Release()
        {
            m_vw01Gen.Dispose();
            m_vw02Ldr.Dispose();
            m_vw03Dry.Dispose();
            m_vw04Pck.Dispose();
            m_vw05Grd.Dispose();
            m_vw06Lvs.Dispose();
            m_vw07Sys.Dispose();
            m_vw08Tbl.Dispose();
            m_vw09Mnt.Dispose(); // 2021.07.14 lhs
        }

        #endregion

        private void _VwAdd()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Gen.Open();
                    pnl_Base.Controls.Add(m_vw01Gen);
                    break;
                case 2:
                    m_vw02Ldr.Open();
                    pnl_Base.Controls.Add(m_vw02Ldr);
                    break;
                case 3:
                    m_vw03Dry.Open();
                    pnl_Base.Controls.Add(m_vw03Dry);
                    break;
                case 4:
                    m_vw04Pck.Open();
                    pnl_Base.Controls.Add(m_vw04Pck);
                    break;
                case 5:
                    m_vw05Grd.Open();
                    pnl_Base.Controls.Add(m_vw05Grd);
                    break;
                case 6:
                    m_vw06Lvs.Open();
                    pnl_Base.Controls.Add(m_vw06Lvs);
                    break;
                case 7:
                    m_vw07Sys.Open();
                    pnl_Base.Controls.Add(m_vw07Sys);
                    break;
                case 8:
                    m_vw08Tbl.Open();
                    pnl_Base.Controls.Add(m_vw08Tbl);
					break;

                // 2021.07.14 lhs Start
                case 9:
					m_vw09Mnt.Open();
					pnl_Base.Controls.Add(m_vw09Mnt);
					break;
                // 2021.07.14 lhs End
            }
        }

        private void _VwClr()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Gen.Close();
                    break;
                case 2:
                    m_vw02Ldr.Close();
                    break;
                case 3:
                    m_vw03Dry.Close();
                    break;
                case 4:
                    m_vw04Pck.Close();
                    break;
                case 5:
                    m_vw05Grd.Close();
                    break;
                case 6:
                    m_vw06Lvs.Close();
                    break;
                case 7:
                    m_vw07Sys.Close();
                    break;
                case 8:
                    m_vw08Tbl.Close();
                    break;

                // 2021.07.14 lhs Start
                case 9:
                    m_vw09Mnt.Close();
                    break;
                // 2021.07.14 lhs End
            }

            m_iPage = 0;
            pnl_Base.Controls.Clear();
        }

        private void _HideMenu()
        {
            // 2020.10.13 SungTae Start : Add
            rdb_Ldr.Visible = (int)CData.Lev >= CData.Opt.iLvOptLoader;
            rdb_Dry.Visible = (int)CData.Lev >= CData.Opt.iLvOptRailDry;
            rdb_Pck.Visible = (int)CData.Lev >= CData.Opt.iLvOptPicker;
            rdb_Grd.Visible = (int)CData.Lev >= CData.Opt.iLvOptGrind;
            // 2020.10.13 SungTae End

            rdb_Sys.Visible = (int)CData.Lev >= CData.Opt.iLvOptSysPos;
            rdb_TbG.Visible = (int)CData.Lev >= CData.Opt.iLvOptTbGrd;
            rdb_LvS.Visible = (int)CData.Lev >= (int)ELv.Engineer;

            // 2021.07.15 lhs Start
            //if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) 
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)//220620 pjh : 조건 추가
            {
                rdb_Gen.Visible = (int)CData.Lev >= CData.Opt.iLvOptGen;    // General
                rdb_Mnt.Visible = (int)CData.Lev >= CData.Opt.iLvOptMnt;    // Maintenance 추가(SCK, JSCK 전용)
            }
            else
            {
                rdb_Mnt.Visible = false;    // Maintenance 숨김
            }
            // 2021.07.15 lhs End
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

        private void rdb_CheckedChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            RadioButton mRdb = sender as RadioButton;
            int iNext = int.Parse(mRdb.Tag.ToString());

            _SetLog(string.Format("N, {0} -> {1}", m_iPage, iNext));
            if (m_iPage != iNext)
            {
                _VwClr();

                m_iPage = iNext;

                _VwAdd();

				if (m_iPage == 9) // 
				{
                    btn_Save.Enabled = false;
				}
                else
                {
                    btn_Save.Enabled = true;
				}

			}

        }

        /// <summary>
        /// 옵션 전체의 세이브 버튼
        /// </summary>
        /// <param name="sender"></param>ㅁ
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            // 2021.07.22 SungTae : [수정] ASE-KR VOC로 Engineer Level에서도 Parameter 수정 가능하도록 조건 추가
			// 2021.07.15 lhs Start : 수정
            ////200422 jym : Lot Open시 저장 불가능
            //if (CData.LotInfo.bLotOpen)
            //{
            //    CMsg.Show(eMsg.Error, "Error", "First Lot End and after Save");
            //    return;
            //}
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET || CData.CurCompany == ECompany.ASE_KR)
            {
                if (CData.LotInfo.bLotOpen && CData.Lev < ELv.Engineer)
                {
                    // 2021-11-17, jhLee, Multi-LOT인 경우 처리중인 LOT이 존재하는 경우에는 저장할 수 없다.
                    if (CData.IsMultiLOT())
                    {
                        // LOT 정보가 있는 경우에에는 저장 불가
                        if (CData.LotMgr.GetCount() > 0)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Check LOT Information and after Save or Change Master Level");
                            return;
                        }
                    }
                    else
                    {
                        //ksg Msg 띄워야 함.
                        CMsg.Show(eMsg.Error, "Error", "First Lot End and after Save or Change Engineer Level");
                        return;
                    }
                }
            }
            else
            {
                if (CData.LotInfo.bLotOpen && CData.Lev < ELv.Master)
                {

                    // 2021-11-17, jhLee, Multi-LOT인 경우 처리중인 LOT이 존재하는 경우에는 저장할 수 없다.
                    if (CData.IsMultiLOT())
                    {
                        // LOT 정보가 있는 경우에에는 저장 불가
                        if (CData.LotMgr.GetCount() > 0)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Check LOT Information and after Save or Change Master Level");
                            return;
                        }
                    }
                    else
                    {
                        //ksg Msg 띄워야 함.
                        CMsg.Show(eMsg.Error, "Error", "First Lot End and after Save or Change Master Level");
                        return;
                    }
                }
            }
            // 2021.07.15 lhs End

            CData.bLastClick = true;

            m_vw01Gen.Get();
            m_vw02Ldr.Get();
            m_vw03Dry.Get();
            m_vw04Pck.Get();
            m_vw05Grd.Get();
            m_vw06Lvs.Get();
            m_vw07Sys.Get();
            m_vw08Tbl.Get();
            //m_vw09Mnt.Get();  // 2021.07.14 lhs : 별도 저장하는 곳에서

            CSetup.It.Save();
            COpt.It.Save();

            // 181218 옵션에서 Position 변경 후 Device에서 Current 포지션 불러올때 값 변경
            CSetup.It.Load();
            COpt  .It.Load();

            m_vw01Gen.Set();
            m_vw02Ldr.Set();
            m_vw03Dry.Set();
            m_vw04Pck.Set();
            m_vw05Grd.Set();
            m_vw06Lvs.Set();
            m_vw07Sys.Set();
            m_vw08Tbl.Set();
            //m_vw09Mnt.Set();  // 2021.07.14 lhs : 별도 저장하는 곳에서

            CMsg.Show(eMsg.Notice, "Notice", "Option data save complete");
        }
    }
}
