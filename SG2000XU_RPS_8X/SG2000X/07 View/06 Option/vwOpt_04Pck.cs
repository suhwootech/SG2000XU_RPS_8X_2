using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_04Pck : UserControl
    {
        public vwOpt_04Pck()
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

            // 2021.07.05 SungTae Start : [수정] 
            //if (CDataOption.PadClean == eOffPadCleanType.LRType)
            if (CDataOption.PadClean == eOffPadCleanType.LRType && CData.CurCompany != ECompany.ASE_KR)
            {
                pnl_PadCleanTime.Visible = false;
            }
            else
            {
                pnl_PadCleanTime.Visible = true;

                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    lbl_OfpPadCleanTime_T.Visible   = false;
                    txt_OfpPadCleanTime.Visible     = false;
                    lbl_OfpPadCleanTime_U.Visible   = false;

                    groupBox3.Visible = true;
                }
                else
                {
                    lbl_OfpPadCleanTime_T.Visible   = true;
                    txt_OfpPadCleanTime.Visible     = true;
                    lbl_OfpPadCleanTime_U.Visible   = true;

                    groupBox3.Visible = false;
                }
            }
            // 2021.07.05 SungTae End

            // 2021.07.22 lhs Start : ASE_KR 만 표시 
            if(CData.CurCompany == ECompany.ASE_KR) {   ckb_OfpUseCenterPos.Visible = true;     }
            else                                    {   ckb_OfpUseCenterPos.Visible = false;    }
            // 2021.07.22 lhs End

            // 2021.04.22 lhs Start  // OffPicker로 Dryer의 물튐 방지용 커버 역할
            if (CDataOption.UseDryWtNozzle) { ckb_OfpCoverDryerSkip.Visible = true;     }
            else                            { ckb_OfpCoverDryerSkip.Visible = false;    }
            // 2021.04.22 lhs End
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            Set();

            // 2021.03.18 SungTae : 고객사(ASE-KR) 요청으로 Center Pos Option 사용유무 조건 추가
            //if(CData.CurCompany == ECompany.ASE_KR) ckb_OfpUseCenterPos.Visible = true;
            //else                                    ckb_OfpUseCenterPos.Visible = false;
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        public void Set()
        {
            txt_OnpRinseTime_L.Text = CData.Opt.aOnpRinseTime[(int)EWay.L].ToString();
            txt_OnpRinseTime_R.Text = CData.Opt.aOnpRinseTime[(int)EWay.R].ToString();
            txt_OfpPadCleanTime.Text = CData.Opt.iOfpPadCleanTime.ToString();

            // 200330 mjy : 신규추가
            txt_PickGap.Text = CData.Opt.dPickGap.ToString();
            txt_PickDelay.Text = CData.Opt.iPickDelay.ToString();
            txt_PlaceGap.Text = CData.Opt.dPlaceGap.ToString();
            txt_PlaceDelay.Text = CData.Opt.iPlaceDelay.ToString();

            ckb_OfpBtmCleanSkip.Checked = CData.Opt.bOfpBtmCleanSkip;
            ckb_OfpUseCenterPos.Checked = CData.Opt.bOfpUseCenterPos;       // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Position 추가
            ckb_OfpCoverDryerSkip.Checked = CData.Opt.bOfpCoverDryerSkip;   // 2021.04.22 lhs Dryer Water Nozzle 사용시 Cover Dryer 역할 Skip 하는 체크박스

            // 2021.07.02 SungTae Start : [추가] 고객사(ASE-KR) 요청으로 Strip Cleaning Mode 추가
            if (CData.Opt.iOfpCleaningMode == (int)eCleanMode.BASIC)    { rdb_ClnModeBasic.Checked = true; }
            else                                                        { rdb_ClnModeRotation.Checked = true; }
            // 2021.07.02 SungTae End
        }

        public void Get()
        {
            int.TryParse(txt_OnpRinseTime_L.Text, out CData.Opt.aOnpRinseTime[(int)EWay.L]);
            int.TryParse(txt_OnpRinseTime_R.Text, out CData.Opt.aOnpRinseTime[(int)EWay.R]);
            int.TryParse(txt_OfpPadCleanTime.Text, out CData.Opt.iOfpPadCleanTime);

            // 200330 mjy : 신규 추가
            double.TryParse(txt_PickGap.Text, out CData.Opt.dPickGap);
            int.TryParse(txt_PickDelay.Text, out CData.Opt.iPickDelay);
            double.TryParse(txt_PlaceGap.Text, out CData.Opt.dPlaceGap);
            int.TryParse(txt_PlaceDelay.Text, out CData.Opt.iPlaceDelay);

            CData.Opt.bOfpBtmCleanSkip      = ckb_OfpBtmCleanSkip.Checked;
            CData.Opt.bOfpUseCenterPos      = ckb_OfpUseCenterPos.Checked;      // 2021.03.18 SungTae : 고객사(ASE-KR) 요청으로 Center Position Option 추가
            CData.Opt.bOfpCoverDryerSkip    = ckb_OfpCoverDryerSkip.Checked;    // 2021.04.22 lhs Dryer Water Nozzle 사용시 Cover Dryer 역할 Skip 하는 체크박스

            // 2021.07.02 SungTae Start : [추가] 고객사(ASE-KR) 요청으로 Strip Cleaning Mode 추가
            if (rdb_ClnModeBasic.Checked)       { CData.Opt.iOfpCleaningMode = (int)eCleanMode.BASIC; }
            if (rdb_ClnModeRotation.Checked)    { CData.Opt.iOfpCleaningMode = (int)eCleanMode.ROTATION; }
            // 2021.07.02 SungTae End
        }

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
        }

        #endregion
        
        #region Private method
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
    }
}
