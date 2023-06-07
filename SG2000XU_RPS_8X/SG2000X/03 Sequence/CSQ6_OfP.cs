using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms; //210907 syc : 2004U

namespace SG2000X
{
    public class CSq_OfP : CStn<CSq_OfP>
    {
        private readonly int TIMEOUT = 30000;
        public bool m_bHD { get; set; }

        public int Step { get { return m_iStep; } }
        private int m_iStep = 0;
        private int m_iPreStep = 0;

        private int m_CleanCnt = 0;
        /// <summary>
        /// Btm Strip Clean, Spray Nozzle 사이클 수행시 Device에서 설정한 #1, #2 Count 만큼 수행
        /// 0 = #1의 Count, 1 = #2의 Count 사용
        /// </summary>
        public int m_nStripNzlCycle = 0;   // 2022.07.28 lhs

        private int m_iX    = (int)EAx.OffLoaderPicker_X;
        private int m_iZ    = (int)EAx.OffLoaderPicker_Z;
        private int m_iLTY  = (int)EAx.LeftGrindZone_Y  ;
        private int m_iRTY  = (int)EAx.RightGrindZone_Y ;
        private int m_iDYZ  = (int)EAx.DryZone_Z        ;
        private int m_iONPX = (int)EAx.OnLoaderPicker_X ;     
        
        // 20190822 pjh_offp_conversion
        private int m_iLGZ = (int)EAx.LeftGrindZone_Z;
        private int m_iRGZ = (int)EAx.RightGrindZone_Z;

        /// <summary>
        /// Picker pick 동작에서 에러 발생 시 설정 딜레이 [ms]
        /// </summary>
        private int m_iErrDelay = 1000;
        //200407 jym : 임시로 저장되는 딜레이, 갭 변수 추가
        /// <summary>
        /// 임시로 사용되는 갭 값 [mm]
        /// </summary>
        private double m_dTempGap = 0;
        //

        private bool bxVacChk;
        private bool bxVacFree; // 진공해제 확인용
        private bool m_binv  ;
        private bool m_bReady;

        private bool m_bReqStop = false;

        private CTim m_Delay    = new CTim();
        private CTim m_TimeOut  = new CTim();
        private CTim m_Place    = new CTim();   // 20200415 jhc : Picker Idle Time 개선
        private CTim m_Wait     = new CTim();   // 2022.03.10 lhs : Wait용 딜레이
        private int m_nWait_ms  = 500;
        //20190926 ghk_ofppadclean
        private CTim m_OfpPadDelay = new CTim();
        private double m_dPos_X; // X축 위치    
        private double m_dPos_Z; // Z축 위치
        private int m_iPadCnt;
        private int m_nRetryCnt = 0;

        //
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
        public int iOFPPara = 1;
        /// <summary>
        /// 임시 X축 위치 변수
        /// </summary>
        public double dIV2Xpos = 0;
        /// <summary>
        /// 임시 Z축 위치 변수
        /// </summary>
        public double dIV2Zpos = 0;
        /// <summary>
        /// 임시 IV2 파라미터 변수
        /// </summary>
        public string SIV2Para = "000";


        //190807 ksg : TackTime 
        private DateTime m_StartSeq;
        private DateTime m_EndSeq  ;
        private TimeSpan m_SeqTime ;


        private DateTime m_tStTime;
        private TimeSpan m_tTackTime ;

        public ESeq iSeq;
        public ESeq SEQ = ESeq.Idle;

        //20200415 jhc : Picker Idle Time 개선
        // Error 발생 시에 Z-Axis Up 하기 위한 변수 20200301 LCY
        private bool bError_Safety_Mv = false;

        //210830 syc : 2004U IV2 Start  
        private CIV2Ctrl pCtrlIV2 = new CIV2Ctrl(1);

        public TPart m_tTarget = new TPart();

        public void initIV2Ctrl(string sIP, int nPort)
        {
            pCtrlIV2.SetAddress(sIP, nPort);
        }

        /// <summary>
        /// IV2 Connect
        /// </summary>
        public void ConnectIV2()
        {
            pCtrlIV2.Connect();
        }

        /// <summary>
        /// IV2 Connect Close
        /// </summary>
        public void CloseIV2()
        {
            pCtrlIV2.Close();
        }

        /// <summary>
        /// IV2 Trigger
        /// </summary>
        public void TriggerIV2()
        {
            pCtrlIV2.SendTrigger();
        }

        /// <summary>
        /// IV2 Send Msg
        /// </summary>
        /// <param name="sMsg"></param>
        public void SendMsg(string sMsg)
        {
            pCtrlIV2.SendString(sMsg);
        }

        /// <summary>
        /// IV2 Log 표시용
        /// </summary>
        /// <param name="pLi"></param>
        public void ShowLog(ListBox pLi)
        {
            pCtrlIV2.LogList(pLi);
        }

        /// <summary>
        /// IV2 프로그램 변경
        /// </summary>
        /// <param name="sPrg"></param>
        public void SendPrg(string sPrg)
        {
            int iPrg = 0;
            iPrg = int.Parse(sPrg);
            pCtrlIV2.SendProgramWrite(iPrg);
        }

        /// <summary>
        /// IV2 현재 프로그램 읽기
        /// </summary>
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
        //end

        private CSq_OfP()
        {
            m_bHD = false;
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
            if(m_iStep != 0 ) return false;
            return true;
        }

        public bool Stop()
        {
            m_iStep = 0;
            return true;
        }

        public bool AutoRun()
        {
            /*
            if (CSQ_Main.It.m_iStat == eStatus.Stop)
            {
                return false;
            }
            */
            iSeq = CData.Parts[(int)EPart.OFP].iStat;
            //ksg
           // if (CSQ_Main.It.m_iStat == eStatus.Error || CSQ_Main.It.m_iStat != eStatus.Auto_Running)
           // { return false; }

            if (SEQ == (int)ESeq.Idle)
            {
                if (m_bReqStop) return false;
                if (CSQ_Main.It.m_iStat == EStatus.Stop)
                { return false; }

                //if(CSQ_Main.It.m_bPause) return false; //190506 ksg :

                bool bCleanBtm   = false;
                bool bCleanStrip = false;
                bool bCleanBrush = false;   // 2022.07.27 lhs : Brush로 Strip Clean
                bool bPickTbL    = false;
                bool bPickTbR    = false;
                //20200415 jhc : Picker Idle Time 개선
                bool bPlaceTbR   = false;  //Off-Picker Place R-Table
                bool bWaitPickTbR = false; //Off-Picker Wait Pick R-Table (Postion = On the R-Table)
                bool bWaitPickTbL = false; //Off-Picker Wait Pick L-Table (Postion = On the L-Table)
                //
                bool bPlace         = false;
                // 2021.04.11 lhs Start                
                bool bIVDryTable        = false;    // 2022.07.07 lhs : IV로 Dryer Table을 촬영하여 Strip 존재 여부 체크                
                bool bIVDryClamp        = false;
                bool bCoverDryer        = false;
                bool bCoverDryerKeep    = false;
                bool bCoverDryerEnd     = false;
                // 2021.04.11 lhs End
                bool bIV2               = false;    //210908 syc : 2004U
                bool bCoverIV2          = false;    //210915 syc : 2004U
                bool bCoverPick         = false;    //210915 syc : 2004U
                bool bCoverPlace        = false;    //210929 syc : 2004U


                //20200415 jhc : Picker Idle Time 개선 ////////////////////////
                if (CDataOption.PickerImprove == ePickerTimeImprove.Use)
                {//Picker Idle Time 개선 기능 사용하는 경우 /////////////////////

                    if (iSeq == ESeq.OFP_Wait       || iSeq == ESeq.Idle           || 
                        iSeq == ESeq.OFP_PickTbR    || iSeq == ESeq.OFP_PickTbL    || 
                        iSeq == ESeq.OFP_PlaceTbR   )
                    {
                        if (CData.Dev.bDual == eDual.Dual)
                        {//듀얼 모드 일 경우
                            //--------------------------------------
                            // Off-Picker R-Table Pick Wait 조건 //
                            if ((iSeq == ESeq.OFP_Wait || iSeq == ESeq.Idle) &&                          //Off-Picker WAIT 또는 IDLE
                                (CData.Parts[(int)EPart.OFP].bExistStrip == false) &&                    //Off-Picker 자재 없음
                                (CData.Parts[(int)EPart.GRDR].bExistStrip == true) &&                    //R-Table 자재 있음
                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= CData.SPos.dONP_X_PlaceL)) //On-Picker 안전 위치
                            {
                                { bWaitPickTbR = true; } //==> Off-Picker Wait Pick R-Table
                            }
                            //--------------------------------------
                            // Off-Picker R-Table Pick 조건 //
                            if ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick) &&             //R-Table WAIT_PICK
                                (CData.Parts[(int)EPart.OFP].bExistStrip == false) &&                    //Off-Picker 자재 없음
                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= CData.SPos.dONP_X_PlaceL)) //On-Picker 안전 위치
                            {
                                { bPickTbR = true; } //==> Off-Picker R-Table Pick Seq.  // Improve, Dual 
                            }
                            //--------------------------------------
                            // Off-Picker R-Table Place 조건 //
                            if ((iSeq == ESeq.OFP_PlaceTbR) &&                                  //Off-Picker PLACE_R_TABLE 스텝
                                                                                                //(CData.Parts[(int)EPart.OFP].bExistStrip == true) &&          //Off-Picker 자재 있음 /*HERE : 이 조건은 없어도?*/
                                (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready) &&       //R-Table READY
                                !CData.DrData[1].bDrs &&                                    //R-Table READY
                                (CData.Parts[(int)EPart.GRDR].bExistStrip == false) &&          //R-Table 자재 없음
                                (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR) &&     //On-Picker PLACE_R_TABLE 아님
                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL))) //On-Picker 안전 위치
                            {
                                { bPlaceTbR = true; } //==> Off-Picker R-Table Place Seq.
                            }
                            //--------------------------------------
                            // Off-Picker L-Table Pick 조건 //
                            if ((iSeq == ESeq.OFP_Wait || iSeq == ESeq.Idle || iSeq == ESeq.OFP_PickTbL) && //Off-Picker WAIT or IDLE state
                                (CData.Parts[(int)EPart.OFP].bExistStrip == false) &&                       //Off-Picker 자재 없음
                                (CData.Parts[(int)EPart.GRDL].bExistStrip == true) &&                       //L-Table 자재 있음
                                                                                                            //(CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick) &&              //L-Table WAIT_PICK             /*HERE : 이 조건은 뺌*/
                                (CData.Parts[(int)EPart.GRDR].bExistStrip == false) &&                      //R-Table 자재 없음
                                (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL) &&                 //On-Picker PLACE_L_TABLE 아님
                                (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL) &&                  //On-Picker PICK_L_TABLE 아님
                                (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR) &&                 //On-Picker PLACE_R_TABLE 아님
                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //On-Picker 안전 위치
                            {
                                if (bPickTbR == false && bPlaceTbR == false)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PickTbL; //Off-Picker가 L-Table Pick 조건일 경우 우선 순위 선점

                                    if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick) //L-Table WAIT_PICK
                                    { bPickTbL = true; } //==> Off-Picker L-Table Pick
                                    else
                                    { bWaitPickTbL = true; } //==> Off-Picker Wait Pick L-Table (Postion = On the L-Table)
                                }
                            }
                            //--------------------------------------
                            // Work END 조건 //
                            if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) && //L-Table WORK_END //20200416 jhc : 종료 조건 추가
                                (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) && //R-Table WORK_END
                                (iSeq == ESeq.OFP_Wait))                                    //Off-Picker IDLE
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                _SetLog(">>>>> Work End.");     // 2021.11.18 lhs

                                return true;
                            }
                            //--------------------------------------
                        }//end : if (CData.Dev.bDual == eDual.Dual)

                        else  //노멀 모드 일 경우
                        {
                            //--------------------------------------
                            // Work End
                            if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) && 
                                (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) && 
                                (iSeq == ESeq.OFP_Wait))
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                _SetLog(">>>>> Work End.");     // 2021.11.18 lhs

                                return true;
                            }
                            //--------------------------------------

                            // 2021-06-10, jhLee, Multi-LOT 사용일때는 Off-Picker가 LOT 종류를 잘못 선택하여 혼류를 유발할 수 있다. 
                            //                    투입시 Magazine에 일련번호를 확인하여 LOT가 섞이는 것을 방지하도록 한다.
                            if (CData.IsMultiLOT())
                            {
                                // _SetLog($"for PickTalble Left_MgzSN:{CData.Parts[(int)EPart.GRDL].nLoadMgzSN}, Right_MgzSN:{CData.Parts[(int)EPart.GRDR].nLoadMgzSN},");

                                if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN < CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                        {
                                            bPickTbL = true;
                                        }
                                    }
                                    else
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        {
                                            bPickTbR = true; // Improve, Nomal, MultiLot    
                                        }
                                    }
                                }

                                if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN == 0) || (CData.Parts[(int)EPart.GRDL].nLoadMgzSN > CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                {
                                    if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                        {
                                            bPickTbR = true; // Improve, Normal, MultiLot
                                        }
                                    }
                                    else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick) //200311 ksg :
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                        {
                                            bPickTbL = true;
                                        }
                                    }
                                }

                                // 동일 LOT 인 경우 (동일 Magazine)
                                if (CData.Parts[(int)EPart.GRDL].nLoadMgzSN == CData.Parts[(int)EPart.GRDR].nLoadMgzSN)
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                    }
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve, Normal, MultiLot
                                        }
                                    }
                                    if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) || CData.Parts[(int)EPart.GRDL].iSlot_No > CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve, Normal, MultiLot
                                        }
                                        //191108 ksg :
                                        /*
                                        if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbL = true; }
                                        }
                                        */
                                        else if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && (!(CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbR) && (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready)) &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                    }
                                }

                                if (CData.Opt.aTblSkip[(int)EWay.L] || CData.Opt.aTblSkip[(int)EWay.R])
                                {
                                    if (CData.Opt.aTblSkip[(int)EWay.L] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve, Normal, MultiLot
                                        }
                                        else if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                    if (CData.Opt.aTblSkip[(int)EWay.R] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                        else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                }


                                // 최종적으로... 
                                // 양 쪽 모두 Pickup 할 수 있고 서로 다른 LOT 이면 먼저 들어온 쪽 Strip을 집도록 한다.
                                //
                                if (bPickTbL && bPickTbR && (CData.Parts[(int)EPart.GRDL].sLotName != CData.Parts[(int)EPart.GRDR].sLotName))
                                {
                                    // 오른쪽 Table이 더 늦게 들어온 Strip이라면
                                    if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN < CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                    {
                                        bPickTbL = true;
                                        bPickTbR = false; // Improve, Normal, MultiLot

                                        _SetLog($"Set PickTalble-Left Left_MgzSN:{CData.Parts[(int)EPart.GRDL].nLoadMgzSN}, Right_MgzSN:{CData.Parts[(int)EPart.GRDR].nLoadMgzSN},");
                                    }
                                    else if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN > CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                    {
                                        bPickTbL = false;
                                        bPickTbR = true; // Improve, Normal, MultiLot

                                        _SetLog($"Set PickTalble-Right Left_MgzSN:{CData.Parts[(int)EPart.GRDL].nLoadMgzSN}, Right_MgzSN:{CData.Parts[(int)EPart.GRDR].nLoadMgzSN},");
                                    }
                                }

                            }
                            else  // of Multi-LOT
                            {
                                if ((CData.Parts[(int)EPart.GRDL].iMGZ_No < CData.Parts[(int)EPart.GRDR].iMGZ_No))
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   && 
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  && 
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                        { bPickTbL = true; }
                                    }
                                    //190716 ksg : 매거진 추가 시 Left에 더 공급이 없을 경우 
                                    if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        { bPickTbR = true; } // Improve, Normal, MultiLot(X)
                                    }
                                }

                                if ((CData.Parts[(int)EPart.GRDL].iMGZ_No == 0) || (CData.Parts[(int)EPart.GRDL].iMGZ_No > CData.Parts[(int)EPart.GRDR].iMGZ_No))
                                {
                                    if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                        { bPickTbR = true; } // Improve, Normal, MultiLot(X)
                                    }
                                    else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick) //200311 ksg :
                                    {
#if true //200723 jhc : On/Off Picker 충돌 방지
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
#else
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
#endif
                                        { bPickTbL = true; }
                                    }
                                }

                                if (CData.Parts[(int)EPart.GRDL].iMGZ_No == CData.Parts[(int)EPart.GRDR].iMGZ_No)
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                    }
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve, Normal, MultiLot(X)
                                        }
                                    }
                                    if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) || CData.Parts[(int)EPart.GRDL].iSlot_No > CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve, Normal, MultiLot(X)
                                        }
                                        //191108 ksg :
                                        /*
                                        if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbL = true; }
                                        }
                                        */
                                        else if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && (!(CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbR) && (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready)) &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                    }
                                }

                                if (CData.Opt.aTblSkip[(int)EWay.L] || CData.Opt.aTblSkip[(int)EWay.R])
                                {
                                    if (CData.Opt.aTblSkip[(int)EWay.L] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve, Normal, MultiLot(X)
                                        }
                                        else if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                    if (CData.Opt.aTblSkip[(int)EWay.R] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                        else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                }

                            }//of if Not Multi-LOT
                        }
                    }

                    if (iSeq == ESeq.OFP_BtmClean)                                  { bCleanBtm     = true; }
                    if (iSeq == ESeq.OFP_BtmCleanStrip)                             { bCleanStrip   = true; }
                    if (iSeq == ESeq.OFP_BtmCleanBrush)                             { bCleanBrush   = true; }
                    
                    if ((iSeq == ESeq.OFP_Place || iSeq == ESeq.OFP_WaitPlace) &&   
                        CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitPlace)    { bPlace        = true; }
                    
                    if (CDataOption.UseDryWtNozzle)     // 2021.04.15 lhs 
                    {
                        if (iSeq == ESeq.OFP_IV_DryClamp &&  CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitIV2)     { bIVDryClamp       = true; }
                        if (iSeq == ESeq.OFP_CoverDryer  &&  CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaterNozzle) { bCoverDryer       = true; }
                        if (iSeq == ESeq.OFP_CoverDryerKeep)                                                            { bCoverDryerKeep   = true; }
                        if (iSeq == ESeq.OFP_CoverDryerEnd)                                                             { bCoverDryerEnd    = true; }
                    }
                    else if (CDataOption.UseOffP_IOIV)  // 2022.06.30 lhs 
                    {
                        if (iSeq == ESeq.OFP_IV_DryTable && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitPlace)    { bIVDryTable       = true; }  // 2022.07.07 lhs
                        if (iSeq == ESeq.OFP_IV_DryClamp && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitIV2)      { bIVDryClamp       = true; }
                    }


                    if (iSeq == ESeq.OFP_WorkEnd)                                   
                    {
                        return true;          
                    }

                    //------------------------------------
                    //koo 190522 if -> else if로 변경
                    if (bPickTbL)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PickTbL;
                        SEQ = ESeq.OFP_PickTbL;
                        _SetLog(">>>>> Pick left table start.");
                    }
                    else if (bPickTbR)  // Imporve
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PickTbR;
                        SEQ = ESeq.OFP_PickTbR;
                        _SetLog(">>>>> Pick right table start.");
                    }
                    ///////////////////////////////////////////////////////////
                    else if (bWaitPickTbR)
                    {
                        if (CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_PickR) == false)
                        {
                            m_iStep = 10;
                            m_iPreStep = 0;
                            SEQ = ESeq.OFF_WaitPickTbR;
                            _SetLog(">>>>> Wait right table start.");
                        }
                    }
                    else if (bPlaceTbR)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PlaceTbR;
                        SEQ = ESeq.OFP_PlaceTbR;
                        _SetLog(">>>>> Place right table start.");
                    }
                    else if (bWaitPickTbL)
                    {
                        if (CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_PickL) == false)
                        {
                            m_iStep = 10;
                            m_iPreStep = 0;
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PickTbL;
                            SEQ = ESeq.OFF_WaitPickTbL;
                            _SetLog(">>>>> Wait left table start.");
                        }
                    }
                    ///////////////////////////////////////////////////////////
                    else if (bCleanBtm)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_BtmClean;
                        _SetLog(">>>>> Bottom clean start.");
                    }
                    else if (bCleanStrip)
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ = ESeq.OFP_BtmCleanStrip;
                        _SetLog(">>>>> Strip clean start.");
                    }
                    else if (bCleanBrush)
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ = ESeq.OFP_BtmCleanBrush;
                        _SetLog(">>>>> Strip Brush clean start.");
                    }
                    else if (bPlace)
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ = ESeq.OFP_Place;
                        _SetLog(">>>>> Place dry start.");
                    }
                    else if (bIVDryTable)   // 2022.07.07 lhs
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ         = ESeq.OFP_IV_DryTable;
                        _SetLog(">>>>> IV Check Dryer Table start.");
                    }
                    // 2021.04.11 lhs Start
                    else if (bIVDryClamp)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_IV_DryClamp;
                        _SetLog(">>>>> IV Check Dryer Clamp start.");
                    }
                    else if (bCoverDryer)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverDryer; 
                        _SetLog(">>>>> CoverDryer start.");
                    }
                    else if (bCoverDryerKeep)
                    {
                        //m_iStep = 10;
                        //m_iPreStep = 0;
                        //SEQ = ESeq.OFP_CoverDryerKeep;
                        //_SetLog(">>>>> CoverDryer Keep start.");
                    }
                    else if (bCoverDryerEnd)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverDryerEnd;
                        _SetLog(">>>>> CoverDryerEnd start.");
                    }
                    // 2021.04.11 lhs End

                    //210915 syc : 2004U
                    else if (bIV2)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_IV2;
                        _SetLog(">>>>> Unit IV2 Check start.");
                    }
                    else if (bCoverIV2)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverIV2;
                        _SetLog(">>>>> Cover IV2 start.");
                    }
                    else if (bCoverPick)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverPick;
                        _SetLog(">>>>> Cover Pick start.");
                    }

                    //koo 190522 추가.
                    else
                    {
#if true //20200417 jhc : R-Table Place 대기 중인 경우 Wait Pos.로 가면 안 됨
                        if ((CDataOption.PickerImprove != ePickerTimeImprove.Use) ||
                            (CData.Dev.bDual != eDual.Dual) ||
                            //(CSQ_Main.It.m_iStat != EStatus.Auto_Running) || //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            (iSeq != ESeq.OFP_PlaceTbR))
                        {
                            if (CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait) == false)
                            {
                                m_iStep = 10;
                                m_iPreStep = 0;
                                SEQ = ESeq.OFP_Wait;
                                _SetLog(">>>>> Wait start.");
                            }
                        }
#else
                        if (CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait)==false)
                        {
                        
                        //_SetLog("Cyl_Wait Start");
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_Wait;
                        m_LogVal.sMsg = "OFL_Wait Start";
                        SaveLog();
                        }
#endif
                    }
                } //end : if (CDataOption.PickerImprove == ePickerTimeImprove.Use)

                else  //Picker Idle Time 개선 기능 사용하지 않는 경우 (기존 방식)
                {
                    if (iSeq == ESeq.OFP_Wait || iSeq == ESeq.Idle || iSeq == ESeq.OFP_PickTbR || iSeq == ESeq.OFP_PickTbL)
                    {
                        if (CData.Dev.bDual == eDual.Dual)
                        {//듀얼 모드 일 경우
                            //if ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick) && (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR))
                            if ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick) && (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= CData.SPos.dONP_X_PlaceL))
                            {
                                //if(CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                { bPickTbR = true; } // Improve(X), Dual, MultiLot(X)
                            }
                            if ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd && iSeq == ESeq.OFP_Wait))
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                return true;
                            }
                        }
                        else  //노멀 모드 일 경우
                        {
                            // 2021-06-10, jhLee, Multi-LOT 사용일때는 Off-Picker가 LOT 종류를 잘못 선택하여 혼류를 유발할 수 있다. 
                            //                    투입시 Magazine에 일련번호를 확인하여 LOT가 섞이는 것을 방지하도록 한다.
                            if (CData.IsMultiLOT())
                            {
                                if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) && (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) && (iSeq == ESeq.OFP_Wait))
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                    return true;
                                }

                                if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN < CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                        {
                                            bPickTbL = true;
                                        }
                                    }

                                    //190716 ksg : 매거진 추가 시 Left에 더 공급이 없을 경우 
                                    if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        {
                                            bPickTbR = true; // Improve(X), Normal, MultiLot
                                        }
                                    }
                                }

                                if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN == 0) || (CData.Parts[(int)EPart.GRDL].nLoadMgzSN > CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                {
                                    if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                        {
                                            bPickTbR = true; // Improve(X), Normal, MultiLot
                                        }
                                    }
                                    else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick) //200311 ksg :
                                    {
#if true //200723 jhc : On/Off Picker 충돌 방지
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
#else
                                    if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL &&
                                        (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
#endif
                                        {
                                            bPickTbL = true;
                                        }
                                    }
                                }

                                if (CData.Parts[(int)EPart.GRDL].nLoadMgzSN == CData.Parts[(int)EPart.GRDR].nLoadMgzSN)
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                                CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                                CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            {
                                                bPickTbL = true;
                                            }
                                        }
                                    }

                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            {
                                                bPickTbR = true; // Improve(X), Normal, MultiLot
                                            }
                                        }
                                    }

                                    if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) || CData.Parts[(int)EPart.GRDL].iSlot_No > CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            {
                                                bPickTbR = true; // Improve(X), Normal, MultiLot
                                            }
                                        }
                                        //191108 ksg :
                                        /*
                                        if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbL = true; }
                                        }
                                        */
                                        else if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL &&
                                                (!(CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbR) && (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready)) &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            {
                                                bPickTbL = true;
                                            }
                                            
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                                CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                                CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready    &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            {
                                                bPickTbL = true;
                                            }
                                        }
                                    }
                                }

                                if (CData.Opt.aTblSkip[(int)EWay.L] || CData.Opt.aTblSkip[(int)EWay.R])
                                {
                                    if (CData.Opt.aTblSkip[(int)EWay.L] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            {
                                                bPickTbR = true; // Improve(X), Normal, MultiLot
                                            }
                                        }
                                        else if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                    if (CData.Opt.aTblSkip[(int)EWay.R] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                                CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                                CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            {
                                                bPickTbL = true;
                                            }
                                        }
                                        else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                }


                                // 최종적으로... 
                                // 양 쪽 모두 Pickup 할 수 있고 서로 다른 LOT 이면 먼저 들어온 쪽 Strip을 집도록 한다.
                                //
                                if (bPickTbL && bPickTbR && (CData.Parts[(int)EPart.GRDL].sLotName != CData.Parts[(int)EPart.GRDR].sLotName))
                                {
                                    // 오른쪽 Table이 더 늦게 들어온 Strip이라면
                                    if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN < CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                    {
                                        bPickTbL = true;
                                        bPickTbR = false; // Improve(X), Normal, MultiLot

                                        _SetLog($"Set PickTalble-Left Left_MgzSN:{CData.Parts[(int)EPart.GRDL].nLoadMgzSN}, Right_MgzSN:{CData.Parts[(int)EPart.GRDR].nLoadMgzSN},");
                                    }
                                    else if ((CData.Parts[(int)EPart.GRDL].nLoadMgzSN > CData.Parts[(int)EPart.GRDR].nLoadMgzSN))
                                    {
                                        bPickTbL = false;
                                        bPickTbR = true; // Improve(X), Normal, MultiLot

                                        _SetLog($"Set PickTalble-Right Left_MgzSN:{CData.Parts[(int)EPart.GRDL].nLoadMgzSN}, Right_MgzSN:{CData.Parts[(int)EPart.GRDR].nLoadMgzSN},");
                                    }
                                }
                            }  // End... if (CData.IsMultiLOT())
                            else  // of Multi-LOT
                            {
                                if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) && (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) && (iSeq == ESeq.OFP_Wait))
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                    return true;
                                }
                                if ((CData.Parts[(int)EPart.GRDL].iMGZ_No < CData.Parts[(int)EPart.GRDR].iMGZ_No))
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  &&
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                        { bPickTbL = true; }
                                    }
                                    //190716 ksg : 매거진 추가 시 Left에 더 공급이 없을 경우 
                                    if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL   && 
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL  && 
                                            CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR  && 
                                            CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        { bPickTbR = true; } // Improve(X), Normal, MultiLot(X)
                                    }
                                }

                                if ((CData.Parts[(int)EPart.GRDL].iMGZ_No == 0) || (CData.Parts[(int)EPart.GRDL].iMGZ_No > CData.Parts[(int)EPart.GRDR].iMGZ_No))
                                {
                                    if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                    {
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                        { bPickTbR = true; } // Improve(X), Normal, MultiLot(X)
                                    }
                                    else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick) //200311 ksg :
                                    {
#if true //200723 jhc : On/Off Picker 충돌 방지
                                        if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                            (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
#else
	                                    if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL &&
	                                        (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
#endif
                                        { bPickTbL = true; }
                                    }
                                }

                                if (CData.Parts[(int)EPart.GRDL].iMGZ_No == CData.Parts[(int)EPart.GRDR].iMGZ_No)
                                {
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                    }
                                    if (CData.Parts[(int)EPart.GRDL].iSlot_No < CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat != ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve(X), Normal, MultiLot(X)
                                        }
                                    }
                                    if ((CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) || CData.Parts[(int)EPart.GRDL].iSlot_No > CData.Parts[(int)EPart.GRDR].iSlot_No)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve(X), Normal, MultiLot(X)
                                        }
                                        //191108 ksg :
                                        /*
                                        if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbL = true; }
                                        }
                                        */
                                        else if (!bPickTbR && CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && (!(CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PlaceTbR) && (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready)) &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR && CData.Parts[(int)EPart.GRDR].iStat != ESeq.GRR_Ready &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                    }
                                }

                                if (CData.Opt.aTblSkip[(int)EWay.L] || CData.Opt.aTblSkip[(int)EWay.R])
                                {
                                    if (CData.Opt.aTblSkip[(int)EWay.L] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WaitPick && CData.Parts[(int)EPart.GRDR].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR)
                                            { bPickTbR = true; } // Improve(X), Normal, MultiLot(X)
                                        }
                                        else if (CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                    if (CData.Opt.aTblSkip[(int)EWay.R] == true)
                                    {
                                        if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WaitPick && CData.Parts[(int)EPart.GRDL].bExistStrip == true)
                                        {
                                            if (CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PickTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbL && CData.Parts[(int)EPart.ONP].iStat != ESeq.ONP_PlaceTbR &&
                                                (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) <= (CData.SPos.dONP_X_PlaceL - 300/*HERE : 실제 On-Picker 안전위치 범위?*/))) //20200420 jhc : 피커 충돌 방지
                                            { bPickTbL = true; }
                                        }
                                        else if (CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd) //190129 ksg : Lot end 추가
                                        {
                                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                            return true;
                                        }
                                    }
                                } // Table Skip
                            } // End...else of Multi-LOT
                        } // End...Normal mode
                    }

                    if (iSeq == ESeq.OFP_BtmClean)                                  { bCleanBtm     = true; }
                    if (iSeq == ESeq.OFP_BtmCleanStrip)                             { bCleanStrip   = true; }
                    if (iSeq == ESeq.OFP_BtmCleanBrush)                             { bCleanBrush   = true; }

                    if ((iSeq == ESeq.OFP_Place || iSeq == ESeq.OFP_WaitPlace) && 
                        CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitPlace)    { bPlace        = true; }

                    if (CDataOption.UseDryWtNozzle)     // 2021.04.15 lhs
                    {
                        if (iSeq == ESeq.OFP_IV_DryClamp && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitIV2)      { bIVDryClamp       = true; }
                        if (iSeq == ESeq.OFP_CoverDryer  && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaterNozzle)  { bCoverDryer       = true; }
                        if (iSeq == ESeq.OFP_CoverDryerKeep)                                                            { bCoverDryerKeep   = true; }
                        if (iSeq == ESeq.OFP_CoverDryerEnd)                                                             { bCoverDryerEnd    = true; }
                    }
                    else if (CDataOption.UseOffP_IOIV)  // 2022.06.30 lhs   
                    {
                        if (iSeq == ESeq.OFP_IV_DryTable && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitPlace)    { bIVDryTable       = true; }  // 2022.07.07 lhs
                        if (iSeq == ESeq.OFP_IV_DryClamp && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitIV2)      { bIVDryClamp       = true; }
                    }


                    //210927 syc : 2004U
                    if (CDataOption.Use2004U)
                    {
                        // ----------------------------------- 2004U Unit IV2 검사 시퀀스 조건
                        if (iSeq == ESeq.OFP_IV2        && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitIV2)           { bIV2      = true; } //2004U

                        // ----------------------------------- 2004U Cover IV2 검사 시퀀스 조건
                        if (CData.bOFP_CoverPick == true)
                        {
                            iSeq = ESeq.OFP_CoverIV2;
                        }
                        if ((iSeq == ESeq.OFP_CoverIV2  && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitCoverPick))    { bCoverIV2 = true;     }

                        // ----------------------------------- 2004U Cover Pick 시퀀스 조건
                        if (iSeq == ESeq.OFP_CoverPick  && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitCoverPick)     { bCoverPick = true;    }

                        // ----------------------------------- 2004U Cover Place 시퀀스 조건
                        if (iSeq == ESeq.OFP_CoverPlace && CData.Parts[(int)EPart.DRY].iStat == ESeq.DRY_WaitCoverPlace)    { bCoverPlace = true;   }

                    }
                    // syc end

                    if (iSeq == ESeq.OFP_WorkEnd)                                   
                    {
						return true;
					}

					//koo 190522 if -> else if로 변경
					if (bPickTbL)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PickTbL;
                        SEQ = ESeq.OFP_PickTbL;
                        _SetLog(">>>>> Pick left table start.");
                    }

                    else if (bPickTbR) // Improve(X)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PickTbR;
                        SEQ = ESeq.OFP_PickTbR;
                        _SetLog(">>>>> Pick right table start.");
                    }
                    else if (bCleanBtm)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_BtmClean;
                        _SetLog(">>>>> Bottom clean start.");
                    }
                    else if (bCleanStrip)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_BtmCleanStrip;
                        _SetLog(">>>>> Strip clean start.");
                    }
                    else if (bCleanBrush)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_BtmCleanBrush;
                        _SetLog(">>>>> Strip Brush clean start.");
                    }
                    
                    else if (bPlace)
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ         = ESeq.OFP_Place;
                        _SetLog(">>>>> Place dry start.");
                    }
                    
                    else if (bIVDryTable)   // 2022.07.07 lhs
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ         = ESeq.OFP_IV_DryTable;
                        _SetLog(">>>>> IV Check Dryer Table start.");
                    }
                    // 2021.04.11 lhs Start
                    else if (bIVDryClamp)     // improve.use 아닐 경우
                    {
                        m_iStep     = 10;
                        m_iPreStep  = 0;
                        SEQ         = ESeq.OFP_IV_DryClamp;
                        _SetLog(">>>>> IV Check Dryer Clamp start.");
                    }
                    
                    else if (bCoverDryer)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverDryer;
                        _SetLog(">>>>> CoverDryer start.");
                    }
                    else if (bCoverDryerKeep)
                    {
                        //m_iStep = 10;
                        //m_iPreStep = 0;
                        //SEQ = ESeq.OFP_CoverDryerKeep;
                        //_SetLog(">>>>> CoverDryer Keep start.");
                    }
                    else if (bCoverDryerEnd)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverDryerEnd;
                        _SetLog(">>>>> CoverDryerEnd start.");
                    }
                    //210915 syc : 2004U
                    else if (bIV2)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_IV2;
                        _SetLog(">>>>> Unit IV2 Check start.");
                    }
                    else if (bCoverIV2)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverIV2;
                        _SetLog(">>>>> Cover IV2 start.");
                    }
                    else if (bCoverPick)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverPick;
                        _SetLog(">>>>> Cover Pick start.");
                    }
                    else if (bCoverPlace)
                    {
                        m_iStep = 10;
                        m_iPreStep = 0;
                        SEQ = ESeq.OFP_CoverPlace;
                        _SetLog(">>>>> Cover Place start.");
                    }
                    else
                    {
                        if (CMot.It.Get_Mv((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait) == false)
                        {
                            m_iStep = 10;
                            m_iPreStep = 0;
                            SEQ = ESeq.OFP_Wait;
                            _SetLog(">>>>> Wait start.");
                        }
                    }

                } // end : else (<-- if (CDataOption.PickerImprove == ePickerTimeImprove.Use)의 else)
                ///////////////////////////////////////////////////////////////
            }// end : if (SEQ == (int)ESeq.Idle)

            switch (SEQ)
            {
                default:
                    return false;

                case ESeq.OFP_PickTbL:
                    if (Cyl_Pick(EWay.L))
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Pick left table finish.");
                    }
                    return false;

                case ESeq.OFP_PickTbR:
                    if (Cyl_Pick(EWay.R))
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Pick right table finish.");
                    }
                    return false;

#if true //20200415 jhc : Picker Idle Time 개선
                case ESeq.OFP_PlaceTbR:
                    if (Cyl_PlaceTbR())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Place right table finish.");
                    }
                    return false;

                case ESeq.OFF_WaitPickTbR:
                    if (Cyl_WaitPickTbR())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Wait right table finish.");
                    }
                    return false;

                case ESeq.OFF_WaitPickTbL:
                    if (Cyl_WaitPickTbL())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Wait left table finish.");
                    }
                    return false;
#endif

                case ESeq.OFP_BtmClean:
                    if (CDataOption.UseSprayBtmCleaner) // 2022.03.10 lhs 
                    {
                        if (Cyl_PckClean_SprayNzl())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Bottom clean finish (Spray Nozzle).");
                        }
                    }
                    else
                    {

                        //20190926 ghk_ofppadclean
                        if (CDataOption.PadClean == eOffPadCleanType.LRType)
                        {
                            if (Cyl_PckClean())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Bottom clean finish.");
                            }
                        }
                        else
                        {
                            if (Cyl_PckPadClean())
                            {
                                SEQ = ESeq.Idle;
                                _SetLog("<<<<< Picker pad clean finish.");
                            }
                        }
                    }
                    return false;

                case ESeq.OFP_BtmCleanStrip:
                    if (CDataOption.UseSprayBtmCleaner) // 2022.03.07 lhs 
                    {
                        if (Cyl_CleanStrip_SprayNzl())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Strip clean finish. (Spray Nozzle) ");
                        }
                    }
                    else
                    {
                        if (Cyl_CleanStrip())
                        {
                            SEQ = ESeq.Idle;
                            _SetLog("<<<<< Strip clean finish.");
                        }
                    }
                    return false;

                case ESeq.OFP_BtmCleanBrush:
                    if (Cyl_CleanBrush())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Strip Brush clean finish.");
                    }
                    return false;

                case ESeq.OFP_Place:
					if (Cyl_Place())
					{
						SEQ = ESeq.Idle;
						_SetLog("<<<<< Place dry finish.");
					}
					return false;

                case ESeq.OFP_IV_DryTable:  // 2022.07.07 lhs
                    if (Cyl_IV_DryTable())   // IV2로 Dryer Table에 Strip이 있는지 촬영 #3 (#1 위치에서)
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Off-Pikcer IV... Dryer Strip(table) Check... finish.");
                    }
                    return false;

                // 2021.04.12 lhs Start
                case ESeq.OFP_IV_DryClamp:
                    if (Cyl_IV_DryClamp())   // IV2로 Dryer Tip을 #1, #2 촬영.
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Off-Pikcer IV... Dryer Clamp Check... finish.");
                    }
                    return false;

                case ESeq.OFP_CoverDryer:
                    if (Cyl_CoverDryer())   // IV2로 Dryer Tip을 #1, #2 촬영.
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< OffLoader Picker Cover Dryer finish.");
                    }
                    return false;

                case ESeq.OFP_CoverDryerEnd:
                    if (Cyl_CoverDryerEnd())   // 커버역할 끝나면 Wait/BtmClean/WorkEnd 수행.
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< OffLoader Picker Cover Dryer End finish.");
                    }
                    return false;
                // 2021.04.12 lhs End

                case ESeq.OFP_Wait:
                    if (Cyl_Wait())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<< Wait finish.");
                    }
                    return false;

                //210908 syc : 2004U
                case ESeq.OFP_IV2:
                    if (Cyl_OFP_IV2())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<<  finish.");
                    }
                    return false;

                case ESeq.OFP_CoverIV2:
                    if (Cyl_OFP_CoverCheck())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<<  finish.");
                    }
                    return false;

                case ESeq.OFP_CoverPick:
                    if (Cyl_CoverPick())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<<  finish.");
                    }
                    return false;

                case ESeq.OFP_CoverPlace:
                    if (Cyl_CoverPlace())
                    {
                        SEQ = ESeq.Idle;
                        _SetLog("<<<<<  finish.");
                    }
                    return false;
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

            CLog.Save_Log(eLog.OFP, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
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

            CLog.Save_Log(eLog.OFP, eLog.None, string.Format("{0}\t[{1}()]\tSt : {2}\t{3}", sStat, sMth, m_iStep.ToString("00"), sMsg));
        }

        public void Init_Cyl()
        {
            m_iStep = 10;
            m_iPreStep = 0;
        }

        public bool Cyl_Servo(bool bVal)
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
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
                        _SetLog("All servo-on : " + bVal);

                        m_iStep++;
                        return false;
                    }

                case 11:    // 2. 서보 온 상태 체크
                    {
                        int iVal = Convert.ToInt32(bVal);
                        if (CMot.It.Chk_Srv(m_iX) == bVal &&
                            CMot.It.Chk_Srv(m_iZ) == bVal)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Wait()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_WAIT_TIMEOUT);
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
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }

                        CIO.It.Set_Y((int)eY.OFFP_Ejector, false);

                        // 200327 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        // syc : 210526 SPH 증가 목적 Qorvo BJ, Qorvo DJ 추가
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.SPIL || CData.CurCompany == ECompany.JCET || CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) // 2021.01.28 lhs : JCET 추가
                        {
                            Func_Picker0();
                            _SetLog("Picker 0");
                        }
                        else
                        {
                            Func_Picker90();
                            _SetLog("Picker 90");
                        }

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        // 200327 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.SPIL || CData.CurCompany == ECompany.JCET || CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) // 2021.01.28 lhs : JCET 추가
                        { if (!Func_Picker0()) return false; }
                        else
                        { if (!Func_Picker90()) return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if(!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Wait))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait); //200414 ksg : Wait 중 이동 못하여 Time Out 발생으로 추가 함.
                            return false;
                        }
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

#if true //20200415 jhc : Picker Idle Time 개선
        public bool Cyl_WaitPickTbR()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_WAIT_TIMEOUT);
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
                    {//Z축 Wait Pos. 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {//Z축 Wait Pos. 이동 확인, Ejector OFF, 피커 0도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }

                        Func_Eject(false);
                        Func_Picker0();
                        _SetLog("Eject off.  Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//피커 0도 확인, X축 R-Table Pick 위치로 이동
                        if (!Func_Picker0()) return false;

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickR);
                        _SetLog("X axis move pick-r.", CData.SPos.dOFP_X_PickR);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//X축 R-Table Pick 위치로 이동 확인
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_PickR))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickR);
                            return false;
                        }

                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_WaitPickTbL()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_WAIT_TIMEOUT);
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
                    {//Z축 Wait Pos. 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {//Z축 Wait Pos. 이동 확인, Ejector OFF, 피커 0도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) 
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }

                        Func_Eject(false);
                        Func_Picker0();
                        _SetLog("Eject off.  Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//피커 0도 확인, X축 L-Table Pick 위치로 이동
                        if (!Func_Picker0()) return false;

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickL);
                        _SetLog("X axis move pick-l.", CData.SPos.dOFP_X_PickL);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//X축 L-Table Pick 위치로 이동 확인
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_PickL))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickL);
                            return false;
                        }

                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
			}
		}
#endif

		public bool Cyl_Home()
        {
            if (m_iStep != m_iPreStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    //CErr.Show(EErr.) //OnPicker Home Time Out Err
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

                case 10:    // 드레인 오프, 바텀 클리너 워터 오프
                    {
                        m_bHD = false;
                        // 2021.01.26 lhs Start
                        if (Chk_Axes(false))
                        {
                            m_iStep = 0;
                            return true;
                        }
                        //2021.01.26 lhs End

                        Func_Drain(false);
                        CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
                        _SetLog("Drain off.  DRY Bottom clean water off.");

                        m_iStep++;
                        return false;
                    }

                case 11:    // Z축 홈
                    {
                        CMot.It.Mv_H(m_iZ);
                        _SetLog("Z axis homing.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!CMot.It.Get_HD(m_iZ)) 
                        { return false; }
                        
                        CMot.It.Mv_H(m_iX);
                        _SetLog("X axis homing.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {
                        if (!CMot.It.Get_HD(m_iX)) 
                        { return false; }
                        //200330 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR)
                        {
                            Func_Picker0();
                            _SetLog("Picker 0.");
                        }
                        else
                        {
                            Func_Picker90();
                            _SetLog("Picker 90.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {
                        //200330 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR)
                        { if (!Func_Picker0()) return false; }
                        else
                        { if (!Func_Picker90()) return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) 
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Wait)) 
                        { return false; }
                        m_bHD = true;
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_PckClean()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            //20191121 ghk_display_strip
            if (Chk_Strip())
            {
                CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
                if (CDataOption.Use2004U)
                {    
                    _SetLog("OFFP_Carrier_Clamp_Close : "   + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));
                    _SetLog("OFFP_Carrier_Clamp_Open : "    + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));
                    _SetLog("OFFP_Picker_Carrier_Detect : " + CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect));
                }

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

                case 10:    //버큠 오프, 이젝트 온, 드레인 워터 오프, 드라이 바텀 워터 온, Z축 대기 위치 이동
                    {
                        //210928 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            m_tStTime = DateTime.Now;
                        }
                        else
                        {
                            m_tStTime = DateTime.Now;
                            Func_Vacuum(false);
                            Func_Eject(true);
                            Func_Drain(false);
                        }

                        if (CData.Dev.iOffpClean <= 0)
                        {//오프로더 클린 사용자 설정 값이 0보다 작을 경우
                            //CMot.It.Mv_N(m_iZ, CData.SPos.dOFL_Z_Wait);   // 2021.04.01 lhs : 삭제
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);     // 2021.04.01 lhs : 오류 수정
                            Func_Picker0();
                            _SetLog("Picker 0.  Clean count 0.");

                            m_iStep = 20;
                        }
                        else
                        {
                            CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, true);//Y6F Btm clean Water
                            m_binv = false;
                            m_CleanCnt = 0;
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                        }
                        return false;
                    }

                case 11:    //Z축 대기 위치 이동 확인, 피커 0도, X축 클린 스타트 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        Func_Picker0();
                        if (!m_binv)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd);
                            _SetLog("X axis move end.", CData.Dev.dOffP_X_ClnEnd);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);
                            _SetLog("X axis move start.", CData.Dev.dOffP_X_ClnStart);
                        }

                        m_iStep++;
                        return false;
                    }

                case 12:    //피커 0도 확인, X축 클린 스타트 위치 이동 확인, Z축 클린 스타트 위치 이동
                    {
                        if (!Func_Picker0())
                        { return false; }

                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd))
                        { return false; }
                        if (m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart))
                        { return false; }

                        // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                        //if (CData.CurCompany == ECompany.ASE_KR)
                        if (CData.CurCompany == ECompany.ASE_KR ||
                            CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                            CData.CurCompany == ECompany.JSCK   ||
                            CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                        {
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_ClnStart);
                            _SetLog("Z axis move clean.", CData.Dev.dOffP_Z_ClnStart);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_ClnStart);
                            _SetLog("Z axis move clean.", CData.SPos.dOFP_Z_ClnStart);
                        }
                        // 2021.02.27 SungTae End

                        m_iStep++;
                        return false;
                    }

                case 13:    //== 반복 시작 지점 == Z축 클린 스타트 위치 이동 확인, X축 클린 동작
                    {
                        // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                        //if (CData.CurCompany == ECompany.ASE_KR)
                        if (CData.CurCompany == ECompany.ASE_KR ||
                            CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                            CData.CurCompany == ECompany.JSCK   ||
                            CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_ClnStart)) { return false; }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_ClnStart)) { return false; }
                        }
                        // 2021.02.27 SungTae End

                        if (!m_binv)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd);
                            _SetLog("X axis move end.", CData.Dev.dOffP_X_ClnEnd);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);
                            _SetLog("X axis move start.", CData.Dev.dOffP_X_ClnStart);

                        }

                        m_iStep++;
                        return false;
                    }

                case 14:    //X축 클린 동작 확인, 딜레이 3초 세팅
                    {
                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd)) return false;
                        if (m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart)) return false;
                        m_Delay.Set_Delay(3000);
                        _SetLog("Set delay : 3000ms");

                        m_iStep++;
                        return false;
                    }

                case 15:    //드라이 바텀 워터 온 확인, X축 클린위치 이동(슬로우)
                    {
                        if (!CIO.It.Get_X(eX.DRY_ClnBtmFlow))
                        {
                            if (m_Delay.Chk_Delay())
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                _SetLog("Error : DRY Bottom clean water off.");

                                CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water

                                //210928 syc : 2004U
                                if (CDataOption.Use2004U)
                                {

                                }
                                else
                                {                   
                                    Func_Vacuum(false);
                                    Func_Eject(false);
                                    Func_Drain(false);
                                }
                                
                                CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);

                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!CIO.It.Get_X(eX.DRY_ClnBtmFlow))
                        { return false; }

                        if (!m_binv)
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dOffP_X_ClnStart);
                            _SetLog("X axis move start(slow).", CData.Dev.dOffP_X_ClnStart);
                        }
                        else
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dOffP_X_ClnEnd);
                            _SetLog("X axis move end(slow).", CData.Dev.dOffP_X_ClnEnd);
                        }

                        m_iStep++;
                        return false;
                    }

                case 16:    //X축 클린위치 이동(슬로우) 확인, 클링 카운트 증가, 클린 방향 변경, 클린 횟수 검사, 완료 후 드라이 바텀워터 종료, Z축 대기위치 이동
                    if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart)) return false;
                    if ( m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd  )) return false;
                    
                    m_CleanCnt++;

                    // 2021.07.19 lhs Start : PM Count OffP Sponge Current Count 증가
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)
                    {
                        CData.tPM.nOffP_Sponge_Check_CurrCnt++;
                        CData.tPM.nOffP_Sponge_Change_CurrCnt++;
                        _SetLog("OffPicker Sponge Current Count : Check Count = "  + CData.tPM.nOffP_Sponge_Check_CurrCnt.ToString() +
                                                               ", Change Count = " + CData.tPM.nOffP_Sponge_Change_CurrCnt.ToString());
                    }
                    // 2021.07.19 lhs End

                    m_binv = !m_binv;
                    if (m_CleanCnt <= CData.Dev.iOffpClean)
                    {
                        _SetLog("Clean count : " + m_CleanCnt);

                        m_iStep = 13;
                        return false;
                    }

                    CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
                    CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                    _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                    // 2021.07.21 lhs Start : PM Count OffP Sponge Current Count, 루프 종료시 저장, SCK VOC
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)
                    {
                        CMnt.It.Save();
                        _SetLog("Maintenance.cfg Save !!!");

                    }
                    // 2021.07.21 lhs End

                    m_iStep = 20;
                    return false;

                case 20:    //Z축 대기위치 이동 확인, X축 클린 스타트 위치 이동, 드라이 바텀 워터 오프, 버큠 오프, 이젝트 오프, 드레인 오프
                    if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) return false;
                    CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);

                    CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
                    CData.dtSpgWOffLastTime = DateTime.Now;

                    //210928 syc : 2004U
                    if (CDataOption.Use2004U)
                    {

                    }
                    else
                    {
                        Func_Vacuum(false);
                        Func_Eject(false);
                        Func_Drain(false);
                    }

                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                    if (CDataOption.Use2004U)
                    {
                        if (CData.Parts[(int)EPart.ONL].iStat   == ESeq.ONL_WorkEnd &&
                            CData.Parts[(int)EPart.INR].iStat   == ESeq.INR_WorkEnd &&
                            CData.Parts[(int)EPart.ONP].iStat   == ESeq.ONP_WorkEnd &&
                            CData.Parts[(int)EPart.GRDL].iStat  == ESeq.GRL_WorkEnd &&
                            CData.Parts[(int)EPart.GRDR].iStat  == ESeq.GRR_WorkEnd)
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverPlace; // WorkEnd가 아닌 Cover Place 하고 WorkEnd 해야함
                        }
                    }
                    else
                    {
#if true //20200417 jhc : Off-Picker Bottom Clean 후 종료 체크 안 함 <== R-Table Place 후 Off-Picker Bottom Clean / Dry Place 후 Wait (Auto-Running && Step Mode에서만)
                        if ((CDataOption.PickerImprove != ePickerTimeImprove.Use) || (CData.Dev.bDual != eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                                                                                                                      //if ((CDataOption.PickerImprove != ePickerTimeImprove.Use) || (CSQ_Main.It.m_iStat != EStatus.Auto_Running) || (CData.Dev.bDual != eDual.Dual))
                        {
                            if (CData.Parts[(int)EPart.ONL].iStat   == ESeq.ONL_WorkEnd &&
                                CData.Parts[(int)EPart.INR].iStat   == ESeq.INR_WorkEnd &&
                                CData.Parts[(int)EPart.ONP].iStat   == ESeq.ONP_WorkEnd &&
                                CData.Parts[(int)EPart.GRDL].iStat  == ESeq.GRL_WorkEnd &&
                                CData.Parts[(int)EPart.GRDR].iStat  == ESeq.GRR_WorkEnd)
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                            }
                        }
#else
                    if(CData.Parts[(int)EPart.ONL ].iStat == ESeq.ONL_WorkEnd &&
                       CData.Parts[(int)EPart.INR ].iStat == ESeq.INR_WorkEnd &&
                       CData.Parts[(int)EPart.ONP ].iStat == ESeq.ONP_WorkEnd &&
                       CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                       CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd   ) CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
#endif
                    }


                    _SetLog("Status change : " + CData.Parts[(int)EPart.OFP].iStat);

                    m_tTackTime = DateTime.Now - m_tStTime;
                    _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));                    

                    m_iStep = 0;
                    return false;
            }
        }

        // 2022.03.08 lhs : Spray Nozzle형 Btm Cleaner (Pad Btm)
        /// <summary>
        /// Sponge형 -> Spray Nozzle형 Btm Cleaner (Pad Btm)
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PckClean_SprayNzl()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (Chk_Strip())
            {
                CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
                if (CDataOption.Use2004U)
                {
                    _SetLog("OFFP_Carrier_Clamp_Close : "   + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));
                    _SetLog("OFFP_Carrier_Clamp_Open : "    + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));
                    _SetLog("OFFP_Picker_Carrier_Detect : " + CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect));
                }
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

                case 10:    // 버큠 확인, 사용자 설정 크리닝 카운터 확인, 버큠 온, 이젝트 오프, 드레인 오프, 바텀 워터 온, X축 위치 확인, Z축 대기위치 이동
                    {
                        if (CData.Dev.iOffpClean <= 0)
                        {
                            _SetLog("Clean Btm skip.  Count : " + CData.Dev.iOffpClean);

                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);     // 2021.04.01 lhs : 오류 수정
                            Func_Picker0();     _SetLog("Picker 0.  Clean count 0.");

                            m_iStep = 30;   // 종료 스텝
                            return false;
                        }

                        m_tStTime = DateTime.Now;

                        if (CDataOption.Use2004U)
                        {
                        }
                        else
                        {
                            Func_Vacuum(false);
                            Func_Eject(true);
                            Func_Drain(false);
                        }

                        m_CleanCnt = 0;
                        m_dPos_X = CData.Dev.dOffP_X_ClnStart;   // X축 시작위치 : Nozzle 중앙에 위치하도록 설정

                        m_bReady = (CMot.It.Get_FP(m_iX) == m_dPos_X) && !Func_Picker0();  // ???
                        if (!m_bReady)
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                        }
                        _SetLog("Ready.  Count : " + CData.Dev.iOffpClean);

                        m_iStep++;
                        return false;
                    }

                case 11:    // Z축 대기위치 이동 확인, 피커 0도, X축 크리닝 위치 이동 (중앙 고정)
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) // Z축 대기위치 이동 확인
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }

                        Func_Picker0();                 // OffP 0도

                        CMot.It.Mv_N(m_iX, m_dPos_X);   // X축 이동
                        _SetLog("X axis move clean start.(Center fixed)", m_dPos_X);

                        m_iStep++;
                        return false;
                    }

                case 12:    // 피커 0도 확인, X축 크리닝 시작위치 이동 확인, Z축 크리닝 위치 이동
                    {
                        if (!Func_Picker0())
                        {
                            return false;
                        }
                        if (!CMot.It.Get_Mv(m_iX, m_dPos_X))
                        {
                            CMot.It.Mv_N(m_iX, m_dPos_X);   // X축 이동
                            return false;
                        }

                        // Z축 크리닝 위치 이동
                        m_dPos_Z = CData.Dev.dOffP_Z_ClnStart;
                        CMot.It.Mv_N(m_iZ, m_dPos_Z);
                        _SetLog("Z axis move strip clean position.", m_dPos_Z);

                        m_iStep++;
                        return false;
                    }

                case 13:    // Z축 크리닝 위치 확인, Btm Clean Water On
                    {
                        // Z축 크리닝 위치 확인 
                        if (!CMot.It.Get_Mv(m_iZ, m_dPos_Z))
                        {
                            CMot.It.Mv_N(m_iZ, m_dPos_Z);
                            return false;
                        }
                        _SetLog("Z axis move strip clean position -> Checked", m_dPos_Z);

                        // 워터 온
                        Func_BtmClnFlow(true);      _SetLog("Btm Clean Water On");

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }
                case 14:    // Water On 확인, 노즐 백워드
                    {
                        bool bWater = Func_BtmClnFlow(true);
                        if (m_Delay.Chk_Delay())  // 일정시간 후 에러
                        {
                            if (!bWater)
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                _SetLog("Error : DRY Bottom clean water does not turn on.");

                                if (CDataOption.Use2004U)
                                {
                                }
                                else
                                {
                                    Func_Vacuum(false);
                                    Func_Eject(false);
                                    Func_Drain(false);
                                }

                                CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);

                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bWater) return false;

                        // 노즐 백워드
                        Func_SprayNzlBackward();        _SetLog("Spray Nozzle Backward.");
                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);        _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms);   _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 15:    // 노즐 백워드 확인, 노즐 포워드 
                    {
                        bool bBackward = Func_SprayNzlBackward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bBackward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bBackward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if(!m_Wait.Chk_Delay())
                        {
                            return false;
						}

                        // 노즐 포워드
                        Func_SprayNzlForward();         _SetLog("Spray Nozzle Forward.");
                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);        _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms);   _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 16:    // 노즐 포워드 확인, 워터 크리닝 카운트 증가, 워터 크리닝 종료 여부 확인, 워터 크리닝 종료시 바텀 워터 오프
                    {
                        bool bForward = Func_SprayNzlForward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bForward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bForward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms);   _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }

                        m_CleanCnt++;   // 워터 크리닝 카운터 증가

                        // 반복
                        if (m_CleanCnt < CData.Dev.iOffpClean)
                        {
                            _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, CData.Dev.iOffpClean));

                            // 딜레이 설정
                            m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");

                            m_iStep = 14; // 워터 온, 백워드 스텝
                            return false;
                        }

                        // 워터 오프
                        Func_BtmClnFlow(false);

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);        _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }

                case 17:    // 워터 오프 확인, 에어 클린 스킵 조건 확인, 에어 클린 카운터 초기화 
                    {
                        bool bWater = Func_BtmClnFlow(false);
                        if (m_Delay.Chk_Delay())
                        {
                            if (bWater) // 워터가 꺼지지 않으면
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (bWater)
                        {
                            return false;
                        }

                        // Air Blow Skip 조건
                        if (CData.Dev.iOffpClean_Air <= 0) // Skip 조건 추가
                        {
                            _SetLog("Clean strip skip(Air Blow).   Count : " + CData.Dev.iOffpClean_Air);
                            m_iStep = 21;   // Z축 대기위치 이동 -> 종료 스텝
                            return false;
                        }

                        // 에어 클린 카운트 초기화
                        m_CleanCnt = 0;

                        // 에어 온
                        Func_BtmClnAirBlow(true);   _SetLog("Btm Clean AirBlow On");

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }

                case 18:    // AirBlow On 확인, 노즐 백워드
                    {
                        bool bAir = Func_BtmClnAirBlow(true);
                        if (m_Delay.Chk_Delay())  // 일정시간 후 에러
                        {
                            if (!bAir)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                                _SetLog("Error : DRY Bottom clean water does not turn on.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bAir) return false;

                        // 노즐 백워드
                        Func_SprayNzlBackward();        _SetLog("Spray Nozzle Backward.");

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);        _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms);   _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");


                        m_iStep++;
                        return false;
                    }

                case 19:    // 노즐 백워드 확인, 노즐 포워드 
                    {
                        bool bBackward = Func_SprayNzlBackward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bBackward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bBackward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms);   _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }

                        // 노즐 포워드
                        Func_SprayNzlForward();             _SetLog("Spray Nozzle Forward.");
                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);            _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms);       _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 20:    // 노즐 포워드 확인, 에어 클린 카운트 증가, 에러 클린 종료 여부 확인, 워터 크리닝 종료시 바텀 워터 오프
                    {
                        bool bForward = Func_SprayNzlForward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bForward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bForward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms);       _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }


                        m_CleanCnt++;   // 에어 클린 카운터 증가

                        // 반복
                        if (m_CleanCnt < CData.Dev.iOffpClean_Air)  // Water와 Air 설정 카운트 구분 필요
                        {
                            _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, CData.Dev.iOffpClean_Air));

                            // 딜레이 설정
                            m_Delay.Set_Delay(5000);        _SetLog("Set delay : 5000ms");

                            m_iStep = 18; // 에어 온, 백워드 스텝
                            return false;
                        }

                        // 워터 오프
                        Func_BtmClnAirBlow(false);

                        // Z축 대기위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 21:    // Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) // Z축 대기위치 이동 확인
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }
                        _SetLog("Z axis move check.");

                        if (CDataOption.Use2004U)
                        {
                        }
                        else
                        {
                            Func_Vacuum(false);
                            Func_Eject(false);
                            Func_Drain(false);
                        }

                        m_iStep = 30;
                        return false;
                    }

                case 30:    // 종료
                    {
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFP].iStat.ToString());

                        if (CDataOption.Use2004U)
                        {
                            if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverPlace; // WorkEnd가 아닌 Cover Place 하고 WorkEnd 해야함
                            }
                        }
                        else
                        {
                            if ((CDataOption.PickerImprove != ePickerTimeImprove.Use) || (CData.Dev.bDual != eDual.Dual))
                            {
                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                                }
                            }
                        }

                        _SetLog("Status change : " + CData.Parts[(int)EPart.OFP].iStat);

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return false;
                    }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Left"></param>
        /// <returns></returns>
        public bool Cyl_Pick(EWay eWay)
        {

            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT * 4);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    // 2020.11.24 JSKim St
                    if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running && m_iStep == 13)
                    {
                        if (eWay == EWay.L && CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                        {
                            CErr.Show(eErr.LEFT_GRIND_TABLE_NOT_READY);
                            _SetLog("Error : Left table not ready.");
                        }
                        else if (eWay == EWay.R && CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                        {
                            CErr.Show(eErr.RIGHT_GRIND_TABLE_NOT_READY);
                            _SetLog("Error : Right table not ready.");
                        }
                        else
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_PICK_TIMEOUT);
                            _SetLog("Error : Timeout.");
                        }
                    }
                    else
                    {
                    // 2020.11.24 JSKim Ed
                        CErr.Show(eErr.OFFLOADERPICKER_PICK_TIMEOUT);
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
                        return true;
                    }

                case 10:    //버큠 오프, 이젝트 오프, 드레인 오프, 드라이 바텀 워터 오프, Z축 대기 위치 이동
                    {
                        m_tStTime = DateTime.Now;
                        //20191121 ghk_display_strip
                        if (Chk_Strip())
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error: Strip Detected");

                            m_iStep = 0;
                            return true;
                        }

                        // 200319 mjy : Unit 모드에서 Unit 갯수 확인
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            // 200708 jym : 유닛 개숫에 따른 조건 추가
                            bool bX1 = CData.Parts[(eWay == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 0 : 0];
                            bool bX2 = CData.Parts[(eWay == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 1 : 0];
                            bool bX3 = CData.Parts[(eWay == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 2 : 1];
                            bool bX4 = CData.Parts[(eWay == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 3 : 1];

                            if (CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_1_Vacuum : eX.GRDR_Unit_1_Vacuum) != bX1 ||
                                CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_2_Vacuum : eX.GRDR_Unit_2_Vacuum) != bX2 ||
                                CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_3_Vacuum : eX.GRDR_Unit_3_Vacuum) != bX3 ||
                                CIO.It.Get_X((eWay == EWay.L) ? eX.GRDL_Unit_4_Vacuum : eX.GRDR_Unit_4_Vacuum) != bX4)
                            {
                                eErr eEr = (eWay == EWay.L) ? eErr.LEFT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR : eErr.RIGHT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR;
                                CErr.Show(eEr);
                                _SetLog("Error : Table unit not all vacuum.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        //190717 ksg :
                        if (CSQ_Main.It.m_iStat == EStatus.Manual)
                        {
                            if (eWay == EWay.L)
                            {
                                if (CMot.It.Get_FP(m_iONPX) > CData.SPos.dONP_X_PlaceL - 10)
                                {
                                    CErr.Show(eErr.ONLOADERPICKER_X_AXIS_NOT_WIAT_POSITION);
                                    _SetLog("Error : ONP X axis not wait.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                            else
                            {
                                if (CMot.It.Get_FP(m_iONPX) > CData.Dev.dOffP_X_ClnStart + 5)
                                {
                                    CErr.Show(eErr.ONLOADERPICKER_X_AXIS_NOT_WIAT_POSITION);
                                    _SetLog("Error : ONP X axis not wait.");

                                    m_iStep = 0;
                                    return true;
                                }
                            }
                        }

                        //190314 ksg :
                        if (eWay == EWay.L)
                        {
                            //210901 syc : 2004U
                            bool bval = false;
                            if (CDataOption.Use2004U) { bval = !CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDL_TbFlow) || CIO.It.Get_Y(eY.GRDL_TbFlow) || !CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U); }
                            else                      { bval = !CIO.It.Get_X(eX.GRDL_TbVaccum)       || CIO.It.Get_X(eX.GRDL_TbFlow) || CIO.It.Get_Y(eY.GRDL_TbFlow); }
                            //210901 syc : 2004U
                            //if (!CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDL_TbFlow) || CIO.It.Get_Y(eY.GRDL_TbFlow))
                            if (bval)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_NO_STRIP_PICK_UP_ON_LEFT_TABLE);
                                _SetLog("Not detect strip in left table.");

                                m_iStep = 0;
                                return true;
                            }

                        }
                        if (eWay == EWay.R)
                        {
                            //210901 syc : 2004U
                            bool bval = false;
                            if (CDataOption.Use2004U) { bval = !CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDR_TbFlow) || CIO.It.Get_Y(eY.GRDR_TbFlow) || !CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U); }
                            else                      { bval = !CIO.It.Get_X(eX.GRDR_TbVaccum)       || CIO.It.Get_X(eX.GRDR_TbFlow) || CIO.It.Get_Y(eY.GRDR_TbFlow); }
                            //210901 syc : 2004U
                            //if (!CIO.It.Get_X(eX.GRDR_TbVaccum) || CIO.It.Get_X(eX.GRDR_TbFlow) || CIO.It.Get_Y(eY.GRDR_TbFlow))
                            if(bval)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_NO_STRIP_PICK_UP_ON_Right_TABLE);
                                _SetLog("Not detect strip in right table.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        //190822 ksg :
                        if (eWay == EWay.L)
                        {
                            //210901 syc : 2004U
                            bool bval = false;
                            if(CDataOption.Use2004U) { bval = (!CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || !CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U)) && CData.Parts[(int)EPart.GRDL].bExistStrip; }
                            else                     { bval =  !CIO.It.Get_X(eX.GRDL_TbVaccum)                                                    && CData.Parts[(int)EPart.GRDL].bExistStrip; }

                            //210901 syc : 2004U
                            //if (!CIO.It.Get_X(eX.GRDL_TbVaccum) && CData.Parts[(int)EPart.GRDL].bExistStrip)
                            if (bval)
                            {
                                CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_LEFT);
                                _SetLog("Unkown strip in left table.");
                                
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (eWay == EWay.R)
                        {
                            //210901 syc : 2004U
                            bool bval = false;
                            if(CDataOption.Use2004U) { bval = (!CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || (!CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U)) && CData.Parts[(int)EPart.GRDL].bExistStrip); }
                            else                     { bval =  !CIO.It.Get_X(eX.GRDR_TbVaccum)                                                     && CData.Parts[(int)EPart.GRDL].bExistStrip ; }

                            //210901 syc : 2004U                            
                            //if (!CIO.It.Get_X(eX.GRDR_TbVaccum) && CData.Parts[(int)EPart.GRDL].bExistStrip)
                            if(bval)
                            {
                                CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_RIGHT);
                                _SetLog("Unkown strip in right table.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        //210909 syc : 2004U st
                        // 4U : 케리어 pick -> Vac
                        // 4U : Unit Pick -> Gripper
                        if (CDataOption.Use2004U)
                        {
                            Func_CarrierClampOpen();
                        }
                        else
                        {
                            Func_Vacuum(false);
                            Func_Eject(false);
                            Func_Drain(false);
                        }
                        //
                        CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:    //Z축 대기위치 이동 확인, X축 테이블 위치 이동, 피커 0도
                    if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                    { return false; }

                    if (eWay == EWay.L)
                    {
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickL);
                        _SetLog("X axis move pick-l.", CData.SPos.dOFP_X_PickL);
                    }
                    else
                    {
                        // 2021.08.24 SungTae Start : [추가] 조건 추가
                        if (CMot.It.Get_CP((int)EAx.OnLoaderPicker_X) < CData.SPos.dONP_X_PlaceR - 50)
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickR);
                            _SetLog("X axis move pick-r.", CData.SPos.dOFP_X_PickR);
                        }
                        else
                        {
                            return false;
                        }
                        // 2021.08.24 SungTae End
                    }

                    Func_Picker0();

                    m_iStep++;
                    return false;

                case 12:    //X축 테이블 위치 이동 확인, 피커 0도 확인
                    if ((eWay == EWay.L) && !CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_PickL))
                    {
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickL); //200311 ksg :
                        { return false; }
                    }
                    if ((eWay == EWay.R) && !CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_PickR))
                    {
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickR); //200311 ksg :
                        { return false; }
                    }
                    if (!Func_Picker0())
                    { return false; }

                    _SetLog("X axis move check.  Picker 0 check.");

                    m_iStep++;
                    return false;

                case 13:    //테이블 대기위치 확인, 이젝트 온, Z축 pick - slow 위치 이동, 딜레이 200mm/s 세팅
                    // 2020.11.24 JSKim St
                    if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                    {
                        if (eWay == EWay.L && CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L])
                        {
                            return false;
                        }

                        if (eWay == EWay.R && CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                        {
                            return false;
                        }
                    }
                    else
                    {
                    // 2020.11.24 JSKim Ed
                        if (eWay == EWay.L && CMot.It.Get_CP(m_iLTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.L]) // Encode == Left Table Pkg 배출위치 비교
                        {
                            CErr.Show(eErr.LEFT_GRIND_TABLE_NOT_READY);
                            _SetLog("Error : Left table not ready.");

                            m_iStep = 0;
                            return true;
                        }

                        if (eWay == EWay.R && CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R]) // Encode == Right Table Pkg 배출위치 비교
                        {
                            CErr.Show(eErr.RIGHT_GRIND_TABLE_NOT_READY);
                            _SetLog("Error : Right table not ready.");

                            m_iStep = 0;
                            return true;
                        }
                    // 2020.11.24 JSKim St
                    }
                    // 2020.11.24 JSKim Ed

                    //210909 syc : 2004U st
                    // 4U : 케리어 pick -> Vac
                    // 4U : Unit Pick -> Gripper
                    if (!CDataOption.Use2004U) { Func_Eject(true);  }
                    
                    CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow); //Pick 직전까지 내림
                    m_Delay.Set_Delay(200);
                    _SetLog("Z axis move position.  Set delay : 200ms", CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow);

                    m_iStep++;
                    return false;

                case 14:    //200mm/s 후 이젝트 오프, Z축 pick - slow 위치 이동 확인, 이젝트 오프, Z축 pick 위치 이동(슬로우)
                    {
                        if (m_Delay.Chk_Delay())
                        { Func_Eject(false); }

                        //loadcell 관련 루틴 추가

                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow))
                        { return false; } //Pick 위치로 내림

                        Func_Eject(false);

                        CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick); //Pick까지 Slow 내림
                        _SetLog("Z axis move pick.", CData.Dev.dOffP_Z_Pick);

                        m_iStep++;
                        return false;
                    }

                case 15:    //Z축 Pick 위치 이동(슬로우) 확인
                    {         
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick))
                        { return false; }

                        _SetLog("Z axis move check.");

                        m_iStep++;
                        return false;
                    }

                case 16:    //버큠 온, 이젝트 오프, 테이블 버큠 오프, 테이블 이젝트 온
                    {

                        //210909 syc : 2004U st
                        // 4U : 케리어 pick -> Vac
                        // 4U : Unit Pick -> Gripper
                        if(CDataOption.Use2004U)
                        {
                            Func_CarrierClampClose();
                        }
                        else
                        {
                            Func_Vacuum(true);
                            Func_Eject(false);
                        }
                        //

                        //200330 mjy : Act 함수 활용
                        if (eWay == EWay.L)
                        {
                            CData.L_GRD.ActVacuum(false);
                            CData.L_GRD.ActEject(true);
                            _SetLog("GDL vacuum off.  GDL eject on.");
                        }
                        else
                        {
                            CData.R_GRD.ActVacuum(false);
                            CData.R_GRD.ActEject(true);
                            _SetLog("GDR vacuum off.  GDR eject on.");
                        }

                        int iDly = (CData.CurCompany != ECompany.SkyWorks) ? 1000 : 5000;
                        m_Delay.Set_Delay(iDly);
                        _SetLog("Set delay : " + iDly + "ms");

                        m_iStep++;
                        return false;
                    }

                case 17:    //딜레이 확인, 테이블 이젝트 오프, 테이블 워터 온
                    {
                        if (!m_Delay.Chk_Delay())
                        {
                            return false;
                        }

                            //200330 mjy : Act 함수 활용
                        if (eWay == EWay.L)
                        {
                            if (CData.CurCompany != ECompany.JSCK) //2021.01.11 lhs
                            {
                                CData.L_GRD.ActEject(false);
                                _SetLog("GDL eject off");
                            }
                            
                            // 2020.10.21 JSKim St
                            if (CData.Dev.bMeasureMode)
                            {
                                CData.L_GRD.ActWater(false);
                                _SetLog("MeasureMode, GDL Water Off.");  //2020.12.30 lhs
                            }
                            else
                            {
                                CData.L_GRD.ActWater(true);
                                _SetLog("GDL Water On.");                //2020.12.30 lhs
                            }
                            // 2020.10.21 JSKim Ed
                        }
                        else
                        {
                            if (CData.CurCompany != ECompany.JSCK) //2021.01.11 lhs
                            {
                                CData.R_GRD.ActEject(false);
                                _SetLog("GDR eject off");
                            }

                            // 2020.10.21 JSKim St
                            if (CData.Dev.bMeasureMode)
                            {
                                CData.R_GRD.ActWater(false);
                                _SetLog("MeasureMode, GDR Water Off.");  //2020.12.30 lhs
                            }
                            else
                            {
                                CData.R_GRD.ActWater(true);
                                _SetLog("GDR Water On.");                //2020.12.30 lhs
                            }
                            // 2020.10.21 JSKim Ed
                        }

                        m_Delay.Set_Delay(CData.Dev.iPickDelayOff); //옵션에서 지정된 Delay 값
                        _SetLog("Set delay : " + CData.Dev.iPickDelayOff + "ms");

                        m_iStep++;
                        return false;
                    }

                case 18:    //버큠 온 확인, 테이블 버큠 온, 테이블 워터 오프, 버큠 오프, 이젝트 온
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if (CDataOption.Use2004U)
                        {
                            //210909 syc : 2004U st
                            // 4U : 케리어 pick -> Vac
                            // 4U : Unit Pick  -> Gripper
                            // 케리어 클램프가 On 상태 && 케리어 디텍트 센서가 On 상태 일경우 케리어 있다고 판단
                            bxVacChk = (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) && !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open)) &&
                                        CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect);
                        }
                        else 
                        {
                            bxVacChk = CIO.It.Get_X(eX.OFFP_Vacuum);    //X62 offLoad Picker Vac
                        }

                        if (m_Delay.Chk_Delay())
                        {
                            if (!bxVacChk)
                            {
                                //210909 syc : 2004U st
                                // 4U : 케리어 pick -> Vac
                                // 4U : Unit Pick -> Gripper
                                if (CDataOption.Use2004U)
                                {
                                    _SetLog("OFFP_Carrier_Clamp_Close : "   + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));
                                    _SetLog("OFFP_Carrier_Clamp_Open : "    + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));
                                    _SetLog("OFFP_Picker_Carrier_Detect : " + CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect));

                                    if (eWay == EWay.L)
                                    {
                                        CData.L_GRD.ActWater(false);
                                        CData.L_GRD.ActVacuum(true);
                                        _SetLog("OffPicker Vacuum False -> GDL water off.  GDL vacuum on.");
                                    }
                                    else
                                    {
                                        CData.R_GRD.ActWater(false);
                                        CData.R_GRD.ActVacuum(true);
                                        _SetLog("OffPicker Vacuum False -> GDR water off.  GDR vacuum on.");
                                    }
                                    //Func_CarrierClampOpen();
                                    //_SetLog("OffPicker Clamp Open");

                                    m_iStep = 40;
                                    return false;
                                }
                                //

                                //200330 mjy : Act 함수 활용
                                if (eWay == EWay.L)
                                {
                                    CData.L_GRD.ActWater(false);
                                    CData.L_GRD.ActVacuum(true);
                                    _SetLog("OffPicker Vacuum False -> GDL water off.  GDL vacuum on.");
                                }
                                else
                                {
                                    CData.R_GRD.ActWater(false);
                                    CData.R_GRD.ActVacuum(true);
                                    _SetLog("OffPicker Vacuum False -> GDR water off.  GDR vacuum on.");
                                }

                                Func_Vacuum(false);
                                Func_Eject(true);

                                int iDly2 = (CData.CurCompany != ECompany.SkyWorks) ? 1000 : 2000;
                                m_Delay.Set_Delay(iDly2);
                                _SetLog("Set delay : " + iDly2 + "ms");

                                m_iStep = 40;
                                return false;
                            }
                        }

                        if (!bxVacChk)
                        { return false; }
                        
                        int iDly = (CData.CurCompany != ECompany.SkyWorks) ? 1000 : 4000;
                        m_Delay.Set_Delay(iDly);
                        _SetLog("Set delay : " + iDly + "ms");

                        m_iStep = 20;
                        return false;
                    }

                case 20:    //딜레이 확인, 테이블 워터 오프, Z축 pick - gap 위치 이동(슬로우)
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        //200407 jym : 오른쪽에 내려놓을때 듀얼모드면 갭 시퀀스 사용 안함
                        if (eWay == EWay.R && CData.Dev.bDual == eDual.Dual)    { m_dTempGap = 0;                   }
                        else                                                    { m_dTempGap = CData.Opt.dPickGap;  }
                        _SetLog("Gap : " + m_dTempGap + "mm");

                        //200330 mjy : Gap 시퀀스 추가
                        CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick - m_dTempGap);
                        _SetLog("Z axis move position(slow).", CData.Dev.dOffP_Z_Pick - m_dTempGap);

                        m_iStep++;
                        return false;
                    }

                case 21:    //Z축 pick - slow 위치 이동(슬로우) 확인, Z축 대기위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick - m_dTempGap))
                        {
                            CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick - m_dTempGap);
                            return false;
                        }

                        //2020.12.30 lhs Start
                        if (eWay == EWay.L)     {   CData.L_GRD.ActEject(false);    }
                        else                    {   CData.R_GRD.ActEject(false);    }
                        //2020.12.30 lhs End
                        
                        //200407 jym : 오른쪽에 내려놓을때 듀얼모드면 딜레이 시퀀스 사용 안함
                        int iDly = (eWay == EWay.R && CData.Dev.bDual == eDual.Dual) ? 0 : CData.Opt.iPickDelay;
                        m_Delay.Set_Delay(iDly);
                        _SetLog("Set delay : " + iDly + "ms");

                        m_iStep++;
                        return false;
                    }

                case 22:    //딜레이 확인, 테이블 워터 오프, Z축 pick - slow 위치 이동(슬로우)
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        //200429 jym : 메뉴얼 동작에서 테이블 워터 오프
                        if (!CDataOption.IsTblWater || CSQ_Main.It.m_iStat == EStatus.Manual)
                        {
                            if (eWay == EWay.L) { CData.L_GRD.ActWater(false); }//Y32 table Clean Water  
                            else                { CData.R_GRD.ActWater(false); }//Y4A table Clean Water 
                            _SetLog(string.Format("{0} water off.", (eWay == EWay.L) ? "GDL" : "GDR"));
                        }

                        CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow);
                        _SetLog("Z axis move position(slow).", CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow);

                        m_iStep++;
                        return false;
                    }

                case 23:    //Z축 pick - slow 위치 이동(슬로우) 확인, Z축 대기위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow))
                        {
                            CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow);
                            return false;
                        }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 24:    //Z축 대기위치 이동 확인, 버큠 확인, X축 크리닝 스타트 위치로 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        
                        if (CDataOption.Use2004U)
                        {
                            //210909 syc : 2004U st
                            // 4U : 케리어 pick -> Vac
                            // 4U : Unit Pick -> Gripper
                            // 케리어 클램프가 On 상태 && 케리어 디텍트 센서가 On 상태 일경우 케리어 있다고 판단
                            bxVacChk = (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) && !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open)) &&
                                        CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect);
                        }
                        else 
                        { 
                            bxVacChk = CIO.It.Get_X(eX.OFFP_Vacuum);    //X62 offLoad Picker Vac
                        }

                        if (!bxVacChk)
                        {
                            if (CDataOption.Use2004U)
                            {
                                ShowVacChkErr_Pick_2004U();  // 2022.08.10 lhs : 에러 메시지 세분화

                                CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);

                                // 2022.08.10 lhs Start : 아래에 있던 코드 여기로 이동 (아래에 있으면 실행 안됨)
                                if (eWay == EWay.L) {   /* 냉무 */    }
                                else
                                {
                                    CIO.It.Set_Y((int)eY.GRDR_Unit_Vacuum_4U,    true);
                                    CIO.It.Set_Y((int)eY.GRDR_Carrier_Vacuum_4U, true);
								}
                                // 2022.08.10 lhs End

                                m_iStep = 0;
								return true;
							}

							//set Error
							CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                            _SetLog("Error : Vacuum check fail.");

                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);

                            if (eWay == EWay.L)
                            {                                
                                CIO.It.Set_Y((int)eY.GRDL_TbVacuum, true);  //Y31 Table Vac1
                                //                           
                                if (CDataOption.Package == ePkg.Unit)   // Hisilicon 일 경우 //20200229 LCY
                                {
                                    CIO.It.Set_Y(eY.GRDL_CarrierVacuum, true);
                                    CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, true);  //Y3F Left Unit Vac1 
                                    CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, true);  //Y41 Left Unit Vac2 
                                    CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, true);  //Y43 Left Unit Vac3 
                                    CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, true);  //Y44 Left Unit Vac4 
                                }
                                CIO.It.Set_Y((int)eY.GRDL_TbFlow, false);//Y32 table Clean Water  
                            }
                            else
                            {
                                CIO.It.Set_Y((int)eY.GRDR_TbVacuum, true);  //Y49 talbe Vac1                                  

                                if (CDataOption.Package == ePkg.Unit)   // Hisilicon 일 경우 //20200229 LCY       
                                {
                                    CIO.It.Set_Y(eY.GRDR_CarrierVacuum, true);
                                    CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, true);  //Y3F Right Unit Vac1 
                                    CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, true);  //Y41 Right Unit Vac2 
                                    CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, true);  //Y43 Right Unit Vac3 
                                    CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, true);  //Y44 Right Unit Vac4 
                                }
                                CIO.It.Set_Y((int)eY.GRDR_TbFlow, false);//Y4A table Clean Water 
                            }

                            Func_Vacuum(false);

                            m_iStep = 0;
                            return true;
                        } // end... if (!bxVacChk)

                        //20200415 jhc : Picker Idle Time 개선
                        if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (eWay == EWay.L) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickR);
                            _SetLog("X axis move pick-r.", CData.SPos.dOFP_X_PickR);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);
                            _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnStart);
                        }

                        //Data 이동 & 상태 변환 & Table Clean 변수 변환           
                        if (eWay == EWay.L)
                        {
                            CData.Parts[(int)EPart.OFP].sLotName    = CData.Parts[(int)EPart.GRDL].sLotName;
                            CData.Parts[(int)EPart.OFP].iMGZ_No     = CData.Parts[(int)EPart.GRDL].iMGZ_No;
                            CData.Parts[(int)EPart.OFP].iSlot_No    = CData.Parts[(int)EPart.GRDL].iSlot_No;
                            CData.Parts[(int)EPart.OFP].sBcr        = CData.Parts[(int)EPart.GRDL].sBcr;
                            CData.Parts[(int)EPart.OFP].dShiftT     = CData.Parts[(int)EPart.GRDL].dShiftT; //190529 ksg : //20200416 jhc :
                            CData.Parts[(int)EPart.GRDL].sBcr       = "";                                   //190823 ksg :
                            CData.Parts[(int)EPart.OFP].bChkGrd     = false;                                //200624 pjh : Grinding 중 Error Check 변수
                            CData.Parts[(int)EPart.OFP].LotColor    = CData.Parts[(int)EPart.GRDL].LotColor;
                            CData.Parts[(int)EPart.OFP].nLoadMgzSN  = CData.Parts[(int)EPart.GRDL].nLoadMgzSN;

                            // 2022.08.17 SungTae
                            _SetLog($"---------> [확인용] From : Part[GRDL].ShiftT = {CData.Parts[(int)EPart.GRDL].dShiftT}");
                            _SetLog($"---------> [확인용]   To : Part[OFP ].ShiftT = {CData.Parts[(int)EPart.OFP].dShiftT}");

                            // 2022.05.04 SungTae Start : [추가]
                            if (CData.CurCompany == ECompany.ASE_K12)
                                CData.Parts[(int)EPart.OFP].sMGZ_ID = CData.Parts[(int)EPart.GRDL].sMGZ_ID;
                            // 2022.05.04 SungTae End

                            // 2022.05.04 SungTae : [확인용]
                            _SetLog($"LoadMgzSN : GRDL({CData.Parts[(int)EPart.GRDL].nLoadMgzSN}) -> OFP({CData.Parts[(int)EPart.OFP].nLoadMgzSN})");

                            // 200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                            // Data Shift : L-Table => Off-Picker (18 Point 측정 여/부)
                            CData.Parts[(int)EPart.OFP].b18PMeasure = CData.Parts[(int)EPart.GRDL].b18PMeasure; //Data Shift는 조건별 사용 여부에 무관하게 진행 함
                            CData.Parts[(int)EPart.GRDL].b18PMeasure = false;
                            //

                            // 20190222 ghk_dynamicfunction
                            Array.Copy(CData.Parts[(int)EPart.GRDL].dPcb, CData.Parts[(int)EPart.OFP].dPcb, CData.Parts[(int)EPart.GRDL].dPcb.Length);
                            CData.Parts[(int)EPart.OFP].dPcbMax     = CData.Parts[(int)EPart.GRDL].dPcbMax;
                            CData.Parts[(int)EPart.OFP].dPcbMin     = CData.Parts[(int)EPart.GRDL].dPcbMin;
                            CData.Parts[(int)EPart.OFP].dPcbMean    = CData.Parts[(int)EPart.GRDL].dPcbMean;

                            //-------------------------
                            // 2022.03.23 lhs Start : 2004U, CarrierWithDummy Flag
                            if (CDataOption.Use2004U)
                            {
                                if(CData.Dev.bDual == eDual.Normal)
                                {
                                    CData.Parts[(int)EPart.GRDL].bCarrierWithDummy = false;  // LT, Normal
                                    //CData.Dev.aData[0].bDummy 초기화는 Before Measure에서...
								}
                                else // Dual일 경우 OnPicker에서 LT Pick을 하기 때문에 OffPicker에서는 Dummy 데이터 전달 필요 없음
                                {
                                    // 냉무
								}
                            }
                            // 2022.03.23 lhs End
                            //-------------------------

                            // 2021.08.21 lhs Start : Top/Btm Mold 데이터 전달
                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.Parts[(int)EPart.OFP].dTopMoldMax = CData.Parts[(int)EPart.GRDL].dTopMoldMax;
                                CData.Parts[(int)EPart.OFP].dTopMoldAvg = CData.Parts[(int)EPart.GRDL].dTopMoldAvg;
                                CData.Parts[(int)EPart.OFP].dBtmMoldMax = CData.Parts[(int)EPart.GRDL].dBtmMoldMax;
                                CData.Parts[(int)EPart.OFP].dBtmMoldAvg = CData.Parts[(int)EPart.GRDL].dBtmMoldAvg;
                            }
                            // 2021.08.21 lhs End

                            if ((CData.GemForm != null) && (CData.Opt.bSecsUse == true))
                            {
                                if (CData.Dev.bDual == eDual.Dual)  {   CData.GemForm.Strip_Data_Shift((int)EDataShift.GRL_AF_MEAS/*10*/, (int)EDataShift.OFP_LT_PICK/*11*/);   }
                                else                                {   CData.GemForm.Strip_Data_Shift((int)EDataShift.GRL_AF_MEAS/*10*/, (int)EDataShift.OFP_RT_PICK/*17*/);   }
                            }

                            //20200317 jhc : 패키지 유닛일 경우 유닛 데이터 복사
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                Array.Copy(CData.Parts[(int)EPart.GRDL].aUnitEx, CData.Parts[(int)EPart.OFP].aUnitEx, CData.Dev.iUnitCnt);
                            }

                            //ksg Strip 유무 변경
                            CData.Parts[(int)EPart.OFP].bExistStrip     = CData.Parts[(int)EPart.GRDL].bExistStrip;
                            CData.Parts[(int)EPart.GRDL].bExistStrip    = false;
                            CData.Parts[(int)EPart.GRDL].iStripStat     = 0;

                            //20200420 jhc : Auto-Mode && STEP 모드 && 피커 동작 개선 옵션 => 드레싱 전 테이블 클리닝 안 함
                            //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            if ((CDataOption.PickerImprove  == ePickerTimeImprove.Use)  && 
                                (CData.Dev.bDual            == eDual.Dual)              &&
                                (CData.Dev.aData[(int)EWay.L].dDrsPrid != 0) && (CData.Whls[(int)EWay.L].iGdc >= CData.Dev.aData[(int)EWay.L].dDrsPrid))
                            {
                                CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_Wait; //그라인딩 AutoRun()에서 드레싱 시퀀스로 전환됨
                            }
                            else
                            {
                                // 2020-11-15, jhLee : Skyworks VOC, Table clean 주기를 지정하여 동작한다.
                                if (CData.CurCompany == ECompany.SkyWorks)
                                {
                                    if (CData.Opt.iTC_Cycle > 1)                                            // Table cleaning 주기가 설정되었다면
                                    {
                                        CData.L_GRD.m_iProcessCnt++;                                        // 처리된 제품 수 증가
                                        if (CData.L_GRD.m_iProcessCnt >= CData.Opt.iTC_Cycle)               // 설정된 주기를 넘겼다면
                                        {
                                            _SetLog("Left Table clean process : " + CData.L_GRD.m_iProcessCnt.ToString() + " / " + CData.Opt.iTC_Cycle.ToString());
                                            CData.L_GRD.m_iProcessCnt = 0;                                  // 처리 제품수 초기화
                                            CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_Table_Clean;      // Table Clean 동작 수행
                                        }
                                        else
                                        {
                                            // Table clean 작업 skip
                                            CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_Ready;
                                            _SetLog("Left Table clean skip : " + CData.L_GRD.m_iProcessCnt.ToString() + " / " + CData.Opt.iTC_Cycle.ToString());
                                        }
                                    }
                                    else
                                    {
                                        CData.L_GRD.m_iProcessCnt = 0;      // 처리 제품수 초기화 (도중에 옵션이 변경될 경우 고려)
                                        CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_Table_Clean;          // 곧바로 Table Clean 동작 수행
                                    }
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.GRDL].iStat = ESeq.GRL_Table_Clean;              // 곧바로 Table Clean 동작 수행
                                }
                                // end of 2020-11-15, jhLee

                            }
                            //
     
                            //20200415 jhc : Picker Idle Time 개선
                            if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            { 
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_PlaceTbR;  //Auto-Running, Dual Mode에서 OffPicker가 L-Table에서 Pick했다면 다음 스텝에서 R-Table에 Place 해야 함        
                            }
                            else
                            { 
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmCleanStrip;
                                CSq_OfP.It.m_nStripNzlCycle = 0; // Spray Nozzle의 #1(첫번째) Count값으로 동작 : 2022.07.28 lhs
                            }
                            //

                            if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd)
                            {
                                CData.L_GRD.LastTbClean             = true;
                                CData.Parts[(int)EPart.GRDL].iStat  = ESeq.GRL_WorkEnd;

                                _SetLog(">>>>> GRL Work End, CData.L_GRD.LastTbClean = true");  // 2021.11.18 lhs 
                            }
                        }
                        else   // Right Table
                        {
                            CData.Parts[(int)EPart.OFP].sLotName    = CData.Parts[(int)EPart.GRDR].sLotName;
                            CData.Parts[(int)EPart.OFP].iMGZ_No     = CData.Parts[(int)EPart.GRDR].iMGZ_No;
                            CData.Parts[(int)EPart.OFP].iSlot_No    = CData.Parts[(int)EPart.GRDR].iSlot_No;
                            CData.Parts[(int)EPart.OFP].sBcr        = CData.Parts[(int)EPart.GRDR].sBcr;
                            CData.Parts[(int)EPart.GRDR].sBcr       = ""; //190823 ksg :
                            CData.Parts[(int)EPart.OFP].bChkGrd     = false; //200624 pjh : Grinding 중 Error Check 변수
                            CData.Parts[(int)EPart.OFP].LotColor    = CData.Parts[(int)EPart.GRDR].LotColor;
                            CData.Parts[(int)EPart.OFP].nLoadMgzSN  = CData.Parts[(int)EPart.GRDR].nLoadMgzSN;

                            // 2022.05.04 SungTae Start : [추가]
                            if (CData.CurCompany == ECompany.ASE_K12)
                                CData.Parts[(int)EPart.OFP].sMGZ_ID = CData.Parts[(int)EPart.GRDR].sMGZ_ID;
                            // 2022.05.04 SungTae End

                            // 2022.05.04 SungTae : [확인용]
                            _SetLog($"LoadMgzSN : GRDR({CData.Parts[(int)EPart.GRDR].nLoadMgzSN}) -> OFP({CData.Parts[(int)EPart.OFP].nLoadMgzSN})");

                            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                            // Data Shift : R-Table => Off-Picker (18 Point 측정 여/부)
                            CData.Parts[(int)EPart.OFP].b18PMeasure  = CData.Parts[(int)EPart.GRDR].b18PMeasure; //Data Shift는 조건별 사용 여부에 무관하게 진행 함
                            CData.Parts[(int)EPart.GRDR].b18PMeasure = false;
                            //

                            //20190222 ghk_dynamicfunction
                            Array.Copy(CData.Parts[(int)EPart.GRDR].dPcb, CData.Parts[(int)EPart.OFP].dPcb, CData.Parts[(int)EPart.GRDR].dPcb.Length);
                            CData.Parts[(int)EPart.OFP].dPcbMax     = CData.Parts[(int)EPart.GRDR].dPcbMax;
                            CData.Parts[(int)EPart.OFP].dPcbMin     = CData.Parts[(int)EPart.GRDR].dPcbMin;
                            CData.Parts[(int)EPart.OFP].dPcbMean    = CData.Parts[(int)EPart.GRDR].dPcbMean;
                            //

                            //-------------------------
                            // 2022.03.23 lhs Start : 2004U, RT Pick시 CarrierWithDummy Flag 초기화
                            if (CDataOption.Use2004U)
                            {
                                CData.Parts[(int)EPart.GRDR].bCarrierWithDummy = false;  // RT, Normal/Dual
                                //CData.Dev.aData[1].bDummy 초기화는 Before Measure에서...
                            }
                            // 2022.03.23 lhs End
                            //-------------------------

                            // 2021.08.21 lhs Start : Top/Btm Mold 데이터 전달
                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.Parts[(int)EPart.OFP].dTopMoldMax = CData.Parts[(int)EPart.GRDR].dTopMoldMax;
                                CData.Parts[(int)EPart.OFP].dTopMoldAvg = CData.Parts[(int)EPart.GRDR].dTopMoldAvg;
                                CData.Parts[(int)EPart.OFP].dBtmMoldMax = CData.Parts[(int)EPart.GRDR].dBtmMoldMax;
                                CData.Parts[(int)EPart.OFP].dBtmMoldAvg = CData.Parts[(int)EPart.GRDR].dBtmMoldAvg;
                            }
                            // 2021.08.21 lhs End

                            // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                            //if ((CData.GemForm != null) && (CData.Opt.bSecsUse == true)) CData.GemForm.Strip_Data_Shift(16, 17);// R/Table Data -> PickUp Data 로 이동
                            if ((CData.GemForm != null) && (CData.Opt.bSecsUse == true))
                            {
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.GRR_AF_MEAS/*16*/, (int)EDataShift.OFP_RT_PICK/*17*/);// R/Table Data -> PickUp Data 로 이동
                            }
                            // 2021.07.19 SungTae End

                            if (CDataOption.Package == ePkg.Unit)
                            {
                                Array.Copy(CData.Parts[(int)EPart.GRDR].aUnitEx, CData.Parts[(int)EPart.OFP].aUnitEx, CData.Dev.iUnitCnt);
                            }

                            //ksg Strip 유무 변경
                            CData.Parts[(int)EPart.OFP].bExistStrip = CData.Parts[(int)EPart.GRDR].bExistStrip;
                            CData.Parts[(int)EPart.GRDR].bExistStrip = false;
                            // 2020.12.12 JSKim St
                            CData.Parts[(int)EPart.GRDR].iStripStat = 0;
                            // 2020.12.12 JSKim Ed

                            //20200420 jhc : Auto-Mode && STEP 모드 && 피커 동작 개선 옵션 => 드레싱 전 테이블 클리닝 안 함
                            //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual) &&
                                (CData.Dev.aData[(int)EWay.R].dDrsPrid != 0) && (CData.Whls[(int)EWay.R].iGdc >= CData.Dev.aData[(int)EWay.R].dDrsPrid))
                            {
                                CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_Wait; //그라인딩 AutoRun()에서 드레싱 시퀀스로 전환됨
                            }
                            else
                            {
                                // 2020-11-15, jhLee : Skyworks VOC, Table clean 주기를 지정하여 동작한다.
                                if (CData.CurCompany == ECompany.SkyWorks)
                                {
                                    if (CData.Opt.iTC_Cycle > 1)                                            // Table cleaning 주기가 설정되었다면
                                    {
                                        CData.R_GRD.m_iProcessCnt++;                                        // 처리된 제품 수 증가
                                        if (CData.R_GRD.m_iProcessCnt >= CData.Opt.iTC_Cycle)               // 설정된 주기를 넘겼다면
                                        {
                                            _SetLog("Right Table clean process : " + CData.R_GRD.m_iProcessCnt.ToString() + " / " + CData.Opt.iTC_Cycle.ToString());
                                            CData.R_GRD.m_iProcessCnt = 0;  // 처리 제품수 초기화
                                            CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_Table_Clean;      // Table Clean 동작 수행
                                        }
                                        else
                                        {
                                            CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_Ready;
                                            _SetLog("Right Table clean skip : " + CData.R_GRD.m_iProcessCnt.ToString() + " / " + CData.Opt.iTC_Cycle.ToString());
                                        }
                                    }
                                    else
                                    {
                                        CData.R_GRD.m_iProcessCnt = 0;      // 처리 제품수 초기화 (도중에 옵션이 변경될 경우 고려)
                                        CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_Table_Clean;          // 곧바로 Table Clean 동작 수행
                                    }
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_Table_Clean;              // 곧바로 Table Clean 동작 수행
                                }
                                // end of 2020-11-15, jhLee
                            }
                            //

                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmCleanStrip;
                            CSq_OfP.It.m_nStripNzlCycle = 0; // Spray Nozzle의 #1(첫번째) Count값으로 동작 : 2022.07.28 lhs

                            //20200416 jhc : R-Table WORK_END 조건 추가
                            if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            {//Picker Idle Time 개선 옵션 && Auto-Running && 스텝 모드
                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd && //On-Loader WORK_END
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd && //In-Rail WORK_END
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd && //On-Picker WORK_END
                                    CData.Parts[(int)EPart.GRDL].bExistStrip == false)       //L-Table 자재 없음
                                {
                                    CData.R_GRD.LastTbClean = true;
                                    CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_WorkEnd;
                                    _SetLog(">>>>> GRR Work End, CData.R_GRD.LastTbClean = true");
                                }
                            }
                            else
                            {
                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd)
                                {
                                    CData.R_GRD.LastTbClean = true;
                                    CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_WorkEnd;
                                    _SetLog(">>>>> GRR Work End, CData.R_GRD.LastTbClean = true");
                                }
                            }
                        }
                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.OFP].iStat);  

                        m_iStep++;
                        return false;
                    }

                case 25:    //x축 크리닝 스타트 위치 이동 확인, 종료
                    {
                        //20200415 jhc : Picker Idle Time 개선
                        if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (eWay == EWay.L) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_PickR)) return false;
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart)) return false;
                        }

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;    
                        return true;    // 정상 종료
                    }

                case 40:    //에러 구문, z축 대기 위치 이동
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 41:    //z축 대기 위치 이동 확인, 이젝트 오프, 에러 발생
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        Func_Eject(false);

                        if (CDataOption.Use2004U)
                        {
                            ShowVacChkErr_Pick_2004U();  // 2022.08.10 lhs : 에러 메시지 세분화
                        }
                        else
                        {
                            //set Error // Picker vac
                            CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                            _SetLog("Error : Vacuum check fail.");
                        }

                        m_iStep = 0;
                        return true;
                    }
            }
        }


#if true //20200415 jhc : Picker Idle Time 개선
        /// <summary>
        /// Place Right Table Cycle
        /// </summary>
        /// <returns></returns>
        public bool Cyl_PlaceTbR()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    // 2020.11.24 JSKim St
                    if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running
                        && m_iStep == 15 && CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                    {
                        CErr.Show(eErr.RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                        _SetLog("Error : GDR Y axis not wait.");
                    }
                    else
                    {
                    // 2020.11.24 JSKim Ed
                        CErr.Show(eErr.OFFLOADERPICKER_PLACE_TIMEOUT);
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
                        return true;
                    }

                case 10:
                    {//버큠 온, 이젝트 오프, 워터 드레인 오프, z축 대기위치 이동
                        m_tStTime = DateTime.Now;
                        if (CSQ_Main.It.m_iStat == EStatus.Manual)
                        {
                            if (CMot.It.Get_FP(m_iONPX) > CData.SPos.dONP_X_PlaceL + 5) /*HERE : OnPicker X축 위치 값이 OffPicker X축 위치 값이 서로 다른 Encoder 값을 참조할텐데...상관 있나?*/
                            {
                                CErr.Show(eErr.ONLOADERPICKER_X_AXIS_NOT_WIAT_POSITION);
                                _SetLog("Error : ONP X axis not wait position.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        Func_Vacuum(true);
                        Func_Eject(false);
                        Func_Drain(false);

                        if (!Chk_Strip())
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error : Not detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        //Right Table 자재 확인
                        if (CIO.It.Get_X(eX.GRDR_TbVaccum) || (!CIO.It.Get_X(eX.GRDR_TbVaccum) && CData.Parts[(int)EPart.GRDR].bExistStrip))
                        {
                            CErr.Show(eErr.UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_RIGHT);
                            _SetLog("Error : GDR Unknown strip.");

                            m_iStep = 0;
                            return true;
                        }

                        if (!CIO.It.Get_Y(eY.PUMPR_Run))
                        { CIO.It.Set_Y(eY.PUMPR_Run, true); }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait); //상단 이동
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:
                    {//Z축 대기위치 이동 확인, X축 Place위치 이동, 피커 0도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        //Right Table로 X축 이동
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_PickR);
                        _SetLog("X axis move pick-r.", CData.SPos.dOFP_X_PickR);

                        //190430 ksg : Eject On 후 동작 시 Off 하는 곳이 없어 수정
                        if (CIO.It.Get_Y(eY.GRDR_TbEjector))
                        { CIO.It.Set_Y(eY.GRDR_TbEjector, false); }

                        Func_Picker0();

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {//X축 Place위치 이동 확인, 피커 0도 확인
                        //Right Table로 X축 이동
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_PickR))
                        { return false; }
                        if (!Func_Picker0())
                        { return false; }

                        _SetLog("X axis move check.  Picker 0 check.");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//테이블 워터 온, 테이블 버큠 오프
                        CData.R_GRD.ActVacuum(false);
                        // 2020.10.21 JSKim St
                        //CData.R_GRD.ActWater(true);
                        if (CData.Dev.bMeasureMode)
                        {
                            CData.R_GRD.ActWater(false);
                        }
                        else
                        {
                            CData.R_GRD.ActWater(true);
                        }
                        // 2020.10.21 JSKim Ed
                        _SetLog("GDR vacuum off.  GDR water on.");

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

                        if (!Chk_Strip())
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                            _SetLog("Error : Vacuum check fail.");

                            Func_Vacuum(false);
                            m_iStep = 0;
                            return true;
                        }

                        if (CData.CurCompany == ECompany.JCET && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            if (CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // 2022.07.22 SungTae Start : [수정] (ASE-KR VOC)
                            // Only SSG002호기에서 BTM Device(Cheddar) 양산 시에만 Right Table이 Auto Dressing 중
                            // "RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION" Alarm 다발하는 Issue로 인해 조건 추가
                            if (CData.Dev.bDual == eDual.Dual && CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Dressing)
                            {
                                m_TimeOut.Set_Delay(TIMEOUT);
                                return false;
                            }
                            // 2022.07.21 SungTae End

                            if (CMot.It.Get_CP(m_iRTY) != CData.SPos.dGRD_Y_Wait[(int)EWay.R])
                            {
                                CErr.Show(eErr.RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION);
                                _SetLog("Error : GDR Y axis not wait.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        _SetLog("Picker 0 check.");

                        m_iStep++;
                        return false;
                    }

                case 16:
                    {//Z축 place - slow 위치 이동
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow));
                        _SetLog("Z axis move position.", (CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 17:
                    bool bJSCK_Warp_Test_Mode = false;// SCK+ Warp 관련 요청 사항 20200201 LCY
                    {//Z축 Place - Slow 위치 이동 확인, 딜레이 세팅
                        if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow)))
                        { return false; }
                        if (bJSCK_Warp_Test_Mode == true)
                        {
                            Func_Vacuum(false);
                            Func_Eject(true);
                            CLog.mLogGnd(string.Format(" CSQ6_OfP 17 -> bJSCK_Warp_Test_Mode True"));
                        }

                        m_Place.Set_Delay(CData.Opt.aOnpRinseTime[(int)EWay.R] * 1000);
                        _SetLog("Set delay : " + (CData.Opt.aOnpRinseTime[(int)EWay.R] * 1000) + "ms");

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//딜레이 확인, 테이블 워터 오프, Z축 Place - Gap 위치 이동(슬로우)
                        if (!m_Place.Chk_Delay())
                        { return false; }
                        CLog.mLogGnd(string.Format(" CSQ6_OfP 18 -> Slow Down"));

                        //200407 jym : 오른쪽에 내려놓을때 듀얼모드면 갭 시퀀스 사용 안함
                        if (CData.Dev.bDual == eDual.Dual)
                        { m_dTempGap = 0; }
                        else
                        { m_dTempGap = CData.Opt.dPlaceGap; }

                        //200330 mjy : Gap 시퀀스 추가
                        CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick - m_dTempGap); //Z축 Place 
                        _SetLog("Z axis move position(slow).", CData.Dev.dOffP_Z_Pick - m_dTempGap);

                        m_iStep++;
                        return false;
                    }

                case 19:
                    {//Z축 Place 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick - m_dTempGap))
                        { return false; }

                        //200407 jym : 오른쪽에 내려놓을때 듀얼모드면 딜레이 시퀀스 사용 안함
                        int iDly = (CData.Dev.bDual == eDual.Dual) ? 0 : CData.Opt.iPlaceDelay;
                        m_Delay.Set_Delay(iDly);
                        _SetLog("Set delay : " + iDly + "ms");

                        m_iStep++;
                        return false;
                    }

                case 20:
                    {//딜레이 확인, 테이블 워터 오프, Z축 Place 위치 이동
                        if (!m_Place.Chk_Delay())
                        { return false; }
                        CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Pick); //Z축 Place 
                        _SetLog("Z axis move pick(slow).", CData.Dev.dOffP_Z_Pick);

                        m_iStep++;
                        return false;
                    }

                case 21:
                    {//Z축 Place 위치 이동 확인
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick))
                        { return false; }

                        CData.R_GRD.ActWater(false); //Y4A Right Table Water
                        _SetLog("GDR water off.");

                        m_iStep++;
                        return false;
                    }

                case 22:
                    {//버큠 오프, 이젝트 온, 테이블 버큠 온, 펌프 런
                        Func_Vacuum(false);
                        Func_Eject(true);

                        CIO.It.Set_Y(eY.PUMPR_Run, true);//Y76 Right Table Pump

                        //210902 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            CIO.It.Set_Y(eY.GRDR_Unit_Vacuum_4U, true);
                            CIO.It.Set_Y(eY.GRDR_Carrier_Vacuum_4U, true);
                        }
                        else
                        {
                            CIO.It.Set_Y(eY.GRDR_TbVacuum, true);//Y49 Right Table Vac1
                        }
                        // Unit 모드에서 버큠 체크
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bX1 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 0 : 0];
                            bool bX2 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 1 : 0];
                            bool bX3 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 2 : 1];
                            bool bX4 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 3 : 1];
                            CIO.It.Set_Y(eY.GRDR_CarrierVacuum, true);
                            CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, bX1);
                            CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, bX2);
                            CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, bX3);
                            CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, bX4);
                        }

                        _SetLog("Vacuum off.  GDR vacuum on.");

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

                        //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
                        int iDly = 0;
                        if (CDataOption.Package == ePkg.Strip)
                        {
                            // 2020.12.16 JSKim St
                            //iDly = (CData.CurCompany != ECompany.SkyWorks) ? 3000 : 5000;
                            if ((CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) && CData.Dev.iPlaceDelayOff != 0)
                            {
                                iDly = CData.Dev.iPlaceDelayOff;
                            }
                            else
                            {
                                iDly = (CData.CurCompany != ECompany.SkyWorks) ? 3000 : 5000;
                            }
                            // 2020.12.16 JSKim Ed
                        }
                        else
                        { iDly = CData.Dev.iPlaceDelayOff; }

                        m_Delay.Set_Delay(iDly);
                        _SetLog("Set delay : " + iDly + "ms");

                        m_iStep++;
                        return false;
                    }

                case 24:
                    {//테이블 버큠 확인                        
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        // 2020.12.14 JSKim St
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            Func_Eject(false);
                        }
                        // 2020.12.14 JSKim Ed

                        // 2020.12.14 JSKim Check
                        // 여기서 자재 유무를 넘겨 주고 자재 Data는 28에서 준다...
                        // Z축 Up 동작을 하면서 자재가 Table로 떨어질 테니 넘겨 주는 것이 맞다
                        // 하지만 왜 나머지 Data는 안넘겨줄까??
                        CData.Parts[(int)EPart.GRDR].bExistStrip = CData.Parts[(int)EPart.OFP].bExistStrip;
                        // 2020.12.12 JSKim St
                        CData.Parts[(int)EPart.GRDR].iStripStat = 0;
                        // 2020.12.12 JSKim Ed
                        CData.Parts[(int)EPart.OFP].bExistStrip = false;

                        bError_Safety_Mv = false;

                        if (!CIO.It.Get_X(eX.GRDR_TbVaccum))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_VACUUM_ERROR);
                            // 2020.12.14 JSKim St
                            // bError_Safety_Mv 처리는 Unit 에서만 하면서 여기서 왜 이짓을 하나????????
                            bError_Safety_Mv = true;
                            _SetLog("Error : Vacuum fail.");
                            // 2020.12.14 JSKim Ed
                        }

                        // Unit 모드일 때 버큠 체크
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bX1 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 0 : 0];
                            bool bX2 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 1 : 0];
                            bool bX3 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 2 : 1];
                            bool bX4 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 3 : 1];

                            if (CIO.It.Get_X(eX.GRDR_Unit_1_Vacuum) != bX1)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT1_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                _SetLog("Error : Unit 1 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X(eX.GRDR_Unit_2_Vacuum) != bX2)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT2_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                _SetLog("Error : Unit 2 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X(eX.GRDR_Unit_3_Vacuum) != bX3)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT3_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                _SetLog("Error : Unit 3 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                                bError_Safety_Mv = true;
                            }
                            if (CIO.It.Get_X(eX.GRDR_Unit_4_Vacuum) != bX4)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT4_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                _SetLog("Error : Unit 4 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                                bError_Safety_Mv = true;
                            }

                            // 2020.12.14 JSKim St - 이미 자재 유무 데이터가 넘어 갔다
                            // Strip 에서도 bError_Safety_Mv 살리는데 Unit에서만 확인한다고???????
                            //if (bError_Safety_Mv)
                            //{
                            //    CIO.It.Set_Y(eY.OFFP_Ejector, false);
                            //    CMot.It.Stop(m_iZ);
                            //    CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            //    _SetLog("Error : GDR vacuum error.");

                            //    m_iStep = 0;
                            //    return true;
                            //}
                            // 2020.12.14 JSKim Ed
                        }

                        // 2020.12.14 JSKim St
                        // Error 처리만 하고 Step를 27로 보내서 자재 데이터를 넘겨 주자
                        if (bError_Safety_Mv)
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            _SetLog("Error : GDR vacuum error.");

                            m_iStep = 27;
                            return false;
                        }
                        // 2020.12.14 JSKim Ed

                        _SetLog("GDR vacuum check.");

                        m_iStep++;
                        return false;
                    }

                case 25:
                    {//Z축 Place - slow 위치 이동(슬로우)
                        CMot.It.Mv_S(m_iZ, (CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow));
                        _SetLog("Z axis move position(slow).", (CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow));

                        m_iStep++;
                        return false;
                    }

                case 26:
                    {//Z축 Place - slow 위치 이동(슬로우) 확인, Z축 Wait 위치 이동, 워터 드레인 온
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Pick - CData.Dev.dOffP_Z_Slow))
                        { return false; }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        Func_Drain(true);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 27:
                    {//Z축 Wait 위치 이동 확인, 워터 드레인 오프
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        Func_Drain(false);
                        _SetLog("Drain off.");

                        m_iStep++;
                        return false;
                    }

                case 28:
                    {//테이블 버큠 다시 한번 확인, 데이터 변경
                        // 2020.12.14 JSKim St
                        // 여기서 bError_Safety_Mv 이걸 왜 체크하는지 모르겠다..
                        // 이미 자재 유무 정보가 넘어 간 상태인데 Vaccum Error 가 발생했다고 해도 자재 Data를 넘겨주는 것이 좋다...
                        // 마지막 Step 이므로 굳이 bError_Safety_Mv를 사용할 필요도 없다.
                        //bError_Safety_Mv = false;
                        // 2020.12.14 JSKim Ed

                        if (!CIO.It.Get_X(eX.GRDR_TbVaccum))
                        {
                            CErr.Show(eErr.RIGHT_GRIND_VACUUM_ERROR);
                            _SetLog("Error : GDR vacuum fail.");

                            // 2020.12.14 JSKim St
                            //Func_Eject(false);
                            //CIO.It.Set_Y(eY.GRDR_TbVacuum, false);
                            //CMot.It.Stop(m_iZ);
                            //CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);

                            //m_iStep = 0;
                            //return true;
                            // 2020.12.14 JSKim Ed
                        }

                        // Unit 모드일 때 버큠 체크
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            bool bX1 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 0 : 0];
                            bool bX2 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 1 : 0];
                            bool bX3 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 2 : 1];
                            bool bX4 = CData.Parts[(int)EPart.OFP].aUnitEx[(CData.Dev.iUnitCnt == 4) ? 3 : 1];

                            if (CIO.It.Get_X(eX.GRDR_Unit_1_Vacuum) != bX1)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT1_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                //bError_Safety_Mv = true;
                                _SetLog("Error : Unit 1 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                            }
                            if (CIO.It.Get_X(eX.GRDR_Unit_2_Vacuum) != bX2)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT2_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                //bError_Safety_Mv = true;
                                _SetLog("Error : Unit 2 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                            }
                            if (CIO.It.Get_X(eX.GRDR_Unit_3_Vacuum) != bX3)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT3_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                //bError_Safety_Mv = true;
                                _SetLog("Error : Unit 3 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                            }
                            if (CIO.It.Get_X(eX.GRDR_Unit_4_Vacuum) != bX4)
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT4_VACUUM_ERROR);
                                // 2020.12.14 JSKim St
                                //bError_Safety_Mv = true;
                                _SetLog("Error : Unit 4 vacuum fail.");
                                // 2020.12.14 JSKim Ed
                            }

                            // 2020.12.14 JSKim St
                            //if (bError_Safety_Mv)
                            //{
                            //    Func_Eject(false);
                            //    CMot.It.Stop(m_iZ);
                            //    CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            //    _SetLog("Error : GDR vacuum fail.");

                            //    m_iStep = 0;
                            //    return true;
                            //}
                            // 2020.12.14 JSKim Ed
                        }

                        //20190703 ghk_dfserver
                        Array.Copy(CData.Parts[(int)EPart.OFP].dPcb, CData.Parts[(int)EPart.GRDR].dPcb, CData.Parts[(int)EPart.OFP].dPcb.Length);

                        //-------------------------
                        // 2022.03.23 lhs Start : 2004U, OnPicker에서 RT Place를 하기 때문에 OffPicker에서는 Dummy 데이터 전달 필요 없음
                        //if (CDataOption.Use2004U)
                        //{
                        //}
                        // 2022.03.23 lhs End
                        //-------------------------

                        if ((CData.Opt.bSecsUse == true) || (CDataOption.MeasureDf == eDfServerType.MeasureDf))
                        {
                            // SECSGEM 사용시 Host에서 Down 받은 Data 사용 20200315 LCY
                            // 20200507 (dDf -> dPcb) 잘 못 적용 된 값 전달 부 SCK현장 수정 PJH
                            CData.Parts[(int)EPart.GRDR].dPcbMin = CData.Parts[(int)EPart.OFP].dPcbMin;
                            CData.Parts[(int)EPart.GRDR].dPcbMax = CData.Parts[(int)EPart.OFP].dPcbMax;
                            CData.Parts[(int)EPart.GRDR].dPcbMean = CData.Parts[(int)EPart.OFP].dPcbMean;

                            // 2021.08.21 lhs Start : Top/Btm Mold 데이터 전달
                            if (CDataOption.UseNewSckGrindProc)
                            {
                                CData.Parts[(int)EPart.GRDR].dTopMoldMax = CData.Parts[(int)EPart.OFP].dTopMoldMax;
                                CData.Parts[(int)EPart.GRDR].dTopMoldAvg = CData.Parts[(int)EPart.OFP].dTopMoldAvg;
                                CData.Parts[(int)EPart.GRDR].dBtmMoldMax = CData.Parts[(int)EPart.OFP].dBtmMoldMax;
                                CData.Parts[(int)EPart.GRDR].dBtmMoldAvg = CData.Parts[(int)EPart.OFP].dBtmMoldAvg;
                            }
                            // 2021.08.21 lhs End

                            // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                            //if (CData.GemForm != null) CData.GemForm.Strip_Data_Shift(11, 12);// L/Table PickUp Data -> R Chuck Data 로 이동
                            if (CData.GemForm != null)
                            {
                                CData.GemForm.Strip_Data_Shift((int)EDataShift.OFP_LT_PICK/*11*/, (int)EDataShift.GRR_TBL_CHUCK/*12*/);// L/Table PickUp Data -> R Chuck Data 로 이동
                            }
                            // 2021.07.19 SungTae End
                        }
                        else if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip && CData.dfInfo.sGl != "GL1" && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            //20191029 ghk_dfserver_notuse_df
                            if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                            {//DF 서버 사용시 DF 측정 사용
                                CData.Parts[(int)EPart.GRDR].dDfMin     = CData.Parts[(int)EPart.OFP].dDfMin;
                                CData.Parts[(int)EPart.GRDR].dDfMax     = CData.Parts[(int)EPart.OFP].dDfMax;
                                CData.Parts[(int)EPart.GRDR].dDfAvg     = CData.Parts[(int)EPart.OFP].dDfAvg;
                                CData.Parts[(int)EPart.GRDR].sBcrUse    = CData.Parts[(int)EPart.OFP].sBcrUse;
                            }
                            else
                            {//DF 서버 사용시 DF 측정 안함
                                CData.Parts[(int)EPart.GRDR].dDfMax     = CData.Parts[(int)EPart.OFP].dDfMax;
                            }
                        }

                        if (CDataOption.UseDFDataServer && !CData.Opt.bSecsUse) //211222 pjh : 조건 변경. SECS/GEM과 D/F Server 동시 사용 불가
                        {
                            if (CData.Dev.eMoldSide != ESide.Top) // 1st Btm Grinding과 2nd Btm Grinding이면, 
                            {
                                m_tTarget = CSq_OnP.It.LoadDFServerData(CData.Parts[(int)EPart.OFP].sLotName, CData.Parts[(int)EPart.OFP].sBcr);
                                //if (m_tTarget.dPcbMean == 0 || m_tTarget.dPcbMax == 0 || m_tTarget.dAfAvg == 0 || m_tTarget.dAfMax == 0)
                                if (m_tTarget.dPcbMean == 0 && m_tTarget.dPcbMax == 0 && m_tTarget.dAfAvg == 0 && m_tTarget.dAfMax == 0)//211221 pjh : 조건 변경
                                {
                                    CErr.Show(eErr.DF_SERVER_DATA_FILE_READING_FAIL);

                                    m_iStep = 0;
                                    return true;
                                }

                                _SetLog(">>>>> [DFSERVER] : LoadDFServerData(b2ndBtm, CData.Parts[(int)EPart.ONP].sLotName, CData.Parts[(int)EPart.ONP].sBcr)");

                                CData.Parts[(int)EPart.GRDR].dPcbMin = m_tTarget.dPcbMin;
                                CData.Parts[(int)EPart.GRDR].dPcbMax = m_tTarget.dPcbMax;
                                CData.Parts[(int)EPart.GRDR].dPcbMean = m_tTarget.dPcbMean;

                                //220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                                CData.Parts[(int)EPart.GRDR].dTopMoldMax = m_tTarget.dAfMax;
                                CData.Parts[(int)EPart.GRDR].dTopMoldAvg = m_tTarget.dAfAvg;
                                //
                            }
                        }
                        // 2021.05.31. SungTae End

                        CData.Parts[(int)EPart.GRDR].sLotName   = CData.Parts[(int)EPart.OFP].sLotName;
                        CData.Parts[(int)EPart.GRDR].iMGZ_No    = CData.Parts[(int)EPart.OFP].iMGZ_No;
                        CData.Parts[(int)EPart.GRDR].iSlot_No   = CData.Parts[(int)EPart.OFP].iSlot_No;
                        CData.Parts[(int)EPart.GRDR].sBcr       = CData.Parts[(int)EPart.OFP].sBcr;
                        CData.Parts[(int)EPart.GRDR].dShiftT    = CData.Parts[(int)EPart.OFP].dShiftT; //190529 ksg :
                        CData.Parts[(int)EPart.OFP].sBcr        = ""; //190823 ksg :
                        CData.Parts[(int)EPart.GRDR].bChkGrd    = false; //200624 pjh : Grinding 중 Error Check 변수
                        CData.Parts[(int)EPart.GRDR].LotColor   = CData.Parts[(int)EPart.OFP].LotColor;
                        CData.Parts[(int)EPart.GRDR].nLoadMgzSN = CData.Parts[(int)EPart.OFP].nLoadMgzSN;

                        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        // Data Shift : Off-Picker => R-Table (18 Point 측정 여/부)
                        CData.Parts[(int)EPart.GRDR].b18PMeasure = CData.Parts[(int)EPart.OFP].b18PMeasure; //Data Shift는 조건별 사용 여부에 무관하게 진행 함
                        CData.Parts[(int)EPart.OFP].b18PMeasure = false;
                        //

                        // 2022.08.17 SungTae
                        _SetLog($"---------> [확인용] From : Part[OFP ].ShiftT = {CData.Parts[(int)EPart.OFP].dShiftT}");
                        _SetLog($"---------> [확인용]   To : Part[GRDR].ShiftT = {CData.Parts[(int)EPart.GRDR].dShiftT}");

                        // 200314-mjy : 패키지 유닛일 경우 유닛 데이터 복사
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            Array.Copy(CData.Parts[(int)EPart.OFP].aUnitEx, CData.Parts[(int)EPart.GRDR].aUnitEx, CData.Dev.iUnitCnt);
                            Array.Copy(CData.Parts[(int)EPart.OFP].aUnitEx, CData.GrData[(int)EWay.R].aUnitEx, CData.Dev.iUnitCnt);
                            for (int i = 0; i < CData.Dev.iUnitCnt; i++)
                            {
                                CData.GrData[(int)EWay.R].aUnit[i].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                CData.GrData[(int)EWay.R].aUnit[i].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                            }
                        }

                        CData.Parts[(int)EPart.GRDR].iStat = ESeq.GRR_Grinding;

                        //20200417 jhc : R-Table Place 후 Off-Picker Bottom Clean / Dry Place 후 Wait (Auto-Running && Step Mode에서만)
                        if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            if (CData.Opt.bOfpBtmCleanSkip) {   CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;      }
                            else                            {   CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmClean;  }
                        }
                        else                                {   CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;      }

                        _SetLog("Status change : " + CData.Parts[(int)EPart.OFP].iStat);

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }
        }
#endif

        /// <summary>
        /// 자재 Pick Up 후
        /// </summary>
        /// <returns></returns>
        public bool Cyl_CleanStrip()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            //190213 ksg : Vac 상시 Check
            //bxVacChk = CIO.It.Get_X(eX.OFFP_Vacuum);//X62 offLoad Picker Vac

            if (!Chk_Strip())
            {//자재 없을 경우 에러 (Vacuum 체크)


                //200805 jhc : Clean Cycle 중 Vac. Error 발생 시 => Off-Picker 버큠 OFF, 바텀 클린 워터 오프, Off-Picker 드레인 ON
                if (!CDataOption.Use2004U) //211027 syc : 2004U
                {
                    Func_Vacuum(false); //Vacuum OFF
                    Func_Eject(false);  //Eject  OFF
                }
                CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false); //바텀 클린 워터 오프
                //Z축 UP 위험 가능성 //CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                Func_Drain(true);   //Off-Picker Drain ON
                //

                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                _SetLog("Error : Vacuum fail.");

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

                case 10:    //버큠 확인, 사용자 설정 크리닝 카운터 확인, 버큠 온, 이젝트 오프, 드레인 오프, 바텀 워터 온, X축 위치 확인, Z축 대기 위치 이동
                    {
                        if (CData.Dev.iOffpCleanStrip <= 0)
                        {
                            // 200910 jym : Skip 조건 추가
                            _SetLog("Clean strip skip.  Count : " + CData.Dev.iOffpCleanStrip);

                            m_iStep = 20;
                            return false;
                        }

                        Func_Vacuum(true);
                        Func_Eject(false);
                        Func_Drain(false);
                        CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, true);//Y6F Btm clean Water

                        m_CleanCnt = 0;
                        m_binv = false;

                        m_bReady = (CMot.It.Get_FP(m_iX) == CData.Dev.dOffP_X_ClnStart) && !Func_Picker0();

                        if (!m_bReady)
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                        }

                        _SetLog("Ready.  Count : " + CData.Dev.iOffpCleanStrip);

                        m_iStep++;
                        return false;
                    }

                case 11:    //Z축 대기위치 이동 확인, 피커 0도, X축 크리닝 시작위치 이동
                    {
                        if (!m_bReady && !CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        Func_Picker0();

                        // 2021.07.02 SungTae Start : [추가] 고객사(ASE-KR) 요청으로 Strip Cleaning Mode에 따른 조건 추가
                        if (CData.Opt.iOfpCleaningMode == (int)eCleanMode.BASIC)
                        {
                            if (!m_binv)
                            {
                                // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Pos Option 사용유무 조건 추가
                                if (CData.CurCompany != ECompany.ASE_KR || !CData.Opt.bOfpUseCenterPos)
                                {
                                    CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);
                                    _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnStart);
                                }
                                else
                                {
                                    CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnCenter);
                                    _SetLog("X axis move clean center pos.", CData.Dev.dOffP_X_ClnCenter);
                                }
                                // 2021.03.18 SungTae End
                            }
                            else
                            {
                                CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd);
                                _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnEnd);
                            }

                            m_iStep++;
                        }
                        else
                        {
                            if (m_CleanCnt < CData.Dev.iOffpCleanStrip)
                            {
                                if (CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart) == 0)
                                {
                                    _SetLog("[Cleaning Mode = " + eCleanMode.ROTATION.ToString() + "] X axis move clean start.", CData.Dev.dOffP_X_ClnStart);

                                    m_iStep = 50;
                                }
                            }
                            else
                            {
                                _SetLog("[Cleaning Mode = " + eCleanMode.ROTATION.ToString() + "] Loop Finished.");

                                CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water

                                m_iStep = 20;
                            }
                        }
                        // 2021.07.02 SungTae End

                        return false;
                    }

                case 12:    //피커 0도 확인, X축 크리닝 시작위치 이동 확인, Z축 clnstart - 자재 두께 위치 이동  -> 수정 필요
                    {
                        if (!Func_Picker0()) { return false; }

                        // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Pos Option 사용유무 조건 추가
                        if (CData.CurCompany != ECompany.ASE_KR || !CData.Opt.bOfpUseCenterPos)
                        {
                            if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart)) { return false; }
                        }
                        else
                        {
                            if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnCenter)) { return false; }
                        }
                        // 2021.03.18 SungTae End

                        if (m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd))      { return false; }

                        //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
                        //if (CData.CurCompany == ECompany.ASE_KR)
                        if (CData.CurCompany == ECompany.ASE_KR ||
                            CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                            CData.CurCompany == ECompany.JSCK   ||
                            CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                        {
                            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_StripClnStart);                            // CData.SPos.dOFP_Z_StripClnStart -> CData.Dev.dOffP_Z_StripClnStart
                            _SetLog("Z axis move strip clean start.", CData.Dev.dOffP_Z_StripClnStart);     // CData.SPos.dOFP_Z_StripClnStart -> CData.Dev.dOffP_Z_StripClnStart
                            // 2021.02.27 SungTae End
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_ClnStart - 1);
                            _SetLog("Z axis move clean start - 1.", CData.SPos.dOFP_Z_ClnStart - 1);
                        }

                        m_iStep++;
                        return false;
                    }

                case 13:    //== 반복 시작 지점 == Z축 Clnstart - 자재 두께 위치 이동 확인, X축 크리닝 시작 위치 이동
                    //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
                    if (CData.CurCompany == ECompany.ASE_KR ||
                        CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                        CData.CurCompany == ECompany.JSCK   ||
                        CData.CurCompany == ECompany.SkyWorks)  //220216 pjh : Skyworks 추가
                    {
                        // 2021.02.27 SungTae : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_StripClnStart))
                        {
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_StripClnStart);
                            return false;
                        }
                    }
                    else
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_ClnStart - 1))
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_ClnStart - 1);
                            return false;
                        }
                    }

                    if (!m_binv)
                    {
                        CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);
                        _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnStart);
                    }
                    else
                    {
                        CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd);
                        _SetLog("X axis move clean end.", CData.Dev.dOffP_X_ClnEnd);
                    }

                    m_iStep++;
                    return false;

                case 14:    //X축 크리닝 시작 위치 이동 확인, 딜레이 5000세팅
                    {
                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart))
                        { return false; }
                        if (m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd))
                        { return false; }
                        m_Delay.Set_Delay(5000);
                        _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }

                case 15:    //드라이 바텀 워터 온  확인, X축 크리닝 위치 이동
                    {
                        bool bxBtmWater = CIO.It.Get_X(eX.DRY_ClnBtmFlow);//X6F
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bxBtmWater)
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                _SetLog("Error : DRY Bottom clean water off.");

                                m_iStep = 0;
                                return true;
                            }
                        }

                        if (!bxBtmWater) return false;

                        if (!m_binv)
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dOffP_X_ClnEnd);
                            _SetLog("X axis move clean end(slow).", CData.Dev.dOffP_X_ClnEnd);
                        }
                        else
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dOffP_X_ClnStart);
                            _SetLog("X axis move clean start(slow).", CData.Dev.dOffP_X_ClnStart);
                        }

                        m_iStep++;
                        return false;
                    }

                case 16:    //X축 크리닝 위치 이동 확인, 크리닝 카운트 증가, 크리닝 종료 여부 확인, 바텀 워터 오프, Z축 대기위치 이동
                    {
                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd))
                        { return false; }
                        if (m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart))
                        { return false; }

                        m_CleanCnt++;

                        // 2021.07.19 lhs Start : PM Count OffP Sponge Current Count 증가
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)
                        {
                            CData.tPM.nOffP_Sponge_Check_CurrCnt++;
                            CData.tPM.nOffP_Sponge_Change_CurrCnt++;
                            _SetLog("OffPicker Sponge Current Count : Check Count = " + CData.tPM.nOffP_Sponge_Check_CurrCnt.ToString() +
                                                                   ", Change Count = " + CData.tPM.nOffP_Sponge_Change_CurrCnt.ToString());
                        }
                        // 2021.07.19 lhs End

                        m_binv = !m_binv;
                        if (m_CleanCnt < CData.Dev.iOffpCleanStrip)
                        {
                            _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, CData.Dev.iOffpCleanStrip));

                            m_iStep = 13;
                            return false;
                        }

                        CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
                        CData.dtSpgWOffLastTime = DateTime.Now;

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Loop end.  Z axis move wait.", CData.SPos.dOFP_Z_Wait);


                        // 2021.07.21 lhs Start : PM Count OffP Sponge Current Count, 루프 종료시 저장, SCK VOC
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)
                        {
                            CMnt.It.Save();
                            _SetLog("Maintenance.cfg Save !!!");

                        }
                        // 2021.07.21 lhs End

                        m_iStep++;
                        return false;
                    }

                case 17:    //Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        _SetLog("Z axis move check.");

                        m_iStep = 20;
                        return false;
                    }

                case 20:    // 종료
                    {
                        if (CDataOption.UseOffP_IOIV && CDataOption.UseSeq_IV_DryTable)  // 2022.07.07 lhs
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_IV_DryTable; // Dryer 자재 존재 체크
                        }
                        else
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WaitPlace;
                        }
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFP].iStat.ToString());

                        m_iStep = 0;
                        return true;
                    }

                // 2021.07.02 SungTae Start : (Only use ASE-KR) Rotation Mode에 대한 처리 부분
                case 50:    // 피커 0도 및 X축 클리닝 시작위치 확인. Z축 클리닝 위치 이동.
                    {
                        if (!Func_Picker0()) { return false; }

                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart)) { return false; }

                        if (CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_StripClnStart) == 0)
                        {
                            _SetLog("[Cleaning Mode = " + eCleanMode.ROTATION.ToString() + "] Z axis move strip clean start.", CData.Dev.dOffP_Z_StripClnStart);

                            m_iStep++;
                        }

                        return false;
                    }

                case 51:    // Z축 클리닝 위치 확인. X축 클리닝 End Pos 이동.
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_StripClnStart)) { return false; }

                        if (CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd) == 0)
                        {
                            _SetLog("[Cleaning Mode = " + eCleanMode.ROTATION.ToString() + "] X axis move clean end.", CData.Dev.dOffP_X_ClnEnd);

                            m_CleanCnt++;       // Cleaning 횟수 증가
                            m_iStep++;
                        }

                        return false;
                    }

                case 52:    // X축 클리닝 End Pos 확인. Z축 대기 위치 이동.
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd)) { return false; }

                        _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, CData.Dev.iOffpCleanStrip));

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0)
                        {
                            _SetLog("[Cleaning Mode = " + eCleanMode.ROTATION.ToString() + "] Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                        }

                        return false;
                    }

                case 53:    // Z축 대기 위치 확인.
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) { return false; }

                        _SetLog("[Cleaning Mode = " + eCleanMode.ROTATION.ToString() + "] Z axis move wait check OK.");
                        m_iStep = 11;

                        return false;
                    }
                    // 2021.07.02 SungTae End
            }
        }

        // 2022.03.08 lhs : Spray Nozzle형 Btm Cleaner (Strip) 
        /// <summary>
        /// Sponge형 -> Spray Nozzle형 Btm Cleaner (Strip)
        /// </summary>
        /// <returns></returns>
        public bool Cyl_CleanStrip_SprayNzl()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            // 2022.08.28 lhs Start : Count 선택
            // #1 Count (기본 적용)
            int nDevWaterCnt    = CData.Dev.iOffpCleanStrip;
            int nDevAirCnt      = CData.Dev.iOffpCleanStrip_Air;
            if(m_nStripNzlCycle == 1)   // #2 Count
            {
                nDevWaterCnt    = CData.Dev.iOffpCleanStrip_N2;
                nDevAirCnt      = CData.Dev.iOffpCleanStrip_Air_N2;
            }
            // 2022.08.28 lhs End

            if (!Chk_Strip()) //자재 없을 경우 에러 (Vacuum 체크)
            {
                //200805 jhc : Clean Cycle 중 Vac. Error 발생 시 => Off-Picker 버큠 OFF, 바텀 클린 워터 오프, Off-Picker 드레인 ON
                if (!CDataOption.Use2004U) //211027 syc : 2004U
                {
                    Func_Vacuum(false); //Vacuum OFF
                    Func_Eject(false);  //Eject  OFF
                }
                CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false); //바텀 클린 워터 오프
                //Z축 UP 위험 가능성 //CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                Func_Drain(true);   //Off-Picker Drain ON
                //

                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                _SetLog("Error : Vacuum fail.");

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

                case 10:    // 버큠 확인, 사용자 설정 크리닝 카운터 확인, 버큠 온, 이젝트 오프, 드레인 오프, 바텀 워터 온, X축 위치 확인, Z축 대기위치 이동
                    {
                        if (nDevWaterCnt <= 0) // 200910 jym : Skip 조건 추가
                        {
                            _SetLog("Clean strip skip.  Count : " + nDevWaterCnt);
                            m_iStep = 30;   // 종료 스텝
                            return false;
                        }

                        Func_Vacuum(true);
                        Func_Eject(false);
                        Func_Drain(false);
                        
                        // lhs 나중에 On
                        //CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, true);//Y6F Btm clean Water

                        m_CleanCnt  = 0;
                        m_dPos_X    = CData.Dev.dOffP_X_ClnStart;   // X축 시작위치 : Nozzle 중앙에 위치하도록 설정

                        m_bReady = (CMot.It.Get_FP(m_iX) == m_dPos_X) && !Func_Picker0();  // ???
                        if (!m_bReady)
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                        }
                        _SetLog("Ready.  Count : " + nDevWaterCnt);

                        m_iStep++;
                        return false;
                    }

                case 11:    // Z축 대기위치 이동 확인, 피커 0도, X축 크리닝 위치 이동 (중앙 고정)
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) // Z축 대기위치 이동 확인
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false; 
                        }
                        
                        Func_Picker0();                 // OffP 0도

                        CMot.It.Mv_N(m_iX, m_dPos_X);   // X축 이동
                        _SetLog("X axis move clean start.(Center fixed)", m_dPos_X);

                        m_iStep++;
                        return false;
                    }

                case 12:    // 피커 0도 확인, X축 크리닝 시작위치 이동 확인, Z축 크리닝 위치 이동
                    {
                        if (!Func_Picker0()) 
                        { 
                            return false; 
                        }
                        if (!CMot.It.Get_Mv(m_iX, m_dPos_X))
                        {
                            CMot.It.Mv_N(m_iX, m_dPos_X);   // X축 이동
                            return false;
                        }

                        // Z축 크리닝 위치 이동
                        m_dPos_Z = CData.Dev.dOffP_Z_StripClnStart;
                        CMot.It.Mv_N(m_iZ, m_dPos_Z);
                        _SetLog("Z axis move strip clean position.", m_dPos_Z);

                        m_iStep++;
                        return false;
                    }

                case 13:    // Z축 크리닝 위치 확인, Btm Clean Water On
                    {
                        // Z축 크리닝 위치 확인 
                        if (!CMot.It.Get_Mv(m_iZ, m_dPos_Z))
                        {
                            CMot.It.Mv_N(m_iZ, m_dPos_Z);
                            return false;
                        }
                        _SetLog("Z axis move strip clean position -> Checked", m_dPos_Z);

                        // 워터 온
                        Func_BtmClnFlow(true);      _SetLog("Btm Clean Water On");

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }
                case 14:    // Water On 확인, 노즐 백워드
                    {
                        bool bWater = Func_BtmClnFlow(true);
                        if (m_Delay.Chk_Delay())  // 일정시간 후 에러
                        {
                            if (!bWater)
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                _SetLog("Error : DRY Bottom clean water does not turn on.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bWater) return false;

                        // 노즐 백워드
                        Func_SprayNzlBackward();    _SetLog("Spray Nozzle Backward.");
                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 15:    // 노즐 백워드 확인, 노즐 포워드 ㅁ
                    {
                        bool bBackward = Func_SprayNzlBackward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bBackward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bBackward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms); //_SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }

                        // 노즐 포워드
                        Func_SprayNzlForward();     _SetLog("Spray Nozzle Forward.");
                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 16:    // 노즐 포워드 확인, 워터 크리닝 카운트 증가, 워터 크리닝 종료 여부 확인, 워터 크리닝 종료시 바텀 워터 오프
                    {
                        bool bForward = Func_SprayNzlForward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bForward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bForward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms); //_SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }

                        m_CleanCnt++;   // 워터 크리닝 카운터 증가

                        // 반복
                        if (m_CleanCnt <= nDevWaterCnt)
                        {
                            _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, nDevWaterCnt));

                            // 딜레이 설정
                            m_Delay.Set_Delay(5000); _SetLog("Set delay : 5000ms");

                            m_iStep = 14; // 워터 온, 백워드 스텝
                            return false;
                        }

                        // 워터 오프
                        Func_BtmClnFlow(false);

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000); _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }

                case 17:    // 워터 오프 확인, 에어 클린 스킵 조건 확인, 에어 클린 카운터 초기화 
                    {
                        bool bWater = Func_BtmClnFlow(false);
                        if (m_Delay.Chk_Delay())
                        {
                            if (bWater) // 워터가 꺼지지 않으면
                            {
                                CErr.Show(eErr.DRY_BOTTOM_CLEAN_WATER_ERROR);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (bWater) 
                        {
                            return false;
                        }

                        // Air Blow Skip 조건
                        if (nDevAirCnt <= 0) // Skip 조건 추가
                        {
                            _SetLog("Clean strip skip(Air Blow).   Count : " + nDevAirCnt);
                            m_iStep = 21;   // Z축 대기위치 이동 -> 종료 스텝
                            return false;
                        }

                        // 에어 클린 카운트 초기화
                        m_CleanCnt = 0;  

                        // 에어 온
                        Func_BtmClnAirBlow(true);   _SetLog("Btm Clean AirBlow On");

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }

                case 18:    // AirBlow On 확인, 노즐 백워드
                    {
                        bool bAir = Func_BtmClnAirBlow(true);
                        if (m_Delay.Chk_Delay())  // 일정시간 후 에러
                        {
                            if (!bAir)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                                _SetLog("Error : DRY Bottom clean water does not turn on.");

                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bAir) return false;

                        // 노즐 백워드
                        Func_SprayNzlBackward();    _SetLog("Spray Nozzle Backward.");
                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 19:    // 노즐 백워드 확인, 노즐 포워드 
                    {
                        bool bBackward = Func_SprayNzlBackward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bBackward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bBackward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }

                        // 노즐 포워드
                        Func_SprayNzlForward();     _SetLog("Spray Nozzle Forward.");

                        // 딜레이 설정
                        m_Delay.Set_Delay(5000);    _SetLog("Set delay : 5000ms");
                        // Wait Delay 설정
                        m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");

                        m_iStep++;
                        return false;
                    }

                case 20:    // 노즐 포워드 확인, 에어 클린 카운트 증가, 에러 클린 종료 여부 확인, 워터 크리닝 종료시 바텀 워터 오프
                    {
                        bool bForward = Func_SprayNzlForward();
                        if (m_Delay.Chk_Delay())
                        {
                            if (!bForward)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                                _SetLog("Error : Timeout.");
                                m_iStep = 0;
                                return true;
                            }
                        }
                        if (!bForward)
                        {
                            // Wait Delay 설정
                            m_Wait.Set_Delay(m_nWait_ms); _SetLog("Set wait delay : " + m_nWait_ms.ToString() + "ms");
                            return false;
                        }
                        // Wait
                        if (!m_Wait.Chk_Delay())
                        {
                            return false;
                        }

                        m_CleanCnt++;   // 에어 클린 카운터 증가

                        // 반복
                        if (m_CleanCnt < nDevAirCnt)  // Water와 Air 설정 카운트 구분 필요
                        {
                            _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, nDevAirCnt));

                            // 딜레이 설정
                            m_Delay.Set_Delay(5000); _SetLog("Set delay : 5000ms");

                            m_iStep = 18; // 에어 온, 백워드 스텝
                            return false;
                        }

                        // 워터 오프
                        Func_BtmClnAirBlow(false);

                        // Z축 대기위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 21:    // Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) // Z축 대기위치 이동 확인
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }
                        _SetLog("Z axis move check.");

                        m_iStep = 30;
                        return false;
                    }

                case 30:    // 종료
                    {
                        if (CDataOption.UseOffP_IOIV && CDataOption.UseSeq_IV_DryTable)  // 2022.07.07 lhs
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_IV_DryTable; // Dryer 자재 존재 여부 체크
                        }
                        else
                        {
                            // 2022.07.28 lhs Start : Brush 사용시 Nzl(0) -> Brush -> Nzl(1) -> Dry Place
                            if (CDataOption.UseBrushBtmCleaner) 
                            {
                                if (m_nStripNzlCycle == 0)  {   CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmCleanBrush; }
                                else                        {   CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WaitPlace;     }
                            }
                            // 2022.07.28 lhs End 
                            else                            {   CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WaitPlace;     }
                        }
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFP].iStat.ToString());

                        m_iStep = 0;
                        return true;
                    }
                
            }
        }

        // 2022.07.27 lhs : Brush Btm Clean (Strip)
        /// <summary>
        /// Brush로 자재 Clean
        /// Btm Water Nozzle -> Btm Brush -> Btm Water Nozzle -> Dry Place
        /// </summary>
        /// <returns></returns>
        public bool Cyl_CleanBrush()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            if (!Chk_Strip())   //자재 없을 경우 에러 (Vacuum 체크)
            {
                // Clean Cycle 중 Vacuum Error 발생 시 => Off-Picker 버큠 OFF             
                if (!CDataOption.Use2004U) //211027 syc : 2004U 제외
                {
                    Func_Vacuum(false); //Vacuum OFF
                    Func_Eject(false);  //Eject  OFF
                }

                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                _SetLog("Error : Vacuum fail.");

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

                case 10:    //버큠 확인, 사용자 설정 크리닝 카운터 확인, 버큠 온, 이젝트 오프, 드레인 오프, 바텀 워터 온, X축 위치 확인, Z축 대기 위치 이동
                    {
                        if (CData.Dev.iOffpCleanBrush <= 0)
                        {
                            // 20.09.10 jym : Skip 조건 추가
                            _SetLog("Clean Brush skip.  Count : " + CData.Dev.iOffpCleanBrush);

                            m_iStep = 20;   // 종료
                            return false;
                        }

                        Func_Vacuum(true);
                        Func_Eject(false);

                        m_CleanCnt  = 0;
                        m_binv      = false;

                        m_bReady = (CMot.It.Get_FP(m_iX) == CData.Dev.dOffP_X_ClnStart_Brush) && !Func_Picker0();

                        if (!m_bReady)
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                        }

                        _SetLog("Ready.  Brush Count : " + CData.Dev.iOffpCleanBrush);

                        m_iStep++;
                        return false;
                    }

                case 11:    //Z축 대기위치 이동 확인, 피커 0도, X축 크리닝 시작위치 이동
                    {
                        if (!m_bReady && !CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        Func_Picker0();

                        if (!m_binv)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart_Brush);
                            _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnStart_Brush);

                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd_Brush);
                            _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnEnd_Brush);
                        }

                        m_iStep++;
                        return false;
                    }

                case 12:    //피커 0도 확인, X축 크리닝 시작위치 이동 확인, Z축 clnstart - 자재 두께 위치 이동  -> 수정 필요
                    {
                        if (!Func_Picker0()) { return false; }

                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart_Brush))   { return false; }
                        if (m_binv  && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd_Brush))     { return false; }

                        CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_StripClnStart_Brush);
                        _SetLog("Z axis move strip clean start.", CData.Dev.dOffP_Z_StripClnStart_Brush);

                        m_iStep++;
                        return false;
                    }

                case 13:    //== 반복 시작 지점 == Z축 Clnstart - 자재 두께 위치 이동 확인, X축 크리닝 시작 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_StripClnStart_Brush))
                        {
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_StripClnStart_Brush);
                            return false;
                        }

                        if (!m_binv)
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart_Brush);
                            _SetLog("X axis move clean start.", CData.Dev.dOffP_X_ClnStart_Brush);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnEnd_Brush);
                            _SetLog("X axis move clean end.", CData.Dev.dOffP_X_ClnEnd_Brush);
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:    //X축 크리닝 시작 위치 이동 확인, 딜레이 5000세팅
                    {
                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart_Brush))   { return false; }
                        if ( m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd_Brush))     { return false; }

                        m_Delay.Set_Delay(5000);
                        _SetLog("Set delay : 5000ms");

                        m_iStep++;
                        return false;
                    }

                case 15:    //X축 크리닝 위치 이동
                    {
                        if (!m_binv)
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dOffP_X_ClnEnd_Brush);
                            _SetLog("X axis move clean end(slow).", CData.Dev.dOffP_X_ClnEnd_Brush);
                        }
                        else
                        {
                            CMot.It.Mv_S(m_iX, CData.Dev.dOffP_X_ClnStart_Brush);
                            _SetLog("X axis move clean start(slow).", CData.Dev.dOffP_X_ClnStart_Brush);
                        }

                        m_iStep++;
                        return false;
                    }

                case 16:    //X축 크리닝 위치 이동 확인, 크리닝 카운트 증가, 크리닝 종료 여부 확인, 바텀 워터 오프, Z축 대기위치 이동
                    {
                        if (!m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnEnd_Brush))     { return false; }
                        if ( m_binv && !CMot.It.Get_Mv(m_iX, CData.Dev.dOffP_X_ClnStart_Brush))   { return false; }

                        m_CleanCnt++;

                        m_binv = !m_binv;
                        
                        if (m_CleanCnt < CData.Dev.iOffpCleanBrush)
                        {
                            _SetLog(string.Format("Loop.  Count : {0} / {1}", m_CleanCnt, CData.Dev.iOffpCleanBrush));

                            m_iStep = 13;
                            return false;
                        }

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Loop end.  Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 17:    //Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        _SetLog("Z axis move check.");

                        m_iStep = 20;   // 종료
                        return false;
                    }

                case 20:    // 사이클 종료
                    {
                        // Spray Nozzle 사용시 Nzl(0) -> Brush -> Nzl(1) -> Dry Place
                        if (CDataOption.UseSprayBtmCleaner)
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmCleanStrip;  // 다시 Btm Spray Nozzle 시퀀스로
                            CSq_OfP.It.m_nStripNzlCycle = 1; // Spray Nozzle의 #2(두번째) Count값으로 동작 : 2022.07.28 lhs
                        }
                        else
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WaitPlace;
                        }
                        
                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFP].iStat.ToString());

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        /// <summary>
        /// Off-Picker에서 Dryer에 자재 Place하는 사이클
        /// 2022.06.23 lhs : Cyl_Place에 Cyl_Place_IV2 병합
        /// </summary>
        /// <returns></returns>
        public bool Cyl_Place()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT * 4);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_PLACE_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;


            // 210910 syc : 2004U : 케리어 pick -> Vac, Unit Pick -> Gripper
            if (CDataOption.Use2004U)
            {
                bxVacChk = CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) && // 그리퍼 Close
                          !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open)  && // 그리퍼 Open 되어있으면 안됨
                           CIO.It.Get_X(eX.OFFP_Vacuum)              && // 케리어 Vac 센서 감지되어있어야함
                           CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect); // 케리어 디텍트 센서 감지 되어있어야함
            }
            else
            {
                //190213 ksg : Vac 상시 Check
                bxVacChk  = CIO.It.Get_X(eX.OFFP_Vacuum);       // X62 offLoad Picker Vac
                bxVacFree = CIO.It.Get_X(eX.OFFP_VacuumFree);   // X7B offLoad Picker 진공해제 확인
            }

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10:    //버큠 온 확인, 이젝트 오프, 드레인 오프, 드라이 워터 오프, Z축 대기 위치 이동
                    {
                        m_tStTime = DateTime.Now;
                        
                        if (!bxVacChk)
                        {
                            if (CDataOption.Use2004U)  //210916 syc : 2004U
                            {
                                ShowVacChkErr_Place_2004U();  // 2022.08.10 lhs : 에러 메시지 세분화
                            }
                            else
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                                _SetLog("Error : Vacuum fail.");
                            }

                            m_iStep = 0;
                            return true;
                        }

                        if (CMot.It.Get_CP((int)EAx.DryZone_Z) != CData.SPos.dDRY_Z_Up)
                        {
                            CErr.Show(eErr.DRY_Z_NOT_UP_POSITION);
                            _SetLog("Error : DRY Z axis not up position.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_Vacuum(true); _SetLog("Vacuum On");
                        Func_Eject(false); _SetLog("Eject Off");
                        Func_Drain(false); _SetLog("Drain Off");
                        CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water

                        //210910 syc : 2004U : 케리어 pick -> Vac, Unit Pick -> Gripper
                        if (CDataOption.Use2004U)
                        {
                            Func_CarrierClampClose();  //2004U면 위의 피커 Vac은 커버를, Calmp는 케리어를 잡고 있다.
                        }
                        else
                        {   
                            if (CDataOption.eDryClamp == EDryClamp.Cylinder)   // 2022.07.02 lhs 
                            {
                                CSq_Dry.It.Func_DryClampOpen(true); // Dryer UnClamp
                                _SetLog("Dryer Clamp Unclamp");
                            }
                        }

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0)
                        {
                            _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                            m_iStep++;
                        }
                        return false;
                    }

                case 11:    //Z축 대기 위치 이동 확인, 피커 90도, X축 place 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }

                        if (CDataOption.eDryClamp == EDryClamp.Cylinder)   // 2022.07.02 lhs 
                        {
                            if(!CSq_Dry.It.Func_DryClampOpen(true)) // Dryer UnClamp 확인
                            {
                                return false;
							}
                            _SetLog("Dryer Clamp Unclamped OK");
                        }

                        // 200417 jym : SG2000U는 피커 사이즈가 커서 드라이존 가서 턴해야함
                        if (CDataOption.Package == ePkg.Strip)
                        {
                            Func_Picker90();
                        }
                        
                        if(CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Place) == 0)
                        {
							_SetLog("X axis move place.", CData.SPos.dOFP_X_Place);
                            m_iStep++;
						}

                        return false;
                    }

                case 12:    //피커 90도 확인, X축 place 위치 이동 확인, Z축 place - slow 위치 이동
                    {
                        if (CDataOption.Package == ePkg.Strip)
                        { 
                            if (!Func_Picker90()) return false; 
                        }

                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Place))
                        { return false; }

                        // 200417 jym : SG2000U는 피커 사이즈가 커서 드라이존 가서 턴해야함
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            if (!Func_Picker90()) return false;
                        }
                        
                        if (!bxVacChk)
                        {
                            if (CDataOption.Use2004U) //210916 syc : 2004U
                            {
                                ShowVacChkErr_Place_2004U();  // 2022.08.10 lhs : 에러 메시지 세분화
                            }
                            else
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                                _SetLog("Error : Vacuum fail.");
                            }

                            m_iStep = 0;
                            return true;
                        }

                        m_Delay.Set_Delay(500);
                        _SetLog("Set delay : 500ms");

                        m_iStep++;
                        return false;
                    }

                case 13: // 일정시간 대기, Z축 place - slow 위치 이동
                    {
                        if (!m_Delay.Chk_Delay())
                        { return false; }

                        if (!bxVacChk)
                        {
                            if (CDataOption.Use2004U)   //210916 syc : 2004U
                            {
                                ShowVacChkErr_Place_2004U();  // 2022.08.10 lhs : 에러 메시지 세분화
                            }
                            else
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                                _SetLog("Error : Vacuum fail.");
                            }

                            m_iStep = 0;
                            return true;
                        }

                        if (CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_Place - CData.Dev.dOffP_Z_Slow) == 0)
                        {
                            _SetLog("Z axis move position.", CData.Dev.dOffP_Z_Place - CData.Dev.dOffP_Z_Slow);
                            m_iStep++;
                        }

                        return false;
                    }

                case 14:    // Z축 place - slow 위치 이동 확인, Z축 place 위치 이동(슬로우)
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place - CData.Dev.dOffP_Z_Slow))
                        { return false; }

                        if (!bxVacChk)
                        {
                            if (CDataOption.Use2004U)  //210916 syc : 2004U
                            {
                                ShowVacChkErr_Place_2004U();  // 2022.08.10 lhs : 에러 메시지 세분화
                            }
                            else
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                                _SetLog("Error : Vacuum fail.");
                            }

                            m_iStep = 0;
                            return true;
                        }

                        if (CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Place) == 0)
                        {
                            _SetLog("Z axis move place(slow).", CData.Dev.dOffP_Z_Place);
                            m_iStep++;
                        }
                        return false;
                    }

                case 15:    //Z축 place 위치 이동(슬로우) 확인, 버큠 오프, 이젝트 온
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place))
                        { return false; }

                        if (CDataOption.Use2004U)
                        {
                            Func_CarrierClampOpen(); // 4U에서는 클램프로 케리어 픽함
                        }

						Func_Vacuum(false); _SetLog("Vacuum Off");
						Func_Eject(true);   _SetLog("Eject On");

						m_Delay.Set_Delay(CData.Dev.iEjectDelayOffP);  // Eject On 유지시간 <- 초기값 2000ms, (SCK+) Device에서 Delay 설정
                        _SetLog("Set Ejector On Keep Delay : " + CData.Dev.iEjectDelayOffP + "ms");

                        m_iStep++;
                        return false;
                    }

                case 16:    //2초 딜레이, 이젝트 오프, Picker Z & Dry Z 같은 속도로 같은 거리 만큼 다운(다운 gap = offp_Z_PlaceDn)
                    {
                        if (!m_Delay.Chk_Delay()) // Eject 유지 시간 기다리기
                        { return false; }

                        // 2022.07.22 lhs Start : Eject Off 시점을 경우에 따라 다르게...
                        if      (CDataOption.Use2004U)          {   Func_Eject(false);  _SetLog("Eject Off");   }
                        else if (CDataOption.UseOffPVacuumFree) {   Func_Eject(false);  _SetLog("Eject Off");   }
                        else                                    {   /*Func_Eject(false); 여기서는 Eject Off 하지 않음*/ }
                        // 2022.07.22 lhs End

                        if (CDataOption.Use2004U) // 210910 syc : 2004U 에서는 드라이존과 피커가 같이 내려갈 필요성 없음.
                        {
                            m_iStep++;
                            return false;
                        }

                        m_Delay.Set_Delay(2000);    _SetLog("Set Wait Delay : 2000ms");

						if (CDataOption.UseOffPVacuumFree)
						{
							m_iStep = 40;  // 40: Vacuum Free 확인하는 시퀀스로
							return false;
						}

                        if (CDataOption.eDryClamp == EDryClamp.Cylinder)   // 2022.07.02 lhs 
                        {
                            CSq_Dry.It.Func_DryClampOpen(false);  // Dryer Clamp
                            _SetLog("Dryer Clamp");
                        }

                        m_iStep = 50;  // 50: OffP & Dryer 천천히 하강
                        return false;
                    }

                // 2022.06.08 lhs Start : ReCheck 시퀀스
                #region Vacuum Free 시퀀스
                case 40:  // Eject Off 했기 때문에 잠시 기다려 진공해제 확인
                    {
                        if (!m_Delay.Chk_Delay()) // Delay 만큼 기다린 후
                        { return false; }

                        m_Delay.Set_Delay(5000);  // 진공해제 확인이 안되었을 경우 최대 기다리는 시간
                        _SetLog("Set Vacuum Free Max Delay : 5000ms");

                        m_iStep++;
                        return false;
                    }
                case 41: // 진공해제 여부 확인 : 다음 스텝 진행 or 에러
                    {
                        if (m_Delay.Chk_Delay())   // 일정시간 이상 진공해제가 안되면
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_VACUUM_FREE_ERROR); // 에러 코드 변경 필요
                            _SetLog("Error : Vacuum Free Error.");

                            m_iStep = 0;
                            return true;
                        }

                        bxVacFree = CIO.It.Get_X(eX.OFFP_VacuumFree);  // X7B : 진공해제 확인용
                        if (!bxVacFree)
                        {
                            return false;
                        }

                        Func_Eject(true); // Eject를 추가적으로 하면 좋지 않을까....

                        if (CDataOption.eDryClamp == EDryClamp.Cylinder)   // 2022.07.02 lhs 
                        {
                            CSq_Dry.It.Func_DryClampOpen(false);  // Dryer Clamp
                            _SetLog("Dryer Clamp");
                        }

                        // Delay 없이 진행
                        m_iStep = 50;  // 50: OffP & Dryer 천천히 하강
                        return false;
					}
                #endregion

                #region Vacuum On 시퀀스 (SCK+)
                case 45: // Z축 일정 위치로 UP 
                    {
                        // OffPicker Z축 일정 위치로 UP
                        if (CMot.It.Mv_S(m_iZ, 30.0) != 0)
                        {
                            return false;
                        }
                        _SetLog("Z axis move (slow).", 30.0);                     

                        m_iStep++;
                        return false;
                    }
                case 46: // Z축 일정 위치로 UP 확인. Eject Off, Set Delay
                    {
                        if (!CMot.It.Get_Mv(m_iZ, 30.0))
                        {
                            return false;
                        }

                        Func_Eject(false);          _SetLog("Eject Off");
                        m_Delay.Set_Delay(2000);    _SetLog("Set Wait Delay : 2000ms");

                        m_iStep++;
                        return false;
                    }
                case 47: // 잠시 기다린 후, 진공형성
                    {
                        if (!m_Delay.Chk_Delay()) // Delay 만큼 기다린 후
                        { return false; }

                        Func_Vacuum(true); _SetLog("Vacuum On");    // ReCheck를 위한 Vacuum On

                        m_Delay.Set_Delay(CData.Dev.iPickDelayOff); // Pick에 사용한 Delay를 사용하자
                        _SetLog("Set delay : " + CData.Dev.iPickDelayOff + "ms");

                        m_iStep++;
                        return false;
                    }
                case 48: // 진공형성을 기다린 후 자재체크(신호확인), 진공형성이면 에러...그렇지 않으면 다음 스텝 진행
                    {
                        if (!m_Delay.Chk_Delay()) // Delay 만큼 기다린 후
                        { return false; }

                        if (Chk_Strip())
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR); // ReCheck시 OffPikcer에 붙어 있는 자재
                            _SetLog("Error : Detect strip.");
                            Func_Vacuum(true); _SetLog("Error -> Vacuum On");   // 에러시 자재 낙하 방지용

                            m_iStep = 0;
                            return true;
                        }
                        else
                        {
                            Func_Vacuum(false); _SetLog("Vacuum Off");
							
                            m_iStep = 18;  // 18 :  Spring 타입은 Dry Z축 Level Check 위치로 이동, OffP Z축 Wait Pos로 이동
                            return false;
						}
					}
                #endregion
                // 2022.06.08 lhs End : ReCheck 시퀀스

                case 50:  // OffP & Dryer 천천히 하강
                    {
                        if (CDataOption.eDryClamp == EDryClamp.Cylinder)   // 2022.07.02 lhs 
                        {
                            if (!CSq_Dry.It.Func_DryClampOpen(false)) // Dryer Clamp 확인
                            {
                                return false;
                            }
                            _SetLog("Dryer Clamp Clamped OK");
                        }

                        // 2022.07.02 lhs Start
                        // Spring   타입은 Off-Picker와 Dryer가 같이 하강해야 함.
                        // Cylinder 타입은 Off-Picker는 하강하면 안됨 : 기구적으로 충돌 발생
                        if (CDataOption.eDryClamp == EDryClamp.None)
                        {
                            if (CMot.It.Mv_S(m_iZ, (CData.Dev.dOffP_Z_Place + CData.Dev.dOffP_Z_PlaceDn)) != 0)  // Off-Picker 하강
                            {
                                return false;
                            }
                            _SetLog("Z axis move position(slow).", (CData.Dev.dOffP_Z_Place + CData.Dev.dOffP_Z_PlaceDn));
                        }
                        // 2022.07.02 lhs End

                        // Dryer 하강
                        if (CMot.It.Mv_S(m_iDYZ, (CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn)) != 0)
                        {
                            return false;
                        }
                        _SetLog("DRY Z axis move position(slow).", (CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn));

                        m_iStep = 17; // 기존 로직
                        return false;
                    }

                case 17:    //picker Z place + placeDn 위치 이동 확인, dry Z축 up - z_placeDn 위치 이동 확인, Z축 대기 위치 이동, Dry Z축 센서 체크 위치 이동
                    {
                        if (CDataOption.Use2004U) //210910 syc : 2004U : 케리어 pick -> Vac, Unit Pick -> Gripper
                        {
                            // 냉무
                        }
                        else
                        {
                            if (CDataOption.eDryClamp == EDryClamp.None)   // 2022.07.02 lhs : Spring type 동작
                            {
                                if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOffP_Z_Place + CData.Dev.dOffP_Z_PlaceDn)))   // Off-Picker Z 위치 확인
                                { return false; }
                            }
                            // Dryer 하강 확인
                            if (!CMot.It.Get_Mv(m_iDYZ, (CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn)))    // Dryer  Z 위치 확인    
                            { return false; }

                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)   // 2022.07.25 lhs : ReCheck : Spring/Cylinder 타입 모두
                            {
                                Func_Eject(false); _SetLog("Eject Off");
                                //m_Delay.Set_Delay(2000); _SetLog("Set Wait Delay : 2000ms");

                                m_iStep = 45;  // Vacuum On Recheck 하는 시퀀스
                                return false;
                            }
                        }

                        m_iStep++;
                        return false;
                    }

                case 18:    // Spring 타입은 Dry Z축 Level Check 위치로 이동, OffP Z축 Wait Pos로 이동
                    { 
                        if (CDataOption.Use2004U) //210910 syc : 2004U : 케리어 pick -> Vac, Unit Pick -> Gripper
                        {
                            // 냉무
                        }
                        else
                        {
                            if (CDataOption.UseDryWtNozzle || CDataOption.UseOffP_IOIV)
                            {
                                // Dryer는 촬영을 위해 현 위치 유지    
                                // IV2로 Clamp를 확인하므로 Level Safety Check 안함
                            }
                            else
                            {
                                if (CDataOption.eDryClamp == EDryClamp.None)
                                {
                                    CMot.It.Mv_S(m_iDYZ, CData.SPos.dDRY_Z_Check);
                                    _SetLog("DRY Z axis move check(slow).", CData.SPos.dDRY_Z_Check);
                                }
                            }
                        }                        

						// OffPicker Z축 대기 위치로 이동
						if (CMot.It.Mv_S(m_iZ, CData.SPos.dOFP_Z_Wait) != 0)
                        {
                            return false;
                        }
                        _SetLog("Z axis move wait(slow).", CData.SPos.dOFP_Z_Wait);

                        Func_Eject(false); _SetLog("Eject Off");

                        m_iStep++;
						return false;
					}

				case 19:    //Z축 대기위치 이동 확인, 드라이 Z축 센서 체크 위치 이동 확인, 버큠 오프, 이젝트 오프, 
                    {
                        // 2021.12.15 SungTae Start : [수정] (ASE-KR VOC) Dry Zone에 Strip 유무 확인용 Flag가 True일 때까지 Waiting
                        if (CData.CurCompany == ECompany.ASE_KR && CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait) && !CData.bDryStripExist)
                            {
                                return false;
                            }
                        }
                        // 2021.12.15 SungTae End
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) { return false; }
                        }

                        // Dryer 센서 Check 위치로 이동 확인
                        if      (CDataOption.Use2004U)          {   /* Dryer Check 위치 확인 없음. */   }
                        else if (CDataOption.UseDryWtNozzle ||
                                 CDataOption.UseOffP_IOIV)      {   /* Dryer Check 위치 확인 없음. */   } // Dryer : Water Nozzle로 Modify
                        else
                        {
                            if (CDataOption.eDryClamp == EDryClamp.None) // 2022.07.02 lhs : 기존 Spring 타입만 
                            {
                                if (!CMot.It.Get_Mv(m_iDYZ, CData.SPos.dDRY_Z_Check))
                                { return false; }
                            }
                        }

                        Func_Eject(false); _SetLog("Eject Off");

                        if (Chk_Strip())
                        {
                            //set Error : 알수 없는 자재 
                            CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error : Detect strip.");
                            Func_Vacuum(true); _SetLog("Error -> Vacuum On");   // 에러시 자재 낙하 방지용

                            m_iStep = 0;
                            return true;
                        }

                        // OFP -> DRY 데이터 전달
                        CData.Parts[(int)EPart.DRY].sLotName = CData.Parts[(int)EPart.OFP].sLotName;
                        CData.Parts[(int)EPart.DRY].iMGZ_No = CData.Parts[(int)EPart.OFP].iMGZ_No;
                        CData.Parts[(int)EPart.DRY].iSlot_No = CData.Parts[(int)EPart.OFP].iSlot_No;
                        CData.Parts[(int)EPart.DRY].sBcr = CData.Parts[(int)EPart.OFP].sBcr;
                        CData.Parts[(int)EPart.OFP].sBcr = "";

                        //20190222 ghk_dynamicfunction
                        Array.Copy(CData.Parts[(int)EPart.OFP].dPcb, CData.Parts[(int)EPart.DRY].dPcb, CData.Parts[(int)EPart.OFP].dPcb.Length);

                        CData.Parts[(int)EPart.DRY].dPcbMax = CData.Parts[(int)EPart.OFP].dPcbMax;
                        CData.Parts[(int)EPart.DRY].dPcbMin = CData.Parts[(int)EPart.OFP].dPcbMin;
                        CData.Parts[(int)EPart.DRY].dPcbMean = CData.Parts[(int)EPart.OFP].dPcbMean;
                        CData.Parts[(int)EPart.DRY].LotColor = CData.Parts[(int)EPart.OFP].LotColor;
                        CData.Parts[(int)EPart.DRY].nLoadMgzSN = CData.Parts[(int)EPart.OFP].nLoadMgzSN;

                        // 2022.05.04 SungTae Start : [추가]
                        if (CData.CurCompany == ECompany.ASE_K12)
                        {
                            CData.Parts[(int)EPart.DRY].sMGZ_ID = CData.Parts[(int)EPart.OFP].sMGZ_ID;
                        }
                        // 2022.05.04 SungTae End

                        // 2022.05.04 SungTae : [확인용]
                        _SetLog($"LoadMgzSN : OFP({CData.Parts[(int)EPart.OFP].nLoadMgzSN}) -> DRY({CData.Parts[(int)EPart.DRY].nLoadMgzSN})");

                        // 2021.08.21 lhs Start : Top/Btm Mold 데이터 전달
                        if (CDataOption.UseNewSckGrindProc)
                        {
                            CData.Parts[(int)EPart.DRY].dTopMoldMax = CData.Parts[(int)EPart.OFP].dTopMoldMax;
                            CData.Parts[(int)EPart.DRY].dTopMoldAvg = CData.Parts[(int)EPart.OFP].dTopMoldAvg;
                            CData.Parts[(int)EPart.DRY].dBtmMoldMax = CData.Parts[(int)EPart.OFP].dBtmMoldMax;
                            CData.Parts[(int)EPart.DRY].dBtmMoldAvg = CData.Parts[(int)EPart.OFP].dBtmMoldAvg;
                        }
                        // 2021.08.21 lhs End

                        //ksg Strip 유무 변경
                        CData.Parts[(int)EPart.DRY].bExistStrip = CData.Parts[(int)EPart.OFP].bExistStrip;
                        CData.Parts[(int)EPart.OFP].bExistStrip = false;

                        // 200314-mjy : 패키지 유닛일 경우 유닛 데이터 복사
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            Array.Copy(CData.Parts[(int)EPart.OFP].aUnitEx, CData.Parts[(int)EPart.DRY].aUnitEx, CData.Dev.iUnitCnt);
                        }

                        if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))
                        {
                            CData.GemForm.Strip_Data_Shift((int)EDataShift.OFP_RT_PICK/*17*/, (int)EDataShift.DRY_WORK/*21*/);// PickUp Data  -> Dry Data 로 이동
                        }

                        if      (CDataOption.Use2004U)          { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitIV2;     }   // 210927 syc : 2004U Place 이후 Check sensor가 아닌 OFP의 Unit Check 이후 Level check 해야함                        
                        else if (CDataOption.UseDryWtNozzle)    { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitIV2;     }   // Dryer는 촬영을 위해 현 위치 유지
                        else if (CDataOption.UseOffP_IOIV)      { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitIV2;     }   // Dryer는 촬영을 위해 현 위치 유지 : 2022.07.01 lhs
                        else                                    { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_CheckSensor; }   // 기존 시퀀스 진행

                        //20200417 jhc : R-Table Place 후 Off-Picker Bottom Clean / Dry Place 후 Wait (Auto-Running && Step Mode에서만)
                        if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                        {
                            if (CDataOption.UseDryWtNozzle || CDataOption.UseOffP_IOIV)  // 2022.06.30 lhs : UseOffP_IOIV 추가 
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_IV_DryClamp;    // Dryer에 Place만 하고 IV2(화상판별센서) 시퀀스로 진행하도록 // Dryer Tip을 IV2로 촬영 및 판정
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                                // 2021.02.01 lhs Start : 현 시점에서 상태 확인
                                _SetLog("Pick Improve Use, Dual Mode Status : " +
                                           CData.Parts[(int)EPart.ONL].iStat +
                                           ", " + CData.Parts[(int)EPart.INR].iStat +
                                           ", " + CData.Parts[(int)EPart.ONP].iStat +
                                           ", " + CData.Parts[(int)EPart.GRDL].iStat +
                                           ", " + CData.Parts[(int)EPart.GRDR].iStat +
                                           ", GRDR Exist Strip = " + CData.Parts[(int)EPart.GRDR].bExistStrip.ToString()
                                           );
                                // 2021.02.01 lhs End

                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd || CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Wait || CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready) &&    // 2021.02.01 lhs GRR_Ready 추가 
                                    (CData.Parts[(int)EPart.GRDR].bExistStrip == false))) //20200420 jhc : 종료 조건 추가
                                {
                                    _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);

                                    m_iStep++; //Off-Picker Wait Pos. 이동 후 종료
                                    return false;
                                }
                            }
                        } // end : if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual))
                        else
                        {
                            if (CDataOption.Use2004U)
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_IV2; // 2004U는 Place 이후 IV2 검사 진행해야함
                            }   
                            else if (CDataOption.UseDryWtNozzle || CDataOption.UseOffP_IOIV)  // 2022.06.30 lhs : UseOffP_IOIV 추가
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_IV_DryClamp; // Dryer에 Place만 하고 IV2(화상판별센서) 시퀀스로 진행하도록 // Dryer Tip을 IV2로 촬영 및 판정
                            }   
                            else
                            {
                                if (CData.Opt.bOfpBtmCleanSkip)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                                    if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                        CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                        CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                        CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                        CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                                    {
                                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);

                                        m_iStep++; //Off-Picker Wait Pos. 이동 후 종료
                                        return false;
                                    }
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmClean;
                                }
                            }
						}

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) 
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power))   {   CIO.It.Set_Y(eY.IOZT_Power, true);  }    // SCK, JSCK
                            CData.Opt.bIOZT_ManualClick = false;    // 2021.09.15 lhs 수동클릭 아님
                        }

                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }

                //20200417 jhc : Dry Place 후 WORK_END 시 -> Wait Pos. 이동 시퀀스 추가 (Auto-Running && Step Mode에서만)
                case 20: //Off-Picker 회전
                    {
                        // 200327 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.JCET) // 2020.11.18 lhs  JCET 추가
                        {
                            Func_Picker0();
                            _SetLog("Picker 0.");
                        }
                        else
                        {
                            Func_Picker90();
                            _SetLog("Picker 90.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 21: //Off-Picker 회전 체크, X축 Wait Pos로 이동
                    {
                        // 200327 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.JCET) // 2020.11.18 lhs  JCET 추가
                        { if (!Func_Picker0()) return false; }
                        else
                        { if (!Func_Picker90()) return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 22:  // X축 Wait Pos로 이동 확인, OFP WorkEnd
                    {
                        if(!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Wait))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                            return false;
                        }

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);    // SCK, JSCK
                            CData.Opt.bIOZT_ManualClick = false;                                    // 2021.09.15 lhs 수동클릭 아님
                        }

                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd; //Off-Picker WORK_END

                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            }
        }


        // 2022.07.07 lhs Start
        /// <summary>
        /// IV(화상판별센서)로 Dryer Table을 촬영 및 판정
        /// Off-Picker에 달린 IV를 이용하여 Dryer에 Place하기 전에 Table을 촬영하여 Strip이 있는지를 체크
        /// </summary>
        /// <returns></returns>
        public bool Cyl_IV_DryTable()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CHECK_IV2_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;

            bxVacChk    = CIO.It.Get_X(eX.OFFP_Vacuum);       // X62 offLoad Picker Vac
            bxVacFree   = CIO.It.Get_X(eX.OFFP_VacuumFree);   // X7B offLoad Picker 진공해제 확인

            #region 시퀀스 설명
            //---------------------
            // CleanStrip 이후 Dryer에 Place하기 전에 Dryer에 자재가 있는지 체크하는 사이클
            //---------------------
            // 10 : 버큠 온 확인, 이젝트 오프, 드레인 오프, 드라이 워터 오프, Z축 촬영 위치로 이동, Z축 촬영 위치로 이동
            // 11 : Z축 촬영 위치로 이동 확인
            // 12 : 0도로 회전
            // 13 : X축 #1 촬영 위치로 이동
            // 14 : X축 #1 촬영 위치로 이동 확인
            // 15 : IV2 Busy 여부 확인, 트리거 아웃하여 촬영
            // 16 : IV2 Busy 여부 확인, 판정 확인, 판정이 NG일 경우 3회 촬영 반복 (case 15) -> NG일 경우는 에러창 띄우기
            // 17 : Z축 대기위치 이동
            // 18 : Z축 대기위치 이동 확인
            // 20 : 종료    ESeq 변경 : OFP_WaitPlace
            // 
            #endregion

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: // 10 : 버큠 온 확인, 이젝트 오프, 드레인 오프, 드라이 워터 오프, Z축 촬영 위치로 이동
                    {
                        m_tStTime = DateTime.Now;

                        // 진공 확인
                        if (!bxVacChk)
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_VACUUM_CHECK_ERROR);
                            _SetLog("Error : Vacuum fail.");

                            m_iStep = 0;
                            return true;
                        }

                        // Dryer 위치 확인
                        if (CMot.It.Get_CP((int)EAx.DryZone_Z) != CData.SPos.dDRY_Z_Up)
                        {
                            CErr.Show(eErr.DRY_Z_NOT_UP_POSITION);
                            _SetLog("Error : DRY Z axis not up position.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_Vacuum(true);
                        Func_Eject(false);
                        Func_Drain(false);
                        Func_BtmClnFlow(false);

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Picture) != 0)
                        {
                            return false;
                        }
                        _SetLog("Z axis move Picture Positon", CData.SPos.dOFP_Z_Picture);

                        m_iStep++;
                        return false;
                    }

                case 11: // 11 : Z축 촬영 위치로 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Picture))
                        {
                            return false;
                        }
                        _SetLog("Z axis move Picture Positon OK", CData.SPos.dOFP_Z_Picture);

                        m_iStep++;
                        return false;
                    }

                case 12: // 12 : 0도로 회전
                    {
                        if (!Func_Picker0())
                        {
                            return false;
                        }
                        _SetLog("OffPicker Rotate 0 : Ok");

                        m_iStep++;
                        return false;
                    }

                case 13: // 13 : X축 #3 촬영 위치로 이동 (#1 위치와 동일)
                    {
                        if (CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Picture1) != 0)
                        {
                            return false;
                        }
                        _SetLog("X axis move Picture #3(#1) Position", CData.SPos.dOFP_X_Picture1);

                        // 촬영#3를 위한 IV Program P002 선택
                        Func_SetIVProgram(2);   // P002

                        m_iStep++;
                        return false;
                    }

                case 14: // 14 : X축 #3 촬영 위치로 이동 확인 (#1 위치와 동일)
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Picture1))
                        {
                            return false;
                        }
                        _SetLog("X axis move Picture #3(#1) Position OK", CData.SPos.dOFP_X_Picture1);

                        m_nRetryCnt = 0;

                        m_iStep++;
                        return false;
                    }

                case 15: // 15 : IV2 Busy 여부 확인, 트리거 아웃하여 촬영
                    {
                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_ERROR);
                            _SetLog("Error : IV2 is Error");

                            m_iStep = 0;
                            return true;
                        }

                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))
                        {
                            _SetLog("IV2 is busy");
                            return false;
                        }

                        // IV2의 외부 트리거
                        // 사양은 ON : Min 100us, OFF : Min 1.2ms
                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, true);
                        _SetLog("IV2 Trigger Out = ON");

                        System.Threading.Thread.Sleep(100);

                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, false);
                        _SetLog("IV2 Trigger Out = OFF");

                        m_iStep++;
                        return false;
                    }

                case 16: // 16 : IV2 Busy 여부 확인, 판정 확인, 판정이 NG일 경우 2회 더 시도 (case 15) -> NG일 경우는 에러창 띄우기
                    {
                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_ERROR);
                            _SetLog("Error : IV2 is Error");

                            m_iStep = 0;
                            return true;
                        }

                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))
                        {
                            _SetLog("IV2 is busy");
                            return false;
                        }

                        // OK가 아니면 2회 더 시도
                        if (!CIO.It.Get_X(eX.OFFP_IV2_AllOK))
                        {
                            _SetLog("IV2 is not AllOK");

                            if (m_nRetryCnt >= 2)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_NG);
                                _SetLog("Error : IV2 is NG");

                                m_iStep = 0;
                                return true;
                            }
                            m_nRetryCnt++;
                            _SetLog(string.Format("Retry Count={0}", m_nRetryCnt));

                            m_iStep = 15;
                            return false;
                        }

                        m_iStep++;
                        return false;
                    }

				case 17:  // 17 : Z축 대기위치 이동
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) != 0)
                        {
                            return false;
                        }
                        _SetLog("Loop end.  Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 18:  // 18 : Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false; 
                        }
                        _SetLog("Z axis move check.");

                        m_iStep = 20; // 종료
                        return false;
                    }


                case 20: // 20 : 종료    ESeq 변경 : OFP_WaitPlace
                    {
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WaitPlace;
                        _SetLog("Status change : " + CData.Parts[(int)EPart.OFP].iStat);

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }

            }//end : switch (m_iStep)
        }//end : Cyl_IV_DryTable()
        // 2022.07.07 lhs End


        // 2021.04.01 lhs Start
        /// <summary>
        /// IV(화상판별센서)로 Dryer Clamp 촬영 및 판정
        /// Off-Picker에 달린 IV를 이용하여 Dryer에 Place 후 Clamping이 잘 되었는지를 체크
        /// </summary>
        /// <returns></returns>
        public bool Cyl_IV_DryClamp()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CHECK_IV2_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            m_iPreStep = m_iStep;
            #region 시퀀스 설명
            // 10 : Z축 촬영 위치로 이동
            // 11 : Z축 촬영 위치로 이동 확인
            // 12 : 0도로 회전
            // 13 : X축 #1 촬영 위치로 이동
            // 14 : X축 #1 촬영 위치로 이동 확인
            // 15 : IV2 Busy 여부 확인, 트리거 아웃하여 촬영
            // 16 : IV2 Busy 여부 확인, 판정 확인, 판정이 NG일 경우 3회 촬영 반복 (case 15) -> NG일 경우는 에러창 띄우기
            // 17 : X축 #2 촬영 위치로 이동
            // 18 : X축 #2 촬영 위치로 이동 확인
            // 19 : IV2 Busy 여부 확인, 트리거 아웃하여 촬영
            // 20 : IV2 Busy 여부 확인, 판정 확인, 판정이 NG일 경우 3회 촬영 반복 (case 19) -> NG일 경우는 에러창 띄우기
            // 21 : ESeq 변경 : OFP_Wait / OFP_BtmClean / OFP_CoverDryer 중 선택
            // 22 : Off-Picker 회전
            // 23 : Off-Picker 회전 체크, X축 Wait position
            // 24 : OffPicker WorkEnd
            #endregion

            switch (m_iStep)
            {
                default:
                    {
                        m_iStep = 0;
                        return true;
                    }

                case 10: // 10 : Z축 촬영 위치로 이동
                    {
                        m_tStTime = DateTime.Now;

                        if (CMot.It.Get_CP((int)EAx.DryZone_Z) != CData.SPos.dDRY_Z_TipClamped)
                        {
                            CErr.Show(eErr.DRY_Z_NOT_TIP_CLAMPED_POSITION);
                            _SetLog("Error : DRY Z axis not tip clamped position.");

                            m_iStep = 0;
                            return true;
                        }

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Picture) != 0)
                        {
                            return false;    
                        }
                        _SetLog("Z axis move Picture Positon", CData.SPos.dOFP_Z_Picture);

                        m_iStep++;
                        return false;
                    }

                case 11: // 11 : Z축 촬영 위치로 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Picture))
                        {
                            return false;
                        }
                        _SetLog("Z axis move Picture Positon OK", CData.SPos.dOFP_Z_Picture);

                        m_iStep++;
                        return false;
                    }

                case 12: // 12 : 0도로 회전
                    {
                        if (!Func_Picker0())
                        {
                            return false;
                        }
                        _SetLog("OffPicker Rotate 0 : Ok");

                        m_iStep++;
                        return false;
                    }


                case 13: // 13 : X축 #1 촬영 위치로 이동
                    {
                        if (CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Picture1) != 0)
                        {
                            return false;
                        }
                        _SetLog("X axis move Picture #1 Position", CData.SPos.dOFP_X_Picture1);

                        // 촬영#1를 위한 IV2 Program P000 선택
                        //CIO.It.Set_Y(eY.OFFP_IV2_ProgBit0, false); 
                        Func_SetIVProgram(0);   // P000

                        m_iStep++;
                        return false;
                    }
                case 14: // 14 : X축 #1 촬영 위치로 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Picture1))
                        {
                            return false;
                        }
                        _SetLog("X axis move Picture #1 Position OK", CData.SPos.dOFP_X_Picture1);

                        m_nRetryCnt = 0;

                        m_iStep++;
                        return false;
                    }
                case 15: // 15 : IV2 Busy 여부 확인, 트리거 아웃하여 촬영
                    {
                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_ERROR);
                            _SetLog("Error : IV2 is Error");

                            m_iStep = 0;
                            return true;
                        }

                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))
                        {
                            _SetLog("IV2 is busy");
                            return false;
                        }

                        // IV2의 외부 트리거
                        // 사양은 ON : Min 100us, OFF : Min 1.2ms
                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, true);
                        _SetLog("IV2 Trigger Out = ON");

                        System.Threading.Thread.Sleep(100);

                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, false);
                        _SetLog("IV2 Trigger Out = OFF");

                        m_iStep++;
                        return false;
                    }
                case 16: // 16 : IV2 Busy 여부 확인, 판정 확인, 판정이 NG일 경우 2회 더 시도 (case 15) -> NG일 경우는 에러창 띄우기
                    {
                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_ERROR);
                            _SetLog("Error : IV2 is Error");

                            m_iStep = 0;
                            return true;
                        }

                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))
                        {
                            _SetLog("IV2 is busy");
                            return false;
                        }

                        // OK가 아니면 2회 더 시도
                        if (!CIO.It.Get_X(eX.OFFP_IV2_AllOK))
                        {
                            _SetLog("IV2 is not AllOK");

                            if (m_nRetryCnt >= 2)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_NG);
                                _SetLog("Error : IV2 is NG");

                                m_iStep = 0;
                                return true; 
                            }
                            m_nRetryCnt++;
                            _SetLog(string.Format("Retry Count={0}", m_nRetryCnt));
                            
                            m_iStep = 15; 
                            return false;
                        }

                        m_iStep++;
                        return false;
                    }
                case 17: // 17 : X축 #2 촬영 위치로 이동
                    {
                        if (CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Picture2) != 0)
                        {
                            return false;
                        }
                        _SetLog("X axis move Picture #2 Position", CData.SPos.dOFP_X_Picture2);

                        // 촬영#2를 위한 IV2 Program P001 선택
                        //CIO.It.Set_Y(eY.OFFP_IV2_ProgBit0, true); 
                        Func_SetIVProgram(1);   // P001

                        m_iStep++;
                        return false;
                    }
                case 18: // 18 : X축 #2 촬영 위치로 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Picture2))
                        {
                            return false;
                        }
                        _SetLog("X axis move Picture #2 Position OK", CData.SPos.dOFP_X_Picture2);

                        m_nRetryCnt = 0;

                        

                        m_iStep++;
                        return false;
                    }
                case 19: // 19 : IV2 Busy 여부 확인, 트리거 아웃하여 촬영
                    {
                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_ERROR);
                            _SetLog("Error : IV2 is Error");

                            m_iStep = 0;
                            return true;
                        }

                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))
                        {
                            _SetLog("IV2 is busy");
                            return false;
                        }

                        // IV2의 외부 트리거
                        // 사양은 ON : Min 100us, OFF : Min 1.2ms
                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, true);
                        _SetLog("IV2 Trigger Out = ON");

                        System.Threading.Thread.Sleep(100);

                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, false);
                        _SetLog("IV2 Trigger Out = OFF");

                        m_iStep++;
                        return false;
                    }
                case 20: // 20 : IV2 Busy 여부 확인, 판정 확인, 판정이 NG일 경우 2회 더 시도 (case 19) -> NG일 경우는 에러창 띄우기
                    {
                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_ERROR);
                            _SetLog("Error : IV2 is Error");

                            m_iStep = 0; 
                            return true;
                        }

                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))
                        {
                            _SetLog("IV2 is busy");
                            return false;
                        }

                        // OK가 아니면 2회 더 시도
                        if (!CIO.It.Get_X(eX.OFFP_IV2_AllOK))
                        {
                            _SetLog("IV2 is not AllOK");

                            if (m_nRetryCnt >= 2)
                            {
                                CErr.Show(eErr.OFFLOADERPICKER_IV2_DI_NG);
                                _SetLog("Error : IV2 is NG");

                                m_iStep = 0;
                                return true;
                            }
                            m_nRetryCnt++;
                            _SetLog(string.Format("Retry Count={0}", m_nRetryCnt));

                            m_iStep = 19; // 다시 촬영
                            return false;
                        }

                        m_iStep++;
                        return false;
                    }

                case 21: // 21 : ESeq 변경 : OFP_Wait / OFP_BtmClean / OFP_CoverDryer 중 선택
                    {
                        // Dryer 상태를 Water Nozzle 로 변경.(물 세척)
                        
                        if      (CDataOption.UseDryWtNozzle)    { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaterNozzle; }
                        else if (CDataOption.UseOffP_IOIV)      { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Run;         }  // 2022.07.06 lhs

                        if (CDataOption.UseDryWtNozzle) { /*설정에 따름*/                        }   
                        else                            { CData.Opt.bOfpCoverDryerSkip = true;  }   // UseOffP_IOIV에서는 CoverDryer가 없음 : 2022.07.12  lhs
                        
                        // 드라이 커버역할 스킵이면 OFP_Wait or OFP_BtmClean
                        if (CData.Opt.bOfpCoverDryerSkip)
                        {
                            if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) //20200610 jhc : Auto_Running 상태가 아닐 수도 있음
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                                // 2021.02.01 lhs Start
                                _SetLog("Pick Improve Use, Dual Mode Status : " +
                                           CData.Parts[(int)EPart.ONL].iStat +
                                           ", " + CData.Parts[(int)EPart.INR].iStat +
                                           ", " + CData.Parts[(int)EPart.ONP].iStat +
                                           ", " + CData.Parts[(int)EPart.GRDL].iStat +
                                           ", " + CData.Parts[(int)EPart.GRDR].iStat +
                                           ", GRDR Exist Strip = " + CData.Parts[(int)EPart.GRDR].bExistStrip.ToString()
                                           );
                                // 2021.02.01 lhs End

                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd || CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Wait || CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready) &&    // 2021.02.01 lhs GRR_Ready 추가 
                                    (CData.Parts[(int)EPart.GRDR].bExistStrip == false))) //20200420 jhc : 종료 조건 추가
                                {
                                    _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                                    m_iStep++; //Off-Picker Wait Pos. 이동 후 종료
                                    return false;
                                }
                            }
                            else
                            {
                                if (CData.Opt.bOfpBtmCleanSkip)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                                    if (CData.Parts[(int)EPart.ONL].iStat  == ESeq.ONL_WorkEnd &&
                                        CData.Parts[(int)EPart.INR].iStat  == ESeq.INR_WorkEnd &&
                                        CData.Parts[(int)EPart.ONP].iStat  == ESeq.ONP_WorkEnd &&
                                        CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                        CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                                    {
                                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                                        m_iStep++; //Off-Picker Wait Pos. 이동 후 종료
                                        return false;
                                    }
                                }
                                else
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmClean;
                                }
                            }
                        } //end : if (CData.Opt.bOfpCoverDryerSkip)
                        else // 드라이 커버역할 스킵이 아니면
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverDryer;
                        }

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);  // SCK, JSCK
                            CData.Opt.bIOZT_ManualClick = false;    // 2021.09.15 lhs 수동클릭 아님
                        }

                        _SetLog("Status change : " + CData.Parts[(int)EPart.OFP].iStat);

                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }

                //20200417 jhc : Dry Place 후 WORK_END 시 -> Wait Pos. 이동 시퀀스 추가 (Auto-Running && Step Mode에서만)
                case 22: //Off-Picker 회전
                    {
                        // 200327 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청 // 2020.11.18 lhs  JCET 추가
                        if (CData.CurCompany == ECompany.ASE_KR || 
                            CData.CurCompany == ECompany.JCET)  {   Func_Picker0();     _SetLog("Picker 0.");   }
                        else                                    {   Func_Picker90();    _SetLog("Picker 90.");  }

                        m_iStep++;
                        return false;
                    }

                case 23: //Off-Picker 회전 체크, X축 Wait position
                    {
                        // 200327 mjy : 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.JCET) // 2020.11.18 lhs  JCET 추가
                        { if (!Func_Picker0()) return false; }
                        else
                        { if (!Func_Picker90()) return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 24:  // OffPicker WorkEnd
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Wait))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                            return false;
                        }

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK) //200121 ksg: , 200625 lks
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);  // SCK, JSCK
                            CData.Opt.bIOZT_ManualClick = false;    // 2021.09.15 lhs 수동클릭 아님
                        }

                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd; //Off-Picker WORK_END

                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }


            }//end : switch (m_iStep)
        }//end : Cyl_IV_DryClamp()
        // 2021.04.01 lhs End

        // 2021.04.01 lhs Start
        public bool Cyl_CoverDryer()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_COVER_DRYER_TIMEOUT);
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

                case 10:    // Vacuum Off, Ejcet Off, Z축 대기 위치로 이동 
                    {
                        m_tStTime = DateTime.Now;

                        if (CMot.It.Get_CP((int)EAx.DryZone_Z) > CData.SPos.dDRY_Z_TipClamped)
                        {
                            CErr.Show(eErr.DRY_Z_NOT_COVER_DRYER_POSITION);
                            _SetLog("Error : DRY Z axis not Cover Dryer position.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_Vacuum(false);
                        Func_Eject(false);
                        //Func_Drain(false);
                        //CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);    //Y6F Btm clean Water

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11:    //Z축 대기 위치 이동 확인, 피커 90도, X축 place 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        // 200417 jym : SG2000U는 피커 사이즈가 커서 드라이존 가서 턴해야함
                        if (CDataOption.Package == ePkg.Strip)
                        { Func_Picker90(); }
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Place);
                        _SetLog("X axis move place.", CData.SPos.dOFP_X_Place);

                        m_iStep++;
                        return false;
                    }

                case 12:    //피커 90도 확인, X축 place 위치 이동 확인, Z축 place - slow 위치 이동
                    {
                        // 200417 jym : SG2000U는 피커 사이즈가 커서 드라이존 가서 턴해야함
                        if (CDataOption.Package == ePkg.Strip)
                        { if (!Func_Picker90()) return false; }

                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Place))
                        { return false; }

                        // 200417 jym : SG2000U는 피커 사이즈가 커서 드라이존 가서 턴해야함
                        if (CDataOption.Package == ePkg.Unit)
                        { if (!Func_Picker90()) return false; }

                        m_Delay.Set_Delay(500);
                        _SetLog("Set delay : 500ms");

                        m_iStep++;
                        return false;
                    }

                case 13:
                    if (!m_Delay.Chk_Delay())
                    { return false; }

                    //CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_Place - CData.Dev.dOffP_Z_Slow);
                    CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_Place + CData.Dev.dOffP_Z_PlaceDn);
                    _SetLog("Z axis move position.", CData.Dev.dOffP_Z_Place + CData.Dev.dOffP_Z_PlaceDn);

                    m_iStep++;
                    return false;

                case 14:    //Z축 place - slow 위치 이동 확인 : 여기까지 내려서 커버역할을 할 수 있을까?
                    {
                        //if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place - CData.Dev.dOffP_Z_Slow))
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place + CData.Dev.dOffP_Z_PlaceDn))
                        { return false; }


                        // 여기까지만 내리고 테스트 후에 결정을 하자.

                        // OffPicker의 시퀀스 상태를 변경하자.
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverDryerKeep; // DryRun 시작 전까지 이 상태를 유지하기 위해


                        m_EndSeq = DateTime.Now; //190807 ksg :
                        m_SeqTime = m_EndSeq - m_StartSeq; //190807 ksg :
                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        _SetLog("Finish.  Time : " + m_SeqTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
                    
            }
        } //end : Cyl_CoverDryer()
          // 2021.04.11 lhs End

        // 2021.04.14 lhs Start
        /// <summary>
        /// Dryer 커버 역할 완료 후의 동작 수행 wait/BtmClean/WorkEnd
        /// </summary>
        /// <returns></returns>                 
        public bool Cyl_CoverDryerEnd()
        {
            if (m_iPreStep != m_iStep)
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_COVER_DRYER_END_TIMEOUT);
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

                case 10: // 10 : Z축 대기 위치로 이동
                    {
                        m_tStTime = DateTime.Now;

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) != 0)
                        {
                            return false;
                        }
                        _SetLog("Z axis move Wait Positon", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 11: // 11 : Z축 대기위치 이동 확인
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        _SetLog("Z axis move Wait Positon OK", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 12: // 12 : OFP_Wait / OFP_BtmClean / WaitPos로 이동 후 WorkEnd
                    {
                        if ((CDataOption.PickerImprove == ePickerTimeImprove.Use) && (CData.Dev.bDual == eDual.Dual)) 
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                            // 2021.02.01 lhs Start
                            _SetLog("Pick Improve Use, Dual Mode Status : " +
                                       CData.Parts[(int)EPart.ONL].iStat +
                                       ", " + CData.Parts[(int)EPart.INR].iStat +
                                       ", " + CData.Parts[(int)EPart.ONP].iStat +
                                       ", " + CData.Parts[(int)EPart.GRDL].iStat +
                                       ", " + CData.Parts[(int)EPart.GRDR].iStat +
                                       ", GRDR Exist Strip = " + CData.Parts[(int)EPart.GRDR].bExistStrip.ToString()
                                       );
                            // 2021.02.01 lhs End

                            if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                ((CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd || CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Wait || CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_Ready) &&    // 2021.02.01 lhs GRR_Ready 추가 
                                (CData.Parts[(int)EPart.GRDR].bExistStrip == false))) //20200420 jhc : 종료 조건 추가
                            {
                                _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);

                                m_iStep++; //Off-Picker Wait Pos. 이동 후 종료
                                return false;
                            }
                        }
                        else
                        {
                            if (CData.Opt.bOfpBtmCleanSkip)
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                   CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                   CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                   CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                   CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                                {
                                    _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);

                                    m_iStep++; //Off-Picker Wait Pos. 이동 후 종료
                                    return false;
                                }
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmClean;
                            }
                        }
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);  // SCK, JSCK
                            CData.Opt.bIOZT_ManualClick = false;    // 2021.09.15 lhs 수동클릭 아님
                        }

                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }


                case 13: // 13 : Off-Picker 회전
                    {
                        // 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.JCET) // 2020.11.18 lhs  JCET 추가
                        {
                            Func_Picker0();
                            _SetLog("Picker 0.");
                        }
                        else
                        {
                            Func_Picker90();
                            _SetLog("Picker 90.");
                        }

                        m_iStep++;
                        return false;
                    }

                case 14:  // 14 : Off-Picker 회전 체크, X축 대기위치로 이동
                    {
                        // 대기시 피커에서 떨어지는 물때문에 0도로 처리요청
                        if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.JCET) // 2020.11.18 lhs  JCET 추가
                        { if (!Func_Picker0()) return false; }
                        else
                        { if (!Func_Picker90()) return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 15: // 15 : X축 대기위치로 이동 확인, OFP_WorkEnd
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Wait))
                        {
                            CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                            return false;
                        }

                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (!CIO.It.Get_Y(eY.IOZT_Power)) CIO.It.Set_Y(eY.IOZT_Power, true);  // SCK, JSCK
                            CData.Opt.bIOZT_ManualClick = false;    // 2021.09.15 lhs 수동클릭 아님
                        }

                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd; 

                        _SetLog("Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        m_tTackTime = DateTime.Now - m_tStTime;
                        _SetLog("Finish.  Time : " + m_tTackTime.ToString(@"hh\:mm\:ss"));

                        m_iStep = 0;
                        return true;
                    }
            } //end : switch (m_iStep)
        } //end : Cyl_CoverDryerEnd()
        // 2021.04.14 lhs End

        public bool Cyl_Conversion()
        {//20190822 pjh : OfpConvPos
            if ((m_iStep < 11) && (m_iStep != m_iPreStep))
            {
                m_TimeOut.Set_Delay(TIMEOUT);
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CONVERSION_TIMEOUT);
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

                //20190821 pjh : Conversion Position시 Grinder Z Wait Pos, Grinder Y Dresser Replace Pos
                case 10:
                    {//Probe Up, Air Off
                        CData.L_GRD.ActPrbDown(false);
                        CData.L_GRD.ActPrbAir(false);
                        CData.R_GRD.ActPrbDown(false);
                        CData.R_GRD.ActPrbAir(false);
                        _SetLog("GDL, GDR Probe up.");

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {//Probe Up, Air Off 확인 Left Grind Z축 Wait Pos 이동
                        //if (!CIO.It.Get_Y(eY.GRDL_ProbeDn) && !CIO.It.Get_Y(eY.GRDR_ProbeDn))
                        if (CIO.It.Get_Y(eY.GRDL_ProbeDn) || CIO.It.Get_Y(eY.GRDR_ProbeDn)) //200311 ksg :
                        { return false; }

                        CMot.It.Mv_N(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]);
                        _SetLog("GDL Z axis move wait.", CData.SPos.dGRD_Z_Wait[0]);
                        CMot.It.Mv_N(m_iLGZ, CData.SPos.dGRD_Z_Wait[1]);
                        _SetLog("GDR Z axis move wait.", CData.SPos.dGRD_Z_Wait[1]);

                        m_iStep++;                        
                        return false;
                    }
                case 12:
                    {//Grind Z Wait Pos 확인
                        if (!CMot.It.Get_Mv(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]))
                        { return false; }
                        _SetLog("GDL, GDR Z xis move check.");

                        m_iStep++;
                        return false;
                    }
                
                case 13:
                    {//Top Cleaner Up

                        CData.L_GRD.ActTcDown(false);
                        CData.R_GRD.ActTcDown(false);
                        _SetLog("GDL, GDR Top cleaner up.");

                        m_iStep++;                        
                        return false;
                    }

                case 14:
                    {//Top Cleaner Up 확인, Grind Y Dresser Change Pos 이동
                        if (CIO.It.Get_Y(eY.GRDL_TopClnDn) && CIO.It.Get_Y(eY.GRDR_TopClnDn))
                        { return false; }

                        CMot.It.Mv_N(m_iLTY, CData.SPos.dGRD_Y_DrsChange[0]);
                        _SetLog("GDL Y axis move dresser change.", CData.SPos.dGRD_Y_DrsChange[0]);
                        CMot.It.Mv_N(m_iRTY, CData.SPos.dGRD_Y_DrsChange[1]);
                        _SetLog("GDR Y axis move dresser change.", CData.SPos.dGRD_Y_DrsChange[1]);

                        m_iStep++;
                        return false;
                    }

                case 15:
                    {//Grind Y Dresser Change Pos 확인
                        if (!CMot.It.Get_Mv(m_iLTY,CData.SPos.dGRD_Y_DrsChange[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iLTY,CData.SPos.dGRD_Y_DrsChange[0]))
                        { return false; }
                        _SetLog("GDL, GDR Y axis move check.");

                        m_iStep++;                        
                        return false;
                    }

                case 16: 
                    {                        
                        double chkOfpPos =  CMot.It.Get_FP((int)EAx.OnLoaderPicker_X);
                        if(chkOfpPos > CData.SPos.dONP_X_PlaceL - 10)
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_CAN_NOT_MOVE_CONVERSION_POSITION);
                            _SetLog("Error : Can't move conversion position.");

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("X axis check.");

                        m_iStep++;
                        return false;

                    }

                case 17:
                    {//Z축 대기위치 이동
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 18:
                    {//Z축 대기위치 이동 확인, 이젝트 오프, 피커 90도
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Conv);
                        _SetLog("X axis move conversion.", CData.SPos.dOFP_X_Conv);

                        m_iStep++;
                        return false;
                    }

                case 19: 
                    {//Conversion Pos 확인 후 종료
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Conv))
                        { return false; }
                        _SetLog("Finish.");

                        m_iStep = 0;
                        return true;
                    }                                                                   
            }
        }

        public bool Cyl_ConversionWait()
        {// 20190822 pjh : Conversion Wait Position
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay() == true)
                {
                    CErr.Show(eErr.OFFLOADERPICKER_WAIT_TIMEOUT);
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
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }
                case 11:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                            return false;
                        }

                        Func_Eject(false);
                        Func_Picker90();
                        _SetLog("Picker 90.");

                        m_iStep++;
                        return false;
                    }

                case 12:
                    {
                        if (!Func_Picker90())
                        { return false; }

                        CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Wait);
                        _SetLog("X axis move wait.", CData.SPos.dOFP_X_Wait);

                        m_iStep++;
                        return false;
                    }

                case 13:
                    {//Off Picker X Wait 이동 확인 후 Probe Up
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Wait))
                        { return false; }

                        CData.L_GRD.ActPrbDown(false);
                        CData.L_GRD.ActPrbAir(false);
                        CData.R_GRD.ActPrbDown(false);
                        CData.R_GRD.ActPrbAir(false);
                        _SetLog("GDL, GDR Probe up.");

                        m_iStep++;
                        return false;
                    }

                case 14:
                    {//Probe Up 확인 후 Left Grind Z Wait 이동
                        if (CIO.It.Get_Y(eY.GRDL_ProbeDn) || CIO.It.Get_Y(eY.GRDR_ProbeDn)) //200311 ksg :
                        { return false; }

                        CMot.It.Mv_N(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]);
                        _SetLog("GDL Z axis move wait.", CData.SPos.dGRD_Z_Wait[0]);
                        CMot.It.Mv_N(m_iRGZ, CData.SPos.dGRD_Z_Wait[1]);
                        _SetLog("GDR Z axis move wait.", CData.SPos.dGRD_Z_Wait[1]);

                        m_iStep++;                        
                        return false;
                    }

                case 15:
                    {//Left Grind Z Wait 확인 후 Right Grind Z Wait 이동
                        if (!CMot.It.Get_Mv(m_iLGZ, CData.SPos.dGRD_Z_Wait[0]))
                        { return false; }
                        if (!CMot.It.Get_Mv(m_iRGZ, CData.SPos.dGRD_Z_Wait[1]))
                        { return false; }

                        //Right Grind Z Wait 확인 후 Top Cleaner Up
                        CData.L_GRD.ActTcDown(false);
                        CData.R_GRD.ActTcDown(false);
                        _SetLog("GDL, GDR Top cleaner up.");

                        m_iStep++;                        
                        return false;
                    }

                case 16:
                    {//Top Cleaner Up 확인 후 Left Grind Y Wait 이동
                        if (CIO.It.Get_Y(eY.GRDL_TopClnDn) && CIO.It.Get_Y(eY.GRDR_TopClnDn))
                        { return false; }

                        CMot.It.Mv_N(m_iLTY, CData.SPos.dGRD_Y_Wait[0]);
                        _SetLog("GDL Y axis move wait.", CData.SPos.dGRD_Y_Wait[0]);
                        CMot.It.Mv_N(m_iRTY, CData.SPos.dGRD_Y_Wait[1]);
                        _SetLog("GDR Y axis move wait.", CData.SPos.dGRD_Y_Wait[1]);
                        
                        m_iStep++;                        
                        return false;
                    }

                case 17:
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

		//OFP UP DOWN CLEAN
        //20190926 ghk_ofppadclean
        public bool Cyl_PckPadClean()
        {
            // Timeout check
            if (m_iPreStep != m_iStep)
            { m_TimeOut.Set_Delay(TIMEOUT); }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    CErr.Show(eErr.OFFLOADERPICKER_CLEAN_TIMEOUT);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            if (Chk_Strip())
            {
                CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
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

                case 10:    //버큠 오프, 이젝트 온, 드레인 워터 오프, 드라이 바텀 워터 온, Z축 대기 위치 이동
                    {
                        if (Chk_Strip())
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_DETECT_STRIP_ERROR);
                            _SetLog("Error : Detect strip.");

                            m_iStep = 0;
                            return true;
                        }

                        Func_Vacuum(false);
                        Func_Eject(true);
                        Func_Drain(false);
                        CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, true);//Y6F Btm clean Water

                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);
                        m_iPadCnt = 0;
                        m_dPos_X = CData.Dev.dOffP_X_ClnStart;

                        m_iStep++;
                        return false;
                    } 
                    
                case 11:    //Z축 대기 위치 이동 확인, 피커 0도
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        Func_Picker0();
                        _SetLog("Picker 0.");

                        m_iStep++;
                        return false;
                    }

                case 12:    //== 반복 시작 == 피커 0도 확인, X축 클린 위치 이동, 반복 종료 확인
                    {
                        if (!Func_Picker0())
                        { return false; }

                        CMot.It.Mv_N(m_iX, m_dPos_X);
                        _SetLog("X axis move position.", m_dPos_X);

                        m_iStep++;
                        return false;
                    }

                case 13:    //X축 클린 위치 이동 확인, Z축 클린 위치 이동
                    {
                        if (!CMot.It.Get_Mv(m_iX, m_dPos_X))
                        { return false; }

                        // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                        if (CData.CurCompany == ECompany.ASE_KR ||
                            CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                            CData.CurCompany == ECompany.JSCK   ||
                            CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                        {
                            CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_ClnStart);
                            _SetLog("Z axis move clean start.", CData.Dev.dOffP_Z_ClnStart);
                        }
                        else
                        {
                            CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_ClnStart);
                            _SetLog("Z axis move clean start.", CData.SPos.dOFP_Z_ClnStart);
                        }
                        // 2021.02.27 SungTae End

                        m_iStep++;
                        return false;
                    }

                case 14:    //Z축 클린 위치 이동 확인, 딜레이 설정
                    {
                        // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                        //if (CData.CurCompany == ECompany.ASE_KR)
                        if (CData.CurCompany == ECompany.ASE_KR ||
                            CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                            CData.CurCompany == ECompany.JSCK   ||
                            CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_ClnStart))  { return false; }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_ClnStart)) { return false; }
                        }
                        // 2021.02.27 SungTae End

                        m_OfpPadDelay.Set_Delay(CData.Opt.iOfpPadCleanTime);
                        _SetLog("Set delay : " + CData.Opt.iOfpPadCleanTime + "ms");

                        m_iStep++;
                        return false;
                    }

                case 15:    //딜레이 확인, Z축 2mm업(Pad Height = 1mm)
                    if (!m_OfpPadDelay.Chk_Delay())
                    { return false; }

                    // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                    //if (CData.CurCompany == ECompany.ASE_KR)
                    if (CData.CurCompany == ECompany.ASE_KR ||
                        CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                        CData.CurCompany == ECompany.JSCK   ||
                        CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                    {
                        CMot.It.Mv_N(m_iZ, (CData.Dev.dOffP_Z_ClnStart - GV.OFP_PAD_CLEAN_Z_UP));
                        _SetLog("Z axis move posiiton.", (CData.Dev.dOffP_Z_ClnStart - GV.OFP_PAD_CLEAN_Z_UP));
                    }
                    else
                    {
                        CMot.It.Mv_N(m_iZ, (CData.SPos.dOFP_Z_ClnStart - GV.OFP_PAD_CLEAN_Z_UP));
                        _SetLog("Z axis move posiiton.", (CData.SPos.dOFP_Z_ClnStart - GV.OFP_PAD_CLEAN_Z_UP));
                    }
                    // 2021.02.27 SungTae End

                    m_iStep++;
                    return false;

                case 16:    //Z축 2mm업 확인, 반복 확인
                    {
                        // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
                        //if (CData.CurCompany == ECompany.ASE_KR)
                        if (CData.CurCompany == ECompany.ASE_KR ||
                            CData.CurCompany == ECompany.SCK    ||  // 2021.07.14 lhs  SCK, JSCK 추가
                            CData.CurCompany == ECompany.JSCK   ||
                            CData.CurCompany == ECompany.SkyWorks) //220216 pjh : Skyworks 추가
                        {
                            if (!CMot.It.Get_Mv(m_iZ, (CData.Dev.dOffP_Z_ClnStart - GV.OFP_PAD_CLEAN_Z_UP))) { return false; }
                        }
                        else
                        {
                            if (!CMot.It.Get_Mv(m_iZ, (CData.SPos.dOFP_Z_ClnStart - GV.OFP_PAD_CLEAN_Z_UP))) { return false; }
                        }
                        // 2021.02.27 SungTae End

                        if (CData.Dev.dShSide > (GV.SPONGE_WIDTH * m_iPadCnt))
                        {
                            m_iPadCnt++;

                            // 2021.07.19 lhs Start : PM Count OffP Sponge Current Count 증가
                            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12) 
                            {
                                CData.tPM.nOffP_Sponge_Check_CurrCnt++;
                                CData.tPM.nOffP_Sponge_Change_CurrCnt++;
                                _SetLog("OffPicker Sponge Current Count : Check Count = " + CData.tPM.nOffP_Sponge_Check_CurrCnt.ToString() +
                                                                       ", Change Count = " + CData.tPM.nOffP_Sponge_Change_CurrCnt.ToString());
                            }
                            // 2021.07.19 lhs End

                            m_dPos_X -= (GV.SPONGE_WIDTH);
                            _SetLog("Loop.  Count : " + m_iPadCnt);

                            m_iStep = 12;
                            return false;
                        }

                        _SetLog("Loop end.");

                        // 2021.07.21 lhs Start : PM Count OffP Sponge Current Count, 루프 종료시 저장, SCK VOC
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)
                        {
                            CMnt.It.Save();
                            _SetLog("Maintenance.cfg Save !!!");
                        }
                        // 2021.07.21 lhs End

                        m_iStep++;
                        return false;
                    }

                case 17:    //바텀 워터 오프, Z축 대기 위치 이동
                    {
                        CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
                        CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait);
                        _SetLog("Z axis move wait.", CData.SPos.dOFP_Z_Wait);

                        m_iStep++;
                        return false;
                    }

                case 18:    //Z축 대기위치 이동 확인, X축 클린 스타트 위치 이동, 드라이 바텀 워터 오프, 버큠 오프, 이젝트 오프, 드레인 오프
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        { return false; }
                        CMot.It.Mv_N(m_iX, CData.Dev.dOffP_X_ClnStart);
                        _SetLog("X axis move wait.", CData.Dev.dOffP_X_ClnStart);

                        CIO.It.Set_Y((int)eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
                        CData.dtSpgWOffLastTime = DateTime.Now;

                        Func_Vacuum(false);
                        Func_Eject(false);
                        Func_Drain(false);

                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                        if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                           CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                           CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                           CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                           CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                        {
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;
                        }

                        _SetLog("Finish.  Status : " + CData.Parts[(int)EPart.OFP].iStat);

                        m_iStep = 0;
                        return false;
                    }
            }
        }

        //211007 syc : 2004U
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Cyl_OFP_IV2()
        {
            int idelay = 800; // 딜레이 설정
            // Timeout check            
            if (m_iPreStep != m_iStep)
            {
                //210824 syc : 2004U 수정 필요 타임 아웃 시간 메뉴얼 동작 돌려고보 다시 정하기
                m_TimeOut.Set_Delay(30000);
                //
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    //210824 syc : 2004U 수정 필요 
                    //에러 추가할 것.
                    CErr.Show(eErr.OFFLOADERPICKER_UNIT_CHECK_TIMEOUT_ERROR);

                    _SetLog("Error : OFFLOADERPICKER_UNIT_CHECK_TIMEOUT_ERROR.");

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
                        iOFPPara = 1;       // IV2 검사할때 몇번째 파라미터를 사용할건지 초기화 (처음에는 첫번째 파라미터 사용)

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
                            _SetLog("OFP IV2 Check Skip");

                            m_iStep = 100; // IV2 사용이 Skip 이라면 마지막 구문으로 이동
                            //
                            return false;
                        }
                        else // IV2 사용 상태라면 진행
                        {
                            _SetLog("OFP IV2 Check Use");

                            m_iStep++;
                            return false;
                        }
                    }

                // Z축 대기 위치 이동                 
                case 12:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0)
                        {
                            _SetLog("Z axis move to wait position.", CData.SPos.dOFP_Z_Wait);
                            m_iStep++;
                        }
                        return false;
                    }
                // Z축 대기 위치 이동 확인
                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait)) { return false; }
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
                        if (iOFPPara == 1) // 첫번째 파라미터일 경우
                        {
                            SIV2Para = CData.Dev.sIV2_OFP1_Para; // 1번째 검사 파라미터
                            dIV2Xpos = CData.Dev.dIV2_OFP1_X;    // X 축 검사 위치
                            dIV2Zpos = CData.Dev.dIV2_OFP1_Z;    // Z 축 검사 위치
                        }
                        else if (iOFPPara == 2) // 두번째 파라미터일 경우
                        {
                            SIV2Para = CData.Dev.sIV2_OFP2_Para; // 2번째 검사 파라미터
                            dIV2Xpos = CData.Dev.dIV2_OFP2_X;    // X 축 검사 위치
                            dIV2Zpos = CData.Dev.dIV2_OFP2_Z;    // Z 축 검사 위치
                        }
                        else // 버그
                        {
                            _SetLog("Error : case 15번 잘못된 접근입니다.");
                            m_iStep = 0;
                            return true;
                        }
                        _SetLog("Parameter = " + iOFPPara);
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
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);
                            _SetLog("Error : Check IV2 Status");

                            m_iStep = 0;
                            return true;
                        }
                        // ----- 가동 상태 확인 완료
                        // ----- 센서 사용가능 상태 확인
                        if (Chk_IV2Ready() != 1) // 사용 불가능 상태 또는 타임 아웃
                        {
                            _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                        m_iStep++;
                        return false;
                    }

                // Y축 검사위치(BCR 회피위치)로 이동 확인
                case 26:
                    {                        
                        m_iStep++;
                        return false;                        
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
                            _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_PARAMETER_CHANGE_FAIL);

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
                            _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                                _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                                CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_RESULT_NG);

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
                        if (CData.Dev.bIV2_ONP2_Use && iOFPPara < 2)
                        //if(iONPPara <??) 만약에 2번이상 찍어야하는 상황오면 수정 이부분은 주석 제거하고 수정 후 사용 필요
                        {
                            iOFPPara++;
                            _SetLog("Parameter Changed : " + iOFPPara);

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
                        CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_CheckSensor;   // Cover Place 상태 검사
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WaitCoverPick; // Dry 종료 후 Cover Pick 하기위해 대기

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //210824 syc : 2004U
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Cyl_OFP_CoverCheck()
        {
            int idelay = 800; // 딜레이 설정

            // Timeout check            
            if (m_iPreStep != m_iStep)
            {
                //210824 syc : 2004U 수정 필요 타임 아웃 시간 메뉴얼 동작 돌려고보 다시 정하기
                m_TimeOut.Set_Delay(60000);
                //
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    //210824 syc : 2004U 수정 필요 
                    //에러 추가할 것.
                    CErr.Show(eErr.OFFLOADERPICKER_COVER_CHECK_TIMEOUT_ERROR);                     
                    _SetLog("Error : OFFLOADERPICKER_COVER_CHECK_TIMEOUT_ERROR.");

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
                        if (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) || // 클램프 Close 센서가 켜져있거나
                           !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open) || // 클램프 Open 센서가 꺼져있거나
                            CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect) ||// 케리어 디텍트 센서가 감지되어있는 경우 알람 확인 알람 발생
                            CIO.It.Get_X(eX.OFFP_Vacuum)) // Vac 감지시
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_CLAMP_CHECK_ERROR);
                            _SetLog("Error : Check Sensor Error");
                            _SetLog("OFFP_Carrier_Clamp_Close   : " + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));
                            _SetLog("OFFP_Carrier_Clamp_Open    : " + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));
                            _SetLog("OFFP_Picker_Carrier_Detect : " + CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect));
                            _SetLog("OFFP_Picker_Vacuum_Detect : " + CIO.It.Get_X(eX.OFFP_Vacuum));

                            m_iStep = 0;
                            return true;
                        }

                        // 축 상태 확인 구문 추가 필요성 검토하여 추가 유무 판단할 것.                       
                        iIV2countCheck = 0; // Conncet 상태 확인 카운트 초기화

                        _SetLog("initializing");

                        m_iStep++;
                        return false;

                    }
                // IV2 사용 유무 검사
                // IV2 카메라 이상시 스킵하고 사용하기 위한 목적 (비상용)
                case 11:
                    {
                        if (CDataOption.UseIV2 == false) // IV2 사용이 Skip 이라면 마지막 구문으로 이동
                        {
                            _SetLog("OFP IV2 Check Skip");
                            m_iStep = 27;

                            return false;
                        }
                        else // IV2 사용 상태라면 진행
                        {
                            _SetLog("OFP IV2 Check Use");

                            m_iStep++;
                            return false;
                        }
                    }

                // Dry 대기 위치 확인
                // Z축 대기 위치 이동                 
                case 12:
                    {
                        if (!CMot.It.Get_Mv((int)EAx.DryZone_X, CData.SPos.dDRY_X_Wait) ||
                            !CMot.It.Get_Mv((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up) ||
                            !CMot.It.Get_Mv((int)EAx.DryZone_Air, CData.SPos.dDRY_R_Wait))
                        {
                            _SetLog("Error : Dry Z Not Up Position");
                            CErr.Show(eErr.DRY_Z_NOT_UP_POSITION);

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check Dry Z Up Position Complete");

                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0)
                        {
                            _SetLog("Z axis move to wait position.", CData.SPos.dOFP_Z_Wait);
                            m_iStep++;
                        }
                        return false;
                    }
                // Z축 대기 위치 이동 확인
                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis wait position movement completed");

                            IVCheckSensorState(); //센서 레디상태 확인 명령어 Send

                            m_iStep = 20;
                            return false;
                        }
                    }

                // 파라미터 추가 가능성 있기때문에 Step 번호 남겨둠
                case 20:
                    {
                        // ----- 파라미터 설정 
                        SIV2Para = CData.Dev.sIV2_OFPCover_Para; // 검사 파라미터
                        dIV2Xpos = CData.Dev.dIV2_OFPCover_X;    // X 축 검사 위치
                        dIV2Zpos = CData.Dev.dIV2_OFPCover_Z;    // Z 축 검사 위치

                        _SetLog("Parameter = " + iOFPPara);
                        // ----- 파라미터 설정 끝

                        IV2CheckRun();             // IV2 가동 상태 확인
                        IVCheckSensorState();      // 센서 레디상태 확인 명령어 Send
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
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);
                            _SetLog("Error : Check IV2 Status");

                            m_iStep = 0;
                            return true;
                        }
                        // ----- 가동 상태 확인 완료
                        // ----- 센서 사용가능 상태 확인
                        if (Chk_IV2Ready() != 1) // 사용 불가능 상태 또는 타임 아웃
                        {
                            _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                        m_iStep++;
                        return false;
                    }

                // Y축 검사위치(BCR 회피위치)로 이동 확인
                case 26:
                    {
                        m_iStep++;
                        return false;
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
                            _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_PARAMETER_CHANGE_FAIL);

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
                            _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                                _SetLog("Error : OFFLOADERPICKER_IV2_NOT_READY");
                                CErr.Show(eErr.OFFLOADERPICKER_IV2_NOT_READY);

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
                            CErr.Show(eErr.OFFLOADERPICKER_IV2_RESULT_NG);

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
                        m_iStep = 100;
                        return false;
                    }
                case 100:
                    {
                        //210927 syc : 2004U
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverPick;
                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.OFP].iStat);

                        CData.bOFP_CoverPick = false;

                        //다음 스텝 조건변경
                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_CoverPick()
        {
            // Timeout check            
            if (m_iPreStep != m_iStep)
            {
                //210824 syc : 2004U 수정 필요 타임 아웃 시간 메뉴얼 동작 돌려고보 다시 정하기
                m_TimeOut.Set_Delay(TIMEOUT);
                //
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    //210831 syc : 2004U 수정 필요 
                    CErr.Show(eErr.OFFLOADERPICKER_COVER_PICK_TIMEOUT_ERROR);                     
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            // Step 23번부터 Vac 꺼지면 알람 발생
            if (m_iStep >= 23 && !CIO.It.Get_X(eX.OFFP_Vacuum))
            {
                CErr.Show(eErr.OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR);
                _SetLog("Error : Cover Vacuum fail");

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
                // 센서 점검
                case 10:
                    {
                        // Vaccum 감지되어있으면 커버있다고 판단, Z축 대기이동, 스텝끝으로 이동
                        if (CIO.It.Get_X(eX.OFFP_Vacuum))
                        {
                            //_SetLog("Detected cover");
                            _SetLog("vacuum on -> cover present, go to step 23");
                            
                            // 2022.08.17 lhs Start : Z축 대기 이동, 마지막 스텝으로 이동하여 상태 변경
                            //CErr.Show(eErr.OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR);
                            //m_iStep = 0;
                            //return true;
                            
                            m_iStep = 23; // Z축 대기 이동, 마지막 스텝으로...
                            return false;
                            // 2022.08.17 lhs End 
                        }

                        if (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close)    || // 클램프 Close 센서가 켜져있거나
                           !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open)     || // 클램프 Open 센서가 꺼져있거나
                            CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect)) // 케리어 디텍트 센서가 감지되어있는 경우 알람 확인 알람 발생
                        {                            
                            CErr.Show(eErr.OFFLOADERPICKER_CLAMP_CHECK_ERROR);
                            _SetLog("Error : Check Sensor Error");
                            _SetLog("OFFP_Carrier_Clamp_Close   : " + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close)  );
                            _SetLog("OFFP_Carrier_Clamp_Open    : " + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open)   );
                            _SetLog("OFFP_Picker_Carrier_Detect : " + CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect));

                            m_iStep = 0;
                            return true;
                        }

                        // Vaccum off, Clamp Open 진행
                        if (!Func_Vacuum(false) && Func_CarrierClampOpen())//OfP Vac 해제, Carrier Clamp 해제
                        {
                            Func_Eject(false);
                            Func_Drain(false);

                            _SetLog("Vacuum off");
                            _SetLog("Gripper Off");

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // Dry Z축 Up Position 확인
                case 11:
                    {                       
                        if (!CMot.It.Get_Mv((int)EAx.DryZone_X  , CData.SPos.dDRY_X_Wait) || // Dry X Wait Position 확인
                            !CMot.It.Get_Mv((int)EAx.DryZone_Z  , CData.SPos.dDRY_Z_Up)   || // Dry Z Up   Position 확인
                            !CMot.It.Get_Mv((int)EAx.DryZone_Air, CData.SPos.dDRY_R_Wait))   // Dry R Wait Position 확인
                        {
                            _SetLog("Error : Dry Z Not Up Position");
                            CErr.Show(eErr.DRY_Z_NOT_UP_POSITION);

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check Dry Z Up Position Check Complete");

                        m_iStep++;
                        return false;
                    }
                //210906 syc : 2004U
                // OFP Z축 대기 위치 이동               
                case 12:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0) // OFP Z축 대기위치 이동                           
                        {
                            _SetLog("OFP Z axis move to Wait position.", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                //210906 syc : 2004U
                //Z축 대기 위치 이동 확인
                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("OFP Z axis Wait position movement completed", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                            return false;
                        }                        
                    }
                // OFP X축 Cover Pick 위치 이동       
                case 14:
                    {
                        if (CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Place) == 0)

                        {
                            _SetLog("OFP X axis move to Cover Pick position."  , CData.SPos.dOFP_X_Place);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // OFP X축 Cover Pick 위치 이동 확인
                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Place))
                        {
                            return false;
                        }
                        else
                        {
                            Func_Picker90(); //피커 90도
                            _SetLog("X axis Cover Pick position movement completed", CData.SPos.dOFP_X_IV2Cover);
                            _SetLog("Act Picker 90");
                            m_iStep++;
                            return false;
                        }
                    }
                // Z축 Cover Pick - 10 위치 이동
                case 16:
                    {
                        if (!Func_Picker90())
                        { return false; }                                               

                        if (CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_Place - 10) == 0) 
                        {
                            _SetLog("Z axis move to Cover Pick -10 position.", CData.Dev.dOffP_Z_Place - 10);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                    
                // Z축 Cover Pick - 10 위치 이동 확인
                case 17:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place - 10))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis Cover Pick - 10 position movement completed", CData.Dev.dOffP_Z_Place - 10);
                            m_iStep++;
                            return false;
                        }
                    }
                // Z축 Cover Pick 위치 이동
                // 속도 느리게
                case 18:
                    {
                        if (!Func_Picker90())
                        { return false; }

                        if (CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Place) == 0) 
                        {
                            _SetLog("Z axis move to Cover Pick position.", CData.Dev.dOffP_Z_Place);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // Z축 Cover Pick 위치 이동 확인
                case 19:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis Cover Pick -10  position movement completed", CData.Dev.dOffP_Z_Place);
                            m_iStep++;
                            return false;
                        }
                    }

                //Vac 활성화
                case 20:
                    {
                        Func_Eject(false);
                        Func_Drain(false);
                        Func_Vacuum(true);
                        m_Delay.Set_Delay(5000); // 몇 초 할지는 정해야함  // 2022.08.17 lhs : 3초->5초로 변경

                        _SetLog("Carrier Vacuum On");
                        _SetLog("Delay set : " + "5000" + "mms"); 

                        m_iStep++;
                        return false;
                    }
                // 딜레이 확인
                case 21:
                    {
                        if (!m_Delay.Chk_Delay())
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Check Delay : 5000 mms");
                            m_iStep++;
                            return false;
                        }
                    }
                //Vacuum 확인
                case 22:
                    {
                        if (Func_Vacuum(true)) //Vacuum 확인
                        {
                            _SetLog("Carrier Vacuum On Check Complete");
                            m_iStep++;
                            return false;
                        }
                        else // Cover Vacuum 확인 안될경우 알람 발생
                        {                            
                            CErr.Show(eErr.OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR); 
                            _SetLog("Error : Cover Vacuum fail");
                            m_iStep = 0;
                            return true;
                        }
                    }
                //Z축 대기 위치 이동
                case 23:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0)
                        {
                            _SetLog("Z axis move to Wait position.", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                //Z축 대기 위치 이동 확인
                case 24:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis Wait position movement completed", CData.SPos.dOFP_Z_Wait);
                            m_iStep++;
                            return false;
                        }
                    }
                case 25:
                    {                             
                        if (CData.bDry_CoverPick) //210929 syc : 2004U Lot 시작시 Cover Pick 
                        {
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitPlace;
                            CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;                      
                        }
                        else
                        {
                            // 드라이 상태 변경
                            CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_Out;

                            // 메뉴얼 동작 이후 드라이 상태 변경 조건, 메뉴얼 동작 이전에 메세지 창으로 확인
                            if (!CData.bOFFP_NextSeqDryout) { CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitPlace; } // 메뉴얼 동작에서만 물어봄

                            CData.bOFFP_NextSeqDryout = true; // 메뉴얼 동작 이후 초기화                          
                            // 메뉴얼 동작 이후 상태 변경 

                            // OFP 제외 Main Work End시 Cover Place 준비
                            if (CSQ_Main.It.m_iStat == EStatus.Manual)
                            {
                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                                {
                                    CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WaitCoverPlace;
                                }
                            }                           
                            // 드라이 상태 변경 종료
                           
                            if (CData.Opt.bOfpBtmCleanSkip) // Btm Clean Skip일경우
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;

                                if (CData.Parts[(int)EPart.ONL].iStat == ESeq.ONL_WorkEnd &&
                                    CData.Parts[(int)EPart.INR].iStat == ESeq.INR_WorkEnd &&
                                    CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDL].iStat == ESeq.GRL_WorkEnd &&
                                    CData.Parts[(int)EPart.GRDR].iStat == ESeq.GRR_WorkEnd)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_CoverPlace; // WorkEnd가 아닌 Cover Place 하고 WorkEnd 해야함
                                } 
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_BtmClean;
                            }                                                        
                        }

                        CData.bDry_CoverPick = false;

                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        public bool Cyl_CoverPlace()
        {
            // Timeout check            
            if (m_iPreStep != m_iStep)
            {
                //210824 syc : 2004U 수정 필요 타임 아웃 시간 메뉴얼 동작 돌려고보 다시 정하기
                m_TimeOut.Set_Delay(TIMEOUT);
                //
            }
            else
            {
                if (m_TimeOut.Chk_Delay())
                {
                    //210831 syc : 2004U 수정 필요 
                    CErr.Show(eErr.OFFLOADERPICKER_COVER_PICK_TIMEOUT_ERROR);
                    _SetLog("Error : Timeout.");

                    m_iStep = 0;
                    return true;
                }
            }

            // Step 10번부터 Vac 꺼지면 알람 발생
            if (10 <= m_iStep && m_iStep <= 19 )
            {
                if (!CIO.It.Get_X(eX.OFFP_Vacuum))
                {
                    CErr.Show(eErr.OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR);
                    _SetLog("Error : Cover Vacuum fail");

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
                // 센서 점검
                case 10:
                    {
                        if (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) || // 클램프 Close 센서가 켜져있거나
                           !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open))// 클램프 Open 센서가 꺼져있거나
                           
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_CLAMP_CHECK_ERROR);
                            _SetLog("Error : Check Sensor Error");
                            _SetLog("OFFP_Carrier_Clamp_Close   : " + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));
                            _SetLog("OFFP_Carrier_Clamp_Open    : " + CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));

                            m_iStep = 0;
                            return true;
                        }

                        // Vaccum off, Clamp Open 진행
                        if (Func_CarrierClampOpen())
                        {
                            _SetLog("Gripper Open");

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // Dry Z축 Up Position 확인
                case 11:
                    {
                        if (!CMot.It.Get_Mv((int)EAx.DryZone_X, CData.SPos.dDRY_X_Wait) || // Dry X Wait Position 확인
                            !CMot.It.Get_Mv((int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up) || // Dry Z Up   Position 확인
                            !CMot.It.Get_Mv((int)EAx.DryZone_Air, CData.SPos.dDRY_R_Wait))   // Dry R Wait Position 확인
                        {
                            _SetLog("Error : Dry Z Not Up Position");
                            CErr.Show(eErr.DRY_Z_NOT_UP_POSITION);

                            m_iStep = 0;
                            return true;
                        }

                        _SetLog("Check Dry Z Up Position Check Complete");

                        m_iStep++;
                        return false;
                    }
                //210906 syc : 2004U
                // OFP Z축 대기 위치 이동               
                case 12:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0) // OFP Z축 대기위치 이동                           
                        {
                            _SetLog("OFP Z axis move to Wait position.", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                //210906 syc : 2004U
                //Z축 대기 위치 이동 확인
                case 13:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("OFP Z axis Wait position movement completed", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                    }
                // OFP X축 Cover Pick 위치 이동       
                case 14:
                    {
                        if (CMot.It.Mv_N(m_iX, CData.SPos.dOFP_X_Place) == 0)

                        {
                            _SetLog("OFP X axis move to Cover Pick position.", CData.SPos.dOFP_X_Place);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // OFP X축 Cover Pick 위치 이동 확인
                case 15:
                    {
                        if (!CMot.It.Get_Mv(m_iX, CData.SPos.dOFP_X_Place))
                        {
                            return false;
                        }
                        else
                        {
                            Func_Picker90(); //피커 90도
                            _SetLog("X axis Cover Pick position movement completed", CData.SPos.dOFP_X_IV2Cover);
                            _SetLog("Act Picker 90");
                            m_iStep++;
                            return false;
                        }
                    }
                // Z축 Cover Pick - 10 위치 이동
                case 16:
                    {
                        if (!Func_Picker90())
                        { return false; }

                        if (CMot.It.Mv_N(m_iZ, CData.Dev.dOffP_Z_Place - 10) == 0)
                        {
                            _SetLog("Z axis move to Cover Pick -10 position.", CData.Dev.dOffP_Z_Place - 10);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }

                // Z축 Cover Pick - 10 위치 이동 확인
                case 17:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place - 10))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis Cover Pick - 10 position movement completed", CData.Dev.dOffP_Z_Place - 10);
                            m_iStep++;
                            return false;
                        }
                    }
                // Z축 Cover Pick 위치 이동
                // 속도 느리게
                case 18:
                    {
                        if (!Func_Picker90())
                        { return false; }

                        if (CMot.It.Mv_S(m_iZ, CData.Dev.dOffP_Z_Place) == 0)
                        {
                            _SetLog("Z axis move to Cover Pick position.", CData.Dev.dOffP_Z_Place);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                // Z축 Cover Pick 위치 이동 확인
                case 19:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.Dev.dOffP_Z_Place))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis Cover Pick -10  position movement completed", CData.Dev.dOffP_Z_Place);
                            m_iStep++;
                            return false;
                        }
                    }

                //Vac 비활성화
                case 20:
                    {
                        Func_Vacuum(false);
                        Func_Eject(true);
                        Func_Drain(false);

                        m_Delay.Set_Delay(3000); // 몇초할지는 정해야함

                        _SetLog("Carrier Vacuum Off");
                        _SetLog("Delay set : " + "3000" + "mms");

                        m_iStep++;
                        return false;
                    }
                // 딜레이 확인
                case 21:
                    {
                        if (!m_Delay.Chk_Delay())
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Check Delay : 3000 mms");
                            m_iStep++;
                            return false;
                        }
                    }
                //Vacuum 확인
                case 22:
                    {
                        if (!Func_Vacuum(false)) //Vacuum 확인
                        {
                            Func_Eject(false);
                            Func_Drain(true);

                            _SetLog("Carrier Vacuum Off Check Complete");
                            m_iStep++;
                            return false;
                        }
                        else // Cover Vacuum 확인 안될경우 알람 발생
                        {
                            CErr.Show(eErr.OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR);
                            _SetLog("Error : Cover Vacuum off fail");
                            m_iStep = 0;
                            return true;
                        }
                    }
                //Z축 대기 위치 이동
                case 23:
                    {
                        if (CMot.It.Mv_N(m_iZ, CData.SPos.dOFP_Z_Wait) == 0)
                        {
                            _SetLog("Z axis move to Wait position.", CData.SPos.dOFP_Z_Wait);

                            m_iStep++;
                            return false;
                        }
                        return false;
                    }
                //Z축 대기 위치 이동 확인
                case 24:
                    {
                        if (!CMot.It.Get_Mv(m_iZ, CData.SPos.dOFP_Z_Wait))
                        {
                            return false;
                        }
                        else
                        {
                            _SetLog("Z axis Wait position movement completed", CData.SPos.dOFP_Z_Wait);
                            m_iStep++;
                            return false;
                        }
                    }
                case 25:
                    {
                        CData.Parts[(int)EPart.DRY].iStat = ESeq.DRY_WorkEnd;
                        CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_WorkEnd;

                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.OFP].iStat);
                        _SetLog("Status change.  Status : " + CData.Parts[(int)EPart.DRY].iStat);

                        m_iStep = 0;
                        return true;
                    }
            }
        }

        //210929 syc : 2004U 
        public bool Func_CarrierClampClose()
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.OFFP_Carrier_Clamp_Close,  true );
            CIO.It.Set_Y(eY.OFFP_Carrier_Clamp_Open ,  false);

            bRet = (!CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open) && 
                     CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));

            return bRet;
        }

        public bool Func_CarrierClampOpen()
        {
            bool bRet = false;
            CIO.It.Set_Y(eY.OFFP_Carrier_Clamp_Close, false);
            CIO.It.Set_Y(eY.OFFP_Carrier_Clamp_Open , true );

            bRet = ( CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open) &&
                    !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close));

            return bRet;
        }
        // syc end

        /// <summary>
        /// 버큠.  true:On false:Off
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_Vacuum(bool bOn)
        {
            CIO.It.Set_Y(eY.OFFP_Vacuum1, (CDataOption.IsRevPicker) ? !bOn : bOn);
            CIO.It.Set_Y(eY.OFFP_Vacuum2, (CDataOption.IsRevPicker) ? !bOn : bOn);

            return CIO.It.Get_X(eX.OFFP_Vacuum);
        }

        /// <summary>
        /// 이젝트.  true:On false:Off
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_Eject(bool bOn)
        {
            return CIO.It.Set_Y(eY.OFFP_Ejector, bOn);
        }

        /// <summary>
        /// 드레인.  true:On false:Off
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_Drain(bool bOn)
        {
            return CIO.It.Set_Y(eY.OFFP_Drain, bOn);
        }

        /// <summary>
        /// 피커 0도로 회전
        /// </summary>
        /// <returns></returns>
        public bool Func_Picker0()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFP_Rotate90, false);
            CIO.It.Set_Y(eY.OFFP_Rotate0 , true );
            bRet = (CIO.It.Get_X(eX.OFFP_Rotate0) && !CIO.It.Get_X(eX.OFFP_Rotate90));

            return bRet;
        }

        /// <summary>
        /// 피커 90도로 회전
        /// </summary>
        /// <returns></returns>
        public bool Func_Picker90()
        {
            bool bRet = false;

            CIO.It.Set_Y(eY.OFFP_Rotate0 , false);
            CIO.It.Set_Y(eY.OFFP_Rotate90, true );
            bRet = (!CIO.It.Get_X(eX.OFFP_Rotate0) && CIO.It.Get_X(eX.OFFP_Rotate90));

            return bRet;
        }

        // 2022.03.09 lhs Start
        /// <summary>
        /// [Btm Cleaner] Spray Nozzle Forward
        /// </summary>
        /// <returns></returns>
        public bool Func_SprayNzlForward()
        {
            bool bRet;

            CIO.It.Set_Y(eY.DRY_ClnBtmNzlForward, true); // Forward
            bRet = (!CIO.It.Get_X(eX.DRY_ClnBtmNzlBackward) && CIO.It.Get_X(eX.DRY_ClnBtmNzlForward));

            return bRet;
        }

        /// <summary>
        /// [Btm Cleaner] Spray Nozzle Backward
        /// </summary>
        /// <returns></returns>
        public bool Func_SprayNzlBackward()
        {
            bool bRet;

            CIO.It.Set_Y(eY.DRY_ClnBtmNzlForward, false); // Backward
            bRet = (CIO.It.Get_X(eX.DRY_ClnBtmNzlBackward) && !CIO.It.Get_X(eX.DRY_ClnBtmNzlForward));

            return bRet;
        }

        /// <summary>
        /// [Btm Cleaner] Water : true = On, false = Off
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_BtmClnFlow(bool bOn)
        {
            bool bRet;
            CIO.It.Set_Y(eY.DRY_ClnBtmFlow, bOn);   
            bRet = CIO.It.Get_X(eX.DRY_ClnBtmFlow);
            return bRet;
        }
        
        /// <summary>
        /// [Btm Cleaner] Air Blow : true = On, false = Off
        /// </summary>
        /// <param name="bOn"></param>
        /// <returns></returns>
        public bool Func_BtmClnAirBlow(bool bOn)
        {
            return CIO.It.Set_Y(eY.DRY_ClnBtmAirBlow, bOn);
        }
        // 2022.03.09 lhs End

        // 2022.07.04 lhs Start : OffP I/O 타입 IV 프로그램 설정
        /// <summary>
        /// /// IV의 Program 번호 설정 : bit 2개 사용 (000~003)
        /// </summary>
        /// <param name="nPrgNum"></param>
        public void Func_SetIVProgram(int  nPrgNum)
        {
            bool bit0 = ((nPrgNum & 0x01) == 1) ? true : false;
            bool bit1 = ((nPrgNum & 0x02) == 2) ? true : false;

            CIO.It.Set_Y(eY.OFFP_IV2_ProgBit0, bit0);
            CIO.It.Set_Y(eY.OFFP_IV2_ProgBit1, bit1);
        }

        /// <summary>
        /// IV의 Program 번호 얻기 : bit 2개 사용 (000~003)
        /// </summary>
        /// <returns></returns>
        public int Func_GetIVProgram()
        {
            bool bit0 = CIO.It.Get_Y(eY.OFFP_IV2_ProgBit0);
            bool bit1 = CIO.It.Get_Y(eY.OFFP_IV2_ProgBit1);

            int nProgNum = 0;
            if (bit0) { nProgNum |= 0x01; }
            if (bit1) { nProgNum |= 0x02; }

            return nProgNum;
        }
        // 2022.07.04 lhs End

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

            if      (pCtrlIV2.RcvSetProgramNo == IV2Result.OK) { iRet =  1; }
            else if (pCtrlIV2.RcvSetProgramNo == IV2Result.NG) { iRet =  0; }
            else                                               { iRet = -1; }

            return iRet;
        }


        /// <summary>
        /// Grind Part X, Y, Z Axes Check
        /// 그라인딩 파트의 X, Y, Z축 상태 체크
        /// </summary>
        /// <param name="bHD"></param>
        /// <returns></returns>
        public bool Chk_Axes(bool bHD = true)
        {
            int iRet = 0;

            iRet = CMot.It.Chk_Rdy(m_iX, bHD);
            if (iRet != 0)
            {

                CErr.Show(eErr.OFFLOADERPICKER_X_NOT_READY);
                _SetLog("Error : X axis not ready.");

                return true;
            }

            iRet = CMot.It.Chk_Rdy(m_iZ, bHD);
            if (iRet != 0)
            {
                CErr.Show(eErr.OFFLOADERPICKER_Z_NOT_READY);
                _SetLog("Error : Z axis not ready.");

                return true;
            }

            return false;
        }

        /// <summary>
        /// 자재가 흡착 상태인지 확인
        /// </summary>
        /// <returns></returns>
        public bool Chk_Strip()
        {
            //210909 syc : 2004U st
            // 4U : 케리어 pick -> Vac
            // 4U : Unit Pick -> Gripper
            if (CDataOption.Use2004U)
            {
                return ((CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) && !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open)) || CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect));
            }
            else
            {
                return CIO.It.Get_X(eX.OFFP_Vacuum);
            }            
        }

        // 2022.08.10 lhs Start :
        // Pick 할 때 Vacuum Check(=bxVacChk) 시 에러 메시지 : 세분화
        private void ShowVacChkErr_Pick_2004U()
        {
            bool bClamped = (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) && !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));
            bool bDetected = CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect);

            if (!bClamped)
            {
                CErr.Show(eErr.OFFLOADERPICKER_CLAMP_CHECK_ERROR);  // 기존 코드
                _SetLog("Error : Clamp check fail.");
            }
            if (!bDetected)
            {
                CErr.Show(eErr.OFFLOADERPICKER_CARRIER_DETECT_ERROR);
                _SetLog("Error : Carrier not detected.");
            }
        }

        // 2022.08.10 lhs Start :
        // Place 할 때 Vacuum Check(=bxVacChk) 시 에러 메시지 : 세분화
        private void ShowVacChkErr_Place_2004U()
        {
            bool bClamped = (CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close) && !CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Open));
            bool bDetected = CIO.It.Get_X(eX.OFFP_Picker_Carrier_Detect);
            bool bVacuumOn = CIO.It.Get_X(eX.OFFP_Vacuum);

            if (!bClamped)
            {
                CErr.Show(eErr.OFFLOADERPICKER_CLAMP_CHECK_ERROR);  // 기존 코드
                _SetLog("Error : Clamp check fail.");
            }
            if (!bDetected)
            {
                CErr.Show(eErr.OFFLOADERPICKER_CARRIER_DETECT_ERROR);
                _SetLog("Error : Carrier not detected.");
            }
            if (!bVacuumOn)
            {
                CErr.Show(eErr.OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR);   // 기존 코드
                _SetLog("Error : Cover Vacuum fail.");
            }
        }
    }
}
