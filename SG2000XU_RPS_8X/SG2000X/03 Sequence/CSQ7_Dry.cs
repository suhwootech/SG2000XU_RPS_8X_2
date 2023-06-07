using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SG2000X
{
    public class CSq_Dry : CStn<CSq_Dry>
    {
        private readonly int TIMEOUT = 80000;
        private readonly int DRY = (int)EPart.DRY;

        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep    = 0;
        private int m_iPreStep = 0;
        private int m_MsgSendCnt = 0; //190419 ksg :
        private int m_iDryCnt = 0; // 200314-mjy : Unit 모드시 Dry Count 체크

        private int m_iY   = (int)EAx.DryZone_Air;// Knife Dryer 에 사용 됨
        private int m_iR   = (int)EAx.DryZone_Air;
        private int m_iZ   = (int)EAx.DryZone_Z;
        private int m_iX   = (int)EAx.DryZone_X;
        private int m_iTrZ = (int)EAx.OffLoaderPicker_Z;
        private bool bTemp_Val;// Bool 로 임시 사용을 위한 변수
        private bool bTemp1_Val;// Bool 로 임시 사용을 위한 변수
        private int nTemp_Val; // Int 로 임시 사용을 위한 변수
        private double dTemp_Pos; // Double 로 임시 사용을 위한 변수

        private bool m_bReqStop  = false;
        public  bool m_bStickRun = false;


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

        private CTim m_Delay = new CTim(); //190406 ksg :Qc
        private CTim m_Delay1 = new CTim(); //190406 ksg :Qc
        private CTim m_Delay2 = new CTim(); // 2022.01.26 lhs : Dryer Z축 Up 후 안정화 시간


        //20191007 ghk_drystick_time
        private CTim m_mCylDelay = new CTim();
        //

        private Timer t_Stick = new Timer();
        //190807 ksg : TackTime 
        private DateTime m_StartSeq;
        private DateTime m_EndSeq  ;
        private TimeSpan m_SeqTime ;

        private DateTime m_tStTime;
        private TimeSpan m_tTackTime ;

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
        private bool isCheckSafety =false;

        private CSq_Dry()
        {
            m_bHD = false;

            t_Stick.Interval = 100;
            t_Stick.Tick += Run_Stick_tick;
            m_bStickRun = false;
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

            CLog.Save_Log(eLog.DRY, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
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
            m_bReqStop = false    ;
            SEQ        = ESeq.Idle;
            return true;
        }

        public bool ToStop()
        {
            m_bReqStop = true;
            if(m_iStep != 0) return false;
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
                Run_Stick(m_bStickRun);
            }

            if (SEQ == (int)ESeq.Idle)
            {
                if(m_bReqStop) return false;
                if (CSQ_Main.It.m_iStat == EStatus.Stop)
                {
                    return false;
                }

                //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                bool bWait  = false;
                bool bCheck = false;
                bool bRun   = false;
                bool bOut   = false;
                bool bWaterNozzle = false;  // 2021.04.09 lhs 

                if (iSeq == ESeq.DRY_Wait || iSeq == ESeq.Idle)                     {   bWait           = true; }
                // 2021.04.15 lhs Start  
                //if (iSeq == ESeq.DRY_CheckSensor) {   bCheck = true;  }
                if (iSeq == ESeq.DRY_CheckSensor && !CDataOption.UseDryWtNozzle)    {   bCheck          = true; }  // Water Nozzle 사용 안하는 조건 추가
                if (iSeq == ESeq.DRY_WaterNozzle &&  CDataOption.UseDryWtNozzle)    {   bWaterNozzle    = true; }
                // 2021.04.15 lhs End
                if (iSeq == ESeq.DRY_Run)                                           {   bRun            = true; }

                if(iSeq == ESeq.DRY_WaitPlace && CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd)
                {
                    CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WorkEnd;
                    _SetLog(">>>>> Work End.");     // 2021.11.18 lhs
                }

                if (iSeq == ESeq.DRY_WorkEnd)
                {
                    return true; 
                }

                // 배출 동작일 때
                if (iSeq == ESeq.DRY_Out)
                {
                    #region SEQ : ESeq.DRY_Out, QC : Using Mode일 경우
                    if (CDataOption.UseQC && CData.Opt.bQcUse)
                    {
                        // QC로부터 EQReadyQuery에 대해 OK가 수신되었다. QC가 받을 수 있게 된것이다.
                        if (CQcVisionCom.rcvEQReadyQueryOK == true)                   
                        {
                            m_MsgSendCnt = 0;
                            bOut = true;                        // 배출 동작을 분기
                        } 
                        else    // QC가 수신 준비가 아직 되어있지 않다
                        {
                            if (CQcVisionCom.rcvEQSendRequst == true)                   // QC가 전송을 요청하였다면,
                            {
                                CQcVisionCom.rcvEQSendRequst = false;                   // 요청 Flag CLear

                                CGxSocket.It.SendMessage("EQReadyQuery");               // QC에게 받을 준비가 되었는지 곧바로 다시 묻는다.

                                _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                return false;
                            }


                            if (m_MsgSendCnt < 100)       // QC가 받을 준비가 되어있다고 하지 않는다면 지정 횟수까지 반복 수행
                            {
                                // 3초마다 준비 여부를 묻도록 한다.
                                if (m_Delay1.Chk_Delay())
                                {
                                    if (CQcVisionCom.nQCeqpStatus == 2)                 // Stop 상태라면 
                                    {
                                        CGxSocket.It.SendMessage("AutoRun");            // QC에게 AutoRun 상태로 전환을 지시한다.

                                        _SetLog("[SEND](EQ->QC) AutoRun");              // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                    }

                                    CGxSocket.It.SendMessage("EQReadyQuery");           // QC에게 받을 준비가 되었는지 다시 묻는다.

                                    _SetLog("[SEND](EQ->QC) EQReadyQuery");             // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                    m_MsgSendCnt++;
                                    m_Delay1.Set_Delay(3000);                           // 3초 주기
                                }

                                return false;
                            }
                            else  // if Timeout
                            {
                                _SetLog($"QC Ready Query Timeout, Count : {m_MsgSendCnt}");

                                m_MsgSendCnt = 0;
                                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_ERROR);         // Alarm을 일으켜준다.
                            }

                        } //of //of if (CQcVisionCom.rcvEQReadyQueryOK)

                    } // end : if(CData.Opt.bQcUse)
                    #endregion
                    #region SEQ : ESeq.DRY_Out, QC : Not Use Mode일 경우
                    else
                    {
                        //매거진 있는지 없는지
                        //매거진 있으면 슬롯이 있는지 없는지
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        bool bMGZ = false;
                        bool bBtmR = false;
                        bool bSlot = (CData.Parts[(int)EPart.OFL].iSlot_info[CData.Parts[(int)EPart.OFL].iSlot_No] == (int)eSlotInfo.Empty);
                        if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                        {
                            bMGZ = CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1) && CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2); //bottom receive 인지?
                            bBtmR = (CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_BtmRecive);
                        }
                        else
                        {
                            bMGZ = CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) && CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2); //top receive 인지?
                            bBtmR = (CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_TopRecive);
                        }
                        //
                        //if(!bSlot)
                        //{ CData.Parts[(int)EPart.OFL].iSlot_No++; } //191223 ksg :
                        if (!bSlot)
                        {
                            if(CData.Opt.bFirstTopSlotOff)  {   CData.Parts[(int)EPart.OFL].iSlot_No--; }
                            else                            {   CData.Parts[(int)EPart.OFL].iSlot_No++; }
                        }

                        if (bMGZ && bSlot && bBtmR)
                        {
                            bOut = true;
                        }
                    }
                    #endregion
                } // end : if (iSeq == ESeq.DRY_Out)

                // 211019 syc : 2004U 마지막장 사용자가 제거, Cover Pick 되어있는 상황에서 Dry Wait Cover Place로 변경
                if (CDataOption.Use2004U)
                {
                    if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                           CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                           CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                           CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                           CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                           CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_CoverPlace &&
						   CData.Parts[(int)EPart.DRY].bExistStrip == false)
					{
						CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitCoverPlace; // WorkEnd가 아닌 Cover Place 하고 WorkEnd 해야함
					}
				}
                //

                if (iSeq == ESeq.DRY_WorkEnd)
                {
                    return true; 
                }

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

                // 2021.04.09 lhs Start
                if (bWaterNozzle)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.DRY_WaterNozzle;
                    _SetLog(">>>>> Water Nozzle Clean start.");
                }
                // 2021.04.09 lhs End

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
                            //210928 syc : 2004U 
                            if (CDataOption.Use2004U)
                            {
                                if (Cyl_ChkCarrierLevel())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Check carrier safty finish.");
                                }
                            }
                            else// 기존 strip 검사 진행
                            {
                                if (Cyl_ChkSafety())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Check safty finish.");
                                }
                            }
                            //syc end
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

                // 2021.04.15 lhs Start
                case ESeq.DRY_WaterNozzle:
                    {
                        if (CDataOption.Package == ePkg.Strip)
                        {
                            if (Cyl_WaterNozzle())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Check safty finish.");
                            }
                        }
                        else
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Unit does not use DRY_WaterNozzle..");
                        }

                        return false;
                    }
                // 2021.04.15 lhs End

                case ESeq.DRY_Run:
                    {
                        if (CDataOption.Dryer == eDryer.Rotate)
                        {
                            if (CDataOption.UseDryWtNozzle) // 2021.04.16 lhs : Dryer에 Air 대신 Water Nozzle 사용시
                            {
                                if (Cyl_DryWork_WtNzl())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Dry work(Water Nozzle) finish.");
                                }
                            }
                            else
                            {
                                if (Cyl_DryWork())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Dry work finish.");
                                }
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
                        if(CDataOption.UseQC && CData.Opt.bQcUse)
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

                            // 2022.06.08 lhs Start : (SCK+)자재 감지시 호밍 제한
                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                            {
                                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                                {
                                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                                    _SetLog("Error : Strip-out detect.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            // 2022.06.08 lhs End

                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Check axes.  Stick standby");

                            // Dryer Clamp : 2022.07.02 lhs
                            if (CDataOption.eDryClamp == EDryClamp.Cylinder)
                            {
                                Func_DryClampOpen(false);  // Clamp : 실린더가 나오지 않게 
                                _SetLog("Dryer Clamp.");
                            }

                            CQcVisionCom.rcvEQReadyQueryOK = false; // QC Ready init

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
                            if (!CMot.It.Get_HD(m_iZ))      { return false; }
                            if (!CMot.It.Get_HD(m_iTrZ))    { return false; }
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

                            if (!CMot.It.Get_HD(m_iX))  { return false; }
                            if (!bTemp_Val)             { return false; }

                            _SetLog("All home done.");

                            m_iStep++;
                            return false;
                        }

                    case 15:
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);     _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                            CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);     _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                            return false;
                        }

                    case 16:
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))  { return false; }
                            if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))  { return false; }
                            
                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);     _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                    case 17:
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))  { return false; }

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
                } //end : switch (m_iStep)
            } //end : if (CDataOption.Dryer == eDryer.Rotate)
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


                            if (CDataOption.Use2004U)
                            {
                                CData.Parts[DRY].iStat = ESeq.DRY_Wait;
                            }
                            else
                            {
                                if (!CData.Parts[DRY].bExistStrip)
                                { CData.Parts[DRY].iStat = ESeq.DRY_WaitPlace; }
                                
                            }
                            _SetLog("Finish.  Status : " + CData.Parts[DRY].iStat);

                            m_bHD = true;
                            m_iStep = 0;
                            return true;
                        }
                } // end : switch (m_iStep)
            } //end : else <<-- if (CDataOption.Dryer == eDryer.Rotate)에 대한
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

                            // 2022.06.08 lhs Start : 자재 감지시 Wait 제한
                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                            {
                                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                                {
                                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                                    _SetLog("Error : Strip-out detect.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            // 2022.06.08 lhs End

                            // Dryer Clamp : 2022.07.02 lhs 
                            if (CDataOption.eDryClamp == EDryClamp.Cylinder)
                            {
                                Func_DryClampOpen(false);  // Clamp : 실린더가 나오지 않게 
                                _SetLog("Dryer Clamp.");
                            }

                            if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up) && CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                            {
                                _SetLog("Z axis check up.  X axis check wait.");

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
                                bTemp_Val = CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait); // 0도
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
            } // end : if (CDataOption.Dryer == eDryer.Rotate)
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
                            

                            //210929 syc : 2004U Lot 시작시 Cover Pick
                            if (CData.bDry_CoverPick)
                            {
                                CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitCoverPick;
                            }

                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.DRY].iStat);
                            //

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
                        if (!m_mCylDelay.Chk_Delay())   { return false; }

                        // 2022.01.17 SungTae Start : [수정] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
                        if (CData.CurCompany == ECompany.ASE_KR && !CData.bDryStripExist &&
                            CData.CurCompanyName != "INARI")                            // 2022.01.18 lhs : Inari 제외 (Inari Company 옵션 : ECompany.ASE_KR로 동일)
                        {
                            CMot.It.Mv_N((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_ChkStrip);
                            _SetLog("Z axis move check strip.", CData.SPos.dDRY_Z_ChkStrip);
                        }
                        else
                        {
                            CMot.It.Mv_N((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Check);
                            _SetLog("Z axis move check.", CData.SPos.dDRY_Z_Check);
                        }
                        // 2022.01.17 SungTae End

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이 Z축 센서 검사 위치 이동 확인, 드라이 스틱 에어 온
                        // 2022.01.17 SungTae Start : [수정] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
                        if (CData.CurCompany == ECompany.ASE_KR && !CData.bDryStripExist &&
                            CData.CurCompanyName != "INARI")                            // 2022.01.18 lhs : Inari 제외 (Inari Company 옵션 : ECompany.ASE_KR로 동일)
                        {
                            if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_ChkStrip))
                            {
                                Func_BtmAir(true);
                                _SetLog("Bottom air on.");

                                m_iStep++;
                            }
                        }
                        else
                        {
                            if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Check))
                            {
                                Func_BtmAir(true);
                                _SetLog("Bottom air on.");

                                m_iStep++;
                            }
                        }
                        // 2022.01.17 SungTae End

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

                            // 2021.12.15 SungTae Start : [수정] (ASE-KR VOC) Dry Rail에 Strip 유무 Check Step으로 이동 조건 추가
                            if (CData.CurCompany == ECompany.ASE_KR && !CData.bDryStripExist &&
                                CData.CurCompanyName != "INARI")                            // 2022.01.18 lhs : Inari 제외 (Inari Company 옵션 : ECompany.ASE_KR로 동일)
                            {
                                m_iStep = 30;
                            }
                            else
                            {
                                m_iStep++;
                            }
                            // 2021.12.15 SungTae End
                        }

                        //m_iStep++;
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
                            CData.bDryStripExist = false;       // 2021.12.15 SungTae : [추가] (ASE - KR VOC) Dry Rail에 Strip 유무 Check 추가 Flag 초기화

                            m_EndSeq  = DateTime.Now         ; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
                        return false;
                    }

                // 2021.12.15 SungTae Start : [추가] (ASE-KR VOC) Dry Rail에 Strip 유무 Check 추가
                case 30:
                    {
                        if (CMot.It.Get_Mv(m_iR, CData.Dev.dDry_R_Check1))
                        {
                            m_mCylDelay.Set_Delay(500);
                            _SetLog("[Checking Step] Dry R-Axis move check1 position. Set delay : 500ms");

                            m_iStep++;
                        }

                        return false;
                    }

                case 31:    // Dry Z축을 저속으로 Up
                    {
                        if (!m_mCylDelay.Chk_Delay()) { return false; }

                        // 2022.01.17 SungTae : [수정] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
                        //if (CMot.It.Mv_Speed(m_iZ, CData.SPos.dDRY_Z_Check + 4.0, 1) == 0)
                        if (CMot.It.Mv_Speed(m_iZ, CData.SPos.dDRY_Z_ChkStrip + 4.0, 1) == 0)
                        {
                            _SetLog($"[Checking Step] Check Start... Moving speed = 1 mm/sec");
                            _SetLog($"[Checking Step] Dry Z-Axis move target position({CData.SPos.dDRY_Z_ChkStrip} + 4.0 mm)");
                            m_iStep++;
                        }

                        return false;
                    }

                case 32:    // Strip Sensing 여부 Check
                    {
                        // 2022.01.17 SungTae : [수정] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Check + 4.0))
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_ChkStrip + 4.0))
                        {
                            // Dry Z축이 천천히 Up 할 때 Safety2의 경우 이미 Rail Block을 감지하기 때문에 Checking에서 제외.
                            if (!CIO.It.Get_X(eX.DRY_LevelSafety1)/* || !CIO.It.Get_X(eX.DRY_LevelSafety2)*/)
                            {
                                CMot.It.Stop(m_iZ);

                                CData.bDryStripExist = true;

                                _SetLog($"[Checking Step] Strip Exist. Dry Z-Axis move Stop. Flag : false -> true");

                                m_iStep++;
                                return false;
                            }

                            return false;
                        }
                        else
                        {
                            CErr.Show(eErr.DRY_NOT_DETECTED_STRIP);
                            _SetLog("[Checking Step] Error : Dry not detected strip.");

                            CData.bDryStripExist = false;
                            m_bStickRun = false;

                            Act_Set_Stick_StandyPos();
                            m_iStep = 0;
                        }

                        return false;
                    }

                case 33:    // Dry Z축 Check Position으로 이동
                    {
                        if (CMot.It.Mv_S(m_iZ, CData.SPos.dDRY_Z_Check) == 0)
                        {
                            _SetLog($"[Checking Step] Dry Z-Axis move to check position({CData.SPos.dDRY_Z_Check} mm).(Slow)");

                            m_iStep++;
                        }

                        return false;
                    }

                case 34:    // Dry Z축 Check Position으로 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Check))
                        {
                            return false;
                        }

                        _SetLog($"[Checking Step] Strip Exist Step Complete.");

                        m_iStep = 18;
                        return false;
                    }
                // 2021.12.15 SungTae End
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
        /// 210917 syc : 2004U
        /// Dry Work 이전 Cover가 제대로 Place 되어 있는지 
        /// </summary>
        /// <returns></returns>
        public bool Cyl_ChkCarrierLevel()
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
                case 12: // Y축 대기 위치 검사 및 이동
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dDRY_R_Wait))
                        {
                            CErr.Show(eErr.DRY_R_NOT_READY);
                            _SetLog("Error : DRY Y NOT WAIT POSITION.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check Y Position complete");

                        m_iStep++;
                        return false;
                    }         

                // Z축 Level Check 포지션 이동
                case 13:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Check) == 0)
                        {
                            _SetLog("Z Dry Check Position", CData.SPos.dDRY_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // Z축 Leve Check 포지션 이동 확인
                case 14: 
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Check))
                        { return false; }

                        int iDelaly = 2000;

                        m_mCylDelay.Set_Delay(iDelaly);
                        _SetLog("Set delay : " + iDelaly + "ms");

                        m_iStep++;
                        return false;
                    }
                case 15:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        if (CIO.It.Get_X(eX.DRY_Carrier_Cover_Close))
                        {
                            CErr.Show(eErr.DRY_CHECK_SAFETY_SENSOR1_ERROR);
                            _SetLog("Error : DRY_CHECK_SAFETY_SENSOR_ERROR.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Cover Checked Complete");

                        m_iStep++;
                        return false;
                    }
                case 16:
                    {
                        CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Run;
                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                        m_iStep = 0;
                        return true;
                    }

            }
        }

        // <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_WaterNozzle  : 워터 노즐로 Strip 세척... 우선은 Unit은 적용 안함.
        /// </summary>
        /// <returns></returns>
        public bool Cyl_WaterNozzle()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_Timeout.Set_Delay(TIMEOUT); }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_WATER_NOZZLE_TIMEOUT);
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

                case 10: //축 상태 체크
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

                case 11: //IO 초기화
                    {
                        _InitIO();
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12: //드라이 TOP Air 온
                    {
                        Func_TopAir(true);        // TopAirㅣ를 써야 하나? 
                        _SetLog("Top air on.");

                        m_iStep++;
                        return false;
                    }

                case 13: //드라이 TOP Air 온 확인, 드라이 스틱 대기 위치
                    {
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
                    {//드라이 스틱 대기 위치 확인 
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 15: //드라이 Z축 센서 다운 위치로 이동
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 16: //드라이 Z축 다운 위치 이동 확인, 드라이 워터노즐 온, R축 대기위치 이동
                    {
                        if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                        {
                            Func_BtmAir(true);  // Water Nozzle (기존 함수명 변경 안함)
                            _SetLog("Water nozzle on.");

                            CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                            _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                        }
                        return false;
                    }
                case 17: //드라이 워터노즐 온 확인, 드라이 스틱 Run
                    {
                        if (!Func_BtmAir(true)) // Water Nozzle (기존 함수명 변경 안함)
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                        { return false; }

                        m_TickTime.Set_Delay(2500);
                        if (CData.Opt.bDryStickSkip)    m_bStickRun = false;
                        else                            m_bStickRun = true;
                        _SetLog("Set delay : 2500ms  Stick skip : " + CData.Opt.bDryStickSkip);

                        m_iStep++;
                        return false;
                    }

                case 18: //드라이 R축 천천히 회전 
                    {
                        dPosR = (CData.Dev.iDryWtNozzleCnt * 360);
                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        CMot.It.MV_R(dPosR, CData.Dev.iDryWtNozzleRPM);
                        _SetLog("R axis run. (Water Nozzle) RPM : " + CData.Dev.iDryWtNozzleRPM + "  Count : " + CData.Dev.iDryWtNozzleCnt);

                        m_iStep++;
                        return false;
                    }

                case 19: // 드라이 R축 완료 확인, 드라이 R축 SetZero
                    {
                        if (CMot.It.Get_Mv(m_iR, dPosR))
                        {
                            CMot.It.Set_ZeroPos(m_iR);
                            _SetLog("R axis set zero.");

                            m_iStep++;
                        }
                        return false;
                    }
                case 20: //드라이 R축 SetZero 확인, 드라이 스틱 정지, 드라이 스틱 대기위치
                    {
                        if (CMot.It.Get_FP(m_iR) == 0)
                        {
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 21: // 드라이 스틱 대기위치 확인 
                    {
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }
                case 22: // 드라이 스틱 워터노즐 오프
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        Func_BtmAir(false);     // Water Nozzle (기존 함수명 변경 안함)
                        _SetLog("Water Nozzle off");

                        m_iStep++;
                        return false;
                    }

                case 23: //드라이 스틱 워터노즐 오프 확인
                    {
                        if (Func_BtmAir(false)) // Water Nozzle (기존 함수명 변경 안함)
                        {
                            CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                            _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                            m_iStep++;
                        }
                        return false;
                    }

                case 24: // 종료
                    {
                        if (CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverDryerEnd; // 커버 드라이 종료

                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Run;
                            _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                            m_EndSeq = DateTime.Now;
                            m_SeqTime = m_EndSeq - m_StartSeq;
                            _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
                        return false;
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
            { 
                m_Timeout.Set_Delay(GV.DRYROTATION_TMOUT);  //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 연장
            }
            else
            {
                if (m_Timeout.Chk_Delay())
                {
                    CErr.Show(eErr.DRY_DRYWORK_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    //syc : Qorvo 수정 - wait psition 진행 중 timeout 에러 발생시 계속 회전 멈추는 구문 없어 추가함.
                    CMot.It.Stop((int)EAx.DryZone_Air);

                    m_iStep = 0;
                    return true;
                }
            }

            if (CDataOption.eDryClamp == EDryClamp.None) // 2022.07.05 lhs : Spring Clamp 일 경우에만 체크하게 수정
            {
                //safety 확인
                if (!isCheckSafety)
                {
                    CMsg.Show(eMsg.Notice, "Notice", "Dry Check Safety Sensor First.");
                    return true;
                }
            }

            if (CDataOption.eDryClamp != EDryClamp.Cylinder) // 2022.07.05 lhs : Cylinder Clamp가 아닐 경우만 체크하게 수정
            {
                if (!CheckSafetySensor())
                {
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
                        _InitIO();                        
                        CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                        _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//드라이 TOP Air 온
                        if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                        { return false; }

                        Func_TopAir(true);  _SetLog("Top air-blow on.");

                        if(CDataOption.UseDryerLevelAirBlow)
                        {
                            Func_LevelSafetyAir(true);      _SetLog("Level Safety Air Blow ON");
                        }

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
                    {//드라이 스틱 대기 위치 확인, 드라이 Z축 Down 위치로 이동
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

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

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이Z축 Down 위치로 이동 확인, 드라이 스틱 에어 온
                        if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                        {
                            Func_BtmAir(true);
                            _SetLog("Bottom air-blow on.");

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
                        if (CData.Opt.bDryStickSkip)    m_bStickRun = false;
                        else                            m_bStickRun = true;
						
                        _SetLog("Set delay : 2500ms  Stick skip : " + CData.Opt.bDryStickSkip);

                        m_iStep++;
                        return false;
                    }
                case 18:
                    {//드라이 R축 Run
                        dPosR = (CData.Dev.iDryCnt * 360);
                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        CMot.It.MV_R(dPosR, CData.Dev.iDryRPM);
                        _SetLog("R axis run.  RPM : " + CData.Dev.iDryRPM + "  Count : " + CData.Dev.iDryCnt);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//드라이 R축 완료 확인, 드라이 R축 SetZero
                        dPosR = (CData.Dev.iDryCnt * 360);

                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        if (CMot.It.Get_Mv(m_iR, dPosR))
                        {
                            CMot.It.Set_ZeroPos(m_iR);
                            _SetLog("R axis set zero.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 20:
                    {//드라이 R축 SetZero 확인, 드라이 스틱 정지, 드라이 스틱 대기 위치
                        if (CMot.It.Get_FP(m_iR) == 0)
                        {
                            //t_Stick.Stop();
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 21:
                    {//드라이 스틱 대기 위치 확인, 드라이 스틱 에어 오프
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");
                        
                        m_iStep++;
                        return false;
                    }
                case 22:
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        Func_BtmAir(false);     _SetLog("Bottom air-blow off");
                        
                        Func_TopAir(false);     _SetLog("Top air-blow off");    //210708 pjh : Dry work End 후 Top air off

                        if (CDataOption.UseDryerLevelAirBlow)
                        {
                            Func_LevelSafetyAir(false); _SetLog("Level Safety Air Blow OFF");
                        }

                        m_iStep++;
                        return false;
                    }
                case 23:
                    {//드라이 스틱 에어 오프 확인, X축 대기 위치 이동
                        if (Func_BtmAir(false) && Func_TopAir(false))
                        {
                            CMot.It.Mv_N((int)EAx.DryZone_X, CData.SPos.dDRY_X_Wait);
                            _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                            m_iStep++;// Dry 완료 후 X-axis 대기 위치 이동 확인
                        }
                        return false;
                    }
               
                case 24:
                    {//X축 대기 위치 이동 확인, Z축 업 위치 이동
                        if (CMot.It.Get_Mv((int)EAx.DryZone_X, CData.SPos.dDRY_X_Wait))
                        {
                            // 2021-01-11, YYY for New QQ-Vision
                            if (CDataOption.UseQC && CData.Opt.bQcUse)
                            {
                                //CTcpIp.It.SendStripOut();
                                //#2 ReadyQuery
                                CGxSocket.It.SendMessage("EQReadyQuery");

                                _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                m_MsgSendCnt = 0;
                                m_Delay1.Set_Delay(3000);
                            }

                            CMot.It.Mv_N((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up);
                            _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                            //if(CData.CurCompany == eCompany.JSCK)
                            if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //190121 ksg : , 200625 lks
                            {
                                if(CIO.It.Get_Y(eY.IOZR_Power)) CIO.It.Set_Y(eY.IOZR_Power, false);  // SCK, JSCK, JCET 
                            }

                            m_iStep++;
                        }
                        return false;
                    }

                case 25:
                    {//Z축 업 위치 이동 확인, 종료
                        if (CMot.It.Get_Mv((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up))
                        {
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Out;
                            //20200420 lks
                            isCheckSafety = false;

                            m_EndSeq  = DateTime.Now         ; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            _SetLog("Status : " + CData.Parts[(int)EPart.DRY].iStat);
                            _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));
                            //190404 ksg : Qc
                            //if(CData.Opt.bQcUse) CTcpIp.It.SendStripOut();

                            m_iStep = 0;
                            return true;
                        }
                        return false; 
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - DRY_Run (Water Nozzle 적용시)
        /// 여기서 Btm Air는 Water Nozzle이라 사용하지 않고, Top Air와 회전으로 Dry 수행.
        /// </summary>
        /// <returns></returns>
        public bool Cyl_DryWork_WtNzl()
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

         
            m_iPreStep = m_iStep;

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: //축 상태 체크
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

                case 11: //IO 초기화
                    {
                        _InitIO();
                        CMot.It.Mv_N(m_iR, CData.SPos.dDRY_R_Wait);
                        _SetLog("R axis move wait.", CData.SPos.dDRY_R_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12: //드라이 TOP Air 온
                    {
                        if (!CMot.It.Get_Mv(m_iR, CData.SPos.dDRY_R_Wait))
                        { return false; }

                        Func_TopAir(true);
                        _SetLog("Top air-blow on.");

                        m_iStep++;
                        return false;
                    }

                case 13: //드라이 TOP Air 온 확인, 드라이 스틱 대기 위치
                    {
                        if (Func_TopAir(true))
                        {
                            m_bStickRun = false; // 스틱 왕복 사용하지 않음.
                            Act_Set_Stick_StandyPos();
                            _SetLog("Stick standby.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 14: //드라이 스틱 대기 위치 확인 
                    {
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

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

                        m_iStep++;
                        return false;
                    }

                case 15: //드라이 Z축 Down 위치로 이동
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dDRY_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 16: //드라이Z축 Down 위치로 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Wait))
                        { return false; }

                        m_iStep++;
                        return false;
                    }
                    
                case 17: //드라이 R축 Run
                    {
                        dPosR = (CData.Dev.iDryCnt * 360);
                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        CMot.It.MV_R(dPosR, CData.Dev.iDryRPM);
                        _SetLog("R axis run.  RPM : " + CData.Dev.iDryRPM + "  Count : " + CData.Dev.iDryCnt);

                        m_iStep++;
                        return false;
                    }

                case 18: //드라이 R축 완료 확인, 드라이 R축 SetZero
                    {
                        dPosR = (CData.Dev.iDryCnt * 360);
                        if (CData.Dev.eDryDir == eDryDir.CCW)
                        { dPosR *= -1; }

                        if (CMot.It.Get_Mv(m_iR, dPosR))
                        {
                            CMot.It.Set_ZeroPos(m_iR);
                            _SetLog("R axis set zero.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 19: //드라이 R축 SetZero 확인
                    {
                        if (CMot.It.Get_FP(m_iR) == 0)
                        {
                            m_bStickRun = false;
                            Act_Set_Stick_StandyPos();
                            _SetLog("R axis zero OK.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 20: //드라이 스틱 대기 위치 확인
                    {
                        if (!Act_Get_Stick_StandyPos())
                        { return false; }

                        //20191007 ghk_drystick_time
                        m_mCylDelay.Set_Delay(GV.DRY_STICK_DELAY);
                        _SetLog("Set delay : " + GV.DRY_STICK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }
                case 21: //X축 대기 위치 이동
                    {
                        if (!m_mCylDelay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                        m_iStep++;  
                        return false;
                    }

                case 22: //X축 대기 위치 이동 확인, Z축 업 위치 이동
                    {
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        {
                            if (CDataOption.UseQC && CData.Opt.bQcUse)
                            {
                                CGxSocket.It.SendMessage("EQReadyQuery");

                                _SetLog("[SEND](EQ->QC) EQReadyQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                
                                m_MsgSendCnt = 0;
                                m_Delay1.Set_Delay(3000);
                            }

                            CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                            _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) 
                            {
                                if (CIO.It.Get_Y(eY.IOZR_Power)) CIO.It.Set_Y(eY.IOZR_Power, false);  // SCK, JSCK, JCET
                            }

                            m_iStep++;
                        }
                        return false;
                    }

                case 23: //Z축 업 위치 이동 확인, 종료
                    {
                        if (CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                        {
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Out;

                            m_EndSeq = DateTime.Now;            
                            m_SeqTime = m_EndSeq - m_StartSeq; 
                            _SetLog("Status : " + CData.Parts[(int)EPart.DRY].iStat);
                            _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
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
                        if (CDataOption.UseQC && CData.Opt.bQcUse)
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

                        //210927 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitCoverPick;   // Cover Pick 하기 이전까지 대기
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverIV2;       // Cover 검사로 변경
                        }
                        else { CData.Parts[DRY].iStat = ESeq.DRY_Out; } // 기존 시퀀스 진행
                        // syc end


                        _SetLog("Status change:" + CData.Parts[DRY].iStat);
                        _SetLog("Status change:" + CData.Parts[(int)EPart.OFP].iStat);

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
                    // 2022.04.28 SungTae Start : [추가] (ASE-KR VOC)
                    // DRY_STRIP_OUT_TIMEOUT 상황에서도 Pusher를 Wait Pos로 이동되도록 변경
                    int nRet = CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                    if (nRet != 0)
                    {
                        return false;
                    }
                    // 2022.04.28 SungTae End

                    CErr.Show(eErr.DRY_STRIP_OUT_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
            if (!CIO.It.Get_X(eX.DRY_PusherOverload)) //B접 (푸셔 오버로드 감지 되었을때 에러 발생)
            {
                CMot.It.EStop(m_iX);

                int nRet = CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                // 2022.02.10 lhs Start : DRY_PUSHER_OVERLOAD_ERROR시 Pusher가 리턴 안되는 문제 개선
                //                      : 사내 66X 설비에서 테스트 완료, Pusher 속도 안 낮추어도 동작 (Fast 150, Slow 60 확인)
                if (nRet != 0)  
                {
                    return false;
				}
                // 2022.02.10 lhs End

                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                _SetLog("Error : Pusher overload.");

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

                        m_iStep++;
                        return false;
                    }

                case 11: //IO 초기화
                    {
                        _InitIO();
                        _SetLog("Init IO.");

                        // 2022.03.08 SungTae Start : [수정] ASE-KR 설비 중 일부 설비(SSG002호기)에서 Alarm 다발하여 Site Option 추가
                        if (CData.CurCompany != ECompany.ASE_KR)
                        {
                            m_Delay.Set_Delay(10000);                           // 10초 주기로 Magazine 준비여부 check
                        }
                        // 2022.03.08 SungTae End

                        m_iStep++;
                        return false;
                    }

                case 12: //오프로더 상태 확인(매거진 유무, 오프로더 Z축 Y축 위치 확인)
                    {
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

                                            m_iStep++;
                                        }
                                        else
                                        {
                                            // 2022.03.08 SungTae Start : [수정] ASE-KR 설비 중 일부 설비(SSG002호기)에서 Alarm 다발하여 Site Option 추가
                                            if (CData.CurCompany == ECompany.ASE_KR)
                                            {
                                                CErr.Show(eErr.OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR);
                                                _SetLog("Error : OFL Bottom not detect magazine.");

                                                m_iStep = 0;
                                                return true;
                                            }
                                            else
                                            {
                                                // 지정 시간 이내라면 기다린다.
                                                if (m_Delay.Chk_Delay())
                                                {
                                                    CErr.Show(eErr.OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR);
                                                    _SetLog("Error : OFL Bottom not detect magazine.");

                                                    m_iStep = 0;
                                                    return true;
                                                }
                                                return false;           // 반복하여 check
                                            }
                                            // 2022.03.08 SungTae End
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

                                            m_iStep++;
                                        }
                                        else
                                        {
                                            // 2022.03.08 SungTae Start : [수정] ASE-KR 설비 중 일부 설비(SSG002호기)에서 Alarm 다발하여 Site Option 추가
                                            if (CData.CurCompany == ECompany.ASE_KR)
                                            {
                                                // 2022.06.08 SungTae Start : [수정]
                                                //CErr.Show(eErr.OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR);
                                                //_SetLog("Error : OFL Bottom not detect magazine.");
                                                CErr.Show(eErr.OFFLOADER_TOP_NOT_DETECT_MGZ_ERROR);
                                                _SetLog("Error : OFL Top not detect magazine.");
                                                // 2022.06.08 SungTae End

                                                m_iStep = 0;
                                                return true;
                                            }
                                            else
                                            {
                                                // 지정 시간 이내라면 기다린다.
                                                if (m_Delay.Chk_Delay())
                                                {
                                                    CErr.Show(eErr.OFFLOADER_TOP_NOT_DETECT_MGZ_ERROR);
                                                    _SetLog("Error : OFL Top not detect magazine.");

                                                    m_iStep = 0;
                                                    return true;
                                                }

                                                return false;
                                            }
                                            // 2022.03.08 SungTae End
                                        }
                                    }
                                }
                            }
                        }
                        
                        return false;
                    }

                case 13: //드라이 R축 0도 이동
                    {
                        CMot.It.MV_R(0, CData.Dev.iDryRPM);
                        _SetLog("R axis move zero.");

                        m_iStep++;
                        return false;
                    }

                case 14: //드라이 R축 0도 이동 확인, 드라이 X축 대기 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iR, 0))
                        { return false; }

                        if(CDataOption.eDryClamp == EDryClamp.Cylinder) // 2022.07.05 lhs
                        {
                            if(!Func_DryClampOpen(false))  // Clamp를 미는 Cylinder가 나오지 않도록
                            {
                                return false;
							}
                            _SetLog("Dryer Cylinder Clamp : Clamped.");
                        }

                        CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 15:  //드라이 X축 대기 위치 이동 확인, 드라이 Z축 Up
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up);
                        m_DuringDryOut = true; //191107 ksg :
                        _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);

                        // 2022.02.10 lhs Start : (SCK+ 전용) Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            m_Delay2.Set_Delay(CData.Opt.nDryZUpStableDelay);
                            _SetLog("Set delay : " + CData.Opt.nDryZUpStableDelay + "ms");
                        }
                        // 2022.02.10 lhs End

                        m_iStep++;
                        return false;
                    }

                case 16: //드라이 Z축 UP 확인, 레일 아웃 에어 온, 드라이 X축 OUt 위치로 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                        {
                            // 2022.02.10 lhs Start : (SCK+ 전용) Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해
                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                            {
                                m_Delay2.Set_Delay(CData.Opt.nDryZUpStableDelay);
                                _SetLog("Set delay : " + CData.Opt.nDryZUpStableDelay + "ms");
                            }
                            // 2022.02.10 lhs End
                            return false; 
                        }

                        // 2022.02.10 lhs Start : (SCK+ 전용)Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (!m_Delay2.Chk_Delay())
                            {
                                return false;
                            }
                        }
                        // 2022.02.10 lhs End

                        // Cylinder Clamp는 UnClamp를 해 주어야 한다.
                        if (CDataOption.eDryClamp == EDryClamp.Cylinder) // 2022.07.05 lhs
                        {
                            if (!Func_DryClampOpen(true))  // UnClamp
                            {
                                return false;
                            }
                            _SetLog("Dryer Cylinder Clamp : UnClamped.");
                        }

                        Func_OutRailAir(true);

                        if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out - 100, CData.Dev.dPushF) == 0)
                        {
                            _SetLog("X axis move position(fast).", CData.SPos.dDRY_X_Out - 100);
                            m_iStep++;
                        }

                        return false;
                    }

                case 17:
                    {
                        // 201007 jym : Out sensor 감지 시 속도 변경
                        if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)

                            CMot.It.Stop(m_iX);             // 2021-06-24, jhLee, 이동을 정지 시킨다. (정지 명령 이후 곧바로 정지되지 않으므로 몇회 더 반복 수행될것으로 예상)

                            //syc : Qorvo - Strip Detect 알람 원인 파악용
                            _SetLog("Strip Detect X Position).", CMot.It.Get_FP((int)EAx.DryZone_X));
                            _SetLog("Strip Detect Sensor : " + CIO.It.Get_X(eX.DRY_StripOutDetect).ToString());

                            // 2021-05-31, jhLee, 센서 감지 후 저속 이동이 진행되지 않는 경우 발생, 저속 이동명령을 자동 Retry 할 수 있도록 변경
                            // 저속으로 이동 명령을 성공적으로 지령하였다면
                            if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS) == 0)
                            {
                                _SetLog("X axis move out(slow).", CData.SPos.dDRY_X_Out);
                                m_iStep = 19;
                            }
                            // 201012 jym : 리턴 빠져있어서 추가
                            return false;
                        }

                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out - 100))
                        { return false; }

                        if (CIO.It.Get_X(eX.DRY_StripOutDetect)) //191108 ksg :
                        {
                            //syc : Qorvo - Strip Detect 알람 원인 파악용
                            _SetLog("Strip Detect X Position).", CMot.It.Get_FP((int)EAx.DryZone_X));
                            _SetLog("Strip Detect Sensor : " + CIO.It.Get_X(eX.DRY_StripOutDetect).ToString());

                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;
                            
                            CErr.Show(eErr.DRY_NOT_DETECTED_STRIP);
                            
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushF) == 0)
                        {
                            _SetLog("X axis move out(fast).", CData.Dev.dPushF);
                            m_iStep++;
                        }

                        return false;
                    }

                case 18: //드라이 X축 OUT위치로 이동 중 레일 아웃 센서 감지 상태 확인
                    {
                        if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)

                            // 2021-05-31, jhLee, 센서 감지 후 저속 이동이 진행되지 않는 경우 발생, 저속 이동명령을 자동 Retry 할 수 있도록 변경
                            if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS) == 0)
                            {
                                _SetLog("X axis move out(slow).", CData.SPos.dDRY_X_Out);
                                m_iStep++;
                            }
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

                case 20: //드라이 X축 대기위치 이동 확인, 종료
                    {
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        {
                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;

                            if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))
                            {
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.DRY_WORK/*21*/, (int)EDataShift.OFL_STRIP_OUT/*23*/);// PickUp Data  -> Dry Data 로 이동
                                
                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Cnt = CData.Parts[DRY].iSlot_No; //191118 ksg :

                                // 2021-05-25, jhLee, Multi-LOT, 각 Strip 정보에 있는 LOT Name 정보를 사용한다.
                                if (CData.IsMultiLOT())
                                {
                                    CData.GemForm.OnCarrierEnded(   CData.Parts[DRY].sLotName, CData.Parts[DRY].sBcr, (uint)CData.Parts[DRY].iSlot_No + 1, CData.Parts[DRY].iTPort + 1, 1);
                                    CData.GemForm.OnCarrierUnloaded(CData.Parts[DRY].sLotName, CData.Parts[DRY].sBcr);
                                }
                                else
                                {   // 기존 Code
                                    CData.GemForm.OnCarrierEnded(   CData.LotInfo.sLotName, CData.Parts[DRY].sBcr, (uint)CData.Parts[DRY].iSlot_No + 1, CData.Parts[DRY].iTPort + 1, 1);
                                    CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[DRY].sBcr);
                                }
                            }

                            int Slot = CSq_OfL.It.GetBtmEmptySlot();
                            //data
                            CData.Parts[(int)EPart.OFL].sLotName    = CData.Parts[DRY].sLotName;
                            CData.Parts[(int)EPart.OFL].iMGZ_No     = CData.Parts[DRY].iMGZ_No;
                            //CData.Parts[(int)EPart.OFL].iSlot_No  = CData.Parts[(int)EPart.DRY].iSlot_No;
                            CData.Parts[(int)EPart.OFL].iSlot_No    = Slot;
                            CData.Parts[(int)EPart.OFL].sBcr        = CData.Parts[DRY].sBcr;
                            CData.Parts[(int)EPart.DRY].sBcr        = ""; //190823 ksg :
                            CData.Parts[(int)EPart.OFL].LotColor    = CData.Parts[(int)EPart.DRY].LotColor;
                            CData.Parts[(int)EPart.OFL].nLoadMgzSN  = CData.Parts[(int)EPart.DRY].nLoadMgzSN;

                            // 2022.05.04 SungTae Start : [추가]
                            if (CData.CurCompany == ECompany.ASE_K12)
                                CData.Parts[(int)EPart.OFL].sMGZ_ID = CData.Parts[(int)EPart.DRY].sMGZ_ID;
                            // 2022.05.04 SungTae End

                            // 2022.05.04 SungTae : [확인용]
                            _SetLog($"LoadMgzSN : DRY({CData.Parts[(int)EPart.DRY].nLoadMgzSN}) -> OFL({CData.Parts[(int)EPart.OFL].nLoadMgzSN})");

                            Array.Copy(CData.Parts[DRY].dPcb, CData.Parts[(int)EPart.OFL].dPcb, CData.Parts[DRY].dPcb.Length);
                            
                            CData.Parts[(int)EPart.OFL].dPcbMax     = CData.Parts[DRY].dPcbMax;
                            CData.Parts[(int)EPart.OFL].dPcbMin     = CData.Parts[DRY].dPcbMin;
                            CData.Parts[(int)EPart.OFL].dPcbMean    = CData.Parts[DRY].dPcbMean;
                            //
                            // 2021.08.21 lhs Start : Top/Btm Mold 데이터 전달
                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.Parts[(int)EPart.OFL].dTopMoldMax = CData.Parts[(int)EPart.DRY].dTopMoldMax;
                                CData.Parts[(int)EPart.OFL].dTopMoldAvg = CData.Parts[(int)EPart.DRY].dTopMoldAvg;
                                CData.Parts[(int)EPart.OFL].dBtmMoldMax = CData.Parts[(int)EPart.DRY].dBtmMoldMax;
                                CData.Parts[(int)EPart.OFL].dBtmMoldAvg = CData.Parts[(int)EPart.DRY].dBtmMoldAvg;
                            }
                            // 2021.08.21 lhs End

                            //CData.Parts[(int)EPart.OFL].iSlot_info[CData.Parts[(int)EPart.OFL].iSlot_No] = (int)eSlotInfo.Exist;
                            CData.Parts[(int)EPart.OFL].iSlot_info[Slot] = (int)eSlotInfo.Exist;
                            
                            if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No--;
                            else                            CData.Parts[(int)EPart.OFL].iSlot_No++;

                            CData.Parts[DRY].bExistStrip = false;

                            // 200314 mjy : 신규추가
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                Array.Copy(CData.Parts[DRY].aUnitEx, CData.Parts[(int)EPart.OFL].aUnitEx, CData.Dev.iUnitCnt);
                            }

                            // 2021-05-31, jhLee, Multi-LOT
                            // 투입 후 LOT을 종료 할 조건인지 Check한다.
                            // 
                            if (CData.IsMultiLOT())
                            {
                                // 지정 LOT의 배출을 처리하였다.
                                CData.LotMgr.SetUnloadStrip(CData.Parts[(int)EPart.OFL].sLotName);

                                // LOT 종료는 Magazine을 Place한 뒤에 판단하기로 한다.

                                // 2022.06.08 SungTae Start : [추가] LOT State 확인용 Log 추가
                                _SetLog($">> Unloading LOT : {CData.Parts[(int)EPart.OFL].sLotName}, Strip ID : {CData.Parts[(int)EPart.OFL].sBcr}, State : {CData.LotMgr.GetLotState(CData.Parts[(int)EPart.OFL].sLotName)}");
                                // 2022.06.08 SungTae End

                                // Unloading중인 LOT의 마지막 Strip인지 Check하여 LOT 종료를 시켜준다.
                                if ( CData.LotMgr.IsUnloadFinishable() == true)     // 현재 배출되고있는 LOT을 종료시켜야 하는가 ?
                                {
									// 2022.03.24 SungTae : [수정] 변수 변경 
                                    // 2021.10.26 SungTae Start : [추가] Multi-LOT 관련 Auto Loading Stop ON 시 LOT COMPLETE 문제로 조건 추가
                                    //if (!CSQ_Main.It.m_bPause/*!CSQ_Main.It.m_bAutoLoadingStop*/)
                                    {
                                        if (CData.Opt.bSecsUse)
                                        {
                                            // 2021.09.03 SungTae Start : [추가] 
                                            CData.GemForm.m_pEndedCarrierList.ClearCarrier();
                                            CData.GemForm.m_pRejectCarrierList.ClearCarrier();
                                            // 2021.09.03 SungTae End 
                                        }

                                        CData.LotMgr.SetCompleteUnloadLOT(CData.Parts[(int)EPart.OFL].sLotName);    //  CData.Parts[(int)EPart.OFL].sLotName);  // 지정 LOT을 종료시킨다 (배출 작업을 마친 LOT의 종료 처리)
                                        _SetLog($"Multi-LOT : LOT Complete [{CData.Parts[(int)EPart.OFL].sLotName}]");
                                    }
                                    // 2021.10.26 SungTae End
                                }

                                CSq_OfL.It.bReadyNextSlot = true;      // 2021-06-07, jhLee : 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는가 ?
                            }

                            //210929 syc : 2004U 
                            if(CDataOption.Use2004U)
                            {
                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                                    (CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_CoverPlace || CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_BtmClean))
								{
									CData.Parts[DRY].iStat = ESeq.DRY_WaitCoverPlace;
								}
							}
                            else
                            {
                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                                    CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd)
                                { 
                                    CData.Parts[DRY].iStat = ESeq.DRY_WorkEnd; 
                                }
                            }
                            
                            CData.LotInfo.iTOutCnt++; //190407 ksg :

                            m_EndSeq  = DateTime.Now         ; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                            _SetLog("Status : " + CData.Parts[(int)EPart.DRY].iStat);
                            _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);


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

            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
            if (!CIO.It.Get_X(eX.DRY_PusherOverload)) //B접 (푸셔 오버로드 감지 되었을때 에러 발생)
            {
                CMot.It.EStop(m_iX);

                int nRet = CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                // 2022.02.10 lhs Start : DRY_PUSHER_OVERLOAD_ERROR시 Pusher가 리턴 안되는 문제 개선
                //                      : 사내 66X 설비에서 테스트 완료, Pusher 속도 안 낮추어도 동작 (Fast 150, Slow 60 확인)
                if (nRet != 0)
                {
                    return false;
                }
                // 2022.02.10 lhs End

                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                _SetLog("Error : Pusher overload.");

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

                        _SetLog("QC Ready ok.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (CData.Opt.bQcByPass)
                        {
                            CGxSocket.It.SendByPass();
                            _SetLog("Send Out for Bypass");
                        }
                        else
                        {
                            CGxSocket.It.SendStartTest();
                            _SetLog("Send Out for Test ");
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //if(CData.Opt.bQcByPass) 
                        //{
                        //    if (!CQcVisionCom.rcvEQReadyQueryOK)
                        //    {
                        //        m_Delay.Wait(3000);
                        //        //CTcpIp.It.SendByPass();
                        //        CGxSocket.It.SendByPass();
                        //        return false;
                        //    }

                        //    _SetLog("By pass ok.");
                        //}
                        //else                    
                        //{
                        //    //if(!CTcpIp.It.bTest_Ok) 
                        //    //if(!CQcVisionCom.rcvEQSendOutACK)
                        //    if (!CQcVisionCom.rcvEQReadyQueryOK)
                        //    {
                        //        m_Delay.Wait(3000);
                        //        //CTcpIp.It.SendTest();
                        //        CGxSocket.It.SendStartTest();
                        //        return false;
                        //    }

                        //    _SetLog("Test ok.");
                        //}
                        
                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//드라이 R축 0도 이동
                        CQcVisionCom.rcvEQSendOutACK = false;
                        
                        CMot.It.MV_R(0, CData.Dev.iDryRPM);
                        _SetLog("R axis move zero.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//드라이 R축 0도 이동 확인, 드라이 X축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iR, 0))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//드라이 X축 대기 위치 이동 확인, 드라이 Z축 Up
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        { return false; }

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dDRY_Z_Up) == 0)
                        {
                            _SetLog("Z axis move up.", CData.SPos.dDRY_Z_Up);
                            m_iStep++;
                        }

                        return false;
                    }

                case 18:
                    {//드라이 Z축 UP 확인, 레일 아웃 에어 온, 드라이 X축 OUt 위치로 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dDRY_Z_Up))
                        { return false; }

                        Func_OutRailAir(true);
                        //CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushF);

                        if (CMot.It.Mv_V(m_iX, (CData.SPos.dDRY_X_Out - 100), CData.Dev.dPushF) == 0)
                        {
                            _SetLog("X axis move Out (Fast) ", CData.SPos.dDRY_X_Out - 100);
                            m_iStep++;
                        }

                        return false;
                    }

                case 19:
                    {
                        // 201007 jym : Out sensor 감지 시 속도 변경
                        if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)

                            CMot.It.Stop(m_iX);             // 2021-06-24, jhLee, 이동을 정지 시킨다. (정지 명령 이후 곧바로 정지되지 않으므로 몇회 더 반복 수행될것으로 예상)

                            // 저속으로 이동 명령을 성공적으로 지령하였다면
                            if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS) == 0)
                            {
                                _SetLog("Sensor Detected, X axsi move out (slow).", CData.SPos.dDRY_X_Out);
                                m_iStep = 21;
                            }

                            // 201012 jym : 리턴 빠져있어서 추가
                            return false;
                        }

                        // 지정 배출 직전 위치까지 도달할때 까지 진행한다.
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out - 100))
                        { return false; }

                        // 배출직전 위치까지 도달하였으나 QC의 Rail 속도에따라서 Strip이 빨리 빠져나가 센서가 감지가 않될 수 도 있다.
                        // 이때는 배출 위치까지 직접 이동 명령을 내려준다.
                        if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushF) == 0)
                        {
                            _SetLog("X axis move out (fast) - Sensor Not Detected", CData.Dev.dPushF);
                            m_iStep++;
                        }
                        return false;
                    }

                case 20:
                    {//드라이 X축 OUT위치로 이동 중 레일 아웃 센서 감지 상태 확인

                        if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {//B접 (자재 감지 되었을때 속도 변경 느리게)

                            CMot.It.Stop(m_iX);             // 2021-06-24, jhLee, 이동을 정지 시킨다. (정지 명령 이후 곧바로 정지되지 않으므로 몇회 더 반복 수행될것으로 예상)

                            if (CMot.It.Mv_V(m_iX, CData.SPos.dDRY_X_Out, CData.Dev.dPushS) == 0)
                            {
                                _SetLog("X axis move out (slow), Sensor Detected, in 2nd check", CData.SPos.dDRY_X_Out);
                                m_iStep++;
                            }
                            return false;
                        }

                        // QC의 Rail 속도에따라서 Strip이 빨리 빠져나가 감지가 않될 수도 있다.
                        // 센서가 감지되지 않더라도 지정된 지점까지 도달하였다면 다음으로 넘어간다.
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out) )
                        {
                            _SetLog("X axis move done (X Out Position), Sensor Not Detected");
                            m_iStep++;
                        }

                        return false;
                    }

                case 21:
                    {//드라이 X축 OUT 위치 이동 확인, 레일 아웃 에어 오프, 드라이 X축 대기 위치 이동
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Out))
                        {
                            Func_OutRailAir(false);

                            if (CMot.It.Mv_N(m_iX, CData.SPos.dDRY_X_Wait) == 0)
                            {
                                _SetLog("X axis move wait.", CData.SPos.dDRY_X_Wait);
                                CGxSocket.It.SendMessage("EQSendEnd");

                                _SetLog("[SEND](EQ->QC) EQSendEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                m_iStep++;      // 2022.01.18 SungTae : [수정] 위치 이동
                            }
                            //m_iStep++;
                        }

                        return false;
                    }

                case 22:
                    {//드라이 X축 대기위치 이동 확인, 종료
                        if (CMot.It.Get_Mv(m_iX, CData.SPos.dDRY_X_Wait))
                        {
                            CData.Parts[DRY].iStat = ESeq.DRY_Wait;

                            if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))
                            {
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.DRY_WORK/*21*/, (int)EDataShift.OFL_STRIP_OUT/*23*/);        // Dry Data  -> OFL Data 로 이동

                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Cnt = CData.Parts[DRY].iSlot_No;    //191118 ksg :

                                // 2022.01.18 SungTae Start : [수정] Multi-LOT 투입 시 Lot Name이 문제될 수 있어서 Dry Part의 Lot Name을 넘겨 주도록 변경.
                                CData.GemForm.OnCarrierEnded(   CData.Parts[DRY].sLotName/*CData.LotInfo.sLotName*/, CData.Parts[DRY].sBcr, (uint)CData.Parts[DRY].iSlot_No + 1, CData.Parts[DRY].iTPort + 1, 1);
                                CData.GemForm.OnCarrierUnloaded(CData.Parts[DRY].sLotName/*CData.LotInfo.sLotName*/, CData.Parts[DRY].sBcr);
                                // 2022.01.18 SungTae End
                            }

                            CData.Parts[DRY].bExistStrip = false;

                            // 2022.03.14 SungTae Start : [수정] (Skyworks VOC) Overload Issue 관련 Site Option으로 구분(발생일 : 2022/03/10)
                            // 2022.01.17 SungTae Start : [수정] (Qorvo향 VOC)
                            /*
                             * <<< 1:1 Matching 기능 사용 시 발견된 문제 >>>
                             * 1) QC에 A LOT의 마지막 Strip이 있는 상태에서 문제 발생하여 조치 전 Dry Zone에 B LOT의 첫번째 Strip이 배출 준비를 하고 있는 상태에서 Auto Run 시작
                             * 2) Off-loader는 다른 MGZ으로 판단하고 Place 동작을 수행하고, QC-Vision은 Off-loader로 Strip Out 동작을 수행
                             *    -> 이때 Strip 파손 발생
                             *    
                             * ※ Part에 QC-Vision을 추가하여 일단 Dry Part의 Data를 OFL에 바로 Write 하지 않고, QC Part에 Write 하도록 변경
                             */

                            if (CData.CurCompany == ECompany.SkyWorks)
                            {
                                CData.Parts[(int)EPart.OFL].sLotName = CData.Parts[DRY].sLotName;
                                CData.Parts[(int)EPart.OFL].iMGZ_No  = CData.Parts[DRY].iMGZ_No;

                                // 2022.03.16 SungTae Start : [추가] Multi-LOT 관련 추가
                                if (CData.IsMultiLOT())
                                {
                                    CData.Parts[(int)EPart.OFL].LotColor    = CData.Parts[DRY].LotColor;
                                    CData.Parts[(int)EPart.OFL].nLoadMgzSN  = CData.Parts[DRY].nLoadMgzSN;
                                }
                                // 2022.03.16 SungTae End
                            }
                            else        // Qorvo 면
                            {
                                CData.Parts[(int)EPart.QCV].sLotName = CData.Parts[DRY].sLotName;
                                CData.Parts[(int)EPart.QCV].iMGZ_No  = CData.Parts[DRY].iMGZ_No;
                                CData.Parts[(int)EPart.QCV].iSlot_No = CData.Parts[DRY].iSlot_No;
                                CData.Parts[(int)EPart.QCV].sBcr     = CData.Parts[DRY].sBcr;
                                CData.Parts[(int)EPart.QCV].sBcr     = "";

                                // 2022.03.16 SungTae Start : [추가] Multi-LOT 관련 추가
                                if (CData.IsMultiLOT())
                                {
                                    CData.Parts[(int)EPart.QCV].LotColor    = CData.Parts[DRY].LotColor;
                                    CData.Parts[(int)EPart.QCV].nLoadMgzSN  = CData.Parts[DRY].nLoadMgzSN;
                                }
                                // 2022.03.16 SungTae End
                                Array.Copy(CData.Parts[DRY].dPcb, CData.Parts[(int)EPart.QCV].dPcb, CData.Parts[DRY].dPcb.Length);

                                CData.Parts[(int)EPart.QCV].dPcbMax  = CData.Parts[DRY].dPcbMax;
                                CData.Parts[(int)EPart.QCV].dPcbMin  = CData.Parts[DRY].dPcbMin;
                                CData.Parts[(int)EPart.QCV].dPcbMean = CData.Parts[DRY].dPcbMean;

                                CData.Parts[(int)EPart.QCV].iSlot_info[CData.Parts[(int)EPart.QCV].iSlot_No] = (int)eSlotInfo.Exist;

                                if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.QCV].iSlot_No--;
                                else                            CData.Parts[(int)EPart.QCV].iSlot_No++;
                                // 2022.01.17 SungTae End
                            }
                            // 2022.03.14 SungTae End

                            // 2022.03.17 SungTae Start : [추가] QC-Vision 사용일 때의 Multi-LOT 관련 추가
                            // 투입 후 LOT을 종료 할 조건인지 Check한다.
                            if (CData.IsMultiLOT())
                            {
                                // 지정 LOT의 배출을 처리하였다.
                                CData.LotMgr.SetUnloadStrip(CData.Parts[(int)EPart.OFL].sLotName);

                                // LOT 종료는 Magazine을 Place한 뒤에 판단하기로 한다.

                                // Unloading중인 LOT의 마지막 Strip인지 Check하여 LOT 종료를 시켜준다.
                                if (CData.LotMgr.IsUnloadFinishable() == true)     // 현재 배출되고있는 LOT을 종료시켜야 하는가 ?
                                {
                                    if (!CSQ_Main.It.m_bPause/*!CSQ_Main.It.m_bAutoLoadingStop*/)	// 2022.03.24 SungTae : [수정] 변수 변
                                    {
                                        if (CData.Opt.bSecsUse)
                                        {
                                            CData.GemForm.m_pEndedCarrierList.ClearCarrier();
                                            CData.GemForm.m_pRejectCarrierList.ClearCarrier();
                                        }

                                        CData.LotMgr.SetCompleteUnloadLOT(CData.Parts[(int)EPart.OFL].sLotName);    // 지정 LOT을 종료시킨다 (배출 작업을 마친 LOT의 종료 처리)

                                        _SetLog($"Multi-LOT : LOT Complete [{CData.Parts[(int)EPart.OFL].sLotName}]");
                                    }
                                }
                            }
                            // 2022.03.17 SungTae End

                            CQcVisionCom.rcvEQReadyQueryOK = false; // QC Ready init

                            if (CData.Parts[(int)EPart.ONL].iStat  == ESeq.ONL_WorkEnd &&
                                CData.Parts[(int)EPart.INR].iStat  == ESeq.INR_WorkEnd &&
                                CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                                CData.Parts[(int)EPart.OFP].iStat  == ESeq.OFP_WorkEnd)
                            { 
                                CData.Parts[DRY].iStat = ESeq.DRY_WorkEnd; 
                            }

                            CSq_OfL.It.bReadyNextSlot = true;      // 2021-06-07, jhLee : 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는가 ?

                            m_EndSeq  = DateTime.Now         ; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                            _SetLog("Status : " + CData.Parts[DRY].iStat + string.Format("Slot [{0}]", CSq_OfL.It.GetBtmEmptySlot()));
                            _SetLog("QC Use Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));
                            
                            CSq_OfL.It.m_DelayRsl.Set_Delay(2000);
                            CQcVisionCom.rcvEQReadyQueryOK = false; // QC Ready init

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
            CIO.It.Set_Y(eY.DRY_BtmStandbyPos, true);
            CIO.It.Set_Y(eY.DRY_BtmTargetPos, false);
        }

        public bool Func_StickStandby()
        {
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
                CIO.It.Set_Y(eY.DRY_BtmStandbyPos, false);
                CIO.It.Set_Y(eY.DRY_BtmTargetPos, true);                
        }

        public bool Func_StickTarget()
        {
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
            if(Run)
            {
                if(!m_TickTime.Chk_Delay()) return;

                if      (Act_Get_Stick_StandyPos()) { Act_Set_Stick_TargetPos(); }
                else if (Act_Get_Stick_TargetPos()) { Act_Set_Stick_StandyPos(); }

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
            //210930 syc : 2004U
            if (CDataOption.Use2004U)
            {
                return CIO.It.Set_Y(eY.DRY_TopAirKnife, bOn);
            }
            else
            {
                return CIO.It.Set_Y(eY.DRY_TopAir, bOn);
            }
            
        }

        /// <summary>
        /// 바텀 에어 블로우 온  true:온   false:오프
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_BtmAir(bool bOn)
        {
            //210930 syc : 2004U
            if(CDataOption.Use2004U)
            {
                return CIO.It.Set_Y(eY.DRY_BtmAirKnife, bOn);
            }
            else
            {
                return CIO.It.Set_Y(eY.DRY_BtmAir, bOn);
            }  
        }

        // 2022.03.15 lhs Start
        /// <summary>
        /// Level Safety Sensor에 설치된 Air Blow (설치여부는 CDataOption에서 설정)
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_LevelSafetyAir(bool bOn)
        {
             return CIO.It.Set_Y(eY.DRY_LevelSafetyAir, bOn);
        }
        // 2022.03.15 lhs End

        // 2022.07.02 lhs Start
        /// <summary>
        /// Dryer Clamp Open(=UnClamp) / Close(=Clamp)
        /// </summary>
        public bool Func_DryClampOpen(bool bSetOpen)
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_ClampOpenOnOff, bSetOpen);

            bool bFrClamped   = CIO.It.Get_X(eX.DRY_Front_Clamp);
            bool bFrUnclamped = CIO.It.Get_X(eX.DRY_Front_Unclamp);
            bool bRrClamped   = CIO.It.Get_X(eX.DRY_Rear_Clamp);
            bool bRrUnclamped = CIO.It.Get_X(eX.DRY_Rear_Unclamp);

            if(bSetOpen)    {   bRet = !bFrClamped &&  bFrUnclamped && !bRrClamped &&  bRrUnclamped;  }  // Open  상태 확인
            else            {   bRet =  bFrClamped && !bFrUnclamped &&  bRrClamped && !bRrUnclamped;  }  // Close 상태 확인

            return bRet;
		}
        // 2022.07.02 lhs Start


        public bool Func_CoverOpen()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.DRY_CoverOpen, true);
            CIO.It.Set_Y(eY.DRY_CoverClose, false);
            bRet = CIO.It.Get_X(eX.DRY_CoverOpen) && !CIO.It.Get_X(eX.DRY_CoverClose);

            return bRet;
        }

        public bool Func_CoverClose()
        {
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
            return CIO.It.Set_Y(eY.DRY_OutRailAir, bOn);
        }

        public bool Func_CheckAir(bool bOn)
        {
            return CIO.It.Set_Y(eY.DRY_CheckAir, bOn);
        }

        //20120112 Abnormal case Strip out 
        public void Func_AbnormalStripOUt()
        {
            if (CMot.It.Get_Mv((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up))
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
                {
                    retValue = true;
                }
            }
            else  // Rotate Type??
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
                {
                    retValue = true;
                }
            }
            
            return retValue;
        }
    }
}