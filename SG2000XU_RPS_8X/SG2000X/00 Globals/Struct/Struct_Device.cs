using System;
using System.Drawing;

/// <summary>
/// 측정 위치 구조체
/// </summary>
public struct tCont
{
    public bool bUse;
    public double dX;
    public double dY;
    /// <summary>
    /// Unit 별 Y값 저장
    /// </summary>
    public double[] aY;
    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    /// <summary>
    /// Lot Open 후 첫 n개 스트립(Leading Strip)의 Contact Point Use/NotUse 설정 용
    /// </summary>
    public bool bUse18P;
    //
}

/// <summary>
/// 그라인딩 스텝 구조체
/// </summary>
public struct tStep
{
    /// <summary>
    /// 해당 스탭 사용 유무
    /// </summary>
    public bool bUse;
    /// <summary>
    /// 그라인딩 모드
    /// </summary>
    public eStepMode eMode;
    /// <summary>
    /// TopDown 모드 일때 = 전체 그라인딩 양 [mm], Target 모드 일때 = 해당 스텝 타켓 값[mm]
    /// </summary>
    public double dTotalDep;
    /// <summary>
    /// 사이클 그라인딩 양 [mm]
    /// </summary>
    public double dCycleDep;
    /// <summary>
    /// 테이블 속도 [mm/s]
    /// </summary>
    public double dTblSpd;
    /// <summary>
    /// 스핀들 속도 [rpm]
    /// </summary>
    public int iSplSpd;
    /// <summary>
    /// 그라인딩 시작 방향
    /// </summary>
    public eStartDir eDir;
    // 200327 mjy : 신규추가
    /// <summary>
    /// 리그라인딩 사용 여부 
    /// </summary>
    public bool bReSkip;
    /// <summary>
    /// 리그라인딩 진행 시 다음스텝 넘어가는 기준 값 [mm]
    /// </summary>
    public double dReJud;
    //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
    /// <summary>
    /// 이전 그라인딩 스텝에서 Over Grinding 발생 시 현재 스텝의 그라인딩 카운트 조정 기능 사용 여부 
    /// </summary>
    public bool bOverGrdCorrectionUse;
    //201125 jhc : Grinding Step별 Correct 기능
    /// <summary>
    /// 그라인딩 스텝별 보정두께 : 해당 스텝에서 Over 발생 경향 시 -값으로, Under 발생하여 Re-grinding 경향 시 -값으로 설정
    /// 그라인딩 스텝별 그라인딩 카운트 계산 시 해당 스텝에서의 그라인딩 량에서 설정된 값대로 -/+ 
    /// </summary>
    public double dCorrectDepth;
}

/// <summary>
/// 좌우 방향별 나뉘는 변수 구조체
/// </summary>
public struct tDevData
{
    /// <summary>
    /// 스트립 전체 두께 [mm]
    /// </summary>
    public double dTotalTh;
    /// <summary>
    /// 스트립 베이스 두께 [mm]
    /// </summary>
    public double dPcbTh;
    /// <summary>
    /// 스트립 몰드 두께 [mm]
    /// </summary>
    public double dMoldTh;
    /// <summary>
    /// 에어컷 양 [mm]
    /// </summary>
    public double dAir;

    // 200803 jym : Wheel file 연동 
    /// <summary>
    /// 휠 파일 이름
    /// </summary>
    public string sWhl;

    //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
    /// <summary>
    /// 드레싱 후 n번째 그라인딩의 Wheel Loss 량 (Max: GV.WHEEL_LOSS_CORRECT_MAX)
    /// </summary>
    public double [] dWheelLoss;
    public double dTotalWheelLossLimit;
    //

    /// <summary>
    /// 드레싱 완료 위치 
    /// 0 : ZDn-B-ZDn-F, 1 : ZDn-B-F - ZDn ~~ F,2 : ZDn-B-F - ZDn ~~ B,
    /// </summary>
    public int iDrs_YEnd_Dir;
    /// <summary>
    /// 드레싱 주기 [ea]
    /// </summary>
    public double dDrsPrid;
    /// <summary>
    /// Table spindle Factor Auto Dressing [%]
    /// </summary>
    public double dSpdAuto;
    /// <summary>
    /// Table spindle Factor Error [%]
    /// </summary>
    public double dSpdError;

    //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
    public int nSpindleCurrentDressLow;     //Lower Limit of Spindle Current when Dressing
    public int nSpindleCurrentDressHigh;    //Upper Limit of Spindle Current when Dressing
    public int nSpindleCurrentGrindLow;     //Lower Limit of Spindle Current when Grinding
    public int nSpindleCurrentGrindHigh;    //Upper Limit of Spindle Current when Grinding
    public double dTableVacuumGrindLimit;   //Lower Limit of Table Vacuum when Grinding
    public double dTableVacuumLowHoldTime;  //201231 : Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)
    //..

    // 2022.08.30 lhs Start : Spindle Current High/Low Limit 설정 (SCK+ VOC)
    /// <summary>
    /// 스핀들 전류 상한치 [mA]
    /// </summary>
    public int nSpdCurrHL;
    /// <summary>
    /// 스핀들 전류 하한치 [mA]
    /// </summary>
    public int nSpdCurrLL;
    // 2022.08.30 lhs End

    //210826 pjh : Selected wheel
    public string sSelWhl;
    //

    #region Step parameter
    /// <summary>
    /// 그라인딩 스타트 높이 방식
    /// </summary>
    public eGrdMode eGrdMod;
    /// <summary>
    /// 그라인딩 파라메터 배열
    /// </summary>
    public tStep[] aSteps;

    /// <summary>
    /// 파인 그라인딩 시작 시 모드 선택
    /// </summary>
    public eFineMode eFine;

    /// <summary>
    /// 파인 모드에서 보정 선택시 보정 값
    /// </summary>
    public double dCpen;

    /// <summary>
    /// TopDown Start Mode
    /// </summary>
    public eTDStartMode eStartMode; //190502 ksg :

    // 2021.07.27 lhs Start (SCK VOC)
    /// <summary>
    /// Mold 및 Total 기준 그라인딩
    /// </summary>
    public EBaseOnThick eBaseOnThick;
    // 2021.07.27 lhs End

    // 2022.09.26 lhs Start : Rough1 Grd Option
    /// <summary>
    /// Rough1, 첫번째 Griding시에 Z축 StartPos에 cycle depth 적용
    /// </summary>
    public bool bAppCylDepOnFirst;
    /// <summary>
    /// Rough1, 대량의 Depth를 한번에 Griding 할 목적....Air Cut 없고, Z축 StartPos에 cycle depth 적용
    /// </summary>
    public bool bAppLargeCylDep;
    // 2022.09.26 lhs End

    // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
    /// <summary>
    /// 실제로 사용할 Step 수량 [ea]
    /// </summary>
    public int nFixStepCnt;
    // 2022.07.25 SungTae End

    #endregion

    #region Contact
    public tCont[,] aPosBf;
    public tCont[,] aPosAf;

    public bool[,]  bDummy;  // 2022.01.24 lhs : 2004U, Carrier내 Dummy 설정

    /// <summary>
    /// 그라인딩 전 자재 측정 시 리밋 (Skyworks에서는 상한값)
    /// </summary>
    public double dBfLimit;

    // 2021-03-08, jhLee : Skyworks VOC, 상/하한을 별도로 관리
    /// <summary>
    /// 그라인딩 전 자재 측정 시 리밋 (Skyworks에서는 하한값)
    /// </summary>
    public double dBfLimitLower;

    /// <summary>
    /// 그라인딩 후 측정 시 리밋
    /// </summary>
    public double dAfLimit;
    /// <summary>
    /// 그라인딩 후 TTV 리밋
    /// </summary>
    public double dTTV;
    /// <summary>
    /// One Point 리밋
    /// </summary>
    public double dOneLimit; //191018 ksg :
    /// <summary>
    /// One Point Checking 시 오버그라인딩 Range 설정
    /// </summary>
    public double dOneOver; // 2020.08.19 JSKim
    /// <summary>
    /// One Point Checking 시 측정 위치 변경시 X좌표 고정
    /// </summary>
    public bool bOnePointXPosFix; // 2020.08.31 JSKim
    /// <summary>
    /// One Point Checking 시 측정 Chip Window
    /// </summary>
    public int iOnePointWin; // 2020.08.31 JSKim
    /// <summary>
    /// One Point Checking 시 측정 Chip Col
    /// </summary>
    public int iOnePointCol; // 2020.08.31 JSKim
    /// <summary>
    /// One Point Checking 시 측정 Chip Col
    /// </summary>
    public int iOnePointRow; // 2020.08.31 JSKim

    // 2022.07.25 SungTae Start : [수정] (ASE-KR 개조건)
    // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
    /// <summary>
    /// Final Target Thickness 
    /// </summary>
    public double dFinalTarget;
    // 2022.07.25 SungTae End
    #endregion

    #region ETC
    /// <summary>
    /// 탑클리너 버블 속도 [mm/s]
    /// </summary>
    public double dTpBubSpd;
    /// <summary>
    /// 탑클리너 버블 횟수
    /// </summary>
    public int iTpBubCnt;
    /// <summary>
    /// 탑클리너 에어 속도 [mm/s]
    /// </summary>
    public double dTpAirSpd;
    /// <summary>
    /// 탑클리너 에어 횟수
    /// </summary>
    public int iTpAirCnt;
    /// <summary>
    /// 탑클리너 스펀지 속도 [mm/s]
    /// </summary>
    public double dTpSpnSpd;
    /// <summary>
    /// 탑클리너 스펀지 횟수
    /// </summary>
    public int iTpSpnCnt;
    /// <summary>
    /// Left Probe RNR
    /// </summary>
    public double dPrbOffset;  //190502 ksg :
    #endregion
}

/// <summary>
/// 디바이스 전체적인 구조체
/// </summary>
public struct tDev
{
    /// <summary>
    /// 디바이스 이름
    /// </summary>
    public string sName;

    /// <summary>
    /// 마지막 수정 날짜
    /// </summary>
    public DateTime dtLast;

    /// <summary>
    /// Short Side 스트립의 짧은 부분 (가로)  [mm]
    /// </summary>
    public double dShSide;
    /// <summary>
    ///  Long Side 스트립의 긴 부분 (세로)  [mm]
    /// </summary>
    public double dLnSide;
    /// <summary>
    /// 듀얼모드 선택 변수
    /// </summary>
    public eDual bDual;
    /// <summary>
    /// 그라인딩 없이 메저측정모드 20200825 lks
    /// </summary>
    public bool bMeasureMode;
    /// <summary>
    /// Strip Dual 모드 일때 Top 또는 Btm 을 구분 하기 위함, 0 : TOP, 1: Btm
    /// </summary>
    // 2021-06-04 lhs Start : 변수명 변경 
    //public ESide bTopMoldSide; 
    public ESide eMoldSide;
    // 2021-06-04 lhs End 
    /// <summary>
    /// 좌우 나뉘는 변수들 구조체 배열,    0:좌    1:우
    /// </summary>
    public tDevData[] aData;
    /// <summary>
    /// 테이블 위에 Unit 갯수 [ea] (2000U 전용)
    /// </summary>
    public int iUnitCnt;


    #region Contact
    /// <summary>
    /// Column Count 자재 측정위치 Column 개수 (칩의 개수) [ea]
    /// </summary>
    public int iCol;
    /// <summary>
    /// Row Count 자재 측정위치 Row 개수 (칩의 개수) [ea]
    /// </summary>
    public int iRow;
    /// <summary>
    /// Chip Width 칩 자체의 가로 길이 [mm]
    /// </summary>
    public double dChipW;
    /// <summary>
    /// Chip Height 칩 자체의 세로 길이 [mm]
    /// </summary>
    public double dChipH;
    /// <summary>
    /// Chip Width Gap 칩간의 가로 간격, 1번째 칩 중앙에서 우측 칩 중앙까지 거리 [mm]
    /// </summary>
    public double dChipWGap;
    /// <summary>
    /// Chip Height Gap 칩간의 세로 간격, 1번째 칩 중앙에서 아래쪽 칩 중앙까지 거리 [mm]
    /// </summary>
    public double dChipHGap;
    /// <summary>
    /// Window Count 윈도우 갯수, 최대 5개
    /// </summary>
    public int iWinCnt;
    /// <summary>
    /// Window Start Point Array 각 윈도우의 좌상점 위치, 최대 5개 [mm]
    /// </summary>
    public PointF[] aWinSt;
    /// <summary>
    /// 각 유닛의 캐리어 중앙에서 부터 거리 (유닛 4, 3번은 양수  2, 1번은 음수)
    /// </summary>
    public double[] aUnitCen;

    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    /// <summary>
    /// Lot Open 후 측정 포인트를 별도로 설정할 첫 스트립의 수
    /// </summary>
    public int i18PStripCount;

    // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
    /// <summary>
    /// Measure(Before/After/One-point) 시 X 방향으로 보정할 Offset
    /// </summary>
    public double dMeasOffsetX;
    /// <summary>
    /// Measure(Before/After/One-point) 시 Y 방향으로 보정할 Offset
    /// </summary>
    public double dMeasOffsetY;
    // 2021.08.02 SungTae End
    #endregion

    #region ETC
    /// <summary>
    /// Magazine Count 매거진 1개에 들어가는 자재 갯수 [ea]
    /// </summary>
    public int iMgzCnt;
    /// <summary>
    /// Magazine Pitch 매거진 슬롯간 간격 사이즈 [mm]
    /// </summary>
    public double dMgzPitch;

    // 2020.10.26 SungTae Start : Add by Qorbo(Strip 배출 관련)
    /// <summary>
    /// Off-loader Magazine Count 매거진 1개에 들어가는 자재 갯수 [ea]
    /// </summary>
    public int iOffMgzCnt;
    /// <summary>
    /// Off-loader Magazine Pitch 매거진 슬롯간 간격 사이즈 [mm]
    /// </summary>
    public double dOffMgzPitch;
    // 2020.10.26 SungTae 
    
    public int iMgzDir;
    /// <summary>
    /// Use Dry Top Blow 드라이 탑 블로우 사용 유무
    /// </summary>
    public bool bDryTop;
    /// <summary>
    /// Dry Speed 드라이 속도 [rpm]
    /// </summary>
    public int iDryRPM;
    /// <summary>
    /// Dry Direction 드라이 회전 방향
    /// </summary>
    public eDryDir eDryDir;
    /// <summary>
    /// Dry Count 드라이 횟수 [ea]
    /// </summary>
    public int iDryCnt;

    /// <summary>
    /// Dry Pusher Fast Speed 드라이 푸셔 빠른속도 [mm/s]
    /// </summary>
    public double dPushF;
    /// <summary>
    /// Dry Pusher Slow Speed 드라이 푸셔 느린속도 [mm/s]
    /// </summary>
    public double dPushS;

    /// <summary>
    /// Bottom Cleaner Cleaning Picker Base Count
    /// 바텀 클리너에 피커 베이스를 닦는 횟수 [ea]
    /// </summary>
    public int iOffpClean;
    /// <summary>
    /// Bottom Cleaner Cleaning Strip Count
    /// 바텀 클리너에 스트립 바닦면을 닦는 횟수 [ea]
    /// </summary>
    public int iOffpCleanStrip;

    // 2022.03.09 lhs Start
    /// <summary>
    /// Bottom Cleaner Cleaning Picker Base Count (Air Blower)
    /// 바텀 클리너에서 피커 베이스에 Air 부는 횟수 [ea]
    /// </summary>
    public int iOffpClean_Air;
    /// <summary>
    /// Bottom Cleaner Cleaning Strip Count (Air Blower)
    /// 바텀 클리너에서 스트립 바닦면에 Air를 부는 횟수 [ea]
    /// </summary>
    public int iOffpCleanStrip_Air;
    // 2022.03.09 lhs End

    // 2022.07.27 lhs Start
    /// <summary>
    /// Brush Bottom Cleaner Strip Count
    /// 브러쉬에 스트립 바닦면을 닦는 횟수 [ea]
    /// </summary>
    public int iOffpCleanBrush;
    /// <summary>
    /// Bottom Cleaner Cleaning Strip Count : (2번째 Water Nozzle Clean)
    /// 바텀 클리너에 스트립 바닦면을 닦는 횟수 [ea] (Nozzle일 경우 Water를 분사하는 횟수)
    /// </summary>
    public int iOffpCleanStrip_N2;
    /// <summary>
    /// Bottom Cleaner Cleaning Strip Count (Air Blower) : (2번째 Water Nozzle Clean : Air )
    /// 바텀 클리너에 스트립 바닦면에 Air를 부는 횟수 [ea]
    /// </summary>
    public int iOffpCleanStrip_Air_N2;
    // 2022.07.27 lhs End

    /// <summary>
    /// On Loader Picker Vacuum Delay [ms]
    /// </summary>
    public int iPickDelayOn;
    /// <summary>
    /// On Loader Picker Place Vacuum Delay [ms]
    /// </summary>
    public int iPlaceDelayOn;
    /// <summary>
    /// On Loader Picker Blow(Eject) Delay [ms]
    /// </summary>
    public int iEjectDelayOnP;       // 2022.05.24 lhs
    /// <summary>
    /// Off Loader Picker Vacuum Delay [ms]
    /// </summary>
    public int iPickDelayOff;
    //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
    /// <summary>
    /// Off Loader Picker Place Vacuum Delay [ms]
    /// </summary>
    public int iPlaceDelayOff;
    /// <summary>
    /// Off Loader Picker Blow(Eject) Delay [ms]
    /// </summary>
    public int iEjectDelayOffP;       // 2022.05.24 lhs
    //
    /// <summary>
    /// Barcode 사용 유무 true:skip, false:use
    /// </summary>
    public bool bBcrSkip;
    /// <summary>
    /// Barcode 2nd 사용 유무 true:Use, false:Skip
    /// </summary>
    public bool bBcrSecondSkip; //190817 ksg :
    /// <summary>
    /// Orientation 사용 유무 true:skip, false:use
    /// </summary>
    public bool bOriSkip;
    //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
    /// <summary>
    /// Orientation Check 1회 Skip 기능 사용(true)/비사용(false)
    /// </summary>
    public bool bOriOneTimeSkipUse;
    //
    /// <summary>
    /// Orientation Marked 사용 유무 true:skip, false:use
    /// </summary>
    public bool bOriMarkedSkip;
    //20190218 ghk_dynamicfunction
    /// <summary>
    /// Dynamic Function 사용 유무 true:skip, false:use
    /// </summary>
    public bool bDynamicSkip;
    //20190309 ksg : Ocr 추가
    /// <summary>
    /// Ocr Function 사용 유무 true:skip, false:use
    /// </summary>
    public bool bOcrSkip;
    //20190610 ksg : LeftDataShift 추가
    /// <summary>
    /// Ocr Function 사용 유무 true:skip, false:use
    /// </summary>
    public bool bDataShift;
    //20200325 ksg : Data Shift Probe Skip
    /// <summary>
    /// Left Last target Value shift right table & Probe Skip. true : use, false : not use
    /// </summary>
    public bool bDShiftPSkip;
    //20190618 ghk_dfserver
    /// <summary>
    /// Dynamic Function Server Skip
    /// </summary>
    public bool bDfServerSkip;
    /// <summary>
    /// Bcr Manual Key In
    /// </summary>
    public bool bBcrKeyInSkip;
    //
    //20190611 ghk_onpclean
    /// <summary>
    /// 온로더 피커 클리닝 카운트
    /// </summary>
    public int iOnpCleanCnt;
    //
    //200408 Ksg : ReDoing Max Count
    /// <summary>
    /// 온로더 피커 클리닝 카운트
    /// </summary>
    public int iReDoNumMax;
    public int iReDoCntMax;
    //
    //
    //200416 pjh : Grinding Strip Start Limit
    /// <summary>
    /// Z축 Limit 저장 변수
    /// </summary>
    public double dStripStartLimit;
    public bool bStripStartLimit;

    /// <summary>
    /// 측정값에 일정 옵셋을 주어 데이터 조작 [mm]
    /// </summary>
    public double[] aFake;

    // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)
    /// <summary>
    /// Left/Right Table Cleaning Velocity [mm/sec]
    /// </summary>
    public double[] aTblCleanVel;
    // 2021.03.02 SungTae End

    // 2021.03.30 lhs Start : Dryer의 Water Nozzle의 회전 속도, 카운트 (JCET VOC)
    /// <summary>
    /// Dryer Water Nozzle 저속회전 속도[rpm]
    /// </summary>
    public int iDryWtNozzleRPM;
    /// <summary>
    /// Dryer Water Nozzle 저속회전 횟수[ea]
    /// </summary>
    public int iDryWtNozzleCnt;
    // 2021.03.30 lhs End

    #endregion

    #region OnLoader Position
    /// <summary>
    /// MGZ 고정 위치
    /// 입력불가
    /// 계산
    /// </summary>
    public double dOnL_X_Algn;
    /// <summary>
    /// 대기위치 & 자재 투입위치
    /// </summary>
    public double dOnL_Y_Wait;
    //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
    /// <summary>
    /// MGZ Barcode 읽는 위치
    /// </summary>
    public double dOnL_Y_MgzBcr;
    //
    /// <summary>
    /// 자재 투입 최초 위치
    /// 아래부터 투입
    /// </summary>
    public double dOnL_Z_Entry_Dn;
    /// <summary>
    /// 자재 투입 최초 위치
    /// 입력불가
    /// 계산
    /// </summary>
    public double dOnL_Z_Entry_Up;
    //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
    /// <summary>
    /// MGZ Barcode 읽는 위치
    /// </summary>
    public double dOnL_Z_MgzBcr;
    //
    #endregion

    #region Inrail Position
    #region Inrail X
    public double dInr_X_Pick  ;
    public double dInr_X_Bcr   ;
    public double dInr_X_Ori   ; //190211 ksg : 추가
    public double dInr_X_Vision;
    public double dInr_X_Align ;

    //20190220 ghk_dynamicfunction
    /// <summary>
    /// 다이나믹 펑션 측정 포지션 1
    /// </summary>
    public double dInr_X_DynamicPos1;
    /// <summary>
    /// 다이나믹 펑션 측정 포지션 2
    /// </summary>
    public double dInr_X_DynamicPos2;
    /// <summary>
    /// 다이나믹 펑션 측정 포지션 3
    /// </summary>
    public double dInr_X_DynamicPos3;
    //
    //20200328 jhc : 다이나믹 펑션용 PCB 측정 포인트 최대 5개로 확대
    /// <summary>
    /// 다이나믹 펑션 측정 포지션 4
    /// </summary>
    public double dInr_X_DynamicPos4;
    /// <summary>
    /// 다이나믹 펑션 측정 포지션 5
    /// </summary>
    public double dInr_X_DynamicPos5;
    /// <summary>
    /// <summary>
    /// Device > SET POSITION > InRail > Dynamic Function 1~5 Position에 설정된 값에 따라 실제 DF 포지션 수가 결정됨
    /// DynamicPos1~3 입력 필수 (즉, DF 측정 포지션 수는 최소 3개 이상이어야 함),
    /// DF 사용 활성화 옵션(!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)일 때,
    /// (DynamicPos1 == 0) || (DynamicPos2 == 0) || (DynamicPos3 == 0) => Error, DF 포지션 수: 3개로 설정됨,
    /// DynamicPos4 == 0, DynamicPos5 == 0 => DF 포지션 수: 3개로 설정됨,
    /// DynamicPos4 != 0, DynamicPos5 == 0 => DF 포지션 수: 4개로 설정됨,
    /// DynamicPos4 == 0, DynamicPos5 != 0 => DF 포지션 수: 4개로 설정됨, DynamicPos5의 값이 DynamicPos4에 할당됨
    /// DynamicPos4 != 0, DynamicPos5 != 0 => DF 포지션 수: 5개로 설정됨,
    /// Default = 3개
    /// </summary>
    public int iDynamicPosNum;
    //
    #endregion

    #region Inrail Y
    public double dInr_Y_Align;
    #endregion
    #endregion

    #region On Loader Picker Position
    public double dOnP_X_Wait;
    public double dOnP_Z_Pick;
    public double dOnP_Z_Place;
    public double dOnP_Z_Slow;

    //20190604 ghk_onpbcr
    public double dOnp_X_Bcr     ;
    public double dOnp_X_Ori     ;
    public double dOnp_X_BcrSecon; //190821 ksg :
    public double dOnp_Z_Bcr     ;
    public double dOnp_Z_Ori     ;
    public double dOnp_Y_Bcr     ;
    public double dOnp_Y_Ori     ;
    public double dOnp_Y_BcrSecon; //190821 ksg :
    //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
    public double dOnp_X_BcrErr;
    public double dOnp_Y_BcrErr;
    public double dOnp_Z_BcrErr;
    //
    //20191010 ghk_manual_bcr
    public double dOnp_X_Bcr_TbL;
    public double dOnp_X_Ori_TbL;
    public double dOnp_X_Bcr_TbR;
    public double dOnp_X_Ori_TbR;
    public double dOnp_Z_Bcr_TbL;
    public double dOnp_Z_Ori_TbL;
    public double dOnp_Z_Bcr_TbR;
    public double dOnp_Z_Ori_TbR;
    public double dOnp_Y_Bcr_TbL;
    public double dOnp_Y_Ori_TbL;
    public double dOnp_Y_Bcr_TbR;
    public double dOnp_Y_Ori_TbR;
    //

    //20190611 ghk_onpclean
    public double dOnp_X_Clean;
    public double dOnp_Z_Clean;
    //

    //211026 syc : Onp Pick Up Offset
    public double dOnp_Z_PickOffset;
    //
    #endregion

    #region Grind Position Array
    public double[] aGrd_Y_Start;
    public double[] aGrd_Y_End;
    //20191011 ghk_manual_bcr
    public double[] aGrd_Y_Ori;
    //
    #endregion

    #region Off-Picker Position
    public double dOffP_X_ClnStart;
    public double dOffP_X_ClnEnd;
    public double dOffP_X_ClnCenter;    // 2021.03.18 SungTae : 고객사(ASE-KR) 요청으로 Bottom Cleaning 시 Center Position 추가
    public double dOffP_Z_Pick;
    public double dOffP_Z_Place;
    public double dOffP_Z_Slow;
    public double dOffP_Z_PlaceDn;

    // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 추가
    public double dOffP_Z_ClnStart;
    public double dOffP_Z_StripClnStart;
    // 2021.02.27 SungTae End

    // 2022.07.27 lhs Start : Brush Clean Position
    public double dOffP_X_ClnStart_Brush;
    public double dOffP_X_ClnEnd_Brush;
    public double dOffP_Z_StripClnStart_Brush;
    // 2022.07.27 lhs End
    #endregion

    #region Dry Position
    public double dDry_R_Check1;
    public double dDry_R_Check2;

    //Rotate Model 의 경우 사용 파라메터 20200301 LCY
    public double dDry_Car_Check;
    public double dDry_Unit_Check;
    public double dDry_Start;
    public double dDry_End;

    #endregion

    #region Off Loader Position
    /// <summary>
    /// MGZ 고정 위치
    /// 입력불가
    /// 계산
    /// </summary>
    public double dOffL_X_Algn;
    /// <summary>
    /// 대기위치 & 자재 받는위치
    /// </summary>
    public double dOffL_Y_Wait;
    //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
    /// <summary>
    /// MGZ Barcode 읽는 위치
    /// </summary>
    public double dOffL_Y_MgzBcr;
    //
    /// <summary>
    /// 자재 투입 최초 위치
    /// 아래부터 투입
    /// </summary>
    public double dOffL_Z_TRcv_Dn;
    /// <summary>
    /// 자재 투입 최초 위치
    /// 입력불가
    /// 계산
    /// </summary>
    public double dOffL_Z_TRcv_Up;
    /// <summary>
    /// 자재 투입 최초 위치
    /// 아래부터 투입
    /// </summary>
    public double dOffL_Z_BRcv_Dn;
    /// <summary>
    /// 자재 투입 최초 위치
    /// 입력불가
    /// 계산
    /// </summary>
    public double dOffL_Z_BRcv_Up;
    /// <summary>
    /// 자재 투입 위치(Slot기준)
    /// 입력 불가
    /// 계산
    /// </summary>
    public double dOffL_Z_Work;
    //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
    /// <summary>
    /// MGZ Barcode 읽는 위치
    /// </summary>
    public double dOffL_Z_MgzBcr;
    //
    #endregion

    //210824 syc : 2004U
    #region IV2 설정 값
    //ONP-------------------------------------------------------start
    /// <summary>
    /// ONP 1st 위치 사용 파라미터
    /// </summary>
    public string sIV2_ONP1_Para;
    /// <summary>
    /// ONP 2nd 위치 사용 파라미터
    /// </summary>
    public string sIV2_ONP2_Para;
    /// <summary>
    /// ONP 1st X 위치
    /// </summary>
    public double dIV2_ONP1_X;
    /// <summary>
    /// ONP 1st Y 위치
    /// </summary>
    public double dIV2_ONP1_Y;
    /// <summary>
    /// ONP 1st Z 위치
    /// </summary>
    public double dIV2_ONP1_Z;
    /// <summary>
    /// ONP 2nd 사용 유무
    /// </summary>
    public bool bIV2_ONP2_Use;
    /// <summary>
    /// ONP 2nd X 위치
    /// </summary>
    public double dIV2_ONP2_X;
    /// <summary>
    /// ONP 2nd Y 위치
    /// </summary>
    public double dIV2_ONP2_Y;
    /// <summary>
    /// ONP 2nd Z 위치
    /// </summary>
    public double dIV2_ONP2_Z;
    //ONP-------------------------------------------------------end
    //OFP-------------------------------------------------------start
    /// <summary>
    /// OFP 1st 위치 사용 파라미터
    /// </summary>
    public string sIV2_OFP1_Para;
    /// <summary>
    /// OFP 2nd 위치 사용 파라미터
    /// </summary>
    public string sIV2_OFP2_Para;
    /// <summary>
    /// OFP Cover Check 위치 사용 파라미터
    /// </summary>
    public string sIV2_OFPCover_Para;
    /// <summary>
    /// OFP 1st X 위치
    /// </summary>
    public double dIV2_OFP1_X;
    /// <summary>
    /// OFP 1st Z 위치
    /// </summary>
    public double dIV2_OFP1_Z;
    /// <summary>
    /// ONP 2nd 사용 유무
    /// </summary>
    public bool bIV2_OFP2_Use;
    /// <summary>
    /// OFP 2nd X 위치
    /// </summary>
    public double dIV2_OFP2_X;
    /// <summary>
    /// OFP 2nd Z 위치
    /// </summary>
    public double dIV2_OFP2_Z;
    /// <summary>
    /// OFP Cover Check X 위치
    /// </summary>
    public double dIV2_OFPCover_X;
    /// <summary>
    /// OFP Cover Check Z 위치
    /// </summary>
    public double dIV2_OFPCover_Z;
    #endregion

    // 2022.03.24 lhs : 2004U, Dummy Data 전달용
    #region CopyDummy 
    /// <summary>
    /// Dummy Data 전달용, OnPicker Pick 및 Place시에 Dummy 전달.
    /// </summary>
    public bool[,] bCopyDummy;
    #endregion

    // 2022-05-26, jhLee : Skyworks OCR 14자리 자릿수 추가
    public int iDigitType;      // OCR 자릿수 0:10자리 (기존),  1:14자리 (신규)
}
