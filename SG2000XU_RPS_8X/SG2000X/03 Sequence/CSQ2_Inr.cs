using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace SG2000X
{
    public class CSq_Inr : CStn<CSq_Inr>
    {
        private readonly int TIMEOUT = 30000;

        private readonly int SECSGEM_BCR_TIMEOUT = 90000; //20200427 jhc : SECG/GEM 사용 시 BCR/ORI Timeout 시간 연장
        /// <summary>
        /// BCR, OCR 측정 실패 시 반복 횟수
        /// </summary>
        private readonly int RETRY = 5;

        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep = 0;
        private int m_iPreStep = 0;
        private string sData = "";

        private readonly int m_iINR = (int)EPart.INR;
        private int m_iX = (int)EAx.Inrail_X;
        private int m_iY = (int)EAx.Inrail_Y;

        //20200601 jhc : InRail BCR/ORI 위치 포지셔닝 시퀀스 수정
        private double m_dInr_X_PreBcrOriPos = 0;   //InRail X축 최초 BCR, ORI 판독 위치로 이동 시 Normal Speed로 이동 위치
        private double m_dInr_X_WaitBcrPos = 0;     //InRail X축 BCR 판독 위치 포지셔닝 후 대기 위치
        private double m_dInr_X_WaitOriPos = 0;     //InRail X축 Orientation 판독 위치 포지셔닝 후 대기 위치
        /// <summary>
        /// 시퀀스 상에서 이동시 사용하는 포지션 변수 [mm]
        /// </summary>
        private double m_dPos = 0;

        /// <summary>
        /// Y Axis Home Done
        /// </summary>
        private bool m_bYHD = false;
        private bool m_bReqStop = false;

        /// <summary>
        /// Delay Timer Class Variable
        /// </summary>
        private CTim m_Delay = new CTim();
        /// <summary>
        /// Time Out Timer Class Variable
        /// </summary>
        private CTim m_TiOut = new CTim();

        private CTim m_ProbeDelay = new CTim();

        private DateTime m_tStTime;
        private TimeSpan m_tTackTime ;

        //20190514 ghk_probe 프로브 업 다운 확인시 저장 할 값
        private double m_dInr_Prb = 0.0;

        //20200328 jhc : 
        private int     m_iDfCurPos = 1;    //현재 측정할 DF Position 번호 (1~5)
        private double  m_dInrXDfPos = 0.0; //InRail X축 DF 측정 위치 (현재 측정할 DF Position 번호에 따라 바뀜)

        private int m_iBcrCnt = 0;

        //EN_SEQ_CYCLE iSeq;
        public ESeq iSeq;
        public ESeq SEQ = ESeq.Idle;

        //20190625 ghk_dfserver
        public bool m_bBcrKeyIn = false;
        public bool m_bBcrErr = false;
        public bool m_bBcrView = false;

        public int  m_DlyYAxis = 500; //190708 ksg :

        //20191111 ghk_bcrocr
        public bool m_bChkBcr = false;

        //210113 pjh : Empty MGZ Lot End Check
        public bool m_LotEndChk = false;

        private CSq_Inr()
        {
            m_bHD = false;
        }

        public void Init_Cyl()
        {
            m_iStep = 10;
            m_iPreStep = 0;
            //20200328 jhc : DF Current Position 초기화
            m_iDfCurPos = 1;
        }

        public bool ToReady()
        {
            m_bReqStop = false    ;
            It.SEQ     = ESeq.Idle;
            return true;
        }

        public bool ToStop()
        {
            m_bReqStop = true;
            // 2020.11.12 JSKim St
            //if(m_iStep != 0) return false;
            //return true;
            if (iSeq == ESeq.INR_Ready)
            {
                return true;
            }
            else
            {
                if (m_iStep != 0) return false;
                return true;
            }
            // 2020.11.12 JSKim Ed
        }

        public bool Stop()
        {
            m_iStep = 0;
            return true;
        }

        public bool AutoRun()
        {
            iSeq = CData.Parts[(int)EPart.INR].iStat;

            if(CDataOption.CurEqu == eEquType.Nomal) //inrail Bcr
            {
                if (SEQ == (int)ESeq.Idle)
                {
                    if (iSeq == ESeq.INR_DynamicFail)
                    {
                        CErr.Show(eErr.INRAIL_NEED_REMOVE_STRIP);
                        return true;
                    }

                    if (m_bReqStop)
                    { return false; }

                    if (CSQ_Main.It.m_iStat == EStatus.Stop)
                    { return false; }

                    bool bBcr   = false;
                    bool bAlign = false;
                    bool bOri   = false;
                    bool bDynamic = false;

                    if (iSeq == ESeq.INR_CheckBcr)      { bBcr      = true; }
                    if (iSeq == ESeq.INR_Align)         { bAlign    = true; }
                    if (iSeq == ESeq.INR_CheckOri)      { bOri      = true; }
                    if (iSeq == ESeq.INR_CheckDynamic)  { bDynamic  = true; }               
                    if (iSeq == ESeq.INR_Ready)
                    {
                        if(CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd) 
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WorkEnd;
                            _SetLog(">>>>> Work End."); // 2021.11.18 lhs 
                        }
                        return false; 
                    }
                    if(iSeq == ESeq.INR_WorkEnd)
                    {
						InrXAxisWait(); //200213 ksg :
                        
						return true;
					}

					if (bBcr)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.INR_CheckBcr;

                        _SetLog(">>>>> BCR start.");
                    }
                    if(bOri)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.INR_CheckOri;

                        _SetLog(">>>>> Orientation start.");
                    }
                    if (bAlign)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.INR_Align;

                        _SetLog(">>>>> Align start.");
                    }
                    if(bDynamic)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        //20200328 jhc : DF Current Position 초기화
                        m_iDfCurPos = 1;
                        //
                        SEQ = ESeq.INR_CheckDynamic;  //inrail Bcr

                        _SetLog(">>>>> Dynamic Function start.");
                    }
                }

                switch (SEQ)
                {
                    default:
                        return false;

                    case ESeq.INR_CheckBcr:
                        {
                            if (Cyl_Bcr())
                            {
                                SEQ = ESeq.Idle;

                                _SetLog("<<<<< BCR finish.");
                            }
                            return false;
                        }
                    //190211 ksg :
                    case ESeq.INR_CheckOri:
                        {
                            if (Cyl_Ori())
                            {
                                SEQ = ESeq.Idle;

                                _SetLog("<<<<< Orientation finish.");
                            }
                            return false;
                        }

                    case ESeq.INR_Align:
                        {
                            if (CDataOption.Package == ePkg.Strip) // 200314-mjy : 조건 추가
                            {
                                if (Cyl_Align())
                                {
                                    SEQ = ESeq.Idle;

                                    _SetLog("<<<<< Align finish.");
                                }
                            }
                            else
                            {
                                if (Cyl_AlignU())
                                {
                                    SEQ = ESeq.Idle;

                                    _SetLog("<<<<< Align finish.");
                                }
                            }
                            return false;
                        }

                    case ESeq.INR_CheckDynamic: //inrail Bcr
                        {
#if true //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                            if (Cyl_DynamicMax5())
#else
                            if (Cyl_Dynamic())
#endif
                            {
                                SEQ = ESeq.Idle;

                                _SetLog("<<<<< Dynamic Function finish.");
                            }
                            return false;
                        }
                }
            }
            else // Picker Bcr cycle
            {
                if (SEQ == (int)ESeq.Idle)
                {

                    if (iSeq == ESeq.INR_DynamicFail)
                    {
                        CErr.Show(eErr.INRAIL_NEED_REMOVE_STRIP);
                        return true;
                    }

                    if (m_bReqStop) return false;
                    if (CSQ_Main.It.m_iStat == EStatus.Stop)
                    { return false; }

                    bool bAlign   = false;
                    bool bDynamic = false;
                    bool bBcr     = false;
                    bool bOri     = false;

                    if(iSeq == ESeq.INR_Align       ){ bAlign   = true; }
                    if(iSeq == ESeq.INR_CheckDynamic){ bDynamic = true; }
                    if(iSeq == ESeq.INR_BcrReady    ){ bBcr     = true; }
                    if(iSeq == ESeq.INR_OriReady    ){ bOri     = true; }//190211 ksg :
                    //if (iSeq == ESeq.INR_Ready || iSeq == ESeq.INR_WaitPicker)
                    if (iSeq == ESeq.INR_Ready)
                    {
                        if(CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd) 
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WorkEnd;
                            _SetLog(">>>>> Work End.");
                        }
                        return false; 
                    }

                    if(iSeq == ESeq.INR_WorkEnd)
                    {
                        InrXAxisWait();

                        return true;
                    }

                    if (bBcr)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.INR_BcrReady;
                        
                        _SetLog(">>>>> BCR start.");
                    }
                    if(bOri)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.INR_OriReady;

                        _SetLog(">>>>> Orientation start.");
                    }
                    if (bAlign)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.INR_Align;

                        _SetLog(">>>>> Align start.");
                    }
                    if(bDynamic)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        //20200328 jhc : DF Current Position 초기화
                        m_iDfCurPos = 1;
                        //
                        SEQ = ESeq.INR_CheckDynamic; //Picker Bcr cycle

                        _SetLog(">>>>> Dynamic Function start.");
                    }
                }

                switch (SEQ)
                {
                    default:
                        return false;
                    case ESeq.INR_Align:
                        {
                            if (CDataOption.Package == ePkg.Strip) // 200314-mjy : 조건 추가
                            {
                                if (Cyl_Align())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Align finish.");
                                }
                            }
                            else
                            {
                                if (Cyl_AlignU())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Align finish.");
                                }
                            }
                            return false;
                        }

                    case ESeq.INR_CheckDynamic: //Picker Bcr cycle
                        {
#if true //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                            if (Cyl_DynamicMax5())
#else
                            if (Cyl_Dynamic())
#endif
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Dynamic Function finish.");
                            }
                            return false;
                        }
                    case ESeq.INR_BcrReady:
                        {
                            if (Cyl_BcrReady())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< BCR Ready finish.");
                            }
                            return false;
                        }
                        //190211 ksg :
                    case ESeq.INR_OriReady:
                        {
                            if (Cyl_OriReady())
                            {
                                SEQ = ESeq.Idle;

                                _SetLog("<<<<< ORI Ready finish.");
                            }
                            return false;
                        }
                }
            }
        }

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

            CLog.Save_Log(eLog.INR, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
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

            CLog.Save_Log(eLog.INR, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }

        /// <summary>
        /// Axes Servo On/Off Cycle
        /// </summary>
        /// <param name="bVal"></param>
        /// <returns></returns>
        public bool Cyl_Servo(bool bVal)
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_SERVO_ON_TIMEOUT);
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
                    {// 1. 모든 축 서보 온
                        CMot.It.Set_SOn(m_iX, bVal);
                        CMot.It.Set_SOn(m_iY, bVal);
                        _SetLog("All servo-on : " + bVal);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {// 2. 서보 온 상태 체크
                        if (CMot.It.Chk_Srv(m_iX) == bVal &&
                            CMot.It.Chk_Srv(m_iY) == bVal)
                        {
                            _SetLog("Check servo.");

                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }
            }
        }

        /// <summary>
        /// Axes Homing Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Home()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_HOME_TIMEOUT);
                    _SetLog("Error : Timeout");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

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
                        //20191203 ghk_display_strip
                        if (!CIO.It.Get_X(eX.INR_StripInDetect))
                        {
                            CErr.Show(eErr.INRAIL_NEED_REMOVE_STRIP);
                            _SetLog("Error : Detected strip on rail.");
                            
                            m_iStep = 0;
                            return true;
                        }
                        
                        if (Chk_Axes(false))
                        {
                            m_iStep = 0;
                            return true;
                        }                        

                        Func_GripperClamp(false);
                        Act_ZLiftDU(false);
                        _SetLog("Check axes.  Gripper open.  Lift down.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        // Timeout Error 추가 필요
                        if (!Func_GripperClamp(false))
                        { return false; }

                        if (!Act_ZLiftDU(false))
                        { return false; }

                        CMot.It.Mv_H(m_iX);
                        _SetLog("X axis homing");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!CMot.It.Get_HD(m_iX))
                        { return false; }

                        if (!CIO.It.Get_X(eX.INR_StripInDetect) || CIO.It.Get_X(eX.INR_StripBtmDetect))
                        {//set error : Strip 제거 
                            CErr.Show(eErr.INRAIL_NEED_REMOVE_STRIP);
                            _SetLog("Error : Detected strip on rail");

                            m_iStep = 0;
                            return true;
                        }
                        
                        CMot.It.Mv_H(m_iY);
                        _SetLog("Y axis homing");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Chk_HD(m_iY))
                        { return false; }
                        if ((!CMot.It.Chk_HD(m_iY)) && (!CMot.It.Chk_HD(m_iX)) && (CMot.It.Chk_OP(m_iX) != EAxOp.Idle))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dINR_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dINR_X_Wait);
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align); //190128 : X 대기 위치로 이동(중국 직원 요청)                        
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dINR_X_Wait))
                        { return false; }

                        //190128 : X 대기 위치로 이동(중국 직원 요청)
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        m_bHD = true;
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Axes Move Wait Position Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Wait()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_WAIT_TIMEOUT);
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
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check axes");

                        m_iStep++;
                        return false;
                    }

                case 11: //온로더 피커 Z축 대기위치 이동
                    {
                        CMot.It.Mv_N((int)EAx.OnLoaderPicker_Z, CData.SPos.dONP_Z_Wait);
                        _SetLog("ONP Z axis move wait", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//온로더 피커 Z축 대기위치 이동 확인, 인레일 그리퍼 오픈, 리프트 다운, 얼라인 포워드
                        if (!CMot.It.Get_Mv((int)EAx.OnLoaderPicker_Z, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        Func_GripperClamp(false);
                        Act_ZLiftDU(false);
                        Act_YAlignBF(true);
                        _SetLog("Gripper open.  Lift down.  Align backward.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//인레일 그리퍼 오픈, 리프트 다운, 얼라인 포워드 확인, Y축 얼라인 위치 이동, X축 대기 위치 이동
                        if (!Func_GripperClamp(false))
                        { return false; }

                        if (!Act_ZLiftDU(false))
                        { return false; }

                        if (!CDataOption.Use2004U)
                        {
                            if (!Act_YAlignBF(true))
                            { return false; }
                        }
                        if (!Chk_Strip())
                        {   // Strip 모드 일 경우
                            CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                            _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);
                        }
                        
                        CMot.It.Mv_N(m_iX, CData.SPos.dINR_X_Wait );
                        _SetLog("X axis move wait.", CData.SPos.dINR_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Y축 얼라인 위치 이동 확인, X축 대기 위치 이동 확인
                        if (!Chk_Strip())
                        {   // Strip 모드 일 경우
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                            { return false; }
                        }                        

                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dINR_X_Wait))
                        { return false; }

                        _SetLog("X, Y axis move check");

                        m_iStep = 0;
                        return true;
                    }
            }
        }
        
        public bool Cyl_DynamicMax5()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            // 2021.01.05 SungTae : 상시 체크에서 해당 Position 이동 완료 후 Check로 변경
            //20191121 ghk_display_strip            
            //if (!Chk_Strip())
            //{//항상 자재가 있어야 됨.
            //    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
            //    CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
            //    _SetLog("Error : Not detect strip.");

            //    m_iStep = 0;
            //    return true;
            //}

            m_iPreStep = m_iStep;

            //DF Position 별 InRail X 축 DF 측정 위치 ////////////////////////////////////
            if      (m_iDfCurPos == 1)  m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos1;
            else if (m_iDfCurPos == 2)  m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos2;
            else if (m_iDfCurPos == 3)  m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos3;
            else if (m_iDfCurPos == 4)  m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos4;
            else if (m_iDfCurPos == 5)  m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos5;
            else    { m_iDfCurPos = 1;  m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos1; }
            /////////////////////////////////////////////////////////////////////////////

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//축 상태 체크
                        //Cycle 시작 : DF Position 1로 초기화 //////////
                        m_iDfCurPos = 1;
                        m_dInrXDfPos = CData.Dev.dInr_X_DynamicPos1;
                        ///////////////////////////////////////////////

                        //20190618 ghk_dfserver
                        if (CDataOption.eDfserver == eDfserver.Use && 
                            !CData.Dev.bDfServerSkip && 
                            CData.dfInfo.sGl != "GL1" && 
                            CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {//다이나믹 펑션 서버 사용, GL2, GL3 일경우 측정 없이 종료 위치로 이동
                             //old if (!CData.Dev.bBcrSkip) //190309 ksg :
                            if (!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) // 2021-05-19, jhLee : BCR/OCR을 읽어야 하는지 여부를 OCR 기능 까지 고려
                            {//바코드 사용
                                CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Bcr);
                            }
                            else
                            {//바코드 스킵
                                if (!CData.Dev.bOriSkip)
                                {//오리 사용
                                    CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Ori);
                                }
                            }

                            _SetLog("DF sever use.  GL2 or GL3.");
                            m_iStep = 31;
                            return false;
                        }
                        //

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }
                        else
                        {
                            _SetLog("Check axes.");

                            m_iStep++; 
                        }                        

                        return false;
                    }

                case 11:
                    {//프로브 업, 프로브 에어 오프
                        for(int i = 0; i < GV.DFPOS_MAX; i++)
                        {
                            CData.Dynamic.dPcb[i] = 999.999;
                        }
                        
                        Act_ProbeUD(false);
                        CIO.It.Set_Y(eY.INR_ProbeAir, false);

                        m_dInr_Prb = Math.Truncate(CPrb.It.Read_Val(EWay.INR));
                        _SetLog("Probe up.  Value : " + m_dInr_Prb + "mm");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//인레일 X축 DF Position 1로 이동
                        CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_DynamicPos1);
                        _SetLog("X axis move DF1.", CData.Dev.dInr_X_DynamicPos1);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//인레일 X축 DF Position 1로 이동 확인, 인레일 그리퍼 클로즈, 자재 유무 확인
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_DynamicPos1))
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_DynamicPos1);
                            return false;
                        }

                        // 2021.01.05 SungTae : 상시 체크에서 해당 Position 이동 완료 후 체크로 변경
                        if (!Chk_Strip())
                        {//항상 자재가 있어야 됨.
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                            CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }
                        Func_GripperClamp(true);
                        _SetLog("Gripper clamp.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//인레일 그리퍼 클로즈 확인
                        if (!Func_GripperClamp(true))
                        { return false; }
                        _SetLog("Gripper clamp check.");

                        m_iStep++;
                        return false;
                    }

                ///////////////////
                // 반복 Loop 시작 //

                case 15:
                    {//인레일 X축 DF Position N(1~5) - 10 이동
                        CMot.It.Mv_S(m_iX, m_dInrXDfPos - 10);
                        _SetLog("X axis move position(slow).", m_dInrXDfPos - 10);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//인레일 X축 DF Position N(1~5) - 10 이동 확인, 인레일 그리퍼 오픈
                        if(!CMot.It.Get_Mv(m_iX, m_dInrXDfPos - 10))
                        {
                            CMot.It.Mv_S(m_iX, m_dInrXDfPos - 10);
                            return false;
                        }
                        Func_GripperClamp(false);
                        _SetLog("Gripper unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//인레일 그리퍼 오픈 확인, 인레일 X축 DF Position N(1~5) 이동
                        if (!Func_GripperClamp(false))
                        { return false; }
                        CMot.It.Mv_S(m_iX, m_dInrXDfPos);
                        _SetLog("X axis move position(slow).", m_dInrXDfPos);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//인레일 X축 DF Position N(1~5) 이동 확인, 인레일 x 축 DF Position N(1~5) - 10 이동
                        if(!CMot.It.Get_Mv(m_iX, m_dInrXDfPos))
                        {
                            CMot.It.Mv_S(m_iX, m_dInrXDfPos);
                            return false;
                        }

                        CMot.It.Mv_S(m_iX, m_dInrXDfPos - 10);
                        _SetLog("X axis move position(slow).", m_dInrXDfPos - 10);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//인레일 X축 DF Position N(1~5) - 10 이동 확인
                        if (!CMot.It.Get_Mv(m_iX, m_dInrXDfPos - 10))
                        {
                            CMot.It.Mv_S(m_iX, m_dInrXDfPos - 10);
                            return false;
                        }
                        _SetLog("X axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//Inrail Y축 얼라인 (자재 움직이지 않도록 고정)
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align + CData.Dynamic.dYAlign);
                        _SetLog("Y axis move position.", CData.Dev.dInr_Y_Align + CData.Dynamic.dYAlign);

                        m_Delay.Set_Delay(m_DlyYAxis); //190708 ksg :

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//Inrail Y축 얼라인 위치 확인
                        if(!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align + CData.Dynamic.dYAlign)) 
                        {
                            CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align + CData.Dynamic.dYAlign); 
                            return false;
                        }

                        if (!m_Delay.Chk_Delay())
                        { return false; } //190708 ksg :
                        _SetLog("Delay check.");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//프로브 에어 ON, 프로브 다운
                        CIO.It.Set_Y(eY.INR_ProbeAir, true); //자재 위의 물을 날려 없애는 목적
                        Act_ProbeUD(true);
                        _SetLog("Probe down.");

                        m_iStep++;
                        return false;
                    }
                case 23:
                    {//프로브 다운 확인, 프로브 에어 off
                        if(GV.INR_PRB_RANGE < Math.Truncate(CPrb.It.Read_Val(EWay.INR)))
                        {//현재 프로브 값이 1보다 클 경우 업상태로 간주 하여 리턴
                            return false;
                        }

                        m_ProbeDelay.Set_Delay(GV.PRB_DELAY);

                        CIO.It.Set_Y(eY.INR_ProbeAir, false);
                        _SetLog("Set delay : " + GV.PRB_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//프로브 값 읽기
                        if (!m_ProbeDelay.Chk_Delay())
                        { return false; }                        

                        int iDfPosIdx = m_iDfCurPos-1;
                        string sPos = "";

                        if(m_iDfCurPos == 1)        sPos = "1st";
                        else if(m_iDfCurPos == 2)   sPos = "2nd";
                        else if(m_iDfCurPos == 3)   sPos = "3rd";
                        else if(m_iDfCurPos == 4)   sPos = "4th";
                        else                        sPos = "5th";
                             
                                                
                        double dPcbHeight = CPrb.It.Read_Val(EWay.INR);

                        dPcbHeight = Math.Round((dPcbHeight - CData.Dynamic.dPcbBase), 4);
                        sData = " - CData.Dynamic.dPcbBase [" + CData.Dynamic.dPcbBase.ToString() + "]";

                        if (CData.Dynamic.dPcbRnr != 0)
                        {
                            dPcbHeight  = dPcbHeight  + CData.Dynamic.dPcbRnr;
                            sData = " + CData.Dynamic.dPcbRnr [" + CData.Dynamic.dPcbRnr + "]";
                        }

                        //읽은 프로브 값을 현재 배열의 현재 DF Position 인덱스에 저장 ///////
                        CData.Dynamic.dPcb[iDfPosIdx] = dPcbHeight;
                        /////////////////////////////////////////////////////////////////

                        _SetLog(sData);

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//프로브 업
                        Act_ProbeUD(false);
                        _SetLog("Probe up.");

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {//프로브 업 확인, 인레일 X축 DF Position N(1~5) 이동
                        if((m_dInr_Prb - GV.INR_PRB_RANGE) > Math.Truncate(CPrb.It.Read_Val(EWay.INR)))
                        {
                            return false;
                        }

                        CMot.It.Mv_S(m_iX, m_dInrXDfPos);
                        _SetLog("X axis move position(slow).", m_dInrXDfPos);

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {//인레일 X축 DF Position N(1~5) 이동 확인, 자재 확인, 그리퍼 클로즈
                        if (!CMot.It.Get_Mv(m_iX, m_dInrXDfPos))
                        {
                            CMot.It.Mv_S(m_iX, m_dInrXDfPos);
                            return false;
                        }

                        // 2021.01.05 SungTae : 상시 체크에서 해당 Position 이동 완료 후 체크로 변경
                        if (!Chk_Strip())
                        {//항상 자재가 있어야 됨.
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                            CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;

                            m_iStep = 0;
                            return true;
                        }

                        Func_GripperClamp(true);
                        _SetLog("Gripper clamp.");

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {//그리퍼 클로즈 확인, Inrail Y축 벌림 (DF 측정 전 원래 위치)
                        if (!Func_GripperClamp(true))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 29:
                    {//Inrail Y축 얼라인 위치 확인
                        if(!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align)) 
                        {
                            CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align); 
                            return false;
                        }

                        ////////////////////////////////////////////////////////////////////
                        // 시퀀스 분기 : 다음 DF 위치에 대한 시퀀스 반복 또는 시퀀스 종료 판단 //
                        ////////////////////////////////////////////////////////////////////

                        if( m_iDfCurPos < CData.Dev.iDynamicPosNum )
                        {
                            //////////////////////////////////////////////////////
                            // 아직 측정할 DF 위치가 남음 >> 번호 증가 & 반복 Loop //

                            m_iDfCurPos++; //다음 DF 측정 위치 설정
                            _SetLog("DF loop.  Count : " + m_iDfCurPos);

                            m_iStep = 15;  //Loop
                        }
                        else
                        {
                            ////////////////////////////////////////////////////////////
                            // 모든 DF 위치 측정 완료 >> 반복 Loop 종료 >> 마무리 시퀀스 //
                            // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                            //if(CData.GemForm != null) CData.GemForm.Strip_Data_Shift(1,3); // Push Data -> DF Data 로 이동
                            if (CData.GemForm != null)
                            {
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.ONL_MGZ_PICK/*1*/, (int)EDataShift.INR_DF_MEAS/*3*/); // Push Data -> DF Data 로 이동
                            }
                            // 2021.07.19 SungTae End

                            m_iDfCurPos = 1; //DF 측정 위치 번호 초기화
                            _SetLog("DF loop end.");

                            m_iStep++;
                        }
                        return false;
                    }

                // 반복 Loop 종료 //
                ///////////////////

                case 30:
                    {//측정 데이터 검사, 계산, Data Shift

                        /////////////////////////////////////////////////////////////////
                        double[] dDynPcb = new double[CData.Dev.iDynamicPosNum];
                        /////////////////////////////////////////////////////////////////

                        for(int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                        {
                            //개별 측정값 검사
                            if(CData.Dynamic.dPcb[i] == 999.999)
                            {
                                CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_READING_FAIL);
                                _SetLog("Error : DF reading fail.");
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;
                                Func_GripperClamp(false);

                                m_iStep = 0;
                                return true;
                            }
                            //개별 측정값의 PCB Range 조건 검사
                            if(CData.Dynamic.dPcbRange != 0)
                            {
                                if(CData.Dynamic.dPcb[i] > (CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbRange) ||
                                    CData.Dynamic.dPcb[i] < (CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbRange)) //190710 ksg :
                                {
                                    CLog.mLogGnd(string.Format("CSQ2_Inr Step - 30 CData.Dynamic.dPcbRange : INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER  dPcb[{0}] = {1}, [Device Setting] Range Min/Man : {2} ~ {3} : Pcb thickness = {4}, Range = {5}",
                                                i, CData.Dynamic.dPcb[i], CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbRange, CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbRange,
                                                CData.Dev.aData[0].dPcbTh , CData.Dynamic.dPcbRange));

                                    CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER);
                                    _SetLog("Error : DF PCB height rangeover1.");
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;
                                    Func_GripperClamp(false);

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            /////////////////////////////////////////
                            dDynPcb[i] = CData.Dynamic.dPcb[i];
                            /////////////////////////////////////////

                            // LCY 200131
                            if(CData.GemForm != null) {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Data[0,i] = CData.Dynamic.dPcb[i];
                                CLog.mLogGnd(string.Format("CSQ2_Inr Step - 30  dPcb[{0}] = {1} ",i, CData.Dynamic.dPcb[i]));
                            }
                        }

                        // Max, Average 계산 /////////////////////////////////////////
                        // 실제 측정된 수량 내에서만 MAX, AVERAGE 값을 구해야 함
                        CData.Dynamic.dPcbMax  = Math.Round(dDynPcb.Max(),      4);
                        CData.Dynamic.dPcbMean = Math.Round(dDynPcb.Average(),  4);
                        //////////////////////////////////////////////////////////////

                        //PCB 두께 평균값의 Range 조건 검사
                        if(CData.Dynamic.dPcbMeanRange != 0)
                        {
                            if (CData.Dynamic.dPcbMean > (CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbMeanRange) ||
                                CData.Dynamic.dPcbMean < (CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbMeanRange)) //190827 ksg:
                            {
                                CLog.mLogGnd(string.Format("CSQ2_Inr Step - 30 CData.Dynamic.dPcbMeanRange : INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER  dPcbMeam = {0} , [Device Setting] Range Min/Max : {1} ~ {2}, Pcb thickness = {3}, Range = {4}",
                                                CData.Dynamic.dPcbMean, CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbRange, CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbRange,
                                                CData.Dev.aData[0].dPcbTh , CData.Dynamic.dPcbRange));
                            
                                m_iStep = 0;
                                CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER);
                                _SetLog("Error : DF PCB height rangeover2.");
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;
                                Func_GripperClamp(false);
                                
                                return true;
                            }
                        }

                        // Data Shift (DF -> InRail)
                        Array.Copy(CData.Dynamic.dPcb, CData.Parts[(int)EPart.INR].dPcb, CData.Dynamic.dPcb.Length);
                        //실제 측정된 수치들 내에서만 MIN 값을 구해야 함
                        //CData.Parts[(int)EPart.INR].dPcbMin  = Math.Round(CData.Parts[(int)EPart.INR].dPcb.Min()    , 4);
                        CData.Parts[(int)EPart.INR].dPcbMin     = Math.Round(dDynPcb.Min(), 4);
                        CData.Parts[(int)EPart.INR].dPcbMax     = CData.Dynamic.dPcbMax;
                        CData.Parts[(int)EPart.INR].dPcbMean    = CData.Dynamic.dPcbMean;

                        // LCY 200131
                        if (CData.GemForm != null)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0] = CData.Parts[(int)EPart.INR].dPcbMin;  // [3=DF Measure 후].[0=DF]
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0] = CData.Parts[(int)EPart.INR].dPcbMax;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0] = CData.Parts[(int)EPart.INR].dPcbMean;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].nDF_User_Mode        = CData.Dynamic.iHeightType;
                            CLog.mLogGnd(string.Format("CSQ2_Inr Step - 30 GL1 ,fMeasure_Min_Data {0},{1},{2}",
                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0],
                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0],
                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0]));
                        }
						
                        if (!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) //190309 ksg :
                        {//바코드 사용
                            CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Bcr);
                        }
                        else
                        {//바코드 스킵
                            if (!CData.Dev.bOriSkip)
                            {//오리 사용
                                CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Ori);
                            }
                        }

                        _SetLog(string.Format("Pcb Min : {0}mm  Max : {1}mm  Mean:{2}mm",   CData.Parts[(int)EPart.INR].dPcbMin, 
                                                                                            CData.Parts[(int)EPart.INR].dPcbMax, 
                                                                                            CData.Parts[(int)EPart.INR].dPcbMean));

                        m_iStep++;
                        return false;
                    }

                case 31:
                    {
                        // old if (!CData.Dev.bBcrSkip) //190309 ksg :
                        if (!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) // 2021-05-19, jhLee : BCR/OCR을 읽어야 하는지 여부를 OCR 기능 까지 고려
                        {//바코드 사용
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                            {
                                return false;
                            }
                            if(CDataOption.CurEqu == eEquType.Nomal) CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr;
                            else                                     CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; //190821 ksg :
                        }
                        else
                        {//바코드 스킵
                            if (!CData.Dev.bOriSkip)
                            {//오리 사용
                                if(!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Ori))
                                {
                                    return false;
                                }
                                if(CDataOption.CurEqu == eEquType.Nomal) CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                                else                                     CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                            }
                            else
                            {//오리 스킵
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                            }
                        }

                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20200601 jhc : InRail BCR/ORI 위치 포지셔닝 시퀀스 수정 ///////////////////////////////////
        
        /// <summary>
        /// InRail X축 최초 BCR, ORI 판독 위치로 이동 시 Normal Speed로 이동 위치 파악
        /// 자재가 현재 어디 있는지 모르는 상태에서 이 위치부터 Slow Speed로 이동 시작
        /// </summary>
        /// <returns></returns>
        private double Chk_InrX_PreBcrOriPos()
        {
#if true
            return (CData.Dev.dInr_X_Bcr - 10);
#else
            double [] dX = new double[3];
            dX[0] = CData.Dev.dInr_X_Align;
            dX[1] = CData.Dev.dInr_X_Bcr;
            dX[2] = CData.Dev.dInr_X_Ori;

            double dMin = dX.Min() - 10;
            if (dMin > 0)
            { return dMin; }
            else
            { return 0; }
#endif
        }

        /// <summary>
        /// InRail X축 BCR 판독 위치 포지셔닝 후 대기 위치 파악
        /// </summary>
        /// <returns></returns>
        private double Get_InrX_WaitBcrPos()
        {
            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            { return (CData.Dev.dInr_X_Bcr - 40); }
            else
            { return (CData.Dev.dInr_X_Bcr - 10); } //191202 ksg :
        }

        /// <summary>
        /// InRail X축 Orientation 판독 위치 포지셔닝 후 대기 위치 파악
        /// </summary>
        /// <returns></returns>
        private double Get_InrX_WaitOriPos()
        {
            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            { return (CData.Dev.dInr_X_Ori - 40); }
            else
            { return (CData.Dev.dInr_X_Ori - 10); } //191202 ksg :
        }

        /// <summary>
        /// Move Barcode Check Position Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Bcr()
        { 
            // Timeout check
            if (m_iPreStep != m_iStep)
            {
                if(CData.Opt.bSecsUse)
                { m_TiOut.Set_Delay(SECSGEM_BCR_TIMEOUT); } //20200427 jhc : SECG/GEM 사용 시 BCR/ORI Timeout 시간 연장
                else
                { m_TiOut.Set_Delay(TIMEOUT); }
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_BARCODE_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (!Chk_Strip())
            {//항상 자재가 있어야 됨.
                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                _SetLog("Error : Not detected strip.  Status : INR_Ready");

                m_iStep = 0;
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
                //190617 ksg :
                case 10:
                    {
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        CBcr.It.Save_RunStatus();
                        m_Delay.Set_Delay(1000);

                        //190801 ghk_Keyin
                        m_bBcrKeyIn = false;
                        m_bBcrErr = false;
                        m_bBcrView = false;
                        // 201029 jym : 변수 변경
                        CData.VwKey.bView = false;

                        //20191111 ghk_ocrbcr
                        m_bChkBcr = false;

                        _SetLog("Check axes.  Save run status.");

                        m_iStep++;
                        //m_iStep = 27;     // 2021.08.24 SungTae : [수정] Remote 상에서 Multi-LOT 기능 디버깅 위해 임시로 변경
                        return false;
                    }

                case 11: 
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }
                        _SetLog("Check delay.");

                        m_iStep++; 
                        return false;
                    }

                case 12:    //바코드 프로그램 런 체크
                    {
                        //20200314 jhc : 2D Vision (UDP) Interface
                        if(CDataOption.BcrInterface == eBcrInterface.Udp)
                        {
                            //2D Vision SW Run/연결 상태 체크
                            if (!CBcr.It.Chk_Run())
                            {
                                CErr.Show(eErr.BCR_2DVISION_CHKRUN_ERROR);
                                _SetLog("Error : 2D Vision check run fail.");

                                m_iStep = 0;
                                return true;
                            }
                            //2D Vision SW Alarm 체크
                            else if ((CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                            {
                                CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                                _SetLog("Error : 2D Vision alarm.");

                                CBcr.It.b2DVisAlarmNotify = false;
                                CBcr.It.s2DVisAlarmMsg = "";

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else if (!CBcr.It.Chk_Run())
                        {
                            CErr.Show(eErr.INRAIL_BARCODE_NOT_READY);
                            _SetLog("Error : BCR not ready1.");

                            m_iStep = 0;
                            return true;
                        }

                        CBcr.It.Load();
                        if(CData.Bcr.bStatusBcr)
                        {
                            CErr.Show(eErr.BCR_NOT_READY);
                            _SetLog("Error : BCR not ready2.");

                            m_iStep = 0;
                            return true;
                        }

                        CData.Bcr.sBcr = "";

                        _SetLog("BCR ready.");

                        m_iStep++;
                        return false;
                    }

                case 13:    //리프트 다운, 그리퍼 오픈, Y축 얼라인 위치 이동
                    {
                        Act_ZLiftDU(false);
                        Func_GripperClamp(false);
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 14:    //리프트 다운 확인, 그리퍼 오픈 확인, Y축 얼라인 위치 이동 확인, X축 BCR 체크 사전 위치 이동(Normal Speed)
                    {
                        if (!Act_ZLiftDU(false))
                        { return false; }
                        if (!Func_GripperClamp(false))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        //X축 BCR 체크 사전 위치 이동(Normal Speed)
                        m_dInr_X_PreBcrOriPos = Chk_InrX_PreBcrOriPos();
                        CMot.It.Mv_N(m_iX, m_dInr_X_PreBcrOriPos);
                        _SetLog("X axis move position.", m_dInr_X_PreBcrOriPos);

                        m_iStep++;
                        return false;
                    }

                case 15:    //X축 BCR 체크 사전 위치 이동 확인, X축 BCR 체크 위치 이동(Slow Speed)
                    {
                        m_dInr_X_PreBcrOriPos = Chk_InrX_PreBcrOriPos();
                        if (!CMot.It.Get_Mv(m_iX, m_dInr_X_PreBcrOriPos))
                        { 
                            CMot.It.Mv_N(m_iX, m_dInr_X_PreBcrOriPos);
                            return false; 
                        }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr); //X축 BCR 체크 위치 이동(Slow Speed)
                        _SetLog("X axis move bcr(slow).", CData.Dev.dInr_X_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 16:    //X축 BCR 체크 위치 이동 확인, 그리퍼 클로즈
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                        { 
                            CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr);
                            return false; 
                        }

                        Func_GripperClamp(true);
                        _SetLog("Gripper close.");

                        m_iStep++;
                        return false;
                    }

                case 17:    //그리퍼 클로즈 확인, X축 (BCR 체크 - 10) 위치 이동(슬로우), 그리퍼 센서 감지 확인
                    {
                        if (!Func_GripperClamp(true))
                        { return false; }

                        m_dPos = CData.Dev.dInr_X_Bcr - 10;
                        CMot.It.Mv_S(m_iX, m_dPos);
                        _SetLog("X axis move position(slow).", m_dPos);

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        m_iStep++;
                        return false;
                    }

                case 18:    //X축 (BCR 체크 - 10) 위치 이동 확인, 그리퍼 센서 감지 확인, 그리퍼 오픈
                    {
                        if (!CMot.It.Get_Mv(m_iX, m_dPos))
                        {
                            CMot.It.Mv_S(m_iX, m_dPos);
                            return false;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper open.");

                        m_iStep++;
                        return false;
                    }

                case 19:    //그리퍼 오픈 확인, X축 BCR 체크 위치 이동(슬로우)
                    {
                        if (!Func_GripperClamp(false))
                        { return false; }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr);
                        _SetLog("X axis move bcr(slow).", CData.Dev.dInr_X_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 20:    //X축 BCR 체크 위치 이동 확인, 그리퍼 센서 감지 확인, X축 BCR 체크 대기 위치 이동(슬로우)
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                        { 
                            CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr);
                            return false; 
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        m_dInr_X_WaitBcrPos = Get_InrX_WaitBcrPos();
                        CMot.It.Mv_S(m_iX, m_dInr_X_WaitBcrPos);
                        _SetLog("X axis move position(slow).", m_dInr_X_WaitBcrPos);

                        m_iStep++;
                        return false;
                    }

                case 21:    //X축 BCR 체크 대기 위치 이동 확인
                    {
                        m_dInr_X_WaitBcrPos = Get_InrX_WaitBcrPos();
                        if (!CMot.It.Get_Mv(m_iX, m_dInr_X_WaitBcrPos))
                        {
                            CMot.It.Mv_S(m_iX, m_dInr_X_WaitBcrPos);
                            return false; 
                        }

                        _SetLog("X axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 22:    //바코드 읽기
                    {
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Bcr();
                        if (CDataOption.BcrPro == eBcrProtocol.Old)
                        { CBcr.It.Chk_Bcr(m_bChkBcr); }
                        else
                        { CBcr.It.Chk_Bcr_InR(m_bChkBcr); }
                        // 201104 jym : 대기시간 1초에서 2초로 변경
                        m_Delay.Set_Delay(2000); 
                        _SetLog("Check bcr.  Set delay : 2000ms");

                        m_iStep++;
                        return false;
                    }

                case 23:    // Bcr 로드
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        //20200314 jhc : 2D Vision SW Alarm 체크
                        if((CDataOption.BcrInterface == eBcrInterface.Udp) && (CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                        {
                            CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                            _SetLog("Error : 2D Vision alarm.");

                            CBcr.It.b2DVisAlarmNotify = false;
                            CBcr.It.s2DVisAlarmMsg = "";
                            
                            m_iStep = 0;
                            return true;
                        }
                        
                        CBcr.It.Load();

                        if(CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;
                        _SetLog("BCR load.");

                        m_iStep++;
                        return false;
                    }

                case 24:    //바코드 읽은 값 확인, 바코드 값 저장 후 상태 변경
                    {
                        if (CData.CurCompany != ECompany.SkyWorks)
                        {
                            //if (!CData.Bcr.bRet || CData.Bcr.sBcr.Equals("error"))
                            if (!CData.Bcr.bRet || CData.Bcr.sBcr.Equals("error") || CData.Bcr.sBcr.Contains(" "))//211230 pjh : OCR에 공백 포함 시 Key In 창 표시
                            {
                                if (m_iBcrCnt > RETRY)
                                {
                                    if (CData.Dev.bBcrKeyInSkip)
                                    {
                                        CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                        _SetLog("Error : Reading fail.  Key-in skip.");

                                        m_iBcrCnt = 0; //190211 ksg :
                                        m_iStep = 0;
                                        return true;
                                    }
                                    else
                                    {
                                        m_bBcrView = true;

                                        if (m_bBcrErr)
                                        {
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : Reading fail.");

                                            m_iBcrCnt = 0;
                                            m_bBcrView = false;

                                            m_iStep = 0;
                                            return true;
                                        }
                                        if (m_bBcrKeyIn)
                                        {
                                            m_bBcrView = false;
                                        }
                                        if (!m_bBcrErr && !m_bBcrKeyIn)
                                        { return false; }
                                    }
                                }
                                else
                                {
                                    _SetLog("Retry.  Count : " + m_iBcrCnt);

                                    m_iStep = 22;
                                    return false;
                                }
                            }
                        }
                        else  
                        {   // OCR 사용시
                            if (!CData.Dev.bBcrSkip && CData.Dev.bOcrSkip)      // BCR Only
                            {
                                //if (!CData.Bcr.bRet || CData.Bcr.sBcr.ToUpper().Equals("ERROR"))// BCR 실패
                                if (!CData.Bcr.bRet || CData.Bcr.sBcr.ToUpper().Equals("ERROR") || CData.Bcr.sOcr.Contains(" "))//211230 pjh : OCR에 공백 포함 시 Key In 창 표시
                                {
                                    if (m_iBcrCnt > RETRY)
                                    {
                                        if (CData.Dev.bBcrKeyInSkip)
                                        {
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : Reading fail.  Key-in skip.");

                                            m_iBcrCnt = 0; //190211 ksg :
                                            m_iStep = 0;
                                            return true;
                                        }
                                        else
                                        {
                                            // 201029 jym : 다른 변수로 대체
                                            //m_bBcrView = true;
                                            CData.VwKey.bView = true;

                                            // 201023 jym : 추가
                                            m_TiOut.Set_Delay(TIMEOUT);

                                            if (m_bBcrErr)
                                            {
                                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                                _SetLog("Error : Reading fail.");

                                                m_iBcrCnt = 0;
                                                // 201029 jym : 다른 변수로 대체
                                                //m_bBcrView = false;
                                                CData.VwKey.bView = false;

                                                m_iStep = 0;
                                                return true;
                                            }
                                            if (m_bBcrKeyIn)
                                            {
                                                // 201029 jym : 다른 변수로 대체
                                                //m_bBcrView = false;
                                                CData.VwKey.bView = false;
                                            }
                                            if (!m_bBcrErr && !m_bBcrKeyIn)
                                            { return false; }
                                        }
                                    }
                                    else
                                    {
                                        _SetLog("Retry.  Count : " + m_iBcrCnt);

                                        m_iStep = 22;
                                        return false;
                                    }
                                }
                            }
                            else if (CData.Dev.bBcrSkip && !CData.Dev.bOcrSkip)     // OCR Only
                            {
                                if (!CData.Bcr.bRetOcr || CData.Bcr.sOcr.ToUpper().Equals("ERROR"))  // OCR 실패
                                {
                                    if (m_iBcrCnt > RETRY)
                                    {
                                        if (CData.Dev.bBcrKeyInSkip)
                                        {
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : Reading fail.  Key-in skip.");

                                            m_iBcrCnt = 0; //190211 ksg :
                                            m_iStep = 0;
                                            return true;
                                        }
                                        else
                                        {
                                            // 201029 jym : 다른 변수로 대체
                                            //m_bBcrView = true;
                                            CData.VwKey.bView = true;

                                            // 201023 jym : 추가
                                            m_TiOut.Set_Delay(TIMEOUT);

                                            if (m_bBcrErr)
                                            {
                                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                                _SetLog("Error : Reading fail.");

                                                m_iBcrCnt = 0;
                                                // 201029 jym : 다른 변수로 대체
                                                //m_bBcrView = false;
                                                CData.VwKey.bView = false;

                                                m_iStep = 0;
                                                return true;
                                            }
                                            if (m_bBcrKeyIn)
                                            {
                                                // 201029 jym : 다른 변수로 대체
                                                //m_bBcrView = false;
                                                CData.VwKey.bView = false;
                                            }
                                            if (!m_bBcrErr && !m_bBcrKeyIn)
                                            { return false; }
                                        }
                                    }
                                    else
                                    {
                                        _SetLog("Retry.  Count : " + m_iBcrCnt);

                                        m_iStep = 22;
                                        return false;
                                    }
                                }
                            }
                            else                                                    // BCR + OCR
                            {
                                //2020-10-29, jhLee, 성공여부 체크 변경

                                //// BCR 혹은 OCR에서 읽기에 성공하였나 ?
                                //if (((CData.Bcr.bRet && CData.Bcr.sBcr.ToUpper() != "ERROR")            // BCR 성공
                                //    || (CData.Bcr.bRetOcr && CData.Bcr.sOcr.ToUpper() != "ERROR"))       // 글자수 10글자 조건은 일시로 해제
                                //    && !m_bChkBcr)  // 이미 처리를 하지 않은것이라면
                                //{
                                //    // 읽기 성공
                                //    _SetLog("BCR + OCR Read ok : " + CData.Bcr.sBcr + ", " + CData.Bcr.sOcr);

                                //    // 다음 루틴으로 가야하므로 아무것도 하지 않는다.
                                //}
                                //else   // 읽기 실패
                                //{
                                    //20191111 ghk_bcrocr
                                    if ((!CData.Bcr.bRet || CData.Bcr.sBcr.ToUpper().Equals("ERROR")) && !m_bChkBcr)
                                    {
                                        if (m_iBcrCnt > RETRY)
                                        {
                                            m_iBcrCnt = 0;
                                            m_bChkBcr = true;
                                        }
                                        else
                                        {
                                            _SetLog("Retry.  Count : " + m_iBcrCnt);

                                            m_iStep = 22;
                                            return false;
                                        }
                                    }

                                    //if ((!CData.Bcr.bRetOcr || CData.Bcr.sOcr == "error") && m_bChkBcr)
                                    if ((!CData.Bcr.bRetOcr || CData.Bcr.sOcr.Length != 10) && m_bChkBcr)
                                    {//[임시] 글자 수 10자가 아니면 에러
                                        if (m_iBcrCnt > RETRY)
                                        {
                                            if (CData.Dev.bBcrKeyInSkip)
                                            {
                                                CErr.Show(eErr.INRAIL_BCR_OCR_READING_FAIL);
                                                _SetLog("Error : OCR reading fail.  Key-in skip.");

                                                m_iBcrCnt = 0; //190211 ksg :
                                                m_iStep = 0;
                                                return true;
                                            }
                                            else
                                            {
                                                // 201029 jym : 다른 변수로 대체
                                                //m_bBcrView = true;
                                                CData.VwKey.bView = true;

                                                // 201023 jym : 추가
                                                m_TiOut.Set_Delay(TIMEOUT);

                                                if (m_bBcrErr)
                                                {
                                                    CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                                    _SetLog("Error : OCR reading fail.");

                                                    m_iBcrCnt = 0;
                                                    // 201029 jym : 다른 변수로 대체
                                                    //m_bBcrView = false;
                                                    CData.VwKey.bView = false;

                                                    m_iStep = 0;
                                                    return true;
                                                }
                                                if (m_bBcrKeyIn)
                                                {
                                                    // 201029 jym : 다른 변수로 대체
                                                    //m_bBcrView = false;
                                                    CData.VwKey.bView = false;
                                                }
                                                if (!m_bBcrErr && !m_bBcrKeyIn)
                                                { return false; }
                                            }
                                        }
                                        else
                                        {
                                            _SetLog("OCR retry.  Count : " + m_iBcrCnt);

                                            m_iStep = 22;
                                            return false;
                                        }
                                    }
                                //}//of if 읽기 성공 else
                            } //of if ... BCR+OCR
                        }
                        //20190703 ghk_dfserver
                        //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip && (CData.dfInfo.sGl != "GL1") && CSQ_Main.It.m_iStat == eStatus.Auto_Running)
                        if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && (CData.dfInfo.sGl != "GL1") && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            CData.dfInfo.sBcr = CData.Bcr.sBcr;
                            if (!CData.dfInfo.bBusy)
                            {
                                CDf.It.SendInrBcr();
                                _SetLog("DF Server - Send bcr.  BCR : " + CData.Bcr.sBcr);

                                m_iStep++;
                            }
                            return false;
                        }
                        else
                        {
                            if (CData.Dev.bBcrSkip && !CData.Dev.bOcrSkip)
                            {//BCR 스킵이고, OCR 스킵 아닐때
                                CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sOcr;
                                _SetLog("BCR skip O.  OCR skip X.  OCR : " + CData.Bcr.sOcr);
                            }
                            else if (!CData.Dev.bBcrSkip && CData.Dev.bOcrSkip)
                            {//BCR 스킵 아니고, OCR 스킵 일때
                                CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sBcr;
                                _SetLog("BCR skip X.  OCR skip O.  BCR : " + CData.Bcr.sBcr);
                            }
                            else
                            {//BCR 스킵 아니고, OCR 스킵 아닐때
                                if (m_bChkBcr)
                                {//BCR NG 일때
                                    CData.Bcr.sBcr = CData.Bcr.sOcr;
                                    CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sOcr;
                                    _SetLog("BCR skip X.  OCR skip X.  BCR NG.  OCR : " + CData.Bcr.sOcr);
                                }
                                else
                                {//BCR OK 일때
                                    CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sBcr;
                                    _SetLog("BCR skip X.  OCR skip X.  BCR OK.  BCR : " + CData.Bcr.sBcr);
                                }
                            }

                            // 201111 jym START : Skyworks 조건 추가
                            if (CData.CurCompany == ECompany.SkyWorks)
                            { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; }
                            else
                            { CData.Parts[(int)EPart.INR].iStat = (CData.Dev.bOriSkip) ? ESeq.INR_Align : ESeq.INR_CheckOri; }
                            // 201111 jym END

                            if (CData.Opt.bSecsUse && CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = CData.Parts[(int)EPart.INR].sBcr;
                                _SetLog(string.Format("BCR : {0}  OCR : {1} CData.JSCK_Gem_Data[3].sInRail_Strip_ID {2}", CData.Bcr.sBcr, CData.Bcr.sOcr, CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID));
                            }
                            else
                            { _SetLog(string.Format("BCR : {0}  OCR : {1} ", CData.Bcr.sBcr, CData.Bcr.sOcr)); }

                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);
                            m_iBcrCnt = 0; 

                            m_iStep = 0;
                            return true;
                        }
                    }

                //20190618 ghk_dfserver
                case 25:
                    {
                        if (CDf.It.ReciveInrBcr() == 1)
                        {
                            CErr.Show(eErr.DYNAMIC_FUNCTION_SERVER_NOTHING_GL1_DATA);
                            _SetLog("DF server nothing GL1.");
                            m_iBcrCnt = 0;

                            m_iStep = 0;
                            return true;
                        }
                        else if (CDf.It.ReciveInrBcr() == 0)
                        {
                            _SetLog("DF server ok.");
                            m_iStep++;
                        }

                        return false;
                    }

                case 26:
                    {
                        if (!CData.dfInfo.bBusy)
                        {
                            CDf.It.SendResultBcr();
                            _SetLog("DF server send bcr.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 27:
                    {
                        if (CDf.It.ReciveResultBcr())
                        {
                            //20191029 ghk_dfserver_notuse_df
                            if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                            {//DF 서버 사용 시 DF 측정 사용
                                CData.Parts[(int)EPart.INR].sBcr    = CData.Bcr.sBcr;
                                CData.Parts[(int)EPart.INR].dDfMin  = CData.dfInfo.dBcrMin;
                                CData.Parts[(int)EPart.INR].dDfMax  = CData.dfInfo.dBcrMax;
                                CData.Parts[(int)EPart.INR].dDfAvg  = CData.dfInfo.dBcrAvg;
                                CData.Parts[(int)EPart.INR].sBcrUse = CData.dfInfo.sBcrUse;
                            }
                            else
                            {//DF 서버 사용 시 DF 측정 사용 안함
                                CData.Parts[(int)EPart.INR].sBcr    = CData.Bcr.sBcr;
                                CData.Parts[(int)EPart.INR].dDfMax  = CData.dfInfo.dBcrMax;
                            }

                            /* // 20200805 syc : ASE KR VOC SECS/GEM (11)
                            if (CData.Dev.bOriSkip) CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                            else CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                            */

                            if (!CData.Opt.bSecsUse) //191125 ksg :
                            {
                                if (CData.Dev.bOriSkip)
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;     //20200805 syc : ASE KR VOC SECS/GEM (11)
                                else
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;  //20200805 syc : ASE KR VOC SECS/GEM (11)

                                _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                                m_iStep = 0;
                                m_iBcrCnt = 0;
                                return true;
                            }
                            else
                            {
                                //20190621 josh    secsgem
                                //999200    carrier verify request
                                CLog.mLogGnd(string.Format("CSQ2_Inr Step - 26 CData.Bcr.sBcr {0} Verify ",CData.Bcr.sBcr));
                                
                                if(CData.GemForm != null)
								{
									CData.GemForm.OnCarrierVerifyRequset(CData.Bcr.sBcr);
                                }

                                // 2022.07.01 SungTae Start : [추가] Log 추가
                                _SetLog($"[SEND](H<-E) S6F11 CEID = {Convert.ToInt32(SECSGEM.JSCK.eCEID.Carrier_Verify_Request)}({SECSGEM.JSCK.eCEID.Carrier_Verify_Request}).  Carrier ID : {CData.Bcr.sBcr}, Set Delay : 180 sec");
                                // 2022.07.01 SungTae End

                                m_Delay.Set_Delay(3 * 60*1000); //20200427 jhc : 60 sec //20200406 jhc : SECSGEM Delay Time 5 m -> 15 sec //m_Delay.Set_Delay(300000);
                                m_iStep++;
                                return false;
                            }
                            //return true;
                        }

                        return false;
                    }

                case 28:
                    {
                        m_TiOut.Set_Delay(TIMEOUT); // SECSGEM 사용시 Host 에서 Reply 시간 3분 이상 으로 시퀸스 Timeout 는 사용 하지 않기 위해 지속 적으로 초기화 진행 LCY 20717

                        // 2021.08.23 SungTae Start : [수정] Remote 상에서 Multi-LOT 기능 디버깅 위해 임시로 주석 처리. 테스트 후 주석 해제 예정.
                        if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripSuccess)/*3*/)
                        {
                            if (CData.Dev.bOriSkip)
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; //20200805 syc : ASE KR VOC SECS/GEM (11)
                            else
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri; //20200805 syc : ASE KR VOC SECS/GEM (11)

                            _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                            m_iStep = 0;
                            m_iBcrCnt = 0;
                            CLog.mLogGnd(string.Format("CSQ2_Inr Step - 27 CData.Bcr.sBcr {0} Verify Finish",CData.Bcr.sBcr));

                            // 2022.07.01 SungTae Start : [추가] Log 추가
                            _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : Verified.");
                            // 2022.07.01 SungTae End
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.ReqError)/*-1*/)
                        {
                            CErr.Show(eErr.HOST_NOT_REQUEST_ERROR);

                            // 2022.07.01 SungTae Start : [추가] Log 추가
                            _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : {SECSGEM.JSCK.eChkStripVerify.ReqError}");
                            // 2022.07.01 SungTae End
                            
                            m_iStep = 0;
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotError)/*-2*/)
                        {
                            CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);

                            // 2022.07.01 SungTae Start : [추가] Log 추가
                            _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : {SECSGEM.JSCK.eChkStripVerify.LotError}");
                            // 2022.07.01 SungTae End

                            m_iStep = 0;
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripError)/*-3*/)
                        {
                            CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);

                            // 2022.07.01 SungTae Start : [추가] Log 추가
                            _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : {SECSGEM.JSCK.eChkStripVerify.StripError}");
                            // 2022.07.01 SungTae End

                            m_iStep = 0;
                            return true;
                        }
                        else if (m_Delay.Chk_Delay())
                        {
                            if(CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.InitVerify)/*1*/)
                            {
                                CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);

                                // 2022.07.01 SungTae Start : [추가] Log 추가
                                _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : {SECSGEM.JSCK.eChkStripVerify.InitVerify}");
                                // 2022.07.01 SungTae End

                                m_iStep = 0;
                                return true;
                            }
                            else if(CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotSuccess)/*2*/)
                            {
                                CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);

                                // 2022.07.01 SungTae Start : [추가] Log 추가
                                _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : {SECSGEM.JSCK.eChkStripVerify.LotSuccess}");
                                // 2022.07.01 SungTae End

                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                CErr.Show(eErr.HOST_STRIP_VERIFY_HOLD_TIME_OVER_ERROR);

                                // 2022.07.01 SungTae Start : [추가] Log 추가
                                _SetLog($"[RECV](H->E) S2F41 RCMD={SECSGEM.GEM.sRCMD_CARRIER_VERIFICATION}. Carrier Verify Status : {CData.SecsGem_Data.nStrip_Check}");
                                // 2022.07.01 SungTae End

                                m_iStep = 0;
                                return true;
                            }
                        }
                        // 2021.08.23 SungTae End

                        return false;
                    }
            }
        }

        /// <summary>
        /// Move Barcode Check Position Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Ori()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_ORIENTATION_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (!Chk_Strip())
            {//항상 자재가 있어야 됨.
                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                _SetLog("Error : Not detect strip.");

                m_iStep = 0;
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
                
                //190617 ksg :
                case 10:
                    {
                        CBcr.It.Save_RunStatus();
                        m_Delay.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        m_iStep++; 
                        return false;
                    }

                case 11: 
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay.");

                        m_iStep++; 
                        return false;
                    }
                    //
                case 12:
                    {//바코드 프로그램 런 체크
                        //20200314 jhc : 2D Vision (UDP) Interface
                        if(CDataOption.BcrInterface == eBcrInterface.Udp)
                        {
                            //2D Vision SW Run/연결 상태 체크
                            if (!CBcr.It.Chk_Run())
                            {
                                CErr.Show(eErr.BCR_2DVISION_CHKRUN_ERROR);
                                _SetLog("Error : 2D Vision check run.");

                                m_iStep = 0;
                                return true;
                            }
                            //2D Vision SW Alarm 체크
                            else if ((CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                            {
                                CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                                _SetLog("Error : 2D Vision alarm.");
                                CBcr.It.b2DVisAlarmNotify = false;
                                CBcr.It.s2DVisAlarmMsg = "";

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else if (!CBcr.It.Chk_Run())
                        {
                            CErr.Show(eErr.INRAIL_BARCODE_NOT_READY);
                            _SetLog("Error : Bcr not ready.");

                            m_iStep = 0;
                            return true;
                        }

                        CData.Bcr.sBcr = "";

                        _SetLog("Bcr ready.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//리프트 다운, 그리퍼 오픈, Y축 얼라인 위치 이동
                        Act_ZLiftDU(false);
                        Func_GripperClamp(false);
                        //CMot.It.Mv_N(m_iY, CData.SPos.dINR_Y_Align);
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//리프트 다운 확인, 그리퍼 오픈 확인, Y축 얼라인 위치 이동 확인, X축 ORI 체크 사전 위치 이동(Normal Speed)
                        if (!Act_ZLiftDU(false))
                        { return false; }
                        if (!Func_GripperClamp(false))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        if (GV.ORIENTATION_ONE_TIME_SKIP && CData.Dev.bOriOneTimeSkipUse && CData.bOriOneTimeSkip)
                        {
                            CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                            CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화

                            CData.Bcr.bRetOri = true;
                            CData.Bcr.bRetMark = true;

                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                            m_iBcrCnt = 0;

                            _SetLog("Orientation One Time Skip.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                            m_iStep = 0;
                            return true;
                        }
                        //

                        //X축 ORI 체크 사전 위치 이동(Normal Speed)
                        m_dInr_X_PreBcrOriPos = Chk_InrX_PreBcrOriPos();
                        CMot.It.Mv_N(m_iX, m_dInr_X_PreBcrOriPos);
                        _SetLog("X axis move position.", m_dInr_X_PreBcrOriPos);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//X축 ORI 체크 사전 위치 이동 확인, X축 ORI 체크 위치 이동(Slow Speed)
                        m_dInr_X_PreBcrOriPos = Chk_InrX_PreBcrOriPos();
                        if (!CMot.It.Get_Mv(m_iX, m_dInr_X_PreBcrOriPos))
                        { 
                            CMot.It.Mv_N(m_iX, m_dInr_X_PreBcrOriPos);
                            return false; 
                        }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Ori); //X축 ORI 체크 위치 이동(Slow Speed)
                        _SetLog("X axis move ori(slow).", CData.Dev.dInr_X_Ori);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//X축 ORI 체크 위치 이동 확인, 그리퍼 클로즈
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Ori))
                        { 
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori));
                            return false; 
                        }

                        Func_GripperClamp(true);
                        _SetLog("Gripper clamp.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//그리퍼 클로즈 확인, X축 (ORI 체크 - 10) 위치 이동(슬로우), 그리퍼 센서 감지 확인
                        if (!Func_GripperClamp(true))
                        { return false; }

                        CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 10));
                        _SetLog("X axsi move position(slow).", (CData.Dev.dInr_X_Ori - 10));

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//X축 (ORI 체크 - 10) 위치 이동 확인, 그리퍼 센서 감지 확인, 그리퍼 오픈
                        if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Ori - 10)))
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 10));
                            return false; 
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//그리퍼 오픈 확인, X축 ORI 체크 위치 이동(슬로우)
                        if (!Func_GripperClamp(false))
                        { return false; }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Ori);
                        _SetLog("X axis move ori(slow).", CData.Dev.dInr_X_Ori);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {// X축 ORI 체크 위치 이동 확인, 그리퍼 센서 감지 확인, X축 ORI 체크 대기 위치 이동(슬로우)
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Ori))
                        { 
                            CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Ori);
                            return false; 
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        m_dInr_X_WaitOriPos = Get_InrX_WaitOriPos();
                        CMot.It.Mv_S(m_iX, m_dInr_X_WaitOriPos);
                        _SetLog("X axis move position.", m_dInr_X_WaitOriPos);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//X축 ORI 체크 대기 위치 이동 확인
                        m_dInr_X_WaitOriPos = Get_InrX_WaitOriPos();
                        if (!CMot.It.Get_Mv(m_iX, m_dInr_X_WaitOriPos))
                        { 
                            CMot.It.Mv_S(m_iX, m_dInr_X_WaitOriPos);
                            return false; 
                        }
                        _SetLog("X axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//Orientation 체크
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Ori();
                        if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Ori();
                        else                                       CBcr.It.Chk_Ori_InR();
                        m_Delay.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if(!m_Delay.Chk_Delay()) return false;
                        //20200314 jhc : 2D Vision SW Alarm 체크
                        if((CDataOption.BcrInterface == eBcrInterface.Udp) && (CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                        {
                            CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                            _SetLog("2D Vision alarm.");
                            CBcr.It.b2DVisAlarmNotify = false;
                            CBcr.It.s2DVisAlarmMsg = "";

                            m_iStep = 0;
                            return true;
                        }
                        //
                        CBcr.It.Load();
                        if(CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;
                        _SetLog("Bcr count : " + m_iBcrCnt);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//Orientation 체크 결과 확인, Orientation 체크 결과 저장 후 상태 변경
                        if (!CData.Bcr.bRetOri)
                        {
                            if(m_iBcrCnt > 3)
                            {
                                CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                _SetLog("Error : Orientation reading fail.");

                                m_iBcrCnt = 0; //190211 ksg :

                                //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                                if (GV.ORIENTATION_ONE_TIME_SKIP && CData.Dev.bOriOneTimeSkipUse)
                                {
                                    CData.bOriOneTimeSkipBtnView = true; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                                    CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화
                                    _SetLog("Orientation One Time Skip Button Display");
                                }

                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                _SetLog("Retry.  Count : " + m_iBcrCnt);

                                m_iStep = 22;
                                return false;
                            }
                        }

                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                        CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화
                        //

                        // 201111 jym START : Skyworks 조건 추가
                        if ((CData.CurCompany == ECompany.SkyWorks) && (!CData.Dev.bBcrSkip) || !CData.Dev.bOcrSkip) // 2021-05-19, jhLee : BCR/OCR을 읽어야 하는지 여부를 OCR 기능 까지 고려
                        { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr; }
                        else
                        { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; }
                        // 201111 jym END

                        m_iBcrCnt = 0;
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Guid Align Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Align()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_ALIGN_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (!Chk_Strip() && m_iStep > 16)//210413 pjh : 조건 추가 > Align-10mm 지점 확인 이후 시점부터 자재 Check
            {//항상 자재가 있어야 됨.
                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                _SetLog("Error : Not detected strip.");

                m_iStep = 0;
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
                    {//리프트 다운, 얼라인 포워드, Y축 algin 위치 이동
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Act_ZLiftDU(false);
                        Act_YAlignBF(true);
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Check axes.  Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//리프트 다운 확인, 얼라인 포워드 확인, Y축 algin 위치 이동 확인, 그리퍼 센서 감지 확인, 감지 안됬을 경우 그리퍼 오픈, 감지 될경우 14번으로 이동
                        if (!Act_ZLiftDU(false))
                        { return false; }

                        //210902 syc : 2004U
                        if (!CDataOption.Use2004U)
                        {
                            if (!Act_YAlignBF(true))
                            { return false; }
                        }
                        //

                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        if (CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            _SetLog("Gripper detect.");

                            m_iStep = 14;
                            return false;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper open.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//그리퍼 오픈 확인, X축 Bcr 위치로 이동(슬로우)
                        if (!Func_GripperClamp(false))
                        { return false; }

                        if (CData.Dev.bDynamicSkip)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Bcr);
                            _SetLog("DF skip.  X axis move bcr.", CData.Dev.dInr_X_Bcr);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_DynamicPos1);
                            _SetLog("X axis move DF1.", CData.Dev.dInr_X_DynamicPos1);
                        }

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//그리퍼 센서 감지 시 X축 정지, X축 Bcr 위치 이동(슬로우) 확인, 그리퍼 센서 감지 안될 경우 에러
                        if (CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CMot.It.Stop(m_iX);
                            _SetLog("Gripper detect.  X axis stop");

                            m_iStep++;
                            return false;
                        }

                        if (CData.Dev.bDynamicSkip)
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                            {
                                CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr);
                                return false;
                            }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_DynamicPos1))
                            {
                                CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_DynamicPos1);
                                return false;
                            }
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Gripper detect.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//그리퍼 클로즈
                        Func_GripperClamp(true);
                        _SetLog("Gripper clamp.");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//그리퍼 클로즈 확인, X축 algin - 10 위치 이동
                        if (!Func_GripperClamp(true))
                        { return false; }

                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //double dAlignOffset = (CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ)|| CData.CurCompany == ECompany.SST) ? 4 : 10;
                        double dAlignOffset = ( CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                                CData.CurCompany == ECompany.SST) ? 4 : 10;

                        CMot.It.Mv_N(m_iX, (CData.Dev.dInr_X_Align - dAlignOffset));
                        _SetLog("X axis move position.", CData.Dev.dInr_X_Align - dAlignOffset);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//X축 algin - 10 위치 이동 확인, 그리퍼 오픈
                        // 2021.03.29 SungTae: Qorvo_RT & NC 조건 추가
                        //double dAlignOffset = (CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ)|| CData.CurCompany == ECompany.SST) ? 4 : 10;
                        double dAlignOffset = ( CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                                CData.CurCompany == ECompany.SST) ? 4 : 10;

                        if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Align - dAlignOffset)))
                        {
                            CMot.It.Mv_N(m_iX, (CData.Dev.dInr_X_Align - dAlignOffset));
                            return false;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//그리퍼 오픈, Y축 align 위치 이동
                        if (!Func_GripperClamp(false))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Y축 align 위치 이동 확인, X축 algin 위치 이동(슬로우)
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Align);
                        _SetLog("X axis move align(slow).", CData.Dev.dInr_X_Align);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//X축 algin 위치 이동(슬로우) 확인, 그리퍼 센서 감지 확인, X축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Align))
                        { return false; }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        CMot.It.Mv_N(m_iX, CData.SPos.dINR_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dINR_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//X축 대기 위치 이동 확인, 상태 변경
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dINR_X_Wait))
                        { return false; }

                        //210927 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            if (!CIO.It.Get_X(eX.INR_Carrier_Detect))
                            {
                                CErr.Show(eErr.INRAIL_FEEDING_ERROR);
                                _SetLog("Error : INR_Carrier_Detect Sensor Detecte.");

                                m_iStep = 0;
                                return true;
                            }

                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckIV2;
                           // CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_IV2;

                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);
                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.ONP].iStat);

                            m_iStep = 0;
                            return true;
                        }
                        else // 기존 시퀀스 사용
                        {
                            //CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker; //190821 ksg :
                            //if(CDataOption.CurEqu == eEquType.Pikcer && CSQ_Main.It.m_iStat == eStatus.Auto_Running)
                            if (CDataOption.CurEqu == eEquType.Pikcer)
                            {
                                // 2021-05-19, jhLee : BCR/OCR을 읽어야 하는지 여부를 OCR 기능 까지 고려

                                if ((!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) && !CData.Dev.bOriSkip)            // BCR/OCR, Orientation 수행
                                {
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcrOri;
                                }
                                else if ((!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) && CData.Dev.bOriSkip)        // Orientation은 생략
                                {
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr;
                                }
                                else if ((CData.Dev.bBcrSkip && CData.Dev.bOcrSkip) && !CData.Dev.bOriSkip)          // BCR/OCR은 Skip, Only Orientation check
                                {
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;
                                }
                            }
                            else
                            {
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;
                            }

                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                            m_iStep = 0;
                            return true;
                        }
                       // syc end
                    }
            }
        }

        /// <summary>
        /// Guid Align Cycle (Unit)
        /// </summary>
        /// <returns></returns>
        public bool Cyl_AlignU()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_ALIGN_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            //항상 자재가 있어야 됨.
            if (!Chk_Strip())
            {
                CData.Parts[m_iINR].iStat = ESeq.INR_Ready;
                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                _SetLog("Error : Not detected unit.");

                m_iStep = 0;
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

                case 10: // Lift down, Align forward
                    {
                        m_tStTime = DateTime.Now;

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Act_ZLiftDU(false);
                        Act_YAlignBF(true);
                        _SetLog("Check axes.  Lift down.  Align forward.");

                        m_iStep++;
                        return false;
                    }

                case 11: // Lift down, Align forward check, Y axis move align position 
                    {   
                        if (!Act_ZLiftDU(false))
                        { return false; }

                        //210902 syc : 2004U
                        if (!CDataOption.Use2004U)
                        {
                            if (!Act_YAlignBF(true))
                            { return false; }
                        }
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_X_Align);

                        m_iStep++;
                        return false;
                    }

                case 12: // Y axis move align position check 
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        _SetLog("Y axis move check");

                        m_iStep++;
                        return false;
                    }

                case 13: // Gripper detect check, Gripper clamp open
                    {
                        if (CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            _SetLog("Gripper carrier detect");

                            // Gripper 감지 시 이미 자재 존재 상태로 판단
                            m_iStep = 20;
                            return false;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper clamp open");

                        m_iStep++;
                        return false;
                    }

                case 14: // Gripper clamp open check, Gripper detect
                    {
                        if (!Func_GripperClamp(false))
                        { return false; }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {                            
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Gripper detect");

                        m_iStep = 20;
                        return false;
                    }

                case 20: // Gripper close
                    {
                        Func_GripperClamp(true);

                        _SetLog("Gripper close");

                        m_iStep++;
                        return false;
                    }

                case 21: // Gripper close check, X Axis move align position
                    {
                        if (!Func_GripperClamp(true))
                        { return false; }
                       
                        CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Align - 10);
                        _SetLog("X axis move align.", CData.Dev.dInr_X_Align - 10);

                        m_iStep++;
                        return false;
                    }

                case 22: // X Axis move align position check, Gripper open
                    {
                        if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Align - 10)))
                        {
                            CMot.It.Mv_N(m_iX, (CData.Dev.dInr_X_Align - 10));
                            return false;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper Open");

                        m_iStep++;
                        return false;
                    }

                case 23: // Gripper open check, Y axis move align position
                    {
                        if (!Func_GripperClamp(false))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 24: // Y axis move align position check, X axis move align position
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Align);
                        _SetLog("X axis move align(slow).", CData.Dev.dInr_X_Align);

                        m_iStep++;
                        return false;
                    }

                case 25: // X axis move align position check, Gripper detect check
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Align))
                        { return false; }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {                            
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Gripper detect check");

                        m_iStep++;
                        return false;
                    }

                case 26: // X axis move wait position
                    {
                        CMot.It.Mv_N(m_iX, CData.SPos.dINR_X_Wait);
                        _SetLog("X axis move wait(slow).", CData.SPos.dINR_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 27: // X axis move wait position check, Unit exist check
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dINR_X_Wait))
                        { return false; }

                        // 2021.05.14 SungTae Start : 2003U 개조건 관련 수정, 기존 SG2001U인 경우에 이곳에서 센서로 자재유무 체크하고, 
                        // SG2003U인 경우 On-Loader에서 자재 체크를 해서 넘어온다.
                        if (CDataOption.iEquipClass == (int)eEqSeries.U_2001)           /// 개조전 2001U설비인 경우
                        {
                            bool bX1 = CIO.It.Get_X(eX.INR_Unit_1_Detect);
                            bool bX2 = CIO.It.Get_X(eX.INR_Unit_2_Detect);
                            bool bX3 = CIO.It.Get_X(eX.INR_Unit_3_Detect);
                            bool bX4 = CIO.It.Get_X(eX.INR_Unit_4_Detect);
                            _SetLog(string.Format("Sensor check.  #1:{0}  #2:{1}  #3:{2}  #4:{3}", bX1, bX2, bX3, bX4));

                            // 200707 jym : 유닛 2개 일 때 판정 추가
                            if (CData.Dev.iUnitCnt == 4)
                            {
                                CData.Parts[m_iINR].aUnitEx[0] = bX1;
                                CData.Parts[m_iINR].aUnitEx[1] = bX2;
                                CData.Parts[m_iINR].aUnitEx[2] = bX3;
                                CData.Parts[m_iINR].aUnitEx[3] = bX4;
                                _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}  #3:{2}  #4:{3}", bX1, bX2, bX3, bX4));                               
                            }
                            else
                            {
                                CData.Parts[m_iINR].aUnitEx[0] = bX1 && bX2;
                                CData.Parts[m_iINR].aUnitEx[1] = bX3 && bX4;
                                _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}", CData.Parts[m_iINR].aUnitEx[0], CData.Parts[m_iINR].aUnitEx[1]));
                            }
                        }
                        else
                        {
                            _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}  #3:{2}  #4:{3}",  CData.Parts[m_iINR].aUnitEx[0],
                                                                                                        CData.Parts[m_iINR].aUnitEx[1],
                                                                                                        CData.Parts[m_iINR].aUnitEx[2],
                                                                                                        CData.Parts[m_iINR].aUnitEx[3]) );
                        }
                        // 2021.05.14 SungTae End

                        m_iStep++;
                        return false;
                    }

                case 28: // Finish, Status change
                    {
                        if (CDataOption.CurEqu == eEquType.Pikcer)
                        {
                            // 2021-05-19, jhLee : BCR/OCR을 읽어야 하는지 여부를 OCR 기능 까지 고려

                            if ((!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) && !CData.Dev.bOriSkip)                 // BCR/OCR, Orientation 수행
                            {
                                CData.Parts[m_iINR].iStat = ESeq.INR_CheckBcrOri;
                            }
                            else if ((!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) && CData.Dev.bOriSkip)            // Orientation은 생략
                            {
                                CData.Parts[m_iINR].iStat = ESeq.INR_CheckBcr;
                            }
                            else if ((CData.Dev.bBcrSkip && CData.Dev.bOcrSkip) && !CData.Dev.bOriSkip)             // BCR/OCR은 Skip, Only Orientation check
                            {
                                CData.Parts[m_iINR].iStat = ESeq.INR_CheckOri;
                            }
                            else
                            {
                                CData.Parts[m_iINR].iStat = ESeq.INR_WaitPicker;
                            }
                        }
                        else
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;
                        }

                        _SetLog("Status change.  Status:" + CData.Parts[(int)EPart.INR].iStat.ToString());

                        m_iStep++;
                        return false;
                    }

                case 29: // 종료, 택 타임 계산
                    {
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog(string.Format("Finish.  Time:{0}", m_tTackTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20190604 ghk_onpbcr/// <summary>
        /// Move Barcode Check Position Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_BcrReady()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_BARCODE_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (!Chk_Strip())
            {//항상 자재가 있어야 됨.
                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                _SetLog("Error : Not detect strip.");

                m_iStep = 0;
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
                    {//리프트 다운, 그리퍼 오픈, Y축 얼라인 위치 이동
                        Act_ZLiftDU(false);
                        Func_GripperClamp(false);
                        //CMot.It.Mv_N(m_iY, CData.SPos.dINR_Y_Align);
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//리프트 다운 확인, 그리퍼 오픈 확인, Y축 얼라인 위치 이동 확인, X축 BcrCheck 위치 이동
                        if (!Act_ZLiftDU(false))
                        { return false; }
                        if (!Func_GripperClamp(false))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dInr_X_Bcr);
                        _SetLog("X axis move bcr.", CData.Dev.dInr_X_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//X축 BcrCheck 위치 이동 확인, 그리퍼 클로즈
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                        {
                            CMot.It.Mv_N(m_iX, (CData.Dev.dInr_X_Bcr));
                            return false;
                        }

                        Func_GripperClamp(true);
                        _SetLog("Gripper clamp.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//그리퍼 클로즈 확인, X축 BcrCheck - 10 위치 이동(슬로우), 그리퍼 센서 감지 확인
                        if (!Func_GripperClamp(true))
                        { return false; }

                        CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Bcr - 10));
                        _SetLog("X axis move position(slow).", CData.Dev.dInr_X_Bcr - 10);

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//X축 BcrCheck - 10 위치 이동(슬로우) 확인, 그리퍼 센서 감지 확인, 그리퍼 오픈
                        if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Bcr - 10)))                        
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Bcr - 10));
                            return false;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//그리퍼 오픈 확인, X축 BcrCheck 위치 이동(슬로우)
                        if (!Func_GripperClamp(false))
                        { return false; }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr);
                        _SetLog("X axsi move bcr(slow).", CData.Dev.dInr_X_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {// X축 BcrCheck 위치 이동(슬로우)확인, 그리퍼 센서 감지 확인, X축 BcrCheck - 10 위치 이동(슬로우)
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Bcr);
                            return false;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany != ECompany.Qorvo && (CData.CurCompany != ECompany.Qorvo_DZ) && CData.CurCompany != ECompany.SST)
                        if (CData.CurCompany != ECompany.Qorvo && CData.CurCompany != ECompany.Qorvo_DZ &&
                            CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&
                            CData.CurCompany != ECompany.SST)
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Bcr - 10));
                            _SetLog("X axis move position(slow).", (CData.Dev.dInr_X_Bcr - 10));
                        }//191202 ksg :
                        else
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Bcr - 40));
                            _SetLog("X axis move position(slow).", (CData.Dev.dInr_X_Bcr - 40));
                        }

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//X축 BcrCheck - 10 위치 이동(슬로우) 확인
                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany != ECompany.Qorvo && (CData.CurCompany != ECompany.Qorvo_DZ) && CData.CurCompany != ECompany.SST)
                        if (CData.CurCompany != ECompany.Qorvo && CData.CurCompany != ECompany.Qorvo_DZ &&
                            CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&
                            CData.CurCompany != ECompany.SST)
                        {
                            if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Bcr - 10)))
                            {
                                CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Bcr - 10));
                                return false;
                            }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Bcr - 40)))
                            {
                                CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Bcr - 40));
                                return false;
                            }
                        }
                        
                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr;
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                        m_iStep = 0;
                        return false;
                    }
            }
        }

        /// <summary>
        /// Move Barcode Check Position Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_OriReady()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.INRAIL_ORIENTATION_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

           if (!Chk_Strip())
            {//항상 자재가 있어야 됨.
                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                _SetLog("Error : Not detected strip.");

                m_iStep = 0;
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
                    {//리프트 다운, 그리퍼 오픈, Y축 얼라인 위치 이동
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Act_ZLiftDU(false);
                        Func_GripperClamp(false);
                        CMot.It.Mv_N(m_iY, CData.Dev.dInr_Y_Align);
                        _SetLog("Y axis move align.", CData.Dev.dInr_Y_Align);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//리프트 다운 확인, 그리퍼 오픈 확인, Y축 얼라인 위치 이동 확인, X축 BcrCheck 위치 이동
                        if (!Act_ZLiftDU(false))
                        { return false; }
                        if (!Func_GripperClamp(false))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dInr_Y_Align))
                        { return false; }

                        CMot.It.Mv_N(m_iX, (CData.Dev.dInr_X_Bcr));
                        _SetLog("X axis move bcr.", CData.Dev.dInr_X_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//X축 BcrCheck 위치 이동 확인, 그리퍼 클로즈
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Bcr))
                        {
                            CMot.It.Mv_N(m_iX, (CData.Dev.dInr_X_Bcr));
                            return false;
                        }

                        Func_GripperClamp(true);
                        _SetLog("Gripper clamp.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//그리퍼 클로즈 확인, X축 OriCheck - 10 위치 이동(슬로우), 그리퍼 센서 감지 확인
                        if (!Func_GripperClamp(true))
                        { return false; }

                        CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 10));
                        _SetLog("X axis move position(slow).", CData.Dev.dInr_X_Ori - 10);

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//X축 OriCheck - 10 위치 이동(슬로우) 확인, 그리퍼 센서 감지 확인, 그리퍼 오픈
                        if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Ori - 10)))
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 10));
                            return false;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_GripperClamp(false);
                        _SetLog("Gripper unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//그리퍼 오픈 확인, X축 OriCheck 위치 이동(슬로우)
                        if (!Func_GripperClamp(false))
                        { return false; }

                        CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Ori);
                        _SetLog("X axis move ori(slow).", CData.Dev.dInr_X_Ori);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {// X축 OriCheck 위치 이동(슬로우)확인, 그리퍼 센서 감지 확인, X축 OriCheck - 10 위치 이동(슬로우)
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Ori))
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dInr_X_Ori);
                            return false;
                        }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            CErr.Show(eErr.INRAIL_GRIPPER_NO_DETECT);
                            _SetLog("Error : Gripper not detect.");

                            m_iStep = 0;
                            return true;
                        }

                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany != ECompany.Qorvo && (CData.CurCompany != ECompany.Qorvo_DZ) && CData.CurCompany != ECompany.SST)
                        if (CData.CurCompany != ECompany.Qorvo && CData.CurCompany != ECompany.Qorvo_DZ &&
                            CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&
                            CData.CurCompany != ECompany.SST)
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 10));
                            _SetLog("X axis move position(slow).", (CData.Dev.dInr_X_Ori - 10));
                        }
                        else
                        {
                            CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 40));
                            _SetLog("X axis move position(slow).", (CData.Dev.dInr_X_Ori - 40));
                        }

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//X축 OriCheck - 10 위치 이동(슬로우) 확인, 바코드 읽기
                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany != ECompany.Qorvo && (CData.CurCompany != ECompany.Qorvo_DZ) && CData.CurCompany != ECompany.SST)
                        if (CData.CurCompany != ECompany.Qorvo && CData.CurCompany != ECompany.Qorvo_DZ &&
                            CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&
                            CData.CurCompany != ECompany.SST)
                        {
                            if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Ori - 10)))
                            {
                                CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 10));
                                return false;
                            }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, (CData.Dev.dInr_X_Ori - 40)))
                            {
                                CMot.It.Mv_S(m_iX, (CData.Dev.dInr_X_Ori - 40));
                                return false;
                            }
                        }

                        m_iBcrCnt = 0;
                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        #region Actuator
        /// <summary>
        /// 인레일 X축 그리퍼 true = Close, false = Open
        /// </summary>
        /// <param name="bClamp"></param>
        /// <returns></returns>
        public bool Func_GripperClamp(bool bClamp)
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.INR_GripperClampOn, bClamp);//Y15 InRail Gripper

            //210902 syc : 2004U
            //if (CDataOption.Package == ePkg.Unit)// Hisilicon 일 경우 20200228 LCY
            if (CDataOption.Package == ePkg.Unit || CDataOption.Use2004U)            
            {
                if (bClamp)
                {
                    bRet = ((CIO.It.Get_X(eX.INR_GripperClampOn) && !CIO.It.Get_X(eX.INR_GripperClampOff)) &&
                            (CIO.It.Get_X(eX.INR_GripperClampOn_Rear) && !CIO.It.Get_X(eX.INR_GripperClampOff_Rear)));
                }
                else
                {
                    bRet = ((!CIO.It.Get_X(eX.INR_GripperClampOn)      && CIO.It.Get_X(eX.INR_GripperClampOff)) &&
                            (!CIO.It.Get_X(eX.INR_GripperClampOn_Rear) && CIO.It.Get_X(eX.INR_GripperClampOff_Rear))); 
                }
            }
            else {
                if (bClamp)
                {
                    bRet = (CIO.It.Get_X(eX.INR_GripperClampOn) && !CIO.It.Get_X(eX.INR_GripperClampOff));
                }
                else
                {
                    bRet = (!CIO.It.Get_X(eX.INR_GripperClampOn) && CIO.It.Get_X(eX.INR_GripperClampOff));
                }
            }
            return bRet;
        }

        public bool Func_GripperClamp()
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.INR_GripperClampOn, true);

            //210910 syc : 2004U
            //if (CDataOption.Package == ePkg.Unit)
            if (CDataOption.Package == ePkg.Unit || CDataOption.Use2004U)
            {
                bRet = ((CIO.It.Get_X(eX.INR_GripperClampOn) && !CIO.It.Get_X(eX.INR_GripperClampOff)) &&
                        (CIO.It.Get_X(eX.INR_GripperClampOn_Rear) && !CIO.It.Get_X(eX.INR_GripperClampOff_Rear)));
            }
            else
            {
                bRet = (CIO.It.Get_X(eX.INR_GripperClampOn) && !CIO.It.Get_X(eX.INR_GripperClampOff));
            }

            return bRet;
        }

        public bool Func_GripperUnclamp()
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.INR_GripperClampOn, false);

            if (CDataOption.Package == ePkg.Unit)
            {
                bRet = ((!CIO.It.Get_X(eX.INR_GripperClampOn) && CIO.It.Get_X(eX.INR_GripperClampOff)) &&
                        (!CIO.It.Get_X(eX.INR_GripperClampOn_Rear) && CIO.It.Get_X(eX.INR_GripperClampOff_Rear)));
            }
            else
            {
                bRet = (!CIO.It.Get_X(eX.INR_GripperClampOn) && CIO.It.Get_X(eX.INR_GripperClampOff));
            }

            return bRet;
        }

        /// <summary>
        /// 인레일 가이드 얼라인 true = Forward, false = Backward
        /// </summary>
        /// <param name="Fwd"></param>
        /// <returns></returns>
        public bool Act_YAlignBF(bool Fwd)
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_GuideForward, Fwd);//Y12 InRail Align

            //X12 InRail Align Forward, X13 InRail Align BackWard
            if (Fwd)
            {
                bRet = (CIO.It.Get_X(eX.INR_GuideForward) && !CIO.It.Get_X(eX.INR_GuideBackward));
            }
            else
            {
                bRet = (!CIO.It.Get_X(eX.INR_GuideForward) && CIO.It.Get_X(eX.INR_GuideBackward));
            }

            return bRet;
        }

        public bool Func_AlignForward()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_GuideForward, true);
            bRet = CIO.It.Get_X(eX.INR_GuideForward) && (!CIO.It.Get_X(eX.INR_GuideBackward));

            return bRet;
        }

        public bool Func_AlignBackward()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_GuideForward, false);
            bRet = (!CIO.It.Get_X(eX.INR_GuideForward)) && CIO.It.Get_X(eX.INR_GuideBackward);

            return bRet;
        }

        /// <summary>
        /// 인레일 리프트 업, true = 업, false = 다운
        /// </summary>
        /// <param name="Up"></param>
        /// <returns></returns>
        public bool Act_ZLiftDU(bool Up)
        {
            bool bRet;

            CIO.It.Set_Y(eY.INR_LiftUp, Up);//Y18 InRail Lift

            //X18 InRail Lift Up, X19 InRail Lift Down
            if (Up)
            {
                bRet = (CIO.It.Get_X(eX.INR_LiftUp) && !CIO.It.Get_X(eX.INR_LiftDn));
            }
            else
            {
                bRet = (!CIO.It.Get_X(eX.INR_LiftUp) && CIO.It.Get_X(eX.INR_LiftDn));
            }

            return bRet;
        }

        public bool Func_LiftUp()
        {
            bool bRet;

            CIO.It.Set_Y(eY.INR_LiftUp, true);
            bRet = (CIO.It.Get_X(eX.INR_LiftUp) && !CIO.It.Get_X(eX.INR_LiftDn));

            return bRet;
        }

        public bool Func_LiftDown()
        {
            bool bRet;

            CIO.It.Set_Y(eY.INR_LiftUp, false);
            bRet = (!CIO.It.Get_X(eX.INR_LiftUp) && CIO.It.Get_X(eX.INR_LiftDn));

            return bRet;
        }

        //20190221 ghk_dynamicfunction
        /// <summary>
        /// 인레일 프로브 업 다운
        /// true : down, false : up
        /// </summary>
        /// <param name="bUpDn"></param>
        /// <returns></returns>
        public bool Act_ProbeUD(bool bUpDn)
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.INR_ProbeDn, bUpDn);    //Y1E Probe UP/Down

            if (bUpDn)
            { bRet = (CIO.It.Get_X(eX.INR_ProbeDn) && !CIO.It.Get_X(eX.INR_ProbeUp)); }
            else
            { bRet = (!CIO.It.Get_X(eX.INR_ProbeDn) && CIO.It.Get_X(eX.INR_ProbeUp)); }

            return bRet;
        }

        public bool Func_PrbUp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_ProbeDn, false);
            bRet = (!CIO.It.Get_X(eX.INR_ProbeDn) && CIO.It.Get_X(eX.INR_ProbeUp));

            return bRet;
        }

        public bool Func_PrbDown()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_ProbeDn, true);
            bRet = (CIO.It.Get_X(eX.INR_ProbeDn) && !CIO.It.Get_X(eX.INR_ProbeUp));

            return bRet;
        }

        #endregion

        public bool Chk_Axes(bool bHD = true)
        {
            int iRet = 0;

            iRet = CMot.It.Chk_Rdy(m_iX, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.INRAIL_X_NOT_READY);
                _SetLog("Error : X axis not ready.");

                return true;
            }

            iRet = CMot.It.Chk_Rdy(m_iY, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.INRAIL_Y_NOT_READY);
                _SetLog("Error : Y axis not ready.");

                return true;
            }

            return false;
        }
        //190714 ksg : Lot End시 Inrail X축 Wait 이동
        public void InrXAxisWait()
        {
            CMot.It.Mv_N(m_iX, CData.SPos.dINR_X_Wait);
            if(CSQ_Main.It.Chk_EmptyMGZ())
            {//210113 pjh : Empty Magazine Lot End Check
                m_LotEndChk = true; 
            }
            
        }

        // 201104 jym : Chk_Strip, Chk_Unit 함수를 하나로 합침
        /// <summary>
        /// 인레일 위에 자재 있는지 확인
        /// </summary>
        /// <returns>true = 자재 있음, false = 자재 없음</returns>
        public bool Chk_Strip()
        {
            bool bRet = false;

            if (!CMot.It.Chk_HD((int)EAx.Inrail_X))
            {//Inrail X축 Home Done 아닐 경우
                bRet = !CIO.It.Get_X(eX.INR_StripInDetect);
            }
            else
            {//Inrail X축 Home Done 일 경우
                if (CMot.It.Get_Mv(m_iX, CData.Dev.dInr_X_Pick))
                {//Inrail X축 Strip Pick 위치 일 경우
                    bRet = !CIO.It.Get_X(eX.INR_StripInDetect);
                }
                else
                {//Inrail X축 Strip Pick 위치 아닐 경우
                    if (CDataOption.Package == ePkg.Strip)
                    {
                        if (!CIO.It.Get_X(eX.INR_StripInDetect) || CIO.It.Get_X(eX.INR_StripBtmDetect))
                        {//스트립 인 센서, 스트립 바텀 센서 둘 중 하나라도 true 일 경우 자재 있음
                            bRet = true;
                        }
                        else
                        {//스트립 인 센서, 스트립 바텀 센서 둘 다 false 일경우 자재 없음
                            bRet = false;
                        }
                    }
                    else
                    {
                        if (CIO.It.Get_X(eX.INR_Unit_1_Detect) ||
                            CIO.It.Get_X(eX.INR_Unit_2_Detect) ||
                            CIO.It.Get_X(eX.INR_Unit_3_Detect) ||
                            CIO.It.Get_X(eX.INR_Unit_4_Detect))
                        {// Rail 위에 자재 (Unit 1 ~ 4 중 1개 감지) 
                            bRet = true;
                        }
                        else
                        {//스트립 인 센서, 스트립 바텀 센서 둘 다 false 일경우 자재 없음
                            bRet = false;
                        }
                    }
                }
            }
            return bRet;
        }

        public bool Chk_StinpIn(bool bDetect)
        {
            if (GV.IDLE_MODE)
            { return true; }

            return CIO.It.Get_X(eX.INR_StripInDetect) == !bDetect;
        }
    }
}
