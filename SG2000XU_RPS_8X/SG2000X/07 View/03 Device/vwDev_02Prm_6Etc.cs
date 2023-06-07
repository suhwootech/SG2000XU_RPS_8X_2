using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm_6Etc : UserControl
    {
        public vwDev_02Prm_6Etc()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");   }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");   }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans"); }
            
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

            lbl_BtmCln_Water.Text = "Count (ea)";

            // 2022.07.20 lhs Start : SprayBtmCleaner 사용시 표시
            bool bSprayCln = CDataOption.UseSprayBtmCleaner;
            if (bSprayCln)
            {
                lbl_BtmCln_Water.Text   = "Water Count (ea)";
                lbl_BtmCln_Air.Text     = "Air Count (ea)";
            }
            lbl_BtmCln_Air.Visible      = bSprayCln;
            txt_BtmClnS_Air.Visible     = bSprayCln;
            txt_BtmClnP_Air.Visible     = bSprayCln;
            // 2022.07.20 lhs End

            // 2022.07.27 lhs Start : Btm Brush 사용시 표시
            bool bBrushCln      = CDataOption.UseBrushBtmCleaner;
            lbl_BtmCln_Brush.Visible     = bBrushCln;
            txt_BtmClnS_Brush.Visible    = bBrushCln;

            bool bSprayNBrush   = bSprayCln && bBrushCln;   // 2개 모두 설치되었을 경우
            if (bSprayNBrush)
            {
                lbl_BtmCln_Water.Text   = "#1 Water Count (ea)";
                lbl_BtmCln_Air.Text     = "#1 Air Count (ea)";
            }            
            lbl_BtmCln_Water_N2.Visible  = bSprayNBrush;
            txt_BtmClnS_Water_N2.Visible = bSprayNBrush;
            lbl_BtmCln_Air_N2.Visible    = bSprayNBrush;
            txt_BtmClnS_Air_N2.Visible   = bSprayNBrush;
            // 2022.07.27 lhs End : Btm Brush 사용시 표시

            // 2021.03.30 lhs Start  : // Dryer Water clean시의 회전속도, 회전카운트, 시간
            if (CDataOption.Dryer == eDryer.Rotate && CDataOption.UseDryWtNozzle)   { grpDryWtNozzle.Visible = true;  }
            else                                                                    { grpDryWtNozzle.Visible = false; }
            // 2021.03.30 lhs End

            // 2020.12.16 JSKim St
            // SCK+에서도 Picker Place Delay 시간은 옵션으로 처리해달라고 함...
            // SCK+ 적용 사항은 SCK에도 해당 됨
            if (CDataOption.Package == ePkg.Unit || 
                CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
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
				// 2022.05.24 lhs Start : (SCK+)On/Off Picker의 Eject Dely 시간 설정 요청
				if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    lblP_OnpDlyEjectT.Visible = true;    
                    lblP_OnpDlyEjectU.Visible = true;
                    txtP_OnpDlyEject.Visible  = true;
                    
                    lblP_OfpDlyEjectT.Visible = true;
                    lblP_OfpDlyEjectU.Visible = true;
                    txtP_OfpDlyEject.Visible  = true;
                }
                // 2022.05.24 lhs End
            }

            //200121 ksg : , 200625 lks
            if (CData.CurCompany == ECompany.SCK  ||
                CData.CurCompany == ECompany.JSCK ||
                CData.CurCompany == ECompany.JCET)
            { pnl_ShiftData.Visible = true; }
            else
            { pnl_ShiftData.Visible = false; }

            // Data Shift Probe Skip
            if (CData.CurCompany == ECompany.ASE_KR)    { pnl_DShiftPSkip.Visible = true; }
            else                                        { pnl_DShiftPSkip.Visible = false; }

            // 200416 pjh : Grinding Strip Start Limit Visible, 200625 lks
            if (CData.CurCompany == ECompany.SCK  || 
                CData.CurCompany == ECompany.JSCK ||  
                CData.CurCompany == ECompany.JCET || 
                CData.CurCompany == ECompany.Suhwoo)
            { 
                pnl_StripStartLimit.Visible = true;
                
                if(CDataOption.UseNewSckGrindProc)
                {
                    lblp_StripStartLimit_Comment.Text = "Base on Thickness, Mold : Only mold thickness\r\n" +
                                                        "Total : Total strip thickness\r\n" +
                                                        "Must put value more than 0.3";
                }
                else
                {
                    lblp_StripStartLimit_Comment.Text = "DF Use: Only mold thickness\r\n" +
                                                        "DF Skip: Total strip thickness\r\n" +
                                                        "Must put value more than 0.3";
                }
            }
            else
            { pnl_StripStartLimit.Visible = false; }

            //190309 ksg :
            if (CData.CurCompany == ECompany.SkyWorks)  { ckb_OcrSkip.Visible = true;   }
            else                                        { ckb_OcrSkip.Visible = false;  }

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
            if (CData.Dev.eDryDir == eDryDir.CCW)   { rdb_CCW.Checked = true; }
            else                                    { rdb_CW.Checked  = true; }
            txt_PushF.Text = CData.Dev.dPushF         .ToString();
            txt_PushS.Text = CData.Dev.dPushS         .ToString();
            txt_BtmClnP_Water.Text = CData.Dev.iOffpClean     .ToString();  // Btm Clean Pad Count
            txt_BtmClnS_Water.Text = CData.Dev.iOffpCleanStrip.ToString();  // Btm Clean Strip Count
            // 2022.03.09 lhs Start : Spray Nozzle형 
            if (CDataOption.UseSprayBtmCleaner)
            {
                txt_BtmClnP_Air.Text = CData.Dev.iOffpClean_Air.ToString();       // Btm Clean Pad Count (Air Blow)
                txt_BtmClnS_Air.Text = CData.Dev.iOffpCleanStrip_Air.ToString();  // Btm Clean Strip Count (Air Blow)
            }
            // 2022.03.09 lhs End

            // 2022.07.27 lhs Start : Brush 추가
            if (CDataOption.UseBrushBtmCleaner)
            {
                txt_BtmClnS_Brush.Text      = CData.Dev.iOffpCleanBrush.ToString();         // Brush Clean Count
                txt_BtmClnS_Water_N2.Text   = CData.Dev.iOffpCleanStrip_N2.ToString();      // Strip Clean Count #2
                txt_BtmClnS_Air_N2.Text     = CData.Dev.iOffpCleanStrip_Air_N2.ToString();  // Strip Clean Count (Air Blow) #2                
            }
            // 2022.07.27 lhs End

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
            txtP_OnpDlyEject.Text = CData.Dev.iEjectDelayOnP.ToString();    // 2022.05.24 lhs   
            txtP_OfpDlyPick.Text = CData.Dev.iPickDelayOff.ToString();
            txtP_OfpDlyPlace.Text = CData.Dev.iPlaceDelayOff.ToString();    //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
            txtP_OfpDlyEject.Text = CData.Dev.iEjectDelayOffP.ToString();   // 2022.05.24 lhs

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
            int   .TryParse(txt_BtmClnP_Water.Text, out CData.Dev.iOffpClean     );
            int   .TryParse(txt_BtmClnS_Water.Text, out CData.Dev.iOffpCleanStrip);
            // 2022.03.09 lhs Start 
            if (CDataOption.UseSprayBtmCleaner)
			{
				int.TryParse(txt_BtmClnP_Air.Text, out CData.Dev.iOffpClean_Air);
				int.TryParse(txt_BtmClnS_Air.Text, out CData.Dev.iOffpCleanStrip_Air);
			}
            // 2022.03.09 lhs End

            // 2022.07.27 lhs Start : Brush 추가
            if (CDataOption.UseBrushBtmCleaner)
            {
                int.TryParse(txt_BtmClnS_Brush.Text,    out CData.Dev.iOffpCleanBrush);         // Brush Clean Count
                int.TryParse(txt_BtmClnS_Water_N2.Text, out CData.Dev.iOffpCleanStrip_N2);      // Strip Clean Count #2
                int.TryParse(txt_BtmClnS_Air_N2.Text,   out CData.Dev.iOffpCleanStrip_Air_N2);  // Strip Clean Count (Air Blow) #2                
            }
            // 2022.07.27 lhs End

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
            int.TryParse(txtP_OnpDlyEject.Text, out CData.Dev.iEjectDelayOnP);    // 2022.05.24 lhs    
            int.TryParse(txtP_OfpDlyPick.Text, out CData.Dev.iPickDelayOff);
            int.TryParse(txtP_OfpDlyPlace.Text, out CData.Dev.iPlaceDelayOff); //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
            int.TryParse(txtP_OfpDlyEject.Text, out CData.Dev.iEjectDelayOffP);    // 2022.05.24 lhs   

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

        //
#endif
    }
}
