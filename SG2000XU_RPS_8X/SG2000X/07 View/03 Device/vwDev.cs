using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwDev : UserControl
    {
        private int m_iPage = 0;

        private vwDev_01Lst m_vw01Lst = new vwDev_01Lst();
        private vwDev_02Prm m_vw02Prm = new vwDev_02Prm();
        private vwDev_03Set m_vw03Set = new vwDev_03Set();
        //210824 syc : 2004U
        private vwDev_02Prm_8IV2 m_vw08IV2 = new vwDev_02Prm_8IV2();

        public vwDev()
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

            m_vw01Lst.GoParm += Apply;
        }

        public void Open()
        {
            //if (CData.CurCompany != ECompany.Qorvo    && CData.CurCompany != ECompany.Qorvo_DZ && CData.CurCompany != ECompany.SkyWorks &&
            if (CData.CurCompany != ECompany.Qorvo    && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany != ECompany.SkyWorks && 
                CData.CurCompany != ECompany.ASE_K12  && CData.CurCompany != ECompany.ASE_KR   && 
                CData.CurCompany != ECompany.SST      && CData.CurCompany != ECompany.USI      &&
                CData.CurCompany != ECompany.SCK      && CData.CurCompany != ECompany.JSCK     && 
                CData.CurCompany != ECompany.JCET   )
            {
                CData.Dev.bDynamicSkip = true;
                //20190618 ghk_dfserver
                CData.Dev.bDfServerSkip = true;
            }

            //20191029 ghk_dfserver_notuse_df
            if(CDataOption.eDfserver == eDfserver.Use && CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {//DF서버 사용시 DF측정 안할 경우

                CData.Dev.bDynamicSkip = false;
            }

            //20190618 ghk_dfserver
            if (CDataOption.eDfserver == eDfserver.NotUse )
            { CData.Dev.bDfServerSkip = true; }

            rdbMn_Parm.Visible = false;
            rdbMn_Set.Visible = false;

            //20190703 ghk_dfserver
            // 2020.08.24 JSKim St
            //CData.Dev.bBcrKeyInSkip = false;
            if (CData.bDevOnlyView == false)
            {
                CData.Dev.bBcrKeyInSkip = false;
            }
            // 2020.08.24 JSKim Ed

            m_iPage = 1;

            _VwAdd();
            _HideMenu();
        }

        public void Close()
        {
            _VwClr();
        }

        /// <summary>
        /// 해당 뷰의 Dispose 함수 실행시 자동으로 실행됨
        /// </summary>
        public void Release()
        {
            m_vw01Lst.Dispose();
            m_vw02Prm.Dispose();
            m_vw03Set.Dispose();
        }

        public void Apply(bool bVal)
        {
            btn_Save.Visible = bVal;
            rdbMn_Parm.Visible = bVal;
            //20191204 ghk_level
                if (bVal && (int)CData.Lev >= CData.Opt.iLvDvPosView)
                { rdbMn_Set.Visible = true; }
                else
                { rdbMn_Set.Visible = false; }

            if (bVal)
            {
                rdbMn_Parm.Checked = true;

                _VwClr();

                m_iPage = 2;

                _VwAdd();

                m_vw02Prm.Set();
                m_vw03Set.Set();
                //210824 syc : 2004U
                m_vw08IV2.Set();
            }
            else
            {
                rdbMn_List.Checked = true;
            }
        }

        public void LinkSelectpage()
        {
            string sPath = GV.PATH_DEVICE + CData.DevGr + "\\";

            try
            {
                // 2020.08.24 JSKim St
                if (CData.bDevOnlyView == false)
                {
                // 2020.08.24 JSKim Ed
                    sPath = CData.DevCur;
                    CData.DevCur = sPath;
                    
                    //iRet = CDev.It.Load(sPath, false);  // 2021.09.07 lhs 잦은 로딩으로 부하가 많이 걸려 삭제
                    
                    int    Lastsp    = sPath.LastIndexOf("\\");
                    int    FindGroup = sPath.IndexOf("Device");
                    string Temp1     = sPath.Substring(FindGroup+7, Lastsp -(FindGroup+7));
                    CData.DevGr = Temp1;
                    CDev.It.m_sGrp = CData.DevGr;
                // 2020.08.24 JSKim St
                }
                // 2020.08.24 JSKim Ed
                m_iPage = 0;
                Apply(true);
            }
            catch (Exception ex)
            {

            }
        }

        public void LoadDeviceSecs(string sDev)
        {
            CData.JobChange = true;
            string sPath = GV.PATH_DEVICE;
            //string sRecipe = "GEM" + "\\";
            string sRecipe = sDev; //20200430 LCY
            if (!CData.Opt.bSecsUse) return;  //20200221 LCY

            try
            {
                sPath = sPath + sDev + ".dev";
                CData.DevCur = sPath;
                CDev.It.Load(sPath, true);
                //190211 ksg :

                char[] Separators = new char[] { '\\' };
                string[] SplitNum = sDev.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                //sRecipe = sRecipe + sDev; 20200430 LCY
                sRecipe = sDev;
                CBcr.It.SaveRecipe(sRecipe);
                CDev.It.m_sGrp = SplitNum[0];
                CData.DevGr = CDev.It.m_sGrp; //20200619 jhc : 그룹명 변수 값이 바뀌어, 디바이스 파일 저장 폴더가 바뀌는 현상 개선

                Apply(true);

                CData.L_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.L_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.L_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                CData.L_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.L_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.GrData[0].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                CData.GrData[0].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                CData.R_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.R_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.R_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                CData.R_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.R_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.GrData[1].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                CData.GrData[1].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                // 200316 mjy : Unit일 시 측정 배열 초기화
                if (CDataOption.Package == ePkg.Unit)
                {
                    for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                    {
                        CData.GrData[0].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[0].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[0].aUnit[iU].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[0].aUnit[iU].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[1].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[1].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[1].aUnit[iU].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                        CData.GrData[1].aUnit[iU].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                    }
                }

                if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.ONL].iSlot_info = new int[CData.Dev.iMgzCnt];
                
                // 2020.10.26 SungTae Start : Modify by Qorbo(Strip 배출 관련)
                //if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                if (!CData.LotInfo.bLotOpen)
                {
                    // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                    if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                        !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                    {
#if true //201116 jhc : DEBUG
                        int nOfLSlotCount = Math.Max(CData.Dev.iOffMgzCnt, CData.Dev.iMgzCnt);
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[nOfLSlotCount];
#else
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iOffMgzCnt];
#endif
                    }
                    else
                    {
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                    }
                }
                // 2020.10.26 SungTae End
                
                //20190624 ghk_qc
                //if(CData.CurCompany == eCompany.SkyWorks)
                if (CData.Opt.bQcUse)
                {
                    if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFR].iSlot_info = new int[CData.Dev.iMgzCnt];  //190325 ksg : Qc
                }

                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sDev;
            }
            catch (Exception ex)
            {

            }
            finally
            {
             
            }
            CData.bLastClick = true; //190509 ksg :

            // 2020.09.07 SungTae : Modify
            if(CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)   { CData.FrmMain.m_vwAuto._Draw();   }
            else                                                    { CData.FrmMain.m_vwAuto2._Draw();  }
            
            CData.JobChange = false;
        }

        private void _VwAdd()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Lst.Open();
                    pnl_Base.Controls.Add(m_vw01Lst);
                    btn_Save.Visible = false;
                    break;
                case 2:
                    m_vw02Prm.Open();
                    pnl_Base.Controls.Add(m_vw02Prm);
                    btn_Save.Visible = true;
                    break;
                case 3:
                    m_vw03Set.Open();
                    pnl_Base.Controls.Add(m_vw03Set);
                    btn_Save.Visible = true;
                    break;
            }
        }

        private void _VwClr()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Lst.Close();
                    break;
                case 2:
                    m_vw02Prm.Close();
                    break;
                case 3:
                    m_vw03Set.Close();
                    break;
            }

            pnl_Base.Controls.Clear();
        }

        private void _HideMenu()
        {
            // 2020.08.24 JSKim St
            //ELv RetLv = CData.Lev;
            //btn_Save.Enabled = (int)RetLv >= CData.Opt.iLvDvSave;
            if (CData.bDevOnlyView == true)
            {
                btn_Save.Enabled = false;
            }
            else
            {
                ELv RetLv = CData.Lev;
                btn_Save.Enabled = (int)RetLv >= CData.Opt.iLvDvSave;
            }
            // 2020.08.24 JSKim Ed
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

        private void rdbMn_CheckedChanged(object sender, EventArgs e)
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

        private void btn_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :

            //210928 pjh : 이전 Data 저장
            if (CData.CurCompany == ECompany.Qorvo)
            {
                CDev.It.SavePreData();
            }
            //

            //20191029 ghk_dfserver_notuse_df
            if (CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {
                CData.Dev.bDynamicSkip = CData.Dev.bDfServerSkip;
            }

            // 2021.07.22 SungTae : [수정] ASE-KR VOC로 Engineer Level에서도 Parameter 수정 가능하도록 Company Option 추가
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

            string sPath = "";
            //190220 ksg : 추가
            string[] Temp;
            string[] Temp1;
            Temp = CData.DevCur.Split('\\');
            Temp1 = Temp[5].Split('.');

            // 2022.07.25 SungTae Start : [추가] (ASE-KR 개발건)
            // 설정된 Step 수량을 제외한 나머지 Step UI 비활성화 관련 Save 시 저장하기 전 Message Pop-up 하여 알림 기능 추가
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                if (CMsg.Show(eMsg.Notice, "Notice", "Do you want to save the current settings?") == DialogResult.Cancel)
                {
                    return;
                }
            }
            // 2022.07.25 SungTae End

            m_vw02Prm.Get();
            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            {
                string strResult = m_vw02Prm.CheckMeasurePointCount(); //스트립 측정 포인트 수 체크 : 반드시 m_vw02Prm.Get() 호출 후 본 함수를 호출해야 정확한 측정 포인트 수가 파악됨
                if (0 < strResult.Length)
                {
                    CMsg.Show(eMsg.Error, "Error", strResult); //측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과(위험)
                    //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                    m_vw02Prm.ReloadEtc(); //BCR 옵션 변경 후 SAVE 실패 시 원래 옵션을 다시 표시해 주기 위함
                    // 
                    return;
                }
            }
            //

            // 200803 jym : 휠파일 검사
            if (CData.CurCompany == ECompany.ASE_KR && GV.DEV_WHL_SYNC)
            {
                if (CData.Dev.aData[(int)EWay.L].sWhl == "" || CData.Dev.aData[(int)EWay.R].sWhl == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Check Wheel File.");
                    return;
                }
            }

            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseWheelLossCorrect) //(DEVICE > PARAM > ADVANCED 메뉴 표시) + (드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능 사용)
            {
                for (int i=0; i < 2; i++)
                {
                    double dWheelLossSum = 0.0;
                    for (int j=0; j < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; j++)
                    {
                        dWheelLossSum += CData.Dev.aData[i].dWheelLoss[j]; //드레싱 후 N 번째까지의 Wheel Loss 설정값 총 합
                    }
                    if (CData.Dev.aData[i].dTotalWheelLossLimit < dWheelLossSum)
                    {
                        if (i == 0) { CMsg.Show(eMsg.Error, "Error", "Sum of Left Wheel Loss is Over the Total Wheel Loss Limit."); }
                        else        { CMsg.Show(eMsg.Error, "Error", "Sum of Right Wheel Loss is Over the Total Wheel Loss Limit."); }

                        m_vw02Prm.ReloadWheelLossSetValue(); //Wheel Loss Correct 설정값이 Total Wheel Loss Limit 값 초과 시 저장하지 않고 기존 설정값 재로딩

                        return;
                    }
                }
            }
            //

            m_vw03Set.Get();

            //20190703 ghk_targetsetting
            bool    bCheck = false;
            double  dBfDep = 0.0;

            // 2020.09.08 SungTae : 3 Step Mode 기능 추가
            int iStepMaxCnt = 0;
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;   }
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3; }

            if (CData.Dev.aData[0].eGrdMod == eGrdMode.Target)
            {
                if (CData.Dev.bDual == eDual.Normal)
                {
                    // 2021.07.28 lhs Start :  Rough 체크
                    if (CDataOption.UseNewSckGrindProc)
                    {
                        bool bMold = (CData.Dev.aData[0].eBaseOnThick == EBaseOnThick.Mold);
                        bCheck = CheckRoughStep(0, bMold);
                    }
                    else
                    // 2021.07.28 lhs End
                    {
                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)  //DF 스킵
                        {
                            for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                            {
                                if (CData.Dev.aData[0].aSteps[i].bUse)
                                {
                                    if (CData.Dev.aData[0].dTotalTh < CData.Dev.aData[0].aSteps[i].dTotalDep ||
                                         ((i > 0) && (dBfDep < CData.Dev.aData[0].aSteps[i].dTotalDep)))
                                    {
                                        bCheck = true;
                                    }
                                    dBfDep = CData.Dev.aData[0].aSteps[i].dTotalDep;
                                }
                            }
                        }
                        else //DF 사용
                        {
                            dBfDep = 0.0;
                            for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                            {
                                if (CData.Dev.aData[0].aSteps[i].bUse)
                                {
                                    if (CData.Dev.aData[0].dMoldTh < CData.Dev.aData[0].aSteps[i].dTotalDep ||
                                        ((i > 0) && (dBfDep < CData.Dev.aData[0].aSteps[i].dTotalDep)))
                                    {
                                        bCheck = true;
                                    }
                                    dBfDep = CData.Dev.aData[0].aSteps[i].dTotalDep;
                                }
                            }
                        }
                    }

                    if (bCheck)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Checking Target Value Entered");
                        return;
                    }
                }
                else  //듀얼
                {
                    //-----------
                    // Left
                    //-----------
                    // 2021.07.28 lhs Start :  Rough 체크
                    if (CDataOption.UseNewSckGrindProc)
                    {
                        bool bMold = (CData.Dev.aData[0].eBaseOnThick == EBaseOnThick.Mold);
                        bCheck = CheckRoughStep(0, bMold); // Left
                    }
                    else // 기존 로직
                    // 2021.07.28 lhs End
                    {
                        //20191029 ghk_dfserver_notuse_df
                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf) //DF 스킵
                        {
                            dBfDep = 0.0;
                            for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                            {
                                if (CData.Dev.aData[0].aSteps[i].bUse)
                                {
                                    if (CData.Dev.aData[0].dTotalTh < CData.Dev.aData[0].aSteps[i].dTotalDep ||
                                        ((i > 0) && (dBfDep < CData.Dev.aData[0].aSteps[i].dTotalDep)))
                                    {
                                        bCheck = true;
                                    }
                                    dBfDep = CData.Dev.aData[0].aSteps[i].dTotalDep;
                                }
                            }
                        }
                        else    //DF 사용
                        {
                            dBfDep = 0.0;
                            for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                            {
                                if (CData.Dev.aData[0].aSteps[i].bUse)
                                {
                                    if (CData.Dev.aData[0].dMoldTh < CData.Dev.aData[0].aSteps[i].dTotalDep ||
                                        ((i > 0) && (dBfDep < CData.Dev.aData[0].aSteps[i].dTotalDep)))
                                    {
                                        bCheck = true;
                                    }
                                    dBfDep = CData.Dev.aData[0].aSteps[i].dTotalDep;
                                }
                            }
                        }
                    }

                    if (bCheck)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Checking Left Target Value Entered");
                        return;
                    }

                    //-----------
                    // Right
                    //-----------
                    // 2021.07.28 lhs Start :  Rough 체크
                    if (CDataOption.UseNewSckGrindProc)
                    {
                        bool bMold = (CData.Dev.aData[1].eBaseOnThick == EBaseOnThick.Mold);
                        bCheck = CheckRoughStep(1, bMold); // right
                    }
                    else
                    // 2021.07.28 lhs End
                    {
                        //20191029 ghk_dfserver_notuse_df
                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                        {
                            dBfDep = 0.0;
                            for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                            {
                                if (CData.Dev.aData[1].aSteps[i].bUse)
                                {
                                    if (CData.Dev.aData[1].dTotalTh < CData.Dev.aData[1].aSteps[i].dTotalDep ||
                                        ((i > 0) && (dBfDep < CData.Dev.aData[1].aSteps[i].dTotalDep)))
                                    {
                                        bCheck = true;
                                    }
                                    dBfDep = CData.Dev.aData[1].aSteps[i].dTotalDep;
                                }
                            }
                        }
                        else
                        {
                            dBfDep = 0.0;
                            for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                            {
                                if (CData.Dev.aData[1].aSteps[i].bUse)
                                {
                                    if (CData.Dev.aData[1].dMoldTh < CData.Dev.aData[1].aSteps[i].dTotalDep ||
                                        ((i > 0) && (dBfDep < CData.Dev.aData[1].aSteps[i].dTotalDep)))
                                    {
                                        bCheck = true;
                                    }
                                    dBfDep = CData.Dev.aData[1].aSteps[i].dTotalDep;
                                }
                            }
                        }
                    }
                    if (bCheck)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Checking Right Target Value Entered");
                        return;
                    }
                }
            }
            // 2021.11.02 lhs Start : TopDown 모드 그라인드양 체크
            else if (CData.Dev.aData[0].eGrdMod == eGrdMode.TopDown)
			{
				double dSumDep = 0.0;
                double dMoldTh = 0.0;

				if (CData.Dev.bDual == eDual.Normal)
				{
                    dMoldTh = CData.Dev.aData[0].dTotalTh - CData.Dev.aData[0].dPcbTh;

                    for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
					{
						if (CData.Dev.aData[0].aSteps[i].bUse)
						{
                            dSumDep += CData.Dev.aData[0].aSteps[i].dTotalDep;
						}
                    }

                    if (dMoldTh <= dSumDep)
					{
						bCheck = true;
						CMsg.Show(eMsg.Error, "Error", "Checking TopDown Value Entered");
                        return;
                    }
                }
                else  //듀얼
                {
                    dSumDep = 0.0;  // Left, Right 전체 체크용
                    //-----------------
                    // Left
                    //-----------------
                    double dSumDep_L = 0.0; // Left 체크용
                    double dMoldTh_L = CData.Dev.aData[0].dTotalTh - CData.Dev.aData[0].dPcbTh;

                    for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                    {
                        if (CData.Dev.aData[0].aSteps[i].bUse)
                        {
                            dSumDep_L   += CData.Dev.aData[0].aSteps[i].dTotalDep;
                            dSumDep     += CData.Dev.aData[0].aSteps[i].dTotalDep;
                        }
                    }
                    if (dMoldTh_L <= dSumDep_L)
                    {
                        bCheck = true;
                        CMsg.Show(eMsg.Error, "Error", "Checking Left TopDown Value Entered");
                        return;
                    }
                    
                    //-----------------
                    // Right
                    //-----------------
                    double dSumDep_R = 0.0; // Right 체크용
                    double dMoldTh_R = CData.Dev.aData[1].dTotalTh - CData.Dev.aData[1].dPcbTh;

                    for (int i = 0; i < iStepMaxCnt; i++)       // 2020.09.08 SungTae : Modify
                    {
                        if (CData.Dev.aData[1].aSteps[i].bUse)
                        {
                            dSumDep_R   += CData.Dev.aData[1].aSteps[i].dTotalDep;
                            dSumDep     += CData.Dev.aData[1].aSteps[i].dTotalDep;
                        }
                    }
                    if (dMoldTh_R <= dSumDep_R)
                    {
                        bCheck = true;
                        CMsg.Show(eMsg.Error, "Error", "Checking Right TopDown Value Entered");
                        return;
                    }

                    //-----------------
                    // Left + Right
                    //-----------------
                    if (dMoldTh_L <= dSumDep) // dMoldTh_L : 전체 Mold thickness
                    {
                        bCheck = true;
                        CMsg.Show(eMsg.Error, "Error", "Checking Left/Right TopDown Value Entered");
                        return;
                    }
                }
            }
            // 2021.11.02 lhs End : TopDown 모드 그라인드양 체크


            GU.Delay(100);

            //20191029 ghk_dfserver_notuse_df
            if (CDataOption.eDfserver == eDfserver.Use)
            {
                //191101 ksg :
                if(!CData.Dev.bDfServerSkip)
                {
                    if(CData.Dev.bBcrSkip)
                    {
                        CMsg.Show(eMsg.Error, "Error", "If use dynamicfunction server, must use bcr");
                        return;
                    }
                }
                
            }

#if true //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 체크
            if ((CDataOption.Dryer == eDryer.Rotate) && (!Check_DryTime()))
            {
                CMsg.Show(eMsg.Error, "Error", "Dry Rotation Time Over!!!\nCheck Dry Rotate Velocity or Dry Rotate Count.");
                return;
            }
#endif
            // 2021.03.30 lhs Start : Water Nozzle 분사시의 회전 동작 타임아웃 시간 체크
            if ((CDataOption.Dryer == eDryer.Rotate) && (!Check_DryWtNozzleTime()))
            {
                CMsg.Show(eMsg.Error, "Error", "Dry Water Nozzle Rotation Time Over!!!\nCheck Dry Water Nozzle Rotate Velocity or Dry Water Nozzle Rotate Count.");
                return;
            }
            // 2021.03.30 lhs End

            if (CData.DevGr == "")  {   CData.DevGr = Temp[4];  }  // 그룹명

            //210928 pjh : Device 저장 시 Grinding Mode가 바뀌었을 때 Message표시
            if (CData.CurCompany == ECompany.Qorvo)
            {
                //if (CDev.It.aPreGrd[0] != CData.Dev.aData[0].eGrdMod.ToString() && CDev.It.aPreGrd[1] != CData.Dev.aData[1].eGrdMod.ToString())
                if (CDev.It.aPreGrd[0] != CDev.It.aNowGrd[0] && CDev.It.aPreGrd[1] != CDev.It.aNowGrd[1])
                {
                    if (CMsg.Show(eMsg.Warning, "Warning", "Grinding Mode is changed" + "\n" + "Left : " + CDev.It.aPreGrd[0] + " -> " + CData.Dev.aData[0].eGrdMod + "\n" + "Right : " + CDev.It.aPreGrd[1] + " -> " + CData.Dev.aData[1].eGrdMod + "\n" + "Are you sure to save data?") == DialogResult.Cancel)
                    {
                        if(CDev.It.aPreGrd[0] == "Top" || CDev.It.aPreGrd[0] == "TopDown")
                        {
                            CData.Dev.aData[0].eGrdMod = eGrdMode.TopDown;
                        }
                        else if (CDev.It.aPreGrd[0] == "Normal" || CDev.It.aPreGrd[0] == "Target")
                        {
                            CData.Dev.aData[0].eGrdMod = eGrdMode.Target;
                        }
                        if (CDev.It.aPreGrd[1] == "Top" || CDev.It.aPreGrd[1] == "TopDown")
                        {
                            CData.Dev.aData[1].eGrdMod = eGrdMode.TopDown;
                        }
                        else if (CDev.It.aPreGrd[1] == "Normal" || CDev.It.aPreGrd[1] == "Target")
                        {
                            CData.Dev.aData[1].eGrdMod = eGrdMode.Target;
                        }
                        return;
                    }
                }
                else if (CDev.It.aPreGrd[0] != CDev.It.aNowGrd[0])
                {
                    if (CMsg.Show(eMsg.Warning, "Warning", "Grinding Mode is changed" + "\n" + "Left : " + CDev.It.aPreGrd[0] + " -> " + CData.Dev.aData[0].eGrdMod + "\n" + "Are you sure to save data?") == DialogResult.Cancel)
                    {
                        if (CDev.It.aPreGrd[0] == "Top" || CDev.It.aPreGrd[0] == "TopDown")
                        {
                            CData.Dev.aData[0].eGrdMod = eGrdMode.TopDown;
                        }
                        else if (CDev.It.aPreGrd[0] == "Normal" || CDev.It.aPreGrd[0] == "Target")
                        {
                            CData.Dev.aData[0].eGrdMod = eGrdMode.Target;
                        }
                        return;
                    }
                }
                else if (CDev.It.aPreGrd[1] != CDev.It.aNowGrd[1])
                {
                    if (CMsg.Show(eMsg.Warning, "Warning", "Grinding Mode is changed" + "\n" + "Right : " + CDev.It.aPreGrd[1] + " -> " + CData.Dev.aData[1].eGrdMod + "\n" + "Are you sure to save data?") == DialogResult.Cancel)
                    {
                        if (CDev.It.aPreGrd[1] == "Top" || CDev.It.aPreGrd[1] == "TopDown")
                        {
                            CData.Dev.aData[1].eGrdMod = eGrdMode.TopDown;
                        }
                        else if (CDev.It.aPreGrd[1] == "Normal" || CDev.It.aPreGrd[1] == "Target")
                        {
                            CData.Dev.aData[1].eGrdMod = eGrdMode.Target;
                        }
                        return;
                    }
                }
            }
            //

            sPath += GV.PATH_DEVICE + string.Format("{0}\\{1}.dev", CData.DevGr, CData.Dev.sName);
            CDev.It.Save(sPath);

            m_vw02Prm.Set();
            m_vw03Set.Set();
            //210824 syc : 2004U
            m_vw08IV2.Set();

            CData.L_GRD    .m_aInspBf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
            CData.L_GRD    .m_aInspAf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
            CData.L_GRD    .m_abMeaBfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
            CData.L_GRD    .m_abMeaAfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.L_GRD    .m_aInspTemp  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.GrData[0].aMeaBf       = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
            CData.GrData[0].aMeaAf       = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

            CData.R_GRD    .m_aInspBf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
            CData.R_GRD    .m_aInspAf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
            CData.R_GRD    .m_abMeaBfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
            CData.R_GRD    .m_abMeaAfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.R_GRD    .m_aInspTemp  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.GrData[1].aMeaBf       = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
            CData.GrData[1].aMeaAf       = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

            // 200316 mjy : Unit일 시 측정 배열 초기화
            if (CDataOption.Package == ePkg.Unit)
            {
                for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                {
                    CData.GrData[0].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[0].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[0].aUnit[iU].aErrBf = new bool  [CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[0].aUnit[iU].aErrAf = new bool  [CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[1].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[1].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[1].aUnit[iU].aErrBf = new bool  [CData.Dev.iRow, CData.Dev.iCol];
                    CData.GrData[1].aUnit[iU].aErrAf = new bool  [CData.Dev.iRow, CData.Dev.iCol];
                }
            }

            if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.ONL].iSlot_info = new int[CData.Dev.iMgzCnt];

            // 2020.10.26 SungTae Start : Modify by Qorbo(Strip 배출 관련)
            //if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
            if (!CData.LotInfo.bLotOpen)
            {
                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                if((CData.CurCompany == ECompany.Qorvo      || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT   || CData.CurCompany == ECompany.Qorvo_NC) &&
                    !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                {
#if true //201116 jhc : DEBUG
                    int nOfLSlotCount = Math.Max(CData.Dev.iOffMgzCnt, CData.Dev.iMgzCnt);
                    CData.Parts[(int)EPart.OFL].iSlot_info = new int[nOfLSlotCount];
#else
                    CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iOffMgzCnt];
#endif
                }
                else
                {
                    CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                }
            }
            // 2020.10.26 SungTae End

            if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFR].iSlot_info = new int[CData.Dev.iMgzCnt]; //190809 ksg : 추가 
            //20190624 ghk_qc
            //if(CData.CurCompany == eCompany.SkyWorks)
            if (CData.Opt.bQcUse)
            {
                if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFR].iSlot_info = new int[CData.Dev.iMgzCnt];  //190325 ksg : Qc
            }
            //CEID 999023 Recipe Parameter Changed
            string sRecipe = CDev.It.m_sGrp + "\\" + CData.Dev.sName;

            if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))// 20200707 LCY
            {
                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sRecipe;
                CData.GemForm.Set_PPIDE(CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
            }
            CMsg.Show(eMsg.Notice, "Notice", "Device file save complete!!");
        }

        private bool CheckRoughStep(int nLtRt, bool bMoldTh)
        {
            bool    bCheck      = false;
            int     iStepMaxCnt = 0;
            double  dBfDep      = 0.0;
            double  dThick      = 0.0;

            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;      }
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3;    }

            if (bMoldTh)    dThick = CData.Dev.aData[nLtRt].dMoldTh;
            else            dThick = CData.Dev.aData[nLtRt].dTotalTh;

            for (int i = 0; i < iStepMaxCnt; i++)
			{
                if (CData.Dev.aData[nLtRt].aSteps[i].bUse)
				{
					if ( dThick < CData.Dev.aData[nLtRt].aSteps[i].dTotalDep ||
						 ((i > 0) && (dBfDep < CData.Dev.aData[nLtRt].aSteps[i].dTotalDep)))
					{
						bCheck = true;
					}
					dBfDep = CData.Dev.aData[nLtRt].aSteps[i].dTotalDep;
				}
			}
			return bCheck;
		}

#if true //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 체크
        private bool Check_DryTime()
        {
            int rpm = CData.Dev.iDryRPM;
            int cnt = CData.Dev.iDryCnt;
            if( rpm == 0 ) rpm = 10; //최소값 10 rpm
            if( cnt == 0 ) cnt = 1;  //최소값 1회전
            double rotationTime = 1000*(60.0/(double)rpm)*(double)cnt;
            if ( rotationTime > GV.DRYROTATION_TMOUT)
            {
                return false;
            }
            return true;
        }
#endif
        // 2021.03.30 lhs Start : Water Nozzle의 회전 동작 타임아웃 시간 체크
        private bool Check_DryWtNozzleTime()
        {
            int rpm = CData.Dev.iDryWtNozzleRPM;
            int cnt = CData.Dev.iDryWtNozzleCnt;
            if (rpm == 0) rpm = 10; //최소값 10 rpm
            if (cnt == 0) cnt = 1;  //최소값 1회전
            double rotationTime = 1000 * (60.0 / (double)rpm) * (double)cnt;
            if (rotationTime > GV.DRYROTATION_TMOUT)
            {
                return false;
            }
            return true;
        }
        // 2021.03.30 lhs End


    }
}
