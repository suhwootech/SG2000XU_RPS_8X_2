using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public class CSQ_Main : CStn<CSQ_Main>
    {
        public EStatus m_iStat = EStatus.Idle;
        private eAutoStatus eAutoStep; 
        public eAutoStatus EAutoStep { get { return eAutoStep; } }
        public  bool m_bToStop     = false;
        public  bool m_bBtStart    = false;
        public  bool m_bBtStop     = false;
        public  bool m_bBtReset    = false;
        public  bool m_bRun        = false;
        public  bool m_bBuzSkip    = false;
        public  bool m_bPause      = false; //190506 ksg :
        public  bool m_bDfWorkEnd  = false;

        // 2020-11-20, jhLee : UI 상의 동작 버튼을 확실히 적용 시키기 위한 flag
        public bool m_bPushStart = false;
        public bool m_bPushStop = false;
        public bool m_bPushReset = false;

        private bool m_bWarningShow  = false;
        // public  bool m_bDspLeftArea  = false; //koo : pause  // 2021-02-05, jhLee : 각각 OnLoader / OffLoader 쪽으로 이전
        // public  bool m_bDspRightArea = false; //koo : pause
        public  bool m_bWhlLimitArm  = false; //190711 ksg :
        public  bool m_bDrsLimitArm  = false; //190714 ksg :
        public  bool m_bPCWOnOff     = false; //190716 ksg :
        public  bool m_bPCWOffOn     = false; //190716 ksg :
        public  bool m_bPCWCheck     = false; //190716 ksg :
        // 2020.10.20 JSKim St
        public bool m_bFirstLock     = false;   // QC Vision Door Lock 상태 동기화
        public bool m_bCheckLock     = false;
        // 2020.10.20 JSKim Ed

        private CTim m_Delay         = new CTim(); //190406 ksg : QC
        private CTim m_Delay1        = new CTim(); //190406 ksg : Pump Check
        private CTim m_DelayQc       = new CTim(); //190406 ksg : QC Status
        private CTim m_OnLAreaTiOut  = new CTim(); //koo : pause
        private CTim m_OffLAreaTiOut = new CTim(); //koo : pause
        private CTim m_DelayPCW      = new CTim(); //190716 ksg : PCW Check Delay Time
        public CTim m_SendQCDelay = new CTim();     // 2021-02-02, jhLee, for QC Sync
        public CTim m_StatusQCDelay = new CTim();   // 2021-02-02, jhLee, for QC Sync
        public CTim m_DelayLeak     = new CTim();     // 2021.03.05 SungTae : Leak Sensor Check
        public CTim m_DelayDryLeak  = new CTim();     // 2021.03.05 SungTae : Dry Leak Sensor Check

        // 2020-10-19, jhLee : HOST로부터 S2F41를 통해 RCMD 'START'를 수신하여 AUTO 전환을 위한 변수들
        public int m_nRCMDRecv      = 0;            //  HOST에서 RCMD를 받았는가 ? 0:미수신, 1:AUTO 수신, -1:AUTO외의 잘못된 응답
        public int m_nRCMDStartSeq  = 0;            //  HOST의 RCMD를 통해 설비를 AUTO로 전환하는 시퀀스
        public CTim m_RCMDTimeout   = new CTim();   //  HOST에서 RCMD를 보내오지않는 시간 측정하여 Timeout 처리용

        // 2020-11-17, jhLee : Auto Loading Stop 기능
        public bool m_bAutoLoadingStop = false;     // 자동으로 투입 중지를 활성한 상태인가 ?
        public int m_iLoadingCount = 0;             // 자동 투입중지 모드를 설정 한 뒤 투입된 제품 수량
        //end of jhLee

        /// <summary>
        /// 210309 pjh : Set up Strip 작업 진행 중 Main Status 명칭 변경
        /// </summary>
        public bool bSetUpStripStatus = false;
        public bool bSetUpStripMsg    = false;
        public bool bSetUpStripLotEnd = false; //210806 pjh : Set Up Strip Lot End 변수
        //

        /// <summary>
        /// 220526 pjh : Qorvo DZ csv error 발생 시 Buzzer 울리도록 하는 변수
        /// </summary>
        public bool isSPC = false;

        private CSQ_Main()
        {

        }

        private bool ToReady()
        {
            bool bRst1, bRst2, bRst3, bRst4, bRst5, bRst6, bRst7, bRst8;

            bRst1 = CSq_OnL.It.ToReady();
            bRst2 = CSq_Inr.It.ToReady();
            bRst3 = CSq_OnP.It.ToReady();
            bRst4 = CData.L_GRD.ToReady();
            bRst5 = CData.R_GRD.ToReady();
            bRst6 = CSq_OfP.It.ToReady();
            bRst7 = CSq_Dry.It.ToReady();
            bRst8 = CSq_OfL.It.ToReady();
        
            return (bRst1 && bRst2 && bRst3 && bRst4 && bRst5 && bRst6 && bRst7 && bRst8);
        }

        private bool AutoRunCly()
        {
            bool bRst1, bRst2, bRst3, bRst4, bRst5, bRst6, bRst7, bRst8;
            bRst1 = CSq_OnL.It   .AutoRun();
            bRst2 = CSq_Inr.It   .AutoRun();
            bRst3 = CSq_OnP.It   .AutoRun();
            bRst4 = CData  .L_GRD.AutoRun();
            bRst5 = CData  .R_GRD.AutoRun();
            bRst6 = CSq_OfP.It   .AutoRun();
            bRst7 = CSq_Dry.It   .AutoRun();
            bRst8 = CSq_OfL.It   .AutoRun();

            return ((bRst1 && bRst2 && bRst3 && bRst4 && bRst5 && bRst6 && bRst7 && bRst8));
            //return ((bRst1 && bRst2 && bRst3 && bRst4 && bRst5 && bRst6 && bRst7 && bRst8) || (CDataOption.UseSetUpStrip && CSQ_Man.It.bOnMGZPlaceDone && CSQ_Man.It.OffloaderMGZPlaceDone()));
        }

        private bool ToStop()
        {
            bool bRst1, bRst2, bRst3, bRst4, bRst5, bRst6, bRst7, bRst8;
            bRst1 = CSq_OnL.It.ToStop();
            bRst2 = CSq_Inr.It.ToStop();
            bRst3 = CSq_OnP.It.ToStop();
            bRst4 = CData.L_GRD.ToStop();
            bRst5 = CData.R_GRD.ToStop();
            bRst6 = CSq_OfP.It.ToStop();
            bRst7 = CSq_Dry.It.ToStop();
            bRst8 = CSq_OfL.It.ToStop();
        
            return (bRst1 && bRst2 && bRst3 && bRst4 && bRst5 && bRst6 && bRst7 && bRst8);
        }

        private bool Stop()
        {
            bool bRst1, bRst2, bRst3, bRst4, bRst5, bRst6, bRst7, bRst8;
            bRst1 = CSq_OnL.It.Stop();
            bRst2 = CSq_Inr.It.Stop();
            bRst3 = CSq_OnP.It.Stop();
            bRst4 = CData.L_GRD.Stop();
            bRst5 = CData.R_GRD.Stop();
            bRst6 = CSq_OfP.It.Stop();
            bRst7 = CSq_Dry.It.Stop();
            bRst8 = CSq_OfL.It.Stop();
        
            return (bRst1 && bRst2 && bRst3 && bRst4 && bRst5 && bRst6 && bRst7 && bRst8);
        }

        private void Reset()
        {
            if(m_bRun) return;

            //if(m_iStat == eStatus.Auto_Running || CSQ_Man.It.Seq == ESeq.All_Home) return;
            if(m_iStat == EStatus.Auto_Running || CSQ_Man.It.Seq == ESeq.All_Home || m_iStat == EStatus.Manual) return; //200227 ksg :

            // QC Vision에게 Reset 신호를 보내준다.
            CGxSocket.It.SendMessage("ErrorReset"); //20210122 RESET message

            _SetLog("[SEND](EQ->QC) ErrorReset");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

            //혹여나 TimeOut이 변경 되면 여기에 추가 할 것.
            //Time Out Clear 추가 부분

            if (CData.LotInfo.bLotEnd)
            {
                CData.LotInfo.bLotEnd = false;
                //190320 ksg : Lot End 시 Lot End Msg 창 클릭 전에 Reset 먼저 누르면 문제되어서 변경 함.
                //if(CData.bLotEndShow) CData.bLotEndShow = false;

                if (CData.CurCompany == ECompany.JCET)
                {
                    CData.bEraseLotName = true; 
                }
            }

            //Servo Reset
            for (int i = CMot.It.AxS; i <= CMot.It.AxL; i++)
            {
                if(CMot.It.Set_Clr(i) != 0)
                {
                    CMsg.Show(eMsg.Error, "Error", string.Format("{0} Servo Alram Not Clear", i));
                }
            }
            //190522 ksg : spindle Reset
           // CSpl.It.Write_Reset(EWay.L);
           // CSpl.It.Write_Reset(EWay.R);
            // 2023.03.15 Max
            CSpl_485.It.Write_Reset(EWay.L);
            CSpl_485.It.Write_Reset(EWay.R);

            //GU.Delay(100);
            if (CDataOption.SplType == eSpindleType.EtherCat)
            {
                CMot.It.Set_SOn((int)EAx.LeftSpindle, true);
                CMot.It.Set_SOn((int)EAx.RightSpindle, true);
            }
            //GU.Delay(100);
            CData.L_GRD.Func_PrbUp();
            CData.R_GRD.Func_PrbUp();

            m_iStat = EStatus.Stop;

            //All Servo On
            //CSQ_Man.It.Seq = ESeq.All_Servo_On;
            //Error시 Error Clear 추가 하고 시도 할 것.            
            //기타 리셋시 동작 할 것들...
            if(CSQ_Man.It.Seq != ESeq.Idle)
            {
                //CSQ_Man.It.Seq = ESeq.Idle;
            }

            // 2020.12.09 JSKim St
#if true //210115 jhc : error 반복 처리 중 발생 오류(hang-up) 에 대한 임시 디버깅
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
			{
				CErr.SaveErrLogLock(eErr.NONE);
			}
			else
			{
				CErr.SaveErrRelease();
			}
#else
            // CErr.SaveErrRelease();
            CErr.SaveErrLogLock(eErr.NONE);
#endif //..
			// 2020.12.09 JSKim Ed

			// 2020.12.10 JSKim St
			if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.LotInfo.bLotOpen == true)
                {
                    CErr.SaveErrLog_LotLock(eErr.NONE);
                }
            }
            // 2020.12.10 JSKim Ed

            m_bBuzSkip = false;
            //190514 ksg :
            CSQ_Man.It.bBtnShow = true;
            //190806 ksg:

        }

        public void AutoRun()
        {
            //200624 pjh : Chk PCW
            CData.L_GRD.Chk_PCW();
            CData.R_GRD.Chk_PCW();
            //조건
            bool xbCheckPump   = CheckPump();//190522
            bool bGetError     = false;
            bool xbStarIO      =   CIO.It.Get_X(eX.SYS_BtnStart  );
            bool xbStopIO      =   CIO.It.Get_X(eX.SYS_BtnStop   );
            bool xbResetIO     =   CIO.It.Get_X(eX.SYS_BtnReset  );
            bool xbWheelLZig   = (!CIO.It.Get_X(eX.GRDL_WheelZig));// || (!CIO.It.Get_X(eX.GRDR_WheelZig));
            bool xbWheelRZig   = (!CIO.It.Get_X(eX.GRDR_WheelZig));// || (!CIO.It.Get_X(eX.GRDR_WheelZig));
            bool xbWheelZig    = xbWheelLZig || xbWheelRZig;
            bool xbAllDoorChk  = CheckDoor       ();
            bool xbCoverChk    = CheckCover      (); //190319 ksg :
            bool xbLeakSensor  = CheckLeakSensor (); //190109 ksg :
            bool xbDryLeak     = ChkDryLeakSensor(); //190417 ksg :
            bool xbCheckWarmUp = CheckWarmUp     ();
            bool xbCheckEmg    = CheckEmergency  ();
            bool CheckQcRun    = false             ; //190406 ksg : Qc
            bool bAutoRun                          ;
            bool bChkWhlCom    = CheckWheelChange(); //190522 ksg :
            int  iOverwheel    = CheckWeelLimit  (); //190711 ksg :
            int  iOverDresser  = ChkDrsLimit     (); //190714 ksg :
            bool bAllHomeDone  = ChkAllHomeDone  (); //190714 ksg : All Home
            int  nSECSGEM_Ck   = Chk_SECSGEM_Par();  //200131 LCY : SCK SECSGEM 및 
            
            bool bStartSW = m_bBtStart || xbStarIO || m_bPushStart;    
            bool bStopSW  = m_bBtStop  || xbStopIO || m_bPushStop;
            bool bResetSW = m_bBtReset || xbResetIO || m_bPushReset;

            bool bStopBtnFlick = (eAutoStep == eAutoStatus.GoStop || eAutoStep == eAutoStatus.Stop) && CTwr.It.m_bFlick;

            // 2020.10.22 JSKim St
            if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
            {
                bool xbCheckAddPump = CheckAddPump();
            }
            // 2020.10.22 JSKim Ed

            //20191121 ghk_display_strip
            if (CDataOption.DisplayStrip == eDisplayStrip.Use)
            {
                Chk_Match_ONL();
                Chk_Match_INR();
                Chk_Match_ONP();
                Chk_Match_GRDL();
                Chk_Match_GRDR();
                Chk_Match_OFP();
                Chk_Match_DRY();
                Chk_Match_OFL_BTM();
                Chk_Match_OFL_TOP();
            }

            if (m_iStat == EStatus.Error)
            {
                bGetError = true;
            }

            CIO.It.Set_Y(eY.SYS_BtnStart, xbStarIO  ||   m_bRun );
            CIO.It.Set_Y(eY.SYS_BtnStop , xbStopIO  ||  !m_bRun || bStopBtnFlick );
            CIO.It.Set_Y(eY.SYS_BtnReset, xbResetIO || (CTwr.It.m_bFlick && bGetError) );

            //20190930 ghk_autowarmup
            if(CDataOption.AutoWarmUp == eAutoWarmUp.Use)
            { 
                Chk_AutoWarmUp(); 
            }

            //Manual Belt Run 
            BeltRun();

            //
            // 2021-02-05, jhLee : Skyworks에서 무언정지 발생, Alarm이 발생되지 않고 Autorun 상태에서 Stop 상태로 전이되지 않고
            //                      m_bReqStop만 true로 되면서 Autorun cycle은 수행되지 못하며 계속 Autorun 상태로 유지되는 문제 발생 하여 
            //                      각각의 OnLoader/OffLoader로 이동하였다.          
            //
            //// 210106 myk : 모든 조건에서 에어리어 센서 감지 구문 타게 수정
            //CheckAreaSensor();//koo : pause
            //

            if (CDataOption.ManualLock == eManualDoorLock.Use)
            {
                //20190426 ghk 스카이웍스 요청 : 장비 구동중에 항상 도어 락
                if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual || m_iStat == EStatus.Warm_Up || m_iStat == EStatus.All_Homing)
                {
                    //20191104 ghk_doorlock
                    if (CSQ_Man.It.Seq != ESeq.All_Servo_On)
                    {
                        if (!xbAllDoorChk)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Door Close Please");
                            bStartSW = false;
                            return;
                        }
                        DoorLock(true);
                        CSQ_Man.It.m_bDoor = false;
                    }
                    CSQ_Man.It.bBtnShow = false;
                }
                else
                {//장비 구동 아닐때만 도어 풀림
                    if (!CSQ_Man.It.m_bDoor)
                    {
                        //20191204 ghk_doorlock
                        if (m_iStat == EStatus.Error)
                        {//에러 일 경우
                            // 200723 jym : Run 완료 상태 조건 추가
                            if (ToStop() && !m_bRun)
                            {//장비가 멈췄을때 도어 락 풀림 (에러가 발생 해도 그라인딩 중 멈추지 않아서 추가)
                                DoorLock(false);
                            }
                        }
                        else
                        {//에러가 아닐 경우
                            DoorLock(false);
                        }
                    }
                }
            }
            else 
            {
                // 2020.12.18 JSKim St
                //DoorLock(m_bRun && (m_iStat == EStatus.Auto_Running));
                if (CData.CurCompany == ECompany.ASE_K12 && m_iStat == EStatus.Manual)
                {
                    if (CSQ_Man.It.Seq == ESeq.GRL_Grinding      || CSQ_Man.It.Seq == ESeq.GRL_Manual_GrdBcr || 
                        CSQ_Man.It.Seq == ESeq.GRR_Grinding      || CSQ_Man.It.Seq == ESeq.GRR_Manual_GrdBcr ||
                        CSQ_Man.It.Seq == ESeq.GRD_Dual_Grinding || CSQ_Man.It.Seq == ESeq.GRD_Dual_Dressing )//220426 pjh : Dual Grinding/Dressing Door Lock
                    {
                        DoorLock(true);
                    }
                }
                else
                {
                    DoorLock(m_bRun && (m_iStat == EStatus.Auto_Running));
                }
                // 2020.12.18 JSKim Ed
            }
            // 190715-maeng
            // 상태 Idle일때 10분 경과 시 PCW off
            if(CDataOption.PcwAutoOff == ePcwAutoOff.Use)
            { 
                if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual    || 
                    m_iStat == EStatus.Warm_Up      || m_iStat == EStatus.All_Homing||
                    !CSQ_Man.It.bBtnShow                                               )
                {
                    // Idle stopwatch stop
                    CData.SwPCW.Stop();

                    // IO 상태 확인 후 PCW on
                    if (!CIO.It.Get_X(eX.GRDL_SplPCW)) { CIO.It.Set_Y(eY.GRDL_SplPCW, true); }
                    if (!CIO.It.Get_X(eX.GRDR_SplPCW)) { CIO.It.Set_Y(eY.GRDR_SplPCW, true); }

                    if (!m_bPCWOffOn)   { m_DelayPCW.Set_Delay(5000); }

                    m_bPCWOffOn = true ;
                    m_bPCWOnOff = false;
                }
                else
                {
                    // Stopwatch 상태 확인
                    if (!CData.SwPCW.IsRunning && CData.PcwTime != 0)
                    {
                        CData.SwPCW.Reset();
                        CData.SwPCW.Start();
                    }
                    else
                    {
                        // 지정된 시간과 경과시간 비교
                        //if (CData.SwPCW.Elapsed.TotalMinutes > CData.PcwTime && CData.PcwTime != 0)
                        if (CData.SwPCW.Elapsed.TotalMinutes > CData.PcwTime && CData.PcwTime != 0 && !CData.bWhlChangeMode)    // 2020.09.10 SungTae : Wheel Change 시 PCW Auto-Off 기능 보류
                        {   // 지정된 시간 이후 PCW off
                            CIO.It.Set_Y(eY.GRDL_SplPCW, false);
                            CIO.It.Set_Y(eY.GRDR_SplPCW, false);

                            m_bPCWOffOn = false;
                            m_bPCWOnOff = true ;
                            m_bPCWCheck = false;
                        }
                    }
                }
                //190716 ksg : PCW Check Delay
                if(m_bPCWOffOn && !m_bPCWOnOff && m_DelayPCW.Chk_Delay())
                {
                    m_bPCWCheck = true;
                }
            }

            //200121 ksg :
            if(CDataOption.PumpOff == ePumpAutoOff.Use)
            {
                ChkPumpAutoOff();
            }

           // 사용을 마친 버튼 Flag 초기화
            m_bBtStart = m_bBtStop = m_bBtReset = false;
 
            // 2021-02-04, jhLee : Main sequence에서 check가 아닌 각각 On Loader / Off loader 에서 AreaSensor 체크하는것으로 변경
            // 2021-02-04, jhLee : Skyworks에서 Ver3.1.2.25 이상의 버전에서 AutoRun 상태에서 무언정지 발생하여 Ver3.1.2.19 버전으로 원복함
            //
            ////190904 ksg : 수정
            //if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual || m_iStat == EStatus.Warm_Up || m_iStat == EStatus.All_Homing)
            //{
            //    CheckAreaSensor();//koo : pause
            //}

            //Wheel Zig 감지 및 잠금
            CData.Opt.bWhlJigSkip = false; //190213 ksg : Qorvo용

            //CData.Opt.bWhlJigSkip = true ; //190213 ksg : Ase Kr용(Jig 센서 변경 하기 전까지)
            if(xbWheelZig && CData.WMX && xbCheckEmg && !CData.Opt.bWhlJigSkip)
            {
                //20190421 ghk_휠 파라미터 선택
                if (xbWheelLZig)
                {
                    CData.DrData[0].bDrsR     = true; 
                    CData.WhlChgSeq.bLWhlJigCheck = true;
                    //20191028 ghk_auto_tool_offset
                    CData.DrData[0].bDrs = true;
                    //
                    //210812 pjh : 휠지그 감지 변수
                    CData.DrData[0].bWheelChange = true;
                    //
                }

                if (xbWheelRZig)
                {
                    CData.DrData[1].bDrsR     = true; 
                    CData.WhlChgSeq.bRWhlJigCheck = true;
                    //20191028 ghk_auto_tool_offset
                    CData.DrData[1].bDrs = true;
                    //210812 pjh : 휠지그 감지 변수
                    CData.DrData[1].bWheelChange = true;
                    //
                }

                if (!CData.WhlChgSeq.bStart)
                {
                    frmMain.bShowWhlView = true;
                }

                CData.bShowWhlJig = true;

                CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Wheel Zig 센서 감지 시 Wheel/Dresser Limit Alarm 보류 해제
            }
            else
            {
                CData.bShowWhlJig = false;
            }


            // 2021-10-26, jhLee, Skyworks VOC : Dressing 동기화
            //
            // Skyworks site only
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                // Manual로 Dressing 작업을 수행했는가 ? 
                if (CData.DrData[(int)EWay.L].bDrsPerformed || CData.DrData[(int)EWay.R].bDrsPerformed)
                {
                    // 만약에 한 쪽만 수동으로 Dressing 을 수행하였다면 다른 한 쪽에 대해 Dressing을 수행시키도록 한다.

                    // 왼쪽만 Dressing 수행
                    if (CData.DrData[(int)EWay.L].bDrsPerformed && !CData.DrData[(int)EWay.R].bDrsPerformed)
                    {
                        // 오른쪽 휠의 Dressing 을 하도록 한다.
                        CData.DrData[(int)EWay.R].bDrs = true;
                        _SetLog($"[Dressing Sync] : Set Right-Table Dressing Request");
                    }

                    // 오른쪽만 Dressing 수행
                    if (!CData.DrData[(int)EWay.L].bDrsPerformed && CData.DrData[(int)EWay.R].bDrsPerformed)
                    {
                        // 왼쪽 휠의 Dressing 을 하도록 한다.
                        CData.DrData[(int)EWay.L].bDrs = true;
                        _SetLog($"[Dressing Sync] : Set Left-Table Dressing Request");
                    }

                    // 그 외에 양쪽 모두 Dressing을 마친것이라면 별도의 추가 Dressing 요청은 없다.
                    // 사용을 마친 Manual Dressing 수행여부 Flag들은 초기화를 해준다.
                    CData.DrData[(int)EWay.L].bDrsPerformed = false;
                    CData.DrData[(int)EWay.R].bDrsPerformed = false;
                }
            }


            // 2020.09.16 SungTae : Program 실행 중 Left & Right Probe Ejector On
            //if (CDataOption.IsPrbEjector) // 200924 jym : 라이센스 옵션으로 변경
            if (CDataOption.IsPrbEjector && (CData.CurCompany != ECompany.Qorvo_DZ && CData.CurCompany != ECompany.Qorvo)) // 200924 jym : 라이센스 옵션으로 변경
            {
                if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual)
                {
                    if (!CIO.It.Get_Y(eY.GRDL_ProbeEjector)) CIO.It.Set_Y(eY.GRDL_ProbeEjector, true);
                    if (!CIO.It.Get_Y(eY.GRDR_ProbeEjector)) CIO.It.Set_Y(eY.GRDR_ProbeEjector, true);
                }
            }

            if(bStartSW)
            {
                //  2022.05.04 SungTae Start : [추가]
                if (CData.bChkManual)   CData.bChkManual = false;
                if (CData.bChkGrdM)     CData.bChkGrdM = false;
                //  2022.05.04 SungTae End

                //조건 검색
                //if(!InspectHomeEnd()      ) { MessageBox.Show("Manual All Home Please"); return;} //1. Home End Check
                //if(!CData.LotInfo.bLotOpen) { MessageBox.Show("Lot Open Please"       ); return;} //2. Lot Open
                //if(!xbAllDoorChk          ) { MessageBox.Show("Door close Pleas"      ); return;} //2. Door Open 

#if true //201121 jhc : [추가] ASE-Kr (SECS/GEM Local Mode)+(DF Bottom) 설정 상태에서 START 버튼 누르면 Alarm
                //           : Mold 두께(DF Bottom, 낮은 값) 기준 Recipe로 Target 그라인딩 시,
                //           :  => (SECS/GEM 비 사용) 또는 (SECS/GEM Local Mode 사용) 경우 Host에서 EQ로 정보 전달이 없기에 자재 파손 위험!!!
                int nBottomMoldSecsGemCondition = Chk_SECSGEM_DF_Bottom();
                if (nBottomMoldSecsGemCondition == (-1)) { CErr.Show(eErr.BTM_MOLD_SECSGEM_OFF_ERROR);      bStartSW = false; return; }
                if (nBottomMoldSecsGemCondition == (-2)) { CErr.Show(eErr.BTM_MOLD_SECSGEM_LOCAL_ERROR);    bStartSW = false; return; }
                //
#else
                //201014 jhc : ASE-Kr (SECS/GEM 비 사용)+(DF Bottom) 설정 상태에서 START 버튼 누르면 Alarm
                //(SECS/GEM 비 사용)+(DF Bottom) 설정 상태 => Mold 두께(DF Bottom, 낮은 값) 기준 Recipe로 Target 그라인딩하면 자재 파손 위험!!!
                if (false == Chk_SECSGEM_DF_Bottom())
                { CErr.Show(eErr.BTM_MOLD_SECSGEM_OFF_ERROR); bStartSW = false; return; }
                //
#endif

#if true       //LGH_D,220502,SKW MultiLot 내부 테스트 조건 구문 설정
                // 2021.08.23 SungTae : [TEMP] 디버깅 위해 임시로 주석 처리. 테스트 후 주석 해제 예정.
                /*
                 * true : Auto Run 진행 시
                 * false : Debugging으로 Dry Run 진행 시
                 */
                //200807 jhc : SECS/GEM 사용 시 BCR Skip 옵션이면 Error
                if (CData.Opt.bSecsUse && CData.Dev.bBcrSkip)
                {
                    CErr.Show(eErr.SECSGEM_USE_BCR_SKIP_OPTION_ERROR);
                    bStartSW = false;
                    return;
                }
#endif          //LGH_D,220502

                if (CDataOption.UseNewSckGrindProc)
                {
                    if (nSECSGEM_Ck == -1) { CMsg.Show(eMsg.Error, "Error", "SCK Company SECSGEM Use and DF Use but Top_Single_Side or Btm_Single_Side Not check, Pls Recipe Check ");      bStartSW = false; return; } //SECSGEM 사용시에는 DF 사용을 필히 하여야 함 20200131 LCY
                    if (nSECSGEM_Ck == -2) { CMsg.Show(eMsg.Error, "Error", "SCK Company SECSGEM Use and DF Not Use but Top_Double_Side or Btm_Double_Side Not check, Pls Recipe Check");   bStartSW = false; return; } // TopD, BtmD는 DF 사용 못함
                }
                else
                {
                    if (nSECSGEM_Ck == -1) { CMsg.Show(eMsg.Error, "Error", "SCK Company SECSGEM Use and DF Use but Topside Not check, Pls Recipe Check "); bStartSW = false; return; } //SECSGEM 사용시에는 DF 사용을 필히 하여야 함 20200131 LCY
                    if (nSECSGEM_Ck == -2) { CMsg.Show(eMsg.Error, "Error", "SCK Company SECSGEM Use and DF Not Use but Btmside Not check, Pls Recipe Check");  bStartSW = false; return; } //SECSGEM 사용시에는 DF 사용을 필히 하여야 함 20200131 LCY
                }
                if (nSECSGEM_Ck == -3) { CMsg.Show(eMsg.Error, "Error", "ASE Company SECSGEM Use and Need to Dress_ID, Pls Input Left Dress_ID");           bStartSW = false; return; } //SECSGEM 사용시 Left Dress ID 입력 필요 함 20200430 LCY
                if (nSECSGEM_Ck == -4) { CMsg.Show(eMsg.Error, "Error", "ASE Company SECSGEM Use and Need to Dress_ID, Pls Input Right Dress_ID");          bStartSW = false; return; } //SECSGEM 사용시 Right Dress ID 입력 필요 함 20200430 LCY
                if (nSECSGEM_Ck == -5) { CMsg.Show(eMsg.Error, "Error", "ASE Company SECSGEM Use and Need to Dress_ID, Pls Input Left Wheel_ID");           bStartSW = false; return; } //SECSGEM 사용시 Left Wheel ID 입력 필요 함 20200430 LCY
                if (nSECSGEM_Ck == -6) { CMsg.Show(eMsg.Error, "Error", "ASE Company SECSGEM Use and Need to Dress_ID, Pls Input Right Wheel_ID");          bStartSW = false; return; } //SECSGEM 사용시 Eight Wheel ID 입력 필요 함 20200430 LCY

                //20190703 ghk_stepmode_tbskip
                if (CData.Dev.bDual == eDual.Dual)
                {
                    if(CData.Opt.aTblSkip[(int)EWay.L] || CData.Opt.aTblSkip[(int)EWay.R])
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "If use step mode, must use the Left and Right table"); bStartSW = false; return;
                    }
                }
                // 200803 jym
                if (CData.CurCompany == ECompany.ASE_KR && GV.DEV_WHL_SYNC)
                {
                    if (CData.Dev.aData[(int)EWay.L].sWhl != CData.Whls[(int)EWay.L].sWhlName)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Device left wheel file difference.");   bStartSW = false;   return;
                    }
                    if (CData.Dev.aData[(int)EWay.R].sWhl != CData.Whls[(int)EWay.R].sWhlName)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Device right wheel file difference.");  bStartSW = false;   return;
                    }
                }

                // 2022.08.16 SungTae Start : [추가] (ASE-KR 개발건)
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    // TOP Device인 경우
                    if (CData.Dev.bDual == eDual.Normal && CData.Dev.aData[(int)EWay.L].dFinalTarget == 0.0)
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "The final target value is not set.\n\nPlease proceed after entering the final target value.");

                        bStartSW = false;
                        return;
                    }

                    // BTM Device인 경우
                    if (CData.Dev.bDual == eDual.Dual && CData.Dev.aData[(int)EWay.R].dFinalTarget == 0.0)
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "The final target value in the right table is not set.\n\nPlease proceed after entering the final target value.");

                        bStartSW = false;
                        return;
                    }
                }
                // 2022.08.16 SungTae End

                if (bChkWhlCom) 
                { 
                    if      (CData.WhlChgSeq.bStart) CMsg.Show(eMsg.Notice , "Warning", "Wheel change not complete. First finish Wheel Cahnge sequence.\n\r Try again Change Cancel -> Wheel Change"); 
                    else if (CData.DrsChgSeq.bStart) CMsg.Show(eMsg.Notice , "Warning", "Dresser change not complete. First finish Dresser Cahnge sequence \n\r Try again Change Cancel -> Dressing Board Change");
                    
                    bStartSW = false; 
                    return;
                } //1. Wheel & Dresser Change
                
                if(!InspectHomeEnd())
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Manual All Home Please");   bStartSW = false;   return;
                } //1. Home End Check

                //
                // 2021-05-27, jhLee : Multi-LOT 진행 조건이면 입력된 LOT 정보가 존재하는지 여부에 따라서 Auto-Run 수행 여부를 판단한다.
                //
                if (CData.IsMultiLOT())
                {
                    // 2021.08.18 SungTae Start : [수정] Off-line Mode와 On-line LOCAL Mode일 경우에만 Check하도록 조건 추가
                    if (!CData.Opt.bSecsUse || CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
                    {
                        // 입력된 LOT 정보가 존재하지 않는다면 Auto-Run을 수행할 수 없다.
                        if (CData.LotMgr.GetCount() <= 0)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "LOT Information Input Please");

                            bStartSW = false;
                            return;
                        }

                        // LOT 정보가 존재한다면 자동 수행 가능
                        CData.LotInfo.bLotOpen = true;                  // 공통사용 LOT 정보의 Lot Open 여부 변수는 항상 LOT가 Open 되었다고 지정
                    }
                    // 2021.08.18 SungTae End  
                }
                else // Multi-LOT을 사용하지 않는 경우, 기존 조건 적용                
                {
                    //if (CDataOption.SecsUse == eSecsGem.Use && CData.Opt.bSecsUse)
                    if (CDataOption.SecsUse == eSecsGem.Use && CData.Opt.bSecsUse
                        && CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Remote)   // 2021.03.09 SungTae : Local Mode 시 Off-line Mode와 동일하게 Manual로 Lot Open 후 Run 진행하도록 조건 추가
                        && CData.CurCompany != ECompany.SkyWorks && CData.CurCompany != ECompany.Suhwoo)//210427 pjh : 조건 추가 > Skyworks는 Lot Name 입력 후 작업 진행
                    {//201005 pjh : SECS/GEM 사용 시 Lot Open 조건 변경
                        if (!CData.LotInfo.bLotOpen)
                        {
                            //191025 ksg :
                            //CData.Parts[(int)EPart.ONL].iMGZ_No = 0;
                            //CData.LotInfo.iTotalMgz = 1;
                            CData.LotInfo.iTInCnt = 0;
                            CData.LotInfo.iTotalStrip = 1;
                        }
                    }
                    else
                    {
                        if (!CData.LotInfo.bLotOpen)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Lot Open Please");

                            bStartSW = false;
                            return;
                        } //3. Lot Open
                    }
                } //of Muti-Lot 미사용

                if (!CData.Opt.bDoorSkip) // Door Skip이 아니면
                {
                    if (CIO.It.aInput[(int)eX.GRD_DoorFront ] == 0) { CMsg.Show(eMsg.Warning, "Warning", "GRD Front Door close Please"      ); bStartSW = false; return; } //2. Door Open 
                    if (CIO.It.aInput[(int)eX.GRD_DoorRear  ] == 0) { CMsg.Show(eMsg.Warning, "Warning", "GRD Rear Door close Please"       ); bStartSW = false; return; } //2. Door Open 
                    if (CIO.It.aInput[(int)eX.ONL_DoorLeft  ] == 0) { CMsg.Show(eMsg.Warning, "Warning", "Onloader Left Door close Please"  ); bStartSW = false; return; } //2. Door Open 
                    if (CIO.It.aInput[(int)eX.ONL_DoorFront ] == 0) { CMsg.Show(eMsg.Warning, "Warning", "Onloader Front Door close Please" ); bStartSW = false; return; } //2. Door Open 
                    if (CIO.It.aInput[(int)eX.OFFL_DoorRight] == 0) { CMsg.Show(eMsg.Warning, "Warning", "Offloader Right Door close Please"); bStartSW = false; return; } //2. Door Open 
                    if (CIO.It.aInput[(int)eX.OFFL_DoorRear ] == 0) { CMsg.Show(eMsg.Warning, "Warning", "Offloader Rear Door close Please" ); bStartSW = false; return; } //2. Door Open 
                    if (CIO.It.aInput[(int)eX.OFFL_DoorFront] == 0) { CMsg.Show(eMsg.Warning, "Warning", "Offloader Front Door close Please"); bStartSW = false; return; } //2. Door Open 
                }

                if (!xbCoverChk)    { CMsg.Show(eMsg.Warning,   "Warning",  "Grinding Cover Check Please");         bStartSW = false; return; } //2. Cover Open 
                if (!xbLeakSensor)  { CMsg.Show(eMsg.Error,     "Error",    "Check Leak Sensor Please");            bStartSW = false; return; } //2. Leak Sensor 
                if (!xbDryLeak)     { CMsg.Show(eMsg.Error,     "Error",    "Check DryLeak Sensor Please");         bStartSW = false; return; } //2. Leak Sensor //190417 ksg :
                if (!CheckRecipe()) { CMsg.Show(eMsg.Error,     "Error",    "Different the device & BCR Recipe");   bStartSW = false; return; } //3. Device & Bcr 레시피 비교 190211 ksg :
                
                if (!xbCheckEmg)
                {
                    CMsg.Show(eMsg.Error, "Error", "Check Emergency");
                    bStartSW = false;

                    CSQ_Man.It.WaterOnOff(false); //syc : All Water On/Off 
                    // 200728 jym : EMO 동작 시 모든 DI off
                    CSQ_Man.It.StopWater();
                    

                    return;
                } //2. Emergency 190213 ksg : 추가

                if(!xbCheckWarmUp && !CData.Opt.bDryAuto) //2. Warm Up   190116 ksg : 추가 수정 중
                { 
                    TimeSpan sTemp  = WarmUpTime();  // Interval
                    string sTemp1 = CLang.It.GetConvertMsg("Need Warm Up Please") + "\n\r";
                    string sTemp2;
                    int    iTime = 0; //190521 ksg
                    int    Hour = sTemp.Hours;
                    int    Day  = sTemp.Days ;
                    if(Day < 1)
                    {
                        //DateTime OverTime = Convert.ToDateTime(sTemp);
                        if      (Hour >= 2) { sTemp2 = "Warm Up Time is 30 minute"; iTime = 30; }
                        else if (Hour >= 1) { sTemp2 = "Warm Up Time is 15 minute"; iTime = 15; }
                        else                { sTemp2 = "Warm Up Time is 10 minute"; iTime = 10; }
                    }
                    else
                    {
                        sTemp2 = "Warm Up Time is 30 minute";
                        iTime = 30;
                    }

                    if (CData.CurCompany != ECompany.Qorvo      && CData.CurCompany != ECompany.Qorvo_DZ &&
                        CData.CurCompany != ECompany.Qorvo_RT   && CData.CurCompany != ECompany.Qorvo_NC &&
                        CData.CurCompany != ECompany.SST)
                    { 
                        CMsg.Show(eMsg.Warning  , "Warning"  , sTemp1 + sTemp2); 
                    }
                    else
                    { 
                        if(CMsg.Show(eMsg.Warning  , "Warning"  , sTemp1 + sTemp2) == DialogResult.OK)
                        {
                            SaveWarmUpTime(iTime);
                        }
                    }
                    bStartSW = false;
                    return;
                }

                
                //if (CData.Opt.bQcUse && !CGxSocket.It.IsConnected())
                //{
                  //  CMsg.Show(eMsg.Error  , "Error"  , "Check QC Vision Connect"); bStartSW = false; return;
                //} //190406 ksg : Qc  Qc&Eq Connect Check

                if(CDataOption.UseQC && CData.Opt.bQcUse) // 2021-01-21, YYY, for New QC Vision
                {
                    if (!CGxSocket.It.IsConnected())
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check QC Vision Connect"); bStartSW = false; return;
                    }

                    //CTcpIp.It.SendAutoRun();            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.
                    if (!CQcVisionCom.rcvAutoRunACK)
                    {
                        CGxSocket.It.SendMessage("AutoRun");            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.
                        //CGxSocket.It.SendMessage("StatusQuery");

                        _SetLog("[SEND](EQ->QC) CSQ_Main() - AutoRun()");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                    }
                    
                    //190723 ksg : 막음
                    /*
                    m_Delay.Wait(2000);
                    //CTcpIp.It.ReciveMsg();
                    //CheckQcRun = CTcpIp.It.sReciveMsg == "AutoRun_Ok";
                    if (!CheckQcRun) {CMsg.Show(eMsg.Error  , "Error"  , "QC Vision Can't AutoRun Now"); bStartSW = false; return;} //190406 ksg : Qc  Qc&Eq Connect Check
                    */
                }

				//3. Idle Run Chek
                if(iOverwheel > 0)
                {
                    if(iOverwheel == 1)
                    {
                        if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {
                            m_bWhlLimitArm       = true;
                            frmMain.bShowWhlView = true;
                        }

                        m_Delay.Wait(1000);
                        CErr.Show(eErr.LEFT_GRIND_OVER_WHEEL_THICKNESS);
                        bStartSW = false;
                        return;
                    }

                    if(iOverwheel == 2)
                    {
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {
                            m_bWhlLimitArm       = true;
                            frmMain.bShowWhlView = true;
                        }

                        m_Delay.Wait(1000);
                        CErr.Show(eErr.RIGHT_GRIND_OVER_WHEEL_THICKNESS);
                        bStartSW = false;
                        return;
                    }
                }

                if(iOverDresser > 0)
                {
                    if(iOverDresser == 1)
                    {
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {
                            m_bDrsLimitArm       = true;
                            frmMain.bShowWhlView = true;
                        }

                        m_Delay.Wait(1000);
                        CErr.Show(eErr.LEFT_GRIND_OVER_DRESSER_THICKNESS);
                        bStartSW = false;
                        return;
                    }

                    if(iOverDresser == 2)
                    {
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {
                            m_bDrsLimitArm       = true;
                            frmMain.bShowWhlView = true;
                        }

                        m_Delay.Wait(1000);
                        CErr.Show(eErr.RIGHT_GRIND_OVER_DRESSER_THICKNESS);
                        bStartSW = false;
                        return;
                    }
                } 

                // Check Servo On
                if(!CMot.It.Chk_AllSrv())
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Need Servo On");
                    bStartSW = false;
                    return;
                }

                // Check All Home
                if(!bAllHomeDone)
                { 
                    CMsg.Show(eMsg.Warning, "Warning", "First Home Please"); 
                    bStartSW = false; 
                    return;
                }

                //----------------------
                // 2022.02.14 lhs Start : (SCK+) SecsGem 사용, Start 클릭시 use skip 일 경우 알림 표시
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    if (CDataOption.SecsUse == eSecsGem.Use && !CData.Opt.bSecsUse) // skip이면
                    {
                        if (CMsg.Show(eMsg.Query, "Question", "Skip SECS/GEM and Run the machine?") == DialogResult.OK)
                        {
                            // Main 화면에 SECS/GEM Skip 표시
                            //CMsg.Show(eMsg.Notice, "Notice", "SECS/GEM Use : OFF, Auto Run"); 필요없어 삭제
                        }
                        else
                        {
                            CData.Opt.bSecsUse = true;
                            COpt.It.Save();
                            CMsg.Show(eMsg.Notice, "Notice", "SECS/GEM Use : ON");

                            bStartSW = false;
                            return;
                        }
                    }
                }
                // 2022.02.14 lhs End
                //----------------------
                //----------------------
                // 2022.06.07 lhs Start : (SCK+) Start 클릭시 Door Skip일 경우 알림 표시
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    if (CData.Opt.bDoorSkip) // Door Skip일 경우 사용자 선택
                    {
                        if (CMsg.Show(eMsg.Query, "Question", "Skip Door and Run the machine?") == DialogResult.OK)
                        {
                            // Main 화면에 Door Skip 표시된 상태
                        }
                        else
                        {
                            CData.Opt.bDoorSkip = false;
                            COpt.It.Save();
                            CMsg.Show(eMsg.Notice, "Notice", "Door Skip Not Use : OFF");

                            bStartSW = false;
                            return;
                        }
                    }
                }
                // 2022.06.07 lhs End
                //----------------------

                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                // 측정 포인트 수 체크하여 SECE/GEM 용 배열 사이즈 범위 초과 시 Error
                string strResult = "";
                CData.L_GRD.m_bManual18PMeasure = true;
                strResult = CData.L_GRD.CheckMeasurePointCount(true); //L-Table 18 Point용 측정 포인트 수 체크
                
                if (0 == strResult.Length)
                {
                    CData.L_GRD.m_bManual18PMeasure = false;
                    strResult = CData.L_GRD.CheckMeasurePointCount(true); //L-Table 일반용 측정 포인트 수 체크
                    if (0 == strResult.Length)
                    {
                        CData.R_GRD.m_bManual18PMeasure = true;
                        strResult = CData.R_GRD.CheckMeasurePointCount(true); //R-Table 18 Point용 측정 포인트 수 체크
                        
                        if (0 == strResult.Length)
                        {
                            CData.R_GRD.m_bManual18PMeasure = false;
                            strResult = CData.R_GRD.CheckMeasurePointCount(true); //R-Table 일반용 측정 포인트 수 체크
                        }
                    }
                }

                if (0 < strResult.Length) //위 4가지 중 하나라도 측정포인트 수가 배열 사이즈포다 큰 경우 Error
                {
                    CMsg.Show(eMsg.Error, "Error", strResult); //측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과(위험)
                    bStartSW = false;
                    return;
                }
                //
            } // end : if(bStartSW)

            //220405 pjh : Set up strip 조건 추가
            if(CDataOption.UseSetUpStrip && bSetUpStripStatus)
            {
                if (CSq_OfL.It.GetAutoLoadStopMaxStripCheck() && !CSQ_Man.It.OffloaderMGZPlaceDone() &&
                    eAutoStep != eAutoStatus.GoStop && m_iStat != EStatus.Stop)
                {//210309 pjh : Offloader MGZ Place 진행
                    if (!CSq_OfL.It.bChkAutoLoadStopEnd && !bSetUpStripMsg)
                    {
                        if (CSq_OfL.It.Step == 0)
                        { CSq_OfL.It.Init_Cyl(); }

                        CSq_OfL.It.Cyl_PlaceLotEnd();

                        if (!CSQ_Man.It.OffloaderMGZPlaceDone())
                        { return; }
                        _SetLog("Auto Loading Stop Done, Status:OFL_MGZPlace");
                    }
                }

                if (CSq_OfL.It.GetAutoLoadStopMaxStripCheck() && m_iStat == EStatus.Stop && CSQ_Man.It.OffloaderMGZPlaceDone() && !bSetUpStripMsg)
                {//210309 pjh : Message 창 Open | OK : Lot 재개 Cancel : Lot End
                    if (CSq_OfL.It.bChkInOutCnt == true)
                    {//투입/배출 Strip Count가 동일하면
                        CSQ_Man.It.SetUpStripManOn();

                        if ((CMot.It.Get_FP((int)EAx.LeftGrindZone_Z) != CData.SPos.dGRD_Z_Able[0]) || (CMot.It.Get_FP((int)EAx.RightGrindZone_Z) != CData.SPos.dGRD_Z_Able[1])) return;

                        if (CMsg.Show(eMsg.Warning, "Notice", "Set Up Strip Work End. Do you want to continue the LOT? \n OK : Continue Lot      |      Cancel : Lot End") == DialogResult.OK)
                        {
                            bStartSW = true;
                            CSQ_Man.It.bOffMGZBtmPlaceDone = false;
                            CSQ_Man.It.bOffMGZTopPlaceDone = false;
                            bSetUpStripStatus = false;
                        }
                        else
                        {
                            CSq_OfL.It.bChkAutoLoadStopEnd = true;
                        }

                        bSetUpStripMsg = true;
                        CSQ_Man.It.SetUpStripManOff();
                    }
                    else
                    {//투입/배출 Strip Count가 상이하면
                        CSQ_Man.It.SetUpStripManOn();

                        if (CMsg.Show(eMsg.Warning, "Warning", "Set Up Strip Work End. But Different In/Out Strip Count. Do you want to continue the LOT? \n OK : Continue Lot      |      Cancel : Lot End") == DialogResult.OK)
                        {
                            bStartSW = true;
                            CSQ_Man.It.bOffMGZBtmPlaceDone = false;
                            CSQ_Man.It.bOffMGZTopPlaceDone = false;
                            bSetUpStripStatus = false;
                        }
                        else
                        {
                            CSq_OfL.It.bChkAutoLoadStopEnd = true;
                        }

                        bSetUpStripMsg = true;
                        CSQ_Man.It.SetUpStripManOff();
                    }
                    if (!bSetUpStripMsg) return;
                    CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                }

                if (CSq_OfL.It.bChkAutoLoadStopEnd)
                {//210309 pjh : Set Up Strip 동작 완료 Check
                    if (CSQ_Man.It.SetUpStripLotEnd())
                    { eAutoStep = eAutoStatus.GoStop; }
                    else return;

                }

                bSetUpStripLotEnd = bSetUpStripStatus && CSQ_Man.It.bOnMGZPlaceDone && CSQ_Man.It.OffloaderMGZPlaceDone();
            }

            //190406 ksg :
            //190723 ksg : 조건 수정 함
            //if(CData.Opt.bQcUse) bAutoRun  = bStartSW && !bGetError && eAutoStep == eAutoStatus.idle && m_iStat != eStatus.Manual && CheckQcRun;
            //else                 bAutoRun  = bStartSW && !bGetError && eAutoStep == eAutoStatus.idle && m_iStat != eStatus.Manual;
            bAutoRun  = bStartSW && !bGetError && eAutoStep == eAutoStatus.idle && m_iStat != EStatus.Manual;

            bool bAutoStop = bStopSW  ||  bGetError;
            bool bReset    = bResetSW && eAutoStep == eAutoStatus.idle;

            //d m_bAutoStopFlag = bAutoStop;            // QC 모니터링 Thread에서 참조용

            // Skyworks QC use
            if (CDataOption.UseQC && CData.Opt.bQcUse )     // 2021-07-01, jhLee, QC Vision 사용유무만 체크, Site옵션은 미사용 지정 && CData.CurCompany == ECompany.SkyWorks)
            {
                if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual)
                {
                    if (!bAutoStop)
                    {
						// 2021-01-11, YYY, for New QC-Vision

                        if (CQcVisionCom.nQCeqpStatus == 5 )                // QC가 Alarm 상태라면 EQ도 즉시 Alarm 유발
                        {
                            //CQcVisionCom.nQCeqpStatus 
                            //0 : 초기화를 진행하지 않은 초기 상태
                            //1 : 초기화 진행중
                            //2 : STOP 상태
                            //3 : AutoRun 상태
                            //4 : ManualRun 상태
                            //5 : Error 발생 상태

                            bAutoStop = true;
                            CQcVisionCom.nQCCheckDelay = 0;
                            CErr.Show(eErr.QC_ERROR_STATUS);
                        }

                        //if (m_iStat == EStatus.Auto_Running && !CTcpIp.It.bQcRunStaus)

                        //
                        // Auto Run 상태 동기화
                        //
                        if (false)      // 2021-02-02, jhLee : 정비를 위해 QC를 세우는 일도 있으니 강제로 Auto로 전환하는것은 일단 보류
                        {
                            // 설비가 AutoRun 상태인데 QC가 Auto 상태가 아니라면 AutoRun 으로 전환을 요청하고 지정 시간동안 대기한다.
                            if (m_iStat == EStatus.Auto_Running && CQcVisionCom.nQCeqpStatus != 3) // && CQcVisionCom.nQCCheckDelay > 5000 /* 5 second */)// 3= QC autoRun
                            {
                                if (CQcVisionCom.nQCCheckDelay == 0)
                                {
                                    CQcVisionCom.nQCCheckDelay++;                   // 동기화 체크 필요
                                    CGxSocket.It.SendMessage("AutoRun");           // AutoRun으로의 전환을 요청한다.

                                    _SetLog("[SEND](EQ->QC) AutoRun");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                    m_SendQCDelay.Set_Delay(5000);                  // 5초동안 기다려본다.
                                }
                                else
                                {
                                    // 지정한 시간이 지났다면 QC와 상태 동기화 실패 알람 발생
                                    if (m_SendQCDelay.Chk_Delay())
                                    {
                                        CQcVisionCom.nQCCheckDelay = 0;

                                        bAutoStop = true;
                                        CErr.Show(eErr.QC_ERROR_STATUS);
                                    }
                                }
                            }
                            else
                                CQcVisionCom.nQCCheckDelay = 0;             // QC와 정상 동기화 중
                        }//of if true/false                        
                    }
                }
            }

            // 2021.03.09 SungTae Start : Leak Sensor 감지 시 30초 Delay 후 Alarm 발생하도록 고객사(ASE-KR) 요청으로 수정
            if (CDataOption.ChkGrdLeak == eChkGrdLeak.Use)
            {
                if (m_bRun && !xbLeakSensor && !bGetError)
                {
                    if (CData.CurCompany == ECompany.ASE_KR)
                    {
                        if (m_DelayLeak.Chk_Delay())
                        {
                            CErr.Show(eErr.TABLE_LEAK_SENSOR_ERROR);
                            return;
                        }
                    }
                    else
                    {
                        CErr.Show(eErr.TABLE_LEAK_SENSOR_ERROR);
                        return;
                    }
                }
                else if(CData.CurCompany == ECompany.ASE_KR)
                {
                    m_DelayLeak.Set_Delay(30000);
                }
            }

            if(CDataOption.ChkDryLeak == eChkDryLeak.Use)
            {
                if(m_bRun && !xbDryLeak && !bGetError)
                {
                    if (CData.CurCompany == ECompany.ASE_KR)
                    {
                        if (m_DelayDryLeak.Chk_Delay())
                        {
                            CErr.Show(eErr.DRY_LEAK_SENSOR_ERROR);
                            return;
                        }
                    }
                    else
                    {
                        CErr.Show(eErr.DRY_LEAK_SENSOR_ERROR);
                        return;
                    }
                }
                else if (CData.CurCompany == ECompany.ASE_KR)
                {
                    m_DelayDryLeak.Set_Delay(30000);
                }
            }
            // 2021.03.09 SungTae End

            if(bAutoRun)
            {
                m_bToStop = false; //190723 ksg : 스위치 2번 눌러서 추가  
                eAutoStep = eAutoStatus.Ready;
            }
            if(bAutoStop && eAutoStep != eAutoStatus.idle)
            {
                m_bToStop = true;    
            }
            if(bReset && eAutoStep == eAutoStatus.idle)
            {
                Reset();

                //에러 클리어 시점 jamtime 스탑, idaleTime 스타트
                //CData.swLotJam.Stop();
                CData.bLotJam = false;

                //CData.swLotIdle.Start();
                CData.bLotIdle = true;
                //m_iStat = eStatus.Reset;
                //CSQ_Man.It.Seq = ESeq.Reset;
            }
            //ksg : To Stop 중 Buzzer 끄기
            if(bGetError && (eAutoStep == eAutoStatus.Auto || eAutoStep == eAutoStatus.GoStop) && bResetSW)
            {
                m_bBuzSkip = true;
            }
            // Trace_Log();  0404 LCY

            //220527 pjh : Qorvo에서 SPC Error가 발생되었을 때 Reset Button 누르면 Buzzer Off
            if (CData.CurCompany == ECompany.Qorvo && isSPC && bReset)
            {
                isSPC = false;
            }
            //

            //Step                                                          
            switch (eAutoStep)
            {
                case eAutoStatus.idle:
                    {
                        //20190624 josh
                        //jsck secsgem
                        //CEID 999002 - idle
                        if (CData.Opt.bSecsUse && CData.GemForm != null)
                        { CData.GemForm.Set_ProcStateChange((int)SECSGEM.JSCK.eProcStatus.Idle); }
                        return;
                    }

                case eAutoStatus.Ready:
                    {
                        if (!ToReady()) return;
	                    switch (StartHostInterface())  // 2020-10-19, jhLee : SPIL CS VOC, HOST로부터 START RCMD 수신뒤 설비 AUTO 진행
	                    {
	                        case -1:    // 수행 실패, 이전 상태로 되돌아간다.
	                            eAutoStep = eAutoStatus.GoStop;
	                            return;

	                        case 0:     // 반복 지속
	                            return;

	                        case 1:     // 다음 진행
	                            m_nRCMDStartSeq = 0;
	                            break;
	                    }
                        eAutoStep = eAutoStatus.Auto;

                        // 2020.10.20 JSKim St - Idle 상태인 경우 Door Lock 동기화 상태를 초기화 한다.
                        m_bFirstLock = false;
                        // 2020.10.20 JSKim Ed

                        // 2020.12.12 JSKim St -  마지막 iStat 호출 용
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (CData.Parts[(int)EPart.GRDL].bExistStrip == true && CData.Parts[(int)EPart.GRDL].iStripStat != 0)
                            {
                                CData.Parts[(int)EPart.GRDL].iStat = (ESeq)CData.Parts[(int)EPart.GRDL].iStripStat;
                            }

                            if (CData.Parts[(int)EPart.GRDR].bExistStrip == true && CData.Parts[(int)EPart.GRDR].iStripStat != 0)
                            {
                                CData.Parts[(int)EPart.GRDR].iStat = (ESeq)CData.Parts[(int)EPart.GRDR].iStripStat;
                            }
                        }
                        // 2020.12.12 JSKim Ed

                        m_bRun = true;
                        //lot start 시작 시점
                        if (CData.SpcInfo.sStartTime == "")
                        {
                            CData.SpcInfo.sStartTime = DateTime.Now.ToString("HH:mm:ss");
                            //jsck secsgem
                            //svid 999103 lot started time
                            if (CData.Opt.bSecsUse && CData.GemForm != null)
                            {
                                CData.GemForm.Set_SVID(Convert.ToInt32(SECSGEM.JSCK.eSVID.LOT_Started_Time), DateTime.Now.ToString("yyyyMMddHHmmss"));
                            }
                        }

                        CData.bLotTotal = true;
                        CData.bLotJam = false;
                        CData.bLotIdle = false;

                        m_Delay1.Set_Delay(15000);
                        CLog.mLogSeq("AutoRun() -> eAutoStatus.Ready");
                        m_DelayQc.Set_Delay(1000); //190806 ksg :

                        // 2021-01-17, jhLee QC Status Sync
                        if (CDataOption.UseQC && CData.Opt.bQcUse)
                        {
                            if (!CGxSocket.It.IsConnected())
                            {
                                CMsg.Show(eMsg.Error, "Error", "Check QC Vision Connect"); bStartSW = false; return;
                            }

                            //CTcpIp.It.SendAutoRun();            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.
                            CGxSocket.It.SendMessage("AutoRun");            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.

                            _SetLog("[SEND](EQ->QC) AutoRun");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                        }



                        return;
                    }

                case eAutoStatus.Auto:
                    {
                        if (!m_bToStop)
                        {
                            m_iStat = EStatus.Auto_Running;
                            //20190624 josh
                            //jsck secsgem
                            //CEID 999002 - autorun
                            if (CData.Opt.bSecsUse && CData.GemForm != null)
                            {
                                CData.GemForm.Set_ProcStateChange(Convert.ToInt32(SECSGEM.JSCK.eProcStatus.Run));
                            }

                            //190529 ksg :
                            if (m_Delay1.Chk_Delay())
                            {
                                if (!CData.Opt.aTblSkip[(int)EWay.L] && !CIO.It.Get_X(eX.PUMPL_Run)) CheckPumpRun();
                                if (!CData.Opt.aTblSkip[(int)EWay.R] && !CIO.It.Get_X(eX.PUMPR_Run)) CheckPumpRun();

                                // 2020.10.22 JSKim St
                                if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                                {
                                    if (!CData.Opt.aTblSkip[(int)EWay.L] && !CIO.It.Get_X(eX.ADD_PUMPL_Run)) CheckAddPumpRun();
                                    if (!CData.Opt.aTblSkip[(int)EWay.R] && !CIO.It.Get_X(eX.ADD_PUMPR_Run)) CheckAddPumpRun();
                                }
                                // 2020.10.22 JSKim Ed
                            }

                            //if (CData.Opt.bQcUse)           // QC Vision 사용시
                            //{
                            //    if (m_DelayQc.Chk_Delay())
                            //    {
                            //        m_DelayQc.Set_Delay(3000);                  // 다음 실행 주기
                            //        //CTcpIp.It.SendStatus();                     // QC 상태 조회 요청
                            //        CGxSocket.It.SendMessage("StatusQuery");
                            //    }
                            //}

                            if (AutoRunCly())
                            {
                                if (CData.IsMultiLOT())
                                {
                                    // 처리를 끝마친 LOT이 존재하는지 여부를 확인한다.
                                    if (CData.LotMgr.GetNotCompleteCount() > 0)
                                    {
                                        // 아직 끝마치지 않은 LOT이 남아있다.

                                        // return;

                                        bool bWorkEnd = (   CData.Parts[(int)EPart.ONL].iStat   == ESeq.ONL_WorkEnd &&
                                                            CData.Parts[(int)EPart.INR].iStat   == ESeq.INR_WorkEnd &&
                                                            CData.Parts[(int)EPart.GRDL].iStat  == ESeq.GRL_WorkEnd &&
                                                            CData.Parts[(int)EPart.GRDR].iStat  == ESeq.GRR_WorkEnd &&
                                                            CData.Parts[(int)EPart.OFP].iStat   == ESeq.OFP_WorkEnd &&
                                                            CData.Parts[(int)EPart.DRY].iStat   == ESeq.DRY_WorkEnd );

                                        // 모든 작업이 끝났으면 Stop으로 전환해준다.
                                        if (bWorkEnd)
                                        {
                                            CData.Parts[(int)EPart.ONL].iStat = ESeq.ONL_Pick;      //다음번에는 Mgz 집는 것을 시도한다.
                                            eAutoStep = eAutoStatus.GoStop;
                                        }

                                    }
                                    else
                                    {
                                        eAutoStep = eAutoStatus.GoStop;
                                    }
                                }
                                else
                                {
                                    //210113 pjh : Empty Magazine Lot End 시 Inrail Wait Check 후 Lot End
                                    if (Chk_EmptyMGZ() && CSq_Inr.It.m_LotEndChk == true)
                                    {
                                        if (!CMot.It.Get_Mv((int)EAx.Inrail_X, CData.SPos.dINR_X_Wait))
                                        {
                                            return;
                                        }
                                        else CSq_Inr.It.m_LotEndChk = false;
                                    }

                                    //20190624 josh
                                    //jsck secsgem
                                    //svid 999103 lot ended time
                                    if (CData.Opt.bSecsUse && CData.GemForm != null)
                                    {
                                        CData.GemForm.OnLotEnded(CData.LotInfo.sLotName, CData.LotInfo.iTInCnt.ToString(), CData.LotInfo.iTOutCnt.ToString());
                                    }

                                    //20190703 ghk_dfserver
                                    //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip)
                                    if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip)
                                    {
                                        if (!m_bDfWorkEnd)
                                        {
                                            if (!CData.dfInfo.bBusy)
                                            {
                                                CDf.It.SendLotEnd();
                                                m_bDfWorkEnd = true;
                                            }
                                            return;
                                        }
                                        else
                                        {
                                            if (CDf.It.ReciveAck((int)ECMD.scLotEnd))
                                            {
                                                m_bDfWorkEnd = false;
                                            }
                                            else
                                            { return; }
                                        }
                                    }

                                    LotInfoClear(); // CSQ_Main
                                    CData.SpcInfo.sEndTime = DateTime.Now.ToString("HH:mm:ss");

                                    //20190624 josh
                                    //jsck secsgem
                                    //svid 999103 lot ended time
                                    if (CData.Opt.bSecsUse && CData.GemForm != null)
                                    { CData.GemForm.Set_SVID(Convert.ToInt32(SECSGEM.JSCK.eSVID.LOT_Ended_Time), DateTime.Now.ToString("yyyyMMddHHmmss")); }

                                    eAutoStep = eAutoStatus.GoStop;
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
	                                    //if (CTcpIp.It.IsConnect() && CTcpIp.It.bIsConnect)
	                                    //{
	                                    //CTcpIp.It.SendLotEnd();
	                                    //}
	                                    if ( CGxSocket.It.IsConnected())
	                                    {
	                                        CGxSocket.It.SendMessage("LotEnd");
                                            //CTcpIp.It.SendLotEnd();

                                            _SetLog("[SEND](EQ->QC) LotEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                        }
                                    }

                                    // 2020.09.16 SungTae : Program 실행 중 Left & Right Probe Ejector Off
                                    if (CDataOption.IsPrbEjector) // 200924 jym : 라이센스 옵션으로 변경
                                    {
                                        if (CIO.It.Get_Y(eY.GRDL_ProbeEjector)) CIO.It.Set_Y(eY.GRDL_ProbeEjector, false);
                                        if (CIO.It.Get_Y(eY.GRDR_ProbeEjector)) CIO.It.Set_Y(eY.GRDR_ProbeEjector, false);
                                    }
                                    //210806 pjh : Set Up Strip 이후 Lot End 시 변수 초기화    
                                    CSQ_Man.It.bOnMGZPlaceDone = false;
                                    CSQ_Man.It.bOffMGZBtmPlaceDone = false;
                                    CSQ_Man.It.bOffMGZTopPlaceDone = false;
                                    //
                                    bSetUpStripStatus = false;
                                } //of if IsMultiLOT
                            }
                            else
                            {//210113 pjh : Empty Magazine Auto run Hang up
                                
                                if (bSetUpStripLotEnd)
                                {//210806 pjh : Set up strip Lot End 조건
                                    CData.Parts[(int)EPart.ONL].iStat   = ESeq.ONL_WorkEnd;
                                    CData.Parts[(int)EPart.INR].iStat   = ESeq.INR_WorkEnd;
                                    CData.Parts[(int)EPart.ONP].iStat   = ESeq.ONP_WorkEnd;
                                    CData.Parts[(int)EPart.GRDL].iStat  = ESeq.GRL_WorkEnd;
                                    CData.Parts[(int)EPart.GRDR].iStat  = ESeq.GRR_WorkEnd;
                                    CData.Parts[(int)EPart.OFP].iStat   = ESeq.OFP_WorkEnd;
                                    CData.Parts[(int)EPart.DRY].iStat   = ESeq.DRY_WorkEnd;
                                    CData.Parts[(int)EPart.OFL].iStat   = ESeq.OFL_WorkEnd;
                                    m_iStat = EStatus.Stop;
                                    _SetLog("Set up Strip Lot End");//220405 pjh : Set Up String Lot End 시 Log 추가
                                    bSetUpStripLotEnd = false;
                                }
                                //if (bSetUpStripStatus)
                                //{//210309 pjh : Set up strip 조건 추가
                                //    if (CSQ_Man.It.bOnMGZPlaceDone && CSQ_Man.It.OffloaderMGZPlaceDone())
                                //    {
                                //        CData.Parts[(int)EPart.ONL].iStat = ESeq.ONL_WorkEnd;
                                //        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WorkEnd;
                                //        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_WorkEnd;
                                //        CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_WorkEnd;
                                //        CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_WorkEnd;
                                //        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                //        CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WorkEnd;
                                //        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_WorkEnd;
                                //        m_iStat = EStatus.Stop;
                                //        bSetUpStripLotEnd = false;
                                //    }
                                //}
                                else
                                {
                                    if (Chk_EmptyMGZ())
                                    {
                                        CData.Parts[(int)EPart.INR].iStat   = ESeq.INR_WorkEnd;
                                        CData.Parts[(int)EPart.ONP].iStat   = ESeq.ONP_WorkEnd;
                                        CData.Parts[(int)EPart.GRDL].iStat  = ESeq.GRL_WorkEnd;
                                        CData.Parts[(int)EPart.GRDR].iStat  = ESeq.GRR_WorkEnd;
                                        CData.Parts[(int)EPart.OFP].iStat   = ESeq.OFP_WorkEnd;
                                        CData.Parts[(int)EPart.DRY].iStat   = ESeq.DRY_WorkEnd;
                                        CData.Parts[(int)EPart.OFL].iStat   = ESeq.OFL_WorkEnd;
                                    }
                                }
                            }

                            //-------------------koo : Qorvo Lot Rework;-----------------------------------

                            if (!CData.IsMultiLOT())//210909 pjh : Qorvo Issue 관련 수정. Multi Lot을 사용하지 않을 때 Pre Lot End 기능 사용
                            {
                                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                                //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
                                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                    CData.CurCompany == ECompany.SST)
                                {
                                    if (CData.bPreLotend)
                                    {
                                        if (CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip)
                                        {
                                            CData.bPreLotEndMsgShow = true;
                                            eAutoStep = eAutoStatus.GoStop;
                                            return;
                                        }
                                    }
                                }
                            }
                            //-----------------------------------------------------------------------------
                            return;
                        }

                        m_bToStop = false;
                        eAutoStep = eAutoStatus.GoStop;
                        return;
                    }

                case eAutoStatus.GoStop:
                    {
                        if (!ToStop())
                        {
                            if (AutoRunCly())
                            {
                                if (CData.IsMultiLOT())
                                {
                                    // 남아있는 Strip과 LOT 정보가 없다면 설비를 정지 상태로 만들어준다.

                                    // 별다른 처리없이 설비를 Stop 시킨다.

                                    m_bToStop = false; //190723 ksg : 스위치 2번 눌러서 추가  
                                    eAutoStep = eAutoStatus.Stop;
                                    m_bRun = false;

                                    // 처리를 끝마친 LOT이 존재하는지 여부를 확인한다.
                                    if (CData.LotMgr.GetNotCompleteCount() > 0)
                                    {
                                        // 아직 끝마치지 않은 LOT이 남아있다.

                                        return;
                                    }

                                    // 여기서부터는 완전해 설비를 LOT 종료 후 정지 상태로 만들어준다.

                                    // 특별한 마무리 처리를 해준다.
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

                                    if (CIO.It.Get_Y(eY.GRDL_TbFlow)) CData.L_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off
                                    if (CIO.It.Get_Y(eY.GRDR_TbFlow)) CData.R_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off

                                    // 2020.09.16 SungTae : Program 실행 중 Left & Right Probe Ejector Off
                                    if (CDataOption.IsPrbEjector) // 200924 jym : 라이센스 옵션으로 변경
                                    {
                                        if (CIO.It.Get_Y(eY.GRDL_ProbeEjector)) CIO.It.Set_Y(eY.GRDL_ProbeEjector, false);
                                        if (CIO.It.Get_Y(eY.GRDR_ProbeEjector)) CIO.It.Set_Y(eY.GRDR_ProbeEjector, false);
                                    }

                                    //미사용 CData.bCompleteStop = true;       // 2021-06-03, jhLee : LOT 종료 후 설비를 완전히 정지시켰는가 ? 각종 Water 종류들의 Off 처리여부

                                    if (CData.Opt.bQcUse)
                                    {
                                        if (!CGxSocket.It.IsConnected())
                                        {
                                            CMsg.Show(eMsg.Error, "Error", "Check QC Vision Connect"); bStartSW = false; return;
                                        }

                                        //CTcpIp.It.SendAutoRun();            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.
                                        CGxSocket.It.SendMessage("AutoStop");            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.

                                        _SetLog("[SEND](EQ->QC) AutoStop");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                    }

                                    return;
                                }//of if Multi-LOT

                                //-------------------koo : Qorvo Lot Rework;-----------------------------------
                                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                                //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
                                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                    CData.CurCompany == ECompany.SST)
                                {
                                    //190613 koo : Lotend
                                    if (CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip)
                                    {
                                        return;
                                    }
                                }
                                //-----------------------------------------------------------------------------
                                
                                LotInfoClear(); // CSQ_Main

                                eAutoStep = eAutoStatus.GoStop;

                                CData.LotInfo.bLotEnd   = true;
                                CData.bLotEndShow       = true;
                                CData.bLotEndMsgShow    = true; //190521 ksg :
                                CData.LotInfo.bLotOpen  = false;
                                CData.bSecLotOpen       = false; //191030 ksg :

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

                                if (CIO.It.Get_Y(eY.GRDL_TbFlow)) CData.L_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off
                                if (CIO.It.Get_Y(eY.GRDR_TbFlow)) CData.R_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off

                                // 2020.09.16 SungTae : Program 실행 중 Left & Right Probe Ejector Off
                                if (CDataOption.IsPrbEjector) // 200924 jym : 라이센스 옵션으로 변경
                                {
                                    if (CIO.It.Get_Y(eY.GRDL_ProbeEjector)) CIO.It.Set_Y(eY.GRDL_ProbeEjector, false);
                                    if (CIO.It.Get_Y(eY.GRDR_ProbeEjector)) CIO.It.Set_Y(eY.GRDR_ProbeEjector, false);
                                }

                                eAutoStep = eAutoStatus.Stop;
                            }

                            return;
                        }

                        // 2020.12.12 JSKim St - 마지막 iStat 보관
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                            {
                                CData.Parts[(int)EPart.GRDL].iStripStat = Convert.ToInt32(CData.Parts[(int)EPart.GRDL].iStat);
                            }

                            if (CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                            {
                                CData.Parts[(int)EPart.GRDR].iStripStat = Convert.ToInt32(CData.Parts[(int)EPart.GRDR].iStat);
                            }
                        }
                        // 2020.12.12 JSKim Ed

                        // 2021-01-17, jhLee QC Status Sync
                        if (CDataOption.UseQC && CData.Opt.bQcUse)
                        {
                            if (!CGxSocket.It.IsConnected())
                            {
                                CMsg.Show(eMsg.Error, "Error", "Check QC Vision Connect"); bStartSW = false; return;
                            }

                            //CTcpIp.It.SendAutoRun();            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.
                            CGxSocket.It.SendMessage("AutoStop");            // QC Vision에게 AutoRun 상태로 전환하라고 요청한다.

                            _SetLog("[SEND](EQ->QC) AutoStop");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                        }


                        m_bToStop = false; //190723 ksg : 스위치 2번 눌러서 추가  
                        eAutoStep = eAutoStatus.Stop;
                        m_bRun = false;
                        return;
                    }

                case eAutoStatus.Stop:
                    {
                        if (bGetError)
                        {
                            m_iStat = EStatus.Error;

                            //20190624 josh
                            //jsck secsgem
                            //CEID 999002 - error
                            if (CData.Opt.bSecsUse && CData.GemForm != null)
                            { CData.GemForm.Set_ProcStateChange(Convert.ToInt32(SECSGEM.JSCK.eProcStatus.Down)); }
                        }
                        else
                        {
                            m_iStat = EStatus.Stop;

                            //20190624 josh
                            //jsck secsgem
                            //CEID 999002 - idle
                            if (CData.Opt.bSecsUse && CData.GemForm != null)
                            { CData.GemForm.Set_ProcStateChange(Convert.ToInt32(SECSGEM.JSCK.eProcStatus.Idle)); }
                        }

                        CSq_OfL.It.bChkAutoLoadStopEnd = false;
                        CSQ_Man.It.bOnMGZPlaceDone = false;

                        eAutoStep = eAutoStatus.idle;
                        //유휴시간 시작점. idle time

                        //CData.swLotIdle.Start();
                        CData.bLotIdle = true;
                        //CData.swLotJam.Stop();
                        CData.bLotJam = false;

                    	m_nRCMDStartSeq = 0;                // 2020-10-19, jhLee : SPIL社 RCMD START 처리를 위한 시퀀스 초기화
                        return;
                    }
            }
        }


        // 2020-10-19, jhLee : SPIL CS VOC, HOST로부터 START RCMD 수신뒤 설비 AUTO 진행을 위해 HOST에게 요청 후 대기
        // return true : 조건을 만족하고 설비를 AUTO로 전환한다.
        //        false : AUTO 수행 대기
        public int StartHostInterface()
        {
            // HOST에게 RCMD를 묻는 기능을 진행 할것인지 여부 판단
            // 2022.06.07 SungTae Start : [수정] ASE-KH도 SPIL 처럼 S2F41 RCMD=START를 받아야만 시작되도록 수정
            //if (CData.CurCompany != ECompany.SPIL) return 1;                                // SPIL社가 아니면 기능 미사용
            if (CData.CurCompany != ECompany.SPIL && CData.CurCompany != ECompany.ASE_K12) return 1;
            // 2022.06.07 SungTae End

            if (!CData.Opt.bSecsUse || (CData.Opt.iRCMDStartTimeout <= 0)) return 1;     // SECS미사용이거나 Timeout을 0 이하로 설정하면 미사용

            switch ( m_nRCMDStartSeq )
            {
                case 0:     // 최초 진입
                    {
                        // HOST에게 S6F11로 999011을 전송한다. (START event report)
                        if (CData.GemForm != null)
                        {
                            m_RCMDTimeout.Set_Delay(CData.Opt.iRCMDStartTimeout);   //  HOST에서 RCMD를 보내오지않는 시간 측정하여 Timeout 처리용
                            m_nRCMDRecv = 0;                    // RCMD 수신 flag 초기화

                            // 2020-11-09, jhLee, 진행중인 LOT이 있다면 Restart를 전송한다.
                            if (CData.GemForm.IsLotOpen())
                            {
                                // RESTART를 상태 변화 없이 HOST로 송신한다.
                                CData.GemForm.Set_MachineReStart(Convert.ToInt32(SECSGEM.JSCK.eProcStatus.Run), true);

                                // 2022.06.08 SungTae : [추가] Log 추가
                                _SetLog($"[SEND](H<-E) S6F11 CEID = {(int)SECSGEM.JSCK.eCEID.Machine_ReStart}({SECSGEM.JSCK.eCEID.Machine_ReStart}), Process Status : {SECSGEM.JSCK.eProcStatus.Run}");
                            }
                            else
                            {
                                // START를 상태 변화 없이 HOST로 송신한다.
                                CData.GemForm.Set_MachineStart(Convert.ToInt32(SECSGEM.JSCK.eProcStatus.Run), true);

                                // 2022.06.08 SungTae : [추가] Log 추가
                                _SetLog($"[SEND](H<-E) S6F11 CEID = {(int)SECSGEM.JSCK.eCEID.Machine_Start}({SECSGEM.JSCK.eCEID.Machine_Start}), Process Status : {SECSGEM.JSCK.eProcStatus.Run}");
                            }

                            m_nRCMDStartSeq = 1;
                            return 0;                   // 반복 진행
                        }
                        else // 예외상황, SECS 통신 불가능
                        {
                            CErr.Show(eErr.HOST_COMMUCATION_DISCONNECT_ERROR);      // HOST와 연결이 되어있지 않다.
                            m_nRCMDStartSeq = 0;                // 시퀀스 초기화
                            return -1;  // 실패
                        }
                    }
                    break;

                case 1:  // HOST로부터 RCMD START를 기다린다.
                    {
                        if (m_nRCMDRecv == 1)             //  HOST에서 RCMD를 받았는가 ? 0:미수신, 1:AUTO 수신, -1:AUTO외의 잘못된 응답
                        {
                            m_nRCMDRecv = 0;                    // RCMD 수신 flag 초기화
                            m_nRCMDStartSeq = 0;                // 시퀀스 초기화
                            return 1;                        // 다음 Step으로 넘어가 설비의 AUTO를 처리한다.
                        }
                        else if (m_nRCMDRecv < 0)             // AUTO 외 STOP 명령 수신
                        {
                            CErr.Show(eErr.HOST_RCMD_RESPONSE_ERROR);   // HOST에서 지정된 RCMD로 START 명령대신 다른 명령이 내려왔다..
                            m_nRCMDStartSeq = 0;                // 시퀀스 초기화
                            return -1; // 진행 실패
                        }

                        if (m_RCMDTimeout.Chk_Delay())        // HOST에서 지정된 시간동은 START가 내려오지 않았다.
                        {
                            CErr.Show(eErr.HOST_RCMD_START_TIMEOUT_ERROR);      // HOST에서 지정된 시간동안 RCMD로 START 명령이 내려오지 않았다.
                            m_nRCMDStartSeq = 0;                // 시퀀스 초기화
                            return -1;      // 실패
                        }
                    }
                    break;                       

                default:    // 예외 상황
                    {
                        CErr.Show(eErr.HOST_RCMD_RESPONSE_ERROR);   // 잘못된 처리로 오류
                        m_nRCMDStartSeq = 0;                        // 시퀀스 초기화
                        return -1;
                    }
            }

            return 0;                                        // 기본적으로 반복 수행
        }//of public int StartHostInterface()


        //
        // 2021-02-05, jhLee : Skyworks Run-down건으로 Light-Curtain 처리를 각자 On/Off loader로 이동시켰다.
        //
        ////koo : pause
        //public void CheckAreaSensor()
        //{
        //    //Display
        //    bool LeftArea  = CIO.It.Get_X(eX.ONL_LightCurtain);
        //    bool RightArea = CIO.It.Get_X(eX.OFFL_LightCurtain);

        //    //if(LeftArea) m_bDspLeftArea = true;
        //    //else         m_bDspLeftArea = false;  

        //    //if(RightArea) m_bDspRightArea = true;
        //    //else          m_bDspRightArea = false;

        //    //
        //    // 2021-02-04, jhLee : Skyworks에서 Ver3.1.2.25 이상의 버전에서 AutoRun 상태에서 무언정지 발생하여 Ver3.1.2.19 버전으로 원복함
        //    //
        //    //
        //    // // 210106 myk : 장비 구동중일때만 에러 발생
        //    // if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual || m_iStat == EStatus.Warm_Up || m_iStat == EStatus.All_Homing)
        //    // {
        //    //TimeOut

        //        if (!LeftArea) m_OnLAreaTiOut.Set_Delay(10000);
        //        else
        //        {
        //            if (m_OnLAreaTiOut.Chk_Delay())
        //            {
        //                m_OnLAreaTiOut.Set_Delay(10000);
        //                CErr.Show(eErr.ONLOADER_DETECT_LIGHTCURTAIN);
        //            }
        //        }

        //        if (!RightArea) m_OffLAreaTiOut.Set_Delay(10000);
        //        else
        //        {
        //            if (m_OffLAreaTiOut.Chk_Delay())
        //            {
        //                m_OffLAreaTiOut.Set_Delay(10000);
        //                CErr.Show(eErr.OFFLOADER_DETECT_LIGHTCURTAIN);
        //            }
        //        }

        //    // }
        //    //Pause & Resume
        //    // 200618 jym : 조건 변경 사이트에서 변수로
        //    if(CDataOption.IsPause) CheckPause(LeftArea,RightArea);
        //}


        //// koo : pause
        //private void CheckPause(bool OnLOn,bool OffLOn)
        //{
            
        //    if (OnLOn)
        //    {
        //        //OnLoader elevator Parts
        //        CMot.It.Pause((int)EAx.OnLoader_X);
        //        CMot.It.Pause((int)EAx.OnLoader_Y);
        //        CMot.It.Pause((int)EAx.OnLoader_Z);
        //        if (CData.CurCompany == ECompany.SkyWorks)  // 스카이웍스만 피커 까지 정지
        //        {
        //            //OnLoader Picker Parts
        //            CMot.It.Pause((int)EAx.OnLoaderPicker_X);
        //            CMot.It.Pause((int)EAx.OnLoaderPicker_Z);
        //            CMot.It.Pause((int)EAx.OnLoaderPicker_Y);
        //        }
        //        //Inrail Parts
        //        CMot.It.Pause((int)EAx.Inrail_X);
        //        CMot.It.Pause((int)EAx.Inrail_Y);
        //    }
        //    else 
        //    {
        //         //OnLoader elevator Parts
        //        CMot.It.Resume((int)EAx.OnLoader_X);
        //        CMot.It.Resume((int)EAx.OnLoader_Y);
        //        CMot.It.Resume((int)EAx.OnLoader_Z);
        //        if (CData.CurCompany == ECompany.SkyWorks)  // 스카이웍스만 피커 까지 정지
        //        {
        //            //OnLoader Picker Parts
        //            CMot.It.Resume((int)EAx.OnLoaderPicker_X);
        //            CMot.It.Resume((int)EAx.OnLoaderPicker_Z);
        //            CMot.It.Resume((int)EAx.OnLoaderPicker_Y);
        //        }
        //        //Inrail Parts
        //        CMot.It.Resume((int)EAx.Inrail_X);
        //        CMot.It.Resume((int)EAx.Inrail_Y);

        //    }
        //    if(OffLOn)
        //    {
        //        //OffLoader elevator Parts
        //        CMot.It.Pause((int)EAx.OffLoader_X);
        //        CMot.It.Pause((int)EAx.OffLoader_Y);
        //        CMot.It.Pause((int)EAx.OffLoader_Z);
        //        if (CData.CurCompany == ECompany.SkyWorks)  // 스카이웍스만 피커 까지 정지
        //        {
        //            //OffLoader Picker Parts
        //            CMot.It.Pause((int)EAx.OffLoaderPicker_X);
        //            CMot.It.Pause((int)EAx.OffLoaderPicker_Z);
        //        }
        //        //Dryzone Parts
        //        CMot.It.Pause((int)EAx.DryZone_X);
        //        CMot.It.Pause((int)EAx.DryZone_Z);
        //        CMot.It.Pause((int)EAx.DryZone_Air);

        //        // 2021-01-11, YYY, for New QC-Vision
        //        //if(!CTcpIp.It.bPause_Ok)
        //        //{
        //        //CTcpIp.It.SendPause();
        //        //CTcpIp.It.bResume_Ok = false;
        //        //}
        //        // 3: AutoRun 상태 , 4 : Manual Run 상태
        //        if (CQcVisionCom.nQCeqpStatus == 3 || CQcVisionCom.nQCeqpStatus == 4)
        //        {
        //            CGxSocket.It.SendMessage("AutoStop");
        //        }
        //    }
        //    else 
        //    {
        //        //OffLoader elevator Parts
        //        CMot.It.Resume((int)EAx.OffLoader_X);
        //        CMot.It.Resume((int)EAx.OffLoader_Y);
        //        CMot.It.Resume((int)EAx.OffLoader_Z);
        //        if (CData.CurCompany == ECompany.SkyWorks)  // 스카이웍스만 피커 까지 정지
        //        {
        //            //OffLoader Picker Parts
        //            CMot.It.Resume((int)EAx.OffLoaderPicker_X);
        //            CMot.It.Resume((int)EAx.OffLoaderPicker_Z);
        //        }
        //        //Dryzone Parts
        //        CMot.It.Resume((int)EAx.DryZone_X);
        //        CMot.It.Resume((int)EAx.DryZone_Z);
        //        CMot.It.Resume((int)EAx.DryZone_Air);

        //        //QC
        //        //if(!CTcpIp.It.bResume_Ok)
        //        //{
        //        //CTcpIp.It.SendResume();
        //        //CTcpIp.It.bPause_Ok = false;
        //        //}
        //        //if (CQcVisionCom.nQCeqpStatus == 2)//2: Stop 상태
        //        //{
        //        //    CGxSocket.It.SendMessage("AutoRun");
        //        //}
        //    }
        //}


        public void EquipStatus()
        {
            int Status = (int)ETowerStatus.Idle;

            bool isInit         = CSQ_Man.It.Seq == ESeq.All_Home; 
            bool isError        = m_iStat == EStatus.Error       ;
            bool isRunning      = m_iStat == EStatus.Auto_Running;
            bool isManual       = m_iStat == EStatus.Manual      ; 
            bool isToStop       = m_bToStop                      ;
            bool IsLotEnd       = CData.LotInfo.bLotEnd == true  ;
            bool isStop         = m_iStat == EStatus.Stop        ;
            bool isIdle         = m_iStat == EStatus.Idle        ;
            bool isLoadingStop  = (CSQ_Main.It.m_bPause == true) ;
            
            // 2021-06-03, jhLee : Multi-LOT 개별적인 LOT는 직접 처리해 준다.
            if (CData.IsMultiLOT())
            {
                IsLotEnd = CData.bLotCompleteBuzzer;                // LOT 종료 버저가 필요하다. LOT 종료 표시 화면에서 작업자가 '확인'을 클릭하면 Clear 해준다.
            }

            if      (isIdle)    Status = (int)ETowerStatus.Idle;
            else if (isInit)    Status = (int)ETowerStatus.Init;
            else if (isError)   Status = (int)ETowerStatus.Error;
            else if (IsLotEnd)  Status = (int)ETowerStatus.LotEnd;      // 2021-06-09, jhLee, LOT END 표시 우선순위를 높여주었다. 
            else if (isLoadingStop && isRunning)    // 2022.06.17 lhs : (SCK+) AutoRun 상태에서 Loading Stop 일때
            {
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    Status = (int)ETowerStatus.LoadingStop;
                }
            }
            else if (isRunning)
            {
                Status = (int)ETowerStatus.Run;

                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET)
                {
                    if (CIO.It.Get_X(eX.ONL_LightCurtain) || CIO.It.Get_X(eX.OFFL_LightCurtain))
                    { CData.bLcDetect = true; }
                    else
                    { CData.bLcDetect = false; }
                }
                else
                { CData.bLcDetect = false; }
            }
            else if (isManual)  Status = (int)ETowerStatus.Manual;
            else if (isToStop)  Status = (int)ETowerStatus.ToStop;
            else if (isStop)    Status = (int)ETowerStatus.Stop;
            if      (isSPC)     Status = (int)ETowerStatus.SPCAlarm;    //220609 pjh : SPC Alarm은 다른 Tower Control과 별개로 동작

            CTwr.It.TowerStatus(Status);
        }

        public bool InspectHomeEnd()
        {
            bool IsOk = true;

            if (CMot.It.Chk_HDI((int)EAx.OnLoader_X       ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OnLoader_Y       ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OnLoader_Z       ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_X ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z ) != 1) {/**/ IsOk = false;}
            //20190604 ghk_onpbcr
            if(CDataOption.CurEqu == eEquType.Pikcer)
            {
                if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Y) != 1) {/**/ IsOk = false; }
            }
            //
            if (CMot.It.Chk_HDI((int)EAx.Inrail_X         ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.Inrail_Y         ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.LeftGrindZone_X  ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.LeftGrindZone_Y  ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.LeftGrindZone_Z  ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.RightGrindZone_X ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.RightGrindZone_Y ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.RightGrindZone_Z ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_X) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.DryZone_X        ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.DryZone_Z        ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.DryZone_Air      ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OffLoader_X      ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OffLoader_Y      ) != 1) {/**/ IsOk = false;}
            if (CMot.It.Chk_HDI((int)EAx.OffLoader_Z      ) != 1) {/**/ IsOk = false;}

            return IsOk;
        }

        public void LotInfoClear()  // CSQ_Main
        {
            //랏 종료 시점
            //CData.swLotJam.Stop();
            CData.bLotJam = false;
            //CData.swLotIdle.Stop();
            CData.bLotIdle = false;
            //CData.swLotTotal.Stop();
            CData.bLotTotal = false;

            if (CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_WorkEnd)
            {
                CData.SpcInfo.sEndTime = DateTime.Now.ToString("HH:mm:ss");
                //CData.SpcInfo.sTotalTime = CData.swLotTotal.Elapsed.ToString(@"hh\:mm\:ss");

                //20190624 josh
                //jsck secsgem
                //svid 999103 lot ended time
                if (CData.GemForm != null) 
                {
                    CData.GemForm.Set_SVID(Convert.ToInt32(SECSGEM.JSCK.eSVID.LOT_Ended_Time), DateTime.Now.ToString("yyyyMMddHHmmss"));
                }

                // 2020.12.10 JSKim St
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    CErr.SaveErrLog_LotInfo();
                }
                // 2020.12.10 JSKim Ed
                CSpc.It.SaveLotInfo();

                CData.SpcInfo.sLotName  = "";
                CData.SpcInfo.sDevice   = "";
            }

            //190325 ksg : Qc
            int iMax;

            if (CDataOption.UseQC && CData.Opt.bQcUse)  iMax = (int)EPart.OFR;
            else                                        iMax = (int)EPart.OFL;

            for (int i = 0; i <= iMax; i++)
            {
                CData.Parts[i].iStat    = ESeq.Idle;
                CData.Parts[i].sLotName = "";
                CData.Parts[i].iMGZ_No  = 0;
                CData.Parts[i].iSlot_No = 0;

                for (int j = 0; j < CData.Dev.iMgzCnt; j++)
                {
                    CData.Parts[i].iSlot_info[j] = 0;
                }

                //CData.Parts[i].dMeaBf = 0;
                //CData.Parts[i].dMeaAf = 0;
                CData.Parts[i].sBcr         = "";
                CData.Parts[i].bBcr         = false;
                CData.Parts[i].bOri         = false;
                CData.Parts[i].bExistStrip  = false;
                CData.Parts[i].iStripStat   = 0;

                CData.Parts[i].b18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: Lot Open 후 첫 n개의 스트립(Leading Strip)인지 여부 (Before Measure 시작 기준)

                //랏 종료 시점
                //CData.swLotJam.Stop();
                CData.bLotJam = false;
                //CData.swLotIdle.Stop();
                CData.bLotIdle = false;
                //CData.swLotTotal.Stop();
                CData.bLotTotal = false;

                CData.SpcInfo.sEndTime = DateTime.Now.ToString("HH:mm:ss");
                //CData.SpcInfo.sTotalTime = CData.swLotTotal.Elapsed.ToString(@"hh\:mm\:ss");

                //20190624 josh
                //jsck secsgem
                //svid 999103 lot ended time
                if (CData.GemForm != null)
                {
                    CData.GemForm.Set_SVID(Convert.ToInt32(SECSGEM.JSCK.eSVID.LOT_Ended_Time), DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
            }
        }

        public void BeltRun()
        {
            if (CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_BtmPick  ||
                CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_BtmPlace ||
                CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_TopPick  ||
                CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_TopPlace ||
                CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_Pick     ||
                CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_Place)
            {
                return;
            }

            if (m_iStat == EStatus.Manual) return;
            
            bool OnlBelt    = CIO.It.Get_X(eX.ONL_BtnBeltRun    );
            bool OflBeltTop = CIO.It.Get_X(eX.OFFL_BtnBeltTopRun);
            bool OflBeltBtm = CIO.It.Get_X(eX.OFFL_BtnBeltBtmRun);

            CSq_OnL.It.Func_TopBeltRun(OnlBelt   );
            CSq_OfL.It.Belt_Top  (OflBeltTop);
            CSq_OfL.It.Belt_Btm  (OflBeltBtm);
        }

        public bool CheckDoor()
        {
            bool Lock = true;

            if(CData.Opt.bDoorSkip) return Lock;

            bool bxFrontDoor  = CIO.It.Get_X(eX.GRD_DoorFront ); // false이면 Open
            bool bxRearDoor   = CIO.It.Get_X(eX.GRD_DoorRear  );
            bool bxOnLFDoor   = CIO.It.Get_X(eX.ONL_DoorFront );
            bool bxOnLLDoor   = CIO.It.Get_X(eX.ONL_DoorLeft  );
            bool bxOnLRDoor   = CIO.It.Get_X(eX.ONL_DoorRear  );
            bool bxOfLFDoor   = CIO.It.Get_X(eX.OFFL_DoorFront);
            bool bxOfLLDoor   = CIO.It.Get_X(eX.OFFL_DoorRight);
            bool bxOfLRDoor   = CIO.It.Get_X(eX.OFFL_DoorRear );


            if(!bxFrontDoor ) Lock = false;
            if(!bxRearDoor  ) Lock = false;
            if(!bxOnLFDoor  ) Lock = false;
            if(!bxOnLLDoor  ) Lock = false;
            if(!bxOnLRDoor  ) Lock = false;
            if(!bxOfLFDoor  ) Lock = false;
            if(!bxOfLLDoor  ) Lock = false;
            if(!bxOfLRDoor  ) Lock = false;

            // 201116 jym START : QC 도어 연동 추가
            if (CDataOption.UseQC && CData.Opt.bQcUse && !CData.Opt.bQcDoor)
            {
                if (!CIO.It.Get_X(eX.OFL_QcFrontDoor))
                { Lock = false; }
                if (!CIO.It.Get_X(eX.OFL_QcRearDoor))
                { Lock = false; }
            }
            // 201116 jym END

            return Lock;
        }

        public bool CheckCover()
        {
            bool Lock = true;

            if(CData.Opt.bCoverSkip) return Lock;

            bool bxFrontCover = CIO.It.Get_X(eX.GRD_CoverFront);
            bool bxRearCover  = CIO.It.Get_X(eX.GRD_CoverRear );

            if(!bxFrontCover) Lock = false;
            if(!bxRearCover ) Lock = false;

            return Lock;
        }

        public void DoorLock(bool Lock)
        {
            if(CData.Opt.bDoorSkip) return;

            // 2021-01-11, YYY, For New QC-Vision
            if (CDataOption.UseQC && CData.Opt.bQcUse)
            {
                //if (CTcpIp.It.IsConnect() && CTcpIp.It.bIsConnect)
                if (CGxSocket.It.IsConnected())
                {
                    if (m_bCheckLock != Lock || m_bFirstLock == false)
                    {
                        //CTcpIp.It.SendDoorLock(Lock);
                        CGxSocket.It.SendMessage("DoorLock");

                        _SetLog("[SEND](EQ->QC) DoorLock");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                        m_bCheckLock = Lock;
                        m_bFirstLock = true;
                    }
                }
            }


            CIO.It.Set_Y(eY.GRD_DoorLock , Lock);
            CIO.It.Set_Y(eY.ONL_DoorLock , Lock);
            CIO.It.Set_Y(eY.OFFL_DoorLock, Lock);
        }

        //190213 ksg :
        public bool CheckEmergency()
        {
            bool IsOk = true;

            bool bFrontEmg = (CIO.It.Get_X(eX.SYS_EMGFront_L)) || 
                             (CIO.It.Get_X(eX.SYS_EMGFront_R)) || 
                             (CIO.It.Get_X(eX.SYS_EMGRear_L )) || 
                             (CIO.It.Get_X(eX.SYS_EMGRear_R )) ||
                             (CIO.It.Get_X(eX.ONL_EMGFront  )) ||
                             (CIO.It.Get_X(eX.ONL_EMGRear   )) ||
                             (CIO.It.Get_X(eX.OFFL_EMGFront )) ||
                             (CIO.It.Get_X(eX.OFFL_EMGRear  )) ;

            //if(bFrontEmg) IsOk = false;
            if (bFrontEmg)
            {
                if (m_iStat != EStatus.Error)
                {
                    m_iStat = EStatus.Error;
                    if (CIO.It.Get_X(eX.SYS_EMGFront_L)) { CErr.Show(eErr.SYSTEM_MAIN_EMG_FRONT_LEFT); }
                    else if (CIO.It.Get_X(eX.SYS_EMGFront_R)) { CErr.Show(eErr.SYSTEM_MAIN_EMG_FRONT_RIGHT); }
                    else if (CIO.It.Get_X(eX.SYS_EMGRear_L)) { CErr.Show(eErr.SYSTEM_MAIN_EMG_REAR_LEFT); }
                    else if (CIO.It.Get_X(eX.SYS_EMGRear_R)) { CErr.Show(eErr.SYSTEM_MAIN_EMG_REAR_RIGHT); }
                    else if (CIO.It.Get_X(eX.ONL_EMGFront)) { CErr.Show(eErr.SYSTEM_ONLOADER_EMG_FRONT); }
                    else if (CIO.It.Get_X(eX.ONL_EMGRear)) { CErr.Show(eErr.SYSTEM_ONLOADER_EMG_REAR); }
                    else if (CIO.It.Get_X(eX.OFFL_EMGFront)) { CErr.Show(eErr.SYSTEM_OFFLOADER_EMG_FRONT); }
                    else if (CIO.It.Get_X(eX.OFFL_EMGRear)) { CErr.Show(eErr.SYSTEM_OFFLOADER_EMG_REAR); }
                    
                    eAutoStep = eAutoStatus.idle;
                    Stop();
                }
                // 201002 jym : EMO 버튼 누를 시 런 상태 강제 해제 (도어락 풀림 기능)
                m_bRun = false;
                IsOk = false;
            }
            

            return IsOk;
        }
        //190522 koo
        public bool CheckPump()
        {
            if (m_iStat == EStatus.Error) return false;
            bool IsOk = false;
            bool bFailLeftPump  = false;
            bool bFailRightPump = false;
            if(CIO.It.Get_Y(eY.PUMPL_Run))
            {
                //bFailLeftPump  = (!CIO.It.Get_X(eX.PUMPL_Run)) || (!CIO.It.Get_X(eX.PUMPL_FlowLow)) || (!CIO.It.Get_X(eX.PUMPL_TempHigh  )) || (!CIO.It.Get_X(eX.PUMPL_OverLoad));
                bFailLeftPump  = (!CIO.It.Get_X(eX.PUMPL_FlowLow)) || (!CIO.It.Get_X(eX.PUMPL_TempHigh  )) || (!CIO.It.Get_X(eX.PUMPL_OverLoad));
                if(bFailLeftPump) 
                {
                    //     if(!CIO.It.Get_X(eX.PUMPL_Run     )) { CErr.Show(eErr.LEFT_PUMP_RUN_ERROR      );}
                    //if(!CIO.It.Get_X(eX.PUMPL_FlowLow )) { CErr.Show(eErr.LEFT_PUMP_FLOW_LOW_ERROR );} //190903 ksg : 여기서 Error 확인하면 설비 처음 키고 Run시 에러 발생이 무조건 한번 발생 됨
                    //else if(!CIO.It.Get_X(eX.PUMPL_TempHigh)) { CErr.Show(eErr.LEFT_PUMP_TEMP_HIGH_ERROR);} 
                    //     if(!CIO.It.Get_X(eX.PUMPL_TempHigh)) { CErr.Show(eErr.LEFT_PUMP_TEMP_HIGH_ERROR);}
                    if(!CIO.It.Get_X(eX.PUMPL_TempHigh)) //200210 ksg :
                    {
                        CIO.It.Set_Y(eY.PUMPL_Run, false);
                        CErr.Show(eErr.LEFT_PUMP_TEMP_HIGH_ERROR);
                    }
                    else if(!CIO.It.Get_X(eX.PUMPL_OverLoad)) { CErr.Show(eErr.LEFT_PUMP_OVERLOAD_ERROR );}
                }
            }
            if(CIO.It.Get_Y(eY.PUMPR_Run))
            {
                //bFailRightPump = (!CIO.It.Get_X(eX.PUMPR_Run)) || (!CIO.It.Get_X(eX.PUMPR_FlowLow)) || (!CIO.It.Get_X(eX.PUMPR_TempHigh  )) || (!CIO.It.Get_X(eX.PUMPR_OverLoad));
                bFailRightPump = (!CIO.It.Get_X(eX.PUMPR_FlowLow)) || (!CIO.It.Get_X(eX.PUMPR_TempHigh  )) || (!CIO.It.Get_X(eX.PUMPR_OverLoad));
                if(bFailRightPump)
                {
                    //     if(!CIO.It.Get_X(eX.PUMPR_Run     )) { CErr.Show(eErr.RIGHT_PUMP_RUN_ERROR      );}
                    //if(!CIO.It.Get_X(eX.PUMPR_FlowLow )) { CErr.Show(eErr.RIGHT_PUMP_FLOW_LOW_ERROR );} //190903 ksg : 여기서 Error 확인하면 설비 처음 키고 Run시 에러 발생이 무조건 한번 발생 됨
                    //else if(!CIO.It.Get_X(eX.PUMPR_TempHigh)) { CErr.Show(eErr.RIGHT_PUMP_TEMP_HIGH_ERROR);}
                    //     if(!CIO.It.Get_X(eX.PUMPR_TempHigh)) { CErr.Show(eErr.RIGHT_PUMP_TEMP_HIGH_ERROR);} 
                    if(!CIO.It.Get_X(eX.PUMPR_TempHigh)) //200210 ksg :
                    {
                        CIO.It.Set_Y(eY.PUMPR_Run, false);
                        CErr.Show(eErr.RIGHT_PUMP_TEMP_HIGH_ERROR);
                    } 
                    else if(!CIO.It.Get_X(eX.PUMPR_OverLoad)) { CErr.Show(eErr.RIGHT_PUMP_OVERLOAD_ERROR );}
                }
            }
            IsOk = !bFailLeftPump && !bFailRightPump;
            return IsOk;
        }

        // 2020.10.22 JSKim St
        public bool CheckAddPump()
        {
            if (m_iStat == EStatus.Error) return false;
            bool IsOk = false;
            bool bFailLeftAddPump = false;
            bool bFailRightAddPump = false;
            if (CIO.It.Get_Y(eY.PUMPL_Run))
            {
                bFailLeftAddPump = (!CIO.It.Get_X(eX.ADD_PUMPL_FlowLow)) || (!CIO.It.Get_X(eX.ADD_PUMPL_TempHigh)) || (!CIO.It.Get_X(eX.ADD_PUMPL_OverLoad));
                if (bFailLeftAddPump)
                {
                    if (!CIO.It.Get_X(eX.ADD_PUMPL_TempHigh))
                    {
                        CIO.It.Set_Y(eY.PUMPL_Run, false);
                        CErr.Show(eErr.LEFT_ADD_PUMP_TEMP_HIGH_ERROR);
                    }
                    else if (!CIO.It.Get_X(eX.ADD_PUMPL_OverLoad)) { CErr.Show(eErr.LEFT_ADD_PUMP_OVERLOAD_ERROR); }
                }
            }
            if (CIO.It.Get_Y(eY.PUMPR_Run))
            {
                bFailRightAddPump = (!CIO.It.Get_X(eX.ADD_PUMPR_FlowLow)) || (!CIO.It.Get_X(eX.ADD_PUMPR_TempHigh)) || (!CIO.It.Get_X(eX.ADD_PUMPR_OverLoad));
                if (bFailRightAddPump)
                {
                    if (!CIO.It.Get_X(eX.ADD_PUMPR_TempHigh))
                    {
                        CIO.It.Set_Y(eY.PUMPR_Run, false);
                        CErr.Show(eErr.RIGHT_ADD_PUMP_TEMP_HIGH_ERROR);
                    }
                    else if (!CIO.It.Get_X(eX.ADD_PUMPR_OverLoad)) { CErr.Show(eErr.RIGHT_ADD_PUMP_OVERLOAD_ERROR); }
                }
            }
            IsOk = !bFailLeftAddPump && !bFailRightAddPump;
            return IsOk;
        }
        // 2020.10.22 JSKim Ed

        public bool CheckPumpRun()
        {
            if (m_iStat == EStatus.Error)
            { return false; }

            bool IsOk = false;
            bool bLPumpRun = true;
            bool bRPumpRun = true;

            if (CIO.It.Get_Y(eY.PUMPL_Run) && !CData.Opt.aTblSkip[(int)EWay.L])
            {
                bLPumpRun = CIO.It.Get_X(eX.PUMPL_Run);
                if (!bLPumpRun)
                { CErr.Show(eErr.LEFT_PUMP_RUN_ERROR); }
                // 201019 jym : Pump running 중에도 Flow low check
                if (!CIO.It.Get_X(eX.PUMPL_FlowLow))
                { CErr.Show(eErr.LEFT_PUMP_FLOW_LOW_ERROR); }
            }
            if (CIO.It.Get_Y(eY.PUMPR_Run) && !CData.Opt.aTblSkip[(int)EWay.R])
            {
                bRPumpRun = CIO.It.Get_X(eX.PUMPR_Run);
                if (!bRPumpRun)
                { CErr.Show(eErr.RIGHT_PUMP_RUN_ERROR); }
                // 201019 jym : Pump running 중에도 Flow low check
                if (!CIO.It.Get_X(eX.PUMPR_FlowLow))
                { CErr.Show(eErr.RIGHT_PUMP_FLOW_LOW_ERROR); }
            }

            IsOk = bLPumpRun && bRPumpRun;
            return IsOk;
        }

        // 2020.10.22 JSKim St
        public bool CheckAddPumpRun()
        {
            if (m_iStat == EStatus.Error)
            { return false; }

            bool IsOk = false;
            bool bLAddPumpRun = true;
            bool bRAddPumpRun = true;

            if (CIO.It.Get_Y(eY.PUMPL_Run) && !CData.Opt.aTblSkip[(int)EWay.L])
            {
                bLAddPumpRun = CIO.It.Get_X(eX.ADD_PUMPL_Run);
                if (!bLAddPumpRun)
                { CErr.Show(eErr.LEFT_ADD_PUMP_RUN_ERROR); }
                // 201019 jym : Pump running 중에도 Flow low check
                if (!CIO.It.Get_X(eX.ADD_PUMPL_FlowLow))
                { CErr.Show(eErr.LEFT_ADD_PUMP_FLOW_LOW_ERROR); }
            }
            if (CIO.It.Get_Y(eY.PUMPR_Run) && !CData.Opt.aTblSkip[(int)EWay.R])
            {
                bRAddPumpRun = CIO.It.Get_X(eX.ADD_PUMPR_Run);
                if (!bRAddPumpRun)
                { CErr.Show(eErr.RIGHT_ADD_PUMP_RUN_ERROR); }
                // 201019 jym : Pump running 중에도 Flow low check
                if (!CIO.It.Get_X(eX.ADD_PUMPR_FlowLow))
                { CErr.Show(eErr.RIGHT_ADD_PUMP_FLOW_LOW_ERROR); }
            }

            IsOk = bLAddPumpRun && bRAddPumpRun;
            return IsOk;
        }
        // 2020.10.22 JSKim Ed

        public bool InspectEmergency()
        {
            bool IsOk = true;
            //emergency X03~ X06
            bool bFrontEmg = (CIO.It.Get_X(eX.SYS_EMGFront_L)) || 
                             (CIO.It.Get_X(eX.SYS_EMGFront_R)) || 
                             (CIO.It.Get_X(eX.SYS_EMGRear_L )) || 
                             (CIO.It.Get_X(eX.SYS_EMGRear_R )) ||
                             (CIO.It.Get_X(eX.ONL_EMGFront  )) ||
                             (CIO.It.Get_X(eX.ONL_EMGRear   )) ||
                             (CIO.It.Get_X(eX.OFFL_EMGFront )) ||
                             (CIO.It.Get_X(eX.OFFL_EMGRear  )) ;

            int m_iZL = (int)EAx.LeftGrindZone_Z ;
            int m_iZR = (int)EAx.RightGrindZone_Z;

            if(bFrontEmg)
            {
                CMot.It.Mv_N(m_iZL, CData.SPos.dGRD_Z_Wait[0]);
                CMot.It.Mv_N(m_iZR, CData.SPos.dGRD_Z_Wait[1]);
                CIO.It.Set_Y(eY.GRDL_ProbeDn, false);
                CIO.It.Set_Y(eY.GRDR_ProbeDn, false);
                CSQ_Man.It.WaterOnOff(false); //syc : All Water On/Off
                CSQ_Man.It.StopWater(); //200616 pjh : 

                //if (CData.WMX)
                //{
                //    CSpl.It.Write_Stop(EWay.L);
                //    CSpl.It.Write_Stop(EWay.R);
                //}
                // 2023.03.15 Max
                CSpl_485.It.Write_Stop(EWay.L);
                CSpl_485.It.Write_Stop(EWay.R);

                IsOk = false;
            }
            return IsOk;
        }

        //190109 ksg : Leak Sensor 추가
        public bool CheckLeakSensor()
        {
            bool IsOk = true;

            if(CData.Opt.bLeakSkip) return IsOk;
            //if(CData.CurCompany != eCompany.AseKr) return IsOk; //190227 ksg
            //if((CData.CurCompany != eCompany.AseKr) && (CData.CurCompany != eCompany.JSCK)) return IsOk; //190227 ksg
            if(CDataOption.ChkGrdLeak == eChkGrdLeak.NotUse) return IsOk; //191203 ksg :
            //sensor 접점 확인 할 것.
            if(CIO.It.Get_X(eX.GRL_Leak)) IsOk = false;
            if(CIO.It.Get_X(eX.GRR_Leak)) IsOk = false;
            return IsOk;
        }
        //190417 ksg :
        public bool ChkDryLeakSensor()
        {
            bool IsOk = true;

            if(CData.Opt.bLeakSkip) return IsOk;
            //if(CData.CurCompany != eCompany.JSCK) return IsOk; //190227 ksg
            if(CDataOption.ChkDryLeak == eChkDryLeak.NotUse) return IsOk;
            //sensor 접점 확인 할 것.
            if(CIO.It.Get_X(eX.DRY_Leak)) IsOk = false;
            return IsOk;
        }

        //190110 ksg : Warm Up
        public bool CheckWarmUp()
        {
            bool IsOk = true;

            DateTime Now ;
            TimeSpan Diff;

            Now = Convert.ToDateTime(DateTime.Now);

            Diff = Now - CData.WarmUpS;

            //190228 ksg :
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                if(CData.Opt.bWarmUpSkip) return IsOk;
            }
            if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.Dev.bMeasureMode) return IsOk;
            }

            double dInterval = 30;
            if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) 
            {
                dInterval = CData.Opt.iWmUI;
            }

            if (Diff.TotalMinutes > dInterval)
            {
                IsOk = false;
            }            
            return IsOk;
        }

        //190221 ksg : Warm Up Time
        public TimeSpan WarmUpTime()
        {
            DateTime Now ;
            TimeSpan Diff;

            Now   = DateTime.Now;

            Diff = Now - CData.WarmUpS;

            return Diff;
        }

        public bool CheckRecipe()
        {
            bool IsOk = true;
            /*
            if (CData.Dev.bBcrSkip && CData.Dev.bOriSkip) return IsOk;
            CBcr.It.LoadRecipe();
            if(CData.Bcr.sDX != CData.Bcr.sBCR) IsOk = false;
            */

            if(!CData.Dev.bOriSkip)
            {
                CBcr.It.LoadRecipe();
                if(CData.Bcr.sDX != CData.Bcr.sBCR) IsOk = false;
            }
            else 
            {
                return IsOk;
            }
            return IsOk;
        }

        //190521 ksg :
        public void SaveWarmUpTime(int iTime)
        {
            CData.Opt.iWmUT = iTime;
            string sPath = GV.PATH_CONFIG + "Option.cfg";
            string sSec = "";

            CIni mIni = new CIni(sPath);
            sSec = "Warm Up";
            mIni.Write(sSec, "Time", CData.Opt.iWmUT);
        }

        //190522 ksg :
        public bool CheckWheelChange()
        {
            bool Ret = false;

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if (CData.CurCompany != ECompany.Qorvo && (CData.CurCompany != ECompany.Qorvo_DZ) && CData.CurCompany != ECompany.SST) return Ret = false; //191202 ksg :
            if (CData.CurCompany != ECompany.Qorvo && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&
                CData.CurCompany != ECompany.SST)
                return Ret = false;

            if (CData.WhlChgSeq.bStart || CData.DrsChgSeq.bStart)   Ret = true ;
            else                                                    Ret = false;

            return Ret;
        }
        //190711 ksg : 
        public int CheckWeelLimit()
        {
            //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC)
            if((CData.CurCompany == ECompany.ASE_KR) && (CData.bWhlDrsLimitAlarmSkip == true))
            { return (0); }
            //

            int result = 0;

            if(CData.Whls[0].dWhlLimit > CData.Whls[0].dWhlAf) result = 1;
            if(CData.Whls[1].dWhlLimit > CData.Whls[1].dWhlAf) result = 2;

            return result;
        }
        //190714 ksg :
        public int ChkDrsLimit()
        {
            //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC)
            if((CData.CurCompany == ECompany.ASE_KR) && (CData.bWhlDrsLimitAlarmSkip == true))
            { return 0; }
            //

            int iWy;
            int result = 0;
            double dTempDrsT = 0.0;

            iWy = (int)EWay.L;
            if (CData.DrData[iWy].bDrsR)
            { dTempDrsT = CData.Whls[iWy].aNewP [0].dTotalDep + CData.Whls[iWy].aNewP [1].dTotalDep; }
            else
            { dTempDrsT = CData.Whls[iWy].aUsedP[0].dTotalDep + CData.Whls[iWy].aUsedP[1].dTotalDep; }

            if (CData.Whls[iWy].dDrsLimit > (CData.Whls[iWy].dDrsAf - dTempDrsT))
            {
                result = 1;
            }

            iWy = (int)EWay.R;
            if (CData.DrData[iWy].bDrsR)
            { dTempDrsT = CData.Whls[iWy].aNewP [0].dTotalDep + CData.Whls[iWy].aNewP [1].dTotalDep; }
            else
            { dTempDrsT = CData.Whls[iWy].aUsedP[0].dTotalDep + CData.Whls[iWy].aUsedP[1].dTotalDep; }

            if (CData.Whls[iWy].dDrsLimit > (CData.Whls[iWy].dDrsAf - dTempDrsT))
            {
                result = 2;
            }

            return result;
        }

        /// <summary>
        /// SCK Site에 SECSGEM 사용시 DF 사용이 필수 
        /// </summary>
        /// Return true : Error,  false : pass
        /// <returns></returns>
        private int Chk_SECSGEM_Par()
        {
            int nret_val = 0;
            if (CData.Opt.bSecsUse != true) return nret_val;

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)  // lhs : 이 부분 다시 확인 필요.
            {
                // 2021.08.06 lhs Start 
                if (CDataOption.UseNewSckGrindProc)
                {
                    if (CData.Dev.bDynamicSkip == false) // DF 사용  
                    {
                        if (CData.Dev.eMoldSide != ESide.Top && CData.Dev.eMoldSide != ESide.BtmS) nret_val = -1;/// Error
                    }
                    else  // DF 미사용
                    {
                        if (CData.Dev.eMoldSide != ESide.Btm && CData.Dev.eMoldSide != ESide.TopD) nret_val = -2;/// Error
                    }
                }
                else  // 기존방식
                // 2021.08.06 lhs End
                {
                    /// SECSGEM : DF : TOP
                    ///  O      : O  : O
                    ///  O      : X  : X      
                    if (CData.Dev.bDynamicSkip == false) // DF 사용  
                    {
                        if (CData.Dev.eMoldSide != ESide.Top) nret_val = -1;/// Error
                    }
                    else  // DF 미사용
                    {
                        if (CData.Dev.eMoldSide != ESide.Btm) nret_val = -2;/// Error
                    }
                }
            }
            else if (CData.CurCompany == ECompany.ASE_KR)
            { //Tool(Wheel & Dresser) load 되지 않은 경우 장비 가동 안되도록 수정 20200430 LCY

                //201121 jhc : 설비에 입력된 tool 정보가 없어도 Local Mode에서는 설비 start될 수 있도록 수정
                if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
                {
                    nret_val = 0;
                }
                else
                {
                    if      (CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] == "")  {   nret_val = -3;  }   /// Left Need to Dress ID 
                    else if (CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] == "")  {   nret_val = -4;  }   /// Right Need to Dress ID 
                    else if (CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0] == "")   {   nret_val = -5;  }   /// Left Need to Wheel ID
                    else if (CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] == "")   {   nret_val = -6;  }   /// Right Need to Wheel ID
                }
            }                
            return nret_val;
        }

        //190714 ksg : AutoRun $ Manual Run시 Check
        public bool ChkAllHomeDone()
        {
            bool Ret = true;

            //190624 ksg : HomeDone 보는거 바꿈                 
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_X       ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_Y       ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_Z       ))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.Inrail_X         ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.Inrail_Y         ))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_X ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z ))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.LeftGrindZone_X  ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.LeftGrindZone_Y  ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.LeftGrindZone_Z  ))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.RightGrindZone_X ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.RightGrindZone_Y ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.RightGrindZone_Z ))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_X))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.DryZone_X        ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.DryZone_Z        ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.DryZone_Air      ))) Ret = false;

            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoader_X      ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoader_Y      ))) Ret = false;
            if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoader_Z      ))) Ret = false;

            if(CDataOption.CurEqu == eEquType.Pikcer) if(!Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Y))) Ret = false;

            return Ret; 
        }
        //190714 ksg : Motor Alram 추가
        public void ChkMotorAlram()
        {
            bool Ret     = false;
            bool Left_Z  = false;
            bool Right_Z = false;
            if(CSQ_Main.It.m_iStat != EStatus.Error && CData.WMX)
            {
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OnLoader_X       ))) { CErr.Show(eErr.ONLOADER_X_AXIS_MOTOR_ALRAM        ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OnLoader_Y       ))) { CErr.Show(eErr.ONLOADER_Y_AXIS_MOTOR_ALRAM        ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OnLoader_Z       ))) { CErr.Show(eErr.ONLOADER_Z_AXIS_MOTOR_ALRAM        ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.Inrail_X         ))) { CErr.Show(eErr.INRAIL_X_AXIS_MOTOR_ALRAM          ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.Inrail_Y         ))) { CErr.Show(eErr.INRAIL_Y_AXIS_MOTOR_ALRAM          ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OnLoaderPicker_X ))) { CErr.Show(eErr.ONLOADER_PICKER_X_AXIS_MOTOR_ALRAM ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OnLoaderPicker_Z ))) { CErr.Show(eErr.ONLOADER_PICKER_Z_AXIS_MOTOR_ALRAM ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.LeftGrindZone_X  ))) { CErr.Show(eErr.LEFT_GRIND_X_AXIS_MOTOR_ALRAM      ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.LeftGrindZone_Y  ))) { CErr.Show(eErr.LEFT_GRIND_Y_AXIS_MOTOR_ALRAM      ); Ret = true; Left_Z = true; }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.LeftGrindZone_Z  ))) { CErr.Show(eErr.LEFT_GRIND_Z_AXIS_MOTOR_ALRAM      ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.RightGrindZone_X ))) { CErr.Show(eErr.RIGHT_GRIND_X_AXIS_MOTOR_ALRAM     ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.RightGrindZone_Y ))) { CErr.Show(eErr.RIGHT_GRIND_Y_AXIS_MOTOR_ALRAM     ); Ret = true; Right_Z = true;}
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.RightGrindZone_Z ))) { CErr.Show(eErr.RIGHT_GRIND_Z_AXIS_MOTOR_ALRAM     ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OffLoaderPicker_X))) { CErr.Show(eErr.OFFLOADER_PICKER_X_AXIS_MOTOR_ALRAM); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OffLoaderPicker_Z))) { CErr.Show(eErr.OFFLOADER_PICKER_Z_AXIS_MOTOR_ALRAM); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.DryZone_X        ))) { CErr.Show(eErr.DRY_X_AXIS_MOTOR_ALRAM             ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.DryZone_Z        ))) { CErr.Show(eErr.DRY_Z_AXIS_MOTOR_ALRAM             ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.DryZone_Air      ))) { CErr.Show(eErr.DRY_AIR_AXIS_MOTOR_ALRAM           ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OffLoader_X      ))) { CErr.Show(eErr.OFFLOADER_X_AXIS_MOTOR_ALRAM       ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OffLoader_Y      ))) { CErr.Show(eErr.OFFLOADER_Y_AXIS_MOTOR_ALRAM       ); Ret = true;                }
                if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OffLoader_Z      ))) { CErr.Show(eErr.OFFLOADER_Z_AXIS_MOTOR_ALRAM       ); Ret = true;                }
                if(CDataOption.CurEqu == eEquType.Pikcer) if(Convert.ToBoolean(CMot.It.Chk_AlrI((int)EAx.OnLoader_Y))) { CErr.Show(eErr.ONLOADER_PICKER_Y_AXIS_MOTOR_ALRAM  ); Ret = true;}

                if(Ret)
                {
                    if(Left_Z)
                    {
                        //CSpl.It.Write_Stop(EWay.L);
                        // 2023.03.15 Max
                        CSpl_485.It.Write_Stop(EWay.L);
                    }
                    if(Right_Z)
                    {
                        //CSpl.It.Write_Stop(EWay.R);
                        // 2023.03.15 Max
                        CSpl_485.It.Write_Stop(EWay.R);
                    }
                }
            }
        }

        //20190930 ghk_autowarmup
        public void Chk_AutoWarmUp()
        {
            if (    (m_iStat == EStatus.Idle || m_iStat == EStatus.Stop) && 
                    ChkAllHomeDone()                && 
                    CMot.It.Chk_AllSrv()            &&
            	    !CIO.It.Get_X(eX.GRDL_TbVaccum) &&  // 진공이 아니다 -> 자재 없음
                    !CIO.It.Get_X(eX.GRDR_TbVaccum) && 
                    !bSetUpStripStatus)
            {
                if ((!CIO.It.Get_X(eX.GRD_DoorFront) || !CIO.It.Get_X(eX.GRD_DoorRear)) || CData.Opt.bAwSkip)  // 201029 jym : 조건추가, 오토웜엄 스킵이면 계속 시간 진행
                {
                    CData.AutoWarmPre = DateTime.Now;
                    CData.AutoWarmSpan = new TimeSpan();
                }
                else
                {
                    CData.AutoWarmNow = DateTime.Now;

                    CData.AutoWarmSpan = CData.AutoWarmNow - CData.AutoWarmPre;
                }

                // 201008 jym : Auto warm up 시간 추가
                if (CData.CurCompany == ECompany.SkyWorks)
                {
                    if (CData.AutoWarmSpan.TotalMinutes > CData.Opt.iAWP)
                    {
                        CSQ_Man.It.Seq = ESeq.Warm_Up;
                        CData.IsATW = true;
                    }
                }
                else
                {
                    if (CData.AutoWarmSpan.Hours > 0)
                    { CSQ_Man.It.Seq = ESeq.Warm_Up; }
                }
            }
            else
            {
                CData.AutoWarmPre = DateTime.Now;
                CData.AutoWarmSpan = new TimeSpan();
            }
        }
        //

        //20191121 ghk_display_strip
        /// <summary>
        /// ONL 매거진상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_ONL()
        {
            CData.Parts[(int)EPart.ONL].bNotMatch = CData.Parts[(int)EPart.ONL].bExistStrip != CSq_OnL.It.Chk_ClampMgz();
        }

        /// <summary>
        /// INR 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_INR()
        {
            CData.Parts[(int)EPart.INR].bNotMatch = CData.Parts[(int)EPart.INR].bExistStrip != CSq_Inr.It.Chk_Strip();
        }

        /// <summary>
        /// ONP 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_ONP()
        {
            CData.Parts[(int)EPart.ONP].bNotMatch = CData.Parts[(int)EPart.ONP].bExistStrip != CSq_OnP.It.Chk_Strip();
        }

        /// <summary>
        /// GRDL 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_GRDL()
        {
            //210901 syc : 2004U
            if (CDataOption.Use2004U)
            {
                CData.Parts[(int)EPart.GRDL].bNotMatch = CData.Parts[(int)EPart.GRDL].bExistStrip != (CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) && CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U));
            }
            //
            else
            {
                CData.Parts[(int)EPart.GRDL].bNotMatch = CData.Parts[(int)EPart.GRDL].bExistStrip != CIO.It.Get_X(eX.GRDL_TbVaccum);
            }
        }

        /// <summary>
        /// GRDR 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_GRDR()
        {
            //210901 syc : 2004U
            if (CDataOption.Use2004U)
            {
                CData.Parts[(int)EPart.GRDR].bNotMatch = CData.Parts[(int)EPart.GRDR].bExistStrip != (CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) && CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U));
            }
            //
            else
            {
                CData.Parts[(int)EPart.GRDR].bNotMatch = CData.Parts[(int)EPart.GRDR].bExistStrip != CIO.It.Get_X(eX.GRDR_TbVaccum);
            }            
        }

        /// <summary>
        /// OFP 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_OFP()
        {
            CData.Parts[(int)EPart.OFP].bNotMatch = CData.Parts[(int)EPart.OFP].bExistStrip != CSq_OfP.It.Chk_Strip();
        }

        /// <summary>
        /// OFL_TOP 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_OFL_TOP()
        {
            CData.Parts[(int)EPart.OFR].bNotMatch = CData.Parts[(int)EPart.OFR].bExistStrip != CSq_OfL.It.Get_TopMgzChk();
        }

        /// <summary>
        /// OFL_BTM 자재 상태와 센서 상태가 같지 않으면 bNotMatch true
        /// 같으면 false
        /// </summary>
        public void Chk_Match_OFL_BTM()
        {
            CData.Parts[(int)EPart.OFL].bNotMatch = CData.Parts[(int)EPart.OFL].bExistStrip != CSq_OfL.It.Get_BtmMgzChk();
        }

        /// <summary>
        /// DRY 자재 상태와 LOT OPEN로 점멸 표시
        /// LOT OPEN이 아닐때
        /// bExistStrip == true 일 경우 bNotMatch = true;
        /// bExistStrip == false 일 경우 bNotMatch = false;
        /// </summary>
        public void Chk_Match_DRY()
        {
            if (!CData.LotInfo.bLotOpen)
            {
                if (CData.Parts[(int)EPart.DRY].bExistStrip)
                {
                    CData.Parts[(int)EPart.DRY].bNotMatch = true;
                }
                else
                {
                    CData.Parts[(int)EPart.DRY].bNotMatch = false;
                }
            }
        }
        //

        public void ChkPumpAutoOff()
        {
            if (m_iStat == EStatus.Auto_Running || m_iStat == EStatus.Manual     || 
                m_iStat == EStatus.Warm_Up      || m_iStat == EStatus.All_Homing || 
                !CSQ_Man.It.bBtnShow                                               )
            {
                // Idle stopwatch stop
                CData.SwPump.Stop();
            }
            else
            {
                // Stopwatch 상태 확인
                if (!CData.SwPump.IsRunning && CData.PumpTime != 0)
                {
                    CData.SwPump.Reset();
                    CData.SwPump.Start();
                }
                else
                {
                    // 지정된 시간과 경과시간 비교
                    if (CData.SwPump.Elapsed.TotalMinutes > CData.PumpTime && CData.PumpTime != 0)
                    {   // 지정된 시간 이후 PCW off
                        if(!CIO.It.Get_X(eX.GRDL_TbVaccum)) CIO.It.Set_Y(eY.PUMPL_Run, false);
                        if(!CIO.It.Get_X(eX.GRDR_TbVaccum)) CIO.It.Set_Y(eY.PUMPR_Run, false);
                    }
                }
            }
        }

        public bool Chk_DI()
        {
            bool bRet = false;

            if (CIO.It.Get_X(eX.GRDL_TbFlow)        || CIO.It.Get_X(eX.GRDR_TbFlow)         ||
                CIO.It.Get_X(eX.GRDL_SplWater)      || CIO.It.Get_X(eX.GRDR_SplWater)       ||
                CIO.It.Get_X(eX.GRDL_SplBtmWater)   || CIO.It.Get_X(eX.GRDR_SplBtmWater)    ||
                CIO.It.Get_X(eX.GRDR_TopClnFlow)    || CIO.It.Get_X(eX.GRDR_TopClnFlow)     ||
                CIO.It.Get_X(eX.DRY_ClnBtmFlow))
            { bRet = true; }

            return bRet;
        }

#if true //201121 jhc : [추가] ASE-Kr (SECS/GEM Local Mode)+(DF Bottom) 설정 상태에서 START 버튼 누르면 Alarm
         //           : Mold 두께(DF Bottom, 낮은 값) 기준 Recipe로 Target 그라인딩 시,
         //           :  => (SECS/GEM 비 사용) 또는 (SECS/GEM Local Mode 사용) 경우 Host에서 EQ로 정보 전달이 없기에 자재 파손 위험!!!
        public int Chk_SECSGEM_DF_Bottom()
        {
            if (CData.CurCompany != ECompany.ASE_KR) { return (0); } //ASE-Kr만 체크

            int nRet = 0;

            if (CData.Dev.eMoldSide == ESide.Btm) //Mold 두께(DF Bottom, 낮은 값) 기준 Recipe로 Target 그라인딩 시 조건 체크
            {
                // 1. (SECS/GEM 비 사용)+(DF Bottom) 설정 상태 ==> eErr.BTM_MOLD_SECSGEM_OFF_ERROR
                if ((CDataOption.SecsUse == eSecsGem.NotUse) || !CData.Opt.bSecsUse)
                {
                    nRet = (-1);
                }
                // 2. (SECS/GEM Local Mode 사용)+(DF Bottom) 설정 상태 ==> eErr.BTM_MOLD_SECSGEM_LOCAL_ERROR
                else if ( (CDataOption.SecsUse == eSecsGem.Use) && CData.Opt.bSecsUse && (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local)) )
                {
                    nRet = (-2);
                } 
            }

            return nRet;
        }
#else
        //201014 jhc : ASE-Kr (SECS/GEM 비 사용)+(DF Bottom) 설정 상태에서 START 버튼 누르면 Alarm
        public bool Chk_SECSGEM_DF_Bottom()
        {
            if (CData.CurCompany != ECompany.ASE_KR) { return true; } //ASE-Kr만 체크

            bool bRet = true;

            //(SECS/GEM 비 사용)+(DF Bottom) 설정 상태 => Mold 두께(DF Bottom, 낮은 값) 기준 Recipe로 Target 그라인딩하면 자재 파손 위험!!!
            if ((CData.Dev.eMoldSide == ESide.Btm) && ((CDataOption.SecsUse == eSecsGem.NotUse) || !CData.Opt.bSecsUse)) { bRet = false; }

            return bRet;
        }
		#endif
        //

        public bool Chk_AllSrvOn()
        {//201118 pjh : All Servo On Check 함수
            bool bRet = true;

            if (!CMot.It.Chk_Srv((int)EAx.OnLoader_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.OnLoader_Y)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.OnLoader_Z)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.Inrail_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.Inrail_Y)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.OnLoaderPicker_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.OnLoaderPicker_Z)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.LeftGrindZone_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.LeftGrindZone_Y)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.LeftGrindZone_Z)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.RightGrindZone_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.RightGrindZone_Y)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.RightGrindZone_Z)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.OffLoaderPicker_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.OffLoaderPicker_Z)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.DryZone_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.DryZone_Z)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.DryZone_Air)) bRet = false;

            if (!CMot.It.Chk_Srv((int)EAx.OffLoader_X)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.OffLoader_Y)) bRet = false;
            if (!CMot.It.Chk_Srv((int)EAx.OffLoader_Z)) bRet = false;

            if (CDataOption.CurEqu == eEquType.Pikcer) if (!CMot.It.Chk_Srv((int)EAx.OnLoaderPicker_Y)) bRet = false;

            return bRet;
        }

        public bool Chk_EmptyMGZ()
        {// 210113 pjh : 빈 매거진 투입 시 Check하는 함수
            bool bRet = false;
            if(CSq_OnL.It.iSeq == ESeq.ONL_WorkEnd && CData.LotInfo.iTInCnt == 0)
            {
                if(CDataOption.UseQC && CData.Opt.bQcUse)
                {
                    CSq_OfL.It.m_GetQcWorkEnd = true; //Offloader에 자재 체크하는 부분에서 QC Workend 체크하게끔 되어있음
                }
                bRet = CSq_OfL.It.Get_StripNone();
            }
            return bRet;
        }
        #region 시퀸스 Trace 저장

        private static int mError_Flag;
        private void Trace_Log()
        {
            string sLog;
            if(CSQ_Main.It.m_iStat.ToString() != EStatus.Error.ToString()) mError_Flag = 0;
           // if(mError_Flag == 1) return;
            // Switch 상태 Start,Stop,Reset, L WZig,R WZig, Error 
            sLog = "[" + Convert.ToInt16(eAutoStep) + ", " +
                         Convert.ToInt16(CIO.It.Get_X(eX.SYS_BtnStart)) + ", " +
                         Convert.ToInt16(CIO.It.Get_X(eX.SYS_BtnStop)) + ", " +
                         Convert.ToInt16(CIO.It.Get_X(eX.SYS_BtnReset)) + ", " +
                         Convert.ToInt16(CIO.It.Get_X(eX.GRDL_WheelZig)) + ", " +
                         Convert.ToInt16(CIO.It.Get_X(eX.GRDR_WheelZig)) + "," +
                         "E:" + CData.iErrNo + "],";
            if ((CSQ_Man.It.Step.ToString() == ESeq.Idle.ToString()) &&
               (CSQ_Main.It.m_iStat.ToString() == EStatus.Idle.ToString())) return;
            // Manual Step . Machine 상태 , 
            sLog += "[" + CSQ_Man.It.Step.ToString() + "," + CSQ_Main.It.m_iStat.ToString() + "],"; 
            // Auto Run Step 
            sLog += "[" + CData.Parts[(int)EPart.ONL].iStat.ToString() + ":" + CSq_OnL.It.Step + "," +
                         CData.Parts[(int)EPart.INR].iStat.ToString() + ":" + CSq_Inr.It.Step + "," +
                         CData.Parts[(int)EPart.ONP].iStat.ToString() + ":" + CSq_OnP.It.Step + "," +
                         CData.Parts[(int)EPart.GRDL].iStat.ToString() + ":" + CData.L_GRD.Step + ":" + CData.L_GRD.GStep + "," +
                         CData.Parts[(int)EPart.GRDR].iStat.ToString() + ":" + CData.R_GRD.Step + ":" + CData.R_GRD.GStep + "," +
                         CData.Parts[(int)EPart.OFP].iStat.ToString() + ":" + CSq_OfP.It.Step + "," +
                         CData.Parts[(int)EPart.DRY].iStat.ToString() + ":" + CSq_Dry.It.Step + "," +
                         CData.Parts[(int)EPart.OFL].iStat.ToString() + ":" + CSq_OfL.It.Step + "],[";
            // Motor HD, Servo, Alarm, Inp
            for (int i = CMot.It.AxS; i < CMot.It.AxL + 1; i++) {
                sLog += CMot.It.Chk_HDI(i) + ":" + CMot.It.Chk_SrvI(i) + ":" + CMot.It.Chk_AlrI(i) + ":" + CMot.It.Chk_InPI(i) + ",";
            }
            //모터 엔코더
            sLog += "],[";
            for (int i = CMot.It.AxS; i < CMot.It.AxL + 1; i++) {
                sLog += Convert.ToString(CMot.It.GetBufEnc(i)) + ",";
            }
            //프로브 값
            sLog += CPrb.It.GetBufValProbe((int)EWay.L).ToString() + "," + CPrb.It.GetBufValProbe((int)EWay.R).ToString() + ",";
            //스핀들
            sLog += CData.Spls[0].iRpm.ToString() + "," + CData.Spls[0].dLoad.ToString() + "," + CData.Spls[1].iRpm.ToString() + "," + CData.Spls[1].dLoad.ToString() + "],";
            CLog.mLogSeq(sLog);

            if(CSQ_Main.It.m_iStat.ToString() == EStatus.Error.ToString()) mError_Flag = 1;
        }
        #endregion 

        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);

            CLog.Save_Log(eLog.None, eLog.AR, string.Format("[{0}()],Status : {1},Auto step : {2},{3}", sMth, m_iStat, eAutoStep, sMsg));
        }

        private void _SetLog(string sMsg, double dPos)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);
            sMsg += "  Pos : " + dPos + "mm";

            CLog.Save_Log(eLog.None, eLog.AR, string.Format("[{0}()],Status : {1},Auto step : {2},{3}", sMth, m_iStat, eAutoStep, sMsg));
        }
    }
}
