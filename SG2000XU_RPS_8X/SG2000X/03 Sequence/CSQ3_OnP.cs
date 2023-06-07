using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms; //210907 syc : 2004U

namespace SG2000X
{
    public class CSq_OnP : CStn<CSq_OnP>
    {
        private readonly int TIMEOUT = 30000;
        //private readonly int TIMEOUT = 60000; //190827 ksg :

        private readonly int SECSGEM_BCR_TIMEOUT = 90000; //20200427 jhc : SECG/GEM 사용 시 BCR/ORI Timeout 시간 연장

        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep = 0;
        private int m_iPreStep = 0;

        private int m_iX    = (int)EAx.OnLoaderPicker_X;
        private int m_iZ    = (int)EAx.OnLoaderPicker_Z;
        //20190604 ghk_onpbcr
        private int m_iY    = (int)EAx.OnLoaderPicker_Y;

        private int m_iLTY  = (int)EAx.LeftGrindZone_Y  ;
        // 2020.11.24 JSKim St
        //private int m_iRTY  = (int)EAx.LeftGrindZone_Y  ;
        private int m_iRTY  = (int)EAx.RightGrindZone_Y;
        // 2020.11.24 JSKim Ed

        private int m_iInRX = (int)EAx.Inrail_X         ; //190716 ksg : 조양 요청
        private int m_iOfpX = (int)EAx.OffLoaderPicker_X; //190717 ksg : 조양 요청

        //190822 pjh_onp_conversion
        private int m_iLGZ = (int)EAx.LeftGrindZone_Z;
        private int m_iRGZ = (int)EAx.RightGrindZone_Z;
        /// <summary>
        /// Picker eject 동작 시 설정 딜레이 [ms]
        /// </summary>
        private int m_iEjcDelay = 200;
        /// <summary>
        /// Picker pick 동작에서 에러 발생 시 설정 딜레이 [ms]
        /// </summary>
        private int m_iErrDelay = 1000;

        //200407 jym : 임시로 저장되는 딜레이, 갭 변수 추가
        /// <summary>
        /// 임시로 사용되는 갭 값 [mm]
        /// </summary>
        private double m_dTempGap = 0;
        /// <summary>
        /// 임시로 사용되는 포지션 값 [mm]
        /// </summary>
        private double m_dPos = 0;
        private bool m_bReqStop = false;
        private bool m_bduringLotEnd = false;

        private CTim m_Delay  = new CTim();
        private CTim m_Delay_01  = new CTim();
        private CTim m_TiOut  = new CTim();
        private CTim m_Place  = new CTim();
        private CTim m_Delay1 = new CTim();

        //190807 ksg : TackTime 
        private DateTime m_StartSeq;
        private DateTime m_EndSeq  ;
        private TimeSpan m_SeqTime ;

        private DateTime m_tStTime;
        private TimeSpan m_tTackTime ;

        //20190604 ghk_onpbcr
        private int m_iBcrCnt = 0;
        public ESeq m_iPreState = 0;

        public ESeq iSeq;
        public ESeq SEQ = ESeq.Idle;
        //public tLog m_LogVal = new tLog(); //190218 ksg :

        //20190611 ghk_onpclean
        /// <summary>
        /// 현재 클리닝 진행 카운트
        /// </summary>
        private int m_iCleanCnt = 0;
        /// <summary>
        /// 피커 클리닝 이후 변경될 시퀀스
        /// </summary>
        private ESeq m_PrESeq = ESeq.ONP_Wait;
        /// <summary>
        /// 피커 클리너 방향 확인 false = Left, true = Right
        /// </summary>
        private bool m_bDir = false;

        //20190625 ghk_dfserver
        public bool m_bBcrKeyIn = false;
        public bool m_bBcrErr   = false;
        public bool m_bBcrView  = false;
        //190820 ksg : 
        public bool m_bSecondBcr = false;
        public bool m_bPickOK = false;

        public bool m_bRetryPickTbL = false;    // 2022.08.18 lhs : 2004U, Cyl_PickTable 재진입하기 위한 변수

        // Error 발생 시에 Z-Axis Up 하기 위한 변수 20200301 LCY
        private bool bError_Safety_Mv = false;

        // 2021.05.31. SungTae Start :
        public TPart m_tTarget = new TPart();
        // 2021.05.31. SungTae End
        
        //210824 syc : 2004U 
        /// <summary>
        /// IV2  여러 상황 반복 횟수 확인 목적
        /// </summary>
        public int iIV2countCheck = 0;
        /// <summary>
        /// iv2 나누어 검사할때 몇번째 파라미터인지 확인 목적
        /// 1 = 첫번째 파라미터 사용
        /// 2 = 두번째 파라미터 사용 ...
        /// </summary>
        public int iONPPara = 1;
        /// <summary>
        /// 임시 X축 위치 변수
        /// </summary>
        public double dIV2Xpos = 0;
        /// <summary>
        /// 임시 Y축 위치 변수
        /// </summary>
        public double dIV2Ypos = 0;
        /// <summary>
        /// 임시 Z축 위치 변수
        /// </summary>
        public double dIV2Zpos = 0;
        /// <summary>
        /// 임시 IV2 파라미터 변수
        /// </summary>
        public string SIV2Para = "000";
        /// <summary>
        /// ///210830 syc : 2004U IV2 -  IV2 연결
        /// ONP IV2
        /// </summary>
        private CIV2Ctrl pCtrlIV2 = new CIV2Ctrl(0);

        //
        ///210830 syc : 2004U IV2 -  IV2 연결
        public void initIV2Ctrl(string sIP, int nPort)
        {
            pCtrlIV2.SetAddress(sIP, nPort);
        }
        ///210830 syc : 2004U IV2 -  IV2 연결
        public void ConnectIV2()
        {
            pCtrlIV2.Connect();
        }
        ///210830 syc : 2004U IV2 -  IV2 연결
        public void CloseIV2()
        {
            pCtrlIV2.Close();
        }
        ///210830 syc : 2004U IV2 -  IV2 연결
        public void TriggerIV2()
        {
            pCtrlIV2.SendTrigger();
        }
        //210907 syc : 2004U
        public void SendMsg(string sMsg)
        {
            pCtrlIV2.SendString(sMsg);
        }
        public void ShowLog(ListBox pLi)
        {
            pCtrlIV2.LogList(pLi);
        }
        public void SendPrg(string sPrg)
        {
            int iPrg = 0;
            iPrg = int.Parse(sPrg);
            pCtrlIV2.SendProgramWrite(iPrg);
        }
        public void ReadPrg()
        {
            pCtrlIV2.SendProgramRead();
        }
        public void IV2CheckRun()
        {
            pCtrlIV2.SendRM();
        }
        public void IVCheckSensorState()
        {
            pCtrlIV2.SendSR();
        }
        public void IVErrorCheck()
        {
            pCtrlIV2.SendERRCheck();
        }
        // end

        private CSq_OnP()
        {
            m_bHD = false;
        }

        public bool ToReady()
        {
            m_bReqStop = false;
            SEQ        = ESeq.Idle;
            return true;
        }

        public bool ToStop()
        {
            m_bReqStop = true;
            if(m_iStep != 0) return false;
#if false //삭제 //20200407 jhc : STOP 버튼 눌렀을 때 OnPicker X축 대기 위치로
            if(CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
            {//Z축 대기 위치에 있을 경우에만 -> OnPicker X축 Wait Position으로 이동
                CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
            }
#endif            
            return true;
        }

        public bool Stop()
        {
            m_iStep = 0;
            return true;
        }

        #region AutoRun
        public bool AutoRun()
        {
            iSeq = CData.Parts[(int)EPart.ONP].iStat;
            
            if (CDataOption.CurEqu == eEquType.Nomal)
            {
                if (SEQ == (int)ESeq.Idle)
                {
                    if (m_bReqStop)
                    { return false; }

                    if (CSQ_Main.It.m_iStat == EStatus.Stop)
                    { return false; }

                    // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                    // syc : ase kr loading stop - 피커 노말 타입 Loading stop시 멈추는 구문
                    if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/) //1=table
                    {
                        if (CData.Dev.bDual == eDual.Dual) //듀얼 모드일 경우
                        {    //Loading Stop On 활성화시 현재 상태가 'Place Left Table' 이거나 'Wait Place' 일 경우 return 
                            if (CSQ_Main.It.m_bPause && ((CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbL) || (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WaitPlace)))
                            { return false; }
                        }
                        else //노말 모드일 경우
                        {   //Loading Stop On 활성화시 현재 상태가 'Place Left/Right Table' 이거나 'Wait Place' 일 경우 return
                            if (CSQ_Main.It.m_bPause && ((CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbR) || (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbL) || (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WaitPlace)))
                            { return false; }
                        }
                    }
                    //syc end

                    //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                    bool bWait     = false;
                    bool bPickRail = false;
                    bool bPickTb   = false;
                    bool bPlaceTbL = false;
                    bool bPlaceTbR = false;
               		bool bClean = false;

                    if(iSeq == ESeq.ONP_WorkEnd)
                    {
                        return true; 
                    }

                    if (iSeq == ESeq.ONP_Wait || iSeq == ESeq.Idle)
					{
						bWait = true;
					}
					if (iSeq == ESeq.ONP_Pick && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WaitPicker)
					{
						// 200318 mjy : 조건추가
						if (CDataOption.Package == ePkg.Strip)
                        {
                            if (!CIO.It.Get_X(eX.INR_StripBtmDetect))
                            {
                                //CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                                m_iStep = 0;
                                return true;
                            }
                        }

                        bPickRail = true;
                    }
                    if (iSeq == ESeq.ONP_Pick && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd)
                    {
                        m_bduringLotEnd = true;
                        //CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_WorkEnd;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait;
                        return true;
                    }
                    if (iSeq == ESeq.ONP_PickTbL && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                    { bPickTb = true; }     // eEquType.Nomal

                    if (iSeq == ESeq.ONP_WaitPlace || iSeq == ESeq.ONP_PlaceTbL || iSeq == ESeq.ONP_PlaceTbR)
                    {
                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            //20200428 jhc : 자재 있으나 버큠 체크 안 될 경우 에러 처리 => 이 때, 픽업된 자재는 제거해야 함
                            if (CData.Parts[(int)EPart.ONP].bExistStrip == true)
                            { 
                                CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                            }
                            
                            CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Pick;
                            m_iStep = 0;
                            return true;
                        }
                        if (CData.Dev.bDual == eDual.Dual)
                        {
                            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                            //syc : ase kr loading stop 
                            if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/) // 1 == table Loading Stop 일시
                            {
                                //if (((iSeq == ESeq.ONP_WaitPlace) || (iSeq == ESeq.ONP_PlaceTbL)) && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs)
                                if ((((iSeq == ESeq.ONP_WaitPlace) || (iSeq == ESeq.ONP_PlaceTbL)) && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs) && !CSQ_Main.It.m_bPause) //Dual 모드일 경우 Loading Stop on 활성화 시 Left TB Plce 안함
                                {
                                    bPlaceTbL = true;
                                }
                                else if (iSeq == ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready && !CData.DrData[1].bDrs)
                                {
                                    if ((CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbR) && (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) != CData.SPos.dOFP_X_PickR))
                                    {
                                        bPlaceTbR = true;
                                    }
                                }
                            }
                            else //기존 : Loading Stop 아닐시
                            {
                                if (((iSeq == ESeq.ONP_WaitPlace) || (iSeq == ESeq.ONP_PlaceTbL)) && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs)
                                {
                                    bPlaceTbL = true;
                                }
                                else if (iSeq == ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready && !CData.DrData[1].bDrs)
                                {
                                    if ((CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbR) && (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) != CData.SPos.dOFP_X_PickR))
                                    {
                                        bPlaceTbR = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 200824 jym : Table skip 판단 조건 추가
                            if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready && !CData.DrData[1].bDrs && !CData.Opt.aTblSkip[(int)EWay.R])
                            {
                                if ((CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbL && CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbR) &&  (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickR))
                                { bPlaceTbR = true; }
                            }
                            else
                            {
                                if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs && !CData.Opt.aTblSkip[(int)EWay.L])
                                {
                                    if (CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbL && (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickL))
                                    { bPlaceTbL = true; }
                                }
                            }
                        }
                    }

                	// onpclean
                	if (iSeq == ESeq.ONP_Clean)
                	{ bClean = true; }

                    if (bWait)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.ONP_Wait;
                        _SetLog(">>>>> Wait start.");
                    }
                    if (bPickRail)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.ONP_Pick;
                        _SetLog(">>>>> Pick rail start.");
                    }
                    if (bPickTb)     // eEquType.Nomal
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.ONP_PickTbL;
                        _SetLog(">>>>> Pick left table start.");
                    }
                    if (bPlaceTbL)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_PlaceTbL;
                        SEQ = ESeq.ONP_PlaceTbL;
                        _SetLog(">>>>> Place left table start.");
                    }
                    if (bPlaceTbR)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_PlaceTbR;
                        SEQ = ESeq.ONP_PlaceTbR;
                        _SetLog(">>>>> Place right table start.");
                    }
	                if (bClean)
	                {
	                    m_iStep = 10;
	                    m_iPreStep = 0;
	                    CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Clean;
	                    SEQ = ESeq.ONP_Clean;
                        _SetLog(">>>>> Picker clean start.");
                    }
                }

                switch (SEQ)
                {
                    default:
                        return false;

                    case ESeq.ONP_Wait:     // eEquType.Normal
                        if (Cyl_Wait())
                        {
                            SEQ = ESeq.Idle;
                            if(m_bduringLotEnd)
                            {
                                m_bduringLotEnd = false;
                                CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_WorkEnd;
                                _SetLog(">>>>> Work End.");
                            }
                            _SetLog("<<<<< Wait finish.");
                        }
                        return false;

                    case ESeq.ONP_Pick:         // eEquType.Nomal
                        if (Cyl_PickRail())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Pick rail finish.");
                        }
                        return false;

                    case ESeq.ONP_PickTbL:
                        if (Cyl_PickTable())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Pick left table finish.");
                        }
                        return false;

                    case ESeq.ONP_PlaceTbL:
                        // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
                        //if (Cyl_Palce(EWay.L))
                        if (Cyl_Place(EWay.L))
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Place left table finish.");

                            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                            //syc : ase kr loading stop - Place Left Table 끝날시 Count 증가
                            if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/)
                            {
                                CData.Chk_AutoLoading_Count();
                            }
                        }
                        return false;

                    case ESeq.ONP_PlaceTbR:
                        // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
                        //if (Cyl_Palce(EWay.R))
                        if (Cyl_Place(EWay.R))
                        {
                            SEQ = ESeq.Idle;
                            
                            _SetLog("<<<<< Place right table finish.");
                            
                            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                            //syc : ase kr loading stop - nomal 모드일 경우 Place Right Table 끝날시에도 Count 증가
                            if (CData.Dev.bDual == eDual.Normal && CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/)
                            {
                                CData.Chk_AutoLoading_Count();
                            }
                        }
                        return false;

                    case ESeq.ONP_Clean:
                        if (Cyl_PickerClean())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Picker clean finish.");
                        }
                        return false;
                }
            }
            else    // eEquType.Pikcer
            {
                 
                if (CDataOption.Use2004U) //210928 syc : 2004U IV2 검사 전에 ONP 상태 기억하기 위해서 사용
                {
                    if (iSeq != ESeq.ONP_CheckBcr && iSeq != ESeq.ONP_CheckOri && iSeq != ESeq.ONP_CheckBcrOri && iSeq != ESeq.ONP_IV2) 
                    { m_iPreState = iSeq; }
                }
                else
                {
                    if (iSeq != ESeq.ONP_CheckBcr && iSeq != ESeq.ONP_CheckOri && iSeq != ESeq.ONP_CheckBcrOri) 
                    { m_iPreState = iSeq; }
                }
                // end syc

                if (SEQ == (int)ESeq.Idle)
                {
                    if (m_bReqStop) return false;
                    if (CSQ_Main.It.m_iStat == EStatus.Stop)
                    { return false; }

                    // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                    // syc : ase kr loading stop - 피커 타입
                    if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/) //1=table loading stop - 피커 bcr 타입 Loading stop시 멈추는 구문
                    {
                        if (CData.Dev.bDual == eDual.Dual) //듀얼 모드일 경우
                        {    //Loading Stop On 활성화시 현재 상태가 'Place Left Table' 이거나 'Wait Place' 일 경우 return  //Place Left상태 빼도 되는지 점검 및 테스트 진행 할것
                            if (CSQ_Main.It.m_bPause && ((CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbL) || (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WaitPlace)))
                            { return false; }
                        }
                        else
                        {   //Loading Stop On 활성화시 현재 상태가 'Place Left/Right Table' 이거나 'Wait Place' 일 경우 return  //Place Left상태 빼도 되는지 점검 및 테스트 진행 할것
                            if (CSQ_Main.It.m_bPause && ((CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbR) || (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbL) || (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WaitPlace)))
                            { return false; }
                        }
                    }
                    //syc end

                    //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                    bool bWait     = false;
                    bool bPickRail = false;
                    bool bPickTb   = false;
                    bool bPlaceTbL = false;
                    bool bPlaceTbR = false;
                    //20190604 ghk_onpbcr
                    bool bBcr    = false;
                    bool bOri    = false;
                    bool bBcrOri = false;
                    //20190611 ghk_onpclean
                    bool bClean = false;

                    //210927 syc : 2004U
                    // 2004U 전용 ONP IV2 검사
                    bool bIV2 = false;

                    if (iSeq == ESeq.ONP_WorkEnd)
                    {
                        return true;
					}

					if (iSeq == ESeq.ONP_Wait || iSeq == ESeq.Idle)
                    {
						bWait = true;
					}
					if (CDataOption.SecsUse == eSecsGem.NotUse)
                    {
                        if (iSeq == ESeq.ONP_Pick && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WaitPicker)
                        {
                            if (CDataOption.Package == ePkg.Strip)
                            {
                                if (!CIO.It.Get_X(eX.INR_StripBtmDetect))
                                {
                                    //CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            bPickRail = true;
                        }
                    }
                    else
                    {
                        if (iSeq == ESeq.ONP_Pick && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WaitPicker && CData.LotInfo.bLotOpen)
                        {
                            if (CDataOption.Package == ePkg.Strip)
                            {
                                if (!CIO.It.Get_X(eX.INR_StripBtmDetect))
                                {
                                    //CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            bPickRail = true;
                        }
                    }

                    //210928 syc : 2004U IV2 사용시 BCR 시퀀시 작동 조건 다름
                    if (CDataOption.Use2004U)
                    {
                        if( (iSeq == ESeq.ONP_Pick || iSeq == ESeq.ONP_IV2)         &&  CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckIV2)     {   bIV2    = true; }
                    }
                    // syc end
                    else // 기존 코드
                    {
                        if ((iSeq == ESeq.ONP_Pick || iSeq == ESeq.ONP_CheckBcrOri) &&  CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcrOri)  {   bBcrOri = true; }
                        if ((iSeq == ESeq.ONP_Pick || iSeq == ESeq.ONP_CheckBcr)    &&  CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcr)     {   bBcr    = true; }
                        if ((iSeq == ESeq.ONP_Pick || iSeq == ESeq.ONP_CheckOri)    &&  CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckOri)     {   bOri    = true; }
                        if ( iSeq == ESeq.ONP_Pick                                  &&  CData.Parts[(int)EPart.INR].iStat == ESeq.INR_OriReady)     {   bOri    = true; }
                    }

                    if (iSeq == ESeq.ONP_Pick && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd)
                    {
                        m_bduringLotEnd = true;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait;
                        return true;
                    }

                    //---- 2022.08.19 lhs Start : // 2004U, Wait 버튼 클릭(ONP_Wait->ONP_Pick)시만 허용 
                    if (CDataOption.Use2004U)   {   m_bRetryPickTbL = (iSeq == ESeq.ONP_Pick && m_bRetryPickTbL);   }
                    else                        {   m_bRetryPickTbL = false;    /*2004U 아닐 경우 영향 없도록..*/      }
                    //---- 2022.08.19 lhs End

                    if ( ( iSeq == ESeq.ONP_PickTbL || m_bRetryPickTbL )            &&      // 2022.08.19 lhs : 재진입 위한 m_bRetryPickTbL 추가
                          CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick   && 
                          CData.Parts[(int)EPart.INR].iStat  != ESeq.INR_CheckBcr)          { bPickTb = true;   m_bRetryPickTbL = false; }     // eEquType.Picker

                    if (iSeq == ESeq.ONP_PickTbL && CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick)
                    {
                        if (CDataOption.Use2004U)  //210928 syc : 2004U IV2 사용시 BCR 시퀀시 작동 조건 다름
                        {
                            if (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckIV2)     { bIV2      = true; }
                        }
                        else
                        {
                            if (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcrOri)  { bBcrOri   = true; }
                            if (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcr)     { bBcr      = true; }
                        }
                    }

                    if (iSeq == ESeq.ONP_WaitPlace || iSeq == ESeq.ONP_PlaceTbL || iSeq == ESeq.ONP_PlaceTbR)
                    {
                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            //20200428 jhc : 자재 있으나 버큠 체크 안 될 경우 에러 처리 => 이 때, 픽업된 자재는 제거해야 함
                            if (CData.Parts[(int)EPart.ONP].bExistStrip == true)
                            { 
                                CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                            }
                            //
                            //CErr.Show(eErr.ONLOADERPICKER_DETECT_STRIP_ERROR);
                            CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Pick;
                            m_iStep = 0;
                            return true;
                        }

                        if (CData.Dev.bDual == eDual.Dual)
                        {
                            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                            if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/)//Loading Stop일때
                            {
                                //syc : ase kr loading stop
                                //if (((iSeq == ESeq.ONP_WaitPlace) || (iSeq == ESeq.ONP_PlaceTbL)) && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs)
                                //Dual 모드일 경우 Loading Stop on 활성화 시 Left TB Plce 안함
                                if ((((iSeq == ESeq.ONP_WaitPlace) || (iSeq == ESeq.ONP_PlaceTbL)) && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs) && !CSQ_Main.It.m_bPause)
                                { bPlaceTbL = true; }
                                else if (iSeq == ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready && !CData.DrData[1].bDrs)
                                {
                                    if ((CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbR) && (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) != CData.SPos.dOFP_X_PickR))
                                    { bPlaceTbR = true; }
                                }
                            }
                            else //기존 Loading stop 아닐 시
                            {
                                if (((iSeq == ESeq.ONP_WaitPlace) || (iSeq == ESeq.ONP_PlaceTbL)) && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs)
                                {
                                    bPlaceTbL = true;
                                }
                                else if (iSeq == ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready && !CData.DrData[1].bDrs)
                                {
                                    if ((CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbR) && (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) != CData.SPos.dOFP_X_PickR))
                                    {
                                        bPlaceTbR = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready && !CData.DrData[1].bDrs)
                            {
                                if (((CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbL && CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbR)) &&  (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickR))
                                {
                                    bPlaceTbR = true;
                                }
                            }
                            else
                            {
                                if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready && !CData.DrData[0].bDrs)
                                {
                                    if (CData.Parts[(int)EPart.OFP].iStat != ESeq.OFP_PickTbL && (CMot.It.Get_CP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickL))
                                    {
                                        bPlaceTbL = true;
                                    }
                                }
                            }
                            //210928 syc : 2004U IV2 사용시 BCR 시퀀시 작동 조건 다름
                            if (CDataOption.Use2004U)
                            {
                                if (CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready && 
                                    CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_Ready && 
                                    CData.Parts[(int)EPart.INR].iStat  == ESeq.INR_CheckIV2)
                                {
                                    bIV2 = true;
                                }
                            }                            
                            else //기존 코드
                            {
                                //190821 ksg :
                                if (CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready && CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_Ready && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcrOri)
                                {
                                    bBcrOri = true;
                                }

                                if (CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready && CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_Ready && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcr)
                                {
                                    bBcr = true;
                                }
                            }
                            //syc end
                        }
                    }

                    //20190611 ghk_onpclean
                    if (iSeq == ESeq.ONP_Clean)
                    {
                        bClean = true;
                    }

                    //210927 syc : 2004U
                    if (iSeq == ESeq.ONP_IV2 && CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckIV2) { bIV2 = true; }

                    //210928 syc : 2004U IV2 사용시 BCR 시퀀시 작동 조건 다름
                    if (CDataOption.Use2004U)
                    {
                        if      (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcrOri)  { bBcrOri  = true; }
                        else if (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckBcr   )  { bBcr     = true; }
                        else if (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_CheckOri   )  { bOri     = true; }
                    }
                    // syc end

                    if (bWait)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.ONP_Wait;
                        _SetLog(">>>>> Wait start.");
                    }

                    if (bPickRail)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.ONP_Pick;
                        _SetLog(">>>>> Pick rail start.");
                    }

                    if (bPickTb)     // eEquType.Picker
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ         = ESeq.ONP_PickTbL;
                        _SetLog(">>>>> Pick left table start.");
                    }

                    if (bPlaceTbL)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_PlaceTbL;
                        SEQ = ESeq.ONP_PlaceTbL;
                        _SetLog(">>>>> Place left table start.");
                    }

                    if (bPlaceTbR)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_PlaceTbR;
                        SEQ = ESeq.ONP_PlaceTbR;
                        _SetLog(">>>>> Place right table start.");
                    }

                    //190821 ksg :
                    if (bBcrOri)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_CheckBcrOri;
                        SEQ = ESeq.ONP_CheckBcrOri;
                        _SetLog(">>>>> Check bcr, ori start.");
                    }

                    //20190604 ghk_onpbcr
                    if (bBcr)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_CheckBcr;
                        SEQ = ESeq.ONP_CheckBcr;
                        _SetLog(">>>>> Check bcr start.");
                    }

                    if (bOri)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_CheckOri;
                        SEQ = ESeq.ONP_CheckOri;
                        _SetLog(">>>>> Check ori start.");
                    }

                    if (bClean)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Clean;
                        SEQ = ESeq.ONP_Clean;
                        _SetLog(">>>>> Picker clean start.");
                    }

                    if (bIV2) //210927 syc : 2004U
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_IV2;
                        SEQ         = ESeq.ONP_IV2;
                        _SetLog(">>>>> ONP IV2 Check start.");
                    }
                }

                switch (SEQ)
                {
                    default:
                        return false;

                    case ESeq.ONP_Wait:     // eEquType.Picker
                        if (Cyl_Wait())
                        {
                            SEQ = ESeq.Idle;
                            if(m_bduringLotEnd)
                            {
                                m_bduringLotEnd = false;
                                CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_WorkEnd;
                                _SetLog(">>>>> Work End.");
                            }
                            _SetLog("<<<<< Wait end.");
                        }
                        return false;

                    case ESeq.ONP_Pick:     // eEquType.Pikcer
                        if (Cyl_PickRail())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Pick rail end.");
                        }
                        return false;

                    case ESeq.ONP_PickTbL:
                        if (Cyl_PickTable())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Pick left table end.");
                        }
                        return false;

                    case ESeq.ONP_PlaceTbL:
                        // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
                        //if (Cyl_Palce(EWay.L))
                        if (Cyl_Place(EWay.L))
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Place left table end.");

                            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                            //syc : ase kr loading stop 
                            if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/)
                            {
                                CData.Chk_AutoLoading_Count();
                            }  //Place Left Table 끝날시 Count 증가
                        }
                        return false;

                    case ESeq.ONP_PlaceTbR:
                        // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
                        //if (Cyl_Palce(EWay.R))
                        if (Cyl_Place(EWay.R))
                        {
                            SEQ = ESeq.Idle;
                            
                            _SetLog("<<<<< Place right table end.");

                            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
                            //syc : ase kr loading stop
                            if (CData.Dev.bDual == eDual.Normal && CData.Opt.iLoadingStopType == (int)eTypeLDStop.Table/*1*/)
                            {
                                CData.Chk_AutoLoading_Count();
                            } //nomal 모드일 경우 Place Right Table 끝날시에도 Count 증가
                        }
                        return false;
                    //190821 ksg :
                    case ESeq.ONP_CheckBcrOri:
                        if (Cyl_OnpOriBcr())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Check bcr, ori end.");
                        }
                        return false;

                    case ESeq.ONP_CheckBcr:
                        if (Cyl_OnpBcr())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Check bcr end.");
                        }
                        return false;

                    case ESeq.ONP_CheckOri:
                        if (Cyl_OnpOri())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Check ori end.");
                        }
                        return false;

                    case ESeq.ONP_Clean:
                        if (Cyl_PickerClean())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Picker clean end.");
                        }
                        return false;

                    //210927 syc : 2004U
                    case ESeq.ONP_IV2:
                        if (Cyl_ONP_IV2())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< ONP IV2 Check end.");
                        }
                        return false;
                }
            }
        }
        #endregion

        #region Cycle
        public void Init_Cyl()
        {
            m_iStep = 10;
            m_iPreStep = 0;
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
                    CErr.Show(eErr.ONLOADERPICKER_SERVO_ON_TIMEOUT);
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
                        CMot.It.Set_SOn(m_iZ, bVal);
                        if (CDataOption.CurEqu == eEquType.Pikcer)
                        { CMot.It.Set_SOn(m_iY, bVal); }
                        _SetLog("All servo-on : " + bVal);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {// 2. 서보 온 상태 체크
                        bool bRet = false;
                        if (CDataOption.CurEqu == eEquType.Pikcer)
                        {
                            if (CMot.It.Chk_Srv(m_iX) == bVal &&
                                CMot.It.Chk_Srv(m_iZ) == bVal &&
                                CMot.It.Chk_Srv(m_iY) == bVal)
                            { bRet = true; }
                        }
                        else
                        {
                            if (CMot.It.Chk_Srv(m_iX) == bVal &&
                                CMot.It.Chk_Srv(m_iZ) == bVal)
                            { bRet = true; }
                        }

                        if (bRet)
                        {
                            _SetLog("Check all servo.");

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
            if ((m_iStep < 11) && (m_iStep != m_iPreStep))
            {
                m_TiOut.Set_Delay(TIMEOUT*2);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_HOME_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if(CDataOption.CurEqu == eEquType.Nomal)
            {
                switch (m_iStep)
                {
                    default:
                        {
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
                            _SetLog("Check axes.");
                        
                            m_iStep++;
                            return false;
                        }

                    case 11: // 이젝트 오프.  Z 축 호밍
                        {
                            m_bHD = false;
                            Func_Eject(false);
                            CMot.It.Mv_H(m_iZ);
                            _SetLog("Z axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 12: // Z축 홈 확인.  피커 90도
                        {
                            if (!CMot.It.Get_HD(m_iZ))
                            { return false; }

                            Func_Picker90();
                            _SetLog("Picker 90.");

                            m_iStep++;
                            return false;
                        }

                    case 13: // 피커 90도 확인.  X축 홈
                        {
                            if (!Func_Picker90())
                            { return false; }

                            CMot.It.Mv_H(m_iX);
                            _SetLog("X axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 14:
                        {
                            if (!CMot.It.Get_HD(m_iX))
                            { return false; }
                            
                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);
                            CMot.It.Mv_N(m_iX, CData.Dev .dOnP_X_Wait);//X축 Wait
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);
                            Func_Picker90();
                            _SetLog("Picker 90.");

                            m_iStep++;
                            return false;
                        }

                    case 15:
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                            { return false; }
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }
                            if (!Func_Picker90())
                            { return false; }

                            m_bHD = true;
                            _SetLog("Finish.");

                            m_iStep = 0;
                            return true;
                        }
                }
            }
            else 
            { 
                switch (m_iStep)
                {
                    default:
                        {
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

                            _SetLog("Check axes.");
                        
                            m_iStep++;
                            return false;
                        }

                    case 11:
                        {
                            m_bHD = false;
                            Func_Eject(false);
                            CMot.It.Mv_H(m_iZ);
                            _SetLog("Z axsi homing.");

                            m_iStep++;
                            return false;
                        }


                    case 12:
                        {
                            if (!CMot.It.Get_HD(m_iZ))
                            { return false; }
                        
                            CMot.It.Mv_I(m_iX, 50);//190828 ksg :
                            _SetLog("X axis move step 50mm.");

                            m_iStep++;
                            return false;
                        }

                    case 13:
                        {
                            if (!CMot.It.Get_Mv(m_iX))
                            { return false; }
                        
                            CMot.It.Mv_H(m_iY);//190828 ksg :
                            _SetLog("Y axis move homing.");

                            m_iStep++;
                            return false;
                        }

                    case 14:
                        {
                            if (!CMot.It.Get_HD(m_iY))
                            { return false; }                        
                            
                            CMot.It.Mv_H(m_iX);
                            _SetLog("X axis homing.");

                            m_iStep++;
                            return false;
                        }

                    case 15:
                        {
                            if (!CMot.It.Get_HD(m_iX))
                            { return false; }


                            if (CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait) == 0)
                            {
                                _SetLog("Y axis move [Wait] position.", CData.SPos.dONP_Y_Wait);

                                m_iStep++;
                            }
                            return false;
                        }

                    case 16:
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                            { return false; }

                            m_dPos = (CData.Dev.dOnP_X_Wait + CData.SPos.dONP_X_PlaceL) / 2;
                            CMot.It.Mv_N(m_iX, m_dPos);
                            _SetLog("X axis move position.", m_dPos);

                            m_iStep++;
                            return false;
                        }

                    case 17:
                        {
                            if (!CMot.It.Get_Mv(m_iX, m_dPos))
                            { return false; }

                            Func_Picker90();
                            _SetLog("Picker 90.");

                            m_iStep++;
                            return false;
                        }

                    case 18:
                        {
                            if (!Func_Picker90())
                            { return false; }

                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);

                            m_iStep++;
                            return false;

                        }
                    case 19:
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }
                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                            CMot.It.Mv_N(m_iX, CData.Dev .dOnP_X_Wait);//X축 Wait
                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);

                            Func_Picker90();
                            _SetLog("Picker 90");

                            m_iStep++;
                            return false;
                        }

                    case 20:
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                            { return false; }
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }
                            //20190604 ghk_onpbcr
                            if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                            { return false; }
                            if (!Func_Picker90())
                            { return false; }

                            m_bHD = true;
                            _SetLog("Finish.");

                            m_iStep = 0;
                            return true;
                        }
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
                    CErr.Show(eErr.ONLOADERPICKER_WAIT_TIMEOUT);
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
                    {//Z축 대기위치 이동
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Check axes.  Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Z축 대기위치 이동 확인, 이젝트 오프, 피커 90도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        m_dPos = (CData.Dev.dOnP_X_Wait + CData.SPos.dONP_X_PlaceL) / 2;
                        CMot.It.Mv_N(m_iX, m_dPos);
                        _SetLog("X axis move position.", m_dPos);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!CMot.It.Get_Mv(m_iX, m_dPos))
                        { return false; }

                        Func_Eject(false);
                        Func_Picker90();
                        _SetLog("Eject off.  Picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//피커 90도 확인, X축 대기위치 이동
                        if (!Func_Picker90())
                        { return false; }

                        //220106 pjh : ASE KH 요청으로 Onp Wait 시 Wait 동작 순서 변경 (기존 : Z > X > Y, 변경 : Z > Y > X)
                        //220114 pjh : Inari에서도 동일 현상 발생하여 Site 옵션 제외
                        if (CDataOption.CurEqu == eEquType.Pikcer)
                        {
                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);
                        }
                        //
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait); 
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//X축 대기위치 이동 확인, 상태 변경
                        //220106 pjh : ASE KH 요청으로 Onp Wait 시 Wait 동작 순서 변경 (기존 : Z > X > Y, 변경 : Z > Y > X)
                        //220114 pjh : Inari에서도 동일 현상 발생하여 Site 옵션 제외
                        if (CDataOption.CurEqu == eEquType.Pikcer)
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                            { return false; }

                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);
                        }
                        //
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }

                            if (CDataOption.CurEqu == eEquType.Nomal)
                            {
                                _SetLog("X axis move check.");
                                m_iStep = 16;
                                return false;
                            }

                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);
                        }

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        //220106 pjh : ASE KH 요청으로 Onp Wait 시 Wait 동작 순서 변경 (기존 : Z > X > Y, 변경 : Z > Y > X)
                        //220114 pjh : Inari에서도 동일 현상 발생하여 Site 옵션 제외
                        if (CDataOption.CurEqu == eEquType.Pikcer)
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }

                            _SetLog("X axis move check.");
                        }
                        //
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                            { return false; }

                            _SetLog("Y axis move check.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Pick;
                        _SetLog("Status : " + CData.Parts[(int)EPart.ONP].iStat);

                        _SetLog("Finish.");                        

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Pick Rail Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PickRail()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            //{ m_TiOut.Set_Delay(TIMEOUT); }
            { m_TiOut.Set_Delay(60000); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_PICKRAIL_TIMEOUT);
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
                    {//버큠 오프, 이젝트 오프, 워터 드레인 온, Z축 대기위치 이동, 인레일 얼라인 가이드 백워드
                        m_tStTime = DateTime.Now;
                        if (CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error : Picker detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        // 200319 mjy : Unit 모드에서 자재 유무 와 매칭 확인
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bE1 = CData.Parts[(int)EPart.INR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 0 : 0];
                            bool bE2 = CData.Parts[(int)EPart.INR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 1 : 0];
                            bool bE3 = CData.Parts[(int)EPart.INR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 2 : 1];
                            bool bE4 = CData.Parts[(int)EPart.INR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 3 : 1];
                            _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}  #3:{2}  #4{3}", bE1, bE2, bE3, bE4));

                            bool bX1 = CIO.It.Get_X(eX.INR_Unit_1_Detect);
                            bool bX2 = CIO.It.Get_X(eX.INR_Unit_2_Detect);
                            bool bX3 = CIO.It.Get_X(eX.INR_Unit_3_Detect);
                            bool bX4 = CIO.It.Get_X(eX.INR_Unit_4_Detect);
                            _SetLog(string.Format("Unit detect check.  #1:{0}  #2:{1}  #3:{2}  #4{3}", bX1, bX2, bX3, bX4));

                            if ((bE1 != bX1) || (bE2 != bX2) || (bE3 != bX3) || (bE4 != bX4))
                            {
                                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                                _SetLog("Error : Inrail not detected carrier.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                        {
                            if (!CSq_Inr.It.Chk_Strip())//20191121 ghk_display_strip
                            {//인레일에 자재 없을 경우 에러
                                CErr.Show(eErr.INRAIL_NOT_DETECTED_STRIP);
                                _SetLog("Error : Inrail not detected strip.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        //190716 ksg : 조양 요청
                        if (!CMot.It.Get_Mv(m_iInRX, CData.SPos.dINR_X_Wait))
                        {
                            CErr.Show(eErr.INRAIL_X_AXIS_NOT_WIAT_POSITION);
                            _SetLog("Error : INR X axis not wait position.");

                            m_iStep = 0;
                            return true;

                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Func_Vacuum(false);
                        Func_Eject(false);
                        Func_Drain(true);

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait); //상단 이동

                        if (!CDataOption.Use2004U)
                        {
                            if (!CSq_Inr.It.Act_YAlignBF(true))
                            { CSq_Inr.It.Act_YAlignBF(true); }// 200131 LCY
                        }
                        _SetLog("Check axes.  Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Z축 대기위치 이동 확인, 피커 90도, X축 대기 위치 이동, 인레일 얼라인 가이드 백워드 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }
                        if (!CDataOption.Use2004U)
                        {
                            if (!CSq_Inr.It.Act_YAlignBF(true)) // 200131 LCY
                            { return false; }
                        }
                        Func_Picker90();

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait); //X축 Rail Pick이동
                        
                        _SetLog("Picker 90.  X axis move wait.", CData.Dev.dOnP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//피커 90도 확인, X축 대기 위치 이동 확인, 워트 드레인 오프, Z축 pick - slow 위치 이동
                        if (!Func_Picker90())                               { return false; }
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))   { return false; }

                        Func_Drain(false);

                        _SetLog("Drain off.  ", (CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//20200910 OCR 조건으로 인해 추가 LCY

                        if ((CData.Opt.bSecsUse) && (CDataOption.CurEqu == eEquType.Nomal)) // Normal=Inrail //20200911 Btm 에서 BCR 이 이미 읽은 상태 LCY 
                        {
                            if (CData.GemForm != null)
                            {
#if false
                                // 2021.08.23 SungTae Start : [TEMP] 디버깅 위해 STRIP ID를 강제 입력. 추후 제거 예정
                                /*
                                 * true : Auto Run 진행 시
                                 * false : Debugging으로 Dry Run 진행 시
                                 */

                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = "LOT2_STRIP2";//CData.Bcr.sBcr;
                                // 2021.08.23 SungTae End
#endif
                                if (CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID != "")
                                {
                                    // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                                    CData.GemForm.Strip_Data_Shift((int)EDataShift.INR_DF_MEAS/*3*/, (int)EDataShift.INR_BCR_READ/*4*/);
                                    
                                    CLog.mLogGnd(string.Format("Cyl_PickRail Step - 13 CData.Bcr.sBcr {0} Verify ", CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID));

                                    _SetLog($"[SEND](H<-E) S6F11 CEID = {(int)SECSGEM.JSCK.eCEID.Carrier_Verify_Request}({SECSGEM.JSCK.eCEID.Carrier_Verify_Request}).  " +
                                            $"Carrier ID : {CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID}");

                                    // (EQ -> Host) Carrier Verify Request 요청
                                    CData.GemForm.OnCarrierVerifyRequset(CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID);
                                    // 2021.07.19 SungTae End

                                    m_Delay.Set_Delay(3 * 60 * 1000);
                                    //_SetLog("Cyl_PickRail Step - 13 CData.Bcr.sBcr Verify ");
                                    _SetLog("Carrier Verify Request Result : Verified.");

                                    m_iStep++;
                                    return false;
                                }
                                else
                                {
                                    _SetLog("Cyl_PickRail Step - 13 CData.Bcr.sBcr Verify Error CData.JSCK_Gem_Data[3].sInRail_Strip_ID = 0");
                                    return true;
                                }
                            }
                            else
                            {
                                _SetLog("Cyl_PickRail Step - 13 CData.GemForm = null");
                                return true;
                            }
                        }
                        else
                        {
                            m_Delay.Set_Delay(30 * 1000);

                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripSuccess)/*3*/;
                            //_SetLog("Cyl_PickRail Step - 13 CData.SecsGem_Data.nStrip_Check " , CData.SecsGem_Data.nStrip_Check);
                            //_SetLog("[RECV](HOST->EQ) S6F12, Check Strip Verify = {0} (Strip Success).", CData.SecsGem_Data.nStrip_Check);
                            _SetLog($"[RECV](HOST->EQ) S6F12, Check Strip Verify = {CData.SecsGem_Data.nStrip_Check} (Strip Success).");

                            m_iStep++;
                            return false;
                        }
                    }

                case 14:
                    {
                        m_TiOut.Set_Delay(TIMEOUT); 

                        if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripSuccess)/*3*/)
                        {
                            //// 2021.08.03 lhs End : 여기는 필요 없을 듯. 사내 63X 설비에서 테스트시 필요.
                            ////Host 에서 DF Verify 성공
                            //if (CDataOption.UseNewSckGrindProc)
                            //{
                            //    CData.Parts[(int)EPart.INR].dPcbMin     = CData.JSCK_Gem_Data[4].fMeasure_Min_Data[0];
                            //    CData.Parts[(int)EPart.INR].dPcbMax     = CData.JSCK_Gem_Data[4].fMeasure_Max_Data[0];
                            //    CData.Parts[(int)EPart.INR].dPcbMean    = CData.JSCK_Gem_Data[4].fMeasure_Avr_Data[0];
                            //    CData.JSCK_Gem_Data[4].nDF_User_New_Mode = CData.JSCK_Gem_Data[4].nDF_User_Mode;

                            //    CData.Parts[(int)EPart.INR].dTopMoldMax = CData.JSCK_Gem_Data[4].dMeasure_TopMold_Max;
                            //    CData.Parts[(int)EPart.INR].dTopMoldAvg = CData.JSCK_Gem_Data[4].dMeasure_TopMold_Avg;
                            //    CData.Parts[(int)EPart.INR].dBtmMoldMax = CData.JSCK_Gem_Data[4].dMeasure_BtmMold_Max;
                            //    CData.Parts[(int)EPart.INR].dBtmMoldAvg = CData.JSCK_Gem_Data[4].dMeasure_BtmMold_Avg;
                            //}
                            //// 2021.08.03 lhs End
							
							// 2022.01.04 lhs Start : MultiLot에서 SecsGem을 사용하지 않는 경우 LotName이 바뀌는 문제 수정
                            //// 2021.08.27 SugnTae Start : [추가]
                            //CData.Parts[(int)EPart.INR].sLotName = (CData.IsMultiLOT() == true) ? CData.LotMgr.LoadingLotName : CData.LotInfo.sLotName;
                            if (CData.IsMultiLOT())  // SecsGem 사용할 경우와 아닐 경우 구분 
                            {
                                if (CData.Opt.bSecsUse)
                                {
                                    CData.Parts[(int)EPart.INR].sLotName = CData.LotMgr.LoadingLotName;
                                    CData.Parts[(int)EPart.INR].LotColor = CData.LotMgr.GetLotColor(CData.LotMgr.LoadingLotName);

                                    // 2022.06.02 SungTae : [추가] (ASE-KR)
                                    CData.Parts[(int)EPart.ONP].sBcr     = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID;

                                    // 2022.06.08 SungTae Start : [추가] Multi-LOT 관련
                                    int nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                                    CData.LotMgr.ListLOTInfo[nIdx].nState = ELotState.eRun;
                                    // 2022.06.08 SungTae End
                                }
                                else {   /*냉무*/ } 
                            }
                            else
                            {
                                CData.Parts[(int)EPart.INR].sLotName = CData.LotInfo.sLotName;
                            }
                            // 2022.01.04 lhs End : MultiLot에서 SecsGem을 사용하지 않는 경우 LotName이 바뀌는 문제 수정


                            CMot.It.Mv_N(m_iZ, (CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow)); //Z축 PrePick 위치
                            
                            //_SetLog("Finish CData.SecsGem_Data.nStrip_Check , Z-axis Mv , Position =   ", CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow);
                            _SetLog("OFP Z-axis Move to Pre-Pick Position =   ", CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow);

                            m_iStep++;
                            return false;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.ReqError)/*-1*/)
                        {
                            //Carrier ID Verifaction 하지 않아는데 "LOT_VERIFICATION" 내려옴
                            CErr.Show(eErr.HOST_NOT_REQUEST_ERROR);
                            
                            _SetLog("[RECV](HOST->EQ) S6F12. Check Strip Verify = -1 (Request Error) : Carrier ID Verifaction 하지 않았는데 LOT_VERIFICATION 내려옴");
                            
                            m_iStep = 0;
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotError)/*-2*/)
                        {
                            //Lot Verify 시 Strip Verificatrion 요청 하지 않았으나 Data 내려옴
                            CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);
                            
                            _SetLog("[RECV](HOST->EQ) S6F12. Check Strip Verify = -2 (LOT Verify Time Over Error) : Lot Verify 시 Strip Verification 요청하지 않았으나 Data 내려옴");
                            
                            m_iStep = 0;
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripError)/*-3*/)
                        {
                            //Lot Varification Strip Count 0 이라서 에러
                            CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);
                            
                            _SetLog("[RECV](HOST->EQ) S6F12. Check Strip Verify = -3 (Strip Verify Time Over Error) : Lot Verification 시 Strip Count가 0이라서 에러");
                            
                            m_iStep = 0;
                            return true;
                        }
                        else if (m_Delay.Chk_Delay())
                        {
                            if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.InitVerify)/*1*/)
                            {
                                CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);
                                
                                _SetLog("[RECV](HOST->EQ) S6F12. Check Strip Verify = 1 (LOT Verify Time Over Error)");
                                
                                m_iStep = 0;
                                return true;
                            }
                            else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotSuccess)/*2*/)
                            {
                                CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);
                                
                                _SetLog("[RECV](HOST->EQ) S6F12. Check Strip Verify = 2 (Strip Verify Time Over Error)");
                                
                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                CErr.Show(eErr.HOST_STRIP_VERIFY_HOLD_TIME_OVER_ERROR);
                                
                                _SetLog("[RECV](HOST->EQ) S6F12. Check Strip Verify = 3 (Strip Verify Hold Time Over Error)");      // 2022.04.06 SungTae : [수정] 오타 수정 (LOT -> Strip)
                                
                                m_iStep = 0;
                                return true;
                            }
                        }

                        return false;
                    }

                case 15:
                    {//Z축 pick - slow 위치 이동 확인, Z축 pick 위치 이동(슬로우)
                        if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow))) { return false; }

                        CMot.It.Mv_S(m_iZ, CData.Dev.dOnP_Z_Pick);
                        _SetLog("Z axis move pick(slow).", CData.Dev.dOnP_Z_Pick);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//Z축 pick 위치 이동(슬로우) 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Pick))
                        { return false; }

                        m_Delay.Set_Delay(500);
                        _SetLog("Set delay : 500ms");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        //210915 syc : 2004U
                        if (!CDataOption.Use2004U)
                        {
                            CSq_Inr.It.Act_YAlignBF(false);// 200131 LCY
                            m_Delay.Set_Delay(3000);
                            _SetLog("INR align backward.  Set delay : 3000ms");

                            m_iStep++;
                            return false;
                        }
                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        //210902 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            m_iStep++;
                            return false;
                        }
                        else
                        {
                            if (CSq_Inr.It.Act_YAlignBF(false))// Backward Check LCY 200113
                            {
                                m_Delay.Set_Delay(500);
                                _SetLog("INR align backward check.  Set delay : 500ms");

                                m_iStep++;
                                return false;
                            }
                            else if (m_Delay.Chk_Delay())
                            {

                                CErr.Show(eErr.INRAIL_ALIGN_TIMEOUT);
                                _SetLog("Error : INR align timeout.");

                                m_iStep = 0;
                                return true;
                            }
                            else
                            { return false; }
                        }                        
                    }

                case 19:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        CSq_Inr.It.Act_ZLiftDU(true);
                        m_Delay.Set_Delay(3000);
                        _SetLog("INR lift up.  Set delay : 3000ms");

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {
                        if (CSq_Inr.It.Act_ZLiftDU(true))
                        {
                            m_Delay.Set_Delay(500);
                            _SetLog("Set delay : 500ms");

                            m_iStep++;
                            return false;
                        }
                        else if (m_Delay.Chk_Delay())
                        {
                            CErr.Show(eErr.INRAIL_LIFT_UD_ERROR);
                            _SetLog("Error : INR lift up fail.");

                            m_iStep = 0;
                            return true;
                        }
                        else 
                        { return false; }
                    }

                case 21:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Func_Vacuum(true);
                        m_Delay_01.Set_Delay(100);// 진공 센서 On 후 경과 시간
                        m_Delay.Set_Delay(10000);// 진공 센서 타임 아웃 시간
                        _SetLog("Vacuum on.  Set delay : 10000ms");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if (m_Delay.Chk_Delay() && !CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                            _SetLog("Error : Picker vacuum fail.");
                            CSq_Inr.It.Act_ZLiftDU(false);
                            Func_Vacuum(false);
                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);

                            m_iStep = 0;
                            return true;
                        }
                        else if (CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            m_Delay_01.Set_Delay(2000);
                            _SetLog("Picker vacuum success.  Set delay : 2000ms");

                            m_iStep++;
                        }

                        return false;
                    }

                case 23:
                    {//Z축 pick - slow 위치 이동(슬로우)
                        if (!m_Delay_01.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_S(m_iZ, (CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow)); // PrePick 위치
                        _SetLog("Z axis move position(slow).", (CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 24:
                    { //Z축 pick - slow 위치 이동(슬로우) 확인, Z축 Wait 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//Z축 wait 위치 이동 확인, 리프트 다운, 종료
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                            _SetLog("Error : Picker vacuum fail.");

                            Func_Vacuum(false);
                            CSq_Inr.It.Act_ZLiftDU(false);
                            
                            m_iStep = 0;
                            return true;
                        }
                        
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) //200121 ksg : ,200625 lks
                        {
                            if(CIO.It.Get_Y(eY.IOZL_Power)) CIO.It.Set_Y(eY.IOZL_Power, false);  // SCK, JSCK 
                        }

                        // 2021.09.15 lhs Start 
                        //// 2020.09.21 JSKim St
                        //if (CData.CurCompany == ECompany.JCET)
                        //{
                        //    if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);  // JCET
                        //}
                        //// 2020.09.21 JSKim Ed

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg : , 200625 lks
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);  // SCK, JSCK, JCET
                        }
                        // 2021.09.15 lhs End 

                        Array.Copy(CData.Parts[(int)EPart.INR].dPcb, CData.Parts[(int)EPart.ONP].dPcb, CData.Parts[(int)EPart.INR].dPcb.Length);
                        //if (CData.Opt.bSecsUse == true || CDataOption.MeasureDf == eDfServerType.MeasureDf || (CData.CurCompany == ECompany.Qorvo_NC && !CData.Dev.bDynamicSkip))//210603 pjh : 조건 추가                        
                        if (CData.Opt.bSecsUse == true || CDataOption.MeasureDf == eDfServerType.MeasureDf || (CDataOption.UseDFDataServer && !CData.Dev.bDynamicSkip))//210818 pjh : D/F Data server 기능 License로 구분
                        {   // SECSGEM 사용시 Host에서 Down 받은 Data 사용 20200315 LCY
                            CData.Parts[(int)EPart.ONP].dPcbMin  = CData.Parts[(int)EPart.INR].dPcbMin;
                            CData.Parts[(int)EPart.ONP].dPcbMax  = CData.Parts[(int)EPart.INR].dPcbMax;
                            CData.Parts[(int)EPart.ONP].dPcbMean = CData.Parts[(int)EPart.INR].dPcbMean;

                            // 2021.08.03 lhs Start : Host에서 받은 Top/Btm Mold 데이터
                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.Parts[(int)EPart.ONP].dTopMoldMax = CData.Parts[(int)EPart.INR].dTopMoldMax;
                                CData.Parts[(int)EPart.ONP].dTopMoldAvg = CData.Parts[(int)EPart.INR].dTopMoldAvg;
                                CData.Parts[(int)EPart.ONP].dBtmMoldMax = CData.Parts[(int)EPart.INR].dBtmMoldMax;
                                CData.Parts[(int)EPart.ONP].dBtmMoldAvg = CData.Parts[(int)EPart.INR].dBtmMoldAvg;
                            }
                            // 2021.08.03 lhs End
								
                            if (CData.GemForm != null)
                            {
                                // 2022.06.07 SungTae Start : [수정] ASE-KR Multi-LOT 관련
                                // Carrier Start 요청 후 -> Data Shift 진행
                                //CData.GemForm.OnCarrierStarted(CData.LotMgr.LoadingLotName/*CData.Parts[(int)EPart.ONP].sLotName*/, CData.Parts[(int)EPart.ONP].sBcr);
                                CData.GemForm.OnCarrierStarted(CData.LotMgr.LoadingLotName, CData.Parts[(int)EPart.INR].sBcr);

                                CData.GemForm.Strip_Data_Shift((int)EDataShift.INR_BCR_READ/*4*/, (int)EDataShift.ONP_STRIP_PICK/*5*/);     // BCR Data -> Picker  로 이동

                                // 2022.05.11 SungTae Start : [추가] Log
                                _SetLog($"[SEND](H<-E) S6F11 CEID = {(int)SECSGEM.JSCK.eCEID.Carrier_Started}({SECSGEM.JSCK.eCEID.Carrier_Started}).  " +
                                        $"LOT : {CData.LotMgr.LoadingLotName}, Carrier ID : {CData.Parts[(int)EPart.ONP].sBcr}");

                                _SetLog($"Data Shift : {EDataShift.INR_BCR_READ}({(int)EDataShift.INR_BCR_READ}) -> {EDataShift.ONP_STRIP_PICK}({(int)EDataShift.ONP_STRIP_PICK})");
                                // 2022.05.11 SungTae End
                            }
                        }
                        else if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            //20191029 ghk_dfserver_notuse_df
                            if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                            {//DF 서버 사용시 DF 측정 사용
                                CData.Parts[(int)EPart.ONP].dDfMin  = CData.Parts[(int)EPart.INR].dDfMin;
                                CData.Parts[(int)EPart.ONP].dDfMax  = CData.Parts[(int)EPart.INR].dDfMax;
                                CData.Parts[(int)EPart.ONP].dDfAvg  = CData.Parts[(int)EPart.INR].dDfAvg;
                                CData.Parts[(int)EPart.ONP].sBcrUse = CData.Parts[(int)EPart.INR].sBcrUse;
                            }
                            else
                            {//DF 서버 사용시 DF 측정 안함
                                CData.Parts[(int)EPart.ONP].dDfMax = CData.Parts[(int)EPart.INR].dDfMax;
                            }
                        }

                        CData.Parts[(int)EPart.ONP].sLotName    = CData.Parts[(int)EPart.INR].sLotName;
                        CData.Parts[(int)EPart.ONP].iMGZ_No     = CData.Parts[(int)EPart.INR].iMGZ_No ;
                        CData.Parts[(int)EPart.ONP].iSlot_No    = CData.Parts[(int)EPart.INR].iSlot_No;
                        CData.Parts[(int)EPart.ONP].sBcr        = CData.Parts[(int)EPart.INR].sBcr    ;
                        CData.Parts[(int)EPart.INR].sBcr        = ""; //190823 ksg :
                        CData.Parts[(int)EPart.ONP].bChkGrd     = false; //200624 pjh : Grinding 중 Error Check 변수
                        
                        CData.Parts[(int)EPart.ONP].LotColor    = CData.Parts[(int)EPart.INR].LotColor;
                        CData.Parts[(int)EPart.ONP].nLoadMgzSN  = CData.Parts[(int)EPart.INR].nLoadMgzSN;

                        // 2022.05.04 SungTae Start : [추가]
                        if (CData.CurCompany == ECompany.ASE_K12)
                            CData.Parts[(int)EPart.ONP].sMGZ_ID = CData.Parts[(int)EPart.INR].sMGZ_ID;
                        // 2022.05.04 SungTae End

                        // 2022.05.04 SungTae : [확인용]
                        _SetLog($"LoadMgzSN : INR({CData.Parts[(int)EPart.INR].nLoadMgzSN}) -> ONP({CData.Parts[(int)EPart.ONP].nLoadMgzSN})");

                        // 200314-mjy : 패키지 유닛일 경우 유닛 데이터 복사
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            Array.Copy(CData.Parts[(int)EPart.INR].aUnitEx, CData.Parts[(int)EPart.ONP].aUnitEx, CData.Dev.iUnitCnt);
                        }

                        //ksg 자재 넘길때 자재 유무만 클리어
                        CData.Parts[(int)EPart.ONP].bExistStrip = CData.Parts[(int)EPart.INR].bExistStrip;          // 자재 유무도 이동
                        CData.Parts[(int)EPart.INR].bExistStrip = false;                                            // 이제 자재 없음

                        // 2021-05-25, jhLee, 사용을 마친 원본 위치의 데이터를 초기화시켜준다.
                        CData.ClearPartData((int)EPart.INR);

                        // 각 UNIT의 상태 변경
                        CData.Parts[(int)EPart.ONP].iStat    = ESeq.ONP_WaitPlace;

                        if(CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd) CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WorkEnd;
                        else                                                      CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;

                        CSq_Inr.It.Act_ZLiftDU(false);

                        //20190925
                        m_bPickOK = false;

#if true //20200417 jhc : Picker Idle Time 개선 - 자재 Pickup 후 On-Picker 대기 위치 L-Table 옆 (On-Picker PickUp 대기위치, Place 대기위치 공용)
                        if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        //if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) &&
                        //    (CData.Dev.bDual == eDual.Dual) &&
                        //    (CSQ_Main.It.m_iStat == EStatus.Auto_Running))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL); //X축 Place 대기 위치로 이동
                            _SetLog("X axis move wait pick left.", CData.SPos.dONP_X_WaitPickL);

                            m_iStep++;
                            return false;
                        }
                        else
                        {
                            _SetLog("Change status.  Status : " + CData.Parts[(int)EPart.ONP].iStat);
                            m_tTackTime = DateTime.Now - m_tStTime;
                            _SetLog("Finish.  Tack time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
#else
                        m_tTackTime = DateTime.Now - m_tStTime;
                        m_LogVal.sMsg = "PickRail Tack Time = " + m_tTackTime.ToString(@"hh\:mm\:ss"); ;
                        SaveLog();

                        m_iStep = 0;
                        return true;
#endif
                    }

#if true //20200417 jhc : Dry Place 후 WORK_END 시 -> Wait Pos. 이동 시퀀스 추가 (Auto-Running && Step Mode에서만)
                case 26:
                    {//X축 Place 대기 위치로 이동 확인, 피커 0도 회전
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);
                            return false;
                        }

                        Func_Picker0(); //피커 0도 회전
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }
                case 27:
                    {//피커 0도 회전 확인
                        if (!Func_Picker0())
                        { return false; }

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Tack time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
#endif
            }
        }

        /// <summary>
        /// Pick Left Table Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PickTable()
        {
            //211022 syc : Onp Pick Up Offset
            double dPickPos = 0;
            if (CDataOption.UseOnpPickUpOffset) { dPickPos = CData.Dev.dOnP_Z_Place - CData.Dev.dOnp_Z_PickOffset; }
            else                                { dPickPos = CData.Dev.dOnP_Z_Place; }

            //Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    // 2020.11.24 JSKim St
                    if (CData.CurCompany    == ECompany.JCET                         && 
                        CSQ_Main.It.m_iStat == EStatus.Auto_Running && m_iStep == 13 && 
                        CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                    {
                        CErr.Show(eErr.LEFT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                        _SetLog("Error : GDL Y axis not wait position.");
                    }
                    // 2020.11.24 JSKim Ed
                    else  // 기존 코드
                    {
                        CErr.Show(eErr.ONLOADERPICKER_PICKTABLELEFT_TIMEOUT);
                        _SetLog("Error : Timeout.");
                    }

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

                case 10: // 피커 버큠 체크
                    {
                        // Tack time 측정 시작
                        m_tStTime = DateTime.Now;
                        
                        if (CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error : Picker detect strip.");
                            
                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Chek axes.");

                        m_iStep++;
                        return false;
                    }

                case 11: // 좌측 테이블 자재 유무 체크 (버큠, 워터)
                    {
                        //210901 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            if (!(CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U)  || CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U)) || 
                                  CIO.It.Get_X(eX.GRDL_TbFlow)          || CIO.It.Get_Y(eY.GRDL_TbFlow))
                            {
                                CErr.Show(eErr.ONLOADERPICKER_NO_STRIP_PICK_UP);
                                _SetLog("Error : Table not strip.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        //
                        else
                        {
                            //190314 ksg :
                            if (!CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDL_TbFlow) || CIO.It.Get_Y(eY.GRDL_TbFlow))
                            {
                                CErr.Show(eErr.ONLOADERPICKER_NO_STRIP_PICK_UP);
                                _SetLog("Error : Table not strip.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                                                
                        _SetLog("Left table strip check.");

                        m_iStep++;
                        return false;
                    }

                case 12: // 좌측 테이블 유닛의 데이터와 매칭 체크
                    {
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bX1 = CData.Parts[(int)EPart.GRDL].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 0 : 0];
                            bool bX2 = CData.Parts[(int)EPart.GRDL].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 1 : 0];
                            bool bX3 = CData.Parts[(int)EPart.GRDL].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 2 : 1];
                            bool bX4 = CData.Parts[(int)EPart.GRDL].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 3 : 1];

                            if (CIO.It.Get_X(eX.GRDL_Unit_1_Vacuum) != bX1 || CIO.It.Get_X(eX.GRDL_Unit_2_Vacuum) != bX2 ||
                                CIO.It.Get_X(eX.GRDL_Unit_3_Vacuum) != bX3 || CIO.It.Get_X(eX.GRDL_Unit_4_Vacuum) != bX4)
                            {
                                CErr.Show(eErr.LEFT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR);
                                _SetLog("Error : Left table unit vacuum not match.");

                                m_iStep = 0;
                                return true;
                            }

                            _SetLog("Left table unit matching check.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 13: // 좌측 테이블 대기 위치 확인
                    {
                        // 2020.11.24 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            if (CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                            {
                                return false;
                            }
                        }
                        else
                        {
                        // 2020.11.24 JSKim Ed
                            if (CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                            {
                                CErr.Show(eErr.LEFT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                                _SetLog("Error : GDL Y axis not wait position.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        _SetLog("GDL Y axis wait position check");

                        m_iStep++;
                        return false;
                    }

                case 14: // 피커 버큠, 이젝트, 드레인 오프
                    {
                        Func_Vacuum(false);
                        Func_Eject(false);
                        Func_Drain(false);
                        _SetLog("On-Picker Vacuum off.  Eject off.  Drain off.");

                        m_iStep++;
                        return false;
                    }

                case 15: // Z축 대기위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 16: // Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        _SetLog("Z axis move wait check.");

                        m_iStep++;
                        return false;
                    }

                case 17: // X축 좌측 테이블 위치 이동, 피커 0도 (동시동작)
                    {
                        CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_PlaceL);
                        Func_Picker0();
                        _SetLog("Picker 0.  X axis move left place.", CData.SPos.dONP_X_PlaceL);

                        m_iStep++;
                        return false;
                    }

                case 18: // 피커 0도 확인
                    {
                        if (!Func_Picker0())
                        { return false; }

                        _SetLog("Picker 0 check.");

                        m_iStep++;
                        return false;
                    }

                case 19: // X축 좌측 테이블 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_PlaceL))
                        { return false; }

                        _SetLog("X axis move left place check.");

                        m_iStep++;
                        return false;
                    }

                case 20: // 피커 버큠 체크
                    {
                        if (CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_ALREADY_DETECT_STRIP_ERROR);
                            _SetLog("Error : Picker already strip.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Picker vacuum check.");

                        m_iStep++;
                        return false;
                    }

                case 21: // 피커 이젝트 온
                    {
                        Func_Eject(true);
                        _SetLog("Eject on.");

                        m_iStep++;
                        return false;
                    }

                case 22: // Z축 슬로우 시작 위치 이동 (Pick위치 - 슬로우 동작 거리) 
                    {
                        //211022 syc : Onp Pick Up Offset
                        // Z축 테이블 place 포지션과 pick 포지션은 같다
                        //double dPos = CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow;
                        double dPos = dPickPos - CData.Dev.dOnP_Z_Slow;
                        // syc end

                        CMot.It.Mv_N(m_iZ, dPos);
                        _SetLog("Z axis move position.", dPos);

                        m_iStep++;
                        return false;
                    }

                case 23: // Z축 슬로우 시작 위치 이동 확인 (Pick위치 - 슬로우 동작 거리) 
                    {
                        //211022 syc : Onp Pick Up Offset
                        //if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow)))
                        //{ return false; }
                        if (!CMot.It.Get_Mv(m_iZ, (dPickPos - CData.Dev.dOnP_Z_Slow)))
                        { return false; }
                        // syc end

                        _SetLog("Z axis move position check.");

                        m_iStep++;
                        return false;
                    }

                case 24: // 이젝트 오프
                    {
                        Func_Eject(false);
                        _SetLog("Eject off.");

                        m_iStep++;
                        return false;
                    }

                case 25: // Z축 테이블 위치 이동 (저속)
                    {
                        //211022 syc : Onp Pick Up Offset
                        //CMot.It.Mv_S(m_iZ, CData.Dev.dOnP_Z_Place);
                        //_SetLog("Z axis move pick(slow).", CData.Dev.dOnP_Z_Place);

                        CMot.It.Mv_S(m_iZ, dPickPos);
                        _SetLog("Z axis move pick(slow).", dPickPos);
                        // syc end

                        m_iStep++;
                        return false;
                    }

                case 26: // Z축 pick 위치 이동 확인
                    {
                        //211022 syc : Onp Pick Up Offset
                        //if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Place))
                        if (!CMot.It.Get_Mv(m_iZ, dPickPos))
                        { return false; }
                        //syc end

                        _SetLog("Z axis move pick check.");

                        m_iStep++;
                        return false;
                    }

                case 27: // 피커 이젝트 오프, 버큠 온
                    {
                        Func_Eject(false);
                        Func_Vacuum(true);
                        _SetLog("On-Picker : Eject off,  Vacuum on.");

                        m_iStep++;
                        return false;
                    }

                case 28: // 좌측 테이블 버큠 오프
                    {
                        CData.L_GRD.ActVacuum(false);
                        _SetLog("Left table vacuum off.");

                        m_iStep++;
                        return false;
                    }

                case 29: // 좌측 테이블 이젝트 온
                    {
                        CData.L_GRD.ActEject(true);
                        _SetLog("Left table eject on.");

                        m_iStep++;
                        return false;
                    }

                case 30: // 좌측 테이블 이젝트 딜레이 설정
                    {
                        m_Delay.Set_Delay(CDataOption.EjtDelay);
                        _SetLog(string.Format("Set eject delay : {0}ms", CDataOption.EjtDelay));

                        m_iStep++; 
                        return false;
                    }

                case 31: // 좌측 테이블 이젝트 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                case 32: // 좌측 테이블 이젝트 오프, 워터 온
                    {
                        CData.L_GRD.ActEject(false);    _SetLog("GDL Eject Off.");      // 로그 수정
                        
                        if (CData.Dev.bMeasureMode) {   CData.L_GRD.ActWater(false);    _SetLog("GDL Water Off.");  }
                        else                        {   CData.L_GRD.ActWater(true);     _SetLog("GDL Water On.");   }

                        m_iStep++;
                        return false;
                    }

                case 33: // 좌측 테이블 워터 딜레이 설정
                    {
                        m_Delay.Set_Delay(CDataOption.WtrDelay);
                        _SetLog(string.Format("Set water delay : {0}ms", CDataOption.WtrDelay));

                        m_iStep++;
                        return false;
                    }

                case 34: // 좌측 테이블 워터 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                case 35: // 피커 버큠 확인
                    {
                        if (!CIO.It.Get_X(eX.ONP_Vacuum)) 
                        {                           
                            _SetLog("On-Picker Vacuum fail");
                            m_iStep = 90;

                            // 2022.08.18 lhs : Vacuum Error 발생하여 에러 조치하기 위해 Wait 버튼을 클릭하면 ONP_Pick 으로 바뀜
                            // 다시 PickTable 하기 위한 변수
                            m_bRetryPickTbL = true;
                        }
                        else
                        {
                            // 200330 mjy : Gap delay 추가
                            m_Delay.Set_Delay(CData.Opt.iPickDelay);
                            _SetLog(string.Format("Set delay {0}ms", CData.Opt.iPickDelay));

                            m_iStep++;
                        }
                        return false;
                    }

                case 36: // Gap delay 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                case 37: // 테이블 워터 오프
                    {
                        //200429 jym : 메뉴얼 동작에서 테이블 워터 오프
                        if (!CDataOption.IsTblWater || CSQ_Main.It.m_iStat == EStatus.Manual)
                        {
                            CData.L_GRD.ActWater(false);
                            _SetLog("GDL water off.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 38: // Z축 슬로우 시작 위치 이동 (Place위치 - Gap 동작 거리) 
                    {
                        // 200330 mjy : Gap 추가
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Opt.dPickGap));

                        _SetLog(string.Format("Z axis move slow pick start position    Pos:{0}mm", CData.Dev.dOnP_Z_Place - CData.Opt.dPickGap));

                        m_iStep++;
                        return false;
                    }

                case 39: // Z축 슬로우 시작 위치 이동 확인 (Place위치 - Gap 동작 거리) 
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Place - CData.Opt.dPickGap))
                        { return false; }

                        _SetLog("Z axis move slow pick start position check");

                        m_iStep++;
                        return false;
                    }

                case 40: // Pick 딜레이 설정
                    {
                        m_Delay.Set_Delay(CData.Dev.iPickDelayOn);

                        _SetLog(string.Format("Vacuum success.  Set delay : {0}ms", CData.Dev.iPickDelayOn));

                        m_iStep++;
                        return false;
                    }

                case 41: // Pick 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay");

                        m_iStep++;
                        return false;
                    }

                case 42: // Z축 슬로우 시작 위치 이동 (Place위치 - 슬로우 동작 거리) 
                    {
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow));

                        _SetLog(string.Format("Z axis move slow pick start position    Pos:{0}mm", CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 43: // Z축 슬로우 시작 위치 이동 확인 (Place위치 - 슬로우 동작 거리) 
                    {
                        if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow)))
                        { return false; }

                        _SetLog("Z axis move slow pick start position check");

                        m_iStep++;
                        return false;
                    }

                case 44: // 피커 버큠 상태 확인   // 2022.08.19 lhs : Wait 위치 이전에 버큠 체크
                    {
                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            _SetLog("Picker vacuum not enable");
                            m_iStep = 90;

                            // 2022.08.18 lhs : Vacuum Error 발생하여 에러 조치하기 위해 Wait 버튼을 클릭하면 ONP_Pick 으로 바뀜
                            // Cyl_PickTable 재진입하기 위한 변수
                            m_bRetryPickTbL = true;
                        }
                        else
                        {
                            _SetLog("Picker vacuum check");
                            m_iStep++;
                        }
                        return false;
                    }

                case 45: // Z축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);

                        _SetLog(string.Format("Z axis move wait position    Pos:{0}mm", CData.SPos.dONP_Z_Wait));

                        m_iStep++;
                        return false;
                    }

                case 46: // Z축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        _SetLog("Z axis move wait position check");

                        m_iStep++;
                        return false;
                    }

                case 47: // 데이터 전달 전 최종 피커 버큠 상태 확인 // 2022.08.19 lhs  
                    {
                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                            _SetLog("Error : On-Picker Vacuum fail.");

                            // 2022.08.18 lhs : Vacuum Error 발생하여 에러 조치하기 위해 Wait 버튼을 클릭하면 ONP_Pick 으로 바뀜
                            // Cyl_PickTable 재진입하기 위한 변수
                            m_bRetryPickTbL = true;
                            
                            m_iStep = 0;
                            return true;
                        }
                        else
                        {
                            _SetLog("Picker vacuum last check");
                            m_iStep++;
                        }
                        return false;
                    }

                case 48: // 데이터 전달, 상태 변환
                    {
                        Array.Copy(CData.Parts[(int)EPart.GRDL].dPcb, CData.Parts[(int)EPart.ONP].dPcb, CData.Parts[(int)EPart.GRDL].dPcb.Length);

                        //-------------------------
                        // 2022.01.25 lhs Start : 2004U, Dumy 데이터 전달
                        if (CDataOption.Use2004U)
                        {
                            if (CData.Dev.bDual == eDual.Dual)
                            {
                                CData.Parts[(int)EPart.ONP].bCarrierWithDummy  = CData.Parts[(int)EPart.GRDL].bCarrierWithDummy; // LT, Dual -> OnPicker
                                CData.Parts[(int)EPart.GRDL].bCarrierWithDummy = false;  // LT, Dual : 데이터 전달 후 Flag 초기화

                                Array.Copy(CData.Dev.aData[0].bDummy, CData.Dev.bCopyDummy, CData.Dev.bCopyDummy.Length); // 데이터 전달 위해 Copy : LT -> OnPicker
                            }
                            else  // Normal
                            {
                                // Normal일 경우는 OffPicker에서 LT Pick
                            }
                        }
                        // 2022.01.25 lhs End : 2004U, Dumy 데이터 전달
                        //-------------------------

                        if ((CData.Opt.bSecsUse == true) || (CDataOption.MeasureDf == eDfServerType.MeasureDf))
                        {   // SECSGEM 사용시 Host에서 Down 받은 Data 사용 20200315 LCY
                            // 20200507 (dDf -> dPcb) 잘 못 적용 된 값 전달 부 SCK현장 수정 PJH
                            CData.Parts[(int)EPart.ONP].dPcbMin  = CData.Parts[(int)EPart.GRDL].dPcbMin;
                            CData.Parts[(int)EPart.ONP].dPcbMax  = CData.Parts[(int)EPart.GRDL].dPcbMax;
                            CData.Parts[(int)EPart.ONP].dPcbMean = CData.Parts[(int)EPart.GRDL].dPcbMean;

                            // 2021.08.03 lhs Start : Top/Btm Mold 데이터 전달
                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.Parts[(int)EPart.ONP].dTopMoldMax = CData.Parts[(int)EPart.GRDL].dTopMoldMax;
                                CData.Parts[(int)EPart.ONP].dTopMoldAvg = CData.Parts[(int)EPart.GRDL].dTopMoldAvg;
                                CData.Parts[(int)EPart.ONP].dBtmMoldMax = CData.Parts[(int)EPart.GRDL].dBtmMoldMax;
                                CData.Parts[(int)EPart.ONP].dBtmMoldAvg = CData.Parts[(int)EPart.GRDL].dBtmMoldAvg;
                            }
                            // 2021.08.03 lhs End

                            // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                            //if (CData.GemForm != null) CData.GemForm.Strip_Data_Shift(10, 11);// Left Table Data -> Picker 로 이동
                            if (CData.GemForm != null)
                            {
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.GRL_AF_MEAS/*10*/, (int)EDataShift.OFP_LT_PICK/*11*/);// Left Table Data -> Picker 로 이동
                            }
                            // 2021.07.19 SungTae End
                        }
                        else if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                            {//DF 서버 사용시 DF 측정 사용
                                CData.Parts[(int)EPart.ONP].dDfMin = CData.Parts[(int)EPart.GRDL].dDfMin;
                                CData.Parts[(int)EPart.ONP].dDfMax = CData.Parts[(int)EPart.GRDL].dDfMax;
                                CData.Parts[(int)EPart.ONP].dDfAvg = CData.Parts[(int)EPart.GRDL].dDfAvg;
                                CData.Parts[(int)EPart.ONP].sBcrUse = CData.Parts[(int)EPart.GRDL].sBcrUse;
                            }
                            else
                            {//DF 서버 사용시 DF 측정 안함
                                CData.Parts[(int)EPart.ONP].dDfMax = CData.Parts[(int)EPart.GRDL].dDfMax;
                            }
                        }

                        CData.Parts[(int)EPart.ONP].sLotName = CData.Parts[(int)EPart.GRDL].sLotName;
                        CData.Parts[(int)EPart.ONP].iMGZ_No  = CData.Parts[(int)EPart.GRDL].iMGZ_No;
                        CData.Parts[(int)EPart.ONP].iSlot_No = CData.Parts[(int)EPart.GRDL].iSlot_No;
                        CData.Parts[(int)EPart.ONP].sBcr     = CData.Parts[(int)EPart.GRDL].sBcr;
                        CData.Parts[(int)EPart.ONP].dShiftT  = CData.Parts[(int)EPart.GRDL].dShiftT; //190529 ksg :
                        CData.Parts[(int)EPart.GRDL].sBcr    = ""; //190823 ksg :
                        CData.Parts[(int)EPart.ONP].bChkGrd  = false; //200624 pjh : Grinding 중 Error Check 변수
                        CData.Parts[(int)EPart.ONP].LotColor = CData.Parts[(int)EPart.GRDL].LotColor;   // LOT 색
                        CData.Parts[(int)EPart.ONP].nLoadMgzSN = CData.Parts[(int)EPart.GRDL].nLoadMgzSN;

                        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        // Data Shift : L-Table => On-Picker (18 Point 측정 여/부)
                        CData.Parts[(int)EPart.ONP].b18PMeasure = CData.Parts[(int)EPart.GRDL].b18PMeasure; //Data Shift는 조건별 사용 여부에 무관하게 진행 함
                        CData.Parts[(int)EPart.GRDL].b18PMeasure = false;
                        //

                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_PlaceTbR;
                        //20190827 ghk_tableclean
                        CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_Table_Clean;

                        // 200314 mjy : 패키지 유닛일 경우 유닛 데이터 복사
                        if (CDataOption.Package == ePkg.Unit)
                        { Array.Copy(CData.Parts[(int)EPart.GRDL].aUnitEx, CData.Parts[(int)EPart.ONP].aUnitEx, CData.Dev.iUnitCnt); }

                        //ksg Strip 유무 변경
                        CData.Parts[(int)EPart.ONP].bExistStrip = CData.Parts[(int)EPart.GRDL].bExistStrip;
                        CData.Parts[(int)EPart.GRDL].bExistStrip = false;
                        // 2020.12.12 JSKim St
                        CData.Parts[(int)EPart.GRDL].iStripStat = 0;
                        // 2020.12.12 JSKim Ed

                        if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                            CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd)
                        {
                            CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_WorkEnd;
                            CData.L_GRD.LastTbClean = true;

                            _SetLog(">>>>> GRL Work End, CData.L_GRD.LastTbClean = true");  // 2021.11.18 lhs 
                        }

                        _SetLog("Status change    Status:" + CData.Parts[(int)EPart.ONP].iStat.ToString());                       

                        m_iStep++;
                        return false;
                    }

                case 49: // 종료, 택 타임 계산
                    {
                        m_tTackTime = DateTime.Now - m_tStTime;

                        _SetLog(string.Format("Finish    Tack time:{0}", m_tTackTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }

                case 90: // 피커 버큠 오프, 이젝트 온 (피커 버큠 실패 시 에러 처리 시작)
                    {
                        Func_Vacuum(false);
                        CData.L_GRD.ActWater(false); //200403 ksg : Error시 물끔
                        Func_Eject(true);

                        _SetLog("Picker vacuum off, eject on");

                        m_iStep++;
                        return false;
                    }

                case 91: // 테이블 자재 흡착
                    {
                        CData.L_GRD.ActVacuum(true);

                        _SetLog("Left table vacuum on");

                        m_iStep++;
                        return false;
                    }

                case 92: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(m_iErrDelay);

                        _SetLog(string.Format("Set delay {0}ms", m_iErrDelay));

                        m_iStep++;
                        return false;
                    }

                case 93: // 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay");

                        m_iStep++;
                        return false;
                    }                

                case 94: // Z축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);

                        _SetLog(string.Format("Z axis move wait position    Pos:{0}mm", CData.SPos.dONP_Z_Wait));

                        m_iStep++;
                        return false;
                    }

                case 95: // Z축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        _SetLog("Z axis move wait position check");

                        m_iStep++;
                        return false;
                    }

                case 96: // 피커 이젝트 오프, 알람 발생
                    {
                        Func_Eject(false);

                        CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                        _SetLog("Error : On-Picker Vacuum fail.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Place Left or Right Table Cycle
        /// </summary>
        /// <param name="eWay"></param>
        /// <returns></returns>
        //public bool Cyl_Palce(EWay eWay)
        public bool Cyl_Place(EWay eWay)        // 2021.04.28 SungTae : 오타로 함수명 변경(Cyl_Palce -> Cyl_Place)
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    // 2020.11.24 JSKim St
                    if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running && m_iStep == 15)
                    {
                        if (eWay == EWay.L && CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                        {
                            CErr.Show(eErr.LEFT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                            _SetLog("Error : GDL Y axis not wait.");
                        }
                        else if (eWay == EWay.R && CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                        {
                            CErr.Show(eErr.RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                            _SetLog("Error : GDR Y axis not wait.");
                        }
                        else
                        {
                            CErr.Show(eErr.ONLOADERPICKER_PLACE_TIMEOUT);
                            _SetLog("Error : Timeout.");
                        }
                    }
                    else
                    {
                    // 2020.11.24 JSKim Ed
                        CErr.Show(eErr.ONLOADERPICKER_PLACE_TIMEOUT);
                        _SetLog("Error : Timeout.");
                    // 2020.11.24 JSKim St
                    }
                    // 2020.11.24 JSKim Ed

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
                        return true; //20200417 jhc :
                    }

                case 10:
                    {//버큠 온, 이젝트 오프, 워터 드레인 오프, z축 대기위치 이동
                        m_tStTime = DateTime.Now;
                        //190717 ksg :
                        if (CSQ_Main.It.m_iStat == EStatus.Manual)
                        {
                            double dPos = (eWay == EWay.L) ? CData.SPos.dOFP_X_PickR + 10 : CData.Dev.dOffP_X_ClnStart + 5;

                            if (CMot.It.Get_FP(m_iOfpX) > dPos)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_X_AXIS_NOT_WIAT_POSITION);
                                _SetLog("Error : X axis not wait position.", dPos);

                                m_iStep = 0;
                                return true;
                            }
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }
                        
                        Func_Vacuum(true);
                        Func_Eject(false);
                        Func_Drain(false);

                        _SetLog("Check axes.  Way : " + eWay);

                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_NOT_DETECT_STRIP_ERROR);
                            _SetLog("Error : Picker not detect strip.");

                            m_iStep = 0;
                            return true;
                        }
                        //190822 ksg :
                        if (eWay == EWay.L)
                        {//Left Table 자재 확인
                            //20191216 ghk_onp_place_debug
                            //210901 syc : 2004U
                            if (CDataOption.Use2004U)
                            {
                                if (CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U) && CData.Parts[(int)EPart.GRDL].bExistStrip)
                                {
                                    CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_LEFT);
                                    _SetLog("Error : Left table unknown Unit.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            //
                            else
                            {
                                if (CIO.It.Get_X(eX.GRDL_TbVaccum) || (!CIO.It.Get_X(eX.GRDL_TbVaccum) && CData.Parts[(int)EPart.GRDL].bExistStrip))
                                {
                                    CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_LEFT);
                                    _SetLog("Error : Left table unknown strip.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }      
                        }
                        else
                        {//Right Table 자재 확인
                            //20191216 ghk_onp_place_debug

                            if (CDataOption.Use2004U)
                            {
                                if (CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U) && CData.Parts[(int)EPart.GRDR].bExistStrip)
                                {
                                    CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_RIGHT);
                                    _SetLog("Error : Right table unknown Unit.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            else
                            {
                                if (CIO.It.Get_X(eX.GRDR_TbVaccum) || (!CIO.It.Get_X(eX.GRDR_TbVaccum) && CData.Parts[(int)EPart.GRDR].bExistStrip))
                                {
                                    CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_RIGHT);
                                    _SetLog("Error : Right table unknown strip.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }                        
                        }
                        //200128 ksg :
                        if(eWay == EWay.L)
                        {
                            if(!CIO.It.Get_Y(eY.PUMPL_Run)) CIO.It.Set_Y(eY.PUMPL_Run, true);
                        }
                        else
                        {
                            if(!CIO.It.Get_Y(eY.PUMPR_Run)) CIO.It.Set_Y(eY.PUMPR_Run, true);
                        }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait); //상단 이동
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Z축 대기위치 이동 확인, X축 Place위치 이동, 피커 0도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))  { return false; }

                        if (eWay == EWay.L)
                        { 
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_PlaceL);
                            _SetLog("X axis move place left.", CData.SPos.dONP_X_PlaceL);

                            //190430 ksg : Eject On 후 동작 시 Off 하는 곳이 없어 수정
                            if(CIO.It.Get_Y(eY.GRDL_TbEjector)) CIO.It.Set_Y(eY.GRDL_TbEjector, false);
                        } 
                        else
                        { 
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_PlaceR);
                            _SetLog("X axis move place right.", CData.SPos.dONP_X_PlaceR);
                            //190430 ksg : Eject On 후 동작 시 Off 하는 곳이 없어 수정
                            if (CIO.It.Get_Y(eY.GRDR_TbEjector)) CIO.It.Set_Y(eY.GRDR_TbEjector, false);
                        } 

                        Func_Picker0();

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//X축 Place위치 이동 확인, 피커 0도 확인
                        if (eWay == EWay.L)
                        {//Left Table로 X축 이동
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_PlaceL))
                            { return false; }
                        }
                        else
                        {//Right Table로 X축 이동
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_PlaceR))
                            { return false; }
                        }

                        if (!Func_Picker0())
                        { return false; }

                        _SetLog("X axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//테이블 워터 온, 테이블 버큠 오프
                        if (eWay == EWay.L)
                        {
                            CData.L_GRD.ActVacuum(false);
                            //200407 jym : 테이블 워터 온 함수사용
                            // 2020.10.21 JSKim St - 측정 모드일 때는 테이블 워터 미사용
                            //CData.L_GRD.ActWater(true);
                            if (CData.Dev.bMeasureMode) {   CData.L_GRD.ActWater(false);    }
                            else                        {   CData.L_GRD.ActWater(true);     }
                            // 2020.10.21 JSKim Ed
                        }
                        else
                        {
                            CData.R_GRD.ActVacuum(false);
                            //200407 jym : 테이블 워터 온 함수사용
                            // 2020.10.21 JSKim St - 측정 모드일 때는 테이블 워터 미사용 
                            //CData.R_GRD.ActWater(true);
                            if (CData.Dev.bMeasureMode) {   CData.R_GRD.ActWater(false);    }
                            else                        {   CData.R_GRD.ActWater(true);     }
                            // 2020.10.21 JSKim Ed
                        }

                        _SetLog("Table vacuum off.  Table water on.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//피커 0도
                        Func_Picker0();
                        _SetLog("Picker 0");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//피커 0도 확인, 버큠 온 확인, 그라인더 Y축 대기 위치 확인
                        if (!Func_Picker0())
                        { return false; }

                        if (!CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_VACUUM_ERROR);
                            _SetLog("Error : Vacuum off.");

                            Func_Vacuum(false);

                            m_iStep = 0;
                            return true;
                        }

                        // 2020.11.24 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            if (eWay == EWay.L)
                            {
                                if (CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                                {
                                     return true;
                                }
                            }
                        }
                        else
                        {
                        // 2020.11.24 JSKim Ed
                            if (eWay == EWay.L)
                            {
                                if (CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                                {
                                    CErr.Show(eErr.LEFT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                                    _SetLog("Error : GDL Y axis not wait.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            else
                            {
                                if (CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                                {
                                    CErr.Show(eErr.RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION); //190801 ksg :
                                    _SetLog("Error : GDR Y axis not wait.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                        // 2020.11.24 JSKim St
                        }
                        // 2020.11.24 JSKim Ed

                        _SetLog("Check table position.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//Z축 place - slow 위치 이동(슬로우)
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow));
                        _SetLog("Z axis move position.", (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 17:    //Z축 Place - Slow 위치 이동 확인, 딜레이 세팅
                    bool bJSCK_Warp_Test_Mode = false;// SCK+ Warp 관련 요청 사항 20200201 LCY
                    {
                        if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow)))
                        {
                            // 2021.11.03 SungTae Start : [수정] 충돌 Issue 관련 조건 추가
                            // Left or Right Stage Y-Axis가 Wait Position을 벗어났거나 Sequence Status가 Dressing 이면
                            if (eWay == EWay.L)
                            {
                                if (CMot.It.Get_FP(m_iLTY) > CData.SPos.dGRD_Y_Wait[(int)EWay.L] + 0.1 ||
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Dressing)
                                {
                                    CMot.It.Stop(m_iZ);     // ONP Z-Axis Down Stop

                                    CErr.Show(eErr.LEFT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                                    _SetLog("Error : GDL Y axis not wait.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            else
                            {
                                if (CMot.It.Get_FP(m_iRTY) > CData.SPos.dGRD_Y_Wait[(int)EWay.R] + 0.1 ||
                                    CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Dressing)
                                {
                                    CMot.It.Stop(m_iZ);     // ONP Z-Axis Down Stop

                                    CErr.Show(eErr.RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                                    _SetLog("Error : GDR Y axis not wait.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            // 2021.11.03 SungTae End

                            return false;
                        }

                        if (bJSCK_Warp_Test_Mode == true)
                        {
                            Func_Vacuum(false);
                            Func_Eject(true);

                            CLog.mLogGnd(string.Format(" CSQ3_OnP 17 -> bJSCK_Warp_Test_Mode True"));
                        }

                        int iTime = CData.Opt.aOnpRinseTime[(int)eWay];
                        iTime *= 1000;
                        m_Place.Set_Delay(iTime);
                        _SetLog("Set delay : " + iTime + "ms"); 

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//딜레이 확인, 테이블 워터 오프, Z축 Place - Gap 위치 이동
                        if (!m_Place.Chk_Delay())
                        { return false; }

                        //200407 jym : 오른쪽에 내려놓을때 듀얼모드면 갭 시퀀스 사용 안함
                        if (eWay == EWay.R && CData.Dev.bDual == eDual.Dual)
                        { m_dTempGap = 0; }
                        else
                        { m_dTempGap = CData.Opt.dPlaceGap; }

                        //200330 mjy : Gap 시퀀스 추가
                        CMot.It.Mv_S(m_iZ, CData.Dev.dOnP_Z_Place - m_dTempGap); //Z축 Place 
                        _SetLog("Z axis move position(slow).", CData.Dev.dOnP_Z_Place - m_dTempGap);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//Z축 Place 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Place - m_dTempGap))
                        { return false; }

                        //200407 jym : 오른쪽에 내려놓을때 듀얼모드면 딜레이 시퀀스 사용 안함
                        if (eWay == EWay.R && CData.Dev.bDual == eDual.Dual)
                        {
                            m_Place.Set_Delay(0);
                            _SetLog("Set delay : 0ms");
                        }
                        else
                        {
                            m_Place.Set_Delay(CData.Opt.iPlaceDelay);
                            _SetLog("Set delay : " + CData.Opt.iPlaceDelay + "ms");
                        }

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//딜레이 확인, 테이블 워터 오프, Z축 Place 위치 이동
                        if (!m_Place.Chk_Delay())   { return false; }

                        CMot.It.Mv_S(m_iZ, CData.Dev.dOnP_Z_Place); //Z축 Place 
                        _SetLog("Z axis move place(slow).", CData.Dev.dOnP_Z_Place);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//Z축 Place 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Place))  { return false; }

                        if (eWay == EWay.L) { CData.L_GRD.ActWater(false); }    //Y32 Left Table Water
                        else                { CData.R_GRD.ActWater(false); }    //Y4A Right Table Water

                        _SetLog("Table water off.");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//버큠 오프, 이젝트 온, 테이블 버큠 온, 펌프 런
                        Func_Vacuum(false);
                        Func_Eject(true);

                        CIO.It.Set_Y((eWay == EWay.L) ? eY.PUMPL_Run : eY.PUMPR_Run, true);//Y70 Left Table Pump 

                        //210902 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_Unit_Vacuum_4U    : eY.GRDR_Unit_Vacuum_4U,    true);
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_Carrier_Vacuum_4U : eY.GRDR_Carrier_Vacuum_4U, true);
                        }
                        else
                        {
                            //200415 jym : 
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_TbVacuum : eY.GRDR_TbVacuum, true);
                        }
                        //

                        if (CDataOption.Package == ePkg.Unit)
                        {
                            // 200707 jym : Unit 갯수 별 조건 변경
                            bool bX1 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[0] : CData.Parts[(int)EPart.ONP].aUnitEx[0];
                            bool bX2 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[1] : CData.Parts[(int)EPart.ONP].aUnitEx[0];
                            bool bX3 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[2] : CData.Parts[(int)EPart.ONP].aUnitEx[1];
                            bool bX4 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[3] : CData.Parts[(int)EPart.ONP].aUnitEx[1];

                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_CarrierVacuum : eY.GRDR_CarrierVacuum, true);
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_Unit_1_Vacuum : eY.GRDR_Unit_1_Vacuum, bX1);
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_Unit_2_Vacuum : eY.GRDR_Unit_2_Vacuum, bX2);
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_Unit_3_Vacuum : eY.GRDR_Unit_3_Vacuum, bX3);
                            CIO.It.Set_Y((eWay == EWay.L) ? eY.GRDL_Unit_4_Vacuum : eY.GRDR_Unit_4_Vacuum, bX4);

                            _SetLog(string.Format("Unit exist check.  #1:{0}  #2:{1}  #3:{2}  #4:{3}", bX1, bX2, bX3, bX4));
                        }

                        _SetLog("Vacuum off.  Table vacuum on.");

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {//이젝트 오프, 딜레이 5초
                        // 2020.12.14 JSKim St - Delay 먹이고 Picker Eject를 끄자.. 
                        // 일부 구형 설비 공압라인 상태가 안좋다... 이 경우 Eject Air 양이 적어서 문제가 있을 수 있다.
                        //Func_Eject(false);
                        if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)
                        {
                            Func_Eject(false);
                        }
                        // 2020.12.14 JSKim Ed

                        //200417 jym : Unit에서는 설정된 딜레이 값 사용
                        if (CDataOption.Package == ePkg.Strip)
                        {
                            // 2020.12.16 JSKim St
                            // SCK+에서도 Picker Place Delay 시간은 옵션으로 처리해달라고 함...
                            if ((CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) && CData.Dev.iPlaceDelayOn != 0)
                            {
                                m_Delay.Set_Delay(CData.Dev.iPlaceDelayOn);
                                _SetLog("Set delay : " + CData.Dev.iPlaceDelayOn + "ms");
                            }
                            else if (CData.CurCompany == ECompany.SkyWorks || CDataOption.Use2004U)  // 2022.08.17 lhs 2004U 추가
                            {
                                m_Delay.Set_Delay(5000);
                                _SetLog("Set delay : 5000ms");
                            }
                            else
                            {
                                m_Delay.Set_Delay(3000);
                                _SetLog("Set delay : 3000ms");
                            }
                            // 2020.12.16 JSKim Ed
                        }
                        else
                        {
                            m_Delay.Set_Delay(CData.Dev.iPlaceDelayOn);
                            _SetLog("Set delay : " + CData.Dev.iPlaceDelayOn + "ms");
                        }

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//테이블 버큠 확인                        
                        if (!m_Delay.Chk_Delay())   { return false; }

                        // 2020.12.14 JSKim St
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            Func_Eject(false);
                        }
                        // 2020.12.14 JSKim Ed

                        // 2020.12.14 JSKim Check
                        // 여기서 자재 유무를 넘겨 주고 자재 Data는 28에서 준다...
                        // Z축 Up 동작을 하면서 자재가 Table로 떨어질 테니 넘겨 주는 것이 맞다
                        // 하지만 왜 나머지 Data는 안넘겨줄까??   => 아래에 나머지 데이터를 모두 넘겨준다.
                        if (eWay == EWay.L)
                        {
                            CData.Parts[(int)EPart.GRDL].bExistStrip    = CData.Parts[(int)EPart.ONP].bExistStrip;
                            CData.Parts[(int)EPart.GRDL].iStripStat     = 0;    // 2020.12.12 JSKim
                            CData.Parts[(int)EPart.ONP ].bExistStrip    = false;
                        }
                        else
                        {
                            CData.Parts[(int)EPart.GRDR].bExistStrip    = CData.Parts[(int)EPart.ONP].bExistStrip;
                            CData.Parts[(int)EPart.GRDR].iStripStat     = 0;    // 2020.12.12 JSKim
                            CData.Parts[(int)EPart.ONP ].bExistStrip    = false;
                        }

                        bError_Safety_Mv = false;

                        //210901 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            if (eWay == EWay.L)
                            {
                                if (!CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || !CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U))
                                {
                                    CErr.Show(eErr.LEFT_GRIND_VACUUM_ERROR);
                                    _SetLog("Error : Left Table Vacuum fail.");
                                }
                            }
                            else
                            {
                                if (!CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || !CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U))
                                {
                                    CErr.Show(eErr.RIGHT_GRIND_VACUUM_ERROR);
                                    _SetLog("Error : Rihgt Table Vacuum fail.");
                                }
                            }
                            m_iStep++;     // 2022.08.17 lhs : 아래부분 수행 안하기 위해 
                            return false;
                        }
                        //
                        else
                        {
                            if (!CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_TbVaccum : eX.GRDR_TbVaccum))
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_VACUUM_ERROR : eErr.RIGHT_GRIND_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                        }                       

                        // Unit 모드일 때 버큠 체크
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bX1 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[0] : CData.Parts[(int)EPart.ONP].aUnitEx[0];
                            bool bX2 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[1] : CData.Parts[(int)EPart.ONP].aUnitEx[0];
                            bool bX3 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[2] : CData.Parts[(int)EPart.ONP].aUnitEx[1];
                            bool bX4 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[3] : CData.Parts[(int)EPart.ONP].aUnitEx[1];
                            
                            _SetLog(string.Format("Unit exist.  #1 : {0}  #2 : {1}  #3 : {2}  #4 : {3}", bX1, bX2, bX3, bX4));

                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_1_Vacuum : eX.GRDR_Unit_1_Vacuum) != bX1)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT1_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT1_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 1 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_2_Vacuum : eX.GRDR_Unit_2_Vacuum) != bX2)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT2_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT2_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 2 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_3_Vacuum : eX.GRDR_Unit_3_Vacuum) != bX3)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT3_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT3_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 3 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_4_Vacuum : eX.GRDR_Unit_4_Vacuum) != bX4)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT4_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT4_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 4 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                        }

                        // 2020.12.14 JSKim St - 이미 자재 유무 데이터가 넘어 갔다
                        if (bError_Safety_Mv)
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                            m_iStep = 27;
                            return false;
                        }
                        // 2020.12.14 JSKim Ed

                        _SetLog("Check vacuum.");

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//Z축 Place - slow 위치 이동(슬로우)
                        CMot.It.Mv_S(m_iZ, (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow));
                        _SetLog("Z axis move position(slow).", (CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {//Z축 Place - slow 위치 이동(슬로우) 확인, Z축 Wait 위치 이동, 워터 드레인 온
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnP_Z_Place - CData.Dev.dOnP_Z_Slow))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        Func_Drain(true);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {//Z축 Wait 위치 이동 확인, 워터 드레인 오프
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))  { return false; }

                        Func_Drain(false);
                        _SetLog("Drain off.");

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {//테이블 버큠 다시 한번 확인, 데이터 변경     
                        bError_Safety_Mv = false;

                        //210901 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            if (eWay == EWay.L)
                            {
                                if (!CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || !CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U))
                                {
                                    CErr.Show(eErr.LEFT_GRIND_VACUUM_ERROR);
                                    _SetLog("Error : Left Table Vacuum fail.");
                                }
                            }
                            else
                            {
                                if (!CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || !CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U))
                                {
                                    CErr.Show(eErr.RIGHT_GRIND_VACUUM_ERROR);
                                    _SetLog("Error : Right Table Vacuum fail.");
                                }
                            }
                        }
                        //
                        else
                        {
                            if (!CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_TbVaccum : eX.GRDR_TbVaccum))
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_VACUUM_ERROR : eErr.RIGHT_GRIND_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                        }

                        // Unit 모드일 때 버큠 체크
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bX1 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[0] : CData.Parts[(int)EPart.ONP].aUnitEx[0];
                            bool bX2 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[1] : CData.Parts[(int)EPart.ONP].aUnitEx[0];
                            bool bX3 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[2] : CData.Parts[(int)EPart.ONP].aUnitEx[1];
                            bool bX4 = (CData.Dev.iUnitCnt == 4) ? CData.Parts[(int)EPart.ONP].aUnitEx[3] : CData.Parts[(int)EPart.ONP].aUnitEx[1];
                            _SetLog(string.Format("Unit exist.  #1 : {0}  #2 : {1}  #3 : {2}  #4 : {3}", bX1, bX2, bX3, bX4));

                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_1_Vacuum : eX.GRDR_Unit_1_Vacuum) != bX1)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT1_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT1_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 1 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_2_Vacuum : eX.GRDR_Unit_2_Vacuum) != bX2)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT2_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT2_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 2 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_3_Vacuum : eX.GRDR_Unit_3_Vacuum) != bX3)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT3_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT3_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 3 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_4_Vacuum : eX.GRDR_Unit_4_Vacuum) != bX4)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT4_VACUUM_ERROR : eErr.RIGHT_GRIND_UNIT4_VACUUM_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Unit 4 vacuum fail.");
                                bError_Safety_Mv = true;
                            }
                        }

                        if (eWay == EWay.L)
                        {
                            Array.Copy(CData.Parts[(int)EPart.ONP].dPcb, CData.Parts[(int)EPart.GRDL].dPcb, CData.Parts[(int)EPart.ONP].dPcb.Length);

                            //-------------
                            if (CDataOption.Use2004U)
                            {
                                CData.Parts[(int)EPart.GRDL].bCarrierWithDummy = false; // LT, Normal/Dual : Flag 초기화  // 2022.01.26 lhs 
                            }
                            //-------------
                            if      ( CData.Opt.bSecsUse == true || CDataOption.MeasureDf == eDfServerType.MeasureDf || (CDataOption.UseDFDataServer && !CData.Dev.bDynamicSkip))//210818 pjh : D/F Data server 기능 License로 구분
                            {// SECSGEM 사용시 Host에서 Down 받은 Data 사용 20200315 LCY
                                // 20200507 (dDf -> dPcb) 잘 못 적용 된 값 전달 부 SCK현장 수정 PJH
                                CData.Parts[(int)EPart.GRDL].dPcbMin  = CData.Parts[(int)EPart.ONP].dPcbMin;
                                CData.Parts[(int)EPart.GRDL].dPcbMax  = CData.Parts[(int)EPart.ONP].dPcbMax;
                                CData.Parts[(int)EPart.GRDL].dPcbMean = CData.Parts[(int)EPart.ONP].dPcbMean;

                                // 2021.08.03 lhs Start : Top/Btm Mold 데이터 전달
                                if (CDataOption.UseNewSckGrindProc)
                                {
                                    CData.Parts[(int)EPart.GRDL].dTopMoldMax = CData.Parts[(int)EPart.ONP].dTopMoldMax;
                                    CData.Parts[(int)EPart.GRDL].dTopMoldAvg = CData.Parts[(int)EPart.ONP].dTopMoldAvg;
                                    CData.Parts[(int)EPart.GRDL].dBtmMoldMax = CData.Parts[(int)EPart.ONP].dBtmMoldMax;
                                    CData.Parts[(int)EPart.GRDL].dBtmMoldAvg = CData.Parts[(int)EPart.ONP].dBtmMoldAvg;
                                }
                                // 2021.08.03 lhs End

                                // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                                //if (CData.GemForm != null) CData.GemForm.Strip_Data_Shift(5, 6);// Picker Data -> L Chuck Data  로 이동
                                if (CData.GemForm != null)
                                {
                                    CData.GemForm.Strip_Data_Shift((int)EDataShift.ONP_STRIP_PICK/*5*/, (int)EDataShift.GRL_TBL_CHUCK/*6*/);// Picker Data -> L Chuck Data  로 이동
                                }
                                // 2021.07.19 SungTae End
                            }
                            else if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                            {
                                //20191029 ghk_dfserver_notuse_df
                                if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                                {//DF 서버 사용시 DF 측정 사용
                                    CData.Parts[(int)EPart.GRDL].dDfMin  = CData.Parts[(int)EPart.ONP].dDfMin;
                                    CData.Parts[(int)EPart.GRDL].dDfMax  = CData.Parts[(int)EPart.ONP].dDfMax;
                                    CData.Parts[(int)EPart.GRDL].dDfAvg  = CData.Parts[(int)EPart.ONP].dDfAvg;
                                    CData.Parts[(int)EPart.GRDL].sBcrUse = CData.Parts[(int)EPart.ONP].sBcrUse;
                                }
                                else
                                {//DF 서버 사용시 DF 측정 안함
                                    CData.Parts[(int)EPart.GRDL].dDfMax = CData.Parts[(int)EPart.ONP].dDfMax;
                                }
                            }

                            // 2021.05.31. SungTae Start : [추가]
                            //if (CData.CurCompany == ECompany.Qorvo_NC) //210818 pjh : D/F Data server 기능 License로 구분
                            if (CDataOption.UseDFDataServer && !CData.Opt.bSecsUse) //211222 pjh : 조건 변경. SECS/GEM과 D/F Server 동시 사용 불가
                            {
                                if (CData.Dev.eMoldSide != ESide.Top) // 1st Btm Grinding과 2nd Btm Grinding이면, 
                                {                                    
                                    m_tTarget = LoadDFServerData(CData.Parts[(int)EPart.ONP].sLotName, CData.Parts[(int)EPart.ONP].sBcr);
                                    //if (m_tTarget.dPcbMean == 0 || m_tTarget.dPcbMax == 0 || m_tTarget.dAfAvg == 0 || m_tTarget.dAfMax == 0)
                                    if (m_tTarget.dPcbMean == 0 && m_tTarget.dPcbMax == 0 && m_tTarget.dAfAvg == 0 && m_tTarget.dAfMax ==0)//211221 pjh : 조건 변경
                                    {
                                        CErr.Show(eErr.DF_SERVER_DATA_FILE_READING_FAIL);

                                        m_iStep = 0;
                                        return true;
                                    }

                                    _SetLog(">>>>> [DFSERVER] : LoadDFServerData(b2ndBtm, CData.Parts[(int)EPart.ONP].sLotName, CData.Parts[(int)EPart.ONP].sBcr)");
                                                                        
                                    CData.Parts[(int)EPart.GRDL].dPcbMin     = m_tTarget.dPcbMin;
                                    CData.Parts[(int)EPart.GRDL].dPcbMax     = m_tTarget.dPcbMax;
                                    CData.Parts[(int)EPart.GRDL].dPcbMean    = m_tTarget.dPcbMean;

                                    //220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                                    CData.Parts[(int)EPart.GRDL].dTopMoldMax = m_tTarget.dAfMax;
                                    CData.Parts[(int)EPart.GRDL].dTopMoldAvg = m_tTarget.dAfAvg;
                                    //
                                }
                            }
                            // 2021.05.31. SungTae End

                            CData.Parts[(int)EPart.GRDL].sLotName = CData.Parts[(int)EPart.ONP].sLotName;
                            CData.Parts[(int)EPart.GRDL].iMGZ_No  = CData.Parts[(int)EPart.ONP].iMGZ_No ;
                            CData.Parts[(int)EPart.GRDL].iSlot_No = CData.Parts[(int)EPart.ONP].iSlot_No;
                            CData.Parts[(int)EPart.GRDL].sBcr     = CData.Parts[(int)EPart.ONP].sBcr    ;
                            CData.Parts[(int)EPart.ONP].sBcr      = ""; 
                            CData.Parts[(int)EPart.GRDL].bChkGrd  = false; //200624 pjh : Grinding 중 Error Check 변수

                            if (CData.IsMultiLOT())
                            {
                                // Grinding에 들어가는 LOT Name이 현재 작업중인 LOT이다.
                                CData.LotInfo.sLotName = CData.Parts[(int)EPart.GRDL].sLotName;
                                CData.Parts[(int)EPart.GRDL].LotColor   = CData.Parts[(int)EPart.ONP].LotColor;
                                CData.Parts[(int)EPart.GRDL].nLoadMgzSN = CData.Parts[(int)EPart.ONP].nLoadMgzSN;

                                // 2022.05.04 SungTae Start : [추가]
                                if (CData.CurCompany == ECompany.ASE_K12)
                                    CData.Parts[(int)EPart.GRDL].sMGZ_ID = CData.Parts[(int)EPart.ONP].sMGZ_ID;
                                // 2022.05.04 SungTae End

                                // 2022.05.04 SungTae : [확인용]
                                _SetLog($"LoadMgzSN : ONP({CData.Parts[(int)EPart.ONP].nLoadMgzSN}) -> GRDL({CData.Parts[(int)EPart.GRDL].nLoadMgzSN})");
                            }

                            // 200314-mjy : 패키지 유닛일 경우 유닛 데이터 복사
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                Array.Copy(CData.Parts[(int)EPart.ONP].aUnitEx, CData.Parts[(int)EPart.GRDL].aUnitEx, CData.Dev.iUnitCnt);
                                Array.Copy(CData.Parts[(int)EPart.ONP].aUnitEx, CData.GrData[(int)EWay.L].aUnitEx, CData.Dev.iUnitCnt);

                                for (int i = 0; i < CData.Dev.iUnitCnt; i++)
                                {
                                    CData.GrData[(int)EWay.L].aUnit[i].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[(int)EWay.L].aUnit[i].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                }
                            }

                            // 2021-05-25, jhLee, Multi-LOT, Strip 정보가 갱신되었음을 표시
                            CData.Parts[(int)EPart.GRDL].bIsUpdate  = true;
                            CData.Parts[(int)EPart.GRDL].iStat      = ESeq.GRL_Grinding ;

                            //ksg Strip 유무 변경
                            //1908920 ksg : 자재 이동 시점이 먼저라 22번 스텝으로 옮김
                            //CData.Parts[(int)EPart.GRDL].bExistStrip = CData.Parts[(int)EPart.ONP].bExistStrip;
                            //CData.Parts[(int)EPart.ONP ].bExistStrip  = false;
                            //=====================================================================================
                            //듀얼 모드 상태 -> ONP_PickTbL
                            //노멀 모드 상태 -> ONP_Wait
                            //=====================================================================================
                            if (CData.Dev.bDual == eDual.Dual)
                            {
                                //20190611 ghk_onpclean
                                if(CDataOption.eOnpClean == eOnpClean.Use)
                                {
                                    //20200415 jhc : Picker Idle Time 개선
                                    if (CDataOption.PickerImprove == ePickerTimeImprove.Use) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                                    {
                                        //20200417 jhc : Off-Picker만 L-Table -> R-Table 자재 이송 담당 (Picker동작개선 && Auto-Running && Step Mode)
                                        m_PrESeq = ESeq.ONP_Wait; //On-Picker -> Clean 후 Wait 예정 : 항상 Off-Picker가 L-Table Pick하도록 함
                                    }
                                    else
                                    {
                                        m_PrESeq = ESeq.ONP_PickTbL; //On-Picker -> Clean 후 L-Table Pick 예정
                                    }
                                    //

                                    CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Clean;
                                    CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Clean);
                                    _SetLog("X axis move clean.", CData.Dev.dOnp_X_Clean);
                                }
                                else
                                {
                                    //20200415 jhc : Picker Idle Time 개선
                                    if (CDataOption.PickerImprove == ePickerTimeImprove.Use) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                                    {
                                        //20200417 jhc : Off-Picker만 L-Table -> R-Table 자재 이송 담당 (Picker동작개선 && Auto-Running && Step Mode)
                                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait; //On-Picker -> Wait 

                                        // 2021.04.30 SungTae Start : Normal Mode에서도 Dual Mode와 동일하게 L-Table Pickup 대기 위치로 이동되도록 수정 요청(QORVO)
                                        if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                                        {
                                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);          //OnPicker -> L-Table Pickup 대기 위치로 이동
                                            _SetLog("X axis move wait.", CData.SPos.dONP_X_WaitPickL);
                                        }
                                        else
                                        {
                                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);         //OnPicker Wait Pos.로 이동 : 항상 Off-Picker가 L-Table Pick하도록 함
                                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);
                                        }
                                        // 2021.04.30 SungTae End
                                    }
                                    else
                                    {
                                        CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_PickTbL; //On-Picker -> L-Table Pick

                                        // 2021.04.30 SungTae Start : Normal Mode에서도 Dual Mode와 동일하게 L-Table Pickup 대기 위치로 이동되도록 수정 요청(QORVO)
                                        if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                                        {
                                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);    //OnPicker -> L-Table Pickup로 이동
                                            _SetLog("X axis move wait.", CData.SPos.dONP_X_WaitPickL);
                                        }
                                        else
                                        {
                                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);          //OnPicker -> WAIT_POS로 이동
                                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);
                                        }
                                        // 2021.04.30 SungTae End
                                    }
                                }

                                _SetLog(string.Format("Status change.  ONP : {0}  GDL : {1}", CData.Parts[(int)EPart.ONP].iStat, CData.Parts[(int)EPart.GRDL].iStat));

                                // 2020.12.14 JSKim St
                                if (bError_Safety_Mv)
                                {
                                    m_iStep = 0;
                                    return true;
                                }
                                else
                                {
                                    m_iStep++;
                                    return false;
                                }
                                // 2020.12.14 JSKim Ed
                            }
                            else   // eDual.Normal
                            {
                                if (CDataOption.eOnpClean == eOnpClean.Use)
                                {
                                    m_PrESeq = ESeq.ONP_Wait;
                                    CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Clean;
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait;
                                }
                            }

                            _SetLog(string.Format("Status change.  ONP : {0}  GDL : {1}", CData.Parts[(int)EPart.ONP].iStat, CData.Parts[(int)EPart.GRDL].iStat));

                            //200717 jhc : Place L-Table Tack Time 로그 추가
                            m_tTackTime = DateTime.Now - m_tStTime;
                            _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
                        else  // Right
                        {
                            Array.Copy(CData.Parts[(int)EPart.ONP].dPcb, CData.Parts[(int)EPart.GRDR].dPcb, CData.Parts[(int)EPart.ONP].dPcb.Length);

                            //-----------
                            // 2022.01.25 lhs Start : 2004U, 
                            if (CDataOption.Use2004U)
                            {
                                if (CData.Dev.bDual == eDual.Dual)  // Dumy 데이터 전달
                                {
                                    CData.Parts[(int)EPart.GRDR].bCarrierWithDummy = CData.Parts[(int)EPart.ONP].bCarrierWithDummy; // OnPicker -> RT, Dual
                                    CData.Parts[(int)EPart.ONP].bCarrierWithDummy  = false;   // OnPicker : Flag 초기화

                                    Array.Copy(CData.Dev.bCopyDummy, CData.Dev.aData[1].bDummy, CData.Dev.bCopyDummy.Length);  // 데이터 전달 위해 Copy : OnPicker -> RT
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.GRDR].bCarrierWithDummy = false; // RT, Normal : Flag 초기화
                                }                                
                            }
                            // 2022.01.25 lhs End 
                            //-----------

                            //if ((CData.Opt.bSecsUse == true) || (CDataOption.MeasureDf == eDfServerType.MeasureDf) || (CData.CurCompany == ECompany.Qorvo_NC && !CData.Dev.bDynamicSkip))
                            if ((CData.Opt.bSecsUse == true) || (CDataOption.MeasureDf == eDfServerType.MeasureDf) || (CDataOption.UseDFDataServer && !CData.Dev.bDynamicSkip))//210818 pjh : D/F Data server 기능 License로 구분
							{
								// SECSGEM 사용시 Host에서 Down 받은 Data 사용 20200315 LCY
								// 20200507 (dDf -> dPcb) 잘 못 적용 된 값 전달 부 SCK현장 수정 PJH
								CData.Parts[(int)EPart.GRDR].dPcbMin  = CData.Parts[(int)EPart.ONP].dPcbMin;
                                CData.Parts[(int)EPart.GRDR].dPcbMax  = CData.Parts[(int)EPart.ONP].dPcbMax;
                                CData.Parts[(int)EPart.GRDR].dPcbMean = CData.Parts[(int)EPart.ONP].dPcbMean;

                                // 2021.08.03 lhs Start : Top/Btm Mold 데이터
                                if (CDataOption.UseNewSckGrindProc)
                                {
                                    CData.Parts[(int)EPart.GRDR].dTopMoldMax = CData.Parts[(int)EPart.ONP].dTopMoldMax;
                                    CData.Parts[(int)EPart.GRDR].dTopMoldAvg = CData.Parts[(int)EPart.ONP].dTopMoldAvg;
                                    CData.Parts[(int)EPart.GRDR].dBtmMoldMax = CData.Parts[(int)EPart.ONP].dBtmMoldMax;
                                    CData.Parts[(int)EPart.GRDR].dBtmMoldAvg = CData.Parts[(int)EPart.ONP].dBtmMoldAvg;
                                }
                                // 2021.08.03 lhs End

                                if (CData.GemForm != null)
                                {// Picker Right Table Place 20200430 LCY
                                    // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                                    if (CData.Dev.bDual == eDual.Dual)// Step Mode 일때는 Letf After Data 를 Right Table 로 이동
                                    {
                                        //CData.GemForm.Strip_Data_Shift(11, 12);// Left Table Pick'up Data -> Right Table 로 이동
                                        CData.GemForm.Strip_Data_Shift((int)EDataShift.OFP_LT_PICK/*11*/, (int)EDataShift.GRR_TBL_CHUCK/*12*/);// Left Table Pick'up Data -> Right Table 로 이동 

#if false
                                        // 2021.08.23 SungTae : [임시] 디버깅 위해 임시로 주석 처리. 테스트 후 주석 해제 예정.
                                        /*
                                         * true : Auto Run 진행 시
                                         * false : Debugging으로 Dry Run 진행 시
                                         */
                                        
                                        CData.GemForm.Strip_Data_Shift((int)EDataShift.GRR_TBL_CHUCK, (int)EDataShift.GRR_AF_MEAS/*12*/);// Left Table Pick'up Data -> Right Table 로 이동 
#endif
                                    }
                                    else
                                    {
                                        // Normal Mode 에서는 Picker Data 를 Right Table 로 이동
                                        //CData.GemForm.Strip_Data_Shift(5, 12);// Picker Data -> Right Table 로 이동
                                        CData.GemForm.Strip_Data_Shift((int)EDataShift.ONP_STRIP_PICK/*5*/, (int)EDataShift.GRR_TBL_CHUCK/*12*/);// Picker Data -> Right Table 로 이동 

#if false
                                        // 2021.08.23 SungTae : [임시] 디버깅 위해 임시로 주석 처리. 테스트 후 주석 해제 예정.
                                        /*
                                         * true : Auto Run 진행 시
                                         * false : Debugging으로 Dry Run 진행 시
                                         */
                                        
                                        CData.GemForm.Strip_Data_Shift((int)EDataShift.GRR_TBL_CHUCK, (int)EDataShift.GRR_AF_MEAS/*12*/);// Picker Data -> Right Table 로 이동 
#endif
                                    }
                                    // 2021.07.19 SungTae End
                                }
                            }
                            else if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                            {
                                //20191029 ghk_dfserver_notuse_df
                                if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                                {//DF 서버 사용시 DF 측정 사용
                                    CData.Parts[(int)EPart.GRDR].dDfMin  = CData.Parts[(int)EPart.ONP].dDfMin;
                                    CData.Parts[(int)EPart.GRDR].dDfMax  = CData.Parts[(int)EPart.ONP].dDfMax;
                                    CData.Parts[(int)EPart.GRDR].dDfAvg  = CData.Parts[(int)EPart.ONP].dDfAvg;
                                    CData.Parts[(int)EPart.GRDR].sBcrUse = CData.Parts[(int)EPart.ONP].sBcrUse;
                                }
                                else
                                {//DF 서버 사용시 DF 측정 안함
                                    CData.Parts[(int)EPart.GRDR].dDfMax = CData.Parts[(int)EPart.ONP].dDfMax;
                                }
                            }

                            // 2021.05.31. SungTae Start : [추가]
                            //if (CData.CurCompany == ECompany.Qorvo_NC)
                            //if (CDataOption.UseDFDataServer) //210818 pjh : D/F Data server 기능 License로 구분
                            if (CDataOption.UseDFDataServer && !CData.Opt.bSecsUse) //211222 pjh : 조건 변경. SECS/GEM과 D/F Server 동시 사용 불가
                            {
                                if (CData.Dev.eMoldSide != ESide.Top) // 1st Btm Grinding과 2nd Btm Grinding이면, 
                                {
                                    m_tTarget = LoadDFServerData(CData.Parts[(int)EPart.ONP].sLotName, CData.Parts[(int)EPart.ONP].sBcr);

                                    //if (m_tTarget.dPcbMean == 0 || m_tTarget.dPcbMax == 0 || m_tTarget.dAfAvg == 0 || m_tTarget.dAfMax == 0)
                                    if (m_tTarget.dPcbMean == 0 && m_tTarget.dPcbMax == 0 && m_tTarget.dAfAvg == 0 && m_tTarget.dAfMax == 0)//211221 pjh : 조건 변경
                                    {
                                        CErr.Show(eErr.DF_SERVER_DATA_FILE_READING_FAIL);

                                        m_iStep = 0;
                                        return true;
                                    }

                                    _SetLog(">>>>> [DFSERVER] : LoadDFServerData(b2ndBtm, CData.Parts[(int)EPart.ONP].sLotName, CData.Parts[(int)EPart.ONP].sBcr)");

                                    CData.Parts[(int)EPart.GRDR].dPcbMin    = m_tTarget.dPcbMin;
                                    CData.Parts[(int)EPart.GRDR].dPcbMax    = m_tTarget.dPcbMax;
                                    CData.Parts[(int)EPart.GRDR].dPcbMean   = m_tTarget.dPcbMean;

                                    //220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                                    CData.Parts[(int)EPart.GRDR].dTopMoldMax = m_tTarget.dAfMax;
                                    CData.Parts[(int)EPart.GRDR].dTopMoldAvg = m_tTarget.dAfAvg;
                                    //
                                }
                            }
                            // 2021.05.31. SungTae End

                            CData.Parts[(int)EPart.GRDR].sLotName = CData.Parts[(int)EPart.ONP].sLotName;
                            CData.Parts[(int)EPart.GRDR].iMGZ_No  = CData.Parts[(int)EPart.ONP].iMGZ_No ;
                            CData.Parts[(int)EPart.GRDR].iSlot_No = CData.Parts[(int)EPart.ONP].iSlot_No;
                            CData.Parts[(int)EPart.GRDR].sBcr     = CData.Parts[(int)EPart.ONP].sBcr    ;
                            CData.Parts[(int)EPart.GRDR].dShiftT  = CData.Parts[(int)EPart.ONP].dShiftT ; //190529 ksg :
                            CData.Parts[(int)EPart.ONP ].sBcr     = ""; //190823 ksg :
                            CData.Parts[(int)EPart.GRDR].bChkGrd  = false; //200624 pjh : Grinding 중 Error Check 변수

                            if (CData.IsMultiLOT())
                            {
                                // Grinding에 들어가는 LOT Name이 현재 작업중인 LOT이다.
                                CData.LotInfo.sLotName = CData.Parts[(int)EPart.GRDR].sLotName;
                                CData.Parts[(int)EPart.GRDR].LotColor   = CData.Parts[(int)EPart.ONP].LotColor;
                                CData.Parts[(int)EPart.GRDR].nLoadMgzSN = CData.Parts[(int)EPart.ONP].nLoadMgzSN;

                                // 2022.05.04 SungTae Start : [추가]
                                if (CData.CurCompany == ECompany.ASE_K12)
                                    CData.Parts[(int)EPart.GRDR].sMGZ_ID = CData.Parts[(int)EPart.ONP].sMGZ_ID;
                                // 2022.05.04 SungTae End

                                // 2022.05.04 SungTae : [확인용]
                                _SetLog($"LoadMgzSN : ONP({CData.Parts[(int)EPart.ONP].nLoadMgzSN}) -> GRDR({CData.Parts[(int)EPart.GRDR].nLoadMgzSN})");
                            }

                            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                            // Data Shift : On-Picker => R-Table  (18 Point 측정 여/부)
                            CData.Parts[(int)EPart.GRDR].b18PMeasure = CData.Parts[(int)EPart.ONP].b18PMeasure; //Data Shift는 조건별 사용 여부에 무관하게 진행 함
                            CData.Parts[(int)EPart.ONP].b18PMeasure  = false;
                            //

                            // 200314-mjy : 패키지 유닛일 경우 유닛 데이터 복사
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                Array.Copy(CData.Parts[(int)EPart.ONP].aUnitEx, CData.Parts[(int)EPart.GRDR].aUnitEx, CData.Dev.iUnitCnt);
                                Array.Copy(CData.Parts[(int)EPart.ONP].aUnitEx, CData.GrData[(int)EWay.R].aUnitEx, CData.Dev.iUnitCnt);

                                for (int i = 0; i < CData.Dev.iUnitCnt; i++)
                                {
                                    CData.GrData[(int)EWay.R].aUnit[i].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[(int)EWay.R].aUnit[i].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                }
                            }


                            // 2021-05-25, jhLee, Multi-LOT, Strip 정보가 갱신되었음을 표시
                            CData.Parts[(int)EPart.GRDR].bIsUpdate  = true;
                            CData.Parts[(int)EPart.GRDR].iStat      = ESeq.GRR_Grinding;

                            //20190611 ghk_onpclean
                            if (CDataOption.eOnpClean == eOnpClean.Use)
                            {
                                m_PrESeq = ESeq.ONP_Wait;
                                CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Clean;
                            }
                            else
                            {
                                CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait;
                            }

                            //
                            //ksg Strip 유무 변경
                            //1908920 ksg : 자재 이동 시점이 먼저라 22번 스텝으로 옮김
                            //CData.Parts[(int)EPart.GRDR].bExistStrip = CData.Parts[(int)EPart.ONP].bExistStrip;
                            //CData.Parts[(int)EPart.ONP ].bExistStrip = false;
                            //if(CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd && 
                            //   CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd   ) CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_WorkEnd; 
                            //CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait; //200213 ksg :
                            _SetLog(string.Format("Status change.  ONP : {0}  GDR : {1}", CData.Parts[(int)EPart.ONP].iStat, CData.Parts[(int)EPart.GRDR].iStat));

                            m_tTackTime = DateTime.Now - m_tStTime;
                            _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                            m_iStep = 0;
                            return true;
                        }
                    }

                case 29:
                    {//듀얼 모드 일 경우 X축 대기위치로 보내는 구문 추가
                        //20190611 ghk_onpclean
                        if (CDataOption.eOnpClean == eOnpClean.Use)
                        {                            
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Clean))
                            { return false; }
                        }
                        else
                        {
                            //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            if (CDataOption.PickerImprove == ePickerTimeImprove.Use && CData.Dev.bDual == eDual.Dual && 
                                CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PickTbL)
                            {
                                if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL)) //OnPicker -> L-Table Pickup 대기 위치로 이동 확인
                                { return false; }
                            }
                            else
                            {
                                // 2021.04.30 SungTae Start : Normal Mode에서도 Dual Mode와 동일하게 L-Table Pickup 대기 위치로 이동되도록 수정 요청(QORVO)
                                if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                                {
                                    if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL)) //OnPicker -> L-Table Pickup 대기 위치로 이동 확인
                                    { return false; }
                                }
                                else
                                {
                                    if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))       //OnPicker Wait Pos.로 이동 확인
                                    { return false; }
                                }
                                // 2021.04.30 SungTae End
                            }
                        }

                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20190611 ghk_onpclean
        public bool Cyl_PickerClean()
        {
            //Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_PICKERCLEAN_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            //20191121 ghk_display_strip 
            if (Chk_Strip())
            {//자재 없어야 함
                CErr.Show(eErr.ONLOADERPICKER_DETECT_STRIP_ERROR);
                _SetLog("Error : Detect strip.");

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
                    {//버큠 오프, 드레인 오프, 이젝트 오프, Z축 대기위치 이동
                        if (CData.Dev.iOnpCleanCnt <= 0)
                        {
                            CData.Parts[(int)EPart.ONP].iStat = m_PrESeq;
                            _SetLog("Finish.  Count : 0");

                            m_iStep = 0;
                            return true;
                        }

                        if (CIO.It.Get_X(eX.ONP_Vacuum))
                        {
                            CErr.Show(eErr.ONLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error : Detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_Vacuum(false);
                        Func_Eject(false);
                        Func_Drain(false);

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iCleanCnt = 0;
                        m_bDir = false;

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Z축 대기 위치 이동 확인, X축 클리닝 위치로 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Clean);
                        _SetLog("X axis move clean.", CData.Dev.dOnp_X_Clean);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//X축 클리닝 위치 이동 확인, 피커 0도
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Clean))
                        { return false; }

                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//피커 0도 확인, 피커 클리너 왼쪽 이동
                        if (!Func_Picker0())
                        { return false; }

                        Func_CleanerLeft();
                        _SetLog("Cleaner left.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//피커 클리너 왼쪽 이동 확인, Z축 클리닝 위치로 이동
                        if (!Func_CleanerLeft())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Clean);
                        _SetLog("Z axis move clean.", CData.Dev.dOnp_Z_Clean);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//Z축 클리닝 위치 이동 확인, 피커 이젝트 온, 클리너 에어 온
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Clean))
                        { return false; }

                        Func_Eject(true);
                        CIO.It.Set_Y(eY.INR_OnpCleaner_Air, true);
                        _SetLog("Eject on.  Cleaner air on.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//클리닝 동작
                        if (!m_bDir)
                        {//왼쪽 방향 -> 오른쪽 방향
                            Func_CleanerRight();
                            _SetLog("Cleaner right.");
                        }
                        else
                        {//오른쪽 방향 -> 왼쪽 방향
                            Func_CleanerLeft();
                            _SetLog("Cleaner left.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//클리닝 동작
                        if (!m_bDir)
                        {//왼쪽 방향 -> 오른족 방향 이동 확인
                            if (!Func_CleanerRight())
                            { return false; }
                        }
                        else
                        {//오른쪽 방향 -> 왼쪽 방향 이동 확인
                            if (!Func_CleanerLeft())
                            { return false; }
                        }
                        m_bDir = !m_bDir;
                        m_iCleanCnt++;
                        _SetLog("Cleaning count : " + m_iCleanCnt);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//클리닝 동작 반복/종료 확인, 클리너 에어 오프, 피커 이젝트 오프
                        if (m_iCleanCnt < CData.Dev.iOnpCleanCnt)
                        {
                            _SetLog("Loop.");
                            m_iStep = 16;
                            return false;
                        }

                        CIO.It.Set_Y(eY.INR_OnpCleaner_Air, false);
                        Func_Eject(false);
                        _SetLog("Loop end.  Cleaner air off.  Eject off.");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//Z축 대기 위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

#if true //20200406 jhc : OnPicker L-Table Pickup 대기 위치 변경
                case 20:
                    {//Z축 대기 위치 이동 확인
                     //(Auto Run && Dual Mode && 다음 Seq. L-Table Pick) 경우 X축 L-Table Pick 대기 위치 이동

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        if ((CData.Dev.bDual == eDual.Dual) && (m_PrESeq == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            //X축 L-Table Pick 대기 위치 이동
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);
                            _SetLog("X axis move wait pick left.", CData.SPos.dONP_X_WaitPickL);

                            m_iStep++;
                            return false;
                        }
                        else
                        {
                            //OnP 상태 변경
                            CData.Parts[(int)EPart.ONP].iStat = m_PrESeq;
                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.ONP].iStat);

                            m_iStep = 0;
                            return true;
                        }
                    }

                case 21:
                    {//X축 L-Table Pick 대기 위치 이동 확인, Onp 상태 변경

                        //Auto Running, Dual Mode, 다음 Seq. L-Table Pick 경우에만 여기로 분기되므로 조건 검사 생략
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL))
                        { return false; }

                        CData.Parts[(int)EPart.ONP].iStat = m_PrESeq;
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.ONP].iStat);

                        m_iStep = 0;
                        return true;
                    }
#else
                case 20:
                    {//Z축 대기 위치 이동 확인, Onp 상태 변경
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CData.Parts[(int)EPart.ONP].iStat = m_PrESeq;

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "CData.Parts[(int)EPart.ONP].iStat = " + CData.Parts[(int)EPart.ONP].iStat.ToString();
                        SaveLog();

                        m_iStep = 0;
                        return true;
                    }
#endif
            }
        }
        //

        public bool Cyl_Conversion()
        {
            if ((m_iStep < 11) && (m_iStep != m_iPreStep))
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_CONVERSION_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                //20190821 pjh : Conversion Position시 Grinder Z Wait Pos, Grinder Y Dresser Replace Pos
                case 10:
                    {//Probe Up, Air Off
                        CIO.It.Set_Y(eY.GRDL_ProbeDn, false);
                        CIO.It.Set_Y(eY.GRDL_ProbeAir, false);
                        CIO.It.Set_Y(eY.GRDR_ProbeDn, false);
                        CIO.It.Set_Y(eY.GRDR_ProbeAir, false);
                        _SetLog("GDL Probe up.  GDR Probe up.");

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {//Probe Up, Air Off 확인 Grind Z축 Wait Pos 이동
                        if (CIO.It.Get_Y(eY.GRDL_ProbeDn) || CIO.It.Get_Y(eY.GRDR_ProbeDn))
                        { return false; }

                        CMot.It.Mv_N(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]);
                        _SetLog("GDL Z axis move wait.", CData.SPos.dGRD_Z_Wait[0]);
                        CMot.It.Mv_N(m_iRGZ, CData.SPos.dGRD_Z_Wait[1]);
                        _SetLog("GDR Z axis move wait.", CData.SPos.dGRD_Z_Wait[1]);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//Grind Z Wait Pos 확인
                        if (!CMot.It.Get_Mv(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iRGZ, CData.SPos.dGRD_Z_Wait[1]))
                        { return false; }
                        _SetLog("Check GDL, GDR Z axis move.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Right Grind Z Wait Pos 확인, Top Cleaner Up
                        if (!CMot.It.Get_Mv(m_iRGZ, CData.SPos.dGRD_Z_Wait[1]))
                        { return false; }

                        //syc : new cleaner
                        //CIO.It.Set_Y(eY.GRDL_TopClnDn, false);
                        //CIO.It.Set_Y(eY.GRDR_TopClnDn, false);
                        CData.L_GRD.ActTcDown(false);
                        CData.R_GRD.ActTcDown(false);

                        _SetLog("GDL, GDR Top cleaner up.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Top Cleaner Up 확인, Left Grind Y Dresser Change Pos 이동
                        //syc : new cleaner
                        //if (CIO.It.Get_Y(eY.GRDL_TopClnDn) && CIO.It.Get_Y(eY.GRDR_TopClnDn))
                        if (CData.L_GRD.Chk_CleanerDn(false) && CData.R_GRD.Chk_CleanerDn(false))
                        { return false; }

                        CMot.It.Mv_N(m_iLTY, CData.SPos.dGRD_Y_DrsChange[0]);
                        _SetLog("GDL Y axis move dresser change.", CData.SPos.dGRD_Y_DrsChange[0]);
                        CMot.It.Mv_N(m_iRTY, CData.SPos.dGRD_Y_DrsChange[1]);
                        _SetLog("GDR Y axis move dresser change.", CData.SPos.dGRD_Y_DrsChange[1]);

                        m_iStep++;
                        return false;
                    }
                case 15:
                    {//Left Grind Y Dresser Change Pos 확인, Right Grind Y Dresser Change Pos 이동
                        if (!CMot.It.Get_Mv(m_iLTY, CData.SPos.dGRD_Y_DrsChange[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iRTY, CData.SPos.dGRD_Y_DrsChange[1]))
                        { return false; }

                        _SetLog("Check GDL, GDR Y axis move.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        double chkOfpPos =  CMot.It.Get_FP((int)EAx.OffLoaderPicker_X);
                        if (chkOfpPos > CData.SPos.dOFP_X_PickR - 10)
                        {
                            CErr.Show(eErr.ONLOADERPICKER_CAN_NOT_MOVE_CONVERSION_POSITION);
                            _SetLog("Error : X axis can't move conversion position.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check X axis position.");

                        m_iStep++;
                        return false;

                    }
                case 17:
                    {//Z축 대기위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Z축 대기위치 이동 확인, 이젝트 오프, 피커 90도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }
                        CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_Con);
                        _SetLog("X axis move conversion.", CData.SPos.dONP_X_Con);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_Con))
                        { return false; }

                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20190604 ghk_onpbcr
        public bool Cyl_OnpBcr()
        {
            //Timeout check
            if (m_iPreStep != m_iStep)
            {
#if true //20200427 jhc : SECG/GEM 사용 시 BCR/ORI Timeout 시간 연장
                if(CData.Opt.bSecsUse)  { m_TiOut.Set_Delay(SECSGEM_BCR_TIMEOUT);   }
                else                    { m_TiOut.Set_Delay(TIMEOUT);               }
#else
                m_TiOut.Set_Delay(TIMEOUT);
#endif
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_BCR_TIMEOUT);
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
                //190617 ksg :
                case 10:
                    {
                        if(m_bBcrView)
                        {
                            if(m_bBcrKeyIn)
                            {
                                m_bBcrView = false;
                                _SetLog("Barcode key-in.");

                                m_iStep = 21;
                                return false;
                            }
                            if(m_bBcrErr)
                            {
                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                _SetLog("Error : Reading fail.");

                                m_iBcrCnt  = 0;
                                m_bBcrView = false;

                                m_iStep    = 0;
                                return true;
                            }
                            else //20200526 jhc : 누락 부분 추가
                            {
                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                _SetLog("Error : Reading fail.");

                                m_iBcrCnt  = 0;
                                m_bBcrView = false;

                                m_iStep    = 0;
                                return true;
                            }
                        }
                        CBcr.It.Save_RunStatus();
                        m_Delay.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        //190801 ghk_Keyin
                        m_bBcrKeyIn = false;
                        m_bBcrErr   = false;
                        m_bBcrView = false;

                        m_iStep++; 
                        return false;
                    }

                case 11: 
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }
                        m_bSecondBcr = false;
                        _SetLog("Init.");

                        m_iStep++; 
                        return false;
                    }

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
                            _SetLog("Error : BCR run fail.");

                            m_iStep = 0;
                            return true;
                        }

                        CBcr.It.Load();
                        if(CData.Bcr.bStatusBcr)
                        {
                            CErr.Show(eErr.BCR_NOT_READY);
                            _SetLog("Error : BCR not ready.");

                            m_iStep = 0;
                            return true;
                        }
                        CData.Bcr.sBcr = "";
                        _SetLog("Check run.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Picker 0 turn
                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }
                case 14:
                    {//Picker 0 turn
                        if (!Func_Picker0())
                        { return false; }
                        _SetLog("Check picker 0.");

                        m_iStep++;
                        return false;
                    }
                case 15:
                    {//X축, Y축 바코드 측정 위치 이동
                        if(!m_bSecondBcr)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Bcr);       _SetLog("X axis move bcr.", CData.Dev.dOnp_X_Bcr);
                            CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Bcr);       _SetLog("Y axis move bcr.", CData.Dev.dOnp_Y_Bcr);
                        }
                        else
                        {//X축, Y축 바코드 Second 측정 위치 이동
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_BcrSecon);  _SetLog("X axis move 2nd bcr.", CData.Dev.dOnp_X_BcrSecon);
                            CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_BcrSecon);  _SetLog("Y axis move 2nd bcr.", CData.Dev.dOnp_X_BcrSecon);
                        }

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//X축 바코드 측정 위치 이동 확인, Z축 바코드 측정 위치 이동
                        if(!m_bSecondBcr)
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Bcr))
                            { return false; }
                        }
                        else
                        {
                            //X축 바코드 Second 측정 위치 이동 확인, Z축 바코드 측정 위치 이동
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_BcrSecon))
                            { return false; }
                        }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Bcr);
                        _SetLog("Z axis move bcr.", CData.Dev.dOnp_Z_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//Z축 바코드 측정 위치 이동 확인, Y축 바코드 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Bcr))
                        { return false; }
                        if(!m_bSecondBcr)
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Bcr)) return false;
                        }
                        else
                        {
                            //Y축 바코드 Second 측정 위치 이동 확인, Y축 바코드 측정 위치 이동
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_BcrSecon)) return false;
                        }
                        _SetLog("Check Y, Z axis move.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Y축 바코드 측정 위치 이동 확인, 바코드 읽기
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Bcr();
                        if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Bcr();
                        else                                       CBcr.It.Chk_Bcr_InR();
                        _SetLog("BCR log : " + CData.m_sBcrLog); //191031 ksg :

#if true //20200401 jhc : 2D-Vision Delay Time 1초
                        if (CDataOption.BcrInterface == eBcrInterface.Udp)
                        {
                            m_Delay.Set_Delay(1000); //1초 후부터 실시간 상태 체크
                            _SetLog("Set delay : 1000ms");
                        }
                        else
                        {
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
                            _SetLog("Set delay : 5000ms");
                        }
#else
                        m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif
                        m_Delay1.Set_Delay(50000); //191031 ksg :
                        
                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//
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
                        //
                        CBcr.It.Load();
                        _SetLog("BCR load.");
                        //191031 ksg :
                        if(m_Delay1.Chk_Delay())
                        {
                            CErr.Show(eErr.BCR_REPLY_TIMEOUT);
                            _SetLog("Error : Reply timeout.");
                            m_iBcrCnt = 0;

                            m_iStep = 0;
                            return true;
                        }
                        if (CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;
                        _SetLog("BCR count : " + m_iBcrCnt);
                        
                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//바코드 읽은 값 확인, 바코드 값 저장, Y축 대기 위치 이동
                        if (!CData.Bcr.bRet || CData.Bcr.sBcr == "error")
                        {
                            if (m_iBcrCnt > 5)
                            {
                                if(!CData.Dev.bBcrSecondSkip && !m_bSecondBcr)
                                {
                                    m_bSecondBcr = true;
                                    m_iBcrCnt    = 0   ;
                                    m_iStep      = 15  ;
                                    return false;
                                }
                                if (CData.Dev.bBcrKeyInSkip)
                                {
                                    CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                    _SetLog("Error : BCR Reading fail.  Key-in skip.");
                                    m_iBcrCnt = 0; //190211 ksg :

                                    m_iStep = 0;
                                    return true;
                                }
                                else
                                {
                                    m_bBcrView = true;
                                    if(CDataOption.KeyInType == eBcrKeyIn.Stop)
                                    {
                                        //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                                        if (CDataOption.Use2DErrorPosition && (CDataOption.CurEqu == eEquType.Pikcer))
                                        {
                                            //1) Z => WAIT_POS
                                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                                            _SetLog("Error : 2D Reading Fail.  Camera Move Error Pos. : 1) Z axis move wait.", CData.SPos.dONP_Z_Wait);

                                            m_iBcrCnt = 0;
                                            m_iStep = 40; //제3의 위치(OCR 위치)로 Picker(카메라) 이동 후 Alarm 발생
                                            return false;
                                        }
                                        else
                                        {
                                            //기존 루틴 : Alarm 발생 후 종료 (Reset => Key In => Start하면 입력한 2D Code로 동작)
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : 2D Reading Fail.  Key-in STOP.");

                                            m_iBcrCnt = 0;
                                            m_iStep = 0;
                                            return true;
                                        }
                                        //..
                                    }
                                    else
                                    {
                                        if (m_bBcrErr)
                                        {
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : BCR Reading fail.");
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
                                        {
                                            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                                            if (CDataOption.Use2DErrorPosition && (CDataOption.CurEqu == eEquType.Pikcer))
                                            {
                                                //1) Z => WAIT_POS
                                                CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                                                _SetLog("Error : 2D Reading Fail.  Camera Move Error Pos. : 1) Z axis move wait.", CData.SPos.dONP_Z_Wait);

                                                m_iBcrCnt = 0;
                                                m_iStep = 40; //제3의 위치(OCR 위치)로 Picker(카메라) 이동 후 Alarm 발생
                                                return false;
                                            }
                                            else
                                            {
                                                //기존 루틴 : Timeout 시간 동안 Key In 대기 (여기에서 Key In 하지않고 가만히 있으면 Timeout 발생하게 됨!)
                                                return false;
                                            }
                                            //..
                                        }
                                    }
                                    
                                }
                            }
                            else
                            {
                                _SetLog("BCR count : " + m_iBcrCnt);

                                //m_iStep = 16;
                                m_iStep = 18;
                                return false;
                            }
                        }

                        _SetLog("BCR : " + CData.Bcr.sBcr);

                        m_iStep++;
                        return false;
                    }
                case 21:
                    { 
                        //20190703 ghk_dfserver
                        //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == eStatus.Auto_Running)
                        if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            CData.dfInfo.sBcr = CData.Bcr.sBcr;
                            if (!CData.dfInfo.bBusy)
                            {
                                CDf.It.SendInrBcr();
                                _SetLog("DF Server send BCR.");

                                m_iStep++;
                                return false;
                            }
                            else
                            { return false; }
                        }
                        else
                        {
                            CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sBcr;
                            /*
                            if (CData.Dev.bOriSkip)
                            {
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;

                                CData.Parts[(int)EPart.ONP].iStat = m_iPreState;

                                m_LogVal.sMsg += ", Bcr = " + CData.Bcr.sBcr;
                                SaveLog();

                                m_iStep = 0;
                                m_iBcrCnt = 0;

                                return true;
                            }
                            */

                            //191118 ksg :
                            if(CData.GemForm != null)	CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = CData.Bcr.sBcr;

                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);

                            m_iStep = 25;
                            return false;
                        }
                    }

                case 22:    // DF Server
                    {
                        if (CDf.It.ReciveInrBcr() == 1)
                        {
                            CErr.Show(eErr.DYNAMIC_FUNCTION_SERVER_NOTHING_GL1_DATA);
                            _SetLog("Error : DF Server nothing GL 1 data.");
                            m_iBcrCnt = 0;

                            m_iStep = 0;
                            return true;
                        }
                        else if (CDf.It.ReciveInrBcr() == 0)
                        {
                            _SetLog("DF Server receive.");
                            m_iStep++;
                        }

                        return false;
                    }

                case 23:    // DF Server
                    {
                        if(!CData.dfInfo.bBusy)
                        {
                            CDf.It.SendResultBcr();
                            _SetLog("DF Server send result.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 24:    // DF Server
                    {
                        if(CDf.It.ReciveResultBcr())
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
                                CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sBcr;
                                CData.Parts[(int)EPart.INR].dDfMax = CData.dfInfo.dBcrMax;
                            }

                            _SetLog("BCR : " + CData.Bcr.sBcr);

                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);

                            m_iStep++;
                        }
                        return false;
                    }

                case 25:
                    {//Y축 대기위치 이동 확인, Z축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {//Z축 대기 위치 이동 확인, X축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        //20200407 jhc : OnPicker L-Table Pickup 대기 위치 변경
                        if ((CData.Dev.bDual == eDual.Dual) && (m_iPreState == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);
                            _SetLog("X axis move wait pick left.", CData.SPos.dONP_X_WaitPickL);

                            m_iStep = 29;
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);

                            m_iStep++;
                        }
                        return false;
                    }

                case 27:
                    {//Picker 90 turn
                        Func_Picker90();
                        _SetLog("Picker 90.");

                        m_iStep++;
                        return false;
                    }
                case 28:
                    {//Picker 0 turn
                        if (!Func_Picker90())
                        { return false; }
                        _SetLog("Check picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 29:
                    {//X축 대기 위치 이동 확인, 시퀀스 상태 변경 
                        //20200407 jhc : OnPicker L-Table Pickup 대기 위치 변경
                        if ((CData.Dev.bDual == eDual.Dual) && (m_iPreState == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL))
                            { return false; }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }
                        }

                        if (!CData.Opt.bSecsUse)
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;    //20200805 syc : ASE KR VOC SECS/GEM (11)
                            CData.Parts[(int)EPart.ONP].iStat = m_iPreState;            //20200805 syc : ASE KR VOC SECS/GEM (11)

                            // SECSGEM 사용 하지 않으면 측정 값 대입 LCY 200131 <== //20200526 jhc : 누락 부분 추가
                            // SECSGEM 사용과 무관 하게 fMeasure_New_Avr_Data 값 사용 하면 됨
                            if (CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Min_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0];
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0];
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0];
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode        = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].nDF_User_Mode;

                                CLog.mLogGnd(string.Format("CSQ3_OnP Step - 29 fMeasure_New_Min_Data {0},{1},{2} , Mode {3}",
                                                                              CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Min_Data[0],
                                                                              CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0],
                                                                              CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0], 
                                                                              CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));
                            }

                            m_iBcrCnt = 0;
                            _SetLog("Finish.  SECS/GEM not use.");

                            m_iStep = 0;
                            return true;
                        }
                        else  // SecsGem 사용
                        {
                            //999200    carrier verify request
                            if (CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = CData.Bcr.sBcr; //20200526 jhc : 누락 부분 추가
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.INR_DF_MEAS/*3*/, (int)EDataShift.INR_BCR_READ/*4*/);// DF Data -> BCR Data 로 이동

                                CData.GemForm.OnCarrierVerifyRequset(CData.Bcr.sBcr);
                                CLog.mLogGnd(string.Format("CSQ3_OnP Step - 29 OnCarrierVerifyRequset {0}", CData.Bcr.sBcr)); //20200526 jhc : 누락 부분 추가
                            }

                            m_Delay.Set_Delay(3 * 60 * 1000); //20200406 jhc : SECSGEM Delay Time 5 m -> 15 sec //m_Delay.Set_Delay(300000);
                            _SetLog("SECS/GEM use.  Set delay : " + (3 * 60 * 1000) + "ms");

                            m_iStep++;
                            return false;
                        }
                    }

                case 30:
                    {
                        m_TiOut.Set_Delay(TIMEOUT); // SECSGEM 사용시 Host 에서 Reply 시간 3분 이상 으로 시퀸스 Timeout 는 사용 하지 않기 위해 지속 적으로 초기화 진행 LCY 20717

                        if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripSuccess)/*3*/)
                        {
                            // SECSGEM 사용 하면 Host 값 대입 LCY 200131
                            // 여기에서 값 Verify 진행 필요.

                            //-----------------------------------
                            // 2022.02.15 lhs Start : SecsGem에서 Download 값 범위 체크, Error 표시
                            //------------------
                            // 수정전
                            //if ((CData.Dynamic.dPcbMeanRange > 0)   &&  //200806 jhc : Range 범위 값이 0인 경우 무시
                            //    (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] > (CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbMeanRange) ||
                            //     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] < (CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbMeanRange) ) )
                            //{
                            //    CLog.mLogGnd(string.Format("CSQ3_OnP Step - 30 Host Download : INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER  New Avr = {0}, [Device Setting] Pcb thickness = {1}, Mean Range = {2} : {3} ~ {4}",
                            //    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0],
                            //    CData.Dev.aData[0].dPcbTh, CData.Dynamic.dPcbMeanRange,
                            //    CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbMeanRange, CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbMeanRange));  // 2021.12.17 lhs 체크하는 변수와 로그표시 변수가 같지 않아 수정 : dPcbRang -> dPcbMeanRange

                            //    CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER);
                            //    _SetLog("Error : DF Pcb height rangeover.(Host Download)");
                            //    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;

                            //    m_iStep = 0;
                            //    return true;
                            //}
                            //------------------

                            bool bOver = IsOverRangeSecsGemDownTh();
                            if (bOver)
                            {
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;
                                m_iStep = 0;
                                return true;
                            }
                            // 2022.02.15 lhs End : SecsGem에서 Download 값 범위 체크, Error 표시
                            //-----------------------------------
                            else // Host 에서 DF Verify 성공
                            { 
                                // 기존에는 Host에서 fMeasure_Max_Data[0]로 Pcb값과 Af값을 받음. 
                                CData.Parts[(int)EPart.INR].dPcbMin         = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0];
                                CData.Parts[(int)EPart.INR].dPcbMax         = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0]; //<== //20200526 jhc : Cyl_OnpOriBcr() 부분과 통일시킴 //CData.JSCK_Gem_Data[4].fMeasure_New_Max_Data[0];
                                CData.Parts[(int)EPart.INR].dPcbMean        = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0]; //<== //20200526 jhc : Cyl_OnpOriBcr() 부분과 통일시킴 //CData.JSCK_Gem_Data[4].fMeasure_New_Avr_Data[0];
                                CData.JSCK_Gem_Data[4].nDF_User_New_Mode    = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_Mode;

                                // 2021.08.03 lhs Start : Top/Btm Mold 데이터
                                if (CDataOption.UseNewSckGrindProc)
                                {
                                    CData.Parts[(int)EPart.INR].dTopMoldMax = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max;
                                    CData.Parts[(int)EPart.INR].dTopMoldAvg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg;
                                    CData.Parts[(int)EPart.INR].dBtmMoldMax = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max;
                                    CData.Parts[(int)EPart.INR].dBtmMoldAvg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg;
                                }
                                // 2021.08.03 lhs End

                                m_iBcrCnt = 0;

                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker; //20200805 syc : ASE KR VOC SECS/GEM (11)
                                CData.Parts[(int)EPart.ONP].iStat = m_iPreState; //20200805 syc : ASE KR VOC SECS/GEM (11)

                                
                                
                                // 2021.08.03 lhs Start : Top/Btm Mold 데이터
                                if (CDataOption.UseNewSckGrindProc)
                                {
                                    CLog.mLogGnd(string.Format("CSQ3_OnP Step - 30 OnCarrierVerifyRequset sBcr={0} finish. Pcb Min={1}, Max={2}, Avg={3}, DF user mode={4}, TopMoldMax={5}, TopMoldAvg={6}, BtmMoldMax={7}, BtmMoldAvg={8}, ", 
                                                                            CData.Bcr.sBcr,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg) );
                                }
                                else // 기존 로직
                                // 2021.08.03 lhs End
                                {
                                    CLog.mLogGnd(string.Format("CSQ3_OnP Step - 30 OnCarrierVerifyRequset {0} finish {1},{2},{3},{4}",
                                                                            CData.Bcr.sBcr,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],    //<== //20200526 jhc : Cyl_OnpOriBcr() 부분과 통일시킴 //CData.JSCK_Gem_Data[4].fMeasure_New_Max_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode ) );   //<== //20200526 jhc : Cyl_OnpOriBcr() 부분과 통일시킴 //CData.JSCK_Gem_Data[4].fMeasure_New_Avr_Data[0], CData.JSCK_Gem_Data[4].nDF_User_New_Mode));
                                }
                                _SetLog("Finish.  DF verify success.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.ReqError)/*-1*/)       {   CErr.Show(eErr.HOST_NOT_REQUEST_ERROR);                 m_iStep = 0;    return true;    }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotError)/*-2*/)       {   CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);        m_iStep = 0;    return true;    }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripError)/*-3*/)     {   CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);      m_iStep = 0;    return true;    }
                        else if (m_Delay.Chk_Delay())
                        {
                            if      (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.InitVerify)/*1*/)  {   CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);        m_iStep = 0;    return true;    }
                            else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotSuccess)/*2*/)  {   CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);      m_iStep = 0;    return true;    }
                            else                                                                                                        {   CErr.Show(eErr.HOST_STRIP_VERIFY_HOLD_TIME_OVER_ERROR); m_iStep = 0;    return true;    }
                        }

                        return false;
                    }

                //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                case 40: //2) Wait Z, Y => WAIT_POS
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                        _SetLog("Camera Move Error Pos. : 2) Y axis move Wait.", CData.SPos.dONP_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 41: //3) Wait Y, X => ERROR_POS
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_BcrErr);
                        _SetLog("Camera Move Error Pos. : 3) X axis move BCR Error Pos.", CData.Dev.dOnp_X_BcrErr);

                        m_iStep++;
                        return false;
                    }

                case 42: //4) Wait X, Y => ERROR_POS
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_BcrErr))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_BcrErr);
                        _SetLog("Camera Move Error Pos. : 4) Y axis move BCR Error Pos.", CData.Dev.dOnp_Y_BcrErr);

                        m_iStep++;
                        return false;
                    }

                case 43: //5) Wait Y, Z => ERROR_POS
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_BcrErr))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_BcrErr);
                        _SetLog("Camera Move Error Pos. : 5) Z axis move BCR Error Pos.", CData.Dev.dOnp_Z_BcrErr);

                        m_iStep++;
                        return false;
                    }

                case 44: //6) Wait Z, Alarm
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_BcrErr))
                        { return false; }
                   
                        CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);

                        _SetLog("Error : 2D Reading Fail.  Key-in at ERROR_POS.");

                        m_iBcrCnt = 0;
                        m_iStep = 0;
                        return true;
                    }
                //.. 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            }
        }

        //20190604 ghk_onpbcr
        public bool Cyl_OnpOri()
        {
            //Timeout check
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_ORI_TIMEOUT);
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
                                _SetLog("2D Vision run check.");

                                m_iStep = 0;
                                return true;
                            }
                            //2D Vision SW Alarm 체크
                            else if ((CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                            {
                                CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                                _SetLog("2D Vision alarm.");
                                CBcr.It.b2DVisAlarmNotify = false;
                                CBcr.It.s2DVisAlarmMsg = "";

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else if (!CBcr.It.Chk_Run())
                        {
                            CErr.Show(eErr.INRAIL_BARCODE_NOT_READY);
                            _SetLog("Error : BCR no run.");

                            m_iStep = 0;
                            return true;
                        }
                        CBcr.It.Load();
                        if(CData.Bcr.bStatusBcr)
                        {
                            CErr.Show(eErr.BCR_NOT_READY);
                            _SetLog("Error : BCR not ready.");

                            m_iStep = 0;
                            return true;
                        }
                        CData.Bcr.sBcr = "";

                        _SetLog("Check BCR run.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Picker 0 turn
                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }
                case 14:
                    {//Picker 0 turn
                        if (!Func_Picker0())
                        { return false; }
                        _SetLog("Check picker 0.");

                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        if (GV.ORIENTATION_ONE_TIME_SKIP && CData.Dev.bOriOneTimeSkipUse && CData.bOriOneTimeSkip)
                        {
                            CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                            CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화

                            CData.Bcr.bRetOri = true;
                            CData.Bcr.bRetMark = true;

                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);

                            _SetLog("Orientation One Time Skip.  Y axis move wait.", CData.SPos.dONP_Y_Wait);

                            m_iStep = 21;
                            return false;
                        }
                        //

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//X축 오리엔테이션 측정 위치 이동
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Ori);
                        _SetLog("X axis move orientation.", CData.Dev.dOnp_X_Ori);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//X축 오리엔테이션 측정 위치 이동 확인, Z축 오리엔테이션 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Ori))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Ori);
                        _SetLog("Z axis move orientation.", CData.Dev.dOnp_Z_Ori);

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//Z축 오리엔테이션 측정 위치 이동 확인, Y축 오리엔테이션 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Ori))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Ori);
                        _SetLog("Y axis move orientation.", CData.Dev.dOnp_Y_Ori);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Y축 오리엔테이션 측정 위치 이동 확인, 오리엔테이션 체크
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Ori))
                        { return false; }

                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Ori();
                        if (CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Ori();
                        else CBcr.It.Chk_Ori_InR();

#if true //20200401 jhc : 2D-Vision Delay Time 1초
                        if (CDataOption.BcrInterface == eBcrInterface.Udp)
                        {
                            m_Delay.Set_Delay(1000); //1초 후부터 실시간 상태 체크
                            _SetLog("Set delay : 1000ms");
                        }
                        else
                        {
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
                            _SetLog("Set delay : 5000ms");
                        }
#else
                        m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif

                        //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
                        if ((CData.m_sBcrLog.Length >= CBcr.ERROR_UDP_REQ.Length) && (CData.m_sBcrLog.Substring(0, CBcr.ERROR_UDP_REQ.Length) == CBcr.ERROR_UDP_REQ))
                        { _SetLog("BCR Log : " + CData.m_sBcrLog); }

                        m_iStep++;
                        return false;
                    }

                case 19:
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
#if true //20200402 jhc : 2D-Vision Retry 추가 Delay Time 1초
                            if (CDataOption.BcrInterface == eBcrInterface.Udp)
                            {
                                m_Delay.Set_Delay(1000); //Retry 추가 Delay 1초
                                _SetLog("Set delay : 1000ms");
                            }
                            else
                            {
                                m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
                                _SetLog("Set delay : 5000ms");
                            }
#else
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif
                                return false;
                        }

                        m_iBcrCnt++;

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//바코드 읽은 값 확인, 바코드 값 저장, Y축 대기 위치 이동
                        //if(CData.CurCompany == eCompany.Qorvo)
                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {
                            if(!CData.Dev.bOriMarkedSkip)
                            {
                                if (!CData.Bcr.bRetOri || !CData.Bcr.bRetMark)
                                {
                                    if (m_iBcrCnt > 3)
                                    {
                                        CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                        _SetLog("Error : Orientation reading fail.");
                                        m_iBcrCnt = 0;

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
                                        _SetLog("Loop.  Count : " + m_iBcrCnt);

                                        m_iStep = 16;
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!CData.Bcr.bRetOri)
                                {
                                    if (m_iBcrCnt > 3)
                                    {
                                        CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                        _SetLog("Error : Orientation reading fail.");
                                        m_iBcrCnt = 0;

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
                                        _SetLog("Loop.  Count : " + m_iBcrCnt);

                                        m_iStep = 16;
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!CData.Bcr.bRetOri)
                            {
                                if (m_iBcrCnt > 3)
                                {
                                    CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                    _SetLog("Error : Orientation reading fail.");
                                    m_iBcrCnt = 0;

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
                                    _SetLog("Loop.  Count : " + m_iBcrCnt);

                                    m_iStep = 16;
                                    return false;
                                }
                            }
                        }
                        
                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                        CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화
                        //

                        CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                        _SetLog("Orientation success.  Y axis move wait.", CData.SPos.dONP_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//Y축 대기위치 이동 확인, Z축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//Z축 대기 위치 이동 확인, X축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

#if true //20200407 jhc : OnPicker L-Table Pickup 대기 위치 변경
                        if ((CData.Dev.bDual == eDual.Dual) && (m_iPreState == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        //if(CSQ_Main.It.m_iStat == EStatus.Auto_Running && CData.Dev.bDual == eDual.Dual && m_iPreState == ESeq.ONP_PickTbL)
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);
                            _SetLog("X axis move wait pick left.", CData.SPos.dONP_X_WaitPickL);

                            m_iStep = 25;
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);

                            m_iStep++;
                        }
#else
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iX.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOnP_X_Wait;
                        SaveLog();

                        m_iStep++;
#endif
                        return false;
                    }

                case 23:
                    {//Picker 90 turn
                        Func_Picker90();
                        _SetLog("Picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//Picker 0 turn
                        if (!Func_Picker90())
                        { return false; }
                        _SetLog("Check picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//X축 대기 위치 이동 확인, 시퀀스 상태 변경
#if true //20200407 jhc : OnPicker L-Table Pickup 대기 위치 변경
                        if ((CData.Dev.bDual == eDual.Dual) && (m_iPreState == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        //if(CSQ_Main.It.m_iStat == EStatus.Auto_Running && CData.Dev.bDual == eDual.Dual && m_iPreState == ESeq.ONP_PickTbL)
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL))
                            { return false; }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }
                        }
#else
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                        { return false; }
#endif
                                                
                        //CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;
                        CData.Parts[(int)EPart.ONP].iStat = m_iPreState;

                        // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                        //if (CData.Opt.bSecsUse && CData.GemForm != null) CData.GemForm.Strip_Data_Shift(3, 4);// DF Data -> BCR Data 로 이동
                        if (CData.Opt.bSecsUse && CData.GemForm != null)
                        {
                            CData.GemForm.Strip_Data_Shift((int)EDataShift.INR_DF_MEAS/*3*/, (int)EDataShift.INR_BCR_READ/*4*/);// DF Data -> BCR Data 로 이동
                        }
                        // 2021.07.19 SungTae End

                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.ONP].iStat);
                        m_iBcrCnt = 0;

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_ConversionWait()
        {//20190822 pjh : OnpConvWait
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_WAIT_TIMEOUT);
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
                    {
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                            return false;
                        }

                        Func_Eject(false);
                        Func_Picker90();
                        _SetLog("Eject off.  Picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Func_Picker90())
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                        _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                        { return false; }

                        _SetLog("Check X axis move.");                        

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//On Picker X Wait 이동 확인 후 Probe Up
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                        { return false; }

                        CIO.It.Set_Y(eY.GRDL_ProbeDn, false);
                        CIO.It.Set_Y(eY.GRDL_ProbeAir, false);
                        CIO.It.Set_Y(eY.GRDR_ProbeDn, false);
                        CIO.It.Set_Y(eY.GRDR_ProbeAir, false);
                        _SetLog("GDL, GDR Probe up.");

                        m_iStep++;
                        return false;
                    } 

                case 15:
                    {//Probe Up 확인 후 Left Grind Z Wait 이동
                        if (CIO.It.Get_Y(eY.GRDL_ProbeDn) && CIO.It.Get_Y(eY.GRDR_ProbeDn))
                        { return false; }

                        CMot.It.Mv_N(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]);
                        _SetLog("GDL Z axis move wait.", CData.SPos.dGRD_Z_Wait[0]);
                        CMot.It.Mv_N(m_iRGZ, CData.SPos.dGRD_Z_Wait[1]);
                        _SetLog("GDR Z axis move wait.", CData.SPos.dGRD_Z_Wait[1]);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//Left Grind Z Wait 확인 후 Right Grind Z Wait 이동
                        if (!CMot.It.Get_Mv(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iRGZ, CData.SPos.dGRD_Z_Wait[0]))
                        { return false; }

                        _SetLog("Check GDL, GDR Z axis move.");

                        m_iStep++;                        
                        return false;
                    }

                case 17:
                    {//Right Grind Z Wait 확인 후 Top Cleaner Up
                        //syc : new cleaner
                        //CIO.It.Set_Y(eY.GRDL_TopClnDn, false);
                        //CIO.It.Set_Y(eY.GRDR_TopClnDn, false);
                        CData.L_GRD.ActTcDown(false);
                        CData.R_GRD.ActTcDown(false);

                        _SetLog("GDL, GDR Top cleaner up.");

                        m_iStep++;                        
                        return false;
                    }

                case 18:
                    {//Top Cleaner Up 확인 후 Left Grind Y Wait 이동
                        //syc : new cleaner
                        //if (CIO.It.Get_Y(eY.GRDL_TopClnDn) && CIO.It.Get_Y(eY.GRDR_TopClnDn))
                        if (CData.L_GRD.Chk_CleanerDn(false) && CData.R_GRD.Chk_CleanerDn(false))
                        { return false; }

                            CMot.It.Mv_N(m_iLTY, CData.SPos.dGRD_Y_Wait[0]);
                        _SetLog("GDL Y axis move wait.", CData.SPos.dGRD_Y_Wait[0]);
                        CMot.It.Mv_N(m_iRTY, CData.SPos.dGRD_Y_Wait[1]);
                        _SetLog("GDR Y axis move wait.", CData.SPos.dGRD_Y_Wait[1]);

                        m_iStep++;                        
                        return false;
                    }

                case 19:
                    {//Left Grind Y Wait 확인 후 Right Grind Y Wait 이동
                        if (!CMot.It.Get_Mv(m_iLTY, CData.SPos.dGRD_Y_Wait[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iRTY, CData.SPos.dGRD_Y_Wait[1]))
                        { return false; }

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :    
                        _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }            
        }

#endregion
		//190819 ksg :
		public bool Cyl_OnpOriBcr()
		{
            //Timeout check
            if (m_iPreStep != m_iStep)
			{
#if true //20200427 jhc : SECG/GEM 사용 시 BCR/ORI Timeout 시간 연장
				if (CData.Opt.bSecsUse)
				{ m_TiOut.Set_Delay(SECSGEM_BCR_TIMEOUT); }
				else
				{ m_TiOut.Set_Delay(TIMEOUT); }
#else
                m_TiOut.Set_Delay(TIMEOUT);
#endif
			}
			else
			{
				if (m_TiOut.Chk_Delay())
				{

					//201230 jhc : Orientation/BCR Timeout 구분 처리
					if ((10 < m_iStep) && (m_iStep < 21))
					{
						CErr.Show(eErr.ONLOADERPICKER_ORI_TIMEOUT);
						_SetLog("Error : Orientation Timeout.");
					}
					else
					{
						CErr.Show(eErr.ONLOADERPICKER_BCR_TIMEOUT);
						_SetLog("Error : BCR Timeout.");
					}
					//..

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
                //190617 ksg :
                case 10:
                    {
                        if(m_bBcrView)
                        {
                            if(m_bBcrKeyIn)
                            {
                                m_bBcrView = false;
                                _SetLog("Key-in.");

                                m_iStep = 27;
                                return false;
                            }
                            else if(m_bBcrErr)
                            {
                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                _SetLog("Error : BCR Reading fail 1.");
                                m_iBcrCnt  = 0;
                                m_iStep    = 0;
                                m_bBcrView = false;

                                m_iStep    = 0;
                                return true;
                            }
                            else
                            {
                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                _SetLog("Error : BCR Reading fail 2.");
                                m_iBcrCnt  = 0;
                                m_iStep    = 0;
                                m_bBcrView = false;

                                m_iStep    = 0;
                                return true;
                            }
                        }

                        CBcr.It.Save_RunStatus();
                        m_Delay.Set_Delay(1000);

                        //190801 ghk_Keyin
                        m_bBcrKeyIn = false;
                        m_bBcrErr = false;
                        m_bBcrView = false;
                        _SetLog("Set delay : 1000ms");

                        m_iStep++; 
                        return false;
                    }

                case 11: 
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }
                        m_bSecondBcr = false;
                        m_bBcrView = false;
                        _SetLog("Check delay.");

                        // 2021.08.23 SungTae : [TEMP] Multi-LOT 관련 Debugging Test 시 임시로 변경
                        /*
                         * true : Auto Run 진행 시
                         * false : Debugging으로 Dry Run 진행 시
                         */
#if true
                        m_iStep++;
#else
                        m_iStep = 27;
#endif
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
                            _SetLog("Error : Check run.");

                            m_iStep = 0;
                            return true;
                        }
                        //
                        CBcr.It.Load();
                        if(CData.Bcr.bStatusBcr)
                        {
                            CErr.Show(eErr.BCR_NOT_READY);
                            _SetLog("Error : BCR not ready.");

                            m_iStep = 0;
                            return true;
                        }

                        CData.Bcr.sBcr = "";
                        _SetLog("BCR run check.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Picker 0 turn
                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Picker 0 turn
                        if (!Func_Picker0())
                        { return false; }
                        _SetLog("Check picker 0.");

                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        if (GV.ORIENTATION_ONE_TIME_SKIP && CData.Dev.bOriOneTimeSkipUse && CData.bOriOneTimeSkip)
                        {
                            CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                            CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화

                            CData.Bcr.bRetOri = true;
                            CData.Bcr.bRetMark = true;

                            _SetLog("Orientation One Time Skip.  Y axis move wait.", CData.SPos.dONP_Y_Wait);

                            m_iStep = 21;
                            return false;
                        }
                        //

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//X축, Y축 오리엔테이션 측정 위치 이동
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Ori);
                        _SetLog("X axis move orientation.", CData.Dev.dOnp_X_Ori);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Ori);
                        _SetLog("Y axis move orientation.", CData.Dev.dOnp_Y_Ori);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//X축 오리엔테이션 측정 위치 이동 확인, Z축 오리엔테이션 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Ori))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Ori);
                        _SetLog("Z axis move orientation.", CData.Dev.dOnp_Z_Ori);

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//Z축 오리엔테이션 측정 위치 이동 확인, Y축 오리엔테이션 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Ori))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Ori))
                        { return false; }

                        _SetLog("Check Y, Z axis move.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Y축 오리엔테이션 측정 위치 이동 확인, 오리엔테이션 체크
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Ori();
                        if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Ori();
                        else                                       CBcr.It.Chk_Ori_InR();
                        
                        _SetLog("BCR log : " + CData.m_sBcrLog);

#if true //20200401 jhc : 2D-Vision Delay Time 1초
                        if (CDataOption.BcrInterface == eBcrInterface.Udp)
                        {
                            m_Delay.Set_Delay(1000); //1초 후부터 실시간 상태 체크
                            _SetLog("Set delay : 1000ms");
                        }
                        else
                        {
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
                            _SetLog("Set delay : 5000ms");
                        }
#else
                        m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif
                        m_Delay1.Set_Delay(50000);

                        m_iStep++;
                        return false;
                    }

                case 19:
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
                        //
                        CBcr.It.Load();
                        //191031 ksg :
                        if(m_Delay1.Chk_Delay())
                        {
                            CErr.Show(eErr.BCR_REPLY_TIMEOUT);
                            _SetLog("Error : BCR reply timeout.");
                            m_iBcrCnt = 0;

                            m_iStep = 0;
                            return true;
                        }
                        if(CData.Bcr.bStatusBcr)
                        {
#if true //20200402 jhc : 2D-Vision Retry 추가 Delay Time 1초
                            if (CDataOption.BcrInterface == eBcrInterface.Udp) m_Delay.Set_Delay(1000); //Retry 추가 Delay 1초
                            else                                               m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#else
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif
                            return false;
                        }

                        _SetLog("BCR load.  Count : " + m_iBcrCnt);
                        m_iBcrCnt++;

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//바코드 읽은 값 확인, 바코드 값 저장, Y축 대기 위치 이동
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {
                            if(!CData.Dev.bOriMarkedSkip)
                            {
                                if (!CData.Bcr.bRetOri || !CData.Bcr.bRetMark)
                                {
                                    if (m_iBcrCnt > 3)
                                    {
                                        CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                        _SetLog("Error : Orientation reading fail.");
                                        m_iBcrCnt = 0;

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
                                        _SetLog("Loop.  Count : " + m_iBcrCnt);

                                        m_iStep = 16;
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!CData.Bcr.bRetOri)
                                {
                                    if (m_iBcrCnt > 3)
                                    {
                                        CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                        _SetLog("Error : Orientation reading fail.");
                                        m_iBcrCnt = 0;

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
                                        _SetLog("Loop.  Count : " + m_iBcrCnt);

                                        m_iStep = 16;
                                        return false;
                                    }
                                }
                            }                            
                        }
                        else
                        {
                            if (!CData.Bcr.bRetOri)
                            {
                                if (m_iBcrCnt > 3)
                                {
                                    CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                    _SetLog("Error : Orientation reading fail.");
                                    m_iBcrCnt = 0;

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
                                    _SetLog("Loop.  Count : " + m_iBcrCnt);

                                    m_iStep = 16;
                                    return false;
                                }
                            }
                        }
                        
                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                        CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화
                        //

                        m_iBcrCnt = 0;
                        _SetLog("Orientation.");

                        m_iStep++;
                        return false;
                    }

               case 21:
                    {//X축, Y축 바코드 측정 위치 이동
                        if(!m_bSecondBcr)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Bcr);
                            _SetLog("X axis move bcr.", CData.Dev.dOnp_X_Bcr);
                            CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Bcr);
                            _SetLog("Y axis move bcr.", CData.Dev.dOnp_Y_Bcr);
                        }
                        else
                        {//X축, Y축 바코드 Second 측정 위치 이동
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_BcrSecon);
                            _SetLog("X axis move 2nd bcr.", CData.Dev.dOnp_X_BcrSecon);
                            CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_BcrSecon);
                            _SetLog("Y axis move 2nd bcr.", CData.Dev.dOnp_Y_BcrSecon);
                        }

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//X축 바코드 측정 위치 이동 확인, Z축 바코드 측정 위치 이동
                        if(!m_bSecondBcr)
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Bcr))
                            { return false; }
                        }
                        else
                        {
                            //X축 바코드 Second 측정 위치 이동 확인, Z축 바코드 측정 위치 이동
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_BcrSecon))
                            { return false; }
                        }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Bcr);
                        _SetLog("Z axis move bcr.", CData.Dev.dOnp_Z_Bcr);

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {//Z축 바코드 측정 위치 이동 확인, Y축 바코드 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Bcr)) return false;
                        if(!m_bSecondBcr)
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Bcr)) return false;
                        }
                        else
                        {
                            //Y축 바코드 Second 측정 위치 이동 확인, Y축 바코드 측정 위치 이동
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_BcrSecon)) return false;
                        }


                        _SetLog("Check Y, Z axis move.");

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//Y축 바코드 측정 위치 이동 확인, 바코드 읽기
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Bcr();
                        if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Bcr();
                        else                                       CBcr.It.Chk_Bcr_InR();

#if true //20200401 jhc : 2D-Vision Delay Time 1초
                        if (CDataOption.BcrInterface == eBcrInterface.Udp)
                        {
                            m_Delay.Set_Delay(1000); //1초 후부터 실시간 상태 체크
                            _SetLog("Set delay : 1000ms");
                        }
                        else
                        {
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
                            _SetLog("Set delay : 5000ms");
                        }
#else
                        m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif
                        //m_iBcrCnt++;

                        m_Delay1.Set_Delay(50000); //191031 ksg : <== //20200526 jhc : 누락 부분 추가

                        //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
                        if ((CData.m_sBcrLog.Length >= CBcr.ERROR_UDP_REQ.Length) && (CData.m_sBcrLog.Substring(0, CBcr.ERROR_UDP_REQ.Length) == CBcr.ERROR_UDP_REQ))
                        { _SetLog("BCR log : " + CData.m_sBcrLog); }

                        
                        m_iStep++;
                        return false;
                    }

                case 25:
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
                        //
                        CBcr.It.Load();
                        //191031 ksg : <== //20200526 jhc : 누락 부분 추가
                        if(m_Delay1.Chk_Delay())
                        {
                            CErr.Show(eErr.BCR_REPLY_TIMEOUT);
                            _SetLog("Error : BCR reply timeout.");
                            m_iBcrCnt = 0;

                            m_iStep = 0;
                            return true;
                        }
                        if(CData.Bcr.bStatusBcr)
                        {
#if true //20200402 jhc : 2D-Vision Retry 추가 Delay Time 1초
                            if (CDataOption.BcrInterface == eBcrInterface.Udp) m_Delay.Set_Delay(1000); //Retry 추가 Delay 1초
                            else                                               m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#else
                            m_Delay.Set_Delay(5000); //191107 ksg : 1초 -> 5초로 변경함.
#endif
                            return false;
                        }

                        m_iBcrCnt++;
                        _SetLog("Bcr count : " + m_iBcrCnt);
                        
                        m_iStep++;
                        return false;
                    }

                case 26:
                    {//바코드 읽은 값 확인, 바코드 값 저장, Y축 대기 위치 이동
                        if (!CData.Bcr.bRet || CData.Bcr.sBcr == "error")
                        {
                            if (m_iBcrCnt > 3)
                            {
                                if(!CData.Dev.bBcrSecondSkip && !m_bSecondBcr)
                                {
                                    _SetLog("2nd skip.");
                                    m_bSecondBcr = true;
                                    m_iBcrCnt    = 0   ;

                                    m_iStep      = 21  ;
                                    return false;
                                }
                                if (CData.Dev.bBcrKeyInSkip)
                                {
                                    CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                    _SetLog("Error : BCR reading fail.  Key-in skip.");
                                    m_iBcrCnt = 0; //190211 ksg :

                                    m_iStep   = 0;
                                    return true;
                                }
                                else
                                {
                                    m_bBcrView = true;
                                    if(CDataOption.KeyInType == eBcrKeyIn.Stop)
                                    {
                                        //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                                        if (CDataOption.Use2DErrorPosition && (CDataOption.CurEqu == eEquType.Pikcer))
                                        {
                                            //1) Z => WAIT_POS
                                            CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                                            _SetLog("Error : 2D Reading Fail.  Camera Move Error Pos. : 1) Z axis move wait.", CData.SPos.dONP_Z_Wait);

                                            m_iBcrCnt = 0;
                                            m_iStep = 40; //제3의 위치(OCR 위치)로 Picker(카메라) 이동 후 Alarm 발생
                                            return false;
                                        }
                                        else
                                        {
                                            //기존 루틴 : Alarm 발생 후 종료 (Reset => Key In => Start하면 입력한 2D Code로 동작)
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : 2D Reading Fail.  Key-in STOP.");

                                            m_iBcrCnt = 0;
                                            m_iStep = 0;
                                            return true;
                                        }
                                        //..
                                    }
                                    else
                                    {
                                        if (m_bBcrErr)
                                        {
                                            CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                            _SetLog("Error : BCR reading fail.  Key-in error.");
                                            m_iBcrCnt  = 0;                                            
                                            m_bBcrView = false;

                                            m_iStep = 0;
                                            return true;
                                        }
                                        if (m_bBcrKeyIn)
                                        {
                                            m_bBcrView = false;
                                        }
                                        if (!m_bBcrErr && !m_bBcrKeyIn)
                                        {
                                            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                                            if (CDataOption.Use2DErrorPosition && (CDataOption.CurEqu == eEquType.Pikcer))
                                            {
                                                //1) Z => WAIT_POS
                                                CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                                                _SetLog("Error : 2D Reading Fail.  Camera Move Error Pos. : 1) Z axis move wait.", CData.SPos.dONP_Z_Wait);

                                                m_iBcrCnt = 0;
                                                m_iStep = 40; //제3의 위치(OCR 위치)로 Picker(카메라) 이동 후 Alarm 발생
                                                return false;
                                            }
                                            else
                                            {
                                                //기존 루틴 : Timeout 시간 동안 Key In 대기 (여기에서 Key In 하지않고 가만히 있으면 Timeout 발생하게 됨!)
                                                return false;
                                            }
                                            //..
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _SetLog("Loop.  Count : " + m_iBcrCnt);

                                //m_iStep = 16;
                                m_iStep = 24;
                                return false;
                            }
                        }

                        _SetLog("BCR success.");
                        m_iStep++;
                        return false;
                    }

                case 27:
                    {   
                        //20190703 ghk_dfserver
                        //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == eStatus.Auto_Running)
                        if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            CData.dfInfo.sBcr = CData.Bcr.sBcr;
                            if (!CData.dfInfo.bBusy)
                            {
                                CDf.It.SendInrBcr();
                                _SetLog("DF Server send bcr.");

                                m_iStep++;
                                return false;
                            }
                            else
                            { return false; }
                        }
                        else
                        {
                            CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sBcr;
                            
                            /*
                            if (CData.Dev.bOriSkip)
                            {
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;

                                CData.Parts[(int)EPart.ONP].iStat = m_iPreState;

                                m_LogVal.sMsg += ", Bcr = " + CData.Bcr.sBcr;
                                SaveLog();

                                m_iStep = 0;
                                m_iBcrCnt = 0;

                                return true;
                            }
                            */

                            //191118 ksg : <== //20200526 jhc : 누락 부분 추가
                            if (CData.GemForm != null)
                            {
                                //CData.JSCK_Gem_Data[3].sInRail_Strip_ID = CData.Bcr.sBcr;
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = CData.Bcr.sBcr;
                            }

                            _SetLog("BCR : " + CData.Bcr.sBcr);

                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);

                            m_iStep = 31;
                            return false;
                        }
                    }
                
                case 28:
                    {
                        if (CDf.It.ReciveInrBcr() == 1)
                        {
                            CErr.Show(eErr.DYNAMIC_FUNCTION_SERVER_NOTHING_GL1_DATA);
                            _SetLog("Error : DF Server nothing GL 1 data.");
                            m_iBcrCnt = 0;

                            m_iStep = 0;
                            return true;
                        }
                        else if (CDf.It.ReciveInrBcr() == 0)
                        {
                            _SetLog("DF Server receive bcr");
                            m_iStep++;
                        }

                        return false;
                    }

                case 29:
                    {
                        if(!CData.dfInfo.bBusy)
                        {
                            CDf.It.SendResultBcr();
                            _SetLog("DF Server send result.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 30:
                    {
                        if(CDf.It.ReciveResultBcr())
                        {
                            //20200526 jhc : 누락 부분 추가 (DF 측정 옵션으로 구분 처리)
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
                                CData.Parts[(int)EPart.INR].sBcr = CData.Bcr.sBcr;
                                CData.Parts[(int)EPart.INR].dDfMax = CData.dfInfo.dBcrMax;
                            }

                            _SetLog("BCR : " + CData.Bcr.sBcr);

                            CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                            _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);

                            m_iStep++;
                        }
                        return false;
                    }

                case 31:
                    {//Y축 대기위치 이동 확인, Z축 대기 위치 이동, 여기로 점프
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 32:
                    {//Z축 대기 위치 이동 확인, X축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

#if true //20200407 jhc : OnPicker L-Table Pickup 대기 위치 변경
                        if ((CData.Dev.bDual == eDual.Dual) && (m_iPreState == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        //if(CSQ_Main.It.m_iStat == EStatus.Auto_Running && CData.Dev.bDual == eDual.Dual && m_iPreState == ESeq.ONP_PickTbL)
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dONP_X_WaitPickL);
                            _SetLog("X axis move wait pick left.", CData.SPos.dONP_X_WaitPickL);

                            m_iStep = 35;
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                            _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);

                            m_iStep++;
                        }
#else
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iX.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOnP_X_Wait;
                        SaveLog();

                        m_iStep++;
#endif
                        return false;
                    }

                case 33:
                    {//Picker 90 turn
                        Func_Picker90();
                        _SetLog("Picker 90.");

                        m_iStep++;
                        return false;
                    }
                case 34:
                    {//Picker 0 turn
                        if (!Func_Picker90())
                        { return false; }
                        _SetLog("Check picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 35:
                    {//X축 대기 위치 이동 확인, 시퀀스 상태 변경 
#if true //20200407 jhc : OnPicker L-Table Pickup 대기 위치 변경
                        if ((CData.Dev.bDual == eDual.Dual) && (m_iPreState == ESeq.ONP_PickTbL)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        //if(CSQ_Main.It.m_iStat == EStatus.Auto_Running && CData.Dev.bDual == eDual.Dual && m_iPreState == ESeq.ONP_PickTbL)
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dONP_X_WaitPickL))
                            { return false; }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                            { return false; }
                        }
#else
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                        { return false; }
#endif

                        if(!CData.Opt.bSecsUse) //191125 ksg :
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;    //20200805 syc : ASE KR VOC SECS/GEM (11)
                            CData.Parts[(int)EPart.ONP].iStat = m_iPreState;            //20200805 syc : ASE KR VOC SECS/GEM (11)

                            // SECSGEM 사용 하지 않으면 측정 값 대입 LCY 200131
                            // SECSGEM 사용과 무관 하게 fMeasure_New_Avr_Data 값 사용 하면 됨
                            if (CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Min_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0];
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0];
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0];
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode        = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].nDF_User_Mode;

                                CLog.mLogGnd(string.Format("CSQ3_OnP Step - 35 fMeasure_New_Min_Data {0},{1},{2} , Mode {3}",
                                                                          CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Min_Data[0],
                                                                          CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0],
                                                                          CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0],
                                                                          CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));
                            }

                            m_iBcrCnt = 0;
                            _SetLog("Finish.  SECS/GEM not use.");
                            //210928 syc :2004U
                            _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.ONP].iStat);

                            m_iStep = 0;
                            return true;
                        }
                        else  // SecsGem 사용
                        {
                            //999200    carrier verify request
                            if (CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = CData.Bcr.sBcr;
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.INR_DF_MEAS/*3*/, (int)EDataShift.INR_BCR_READ/*4*/);// DF Data -> BCR Data 로 이동
                                                                                                                // 
                                CData.GemForm.OnCarrierVerifyRequset(CData.Bcr.sBcr);
                                CLog.mLogGnd(string.Format("CSQ3_OnP Step - 35 OnCarrierVerifyRequset {0}", CData.Bcr.sBcr));
                            }

                            // 2022.07.01 SungTae Start : [추가] Log 추가
                            _SetLog($"[SEND](H<-E) S6F11 CEID = {Convert.ToInt32(SECSGEM.JSCK.eCEID.Carrier_Verify_Request)}({SECSGEM.JSCK.eCEID.Carrier_Verify_Request}).  Carrier ID : {CData.Bcr.sBcr}, Set Delay : 180 sec");
                            // 2022.07.01 SungTae End

                            m_Delay.Set_Delay(3 * 60 * 1000); //20200406 jhc : SECSGEM Delay Time 5 m -> 15 sec //m_Delay.Set_Delay(300000)
                            _SetLog("SECS/GEM use.  Set delay : " + (3 * 60 * 1000) + "ms");

                            m_iStep++;
                            return false;
                        }
                        //return true;
                    }
                case 36:
                    {
                        m_TiOut.Set_Delay(TIMEOUT); // SECSGEM 사용시 Host 에서 Reply 시간 3분 이상 으로 시퀸스 Timeout 는 사용 하지 않기 위해 지속 적으로 초기화 진행 LCY 20717
                        
                        if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripSuccess)/*3*/)
                        {
                            // SECSGEM 사용 하면 Host 값 대입 LCY 200131
                            // 여기에서 값 Verify 진행 필요.

                            //-----------------------------------
                            // 2022.02.15 lhs Start : SecsGem에서 Download 값 범위 체크, Error 표시
                            //------------------
                            // 수정전
                            //if ((CData.Dynamic.dPcbMeanRange > 0)   && //200806 jhc : Range 범위 값이 0인 경우 무시
                            //(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] > (CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbMeanRange) ||
                            // CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] < (CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbMeanRange))) 
                            //{
                            //    CLog.mLogGnd(string.Format("CSQ3_OnP Step - 36 Host Download : INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER  New Avr = {0}, [Device Setting] Pcb thickness = {1}, Mean Range = {2} : {3} ~ {4}",
                            //                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0], 
                            //                CData.Dev.aData[0].dPcbTh , CData.Dynamic.dPcbMeanRange,
                            //                CData.Dev.aData[0].dPcbTh + CData.Dynamic.dPcbMeanRange, CData.Dev.aData[0].dPcbTh - CData.Dynamic.dPcbMeanRange));  // 2021.12.17 lhs 체크하는 변수와 로그표시 변수가 같지 않아 수정 : dPcbRang -> dPcbMeanRange

                            //    CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER);
                            //    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;
                            //    m_iStep = 0;
                            //    return true;
                            //}
                            //------------------

                            bool bOver = IsOverRangeSecsGemDownTh();
                            if (bOver)
                            {
                                CData.Parts[(int)EPart.INR].iStat = ESeq.INR_DynamicFail;
                                m_iStep = 0;
                                return true;
                            }
                            // 2022.02.15 lhs End : SecsGem에서 Download 값 범위 체크, Error 표시
                            //-----------------------------------
                            else  // Host 에서 DF Verify 성공 
                            {
                                CData.Parts[(int)EPart.INR].dPcbMin         = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0];
                                CData.Parts[(int)EPart.INR].dPcbMax         = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0];
                                CData.Parts[(int)EPart.INR].dPcbMean        = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0];
                                CData.JSCK_Gem_Data[4].nDF_User_New_Mode    = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_Mode;

                                // 2021.08.03 lhs Start : Top/Btm Mold 데이터
                                if (CDataOption.UseNewSckGrindProc)
                                {
                                    CData.Parts[(int)EPart.INR].dTopMoldMax = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max;
                                    CData.Parts[(int)EPart.INR].dTopMoldAvg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg;
                                    CData.Parts[(int)EPart.INR].dBtmMoldMax = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max;
                                    CData.Parts[(int)EPart.INR].dBtmMoldAvg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg;
                                }
                                // 2021.08.03 lhs End

                                CData.Parts[(int)EPart.INR].iStat           = ESeq.INR_WaitPicker;      //20200805 syc : ASE KR VOC SECS/GEM (11)
                                CData.Parts[(int)EPart.ONP].iStat           = m_iPreState;              //20200805 syc : ASE KR VOC SECS/GEM (11)

                                m_iBcrCnt = 0;
                                // 2021.08.03 lhs Start : Top/Btm Mold 데이터
                                if (CDataOption.UseNewSckGrindProc)
                                {
                                    CLog.mLogGnd(string.Format("CSQ3_OnP Step - 36 OnCarrierVerifyRequset sBcr={0} finish. Pcb Min={1}, Max={2}, Avg={3}, DF user mode={4}, TopMoldMax={5}, TopMoldAvg={6}, BtmMoldMax={7}, BtmMoldAvg={8}, ",
                                                                            CData.Bcr.sBcr,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg));
                                }
                                else // 기존 로직
                                // 2021.08.03 lhs End
                                {
                                    CLog.mLogGnd(string.Format("CSQ3_OnP Step - 36 OnCarrierVerifyRequset {0} finish {1},{2},{3},{4}",
                                                                            CData.Bcr.sBcr,
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],    //<== //20200526 jhc : Cyl_OnpOriBcr() 부분과 통일시킴 //CData.JSCK_Gem_Data[4].fMeasure_New_Max_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));     //<== //20200526 jhc : Cyl_OnpOriBcr() 부분과 통일시킴 //CData.JSCK_Gem_Data[4].fMeasure_New_Avr_Data[0], CData.JSCK_Gem_Data[4].nDF_User_New_Mode));
                                }

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.ReqError)/*-1*/)
                        {
                            CErr.Show(eErr.HOST_NOT_REQUEST_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotError)/*-2*/)
                        {
                            CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.StripError)/*-3*/)
                        {
                            CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        else if (m_Delay.Chk_Delay())
                        {
                            if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.InitVerify)/*1*/)
                            {
                                CErr.Show(eErr.HOST_LOT_VERIFY_TIME_OVER_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(SECSGEM.JSCK.eChkStripVerify.LotSuccess)/*2*/)
                            {
                                CErr.Show(eErr.HOST_STRIP_VERIFY_TIME_OVER_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                CErr.Show(eErr.HOST_STRIP_VERIFY_HOLD_TIME_OVER_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                        }

                        return false;
                    }

                //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                case 40: //2) Wait Z, Y => WAIT_POS
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                        _SetLog("Camera Move Error Pos. : 2) Y axis move Wait.", CData.SPos.dONP_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 41: //3) Wait Y, X => ERROR_POS
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_BcrErr);
                        _SetLog("Camera Move Error Pos. : 3) X axis move BCR Error Pos.", CData.Dev.dOnp_X_BcrErr);

                        m_iStep++;
                        return false;
                    }

                case 42: //4) Wait X, Y => ERROR_POS
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_BcrErr))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_BcrErr);
                        _SetLog("Camera Move Error Pos. : 4) Y axis move BCR Error Pos.", CData.Dev.dOnp_Y_BcrErr);

                        m_iStep++;
                        return false;
                    }

                case 43: //5) Wait Y, Z => ERROR_POS
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_BcrErr))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_BcrErr);
                        _SetLog("Camera Move Error Pos. : 5) Z axis move BCR Error Pos.", CData.Dev.dOnp_Z_BcrErr);

                        m_iStep++;
                        return false;
                    }

                case 44: //6) Wait Z, Alarm
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_BcrErr))
                        { return false; }
                   
                        CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);

                        _SetLog("Error : 2D Reading Fail.  Key-in at ERROR_POS.");

                        m_iBcrCnt = 0;
                        m_iStep = 0;
                        return true;
                    }
                //.. 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            }
        }

        //20191010 ghk_manual_bcr
        public bool Cyl_OnpManOriBcrLeft()
        {
            //Timeout check
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_ORI_TIMEOUT);
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
                    {//오프로더피커 Z축 대기위치 이동
                        CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.SPos.dOFP_Z_Wait);
                        _SetLog("OFP Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        CData.Parts[(int)EPart.GRDL].sBcr = "";

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {//오프로더피커 Z축 대기위치 이동 확인, 오프로더 피커 90도
                        if (!CMot.It.Get_Mv((int)EAx.OffLoaderPicker_Z, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        CSq_OfP.It.Func_Picker90();
                        _SetLog("OFP Picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//오프로더 피커 90도 확인, 오프로더 피커 X축 대기위치 이동
                        if (!CSq_OfP.It.Func_Picker90())
                        { return false; }

                        CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait);
                        _SetLog("OFP X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//오프로더 피커 X축 대기위치 이동 확인, 그라인드 왼쪽 Z축 대기 위치 이동
                        if (!CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait))
                        { return false; }

                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Wait[(int)EWay.L]);
                        _SetLog("GDL Z axis move wait.", CData.SPos.dGRD_Z_Wait[(int)EWay.L]);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//그라인드 왼쪽 Z축 대기 위치 이동 확인, 그라인드 왼쪽 Y축 Ori 위치 이동
                        if (!CMot.It.Get_Mv((int)EAx.LeftGrindZone_Z, CData.SPos.dGRD_Z_Wait[(int)EWay.L]))
                        { return false; }

                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Y, CData.Dev.aGrd_Y_Ori[(int)EWay.L]);
                        _SetLog("GDL Y axis move orientation.", CData.Dev.aGrd_Y_Ori[(int)EWay.L]);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//그라인드 왼쪽 Y축 Ori 위치 이동 확인
                        if (!CMot.It.Get_Mv((int)EAx.LeftGrindZone_Y, CData.Dev.aGrd_Y_Ori[(int)EWay.L]))
                        { return false; }

                        CBcr.It.Save_RunStatus();
                        m_Delay.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        m_iStep++;
                        return false;
                    }
                    
                case 16:
                    {//딜레이 확인
                        if (!m_Delay.Chk_Delay())
                        { return false; }
                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//바코드 프로그램 런 체크
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
                            _SetLog("Error : BCR not ready.");

                            m_iStep = 0;
                            return true;
                        }

                        CData.Bcr.sBcr = "";
                        _SetLog("BCR Ready.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Picker 0 turn
                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//Picker 0 turn, Z축 Wait 이동
                        if (!Func_Picker0())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//Z축 Wait 이동 확인, X축, Y축 오리엔테이션 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Ori_TbL);
                        _SetLog("X axis move left table orientation.", CData.Dev.dOnp_X_Ori_TbL);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Ori_TbL);
                        _SetLog("Y axis move left table orientation.", CData.Dev.dOnp_Y_Ori_TbL);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//X축, Y축 오리엔테이션 측정 위치 이동 확인, Z축 오리엔테이션 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Ori_TbL))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Ori_TbL))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Ori_TbL);
                        _SetLog("Z axis move left table orientation.", CData.Dev.dOnp_Z_Ori_TbL);

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//Z축 오리엔테이션 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Ori_TbL))
                        { return false; }
                        _SetLog("Check Z axis move.");

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {//오리엔테이션 체크
                        if (CDataOption.BcrPro == eBcrProtocol.Old)
                        { CBcr.It.Chk_Ori(); }
                        else
                        { CBcr.It.Chk_Ori_LTable(); }

                        m_Delay.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        if (!m_Delay.Chk_Delay()) return false;
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

                        if (CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;
                        _SetLog("BCR Load.  Count : " + m_iBcrCnt);

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//오리엔테이션 확인, Y축 대기 위치 이동
                        if (!CData.Bcr.bRetOri)
                        {
                            if (m_iBcrCnt > 3)
                            {
                                CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                _SetLog("Error : Orientation reading fail.");
                                m_iBcrCnt = 0;

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
                                _SetLog("Loop.  Count : " + m_iBcrCnt);

                                m_iStep = 23;
                                return false;
                            }
                        }

                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                        CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화
                        //

                        m_iBcrCnt = 0;
                        _SetLog("Loop end.");

                        m_iStep = 32;
                        return false;
                    }

                case 26:
                    {//X축, Y축 바코드 측정 위치 이동
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Bcr_TbL);
                        _SetLog("X axis move left table barcode.", CData.Dev.dOnp_X_Bcr_TbL);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Bcr_TbL);
                        _SetLog("Y axis move left table barcode.", CData.Dev.dOnp_Y_Bcr_TbL);

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {//X축 바코드 측정 위치 이동 확인, Z축 바코드 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Bcr_TbL))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Bcr_TbL))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Bcr_TbL);
                        _SetLog("Z axis move left table barcode.", CData.Dev.dOnp_Z_Bcr_TbL);

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {//Z축 바코드 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Bcr_TbL))
                        { return false; }
                        _SetLog("Check Z axis move.");

                        m_iStep++;
                        return false;
                    }

                case 29:
                    {//바코드 읽기
                        if (CDataOption.BcrPro == eBcrProtocol.Old)
                        { CBcr.It.Chk_Bcr(); }
                        else
                        { CBcr.It.Chk_Bcr_LTable(); }
                        m_Delay.Set_Delay(1000);

                        //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
                        _SetLog("Set delay : 1000ms  BCR Log : " + CData.m_sBcrLog);

                        m_iStep++;
                        return false;
                    }

                case 30:
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

                        if (CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;
                        _SetLog("BCR Load.  Count : " + m_iBcrCnt);

                        m_iStep++;
                        return false;
                    }

                case 31:
                    {//바코드 확인. 바코드 값 저장
                        if (!CData.Bcr.bRet || CData.Bcr.sBcr == "error")
                        {
                            if (m_iBcrCnt > 3)
                            {
                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                _SetLog("Error : BCR Reading fail.");
                                m_iBcrCnt = 0; 

                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                _SetLog("Loop.  Count : " + m_iBcrCnt);
                                
                                m_iStep = 29;
                                return false;
                            }
                        }

                        CData.Parts[(int)EPart.GRDL].sBcr = CData.Bcr.sBcr;
                        _SetLog("BCR : " + CData.Bcr.sBcr);

                        m_iStep++;
                        return false;
                    }

                case 32:
                    {//Z축 대기위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axsi move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 33:
                    {//Z축 대기위치 이동 확인, Y축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);
                        _SetLog("Y axis move wait.", CData.SPos.dONP_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 34:
                    {//Y축 대기 위치 이동 확인, Picker 0 turn
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 35:
                    {//Picker 0 turn 확인, X축 대기위치 이동
                        if (!Func_Picker0())
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);
                        _SetLog("X axis move wait.", CData.Dev.dOnP_X_Wait);

                        m_iStep++;
                        return false;
                    }
                case 36:
                    {//X축 대기위치 이동 확인, Table Y축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                        { return false; }

                        CMot.It.Mv_N((int)EAx.LeftGrindZone_Y, CData.SPos.dGRD_Y_Wait[(int)EWay.L]);
                        _SetLog("GDL Y axis move wait.", CData.SPos.dGRD_Y_Wait[(int)EWay.L]);

                        m_iStep++;
                        return false;
                    }

                case 37:
                    {//Table Y축 대기 위치 이동 확인, 종료
                        if (!CMot.It.Get_Mv((int)EAx.LeftGrindZone_Y, CData.SPos.dGRD_Y_Wait[(int)EWay.L]))
                        { return false; }

                        m_iBcrCnt = 0;
                        _SetLog("Finish.");

                        m_iStep = 0;                        
                        return true;
                    }
            }
        }

        public bool Cyl_OnpManOriBcrRight()
        {
            //Timeout check
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADERPICKER_ORI_TIMEOUT);
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
                    {//오프로더 피커 Z축 대기위치 이동
                        CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.SPos.dOFP_Z_Wait);
                        _SetLog("OFP Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        CData.Parts[(int)EPart.GRDR].sBcr = "";

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {//오프로더 피커 Z축 대기위치 이동 확인, 오프로더 피커 90도
                        if (!CMot.It.Get_Mv((int)EAx.OffLoaderPicker_Z, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        CSq_OfP.It.Func_Picker90();
                        _SetLog("OFP Picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 12:    //오프로더 피커 90도 확인, 오프로더 피커 X축 대기위치 이동 //-> 2021.08.24 lhs OffPicker 대기위치에서 100으로 변경
                    {
                        if (!CSq_OfP.It.Func_Picker90())
                        { return false; }

                        // 2021.08.24 lhs Start : 대기위치에서 100으로 변경, OnPicker와 충돌
                        //CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait);
                        //_SetLog("OFP X axis move wait.", CData.SPos.dOFP_X_Wait);

                        CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, 100.0);
                        _SetLog("OFP X axis move wait.", 100.0);
                        // 2021.08.24 lhs End

                        m_iStep++;
                        return false;
                    }

                case 13:    //오프로더 피커 X축 대기위치 이동 확인, 그라인드 오른쪽 Z축 대기위치 이동  //-> 2021.08.24 lhs OffPicker 대기위치에서 100으로 변경
                    {
                        // 2021.08.24 lhs Start : 대기위치에서 100으로 변경, OnPicker와 충돌
                        //if (!CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait))
                        //{ return false; }

                        if (!CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, 100.0))
                        { return false; }
                        // 2021.08.24 lhs End

                        CMot.It.Mv_N((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Wait[(int)EWay.R]);
                        _SetLog("GDR Z axis move wait.", CData.SPos.dGRD_Z_Wait[(int)EWay.R]);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//그라인드 오른쪽 Z축 대기 위치 이동 확인, 그라인드 오른쪽 Y축 Ori 위치 이동
                        if (!CMot.It.Get_Mv((int)EAx.RightGrindZone_Z, CData.SPos.dGRD_Z_Wait[(int)EWay.R]))
                        { return false; }

                        CMot.It.Mv_N((int)EAx.RightGrindZone_Y, CData.Dev.aGrd_Y_Ori[(int)EWay.R]);
                        _SetLog("GDR Y axis move orientation.", CData.Dev.aGrd_Y_Ori[(int)EWay.R]);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//그라인드 오른쪽 Y축 Ori 위치 이동 확인
                        if (!CMot.It.Get_Mv((int)EAx.RightGrindZone_Y, CData.Dev.aGrd_Y_Ori[(int)EWay.R]))
                        { return false; }

                        CBcr.It.Save_RunStatus();
                        m_Delay.Set_Delay(1000);
                        _SetLog("Set delay : 1000ms");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//딜레이 확인
                        if (!m_Delay.Chk_Delay())
                        { return false; }
                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//바코드 프로그램 런 체크
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
                            _SetLog("Error : BCR Not ready.");

                            m_iStep = 0;
                            return true;
                        }

                        CData.Bcr.sBcr = "";
                        _SetLog("BCR Ready.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Picker 0 turn
                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//Picker 0 turn, Z축 Wait 이동
                        if (!Func_Picker0())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//Z축 Wait 이동 확인, X축, Y축 오리엔테이션 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Ori_TbR);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Ori_TbR);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//X축, Y축 오리엔테이션 측정 위치 이동 확인, Z축 오리엔테이션 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Ori_TbR))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Ori_TbR))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Ori_TbR);

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//Z축 오리엔테이션 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Ori_TbR))
                        { return false; }

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {//오리엔테이션 체크
                        //200118 ksg :
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Ori();
                        if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Ori       ();
                        else                                       CBcr.It.Chk_Ori_RTable();
                        m_Delay.Set_Delay(1000);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        if (!m_Delay.Chk_Delay()) return false;
                        //20200314 jhc : 2D Vision SW Alarm 체크
                        if((CDataOption.BcrInterface == eBcrInterface.Udp) && (CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                        {
                            CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                            CBcr.It.b2DVisAlarmNotify = false;
                            CBcr.It.s2DVisAlarmMsg = "";
                            m_iStep = 0;
                            return true;
                        }
                        //
                        CBcr.It.Load();
                        if (CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;


                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//오리엔테이션 확인
                        if (!CData.Bcr.bRetOri)
                        {
                            if (m_iBcrCnt > 3)
                            {
                                CErr.Show(eErr.INRAIL_ORIENTATION_READING_FAIL);
                                _SetLog("Error : Orientation reading fail.");
                                m_iBcrCnt = 0;

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


                                m_iStep = 23;
                                return false;
                            }
                        }

                        m_iBcrCnt = 0;

                        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                        CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                        CData.bOriOneTimeSkip = false; //Orientation One Time Skip 설정 초기화
                        //

                        //m_iStep++;
                        //바코드 읽는 위치 축이동이 안돼서 오리엔테이션만 한다고 합니다.
                        m_iStep = 32;

                        return false;
                    }

                case 26:
                    {//X축, Y축 바코드 측정 위치 이동
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnp_X_Bcr_TbR);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnp_Y_Bcr_TbR);


                        m_iStep++;
                        return false;
                    }

                case 27:
                    {//X축 바코드 측정 위치 이동 확인, Z축 바코드 측정 위치 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnp_X_Bcr_TbR))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnp_Y_Bcr_TbR))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnp_Z_Bcr_TbR);

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {//Z축 바코드 측정 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnp_Z_Bcr_TbR))
                        { return false; }


                        m_iStep++;
                        return false;
                    }

                case 29:
                    {//바코드 읽기
                        //200131 ksg : S/W 변경으로 인하여 두가지로 변경 됨
                        //CBcr.It.Chk_Bcr();
                        if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Bcr       ();
                        else                                       CBcr.It.Chk_Bcr_RTable();
                        m_Delay.Set_Delay(1000);

                        m_iStep++;
                        return false;
                    }

                case 30:
                    {
                        if (!m_Delay.Chk_Delay()) return false;
                        //20200314 jhc : 2D Vision SW Alarm 체크
                        if((CDataOption.BcrInterface == eBcrInterface.Udp) && (CBcr.It.eBcrCommStatus == eBcrUdpStatus._4_Alarm) && CBcr.It.b2DVisAlarmNotify)
                        {
                            CErr.Show(eErr.BCR_2DVISION_ALARM_STATUS);
                            CBcr.It.b2DVisAlarmNotify = false;
                            CBcr.It.s2DVisAlarmMsg = "";
                            m_iStep = 0;
                            return true;
                        }
                        //
                        CBcr.It.Load();
                        if (CData.Bcr.bStatusBcr)
                        {
                            m_Delay.Set_Delay(1000);
                            return false;
                        }

                        m_iBcrCnt++;

                        m_iStep++;
                        return false;
                    }

                case 31:
                    {//바코드 읽은 값 확인, 바코드 값 저장, Y축 대기 위치 이동
                        if (!CData.Bcr.bRet || CData.Bcr.sBcr == "error")
                        {
                            if (m_iBcrCnt > 3)
                            {
                                CErr.Show(eErr.INRAIL_BARCODE_READING_FAIL);
                                m_iBcrCnt = 0;
                                m_iStep = 0;

                                return true;
                            }
                            else
                            {


                                m_iStep = 29;
                                return false;
                            }
                        }
                        CData.Parts[(int)EPart.GRDR].sBcr = CData.Bcr.sBcr;

                        m_iStep++;
                        return false;
                    }

                case 32:
                    {//Z축 대기위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 33:
                    {//Z축 대기위치 이동 확인, Y축 대기 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dONP_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 34:
                    {//Y축 대기 위치 이동 확인, Picker 0 turn
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONP_Y_Wait))
                        { return false; }

                        Func_Picker0();

                        m_iStep++;
                        return false;
                    }

                case 35:
                    {//Picker 0 turn 확인, X축 대기위치 이동
                        if (!Func_Picker0()) return false;

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnP_X_Wait);

                        m_iStep++;
                        return false;
                    }
                case 36:
                    {//X축 대기위치 이동 확인, Table Y축 대기 위치로 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnP_X_Wait))
                        { return false; }

                        CMot.It.Mv_N((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_Wait[(int)EWay.R]);


                        m_iStep++;
                        return false;
                    }
                // 2021.08.24 lhs Start : OffPicker 대기위치로 이동할 목적으로 case37 수정 및 case38 추가
                //case 37:
                //	{//Table Y축 대기 위치 이동 확인, 종료
                //		if (!CMot.It.Get_Mv((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_Wait[(int)EWay.R]))
                //		{ return false; }

                //		m_iStep = 0;
                //		m_iBcrCnt = 0;
                //		return true;
                //	}

                case 37: //Table Y축 대기 위치 이동 확인, 오프로더 피커 90도, X축 대기위치로 이동
                    {
                        if (!CMot.It.Get_Mv((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_Wait[(int)EWay.R]))
                        { return false; }

                        if (!CSq_OfP.It.Func_Picker90())
                        { return false; }

                        CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait);
                        _SetLog("OFP X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 38:    //오프로더 피커 X축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait))
                        { return false; }

                        m_iStep = 0;
                        m_iBcrCnt = 0;
                        return true;
                    }
                // 2021.08.24 lhs End : OffPicker 대기위치로 이동할 목적으로 case37 수정 및 case38 추가

            }
        }
        //211007 syc : 2004U
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Cyl_ONP_IV2()
        {
            int idelay = 800; // 딜레이 설정
            // Timeout check            
            if (m_iPreStep != m_iStep)
            {
                //210824 syc : 2004U 수정 필요 타임 아웃 시간 메뉴얼 동작 돌려고보 다시 정하기
                m_TiOut.Set_Delay(30000);
                //
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    //210824 syc : 2004U 수정 필요 
                    //에러 추가할 것.
                    CErr.Show(eErr.ONLOADERPICKER_UNIT_CHECK_TIMEOUT_ERROR);

                    _SetLog("Error : ONLOADERPICKER_UNIT_CHECK_TIMEOUT_ERROR.");

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
                //변수 초기화
                case 10:
                    {
                        // 축 상태 확인 구문 추가 필요성 검토하여 추가 유무 판단할 것.                       
                        iIV2countCheck = 0; // Conncet 상태 확인 카운트 초기화
                        iONPPara = 1;       // IV2 검사할때 몇번째 파라미터를 사용할건지 초기화 (처음에는 첫번째 파라미터 사용)

                        _SetLog("");

                        m_iStep++;
                        return false;
                    }

                // IV2 사용 유무 검사
                // IV2 카메라 이상시 스킵하고 사용하기 위한 목적 (비상용)
                case 11:
                    {
                        if (CDataOption.UseIV2 == false) // IV2 사용이 Skip 이라면 마지막 구문으로 이동
                        {
                            _SetLog("ONP IV2 Check Skip");

                            m_iStep = 100; // IV2 사용이 Skip 이라면 마지막 구문으로 이동
                            //
                            return false;
                        }
                        else // IV2 사용 상태라면 진행
                        {
                            _SetLog("ONP IV2 Check Use");

                            m_iStep++;
                            return false;
                        }
                    }

                // Z축 대기 위치 이동                 
                case 12:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dONP_Z_Wait) == 0)
                        {
                            _SetLog("Z axis move to wait position.", CData.SPos.dONP_Z_Wait);
                            m_iStep++;
                        }
                        return false;
                    }
                // Z축 대기 위치 이동 확인
                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONP_Z_Wait)) { return false; }
                        else
                        {
                            _SetLog("Z axis wait position movement completed");

                            m_Delay.Set_Delay(idelay); // IV2 사용 가능상태 확인 답변을 위한 딜레이 설정

                            m_iStep = 20;
                            return false;
                        }
                    }

                // 파라미터 추가 가능성 있기때문에 Step 번호 남겨둠
                case 20:
                    {
                        // ----- 파라미터 설정 
                        if (iONPPara == 1) // 첫번째 파라미터일 경우
                        {
                            SIV2Para = CData.Dev.sIV2_ONP1_Para; // 1번째 검사 파라미터
                            dIV2Xpos = CData.Dev.dIV2_ONP1_X;    // X 축 검사 위치
                            dIV2Ypos = CData.Dev.dIV2_ONP1_Y;    // Y 축 검사 위치
                            dIV2Zpos = CData.Dev.dIV2_ONP1_Z;    // Z 축 검사 위치
                        }
                        else if (iONPPara == 2) // 두번째 파라미터일 경우
                        {
                            SIV2Para = CData.Dev.sIV2_ONP2_Para; // 2번째 검사 파라미터
                            dIV2Xpos = CData.Dev.dIV2_ONP2_X;    // X 축 검사 위치
                            dIV2Ypos = CData.Dev.dIV2_ONP2_Y;    // Y 축 검사 위치
                            dIV2Zpos = CData.Dev.dIV2_ONP2_Z;    // Z 축 검사 위치
                        }
                        else // 버그
                        {
                            _SetLog("Error : case 15번 잘못된 접근입니다.");
                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Parameter = " + iONPPara);
                        // ----- 파라미터 설정 끝

                        IV2CheckRun();             // IV2 가동 상태 확인
                        IVCheckSensorState();      //센서 레디상태 확인 명령어 Send
                        m_Delay.Set_Delay(idelay); // IV2 가동상태 확인 답변을 위한 딜레이 설정

                        m_iStep++;
                        return false;
                    }

                // 센서 이용 가능 상태 확인
                case 21:
                    {
                        // ----- 가동 상태 확인
                        if (!m_Delay.Chk_Delay()) { return false; }

                        if (Chk_IV2Run() != 1) // IV2 가동상태 아님 에러 발생
                        {
                            CErr.Show(eErr.ONLOADERPICKER_IV2_NOT_READY);
                            _SetLog("Error : Check IV2 Status");

                            m_iStep = 0;
                            return true;
                        }
                        // ----- 가동 상태 확인 완료
                        // ----- 센서 사용가능 상태 확인
                        if (Chk_IV2Ready() != 1) // 사용 불가능 상태 또는 타임 아웃
                        {
                            _SetLog("Error : ONLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.ONLOADERPICKER_IV2_NOT_READY);

                            m_iStep = 0;
                            return true;
                        }
                        // ----- 센서 사용가능 상태 확인 완료

                        SendPrg(SIV2Para); // 프로그램 변경 //int형으로 바꿀것.
                        _SetLog("IV2 parameter program try change : " + SIV2Para);

                        m_iStep = 25;
                        return false;
                    }

                // 파라미터 추가 가능성 있기때문에 Step 번호 남겨둠
                //반복 시작 25 ~ 36
                // Y축 회피 위치 이동
                case 25:
                    {
                        if (CMot.It.Mv_N(m_iY, dIV2Ypos) == 0)
                        {
                            _SetLog("Y axis move to IV2 Check position.", dIV2Ypos);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }

                // Y축 검사위치(BCR 회피위치)로 이동 확인
                case 26:
                    {
                        if (!CMot.It.Get_Mv(m_iY, dIV2Ypos)) { return false; }
                        else
                        {
                            _SetLog("Y axis IV2 Check position movement completed", dIV2Ypos);
                            m_iStep++;
                            return false;
                        }
                    }

                // X축 검사위치로 이동
                case 27:
                    {
                        if (CMot.It.Mv_N(m_iX, dIV2Xpos) == 0)
                        {
                            _SetLog("X axis move to IV2 Check position.", dIV2Xpos);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }

                // X축 검사위치로 이동 확인
                // 피커 0도
                case 28:
                    {
                        if (!CMot.It.Get_Mv(m_iX, dIV2Xpos)) { return false; }
                        else
                        {
                            Func_Picker0(); //피커 0도
                            _SetLog("X axis IV2 Check position movement completed", dIV2Xpos);
                            m_iStep++;
                            return false;
                        }
                    }

                // Z축 검사위치로 이동
                // 피커 0도 확인
                case 29:
                    {
                        if (!Func_Picker0()) { return false; } //0 도 확인
                        if (CMot.It.Mv_N(m_iZ, dIV2Zpos) == 0)
                        {
                            _SetLog("Z axis move to IV2 Check position.", dIV2Zpos);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }

                // Z축 검사위치로 이동 확인
                case 30:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, dIV2Zpos)) { return false; }
                        else
                        {
                            _SetLog("Z axis IV2 Check position movement completed", dIV2Zpos);

                            IVCheckSensorState();      //센서 레디상태 확인 명령어 Send
                            m_Delay.Set_Delay(idelay); // 딜레이 설정

                            m_iStep++;
                            return false;
                        }
                    }

                // IV2 파라미터 변경
                case 31:
                    {
                        if (!m_Delay.Chk_Delay()) return false;

                        if (Chk_IV2Ready() != 1) // 사용 불가능 상태 또는 타임 아웃
                        {
                            _SetLog("Error : ONLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.ONLOADERPICKER_IV2_NOT_READY);

                            m_iStep = 0;
                            return true;
                        }

                        SendPrg(SIV2Para);         // 프로그램 변경 //int형으로 바꿀것.
                        m_Delay.Set_Delay(idelay); // 딜레이 설정
                        _SetLog("IV2 parameter program try change : " + SIV2Para);

                        m_iStep++;
                        return false;
                    }

                // 파라미터 변경 확인
                case 32:
                    {   // ----- 파라미터 변경 확인
                        if (!m_Delay.Chk_Delay()) return false;

                        if (Chk_IV2PrgSetComplete() != 1)
                        {
                            _SetLog("Error : Parameter Change Fail");
                            CErr.Show(eErr.ONLOADERPICKER_IV2_PARAMETER_CHANGE_FAIL);

                            m_iStep = 0;
                            return true;
                        }
                        // ------ 파라미터 변경 확인 완료

                        IVCheckSensorState();      //센서 레디상태 확인 명령어 Send
                        m_Delay.Set_Delay(idelay); // 딜레이 설정
                        _SetLog("IV2 Parameter Changed : " + SIV2Para);

                        m_iStep++;
                        return false;
                    }

                // 센서 사용가능 상태 확인
                // 반복 검사 시작 점 33 ~ 35
                case 33:
                    {
                        // ----- 센서 사용가능 상태 확인
                        if (!m_Delay.Chk_Delay()) return false;

                        if (Chk_IV2Ready() != 1) // 사용 불가능 상태 또는 타임 아웃
                        {
                            _SetLog("Error : ONLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.ONLOADERPICKER_IV2_NOT_READY);

                            m_iStep = 0;
                            return true;
                        }
                        // ------ 센서 사용가능 상태 확인 완료

                        m_Delay.Set_Delay(idelay); // 딜레이 설정
                        _SetLog("Sensor Check");

                        m_iStep++;
                        return false;
                    }


                case 34:
                    {
                        // 5회 검사 시도
                        if (iIV2countCheck < 5)
                        {
                            // ----- 센서 사용가능 상태 확인
                            if (!m_Delay.Chk_Delay()) return false;

                            if (Chk_IV2Ready() != 1) // 사용 불가능 상태 또는 타임 아웃
                            {
                                _SetLog("Error : ONLOADERPICKER_IV2_NOT_READY");
                                CErr.Show(eErr.ONLOADERPICKER_IV2_NOT_READY);

                                m_iStep = 0;
                                return true;
                            }
                            // ------ 센서 사용가능 상태 확인 완료

                            pCtrlIV2.SendTrigger();    // 트리거 전송
                            m_Delay.Set_Delay(idelay); // 딜레이 설정
                            IVCheckSensorState();      // 센서 레디상태 확인 명령어 Send
                            _SetLog("Send Trigger");

                            m_iStep++;
                            return false;
                        }
                        else  //5회 시도 후 실패시 
                        {
                            _SetLog("Error : Result is NG");
                            CErr.Show(eErr.ONLOADERPICKER_IV2_RESULT_NG);

                            m_iStep = 0;
                            return true;
                        }
                    }

                case 35:
                    {
                        if (!m_Delay.Chk_Delay()) return false;

                        // OK 이거나 일정 시간이후 OK 신호가 들어오지 않았을 경우
                        if (pCtrlIV2.RcvResult != IV2Result.OK)
                        {
                            //반복 시도
                            iIV2countCheck++;
                            _SetLog($"Try Count : {iIV2countCheck}");

                            m_Delay.Set_Delay(idelay); // 딜레이 설정
                            IVCheckSensorState();      //센서 레디상태 확인 명령어 Send

                            m_iStep = 33;
                            return false;
                        }

                        _SetLog("Result is OK");
                        iIV2countCheck = 0;
                        m_iStep++;
                        return false;
                    }
                case 36:
                    {
                        if (CData.Dev.bIV2_ONP2_Use && iONPPara < 2)
                        //if(iONPPara <??) 만약에 2번이상 찍어야하는 상황오면 수정 이부분은 주석 제거하고 수정 후 사용 필요
                        {
                            iONPPara++;
                            _SetLog("Parameter Changed : " + iONPPara);

                            m_iStep = 20; //파라미터 설정 위치로 이동
                            return false;
                        }
                        else
                        {
                            _SetLog("IV2 Check finish");

                            m_iStep = 100;
                            return false;
                        }
                    }
                case 100:
                    {
                        if ((!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) && !CData.Dev.bOriSkip)            // BCR/OCR, Orientation 수행
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcrOri;
                        }
                        else if ((!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip) && CData.Dev.bOriSkip)        // Orientation은 생략
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr;
                        }
                        else if ((CData.Dev.bBcrSkip && CData.Dev.bOcrSkip) && !CData.Dev.bOriSkip)         // BCR/OCR은 Skip, Only Orientation check
                        {
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                        }
                        else
                        {
                            CData.Parts[(int)EPart.ONP].iStat = m_iPreState;
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_WaitPicker;
                        }

                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.INR].iStat);
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.ONP].iStat);


                        //다음 스텝 조건변경
                        m_iStep = 0;
                        return true;
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

            CLog.Save_Log(eLog.ONP, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
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

            CLog.Save_Log(eLog.ONP, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }



#region Function
        /// <summary>
        /// 피커 0도
        /// </summary>
        /// <returns></returns>
        public bool Func_Picker0()
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.ONP_Rotate90, false);
            CIO.It.Set_Y(eY.ONP_Rotate0 , true );
            bRet = (CIO.It.Get_X(eX.ONP_Rotate0) && !CIO.It.Get_X(eX.ONP_Rotate90));

            return bRet;
        }

        /// <summary>
        /// 피커 90도
        /// </summary>
        /// <returns></returns>
        public bool Func_Picker90()
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.ONP_Rotate0 , false);
            CIO.It.Set_Y(eY.ONP_Rotate90, true );
            bRet = (!CIO.It.Get_X(eX.ONP_Rotate0) && CIO.It.Get_X(eX.ONP_Rotate90));

            return bRet;
        }

        /// <summary>
        /// 버큠 온, true = On, false = Off
        /// </summary>
        /// <param name="bOn"></param>
        public void Func_Vacuum(bool bOn)
        {
            CIO.It.Set_Y(eY.ONP_Vacuum1, (CDataOption.IsRevPicker) ? !bOn : bOn);
            CIO.It.Set_Y(eY.ONP_Vacuum2, (CDataOption.IsRevPicker) ? !bOn : bOn);
        }

        /// <summary>
        /// 이젝트 온, true = On, false = Off
        /// </summary>
        /// <param name="bOn"></param>
        public bool Func_Eject(bool bOn)
        {
            return CIO.It.Set_Y(eY.ONP_Ejector, bOn);
        }

        /// <summary>
        /// 드레인 온, true = On, false = Off
        /// </summary>
        /// <param name="bOn"></param>
        public bool Func_Drain(bool bOn)
        {
            return CIO.It.Set_Y(eY.ONP_Drain, bOn);
        }

        //20190611 ghk_onpclean
        /// <summary>
        /// On = Left
        /// </summary>
        /// <returns></returns>
        public bool Func_CleanerLeft()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_OnpCleaner, true);
            bRet = (CIO.It.Get_X(eX.INR_OnpCleaner_L)) && (!CIO.It.Get_X(eX.INR_OnpCleaner_R));

            return bRet;
        }

        /// <summary>
        ///Off = Right
        /// </summary>
        /// <returns></returns>
        public bool Func_CleanerRight()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.INR_OnpCleaner, false);
            bRet = (!CIO.It.Get_X(eX.INR_OnpCleaner_L)) && (CIO.It.Get_X(eX.INR_OnpCleaner_R));

            return bRet;
        }

#endregion

#region Check

        public bool Chk_Axes(bool bHD = true)
        {
            int iRet = 0;

            iRet = CMot.It.Chk_Rdy(m_iX, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.ONLOADERPICKER_X_NOT_READY);
                _SetLog("Error : X axis not ready.");

                return true;
            }

            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                iRet = CMot.It.Chk_Rdy(m_iY, bHD);
                if (iRet != 0)
                {
                    CErr.Show(eErr.ONLOADERPICKER_Y_NOT_READY);
                    _SetLog("Error : Y axis not ready.");

                    return true;
                }
            }

            iRet = CMot.It.Chk_Rdy(m_iZ, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.ONLOADERPICKER_Z_NOT_READY);
                _SetLog("Error : Z axis not ready.");

                return true;
            }

            return false;
        }

        public bool Chk_Strip()
        {
            bool bRet = false;

            bRet = CIO.It.Get_X(eX.ONP_Vacuum);
            return bRet;
        }

        public bool Chk_Vacuum(bool bOn)
        {
            if (GV.IDLE_MODE)
            { return true; }

            return CIO.It.Get_X(eX.ONP_Vacuum) == bOn;
        }

        /// <summary>
        /// 211001 syc : 2004U IV2 센서상태 확인
        /// "-1 = none" : 결과 대기 중,
        /// "0  = false : 사용 불가능 상태",
        /// "1  = true : 사용 가능 상태"
        /// </summary>
        /// <returns></returns>
        public int Chk_IV2Ready()
        {
            int iRet = -1;

            if      (pCtrlIV2.RcvStateCheck == IV2Result.OK) { iRet =  1; }
            else if (pCtrlIV2.RcvStateCheck == IV2Result.NG) { iRet =  0; }
            else                                             { iRet = -1; }

            return iRet;
        }

        /// <summary>
        /// IV2 Run 상태 확인
        /// "-1 = none",
        /// "0  = false",
        /// "1  = true"
        /// </summary>
        /// <returns></returns>
        /// 
        public int Chk_IV2Run()
        {
            int iRet = -1;

            if      (pCtrlIV2.RcvRunCheck == IV2Result.OK) { iRet =  1; }
            else if (pCtrlIV2.RcvRunCheck == IV2Result.OK) { iRet =  0; }
            else                                           { iRet = -1; }

            return iRet;
        }

        /// <summary>
        /// IV2 프로그램 변경 확인
        /// "-1 = none",
        /// "0  = false",
        /// "1  = true"
        /// </summary>
        /// <returns></returns>
        /// 
        public int Chk_IV2PrgSetComplete()
        {
            int iRet = -1;

            if (pCtrlIV2.RcvSetProgramNo      == IV2Result.OK) { iRet =  1; }
            else if (pCtrlIV2.RcvSetProgramNo == IV2Result.NG) { iRet =  0; }
            else                                               { iRet = -1; }

            return iRet;
        }

        /// <summary>
        /// IV2 에러 체크
        /// "-1 = none",
        /// "0  = 에러",
        /// "1  =  정상
        /// </summary>
        /// <returns></returns>
        /// 
        public int Chk_IV2ErrorCheck()
        {
            int iRet = -1;

            if      (pCtrlIV2.RcvErrCheck == IV2Result.OK) { iRet =  1; }
            else if (pCtrlIV2.RcvErrCheck == IV2Result.NG) { iRet =  0; }
            else                                           { iRet = -1; }

            return iRet;
        }

#endregion

        // 2021.05.31. SungTae Start : [추가]
        public TPart LoadDFServerData(string sLotName, string sBcr)
        {
            TPart tSrc = new TPart();

            string sTemp = "";
            string sPath = GV.PATH_EQ_DF;
            string root = @"\\" + CData.Opt.sNetIP;
            //210819 pjh : D/F Server Net drive 사용 시 연결 필요
            bool bCon = false;            
            if (CData.Opt.bDFNetUse)
            {                
                int ret = CNetDrive.Connect(root, CData.Opt.sNetID, CData.Opt.sNetPw);
                if (ret == (int)ENetError.NO_ERROR)
                {
                    sPath = (@"\\");
                    sPath += CData.Opt.sNetIP + CData.Opt.sNetPath + "\\Suhwoo\\SG2000X\\DfServer\\" + sLotName + "\\" + sBcr;
                    bCon = true;
                }
             }
            else
            { sPath += sLotName + "\\" + sBcr; }
            //
            if (!Directory.Exists(sPath) || sBcr == null) { return tSrc; }
            //210601 pjh : 조건 변경
            sPath += "\\" + sBcr + "_Top.txt";    // Bottom Grinding의 경우 Top Grinding 정보 Load.

            if(!File.Exists(sPath)) { return tSrc; }
            CIni mIni = new CIni(sPath);

            string sSec = "Lot Info";
            tSrc.sLotName = mIni.Read(sSec, "Lot ID");
            tSrc.sBcr = mIni.Read(sSec, "Strip ID");

            sTemp = ESide.Top.ToString();
            sSec = sTemp + " Side Info";
            tSrc.sDeviceName = mIni.Read(sSec, "Recipe Name");
            tSrc.sDualMode = mIni.Read(sSec, "Dual Mode");
            tSrc.sStage = mIni.Read(sSec, "Stage");
            tSrc.sGrdMode = mIni.Read(sSec, "Grind Mode");
            tSrc.sSaveDate = mIni.Read(sSec, "Save Time");

            tSrc.dPcbMin = mIni.ReadD(sSec, "PCB Min");
            tSrc.dPcbMax = mIni.ReadD(sSec, "PCB Max");
            tSrc.dPcbMean = mIni.ReadD(sSec, "PCB Avg");

            tSrc.dBfMin = mIni.ReadD(sSec, "BF Min");
            tSrc.dBfMax = mIni.ReadD(sSec, "BF Max");
            tSrc.dBfAvg = mIni.ReadD(sSec, "BF Avg");

            tSrc.dAfMin = mIni.ReadD(sSec, "AF Min");
            tSrc.dAfMax = mIni.ReadD(sSec, "AF Max");
            tSrc.dAfAvg = mIni.ReadD(sSec, "AF Avg");

            //210819 pjh : 넷드라이브 연결 해제
            if(CDataOption.UseDFNet && bCon)
            {
                CNetDrive.Disconnect();
                bCon = false;
            }
            //
            return tSrc;
        }
        // 2021.05.31. SungTae End


        // 2022.02.15 lhs : SecsGem으로 받은 PCB or Total thickness의 Range Check, Error 표시
        private bool IsOverRangeSecsGemDownTh()
        {
            bool   bCheck       = false;
            double dDownTh      = 0.0;
            double dStandTh     = 0.0;
            double dRange       = 0.0;
            double dMinRange    = 0.0;
            double dMaxRange    = 0.0;

            eErr   eError       = eErr.NONE;
            string sErrMsg      = "";

            if (CDataOption.UseNewSckGrindProc)
            {
                //------------------
                // case 2 : UseNewSckGrindProc 일 경우
                //------------------
                // strREF_THK_AVG     -> 사용 안함
                // strREF_PCB_THK_AVG -> = fMeasure_New_Avr_Data[0] = fMeasure_Avr_Data[0]
                // 항상 PCB 다운로드
                // 여기서는 Pcb만 체크, TopMold, BtmMold는 CalcTopMoldBtmMold(iBfAf, dMax, dAvg)에서 계산.  

                bCheck      = (CData.Dynamic.dPcbMeanRange > 0);
                dDownTh     = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0];  // Pcb Thick
                dStandTh    = CData.Dev.aData[0].dPcbTh;
                dRange      = CData.Dynamic.dPcbMeanRange;
                dMaxRange   = dStandTh + dRange;
                dMinRange   = dStandTh - dRange;

                eError      = eErr.SECSGEM_DOWNLOAD_PCBTHICKNESS_RNAGE_OVER_ERROR;
                sErrMsg     = string.Format("CSQ3_OnP Step - " + m_iStep.ToString() + " Host Download : SECSGEM_DOWNLOAD_PCBTHICKNESS_RNAGE_OVER_ERROR  New Avr = {0}, [Device] PCB thickness = {1}, PCB Mean Range = {2} : {3} ~ {4}",
                                        dDownTh, dStandTh, dRange, dMaxRange, dMinRange);
            }
            else
            {
                //------------------
                // case 1 : 기존 로직이면
                //------------------
                // strREF_THK_AVG -> = fMeasure_New_Avr_Data[0] = fMeasure_Avr_Data[0]
                // TopS 일 경우 : PCB 다운로드
                // BtmD 일 경우 : (TopMold + PCB) 다운로드

                if (CData.Dev.eMoldSide == ESide.Top)       // TopS : Pcb thickness
                {
                    bCheck      = (CData.Dynamic.dPcbMeanRange > 0);
                    dDownTh     = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0];  // Pcb Thick
                    dStandTh    = CData.Dev.aData[0].dPcbTh;
                    dRange      = CData.Dynamic.dPcbMeanRange;
                    dMaxRange   = dStandTh + dRange;
                    dMinRange   = dStandTh - dRange;

                    eError      = eErr.SECSGEM_DOWNLOAD_PCBTHICKNESS_RNAGE_OVER_ERROR;
                    sErrMsg     = string.Format("CSQ3_OnP Step - " + m_iStep.ToString() + " Host Download : SECSGEM_DOWNLOAD_PCBTHICKNESS_RNAGE_OVER_ERROR  New Avr = {0}, [Device] PCB thickness = {1}, PCB Mean Range = {2} : {3} ~ {4}",
                                            dDownTh, dStandTh, dRange, dMaxRange, dMinRange);
                }
                else if (CData.Dev.eMoldSide == ESide.Btm)  // BtmD : Total thickness (TopMold + Pcb)
                {
                    bCheck      = (CData.Dynamic.dPcbMeanRange > 0 || CData.Dev.aData[(int)EWay.L].dBfLimit > 0);
                    dDownTh     = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0];  // Total Thick
                    dStandTh    = CData.Dev.aData[0].dPcbTh;
                    dRange      = CData.Dynamic.dPcbMeanRange + CData.Dev.aData[(int)EWay.L].dBfLimit;
                    dMaxRange   = dStandTh + dRange;
                    dMinRange   = dStandTh - dRange;

                    eError      = eErr.SECSGEM_DOWNLOAD_TOTAL_THICKNESS_RNAGE_OVER_ERROR;
                    sErrMsg     = string.Format("CSQ3_OnP Step - " + m_iStep.ToString() + " Host Download : SECSGEM_DOWNLOAD_TOTAL_THICKNESS_RNAGE_OVER_ERROR  New Avr = {0}, [Device] PCB thickness(Total)  = {1}, PCB Mean Range + BfLimit = {2} : {3} ~ {4}",
                                                dDownTh, dStandTh, dRange, dMaxRange, dMinRange);
                }
            }

            // 범위를 벗어나면 에러 표시
            if (bCheck && (dDownTh > dMaxRange || dDownTh < dMinRange))
            {
                CLog.mLogGnd(sErrMsg);
                CErr.Show(eError);
                _SetLog(sErrMsg);
                
                return true;
            }
            return false;
        }



    }

    




}