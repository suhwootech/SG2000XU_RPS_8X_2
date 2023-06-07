using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using SECSGEM;

using SG2000X.Gaus.Log;     // for Log

using System.IO;            // 2021.05.28 SungTae : [추가]
using System.Text;          // 2021.05.28 SungTae : [추가]
using System.Linq;          // 2022.02.08 SungTae : [추가]

namespace SG2000X
{
    // 2021-05-06, jhLee : Muliti-LOT, LOT 정보를 관리하기위한 class
    public class CMultiLotMgr
    {
        CGxLog log = new CGxLog();
        CGxLog Report = new CGxLog();

        CGxLog GetLogPtr() { return log; }             // Log 객체 조회

        // Multi-LOT 처리를 위한 복수개의 Lot 정보 관리 필요
        public List<TLotInfo> ListLOTInfo = new List<TLotInfo>();               // LOT 정보를 관리하기 위한 List

        public string m_sReadLOT_T = string.Empty;              // 읽은 carrier list - 임시 저장
        
        public bool DataUpdateFlag { get; set; } = true;                        // 내용이 변경되어 화면에 표시를 해야하는가 ?
        public bool ListUpdateFlag { get; set; } = true;                        // Lot 구성 요소가 변경되었다.

        public bool IsLightCurtailDetect { set; get; }          // Loader부 Magazine이 준비가 되었는지 체크할 필요가 있는지 표식을 남긴다.

        public bool DeviceInit { get; set; } = false;                           // 해당 Recipe를 처음 적용시 수행을 하기 위한 설비 초기화 Flag,// 초기화나 Recipe 변경시 false로 수행

        public string LoadingLotName { get; set; } = "";        // 현재 투입중인 LOT 이름 (간편한 접근을 위한 Property)
        public string UnloadingLotName { get; set; } = "";      // 현재 배출중인 LOT 이름 (간편한 접근을 위한 Property)

        public int MaxEmptySlot { get; set; } = 3;      // LOT을 분리하는 연속 Empty Slot의 수
        public int MaxLoadMgzTry { get; set; } = 3;       // LOT을 분리하는 Magazine Pickup 연속 실패 횟수, 
        public int EmptySlotCnt { get; set; } = 0;      // 연속해서 비어있는 Slot 카운터
        public int LoadMgzTryCnt { get; set; } = 0;       // 연속해서 Magazine을 Pickup 하지 못한 카운터
        public bool MgzPickupFlag { get; set; } = true;         // magazine Pickup을 시도하라고하는 Flag, On-Loader Light-Curtain 감지시 Set 된다.
        public int LoadMgzSN { get; set; } = 0;             // 투입되는 Magazine의 연속 번호, 투입된 Strip의 투입 LOT 순서를 판펼하기 위해 사용

        public ELotUserConfirm UserConfirmReply { get; set; } = ELotUserConfirm.eNone;  // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻고 답을 받는다.
        public string UserConfirmMsg { get; set; } = "";    // LOT 종료 조건에 해당될때 사용자에게 투입종료 여부를 묻기위한 메세지
        public ELotEndMode UserConfirmNextMode { get; set; } = ELotEndMode.eEmptySlot;   // 작업자가 계속 투입을 선택하였을때 변경시킬 LOT 종료 모드

        // 2022.04.14 SungTae Start : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련
        // Auto-In 시 Abnormal 상황에 대한 Track Out 확인용 Flag
        public eTrackOutFlag IsLoaderTrackOutFlag	{ get; set; } = eTrackOutFlag.Valid;     // -1 : Error(Abnormal State), 0 : Valid(Normal State)
        public eTrackOutFlag IsUnloaderTrackOutFlag	{ get; set; } = eTrackOutFlag.Valid;     // -1 : Error(Abnormal State), 0 : Valid(Normal State)
        // 2022.04.14 SungTae End

        public bool IsLotCloseableSlot { get { return (EmptySlotCnt >= MaxEmptySlot); } }    // 연속 빈 Slot으로 LOT이 종료되는 조건인가 ?
        public bool IsLotCloseableMgz { get { return (LoadMgzTryCnt >= MaxLoadMgzTry); } }    // 연속 Magazine Pickup 실패로 LOT이 종료되는 조건인가 ?

        public bool IsForceUnloadPlace { get; set; } = false;       // Unloading Magazine을 강제로 Place 시켜야하는가 ? LOT 정보를 강제로 삭제시 적용된다.
        
        // .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..

        public CMultiLotMgr()          // 생성자
        {
            log.SetFileName(@"D:\Log_SG2X", "MultiLOT", "LOT_Mgr");           // Log 경로 및 파일 이름 지정
            Report.SetFileName(@"D:\Log_SG2X", "MultiLOT", "LOT_Report");       // Report Log 경로 및 파일 이름 지정
            Report.m_sHeader = "LOT Name,Start Time,End Time,Mgz Qty,In Qty,Out Qty";  // Report Header 지정
            Report.m_bTimePrint = false;                            // 기록시 시간을 출력하지 않는다.
        }


        //
        // 현재 투입되고있는 LOT을 Close(투입종료) 해야하는 조건인가 ? 
        //
        // [out] int nMode : 투입중인 LOT의 투입종료 조건 모드 설정값
        // [out] string sMsg : 투입 종료 원인
        //
        // return : -1 : 투입을 지속해야한다.
        //          0 ~ : 지정된 원인으로 투입을 종료해야한다.
        //
        public int GetLotCloseCheck(ref int nMode, ref string sMsg)
        {
            nMode = 0;          // 기본값 치환
            sMsg = "";

            if (LoadingLotName == "") return -1;             // Loading중인 LOT이 없다면 투입 종료도 없다.

            TLotInfo rLotInfo = null;
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);
            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                rLotInfo = ListLOTInfo[nIdx];                   // LOT 정보 취득 
                nMode = (int)rLotInfo.nLotEndMode;              // LOT 투입 종료 모드 대입
            }
            else
            {
                return -1;                                   // 투입중인 LOT 정보를 취득하지 못하면 투입종료도 없다.
            }

            // 1. 연속 빈 Slot이 지정된 수량만큼 나왔는가 ? : 기본 종료 조건
            if (EmptySlotCnt >= MaxEmptySlot)
            {
                sMsg = $"{EmptySlotCnt} consecutive empty slots occurred.";
                return (int)ELotEndMode.eEmptySlot;        // 연속 빈 Slot 으로 구분 (기본), // 투입 중지를 해야한다.
            }

            // 2. Strip 수량에 대한 제한이 되어있는 조건이라면, Strip을 지정된 수량만큼 투입하였는가 ?
            if ((rLotInfo.nLotEndMode == ELotEndMode.eStripQty) || (rLotInfo.nLotEndMode == ELotEndMode.eStripMgzQty))
            {
                if ((rLotInfo.iTotalStrip > 0) && (rLotInfo.iTInCnt >= rLotInfo.iTotalStrip))
                {
                    sMsg = $"We have put all {rLotInfo.iTInCnt} strips that are the specified quantity";
                    return (int)ELotEndMode.eStripQty;          // 지정된 Strip 수량 투입시 LOT 종료
                }
            }

            // 3. Magazine 수량에 대한 제한이 되어있는 조건이라면, Magazine을 지정된 수량만큼 투입하였는가 ?
            if ((rLotInfo.nLotEndMode == ELotEndMode.eStripQty) || (rLotInfo.nLotEndMode == ELotEndMode.eStripMgzQty))
            {
                if ((rLotInfo.iTotalMgz > 0) && (rLotInfo.nInMgzCnt > rLotInfo.iTotalMgz))
                {
                    sMsg = $"We have put all {rLotInfo.nInMgzCnt} magazines that are the specified quantity";
                    return (int)ELotEndMode.eMgzQty;            // 지정된 Magazine 수량 투입시 LOT 종료
                }
            }
            
            // 투입 종료 조건이 아니다.
            return -1;
        }


        //
        // (현재 투입중인 LOT의) 지정된 LOT 투입 종료 모드를 조회한다.
        //
        public ELotEndMode GetLotEndMode()
        {
            if (LoadingLotName == "") return ELotEndMode.eEmptySlot;    // 투입중인 LOT이 없다면 기본값을 되돌린다.

            TLotInfo rLotInfo = null;
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);
            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                rLotInfo = ListLOTInfo[nIdx];                   // LOT 정보 취득 
                return rLotInfo.nLotEndMode;            // 지종된 LOT 투입 종료 모드 회신
            }

            return ELotEndMode.eEmptySlot;    // 투입중인 LOT의 정보가 없다면 기본값을 되돌린다.
        }


        //
        // (현재 투입중인 LOT의) LOT 투입 종료 모드로 지정한다.
        //
        public bool SetLotEndMode(ELotEndMode nMode)
        {
            if (LoadingLotName == "") return false;    // 투입중인 LOT이 없다면 설정 불가

            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);
            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                ListLOTInfo[nIdx].nLotEndMode = nMode;                   // LOT 투입 종료 모드 지정
                return true;
            }

            return false;       // 지정 실패
        }



        // 
        // 지정 Strip 투입 수량만큼 투입이 되어 LOT 투입 종료가 가능한가 ?
        //
        public bool CheckStripInputQty()
        {
            if (LoadingLotName == "")   return false;             // Loading중인 LOT이 없다면 투입 종료도 없다.

            TLotInfo rLotInfo = null;
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);

            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                rLotInfo = ListLOTInfo[nIdx];                   // LOT 정보 취득 
            }
            else
            {
                return false;                                   // 투입중인 LOT 정보를 취득하지 못하면 투입종료도 없다.
            }

            // 2. Strip 수량에 대한 제한이 되어있는 조건이라면, Strip을 지정된 수량만큼 투입하였는가 ?
            if ((rLotInfo.nLotEndMode == ELotEndMode.eStripQty) || (rLotInfo.nLotEndMode == ELotEndMode.eStripMgzQty))
            {
                if ((rLotInfo.iTotalStrip > 0) && (rLotInfo.iTInCnt >= rLotInfo.iTotalStrip))
                {
                    // sMsg = $"We have put all {rLotInfo.iTInCnt} strips that are the specified quantity";
                    return true;                                // 지정된 Strip 수량 투입시 LOT 종료 가능
                }
            }

            return false;
        }


        //
        // 투입해야하는 Strip이 얼마나 남아있는가 ?
        //
        public int GetStripInputRemain()
        {
            if (LoadingLotName == "") return 0;             // Loading중인 LOT이 없다면 투입 종료도 없다.

            TLotInfo rLotInfo = null;
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);
            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                rLotInfo = ListLOTInfo[nIdx];                   // LOT 정보 취득 
            }
            else
            {
                return 0;                                   // 투입중인 LOT 정보를 취득하지 못하면 투입종료도 없다.
            }


            // Strip 수량에 대한 제한이 되어있는 조건이라면, 투입해야하는 수량은 몇개인가 ?
            if ((rLotInfo.nLotEndMode == ELotEndMode.eStripQty) || (rLotInfo.nLotEndMode == ELotEndMode.eStripMgzQty))
            {
                if ((rLotInfo.iTotalStrip > 0) )
                {
                    return (rLotInfo.iTotalStrip - rLotInfo.iTInCnt);
                }
            }

            return 0;
        }


        // 
        // 지정 Magazine 투입 수량만큼 투입이 되어 LOT 투입 종료가 가능한가 ?
        // Magazine을 Place하기전에 조회하며, 현재 투입중인 Magazine 까지 수량에 포함된다.
        //
        public bool CheckMagazineInputQty()
        {
            // 2021.08.26 SungTae Start : [수정] REMOTE 상태에서는 각 MGZ의 첫 STRIP에 대한 Carrier Verification 전까지는 Lot Name이 NULL일 수 있어서 조건 추가
            if (CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
            {
                if (LoadingLotName == "") return false;             // Loading중인 LOT이 없다면 투입 종료도 없다.
            }
            else
            {
                if (LoadingLotName == "") return true;
            }
            // 2021.08.26 SungTae End

            TLotInfo rLotInfo = null;
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);
            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                rLotInfo = ListLOTInfo[nIdx];                   // LOT 정보 취득 
            }
            else
            {
                return false;                                   // 투입중인 LOT 정보를 취득하지 못하면 투입종료도 없다.
            }


            // Magazine 수량에 대한 제한이 되어있는 조건이라면, Magazine을 지정된 수량만큼 투입하였는가 ?
            if ((rLotInfo.nLotEndMode == ELotEndMode.eMgzQty) || (rLotInfo.nLotEndMode == ELotEndMode.eStripMgzQty))
            {
                if ((rLotInfo.iTotalMgz > 0) && (rLotInfo.nInMgzCnt >= rLotInfo.iTotalMgz))
                {
                    // sMsg = $"We have put all {rLotInfo.nInMgzCnt} magazines that are the specified quantity";
                    return true;                                // 지정된 Magazine 수량 투입시 LOT 종료
                }
            }

            return false;
        }


        public int GetListIndexNum(string sLotName)
        {
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);
            
            return nIdx;
        }


        //
        // 지정 LOT에 포함된 Strip이 설비내에 존재하는가 ? 수량을 되돌린다.  (지정 LOT의 종료를 조사)
        //
        // return 0 : 지정 LOT에 해당되는 Strip이 존재하지 않는다.
        //        1 ~ : 지정 LOT에 해당되는 Strip의 수량
        //
        public int GetLOTStripCount(string sLotName)
        {
            int nCount = 0;

            // IRail ~ DryZone 까지의 Strip 조사
            // 2021-07-29, jhLee : On-Loader는 화면에 데이터 표시용으로만 사용되므로 실제 설비내 Strip 유무 조사시에는 포함시키지 않도록 한다. 
            for (int i = (int)EPart.INR; i <= (int)EPart.DRY; i++)
            {
                if (CData.Parts[i].bExistStrip) // Strip이 존재하고
                {
                    // lhs 디버깅
                    log.Write($" GetLOTStripCount() :  Exist Strip....Part[{i}] LOT Name: [{CData.Parts[i].sLotName}]");

                    // 지정한  LOT Name을 가진다면
                    if (string.Compare(CData.Parts[i].sLotName, sLotName, true) == 0)
                    {
                        nCount++;               // 지정 LOT의 Strip 수량 증가
                        log.Write($" .Strip exist in Parts[{i}]");
                    }
                }
            }

            return nCount;
        }


        //
        // 현재 Loading중인 LOT에서 투입처리된 Strip의 수량을 조회한다.
        // 투입된 Strip이 없다면 LOT 분리 조건이 되더라도 투입 종료를 하지 않도록 한다.
        //
        public int GetInputCountLoadLot(string sLotName)
        {

            // 지정 LOT이 존재하는지 검색
            int nIdx = ListLOTInfo.FindIndex(x => (string.Compare(x.sLotName, sLotName, true) == 0));

            // 지정 LOT 정보를 찾았다.
            if (nIdx >= 0)
            {
                return ListLOTInfo[nIdx].iTInCnt;                 // 해당 LOT에 투입된 Strip 수량 회신
            }

            return 0;
        }


        //
        // 설비내 존재하는 Strip들의 LOT 종류를 조회한다. 
        //
        // return 0 : 진행중인 LOT이 없다.
        //        1 ~ : 진행중인 LOT의 수량
        //
        public int GetExistLotCount()
        {
            int nCount = 0;
            string sPrevLotName = "";

            // ON-Loader ~ DryZone 까지의 Strip 조사
            for (int i = (int)EPart.ONL; i <= (int)EPart.DRY; i++)
            {
                if (CData.Parts[i].bExistStrip)     // Strip이 존재하고
                {
                    // LOT Name이 다르다면 
                    if (string.Compare(CData.Parts[i].sLotName, sPrevLotName, true) != 0)
                    {
                        nCount++;                                       // LOT 종류 수량 증가
                        sPrevLotName = CData.Parts[i].sLotName;         // 비교대상 LOT 이름 갱신
                    }
                }
            }

            return nCount;
        }
        

        // 설비 내 존재하는 LOT 종류를 조회한다. 
        //
        // return false : 진행 중인 LOT과 다른 LOT이다.
        //        true  : 진행 중인 LOT이다
        //
        public bool CheckExistLOT(string sLotName)
        {
            bool bRet = false;

            if (string.Compare(CData.Parts[(int)EPart.ONP].sLotName, sLotName, true) == 0)
            {
                bRet = true;    // LOT Name이 같다
            }
            else
            {
                bRet = false;   // LOT Name이 다르다 
            }

            return bRet;
        }


        //
        // Complete 되지않은 LOT의 수량을 조회한다. (모든 LOT 처리 여부를 파악하기 위해 사용)
        //
        public int GetNotCompleteCount()
        {
            int nCount = 0;

            for (int i = 0; i < ListLOTInfo.Count; i++)
            {
                if (ListLOTInfo[i].nState != ELotState.eComplete)           // 완료되지 않은 LOT의 수량 누적
                {
                    nCount++;
                }
            }

            return nCount;
        }


        //
        // 현재 배출중인 LOT을 종료시켜야하는 조건인가 ?
        //
        public bool IsUnloadFinishable()
        {
            // 현재 배출중인 LOT이 존재하는가 ?
            if (UnloadingLotName == "") return false;                      // 배출중이 아니다

            // 1. 지정 LOT이 이미 On-Loader에서 투입을 종료했어야 한다. RUN 중이 아니어야 한다.
            // On-Loader에서 아직 RUN 중이라고 지정되었다면 이후에라도 후속으로 Strip이 들어올 확률이 있어서 체크가 필요하다.
            //
            // 현재 Loading 중인 LOT과 동일한 LOT인가 ?
            if (UnloadingLotName == LoadingLotName)
            {
                if (GetLotState(LoadingLotName) == ELotState.eRun)         // 아직 투입 동작 중이라면 LOT을 종료할 수 없다.
                {
                    return false;
                }
            }

            // 투입과 배출 진행 LOT이 서로 다르거나 동일하더라도 투입을 마친경우,
            // 설비내 지정 LOT에 속하는 Strip이 존재하지 않는경우 LOT을 종료할 수 있다.
            int nCount = GetLOTStripCount(UnloadingLotName);            // 현재 배출중인 LOT의 잔여 Strip 수량

            return (nCount <= 0);                                       // 잔여 Strip이 없다.
        }


        //
        // 지정 LOT이 종료되었는가 ? 
        // ( Off-Loader에서 Magazine 배출을 위한 조회때 사용 )
        //
        public bool IsLotComplete(string sLotName)
        {
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                return (ListLOTInfo[nIdx].nState == ELotState.eComplete);           // 해당 LOT이 종료 되었는가 ? 종료는 DryZone에서 종료 처리 해준다.
            }

            return false;
        }


        //
        // Loading 중인 LOT의 이름을 초기화 한다.
        //
        public void ClearLoadingLot()
        {
            log.Write($"Clear Loading LOT : {LoadingLotName}");

            LoadingLotName = "";            // Unloading 중인 LOT을 초기화 한다.
            DataUpdateFlag = true;              // Data 내용 변경
        }


        //
        // Unloading 중인 LOT의 이름을 초기화 한다.
        //
        public void ClearUnloadingLot()
        {
            log.Write($"Clear Unloading LOT : [{UnloadingLotName}]");

            UnloadingLotName = "";              // Unloading 중인 LOT을 초기화 한다.
            DataUpdateFlag   = true;              // Data 내용 변경
        }


        // 현재 배출중인 

        // 현재 Slot이 빈 슬롯임을 표시, 연속 빈 slot 을 감지하기 위함
        //
        // [in] bool bIsEmpty : 현재 Slot이 빈 Slot인지 여부 : true : 빈슬롯, false : 제품이 들어있는 슬롯
        //
        public void SetLoadEmptySlot(bool bIsEmpty)
        {
            if (bIsEmpty)                   // 비어있는 Slot인가 ?
            {
                EmptySlotCnt++;             // 연속해서 비어있는 Slot 수량 증가

                log.Write($"Push empty slot, LOT:{LoadingLotName}, Empty count:{EmptySlotCnt}");
            }
            else
            {
                // 정상적인 Strip Loading이면
                if (EmptySlotCnt > 0) log.Write($"Push Non-Empty slot : {EmptySlotCnt} -> 0 Clear count");

                EmptySlotCnt = 0;           // 연속해서 비어있는 Slot 수량 초기화

                int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);

                if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
                {
                    ListLOTInfo[nIdx].iTInCnt++;                      // 투입 수량 증가
                    log.Write($"Strip load LOT : [{ListLOTInfo[nIdx].sLotName}], Count:{ListLOTInfo[nIdx].iTInCnt}");

                    //--------------
                    // lhs 디버깅
                    for (int i = (int)EPart.INR; i <= (int)EPart.DRY; i++)
                    {
                        if (CData.Parts[i].bExistStrip) // Strip이 존재하고
                        {
                            log.Write($" SetLoadEmptySlot() : Exist Strip....Part[{i}] LOT Name: [{CData.Parts[i].sLotName}]");
                        }
                    }
                    //--------------
                }
            }

            DataUpdateFlag = true;                              // 내용이 변경되어 화면에 표시를 해야한다.
        }


        // 연속 빈 Slot 감지 카운터를 Clear 한다.
        //
        public void ClearLoadEmptySlot()
        {
            EmptySlotCnt = 0;
        }



        // Magazine Pickup 결과 지정, 연속 Magzine pickup 실패를통한 LOT Close 여부를 감지하기 위함
        //
        // [in] bool bIsSuccess : 새로운 Magazine을 Pickup 하였는지 여부, true : Pickup 성공, false : Pickup 실패
        //
        public void SetLoadMgzPickup(bool bIsSuccess)
        {
            if (bIsSuccess)                 // Magazine pickup에 성공하였는가 ?
            {
                LoadMgzTryCnt = 0;            // Pickup에 실패한 Count clear
                LoadMgzSN++;                      // Magazine serial 번호 Clear

                if (LoadingLotName != "")
                {
                    log.Write($"Loader pickup magazine LOT:{LoadingLotName}, MgzSN:{LoadMgzSN}");
                }
                else
                {
                    log.Write($"Loader pickup new magazine, MgzSN:{LoadMgzSN}");
                }
            }
            else
            {
                // 지정 횟수를 초과하여도 새로운 Magazine을 Pickup 하지 못하였다.
                LoadMgzTryCnt++;

                if (LoadingLotName != "")
                {
                    log.Write($"Load fail pickup new magazine in LOT:{LoadingLotName}], TryCount:{LoadMgzTryCnt}");
                }
                else
                {
                    log.Write($"Load fail pickup new magazine, TryCount:{LoadMgzTryCnt}");
                }
                // 이후 On-Loader에서 시도 횟수를 판단하여 LOT를 분리하도록 한다.
            }

            DataUpdateFlag = true;                              // 내용이 변경되어 화면에 표시를 해야한다.
        }



        //
        // Strip을 배출하였다. 
        //
        // [in] string  sLotName : 배출하는 Strip의 LOT 이름
        //
        public bool SetUnloadStrip(string sLotName)
        {
            // 배출 중인 LOT이 존재한다면 

            if (UnloadingLotName == "")     // 아직 배출대상 LOT이 아직 배정되지 않았다.
            {
                // 지정 LOT이 존재하는지 검색
                int nIdx = ListLOTInfo.FindIndex(x => (string.Compare(x.sLotName, sLotName, true) == 0));

                // 지정 LOT 정보를 찾았다.
                if (nIdx >= 0)
                {
                    UnloadingLotName = sLotName;                              // 배출 작업중인 LOT Name 대입
                    log.Write($"Assign new Unload LOT: [{sLotName}]");

                    //--------------
                    // lhs 디버깅
                    for (int i = (int)EPart.INR; i <= (int)EPart.DRY; i++)
                    {
                        if (CData.Parts[i].bExistStrip) // Strip이 존재하고
                        {
                            log.Write($" SetUnloadStrip()  : UnloadingLotName == NO :  Exist Strip....Part[{i}] LOT Name: [{CData.Parts[i].sLotName}]");                          
                        }
                    }
                    //--------------
                }
                else
                {
                    log.Write($"* Unloading LOT not assigned, Not find LOT : [{sLotName}]");
                }
            }

            // 배출 처리중인 LOT이 배정되었다면 
            if (UnloadingLotName != "")
            {
                int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, UnloadingLotName, true) == 0);

                if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
                {
                    ListLOTInfo[nIdx].iTOutCnt++;                      // 배출 수량 증가

                    DataUpdateFlag = true;                              // 데이터 내용이 변경되었다.
                    log.Write($"Unloading Strip LOT : [{sLotName}], OutStripCount : {ListLOTInfo[nIdx].iTOutCnt}");


                    //--------------
                    // lhs 디버깅
                    for (int i = (int)EPart.INR; i <= (int)EPart.DRY; i++)
                    {
                        if (CData.Parts[i].bExistStrip) // Strip이 존재하고
                        {
                            log.Write($" SetUnloadStrip() : UnloadingLotName == OK : Exist Strip....Part[{i}] LOT Name: [{CData.Parts[i].sLotName}]");
                        }
                    }
                    //--------------

                    return true;    // 정상적인 처리
                }
                else
                {
                    log.Write($"* Exeption, SetUnloadStrip() UnloadingLotName [{UnloadingLotName}] is empty information");
                }
            }

            return false;           // 비정상 처리
        }

        // 현재 투입중인 LOT의 투입을 종료시킨다
        //
        // return true : 투입중이던 LOT을 종료시켰다.
        //          false : 투입 종료를 할 수가 없다 (투입중인 LOT이 없는경우에도 해당)
        //
        public bool SetCloseLoadLOT()
        {
            if (LoadingLotName != "")                               // 투입중인 LOT이 존재한다면
            {
                int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, LoadingLotName, true) == 0);

                if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
                {
                    ListLOTInfo[nIdx].dtClose = DateTime.Now;         // 투입이 종료된 시각
                    ListLOTInfo[nIdx].nState = ELotState.eClose;      // LOT 투입이 종료되었다.

                    log.Write($"Close loading magazine, LOT:{LoadingLotName}");

                    EmptySlotCnt = 0;           // 연속해서 비어있는 Slot 수량 초기화
                    LoadingLotName = "";        // 현재 투입중인 LOT 이름 초기화
                    LoadMgzTryCnt = 0;          // 투입 Magazine Pickup 시도수 초기화
                    DataUpdateFlag = true;      // 내용이 변경되어 화면에 표시를 해야한다.

                    return true;                // 투입중이던 LOT의 투입을 Close 하였다.
                }
                else
                {
                    // Logic error 예외 상황, 해당 Loading LOT의 정보가 없어도 투입을 종료한다.
                    LoadingLotName = "";        // 더이상 투입중인 LOT 정보가 없다.
                    EmptySlotCnt = 0;           // 연속해서 비어있는 Slot 수량 초기화

                    log.Write($"* Exeption, SetCloseLoadLOT() LoadingLotName [{LoadingLotName}] is empty information, Close loading LOT");
                }
            }
            else
            {
                log.Write($"* Exeption, SetCloseLoadLOT() Current IdxLoadLOT is empty.");
            }

            LoadMgzTryCnt = 0;                  // 투입 Magazine Pickup 시도수 초기화
            DataUpdateFlag = true;              // 내용이 변경되어 화면에 표시를 해야한다.

            return false;                       // 투입 종료할 내용이 없다.
        }


        //
        // 현재 Unloading중인 LOT을 종료시킨다 (배출 작업을 마친 LOT의 종료 처리, CSQ8_OfL.cs 에서 Mgz Place후 호출한다.)
        //
        public void SetCompleteUnloadLOT(string sLotName)
        {
            //old if (UnloadingLotName != "")     //  배출대상 LOT
            if (sLotName != "")     //  완료대상 LOT
            {
                // 지정 LOT이 존재하는지 검색
                int nIdx = ListLOTInfo.FindIndex(x => (string.Compare(x.sLotName, sLotName, true) == 0));

                // 지정 LOT 정보를 찾았다.
                if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))                   // Index 정합성 check
                {
                    ListLOTInfo[nIdx].dtEnd = DateTime.Now;             // LOT이 종료된 시각
                    ListLOTInfo[nIdx].nState = ELotState.eComplete;     // LOT 처리가 종료되었다.
                    ListLOTInfo[nIdx].bLotOpen = false;                 // LOT End 지정
                    ListLOTInfo[nIdx].bLotEnd = true;

                    // "LOT Name,Start Time,End Time,Mgz Qty,In Qty,Out Qty";  // Report Header
                    Report.Write($"{ListLOTInfo[nIdx].sLotName},{ListLOTInfo[nIdx].dtOpen.ToString("HH:mm:ss.fff")},{ListLOTInfo[nIdx].dtEnd.ToString("HH:mm:ss.fff")},{ListLOTInfo[nIdx].nInMgzCnt},{ListLOTInfo[nIdx].iTInCnt},{ListLOTInfo[nIdx].iTOutCnt}");

                    log.Write($"Complete LOT : [{ListLOTInfo[nIdx].sLotName}]");

                    //// 지정  Index의 LOT 정보를 삭제한다   -> DeleteCompleteLOT()으로 이동
                    //ListLOTInfo.RemoveAt(IdxUnloadLOT);
                }
                else
                {
                    // Logic error 예외 상황, 해당 배출되는 LOT의 정보가 없어도 투입을 종료한다.
                    log.Write($"* Exeption, SetCompleteUnloadLOT() IdxUnloadLOT {nIdx} is empty information, Close Unloading LOT");
                }

                // 배출중인 LOT 이름은 실제 Magazine이 Place 된 후 clear해준다.
                // UnloadingLotName = "";        // 현재 배출중인 LOT 이름 초기화

                DataUpdateFlag = true;        // 항목 구성 내용이 변경되어 화면에 갱신할 필요가 있다.
            }
            else
            {
                log.Write($"* Exeption, SetCompleteUnloadLOT() Current IdxUnloadLOT is empty.");
            }
        }



        // 새로운 Magazine이 투입되었다 (Pickup)
        // On-Loader에서 Pickup 동작 후에 호출하여 LOT 정보와 매칭시켜주도록한다.
        public bool SetNewLoadMgz(string sMgzID)
        {
            int nIdx = -1;

            if (!CData.Opt.bSecsUse)
            {
                if (LoadingLotName == "")           // 아직 투입처리중인 LOT이 존재하지 않는다면 투입대상 LOT이 아직 배정되지 않았다.
                {
                    // 대기중인 LOT이 존재하는지 검색
                    nIdx = ListLOTInfo.FindIndex(x => (x.nState == ELotState.eWait));

                    // 대기중인 LOT 정보를 찾았다.
                    if (nIdx >= 0)
                    {
                        ListLOTInfo[nIdx].nState = ELotState.eRun;       // 해당 LOT은 이제부터 Loading 동작임을 명시한다.
                        ListLOTInfo[nIdx].dtOpen = DateTime.Now;         // LOT이 Open된 시각 지정
                        ListLOTInfo[nIdx].bLotOpen = true;                 // LOT이 Open 되었다.

                        LoadingLotName = ListLOTInfo[nIdx].sLotName;        // 현재 투입중인 LOT 이름 지정

                        log.Write($"Assign new LOT : [{ListLOTInfo[nIdx].sLotName}], Wait -> Run state change");
                    }
                    else
                    {
                        //// 2021.08.26 SungTae Start : [추가] Remote 상태에서의 연속 LOT 처리 구분
                        //if (CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(JSCK.eCont.Remote))
                            log.Write($"Loading LOT not assigned, Not find next wait state LOT !");
                        //else
                        //    log.Write($"[REMOTE] Assign new LOT : [LOAD_NEW_LOT], Wait -> Run state change");
                        //// 2021.08.26 SungTae End
                    }
                }
                else
                {
                    // 지정 LOT이 존재하는지 검색
                    nIdx = ListLOTInfo.FindIndex(x => (string.Compare(x.sLotName, LoadingLotName, true) == 0));

                    log.Write($"Assign running LOT:{LoadingLotName}, Run state");
                }

                // 투입처리중인 LOT이 배정되었다면 
                if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))          // Index 정합성 check
                {
                    ListLOTInfo[nIdx].nInMgzCnt++;                      // 투입된 Magazine의 수량을 증가시킨다.
                    ListLOTInfo[nIdx].sGMgzId = sMgzID;                 // 투입된 Magazine의 ID를 지정한다.

                    DataUpdateFlag = true;                              // 내용이 변경되어 화면에 표시를 해야한다.

                    log.Write($"Pickup new magazine LOT:{ListLOTInfo[nIdx].sLotName}, MgzCount:{ListLOTInfo[nIdx].nInMgzCnt}, StripCount:{ListLOTInfo[nIdx].iTInCnt}");
                    return true;    // 정상적인 처리
                }
            }
            else
            {
                //// 지정 LOT이 존재하는지 검색
                //nIdx = ListLOTInfo.FindIndex(x => (string.Compare(x.sLotName, LoadingLotName, true) == 0));

                //log.Write($"Assign running LOT:{LoadingLotName}, Run state");

                log.Write($"[REMOTE] Assign new LOT : [LOAD_NEW_LOT], Wait -> Run state change");
            }

            //// 투입처리중인 LOT이 배정되었다면 
            //if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))          // Index 정합성 check
            //{
            //    ListLOTInfo[nIdx].nInMgzCnt++;                      // 투입된 Magazine의 수량을 증가시킨다.
            //    ListLOTInfo[nIdx].sGMgzId = sMgzID;                 // 투입된 Magazine의 ID를 지정한다.

            //    DataUpdateFlag = true;                              // 내용이 변경되어 화면에 표시를 해야한다.

            //    log.Write($"Pickup new magazine LOT:{ListLOTInfo[nIdx].sLotName}, MgzCount:{ListLOTInfo[nIdx].nInMgzCnt}, StripCount:{ListLOTInfo[nIdx].iTInCnt}");
            //    return true;    // 정상적인 처리
            //}

            return false;           // 비정상 처리
        }


        // 관리되고있는 LOT 정보의 수량을 
        public int GetCount()
        {
            return ListLOTInfo.Count;                                               // 입력된 LOT 정보 수량
        }

        // 모든 LOT 정보를 제거한다.
        public void Clear()
        {
            ListLOTInfo.Clear();
        }



        // 지정 Index의 LOT 정보를 조회한다.
        public TLotInfo GetLotInfo(int nIdx)
        {
            if ((nIdx >= 0) && (nIdx < ListLOTInfo.Count))
            {
                return ListLOTInfo[nIdx];
            }

            return null;                           // 찾지를 못하였다.
        }



        // 화면에 각 LOT를 구분해주기위해 색으로 구분해줄 색 구분자 조회
        public int GetNextType()
        {
            int nNext = 0;
            int nIdx = 0;

                // 마지막에 적용된 Type을 조회하여 그 다음 Type으로 지정한다.
                nIdx = ListLOTInfo.Count - 1;

                if (nIdx >= 0)                        // LOT 정보가 존재
                {
                    nNext = (ListLOTInfo[nIdx].nType + 1) % 3;          // 전체 3가지 중 하나
                }
                else
                {
                    nNext = 0;
                }
            
            return nNext;
        }


        // 지정 이름을 가지는 LOT 를 추가한다 (GUI를 통해 작업자가 수작업으로 입력한다)
        public bool AddLot(string sLotName, int nMgzQty, int nStripQty)
        {
            // 동일한 이름을 가지는 LOT가 존재하지 않는다면 지정한 LOT를 추가한다.
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx >= 0) return false;            // 이미 존재하는 LOT 이름이다.

            // 새로이 LOT 정보 구조체를 구성
            TLotInfo rLotInfo = new TLotInfo();
            
            rLotInfo.sLotName    = sLotName;        // 지정한 LOT 이름을 대입한다.
            rLotInfo.iTotalStrip = nStripQty;       // 투입될 Strip 수량 설정
            rLotInfo.iTotalMgz   = nMgzQty;         // 투입될 Magzine 수량 설정
            rLotInfo.nInMgzCnt   = 0;               // 투입된 Magzine 수량 초기화

            if (CData.Opt.bSecsUse)
                rLotInfo.iTInCnt = 1;               // 투입된 Strip 수량 초기화
            else
                rLotInfo.iTInCnt = 0;               // 투입된 Strip 수량 초기화

            rLotInfo.iTOutCnt    = 0;               // 배출할 Strip 수량 초기화
            rLotInfo.i18PMeasuredCount = 0;         // 18 포인트 측정 (ASE-KR VOC)

            // 설정된 Magazine과 Strip의 수량에 따라 LOT 분리 방법을 전환한다.
            // 2가지 값이 모두 0으로 설정되었다면 
            if ( (nMgzQty <= 0) && (nStripQty <= 0) )
            {
                rLotInfo.nLotEndMode = ELotEndMode.eEmptySlot;          // 빈 슬롯 수량으로 LOT을 구분시킨다.
            }
            // 2가지 모두 입력되었다.
            else if ((nMgzQty > 0) && (nStripQty > 0))
            {
                rLotInfo.nLotEndMode = ELotEndMode.eStripMgzQty;       // Strip 수량우선으로 체크, 이후 Magazine 수량을 보조로 체크
            }
            // Magazine 수량을 지정하였다
            else if ((nMgzQty > 0) && (nStripQty <= 0))    
            {
                rLotInfo.nLotEndMode = ELotEndMode.eMgzQty;             // Magazine 수량으로 LOT을 구분시킨다.
            }
            // Strip 수량을 지정하였다
            else if ((nMgzQty <= 0) && (nStripQty > 0))                 
            {
                rLotInfo.nLotEndMode = ELotEndMode.eStripQty;           // Strip 수량으로 LOT을 구분시킨다.
            }

            // 2022.03.26 SungTae : [수정] UI Display 시 Mismatch 부분 수정
			//if (CData.Opt.bSecsUse && CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
            //if (!CData.Opt.bSecsUse)
            //{
                rLotInfo.nType      = GetNextType();                    // 색을 지정한다.
                rLotInfo.LotColor   = GetLotTypeColor(rLotInfo.nType);  // LOT 구분 색 지정
            //}

            // SPC 작성에 필요한 데이터들의 기본값을 대입해준다.
            rLotInfo.rSpcData.sLotName      = sLotName;
            rLotInfo.rSpcInfo.sLotName      = sLotName;
            rLotInfo.rSpcInfo.sDevice       = CData.Dev.sName;
            rLotInfo.rSpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
            rLotInfo.rSpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;

            ListLOTInfo.Add(rLotInfo);              // 관리용으로 데이터를 입력한다.

            ListUpdateFlag = true;                  // 항목 구성 내용이 변경되어 화면에 갱신할 필요가 있다.

            return true;
        }

        // 지정 구조체의 LOT 정보를 추가한다 (GUI를 통해 작업자가 수작업으로 입력한다)
        public bool AddLotInfo(ref TLotInfo rLotInfo)
        {
            rLotInfo.nType = GetNextType();     // 색을 지정한다.

            ListLOTInfo.Add(rLotInfo);          // 관리용으로 데이터를 입력한다.

            log.Write($"Add LOT Information, Name:{rLotInfo.sLotName}, MgzQty:{rLotInfo.iTotalMgz}, StripQty:{rLotInfo.iTotalStrip}");

            return true;
        }


        //
        // 설비내 Stirp을 제거하였을 경우 처리
        // 조건이 부합되는경우 LOT 을 Close 처리한다.
        //
        public void RemoveStrip(string sLotName)
        {
            // 지정 LOT이 실존하는 LOT 인가 ?
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx < 0)       // 존재하지 않는 LOT 이다.
            {
                log.Write($"* RemoveStrip({sLotName}), Missing Lot Name, Not exist LOT");
                return;
            }

            // 1. 지정 LOT 이 Close 될것인지 확인
            //   - 현재 Loading 중이거나 설비 내부에 잔여 Strip이 존재하는가 ? -> Close 불가
            int nCount = GetLOTStripCount(sLotName);            // 설비내 지정 LOT Strip의 수량 조사

            if (nCount > 0)
            {
                log.Write($"RemoveStrip({sLotName}), Exist {nCount} strips in EQ");
                return;
            }

            // 현재 Loading 작업 중이다.
            if ((sLotName == LoadingLotName) && (ListLOTInfo[nIdx].nState == ELotState.eRun))
            {
                log.Write($"RemoveStrip({sLotName}), Loading run state");
                return;
            }

            // 지정 LOT을 Complete 처리해준다.
            // 2. 지정 LOT Complete
                
            // 2022.05.18 SungTae Start : [수정] ASE-KR Multi-LOT 관련
            /*
             * Remote 상태에서 Auto Run 중 Alarm으로 인해 Strip 제거 시 현재 진행 중인 LOT을 "LOT END" 하여 Complete(LOT 정보 제거) 시킨다.
             * -> 이때 재시작 시 진행하던 LOT 정보가 List에 없기 때문에 Exception이 발생하여 P/G이 튕기면서 강제 종료됨.
             */
            if (!CData.Opt.bSecsUse)
            {
                SetCompleteUnloadLOT(sLotName);    //  지정 LOT을 종료시킨다  
                log.Write($"RemoveStrip({sLotName}), LOT Complete");

                //   - 현재 Unloading 중인가 ? -> Mgz Place 처리는 Off-Loader 시퀀스에서 처리
                //   - 현재 Unloading 중이 아니고 더이상 설비내에 잔여 Strip이 존재하지 않는다면 LOT 정보 삭제
                if (sLotName != UnloadingLotName)
                {
                    CData.bLotCompleteMsgShow = true;                   // LOT 종료 메세지를 보여주도록 한다. -> 메세지를 보여줘야 완료된 LOT 정보를 삭제할 수 있다.
                    log.Write($"RemoveStrip({sLotName}), Show LOT Complete Message");
                }
            }
            // 2022.05.18 SungTae End

            DataUpdateFlag = true;                              // 내용이 변경되어 화면에 표시를 해야한다.
        }


        // 2021-07-28, jhLee, On-Loader에서 투입 종료시 LOT-END 조건 검사
        //
        // 투입 LOT close시 LOT을 End시킬 필요가 있는지 체크한다.
        // 조건이 부합되는경우 LOT 을 Close 처리한다.
        //
        public void CheckAtLotClose(string sLotName)
        {
            // 지정 LOT이 실존하는 LOT 인가 ?
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx < 0)       // 존재하지 않는 LOT 이다.
            {
                log.Write($"* CheckAtLotClose({sLotName}), Missing Lot Name, Not exist LOT");
                return;
            }

            // 1. 지정 LOT에 속하는 Strip이 설비내에 존재하는가 ?
            //    ->  LOT-End 불가
            int nCount = GetLOTStripCount(sLotName);            // 설비내 지정 LOT Strip의 수량 조사
            if (nCount > 0)
            {
                log.Write($"CheckAtLotClose({sLotName}), Exist {nCount} strips in EQ");
                return;
            }

            // 2. 지정 LOT END 처리
			// 2022.03.24 SungTae : [수정] 변수 변경
            // 2022.02.18 SungTae Start : [수정] (ASE-KR VOC) Multi-LOT 관련 조건 추가
            if (!CSQ_Main.It.m_bPause/*!CSQ_Main.It.m_bAutoLoadingStop*/)
            {
                SetCompleteUnloadLOT(sLotName);    //  지정 LOT을 종료시킨다 
                log.Write($"CheckAtLotClose({sLotName}), LOT Complete");

                //   - 현재 Unloading 중인가 ? -> Mgz Place 처리는 Off-Loader 시퀀스에서 처리
                //   - 현재 Unloading 중이 아니고 더이상 설비내에 잔여 Strip이 존재하지 않는다면 LOT 정보 삭제
                if (sLotName != UnloadingLotName)
                {
                    CData.bLotCompleteMsgShow = true;                   // LOT 종료 메세지를 보여주도록 한다. -> 메세지를 보여줘야 완료된 LOT 정보를 삭제할 수 있다.
                    log.Write($"CheckAtLotClose({sLotName}), Show LOT Complete Message");
                }
            }
            // 2022.02.18 SungTae End

            DataUpdateFlag = true;                              // 내용이 변경되어 화면에 표시를 해야한다.
        }


        //
        // 삭제된 LOT에 해당되는 Magazine을 잡고있다면 내려놓도록 처리한다.
        //
        public void CheckMgzPlace(string sLotName)
        {
            if (sLotName == "") return;                 // 지정된 LOT Name이 없으면 수행 못함

            // On-Loader 처리
            if (LoadingLotName == sLotName)             // 지정한 LOT Name으로 현재 Loading 작업 진행중이면
            {
                // Magazine을 내려 놓도록 지정한다.
                CData.Parts[(int)EPart.ONL].iStat = ESeq.ONL_Place;
                log.Write($"CheckMgzPlace({sLotName}), Place Loading magazine");

                ClearLoadingLot();                      // Loading 중인 LOT 정보를 초기화 한다.
            }

            // Off-Loader 처리
            if (UnloadingLotName == sLotName)           // 지정한 LOT Name으로 현재 Unloading 작업 진행중이면
            {
                // Magazine을 내려 놓도록 지정한다.
                ClearUnloadingLot();                    // Unloading 중인 LOT 정보를 초기화 한다.
                IsForceUnloadPlace = true;              // Unloading Magazine을 강제로 Place 시켜야하는가 ? LOT 정보를 강제로 삭제시 적용된다.

                log.Write($"CheckMgzPlace({sLotName}), Place Unloading magazine request");
            }
        }


        //
        // LOT 이름으로 해당 LOT 정보를 취득한다.
        //
        public bool GetLotInfoName(string sLotName, ref TLotInfo rInfo)
        {
            // 동일한 이름을 가지는 LOT가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx >= 0)
            {
                rInfo = ListLOTInfo[nIdx];          // 검색된 데이터 대입
                return true;                        // 검색 성공
            }

            return false;                           // 찾지를 못하였다.
        }


        //
        // LOT 이름으로 해당 LOT 정보를 삭제한다.
        //
        public bool DeleteLotInfo(string sLotName)
        {
            // 지정 이름을 가지는 LOT가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx >= 0)
            {
                ListLOTInfo.RemoveAt(nIdx);         // 검색된 데이터 삭제
                ListUpdateFlag = true;                      // 항목 구성 내용이 변경되어 화면에 갱신할 필요가 있다.
                return true;                        // 삭제 성공
            }

            return false;                           // 삭제를 하지 못하였다.
        }


        //
        // 완료가 된 LOT을 삭제한다.
        //
        public void DeleteFinishLot()
        {
            int nBefore = ListLOTInfo.Count;
            int nIdx = -1;

            // 1개씩 순회하며 삭제처리한다.
            while ((nIdx = ListLOTInfo.FindIndex(x => x.nState == ELotState.eComplete)) >= 0)
            {
                log.Write($"Delete finished LOT information, deleted : {ListLOTInfo[nIdx].sLotName}");

                ListLOTInfo.RemoveAt(nIdx);
            }

            // ListLOTInfo.RemoveAll(x => x.nState == ELotState.eComplete);            // 완료된 LOT 항목을 일괄 삭제한다.

            // 만약에 모든 LOT 정보가 삭제되었다면 Magazine SN을 초기화 시키도록 한다.
            if (ListLOTInfo.Count == 0)
            {
                if (LoadMgzSN >= 1000)                  // 지정 수량 이상 증가하였다면 999 초과
                {
                    LoadMgzSN = 0;                      // Magazine serial 번호 Clear
                }
            }


            ListUpdateFlag = true;                      // 항목 구성 내용이 변경되어 화면에 갱신할 필요가 있다.
        }


        //
        // 지정 상태 조건을 가지는 LOT를 조회한다.
        //
        public bool GetStateLotInfo(ELotState eState, ref TLotInfo rInfo)
        {
            // 지정한 조건의 LOT가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => x.nState == eState);

            if (nIdx >= 0)
            {
                rInfo = ListLOTInfo[nIdx];          // 검색된 데이터 대입
                return true;                        // 검색 성공
            }

            return false;                           // 찾지를 못하였다.
        }


        //
        // 지정 상태 조건을 가지는 LOT 정보의 Index를 조회한다.
        //
        public int GetStateLotIndex(ELotState eState)
        {
            // 지정한 조건의 LOT가 존재한다면 
            return ListLOTInfo.FindIndex(x => x.nState == eState);
        }


        //
        // LOT별 색상 지정
        //
        public Color GetLotTypeColor(int nType)
        {
            // 지정된 색으로 표시해준다.
            // 현재는 3가지 종류

            if (nType == 0)
            {
                return Color.Lime;
            }
            else if (nType == 1)
            {
                return Color.Aqua;
            }
            else if (nType == 2)
            {
                return Color.Violet;
            }

            return Color.White;
        }


        //
        // 지정 LOT의 색상 조회
        //
        public Color GetLotColor(string sLotName)
        {
            // 지정 이름을 가지는 LOT가 존재한다면 해당 LOT의 색상을 되돌려준다.
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (sLotName == "" || ListLOTInfo.Count == 0 || nIdx < 0)
            {
                return Color.Silver;
            }
            else
            {
                if (nIdx >= 0)  return GetLotTypeColor(ListLOTInfo[nIdx].nType);
                else            return Color.Silver;
            }

            //return Color.White;
        }


        //
        // Load Magazine pickup 동작 가능 여부 조회
        // 투입 작업을 진행할 수 있는 상황인가 ? 입력된 LOT 정보가 있는가 ?
        //
        public bool IsMgzPickupable()
        {
            if (LoadingLotName != "") return true;              //  이미 Loading중인 LOT이 존재한다.

            // 대기중인 LOT 정보가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => (x.nState == ELotState.eWait) || (x.nState == ELotState.eRun));

            return (nIdx >= 0);
        }


        //
        // 투입이 가능한 LOT 정보를 취득한다.
        //
        public bool GetWaitLot(ref TLotInfo rInfo)
        {
            // 지정한 조건의 LOT가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => x.nState == ELotState.eWait);

            if (nIdx >= 0)
            {
                rInfo = ListLOTInfo[nIdx];          // 검색된 LOT 정보를 되돌려준다.
                return true;                        // 검색 성공
            }

            return false;                           // 찾지를 못하였다.
        }


        //
        // 지정 이름의 LOT이 존재하는가 ? 중첩 등록 방지용
        //
        public bool IsExistLot(string sLotName)
        {
            // 지정 이름을 가지는 LOT가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx >= 0)
            {
                return true;                        // 존재하고 있다.
            }

            return false;                           // 존재하지 않는다.
        }


        //
        // 지정 LOT의 현재 상태를 조회한다.
        //
        public ELotState GetLotState(string sLotName)
        {
            // 지정 이름을 가지는 LOT가 존재한다면 
            int nIdx = ListLOTInfo.FindIndex(x => string.Compare(x.sLotName, sLotName, true) == 0);

            if (nIdx >= 0)
            {
                return ListLOTInfo[nIdx].nState;            // 상태 회신
            }

            return ELotState.eError;                        // 문제가 있다.
        }


        //
        // 새로운 LOT를 시작하기 위해 관련 정보를 초기화 한다.
        //
        public void ClearLotInfo()
        {
            // Multi-LOT을 사용할 경우 항상 LOT는 Open 상태이다.
            CData.LotInfo.bLotEnd   = false;
            CData.LotInfo.bLotOpen  = true;

            LoadMgzSN = 0;                      // Magazine serial 번호 Clear

            CData.bLotEndShow    = false;
            CData.bLotEndMsgShow = false;
            CData.bLotTotalReset = true;
            CData.bLotIdleReset  = true;
            CData.bLotJamReset   = true;

            CData.nLotVerifyCount = 0;              // 중복된 Lot 정보 초기화를 막는다. 

            CData.bWhlDrsLimitAlarmSkip = false;    // Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

            // CData.LotInfo.sLotName = "";            // 입력된 LOT 이름
            CData.LotInfo.iLotOpenHour = DateTime.Now.Hour;

            CData.LotInfo.iTotalMgz         = 0;
            CData.LotInfo.iTotalStrip       = 0;
            //CData.LotInfo.bLotOpen          = false;
            //CData.LotInfo.bLotEnd           = false;
            CData.LotInfo.iTInCnt           = 0;
            CData.LotInfo.iTOutCnt          = 0;
            CData.LotInfo.i18PMeasuredCount = 0;    // 18 포인트 측정 (ASE-KR VOC)

            CData.LotInfo.dtOpen = DateTime.Now;        // Lot open 시간 초기화
            CData.LotInfo.dtEnd  = DateTime.Now;         // Lot end 시간 초기화

            CData.SpcInfo.sStartTime    = "";
            CData.SpcInfo.sEndTime      = "";
            CData.SpcInfo.sIdleTime     = "";
            CData.SpcInfo.sJamTime      = "";
            CData.SpcInfo.sRunTime      = "";
            CData.SpcInfo.sTotalTime    = "";
            CData.SpcInfo.sLotName      = CData.LotInfo.sLotName;
            CData.SpcInfo.sDevice       = CData.Dev.sName;
            CData.SpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
            CData.SpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;

            log.Write($"! ClearLotInfo() Clear all LotInfo data");
        }

    }
    //end of public class CMultiLotMgr == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==



    public static class CData
    {
        public static ManagerLot m_pHostLOT;

        /// 20190417 josh    secsgem
        public static frmMain FrmMain;
        // SECSGEM
        public static tSG SG = new tSG();

        public static CMultiLotMgr LotMgr = new CMultiLotMgr();     // 2021-06-07, jhLee, LOT 정보를 관리하는 관리자를 선언한다.

        // 2020.12.09 JSKim St
        ///// <summary>
        ///// Max 2020103 : SCK+ Rader & Error Text Modify
        ///// </summary>
        //public static int nTotalJamCount = 0;  // MTBA 계산에 사용.
        //public static DateTime dtTotalRunTim_MC;
        //public static DateTime dtTotalStopTim_MC;
        //public static DateTime dtTotalJamTim_MC;
        //public static Stopwatch SwTotalRunTim_MC = new Stopwatch();
        //public static Stopwatch SwTotalStopTim_MC = new Stopwatch();
        //public static Stopwatch SwTotalJamTim_MC = new Stopwatch();
        // 2020.12.09 JSKim Ed

        /// <summary>
        /// Max 2020103 : SCK+ Rader & Error Text Modify
        /// </summary>
        //dtTotalRunTim_MC = new DateTime();
        //dtTotalStopTim_MC = new DateTime();
        //dtTotalJamTim_MC = new DateTime();
        //SwTotalRunTim_MC = new Stopwatch();
        //SwTotalStopTim_MC = new Stopwatch();
        //SwTotalJamTim_MC = new Stopwatch();

        public static void Init()
        {
            for (int i = 0; i < 2; i++)
            {
                // 2020.09.08 SungTae : Grind 3 Step Mode 추가에 따른 변경
                if(CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                {
                    GrData[i].aInx       = new int   [GV.StepMaxCnt];
                    GrData[i].aCnt       = new int   [GV.StepMaxCnt];
                    GrData[i].aTar       = new double[GV.StepMaxCnt];
                    GrData[i].aReNum     = new int   [GV.StepMaxCnt];
                    GrData[i].aReInx     = new int   [GV.StepMaxCnt];
                    GrData[i].aReCnt     = new int   [GV.StepMaxCnt];
                    GrData[i].a1Pt       = new double[GV.StepMaxCnt];
                }
                else
                {
                    GrData[i].aInx       = new int   [GV.StepMaxCnt_3];
                    GrData[i].aCnt       = new int   [GV.StepMaxCnt_3];
                    GrData[i].aTar       = new double[GV.StepMaxCnt_3];
                    GrData[i].aReNum     = new int   [GV.StepMaxCnt_3];
                    GrData[i].aReInx     = new int   [GV.StepMaxCnt_3];
                    GrData[i].aReCnt     = new int   [GV.StepMaxCnt_3];
                    GrData[i].a1Pt       = new double[GV.StepMaxCnt_3];
                }

                GrData[i].aOldOnPont = new double[2]; //191018 ksg :
                // 200316 mjy : Unit 초기화 추가
                GrData[i].aUnit = new TUnit[GV.PKG_MAX_UNIT];
                GrData[i].aUnitEx = new bool[GV.PKG_MAX_UNIT];
                
                GrData[i].bSplLoadFlag = false;
                GrData[i].dSplMaxLoad = 0;

                //GrData[i].bSplMaxCurrFlag   = false;
                //GrData[i].bSplMinCurrFlag   = false;
                //GrData[i].nSplMaxCurr       = 0;
                //GrData[i].nSplMinCurr       = 999999;
                              

                //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                GrData[i].bCheckGrindCondition = false; //고급 설정 범위 체크 기능 동작/중지 플래그 (그라인딩 시)
                GrData[i].bCheckDressCondition = false; //고급 설정 범위 체크 기능 동작/중지 플래그 (드레싱 시)
                GrData[i].dTableVacuumMin = 0;          //Grinding 중 Table Vacuum 최소값 (그라인딩 시 체크)
                GrData[i].nSpindleCurrentMin = 0;       //Grinding 중 Spindle 전류 최소값 (그라인딩 또는 드레싱 시 체크)
                GrData[i].nSpindleCurrentMax = 0;       //Grinding 중 Spindle 전류 최대값 (그라인딩 또는 드레싱 시 체크)
                //..

                DrData[i].aInx = new int[2];
                DrData[i].aCnt = new int[2];

                ProbeTest[i].dBase   = new double[10];
                ProbeTest[i].dCenter = new double[10];
                ProbeTest[i].dBlock  = new double[10];
            }
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            Dynamic.dPcb = new double[GV.DFPOS_MAX];

            SecsGem_Data.qError_Count  = new Queue<int>();
            SecsGem_Data.qTerminal_Msg = new Queue<string>();
            SecsGem_Data.qTerminal_Log = new Queue<string>();
            
            m_pHostLOT = new ManagerLot();

            // 200316 mjy : Unit 초기화 추가
            for (int iSq = 0; iSq < 10; iSq++)
            {
                Parts[iSq].aUnitEx = new bool[GV.PKG_MAX_UNIT];
            }
        }

        public static void Release()
        {
            SwIdle.Stop();
            SwIdle = null;
            SwErr.Stop();
            SwErr = null;
            SwRun.Stop();
            SwRun = null;
            SwPCW.Stop();
            SwPCW = null;

            // 2020.12.09 JSKim St
            ///// <summary>
            ///// Max 2020103 : SCK+ Rader & Error Text Modify
            ///// </summary>
            //SwTotalRunTim_MC.Stop();
            //SwTotalRunTim_MC = null;
            //SwTotalStopTim_MC.Stop();
            //SwTotalStopTim_MC = null;
            //SwTotalJamTim_MC.Stop();
            //SwTotalJamTim_MC = null;
            // 2020.12.09 JSKim Ed
        }


        // 2021.10.20 SungTae Start : [추가] (ASE-KR VOC) Lot End 후 Lot Input 재시도 시 Strip List 상태 표시 개선
        public static int GetCarrierStatus(string sStatus)
        {
            int nStatus = 0;

            if (sStatus == JSCK.eCarrierStatus.Idle.ToString())     { nStatus = 0; }
            if (sStatus == JSCK.eCarrierStatus.Read.ToString())     { nStatus = 1; }
            if (sStatus == JSCK.eCarrierStatus.Validate.ToString()) { nStatus = 2; }
            if (sStatus == JSCK.eCarrierStatus.Start.ToString())    { nStatus = 3; }
            if (sStatus == JSCK.eCarrierStatus.End.ToString())      { nStatus = 4; }
            if (sStatus == JSCK.eCarrierStatus.Reject.ToString())   { nStatus = 5; }

            return nStatus;
        }

        /*
         * - 기존 : 별도로 저장하지 않아서 다시 Loading 시 모든 Strip의 상태를 초기화(Not read)로 표시
         * - 변경 : Carrier List의 마지막 Status를 저장 후 Loading 하여 Strip 진행 여부 파악하기 쉽도록 수정
         */
        /// <summary>
        /// Header : Carrier ID, Status
        /// 저장 경로 - D:\Spc\LotLog\yyyyMMdd\LotName\LotStatus.csv
        /// </summary>
        public static bool LoadLastLotStatus(string sLotName)
        {
            int nStatus = 0;

            string sPath = GV.PATH_SPC + "LotLog\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + sLotName;

            DirectoryInfo di = new DirectoryInfo(sPath);
            if (!di.Exists) return false;

            sPath += "\\LotStatus.csv";

            FileInfo fi = new FileInfo(sPath);
            if (!fi.Exists) return false;

            try
            {
                using (FileStream fs = new FileStream(sPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
                    {
                        string lines = null;
                        string[] values = null;

                        while ((lines = sr.ReadLine()) != null)
                        {
                            if (string.IsNullOrEmpty(lines))
                                sr.Close();

                            values = lines.Split(','); // 콤마로 분리
                            nStatus = GetCarrierStatus(values[1]);

                            ManagerLot pLot = new ManagerLot();
                            Carrier pCar = new Carrier();

                            //pCar.strCarrierId   = values[0];
                            //pCar.nCarrierStatus = nStatus;
                            //pLot.AddCarrier(pCar);

                            //LotMgr.ListLOTInfo.ElementAt(nIdx).m_pHostLOT.Add(pLot);
                        }

                        sr.Close();
                    }

                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return true;
        }

        public static bool SaveLotStatus(ref TLotInfo pLotInfo)
        {
            string sPath = "";
            string sWriteLine = "";
            string sCarrierID = "";
            string sStatus = "";

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += pLotInfo.sLotName + "\\";

            DirectoryInfo dtInfo = new DirectoryInfo(sPath);

            if (!dtInfo.Exists)
            {
                dtInfo.Create();
            }

            sPath += "LotStatus.csv";

            FileInfo fiInfo = new FileInfo(sPath);

            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
            }

            for (int i = 0; i < pLotInfo.m_pHostLOT.Count; i++)
            {
                // Carrier ID
                sCarrierID = pLotInfo.m_pHostLOT.GetCarrierID(i);

                // Carrier Status
                sStatus = pLotInfo.m_pHostLOT.GetCarrierStatus(i).ToString();

                sWriteLine = sCarrierID + "," + sStatus + ",";

                CLog.Check_File_Access(sPath, sWriteLine, true);
            }

            return true;
        }
        // 2021.10.20 SungTae End

        /// <summary>
        /// WMX Connection    false : Disconnect, true : Connect
        /// </summary>
        public static bool WMX = false;
        
        public static bool VIRTUAL = false;

        //ksg : Dry Run Mode 시 Table table Skip시 True
        /// <summary>
        /// Auto Run시 그라인더 스킵
        /// </summary>
        public static bool DRY_AUTO = false;
        //public static bool DRY_AUTO = true;

        public static tAx     [] Axes = new tAx     [32];
        public static t232C   [] Prbs = new t232C   [3 ];
        public static tSplData[] Spls = new tSplData[2 ];
        //20191108 ghk_light
        public static t232C Light = new t232C();
        public static int LightVal1;
        public static int LightVal2;

        /// <summary>
        /// 화면 초기화 변수
        /// FrmMain 에서 여기로 옮김
        /// </summary>
        public static bool bInitPage;

        public static TLotInfo LotInfo = new TLotInfo();

        public const int JSCK_Max_Cnt = 30;
        public static JSCK_SECSGEM_STRIP_DATA[] JSCK_Gem_Data = new JSCK_SECSGEM_STRIP_DATA[JSCK_Max_Cnt];

        #region SECSGEM  Info
        public static TSecsgem_Data SecsGem_Data = new TSecsgem_Data();
        
        #endregion

        #region Position
        /// <summary>
        /// 비공개 셋업 포지션 0:좌 1:우
        /// </summary>
        public static tMnP[] MPos = new tMnP[2];

        /// <summary>
        /// 공개 셋업 포지션
        /// </summary>
        public static tSyP SPos = new tSyP();
        #endregion

        /// <summary>
        /// 옵션 구조체
        /// </summary>
        public static tOpt Opt = new tOpt();

        /// <summary>
        /// 디바이스 구조체
        /// </summary>
        public static tDev Dev = new tDev();
        /// <summary>
        /// 현재 사용중인 디바이스 파일의 경로
        /// </summary>
        public static string DevCur = "";

        public static string DevGr = "";

        /// <summary>
        /// 휠 구조체 0:좌 1:우
        /// </summary>
        public static tWhl[] Whls = new tWhl[2];

        //211122 pjh : 
        /// <summary>
        /// Dresser 구조체 0:좌 1:우
        /// </summary>
        public static TDrs[] Drs = new TDrs[2];

        /// <summary>
        /// Wheel Log Array
        /// 휠 히스토리 구조체
        /// </summary>
        public static tWhlLog[] WhlsLog = new tWhlLog[2];

        /// <summary>
        /// Dresser Log Array
        /// 210106 pjh : Dresser 히스토리 구조체
        /// </summary>
        public static tDrsLog[] DrsLog = new tDrsLog[2];

        //20190216 ghk_probeTest
        /// <summary>
        /// 프로브 반복 측정 테스트
        /// 프로브 반복 데이터 구조체
        /// </summary>
        public static tProbeTest[] ProbeTest = new tProbeTest[2];

        /// <summary>
        /// 바코드 데이터 구조체
        /// </summary>
        public static tBcr Bcr = new tBcr();

        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        /// <summary>
        /// KEYENCE 바코드 데이터 구조체
        /// </summary>
        public static tKeyenceBcr [] KeyBcr = new tKeyenceBcr[(int)EKeyenceBcrPos.MaxNum];
        //

        /// <summary>
        /// 다이나믹 펑션 구조체
        /// </summary>
        public static tDynamic Dynamic = new tDynamic();

        #region Wheel thickness
        /// <summary>
        /// 휠 이전 측정 값 0:좌 1:우
        /// </summary>
        public static double[] WhlBf = new double[2];
        /// <summary>
        /// 휠 현재 측정 값 0:좌 1:우
        /// </summary>
        public static double[] WhlAf = new double[2];
        /// <summary>
        /// 휠 측정 시 측정 횟수 높이 [좌우,측정횟수]
        /// </summary>
        public static double[,] WhlMea = new double[2,3];

        //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
        /// <summary>
        /// 휠 Before 측정 시 Z축 위치 보관용/계산용 변수 0:좌 1:우
        /// </summary>
        public static double[] WheelTipBeforeZPos = new double[2];
        /// <summary>
        /// 휠 After 측정 시 Z축 위치 보관용/계산용 변수 0:좌 1:우
        /// </summary>
        public static double[] WheelTipAfterZPos = new double[2];
        /// <summary>
        /// Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
        /// </summary>
        public static bool bUseWheelZAxisAfterMeasureWheel = false;
        //

        /// <summary>
        /// 휠 로스량 반환 이전값 - 현재값
        /// </summary>
        /// <param name="iDir">0:좌 1:우</param>
        /// <returns></returns>
        //public static double GetWLoss(int iDir) { return Math.Round(WhlBf[iDir] - WhlAf[iDir], 5); }
        public static double GetWLoss(int iDir) { return Math.Round(Whls[iDir].dWhlBf - Whls[iDir].dWhlAf, 5); }
        #endregion

        #region Dresser thickness
        /// <summary>
        /// 드레셔 이전 측정 값 0:좌 1:우
        /// </summary>
        public static double[] DrsBf = new double[2];
        /// <summary>
        /// 드레셔 현재 측정 값 0:좌 1:우
        /// </summary>
        public static double[] DrsAf = new double[2];
        /// <summary>
        /// 드레셔 로스량 반환 (이전값 - 현재값)
        /// </summary>
        /// <param name="iDir">0:좌 1:우</param>
        /// <returns></returns>
        public static double GetDLoss(int iDir) { return Math.Round(Whls[iDir].dDrsBf - Whls[iDir].dDrsAf, 5); }
        #endregion

        #region Table thickness
        public static double[,] Tbl_Bf = new double[2, 10];
        public static double[,] Tbl_Af = new double[2, 10];
        #endregion

        #region Table thickness Six Point
        public static double[,] Tbl_BfSix = new double[2, 10];
        public static double[,] Tbl_AfSix = new double[2, 10];
        #endregion

        public static DateTime Warm;
        /// <summary>
        /// 프로그램 시작한 시간
        /// </summary>
        public static DateTime ProgS;
        /// <summary>
        /// 유휴상태 시작된 시간
        /// </summary>
        public static DateTime IdleS;
        /// <summary>
        /// Jamming 시작된 시간
        /// </summary>
        public static DateTime JamS;
        /// <summary>
        /// Jamming 시작된 시간
        /// </summary>
        public static DateTime RunS;
        /// <summary>
        /// Warm Up 마지막 시간
        /// </summary>
        public static DateTime WarmUpS;
        /// <summary>
        /// Last Click 마지막 시간
        /// </summary>
        public static DateTime LastClickS; //190509 ksg :

        //20190927 ghk_autowarmup
        public static DateTime AutoWarmPre;
        public static DateTime AutoWarmNow;
        public static TimeSpan AutoWarmSpan;

        public static DateTime dtSpgWOffLastTime;  // 2022.06.14 lhs : Sponge water Auto on/off에 사용 


        // 200327 mjy : TElapse 구조체로 변경 처리
        /// <summary>
        /// 그라인딩 시작 시간 (Before Measure Strip 후)
        /// 그라인딩 종료 시간 (After measure Strip 전)
        /// 그라인딩 시간
        /// </summary>    
        public static TElapse[] GrdElp = new TElapse[2];
        // 200326 mjy : 신규 추가
        /// <summary>
        /// 드레싱 시작, 종료, 경과시간
        /// </summary>
        public static TElapse[] DrsElp = new TElapse[2];

        /// <summary>
        /// lot 오픈 후 lot End까지 시간
        /// </summary>
        //public static Stopwatch swLotTotal = new Stopwatch();
        public static bool bLotTotal;
        public static bool bLotTotalReset;

        /// <summary>
        /// lot 오픈 후 lot End까지 에러 시간
        /// </summary>
        //public static Stopwatch swLotJam = new Stopwatch();
        public static bool bLotJam;
        public static bool bLotJamReset;
        /// <summary>
        /// lot 오픈 후 lot End까지 유휴시간
        /// </summary>
        //public static Stopwatch swLotIdle = new Stopwatch();
        public static bool bLotIdle;
        public static bool bLotIdleReset;
        /// <summary>
        /// 에러 발생시 에러 번호 저장
        /// </summary>
        public static int iErrNo;

        public static string m_sBcrLog = "";

        /// <summary>
        /// 각 파트의 데이터 배열
        /// 0:On Loader
        /// 1:In Rail
        /// 2:On Loader Picker
        /// 3:Grind Left
        /// 4:Grind Right
        /// 5:Off Loader Picker
        /// 6:Dry
        /// 7:Off Loader
        /// </summary>
        public static TPart[] Parts = new TPart[10];

        /// <summary>
        /// Grinding 진행 시 사용되는 변수
        /// </summary>
        public static TGrdData[] GrData = new TGrdData[2];

        /// <summary>
        /// Dressing 진행 시 사용되는 변수
        /// </summary>
        public static TDrsData[] DrData = new TDrsData[2];

        /// <summary>
        /// 현재 설정 된 권한 레벨
        /// </summary>
        public static ELv Lev;
        public static ELv CurLevel;

        /// <summary>
        /// SPC LotData
        /// </summary>
        public static TSpcData SpcData = new TSpcData();
        /// <summary>
        /// SPC LotInfo
        /// </summary>
        public static TSpcInfo SpcInfo = new TSpcInfo();

        public static tErr[] ErrList = new tErr[(int)eErr.ERR_MAXCOUNT];
        /// <summary>
        /// Tower Lamp Status
        /// 0. Off
        /// 1. On
        /// 2. Flick
        /// &&
        /// Buzzer
        /// 0. Off
        /// 1. On
        /// </summary>
        public static tTowerInfo[] m_TowerInfo = new tTowerInfo[(int)ETowerStatus.MAX_TWR_STATUS];
        
        //20190614 ghk_dfserver
        public static TDfInfo dfInfo = new TDfInfo();
        public static TGrdDfData GRLDfData = new TGrdDfData();
        public static TGrdDfData GRRDfData = new TGrdDfData();

        
        /// <summary>
        /// 에러 발생 시 에러 화면 출력 플래그
        /// </summary>
        public static bool ShowErrForm = false;
        //20190227 ghk_err
        //public static eErr IsErr = eErr.NONE;
        public static eErr IsErr = 0;
        public static bool bLotEndShow          = false;
        public static bool bLotEndMsgShow       = false;    //190521 ksg :
        public static bool bPreLotEndMsgShow    = false;    //koo : Qorvo Lot Rework
        public static bool bPreLotend           = false;    //koo : Qorvo Lot Rework
        public static bool bSecLotOpen          = false;    //191030 ksg :
        public static int nLotVerifyCount       = 0;        // 2021-01-18, jhLee : SECS/GEM Server에서 보내온 LOT Verify info를 몇개나 받았나.
        public static bool bEraseLotName        = false;    // 2021.04.26 lhs : JCET 전용으로 LotEnd시 LotName 화면에서 지우기
        public static bool bLotCompleteMsgShow  = false;    // 2021-06-03, jhLee : 개별 LOT 종료시 내용을 화면에 표시해서 작업자의 확인을 받도록 한다.
        public static bool bLotCompleteBuzzer   = false;    // 2021-06-03, jhLee : LOT 종료 후 Buzzer를 울려줄 것인가 ?

        //190103 ksg : 연속 빈슬롯 감지시 배출
        public static int  iEmptySlotCnt = 0    ;
        public static bool bEmptySlotCnt = false;
        public static bool bShowWhlJig   = false; //1890217 ksg :
        public static bool bDrsChange    = false;
        /// <summary>
        /// Auto Level Change 사용 하기 위한 변수
        /// </summary>
        public static bool bLastClick    = false;

        public static tPbResult[] PbResultVal = new tPbResult[2]; //190319 ksg :
        /// <summary>
        /// Wheel 교체 진행 시 진행 상황 관리하는 변수
        /// </summary>
        public static tWhlChgSeq WhlChgSeq = new tWhlChgSeq(); //190523 ksg :
        public static tDrsChgSeq DrsChgSeq = new tDrsChgSeq(); //190622 ksg :

        //소스 그룹
        //1. 
        //소스 적용 회사
        //Company 변경시 주석 바꿀 것
        public static ECompany CurCompany = ECompany.ASE_KR    ; //190225
        //public static ECompany CurCompany = ECompany.Qorvo   ; //190225
        //public static ECompany CurCompany = ECompany.Qorvo_DZ   ; //190225
        //public static ECompany CurCompany = ECompany.Qorvo_RT; // 21.03.16 SungTae : Add
        //public static ECompany CurCompany = ECompany.Qorvo_NC; // 21.03.16 SungTae : Add
        //public static ECompany CurCompany = ECompany.SkyWorks; //190225
        //public static ECompany CurCompany = ECompany.JSCK    ; //190415
        //public static ECompany CurCompany = ECompany.Suhwoo  ; //190521 ksg :
        //public static ECompany CurCompany = ECompany.ASE_K12 ; //190614 ksg :
        //public static ECompany CurCompany = ECompany.SST     ; //191202 ksg :
        //public static ECompany CurCompany = ECompany.USI     ; //191202 ksg :
        //public static ECompany CurCompany = ECompany.SCK     ; //200121 ksg :
        //public static ECompany CurCompany = ECompany.SPIL ; //200301 LCY
        //public static ECompany CurCompany = ECompany.JCET ; //200625 LKS

        public static string CurCompanyName = "Company";
        public static bool useLicense = false;
        /// <summary>
        /// 장비 유휴 상태 이후 PCW off로 설정되기 전 대기시간 [min]
        /// 설정된 시간 동안 유휴상태(Idle)이면, PCW off
        /// </summary>
        public static int PcwTime  = 10;        
        /// <summary>
        /// 장비 유휴 상태 이후 PCW off로 설정되기 전 대기시간 [min]
        /// 설정된 시간 동안 유휴상태(Idle)이면, PCW off
        /// </summary>
        public static int PumpTime  = 15;        

        /// <summary>
        /// Idle 상태 유지 시간 
        /// </summary>
        public static Stopwatch SwIdle ;
        /// <summary>
        /// Error 상태 유지 시간
        /// </summary>
        public static Stopwatch SwErr;
        /// <summary>
        /// 프로그램 가동 유지 시간
        /// </summary>
        public static Stopwatch SwRun  ;
        // 190715-maeng
        /// <summary>
        /// Idle 상태로 변경 후 PCW조작을 위한 시간 측정
        /// </summary>
        public static Stopwatch SwPCW = new Stopwatch();
        // 200121 ksg :
        /// <summary>
        /// Idle 상태로 변경 후 Pump조작을 위한 시간 측정
        /// </summary>
        public static Stopwatch SwPump = new Stopwatch();

        /// <summary>
        /// 랏 종료 시 사용되는 랏 이름
        /// 190701-maeng
        /// </summary>
        public static string sLastLot = "";
        /// <summary>
        /// 랏 종료 시 사용되는 매거진 갯수
        /// 190701-maeng
        /// </summary>
        public static int iLastMGZ = 0;        
        // 190802-maeng
        /// <summary>
        /// Log를 저장하기 위한 큐
        /// </summary>
        public static ConcurrentQueue<tMainLog> QueLog = new ConcurrentQueue<tMainLog>();
        // 210601 jym : 네트워크 드라이브 스레드 추가
        public static ConcurrentQueue<TNetDrive> QueNet = new ConcurrentQueue<TNetDrive>();

        public static CSq_Grd L_GRD;
        
        public static CSq_Grd R_GRD;

        //20191111 ghk_regrindinglog
        //public static ReGrdCntLog[,] RedGrdCnt = new ReGrdCntLog[2, 4];
        public static ReGrdCntLog[,] RedGrdCnt = new ReGrdCntLog[2, GV.StepMaxCnt];//200414 ksg : 12 Step  기능 추가
        public static ReGrdCntLog[,] RedGrdCnt3 = new ReGrdCntLog[2, GV.StepMaxCnt_3];   // 2020.09.08 SungTae : 3 Step 기능 추가
        //

        //191120 ksg :
        public static PrboeValue[] m_dPbVal = new PrboeValue[2];
		// SECSGEM
        public static frmGEM GemForm;
        public static bool JobChange = false;
        public static string sBST_L = "";
        public static string sAST_L = "";
        public static string sBST_R = "";
        public static string sAST_R = "";

        // 2021-06-08, jhLee : Multi-LOT 입력/관리 Form
        public static CDlgLotInput dlgLotInput;

        // 200326 mjy : 라이트커튼 센서 감지시 램프 노란색 점멸 및 부져 온
        public static bool bLcDetect = false;

        public static bool bBuzzPMMsgWnd        = false;    // 2021.10.22 lhs PMCount 메시지창 표시된 상태 -> 부저 온 목적
        public static int  nShowStatePMMsgWnd   = 0;        // 2021.10.28 lhs PMCount 메시지창 표시 상태 0=None, 1=Show, 2=Close -> 부저 온 목적


        //200406 jym : 신규 추가
        /// <summary>
        /// 테이블 그라인딩 시 측정 스탭 플래그
        /// </summary>
        public static EMeaStep TblMea = EMeaStep.Before;
        /// <summary>
        /// 드레싱 시작 확인 플래그
        /// </summary>
        public static bool[] IsDrsStart = new bool[2];

        //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC)
        /// <summary>
        /// Wheel/Dresser Limit 도달 시 Wheel/Dresser 교체 옵션 선택 팝업에서
        /// "After Current Loaded Strip" 선택 => OK 클릭 시 => Loading STOP 활성화, Error 발생 보류시킴
        /// true  = Wheel/Dresser Limit 도달되어도 Error 발생 보류시킴
        /// false = Wheel/Dresser Limit 도달되면 Error 발생
        /// </summary>
        public static bool bWhlDrsLimitAlarmSkip = false;

        // 2020.09.10 SungTae : Wheel Change 시 PCW Auto Off 기능 보류용 (Qorvo VOC)
        /// <summary>
        /// true  = Wheel Change 시 설비가 IDLE 상태로 10분이 지나도 PCW Off 안함.
        /// false = 설비가 IDLE 상태로 10분이 지나면 PCW Off.
        /// </summary>
        public static bool bWhlChangeMode = false;

        // 2021.09.28 SungTae Start : [ASE-KR VOC] 매 LOT의 첫번째 Strip일 경우 Auto로 Table Measuring 하기 위한 Flag
        /// <summary>
        /// false : Not Use
        /// true  : Use
        /// </summary>
        public static bool bAutoTblMeas = false;

        /// <summary>
        /// Table 측정 Point 간 차이값 저장
        /// </summary>
        public static double[,,] dTblMeasDiff = new double[2, 2, 2];
        // 2021.09.28 SungTae End

        // 2021.10.19 SungTae Start : [추가] (ASE-KR VOC) SECS/GEM 관련
        /* 
         * <사고 내용>
         * Remote로 정상적으로 LOT 진행 후 Operator의 조작 미숙으로 SECS/GEM을 끄지 않은 상태에서 Manual Grinding 진행
         * => 이전 LOT의 마지막 Strip ID로 Data가 Host로 다시 보고되어 해당 Strip을 Bottom Grinding 진행 시 Over Grinding 사고 발생
         */
        /// <summary>
        /// SECS/GEM Use 상태에서 Manual Grinding 시 Host로 Data 보고 안되도록 하기 위한 Check용 Flag
        /// </summary>
        public static bool bChkGrdM = false;
        // 2021.10.19 SungTae End

        // 2022.04.25 SungTae Start : [추가] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag
        public static bool bChkManual = false;


        // 2021.12.15 SungTae Start : [추가] (ASE-KR VOC) Dry Zone에 Strip 유무 확인용 Flag
        public static bool bDryStripExist = false;
        // 2021.12.15 SungTae End

        // 2022.05.31 SungTae Start : [추가] (ASE-KR VOC) 
        /// <summary>
        /// 프로그램 재실행 후 첫번째 Strip Grinding 완료 뒤 S6F11 CEID=999307(Carrier Grind Finished) 보고할 때
        /// Left Table Data를 Default 값(0)으로 보고되도록 하기 위한 Flag 추가
        /// </summary>
        public static bool bChkFirstTopGrdFlag = true;
        // 2022.05.31 SungTae End


        public static int ReSkipCnt = 1;

        public static TMsg VwMsg = new TMsg();
        public static TMsg VwWhl = new TMsg();
        public static TMsg VwKey = new TMsg();

        //200811 lks 라이선스 기간
        public static DateTime licenseDate;

        //200811 lks 라이선스 기간 검증
        public static void CheckLicenseValidation()
        {
            //단순 날짜 검증
            if (licenseDate < DateTime.Now.AddDays(-1))
            {
                CLog.mLogSeq(" #License Check : Expired");
                CMsg.Show(eMsg.Error, "License", "[" + CurCompany+ "] License Expired!!\r\n\r\nPlease contact the SUHWOO Technology Co.,Ltd.");                
            }
            else
                CLog.mLogSeq(" #License Check : Done");

            //날짜 저장 후 검증
        }

        // 2020.08.24 JSKim
        /// <summary>
        /// Device Parameter 확인 시 Only View 로 보기 위한 플래그
        /// </summary>
        public static bool bDevOnlyView = false;
        // 201023 jym : 추가
        /// <summary>
        /// 오토 웜업 인지 확인 
        /// </summary>
        public static bool IsATW = false;
        // 201028 jym : 추가
        /// <summary>
        /// 부저 끄기
        /// </summary>
        public static bool BuzzOff = false;

        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        /// <summary>
        /// 다음 번 Orientation 검사 1회에 한해 Skip 여부 : true = 1회 Skip, false = Skip 않음 
        /// </summary>
        public static bool bOriOneTimeSkip = false;
        //201117 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        /// <summary>
        /// Orientation 검사 1회 Skip 버튼 표시 여부 : true : 표시, false : 표시안함 (Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤)
        /// </summary>
        public static bool bOriOneTimeSkipBtnView = false;

        public static void Chk_AutoLoading_Count()
        {
            // syc : ase kr loading stop : 2020-11-17 이재현 수석님 작성 부분
            {
                if (CSQ_Main.It.m_bAutoLoadingStop)
                {
                    CSQ_Main.It.m_iLoadingCount++;             // 자동 투입중지 모드를 설정 한 뒤 투입된 제품 수량 증가

                    if (CDataOption.UseSetUpStrip)
                    {
                        // 2022.02.08 SungTae Start : [수정] Multi-LOT 관련
                        // Auto Loading Stop 시 투입한 수량만큼 Off-loader MGZ에 적재되면 "LOT END" 시키는 문제 관련 수정
                        if (IsMultiLOT())
                        {
                            int nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                            if (CData.LotMgr.ListLOTInfo.Count > 0)
                            {
                                if (CData.LotMgr.ListLOTInfo.ElementAt(nIdx).iTInCnt >= CData.Opt.iAutoLoadingStopCount)
                                {
                                    CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
                                    CSQ_Main.It.m_bPause = true;                // 투입 중지가 활성화 된다.       

                                    //_SetLog("[Auto Loading Stop] finish " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                                }
                            }
                        }
                        else
                        {
                            if (LotInfo.iTInCnt >= Opt.iAutoLoadingStopCount)
                            {
                                CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
                                CSQ_Main.It.m_bPause = true;                // 투입 중지가 활성화 된다.       

                                //_SetLog("[Auto Loading Stop] finish " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                            }
                        }
                        // 2022.02.08 SungTae End
                    }
                    else
                    {
                        // 2022.03.14 SungTae Start : [수정] Multi-LOT 관련 조건 추가. 
                        // Auto Loading Stop 시 투입한 수량만큼 Off-loader MGZ에 적재되면 "LOT END" 시키는 문제 관련 수정
                        if (IsMultiLOT())
                        {
                            int nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                            if (CData.LotMgr.ListLOTInfo.Count > 0)
                            {
                                if (CData.LotMgr.ListLOTInfo.ElementAt(nIdx).iTInCnt >= CData.Opt.iAutoLoadingStopCount)
                                {
                                    CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
                                    CSQ_Main.It.m_bPause = true;                // 투입 중지가 활성화 된다.       

                                    //_SetLog("[Auto Loading Stop] finish " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                                }
                            }
                        }
                        else
                        {
                            if (CSQ_Main.It.m_iLoadingCount >= CData.Opt.iAutoLoadingStopCount)
                            {
                                CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
                                CSQ_Main.It.m_bPause = true;                // 투입 중지가 활성화 된다.       

                                //_SetLog("[Auto Loading Stop] finish " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                            }
                        }
                        // 2022.03.14 SungTae End
                    }
                    //else
                    //{
                    //    _SetLog("[Auto Loading Stop] remain " + CSQ_Main.It.m_iLoadingCount.ToString() + " / " + CData.Opt.iAutoLoadingStopCount.ToString());
                    //}
                }
            }
        }

        //210827 syc : 2004U IV2 - 옵션 데이터 저장용
        public static int iIV2OnpPort = 0;
        public static int iIV2OfpPort = 0;
        public static string sIV2OnpIP = "";
        public static string sIV2OfpIP = "";
        //210827 syc end

        //210929 syc : 2004U Lot 시작시 Cover Pick
        /// <summary>
        /// Lot 시작시 Cover Pick을 위해서 사용
        /// </summary>
        public static bool bDry_CoverPick = false;
        /// <summary>
        /// Lot 시작시 Cover Pick을 위해서 사용
        /// </summary>
        public static bool bOFP_CoverPick = false;
        /// <summary>
        /// Cover PIck 메뉴얼 동작시 Dry 다음 동작 설정
        /// True  : Dry out (기본)
        /// False : Dry wait place 
        /// </summary>
        public static bool bOFFP_NextSeqDryout = true;
        /// <summary>
        /// Cvoer Pick 메뉴얼 동작시 물어보는 메세지 박스
        /// </summary>
        public static bool bOFFP_CoverPickMSGCheck = true;
        /// <returns></returns>
        // syc end

        // 2021.08.10 SungTae : [추가] (ASE-KR VOC) Remote 상태에서 Open한 LOT의 수 확인
        public static int OpenLotCnt { get; set; } = 0;        // On-line Remote로 Open한 LOT의 수

        //211202 pjh : DF Net Drive Connect Check
        /// <summary>
        /// Net Drive 연결 상태 Check bool 변수
        /// </summary>
        public static bool m_bNetCheck = false;
        //

        // 2021-05-27, jhLee, Multi-LOT  실행 조건인가 ?
        //
        // 라이센스에서 Multi LOT 기능을 사용가능으로 설정하고 Option 에서 사용하도록 지정하였다면 Multi-LOT 사용 조건이다.
        //
        public static bool IsMultiLOT()
        {
            return (CDataOption.UseMultiLOT && CData.Opt.bMultiLOTUse);
        }

        //---------------------------
        // 2021.12.28 lhs Start : 사용한 곳이 없어 일단 주석처리함... 필요시 주석해제하시길....

        //// 2021-05-24, jhLee, 데이터를 지정 Unit으로 이동 시킨다. 이전 데이터는 초기화 한다.
        //public static void MovePartData(int nSrc, int nDest)
        //{
        //    Parts[nDest].sLotName = Parts[nSrc].sLotName;

        //    Parts[nDest].iMGZ_No = Parts[nSrc].iMGZ_No;
        //    Parts[nDest].sMGZ_ID = Parts[nSrc].sMGZ_ID;
        //    Parts[nDest].iSlot_No = Parts[nSrc].iSlot_No;
        //    Parts[nDest].sBcr = Parts[nSrc].sBcr;
        //    Parts[nDest].bBcr = Parts[nSrc].bBcr;
        //    //Parts[nDest].bBcr = Parts[nSrc].bBcr;
        //    Parts[nDest].bNotMatch = Parts[nSrc].bNotMatch;
        //    Parts[nDest].dPcbMax = Parts[nSrc].dPcbMax;
        //    Parts[nDest].dPcbMean = Parts[nSrc].dPcbMean;
        //    Parts[nDest].dPcbMin = Parts[nSrc].dPcbMin;
        //    Parts[nDest].dShiftT = Parts[nSrc].dShiftT;
        //    Parts[nDest].dDfMin = Parts[nSrc].dDfMin;
        //    Parts[nDest].dDfMax = Parts[nSrc].dDfMax;
        //    Parts[nDest].dDfAvg = Parts[nSrc].dDfAvg;
        //    Parts[nDest].sBcrUse = Parts[nSrc].sBcrUse;
        //    Parts[nDest].iTPort = Parts[nSrc].iTPort;
        //    Parts[nDest].bChkGrd = Parts[nSrc].bChkGrd;
        //    Parts[nDest].b18PMeasure = Parts[nSrc].b18PMeasure;
        //    Parts[nDest].iStripStat = Parts[nSrc].iStripStat;
        //    Parts[nDest].dChuck_Vacuum = Parts[nSrc].dChuck_Vacuum;
        //    Parts[nDest].nLotType = Parts[nSrc].nLotType;
        //    Parts[nDest].LotColor = Parts[nSrc].LotColor;
        //    Parts[nDest].nLoadMgzSN = Parts[nSrc].nLoadMgzSN;


        //    // 배열 전체를 복사해준다.
        //    Array.Copy(Parts[nSrc].dPcb, Parts[nDest].dPcb, Parts[nSrc].dPcb.Length);
        //    Array.Copy(Parts[nSrc].aUnitEx, Parts[nDest].aUnitEx, Parts[nSrc].aUnitEx.Length);

        //    Parts[nDest].bExistStrip = Parts[nSrc].bExistStrip;             // Strip 존재 여부는 가장 마지막에 이동시킨다.


        //    // 이동을 마친 원본 데이터를 초기화 한다.
        //    ClearPartData(nSrc);

        //} //of MovePartData()

        // 2021.12.28 lhs End
        //---------------------------


        // 지정 Part 항목의 내용들을 Clear 시켜준다.
        public static void ClearPartData(int nPos)
        {
            Parts[nPos].bExistStrip     = false;                               // Strip 존재 여부는 가장 처음에 초기화 시켜준다.

            Parts[nPos].sLotName        = "";
            Parts[nPos].iMGZ_No         = 0;
            Parts[nPos].sMGZ_ID         = "";
            Parts[nPos].iSlot_No        = 0;
            Parts[nPos].sBcr            = "";
            Parts[nPos].bBcr            = false;
            //Parts[nPos].bBcr = false;               // 2021.08.05 SungTae : [삭제] 중복 선언으로 주석 처리함.
            Parts[nPos].bNotMatch       = false;
            Parts[nPos].dPcbMax         = 0.0;
            Parts[nPos].dPcbMean        = 0.0;
            Parts[nPos].dPcbMin         = 0.0;
            Parts[nPos].dShiftT         = 0.0;
            Parts[nPos].dDfMin          = 0.0;
            Parts[nPos].dDfMax          = 0.0;
            Parts[nPos].dDfAvg          = 0.0;
            Parts[nPos].sBcrUse         = "";
            Parts[nPos].iTPort          = 0;
            Parts[nPos].bChkGrd         = false;
            Parts[nPos].b18PMeasure     = false;
            Parts[nPos].iStripStat      = 0;
            Parts[nPos].dChuck_Vacuum   = 0.0;
            Parts[nPos].nLotType        = 0;
            Parts[nPos].nLoadMgzSN      = 0;

            int i;
            for (i = 0; i < Parts[nPos].dPcb.Length; i++)
            {
                Parts[nPos].dPcb[i] = 0.0;
            }

            for (i = 0; i < Parts[nPos].aUnitEx.Length; i++)
            {
                Parts[nPos].aUnitEx[i] = false;
            }
        }

        // 2021.07.14 lhs Start : PM Count 
        #region PM Count
        /// <summary>
        /// PM Count 구조체 (Life Time)
        /// </summary>
        public static TPM tPM = new TPM();
        #endregion
        // 2021.07.14 lhs End


        // 2021.10.04 lhs Start : OnLoader Pick Msg창
        public static EOnLUserConfirm eOnLUserConfirm = EOnLUserConfirm.eNone;
        // 2021.10.04 lhs End

        // 2021.05.28 SungTae Start : DF Data 저장 함수 추가
        public static void SaveDFServerData(EWay nWay, int nTopSide, EPart nPart)
        {
            //double dTotalThick = 0D;
            //double dMoldThick = 0D;
            string sGrindMode = "";
            string sStage = "";
            string sPath = GV.PATH_EQ_DF;
#if false
            // Test
            CData.Parts[(int)nPart].sLotName = "LOT1234";
            CData.Parts[(int)nPart].sBcr     = "ST123401";
            CData.Parts[(int)nPart].dPcbMax  = 0.103;
            CData.Parts[(int)nPart].dPcbMean = 0.102;
            CData.Parts[(int)nPart].dPcbMin  = 0.101;
#endif

            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[Lot Info]");
            mSB.AppendLine("Lot ID="    + CData.Parts[(int)nPart].sLotName);
            mSB.AppendLine("Strip ID=" + CData.Parts[(int)nPart].sBcr);
            mSB.AppendLine();

            if (CData.Dev.eMoldSide == ESide.Top)
                mSB.AppendLine("[" + ESide.Top.ToString() + " Side Info]");

            mSB.AppendLine("Recipe Name=" + CData.Dev.sName);

            sStage = (nWay == (int)EWay.L) ? "Left" : "Right";
            mSB.AppendLine("Stage=" + sStage);
            mSB.AppendLine("Dual Mode="   + CData.Dev.bDual.ToString());

            sGrindMode = (CData.Dev.aData[(int)nWay].eGrdMod == eGrdMode.TopDown) ? "TopDown" : "Target";
            mSB.AppendLine("Grind Mode="  + sGrindMode);
            mSB.AppendLine("Save Time="   + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            mSB.AppendLine();

            mSB.AppendLine("PCB Min=" + CData.Parts[(int)nPart].dPcbMin);
            mSB.AppendLine("PCB Max=" + CData.Parts[(int)nPart].dPcbMax);
            mSB.AppendLine("PCB Avg=" + CData.Parts[(int)nPart].dPcbMean);
            mSB.AppendLine();

            mSB.AppendLine("BF Min=" + CData.PbResultVal[(int)nWay].dBMin);
            mSB.AppendLine("BF Max=" + CData.PbResultVal[(int)nWay].dBMax);
            mSB.AppendLine("BF Avg=" + CData.PbResultVal[(int)nWay].dBAvg);
            mSB.AppendLine();

            mSB.AppendLine("AF Min=" + CData.PbResultVal[(int)nWay].dAMin);
            mSB.AppendLine("AF Max=" + CData.PbResultVal[(int)nWay].dAMax);
            mSB.AppendLine("AF Avg=" + CData.PbResultVal[(int)nWay].dAAvg);
            mSB.AppendLine();

            //210811 pjh : D/F Server Data Save 기능 Net Drive 추가
            if (Opt.bDFNetUse)
            {
                StringBuilder mDFPath = new StringBuilder(@"\\");
                mDFPath.Append(CData.Opt.sNetIP);
                mDFPath.Append(CData.Opt.sNetPath);
                mDFPath.Append("\\Suhwoo\\SG2000X\\DfServer\\");
                mDFPath.Append(CData.Parts[(int)nPart].sLotName + "\\");
                mDFPath.Append(CData.Parts[(int)nPart].sBcr + "\\");
                mDFPath.Append(CData.Parts[(int)nPart].sBcr + "_" + ESide.Top.ToString() + ".txt");
                CData.QueNet.Enqueue(new TNetDrive(mDFPath.ToString(), mSB.ToString()));
            }
            else
            {
                sPath += CData.Parts[(int)nPart].sLotName + "\\" + CData.Parts[(int)nPart].sBcr + "\\";

                if (!Directory.Exists(sPath)) { Directory.CreateDirectory(sPath); }

                if (nTopSide == (int)ESide.Top)
                {
                    sPath += CData.Parts[(int)nPart].sBcr + "_" + ESide.Top.ToString() + ".txt";
                }
                else
                { return; }

                CLog.Check_File_Access(sPath, mSB.ToString(), false); 
            }
            //
        }
        // 2021.05.28 SungTae End
    }
}
