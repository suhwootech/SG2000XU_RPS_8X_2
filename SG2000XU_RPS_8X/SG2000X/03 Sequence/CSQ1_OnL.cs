using System;
using System.Diagnostics;
using System.Windows.Forms;     // 2022.04.26 SungTae : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련

namespace SG2000X
{
    public class CSq_OnL : CStn<CSq_OnL>
    {
        private readonly int TIMEOUT = 30000;
        private readonly int ONL = (int)EPart.ONL;

        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep      = 0;  
        private int m_iPreStep   = 0;  
        private int m_iReReadCnt = 0; //200120 ksg :

        private int m_iX    = (int)EAx.OnLoader_X;
        private int m_iY    = (int)EAx.OnLoader_Y;
        private int m_iZ    = (int)EAx.OnLoader_Z;
        private int m_iInrX = (int)EAx.Inrail_X  ;
        private int m_iInrY = (int)EAx.Inrail_Y  ;
        /// <summary>
        /// 매거진 배출 슬롯의 Z축 높이 값 [mm]
        /// </summary>
        private double m_dWorkPosZ = 0.0;
        //20190514 ghk_probe 프로브 업 다운 확인시 저장 할 값
        private double m_dInr_Prb = 0.0;

        private bool m_bPreChkLight = false;
        private bool m_bReqStop     = false;

        private CTim m_Delay      = new CTim(); 
        private CTim m_TiOut      = new CTim();
        private CTim m_LightTiOut = new CTim();
        private CTim m_ProbeDelay = new CTim();

        private DateTime m_tStTime;
        private TimeSpan m_tTackTime ;

        public ESeq iSeq;
        public ESeq SEQ = ESeq.Idle;

        // 2021-02-05, jhLee : AreaSensor 감지여부, 메인화면 상단에 표시를 위한 변수, 기존 MainSeq에서 이동되어 옴
        public bool m_bDspLeftArea = false;

        // 2021.05.04 SungTae : 2003U 개조건 관련 Unit Count 확인용 변수 추가
        private int m_iUnitCnt = 0;

        // 2021-05-11, jhLee : Multi-LOT 처리
        public TLotInfo m_rInputLotInfo = null;         // 투입 처리중인 LOT  정보

        private CSq_OnL()
        {
            m_bHD = false;
        }

        public void Init_Cyl()
        {
            m_iStep = 10;
            m_iPreStep = 0;
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
            return true;
        }

        public bool Stop()
        {
            m_iStep = 0;
            return true;
        }


        public bool AutoRun()
        {
            iSeq = CData.Parts[ONL].iStat;

            #region Light-Curatian Check 
            // Area Sensor 감지여부 처리
            m_bDspLeftArea = CheckLightCurtainPause();         // Area Sensor 감지 여부로 일시멈춤 기능 호출

            // 2021-02-05, jhLee : 일시 멈춤 기능을 이용하는 Skyworks에서도 10초 이후 Alarm 기능은 이용하고 있다.
            // 200618 jym : 스카이웍스 제외한 조건에서 일시정지 안쓰는 조건으로 변경

            if (m_bDspLeftArea)
            {
                if (!m_bPreChkLight)                // 이전에는 감지가 되고있지 않았다.
                {
                    m_bPreChkLight = true;
                    m_LightTiOut.Set_Delay(10000);   // 감지상태 유지 시간을 재기위해 시간 설정, 10초
                }
                else
                {
                    if (m_LightTiOut.Chk_Delay())   // 지정 시간을 초과하였다.
                    {
                        // Alarm 발생 처리
                        _SetLog("Error : Detected light curtain.");
                        CErr.Show(eErr.ONLOADER_DETECT_LIGHTCURTAIN);

                        m_bPreChkLight = false;
                        m_iStep = 0;
                        return true;
                    }
                }
            }
            else
            {
                m_bPreChkLight = false;                 // 이전 감지여부도 Off
            }

            // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
            // Multi-LOT, Idle 상태에서 작업자에의해 Light Curtain이 감지되었다면 
            if (CData.IsMultiLOT())
            {
                // Idle 상태에서 작업자의 움직임을 포착하여 Mgz을 투입하는 행동을 예측하여 Pickup 동작을 할 수 있도록 한다.
                if (((iSeq == ESeq.ONL_WorkEnd) || (iSeq == ESeq.Idle)) && (CData.LotMgr.IsLightCurtailDetect))
                {
                    CData.Parts[ONL].iStat = ESeq.ONL_Wait;             // Mgz Pickup 동작 지시

                    CData.LotMgr.IsLightCurtailDetect = false;          // 감지여부 초기화
                    Debug.WriteLine("Multi-LOT, [ONL] IsLightCurtailDetect execute in Idle state, Idle -> ONL_Pick");
                }

                // IN RAIL이  WorkEnd면 깨워준다.
                if ((CData.Parts[ONL].iStat == ESeq.ONL_Push) && (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd))
                {
                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Ready;
                    // CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Wait;

                    // 2021-06-09, jhLee, Multi-LOT, WorkEnd에 빠진 나머지 Unit들을 깨워준다.
                    if (CData.Parts[(int)EPart.ONP].iStat   == ESeq.ONP_WorkEnd)    CData.Parts[(int)EPart.ONP].iStat   = ESeq.ONP_Wait;
                    if (CData.Parts[(int)EPart.GRDL].iStat  == ESeq.GRL_WorkEnd)    CData.Parts[(int)EPart.GRDL].iStat  = ESeq.GRL_Wait;
                    if (CData.Parts[(int)EPart.GRDR].iStat  == ESeq.GRR_WorkEnd)    CData.Parts[(int)EPart.GRDR].iStat  = ESeq.GRR_Wait;
                    if (CData.Parts[(int)EPart.OFP].iStat   == ESeq.OFP_WorkEnd)    CData.Parts[(int)EPart.OFP].iStat   = ESeq.OFP_Wait;
                    if (CData.Parts[(int)EPart.DRY].iStat   == ESeq.DRY_WorkEnd)    CData.Parts[(int)EPart.DRY].iStat   = ESeq.DRY_Wait;
                    if (CData.Parts[(int)EPart.OFL].iStat   == ESeq.OFL_WorkEnd)    CData.Parts[(int)EPart.OFL].iStat   = ESeq.OFL_Wait;
                }
            }
            #endregion // of #region Light-Curatian Check


            //ksg
            //if (CSQ_Main.It.m_iStat == eStatus.Error || CSQ_Main.It.m_iStat != eStatus.Auto_Running)
            //{ return false; }

            if (SEQ == (int)ESeq.Idle)
            {
                if (m_bReqStop)
                { return false; }

                if (CSQ_Main.It.m_iStat == EStatus.Stop)
                { return false; }

                //syc : ase kr loading stop - 
                // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
                if (CSQ_Main.It.m_bPause && CData.Opt.iLoadingStopType == (int)eTypeLDStop.Inrail/*0*/) //'Loading Stop on 활성화' 이고 'Stop Type 이 0=Inrail' 이면 멈춤
                {
                    return false;
                }

                bool bWait    = false;    // Move Wait Position 함수 실행 조건
                bool bPick    = false;    // Pick Magazine 함수 실행 조건
                bool bPush    = false;    // Push Strip 함수 실행 조건
                bool bPlace   = false;    // Place Magazine 함수 실행 조건
                bool bWorkEnd = false;    // LOT 동작 종료 함수 실행
                // bool bWaitLot = false;    // 2021-05-10, jhLee : LOT 정보가 준비되어 새로운 Magazine을 투입할 수 있는 상황인가 ?


                //// Multi-LOT, Idle 상태에서 작업자에의해 Light Curtain이 감지되었다면 
                //if (CData.IsMultiLOT())
                //{
                //    // Idle 상태에서 작업자의 움직임을 포착하여 Mgz을 투입하는 행동을 예측하여 Pickup 동작을 할 수 있도록 한다.
                //    if (((iSeq == ESeq.ONL_WorkEnd) || (iSeq == ESeq.Idle)) && (CData.LotMgr.IsLightCurtailDetect))
                //    {
                //        CData.Parts[ONL].iStat = ESeq.ONL_Pick;             // Mgz Pickup 동작 지시

                //        CData.LotMgr.IsLightCurtailDetect = false;          // 감지여부 초기화
                //        Debug.WriteLine("Multi-LOT, [ONL] IsLightCurtailDetect execute in Idle state, Idle -> ONL_Pick");
                //    }
                //}


                if (iSeq == ESeq.Idle || iSeq == ESeq.ONL_Wait)
                {   // Move Wait Position
                    bWait = true;
                }
                else if (iSeq == ESeq.ONL_Pick) // Pick Magazine
                {
                    #region SECS/GEM : 미사용 (Off-Line Mode)
                    if (!CData.Opt.bSecsUse/* || CData.CurCompany == ECompany.SkyWorks*/) // 20201028 LCY , skyworks 요구사항 , 정해진 MGZ 만큼만 투입 하도록 요청
					{
                        #region Multi-LOT : 사용
                        // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                        // 2021-05-10, jhLee : Multi-LOT 처리, 투입하고자하는 LOT 정보가가 준비되어있는 경우에 Pickup 동작 수행
                        if (CData.IsMultiLOT())
						{
							// 만약 사용자의 응답을 대기하는 중이라면 응답을 받을 때 까지 새로운 Magazine을 Pickup 하지 않도록 한다.
							// 사용자 응답 대기 상태가 아니다
							if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eNone)
							{
								// 현재 투입중이거나 투입 대기중인 LOT 정보가 존재하여 Pickup이 가능한 상태인가 ?
								if (CData.LotMgr.IsMgzPickupable())
								{
									// 다음 매거진 픽하는 동작 실행
									bPick = true;
									bWorkEnd = false;
								}
								else
								{
									bPick = false;                  // Pickup 동작을 하지 않는다.
									bWorkEnd = true;
								}
							}
							else
							{
								// Yes 응답을 받았다. LOT을 종료시키라는 응답
								if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyYes)
								{
									string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

									// LOT을 종료시킨다.
									// 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
									_SetLog($"Multi-LOT, User confirm response Yes, Close Loading LOT : {CData.LotMgr.LoadingLotName}");

									CData.LotMgr.SetCloseLoadLOT();
									CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.
									CData.LotMgr.CheckAtLotClose(sLotName);                         // LOT-End 처리할 것인지 Check
								}
								// No 응답을 받았다.  LOT을 유지하고 투입을 계속하라는 응답
								else if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyNo)
								{
									// MGZ Pickup 실패에 의해 Lot close 가능한 상태였다면
									if (CData.LotMgr.IsLotCloseableMgz)
									{
										CData.LotMgr.LoadMgzTryCnt = 0;         // Count Clear
										_SetLog($"Multi-LOT, Loading MGZ pickup try count to 0");
									}

									_SetLog($"Multi-LOT, User confirm response No, Continue Loading LOT : {CData.LotMgr.LoadingLotName}");

									// 작업자가 계속 투입을 선택하였을때 변경시킬 LOT 종료 모드를 지정한다.
									CData.LotMgr.SetLotEndMode(CData.LotMgr.UserConfirmNextMode);

									CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.
								}

								bPick = false;                  // Pickup 동작을 하지 않는다.
																// bWorkEnd = true;
							}
						}
                        #endregion
                        #region Multi-LOT : 미사용
                        else   // Multi-LOT을 사용하지 않는 경우
                        {
                            if (CData.Parts[ONL].iMGZ_No >= CData.LotInfo.iTotalMgz)
							{    // 랏의 매거진 갯수 보다 현재 매거진 인덱스 높그나 같을 때 -> 랏의 매거진 갯수만큼 실행이 완료 된 상태
								bPick       = false;
								bWorkEnd    = true; //2021.10.06 lhs 주석 제거하여 복원
							}
							else
                            {    // 다음 매거진 픽하는 동작 실행
                                bPick       = true;
                                bWorkEnd    = false;
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region SECS/GEM : 사용 (On-Line Mode)
                    else // else :  if (!CData.Opt.bSecsUse || CData.CurCompany == ECompany.SkyWorks)
                    {
                        #region Multi-LOT : 사용 시작
                        // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                        // 2021-05-10, jhLee : Multi-LOT 처리, 투입하고자하는 LOT 정보가가 준비되어있는 경우에 Pickup 동작 수행
                        if (CData.IsMultiLOT())
                        {
                            if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eNone)
                            {
                                // 현재 투입중이거나 투입 대기중인 LOT 정보가 존재하는가 ?
                                if (CData.LotMgr.IsMgzPickupable())
                                {
                                    // 다음 매거진 픽하는 동작 실행
                                    bPick = true;
                                    bWorkEnd = false;
                                }
                                else
                                {
                                    // 2021.08.24 SungTae Start : [수정] Remote일 때의 조건 추가
                                    if (CData.Opt.bSecsUse && CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
                                    {
                                        // Remote의 경우 아직 Host로부터 Data를 못받은 상태이니 다음 매거진 픽하는 동작 실행
                                        bPick = true;
                                        bWorkEnd = false;
                                    }
                                    else
                                    {
                                        // Multi-Lot 사용 시에는 모든 Magazine을 Pickup  하였더라도 WorkEnd는 아니다.
                                        bPick = false;                  // Pickup 동작을 하지 않는다.
                                        // bWorkEnd = true;
                                    }
                                    // 2021.08.24 SungTae End
                                }
                            }
                            else
                            {
                                #region UserConfirmReply : YES 면
                                // 2021.12.24 SungTae Start : [수정] Remote 상태에서의 Multi-LOT 관련 조건 추가
                                // Yes 응답을 받았다. LOT을 종료시키라는 응답
                                if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyYes)
                                {
                                    #region SECS/GEM Remote Flag : On-line Remote 면
                                    if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
                                    {
                                        CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.    

                                        // LOT을 종료시킨다.
                                        // 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
                                        _SetLog($"Multi-LOT, User confirm response Yes, MGZ is no longer put in.");

                                        bWorkEnd = true;
                                    }
                                    #endregion
                                    #region SECS/GEM Remote Flag : On-line Local 이면
                                    else
                                    {
                                        string sLotName = CData.LotMgr.LoadingLotName;                  // Loading 중인 LOT의 이름

                                        // LOT을 종료시킨다.
                                        // 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
                                        _SetLog($"Multi-LOT, User confirm response Yes, Close Loading LOT : {CData.LotMgr.LoadingLotName}");

                                        CData.LotMgr.SetCloseLoadLOT();
                                        CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.
                                        CData.LotMgr.CheckAtLotClose(sLotName);                         // LOT-End 처리할 것인지 Check
                                    }
                                    #endregion
                                }
                                #endregion
                                #region UserConfirmReply : NO 면
                                // No 응답을 받았다.  LOT을 유지하고 투입을 계속하라는 응답
                                else if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyNo)
                                {
                                    if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
                                    {
                                        _SetLog($"Multi-LOT, User confirm response No, waiting for a new MGZ to be put in.");

                                        // 2022.01.07 SungTae Start : [추가] Remote 상태에서의 Multi-LOT 관련 조건 추가
                                        // MGZ Pickup 실패에 의해 Lot close 가능한 상태였다면
                                        if (CData.LotMgr.IsLotCloseableMgz)
                                        {
                                            CData.LotMgr.LoadMgzTryCnt = 0;         // Count Clear
                                            _SetLog($"Multi-LOT, Loading MGZ pickup try count to 0");
                                        }
                                        // 2022.01.07 SungTae End
                                    }
                                    else
                                    {
                                        _SetLog($"Multi-LOT, User confirm response No, Continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                    }

                                    // 작업자가 계속 투입을 선택하였을때 변경시킬 LOT 종료 모드를 지정한다.
                                    CData.LotMgr.SetLotEndMode(CData.LotMgr.UserConfirmNextMode);

                                    CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.
                                }
                                // 2021.12.24 SungTae End
                                #endregion

                                bPick = false;                  // Pickup 동작을 하지 않는다.
                                //bWorkEnd = true;
                            }
                        }
                        #endregion
                        #region Multi-LOT : 미사용 시작
                        else // of if (CData.IsMultiLOT())
                        {
                            if (CData.LotInfo.iTInCnt >= CData.LotInfo.iTotalStrip)
                            {    // 랏의 매거진 갯수 보다 현재 매거진 인덱스 높그나 같을 때 -> 랏의 매거진 갯수만큼 실행이 완료 된 상태
                                bPick    = false;
                                bWorkEnd = true;

                                // 2021.09.09 lhs Start : 로그 추가
                                _SetLog(string.Format("CData.LotInfo.iTInCnt = {0} >= CData.LotInfo.iTotalStrip = {1} => bPick = false, bWorkEnd = true", CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip));
                            }
                            else // 다음 매거진 픽하는 동작 실행
                            {
                                // 2021.10.04 lhs Start : OnLoader에서 사용자에게 Mgz Pick을 할 것인지 WorkEnd를 할 것인지 묻기 (SCK+ VOC)
                                // 조건1 : Strip이 적게 투입된 경우 계속 Mgz Pick을 함 (CData.LotInfo.iTInCnt < CData.LotInfo.iTotalStrip)
                                // 조건2 : 투입된 Mgz이 사용자가 설정한 Mgz 이상일 경우 묻기 (CData.Parts[ONL].iMGZ_No >= CData.LotInfo.iTotalMgz)
                                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                                {
                                    if (CData.Parts[ONL].iMGZ_No >= CData.LotInfo.iTotalMgz) // 메시지창으로 묻기
                                    {
                                        if (CData.eOnLUserConfirm == EOnLUserConfirm.eNone)
                                        {
                                            CData.eOnLUserConfirm = EOnLUserConfirm.eQuestion;
                                            _SetLog(string.Format("Show Question Window : iTInCnt = {0}, iTotalStrip = {1}, iMGZ_No = {2}, iTotalMgz = {3}",
                                                                            CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip, CData.Parts[ONL].iMGZ_No, CData.LotInfo.iTotalMgz));
                                        }
                                        else if (CData.eOnLUserConfirm == EOnLUserConfirm.eReplyYes) // WorkEnd 선택
                                        {
                                            CData.eOnLUserConfirm = EOnLUserConfirm.eNone;
                                            
                                            bPick = false;
                                            bWorkEnd = true;
                                            _SetLog(string.Format("User Yes Selected -> WorkEnd : iTInCnt = {0}, iTotalStrip = {1}, iMGZ_No = {2}, iTotalMgz = {3}", 
                                                                               CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip, CData.Parts[ONL].iMGZ_No,CData.LotInfo.iTotalMgz));
                                        }
                                        else if (CData.eOnLUserConfirm == EOnLUserConfirm.eReplyNo) // Pick 계속
                                        {
                                            CData.eOnLUserConfirm = EOnLUserConfirm.eNone;

                                            bPick = true;
                                            bWorkEnd = false;
                                            CData.LotInfo.iTotalMgz++;  // 임의로 증가

                                            _SetLog(string.Format("User No Selected -> Pick ( iTotalMgz++ )  : iTInCnt = {0}, iTotalStrip = {1}, iMGZ_No = {2}, iTotalMgz = {3}",
                                                                                            CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip, CData.Parts[ONL].iMGZ_No, CData.LotInfo.iTotalMgz));
                                        }
                                    }
                                    else
                                    {
                                        bPick = true;
                                        bWorkEnd = false;
                                        _SetLog(string.Format("CData.LotInfo.iTInCnt = {0} < CData.LotInfo.iTotalStrip  = {1} => bPick = true, bWorkEnd = false", CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip));
                                    }
								}
                                // 2021.10.04 lhs End
                                else // 기존 로직
                                {
                                    bPick = true;
                                    bWorkEnd = false;

                                    // 2021.09.09 lhs Start : 로그 추가
                                    _SetLog(string.Format("CData.LotInfo.iTInCnt = {0} < CData.LotInfo.iTotalStrip  = {1} => bPick = true, bWorkEnd = false", CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip));
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                }

                // 201104 jym : 비교 조건 변경
                //if (iSeq == ESeq.ONL_Push && ((CSq_Inr.It.iSeq == ESeq.Idle) || (CSq_Inr.It.iSeq == ESeq.INR_Ready)))
                if (iSeq == ESeq.ONL_Push && 
                    ((CData.Parts[(int)EPart.INR].iStat == ESeq.Idle) || (CData.Parts[(int)EPart.INR].iStat == ESeq.INR_Ready)))
                {
                    // 만약 사용자의 응답을 대기하는 중이라면 응답을 받을 때 까지 새로운 strip을 투입하지 않는다.
                    if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eNone) // 사용자 응답 대기 상태가 아니다
                    {
                        // 정상적으로 투입 작업을 수행한다.
                        bPush = true;
                    }
                    else
                    {
                        // Yes 응답을 받았다. LOT을 종료시키라는 응답
                        if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyYes)
                        {
                            string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                            Func_PusherForward(false);
                            CData.Parts[ONL].iStat = ESeq.ONL_Place;
                            _SetLog("LOT Close, Magazine place  sequence");

                            // LOT을 종료시킨다.
                            // 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
                            _SetLog($"Multi-LOT, User confirm response Yes, Close Loading LOT : {CData.LotMgr.LoadingLotName}");

                            CData.LotMgr.SetCloseLoadLOT();
                            CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.
                            CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                        }
                        // No 응답을 받았다.  LOT을 유지하고 투입을 계속하라는 응답
                        else if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyNo)
                        {
                            _SetLog($"Multi-LOT, User confirm response No, Continue Loading LOT : {CData.LotMgr.LoadingLotName}");

                            // 작업자가 계속 투입을 선택하였을때 변경시킬 LOT 종료 모드를 지정한다.
                            CData.LotMgr.SetLotEndMode(CData.LotMgr.UserConfirmNextMode);
                            CData.LotMgr.UserConfirmReply = ELotUserConfirm.eNone;          // 이제 응답 대기중이 아니다.
                        }

                        bPush = false;                  // push 동작을 하지 않는다.
                    }
                }

                //220624 pjh : LotOpen시 입력한 Strip Count 수량만큼 진행여부 확인  
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardB)
                {
                    if (iSeq == ESeq.ONL_Push)
                    {
                        if (CData.LotInfo.iTInCnt >= CData.LotInfo.iTotalStrip)
                        {    // 랏의 매거진 갯수 보다 현재 매거진 인덱스 높그나 같을 때 -> 랏의 매거진 갯수만큼 실행이 완료 된 상태
                            bPlace = true;

                            // 2021.09.09 lhs Start : 로그 추가
                            _SetLog(string.Format("CData.LotInfo.iTInCnt = {0} >= CData.LotInfo.iTotalStrip = {1} => bPick = false, bWorkEnd = true", CData.LotInfo.iTInCnt, CData.LotInfo.iTotalStrip));
                        }
                    }
                }
                //

                // Magazine Place 조건 체크
                if (CDataOption.CurEqu == eEquType.Nomal)
                {
                    if (iSeq == ESeq.ONL_Place && CIO.It.Get_X(eX.INR_StripInDetect) && Func_PusherForward(false)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_CheckBcr)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_CheckOri)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_CheckDynamic)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_DynamicFail))
                    {    // 매거진 내려 놓는 함수 실행 조건
                        bPlace = true;
                    }
                }
                else
                {
                    if (iSeq == ESeq.ONL_Place && CIO.It.Get_X(eX.INR_StripInDetect) && Func_PusherForward(false)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_CheckBcr)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_CheckOri)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_CheckDynamic)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_DynamicFail)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_BcrReady)
                        && (CData.Parts[(int)EPart.INR].iStat != ESeq.INR_OriReady))
                    {    // 매거진 내려 놓는 함수 실행 조건
                        bPlace = true;
                    }
                }

                if (iSeq == ESeq.ONL_WorkEnd)
                {
                    return true;
                }

                if (bWait)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.ONL_Wait;
                    _SetLog(">>>>> Wait start.");
                }
                else if (bPick)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.ONL_Pick;

                    // 2022.04.28 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련
                    //_SetLog(">>>>> Pick start.");
                    if (CData.Opt.bOnlRfidSkip)
                    {
                        _SetLog(">>>>> Pick Start.");
                    }
                    else
                    {
                        if (CDataOption.RFID == ERfidType.Keyence)  _SetLog(">>>>> Pick BCR Start.");
                        else                                        _SetLog(">>>>> Pick RFID Start.");
                    }
                    // 2022.04.28 SungTae End
                }
                else if (bPush)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.ONL_Push;
                    _SetLog(">>>>> Push start.");
                }
                else if (bPlace)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.ONL_Place;
                    CData.Parts[ONL].iStat = ESeq.ONL_Place;
                    _SetLog(">>>>> Place start.");
                }
                else if (bWorkEnd)
                {
                    CData.Parts[ONL].iStat = ESeq.ONL_WorkEnd;
                    _SetLog(">>>>> Work End.");
                    return true;
                }
            }

            switch (SEQ)
            {
                default:
                    return false;

                case ESeq.ONL_Wait:
                    {
                        if (Cyl_Wait())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Wait finish.");
                        }

                        return false;
                    }

                case ESeq.ONL_Pick:
                    {
                        if (CDataOption.OnlRfid == eOnlRFID.Use)
                        {
                            if (CData.Opt.bOnlRfidSkip)
                            {
                                if (Cyl_Pick())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Pick finish.");
                                }
                            }
                            else
                            {
#if true //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드, SPIL)
                                bool bResult = false;

                                if (CDataOption.RFID == ERfidType.Keyence) //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용 //if (CData.CurCompany == ECompany.SPIL)
                                { bResult = Cyl_PickBcr(); }
                                else
                                { bResult = Cyl_PickRfid(); }

                                if (bResult)
                                {
                                    SEQ = ESeq.Idle;

                                    // 2022.04.28 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련
                                    //_SetLog("<<<<< Pick RFID Finish.");
                                    if (CDataOption.RFID == ERfidType.Keyence)  _SetLog("<<<<< Pick BCR Finish.");
                                    else                                        _SetLog("<<<<< Pick RFID Finish.");
                                    // 2022.04.28 SungTae End
                                }
#else
                                if (Cyl_PickRfid())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("Pick RFID cycle end");
                                }
#endif
                            }
                        }
                        else
                        {
                            if (Cyl_Pick())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Pick finish.");
                            }
                        }

                        return false;
                    }

                case ESeq.ONL_Push:
                    {
                        // 2021.05.03 SungTae Start : 2003U 개조건 관련 조건 추가
                        if (CDataOption.Package == ePkg.Strip)
                        {
                            if (Cyl_Push())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Push finish.");

                                // syc : ase kr loading stop 
                                //// 2020-11-17, jhLee : 지정 수량 투입 후 자동 투입중지 기능
                                //if (CData.CurCompany == ECompany.SkyWorks)      // Skyworks 전용
                                //{
                                //    if (CSQ_Main.It.m_bAutoLoadingStop)
                                //    {
                                //        CSQ_Main.It.m_iLoadingCount++;             // 자동 투입중지 모드를 설정 한 뒤 투입된 제품 수량 증가

                                //        if (CSQ_Main.It.m_iLoadingCount >= CData.Opt.iAutoLoadingStopCount)
                                //        {
                                //            CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
                                //            CSQ_Main.It.m_bPause = true;                // 투입 중지가 활성화 된다.       

                                //            _SetLog("[Auto Loading Stop] finish " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                                //        }
                                //        else
                                //        {
                                //            _SetLog("[Auto Loading Stop] remain " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                                //        }
                                //    }

                                // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
                                if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Inrail/*0*/)
                                {
                                    CData.Chk_AutoLoading_Count();
                                }
                                //syc end
                            }
                        }
                        else
                        {
                            //
                            // < !--U - Type 설비 구분 사용  0 : 2001U, 1 : 2003U 개조-- >
                            //  
                            // < !--0 = 2001U, 1 = 2003U-- >
                            // < EQUIP_CLASSIFICATION > 1 </ EQUIP_CLASSIFICATION >
                            // 
                            // 2001U : 개조전 설비
                            // 2003U : Unit부 개조 설비
                            //
                            if (CDataOption.iEquipClass == (int)eEqSeries.U_2003)      // 2021.05.18 SungTae : 설비 구분 조건 추가로 변경
                            {
                                if (Cyl_PushU())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Push_U finish.");

                                    // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
                                    if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Inrail/*0*/)
                                    {
                                        CData.Chk_AutoLoading_Count();
                                    }
                                }
                            }
                            else
                            {
                                if (Cyl_Push())
                                {
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< Push finish.");

                                    // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
                                    if (CData.Opt.iLoadingStopType == (int)eTypeLDStop.Inrail/*0*/)
                                    {
                                        CData.Chk_AutoLoading_Count();
                                    }
                                }
                            }
                        }
                        // 2021.05.03 SungTae End

                        return false;
                    }

                case ESeq.ONL_Place:
                    {
                        if (Cyl_Place())
                        {
                            // 2022.04.14 SungTae SungTae : [추가] 
                            if (CData.CurCompany == ECompany.ASE_K12 && CData.IsMultiLOT())
                            {
                                // Error로 인한 MGZ Place 라면
                                if (CData.LotMgr.IsLoaderTrackOutFlag == eTrackOutFlag.Error)
                                    CData.LotMgr.IsLoaderTrackOutFlag = eTrackOutFlag.Valid;
                            }
                            // 2022.04.14 SungTae 

                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Place finish.");
                        }

                        return false;
                    }

                // 2022.04.19 SungTae Start : [추가] ASE-KH On-loader MGZ ID Reading 관련 추가
                case ESeq.ONL_CheckBcr:
                    {
                        if(Cyl_CheckBcr())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< MGZ ID Reading Finish.");
                        }

                        return false;
                    }
                // 2022.04.19 SungTae 
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

            CLog.Save_Log(eLog.ONL, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
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

            CLog.Save_Log(eLog.ONL, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }

        /// <summary>
        /// Axes Servo On/Off Cycle
        /// true:On    false:Off
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

                case 10: // 모든 축 서보 온
                    {
                        CMot.It.Set_SOn(m_iX, bVal);
                        CMot.It.Set_SOn(m_iY, bVal);
                        CMot.It.Set_SOn(m_iZ, bVal);
                        
                        _SetLog("All servo-on : " + bVal);

                        m_iStep++;
                        return false;
                    }

                case 11: // 서보 온 상태 체크
                    {
                        if (CMot.It.Chk_Srv(m_iX) == bVal &&
                            CMot.It.Chk_Srv(m_iY) == bVal &&
                            CMot.It.Chk_Srv(m_iZ) == bVal)
                        {
                            _SetLog("All servo check.");

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
            // Timeout check
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_HOME_ERROR);
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
                        m_bHD = false;
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {
                        // 2021.01.26 lhs Start
                        m_bHD = false;
                        if (Chk_Axes(false))
                        {
                            m_iStep = 0;
                            return true;
                        }
                        //2021.01.26 lhs End

                        if (!CIO.It.Get_X(eX.INR_StripInDetect))
                        {                            
                            CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                            _SetLog("Error : Strip-in detect.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_PusherForward (false       );    // 푸셔 백워드
                        Func_MgzClamp  (true        );    // 클램프 다운
                        Func_TopBeltRun   (false       );    // 탑 벨트 스탑
                        Func_BtmBeltRun(false, false);    // 바텀 벨트 스탑

                        _SetLog("Pusher backward.  Clamp.  Top belt stop.  Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Func_PusherForward(false))    // 클램프 다운 확인 안함 - 매거진 없을 시 클램프 다운 신호 안들어옴
                        { return false; }

                        CMot.It.Mv_H(m_iY);
                        _SetLog("Y axis homing.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!CMot.It.Get_HD(m_iY))
                        { return false; }

                        CMot.It.Mv_H(m_iZ);
                        CMot.It.Mv_H(m_iX);
                        _SetLog("X, Z axis homing.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_HD(m_iZ))
                        { return false; }

                        if (!CMot.It.Get_HD(m_iX))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONL_Z_Wait);
                        CMot.It.Mv_N(m_iY, CData.Dev .dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);
                        CMot.It.Mv_N(m_iX, CData.Dev .dOnL_X_Algn); //190128 : X 대기 위치로 이동
                        _SetLog("X axis move align.", CData.Dev.dOnL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_Wait))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }
                        //190128 : X 대기 위치로 이동(중국 직원 요청)
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnL_X_Algn))
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
                    CErr.Show(eErr.ONLOADER_WAIT_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {                
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : Strip-in detect");

                m_iStep = 0;
                return true;
            }

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

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Pusher Backward, 클램프 다운
                        Func_PusherForward(false);
                        Func_MgzClamp(true);
                        _SetLog("Pusher backward.  Clamp.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//Pusher Backward 확인
                        if ((!Func_PusherForward(false)) && (!Func_MgzClamp(true)))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Y축 대기위치, Z축 대기위치 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dONL_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Z축 대기위치 이동 확인, X축 Algin 위치로 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOnL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//X축 Algin 위치 이동 확인, 종료
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnL_X_Algn))
                        { return false; }

                        // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                        if (CData.IsMultiLOT())
                        {
                            // 작업자에게 LOT close를 묻고있는 과정이 아니라면 WorkEnd 처리
                            if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eNone)
                            {
                                if (CData.LotMgr.LoadMgzTryCnt > CData.LotMgr.MaxLoadMgzTry)
                                {
                                    CData.LotMgr.LoadMgzTryCnt = 0;         // Count Clear
                                    CData.Parts[ONL].iStat = ESeq.ONL_WorkEnd;
                                    _SetLog("Finish. Multi-LOT Use, Status:" + CData.Parts[ONL].iStat);

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                        }
                        // 2021.07.08 SungTae End

                        CData.Parts[ONL].iStat = ESeq.ONL_Pick;

                        _SetLog("Finish.  Status:" + CData.Parts[ONL].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Magazine Pick Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Pick()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_PICK_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {                
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : Strip-in detect.");

                m_iStep = 0;
                return true;
            }

            if (!CIO.It.Get_X(eX.ONL_PusherBackward))//210811 pjh : Pick 시 Backward 상태가 아니면 Error 발생
            {
                CErr.Show(eErr.ONLOADER_PUSHER_FORWARD);
                _SetLog("Error : Pusher forward.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: // 매거진 클램프 감지 여부 확인
                    {
                        // Tack time 측정 시작
                        m_tStTime = DateTime.Now;

                        //Mgz 둘중 하나라도 감지 되고 있는지 확인
                        if (CIO.It.Get_X(eX.ONL_ClampMGZDetect1) || CIO.It.Get_X(eX.ONL_ClampMGZDetect2))
                        {                            
                            CErr.Show(eErr.ONLOADER_CLAMP_DETECTED_MGZ);
                            _SetLog("Error : Clamp detected magazine");

                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11: // 푸셔 백워드, 언클램프
                    {
                        Func_PusherForward(false);
                        Func_MgzClamp(false);
                        _SetLog("Pusher backward. Clamp");

                        m_iStep++;
                        return false;
                    }

                case 12: //푸셔 백워드 확인, 언클램프 확인
                    {
                        if ((!Func_PusherForward(false)) && (!Func_MgzClamp(false)))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOnL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 13: // X축 얼라인 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnL_X_Algn))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14: // X축 얼라인 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PickGo);
                        _SetLog("Z axis move pick.", CData.SPos.dONL_Z_PickGo);

                        m_iStep++;
                        return false;
                    }

                case 15: // Z축 픽 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dONL_Y_Pick);
                        _SetLog("Y axis move pick.", CData.SPos.dONL_Y_Pick);

                        m_iStep++;
                        return false;
                    }

                case 16: // Y축 Pick 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONL_Y_Pick))
                        { return false; }

                        Func_BtmBeltRun(true, false);   // Run, CCW
                        _SetLog("Bottom belt ccw run.");

                        m_iStep++;
                        return false;
                    }

                case 17: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY); // 12000
                        _SetLog(string.Format("Set delay {0}ms", GV.ONL_DETECT_MGZ_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 18: // 딜레이 확인, 매거진 감지 여부 확인
                    {                        
                        if (Chk_ClampMgz() && !m_Delay.Chk_Delay())
                        {
                            // 딜레이 시간 내에 매거진 감지
                            _SetLog("Magazine detected.");
                            
                            // 2021.09.07 lhs Start : 로그 추가
                            _SetLog(string.Format("CData.Parts[ONL].iMGZ_No = {0}, CData.LotInfo.iTotalMgz = {1}", CData.Parts[ONL].iMGZ_No, CData.LotInfo.iTotalMgz));
                            // 2021.09.07 lhs End

                            m_iStep = 50;
                        }
                        else if (!Chk_ClampMgz() && m_Delay.Chk_Delay())
                        {
                            _SetLog("Magazine not detected.");
                            
                            // 2021.09.07 lhs Start : 로그 추가
                            _SetLog(string.Format("CData.Parts[ONL].iMGZ_No = {0}, CData.LotInfo.iTotalMgz = {1}", CData.Parts[ONL].iMGZ_No, CData.LotInfo.iTotalMgz));
                            // 2021.09.07 lhs End

                            m_iStep++;
                        }

                        return false;
                    }

                case 19: // 바닥 벨트 정지
                    {
                        Func_BtmBeltRun(false, false);  // Stop
                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 20: // 바닥 벨트 CW 런
                    {
                        Func_BtmBeltRun(true, true);    // Run, CW
                        _SetLog("Bottom belt cw run.");

                        m_iStep++;
                        return false;
                    }

                case 21: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(GV.ONL_BELT_BACK_DELAY);  // 100
                        _SetLog(string.Format("Set delay {0}ms", GV.ONL_BELT_BACK_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 22: // 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Func_BtmBeltRun(false, false);  // Stop
                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 23: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 24: // Z축 pick 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                        if (CData.IsMultiLOT())
                        {
                            _SetLog("Magazine empty, Check LOT Close");

                            //
                            // !!! Multi-LOT 기능 이용시에만 다음 순번으로 이동하여 처리한다.
                            //

                            m_iStep++;
                            return false;
                        }
                        // 2021.07.08 SungTae End

                        // 2021.09.07 lhs Start : 로그 추가
                        _SetLog(string.Format("CData.Parts[ONL].iMGZ_No = {0}, CData.LotInfo.iTotalMgz = {1}", CData.Parts[ONL].iMGZ_No, CData.LotInfo.iTotalMgz));
                        // 2021.09.07 lhs End

                        // 현재 매거진 번호가 Lot 총 메거진 숫자 보다 적을 경우 에러                    
                        if (CData.Parts[ONL].iMGZ_No <= CData.LotInfo.iTotalMgz)
                        {                            
                            CErr.Show(eErr.ONLOADER_NEED_MGZ); 
                            _SetLog("Error : Need magazine.");
                        }

                        _SetLog("Magazine empty.");

                        m_iStep = 0;
                        return true;
                    }


                case 25: // Multi-LOT 전용
                    {
                        // 새로운 Magazine을 Pickup하지 못하였다.
                        CData.LotMgr.SetLoadMgzPickup(false); 

                        // 지정된 횟수를 넘겼다면,
                        if (CData.LotMgr.IsLotCloseableMgz )       // 연속 Magazine Pickup 실패로 LOT이 종료되는 조건인가 ?)
                        {
                            CData.LotMgr.LoadMgzTryCnt = 0;                 // 재시도 횟수 초기화

                            // 2021.12.24 SungTae Start : [수정] Remote 상태에서의 Multi-LOT 관련 조건 추가
                            if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.SkyWorks)
                            {
                                // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
                                CData.LotMgr.UserConfirmMsg   = $"There is no MGZ to go any further on the onloader.\r\nSelect \"No\" to put in the new LOT's MGZ, or \"Yes\" if you don't want to put in any more.";

                                _SetLog("Multi-LOT, The operator selects whether to add new LOT.");

                                //CData.Parts[ONL].iStat = ESeq.ONL_Wait;
                            }
                            else
                            {
                                CErr.Show(eErr.ONLOADER_NEED_MGZ);              // 알람 발생
                                _SetLog("Multi-LOT, Error : Need magazine.");


                                // 이후로는 LOT 투입종료 조건을 변경시켜준다.
                                //        CData.LotMgr.UserConfirmNextMode = CData.LotMgr.GetLotEndMode();
                                //            CData.LotMgr.UserConfirmMsg = "MGZ has not been deployed. Are you sure you want to exit the input LOT ?";

                                //            // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                //            CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.

                                //
                                // OLD : 무조건 LOT을 종료 시킨다.
                                //
                                //// 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                //if (CData.LotMgr.GetInputCountLoadLot(CData.LotMgr.LoadingLotName) > 0)
                                //{
                                //    _SetLog($"Close loading LOT by Magazine pickup try count over : {CData.LotMgr.LoadingLotName}");
                                //    // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                //    CData.LotMgr.SetCloseLoadLOT();
                                //}


                                CData.Parts[ONL].iStat = ESeq.ONL_Pick;
                                _SetLog("Confirm contiue MGZ Loading");
                                
                                m_iStep = 0;
                                return true;
                            }
                            // 2021.12.24 SungTae End
                        }

                        CData.Parts[ONL].iStat = ESeq.ONL_Wait;
                        _SetLog("Finish.  Status : " + CData.Parts[ONL].iStat);

                        m_iStep = 0;
                        return true;
                    }

                case 50: // Z축 클램프 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_Clamp);

                        _SetLog("Z axis move clamp.", CData.SPos.dONL_Z_Clamp);

                        m_iStep++;
                        return false;
                    }

                case 51: // Z축 클램프 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_Clamp))
                        { return false; }

                        Func_MgzClamp(true);
                        _SetLog("Clamp.");

                        m_iStep++;
                        return false;
                    }

                case 52: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(10000);
                        _SetLog(string.Format("Set delay {0}ms", 10000));

                        m_iStep++;
                        return false;
                    }

                case 53: // 딜레이 확인, 클램프 닫기 확인
                    {
                        if (m_Delay.Chk_Delay() && !Func_MgzClamp(true))
                        {
                            Func_BtmBeltRun(false, true);   // Stop

                            CErr.Show(eErr.ONLOADER_CLAMP_NOT_CLOSE);
                            _SetLog("Error : Clamp not close");

                            m_iStep = 0;
                            return true;
                        }

                        if (!Func_MgzClamp(true))   { return false; }

                        Func_BtmBeltRun(true, true);    // Run, CW
                        _SetLog("Bottom belt cw run.");

                        m_iStep++;
                        return false;
                    }

                case 54: // Z축 Pick up 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PickUp);
                        _SetLog("Z axis move pick up.", CData.SPos.dONL_Z_PickUp);

                        m_iStep++;
                        return false;
                    }

                case 55: // Z축 Pick up 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PickUp))    { return false; }

                        Func_BtmBeltRun(false, false);  // Stop 
                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 56: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 57: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))   { return false; }

                        _SetLog("Y axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 58: // 데이터 전달, 상태 변환
                    {
                        //20191121 ghk_display_strip                        
                        CData.Parts[ONL].bExistStrip = true;

                        CData.Parts[ONL].iMGZ_No++;
                        // 2021.09.07 lhs Start : 로그 추가
                        _SetLog(string.Format("CData.Parts[ONL].iMGZ_No = {0}, CData.LotInfo.iTotalMgz = {1}", CData.Parts[ONL].iMGZ_No, CData.LotInfo.iTotalMgz));
                        // 2021.09.07 lhs End

                        CData.Parts[ONL].iStat = ESeq.ONL_Push;

                        int iCnt = CData.Dev.iMgzCnt;
                        for (int i = 0; i < iCnt; i++)
                        {//매거진 슬롯 상태 정보 처리
                            CData.Parts[ONL].iSlot_info[i] = (int)eSlotInfo.Exist;
                        }

                        //190103 ksg : 연속 빈슬롯 감지시 배출
                        CData.iEmptySlotCnt = 0;
                        CData.bEmptySlotCnt = false;

                        if (CData.Opt.bFirstTopSlot)    { CData.Parts[ONL].iSlot_No = CData.Parts[ONL].iSlot_info.Length; }
                        else                            { CData.Parts[ONL].iSlot_No = 0; }

                        if (CData.GemForm != null)
                        {
						    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt = CData.Parts[ONL].iMGZ_No;
						}
						
                        // 2021-05-10, jhLee, Multi-LOT, 새로운 Magazine이 투입되었다고 지정한다.
                        if (CData.IsMultiLOT())
                        {
                            // Loading용으로 새로운 Magazine이 투입 되었다고 지정한다.
                            CData.LotMgr.SetNewLoadMgz("LOAD_MGZ");

                            // 입력 처리중인 LOT ID 지정
                            CData.Parts[ONL].sLotName = CData.LotMgr.LoadingLotName;
                            CData.Parts[ONL].LotColor = CData.LotMgr.GetLotColor(CData.LotMgr.LoadingLotName);

                            // 새로운 Magazine을 Pickup하였다.
                            CData.LotMgr.SetLoadMgzPickup(true);

							// 2022.03.22 SungTae : [수정] 조건 변경
							// 2021.08.26 SungTae Start : [추가] Remote 상태에서 첫 투입된 MGZ의 경우 LOT Name을 모르기 때문에 MGZ 수량을 1로 설정
                            //if (CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
                            if(!CData.Opt.bSecsUse)
                            {
                                CData.Parts[ONL].nLoadMgzSN = CData.LotMgr.LoadMgzSN;       // Loading된 Magazine의 일련번호
                            }
                            else
                            {
                                if (CData.LotMgr.LoadingLotName == "")  CData.Parts[ONL].nLoadMgzSN = 1;
                                else                                    CData.Parts[ONL].nLoadMgzSN = CData.LotMgr.LoadMgzSN;       // Loading된 Magazine의 일련번호
                            }
                            // 2021.08.26 SungTae End

                            // 2021.12.29 lhs : 이것이 오류의 주범인듯...삭제함.
                            //CData.Parts[ONL].iMGZ_No = 1;                               // 투입된 Magazine 번호

                            _SetLog($"Multi-LOT, LOT Name:{CData.Parts[ONL].sLotName} assigned, MgzSN:{CData.Parts[ONL].nLoadMgzSN}");
                        }
                        else
                        {
                            // 일반적인 LOT 정보 대입 처리
                            CData.Parts[ONL].sLotName = CData.LotInfo.sLotName;
                        }

                        _SetLog("Status change.  Status:" + CData.Parts[ONL].iStat.ToString());

                        m_iStep++;
                        return false;
                    }

                case 59: // 종료, 택 타임 계산
                    {
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog(string.Format("Finish.  {0}", m_tTackTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        
        /// <summary>
        /// Magazine Pick Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PickRfid()
        {// 매거진은 1부터, 슬롯은 0부터 카운트

            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_PICK_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : Strip-in detect.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: //푸셔 백워드, 언클램프
                    {                        
                        if (CIO.It.Get_X(eX.ONL_ClampMGZDetect1) || CIO.It.Get_X(eX.ONL_ClampMGZDetect2))
                        {//Mgz 둘중 하나라도 감지 되고 있는지 확인 (or 검사)
                            CErr.Show(eErr.ONLOADER_CLAMP_DETECTED_MGZ);
                            _SetLog("Error : Magazine detected.");

                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Func_PusherForward(false);
                        Func_MgzClamp(false);
                        _SetLog("Check axes.  Pusher backward.  Unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 11: //푸셔 백워드 확인, 언클램프 확인, X축 얼라인위치 이동
                    {
                        if ((!Func_PusherForward(false)) && (!Func_MgzClamp(false)))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOnL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOnL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 12: //X축 얼라인 위치 이동 확인, Y축 대기 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnL_X_Algn))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13: //Y축 대기 위치 이동 확인, Z축 Pick Go 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PickGo);
                        _SetLog("Z axis move pick.", CData.SPos.dONL_Z_PickGo);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Z축 Pick Go 위치 이동 확인, Y축 Pick 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dONL_Y_Pick);
                        _SetLog("Y axis move pick.", CData.SPos.dONL_Y_Pick);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//Y축 Pick 위치 이동 확인, bottom Belt Run(12초 설정)
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONL_Y_Pick))
                        { return false; }

                        Func_BtmBeltRun(true, false);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);
                        _SetLog("Bottom belt ccw run.  Set delay : " + GV.ONL_DETECT_MGZ_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//매거진 감지 및 12초 경과 여부 확인, 감지 됬을 경우 벨트 정지,
                        if (Chk_ClampMgz() && !m_Delay.Chk_Delay())
                        {//MGZ 센서 둘다 감지 되었는지 확인(And 검사)
                            Func_BtmBeltRun(false, false);
                            //감지 됬을 경우 21번으로 이동
                            _SetLog("Magazine detect.  Bottom belt stop.");

                            m_iStep = 21;
                        }
                        else if (!Chk_ClampMgz() && m_Delay.Chk_Delay())
                        {//MGZ 센서 둘다 감지 되지 않았을 경우
                            _SetLog("Magazine not detect.");

                            m_iStep++; 
                        }

                        return false;
                    }

                case 17:
                    {//감지 안되었을 경우 Bottom Belt 반대 방향으로 지정 한 시간 만큼 Run
                        Func_BtmBeltRun(true, true);
                        m_Delay.Set_Delay(GV.ONL_BELT_BACK_DELAY); //옵션에서 Delay값
                        _SetLog("Bottom belt cw run.  Set delay : " + GV.ONL_BELT_BACK_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//시간 지난 후 벨트 정지
                        if (m_Delay.Chk_Delay())
                        {
                            Func_BtmBeltRun(false, false);
                            _SetLog("Bottom belt stop.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 19:
                    {//Y축 대기 위치 이동 현재 매거진 번호가 Lot 총 메거진 숫자 보다 적을 경우 에러
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                        // 2021-05-28, jhLee, Multi-LOT, 지정 횟수까지 Magazine이 채워지지 않는다면 현재 진행중인 LOT Close 시킨다.
                        if (CData.IsMultiLOT())
                        {
                            _SetLog("Magazine empty, Check LOT Close");

                            //
                            // !!! Multi-LOT 기능 이용시에만 다음 순번으로 이동하여 처리한다.
                            //

                            m_iStep++;
                            return false;
                        }

                        if (CData.Parts[ONL].iMGZ_No <= CData.LotInfo.iTotalMgz)
                        { 
                            CErr.Show(eErr.ONLOADER_NEED_MGZ);
                            _SetLog("Error : Need magazine.");
                        }

                        m_iStep = 0;
                        return true;
                    }


                case 20: // Multi-LOT 전용
                    {
                        // 새로운 Magazine을 Pickup하지 못하였다.
                        CData.LotMgr.SetLoadMgzPickup(false);

                        // 지정된 횟수를 넘겼다면,
                        if (CData.LotMgr.IsLotCloseableMgz)       // 연속 Magazine Pickup 실패로 LOT이 종료되는 조건인가 ?)
                        {
                            // 만약 Loading 중인 LOT이 있다면, 해당 LOT은 Close 시켜준다. (투입 종료)
                            if (CData.LotMgr.LoadingLotName != "")
                            {
                                // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                if (CData.LotMgr.GetInputCountLoadLot(CData.LotMgr.LoadingLotName) > 0)
                                {
                                    string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                                    _SetLog($"Close loading LOT by Magazine pickup try count over : {CData.LotMgr.LoadingLotName}");
                                    // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                    CData.LotMgr.SetCloseLoadLOT();
                                    CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                                }
                            }
                        }

                        CData.Parts[ONL].iStat = ESeq.ONL_Wait;
                        
                        _SetLog("Finish.  Status : " + CData.Parts[ONL].iStat);

                        m_iStep = 0;
                        return true;
                    }
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


                case 21:
                    {//매거진 감지 되었을 경우 Z축 매거진 클램프 위치로 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_Clamp);
                        _SetLog("Z axis move clamp.", CData.SPos.dONL_Z_Clamp);

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//Z축 매거진 클램프 위치로 이동 확인, 클램프 다운
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_Clamp))
                        { return false; }

                        Func_MgzClamp(true);
                        m_Delay.Set_Delay(10000);
                        
                        _SetLog("Clamp.  Set delay : 10000ms");

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {//클램프 다운 확인, 벨트 런, Z축 Pick Up 위치로 이동
                        //190514 ksg :
                        if(m_Delay.Chk_Delay() && !Func_MgzClamp(true))
                        {
                            // 2021.06.16 SungTae Start : [수정] Error명 오타로 변경 (ONLOADER_CLAMP_NOT_CLOASE -> ONLOADER_CLAMP_NOT_CLOSE)
                            //CErr.Show(eErr.ONLOADER_CLAMP_NOT_CLOASE);
                            CErr.Show(eErr.ONLOADER_CLAMP_NOT_CLOSE);
                            // 2021.06.16 SungTae End

                            _SetLog("Error : Not clamp.");

                            m_iStep = 0;
                            return true;
                        }
                        if (!Func_MgzClamp(true))
                        { return false; }

                        Func_BtmBeltRun(true, true);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PickUp);
                        _SetLog("Z axis move pick up.", CData.SPos.dONL_Z_PickUp);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//Z축 Pick UP 위치로 이동 확인, 벨트 정지, 매거진 상태 및 번호 변경
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PickUp))
                        { return false; }

                        Func_BtmBeltRun(false, false);

                        m_iReReadCnt = 0; //200120 ksg :
                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }
                //200120 ksg :    
                case 25:
                    {//RFID 리딩 
                        if(m_iReReadCnt < 5)
                        {
                            CRfid.It.ReciveMsgOnl = "";
                            CRfid.It.ReadRfidOnl(true);
                            m_Delay.Set_Delay(500);
                            m_iReReadCnt++;

                            _SetLog("Read RFID.  Count : " + m_iReReadCnt);
                            m_iStep++;
                        }
                        else
                        {
                            CLog.mLogGnd(string.Format("CSQ1_OnL Step - 25 Rfid Read Fail m_iReReadCnt->  {0}  ",m_iReReadCnt));
                            m_iReReadCnt = 0;
                            
                            CErr.Show(eErr.ONLOADER_CAN_NOT_READ_RFID);
                            _SetLog("Error : Can't read RFID.");

                            m_iStep = 0;
                            return true;
                        }

                        return false;
                    }
                //200120 ksg:
                case 26:
                    {//RFID Check
                        if(!m_Delay.Chk_Delay()) return false;
                        if(CRfid.It.ReciveMsgOnl == "")
                        {
                            m_iStep = 25;
                            return false;
                        }

                        CData.Parts[ONL].sMGZ_ID = CRfid.It.ReciveMsgOnl;
                        _SetLog("RFID : " + CRfid.It.ReciveMsgOnl);

                        //200131 LCY                        
                        if(CData.GemForm != null)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID = CRfid.It.ReciveMsgOnl;
                            
							if(!CData.bChkManual)
                            {
								CData.GemForm.LoaderMgzVerifyRequest(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID, (uint)SECSGEM.JSCK.eMGZPort.LOADER); // Loader MGZ Verify 진행 요청
                            
                            	// 2022.03.31 SungTae Start : [추가] Log 추가
                            	_SetLog($"[LOADER][SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request).  MgzID : {CData.Parts[ONL].sMGZ_ID}, Mgz Port : {(uint)SECSGEM.JSCK.eMGZPort.LOADER}.");
                            	// 2022.03.31 SungTae End
							}

                            CLog.mLogGnd(string.Format("CSQ1_OnL Step - 26 CData.JSCK_Gem_Data[1].sLD_MGZ_ID {0} Verify Req ", CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID));
                        }

                        m_Delay.Set_Delay(60000);
                        _SetLog("Set delay : 60000ms");
                        
                        m_iStep++;
                        return false;
                    }

                case 27:
                    {
                        if (CData.Opt.bSecsUse)
                        {
                            // SECSGEM 사용 시 Verify 완료 여부 확인
                            if (m_Delay.Chk_Delay())
                            {
                                // Verify 진행 60 초 Timeout 발생
                                CLog.mLogGnd(string.Format("CSQ1_OnL Step - 27 Rfid Verify Error 001 "));
                                
                                m_iStep = 0;
                                
                                CErr.Show(eErr.HOST_LD_MGZ_VERIFY_TIME_OVER_ERROR);
                                return true;
                            }
                            else if (CData.SecsGem_Data.nLdMGZ_Check != Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Complete)/*2*/)
                            {
                                // Verify 진행 중.
                                if (CData.SecsGem_Data.nLdMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Error)/*-1*/)
                                {
                                    // Verify Error 발생
                                    CLog.mLogGnd(string.Format("CSQ1_OnL Step - 27 Rfid Verify Error 002 "));
                                    
                                    m_iStep = 0;
                                    
                                    CErr.Show(eErr.HOST_LD_MGZ_VERIFY_TIME_OVER_ERROR);
                                    return true;
                                }
                                else if (CData.SecsGem_Data.nLdMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Run)/*1*/)
								{
									return false;
								}
                            }
							else
							{                                
                                if(CData.GemForm != null)
									CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt++;// Loader MGZ Count 증가
                            }
                        }
						else
						{
                            // secsgem 사용 하지 않지만 값 대입 
                            if(CData.GemForm != null)
								CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt++;// Loader MGZ Count 증가 시켜도 됨
                        }
                        
                        if(CData.GemForm != null)
                        {
						    CLog.mLogGnd(string.Format("CSQ1_OnL Step - 26 CData.JSCK_Gem_Data[1].sLD_MGZ_ID {0}  Cnt {1} Verify Finish ",
                                                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID,
                                                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt));
                        }
						
						_SetLog("SECS/GEM verify finish.");

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {//Y축 대기위치 이동, 종료
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 29:
                    {// Y축 대기위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }
                    
                        CData.Parts[ONL].bExistStrip = true;
                        CData.Parts[ONL].iMGZ_No++;
                        CData.Parts[ONL].iStat = ESeq.ONL_Push;

                        int iCnt = CData.Dev.iMgzCnt;
                        for (int i = 0; i < iCnt; i++)
                        {//매거진 슬롯 상태 정보 처리
                            CData.Parts[ONL].iSlot_info[i] = (int)eSlotInfo.Exist;
                        }
                        //190103 ksg : 연속 빈슬롯 감지시 배출
                        CData.iEmptySlotCnt = 0    ;
                        CData.bEmptySlotCnt = false;
                        
                        if(CData.Opt.bFirstTopSlot) CData.Parts[ONL].iSlot_No = CData.Parts[ONL].iSlot_info.Length;
                        else                        CData.Parts[ONL].iSlot_No = 0;

                        // 2021-05-10, jhLee, Multi-LOT, 새로운 Magazine이 투입되었다고 지정한다.
                        if (CData.IsMultiLOT())
                        {
                            // Loading용으로 새로운 Magazine이 투입 되었다고 지정한다.
                            CData.LotMgr.SetNewLoadMgz(CData.Parts[ONL].sMGZ_ID);             // BCR을 통해 읽어들인 Magazine ID 대입

                            // 입력 처리중인 LOT ID 지정
                            CData.Parts[ONL].sLotName = CData.LotMgr.LoadingLotName;
                            CData.Parts[ONL].LotColor = CData.LotMgr.GetLotColor(CData.LotMgr.LoadingLotName);
                            
                            // 새로운 Magazine을 Pickup하였다.
                            CData.LotMgr.SetLoadMgzPickup(true);

                            // Remote 상태에서 첫 투입된 MGZ의 경우 LOT Name을 모르기 때문에 MGZ 수량을 1로 설정
                            if (!CData.Opt.bSecsUse)
                            {
                                CData.Parts[ONL].nLoadMgzSN = CData.LotMgr.LoadMgzSN;       // Loading된 Magazine의 일련번호
                            }
                            else
                            {
                                if (CData.LotMgr.LoadingLotName == "")  CData.Parts[ONL].nLoadMgzSN = 1;
                                else                                    CData.Parts[ONL].nLoadMgzSN = CData.LotMgr.LoadMgzSN;       // Loading된 Magazine의 일련번호
                            }

                            _SetLog($"Multi-LOT, LOT Name:{CData.Parts[ONL].sLotName} assigned, MgzSN:{CData.Parts[ONL].nLoadMgzSN}");
                        }
                        else
                        {
                            // 일반적인 LOT 정보 대입 처리
                            CData.Parts[ONL].sLotName = CData.LotInfo.sLotName;
                        }


                        _SetLog("Finish.  Status : " + CData.Parts[ONL].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        /// <summary>
        /// Magazine Pick Cycle (매거진 BCR 읽음)
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PickBcr()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {                    
                    CErr.Show(eErr.ONLOADER_PICK_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    // 2022.05.11 SungTae Start : [추가] ASE-KH Multi-LOT 관련
                    if (CData.CurCompany == ECompany.ASE_K12)   { CData.LotMgr.IsLoaderTrackOutFlag = eTrackOutFlag.Error; }
                    // 2022.05.11 SungTae End

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {                
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : Strip-in detect.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: // 매거진 클램프 감지 여부 확인
                    {
                        // Tack time 측정 시작
                        m_tStTime = DateTime.Now;

                        // 2022.04.26 SungTae Start : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련 Track Out Flag 확인
                        if(CData.CurCompany == ECompany.ASE_K12 && CData.IsMultiLOT())
                        {
                            if (CData.LotMgr.IsLoaderTrackOutFlag == eTrackOutFlag.Error)
                            {
                                string sMsg = $"[ON-LOADER]\nPlease select one of the buttons below :\n\n" +
                                              $"1) [OK] - Invalid MGZ information received from host. Eject the loaded MGZ.\n            (MGZ ID : {CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr})\n" +
                                              $"2) [CANCEL] - Retry reading MGZ ID.";

                                if (CMsg.Show(eMsg.Warning, "Warning", sMsg) == DialogResult.OK)
                                {
                                    m_iStep = 100;      // Track-Out Step으로 이동
                                }
                                else
                                {
                                    m_iStep = 66;       // Auto Run 재시작 시 MGZ ID Read 하는 Step으로 이동
                                }

                                CData.LotMgr.IsLoaderTrackOutFlag = eTrackOutFlag.Valid;
                                return false;
                            }
                        }
                        // 2022.04.14 SungTae End

                        //Mgz 둘중 하나라도 감지 되고 있는지 확인
                        if (CIO.It.Get_X(eX.ONL_ClampMGZDetect1) || CIO.It.Get_X(eX.ONL_ClampMGZDetect2))
                        {
                            CErr.Show(eErr.ONLOADER_CLAMP_DETECTED_MGZ);
                            _SetLog("Error : Clamp detected magazine");

                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Chekc axes.");

                        m_iStep++;
                        return false;
                    }

                case 11: // 푸셔 백워드, 언클램프
                    {
                        Func_PusherForward(false);
                        Func_MgzClamp(false);

                        _SetLog("Pusher backward, Clamp open");

                        m_iStep++;
                        return false;
                    }

                case 12: //푸셔 백워드 확인, 언클램프 확인
                    {
                        if ((!Func_PusherForward(false)) && (!Func_MgzClamp(false)))
                        { return false; }
                        
                        CMot.It.Mv_N(m_iX, CData.Dev.dOnL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOnL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 13: // X축 얼라인 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOnL_X_Algn))
                        { return false; }
                        
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }
                        
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PickGo);
                        _SetLog("Z axis move pick.", CData.SPos.dONL_Z_PickGo);

                        m_iStep++;
                        return false;
                    }

                case 15: // Z축 픽 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PickGo))
                        { return false; }
                        
                        CMot.It.Mv_N(m_iY, CData.SPos.dONL_Y_Pick);
                        _SetLog("Y axis move pick.", CData.SPos.dONL_Y_Pick);

                        m_iStep++;
                        return false;
                    }

                case 16: // Y축 Pick 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONL_Y_Pick))
                        { return false; }
                        
                        Func_BtmBeltRun(true, false);
                        _SetLog("Bottom belt ccw run.");

                        m_iStep++;
                        return false;
                    }

                case 17: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);
                        _SetLog(string.Format("Set delay : {0}ms", GV.ONL_DETECT_MGZ_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 18: // 딜레이 확인, 매거진 감지 여부 확인
                    {                        
                        if (Chk_ClampMgz() && !m_Delay.Chk_Delay())
                        {
                            // 딜레이 시간 내에 매거진 감지
                            _SetLog("Magazine detected.");

                            m_iStep = 50;
                        }
                        else if (!Chk_ClampMgz() && m_Delay.Chk_Delay())
                        {
                            _SetLog("Magazine not detected.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 19: // 바닥 벨트 정지
                    {
                        Func_BtmBeltRun(false, false);
                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 20: // 바닥 벨트 CW 런
                    {
                        Func_BtmBeltRun(true, true);
                        _SetLog("Bottom belt cw run.");

                        m_iStep++;
                        return false;
                    }

                case 21: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(GV.ONL_BELT_BACK_DELAY);
                        _SetLog(string.Format("Set delay : {0}ms", GV.ONL_BELT_BACK_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 22: // 딜레이 확인
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }
                        
                        Func_BtmBeltRun(false, false);
                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 23: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 24: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))   { return false; }

                        // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                        // 2021-05-28, jhLee, Multi-LOT, 지정 횟수까지 Magazine이 채워지지 않는다면 현재 진행중인 LOT Close 시킨다.
                        if (CData.IsMultiLOT())
                        {
                            _SetLog("Magazine empty, Check LOT Close");

                            //
                            // !!! Multi-LOT 기능 이용시에만 다음 순번으로 이동하여 처리한다.
                            //

                            m_iStep++;
                            return false;
                        }

                        if (CData.Parts[ONL].iMGZ_No <= CData.LotInfo.iTotalMgz)
                        {                            
                            CErr.Show(eErr.ONLOADER_NEED_MGZ); 
                            _SetLog("Error : Need magazine.");
                        }

                        _SetLog("Magazine empty");

                        m_iStep = 0;
                        return true;
                    }


                case 25: // Multi-LOT 전용
                    {
                        // 새로운 Magazine을 Pickup하지 못하였다.
                        CData.LotMgr.SetLoadMgzPickup(false);

                        // 2022.05.10 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련
                        // 지정된 횟수를 넘겼다면,
                        if (CData.LotMgr.IsLotCloseableMgz)       // 연속 Magazine Pickup 실패로 LOT이 종료되는 조건인가 ?)
                        {
                            CData.LotMgr.LoadMgzTryCnt = 0;                 // 재시도 횟수 초기화

                            // 만약 Loading 중인 LOT이 있다면, 해당 LOT은 Close 시켜준다. (투입 종료)
                            //if (CData.LotMgr.LoadingLotName != "")
                            //{
                            //    // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                            //    if (CData.LotMgr.GetInputCountLoadLot(CData.LotMgr.LoadingLotName) > 0)
                            //    {
                            //        string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                            //        _SetLog($"Close loading LOT by Magazine pickup try count over : {CData.LotMgr.LoadingLotName}");
                            //        // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                            //        CData.LotMgr.SetCloseLoadLOT();
                            //        CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                            //    }
                            //}

                            if (CData.CurCompany == ECompany.ASE_K12)
                            {
                                // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
                                CData.LotMgr.UserConfirmMsg = $"There is no MGZ to go any further on the onloader.\r\nSelect \"No\" to put in the new LOT's MGZ, or \"Yes\" if you don't want to put in any more.";

                                _SetLog("Multi-LOT, The operator selects whether to add new LOT.");
                            }
                            else
                            {
                                CErr.Show(eErr.ONLOADER_NEED_MGZ);              // 알람 발생
                                _SetLog("Multi-LOT, Error : Need magazine.");

                                CData.Parts[ONL].iStat = ESeq.ONL_Pick;
                                _SetLog("Confirm contiue MGZ Loading");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        // 2022.05.10 SungTae End
                                
                        CData.bChkManual = false;   // 2022.04.25 SungTae : [추가] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag 초기화
                         
                        CData.Parts[ONL].iStat = ESeq.ONL_Wait;
                        
                        _SetLog("Cyl_PickBcr() Finish.  Status : " + CData.Parts[ONL].iStat);

                        m_iStep = 0;
                        return true;
                   }


                case 50: // Z축 클램프 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_Clamp);
                        _SetLog("Z axis move clamp.", CData.SPos.dONL_Z_Clamp);

                        m_iStep++;
                        return false;
                    }

                case 51: // Z축 클램프 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_Clamp))
                        { return false; }
                        
                        Func_MgzClamp(true);
                        _SetLog("Clamp close.");

                        m_iStep++;
                        return false;
                    }

                case 52: // 딜레이 설정
                    {
                        m_Delay.Set_Delay(10000);
                        _SetLog(string.Format("Set delay : {0}ms", 10000));

                        m_iStep++;
                        return false;
                    }

                case 53: // 딜레이 확인, 클램프 닫기 확인
                    {
                        if (m_Delay.Chk_Delay() && !Func_MgzClamp(true))
                        {
                            CErr.Show(eErr.ONLOADER_CLAMP_NOT_CLOSE);
                            
                            CData.bChkManual = false;   // 2022.04.25 SungTae : [추가] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag 초기화

                            _SetLog("Error : Clamp not close.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!Func_MgzClamp(true))   { return false; }
                        
                        Func_BtmBeltRun(true, true);
                        
                        _SetLog("Bottom belt cw run.");

                        m_iStep++;
                        return false;
                    }

                case 54: // Z축 Pick up 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PickUp);
                        _SetLog("Z axis move pick.", CData.SPos.dONL_Z_PickUp);

                        m_iStep++;
                        return false;
                    }

                case 55: // Z축 Pick up 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PickUp))    { return false; }
                        
                        Func_BtmBeltRun(false, false);

                        _SetLog("Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 56: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 57: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))   { return false; }

                        _SetLog("Y axis move check");

                        //////////////////////////////////////////////
                        m_iReReadCnt = 0; //바코드 판독 시도 회수 초기화
                        //////////////////////////////////////////////

                        m_iStep++;
                        return false;
                    }

                // 반복 Loop //////////////////////////////////////////////////////////////////////

                case 58: // 바코드 읽기 시퀀스 돌입
                    {
                        if(m_iReReadCnt > 0) //재시도인 경우 바코드 읽기 중지(Timing OFF) => Z축 +30 mm (아래로) => Z축 바코드 위치 => 바코드 읽기
                        { 
                            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)
                            _SetLog("MGZ BCR timing off.");

                            m_iStep++;
                            return false;
                        }

                        m_iStep = 62; //바코드 읽기
                        return false;
                    }

                case 59: // Z축 (바코드 위치 + 10 mm) 이동 (아래로)
                    {
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOnL_Z_MgzBcr + 10));

                        _SetLog("Z axis move.", (CData.Dev.dOnL_Z_MgzBcr + 10));

                        m_iStep++;
                        return false;
                    }

                case 60: // Z축 (바코드 위치 + 10 mm) 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOnL_Z_MgzBcr + 10)))
                        { return false; }

                        m_Delay.Set_Delay(100); //이동 후 0.1초 대기
                        _SetLog("Set delay : 100ms");

                        m_iStep++;
                        return false;
                    }

                case 61: //Delay 체크
                    {
                        if(!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay.");

                        m_iStep++;
                        return false;
                    }

                // 바코드 읽기 /////////////////////////////////////////////////

                //20200611 jhc : Z축, Y축 이동 순서 변경 (Y->Z ==> Z->Y)

                case 62: // Z축 바코드 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnL_Z_MgzBcr);

                        _SetLog("Z axis move bcr.", CData.Dev.dOnL_Z_MgzBcr);

                        m_iStep++;
                        return false;
                    }

                case 63: // Z축 바코드 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnL_Z_MgzBcr)) { return false; }

                        _SetLog("Z axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 64: // Y축 바코드 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_MgzBcr);

                        _SetLog("Y axis move bcr.", CData.Dev.dOnL_Y_MgzBcr);

                        m_iStep++;
                        return false;
                    }

                case 65: // Y축 바코드 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_MgzBcr)) { return false; }

                        _SetLog("Y axis move check.");

                        m_iStep++;
                        return false;
                    }
                //

                case 66: // 바코드 읽기
                    {
                        CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr = ""; //BCR 값 초기화

                        CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LON); //바코드 읽기 요청 (Timing ON)

                        ////////////////////////////////////////////
                        m_iReReadCnt++; //바코드 판독 시도 회수 1 증가
                        ////////////////////////////////////////////

                        m_Delay.Set_Delay(3000); //최대 대기 시간                        
                        _SetLog("MGZ BCR read start.  Set delay : 3000ms");

                        m_iStep++;
                        return false;
                    }

                case 67: // 바코드 읽기 완료 체크
                    {
                        if (!CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr.Equals(""))
                        {
                            /////////////////////
                            // 바코드 판독 완료 //

                            CData.Parts[ONL].sMGZ_ID = CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr;

                            _SetLog(string.Format("MGZ BCR read ok [{0}].  Read count : {1}", CData.Parts[ONL].sMGZ_ID, m_iReReadCnt));

                            //200131 LCY                        
                            if (CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID = CData.Parts[ONL].sMGZ_ID;

                                // 2022.04.25 SungTae Start : [수정] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag 확인
                                if (!CData.bChkManual)
                                {
                                    CData.GemForm.LoaderMgzVerifyRequest(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID, (uint)SECSGEM.JSCK.eMGZPort.LOADER); // Loader MGZ Verify 진행 요청

                                	// 2022.03.31 SungTae Start : [추가] Log 추가
                                	_SetLog($"[LOADER][SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request).  MgzID : {CData.Parts[ONL].sMGZ_ID}, Mgz Port : {(uint)SECSGEM.JSCK.eMGZPort.LOADER}.");
                                	// 2022.03.31 SungTae End
								}
								// 2022.04.25 SungTae End

                                // 2022.04.07 SungTae : [수정] Log 변경
                                //CLog.mLogGnd(string.Format("CSQ1_OnL Step - 70 CData.JSCK_Gem_Data[1].sLD_MGZ_ID {0} Verify Req ",CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID));
                                CLog.mLogGnd($"CSQ1_ONL Step(67) - [SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request). sLD_MGZ_ID : {CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID}");
                            }

                            m_Delay.Set_Delay(60000);

                            m_iStep++;
                            return false;
                        }

                        // 타임 아웃 체크
                        if (!m_Delay.Chk_Delay())   { return false; }

                        /////////////
                        // 타임아웃 //

                        if (m_iReReadCnt < 3)
                        {
                            _SetLog(string.Format("MGZ BCR read fail -> retry.  Read count : {0}", m_iReReadCnt));

                            m_iStep = 58; //매거진 바코드 읽기 재시도
                            return false;
                        }
                        else
                        {
                            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)
                            _SetLog("MGZ BCR timing off");

                            //바코드 읽기 시도 회수 초과 => Error
                            _SetLog(string.Format("MGZ BCR read fail (Read count over).  Read count : {0}", m_iReReadCnt));

                            m_iReReadCnt = 0;

                            m_iStep = 0;
                            //20200611 jhc : 메시지 표시 변경
                            CMsg.Show(eMsg.Error, "OnLoader magazine BCR read fail", "1.Check the direction of the magazine.\r2.Please confirm that BCR module works normally.");
                            return true;
                        }
                    }

                case 68:
                    {
                        if (CData.Opt.bSecsUse)
                        {
                            // SECSGEM 사용 시 Verify 완료 여부 확인
                            if (m_Delay.Chk_Delay())
                            { // Verify 진행 60 초 Timeout 발생
                                // 2022.04.07 SungTae : [수정] Log 변경
                                //CLog.mLogGnd(string.Format("CSQ1_OnL Step - 71 BCR Verify Error 001 "));
                                CLog.mLogGnd($"CSQ1_ONL Step(68) - Check Magazine Verify Request State (Timeout Happen) : {(int)SECSGEM.JSCK.eChkVerify.Error}({SECSGEM.JSCK.eChkVerify.Error})");

                                CData.LotMgr.IsLoaderTrackOutFlag = eTrackOutFlag.Error;    // 2022.04.26 SungTae : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련

                                m_iStep = 0;
                                CErr.Show(eErr.HOST_LD_MGZ_VERIFY_TIME_OVER_ERROR);
                                return true;
                            }
                            else if (CData.SecsGem_Data.nLdMGZ_Check != Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Complete)/*2*/)
                            {// Verify 진행 중.
                                // 2022.04.07 SungTae Start : [수정] Log 변경 및 추가
                                if (CData.SecsGem_Data.nLdMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Error)/*-1*/)
                                {// Verify Error 발생
                                    //CLog.mLogGnd(string.Format("CSQ1_OnL Step - 71 BCR Verify Error 002 "));
                                    CLog.mLogGnd($"CSQ1_ONL Step(68) - Check Magazine Verify Request State : {(int)SECSGEM.JSCK.eChkVerify.Error}({SECSGEM.JSCK.eChkVerify.Error})");

                                    CData.LotMgr.IsLoaderTrackOutFlag = eTrackOutFlag.Error;    // 2022.04.26 SungTae : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련

                                    m_iStep = 0;
                                    CErr.Show(eErr.HOST_LD_MGZ_VERIFY_TIME_OVER_ERROR);
                                    return true;
                                }
                                else if (CData.SecsGem_Data.nLdMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Run)/*1*/)
                                {
                                    CLog.mLogGnd($"CSQ1_ONL Step(68) - Check Magazine Verify Request State : {(int)SECSGEM.JSCK.eChkVerify.Run}({SECSGEM.JSCK.eChkVerify.Run})");
                                    return false;
                                }
                                // 2022.04.07 SungTae End
                            }
                            else
                            {
                                if (CData.GemForm != null)
                                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt++;// Loader MGZ Count 증가
                            }
                        }
                        else
                        {
                            // secsgem 사용 하지 않지만 값 대입 
                            if (CData.GemForm != null)
                                CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt++;// Loader MGZ Count 증가 시켜도 됨
                        }

                        if (CData.GemForm != null)
                        {
                            // 2022.04.07 SungTae : [수정] Log 변경
                            //CLog.mLogGnd(string.Format("CSQ1_OnL Step - 71 CData.JSCK_Gem_Data[1].sLD_MGZ_ID {0}  Cnt {1} Verify Finish ",
                            //                            CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID,
                            //                            CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt));

                            _SetLog($"CSQ1_ONL Step(68) - [RECV](H->E) S6F12 (Magazine Verify Request Acknowledge). sLD_MGZ_ID : {CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID}, " +
                                    $"nLD_MGZ_Cnt : {CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt}");
                        }

                        _SetLog("SECSGEM verify finish");

                        m_iStep++;
                        return false;
                    }

                case 69: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 70: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))   { return false; }

                        _SetLog("Y axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 71: // 데이터 전달, 상태 변환
                    {
                        //20191121 ghk_display_strip                        
                        CData.Parts[ONL].bExistStrip = true;

                        CData.Parts[ONL].iMGZ_No++;
                        CData.Parts[ONL].iStat = ESeq.ONL_Push;

                        int iCnt = CData.Dev.iMgzCnt;

                        for (int i = 0; i < iCnt; i++)
                        {//매거진 슬롯 상태 정보 처리
                            CData.Parts[ONL].iSlot_info[i] = (int)eSlotInfo.Exist;
                        }

                        //190103 ksg : 연속 빈슬롯 감지시 배출
                        CData.iEmptySlotCnt = 0;
                        CData.bEmptySlotCnt = false;

                        if (CData.Opt.bFirstTopSlot)    CData.Parts[ONL].iSlot_No = CData.Parts[ONL].iSlot_info.Length;
                        else                            CData.Parts[ONL].iSlot_No = 0;
                        
                        if (CData.GemForm != null) CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].nLD_MGZ_Cnt = CData.Parts[ONL].iMGZ_No;

                        // 2021-05-14, jhLee, Multi-LOT, 새로운 Magazine이 투입되었다고 지정한다.
                        if (CData.IsMultiLOT())
                        {
                            // 2022.05.10 SungTae Start : [수정] ASE-KH Multi-LOT 관련
                            // Loading용으로 새로운 Magazine이 투입 되었다고 지정한다.
                            CData.LotMgr.SetNewLoadMgz(CData.Parts[ONL].sMGZ_ID);             // BCR을 통해 읽어들인 Magazine ID 대입

                            // 입력 처리중인 LOT ID 지정
                            CData.Parts[ONL].sLotName = CData.LotMgr.LoadingLotName;

                            if (!CData.Opt.bSecsUse)
                            {
                                CData.Parts[ONL].LotColor = CData.LotMgr.GetLotColor(CData.LotMgr.LoadingLotName);
                            }
                            else
                            {
                                CData.Parts[ONL].LotColor = CData.LotMgr.GetLotTypeColor(CData.LotMgr.ListLOTInfo.Count);
                            }

                            // 새로운 Magazine을 Pickup하였다.
                            CData.LotMgr.SetLoadMgzPickup(true);
                            
                            CData.Parts[ONL].nLoadMgzSN = CData.LotMgr.LoadMgzSN;       // Loading된 Magazine의 일련번호
                            // 2022.05.10 SungTae End

                            _SetLog($"Multi-LOT, LOT Name:{CData.Parts[ONL].sLotName} assigned, MgzSN:{CData.Parts[ONL].nLoadMgzSN}");
                        }
                        else
                        {
                            // 일반적인 LOT 정보 대입 처리
                            CData.Parts[ONL].sLotName = CData.LotInfo.sLotName;
                        }

                        _SetLog("Status change.  Status : " + CData.Parts[ONL].iStat.ToString());

                        m_iStep++;
                        return false;
                    }

                case 72: // 종료, 택 타임 계산
                    {

                        // 2022.04.25 SungTae Start : [수정] Manual 동작 시 SECS/GEM Issue 관련 Flag 초기화
                        CData.bChkManual = false;
                        // 2021.10.19 SungTae End

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog(string.Format("Finish.  Tack time : {0}", m_tTackTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }

                // 2022.04.26 SungTae Start : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련 Track Out 처리 부분 추가
                ////////////////////////////////////////// [ TRACK-OUT STEP] //////////////////////////////////////////
                case 100:
                    {
                        _SetLog($"[TRACK-OUT] Loaded MGZ Track-out Start... MGZ ID : {CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID}");

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PlaceGo);

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                            _SetLog("[TRACK-OUT] Error : Top magazine full.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))    //X9C Top Conveyor Mgz Detect
                        {
                            Func_TopBeltRun(true);
                            _SetLog("[TRACK-OUT] Top belt run.");
                        }

                        _SetLog("[TRACK-OUT] Z axis move place.", CData.SPos.dONL_Z_PlaceGo);

                        m_iStep++;
                        return false;
                    }

                case 101:   //Top 매거진 감지 센서 감지 안될 때까지 런, Z축 PlaceGo 위치 이동 확인, 딜레이 2000ms 설정
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PlaceGo))   { return false; }

                        //X9C Top Conveyor Mgz Detect
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect)) { Func_TopBeltRun(true); }
                        else                                    { Func_TopBeltRun(false); }

                        m_Delay.Set_Delay(2000);
                        _SetLog("[TRACK-OUT] Set delay : 2000ms");

                        m_iStep++;
                        return false;
                    }

                case 102:   //Top 매거진 감지 센서 감지 안될 때까지 2초 동안 런, Top 매거진 Full센서 감지시 에러, Top 매거진 감지 센서 감지 시 벨트 정지
                    {
                        if (m_Delay.Chk_Delay())
                        {
                            Func_TopBeltRun(false);

                            // 2초가 지난 후에도 매거진 감지 또는 풀 매거진 감지 시  에러 발생
                            if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) || !CIO.It.Get_X(eX.ONL_TopMGZDetectFull))    //X9D Top Conveyor Full Mgz
                            {
                                CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                                
                                _SetLog("[TRACK-OUT] Error : Top magazine full.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                        {
                            if (CIO.It.Get_X(eX.ONL_TopMGZDetect) && CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                            {
                                // 탑 벨트 스탑
                                Func_TopBeltRun(false);
                                _SetLog("[TRACK-OUT] Top belt stop.");

                                m_iStep++;
                            }
                        }

                        return false;
                    }

                case 103:   //Y축 Place 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dONL_Y_Place);
                        
                        _SetLog("[TRACK-OUT] Y axis move place.", CData.SPos.dONL_Y_Place);

                        m_iStep++;
                        return false;
                    }

                case 104:   //Y축 Place 위치 이동 확인, Z축 UnClamp 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONL_Y_Place)) { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_UnClamp);

                        _SetLog("[TRACK-OUT] Z axis move Unclamp.", CData.SPos.dONL_Z_UnClamp);

                        m_iStep++;
                        return false;
                    }

                case 105:   //Z축 UnClamp 위치 이동 확인, 클램프 업
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_UnClamp))   { return false; }

                        Func_MgzClamp(false);

                        _SetLog("[TRACK-OUT] MGZ Unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 106:   //클램프 업 확인, 푸셔 백워드
                    {
                        if (!Func_MgzClamp(false))  { return false; }

                        Func_PusherForward(false);

                        _SetLog("[TRACK-OUT] Pusher backward.");

                        m_iStep++;
                        return false;
                    }

                case 107:   //푸셔 백워드 확인, Z축 PlaceDn 위치 이동
                    {
                        if (!Func_PusherForward(false)) { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PlaceDn);
                        
                        _SetLog("[TRACK-OUT] Z axis move place down.", CData.SPos.dONL_Z_PlaceDn);

                        m_iStep++;
                        return false;
                    }

                case 108:   //Z축 PlaceDn 위치 이동, 벨트 2초 런, 푸셔 백워드 확인, Y축 Wait 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PlaceDn))   { return false; }
                        
                        if (CDataOption.OnlRfid == eOnlRFID.Use)
                        {
                            CData.Parts[ONL].sMGZ_ID = "";
                        }

                        Func_TopBeltRun(true);

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);

                        m_Delay.Set_Delay(1500);

                        _SetLog($"[TRACK-OUT] Top belt run. Y axis move wait({CData.Dev.dOnL_Y_Wait} mm).  Set delay : 1500ms");

                        m_iStep++;
                        return false;
                    }

                case 109:   //Top 매거진 감지 센서 감지 안될때까지 런
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait)) { return false; }

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) && !m_Delay.Chk_Delay())     //X9C Top Conveyor Mgz Detect
                        {
                            Func_TopBeltRun(true);
                            return false;
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        _SetLog("[TRACK-OUT] Top belt stop. Set Delay : 2000 ms");
                        m_Delay.Set_Delay(2000);

                        m_iStep++;
                        return false;
                    }

                case 110:   //벨트 2초간 런 후 정지
                    {
                        if (!m_Delay.Chk_Delay()) return false;

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) || !CIO.It.Get_X(eX.ONL_TopMGZDetectFull))    //X9D Top Conveyor Full Mgz
                        {
                            CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                            
                            _SetLog("Error : Top magazine full.");
                                
                            m_iStep = 0;
                            return true;
                        }

                        CData.Parts[ONL].bExistStrip = false;
                        CData.Parts[ONL].sLotName    = string.Empty;
                        CData.Parts[ONL].sMGZ_ID     = string.Empty;

                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].sLot_Name  = string.Empty;
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].sLD_MGZ_ID = string.Empty;

                        for (int i = 0; i < CData.Dev.iMgzCnt; i++)
                        {
                            CData.Parts[ONL].iSlot_info[i] = (int)eSlotInfo.Empty;
                        }

                        if (CData.IsMultiLOT())
                        {
                            // 더이상 Loading 진행 LOT이 없다면 초기화 시켜준다.
                            if (CData.LotMgr.LoadingLotName == "")
                            {
                                CData.Parts[ONL].iMGZ_No = 0;                               // 투입된 Magazine 번호 초기화
                            }
                        }

                        CData.bChkManual = false;
                        CData.LotMgr.IsLoaderTrackOutFlag = eTrackOutFlag.Valid;

                        CData.Parts[ONL].iStat = ESeq.ONL_Wait;
                        
                        _SetLog("[TRACK-OUT] Loaded MGZ Track-out Finish.  Status : " + CData.Parts[ONL].iStat);

                        CSQ_Man.It.bOnMGZPlaceDone = true;      // Set Up Strip Onloader MGZ Place Done

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog(string.Format("Finish (Track-Out : Abnormal Case).  Tack time : {0}", m_tTackTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }
                // 2022.04.26 SungTae End
            }
        }

        /// <summary>
        /// Strip Push Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Push()
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
                    CErr.Show(eErr.ONLOADER_PUSH_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            // 메거진 없을 시 에러 발생 이후 상태 픽으로 변경
            if (!Chk_ClampMgz())
            {
                CErr.Show(eErr.ONLOADER_CLAMP_DETECT_MGZ_READY);
                _SetLog("Error : Not detect magazine.");

                Func_PusherForward(false);
                CData.Parts[ONL].iStat = ESeq.ONL_Pick;
                _SetLog("Change status.  Status : " + CData.Parts[ONL].iStat);

                m_iStep = 0;
                return true;
            }

            bool bPushOver = Chk_PusherOverload();

            if (!bPushOver)
            {
                Func_PusherForward(false);
                CErr.Show(eErr.ONLOADER_PUSHER_OVERLOAD_ERROR);
                _SetLog("Error : Pusher overload.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//클램프 클로즈, 푸셔 BackWard, 인레일 Z그리퍼 오픈, 인레일 Y얼라인 BackWard, 인레일 리프트 다운, 인레일 Y축 얼라인 위치 이동, 인레일 프로브 업
                        m_tStTime = DateTime.Now;

                        if (!Func_PusherForward(false))
                        {
                            return false;
                        }

                        Func_MgzClamp(true);

                        CSq_Inr.It.Func_GripperClamp(false);    // In Rail Gripper Open
                        CSq_Inr.It.Act_YAlignBF(true);          // In Rail Align Forward LCY 200113
                        CSq_Inr.It.Act_ZLiftDU(false);          // In Rail Lift Down
                        
                        //20190221 ghk_dynamicfunction
                        //다이나믹 쓰던 안쓰던 해당 프로브는 업상태로 되야 합니다.
                        CSq_Inr.It.Act_ProbeUD(false);    //Inrail Probe Up
                        //if(!CData.Dev.bDynamicSkip) CSq_Inr.It.Act_ProbeUD(false); //190225 ksg :

                        //191109 ksg :
                        if (!CIO.It.Get_X(eX.INR_StripInDetect))
                        {                            
                            CErr.Show(eErr.INRAIL_INPUT_DETECTED_STRIP);
                            _SetLog("Error : Strip-in detect.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.INR_StripInDetect))
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                            {
                                CErr.Show(eErr.ONLOADER_Y_NOT_WAITPOSITION);
                                _SetLog("Error : Y axis not wait position.");

                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Pick);
                                _SetLog("INR X axis move pick.", CData.Dev.dInr_X_Pick);

                                m_iStep = 12;
                                return false;
                            }
                        }

                        m_dWorkPosZ = _Cal_WorkPos();

                        CMot.It.Mv_N(m_iZ, m_dWorkPosZ);
                        _SetLog("Z axis move work.", m_dWorkPosZ);

                        CMot.It.Mv_N(m_iInrY, CData.Dev.dInr_Y_Align); // Inr_Y_Align == Feed ?
                        _SetLog("INR Y axis move align.", CData.Dev.dInr_Y_Align);

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//클램프 다운 확인, 푸셔 백워드 확인, 인레일 그라퍼 업 확인, 인레일 Y얼라인 백워드 확인, 인레일 리프트 다운 확인, 인레일 Y축 언라인 위치 이동 확인, 인레일 X축 Pick 위치 이동
                        if (!Func_MgzClamp(true))                           { return false; }
                        if (!Func_PusherForward(false))                     { return false; }
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))   { return false; }
                        if (!CMot.It.Get_Mv(m_iZ, m_dWorkPosZ))             { return false; }
                        if (!CSq_Inr.It.Func_GripperClamp(false))           { return false; }

                        //210902 syc : 2004U
                        if (!CDataOption.Use2004U)
                        {
                            if (!CSq_Inr.It.Act_YAlignBF(true))// Foreward Check LCY 200113
                            { return false; }
                        }
                        if (!CSq_Inr.It.Act_ZLiftDU(false))
                        { return false; }

                        //20190221 ghk_dynamicfunction
                        //다이나믹 펑션 사용 하던 안하던 프로브 업상태로 되야 합니다.
                        if (!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)
                        {
                        }

                        if (!CMot.It.Get_Mv(m_iInrY, CData.Dev.dInr_Y_Align))   { return false; }

                        CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Pick);
                        _SetLog("INR X axis move pick.", CData.Dev.dInr_X_Pick);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//인레일 그리퍼 자재 유무 검사, 있을 경우 14번 이동, 없을 경우 인레일 X Pick 위치 이동 확인, 다이나믹 펑션 사용유무 확인

                        if (CData.GemForm != null) CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = "";// 20200910 BCR 값 초기화 LCY

                        if (CIO.It.Get_X(eX.INR_GripperDetect))    //X14 In Rail Gripper Detect  
                        {
                            CMot.It.Stop(m_iInrX);
                            m_Delay.Set_Delay(100);
                            _SetLog("INR X axis stop.  Gripper detect.  Set delay : 100ms");

                            m_iStep = 18;
                            return false;
                        }

                        if (CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_Pick))
                        {
                            if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                            {//다이나믹 펑션 사용 안함
                                Func_PusherForward(true);
                                _SetLog("Pusher forward.");

                                m_iStep = 17;
                                return false;
                            }
                            else
                            {//다이나믹 펑션 사용
                                CData.Dynamic.dPcbBase = 999.999;

                                //20190514 ghk_probe
                                //프로브 업상태일때 정수 값 저장
                                m_dInr_Prb = Math.Truncate(CPrb.It.Read_Val(EWay.INR));
                                _SetLog("Use DF.  Probe up value : " + m_dInr_Prb + "mm");

                                m_iStep++;
                                return false;
                            }
                        }
                        return false;
                    }

                case 13:
                    {//PCB 베이스 측정 1 => 프로브 에어 온, 프로브 다운
                        CIO.It.Set_Y(eY.INR_ProbeAir, true);
                        CSq_Inr.It.Act_ProbeUD(true);
                        
                        m_ProbeDelay.Set_Delay(GV.PRB_DELAY);
                        _SetLog("INR probe down.  Set delay : " + GV.PRB_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//PCB 베이스 측정 2 => 프로브 다운 확인, 프로브 값 읽기                     
                        CIO.It.Set_Y(eY.INR_ProbeAir, false);

                        if (!m_ProbeDelay.Chk_Delay()) { return false; }

                        CData.Dynamic.dPcbBase = CPrb.It.Read_Val(EWay.INR);

                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {//Qorvo 업체 프로브 업다운 센서 제거, 측정 값이 1보다 작으면 내려왔다고 간주 함.
                            if (GV.INR_PRB_RANGE < CData.Dynamic.dPcbBase)
                            {//다운 일때  1 값 보다 현재 값이 크면 리턴(다운 되지 않음)
                                return false;
                            }
                        }

                        _SetLog("PCB base : " + CData.Dynamic.dPcbBase + "mm");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//PCB 베이스 측정 3 => 프로브 업
                        CSq_Inr.It.Act_ProbeUD(false);
                        _SetLog("INR probe up.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//PCB 베이스 측정 4 => 프로브 업 확인, 푸셔 포워드
                        if ((m_dInr_Prb - 1) > Math.Truncate(CPrb.It.Read_Val(EWay.INR)))
                        {//현재 프로브 값이 이전 업상태 프로브 값 - 1 보다 작을 경우 다운 으로 간주 하여 리턴
                            return false;
                        }

                        if (CData.Dynamic.dPcbBase == 999.999)
                        {//pcb base 값 확인
                            CErr.Show(eErr.INRAIL_NOTHING_PCBBASE_VALUE);
                            _SetLog("Error : PCB base nothing.");

                            m_iStep = 0;
                            return true;
                        }
                        //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-) 초과 시 Error 발생
                        //DF Probe 셋업 시 DF Probe를 InRail Base로 DOWN 상태에서 Probe 값을 0으로 초기화해두어야 함.
                        if (CData.Dynamic.dBaseRange != 0)
                        {
                            if((CData.Dynamic.dPcbBase < -CData.Dynamic.dBaseRange) || (CData.Dynamic.dBaseRange < CData.Dynamic.dPcbBase))
                            {
                                CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_BASEHEIGHT_RANGEOVER);
                                _SetLog("PCB base rangeover.");

                                m_iStep = 0;
                                return true;;
                            }
                        }

                        Func_PusherForward(true);
                        _SetLog("Pusher forward.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//푸셔 오버로드 검사, 오버로드 일경우 푸셔 백워드 후 에러, 아닐 경우 푸셔 포워드 확인
                        if (!Func_PusherForward(true))  { return false; }
                        else                            { m_Delay.Set_Delay(500); }

                        //bool bPushOver = CIO.It.Get_X(eX.ONL_PusherOverLoad);//XA4 OnLoader Pusher OverLoad

                        if (!bPushOver)
                        {
                            Func_PusherForward(false);
                            CErr.Show(eErr.ONLOADER_PUSHER_OVERLOAD_ERROR);
                            _SetLog("Error : Pusher overload.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check overload.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//인레일 그리퍼 디텍트 센서, 스트립 인 디텍트 센서 확인 후 매거진이 비었을 경우 종료, 감지 안될경우 애러, 
                        if (!m_Delay.Chk_Delay())               { return false; }
                        if (CIO.It.Get_X(eX.ONL_LightCurtain))  { return false; }

                        //X010 = 스트립 인 감지 센서 (B접점)
                        //X014 = 그리퍼 감지 센서 (A접점)
                        //==============================================================================
                        //매거진 해당 슬롯 자재 비어 있을 경우, 인레일 스트립 인 센서 감지 안됨, 인레일 그리퍼 센서 감지 안됨
                        //==============================================================================
                        if (!CIO.It.Get_X(eX.INR_GripperDetect) && CIO.It.Get_X(eX.INR_StripInDetect))
                        {//스트립 인 자재 감지 안됨, 그리퍼 자재 감지 안됨
                            CData.Parts[ONL].iSlot_info[CData.Parts[ONL].iSlot_No] = (int)eSlotInfo.Empty;
                            
                            if (CData.Opt.bFirstTopSlot)            // Top slot -> Bottom slot으로 진행
                            {
                                if (CData.Parts[ONL].iSlot_No == 0)
                                {//매거진 슬롯 전부 Empty 되었을때 종료
                                    Func_PusherForward(false);

                                    CData.Parts[ONL].iStat = ESeq.ONL_Place;

                                    _SetLog("Top slot first1.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                    // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                                    // Multi-LOT, 투입된 Magazine이 지정 수량을 넘겼을 경우 해당 LOT은 종료된다.
                                    if (CData.IsMultiLOT())
                                    {
                                        CData.LotMgr.ClearLoadEmptySlot();          // Magazine을 Place해야하기때문에 연속 빈 Slot 감지 카운터 초기화

                                        // Magazine을 Place할때 지정된 Magazine수량을 모두 투입하였는지 Check한다.
                                        // 지정된 Magazine을 모두 투입하였다면
                                        if (CData.LotMgr.CheckMagazineInputQty() == true)
                                        {
                                            _SetLog($"Multi-LOT, CheckMagazineInputQty is true, LOT : {CData.LotMgr.LoadingLotName}");
                                            
											// 더 투입해야하는 Strip이 남아있는가 ?
                                            if (CData.LotMgr.GetStripInputRemain() > 0)
                                            {
                                                // 지정된 Mgz를 모두 투입하였지만 아직 투입해야하는 Strip이 남아있다면
                                                // 작업자에게 이대로 LOT을 종료할 것인지 POP-UP 창을 띄워 묻고 계속 진행 여부를 선택할때 까지 대기한다.

                                                // 이후로는 LOT 투입종료 조건을 변경시켜준다.
                                                CData.LotMgr.UserConfirmNextMode = ELotEndMode.eStripQty;  // Strip 수량만 체크하도록 한다.
                                                CData.LotMgr.UserConfirmMsg = "We have put in all the specified MGZ quantities, but we have not put in all the strips of the specified quantities yet.";
                                                //    \n Shall we continue with the input ?
                                                //(Continue: Continuous input / Stop: Input end of input)";    

                                                // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
                                            }
                                            else
                                            {
                                                // 지정한 Magazine 수량만큼 투입 하였다면 LOT을 종료 처리한다.

                                                // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                                if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                                {
                                                    string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                                                    _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                                    // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                                    CData.LotMgr.SetCloseLoadLOT();
                                                    CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                                                }
                                                else
                                                {
                                                    // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                                    _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                                }
                                            }
                                        }
                                        // Magazine 투입 수량으로 LOT을 종료하는 조건이 아니라면 그대로 진행
                                    }//of Multi-LOT -----

                                    m_iStep = 0;
                                    return true;
                                }
                                else
                                {//매거진 슬롯 전부 Empty 아닐 경우
                                    Func_PusherForward(false);

                                    CData.Parts[ONL].iSlot_No--;
                                    m_iStep = 0;

                                    // 2021-05-13, jhLee, Multi-LOT, LOT 종료를 빈 슬롯의 수량으로 체크한다.
                                    // 기존의 Empty Slot count 제한 값으로 LOT 종료를 인식하도록 한다.
                                    //

                                    // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                                    // Multi-LOT, 누적된 빈 Slot이 지정 수량을 넘겼을 경우 해당 LOT은 종료된다.
                                    if (CData.IsMultiLOT())
                                    {
                                        CData.LotMgr.SetLoadEmptySlot(true);            // 현재 Slot은 빈 슬롯임을 표시

                                        //if (CData.LotMgr.IsLotCloseableSlot)            // 연속 빈 Slot에 의한 LOT 종료 조건에 만족하는가 ?
                                        //{
                                        //    // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                        //    if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                        //    {
                                        //        _SetLog($"Multi-LOT, Top slot first2, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                        //        // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                        //        CData.LotMgr.SetCloseLoadLOT();
                                        //    }
                                        //    else
                                        //    {
                                        //        // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                        //        CData.LotMgr.ClearLoadEmptySlot();          // 연속 빈 Slot 감지 카운터 초기화
                                        //        _SetLog($"Multi-LOT, Top slot first2, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                        //    }

                                        //    CData.Parts[ONL].iStat = ESeq.ONL_Place;            // 집고있는 Magazine을 내려 놓는다.
                                        //}

                                        if (CData.LotMgr.IsLotCloseableSlot)            // 연속 빈 Slot에 의한 LOT 종료 조건에 만족하는가 ?
                                        {
                                            ELotEndMode nEndMode = CData.LotMgr.GetLotEndMode();        // 지정된 LOT 종료 모드

                                            if (nEndMode == ELotEndMode.eEmptySlot)
                                            {
                                                CData.Parts[ONL].iStat = ESeq.ONL_Place; // Mgz을 내려 놓는다.

                                                // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                                if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                                {
                                                    string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                                                    _SetLog($"Multi-LOT, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");

                                                    // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                                    CData.LotMgr.SetCloseLoadLOT();
                                                    CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                                                }
                                                else
                                                {
                                                    // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                                    CData.LotMgr.ClearLoadEmptySlot();          // 연속 빈 Slot 감지 카운터 초기화
                                                    _SetLog($"Multi-LOT, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                                }
                                            }
                                            else // 연속 빈 Slot에 의한 LOT 투입 종료가 아니라면 
                                            {
                                                // 이대로 투입을 지속할 것인지 LOT 투입을 종료할 것인지 묻는다.

                                                CData.LotMgr.UserConfirmNextMode = nEndMode;   // 기존 종료 조건 유지 
                                                CData.LotMgr.UserConfirmMsg = "A series of empty slots occurred for the specified quantity.";
                                                //    \n Shall we continue with the input ?
                                                //(Continue: Continuous input / Stop: LOT Input end)";    

                                                // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.

                                                CData.LotMgr.ClearLoadEmptySlot();          // 연속 빈 Slot 감지 카운터 초기화
                                                _SetLog($"Multi-LOT, Detect empty slot over limit, Confirm User select : {CData.LotMgr.LoadingLotName}");
                                            }
                                        }
                                    }
                                    //if (CDataOption.UseMultiLOT && CData.Opt.bMultiLOTUse)
                                    //{
                                    //    // Multi-LOT, 누적된 빈 Slot이 지정 수량을 넘겼을 경우 해당 LOT은 종료된다.
                                    //    CData.LotMgr.SetLoadEmptySlot(true);    // 현재 Slot은 빈 슬롯임을 표시
                                    //    if (CData.LotMgr.IsLotCloseableSlot)            // 연속 빈 Slot에 의한 LOT 종료 조건에 만족하는가 ?
                                    //    {
                                    //        // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                    //        CData.LotMgr.SetCloseLoadLOT();
                                    //        CData.Parts[ONL].iStat = ESeq.ONL_Place;            // 집고있는 Magazine을 내려 놓는다.
                                    //        _SetLog($"Top slot first2.  Strip-in X.  Gripper X.  Magazine slot empty. [LOT End]  iSlot_No : {CData.Parts[ONL].iSlot_No}");
                                    //    }
                                    //    // -----
                                    //}
                                    else // 기존 루틴
                                    { 
                                        //190103 ksg : 연속 빈슬롯 감지시 배출
                                        if (!CData.bEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0))
                                        {
                                            CData.bEmptySlotCnt = true;     // 빈슬롯의 수량을 누적시킨다.
                                        }

                                        if (CData.bEmptySlotCnt)
                                        {
                                            CData.iEmptySlotCnt++;          // 빈 슬롯 수 증가
                                        }

                                        if (CData.iEmptySlotCnt >= CData.Opt.iEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190116 ksg :  조건 추가
                                        {
                                            CData.Parts[ONL].iStat = ESeq.ONL_Place;            // 집고있는 Magazine을 내려 놓는다.
                                            _SetLog("Top slot first2.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                            m_iStep = 0;
                                        }
                                    }//of if Multi-LOT else

                                    return true;
                                }
                            }
                            else
                            {
                                // Bottom slot -> Top slot 으로 진행

                                if (CData.Parts[ONL].iSlot_No == (CData.Parts[ONL].iSlot_info.Length - 1))
                                {//매거진 슬롯 전부 Empty 되었을때 종료
                                    Func_PusherForward(false);
                                    
                                    CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                    
                                    _SetLog("Bottom slot first1.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                    // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                                    // Multi-LOT, 투입된 Magazine이 지정 수량을 넘겼을 경우 해당 LOT은 종료된다.
                                    if (CData.IsMultiLOT())
                                    {
                                        CData.LotMgr.ClearLoadEmptySlot();          // Magazine을 Place해야 하기 때문에 연속 빈 Slot 감지 카운터 초기화

                                        // Magazine을 Place할 때 지정된 Magazine 수량을 모두 투입하였는지 Check한다.
                                        // 지정된 Magazine을 모두 투입하였다면
                                        if (CData.LotMgr.CheckMagazineInputQty() == true)
                                        {
                                            // 더 투입해야하는 Strip이 남아있는가 ?
                                            if (CData.LotMgr.GetStripInputRemain() > 0)
                                            {
                                                // 지정된 Mgz를 모두 투입하였지만 아직 투입해야하는 Strip이 남아있다면
                                                // 작업자에게 이대로 LOT을 종료할 것인지 POP-UP 창을 띄워 묻고 계속 진행 여부를 선택할때 까지 대기한다.

                                                // 이후로는 LOT 투입종료 조건을 변경시켜준다.
                                                CData.LotMgr.UserConfirmNextMode = ELotEndMode.eStripQty;  // Strip 수량만 체크하도록 한다.
                                                CData.LotMgr.UserConfirmMsg = "We have put in all the specified MGZ quantities, but we have not put in all the strips of the specified quantities yet.";
                                                //    \n Shall we continue with the input ?
                                                //(Continue: Continuous input / Stop: Input end of input)";    

                                                // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
                                            }
                                            else
                                            {
                                                // 지정한 Magazine 수량만큼 투입 하였다면 LOT을 종료 처리한다.

                                                // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                                if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                                {
                                                    string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                                                    _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                                    // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                                    CData.LotMgr.SetCloseLoadLOT();
                                                    CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                                                }
                                                else
                                                {
                                                    // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                                    _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                                }
                                            }
                                        }
                                        // Magazine 투입 수량으로 LOT을 종료하는 조건이 아니라면 그대로 진행
                                    }//of Multi-LOT -----

                                    m_iStep = 0;
                                    return true;
                                }
                                else
                                {//매거진 슬롯 전부 Empty 아닐 경우
                                    Func_PusherForward(false);

                                    CData.Parts[ONL].iSlot_No++;
                                    m_iStep = 0;

                                    // 2021-05-13, jhLee, Multi-LOT, LOT 종료를 빈 슬롯의 수량으로 체크한다.
                                    // 기존의 Empty Slot count 제한 값으로 LOT 종료를 인식하도록 한다.
                                    //

                                    // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                                    // Multi-LOT, 누적된 빈 Slot이 지정 수량을 넘겼을 경우 해당 LOT은 종료된다.
                                    if (CData.IsMultiLOT())
                                    {
                                        CData.LotMgr.SetLoadEmptySlot(true);            // 현재 Slot은 빈 슬롯임을 표시

                                        if (CData.LotMgr.IsLotCloseableSlot)            // 연속 빈 Slot에 의한 LOT 종료 조건에 만족하는가 ?
                                        {
                                            ELotEndMode nEndMode = CData.LotMgr.GetLotEndMode();        // 지정된 LOT 종료 모드

                                            if (nEndMode == ELotEndMode.eEmptySlot)
                                            {
                                                // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                                if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                                {
                                                    string sLotName = CData.LotMgr.LoadingLotName;          // Loading 중인 LOT의 이름

                                                    _SetLog($"Multi-LOT, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");

                                                    // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                                    CData.LotMgr.SetCloseLoadLOT();
                                                    CData.LotMgr.CheckAtLotClose(sLotName);      // LOT-End 처리할 것인지 Check
                                                }
                                                else
                                                {
                                                    // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                                    CData.LotMgr.ClearLoadEmptySlot();          // 연속 빈 Slot 감지 카운터 초기화
                                                    _SetLog($"Multi-LOT, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                                }

                                                CData.Parts[ONL].iStat = ESeq.ONL_Place; // Mgz을 내려 놓는다.
                                            }
                                            else // 연속 빈 Slot에 의한 LOT 투입 종료가 아니라면 
                                            {
                                                // 이대로 투입을 지속할 것인지 LOT 투입을 종료할 것인지 묻는다.

                                                CData.LotMgr.UserConfirmNextMode = nEndMode;  // 이후에도 종료 조건 유지
                                                CData.LotMgr.UserConfirmMsg = "A series of empty slots occurred for the specified quantity.";
                                                //    \n Shall we continue with the input ?
                                                //(Continue: Continuous input / Stop: LOT Input end)";    

                                                // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.

                                                CData.LotMgr.ClearLoadEmptySlot();          // 연속 빈 Slot 감지 카운터 초기화
                                                _SetLog($"Multi-LOT, Detect empty slot over limit, Confirm User select : {CData.LotMgr.LoadingLotName}");
                                            }
                                        }
                                    } // -----
                                    else   // Multi-LOT 이전의 기존 루틴
                                    {
                                        //190103 ksg : 연속 빈슬롯 감지시 배출
                                        if (!CData.bEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190111 ksg : 조건 추가
                                        {
                                            CData.bEmptySlotCnt = true;
                                        }

                                        if (CData.bEmptySlotCnt)
                                        {
                                            CData.iEmptySlotCnt++;
                                        }

                                        if (CData.iEmptySlotCnt >= CData.Opt.iEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190116 ksg :  조건 추가
                                        {
                                            CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                            _SetLog("Bottom slot first2.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                            m_iStep = 0;
                                        }
                                    }//of if Multi-LOT else

                                    return true;
                                }
                            }
                        }

                        //==============================================================================
                        //매거진 해당 슬롯 자재 있으나 그리퍼 감지 안될 경우, 인레일 스트립 인 센서 감지, 인레일 그리퍼 센서 감지 안됨
                        //==============================================================================
                        if (!CIO.It.Get_X(eX.INR_GripperDetect) && !CIO.It.Get_X(eX.INR_StripInDetect))
                        {//에러(스트립 인 자재 감지 됨, 그리퍼 자재 감지 안됨)
                            Func_PusherForward(false);
                            CErr.Show(eErr.ONLOADER_PUSHERFAIL);
                            _SetLog("Error : Push fail.  Strip-in O.  Gripper X.");

                            m_iStep = 0;
                            return true;
                        }

                        //==============================================================================
                        //매거진 해당 슬롯 자재 정상적으로 감지 되었을 때, 스트립 인 자재 감지, 그리퍼 자재 감지 됬을 경우
                        //==============================================================================
                        if (CIO.It.Get_X(eX.INR_GripperDetect) && !CIO.It.Get_X(eX.INR_StripInDetect))
                        {
                            CSq_Inr.It.Func_GripperClamp(true);
                            
                            //190103 ksg : 연속 빈슬롯 감지시 배출
                            CData.iEmptySlotCnt = 0;
                            CData.bEmptySlotCnt = false;
                            
                            _SetLog("Strip-in O.  Gripper O.  INR gripper clamp.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 19:
                    {//인레일 그리퍼 클로즈 확인, 인레일 X축 바코드 위치 이동, 푸셔 백워드
                        if (!CSq_Inr.It.Func_GripperClamp(true))    { return false; }

                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                        {
                            if (CDataOption.CurEqu == eEquType.Nomal)
                            {
                                CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Bcr);
                                _SetLog("INR X axis move bcr.", CData.Dev.dInr_X_Bcr);
                            }
                            else
                            {
                                CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Align);
                                _SetLog("INR X axis move align.", CData.Dev.dInr_X_Align);
                            }
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_DynamicPos1);
                            _SetLog("INR X axis move DF1.", CData.Dev.dInr_X_DynamicPos1);
                        }

                        Func_PusherForward(false);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//인레일 X축 바코드 위치 이동 확인, 푸셔 백워드 확인, 인레일 그리퍼 센서 감지 확인,
                        //20191104 ghk_dfserver_notuse_df
                        //if (CData.Dev.bDynamicSkip)
                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                        {
                            if (CDataOption.CurEqu == eEquType.Nomal)
                            {
                                if (!CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_Bcr))     { return false; }
                            }
                            else
                            {
                                if (!CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_Align))   { return false; }
                            }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_DynamicPos1)) { return false; }
                        }

                        if (!Func_PusherForward(false)) { return false; }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            Func_PusherForward(false);
                            CErr.Show(eErr.INRAIL_FEEDING_ERROR);
                            _SetLog("Error : INR gripper feeding fail.");

                            m_iStep = 0;
                            return true;
                        }

                        int iNum = CData.Parts[ONL].iSlot_No;      // 현재 슬롯의 인덱스
                        CData.Parts[(int)EPart.INR].iMGZ_No  = CData.Parts[ONL].iMGZ_No;  // 인레일 매거진 번호에 데이터 전달
                        CData.Parts[(int)EPart.INR].iSlot_No = iNum;  // 인레일 슬롯 번호에 현재 자재 번호 전달
                        CData.Parts[(int)EPart.INR].bChkGrd  = false; //200624 pjh : Grinding 중 Error Check 변수

                        //201014 jhc : InRail 투입된 자재 PCB 두께값 초기화
                        CData.Parts[(int)EPart.INR].dDfMin = 0.0;
                        CData.Parts[(int)EPart.INR].dDfMax = 0.0;
                        CData.Parts[(int)EPart.INR].dDfAvg = 0.0;

                        CData.Parts[(int)EPart.INR].dPcbMin  = 0.0;
                        CData.Parts[(int)EPart.INR].dPcbMax  = 0.0;
                        CData.Parts[(int)EPart.INR].dPcbMean = 0.0;

						if (CDataOption.UseNewSckGrindProc)
						{
							CData.Parts[(int)EPart.INR].dTopMoldMax = 0.0;
							CData.Parts[(int)EPart.INR].dTopMoldAvg = 0.0;
							CData.Parts[(int)EPart.INR].dBtmMoldMax = 0.0;
							CData.Parts[(int)EPart.INR].dBtmMoldAvg = 0.0;
						}

                        if (CData.Opt.bSecsUse && CData.GemForm != null)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0] = 0.0;  // [3=DF Measure 후].[0=DF]
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0] = 0.0;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0] = 0.0;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].nDF_User_Mode        = CData.Dynamic.iHeightType;

                            //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0] = 0.0;  // [4=Strip ID 읽음].[0=DF]
                            //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0] = 0.0;
                           // CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0] = 0.0;

                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].dMeasure_TopMold_Max = 0.0;
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].dMeasure_TopMold_Avg = 0.0;
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].dMeasure_BtmMold_Max = 0.0;
                                CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].dMeasure_BtmMold_Avg = 0.0;

                                //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max = 0.0;
                                //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg = 0.0;
                                //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max = 0.0;
                                //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg = 0.0;
                            }
                        }

                        if ((!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)   ||
                            (!CData.Dev.bDynamicSkip && CData.Dev.eMoldSide   == ESide.Top)                 ||
                            (!CData.Dev.bDynamicSkip && CData.Dev.eMoldSide   == ESide.BtmS) )              // 2021.08.03 lhs : BtmS 추가
                        {//다이나믹 펑션 사용, 
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckDynamic;
                        }
                        else
                        {//다이나믹 펑션 스킵
                            // 201111 jym START : Skyworks Ori -> Bcr 순으로 변경
                            if (CData.CurCompany == ECompany.SkyWorks)
                            {
                                if (!CData.Dev.bOriSkip)
                                {
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                                }
                                else
                                {
                                    if (!CData.Dev.bBcrSkip)    { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr;    }
                                    else                        { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;       }
                                }                                
                            } // 201111 jym END
                            else
                            {
                                if (!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip)
                                {//바코드 사용
                                 //20190604 ghk_onpbcr
                                    if (CDataOption.CurEqu == eEquType.Nomal)   { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr; }
                                    else                                        { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; }//190821 ksg :
                                }
                                else
                                {//바코드 스킵
                                    if (!CData.Dev.bOriSkip)
                                    {//Ori 사용
                                     //20190604 ghk_onpbcr
                                        if (CDataOption.CurEqu == eEquType.Nomal)   { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri; }
                                        else                                        { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; }//190821 ksg :
                                    }
                                    else
                                    {//Ori 스킵
                                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                                    }
                                }
                            }
                        }

                        CData.Parts[ONL].iSlot_info[iNum] = (int)eSlotInfo.Empty;
                        CData.Parts[(int)EPart.INR].bExistStrip = true;  //Ksg InRail에 자재 있음으로 변경

                        //190503 ksg :
                        if (CData.Opt.bFirstTopSlot)
                        {
                            if (iNum == 0)
                            {//매거진 슬롯 전부 Empty 되었을때 종료
                                Func_PusherForward(false);
                                
                                CData.Parts[ONL].iStat = ESeq.ONL_Place;                                // 들고있는 MGZ를 내려놓는다.
                                CData.Parts[(int)EPart.INR].sLotName = CData.Parts[ONL].sLotName;       // 현재 투입된 Strip의 LOT 이름

                                // 투입 총량 누적
                                CData.LotInfo.iTInCnt++; //190407 ksg :

                                // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                                // 2021-05-13, jhLee, Mulit-LOT, 투입수량 증가
                                if (CData.IsMultiLOT())
                                {
                                    CData.LotMgr.SetLoadEmptySlot(false);           // 정상적인 제품이 투입되었다.

                                    CData.Parts[(int)EPart.INR].LotColor    = CData.Parts[ONL].LotColor;    // LOT 색
                                    CData.Parts[(int)EPart.INR].nLoadMgzSN  = CData.Parts[ONL].nLoadMgzSN;  // Magazine SN
                                    
                                    // 2022.05.04 SungTae Start : [추가]
                                    if (CData.CurCompany == ECompany.ASE_K12)
                                        CData.Parts[(int)EPart.INR].sMGZ_ID = CData.Parts[(int)EPart.ONL].sMGZ_ID;
                                    // 2022.05.04 SungTae End

                                    // 2022.05.04 SungTae : [확인용]
                                    _SetLog($"LoadMgzSN : ONL({CData.Parts[(int)EPart.ONL].nLoadMgzSN}) -> INR({CData.Parts[(int)EPart.INR].nLoadMgzSN})");

                                    // 지정된 Strip을 모두 투입하였다면 지정된 LOT의 투입을 종료시켜준다.
                                    if (CData.LotMgr.CheckStripInputQty() == true)
                                    {
                                        _SetLog($"Multi-LOT, All strip input, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                        // 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
                                        CData.LotMgr.SetCloseLoadLOT();
                                    }
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.INR].LotColor = System.Drawing.Color.Lime;       // 기본 색
                                }

                                // Magazine을 Place할때 지정된 Magazine수량을 모두 투입하였는지 Check한다.
                                // 지정된 Magazine을 모두 투입하였다면
                                if (CData.LotMgr.CheckMagazineInputQty() == true)
                                {
                                    // 더 투입해야하는 Strip이 남아있는가 ?
                                    if (CData.LotMgr.GetStripInputRemain() > 0)
                                    {
                                        // 지정된 Mgz를 모두 투입하였지만 아직 투입해야하는 Strip이 남아있다면
                                        // 작업자에게 이대로 LOT을 종료할 것인지 POP-UP 창을 띄워 묻고 계속 진행 여부를 선택할때 까지 대기한다.

                                        // 이후로는 LOT 투입종료 조건을 변경시켜준다.
                                        CData.LotMgr.UserConfirmNextMode = ELotEndMode.eStripQty;  // Strip 수량만 체크하도록 한다.
                                        CData.LotMgr.UserConfirmMsg = "We have put in all the specified MGZ quantities, but we have not put in all the strips of the specified quantities yet.";
                                        //    \n Shall we continue with the input ?
                                        //(Continue: Continuous input / Stop: Input end of input)";    

                                        // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                        CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
                                    }
                                    else
                                    {
                                        // 지정한 Magazine 수량만큼 투입 하였다면 LOT을 종료 처리한다.

                                        // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                        if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                        {
                                            _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                            // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                            CData.LotMgr.SetCloseLoadLOT();
                                        }
                                        else
                                        {
                                            // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                            _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                        }
                                    }
                                }
                                // Magazine 투입 수량으로 LOT을 종료하는 조건이 아니라면 그대로 진행

                                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                                {
                                    CIO.It.Set_Y(eY.IOZL_Power, true); //200121 ksg : 200625 lks
                                }

                                _SetLog("Top slot first.  Slot all empty.  Status : " + CData.Parts[ONL].iStat);
                                
                                m_iStep = 0;
                                return true;
                            }
                        }
                        else   //if (CData.Opt.bFirstTopSlot)의 else
                        {
                            if (iNum == (CData.Dev.iMgzCnt - 1))
                            {//매거진 슬롯 전부 Empty 되었을때 종료
                                Func_PusherForward(false);

                                CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                CData.Parts[(int)EPart.INR].sLotName = CData.Parts[ONL].sLotName;       // 현재 투입된 Strip의 LOT 이름

                                // 투입 총량 누적
                                CData.LotInfo.iTInCnt++; //190407 ksg :

                                // 2021.07.08 SungTae Start : [추가] 연속 LOT 기능 추가
                                // 2021-05-13, jhLee, Mulit-LOT, 투입수량 증가
                                if (CData.IsMultiLOT())
                                {
                                    CData.LotMgr.SetLoadEmptySlot(false);           // 정상적인 제품이 투입되었다.

                                    CData.Parts[(int)EPart.INR].LotColor    = CData.Parts[ONL].LotColor;       // LOT 색
                                    CData.Parts[(int)EPart.INR].nLoadMgzSN  = CData.Parts[ONL].nLoadMgzSN;

                                    // 지정된 Strip을 모두 투입하였다면 지정된 LOT의 투입을 종료시켜준다.
                                    if (CData.LotMgr.CheckStripInputQty() == true)
                                    {
                                        _SetLog($"Multi-LOT, All strip input, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                        // 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
                                        CData.LotMgr.SetCloseLoadLOT();
                                    }
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.INR].LotColor = System.Drawing.Color.Lime;       // 기본 색
                                }

                                // Magazine을 Place할때 지정된 Magazine수량을 모두 투입하였는지 Check한다.
                                // 지정된 Magazine을 모두 투입하였다면
                                if (CData.LotMgr.CheckMagazineInputQty() == true)
                                {
                                    // 더 투입해야하는 Strip이 남아있는가 ?
                                    if (CData.LotMgr.GetStripInputRemain() > 0)
                                    {
                                        // 지정된 Mgz를 모두 투입하였지만 아직 투입해야하는 Strip이 남아있다면
                                        // 작업자에게 이대로 LOT을 종료할 것인지 POP-UP 창을 띄워 묻고 계속 진행 여부를 선택할때 까지 대기한다.

                                        // 이후로는 LOT 투입종료 조건을 변경시켜준다.
                                        CData.LotMgr.UserConfirmNextMode = ELotEndMode.eStripQty;  // Strip 수량만 체크하도록 한다.
                                        CData.LotMgr.UserConfirmMsg = "We have put in all the specified MGZ quantities, but we have not put in all the strips of the specified quantities yet.";
                                        //    \n Shall we continue with the input ?
                                        //(Continue: Continuous input / Stop: Input end of input)";    

                                        // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
                                        CData.LotMgr.UserConfirmReply = ELotUserConfirm.eQuestion;      // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
                                    }
                                    else
                                    {
                                        // 지정한 Magazine 수량만큼 투입 하였다면 LOT을 종료 처리한다.

                                        // 현재 Loading 작업중인 LOT 소속으로 투입된 Strip이 존재한다면
                                        if (CData.LotMgr.GetInputCountLoadLot(CData.Parts[ONL].sLotName) > 0)
                                        {
                                            _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                            // 현재 처리중인 LOT의 투입을 종료 한다.      RUN -> CLOSE 상태로 변경
                                            CData.LotMgr.SetCloseLoadLOT();
                                        }
                                        else
                                        {
                                            // 해당 LOT으로 투입된 Strip이 존재하지 않는다면 비어있는 Mgz을 투입시켰다.
                                            _SetLog($"Multi-LOT, Top slot first1, Detect empty slot over limit, But continue Loading LOT : {CData.LotMgr.LoadingLotName}");
                                        }
                                    }
                                }
                                // Magazine 투입 수량으로 LOT을 종료하는 조건이 아니라면 그대로 진행

                                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                                {
                                    CIO.It.Set_Y(eY.IOZL_Power, true); //200121 ksg : 200625 lks
                                }

                                _SetLog("Bottom slot first.  Slot all empty.  Status : " + CData.Parts[ONL].iStat);
                                
                                m_iStep = 0;
                                return true;
                            }
                        }

                        //CData.Parts[ONL].iSlot_No++;
                        //190503 ksg :
                        if (CData.Opt.bFirstTopSlot)    CData.Parts[ONL].iSlot_No--;
                        else                            CData.Parts[ONL].iSlot_No++;

                        // 투입 총량 누적
                        CData.LotInfo.iTInCnt++; //190407 ksg :

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        { 
                            CIO.It.Set_Y(eY.IOZL_Power, true);  // SCK, JSCK 
                        }

                        CData.Parts[(int)EPart.INR].sLotName = CData.Parts[ONL].sLotName;       // 현재 투입된 Strip의 LOT 이름

                        // 2021-05-13, jhLee, Mulit-LOT, 투입수량 증가
                        if (CData.IsMultiLOT())
                        {
                            CData.LotMgr.SetLoadEmptySlot(false);           // 정상적인 제품이 투입되었다.

                            CData.Parts[(int)EPart.INR].LotColor    = CData.Parts[ONL].LotColor;       // LOT 색
                            CData.Parts[(int)EPart.INR].nLoadMgzSN  = CData.Parts[ONL].nLoadMgzSN;

                            // 지정된 Strip을 모두 투입하였다면 지정된 LOT의 투입을 종료시켜준다.
                            if (CData.LotMgr.CheckStripInputQty() == true)
                            {
                                Func_PusherForward(false);
                                CData.Parts[ONL].iStat = ESeq.ONL_Place;            // LOT 투입이 종료 되므로 Magazine을 내려놓도록 한다.

                                _SetLog($"Multi-LOT, All strip input, Close Loading LOT : {CData.LotMgr.LoadingLotName}");
                                // 현재 처리중인 LOT의 투입을 종료 한다.    LOT 상태를  RUN -> CLOSE 상태로 변경
                                CData.LotMgr.SetCloseLoadLOT();
                            }
                        }
                        else
                        {
                            CData.Parts[(int)EPart.INR].LotColor = System.Drawing.Color.Lime;       // 기본 색
                        }


                        _SetLog("Lot in count : " + CData.LotInfo.iTInCnt + "  INR Status : " + CData.Parts[(int)EPart.INR].iStat);

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Tack time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));                 

                        m_iStep = 0;
                        return true;
                    }
            }
        }


        //
        // 2021-05-18, 오성태 책임, SPIL-FJ의 SG2003U 개조 대응
        //
        /// <summary>
        /// Package Unit Push Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PushU()
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
                    CErr.Show(eErr.ONLOADER_PUSH_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            // 메거진 없을 시 에러 발생 이후 상태 픽으로 변경
            if (!Chk_ClampMgz())
            {
                CErr.Show(eErr.ONLOADER_CLAMP_DETECT_MGZ_READY);
                _SetLog("Error : Not detect magazine.");

                Func_PusherForward(false);

                CData.Parts[ONL].iStat = ESeq.ONL_Pick;
                _SetLog("Change status.  Status : " + CData.Parts[ONL].iStat);

                m_iStep = 0;
                return true;
            }

            bool bPushOver = Chk_PusherOverload();

            if (!bPushOver)
            {
                Func_PusherForward(false);

                CErr.Show(eErr.ONLOADER_PUSHER_OVERLOAD_ERROR);
                _SetLog("Error : Pusher overload.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//클램프 클로즈, 푸셔 BackWard, 인레일 Z그리퍼 오픈, 인레일 Y얼라인 BackWard, 인레일 리프트 다운, 인레일 Y축 얼라인 위치 이동, 인레일 프로브 업
                        m_tStTime = DateTime.Now;

                        if (!Func_PusherForward(false))
                        {
                            return false;
                        }

                        Func_MgzClamp(true);

                        CSq_Inr.It.Func_GripperClamp(false);  // In Rail Gripper Open
                        CSq_Inr.It.Act_YAlignBF(true);     // In Rail Align Forward LCY 200113
                        CSq_Inr.It.Act_ZLiftDU(false);  // In Rail Lift Down
                                                        //20190221 ghk_dynamicfunction
                                                        //다이나믹 쓰던 안쓰던 해당 프로브는 업상태로 되야 합니다.
                        CSq_Inr.It.Act_ProbeUD(false);    //Inrail Probe Up
                        //if(!CData.Dev.bDynamicSkip) CSq_Inr.It.Act_ProbeUD(false); //190225 ksg :

                        //191109 ksg :
                        if (!CIO.It.Get_X(eX.INR_StripInDetect))
                        {
                            CErr.Show(eErr.INRAIL_INPUT_DETECTED_STRIP);
                            _SetLog("Error : Strip-in detect.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.INR_StripInDetect))
                        {
                            if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                            {
                                CErr.Show(eErr.ONLOADER_Y_NOT_WAITPOSITION);
                                _SetLog("Error : Y axis not wait position.");

                                m_iStep = 0;
                                return true;
                            }
                            else
                            {
                                CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Pick);
                                _SetLog("INR X axis move pick.", CData.Dev.dInr_X_Pick);

                                m_iStep = 12;
                                return false;
                            }
                        }

                        m_dWorkPosZ = _Cal_WorkPos();
                        
                        CMot.It.Mv_N(m_iZ, m_dWorkPosZ);
                        _SetLog("Z axis move work.", m_dWorkPosZ);
                        
                        CMot.It.Mv_N(m_iInrY, CData.Dev.dInr_Y_Align); // Inr_Y_Align == Feed ?
                        _SetLog("INR Y axis move align.", CData.Dev.dInr_Y_Align);
                        
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//클램프 다운 확인, 푸셔 백워드 확인, 인레일 그라퍼 업 확인, 인레일 Y얼라인 백워드 확인, 인레일 리프트 다운 확인, 인레일 Y축 언라인 위치 이동 확인, 인레일 X축 Pick 위치 이동
                        if (!Func_MgzClamp(true) || !Func_PusherForward(false))
                        {
                            return false;
                        }

                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait) || !CMot.It.Get_Mv(m_iZ, m_dWorkPosZ))
                        {
                            return false;
                        }
                        //210902 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            if (!CSq_Inr.It.Func_GripperClamp(false) || !CSq_Inr.It.Act_ZLiftDU(false)) return false;
                        }
                        else
                        {
                            if (!CSq_Inr.It.Func_GripperClamp(false) || !CSq_Inr.It.Act_YAlignBF(true) || !CSq_Inr.It.Act_ZLiftDU(false))  return false;
                            
                        }
                     

                        //20190221 ghk_dynamicfunction
                        //다이나믹 펑션 사용 하던 안하던 프로브 업상태로 되야 합니다.
                        if (!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)
                        {
                        }

                        if (!CMot.It.Get_Mv(m_iInrY, CData.Dev.dInr_Y_Align))   { return false; }

                        CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Pick);
                        _SetLog("INR X axis move pick.", CData.Dev.dInr_X_Pick);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//인레일 그리퍼 자재 유무 검사, 있을 경우 14번 이동, 없을 경우 인레일 X Pick 위치 이동 확인, 다이나믹 펑션 사용유무 확인
                        if (CData.GemForm != null) CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = "";

                        if (CIO.It.Get_X(eX.INR_GripperDetect))    //X14 In Rail Gripper Detect  
                        {
                            CMot.It.Stop(m_iInrX);

                            m_Delay.Set_Delay(100);
                            _SetLog("INR X axis stop.  Gripper detect.  Set delay : 100ms");

                            m_iStep = 18;
                            return false;
                        }

                        if (CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_Pick))
                        {
                            if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                            {//다이나믹 펑션 사용 안함
                                Func_PusherForward(true);
                                _SetLog("Pusher forward.");

                                m_iStep = 17;
                                return false;
                            }
                            else
                            {//다이나믹 펑션 사용
                                CData.Dynamic.dPcbBase = 999.999;

                                //20190514 ghk_probe
                                //프로브 업상태일때 정수 값 저장
                                m_dInr_Prb = Math.Truncate(CPrb.It.Read_Val(EWay.INR));
                                _SetLog("Use DF.  Probe up value : " + m_dInr_Prb + "mm");

                                m_iStep++;
                                return false;
                            }
                        }

                        return false;
                    }

                case 13:
                    {//PCB 베이스 측정 1 => 프로브 에어 온, 프로브 다운
                        CIO.It.Set_Y(eY.INR_ProbeAir, true);
                        CSq_Inr.It.Act_ProbeUD(true);

                        m_ProbeDelay.Set_Delay(GV.PRB_DELAY);
                        _SetLog("INR probe down.  Set delay : " + GV.PRB_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//PCB 베이스 측정 2 => 프로브 다운 확인, 프로브 값 읽기
                        CIO.It.Set_Y(eY.INR_ProbeAir, false);

                        if (!m_ProbeDelay.Chk_Delay()) { return false; }

                        CData.Dynamic.dPcbBase = CPrb.It.Read_Val(EWay.INR);

                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                        {//Qorvo 업체 프로브 업다운 센서 제거, 측정 값이 1보다 작으면 내려왔다고 간주 함.
                            if (GV.INR_PRB_RANGE < CData.Dynamic.dPcbBase)
                            {//다운 일때  1 값 보다 현재 값이 크면 리턴(다운 되지 않음)
                                return false;
                            }
                        }

                        _SetLog("PCB base : " + CData.Dynamic.dPcbBase + "mm");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//PCB 베이스 측정 3 => 프로브 업
                        CSq_Inr.It.Act_ProbeUD(false);
                        _SetLog("INR probe up.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//PCB 베이스 측정 4 => 프로브 업 확인, 푸셔 포워드
                        if ((m_dInr_Prb - 1) > Math.Truncate(CPrb.It.Read_Val(EWay.INR)))
                        {//현재 프로브 값이 이전 업상태 프로브 값 - 1 보다 작을 경우 다운 으로 간주 하여 리턴
                            return false;
                        }

                        if (CData.Dynamic.dPcbBase == 999.999)
                        {//pcb base 값 확인
                            CErr.Show(eErr.INRAIL_NOTHING_PCBBASE_VALUE);
                            _SetLog("Error : PCB base nothing.");

                            m_iStep = 0;
                            return true;
                        }

                        //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-) 초과 시 Error 발생
                        //DF Probe 셋업 시 DF Probe를 InRail Base로 DOWN 상태에서 Probe 값을 0으로 초기화해두어야 함.
                        if (CData.Dynamic.dBaseRange != 0)
                        {
                            if ((CData.Dynamic.dPcbBase < -CData.Dynamic.dBaseRange) || (CData.Dynamic.dBaseRange < CData.Dynamic.dPcbBase))
                            {
                                CErr.Show(eErr.INRAIL_DYNAMICFUNCTION_BASEHEIGHT_RANGEOVER);
                                _SetLog("PCB base rangeover.");

                                m_iStep = 0;
                                return true; ;
                            }
                        }

                        Func_PusherForward(true);
                        _SetLog("Pusher forward.");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//푸셔 오버로드 검사, 오버로드 일경우 푸셔 백워드 후 에러, 아닐 경우 푸셔 포워드 확인
                        if (!Func_PusherForward(true))  { return false; }
                        else                            { m_Delay.Set_Delay(500); }

                        //bool bPushOver = CIO.It.Get_X(eX.ONL_PusherOverLoad);//XA4 OnLoader Pusher OverLoad

                        if (!bPushOver)
                        {
                            Func_PusherForward(false);
                            CErr.Show(eErr.ONLOADER_PUSHER_OVERLOAD_ERROR);
                            _SetLog("Error : Pusher overload.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check overload.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//인레일 그리퍼 디텍트 센서, 스트립 인 디텍트 센서 확인 후 매거진이 비었을 경우 종료, 감지 안될경우 애러, 
                        if (!m_Delay.Chk_Delay())               { return false; }
                        if (CIO.It.Get_X(eX.ONL_LightCurtain))  { return false; }

                        //X010 = 스트립 인 감지 센서 (B접점)
                        //X014 = 그리퍼 감지 센서 (A접점)
                        //==============================================================================
                        //매거진 해당 슬롯 자재 비어 있을 경우, 인레일 스트립 인 센서 감지 안됨, 인레일 그리퍼 센서 감지 안됨
                        //==============================================================================
                        if (!CIO.It.Get_X(eX.INR_GripperDetect) && CIO.It.Get_X(eX.INR_StripInDetect))
                        {//스트립 인 자재 감지 안됨, 그리퍼 자재 감지 안됨
                            CData.Parts[ONL].iSlot_info[CData.Parts[ONL].iSlot_No] = (int)eSlotInfo.Empty;

                            if (CData.Opt.bFirstTopSlot)
                            {
                                if (CData.Parts[ONL].iSlot_No == 0)
                                {//매거진 슬롯 전부 Empty 되었을때 종료
                                    Func_PusherForward(false);
                                    CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                    _SetLog("Top slot first1.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                    m_iStep = 0;
                                    return true;
                                }
                                else
                                {//매거진 슬롯 전부 Empty 아닐 경우
                                    Func_PusherForward(false);

                                    CData.Parts[ONL].iSlot_No--;
                                    m_iStep = 0;

                                    //190103 ksg : 연속 빈슬롯 감지시 배출
                                    if (!CData.bEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190111 ksg : 조건 추가
                                    {
                                        CData.bEmptySlotCnt = true;
                                    }

                                    if (CData.bEmptySlotCnt)
                                    {
                                        CData.iEmptySlotCnt++;
                                    }

                                    if (CData.iEmptySlotCnt >= CData.Opt.iEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190116 ksg :  조건 추가
                                    {
                                        CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                        _SetLog("Top slot first2.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                        m_iStep = 0;
                                    }

                                    return true;
                                }
                            }
                            else
                            {
                                if (CData.Parts[ONL].iSlot_No == (CData.Parts[ONL].iSlot_info.Length - 1))
                                {//매거진 슬롯 전부 Empty 되었을때 종료
                                    Func_PusherForward(false);

                                    CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                    _SetLog("Bottom slot first1.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                    m_iStep = 0;
                                    return true;
                                }
                                else
                                {//매거진 슬롯 전부 Empty 아닐 경우
                                    Func_PusherForward(false);

                                    CData.Parts[ONL].iSlot_No++;
                                    m_iStep = 0;

                                    //190103 ksg : 연속 빈슬롯 감지시 배출
                                    if (!CData.bEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190111 ksg : 조건 추가
                                    {
                                        CData.bEmptySlotCnt = true;
                                    }

                                    if (CData.bEmptySlotCnt)
                                    {
                                        CData.iEmptySlotCnt++;
                                    }

                                    if (CData.iEmptySlotCnt >= CData.Opt.iEmptySlotCnt && (CData.Opt.iEmptySlotCnt > 0)) //190116 ksg :  조건 추가
                                    {
                                        CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                        _SetLog("Bottom slot first2.  Strip-in X.  Gripper X.  Magazine slot empty.  Status : " + CData.Parts[ONL].iStat);

                                        m_iStep = 0;
                                    }

                                    return true;
                                }
                            }
                        }

                        //==============================================================================
                        //매거진 해당 슬롯 자재 있으나 그리퍼 감지 안될 경우, 인레일 스트립 인 센서 감지, 인레일 그리퍼 센서 감지 안됨
                        //==============================================================================
                        if (!CIO.It.Get_X(eX.INR_GripperDetect) && !CIO.It.Get_X(eX.INR_StripInDetect))
                        {//에러(스트립 인 자재 감지 됨, 그리퍼 자재 감지 안됨)
                            Func_PusherForward(false);

                            CErr.Show(eErr.ONLOADER_PUSHERFAIL);
                            _SetLog("Error : Push fail.  Strip-in O.  Gripper X.");

                            m_iStep = 0;
                            return true;
                        }

                        //==============================================================================
                        //매거진 해당 슬롯 자재 정상적으로 감지 되었을 때, 스트립 인 자재 감지, 그리퍼 자재 감지 됬을 경우
                        //==============================================================================
                        if (CIO.It.Get_X(eX.INR_GripperDetect) && !CIO.It.Get_X(eX.INR_StripInDetect))
                        {
                            CSq_Inr.It.Func_GripperClamp(true);

                            //190103 ksg : 연속 빈슬롯 감지시 배출
                            CData.iEmptySlotCnt = 0;
                            CData.bEmptySlotCnt = false;
                            _SetLog("Strip-in O.  Gripper O.  INR gripper clamp.");

                            m_iStep++;
                        }

                        return false;
                    }

                case 19:
                    {// Gripper Clamp 상태 체크, Unit #1 Check Position 이동
                        if (!CSq_Inr.It.Func_GripperClamp(true)) { return false; }

                        double dPos = CData.SPos.dINR_X_ChkUnit - (80.0D * m_iUnitCnt);

                        if (CMot.It.Mv_N(m_iInrX, dPos) != 0) { return false; }

                        _SetLog("INR gripper clamp OK. INR X-axis move unit check position. [" + dPos + " mm]");

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {// Unit #1 Check Sensor로 Unit 유무 확인
                        double dPos = CData.SPos.dINR_X_ChkUnit - (80.0D * m_iUnitCnt);
                        
                        if (!CMot.It.Get_Mv(m_iInrX, dPos)) { return false; }

                        _SetLog("INR X-axis unit check position move completed.");

                        if (m_iUnitCnt < 4)
                        {
                            bool bCheck = CIO.It.Get_X(eX.INR_Unit_1_Detect);

                            CData.Parts[(int)EAx.Inrail_X].aUnitEx[m_iUnitCnt] = bCheck;

                            if (bCheck)
                                _SetLog("Unit-#" + (m_iUnitCnt + 1) + "detected.");
                            else
                                _SetLog("Unit-#" + (m_iUnitCnt + 1) + "not detected.");

                            m_iUnitCnt++;

                            m_iStep = 19;
                        }
                        else
                        {
                            m_iUnitCnt = 0;
                            m_iStep++;
                        }
                        
                        return false;
                    }

                case 21:
                    {// Unit 유무 확인
                        if (CData.Parts[m_iInrX].aUnitEx[0] && CData.Parts[m_iInrX].aUnitEx[1] &&
                            CData.Parts[m_iInrX].aUnitEx[2] && CData.Parts[m_iInrX].aUnitEx[3])
                        {
                            m_iStep++;
                        }
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (!CData.Parts[m_iInrX].aUnitEx[i])
                                {
                                    _SetLog("Error : Unit-#" + (i + 1) + " not exist!!");
                                }
                            }

                            CErr.Show(eErr.INRAIL_CHECK_UNIT_NOT_EXIST_ERROR);

                            m_iStep = 0;
                        }

                        return false;
                    }

                case 22:
                    {//인레일 그리퍼 클로즈 확인, 인레일 X축 바코드 위치 이동, 푸셔 백워드
                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                        {
                            if (CDataOption.CurEqu == eEquType.Nomal)
                            {
                                CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Bcr);
                                _SetLog("INR X axis move bcr.", CData.Dev.dInr_X_Bcr);
                            }
                            else
                            {
                                CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_Align);
                                _SetLog("INR X axis move align.", CData.Dev.dInr_X_Align);
                            }
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iInrX, CData.Dev.dInr_X_DynamicPos1);
                            _SetLog("INR X axis move DF1.", CData.Dev.dInr_X_DynamicPos1);
                        }

                        Func_PusherForward(false);

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {//인레일 X축 바코드 위치 이동 확인, 푸셔 백워드 확인, 인레일 그리퍼 센서 감지 확인,
                        if (CData.Dev.bDynamicSkip || CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
                        {
                            if (CDataOption.CurEqu == eEquType.Nomal)
                            {
                                if (!CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_Bcr))     { return false; }
                            }
                            else
                            {
                                if (!CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_Align))   { return false; }
                            }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iInrX, CData.Dev.dInr_X_DynamicPos1)) { return false; }
                        }

                        if (!Func_PusherForward(false))     { return false; }

                        if (!CIO.It.Get_X(eX.INR_GripperDetect))
                        {
                            Func_PusherForward(false);
                            CErr.Show(eErr.INRAIL_FEEDING_ERROR);
                            _SetLog("Error : INR gripper feeding fail.");

                            m_iStep = 0;
                            return true;
                        }

                        int iNum = CData.Parts[ONL].iSlot_No;      // 현재 슬롯의 인덱스
                        CData.Parts[(int)EPart.INR].iMGZ_No = CData.Parts[ONL].iMGZ_No;  // 인레일 매거진 번호에 데이터 전달
                        CData.Parts[(int)EPart.INR].iSlot_No = iNum;  // 인레일 슬롯 번호에 현재 자재 번호 전달
                        CData.Parts[(int)EPart.INR].bChkGrd = false; //200624 pjh : Grinding 중 Error Check 변수

                        //201014 jhc : InRail 투입된 자재 PCB 두께값 초기화
                        CData.Parts[(int)EPart.INR].dDfMin = 0.0;
                        CData.Parts[(int)EPart.INR].dDfMax = 0.0;
                        CData.Parts[(int)EPart.INR].dDfAvg = 0.0;

                        CData.Parts[(int)EPart.INR].dPcbMin = 0.0;
                        CData.Parts[(int)EPart.INR].dPcbMax = 0.0;
                        CData.Parts[(int)EPart.INR].dPcbMean = 0.0;

                        if (CData.Opt.bSecsUse && CData.GemForm != null)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0] = 0.0;  // [3=DF Measure 후].[0=DF]
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0] = 0.0;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0] = 0.0;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].nDF_User_Mode        = CData.Dynamic.iHeightType;

                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0] = 0.0;  // [4=Strip ID 읽음].[0=DF]
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0] = 0.0;
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0] = 0.0;
                        }

                        if ((!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf) ||
                            (!CData.Dev.bDynamicSkip && CData.Dev.eMoldSide == ESide.Top) )
                        {// Dynamic Function Using
                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckDynamic;
                        }
                        else
                        {// Dynamic Function Skip
                            // 201111 jym START : Skyworks Ori -> Bcr 순으로 변경
                            if (CData.CurCompany == ECompany.SkyWorks)
                            {
                                if (!CData.Dev.bOriSkip)
                                {
                                    CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                                }
                                else
                                {
                                    if (!CData.Dev.bBcrSkip)    { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr; }
                                    else                        { CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align; }
                                }
                            } // 201111 jym END
                            else
                            {
                                if (!CData.Dev.bBcrSkip || !CData.Dev.bOcrSkip)
                                {//바코드 사용
                                     if (CDataOption.CurEqu == eEquType.Nomal)
                                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckBcr;
                                    else
                                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                                }
                                else
                                {//바코드 스킵
                                    if (!CData.Dev.bOriSkip)
                                    {//Ori 사용
                                        if (CDataOption.CurEqu == eEquType.Nomal)
                                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_CheckOri;
                                        else
                                            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                                    }
                                    else
                                    {//Ori 스킵
                                        CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
                                    }
                                }
                            }
                        }

                        CData.Parts[ONL].iSlot_info[iNum] = (int)eSlotInfo.Empty;
                        CData.Parts[(int)EPart.INR].bExistStrip = true;             // In-Rail에 자재 있음으로 변경

                        if (CData.Opt.bFirstTopSlot)
                        {
                            if (iNum == 0)
                            {//매거진 슬롯 전부 Empty 되었을때 종료
                                Func_PusherForward(false);

                                CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                CData.LotInfo.iTInCnt++;

                                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                                {
                                    CIO.It.Set_Y(eY.IOZL_Power, true);  // SCK, JSCK 
                                }

                                _SetLog("Top slot first.  Slot all empty.  Status : " + CData.Parts[ONL].iStat);
                                
                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                        {
                            if (iNum == (CData.Dev.iMgzCnt - 1))
                            {//매거진 슬롯 전부 Empty 되었을때 종료
                                Func_PusherForward(false);

                                CData.Parts[ONL].iStat = ESeq.ONL_Place;
                                CData.LotInfo.iTInCnt++;

                                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                                {
                                    CIO.It.Set_Y(eY.IOZL_Power, true);  // SCK, JSCK 
                                }

                                _SetLog("Bottom slot first.  Slot all empty.  Status : " + CData.Parts[ONL].iStat);
                                
                                m_iStep = 0;
                                return true;
                            }
                        }

                        if (CData.Opt.bFirstTopSlot)
                            CData.Parts[ONL].iSlot_No--;
                        else
                            CData.Parts[ONL].iSlot_No++;

                        CData.LotInfo.iTInCnt++; //190407 ksg :

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            CIO.It.Set_Y(eY.IOZL_Power, true);  // SCK, JSCK 
                        }

                        CData.Parts[(int)EPart.INR].sLotName = CData.Parts[ONL].sLotName;

                        _SetLog("Lot in count : " + CData.LotInfo.iTInCnt + "  INR Status : " + CData.Parts[(int)EPart.INR].iStat);

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Tack time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }
        }

       
        /// <summary>
        /// Magazine Place Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Place()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_PLACE_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : Strip-in detect.");

                m_iStep = 0;
                return true;
            }

            //if (!Func_PusherForward(false))
            if (!CIO.It.Get_X(eX.ONL_PusherBackward))//210811 pjh : Place 시 Backward 상태가 아니면 Error 발생
            {
                CErr.Show(eErr.ONLOADER_PUSHER_FORWARD);
                _SetLog("Error : Pusher forward.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//푸셔 백워드, 클램프 다운, 인레일 스트립 인 센서 감지 확인
                        if (!Chk_ClampMgz())
                        {
                            CErr.Show(eErr.ONLOADER_CLAMP_DETECT_MGZ_READY);
                            Func_PusherForward(false);
                            CData.Parts[ONL].iStat = ESeq.ONL_Pick;
                            _SetLog("Magazine detect.  Status : " + CData.Parts[ONL].iStat);

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                            _SetLog("Error : Top magazine full.");

                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Func_PusherForward(false);
                        Func_MgzClamp(true);
                        _SetLog("Check axes.  Pusher backward.  Clamp.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//푸셔 백워드 확인, 클램프 다운 확인, Y축 대기위치 이동
                        if ((!Func_PusherForward(false)) && (!Func_MgzClamp(true)))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//Y축 대기 위치 이동 확인, Z축 PlaceGo 위치 이동, Top 매거진 Full센서 확인, Top 매거진 감지 센서 확인, 감지 되있을경우 벨트 런
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect (B접점)
                            Func_TopBeltRun(true);
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PlaceGo);                        

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                            _SetLog("Error : Top magazine full.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))    //X9C Top Conveyor Mgz Detect
                        {
                            Func_TopBeltRun(true);
                            _SetLog("Top belt run.");
                        }

                        _SetLog("Z axis move place.", CData.SPos.dONL_Z_PlaceGo);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Top 매거진 감지 센서 감지 안될 때까지 런, Z축 PlaceGo 위치 이동 확인, 딜레이 2000ms 설정
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect
                            Func_TopBeltRun(true);
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PlaceGo))
                        { return false; }

                        m_Delay.Set_Delay(2000);
                        _SetLog("Set delay : 2000ms");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Top 매거진 감지 센서 감지 안될 때까지 2초 동안 런, Top 매거진 Full센서 감지시 에러, Top 매거진 감지 센서 감지 시 벨트 정지
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect
                            Func_TopBeltRun(true);
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        if (m_Delay.Chk_Delay())
                        {
                            // 탑 벨트 스탑
                            Func_TopBeltRun(false);
                            // 2초가 지난 후에도 매거진 감지 또는 풀 매거진 감지 시  에러 발생
                            if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) || !CIO.It.Get_X(eX.ONL_TopMGZDetectFull))    //X9D Top Conveyor Full Mgz
                            {
                                //set Error
                                CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                                _SetLog("Error : Top magazine full.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                        {
                            if (CIO.It.Get_X(eX.ONL_TopMGZDetect) && CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                            {
                                // 탑 벨트 스탑
                                Func_TopBeltRun(false);
                                _SetLog("Top belt stop.");

                                m_iStep++;
                            }
                        }

                        return false;
                    }

                case 15:
                    {//Y축 Place 위치 이동
                        CMot.It.Mv_N(m_iY, CData.SPos.dONL_Y_Place);
                        _SetLog("Y axis move place.", CData.SPos.dONL_Y_Place);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//Y축 Place 위치 이동 확인, Z축 UnClamp 위치 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONL_Y_Place))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_UnClamp);
                        _SetLog("Z axis move Unclamp.", CData.SPos.dONL_Z_UnClamp);

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//Z축 UnClamp 위치 이동 확인, 클램프 업
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_UnClamp))
                        { return false; }

                        Func_MgzClamp(false);
                        _SetLog("Unclamp.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//클램프 업 확인, 푸셔 백워드
                        if (!Func_MgzClamp(false))
                        { return false; }

                        Func_PusherForward(false);
                        _SetLog("Pusher backward.");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//푸셔 백워드 확인, Z축 PlaceDn 위치 이동
                        if (!Func_PusherForward(false))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PlaceDn);
                        _SetLog("Z axis move place down.", CData.SPos.dONL_Z_PlaceDn);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//Z축 PlaceDn 위치 이동, 벨트 2초 런, 푸셔 백워드 확인, Y축 Wait 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PlaceDn))
                        { return false; }

                        // 2022.03.08 SungTae Start : [수정] ASE-KR에서도 일부 설비(SSG001호기)에서 TopBelt가 멈추지 않아서 ASE-KR도 제외
                        // 2021.09.02 lhs Start : SCK+에서 Onloader가 감지되어 TopBelt가 멈추지 않는 문제 발생하여 아래 부분 SCK, JSCK 제외
                        //if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)
                        if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK && CData.CurCompany != ECompany.ASE_KR)
                        // 2022.03.08 SungTae End
                        {
                            if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                            {
                                Func_TopBeltRun(true);
                                return false;
                            }
                            else
                            {
                                Func_TopBeltRun(false);
                            }
                        }
                        // 2021.09.02 lhs End

                        //200123 ksg :
                        if (CDataOption.OnlRfid == eOnlRFID.Use)
                        {
                            CData.Parts[ONL].sMGZ_ID = "";
                        }

                        Func_TopBeltRun(true);
                        
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        
                        m_Delay.Set_Delay(1500);
                        
                        _SetLog("Top belt run.  Set delay : 1500ms  Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//Top 매거진 감지 센서 감지 안될때까지 런
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) && !m_Delay.Chk_Delay())
                        {//X9C Top Conveyor Mgz Detect
                            Func_TopBeltRun(true);
                            _SetLog("Top belt run.");
                            return false;
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                            _SetLog("Top belt stop.");
                        }

                        m_Delay.Set_Delay(2000);

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//벨트 2초간 런 후 정지
                        //if (!m_Delay.Chk_Delay())
                        //{
                        //    if (CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        //    { //2초 대기가 끝나기 전 매거진 감지 안되면 벨트 스탑
                        //        BeltTopRun(false);
                        //        m_iStep++;
                        //    }
                        //}
                        if(m_Delay.Chk_Delay())
                        {// 2초가 지난 후 벨트 정지
                            Func_TopBeltRun(false);

                            if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) || !CIO.It.Get_X(eX.ONL_TopMGZDetectFull))    //X9D Top Conveyor Full Mgz
                            {// 2초가 지난 후에도 매거진 감지 또는 풀 매거진 감지 시  에러 발생
                             //set Error
                                CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                                _SetLog("Error : Top magazine full.");
                                m_iStep = 0; //190620 ksg : 이미 Mgz을 놓은 후 발생되는 Error라 step 완료 시킴
                                return true;
                            }

                            _SetLog("Top belt stop.");

                            m_iStep++;
                        }
                        return false;
                    }

                case 23:
                    {//Y축 Wait 위치 이동 확인, 상태 변경 Place -> Wait
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        //20191121 ghk_display_strip
                        CData.Parts[ONL].bExistStrip = false;

                        int iCnt = CData.Dev.iMgzCnt;
                        for (int i = 0; i < iCnt; i++)
                        {//매거진 슬롯 상태 정보 처리
                            CData.Parts[ONL].iSlot_info[i] = (int)eSlotInfo.Empty;
                        }

                        if (CData.IsMultiLOT())         // 2021-06-15, jhLee : Multi-LOT
                        {
                            // 더이상 Loading 진행 LOT이 없다면 초기화 시켜준다.
                            if (CData.LotMgr.LoadingLotName == "")
                            {
                                CData.Parts[ONL].iMGZ_No = 0;                               // 투입된 Magazine 번호 초기화
                            }
                        }

                        CData.bChkManual = false;   // 2022.04.25 SungTae : [추가] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag 초기화

                        CData.Parts[ONL].iStat = ESeq.ONL_Wait;
                        _SetLog("Finish.  Status : " + CData.Parts[ONL].iStat);
                        CSQ_Man.It.bOnMGZPlaceDone = true;//210309 pjh : Set Up Strip Onloader MGZ Place Done

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20191209 ghk_lotend_placemgz
        /// <summary>
        /// LotEnd 버튼 클릭시 매거진 버리는 시퀀스
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PlaceLotEnd()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_PLACE_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : INR Strip-in detected.");

                m_iStep = 0;
                return true;
            }

            if (!CIO.It.Get_X(eX.ONL_PusherBackward))//210811 pjh : Place 시 Backward 상태가 아니면 Error 발생
            {
                CErr.Show(eErr.ONLOADER_PUSHER_FORWARD);
                _SetLog("Error : Pusher forward.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//푸셔 백워드, 매거진 유무 확인
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        Func_PusherForward(false);

                        if (!Chk_ClampMgz())
                        {//매거진 없을 경우
                            _SetLog("Magazine empty.");

                            m_iStep = 23;
                            return false;
                        }

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                            _SetLog("Error : Top magazine full.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check axes.  Pusher backward.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//푸셔 백워드 확인, 클램프 다운 확인, Y축 대기위치 이동
                        if (!Func_PusherForward(false))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//Y축 대기 위치 이동 확인, Z축 PlaceGo 위치 이동, Top 매거진 Full센서 확인, Top 매거진 감지 센서 확인, 감지 되있을경우 벨트 런
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect (B접점)
                            Func_TopBeltRun(true);
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PlaceGo);

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                            _SetLog("Error : Top magazine full.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))    //X9C Top Conveyor Mgz Detect
                        {
                            Func_TopBeltRun(true);
                        }

                        _SetLog("Z axis move place.", CData.SPos.dONL_Z_PlaceGo);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Top 매거진 감지 센서 감지 안될 때까지 런, Z축 PlaceGo 위치 이동 확인, 딜레이 2000ms 설정
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect
                            Func_TopBeltRun(true);
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PlaceGo))
                        { return false; }

                        m_Delay.Set_Delay(2000);
                        _SetLog("Set delay : 2000ms");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Top 매거진 감지 센서 감지 안될 때까지 2초 동안 런, Top 매거진 Full센서 감지시 에러, Top 매거진 감지 센서 감지 시 벨트 정지
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect
                            Func_TopBeltRun(true);
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                        }

                        if (m_Delay.Chk_Delay())
                        {
                            // 탑 벨트 스탑
                            Func_TopBeltRun(false);
                            // 2초가 지난 후에도 매거진 감지 또는 풀 매거진 감지 시  에러 발생
                            if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) || !CIO.It.Get_X(eX.ONL_TopMGZDetectFull))    //X9D Top Conveyor Full Mgz
                            {
                                //set Error
                                CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                                _SetLog("Error : Top magazine full.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                        {
                            if (CIO.It.Get_X(eX.ONL_TopMGZDetect) && CIO.It.Get_X(eX.ONL_TopMGZDetectFull))
                            {
                                // 탑 벨트 스탑
                                Func_TopBeltRun(false);
                                _SetLog("Top belt stop.");

                                m_iStep++;
                            }
                        }

                        return false;
                    }

                case 15:
                    {//Y축 Place 위치 이동
                        CMot.It.Mv_N(m_iY, CData.SPos.dONL_Y_Place);
                        _SetLog("Y axis move place.", CData.SPos.dONL_Y_Place);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//Y축 Place 위치 이동 확인, Z축 UnClamp 위치 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dONL_Y_Place))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_UnClamp);
                        _SetLog("Z axis move unclamp.", CData.SPos.dONL_Z_UnClamp);

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {//Z축 UnClamp 위치 이동 확인, 클램프 업
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_UnClamp))
                        { return false; }

                        Func_MgzClamp(false);
                        _SetLog("Clamp open.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//클램프 업 확인, 푸셔 백워드
                        if (!Func_MgzClamp(false))
                        { return false; }

                        Func_PusherForward(false);
                        _SetLog("Pusher backward.");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//푸셔 백워드 확인, Z축 PlaceDn 위치 이동
                        if (!Func_PusherForward(false))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dONL_Z_PlaceDn);
                        _SetLog("Z axis move place down.", CData.SPos.dONL_Z_PlaceDn);

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//Z축 PlaceDn 위치 이동, 벨트 2초 런, 푸셔 백워드 확인, Y축 Wait 위치 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dONL_Z_PlaceDn))
                        { return false; }

                        Func_TopBeltRun(true);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        m_Delay.Set_Delay(1500);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//Top 매거진 감지 센서 감지 안될때까지 런
                        if (!CIO.It.Get_X(eX.ONL_TopMGZDetect))
                        {//X9C Top Conveyor Mgz Detect
                            Func_TopBeltRun(true);
                            _SetLog("Top belt run.");
                        }
                        else
                        {
                            Func_TopBeltRun(false);
                            _SetLog("Top belt stop.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//벨트 2초간 런 후 정지
                        if (m_Delay.Chk_Delay())
                        {// 2초가 지난 후 벨트 정지
                            Func_TopBeltRun(false);
                            _SetLog("Top belt stop.");

                            if (!CIO.It.Get_X(eX.ONL_TopMGZDetect) || !CIO.It.Get_X(eX.ONL_TopMGZDetectFull))    //X9D Top Conveyor Full Mgz
                            {// 2초가 지난 후에도 매거진 감지 또는 풀 매거진 감지 시  에러 발생
                                CErr.Show(eErr.ONLOADER_TOP_MGZ_DETECT_FULL);
                                _SetLog("Error : Top belt full.");
                            }                            

                            m_iStep = 24;
                        }
                        return false;
                    }

                case 23:
                    {//case 10에서 매거진 감지 되지 않았을때 동작, 푸셔 백워드 확인, Y축 wait 위치로 이동
                        if (!Func_PusherForward(false))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;                        
                        return false;
                    }

                case 24:
                    {//Y축 Wait 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        CData.Parts[ONL].iStat = ESeq.Idle;
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20200109 myk_ONL_Push_Pos
        /// <summary>
        /// Onloader Push Position 버튼 클릭했을 때 시퀀스
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Push_Pos()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TiOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.ONLOADER_PUSH_POSITION_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("INR Strip-in detected.");

                m_iStep = 0;
                return true;
            }

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

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Pusher Backward, 클램프 다운
                        Func_PusherForward(false);
                        Func_MgzClamp(true);
                        _SetLog("Pusher backward.  Clamp close.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//Pusher Backward 확인
                        if ((!Func_PusherForward(false)) && (!Func_MgzClamp(true)))
                        { return false; }

                        //Y축 대기위치 이동
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Y축 대기위치 이동 확인, Z축 Push Position 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        { return false; }

                        if (CData.Opt.bFirstTopSlot)
                        {//1번째 슬롯이 Top일 때
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOnL_Z_Entry_Up);
                            _SetLog("Z axis move entry up.", CData.Dev.dOnL_Z_Entry_Up);
                        }
                        else
                        {//1번째 슬롯이 Bottom일 때
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOnL_Z_Entry_Dn);
                            _SetLog("Z axis move entry down.", CData.Dev.dOnL_Z_Entry_Dn);
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Z축 Push Position 이동 확인
                        if (CData.Opt.bFirstTopSlot)
                        {//1번째 슬롯이 Top일 때
                            if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnL_Z_Entry_Up))
                            { return false; }
                        }
                        else
                        {//1번째 슬롯이 Bottom일 때
                            if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnL_Z_Entry_Dn))
                            { return false; }
                        }

                        CData.Parts[ONL].iStat = ESeq.Idle;
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }


        // 2022.04.19 SungTae Start : [추가] ASE-KH Magazine ID Reading
        /// <summary>
        /// Onloader Check Barcode 버튼 클릭했을 때 시퀀스
        /// </summary>
        /// <returns></returns>
        public bool Cyl_CheckBcr()
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
                    CErr.Show(eErr.ONLOADER_MGZ_ID_READ_TIMEOUT);
                    _SetLog("Error : ONLOADER_MGZ_ID_READ_TIMEOUT");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!CIO.It.Get_X(eX.INR_StripInDetect))
            {
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("INR Strip-in detected.");

                m_iStep = 0;
                return true;
            }

            if (!CIO.It.Get_X(eX.ONL_PusherBackward))
            {
                CErr.Show(eErr.ONLOADER_PUSHER_FORWARD);
                _SetLog("Error : Pusher forward.");

                m_iStep = 0;
                return true;
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {//축 상태 체크
                        if (!Chk_ClampMgz())
                        {
                            CErr.Show(eErr.ONLOADER_CLAMP_DETECT_MGZ_READY);
                            Func_PusherForward(false);
                            
                            _SetLog("Magazine detect.  Status : " + CData.Parts[ONL].iStat);

                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr = ""; //BCR 값 초기화

                        // Z축 MGZ ID Reading Position 이동 확인 및 Y축 MGZ ID Reading Position 이동
                        if (CMot.It.Get_Mv(m_iZ, CData.Dev.dOnL_Z_MgzBcr) && CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_MgzBcr))
                        {
                            // Y & Z축 모두 MGZ ID Reading Position면 BCR Reading Step으로 이동
                            m_iStep = 16;
                            return false;
                        }

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Pusher Backward, 클램프 다운
                        Func_PusherForward(false);
                        Func_MgzClamp(true);
                        
                        _SetLog("Pusher backward.  Clamp close.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//Pusher Backward 확인
                        if (!Func_PusherForward(false) && !Func_MgzClamp(true))
                        {
                            return false;
                        }

                        //Y축 대기위치 이동
                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOnL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //Y축 대기위치 이동 확인, Z축 MGZ ID Reading Position 이동
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_Wait))
                        {
                            return false;
                        }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOnL_Z_MgzBcr);
                        _SetLog("Z axis move BCR read pos.", CData.Dev.dOnL_Z_MgzBcr);
                        
                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        // Z축 MGZ ID Reading Position 이동 확인 및 Y축 MGZ ID Reading Position 이동
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOnL_Z_MgzBcr))
                        {
                            return false;
                        }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOnL_Y_MgzBcr);

                        _SetLog("Z axis move check & Y axis move BCR read pos.", CData.Dev.dOnL_Y_MgzBcr);

                        m_iStep++;
                        return false;
                    }

                case 15: 
                    {
                        // Y축 바코드 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOnL_Y_MgzBcr))
                        {
                            return false;
                        }

                        //////////////////////////////////////////////
                        m_iReReadCnt = 0; //바코드 판독 시도 회수 초기화
                        //////////////////////////////////////////////

                        _SetLog("Y axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        if (m_iReReadCnt > 0) //재시도인 경우 바코드 읽기 중지(Timing OFF) => Z축 +30 mm (아래로) => Z축 바코드 위치 => 바코드 읽기
                        {
                            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)
                            _SetLog("MGZ BCR timing off.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr = ""; //BCR 값 초기화
                        
                        //바코드 읽기 요청 (Timing ON)
                        CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LON); 

                        m_Delay.Set_Delay(3000);
                        _SetLog("MGZ BCR read start.  Set delay : 3000ms");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if (m_Delay.Chk_Delay())
                        {
                            // 바코드 읽기 완료 체크
                            if (!CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr.Equals(""))
                            {
                                _SetLog($"MGZ BCR read ok.  MGZ ID : {CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr}");
                            }
                            else
                            {
                                if (m_iReReadCnt < 3)
                                {
                                    m_iReReadCnt++;

                                    m_iStep = 16;
                                    return false;
                                }
                                else
                                {
                                    CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)

                                    _SetLog("MGZ BCR read fail.");

                                    m_iStep = 0;

                                    CMsg.Show(eMsg.Error, "OnLoader magazine BCR read fail", "1.Check the direction of the magazine.\r2.Please confirm that BCR module works normally.");
                                    return true;
                                }
                            }
                        }

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        CData.Parts[ONL].iStat = ESeq.Idle;
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }
        // 2022.04.19 SungTae End

        #region 동작 함수(Func)
        /// <summary>
        /// 로더 매거진 클램프/언클램프.  
        /// </summary>
        /// <param name="bVal">true:Clamp  false:Unclamp</param>
        /// <returns>true:동작 완료  false:동작 미완료</returns>
        public bool Func_MgzClamp(bool bVal)
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.ONL_ClampOn, bVal);    //YAA Onloader Clamp 
            CIO.It.Set_Y(eY.ONL_ClampOff, !bVal);    //YAB Onloader Clamp  

            if (bVal)
            { bRet = (CIO.It.Get_X(eX.ONL_ClampOn) && !CIO.It.Get_X(eX.ONL_ClampOff)); }
            else
            { bRet = (!CIO.It.Get_X(eX.ONL_ClampOn) && CIO.It.Get_X(eX.ONL_ClampOff)); }

            return bRet;
        }

        public bool Func_Clamp()
        {
            CIO.It.Set_Y(eY.ONL_ClampOn, true);   
            CIO.It.Set_Y(eY.ONL_ClampOff, false); 

            bool bRet = CIO.It.Get_X(eX.ONL_ClampOn) && (!CIO.It.Get_X(eX.ONL_ClampOff));

            return bRet;
        }

        public bool Func_Unclamp()
        {
            CIO.It.Set_Y(eY.ONL_ClampOn, false);    
            CIO.It.Set_Y(eY.ONL_ClampOff, true);    

            bool bRet = (!CIO.It.Get_X(eX.ONL_ClampOn)) && CIO.It.Get_X(eX.ONL_ClampOff); 

            return bRet;
        }

        /// <summary>
        /// 로더 푸셔 포워드/백워드.  
        /// </summary>
        /// <param name="bVal">true:Forward  false:backward</param>
        /// <returns>true:동작 완료  false:동작 미완료</returns>
        public bool Func_PusherForward(bool bVal)
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.ONL_PusherForward, bVal);    //YA5 OnLoader Pusher

            if (bVal)
            { bRet = ((CIO.It.Get_X(eX.ONL_PusherForward)) && (!CIO.It.Get_X(eX.ONL_PusherBackward))); }
            else
            { bRet = !CIO.It.Get_X(eX.ONL_PusherForward) && CIO.It.Get_X(eX.ONL_PusherBackward); }
            
            return bRet;
        }

        public bool Func_PusherF()
        {
            CIO.It.Set_Y(eY.ONL_PusherForward, true);  

            bool bRet = CIO.It.Get_X(eX.ONL_PusherForward) && (!CIO.It.Get_X(eX.ONL_PusherBackward));

            return bRet;
        }

        public bool Func_PusherB()
        {
            CIO.It.Set_Y(eY.ONL_PusherForward, false);  

            bool bRet = (!CIO.It.Get_X(eX.ONL_PusherForward)) && CIO.It.Get_X(eX.ONL_PusherBackward);

            return bRet;
        }

        /// <summary>
        /// 바텀 벨트 런/스탑.  방향 CW/CCW
        /// </summary>
        /// <param name="bRun">true:Run  false:Stop</param>
        /// <param name="bCw">true:CW  false:CCW</param>
        public void Func_BtmBeltRun(bool bRun, bool bCw)
        {
            CIO.It.Set_Y(eY.ONL_BtmBeltCCW, bCw);    //YA1 Belt Direction
            CIO.It.Set_Y(eY.ONL_BtmBeltRun, bRun);    //YA0 Belt Run
        }

        public bool Func_BtmBeltRunCW()
        {
            CIO.It.Set_Y(eY.ONL_BtmBeltCCW, true); 
            return CIO.It.Set_Y(eY.ONL_BtmBeltRun, true);
        }

        public bool Func_BtmBeltRunCCW()
        {
            CIO.It.Set_Y(eY.ONL_BtmBeltCCW, false);
            return CIO.It.Set_Y(eY.ONL_BtmBeltRun, true);
        }

        public bool Func_BtmBeltStop()
        {
            return CIO.It.Set_Y(eY.ONL_BtmBeltRun, false);
        }

        /// <summary>
        /// 탑 벨트 런/스탑.
        /// </summary>
        /// <param name="bRun">true:Run  false:Stop</param>
        public void Func_TopBeltRun(bool bRun)
        {//YA5 OnLoader Pusher
            CIO.It.Set_Y(eY.ONL_TopBeltRun, bRun);
            CIO.It.Set_Y(eY.ONL_BtnBeltRun, bRun); //Top Belt Run Btn Lamp 
        }

        public bool Func_TopBeltRun()
        {
            CIO.It.Set_Y(eY.ONL_BtnBeltRun, true);
            return CIO.It.Set_Y(eY.ONL_TopBeltRun, true);
        }

        public bool Func_TopBeltStop()
        {
            CIO.It.Set_Y(eY.ONL_BtnBeltRun, false);
            return CIO.It.Set_Y(eY.ONL_TopBeltRun, false);
        }
        #endregion

        #region 체크 함수(Chk)       

        public bool Chk_Axes(bool bHD = true)
        {
            int iRet = 0;

            iRet = CMot.It.Chk_Rdy(m_iX, bHD);
            if (iRet != 0)
            {

                CErr.Show(eErr.ONLOADER_X_NOT_READY);
                _SetLog("Error : X axis not ready.");

                return true;
            }

            iRet = CMot.It.Chk_Rdy(m_iY, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.ONLOADER_Y_NOT_READY);
                _SetLog("Error : Y axis not ready.");

                return true;
            }

            iRet = CMot.It.Chk_Rdy(m_iZ, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.ONLOADER_Z_NOT_READY);
                _SetLog("Error : Z axis not ready.");

                return true;
            }

            return false;
        }

        /// <summary>
        /// 클램프 매거진 감지 유무.  1번, 2번 센서 2개가 다 같은 동작을 해야함.
        /// </summary>
        /// <returns>true:감지  false:미감지</returns>
        public bool Chk_ClampMgz()
        {
            if (GV.IDLE_MODE)
            { return true; }

            bool bRet = false;
            bRet = (CIO.It.Get_X(eX.ONL_ClampMGZDetect1) && CIO.It.Get_X(eX.ONL_ClampMGZDetect2));
            return bRet;
        }

        public bool Chk_ClampMgz1(bool bOn)
        {
            if (GV.IDLE_MODE)
            { return true; }

            return CIO.It.Get_X(eX.ONL_ClampMGZDetect1) == bOn;
        }

        public bool Chk_ClampMgz2(bool bOn)
        {
            if (GV.IDLE_MODE)
            { return true; }
            
            return CIO.It.Get_X(eX.ONL_ClampMGZDetect2) == bOn;
        }

        public bool Chk_TopDetect(bool bOn)
        {
            if (GV.IDLE_MODE)
            { return true; }

            return !CIO.It.Get_X(eX.ONL_TopMGZDetect) == bOn;
        }

        public bool Chk_TopFull(bool bOn)
        {
            if (GV.IDLE_MODE)
            { return true; }

            return !CIO.It.Get_X(eX.ONL_TopMGZDetectFull) == bOn;
        }

        public bool Chk_BtmDetect(bool bOn)
        {
            if (GV.IDLE_MODE)
            { return true; }

            return !CIO.It.Get_X(eX.ONL_BtmMGZDetect) == bOn;
        }

        /// <summary>
        /// 푸셔 오버로드 확인.
        /// </summary>
        /// <returns>true:오버로드  false:정상</returns>
        public bool Chk_PusherOverload()
        {
            bool bRet = CIO.It.Get_X(eX.ONL_PusherOverLoad);
            return bRet;
        }

        // Light Curtain이 감지되는지 확인한다. Main 화면 상단에 감지여부 표시용
        public bool Chk_LightCurtain()
        {
            return CIO.It.Get_X(eX.ONL_LightCurtain);
        }
        #endregion

        #region 계산 함수(Calc) 
        /// <summary>
        /// 매거진 슬롯 자재 유무 확인후 비어있는 가장 빠른 슬롯 Z축 위치 값 반환
        /// </summary>
        /// <returns></returns>
        private double _Cal_WorkPos()
        {
            int iRet = 0;
            double dZPos = 0;

            if(CData.Opt.bFirstTopSlot)
            {
                for (int i = CData.Parts[ONL].iSlot_info.Length - 1; i >= 0 ; i--)
                {
                    if (CData.Parts[ONL].iSlot_info[i] == (int)eSlotInfo.Exist)
                    {
                        iRet = i;
                        break;
                    }
                }
                CData.Parts[ONL].iSlot_No = iRet;
                dZPos = CData.Dev.dOnL_Z_Entry_Dn + CData.Dev.dMgzPitch * iRet;
            }
            else
            {
                int iCnt = CData.Parts[ONL].iSlot_info.Length;
                for (int i = 0; i < iCnt; i++)
                {
                    if (CData.Parts[ONL].iSlot_info[i] == (int)eSlotInfo.Exist)
                    {
                        iRet = i;
                        break;
                    }
                }
                CData.Parts[ONL].iSlot_No = iRet;
                dZPos = CData.Dev.dOnL_Z_Entry_Dn + CData.Dev.dMgzPitch * iRet;
            }
            return dZPos;
        }
        #endregion


        // 2021-02-05, jhLee : Skyworks Light-Curtain 감지시 일시멈춤기능을 별도의 함수로 변경
        //
        // Light curtain 감지시 일시멈춤을 지정하고 센서 감지 결과를 리턴해준다.
        public bool CheckLightCurtainPause()
        {
            bool bSensor = CIO.It.Get_X(eX.ONL_LightCurtain);         // Area Sensor 감지 여부

            // AreaSensor가 감지되는 동안 모터 일시 정지 기능을 이용할 것인가 ?
            if (CDataOption.IsPause)
            {
                // 2021-02-05, jhLee : Skyworks 무언정지 원인으로 AreaSensor 동작을 개선하기위해 기능을 축소
                //                      각 사이트별로 센서 감지시 동작을 고객과 다시 협의할 필요가 있다.
                //                      // 현재는 감지시 On-Loader/Off-Loader 자체만 정지하도록 한다.

                // Area sensor가 감지시 일시 정지 처리
                if (bSensor)
                {
                    //OnLoader elevator Parts
                    CMot.It.Pause((int)EAx.OnLoader_X);
                    CMot.It.Pause((int)EAx.OnLoader_Y);
                    CMot.It.Pause((int)EAx.OnLoader_Z);

                    //Inrail과 On-Picker의 일시 정지는 고객사와 협의 필요

                    //Inrail Parts
                    CMot.It.Pause((int)EAx.Inrail_X);
                    CMot.It.Pause((int)EAx.Inrail_Y);

                    if (CData.CurCompany == ECompany.SkyWorks)  // 스카이웍스만 피커 까지 정지
                    {
                        //OnLoader Picker Parts
                        CMot.It.Pause((int)EAx.OnLoaderPicker_X);
                        CMot.It.Pause((int)EAx.OnLoaderPicker_Z);
                        CMot.It.Pause((int)EAx.OnLoaderPicker_Y);
                    }
                }
                else
                {
                    //OnLoader elevator Parts
                    CMot.It.Resume((int)EAx.OnLoader_X);
                    CMot.It.Resume((int)EAx.OnLoader_Y);
                    CMot.It.Resume((int)EAx.OnLoader_Z);

                    //Inrail Parts
                    CMot.It.Resume((int)EAx.Inrail_X);
                    CMot.It.Resume((int)EAx.Inrail_Y);

                    if (CData.CurCompany == ECompany.SkyWorks)  // 스카이웍스만 피커 까지 정지
                    {
                        //OnLoader Picker Parts
                        CMot.It.Resume((int)EAx.OnLoaderPicker_X);
                        CMot.It.Resume((int)EAx.OnLoaderPicker_Z);
                        CMot.It.Resume((int)EAx.OnLoaderPicker_Y);
                    }
                }
            }

            return bSensor;         // 감지여부 회신
        }



        //
        // 2021-05-31, jhLee : Idle 상태에서 Light Curtain 감지시 Idle -> Pickup 으로 상태 전환을 위해 센서 상태를 지정한다.
        //
        public void SetLightCurtainDetect(bool bFlag)
        {
            if (CData.Parts[ONL].iStat == ESeq.Idle)
            {
                CData.Parts[ONL].iStat = ESeq.ONL_Pick;            // Magazine Pickup을 수해할 수 있도록 지정한다.
            }
        }

    }
}
