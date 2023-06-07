using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using Microsoft.VisualBasic.FileIO;

namespace SG2000X
{
    public partial class vwDev_02Prm_6Etc2 : UserControl
    {
        //210825 pjh : Skyworks Device에 Wheel Parameter 추가
        private EWay m_eWy;
        private tWhl m_tWhl;
        private int m_iLeft;
        private int m_iRight;
        //
        public vwDev_02Prm_6Etc2()
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

            if (CDataOption.eOnpClean == eOnpClean.Use)
            {
                pnl_OnpCnt.Visible = true;
            }

            if (CDataOption.Dryer == eDryer.Knife)
            {
                ckb_UseDry.Visible = false;

                lbl_DryVel.Visible = false;
                txt_DryVel.Visible = false;
                lbl_DryVel_U.Visible = false;

                lbl_DryCnt.Text = "Dry Knife Count";

                grb_DryDir.Visible = false;

#if true //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 연장
                lbl_DryTime.Visible     = false;
                txt_DryTime.Visible     = false;
                lbl_DryTime_U.Visible   = false;
                lbl_DryTimeDesc.Visible = false;
#endif
            }

            // 2021.03.30 lhs Start  : // Dryer Water clean시의 회전속도, 회전카운트, 시간
            if (CDataOption.Dryer == eDryer.Rotate && CDataOption.UseDryWtNozzle)   { grpDryWtNozzle.Visible = true;  }
            else                                                                    { grpDryWtNozzle.Visible = false; }
            // 2021.03.30 lhs End

            // 2020.12.16 JSKim St
            // SCK+에서도 Picker Place Delay 시간은 옵션으로 처리해달라고 함...
            // SCK+ 적용 사항은 SCK에도 해당 됨
            if (CDataOption.Package == ePkg.Unit || CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            // 2020.12.16 JSKim Ed
            {
                lblP_OnpDlyPlaceT.Visible = true;
                lblP_OnpDlyPlaceU.Visible = true;
                txtP_OnpDlyPlace.Visible = true;
                //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
                if (CDataOption.PickerImprove == ePickerTimeImprove.Use)
                {
                    lblP_OfpDlyPlaceT.Visible = true;
                    txtP_OfpDlyPlace.Visible = true;
                    lblP_OfpDlyPlaceU.Visible = true;
                }
                //
            }

            //200121 ksg : , 200625 lks
            if (CData.CurCompany == ECompany.SCK  ||
                CData.CurCompany == ECompany.JSCK ||
                CData.CurCompany == ECompany.JCET)
            { pnl_ShiftData.Visible = true; }
            else
            { pnl_ShiftData.Visible = false; }

            // Data Shift Probe Skip
            if (CData.CurCompany == ECompany.ASE_KR)
            { pnl_DShiftPSkip.Visible = true; }
            else
            { pnl_DShiftPSkip.Visible = false; }

            // 200416 pjh : Grinding Strip Start Limit Visible, 200625 lks
            if (CData.CurCompany == ECompany.SCK  || 
                CData.CurCompany == ECompany.JSCK ||  
                CData.CurCompany == ECompany.JCET || 
                CData.CurCompany == ECompany.Suhwoo)
            { pnl_StripStartLimit.Visible = true; }
            else
            { pnl_StripStartLimit.Visible = false; }

            //190309 ksg :
            if (CData.CurCompany == ECompany.SkyWorks)
            { ckb_OcrSkip.Visible = true; }
            else
            { ckb_OcrSkip.Visible = false; }

            // 200708 jym : Fake 데이터 추가
            //syc : Qorvo After Measure
            //pnl_Fake.Visible = CDataOption.IsFakeAf;
            pnl_Fake.Visible = false;
            if (CDataOption.Package == ePkg.Unit)
            {
                pnl_Fake.Visible = CDataOption.IsFakeAf;
            }

            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            _set_Ckb_OriOneTimeSkipUse_Visible();
            //

            //211015 pjh : Replace Air Cut 사용 시 표시
            if (CDataOption.IsDrsAirCutReplace)
            {
                label95.Text = "Dresser Information(Using)";
                panel13.Visible = true;
                label32.Text = "Dresser Information(Replace)";

                label45.Text = "Dresser Information(Using)";
                panel7.Visible = true;
                label105.Text = "Dresser Information(Replace)";
            }
            else
            {
                label95.Text = "Dresser Information";
                panel13.Visible = false;
                label32.Text = "Dresser Information(Replace)";

                label84.Text = "Dresser Information";
                panel7.Visible = false;
                label91.Text = "Dresser Information(Replace)";
            }
        }

        #region Private method
        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
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

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        private void SetWriteLanguage(string controlName, string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            string sName = "";

            Set();

            if (CData.CurCompany == ECompany.Qorvo    || 
                CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가 
                CData.CurCompany == ECompany.SST      || 
                CData.CurCompany == ECompany.ASE_KR) //200107 syc :
            {
#if true //200713 jhc
                ckb_BCRSecondUse.Visible = true; //190817 ksg :
                if (CData.CurCompany == ECompany.ASE_KR)
                { ckb_OcrMarkSkip.Visible = false; }
                else
                { ckb_OcrMarkSkip.Visible = true; }
#else
                if (CData.CurCompany != ECompany.ASE_KR)
                {
                    ckbP_EtBCRSecondUse.Visible = true; //190817 ksg :
                    ckbP_EtOcrMarkSkip.Visible = true;
                }

                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    ckbP_EtBCRSecondUse.Visible = false; //190817 ksg :
                    ckbP_EtOcrMarkSkip.Visible = false;
                }
#endif                
            }
            else
            {
                ckb_BCRSecondUse.Visible = false; //190817 ksg :
                ckb_OcrMarkSkip.Visible = false;
            }

            panel1.Location = new Point(3, 3);
            panel4.Location = new Point(3, 3);

            //210825 pjh : Skyworks Device에 Wheel Parameter 추가
            if (m_eWy == EWay.L && CData.Dev.aData[(int)EWay.L].sSelWhl != "")
            {
                //sName = CData.Dev.aData[(int)EWay.L].sSelWhl;
                sName = CData.Whls[(int)EWay.L].sWhlName;
                lbxL_List_Dev.SelectedItem = sName;
            }
            else if (m_eWy == EWay.R && CData.Dev.aData[(int)EWay.R].sSelWhl != "")
            {
                //sName = CData.Dev.aData[(int)EWay.R].sSelWhl;
                sName = CData.Whls[(int)EWay.R].sWhlName;
                lbxR_List_Dev.SelectedItem = sName;
            }
                    
            _ListUp();

            if (CDataOption.UseSeperateDresser)
            {
                txtL_DrName.ReadOnly = true;
                txtL_DrName.BackColor = Color.LightGray;
                txtR_DrName.ReadOnly = true;
                txtR_DrName.BackColor = Color.LightGray;
            }

            _Load();

            // 2020.10.26 SungTae : Add
            ViewUpdate();

            //syc : new cleaner
            NewClnView(CDataOption.UseNewClenaer);
        }
        //syc : new cleaner
        public void NewClnView(bool bret)
        {

            label235.Visible = bret;
            label234.Visible = bret;
            txt_LSpgSpd.Visible = bret;
            txt_LSpgCnt.Visible = bret;
            label344.Visible = bret;
            label343.Visible = bret;
            label18.Visible = bret;

            label237.Visible = bret;
            label236.Visible = bret;
            txtP_EtRSpgSpd.Visible = bret;
            txtP_EtRSpgCnt.Visible = bret;
            label345.Visible = bret;
            label346.Visible = bret;
            label22.Visible = bret;
        }

        // 2020.10.26 SungTae Add : Add
        public void ViewUpdate()
        {
            if(CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
               CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            {
                label19.Visible = true;
                label17.Visible = true;
                label20.Visible = true;
                label21.Visible = true;

                txt_OffMgzCnt.Visible   = true;
                txt_OffMgzPitch.Visible = true;

                label209.Text = "On-loader Slot Count";
                label202.Text = "On-loader Pitch Size";
                label19.Text = "Off-loader Unload Strip Count";
                label17.Text = "Off-loader Pitch Size";
            }
            else
            {
                label19.Visible = false;
                label17.Visible = false;
                label20.Visible = false;
                label21.Visible = false;

                txt_OffMgzCnt.Visible = false;
                txt_OffMgzPitch.Visible = false;

                label209.Text = "Slot Count";
                label202.Text = "Pitch Size";
            }

            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)
            if (CData.CurCompany == ECompany.ASE_KR)    pnl_SetTblCln.Visible = true;
            else                                        pnl_SetTblCln.Visible = false;
            // 2021.03.02 SungTae End
        }
        // 2020.10.26 SungTae End

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// 데이터를 화면에 출력
        /// </summary>
        public void Set()
        {
            txt_MgzCnt  .Text    = CData.Dev.iMgzCnt  .ToString();
            txt_MgzPitch.Text    = CData.Dev.dMgzPitch.ToString();

            // 2020.10.26 SungTae Start : Add by Qorvo(Strip 분리 배출)
            txt_OffMgzCnt.Text   = CData.Dev.iOffMgzCnt.ToString();
            txt_OffMgzPitch.Text = CData.Dev.dOffMgzPitch.ToString();
            // 2020.10.26 SungTae End

            ckb_UseDry.Checked = CData.Dev.bDryTop;
            txt_DryVel  .Text    = CData.Dev.iDryRPM  .ToString();
            txt_DryCnt  .Text    = CData.Dev.iDryCnt  .ToString();
            if (CData.Dev.eDryDir == eDryDir.CCW)
            { rdb_CCW.Checked = true; }
            else
            { rdb_CW.Checked = true; }
            txt_PushF.Text = CData.Dev.dPushF         .ToString();
            txt_PushS.Text = CData.Dev.dPushS         .ToString();
            txt_BtClP.Text = CData.Dev.iOffpClean     .ToString();
            txt_BtClS.Text = CData.Dev.iOffpCleanStrip.ToString();

            txt_DryWtNozzleVel.Text = CData.Dev.iDryWtNozzleRPM.ToString(); // 2021.03.30 lhs 
            txt_DryWtNozzleCnt.Text = CData.Dev.iDryWtNozzleCnt.ToString(); // 2021.03.30 lhs 

            txt_LWknSpd .Text = CData.Dev.aData[0].dTpBubSpd .ToString();
            txt_LWknCnt .Text = CData.Dev.aData[0].iTpBubCnt .ToString();
            txt_LAirSpd .Text = CData.Dev.aData[0].dTpAirSpd .ToString();
            txt_LAirCnt .Text = CData.Dev.aData[0].iTpAirCnt .ToString();
            txt_LSpgSpd .Text = CData.Dev.aData[0].dTpSpnSpd .ToString();
            txt_LSpgCnt .Text = CData.Dev.aData[0].iTpSpnCnt .ToString();
            txtP_EtLPbOfset.Text = CData.Dev.aData[0].dPrbOffset.ToString(); //190502 ksg :

            txt_RWknSpd .Text = CData.Dev.aData[1].dTpBubSpd .ToString();
            txt_RWknCnt .Text = CData.Dev.aData[1].iTpBubCnt .ToString();
            txtP_EtRAirSpd .Text = CData.Dev.aData[1].dTpAirSpd .ToString();
            txtP_EtRAirCnt .Text = CData.Dev.aData[1].iTpAirCnt .ToString();
            txtP_EtRSpgSpd .Text = CData.Dev.aData[1].dTpSpnSpd .ToString();
            txtP_EtRSpgCnt .Text = CData.Dev.aData[1].iTpSpnCnt .ToString();
            txtP_EtRPbOfset.Text = CData.Dev.aData[1].dPrbOffset.ToString(); //190502 ksg :

            txtP_OnpDlyPick .Text = CData.Dev.iPickDelayOn .ToString();
            txtP_OnpDlyPlace.Text = CData.Dev.iPlaceDelayOn.ToString();
            txtP_OfpDlyPick.Text = CData.Dev.iPickDelayOff.ToString();
            txtP_OfpDlyPlace.Text = CData.Dev.iPlaceDelayOff.ToString(); //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용

            //20190611 ghk_onpclean
            txtP_EtOnpCnt.Text = CData.Dev.iOnpCleanCnt.ToString();
            //
            txtP_EtReNumMax.Text = CData.Dev.iReDoNumMax    .ToString(); //200408 ksg:
            txtP_EtReCntMax.Text = CData.Dev.iReDoCntMax    .ToString(); //200408 ksg:
            //190211
            //
            txtp_StripStartLimit.Text = CData.Dev.dStripStartLimit.ToString(); //200416 pjh:
            //
            ckb_BCRSkip    .Checked = CData.Dev.bBcrSkip    ;
            ckb_OriSkip    .Checked = CData.Dev.bOriSkip    ;
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            if (CData.Dev.bOriSkip)
            {
                CData.Dev.bOriOneTimeSkipUse = false;   //Orientation 검사 Skip인 경우 Orientation One Time Skip 설정 무의미 (Device 옵션)
                ckb_OriOneTimeSkipUse.Enabled = false;
                CData.bOriOneTimeSkip = false;          //Orientation One Time Skip 설정 초기화 (현재 설정 상태)
            }
            else
            {
                ckb_OriOneTimeSkipUse.Enabled = true;
            }
            ckb_OriOneTimeSkipUse.Checked = CData.Dev.bOriOneTimeSkipUse;
            //
            ckb_OcrSkip    .Checked = CData.Dev.bOcrSkip    ; //190309 ksg :
            ckbP_EtDataShift  .Checked = CData.Dev.bDataShift  ; //190610 ksg :
            ckbP_EtDShiftPSkip.Checked = CData.Dev.bDShiftPSkip; //200325 ksg : Data Shift Probe Skip

            ckbP_StripStart   .Checked = CData.Dev.bStripStartLimit; //200416 pjh : Strip Start Limit Skip
            //190817 ksg :
            if(CData.Dev.bBcrSkip)
            {
                CData.Dev.bBcrSecondSkip    = true;
                ckb_BCRSecondUse.Checked = true;
            }
            else
            {
                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST || CData.CurCompany == ECompany.ASE_KR) //200713 jhc : //191202 ksg :
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                    CData.CurCompany == ECompany.SST || CData.CurCompany == ECompany.ASE_KR)
                {
                    ckb_BCRSecondUse.Checked = CData.Dev.bBcrSecondSkip;
                    ckb_BCRSecondUse.Visible = true;
                }
                else 
                {
                    ckb_BCRSecondUse.Checked = true ;
                    ckb_BCRSecondUse.Visible = false;
                    //CData.Dev.bBcrSecondSkip    = false;
                    CData.Dev.bBcrSecondSkip    = true; //191005 ksg :
                }
                
            }

            ckb_OcrMarkSkip.Checked = CData.Dev.bOriMarkedSkip;

            //20190625 ghk_dfserver
            ckb_BCRKeyInSkip.Checked = CData.Dev.bBcrKeyInSkip;

            if (CData.Dev.aFake != null)
            {
                txt_FakeL.Text = CData.Dev.aFake[(int)EWay.L].ToString();
                txt_FakeR.Text = CData.Dev.aFake[(int)EWay.R].ToString();
            }

            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE - KR)
            if( CData.CurCompany == ECompany.ASE_KR)
            {
                if (CData.Dev.aTblCleanVel != null)
                {
                    txtP_LTblClnVel.Text = CData.Dev.aTblCleanVel[(int)EWay.L].ToString();
                    txtP_RTblClnVel.Text = CData.Dev.aTblCleanVel[(int)EWay.R].ToString();
                }
            }
            // 2021.03.02 SungTae End

            if(CDataOption.UseDeviceWheel)
            {
                if (txtL_DrOuter.Text == "0")
                {
                    CData.Drs[0].dDrsOuter = m_tWhl.dDrsOuter;
                }
                if (txtR_DrOuter.Text == "0")
                {
                    CData.Drs[1].dDrsOuter = m_tWhl.dDrsOuter;
                }
            }
        }

        public void Get()
        {
            int   .TryParse(txt_MgzCnt  .Text, out CData.Dev.iMgzCnt  );
            double.TryParse(txt_MgzPitch.Text, out CData.Dev.dMgzPitch);

            // 2020.10.26 SungTae Start : Add by Qorvo(Strip 분리 배출)
            int   .TryParse(txt_OffMgzCnt  .Text, out CData.Dev.iOffMgzCnt  );
            double.TryParse(txt_OffMgzPitch.Text, out CData.Dev.dOffMgzPitch);
            // 2020.10.26 SungTae End

            CData.Dev.bDryTop = ckb_UseDry.Checked;
            int   .TryParse(txt_DryVel.Text  , out CData.Dev.iDryRPM);
            int   .TryParse(txt_DryCnt.Text  , out CData.Dev.iDryCnt);
            if (rdb_CCW.Checked)
            { CData.Dev.eDryDir = eDryDir.CCW; }
            else
            { CData.Dev.eDryDir = eDryDir.CW; }
            double.TryParse(txt_PushF.Text, out CData.Dev.dPushF         );
            double.TryParse(txt_PushS.Text, out CData.Dev.dPushS         );
            int   .TryParse(txt_BtClP.Text, out CData.Dev.iOffpClean     );
            int   .TryParse(txt_BtClS.Text, out CData.Dev.iOffpCleanStrip);

            int.TryParse(txt_DryWtNozzleVel.Text, out CData.Dev.iDryWtNozzleRPM);  // 2021.03.30 lhs
            int.TryParse(txt_DryWtNozzleCnt.Text, out CData.Dev.iDryWtNozzleCnt);  // 2021.03.30 lhs

            double.TryParse(txt_LWknSpd .Text, out CData.Dev.aData[0].dTpBubSpd );
            //syc : new cleaner val
            if      (CData.Dev.aData[0].dTpBubSpd <  10) { CData.Dev.aData[0].dTpBubSpd =  10; }
            else if (CData.Dev.aData[0].dTpBubSpd > 200) { CData.Dev.aData[0].dTpBubSpd = 200; }
            //
            int   .TryParse(txt_LWknCnt .Text, out CData.Dev.aData[0].iTpBubCnt );
            //syc : new cleaner val
            double.TryParse(txt_LAirSpd .Text, out CData.Dev.aData[0].dTpAirSpd );
            if      (CData.Dev.aData[0].dTpAirSpd <  10) { CData.Dev.aData[0].dTpAirSpd =  10; }
            else if (CData.Dev.aData[0].dTpAirSpd > 200) { CData.Dev.aData[0].dTpAirSpd = 200; }
            //
            int   .TryParse(txt_LAirCnt .Text, out CData.Dev.aData[0].iTpAirCnt );
            //syc : new cleaner val
            double.TryParse(txt_LSpgSpd .Text, out CData.Dev.aData[0].dTpSpnSpd );
            if      (CData.Dev.aData[0].dTpSpnSpd <  10) { CData.Dev.aData[0].dTpSpnSpd =  10; }
            else if (CData.Dev.aData[0].dTpSpnSpd > 200) { CData.Dev.aData[0].dTpSpnSpd = 200; }
            //
            int.TryParse(txt_LSpgCnt .Text, out CData.Dev.aData[0].iTpSpnCnt );
            double.TryParse(txtP_EtLPbOfset.Text, out CData.Dev.aData[0].dPrbOffset);  //190502 ksg :

            //syc : new cleaner val
            double.TryParse(txt_RWknSpd .Text, out CData.Dev.aData[1].dTpBubSpd );
            if      (CData.Dev.aData[1].dTpBubSpd <  10) { CData.Dev.aData[1].dTpBubSpd =  10; }
            else if (CData.Dev.aData[1].dTpBubSpd > 200) { CData.Dev.aData[1].dTpBubSpd = 200; }
            //
            int   .TryParse(txt_RWknCnt .Text, out CData.Dev.aData[1].iTpBubCnt );
            //syc : new cleaner val
            double.TryParse(txtP_EtRAirSpd .Text, out CData.Dev.aData[1].dTpAirSpd );
            if      (CData.Dev.aData[1].dTpAirSpd <  10) { CData.Dev.aData[1].dTpAirSpd =  10; }
            else if (CData.Dev.aData[1].dTpAirSpd > 200) { CData.Dev.aData[1].dTpAirSpd = 200; }
            //
            int   .TryParse(txtP_EtRAirCnt .Text, out CData.Dev.aData[1].iTpAirCnt );
            //syc : new cleaner val
            double.TryParse(txtP_EtRSpgSpd .Text, out CData.Dev.aData[1].dTpSpnSpd );
            if      (CData.Dev.aData[1].dTpSpnSpd <  10) { CData.Dev.aData[1].dTpSpnSpd =  10; }
            else if (CData.Dev.aData[1].dTpSpnSpd > 200) { CData.Dev.aData[1].dTpSpnSpd = 200; }
            //
            int   .TryParse(txtP_EtRSpgCnt .Text, out CData.Dev.aData[1].iTpSpnCnt );
            double.TryParse(txtP_EtRPbOfset.Text, out CData.Dev.aData[1].dPrbOffset); //190502 ksg :

            int.TryParse(txtP_OnpDlyPick .Text, out CData.Dev.iPickDelayOn );
            int.TryParse(txtP_OnpDlyPlace.Text, out CData.Dev.iPlaceDelayOn);
            int.TryParse(txtP_OfpDlyPick.Text, out CData.Dev.iPickDelayOff);
            int.TryParse(txtP_OfpDlyPlace.Text, out CData.Dev.iPlaceDelayOff); //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용

            //20190611 ghk_onpclean
            int.TryParse(txtP_EtOnpCnt.Text, out CData.Dev.iOnpCleanCnt);
            //
            int.TryParse(txtP_EtReNumMax.Text, out CData.Dev.iReDoNumMax    ); //200408 ksg :
            int.TryParse(txtP_EtReCntMax.Text, out CData.Dev.iReDoCntMax    ); //200408 ksg :
            //
            double.TryParse(txtp_StripStartLimit.Text, out CData.Dev.dStripStartLimit); //200416 pjh:
            //190211 ksg :
            CData.Dev.bBcrSkip       = ckb_BCRSkip     .Checked;
            CData.Dev.bOriSkip       = ckb_OriSkip     .Checked;
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            CData.Dev.bOriOneTimeSkipUse = ckb_OriOneTimeSkipUse.Checked;
            //
            CData.Dev.bOcrSkip       = ckb_OcrSkip     .Checked;
            CData.Dev.bDataShift     = ckbP_EtDataShift   .Checked; //190610 ksg :
            CData.Dev.bBcrSecondSkip = ckb_BCRSecondUse.Checked; //190817 ksg :
            CData.Dev.bOriMarkedSkip = ckb_OcrMarkSkip .Checked;
            CData.Dev.bDShiftPSkip   = ckbP_EtDShiftPSkip .Checked; //200325 ksg : Data Shift Probe Skip

            //20190625 ghk_dfserver
            CData.Dev.bBcrKeyInSkip = ckb_BCRKeyInSkip.Checked;

            CData.Dev.bStripStartLimit = ckbP_StripStart.Checked; //200416 pjh : 200416 pjh : Strip Start Limit Skip

            CData.Dev.aFake = new double[2];
            double.TryParse(txt_FakeL.Text, out CData.Dev.aFake[(int)EWay.L]);
            double.TryParse(txt_FakeR.Text, out CData.Dev.aFake[(int)EWay.R]);

            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Dev.aTblCleanVel = new double[2];
                double.TryParse(txtP_LTblClnVel.Text, out CData.Dev.aTblCleanVel[(int)EWay.L]);
                double.TryParse(txtP_RTblClnVel.Text, out CData.Dev.aTblCleanVel[(int)EWay.R]);
            }

            //210826 pjh : Wheel 정보도 저장
            _Get();
        }
        #endregion

        private void ckbP_EtBCRSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_BCRSkip.Checked)
            {
                ckb_BCRKeyInSkip.Checked = true ;
                ckb_BCRKeyInSkip.Enabled = false;
                ckb_BCRSecondUse.Checked = false; //190817 ksg :
                //ckbP_EtBCRSecondUse.Enabled = false; //190817 ksg :
            }
            else
            {
                ckb_BCRKeyInSkip.Enabled = true;
                ckb_BCRSecondUse.Enabled = true; //190817 ksg :
            }
        }


#if true //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 계산
        private void txt_DryVel_TextChanged(object sender, EventArgs e)
        {
            if (CDataOption.Dryer == eDryer.Rotate)
            { calc_DryTime(); } //회전 동작 시간 계산하여 자동으로 표시
        }

        private void txt_DryCnt_TextChanged(object sender, EventArgs e)
        {
            if (CDataOption.Dryer == eDryer.Rotate)
            { calc_DryTime(); } //회전 동작 시간 계산하여 자동으로 표시
        }

        //회전 동작 시간 계산하여 자동으로 표시
        private void calc_DryTime()
        {
            int rpm = 10;
            int cnt = 1;
            int.TryParse(txt_DryVel.Text, out rpm);
            int.TryParse(txt_DryCnt.Text, out cnt);
            if( rpm == 0 ) rpm = 10;
            if( cnt == 0 ) cnt = 1;
            double rotationTime = (60.0/(double)rpm)*(double)cnt;
            string strTm = string.Format("{0:0.0}", rotationTime);
            txt_DryTime.Text = strTm;
        }

        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        private void ckb_OriSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_OriSkip.Checked)
            {
                ckb_OriOneTimeSkipUse.Checked = false;
                ckb_OriOneTimeSkipUse.Enabled = false;
            }
            else
            {
                ckb_OriOneTimeSkipUse.Enabled = true;
            }
        }

        private void _set_Ckb_OriOneTimeSkipUse_Visible()
        {
            ckb_OriOneTimeSkipUse.Visible = GV.ORIENTATION_ONE_TIME_SKIP;
        }

        private void txt_DryWtNozzleVel_TextChanged(object sender, EventArgs e)
        {
            if (CDataOption.Dryer == eDryer.Rotate)
            { calc_DryWaterTime(); } //회전 동작 시간 계산하여 자동으로 표시
        }

        private void txt_DryWtNozzleCnt_TextChanged(object sender, EventArgs e)
        {
            if (CDataOption.Dryer == eDryer.Rotate)
            { calc_DryWaterTime(); } //회전 동작 시간 계산하여 자동으로 표시
        }

        private void calc_DryWaterTime()
        {
            int rpm = 10;
            int cnt = 1;
            int.TryParse(txt_DryWtNozzleVel.Text, out rpm);
            int.TryParse(txt_DryWtNozzleCnt.Text, out cnt);
            if (rpm == 0) rpm = 10;
            if (cnt == 0) cnt = 1;
            double rotationTime = (60.0 / (double)rpm) * (double)cnt;
            string strTm = string.Format("{0:0.0}", rotationTime);
            txt_DryWtNozzleTime.Text = strTm;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

        }
        //210827 pjh : Device Wheel Parameter
        #region 
        //
        private void _ListUp()
        {
            string sPath = GV.PATH_WHEEL;
            if (m_eWy == EWay.L)
            {
                lbxL_List_Dev.Items.Clear();
                sPath += "Left\\";

                if (Directory.Exists(sPath))
                {
                    DirectoryInfo mFile = new DirectoryInfo(sPath);
                    //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                    foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                    {
                        //lbxL_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                        lbxL_List_Dev.Items.Add(mInfo.Name);
                    }
                    //lbxL_List_Dev.SelectedItem = CData.Dev.aData[(int)EWay.L].sSelWhl; //211118 pjh :
					lbxL_List_Dev.SelectedItem = CData.Whls[(int)EWay.L].sWhlName;
                }
                else
                { Directory.CreateDirectory(sPath); }
            }
            else
            {
                lbxR_List_Dev.Items.Clear();
                sPath = GV.PATH_WHEEL + "Right\\";

                if (Directory.Exists(sPath))
                {
                    DirectoryInfo mFile = new DirectoryInfo(sPath);
                    //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                    foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                    {
                        //lbxR_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                        lbxR_List_Dev.Items.Add(mInfo.Name);
                    }
                    //lbxR_List_Dev.SelectedItem = CData.Dev.aData[(int)EWay.R].sSelWhl; //211118 pjh :
					lbxR_List_Dev.SelectedItem = CData.Whls[(int)EWay.R].sWhlName;
                }
                else
                { Directory.CreateDirectory(sPath); }
            }
        }
        //211015 pjh : Wheel Dressing Parameter Get
        private void _Get()
        {
            if (m_eWy == EWay.L)
            {
                // Wheel information
                m_tWhl.sWhlName = txtL_Name.Text;
                double.TryParse(txtL_WhOuter.Text, out m_tWhl.dWhlO);
                double.TryParse(txtL_WhTip.Text, out m_tWhl.dWhltH);
                int.TryParse(txtL_WhToCnt.Text, out m_tWhl.iGtc);
                int.TryParse(txtL_WhDrCnt.Text, out m_tWhl.iGdc);
                double.TryParse(txtL_WhToLoss.Text, out m_tWhl.dWhltL);
                double.TryParse(txtL_WhStLoss.Text, out m_tWhl.dWhloL);
                double.TryParse(txtL_WhDrLoss.Text, out m_tWhl.dWhldoL);
                double.TryParse(txtL_WhCyLoss.Text, out m_tWhl.dWhldcL);
                //double.TryParse(txtL_Air.Text, out CData.Whls[0].dDair);
                // 2020.11.23 JSKim St
                //double.TryParse(txtL_Air_Rep.Text, out CData.Whls[0].dDairRep);
                // 2020.11.23 JSKim Ed

                if (Convert.ToDouble(txtL_WhlLimit.Text) < 1.0)
                { txtL_WhlLimit.Text = "1"; }
                if (Convert.ToDouble(txtL_WhlLimit.Text) > GV.WHEEL_LIMIT_MAX)
                { txtL_WhlLimit.Text = GV.WHEEL_LIMIT_MAX.ToString(); }
                double.TryParse(txtL_WhlLimit.Text, out m_tWhl.dWhlLimit);

                // Dresser information
                if(CDataOption.UseSeperateDresser)
                {
                    double.TryParse(txtL_DrOuter.Text, out CData.Drs[0].dDrsOuter);
                    double.TryParse(txtL_DrHei.Text, out CData.Drs[0].dDrsH);
                    m_tWhl.dDrsAf = CData.Drs[0].dDrsH;
                }
                else
                {
                    m_tWhl.sDrsName = txtL_DrName.Text;
                    double.TryParse(txtL_DrOuter.Text, out m_tWhl.dDrsOuter);
                    double.TryParse(txtL_DrHei.Text, out m_tWhl.dDrsH);
                }
                if (Convert.ToDouble(txtL_DrsLimit.Text) < 1.0)
                { txtL_DrsLimit.Text = "1"; }
                if (Convert.ToDouble(txtL_DrsLimit.Text) > GV.DRESSER_LIMIT_MAX)
                { txtL_DrsLimit.Text = GV.DRESSER_LIMIT_MAX.ToString(); }
                double.TryParse(txtL_DrsLimit.Text, out m_tWhl.dDrsLimit);

                if (CDev.It.a_tWhl[0].aUsedP == null)
                {
                    //211012 pjh : Dressing Air Cut
                    double.TryParse(txtL_Air.Text, out m_tWhl.dDair);

                    //211013 pjh : Replace Dressing Parameter
                    if (CDataOption.IsDrsAirCutReplace)
                    { 
                        double.TryParse(txtL_Air_Rep.Text, out m_tWhl.dDairRep); 
                    }
                    //

                    // Using 01
                    m_tWhl.aUsedP[0].eMode = (eStepMode)cmbL_U1Mod.SelectedIndex;
                    double.TryParse(txtL_U1To.Text, out m_tWhl.aUsedP[0].dTotalDep);
                    double.TryParse(txtL_U1Cy.Text, out m_tWhl.aUsedP[0].dCycleDep);
                    double.TryParse(txtL_U1Tb.Text, out m_tWhl.aUsedP[0].dTblSpd);
                    int.TryParse(txtL_U1Sp.Text, out m_tWhl.aUsedP[0].iSplSpd);
                    m_tWhl.aUsedP[0].eDir = (eStartDir)cmbL_U1Dir.SelectedIndex;

                    // Using 02
                    m_tWhl.aUsedP[1].eMode = (eStepMode)cmbL_U2Mod.SelectedIndex;
                    double.TryParse(txtL_U2To.Text, out m_tWhl.aUsedP[1].dTotalDep);
                    double.TryParse(txtL_U2Cy.Text, out m_tWhl.aUsedP[1].dCycleDep);
                    double.TryParse(txtL_U2Tb.Text, out m_tWhl.aUsedP[1].dTblSpd);
                    int.TryParse(txtL_U2Sp.Text, out m_tWhl.aUsedP[1].iSplSpd);
                    m_tWhl.aUsedP[1].eDir = (eStartDir)cmbL_U2Dir.SelectedIndex;

                    // Replace 01
                    m_tWhl.aNewP[0].eMode = (eStepMode)cmbL_R1Mod.SelectedIndex;
                    double.TryParse(txtL_R1To.Text, out m_tWhl.aNewP[0].dTotalDep);
                    double.TryParse(txtL_R1Cy.Text, out m_tWhl.aNewP[0].dCycleDep);
                    double.TryParse(txtL_R1Tb.Text, out m_tWhl.aNewP[0].dTblSpd);
                    int.TryParse(txtL_R1Sp.Text, out m_tWhl.aNewP[0].iSplSpd);
                    m_tWhl.aNewP[0].eDir = (eStartDir)cmbL_R1Dir.SelectedIndex;

                    // Replace 02
                    m_tWhl.aNewP[1].eMode = (eStepMode)cmbL_R2Mod.SelectedIndex;
                    double.TryParse(txtL_R2To.Text, out m_tWhl.aNewP[1].dTotalDep);
                    double.TryParse(txtL_R2Cy.Text, out m_tWhl.aNewP[1].dCycleDep);
                    double.TryParse(txtL_R2Tb.Text, out m_tWhl.aNewP[1].dTblSpd);
                    int.TryParse(txtL_R2Sp.Text, out m_tWhl.aNewP[1].iSplSpd);
                    m_tWhl.aNewP[1].eDir = (eStartDir)cmbL_R2Dir.SelectedIndex;
                }
                else
                {
                    //211012 pjh : Dressing Air Cut
                    double.TryParse(txtL_Air.Text, out CDev.It.a_tWhl[0].dDair);

                    //211013 pjh : Replace Dressing Parameter
                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        double.TryParse(txtL_Air_Rep.Text, out CDev.It.a_tWhl[0].dDairRep);
                    }
                    //

                    // Using 01
                    CDev.It.a_tWhl[0].aUsedP[0].eMode = (eStepMode)cmbL_U1Mod.SelectedIndex;
                    double.TryParse(txtL_U1To.Text, out CDev.It.a_tWhl[0].aUsedP[0].dTotalDep);
                    double.TryParse(txtL_U1Cy.Text, out CDev.It.a_tWhl[0].aUsedP[0].dCycleDep);
                    double.TryParse(txtL_U1Tb.Text, out CDev.It.a_tWhl[0].aUsedP[0].dTblSpd);
                    int.TryParse(txtL_U1Sp.Text, out CDev.It.a_tWhl[0].aUsedP[0].iSplSpd);
                    CDev.It.a_tWhl[0].aUsedP[0].eDir = (eStartDir)cmbL_U1Dir.SelectedIndex;

                    // Using 02
                    CDev.It.a_tWhl[0].aUsedP[1].eMode = (eStepMode)cmbL_U2Mod.SelectedIndex;
                    double.TryParse(txtL_U2To.Text, out CDev.It.a_tWhl[0].aUsedP[1].dTotalDep);
                    double.TryParse(txtL_U2Cy.Text, out CDev.It.a_tWhl[0].aUsedP[1].dCycleDep);
                    double.TryParse(txtL_U2Tb.Text, out CDev.It.a_tWhl[0].aUsedP[1].dTblSpd);
                    int.TryParse(txtL_U2Sp.Text, out CDev.It.a_tWhl[0].aUsedP[1].iSplSpd);
                    CDev.It.a_tWhl[0].aUsedP[1].eDir = (eStartDir)cmbL_U2Dir.SelectedIndex;

                    // Replace 01
                    CDev.It.a_tWhl[0].aNewP[0].eMode = (eStepMode)cmbL_R1Mod.SelectedIndex;
                    double.TryParse(txtL_R1To.Text, out CDev.It.a_tWhl[0].aNewP[0].dTotalDep);
                    double.TryParse(txtL_R1Cy.Text, out CDev.It.a_tWhl[0].aNewP[0].dCycleDep);
                    double.TryParse(txtL_R1Tb.Text, out CDev.It.a_tWhl[0].aNewP[0].dTblSpd);
                    int.TryParse(txtL_R1Sp.Text, out CDev.It.a_tWhl[0].aNewP[0].iSplSpd);
                    CDev.It.a_tWhl[0].aNewP[0].eDir = (eStartDir)cmbL_R1Dir.SelectedIndex;

                    // Replace 02
                    CDev.It.a_tWhl[0].aNewP[1].eMode = (eStepMode)cmbL_R2Mod.SelectedIndex;
                    double.TryParse(txtL_R2To.Text, out CDev.It.a_tWhl[0].aNewP[1].dTotalDep);
                    double.TryParse(txtL_R2Cy.Text, out CDev.It.a_tWhl[0].aNewP[1].dCycleDep);
                    double.TryParse(txtL_R2Tb.Text, out CDev.It.a_tWhl[0].aNewP[1].dTblSpd);
                    int.TryParse(txtL_R2Sp.Text, out CDev.It.a_tWhl[0].aNewP[1].iSplSpd);
                    CDev.It.a_tWhl[0].aNewP[1].eDir = (eStartDir)cmbL_R2Dir.SelectedIndex;

                    //Limit
                    double.TryParse(txtL_WhlLimit.Text, out CDev.It.a_tWhl[0].dWhlLimit);
                    double.TryParse(txtL_DrsLimit.Text, out CDev.It.a_tWhl[0].dDrsLimit);
                }
            }
            else if(m_eWy == EWay.R)
            {
                // Wheel information
                m_tWhl.sWhlName = txtR_Name.Text;
                double.TryParse(txtR_WhOuter.Text, out m_tWhl.dWhlO);
                double.TryParse(txtR_WhTip.Text, out m_tWhl.dWhltH);
                int.TryParse(txtR_WhToCnt.Text, out m_tWhl.iGtc);
                int.TryParse(txtR_WhDrCnt.Text, out m_tWhl.iGdc);
                double.TryParse(txtR_WhToLoss.Text, out m_tWhl.dWhltL);
                double.TryParse(txtR_WhStLoss.Text, out m_tWhl.dWhloL);
                double.TryParse(txtR_WhDrLoss.Text, out m_tWhl.dWhldoL);
                double.TryParse(txtR_WhCyLoss.Text, out m_tWhl.dWhldcL);
                //double.TryParse(txtR_Air.Text, out CData.Whls[1].dDair);
                // 2020.11.23 JSKim St
                //double.TryParse(txtR_Air_Rep.Text, out CData.Whls[1].dDairRep);
                // 2020.11.23 JSKim Ed

                if (Convert.ToDouble(txtR_WhlLimit.Text) < 1.0)
                { txtR_WhlLimit.Text = "1"; }
                if (Convert.ToDouble(txtR_WhlLimit.Text) > GV.WHEEL_LIMIT_MAX)
                { txtR_WhlLimit.Text = GV.WHEEL_LIMIT_MAX.ToString(); }
                double.TryParse(txtR_WhlLimit.Text, out m_tWhl.dWhlLimit);

                // Dresser information
                if (CDataOption.UseSeperateDresser)
                {
                    if (txtR_DrOuter.Text == "0")
                    {
                        CData.Drs[1].dDrsOuter = m_tWhl.dDrsOuter;
                    }
                    double.TryParse(txtR_DrOuter.Text, out CData.Drs[1].dDrsOuter);
                    double.TryParse(txtR_DrHei.Text, out CData.Drs[1].dDrsH);
                    m_tWhl.dDrsAf = CData.Drs[1].dDrsH;
                }
                else
                {
                    m_tWhl.sDrsName = txtR_DrName.Text;
                    double.TryParse(txtR_DrOuter.Text, out m_tWhl.dDrsOuter);
                    double.TryParse(txtR_DrHei.Text, out m_tWhl.dDrsH);
                }
                if (Convert.ToDouble(txtR_DrsLimit.Text) < 1.0)
                { txtR_DrsLimit.Text = "1"; }
                if (Convert.ToDouble(txtR_DrsLimit.Text) > GV.DRESSER_LIMIT_MAX)
                { txtR_DrsLimit.Text = GV.DRESSER_LIMIT_MAX.ToString(); }
                double.TryParse(txtR_DrsLimit.Text, out m_tWhl.dDrsLimit);

                if (CDev.It.a_tWhl[1].aUsedP == null)
                {
                    //211012 pjh : Dressing Air Cut
                    double.TryParse(txtR_Air.Text, out m_tWhl.dDair);

                    //211013 pjh : Replace Dressing Parameter
                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        double.TryParse(txtR_Air_Rep.Text, out m_tWhl.dDairRep);
                    }
                    //

                    // Using 01
                    m_tWhl.aUsedP[0].eMode = (eStepMode)cmbR_U1Mod.SelectedIndex;
                    double.TryParse(txtR_U1To.Text, out m_tWhl.aUsedP[0].dTotalDep);
                    double.TryParse(txtR_U1Cy.Text, out m_tWhl.aUsedP[0].dCycleDep);
                    double.TryParse(txtR_U1Tb.Text, out m_tWhl.aUsedP[0].dTblSpd);
                    int.TryParse(txtR_U1Sp.Text, out m_tWhl.aUsedP[0].iSplSpd);
                    m_tWhl.aUsedP[0].eDir = (eStartDir)cmbR_U1Dir.SelectedIndex;

                    // Using 02
                    m_tWhl.aUsedP[1].eMode = (eStepMode)cmbR_U2Mod.SelectedIndex;
                    double.TryParse(txtR_U2To.Text, out m_tWhl.aUsedP[1].dTotalDep);
                    double.TryParse(txtR_U2Cy.Text, out m_tWhl.aUsedP[1].dCycleDep);
                    double.TryParse(txtR_U2Tb.Text, out m_tWhl.aUsedP[1].dTblSpd);
                    int.TryParse(txtR_U2Sp.Text, out m_tWhl.aUsedP[1].iSplSpd);
                    m_tWhl.aUsedP[1].eDir = (eStartDir)cmbR_U2Dir.SelectedIndex;

                    // Replace 01
                    m_tWhl.aNewP[0].eMode = (eStepMode)cmbR_R1Mod.SelectedIndex;
                    double.TryParse(txtR_R1To.Text, out m_tWhl.aNewP[0].dTotalDep);
                    double.TryParse(txtR_R1Cy.Text, out m_tWhl.aNewP[0].dCycleDep);
                    double.TryParse(txtR_R1Tb.Text, out m_tWhl.aNewP[0].dTblSpd);
                    int.TryParse(txtR_R1Sp.Text, out m_tWhl.aNewP[0].iSplSpd);
                    m_tWhl.aNewP[0].eDir = (eStartDir)cmbR_R1Dir.SelectedIndex;

                    // Replace 02
                    m_tWhl.aNewP[1].eMode = (eStepMode)cmbR_R2Mod.SelectedIndex;
                    double.TryParse(txtR_R2To.Text, out m_tWhl.aNewP[1].dTotalDep);
                    double.TryParse(txtR_R2Cy.Text, out m_tWhl.aNewP[1].dCycleDep);
                    double.TryParse(txtR_R2Tb.Text, out m_tWhl.aNewP[1].dTblSpd);
                    int.TryParse(txtR_R2Sp.Text, out m_tWhl.aNewP[1].iSplSpd);
                    m_tWhl.aNewP[1].eDir = (eStartDir)cmbR_R2Dir.SelectedIndex;
                }
                else
                {
                    //211012 pjh : Dressing Air Cut
                    double.TryParse(txtR_Air.Text, out CDev.It.a_tWhl[1].dDair);

                    //211013 pjh : Replace Dressing Parameter
                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        double.TryParse(txtR_Air_Rep.Text, out CDev.It.a_tWhl[1].dDairRep);
                    }
                    //

                    // Using 01
                    CDev.It.a_tWhl[1].aUsedP[0].eMode = (eStepMode)cmbR_U1Mod.SelectedIndex;
                    double.TryParse(txtR_U1To.Text, out CDev.It.a_tWhl[1].aUsedP[0].dTotalDep);
                    double.TryParse(txtR_U1Cy.Text, out CDev.It.a_tWhl[1].aUsedP[0].dCycleDep);
                    double.TryParse(txtR_U1Tb.Text, out CDev.It.a_tWhl[1].aUsedP[0].dTblSpd);
                    int.TryParse(txtR_U1Sp.Text, out CDev.It.a_tWhl[1].aUsedP[0].iSplSpd);
                    CDev.It.a_tWhl[1].aUsedP[0].eDir = (eStartDir)cmbR_U1Dir.SelectedIndex;

                    // Using 02
                    CDev.It.a_tWhl[1].aUsedP[1].eMode = (eStepMode)cmbR_U2Mod.SelectedIndex;
                    double.TryParse(txtR_U2To.Text, out CDev.It.a_tWhl[1].aUsedP[1].dTotalDep);
                    double.TryParse(txtR_U2Cy.Text, out CDev.It.a_tWhl[1].aUsedP[1].dCycleDep);
                    double.TryParse(txtR_U2Tb.Text, out CDev.It.a_tWhl[1].aUsedP[1].dTblSpd);
                    int.TryParse(txtL_U2Sp.Text, out CDev.It.a_tWhl[1].aUsedP[1].iSplSpd);
                    CDev.It.a_tWhl[1].aUsedP[1].eDir = (eStartDir)cmbR_U2Dir.SelectedIndex;

                    // Replace 01
                    CDev.It.a_tWhl[1].aNewP[0].eMode = (eStepMode)cmbR_R1Mod.SelectedIndex;
                    double.TryParse(txtR_R1To.Text, out CDev.It.a_tWhl[1].aNewP[0].dTotalDep);
                    double.TryParse(txtR_R1Cy.Text, out CDev.It.a_tWhl[1].aNewP[0].dCycleDep);
                    double.TryParse(txtR_R1Tb.Text, out CDev.It.a_tWhl[1].aNewP[0].dTblSpd);
                    int.TryParse(txtR_R1Sp.Text, out CDev.It.a_tWhl[1].aNewP[0].iSplSpd);
                    CDev.It.a_tWhl[1].aNewP[0].eDir = (eStartDir)cmbR_R1Dir.SelectedIndex;

                    // Replace 02
                    CDev.It.a_tWhl[1].aNewP[1].eMode = (eStepMode)cmbR_R2Mod.SelectedIndex;
                    double.TryParse(txtR_R2To.Text, out CDev.It.a_tWhl[1].aNewP[1].dTotalDep);
                    double.TryParse(txtR_R2Cy.Text, out CDev.It.a_tWhl[1].aNewP[1].dCycleDep);
                    double.TryParse(txtR_R2Tb.Text, out CDev.It.a_tWhl[1].aNewP[1].dTblSpd);
                    int.TryParse(txtL_R2Sp.Text, out CDev.It.a_tWhl[1].aNewP[1].iSplSpd);
                    CDev.It.a_tWhl[1].aNewP[1].eDir = (eStartDir)cmbR_R2Dir.SelectedIndex;

                    //Limit
                    double.TryParse(txtR_WhlLimit.Text, out CDev.It.a_tWhl[1].dWhlLimit);
                    double.TryParse(txtR_DrsLimit.Text, out CDev.It.a_tWhl[1].dDrsLimit);
                }
            }
            else
            {
                CData.Dev.aData[(int)EWay.L].sSelWhl = CData.Whls[0].sWhlName;

                //211012 pjh : Dressing Air Cut
                double.TryParse(txtL_Air.Text, out CData.Whls[0].dDair);

                //211013 pjh : Replace Dressing Parameter
                if (CDataOption.IsDrsAirCutReplace)
                {
                    double.TryParse(txtL_Air_Rep.Text, out CDev.It.a_tWhl[0].dDairRep);
                }
                //

                // Using 01
                CDev.It.a_tWhl[0].aUsedP[0].eMode = (eStepMode)cmbL_U1Mod.SelectedIndex;
                double.TryParse(txtL_U1To.Text, out CDev.It.a_tWhl[0].aUsedP[0].dTotalDep);
                double.TryParse(txtL_U1Cy.Text, out CDev.It.a_tWhl[0].aUsedP[0].dCycleDep);
                double.TryParse(txtL_U1Tb.Text, out CDev.It.a_tWhl[0].aUsedP[0].dTblSpd);
                int.TryParse(txtL_U1Sp.Text, out CDev.It.a_tWhl[0].aUsedP[0].iSplSpd);
                CDev.It.a_tWhl[0].aUsedP[0].eDir = (eStartDir)cmbL_U1Dir.SelectedIndex;

                // Using 02
                CDev.It.a_tWhl[0].aUsedP[1].eMode = (eStepMode)cmbL_U2Mod.SelectedIndex;
                double.TryParse(txtL_U2To.Text, out CDev.It.a_tWhl[0].aUsedP[1].dTotalDep);
                double.TryParse(txtL_U2Cy.Text, out CDev.It.a_tWhl[0].aUsedP[1].dCycleDep);
                double.TryParse(txtL_U2Tb.Text, out CDev.It.a_tWhl[0].aUsedP[1].dTblSpd);
                int.TryParse(txtL_U2Sp.Text, out CDev.It.a_tWhl[0].aUsedP[1].iSplSpd);
                CDev.It.a_tWhl[0].aUsedP[1].eDir = (eStartDir)cmbL_U2Dir.SelectedIndex;

                // Replace 01
                CDev.It.a_tWhl[0].aNewP[0].eMode = (eStepMode)cmbL_R1Mod.SelectedIndex;
                double.TryParse(txtL_R1To.Text, out CDev.It.a_tWhl[0].aNewP[0].dTotalDep);
                double.TryParse(txtL_R1Cy.Text, out CDev.It.a_tWhl[0].aNewP[0].dCycleDep);
                double.TryParse(txtL_R1Tb.Text, out CDev.It.a_tWhl[0].aNewP[0].dTblSpd);
                int.TryParse(txtL_R1Sp.Text, out CDev.It.a_tWhl[0].aNewP[0].iSplSpd);
                CDev.It.a_tWhl[0].aNewP[0].eDir = (eStartDir)cmbL_R1Dir.SelectedIndex;

                // Replace 02
                CDev.It.a_tWhl[0].aNewP[1].eMode = (eStepMode)cmbL_R2Mod.SelectedIndex;
                double.TryParse(txtL_R2To.Text, out CDev.It.a_tWhl[0].aNewP[1].dTotalDep);
                double.TryParse(txtL_R2Cy.Text, out CDev.It.a_tWhl[0].aNewP[1].dCycleDep);
                double.TryParse(txtL_R2Tb.Text, out CDev.It.a_tWhl[0].aNewP[1].dTblSpd);
                int.TryParse(txtL_R2Sp.Text, out CDev.It.a_tWhl[0].aNewP[1].iSplSpd);
                CDev.It.a_tWhl[0].aNewP[1].eDir = (eStartDir)cmbL_R2Dir.SelectedIndex;

                CData.Dev.aData[(int)EWay.R].sSelWhl = CData.Whls[1].sWhlName;

                //211012 pjh : Dressing Air Cut
                double.TryParse(txtR_Air.Text, out CData.Whls[1].dDair);

                //211013 pjh : Replace Dressing Parameter
                if (CDataOption.IsDrsAirCutReplace)
                {
                    double.TryParse(txtR_Air_Rep.Text, out CDev.It.a_tWhl[1].dDairRep);
                }
                //

                // Using 01
                CDev.It.a_tWhl[1].aUsedP[0].eMode = (eStepMode)cmbR_U1Mod.SelectedIndex;
                double.TryParse(txtR_U1To.Text, out CDev.It.a_tWhl[1].aUsedP[0].dTotalDep);
                double.TryParse(txtR_U1Cy.Text, out CDev.It.a_tWhl[1].aUsedP[0].dCycleDep);
                double.TryParse(txtR_U1Tb.Text, out CDev.It.a_tWhl[1].aUsedP[0].dTblSpd);
                int.TryParse(txtR_U1Sp.Text, out CDev.It.a_tWhl[1].aUsedP[0].iSplSpd);
                CDev.It.a_tWhl[1].aUsedP[0].eDir = (eStartDir)cmbR_U1Dir.SelectedIndex;

                // Using 02
                CDev.It.a_tWhl[1].aUsedP[1].eMode = (eStepMode)cmbR_U2Mod.SelectedIndex;
                double.TryParse(txtR_U2To.Text, out CDev.It.a_tWhl[1].aUsedP[1].dTotalDep);
                double.TryParse(txtR_U2Cy.Text, out CDev.It.a_tWhl[1].aUsedP[1].dCycleDep);
                double.TryParse(txtR_U2Tb.Text, out CDev.It.a_tWhl[1].aUsedP[1].dTblSpd);
                int.TryParse(txtR_U2Sp.Text, out CDev.It.a_tWhl[1].aUsedP[1].iSplSpd);
                CDev.It.a_tWhl[1].aUsedP[1].eDir = (eStartDir)cmbR_U2Dir.SelectedIndex;

                // Replace 01
                CDev.It.a_tWhl[1].aNewP[0].eMode = (eStepMode)cmbR_R1Mod.SelectedIndex;
                double.TryParse(txtR_R1To.Text, out CDev.It.a_tWhl[1].aNewP[0].dTotalDep);
                double.TryParse(txtR_R1Cy.Text, out CDev.It.a_tWhl[1].aNewP[0].dCycleDep);
                double.TryParse(txtR_R1Tb.Text, out CDev.It.a_tWhl[1].aNewP[0].dTblSpd);
                int.TryParse(txtR_R1Sp.Text, out CDev.It.a_tWhl[1].aNewP[0].iSplSpd);
                CDev.It.a_tWhl[1].aNewP[0].eDir = (eStartDir)cmbR_R1Dir.SelectedIndex;

                // Replace 02
                CDev.It.a_tWhl[1].aNewP[1].eMode = (eStepMode)cmbR_R2Mod.SelectedIndex;
                double.TryParse(txtR_R2To.Text, out CDev.It.a_tWhl[1].aNewP[1].dTotalDep);
                double.TryParse(txtR_R2Cy.Text, out CDev.It.a_tWhl[1].aNewP[1].dCycleDep);
                double.TryParse(txtR_R2Tb.Text, out CDev.It.a_tWhl[1].aNewP[1].dTblSpd);
                int.TryParse(txtR_R2Sp.Text, out CDev.It.a_tWhl[1].aNewP[1].iSplSpd);
                CDev.It.a_tWhl[1].aNewP[1].eDir = (eStartDir)cmbR_R2Dir.SelectedIndex;
            }
            //
        }

        //211015 pjh : Wheel Dressing Parameter Set
        private void _Set()
        {
            string sPath = GV.PATH_WHEEL;

            if (m_eWy == EWay.L)
            {
                // Wheel information
                txtL_Name.Text = m_tWhl.sWhlName;
                txtL_WhDt.Text = m_tWhl.dtLast.ToString();
                txtL_WhOuter.Text = m_tWhl.dWhlO.ToString();
                txtL_WhTip.Text = m_tWhl.dWhltH.ToString();
                txtL_WhToCnt.Text = m_tWhl.iGtc.ToString();
                txtL_WhDrCnt.Text = m_tWhl.iGdc.ToString();
                txtL_WhToLoss.Text = m_tWhl.dWhltL.ToString();
                txtL_WhStLoss.Text = m_tWhl.dWhloL.ToString();
                txtL_WhDrLoss.Text = m_tWhl.dWhldoL.ToString();
                txtL_WhCyLoss.Text = m_tWhl.dWhldcL.ToString();
                //txtL_Air.Text = m_tWhl.dDair.ToString();
                // 2020.11.23 JSKim St
                //txtL_Air_Rep.Text = m_tWhl.dDairRep.ToString();
                // 2020.11.23 JSKim Ed
                //txtL_WhlLimit.Text = m_tWhl.dWhlLimit.ToString();

                // Dresser information
                if(CDataOption.UseSeperateDresser)
                {
                    txtL_DrName.Text = CData.Drs[0].sDrsName;
                    txtL_DrOuter.Text = CData.Drs[0].dDrsOuter.ToString();
                    txtL_DrHei.Text = CData.Drs[0].dDrsH.ToString();
                }
                else
                {
                    txtL_DrName.Text = m_tWhl.sDrsName;
                    txtL_DrOuter.Text = m_tWhl.dDrsOuter.ToString();
                    txtL_DrHei.Text = m_tWhl.dDrsH.ToString();
                }
                //txtL_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();

                //if (CDev.It.a_tWhl[0].dWhlAf == 0)
				//211118 pjh : 조건 변경
                //if(m_tWhl.sWhlName != CData.Dev.aData[0].sSelWhl) 
                if(CDev.It.a_tWhl[0].aUsedP == null)
                {
                    //211012 pjh : Dressing Air Cut
                    txtL_Air.Text = m_tWhl.dDair.ToString();

                    if(CDataOption.IsDrsAirCutReplace)
                    {
                        txtL_Air_Rep.Text = m_tWhl.dDairRep.ToString();
                    }

                    // Using 01
                    cmbL_U1Mod.SelectedIndex = (int)m_tWhl.aUsedP[0].eMode;
                    txtL_U1To.Text = m_tWhl.aUsedP[0].dTotalDep.ToString();
                    txtL_U1Cy.Text = m_tWhl.aUsedP[0].dCycleDep.ToString();
                    txtL_U1Tb.Text = m_tWhl.aUsedP[0].dTblSpd.ToString();
                    txtL_U1Sp.Text = m_tWhl.aUsedP[0].iSplSpd.ToString();
                    cmbL_U1Dir.SelectedIndex = (int)m_tWhl.aUsedP[0].eDir;

                    // Using 02
                    cmbL_U2Mod.SelectedIndex = (int)m_tWhl.aUsedP[1].eMode;
                    txtL_U2To.Text = m_tWhl.aUsedP[1].dTotalDep.ToString();
                    txtL_U2Cy.Text = m_tWhl.aUsedP[1].dCycleDep.ToString();
                    txtL_U2Tb.Text = m_tWhl.aUsedP[1].dTblSpd.ToString();
                    txtL_U2Sp.Text = m_tWhl.aUsedP[1].iSplSpd.ToString();
                    cmbL_U2Dir.SelectedIndex = (int)m_tWhl.aUsedP[1].eDir;

                    // Replace 01
                    cmbL_R1Mod.SelectedIndex = (int)m_tWhl.aNewP[0].eMode;
                    txtL_R1To.Text = m_tWhl.aNewP[0].dTotalDep.ToString();
                    txtL_R1Cy.Text = m_tWhl.aNewP[0].dCycleDep.ToString();
                    txtL_R1Tb.Text = m_tWhl.aNewP[0].dTblSpd.ToString();
                    txtL_R1Sp.Text = m_tWhl.aNewP[0].iSplSpd.ToString();
                    cmbL_R1Dir.SelectedIndex = (int)m_tWhl.aNewP[0].eDir;

                    // Replace 02
                    cmbL_R2Mod.SelectedIndex = (int)m_tWhl.aNewP[1].eMode;
                    txtL_R2To.Text = m_tWhl.aNewP[1].dTotalDep.ToString();
                    txtL_R2Cy.Text = m_tWhl.aNewP[1].dCycleDep.ToString();
                    txtL_R2Tb.Text = m_tWhl.aNewP[1].dTblSpd.ToString();
                    txtL_R2Sp.Text = m_tWhl.aNewP[1].iSplSpd.ToString();
                    cmbL_R2Dir.SelectedIndex = (int)m_tWhl.aNewP[1].eDir;

                    txtL_WhlLimit.Text = m_tWhl.dWhlLimit.ToString();
                    txtL_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();
                }
                else
                {
                    //211012 pjh : Dressing Air Cut
                    txtL_Air.Text = CDev.It.a_tWhl[0].dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtL_Air_Rep.Text = CDev.It.a_tWhl[0].dDairRep.ToString();
                    }

                    // Using 01
                    cmbL_U1Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[0].eMode;
                    txtL_U1To.Text = CDev.It.a_tWhl[0].aUsedP[0].dTotalDep.ToString();
                    txtL_U1Cy.Text = CDev.It.a_tWhl[0].aUsedP[0].dCycleDep.ToString();
                    txtL_U1Tb.Text = CDev.It.a_tWhl[0].aUsedP[0].dTblSpd.ToString();
                    txtL_U1Sp.Text = CDev.It.a_tWhl[0].aUsedP[0].iSplSpd.ToString();
                    cmbL_U1Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[0].eDir;

                    // Using 02
                    cmbL_U2Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[1].eMode;
                    txtL_U2To.Text = CDev.It.a_tWhl[0].aUsedP[1].dTotalDep.ToString();
                    txtL_U2Cy.Text = CDev.It.a_tWhl[0].aUsedP[1].dCycleDep.ToString();
                    txtL_U2Tb.Text = CDev.It.a_tWhl[0].aUsedP[1].dTblSpd.ToString();
                    txtL_U2Sp.Text = CDev.It.a_tWhl[0].aUsedP[1].iSplSpd.ToString();
                    cmbL_U2Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[1].eDir;

                    // Replace 01
                    cmbL_R1Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[0].eMode;
                    txtL_R1To.Text = CDev.It.a_tWhl[0].aNewP[0].dTotalDep.ToString();
                    txtL_R1Cy.Text = CDev.It.a_tWhl[0].aNewP[0].dCycleDep.ToString();
                    txtL_R1Tb.Text = CDev.It.a_tWhl[0].aNewP[0].dTblSpd.ToString();
                    txtL_R1Sp.Text = CDev.It.a_tWhl[0].aNewP[0].iSplSpd.ToString();
                    cmbL_R1Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[0].eDir;

                    // Replace 02
                    cmbL_R2Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[1].eMode;
                    txtL_R2To.Text = CDev.It.a_tWhl[0].aNewP[1].dTotalDep.ToString();
                    txtL_R2Cy.Text = CDev.It.a_tWhl[0].aNewP[1].dCycleDep.ToString();
                    txtL_R2Tb.Text = CDev.It.a_tWhl[0].aNewP[1].dTblSpd.ToString();
                    txtL_R2Sp.Text = CDev.It.a_tWhl[0].aNewP[1].iSplSpd.ToString();
                    cmbL_R2Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[1].eDir;

                    //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                    txtL_WhlLimit.Text = CDev.It.a_tWhl[0].dWhlLimit.ToString();
                    txtL_DrsLimit.Text = CDev.It.a_tWhl[0].dDrsLimit.ToString();
                    //
                }
                //210826 pjh : Selected wheel
                if (txtL_SelWhl.Text == string.Empty)
                { txtL_SelWhl.Text = m_tWhl.sWhlName; }
                else
                { txtL_SelWhl.Text = CData.Dev.aData[(int)EWay.L].sSelWhl; }
            }
            else
            {
                // Wheel information
                txtR_Name.Text = m_tWhl.sWhlName;
                txtR_WhDt.Text = m_tWhl.dtLast.ToString();
                txtR_WhOuter.Text = m_tWhl.dWhlO.ToString();
                txtR_WhTip.Text = m_tWhl.dWhltH.ToString();
                txtR_WhToCnt.Text = m_tWhl.iGtc.ToString();
                txtR_WhDrCnt.Text = m_tWhl.iGdc.ToString();
                txtR_WhToLoss.Text = m_tWhl.dWhltL.ToString();
                txtR_WhStLoss.Text = m_tWhl.dWhloL.ToString();
                txtR_WhDrLoss.Text = m_tWhl.dWhldoL.ToString();
                txtR_WhCyLoss.Text = m_tWhl.dWhldcL.ToString();
                //txtR_Air.Text = m_tWhl.dDair.ToString();
                // 2020.11.23 JSKim St
                //txtR_Air_Rep.Text = m_tWhl.dDairRep.ToString();
                // 2020.11.23 JSKim Ed
                //txtR_WhlLimit.Text = m_tWhl.dWhlLimit.ToString();

                // Dresser information
                if (CDataOption.UseSeperateDresser)
                {
                    txtR_DrName.Text = CData.Drs[1].sDrsName;
                    txtR_DrOuter.Text = CData.Drs[1].dDrsOuter.ToString();
                    txtR_DrHei.Text = CData.Drs[1].dDrsH.ToString();
                }
                else
                {
                    txtR_DrName.Text = m_tWhl.sDrsName;
                    txtR_DrOuter.Text = m_tWhl.dDrsOuter.ToString();
                    txtR_DrHei.Text = m_tWhl.dDrsH.ToString();
                }
                //txtR_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();

                //if (CDev.It.a_tWhl[0].dWhlAf == 0)
                //211118 pjh : 조건 변경
                //if (m_tWhl.sWhlName != CData.Dev.aData[1].sSelWhl)
                if (CDev.It.a_tWhl[1].aUsedP == null)
                {
                    //211012 pjh : Dressing Air Cut
                    txtR_Air.Text = m_tWhl.dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtR_Air.Text = m_tWhl.dDair.ToString();
                    }

                    // Using 01
                    cmbR_U1Mod.SelectedIndex = (int)m_tWhl.aUsedP[0].eMode;
                    txtR_U1To.Text = m_tWhl.aUsedP[0].dTotalDep.ToString();
                    txtR_U1Cy.Text = m_tWhl.aUsedP[0].dCycleDep.ToString();
                    txtR_U1Tb.Text = m_tWhl.aUsedP[0].dTblSpd.ToString();
                    txtR_U1Sp.Text = m_tWhl.aUsedP[0].iSplSpd.ToString();
                    cmbR_U1Dir.SelectedIndex = (int)m_tWhl.aUsedP[0].eDir;

                    // Using 02
                    cmbR_U2Mod.SelectedIndex = (int)m_tWhl.aUsedP[1].eMode;
                    txtR_U2To.Text = m_tWhl.aUsedP[1].dTotalDep.ToString();
                    txtR_U2Cy.Text = m_tWhl.aUsedP[1].dCycleDep.ToString();
                    txtR_U2Tb.Text = m_tWhl.aUsedP[1].dTblSpd.ToString();
                    txtR_U2Sp.Text = m_tWhl.aUsedP[1].iSplSpd.ToString();
                    cmbR_U2Dir.SelectedIndex = (int)m_tWhl.aUsedP[1].eDir;

                    // Replace 01
                    cmbR_R1Mod.SelectedIndex = (int)m_tWhl.aNewP[0].eMode;
                    txtR_R1To.Text = m_tWhl.aNewP[0].dTotalDep.ToString();
                    txtR_R1Cy.Text = m_tWhl.aNewP[0].dCycleDep.ToString();
                    txtR_R1Tb.Text = m_tWhl.aNewP[0].dTblSpd.ToString();
                    txtR_R1Sp.Text = m_tWhl.aNewP[0].iSplSpd.ToString();
                    cmbR_R1Dir.SelectedIndex = (int)m_tWhl.aNewP[0].eDir;

                    // Replace 02
                    cmbR_R2Mod.SelectedIndex = (int)m_tWhl.aNewP[1].eMode;
                    txtR_R2To.Text = m_tWhl.aNewP[1].dTotalDep.ToString();
                    txtR_R2Cy.Text = m_tWhl.aNewP[1].dCycleDep.ToString();
                    txtR_R2Tb.Text = m_tWhl.aNewP[1].dTblSpd.ToString();
                    txtR_R2Sp.Text = m_tWhl.aNewP[1].iSplSpd.ToString();
                    cmbR_R2Dir.SelectedIndex = (int)m_tWhl.aNewP[1].eDir;

                    txtR_WhlLimit.Text = m_tWhl.dWhlLimit.ToString();
                    txtR_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();
                }
                else
                {
                    //211012 pjh : Dressing Air Cut
                    txtR_Air.Text = CDev.It.a_tWhl[1].dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtR_Air_Rep.Text = CDev.It.a_tWhl[1].dDairRep.ToString();
                    }

                    // Using 01
                    cmbR_U1Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[0].eMode;
                    txtR_U1To.Text = CDev.It.a_tWhl[1].aUsedP[0].dTotalDep.ToString();
                    txtR_U1Cy.Text = CDev.It.a_tWhl[1].aUsedP[0].dCycleDep.ToString();
                    txtR_U1Tb.Text = CDev.It.a_tWhl[1].aUsedP[0].dTblSpd.ToString();
                    txtR_U1Sp.Text = CDev.It.a_tWhl[1].aUsedP[0].iSplSpd.ToString();
                    cmbR_U1Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[0].eDir;

                    // Using 02
                    cmbR_U2Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[1].eMode;
                    txtR_U2To.Text = CDev.It.a_tWhl[1].aUsedP[1].dTotalDep.ToString();
                    txtR_U2Cy.Text = CDev.It.a_tWhl[1].aUsedP[1].dCycleDep.ToString();
                    txtR_U2Tb.Text = CDev.It.a_tWhl[1].aUsedP[1].dTblSpd.ToString();
                    txtR_U2Sp.Text = CDev.It.a_tWhl[1].aUsedP[1].iSplSpd.ToString();
                    cmbR_U2Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[1].eDir;

                    // Replace 01
                    cmbR_R1Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[0].eMode;
                    txtR_R1To.Text = CDev.It.a_tWhl[1].aNewP[0].dTotalDep.ToString();
                    txtR_R1Cy.Text = CDev.It.a_tWhl[1].aNewP[0].dCycleDep.ToString();
                    txtR_R1Tb.Text = CDev.It.a_tWhl[1].aNewP[0].dTblSpd.ToString();
                    txtR_R1Sp.Text = CDev.It.a_tWhl[1].aNewP[0].iSplSpd.ToString();
                    cmbR_R1Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[0].eDir;

                    // Replace 02
                    cmbR_R2Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[1].eMode;
                    txtR_R2To.Text = CDev.It.a_tWhl[1].aNewP[1].dTotalDep.ToString();
                    txtR_R2Cy.Text = CDev.It.a_tWhl[1].aNewP[1].dCycleDep.ToString();
                    txtR_R2Tb.Text = CDev.It.a_tWhl[1].aNewP[1].dTblSpd.ToString();
                    txtR_R2Sp.Text = CDev.It.a_tWhl[1].aNewP[1].iSplSpd.ToString();
                    cmbR_R2Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[1].eDir;

                    //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                    txtR_WhlLimit.Text = CDev.It.a_tWhl[1].dWhlLimit.ToString();
                    txtR_DrsLimit.Text = CDev.It.a_tWhl[1].dDrsLimit.ToString();
                    //
                }
                //210826 pjh : Selected wheel
                if (txtR_SelWhl.Text == string.Empty)
                { txtR_SelWhl.Text = m_tWhl.sWhlName; }
                else
                { txtR_SelWhl.Text = CData.Dev.aData[(int)EWay.R].sSelWhl; }
            }
        }

        private void btn_Whl_Save_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;

            if (m_eWy == EWay.L)
            { iIdx = m_iLeft; }
            else if (m_eWy == EWay.R)
            { iIdx = m_iRight; }

            // 2021.10.26 SungTae Start : [추가] (Skyworks VOC) User Level이 "Operator"일 경우 저장 안되도록 고객사에서 수정 요청
            if(CData.Lev == ELv.Operator && (m_eWy == EWay.L || m_eWy == EWay.R))
            {
                CMsg.Show(eMsg.Notice, "Notice", "Parameter cannot be saved when user level is operator.");
                BeginInvoke(new Action(() => mBtn.Enabled = true));
                return;
            }
            // 2021.10.26 SungTae End

            if (CData.WhlChgSeq.bStart && CData.WhlChgSeq.iStep > 1 && CData.WhlChgSeq.bSltLeft && m_eWy != EWay.L)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Select file wrong. Select left wheel file please");
                BeginInvoke(new Action(() => mBtn.Enabled = true));
                return;
            }
            if (CData.WhlChgSeq.bStart && CData.WhlChgSeq.iStep > 1 && CData.WhlChgSeq.bSltRight && m_eWy != EWay.R)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Select file wrong. Select right wheel file please");
                BeginInvoke(new Action(() => mBtn.Enabled = true));
                return;
            }


            try
            {
                if (iIdx < 0)
                {
                    CErr.Show(eErr.SYSTEM_WHEEL_FILE_NOT_SELECTE);
                }
                else
                {
                    EWay eWy = (EWay)m_eWy;
                    string sNote = "";
                    string sName = "";

                    // 현재 휠 파일 이름 획득
                    if (m_eWy == EWay.L)
                    {
                        sName = lbxL_List_Dev.SelectedItem.ToString();
                        CData.Dev.aData[0].sSelWhl = sName;
                    }
                    else
                    {
                        sName = lbxR_List_Dev.SelectedItem.ToString();
                        CData.Dev.aData[1].sSelWhl = sName;
                    }

                    // 저장 여부 확인
                    sNote = string.Format("Do you want [{0}] wheel file?", sName);
                    if (CMsg.Show(eMsg.Warning, "Warning", sNote) == DialogResult.OK)
                    {
                        _Get();

                        string sPath = GV.PATH_DEVICE + string.Format("{0}\\{1}.dev", CData.DevGr, CData.Dev.sName);
                        CDev.It.Save(sPath);
                        //CDev.It._Save(m_eWy, CData.Whls[(int)m_eWy]);//210929 pjh : Device에서 Wheel 정보 저장

                        /*
                        iRet = CWhl.It.Save(eWy, m_tWhl);
                        if (iRet != 0)
                        {
                            CErr.Show(eErr.SYSTEM_WHEEL_FILE_SAVE_ERROR);
                        }
                        */

                        _Set();
                        
                        if (m_eWy == EWay.L)
                        {
                            CData.Whls[0] = m_tWhl;
                        }
                        else
                        {
                            CData.Whls[1] = m_tWhl;
                        }
                        CMsg.Show(eMsg.Notice, "Notice", "Wheel file save success");
                        CData.WhlChgSeq.bSltWhlFile = true; //190711 ksg :추가
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }
        }

        private void tbcon_Etc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage mTp = sender as TabPage;

            int iIdx = 0;
            m_eWy = (EWay)int.Parse(tbcon_Etc.SelectedTab.Tag.ToString());

            _ListUp();

            if (m_eWy == EWay.L)
            {
                // lbxL_List.SelectedIndex = -1;
                //lbxL_List.SelectedIndex = m_iLeft;
                if (m_iLeft >= 0) { lbxL_List_Dev.SetSelected(m_iLeft, true); }
                tbcon_Etc.Size = new Size(720, 840);

                iIdx = m_iLeft;
            }
            else if (m_eWy == EWay.R)
            {
                //lbxR_List.SelectedIndex = -1;
                if (m_iRight >= 0) { lbxR_List_Dev.SetSelected(m_iRight, true); }
                //lbxR_List.SelectedIndex = m_iRight;
                tbcon_Etc.Size = new Size(720, 840);

                iIdx = m_iRight;
            }
            else
            {
                tbcon_Etc.Size = new Size(1010, 840);
            }

            _Set();
        }

        private void lbx_List_Dev_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox mLbx = sender as ListBox;
            int iIdx = 0;

            if (m_eWy == EWay.L)
            {
                iIdx = lbxL_List_Dev.SelectedIndex;
            }
            else if (m_eWy == EWay.R)
            {
                iIdx = lbxR_List_Dev.SelectedIndex;
            }


            if (iIdx < 0)
            {
                CWhl.It.InitWhl(out m_tWhl);
                _Set();
            }
            else
            {
                if (m_eWy == EWay.L) { m_iLeft = iIdx; }
                else if (m_eWy == EWay.R) { m_iRight = iIdx; }
                CWhl.It.Load(m_eWy, mLbx.SelectedItem.ToString(), out m_tWhl);

                _Set();
            }
        }

        private void lbx_List_Dev_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, Color.FromArgb(43, 173, 175));
            }

            ListBox mLbx = sender as ListBox;
            int iX = e.Bounds.X + (e.Font.Height / 2);
            int iY = e.Bounds.Y + (e.Font.Height / 2);

            e.DrawBackground();
            e.Graphics.DrawString(mLbx.Items[e.Index].ToString(), e.Font, Brushes.Black, iX, iY, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void btn_New_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_WHEEL;
            string sPathWhl = "";

            try
            {
                if (m_eWy == EWay.L) { sPath += "Left\\"; }
                else { sPath += "Right\\"; }

                using (frmTxt mForm = new frmTxt(m_eWy.ToString() + " Wheel Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.Cancel) { return; }

                    //if (File.Exists(sPath + mForm.Val + ".whl"))
                    if (Directory.Exists(sPath + mForm.Val))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Wheel name exist !!!");
                        return;
                    }
                    //sPath += mForm.Val + ".whl";
                    sPath += mForm.Val + "\\";
                    Directory.CreateDirectory(sPath);

                    //sPath += mForm.Val + ".whl";
                    sPathWhl += sPath + "WheelInfo.whl";

                    // 휠 데이터 초기값
                    CWhl.It.InitWhl(out m_tWhl);
                    m_tWhl.sWhlName = mForm.Val;

                    CWhl.It.Save(sPathWhl, m_tWhl);

                    // 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 생성하도록 변경
                    //sPath += "WheelHistory.csv";

                    // 2021.06.09 lhs Start : 폴더 없을시 만들기
                    //sPath += "WheelHistory\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                    sPath += "WheelHistory\\";
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    sPath += DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                    // 2021.06.09 lhs End
                    // 2021.04.06 SungTae End

                    CWhl.It.SaveNewHistory(sPath);

                    _ListUp();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }
        }

        private void btn_SaveAs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;
            string sSrc = "";
            string sDst = "";
            string sPath = "";

            if (m_eWy == EWay.L)
            { iIdx = lbxL_List_Dev.SelectedIndex; }
            else
            { iIdx = lbxR_List_Dev.SelectedIndex; ; }

            try
            {
                if (iIdx < 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected wheel");
                    return;
                }

                using (frmTxt mForm = new frmTxt("Save As Wheel Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        //if (m_eWy == EWay.L) { sSrc = GV.PATH_WHEEL + lbxL_List.SelectedValue.ToString() + ".whl"; }
                        if (m_eWy == EWay.L)
                        {
                            sSrc = GV.PATH_WHEEL + "Left\\" + lbxL_List_Dev.SelectedItem.ToString() + "\\";
                            sDst = GV.PATH_WHEEL + "Left\\";
                        }
                        //else { sSrc = GV.PATH_WHEEL + lbxR_List.SelectedValue.ToString() + ".whl"; }\
                        else
                        {
                            sSrc = GV.PATH_WHEEL + "Right\\" + lbxR_List_Dev.SelectedItem.ToString() + "\\";
                            sDst = GV.PATH_WHEEL + "Right\\";
                        }
                        //sDst = GV.PATH_WHEEL + mForm.Val + "\\" + mForm.Val + ".whl";
                        // 원본과 동일한지 확인
                        sDst += mForm.Val + "\\";
                        if (sSrc == sDst)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Wheel name same !!!");
                            return;
                        }
                        // 동일한 이름 확인
                        if (Directory.Exists(sDst))
                        {
                            CMsg.Show(eMsg.Error, "Error", "Wheel name exist !!!");
                            return;
                        }
                        else
                        {
                            Directory.CreateDirectory(sDst);
                            /*
                            if (m_eWy == EWay.L)
                            {
                                sSrc += lbxL_List.SelectedItem.ToString() + ".whl";
                            }
                            else
                            {
                                sSrc += lbxR_List.SelectedItem.ToString() + ".whl";
                            }
                            */
                            sSrc += "WheelInfo.whl";
                        }

                        // 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 생성하도록 변경
                        //sPath = sDst + "WheelHistory.csv";

                        // 2021.06.09 lhs Start : 폴더 없을시 만들기
                        //sPath = sDst + "WheelHistory\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                        sPath = sDst + "WheelHistory\\";
                        if (!Directory.Exists(sPath))
                        {
                            Directory.CreateDirectory(sPath);
                        }
                        sPath += DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                        // 2021.06.09 lhs End

                        // 2021.04.06 SungTae End

                        sDst += "WheelInfo.whl";

                        // 파일 복사
                        FileSystem.CopyFile(sSrc, sDst, UIOption.AllDialogs);
                        CCheckChange.SaveAs("WHEEL", sSrc, sDst); //200716 lks

                        // 복사한 파일의 이름 변수 변경을 위해 로드 -> 이름변경 -> 세이브
                        CWhl.It.Load(sDst, out m_tWhl);
                        m_tWhl.sWhlName = mForm.Val;
                        CWhl.It.Save(sDst, m_tWhl);
                        //휠 파라미터와 휠 이름 제외한 휠 정보 초기화
                        CWhl.It.SaveNew(m_eWy, m_tWhl);
                        CWhl.It.SaveNewHistory(sPath);

                        _ListUp();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;
            string sName = "";
            string sPath = GV.PATH_WHEEL;

            if (m_eWy == EWay.L) { iIdx = m_iLeft; }
            else { iIdx = m_iRight; }

            try
            {
                if (iIdx < 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected wheel");
                    return;
                }
                if (m_eWy == EWay.L)
                {
                    sName = lbxL_List_Dev.SelectedItem.ToString();
                    sPath += "Left\\";
                    if (CData.Whls[0].sWhlName == sName)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Current using wheel");
                        return;
                    }
                }
                else
                {
                    sName = lbxR_List_Dev.SelectedItem.ToString();
                    sPath += "Right\\";

                    if (CData.Whls[1].sWhlName == sName)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Current using wheel");
                        return;
                    }
                }

                // 현재 사용중인지 확인

                // 경로 생성
                sPath = sPath + sName;
                //File.Delete(sPath);
                CCheckChange.DeleteLog("WHEEL", sPath); //200716 lks

                Directory.Delete(sPath, true);

                CWhl.It.InitWhl(out m_tWhl);
                //if (m_eWy == EWay.L) { lbxL_List.ClearSelected(); }
                if (m_eWy == EWay.L)
                {
                    lbxL_List_Dev.ClearSelected();
                    m_iLeft--;
                }
                //else { lbxR_List.ClearSelected(); }
                else
                {
                    lbxR_List_Dev.ClearSelected();
                    m_iRight--;
                }

                _ListUp();
            }
            catch (Exception ex)
            {

            }
            finally
            { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }

        public void _Load()
        {            
            string spathL = "";
            string spathR = "";
            string sWhlL = CData.Dev.aData[(int)EWay.L].sSelWhl;
            string sWhlR = CData.Dev.aData[(int)EWay.R].sSelWhl;
            bool bBoth = false;
            bool bL = false;
            bool bR = false;

            if(sWhlL == "" && sWhlR == "")
            {
                sWhlL = CData.Whls[0].sWhlName;
                sWhlR = CData.Whls[1].sWhlName;
                bBoth = true;
            }
            else if(sWhlL == "")
            {
                sWhlL = CData.Whls[0].sWhlName;
                bL = true;
            }
            else if(sWhlR == "")
            {
                sWhlR = CData.Whls[1].sWhlName;
                bR = true;
            }

            spathL = GV.PATH_WHEEL + "Left\\" + sWhlL + "\\WheelInfo.whl";
            spathR = GV.PATH_WHEEL + "Right\\" + sWhlR + "\\WheelInfo.whl";

            if(File.Exists(spathL) && File.Exists(spathR))
            {
			    //211118 pjh : Wheel File Load 할 필요 없어서 주석 처리
                //CWhl.It.Load(spathL, out CData.Whls[0]);
                //CWhl.It.Load(spathR, out CData.Whls[1]);

                if(bBoth)
                {
                    CMsg.Show(eMsg.Notice, "Notice", "S/W load previous [Wheel File].\rBecause, Current Device doesn't have loded device for both wheel.");
                    bBoth = false;
                }
                else if(bL)
                {
                    CMsg.Show(eMsg.Notice, "Notice", "S/W load previous [Wheel File].\rBecause, Current Device doesn't have loded device for left wheel.");
                    bL = false;
                }
                else if(bR)
                {
                    CMsg.Show(eMsg.Notice, "Notice", "S/W load previous [Wheel File].\rBecause, Current Device doesn't have loded device for right wheel.");
                    bR = false;
                }
            }
            else
            {
                if (m_eWy == EWay.L) CErr.Show(eErr.SYSTEM_LEFT_WHEEL_FILE_NOT_LOAD);
                else if (m_eWy == EWay.R) CErr.Show(eErr.SYSTEM_RIGHT_WHEEL_FILE_NOT_LOAD);
            }
            CData.Dev.aData[(int)EWay.L].sSelWhl = sWhlL;
            CData.Dev.aData[(int)EWay.R].sSelWhl = sWhlR;
        }
        #endregion
#endif
    }
}
