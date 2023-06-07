
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Drawing;
using SECSGEM;          // 2021.08.09 SungTae : [추가] (ASE-KR VOC) Remote 상태에서 Carrier List 처리를 위해 추가

public class TLotInfo
{
    public string sLotName   ;
    public string sToolId    ; //190410 ksg : ASE secs/gem 용
    public int    iTotalMgz  ;
    public int    iTotalStrip;//koo : Qorvo Lot Rework
    public bool   bLotOpen   ; //ksg
    public bool   bLotEnd    ; //ksg
    public int    iTInCnt    ; //190407 ksg :
    public int    iTOutCnt   ; //190407 ksg :
    public string sLToolId   ; //190802 ksg : Tool Id & Dresser Id
    public string sRToolId   ; //190802 ksg : Tool Id & Dresser Id
    public string sLTool_Serial_Num; //200628 LCY 현재 Wheel Serial Num
    public string sRTool_Serial_Num; //200628 LCY 현재 Wheel Serial Num
    public string sCurr_LToolId; //191113 LCY 현재 Tool ID
    public string sCurr_RToolId; //191113 LCY 현재 Tool ID

    public string sLDrsId    ; //190802 ksg : Tool Id & Dresser Id
    public string sRDrsId    ; //190802 ksg : Tool Id & Dresser Id
    public string sCurr_LDrsId; //191113 LCY 현재 Dress ID
    public string sCurr_RDrsId; //191113 LCY 현재 Dress ID
    public string sGMgzId    ; //191023 ksg : Good Magazine Id

    public string sNMgzId    ; //191023 ksg : NG Magazine Id
    public DateTime dtOpen; //200116 jym : Lot open time
    public DateTime dtEnd; //200116 jym : Lot end time

    public string sNewLotName;//211210 pjh : 중복된 Lot Name 입력 시 자동으로 변경 될 Name
    /// <summary>
    /// SCK의 경우 Top Mold 자재 측정만 진행 -> Btm Mold -> Btm Mold Grinding 순으로 진행
    /// 동일 Lot가 1시간 이후에 들어오기 때문에 중복 Lot 투입을 체크하는 폴더 명에 시간을 추가
    /// </summary>
    public int iLotOpenHour; //20200827 lks     // 2020.09.11 JSKim Add

    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    /// <summary>
    /// Lot Open 후 첫 n개 스트립 중 현재 측정 진행한 스트립 수 (Before Measure 시작 시점 기준으로 카운팅)
    /// Lot Open 시 0으로 초기화
    /// Measure 함수 내에서 첫 n개의 스트립(Leading Strip)에 해당할 경우 +1 증가 
    /// </summary>
    public int i18PMeasuredCount;


    // 2021-05-06, jhLee : Multi-LOT을 위해 LOT 상태 추가
    public ELotState nState;            // LOT 진행 상태
    public int nType;                   // 화면에 보여주는 Type
    public int nInMgzCnt;               // 투입 Magazine 수량

    public DateTime dtClose;            // 투입이 종료된 시각 

    public TSpcInfo rSpcInfo;           // SPC 정보 저장/관리용
    public TSpcData rSpcData;           // SPC 데이터
    public Color LotColor;              // LOT의 색 지정

    public ELotEndMode nLotEndMode;             // 투입시 LOT 구분에 대한 모드, 투입 종료와 LOT 분리를 위한 지정
                                                //of Multi-LOT

    // 2021.08.09 SungTae : [추가] (ASE-KR VOC) Remote 상태에서 Carrier List 처리를 위해 추가
    public ManagerLot m_pHostLOT = new ManagerLot();

    // 2021-05-06, jhLee, Multi-LOT 관리를 위해 값을 초기화 한다.
    public TLotInfo()
    {
        sLotName = "";
        sToolId = "";               
        iTotalMgz = 0;          // 투입시킬 magazine 수량
        iTotalStrip = 0;        // 투입시킬 Strip 수량
        bLotOpen = false;
        bLotEnd = false;
        iTInCnt = 0;
        iTOutCnt = 0;
        sLToolId = ""; // Tool Id & Dresser Id
        sRToolId = ""; // Tool Id & Dresser Id
        sLTool_Serial_Num = ""; //200628 LCY 현재 Wheel Serial Num
        sRTool_Serial_Num = ""; //200628 LCY 현재 Wheel Serial Num
        sCurr_LToolId = ""; // 현재 Tool ID
        sCurr_RToolId = ""; // 현재 Tool ID

        sLDrsId = ""; // Tool Id & Dresser Id
        sRDrsId = ""; // Tool Id & Dresser Id
        sCurr_LDrsId = ""; // 현재 Dress ID
        sCurr_RDrsId = ""; // 현재 Dress ID
        sGMgzId = ""; //  Good Magazine Id

        sNMgzId = ""; // NG Magazine Id
        dtOpen = DateTime.Now; // Lot open time
        dtEnd = DateTime.Now; // Lot end time
        iLotOpenHour = 0; //
        i18PMeasuredCount = 0;

        nState = ELotState.eWait;          // LOT 진행 상태
        dtClose = DateTime.Now; ;          // 투입이 종료된 시각 

        rSpcInfo = new TSpcInfo();           // SPC 정보 저장/관리용
        rSpcData = new TSpcData();           // SPC 데이터

        nLotEndMode = ELotEndMode.eEmptySlot;              // 투입시 LOT 구분에 대한 모드, 투입 종료와 LOT 분리를 위한 지정

        sNewLotName = "";//211210 pjh : 중복된 Lot Name 입력 시 자동으로 변경 될 Name
    }
}


#region Motion
/// <summary>
/// Motion Move Parameter
/// </summary>
public struct tMV
{
    /// <summary>
    /// Velocity [mm/s]
    /// </summary>
    public int iVel;

    /// <summary>
    /// Acceleration [mm/s^2]
    /// </summary>
    public int iAcc;

    /// <summary>
    /// Deceleration [mm/s^2]
    /// </summary>
    public int iDec;
}
/// <summary>
/// Axis Parameter
/// </summary>
public struct tAx
{
    /// <summary>
    /// bUse    false : Not Use, true : Use
    /// </summary>
    public bool bUse;
    /// <summary>
    /// Ball screw pitch [mm]
    /// </summary>
    public double dBSP;
    /// <summary>
    /// 1mm 에 필요한 pulse값 [p]
    /// </summary>
    public double dPP1;
    /// <summary>
    /// Run Normal
    /// </summary>
    public tMV tRN;
    /// <summary>
    /// Run Slow
    /// </summary>
    public tMV tRS;
    /// <summary>
    /// Jog Normal
    /// </summary>
    public tMV tJN;
    /// <summary>
    /// Software Max limit position [mm]
    /// </summary>
    public double dSWMax;
    /// <summary>
    /// Software Min limit position [mm]
    /// </summary>
    public double dSWMin;
    /// <summary>
    /// NOT sensor use     false:Not use, true:Use
    /// </summary>
    public bool bNOT;
    /// <summary>
    /// POT sensor use     false:Not use, true:Use
    /// </summary>
    public bool bPOT;
    /// <summary>
    /// Acceleration Ratio 가속 적용 비율 10=0.1초 5=0.2초
    /// </summary>
    public int iAccR;
    /// <summary>
    /// Deceleration Ratio 감속 적용 비율 10=0.1초 5=0.2초
    /// </summary>
    public int iDecR;
}
#endregion

#region Spindle
public struct tSplData
{
    public BitArray aStat;
    public int iRpm;
    public string sErr;
    public double dLoad;
    public double dTemp_Val;
    public int nCurrent_Amp;
};
#endregion

#region Barcode
public struct tBcr
{
    public bool bRun    ; //BCR & OCR & Ori 실행 상태
    public bool bCon    ; //연결 상태                    //190211 ksg : 추가 항목 
    public bool bRet    ; //Bcr 읽은 결과 
    public bool bRetOri ; //Ori 읽은 결과                //190211 ksg : 추가 항목 
    public bool bRetTOri; //Table Ori 읽은 결과          //200118 ksg : 추가 항목 
    public bool bRetOcr ; //Ocr 읽은 결과                //190309 ksg : 추가 항목 
    public bool bRetCom ; //Bcr & Ocr 읽고 비교 결과     //190309 ksg : 추가 항목 
    public bool bRetMark; //Mark 읽은 결과               //190712 ksg : 추가 항목 
    public bool bTrigger; //X의 트리거 0 : 대기, 1 : 측정
    public bool bBcr    ; //X의 트리거                   //190211 ksg : 추가 항목 
    public bool bOcr    ; //X의 트리거                   //190309 ksg : 추가 항목 
    public bool bOri    ; //X의 트리거                   //190211 ksg : 추가 항목 
    public bool bTOri   ; //X의 트리거                   //200118 ksg : 추가 항목 -> Table Ori 기능 추가
    public bool bMark   ; //X의 트리거                   //190712 ksg : 추가 항목 
    //--------------------------------------------
    public bool bBcr_Ir ; //X의 트리거(Inrail     ) //200129 ksg :
    public bool bOri_Ir ; //X의 트리거(Inrail     ) //200129 ksg :
    public bool bOcr_Ir ; //X의 트리거(Inrail     ) //200129 ksg :
    public bool bMark_Ir; //X의 트리거(Inrail     ) //200129 ksg :
    public bool bBcr_Gl ; //X의 트리거(Left Table ) //200129 ksg :
    public bool bOri_Gl ; //X의 트리거(Left Table ) //200129 ksg :
    public bool bOcr_Gl ; //X의 트리거(Left Table ) //200129 ksg :
    public bool bMark_Gl; //X의 트리거(Left Table ) //200129 ksg :
    public bool bBcr_Gr ; //X의 트리거(Right Table) //200129 ksg :
    public bool bOri_Gr ; //X의 트리거(Right Table) //200129 ksg :
    public bool bOcr_Gr ; //X의 트리거(Right Table) //200129 ksg :
    public bool bMark_Gr; //X의 트리거(Right Table) //200129 ksg :
    //--------------------------------------------
    public string sBcr  ; //Bcr 읽은 String
    public string sOcr  ; //Ocr 읽은 String              //190309 ksg : 추가 항목 
    public string sDX   ; //190211 ksg : 추가 항목 
    public string sBCR  ; //190211 ksg : 추가 항목 
    public bool bStatusBcr; //190827 ksg : 추가 항목, Bcr 상태
    //--------------------------------------------
    //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
    public int iUserLevel;  //User Level (2D Vision 화면에 표시할 항목 조절 용) : 0=Operator / 1=Technician or Engineer or Master
    public int iUIMode;     //2D Vision 현재 화면 모드 : 0=Auto Mode Window, 1=Training Mode Window
    //
    //--------------------------------------------
    // 2022-05-26, jhLee : Skyworks OCR 글자수가 10자리 및 14자리 지원, ID Vision에서 Job 파일을 변경해준다.
    public int iDigitType;      // 글자수 타입, 0:10자리 (기존), 1:14자리 (신규)
}
//20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드, SPIL)
/// <summary>
/// KEYENCE 바코드 리더와 통신 상태 및 결과 데이터 관리용
/// </summary>
public struct tKeyenceBcr
{
    public eKeyenceBcrTcpStatus eState;     //BCR 동작 상태   : 0=Ready / 1=Command 송신 완료 (Command Response 수신 대기)/ 2=Command Response 수신 (Data 수신 대기) / 3=Data 수신 완료
    public string               sCmd;       //Command        : "" / "LON" / "LOFF" / "BCLR"
    public string               sCmdResult; //Command Result : "" / "OK" / "ER"
    public string               sErrCode;   //Error Code     : "" / "00"~"99" (Command Result가 "ER"인 경우에 함께 수신됨)
    public string               sBcr;       //Data Response  : 읽은 바코드 값(LON 전송 -> OK 수신 -> 바코드 수신) / "ERROR"(LON 전송 -> LOF 전송 -> OK LOFF 수신 -> 후 OK Response 전 LOF 전송시)
    public bool                 bStErr;     //통신 처리 중 Error 발생 상태 : true=에러상태 / false=정상상태
}
#endregion

#region Wheel
public struct tWhl
{
    #region Wheel Information
    /// <summary>
    /// 드레싱 총 카운트
    /// </summary>
    public int iDrsC;
    /// <summary>
    /// Grinding Total Count 
    /// 스트립 그라인딩 총 카운트 [ea]
    /// </summary>
    public int iGtc;
    /// <summary>
    /// Dressing After Grinding Count 
    /// 드레싱 이후 스트립 그라인딩 카운트 [ea]
    /// </summary>
    public int iGdc;
    /// <summary>
    /// Wheel Outer 
    /// 휠의 외경 [mm]
    /// </summary>
    public double dWhlO;
    /// <summary>
    /// 최초 측정 휠 높이 값
    /// </summary>
    public double dWhlFtH;
    /// <summary>
    /// Wheel Tip Height 
    /// 팁의 높이 초기 값 10 [mm]
    /// </summary>
    public double dWhltH;
    /// <summary>
    /// 그라인딩 간의 자재에 따른 보정이 적용된 휠 높이 [mm]
    /// </summary>
    public double dWhlvH;
    /// <summary>
    /// Wheel Total Loss 
    /// 휠 전체적인 로스량 (최초 측정 값 - 현재 높이) [mm]
    /// </summary>
    public double dWhltL;
    /// <summary>
    /// Wheel 1 Strip Loss 
    /// 1장의 스트립의 로스량 [mm]
    /// </summary>
    public double dWhloL;
    /// <summary>
    /// 1 Dressing Loss 
    /// 드레싱 1번당 로스량 [mm]
    /// </summary>
    public double dWhldoL;
    /// <summary>
    /// Dressing Cycle Loss 
    /// 드레싱 사이클 사이의 로스량 (드레싱 끝 이후 높이 - 다음 드레싱 시작 전 높이) [mm]
    /// </summary>
    public double dWhldcL;
    /// <summary>
    /// 측정 전 휠 높이
    /// </summary>
    public double dWhlBf;
    /// <summary>
    /// 측정 후 휠 높이
    /// </summary>
    public double dWhlAf;
    /// <summary>
    /// 휠 리미트
    /// </summary>
    public double dWhlLimit;
    /// <summary>
    /// 드레셔 리미트
    /// </summary>
    public double dDrsLimit;
    /// <summary>
    /// Dressing Air Cut Thickness 
    /// 드레싱 에어컷 [mm]
    /// </summary>
    public double dDair;
    // 2020.11.23 JSKim St
    /// <summary>
    /// Dressing Air Cut Replace Thickness 
    /// 드레싱 에어컷 [mm]
    /// </summary>
    public double dDairRep;
    // 2020.11.23 JSKim Ed
    /// <summary>
    /// Wheel Name (Serial Number) 
    /// 휠 이름 (시리얼 넘버 - 입력 값) 
    /// </summary>
    public string sWhlName;
    /// <summary>
    /// Wheel File Last Save Date 
    /// 파일의 마지막 수정 날짜
    /// </summary>
    public DateTime dtLast;
    //20190421 ghk_휠 리그라인딩 옵셋
    /// <summary>
    /// 리그라인딩 횟수에 따른 그라인딩 스타트 위치 조정 변수
    /// </summary>
    public double dReGrdOffset;
    /// <summary>
    /// 휠 파트 넘버 (ASE KR 전용)
    /// </summary>
    public string sPartNo;
    /// <summary>
    /// Mesh 정보
    /// </summary>
    public string sMesh;
    #endregion

    #region Dresser Infomation
    /// <summary>
    /// Dresser Outer 
    /// 드레셔 외경 (입력 불가) [mm]
    /// </summary>
    public double dDrsOuter;
    /// <summary>
    /// Dresser Height 
    /// 드레셔 높이 (입력 불가) [mm]
    /// </summary>
    public double dDrsH;
    /// <summary>
    /// Dresser Name 
    /// 드레셔 이름 (입력 값)
    /// </summary>
    public string sDrsName;
    /// <summary>
    /// 측정 전 드레셔 높이
    /// </summary>
    public double dDrsBf;
    /// <summary>
    /// 측정 후 드레셔 높이
    /// </summary>
    public double dDrsAf;
    /// <summary>
    /// Dressing Used Parameter Array 
    /// 사용 중에 사용되는 드레싱 파라메터
    /// </summary>
    public tStep[] aUsedP;
    /// <summary>
    /// Dressing New Parameter Array 
    /// 휠 교체시 사용되는 드레싱 파라메터
    /// </summary>
    public tStep[] aNewP;
    #endregion
}

public struct tWhlLog
{
    /// <summary>
    /// Wheel Log Time
    /// 휠 로그 시간
    /// col 1
    /// </summary>
    public string sDate;
    /// <summary>
    /// Wheel Measure Type
    /// 휠 측정 시점 ex)그라인딩 전/후, 드레싱 전/후
    /// col 2
    /// </summary>
    public string sMeaType;
    /// <summary>
    /// Wheel Tip Height
    /// 휠 팁의 높이 [mm]
    /// col 3
    /// </summary>
    public double dWhltipH;
    /// <summary>
    /// Before Wheel Tip - Now Wheel Tip 
    /// 이전의 히스토리 휠 팁에서 현재 휠 팁 값을 빼준 양 [mm]
    /// col 4
    /// </summary>
    public double dWhltL;
    /// <summary>
    /// Dresser Name
    /// 드레셔 이름
    /// col 5
    /// </summary>
    public string sDrsName;
    /// <summary>
    /// Dresser Height
    /// 드레셔 높이 [mm]
    /// col 6
    /// </summary>
    public double dDrsH;
    /// <summary>
    /// Before Dresser Height - Now Dresser Height 
    /// 이전의 히스토리 드레셔 높이에서 현재 드레셔 높이 값을 빼준 양 [mm]
    /// col 7
    /// </summary>
    public double dDrsL;
    /// <summary>
    /// Grinding Total Count 
    /// 스트립 그라인딩 총 카운트 [ea]
    /// col 8
    /// </summary>
    public int iGtc;
    /// <summary>
    /// Dressing Count 
    /// 드레싱 진행한 횟수 [ea]
    /// col 9
    /// </summary>
    public int iDrsC;
    /// <summary>
    /// Cycle Dressing count
    /// 드레싱 사이클 사이에 그라인딩한 스트립 장수
    /// col 10
    /// </summary>
    public int iDrsCycleStrip;
    /// <summary>
    /// 1 Strip Loss
    /// 한장당 휠 소모량 -> dDrsL / iDrsCycleStrip
    /// col 11
    /// </summary>
    public double dOneStripLoss;
    /// <summary>
    /// Air Cut
    /// 드레싱 에어컷
    /// col 12
    /// </summary>
    public double dAirCut;
    /// <summary>
    /// Step1 Total Thickness
    /// 스텝1 드레싱 양
    /// col 13
    /// </summary>
    public double dStep1Total;
    /// <summary>
    /// Step1 Cycle Thickness
    /// 스텝1 사이클 두께
    /// col 14
    /// </summary>
    public double dStep1Cycle;
    /// <summary>
    /// Step1 Table Speed
    /// 스텝1 테이블 스피드
    /// col 15
    /// </summary>
    public double dStep1TbSpeed;
    /// <summary>
    /// Step1 RPM
    /// 스텝1 RPM
    /// col 16
    /// </summary>
    public int iStep1Rpm;
    /// <summary>
    /// Step2 Total Thickness
    /// 스텝2 드레싱 양
    /// col 17
    /// </summary>
    public double dStep2Total;
    /// <summary>
    /// Step2 Cycle Thickness
    /// 스텝2 사이클 두께
    /// col 18
    /// </summary>
    public double dStep2Cycle;
    /// <summary>
    /// Step2 Table Speed
    /// 스텝2 테이블 스피드
    /// col 19
    /// </summary>
    public double dStep2TbSpeed;
    /// <summary>
    /// Step2 RPM
    /// 스텝2 RPM
    /// col 20
    /// </summary>
    public int iStep2Rpm;
}
#endregion

//210106 pjh : Dresser Log 항목 추가
#region Dresser
public struct tDrsLog
{
    /// <summary>
    /// Wheel Log Time
    /// Dresser Log Time
    /// col 1
    /// </summary>
    public string sDate;
    /// <summary>
    /// Wheel Measure Type
    /// 휠 측정 시점 ex)그라인딩 전/후, 드레싱 전/후
    /// col 2
    /// </summary>
    public string sMeaType;
    /// <summary>
    /// Dresser Name
    /// 드레셔 이름
    /// col 3
    /// </summary>
    public string sDrsName;
    /// <summary>
    /// Dresser Height
    /// 드레셔 높이 [mm]
    /// col 4
    /// </summary>
    public double dDrsH;
    /// <summary>
    /// Before Dresser Height - Now Dresser Height 
    /// 이전의 히스토리 드레셔 높이에서 현재 드레셔 높이 값을 빼준 양 [mm]
    /// col 5
    /// </summary>
    public double dDrsL;
}
#endregion

#region Setup Position
/// <summary>
/// Main Positon Struct 장비 메인 포지션 구조체
/// </summary>
public struct tMnP
{
    #region Setup manual position
    /// <summary>
    /// Probe Measure Table Base
    /// Manual -> 프로브가 테이블의 베이스를 측정하여 0이 되는 Z축의 위치 값 [mm]
    /// </summary>
    public double dZ_TBL_BASE;

    /// <summary>
    /// Wheel Base Measrue Tool Setter
    /// Manual -> 팁이 없는 휠로 툴세터 측정 되는 Z축의 위치 값 [mm]
    /// </summary>
    public double dZ_WHL_BASE;

    /// <summary>
    /// Probe Measure Dresser Base
    /// Manual -> 프로브가 드레셔의 베이스를 측정하여 0이 되는 Z축의 위치 값 [mm]
    /// </summary>
    public double dZ_DRS_BASE;

    /// <summary>
    /// Probe Measure Tool Setter
    /// Manual -> 프로브가 툴세터를 측정하여 0이 되는 Z축의 위치 값 [mm]
    /// </summary>
    public double dZ_PRB_TOOL_SETTER;

    /// <summary>
    /// Probe Table Center
    /// Manual -> 프로브가 테이블의 중앙을 측정하는 Y축의 위치 값 [mm]
    /// </summary>
    public double dY_PRB_TBL_CENTER;

    /// <summary>
    /// Wheel Center Table Center
    /// Manual -> 휠의 중앙이 테이블의 중앙을 위치하는 Y축의 위치 값 [mm]
    /// </summary>
    public double dY_WHL_TBL_CENTER;

    /// <summary>
    /// Tool Setter Gap
    /// Wheel base로 툴세터 측정 시 툴세터가 눌리는 갭 [mm]
    /// </summary>
    public double dTOOL_SETTER_GAP;

    /// <summary>
    /// Probe Offset
    /// 실측과 측정치의 편차를 조정하는 변수    측정값 + 옵셋 [mm]
    /// </summary>
    public double dPRB_OFFSET;

    /// <summary>
    /// PCB Thickness
    /// PCB 두께를 입력하여 처박는거 방지(Left & Right 동일, 한 변수로 씀) [mm]
    /// </summary>
    public double dPCBThickness;
    #endregion

    #region Setup auto calculate position
    /// <summary>
    /// Table Thickness Measure Start Z Position
    /// Calc -> 테이블 측정 시작 높이 (Z_TABLE_BASE - 19.0mm) [mm]
    /// </summary>
    public double dZ_TBL_MEA_POS;

    /// <summary>
    /// Dresser Thickness Measure Start Z Position
    /// Calc -> 드레셔 측정 시작 높이 (Z_DRESSER_BASE - 7.5mm) [mm]
    /// </summary>
    public double dZ_DRS_MEA_POS;

    /// <summary>
    /// Wheel Center Tool Setter Table Position
    /// Calc -> 휠의 중앙이 툴세터의 위치하는 Y축의 위치 값 [mm]
    /// </summary>
    public double dY_WHL_TOOL_SETTER;

    /// <summary>
    /// Wheel Center Dresser Center Table Position
    /// Calc -> 휠의 중앙이 드레셔의 중앙을 위치하는 Y축의 위치 값 [mm]
    /// </summary>
    public double dY_WHL_DRS;

    /// <summary>
    /// Probe Dresser Center  Table Position
    /// Calc -> 프로브가 드레셔의 중앙을 위치하는 Y축의 위치 값 [mm]
    /// </summary>
    public double dY_PRB_DRS;

    /// <summary>
    /// Probe By Wheel Base Distance
    /// Calc -> 프로브의 끝(프로브의 제로 값)과 휠 베이스의 차이 값 [mm]
    /// </summary>
    public double dPRB_TO_WHL_BASE;

    #endregion

    // 210426 jym
    /// <summary>
    /// After measure 데이터에 더하는 옵셋 [mm]
    /// </summary>
    public double fake;
}
/// <summary>
/// System Position Struct 시스템 포지션 구조체
/// </summary>
public struct tSyP
{
    #region OnLoader X
    public double dONL_X_Wait;
    /// <summary>
    /// 240mm 기준 포지션
    /// </summary>
    public double dONL_X_DefAlign;
    #endregion

    #region OnLoader Y
    public double dONL_Y_Pick;
    public double dONL_Y_Place;
    #endregion

    #region OnLoader Z
    public double dONL_Z_Wait;
    public double dONL_Z_PickGo;
    public double dONL_Z_Clamp;
    public double dONL_Z_PickUp;
    public double dONL_Z_PlaceGo;
    public double dONL_Z_UnClamp;
    public double dONL_Z_PlaceDn;
    #endregion

    #region InRail X
    public double dINR_X_Wait;
    public double dINR_X_ChkUnit;     // 2021.05.03 SungTae : 2003U 개조건 관련 Unit #1 Check Position 추가
    #endregion

    #region InRail Y
    public double dINR_Y_Wait;
    /// <summary>
    /// 디폴트 얼라인 포지션 - 기준 74mm
    /// </summary>
    public double dINR_Y_Align;
    #endregion

    #region OnPicker X
    public double dONP_X_PlaceL;
    public double dONP_X_PlaceR;
    public double dONP_X_Con   ; //190321 ksg :
    public double dONP_X_WaitPickL; //20200406 jhc : OnPicker L-Table Pickup 대기 위치
    #endregion

    #region OnPicker Z
    public double dONP_Z_Wait;
    #endregion

    //20190604 ghk_onpbcr
    #region OnPicker Y
    public double dONP_Y_Wait;
    #endregion

    #region Grd X
    public double[] dGRD_X_Zero;
    public double[] dGRD_X_Wait;
    #endregion

    #region Grd Y
    public double[] dGRD_Y_Wait;
    /// <summary>
    /// 테이블 클리닝 시작 포지션
    /// </summary>
    public double[] dGRD_Y_ClnStart;
    public double[] dGRD_Y_ClnEnd;
    public double[] dGRD_Y_DrsStart;
    public double[] dGRD_Y_DrsEnd;
    public double[] dGRD_Y_DrsChange;
    /// <summary>
    /// 테이블 상단 측정 위치
    /// </summary>
    public double[] dGRD_Y_TblInsp;

    // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
    /// <summary>
    /// Table Grinding Start Position
    /// </summary>
    public double[] dGRD_Y_TblGrdSt;
    /// <summary>
    /// Table Grinding End Position
    /// </summary>
    public double[] dGRD_Y_TblGrdEd;
    // 2021.04.13 SungTae End

    #endregion

    #region Grd Z
    public double[] dGRD_Z_Wait;
    /// <summary>
    /// 테이블이 움직이기 위한 Z축의 허용 가능 높이
    /// </summary>
    public double[] dGRD_Z_Able;
    #endregion

    #region OffPicker X
    public double dOFP_X_Wait ;
    public double dOFP_X_PickL;
    public double dOFP_X_PickR;
    public double dOFP_X_Place;
    public double dOFP_X_Conv ; //190321 ksg :
    public double dOFP_X_Picture1;    // 2021.03.30 lhs  // 화상판별센서(IV2) 찍는 X축 #1 위치
    public double dOFP_X_Picture2;    // 2021.03.30 lhs  // 화상판별센서(IV2) 찍는 X축 #2 위치
    public double dOFP_X_IV2Cover; //210831 syc : 2004U IV2 - Cover 검사 포지션
    #endregion

    #region OffPicker Z
    public double dOFP_Z_Wait;
    public double dOFP_Z_ClnStart;
#if false //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정   // 2022.03.08 lhs 삭제
    public double dOFP_Z_StripClnStart; //Strip Clean 높이  
#endif
    public double dOFP_Z_Picture;    // 2021.03.30 lhs  // 화상판별센서(IV2) 찍는 Z축 위치(#1, #2 공통)
    public double dOFP_Z_IV2Cover; //210831 syc : 2004U IV2 - Cover 검사 포지션
    #endregion

    #region Dry X
    public double dDRY_X_Wait;
    /// <summary>
    /// 최종 배출 위치
    /// </summary>
    public double dDRY_X_Out;
    #endregion

    #region Dry Z
    public double dDRY_Z_Wait;
    public double dDRY_Z_Up;
    public double dDRY_Z_Check;
    public double dDRY_Z_TipClamped;  //2021.04.22 lhs
    public double dDRY_Z_ChkStrip;         // 2022.01.18 SungTae Start : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청으로 추가
    #endregion

    #region Dry R
    public double dDRY_R_Wait;
    #endregion

    #region Off Loader X
    public double dOFL_X_Wait;
    /// <summary>
    /// 240mm 기준 포지션
    /// </summary>
    public double dOFL_X_DefAlign;
    #endregion

    #region Off Loader Y
    public double dOFL_Y_Pick ;
    public double dOFL_Y_Place;
    #endregion

    #region Off Loader Z
    public double dOFL_Z_Wait    ;
    public double dOFL_Z_TPickGo ;
    public double dOFL_Z_TClamp  ;
    public double dOFL_Z_TPickUp ;
    public double dOFL_Z_TPlaceGo;
    public double dOFL_Z_TUnClamp;
    public double dOFL_Z_TPlaceDn;
    public double dOFL_Z_BPickGo ;
    public double dOFL_Z_BClamp  ;
    public double dOFL_Z_BPickUp ;
    public double dOFL_Z_BPlaceGo;
    public double dOFL_Z_BUnClamp;
    public double dOFL_Z_BPlaceDn;
    #endregion
}
#endregion

/// <summary>
/// Option Struct
/// </summary>
public struct tOpt
{
    /// <summary>
    /// Warm Up Time 웜업 시간 [m]
    /// </summary>
    public int iWmUT;
    /// <summary>
    /// Warm Up Spindle Speed 웜업 스핀들 속도 [rpm]
    /// </summary>
    public int iWmUS;
    /// <summary>
    /// 오토 웜업 진행 시간 [min]
    /// </summary>
    public int iAWT;
    /// <summary>
    /// 오토 웜업 진행 간격 [min]
    /// </summary>
    public int iAWP;
    // 201029 jym : Add
    /// <summary>
    /// 오토 웜업 스킵 여부
    /// </summary>
    public bool bAwSkip;
    // 201029 jym End
  
    /// <summary>
    /// Wheel Measure Tip Height Spindle Speed 휠 측정 시 스핀들 속도 [rpm]
    /// </summary>
    public int iMeaS;
    /// <summary>
    /// Wheel Measure Tip Height Delay Time 횔 측정 시 대기 시간 [ms]
    /// </summary>
    public int iMeaT;

    /// <summary>
    /// 온로더 피커 테이블 Place 할때 워터 대기 시간
    /// </summary>
    public int[] aOnpRinseTime;

    //20190926 ghk_ofppadclean
    public int iOfpPadCleanTime;
    //

    /// <summary>
    /// Table Grinding Step Parameter Array
    /// </summary>
    public tStep[] aTbGrd;
    /// <summary>
    /// Table Grinding Air Cut Thickness [mm]
    /// </summary>
    public double dTbGrdAir;
    /// <summary>
    /// Last Left Table Grinding Time
    /// </summary>
    public DateTime dtLast_L;
    /// <summary>
    /// Last Right Table Grinding Time
    /// </summary>
    public DateTime dtLast_R;
    /// <summary>
    /// LOT Magazine Max Count
    /// LOT 오픈시 설정 할 수 있는 최대 매거진 갯수
    /// </summary>
    public int iLotCnt;
    /// <summary>
    /// Magazine Empty Count
    /// 연속 Magazin Empty Slot Push 할때 Mgz 자동 배출 
    /// </summary>
    public int iEmptySlotCnt;
    /// <summary>
    /// Wheel 측정 시 TTV 제한 값 [mm]
    /// </summary>
    //public double dWhlTTV;
    public double[] aWhlTTV;

    /// <summary>
    /// 
    /// </summary>
    public int iChangeLevel; //190509 ksg :

    /// <summary>
    /// Table Skip false:Use, true:Skip 
    /// </summary>
    public bool[] aTblSkip;
    /// <summary>
    /// Door Skip false:Use, true:Skip 
    /// </summary>
    public bool bDoorSkip;
    /// <summary>
    /// Cover Skip false:Use, true:Skip 
    /// </summary>
    public bool bCoverSkip;
    /// <summary>
    /// Dry Zone Top Blow Stick Skip false:Use, true:Skip 
    /// </summary>
    public bool bDryStickSkip;
    /// <summary>
    /// Grind Skip false:Use, true:Skip (Dry Auto Run Mode) 
    /// </summary>
    public bool bDryAuto;
    /// <summary>
    /// OffLoadPicker Bottom Clean : false:Use, true:Skip (ASE K 빨판 사용으로 인하여) 
    /// </summary>
    public bool bOfpBtmCleanSkip; //190109 ksg : 추가

    // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Position 추가
    /// <summary>
    /// OffLoadPicker Strip Clean Center Pos Use : false:Not Use, true:Use
    /// </summary>
    public bool bOfpUseCenterPos;

    // 2021.04.02 lhs Start
    /// <summary>
    /// OffPicker로 Dryer의 물 튐 방지용으로 커버 역할
    /// </summary>
    public bool bOfpCoverDryerSkip;
    // 2021.04.02 lhs End

    //20190628 ghk_automeasure
    /// <summary>
    /// Measure Strip When Skip Grinding  false:Use, true:Skip (Dry Auto Run Mode)
    /// </summary>
    public bool bDryAutoMeaStripSkip;
    //

    /// <summary>
    /// Leak Sensor Check : false:Use, true:Skip 
    /// </summary>
    public bool bLeakSkip; //190109 ksg : 추가

    /// <summary>
    /// Wheel Jig Use : Use:false, Skip:true 
    /// </summary>
    public bool bWhlJigSkip; //190213 ksg : 추가
    
    /// <summary>
    /// 2매거진이 Full Slot이 아니고 2개 매거진 자재를 합쳐도 1개 매거진이 full Slot이 아닐경우
    /// On  : 투입 매거진과 동일하게 배출 매거진 발생
    /// off : 배출 매거진이 full Slot 또는 자재가 없을시 배출 
    /// </summary>
    public bool bOfLMgzMatchingOn; //190213 ksg : 추가

    // 2021.12.08 SungTae Start : [추가] (Qorvo향 VOC) 
    /// <summary>
    /// QC-Vision 사용 시 판정에 상관없이 BTM MGZ 하나에 전부 적재하여 배출하는 경우
    /// On  : 모두 Btm Mgz에 적재 후 배출 (Only Qorvo)
    /// off : Good은 Btm Mgz에 적재, NG는 Top Mgz에 적재 후 배출
    /// </summary>
    public bool bOfLMgzUnloadType;
    // 2021.12.08 SungTae End

    /// <summary>
    /// On Loader Mgz First 작업 위치 선택
    /// On  : Top Slot부터 작업진행
    /// off : Bottom Slot부터 작업진행
    /// </summary>
    public bool bFirstTopSlot; //190503 ksg : 추가

    /// <summary>
    /// Off Loader Mgz First 작업 위치 선택
    /// On  : Top Slot부터 작업진행
    /// off : Bottom Slot부터 작업진행
    /// </summary>
    public bool bFirstTopSlotOff; //190511 ksg : OFF Loader 따로 생성

    /// <summary>
    /// Warm Up Skip 0 : Use, 1 : Skip
    /// </summary>
    public bool bWarmUpSkip; //190218 ksg : 추가

    /// <summary>
    /// QCVision Part Use 0 : Skip, 1 : Use
    /// </summary>
    public bool bQcUse; //190403 ksg : Qc 추가
    public bool bQcSimulation; //190403 ksg : Qc 추가

    /// <summary>
    /// QCVision Test / ByPass Test : 0, ByPass : 1
    /// </summary>
    public bool bQcByPass; //190406 ksg : Qc 추가
    // 201116 jym : 기능 추가
    /// <summary>
    /// QC 도어 오프로더 도어와 연동 스킵 여부  사용:false, 미사용:true
    /// </summary>
    public bool bQcDoor;
    
    /// <summary>
    /// MTBA MTBF 하루 작업 시간(단위 시간)
    /// </summary>
    public double dDayWorking;

    /// <summary>
    /// MTBA MTBF 산정 주기
    /// 0 = 한달, 1 = 일주일, 2 = 하루
    /// </summary>
    public int iPeriod;

    /// <summary>
    /// MTBA MTBF 산정 갱신 시간(단위 시간)
    /// </summary>
    public int iRenewal;

    /// <summary>
    /// MTBA MTBF MTBA 기준 시간(단위 분)
    /// 에러 발생 후 조치 시간
    /// </summary>
    public int iSetTime;

    /// <summary>
    /// Table Manual Pos Setting
    /// </summary>
    //191203 ksg :
    public double LeftTopPos ;
    public double LeftBtmPos ;
    public double RightTopPos;
    public double RightBtmPos;
    public double LeftXPos   ;
    public double RightXPos  ;

    // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
    /// <summary>
    /// Table Tilt Limit Setting
    /// </summary>
    public double LeftLimitTtoB;
    public double LeftLimitLtoR;
    public double RightLimitTtoB;
    public double RightLimitLtoR;
    // 2021.10.08 SungTae End

    // 2021.04.05 SungTae Start
    public double[] aLTblMeasPos;
    public double[] aRTblMeasPos;
    // 2021.04.05 SungTae End


    /// <summary>
    /// MTBA MTBF MTBA 기준 시간(단위 분)
    /// Language 선택
    /// </summary>
    public int iSelLan;

    /// <summary>
    /// Probe Meansure Type 
    /// </summary>
    public bool bPbType;

    //20191010 ghk_manual_bcr
    /// <summary>
    /// 메뉴얼 그라인딩 시 Bcr 체크 기능
    /// </summary>
    public bool bManBcrSkip;

    //191125 ksg :
    /// <summary>
    /// SECS / GEM USE
    /// </summary>
    public bool bSecsUse;

    //191125 ksg :
    /// <summary>
    /// Current Remote Control 
    /// </summary>
    public bool bCurRemote;

    //200203 ksg :
    /// <summary>
    /// Onload RFID Skip 옵션
    /// </summary>
    public bool bOnlRfidSkip;

    //200203 ksg :
    /// <summary>
    /// Offload RFID Skip 옵션
    /// </summary>
    public bool bOflRfidSkip;

    // 200318 mjy : 항목 추가
    /// <summary>
    /// Unit 모드에서 Dry top air-blow와 Bottom air-blow 사이의 딜레이 [ms]
    /// </summary>
    public int iDryDelay;
	
	 //200318 ksg : Wheel One Point Check
    /// <summary>
    /// Wheel One Point Check
    /// </summary>
    public bool bWhlOneCheck;
    // 200728 jym : 휠 클린 노즐 추가
    /// <summary>
    /// 휠 클린 노즐 Skip 여부
    /// </summary>
    public bool bWhlClnSkip;

    // syc : Warm up 유휴 시간
    /// <summary>
    /// 웜업 유휴시간 설정
    /// </summary>
    public int iWmUI;

    
    /// <summary>
    /// 자재가 있어도 워밍업 수행 (SCK+)
    /// </summary>
    public bool bWarmUpWithStrip;   // 2022.06.10 lhs

    //2022.06.14 lhs Start : Sponge water auto on/off
    /// <summary>
    /// Sponge water auto On/Off 사용 여부
    /// </summary>
    public bool bSpgWtAutoOnOff;
    /// <summary>
    /// Sponge water auto off time 시간 설정, 해당 시간 지나면 Water on
    /// </summary>
    public int nSpgWtOffMin;
    /// <summary>
    /// Sponge water auto on time 설정 시간, 해당 시간 만큼 Water off
    /// </summary>
    public int nSpgWtOnMin;
    //2022.06.14 lhs End : Sponge water auto on/off

    // 200820 jym : Network drive 설정 
    public bool bNetUse;
    public string sNetIP;
    public string sNetPath;
    public string sNetID;
    public string sNetPw;
    //210811 pjh : D/F Server Data Save 기능 Net Drive 추가
    public bool bDFNetUse;
    //

    //JSKim : Probe Value 표시 폰트 사이즈
    /// <summary>
    /// Probe Value 표시 폰트 사이즈
    /// </summary>
    public int iProbeFontSize;

    //JSKim : Probe Value 표시 소수점 자리 수
    /// <summary>
    /// Probe Value 표시 소수점 자리 수
    /// </summary>
    public int iProbeFloationPoint;
    // 200917 jym : Skyworks chiller skip 기능 추가
    /// <summary>
    /// DI Chiller 사용여부  true : 사용안함  false : 사용
    /// </summary>
    public bool bDiChillerSkip;
    // 201006 jym : 추가
    /// <summary>
    /// DI & PCW Check 사용 시 재검사 진행 딜레이 [ms]
    /// 설정 된 시간동안 계속 체크 진행 함
    /// </summary>
    public int iChkDiTime;

    // 2020.10.13 SungTae : Dresser Five Point Check
    /// <summary>
    /// Dresser Five Point Check - 0 : 1 Point, 1 : 5 Point
    /// </summary>
    public bool bDrsFiveCheck;

    // 2020.10.22 JSKim St
    /// <summary>
    /// Dual Pump 사용여부 true : 사용 false : 사용안함
    /// </summary>
    public bool bDualPumpUse;
    // 2020.10.22 JSKim Ed

    //201005 pjh : Log Delete Period
    /// <summary>
    /// Log 자동 삭제 주기
    /// </summary>
    public int iDelPeriod;

    // 2021.04.08 SungTae : Wheel History Delete Period 추가
    /// <summary>
    /// Log 자동 삭제 주기
    /// </summary>
    public int iDelPeriodHistory;

    // 201022 jym : 신규 추가
    /// <summary>
    /// Auto tool setter gap 변동 시 체크하는 리밋 값[mm]
    /// </summary>
    public double[] aAtoLimit;

    //20190309 ghk_level

    // 2020-12-14, jhLee : 기존 MeaStrip_T1, T2를 통합한 함수를 이용할것인지 여부
    public bool bMeasureFunctionType;

    // 2021.07.02 SungTae : 고객사(ASE-KR) 요청으로 Strip Cleaning Mode 추가
    /// <summary>
    /// Off-load Picker Strip Cleaning Mode (0 : Basic Mode, 1 : Rotation Mode)
    /// </summary>
    public int iOfpCleaningMode;


    // 2021.05.17 jhLee : 연속LOT (Multi-LOT) 기능 사용 여부, CDataOption.UseMultiLOT 이 활성화 된 상태에서 사용 가능
    /// <summary>
    /// 연속 LOT 기능 사용  false:사용안함, true:사용 함
    /// </summary>
    public bool bMultiLOTUse;

    /// <summary>
    /// 연속 LOT에서 LOT의 구분을 위한 연속된 빈 Slot의 수량 3 ~ 5
    /// </summary>
    public int nLOTEndEmptySlot;

    //210727 pjh : Wheel 및 Dresser의 높이가 비정상적으로 변했을 때 에러 발생을 위한 Limit값
    /// <summary>
    /// Wheel 및 Dresser 최고 높이 변수.
    /// Wheel 및 Dresser의 높이가 비정상적으로 변했을 때 에러 발생을 위한 Limit값
    /// </summary>
    public bool bVarLimitUse;

    public double[] aWheelMax;

    public double[] aDresserMax;
    //
    // 210804 pjh : Tool Setter Gap 변경 최대 값(Tool Setter Gap이 해당 설정값 이상으로 바뀌면 Tool Setter Gap 값 갱신하지 않음)
    public double dToolMax;
    //

    // 2021.09.14 Start : 이오나이저 Sol/V Off 설정 (SCK전용)
    public bool bUseIOZTSolOff;         // Off 기능 사용
    public int  nIOZTSolOffDelaySec;    // Delay Second
    public bool bIOZT_ManualClick;      // 사용자 수동 클릭 여부
    // 2021.09.14 End

    // 2022.01.26 lhs Start : Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해
    /// <summary>
    /// Dryer Z축 Up 후에 안정화 시간
    /// </summary>
    public int nDryZUpStableDelay;
    // 2022.01.26 lhs End

    // 2022.03.33 lhs Start : Dummy Thickness 설정 (2004U)
    /// <summary>
    /// Dummy Thickness
    /// </summary>
    public double dDummyThick;
    // 2022.03.33 lhs End



    #region Auto
    /// <summary>
    /// Auto - Manual
    /// </summary>
    public int iLvManual;

    /// <summary>
    /// Auto - Device
    /// 메인화면 우측 상단 Device Name 클릭 포함
    /// </summary>
    public int iLvDevice;

    /// <summary>
    /// Auto - Wheel
    /// </summary>
    public int iLvWheel;

    /// <summary>
    /// Auto - Spc
    /// </summary>
    public int iLvSpc;

    /// <summary>
    /// Auto - Option
    /// </summary>
    public int iLvOption;

    /// <summary>
    /// Auto - Util
    /// </summary>
    public int iLvUtil;

    /// <summary>
    /// Auto - Exit
    /// </summary>
    public int iLvExit;
    #endregion

    #region Manual
    /// <summary>
    /// Manual - 매뉴얼
    /// </summary>
    public int iLvWarmSet;

    /// <summary>
    /// Manual - 온로더
    /// </summary>
    public int iLvOnL;

    /// <summary>
    /// Manual - 인레일
    /// </summary>
    public int iLvInr;

    /// <summary>
    /// Manual - 온로더 피커
    /// </summary>
    public int iLvOnp;

    /// <summary>
    /// Manual - 그라인드 왼쪽
    /// </summary>
    public int iLvGrL;

    /// <summary>
    /// Manual - 그라인드 양쪽
    /// </summary>
    public int iLvGrd;

    /// <summary>
    /// Manual - 그라인드 오른쪽
    /// </summary>
    public int iLvGrr;

    /// <summary>
    /// Manual - 오프로더 피커
    /// </summary>
    public int iLvOfp;

    /// <summary>
    /// Manual - 드라이
    /// </summary>
    public int iLvDry;

    /// <summary>
    /// Manual - 오프로더
    /// </summary>
    public int iLvOfL;

    //20191204 ghk_level
    /// <summary>
    /// Manual - 올 서버 온 버튼 활성/비활성
    /// </summary>
    public int iLvAllSvOn;
    /// <summary>
    /// Manual - 올 서버 오프 버튼 활성/비활성
    /// </summary>
    public int iLvAllSvOff;
    //
    #endregion

    #region Device
    /// <summary>
    /// Device - 그룹 생성
    /// </summary>
    public int iLvGpNew;
    /// <summary>
    /// Device - 그룹 복사 붙이기
    /// </summary>
    public int iLvGpSaveAs;
    /// <summary>
    /// Device - 그룹 삭제
    /// </summary>
    public int iLvGpDel;
    /// <summary>
    /// Device - 디바이스 생성
    /// </summary>
    public int iLvDvNew;
    /// <summary>
    /// Device - 디바이스 복사 붙이기
    /// </summary>
    public int iLvDvSaveAs;
    /// <summary>
    /// Device - 디바이스 삭제
    /// </summary>
    public int iLvDvDel;
    /// <summary>
    /// Device - 디바이스 불러와서 적용하기
    /// </summary>
    public int iLvDvLoad;
    /// <summary>
    /// Device - 디바이스 현재 파라미터 보기
    /// </summary>
    public int iLvDvCurrent;
    /// <summary>
    /// Device - 디바이스 저장(수정)
    /// </summary>
    public int iLvDvSave;
    //20191203 ghk_level
    /// <summary>
    /// Device - 디바이스 파라미터 설정 기능
    /// </summary>
    public int iLvDvParaEnable;
    /// <summary>
    /// Device - 디바이스 Set Position 숨기기
    /// </summary>
    public int iLvDvPosView;
    #endregion

    #region Wheel
    /// <summary>
    /// Wheel - 휠 생성
    /// </summary>
    public int iLvWhlNew;
    /// <summary>
    /// Wheel - 휠 복사 붙이기
    /// </summary>
    public int iLvWhlSaveAs;
    /// <summary>
    /// Wheel - 휠 삭제
    /// </summary>
    public int iLvWhlDel;
    /// <summary>
    /// 휠 저장 / 적용
    /// </summary>
    public int iLvWhlSave;
    /// <summary>
    /// 휠 저장 / 적용
    /// </summary>
    public int iLvWhlChange;
    /// <summary>
    /// 휠 저장 / 적용
    /// </summary>
    public int iLvWhlDrsChange;
    #endregion

    #region Spc
    /// <summary>
    /// Spc - 그래프 저장
    /// </summary>
    public int iLvSpcGpSave;
    /// <summary>
    /// Spc - 에러 리스트 창
    /// </summary>
    public int iLvSpcErrList;
    /// <summary>
    /// Spc - 에러 히스토리 창
    /// </summary>
    public int iLvSpcErrHis;
    /// <summary>
    /// Spc - 에러 히스토리 뷰 버튼
    /// </summary>
    public int iLvSpcErrHisView;
    /// <summary>
    /// Spc - 에러 히스토리 저장
    /// </summary>
    public int iLvSpcErrHisSave;
    #endregion

    #region Option
    /// <summary>
    /// Option - 시스템 포지션
    /// </summary>
    public int iLvOptSysPos;
    /// <summary>
    /// Option - 테이블 그라인딩
    /// </summary>
    public int iLvOptTbGrd;

    // 2020.10.09 SungTae Start : Add only Qorvo
    /// <summary>
    /// Option - On/Off Loader
    /// </summary>
    public int iLvOptLoader;
    /// <summary>
    /// Option - Inrail / Dry
    /// </summary>
    public int iLvOptRailDry;
    /// <summary>
    /// Option - On/Off Picker
    /// </summary>
    public int iLvOptPicker;
    /// <summary>
    /// Option - Left/Right Grind
    /// </summary>
    public int iLvOptGrind;
    // 2020.10.09 SungTae End
    
    // 2021.07.15 lhs Start
    /// <summary>
    /// Option - General
    /// </summary>
    public int iLvOptGen;
    /// <summary>
    /// Option - Maintenance
    /// </summary>
    public int iLvOptMnt;
    // 2021.07.15 lhs End

    #endregion

    #region Util
    /// <summary>
    /// Util - 모션
    /// </summary>
    public int iLvUtilMot;
    /// <summary>
    /// Util - 스핀들
    /// </summary>
    public int iLvUtilSpd;
    /// <summary>
    /// Util - 인풋
    /// </summary>
    public int iLvUtilIn;
    public int iLvUtilOut;
    /// <summary>
    /// Util - 아웃풋
    /// </summary>
    public int iLvUtilPrb;
    /// <summary>
    /// Util - 타워램프
    /// </summary>
    public int iLvUtilTw;
    /// <summary>
    /// Util - 바코드
    /// </summary>
    public int iLvUtilBcr;
    /// <summary>
    /// Util - 반복 측정
    /// </summary>
    public int iLvUtilRepeat;
    #endregion

    #region OpManual
    //20191204 ghk_level
    public int iLvOpDrsPos;

    // 2020-10-21, jhLee : Skyworks VOC, 작업자 레벨에 따라 Magazine의 Strip 존재여부 편집 기능을 사용 권한을 지정한다.
    /// <summary>
    /// OPManual - 서브화면의 Magazine의 Strip 존재여부 편집 권한 지정
    /// </summary>
    public int iLvOpStripExistEdit;

    #endregion
    //
    /// <summary>
    /// 테이블 클리닝 카운트 0:왼쪽 1:오른쪽
    /// </summary>
    public int[] aTC_Cnt;

    /// <summary>
    ///  테이블 크리닝 주기, 0~1 : 매회 수행, 2~10 : 지정 수량 생산 후 테이블 크리닝 수행      // 2020-11-16, jhLee, Skyworks VOC
    /// </summary>
    public int iTC_Cycle;

    // 200330 mjy : Option에 Pick/Place 관련 파라메터 추가
    /// <summary>
    /// Pick 작업 시 워터 클린 높이 [mm]
    /// </summary>
    public double dPickGap;
    /// <summary>
    /// Pick 작업 시 워터 클린 대기 시간 [ms]
    /// </summary>
    public int iPickDelay;
    /// <summary>
    /// Place 작업 시 워터 클린 높이 [mm]
    /// </summary>
    public double dPlaceGap;
    /// <summary>
    /// Place 작업 시 워터 클린 대기 시간 [ms]
    /// </summary>
    public int iPlaceDelay;
    /// <summary>
    /// LOT Open 후 시작 시 테이블 클리닝 사용 여부
    /// true:사용  false:미사용
    /// </summary>
    public bool bLotTblClean;
    /// <summary>
    /// LOT End 후 드레싱 진행 사용 여부
    /// true:사용  false:미사용
    /// </summary>
    public bool bLotDrs;
	//20200513 jym : 
    /// <summary>
    /// 매거진 배출 위치 설정
    /// Top/Bottom
    /// </summary>
    public EMgzWay eEmitMgz;
	/// <summary>
    /// QC Vision IP
    /// default 10.0.0.1
    /// </summary>
    public string sQcServerIp;
    /// <summary>
    /// QC Vision PORT
    /// default 5500
    /// </summary>
    public string sQcServerPort;

    //2020.10.19 jhLee : (SPIL CS)  RCMD를통해 START 명령으로 설비 구동시 START요청 후 얼마나 기다릴 것인가 ?
    /// <summary>
    /// 설비 구동 요청 후 Host로부터 얼마동안 RCMD START를 기다릴 것인가 ?, 시간초과시 Idle 상태로 복귀
    /// 0:묻지않고 곧바로 설비를 AUTO로 기동(기본값),   1 ~ : 대기시간 [ms]
    /// </summary>
    public int iRCMDStartTimeout;

    //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
    /// <summary>
    /// 현재 그라인딩 스텝 진행 => one-point 측정 => Over Grinding 발생 시 다음 스텝의 그라인딩 카운트 조정 기능 사용 여부 
    /// </summary>
    public bool bOverGrdCountCorrectionUse;

    // 2020-11-17, jhLee : Skyworks 일정매수 투입 후 자동으로 Loading stop 되기위한 투입 수량
    public int iAutoLoadingStopCount;

    //syc : ase kr loading stop 
    /// <summary>
    /// 0 : inrail(nomal) 
    /// 1 : table
    /// </summary>
    public int iLoadingStopType;

    // 2021.02.20 lhs Start
    /// <summary>
    /// JCET VOC 전용 
    /// Wheel이 멈출때까지 기다림 Skip (Wheel이 멈출때까지 기다리지 않고 Probe로 측정하기 위해)
    /// true = 기다리지 않음, false = 기다림
    /// </summary>
    public bool bWheelStopWaitSkip;
    // 2021.02.20 lhs End

    // 2020-10-26, jhLee
    /// <summary>
    /// Probe 측정의 정확성을 높이기 위하여 기존 1mm 에서 더 눌러주는 양 0 ~ 4 mm 사이
    /// </summary>
    public double dProbeOD;                 // Probe Overdrive

    /// <summary>
    /// Probe 측정의 정확성을 높이기 위하여 측정면에 접촉 한 뒤 지연되는 시간 ms
    /// </summary>
    public int iProbeStableDelay;        // Probe down 뒤 안정화 시간 ms

    // 2020-11-03, jhLee : 속도 단축을 위해 사용되는 위치/속도 값 정의
    /// <summary>
    /// Probe 측정의 속도를 높이기 위하여 낮은 위치이지만 안전을 위해 설정하는 Table Top에서의 Offset,  최소 양 5 ~ 20 mm 사이
    /// </summary>
    public double dSafetyTopOffset;     // Probe를 Down상태로 안전하게 이동할 수 있는 Table 상단에서의 Offset

    /// <summary>
    /// Probe 측정의 속도를 높이기 위하여 측정 후 Z축 고속 Up 이동을 위한 속도, Normal 속도보다 높은값을 권장
    /// </summary>
    public double dZAxisMoveUpSpeed;     // 측정 후 Z축을 Up 이동시 이동속도

}

public struct tMainLog
{
    /// <summary>
    /// 로그의 시퀀스
    /// </summary>
    public eLog ESeq;
    /// <summary>
    /// 로그 종류
    /// </summary>
    public eLog eType;
    /// <summary>
    /// 로그 발생 시간
    /// </summary>
    public DateTime dtTime;
    /// <summary>
    /// 로그 메세지
    /// </summary>
    public string sMsg;    
}

/// <summary>
/// RS-232 정보 
/// </summary>
public struct t232C
{
    /// <summary>
    /// COM 포트 이름
    /// </summary>
    public string sPort;
    /// <summary>
    /// Baud rate 속도
    /// </summary>
    public int iBaud;
    /// <summary>
    /// Data bits
    /// </summary>
    public int iDtBits;
    /// <summary>
    /// Parity
    /// </summary>
    public Parity eParity;
    /// <summary>
    /// Stop bits
    /// </summary>
    public StopBits eStBits;
    /// <summary>
    /// Use RTS
    /// </summary>
    public bool bRts;
    /// <summary>
    /// 
    /// </summary>
    public double dX;
    public double dY;
}

public struct tTowerInfo
{
    public ELamp Red;
    public ELamp Yel;
    public ELamp Grn;
    public EBuzz Buzz;
}
/// <summary>
/// 각 파트별 데이터 구조체
/// </summary>
public struct TPart
{
    /// <summary>
    /// Sequence State
    /// 시퀀스 상태
    /// </summary>
    public ESeq iStat;
    /// <summary>
    /// LOT Name
    /// </summary>
    public string sLotName;
    /// <summary>
    /// Magazine Number
    /// </summary>
    public int iMGZ_No;
    /// <summary>
    /// Magazine ID(RFID)
    /// </summary>
    public string sMGZ_ID;
    /// <summary>
    /// Slot Number
    /// </summary>
    public int iSlot_No;
    /// <summary>
    /// 랏 오픈 시 초기화 되어야 함
    /// </summary>
    public int[] iSlot_info;
    /// <summary>
    /// 바코드 입력 값 (U:캐리어 바코드 값)
    /// </summary>
    public string sBcr;

    //20190604 ghk_onpbcr
    /// <summary>
    /// 바코드 검사 유무
    /// </summary>
    public bool bBcr;
    /// <summary>
    /// 오리엔테이션 검사 유무
    /// </summary>
    public bool bOri;

    /// <summary>
    /// Each Module Strip Check 
    /// 각 모듈별 자재 유무 확인 변수
    /// </summary>
    public bool bExistStrip;

    //20191120 ghk_display_strip
    /// <summary>
    /// 센서와 시퀀스상 자재 유무 확인 변수
    /// true : 다름, false : 같음
    /// true 일 경우 자재 상태 점멸 표시 및 에러.
    /// false 일 경우 자재 유무에 따라 자재 표시
    /// </summary>
    public bool bNotMatch;

    public double[] dPcb  ;
    public double dPcbMax ;
    public double dPcbMean;
    public double dPcbMin ;
    public double dShiftT ; //190529 ksg :

    //20190618 ghk_dfserver
    public double dDfMin;
    public double dDfMax;
    public double dDfAvg;
    public string sBcrUse;

    // 2021.07.30 lhs Start : SECS/GEM에서 받은 Top Mold, Btm Mold 저장용 (UseNewSckGrdProc 적용시)
    public double dTopMoldMax; // 필요할까?
    public double dTopMoldAvg;
    public double dBtmMoldMax;
    public double dBtmMoldAvg;
    // 2021.07.30 lhs End

    public int iTPort;          //20190624 josh    table port 1 : left, 2 : right

    // 200314 mjy : 유닛 데이터 배열 추가
    /// <summary>
    /// 유닛 유무 배열
    /// </summary>
    public bool[] aUnitEx;

    //200624 pjh : Grinding 중 Error Check 변수
    /// <summary>
    /// 자재가 Grinding 중 Error가 발생했는지 Check하는 bool변수
    /// </summary>
    public bool bChkGrd;
	
    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    /// <summary>
    /// 별도 측정 포인트가 지정된 스트립인지 여부 (Lot Open 후 첫 n장)
    /// Before Measure 시작 기준 → On-Picker까지에서는 이 값이 false, L-Table/R-Table 선착하여 Before Measure 시작되는 첫 n장까지 true로 설정됨
    /// Step Mode에서 L-Table에서 첫 n장으로 설정되면 R-Table에서도 첫 n장으로 설정되어야 함
    /// (즉, Step Mode에서 L-Table에서 설정된 bLeaderStrip 값은 L-Table -> Off-Picker -> R-Table로 그 데이터가 shift되어야 함)
    /// </summary>
    public bool b18PMeasure;
    //

    // 2020.12.12 JSKim St
    /// <summary>
    /// AutoRun 진행 간 마지막 iStat를 보관
    /// </summary>
    public int iStripStat;
    // 2020.12.12 JSKim Ed

    // 20201217 myk : Table Vacuum 값 로그 저장
    /// <summary>
    /// Table Vacuum 값 로그 저장 변수
    /// </summary>
    public double dChuck_Vacuum;

    // 2021-05-10, jhLee : Multi-LOT의 표시 
    // 미사용 public bool bIsLotEnd;      // 지정 LOT의 마지막 Strip인가 ?
    public int nLotType;        // Type지정 (색상)
    public bool bIsUpdate;      // Strip의 위치가 변경되었는가 ? 사용되는 LOT Info를 갱신하기 위한 Flag
    public Color LotColor;              // LOT의 색 지정
    public int nLoadMgzSN;       // Load된 Magazine의 Serial 번호
	
    // 2021.05.31. SungTae Start : [추가]
    public string sDeviceName;
    public string sStage;
    public string sDualMode;
    public string sGrdMode;
    public string sSaveDate;

    public double dBfMin;
    public double dBfMax;
    public double dBfAvg;

    public double dAfMin;
    public double dAfMax;
    public double dAfAvg;

    public double dAfTotMin;
    public double dAfTotMax;
    public double dAfTotAvg;

    // 2021.05.31. SungTae End

    // 2022.01.21 lhs Start : 2004U용, Dummy가 포함된 Carrier 여부
    public bool bCarrierWithDummy;
    // 2022.01.21 lhs End

}

public struct TGrdData
{
    /// <summary>
    /// 현재 진행 중인 스텝의 인덱스
    /// </summary>
    public int iStep;
    /// <summary>
    /// 리그라인딩 인지 확인
    /// </summary>
    public bool bReGrd;
    /// <summary>
    /// 각 스텝의 그라인딩 횟수 중 현재 횟수
    /// </summary>
    public int[] aInx;
    /// <summary>
    /// 각 스텝의 그라인딩 횟수
    /// </summary>
    public int[] aCnt;
    /// <summary>
    /// 각 스텝의 타겟 높이 [mm]
    /// </summary>
    public double[] aTar;
    /// <summary>
    /// 각 스텝의 리그라인딩이 진행 된 횟수
    /// </summary>
    public int[] aReNum;
    /// <summary>
    /// 각 스텝의 현재 리그라인딩 횟수 중 현재 횟수
    /// </summary>
    public int[] aReInx;
    /// <summary>
    /// 각 스텝의 현재 리그라인딩 횟수
    /// </summary>
    public int[] aReCnt;
    /// <summary>
    /// 각 스텝의 1포인트 측정 값 [mm]
    /// </summary>
    public double[] a1Pt;
    /// <summary>
    /// 스트립 측정 이전 데이터
    /// </summary>
    public double[,] aMeaBf;
    /// <summary>
    /// 스트립 측정 이후 데이터
    /// </summary>
    public double[,] aMeaAf;
    /// <summary>
    /// 이전 원포인트 이전 값
    /// </summary>
    public double[] aOldOnPont; //191018 ksg :

    // 200314-mjy : 유닛 데이터 배열 추가
    /// <summary>
    /// 유닛 측정 값 배열
    /// </summary>
    public TUnit[] aUnit;
    /// <summary>
    /// 유닛 유무 배열
    /// </summary>
    public bool[] aUnitEx;
    /// <summary>
    /// 스핀들 부하량 검사 시작 플레그
    /// </summary>
    public bool bSplLoadFlag;
    /// <summary>
    /// 스핀들 최대 부하량 [%]
    /// </summary>
    public double dSplMaxLoad;

    //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
    public bool bCheckGrindCondition;   //고급 설정 범위 체크 기능 동작/중지 플래그 (그라인딩 시)
    public bool bCheckDressCondition;   //고급 설정 범위 체크 기능 동작/중지 플래그 (드레싱 시)
    public double dTableVacuumMin;      //Grinding 중 Table Vacuum 최저값 (그라인딩 시 체크, 그라인딩 중 이 값이 변동될 경우 두께 불균일 가능성 있음)
    public int nSpindleCurrentMin;      //Grinding 중 Spindle 전류 최소값 (그라인딩 또는 드레싱 시 체크, 향 후 사용 가능성 고려한 변수)
    public int nSpindleCurrentMax;      //Grinding 중 Spindle 전류 최대값 (그라인딩 또는 드레싱 시 체크)
    //..

    // 2022.08.30 lhs Start : Spindle Current(전류)가 설정범위 초과시 알람 표시 (SCK+ VOC)
    /// <summary>
    /// 스핀들 최대 전류 플레그 : 1번만 표시하기 위한 플래그
    /// </summary>
    public bool bSplMaxCurrFlag;
    /// <summary>
    /// 스핀들 최소 전류 플레그 : 1번만 표시하기 위한 플래그
    /// </summary>
    public bool bSplMinCurrFlag;
    /// <summary>
    /// 스핀들 최대 전류 [mA]
    /// </summary>
    public int nSplMaxCurr;
    /// <summary>
    /// 스핀들 최소 전류 [mA]
    /// </summary>
    public int nSplMinCurr;
    // 2022.08.30 lhs End

}

public struct TDrsData
{
    /// <summary>
    /// 현재 진행 중인 스텝의 인덱스
    /// </summary>
    public int iStep;
    /// <summary>
    /// 각 스텝의 그라인딩 횟수 중 현재 횟수
    /// </summary>
    public int[] aInx;
    /// <summary>
    /// 각 스텝의 그라인딩 횟수
    /// </summary>
    public int[] aCnt;

    public tStep[] aParm;

    // 201006 jym : 여기로 변수 옮김 (기존 구조체 삭제)
    /// <summary>
    /// 드레싱 실행 여부  true:드레싱 할 시점  false:안함
    /// </summary>
    public bool bDrs;
    /// <summary>
    /// 드레싱 파라메터 판별  true:New parameter  false:Using parameter
    /// </summary>
    public bool bDrsR;

    //210811 pjh : Wheel Jig 감지 시 Wheel Tip Max Limit 체크하지 않음
    public bool bWheelChange;
    //

    // 2021-10-25, jhLee, For SkyWorks Dressing Sync VOC
    public bool bDrsPerformed;      // Dressing 작업을 수행했는가 ? Auto-Run 진행시 check한다.
}

// 200314 mjy : 신규 생성
/// <summary>
/// 패키지가 Unit 모드 일때 사용하는 각 유닛별 구조체
/// </summary>
public struct TUnit
{
    /// <summary>
    /// 측정 이전 데이터
    /// </summary>
    public double[,] aMeaBf;
    /// <summary>
    /// 측정 이후 데이터
    /// </summary>
    public double[,] aMeaAf;

    public bool[,] aErrBf;
    public bool[,] aErrAf;
}

public struct TMea
{
    public double dVal;
    public bool bErr;
}

/// <summary>
/// SPC LotInfo 오토러닝 Lot End 후 데이터 저장용 구조체
/// </summary>
public struct TSpcInfo
{
	/// <summary>
	/// lot 이름
	/// </summary>
	public string sLotName;
    /// <summary>
    /// 그라인딩 모드 "STEP" = 스텝 모드, "NOMAL" = 노멀 모드
    /// </summary>
    public string sGrdMode;
    /// <summary>
    /// Device 이름
    /// </summary>
    public string sDevice;
    /// <summary>
    /// Target 값
    /// </summary>
    public string sTarget;
    /// <summary>
    /// lot oppen 후 시작 시간
    /// </summary>
    public string sStartTime;
    /// <summary>
    /// lot end 시간
    /// </summary>
    public string sEndTime;
    /// <summary>
    /// 에러, 정지 상태 아닌 장비 구동 시간
    /// </summary>
    public string sRunTime;
    /// <summary>
    /// 장비 유휴 누적 시간
    /// </summary>
    public string sIdleTime;
    /// <summary>
    /// 에러 발생 누적 시간
    /// </summary>
    public string sJamTime;
    /// <summary>
    /// Lot Open 후 시작 부터 lot End 까지 시간
    /// </summary>
    public string sTotalTime;
    /// <summary>
    /// UPH
    /// </summary>
    public string sUph;
    /// <summary>
    /// lot open 부터 lot end 까지 자재 그라인딩 장수
    /// </summary>
    public int iWorkStrip;
    /// <summary>
    /// 왼쪽 휠 시리얼 넘버
    /// </summary>
    public string sWhlSerial_L;
    /// <summary>
    /// 오른쪽 휠 시리얼 넘버
    /// </summary>
    public string sWhlSerial_R;
    // 2020.12.10 JSKim St
    /// <summary>
    /// Lot Open 후 시작 부터 lot End 까지 발생 Error Count
    /// </summary>
    public int iErrCnt;
    // 2020.12.10 JSKim Ed

}

/// <summary>
/// SPC LotData 오토러닝 Lot End 후 데이터 저장용 구조체
/// </summary>
public struct TSpcData
{
    /// <summary>
    /// lot 이름
    /// </summary>
    public string sLotName;
    /// <summary>
    /// 메거진 번호
    /// </summary>
    public string sMgzNo;
    /// <summary>
    /// 슬롯번호
    /// </summary>
    public string sSlotNo;
    /// <summary>
    /// 바코드 번호
    /// </summary>
    public string sBcr;
    /// <summary>
    /// 테이블 L or R
    /// </summary>
    public string sTable;
    /// <summary>
    /// 측정 값 중 Max
    /// </summary>
    public string sMax;
    /// <summary>
    /// 측정 값 중 Min
    /// </summary>
    public string sMin;
    /// <summary>
    /// 측정 값 평균
    /// </summary>
    public string sMean;
    /// <summary>
    /// 측정 값 Max - Min
    /// </summary>
    public string sTtv;
    /// <summary>
    /// 그라인딩 타켓
    /// </summary>
    public string sTarget;
    /// <summary>
    /// 그라인딩 타켓 오차 리미트 범위
    /// </summary>
    public string sTargetLim;
    /// <summary>
    /// 그라인딩 ttv 오차 리미트 범위
    /// </summary>
    public string sTtvLim;
    /// <summary>
    /// Grd Before Max
    /// </summary>
    public string sBMax;
    /// <summary>
    /// Grd Before Min
    /// </summary>
    public string sBMin;
    /// <summary>
    /// Grd Before Avg
    /// </summary>
    public string sBAvg;
    /// <summary>
    /// Grd Mode
    /// </summary>
    public string sMode; //191128 ksg :
    //
}
//20190216 ghk_probeTest
/// <summary>
/// 프로브 반복 테스트
/// </summary>
public struct tProbeTest
{
    /// <summary>
    /// 베이스 측정 값
    /// </summary>
    public double[] dBase;
    /// <summary>
    /// 센터 측정 값
    /// </summary>
    public double[] dCenter;
    /// <summary>
    /// 게이지 블럭 높이
    /// 센터 - 베이스 값
    /// </summary>
    public double[] dBlock;
}

//20190218 ghk_dynamicfunction
/// <summary>
/// 다이나믹 펑션
/// </summary>
public struct tDynamic
{
    /// <summary>
    /// pcb 측정 값
    /// </summary>
    public double[] dPcb;
    /// <summary>
    /// pcb 측정 값 Max
    /// </summary>
    public double dPcbMax;
    /// <summary>
    /// pcb 측정 값 Mean
    /// </summary>
    public double dPcbMean;
    /// <summary>
    /// pcb 측정 값 Ttv
    /// </summary>
    public double dPcbTtv;
    /// <summary>
    /// pcb 측정 값 에러 범위
    /// </summary>
    public double dPcbRange;
    /// <summary>
    /// pcb 측정 값 Mean 에러 범위
    /// </summary>
    public double dPcbMeanRange;
    /// <summary>
    /// 그라인딩 시 적용 될 PCB 측정 값
    /// 0 : MAX, 1 : MEAN
    /// </summary>
    public int iHeightType;
    /// <summary>
    /// 인레일 pcb base 측정 값
    /// </summary>
    public double dPcbBase;
    /// <summary>
    /// 인레일 pcb base 측정 값
    /// </summary>
    public double dPcbRnr; //190511 ksg
    /// <summary>
    /// 인레일 Y축 Ailig 갭 측정시 Y축을 좁혀서 측정값을 더 정확히 함
    /// </summary>
    public double dYAlign; //190511 ksg
    //20200402 jhc : 
    /// <summary>
    /// DF InRail Base 측정 값 허용 범위(+/-)
    /// DF Probe 셋업 시 DF Probe를 InRail Base로 DOWN했을 때 Probe 값을 0으로 초기화해두어야 함
    /// </summary>
    public double dBaseRange;
}

public struct tErr
{
    /// <summary>
    /// 에러번호
    /// </summary>
    public string sNo;
    /// <summary>
    /// 에러이름
    /// </summary>
    public string sName;
    /// <summary>
    /// 에러설명
    /// </summary>
    public string sAction;
    
    // 2021.09.06 SungTae Start : [추가] (ASE-KR VOC) Error명 한글 표시 요청
    /// <summary>
    /// 에러이름
    /// </summary>
    public string sKorName;
    // 2021.09.06 SungTae End

    // 2020.12.07 JSKim St
    /// <summary>
    /// Max 2020103 : SCK+ Rader & Error Text Modify
    /// Rader 에러 Count & Flag : 0 이면 Rader Counter Mode Off ,  0 보다 크면 sRaderCountFlag 발생 횟수 만큼 sRaderCounter를 증가, 설정수량 이상 발생시 Rader Count Over Error 발생
    /// </summary>
    //public int iRaderUseCnt;
    //public int iRaderCounter;
    //public int iJamCount;       // 동작 하지 않아 삭제 - Concept Image에 없음 왜 만들었는지 모름
    //public string sSPCFlag;     // 실제 쓰지 않아 삭제 - 이력을 모르니 왜 만들었는지 모름
    //public string sParts;       // 실제 쓰지 않아 삭제 - 이력을 모르니 왜 만들었는지 모름
    /// <summary>
    /// Radar 감지 사용 여부
    /// </summary>
    public bool bRadarUse;
    /// <summary>
    /// Radar 감지 설정 Count
    /// </summary>
    public int iRadarOptionCnt;
    /// <summary>
    /// Radar 감지 발생 Count
    /// </summary>
    public int iRadarErrorCnt;
    // 2020.12.07 JSKim Ed
}

//190319 ksg :
public struct tPbResult
{
    /// <summary>
    /// Probe Before 측정 값
    /// </summary>
    public double dBMax;
    /// <summary>
    /// Probe Before 측정 값
    /// </summary>
    public double dBMin;
    /// <summary>
    /// Probe Before 측정 값
    /// </summary>
    public double dBAvg;
    /// <summary>
    /// Probe Before 측정 값
    /// </summary>
    public double dBMean;
    /// <summary>
    /// Probe After 측정 값
    /// </summary>
    public double dAMax;
    /// <summary>
    /// Probe After 측정 값
    /// </summary>
    public double dAMin;
    /// <summary>
    /// Probe After 측정 값
    /// </summary>
    public double dAAvg;
    /// <summary>
    /// Probe After 측정 값
    /// </summary>
    public double dAMean;
}

//20190614 ghk_dfserver
public struct TDfInfo
{
    public string sIp;    //서버 Ip
    public int iPort;    //서버 Port
    public bool bBusy;    //서버 상태
    public string sId;    //장비 ID
    public string sLotName;
    public string sGl;    //Lot 정보 GL1 or GL2 or GL3
    public string sBcr;    //Inrail GRL or GL3 일때 데이터 요청시 바코드 ID
    public double dBcrMin;    //Inrail GL2 or GL3일때 서버에서 읽어온 데이터 Min
    public double dBcrMax;    //Inrail GL2 or GL3일때 서버에서 읽어온 데이터 Max
    public double dBcrAvg;    //Inrail GL2 or GL3일때 서버에서 읽어온 데이터 Avg
    public string sBcrUse;    //Inrail GL2 or GL3일때 서버에서 읽어온 데이터 사용 타입(Min or Max or Avg)
}

public struct TGrdDfData
{
    public string sBcr;    //Grd 데이터 전송시 바코드 ID
    public double dPcb1;    //PCB 1번 포인트 값
    public double dPcb2;    //PCB 2번 포인트 값
    public double dPcb3;    //PCB 3번 포인트 값
    public string sPcbUse;    //그라인딩시 사용한 PCB 높이 타입(Min or Max or Avg)
    public string sWay;    //왼쪽 오른쪽 LGD or RGD
    public string sRow;    //데이터 Row표시 (R01 or R02 or R03......)
    public bool   bAck;
}
//

#region Wheel & Dresser Change part
#region Wheel part
//190522 ksg :
public struct tWhlChgSeq
{
    /// <summary>
    /// Wheel Change start
    /// </summary>
    public int iStep     ;
    public bool bBtnHide ;
//1 Step
    /// <summary>
    /// Wheel Change start
    /// </summary>
    public bool bStart    ;
    public bool bStartShow;
    /// <summary>
    /// Wheel Change Select Left
    /// </summary>
    public bool bSltLeft;
    /// <summary>
    /// Wheel Change Select Right
    /// </summary>
    public bool bSltRight;
//2 Step
    /// <summary>
    /// Select Wheel File
    /// </summary>
    public bool bSltWhlFile;
    public bool bWhlSelShow;
//3 Step
    /// <summary>
    /// Select Wheel File Finish
    /// </summary>
    public bool bWhlSelFShow;
//4 Step
    /// <summary>
    /// Left Wheel Change 
    /// </summary>
    public bool bLWhlChangeShow;
    /// <summary>
    /// Left Wheel Jig check 
    /// </summary>
    public bool bLWhlJigCheck  ;
    /// <summary>
    /// Right Wheel Change 
    /// </summary>
    public bool bRWhlChangeShow;
    /// <summary>
    /// Right Wheel Jig Check 
    /// </summary>
    public bool bRWhlJigCheck  ;
//5 Step
    /// <summary>
    /// Left Wheel Meansure
    /// </summary>
    public bool bWhlMeanS;
    public bool bWhlMeanF;
    public bool bWhlMeanShow;
//6 Step
    /// <summary>
    /// Left Wheel Dressing
    /// </summary>
    public bool bWhlDressS;
    public bool bWhlDressF;
    public bool bWhlDressShow;

//7 Step
    /// <summary>
    /// Left Wheel change Complet
    /// </summary>
    public bool bWhlCompShow;

//8 Step
    /// <summary>
    /// Wheel Limit Alram
    /// </summary>
    public bool bWhlLimit;
}
#endregion

//190620 ksg :
#region Dresser Part
public struct tDrsChgSeq
{
    /// <summary>
    /// Dresser Change start
    /// </summary>
    public int iStep     ;
//1 Step
    /// <summary>
    /// Dresser Change start
    /// </summary>
    public bool bStart    ;
    public bool bStartShow;
    /// <summary>
    /// Dresser Change Select Left
    /// </summary>
    public bool bSltLeft;
    /// <summary>
    /// Dresser Change Select Right
    /// </summary>
    public bool bSltRight;
    /// <summary>
    /// Dresser Change Select Right
    /// </summary>
    public bool bSltDual;
//2 Step
    /// <summary>
    /// Left Dresser Change 
    /// </summary>
    public bool bLDrsChangeShow;
    /// <summary>
    /// Right Dresser Change 
    /// </summary>
    public bool bRDrsChangeShow;
    /// <summary>
    /// Right Dresser Change 
    /// </summary>
    public bool bDDrsChangeShow;
//3 Step
    /// <summary>
    /// Left Dresser Meansure
    /// </summary>
    public bool bDrsMeanS;
    public bool bDrsMeanF;
    public bool bDrsMeanDS;
    public bool bDrsMeanDF;
    public bool bDrsMeanShow;
//4 Step
    /// <summary>
    /// Left Dresser change Complet
    /// </summary>
    public bool bDrsCompShow;
}
#endregion
#endregion

//20191111 ghk_regrindinglog
public struct ReGrdCntLog
{
    public List<int> m_lRedCnt;    
}

//191120 ksg :
public struct PrboeValue
{
    public List<double> m_BFVal;
    public List<double> m_AFVal;
}

/// <summary>
/// Grd Log
/// </summary>
public struct tLog //190218 ksg :
{
    public string sStatus;
    public int    iStep  ;
    public string sAsix1 ;
    public double dPos1  ;
    public string sAsix2 ;
    public double dPos2  ;
    public string sAsix3 ;
    public double dPos3  ;
    public string sMsg   ;
}
/// <summary>
/// 임시변수 구조체
/// </summary>
public struct tTempData
{
    public eX eIn1;
    public eX eIn2;
    public eX eIn3;
    public eY eOt1;
    public eY eOt2;
    public eY eOt3;
    public double dPosX;
    public double dPosY;
    public double dPosZ;
}

/// <summary>
/// 200326 mjy : 경과시간 구조체
/// </summary>
public struct TElapse
{
    /// <summary>
    /// 시작 시간
    /// </summary>
    public DateTime dtStr;
    /// <summary>
    /// 종료 시간
    /// </summary>
    public DateTime dtEnd;
    /// <summary>
    /// 경과 시간
    /// </summary>
    public TimeSpan tsEls;
}

/// <summary>
/// Message view wheel
/// </summary>
public struct TMsg
{
    /// <summary>
    /// 화면에 표시 여부
    /// </summary>
    public bool bView;
    /// <summary>
    /// 확인 버튼 선택 여부
    /// </summary>
    public bool bOk;
    /// <summary>
    /// 취소 버튼 선택 여부
    /// </summary>
    public bool bCan;
    /// <summary>
    /// 버튼 선택 여부
    /// </summary>
    public bool bSelc;
    /// <summary>
    /// 표시되는 타이틀 이름
    /// </summary>
    public string sTitle;
    /// <summary>
    /// 표시되는 내용
    /// </summary>
    public string sNote;
    /// <summary>
    /// 옵션 1
    /// </summary>
    public string sOpt1;
    /// <summary>
    /// 옵션 2
    /// </summary>
    public string sOpt2;
    /// <summary>
    /// 메세지 종류
    /// </summary>
    public eMsg eType;
}

// 210601 jym : 네트워크 스레드 추가
/// <summary>
/// 네트워크 드라이브 저장 큐 구조체
/// </summary>
public struct TNetDrive
{   
    /// <summary>
    /// 네트워크 드라이브 저장 경로
    /// </summary>
    public string path;
    /// <summary>
    /// 저장 데이터
    /// </summary>
    public string data;

    public TNetDrive(string _path, string _data)
    {
        path = _path;
        data = _data;
    }
}


// 2021.07.14 lhs Start : PM Count , SCK VOC
/// <summary>
/// PM Count 구조체  (Life Time) 
/// </summary>
public struct TPM
{
	//public string strResetPwd; // config의 Pwd 사용

	/// <summary>
	/// Left Table Sponge Check Set Count
	/// </summary>
    public int nLT_Sponge_Check_SetCnt;
    /// <summary>
	/// Left Table Sponge Check Current Count
	/// </summary>
	public int nLT_Sponge_Check_CurrCnt;
    /// <summary>
	/// Left Table Sponge Check 사용여부
	/// </summary>
	public bool bLT_Sponge_Check_Use;

    /// <summary>
	/// Right Table Sponge Check Set Count
	/// </summary>
	public int nRT_Sponge_Check_SetCnt;
    /// <summary>
	/// Right Table Sponge Check Current Count
	/// </summary>
	public int nRT_Sponge_Check_CurrCnt;
    /// <summary>
	/// Right Table Sponge Check 사용여부
	/// </summary>
	public bool bRT_Sponge_Check_Use;

    /// <summary>
	/// OffPicker Sponge Check Set Count
	/// </summary>
	public int nOffP_Sponge_Check_SetCnt;
    /// <summary>
	/// OffPicker Sponge Check Current Count
	/// </summary>
	public int nOffP_Sponge_Check_CurrCnt;
    /// <summary>
	/// OffPicker Sponge Check 사용여부
	/// </summary>
	public bool bOffP_Sponge_Check_Use;

    /// <summary>
	/// Left Table Sponge Change Set Count
	/// </summary>
    public int nLT_Sponge_Change_SetCnt;
    /// <summary>
	/// Left Table Sponge Change Current Count
	/// </summary>
	public int nLT_Sponge_Change_CurrCnt;
    /// <summary>
	/// Left Table Sponge Change 사용여부
	/// </summary>
	public bool bLT_Sponge_Change_Use;

    /// <summary>
	/// Right Table Sponge Change Set Count
	/// </summary>
	public int nRT_Sponge_Change_SetCnt;
    /// <summary>
	/// Right Table Sponge Change Current Count
	/// </summary>
	public int nRT_Sponge_Change_CurrCnt;
    /// <summary>
	/// Right Table Sponge Change 사용여부
	/// </summary>
	public bool bRT_Sponge_Change_Use;

    /// <summary>
	/// OffPicker Sponge Change Set Count
	/// </summary>
	public int nOffP_Sponge_Change_SetCnt;
    /// <summary>
	/// OffPicker Sponge Change Current Count
	/// </summary>
	public int nOffP_Sponge_Change_CurrCnt;
    /// <summary>
    /// OffPicker Sponge Change 사용여부
    /// </summary>
    public bool bOffP_Sponge_Change_Use;


    /// <summary>
    /// 메시지창 닫은 후 다시 표시 시간(분) : PM Count 항목은 아니지만 별도로 만들지 않으려고...
    /// </summary>
    public int nPMMsg_ReDisp_Minute;

}
// 2021.07.14 lhs End

//211122 pjh : Dresser 
public struct TDrs
{
    #region Dresser Infomation
    /// <summary>
    /// Dresser Outer 
    /// 드레셔 외경 (입력 불가) [mm]
    /// </summary>
    public double dDrsOuter;
    /// <summary>
    /// Dresser Height 
    /// 드레셔 높이 (입력 불가) [mm]
    /// </summary>
    public double dDrsH;
    /// <summary>
    /// Dresser Name 
    /// 드레셔 이름 (입력 값)
    /// </summary>
    public string sDrsName;
    #endregion
}

