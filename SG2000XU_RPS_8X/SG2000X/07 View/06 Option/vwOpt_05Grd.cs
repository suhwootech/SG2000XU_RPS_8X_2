using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_05Grd : UserControl
    {
        private CTim m_Timout_485 = new CTim(); //  2023.03.15 Max

        public vwOpt_05Grd()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            InitializeComponent();

            if (CData.CurCompany == ECompany.ASE_KR)    { pnl_WhlOneCheck.Visible = true;   }
            else                                        { pnl_WhlOneCheck.Visible = false;  }

            pnl_TblCln.Visible      = (CDataOption.TableClean == eTableClean.Use) ? true : false;
            ckb_ManualBcr.Visible   = (CDataOption.ManualBcr  == eManualBcr.Use)  ? true : false;

            ckb_WhlClnSkip.Visible = CDataOption.IsWhlCleaner;
            // 201006 jym : 추가
            pnl_ChkDI.Visible = CDataOption.IsChkDI;

            if (CData.CurCompany == ECompany.SCK  ||
                CData.CurCompany == ECompany.JSCK ||
                CData.CurCompany == ECompany.JCET)
            {
                ckb_WhlJigSkip.Visible = false;
            }
            else
            {
                ckb_WhlJigSkip.Visible = true;
            }

            // 201022 jym : Auto offset(Auto tool setter gap) 변동 시 알람 제한 값 설정
#if true //201202 jhc : Auto offset limit 확인 기능 라이선스 옵션으로 적용
            pnl_Ato.Visible = CDataOption.UseAutoToolSetterGapLimit; //라이선스 체크
#else
            pnl_Ato.Visible = (CData.CurCompany == ECompany.SkyWorks) ? true : false;
#endif
            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //pnl_DrsMeaChk.Visible = (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) ? true : false;
            pnl_DrsMeaChk.Visible = (CData.CurCompany == ECompany.Qorvo     ||
                                     CData.CurCompany == ECompany.Qorvo_DZ  ||
                                     CData.CurCompany == ECompany.Qorvo_RT  ||
                                     CData.CurCompany == ECompany.Qorvo_NC) ? true : false;

            // 2020.10.22 JSKim St
            if (CData.CurCompany == ECompany.JCET)
            {
                pnl_DualPumpCheck.Visible       = true;
                ckb_WheelStopWaitSkip.Visible   = true;     // 2021.02.20 lhs  스핀들 멈추기를 기다림 Skip 체크박스 표시
            }
            else
            {
                pnl_DualPumpCheck.Visible       = false;
                ckb_WheelStopWaitSkip.Visible   = false;    // 2021.02.20 lhs  스핀들 멈추기를 기다림 Skip 체크박스 숨김
            }
            // 2020.10.22 JSKim Ed

            //201025 jhc : Over Grinding Correction - Grinding Count Correction 기능 제공/감춤 용
            pnl_OverGrdCorrection.Visible   = CDataOption.UseGrindingCorrect;
            //                     

            // 210727 pjh : Wheel 및 Dresser 최대 높이 설정
            pnl_WheelDresserMax.Visible     = true;
            //pnl_WheelDresserMax.Visible     = CDataOption.UseWheelDresserMaxLimit ? true : false;
            //

            // 2021.09.14 Start : 이오나이저 Sol/V Off 설정 (SCK전용)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)  {   pnl_Ionizer.Visible = true;     }
            else                                                                        {   pnl_Ionizer.Visible = false;    }
            // 2021.09.14 End

            // 2022.03.22 lhs : Dummy Thickness, 2004U일 경우에만 표시
            pnl_Dummy.Visible = CDataOption.Use2004U;
            //


        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            Set();

            //임시 2020-11-12, jhLee : Master level에서만 Motorize 설정을 볼 수 있다.
            pnl_MeasureType.Visible = (CData.CurCompany == ECompany.ASE_KR && CData.Lev == ELv.Master);

            // 2020-11-18, jhLee : Skyworks Only
            if (CData.CurCompany != ECompany.SkyWorks)
            {
                nud_TbCleanCycle.Visible = false;
                lbl_TbCleanCap1.Visible = false;
                lbl_TbCleanCap2.Visible = false;
                lbl_TbCleanCap3.Visible = false;
            }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        public void Set()
        {
            txt_WIns_Spd.Text       = CData.Opt.iMeaS.ToString();
            txt_WIns_Dly.Text       = CData.Opt.iMeaT.ToString();
            txt_TtvL.Text           = CData.Opt.aWhlTTV[(int)EWay.L].ToString();
            txt_TtvR.Text           = CData.Opt.aWhlTTV[(int)EWay.R].ToString();

            ckb_PbType.Checked      = CData.Opt.bPbType; 
            ckb_WhlOneChk.Checked   = CData.Opt.bWhlOneCheck; 
            nud_TbClean_L.Value     = CData.Opt.aTC_Cnt[(int)EWay.L];
            nud_TbClean_R.Value     = CData.Opt.aTC_Cnt[(int)EWay.R];
            nud_TbCleanCycle.Value  = CData.Opt.iTC_Cycle;               // 2020-11-16, jhLee, Skyworks VOC, Table Clean 주기

            ckb_CoverSkip.Checked   = CData.Opt.bCoverSkip;                 
            ckb_LTableSkip.Checked  = CData.Opt.aTblSkip[(int)EWay.L];
            ckb_RTableSkip.Checked  = CData.Opt.aTblSkip[(int)EWay.R];
            ckb_WhlJigSkip.Checked  = CData.Opt.bWhlJigSkip; 
            ckb_ManualBcr.Checked   = CData.Opt.bManBcrSkip;            
            if (CDataOption.IsWhlCleaner)   { ckb_WhlClnSkip.Checked    = CData.Opt.bWhlClnSkip;            }
            if (CDataOption.IsChkDI)        { txt_ChkDiTime.Text        = CData.Opt.iChkDiTime.ToString();  }

            // 2020.10.13 SungTae : Add
            ckb_DrsFiveChk.Checked = CData.Opt.bDrsFiveCheck;

            // 201022 jym : 추가
            txt_AtoL.Text           = CData.Opt.aAtoLimit[(int)EWay.L].ToString();
            txt_AtoR.Text           = CData.Opt.aAtoLimit[(int)EWay.R].ToString();
			
			// 2020.10.22 JSKim St
            ckb_DualPumpChk.Checked = CData.Opt.bDualPumpUse;
            // 2020.10.22 JSKim Ed	

            //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
            ckb_GrdCntCorrection.Checked    = CData.Opt.bOverGrdCountCorrectionUse;
            //
            ckb_WheelStopWaitSkip.Checked   = CData.Opt.bWheelStopWaitSkip;       // 2021.02.20 lhs 


            // 2020-10-26, jhLee : 측정 정확도를 높이기 위해 Probe를 더 눌러주는 양
            txt_ProbeOD.Text            = string.Format("{0:0.#}", CData.Opt.dProbeOD);
            txt_ProbeStableDelay.Text   = CData.Opt.iProbeStableDelay.ToString();
            txt_TopMoveOffset.Text      = string.Format("{0:0.#}", CData.Opt.dSafetyTopOffset);
            txt_ZMoveUpSpeed.Text       = string.Format("{0:0.#}", CData.Opt.dZAxisMoveUpSpeed);
            //end of 2020-11-03, jhLee

            // 2020-12-14, jhLee : 기존 MeaStrip_T1, T2를 통합한 함수를 이용할것인지 여부
            ckb_FunctionType.Checked    = CData.Opt.bMeasureFunctionType;

            // 210727 pjh : Wheel 및 Dresser 최대 높이 설정
            ckb_VarLimitUse.Checked = CData.Opt.bVarLimitUse;

            Extxt_LWheelMaxT.Text = CData.Opt.aWheelMax[(int)EWay.L].ToString();
            Extxt_RWheelMaxT.Text = CData.Opt.aWheelMax[(int)EWay.R].ToString();
            Extxt_LDressMaxT.Text = CData.Opt.aDresserMax[(int)EWay.L].ToString();
            Extxt_RDressMaxT.Text = CData.Opt.aDresserMax[(int)EWay.R].ToString();
            //
            // 210804 pjh : Tool Setter Gap 변경 최대 값(Tool Setter Gap이 해당 설정값 이상으로 바뀌면 Tool Setter Gap 값 갱신하지 않음)
            Extxt_ToolMax.Text = CData.Opt.dToolMax.ToString();
            //

            // 2021.09.14 Start : 이오나이저 Sol/V Off 설정 (SCK전용)
            ckb_UseIOZTSolOff.Checked   = CData.Opt.bUseIOZTSolOff;
            txt_IOZTSolOffDelaySec.Text = CData.Opt.nIOZTSolOffDelaySec.ToString();
            // 2021.09.14 End

            // 2022.03.22 Start : Dummy Thickness (2004U)
            if (CDataOption.Use2004U)
            {
                txt_DummyThick.Text = CData.Opt.dDummyThick.ToString();
            }
            // 2022.03.22 End


        }

        public void Get()
        {
            int.TryParse(txt_WIns_Spd.Text, out CData.Opt.iMeaS);
            int.TryParse(txt_WIns_Dly.Text, out CData.Opt.iMeaT);
            double.TryParse(txt_TtvL.Text, out CData.Opt.aWhlTTV[(int)EWay.L]);
            double.TryParse(txt_TtvR.Text, out CData.Opt.aWhlTTV[(int)EWay.R]);
            CData.Opt.bPbType = ckb_PbType.Checked;
            CData.Opt.bWhlOneCheck = ckb_WhlOneChk.Checked;
            CData.Opt.aTC_Cnt[(int)EWay.L] = (int)nud_TbClean_L.Value;
            CData.Opt.aTC_Cnt[(int)EWay.R] = (int)nud_TbClean_R.Value;
            CData.Opt.iTC_Cycle = (int)nud_TbCleanCycle.Value ;               // 2020-11-16, jhLee, Skyworks VOC, Table Clean 주기

            CData.Opt.bCoverSkip = ckb_CoverSkip.Checked;
            CData.Opt.aTblSkip[(int)EWay.L] = ckb_LTableSkip.Checked;
            CData.Opt.aTblSkip[(int)EWay.R] = ckb_RTableSkip.Checked;
            CData.Opt.bWhlJigSkip = ckb_WhlJigSkip.Checked;
            CData.Opt.bManBcrSkip = ckb_ManualBcr.Checked;
            if (CDataOption.IsWhlCleaner)
            { CData.Opt.bWhlClnSkip = ckb_WhlClnSkip.Checked; }
            if (CDataOption.IsChkDI)
            { int.TryParse(txt_ChkDiTime.Text, out CData.Opt.iChkDiTime); }

            // 2020.10.13 SungTae : Add
            CData.Opt.bDrsFiveCheck = ckb_DrsFiveChk.Checked;

            // 201022 jym : 추가
            double.TryParse(txt_AtoL.Text, out CData.Opt.aAtoLimit[(int)EWay.L]);
            double.TryParse(txt_AtoR.Text, out CData.Opt.aAtoLimit[(int)EWay.R]);
			
			// 2020.10.22 JSKim St
            CData.Opt.bDualPumpUse = ckb_DualPumpChk.Checked;
            // 2020.10.22 JSKim Ed

            //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
            CData.Opt.bOverGrdCountCorrectionUse = ckb_GrdCntCorrection.Checked;
            //

            CData.Opt.bWheelStopWaitSkip = ckb_WheelStopWaitSkip.Checked;   // 2021.02.20 lhs 
            

            // 2020-10-26, jhLee : 측정 정확도를 높이기 위해 Probe를 더 눌러주는 양
            double.TryParse(txt_ProbeOD.Text, out CData.Opt.dProbeOD);
            int.TryParse(txt_ProbeStableDelay.Text, out CData.Opt.iProbeStableDelay);       // Probe 안정화 시간
            double.TryParse(txt_TopMoveOffset.Text, out CData.Opt.dSafetyTopOffset);
            double.TryParse(txt_ZMoveUpSpeed.Text, out CData.Opt.dZAxisMoveUpSpeed);
            //end of 2020-11-03, jhLee

            // 2020-12-14, jhLee : 기존 MeaStrip_T1, T2를 통합한 함수를 이용할것인지 여부
            CData.Opt.bMeasureFunctionType = ckb_FunctionType.Checked;

            // 210727 pjh : Wheel 및 Dresser 최대 높이 설정
            CData.Opt.bVarLimitUse = ckb_VarLimitUse.Checked;

            double.TryParse(Extxt_LWheelMaxT.Text, out CData.Opt.aWheelMax[(int)EWay.L]);
            double.TryParse(Extxt_RWheelMaxT.Text, out CData.Opt.aWheelMax[(int)EWay.R]);
            double.TryParse(Extxt_LDressMaxT.Text, out CData.Opt.aDresserMax[(int)EWay.L]);
            double.TryParse(Extxt_RDressMaxT.Text, out CData.Opt.aDresserMax[(int)EWay.R]);
            //
            // 210804 pjh : Tool Setter Gap 변경 최대 값(Tool Setter Gap이 해당 설정값 이상으로 바뀌면 Tool Setter Gap 값 갱신하지 않음)
            double.TryParse(Extxt_ToolMax.Text, out CData.Opt.dToolMax);
            //

            // 2021.09.14 Start : 이오나이저 Sol/V Off 설정 (SCK전용)
            CData.Opt.bUseIOZTSolOff = ckb_UseIOZTSolOff.Checked;
            int.TryParse(txt_IOZTSolOffDelaySec.Text, out CData.Opt.nIOZTSolOffDelaySec);
            // 2021.09.14 End

            // 2022.03.22 lhs Start : Dummy Thickness (2004U)
            if (CDataOption.Use2004U)
            {
                double.TryParse(txt_DummyThick.Text, out CData.Opt.dDummyThick);
            }
            // 2022.03.22 lhs End

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

        private void btnO_WInsStart_Click(object sender, EventArgs e)
        {
            int iRpm, iTm;

            int.TryParse(txt_WIns_Spd.Text, out iRpm);
            int.TryParse(txt_WIns_Dly.Text, out iTm);

            // 2020.12.15 JSKim St
            //CSpl.It.Write_Run(EWay.L, iRpm);

            //if (CDataOption.SplType == eSpindleType.EtherCat)
            //{
            //    CData.Spls[0].iRpm = Convert.ToInt32(CSpl.It.GetFrpm(true));
            //}

            //while ((CData.Spls[0].iRpm + 10) < iRpm)
            //{
            //    Application.DoEvents();
            //}


            _SetLog("Click");
            CData.bLastClick = true;

            //if (int.TryParse(txt_WIns_Spd.Text, out iRpm) == false)     // 여기는 Textbox에서 정수형 변환에 실패
            //{
            //    CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
            //    _SetLog("Error : Need Left spindle value check.");
            //    return;
            //}
            //else      // 여기는 Textbox에서 정수형 변환에 성공
            //{
            //    if (iRpm <= 0 || iRpm > 2000 )
            //    {
            //        CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
            //        _SetLog("Error : Need Left spindle value check.");
            //        return;
            //    }
            //}

            //if (int.TryParse(txt_WIns_Dly.Text, out iTm) == false)     // 여기는 Textbox에서 정수형 변환에 실패
            //{
            //    CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
            //    _SetLog("Error : Need Left delay time check.");
            //    return;
            //}
            //else      // 여기는 Textbox에서 정수형 변환에 성공
            //{
            //    if (iTm <= 0 || iTm > 20)
            //    {
            //        CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
            //        _SetLog("Error : Need Left delay time check.");
            //        return;
            //    }
            //}

            // MC check
            if (!CIO.It.Get_Y(eY.GRDL_SplInverterMC))
            {
                CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
                _SetLog("Error : Need Left spindle MC check.");
                return;
            }
            // PCW check
            if (!CIO.It.Get_X(eX.GRDL_SplPCW))
            {
                CErr.Show(eErr.LEFT_GRIND_SPINDLE_COOLANT_OFF);
                return;
            }
            // CDA check
            if (!CIO.It.Get_Y(eY.GRDL_SplCDA))
            {
                CErr.Show(eErr.LEFT_GRIND_SPINDLE_CDA_OFF);
                return;
            }

            /*
            //int iRet;
            
            //iRet = CSpl.It.Write_Rpm(EWay.L, iRpm);

            //if (iRet != 0)
            //{
            //    CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
            //    return;
            //}

            //iRet = CSpl.It.Write_Run(EWay.L, iRpm);
            //if (iRet != 0)
            //{
            //    CErr.Show(eErr.LEFT_GRIND_SPINDLE_RUN_RPM_ERROR);
            //    return;
            //}
            // 2020.12.15 JSKim Ed

            //GU.Delay(iTm);

            //CSpl.It.Write_Stop(EWay.L);
            */

            // 2023.03.15 Max            
            bool bExit = false;
            bool bTimeOutFlag = false;

            // RPM Setting
            CSpl_485.It.Write_Rpm(EWay.L, iRpm);
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptRPMSpindle(EWay.L);
            } while (bExit != true);
            CSpl_485.It.SetAcceptRPMSpindle(EWay.L);

            if (bTimeOutFlag)
            {
                CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
                return;
            }

            // Spindle RUN
            bExit = false;
            bTimeOutFlag = false;
            CSpl_485.It.Write_Run(EWay.L);
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptRunSpindle(EWay.L);
            } while (bExit != true);
            CSpl_485.It.SetAcceptRunSpindle(EWay.L);

            if (bTimeOutFlag)
            {
                CErr.Show(eErr.LEFT_GRIND_SPINDLE_RUN_RPM_ERROR);
                return;
            }

            GU.Delay(iTm);

            CSpl_485.It.Write_Stop(EWay.L);
        }

        private void btnO_WInsStop_Click(object sender, EventArgs e)
        {
            // CSpl.It.Write_Stop(EWay.L);
            // 2023.03.15 Max
            CSpl_485.It.Write_Stop(EWay.L);
        }
    }
}
