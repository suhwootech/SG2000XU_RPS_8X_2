using System;
using System.Diagnostics;
using System.IO;
// 2020.09.28 JSKim St
using System.Threading;
// 2020.09.28 JSKim Ed

using System.Linq;              // 2022.03.26 SungTae : [추가] Multi-LOT 관련
using System.Windows.Forms;     // 2022.04.26 SungTae : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련

namespace SG2000X
{

    public class CSq_OfL : CStn<CSq_OfL>
    {
        private const int CNT_QCSafetyCheck = 20;           // 2021-02-02, jhLee :  QC와의 간섭이 없는지 체크하는 최대 대기 횟수, QCPermission에서 사용
        private const int TIME_QCSafetyCheck = 3000;        //                      QC와의 간섭이 없는지 체크하는 반복 주기

        private readonly int TIMEOUT = 30000;
        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep = 0;
        private int m_iPreStep = 0;
        private int m_iReReadCnt = 0; //190418 ksg :

        private int m_iX = (int)EAx.OffLoader_X;
        private int m_iY = (int)EAx.OffLoader_Y;
        private int m_iZ = (int)EAx.OffLoader_Z;
        private int m_iDX = (int)EAx.DryZone_X; //200508 pjh : Dry X축

        private bool m_ActUp      = true ;
        private bool m_ActDn      = false;
        private bool m_ActOpen    = false;
        private bool m_ActClose   = true ;
        private bool m_BeltRun    = true ;
        private bool m_BeltStop   = false;
        private bool m_BeltCw     = true ;
        private bool m_BeltCCw    = false;
        private bool m_bxChkLight = false;

        private bool m_bReqStop     = false;
        public bool m_bDuringGOut  = false; // 양품 Mgz으로 배출중인가 ?
        public bool m_bDuringFOut  = false; // 불량 Mgz으로 배출중인가 ?

        public  bool m_GetQcWorkEnd = false; //190406 ksg :Qc
        public  bool m_ChkQcStrip   = false;
           
        private bool m_bPreChkLight = false;

        public int m_nDuringOut = 0;// 1: Good , 2: NG 20210123 yyy
        public int m_nCurSlot = 0;// 20210123 yyy

        private CTim m_Delay       = new CTim();
        public  CTim m_DelayRsl    = new CTim(); //190406 ksg :Qc
        public  CTim m_DelayWE     = new CTim(); //190406 ksg :Qc
        private CTim m_TiOut       = new CTim();
        private CTim m_TLightDelay = new CTim();
        private CTim m_TSendDelay  = new CTim();
        private CTim m_LightTiOut  = new CTim();
        private CTim m_tmSafety = new CTim();

        // private CTim m_Permission = new CTim();
        private int m_nSafetyCount = 0;                 // QC의 Safety 관련 얼마나 대기하는가 ? 일정 횟수를 넘기면 Alarm이 필요하다.


        //190807 ksg : TackTime 
        private DateTime m_StartSeq;
        private DateTime m_EndSeq  ;
        private TimeSpan m_SeqTime ;

        // 2020-11-19, jhLee : 이동 위치 보관용 변수
        private double m_dMovePos = 0.0;

        ESeq iSeq;
        public ESeq SEQ = ESeq.Idle;

        // 2021-02-05, jhLee : AreaSensor 감지여부, 메인화면 상단에 표시를 위한 변수, 기존 MainSeq에서 이동되어 옴
        public bool m_bDspRightArea = false;


        public tLog m_LogVal = new tLog(); //190218 ksg :
        /// <summary>
        /// 210309 pjh : Auto Load Stop 기능 동작 완료 Check
        /// </summary>
        public bool bChkAutoLoadStopEnd = false;
        //
        /// <summary>
        /// 210309 pjh : 자재 투입/배출 수량 비교 변수
        /// </summary>
        public bool bChkInOutCnt = false;
        //
        /// <summary>
        /// 210309 pjh : Strip이 설정한 수량 만큼 투입 됐는지 확인하는 변수 
        /// </summary>
        public bool bSetUpStripCnt = false;
        //

        // 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는가 ?
        public bool bReadyNextSlot = false;



        private CSq_OfL()
        {
            m_bHD = false;
        }

        public bool ToReady()
        {
            m_bReqStop = false;
            SEQ        = ESeq.Idle;
            m_DelayWE.Set_Delay(2000);
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
        /// Auto Cycle & define Step
        /// </summary>      
        public bool AutoRun()
        {
            /*
            if (CSQ_Main.It.m_iStat == eStatus.Stop)
            { return false; }
            */
            iSeq = CData.Parts[(int)EPart.OFL].iStat;
            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);


            #region Light-Curatian Check 
                // Area Sensor 감지여부 처리
                m_bDspRightArea = CheckLightCurtainPause();         // Area Sensor 감지 여부로 일시멈춤 기능 호출

                // 2021-02-05, jhLee : 일시 멈춤 기능을 이용하는 Skyworks에서도 10초 이후 Alarm 기능은 이용하고 있다.
                // 200618 jym : 스카이웍스 제외한 조건에서 일시정지 안쓰는 조건으로 변경

                if (m_bDspRightArea)
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
                            CErr.Show(eErr.OFFLOADER_DETECT_LIGHTCURTAIN);

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
            #endregion // of #region Light-Curatian Check


            //Error 확인
            //ksg
            //if (CSQ_Main.It.m_iStat == eStatus.Error || CSQ_Main.It.m_iStat != eStatus.Auto_Running)
            //{ return false; }

            if (SEQ == (int)ESeq.Idle)
            {
                if (m_bReqStop)                             { return false; }

                if (CSQ_Main.It.m_iStat == EStatus.Stop)    { return false; }

                //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                //조건 상태 확인
                int  iIsDryMgzNum  = CData.Parts[(int)EPart.DRY].iMGZ_No;
                int  iIsQcvMgzNum  = CData.Parts[(int)EPart.QCV].iMGZ_No;       // 2022.01.17 SungTae : [추가] QC-Vision 사용 시
                int  iIsOflMgzNum  = CData.Parts[(int)EPart.OFL].iMGZ_No;

                // 2022.03.26 SungTae Start : [추가] QC-Vision Multi-LOT 관련
                int iIdx = 0;

                if (CData.IsMultiLOT())
                {
                    if(CData.LotMgr.UnloadingLotName != "")
                        iIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.UnloadingLotName);
                }
                // 2022.03.26 SungTae End

                bool bMgzTopCheck  = Get_TopMgzChk      ();                     // 위쪽 magazine 감지
                bool bMgzBtmCheck  = Get_BtmMgzChk      ();                     // 아래쪽 Magazine 감지 
                bool bReadyMidMsg  = Get_MidConReadyMgz ();
                bool bMgzFullSlot  = GetFullSlotCheck   ();                     // 모든 Slot이 가득 찼는가 ?
                bool bTMgzFullSlot = GetTopFullSlotCheck();  //190404 ksg : Qc 
                bool bStripNone    = Get_StripNone      ();                     // 장비내 Strip이 존재하지 않는가 ?
                bool bOnLdWorkEnd  = Get_OnLoaderWorkEnd();                     // OnLoader의 작업 종료
                bool bLastSlot     = Get_LastSlot       ();                     // 투입되는 Mgz 수량을 넘기고, Mgz의 Slot을 가득 채운경우
                bool bDiffMgz = false;         
                bool bFstSlotEmpty = CData.Parts[(int)EPart.OFL].iSlot_info[0] == (int)eSlotInfo.Empty;     // 첫번째 Slot이 비어있는가 ?
                bool bWorkEnd = false;

                bool bUnloadLotComplete = Get_LotComplete();                    // MultiLOT 환경에서 현재 Unloading되는 LOT이 Complete(종료)되었는지 check, Mgz Place 여부 판단용
                
                bool MgzPlace1 = false;
                bool MgzPlace2 = false;
                bool MgzPlace3 = false;
                bool TMgzPlace1 = false;
                bool TMgzPlace2 = false;
                bool TMgzPlace3 = false;
                //조건 맞추기
                bool bWait      = false;
                bool bBtmPick   = false;
                bool bBtmPlace  = false;
                bool bBtmRecive = false;
                bool bTopPick   = false;
                bool bTopPlace  = false;
                bool bTopRecive = false;

                bool bBtmRcvAction = false;     // Need for GOOD Mgz ready
                bool bTopRcvAction = false;     // Need for NG Mgz ready

                // 2022.01.11 SungTae Start : [추가]
                string sData = string.Empty;    
                string sMode;
                sMode = (CData.Opt.bQcByPass == true) ? "ByPass" : "Inspection";
                // 2022.01.11 SungTae End

                if (CData.Opt.bFirstTopSlotOff)
                {
                    bFstSlotEmpty = CData.Parts[(int)EPart.OFL].iSlot_info[CData.Dev.iMgzCnt -1] == (int)eSlotInfo.Empty;
                }
                else
                {
                    bFstSlotEmpty = CData.Parts[(int)EPart.OFL].iSlot_info[0] == (int)eSlotInfo.Empty;
                }

                // 연속LOT 일 경우 Magazine마다의 일련 번호를 이용하여 동일한 Magazine인지 구분한다.
                if (CData.IsMultiLOT())
                {
                    // 기존의 iMGZ_No의 경우 다수의 LOT이 진행되는 연속LOT 상황일 경우 동일한 Mgz 번호가 발생할 우려가 있다.
                    iIsDryMgzNum = CData.Parts[(int)EPart.DRY].nLoadMgzSN;
                    iIsQcvMgzNum = CData.Parts[(int)EPart.QCV].nLoadMgzSN;		// 2022.03.22 SungTae : [추가] QC-Vision Multi-LOT 관련
                    iIsOflMgzNum = CData.Parts[(int)EPart.OFL].nLoadMgzSN;
                }

                
                // 2022.01.17 SungTae Start : [추가] QC-Vision 사용할 경우에 대한 조건 추가
                // DryZone에 존재하는 Strip이 들어있던 투입 매거진 번호와 
                // 이미 배출된 Strip이 들어있던 투입 매거진의 번호가 다르고
                // 배출 매거진의 첫번째 Slot이 비어있지 않은경우 매거진 분리를 할것인가 ?

                // 2022.03.14 SungTae Start : [수정] (Skyworks VOC) Overload Issue 관련 Site Option으로 구분 (발생일 : 2022/03/10)
                //if (CDataOption.UseQC && CData.Opt.bQcUse)
                if (CDataOption.UseQC && CData.Opt.bQcUse &&
                    (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ))
                // 2022.03.14 SungTae End
                {
                    // 2022.03.26 SungTae Start : [수정] QC-Vision Multi-LOT 관련 조건 수정                  
                    if(CData.IsMultiLOT())
                    {
                        if (((iIsQcvMgzNum != iIsOflMgzNum) && (CData.Parts[(int)EPart.QCV].sLotName != CData.LotMgr.UnloadingLotName)) &&
                            (iIsOflMgzNum > 0) && !bFstSlotEmpty)
                        {
                            // Unloading 하는 LOT이 Complete 상태이거나 배출하는 Strip의 수량이 해당 LOT의 Total Count와 같으면
                            if (bUnloadLotComplete || CData.LotInfo.iTOutCnt == CData.LotMgr.ListLOTInfo.ElementAt(iIdx).iTotalStrip)
                            {
                                bDiffMgz = true;
                                CData.LotInfo.iTOutCnt = 0;     // 현재 LOT의 배출 수량은 "0"으로 Reset 한다.
                            }
                            else
                                bDiffMgz = false;
                        }
                        else
                            bDiffMgz = false;
                    }
                    else
                    {
                        //if ((iIsQcvMgzNum != iIsOflMgzNum) && (iIsOflMgzNum > 0) && !bFstSlotEmpty)
                        if ((CQcVisionCom.nQCexistStrip == 1) &&
                            ((iIsQcvMgzNum != iIsOflMgzNum) && (CData.Parts[(int)EPart.QCV].sLotName != CData.Parts[(int)EPart.OFL].sLotName)) &&
                            (iIsOflMgzNum > 0) && !bFstSlotEmpty)
                            bDiffMgz = true;
                        else
                            bDiffMgz = false;
                    }
                    // 2022.03.26 SungTae End
                }
                else
                {
                    // 2022.04.29 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 조건 수정
                    // QC-Vision 미사용이거나 QC-Vision 사용하지만 Site가 Skyworks면
                    if (CData.IsMultiLOT() || CData.CurCompany == ECompany.ASE_K12)
                    {
                        if ((iIsDryMgzNum != iIsOflMgzNum) &&
                            (CData.Parts[(int)EPart.DRY].sLotName != CData.LotMgr.UnloadingLotName || CData.Parts[(int)EPart.DRY].sMGZ_ID != CData.Parts[(int)EPart.OFL].sMGZ_ID) &&
                            (iIsOflMgzNum > 0) && !bFstSlotEmpty)
                        {
                            _SetLog($"CData.Parts[DRY].sLotName : {CData.Parts[(int)EPart.DRY].sLotName}, UnloadingLotName : {CData.LotMgr.UnloadingLotName}");

                            // Unloading 하는 LOT이 Complete 상태이거나 배출하는 Strip의 수량이 해당 LOT의 Total Count와 같으면
                            if (bUnloadLotComplete || CData.LotInfo.iTOutCnt == CData.LotMgr.ListLOTInfo.ElementAt(iIdx).iTotalStrip)
                            {
                                bDiffMgz = true;
                                CData.LotInfo.iTOutCnt = 0;     // 현재 LOT의 배출 수량은 "0"으로 Reset 한다.
                            }
                            else
                                bDiffMgz = false;
                        }
                        else
                            bDiffMgz = false;
                    }
                    else
                    {
                        if ((iIsDryMgzNum != iIsOflMgzNum) && (iIsOflMgzNum > 0) && !bFstSlotEmpty)
                            bDiffMgz = true;
                        else
                            bDiffMgz = false;
                    }
                    // 2022.04.29 SungTae End
                }
                // 2022.01.17 SungTae End

                // LOT End 전송   
                if (CDataOption.UseQC && CData.Opt.bQcUse)
                {
                    if(m_DelayWE.Chk_Delay())
                    {
                        bool bWorkEndCheck = (  CData.Parts[(int)EPart.ONL].iStat   == ESeq.ONL_WorkEnd &&
                                                CData.Parts[(int)EPart.INR].iStat   == ESeq.INR_WorkEnd &&
                                                CData.Parts[(int)EPart.GRDL].iStat  == ESeq.GRL_WorkEnd &&
                                                CData.Parts[(int)EPart.GRDR].iStat  == ESeq.GRR_WorkEnd &&
                                                CData.Parts[(int)EPart.OFP].iStat   == ESeq.OFP_WorkEnd &&
                                                CData.Parts[(int)EPart.DRY].iStat   == ESeq.DRY_WorkEnd );

                        // 모든 작업이 끝났고 QC Vision에서만 Strip이 존재하면 QC Vision에게 LOT End를 보낸다.
                        if (bWorkEndCheck && CQcVisionCom.nQCexistStrip == 1) //CQcVisionCom.nQCexistStrip (QC strip 유무)
                        {
                            // send LotEnd command to QC
                            CGxSocket.It.SendMessage("LotEnd");//Lot End all

                            _SetLog("[SEND](EQ->QC) LotEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            
                            m_DelayWE.Set_Delay(3000);
                        }
                    }

                    // 작업이 종료되었는지 Check한다.
                    bWorkEnd =(CData.Parts[(int)EPart.ONL ].iStat == ESeq.ONL_WorkEnd &&
                               CData.Parts[(int)EPart.INR ].iStat == ESeq.INR_WorkEnd &&
                               CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                               CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                               CData.Parts[(int)EPart.OFP ].iStat == ESeq.OFP_WorkEnd &&
                               CData.Parts[(int)EPart.DRY ].iStat == ESeq.DRY_WorkEnd &&
                               CQcVisionCom.nQCexistStrip == 0);                                // QC에 Strip이 없다면 QC 작업 종료
                               //m_GetQcWorkEnd                                            ) ;
                }
                else   // QC 미사용
                {
                    bWorkEnd =(CData.Parts[(int)EPart.ONL ].iStat == ESeq.ONL_WorkEnd &&
                               CData.Parts[(int)EPart.INR ].iStat == ESeq.INR_WorkEnd &&
                               CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                               CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd &&
                               CData.Parts[(int)EPart.OFP ].iStat == ESeq.OFP_WorkEnd &&
                               CData.Parts[(int)EPart.DRY ].iStat == ESeq.DRY_WorkEnd   ) ;
                }

                // QC를 사용하는 경우
                if (CDataOption.UseQC && CData.Opt.bQcUse)
                {
                    // 1. 배출 요청이 수신되었나 ? QCReadyQuery GOOD or NG
                    // 2. 해당 Magazine이 준비 되었나 ? nMgzReady 1:Good or 2:NG
                    // 

                    // Strip을 받아오는 순서인 경우    
                    if (iSeq == ESeq.OFL_BtmRecive || iSeq == ESeq.OFL_TopRecive)
                    {
                        //----------------------------------------------------------------------------------------------------------------------//
                        // 2021.12.09 SungTae Start : [수정] (Qorvo향 VOC)
                        if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                            (CData.Opt.bOfLMgzUnloadType || CData.Opt.bQcByPass))
                        {
                            // 양품 or 불량품 Magazine을 요청하였다면
                            if (CQcVisionCom.m_nReadyReqMgz != CQcVisionCom.eMGZ_NONE)
                            {
                                // 양품 or 불량품 모두 하단 Mgz으로 작업을 진행한다.
                                iSeq = ESeq.OFL_BtmRecive;
                                CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                                bBtmRcvAction = true;       // Need Action
                                                            //bReadyNextSlot = true;

                                CQcVisionCom.rcvQCReadyQueryGOOD = false;
                                CQcVisionCom.rcvQCReadyQueryNG = false;
                            }
                        }
                        else
                        {
                            // Inspection Mode일 경우
                            if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_GOOD)    // 양품 Magazine을 요청하였다. 
                            {
                                if (CQcVisionCom.m_nMgzReady != CQcVisionCom.eMGZ_GOOD)     // 양품 Magazine이 준비되지 않았다면
                                {
                                    // 양품은 하단 Mgz으로 작업을 진행한다.
                                    iSeq = ESeq.OFL_BtmRecive;
                                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                                    bBtmRcvAction = true;       // Need Action

                                    // 참조를 마친 변수 clear
                                    CQcVisionCom.rcvQCReadyQueryGOOD = false;
                                    CQcVisionCom.rcvQCReadyQueryNG = false;
                                }
                            }
                            else if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_NG)    // 불량품 Magazine을 요청하였다. 
                            {
                                if (CQcVisionCom.m_nMgzReady != CQcVisionCom.eMGZ_NG)     // 불량품 Magazine이 준비되지 않았다면
                                {
                                    // 불량품은 상단 Mgz으로 작업을 진행한다.
                                    iSeq = ESeq.OFL_TopRecive;
                                    CData.Parts[(int)EPart.OFR].iStat = ESeq.OFL_TopRecive;
                                    bTopRcvAction = true;       // Need Action

                                    CQcVisionCom.rcvQCReadyQueryNG = false;
                                    CQcVisionCom.rcvQCReadyQueryGOOD = false;
                                }
                            }
                        }

                        // 수신된 데이터 처리
                        // 전송 완료 / 전송 취소 처리
                        if (CQcVisionCom.m_nQCSend == CQcVisionCom.eQCSend_End)             // QC에서 전송 완료가 수신되었다.
                        {
                            sData = Chk_QCSendMsg(CQcVisionCom.m_nQCSend);
                            _SetLog($">>> [RECV](QC -> EQ) QCSendMessage : {sData}");

                            if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                                (CData.Opt.bOfLMgzUnloadType || CData.Opt.bQcByPass))
                            {
                                if (CQcVisionCom.m_nReadyReqMgz != CQcVisionCom.eMGZ_NONE)      // 양품 or 불량품 Magazine을 요청한 상태애서 전송 완료
                                {
                                    // Mgz에 데이터를 채워 넣는다.
                                    int SlotNum = GetBtmEmptySlot();

                                    CData.Parts[(int)EPart.OFL].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;         // 이제 Strip이 존재한다고 지정
                                    CData.LotInfo.iTOutCnt++;                                                       // 배출 수량 증가

                                    try
                                    {
                                        // 2022.01.11 SungTae Start : [수정] (Qorvo향 VOC) QC-Vision 관련
                                        // (QC -> EQ) ByPass Mode일 경우 : 판정결과(NG)만 있고, 나머지 데이터는 없다
                                        if (CData.CurCompany == ECompany.SkyWorks/*CData.Opt.bQcByPass*/)
                                        {
                                            CData.Parts[(int)EPart.OFL].sLotName = CData.Parts[(int)EPart.DRY].sLotName;
                                            CData.Parts[(int)EPart.OFL].iMGZ_No  = CData.Parts[(int)EPart.DRY].iMGZ_No;
                                            
                                            if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No--;
                                            else                            CData.Parts[(int)EPart.OFL].iSlot_No++;
                                        }
                                        else
                                        {
                                            CData.Parts[(int)EPart.OFL].sLotName    = CData.Parts[(int)EPart.QCV].sLotName;
                                            CData.Parts[(int)EPart.OFL].iMGZ_No     = CData.Parts[(int)EPart.QCV].iMGZ_No;
                                            CData.Parts[(int)EPart.OFL].iSlot_No    = CData.Parts[(int)EPart.QCV].iSlot_No;
                                            CData.Parts[(int)EPart.OFL].nLoadMgzSN  = CData.Parts[(int)EPart.QCV].nLoadMgzSN;		// 2022.03.22 SungTae : [추가] QC-Vision Multi-LOT 관련

                                            Array.Copy(CData.Parts[(int)EPart.QCV].dPcb, CData.Parts[(int)EPart.OFL].dPcb, CData.Parts[(int)EPart.QCV].dPcb.Length);

                                            CData.Parts[(int)EPart.OFL].dPcbMax     = CData.Parts[(int)EPart.QCV].dPcbMax;
                                            CData.Parts[(int)EPart.OFL].dPcbMin     = CData.Parts[(int)EPart.QCV].dPcbMin;
                                            CData.Parts[(int)EPart.OFL].dPcbMean    = CData.Parts[(int)EPart.QCV].dPcbMean;

                                            if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No--;
                                            else                            CData.Parts[(int)EPart.OFL].iSlot_No++;

                                            //CData.Parts[(int)EPart.OFL].sLotName = CQcVisionCom.sStringInfo[0];
                                            //if (Int32.TryParse(CQcVisionCom.sStringInfo[1], out CData.Parts[(int)EPart.OFL].iMGZ_No) == false)
                                            //{
                                            //    CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                                            //}

                                            //if (Int32.TryParse(CQcVisionCom.sStringInfo[2], out CData.Parts[(int)EPart.OFL].iSlot_No) == false)
                                            //{
                                            //    CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                                            //}
                                        }
                                        // 2022.01.11 SungTae End
                                    }
                                    catch
                                    {
                                        CData.Parts[(int)EPart.OFL].sLotName = "Exception";
                                        CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                                        CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                                    }

                                    // 2021.12.13 SungTae Start : [수정] (Qorvo향 VOC) 1:1 Matching 관련 조건 추가
                                    sData = Chk_MgzJudgment(CQcVisionCom.m_nReadyReqMgz);
                                    
                                    _SetLog($"");
                                    _SetLog($"QC-Vision Result (Mode : {sMode})>>>>>");
                                    _SetLog($"Lot Name : {CData.Parts[(int)EPart.OFL].sLotName}, Result : {sData}");
                                    
									// 2022.03.22 SungTae Start : [수정] QC-Vision Multi-LOT 관련
									//_SetLog($"Mgz No. : QCV({CData.Parts[(int)EPart.QCV].iMGZ_No}) -> OFL({CData.Parts[(int)EPart.OFL].iMGZ_No})");
                                    _SetLog($"Mgz No. : QCV({iIsQcvMgzNum}) -> OFL({iIsOflMgzNum})");
                               		// 2022.03.22 SungTae End
									
									_SetLog($"Slot No. : {SlotNum}, Strip-Out Cnt : {CData.LotInfo.iTOutCnt}, Diff. Mgz : {bDiffMgz}");
                                    _SetLog($"");
                                    // 2021.12.13 SungTae End

                                    m_EndSeq = DateTime.Now;
                                    m_SeqTime = m_EndSeq - m_StartSeq;

                                    //_SetLog($"QC Send-End, Transfer finish GOOD-Mgz Slot:{SlotNum}. Lot:{CData.Parts[(int)EPart.OFL].sLotName}");
                                }
                            }
                            else
                            {
                                if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_GOOD)      // 양품 Magazine을 요청한 상태애서 전송 완료
                                {
                                    // Mgz에 데이터를 채워 넣는다.
                                    int SlotNum = GetBtmEmptySlot();

                                    CData.Parts[(int)EPart.OFL].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;         // 이제 Strip이 존재한다고 지정
                                    CData.LotInfo.iTOutCnt++;                                                       // 배출 수량 증가

                                    try
                                    {
                                        CData.Parts[(int)EPart.OFL].sLotName = CQcVisionCom.sStringInfo[0];
                                        if (Int32.TryParse(CQcVisionCom.sStringInfo[1], out CData.Parts[(int)EPart.OFL].iMGZ_No) == false)
                                        {
                                            CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                                        }

                                        if (Int32.TryParse(CQcVisionCom.sStringInfo[2], out CData.Parts[(int)EPart.OFL].iSlot_No) == false)
                                        {
                                            CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                                        }
                                    }
                                    catch
                                    {
                                        CData.Parts[(int)EPart.OFL].sLotName = "Exception";
                                        CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                                        CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                                    }

                                    m_EndSeq = DateTime.Now;
                                    m_SeqTime = m_EndSeq - m_StartSeq;

                                    _SetLog($"QC Send-End, Transfer finish GOOD-Mgz Slot:{SlotNum}. Lot:{CData.Parts[(int)EPart.OFL].sLotName}. Out Cnt:{CData.LotInfo.iTOutCnt}");
                                }
                                else if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_NG)      // 불량품 Magazine을 요청한 상태애서 전송 완료
                                {
                                    int SlotNum = GetTopEmptySlot();

                                    CData.Parts[(int)EPart.OFR].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;         // 이제 Strip이 존재한다고 지정
                                    CData.LotInfo.iTOutCnt++;                                                       // 배출 수량 증가

                                    try
                                    {
                                        CData.Parts[(int)EPart.OFR].sLotName = CQcVisionCom.sStringInfo[0];

                                        if (Int32.TryParse(CQcVisionCom.sStringInfo[1], out CData.Parts[(int)EPart.OFR].iMGZ_No) == false)
                                        {
                                            CData.Parts[(int)EPart.OFR].iMGZ_No = 0;
                                        }

                                        if (Int32.TryParse(CQcVisionCom.sStringInfo[2], out CData.Parts[(int)EPart.OFR].iSlot_No) == false)
                                        {
                                            CData.Parts[(int)EPart.OFR].iSlot_No = 0;
                                        }
                                    }
                                    catch
                                    {
                                        CData.Parts[(int)EPart.OFR].sLotName = "Exception";
                                        CData.Parts[(int)EPart.OFR].iMGZ_No = 0;
                                        CData.Parts[(int)EPart.OFR].iSlot_No = 0;
                                    }

                                    m_EndSeq = DateTime.Now;
                                    m_SeqTime = m_EndSeq - m_StartSeq;

                                    _SetLog($"QC Send-End, Transfer finish NG-Mgz Slot:{SlotNum}. Lot:{CData.Parts[(int)EPart.OFR].sLotName}. Out Cnt:{CData.LotInfo.iTOutCnt}");
                                }
                                else // Logic error
                                {
                                    _SetLog($"ERROR ! QC Send-End Without QCReadyQuery, Ignored command");
                                }
                            }

                            //20211001 myk : PreLotend 조건 추가
                            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                CData.CurCompany == ECompany.SST)
                            {
                                if (CData.Opt.bQcUse)
                                {
                                    if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                                    { CData.bPreLotend = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip; } //koo : Qorvo Lot Rework
                                    else
                                    { CData.bPreLotend = bWorkEnd && bStripNone && bMgzTopCheck && CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip; } //koo : Qorvo Lot Rework
                                }
                            }

                            // 사용을 마친 각종 Flag 초기화
                            ClearQCInterfaceFlag();     // QC와의 Interface 변수들을 초기화 해준다.
                        }//of if (CQcVisionCom.m_nQCSend == CQcVisionCom.eQCSend_End)             // QC에서 전송 완료가 수신되었다.
                        // 2021.12.09 SungTae End
                        //----------------------------------------------------------------------------------------------------------------------//

                        // 전송 취소 처리
                        if (CQcVisionCom.m_nQCSend == CQcVisionCom.eQCSend_Abort)             // QC에서 전송 취소가 수신되었다.
                        {
                            _SetLog($"EXCEPTION ! QC Send AbortTransfer cancel process");

                            // 사용을 마친 각종 Flag 초기화
                            ClearQCInterfaceFlag();     // QC와의 Interface 변수들을 초기화 해준다.
                        }

                        //if (!bBtmRcvAction && !bTopRcvAction && !bWorkEnd)        // Need Action
                        //{
                        //    return  false;
                        //}

                    }//of if (iSeq == ESeq.OFL_BtmRecive || iSeq == ESeq.OFL_TopRecive)

                }//of if (CData.Opt.bQcUse)


                #region QC-Vision 사용 시
                if (CDataOption.UseQC && CData.Opt.bQcUse)
                {
                    //Btm
                    if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                        CData.CurCompany == ECompany.SST)
                    // 2022.03.26 SungTae Start : [수정] QC-Vision Multi-LOT 관련 조건 추가
                    {
                        //MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;//koo : Qorvo Lot Rework //191202 ksg :

                        // Multi-LOT이고, LOT 수량이 1개 이상이라면
                        if (CData.IsMultiLOT() && CData.LotMgr.ListLOTInfo.Count > 0)
                            MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt == CData.LotMgr.ListLOTInfo.ElementAt(iIdx).iTotalStrip;
                        else
                            MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;
                    }
                    // 2022.03.26 SungTae End
                    else
                        MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck;
                           
                    MgzPlace2  = !bWorkEnd && !bStripNone && bMgzBtmCheck  &&  bMgzFullSlot                                            ;
                    MgzPlace3  = !bWorkEnd && !bStripNone && bMgzBtmCheck  && !bMgzFullSlot  && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;

                    //Top
                    if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                        CData.CurCompany == ECompany.SST)
                        TMgzPlace1 =  bWorkEnd &&  bStripNone && !bMgzBtmCheck  && bMgzTopCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;//koo : Qorvo Lot Rework //191202 ksg :
                    else
                        TMgzPlace1 =  bWorkEnd &&  bStripNone && !bMgzBtmCheck  && bMgzTopCheck;

                    TMgzPlace2 = !bWorkEnd && !bStripNone && bMgzTopCheck &&  bTMgzFullSlot                                           ;
                    TMgzPlace3 = !bWorkEnd && !bStripNone && bMgzTopCheck && !bTMgzFullSlot && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;

                    //조건 맞추기
                    bWait      = !bWorkEnd  && (iSeq == ESeq.Idle && (iSeq != ESeq.OFL_BtmRecive) && (iSeq != ESeq.OFL_BtmRecive)) || (iSeq == ESeq.OFL_Wait);
                    bBtmPick   = !bWorkEnd  && (iSeq == ESeq.OFL_BtmPick  ) && (!bStripNone) && (!bMgzBtmCheck); 
                    bBtmPlace  =  MgzPlace1 || MgzPlace2  || MgzPlace3;

                    //20211130 myk : QORVO 관련 조건 추가
                    //if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ)
                    if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) && CData.Opt.bOfLMgzUnloadType)
                        bBtmRecive = !bWorkEnd && (iSeq == ESeq.OFL_BtmRecive) && (!bStripNone) && (bMgzBtmCheck) && (!bMgzFullSlot) && (bBtmRcvAction) && bReadyNextSlot;
                    else
                        bBtmRecive = !bWorkEnd && (iSeq == ESeq.OFL_BtmRecive) && (!bStripNone) && (bMgzBtmCheck) && (!bMgzFullSlot) && (bMgzTopCheck) && (bBtmRcvAction);
                    
                    bTopPick    = !bWorkEnd  && (!bStripNone) && (bMgzBtmCheck) && (!bMgzTopCheck);
                    bTopPlace   = TMgzPlace1 || TMgzPlace2 || TMgzPlace3;
                    bTopRecive  = !bWorkEnd  && (iSeq == ESeq.OFL_TopRecive) && (!bStripNone) && (bMgzTopCheck) && (!bTMgzFullSlot) && (bTopRcvAction);
                }
                #endregion
                #region QC-Vision 미사용 시
                else
                {
                    // QC 미사용
                    bWait      = !bWorkEnd && (iSeq == ESeq.Idle) || (iSeq == ESeq.OFL_Wait);

                    #region MGZ 배출 위치가 BOTTOM인 경우
                    //20200513 jym : 매거진 배출 위치 변경 추가
                    // 배출 위치가 Bottom Magazine인경우
                    if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                    {
                        // 2021-05-31, jhLee, Multi-LOT 처리, 연속LOT 실행의 경우 LOT 종료시 Place를 하도록 한다.
                        if (CData.IsMultiLOT())
                        {
                            // 작업 종료시 Place
                            // OLD MgzPlace1 = bWorkEnd && bUnloadLotFinish && bMgzBtmCheck;     // 작업종료 & 설비내 동일 LOT의 Strip 없음 & 하단 Mgz존재

                            MgzPlace1 = bUnloadLotComplete && bMgzBtmCheck;     // 작업종료 & 설비내 동일 LOT의 Strip 없음 & 하단 Mgz존재

                            // 가득찬 경우 Place
                            // MgzPlace2 = 아직 작업중이고, 설비내 동일 LOT의 Strip이 존재하며, 하단에 Mgz이 존재하고, Magazine이 가득찬 경우
                            //OLD MgzPlace2 = !bWorkEnd && !bUnloadLotFinish && bMgzBtmCheck && bMgzFullSlot;
                            MgzPlace2 = !bUnloadLotComplete && bMgzBtmCheck && bMgzFullSlot;

                            // 매거진 분리 : Qorvo의 경우 1:1 매칭기능, 투입된 MGZ이 다르면 분리시킨다.
                            MgzPlace3 = !bWorkEnd && !bStripNone && bMgzBtmCheck && !bMgzFullSlot && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;

                            bBtmPlace = MgzPlace1 || MgzPlace2 || MgzPlace3;

                            bBtmPick = !bWorkEnd && (iSeq == ESeq.OFL_BtmPick) && (!bStripNone) && (!bMgzBtmCheck); // && (bReadyMidMsg);

                            // 제품을 받을 수 있게 비어있는 Slot으로 이동시켜줘야 할 필요가 있다면 지정 빈 위치로 이동시켜준다.
                            bBtmRecive = (iSeq == ESeq.OFL_BtmRecive) && (bMgzBtmCheck) && (!bMgzFullSlot) && bReadyNextSlot;
                        }
                        else    // Multi-LOT 미사용
                        {
                            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
                            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                CData.CurCompany == ECompany.SST)
                            {
                                // 지정 LOT의 Strip을 모두 처리한 경우 Place 한다.
                                MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;//koo : Qorvo Lot Rework
                            }
                            else
                            {
                                MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck;     // 작업종료, 설비내 Strip 없음, 하단 Mgz존재
                            }


                            // MgzPlace2 = 아직 작업중이고, 설비내 Strip이 존재하며, 하단에 Mgz이 존재하고, Magazine이 가득찬 경우
                            MgzPlace2 = !bWorkEnd && !bStripNone && bMgzBtmCheck && bMgzFullSlot;

                            // 매거진 분리 : Qorvo의 경우 1:1 매칭기능, 투입된 MGZ이 다르면 분리시킨다.
                            MgzPlace3 = !bWorkEnd && !bStripNone && bMgzBtmCheck && !bMgzFullSlot && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;

                            bBtmPick = !bWorkEnd && (iSeq == ESeq.OFL_BtmPick) && (!bStripNone) && (!bMgzBtmCheck); // && (bReadyMidMsg);
                            bBtmPlace = MgzPlace1 || MgzPlace2 || MgzPlace3;

                            bBtmRecive = !bWorkEnd && (iSeq == ESeq.OFL_BtmRecive) && (!bStripNone) && (bMgzBtmCheck) && (!bMgzFullSlot);
                        }//of if Multi-Lot else


                    }
                    #endregion
                    #region MGZ 배출 위치가 TOP인 경우
                    else // 배출 위치가 TOP인 경우
                    {
                        // 2021-05-31, jhLee, Multi-LOT 처리, 연속LOT 실행의 경우 LOT 종료시 Place를 하도록 한다.
                        if (CData.IsMultiLOT())
                        {
                            // 2022.04.29 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 
                            //// 작업 종료시 Place
                            //TMgzPlace1 = bUnloadLotComplete && bMgzTopCheck;     // 작업종료 & 설비내 동일 LOT의 Strip 없음 & 하단 Mgz존재
                            if (CData.CurCompany == ECompany.ASE_K12 && CData.LotMgr.ListLOTInfo.Count > 0)
                                TMgzPlace1 = bWorkEnd && bStripNone && bMgzTopCheck && CData.LotInfo.iTOutCnt == CData.LotMgr.ListLOTInfo.ElementAt(iIdx).iTotalStrip;
                            else
                                TMgzPlace1 = bWorkEnd && bStripNone && bMgzTopCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;
                            // 2022.04.29 SungTae End

                            // 가득찬 경우 Place
                            // MgzPlace2 = 아직 작업중이고, 설비내 동일 LOT의 Strip이 존재하며, 하단에 Mgz이 존재하고, Magazine이 가득찬 경우
                            TMgzPlace2 = !bUnloadLotComplete && bMgzTopCheck && bTMgzFullSlot;
                            TMgzPlace3 = !bWorkEnd && !bStripNone && bMgzTopCheck && !bTMgzFullSlot && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;

                            // 2022.05.09 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 
                            if (CData.CurCompany == ECompany.ASE_K12 && CData.LotMgr.IsUnloaderTrackOutFlag == eTrackOutFlag.Error)
                                bTopPick = !bWorkEnd && (iSeq == ESeq.OFL_TopPick) && (!bStripNone) && (bMgzTopCheck);
                            else
                                bTopPick = !bWorkEnd && (iSeq == ESeq.OFL_TopPick) && (!bStripNone) /*20200513 jhc //&& (bMgzBtmCheck)*/ && (!bMgzTopCheck);
                            // 2022.05.09 SungTae End

                            bTopPlace = TMgzPlace1 || TMgzPlace2 || TMgzPlace3;

                            // 제품을 받을 수 있게 비어있는 Slot으로 이동시켜줘야 할 필요가 있다면 지정 빈 위치로 이동시켜준다.
                            bTopRecive = (iSeq == ESeq.OFL_TopRecive) && (bMgzTopCheck) && (!bTMgzFullSlot) && bReadyNextSlot;
                        }
                        else
                        {
                            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                                CData.CurCompany == ECompany.SST)
                                TMgzPlace1 = bWorkEnd && bStripNone /*20200513 jhc //&& !bMgzBtmCheck */ && bMgzTopCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;//koo : Qorvo Lot Rework //191202 ksg :
                            else
                                TMgzPlace1 = bWorkEnd && bStripNone /*20200513 jhc //&& !bMgzBtmCheck*/ && bMgzTopCheck;

                            TMgzPlace2 = !bWorkEnd && !bStripNone && bMgzTopCheck && bTMgzFullSlot;
                            TMgzPlace3 = !bWorkEnd && !bStripNone && bMgzTopCheck && !bTMgzFullSlot && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;

                            bTopPick = !bWorkEnd && (iSeq == ESeq.OFL_TopPick) && (!bStripNone) /*20200513 jhc //&& (bMgzBtmCheck)*/ && (!bMgzTopCheck);
                            bTopPlace = TMgzPlace1 || TMgzPlace2 || TMgzPlace3;

                            bTopRecive = !bWorkEnd && (iSeq == ESeq.OFL_TopRecive) && (!bStripNone) && (bMgzTopCheck) && (!bTMgzFullSlot);
                        }

                    }
                    #endregion
                }
                #endregion

                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                    CData.CurCompany == ECompany.SST)
                {
                    //20200513 jhc : 매거진 배출 위치 변경 추가
                    if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                    { CData.bPreLotend = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip; } //koo : Qorvo Lot Rework
                    else
                    { CData.bPreLotend = bWorkEnd && bStripNone && bMgzTopCheck && CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip; } //koo : Qorvo Lot Rework}
                }
                
                if(iSeq == ESeq.OFL_WorkEnd) 
                {
                    return true; 
                }


                if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                {
                    if (!bBtmRecive && (iSeq == ESeq.OFL_BtmRecive) && !bBtmPlace)
                    {
                        // CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;
                        return false; 
                    }

                    //20200513 jym : 매거진 배출 위치 변경 추가
                    if (!bTopRecive && (iSeq == ESeq.OFL_TopRecive) && !bTopPlace)
                    {
                        // CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;
                        return false;
                    }
                }

                if (bWait)
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_Wait;
                    _SetLog(">>>>> Wait start.");
                }
                else if (bBtmPick)
                {                    
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_BtmPick;
                    _SetLog(">>>>> Bottom pick start.");
                }
                else if (bBtmPlace)
                {                    
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_BtmPlace;
                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPlace;
                    _SetLog(string.Format(">>>>> Bottom place start.  Place1 : {0}  Place2 : {1}  Place3 : {2}", MgzPlace1, MgzPlace2, MgzPlace3));
                }
                else if (bBtmRecive)
                {                    
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_BtmRecive;
                    _SetLog(">>>>> Bottom recive start.");
                }
                else if (bTopPick)
                {                    
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_TopPick;
                    _SetLog(">>>>> Top pick start.");
                }
                else if (bTopPlace)
                {                    
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_TopPlace;
                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_TopPlace;
                    
                    // 2022.05.09 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 
                    if (CData.CurCompany == ECompany.ASE_K12 && CData.LotMgr.IsUnloaderTrackOutFlag == eTrackOutFlag.Error)
                        _SetLog(string.Format(">>>>> [TRACK-OUT] Top place start.  Place1 : {0}  Place2 : {1}  Place3 : {2}", TMgzPlace1, TMgzPlace2, TMgzPlace3));
                    else
                        _SetLog(string.Format(">>>>> Top place start.  Place1 : {0}  Place2 : {1}  Place3 : {2}", TMgzPlace1, TMgzPlace2, TMgzPlace3));
                    // 2022.05.09 SungTae End
                }
                else if (bTopRecive)
                {                    
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_TopRecive;
                    _SetLog(">>>>> Top recive start.");
                }
                else if (bWorkEnd)
                {
                    if(CDataOption.UseQC && CData.Opt.bQcUse)
                    {
                        if(bWorkEnd && !bMgzBtmCheck && !bMgzTopCheck)
                        {
                            m_GetQcWorkEnd = false;
                            
                            m_iStep = 0;
                            m_iPreStep = 0;
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_WorkEnd;
                            _SetLog(">>>>> QC use, Work End. -> "+ bWorkEnd + "," + bMgzBtmCheck + "," +bMgzTopCheck);
                        }
                    }
                    else
                    {
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                        {
                            if (bWorkEnd && !bMgzBtmCheck)
                            {
                                m_iStep = 0;
                                m_iPreStep = 0;
                                CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_WorkEnd;
                                _SetLog(">>>>> QC skip, Work End(bottom).");
                            }
                        }
                        else
                        {
                            if (bWorkEnd && !bMgzTopCheck)
                            {
                                m_iStep = 0;
                                m_iPreStep = 0;
                                CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_WorkEnd;
                                _SetLog(">>>>> QC skip, Work End(top).");
                            }
                        }
                        //
                    }
                }
            }

            switch (SEQ)
            {
                default:
                    return false;

                case ESeq.OFL_Wait:
                    {
                        if (Cyl_Wait())
                        {                            
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Wait end.");
                        }

                        return false;
                    }

                case ESeq.OFL_BtmPick:
                    {
                        //200120 ksg :
                        //if(CData.CurCompany != eCompany.JSCK)
                        if(CDataOption.OflRfid == eOflRFID.NotUse)
                        {
                            if (Cyl_BtmPick())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< RFID not use.  Bottom pick end.");
                            }
                        }
                        else
                        {
                            if(CData.Opt.bOflRfidSkip)
                            {
                                if (Cyl_BtmPick())
                                {                                   
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< RFID skip.  Bottom pick end.");
                                }
                            }
                            else
                            {
                                if (Cyl_BtmPick_Rfid())
                                {                                    
                                    SEQ = ESeq.Idle;
                                    _SetLog("<<<<< RFID use.  Bottom pick end.");
                                }
                            }
                        }

                        return false;
                    }

                case ESeq.OFL_BtmPlace:
                    {
                        /*
                        if((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))  
                        {
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPick;
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR);
                            SEQ = ESeq.Idle;
                            m_iStep = 0;

                            return true;
                        }
                        */
                        if (Cyl_BtmPlace())
                        {                            
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Bottom place end.");
                        }

                        return false;
                    }

                case ESeq.OFL_BtmRecive:
                    {
                        if ((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        {                            
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPick;
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR);
                            _SetLog("Error : Not detected bottom magazine");
                            SEQ = ESeq.Idle;

                            m_iStep = 0;
                            return true;
                        }

                        if (Cyl_BtmRecive())
                        {                            
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Bottom recive end.");
                        }

                        return false;
                    }

                case ESeq.OFL_TopPick:
                    {
#if true //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드, SPIL)
                        bool bResult = false;

                        //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
                        if (CDataOption.OflRfid == eOflRFID.Use && CDataOption.RFID == ERfidType.Keyence && CData.Opt.eEmitMgz == EMgzWay.Top)
                        //if (CDataOption.OflRfid == eOflRFID.Use && CData.Opt.eEmitMgz == EMgzWay.Top && CData.CurCompany == ECompany.SPIL)
                        {
                            if (CData.Opt.bOflRfidSkip)
                            { bResult = Cyl_TopPick(); }
                            else
                            { bResult = Cyl_TopPick_Bcr(); }
                        }
                        else
                        { bResult = Cyl_TopPick(); }

                        if (bResult)
                        {
                            _SetLog("<<<<< Top pick end.");
                            SEQ = ESeq.Idle;
                        }
#else
                        if (Cyl_TopPick())
                        {
                            _SetLog("Top pick end");
                            SEQ = ESeq.Idle;
                        }
#endif
                        return false;
                    }

                case ESeq.OFL_TopPlace:
                    {
                        if (Cyl_TopPlace())
                        {
                            // 2022.04.14 SungTae SungTae : [추가] 
                            if (CData.CurCompany == ECompany.ASE_K12 && CData.IsMultiLOT())
                            {
                                // Error로 인한 MGZ Place 라면
                                if (CData.LotMgr.IsUnloaderTrackOutFlag == eTrackOutFlag.Error)
                                    CData.LotMgr.IsUnloaderTrackOutFlag = eTrackOutFlag.Valid;
                            }
                            // 2022.04.14 SungTae 

                            _SetLog("<<<<< Top place end.");
                            SEQ = ESeq.Idle;
                        }

                        return false;
                    }

                case ESeq.OFL_TopRecive:
                    {
                        if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        {
                            _SetLog("Error : Not detected top magazine");
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_TopPick;
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_TOP_MGZ_ERROR);
                            SEQ = ESeq.Idle;
                            m_iStep = 0;

                            return true;
                        }
                        if (Cyl_TopRecive())
                        {
                            _SetLog("<<<<< Top recive end.");
                            SEQ = ESeq.Idle;
                        }

                        return false;
                    }

                // 2022.04.19 SungTae Start : [추가] ASE-KH On-loader MGZ ID Reading 관련 추가
                case ESeq.OFL_CheckBcr:
                    {
                        if (Cyl_CheckBcr())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Off-loader MGZ ID Reading Finish.");
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

            CLog.Save_Log(eLog.OFL, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
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

            CLog.Save_Log(eLog.OFL, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }

        public void Init_Cyl()
        {
            m_iStep = 10;
            m_iPreStep = 0;
        }

        #region Cycle
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
                    _SetLog("Error : Time out");

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
                        CMot.It.Set_SOn(m_iY, bVal);
                        CMot.It.Set_SOn(m_iZ, bVal);
                        _SetLog("All servo-on : " + bVal);

                        m_iStep++;
                        return false;
                    }

                case 11:    // 2. 서보 온 상태 체크
                    {
                        if (CMot.It.Chk_Srv(m_iX) == bVal &&
                            CMot.It.Chk_Srv(m_iY) == bVal &&
                            CMot.It.Chk_Srv(m_iZ) == bVal)
                        {
                            _SetLog("All servo check.");

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

        /// <summary>
        /// Sequence Cycle : Home
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
                    CErr.Show(eErr.OFFLOADER_HOME_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            // Light curtain 상시 체크
            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

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
                        ClearQCInterfaceFlag();     // QC와의 Interface 변수들을 초기화 해준다.

                        m_bHD = false;
                        if (Chk_Axes(false))
                        {
                            m_iStep = 0;
                            return true;
                        }

                        if (m_bxChkLight) 
                        { return false; }

                        //200625 jhc : Homing 전 자재 감지 확인
                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                _SetLog("Error : Strip-out detect.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        // 빈매거진 시 클램프 다운 신호 안들어옴
                        Func_TopClamp();
                        Func_BtmClamp();
                        if (!Func_TopBeltStop())
                        { return false; }
                        if (!Func_MidBeltStop())
                        { return false; }
                        if (!Func_BtmBeltStop())
                        { return false; }
                        _SetLog("Init IO.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        CMot.It.Mv_H(m_iY);
                        _SetLog("Y axis homing.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_HD(m_iY)) 
                        { return false; }

                        CMot.It.Mv_H(m_iZ);
                        CMot.It.Mv_H(m_iX);
                        _SetLog("X and Z axis homing.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_HD(m_iZ)) 
                        { return false; }
                        if (!CMot.It.Get_HD(m_iX)) 
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev .dOffL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_Wait );
                        _SetLog("Z axis move wait.", CData.SPos.dOFL_Z_Wait);
                        // X 대기 위치로 이동(중국 직원 요청)
                        CMot.It.Mv_N(m_iX, CData.Dev .dOffL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOffL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait)) 
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_Wait)) 
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn)) 
                        { return false; }

                        m_bHD = true;
                        _SetLog("Finish.");


                        // QC와의 Interface 변수 초기화
                        CQcVisionCom.m_nReadyReqMgz = CQcVisionCom.eMGZ_NONE;           // QC가 요청한 Magazine 종료 
                        CQcVisionCom.m_nMgzReady = CQcVisionCom.eMGZ_NONE;              // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료
                        CQcVisionCom.m_nQCSend = CQcVisionCom.eQCSend_None;             // QC가 전송하는 과정 및 결과

                        bReadyNextSlot = false;      // 2021-06-07, jhLee : 이동을 마친 뒤 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는 Flag 초기화

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - OFFL_Wait
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Wait()
        {
            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {                    
                    CErr.Show(eErr.OFFLOADER_WAIT_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            //200625 jhc : Wait Pos. 이동 전 자재 감지 확인
            if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
            {
                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                    _SetLog("Error : Strip-out detect.");

                    m_iStep = 0;
                    return true;
                }
            }
            else
                ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.

            //

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:
                    {
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        if (m_bxChkLight)
                        { return false; }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        Belt_Top(m_BeltStop);
                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        Belt_Btm(m_BeltStop);
                        _SetLog("Check axes.  Top, Middel, Bottom belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOffL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFL_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_Wait))
                        { return false; }

                        //20200513 jym : 매거진 배출 위치 변경 추가
                        if ((CData.Opt.eEmitMgz == EMgzWay.Top) ? Get_TopMgzChk() : Get_BtmMgzChk())
                        {
                            if (!GetFullSlotCheck() && CData.Parts[(int)EPart.ONL].iStat != ESeq.ONL_WorkEnd)
                            {
                                CData.Parts[(int)EPart.OFL].iStat = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopRecive : ESeq.OFL_BtmRecive;
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFL].iStat = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopPlace : ESeq.OFL_BtmPlace;
                            }
                        }
                        else
                        {
                            CData.Parts[(int)EPart.OFL].iStat = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopPick : ESeq.OFL_BtmPick;
                        }

                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFL].iStat);

                        bReadyNextSlot = true;          // 다음에 Magazine이 지정 위치로 이동시 Strip 배출 위치로 이동 해야 한다.

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - OFFL_BtmPick
        /// Mgz Mid Conveyor
        /// </summary>
        /// <returns></returns>
        public bool Cyl_BtmPick()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADER_BOTTOM_PICK_TIMEOUT);
                    m_iStep = 0;

                    return true;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            m_iPreStep = m_iStep;

            //200625 jhc : Wait Pos. 이동 전 자재 감지 확인
            if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
            {
                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                    _SetLog("Error : Strip-out detect.");

                    m_iStep = 0;
                    return true;
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

                case 10:
                    {
                        m_StartSeq = DateTime.Now; //190807 ksg :
                        ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.

                        //190630 ksg :
                        if ((CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        {
                            CErr.Show(eErr.OFFLOADER_ALREADY_BOTTOM_MGZ_ERROR);
                            _SetLog("Error : Already bottom magazine.");

                            m_iStep = 0;
                            return true;
                        }

                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                _SetLog("Error : DRY X axis not wait.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        //

                        if (m_bxChkLight) return false;

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        Act_BtmClampOC(m_ActOpen);
                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        _SetLog("Bottom clamp open.  Middle belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_BtmClampOC(m_ActOpen))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActDn);
                        Act_TopClampModuleDU(m_ActUp);
                        _SetLog("Bottom clamp down.  Top clamp up.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActDn))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOffL_X_Algn);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn))
                        { return false; }                           

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickGo);
                        _SetLog("Z axis move pick.", CData.SPos.dOFL_Z_BPickGo);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);
                        _SetLog("Y axis move pick.", CData.SPos.dOFL_Y_Pick);

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        { return false; }

                        if(m_bxChkLight) return false;
                        
                        Belt_Mid(m_BeltRun, m_BeltCCw);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);
                        _SetLog("Middle belt run.  Set delay : " + GV.ONL_DETECT_MGZ_DELAY + "ms");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);

                        if (!Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        if (Get_BtmMgzChk())
                        {
                            _SetLog("Detect bottom magazine.");

                            m_iStep = 20;
                            return false;
                        }

                        _SetLog("Middle belt stop.");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        m_Delay.Set_Delay(100);
                        _SetLog("Middle belt run(CW).  Set delay : 100ms");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);
                        _SetLog("Error : Middle belt not detect magazine.");

                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BClamp);
                        _SetLog("Z axis move clamp.", CData.SPos.dOFL_Z_BClamp);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BClamp))
                        { return false; }

                        Act_BtmClampOC(m_ActClose);
                        m_Delay.Set_Delay(10000);
                        _SetLog("Bottom clamp close.  Set delay : 10000ms");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        //190514 ksg :
                        if(m_Delay.Chk_Delay() && !Act_BtmClampOC(m_ActClose))
                        {
                            CErr.Show(eErr.OFFLOADER_BOTTOM_CLAMP_NOT_CLOSE);
                            _SetLog("Error : Bottom clamp not close.");
                              
                            m_iStep = 0;
                            return true;                     
                        }
                        if (!Act_BtmClampOC(m_ActClose))
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickUp);
                        _SetLog("Z axis move pick.", CData.SPos.dOFL_Z_BPickUp);

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickUp))
                        { return false; }

                        if(m_bxChkLight) return false;

                        //20191121 ghk_display_strip
                        CData.Parts[(int)EPart.OFL].bExistStrip = true;

                        //Mgz 상태 변환
                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                        SetAllStatusMgz(eSlotInfo.Empty);

                        bReadyNextSlot = true;      // 2021-06-07, jhLee : 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는가 ?

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if(CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        else                           CData.Parts[(int)EPart.OFL].iSlot_No = 0;
						
                        Belt_Mid(m_BeltStop, m_BeltCw);
                        _SetLog("Middle belt stop.  Status : " + CData.Parts[(int)EPart.OFL].iStat);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        if ((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        {
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR);
                            _SetLog("Error : Not detected bottom magazine.");

                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))  { return false; }

                        m_EndSeq  = DateTime.Now         ; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                        _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }
        }
        /// <summary>
        /// JSCK 전용
        /// Sequence Cycle : ESeq_Stat - OFFL_BtmPick + RFID 
        /// Mgz Mid Conveyor
        /// </summary>
        /// <returns></returns>
        public bool Cyl_BtmPick_Rfid()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADER_BOTTOM_PICK_TIMEOUT);
                    m_iStep = 0;

                    return true;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_BtmPick";

            //200625 jhc : Wait Pos. 이동 전 자재 감지 확인
            if ( (CDataOption.UseQC && CData.Opt.bQcUse) == false)
            {
                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                    _SetLog("Error : Strip-out detect.");

                    m_iStep = 0;
                    return true;
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

                case 10:
                    {
                        m_StartSeq = DateTime.Now; //190807 ksg :

                        if ( (CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                        }
                        else
                            ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.

                        //


                        if (m_bxChkLight) return false;

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        Act_BtmClampOC(m_ActOpen);
                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Act_BtmClampOC(m_ActOpen), Belt_Mid(m_BeltStop, m_BeltCCw)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_BtmClampOC(m_ActOpen))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActDn);
                        Act_TopClampModuleDU(m_ActUp);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Act_BtmClampModuleDU(m_ActDn), Act_TopClampModuleDU(m_ActUp)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActDn))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iX.ToString();
                        m_LogVal.dPos1  = CData.Dev.dOffL_X_Algn;
                        SaveLog();

                        m_iStep++;

                        return false;


                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn)) return false;

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickGo);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Z_BPickGo;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Y_Pick;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        { return false; }

                        if(m_bxChkLight) return false;
                        
                        Belt_Mid(m_BeltRun, m_BeltCCw);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Mid(m_BeltRun, m_BeltCCw), Set_Delay(GV.ONL_DETECT_MGZ_DELAY)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);

                        if (!Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        if (Get_BtmMgzChk())
                        {

                            m_LogVal.iStep  = m_iStep;
                            m_LogVal.sMsg   = "Belt_Mid(m_BeltRun, m_BeltCCw), Belt_Mid(m_BeltStop, m_BeltCCw)";
                            SaveLog();

                            m_iStep = 20;
                            return false;
                        }

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Mid(m_BeltRun, m_BeltCCw), Belt_Mid(m_BeltStop, m_BeltCCw)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        m_Delay.Set_Delay(100);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Mid(m_BeltRun, m_BeltCw), Set_Delay(100)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);

                        m_LogVal.iStep  = m_iStep;
                        SaveLog();

                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BClamp);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Z_BClamp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BClamp))
                        { return false; }

                        Act_BtmClampOC(m_ActClose);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Act_BtmClampOC(m_ActClose)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if (!Act_BtmClampOC(m_ActClose))
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickUp);

                        m_iReReadCnt = 0; //190418 ksg : 
                        
                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Z_BPickUp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickUp))
                        { return false; }

                        //190418 ksg : 
                        if(m_iReReadCnt < 5)
                        {
                            CRfid.It.ReciveMsg = "";
                            CRfid.It.ReadRfid(true);
                            m_Delay.Set_Delay(500);
                            m_iReReadCnt++;
                            m_iStep++;
                        }
                        else
                        {
                            m_iReReadCnt = 0;
                            CErr.Show(eErr.OFFLOADER_CAN_NOT_READ_RFID);
                            Belt_Mid(m_BeltStop, m_BeltCw); //200120 ksg : 추가
                            m_iStep = 0;
                            return true;
                        }
                        return false;

                    }

                case 24:
                    {
                        if(!m_Delay.Chk_Delay()) return false;
                        if(CRfid.It.ReciveMsg == "")
                        {
                            m_iStep = 23;
                            return false;
                        }
                        
                        CData.Parts[(int)EPart.OFL].sMGZ_ID = CRfid.It.ReciveMsg;
                        CData.LotInfo.sGMgzId = CRfid.It.ReciveMsg;

                        //191118 ksg :
                        if(CData.GemForm != null)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID = CRfid.It.ReciveMsg; 
                            
                            CData.GemForm.UnloaderMgzVerifyRequest(CData.Parts[(int)EPart.OFL].sMGZ_ID, (uint)SECSGEM.JSCK.eMGZPort.GOOD);

                            // 2022.03.31 SungTae Start : [추가] Log 추가
                            _SetLog($"[UNLOADER][SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request).  MgzID : {CData.Parts[(int)EPart.OFL].sMGZ_ID}, Mgz Port : {(uint)SECSGEM.JSCK.eMGZPort.GOOD}.");
                            // 2022.03.31 SungTae End
                        }
                        m_Delay.Set_Delay(60000);

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {
                        if(CData.Opt.bSecsUse) //191125 ksg :
                        {
                            if (m_Delay.Chk_Delay())
                            {
                                CErr.Show(eErr.HOST_ULD_MGZ_VERIFY_TIME_OVER_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            else if (CData.SecsGem_Data.nMGZ_Check != Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Complete)/*2*/)
                            {
                                if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Error)/*-1*/)
                                {
                                    CErr.Show(eErr.HOST_ULD_MGZ_VERIFY_TIME_OVER_ERROR);
                                    m_iStep = 0;
                                    return true;
                                }
                                else if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Run)/*1*/) return false;
                            } 
                        }
                        if(m_bxChkLight) return false;

                        //20191121 ghk_display_strip
                        CData.Parts[(int)EPart.OFL].bExistStrip = true;
                        //

                        //Mgz 상태 변환
                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                        SetAllStatusMgz(eSlotInfo.Empty);

                        bReadyNextSlot = true;      // 2021-06-07, jhLee : 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는가 ?

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        else                            CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        Belt_Mid(m_BeltStop, m_BeltCw);

                        //191118 ksg :
                        if(CData.GemForm != null)	CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Cnt++; 

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Mid(m_BeltStop, m_BeltCw)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        m_EndSeq  = DateTime.Now         ; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg  = "\r\n Time_Ofl Btm Pick : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                        SaveLog();

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - OFFL_BtmPlace
        /// </summary>
        /// <returns></returns>
        public bool Cyl_BtmPlace()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADER_PLACE_TIMEOUT);
                    m_iStep = 0;

                    return false;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_BtmPlace";

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

                        ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.

                        if (m_bxChkLight) return false;

                        if ((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        {
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPick;
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR);
                            SEQ = ESeq.Idle;
                            m_iStep = 0;

                            return true;
                        }
                        //190724 ksg : MGZ 배출 시 자재 배출 센서에 감지시 Error 발생 추가
                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                _SetLog("Error : Strip-out detect.");

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (!(CDataOption.UseQC && CData.Opt.bQcUse) && CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            //
                        }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        Act_BtmClampOC(m_ActClose);
                        Belt_Btm(m_BeltStop);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Act_BtmClampOC(m_ActClose), Belt_Btm(m_BeltStop)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_BtmClampOC(m_ActClose))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActDn);
                        Act_TopClampModuleDU(m_ActUp);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Act_BtmClampModuleDU(m_ActDn), Act_TopClampModuleDU(m_ActUp)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActDn)) { return false; }
                        if (!Act_TopClampModuleDU(m_ActUp)) { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))  { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPlaceGo);

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetectFull))
                        {
                            //set Error
                            CErr.Show(eErr.OFFLOADER_BTM_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        if(m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))
                        {
                            Belt_Btm(m_BeltRun);
                        }

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Btm(m_BeltRun)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if(m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))    { Belt_Btm(m_BeltRun); }
                        else                                        { Belt_Btm(m_BeltStop); }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPlaceGo))  { return false; }

                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if(m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))    { Belt_Btm(m_BeltRun); }
                        else                                        { Belt_Btm(m_BeltStop); }

                        if (Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Btm(m_BeltStop);

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect) || !CIO.It.Get_X(eX.OFFL_BtmMGZDetectFull))
                        {
                            //set Error : Full Mgz
                            CErr.Show(eErr.OFFLOADER_BTM_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        m_LogVal.iStep  = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Place);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Y_Place;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Place)) { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BUnClamp);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Z_BUnClamp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BUnClamp))  { return false; }

                        Act_BtmClampOC(m_ActOpen);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Act_BtmClampOC(m_ActOpen)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!Act_BtmClampOC(m_ActOpen)) { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPlaceDn);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1  = CData.SPos.dOFL_Z_BPlaceDn;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPlaceDn))  { return false; }

                        //200123 ksg :
                        if(CDataOption.OflRfid == eOflRFID.Use)
                        {
                            CData.Parts[(int)EPart.OFL].sMGZ_ID = "";
                        }

                        if(m_bxChkLight) return false;

                        Belt_Btm(m_BeltRun);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        //Data 이동 및 상태 변환
                        //CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;
                        SetAllStatusMgz(eSlotInfo.Empty);

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        else                            CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        
                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.Dev.dOffL_Y_Wait;
                        m_LogVal.sMsg   = "CData.Parts[(int)EPart.OFL].iStat = " + CData.Parts[(int)EPart.OFL].iStat.ToString() + ", m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect) && !m_Delay.Chk_Delay())
                        { return false; }

                        //m_Delay.Set_Delay(); //옵션 Delay 값 넣기
                        m_LogVal.iStep  = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Btm(m_BeltRun);

                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Btm(m_BeltStop);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Btm(m_BeltStop)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        //20191121 ghk_display_strip
                        CData.Parts[(int)EPart.OFL].bExistStrip = false;
                        //

                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;

                        CData.sLastLot = CData.Parts[(int)EPart.OFL].sLotName;
                        CData.iLastMGZ = CData.Parts[(int)EPart.OFL].iMGZ_No;

                        m_EndSeq  = DateTime.Now         ; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sMsg   = "Belt_Btm(m_BeltStop)";
                        m_LogVal.sMsg  += "\r\n Time_Ofl Place : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                        SaveLog();

                        // 2021-05-31, jhLee, Multi-LOT 처리, 연속LOT 실행의 경우  Magazine Place의 경우 LOT 완료를 처리 하도록 한다.
                        if (CData.IsMultiLOT())
                        {
                            string sLotName = CData.LotMgr.UnloadingLotName;            // Unloading 중인 LOT의 이름

                            // Multi-LOT 환경에서 LOT이 종료되었는지 check   
                            if (CData.LotMgr.IsLotComplete(sLotName))
                            {
                                // 더이상 배출중인 LOT의 잔여 Strip이 없을경우 해당 LOT을 종료 시킨다.
                                CData.LotMgr.ClearUnloadingLot();                   // Unloading 중인 LOT을 초기화 해준다.
                                CData.bLotCompleteMsgShow = true;                   // LOT 종료 메세지를 보여주도록 한다.

                                CData.Parts[(int)EPart.OFL].iMGZ_No = 0;        // 배출된 Magazine 번호 초기화

                                _SetLog($"Multi-LOT: (BTM) Finish Unloading LOT, {sLotName}");
                            }
                            else
                            {
                                _SetLog($"Multi-LOT: (BTM) Continue Unloading LOT, {sLotName}");
                            }
                        }

                        CSQ_Man.It.bOffMGZBtmPlaceDone = true;

                        //20190624 josh
                        //jsck secsgem
                        //ceid 999403 mgz ended
                        if (CData.GemForm != null) CData.GemForm.OnMagazineEnd((uint)SECSGEM.JSCK.eMGZPort.GOOD);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Sequence Cycle : ESeq_Stat - OFFL_BtmRecive
        /// </summary>
        /// <returns></returns>
        public bool Cyl_BtmRecive()
        {
            //20191121 ghk_display_strip
            if (!Get_BtmMgzChk())
            {//매거진 항상 있어야 함.
                CErr.Show(eErr.OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR);
                m_iStep = 0;

                return true;
            }
            //

            //200625 jhc : Clamp 모듈, Y축, Z축 움직임 전 스트립 걸림 재확인
            if (!(CDataOption.UseQC && CData.Opt.bQcUse))
            {
                if ((11 <= m_iStep) && (m_iStep <= 15))
                {
                    if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                    {
                        CErr.Show(eErr.DRY_DETECTED_STRIP);
                        m_iStep = 0;
                        return true;
                    }
                }
            }
            //

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_BtmRecive";

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
                        m_tmSafety.Set_Delay(1);

                        // QC Vision을 사용하지 않는다면 Dry Zone에서 넘어오는 제품과의 간섭 check 필요
                        if (!(CDataOption.UseQC && CData.Opt.bQcUse))
                        {
                            //191108 ksg :
                            if (CSq_Dry.It.m_DuringDryOut)
                            {
                                _SetLog("Check error : m_DuringDryOut");
                                m_iStep = 0;
                                return true;
                            }

                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                _SetLog("Check error : DRY_StripOutDetect");
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End
                        }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if ( ++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety


                        Act_BtmClampOC(m_ActClose);

                        _SetLog("Check Safety & Bottom Clamp On");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_BtmClampOC(m_ActClose))    { return false; }

                        Act_BtmClampModuleDU(m_ActDn);

                        _SetLog("Bottom clamp down");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActDn)) { return false; }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        // Y축 이동
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1  = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        if (GetFullSlotCheck())
                        {
                            _SetLog("Bottom magazine slot full, Status:OFL_BtmPlace");
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPlace;
                            m_iStep = 0;

                            return true;
                        }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        m_dMovePos = GetBtmWorkPos();           // 이동해야 할 Slot 위치 계산
                        CMot.It.Mv_N(m_iZ, m_dMovePos);         // 지정 높이로 이동 지령

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = m_dMovePos; 
                        SaveLog();

                        _SetLog($"Z Axis move to {m_dMovePos:F3}");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, m_dMovePos))          // 지정 위치가 아니라면
                        {
                            if (CMot.It.Get_Stop(m_iZ))                 // 만약 축이 멈춰있다면 
                            {
                                CMot.It.Mv_N(m_iZ, m_dMovePos);         // 다시 이동 명령을 내린다.
                            }

                            return false;
                        }

                        m_LogVal.iStep  = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = m_dMovePos;            //  GetBtmWorkPos();
                        SaveLog();

                        _SetLog("Bottom Mgz move empty slot position end");

                        m_iStep++;
                        return false;
                    }


               case 15:
                    {
                        if (CDataOption.UseQC && CData.Opt.bQcUse )
                        {
                            // 2021.12.09 SungTae Start : [수정] (Qorvo향 VOC)
                            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                            {
                                // 2022.01.11 SungTae Start : [수정] QC Vision 관련 ByPass에 대한 조건 추가
                                // 판정에 상관없이 BTM Mgz으로 받을 때 NG에 대한 처리 조건 추가
                                //if (CData.Opt.bOfLMgzUnloadType)
                                if (CData.Opt.bOfLMgzUnloadType || CData.Opt.bQcByPass)
                                {
                                    CQcVisionCom.m_nMgzReady = CQcVisionCom.m_nReadyReqMgz;
                                }
                                else
                                {
                                    if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_GOOD)
                                    {
                                        CQcVisionCom.m_nMgzReady = CQcVisionCom.eMGZ_GOOD;          // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료
                                    }
                                }
                            }
                            else
                            {
                                if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_GOOD)
                                {
                                    CQcVisionCom.m_nMgzReady = CQcVisionCom.eMGZ_GOOD;         // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료

                                    CGxSocket.It.SendMessage("QCSendRequest");            // QC Vision에게 Strip send request

                                    _SetLog("[SEND](EQ->QC) QCSendRequest");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                }
                            }
                            // 2021.12.09 SungTae End
                        }

                        bReadyNextSlot = false;      // 2021-06-07, jhLee : 이동을 마친 뒤 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는 Flag 초기화

                        m_EndSeq  = DateTime.Now; 
                        m_SeqTime = m_EndSeq - m_StartSeq;

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg  = "m_bDuringGOut = " + m_bDuringGOut.ToString();
                        m_LogVal.sMsg += "CData.Opt.bQcUse = " + CData.Opt.bQcUse.ToString();
                        m_LogVal.sMsg += "\r\n Time_Ofl Btm Recive : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                        SaveLog();

                        _SetLog($"Cyl_BtmRecive() Finish. QC Result : {CQcVisionCom.m_nReadyReqMgz}, Mgz Ready : {CQcVisionCom.m_nMgzReady}");
                        
                        m_iStep = 0;            // 지정 위치로 이동 완료
                        return true;
                    }



                    //// QC Emit command Retry 지점
                    //case 16: // QC Vision에게 배출 요청 전송 : Emit
                    //    {
                    //        if(!m_TSendDelay.Chk_Delay()) return false;

                    //        //m_bDuringGOut = false;
                    //        //CTcpIp.It.bSuccess_Ok   = false;            // 배출 성공 Flag는 초기화
                    //        //CTcpIp.It.bSuccess_Fail = false;
                    //        //CTcpIp.It.bSuccess_Du = false;              // 배출 수행중 응답
                    //        //CTcpIp.It.bEmit_Fail = false;             // 배출 요청에 대한 응답 flag 초기화
                    //        //CTcpIp.It.bEmit_Ok = false;
                    //        //CTcpIp.It.SendEmit();                       // QC에게 배출 요청을 한다.

                    //        CGxSocket.It.SendMessage("QCSendRequest");

                    //        m_Delay.Set_Delay(30000);               //  수신 Timeout

                    //        _SetLog("Send QCSend-Request -> QC ");

                    //        m_iStep++;
                    //        return false;
                    //    }


                    //case 17:  // QC로부터 전송시작을 알리는 SendOut 명령을 기다린다.  
                    //    {
                    //        // SendOut시의 GOOD이나 NG는 무시된다.
                    //        if ( CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG )    // SendOut 수신완료
                    //        {
                    //            if (CQcVisionCom.rcvQCSendOutGOOD)
                    //            {
                    //                _SetLog("Recv QCSendOut GOOD from QC");
                    //            }
                    //            else
                    //                _SetLog("Recv QCSendOut NG from QC, Ignored NG message");

                    //            // SendEnd를 기다리는 곳으로 간다.
                    //            m_iStep = 18;           // 전송완료를 기다리는 곳으로 간다.
                    //            return false;
                    //        }

                    //        // 만약에 불량품에대한 준비가 완료되었는지 문의가 들어오면 지금 루틴을 끝낸다.
                    //        // 시퀀스시점에서는 모순이지만 대비한다.
                    //        if ( CQcVisionCom.rcvQCReadyQueryNG )
                    //        {
                    //            _SetLog("Recv QCSReadyQuery NG from QC, Exception process");
                    //            m_bDuringGOut = false;           // 더이상 양품으로 제품을 받는 과정이 아니다

                    //            m_iStep = 0;
                    //            return true;            // Cyl_BtmRecive를 끝낸다.
                    //        }

                    //        return false;           // 계속 반복한다.



                    //        //if (!CQcVisionCom.rcvQCSendOutGOOD && !CQcVisionCom.rcvQCSendOutNG)            // SendOut을 받지 못하였다면
                    //        //{
                    //        //    if (!m_Delay.Chk_Delay())                                // 30초 이내라면 다시 전송한다.
                    //        //    {
                    //        //        return false;
                    //        //    }
                    //        //    else // 60초 Timeout
                    //        //    {
                    //        //        m_bDuringGOut = false;

                    //        //        CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false;
                    //        //        _SetLog("Error: QC Emit command Fail by Timeout");

                    //        //        CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                    //        //        m_iStep = 0;            // 처음부터 다시 시작
                    //        //        return false;
                    //        //    }
                    //        //}

                    //        //else if (CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG)                // Emit 명령 수렴
                    //        //{
                    //        //    CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false;

                    //        //    _SetLog("QC Send QCSendOutGOOD / QCSendOutNG ");
                    //        //    int SlotNum = GetBtmEmptySlot();
                    //        //    CData.Parts[(int)EPart.OFL].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
                    //        //    CData.LotInfo.iTOutCnt++;
                    //        //    m_nCurSlot = SlotNum;
                    //        //    m_iStep++;
                    //        //    return false;
                    //        //}

                    //        // return false;

                    //    }


                    //case 18: // 배출 완료 대기 Timeout 지정
                    //    {

                    //        if (CQcVisionCom.rcvQCSendEnd)    // 전송 완료 수신
                    //        {
                    //            m_bDuringGOut = false;                      // GOOD MGZ배출 동작 완료

                    //            // Mgz에 데이터를 채워 넣는다.
                    //            int SlotNum = GetBtmEmptySlot();
                    //            CData.Parts[(int)EPart.OFL].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
                    //            CData.LotInfo.iTOutCnt++;

                    //            try
                    //            {
                    //                CData.Parts[(int)EPart.OFL].sLotName = CQcVisionCom.sStringInfo[0];
                    //                if (Int32.TryParse(CQcVisionCom.sStringInfo[1], out CData.Parts[(int)EPart.OFL].iMGZ_No) == false)
                    //                {
                    //                    CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                    //                }

                    //                if (Int32.TryParse(CQcVisionCom.sStringInfo[2], out CData.Parts[(int)EPart.OFL].iSlot_No) == false)
                    //                {
                    //                    CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                    //                }

                    //            }
                    //            catch
                    //            {
                    //                CData.Parts[(int)EPart.OFL].sLotName = "Exception";
                    //                CData.Parts[(int)EPart.OFL].iMGZ_No = 0;
                    //                CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                    //            }

                    //            m_EndSeq = DateTime.Now;
                    //            m_SeqTime = m_EndSeq - m_StartSeq;

                    //            // 모든 인터페이스 Flag 초기화
                    //            CQcVisionCom.rcvQCReadyQueryGOOD = false;
                    //            CQcVisionCom.rcvQCReadyQueryNG = false;
                    //            CQcVisionCom.rcvQCSendOutGOOD = false;
                    //            CQcVisionCom.rcvQCSendOutNG = false;
                    //            CQcVisionCom.rcvQCSendEnd = false;
                    //            CQcVisionCom.rcvQCAbortTransfer = false;

                    //            _SetLog("QC Send-End, Transfer finish ");
                    //            m_iStep = 0;
                    //            return true;                            // 배출 성공으로 끝낸다.
                    //        }

                    //        // 전송 취소가 수신되었다.
                    //        if (CQcVisionCom.rcvQCAbortTransfer)
                    //        {
                    //            m_bDuringGOut = false;                      // GOOD MGZ배출 동작 완료

                    //            m_EndSeq = DateTime.Now;
                    //            m_SeqTime = m_EndSeq - m_StartSeq;

                    //            // 모든 인터페이스 Flag 초기화
                    //            CQcVisionCom.rcvQCReadyQueryGOOD = false;
                    //            CQcVisionCom.rcvQCReadyQueryNG = false;
                    //            CQcVisionCom.rcvQCSendOutGOOD = false;
                    //            CQcVisionCom.rcvQCSendOutNG = false;
                    //            CQcVisionCom.rcvQCSendEnd = false;
                    //            CQcVisionCom.rcvQCAbortTransfer = false;

                    //            _SetLog("QC Abort-Transfer, Strip Transfer canceled");
                    //            m_iStep = 0;
                    //            return true;                            // 배출 취소로 끝낸다.
                    //        }
                    //    } 
                    //    //of case 18





                    //        // 배출 완료 응답 초기화
                    //        //CTcpIp.It.bSuccess_Du = false;
                    //        //CTcpIp.It.bSuccess_Ok = false;
                    //        //CTcpIp.It.bSuccess_Fail = false;
                    //        //CQcVisionCom.rcvQCSendEnd = false;
                    //        CQcVisionCom.rcvQCReadyQueryGOOD = false;
                    //        CQcVisionCom.rcvQCReadyQueryNG = false;

                    //        m_Delay.Set_Delay(30000);
                    //        m_TSendDelay.Set_Delay(1);

                    //        m_iStep++;
                    //        return false;
                    //    }

                    //case 19:    // 배출 작업을 마쳤는가 ?
                    //    {
                    //        if(!m_TSendDelay.Chk_Delay()) return false;

                    //        //CTcpIp.It.SendSucces();                     // 배출 성공 여부 조회

                    //        _SetLog("Send Emit success query -> QC");

                    //        m_iStep++;
                    //        return false;
                    //    }

                    //case 20:                // 배출 동작중인가 ?
                    //    {
                    //        // 응답이 아직 도착하지 않았다.
                    //        //if(!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail) 
                    //        if(!CQcVisionCom.rcvQCSendEnd)
                    //        {
                    //            if (!m_Delay.Chk_Delay())           // 30초 이내라면
                    //            {
                    //                //m_TSendDelay.Wait(2000);
                    //                m_TSendDelay.Set_Delay(2000);
                    //                //딜레이 후 한번 더 확인
                    //                //if (!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail)
                    //                if(!CQcVisionCom.rcvQCSendEnd)
                    //                {
                    //                    _SetLog("Wait Emit success result from QC... ");

                    //                    m_iStep = 19;           // 배출 완료 여부를 다시 묻는다.
                    //                    return false;
                    //                }
                    //            }
                    //            else                           // 30초 초과
                    //            {
                    //                m_bDuringGOut           = false;
                    //                //CTcpIp.It.bSuccess_Du   = false;
                    //                //CTcpIp.It.bSuccess_Ok   = false;
                    //                //CTcpIp.It.bSuccess_Fail = false;
                    //                CQcVisionCom.rcvQCSendEnd = false;

                    //                //_SetLog("Error: QC Emit success query by Timeout");
                    //                _SetLog("Error: QC strip transfer success query by Timeout");

                    //                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                    //                m_iStep = 0;                // 동작 실패, 다시 처음으로 되돌아가서 재시도한다.
                    //                return false;
                    //            }
                    //        }

                    //        m_iStep = 22;           // 수신 결과 처리
                    //        return false;
                    //    }

                    ////case 21:
                    ////    {
                    ////        if(!m_Delay.Chk_Delay())
                    ////        {
                    ////            if(!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail) return false;
                    ////            else 
                    ////            {
                    ////                CTcpIp.It.SendSucces();
                    ////                m_LogVal.iStep = m_iStep;
                    ////                m_LogVal.sMsg  = "Qc recmd Send Ok";
                    ////                SaveLog();

                    ////                m_iStep++;
                    ////                return false;
                    ////            }
                    ////        }
                    ////        else 
                    ////        {
                    ////            m_bDuringGOut           = false;
                    ////            CTcpIp.It.bEmit_Ok      = false;
                    ////            CTcpIp.It.bEmit_Fail    = false;
                    ////            CTcpIp.It.bSuccess_Ok   = false;
                    ////            CTcpIp.It.bSuccess_Fail = false;
                    ////            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                    ////            m_LogVal.iStep = m_iStep;
                    ////            m_LogVal.sMsg  = "Not Qc recmd ";
                    ////            SaveLog();

                    ////            m_iStep = 0;
                    ////            return false;
                    ////        }
                    ////    }

                    //case 22:
                    //    {
                    //        //if(CTcpIp.It.bSuccess_Ok)           // 배출 성공
                    //        if(CQcVisionCom.rcvQCSendEnd)
                    //        {
                    //    m_bDuringGOut = false;
                    //    //int SlotNum = GetBtmEmptySlot();
                    //    //CData.Parts[(int)EPart.OFL].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
                    //    //190731 ksg :
                    //    //----------------
                    //    try
                    //    {
                    //        //CData.Parts[(int)EPart.OFR].sLotName = CTcpIp.It.sStringInfo[0];
                    //        //CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt16(CTcpIp.It.sStringInfo[1]);
                    //        //CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt16(CTcpIp.It.sStringInfo[2]);
                    //        CData.Parts[(int)EPart.OFR].sLotName = CQcVisionCom.sStringInfo[0];
                    //        CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt16(CQcVisionCom.sStringInfo[1]);
                    //        CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt16(CQcVisionCom.sStringInfo[2]);

                    //    }
                    //    catch
                    //    {
                    //        CData.Parts[(int)EPart.OFR].sLotName = "Exception";
                    //        CData.Parts[(int)EPart.OFR].iMGZ_No = 0;
                    //        CData.Parts[(int)EPart.OFR].iSlot_No = 0;
                    //    }
                    //    //CData.Parts[(int)EPart.OFR].group  = CTcpIp.It.sStringInfo[3]; <-Group 없음
                    //    //CData.Parts[(int)EPart.OFR].device = CTcpIp.It.sStringInfo[4]; <-Deivce 없음
                    //    //----------------
                    //    //CData.LotInfo.iTOutCnt++; //190407 ksg :
                    //    //CTcpIp.It.bResultGood = false;
                    //    CQcVisionCom.rcvQCReadyQueryGOOD = false;
                    //    CQcVisionCom.rcvQCReadyQueryNG = false;
                    //    CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false; //EQ -> QC
                    //    CQcVisionCom.rcvQCSendEnd = false;
                    //    m_EndSeq = DateTime.Now; //190807 ksg :
                    //    m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                    //    _SetLog("Receive Emit success OK <- QC");

                    //    // 더이상 Emit Retry는 없다.
                    //    //CTcpIp.It.bEmit_Ok      = false;        // 이 Flag가 false가 되면 Retry 대기는 없다.
                    //    //CTcpIp.It.bEmit_Fail    = false;

                    //    if (!CData.Opt.bQcByPass)                // QC Vision Bypass가 아닌경우 
                    ////            {
                //                //200316 ksg :
                //                //CTcpIp.It.bTResult_Ok = false;
                //                //CTcpIp.It.SendTestResult();         // 결과를 받아온다
                //                CQcVisionCom.rcvResultQuery = false;
                //                CGxSocket.It.SendMessage("ResultQuery");
                //                _SetLog("Send Test result query -> QC");

                        //                m_Delay.Set_Delay(10000);
                        //                m_TSendDelay.Set_Delay(1000 );

                        //                m_iStep++; //200316 ksg :
                        //                return false;                           // 이어서 진행
                        //            }

                        //            _SetLog("QC Bypass mode, Receive done");

                        //            // QC Bypass일 경우
                        //            m_iStep = 0; //200316 ksg :
                        //            return true;                            // 2020-11-19, jhLee : 배출 성공으로 끝낸다.
                        //        }

                        //        // 배출 실패
                        //        // success 명령 반복 수행 필요

                        //        _SetLog("Error: QC Emit success Fail <- QC");

                        //        m_bDuringGOut = false;

                        //        CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                        //        m_iStep = 0;                // 배출 성공 여부를 체크하는 곳에서 부터 다시 시작한다.
                        //        return false;           
                        //    }
                        //    //200316 ksg :

                        //case 23:  // QC에게 Test 결과를 묻는다.
                        //    {
                        //        //if(!m_Delay.Chk_Delay() && !CTcpIp.It.bTResult_Ok)
                        //        if (!m_Delay.Chk_Delay() && !CQcVisionCom.rcvResultQuery) 
                        //        {
                        //            if(m_TSendDelay.Chk_Delay())
                        //            {
                        //                //CTcpIp.It.SendTestResult();
                        //                CGxSocket.It.SendMessage("ResultQuery");
                        //                _SetLog("Retry Send Test result query -> QC");

                        //                m_TSendDelay.Set_Delay(1000);
                        //                return false;
                        //            }
                        //            else
                        //            {
                        //                return false;
                        //            }
                        //        }
                        //        //else if(CTcpIp.It.bTResult_Ok)
                        //        else if (CQcVisionCom.rcvResultQuery)
                        //        {
                        //            _SetLog("Receive Test ResultQuery OK <- QC");

                        //            m_iStep = 0;
                        //            return true;
                        //        }
                        //        else
                        //        {

                        //            _SetLog("Error : QC Test result receive Timeout eror");

                        //            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                        //            m_iStep = 0;
                        //            return true;      
                        //        }
                        //    }
            }
        }

        /// <summary>
        /// Sky Works 전용
        /// Cycle CycleTopPick(비젼 사용시 나중에 완료 할 것)
        /// Sequence Cycle : ESeq_Stat - OFFL_TopPick
        /// </summary>
        /// <returns></returns>
        public bool Cyl_TopPick()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    _SetLog("Error : Time out");
                    CErr.Show(eErr.OFFLOADER_TOP_PICK_TIMEOUT);
                    m_iStep = 0;

                    return true;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_TopPick"; //200630

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
                        if (m_bxChkLight) return false;

                        //20191120 ghk_display_strip
                        if (CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) || CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2))
                        {//Mgz 둘중 하나라도 감지 되고 있는지 확인(or 검사)
                            _SetLog("Error : Already detected top magazine");
                            CErr.Show(eErr.OFFLOADER_ALREADY_TOP_MGZ_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                _SetLog("Error : Strip-out detect.");

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (!(CDataOption.UseQC && CData.Opt.bQcUse) && CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            //
                        }
                        else
                            ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.


                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety


                        Act_TopClampOC(m_ActOpen);
                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        _SetLog("Middel belt stop, Top clamp open");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_TopClampOC(m_ActOpen))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActUp);
                        Act_TopClampModuleDU(m_ActUp);
                        //Act_TopClampModuleDU(m_ActDn);

                        _SetLog("Bottom clamp up, Top clamp up");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActUp))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        //if (!Act_TopClampModuleDU(m_ActDn))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);

                        _SetLog(string.Format("X axis move align, Pos:{0}mm", CData.Dev.dOffL_X_Algn));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn)) return false;

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPickGo);

                        _SetLog(string.Format("Z axis move pick, Pos:{0}mm", CData.SPos.dOFL_Z_TPickGo));

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);

                        _SetLog(string.Format("Y axis move pick, Pos:{0}mm", CData.SPos.dOFL_Y_Pick));

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        { return false; }

                        if(m_bxChkLight) return false;
                        
                        Belt_Mid(m_BeltRun, m_BeltCCw);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);

                        _SetLog(string.Format("Middle belt run CCW, Set delay {0}ms", GV.ONL_DETECT_MGZ_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);

                        if (!Get_TopMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        if (Get_TopMgzChk())
                        {

                            _SetLog("Top clamp detect, Middle belt stop");

                            m_iStep = 20;
                            return false;
                        }

                        _SetLog("Middle belt stop");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        m_Delay.Set_Delay(100);

                        _SetLog("Middle belt stop, Set dealy:100ms");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);

                        _SetLog("Error : Middle conveyor not detect magazine");

                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TClamp);

                        _SetLog(string.Format("Z axis move top clamp, Pos:{0}mm", CData.SPos.dOFL_Z_TClamp));

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TClamp))
                        { return false; }

                        Act_TopClampOC(m_ActClose);

                        _SetLog("Top clamp close");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if (!Act_TopClampOC(m_ActClose))
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPickUp);

                        _SetLog(string.Format("Middle belt run CW, Z axis move pick up, Pos:{0}mm", CData.SPos.dOFL_Z_TPickUp));

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPickUp))
                        { return false; }

                        if(m_bxChkLight) return false;
                        SetTopAllStatusMgz(eSlotInfo.Empty);

                        //20200513 jym : QC 외에 Top 사용 추가
                        int iPt = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        //20191121 ghk_display_strip
                        CData.Parts[iPt].bExistStrip = true;

                        //Mgz 상태 변환
                        CData.Parts[iPt].iStat = ESeq.OFL_TopRecive;

                        bReadyNextSlot = true;      // 2021-06-07, jhLee : 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는가 ?
                       
                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if(CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else                           CData.Parts[iPt].iSlot_No = 0;

                        Belt_Mid(m_BeltStop, m_BeltCw);

                        _SetLog("Middle belt stop, Status:" + CData.Parts[iPt].iStat);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        {
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_TOP_MGZ_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        m_EndSeq  = DateTime.Now         ; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        _SetLog(string.Format("Finish, Tack time:{0}", m_SeqTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        /// <summary>
        /// Magazine Pick Cycle (매거진 BCR 읽음, OFF-Loader RFID 사용 옵션 && 매거진 TOP 배출 && SPIL 의 경우에만)
        /// </summary>
        /// <returns></returns>
        public bool Cyl_TopPick_Bcr()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    _SetLog("Error : Time out");
                    CErr.Show(eErr.OFFLOADER_TOP_PICK_TIMEOUT);
                    m_iStep = 0;

                    // 2022.05.11 SungTae Start : [추가] ASE-KH Multi-LOT 관련
                    if (CData.CurCompany == ECompany.ASE_K12) { CData.LotMgr.IsUnloaderTrackOutFlag = eTrackOutFlag.Error; }
                    // 2022.05.11 SungTae End

                    return true;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_TopPick_Bcr"; //200630

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
                        if(m_bxChkLight) return false;

                        // 2022.04.26 SungTae Start : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련 Track Out Flag 확인
                        if (CData.CurCompany == ECompany.ASE_K12 && CData.IsMultiLOT())
                        {
                            if (CData.LotMgr.IsUnloaderTrackOutFlag == eTrackOutFlag.Error)
                            {
                                string sMsg = $"[OFF-LOADER]\nPlease select one of the buttons below :\n\n" +
                                              $"1) [OK] - Invalid MGZ information received from host. Eject the loaded MGZ.\n            (MGZ ID : {CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr})\n" +
                                              $"2) [CANCEL] - Retry reading MGZ ID.";

                                if (CMsg.Show(eMsg.Warning, "Warning", sMsg) == DialogResult.OK)
                                {
                                    m_iStep = 100;      // Track-Out Step으로 이동
                                }
                                else
                                {
                                    m_iStep = 34;       // Auto Run 재시작 시 MGZ ID Read 하는 Step으로 이동
                                }

                                CData.LotMgr.IsUnloaderTrackOutFlag = eTrackOutFlag.Valid;
                                return false;
                            }
                        }
                        // 2022.04.14 SungTae End

                        //20191120 ghk_display_strip
                        if (CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) || CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2))
                        {//Mgz 둘중 하나라도 감지 되고 있는지 확인(or 검사)
                            _SetLog("Error : Already detected top magazine");
                            CErr.Show(eErr.OFFLOADER_ALREADY_TOP_MGZ_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                _SetLog("Error : Strip-out detect.");

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (!(CDataOption.UseQC && CData.Opt.bQcUse) && CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            //
                        }
                        else
                            ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        Act_TopClampOC(m_ActOpen);
                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        _SetLog("Middel belt stop, Top clamp open");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_TopClampOC(m_ActOpen))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActUp);
                        Act_TopClampModuleDU(m_ActUp);
                        //Act_TopClampModuleDU(m_ActDn);

                        _SetLog("Bottom clamp up, Top clamp up");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActUp))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        //if (!Act_TopClampModuleDU(m_ActDn))
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);

                        _SetLog(string.Format("X axis move align, Pos:{0}mm", CData.Dev.dOffL_X_Algn));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn)) return false;

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPickGo);

                        _SetLog(string.Format("Z axis move pick, Pos:{0}mm", CData.SPos.dOFL_Z_TPickGo));

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);

                        _SetLog(string.Format("Y axis move pick, Pos:{0}mm", CData.SPos.dOFL_Y_Pick));

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        { return false; }

                        if(m_bxChkLight) return false;
                        
                        Belt_Mid(m_BeltRun, m_BeltCCw);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);

                        _SetLog(string.Format("Middle belt run CCW, Set delay {0}ms", GV.ONL_DETECT_MGZ_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);

                        if (!Get_TopMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        if (Get_TopMgzChk())
                        {
                            _SetLog("Top clamp detect, Middle belt stop");

                            m_iStep = 20;
                            return false;
                        }

                        _SetLog("Middle belt stop");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        m_Delay.Set_Delay(100);

                        _SetLog("Middle belt run, Set dealy:100ms");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);

                        _SetLog("Error : Middle conveyor not detect magazine");

                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TClamp);

                        _SetLog(string.Format("Z axis move top clamp, Pos:{0}mm", CData.SPos.dOFL_Z_TClamp));

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TClamp))
                        { return false; }

                        Act_TopClampOC(m_ActClose);

                        _SetLog("Top clamp close");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if (!Act_TopClampOC(m_ActClose))
                        { return false; }

                        if(m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPickUp);

                        _SetLog(string.Format("Middle belt run CW, Z axis move pick up, Pos:{0}mm", CData.SPos.dOFL_Z_TPickUp));

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPickUp))
                        { return false; }

                        Belt_Mid(m_BeltStop, m_BeltCw); //중간 벨트 정지

                        _SetLog("Z axis move check, Middle belt stop");

                        m_iStep++;
                        return false;
                    }


                case 24: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog(string.Format("Y axis move Wait   {0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 25: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        _SetLog("Y axis move check");

                        //////////////////////////////////////////////
                        m_iReReadCnt = 0; //바코드 판독 시도 회수 초기화
                        //////////////////////////////////////////////

                        m_iStep++;
                        return false;
                    }

                // 반복 Loop //////////////////////////////////////////////////////////////////////

                case 26: // 바코드 읽기 시퀀스 돌입
                    {
                        if(m_iReReadCnt > 0) //재시도인 경우 바코드 읽기 중지(Timing OFF) => Z축 +30 mm (아래로) => Z축 바코드 위치 => 바코드 읽기
                        { 
                            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)

                            _SetLog("MGZ BCR Timing OFF");

                            m_iStep++;
                            return false;
                        }

                        m_iStep = 30; //바코드 읽기
                        return false;
                    }

                case 27:  // Z축 (바코드 위치 + 10 mm) 이동 (아래로)
                    {
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOffL_Z_MgzBcr + 10));

                        _SetLog(string.Format("Z axis move    {0}mm", (CData.Dev.dOffL_Z_MgzBcr + 10)));

                        m_iStep++;
                        return false;
                    }

                case 28:  // Z축 (바코드 위치 + 10 mm) 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOffL_Z_MgzBcr + 10)))
                        { return false; }

                        m_Delay.Set_Delay(100); //이동 후 0.1초 대기

                        _SetLog("Z axis move check. Set Delay(100)");

                        m_iStep++;
                        return false;
                    }

                case 29: //Delay 체크
                    {
                        if(!m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay");

                        m_iStep++;
                        return false;
                    }

                // 바코드 읽기 /////////////////////////////////////////////////

                //20200611 jhc : Z축, Y축 이동 순서 변경 (Y->Z ==> Z->Y)

                case 30: // Z축 바코드 위치 이동
                    {
                        CMot.It.Mv_N(m_iZ, CData.Dev.dOffL_Z_MgzBcr);

                        _SetLog(string.Format("Z axis move    {0}mm", CData.Dev.dOffL_Z_MgzBcr));

                        m_iStep++;
                        return false;
                    }

                case 31: // Z축 바코드 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffL_Z_MgzBcr))
                        { return false; }

                        _SetLog("Z axis move check");

                        m_iStep++;
                        return false;
                    }

                case 32: // Y축 바코드 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_MgzBcr);

                        _SetLog(string.Format("Y axis move    {0}mm", CData.Dev.dOffL_Y_MgzBcr));

                        m_iStep++;
                        return false;
                    }

                case 33: // Y축 바코드 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_MgzBcr))
                        { return false; }

                        _SetLog("Y axis move check");

                        m_iStep++;
                        return false;
                    }

                case 34: // 바코드 읽기
                    {
                        CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr = ""; //BCR 값 초기화

                        CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LON); //바코드 읽기 요청 (Timing ON)

                        ////////////////////////////////////////////
                        m_iReReadCnt++; //바코드 판독 시도 회수 1 증가
                        ////////////////////////////////////////////

                        m_Delay.Set_Delay(3000); //최대 대기 시간
                        
                        _SetLog("MGZ BCR read start");

                        m_iStep++;
                        return false;
                    }

                case 35: // 바코드 읽기 완료 체크
                    {
                        if (!CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr.Equals(""))
                        {
                            /////////////////////
                            // 바코드 판독 완료 //
                            
                            CData.Parts[(int)EPart.OFL].sMGZ_ID = CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr;
                            CData.LotInfo.sGMgzId = CData.Parts[(int)EPart.OFL].sMGZ_ID;

                            _SetLog(string.Format("MGZ BCR read OK [{0}],  Read count = {1}", CData.Parts[(int)EPart.OFL].sMGZ_ID, m_iReReadCnt));

                            //200131 LCY                        
                            if(CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID = CData.Parts[(int)EPart.OFL].sMGZ_ID; 

                                // 2022.04.27 SungTae Start : [수정] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag 확인 및 Log 추가
                                if (!CData.bChkManual)
                                {
                                	// 2022.03.31 SungTae Start : [추가] Log 추가
                                	_SetLog($"[UNLOADER][SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request).  MgzID : {CData.Parts[(int)EPart.OFL].sMGZ_ID}, Mgz Port : {(uint)SECSGEM.JSCK.eMGZPort.GOOD}.");
                                	// 2022.03.31 SungTae End
									
                                    CData.GemForm.UnloaderMgzVerifyRequest(CData.Parts[(int)EPart.OFL].sMGZ_ID, (uint)SECSGEM.JSCK.eMGZPort.GOOD);
                                    
                                    CLog.mLogGnd($"CSQ8_ONL Step(67) - [SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request). sLD_MGZ_ID : {CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID}");
                                }
                                // 2022.04.27 SungTae End
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
                            _SetLog(string.Format("MGZ BCR read fail -> retry.  Read count = {0}", m_iReReadCnt));

                            m_iStep = 26; //매거진 바코드 읽기 재시도
                            return false;
                        }
                        else
                        {
                            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)
                            _SetLog("MGZ BCR Timing OFF");
                            
                            //바코드 읽기 시도 회수 초과 => Error
                            _SetLog(string.Format("MGZ BCR read fail (Read count over).  Read count = {0}", m_iReReadCnt));

                            m_iReReadCnt = 0;

                            m_iStep = 0;
                            //20200611 jhc : 메시지 표시 변경
                            CMsg.Show(eMsg.Error, "OffLoader magazine BCR read fail", "1.Check the direction of the magazine.\r2.Please confirm that BCR module works normally.");
                            return true;
                        }
                    }

                case 36:
                    {
                        if (CData.Opt.bSecsUse) // SECSGEM 사용 시 Verify 완료 여부 확인
                        {
                            if (m_Delay.Chk_Delay())
                            { // Verify 진행 60 초 Timeout 발생
                                CErr.Show(eErr.HOST_ULD_MGZ_VERIFY_TIME_OVER_ERROR);

                                CData.LotMgr.IsUnloaderTrackOutFlag = eTrackOutFlag.Error;    // 2022.04.26 SungTae : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련
                                
                                m_iStep = 0;
                                return true;
                            }
                            else if (CData.SecsGem_Data.nMGZ_Check != Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Complete)/*2*/)
                            {// Verify 진행 중.
                                if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Error)/*-1*/)
                                {// Verify Error 발생
                                    CErr.Show(eErr.HOST_ULD_MGZ_VERIFY_TIME_OVER_ERROR);

                                    CData.LotMgr.IsUnloaderTrackOutFlag = eTrackOutFlag.Error;    // 2022.04.26 SungTae : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련
                                    
                                    m_iStep = 0;
                                    return true;
                                }
                                else if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Run)/*1*/)
								{
                                    CLog.mLogGnd($"CSQ8_OFL Step(36) - Check Magazine Verify Request State : {(int)SECSGEM.JSCK.eChkVerify.Run}({SECSGEM.JSCK.eChkVerify.Run})");
                                    return false;
								}
                            }
                        }

                        if(m_bxChkLight) return false;

                        SetTopAllStatusMgz(eSlotInfo.Empty);

                        //20200513 jym : QC 외에 Top 사용 추가
                        int iPt = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        CData.Parts[iPt].bExistStrip = true;

                        //Mgz 상태 변환
                        CData.Parts[iPt].iStat = ESeq.OFL_TopRecive;

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        if(CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else                           CData.Parts[iPt].iSlot_No = 0;

                        //191118 ksg :
                        if(CData.GemForm != null)	CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Cnt++;

                        _SetLog("Status:" + CData.Parts[iPt].iStat.ToString());

                        m_iStep++;
                        return false;
                    }

                case 37: // Y축 대기 위치 이동
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        {
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_TOP_MGZ_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 38: // Y축 대기 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        m_EndSeq  = DateTime.Now         ; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        _SetLog(string.Format("Finish, Tack time:{0}", m_SeqTime.ToString(@"hh\:mm\:ss")));

                        m_iStep = 0;
                        return true;
                    }

                ///////////////////////
                case 100:
                    {
                        if (m_bxChkLight)   return false;

                        _SetLog($"[TRACK-OUT] Loaded MGZ Track-out Start... MGZ ID : {CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT].sULD_MGZ_ID}");

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))    { Belt_Top(m_BeltRun); }
                        else                                        { Belt_Top(m_BeltStop); }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPlaceGo);

                        _SetLog($"[TRACK-OUT] Z axis move place. ({CData.SPos.dOFL_Z_TPlaceGo}mm)");

                        m_iStep++;
                        return false;
                    }

                case 101:
                    {
                        if (m_bxChkLight)   return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))    { Belt_Top(m_BeltRun); }
                        else                                        { Belt_Top(m_BeltStop); }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPlaceGo))  { return false; }

                        m_Delay.Set_Delay(2000);

                        _SetLog($"[TRACK-OUT] Z axis move complete. Set delay : 2000ms");

                        m_iStep++;
                        return false;
                    }

                case 102:
                    {
                        if (m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))    { Belt_Top(m_BeltRun); }
                        else                                        { Belt_Top(m_BeltStop); }

                        if (Get_TopMgzChk() && !m_Delay.Chk_Delay())    { return false; }

                        Belt_Top(m_BeltStop);

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect) || !CIO.It.Get_X(eX.OFFL_TopMGZDetectFull))
                        {
                            _SetLog("[TRACK-OUT] Error : Top magazine full");

                            CErr.Show(eErr.OFFLOADER_TOP_MAGZIN_FULL);

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("[TRACK-OUT] Top belt stop");

                        m_iStep++;
                        return false;
                    }

                case 103:
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Place);

                        _SetLog(string.Format("[TRACK-OUT] Y axis move place, Pos:{0}mm", CData.SPos.dOFL_Y_Place));

                        m_iStep++;
                        return false;
                    }

                case 104:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Place))
                        {
                            return false;
                        }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TUnClamp);

                        _SetLog(string.Format("[TRACK-OUT] Z axis move top unclamp, Pos:{0}mm", CData.SPos.dOFL_Z_TUnClamp));

                        m_iStep++;
                        return false;
                    }

                case 105:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TUnClamp))
                        {
                            return false;
                        }

                        Act_TopClampOC(m_ActOpen);

                        _SetLog("[TRACK-OUT] Top clamp open");

                        m_iStep++;
                        return false;
                    }

                case 106:
                    {
                        if (!Act_TopClampOC(m_ActOpen)) { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPlaceDn);

                        _SetLog(string.Format("[TRACK-OUT] Z axis move top place down, Pos : {0}mm", CData.SPos.dOFL_Z_TPlaceDn));

                        m_iStep++;
                        return false;
                    }

                case 107:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPlaceDn))  { return false; }

                        if (m_bxChkLight)   return false;

                        Belt_Top(m_BeltRun);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_Delay.Set_Delay(2000);

                        _SetLog(string.Format("[TRACK-OUT] Top belt run, Y axis move wait, Pos:{0}mm, Set delay:2000ms", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 108:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect) && !m_Delay.Chk_Delay())
                        {
                            return false;
                        }

                        _SetLog("[TRACK-OUT] Check delay");

                        m_iStep++;
                        return false;
                    }

                case 109:
                    {
                        if (m_bxChkLight) return false;

                        Belt_Top(m_BeltRun);

                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Top(m_BeltStop);

                        _SetLog("[TRACK-OUT] Top belt stop");

                        m_iStep++;
                        return false;
                    }

                case 110: // Data 처리
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))  { return false; }

                        //20200513 jym : QC 외에 Top 사용 추가
                        int iPt = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        CData.Parts[iPt].bExistStrip = false;
                        CData.Parts[iPt].sLotName    = string.Empty;
                        CData.Parts[iPt].iStat       = ESeq.OFL_Wait;

                        CData.JSCK_Gem_Data[(int)EDataShift.LOT_END].sLot_Name   = string.Empty;
                        CData.JSCK_Gem_Data[(int)EDataShift.LOT_END].sULD_MGZ_ID = string.Empty;

                        if (CDataOption.OflRfid == eOflRFID.Use && CDataOption.RFID == ERfidType.Keyence && CData.Opt.eEmitMgz == EMgzWay.Top)
                        {
                            CData.Parts[iPt].sMGZ_ID = "";
                        }

                        SetTopAllStatusMgz(eSlotInfo.Empty);

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else                            CData.Parts[iPt].iSlot_No = 0;

                        CData.bChkManual = false;
                        CData.LotMgr.IsUnloaderTrackOutFlag = eTrackOutFlag.Valid;

                        _SetLog("Status:" + CData.Parts[iPt].iStat);

                        m_iStep++;
                        return false;
                    }

                case 111:
                    {
                        m_EndSeq    = DateTime.Now; //190807 ksg :
                        m_SeqTime   = m_EndSeq - m_StartSeq; //190807 ksg :

                        _SetLog("[TRACK-OUT] Unloaded MGZ Track-out Finish. Tack time - " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        CSQ_Man.It.bOffMGZTopPlaceDone = true;

                        m_iStep = 0;
                        return true;
                    }
            }
        }
        //


        /// <summary>
        /// Sky Works 전용
        /// Cycle Cycle Top Place(Vision 사용 시 추가 해야 됨)
        /// Sequence Cycle : ESeq_Stat - OFFL_TopPlace
        /// </summary>
        /// <returns></returns>
        public bool Cyl_TopPlace()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    _SetLog("Error : Time out");
                    CErr.Show(eErr.OFFLOADER_PLACE_TIMEOUT);

                    m_iStep = 0;
                    return true;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_TopPlace"; //200630

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
                        if(m_bxChkLight) return false;

                        if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        {
                            CData.Parts[(CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL].iStat = ESeq.OFL_TopPick;
                            
                            _SetLog("Error : Not detected top magazine, Status:OFL_TopPick");
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_TOP_MGZ_ERROR);
                            SEQ = ESeq.Idle;
                            m_iStep = 0;

                            return true;
                        }

                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                _SetLog("Error : Strip-out detect.");

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (!(CDataOption.UseQC && CData.Opt.bQcUse) && CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            //
                        }
                        else
                            ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.



                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청
                                
                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety


                        Act_TopClampOC(m_ActClose);
                        Belt_Top(m_BeltStop);

                        _SetLog("Top clamp close, Top belt stop");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_TopClampOC(m_ActClose))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActUp);
                        Act_TopClampModuleDU(m_ActUp);
                        //Act_TopClampModuleDU(m_ActDn);

                        _SetLog("Bottom clamp up, Top clamp up");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActUp))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        //if (!Act_TopClampModuleDU(m_ActDn))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPlaceGo);

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetectFull))
                        {
                            //set Error
                            _SetLog("Error : Top magazine full");
                            CErr.Show(eErr.OFFLOADER_TOP_MAGZIN_FULL);

                            m_iStep = 0;
                            return true;
                        }

                        if(m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        { Belt_Top(m_BeltRun); }

                        _SetLog(string.Format("Z axis move top place go, Pos:{0}mm", CData.SPos.dOFL_Z_TPlaceGo));

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if(m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        { Belt_Top(m_BeltRun); }
                        else
                        { Belt_Top(m_BeltStop); }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPlaceGo))
                        { return false; }

                        m_Delay.Set_Delay(2000);

                        _SetLog("Set delay:2000ms");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if(m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        { Belt_Top(m_BeltRun); }
                        else
                        { Belt_Top(m_BeltStop); }

                        if (Get_TopMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Top(m_BeltStop);

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect) || !CIO.It.Get_X(eX.OFFL_TopMGZDetectFull))
                        {
                            //set Error : Full Mgz
                            _SetLog("Error : Top magazine full");
                            CErr.Show(eErr.OFFLOADER_TOP_MAGZIN_FULL);
                            
                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Top belt stop");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Place);

                        _SetLog(string.Format("Y axis move place, Pos:{0}mm", CData.SPos.dOFL_Y_Place));

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Place))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TUnClamp);

                        _SetLog(string.Format("Z axis move top unclamp, Pos:{0}mm", CData.SPos.dOFL_Z_TUnClamp));

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TUnClamp))
                        { return false; }

                        Act_TopClampOC(m_ActOpen);

                        _SetLog("Top clamp open");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!Act_TopClampOC(m_ActOpen))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPlaceDn);

                        _SetLog(string.Format("Z axis move top place down, Pos:{0}mm", CData.SPos.dOFL_Z_TPlaceDn));

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPlaceDn))
                        { return false; }

                        if(m_bxChkLight) return false;                        

                        Belt_Top(m_BeltRun);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);                       
                        m_Delay.Set_Delay(2000);

                        _SetLog(string.Format("Top belt run, Y axis move wait, Pos:{0}mm, Set delay:2000ms", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect) && !m_Delay.Chk_Delay())
                        { return false; }

                        _SetLog("Check delay");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if(m_bxChkLight) return false;

                        Belt_Top(m_BeltRun);

                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Top(m_BeltStop);

                        _SetLog("Top belt stop");

                        m_iStep++;
                        return false;
                    }

                case 23: // Data 처리
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
                        //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
                        if (CDataOption.OflRfid == eOflRFID.Use && CDataOption.RFID == ERfidType.Keyence && CData.Opt.eEmitMgz == EMgzWay.Top)
                        //if (CDataOption.OflRfid == eOflRFID.Use && CData.Opt.eEmitMgz == EMgzWay.Top && CData.CurCompany == ECompany.SPIL)
                        {
                            CData.Parts[(int)EPart.OFL].sMGZ_ID = "";
                        }

                        //20200513 jym : QC 외에 Top 사용 추가
                        int iPt = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        //20191121 ghk_display_strip
                        CData.Parts[iPt].bExistStrip = false;

                        //Data 이동 및 상태 변환
                        CData.Parts[iPt].iStat = ESeq.OFL_Wait;

                        SetTopAllStatusMgz(eSlotInfo.Empty);
                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else                            CData.Parts[iPt].iSlot_No = 0;

                        CData.sLastLot = CData.Parts[(int)EPart.OFL].sLotName;
                        CData.iLastMGZ = CData.Parts[(int)EPart.OFL].iMGZ_No;

                        _SetLog("Status:" + CData.Parts[iPt].iStat);

                        m_iStep++;
                        return false;
                    }

                case 24:
                {
                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        _SetLog("Finish, Tack time:" + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        // 2021-05-31, jhLee, Multi-LOT 처리, 연속LOT 실행의 경우  Magazine Place의 경우 LOT 완료를 처리 하도록 한다.
                        if (CData.IsMultiLOT())
                        {
                            string sLotName = CData.LotMgr.UnloadingLotName;            // Unloading 중인 LOT의 이름

                            // MultiLOT 환경에서 LOT이 종료되었는지 check   
                            if (CData.LotMgr.IsLotComplete(sLotName))
                            {
                                // 더이상 배출중인 LOT의 잔여 Strip이 없을경우 해당 LOT을 종료 시킨다.
                                CData.LotMgr.ClearUnloadingLot();                   // Unloading 중인 LOT을 초기화 해준다.
                                CData.bLotCompleteMsgShow = true;                   // LOT 종료 메세지를 보여주도록 한다.

                                CData.Parts[(int)EPart.OFL].iMGZ_No = 0;        // 배출된 Magazine 번호 초기화
                                _SetLog($"Multi-LOT: (TOP) Finish Unloading LOT, LOT ID : {sLotName}");
                            }
                            else
                                _SetLog($"Multi-LOT: (TOP) Continue Unloading LOT, LOT ID : {sLotName}");
                        }


                        CSQ_Man.It.bOffMGZTopPlaceDone = true;

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// (Sky Work 전용)
        /// Sequence Cycle : ESeq_Stat - OFFL_TOPRecive
        /// </summary>
        /// <returns></returns>
        public bool Cyl_TopRecive()
        {

            //20191121 ghk_display_strip
            if (!Get_TopMgzChk())
            {//매거진 항상 있어야 함.
                _SetLog("Error : Not detected top magazine");
                CErr.Show(eErr.OFFLOADER_TOP_NOT_DETECT_MGZ_ERROR);

                m_iStep = 0;
                return true;
            }

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_TopRecive"; //200630

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
                        m_tmSafety.Set_Delay(1);


                        // QC Vision을 사용하지 않는다면 Dry Zone에서 넘어오는 제품과의 간섭 check 필요
                        if (!(CDataOption.UseQC && CData.Opt.bQcUse))
                        {
                            //200630 jhc
                            if (CSq_Dry.It.m_DuringDryOut)
                            {
                                m_iStep = 0;
                                return true;
                            }

                            //200630 jhc : 이동 전 자재 감지 확인
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                _SetLog("Error : Strip-out detect");
                                CErr.Show(eErr.DRY_DETECTED_STRIP);

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End
                        }
                        else
                        {
                            // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                            if (QcVisionPermission() == 1)                      // 간섭 발생
                            {
                                if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                                {
                                    if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                    {
                                        m_nSafetyCount = 0;

                                        _SetLog("Error : QC Strip-out detect at long time");
                                        CErr.Show(eErr.QC_DETECTED_STRIP);

                                        m_iStep = 0;
                                        return true;            // 실행 취소
                                    }

                                    _SetLog("Check Safety Interlock Fail from QC");
                                    m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                    CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                    _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                }
                                return false;       // 반복
                            }//of if QC Not Safety
                        }

                        Act_TopClampOC(m_ActClose);

                        _SetLog("Check Safety & Top clamp On");

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_TopClampOC(m_ActClose))    { return false; }

                        Act_TopClampModuleDU(m_ActDn);

                        _SetLog("Top clamp down");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_TopClampModuleDU(m_ActDn)) { return false; }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        // Y축 이동
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        if (GetTopFullSlotCheck())
                        {
                            _SetLog("Top magazine slot full, Status:OFL_TopPlace");
                            CData.Parts[(CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL].iStat = ESeq.OFL_TopPlace;
                            
                            m_iStep = 0;
                            return true;
                        }

                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        m_dMovePos = GetTopWorkPos();               // 이동해야 할 Slot 위치 계산

                        CMot.It.Mv_N(m_iZ, m_dMovePos);             // Slot 위치로 Z축 이동

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = m_dMovePos;
                        SaveLog();

                        _SetLog($"Z Axis move to {m_dMovePos:F3}");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //double CurPos = GetWorkPos(); //수식 계산 완료 해야 함.
                        if (!CMot.It.Get_Mv(m_iZ, m_dMovePos))
                        {
                            if (CMot.It.Get_Stop(m_iZ))         // 만약 축이 멈춰있다면 
                            {
                                CMot.It.Mv_N(m_iZ, m_dMovePos); // 다시 이동 명령을 내린다.
                            }

                            return false;
                        }

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = m_dMovePos;            //  GetBtmWorkPos();
                        SaveLog();

                        _SetLog("Bottom Mgz move empty slot position end");

                        m_iStep++;
                        return false;
                    }


                case 15:
                    {
                        if (CDataOption.UseQC && CData.Opt.bQcUse)
                        {
                            if (CQcVisionCom.m_nReadyReqMgz == CQcVisionCom.eMGZ_NG)
                            {
                                CQcVisionCom.m_nMgzReady = CQcVisionCom.eMGZ_NG;         // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료

                                CGxSocket.It.SendMessage("QCSendRequest");            // QC Vision에게 Strip send request
                                
                                _SetLog("[SEND](EQ->QC) QCSendRequest");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                        }

                        bReadyNextSlot = false;      // 2021-06-07, jhLee : 이동을 마친 뒤 다음 Strip을 받을 수 있도록 빈 Slot 준비 위치로 이동시켜줘야 하는 Flag 초기화

                        m_EndSeq = DateTime.Now;
                        m_SeqTime = m_EndSeq - m_StartSeq;

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = " m_bDuringFOut = " + m_bDuringFOut.ToString() + ", CData.Opt.bQcUse = " + CData.Opt.bQcUse.ToString();
                        m_LogVal.sMsg  += "\r\n Time_Ofl Top Recive : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :

                        _SetLog($"Cyl_TopRecive() Finish.  Time - {m_SeqTime:hh\\:mm\\:ss}");

                        m_iStep = 0;            // 지정 위치로 이동 완료
                        return true;
                    }



//                case 16:    // QC Vision에게 배출 요청 전송 
//                    {
//                        if(!m_TSendDelay.Chk_Delay()) return false;

//                        //CTcpIp.It.SendEmit();
//                        CGxSocket.It.SendMessage("QCSendRequest");

//                        m_Delay.Set_Delay(30000);

//                        _SetLog("Send QCSendRequest -> QC");

//                        m_iStep++;
//                        return false;
//                    }


//                case 17:
//                    {
//                        if (!CQcVisionCom.rcvQCSendOutGOOD && !CQcVisionCom.rcvQCSendOutNG)            // SendOut 명령을 받지 못하였다면
//                        {
//                            if (!m_Delay.Chk_Delay())                                               // 30초 이내라면 다시 전송한다.
//                            {
//                                return false;
//                            }
//                            else // Timeout
//                            {
//                                m_bDuringGOut = false;

//                                // CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false;
//                                _SetLog("Error: QC Emit command Fail by Timeout");

//                                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

//                                m_iStep = 0;            // 처음부터 다시 시작
//                                return false;
//                            }
//                        }
//                        else if (CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG)         // SendOut 명령 수렴
//                        {
//                            CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false;

//                            _SetLog("QC Send QCSendOutGOOD / QCSendOutNG ");
//                            int SlotNum = GetTopEmptySlot();

//                            // 미리 데이터를 갱신한다.
//                            CData.Parts[(int)EPart.OFR].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
//                            CData.LotInfo.iTOutCnt++;
//                            m_nCurSlot = SlotNum;
//                            m_iStep++;

//                            return false;
//                        }

//                        return false;                        
//                    }

//                case 18:
//                    {
//                        CQcVisionCom.rcvQCReadyQueryGOOD = false;
//                        CQcVisionCom.rcvQCReadyQueryNG = false;
    

//                        m_Delay.Set_Delay(30000);
//                        m_TSendDelay.Set_Delay(1);

//                        m_iStep++;
//                        return false;
//                    }

//                case 19:   // 배출 작업을 마쳤는가 ?
//                    {
//                        if(!m_TSendDelay.Chk_Delay())return false;

//                        m_TSendDelay.Set_Delay(3000);

//                        // 배출완료 정보 수신 여부 조회
//                        if (CQcVisionCom.rcvQCSendEnd)                  // 
//                        {
//                            _SetLog("finish transferring from QC : QCSendEnd");
//                            CQcVisionCom.rcvQCSendOutNG = CQcVisionCom.rcvQCSendOutGOOD = false;        // 또 초기화 ??

//                            m_iStep = 22;  // 배출 완료 처리로 간다.

//                        }
//                        // QC로부터 전송 취소가 수신되었는가 ?
//                        else if (CQcVisionCom.rcvQCAbortTransfer)
//                        {
//                            CQcVisionCom.rcvQCAbortTransfer = false;            // 사용된 변수 초기화
//                            _SetLog("Abort transferring from QC : QCAbortTreanser");
//dd
//                            m_iStep = 0;
//                            return false;
//                        }
//                        else if (m_TSendDelay.Chk_Delay())
//                        {
//                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

//                            m_iStep = 0;
//                            return false;

//                        }
                        
//                        return false;
//                    }

                //case 20:    // 배출 동작중인가 ?
                //    {
                //        //if(!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail) 
                //        if (!CQcVisionCom.rcvQCSendEnd)
                //        {
                //            if(!m_Delay.Chk_Delay())
                //            {
                //                //m_TSendDelay.Wait(2000);
                //                m_TSendDelay.Set_Delay(2000);

                //                //딜레이 후 한번 더 확인
                //                //if (!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail)
                //                if (!CQcVisionCom.rcvQCSendEnd)
                //                {
                //                    _SetLog("Tr success result");

                //                    //m_LogVal.iStep = m_iStep;
                //                    //m_LogVal.sMsg = "m_TSendDelay && goto 19 Step";
                //                    //SaveLog();

                //                    m_iStep = 19;        // 배출 완료 여부를 다시 묻는다.
                //                    return false;
                //                }
                //            }
                //            else      // 30초 초과
                //            {
                //                m_bDuringGOut           = false;
                //                //CTcpIp.It.bSuccess_Du   = false;
                //                //CTcpIp.It.bSuccess_Ok   = false;
                //                //CTcpIp.It.bSuccess_Fail = false;
                //                CQcVisionCom.rcvQCSendEnd = false;
                //                _SetLog("Error : Emit success query timeout from QC");

                //                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                //                m_iStep = 0;
                //                return false;
                //            }
                //        }

                //        m_iStep = 22;           // 수신 결과 처리
                //        return false;
                //    }

                //case 21:
                //    {
                //        if(!m_Delay.Chk_Delay())
                //        {
                //            if(!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail) return false;
                //            else 
                //            {
                //                CTcpIp.It.SendSucces();
                //                m_LogVal.iStep = m_iStep;
                //                m_LogVal.sMsg  = "Qc recmd Send Ok";
                //                SaveLog();

                //                m_iStep++;
                //                return false;
                //            }
                //        }
                //        else 
                //        {
                //            m_bDuringFOut           = false;
                //            CTcpIp.It.bEmit_Ok      = false;
                //            CTcpIp.It.bEmit_Fail    = false;
                //            CTcpIp.It.bSuccess_Ok   = false;
                //            CTcpIp.It.bSuccess_Fail = false;
                //            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                //            m_LogVal.iStep = m_iStep;
                //            m_LogVal.sMsg  = "Not Qc recmd ";
                //            SaveLog();

                //            m_iStep = 0;
                //            return false;
                //        }
                //    }

                //case 22:
                //    {
                //        //if(CTcpIp.It.bSuccess_Ok)    // 배출 성공
                //        if(CQcVisionCom.rcvQCSendEnd)
                //        {
                //            m_bDuringFOut = false;

                //            //int SlotNum = GetTopEmptySlot();
                //            //CData.Parts[(int)EPart.OFR].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;

                //            //190731 ksg :
                //            //----------------
                //            try
                //            {
                //                //CData.Parts[(int)EPart.OFR].sLotName = CTcpIp.It.sStringInfo[0];
                //                //CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt32(CTcpIp.It.sStringInfo[1]);
                //                //CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt32(CTcpIp.It.sStringInfo[2]);

                //                CData.Parts[(int)EPart.OFR].sLotName = CQcVisionCom.sStringInfo[0];
                //                CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt32(CQcVisionCom.sStringInfo[1]);
                //                CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt32(CQcVisionCom.sStringInfo[2]);
                //            }
                //            catch
                //            {
                //                CData.Parts[(int)EPart.OFR].sLotName = "Exception";
                //                CData.Parts[(int)EPart.OFR].iMGZ_No = 0;
                //                CData.Parts[(int)EPart.OFR].iSlot_No = 0;
                //            }

                //            //CData.Parts[(int)EPart.OFR].group  = CTcpIp.It.sStringInfo[3]; <-Group 없음
                //            //CData.Parts[(int)EPart.OFR].device = CTcpIp.It.sStringInfo[4]; <-Deivce 없음
                //            //----------------
                //            //CData.LotInfo.iTOutCnt++; //190407 ksg : //20210122
                //            //CTcpIp.It.bResultFail = false;

                //            CQcVisionCom.rcvResultQuery = false;
                //            CQcVisionCom.rcvQCReadyQueryNG = false;
                //            m_EndSeq  = DateTime.Now         ; //190807 ksg :
                //            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                //           _SetLog("SendEnd <- QC");

                //            // 더이상 Emit Retry는 없다.
                //            //CTcpIp.It.bEmit_Ok = false;        // 이 Flag가 false가 되면 Retry 대기는 없다.
                //            //CTcpIp.It.bEmit_Fail = false;

                //            CQcVisionCom.rcvQCSendOutGOOD = false;
                //            CQcVisionCom.rcvQCSendOutNG = false;

                //            //200316 ksg :
                //            if (!CData.Opt.bQcByPass)                     // QC Vision Bypass가 아닌경우 
                //            {
                //                //CTcpIp.It.bTResult_Ok = false;
                //                //CTcpIp.It.SendTestResult();               // 결과를 받아온다.
                //                CQcVisionCom.rcvResultQuery = false;
                //                CGxSocket.It.SendMessage("ResultQuery");

                //                m_Delay     .Set_Delay(10000);
                //                m_TSendDelay.Set_Delay(1000 );

                //                m_iStep++; //200316 ksg :
                //                return false;
                //            }
                //            else
                //            {
                //                _SetLog("QC Bypass, Top Receive Done");
                //                m_iStep = 0;
                //                return true;                             // 2020-11-20, jhLee : 배출 성공으로 끝낸다.
                //            }
                //        }

                //        // 배출 실패
                //        // Emit 명령 반복 수행 필요

                //        _SetLog("Error : Emit success Fail <- QC");

                //        m_bDuringFOut           = false;

                //        CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                //        m_iStep = 0;
                //        return false;
                //    }
                ////200316 ksg :

                //case 23: // QC에게 Test 결과를 묻는다.
                //    {
                //        //if(!m_Delay.Chk_Delay() && !CTcpIp.It.bTResult_Ok)
                //        if (!m_Delay.Chk_Delay() && !CQcVisionCom.rcvResultQuery)
                //        {
                //            if(m_TSendDelay.Chk_Delay())
                //            {
                //                //CTcpIp.It.SendTestResult();
                //                CGxSocket.It.SendMessage("ResultQuery");
                //                _SetLog("Retry Test result query -> QC");

                //                m_TSendDelay.Set_Delay(1000);
                //                return false;
                //            }
                //            else
                //            {
                //                return false;
                //            }
                //        }
                //        //else if(CTcpIp.It.bTResult_Ok)
                //        else if(CQcVisionCom.rcvResultQuery)
                //        {
                //            m_iStep = 0;
                //            return true;
                //        }
                //        else
                //        {
                //            _SetLog("Error : QC Vision Test result receive fail");

                //            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                //            m_iStep = 0;
                //            return true;
                //        }
                //    }
            }
        }

        /// <summary>
        /// LotEnd버튼 클릭시 매거진 버리는 시퀀스
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PlaceLotEnd()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADER_PLACE_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "Ofl_CylPlaceLotEnd";

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;

                        return true;
                    }

                case 10:
                    {
                        if (!Get_BtmMgzChk())
                        {//Btm 매거진 감지 안됄 경우
                            m_iStep = 23;
                            return false;
                        }

                        if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
                        {
                            if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                _SetLog("Error : Strip-out detect.");

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                                CMot.It.EStop(m_iDX);

                                CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                                _SetLog("Error : Pusher overload.");

                                CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                                m_iStep = 0;
                                return true;
                            }
                            // 2021.11.18 SungTae End

                            //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                            if (!(CDataOption.UseQC && CData.Opt.bQcUse) && CMot.It.Get_FP(m_iDX) > 5)
                            {
                                CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                                m_iStep = 0;
                                return true;
                            }
                            //

                        }
                        else
                            ClearMgzReadyFlag();    // 2021-02-05, jhLee : Magazine이 준비되었다는 flag를 Clear 한다.


                        // 2021-02-02, jhLee : QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        if (QcVisionPermission() == 1)                      // 간섭 발생
                        {
                            if (m_tmSafety.Chk_Delay())                     // 일정 주기로 QC 상태를 조회한다.
                            {
                                if (++m_nSafetyCount > CNT_QCSafetyCheck)                // 일정 횟수만큼 계속 간섭중이라면 알람을 일으켜 확인을 요청한다.
                                {
                                    m_nSafetyCount = 0;

                                    _SetLog("Error : QC Strip-out detect at long time");
                                    CErr.Show(eErr.QC_DETECTED_STRIP);

                                    m_iStep = 0;
                                    return true;            // 실행 취소
                                }

                                _SetLog("Check Safety Interlock Fail from QC");
                                m_tmSafety.Set_Delay(TIME_QCSafetyCheck);                 // 3초 주기

                                CGxSocket.It.SendMessage("StatusQuery");    // 상태 요청

                                _SetLog("[SEND](EQ->QC) StatusQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                            }
                            return false;       // 반복
                        }//of if QC Not Safety

                        Act_BtmClampOC(m_ActClose);
                        Belt_Btm(m_BeltStop);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampOC(m_ActClose), Belt_Btm(m_BeltStop)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        if (!Act_BtmClampOC(m_ActClose))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActDn);
                        Act_TopClampModuleDU(m_ActUp);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampModuleDU(m_ActDn), Act_TopClampModuleDU(m_ActUp)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Act_BtmClampModuleDU(m_ActDn))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPlaceGo);

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetectFull))
                        {
                            //set Error
                            CErr.Show(eErr.OFFLOADER_BTM_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))
                        { Belt_Btm(m_BeltRun); }

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltRun)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))
                        { Belt_Btm(m_BeltRun); }
                        else
                        { Belt_Btm(m_BeltStop); }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPlaceGo))
                        { return false; }

                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        { Belt_Btm(m_BeltRun); }
                        else
                        { Belt_Btm(m_BeltStop); }

                        if (Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Btm(m_BeltStop);

                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect) || !CIO.It.Get_X(eX.OFFL_BtmMGZDetectFull))
                        {
                            CErr.Show(eErr.OFFLOADER_BTM_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        m_LogVal.iStep = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Place);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Y_Place;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Place))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BUnClamp);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BUnClamp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BUnClamp))
                        { return false; }

                        Act_BtmClampOC(m_ActOpen);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampOC(m_ActOpen)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!Act_BtmClampOC(m_ActOpen))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPlaceDn);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BPlaceDn;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPlaceDn))
                        { return false; }
                        if(!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))
                        {
                            Belt_Btm(m_BeltRun);
                            return false;
                        }
                        else
                        {
                            Belt_Btm(m_BeltStop);
                        }
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        SetAllStatusMgz(eSlotInfo.Empty);

                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        else CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        m_LogVal.sMsg = "CData.Parts[(int)EPart.OFL].iStat = " + CData.Parts[(int)EPart.OFL].iStat.ToString() + ", m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect) && !m_Delay.Chk_Delay())
                        { return false; }

                        m_LogVal.iStep = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        Belt_Btm(m_BeltRun);

                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Btm(m_BeltStop);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltStop)";
                        SaveLog();

                        m_iStep = 24;
                        return false;
                    }

                case 23:
                    {//case 10에서 매거진 없을 경우, Y축 Wait 위치 로 이동
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//btm 매거진 버리는 구문, top 매거진 유무 검사
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CData.Parts[(int)EPart.OFL].iStat = ESeq.Idle;

                        CData.sLastLot = CData.Parts[(int)EPart.OFL].sLotName;
                        CData.iLastMGZ = CData.Parts[(int)EPart.OFL].iMGZ_No;

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltStop)";
                        SaveLog();

                        if(!Get_TopMgzChk())
                        {//Top 매거진 없을 경우 종료 
                            CData.Parts[(int)EPart.OFR].iStat = ESeq.Idle;
                            m_iStep = 0;
                            CSQ_Man.It.bOffMGZBtmPlaceDone = true;

                            return true;
                        }

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//top 매거진 있을 경우
                        Act_TopClampOC(m_ActClose);
                        Belt_Top(m_BeltStop);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_TopClampOC(m_ActClose), Belt_Top(m_BeltStop)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {
                        if (!Act_TopClampOC(m_ActClose))
                        { return false; }

                        Act_BtmClampModuleDU(m_ActUp);
                        Act_TopClampModuleDU(m_ActUp);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampModuleDU(m_ActUp), Act_TopClampModuleDU(m_ActUp)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {
                        if (!Act_BtmClampModuleDU(m_ActUp))
                        { return false; }

                        if (!Act_TopClampModuleDU(m_ActUp))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPlaceGo);

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.OFFLOADER_TOP_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        { Belt_Top(m_BeltRun); }

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Top(m_BeltRun)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 29:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        {
                            Belt_Top(m_BeltRun);
                            return false;
                        }
                        else
                        { Belt_Top(m_BeltStop); }

                        if (Get_TopMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPlaceGo))
                        { return false; }

                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 30:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        {
                            Belt_Top(m_BeltRun);
                            return false;
                        }
                        else
                        { Belt_Top(m_BeltStop); }

                        if (Get_TopMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Top(m_BeltStop);

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect) || !CIO.It.Get_X(eX.OFFL_TopMGZDetectFull))
                        {
                            CErr.Show(eErr.OFFLOADER_TOP_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        m_LogVal.iStep = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 31:
                    {
                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Place);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Y_Place;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 32:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Place))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TUnClamp);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_TUnClamp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 33:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TUnClamp))
                        { return false; }

                        Act_TopClampOC(m_ActOpen);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_TopClampOC(m_ActOpen)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 34:
                    {
                        if (!Act_TopClampOC(m_ActOpen))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPlaceDn);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_TPlaceDn;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 35:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPlaceDn))
                        { return false; }
                        
						if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        {
                            Belt_Top(m_BeltRun);
                            return false;
                        }
                        else
                        {
                            Belt_Top(m_BeltStop);
                        }
						
                        Belt_Top(m_BeltRun);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        
                        SetTopAllStatusMgz(eSlotInfo.Empty);

                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFR].iSlot_No = CData.Parts[(int)EPart.OFR].iSlot_info.Length - 1;
                        else CData.Parts[(int)EPart.OFR].iSlot_No = 0;

                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        m_LogVal.sMsg = "CData.Parts[(int)EPart.OFL].iStat = " + CData.Parts[(int)EPart.OFR].iStat.ToString() + ", m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 36:
                    {
                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect) && !m_Delay.Chk_Delay())
                        { return false; }

                        //m_Delay.Set_Delay(); //옵션 Delay 값 넣기
                        m_LogVal.iStep = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 37:
                    {
                        Belt_Top(m_BeltRun);

                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Top(m_BeltStop);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltStop)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 38:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Top(m_BeltStop)";
                        SaveLog();

                        CData.Parts[(int)EPart.OFR].iStat = ESeq.Idle;

                        CData.sLastLot = CData.Parts[(int)EPart.OFL].sLotName;
                        CData.iLastMGZ = CData.Parts[(int)EPart.OFL].iMGZ_No;
                        CSQ_Man.It.bOffMGZTopPlaceDone = true;

                        m_iStep = 0;
                        return true;
                    }

            }
        }


        // 2022.04.19 SungTae Start : [추가] ASE-KH Magazine ID Reading
        /// <summary>
        /// Offloader Check Barcode 버튼 클릭했을 때 시퀀스
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
                    CErr.Show(eErr.OFFLOADER_MGZ_ID_READ_TIMEOUT);
                    _SetLog("Error : OFFLOADER_MGZ_ID_READ_TIMEOUT");

                    m_iStep = 0;
                    return true;
                }
            }

            m_bxChkLight = CIO.It.Get_X(eX.OFFL_LightCurtain);

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
                        if (m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) || !CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2))
                        {
                            //Mgz 둘중 하나라도 감지 되고 있는지 확인(or 검사)
                            CErr.Show(eErr.OFFLOADER_TOP_NOT_DETECT_MGZ_ERROR);
                            _SetLog("Error : OFL Top not detect magazine.");
                            
                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {
                            CErr.Show(eErr.DRY_DETECTED_STRIP);
                            _SetLog("Error : Strip-out detect.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                        {//B접 (푸셔 오버로드 감지 되었을때 에러 발생)
                            CMot.It.EStop(m_iDX);

                            CErr.Show(eErr.DRY_PUSHER_OVERLOAD_ERROR);
                            _SetLog("Error : Pusher overload.");

                            CMot.It.Mv_N(m_iDX, CData.SPos.dDRY_X_Wait);

                            m_iStep = 0;
                            return true;
                        }

                        if (!(CDataOption.UseQC && CData.Opt.bQcUse) && CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }

                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr = ""; //BCR 값 초기화

                        // Z축 MGZ ID Reading Position 이동 확인 및 Y축 MGZ ID Reading Position 이동
                        if (CMot.It.Get_Mv(m_iZ, CData.Dev.dOffL_Z_MgzBcr) && CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_MgzBcr))
                        {
                            // Y & Z축 모두 MGZ ID Reading Position면 BCR Reading Step으로 이동
                            m_iStep = 16;
                            return false;
                        }

                        _SetLog("Check axes.");

                        m_iStep++;
                        return false;
                    }

                case 11:    //Top Belt Stop & Top Mgz Clamp Close
                    {
                        
                        Act_TopClampOC(m_ActClose);
                        Belt_Top(m_BeltStop);

                        _SetLog("Top clamp close, Top belt stop");

                        m_iStep++;
                        return false;
                    }

                case 12:    //Top Mgz Clamp Close 확인
                    {
                        if (!Act_TopClampOC(m_ActClose))    { return false; }

                        //Y축 대기위치 이동
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog($"Y axis move wait. Pos : {CData.Dev.dOffL_Y_Wait}mm");

                        m_iStep++;
                        return false;
                    }

                case 13:    // Y축 대기위치 이동 확인, Z축 MGZ ID Reading Position 이동
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))  { return false; }
                        
                        CMot.It.Mv_N(m_iZ, CData.Dev.dOffL_Z_MgzBcr);

                        _SetLog($"Z axis move BCR read pos. Pos : {CData.Dev.dOffL_Z_MgzBcr}mm");

                        m_iStep++;
                        return false;
                    }

                case 14:    // Z축 MGZ ID Reading Position 이동 확인 및 Y축 MGZ ID Reading Position 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffL_Z_MgzBcr)) { return false; }

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_MgzBcr);

                        _SetLog($"Z axis move check & Y axis move BCR read pos. Pos : {CData.Dev.dOffL_Y_MgzBcr}mm");

                        m_iStep++;
                        return false;
                    }

                case 15:    // Y축 바코드 위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_MgzBcr))    { return false; }

                        //////////////////////////////////////////////
                        m_iReReadCnt = 0; //바코드 판독 시도 회수 초기화
                        //////////////////////////////////////////////

                        _SetLog("Y axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 16:    //////////// 반복 시작 ////////////
                    {
                        if (m_iReReadCnt > 0)
                        {
                            // 재시도인 경우 바코드 읽기 중지(Timing OFF)
                            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LOFF);
                            
                            _SetLog("MGZ BCR timing off.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr = ""; //BCR 값 초기화

                        //바코드 읽기 요청 (Timing ON)
                        CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LON);

                        m_Delay.Set_Delay(3000);
                        _SetLog("MGZ BCR read start.  Set delay : 3000ms");

                        m_iStep++;
                        return false;
                    }

                case 18:    // 바코드 읽기 완료 체크
                    {
                        if (!m_Delay.Chk_Delay())
                        {
                            return false;
                        }

                        // 바코드 읽기 완료 체크
                        if (!CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr.Equals(""))
                        {
                            _SetLog($"MGZ BCR read ok.  MGZ ID : {CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr}");
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
                                CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LOFF); //바코드 읽기 중지(Timing OFF)

                                _SetLog("MGZ BCR read fail.");

                                CMsg.Show(eMsg.Error, "OffLoader magazine BCR read fail", "1.Check the direction of the magazine.\r2.Please confirm that BCR module works normally.");
                                return true;
                            }
                        }

                        m_iStep++;
                        return false;
                    }

                case 19:    // Finish
                    {
                        CData.Parts[(int)EPart.OFL].iStat = ESeq.Idle;
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }
        // 2022.04.19 SungTae End

        #endregion

        #region Function
        #region Top & Btm GetWork Pos
        /// <summary>
        /// OffLoader Get Bottom Work Pos
        /// </summary>
        /// <returns></returns>
        public double GetBtmWorkPos()
        {
            double dPos;
            int iCnt;
            /*
            for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
            {
                if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                { break; }
            }

            dPos = CData.Dev.dOffL_Z_BRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
            */

            // 2021.12.09 SungTae Start : [수정] (Qorvo향 VOC)
            int /*iPart, */iTarget;

            if (CData.Opt.bFirstTopSlotOff)
            {
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    iTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iMgzCnt - CData.Dev.iOffMgzCnt : 0;

                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= iTarget; iCnt--)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= 0; iCnt--)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    iTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iOffMgzCnt : CData.Dev.iMgzCnt;

                    for (iCnt = 0; iCnt < iTarget; iCnt++)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
            }
            
            dPos = CData.Dev.dOffL_Z_BRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
            // 2020.12.09 SungTae End

            // 2021.12.09 SungTae : [추가]
            _SetLog($">>> GetBtmWorkPos() - Part : {EPart.OFL}, Slot Num : {iCnt}, Z-Pos : {dPos} mm");

            return dPos;
        }

        /// <summary>
        /// OffLoader Get Top Work Pos
        /// </summary>
        /// <returns></returns>
        public double GetTopWorkPos()
        {
            double dPos;
            int iCnt;

            /*
            for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
            {
                if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                { break; }
            }

            dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
            */

            // 2021.12.09 SungTae Start : [수정] (Qorvo향 VOC)
            int iPart, iTarget;

            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
            {
                if (!CData.Opt.bOfLMgzUnloadType)
                {
                    // QC 판정에 따라 TOP or BTM Mgz에 분리해서 적재할 경우
                    iPart = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;
                }
                else
                {
                    // QC 판정에 상관 없이 BTM Mgz 하나에 적재할 경우
                    iPart = (int)EPart.OFL;
                }
            }
            else
            {
                // QC 판정에 따라 TOP or BTM Mgz에 분리해서 적재할 경우
                iPart = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;
            }

            if (CData.Opt.bFirstTopSlotOff)
            {
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    iTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iMgzCnt - CData.Dev.iOffMgzCnt : 0;

                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= iTarget; iCnt--)
                    {
                        if (CData.Parts[iPart].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= 0; iCnt--)
                    {
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if (CData.Parts[iPart].iSlot_info[iCnt] == (int)eSlotInfo.Empty) //if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)) /*&&
                    !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)*/
                {
                    iTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iOffMgzCnt : CData.Dev.iMgzCnt;

                    for (iCnt = 0; iCnt < iTarget; iCnt++)
                    {
                        if (CData.Parts[iPart].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
                    {
                        if (CData.Parts[iPart].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        {
                            break;
                        }
                    }   
                }   
            }
            
            dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
            // 2020.10.27 SungTae End

            // 2021.12.09 SungTae : [추가]
            _SetLog($">>> GetTopWorkPos() - Part : {EPart.OFL}, Slot Num : {iCnt}, Z-Pos : {dPos} mm");

            return dPos;
        }
        #endregion

        #region GetEmptySlot
        /// <summary>
        /// OffLoader Bottom MGZ Get Empty Slot
        /// </summary>
        /// <returns></returns>
        public int GetBtmEmptySlot()
        {
            int iCnt;
            /*
            for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
            {
                if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                { break; }
            }
            */
            //if(CData.Opt.bFirstTopSlot)
            if(CData.Opt.bFirstTopSlotOff) //190511 ksg :
            {
                for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= 0; iCnt--)
                {
                    if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                    { break; }
                }
            }
            else
            {
                for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
                {
                    if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                    { break; }
                }
            }

            // 2021.12.09 SungTae : [추가]
            _SetLog($">>> GetBtmEmptySlot() : Return Slot Num = {iCnt}");

            return iCnt;
        }

        /// <summary>
        /// OffLoader Bottom MGZ Get Empty Slot
        /// </summary>
        /// <returns></returns>
        public int GetTopEmptySlot()
        {
            int iCnt;
            /*
            for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
            {
                if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                { break; }
            }
            */
            //if(CData.Opt.bFirstTopSlot)
            if(CData.Opt.bFirstTopSlotOff) //190511 ksg :
            {
                for (iCnt = CData.Dev.iMgzCnt; iCnt >= 0; iCnt--)
                {
                    if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                    { break; }
                }
            }
            else
            {
                for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
                {
                    if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                    { break; }
                }
            }

            // 2021.12.09 SungTae : [추가]
            _SetLog($">>> GetTopEmptySlot() : Return Slot Num = {iCnt}");

            return iCnt;
        }
        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------//
        // 2021.12.08 SungTae Start : [수정] (Qorvo향 VOC) 
        #region GetFullSlotCheck
        /// <summary>
        /// 현재 매거진의 슬롯이 Full 인지 확인
        /// </summary>
        /// <returns></returns>
        private bool GetFullSlotCheck()
        {
            bool bRet = true;

            // 2021.12.09 SungTae Start : [수정] (Qorvo향 VOC)
            int iPart, iSlot, nTarget;
            string sMsg = "";

            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
            {
                if (!CData.Opt.bOfLMgzUnloadType)
                {
                    // QC 판정에 따라 TOP or BTM Mgz에 분리해서 적재할 경우
                    iPart = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;
                }
                else
                {
                    // QC 판정에 상관 없이 BTM Mgz 하나에 적재할 경우
                    iPart = (int)EPart.OFL;
                }
            }
            else
            {
                // QC 판정에 따라 TOP or BTM Mgz에 분리해서 적재할 경우
                iPart = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;
            }

            if (CData.Opt.bFirstTopSlotOff)
            {
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    // TOP -> DOWN
                    nTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iMgzCnt - CData.Dev.iOffMgzCnt : 0;

                    for (iSlot = CData.Dev.iMgzCnt - 1; iSlot >= nTarget; iSlot--)
                    {                       
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty)
                        { 
                            bRet = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (iSlot = CData.Dev.iMgzCnt - 1; iSlot >= 0; iSlot--)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                // BOTTOM -> UP
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    nTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iOffMgzCnt : CData.Dev.iMgzCnt;

                    for (iSlot = 0; iSlot < nTarget; iSlot++)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty/*0*/)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (iSlot = 0; iSlot < CData.Dev.iMgzCnt; iSlot++)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty/*0*/)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
            }

            sMsg = (CData.Opt.bFirstTopSlotOff == true) ? "[TOP->DOWN]" : "[BOTTOM->UP]";

            //_SetLog($"Strip Unloading Mode : {sMsg}.  GetFullSlotCheck() - Part : {iPart}, Slot No. : {iSlot}, Slot Info = {CData.Parts[iPart].iSlot_info[iSlot]} ");
            // 2021.12.09 SungTae End

            return bRet;
        }

        /// <summary>
        /// OffLoader MGZ Get Full Slot
        /// </summary>
        /// <returns></returns>
        private bool GetTopFullSlotCheck()
        {
            bool bRet = true;

            // 2021.12.09 SungTae Start : [수정] (Qorvo향 VOC)
            int iPart, iSlot, nTarget;
            string sMsg = "";

            if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                CData.Opt.bOfLMgzUnloadType)
            {
                // QC 판정에 상관 없이 BTM Mgz 하나에 적재할 경우
                iPart = (int)EPart.OFL;
            }
            else
            {
                // QC 판정에 따라 TOP or BTM Mgz에 분리해서 적재할 경우
                iPart = (CDataOption.UseQC && CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;
            }

            if (CData.Opt.bFirstTopSlotOff)
            {
                // TOP -> DOWN
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    nTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iMgzCnt - CData.Dev.iOffMgzCnt : 0;

                    for (iSlot = CData.Dev.iMgzCnt - 1; iSlot >= nTarget; iSlot--)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (iSlot = CData.Dev.iMgzCnt - 1; iSlot >= 0; iSlot--)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                // BOTTOM -> UP
                if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                {
                    nTarget = (CData.Dev.iOffMgzCnt != 0) ? CData.Dev.iOffMgzCnt : CData.Dev.iMgzCnt;

                    for (iSlot = 0; iSlot < nTarget; iSlot++)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (iSlot = 0; iSlot < CData.Dev.iMgzCnt; iSlot++)
                    {
                        if (CData.Parts[iPart].iSlot_info[iSlot] == (int)eSlotInfo.Empty)
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
            }

            sMsg = (CData.Opt.bFirstTopSlotOff == true) ? "[TOP->DOWN]" : "[BOTTOM->UP]";

            //_SetLog($"Strip Unloaing Mode : {sMsg}.  GetTopFullSlotCheck() - Part : {iPart}, Slot No. : {iSlot}, Slot Info = {CData.Parts[iPart].iSlot_info[iSlot]} ");
            // 2021.12.09 SungTae End

            return bRet;
        }
        // 2021.12.08 SungTae End
        //---------------------------------------------------------------------------------------------------------------------------------------------//

        //
        // 2021-07-13, jhLee, Multi-LOT

        //
        // 현재 Mgz에 수납된 Slot의 수량을 회신한다.
        // EPart.OFL : Bottom
        // EPart.OFR : Top
        //
        public int GetStripCountMgz(int nPart)
        {
            int nCount = 0;

            for (int i = 0; i < CData.Dev.iOffMgzCnt; i++)
            {
                if (CData.Parts[nPart].iSlot_info[i] == 1)
                {
                    nCount++;
                }
            }

            return nCount;
        }

        /// <summary>
        /// 210309 pjh : Set up strip 사용 시 Strip이 최대 수량 만큼 투입 되었는지 확인 하는 함수 
        /// </summary>
        /// <returns></returns>
        public bool GetAutoLoadStopMaxStripCheck()
        {//210309 pjh : Strip 투입/배출 갯수 비교
            bool bRet = false;

            //if(CData.Opt.iAutoLoadingStopCount != 0 && CDataOption.UseAutoLoadingStop && CData.CurCompany == ECompany.SkyWorks)
            if (CData.Opt.iAutoLoadingStopCount != 0 && CDataOption.UseSetUpStrip)
            {
                if (CData.LotInfo.iTInCnt == CData.Opt.iAutoLoadingStopCount && Get_StripNone())
                {
                    if (CData.LotInfo.iTInCnt == CData.LotInfo.iTOutCnt)
                    {
                        bChkInOutCnt = true;
                    }
                    else
                    {
                        bChkInOutCnt = false;
                    }
                    bRet = true;
                }
            }
            return bRet;
        }
        #endregion

        #region SetStatusMgz
        /// <summary>
        /// OffLoader Bottom MGZ Set Status
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="Status"></param>
        private void SetStatusMgz(int iNum, eSlotInfo eStatus)
        {
            CData.Parts[(int)EPart.OFL].iSlot_info[iNum] = (int)eStatus;
        }

        /// <summary>
        /// OffLoader Top MGZ Set Status
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="Status"></param>
        private void SetTopStatusMgz(int iNum, eSlotInfo eStatus)
        {
            //20200513 jym : QC 외에 Top 사용 추가
            if (CDataOption.UseQC && CData.Opt.bQcUse)
            { CData.Parts[(int)EPart.OFR].iSlot_info[iNum] = (int)eStatus; }
            else
            { CData.Parts[(int)EPart.OFL].iSlot_info[iNum] = (int)eStatus; }
        }
        #endregion

        #region SetAllStatusMgz
        /// <summary>
        /// OffLoader Bottom All MGZ Set Status
        /// </summary>
        /// <param name="status"></param>
        private void SetAllStatusMgz(eSlotInfo estatus)
        {
            for (int i = 0; i < CData.Dev.iMgzCnt; i++)
            {
                SetStatusMgz(i, estatus);
            }
        }

        /// <summary>
        /// OffLoader Top All MGZ Set Status
        /// </summary>
        /// <param name="status"></param>
        private void SetTopAllStatusMgz(eSlotInfo estatus)
        {
            for (int i = 0; i < CData.Dev.iMgzCnt; i++)
            {
                SetTopStatusMgz(i, estatus);
            }
        }
        #endregion

        #region OFL Cylinder 
        /// <summary>
        /// OffLoader Top MGZ Clamp OnOff
        /// /// 로더 클램프 true = Close, false = Open
        /// 리턴 true : 동작 완료, false : 동작 미완료
        /// </summary>
        /// <param name="Up"></param>
        /// <returns></returns>
        public bool Act_TopClampOC(bool bClose)
        {
            bool bRet;
            CIO.It.Set_Y(eY.OFFL_TopClampOn ,  bClose);    //YC4
            CIO.It.Set_Y(eY.OFFL_TopClampOff, !bClose);    //YC5

            if (bClose) { bRet = ( CIO.It.Get_X(eX.OFFL_TopClampOn)) && (!CIO.It.Get_X(eX.OFFL_TopClampOff)); }   //XC4
            else        { bRet = (!CIO.It.Get_X(eX.OFFL_TopClampOn)) && ( CIO.It.Get_X(eX.OFFL_TopClampOff)); }   //XC5

            return bRet;
        }

        public bool Func_TopClamp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_TopClampOn, true);    
            CIO.It.Set_Y(eY.OFFL_TopClampOff, false);        
            bRet = (CIO.It.Get_X(eX.OFFL_TopClampOn)) && (!CIO.It.Get_X(eX.OFFL_TopClampOff));

            return bRet;
        }

        public bool Func_TopUnclamp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_TopClampOn, false);   
            CIO.It.Set_Y(eY.OFFL_TopClampOff, true);       
            bRet = (!CIO.It.Get_X(eX.OFFL_TopClampOn)) && (CIO.It.Get_X(eX.OFFL_TopClampOff));

            return bRet;
        }

        /// <summary>
        /// OffLoader Top MGZ UpDown
        /// </summary>
        /// <param name="Up"></param>
        /// <returns></returns>
        public bool Act_TopClampModuleDU(bool bUp)
        {
            bool bRet;
            CIO.It.Set_Y(eY.OFFL_TopMGZUp,  bUp);    //YC2
            CIO.It.Set_Y(eY.OFFL_TopMGZDn, !bUp);    //YC3

            if (bUp) { bRet = ( CIO.It.Get_X(eX.OFFL_TopMGZUp)) && (!CIO.It.Get_X(eX.OFFL_TopMGZDn)); }   //XC2
            else     { bRet = (!CIO.It.Get_X(eX.OFFL_TopMGZUp)) && ( CIO.It.Get_X(eX.OFFL_TopMGZDn)); }   //XC3

            return bRet;
        }

        public bool Func_TopModUp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_TopMGZUp, true);   
            CIO.It.Set_Y(eY.OFFL_TopMGZDn, false);                                                                
            bRet = (CIO.It.Get_X(eX.OFFL_TopMGZUp)) && (!CIO.It.Get_X(eX.OFFL_TopMGZDn)); 

            return bRet;
        }

        public bool Func_TopModDown()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_TopMGZUp, false);
            CIO.It.Set_Y(eY.OFFL_TopMGZDn, true);
            bRet = (!CIO.It.Get_X(eX.OFFL_TopMGZUp)) && (CIO.It.Get_X(eX.OFFL_TopMGZDn));

            return bRet;
        }

        /// <summary>
        /// OffLoader Bottom MGZ Clamp OnOff
        /// </summary>
        /// <param name="Up"></param>
        /// <returns></returns>
        public bool Act_BtmClampOC(bool bClose)
        {
            bool bRet;
            CIO.It.Set_Y(eY.OFFL_BtmClampOn,   bClose);    //YCC
            CIO.It.Set_Y(eY.OFFL_BtmClampOff, !bClose);    //YCD

            if (bClose) { bRet = ( CIO.It.Get_X(eX.OFFL_BtmClampOn)) && (!CIO.It.Get_X(eX.OFFL_BtmClampOff)); }    //XCC
            else        { bRet = (!CIO.It.Get_X(eX.OFFL_BtmClampOn)) && ( CIO.It.Get_X(eX.OFFL_BtmClampOff)); }    //XCD

            return bRet;
        }

        public bool Func_BtmClamp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_BtmClampOn, true);
            CIO.It.Set_Y(eY.OFFL_BtmClampOff, false);
            bRet = (CIO.It.Get_X(eX.OFFL_BtmClampOn)) && (!CIO.It.Get_X(eX.OFFL_BtmClampOff));

            return bRet;
        }

        public bool Func_BtmUnclamp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_BtmClampOn, false);
            CIO.It.Set_Y(eY.OFFL_BtmClampOff, true);
            bRet = (!CIO.It.Get_X(eX.OFFL_BtmClampOn)) && (CIO.It.Get_X(eX.OFFL_BtmClampOff));

            return bRet;
        }

        /// <summary>
        /// OffLoader Bottom MGZ UpDown
        /// </summary>
        /// <param name="Up"></param>
        /// <returns></returns>
        public bool Act_BtmClampModuleDU(bool bUp)
        {
            bool bRet;

            CIO.It.Set_Y(eY.OFFL_BtmMGZUp,  bUp);    //YCA
            CIO.It.Set_Y(eY.OFFL_BtmMGZDn, !bUp);    //YCB

            if (bUp) { bRet = ( CIO.It.Get_X(eX.OFFL_BtmMGZUp)) && (!CIO.It.Get_X(eX.OFFL_BtmMGZDn)); } //XCA
            else     { bRet = (!CIO.It.Get_X(eX.OFFL_BtmMGZUp)) && ( CIO.It.Get_X(eX.OFFL_BtmMGZDn)); } //XCB

            return bRet;
        }

        public bool Func_BtmModUp()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_BtmMGZUp, true);
            CIO.It.Set_Y(eY.OFFL_BtmMGZDn, false);
            bRet = (CIO.It.Get_X(eX.OFFL_BtmMGZUp)) && (!CIO.It.Get_X(eX.OFFL_BtmMGZDn));

            return bRet;
        }

        public bool Func_BtmModDown()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFL_BtmMGZUp, false);
            CIO.It.Set_Y(eY.OFFL_BtmMGZDn, true);
            bRet = (!CIO.It.Get_X(eX.OFFL_BtmMGZUp)) && (CIO.It.Get_X(eX.OFFL_BtmMGZDn));

            return bRet;
        }
        #endregion

        #region Belt Move
        /// <summary>
        /// OffLoader Top Belt Run
        /// </summary>
        /// <param name="bRun"></param>
        public void Belt_Top(bool bRun)
        {
            CIO.It.Set_Y(eY.OFFL_TopBeltRun, bRun);    //YB8
            CIO.It.Set_Y(eY.OFFL_BtnBeltTopRun, bRun);
        }

        public bool Func_TopBeltRun()
        {
            CIO.It.Set_Y(eY.OFFL_BtnBeltTopRun, true);
            return CIO.It.Set_Y(eY.OFFL_TopBeltRun, true);
        }

        public bool Func_TopBeltStop()
        {
            CIO.It.Set_Y(eY.OFFL_BtnBeltTopRun, false);
            return CIO.It.Set_Y(eY.OFFL_TopBeltRun, false);
        }

        /// <summary>
        /// OffLoader Mid Belt Run
        /// </summary>
        /// <param name="bRun"></param>
        /// <param name="bCw"></param>
        public void Belt_Mid(bool bRun, bool bCw)
        {
            CIO.It.Set_Y(eY.OFFL_MidBeltCCW, bCw );    //YBC
            CIO.It.Set_Y(eY.OFFL_MidBeltRun, bRun);    //YBB
        }

        public bool Func_MidBeltRunCW()
        {
            CIO.It.Set_Y(eY.OFFL_MidBeltCCW, true);
            return CIO.It.Set_Y(eY.OFFL_MidBeltRun, true);
        }

        public bool Func_MidBeltRunCCW()
        {
            CIO.It.Set_Y(eY.OFFL_MidBeltCCW, false);
            return CIO.It.Set_Y(eY.OFFL_MidBeltRun, true);
        }

        public bool Func_MidBeltStop()
        {
            return CIO.It.Set_Y(eY.OFFL_MidBeltRun, false);
        }

        /// <summary>
        /// OffLoader Bottom Belt Run
        /// </summary>
        /// <param name="bRun"></param>
        public void Belt_Btm(bool bRun)
        {
            CIO.It.Set_Y(eY.OFFL_BtmBeltRun, bRun);     //YBE
            CIO.It.Set_Y(eY.OFFL_BtnBeltBtmRun, bRun);
        }

        public bool Func_BtmBeltRun()
        {
            CIO.It.Set_Y(eY.OFFL_BtnBeltBtmRun, true);
            return CIO.It.Set_Y(eY.OFFL_BtmBeltRun, true);
        }

        public bool Func_BtmBeltStop()
        {
            CIO.It.Set_Y(eY.OFFL_BtnBeltBtmRun, false);
            return CIO.It.Set_Y(eY.OFFL_BtmBeltRun, false);
        }
        #endregion

        #region Check Mgz sensor
        /// <summary>
        /// 탑 클램프 매거진 감지 여부 확인
        /// </summary>
        /// <returns></returns>
        public bool Get_TopMgzChk()
        {
            bool bTopClampMGZDetect1 = false;
            bool bTopClampMGZDetect2 = false;

            bTopClampMGZDetect1 = CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1);    //XC0
            bTopClampMGZDetect2 = CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2);    //XC1

            return bTopClampMGZDetect1 && bTopClampMGZDetect2;
        }

        /// <summary>
        /// 바텀 클램프 메거진 감지 여부 확인
        /// </summary>
        /// <returns></returns>
        public bool Get_BtmMgzChk()
        {
            bool bBtmClampMGZDetect1 = false;
            bool bBtmClampMGZDetect2 = false;

            bBtmClampMGZDetect1 = CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1);    //XC8
            bBtmClampMGZDetect2 = CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2);    //XC9

            return bBtmClampMGZDetect1 && bBtmClampMGZDetect2;
        }
        
        /// <summary>
        /// 공급 레일에 매거진 존재 유무 확인
        /// </summary>
        /// <returns></returns>
        public bool Get_MidConReadyMgz()
        {
            return !CIO.It.Get_X(eX.OFFL_MidMGZDetect);
        }
        #endregion

        /// <summary>
        /// Check Motion Ready
        /// </summary>
        /// <param name="bHD"></param>
        /// <returns></returns>
        public bool Chk_Axes(bool bHD = true)
        {
            int iRet = 0;
            bool bRet = false;

            iRet = CMot.It.Chk_Rdy((int)EAx.OffLoader_X, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.OFFLOADER_X_NOT_READY);
                return bRet = true;
            }

            iRet = CMot.It.Chk_Rdy((int)EAx.OffLoader_Y, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.OFFLOADER_Y_NOT_READY);
                return bRet = true;
            }

            iRet = CMot.It.Chk_Rdy((int)EAx.OffLoader_Z, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.OFFLOADER_Z_NOT_READY);
                return bRet = true;
            }

            return bRet;
        }


        // Light Curtain이 감지되는지 확인한다. Main 화면 상단에 감지여부 표시용
        public bool Chk_LightCurtain()
        {
            return CIO.It.Get_X(eX.OFFL_LightCurtain);
        }


        /// <summary>
        /// 현재 장비 내부에 배출 될 스트립이 존재 하지 않을 경우
        /// </summary>
        /// <returns></returns>
        public bool Get_StripNone()
        {
            bool bRet = false;
            /*
            bRet = 
                   (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_Ready) &&
                   (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready) &&
                   (CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_Wait) &&
                   (CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitPlace) &&
                   (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd);
            */
            if(CDataOption.UseQC && CData.Opt.bQcUse)
            {
                bRet = (CData.Parts[(int)EPart.INR ].bExistStrip == false ) &&
                     (CData.Parts[(int)EPart.ONP ].bExistStrip == false ) &&
                     (CData.Parts[(int)EPart.GRDL].bExistStrip == false ) &&
                     (CData.Parts[(int)EPart.GRDR].bExistStrip == false ) &&
                     (CData.Parts[(int)EPart.OFP ].bExistStrip == false ) &&
                     (CData.Parts[(int)EPart.DRY ].bExistStrip == false ) &&
                     (CQcVisionCom.nQCexistStrip == 0  );  

            }
            //syc : ase kr loading stop
            //INR 자재 O, ONP 자재 O and INR WorkEnd X, ONP WorkEnd X
            //INR 자재 X, ONP 자재 O and INR WorkEnd O, ONP WorkEnd X
            //INR 자재 X, ONP 자재 X and INR WorkEnd O, ONP WorkEnd O
            else 
            {
                bRet = (CData.Parts[(int)EPart.INR ].bExistStrip == false ) &&
                       (CData.Parts[(int)EPart.ONP ].bExistStrip == false ) &&
                       (CData.Parts[(int)EPart.GRDL].bExistStrip == false ) &&
                       (CData.Parts[(int)EPart.GRDR].bExistStrip == false ) &&
                       (CData.Parts[(int)EPart.OFP ].bExistStrip == false ) &&
                       (CData.Parts[(int)EPart.DRY ].bExistStrip == false );
            }
            return bRet;
        }

        /// <summary>
        /// Get OnLoader Work End
        /// </summary>
        /// <returns></returns>
        public bool Get_OnLoaderWorkEnd()
        {
            bool bRet = false;

            bRet = CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd;

            return bRet;
        }

        /// <summary>
        /// Get Last Slot
        /// </summary>
        /// <returns></returns>
        public bool Get_LastSlot()
        {
            bool bRet = false;

            bRet = (CData.Parts[(int)EPart.ONL].iMGZ_No >= CData.LotInfo.iTotalMgz) && (CData.Parts[(int)EPart.ONL].iSlot_No >= CData.Dev.iMgzCnt);

            return bRet;
        }

        // 2021-05-31, jhLee, Multi-LOT 기능
        // MultiLOT 환경에서 LOT이 종료되었는지 check   
        // Magaizne을 Place하는 조건을 물을때 사용되므로 강제로 Magazine을 내려놓으라는 요청도 함께 참고한다.
        public bool Get_LotComplete()
        {
            // 1. 현재 Multi-Lot 사용중인가 ?
            if (CData.IsMultiLOT() == false) return false;          // Multi-LOT 미사용이면 본 기능은 사용하지 않는다.

            // 2. 강제로 Unloading중인 Magazine을 Place하라고 지정되었는가 ?
            if (CData.LotMgr.IsForceUnloadPlace)
            {
                if (Get_BtmMgzChk() || Get_TopMgzChk())             // 실제 Magazine이 감지되고 있다면 
                {
                    return true;                                    // Place를 시키도록 한다.
                }
                else
                    CData.LotMgr.IsForceUnloadPlace = false;        // Magazine이 없다면 Flag를 초기화 한다.

            }

            // 3. 배출중인 LOT이 존재하며 LOT이 종료되었는가 ?
            return CData.LotMgr.IsLotComplete(CData.LotMgr.UnloadingLotName);
        }


        #endregion

        public int QcVisionPermission()
        {
            if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
            {
                m_nSafetyCount = 0;                 // 연속 간섭 카운터 초기화
                return 0;
            }

            //Thread.Sleep(500);

            //CQcVisionCom.nGet_QcVisionPermissionCnt++;

            //if (CQcVisionCom.nQcVisionPermissionTO > CQcVisionCom.nGet_QcVisionPermissionCnt)
            //{
            //    CQcVisionCom.nGet_QcVisionPermissionCnt = 0;
            //    return 2;//Error Occur
            //}

            //Magazine Interrupt 
            //0 : 간섭이 없는 안전한 상태이다.
            //1 : 배출 동작중으로 간섭이 발생할 수 있는 상태이다


            if (CQcVisionCom.nMgzMoving == 0)       // 간섭이 없다.
            {
                m_nSafetyCount = 0;                 // 연속 간섭 카운터 초기화
                return 0;
            }

            return 1;
        }

        // 2021-02-05, jhLee : Skyworks Light-Curtain 감지시 일시멈춤기능을 별도의 함수로 변경
        //
        // Light curtain 감지시 일시멈춤을 지정하고 센서 감지 결과를 리턴해준다.
        public bool CheckLightCurtainPause()
        {
            bool bSensor = CIO.It.Get_X(eX.OFFL_LightCurtain);         // Area Sensor 감지 여부

            // AreaSensor가 감지되는 동안 모터 일시 정지 기능을 이용할 것인가 ?
            if (CDataOption.IsPause)
            {
                // 2021-02-05, jhLee : Skyworks 무언정지 원인으로 AreaSensor 동작을 개선하기위해 기능을 축소
                //                      각 사이트별로 센서 감지시 동작을 고객과 다시 협의할 필요가 있다.
                //                      // 현재는 감지시 On-Loader/Off-Loader 자체만 정지하도록 한다.

                // Area sensor가 감지시 일시 정지 처리
                if (bSensor)
                {
                    //OffLoader elevator Parts
                    CMot.It.Pause((int)EAx.OffLoader_X);
                    CMot.It.Pause((int)EAx.OffLoader_Y);
                    CMot.It.Pause((int)EAx.OffLoader_Z);

                    //고객사협의필요
                    if (ECompany.SkyWorks == CData.CurCompany)
                    {
                        if (false == CData.Opt.bQcUse)
                        {
                            //Dryzone Parts
                            CMot.It.Pause((int)EAx.DryZone_X);
                            CMot.It.Pause((int)EAx.DryZone_Z);
                            CMot.It.Pause((int)EAx.DryZone_Air);

                            //OffLoader Picker Parts
                            CMot.It.Pause((int)EAx.OffLoaderPicker_X);
                            CMot.It.Pause((int)EAx.OffLoaderPicker_Z);
                        }
                    }
                }
                else
                {
                    //OffLoader elevator Parts
                    CMot.It.Resume((int)EAx.OffLoader_X);
                    CMot.It.Resume((int)EAx.OffLoader_Y);
                    CMot.It.Resume((int)EAx.OffLoader_Z);

                    if (ECompany.SkyWorks == CData.CurCompany)
                    {
                        if (false == CData.Opt.bQcUse)
                        {
                            //Dryzone Parts
                            CMot.It.Resume((int)EAx.DryZone_X);
                            CMot.It.Resume((int)EAx.DryZone_Z);
                            CMot.It.Resume((int)EAx.DryZone_Air);

                            //OffLoader Picker Parts
                            CMot.It.Resume((int)EAx.OffLoaderPicker_X);
                            CMot.It.Resume((int)EAx.OffLoaderPicker_Z);
                        }
                    }
                }
            }

            return bSensor;         // 감지여부 회신
        }

        // 2021-02-05, jhLee ; QC와의 Interface 변수들을 초기화 해준다.
        public void ClearQCInterfaceFlag()
        {
            // 사용을 마친 각종 Flag 초기화
            CQcVisionCom.m_nReadyReqMgz = CQcVisionCom.eMGZ_NONE;           // QC가 요청한 Magazine 종료 
            CQcVisionCom.m_nMgzReady    = CQcVisionCom.eMGZ_NONE;           // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료
            CQcVisionCom.m_nQCSend      = CQcVisionCom.eQCSend_None;        // QC가 전송하는 과정 및 결과
        }

        
        // Magazine이 준비되었다는 flag를 Clear 한다.
        public void ClearMgzReadyFlag()
        {
            CQcVisionCom.m_nMgzReady = CQcVisionCom.eMGZ_NONE;              // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료
        }

        // 2021.12.13 SungTae Start : [추가] (Qorvo향 VOC) 1:1 Matching 관련 Mgz No. Check용
        // false : Same Mgz
        //  true : Different Mgz
        public bool Chk_QcVisionDiffentMgz(int nMgzNo, bool bFstSlotEmpty)
        {
            bool bRet;

            if (CData.Parts[(int)EPart.DRY].iMGZ_No != nMgzNo && nMgzNo > 0 && !bFstSlotEmpty)
                bRet = true;
            else
                bRet = false;

            return bRet;
        }
        // 2021.12.13 SungTae End

        // 2022.01.11 SungTae Start : [추가]
        public string Chk_QCSendMsg(int nMsgNo)
        {
            string sMsg = string.Empty;

            switch(nMsgNo)
            {
                case 0: sMsg = "QCSend_None ";   break;
                case 1: sMsg = "QCSend_Start";   break;
                case 2: sMsg = "QCSend_End  ";   break;
                case 3: sMsg = "QCSend_Abort";   break;
            }

            return sMsg;
        }
        
        public string Chk_MgzJudgment(int nJudge)
        {
            string sMsg = string.Empty;

            switch (nJudge)
            {
                case 0: sMsg = "MGZ_NONE";  break;
                case 1: sMsg = "MGZ_GOOD";  break;
                case 2: sMsg = "MGZ_NG";    break;
            }

            return sMsg;
        }
        // 2022.01.11 SungTae 

        //// 2020.09.28 JSKim St
        //public bool Get_QcVisionPermission()
        //{
        //    if (CData.Opt.bQcUse == false)
        //    {
        //        return true;
        //    }

        //    CTcpIp.It.SendPermission();
        //    m_Permission.Set_Delay(10000);

        //    do
        //    {
        //        if (m_Permission.Chk_Delay() == true)
        //        {
        //            return false;
        //        }
        //        Thread.Sleep(10);
        //    }
        //    while (CTcpIp.It.bRecivePermission == false);

        //    return CTcpIp.It.bPermission;
        //}
        //// 2020.09.28 JSKim Ed


        //// 2021-01-11, YYY QC에서 전송 취소 요청이 들어왔다.
        //public void QcAbortTransferring()
        //{
        //    int SlotNum;
        //    if (m_nDuringOut == 1)// GOOD
        //    {
        //        m_nDuringOut = 0;

        //        //SlotNum = GetBtmEmptySlot();
        //        //if (SlotNum > 0)
        //        //{
        //        //SlotNum--;
        //        // }

        //        if (m_nCurSlot < 0 || m_nCurSlot > CData.Dev.iMgzCnt)       // Range Check
        //        {
        //            _SetLog($"Error : Current slot out of range, {m_nCurSlot} of {CData.Dev.iMgzCnt}  GOOD Mgz");
        //        }
        //        else
        //        {
        //            CData.Parts[(int)EPart.OFL].iSlot_info[m_nCurSlot] = (int)eSlotInfo.Empty;
        //            CData.LotInfo.iTOutCnt--;
        //        }
        //    }
        //    else if (m_nDuringOut == 2)//NG
        //    {
        //        m_nDuringOut = 0;
        //        //SlotNum = GetTopEmptySlot();
        //        //if (SlotNum > 0)
        //        //{
        //        // SlotNum--;
        //        //}

        //        if (m_nCurSlot < 0 || m_nCurSlot > CData.Dev.iMgzCnt)
        //        {
        //            _SetLog($"Error : Current slot out of range, {m_nCurSlot} of {CData.Dev.iMgzCnt}  NG Mgz");
        //        }
        //        else
        //        {
        //            CData.Parts[(int)EPart.OFR].iSlot_info[m_nCurSlot] = (int)eSlotInfo.Empty;
        //            CData.LotInfo.iTOutCnt--;
        //        }
        //    }
        //}

        #region  Log 구조체 초기화
        /// <summary>
        /// Log 구조체 초기화
        /// </summary>
        //190218 ksg :
        public void LogclearVal()
        {
            m_LogVal.iStep  =  0; 
            m_LogVal.sAsix1 = "";
            m_LogVal.dPos1  = -1;
            m_LogVal.sAsix2 = "";
            m_LogVal.dPos2  = -1;
            m_LogVal.sMsg   = "";    
        }
        #endregion

        #region Log 저장
        /// <summary>
        /// Log 저장 함수
        /// </summary>
        //190218 ksg :
        public void SaveLog()
        {
            string sLine = "";
            string sDir, sPath, sDay, sHour, sStep, sPos1, sPos2;

            DateTime      Now  = DateTime.Now;

            sDir = GV.PATH_LOG+ Now.Year .ToString("0000") + "\\" + Now.Month.ToString("00") + "\\" + Now.Day  .ToString("00") + "\\OFL\\";

            if (!Directory.Exists(sDir.ToString()))
            {
                Directory.CreateDirectory(sDir.ToString());
            }

            sDay   = DateTime.Now.ToString("[yyyyMMdd-HHmmss fff]");
            sHour  = DateTime.Now.ToString("yyyyMMddHH"           );
            sStep  = m_LogVal.iStep .ToString();
            sPos1  = m_LogVal.dPos1 .ToString();
            sPos2  = m_LogVal.dPos2 .ToString();

            sLine  = sDay             + "\t";
            sLine += m_LogVal.sStatus + "\t";
            sLine += sStep            + "\t";
            sLine += m_LogVal.sAsix1  + "\t";
            sLine += sPos1            + "\t";
            sLine += m_LogVal.sAsix2  + "\t";
            sLine += sPos2            + "\t";

            sLine += m_LogVal.sMsg;
            //sLine += "\n";

            //sPath = sDir + sHour + ".log";
            sPath = sDir;
            /*
            FileInfo fI = new FileInfo(sPath);
            if(!fI.Exists)
            {
                fI.Create().Close();
                GU.Delay(50);
            }
            
            StreamWriter Log = new StreamWriter(sPath, true);
            Log.WriteLine(sLine);
            Log.Close();
            */
            CLogManager Log = new CLogManager(sPath, null, null);
            Log.WriteLine(sLine);   

            LogclearVal();
        }
        #endregion
    }
}
