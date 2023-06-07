using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace SG2000X
{
    class CSQ8_OfL_SIM : CStn<CSQ8_OfL_SIM>
    {
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

        private bool m_ActUp = true;
        private bool m_ActDn = false;
        private bool m_ActOpen = false;
        private bool m_ActClose = true;
        private bool m_BeltRun = true;
        private bool m_BeltStop = false;
        private bool m_BeltCw = true;
        private bool m_BeltCCw = false;
        private bool m_bxChkLight = false;

        private bool m_bReqStop = false;
        private bool m_bDuringGOut = false; //190406 ksg :Qc
        private bool m_bDuringFOut = false; //190406 ksg :Qc
        public bool m_GetQcWorkEnd = false; //190406 ksg :Qc
        public bool m_ChkQcStrip = false;

        private bool m_bPreChkLight = false;

        //미사용 삭제 private bool m_bEmitRetry = false;          // 2020-11-20, jhLee : Emit 동작을 재시도해야하는가 ?

        private CTim m_Delay = new CTim();
        public CTim m_DelayRsl = new CTim(); //190406 ksg :Qc
        public CTim m_DelayWE = new CTim(); //190406 ksg :Qc
        private CTim m_TiOut = new CTim();
        private CTim m_TLightDelay = new CTim();
        private CTim m_TSendDelay = new CTim();
        private CTim m_LightTiOut = new CTim();
        // 2020.09.28 JSKim St
        private CTim m_Permission = new CTim();
        // 2020.09.28 JSKim Ed

        //190807 ksg : TackTime 
        private DateTime m_StartSeq;
        private DateTime m_EndSeq;
        private TimeSpan m_SeqTime;

        // 2020-11-19, jhLee : 이동 위치 보관용 변수
        private double m_dMovePos = 0.0;

        ESeq iSeq;
        public ESeq SEQ = ESeq.Idle;

        public tLog m_LogVal = new tLog(); //190218 ksg :


        bool bMgzTopCheck = false; // Get_TopMgzChk();
        bool bMgzBtmCheck = false; //Get_BtmMgzChk();
        bool bReadyMidMsg = false;// Get_MidConReadyMgz();
        bool bMgzFullSlot = false; //GetFullSlotCheck();
        bool bTMgzFullSlot = false;///GetTopFullSlotCheck();  //190404 ksg : Qc 
        bool bStripNone = false;//Get_StripNone();//장비내 Strip 있음
        bool bOnLdWorkEnd = false;// Get_OnLoaderWorkEnd(); CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd;
        bool bLastSlot = false;// Get_LastSlot();
        bool bDiffMgz = false;
        bool bFstSlotEmpty = false;// CData.Parts[(int)EPart.OFL].iSlot_info[0] == (int)eSlotInfo.Empty;
        bool bWorkEnd = false;


        bool MgzPlace1 = false;
        bool MgzPlace2 = false;
        bool MgzPlace3 = false;
        bool TMgzPlace1 = false;
        bool TMgzPlace2 = false;
        bool TMgzPlace3 = false;
        //조건 맞추기
        bool bWait = false;
        bool bBtmPick = false;
        bool bBtmPlace = false;
        bool bBtmRecive = false;
        bool bTopPick = false;
        bool bTopPlace = false;
        bool bTopRecive = false;

        private CSQ8_OfL_SIM()
        {
            if (!CData.Opt.bQcSimulation)
            {
                return;
            }
            m_bHD = false;
        }

        public bool ToReady()
        {
            m_bReqStop = false;
            SEQ = ESeq.Idle;
            m_DelayWE.Set_Delay(2000);
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
        /// Auto Cycle & define Step
        /// </summary>      
        public bool AutoRun()
        {
            /*
            if (CSQ_Main.It.m_iStat == eStatus.Stop)
            { return false; }
            */
            iSeq = CData.Parts[(int)EPart.OFL].iStat;
            m_bxChkLight = false;

            // 200618 jym : 스카이웍스 제외한 조건에서 일시정지 안쓰는 조건으로 변경
            if (!CDataOption.IsPause)
            {
                if (!m_bPreChkLight)
                { m_LightTiOut.Set_Delay(8000); }
                //if (CIO.It.Get_X(eX.OFFL_LightCurtain))
                if(false)
                {
                    m_bPreChkLight = true;
                    if (m_LightTiOut.Chk_Delay() == true)
                    {
                        m_bPreChkLight = false;

                        CErr.Show(eErr.OFFLOADER_DETECT_LIGHTCURTAIN);
                        _SetLog("Error : Detect lightcurtain.");

                        m_iStep = 0;
                        return true;
                    }
                    return false;
                }
                else
                {
                    m_bPreChkLight = false;
                }
            }

            //Error 확인
            //ksg
            //if (CSQ_Main.It.m_iStat == eStatus.Error || CSQ_Main.It.m_iStat != eStatus.Auto_Running)
            //{ return false; }

            if (SEQ == (int)ESeq.Idle)
            {
                if (m_bReqStop) return false;
                if (CSQ_Main.It.m_iStat == EStatus.Stop)
                { return false; }

                //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                //조건 상태 확인
                int iIsDryMgzNum = CData.Parts[(int)EPart.DRY].iMGZ_No;
                int iIsOflMgzNum = CData.Parts[(int)EPart.OFL].iMGZ_No;
                
                if (CData.Opt.bFirstTopSlotOff)
                {
                    bFstSlotEmpty = CData.Parts[(int)EPart.OFL].iSlot_info[CData.Dev.iMgzCnt - 1] == (int)eSlotInfo.Empty;
                }
                else
                {
                    bFstSlotEmpty = CData.Parts[(int)EPart.OFL].iSlot_info[0] == (int)eSlotInfo.Empty;
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

                if ((iIsDryMgzNum != iIsOflMgzNum) && (iIsOflMgzNum > 0) && !bFstSlotEmpty) bDiffMgz = true;
                else bDiffMgz = false;

                bool bWorkEndCheck = (CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WorkEnd &&
                                      CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd);


                if (CData.Opt.bQcUse)
                {
                    if (m_DelayWE.Chk_Delay())
                    {
                        //if (bWorkEndCheck && !m_GetQcWorkEnd)
                        if (bWorkEndCheck && CQcVisionCom.nQCexistStrip == 1)
                        {
                            //CTcpIp.It.SendWorkEnd();
                            //OfLoad reaquest strip to QCVision
                            CGxSocket.It.SendMessage("LotEnd");
                            
                            _SetLog("[SEND](EQ->QC) LotEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                            m_DelayWE.Set_Delay(3000);
                        }
                    }
                }

                if (CData.Opt.bQcUse)
                {
                    bWorkEnd = ( CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WorkEnd &&
                                 CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_WorkEnd &&
                                //m_GetQcWorkEnd);
                                CQcVisionCom.nQCexistStrip == 0);
                }
                else
                {
                    bWorkEnd = ( CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WorkEnd);
                }

                if (CData.Opt.bQcUse)
                {
                    //if (CTcpIp.It.bResultGood && !CTcpIp.It.bResultFail && !m_bDuringGOut)
                    //m_GetQcWorkEnd [QC send "SendEnd"]
                    if (CQcVisionCom.rcvQCSendOutNG   && !CQcVisionCom.rcvQCSendOutGOOD  && !m_bDuringGOut)//QC 에서 양품받을수있는지 문의 [QC -> EQ]
                    {
                        m_bDuringGOut = true;
                        if (iSeq == ESeq.OFL_BtmRecive || iSeq == ESeq.OFL_TopRecive)
                        {
                            iSeq = ESeq.OFL_BtmRecive;
                            vmMan_13QcVision.m_qcVision.addMsgList("OfL [AutoRun] Off Load: ESeq.OFL_BtmRecive");
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                            //CTcpIp.It.bResultGood = false;
                            //CQcVisionCom.rcvQCReadyQueryGOOD = false;
                            m_LogVal.sMsg = "QC rcvQCSendOut  = ";
                            m_LogVal.sMsg += ", m_bDuringGOut = " + m_bDuringGOut.ToString();
                            SaveLog();
                        }
                    }
                    else if (CQcVisionCom.rcvQCSendOutGOOD && !m_bDuringFOut)
                    {
                        m_bDuringFOut = true;
                        if (iSeq == ESeq.OFL_BtmRecive || iSeq == ESeq.OFL_TopRecive)
                        {
                            iSeq = ESeq.OFL_TopRecive;
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_TopRecive;
                            //CTcpIp.It.bResultFail = false;
                            //CQcVisionCom.rcvQCSendOutNG = false;
                            m_LogVal.sMsg = "rcvQCSendOutNG = ";
                            // + CTcpIp.It.bResultFail.ToString();
                            m_LogVal.sMsg = ", m_bDuringGOut = " + m_bDuringFOut.ToString();
                            SaveLog();
                        }
                    }

                    //else if (!CTcpIp.It.bResultGood && !CTcpIp.It.bResultFail && !m_bDuringGOut && !m_bDuringFOut && !bWorkEnd && bMgzTopCheck && bMgzBtmCheck)
                    else if (!CQcVisionCom.rcvQCReadyQueryGOOD && !CQcVisionCom.rcvQCReadyQueryNG && !m_bDuringGOut && !m_bDuringFOut && !bWorkEnd && bMgzTopCheck && bMgzBtmCheck)
                    {
                        if (iSeq == ESeq.OFL_BtmRecive || iSeq == ESeq.OFL_TopRecive)
                        {
                            if (m_DelayRsl.Chk_Delay())
                            {
                                //CTcpIp.It.SendResult();
                                //CGxSocket.It.SendMessage("ResultQuery");
                                //m_DelayRsl.Set_Delay(5000);
                                //m_LogVal.sMsg = "SendResult()";
                                //SaveLog();
                                return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                //190404 ksg : Qc
                if (CData.Opt.bQcUse)
                {   //Btm
                    bWait = !bWorkEnd && (iSeq == ESeq.Idle) || (iSeq == ESeq.OFL_Wait);
                    bBtmPick = !bWorkEnd && (iSeq == ESeq.OFL_BtmPick);
                    bBtmRecive = !bWorkEnd && (iSeq == ESeq.OFL_BtmRecive) && (bMgzBtmCheck) && (bMgzTopCheck);
                    bTopPick = !bWorkEnd  && (bMgzBtmCheck) && (!bMgzTopCheck) ;
                    bTopRecive = !bWorkEnd && (iSeq == ESeq.OFL_TopRecive) && (bMgzTopCheck);
                }
                else
                {
                    bWait = !bWorkEnd && (iSeq == ESeq.Idle) || (iSeq == ESeq.OFL_Wait);
                    //20200513 jym : 매거진 배출 위치 변경 추가
                    if (CData.Opt.eEmitMgz == EMgzWay.Btm)
                    {
                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
                        if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                            CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                            CData.CurCompany == ECompany.SST)
                            MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck && CData.LotInfo.iTOutCnt == CData.LotInfo.iTotalStrip;//koo : Qorvo Lot Rework
                        else
                            MgzPlace1 = bWorkEnd && bStripNone && bMgzBtmCheck;

                        MgzPlace2 = !bWorkEnd && !bStripNone && bMgzBtmCheck && bMgzFullSlot;
                        MgzPlace3 = !bWorkEnd && !bStripNone && bMgzBtmCheck && !bMgzFullSlot && bDiffMgz && CData.Opt.bOfLMgzMatchingOn;
                        bBtmPick = !bWorkEnd && (iSeq == ESeq.OFL_BtmPick) && (!bStripNone) && (!bMgzBtmCheck); // && (bReadyMidMsg);
                        bBtmPlace = MgzPlace1 || MgzPlace2 || MgzPlace3;
                        bBtmRecive = !bWorkEnd && (iSeq == ESeq.OFL_BtmRecive) && (!bStripNone) && (bMgzBtmCheck) && (!bMgzFullSlot);
                    }
                    else
                    {
                        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
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

                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
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

                if (iSeq == ESeq.OFL_WorkEnd) 
                {
                    return true; 
                }

                if (!bBtmRecive && (iSeq == ESeq.OFL_BtmRecive))
                {
                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;
                }

                //20200513 jym : 매거진 배출 위치 변경 추가
                if (!bTopRecive && (iSeq == ESeq.OFL_TopRecive))
                {
                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;
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
                else if (bTopPick) //빈 MGZ 가져옴
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_TopPick;
                    _SetLog(">>>>> Top pick start.");
                }
                else if (bTopPlace)// MGZ 배출위치로 가져옴
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_TopPlace;
                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_TopPlace;
                    _SetLog(string.Format(">>>>> Top place start.  Place1 : {0}  Place2 : {1}  Place3 : {2}", TMgzPlace1, TMgzPlace2, TMgzPlace3));
                }
                else if (bTopRecive) ////MGZ 자제 받는 위치로 
                {
                    m_iStep = 10;
                    m_iPreStep = 0;
                    SEQ = ESeq.OFL_TopRecive;
                    _SetLog(">>>>> Top recive start.");
                }

                //190406 ksg : Qc
                /*
                else if (bWorkEnd && !bMgzBtmCheck)
                {
                    _SetLog("OFFL_WorkEnd");
                    m_iStep = 0;
                    m_iPreStep = 0;
                    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_WorkEnd;
                }
                */

                else if (bWorkEnd)
                {
                    if (CData.Opt.bQcUse)
                    {
                        if (bWorkEnd && !bMgzBtmCheck && !bMgzTopCheck)
                        {
                            m_GetQcWorkEnd = false;

                            m_iStep = 0;
                            m_iPreStep = 0;
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_WorkEnd;
                            _SetLog(">>>>> QC use, Work End. -> " + bWorkEnd + "," + bMgzBtmCheck + "," + bMgzTopCheck);
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
                                _SetLog(">>>>> QC skip, Work end(top).");
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
                            _SetLog("<<<<< Off Load : Wait end SEQ = ESeq.Idle.");
                            vmMan_13QcVision.m_qcVision.addMsgList("OfL [AutoRun()] Off Load:  Wait end.");
                        }

                        return false;
                    }

                case ESeq.OFL_BtmPick:
                    {
                        //200120 ksg :
                        //if(CData.CurCompany != eCompany.JSCK)
                        if (CDataOption.OflRfid == eOflRFID.NotUse)
                        {
                            if (Cyl_BtmPick())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< RFID not use.  Bottom pick end.");
                            }
                        }
                        else
                        {
                            if (CData.Opt.bOflRfidSkip)
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
                        //if ((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        if(false)
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
                            vmMan_13QcVision.m_qcVision.addMsgList("Cyl_BtmRecive() done SEQ = ESeq.Idle");
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
                            _SetLog("<<<<< Top place end.");
                            SEQ = ESeq.Idle;
                        }

                        return false;
                    }

                case ESeq.OFL_TopRecive:
                    {
                        //if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        if(false)
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
                            vmMan_13QcVision.m_qcVision.addMsgList(" >>> [OfLoad] ESeq.Idle");
                        }

                        return false;
                    }
            }
        }

        private void _SetLog(string sMsg)
        {
            if (CData.Opt.bQcSimulation)
            {
                return;
            }
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
            m_bxChkLight = false;

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
                        m_bHD = false;
                        if (Chk_Axes(false))
                        {
                            m_iStep = 0;
                            return true;
                        }

                        if (m_bxChkLight)
                        { return false; }

                        //200625 jhc : Homing 전 자재 감지 확인

                        if (CData.Opt.bQcUse == false)
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
                        }


                        if (QcVisionPermission() == 1)
                        {
                            //Mgz = 1 배출 동작중 간섭 발생 상태
                            //확인 30초 동안 계속 
                            return false;
                        }
                        else if (QcVisionPermission() == 2)
                        {
                            //Mgz = 1 배출 동작중 간섭 발생 상태 timeout
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            _SetLog("Error : QC Strip-out detect, Cyl_Home");

                            m_iStep = 0;
                            return true;
                        }
                        // 2020.09.28 JSKim Ed

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

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFL_Z_Wait);
                        // X 대기 위치로 이동(중국 직원 요청)
                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);
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
            //m_bxChkLight = false;
            m_bxChkLight = false;
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

            if (CData.Opt.bQcUse == false)
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
                        if (Chk_Axes())
                        {
                            m_iStep = 0;
                            return true;
                        }

                        if (m_bxChkLight)
                        { return false; }

                        // 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        if (false)
                        {
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            _SetLog("Error : QC Strip-out detect. Wait");

                            m_iStep = 0;
                            return true;
                        }
                        // 2020.09.28 JSKim Ed

                        //Belt_Top(m_BeltStop);
                        //Belt_Mid(m_BeltStop, m_BeltCCw);
                        //Belt_Btm(m_BeltStop);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_Wait()] :Check axes.  Top, Middel, Bottom belt stop.");
                        Thread.Sleep(200);
                        
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        //CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOffL_X_Algn);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_wiat] 11: X axis move align");
                        Thread.Sleep(200);
                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_wait] : Y axis move wait");
                        Thread.Sleep(200);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFL_Z_Wait);

                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_wait] : Z axis move wait");

                        Thread.Sleep(200);
                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_Wait))
                        //{ return false; }

                        //20200513 jym : 매거진 배출 위치 변경 추가
                        /*
                        if ((CData.Opt.eEmitMgz == EMgzWay.Top) ? Get_TopMgzChk() : Get_BtmMgzChk())
                        {
                            if (!GetFullSlotCheck() && CData.Parts[(int)EPart.ONL].iStat != ESeq.ONL_WorkEnd)
                            {
                                CData.Parts[(int)EPart.OFL].iStat = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopRecive : ESeq.OFL_BtmRecive; //Strip IN 위치 
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFL].iStat = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopPlace : ESeq.OFL_BtmPlace; //MGZ 배출 위치 
                            }
                        }
                        else
                        {
                            CData.Parts[(int)EPart.OFL].iStat = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopPick : ESeq.OFL_BtmPick; //Empty MGZ Pick 위치 
                        }
                        */


                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPick;
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFL].iStat);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_wait] : Finish.  Status");

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

            //m_bxChkLight = false;
            m_bxChkLight = false;
            m_iPreStep = m_iStep;

            ////200625 jhc : Wait Pos. 이동 전 자재 감지 확인
            ////if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
            //if(false)
            //{
            //    CErr.Show(eErr.DRY_DETECTED_STRIP);
            //    _SetLog("Error : Strip-out detect.");

            //    m_iStep = 0;
            //    return true;
            //}
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
                        //190630 ksg :
                        //if ((CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        if(false)
                        {
                            CErr.Show(eErr.OFFLOADER_ALREADY_BOTTOM_MGZ_ERROR);
                            _SetLog("Error : Already bottom magazine.");

                            m_iStep = 0;
                            return true;
                        }

                        // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                        //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                        //if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                        if(false)
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
                        //if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        if (false)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            _SetLog("Error : DRY X axis not wait.");

                            m_iStep = 0;
                            return true;
                        }
                        //

                        //if (m_bxChkLight) return false;

                        // 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        if(false)
                        {
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            _SetLog("Error : QC Strip-out detect. BtmPick");

                            m_iStep = 0;
                            return true;
                        }
                        // 2020.09.28 JSKim Ed

                        //Act_BtmClampOC(m_ActOpen);
                        //Belt_Mid(m_BeltStop, m_BeltCCw);
                        _SetLog("Bottom clamp open.  Middle belt stop.");
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Bottom clamp open.  Middle belt stop.");
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        //if (!Act_BtmClampOC(m_ActOpen))
                        //{ return false; }

                        //Act_BtmClampModuleDU(m_ActDn);
                        //Act_TopClampModuleDU(m_ActUp);
                        Thread.Sleep(200);
                        _SetLog("Bottom clamp down.  Top clamp up.");
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Bottom clamp down.  Top clamp up.");
                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!Act_BtmClampModuleDU(m_ActDn))
                        //{ return false; }

                        //if (!Act_TopClampModuleDU(m_ActUp))
                        //{ return false; }
                        Thread.Sleep(200);
                        //CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);
                        _SetLog("X axis move align.", CData.Dev.dOffL_X_Algn);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] X axis move align.");
                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Y axis move wait.");
                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickGo);
                        _SetLog("Z axis move pick.", CData.SPos.dOFL_Z_BPickGo);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Z axis move pick.");
                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickGo))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);
                        _SetLog("Y axis move pick.", CData.SPos.dOFL_Y_Pick);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Y axis move pick.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        //{ return false; }

                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCCw);
                        //m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);
                        //_SetLog("Middle belt run.  Set delay : " + GV.ONL_DETECT_MGZ_DELAY + "ms");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Middle belt run.  Set delay");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCCw);

                        //if (!Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        //{ return false; }

                        //Belt_Mid(m_BeltStop, m_BeltCCw);

                        //if (Get_BtmMgzChk())
                        if(true)
                        {
                            _SetLog("Detect bottom magazine.");

                            bMgzBtmCheck = true;//simulation 

                            Thread.Sleep(200);
                            vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Detect bottom magazine.");

                            m_iStep = 20;
                            return false;
                        }

                        _SetLog("Middle belt stop.");
                        
                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        /*
                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        m_Delay.Set_Delay(100);
                        _SetLog("Middle belt run(CW).  Set delay : 100ms");

                        */

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        /*
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);
                        _SetLog("Error : Middle belt not detect magazine.");
                        */
                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BClamp);
                        _SetLog("Z axis move clamp.", CData.SPos.dOFL_Z_BClamp);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Z axis move clamp.");

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BClamp))
                        //{ return false; }

                        //Act_BtmClampOC(m_ActClose);
                        //m_Delay.Set_Delay(10000);
                        //_SetLog("Bottom clamp close.  Set delay : 10000ms");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Bottom clamp close. Set delay:10000ms");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        //190514 ksg :
                        //if (m_Delay.Chk_Delay() && !Act_BtmClampOC(m_ActClose))
                        if(false)
                        {
                            CErr.Show(eErr.OFFLOADER_BOTTOM_CLAMP_NOT_CLOSE);
                            _SetLog("Error : Bottom clamp not close.");

                            m_iStep = 0;
                            return true;
                        }
                        //if (!Act_BtmClampOC(m_ActClose))
                        //{ return false; }

                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCw);
                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickUp);
                        _SetLog("Z axis move pick.", CData.SPos.dOFL_Z_BPickUp);

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Z axis move pick.");


                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickUp))
                        //{ return false; }

                        //if (m_bxChkLight) return false;

                        //20191121 ghk_display_strip
                        CData.Parts[(int)EPart.OFL].bExistStrip = true;

                        //Mgz 상태 변환
                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                        SetAllStatusMgz(eSlotInfo.Empty);
                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        //if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        //else CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        //Belt_Mid(m_BeltStop, m_BeltCw);
                        _SetLog("Middle belt stop.  Status : [" + CData.Parts[(int)EPart.OFL].iStat);

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Middle belt stop.  Status " + CData.Parts[(int)EPart.OFL].iStat);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        //if ((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        if(false)
                        {
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR);
                            _SetLog("Error : Not detected bottom magazine.");

                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Y axis move wait.", CData.Dev.dOffL_Y_Wait);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Y axis move wait ");
                        m_iStep++;
                        return false;
                    }

                case 25:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                        _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPick] Finish Cyl_BtmPick() ");
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

            //m_bxChkLight = false;
            m_bxChkLight = false;
            m_iPreStep = m_iStep;

            m_LogVal.sStatus = "OfL_Cyl_BtmPick";

            //200625 jhc : Wait Pos. 이동 전 자재 감지 확인

            if (CData.Opt.bQcUse == false)
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
                        if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

                        if (m_bxChkLight) return false;

                        //// 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        //{
                        //    CErr.Show(eErr.QC_DETECTED_STRIP);
                        //    _SetLog("Error : QC Strip-out detect. RFID");

                        //    m_iStep = 0;
                        //    return true;
                        //}
                        //// 2020.09.28 JSKim Ed

                        Act_BtmClampOC(m_ActOpen);
                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampOC(m_ActOpen), Belt_Mid(m_BeltStop, m_BeltCCw)";
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

                        CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iX.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_X_Algn;
                        SaveLog();

                        m_iStep++;

                        return false;


                    }

                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn)) return false;

                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickGo);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BPickGo;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickGo))
                        { return false; }

                        CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Y_Pick;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        { return false; }

                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Mid(m_BeltRun, m_BeltCCw), Set_Delay(GV.ONL_DETECT_MGZ_DELAY)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);

                        if (!Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        { return false; }

                        Belt_Mid(m_BeltStop, m_BeltCCw);

                        if (Get_BtmMgzChk())
                        {

                            m_LogVal.iStep = m_iStep;
                            m_LogVal.sMsg = "Belt_Mid(m_BeltRun, m_BeltCCw), Belt_Mid(m_BeltStop, m_BeltCCw)";
                            SaveLog();

                            m_iStep = 20;
                            return false;
                        }

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Mid(m_BeltRun, m_BeltCCw), Belt_Mid(m_BeltStop, m_BeltCCw)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        m_Delay.Set_Delay(100);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Mid(m_BeltRun, m_BeltCw), Set_Delay(100)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltStop, m_BeltCCw);
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);

                        m_LogVal.iStep = m_iStep;
                        SaveLog();

                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BClamp);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BClamp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BClamp))
                        { return false; }

                        Act_BtmClampOC(m_ActClose);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampOC(m_ActClose)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        if (!Act_BtmClampOC(m_ActClose))
                        { return false; }

                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCw);
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPickUp);

                        m_iReReadCnt = 0; //190418 ksg : 

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BPickUp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPickUp))
                        { return false; }

                        //190418 ksg : 
                        if (m_iReReadCnt < 5)
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
                        if (!m_Delay.Chk_Delay()) return false;
                        if (CRfid.It.ReciveMsg == "")
                        {
                            m_iStep = 23;
                            return false;
                        }

                        CData.Parts[(int)EPart.OFL].sMGZ_ID = CRfid.It.ReciveMsg;
                        CData.LotInfo.sGMgzId = CRfid.It.ReciveMsg;

                        //191118 ksg :
                        if (CData.GemForm != null)
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
                        if (CData.Opt.bSecsUse) //191125 ksg :
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
                                else if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Run)/*1*/)
								{
									return false;
									}
                            }
                        }
                        if (m_bxChkLight) return false;

                        //20191121 ghk_display_strip
                        CData.Parts[(int)EPart.OFL].bExistStrip = true;
                        //

                        //Mgz 상태 변환
                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                        SetAllStatusMgz(eSlotInfo.Empty);
                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        else CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        Belt_Mid(m_BeltStop, m_BeltCw);

                        //191118 ksg :
                        if (CData.GemForm != null) CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Cnt++;

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Mid(m_BeltStop, m_BeltCw)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {
                        CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {
                        if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        { return false; }

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "\r\n Time_Ofl Btm Pick : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
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

            //m_bxChkLight = false;
            m_bxChkLight = false;
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
                        if (m_bxChkLight) return false;

                        //if ((!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2)))
                        if(false)
                        {
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPick;
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR);
                            SEQ = ESeq.Idle;
                            m_iStep = 0;

                            return true;
                        }
                        //190724 ksg : MGZ 배출 시 자재 배출 센서에 감지시 Error 발생 추가



                        if (false) // CData.Opt.bQcUse == false)
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
                        }

                        //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                        /*if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        */
                        //

                        //Act_BtmClampOC(m_ActClose);
                        //Belt_Btm(m_BeltStop);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampOC(m_ActClose), Belt_Btm(m_BeltStop)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        //if (!Act_BtmClampOC(m_ActClose))
                        //{ return false; }

                        //Act_BtmClampModuleDU(m_ActDn);
                        //Act_TopClampModuleDU(m_ActUp);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampModuleDU(m_ActDn), Act_TopClampModuleDU(m_ActUp)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!Act_BtmClampModuleDU(m_ActDn))
                        //{ return false; }

                        //if (!Act_TopClampModuleDU(m_ActUp))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        //SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPlaceGo);

                        //if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetectFull))
                        if(false)
                        {
                            //set Error
                            CErr.Show(eErr.OFFLOADER_BTM_MAGZIN_FULL);
                            m_iStep = 0;

                            return true;
                        }

                        //if (m_bxChkLight) return false;

                        //if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))
                        //{ Belt_Btm(m_BeltRun); }
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPlace] Belt_Btm(m_BeltRun)");
                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltRun)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //if (m_bxChkLight) return false;

                        //if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect))
                        //{ Belt_Btm(m_BeltRun); }
                        //else
                        //{ Belt_Btm(m_BeltStop); }

                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPlaceGo))
                        //{ return false; }

                        Thread.Sleep(200);
                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "m_Delay.Set_Delay(2000)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        //if (m_bxChkLight) return false;

                        //if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        //{ Belt_Btm(m_BeltRun); }
                        //else
                        //{ Belt_Btm(m_BeltStop); }

                        //if (Get_BtmMgzChk() && !m_Delay.Chk_Delay())
                        //{ return false; }

                        //Belt_Btm(m_BeltStop);

                        //if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect) || !CIO.It.Get_X(eX.OFFL_BtmMGZDetectFull))
                        if(false)
                        {
                            //set Error : Full Mgz
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
                       // CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Place);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Y_Place;
                        SaveLog();
                        
                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Place))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BUnClamp);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPlace]dOFL_Z_BUnClamp ");

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BUnClamp;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BUnClamp))
                        //{ return false; }

                        //Act_BtmClampOC(m_ActOpen);
                        Thread.Sleep(200);

                        
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPlace] dOFL_Z_BUnClamp ");
                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Act_BtmClampOC(m_ActOpen)";
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        //if (!Act_BtmClampOC(m_ActOpen))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_BPlaceDn);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = CData.SPos.dOFL_Z_BPlaceDn;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_BPlaceDn))
                        //{ return false; }

                        //200123 ksg :
                        //if (CDataOption.OflRfid == eOflRFID.Use)
                        //{
                          //  CData.Parts[(int)EPart.OFL].sMGZ_ID = "";
                        //}

                        //if (m_bxChkLight) return false;

                        //Belt_Btm(m_BeltRun);
                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        //Data 이동 및 상태 변환
                        //CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;
                        SetAllStatusMgz(eSlotInfo.Empty);
                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        //if (CData.Opt.bFirstTopSlotOff) CData.Parts[(int)EPart.OFL].iSlot_No = CData.Parts[(int)EPart.OFL].iSlot_info.Length - 1;
                        //else CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        m_Delay.Set_Delay(2000);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        m_LogVal.sMsg = "CData.Parts[(int)EPart.OFL].iStat = " + CData.Parts[(int)EPart.OFL].iStat.ToString() + ", m_Delay.Set_Delay(2000)";
                        Thread.Sleep(200);
                        
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        //if (!CIO.It.Get_X(eX.OFFL_BtmMGZDetect) && !m_Delay.Chk_Delay())
                        //{ return false; }

                        //m_Delay.Set_Delay(); //옵션 Delay 값 넣기
                        m_LogVal.iStep = m_iStep;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        //if (m_bxChkLight) return false;

                        //Belt_Btm(m_BeltRun);

                        //if (!m_Delay.Chk_Delay())
                        //{ return false; }

                        //Belt_Btm(m_BeltStop);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltStop)";
                        SaveLog();
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPlace] Belt_Btm(m_BeltStop)");
                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //20191121 ghk_display_strip
                        CData.Parts[(int)EPart.OFL].bExistStrip = false;
                        //

                        CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_Wait;

                        CData.sLastLot = CData.Parts[(int)EPart.OFL].sLotName;
                        CData.iLastMGZ = CData.Parts[(int)EPart.OFL].iMGZ_No;

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sMsg = "Belt_Btm(m_BeltStop)";
                        m_LogVal.sMsg += "\r\n Time_Ofl Place : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                        SaveLog();

                        //20190624 josh
                        //jsck secsgem
                        //ceid 999403 mgz ended
                        //if (CData.GemForm != null) CData.GemForm.OnMagazineEnd((uint)SECSGEM.JSCK.eMGZPort.GOOD);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmPlace] finish -> ESeq.OFL_Wait");

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
            /*
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADER_RECIVE_TIMEOUT);
                    m_iStep = 0;

                    return true;
                }
            }
            */
            //20191121 ghk_display_strip
            //if (!Get_BtmMgzChk())
            if (false)
            {//매거진 항상 있어야 함.
                CErr.Show(eErr.OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR);
                m_iStep = 0;

                return true;
            }
            //

            //200625 jhc : Clamp 모듈, Y축, Z축 움직임 전 스트립 걸림 재확인
            /*if ((11 <= m_iStep) && (m_iStep <= 15))
            {
                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                    m_iStep = 0;
                    return true;
                }
            }*/
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
                                                   //190724 ksg : MGZ 상승 시 자재 배출 센서에 감지시 Error 발생 추가
                                                   /*
                                                   if(CSQ_Main.It.m_iStat != eStatus.Auto_Running)
                                                   { 
                                                       if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                                                       {
                                                           CErr.Show(eErr.DRY_DETECTED_STRIP);
                                                           m_iStep = 0;
                                                           return true;
                                                       }
                                                   }*/


                        // QC Vision을 사용하지 않는다면 Dry Zone에서 넘어오는 제품과의 간섭 check 필요
                        
                        if (!CData.Opt.bQcUse)
                        {
                            //191108 ksg :
                            if (CSq_Dry.It.m_DuringDryOut)
                            {
                                m_iStep = 0;
                                return true;
                            }

                            //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            if(false)
                            {
                                CErr.Show(eErr.DRY_DETECTED_STRIP);
                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            //if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            if (false)
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
                        else         // QC Vision을 사용한다면 QC Vision에서 동작중인지 확인
                        {
                            //
                            // 2020-11-24, jhLee : Emit 동작을 재시도해야하는가 ? Emit 동작만 재시도 해야하므로 실행전 조건 체크 및 사전 동작은 생략한다.
                            //
                            // QC로부터 Emit OK가 수신되었고 아직 Emit 성공을 못한 경우에는 재시도 동작으로 인지한다.
                            //
                            //if ( CTcpIp.It.bEmit_Ok && !CTcpIp.It.bSuccess_Ok)
                            if ((CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG) && !CQcVisionCom.rcvQCSendEnd)
                            //if(true)
                            {

                                m_TSendDelay.Set_Delay(1);

                                _SetLog("QC Emit Retry detected, go to Emit Retry");
                                vmMan_13QcVision.m_qcVision.addMsgList("QC Emit Retry detected, go to Emit Retry");
                                m_iStep = 18;   // 사전 처리과정은 Skip 하고 Emit success 확인을 하는 곳으로 jump 한다.
                                return false;
                            }

                            _SetLog("QC Emit Retry flag off");
                            vmMan_13QcVision.m_qcVision.addMsgList("QC Emit Retry flag off");

                            // 2020.09.28 JSKim St
                            /*if (Get_QcVisionPermission() == false)
                            {
                                _SetLog("QC Permission Error. BtmReceive");

                                CErr.Show(eErr.QC_DETECTED_STRIP);
                                m_iStep = 0;
                                return true;
                            }
                            */
                            // 2020.09.28 JSKim Ed
                        }

#if false //20200611 jhc : 임시 삭제 (아래 코드는 Dry X축 Push 동작 중일 경우 항상 Error 발생)
                        //200508 pjh : Dry Position Interlock                        
                        //if (!CMot.It.Get_Mv(m_iDX, CData.SPos.dDRY_X_Wait))
                        if (CMot.It.Get_FP(m_iDX) > CData.SPos.dDRY_X_Wait + 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //
#endif

                        //Act_BtmClampOC(m_ActClose);

                        _SetLog("Check Safety & Bottom Clamp On");
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmRecive] Check Safety & Bottom Clamp On");
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        //if (!Act_BtmClampOC(m_ActClose))
                        //{ return false; }

                        //Act_BtmClampModuleDU(m_ActDn);

                        _SetLog("Act_BtmClampModuleDU(m_ActDn)");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmRecive] Act_BtmClampModuleDU(m_ActDn)");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!Act_BtmClampModuleDU(m_ActDn))
                        //{ return false; }

                        // 2020.09.28 JSKim St
                        /*if (Get_QcVisionPermission() == false)
                        {
                            _SetLog("QC Permission Error, BtmReceive ");
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            m_iStep = 0;
                            return true;
                        }
                        */
                        // 2020.09.28 JSKim Ed

                        if (QcVisionPermission() == 1)
                        {
                            //Mgz = 1 배출 동작중 간섭 발생 상태
                            //확인 30초 동안 계속 
                            return false;
                        }
                        else if (QcVisionPermission() == 2)
                        {
                            //Mgz = 1 배출 동작중 간섭 발생 상태 timeout
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            _SetLog("Error : QC Strip-out detect, Cyl_Home");

                            m_iStep = 0;
                            return true;
                        }

                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iY.ToString();
                        m_LogVal.dPos1 = CData.Dev.dOffL_Y_Wait;
                        SaveLog();

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //if (GetFullSlotCheck())
                        //{
                        //    CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmPlace;
                        //    m_iStep = 0;

                        //    return true;
                        //}

                        // 2020.09.28 JSKim St
                        /*if (Get_QcVisionPermission() == false)
                        {
                            _SetLog("QC Permission Error [13]");
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            m_iStep = 0;
                            return true;
                        }
                        */

                        if (QcVisionPermission() == 1)
                        {
                            //Mgz = 1 배출 동작중 간섭 발생 상태
                            //확인 30초 동안 계속 
                            return false;
                        }
                        else if (QcVisionPermission() == 2)
                        {
                            //Mgz = 1 배출 동작중 간섭 발생 상태 timeout
                            CErr.Show(eErr.QC_DETECTED_STRIP);
                            _SetLog("Error : QC Strip-out detect, Cyl_Home");

                            m_iStep = 0;
                            return true;
                        }


                        // 2020.09.28 JSKim Ed

                        m_dMovePos = GetBtmWorkPos();           // 이동해야 할 Slot 위치 계산

                        // CMot.It.Mv_N(m_iZ, GetBtmWorkPos()); //수식 계산 완료 해야 함.
                        //CMot.It.Mv_N(m_iZ, m_dMovePos);         // 지정 높이로 이동 지령

                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = m_dMovePos;    //  GetBtmWorkPos();
                        SaveLog();
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmRecive] OfLd move BtmWorkPosition");
                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        /*if (!CMot.It.Get_Mv(m_iZ, m_dMovePos))
                        {
                        
                            if (CMot.It.Get_Stop(m_iZ))         // 만약 축이 멈춰있다면 
                            {
                                CMot.It.Mv_N(m_iZ, m_dMovePos); // 다시 이동 명령을 내린다.
                            }

                            return false;
                        }
                        */
                        m_LogVal.iStep = m_iStep;
                        m_LogVal.sAsix1 = m_iZ.ToString();
                        m_LogVal.dPos1 = m_dMovePos;            //  GetBtmWorkPos();
                        SaveLog();
                        m_iStep++;
                        
                        return false;

                    }
                case 15:
                    {
                        if (CData.Opt.bQcUse)
                        {
                            if (m_bDuringGOut)
                            {
                                m_LogVal.iStep = m_iStep;
                                m_LogVal.sMsg = "m_bDuringGOut = " + m_bDuringGOut.ToString();
                                //CTcpIp.It.bEmit_Fail = false;
                                //CTcpIp.It.bEmit_Ok = false;
                                CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false; //EQ -> QC
                                m_TSendDelay.Set_Delay(1);
                                m_Delay.Set_Delay(30000);               // Emit_Ok 수신 Timeout
                                m_iStep++;
                                vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmRecive]>>rcvQCSendOutGOOD = false");

                            }
                            else
                            {
                                m_EndSeq = DateTime.Now; //190807 ksg :
                                m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                                m_LogVal.iStep = m_iStep;
                                m_LogVal.sMsg = "m_bDuringGOut = " + m_bDuringGOut.ToString();
                                m_LogVal.sMsg += "\r\n Time_Ofl Btm Recive : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                                m_iStep = 0;
                            }
                        }
                        else
                        {
                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                            m_LogVal.iStep = m_iStep;
                            m_LogVal.sMsg = "m_bDuringGOut = " + m_bDuringGOut.ToString();
                            m_LogVal.sMsg += "CData.Opt.bQcUse = " + CData.Opt.bQcUse.ToString();
                            m_LogVal.sMsg += "\r\n Time_Ofl Btm Recive : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                            m_iStep = 0;
                        }
                        SaveLog();
                        return false;
                    }

                // QC Emit command Retry 지점
                case 16: // QC Vision에게 배출 요청 전송 : Emit
                    {
                        //if (!m_TSendDelay.Chk_Delay()) return false;

                        //m_bDuringGOut = false;
                        //CTcpIp.It.bSuccess_Ok   = false;            // 배출 성공 Flag는 초기화
                        //CTcpIp.It.bSuccess_Fail = false;
                        //CTcpIp.It.bSuccess_Du = false;              // 배출 수행중 응답
                        //CTcpIp.It.bEmit_Fail = false;             // 배출 요청에 대한 응답 flag 초기화
                        //CTcpIp.It.bEmit_Ok = false;


                        //CTcpIp.It.SendEmit();                       // QC에게 배출 요청을 한다.
                        CGxSocket.It.SendMessage("QCSendRequest");

                        _SetLog("[SEND](EQ->QC) QCSendRequest");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                        // m_Delay.Set_Delay(30000);               // Emit_Ok 수신 Timeout
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_BtmRecive] QC Vision에게 배출 요청 전송");
                        _SetLog("Send Emit -> QC ");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //if(!CTcpIp.It.bEmit_Fail && !CTcpIp.It.bEmit_Ok)            // Emit 명령에 대한 응답을 받지 못하였다면
                        if (!CQcVisionCom.rcvQCSendOutGOOD && !CQcVisionCom.rcvQCSendOutNG)            // Emit 명령에 대한 응답을 받지 못하였다면
                        {
                            if (!m_Delay.Chk_Delay())                                // 30초 이내라면 다시 전송한다.
                            {
                                m_TSendDelay.Set_Delay(2000);                       // 2초마다 Emit 명령을 재 전송한다.

                                // _SetLog("Wait Emit command result");

                                m_iStep = 16;                       // Emit 명령 반복
                                return false;
                            }
                            else // 30초 Timeout
                            {
                                m_bDuringGOut = false;
                                //CTcpIp.It.bEmit_Fail = false;
                                //CTcpIp.It.bEmit_Ok   = false;
                                CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false;
                                _SetLog("Error: QC Emit command Fail by Timeout");

                                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                                m_iStep = 0;            // 처음부터 다시 시작
                                return false;
                            }
                        }

                        if (!CQcVisionCom.rcvQCSendOutGOOD && !CQcVisionCom.rcvQCSendOutNG)                   // Emit 명령 수렴 불가
                        {
                            m_bDuringGOut = false;
                            CQcVisionCom.rcvQCSendRequestACK = false;

                            //_SetLog("Error: QC Emit command Fail <- QC");
                            _SetLog("Error: QC ready transfer Strip <- QC");

                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                            m_iStep = 0;                            // 처음부터 다시시작
                            return false;
                        }
                        else if (CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG)                // Emit 명령 수렴
                        {
                            // 2020-11-24, jhLee : Emit 완료 실패시 Retry를 위해  Emit_Ok 수신 flag는 유지한다.
                            //
                            //d CTcpIp.It.bEmit_Ok = false;         

                            m_Delay.Set_Delay(30000);

                            _SetLog("Emit command OK <- QC");

                            m_iStep++;
                            return false;
                        }

                        return false;

                    }

                case 18: // 배출 완료 대기 Timeout 지정
                    {
                        // 배출 완료 응답 초기화
                        //CTcpIp.It.bSuccess_Du = false;
                        //CTcpIp.It.bSuccess_Ok = false;
                        //CTcpIp.It.bSuccess_Fail = false;
                        CQcVisionCom.rcvQCSendEnd = false;
                        m_Delay.Set_Delay(30000);
                        m_TSendDelay.Set_Delay(1);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Wait QC strip out");

                        m_iStep++;
                        return false;
                    }

                case 19:    // 배출 작업을 마쳤는가 ?
                    {
                        if (!m_TSendDelay.Chk_Delay()) return false;

                        //CTcpIp.It.SendSucces();                     // 배출 성공 여부 조회

                        _SetLog("Send Emit success query -> QC");
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Wait QC strip out Finish");
                        m_iStep++;
                        return false;
                    }

                case 20:// 배출 동작중인가 ?
                    {
                        // 응답이 아직 도착하지 않았다.
                        //if(!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail) 
                        if (!CQcVisionCom.rcvQCSendEnd)
                        {
                            if (!m_Delay.Chk_Delay())           // 30초 이내라면
                            {
                                //m_TSendDelay.Wait(2000);
                                m_TSendDelay.Set_Delay(2000);
                                //딜레이 후 한번 더 확인
                                //if (!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail)
                                if (!CQcVisionCom.rcvQCSendEnd)
                                {
                                    _SetLog("Wait Emit success result from QC... ");
                                    vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Wait Emit success result from QC...");
                                    m_iStep = 19;           // 배출 완료 여부를 다시 묻는다.
                                    return false;
                                }
                            }
                            else                           // 30초 초과
                            {
                                m_bDuringGOut = false;
                                //CTcpIp.It.bSuccess_Du   = false;
                                //CTcpIp.It.bSuccess_Ok   = false;
                                //CTcpIp.It.bSuccess_Fail = false;
                                CQcVisionCom.rcvQCSendEnd = false;

                                //_SetLog("Error: QC Emit success query by Timeout");
                                _SetLog("Error: QC strip transfer success query by Timeout");
                                vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive]Error: QC strip transfer Timeout.");
                                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                                m_iStep = 0;                // 동작 실패, 다시 처음으로 되돌아가서 재시도한다.
                                return false;
                            }
                        }

                        m_iStep = 22;           // 수신 결과 처리
                        return false;
                    }

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
                //            m_bDuringGOut           = false;
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

                case 22:
                    {
                        //if(CTcpIp.It.bSuccess_Ok)           // 배출 성공
                        if (CQcVisionCom.rcvQCSendEnd)
                        {
                            m_bDuringGOut = false;
                            int SlotNum = GetBtmEmptySlot();
                            CData.Parts[(int)EPart.OFL].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
                            //190731 ksg :
                            //----------------
                            try
                            {
                                //CData.Parts[(int)EPart.OFR].sLotName = CTcpIp.It.sStringInfo[0];
                                //CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt16(CTcpIp.It.sStringInfo[1]);
                                //CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt16(CTcpIp.It.sStringInfo[2]);
                                CData.Parts[(int)EPart.OFR].sLotName = CQcVisionCom.sStringInfo[0];
                                CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt16(CQcVisionCom.sStringInfo[1]);
                                CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt16(CQcVisionCom.sStringInfo[2]);

                            }
                            catch
                            {
                                CData.Parts[(int)EPart.OFR].sLotName = "Exception";
                                CData.Parts[(int)EPart.OFR].iMGZ_No = 0;
                                CData.Parts[(int)EPart.OFR].iSlot_No = 0;
                            }
                            //CData.Parts[(int)EPart.OFR].group  = CTcpIp.It.sStringInfo[3]; <-Group 없음
                            //CData.Parts[(int)EPart.OFR].device = CTcpIp.It.sStringInfo[4]; <-Deivce 없음
                            //----------------
                            CData.LotInfo.iTOutCnt++; //190407 ksg :
                            //CTcpIp.It.bResultGood = false;
                            CQcVisionCom.rcvQCReadyQueryGOOD = false;
                            CQcVisionCom.rcvQCReadyQueryNG = false;
                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            Thread.Sleep(200);
                            _SetLog("Receive Emit success OK <- QC");
                            vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Receive Strip Success !!!");
                            // 더이상 Emit Retry는 없다.
                            //CTcpIp.It.bEmit_Ok      = false;        // 이 Flag가 false가 되면 Retry 대기는 없다.
                            //CTcpIp.It.bEmit_Fail    = false;
                            CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutNG = false;
                            if (!CData.Opt.bQcByPass)                // QC Vision Bypass가 아닌경우 
                            {
                                //200316 ksg :
                                //CTcpIp.It.bTResult_Ok = false;
                                //CTcpIp.It.SendTestResult();         // 결과를 받아온다
                                CQcVisionCom.rcvResultQuery = false;
                                CGxSocket.It.SendMessage("ResultQuery");
                                _SetLog("Send Test result query -> QC");
                                
                                _SetLog("[SEND](EQ->QC) ResultQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Send ResultQuery -> QC!!!");
                                m_Delay.Set_Delay(10000);
                                m_TSendDelay.Set_Delay(1000);

                                m_iStep++; //200316 ksg :
                                return false;                           // 이어서 진행
                            }

                            _SetLog("QC Bypass mode, Receive done");
                            vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] QC Bypass mode, Receive done");
                            // QC Bypass일 경우
                            m_iStep = 0; //200316 ksg :
                            return true;                            // 2020-11-19, jhLee : 배출 성공으로 끝낸다.
                        }

                        // 배출 실패
                        // success 명령 반복 수행 필요

                        _SetLog("Error: QC Emit success Fail <- QC");
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Error: QC Emit success Fail <- QC");
                        m_bDuringGOut = false;

                        CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                        m_iStep = 0;                // 배출 성공 여부를 체크하는 곳에서 부터 다시 시작한다.
                        return false;
                    }
                //200316 ksg :

                case 23:  // QC에게 Test 결과를 묻는다.
                    {
                        //if(!m_Delay.Chk_Delay() && !CTcpIp.It.bTResult_Ok)
                        if (!m_Delay.Chk_Delay() && !CQcVisionCom.rcvResultQuery)
                        {
                            if (m_TSendDelay.Chk_Delay())
                            {
                                //CTcpIp.It.SendTestResult();
                                CGxSocket.It.SendMessage("ResultQuery");
                                _SetLog("Retry Send Test result query -> QC");

                                _SetLog("[SEND](EQ->QC) ResultQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                m_TSendDelay.Set_Delay(1000);
                                return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        //else if(CTcpIp.It.bTResult_Ok)
                        else if (CQcVisionCom.rcvResultQuery)
                        {
                            _SetLog("Receive Test ResultQuery OK <- QC");
                            vmMan_13QcVision.m_qcVision.addMsgList("OfL[Cyl_BtmReceive] Receive Test ResultQuery OK <- QC");
                            m_iStep = 0;
                            return true;
                        }
                        else
                        {

                            _SetLog("Error : QC Test result receive Timeout eror");

                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                            m_iStep = 0;
                            return true;
                        }
                    }
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
                //if (m_TiOut.Chk_Delay())
                if(false)
                {
                    _SetLog("Error : Time out");
                    CErr.Show(eErr.OFFLOADER_TOP_PICK_TIMEOUT);
                    m_iStep = 0;

                    return true;
                }
            }

            //m_bxChkLight = false;
            m_bxChkLight = false;
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
                        //if (m_bxChkLight) return false;

                        //20191120 ghk_display_strip
                        //if (CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) || CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2))
                        if(false)
                        {//Mgz 둘중 하나라도 감지 되고 있는지 확인(or 검사)
                            _SetLog("Error : Already detected top magazine");
                            CErr.Show(eErr.OFFLOADER_ALREADY_TOP_MGZ_ERROR);
                            m_iStep = 0;

                            return true;
                        }

                        // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                        //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                        //if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                        if (false)
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
                        //if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        if (false)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

                        //200630 jhc : 이동 전 자재 감지 확인
                        //if (!CData.Opt.bQcUse && !CIO.It.Get_X(eX.DRY_StripOutDetect))
                        if(false)
                        {
                            _SetLog("Error : Strip-out detect");
                            CErr.Show(eErr.DRY_DETECTED_STRIP);

                            m_iStep = 0;
                            return true;
                        }
                        //

                        // 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        if(false)
                        {
                            _SetLog("Error : QC Strip-out detect");
                            CErr.Show(eErr.QC_DETECTED_STRIP);

                            m_iStep = 0;
                            return true;
                        }
                        // 2020.09.28 JSKim Ed

                        //Act_TopClampOC(m_ActOpen);//Top Clamp Open
                        //Belt_Mid(m_BeltStop, m_BeltCCw);//Mid belt stop
                        _SetLog("Middel belt stop, Top clamp open");
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Middel belt stop, Top clamp open");
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        //if (!Act_TopClampOC(m_ActOpen)) //check Top clamp open
                        //{ return false; }

                        //Act_BtmClampModuleDU(m_ActUp); //BtmClam Up
                        //Act_TopClampModuleDU(m_ActUp); //Top Clamp Up

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Bottom clamp up, Top clamp up");
                        _SetLog("Bottom clamp up, Top clamp up");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!Act_BtmClampModuleDU(m_ActUp))
                        //{ return false; }

                        //if (!Act_TopClampModuleDU(m_ActUp))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iX, CData.Dev.dOffL_X_Algn); //X move align

                        _SetLog(string.Format("X axis move align, Pos:{0}mm", CData.Dev.dOffL_X_Algn));
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] X axis move align, Pos");
                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffL_X_Algn)) return false;

                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Y axis move wait, Pos");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPickGo);

                        _SetLog(string.Format("Z axis move pick, Pos:{0}mm", CData.SPos.dOFL_Z_TPickGo));

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Z axis move pick, Pos");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPickGo))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iY, CData.SPos.dOFL_Y_Pick);

                        _SetLog(string.Format("Y axis move pick, Pos:{0}mm", CData.SPos.dOFL_Y_Pick));
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Y axis move pick, Pos");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.SPos.dOFL_Y_Pick))
                        //{ return false; }

                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCCw);
                        //m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);

                        _SetLog(string.Format("Middle belt run CCW, Set delay {0}ms", GV.ONL_DETECT_MGZ_DELAY));
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Middle belt run CCW, Set delay");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCCw);

                        //if (!Get_TopMgzChk() && !m_Delay.Chk_Delay())
                        //{ return false; }

                        //Belt_Mid(m_BeltStop, m_BeltCCw);

                        //if (Get_TopMgzChk())
                        
                        bMgzTopCheck = true; //Top MGZ check

                        if (true)
                        {

                            vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick] Top clamp detect, Middle belt stop");
                            _SetLog("Top clamp detect, Middle belt stop");

                            m_iStep = 20;
                            return false;
                        }

                        _SetLog("Middle belt stop");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Middle belt stop");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {
                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCw);
                        //m_Delay.Set_Delay(100);

                        _SetLog("Middle belt stop, Set dealy:100ms");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Middle belt stop, Set dealy:100ms");

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {
                        //if (!m_Delay.Chk_Delay())
                        //{ return false; }

                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltStop, m_BeltCCw);
                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);
                        //CErr.Show(eErr.OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR);

                        _SetLog("Error : Middle conveyor not detect magazine");

                        m_iStep = 0;
                        return true;
                    }

                case 20:
                    {
                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TClamp);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL : Z axis move top clamp");
                        
                        _SetLog(string.Format("OfL [Cyl_TopPick] Z axis move top clamp, Pos:{0}mm", CData.SPos.dOFL_Z_TClamp));

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TClamp))
                        //{ return false; }

                        //Act_TopClampOC(m_ActClose);
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick] :Top clamp close");
                        _SetLog("Top clamp close");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {
                        //if (!Act_TopClampOC(m_ActClose))
                        //{ return false; }

                        //if (m_bxChkLight) return false;

                        //Belt_Mid(m_BeltRun, m_BeltCw);
                        //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_TPickUp);

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick] Middle belt run CW, Z axis move pick up");

                        _SetLog(string.Format("Middle belt run CW, Z axis move pick up, Pos:{0}mm", CData.SPos.dOFL_Z_TPickUp));

                        m_iStep++;
                        return false;
                    }

                case 23:
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFL_Z_TPickUp))
                        //{ return false; }

                        //if (m_bxChkLight) return false;
                        SetTopAllStatusMgz(eSlotInfo.Empty);

                        //20200513 jym : QC 외에 Top 사용 추가
                        int iPt = (CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        //20191121 ghk_display_strip
                        CData.Parts[iPt].bExistStrip = true;

                        //Mgz 상태 변환
                        CData.Parts[iPt].iStat = ESeq.OFL_TopRecive;

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else CData.Parts[iPt].iSlot_No = 0;

                        //Belt_Mid(m_BeltStop, m_BeltCw);

                        _SetLog("Middle belt stop, Status:" + CData.Parts[iPt].iStat);

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {
                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        //if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        bMgzTopCheck = true; //simulation
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Y axis move wait, Pos:");

                        if (false)
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
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        _SetLog(string.Format("Finish, Tack time:{0}", m_SeqTime.ToString(@"hh\:mm\:ss")));

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("OfL [Cyl_TopPick()] Finish, Tack time:");


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

                    return true;
                }
            }

            //m_bxChkLight = false;
            m_bxChkLight = false;
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
                        if (m_bxChkLight) return false;

                        //20191120 ghk_display_strip
                        if (CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1) || CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2))
                        {//Mgz 둘중 하나라도 감지 되고 있는지 확인(or 검사)
                            _SetLog("Error : Already detected top magazine");
                            CErr.Show(eErr.OFFLOADER_ALREADY_TOP_MGZ_ERROR);
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
                        if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

                        //200630 jhc : 이동 전 자재 감지 확인
                        if (!CData.Opt.bQcUse && !CIO.It.Get_X(eX.DRY_StripOutDetect))
                        {
                            _SetLog("Error : Strip-out detect");
                            CErr.Show(eErr.DRY_DETECTED_STRIP);

                            m_iStep = 0;
                            return true;
                        }
                        //

                        //// 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        //{
                        //    _SetLog("Error : QC Strip-out detect");
                        //    CErr.Show(eErr.QC_DETECTED_STRIP);

                        //    m_iStep = 0;
                        //    return true;
                        //}
                        //// 2020.09.28 JSKim Ed

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

                        if (m_bxChkLight) return false;

                        Belt_Mid(m_BeltRun, m_BeltCCw);
                        m_Delay.Set_Delay(GV.ONL_DETECT_MGZ_DELAY);

                        _SetLog(string.Format("Middle belt run CCW, Set delay {0}ms", GV.ONL_DETECT_MGZ_DELAY));

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        if (m_bxChkLight) return false;

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
                        if (m_bxChkLight) return false;

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

                        if (m_bxChkLight) return false;

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

                        if (m_bxChkLight) return false;

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
                        if (m_iReReadCnt > 0) //재시도인 경우 바코드 읽기 중지(Timing OFF) => Z축 +30 mm (아래로) => Z축 바코드 위치 => 바코드 읽기
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
                        if (!m_Delay.Chk_Delay())
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
                            if (CData.GemForm != null)
                            {
                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID = CData.Parts[(int)EPart.OFL].sMGZ_ID;
                                
                                CData.GemForm.UnloaderMgzVerifyRequest(CData.Parts[(int)EPart.OFL].sMGZ_ID, (uint)SECSGEM.JSCK.eMGZPort.GOOD);

                                // 2022.03.31 SungTae Start : [추가] Log 추가
                                _SetLog($"[UNLOADER][SEND](H<-E) S6F11 CEID = 999400(Magazine_Verify_Request).  MgzID : {CData.Parts[(int)EPart.OFL].sMGZ_ID}, Mgz Port : {(uint)SECSGEM.JSCK.eMGZPort.GOOD}.");
                                // 2022.03.31 SungTae End
                            }

                            m_Delay.Set_Delay(60000);

                            m_iStep++;
                            return false;
                        }

                        // 타임 아웃 체크
                        if (!m_Delay.Chk_Delay())
                        { return false; }

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
                                m_iStep = 0;
                                return true;
                            }
                            else if (CData.SecsGem_Data.nMGZ_Check != Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Complete)/*2*/)
                            {// Verify 진행 중.
                                if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Error)/*-1*/)
                                {// Verify Error 발생
                                    CErr.Show(eErr.HOST_ULD_MGZ_VERIFY_TIME_OVER_ERROR);
                                    m_iStep = 0;
                                    return true;
                                }
                                else if (CData.SecsGem_Data.nMGZ_Check == Convert.ToInt32(SECSGEM.JSCK.eChkVerify.Run)/*1*/)
								{
									return false;
								}
                            }
                        }

                        if (m_bxChkLight) return false;

                        SetTopAllStatusMgz(eSlotInfo.Empty);

                        //20200513 jym : QC 외에 Top 사용 추가
                        int iPt = (CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        //20191121 ghk_display_strip
                        CData.Parts[iPt].bExistStrip = true;

                        //Mgz 상태 변환
                        CData.Parts[iPt].iStat = ESeq.OFL_TopRecive;

                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else CData.Parts[iPt].iSlot_No = 0;

                        //191118 ksg :
                        if (CData.GemForm != null) CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Cnt++;

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

                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                        _SetLog(string.Format("Finish, Tack time:{0}", m_SeqTime.ToString(@"hh\:mm\:ss")));

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

            m_bxChkLight = false;

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
                        if (m_bxChkLight) return false;

                        if ((!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1)) || (!CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2)))
                        {
                            CData.Parts[(CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL].iStat = ESeq.OFL_TopPick;

                            _SetLog("Error : Not detected top magazine, Status:OFL_TopPick");
                            CErr.Show(eErr.OFFLOADER_NOT_DETECTED_TOP_MGZ_ERROR);
                            SEQ = ESeq.Idle;
                            m_iStep = 0;

                            return true;
                        }

                        //201007 jym : QC 사용 시 배제   ->   200508 pjh : Dry Position Interlock   
                        if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

                        //200630 jhc : 이동 전 자재 감지 확인

                        if (CData.Opt.bQcUse == false)
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
                        }
                        //

                        //// 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        //{
                        //    _SetLog("Error : QC Strip-out detect");
                        //    CErr.Show(eErr.QC_DETECTED_STRIP);

                        //    m_iStep = 0;
                        //    return true;
                        //}
                        //// 2020.09.28 JSKim Ed

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

                        if (m_bxChkLight) return false;

                        if (!CIO.It.Get_X(eX.OFFL_TopMGZDetect))
                        { Belt_Top(m_BeltRun); }

                        _SetLog(string.Format("Z axis move top place go, Pos:{0}mm", CData.SPos.dOFL_Z_TPlaceGo));

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        if (m_bxChkLight) return false;

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
                        if (m_bxChkLight) return false;

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

                        if (m_bxChkLight) return false;

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
                        if (m_bxChkLight) return false;

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
                        int iPt = (CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;

                        //20191121 ghk_display_strip
                        CData.Parts[iPt].bExistStrip = false;

                        //Data 이동 및 상태 변환
                        CData.Parts[iPt].iStat = ESeq.OFL_Wait;

                        SetTopAllStatusMgz(eSlotInfo.Empty);
                        //190103 ksg : Dry Out에서 iSlot_No을 확인 하는데 초기화 하는 부분이 없음.
                        //190503 ksg :
                        //CData.Parts[(int)EPart.OFL].iSlot_No = 0;
                        if (CData.Opt.bFirstTopSlotOff) CData.Parts[iPt].iSlot_No = CData.Parts[iPt].iSlot_info.Length - 1;
                        else CData.Parts[iPt].iSlot_No = 0;

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
            /*
            if (m_iPreStep != m_iStep)
            {
                m_TiOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TiOut.Chk_Delay())
                {
                    //CErr.Show(EErr. Timeout)
                    m_iStep = 0;

                    return true;
                }
            }
            */

            //20191121 ghk_display_strip
            //if (!Get_TopMgzChk())
            if(false)
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


                        // QC Vision을 사용하지 않는다면 Dry Zone에서 넘어오는 제품과의 간섭 check 필요
                        if (!CData.Opt.bQcUse)
                        {
                            //200630 jhc
                            //if (CSq_Dry.It.m_DuringDryOut)
                            if(false)
                            {
                                m_iStep = 0;
                                return true;
                            }

                            //200630 jhc : 이동 전 자재 감지 확인
                            //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                            if(false)
                            {
                                _SetLog("Error : Strip-out detect");
                                CErr.Show(eErr.DRY_DETECTED_STRIP);

                                m_iStep = 0;
                                return true;
                            }

                            // 2021.11.18 SungTae Start : [추가] AutoRun()에서 Dry Pusher Overload 상시 체크.
                            //센서에서 L선을 자를것.(Ase Kr, Qorvo,Sky Works), 모든 사이트  적용 되면 지울 것
                            //if (!CIO.It.Get_X(eX.DRY_PusherOverload))
                            if (false)
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

                            // 2020-11-24, jhLee : Emit 동작을 재시도해야하는가 ? Emit 동작만 재시도 해야하므로 실행전 조건 체크 및 사전 동작은 생략한다.
                            // QC로부터 Emit OK가 수신되었고 아직 Emit 성공을 못한 경우에는 재시도 동작으로 인지한다.
                            //if (CTcpIp.It.bEmit_Ok && !CTcpIp.It.bSuccess_Ok)
                            if ((CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG) && !CQcVisionCom.rcvQCSendEnd)
                            {
                                m_TSendDelay.Set_Delay(1);

                                _SetLog("QC Emit Retry detected, go to Emit Retry");

                                m_iStep = 18;   // 사전 처리과정은 Skip 하고 Emit success 확인을 하는 곳으로 jump 한다.
                                return false;
                            }


                            // 2020.09.28 JSKim St
                            //if (Get_QcVisionPermission() == false)
                            if(false)
                            {
                                _SetLog("Error : QC Strip-out detect, TopReceive");
                                CErr.Show(eErr.QC_DETECTED_STRIP);

                                m_iStep = 0;
                                return true;
                            }
                            // 2020.09.28 JSKim Ed
                        }

#if false //20200611 jhc : 임시 삭제 (아래 코드는 Dry X축 Push 동작 중일 경우 항상 Error 발생)
                        //200710 jym : 조건 변경   ->   200508 pjh : Dry Position Interlock                        
                        //if (!CMot.It.Get_Mv(m_iDX, CData.SPos.dDRY_X_Wait))
                        //if (CMot.It.Get_FP(m_iDX) > CData.SPos.dDRY_X_Wait + 5)
                            if (CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //
#endif

                        //Act_TopClampOC(m_ActClose);
                        _SetLog("Check Safety & Top clamp close");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Check Safety & Top clamp close");
                        m_iStep++;
                        return false;
                    }

                case 11:
                    {
                        //if (!Act_TopClampOC(m_ActClose))
                        //{ return false; }

                        //Act_TopClampModuleDU(m_ActDn);
                        _SetLog("Top clamp down");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Top clamp down");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        //if (!Act_TopClampModuleDU(m_ActDn))
                        //{ return false; }

                        //CMot.It.Mv_N(m_iY, CData.Dev.dOffL_Y_Wait);

                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Y axis move wait, Pos:");

                        _SetLog(string.Format("Y axis move wait, Pos:{0}mm", CData.Dev.dOffL_Y_Wait));

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        //if (!CMot.It.Get_Mv(m_iY, CData.Dev.dOffL_Y_Wait))
                        //{ return false; }

                        //if (GetTopFullSlotCheck())
                        if(false)
                        {
                            _SetLog("Top magazine slot full, Status:OFL_TopPlace");
                            CData.Parts[(CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL].iStat = ESeq.OFL_TopPlace;

                            m_iStep = 0;
                            return true;
                        }

                        m_dMovePos = GetTopWorkPos();               // 이동해야 할 Slot 위치 계산

                        //CMot.It.Mv_N(m_iZ, m_dMovePos);             // Slot 위치로 Z축 이동
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Z axis move work, Pos:");
                        _SetLog(string.Format("Z axis move work, Pos:{0}mm", m_dMovePos));

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        
                        /*if (!CMot.It.Get_Mv(m_iZ, m_dMovePos))
                        {
                            if (CMot.It.Get_Stop(m_iZ))         // 만약 축이 멈춰있다면 
                            {
                                CMot.It.Mv_N(m_iZ, m_dMovePos); // 다시 이동 명령을 내린다.
                            }

                            return false;
                        }*/

                        _SetLog("Z axis move check");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Z axis move check");

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (CData.Opt.bQcUse)
                        {
                            if (m_bDuringFOut)
                            {
                                _SetLog("QC use, DuringFOut true");
                                //CTcpIp.It.bEmit_Fail = false;
                                //CTcpIp.It.bEmit_Ok = false;
                                CQcVisionCom.rcvQCSendOutGOOD = CQcVisionCom.rcvQCSendOutGOOD = false;

                                Thread.Sleep(200);
                                vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] QC use, DuringFOut true");
                                //int SlotNum = GetTopEmptySlot();
                                //int SlotNum = 0;
                                CQcVisionCom.rcvQCSendOutNG = CQcVisionCom.rcvQCSendOutGOOD = false;

                                //CData.Parts[(int)EPart.OFR].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
                                m_TSendDelay.Set_Delay(1);
                                m_iStep = 18;
                            }
                            else
                            {
                                m_EndSeq = DateTime.Now; //190807 ksg :
                                m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                                _SetLog("Finish, QC use, Tack time:");
                                Thread.Sleep(200);
                                vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Finish, QC use, Tack time");

                                m_iStep = 0;
                            }
                        }
                        else
                        {
                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                            m_LogVal.iStep = m_iStep;
                            m_LogVal.sMsg = " m_bDuringFOut = " + m_bDuringFOut.ToString() + ", CData.Opt.bQcUse = " + CData.Opt.bQcUse.ToString();
                            m_LogVal.sMsg += "\r\n Time_Ofl Top Recive : " + m_SeqTime.ToString(@"hh\:mm\:ss"); //190708 ksg :
                            m_iStep = 0;
                        }

                        SaveLog();
                        return false;
                    }

                case 16:    // QC Vision에게 배출 요청 전송 : Emit
                    {
                        if (!m_TSendDelay.Chk_Delay()) return false;

                        //CTcpIp.It.SendEmit();
                        CGxSocket.It.SendMessage("QCSendRequest");//

                        _SetLog("[SEND](EQ->QC) QCSendRequest");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                        m_Delay.Set_Delay(30000);

                        _SetLog("Send Emit command -> QC");

                        m_iStep++;
                        return false;
                    }

                case 17:
                    {
                        //if(!CTcpIp.It.bEmit_Fail && !CTcpIp.It.bEmit_Ok) 
                        //if (!CQcVisionCom.rcvQCReadyQueryGOOD && !CQcVisionCom.rcvQCReadyQueryNG)
                        if(!CQcVisionCom.rcvQCSendRequestACK)
                        {
                            if (!m_Delay.Chk_Delay())
                            {
                                //m_TSendDelay.Set_Delay(2000);

                                m_iStep = 16;
                                return false;               // Emit 명령 전송 반복
                            }
                            else
                            {
                                _SetLog("Error : Emit command reply timeout from QC");


                                m_bDuringFOut = false;
                                //CTcpIp.It.bEmit_Fail = false;
                                //CTcpIp.It.bEmit_Ok   = false;
                                CQcVisionCom.rcvQCSendRequestACK = false;
                                //CQcVisionCom.rcvQCReadyQueryGOOD = false;//OK strip 받을수 있는지 문의 QC->EQ
                                //CQcVisionCom.rcvQCReadyQueryNG = false;//OK strip 받을수 있는지 문의 QC->EQ
                                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                                m_iStep = 0;                // 처음부터 다시 시작
                                return false;
                            }
                        }

                        // QC로부터 전송 취소가 수신되었는가 ?
                        if (CQcVisionCom.rcvQCAbortTransfer)
                        {
                            CQcVisionCom.rcvQCAbortTransfer = false;
                            _SetLog("Error : Emit command Fail <- QC");

                            m_bDuringFOut = false;
                            //CTcpIp.It.bEmit_Ok   = false;
                            //CTcpIp.It.bEmit_Fail = false;

                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                            m_iStep = 0;
                            return false;
                        }
                        //else if (CTcpIp.It.bEmit_Ok)            // Emit 명령 수렴
                        //else if (CQcVisionCom.rcvQCReadyQueryGOOD || CQcVisionCom.rcvQCReadyQueryNG)
                        else if (CQcVisionCom.rcvQCSendEnd)
                        {
                            m_Delay.Set_Delay(30000);

                            _SetLog("Emit command OK <- QC");

                            Thread.Sleep(200);
                            vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Emit command OK <- QC");

                            m_iStep++;
                            return false;
                        }

                        return false;
                    }

                case 18:
                    {
                        //CTcpIp.It.bSuccess_Du   = false;
                        //CTcpIp.It.bSuccess_Ok   = false;
                        //CTcpIp.It.bSuccess_Fail = false;

                        CQcVisionCom.rcvQCReadyQueryGOOD = false;
                        CQcVisionCom.rcvQCReadyQueryNG = false;


                        m_Delay.Set_Delay(30000);
                        m_TSendDelay.Set_Delay(1);

                        m_iStep++;
                        return false;
                    }

                case 19:   // 배출 작업을 마쳤는가 ?
                    {
                        //if (!m_TSendDelay.Chk_Delay()) return false;

                        m_TSendDelay.Set_Delay(3000);
                        //CTcpIp.It.SendSucces();                     // 배출 성공 여부 조회
                        //if (CQcVisionCom.rcvQCSendOutGOOD || CQcVisionCom.rcvQCSendOutNG)
                        if(CQcVisionCom.rcvQCSendEnd)
                        {
                            _SetLog("QC start transferring QC");

                            Thread.Sleep(200);
                            vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] QC start transferring QC");

                            m_iStep++;

                        }
                        else if (m_TSendDelay.Chk_Delay())
                        {
                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                            m_iStep = 0;
                            return false;

                        }

                        return false;
                    }

                case 20:    // 배출 동작중인가 ?
                    {
                        //if(!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail) 
                        if (!CQcVisionCom.rcvQCSendEnd)
                        {
                            if (!m_Delay.Chk_Delay())
                            {
                                //m_TSendDelay.Wait(2000);
                                m_TSendDelay.Set_Delay(2000);
                                //딜레이 후 한번 더 확인
                                //if (!CTcpIp.It.bSuccess_Ok && !CTcpIp.It.bSuccess_Fail)
                                if (!CQcVisionCom.rcvQCSendEnd)
                                {
                                    _SetLog("Wait Emit success result");

                                    //m_LogVal.iStep = m_iStep;
                                    //m_LogVal.sMsg = "m_TSendDelay && goto 19 Step";
                                    //SaveLog();


                                    Thread.Sleep(200);
                                    vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Wait QC -> SendEnd Msg");

                                    m_iStep = 19;        // 배출 완료 여부를 다시 묻는다.
                                    return false;
                                }
                            }
                            else      // 30초 초과
                            {
                                m_bDuringGOut = false;
                                //CTcpIp.It.bSuccess_Du   = false;
                                //CTcpIp.It.bSuccess_Ok   = false;
                                //CTcpIp.It.bSuccess_Fail = false;
                                CQcVisionCom.rcvQCSendEnd = false;
                                _SetLog("Error : Emit success query timeout from QC");

                                CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                                m_iStep = 0;
                                return false;
                            }
                        }

                        m_iStep = 22;           // 수신 결과 처리
                        return false;
                    }

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

                case 22:
                    {
                        //if(CTcpIp.It.bSuccess_Ok)    // 배출 성공
                        if (CQcVisionCom.rcvQCSendEnd)
                        {
                            m_bDuringFOut = false;
                            //int SlotNum = GetTopEmptySlot();
                            //int SlotNum = 0;
                            CQcVisionCom.rcvQCSendOutNG = CQcVisionCom.rcvQCSendOutGOOD = false;

                            //CData.Parts[(int)EPart.OFR].iSlot_info[SlotNum] = (int)eSlotInfo.Exist;
                            //190731 ksg :
                            //----------------
                            try
                            {
                                //CData.Parts[(int)EPart.OFR].sLotName = CTcpIp.It.sStringInfo[0];
                                //CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt32(CTcpIp.It.sStringInfo[1]);
                                //CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt32(CTcpIp.It.sStringInfo[2]);
                                CData.Parts[(int)EPart.OFR].sLotName = CQcVisionCom.sStringInfo[0];
                                CData.Parts[(int)EPart.OFR].iMGZ_No = Convert.ToInt32(CQcVisionCom.sStringInfo[1]);
                                CData.Parts[(int)EPart.OFR].iSlot_No = Convert.ToInt32(CQcVisionCom.sStringInfo[2]);
                            }
                            catch
                            {
                                CData.Parts[(int)EPart.OFR].sLotName = "Exception";
                                CData.Parts[(int)EPart.OFR].iMGZ_No = 0;
                                CData.Parts[(int)EPart.OFR].iSlot_No = 0;
                            }
                            //CData.Parts[(int)EPart.OFR].group  = CTcpIp.It.sStringInfo[3]; <-Group 없음
                            //CData.Parts[(int)EPart.OFR].device = CTcpIp.It.sStringInfo[4]; <-Deivce 없음
                            //----------------
                            CData.LotInfo.iTOutCnt++; //190407 ksg :
                            //CTcpIp.It.bResultFail = false;
                            CQcVisionCom.rcvResultQuery = false;
                            CQcVisionCom.rcvQCReadyQueryNG = false;
                            m_EndSeq = DateTime.Now; //190807 ksg :
                            m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :

                            _SetLog("Emit success OK <- QC");

                            Thread.Sleep(200);
                            vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Emit success OK <- QC");

                            // 더이상 Emit Retry는 없다.
                            //CTcpIp.It.bEmit_Ok = false;        // 이 Flag가 false가 되면 Retry 대기는 없다.
                            //CTcpIp.It.bEmit_Fail = false;

                            CQcVisionCom.rcvQCSendOutGOOD = false;
                            CQcVisionCom.rcvQCSendOutNG = false;

                            //200316 ksg :
                            if (!CData.Opt.bQcByPass)                     // QC Vision Bypass가 아닌경우 
                            {
                                //CTcpIp.It.bTResult_Ok = false;
                                //CTcpIp.It.SendTestResult();               // 결과를 받아온다.
                                CQcVisionCom.rcvResultQuery = false;
                                CGxSocket.It.SendMessage("ResultQuery");

                                _SetLog("[SEND](EQ->QC) ResultQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가

                                m_Delay.Set_Delay(10000);
                                m_TSendDelay.Set_Delay(1000);

                                m_iStep++; //200316 ksg :
                                return false;
                            }
                            else
                            {
                                Thread.Sleep(200);
                                vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] QC Bypass, Top Receive Done");
                                _SetLog("QC Bypass, Top Receive Done");
                                m_iStep = 0;
                                return true;                             // 2020-11-20, jhLee : 배출 성공으로 끝낸다.
                            }
                        }

                        // 배출 실패
                        // Emit 명령 반복 수행 필요

                        _SetLog("Error : Emit success Fail <- QC");
                        Thread.Sleep(200);
                        vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Error : Emit success Fail <- QC");

                        _SetLog("QC Bypass, Top Receive Done");
                        m_bDuringFOut = false;

                        CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                        m_iStep = 0;
                        return false;
                    }
                //200316 ksg :

                case 23: // QC에게 Test 결과를 묻는다.
                    {
                        //if(!m_Delay.Chk_Delay() && !CTcpIp.It.bTResult_Ok)
                        if (!m_Delay.Chk_Delay() && !CQcVisionCom.rcvResultQuery)
                        {
                            if (m_TSendDelay.Chk_Delay())
                            {
                                //CTcpIp.It.SendTestResult();
                                CGxSocket.It.SendMessage("ResultQuery");

                                _SetLog("[SEND](EQ->QC) ResultQuery");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                                
                                _SetLog("Retry Test result query -> QC");
                                vmMan_13QcVision.m_qcVision.addMsgList("Ofl [Cyl_TopRecieve()] Retry Test result query -> QC");
                                m_TSendDelay.Set_Delay(1000);
                                return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        //else if(CTcpIp.It.bTResult_Ok)
                        else if (CQcVisionCom.rcvResultQuery)
                        {
                            m_iStep = 0;
                            return true;
                        }
                        else
                        {
                            _SetLog("Error : QC Vision Test result receive fail");

                            CErr.Show(eErr.QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR);

                            m_iStep = 0;
                            return true;
                        }
                    }
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
                    m_iStep = 0;

                    return false;
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


                        if (CData.Opt.bQcUse == false)
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
                        }

                        //if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                        //{
                        //    CErr.Show(eErr.DRY_DETECTED_STRIP);
                        //    m_iStep = 0;
                        //    return true;
                        //}

                        //// 2020.09.28 JSKim St
                        //if (Get_QcVisionPermission() == false)
                        //{
                        //    _SetLog("Error : QC Strip-out detect, PlaceLotEnd");

                        //    CErr.Show(eErr.QC_DETECTED_STRIP);
                        //    m_iStep = 0;
                        //    return true;
                        //}
                        //// 2020.09.28 JSKim Ed

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
                        if (!CData.Opt.bQcUse && CMot.It.Get_FP(m_iDX) > 5)
                        {
                            CErr.Show(eErr.DRY_X_WAIT_POSITION_ERROR);
                            m_iStep = 0;
                            return true;
                        }
                        //

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

                        Belt_Btm(m_BeltRun);
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

                        if (!Get_TopMgzChk())
                        {//Top 매거진 없을 경우 종료 
                            CData.Parts[(int)EPart.OFR].iStat = ESeq.Idle;
                            m_iStep = 0;

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
                        { Belt_Top(m_BeltRun); }
                        else
                        { Belt_Top(m_BeltStop); }

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
                        { Belt_Top(m_BeltRun); }
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

                        m_iStep = 0;
                        return true;
                    }

            }
        }


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

            // 2020.10.27 SungTae Start : Modify
            //if(CData.Opt.bFirstTopSlot)
            if (CData.Opt.bFirstTopSlotOff) //190511 ksg :
            {
                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                    !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                {
                    int iTarget = CData.Dev.iMgzCnt - CData.Dev.iOffMgzCnt;

                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= iTarget; iCnt--)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_BRcv_Dn + (CData.Dev.dOffMgzPitch * iCnt);
                }
                else
                {
                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= 0; iCnt--)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_BRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
                }
            }
            else
            {
                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                   !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                {
                    for (iCnt = 0; iCnt < CData.Dev.iOffMgzCnt; iCnt++)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_BRcv_Dn + (CData.Dev.dOffMgzPitch * iCnt);
                }
                else
                {
                    for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_BRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
                }
            }
            // 2020.10.27 SungTae End

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
            //20200513 jhc : 매거진 배출 위치 변경 추가
            int iPart = (CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL;
            //

            /*
            for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
            {
                if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                { break; }
            }

            dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
            */

            // 2020.10.27 SungTae Start : Modify
            //if(CData.Opt.bFirstTopSlot)
            if (CData.Opt.bFirstTopSlotOff) //190511 ksg :
            {
                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                    !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                {
                    int iTarget = CData.Dev.iMgzCnt - CData.Dev.iOffMgzCnt;

                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= iTarget; iCnt--)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dOffMgzPitch * iCnt);
                }
                else
                {
                    for (iCnt = CData.Dev.iMgzCnt - 1; iCnt >= 0; iCnt--)
                    {
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if (CData.Parts[iPart].iSlot_info[iCnt] == (int)eSlotInfo.Empty) //if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
                }
            }
            else
            {
                // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                    !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                {
                    for (iCnt = 0; iCnt < CData.Dev.iOffMgzCnt; iCnt++)
                    {
                        if (CData.Parts[(int)EPart.OFL].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dOffMgzPitch * iCnt);
                }
                else
                {
                    for (iCnt = 0; iCnt < CData.Dev.iMgzCnt; iCnt++)
                    {
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if (CData.Parts[iPart].iSlot_info[iCnt] == (int)eSlotInfo.Empty) //if (CData.Parts[(int)EPart.OFR].iSlot_info[iCnt] == (int)eSlotInfo.Empty)
                        { break; }
                    }

                    dPos = CData.Dev.dOffL_Z_TRcv_Dn + (CData.Dev.dMgzPitch * iCnt);
                }
            }
            // 2020.10.27 SungTae End

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
            if (CData.Opt.bFirstTopSlotOff) //190511 ksg :
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
            if (CData.Opt.bFirstTopSlotOff) //190511 ksg :
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
            return iCnt;
        }
        #endregion

        #region GetFullSlotCheck
        /// <summary>
        /// 현재 매거진의 슬롯이 Full 인지 확인
        /// </summary>
        /// <returns></returns>
        private bool GetFullSlotCheck()
        {
            bool bRet = true;

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
            if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
            {
                for (int i = 0; i < CData.Dev.iOffMgzCnt; i++)
                {
                    if (CData.Parts[(int)EPart.OFL].iSlot_info[i] == 0)
                    {
                        bRet = false;

                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < CData.Dev.iMgzCnt; i++)
                {
                    if (CData.Parts[(int)EPart.OFL].iSlot_info[i] == 0)
                    {
                        bRet = false;

                        break;
                    }
                }
            }
            // 2020.10.28 SungTae End

            return bRet;
        }

        /// <summary>
        /// OffLoader MGZ Get Full Slot
        /// </summary>
        /// <returns></returns>
        private bool GetTopFullSlotCheck()
        {
            bool bRet = true;

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
            if ((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
            {
                for (int i = 0; i < CData.Dev.iOffMgzCnt; i++)
                {
                    if (CData.Parts[(CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL].iSlot_info[i] == 0)
                    {
                        bRet = false;

                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < CData.Dev.iMgzCnt; i++)
                {
                    //20200605 jhc : Top 매거진 배출 옵션에서 슬롯 full 일때 TopPlace 않고 멈춰 있는 이슈 개선
                    if (CData.Parts[(CData.Opt.bQcUse) ? (int)EPart.OFR : (int)EPart.OFL].iSlot_info[i] == 0)
                    //if (CData.Parts[(int)EPart.OFR].iSlot_info[i] == 0)
                    {
                        bRet = false;

                        break;
                    }
                }
            }
            // 2020.10.28 SungTae End

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
            if (CData.Opt.bQcUse)
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
            CIO.It.Set_Y(eY.OFFL_TopClampOn, bClose);    //YC4
            CIO.It.Set_Y(eY.OFFL_TopClampOff, !bClose);    //YC5

            if (bClose) { bRet = (CIO.It.Get_X(eX.OFFL_TopClampOn)) && (!CIO.It.Get_X(eX.OFFL_TopClampOff)); }   //XC4
            else { bRet = (!CIO.It.Get_X(eX.OFFL_TopClampOn)) && (CIO.It.Get_X(eX.OFFL_TopClampOff)); }   //XC5

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
            CIO.It.Set_Y(eY.OFFL_TopMGZUp, bUp);    //YC2
            CIO.It.Set_Y(eY.OFFL_TopMGZDn, !bUp);    //YC3

            if (bUp) { bRet = (CIO.It.Get_X(eX.OFFL_TopMGZUp)) && (!CIO.It.Get_X(eX.OFFL_TopMGZDn)); }   //XC2
            else { bRet = (!CIO.It.Get_X(eX.OFFL_TopMGZUp)) && (CIO.It.Get_X(eX.OFFL_TopMGZDn)); }   //XC3

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
            CIO.It.Set_Y(eY.OFFL_BtmClampOn, bClose);    //YCC
            CIO.It.Set_Y(eY.OFFL_BtmClampOff, !bClose);    //YCD

            if (bClose) { bRet = (CIO.It.Get_X(eX.OFFL_BtmClampOn)) && (!CIO.It.Get_X(eX.OFFL_BtmClampOff)); }    //XCC
            else { bRet = (!CIO.It.Get_X(eX.OFFL_BtmClampOn)) && (CIO.It.Get_X(eX.OFFL_BtmClampOff)); }    //XCD

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

            CIO.It.Set_Y(eY.OFFL_BtmMGZUp, bUp);    //YCA
            CIO.It.Set_Y(eY.OFFL_BtmMGZDn, !bUp);    //YCB

            if (bUp) { bRet = (CIO.It.Get_X(eX.OFFL_BtmMGZUp)) && (!CIO.It.Get_X(eX.OFFL_BtmMGZDn)); } //XCA
            else { bRet = (!CIO.It.Get_X(eX.OFFL_BtmMGZUp)) && (CIO.It.Get_X(eX.OFFL_BtmMGZDn)); } //XCB

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
            CIO.It.Set_Y(eY.OFFL_MidBeltCCW, bCw);    //YBC
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
            if (CData.Opt.bQcSimulation)
            {
                Thread.Sleep(200);
                return false;
            }
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
            if (CData.Opt.bQcUse)
            {
                bRet = (CData.Parts[(int)EPart.INR].bExistStrip == false) &&
                     (CData.Parts[(int)EPart.ONP].bExistStrip == false) &&
                     (CData.Parts[(int)EPart.GRDL].bExistStrip == false) &&
                     (CData.Parts[(int)EPart.GRDR].bExistStrip == false) &&
                     (CData.Parts[(int)EPart.OFP].bExistStrip == false) &&
                     (CData.Parts[(int)EPart.DRY].bExistStrip == false) &&
                     (CQcVisionCom.nQCexistStrip == 0);
                //(m_GetQcWorkEnd == true);
                //(m_ChkQcStrip                             == false);
            }
            else
            {
                bRet = (CData.Parts[(int)EPart.INR].bExistStrip == false) &&
                       (CData.Parts[(int)EPart.ONP].bExistStrip == false) &&
                       (CData.Parts[(int)EPart.GRDL].bExistStrip == false) &&
                       (CData.Parts[(int)EPart.GRDR].bExistStrip == false) &&
                       (CData.Parts[(int)EPart.OFP].bExistStrip == false) &&
                       (CData.Parts[(int)EPart.DRY].bExistStrip == false);
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
        #endregion


        public int QcVisionPermission()
        {
            if (CData.Opt.bQcUse == false)
            {
                return 0;
            }

            //Thread.Sleep(200);

            CQcVisionCom.nGet_QcVisionPermissionCnt++;

            if (CQcVisionCom.nQcVisionPermissionTO > CQcVisionCom.nGet_QcVisionPermissionCnt)
            {
                CQcVisionCom.nGet_QcVisionPermissionCnt = 0;
                return 2;//Error Occur
            }

            if (CQcVisionCom.nMgzMoving == 0)
            {
                return 0;
            }

            //Magazine Interrupt 
            //0 : 간섭이 없는 안전한 상태이다.
            //1 : 배출 동작중으로 간섭이 발생할 수 있는 상태이다

            //return CTcpIp.It.bPermission;
            return 1;
        }



        // 2020.09.28 JSKim St
        public bool Get_QcVisionPermission()
        {
            if (CData.Opt.bQcUse == false)
            {
                return true;
            }

            //CTcpIp.It.SendPermission();
            //m_Permission.Set_Delay(10000);
            m_Permission.Set_Delay(3000);

            do
            {
                if (m_Permission.Chk_Delay() == true)
                {
                    return false;
                }
                Thread.Sleep(10);
            }
            //while (CTcpIp.It.bRecivePermission == false);
            while (CQcVisionCom.rcvPermission == false);

            //return CTcpIp.It.bPermission;
            return CQcVisionCom.rcvPermission;
        }
        // 2020.09.28 JSKim Ed

        #region  Log 구조체 초기화
        /// <summary>
        /// Log 구조체 초기화
        /// </summary>
        //190218 ksg :
        public void LogclearVal()
        {
            m_LogVal.iStep = 0;
            m_LogVal.sAsix1 = "";
            m_LogVal.dPos1 = -1;
            m_LogVal.sAsix2 = "";
            m_LogVal.dPos2 = -1;
            m_LogVal.sMsg = "";
        }
        #endregion

        #region Log 저장
        /// <summary>
        /// Log 저장 함수
        /// </summary>
        //190218 ksg :
        public void SaveLog()
        {
            return;

            string sLine = "";
            string sDir, sPath, sDay, sHour, sStep, sPos1, sPos2;

            DateTime Now = DateTime.Now;

            sDir = GV.PATH_LOG + Now.Year.ToString("0000") + "\\" + Now.Month.ToString("00") + "\\" + Now.Day.ToString("00") + "\\OFL\\";

            //if (!Directory.Exists(sDir.ToString()))
            //{
              //  Directory.CreateDirectory(sDir.ToString());
            //}

            sDay = DateTime.Now.ToString("[yyyyMMdd-HHmmss fff]");
            sHour = DateTime.Now.ToString("yyyyMMddHH");
            sStep = m_LogVal.iStep.ToString();
            sPos1 = m_LogVal.dPos1.ToString();
            sPos2 = m_LogVal.dPos2.ToString();

            sLine = sDay + "\t";
            sLine += m_LogVal.sStatus + "\t";
            sLine += sStep + "\t";
            sLine += m_LogVal.sAsix1 + "\t";
            sLine += sPos1 + "\t";
            sLine += m_LogVal.sAsix2 + "\t";
            sLine += sPos2 + "\t";

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
