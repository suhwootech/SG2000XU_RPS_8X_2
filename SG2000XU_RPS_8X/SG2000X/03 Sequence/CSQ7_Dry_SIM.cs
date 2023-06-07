using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SG2000X
{
    class CSQ7_Dry_SIM : CStn<CSQ7_Dry_SIM>
    {
        private readonly int TIMEOUT = 80000;
        private readonly int DRY = (int)EPart.DRY;
        private static System.Timers.Timer m_TimerTO;//TimeOut checker timer
        private static int m_nTOErrorNum = 0;//

        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep = 0;
        private int m_iPreStep = 0;
        private int m_MsgSendCnt = 0; //190419 ksg :
        private int m_iDryCnt = 0; // 200314-mjy : Unit 모드시 Dry Count 체크

        private int m_iY = (int)EAx.DryZone_Air;// Knife Dryer 에 사용 됨
        private int m_iR = (int)EAx.DryZone_Air;
        private int m_iZ = (int)EAx.DryZone_Z;
        private int m_iX = (int)EAx.DryZone_X;
        private int m_iTrZ = (int)EAx.OffLoaderPicker_Z;
        private bool bTemp_Val;// Bool 로 임시 사용을 위한 변수
        private bool bTemp1_Val;// Bool 로 임시 사용을 위한 변수
        private int nTemp_Val; // Int 로 임시 사용을 위한 변수
        private double dTemp_Pos; // Double 로 임시 사용을 위한 변수

        private bool m_bReqStop = false;
        public bool m_bStickRun = false;


        /// <summary>
        /// Dry Zone Status : 현재 드라이존 상태
        /// </summary>
        public ESeq iSeq;

        /// <summary>
        /// Auto Run Flag : 오토런 중 드라이존 명령 플래그
        /// </summary>
        public ESeq SEQ = ESeq.Idle;

        /// <summary>
        /// 사이클 함수 자체의 타임아웃 타이머
        /// </summary>
        private CTim m_Timeout = new CTim();
        private CTim m_TickTime = new CTim();

        private CTim m_mSpl = new CTim();
        private CTim m_Delay = new CTim(); //190406 ksg :Qc
        private CTim m_Delay1 = new CTim(); //190406 ksg :Qc

        private System.Windows.Forms.Timer m_tmSimuation;

        //20191007 ghk_drystick_time
        private CTim m_mCylDelay = new CTim();
        //
        //private Timer t_Stick = new Timer();
        //190807 ksg : TackTime 
        private DateTime m_StartSeq;
        private DateTime m_EndSeq;
        private TimeSpan m_SeqTime;

        private DateTime m_tStTime;
        private TimeSpan m_tTackTime;

        /// <summary>
        /// 드라이 R축 클리닝 작업 종료 위치
        /// </summary>
        private double dPosR = 0.0;
        /// <summary>
        /// Air Knife 현재 카운트
        /// </summary>
        public int m_iCnt = 0;

        public bool m_DuringDryOut = false; //191107 ksg :

        //20200420 lks dry work 전 센서확인
        private bool isCheckSafety = false;

        private CSQ7_Dry_SIM()
        {
            if(!CData.Opt.bQcSimulation)
            {
                return;
            }
                m_bHD = false;

            //t_Stick.Interval = 100;
            //t_Stick.Tick += Run_Stick_tick;
            m_bStickRun = false;

            m_tmSimuation = new System.Windows.Forms.Timer();
            m_tmSimuation.Interval = 500;
            m_tmSimuation.Tick += _fun_Simulation_Tick;
            m_tmSimuation.Start();

            CData.Parts[(int)EPart.DRY].sLotName = "lotName";
            CData.Parts[(int)EPart.DRY].iMGZ_No= 21;
            //string SlotNum = iSlotNum.ToString();
            CData.Parts[(int)EPart.DRY].iSlot_No = 2; //200606 lks
            CDev.It.m_sGrp = "GR";
            CData.Dev.sName = "DVC";
            CData.Parts[(int)EPart.DRY].sBcr = "232qq87Ziral";

            m_TimerTO = new System.Timers.Timer();
            m_TimerTO.Interval = 3000;
            m_TimerTO.Elapsed += OnTimeOutEvent;
            m_TimerTO.AutoReset = true;
            m_TimerTO.Enabled = false;

        }

        public void startAutoRun()
        {
            if (CData.Opt.bQcSimulation)
            {
                vmMan_13QcVision.m_qcVision.addMsgList(">>> Simulation Mode [Start Polliing]");
                m_tmSimuation.Start();
            }
        }

        private void _fun_Simulation_Tick(object sender, EventArgs e)
        {
            AutoRun();
            CSQ8_OfL_SIM.It.AutoRun();
        }
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="sMsg"></param>
        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);

            //200824 jhc : 
            string sStat = CSQ_Main.It.m_iStat.ToString();
            if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                sStat = string.Format("{0}[{1}]", sStat, (CData.iErrNo + 1).ToString("000"));
            }
            sStat.PadRight(20);

            CLog.Save_Log(eLog.DRY, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }

        private void _SetLog(string sMsg, double dPos)
        {
            sMsg = string.Format("{0}  Pos : {1}mm", sMsg, dPos);

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);

            //200824 jhc : 
            string sStat = CSQ_Main.It.m_iStat.ToString();
            if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                sStat = string.Format("{0}[{1}]", sStat, (CData.iErrNo + 1).ToString("000"));
            }
            sStat.PadRight(20);

            CLog.Save_Log(eLog.DRY, eLog.None, string.Format("SIMULATION : {0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }

        /// <summary>
        /// Init Cycle
        /// </summary>
        public void Init_Cyl()
        {
            m_iPreStep = 0;
            m_iStep = 10;
        }

        public bool ToReady()
        {
            m_bReqStop = false;
            SEQ = ESeq.Idle;
            return true;
        }

        public bool ToStop()
        {
            m_bReqStop = true;
            if (m_iStep != 0) return false;
            return true;
        }

        public bool Stop()
        {
            m_iStep = 0;
            return true;
        }
        /// <summary>
        /// Auto Run
        /// </summary>
        public bool AutoRun()
        {
            iSeq = CData.Parts[(int)EPart.DRY].iStat;
            //ksg
            //if (CSQ_Main.It.m_iStat == eStatus.Error || CSQ_Main.It.m_iStat != eStatus.Auto_Running)
            //{ return false; }
            if (CDataOption.Dryer == eDryer.Rotate) //200318 syc :
            {
                //Run_Stick(m_bStickRun);
            }

            if (SEQ == (int)ESeq.Idle)
            {
                if (m_bReqStop) return false;
                if (CSQ_Main.It.m_iStat == EStatus.Stop)
                {
                    //return false;
                }

                //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                bool bWait = false;
                bool bCheck = false;
                bool bRun = false;
                bool bOut = false;

                if (iSeq == ESeq.DRY_Wait || iSeq == ESeq.Idle)
                {
                    bWait = true;
                    vmMan_13QcVision.m_qcVision.addMsgList("DRY [AutoRun] ESeq.DRY_Wait");
                }

                if (iSeq == ESeq.DRY_CheckSensor)
                {
                    bCheck = true;
                    vmMan_13QcVision.m_qcVision.addMsgList("DRY [AutoRun]ESeq.DRY_CheckSensor");
                }

                if (iSeq == ESeq.DRY_Run)
                {
                    bRun = true;
                    vmMan_13QcVision.m_qcVision.addMsgList("DRY [AutoRun] ESeq.DRY_Run");
                }

                if (iSeq == ESeq.DRY_WaitPlace && CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd)
                {
                    CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WorkEnd;
                    vmMan_13QcVision.m_qcVision.addMsgList("DRY [AutoRun] DRY_WorkEnd");
                }

                if (iSeq == ESeq.DRY_WorkEnd)
                {
                    return true; 
                }

                if (iSeq == ESeq.DRY_Out)
                {
                    if (CData.Opt.bQcUse)
                    {
                        //190404 ksg : QC 
                        //if (!CTcpIp.It.bReady_Ok && m_MsgSendCnt < 110)
                        if (!CQcVisionCom.rcvEQReadyQueryOK && m_MsgSendCnt < 110)
                        {
                            if (m_Delay1.Chk_Delay())
                            {
                                //CTcpIp.It.SendStripOut();
                                //#2 ReadyQuery
                                CGxSocket.It.SendMessage("EQReadyQuery");

                                _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                //m_Delay.Wait(2000);
                                //m_Delay.Wait(20000);
                                m_MsgSendCnt++;
                                m_Delay1.Set_Delay(5000);
                                return false;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        //else if (!CTcpIp.It.bReady_Ok && m_MsgSendCnt >= 90)
                        else if (!CQcVisionCom.rcvEQReadyQueryOK && m_MsgSendCnt >= 90)
                        {
                            m_MsgSendCnt = 0;
                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_ERROR);
                        }
                        //if(CTcpIp.It.bReady_Ok)
                        if (CQcVisionCom.rcvEQReadyQueryOK)
                        {
                            m_MsgSendCnt = 0;
                            bOut = true;
                        }
                    }
                    else
                    {
                        //매거진 있는지 없는지
                        //매거진 있으면 슬롯이 있는지 없는지
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        bool bMGZ = false;
                        bool bBtmR = false;
                        //bool bSlot = (CData.Parts[(int)EPart.OFL].iSlot_info[CData.Parts[(int)EPart.OFL].iSlot_No] == (int)eSlotInfo.Empty);
                        bool bSlot = true;
                        if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                        {
                            //bMGZ = CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1) && CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2); //bottom receive 인지?
                            //bBtmR = (CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_BtmRecive);
                            bMGZ = true;
                            bBtmR = true;

                        }
                        else
                        {
                            //bMGZ = CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) && CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2); //top receive 인지?
                            //bBtmR = (CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_TopRecive);
                            bMGZ = true;
                            bBtmR = true;
                        }
                        //

                        //if(!bSlot)
                        //{ CData.Parts[(int)EPart.OFL].iSlot_No++; } //191223 ksg :
                        if (!bSlot)
                        {
                            if (CData.Opt.bFirstTopSlotOff)
                            {
                                CData.Parts[(int)EPart.OFL].iSlot_No--;
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFL].iSlot_No++;
                            }
                        }

                        if (bMGZ && bSlot && bBtmR)
                        {
                            bOut = true;
                        }
                    }
                }

                if (iSeq == ESeq.DRY_WorkEnd)
                {
					return true;
				}

                // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                    CMot.It.EStop(m_iX);

                    CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                    _SetLog("Error : Pusher overload.");

                    CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);

                    m_iStep = 0;
                    return true;
                }
                // 2021.11.18 SungTae End

                if (bWait)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.DRY_Wait;
                    _SetLog(">>>>> Wait start.");
                }

                if (bCheck)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.DRY_CheckSensor;
                    _SetLog(">>>>> Check safty start.");
                }

                if (bRun)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.DRY_Run;
                    _SetLog(">>>>> Dry work start.");
                }

                if (bOut)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.DRY_Out;
                    _SetLog(">>>>> Dry out start.");
                }
            }

            switch (SEQ)
            {
                default:
                    {
                        return false;
                    }

                case ESeq.DRY_Wait:
                    {
                        if (Cyl_Wait())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Wait finish.");
                        }

                        return false;
                    }

                case ESeq.DRY_CheckSensor:
                    {
                        if (CDataOption.Package == ePkg.Strip)
                        {
                            if (Cyl_ChkSafety())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Check safty finish.");
                            }
                        }
                        else
                        {
                            if (Cyl_ChkUnit())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Check unit finish.");
                            }
                        }

                        return false;
                    }

                case ESeq.DRY_Run:
                    {
                        if (CDataOption.Dryer == eDryer.Rotate)
                        {
                            if (Cyl_DryWork())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Dry work finish.");
                                vmMan_13QcVision.m_qcVision.addMsgList("DRY [AutoRun] Dry work finish.");
                            }
                        }
                        else
                        {
                            if (Cyl_DryWorkU())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Dry work finish.");
                            }
                        }
                        return false;
                    }

                case ESeq.DRY_Out:
                    {
                        //190404 ksg : Qc
                        if (CData.Opt.bQcUse)
                        {
                            if (Cyl_DryOut_Qc())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Dry out finish(QC).");
                            }
                        }
                        else
                        {
                            if (Cyl_DryOut())
                            {
                                m_DuringDryOut = false; //191107 ksg :
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Dry out finish.");
                            }
                        }

                        return false;
                    }
            }
        }

        #region Cycle
        public bool Cyl_Servo(bool bVal)
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_ALL_SERVO_ON_TIMEOUT);
                    _SetLog("Error : Timeout.");

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

                case 10:    // 1. 모든 축 서보 온
                    {
                        CMot.It.Set_SOn(m_iX, bVal);
                        CMot.It.Set_SOn(m_iZ, bVal);
                        if (CDataOption.Dryer == eDryer.Rotate)
                        { CMot.It.Set_SOn(m_iR, bVal); }
                        else
                        { CMot.It.Set_SOn(m_iY, bVal); }
                        _SetLog("All servo-on : " + bVal);

                        m_iStep++;
                        return false;
                    }

                case 11:    // 2. 서보 온 상태 체크
                    {
                        bool bTemp = false;
                        if (CDataOption.Dryer == eDryer.Rotate)
                        { bTemp = CMot.It.Chk_Srv(m_iR); }
                        else
                        { bTemp = CMot.It.Chk_Srv(m_iY); }

                        if (CMot.It.Chk_Srv(m_iX) == bVal &&
                            CMot.It.Chk_Srv(m_iZ) == bVal &&
                            bTemp == bVal)
                        {
                            _SetLog("All servo-on check.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 12:
                    {
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_Home()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_HOME_ERROR);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (CDataOption.Dryer == eDryer.Rotate)
            {
                switch (m_iStep)
                {
                    default:
                        {
                            m_bHD = false;
                            m_iStep = 0;
                            return true;
                        }

                    case 10:
                        {
                            if (Chk_Axes(false))
                            {
                                m_iStep = 0;
                                return true;
                            }

                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Check axes.  Stick standby");

                            m_iStep++;
                            return false;
                        }

                    case 11:
                        {
                            if (!Act_Get_Stick_StandyPos())
                            { return false; }

                            m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                            _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                            m_iStep++;
                            return false;
                        }

                    case 12:
                        {
                            if (!m_mCylDelay.Chk_Delay())
                            { return false; }

                            CMot.It.Mv_H(m_iZ);
                            _SetLog("Z axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 13:
                        {
                            if (!CMot.It.Get_HD(m_iZ)) { return false; }
                            if (!CMot.It.Get_HD(m_iTrZ)) { return false; }
                            if (CMot.It.Get_FP(m_iTrZ) > CData.Dev.dOffP_Z_Place) { return false; }

                            CMot.It.Mv_H(m_iX);
                            CMot.It.Mv_H(m_iR);
                            _SetLog("X, R axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 14:
                        {
                            bTemp_Val = CMot.It.Get_HD(m_iR);

                            if (!CMot.It.Get_HD(m_iX)) { return false; }
                            if (!bTemp_Val) { return false; }

                            _SetLog("All home done.");

                            m_iStep++;
                            return false;
                        }

                    case 15:
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                            CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                            _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 16:
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            { return false; }
                            if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                            { return false; }
                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                    case 17:
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                            { return false; }

                            CMot.It.MV_R(CData.SPos.dDRY_R_Wait, CData.Axes[m_iR].tRN.iVel);
                            _SetLog("R axis move wait.  Vel : " + CData.Axes[m_iR].tRN.iVel, CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 18:
                        {//드라이 R축 0도 이동 확인, 드라이 Z축 Up포지션으로 이동
                            if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                            { return false; }

                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                            _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                            m_iStep++;
                            return false;
                        }

                    case 19:
                        {//드라이 Z축 Up 포지션 이동 확인, 드라이 X축 대기 위치 이동
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                            { return false; }

                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 20:
                        {//드라이 X축 대기 위치 이동 확인, 종료
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            { return false; }

                            // 200721 jym : 조건 추가 자재 없을 때만 대기상태 변경
                            if (!CData.Parts[DRY].bExistStrip)
                            { CData.Parts[DRY].iStat = ESeq.DRY_WaitPlace; }
                            _SetLog("Finish.  Status : " + CData.Parts[DRY].iStat);

                            m_bHD = true;
                            m_iStep = 0;
                            return true;
                        }
                }
            }
            else // Unit 모드 Home
            {
                switch (m_iStep)
                {
                    default:
                        {
                            m_bHD = false;
                            m_iStep = 0;
                            return true;
                        }

                    case 10: // Cover open
                        {
                            if (Chk_Axes(false))
                            {
                                m_iStep = 0;
                                return true;
                            }

                            m_bStickRun = false;
                            Func_CoverClose(false);
                            _SetLog("Check axes.  Cover open.");

                            m_iStep++;
                            return false;
                        }

                    case 11: // Cover open check
                        {
                            if (!Func_CoverClose(false))
                            { return false; }

                            CMot.It.Mv_H(m_iY);
                            _SetLog("Y axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 12: // Y axis homing check
                        {
                            if (!CMot.It.Get_HD(m_iY))
                            { return false; }

                            CMot.It.Mv_H(m_iZ);
                            _SetLog("Z axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 13: // Z axis homing check
                        {
                            if (!CMot.It.Get_HD(m_iZ))
                            { return false; }
                            if (!CMot.It.Get_HD(m_iTrZ))
                            { return false; }
                            if (CMot.It.Get_FP(m_iTrZ) > CData.Dev.dOffP_Z_Place)
                            { return false; }

                            CMot.It.Mv_H(m_iX);
                            _SetLog("X axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 14: // X axis homing check
                        {
                            if (!CMot.It.Get_HD(m_iX))
                            { return false; }

                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 15: // Z axis move wait position check
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                            { return false; }

                            // 200721 jym : 조건 추가 자재 없을 때만 대기상태 변경
                            if (!CData.Parts[DRY].bExistStrip)
                            { CData.Parts[DRY].iStat = ESeq.DRY_WaitPlace; }
                            _SetLog("Finish.  Status : " + CData.Parts[DRY].iStat);

                            m_bHD = true;
                            m_iStep = 0;
                            return true;
                        }
                }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_Wait
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Wait()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_WAIT_POSITION_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (CDataOption.Dryer == eDryer.Rotate)
            {
                switch (m_iStep)
                {
                    default:
                        {
                            m_iStep = 0;
                            return true;
                        }

                    case 10: // 축 상태 체크 
                        {
                            m_StartSeq = DateTime.Now; //190807 ksg :                   
                            if (Chk_Axes())
                            {
                                m_iStep = 0;
                                return true;
                            }

                            //if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up) && CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            if(true)//
                            {
                                _SetLog("Simulation : Z axis check up.  X axis check wait.");

                                Thread.Sleep(200);

                                m_iStep = 16;
                                return false;
                            }

                            _SetLog("Check axes.");

                            m_iStep++;
                            return false;
                        }

                    case 11: // IO 초기화
                        {
                            _InitIO();
                            _SetLog("Init IO.");

                            m_iStep++;
                            return false;
                        }

                    case 12: // 드라이 Z축 다운 위치 이동
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                            _SetLog("Z axis move wait.");

                            m_iStep++;
                            return false;
                        }

                    case 13: // 드라이 Z축 다운 위치 이동 확인, 드라이 R축 0도로 이동
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                            {
                                return false;
                            }

                            if (CDataOption.Dryer == eDryer.Knife)
                            {
                                CMot.It.Mv_N(m_iY, CData.SPos.dDRY_R_Wait);
                                _SetLog("Y axis move wait.", CData.SPos.dDRY_R_Wait);
                            }
                            else
                            {
                                CMot.It.MV_R(CData.SPos.dDRY_R_Wait, CData.Axes[m_iR].tRN.iVel);
                                _SetLog("R axis move wait.  Vel : " + CData.Axes[m_iR].tRN.iVel, CData.SPos.dDRY_R_Wait);
                            }

                            m_iStep++;
                            return false;
                        }

                    case 14:
                        {//드라이 R축 0도 이동 확인, 드라이 Z축 Up포지션으로 이동
                            if (CDataOption.Dryer == eDryer.Knife)
                            {
                                bTemp_Val = CMot.It.Get_Mv(m_iY, CData.SPos.dDRY_R_Wait);
                            }
                            else
                            {
                                bTemp_Val = CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait);
                            }
                            if (bTemp_Val)
                            {
                                CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                                _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                                m_iStep++;
                            }

                            return false;
                        }

                    case 15:
                        {//드라이 Z축 Up 포지션 이동 확인, 드라이 X축 대기 위치 이동
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                            { return false; }

                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 16:
                        {//드라이 X축 대기 위치 이동 확인, 종료
                            //if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            //{ return false; }

                            Thread.Sleep(200);
                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            _SetLog("Tack time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                            // 200721 jym : 조건 추가 자재 없을 때만 대기상태 변경
                            if (!CData.Parts[DRY].bExistStrip)
                            { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitPlace; }
                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                            m_iStep = 0;
                            return true;
                        }
                }
            }
            else // Unit 모드
            {
                switch (m_iStep)
                {
                    default:
                        {
                            m_iStep = 0;
                            return true;
                        }

                    case 10: // Axes check
                        {
                            m_StartSeq = DateTime.Now; //190807 ksg :                   
                            if (Chk_Axes())
                            {
                                m_iStep = 0;
                                return true;
                            }

                            _SetLog("Check axes.");
                            m_iStep++;
                            return false;
                        }

                    case 11: // IO initailize
                        {
                            _InitIO();
                            _SetLog("Init IO.");

                            m_iStep++;
                            return false;
                        }

                    case 12: // Z axis, X axis position check
                        {
                            if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up) && CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            {
                                _SetLog("Z axis check up.  X axis check wait");

                                m_iStep = 16;
                                return false;
                            }

                            _SetLog("X, Z axis not wait.");
                            m_iStep++;
                            return false;
                        }

                    case 13: // Y axis move wait position
                        {
                            CMot.It.Mv_N(m_iY, CData.SPos.dDRY_R_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 14: // Y axis move wait position check
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.SPos.dDRY_R_Wait))
                            { return false; }

                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                            _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                            m_iStep++;
                            return false;
                        }

                    case 15: // Z axis move up position check
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                            { return false; }

                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 16: // X axis move wait position check
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            { return false; }

                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            _SetLog("Tack time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                            // 200721 jym : 조건 추가 자재 없을 때만 대기상태 변경
                            if (!CData.Parts[DRY].bExistStrip)
                            { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitPlace; }
                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                            m_iStep = 0;
                            return true;
                        }
                }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_CheckSensor
        /// </summary>
        /// <returns></returns>
        public bool Cyl_ChkSafety()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_CHECK_SAFETY_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (CData.Opt.bQcSimulation)
            {
                return true;
            }

            m_iPreStep = m_iStep;

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//축 상태 체크
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//IO 초기화
                        _InitIO();

                        //20200420 변수 초기화
                        isCheckSafety = false;
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//드라이 TOP Air 온
                        Func_TopAir(true);
                        _SetLog("Top air on.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//드라이 TOP Air 온 확인, 드라이 스틱 대기 위치
                        if (Func_TopAir(true))
                        {
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 14:
                    {//드라이 스틱 대기 위치 확인, 드라이 Z축 센서 검사 위치로 이동
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Check);
                        _SetLog("Z axis move check.", CData.SPos.dDRY_Z_Check);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이 Z축 센서 검사 위치 이동 확인, 드라이 스틱 에어 온
                        if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Check))
                        {
                            Func_BtmAir(true);
                            _SetLog("Bottom air on.");

                            m_iStep++;
                        }
                        return false;
                    }
                case 17:
                    {//드라이 스틱 에어 온 확인, 드라이 스틱 Run
                        if (Func_BtmAir(true))
                        {
                            _SetLog("Bottom air check.");
                            m_iStep++;
                        }

                        return false;
                    }

                case 18:
                    {//드라이 R축 센서 검사 위치 #1 이동        

                        if (CDataOption.Dryer == eDryer.Knife)
                        {// Knife Model 경우 Carrier 유무 감지 위해 이동
                            CMot.It.Mv_N(m_iY, CData.Dev.dDry_Car_Check);
                            _SetLog("Y axis move check.", CData.Dev.dDry_Car_Check);
                        }
                        else
                        {
                            CMot.It.MV_R(CData.Dev.dDry_R_Check1, CData.Axes[m_iR].tRN.iVel);
                            _SetLog("R axis move check1.", CData.Dev.dDry_R_Check1);
                        }

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//드라이 R축 센서 검사 위치 #1 이동 확인, 센서 검사 드라이 #1, ERROR 아닐 경우 드라이 R축 센서 검사 위치 #2 이동
                        if (CDataOption.Dryer == eDryer.Knife)
                        {// Knife Model 경우 Carrier 유무 감지 위해 이동
                            bTemp_Val = CMot.It.Get_Mv(m_iY, CData.Dev.dDry_Car_Check);
                        }
                        else
                        {
                            bTemp_Val = CMot.It.Get_Mv(m_iR, CData.Dev.dDry_R_Check1);
                        }

                        if (bTemp_Val)
                        {
                            m_mCylDelay.Set_Delay(500);
                            _SetLog("Set delay : 500ms");

                            m_iStep++;
                        }
                        return false;
                    }
                case 20:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        if (CIO.It.Get_X(eX.DRY_LevelSafety1))
                        {
                            CMot.It.MV_R(CData.Dev.dDry_R_Check2, CData.Axes[m_iR].tRN.iVel);
                            _SetLog("R axis move check2.", CData.Dev.dDry_R_Check2);

                            m_iStep++;
                        }
                        else
                        {
                            CErr.Show(eErr.DRY_CHECK_SAFETY_SENSOR1_ERROR);
                            _SetLog("Error : Check safty sensor1.");
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();

                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }

                case 21:
                    {//드라이 R축 센서 검사 위치 #2 이동 확인, 센서 검사 드라이 #2, 드라이 스틱 반복 동작 정지, 드라이 스틱 스탠드 위치 이동
                        if (CMot.It.Get_Mv(m_iR, CData.Dev.dDry_R_Check2))
                        {
                            m_mCylDelay.Set_Delay(500);
                            _SetLog("Set delay : 500ms");

                            m_iStep++;
                        }
                        return false;
                    }
                case 22:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        if (!CIO.It.Get_X(eX.DRY_LevelSafety2))
                        {   //ERROR
                            CErr.Show(eErr.DRY_CHECK_SAFETY_SENSOR2_ERROR);
                            _SetLog("Error : Check safty sensor2.");
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();

                            m_iStep = 0;
                            return true;
                        }
                        else
                        {
                            //t_Stick.Start();
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");

                            m_iStep++;
                        }
                        return false;
                    }
                case 23:
                    {//드라이 스틱 스탠드 위치 이동 확인, 드라이 스틱 에어 오프
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        Func_BtmAir(false);
                        _SetLog("Bottom air-blow off.");

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//드라이 스틱 에어 오프 확인, 종료
                        if (Func_BtmAir(false))
                        {
                            CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                            _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                        }
                        return false;
                    }

                case 26:
                    {
                        if (CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                        {
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Run;
                            _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                            //safety 확인
                            isCheckSafety = true;

                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
                        return false;
                    }
            }
        }

        /// <summary>
        /// Unit 모드 시 자재 유무 검사
        /// </summary>
        /// <returns></returns>
        public bool Cyl_ChkUnit()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_CHECK_SAFETY_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (CData.Opt.bQcSimulation) return true;
            m_iPreStep = m_iStep;

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: // Axes check
                    {
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes(true))
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11: // IO initialize
                    {
                        _InitIO();
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12: // Z axis move wait position
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13: // Z axis move wait position check
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                        { return false; }

                        //200429 jym : Unit check sensor air-blow
                        Func_CheckAir(true);
                        _SetLog("Check air-blow on.");

                        m_iStep++;
                        return false;
                    }

                case 14: // Y axis move unit_check position
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dDry_Unit_Check);
                        _SetLog("Y axis move check.", CData.Dev.dDry_Unit_Check);

                        m_iStep++;
                        return false;
                    }

                case 15: // Y axis move unit_check position check
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dDry_Unit_Check))
                        { return false; }

                        m_mCylDelay.Set_Delay(500);
                        _SetLog("Set delay : 500ms");

                        m_iStep++;
                        return false;
                    }

                case 16: // Check delay. Check Unit
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        bool bX1 = CIO.It.Get_X(eX.DRY_Unit1_Detect);
                        bool bX2 = CIO.It.Get_X(eX.DRY_Unit2_Detect);
                        bool bX3 = CIO.It.Get_X(eX.DRY_Unit3_Detect);
                        bool bX4 = CIO.It.Get_X(eX.DRY_Unit4_Detect);

                        _SetLog(string.Format("Sensor status.  #1:{0}  #2:{1}  #3:{2}  #4:{3}", bX1, bX2, bX3, bX4));

                        // 200707 jym : 유닛 2개 일 때 판정 추가
                        if (CData.Dev.iUnitCnt == 4)
                        {
                            _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}  #3:{2}  #4:{3}",
                                                        CData.Parts[DRY].aUnitEx[0],
                                                        CData.Parts[DRY].aUnitEx[1],
                                                        CData.Parts[DRY].aUnitEx[2],
                                                        CData.Parts[DRY].aUnitEx[3]));

                            if (CData.Parts[DRY].aUnitEx[0] != bX1)
                            {
                                CErr.Show(eErr.DRY_CHECK_UNIT1_NOT_EXIST_ERROR);
                                _SetLog("Error : Unit 1 not matching.");

                                m_iStep = 0;
                                return true;
                            }
                            if (CData.Parts[DRY].aUnitEx[1] != bX2)
                            {
                                CErr.Show(eErr.DRY_CHECK_UNIT2_NOT_EXIST_ERROR);
                                _SetLog("Error : Unit 2 not matching.");

                                m_iStep = 0;
                                return true;
                            }
                            if (CData.Parts[DRY].aUnitEx[2] != bX3)
                            {
                                CErr.Show(eErr.DRY_CHECK_UNIT3_NOT_EXIST_ERROR);
                                _SetLog("Error : Unit 3 not matching.");

                                m_iStep = 0;
                                return true;
                            }
                            if (CData.Parts[DRY].aUnitEx[3] != bX4)
                            {
                                CErr.Show(eErr.DRY_CHECK_UNIT4_NOT_EXIST_ERROR);
                                _SetLog("Error : Unit 4 not matching.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                        {
                            _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}", CData.Parts[DRY].aUnitEx[0], CData.Parts[DRY].aUnitEx[1]));

                            if (CData.Parts[DRY].aUnitEx[0] != (bX1 && bX2))
                            {
                                CErr.Show(eErr.DRY_CHECK_UNIT1_NOT_EXIST_ERROR);
                                _SetLog("Error : Unit 1 not matching.");

                                m_iStep = 0;
                                return true;
                            }
                            if (CData.Parts[DRY].aUnitEx[1] != (bX3 && bX4))
                            {
                                CErr.Show(eErr.DRY_CHECK_UNIT2_NOT_EXIST_ERROR);
                                _SetLog("Error : Unit 2 not matching.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        m_iStep++;
                        return false;
                    }

                case 17: // Y axis move wait position
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dDRY_R_Wait);
                        _SetLog("Y axis move wait.", CData.SPos.dDRY_R_Wait);

                        m_iStep++;
                        return false;
                    }

                case 18: // Y axis move wait position check
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dDRY_R_Wait))
                        { return false; }

                        //200429 jym : Unit check sensor air-blow
                        Func_CheckAir(false);
                        _SetLog("Check air-blow off.");

                        m_iStep++;
                        return false;
                    }

                case 19: // Finish, Satus change
                    {
                        CData.Parts[DRY].iStat = ESeq.DRY_Run;

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                        _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_Run
        /// </summary>
        /// <returns></returns>
        public bool Cyl_DryWork()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
#if true //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 연장
            { m_Timeout.Set_Delay(GV.DRYROTATION_TMOUT); }
#else
            { m_Timeout.Set_Delay(TIMEOUT); }
#endif
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_DRYWORK_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            //safety 확인
            if (!isCheckSafety && !CData.Opt.bQcSimulation)
            {
                CMsg.Show(eMsg.Notice, "Notice", "Dry Check Safety Sensor First.");
                return true;
            }
            if (!CheckSafetySensor() && !CData.Opt.bQcSimulation)
            {
                return true;
            }

            m_iPreStep = m_iStep;

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//축 상태 체크
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Check axes.");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Check axes");
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//IO 초기화
                        _InitIO();
                        //CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                        _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] R axis move wait.");
                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//드라이 TOP Air 온
                        //if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                        if(false)
                        { return false; }

                        Func_TopAir(true);
                        _SetLog("Top air-blow on.");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Top air-blow on..");
                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//드라이 TOP Air 온 확인, 드라이 스틱 대기 위치
                        if (Func_TopAir(true))
                        {
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Stick standby..");
                            m_iStep++;
                        }

                        return false;
                    }

                case 14:
                    {//드라이 스틱 대기 위치 확인, 드라이 Z축 Down 위치로 이동
                        //if (!Act_Get_Stick_StandyPos())
                        //{ return false; }

                        if (CDataOption.Dryer == eDryer.Knife)
                        {// Knife Model 경우 Delay 10ms 설정
                            m_mCylDelay.Set_Delay(10);
                            _SetLog("Set delay : 10ms");
                        }
                        else
                        {
                            m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                            _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");
                        }

                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Dry Stick Pos check, DRY Z Down move Pos");
                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Z axis move wait.");
                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이Z축 Down 위치로 이동 확인, 드라이 스틱 에어 온
                        //if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                        if (true)
                        {
                            Thread.Sleep(1000);
                            Func_BtmAir(true);
                            _SetLog("Bottom air-blow on.");
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Bottom air-blow on.");
                            m_iStep++;
                        }
                        return false;
                    }

                case 17:
                    {//드라이 스틱 에어 온 확인, 드라이 스틱 Run
                        if (!Func_BtmAir(true))
                        { return false; }
                        //t_Stick.Start();
                        m_TickTime.Set_Delay(2500);
                        //ksg 추가
                        if (CData.Opt.bDryStickSkip) m_bStickRun = false;
                        else m_bStickRun = true;

                        if (CData.Opt.bQcSimulation)
                        { m_bStickRun = false; }

                        _SetLog("Set delay : 2500ms  Stick skip : " + CData.Opt.bDryStickSkip);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Dry Stick Air On, Dry Stick Run");
                        m_iStep++;
                        return false;
                    }
                case 18:
                    {//드라이 R축 Run
                        dPosR = (CData.Dev.iDryCnt * 360);
                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        //CMot.It.MV_R(dPosR, CData.Dev.iDryRPM);
                        _SetLog("R axis run.  RPM : " + CData.Dev.iDryRPM + "  Count : " + CData.Dev.iDryCnt);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Dry R Run");
                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//드라이 R축 완료 확인, 드라이 R축 SetZero
                        dPosR = (CData.Dev.iDryCnt * 360);

                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        //if (CMot.It.Get_Mv(m_iR, dPosR))
                        if(true)
                        {
                            Thread.Sleep(1000);
                            //CMot.It.Set_ZeroPos(m_iR);
                            _SetLog("R axis set zero.");
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] R axis set zero.");
                            m_iStep++;
                        }
                        return false;
                    }

                case 20:
                    {//드라이 R축 SetZero 확인, 드라이 스틱 정지, 드라이 스틱 대기 위치
                        //if (CMot.It.Get_FP(m_iR) == 0)
                        if(true)
                        {
                            Thread.Sleep(1000);
                            //t_Stick.Stop();
                            m_bStickRun = false;
                            //Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Stick standby.");
                            m_iStep++;
                        }
                        return false;
                    }

                case 21:
                    {//드라이 스틱 대기 위치 확인, 드라이 스틱 에어 오프
                        //if (!Act_Get_Stick_StandyPos())
                        //{ return false; }
                        Thread.Sleep(1000);
                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Dry Stick Wiat Pos, Dry Stick Air Off");
                        m_iStep++;
                        return false;
                    }
                case 22:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        Func_BtmAir(false);
                        _SetLog("Bottom air-blow off");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Dry Stick check Pos, Dry Stick Air Off");
                        m_iStep++;
                        return false;
                    }
                case 23:
                    {//드라이 스틱 에어 오프 확인, X축 대기 위치 이동
                        if (Func_BtmAir(false))
                        {
                            //CMot.It.Mv_N((int)EAx.DryZone_X, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] Dry Stick Air off , X move wait Pos");
                            m_iStep++;// Dry 완료 후 X-axis 대기 위치 이동 확인
                        }
                        return false;
                    }

                case 24:
                    {//X축 대기 위치 이동 확인, Z축 업 위치 이동
                        //if (CMot.It.Get_Mv((int)EAx.DryZone_X, CData.SPos.dDRY_X_Wait))
                        if(true)
                        {
                            Thread.Sleep(1000);
                            //190404 ksg : Qc
                            if (CData.Opt.bQcUse)
                            {
                                //CTcpIp.It.SendStripOut();
                                //#2 ReadyQuery
                                CGxSocket.It.SendMessage("EQReadyQuery");

                                _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                
                                m_MsgSendCnt = 0;
                                m_Delay1.Set_Delay(2000);
                            }

                            //CMot.It.Mv_N((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up);
                            _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] X move wait pos, Z move up");
                            
                            //if(CData.CurCompany == eCompany.JSCK)
                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //190121 ksg : , 200625 lks
                            {
                                //if (CIO.It.Get_Y(eY.IOZR_Power)) CIO.It.Set_Y(eY.IOZR_Power, false); //190418 ksg :
                            }

                            m_iStep++;
                        }
                        return false;
                    }

                case 25:
                    {//Z축 업 위치 이동 확인, 종료
                        //if (CMot.It.Get_Mv((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up))
                        if(true)
                        {
                            Thread.Sleep(1000);
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Out;
                            //20200420 lks
                            isCheckSafety = false;

                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            _SetLog("Status : " + CData.Parts[(int)EPart.DRY].iStat);
                            _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));
                            //190404 ksg : Qc
                            //if(CData.Opt.bQcUse) CTcpIp.It.SendStripOut();
                            m_iStep = 0;
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryWork] check Z up pos , Finish");
                            return true;
                        }
                        return false;
                    }
            }
        }

        /// <summary>
        /// Unit 모드 시 사용
        /// </summary>
        /// <returns></returns>
        public bool Cyl_DryWorkU()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_DRYWORK_TIMEOUT);

                    _SetLog("Error:Dry work timeout.");

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

                case 10: // Axes check
                    {
                        m_tStTime = DateTime.Now;
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11: // IO initialize
                    {
                        _InitIO();
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12: // Y axis move wait position
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dDRY_R_Wait);
                        _SetLog("Y axis move wait.", CData.SPos.dDRY_R_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13: // Y axis move wait position check
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dDRY_R_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14: // Z axis move wait position check
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 15: // X axis move wait position check
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        { return false; }

                        if (!Func_CoverClose(true))
                        { return false; }
                        _SetLog("Cover close.");

                        m_iStep++;
                        return false;
                    }

                case 16: // Dry count intialize
                    {
                        m_iDryCnt = 0;
                        _SetLog("Dry count init.  Dry count:" + CData.Dev.iDryCnt);

                        m_iStep = 20;
                        return false;
                    }

                case 20: // Dry count check  =====> 반복 시작
                    {
                        _SetLog("Now count:" + m_iDryCnt);

                        if (m_iDryCnt >= CData.Dev.iDryCnt)
                        { m_iStep = 50; } // Dry 종료
                        else
                        { m_iStep++; } // Dry 반복

                        return false;
                    }

                case 21: // Y axis move start position
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dDry_Start);
                        _SetLog("Y axis move start.", CData.Dev.dDry_Start);

                        m_iStep++;
                        return false;
                    }

                case 22: // Y axis move start position check
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dDry_Start))
                        { return false; }

                        Func_TopAir(true);
                        _SetLog("Top air-blow on.");

                        m_iStep++;
                        return false;
                    }

                case 23: // Set delay
                    {
                        m_mCylDelay.Set_Delay(CData.Opt.iDryDelay);
                        _SetLog("Set delay:" + CData.Opt.iDryDelay + "ms");

                        m_iStep++;
                        return false;
                    }

                case 24: // Check delay
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        Func_BtmAir(true);
                        _SetLog("Bottom air-blow on.");

                        m_iStep++;
                        return false;
                    }

                case 25: // Y axis move end position (slow)
                    {
                        CMot.It.Mv_S(m_iY, CData.Dev.dDry_End);
                        _SetLog("Y axis move end (slow).", CData.Dev.dDry_End);

                        m_iStep++;
                        return false;
                    }

                case 26: // Y axis move end position check
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dDry_End))
                        { return false; }

                        m_mCylDelay.Set_Delay(GV.DRY_UNIT_DELAY);
                        _SetLog("Set delay:" + GV.DRY_UNIT_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 27: // Check delay
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        Func_BtmAir(false);
                        _SetLog("Bottom air-blow off.");

                        m_iStep++;
                        return false;
                    }

                case 28: // Set delay
                    {
                        m_mCylDelay.Set_Delay(CData.Opt.iDryDelay);
                        _SetLog("Set delay:" + CData.Opt.iDryDelay + "ms");

                        m_iStep++;
                        return false;
                    }

                case 29: // Check delay
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        m_iDryCnt++;
                        Func_TopAir(false);
                        _SetLog("Top air-blow off.");

                        m_iStep = 20;
                        return false;
                    }

                case 50: // Y axis move wait position
                    {
                        CMot.It.Mv_S(m_iY, CData.SPos.dDRY_R_Wait);
                        _SetLog("Y axis move wait (slow).", CData.SPos.dDRY_R_Wait);

                        m_iStep++;
                        return false;
                    }

                case 51: // Y axis move wait position check
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dDRY_R_Wait))
                        { return false; }

                        Func_TopAir(false);
                        Func_BtmAir(false);
                        _SetLog("Top ait-blow off.  Bottom air-blow off");

                        m_iStep++;
                        return false;
                    }

                case 52: // Bottom air-blow off check
                    {
                        if (!Func_CoverClose(false))
                        { return false; }

                        _SetLog("Cover open.");

                        m_iStep++;
                        return false;
                    }

                case 53: // Z axis move up position
                    {
                        CMot.It.Mv_S(m_iZ, CData.SPos.dDRY_Z_Up);
                        _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                        m_iStep++;
                        return false;
                    }

                case 54: // Z axis move up position check
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                        { return false; }

                        //190404 ksg : Qc
                        if (CData.Opt.bQcUse)
                        {
                            //CTcpIp.It.SendStripOut();
                            //#2 ReadyQuery
                            CGxSocket.It.SendMessage("EQReadyQuery");

                            _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            
                            m_MsgSendCnt = 0;
                            m_Delay1.Set_Delay(2000);
                            _SetLog("Send QC message");
                        }
                        else
                        { _SetLog("Not use QC"); }

                        m_iStep++;
                        return false;
                    }

                case 55: // Ionizer off
                    {
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //190121 ksg : , 200625 lks
                        {
                            if (CIO.It.Get_Y(eY.IOZR_Power)) CIO.It.Set_Y(eY.IOZR_Power, false);  // SCK, JSCK, JCET
                        }

                        CData.Parts[DRY].iStat = ESeq.DRY_Out;
                        _SetLog("Status change:" + CData.Parts[DRY].iStat);

                        m_iStep++;
                        return false;
                    }

                case 56: // Finish, Status change
                    {
                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                        _SetLog("Finish.  Time:" + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_Out
        /// </summary>
        /// <returns></returns>
        public bool Cyl_DryOut()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_STRIP_OUT_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                CMot.It.EStop(m_iX);

                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                _SetLog("Error : Pusher overload.");

                CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);

                m_iStep = 0;
                return true;
            }
            // 2021.11.18 SungTae End

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//축 상태 체크
                        m_tStTime = DateTime.Now;
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Check axes.");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] : Check axes.");
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//IO 초기화
                        _InitIO();
                        _SetLog("Init IO.");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] IO Init");
                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//오프로더 상태 확인(매거진 유무, 오프로더 Z축 Y축 위치 확인)
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                        {
                            if (CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1) && CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2))
                            {//오프로더 바텀 매거진 유무 확인
                                if (CIO.It.Get_X(eX.OFFL_BtmClampOn) && !CIO.It.Get_X(eX.OFFL_BtmClampOff))
                                {//오프로더 바텀 클램프 온 확인
                                    if (CMot.It.Get_FP((int)EAx.OffLoader_Y) == CData.Dev.dOffL_Y_Wait)
                                    {//오프로더 Y축 위치 확인
                                        if (CMot.It.Get_Mv((int)EAx.OffLoader_Z, CSq_OfL.It.GetBtmWorkPos()))
                                        {//오프로더 Z축 위치 확인

                                            _SetLog("Emit bottom.");
                                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] Emit bottom.");
                                            m_iStep++;
                                        }
                                        else
                                        {
                                            CErr.Show(eErr.OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR);
                                            _SetLog("Error : OFL Bottom not detect magazine.");
                                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] Error : OFL Bottom not detect magazine.");
                                            m_iStep = 0;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) && CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2))
                            {//오프로더 탑 매거진 유무 확인
                                if (CIO.It.Get_X(eX.OFFL_TopClampOn) && !CIO.It.Get_X(eX.OFFL_TopClampOff))
                                {//오프로더 탑 클램프 온 확인
                                    if (CMot.It.Get_FP((int)EAx.OffLoader_Y) == CData.Dev.dOffL_Y_Wait)
                                    {//오프로더 Y축 위치 확인
                                        if (CMot.It.Get_Mv((int)EAx.OffLoader_Z, CSq_OfL.It.GetTopWorkPos()))
                                        {//오프로더 Z축 위치 확인

                                            _SetLog("Emit top.");
                                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] Emit top");
                                            m_iStep++;
                                        }
                                        else
                                        {
                                            CErr.Show(eErr.OFFLOADER_TOP_NOT_DETECT_MGZ_ERROR);
                                            _SetLog("Error : OFL Top not detect magazine.");
                                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] OFL Top not detect magazine.");
                                            m_iStep = 0;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }

                        return false;
                    }

                case 13:
                    {//드라이 R축 0도 이동
                        //CMot.It.MV_R(0, CData.Dev.iDryRPM);
                        _SetLog("R axis move zero.");
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt]: R axis move zero.");
                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//드라이 R축 0도 이동 확인, 드라이 X축 대기 위치 이동
                        //if (!CMot.It.Get_Mv(m_iR, 0))
                        //{ return false; }
                        Thread.Sleep(200);
                        //CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] X axis move wait.");
                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//드라이 X축 대기 위치 이동 확인, 드라이 Z축 Up
                        //if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                        m_DuringDryOut = true; //191107 ksg :
                        _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt]: Z axis move up.");
                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이 Z축 UP 확인, 레일 아웃 에어 온, 드라이 X축 OUt 위치로 이동
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                        //{ return false; }

                        Func_OutRailAir(true);
                        //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out - 100, CData.Dev.dPushF);
                        _SetLog("X axis move position(fast).", CData.SPos.dDRY_X_Out - 100);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt]: X axis move position(fast)..");
                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        // 201007 jym : Out sensor 감지 시 속도 변경
                        //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        if(true)
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)
                            //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS);
                            _SetLog("X axis move out(slow).", CData.SPos.dDRY_X_Out);
                            //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            m_iStep = 19;
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] X axis move out(slow).");

                            // 201012 jym : 리턴 빠져있어서 추가
                            return false;
                        }

                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out - 100))
                        { return false; }

                        if (CIO.It.Get_X(eX.DRY_StripOutDetect)) //191108 ksg :
                        {
                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;
                            CErr.Show(eErr.DRY_NOT_DETECTED_STRIP);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }
                        CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushF);
                        _SetLog("X axis move out(fast).", CData.Dev.dPushF);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//드라이 X축 OUT위치로 이동 중 레일 아웃 센서 감지 상태 확인
                        if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)
                            CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS);
                            _SetLog("X axis move out(slow).", CData.SPos.dDRY_X_Out);

                            m_iStep++;
                        }

                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out))
                        {
                            CErr.Show(eErr.DRY_NOT_DETECTED_STRIP);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }

                case 19: // 드라이 X축 OUT 위치 이동 확인, 레일 아웃 에어 오프, 드라이 X축 대기 위치 이동
                    {
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out))
                        {
                            Func_OutRailAir(false);
                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                            m_iStep++;
                        }

                        return false;
                    }

                case 20:
                    {//드라이 X축 대기위치 이동 확인, 종료
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        {
                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;

                            if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))
                            {
                                // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                                ////CData.GemForm.Strip_Data_Shift(17, 23);// PickUp Data  -> Dry Data 로 이동
                                //CData.GemForm.Strip_Data_Shift(21, 23);  // PickUp Data  -> Dry Data 로 이동
                                //CData.JSCK_Gem_Data[23].nULD_MGZ_Strip_Cnt = CData.Parts[DRY].iSlot_No; //191118 ksg :

                                CData.GemForm.Strip_Data_Shift((int)EDataShift.DRY_WORK/*21*/, (int)EDataShift.OFL_STRIP_OUT/*23*/);// PickUp Data  -> Dry Data 로 이동

                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Cnt = CData.Parts[DRY].iSlot_No; //191118 ksg :
                                // 2021.07.19 SungTae End

                                CData.GemForm.OnCarrierEnded(CData.LotInfo.sLotName, CData.Parts[DRY].sBcr, (uint)CData.Parts[DRY].iSlot_No + 1, CData.Parts[DRY].iTPort + 1, 1);
                                CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[DRY].sBcr);
                            }

                            int Slot = CSq_OfL.It.GetBtmEmptySlot();
                            //data
                            CData.Parts[(int)EPart.OFL].sLotName = CData.Parts[DRY].sLotName;
                            CData.Parts[(int)EPart.OFL].iMGZ_No  = CData.Parts[DRY].iMGZ_No;
                            //CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.DRY].iSlot_No;
                            CData.Parts[(int)EPart.OFL].iSlot_No = Slot;
                            CData.Parts[(int)EPart.OFL].sBcr     = CData.Parts[DRY].sBcr;
                            CData.Parts[(int)EPart.DRY].sBcr     = ""; //190823 ksg :

                            //20190222 ghk_dynamicfunction
                            Array.Copy(CData.Parts[DRY].dPcb, CData.Parts[(int)EPart.OFL].dPcb, CData.Parts[DRY].dPcb.Length);
                            
                            //20200328 jhc : PCB 높이 Data Shift 오류 정정 (사용되고 있지 않지만 오류이므로 정정함)
                            //CData.Parts[(int)EPart.DRY].dPcbMax = CData.Parts[(int)EPart.OFP].dPcbMax;
                            //CData.Parts[(int)EPart.DRY].dPcbMin = CData.Parts[(int)EPart.OFP].dPcbMin;
                            //CData.Parts[(int)EPart.DRY].dPcbMean = CData.Parts[(int)EPart.OFP].dPcbMean;
                            CData.Parts[(int)EPart.OFL].dPcbMax  = CData.Parts[DRY].dPcbMax;
                            CData.Parts[(int)EPart.OFL].dPcbMin  = CData.Parts[DRY].dPcbMin;
                            CData.Parts[(int)EPart.OFL].dPcbMean = CData.Parts[DRY].dPcbMean;
                            //
                            //CData.Parts[(int)EPart.OFL].iSlot_info[CData.Parts[(int)EPart.OFL].iSlot_No] = (int)eSlotInfo.Exist;
                            CData.Parts[(int)EPart.OFL].iSlot_info[Slot] = (int)eSlotInfo.Exist;
                            //190503 ksg :
                            //CData.Parts[(int)EPart.OFL].iSlot_No++;
                            if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No--;
                            else                            CData.Parts[(int)EPart.OFL].iSlot_No++;

                            CData.Parts[DRY].bExistStrip = false;

                            // 200314 mjy : 신규추가
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                Array.Copy(CData.Parts[DRY].aUnitEx, CData.Parts[(int)EPart.OFL].aUnitEx, CData.Dev.iUnitCnt);
                            }

                            if (CData.Parts[(int)EPart.ONL].iStat   == ESeq.ONL_WorkEnd &&
                                CData.Parts[(int)EPart.INR].iStat   == ESeq.INR_WorkEnd &&
                                CData.Parts[(int)EPart.GRDR].iStat  == ESeq.GRR_WorkEnd &&
                                CData.Parts[(int)EPart.GRDL].iStat  == ESeq.GRL_WorkEnd &&
                                CData.Parts[(int)EPart.OFP].iStat   == ESeq.OFP_WorkEnd)
                            {
                                CData.Parts[DRY].iStat = ESeq.DRY_WorkEnd;
                            }

                            CData.LotInfo.iTOutCnt++; //190407 ksg :

                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                            _SetLog("Status : " + CData.Parts[(int)EPart.DRY].iStat);
                            m_tTackTime = DateTime.Now - m_tStTime;
                            _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
                        return false;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_Out -> Qc
        /// </summary>
        /// <returns></returns>
        public bool Cyl_DryOut_Qc()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_STRIP_OUT_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            // 2021.11.18 SungTae Start : [삭제]
            // AutoRun()에서 상시 체크하게 변경하면서 각 Cycle() 내에서 체크하는 것은 주석 처리. 추후 완전히 삭제할 예정.
            ////if (!CIO.It.Get_X(eX.DRY_PusherOverload))
            //if (false)
            //{//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
            //    CMot.It.EStop(m_iX);
            //    CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
            //    _SetLog("Error : Pusher overload.");

            //    m_iStep = 0;
            //    return true;
            //}
            // 2021.11.18 SungTae End

            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
            //if (!CIO.It.Get_X(eX.DRY_PusherOverload))
            if (false)
            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                CMot.It.EStop(m_iX);

                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                _SetLog("Error : Pusher overload.");

                CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);

                m_iStep = 0;
                return true;
            }
            // 2021.11.18 SungTae End

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//축 상태 체크
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//IO 초기화
                        _InitIO();
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!CTcpIp.It.bReady_Ok) return false;
                        if (!CQcVisionCom.rcvEQReadyQueryOK) return false;
                        //CTcpIp.It.bReady_Ok = false;
                        //CQcVisionCom.rcvEQReadyQueryOK = false;// Move to Case 20 : 
                        _SetLog("Ready ok.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (CData.Opt.bQcByPass)
                        {
                            //CTcpIp.It.SendByPass();
                            CGxSocket.It.SendByPass();
                        }
                        else
                        {
                            //CTcpIp.It.SendTest();
                            CGxSocket.It.SendStartTest();
                        }
                        _SetLog("By pass : " + CData.Opt.bQcByPass);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (CData.Opt.bQcByPass)
                        {
                            //if(!CTcpIp.It.bByPass_Ok) 
                            if (!CQcVisionCom.rcvEQSendOutACK)
                            {
                                m_Delay.Wait(100);
                                //CTcpIp.It.SendByPass();
                                CGxSocket.It.SendByPass();
                                return false;
                            }

                            _SetLog("By pass ok.");
                        }
                        else
                        {
                            //if(!CTcpIp.It.bTest_Ok) 
                            if (!CQcVisionCom.rcvEQSendOutACK)
                            {
                                m_Delay.Wait(100);
                                //CTcpIp.It.SendTest();
                                CGxSocket.It.SendStartTest();
                                return false;
                            }

                            _SetLog("Test ok.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//드라이 R축 0도 이동
                        //CTcpIp.It.bByPass_Ok = false;
                        //CTcpIp.It.bTest_Ok   = false;
                        CQcVisionCom.rcvEQSendOutACK = false;
                        CMot.It.MV_R(0, CData.Dev.iDryRPM);
                        _SetLog("R axis move zero.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이 R축 0도 이동 확인, 드라이 X축 대기 위치 이동
                       // if (!CMot.It.Get_Mv(m_iR, 0))
                       // { return false; }

                        //CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                        Thread.Sleep(1000);
                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//드라이 X축 대기 위치 이동 확인, 드라이 Z축 Up
                        //if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                        _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);
                        Thread.Sleep(1000);
                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//드라이 Z축 UP 확인, 레일 아웃 에어 온, 드라이 X축 OUt 위치로 이동
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                        //{ return false; }

                        Func_OutRailAir(true);
                        //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushF);
                        //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out - 100, CData.Dev.dPushF);
                        _SetLog("X axis move position(fast).", CData.SPos.dDRY_X_Out - 100);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        // 201007 jym : Out sensor 감지 시 속도 변경
                        //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        if(true)
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)
                            //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS);
                            _SetLog("X axsi move out(slow).", CData.SPos.dDRY_X_Out);
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOUt] X axsi move out(slow).");
                            m_iStep = 21;
                            // 201012 jym : 리턴 빠져있어서 추가
                            return false;
                        }

                        //if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out - 100))
                        //{ return false; }

                        //if (CIO.It.Get_X(eX.DRY_StripOutDetect)) //191108 ksg :
                        if(false)
                        {
                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;
                            CErr.Show(eErr.DRY_NOT_DETECTED_STRIP);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushF);
                        _SetLog("X axis move out(fast).", CData.Dev.dPushF);
                        vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOut_Qc] X axsi move out(fast).");
                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//드라이 X축 OUT위치로 이동 중 레일 아웃 센서 감지 상태 확인
                        //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        if(true)
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)
                            //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS);
                            _SetLog("X axsi move out(slow).", CData.SPos.dDRY_X_Out);
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOut_Qc] Rail Out sensor on during Dry X move OUT Pos");
                            m_iStep++;
                        }

                        //if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out))
                        if (false)
                        {
                            CErr.Show(eErr.DRY_NOT_DETECTED_STRIP);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }

                case 21:
                    {//드라이 X축 OUT 위치 이동 확인, 레일 아웃 에어 오프, 드라이 X축 대기 위치 이동
                        //if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out))
                        if(true)
                        {
                            Thread.Sleep(200);
                            Func_OutRailAir(false);
                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOut_Qc] check X move OUT pos ");

                            CGxSocket.It.SendMessage("EQSendEnd");

                            _SetLog("[SEND](EQ->QC) EQSendEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            m_iStep++;
                        }

                        return false;
                    }

                case 22:
                    {//드라이 X축 대기위치 이동 확인, 종료
                        //if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        if(true)
                        {
                            Thread.Sleep(1000);
                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;

                            if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))
                            {
                                // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                                ////CData.GemForm.Strip_Data_Shift(17, 23);// PickUp Data  -> Dry Data 로 이동
                                //CData.GemForm.Strip_Data_Shift(21, 23);  // PickUp Data  -> Dry Data 로 이동
                                //CData.JSCK_Gem_Data[23].nULD_MGZ_Strip_Cnt = CData.Parts[DRY].iSlot_No; //191118 ksg :

                                CData.GemForm.Strip_Data_Shift((int)EDataShift.DRY_WORK/*21*/, (int)EDataShift.OFL_STRIP_OUT/*23*/);// PickUp Data  -> Dry Data 로 이동

                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Cnt = CData.Parts[DRY].iSlot_No; //191118 ksg :
                                // 2021.07.19 SungTae End

                                CData.GemForm.OnCarrierEnded(CData.LotInfo.sLotName, CData.Parts[DRY].sBcr, (uint)CData.Parts[DRY].iSlot_No + 1, CData.Parts[DRY].iTPort + 1, 1);
                                CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[DRY].sBcr);
                            }
                            //190404 ksg : QC
                            if (CData.Opt.bQcUse)
                            {
                                CData.Parts[DRY].bExistStrip = false;

                                CData.Parts[(int)EPart.OFL].sLotName = CData.Parts[DRY].sLotName;
                                CData.Parts[(int)EPart.OFL].iMGZ_No = CData.Parts[DRY].iMGZ_No;

                                CQcVisionCom.rcvEQReadyQueryOK = false; // QC Ready init
                                
                            }
                            else
                            {
                                int Slot = CSq_OfL.It.GetBtmEmptySlot();
                                //data
                                CData.Parts[(int)EPart.OFL].sLotName = CData.Parts[DRY].sLotName;
                                CData.Parts[(int)EPart.OFL].iMGZ_No = CData.Parts[DRY].iMGZ_No;
                                //CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.DRY].iSlot_No;
                                CData.Parts[(int)EPart.OFL].iSlot_No = Slot;
                                CData.Parts[(int)EPart.OFL].sBcr = CData.Parts[DRY].sBcr;
                                //20190222 ghk_dynamicfunction
                                Array.Copy(CData.Parts[DRY].dPcb, CData.Parts[(int)EPart.OFL].dPcb, CData.Parts[DRY].dPcb.Length);
                                //20200328 jhc : DF Data Shift 오류 정정 (사용되고 있지 않지만 오류이므로 정정함)
                                //CData.Parts[(int)EPart.DRY].dPcbMax  = CData.Parts[(int)EPart.OFP].dPcbMax;
                                //CData.Parts[(int)EPart.DRY].dPcbMin  = CData.Parts[(int)EPart.OFP].dPcbMin;
                                //CData.Parts[(int)EPart.DRY].dPcbMean = CData.Parts[(int)EPart.OFP].dPcbMean;
                                CData.Parts[(int)EPart.OFL].dPcbMax = CData.Parts[DRY].dPcbMax;
                                CData.Parts[(int)EPart.OFL].dPcbMin = CData.Parts[DRY].dPcbMin;
                                CData.Parts[(int)EPart.OFL].dPcbMean = CData.Parts[DRY].dPcbMean;
                                //
                                //CData.Parts[(int)EPart.OFL].iSlot_info[CData.Parts[(int)EPart.OFL].iSlot_No] = (int)eSlotInfo.Exist;
                                CData.Parts[(int)EPart.OFL].iSlot_info[Slot] = (int)eSlotInfo.Exist;
                                //190503 ksg :
                                //CData.Parts[(int)EPart.OFL].iSlot_No++;
                                if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No--;
                                else CData.Parts[(int)EPart.OFL].iSlot_No++;
                                CData.Parts[DRY].bExistStrip = false;

                                // 200314 mjy : 신규추가
                                if (CDataOption.Package == ePkg.Unit)
                                {
                                    Array.Copy(CData.Parts[DRY].aUnitEx, CData.Parts[(int)EPart.OFL].aUnitEx, CData.Dev.iUnitCnt);
                                }
                            }

                            //if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                            //  CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                            //  CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                            //  CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                            //  CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd)
                            //{ CData.Parts[DRY].iStat = ESeq.DRY_WorkEnd; }
                            //if(CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd)
                            CData.Parts[DRY].iStat = ESeq.DRY_WorkEnd;

                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                            _SetLog("Status : " + CData.Parts[DRY].iStat);
                            _SetLog("QC Use Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));
                            
                            CSq_OfL.It.m_DelayRsl.Set_Delay(2000);
                            vmMan_13QcVision.m_qcVision.addMsgList("DRY [Cyl_DryOut_Qc] DryOut Findish");
                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }
            }
        }

        //200325 ksg :
        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_CheckSensor
        /// </summary>
        /// <returns></returns>
        public bool Cyl_ConversionPos()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay() == true)
                {
                    CErr.Show(eErr.DRY_CHECK_SAFETY_TIMEOUT);
                    _SetLog("Error : Timeout.");

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

                case 10:
                    {//축 상태 체크
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//IO 초기화
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//드라이 TOP Air 온
                        CIO.It.Set_Y(eY.DRY_TopAir, true);
                        _SetLog("Top ait-blow on.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//드라이 TOP Air 온 확인, 드라이 스틱 대기 위치
                        if (CIO.It.Get_Y(eY.DRY_TopAir))
                        {
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 14:
                    {//드라이 스틱 대기 위치 확인, 드라이 Z축 센서 검사 위치로 이동
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Check);
                        _SetLog("Z axis move check.", CData.SPos.dDRY_Z_Check);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이 Z축 센서 검사 위치 이동 확인, 드라이 스틱 에어 온
                        if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Check))
                        {
                            CIO.It.Set_Y((int)eY.DRY_BtmAir, true);
                            _SetLog("Finish.  Bottom air-blow on.");

                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }
            }
        }
        #endregion

        #region Function
        /// <summary>
        /// Check Motion Ready
        /// </summary>
        /// <param name="bHD"></param>
        /// <returns></returns>
        public bool Chk_Axes(bool bHD = true)
        {
            if (CData.Opt.bQcSimulation) return false;

            int iRet = 0;
            bool bRet = false;

            iRet = CMot.It.Chk_Rdy((int)EAx.DryZone_X, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.DRY_X_NOT_READY);
                return true;
            }

            iRet = CMot.It.Chk_Rdy((int)EAx.DryZone_Z, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.DRY_Z_NOT_READY);
                return true;
            }

            iRet = (CDataOption.Dryer == eDryer.Rotate) ? CMot.It.Chk_Rdy(m_iR, bHD) :
                                                          CMot.It.Chk_Rdy(m_iY, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.DRY_R_NOT_READY);
                return true;
            }

            return bRet;
        }

        public bool Chk_Unit()
        {
            bool bRet = false;
            if (CData.Dev.iUnitCnt == 4)
            {
                bRet = CIO.It.Get_X(eX.DRY_Unit1_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[0];
                bRet = CIO.It.Get_X(eX.DRY_Unit2_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[1];
                bRet = CIO.It.Get_X(eX.DRY_Unit3_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[2];
                bRet = CIO.It.Get_X(eX.DRY_Unit4_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[3];
            }
            else
            {
                bRet = CIO.It.Get_X(eX.DRY_Unit1_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[0];
                bRet = CIO.It.Get_X(eX.DRY_Unit2_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[0];
                bRet = CIO.It.Get_X(eX.DRY_Unit3_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[1];
                bRet = CIO.It.Get_X(eX.DRY_Unit4_Detect) == CData.Parts[(int)EPart.DRY].aUnitEx[1];
            }
            return bRet;
        }

        /// <summary>
        /// Init IO
        /// </summary>
        private void _InitIO()
        {
            if (CData.Opt.bQcSimulation) return;

            CIO.It.Set_Y(eY.DRY_TopAir, false);
            CIO.It.Set_Y(eY.DRY_BtmAir, false);
            CIO.It.Set_Y(eY.DRY_OutRailAir, false);
            m_bStickRun = false;
            if (CDataOption.Dryer == eDryer.Rotate)
            { Act_Set_Stick_StandyPos(); }
            else
            { Func_CoverClose(false); }
        }
        #endregion

        #region Dry Stick
        /// <summary>
        /// 드라이 스틱 스탠드 포지션 세팅
        /// </summary>
        public void Act_Set_Stick_StandyPos()
        {
            if (CData.Opt.bQcSimulation) return;

            CIO.It.Set_Y(eY.DRY_BtmStandbyPos, true);
            CIO.It.Set_Y(eY.DRY_BtmTargetPos, false);
        }

        public bool Func_StickStandby()
        {
            if (CData.Opt.bQcSimulation) return true;

            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_BtmStandbyPos, true);
            CIO.It.Set_Y(eY.DRY_BtmTargetPos, false);
            bRet = CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos);

            return bRet;
        }

        /// <summary>
        /// 드라이 스틱 스탠드 포지션 확인
        /// </summary>
        /// <returns></returns>
        public bool Act_Get_Stick_StandyPos()
        {
            Thread.Sleep(1000);
            if (CData.Opt.bQcSimulation) return false;

            bool bRet = false;
            if (CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos))
            { bRet = true; }

            return bRet;
        }

        /// <summary>
        /// 드라이 스틱 타켓 포지션 세팅
        /// </summary>
        public void Act_Set_Stick_TargetPos()
        {
            if (CData.Opt.bQcSimulation) return;

            CIO.It.Set_Y(eY.DRY_BtmStandbyPos, false);
            CIO.It.Set_Y(eY.DRY_BtmTargetPos, true);
        }

        public bool Func_StickTarget()
        {
            if (CData.Opt.bQcSimulation) return true;

            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_BtmStandbyPos, false);
            CIO.It.Set_Y(eY.DRY_BtmTargetPos, true);
            bRet = !CIO.It.Get_X(eX.DRY_BtmStandbyPos) && CIO.It.Get_X(eX.DRY_BtmTargetPos);

            return bRet;
        }

        /// <summary>
        /// 드라이 스틱 타켓 포지션 확인
        /// </summary>
        /// <returns></returns>
        public bool Act_Get_Stick_TargetPos()
        {
            if (CData.Opt.bQcSimulation) return true;

            bool bRet = false;

            if (!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && CIO.It.Get_X(eX.DRY_BtmTargetPos))
            { bRet = true; }

            return bRet;
        }

        /// <summary>
        /// 드라이 스틱 반복 움직이기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Run_Stick_tick(object sender, EventArgs e)
        {

            if (CData.Opt.bQcSimulation) return;

            if (Act_Get_Stick_StandyPos())
            { Act_Set_Stick_TargetPos(); }
            else if (Act_Get_Stick_TargetPos())
            { Act_Set_Stick_StandyPos(); }
        }

        /// <summary>
        /// 드라이 스틱 반복 움직이기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Run_Stick(bool Run = false)
        {
            if (CData.Opt.bQcSimulation) return;

            if (Run)
            {
                if (!m_TickTime.Chk_Delay()) return;
                if (Act_Get_Stick_StandyPos())
                { Act_Set_Stick_TargetPos(); }
                else if (Act_Get_Stick_TargetPos())
                { Act_Set_Stick_StandyPos(); }
                m_TickTime.Set_Delay(2500);
            }
            else
            {
                Act_Set_Stick_StandyPos();
            }
        }
        #endregion

        /// <summary>
        /// 탑 에어 블로우 온  true:온   false:오프
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_TopAir(bool bOn)
        {
            Thread.Sleep(200);
            if (CData.Opt.bQcSimulation) return true;

            return CIO.It.Set_Y(eY.DRY_TopAir, bOn);
        }

        /// <summary>
        /// 바텀 에어 블로우 온  true:온   false:오프
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_BtmAir(bool bOn)
        {
            Thread.Sleep(1000);
            if (CData.Opt.bQcSimulation) return true;

            return CIO.It.Set_Y(eY.DRY_BtmAir, bOn);
        }

        public bool Func_CoverOpen()
        {
            if (CData.Opt.bQcSimulation) return false;

            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_CoverOpen, true);
            CIO.It.Set_Y(eY.DRY_CoverClose, false);
            bRet = CIO.It.Get_X(eX.DRY_CoverOpen) && !CIO.It.Get_X(eX.DRY_CoverClose);

            return bRet;
        }

        public bool Func_CoverClose()
        {
            if (CData.Opt.bQcSimulation) return true;

            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_CoverOpen, false);
            CIO.It.Set_Y(eY.DRY_CoverClose, true);
            bRet = !CIO.It.Get_X(eX.DRY_CoverOpen) && CIO.It.Get_X(eX.DRY_CoverClose);

            return bRet;
        }

        /// <summary>
        /// Unit모드에서 커버 닫힘/열림
        /// </summary>
        /// <param name="bVal">true:닫힘  false:열림</param>
        /// <returns></returns>
        public bool Func_CoverClose(bool bVal)
        {
            if (CData.Opt.bQcSimulation) return true;

            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_CoverOpen, !bVal);
            CIO.It.Set_Y(eY.DRY_CoverClose, bVal);

            if (bVal)
            {
                bRet = !CIO.It.Get_X(eX.DRY_CoverOpen) && CIO.It.Get_X(eX.DRY_CoverClose);
            }
            else
            {
                bRet = CIO.It.Get_X(eX.DRY_CoverOpen) && !CIO.It.Get_X(eX.DRY_CoverClose);
            }

            return bRet;
        }

        public bool Func_OutRailAir(bool bOn)
        {
            if (CData.Opt.bQcSimulation) return true;

            return CIO.It.Set_Y(eY.DRY_OutRailAir, bOn);
        }

        public bool Func_CheckAir(bool bOn)
        {
            if (CData.Opt.bQcSimulation) return true;

            return CIO.It.Set_Y(eY.DRY_CheckAir, bOn);
        }

        private static void OnTimeOutEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //vmMan_13QcVision.m_qcVision.addMsgList($"TimeOut Occur : Error [{m_nTOErrorNum}]");
        }

        public void startTimeoutTimer(int nMilliSec, int nErrorNumber)
        {
            m_TimerTO.Interval = nMilliSec;
            m_TimerTO.Start();
            m_nTOErrorNum = nErrorNumber;
        }

        public void stopTimeoutTimer()
        {
            m_TimerTO.Stop();
        }

        public void Func_AbnormalStripOUt()
        {
            //if (CMot.It.Get_Mv((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up))
            if(true)
            {
                CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Out;
                isCheckSafety = false;
            }
        }

        /// <summary>
        /// DRY 센서 확인 20200420
        /// </summary>
        /// <returns></returns>
        public bool CheckSafetySensor()
        {
            bool retValue = false;

            if (CData.Opt.bQcSimulation) return true;

            //Knife 타입
            if (CDataOption.Dryer == eDryer.Knife)
            {
                if (CMot.It.Get_Mv(m_iY, CData.Dev.dDry_Car_Check))
                {
                    retValue = CIO.It.Get_X(eX.DRY_LevelSafety1);
                    if (!retValue)
                    {
                        CErr.Show(eErr.DRY_CHECK_SAFETY_SENSOR1_ERROR);
                        m_bStickRun = false;
                        Act_Set_Stick_StandyPos();
                    }
                }
                else
                    retValue = true;
            }
            else
            {
                if (CMot.It.Get_Mv(m_iR, CData.Dev.dDry_R_Check1))
                {
                    retValue = CIO.It.Get_X(eX.DRY_LevelSafety1);
                    if (!retValue)
                    {
                        CErr.Show(eErr.DRY_CHECK_SAFETY_SENSOR1_ERROR);
                        m_bStickRun = false;
                        Act_Set_Stick_StandyPos();
                    }
                }
                else if (CMot.It.Get_Mv(m_iR, CData.Dev.dDry_R_Check2))
                {
                    retValue = CIO.It.Get_X(eX.DRY_LevelSafety2);
                    if (!retValue)
                    {
                        CErr.Show(eErr.DRY_CHECK_SAFETY_SENSOR2_ERROR);
                        m_bStickRun = false;
                        Act_Set_Stick_StandyPos();
                    }
                }
                else
                    retValue = true;
            }

            return retValue;
        }
    }
}
