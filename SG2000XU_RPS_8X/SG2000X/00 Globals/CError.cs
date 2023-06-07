using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SG2000X
{
    // XXnnnnnnnn    Module
    // nnXXnnnnnn    Cycle
    // nnnnXXnnnn    Cycle Step
    // nnnnnnXXnn    Equipment
    // nnnnnnnnXX    Error Number


    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Module    XXnnnnnnnn
    /// 00nnnnnnnn System
    /// 01nnnnnnnn On Loader
    /// 02nnnnnnnn In Rail
    /// 03nnnnnnnn On Loader Picker
    /// 04nnnnnnnn Grind Zone Left
    /// 05nnnnnnnn Grind Zone Right
    /// 06nnnnnnnn Off Loader Picker
    /// 07nnnnnnnn Dry Zone
    /// 08nnnnnnnn QC Vision Part - Option
    /// 09nnnnnnnn Off Loader
    /// 10nnnnnnnn In Rail Vision
    /// 11nnnnnnnn In Rail 2D Bar Code Reader
    /// 12nnnnnnnn QC Vision
    /// </summary>
    public enum eErr_Module
    {
        SYS = 00,
        ONL,
        INR,
        ONP,
        GRL,
        GRR,
        OFP,
        DRY,
        QCP,
        OFL,
        IRV,
        IRB,
        QCV,
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle On Loader    nnXXnnnnnn
    /// Servo - Servo On/Off 시 에러 
    /// Home - Home 동작 시 에러
    /// Wait - 대기 위치 이동 시 에러
    /// Pick - 매거진 집으러 가는 동작 시 에러
    /// Push - 자재를 인레일에 밀어넣는 동작 시 에러
    /// Place - 매거진 내려 놓는 동작 시 에러
    /// </summary>
    public enum eErr_CyONL
    {
        Servo = 00,
        Home,    // 온로더 홈
        Wait,    //매거진 없는 상태 웨이트   Cyl_Wait
        Pick,   //매거진 집는거  Cyl_Pick
        Push,    //자재 투입    Cyl_PusherWork : Z축 높이 확인 함수 필요(피치 이동)
        Place,    //매거진 버리는거    Cyl_Place
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle In Rail    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyINR
    {
        Servo = 00,
        Wait,    //웨이트    Cyl_Wait
        Home,
        Bcr,    //바코드 리딩 위치 이동
        Align,    //얼라인 위치    Cyl_Align        
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle On Loader Picker    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyONP
    {
        Servo = 00,
        Home,
        Wait,
        PickRail,    //인레일 자재 Pick    Cyl_Pick
        PickTbL,    //왼쪽 테이블 자재 Pick    Cyl_PickTbL
        PlaceTbL,    //왼쪽 테이블에 자재 놓기    Cyl_PlaceTbL
        PlaceTbR,    //오른쪽 테이블에 자재 놓기    Cyl_PlaceTbR

    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle Grind Zone Left    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyGRL
    {
        Servo = 0,
        Home,
        Wait,
        MeaStrip,
        MeaStripOne,
        MeaWhl,
        MeaDrs,
        Grind,
        Dressing,
        GrdTable
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle Grind Zone Right    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyGRR
    {
        Servo = 0,
        Home,
        Wait,
        MeaStrip,
        MeaStripOne,
        MeaWhl,
        MeaDrs,
        Grind,
        Dressing,
        GrdTable
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle Off Loader Picker    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyOFP
    {
        Servo = 00,
        Home,
        Wait,
        PicClean,
        PickL,
        PickR,
        PlaceTbR, //오른쪽 테이블에 자재 놓기 : Cyl_PlaceTbR //20200415 jhc : Picker Idle Time 개선
        StpClean,
        Place
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle Dry Zone    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyDRY
    {
        Servo = 00,
        Home,
        Wait,
        ChkSafty,
        DryWork,
        OutStrip
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Cycle Off Loader    nnXXnnnnnn
    /// 
    /// </summary>
    public enum eErr_CyOFL
    {
        Servo = 00,
        Home,
        Wait,
        BtmPick,
        BtmPlace,
        BtmRcv,
        TopPick,
        TopPlace,
        TopRcv
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment    nnnnnnXXnn
    /// nnnnnn00nn    System    만약에 100개 넘으면 이부분은 전체 숫자 사용으로 변경 처리 가능
    /// nnnnnn01nn    Spindle_Left
    /// nnnnnn02nn    Spindle_Right
    /// nnnnnn03nn    WaterPump_Left
    /// nnnnnn04nn    WaterPump_Right
    /// nnnnnn05nn    Probe_Left
    /// nnnnnn06nn    Probe_Right
    /// nnnnnn07nn    Air
    /// nnnnnn08nn    Water
    /// nnnnnn09nn    WMX
    /// nnnnnn10nn    Input_Read
    /// nnnnnn11nn    Output_Read
    /// nnnnnn12nn    Output_Write
    /// 
    /// nnnnnn50nn    Axis_00
    /// nnnnnn51nn    Axis_01
    /// nnnnnn52nn    Axis_02
    /// nnnnnn53nn    Axis_03
    /// nnnnnn54nn    Axis_04
    /// nnnnnn55nn    Axis_05
    /// nnnnnn56nn    Axis_06
    /// nnnnnn57nn    Axis_07
    /// nnnnnn58nn    Axis_08
    /// nnnnnn59nn    Axis_09
    /// nnnnnn60nn    Axis_10
    /// nnnnnn61nn    Axis_11
    /// nnnnnn62nn    Axis_12
    /// nnnnnn63nn    Axis_13
    /// nnnnnn64nn    Axis_14
    /// nnnnnn65nn    Axis_15
    /// nnnnnn66nn    Axis_16
    /// nnnnnn67nn    Axis_17
    /// nnnnnn68nn    Axis_18
    /// nnnnnn69nn    Axis_19
    /// nnnnnn70nn    Axis_20
    /// nnnnnn71nn    Axis_21
    /// nnnnnn72nn    Axis_22
    /// nnnnnn73nn    Axis_23
    /// nnnnnn74nn    Axis_24
    /// nnnnnn75nn    Axis_25
    /// nnnnnn76nn    Axis_26
    /// nnnnnn77nn    Axis_27
    /// nnnnnn78nn    Axis_28
    /// nnnnnn79nn    Axis_29
    /// </summary>
    public enum eErr_Equ
    {
        Sys = 0,
        Spl_L,
        Spl_R,
        WPump_L,
        WPump_R,
        Prb_L,
        Prb_R,
        Air,
        Water,
        WMX,
        X_Read,
        Y_Read,
        Y_Write,

        Ax00_InrX = 50, // Axis_00 = 50, 
        Ax01_InrY,      //Axis_01,  
        Ax02_OnpX,      //Axis_02,
        Ax03_OnpZ,      //Axis_03,
        Ax04_GrlX,      //Axis_04,
        Ax05_GrlY,      //Axis_05,
        Ax06_GrlZ,      //Axis_06,
        Ax07_GrrX,      //Axis_07,
        Ax08_GrrY,      //Axis_08,
        Ax09_GrrZ,      //Axis_09,
        Ax10_OfpX,      //Axis_10,
        Ax11_OfPZ,      //Axis_11,
        Ax12_DryX,      //Axis_12,
        Ax13_DryZ,      //Axis_13,
        Ax14_DryR,      //Axis_14,
        Ax15_OnlX,      //Axis_15,
        Ax16_OnlY,      //Axis_16,
        Ax17_OnlZ,      //Axis_17,
        Ax18_OflX,     //Axis_18,
        Ax19_OflY,     //Axis_19,
        Ax20_OflZ,     //Axis_20,
        Ax21,
        Ax22,
        Ax23,
        Ax24,
        Ax25,
        Ax26,
        Ax27,
        Ax28,
        Ax29,
        Ax30,
        Ax31
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - System    nnnnnnnnXX
    /// </summary>
    public enum eErr_Sys
    {
        None = 00,
        Wheel_File_Left_Save,
        Wheel_File_Left_Load,
        Wheel_File_Right_Save,
        Wheel_File_Right_Load,
        Device_File_Save,
        Device_File_Load
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Spindle    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqSpl
    {
        RS232C_Port_Open,
        RS232C_Port_Close,
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment WaterPump    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqWPump
    {
        Flow_Low,
        Temperature_High,
        Overload,
        TableVacuum,
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Probe    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqPrb
    {
        RS232C_Port_Open,
        RS232C_Port_Close,
        Config_File_Save,
        Config_File_Load
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Air    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqAir
    {
        Main_Pneumatic_Low,
        INR_Align_Forward,
        INR_Align_Backward,
        INR_Gripper_ClampOn,
        INR_Gripper_ClampOff,
        INR_LiftUp,
        INR_LiftDown,

        ONP_Rotate0,
        ONP_Rotate90,

        GRL_TCleanerDown,
        GRR_TCleanerDown,

        OFP_Rotate0,
        OFP_Rotate90,

        DRY_BtmAirBlower_StandbyPosition,
        DRY_BtmAirBlower_TargetPosition,

        ONL_PusherF,
        ONL_PusherB,
        ONL_ClampOn,
        ONL_ClampOff,

        OFL_TopClampUp,
        OFL_TopClampDown,
        OFL_TopClampOn,
        OFL_TopClampOff,
        OFL_BtmClampUp,
        OFL_BtmClampDown,
        OFL_BtmClampOn,
        OFL_BtmClampOff,
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Water    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqWater
    {
        GRL_FlowTable,
        GRL_FlowTopCleaner,
        GRL_FlowPCW,
        GRL_FlowGrind,
        GRL_FlowBtmBubble,

        GRR_FlowTable,
        GRR_FlowTopCleaner,
        GRR_FlowPCW,
        GRR_FlowGrind,
        GRR_FlowBtmBubble,

        DRY_FlowCleaner,
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment WMX    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqWMX
    {
        Create_Device,
        Close_Device,
        Start_Communication,
        Stop_Communication,
        Save_XML_File,
        Load_XML_File,
        Set_Axis_Parameter,
        Get_Axis_Parameter,
        Set_System_Parameter,
        Get_System_Parameter
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Input Read    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqX_Read
    {
        Main_ControllerOff,
        Main_EMO,
        Main_EMO_FL,
        Main_EMO_FR,
        Main_EMO_RL,
        Main_EMO_RR,

        INR_InDetect,
        INR_BtmDetect,
        INR_GripperDetect,

        Main_DoorOpen_F,
        Main_DoorOpen_R,
        Main_CoverOpen_F,
        Main_CoverOpen_R,

        Main_Temp_PCW,
        Main_Temp_GrindWater,

        GRL_WheelZig,
        GRR_WheelZig,

        DRY_Overload,
        DRY_OutDetect,
        DRY_Safty1,
        DRY_Safty2,

        ION_Alarm_L,
        ION_Alarm_R,

        ONL_EMO_F,
        ONL_EMO_R,
        ONL_DoorOpen_F,
        ONL_DoorOpen_L,
        ONL_DoorOpen_R,
        ONL_LightCurtain,
        ONL_TC_MgzDetect,
        ONL_TC_MgzFullDetect,
        ONL_BC_MgzDetect,
        ONL_Overload,
        ONL_ClampDetect1,
        ONL_ClampDetect2,

        OFL_EMO_F,
        OFL_EMO_R,
        OFL_DoorOpen_F,
        OFL_DoorOpen_Rt,
        OFL_DoorOpen_R,
        OFL_LightCurtain,
        OFL_TC_MgzDetect,
        OFL_TC_MgzFullDetect,
        OFL_MC_MgzDetect,
        OFL_BC_MgzDetect,
        OFL_BC_MgzFullDetect,
        OFL_TClampDetect1,
        OFL_TClampDetect2,
        OFL_BClampDetect1,
        OFL_BClampDetect2,
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Output Read    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqY_Read
    {
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Output Write    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqY_Write
    {
    }

    /// <summary>
    /// josh    항목이 추가되면, 항목 추가 및 주석 기입 필수
    /// Error 명세 - Equipment Axis    nnnnnnnnXX
    /// </summary>
    public enum eErr_EqAx
    {
        NotUse = 01,    //Axis Not Use
        Busy,    //Busy
        Alarm,    //Alarm
        SOn,    //Servo On
        NotHD,    //Not Home Done
        NotInp,    //Not Inposition
        DifOP,    //Difference OP Status
        Timeout,    //Timeout
        NOT,    //Sensing NOT
        POT,    //Sensing POT
        NotHome,    //Can not find Home
        NotStop,    //Can not Stop
        NotQStop,    //Can not QStop
        LimitMaxPosition,
        LimitMinPosition
    }

    public struct tError
    {
        /// <summary>
        /// 에러 발생 여부
        /// false : Not Error
        /// true : On Error
        /// </summary>
        public bool bOn;
        /// <summary>
        /// 에러 코드
        /// </summary>
        public string sCode;
        /// <summary>
        /// 에러 명
        /// </summary>
        public string sName;
        /// <summary>
        /// 에러 메세지 : 에러 설명
        /// </summary>
        public string sMsg;
        /// <summary>
        /// 에러 조치 방법
        /// </summary>
        public string sSol;
        /// <summary>
        /// 에러 이미지 경로
        /// </summary>
        public string sPath;
    }

    public static class CError
    {
        /// <summary>
        /// 현재 선택된 에러 인덱스
        /// </summary>
        public static int m_iOnErr = 0;
        /// <summary>
        /// Unknown 에러 구조체
        /// </summary>
        public static tError m_tUnk = new tError();
        /// <summary>
        /// 전체 에러 배열
        /// </summary>
        public static tError[] m_aErr;
        

        static CError()
        {
            //메모리 영역 확보
            m_tUnk = new tError();
            m_tUnk.bOn = true;
            m_tUnk.sCode = "-0000000001";
            m_tUnk.sName = "Unknown Error";
            m_tUnk.sMsg = "Unknown Error";
            m_tUnk.sSol = "";
            m_tUnk.sPath = "";
            m_aErr = new tError[GV.MAX_ERR];

            //파일로부터 데이터 획득

            //중요 데이터 초기화
            Init();

            //에러 내용 등록 파일 로드


        }

        private static void Init()
        {
            //데이터 초기화
            for (int i = 0; i < GV.MAX_ERR; i++)
            {
                m_aErr[i].bOn = false;
            }
        }
    }
}
