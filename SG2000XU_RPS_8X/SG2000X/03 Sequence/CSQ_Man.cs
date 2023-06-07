using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SG2000X
{
    /// <summary>
    /// Manual sequence class
    /// </summary>
    public class CSQ_Man : CStn<CSQ_Man>
    {
        private readonly int TIMEOUT = 30000;
        public  ESeq Seq        = ESeq.Idle;
        private ESeq m_iSeq     = ESeq.Idle;

        private int  m_iStep    = 0;
        private int  m_iPreStep = 0;
        public string Step { get { return Convert.ToString(m_iSeq); } }

        private bool m_bOnlH, m_bInrH, m_bOnpH, m_bGrlH, m_bGrrH, m_bOfpH, m_bDryH, m_bOflH = false;
        private bool m_bTblOk;

        private int m_iCnt = 0;
        private double m_dPos = 0;

        private CTim m_mTiOut   = new CTim();
        private CTim m_mPrb     = new CTim();
        private CTim m_mIdleDly = new CTim();
        //20191128 ghk_warmup_error
        private CTim m_mFlowTime = new CTim();
        //

        private CTim m_Timout_485 = new CTim(); //  2023.03.15 Max

        private CSQ_Man() { }

        //190108 ksg : 메뉴얼 스톱 추가 
        public bool bStop = false;

        //190410 ksg :
        public bool bBtnShow = false;

        //20191104 ghk_doorlock
        public bool m_bDoor = false;

        //210309 pjh : Set up strip
        public bool bOnMGZPlaceDone = false;
        public bool bOffMGZBtmPlaceDone = false;
        public bool bOffMGZTopPlaceDone = false;
        //

        // 2021-06-24, jhlee, Dual-Dressing에서 Timeout 연장을 위한 Flag
        public bool bDDressFinish_L = false;        // 왼쪽 Table dressing 작업완료 여부
        public bool bDDressFinish_R = false;        // 오른쪽 Table dressing 작업완료 여부

        //220321 pjh : Dual Grinding Flag
        public bool bDGrindFinish_L = false;        //왼쪽 Table Grinding 작업 완료 여부
        public bool bDGrindFinish_R = false;        //오른쪽 Table Grinding 작업 완료 여부


        //public tLog m_LogVal = new tLog(); //190218 ksg :
        /// <summary>
        /// All Part Initialize Step Variable
        /// 모든 파트의 스텝 변수 초기화
        /// </summary>
        private void Init_Part()
        {
            CSq_OnL.It   .Init_Cyl();
            CSq_Inr.It   .Init_Cyl();
            CSq_OnP.It   .Init_Cyl();
            CData  .L_GRD.Init_Cyl();
            CData  .R_GRD.Init_Cyl();
            CSq_OfP.It   .Init_Cyl();
            CSq_Dry.It   .Init_Cyl();
            CSq_OfL.It   .Init_Cyl();
        }

        public void Cycle()
        {
            //Manual 동작에서 버튼 입력 부
            bool xbStarIO = CIO.It.Get_X(eX.SYS_BtnStart);
            bool xbStopIO = CIO.It.Get_X(eX.SYS_BtnStop);
            bool xbResetIO = CIO.It.Get_X(eX.SYS_BtnReset);

            //CData.L_GRD.Encoder_Log_Save();  0404 LCY
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running)
            { 
                Seq = ESeq.Idle; //190430 ksg : 추가
                return; 
            }

            // 2021-02-05, jhLee : Manual 기능 수행시에도 Light Curatin check 기능 추가
            CSq_OnL.It.CheckLightCurtainPause();    // 감지시 모션 동작 중지, 해제시 동작 재개
            CSq_OfL.It.CheckLightCurtainPause();


            if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                if (Seq == ESeq.GRD_Dual_Grinding && (!bDGrindFinish_L || !bDGrindFinish_R))
                {
                    //220609 pjh : ASE KH Dual Grinding 시 한쪽에서 Error 발생하면 반대쪽에서 작업 정상 종료 후 Stop
                }
                else
                {
                    m_iStep = 0;
                    m_iPreStep = 0;
                    m_iSeq = Seq = ESeq.Idle;
                    // 200908 pjh : Skyworks에서는 기능 안씀
                    if (CData.CurCompany != ECompany.SkyWorks && CSQ_Main.It.m_bBtReset)
                    {
                        CSQ_Main.It.m_iStat = EStatus.Reset;
                    }
                }
            }
            if(xbStopIO)    //20200427 lks 버튼 정지 입력
            {
                if (!bStop) {   bStop = true;   }
            }

            //---------------------------------------------
            // 도어락
            //---------------------------------------------
            if(CDataOption.ManualLock == eManualDoorLock.Use)
            { 
                if (CData.CurCompany == ECompany.Qorvo      ||
                    CData.CurCompany == ECompany.Qorvo_DZ   ||
                    CData.CurCompany == ECompany.Qorvo_RT   ||  // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    CData.CurCompany == ECompany.Qorvo_NC   ||  // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    CData.CurCompany == ECompany.SST        || 
                    CData.CurCompany == ECompany.SkyWorks   //|| 
                    //CData.CurCompany == ECompany.SCK        ||  // 2022.06.08 lhs : SCK, JSCK 추후에 추가
                    //CData.CurCompany == ECompany.JSCK
                    ) 
                {
                    // 도어 체크
                    if(!CSQ_Main.It.CheckDoor() && Seq != ESeq.Idle)
                    {
                        m_iStep = 0;
                        m_iPreStep = 0;
                        m_iSeq = Seq = ESeq.Idle;
                        CMsg.Show(eMsg.Warning,"Warning","Door Close Please");
                    	return;
					}
                }
            }
            //---------------------------------------------


            // 커버 체크
            if (m_iSeq != ESeq.Idle && !CData.Opt.bCoverSkip && !CSQ_Main.It.CheckCover())
            {
                m_iStep      = 0;
                m_iPreStep   = 0;
                m_iSeq = Seq = ESeq.Idle;
                CErr.Show(eErr.MAIN_COVER_NOT_CLOSE);
                return;
            }

            // 동작별 스텝 및 기타 초기화 영역
            if (m_iSeq == ESeq.Idle)
            {
                if (Seq == ESeq.Idle)
                { return; }
                else
                {
                    CSQ_Main.It.m_iStat = EStatus.Manual;
                    if      (Seq == ESeq.Warm_Up || Seq == ESeq.All_Home)   { m_iStep = 10; m_iPreStep = 0; }
                    else if (ESeq.Warm_Up   < Seq && Seq < ESeq.ONL_Wait)   { Init_Part();                  }   // All Part Cycle
                    else if (ESeq.ONL_Wait <= Seq && Seq < ESeq.INR_Wait)   { CSq_OnL.It.Init_Cyl();        }   // On Loader Part
                    else if (ESeq.INR_Wait <= Seq && Seq < ESeq.ONP_Wait)   { CSq_Inr.It.Init_Cyl();        }   // In Rail Part
                    else if (ESeq.ONP_Wait <= Seq && Seq < ESeq.GRD_Wait)   { CSq_OnP.It.Init_Cyl();        }   // On Loader Picker Part
                    else if (ESeq.GRD_Wait <= Seq && Seq < ESeq.GRL_Wait)                                       // Dual Grind Part
                    {
                        CData.L_GRD.Init_Cyl();
                        CData.R_GRD.Init_Cyl();
                    }
                    else if (ESeq.GRL_Wait <= Seq && Seq < ESeq.GRR_Wait)    // Grind Left Part
                    {
                        CData.L_GRD.Init_Cyl();
                        if(Seq == ESeq.GRL_Manual_Bcr || Seq == ESeq.GRL_Manual_GrdBcr)
                        {
                            CSq_OnP.It.Init_Cyl();
                        }
                    }
                    else if (ESeq.GRR_Wait <= Seq && Seq < ESeq.OFP_Wait)    // Grind Right Part
                    {
                        CData.R_GRD.Init_Cyl();
                        if(Seq == ESeq.GRR_Manual_Bcr || Seq == ESeq.GRR_Manual_GrdBcr)
                        {
                            CSq_OnP.It.Init_Cyl();
                        }
                    }
                    else if (ESeq.OFP_Wait <= Seq && Seq < ESeq.DRY_Wait)   { CSq_OfP.It.Init_Cyl(); }  // Off Loader Picker Part
                    else if (ESeq.DRY_Wait <= Seq && Seq < ESeq.OFL_Wait)   { CSq_Dry.It.Init_Cyl(); }  // Dry Part
                    else if (ESeq.OFL_Wait <= Seq)                          { CSq_OfL.It.Init_Cyl(); }  // Off Loader Part

                    if (CDataOption.LotEndPlcMgz == eLotEndPlaceMgz.Use)
                    {//LotEnd 버튼 클릭시 매거진 버리는 기능
                        if(Seq == ESeq.ONLOFL_Place)
                        {//매거진 버리는 시퀀스 일때 온로더, 오프로더 m_iStep 초기화
                            CSq_OnL.It.Init_Cyl();
                            CSq_OfL.It.Init_Cyl();
                        }
                    }

                    m_iSeq = Seq;

                    //190327 : 막음
                    //m_LogVal.sMsg = "m_iSeq = " + m_iSeq.ToString();
                    //SaveLog();
                }
            } //end : if (m_iSeq == ESeq.Idle)

            //190108 ksg : 메뉴얼 스톱 추가 
            if (bStop)
            {   
                bStop = false;
                
                if (m_iSeq == ESeq.GRL_Grinding || m_iSeq == ESeq.GRL_Dressing)
                {
                    CData.L_GRD.ToStopManual();
                }
                else if (m_iSeq == ESeq.GRR_Grinding || m_iSeq == ESeq.GRR_Dressing)
                {
                    CData.R_GRD.ToStopManual();
                }
                else if (   (ESeq.GRL_Dressing      <= m_iSeq && m_iSeq <= ESeq.GRL_WaterKnife)             ||
                            (ESeq.GRL_Table_Measure <= m_iSeq && m_iSeq <= ESeq.GRL_Table_Grinding_Work)    ||
                            (ESeq.GRR_Dressing      <= m_iSeq && m_iSeq <= ESeq.GRR_WaterKnife)             ||
                            (ESeq.GRR_Table_Measure <= m_iSeq && m_iSeq <= ESeq.GRR_Table_Grinding_Work)    ||
                            (m_iSeq == ESeq.DRY_Run) )
                {
                    return;
                }
                else
                {
                    //190308 ksg :
                    if (m_iSeq == ESeq.Warm_Up)
                    {
                        //CSpl.It.Write_Stop(EWay.L);
                        //CSpl.It.Write_Stop(EWay.R);
                        // 2023.03.15 Max
                        CSpl_485.It.Write_Stop(EWay.L);
                        CSpl_485.It.Write_Stop(EWay.R);

                        CIO.It.Set_Y(eY.GRDL_SplWater,      false);
                        CIO.It.Set_Y(eY.GRDL_SplBtmWater,   false);
                        CIO.It.Set_Y(eY.GRDR_SplWater,      false);
                        CIO.It.Set_Y(eY.GRDR_SplBtmWater,   false);
                        //200515 myk : Wheel Cleaner Water 추가
                        if (CDataOption.IsWhlCleaner && !CData.Opt.bWhlClnSkip)
                        {
                            CIO.It.Set_Y(eY.GRDL_WhlCleaner, false);
                            CIO.It.Set_Y(eY.GRDR_WhlCleaner, false);
                        }
                        
                    }

                    CSQ_Main.It.m_iStat = EStatus.Idle; //190116 ksg : 추가
                    Seq = ESeq.Idle;
                    m_iSeq = ESeq.Idle;
                    //190410 ksg :
                    bBtnShow = true;
                    _SetLog("Manual stop.");

                    CMsg.Show(eMsg.Notice, "NOTICE", CLang.It.GetConvertMsg("Manual Stop Now, Move the stopped module to the wait position please"));
                }
            } //end : if (bStop)

            //if(CSQ_Main.It.m_iStat == EStatus.Manual){
            // 20200605 Encoder Log 저장 하기 위함.LCY
            //CMot.It.ReadMemoryLog_Data();
            //}

            // 2021.09.23 SungTae Start : [추가]
            bool bUse = false;
            int nIdx = 0;
            // 2021.09.23 SungTae End

            switch (m_iSeq)
            {
                default:
                    break;

                case ESeq.Warm_Up:
                    {
                        if (Cyl_WarmUp())
                        {
                            Seq     = ESeq.Idle; //201109 pjh : Warm Up Sequence 변경
                            m_iSeq  = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                _SetLog(">>>>> Warm up finish.");
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                bBtnShow = true;

                                if(!CData.IsATW)    //201109 pjh : Manual Warm Up 일때만 Message 표시
                                {
                                    CMsg.Show(eMsg.Notice, "Notice", "Warm Up OK");
                                }
                            }
                            CData.IsATW = false;
                            CData.AutoWarmPre = DateTime.Now; //201109 pjh : Warm Up 종료 후 Auto Warm Up 기준 시간 초기화

                            WaterOnOff(false);
                        }
                        break;
                    }

                case ESeq.All_Servo_On:
                    {
                        bool r1 = CSq_OnL.It   .Cyl_Servo(true);
                        bool r2 = CSq_Inr.It   .Cyl_Servo(true);
                        bool r3 = CSq_OnP.It   .Cyl_Servo(true);
                        bool r4 = CData  .L_GRD.Cyl_Servo(true);
                        bool r5 = CData  .R_GRD.Cyl_Servo(true);
                        bool r6 = CSq_OfP.It   .Cyl_Servo(true);
                        bool r7 = CSq_Dry.It   .Cyl_Servo(true);
                        bool r8 = CSq_OfL.It   .Cyl_Servo(true);
                        if (r1 && r2 && r3 && r4 && r5 && r6 && r7 && r8)
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> All servo on finish.");
                                //190410 ksg :
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("All servo on complete."));
                            }
                        }
                        break;
                    }

                case ESeq.All_Servo_Off:
                    {
                        bool r1 = CSq_OnL.It   .Cyl_Servo(false);
                        bool r2 = CSq_Inr.It   .Cyl_Servo(false);
                        bool r3 = CSq_OnP.It   .Cyl_Servo(false);
                        bool r4 = CData  .L_GRD.Cyl_Servo(false);
                        bool r5 = CData  .R_GRD.Cyl_Servo(false);
                        bool r6 = CSq_OfP.It   .Cyl_Servo(false);
                        bool r7 = CSq_Dry.It   .Cyl_Servo(false);
                        bool r8 = CSq_OfL.It   .Cyl_Servo(false);
                        if (r1 && r2 && r3 && r4 && r5 && r6 && r7 && r8)
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> All servo off finish.");
                                //190410 ksg :
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("All servo off complete."));
                            }
                        }
                        break;
                    }

                case ESeq.All_Home:
                    if (Cyl_AllHome())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> All home finish.");
                            //190410 ksg :
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("All home complete."));
                        }
                    }
                    break;

                case ESeq.Reset:
                    {
                        for (int i = CMot.It.AxS; i <= CMot.It.AxL; i++)
                        {
                            CMot.It.Set_Clr(i);
                        }
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        CSQ_Main.It.m_iStat = EStatus.Idle;
                        _SetLog(">>>>> Reset finish.");
                        //190410 ksg :
                        bBtnShow = true;
                        CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("Reset OK"));
                        break;
                    }

                //20191209 ghk_lotend_placemgz
                case ESeq.ONLOFL_Place:
                    {
                        bool bOnl_Place = CSq_OnL.It.Cyl_PlaceLotEnd();
                        bool bOfl_Place = CSq_OfL.It.Cyl_PlaceLotEnd();
                        if (bOnl_Place && bOfl_Place)
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> ONL, OFL Place finish.");

                                bBtnShow = true;
                            }
                        }
                        break;
                    }
                //

                #region On Loader Part
                case ESeq.ONL_Wait:
                    if (CSq_OnL.It.Cyl_Wait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONL Wait finish.");
                            //190410 ksg :
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader wait ok"));
                        }
                    }
                    break;

                case ESeq.ONL_Home:
                    if (CSq_OnL.It.Cyl_Home())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONL Home finish.");
                            //190410 ksg :
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader home ok"));
                        }
                    }
                    break;

                case ESeq.ONL_Pick:
                    //20200120 ksg : RFID 기능 추가
                    if(CDataOption.OnlRfid == eOnlRFID.Use)
                    {
                        /*
                        if (CSq_OnL.It.Cyl_PickRfid())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != eStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = eStatus.Idle;
                                m_LogVal.sMsg = "ONL_Pick Complet";
                                SaveLog();
                                //190410 ksg :
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader pick ok"));
                            }
                        }
                        */
                        //200203 ksg : CDataoption에서 Use라도 Cdata.opt에서 스킵 기능 추가
                        if(CData.Opt.bOnlRfidSkip)
                        {
                            if (CSq_OnL.It.Cyl_Pick())
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> ONL Pick finish.");
                                    //190410 ksg :
                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader pick ok"));
                                }
                            }
                        }
                        else
                        {
#if true //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드, SPIL)
                            bool bRet = false;

                            if (CDataOption.RFID == ERfidType.Keyence) //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용 //if (CData.CurCompany == ECompany.SPIL)
                            { bRet = CSq_OnL.It.Cyl_PickBcr(); } //SPIL 사는 매거진 바코드 읽기 (ASE-KH 2040X 추가)
                            else
                            { bRet = CSq_OnL.It.Cyl_PickRfid(); } //타 Company는 매거진 RFID 읽기

                            if (bRet)
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> ONL Pick bcr finish.");
                                    //190410 ksg :
                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader pick ok"));
                                }
                            }
#else
                            if (CSq_OnL.It.Cyl_PickRfid())
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    m_LogVal.sMsg = "ONL_Pick Complet";
                                    SaveLog();
                                    //190410 ksg :
                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader pick ok"));
                                }
                            }
#endif
                        }
                    }
                    else
                    {
                        if (CSq_OnL.It.Cyl_Pick())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> ONL Pick finish.");
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On loader pick ok"));
                            }
                        }
                    }
                    break;

                case ESeq.ONL_Place:
                    if (CSq_OnL.It.Cyl_Place())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONL Place finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On loader place ok");
                        }
                    }
                    break;

                case ESeq.ONL_Push:
                    if (CSq_OnL.It.Cyl_Push())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONL Push finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On Loader Push OK"));
                        }
                    }
                    break;

                //20200109 myk_ONL_Push_Pos
                case ESeq.ONL_Push_Pos:
                    if (CSq_OnL.It.Cyl_Push_Pos())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONL Push finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On Loader Push Position OK"));
                        }
                    }
                    break;

                // 2022.04.19 SungTae Start : [추가] ASE-KH Magazine ID Reading
                case ESeq.ONL_CheckBcr:
                    if(CSq_OnL.It.Cyl_CheckBcr())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;

                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONL MGZ ID Reading Finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("On-Loader MGZ ID Reading OK"));
                        }
                    }
                    break;
                // 2022.04.19 SungTae End
                #endregion

                #region In Rail Part
                case ESeq.INR_Home:
                    if (CSq_Inr.It.Cyl_Home())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> INR Home finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "In Rail Home OK");
                        }
                    }
                    break;

                case ESeq.INR_Wait:
                    if (CSq_Inr.It.Cyl_Wait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> INR Wait finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "In Rail Move Wait Position OK");
                        }
                    }
                    break;

                case ESeq.INR_Align:
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CSq_Inr.It.Cyl_Align())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> INR Align finish.");
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "In Rail Align OK");
                            }
                        }
                    }
                    else
                    {
                        if (CSq_Inr.It.Cyl_AlignU())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> INR Align finish.");
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "In Rail Align OK");
                            }
                        }
                    }
                    break;

                //20190604 ghk_onpbcr
                case ESeq.INR_CheckBcr:
                    if (CSq_Inr.It.Cyl_Bcr())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> INR Check BCR finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "In Rail Barcode Check OK");
                        }
                    }
                    break;
                //190211 ksg :
                case ESeq.INR_CheckOri:
                    if (CSq_Inr.It.Cyl_Ori())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> INR Check orientation finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "In Rail Orientation Check OK");
                        }
                    }
                    break;
                //190821 ksg :
                case ESeq.INR_CheckBcrOri:
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CSq_Inr.It.Cyl_Align())
                        {
                            Seq = ESeq.ONP_CheckBcrOri;
                            m_iSeq = ESeq.Idle;
                            _SetLog(">>>>> INR Check Bcr, Ori finish.");
                        }
                    }
                    else
                    {
                        if (CSq_Inr.It.Cyl_AlignU())
                        {
                            Seq = ESeq.ONP_CheckBcrOri;
                            m_iSeq = ESeq.Idle;
                            _SetLog(">>>>> INR Check BCR, Ori finish.");
                        }
                    }
                    break;

                case ESeq.INR_BcrReady:
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CSq_Inr.It.Cyl_Align())
                        {
                            Seq = ESeq.ONP_CheckBcr;
                            m_iSeq = ESeq.Idle;
                            _SetLog(">>>>> INR BCR Ready finish.");
                        }
                    }
                    else
                    {
                        if (CSq_Inr.It.Cyl_AlignU())
                        {
                            Seq = ESeq.ONP_CheckBcr;
                            m_iSeq = ESeq.Idle;
                            _SetLog(">>>>> INR BCR Ready finish.");
                        }
                    }
                    break;

                case ESeq.INR_OriReady:
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CSq_Inr.It.Cyl_Align())
                        {
                            Seq = ESeq.ONP_CheckOri;
                            m_iSeq = ESeq.Idle;
                            _SetLog(">>>>> INR Ori ready finish.");
                        }
                    }
                    else
                    {
                        if (CSq_Inr.It.Cyl_AlignU())
                        {
                            Seq = ESeq.ONP_CheckOri;
                            m_iSeq = ESeq.Idle;
                            _SetLog(">>>>> INR Ori ready finish.");
                        }
                    }
                    break;
                //190821 ksg :
                case ESeq.ONP_CheckBcrOri:
                    if (CSq_OnP.It.Cyl_OnpOriBcr())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Check Bcr, Ori finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On loader picker Barcode Check OK");
                        }
                    }
                    break;

                case ESeq.ONP_CheckBcr:
                    if (CSq_OnP.It.Cyl_OnpBcr())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Check Bcr finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On loader picker Barcode Check OK");
                        }
                    }
                    break;

                case ESeq.ONP_CheckOri:
                    if(CSq_OnP.It.Cyl_OnpOri())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Check Ori finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On loader picker Orientation Check OK");
                        }
                    }
                    break;

                case ESeq.INR_CheckDynamic:
#if true //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                    if (CSq_Inr.It.Cyl_DynamicMax5())
#else
                    if (CSq_Inr.It.Cyl_Dynamic())
#endif
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> INR DF finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "In Rail DynamicFunction Check OK");
                        }
                    }
                    break;
                #endregion

                #region On Loader Picker Part
                case ESeq.ONP_Wait:
                    if (CSq_OnP.It.Cyl_Wait())
                    {
                        Seq     = ESeq.Idle;
                        m_iSeq  = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Wait finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On loader picker wait ok");
                        }
                    }
                    break;

                case ESeq.ONP_Home:
                    if (CSq_OnP.It.Cyl_Home())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Home finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Home OK");
                        }
                    }
                    break;

                case ESeq.ONP_Pick:
                    if (CSq_OnP.It.Cyl_PickRail())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Pick finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Pick In Rail OK");
                        }
                    }
                    break;

                case ESeq.ONP_PickTbL:
                    if (CSq_OnP.It.Cyl_PickTable())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Pick left table finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Pick Left Table OK");
                        }
                    }
                    break;

                case ESeq.ONP_PlaceTbL:
                    // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
                    //if (CSq_OnP.It.Cyl_Palce(EWay.L))
                    if (CSq_OnP.It.Cyl_Place(EWay.L))
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Place left table finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Place Left Table OK");
                        }
                    }
                    break;

                case ESeq.ONP_PlaceTbR:
                    // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
                    //if (CSq_OnP.It.Cyl_Palce(EWay.R))
                    if (CSq_OnP.It.Cyl_Place(EWay.R))
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Place right table finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Place Right Table OK");
                        }
                    }
                    break;
                
                case ESeq.ONP_Conv:
                    if (CSq_OnP.It.Cyl_Conversion())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Conversion finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Conversion OK");
                        }
                    }
                    break;
                //20190611 ghk_onpclean
                case ESeq.ONP_Clean:
                    if (CSq_OnP.It.Cyl_PickerClean())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Clean finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Clean OK");
                        }
                    }
                    break;
                
                //20190822 pjh : OnpConvWait
                case ESeq.ONP_ConvWait:
                    if (CSq_OnP.It.Cyl_ConversionWait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP Conversion wait finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker Conversion Wait OK");
                        }
                    }
                    break;

                case ESeq.ONP_IV2:
                    if (CSq_OnP.It.Cyl_ONP_IV2())
                    {
                        Seq     = ESeq.Idle;
                        m_iSeq  = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> ONP IV2 finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "On Loader Picker IV2 Check OK");
                        }
                    }
                    break;
                #endregion

                #region Grind Dual Part
                case ESeq.GRD_Wait:
                    {
                        bool bL = CData.L_GRD.Cyl_Wait();
                        bool bR = CData.R_GRD.Cyl_Wait();

                        if (bL && bR)
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GRD Wait finish.");
                                bBtnShow = true;
                                if (CData.WhlChgSeq.iStep > 0)
                                {
                                    if (CData.WhlChgSeq.bSltLeft)
                                    {
                                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Wait[0]);
                                        CMot.It.Mv_N((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Able[1]);
                                    }
                                    else
                                    {
                                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Able[0]);
                                        CMot.It.Mv_N((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Wait[1]);
                                    }
                                    CData.WhlChgSeq.iStep = 4;
                                }
                                else
                                {
                                    if (!CData.WhlChgSeq.bStart)
                                    { CMsg.Show(eMsg.Notice, "Notice", "Grind Dual Move Wait Position OK"); }
                                }
                            }
                        }
                        break;
                    }

                case ESeq.GRD_Home:
                    {
                        bool bL = CData.L_GRD.Cyl_Home();
                        bool bR = CData.R_GRD.Cyl_Home();

                        if (bL && bR)
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GRD Home finish.");
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Dual Home OK");
                            }
                        }
                        break;
                    }

                case ESeq.GRD_DRESSER_REPLACE:
                    {
                        bool bL = CData.L_GRD.Cyl_DrsReplace();
                        bool bR = CData.R_GRD.Cyl_DrsReplace();

                        if (bL && bR)
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GRD Dresser replace position finish."); //200910 jhc : 버튼 타이틀 및 로그 문구 변경
                                bBtnShow = true;
                                CData.bDrsChange = true;
                            }
                        }
                        break;
                    }

                case ESeq.GRD_BufferTest:
                    if (CData.L_GRD.Cyl_BfTest())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GRD Buffer test finish.");
                            bBtnShow = true;
                        }
                    }
                    break;

                //20200731 josh    dual dressing
                case ESeq.GRD_Dual_Dressing:
                    {
                        // 2021-06-24, jhLee, Skyworks VOC, 어느한 쪽이 먼저 끝났을 경우 Timeout 발생시키지않고 다른 한쪽이 완료될때까지 대기하도록 수정
                        // 작업을 완료한 쪽은 더이상 Cyl_Drs()를 호출하지 않는다.

                        //bool bL = (CDataOption.IsBfSeq == true) ? CData.L_GRD.Cyl_DrsML() : CData.L_GRD.Cyl_Drs();
                        //bool bR = (CDataOption.IsBfSeq == true) ? CData.R_GRD.Cyl_DrsML() : CData.R_GRD.Cyl_Drs();

                        bool bL = false;
                        bool bR = false;

                        // 왼쪽 드레싱 동작 
                        if ( !bDDressFinish_L )
                        {
                            bL = (CDataOption.IsBfSeq == true) ? CData.L_GRD.Cyl_DrsML() : CData.L_GRD.Cyl_Drs();

                            // 동작 완료
                            if ( bL )
                            {
                                bDDressFinish_L = true;             // 왼쪽 드레싱 작업 종료
                                CData.L_GRD.ResetAdvancedGrindCondition(1, false); //Condition 검사 플래그 리셋
                                _SetLog("   >> (Dual-Dressing) Left GRD Dressing finish.");
                            }
                        }

                        // 오른쪽 드레싱 동작
                        if (!bDDressFinish_R)
                        {
                            bR = (CDataOption.IsBfSeq == true) ? CData.R_GRD.Cyl_DrsML() : CData.R_GRD.Cyl_Drs();

                            if (bR)
                            {
                                bDDressFinish_R = true;                             // 오른쪽 드레싱 작업 종료
                                CData.R_GRD.ResetAdvancedGrindCondition(1, false); //Condition 검사 플래그 리셋
                                _SetLog("   >> (Dual-Dressing) Right GRD Dressing finish.");
                            }
                        }

                        //old if (bL && bR)
                        if (bDDressFinish_L && bDDressFinish_R)         // 양쪽 모두 Dressing을 마쳤다.
                        {
                                ////201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                                //CData.L_GRD.ResetAdvancedGrindCondition(1, false); //Condition 검사 플래그 리셋
                                //CData.R_GRD.ResetAdvancedGrindCondition(1, false); //Condition 검사 플래그 리셋
                                ////..

                            if (CData.CurCompany == ECompany.ASE_K12) Seq = ESeq.GRD_Wait;//220513 pjh : ASE KH는 Dual Dressing 이후 대기 위치 이동
                            else Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;

                                CSQ_Man.It.bDDressFinish_L = false;        // 왼쪽 Table dressing 완료 여부
                                CSQ_Man.It.bDDressFinish_R = false;        // 오른쪽 Table dressing 완료 여부

                                _SetLog(">>>>> (Dual-Dressing) GRD Dressing finish.");
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dual dressing finish.");
                            }
                        }

                        break;
                    }
                //
                //220321 pjh : Manual Dual Grinding 추가
                case ESeq.GRD_Dual_Grinding :
                    {

                        bool bL = false;
                        bool bR = false;

                        // 왼쪽 Grinding 동작 
                        if (!bDGrindFinish_L)
                        {
                            bL = (CDataOption.IsBfSeq == true) ? CData.L_GRD.Cyl_GrdML() : CData.L_GRD.Cyl_Grd();

                            // 동작 완료
                            if (bL)
                            {
                                bDGrindFinish_L = true;             // 왼쪽 Grinding 작업 종료
                                CData.L_GRD.ResetAdvancedGrindCondition(0, false); //Condition 검사 플래그 리셋
                                _SetLog(">> (Dual-Grinding) Left GRD Grinding finish.");
                            }
                        }

                        // 오른쪽 Grinding 동작
                        if (!bDGrindFinish_R)
                        {
                            bR = (CDataOption.IsBfSeq == true) ? CData.R_GRD.Cyl_GrdML() : CData.R_GRD.Cyl_Grd();

                            if (bR)
                            {
                                bDGrindFinish_R = true;                             // 오른쪽 Grinding 작업 종료
                                CData.R_GRD.ResetAdvancedGrindCondition(0, false); //Condition 검사 플래그 리셋
                                _SetLog(">> (Dual-Grinding) Right GRD Grinding finish.");
                            }
                        }

                        if (bDGrindFinish_L && bDGrindFinish_R)         // 양쪽 모두 Grinding을 마쳤다.
                        {
                            Seq = ESeq.GRD_Wait;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;

                                bDGrindFinish_L = false;        // 왼쪽 Table Grinding 완료 여부
                                bDGrindFinish_R = false;        // 오른쪽 Table Grinding 완료 여부

                                _SetLog(">>>>> (Dual-Grinding) GRD Grinding finish.");
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dual Grinding finish.");
                            }
                        }

                        break;
                    }            
                    //

                #endregion

                #region Grind Left Part
                case ESeq.GRL_Home:
                    if (CData.L_GRD.Cyl_Home() == true)
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Home finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Home OK");
                        }
                    }
                    break;

                case ESeq.GRL_Wait:
                    if (CData.L_GRD.Cyl_Wait() == true)
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Wait finish.");
                            bBtnShow = true;
                            //190622 ksg 
                            if(CData.DrsChgSeq.iStep == 5 && !CData.DrsChgSeq.bDrsMeanDS) 
                            {
                                m_iSeq = ESeq.Idle;
                                Seq    = ESeq.GRR_Dresser_Measure;
                            }
                            if(!CData.DrsChgSeq.bStart && !CData.WhlChgSeq.bStart)CMsg.Show(eMsg.Notice, "Notice", "Grind Left Move Wait Position OK");
                        }
                    }
                    break;

                case ESeq.GRL_Wheel_Measure:
                    if (CDataOption.MeasureType == eMeasureType.Jog)
                    {//조그 무브 휠 측정 방식
                        if (CData.L_GRD.Cyl_MeaWhl() == true)
                        {
                            Seq = ESeq.GRL_Wait;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                //휠 히스토리 저장
                                CData.WhlsLog[0].dWhltL = 0;
                                CData.WhlsLog[0].dDrsL = 0;
                                CWhl.It.SaveHistory(EWay.L);
                                //
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Measure wheel finish.");
                                bBtnShow = true;
                                //190620 ksg :
                                CData.WhlChgSeq.bWhlMeanS = true;
                                if (CData.WhlChgSeq.iStep == 0) CMsg.Show(eMsg.Notice, "Notice", "Grind Left Wheel Hight Measure OK");
                            }
                            else //190620 ksg :
                            {
                                CData.WhlChgSeq.bWhlMeanS = false;
                            }
                        }
                    }
                    else
                    {//스텝 무브 휠 측정 방식
                        if (CData.L_GRD.Cyl_MeaWhl_Step() == true)
                        {
                            Seq = ESeq.GRL_Wait;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                //휠 히스토리 저장
                                CData.WhlsLog[0].dWhltL = 0;
                                CData.WhlsLog[0].dDrsL = 0;
                                CWhl.It.SaveHistory(EWay.L);
                                //
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Measure wheel finish.");
                                bBtnShow = true;
                                //190620 ksg :
                                CData.WhlChgSeq.bWhlMeanS = true;
                                if (CData.WhlChgSeq.iStep == 0) CMsg.Show(eMsg.Notice, "Notice", "Grind Left Wheel Hight Measure OK");
                            }
                            else //190620 ksg :
                            {
                                CData.WhlChgSeq.bWhlMeanS = false;
                            }
                        }
                    }
                    break;

                case ESeq.GRL_Dresser_Measure:
                    if (CData.L_GRD.Cyl_MeaDrs() == true)
                    {
                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            //211230 pjh : Dresser 히스토리 저장
                            if (CDataOption.UseSeperateDresser)
                            {
                                CWhl.It.SaveDrsHistory(EWay.L);
                            }
                            //
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Measure dresser finish.");
                            bBtnShow = true;
                            CData.DrsChgSeq.bDrsMeanS = true;
                            if(!CData.DrsChgSeq.bStart)CMsg.Show(eMsg.Notice, "Notice", "Grind Left Dresser Thickness Measure OK");
                        }
                        else //190622 ksg :
                        {
                            CData.DrsChgSeq.bDrsMeanS = false;
                        }
                    }
                    break;

                case ESeq.GRL_Table_Measure:
                    // 2021.09.23 SungTae Start : [수정]
                    if (CDataOption.TblSetPos == eTblSetPos.Use) { bUse = true; }
                    else { bUse = false; }

                    if (CData.L_GRD.Cyl_MeaTbl((int)EMeaStep.Before, bUse) == true)   //if (CData.L_GRD.Cyl_MeaTbl() == true)
                    // 2021.09.23 SungTae End
                    {
                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;

                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;

                            _SetLog(">>>>> GDL Measure table finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Table Thickness Measure OK");
                        }
                    }
                    break;
                //190628 ksg :
                case ESeq.GRL_Table_MeasureSix:
                    if (CData.L_GRD.Cyl_MeaTblSixPoint() == true)
                    {
                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Measure table six finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Table Six Point Thickness Measure OK");
                        }
                    }
                    break;

                //200928 lhs :
                case ESeq.GRL_Table_Measure_8p:
                    //if (CData.L_GRD.Cyl_MeaTbl_8p() == true)  // 2020.11.27 lhs (측정값 그대로 표시)
                    if (CData.L_GRD.Cyl_MeaTbl_TM8() == true)   // 2020.11.27 lhs (보정값 적용 : SCK+ 요구사항)
                    {
                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Measure table 8 point finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Table 8 Point Thickness Measure OK");
                        }
                    }
                    break;

                case ESeq.GRL_Dressing:
                    if (((CDataOption.IsBfSeq == true) ? CData.L_GRD.Cyl_DrsML() : CData.L_GRD.Cyl_Drs()) == true)
                    {
                        //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                        CData.L_GRD.ResetAdvancedGrindCondition(1, false); //Condition 검사 플래그 리셋
                        //..

                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Dressing finish.");
                            bBtnShow = true;
                            //190620 ksg :
                            CData.WhlChgSeq.bWhlDressS = true;
                            if(CData.WhlChgSeq.iStep == 0) CMsg.Show(eMsg.Notice, "Notice", "Grind Left Dressing OK");
                            //CMsg.Show(eMsg.Notice, "Notice", "Grind Left Dressing OK");
                        }
                    }
                    break;

                case ESeq.GRL_Grinding:
                    if(CData.DrData[0].bDrs)
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        CMsg.Show(eMsg.Notice, "Notice", "Need Left Dressing");
                        CSQ_Main.It.m_iStat = EStatus.Idle;
                        bBtnShow = true;
                        break;
                    }
                    //201025 jhc : ASE-Kr (DF Bottom) 설정 상태(Mold 두께 Recipe, 낮은 값)에서 Manual Grinding 불가 : 자재 파손 위험!!!
                    if ((CData.CurCompany == ECompany.ASE_KR) && (CData.Dev.eMoldSide == ESide.Btm))
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        CMsg.Show(eMsg.Error, "Error", "Danger! Manual Grinding should not be done with the Bottom Side Recipe.");
                        CSQ_Main.It.m_iStat = EStatus.Idle;
                        bBtnShow = true;
                        break;
                    }

                    // 2021.08.04 lhs Start : Manual Grinding 예외 처리 (SCK전용)
                    if(CDataOption.UseNewSckGrindProc)
                    {
                        if(CData.Dev.aData[0].eBaseOnThick == EBaseOnThick.Mold)
                        {
                            Seq     = ESeq.Idle;
                            m_iSeq  = ESeq.Idle;
                            CMsg.Show(eMsg.Error, "Error", "Danger! Manual Grinding is not available when set to Base on thickness mode.");
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            bBtnShow = true;
                            break;
                        }
					}
                    // 2021.08.04 lhs End

                    if (((CDataOption.IsBfSeq == true) ? CData.L_GRD.Cyl_GrdML() : CData.L_GRD.Cyl_Grd()) == true)
                    {
                        //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                        CData.L_GRD.ResetAdvancedGrindCondition(0, false); //Condition 검사 플래그 리셋
                        //..

                        CData.L_GRD.m_bManual18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 18 Point 측정 플래그 초기화

                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Grinding finish.");
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Manual Grinding OK");
                        }

                        // 2020.09.21 JSKim St
                        if (CData.CurCompany == ECompany.JCET)
                        {
                            if (CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, false);  // JCET
                        }
                        // 2020.09.21 JSKim Ed
                    }
                    break;

                //20191010 ghk_manual_bcr
                case ESeq.GRL_Manual_GrdBcr:
                    if(CData.Opt.bManBcrSkip)
                    {//Manual 바코드 사용 안함
                        Seq = ESeq.GRL_Grinding;
                        m_iSeq = ESeq.Idle;

                        CSQ_Main.It.m_iStat = EStatus.Idle;
                    }
                    else
                    {//바코드 사용
                        if(CSq_OnP.It.Cyl_OnpManOriBcrLeft())
                        {
                            if(CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                Seq = ESeq.GRL_Grinding;
                                m_iSeq = ESeq.Idle;
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL BCR finish.");
                                bBtnShow = true;
                            }
                        }
                    }
                    break;

                case ESeq.GRL_Manual_Bcr:
                    if (CSq_OnP.It.Cyl_OnpManOriBcrLeft())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL BCR finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Manual Bcr OK");
                        }
                    }
                    break;

                case ESeq.GRL_Strip_Measure:
                    // 200317 mjy : 조건 추가
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CData.L_GRD.Cyl_MeaStrip((int)EMeaStep.Before))    // Before
                        {
                            CData.L_GRD.m_bManual18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 18 Point 측정 플래그 초기화

                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Measure strip finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Left Strip Thickness Measure OK");
                            }
                        }
                    }
                    else
                    {
                        // 2020-10-27, jhLee : Motorizing probe measure
                        //d if (CData.L_GRD.Cyl_MeaUnit(0))
                        if ((CData.Opt.bPbType) ? CData.L_GRD.Cyl_MeaUnit_ZMotor(0) : CData.L_GRD.Cyl_MeaUnit(0))
                        {
                            CData.L_GRD.m_bManual18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 18 Point 측정 플래그 초기화

                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Measure unit finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Left Strip Thickness Measure OK");
                            }
                        }
                    }

                    break;

                case ESeq.GRL_Strip_Measureone:
                    if(CData.Opt.bPbType)
                    {
                        if (CData.L_GRD.Cyl_MeaStripOne_ZMotor())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Measure one point finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Left Strip Thickness Measure One OK");
                            }
                        }
                    }
                    else
                    {
                        if (CData.L_GRD.Cyl_MeaStripOne())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Measure one point finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Left Strip Thickness Measure One OK");
                            }
                        }
                    }
                    break;
                
                case ESeq.GRL_Water_Knife:
                    if (CData.L_GRD.Cyl_WKnife())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Water knife finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Water Knife OK");
                        }
                    }
                    break;

                case ESeq.GRL_ProbeTest:
                    if (CData.L_GRD.Cyl_ProbeTest() == true)
                    {
                        Seq = ESeq.GRL_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Porbe test finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Probe Test Measure OK");
                        }
                    }
                    break;

                //20190819 ghk_tableclean
                case ESeq.GRL_Table_Clean:
                    if (CData.L_GRD.Cyl_TblClean())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDL Table clean finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Table Clean OK");
                        }
                    }
                    break;

                #region 테이블 그라인딩
                case ESeq.GRL_Table_Grinding:
                    m_bTblOk = false;
                    Seq = ESeq.GRL_Table_Grinding_Wheel_Measure;
                    _SetLog(">>>>> GDL Table grinding finish.");

                    m_iSeq = ESeq.Idle;
                    break;

                case ESeq.GRL_Table_Grinding_Wheel_Measure:
                    if (CDataOption.MeasureType == eMeasureType.Jog)
                    {//조그 무브 휠 측정 방식
                        if (CData.L_GRD.Cyl_MeaWhl())
                        {
                            Seq = ESeq.GRL_Table_Grinding_Table_Measure;
                            _SetLog(">>>>> GDL Table grinding measure wheel finish.");

                            m_iSeq = ESeq.Idle;
                        }
                    }
                    else
                    {//스텝 무브 휠 측정 방식
                        if (CData.L_GRD.Cyl_MeaWhl_Step())
                        {
                            Seq = ESeq.GRL_Table_Grinding_Table_Measure;
                            _SetLog(">>>>> GDL Table grinding measure wheel finish.");

                            m_iSeq = ESeq.Idle;
                        }
                    }
                    break;

                case ESeq.GRL_Table_Grinding_Table_Measure:
                    // 2021.09.23 SungTae Start : [수정]
                    if (CDataOption.TblSetPos == eTblSetPos.Use)    { bUse = true; }
                    else                                            { bUse = false; }

                    nIdx = (m_bTblOk == false) ? (int)EMeaStep.Before : (int)EMeaStep.After;

                    if (CData.L_GRD.Cyl_MeaTbl(nIdx, bUse))   //if (CData.L_GRD.Cyl_MeaTbl())
                    // 2021.09.23 SungTae End
                    {
                        if (m_bTblOk)
                        {
                            Seq = ESeq.GRL_Wait;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDL Table grinding measure table finish.");

                                CMsg.Show(eMsg.Notice, "Notice", "Grind Left Table Grinding OK");
                            }
                        }
                        else
                        { 
                            Seq = ESeq.GRL_Table_Grinding_Work;
                            _SetLog(">>>>> GDL Table grinding measure table finish.");
                        }
                        m_iSeq = ESeq.Idle;
                    }
                    break;

                case ESeq.GRL_Table_Grinding_Work:
                    if (CData.L_GRD.Cyl_GrdTbl())
                    {
                        m_bTblOk = true;
                        Seq = ESeq.GRL_Table_Grinding_Wheel_Measure;
                        _SetLog(">>>>> GDL Table grinding finish.");

                        m_iSeq = ESeq.Idle;
                    }
                    break;

                case ESeq.GRL_Probe_Calibration_Check:
                    if (CData.L_GRD.Cyl_ProbeCalCheck())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;

                            _SetLog(">>>>> GRL_Probe_Calibration_Check Complete");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Probe Calibration Check OK");
                        }
                    }
                    break;

                //200818 myk : Probe Auto Calibration
                case ESeq.GRL_Probe_Auto_Calibration:
                    if (CData.L_GRD.Cyl_AutoCalibration())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GRL_Probe_Auto_Calibration Complete");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Left Probe Auto Calibration OK");
                        }
                    }
                    break;
                #endregion
                #endregion

                #region Grind Right Part
                case ESeq.GRR_Home:
                    if (CData.R_GRD.Cyl_Home() == true)
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Home finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Home OK");
                        }
                    }
                    break;

                case ESeq.GRR_Wait:
                    if (CData.R_GRD.Cyl_Wait() == true)
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Wait finish.");

                            bBtnShow = true;
                            if(!CData.WhlChgSeq.bStart && !CData.DrsChgSeq.bStart)CMsg.Show(eMsg.Notice, "Notice", "Grind Right Move Wait Position OK");
                        }
                    }
                    break;

                case ESeq.GRR_Wheel_Measure:
                    //CData.AutoWheelMeaR = false;
                    //20191105 ghk_measuretype
                    if (CDataOption.MeasureType == eMeasureType.Jog)
                    {//조그 무브 휠 측정 방식
                        if (CData.R_GRD.Cyl_MeaWhl() == true)
                        {
                            Seq = ESeq.GRR_Wait;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                //휠 히스토리 저장
                                CData.WhlsLog[1].dWhltL = 0;
                                CData.WhlsLog[1].dDrsL = 0;
                                CWhl.It.SaveHistory(EWay.R);
                                //
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR Measure wheel finish.");

                                bBtnShow = true;
                                //190620 ksg :
                                CData.WhlChgSeq.bWhlMeanS = true;
                                if (CData.WhlChgSeq.iStep == 0)
                                { CMsg.Show(eMsg.Notice, "Notice", "Grind Right Wheel Height Measure OK"); }
                            }
                            else //190620 ksg :
                            {
                                CData.WhlChgSeq.bWhlMeanS = false;
                            }
                        }
                    }
                    else
                    {//스텝 무브 휠 측정 방식
                        if (CData.R_GRD.Cyl_MeaWhl_Step() == true)
                        {
                            Seq = ESeq.GRR_Wait;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                //휠 히스토리 저장
                                CData.WhlsLog[1].dWhltL = 0;
                                CData.WhlsLog[1].dDrsL = 0;
                                CWhl.It.SaveHistory(EWay.R);
                                //
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR Measure wheel finish.");

                                bBtnShow = true;
                                //190620 ksg :
                                CData.WhlChgSeq.bWhlMeanS = true;
                                if (CData.WhlChgSeq.iStep == 0)
                                { CMsg.Show(eMsg.Notice, "Notice", "Grind Right Wheel Height Measure OK"); }
                            }
                            else //190620 ksg :
                            {
                                CData.WhlChgSeq.bWhlMeanS = false;
                            }
                        }
                    }
                    break;

                case ESeq.GRR_Dresser_Measure:
                    if (CData.R_GRD.Cyl_MeaDrs() == true)
                    {
                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            //211230 pjh : Dresser 히스토리 저장
                            if (CDataOption.UseSeperateDresser)
                            {
                                CWhl.It.SaveDrsHistory(EWay.R);
                            }
                            //
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Measure dresser finish.");

                            bBtnShow = true;
                            //190622 ksg :
                            CData.DrsChgSeq.bDrsMeanS = true; 
                            if(CData.DrsChgSeq.iStep == 5 && !CData.DrsChgSeq.bDrsMeanDS) CData.DrsChgSeq.bDrsMeanDS = true;
                            if(!CData.DrsChgSeq.bStart) CMsg.Show(eMsg.Notice, "Notice", "Grind Right Dresser Thickness Measure OK");
                        }
                        else //190622 ksg :
                        {
                            CData.DrsChgSeq.bDrsMeanS = false;
                        }
                    }
                    break;

                case ESeq.GRR_Table_Measure:
                    // 2021.09.23 SungTae Start : [수정]
                    if (CDataOption.TblSetPos == eTblSetPos.Use)    { bUse = true; }
                    else                                            { bUse = false; }

                    if (CData.R_GRD.Cyl_MeaTbl((int)EMeaStep.Before, bUse) == true)   //if (CData.R_GRD.Cyl_MeaTbl() == true)
                    // 2021.09.23 SungTae End
                    {
                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;

                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;

                            _SetLog(">>>>> GDR Measure table finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Table Thickness Measure OK");
                        }
                    }
                    break;
                
                case ESeq.GRR_Table_MeasureSix:
                    if (CData.R_GRD.Cyl_MeaTblSixPoint() == true)
                    {
                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Measure table six finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Table Six Point Thickness Measure OK");
                        }
                    }
                    break;

                //200928 lhs : 
                case ESeq.GRR_Table_Measure_8p:
                    //if (CData.R_GRD.Cyl_MeaTbl_8p() == true)  // 2020.11.27 lhs (측정값 그대로 표시)
                    if (CData.R_GRD.Cyl_MeaTbl_TM8() == true)   // 2020.11.27 lhs (보정값 적용 : SCK+ 요구사항)
                    {
                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Measure table 8 point finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Table 8 Point Thickness Measure OK");
                        }
                    }
                    break;

                case ESeq.GRR_Dressing:
                    if (((CDataOption.IsBfSeq == true) ? CData.R_GRD.Cyl_DrsML() : CData.R_GRD.Cyl_Drs()) == true)
                    {
                        //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                        CData.R_GRD.ResetAdvancedGrindCondition(1, false); //Condition 검사 플래그 리셋
                        //..

                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Dressing finish.");

                            bBtnShow = true;
                            //190620 ksg :
                            CData.WhlChgSeq.bWhlDressS = true;
                            if (CData.WhlChgSeq.iStep == 0)
                            { CMsg.Show(eMsg.Notice, "Notice", "Grind Right Dressing OK"); }
                        }
                    }
                    break;

                case ESeq.GRR_Grinding:
                    if (CData.DrData[1].bDrs)
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        CMsg.Show(eMsg.Notice, "Notice", "Need Right Dressing");
                        CSQ_Main.It.m_iStat = EStatus.Idle;
                        bBtnShow = true;
                        break;
                    }
                    //201025 jhc : ASE-Kr (DF Bottom) 설정 상태(Mold 두께 Recipe, 낮은 값)에서 Manual Grinding 불가 : 자재 파손 위험!!!
                    if ((CData.CurCompany == ECompany.ASE_KR) && (CData.Dev.eMoldSide == ESide.Btm))
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        CMsg.Show(eMsg.Error, "Error", "Danger! Manual Grinding should not be done with the Bottom Side Recipe.");
                        CSQ_Main.It.m_iStat = EStatus.Idle;
                        bBtnShow = true;
                        break;
                    }
                    //

                    // 2021.08.04 lhs Start : Manual Grinding 예외 처리 (SCK전용)
                    if (CDataOption.UseNewSckGrindProc)
                    {
                        if (CData.Dev.aData[1].eBaseOnThick == EBaseOnThick.Mold)
                        {
                            Seq     = ESeq.Idle;
                            m_iSeq  = ESeq.Idle;
                            CMsg.Show(eMsg.Error, "Error", "Danger! Manual Grinding is not available when set to Base on thickness mode.");
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            bBtnShow = true;
                            break;
                        }
                    }
                    // 2021.08.04 lhs End

                    if (((CDataOption.IsBfSeq == true) ? CData.R_GRD.Cyl_GrdML() : CData.R_GRD.Cyl_Grd()) == true)
                    {
                        //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                        CData.R_GRD.ResetAdvancedGrindCondition(0, false); //Condition 검사 플래그 리셋
                        //..

                        CData.R_GRD.m_bManual18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 18 Point 측정 플래그 초기화

                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Grinding finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Manual Grinding OK");
                        }

                        // 2020.09.21 JSKim St
                        if (CData.CurCompany == ECompany.JCET)
                        {
                            if (CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, false);  // JCET
                        }
                        // 2020.09.21 JSKim Ed
                    }
                    break;

                //20191010 ghk_manual_bcr
                case ESeq.GRR_Manual_GrdBcr:
                    if (CData.Opt.bManBcrSkip)
                    {//Manual 바코드 사용 안함
                        Seq = ESeq.GRR_Grinding;
                        m_iSeq = ESeq.Idle;

                        CSQ_Main.It.m_iStat = EStatus.Idle;
                    }
                    else
                    {//바코드 사용
                        if (CSq_OnP.It.Cyl_OnpManOriBcrRight())
                        {
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                Seq = ESeq.GRR_Grinding;
                                m_iSeq = ESeq.Idle;
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR BCR finish.");
                                bBtnShow = true;
                            }
                        }
                    }
                    break;

                case ESeq.GRR_Manual_Bcr:
                    if(CSq_OnP.It.Cyl_OnpManOriBcrRight())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR BCR finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Manual Bcr OK");
                        }
                    }
                    break;
                //

                case ESeq.GRR_Strip_Measure:

                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CData.R_GRD.Cyl_MeaStrip((int)EMeaStep.Before))    // Before
                        {
                            CData.R_GRD.m_bManual18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 18 Point 측정 플래그 초기화

                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR Measure strip finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Right Strip Thickness Measure OK");
                            }
                        }
                    }
                    else
                    {
                        // 2020-10-27, jhLee : Motorizing probe measure
                        //d if (CData.R_GRD.Cyl_MeaUnit(0))
                        if ((CData.Opt.bPbType) ? CData.R_GRD.Cyl_MeaUnit_ZMotor(0) : CData.R_GRD.Cyl_MeaUnit(0))
                        {
                            CData.R_GRD.m_bManual18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 18 Point 측정 플래그 초기화

                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR Measure unit finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Right Strip Thickness Measure OK");
                            }
                        }
                    }

                    break;

                case ESeq.GRR_Strip_Measureone:
                    if(CData.Opt.bPbType)
                    {
                        if (CData.R_GRD.Cyl_MeaStripOne_ZMotor())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR Measure one point finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Right Strip Thickness Measure One OK");
                            }
                        }
                    }
                    else
                    {
                        if (CData.R_GRD.Cyl_MeaStripOne())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> GDR Measure one point finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Grind Right Strip Thickness Measure One OK");
                            }
                        }
                    }
                    break;
                
                case ESeq.GRR_Water_Knife:
                    if (CData.R_GRD.Cyl_WKnife())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Water knife finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Water Knife OK");
                        }
                    }
                    break;

                case ESeq.GRR_ProbeTest:
                    if (CData.R_GRD.Cyl_ProbeTest() == true)
                    {
                        Seq = ESeq.GRR_Wait;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Probe test finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Probe Test Measure OK");
                        }
                    }
                    break;

                //20190819 ghk_tableclean
                case ESeq.GRR_Table_Clean:
                    if (CData.R_GRD.Cyl_TblClean())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GDR Table clean finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Table Clean OK");
                        }
                    }
                    break;

                //200812 myk : Probe Calibration Check
                case ESeq.GRR_Probe_Calibration_Check:
                    if (CData.R_GRD.Cyl_ProbeCalCheck())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GRR_Probe_Calibration_Check Complete");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Probe Calibration Check OK");
                        }
                    }
                    break;

                //200818 myk : Probe Auto Calibration
                case ESeq.GRR_Probe_Auto_Calibration:
                    if (CData.R_GRD.Cyl_AutoCalibration())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> GRR_Probe_Auto_Calibration Complete");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Grind Right Probe Auto Calibration OK");
                        }
                    }
                    break;
                //

                #region 테이블 그라인딩
                case ESeq.GRR_Table_Grinding:
                    m_bTblOk = false;
                    Seq = ESeq.GRR_Table_Grinding_Wheel_Measure;
                    _SetLog("<<<<< GDR Measure wheel start.");
                    m_iSeq = ESeq.Idle;
                    break;

                case ESeq.GRR_Table_Grinding_Wheel_Measure:
                    if (CDataOption.MeasureType == eMeasureType.Jog)
                    {//조그 무브 휠 측정 방식
                        if (CData.R_GRD.Cyl_MeaWhl())
                        {
                            Seq = ESeq.GRR_Table_Grinding_Table_Measure;
                            _SetLog(">>>>> GDR Table grinding measure wheel finish.");
                            m_iSeq = ESeq.Idle;
                        }
                    }
                    else
                    {//스텝 무브 휠 측정 방식
                        if (CData.R_GRD.Cyl_MeaWhl_Step())
                        {
                            Seq = ESeq.GRR_Table_Grinding_Table_Measure;
                            _SetLog(">>>>> GDR Table grinding measure wheel finish.");
                            m_iSeq = ESeq.Idle;
                        }
                    }
                    break;

                case ESeq.GRR_Table_Grinding_Table_Measure:
                    // 2021.09.23 SungTae Start : [수정]
                    if (CDataOption.TblSetPos == eTblSetPos.Use)    { bUse = true; }
                    else                                            { bUse = false; }

                    nIdx = (m_bTblOk == false) ? (int)EMeaStep.Before : (int)EMeaStep.After;

                    if (CData.R_GRD.Cyl_MeaTbl(nIdx, bUse) == true)   //if (CData.R_GRD.Cyl_MeaTbl() == true)
                    // 2021.09.23 SungTae End
                    {
                        if (m_bTblOk)
                        {
                            Seq = ESeq.GRR_Wait;

                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;

                                _SetLog(">>>>> GDR Table grinding finish.");

                                CMsg.Show(eMsg.Notice, "Notice", "Grind Right Table Grinding OK");
                            }
                        }
                        else
                        { 
                            Seq = ESeq.GRR_Table_Grinding_Work;
                        }

                        m_iSeq = ESeq.Idle;
                    }
                    break;

                case ESeq.GRR_Table_Grinding_Work:
                    if (CData.R_GRD.Cyl_GrdTbl())
                    {
                        m_bTblOk = true;
                        Seq = ESeq.GRR_Table_Grinding_Wheel_Measure;
                        m_iSeq = ESeq.Idle;
                        _SetLog(">>>>> GDR Table grinding finish.");
                    }
                    break;
                #endregion
                #endregion

                #region Off Loader Picker Part
                case ESeq.OFP_Home:
                    if (CSq_OfP.It.Cyl_Home())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Home finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Home OK");
                        }
                    }
                    break;

                case ESeq.OFP_Wait:
                    if (CSq_OfP.It.Cyl_Wait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Wait finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Move Wait Position OK");
                        }
                    }
                    break;

                case ESeq.OFP_PickTbL:
                    if (CSq_OfP.It.Cyl_Pick(EWay.L))
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Pick left table finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Left Table Pick OK");
                        }
                    }
                    break;

                case ESeq.OFP_PickTbR:
                    if (CSq_OfP.It.Cyl_Pick(EWay.R))
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Pick right table finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Right Table Pick OK");
                        }
                    }
                    break;

#if true //20200415 jhc : Picker Idle Time 개선
                case ESeq.OFP_PlaceTbR:
                    if (CSq_OfP.It.Cyl_PlaceTbR())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Place right table finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Place Right Table OK");
                        }
                    }
                    break;
#endif

				case ESeq.OFP_Place:
					if (CSq_OfP.It.Cyl_Place()) // Dryer에 Place
					{
						Seq = ESeq.Idle;
						m_iSeq = ESeq.Idle;
						if (CSQ_Main.It.m_iStat != EStatus.Error)
						{
							CSQ_Main.It.m_iStat = EStatus.Idle;
							_SetLog(">>>>> OFP Place finish.");

							bBtnShow = true;
							CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Dry Place OK");
						}
					}
					break;

                case ESeq.OFP_IV_DryTable:  // 2022.07.07 lhs : OffPicker에 달린 IV를 이용하여 Dryer에 자재가 있는지 체크
                    {
                        if (CDataOption.UseOffP_IOIV)
                        {
                            if (CSq_OfP.It.Cyl_IV_DryTable())   // Dryer Table을 IV로 촬영하여 Strip 존재여부 체크
                            {
                                Seq     = ESeq.Idle;
                                m_iSeq  = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> Off-Picker IV... Check Dryer Strip Exist... finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Off-Picker IV... Check Dryer Strip Exist... OK");
                                }
                            }
                        }
                        break;
                    }

                // 2021.04.12 lhs Start
                case ESeq.OFP_IV_DryClamp:  
                    if (CDataOption.UseDryWtNozzle ||  // Dryer에 Water Nozzle을 사용할 경우
                        CDataOption.UseOffP_IOIV)      // 2022.06.30 lhs : OffPicker IO로 동작하는 IV 추가
                    {
                        if (CSq_OfP.It.Cyl_IV_DryClamp())   // Dryer Tip을 IV2로 촬영 및 판정
                        {
                            Seq     = ESeq.Idle;
                            m_iSeq  = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> Off-Picker IV... Check Dryer Clamp... finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off-Picker IV... Check Dryer Clamp... OK");
                            }
                        }
                    }
                    else  // 여기 들어오면 잘못된건데...
                    {
                        Seq     = ESeq.Idle;
                        m_iSeq  = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OffPicker OFP_IV_DryClamp can be used when UseDryWtNozzle is true.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker :  OFP_IV_DryClamp can be used when UseDryWtNozzle is true.");
                        }
                    }
                    break;
                // 2021.04.12 lhs End

                // 2021.04.15 lhs Start
                case ESeq.OFP_CoverDryer:
                    if (CDataOption.UseDryWtNozzle)  // Dryer에 Water Nozzle을 사용할 경우
                    {
                        if (CSq_OfP.It.Cyl_CoverDryer())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> OffPicker Cover Dryer finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Cover Dryer OK");
                            }
                        }
                    }
                    else  // 여기 들어오면 잘못된건데...
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP_CoverDryer can be used when UseDryWtNozzle is true.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker :  OFP_CoverDryer can be used when UseDryWtNozzle is true.");
                        }
                    }
                    break;

                case ESeq.OFP_CoverDryerEnd:
                    if (CDataOption.UseDryWtNozzle)  // Dryer에 Water Nozzle을 사용할 경우
                    {
                        if (CSq_OfP.It.Cyl_CoverDryerEnd())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> OffPicker CoverDryerEnd() finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker CoverDryerEnd() OK");
                            }
                        }
                    }
                    else  // 여기 들어오면 잘못된건데...
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP_CoverDryerEnd can be used when UseDryWtNozzle is true.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker :  OFP_CoverDryerEnd can be used when UseDryWtNozzle is true.");
                        }
                    }
                    break;
                // 2021.04.15 lhs End

                case ESeq.OFP_BtmClean:
                    if (CDataOption.UseSprayBtmCleaner) // 2022.03.10 lhs 
                    {
                        if (CSq_OfP.It.Cyl_PckClean_SprayNzl())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> OFP Bottom clean finish (Spray Nozzle).");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Picker Cleaning OK (Spray Nozzle)");
                            }
                        }
                    }
                    else
                    {
                        //20190926 ghk_ofppadclean
                        if (CDataOption.PadClean == eOffPadCleanType.LRType)
                        {
                            if (CSq_OfP.It.Cyl_PckClean())
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> OFP Bottom clean finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Picker Cleaning OK");
                                }
                            }
                        }
                        else
                        {
                            if (CSq_OfP.It.Cyl_PckPadClean())
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> OFP Pad clean finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Picker Cleaning OK");
                                }
                            }
                        }
                    }
                    break;

                //210908 syc : 2004U
                case ESeq.OFP_IV2:
                   if (CSq_OfP.It.Cyl_OFP_IV2())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Check IV2 finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "OFP Check IV2 OK");
                        }
                    }
                    break;
                //210915 syc : 2004U
                case ESeq.OFP_CoverIV2:
                    if (CSq_OfP.It.Cyl_OFP_CoverCheck())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Cover Check finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "OFP Cover Check OK");
                        }
                    }
                    break;

                //210915 syc : 2004U
                case ESeq.OFP_CoverPick:

                    if (CData.bOFFP_CoverPickMSGCheck)
                    {
                        CData.bOFFP_CoverPickMSGCheck = false;
                        if (CMsg.Show(eMsg.Query, "Notie", "Is the next sequence of Dry part Dry Out? \n Yes : Dry next sequence -> Dry Out \n No : Dry next sequence -> Dry Wait place") == DialogResult.Cancel)
                        {
                            CData.bOFFP_NextSeqDryout = false;
                            _SetLog("NO Selected : Dry next sequence -> Dry Wait place");   // 2022.08.18 lhs
                        }
                        _SetLog("YES Selected : Dry next sequence -> Dry Out");             // 2022.08.18 lhs
                    }

                    if (CSq_OfP.It.Cyl_CoverPick())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Cover Pick finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "OFP Cover Pick OK");

                            CData.bOFFP_CoverPickMSGCheck = true;
                        }
                    }
                    break;

                //210915 syc : 2004U
                case ESeq.OFP_CoverPlace:
                    if (CSq_OfP.It.Cyl_CoverPlace())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Cover Place finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "OFP Cover Place OK");
                        }
                    }
                    break;

                case ESeq.OFP_BtmCleanStrip:
                    if (CDataOption.UseSprayBtmCleaner) // 2022.03.10 lhs 
                    {
                        if (CSq_OfP.It.Cyl_CleanStrip_SprayNzl())
                        {
                            Seq     = ESeq.Idle;
                            m_iSeq  = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> OFP Strip clean finish (Srpay Nozzle).");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Strip Cleaning OK (Srpay Nozzle)");
                            }
                        }
                    }
                    else
                    {
                        if (CSq_OfP.It.Cyl_CleanStrip())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> OFP Strip clean finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Strip Cleaning OK");
                            }
                        }
                    }
                    break;

                case ESeq.OFP_BtmCleanBrush:        // 2022.07.27 lhs
                    if (CSq_OfP.It.Cyl_CleanBrush())
                    {
                        Seq     = ESeq.Idle;
                        m_iSeq  = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Strip brush clean finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off-Picker Strip Brush Cleaning OK");
                        }
                    }
                    break;

                case ESeq.OFP_Conv:
                    if (CSq_OfP.It.Cyl_Conversion())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Conversion finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Conversion OK");
                        }
                    }
                    break;

                //190822 pjh : OfpConvWait
                case ESeq.OFP_ConvWait:
                    if(CSq_OfP.It.Cyl_ConversionWait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if(CSQ_Main.It.m_iStat!=EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFP Conversion wait finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Picker Conversion Wait OK");
                        }
                    }
                    break;
                #endregion

                #region Dry Part
                case ESeq.DRY_Home:
                    if (CSq_Dry.It.Cyl_Home())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> DRY Home finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Dry Home OK");
                        }
                    }
                    break;

                case ESeq.DRY_Wait:
                    if (CSq_Dry.It.Cyl_Wait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> DRY Wait finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Dry Move Wait Position OK");
                        }
                    }
                    break;

                case ESeq.DRY_CheckSensor:
                    // 2021.04.15 lhs Start
                    if (CDataOption.UseDryWtNozzle                  ||  // Dryer Water Nozzle 사용시는 이 시퀀스는 사용 못함.
                        CDataOption.eDryClamp == EDryClamp.Cylinder)    // 2022.07.01 lhs : Cylinder Clamp 사용시 이 시퀀스는 사용 안함.
                    {
                        Seq     = ESeq.Idle;
                        m_iSeq  = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> Dry : DRY_CheckSensor can be used when UseDryWtNozzle is false.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Dry : DRY_CheckSensor can be used when UseDryWtNozzle is false.");
                        }
                        break;
                    }
                    // 2021.04.15 lhs End

                    if (CDataOption.Use2004U)   //210923 syc : 2004U
                    {
                        if (CSq_Dry.It.Cyl_ChkCarrierLevel())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Check safty finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Check Safty Sensor OK");
                            }
                        }
                    }
                    else if (CDataOption.Package == ePkg.Strip)
                    {
                        if (CSq_Dry.It.Cyl_ChkSafety())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Check safty finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Check Safty Sensor OK");
                            }
                        }
                    }
                    else
                    {
                        if (CSq_Dry.It.Cyl_ChkUnit())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Check unit finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Check Unit Sensor OK");
                            }
                        }
                    }
                    break;

                case ESeq.DRY_WaterNozzle:
                    if (CDataOption.Package == ePkg.Strip && CDataOption.UseDryWtNozzle) // Strip 이고 Water Nozzle을 사용할 경우만..
                    {
                        CSq_Dry.It.Run_Stick(CSq_Dry.It.m_bStickRun);
                        if (CSq_Dry.It.Cyl_WaterNozzle())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Water Nozzle finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Water Nozzle OK");
                            }
                        }
                    }
                    else // 그외에는 사용 안하는 거로...
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> Dry : DRY_WaterNozzle can be used when UseDryWtNozzle is true.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Dry : DRY_WaterNozzle can be used when UseDryWtNozzle is true.");
                        }
                    }
                    break;

                case ESeq.DRY_Out:
                    //190405 ksg : Qc
                    if (CDataOption.UseQC && CData.Opt.bQcUse)
                    {
                        if (CSq_Dry.It.Cyl_DryOut_Qc())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Out finish(QC).");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Strip Out OK");
                            }
                        }
                    }
                    else
                    {
                        if (CSq_Dry.It.Cyl_DryOut())
                        {
                            Seq    = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            CSq_Dry.It.m_DuringDryOut = false; //191107 ksg :
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Out finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Strip Out OK");
                            }
                        }
                    }
                    
                    break;

                case ESeq.DRY_Run:
                    if (CDataOption.Dryer == eDryer.Rotate)
                    {
                        if (CDataOption.UseDryWtNozzle)     // 2021.04.16 lhs : // Water Nozzle 적용시
                        {
                            if (CSq_Dry.It.Cyl_DryWork_WtNzl()) 
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> DRY Run(Water Nozzle) finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Dry Work(Water Nozzle) OK");
                                }
                            }
                        }
                        else // 기존
                        {
                            CSq_Dry.It.Run_Stick(CSq_Dry.It.m_bStickRun);
                            if (CSq_Dry.It.Cyl_DryWork())    // 2022.06.30 lhs : 사이클 내부에 UseOffP_IOIV 시퀀스 추가
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> DRY Run finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Dry Work OK");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (CSq_Dry.It.Cyl_DryWorkU()) // 200314 mjy : 신규추가
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> DRY Run finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Dry Work OK");
                            }
                        }
                    }
                    break;
                #endregion

                #region Off Loader Part
                case ESeq.OFL_Wait:
                    if (CSq_OfL.It.Cyl_Wait())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Wait finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Move Wait Position OK");
                        }
                    }
                    break;

                case ESeq.OFL_Home:
                    if (CSq_OfL.It.Cyl_Home())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Home finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Home OK");
                        }
                    }
                    break;
                case ESeq.OFL_TopPick:
#if true //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드, SPIL)
                    bool bResult = false;

                    if (CDataOption.OflRfid == eOflRFID.Use && CDataOption.RFID == ERfidType.Keyence && CData.Opt.eEmitMgz == EMgzWay.Top) //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
                    //if (CDataOption.OflRfid == eOflRFID.Use && CData.Opt.eEmitMgz == EMgzWay.Top && CData.CurCompany == ECompany.SPIL)
                    {
                        if (CData.Opt.bOflRfidSkip)
                        { bResult = CSq_OfL.It.Cyl_TopPick(); }
                        else
                        { bResult = CSq_OfL.It.Cyl_TopPick_Bcr(); }
                    }
                    else
                    { bResult = CSq_OfL.It.Cyl_TopPick(); }

                    if (bResult)
                    {
                        Seq    = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Pick finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Top Conveyor Pick OK");
                        }
                    }
#else
                    if (CSq_OfL.It.Cyl_TopPick())
                    {
                        Seq    = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            m_LogVal.sMsg = "OFL_TopPick Complet";
                            SaveLog();
                            //190410 ksg :
                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Top Conveyor Pick OK");
                        }
                    }
#endif
                    break;
                case ESeq.OFL_TopPlace:
                    if (CSq_OfL.It.Cyl_TopPlace())
                    {
                        Seq    = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Place finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Top Conveyor Place OK");
                        }
                    }
                    break;

                case ESeq.OFL_TopRecive:
                    if (CSq_OfL.It.Cyl_TopRecive())
                    {
                        Seq    = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Top receive finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Top Conveyor Receive OK");
                        }
                    }
                    break;
                case ESeq.OFL_BtmPick:
                    //200120 ksg :
                    //if(CData.CurCompany != eCompany.JSCK) //190418 ksg :
                    if(CDataOption.OflRfid == eOflRFID.NotUse) //200120 ksg :
                    { 
                        if (CSq_OfL.It.Cyl_BtmPick())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != EStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = EStatus.Idle;
                                _SetLog(">>>>> OFL Bottom receive finish.");

                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Bottom Conveyor Pick OK");
                            }
                        }
                    }
                    else 
                    { 
                        //200203 ksg : cdata.opt에 사용 여부 추가로 인하여 분기 나눔
                        /*
                        if (CSq_OfL.It.Cyl_BtmPick_Rfid())
                        {
                            Seq = ESeq.Idle;
                            m_iSeq = ESeq.Idle;
                            if (CSQ_Main.It.m_iStat != eStatus.Error)
                            {
                                CSQ_Main.It.m_iStat = eStatus.Idle;
                                m_LogVal.sMsg = "OFL_BtmPick Complet";
                                SaveLog();
                                //190410 ksg :
                                bBtnShow = true;
                                CMsg.Show(eMsg.Notice, "Notice", "Off Loader Bottom Conveyor Pick OK");
                            }
                        }
                        */
                        if(CData.Opt.bOflRfidSkip)
                        {
                            if (CSq_OfL.It.Cyl_BtmPick())
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> OFL Bottom pick finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Off Loader Bottom Conveyor Pick OK");
                                }
                            }
                        }
                        else
                        {
                            if (CSq_OfL.It.Cyl_BtmPick_Rfid())
                            {
                                Seq = ESeq.Idle;
                                m_iSeq = ESeq.Idle;
                                if (CSQ_Main.It.m_iStat != EStatus.Error)
                                {
                                    CSQ_Main.It.m_iStat = EStatus.Idle;
                                    _SetLog(">>>>> OFL Bottom pick rfid finish.");

                                    bBtnShow = true;
                                    CMsg.Show(eMsg.Notice, "Notice", "Off Loader Bottom Conveyor Pick OK");
                                }
                            }
                        }
                    }
                    break;

                case ESeq.OFL_BtmPlace:
                    if (CSq_OfL.It.Cyl_BtmPlace())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Place finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Bottom Conveyor Place OK");
                        }
                    }
                    break;

                case ESeq.OFL_BtmRecive:
                    if (CSq_OfL.It.Cyl_BtmRecive())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;
                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL Receive finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", "Off Loader Bottom Conveyor Receive OK");
                        }
                    }
                    break;


                // 2022.04.19 SungTae Start : [추가] ASE-KH Magazine ID Reading
                case ESeq.OFL_CheckBcr:
                    if (CSq_OfL.It.Cyl_CheckBcr())
                    {
                        Seq = ESeq.Idle;
                        m_iSeq = ESeq.Idle;

                        if (CSQ_Main.It.m_iStat != EStatus.Error)
                        {
                            CSQ_Main.It.m_iStat = EStatus.Idle;
                            _SetLog(">>>>> OFL MGZ ID Reading Finish.");

                            bBtnShow = true;
                            CMsg.Show(eMsg.Notice, "Notice", CLang.It.GetConvertMsg("Off-Loader MGZ ID Reading OK"));
                        }
                    }
                    break;
                    // 2022.04.19 SungTae End
                    #endregion
            }
        }

        public bool Cyl_AllHome()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_mTiOut.Set_Delay(TIMEOUT*5); }
            else
            {
                if (m_mTiOut.Chk_Delay())
                {
                    CErr.Show(eErr.MANUAL_ALL_HOME_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }
            //koo : pause 홈밍 동작 중 area센서 감지 되면 모터 All 정지 후 에러 발생 
            if(CData.CurCompany == ECompany.SkyWorks)
            {
                bool LeftArea  = CIO.It.Get_X(eX.ONL_LightCurtain);
                bool RightArea = CIO.It.Get_X(eX.OFFL_LightCurtain);
                if (LeftArea)
                {
                    //Servo All EStop
                    for (int i = CMot.It.AxS; i <= CMot.It.AxL; i++)
                    {
                        CMot.It.EStop(i);
                    }
                    HomeDoneReset(); //200217 ksg :
                    CErr.Show(eErr.ONLOADER_DETECT_LIGHTCURTAIN);
                    _SetLog("Error : ONL Light curtain detect.");

                    m_iStep = 0;
                    return true;
                }
                if (RightArea)
                {
                    //Servo All EStop
                    for (int i = CMot.It.AxS; i <= CMot.It.AxL; i++)
                    {
                        CMot.It.EStop(i);
                    }
                    HomeDoneReset(); //200217 ksg :
                    CErr.Show(eErr.OFFLOADER_DETECT_LIGHTCURTAIN);
                    _SetLog("Error : OFL Light curtain detect.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:    // 축 상태 체크
                    {
                        bool bSrv = false;

                        for (int i = CMot.It.AxS; i < CMot.It.AxL; i++)
                        {
                            bSrv = CMot.It.Chk_Srv(i);
                        }

                        if (bSrv)
                        {
                            Init_Part();

                            CSq_OnL.It.m_bHD = false;
                            CSq_OfL.It.m_bHD = false;
                            CSq_OnP.It.m_bHD = false;
                            CSq_OfP.It.m_bHD = false;
                            CSq_Inr.It.m_bHD = false;
                            CData.L_GRD.m_bHD = false;
                            CData.R_GRD.m_bHD = false;
                            CSq_Dry.It.m_bHD = false;

                            _SetLog("Init part.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 11:    // 축 상태 체크
                    {
                        if (!CSq_OnL.It.m_bHD) { CSq_OnL.It.Cyl_Home(); }
                        if (!CSq_OfL.It.m_bHD) { CSq_OfL.It.Cyl_Home(); }
                        //if (!CSq_OnP.It.m_bHD) { CSq_OnP.It.Cyl_Home(); }
                        if (!CSq_OfP.It.m_bHD) { CSq_OfP.It.Cyl_Home(); }
                        if (!CSq_Inr.It.m_bHD) { CSq_Inr.It.Cyl_Home(); }

                        if ((!CMot.It.Chk_HD((int)EAx.OffLoaderPicker_Z)) || (!CMot.It.Chk_HD((int)EAx.OffLoaderPicker_X))) return false;
                        if (!CMot.It.Chk_HD((int)EAx.Inrail_Y)) return false;
                        /*
                        if (CMot.It.Chk_HD((int)eAx.OnLoaderPicker_Z) && CMot.It.Chk_HD((int)eAx.OffLoaderPicker_Z))
                        {
                            if (!CSq_OnP.It.m_bHD) { CSq_OnP.It.Cyl_Home(); }
                            //if (!CSq_Inr.It.m_bHD) { CSq_Inr.It.Cyl_Home(); }
                            if (!CData.L_GRD.m_bHD) { CData.L_GRD.Cyl_Home(); }
                            if (!CData.R_GRD.m_bHD) { CData.R_GRD.Cyl_Home(); }
                            if (!CSq_Dry.It.m_bHD) { CSq_Dry.It.Cyl_Home(); }
                        }
                        */
                        if (!CSq_OnP.It.m_bHD) { CSq_OnP.It.Cyl_Home(); }
                        //if (!CSq_Inr.It.m_bHD) { CSq_Inr.It.Cyl_Home(); }
                        if (!CData.L_GRD.m_bHD) { CData.L_GRD.Cyl_Home(); }
                        if (!CData.R_GRD.m_bHD) { CData.R_GRD.Cyl_Home(); }
                        if (!CSq_Dry.It.m_bHD) { CSq_Dry.It.Cyl_Home(); }

                        if (CSq_OnL.It.m_bHD &&
                            CSq_Inr.It.m_bHD &&
                            CSq_OnP.It.m_bHD &&
                            CData.L_GRD.m_bHD &&
                            CData.R_GRD.m_bHD &&
                            CSq_OfP.It.m_bHD &&
                            CSq_Dry.It.m_bHD &&
                            CSq_OfL.It.m_bHD)
                        {
                            _SetLog("Check all home.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 12:    // 종료
                    {
                        // 2021-06-10, jhLee, Multi-LOT 사용시 데이터 초기화 수행
                        if (CData.IsMultiLOT())
                        {
                            CData.LotMgr.ClearLotInfo();
                        }

                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_WarmUp()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            {
                m_mTiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_mTiOut.Chk_Delay())
                {
                    _SetLog("Error : Timeout.");

                    AbortWarmUpMotor("Timeout");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            //if(CDataOption.SplType == eSpindleType.EtherCat)
            //{
            //    CData.Spls[0].iRpm =  Convert.ToInt32(CSpl.It.GetFrpm(true));   // Left
            //    CData.Spls[1].iRpm =  Convert.ToInt32(CSpl.It.GetFrpm(false));  // Right
            //}
            // 2023.03.15 Max
            CData.Spls[(int)EWay.L].iRpm = Convert.ToInt32(CSpl_485.It.GetFrpm(EWay.L));
            CData.Spls[(int)EWay.R].iRpm = Convert.ToInt32(CSpl_485.It.GetFrpm(EWay.R));

            //20191205 ghk_warmup_error
            if (m_iStep > 12 && m_iStep < 23)
            {//m_iStep가 12번 초과 23미만 일때 탑클린 업상태 / 탑 클린 워터 온 상태 / 드라이 바텀 클린 워터 온 상태 상시 검사
                //syc : new cleaner
                if (CData.L_GRD.Chk_CleanerDn(false))
                {
                    CErr.Show(eErr.LEFT_GRIND_TOPCLEAN_NOT_UP);
                    m_iStep = 0;

                    AbortWarmUpMotor("LEFT_GRIND_TOPCLEAN_NOT_UP");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                    return true;
                }
                //syc : new cleaner
                if (CData.R_GRD.Chk_CleanerDn(false))
                {
                    CErr.Show(eErr.RIGHT_GRIND_TOPCLEAN_NOT_UP);
                    m_iStep = 0;

                    AbortWarmUpMotor("RIGHT_GRIND_TOPCLEAN_NOT_UP");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDL_TopClnFlow))
                {
                    CErr.Show(eErr.LEFT_GRIND_TOPCLEAN_WATER_ERROR);
                    m_iStep = 0;

                    AbortWarmUpMotor("LEFT_GRIND_TOPCLEAN_WATER_ERROR");   
                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDR_TopClnFlow))
                {
                    CErr.Show(eErr.RIGHT_GRIND_TOPCLEAN_WATER_ERROR);
                    m_iStep = 0;

                    AbortWarmUpMotor("RIGHT_GRIND_TOPCLEAN_WATER_ERROR"); 
                    return true;
                }

                if (CDataOption.UseSprayBtmCleaner == false)    // 2022.04.29 lhs : Spray Nozzle형은 제외
                {
                    if (!CIO.It.Get_X(eX.DRY_ClnBtmFlow))
                    {
                        CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                        m_iStep = 0;

                        AbortWarmUpMotor("DRY_BOTTOM_CLEAN_WATER_ERROR");
                        return true;
                    }
                }
            }

            if(m_iStep > 18 && m_iStep < 21)
            {//m_iStep가 18번 초과 21미만 일때 그라인딩 워터 / 그라인딩 바텀 워터 상시 검사
                if (!CIO.It.Get_X(eX.GRDL_SplWater))
                {
                    CErr.Show(eErr.LEFT_GRIND_SPINDLE_WATER_ERROR);
                    m_iStep = 0;

                    AbortWarmUpMotor("LEFT_GRIND_SPINDLE_WATER_ERROR");  
                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDL_SplBtmWater))
                {
                    CErr.Show(eErr.LEFT_GRIND_SPINDLE_BOTTOM_WATER_ERROR);
                    m_iStep = 0;

                    AbortWarmUpMotor("LEFT_GRIND_SPINDLE_BOTTOM_WATER_ERROR");  
                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDL_SplPCW))
                {
                    CErr.Show(eErr.LEFT_GRIND_SPINDLE_COOLANT_OFF);
                    m_iStep = 0;

                    AbortWarmUpMotor("LEFT_GRIND_SPINDLE_COOLANT_OFF");  
                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDR_SplWater))
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_WATER_ERROR);
                    AbortWarmUpMotor("RIGHT_GRIND_SPINDLE_WATER_ERROR");             
                    m_iStep = 0;

                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDR_SplPCW))
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_COOLANT_OFF);
                    m_iStep = 0;

                    AbortWarmUpMotor("RIGHT_GRIND_SPINDLE_COOLANT_OFF");                   
                    return true;
                }

                if (!CIO.It.Get_X(eX.GRDR_SplBtmWater))
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_BOTTOM_WATER_ERROR);
                    m_iStep = 0;

                    AbortWarmUpMotor("RIGHT_GRIND_SPINDLE_BOTTOM_WATER_ERROR");                   
                    return true;
                }

                //20200618 myk : Spindle Bottom Air Check Error 추가
                if(CDataOption.IsBtmAir == true)
                {
                    if (!CIO.It.Get_X(eX.GRDL_SplBtmAir))
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_BOTTOM_AIR_ERROR);
                        m_iStep = 0;

                        AbortWarmUpMotor("LEFT_GRIND_SPINDLE_BOTTOM_AIR_ERROR");                   
                        return true;
                    }

                    if (!CIO.It.Get_X(eX.GRDR_SplBtmAir))
                    {
                        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_BOTTOM_AIR_ERROR);
                        m_iStep = 0;

                        AbortWarmUpMotor("RIGHT_GRIND_SPINDLE_BOTTOM_AIR_ERROR");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                        return true;
                    }
                }
            }
            //

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:    // 축 상태 체크
                    {
                        if (CData.L_GRD.Chk_Axes())
                        {
                            m_iStep = 0;
                            AbortWarmUpMotor("Check Left-Table Axes error");   
                            return true;
                        }
                        if (CData.R_GRD.Chk_Axes())
                        {
                            m_iStep = 0;
                            AbortWarmUpMotor("Check Right-Table Axes error");   
                            return true;
                        }

                        // 2023.03.15 Max
                        // 485 방식 변경으로 하기 Chk_Servo 는 주석처리

                        //if (!CSpl.It.Chk_Servo(EWay.L))
                        //{
                        //    CErr.Show(eErr.LEFT_SPINDLE_SERVO_OFF);
                        //    m_iStep = 0;
                        //    AbortWarmUpMotor("LEFT_SPINDLE_SERVO_OFF");         
                        //    return true;
                        //}

                        //if (!CSpl.It.Chk_Servo(EWay.R))
                        //{
                        //    CErr.Show(eErr.RIGHT_SPINDLE_SERVO_OFF);
                        //    m_iStep = 0;
                        //    AbortWarmUpMotor("RIGHT_SPINDLE_SERVO_OFF");     
                        //    return true;
                        //}



                        //if (CSpl.It.Chk_Alarm(EWay.L))
                        //{
                        //    CErr.Show(eErr.LEFT_SPINDLE_ALARM);
                        //    m_iStep = 0;
                        //    AbortWarmUpMotor("LEFT_SPINDLE_ALARM");        
                        //    return true;
                        //}

                        //if (CSpl.It.Chk_Alarm(EWay.R))
                        //{
                        //    CErr.Show(eErr.RIGHT_SPINDLE_ALARM);
                        //    m_iStep = 0;
                        //    AbortWarmUpMotor("RIGHT_SPINDLE_ALARM");         
                        //    return true;
                        //}

                        // 2023.03.15 Max
                        if (CSpl_485.It.Chk_Alarm(EWay.L))
                        {
                            CErr.Show(eErr.LEFT_SPINDLE_ALARM);
                            m_iStep = 0;
                            AbortWarmUpMotor("LEFT_SPINDLE_ALARM");        
                            return true;
                        }

                        if (CSpl_485.It.Chk_Alarm(EWay.R))
                        {
                            CErr.Show(eErr.RIGHT_SPINDLE_ALARM);
                            m_iStep = 0;
                            AbortWarmUpMotor("RIGHT_SPINDLE_ALARM");         
                            return true;
                        }



                        m_iCnt = 0;
                        m_dPos = 0;
                        _SetLog("Check axes.");

                        m_iStep++;

                        return false;
                    }

                case 11:    // 탑클리너 업 및 스펀지 워터 온, 바텀 클리닝 스펀지 온, 테이블 이젝트 오프
                    {
                        //syc : new cleaner
                        //CIO.It.Set_Y(eY.GRDL_TopClnDn, false);
                        //CIO.It.Set_Y(eY.GRDR_TopClnDn, false);
                        CData.L_GRD.ActTcDown(false);
                        CData.R_GRD.ActTcDown(false);
                        //CIO.It.Set_Y(eY.GRDL_TopClnFlow, true);
                        //CIO.It.Set_Y(eY.GRDR_TopClnFlow, true);
                        CData.L_GRD.Func_TcWater(true);
                        CData.R_GRD.Func_TcWater(true);

                        if (CDataOption.UseSprayBtmCleaner == false) // 2022.04.29 lhs : Spray Nozzle형은 제외(웜업시 물이 많이 튐)
                        {
                            CIO.It.Set_Y(eY.DRY_ClnBtmFlow, true);
                        }                       
                        CData.L_GRD.ActEject(false);
                        CData.R_GRD.ActEject(false);

                        //20191128 ghk_warmup_error
                        m_mFlowTime.Set_Delay(3000);
                        _SetLog("Init GDL, GDR IO.");

                        m_iStep++;
                        return false;
                    }

                case 12:    // 탑클리너 업 및 워터, 바텀클리닝 워터 확인
                    {
                        //20191128 ghk_warmup_error
                        if (!m_mFlowTime.Chk_Delay())
                        { return false; }

                        //syc : new cleaner 이면 검사 안함
                        //if (CIO.It.Get_X(eX.GRDL_TopClnDn))
                        if (CData.L_GRD.Chk_CleanerDn(false))
                        {
                            CErr.Show(eErr.LEFT_GRIND_TOPCLEAN_NOT_UP);
                            m_iStep = 0;

                            return true;
                        }

                        //syc : new cleaner
                        //if (CIO.It.Get_X(eX.GRDR_TopClnDn))
                        if (CData.R_GRD.Chk_CleanerDn(false))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_TOPCLEAN_NOT_UP);
                            m_iStep = 0;

                            return true;
                        }

                        if (!CIO.It.Get_X(eX.GRDL_TopClnFlow))
                        {
                            CErr.Show(eErr.LEFT_GRIND_TOPCLEAN_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (!CIO.It.Get_X(eX.GRDR_TopClnFlow))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_TOPCLEAN_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (CDataOption.UseSprayBtmCleaner == false)    // 2022.04.29 lhs : Spray Nozzle형은 제외
                        {
                            if (!CIO.It.Get_X(eX.DRY_ClnBtmFlow))
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                m_iStep = 0;

                                return true;
                            }
                        }

                        _SetLog("Check water.");

                        m_iStep++;
                        return false;
                    }

                case 13:    // 프로브 업
                    {
                        CIO.It.Set_Y(eY.GRDL_ProbeDn, false);
                        CIO.It.Set_Y(eY.GRDR_ProbeDn, false);
                        _SetLog("Probe up.");

                        m_iStep++;
                        return false;
                    }

                case 14:    // 프로브 업 확인
                    {
                        //200710 jhc : 프로브 업체크 조건 = (프로브 UP 신호) && (프로브 값 > 기준 높이)
                        //if (CIO.It.Get_X(eX.GRDL_ProbeAMP) && CIO.It.Get_X(eX.GRDR_ProbeAMP))
                        double dProbeValL = CPrb.It.Read_Val(EWay.L); //프로브 값
                        double dProbeValR = CPrb.It.Read_Val(EWay.R); //프로브 값
                        if (CIO.It.Get_X(eX.GRDL_ProbeAMP) && (dProbeValL >= GV.dProbeUpHeight) && CIO.It.Get_X(eX.GRDR_ProbeAMP) && (dProbeValR >= GV.dProbeUpHeight))
                        {
                            _SetLog("Check probe up.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 15:    // 프로브 축 대기위치 이동
                    {
                        CMot.It.Mv_MN((int)EAx.LeftGrindZone_X, CData.SPos.dGRD_X_Wait[0],
                                                (int)EAx.RightGrindZone_X, CData.SPos.dGRD_X_Wait[1]);
                        _SetLog("GDL X, GDR X axis move wait.");

                        m_iStep++;
                        return false;
                    }

                case 16:    // 프로브 축 대기위치 이동 확인 및 Z축 Able 이동
                    {
                        if (CMot.It.Get_Mv((int)EAx.LeftGrindZone_X, CData.SPos.dGRD_X_Wait[0]) &&
                            CMot.It.Get_Mv((int)EAx.RightGrindZone_X, CData.SPos.dGRD_X_Wait[1]))
                        {
                            CMot.It.Mv_MN((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Able[0],
                                          (int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Able[1]);
                            _SetLog("GDL Z, GDR Z axis move albe.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 17:    // Z축 이동 확인 및 스핀들 동작, 워터 온
                    {
                        if (CMot.It.Get_Mv((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Able[0]) &&
                            CMot.It.Get_Mv((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Able[1]))
                        {
                            //CSpl.It.Write_Run(EWay.L, CData.Opt.iWmUS);
                            // 2023.03.15 Max
                            bool bExit = false;
                            bool bTimeOutFlag = false;

                            // RPM Setting
                            CSpl_485.It.Write_Rpm(EWay.L, CData.Opt.iWmUS);
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
                                return false;
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
                                return false;
                            }

                            //CSpl.It.Write_Run(EWay.R, CData.Opt.iWmUS);
                            // 2023.03.15 Max
                            bExit = false;
                            bTimeOutFlag = false;
                            // RPM Setting
                            CSpl_485.It.Write_Rpm(EWay.R, CData.Opt.iWmUS);
                            m_Timout_485.Set_Delay(300);
                            do
                            {
                                Application.DoEvents();
                                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                                bExit = CSpl_485.It.GetAcceptRPMSpindle(EWay.R);
                            } while (bExit != true);
                            CSpl_485.It.SetAcceptRPMSpindle(EWay.R);

                            if (bTimeOutFlag)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
                                return false;
                            }

                            // Spindle RUN
                            bExit = false;
                            bTimeOutFlag = false;
                            CSpl_485.It.Write_Run(EWay.R);
                            m_Timout_485.Set_Delay(300);
                            do
                            {
                                Application.DoEvents();
                                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                                bExit = CSpl_485.It.GetAcceptRunSpindle(EWay.R);
                            } while (bExit != true);
                            CSpl_485.It.SetAcceptRunSpindle(EWay.R);

                            if (bTimeOutFlag)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_SPINDLE_RUN_RPM_ERROR);
                                return false;
                            }


                            CIO.It.Set_Y(eY.GRDL_SplWater, true);
                            CIO.It.Set_Y(eY.GRDL_SplBtmWater, true);
                            CIO.It.Set_Y(eY.GRDR_SplWater, true);
                            CIO.It.Set_Y(eY.GRDR_SplBtmWater, true);
                            //200515 myk : Wheel Cleaner Water 추가
                            //220927 pjh : Warm Up 시 Wheel Cleaner 사용하지 않음
                            if (CDataOption.IsWhlCleaner && !CData.Opt.bWhlClnSkip)
                            {
                                CIO.It.Set_Y(eY.GRDL_WhlCleaner, false);
                                CIO.It.Set_Y(eY.GRDR_WhlCleaner, false);
                            }

                            //20191128 ghk_warmup_error
                            m_mFlowTime.Set_Delay(3000);
                            _SetLog("Water on.  Spindle run.  Vel : " + CData.Opt.iWmUS + "rpm");

                            m_iStep++;
                        }
                        return false;
                    }

                case 18:    // 스핀들 확인 및 워터 확인 
                    {
                        //20191128 ghk_warmup_error
                        if (!m_mFlowTime.Chk_Delay())
                        { return false; }

                        if (!CIO.It.Get_X(eX.GRDL_SplWater))
                        {
                            CErr.Show(eErr.LEFT_GRIND_SPINDLE_WATER_ERROR);
                            m_iStep = 0;

                            AbortWarmUpMotor("LEFT_GRIND_SPINDLE_WATER_ERROR");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.GRDL_SplBtmWater))
                        {
                            CErr.Show(eErr.LEFT_GRIND_SPINDLE_BOTTOM_WATER_ERROR);
                            m_iStep = 0;

                            AbortWarmUpMotor("LEFT_GRIND_SPINDLE_BOTTOM_WATER_ERROR");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.GRDR_SplWater))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_SPINDLE_WATER_ERROR);
                            m_iStep = 0;

                            AbortWarmUpMotor("DRY_BOTTOM_CLEAN_WATER_ERROR");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.GRDR_SplBtmWater))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_SPINDLE_BOTTOM_WATER_ERROR);
                            m_iStep = 0;

                            AbortWarmUpMotor("RIGHT_GRIND_SPINDLE_BOTTOM_WATER_ERROR");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                            return true;
                        }
                        //

                        if (90 < CData.Spls[0].iRpm && 90 < CData.Spls[1].iRpm)
                        {
                            int iTime = CData.Opt.iWmUT;
                            // 201023 jym : 추가
                            if (CData.CurCompany == ECompany.SkyWorks && CDataOption.AutoWarmUp == eAutoWarmUp.Use && CData.IsATW)
                            {
                                iTime = CData.Opt.iAWT;
                                //CData.IsATW = false;
                            }
                            CData.Warm = DateTime.Now.Add(new TimeSpan(0, iTime, 0));
                            _SetLog("Check spindle.");
                            m_iStep++;
                        }
                        return false;
                    }

                case 19:    // 테이블 이동
                    {
                        m_dPos = (m_iCnt % 2 == 0) ? 200 : 600;

                        //190904 ksg :
                        if (!CIO.It.Get_X(eX.GRDL_SplWater) || !CIO.It.Get_X(eX.GRDL_SplBtmWater) ||
                            !CIO.It.Get_X(eX.GRDR_SplWater) || !CIO.It.Get_X(eX.GRDR_SplBtmWater))
                        {
                            if (!CIO.It.Get_X(eX.GRDL_SplWater) || !CIO.It.Get_X(eX.GRDR_SplWater))
                            {
                                CErr.Show(eErr.TABLE_WATER_OFF_DURING_WARMUP);
                                _SetLog("Error : Spindle water off.");
                                AbortWarmUpMotor("TABLE_WATER_OFF_DURING_WARMUP");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                            }
                            else if (!CIO.It.Get_X(eX.GRDL_SplBtmWater) || !CIO.It.Get_X(eX.GRDR_SplBtmWater))
                            {
                                CErr.Show(eErr.TABLE_WATER_OFF_DURING_WARMUP);
                                _SetLog("Error : Bottom water off.");
                                AbortWarmUpMotor("TABLE_WATER_OFF_DURING_WARMUP");                    // 가동중인 Spindle motor및 관련 Water vaue 잠그고 종료시킨다.
                            }
                            CSQ_Man.It.WaterOnOff(false); //syc : All Water On/Off
                            StopWater();                            

                            m_iStep = 0;
                            return true;
                        }

                        CMot.It.Mv_MN((int)EAx.LeftGrindZone_Y, m_dPos,
                                      (int)EAx.RightGrindZone_Y, m_dPos);
                        _SetLog("GDL Y, GDR Y axis move position.", m_dPos);

                        m_iStep++;
                        return false;
                    }

                case 20:    // 테이블 이동 확인 및 루프
                    {
                        if (CMot.It.Get_Mv((int)EAx.LeftGrindZone_Y, m_dPos) &&
                            CMot.It.Get_Mv((int)EAx.RightGrindZone_Y, m_dPos))
                        {
                            m_iCnt++;

                            if (CData.Warm <= DateTime.Now) // (m_iCnt == 1000)
                            {
                                _SetLog("Warm up time end.");

                                m_iStep++;
                            }
                            else
                            {
                                _SetLog("Loop.");

                                m_iStep = 19;
                            }
                        }
                        return false;
                    }

                case 21:    // Z스핀들 스탑, 워터 오프
                    {
                        //CSpl.It.Write_Stop(EWay.L);
                        //CSpl.It.Write_Stop(EWay.R);
                        // 2023.03.15 Max
                        CSpl_485.It.Write_Stop(EWay.L);
                        CSpl_485.It.Write_Stop(EWay.R);

                        CIO.It.Set_Y(eY.GRDL_SplWater, false);
                        CIO.It.Set_Y(eY.GRDL_SplBtmWater, false);
                        CIO.It.Set_Y(eY.GRDR_SplWater, false);
                        CIO.It.Set_Y(eY.GRDR_SplBtmWater, false);
                        if (CDataOption.IsWhlCleaner && !CData.Opt.bWhlClnSkip)
                        {
                            CIO.It.Set_Y(eY.GRDL_WhlCleaner, false);
                            CIO.It.Set_Y(eY.GRDR_WhlCleaner, false);
                        }

                        //20191128 ghk_warmup_error
                        m_mFlowTime.Set_Delay(1000);
                        _SetLog("Spindle stop.  Water off.");

                        m_iStep++;
                        return false;
                    }

                case 22:    // 스핀들 스탑 및 워터 오프 확인 
                    {
                        //20191128 ghk_warmup_error
                        if (!m_mFlowTime.Chk_Delay())
                        { return false; }

                        /*
                        if (CIO.It.Get_X(eX.GRDL_SplWater))
                        {
                            CErr.Show(eErr.LEFT_GRIND_SPINDLE_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (CIO.It.Get_X(eX.GRDL_SplBtmWater))
                        {
                            CErr.Show(eErr.LEFT_GRIND_SPINDLE_BOTTOM_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (CIO.It.Get_X(eX.GRDR_SplWater))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_SPINDLE_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (CIO.It.Get_X(eX.GRDR_SplBtmWater))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_SPINDLE_BOTTOM_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }
                        //
                        */

                        if (CData.Spls[0].iRpm == 0 && CData.Spls[1].iRpm == 0)
                        {
                            _SetLog("Check spindle.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 23:    // 탑클리너 스펀지 워터 오프, 바텀 클리닝 스펀지 오프
                    {
                        CIO.It.Set_Y(eY.GRDL_TopClnFlow, false);
                        CIO.It.Set_Y(eY.GRDR_TopClnFlow, false);
                        CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);

                        CData.dtSpgWOffLastTime = DateTime.Now;

                        //20191128 ghk_warmup_error
                        m_mFlowTime.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        m_iStep++;
                        return false;
                    }

                case 24:    // 탑클리너 업 및 워터, 바텀클리닝 워터 확인
                    {
                        //20191128 ghk_warmup_error
                        if (!m_mFlowTime.Chk_Delay())
                        { return false; }

                        /*
                        if (CIO.It.Get_X(eX.GRDL_TopClnFlow))
                        {
                            CErr.Show(eErr.LEFT_GRIND_TOPCLEAN_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (CIO.It.Get_X(eX.GRDR_TopClnFlow))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_TOPCLEAN_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if (CIO.It.Get_X(eX.DRY_ClnBtmFlow))
                        {
                            CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                            m_iStep = 0;

                            return true;
                        }
                        // 
                        */
                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//201109 pjh : Wait Position 이동 후 Warm Up 종료 되도록 Warm Up Sequence 변경 (case 25~28)
                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Z,  CData.SPos.dGRD_Z_Wait[0]);
                        CMot.It.Mv_N((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Wait[1]);

                        _SetLog("Grinder Z axis move wait position");

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {
                        if(!CMot.It.Get_Mv((int)EAx.LeftGrindZone_Z,  CData.SPos.dGRD_Z_Wait[0]) &&
                           !CMot.It.Get_Mv((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Wait[1]))
                        { return false; }

                        CMot.It.Mv_N((int)EAx.LeftGrindZone_X,  CData.SPos.dGRD_X_Wait[0]);
                        CMot.It.Mv_N((int)EAx.RightGrindZone_X, CData.SPos.dGRD_X_Wait[1]);

                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Y,  CData.SPos.dGRD_Y_Wait[0]);
                        CMot.It.Mv_N((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_Wait[1]);

                        _SetLog("Grinder X and Y axis move wait position");

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {
                        if(!CMot.It.Get_Mv((int)EAx.LeftGrindZone_X,  CData.SPos.dGRD_X_Wait[0]) &&
                           !CMot.It.Get_Mv((int)EAx.RightGrindZone_X, CData.SPos.dGRD_X_Wait[1]) &&
                           !CMot.It.Get_Mv((int)EAx.LeftGrindZone_Y,  CData.SPos.dGRD_Y_Wait[0]) &&
                           !CMot.It.Get_Mv((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_Wait[1]))
                        { return false; }

                        _SetLog("Check Grinder X and Y axis move wait position");

                        m_iStep++;
                        return false;
                    }

                case 28:    // 종료
                    {
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //
        // 2021-02-09, jhLee : 지정한 이유로 WarmUp 동작을 중지한다. Spindle을 멈추고, 각종 Water들을 잠그어준다.
        public void AbortWarmUpMotor(string sMsg = "None")
        {
            //CSpl.It.Write_Stop(EWay.L);
            //CSpl.It.Write_Stop(EWay.R);
            // 2023.03.15 Max
            CSpl_485.It.Write_Stop(EWay.L);
            CSpl_485.It.Write_Stop(EWay.R);

            CIO.It.Set_Y(eY.GRDL_SplWater, false);
            CIO.It.Set_Y(eY.GRDL_SplBtmWater, false);
            CIO.It.Set_Y(eY.GRDR_SplWater, false);
            CIO.It.Set_Y(eY.GRDR_SplBtmWater, false);
            if (CDataOption.IsWhlCleaner && !CData.Opt.bWhlClnSkip)
            {
                CIO.It.Set_Y(eY.GRDL_WhlCleaner, false);
                CIO.It.Set_Y(eY.GRDR_WhlCleaner, false);
            }

            CIO.It.Set_Y(eY.GRDL_TopClnFlow, false);
            CIO.It.Set_Y(eY.GRDR_TopClnFlow, false);
            CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);

            _SetLog($"Abort Warm-Up, Resean : {sMsg}");

        }


        /// <summary>
        /// Tenkey Cycle
        /// </summary>
        //190218 ksg :
        public void Tenkey(int Num)
        {
            ESeq ESeq = ESeq.Idle;
            //Onloader
                 if(Num ==  1) ESeq = ESeq.ONL_Pick        ;
            else if(Num ==  2) ESeq = ESeq.ONL_Place       ;
            else if(Num ==  3) ESeq = ESeq.ONL_Push        ;
            //Inrail                                       
            else if(Num == 11) ESeq = ESeq.INR_Align       ;
            else if(Num == 12) ESeq = ESeq.INR_CheckBcr    ;
            else if(Num == 13) ESeq = ESeq.INR_CheckOri    ;
            else if(Num == 14) ESeq = ESeq.INR_CheckDynamic;
            //else if (Num == 12) ESeq = ESeq.INR_BcrReady;
            //else if (Num == 13) ESeq = ESeq.INR_OriReady;
            //
            //Onloder Picker
            else if(Num == 21) ESeq = ESeq.ONP_Pick    ;
            else if(Num == 22) ESeq = ESeq.ONP_PickTbL ;
            else if(Num == 23) ESeq = ESeq.ONP_PlaceTbL;
            else if(Num == 24) ESeq = ESeq.ONP_PlaceTbR;
            else if(Num == 25) ESeq = ESeq.ONP_CheckBcr; //190711 ksg : 버튼 추가 생성으로 추가
            else if(Num == 26) ESeq = ESeq.ONP_CheckOri; //190711 ksg : 버튼 추가 생성으로 추가
            else if(Num == 27) ESeq = ESeq.ONP_Clean   ; //190711 ksg : 버튼 추가 생성으로 추가
            //offloader Picker
            else if(Num == 61) ESeq = ESeq.OFP_PickTbL      ;
            else if(Num == 62) ESeq = ESeq.OFP_PickTbR      ;
            else if(Num == 63) ESeq = ESeq.OFP_Place        ;
            else if(Num == 64) ESeq = ESeq.OFP_BtmClean     ;
            else if(Num == 65) ESeq = ESeq.OFP_BtmCleanStrip;
            //offloader Picker
            else if(Num == 71) ESeq = ESeq.DRY_CheckSensor;
            else if(Num == 72) ESeq = ESeq.DRY_Run        ;
            else if(Num == 73) ESeq = ESeq.DRY_Out        ;
            //offloader 
            else if(Num == 81) ESeq = ESeq.OFL_BtmPick  ;
            else if(Num == 82) ESeq = ESeq.OFL_BtmPlace ;
            else if(Num == 83) ESeq = ESeq.OFL_BtmRecive;
            else if(Num == 84) ESeq = ESeq.OFL_TopPick  ;
            else if(Num == 85) ESeq = ESeq.OFL_TopPlace ;
            else if(Num == 86) ESeq = ESeq.OFL_TopRecive;

            if(ESeq == ESeq.Warm_Up)
            {
                bool bStrip = false;
                if (CDataOption.Use2004U) { bStrip = CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U) || CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U); }
                else
                {
                    // 2022.06.10 lhs Start : (SCK+) 옵션 설정으로 자재가 있어도 WarmUp 수행
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                    {
                        if (CData.Opt.bWarmUpWithStrip) { bStrip = false; } // Strip이 없는 것으로 처리
                        else                            { bStrip = CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDR_TbVaccum);    }
                    }
                    else                                { bStrip = CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDR_TbVaccum);    }
                }
                if (bStrip)
                {
                    CMsg.Show(eMsg.Notice, "Notice", "Strip exist on the table. Please check");
                    return;
                }
            }

            //1900116 ksg : 조건 추가
            if(ESeq == ESeq.GRL_Grinding      || ESeq == ESeq.GRR_Grinding     ||
               ESeq == ESeq.GRL_Strip_Measure || ESeq == ESeq.GRR_Strip_Measure  )
            {
                if(!CSQ_Main.It.CheckWarmUp())
                {
                    CMsg.Show(eMsg.Error, "Error", "Need Warm Up Please");
                    return;
                }
            }
            //190131 ksg : Table 동작 중 Strip Check 기능 추가
            if(ESeq == ESeq.Warm_Up)
            {
                bool bStrip = false;
                //210902 syc : 2004U
                if (CDataOption.Use2004U)
                {
                    bStrip = CIO.It.Get_Y(eY.GRDL_Unit_Vacuum_4U) || CIO.It.Get_Y(eY.GRDL_Carrier_Vacuum_4U) || CIO.It.Get_Y(eY.GRDR_Unit_Vacuum_4U) || CIO.It.Get_Y(eY.GRDR_Carrier_Vacuum_4U);
                }
                else
                {
                    // 2022.06.10 lhs Start : (SCK+) 옵션 설정으로 자재가 있어도 WarmUp 수행
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                    {
                        if (CData.Opt.bWarmUpWithStrip) { bStrip = false; } // Strip이 없는 것으로 처리
                        else                            { bStrip = CIO.It.Get_Y(eY.GRDL_TbVacuum) || CIO.It.Get_Y(eY.GRDR_TbVacuum);    }
                    }
                    else                                { bStrip = CIO.It.Get_Y(eY.GRDL_TbVacuum) || CIO.It.Get_Y(eY.GRDR_TbVacuum);    }  // 기존
				}
                if (bStrip)
                {
                    CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                    return;
                }

                    if (CDataOption.Package == ePkg.Unit)
				{// Hisilicon 일 경우 //20200229 LCY
					if (CIO.It.Get_Y(eY.GRDL_Unit_1_Vacuum) ||
						 CIO.It.Get_Y(eY.GRDL_Unit_2_Vacuum) ||
						 CIO.It.Get_Y(eY.GRDL_Unit_3_Vacuum) ||
						 CIO.It.Get_Y(eY.GRDL_Unit_4_Vacuum))
					{
						CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
						return;
					}
					if (CIO.It.Get_Y(eY.GRDR_Unit_1_Vacuum) ||
						 CIO.It.Get_Y(eY.GRDR_Unit_2_Vacuum) ||
						 CIO.It.Get_Y(eY.GRDR_Unit_3_Vacuum) ||
						 CIO.It.Get_Y(eY.GRDR_Unit_4_Vacuum))
					{
						{
							CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
							return;
						}
					}
				}
				//190131 ksg : Table 동작 중 Strip Check 기능 추가
				if (ESeq == ESeq.GRR_Dressing       || ESeq == ESeq.GRR_Dresser_Measure ||
					ESeq == ESeq.GRR_Wheel_Measure  || ESeq == ESeq.GRR_Table_Grinding)
				{
					if (CIO.It.Get_Y(eY.GRDR_TbVacuum))
					{
						CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
						return;
					}
					if (CDataOption.Package == ePkg.Unit)
					{// Hisilicon 일 경우 //20200229 LCY
						if (CIO.It.Get_Y(eY.GRDL_Unit_1_Vacuum) ||
							CIO.It.Get_Y(eY.GRDL_Unit_2_Vacuum) ||
							CIO.It.Get_Y(eY.GRDL_Unit_3_Vacuum) ||
							CIO.It.Get_Y(eY.GRDL_Unit_4_Vacuum))
						{
							CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
							return;
						}
					}
				}
			}

            //190131 ksg : Table 동작 중 Strip Check 기능 추가
            if (ESeq == ESeq.GRL_Dressing      || ESeq == ESeq.GRL_Dresser_Measure || 
                ESeq == ESeq.GRL_Wheel_Measure || ESeq == ESeq.GRL_Table_Grinding)
            {
                if(CIO.It.Get_Y(eY.GRDL_TbVacuum))
                {
                    CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                    return;
                }
                if (CDataOption.Package == ePkg.Unit)
                {// Hisilicon 일 경우 //20200229 LCY
                    if (CIO.It.Get_Y(eY.GRDR_Unit_1_Vacuum) ||
                        CIO.It.Get_Y(eY.GRDR_Unit_2_Vacuum) ||
                        CIO.It.Get_Y(eY.GRDR_Unit_3_Vacuum) ||
                        CIO.It.Get_Y(eY.GRDR_Unit_4_Vacuum))
                    {
                        CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                        return;
                    }
                }
            }

            if (ESeq == ESeq.GRL_Grinding && CData.Opt.aTblSkip[0])
            {
                CMsg.Show(eMsg.Error, "Error", "Left Table Skip Now!");
                return;
            }
            if(ESeq == ESeq.GRR_Grinding && CData.Opt.aTblSkip[1])
            {
                CMsg.Show(eMsg.Error, "Error", "Right Table Skip Now!");
                return;
            }

            if(ESeq == ESeq.GRR_Dressing         || ESeq == ESeq.GRR_Grinding        || ESeq == ESeq.GRR_WaterKnife    ||
               ESeq == ESeq.GRR_Table_Measure    || ESeq == ESeq.GRR_Strip_Measure   || ESeq == ESeq.GRR_Water_Knife   ||
               ESeq == ESeq.GRR_Strip_Measureone || ESeq == ESeq.GRR_Dresser_Measure || ESeq == ESeq.GRR_Wheel_Measure ||
               ESeq == ESeq.GRR_Table_Grinding   ||
               ESeq == ESeq.GRL_Dressing         || ESeq == ESeq.GRL_Grinding        || ESeq == ESeq.GRL_WaterKnife    ||
               ESeq == ESeq.GRL_Table_Measure    || ESeq == ESeq.GRL_Strip_Measure   || ESeq == ESeq.GRL_Water_Knife   ||
               ESeq == ESeq.GRL_Strip_Measureone || ESeq == ESeq.GRL_Dresser_Measure || ESeq == ESeq.GRL_Wheel_Measure ||
               ESeq == ESeq.GRL_Table_Grinding   )
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "One More Check Strip Jig. Please") == DialogResult.Cancel) 
                return;

                if(!CData.Opt.bCoverSkip && !CSQ_Main.It.CheckCover())
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Grinding Cover Check Please");
                }
            }
            //190211 ksg :
            //20190604 ghk_onpbcr
            //if(ESeq == ESeq.INR_CheckBcr || ESeq == ESeq.INR_CheckOri)
            if (ESeq == ESeq.ONP_CheckBcr || ESeq == ESeq.ONP_CheckOri)
            {
                CBcr.It.LoadRecipe();
                if(CData.Bcr.sDX != CData.Bcr.sBCR) 
                {
                    CMsg.Show(eMsg.Error, "Error", "Different the device & BCR Recipe");
                    return;
                }
            }

            if(CDataOption.UseQC && CData.Opt.bQcUse && ESeq == ESeq.DRY_Out)
            {
                //CTcpIp.It.SendStripOut();
                //#2 ReadyQuery
                CGxSocket.It.SendMessage("EQReadyQuery");
                //TimeOut Set

                _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
            }
            //ksg 추가
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                return;
            }
            else 
            { 
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC) : 측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과(위험) 여부 체크
                if (!_checkMeasurePointCount(ESeq))
                { return; }

                //CData.L_GRD.m_tStat.bDrsR = true;
                CSQ_Man.It.Seq            = ESeq;
                //190410 ksg :
                //ShowHideBtn(false);
                //CSQ_Man.It.bBtnShow = false;

                _SetLog("Tenkey start : " + ESeq.ToString());
            }
        }

        // syc : All Water On/Off ,기존 All Water On/Off 통합
        // <summary>
        /// All Water On/Off 
        /// True  : Water on
        /// False : Water off
        /// </summary>
        public void WaterOnOff(bool bOn)
        {
            if (!CIO.It.Get_Y(eY.GRDL_TbFlow       ) == bOn) CIO.It.Set_Y(eY.GRDL_TbFlow       , bOn); //Talbe 
            if (!CIO.It.Get_Y(eY.GRDL_TopClnFlow   ) == bOn) CIO.It.Set_Y(eY.GRDL_TopClnFlow   , bOn); //Grind Left Top Cleaner
            if (!CIO.It.Get_Y(eY.GRDL_SplWater     ) == bOn) CIO.It.Set_Y(eY.GRDL_SplWater     , bOn); //Grind Left Spindle Water
            if (!CIO.It.Get_Y(eY.GRDL_SplBtmWater  ) == bOn) CIO.It.Set_Y(eY.GRDL_SplBtmWater  , bOn); //Grind Left Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가                  
            if (!CIO.It.Get_Y(eY.GRDL_WhlCleaner   ) == bOn) CIO.It.Set_Y(eY.GRDL_WhlCleaner   , bOn); //Grind Left Wheel Cleaner Water
            if (!CIO.It.Get_Y(eY.GRDL_TopWaterKnife) == bOn) CIO.It.Set_Y(eY.GRDL_TopWaterKnife, bOn);
                                                                                  
            if (!CIO.It.Get_Y(eY.GRDR_TbFlow       ) == bOn) CIO.It.Set_Y(eY.GRDR_TbFlow       , bOn); //Talbe 
            if (!CIO.It.Get_Y(eY.GRDR_TopClnFlow   ) == bOn) CIO.It.Set_Y(eY.GRDR_TopClnFlow   , bOn); //Grind Right Top Cleaner
            if (!CIO.It.Get_Y(eY.GRDR_SplWater     ) == bOn) CIO.It.Set_Y(eY.GRDR_SplWater     , bOn); //Grind Right Spindle Water
            if (!CIO.It.Get_Y(eY.GRDR_SplBtmWater  ) == bOn) CIO.It.Set_Y(eY.GRDR_SplBtmWater  , bOn); //Grind Right Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가                                     
            if (!CIO.It.Get_Y(eY.GRDR_WhlCleaner   ) == bOn) CIO.It.Set_Y(eY.GRDR_WhlCleaner   , bOn); //Grind Left Wheel Cleaner Water
            if (!CIO.It.Get_Y(eY.GRDR_TopWaterKnife) == bOn) CIO.It.Set_Y(eY.GRDR_TopWaterKnife, bOn);
            if (!CIO.It.Get_Y(eY.DRY_ClnBtmFlow    ) == bOn) CIO.It.Set_Y(eY.DRY_ClnBtmFlow,     bOn);//Y6F Btm clean Water  
        }



        // 2020.10.28 JSKim St
        // <summary>
        /// WarmUp 중 Water 가 중단될때 모든 Water 끄기
        /// </summary>
        public void StartWater()
        {
            if (CIO.It.Get_Y(eY.GRDL_TbFlow)        == false) CIO.It.Set_Y(eY.GRDL_TbFlow,          true); //Talbe 
            if (CIO.It.Get_Y(eY.GRDL_TopClnFlow)    == false) CIO.It.Set_Y(eY.GRDL_TopClnFlow,      true); //Grind Left Top Cleaner
            if (CIO.It.Get_Y(eY.GRDL_SplWater)      == false) CIO.It.Set_Y(eY.GRDL_SplWater,        true); //Grind Left Spindle Water
            if (CIO.It.Get_Y(eY.GRDL_SplBtmWater)   == false) CIO.It.Set_Y(eY.GRDL_SplBtmWater,     true); //Grind Left Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가
            if (CIO.It.Get_Y(eY.GRDL_WhlCleaner)    == false) CIO.It.Set_Y(eY.GRDL_WhlCleaner,      true); //Grind Left Wheel Cleaner Water
            if (CIO.It.Get_Y(eY.GRDL_TopWaterKnife) == false) CIO.It.Set_Y(eY.GRDL_TopWaterKnife,   true);

            if (CIO.It.Get_Y(eY.GRDR_TbFlow)        == false) CIO.It.Set_Y(eY.GRDR_TbFlow,          true); //Talbe 
            if (CIO.It.Get_Y(eY.GRDR_TopClnFlow)    == false) CIO.It.Set_Y(eY.GRDR_TopClnFlow,      true); //Grind Right Top Cleaner
            if (CIO.It.Get_Y(eY.GRDR_SplWater)      == false) CIO.It.Set_Y(eY.GRDR_SplWater,        true); //Grind Right Spindle Water
            if (CIO.It.Get_Y(eY.GRDR_SplBtmWater)   == false) CIO.It.Set_Y(eY.GRDR_SplBtmWater,     true); //Grind Right Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가
            if (CIO.It.Get_Y(eY.GRDR_WhlCleaner)    == false) CIO.It.Set_Y(eY.GRDR_WhlCleaner,      true); //Grind Left Wheel Cleaner Water
            if (CIO.It.Get_Y(eY.GRDR_TopWaterKnife) == false) CIO.It.Set_Y(eY.GRDR_TopWaterKnife,   true);
            if (CIO.It.Get_Y(eY.DRY_ClnBtmFlow)     == false) CIO.It.Set_Y(eY.DRY_ClnBtmFlow,       true); //Y6F Btm clean Water  
            
            // 2021.04.16 lhs Start
            if (CDataOption.UseDryWtNozzle)   // 2022.06.30 lhs : UseOffP_IOIV에서도 사용되는지 확인 필요
            {
                if (CIO.It.Get_Y(eY.DRY_BtmAir)     == false) CIO.It.Set_Y(eY.DRY_BtmAir,           true); //Dryer Water Nozzle 
            }
            // 2021.04.16 lhs End
        }
        // 2020.10.28 JSKim Ed

        // <summary>
        /// WarmUp 중 Water 가 중단될때 모든 Water 끄기
        /// </summary>
        //190904 ksg :
        public void StopWater()
        {
            if (CIO.It.Get_Y(eY.GRDL_TbFlow     )) CIO.It.Set_Y(eY.GRDL_TbFlow     , false); //Talbe 
            if (CIO.It.Get_Y(eY.GRDL_TopClnFlow )) CIO.It.Set_Y(eY.GRDL_TopClnFlow , false); //Grind Left Top Cleaner
            if (CIO.It.Get_Y(eY.GRDL_SplWater   )) CIO.It.Set_Y(eY.GRDL_SplWater   , false); //Grind Left Spindle Water
            if (CIO.It.Get_Y(eY.GRDL_SplBtmWater)) CIO.It.Set_Y(eY.GRDL_SplBtmWater, false); //Grind Left Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가
            if (CIO.It.Get_Y(eY.GRDL_WhlCleaner   )) CIO.It.Set_Y(eY.GRDL_WhlCleaner   , false); //Grind Left Wheel Cleaner Water
            if (CIO.It.Get_Y(eY.GRDL_TopWaterKnife)) CIO.It.Set_Y(eY.GRDL_TopWaterKnife, false);

            if (CIO.It.Get_Y(eY.GRDR_TbFlow     )) CIO.It.Set_Y(eY.GRDR_TbFlow     , false); //Talbe 
            if (CIO.It.Get_Y(eY.GRDR_TopClnFlow )) CIO.It.Set_Y(eY.GRDR_TopClnFlow , false); //Grind Right Top Cleaner
            if (CIO.It.Get_Y(eY.GRDR_SplWater   )) CIO.It.Set_Y(eY.GRDR_SplWater   , false); //Grind Right Spindle Water
            if (CIO.It.Get_Y(eY.GRDR_SplBtmWater)) CIO.It.Set_Y(eY.GRDR_SplBtmWater, false); //Grind Right Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가
            if (CIO.It.Get_Y(eY.GRDR_WhlCleaner   )) CIO.It.Set_Y(eY.GRDR_WhlCleaner   , false); //Grind Left Wheel Cleaner Water
            if (CIO.It.Get_Y(eY.GRDR_TopWaterKnife)) CIO.It.Set_Y(eY.GRDR_TopWaterKnife, false);
            //190430 ksg :
            if (CIO.It.Get_Y(eY.DRY_ClnBtmFlow)) CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water  
        }

        // <summary>
        /// All Home 중 LightCurtain 감지 시 Home 동작 중지 및 이미 되어 있는 홈도 Clear 함
        /// </summary>
        //200217 ksg :
        public void HomeDoneReset()
        {
            CSq_OnL.It   .m_bHD = false;
            CSq_OfL.It   .m_bHD = false;
            CSq_OnP.It   .m_bHD = false;
            CSq_OfP.It   .m_bHD = false;
            CSq_Inr.It   .m_bHD = false;
            CData  .L_GRD.m_bHD = false;
            CData  .R_GRD.m_bHD = false;
            CSq_Dry.It   .m_bHD = false;
        }

        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        private bool _checkMeasurePointCount(ESeq seq)
        {
            //Manual Grinding, Manual Measure Strip 시 18 Point 진행 여부 Query
            if (CDataOption.Use18PointMeasure && (0 < CData.Dev.i18PStripCount))
            {
                if (seq == ESeq.GRL_Grinding || seq == ESeq.GRL_Strip_Measure || seq == ESeq.GRR_Grinding || seq == ESeq.GRR_Strip_Measure)
                {
                    if (CMsg.Show(eMsg.Query, "Strip Measure Type", "Is This Test Strip(18 Points) Measure?") == DialogResult.OK)
                    {
                        if (seq == ESeq.GRL_Grinding || seq == ESeq.GRL_Strip_Measure)
                        { CData.L_GRD.m_bManual18PMeasure = true; }
                        else
                        { CData.R_GRD.m_bManual18PMeasure = true; }
                    }
                    else
                    {
                        if (seq == ESeq.GRL_Grinding || seq == ESeq.GRL_Strip_Measure)
                        { CData.L_GRD.m_bManual18PMeasure = false; }
                        else
                        { CData.R_GRD.m_bManual18PMeasure = false; }
                    }
                }
                else
                {
                    CData.L_GRD.m_bManual18PMeasure = false;
                    CData.R_GRD.m_bManual18PMeasure = false;
                }
            }
            else
            {
                CData.L_GRD.m_bManual18PMeasure = false;
                CData.R_GRD.m_bManual18PMeasure = false;
            }

            string strResult = "";
            if (seq == ESeq.GRL_Grinding || seq == ESeq.GRL_Strip_Measure)
            {
                strResult = CData.L_GRD.CheckMeasurePointCount(true); //스트립 측정 포인트 수 체크 : 매뉴얼 동작에서는 반드시 CData.L_GRD.m_bManual18PMeasure 값 설정 후 호출해야 정확한 측정 포인트 수가 파악됨
            }
            else if (seq == ESeq.GRR_Grinding || seq == ESeq.GRR_Strip_Measure)
            {
                strResult = CData.R_GRD.CheckMeasurePointCount(true); //스트립 측정 포인트 수 체크 : 매뉴얼 동작에서는 반드시 CData.R_GRD.m_bManual18PMeasure 값 설정 후 호출해야 정확한 측정 포인트 수가 파악됨
            }
            if (0 < strResult.Length)
            {
                CMsg.Show(eMsg.Error, "Error", strResult); //측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과(위험)
                return false;
            }

            return true;
        }
        public bool SetUpStripLotEnd()
        {//210309 pjh : Set up Strip 완료 후 Lot End
            if (!CSQ_Main.It.bSetUpStripStatus && !CSQ_Main.It.m_bAutoLoadingStop)
            {
                return false;
            }
            
            if(CSq_OnL.It.Step == 0)    { CSq_OnL.It.Init_Cyl(); }
            
            CSq_OnL.It.Cyl_Place();
            
            if(!bOnMGZPlaceDone && OffloaderMGZPlaceDone()) { return false; }

            if (CData.Opt.bSecsUse && CData.GemForm != null)
            {
                CData.GemForm.OnLotEnded(CData.LotInfo.sLotName, CData.LotInfo.iTInCnt.ToString(), CData.LotInfo.iTOutCnt.ToString());
            }

            //20190703 ghk_dfserver
            //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip)
            if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip)
            {
                if (!CSQ_Main.It.m_bDfWorkEnd)
                {
                    if (!CData.dfInfo.bBusy)
                    {
                        CDf.It.SendLotEnd();
                        CSQ_Main.It.m_bDfWorkEnd = true;
                    }
                    return false;
                }
                else
                {
                    if (CDf.It.ReciveAck((int)ECMD.scLotEnd))
                    {
                        CSQ_Main.It.m_bDfWorkEnd = false;
                    }
                    else
                    { return false; }
                }
            }

            CSQ_Main.It.LotInfoClear(); // CSQ_Main
            CData.SpcInfo.sEndTime = DateTime.Now.ToString("HH:mm:ss");

            //20190624 josh
            //jsck secsgem
            //svid 999103 lot ended time
            if (CData.Opt.bSecsUse && CData.GemForm != null)
            { CData.GemForm.Set_SVID(Convert.ToInt32(SECSGEM.JSCK.eSVID.LOT_Ended_Time), DateTime.Now.ToString("yyyyMMddHHmmss")); }

            CData.LotInfo.bLotEnd = true;
            CData.bLotEndShow = true;
            CData.bLotEndMsgShow = true; //190521 ksg :
            CData.LotInfo.bLotOpen = false;
            CData.bSecLotOpen = false; //191030 ksg :

            if (CIO.It.Get_Y(eY.GRDL_TbFlow)) CData.L_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off
            if (CIO.It.Get_Y(eY.GRDR_TbFlow)) CData.R_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off

            // 2021.09.14 lhs Start : SCK/JSCK는 별도로 Off 로직 구현
            //if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET)
            //{
            //    if (CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, false);  // SCK, JSCK, JCET
            //}
            if (CData.CurCompany == ECompany.JCET)
            {
                if (CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, false);  // JCET
            }
            // 2021.09.14 lhs End

            //2021-01-?? YYY, For New QC-Vision LOT End
            if (CDataOption.UseQC && CData.Opt.bQcUse)
            {
                if (CGxSocket.It.IsConnected())
                {
                    CGxSocket.It.SendMessage("LotEnd");

                    _SetLog("[SEND](EQ->QC) LotEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                }
            }

            // 2020.09.16 SungTae : Program 실행 중 Left & Right Probe Ejector Off
            //if (CData.CurCompany == ECompany.Qorvo/* || CData.CurCompany == ECompany.Qorvo_DZ*/)
            if (CDataOption.IsPrbEjector) // 200924 jym : 라이센스 옵션으로 변경
            {
                if (CIO.It.Get_Y(eY.GRDL_ProbeEjector)) CIO.It.Set_Y(eY.GRDL_ProbeEjector, false);
                if (CIO.It.Get_Y(eY.GRDR_ProbeEjector)) CIO.It.Set_Y(eY.GRDR_ProbeEjector, false);
            }
            CSQ_Main.It.m_iLoadingCount = 0;
            //210806 pjh : Set Up Strip 이후 Lot End 시 변수 초기화
            CSQ_Main.It.bSetUpStripStatus = false;
            bOnMGZPlaceDone = false;
            bOffMGZBtmPlaceDone = false;
            bOffMGZTopPlaceDone = false;
            //
            CSq_OfL.It.bChkAutoLoadStopEnd = false;//210309 pjh : Set up strip
            return true;
        }
        //
        public bool OffloaderMGZPlaceDone()
        {//210309 pjh : Offloader Magazine Place 확인 구문
            bool bRet = false;
            if (CSQ_Main.It.bSetUpStripStatus)
            {
                if (CDataOption.UseQC && CData.Opt.bQcUse)
                { bRet = bOffMGZBtmPlaceDone && bOffMGZTopPlaceDone; }
                else if (CData.Opt.eEmitMgz == EMgzWay.Top)
                { bRet = bOffMGZTopPlaceDone; }
                else
                { bRet = bOffMGZBtmPlaceDone; }
            }

            return bRet;
        }
        public void SetUpStripManOn()
        {//210706 pjh : 고객사 요청사항. Set up strip 기능 중 Message가 표시 됐을 때 Warm up 하게끔 요청 > 협의 후 Table 및 Spindle에 Water on 하는 기능으로 대체

            CMot.It.Mv_N((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Able[0]);
            CMot.It.Mv_N((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Able[1]);

            if ((CMot.It.Get_FP((int)EAx.LeftGrindZone_Z) != CData.SPos.dGRD_Z_Able[0]) || (CMot.It.Get_FP((int)EAx.RightGrindZone_Z) != CData.SPos.dGRD_Z_Able[1])) return;
            else
            {
                if (CIO.It.Get_Y(eY.GRDL_TbFlow) == false) CIO.It.Set_Y(eY.GRDL_TbFlow, true); //Talbe             
                if (CIO.It.Get_Y(eY.GRDL_SplWater) == false) CIO.It.Set_Y(eY.GRDL_SplWater, true); //Grind Left Spindle Water            

                if (CIO.It.Get_Y(eY.GRDR_TbFlow) == false) CIO.It.Set_Y(eY.GRDR_TbFlow, true); //Talbe             
                if (CIO.It.Get_Y(eY.GRDR_SplWater) == false) CIO.It.Set_Y(eY.GRDR_SplWater, true); //Grind Right Spindle Water

                //CSpl.It.Write_Run(EWay.L, CData.Opt.iWmUS);
                // 2023.03.15 Max
                bool bExit = false;
                bool bTimeOutFlag = false;

                // RPM Setting
                CSpl_485.It.Write_Rpm(EWay.L, CData.Opt.iWmUS);
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

                //CSpl.It.Write_Run(EWay.R, CData.Opt.iWmUS);
                // 2023.03.15 Max
                bExit = false;
                bTimeOutFlag = false;
                // RPM Setting
                CSpl_485.It.Write_Rpm(EWay.R, CData.Opt.iWmUS);
                m_Timout_485.Set_Delay(300);
                do
                {
                    Application.DoEvents();
                    if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                    bExit = CSpl_485.It.GetAcceptRPMSpindle(EWay.R);
                } while (bExit != true);
                CSpl_485.It.SetAcceptRPMSpindle(EWay.R);

                if (bTimeOutFlag)
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
                    return;
                }

                // Spindle RUN
                bExit = false;
                bTimeOutFlag = false;
                CSpl_485.It.Write_Run(EWay.R);
                m_Timout_485.Set_Delay(300);
                do
                {
                    Application.DoEvents();
                    if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                    bExit = CSpl_485.It.GetAcceptRunSpindle(EWay.R);
                } while (bExit != true);
                CSpl_485.It.SetAcceptRunSpindle(EWay.R);

                if (bTimeOutFlag)
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_RUN_RPM_ERROR);
                    return;
                }

                

                CSQ_Main.It.DoorLock(true);
            }
        }

        public void SetUpStripManOff()
        {
			//210706 pjh : 고객사 요청사항. Set up strip 기능 중 Message가 표시 됐을 때 Warm up 하게끔 요청 > 협의 후 Table 및 Spindle에 Water on 하는 기능으로 대체
            if (CIO.It.Get_Y(eY.GRDL_TbFlow) == true) CIO.It.Set_Y(eY.GRDL_TbFlow, false); //Talbe 
            if (CIO.It.Get_Y(eY.GRDL_SplWater) == true) CIO.It.Set_Y(eY.GRDL_SplWater, false); //Grind Left Spindle Water            

            if (CIO.It.Get_Y(eY.GRDR_TbFlow) == true) CIO.It.Set_Y(eY.GRDR_TbFlow, false); //Talbe 
            if (CIO.It.Get_Y(eY.GRDR_SplWater) == true) CIO.It.Set_Y(eY.GRDR_SplWater, false); //Grind Right Spindle Water

            //CSpl.It.Write_Stop(EWay.L);
            //CSpl.It.Write_Stop(EWay.R);
            // 2023.03.15 Max
            CSpl_485.It.Write_Stop(EWay.L);
            CSpl_485.It.Write_Stop(EWay.R);

            CSQ_Main.It.DoorLock(false);
        }

        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);

            CLog.Save_Log(eLog.None, eLog.MR, string.Format("[{0}()]\tSt : {1}\t{2}", sMth, m_iStep.ToString("00"), sMsg));
        }

        private void _SetLog(string sMsg, double dPos)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);
            sMsg += "  Pos : " + dPos + "mm";

            CLog.Save_Log(eLog.None, eLog.MR, string.Format("[{0}()]\tSt : {1}\t{2}", sMth, m_iStep.ToString("00"), sMsg));
        }
    }
}
