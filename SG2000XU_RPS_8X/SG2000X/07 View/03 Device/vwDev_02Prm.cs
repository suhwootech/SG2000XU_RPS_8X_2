using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm : UserControl
    {
        private int m_iPage = 0;

        private vwDev_02Prm_1Str        m_vw1Str        = new vwDev_02Prm_1Str();
        private vwDev_02Prm_2Stp        m_vw2StpL       = new vwDev_02Prm_2Stp(EWay.L);
        private vwDev_02Prm_2Stp        m_vw3StpR       = new vwDev_02Prm_2Stp(EWay.R);
        private vwDev_02Prm_4Con        m_vw4ConL       = new vwDev_02Prm_4Con(EWay.L);
        private vwDev_02Prm_4ConU       m_vw4ConL_U     = new vwDev_02Prm_4ConU(EWay.L);
        private vwDev_02Prm_4Con        m_vw5ConR       = new vwDev_02Prm_4Con(EWay.R);
        private vwDev_02Prm_4ConU       m_vw5ConR_U     = new vwDev_02Prm_4ConU(EWay.R);
        private vwDev_02Prm_6Etc        m_vw6Etc        = new vwDev_02Prm_6Etc();
        private vwDev_02Prm_6Etc2       m_vw6Etc2       = new vwDev_02Prm_6Etc2();//210825 pjh : Skyworks Option
        private vwDev_02Prm_7Advanced   m_vw7Advanced   = new vwDev_02Prm_7Advanced(); //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
        private vwDev_02Prm_8IV2        vw_02Prm_8IV2   = new vwDev_02Prm_8IV2(); //210824 syc : 2004U device

        public vwDev_02Prm()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");   }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");   }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans"); }

            InitializeComponent();

            m_vw1Str.GoDual += SetDual;

            _SetAdvancedOptionVisible(); //201203 jhc : Advanced Option 메뉴 표시/감춤

            rdb_SubMenu8.Visible = CDataOption.Use2004U;//210827 syc : 2004U IV2

            m_iPage = 1;
        }

        #region Private method
        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
            m_vw1Str .Dispose();
            m_vw2StpL.Dispose();
            m_vw3StpR.Dispose();
            m_vw4ConL.Dispose();
            m_vw4ConL_U.Dispose();
            m_vw5ConR.Dispose();
            m_vw5ConR_U.Dispose();
            m_vw6Etc .Dispose();
            m_vw6Etc2.Dispose();//210825 pjh : Skyworks Device에 Wheel Parameter 추가
            m_vw7Advanced.Dispose(); //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            vw_02Prm_8IV2.Dispose(); //210824 syc : 2004U device
            

        }

        private void _VwAdd()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw1Str.Open();
                    pnl_Base.Controls.Add(m_vw1Str);
                    break;
                case 2:
                    m_vw2StpL.Open();
                    pnl_Base.Controls.Add(m_vw2StpL);
                    break;
                case 3:
                    m_vw3StpR.Open();
                    pnl_Base.Controls.Add(m_vw3StpR);
                    break;
                case 4:
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        m_vw4ConL.Open();
                        pnl_Base.Controls.Add(m_vw4ConL);
                    }
                    else
                    {
                        m_vw4ConL_U.Open();
                        pnl_Base.Controls.Add(m_vw4ConL_U);
                    }
                    break;
                case 5:
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        m_vw5ConR.Open();
                        pnl_Base.Controls.Add(m_vw5ConR);
                    }
                    else 
                    {
                        m_vw5ConR_U.Open();
                        pnl_Base.Controls.Add(m_vw5ConR_U);
                    }
                    break;
                case 6:
                    //210825 pjh : Skyworks Device에 Wheel Parameter 추가
                    //if(CData.CurCompany == ECompany.SkyWorks)
                    //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
                    if (CDataOption.UseDeviceWheel)
                    {
                        m_vw6Etc2.Open();
                        pnl_Base.Controls.Add(m_vw6Etc2);
                    }
                    else
                    {
                        m_vw6Etc.Open();
                        pnl_Base.Controls.Add(m_vw6Etc);
                    }
                    //
                    break;

                //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
                case 7:
                    m_vw7Advanced.Open();
                    pnl_Base.Controls.Add(m_vw7Advanced);
                    break;
                case 8:
                    {
                        vw_02Prm_8IV2.Open();
                        pnl_Base.Controls.Add(vw_02Prm_8IV2);
                        break;
                    }
                //
            }
        }

        private void _VwClr()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw1Str.Close();
                    break;
                case 2:
                    m_vw2StpL.Close();
                    break;
                case 3:
                    m_vw3StpR.Close();
                    break;
                case 4:
                    if (CDataOption.Package == ePkg.Strip)
                    { m_vw4ConL.Close(); }
                    else
                    { m_vw4ConL_U.Close(); }
                    break;
                case 5:
                    if (CDataOption.Package == ePkg.Strip)
                    { m_vw5ConR.Close(); }
                    else
                    { m_vw5ConR_U.Close(); }
                    break;
                case 6:
                    //210825 pjh : Skyworks Device에 Wheel Parameter 추가
                    //if (CData.CurCompany == ECompany.SkyWorks)
                    //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
                    if (CDataOption.UseDeviceWheel)
                    {
                        m_vw6Etc2.Close();
                    }
                    else
                    {
                        m_vw6Etc.Close();
                    }
                    //
                    break;

                //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
                case 7:
                    m_vw7Advanced.Close();
                    break;
                //210824 syc : 2004U device
                case 8:
                    vw_02Prm_8IV2.Close();
                    break;

                //
            }

            pnl_Base.Controls.Clear();
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
            _VwAdd();
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
            _VwClr();
        }

        public void Set()
        {
            if (CData.Dev.bDual == eDual.Normal)
            {
                SetDual(false);
            }
            else
            {
                SetDual(true);
            }

            m_vw1Str .Set();
            m_vw2StpL.Set();
            m_vw3StpR.Set();
            if (CDataOption.Package == ePkg.Strip)
            {
                m_vw4ConL.Set();
                m_vw5ConR.Set();
            }
            else
            {
                m_vw4ConL_U.Set();
                m_vw5ConR_U.Set();
            }

            //210825 pjh : Skyworks Device에 Wheel Parameter 추가
            //if (CData.CurCompany == ECompany.SkyWorks)
            //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
            if (CDataOption.UseDeviceWheel)
            {
                m_vw6Etc2.Set();
            }
            else
            {
                m_vw6Etc.Set();
            }
            //

            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            m_vw7Advanced.Set();
            //

            //210824 syc : 2004U device 
            //현재 테스트 목적으로 Open 추후 라이센스별로 나눌것.
            vw_02Prm_8IV2.Set();
            //
        }

        public void Get()
        {
            m_vw1Str .Get();
            m_vw2StpL.Get();
            m_vw3StpR.Get();

            if (CDataOption.Package == ePkg.Strip)
            {
                m_vw4ConL.Get();
                m_vw5ConR.Get();
            }
            else
            {
                m_vw4ConL_U.Get();
                m_vw5ConR_U.Get();
            }

            //210825 pjh : Skyworks Device에 Wheel Parameter 추가
            //if (CData.CurCompany == ECompany.SkyWorks)
            //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
            if (CDataOption.UseDeviceWheel)
            {
                m_vw6Etc2.Get();
            }
            else
            {
                m_vw6Etc.Get();
            }
            //

            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            m_vw7Advanced.Get();
            //

            //210824 syc : 2004U device 
            //현재 테스트 목적으로 Open 추후 라이센스별로 나눌것.
            vw_02Prm_8IV2.Get();
            //
        }

        public void SetDual(bool bVal)
        {
            if (!bVal)
            {
                rdb_SubMenu2.Text = "Step";
                rdb_SubMenu4.Text = "Contact";
                rdb_SubMenu3.Visible = false;
                rdb_SubMenu5.Visible = false;
            }
            else
            {
                rdb_SubMenu2.Text = "Step (Left)";
                rdb_SubMenu4.Text = "Contact (Left)";
                rdb_SubMenu3.Visible = true;
                rdb_SubMenu5.Visible = true;
            }
        }
        #endregion

        private void rdb_SubMenu_CheckedChanged(object sender, EventArgs e)
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
            }
        }

        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        /// <summary>
        /// 스트립 측정 포인트 수 체크
        /// 반드시 m_vw4ConL.Get(), m_vw5ConR.Get() 호출 후 본 함수를 호출해야 정확한 측정 포인트 수가 파악됨
        /// </summary>
        /// <returns>측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과시 표시할 에러 메시지, ""=측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 이내(안전), !""=측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과(위험)</returns>
        public string CheckMeasurePointCount()
        {
            int [] iUseCnt = {0,0};
            int [] iUse18PCnt = {0,0};
            int iRet = 0; //200713 jhc


            if (CDataOption.Package == ePkg.Strip)
            {
                //Left Table
                //200713 jhc :
                iRet = m_vw4ConL.CheckMeasurePointCount( ref iUseCnt, ref iUse18PCnt );
                if (iRet == 0)
                //if (0 == m_vw4ConL.CheckMeasurePointCount( ref iUseCnt, ref iUse18PCnt ))
                //
                {
                    //Main Strip 측정 포인트 수 체크
                    if (GV.STRIP_MEASURE_POINT_MAX < iUseCnt[0])
                    { return ("Left Table Main Strip Measure Point Count Over - Before (" + iUseCnt[0] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                    else if (GV.STRIP_MEASURE_POINT_MAX < iUseCnt[1])
                    { return ("Left Table Main Strip Measure Point Count Over - After (" + iUseCnt[1] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                    //18 Point Strip 측정 포인트 수 체크
                    if (CDataOption.Use18PointMeasure)
                    {
                        if (GV.STRIP_MEASURE_POINT_MAX < iUse18PCnt[0])
                        { return ("Left Table Test Strip Measure Point Count Over - Before (" + iUse18PCnt[0] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                        else if (GV.STRIP_MEASURE_POINT_MAX < iUse18PCnt[1])
                        { return ("Left Table Test Strip Measure Point Count Over - After (" + iUse18PCnt[1] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                    }
                }

                //200713 jhc : 측정 포인트가 하나도 없어도 Error
                else if (iRet == 10)
                { return ("Left Table Main Strip, Before Measure Point Count = 0\n\nPlease Select Measure Point"); }
                else if (iRet == 11)
                { return ("Left Table Main Strip, After Measure Point Count = 0\n\nPlease Select Measure Point"); }
                 else if (iRet == 20)
                { return ("Left Table Test Strip, Before Measure Point Count = 0\n\nPlease Select Measure Point"); }
                else if (iRet == 21)
                { return ("Left Table Test Strip, After Measure Point Count = 0\n\nPlease Select Measure Point"); }
                //

                //Right Table
                iRet = m_vw5ConR.CheckMeasurePointCount( ref iUseCnt, ref iUse18PCnt );
                if (iRet == 0)
                //if (0 == m_vw5ConR.CheckMeasurePointCount( ref iUseCnt, ref iUse18PCnt ))
                //
                {
                    //Main Strip 측정 포인트 수 체크
                    if (GV.STRIP_MEASURE_POINT_MAX < iUseCnt[0])
                    { return ("Right Table Main Strip Measure Point Count Over - Before (" + iUseCnt[0] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                    else if (GV.STRIP_MEASURE_POINT_MAX < iUseCnt[1])
                    { return ("Right Table Main Strip Measure Point Count Over - After (" + iUseCnt[1] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                    //18 Point Strip 측정 포인트 수 체크
                    if (CDataOption.Use18PointMeasure)
                    {
                        if (GV.STRIP_MEASURE_POINT_MAX < iUse18PCnt[0])
                        { return ("Right Table Test Strip Measure Point Count Over - Before (" + iUse18PCnt[0] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                        else if (GV.STRIP_MEASURE_POINT_MAX < iUse18PCnt[1])
                        { return ("Right Table Test Strip Measure Point Count Over - After (" + iUse18PCnt[1] + ") > " + GV.STRIP_MEASURE_POINT_MAX); }
                    }
                }

                //200713 jhc : 측정 포인트가 하나도 없어도 Error
                else if (iRet == 10)
                { return ("Right Table Main Strip, Before Measure Point Count = 0\n\nPlease Select Measure Point"); }
                else if (iRet == 11)
                { return ("Right Table Main Strip, After Measure Point Count = 0\n\nPlease Select Measure Point"); }
                 else if (iRet == 20)
                { return ("Right Table Test Strip, Before Measure Point Count = 0\n\nPlease Select Measure Point"); }
                else if (iRet == 21)
                { return ("Right Table Test Strip, After Measure Point Count = 0\n\nPlease Select Measure Point"); }
                //
            }
            else
            {
                return (""); //2000U는 아직 18 Point 측정 기능 미지원
            }

            return (""); //스트립 측정 포인트 수 허용 범위 내 (OK)
        }

        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        /// <summary>
        /// BCR 옵션 변경 후 SAVE 실패 시 원래 옵션을 다시 표시해 주기 위함
        /// </summary>
        public void ReloadEtc()
        {
            CDev.It.ReLoadEtcBcrOption();
            m_vw6Etc.Set(); //Etc 화면 업데이트
        }
        //

        //201203 jhc : Advanced Option 메뉴 표시/감춤
        private void _SetAdvancedOptionVisible()
        {
            rdb_SubMenu7.Visible = CDataOption.UseDeviceAdvancedOption;
        }
        //

        //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
        /// <summary>
        /// Wheel Loss Correct 설정값이 Total Wheel Loss Limit 값 초과 시 저장하지 않고 기존 설정값을 재로딩하기 위함
        /// </summary>
        public void ReloadWheelLossSetValue()
        {
            CDev.It.ReLoadWheelLossCorrect(); //Wheel Loss Correct 설정값 재 로딩 from the Device File
            m_vw7Advanced.Set(); //Advanced 메뉴 화면 업데이트
        }
        //
    }
}
