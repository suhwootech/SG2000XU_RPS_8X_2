using SuhwooUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SG2000X
{
    public class CDataOption : CStn<CDataOption>
    {
        //Bcr 적용 상태
        public static eEquType         CurEqu      ; //Picker Y축 유무                                : Nomal: 스카이웍스, Ase Kr   Picker :  그외 사이트
        public static eBcrKeyIn        KeyInType   ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
        public static eOnpClean        eOnpClean   ; //OnLoader Picker Clean 사용 여부                :Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
        public static eDfserver        eDfserver   ; //Use : ASE KR, ASE K26                                                        - Not Use : 그외 사이트       //20190703 ghk_dfserver
        public static eDfServerType    MeasureDf   ; //DF Server 사용시 DF사용 안함 측정               : ASE Kr, DF 서버 사용시 DF 측정 사용 : 그외 사이트           //20191029 ghk_dfserver_notuse_df
        public static eDfProbe         eDfType     ; //DF 모듈                                                                                                   //20190910 ksg df Probe Type
        public static eSecsGemVer      SecsGemVer; //Secs Gem 사용 버전                            : StandardA 기존 2000X 사양     StandardB 1000시리즈 사양
        public static eSecsGem         SecsUse     ; //Secs Gem 사용 유무                             :Use : Jsck                    - Not use   : 기타
        public static eOffPadCleanType PadClean    ; //OFP Clean Type                                : UDType : JSCP                - LRType    : 그 외 사이트    //20190926 ghk_ofppadclean
        public static eMeasureType     MeasureType ; //Wheel Tip Measure Type                        : Step : 스카이웍스             - Jog       : 그 외 사이트
        public static eManualBcr       ManualBcr   ; //Table Bcr Read                                : Use : JSCK                   - Not Use   : 그 외 사이트    //20191010 ghk_manual_bcr
        public static eSpindleType     SplType     ; //Spindle 구동 타입                              : Rs232 : 1 ~ 5호기            - Ethercat : 6 ~ 이후 쭉
        public static eAutoToolOffset  AutoOffset  ; //Wheel 높이 자동 계산                           : Use : 스카이웍스, Ase Kr      - Not Use  : 그 외 사이트     //20191028 ghk_auto_tool_offset
        public static eUseToolSetterIO UseIO       ; //Tool Setter IO 사용 여부                       : Use : 그 외 사이트            - Not Use   : K26, 스카이웍스 //20191025 ksg :
        public static eLightControl    LightControl; //Light Contorol 사용여부                        : Use : 스카이웍스              - Not Use   : 그외 사이트     //20191106 ghk_lightcontrol
        public static eReGrdLog        ReGrdLog    ; //ReGrd Log 사용 여부                            : Use : JSCK                   - Not Use   : 그외 사이트 
        public static eTblSetPos       TblSetPos   ; //Table Grd Manual Set Pos :아직 모름                                                       
        public static eAutoWarmUp      AutoWarmUp  ; //AutoWarmUp 사용 유무                           : Use : JSCK                   - Not Use   : 그외 사이트
        public static eManualDoorLock  ManualLock  ; //Manual Door Lock 사용 유무                     : Use : Sky Works, Qorvo, SST  - Not Use   : 그외 사이트
        public static ePcwAutoOff      PcwAutoOff  ; //Pcw Auto Off 사용 유무                         : Use : Qorvo, SST             - Not Use   : 그외 사이트
        public static eChkGrdLeak      ChkGrdLeak  ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
        public static eChkDryLeak      ChkDryLeak  ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
        public static eZUpOffset       ZUpOffset   ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
        //20191206 ghk_lotend_placemgz
        public static eLotEndPlaceMgz LotEndPlcMgz ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
        //20191211 ghk_display_strip
        public static eDisplayStrip DisplayStrip   ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
        public static eTableClean   TableClean     ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
        public static eOflRFID      OflRfid        ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트
        public static eOnlRFID      OnlRfid        ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트
        /// <summary>
        /// RFID Type ->  None:사용안함  RFID:RFID센서 사용(SCK/+)  Keyence:Keyence 바코드 리더기(SPIL)
        /// </summary>
        public static ERfidType     RFID;
        public static ePumpAutoOff  PumpOff        ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트
        public static eBcrProtocol  BcrPro         ; //Protocol 방식(Table 측정 여부)                 : Old : 그 외 사이트           - New     : SKC+, SKC    //200131 ksg :
        public static ePkg         Package         ; //Package 형태                                  : Strip : 그 외 사이트       - Unit : Hisilicon //200228
        public static eDryer        Dryer	       ; //Dryer 형태                                  : Rotate : 그 외 사이트         - Knife : Hisilicon //200228
        public static eBcrInterface BcrInterface   ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
        /// <summary>
        /// Picker pick 동작 시 Table eject on delay time [ms]
        /// </summary>
        public static int EjtDelay;
        /// <summary>
        /// Picker pick 동작 시 Table water on delay time [ms]
        /// </summary>
        public static int WtrDelay;
        /// <summary>
        /// 리그라인딩 스킵기능 사용 유무
        /// </summary>
        public static bool IsReSkip;    // 리그라인딩 스킵 사용 여부        true:사용         false:미사용
        /// <summary>
        /// 리그라인딩 판정 값 설정 사용 유무
        /// </summary>
        public static bool IsReJudge;   // 리그라인딩 판정 값 설정 사용 여부  true:사용       false:미사용
        /// <summary>
        /// 동작 완료 및 오토 러닝 중 테이블 워터 상시 온 사용 유무
        /// </summary>
        public static bool IsTblWater;  // 동작 완료 및 오토러닝 중 테이블에 워터 상시 온  true:사용    false:미사용
        /// <summary>
        /// Buffer motion 시퀀스 사용 유무
        /// </summary>
        public static bool IsBfSeq;
        //20200415 jhc : Picker Idle Time 개선
        public static ePickerTimeImprove PickerImprove; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
        //20200515 myk : Wheel Clenaer Water 추가
        /// <summary>
        /// Wheel Cleaner 사용 유무
        /// </summary>
        public static bool IsWhlCleaner; // Dressing, Grinding 시 Wheel Cleaner 워터 온  true:사용    false:미사용
        // 200618 jym : 추가
        /// <summary>
        /// 온로더, 오프로더 라이트 커튼 감지 시 일시정지 기능 사용 유무
        /// </summary>
        public static bool IsPause;
		//200619 pjh : Bottom Air 추가
        /// <summary>
        /// Bottom Bubble Air Sensor 사용 여부 : true : 사용 - false : 미사용
        /// </summary>
        public static bool IsBtmAir; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
        // 200708 jym : 추가
        /// <summary>
        /// 그라인딩 이후 After data에 일정 옵셋을 주어 데이터를 조작 사용 여부  true:사용  false:미사용
        /// </summary>
        public static bool IsFakeAf;
        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        /// <summary>
        /// Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
        /// </summary>
        public static bool Use18PointMeasure;
        /// <summary>
        /// Grinding중에 DI, PCW 체크하는 구문 사용여부
        /// </summary>
        public static bool IsChkDI;
        // 200730 myk : Manual Grinding 시 Barcode 입력 기능 추가
        /// <summary>
        /// Manual Grinding 시 Barcode 입력 기능 추가
        /// </summary>
        public static bool IsGrdBcr; // Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)   false : 미사용
        // 200907 jym : 신규 추가 (SCK VOC)
        /// <summary>
        /// 1 포인트 측정 위치 변경에 대한 사용유무  true : 위치설정 사용   false : 미사용 (정해진 위치로 자동 사용, 기본값)
        /// </summary>
        public static bool Is1Point = false;
        /// <summary>
        /// 화면에 출력되는 디바이스 파라메터 스탭의 갯수  3, 12(기본값)
        /// </summary>
        public static int StepCnt = 12;
        // 200908 jym : 신규 추가 (SPIL VOC)
        /// <summary>
        /// Picker vacuum 접점을 반대로 사용유무  true : 접점 반대로 사용(신호 꺼지면 흡착)   false : 미사용 (기본값)
        /// </summary>
        public static bool IsRevPicker = false;
        /// <summary>
        /// Probe ejector 사용 유무  true : 사용    false : 미사용 (기본값)
        /// </summary>
        public static bool IsPrbEjector = false;
        //201022 lhs : Table 높이 Auto측정(8Point)
        /// <summary>
        /// Table 높이 Auto측정(8Point) 사용여부 : true=사용(JSCK VOC), false=미사용(기본값)  
        /// </summary>
        public static bool UseTableMeas8p = false;
        //201025 jhc : Over Grinding Correction - Grinding Count Correction 기능 제공/감춤 용
        public static bool UseGrindingCorrect = false;
        //
        // 201116 jym : QC 모듈 관련 추가
        /// <summary>
        /// QC 모듈 사용 유무  true:사용, false:미사용(기본값)
        /// </summary>
        public static bool UseQC = false;
        //
        //201125 jhc : Grinding Step별 Correct 기능 제공/감춤 용
        public static bool UseGrindingStepCorrect = false;
        //
		// 2020.11.23 JSKim St
        /// <summary>
        /// Dressing Air Cut Using, Replace 구분 사용 유무  true : 구분 사용    false : 미 구분 사용(기본값)
        /// </summary>
        public static bool IsDrsAirCutReplace = false;
        // 2020.11.23 JSKim Ed
        //
        //201202 jhc : Auto offset(Auto tool setter gap) 변동 시 알람 제한 값 설정 기능 제공/감춤 용
        public static bool UseAutoToolSetterGapLimit = false;
        //
        //201203 jhc
        public static bool UseDeviceAdvancedOption = false; //DEVICE > PARAM > ADVANCED 메뉴(추가) 표시/감춤 용
        public static bool UseWheelLossCorrect = false;     //드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
        //
        //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
        public static bool UseAdvancedGrindCondition = false;
        //
        //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
        public static bool Use2DErrorPosition = false;
        //
        //syc : ase kr loading stop - Auto Loading 기능 및 Loading Stop 위치설정 사용 유무, Opt -> Genaral
        public static bool UseAutoLoadingStop = false;
        //
        //syc : new cleaner
        public static bool UseNewClenaer = false;
        //
        // 2021.03.30 lhs Dryer Water Nozzle 사용여부 <--- JCET에서 요청, 실제 설치 안됨.
        public static bool UseDryWtNozzle = false;

        // 2022.06.29 lhs :
        /// <summary>
        /// OffPicker에 설치된 IV를 이용하여 Dryer의 Strip 존재여부 및 Clamp이 정상인지를 확인
        /// I/O 신호로 동작함.      ( cf. 2004U에서 사용하는 IV2는 이더넷으로 동작함 )
        /// </summary>
        public static bool UseOffP_IOIV = false;

        // 2022.07.07 lhs : IV로 Dryer Table에 Strip이 없는지를 확인하는 사이클 사용
        public static bool UseSeq_IV_DryTable = false;

        // 2022.06.30 lhs
        /// <summary>
        /// 드라이 클램프 종류
        /// </summary>
        public static EDryClamp eDryClamp = EDryClamp.None;

        // 2021.05.18 SungTae : [추가] 2003U 개조 관련 설비 구분 변수
        public static int iEquipClass = 0;

        //라이선스 적용 신규 파라미터
        string version;
        string compCode;
        string compName;
        //string licDate;
        bool useLicense;
        //bool isAddedNode = false;

        // 2020.09.29 jhLee : 신규추가, SPIL에서 요청한 MainView/SPC 화면 표시 내용 추가 요청건
        /// <summary>
        /// 데이터 표시내용 확장  0:사용안함, 1:추가된 내용 표시(Main화면에 Tab으로 Befer/After Measure data표시, SPC 그래프에 Min/Max/평균값 표시)
        /// </summary>
        public static int nDispDataExtend = 0;

        // 2021.04.30 jhLee : 연속LOT (Multi-LOT) 기능 활성화 여부
        /// <summary>
        /// 연속 LOT 기능 사용  false:사용안함, true:사용 함
        /// </summary>
        public static bool UseMultiLOT = false;

        //210705 pjh : Set up strip function
        /// <summary>
        /// Set up strip function 0 : Not use 1 : Use
        /// </summary>
        public static bool UseSetUpStrip = false;

        // 2021.07.06 lhs Start : 새로운 SCK Grinding Process & SecsGem  사용여부 (SCK 전용)
        // TopD, BtmS 모드 추가
        // (SecsGem으로 PCB, Top Mold, Btm Mold 값을 별도로 주고 받을 수 있도록 추가, Mold 기준으로 Grinding 가능)
        /// <summary>
        /// New SCK Grinding Process & SecsGem 사용여부 : false = Not use,  true = Use
        /// </summary>
        public static bool UseNewSckGrindProc = false;
        // 2021.07.06 lhs End

        // 210727 pjh : Wheel 및 Dresser 최대 높이 설정
        /// <summary>
        /// Wheel 및 Dresser 최대 높이 설정 옵션 사용 N : Not use Y : Use
        /// </summary>
        public static bool UseWheelDresserMaxLimit = false;

        //210817 pjh : Net drive License로 관리
        /// <summary>
        /// Net drive 사용 Y : Use N : Not use
        /// D/F Server Net drive 사용 Y : Use N : Not use
        /// </summary>
        public static bool UseNetDrive = false;
        public static bool UseDFNet    = false;
        //
        //210818 pjh : D/F Data Server 기능 License로 구분
        /// <summary>
        /// DF Server 사용 Y : Use N : Not use
        /// </summary>
        public static bool UseDFDataServer = false;

        /// <summary>
        /// 2021.08.20 syc : Qorvo Auto user level change 기능 원하지 않아 기능 라이센스화
        /// </summary>
        public static bool SkipAutoUserLevelChange = false;
        //

        //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
        /// <summary>
        /// DF Server 사용 Y : Use N : Not use
        /// </summary>
        public static bool UseDeviceWheel = false;

        //210824 syc : 2004U
        /// <summary>
        /// Unit 타입의 자재를 그라인딩 하지만
        /// 대체적인 SEQ는 Strip 로직을 사용하기 때문에 Strip타입을 사용
        /// 2004U 사용
        /// </summary>
        public static bool Use2004U = false;
        /// <summary>
        /// Unit을 Strip 처럼 사용 할때 IV2 카메라 사용 유무
        /// IV2 카메라가 문제가 생겼을 경우 임시조치로 해제하고 사용할 수 있도록 만듬 (비상용)
        /// </summary>
        public static bool UseIV2 = false;

        //211022 syc : Onp Pick Up Offset
        /// <summary>
        /// ONP로 Table 에서 자재 Pick시
        /// Pick Position 일정값 조절하는 기능
        /// 2004U Unit 타입 "Place" 포지션 하나로 Pick, Place 시 Unit 피커 패드에 붙음
        /// </summary>
        public static bool UseOnpPickUpOffset = false;

        //211119 pjh : Seperated Dresser Information
        /// <summary>
        /// Skyworks Option
        /// Dresser 정보도 Wheel과 별개로 관리할 수 있도록 요청
        /// </summary>
        public static bool UseSeperateDresser = false;

        // 211223 syc : 10point auto cal
        /// <summary>
        /// false :  5Point Auto cal (기본값)
        /// true  : 10Point Auto cal 
        /// </summary>
        public static bool Use10PointAutoCal = false;

        // 2022.03.02 lhs : QLE SG2004U BTM CLEANER MODIFY 
        /// <summary>
        /// Sponge -> Spray Nozzle형
        /// (OUT) Nozzle 구동용 Sol V/V, Air Blower Sol V/V 추가
        /// (IN)  Nozzle Forward, Nozzle Backward
        /// </summary>
        public static bool UseSprayBtmCleaner = false;

        // 2022.07.27 lhs : QLE SG2004U BTM BRUSH MODIFY
        /// <summary>
        /// Btm Water Nozzle에 추가적으로 Brush 장착
        /// 시퀀스 : Water Nozzle -> Brush -> Water Nozzle -> Dry
        /// </summary> 
        public static bool UseBrushBtmCleaner = false;


        // 2022.03.14 lhs : (JCET) Dry Level Safety Sensor Air Blow
        /// <summary>
        /// Dry Level Safety Sensor에 Air Blow 추가 설치
        /// </summary>
        public static bool UseDryerLevelAirBlow = false;

        /// <summary>
        /// 220328 pjh : Dual Grinding Option
        /// </summary>
        public static bool UseDualGrinding = false;

        /// <summary>
        /// 220330 pjh : ASE KH Card Reader
        /// </summary>
        public static bool UseCardReader = false;

        // 2022.06.16 lhs Start
        /// <summary>
        /// On-Picker 진공해제 센서 사용여부
        /// </summary>        
        public static bool UseOnPVacuumFree = false;
        /// <summary>
        /// Off-Picker 진공해제 센서 사용여부
        /// </summary>        
        public static bool UseOffPVacuumFree = false;
        // 2022.06.16 lhs End

        // 2022.09.26 lhs
        /// <summary>
        /// Device Rough 01 Option : 첫 그라인딩시 Cycle Depth 적용, 대량의 Depth를 한 번에 그라인딩
        /// </summary>
        public static bool UseDevR1Option = false;
        

        //
        // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==


        private CDataOption()
        {

        }
        
        public bool SetOptionNew(bool isDebug = false)
        {

            string licPath = GV.PATH_CONFIG;
            //isDebug = true;
            //신규 라이선스파일 사용
            string[] licfiles = Directory.GetFiles(licPath, "suhwoo*.lic");
            if (licfiles.Length > 1)
            {
                CMsg.Show(eMsg.Error, "Error", "License Initialize Failed\r\n " + "The .lic file must be one");
                return false;
            }
            else if(licfiles.Length == 1)
                licPath = licfiles[0];//"suhwoo.lic";
            else //파일 없음
            {
                //SetOption();
                CMsg.Show(eMsg.Error, "Error", "License Initialize Failed\r\n " + "suhwoo.lic file not exists");
                return false;
            }

            if (File.Exists(licPath))
            {
                try
                {
                    CLog.mLogSeq("■ CDATA Option(suhwoo.lic) START ■");

                    //licPath = GV.PATH_CONFIG + "suhwoo.lic";
                    string fileString = File.ReadAllText(licPath);
                    string licString;
                    if (!DecodeString(fileString, out licString))
                    {
                        CMsg.Show(eMsg.Error, "Error", "License Initialize Failed\r\n " + "Decrypt failed");
                        return false;
                    }

                    XmlDocument xmlLicense = new XmlDocument();

                    xmlLicense.LoadXml(licString);

                    //라이선스 정보
                    XmlNode InfoNodes = xmlLicense.SelectSingleNode("/LICENSE/CDATA_INFO");
                    
                    useLicense = InTextToBool(ref InfoNodes, "LICFILE_USE", true);              
                    CData.useLicense = useLicense;                                              if (isDebug) CLog.mLogSeq("  - " + "LICFILE_USE" + " : " + useLicense.ToString());

                    version = InTextToString(ref InfoNodes, "VERSION", "1.0");                  if (isDebug) CLog.mLogSeq("  - " + "VERSION" + " : " + version.ToString());
                    compCode = InTextToString(ref InfoNodes, "COMPANYCODE", "Suhwoo");          if (isDebug) CLog.mLogSeq("  - " + "COMPANYCODE" + " : " + compCode.ToString());
                    compName = InTextToString(ref InfoNodes, "COMPANYNAME", "SUHWOO TECH.");    
                    CData.CurCompanyName = compName;                                            if (isDebug) CLog.mLogSeq("  - " + "COMPANYNAME" + " : " + compName.ToString());

                    string licDate = InTextToString(ref InfoNodes, "LICENSE_ENDDATE", "2100-12-31");
                    DateTime tempLicDate;
                    if (DateTime.TryParse(licDate, out tempLicDate)) CData.licenseDate = tempLicDate;
                    if (isDebug) CLog.mLogSeq("  - " + "LICENSE_ENDDATE" + " : " + licDate.ToString());

                    if (!Enum.TryParse<ECompany>(compCode, out CData.CurCompany))
                    {
                        //useLicense = false;
                        //CData.useLicense = useLicense;
                        //SetOption();
                        CMsg.Show(eMsg.Error, "Error", "License Initialize Failed\r\n " + "CompanyCode Error");
                        return false;
                    }

                    if (!useLicense)
                    {
                        SetOption();
                        return true;
                    }

                    //CDATA Option 항목 처리
                    XmlNode OptionNodes = xmlLicense.SelectSingleNode("/LICENSE/CDATA_OPTION");

                    CurEqu = (eEquType)InTextToInteager(ref OptionNodes, "CUR_EQUIP" , 0);                      if (isDebug) CLog.mLogSeq("  - " + "CUR_EQUIP" + " : " + CurEqu.ToString());
                    KeyInType = (eBcrKeyIn)InTextToInteager(ref OptionNodes, "KEY_IN_TYPE", 0);                 if (isDebug) CLog.mLogSeq("  - " + "KEY_IN_TYPE" + " : " + KeyInType.ToString());
                    eOnpClean = (eOnpClean)InTextBoolToInteager(ref OptionNodes, "ONP_CLEAN_USE", 0);           if (isDebug) CLog.mLogSeq("  - " + "ONP_CLEAN_USE" + " : " + eOnpClean.ToString());
                    eDfserver = (eDfserver)InTextBoolToInteager(ref OptionNodes, "DFSERVER_USE", 0);            if (isDebug) CLog.mLogSeq("  - " + "DFSERVER_USE" + " : " + eDfserver.ToString());
                    MeasureDf = (eDfServerType)InTextToInteager(ref OptionNodes, "DFSERVER_MEASURE_DF_USE", 0); if (isDebug) CLog.mLogSeq("  - " + "DFSERVER_MEASURE_DF_USE" + " : " + MeasureDf.ToString());

                    eDfType = (eDfProbe)InTextToInteager(ref OptionNodes, "DFTYPE", 0);                         if (isDebug) CLog.mLogSeq("  - " + "DFTYPE" + " : " + eDfType.ToString());
                    SecsUse = (eSecsGem)InTextBoolToInteager(ref OptionNodes, "SECSGEM_USE", 0);                if (isDebug) CLog.mLogSeq("  - " + "SECSGEM_USE" + " : " + SecsUse.ToString());
                    SecsGemVer = (eSecsGemVer)InTextToInteager(ref OptionNodes, "SECSGEM_VER", 1/*0*/);              if (isDebug) CLog.mLogSeq("  - " + "SECSGEM_VER" + " : " + SecsGemVer.ToString());
                    PadClean = (eOffPadCleanType)InTextToInteager(ref OptionNodes, "PAD_CLEAN", 0);             if (isDebug) CLog.mLogSeq("  - " + "PAD_CLEAN" + " : " + PadClean.ToString());
                    MeasureType = (eMeasureType)InTextToInteager(ref OptionNodes, "MEASURE_TYPE", 0);           if (isDebug) CLog.mLogSeq("  - " + "MEASURE_TYPE" + " : " + MeasureType.ToString());
                    ManualBcr = (eManualBcr)InTextBoolToInteager(ref OptionNodes, "MANUAL_BCR", 0);             if (isDebug) CLog.mLogSeq("  - " + "MANUAL_BCR" + " : " + ManualBcr.ToString());

                    SplType = (eSpindleType)InTextToInteager(ref OptionNodes, "SPINDLE_TYPE", 0);               if (isDebug) CLog.mLogSeq("  - " + "SPINDLE_TYPE" + " : " + SplType.ToString());
                    AutoOffset = (eAutoToolOffset)InTextBoolToInteager(ref OptionNodes, "AUTO_TOOLOFFSET", 0);  if (isDebug) CLog.mLogSeq("  - " + "AUTO_TOOLOFFSET" + " : " + AutoOffset.ToString());
                    UseIO = (eUseToolSetterIO)InTextBoolToInteager(ref OptionNodes, "TOOLSETTER_IO", 0);        if (isDebug) CLog.mLogSeq("  - " + "TOOLSETTER_IO" + " : " + UseIO.ToString());
                    LightControl = (eLightControl)InTextBoolToInteager(ref OptionNodes, "LIGHTCONTROL", 0);     if (isDebug) CLog.mLogSeq("  - " + "LIGHTCONTROL" + " : " + LightControl.ToString());
                    ReGrdLog = (eReGrdLog)InTextBoolToInteager(ref OptionNodes, "REGRDLOG_USE", 0);             if (isDebug) CLog.mLogSeq("  - " + "REGRDLOG_USE" + " : " + ReGrdLog.ToString());

                    TblSetPos = (eTblSetPos)InTextBoolToInteager(ref OptionNodes, "TABLE_SETPOSITION", 0);      if (isDebug) CLog.mLogSeq("  - " + "TABLE_SETPOSITION" + " : " + TblSetPos.ToString());
                    AutoWarmUp = (eAutoWarmUp)InTextBoolToInteager(ref OptionNodes, "AUTO_WARMUP", 0);          if (isDebug) CLog.mLogSeq("  - " + "AUTO_WARMUP" + " : " + AutoWarmUp.ToString());
                    ManualLock = (eManualDoorLock)InTextBoolToInteager(ref OptionNodes, "MANUAL_DOORLOCK", 0);  if (isDebug) CLog.mLogSeq("  - " + "MANUAL_DOORLOCK" + " : " + ManualLock.ToString());
                    PcwAutoOff = (ePcwAutoOff)InTextBoolToInteager(ref OptionNodes, "PCW_AUTOOFF", 0);          if (isDebug) CLog.mLogSeq("  - " + "PCW_AUTOOFF" + " : " + PcwAutoOff.ToString());
                    ChkGrdLeak = (eChkGrdLeak)InTextBoolToInteager(ref OptionNodes, "CHECK_GRDLEAK_SENSOR", 0); if (isDebug) CLog.mLogSeq("  - " + "CHECK_GRDLEAK_SENSOR" + " : " + ChkGrdLeak.ToString());

                    ChkDryLeak = (eChkDryLeak)InTextBoolToInteager(ref OptionNodes, "CHECK_DRYLEAK_SENSOR", 0); if (isDebug) CLog.mLogSeq("  - " + "CHECK_DRYLEAK_SENSOR" + " : " + ChkDryLeak.ToString());
                    ZUpOffset = (eZUpOffset)InTextBoolToInteager(ref OptionNodes, "ZUP_OFFSET", 0);             if (isDebug) CLog.mLogSeq("  - " + "ZUP_OFFSET" + " : " + ZUpOffset.ToString());
                    LotEndPlcMgz = (eLotEndPlaceMgz)InTextBoolToInteager(ref OptionNodes, "LOTEND_PLACEMGZ", 0); if (isDebug) CLog.mLogSeq("  - " + "LOTEND_PLACEMGZ" + " : " + LotEndPlcMgz.ToString());
                    DisplayStrip = (eDisplayStrip)InTextBoolToInteager(ref OptionNodes, "DISPLAY_STRIP", 0);    if (isDebug) CLog.mLogSeq("  - " + "DISPLAY_STRIP" + " : " + DisplayStrip.ToString());
                    TableClean = (eTableClean)InTextBoolToInteager(ref OptionNodes, "TABLE_CLEAN", 0);          if (isDebug) CLog.mLogSeq("  - " + "TABLE_CLEAN" + " : " + TableClean.ToString());

                    OflRfid = (eOflRFID)InTextBoolToInteager(ref OptionNodes, "OFL_RFID", 0);                   if (isDebug) CLog.mLogSeq("  - " + "OFL_RFID" + " : " + OflRfid.ToString());
                    OnlRfid = (eOnlRFID)InTextBoolToInteager(ref OptionNodes, "ONL_RFID", 0);                   if (isDebug) CLog.mLogSeq("  - " + "ONL_RFID" + " : " + OnlRfid.ToString());
                    PumpOff = (ePumpAutoOff)InTextBoolToInteager(ref OptionNodes, "PUMP_AUTOOFF", 0);           if (isDebug) CLog.mLogSeq("  - " + "PUMP_AUTOOFF" + " : " + PumpOff.ToString());
                    BcrPro = (eBcrProtocol)InTextToInteager(ref OptionNodes, "BCR_PROTOCOL", 0);                if (isDebug) CLog.mLogSeq("  - " + "BCR_PROTOCOL" + " : " + BcrPro.ToString());
                    Package = (ePkg)InTextToInteager(ref OptionNodes, "PACKAGE_TYPE", 0);                       if (isDebug) CLog.mLogSeq("  - " + "PACKAGE_TYPE" + " : " + Package.ToString());

                    Dryer = (eDryer)InTextToInteager(ref OptionNodes, "DRYER_TYPE", 0);                         if (isDebug) CLog.mLogSeq("  - " + "DRYER_TYPE" + " : " + Dryer.ToString());
                    BcrInterface = (eBcrInterface)InTextToInteager(ref OptionNodes, "BCR_INTERFACE", 0);        if (isDebug) CLog.mLogSeq("  - " + "BCR_INTERFACE" + " : " + BcrInterface.ToString());
                    EjtDelay = InTextToInteager(ref OptionNodes, "EJECT_ON_DELAY", 3000);                       if (isDebug) CLog.mLogSeq("  - " + "EJECT_ON_DELAY" + " : " + EjtDelay.ToString());
                    WtrDelay = InTextToInteager(ref OptionNodes, "WATER_ON_DELAY", 3000);                       if (isDebug) CLog.mLogSeq("  - " + "WATER_ON_DELAY" + " : " + WtrDelay.ToString());
                    IsReSkip = InTextToBool(ref OptionNodes, "REGRD_SKIP", false);                              if (isDebug) CLog.mLogSeq("  - " + "REGRD_SKIP" + " : " + IsReSkip.ToString());

                    IsReJudge = InTextToBool(ref OptionNodes, "REGRD_JUDGE", false);                            if (isDebug) CLog.mLogSeq("  - " + "REGRD_JUDGE" + " : " + IsReJudge.ToString());
                    IsTblWater = InTextToBool(ref OptionNodes, "TABLE_WATER", false);                           if (isDebug) CLog.mLogSeq("  - " + "TABLE_WATER" + " : " + IsTblWater.ToString());
                    IsBfSeq = InTextToBool(ref OptionNodes, "BUFFER_SEQ_USE", false);                           if (isDebug) CLog.mLogSeq("  - " + "BUFFER_SEQ_USE" + " : " + IsBfSeq.ToString());
                    PickerImprove = (ePickerTimeImprove)InTextBoolToInteager(ref OptionNodes, "PICKER_IMPROVE_USE", 0); if (isDebug) CLog.mLogSeq("  - " + "PICKER_IMPROVE_USE" + " : " + PickerImprove.ToString());
                    IsWhlCleaner = InTextToBool(ref OptionNodes, "WHEEL_CLEAN_WATERON", false);                 if (isDebug) CLog.mLogSeq("  - " + "WHEEL_CLEAN_WATERON" + " : " + IsWhlCleaner.ToString());

                    IsPause = InTextToBool(ref OptionNodes, "PAUSE_USE", false);                                if (isDebug) CLog.mLogSeq("  - " + "PAUSE_USE" + " : " + IsPause.ToString());
                    IsBtmAir = InTextToBool(ref OptionNodes, "BOTTOM_AIR", false);                              if (isDebug) CLog.mLogSeq("  - " + "BOTTOM_AIR" + " : " + IsBtmAir.ToString());
                    IsFakeAf = InTextToBool(ref OptionNodes, "FAKE_AF", false);                                 if (isDebug) CLog.mLogSeq("  - " + "FAKE_AF" + " : " + IsFakeAf.ToString());
                    Use18PointMeasure = InTextToBool(ref OptionNodes, "POINT18_MEASURE", false);                if (isDebug) CLog.mLogSeq("  - " + "POINT18_MEASURE" + " : " + Use18PointMeasure.ToString());

                    IsChkDI = InTextToBool(ref OptionNodes, "CHECK_DIPCW", false);                              if (isDebug) CLog.mLogSeq("  - " + "CHECK_DIPCW" + " : " + IsChkDI.ToString());
                    IsGrdBcr = InTextToBool(ref OptionNodes, "GRIND_BCRINPUT_USE", false);                      if (isDebug) CLog.mLogSeq("  - " + "GRIND_BCRINPUT_USE" + " : " + IsGrdBcr.ToString());

                    //---------------------------------------------------------------------------------------------------------------------------//
                    // 이후 추가되는 옵션은 Default 값 처리를 하며, 옵션 해당 고객사는 반드시 업데이트 된 lic파일을 같이 전달
                    //---------------------------------------------------------------------------------------------------------------------------//
                    //추가 영역 시작
                    Is1Point = InTextToBool(ref OptionNodes, "CHANGE_1POINT", false);                           if (isDebug) CLog.mLogSeq("  - " + "CHANGE_1POINT" + " : " + Is1Point.ToString());
                    StepCnt = InTextToInteager(ref OptionNodes, "STEP_COUNT", 12);                              if (isDebug) CLog.mLogSeq("  - " + "STEP_COUNT" + " : " + StepCnt.ToString());
                    IsRevPicker = InTextToBool(ref OptionNodes, "REVERSE_PICKER_VACUUM", false);                if (isDebug) CLog.mLogSeq("  - " + "REVERSE_PICKER_VACUUM" + " : " + IsRevPicker.ToString());
                    IsPrbEjector = InTextToBool(ref OptionNodes, "PROBE_EJECTOR", false);                       if (isDebug) CLog.mLogSeq("  - " + "PROBE_EJECTOR" + " : " + IsPrbEjector.ToString());
                    nDispDataExtend = InTextToInteager(ref OptionNodes, "DISPLAY_DATA_EXTEND", 0);              if (isDebug) CLog.mLogSeq("  - " + "DISPLAY_DATA_EXTEND" + " : " + nDispDataExtend.ToString());
                    RFID = (ERfidType)InTextToInteager(ref OptionNodes, "RFID_TYPE", 0);                        if (isDebug) CLog.mLogSeq("  - " + "RFID_TYPE" + " : " + RFID.ToString()); //201012 jhc : 라이선스 파일에 누락되었던 옵션 항목 추가
					UseTableMeas8p  = InTextToBool(ref OptionNodes, "TABLE_MEASURE_8P", false);                 if (isDebug) CLog.mLogSeq("  - " + "TABLE_MEASURE_8P" + " : " + UseTableMeas8p.ToString());
                    //201025 jhc : Over Grinding Correction - Grinding Count Correction 기능 제공/감춤 용
                    UseGrindingCorrect = InTextToBool(ref OptionNodes, "GRINDING_CORRECT", false);              if (isDebug) CLog.mLogSeq("  - " + "GRINDING_CORRECT" + " : " + UseGrindingCorrect.ToString());
                    //
                    // 201116 jym : QC 추가
                    UseQC = InTextToBool(ref OptionNodes, "QC_USE", false); if (isDebug) CLog.mLogSeq("  - " + "QC_USE" + " : " + UseQC.ToString());
                    //201125 jhc : Grinding Step별 Correct 기능 제공/감춤 용
                    UseGrindingStepCorrect = InTextToBool(ref OptionNodes, "GRIND_STEP_CORRECT", false);        if (isDebug) CLog.mLogSeq("  - " + "GRIND_STEP_CORRECT" + " : " + UseGrindingStepCorrect.ToString());
					// 2020.11.23 JSKim St
                    IsDrsAirCutReplace = InTextToBool(ref OptionNodes, "DRESSING_AIRCUT_REPLACE", false);       if (isDebug) CLog.mLogSeq("  - " + "DRESSING_AIRCUT_REPLACE" + " : " + IsDrsAirCutReplace.ToString());
                    // 2020.11.23 JSKim Ed
                    //
                    //201202 jhc : Auto offset(Auto tool setter gap) 변동 시 알람 제한 값 설정 기능 제공/감춤 용
                    UseAutoToolSetterGapLimit = InTextToBool(ref OptionNodes, "AUTO_TOOLSETTER_GAP_LIMIT", false);  if (isDebug) CLog.mLogSeq("  - " + "AUTO_TOOLSETTER_GAP_LIMIT" + " : " + UseAutoToolSetterGapLimit.ToString());
                    //
                    //201203 jhc
                    //DEVICE > PARAM > ADVANCED 메뉴(추가) 표시/감춤 용
                    UseDeviceAdvancedOption = InTextToBool(ref OptionNodes, "DEVICE_ADVANCED_OPTION", false);   if (isDebug) CLog.mLogSeq("  - " + "DEVICE_ADVANCED_OPTION" + " : " + UseDeviceAdvancedOption.ToString());
                    //드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
                    UseWheelLossCorrect = InTextToBool(ref OptionNodes, "WHEEL_LOSS_CORRECT", false);           if (isDebug) CLog.mLogSeq("  - " + "WHEEL_LOSS_CORRECT" + " : " + UseWheelLossCorrect.ToString());
                    //
                    //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                    UseAdvancedGrindCondition = InTextToBool(ref OptionNodes, "USE_ADVANCED_GRIND_CONDITION", false); if (isDebug) CLog.mLogSeq("  - " + "USE_ADVANCED_GRIND_CONDITION" + " : " + UseAdvancedGrindCondition.ToString());
                    //
                    //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
                    Use2DErrorPosition = InTextToBool(ref OptionNodes, "USE_2D_ERROR_POSITION", false);         if (isDebug) CLog.mLogSeq("  - " + "USE_2D_ERROR_POSITION" + " : " + Use2DErrorPosition.ToString());
                    //
                    // syc : ASE-KR, SKYWORKS loading stop
                    UseAutoLoadingStop = InTextToBool(ref OptionNodes, "USE_AUTO_LOADING_STOP", false); if (isDebug) CLog.mLogSeq("  - " + "USE_AUTO_LOADING_STOP" + " : " + UseAutoLoadingStop.ToString());
                    //
                    //syc : new cleaner
                    UseNewClenaer = InTextToBool(ref OptionNodes, "USE_NEW_CLEANER", false); if (isDebug) CLog.mLogSeq("  - " + "USE_NEW_CLEANER" + " : " + UseNewClenaer.ToString());

                    // 2021.03.30 lhs : Dryer의 Water Nozzle과 IV2(화상판별센서) 사용 여부 Y = Water Nozzle + IV2, N = 기존 방식(Air)
                    UseDryWtNozzle = InTextToBool(ref OptionNodes, "USE_DRYER_WATER_NOZZLE", false); if (isDebug) CLog.mLogSeq("  - " + "USE_DRYER_WATER_NOZZLE" + " : " + UseDryWtNozzle.ToString());

                    // 2022.06.29 lhs : OffPicker에 설치된 IV를 이용하여 Dryer의 Strip 존재여부 및 Clamp이 정상인지를 확인
                    UseOffP_IOIV = InTextToBool(ref OptionNodes, "USE_OFFP_IOIV", false);           if (isDebug) CLog.mLogSeq("  - " + "USE_OFFP_IOIV" + " : " + UseOffP_IOIV.ToString());
                    UseSeq_IV_DryTable = true;  // 2022.07.07 lhs AutoRun모드에서 사이클을 사용여부 변수

                    // 2022.07.01 lhs : Dryer Clamp Type, 0=None (기존 Spring Type), 1=Cylinder
                    eDryClamp = (EDryClamp)InTextToInteager(ref OptionNodes, "DRY_CLAMP_TYPE", 0);  if (isDebug) CLog.mLogSeq("  - " + "DRY_CLAMP_TYPE" + " : " + eDryClamp.ToString());

                    // 2021-04-30, jhLee : 연속LOT 사용 여부
                    UseMultiLOT = InTextToBool(ref OptionNodes, "USE_MULTI_LOT", false); if (isDebug) CLog.mLogSeq("  - " + "USE_MULTI_LOT" + " : " + UseMultiLOT.ToString());
                    
                    // 2021.05.18 SungTae : [추가] 2003U 개조 구분
                    iEquipClass = InTextToInteager(ref OptionNodes, "EQUIP_CLASSIFICATION", 0); if (isDebug) CLog.mLogSeq("  - " + "EQUIP_CLASSIFICATION" + " : " + iEquipClass.ToString());

                    //210705 pjh : Set up strip
                    UseSetUpStrip = InTextToBool(ref OptionNodes, "USE_SET_UP_STRIP", false); if (isDebug) CLog.mLogSeq("  - " + "USE_SET_UP_STRIP" + " : " + UseSetUpStrip.ToString());

                    //------------------------
                    // 2021.07.06 lhs : (SCK 전용) 새로운 SCK Grinding Process & SecsGem  사용여부  Y : 사용  N : 미사용(기본값)
                    // SecsGem으로 PCB, Top Mold, Btm Mold 값을 별도로 주고 받을 수 있도록 추가, Mold 기준으로 Grinding 가능하게...
                    UseNewSckGrindProc = InTextToBool(ref OptionNodes, "USE_NEW_SCK_GRIND_PROC", false); if (isDebug) CLog.mLogSeq("  - " + "USE_NEW_SCK_GRIND_PROC" + " : " + UseNewSckGrindProc.ToString());

                    // 2021.07.30 lhs Start :  일단, 아래의 옵션은 적용하지 않음.
                    if (UseNewSckGrindProc)
                    {
                        Use18PointMeasure = false;
                        eDfserver = eDfserver.NotUse;

                        // 다른 company 옵션

                    }
                    // 2021.07.30 lhs End 
                    //------------------------

                    //210728 pjh : Wheel/Dresser Limit 사용
                    UseWheelDresserMaxLimit = true;
                    //UseWheelDresserMaxLimit = InTextToBool(ref OptionNodes, "USE_WHEEL_DRESSER_MAX_LIMIT", false); if (isDebug) CLog.mLogSeq("  - " + "USE_WHEEL_DRESSER_MAX_LIMIT" + " : " + UseWheelDresserMaxLimit.ToString());

                    //210817 pjh : Net Drive 사용
                    UseNetDrive = InTextToBool(ref OptionNodes, "USE_NET_DRIVE", false); if (isDebug) CLog.mLogSeq("  - " + "USE_NET_DRIVE" + " : " + UseNetDrive.ToString());
                    UseDFNet    = InTextToBool(ref OptionNodes, "USE_DF_NET_DRIVE", false); if (isDebug) CLog.mLogSeq("  - " + "USE_DF_NET_DRIVE" + " : " + UseDFNet.ToString());

                    //210818 pjh : D/F Data Server 기능 License로 구분
                    UseDFDataServer = InTextToBool(ref OptionNodes, "USE_DF_DATA_SERVER", false); if (isDebug) CLog.mLogSeq("  - " + "USE_DF_DATA_SERVER" + " : " + UseDFDataServer.ToString());

                    //2021.08.20 syc : Qorvo Auto user level change 기능 원하지 않아 기능 라이센스화
                    //Qorvo 외 타사이트는 적용을 기본으로 사용할 예정, 따라서 N : 기능 사용, Y 기능 스킵
                    SkipAutoUserLevelChange = InTextToBool(ref OptionNodes, "Skip_Auto_User_Level_Change", false); if (isDebug) CLog.mLogSeq("  - " + "Skip_Auto_User_Level_Change" + " : " + SkipAutoUserLevelChange.ToString());

                    //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
                    UseDeviceWheel = InTextToBool(ref OptionNodes, "USE_DEVICE_WHEEL", false); if (isDebug) CLog.mLogSeq("  - " + "USE_DEVICE_WHEEL" + " : " + UseDeviceWheel.ToString());

                    //210824 syc : 2004U
                    Use2004U    = InTextToBool(ref OptionNodes, "Use2004U", false); if (isDebug) CLog.mLogSeq("  - " + "Use2004U" + " : " + Use2004U.ToString());
                    UseIV2      = InTextToBool(ref OptionNodes, "UseIV2",   false); if (isDebug) CLog.mLogSeq("  - " + "UseIV2"   + " : " + UseIV2.ToString());     //2022.01.25 lhs 오타수정 Use2004U->UseIV2
                    //Use2004U = false; // lhs 임시테스트
                    //

                    //211022 syc : Onp Pick Up Offset
                    UseOnpPickUpOffset  = InTextToBool(ref OptionNodes, "Use_OnpPickUpOffset",   false); if (isDebug) CLog.mLogSeq("  - " + "Use_OnpPickUpOffset" + " : " + UseOnpPickUpOffset.ToString());

                    //211119 pjh : Dresser 정보 별도로 관리
                    UseSeperateDresser  = InTextToBool(ref OptionNodes, "USE_SEPERATED_DRESSER", false); if (isDebug) CLog.mLogSeq("  - " + "USE_SEPERATED_DRESSER" + " : " + UseSeperateDresser.ToString());

                    // 211223 syc : 10point auto cal
                    Use10PointAutoCal   = InTextToBool(ref OptionNodes, "Use_10PointAutoCal",    false); if (isDebug) CLog.mLogSeq("  - " + "Use_10PointAutoCal" + " : " + Use10PointAutoCal.ToString());

                    // 2022.03.03 lhs : Spray Nozzle형 Bottom Cleaner
                    // <!--Spray Nozzle형 Bottom Cleaner 사용 유무: Y = 사용, N = 사용안함(기본값)-->
                    UseSprayBtmCleaner = InTextToBool(ref OptionNodes, "USE_SPRAY_BTMCLEANER", false); if (isDebug) CLog.mLogSeq("  - " + "USE_SPRAY_BTMCLEANER" + " : " + UseSprayBtmCleaner.ToString());
                    //UseSprayBtmCleaner = false; // 2022.03.08 lhs 임시테스트

                    // 2022.07.27 lhs : Brush Bottom Cleaner // Strip Clean용으로 Brush 추가 장착
                    // <!--Brush Bottom Cleaner 사용 유무: Y = 사용, N = 사용안함(기본값)-->
                    UseBrushBtmCleaner = InTextToBool(ref OptionNodes, "USE_BRUSH_BTMCLEANER", false); if (isDebug) CLog.mLogSeq("  - " + "USE_BRUSH_BTMCLEANER" + " : " + UseBrushBtmCleaner.ToString());
                    //UseBrushBtmCleaner = false; // 2022.07.27 lhs 임시테스트

                    // 2022.03.14 lhs : (JCET) Dry Safe Sensor AirBlow
                    UseDryerLevelAirBlow = InTextToBool(ref OptionNodes, "USE_DRYER_LEVEL_AIRBLOW", false); if (isDebug) CLog.mLogSeq("  - " + "USE_DRYER_LEVEL_AIRBLOW" + " : " + UseDryerLevelAirBlow.ToString());
                    //UseDryerLevelAirBlow = false; // 2022.03.14 lhs 임시테스트

                    //220328 pjh : Dual Grinding Option
                    UseDualGrinding = InTextToBool(ref OptionNodes, "USE_DUAL_GRINDING", false); if(isDebug) CLog.mLogSeq("  - " + "USE_DUAL_GRINDING" + " : " + UseDualGrinding.ToString());
                    //

                    //220330 pjh : ASE KH Card Reading Option
                    UseCardReader       = InTextToBool(ref OptionNodes, "USE_CARD_READER",     false);  if (isDebug) CLog.mLogSeq("  - " + "USE_CARD_READER" + " : " + UseCardReader.ToString());

                    // 2022.06.16 lhs Start : 
                    // OnPicker 진공해제 센서 신호 사용
                    UseOnPVacuumFree    = InTextToBool(ref OptionNodes, "USE_ONP_VACUUM_FREE", false);  if (isDebug) CLog.mLogSeq("  - " + "USE_ONP_VACUUM_FREE" + " : " + UseOnPVacuumFree.ToString());

                    // Off-Picker EjectOk : 진공해제 감지센서 사용여부
                    UseOffPVacuumFree   = InTextToBool(ref OptionNodes, "USE_OFP_VACUUM_FREE", false);  if (isDebug) CLog.mLogSeq("  - " + "USE_OFP_VACUUM_FREE" + " : " + UseOffPVacuumFree.ToString());
                    // 2022.06.16 lhs End

                    // 2022.09.26 lhs : Device Rough 01 Option : Apply Cycle Depth On First count, Large Depth Grinding 
                    UseDevR1Option      = InTextToBool(ref OptionNodes, "USE_DEV_R1_OPTION",   false);  if (isDebug) CLog.mLogSeq("  - " + "USE_DEV_R1_OPTION" + " : " + UseDevR1Option.ToString());
                    //UseDevR1Option      = true; // 2022.09.26 lhs 임시테스트

                    //추가 영역 끝
                    //---------------------------------------------------------------------------------------------------------------------------//

                    InfoNodes = null;
					OptionNodes = null;
					xmlLicense = null;

					CLog.mLogSeq("■ CDATA Option(suhwoo.lic) END ■");

                    return true;
                }
                catch (Exception err)
                {
                    //라이선스 실패시 기존 코드 사용
                    CMsg.Show(eMsg.Error, "Error", "License Initialize Failed\r\n " + err.Message);
                    return false;
                    //string errMsg = err.Message;

                    //SetOption();
                }
            }
            else //파일 없음
            {
                //SetOption();
                CMsg.Show(eMsg.Error, "Error", "License Initialize Failed\r\n " + "suhwoo.lic file not exists");
                return false;
            }

        }

        /// <summary>
        /// XML.InnerText to String
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string InTextToString(ref XmlNode dataNode, string value, string defaultValue = "")
        {
            try
            {
                string temp = dataNode.SelectSingleNode(value).InnerText;

                if (string.IsNullOrEmpty(temp))
                    return defaultValue;
                else
                    return temp;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// XML.InnerText to Inteager
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private int InTextToInteager(ref XmlNode dataNode, string value, int defaultValue = 0)
        {
            try
            {
                string temp = dataNode.SelectSingleNode(value).InnerText;

                if (string.IsNullOrEmpty(temp))
                    return defaultValue;
                else
                {
                    int result;
                    if (int.TryParse(temp, out result))
                        return result;
                    else
                        return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// XML.InnerText to Float
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private float InTextToFloat(ref XmlNode dataNode, string value, float defaultValue = 0f)
        {
            try
            {
                string temp = dataNode.SelectSingleNode(value).InnerText;

                if (string.IsNullOrEmpty(temp))
                    return defaultValue;
                else
                {
                    float result;
                    if (float.TryParse(temp, out result))
                        return result;
                    else
                        return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// XML.InnerText to Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private double InTextToDouble(ref XmlNode dataNode, string value, double defaultValue = 0d)
        {
            try
            {
                string temp = dataNode.SelectSingleNode(value).InnerText;

                if (string.IsNullOrEmpty(temp))
                    return defaultValue;
                else
                {
                    double result;
                    if (double.TryParse(temp, out result))
                        return result;
                    else
                        return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// XML.InnerText to Bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private bool InTextToBool(ref XmlNode dataNode, string value, bool defaultValue = false)
        {
            try
            {
                string temp = dataNode.SelectSingleNode(value).InnerText;

                if (string.IsNullOrEmpty(temp))
                    return defaultValue;
                else
                {
                    if ("Y".Equals(temp.Trim()))
                        return true;
                    if ("N".Equals(temp.Trim()))
                        return false;
                    else
                        return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// XML.InnerText BoolType to Inteager Bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private int InTextBoolToInteager(ref XmlNode dataNode, string value, int defaultValue = 0)
        {
            try
            {
                string temp = dataNode.SelectSingleNode(value).InnerText;

                if (string.IsNullOrEmpty(temp))
                    return defaultValue;
                else
                {
                    if ("Y".Equals(temp.Trim()))
                        return 1;
                    if ("N".Equals(temp.Trim()))
                        return 0;
                    else
                        return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool DecodeString(string orgStr, out string outStr)
        {
            return SuhwooUtils.DecryptAES256(orgStr, out outStr);
        }

        private bool EncodeString(string orgStr, out string outStr)
        {
            return SuhwooUtils.EncryptAES256(orgStr, out outStr);
        }

        /*
        private void SaveLicFile(string path, string contents)
        {
            try
            {
                //백업
                File.Copy(path, path + DateTime.Now.ToString("_yyyymmdd_HHmm"));

                //저장
                File.WriteAllText(path, EncodeString(contents));
            }
            catch (Exception err)
            {
                CMsg.Show(eMsg.Error, "Error", "License save Failed\r\n " + err.Message);
            }
        }
        */

        public void SetOption()
        {
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr 1호기,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Wait        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse         ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .NotMeasureDf; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Keyence     ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .Use         ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Step         ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat    ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .Use         ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.Use         ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .Use         ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse      ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse      ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse      ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .Use         ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse      ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .NotUse      ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Udp         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ;
                WtrDelay     = 3000                         ;
                IsReSkip     = true                         ;
                IsReJudge    = true                         ;
                IsTblWater   = true                         ;
                IsBfSeq      = true                         ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.Use      ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = true                         ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = true;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = true                    ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.Qorvo)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Keyence     ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Jog         ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .Rs232       ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .NotUse      ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.NotUse      ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .NotUse      ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse      ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .Use         ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .Use         ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .NotUse      ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse      ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .NotUse      ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ; 
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if (CData.CurCompany == ECompany.Qorvo_DZ)
            {
                CurEqu = eEquType.Pikcer; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType = eBcrKeyIn.Stop; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean = eOnpClean.NotUse; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver = eDfserver.NotUse; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf = eDfServerType.MeasureDf; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType = eDfProbe.Hanse; //20190910 ksg df Probe Type
                SecsUse = eSecsGem.NotUse; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean = eOffPadCleanType.LRType; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType = eMeasureType.Step; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr = eManualBcr.NotUse; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType = eSpindleType.EtherCat; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset = eAutoToolOffset.Use; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO = eUseToolSetterIO.NotUse; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl.NotUse; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog = eReGrdLog.NotUse; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos = eTblSetPos.NotUse; //Table Grd Manual Set Pos
                AutoWarmUp = eAutoWarmUp.NotUse; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock = eManualDoorLock.Use; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff = ePcwAutoOff.Use; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak = eChkGrdLeak.NotUse; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak = eChkDryLeak.NotUse; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset = eZUpOffset.NotUse; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz.NotUse; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip.NotUse; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean = eTableClean.NotUse; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid = eOflRFID.NotUse; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid = eOnlRFID.NotUse; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff = ePumpAutoOff.NotUse; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro = eBcrProtocol.Old; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package = ePkg.Strip; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer = eDryer.Rotate; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface.Ini; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay = 3000;
                WtrDelay = 3000;
                IsReSkip = true;
                IsReJudge = true;
                IsTblWater = false;
                IsBfSeq = true;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = true; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
                //200619 pjh : Bottom Bubble Air
                IsBtmAir = false; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr = false; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.SkyWorks)
            {
                CurEqu       = eEquType        .Nomal       ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Wait        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .Use         ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Hanse       ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Step        ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat    ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .Use         ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.NotUse      ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .Use         ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .NotUse      ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse      ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .Use         ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse      ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .NotUse      ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse      ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .NotUse      ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .Use         ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .Use         ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .Use         ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ; 
                WtrDelay     = 5000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = true;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = true                         ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.JSCK)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Keyence     ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.UDType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Jog         ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .Rs232       ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .NotUse      ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.Use         ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .Use         ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .Use         ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse      ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse      ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .Use         ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .Use         ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .Use         ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ;
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.SCK)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Keyence     ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .Use         ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.UDType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Jog         ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .Use         ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat    ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .NotUse      ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.Use         ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .Use         ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .Use         ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse      ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse      ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .Use         ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .Use         ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK,SKC+               - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK                    - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .New         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ; 
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용 
            }
            else if(CData.CurCompany == ECompany.ASE_K12)
            {
                CurEqu = eEquType.Pikcer; //Nomal : 스카이웍스, Ase Kr 1호기,   Picker :  그외 사이트
                KeyInType = eBcrKeyIn.Stop; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean = eOnpClean.NotUse; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver = eDfserver.NotUse; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf = eDfServerType.NotMeasureDf; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType = eDfProbe.Keyence; //20190910 ksg df Probe Type
                SecsUse = eSecsGem.NotUse; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean = eOffPadCleanType.LRType; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType = eMeasureType.Step; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr = eManualBcr.NotUse; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType = eSpindleType.EtherCat; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset = eAutoToolOffset.Use; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO = eUseToolSetterIO.NotUse; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl.NotUse; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog = eReGrdLog.Use; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos = eTblSetPos.NotUse; //Table Grd Manual Set Pos
                AutoWarmUp = eAutoWarmUp.NotUse; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock = eManualDoorLock.NotUse; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff = ePcwAutoOff.NotUse; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak = eChkGrdLeak.NotUse; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak = eChkDryLeak.NotUse; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset = eZUpOffset.NotUse; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz.NotUse; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip.NotUse; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean = eTableClean.Use; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid = eOflRFID.NotUse; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid = eOnlRFID.NotUse; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff = ePumpAutoOff.NotUse; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro = eBcrProtocol.Old; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package = ePkg.Strip; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer = eDryer.Rotate; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface.Ini; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay = 3000;
                WtrDelay = 3000;
                IsReSkip = true;
                IsReJudge = true;
                IsTblWater = true;
                IsBfSeq = true;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.Use; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = true;
                //200619 pjh : Bottom Bubble Air
                IsBtmAir = false; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr = false; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.SST)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Hanse       ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Jog         ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat    ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .NotUse      ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.NotUse      ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .NotUse      ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse      ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .Use         ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .Use         ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .NotUse      ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse      ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ; 
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.USI)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Hanse       ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Jog         ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat    ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .NotUse      ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.NotUse      ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .NotUse      ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse      ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse      ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse      ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .NotUse      ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse      ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ; 
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.Suhwoo)
            {
                CurEqu       = eEquType        .Nomal       ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .Use         ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Keyence     ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Step        ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .Rs232       ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .Use         ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.NotUse      ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .Use         ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .NotUse      ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse      ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse      ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .Use         ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .NotUse      ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse      ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .Use         ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Ini         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ; 
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.SPIL)
            {
                CurEqu       = eEquType        .Pikcer     ; //OnLoader Picker Y-axis 유무 Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop       ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean   = eOnpClean        .NotUse     ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse     ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf  ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Keyence    ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .Use     ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.LRType     ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Step       ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse     ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat   ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .Use        ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.Use        ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse     ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .NotUse     ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .Use        ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .NotUse     ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse     ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse     ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .NotUse     ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .NotUse     ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .NotUse        ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse     ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse     ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use        ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .Use     ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트 200121 ksg :
                OnlRfid      = eOnlRFID        .Use     ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트 200121 ksg :
                PumpOff      = ePumpAutoOff    .Use        ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .New        ; //20200318 jhc : //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Unit       ; //Pkg Unit 형태                           : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Knife      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Udp        ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                        ; 
                WtrDelay     = 3000                        ;
                IsReSkip     = true                       ;
                IsReJudge    = true                       ;
                IsTblWater   = false                       ;
                IsBfSeq      = false                       ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse  ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                       ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = true;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                                                              //SPIL사(2000U)에 대해서는 아직 본 기능 구현되지 않아 현재는 false로만 유지해야 함
                IsChkDI = false;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
            else if(CData.CurCompany == ECompany.JCET)
            {
                CurEqu       = eEquType        .Pikcer      ; //Nomal : 스카이웍스, Ase Kr,   Picker :  그외 사이트
                KeyInType    = eBcrKeyIn       .Stop        ; //BCR KeyIn Type  Bcr Reading Fail 시 Error 발생 : Stop, Bcr Reading Fail 시 Wait 발생 : Wait 
                eOnpClean    = eOnpClean       .NotUse      ; //Use : 스카이웍스 Not Use : 그 외 //ONP 클리너 사용 유무
                eDfserver    = eDfserver       .NotUse      ; //Use : ASE KR, ASE K26, Not Use : 그 외  //20190703 ghk_dfserver
                MeasureDf    = eDfServerType   .MeasureDf   ; //DF Server 사용시 DF사용 안함 측정 : ASE 코리아, //DF 서버 사용시 DF 측정 사용 그외 사이트 //20191029 ghk_dfserver_notuse_df
                eDfType      = eDfProbe        .Hanse       ; //20190910 ksg df Probe Type
                SecsUse      = eSecsGem        .NotUse      ; //Secs Gem 사용 유무 Use : Jsck, Not use : 기타
                PadClean     = eOffPadCleanType.UDType      ; //OFP Clean Type, UDType : JSCP, LRType : 그 외 사이트 //20190926 ghk_ofppadclean
                MeasureType  = eMeasureType    .Step        ; //Wheel Tip Measure Type, Step : 스카이웍스, Jog : 그 외 사이트
                ManualBcr    = eManualBcr      .NotUse      ; //Table Bcr Read, Use : JSCK, Not Use : 그 외 사이트  //20191010 ghk_manual_bcr
                SplType      = eSpindleType    .EtherCat    ; //Spindle 구동 타입, Rs232 : 1 ~ 5호기, Ethercat : 6 ~ 이후 쭉
                AutoOffset   = eAutoToolOffset .NotUse      ; //Wheel 높이 자동 계산, Use : 스카이웍스, Not Use: 그 외 사이트  //20191028 ghk_auto_tool_offset
                UseIO        = eUseToolSetterIO.Use         ; //Tool Setter IO 사용 여부, Use : 그 외 사이트, Not Use : K26, 스카이웍스 //20191025 ksg :
                LightControl = eLightControl   .NotUse      ; //Light Contorol 사용여부, Use : 스카이웍스, Not Use : 기타 //20191106 ghk_lightcontrol
                ReGrdLog     = eReGrdLog       .Use         ; //ReGrd Log 사용 여부, Use : JSCK, Not Use : 그외 사이트 
                TblSetPos    = eTblSetPos      .NotUse      ; //Table Grd Manual Set Pos
                AutoWarmUp   = eAutoWarmUp     .Use         ; //AutoWarmUp 사용 유무, Use : JSCK, Not Use : 그외 사이트
                ManualLock   = eManualDoorLock .NotUse      ; //Manual Door Lock 사용 유무, Use : Sky Works, Qorvo,  SST Not Use : 그외 사이트
                PcwAutoOff   = ePcwAutoOff     .NotUse      ; //Pcw Auto Off 사용 유무, Use : Qorvo,  SST Not Use : 그외 사이트
                ChkGrdLeak   = eChkGrdLeak     .Use         ; //Grd Leak Sensor 사용 유무                      : Use : Ase Kr, JSCK           - Not Use   : 그외 사이트
                ChkDryLeak   = eChkDryLeak     .Use         ; //Dry Leak Sensor 사용 유무                      : Use : JSCK                   - Not Use   : 그외 사이트
                ZUpOffset    = eZUpOffset      .Use         ; //ReGrd 할때 Z Axis Up Offset                    : Use : 그외 사이트             - Not Use   : Sky Works, Ase Kr
                //20191206 ghk_lotend_placemgz
                LotEndPlcMgz = eLotEndPlaceMgz .NotUse      ; //강제로 LotEnd 할때 Mgz Palce 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                //20191211 ghk_display_strip
                DisplayStrip = eDisplayStrip   .NotUse      ; //시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부 : Use : Sky Works    - Not Use : 그외 사이트
                //
                TableClean   = eTableClean     .Use         ; //Table Clean 사용 여부                          : Use : 그 외 사이트            - Not Use : Qorvo
                //200120 ksg :
                OflRfid      = eOflRFID        .NotUse      ; //Ofloader RFID 사용 여부                        : Use : SCK(SKC+포함)          - Not Use : 그 외 사이트
                OnlRfid      = eOnlRFID        .NotUse      ; //Onloader RFID 사용 여부                        : Use : SCK(SKC+제외)          - Not Use : 그 외 사이트
                PumpOff      = ePumpAutoOff    .NotUse      ; //Pump Auto Off 사용 여부                        : Use : 스카이웍스,Suhwoo       - Not Use : 그 외 사이트 200121 ksg :
                BcrPro       = eBcrProtocol    .Old         ; //Protocol 방식(Table 측정 여부)                  : Old : 그 외 사이트           - New     : SKC+, SKC   //200131 ksg :
                Package      = ePkg            .Strip       ; //Pkg Unit 형태                                  : One_Strip_Mold : 그 외 사이트 - Unit_Pkg_Mold : Hisilicon //200228
                Dryer        = eDryer          .Rotate      ; //Dryer 기구 Model                               : Rotate_Model : 그 외 사이트   - Knife_Model : Hisilicon //200228
                BcrInterface = eBcrInterface   .Udp         ; //BCR Interface Type                           : Ini : COGNEX,                 - Udp : 한솔루션
                EjtDelay     = 3000                         ;
                WtrDelay     = 3000                         ;
                IsReSkip     = false                        ;
                IsReJudge    = false                        ;
                IsTblWater   = false                        ;
                IsBfSeq      = false                        ;
                //20200415 jhc : Picker Idle Time 개선
                PickerImprove = ePickerTimeImprove.NotUse   ; //Picker Idle Time 개선 기능 사용 여부          : Use : ASE-KR          - Not Use : 그외 사이트
                //20200515 myk : Wheel Cleaner Water
                IsWhlCleaner = false                        ; //Dressing, Grinding 시 Wheel Cleaner 워터 온
                IsPause = false;
				//200619 pjh : Bottom Bubble Air
                IsBtmAir     = false                        ; // Bottom Bubble Air Sensor 사용 여부 : true : 사용 false : 미사용
                IsFakeAf = false;
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                Use18PointMeasure = false                   ; //Lot Open 후 첫 n개의 Strip 측정 포인트 별도 설정 : true=사용(ASE-KR only) / false=미사용
                IsChkDI = true;
                // 200730 myk : Manual Grinding Barcode 입력
                IsGrdBcr     = false                        ; //Manual Grinding 시 Barcode 입력 기능 사용 여부   true : 사용 (Skyworks only)  false : 미사용
            }
        }
    }
    
}
