using System.Drawing;
using System.Windows.Forms;
/// <summary>
/// Global Variable
/// </summary>
public class GV
{
    #region Path
    /// <summary>
    /// Config Folder Path  StartUp\\Config\\
    /// </summary>
    public static readonly string PATH_CONFIG = Application.StartupPath + "\\Config\\";
    /// <summary>
    /// Wheel Folder Path  StartUp\\Wheel\\
    /// </summary>
    public static readonly string PATH_WHEEL = Application.StartupPath + "\\Wheel\\";
    /// <summary>
    /// Device Folder Path  StartUp\\Device\\
    /// </summary>
    public static readonly string PATH_DEVICE = Application.StartupPath + "\\Device\\";
    /// <summary>
    /// Equipment Motion Folder Path  StartUp\\Equip\\Motion\\
    /// </summary>
    public static readonly string PATH_EQ_MOTION = Application.StartupPath + "\\Equip\\Motion\\";
    /// <summary>
    /// Equipment IO Folder Path  StartUp\\Equip\\IO\\
    /// </summary>
    public static readonly string PATH_EQ_IO = Application.StartupPath + "\\Equip\\IO\\";
   /// <summary>
    /// Equipment Probe Folder Path  StartUp\\Equip\\Probe\\
    /// </summary>
    public static readonly string PATH_EQ_PROBE = Application.StartupPath + "\\Equip\\Probe\\";

    //20191108 ghk_light
    /// <summary>
    /// Equipment Light Folder Path  StartUp\\Equip\\Light\\
    /// </summary>
    public static readonly string PATH_EQ_LIGHT = Application.StartupPath + "\\Equip\\Light\\";   

    /// <summary>
    /// Equipment Barcode Vision Folder Path  StartUp\\Equip\\Bcr\\
    /// </summary>
    public static readonly string PATH_EQ_BCR = Application.StartupPath + "\\Equip\\Bcr\\";
    /// <summary>
    /// Equipment Motion WMX File Path  StartUp\\Equip\\Motion\\wmx_parameters.xml
    /// </summary>
    public static readonly string PATM_WMX_XML = PATH_EQ_MOTION + "wmx_parameters.xml";
    //20200905 Count Base Monitor Mode
    /// <summary>
    /// Equipment Light Folder Path  StartUp\\Equip\\Light\\
    /// </summary>
    public static readonly string PATH_EQ_CBM = Application.StartupPath + "\\Equip\\CBM\\";
    //20201004 lhs Table Measure 8 Points
    /// <summary>
    /// Equipment Table Measure 8 Point Folder Path  StartUp\\Equip\\Tm8\\
    /// </summary>
    public static readonly string PATH_EQ_TM8 = Application.StartupPath + "\\Equip\\TM8\\";
    /// <summary>
    /// Log Folder Path  D:\\Log_SG2X\\
    /// </summary>
    public static readonly string PATH_LOG = @"D:\Log_SG2X\";
    /// <summary>
    /// SPC Folder Path D:\\SPC\\
    /// </summary>
    public static readonly string PATH_SPC = @"D:\SPC\";
    /// <summary>
    /// 에러 파일 저장 경로
    /// </summary>
    public static readonly string PATH_ERR = Application.StartupPath + "\\Error\\";
    /// <summary>
    /// 에러 사진 저장 경로
    /// </summary>
    public static readonly string PATH_ERRIMG = Application.StartupPath + "\\Error\\Err_image\\";
    /// <summary>
    /// 에러 로그 파일 저장 경로
    /// </summary>
    public static readonly string PATH_ERRLOG = @"D:\SPC\ErrLog\";
    //20190614 ghk_dfserver
    /// <summary>
    /// DfServer 정보 저장
    /// </summary>
    //public static readonly string PATH_EQ_DF = Application.StartupPath + "\\Equip\\DfServer\\";
	public static readonly string PATH_EQ_DF = Application.StartupPath + "\\DfServer\\";     // 2021.05.28 SungTae : 경로 변경
    //
	/// <summary>
    /// 210707 pjh : OCR Key In 시 Key In 화면에 불러 올 이미지 경로
    /// </summary>
    public static readonly string PATH_ImageLoad = @"D:\Suhwoo\2D OCR\";
    //
    /// <summary>
    /// 220328 pjh : ASE KH 등록 된 User 정보 경로
    /// </summary>
    public static readonly string PATH_USER = Application.StartupPath + "\\User\\";
    #endregion

    /// <summary>
    /// 측정 값의 소수점 표시 갯수 기본 3개
    /// </summary>
    public static readonly int MEA_POINT = 3;

    /// <summary>
    /// Input Color Array
    /// </summary>
    public static readonly Color[] CR_X = new Color[2] { Color.LightGray, Color.Lime };
    /// <summary>
    /// Output Color Array
    /// </summary>
    public static readonly Color[] CR_Y = new Color[2] { Color.LightGray, Color.Red };

    /// <summary>
    /// 테이블 최소 사용 가능 높이 [mm]
    /// </summary>
    public static readonly double EQP_TABLE_MIN_THICKNESS = 19.0;
    /// <summary>
    /// 드레셔 고정 브라켓 높이 [mm]
    /// </summary>
    //public static readonly double EQP_DRESSER_BLOCK_HEIGHT = 7.5; //190221 ksg : 실측이 높게 나옴
    public static readonly double EQP_DRESSER_BLOCK_HEIGHT = 8.5;
    /// <summary>
    /// X Model 테이블 중앙에서 툴세터까지 거리 [mm] (설계치)  154
    /// </summary>
    public static readonly double EQP_TABLE_CENTER_TO_TOOLSETTER = 175.0;
    /// <summary>
    /// U Model 테이블 중앙에서 툴세터까지 거리 [mm] (설계치)  154
    /// </summary>
    public static readonly double EQP_TABLE_CENTER_TO_TOOLSETTER_U_MODEL = 190.0;
    /// <summary>
    /// 4U Model 테이블 중앙에서 툴세터까지 거리 [mm] (설계치)  154
    /// </summary>
    public static readonly double EQP_TABLE_CENTER_TO_TOOLSETTER_4U_MODEL = 187.5;
    /// <summary>
    /// X Model 테이블 중앙에서 드레셔 중앙까지 거리 [mm] (설계치)
    /// </summary>
    public static readonly double EQP_TABLE_CENTER_TO_DRESSER_CENTER = 250.0;
    /// <summary>
    /// U Model 테이블 중앙에서 드레셔 중앙까지 거리 [mm] (설계치)
    /// </summary>
    public static readonly double EQP_TABLE_CENTER_TO_DRESSER_CENTER_U_MODEL  = 265.0;
    /// <summary>
    /// U Model 테이블 중앙에서 드레셔 중앙까지 거리 [mm] (설계치) 262.5
    /// </summary>
    public static readonly double EQP_TABLE_CENTER_TO_DRESSER_CENTER_4U_MODEL = 262.5;
    /// <summary>
    /// 휠 리미트 맥스 값[mm]
    /// 9
    /// </summary>
    public static readonly double WHEEL_LIMIT_MAX = 9;
    /// <summary>
    /// 드레셔 리미트 맥스 값[mm]
    /// 4
    /// </summary>
    public static readonly double DRESSER_LIMIT_MAX = 4;

    /// <summary>
    /// Axis Delay Time [ms]
    /// </summary>
    public static readonly int AX_DELAY = 300;

    /// <summary>
    /// On Loader Magazine Detect Delay Time [ms]
    /// </summary>
    public static readonly int ONL_DETECT_MGZ_DELAY = 12000;
    /// <summary>
    /// On Loader Belt Back Motion Delay Time [ms]
    /// </summary>
    public static readonly int ONL_BELT_BACK_DELAY = 100;

    //20191007 ghk_drystick_time
    public static readonly int DRY_STICK_DELAY = 500;
    // 200314 mjy : 신규추가
    /// <summary>
    /// Unit 반복 운동 시 End 도달 후 대기 딜레이 [ms]
    /// </summary>
    public static readonly int DRY_UNIT_DELAY = 500;
    /// <summary>
    /// Unit vacuum 동작시 딜레이 [ms]
    /// </summary>
    public static readonly int UNIT_VAC_DELAY = 1000;
    /// <summary>
    /// 프로브 다운 완료 대기 딜레이 [ms]
    /// </summary>
    //public static readonly int PRB_DELAY = 1000;
    //public static readonly int PRB_DELAY = 2000; //190610 ksg : Delay 늘림
    public static int PRB_DELAY = 2000; //190712 ksg : 업체별로 다른값 적용 해야 할 듯
    /// <summary>
    /// 프로브 업 완료 시까지 Max. 대기 시간
    /// </summary>
    public static int PRB_UPDLY = 10000; //20200518 syc : 
    /// <summary>
    /// 프로브 업 완료 체크 후 절대 대기 시간
    /// </summary>
    public static int PRB_UPWAIT = 500; //20200518 syc : 

    /// <summary>
    /// Error Array Max Size
    /// </summary>
    public static readonly int MAX_ERR = 400;

    public static readonly double MEA_ZEROSET = 0.000;

    //20190514 ghk_Inr_probe
    /// <summary>
    /// Qorvo 인레일 다이나믹 펑션용 프로브 업다운 센서 제거
    /// 업다운 시 확인 용 옵션
    /// </summary>
    //public static readonly int INR_PRB_RANGE = 1;
    public static readonly int INR_PRB_RANGE = 5;


    /// <summary>
    /// Strip Default Short Side Distance [mm]
    /// </summary>
    public static readonly double STRIP_DEF_SHORT = 74.0;
    /// <summary>
    /// Strip Default Long Side Distance [mm]
    /// </summary>
    public static readonly double STRIP_DEF_LONG = 240.0;
    /// <summary>
    /// Wheel Default Outer Size [mm]
    /// </summary>
    public static readonly double WHEEL_DEF_OUTER = 180.0;
      /// <summary>
    /// Wheel Default Tip Thickness [mm]
    /// </summary>
    public static readonly double WHEEL_DEF_TIP_T = 12.0;
      /// <summary>
    /// Wheel Default Tip Width [mm]
    /// </summary>
    public static readonly double WHEEL_DEF_TIP_W = 5.0;

    //20190926 ghk_ofppadclean
    /// <summary>
    /// Sponge width(mm)
    /// </summary>
    public static readonly int SPONGE_WIDTH = 15;
    /// <summary>
    /// OffloadPicker Pad Clean Z Axis Up Pos
    /// </summary>
    public static readonly int OFP_PAD_CLEAN_Z_UP = 2;
    /// <summary>
    /// Package가 Unit 일때 캐리어에 최대 Unit 갯수
    /// </summary>
    public static readonly int PKG_MAX_UNIT = 4;
    /// <summary>
    /// 그라인딩 시작, 종료 위치 계산 시 추가적으로 주는 옵셋 [mm]
    /// </summary>
    public static readonly int GRD_WHL_OFFSET = 10;

    //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
    /// <summary>
    /// 다이나믹 펑션 측정 포지션 최대 수,
    /// 관련 배열 초기화 시 이 값을 적용해야 함
    /// </summary>
    public static readonly int DFPOS_MAX = 5;
    //
    //200407 jym : 추가
    /// <summary>
    /// 그라인딩 사이클 뎁스 이동 후 인포지션 확인 범위 0.0005mm
    /// </summary>
    public static readonly double INPOS_GRD_CYCLE = 0.0005;
    /// <summary>
    /// 그라인딩, 드레싱 테이블 반복 이동 후 인포지션 확인 범위 1mm
    /// </summary>
    public static readonly double INPOS_TBL = 1;
    //
    //20200424 jhc : 회전형 드라이 동작 타임아웃 시간 연장 : 180초
    /// <summary>
    /// 회전형 드라이 동작 타임아웃
    /// </summary>
    public static readonly int DRYROTATION_TMOUT = 180000; //180,000 ms = 3 min
    //
    /////Log
    /// <summary>
    /// false : MR, true : AR
    /// </summary>
    public static bool bAR = false;
    /// <summary>
    /// false : None, true : Error
    /// </summary>
    public static bool bErr = false;
    //200414 ksg : 12 Step  기능 추가
    /// <summary>
    /// Grinding Step Max Count 13
    /// </summary>
    public static int StepMaxCnt = 13;
    // 2020.09.08 SungTae : 3 Step용  기능 추가
    /// <summary>
    /// Grinding Step Max Count 4
    /// </summary>
    public static int StepMaxCnt_3 = 4;
    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    /// <summary>
    /// 측정 포인트를 다르게 설정하는 Lot Open 후 첫 n개 스트립의 최대 수량 (UI 표시/설정용)
    /// </summary>
    public static int LEADER_STRIP_CNT_MAX = 28; //ASE-KR 1 Lot = 2 MGZ = 28 Strip
    /// <summary>
    /// 측정 포인트 최대 수 (SECS/GEM 측정데이터 배열 사이즈 설정/제한용)
    /// 초과시 => 측정 전 Error 팝업, Device Save 전 Error 팝업
    /// </summary>
    public static int STRIP_MEASURE_POINT_MAX = 100;
    /// <summary>
    /// Leader Strip Contact Point Use 설정 시 표시할 색상 (UI 표시용)
    /// 0: 비측정 포인트(Window단위로 색상 변경됨)
    /// 1: Main Strip 전용 측정 포인트(Color Lime)
    /// 2: Leader Strip 전용 측정 포인트(Color Cyan)
    /// 3: Leader Strip, Main Strip 공용 측정 포인트(Color Gold)
    /// </summary>
    public static readonly Color [] CELL_BK_COLOR = { Color.White/*가변*/, Color.Lime, Color.Yellow, Color.Pink };
    //

    /// <summary>
    /// //20200710    josh    프로브업체크 입력신호&프로브값 같이 비교
    /// 프로브 업으로 예상하는 프로브 앰프 출력 값
    /// </summary>
    public static double dProbeUpHeight = 17.0;

    /// <summary>
    /// 임시로 사용하는 변수 추후 ASE KR VOC 적용 할때 풀것
    /// 휠 히스토리 양식 변경
    /// </summary>
    public static bool WHEEL_HISTORY = true; //200824 jhc : ASE-Kr Release O
    /// <summary>
    /// 임시로 사용하는 변수 추후 ASE KR VOC 적용시 해제할것
    /// 디바이스와 휠 파일 동기화 
    /// </summary>
    public static bool DEV_WHL_SYNC = false; //200824 jhc : ASE-Kr Release X
    /// <summary>
    /// 20200731 josh    dual dressing
    /// 임시 사용 변수 (ASE-Kr VOC 적용 시 true 설정 또는 삭제 예정)
    /// 매뉴얼 좌/우 동시 드레싱 기능
    /// </summary>
    public static bool MANUAL_DUAL_DRESSING = true; //200824 jhc : ASE-Kr Release O

    //200731 myk : Dressing Position 자동화 관련 변수 추가
    /// <summary>
    /// Dressing Start Position 관련 변수
    /// </summary>
    public static double dDressStartPos = 150.0;
    //200731 myk : Dressing Position 자동화 관련 변수 추가
    /// <summary>
    /// Dressing End Position 관련 변수
    /// </summary>
    public static double dDressEndPos = 42.0;
    //200810 myk : Probe Calibration Check 관련 변수 추가
    /// <summary>
    /// Probe Calibration Check 오차범위 최대값 관련 변수
    /// </summary>
    public static double dProbeCalCheck = 0.0005;
    /// <summary>
    /// Idle 모드 활성화 여부
    /// </summary>
    public static bool IDLE_MODE = false;
    /// <summary>
    /// 엔코더 로그 저장 여부
    /// </summary>
    public static bool ENC_LOG = false;
    /// <summary>
    /// [임시] 드레싱 할때 2번째 스텝 내리고 시작 사용
    /// </summary>
    public static bool TEMP_DRS_1STEP = false;
    /// <summary>
    /// 잘못 측정 되었을 때 재 반복 횟수
    /// </summary>
    public static int MEASURE_RETRY = 3;

    // 201110 jym : Skyworks batch file
    /// <summary>
    /// 스카이웍스 전용 디바이스 관리 프로그램 경로
    /// </summary>
    public static string SKW_BATCH = PATH_CONFIG + "Suhwoo GlobalCfg.exe";

    //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
    /// <summary>
    /// Orientation Check 1회 Skip 옵션 표시 플래그 : 표시(true)/비표시(false) : 현 시점 ASE-Kr만 표시
    /// </summary>
    public static bool ORIENTATION_ONE_TIME_SKIP = false;

    //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
    /// <summary>
    /// 휠 소모량 보정값 설정 최대 회수 (드레싱 후 최대 몇번째 그라인딩까지 휠 소모량 보정값 설정을 하겠느냐?)
    /// </summary>
    public static readonly int WHEEL_LOSS_CORRECT_STRIP_MAX = 15;
    //

    //20201217 myk : Table Vacuum 값 Log 추가 
    /// <summary>
    /// Table Vacuum 값 0일 시 Analog Input 값
    /// </summary>
    public static int TblVacZeroValue = 16520;

    //20201217 myk : Table Vacuum 값 Log 추가 
    /// <summary>
    /// Table Vacuum 값 1감소 시 Analog Input 감소량
    /// </summary>
    public static int TblVacOneValue = 110;

    //201217 jhc : 테이블 버큠
    /// <summary>
    /// Table Vacuum 최대 압력 값
    /// 이 값 기준으로 점점 낮아지는 값을 갱신하며 실시간 최저값 체크
    /// 단위 : kPa
    /// </summary>
    public static double TABLE_VACUUM_MAX = -999;
    //201217 yyy : 스핀들 전류
    /// <summary>
    /// 스핀들 전류 SDO Read 값 계수
    /// SDO Read 값 1 => 0.032 A
    /// </summary>
    public static double SPINDLE_CURRENT_COEFFICIENT = 0.032;
    /// <summary>
    /// 스핀들 최대 설정 전류(출하 시 정격전류 : 32 A)
    /// 단위 : mA
    /// </summary>
    public static int SPINDLE_CURRENT_MAX = 32000;
    
    // 2022.09.01 lhs
    /// <summary>
    /// 스핀들 Peak Current 전류(인버터 Peak Current : 56.57 Arms)
    /// 단위 : mA
    /// </summary>
    public static int SPINDLE_PEAK_CURRENT = 56570;

    //211122 pjh : Dresser 정보 저장 경로
    ///<summary>
    /// Dresser Folder Path  StartUp\\Dresser\\
    /// </summary>
    public static readonly string PATH_DRESSER = Application.StartupPath + "\\Dresser\\";
    //..
}
