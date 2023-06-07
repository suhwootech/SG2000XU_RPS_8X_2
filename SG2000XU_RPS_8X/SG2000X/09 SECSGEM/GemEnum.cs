using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECSGEM
{
    #region JSCK
    static class JSCK
    {
        /// <summary>
        /// Communication state
        /// </summary>
        public enum eComm
        {
            Disable = 0,
            NotCommunicating,
            Communicating
        }

        /// <summary>
        /// Connection state
        /// </summary>
        public enum eConn
        {
            Disconnected = 0,
            Connected = 1
        }

        /// <summary>
        /// Control State
        /// 999011    Previous_Control_State
        /// 999012    Current_Control_State
        /// </summary>
        public enum eCont
        {
            Unknown,
            Offline = 1,
            HostOffline = 2,
            AttempToOnline = 3,
            Local = 4,
            Remote = 5
        }

        /// <summary>
        /// Process State
        /// 999013    Previous_Process_Status
        /// 999014    Current_Process_Status
        /// </summary>
        public enum eProcStatus 
        {
            //Idle = 0,
            //Processing = 1,
            //Done = 2,
            //Abort = 3,
            //Down = 4,
            //Pause = 5,
            //Init = 6,
            //Setup = 7,
            //Unknown = 8

            Idle = 0,
            Run = 1,
            Ready = 2,
            Done = 2,
            Down = 4
        }

        public enum eMGZPort
        {
            GOOD = 1,
            NG = 2,
            LOADER = 3
        }

        public enum eTableLoc
        {
            N1 = 1,
            N2 = 2
        }

        // 2021.08.09 SungTae Start : [추가]
        // SECS/GEM에서 LOT, MGZ, Loader MGZ의 Verify Check
        public enum eChkVerify
        {
            Error    = -1,  // Error
            Run      = 1,   // 진행
            Complete = 2,   // 완료 시
        }

        // SECS/GEM에서 Strip Verify Check
        public enum eChkStripVerify
        {
            StripError = -3,   // Strip Verify Error
            LotError,          // LOT Verify Error
            ReqError,          // Host Request Error
            Default,           // Default Value
            InitVerify,        // Init Strip Verify
            LotSuccess,        // LOT Verify Success
            StripSuccess,      // Strip Verify Success
        }
        // 2021.08.09 SungTae End

        // 2022.04.01 SungTae Start : [추가] 코드 확인 시 용이하도록 Define
        /// <summary>
        /// HCACK (Host Command parameter ACKnowledge code)
        /// 0 : Acknowledged. Command has been performed.
        /// 1 : Command does not exist.
        /// 2 : Cannot perform now.
        /// 3 : At least one parameter is invalid.
        /// 4 : Acknowledged. Command will be performed.
        /// 5 : Rejected. Already in desired condition.
        /// </summary>
        public enum eHCACK
        {
            AckOK = 0,
            DoesNotExist,
            CannotPerform,
            Invalid,
            Ack,
            Rejected,
        }
        // 2022.04.01 SungTae End

        public enum eCarrierStatus
        {
            Idle = 0,
            Read = 1,
            Validate = 2,
            Start = 3,
            End = 4,
            Reject = 5
        }

        /// <summary>
        /// message id
        /// </summary>
        public enum eMID
        {
            /// <summary>
            /// message ID - SVID
            /// </summary>
            SVID = 1001,
            /// <summary>
            /// message ID - CEID
            /// </summary>
            CEID = 1002,
            /// <summary>
            /// message ID - ECID
            /// </summary>
            ECID = 1003,
            /// <summary>
            /// message ID - alarm occur
            /// </summary>
            AlarmOccur = 1004,
            /// <summary>
            /// message ID - alarm reset
            /// </summary>
            AlarmReset = 1005,
            /// <summary>
            /// message ID - LOG
            /// </summary>
            LOG = 1006
        }

        /// <summary>
        /// JSCK    ECID
        /// </summary>
        public enum eECID
        {
            /// <summary>
            /// U2
            /// 
            /// > 2000
            /// </summary>
            TCP_Port = 1,
            /// <summary>
            /// U1
            /// 
            /// >= 0
            /// </summary>
            Device_ID = 2,
            /// <summary>
            /// U1
            /// second
            /// 
            /// </summary>
            T3_Timeout = 3,
            /// <summary>
            /// U1
            /// second
            /// 
            /// </summary>
            T5_Timeout = 5,
            /// <summary>
            /// U1
            /// second
            /// 
            /// </summary>
            T6_Timeout = 6,
            /// <summary>
            /// U1
            /// second
            /// 
            /// </summary>
            T7_Timeout = 7,
            /// <summary>
            /// U1
            /// second
            /// 
            /// </summary>
            T8_Timeout = 8,
            /// <summary>
            /// U1
            /// second
            /// circuit assurance (in second)
            /// </summary>
            Link_Test_Interval = 9,
            /// <summary>
            /// U2
            /// second
            /// 
            /// </summary>
            Communication_Establish_Timeout = 10,
        }

        /// <summary>
        /// JSCK    CEID
        /// </summary>
        public enum eCEID
        {
            Control_State_Change        = 999001,
            Process_State_Chagne        = 999002,
            Remote_Paused               = 999003, // 지원 않음
            Remote_Resume               = 999004, // 지원 않음
            Machine_Start               = 999011, // add V1.0.16a , 20201010
            Machine_ReStart             = 999012, // add V1.0.16b , 20201109, jhLee, for SPIL

            Recipe_Created              = 999021,
            Recipe_Loaded               = 999022,
            Recipe_Parameter_Changed    = 999023,
            Recipe_Deleted              = 999024,
            LOT_Verify_Request          = 999100,
            LOT_Verified                = 999101,
            LOT_Started                 = 999102,
            LOT_Ended                   = 999103,
            LOT_Changed                 = 999104,
            Carrier_Verify_Request      = 999200,
            Carrier_Verified            = 999201,
            Carrier_Started             = 999202,
            Carrier_Ended               = 999203,
            Carrier_Unloaded            = 999204,
            Carrier_Summary_Uploaded    = 999205,
            Carrier_Grind_Finished      = 999307, // add V1.0.16b , 20201031 ASE-KR
            Magazine_Verify_Request     = 999400,
            Magazine_Verified           = 999401,
            Magazine_Started            = 999402,
            Magazine_Ended              = 999403,
            Material_Verify_Request     = 999601,
            Material_Verified           = 999602,
            Material_Changed            = 999603,
            Tool_Verify_Request         = 999701,
            Tool_Verified               = 999702,
            Tool_Changed                = 999703
        }
        /// <summary>
        /// StandardB    CEID
        /// </summary>
        public enum eBCEID
        {
            #region Standard
            Control_State_Change = 1,
            Equipment_State_Change = 2,
            Communication_State_Change = 3,

            Start_Allowed_Reqeust = 10,
            Machine_Start = 11,

            LOT_Started = 93,
            Lot_Ended = 94,

            Carrier_Verify_Request = 104,
            Carrier_Verified = 105,
            Carrier_Loaded = 107,
            Carrier_Started = 108,

            Carrier_Summary_Uploaded = 111,
            Carrier_Grind_Finished = 113,
            Carrier_Ended = 115,
            Carrier_Unloaded = 116,
            Carrier_Scrap = 119,

            Magazine_Verify_Request = 131,
            Magazine_Verified = 132,
            Magazine_Started = 133,
            Magazine_Ended = 134,

            Wheel_Verify_Request = 141,
            Wheel_Verified = 142,
            Wheel_Changed = 143,

            Dresser_Verify_Request = 151,
            Dresser_Verified = 152,
            Dresser_Changed = 153,



            Loader_Normal_MGZ_Load_Request = 181,
            Loader_Normal_MGZ_Load_Complete = 182,
            Loader_Empty_MGZ_Place = 183,
            Loader_Empty_MGZ_Unload_Request = 184,
            Loader_Empty_MGZ_Unload_Complete = 185,

            Unloader_Empty_MGZ_Load_Request = 191,
            Unloader_Empty_MGZ_Load_Complete = 192,
            Unloader_Normal_MGZ_Place = 193,
            Unloader_Normal_MGZ_Unload_Request = 194,
            Unloader_Normal_MGZ_Unload_Complete = 195,

            Operator_LogIn = 281,
            Operator_LogOut = 282,

            Equipment_Constants_Change = 301,

            Recipe_Loaded = 601,
            Recipe_Created = 602,
            Recipe_Deleted = 603,

            Recipe_Parameters_modified = 605,
            Recipe_Selected_Failed = 606,

            #endregion

        }
        /// <summary>
        /// JSCK    SVID
        /// </summary>
        public enum eSVID
        {
            /// <summary>
            /// A[14]
            /// CurrentClock
            /// yyyyMMddHHmmss
            /// </summary>
            Clock = 2012,
            /// <summary>
            /// U1
            /// 
            /// Equipment ID
            /// </summary>
            EQ_ID = 999001,
            /// <summary>
            /// U1
            /// Control State
            /// 1 : Offline
            /// 2 : Host offline
            /// 3 : Attemp to online
            /// 4 : Local
            /// 5 : Remote
            /// </summary>
            Previous_Control_State = 999011,
            /// <summary>
            /// U1
            /// Control State
            /// 1 : Offline
            /// 2 : Host offline
            /// 3 : Attemp to online
            /// 4 : Local
            /// 5 : Remote
            /// </summary>
            Current_Control_State = 999012,
            /// <summary>
            /// U1
            /// Process State
            /// 0 : Idle
            /// 1 : Processing
            /// 2 : Done
            /// 3 : Abort
            /// 4 : Down
            /// 5 : Pause
            /// 6 : Init
            /// 7 : Setup
            /// 8 : Unknown
            /// </summary>
            Previous_Process_Status = 999013,
            /// <summary>
            /// U1
            /// Process State
            /// 0 : Idle
            /// 1 : Processing
            /// 2 : Done
            /// 3 : Abort
            /// 4 : Down
            /// 5 : Pause
            /// 6 : Init
            /// 7 : Setup
            /// 8 : Unknown
            /// </summary>
            Current_Process_Status = 999014,
            /// <summary>
            /// A
            /// PPID
            /// Current PPID
            /// </summary>
            Current_PPID = 999021,
            /// <summary>
            /// A
            /// PPID
            /// Changed PPID
            /// </summary>
            PPID = 999022,
            /// <summary>
            /// A
            /// Current LOT ID
            /// </summary>
            Current_LOT_ID      = 999101,
            /// <summary>
            /// A
            /// LOT ID
            /// </summary>
            LOT_ID = 999102,
            /// <summary>
            /// A[14]
            /// yyyyMMddHHmmss
            /// </summary>
            LOT_Started_Time    = 999103,
            /// <summary>
            /// A[14]
            /// yyyyMMddHHmmss
            /// </summary>
            LOT_Ended_Time      = 999104,
            /// <summary>
            /// L[A]
            /// </summary>
            Total_Carrier_List  = 999105,
            /// <summary>
            /// L[A]
            /// </summary>
            Proceed_Carrier_List = 999106,
            /// <summary>
            /// U4
            /// </summary>
            Total_Unit_Count    = 999107,
            /// <summary>
            /// U4
            /// </summary>
            Proceed_Unit_Count  = 999108,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Read   = 999111,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Validate = 999112,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Started = 999113,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Complete = 999114,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Reject = 999115,
            /// <summary>
            /// SVID, 0 : Step, 1: Normal
            /// </summary>
            Grind__Mode = 999116,
            /// <summary>
            /// SVID는 아니지만, Temp Read 전달
            /// </summary>
            Carrier_List_Temp_Read = 999199,
            /// <summary>
            /// A
            /// Carrier ID Value
            /// </summary>
            Carrier_ID = 999201,
            /// <summary>
            /// A[14]
            /// yyyyMMddHHmmss
            /// </summary>
            Carrier_Started_Time = 999202,
            /// <summary>
            /// A[14]
            /// yyyyMMddHHmmss
            /// </summary>
            Carrier_Ended_Time = 999203,
            /// <summary>
            /// A
            /// ex) 111111011110...
            /// </summary>
            Carrier_Map = 999204,
            /// <summary>
            /// L<>
            /// before work, after work
            /// <L[3]
            ///     <L[3]    //before work
            ///         <F4>    //min value
            ///         <F4>    //max value
            ///         <F4>    //ave value
            ///     >    
            ///     <L[3]    //end work
            ///         <F4>    //min value
            ///         <F4>    //max value
            ///         <F4>    //ave value
            ///     >
            /// >    
            /// </summary>
            Carrier_Left_Work_Point = 999205,
            /// <summary>
            /// Left Value
            /// L<>
            /// n is point count for before work, n is point count for after work
            /// <L[3]
            ///     <L[n]    //n is point count for before work
            ///         <F4>    //point 1
            ///         <F4>    
            ///         <F4>    
            ///         ... to n
            ///     >    
            ///     <L[3]    //n is point count for end work
            ///         <F4>    //point 1
            ///         <F4>    
            ///         <F4>
            ///         ... to n
            ///     >
            /// >    
            /// </summary>
            Carrier_Left_Raw_Point = 999206,
            /// <summary>
            /// Left Value
            /// A
            /// 
            /// Reading MGZ ID
            /// </summary>
            Carrier_Right_Work_Point = 999207,
            /// <summary>
            /// Right Value
            /// L<>
            /// n is point count for before work, n is point count for after work
            /// <L[3]
            ///     <L[n]    //n is point count for before work
            ///         <F4>    //point 1
            ///         <F4>    
            ///         <F4>    
            ///         ... to n
            ///     >    
            ///     <L[3]    //n is point count for end work
            ///         <F4>    //point 1
            ///         <F4>    
            ///         <F4>
            ///         ... to n
            ///     >
            /// >    
            /// </summary>
            Carrier_Right_Raw_Point = 999208,
            // 2020 01 30 skpark svid 추가 999210 ~ 999222
            /// <summary>
            /// U1
            /// 
            /// DF 진행 후 AVR 값           
            /// </summary>
            DF_DATA_PCB_AVR = 999210,
            /// <summary>
            /// A
            /// DF 진행 후 MAX 값           
            /// DF_DATA_PCB_MAX
            /// </summary>
            DF_DATA_PCB_MAX = 999211,
            /// <summary>
            /// A
            /// Recipe에 설정 되어 있는 Top또는 Btm 값
            /// 0: Top, 1: Btm, 2: TopD, 3=BtmS  // 2021.07.30 lhs BtmS Comment 추가
            /// </summary>
            PCB_TOPBTM_USE_MODE = 999212,
            /// <summary>
            /// A
            /// 영상으로 Strip 검사 시 Top 또는 Btm  값 (사용 미 확정)
            /// 0: Top, 1: Btm
            /// </summary>
            PCB_ORIENT_VALUE = 999213,

            /// <summary>
            /// A
            /// Left Wheel Thickness
            /// </summary>
            WHEEL_L_THICKNESS = 999216,
            /// <summary>
            /// A
            /// Right Wheel Thickness
            /// </summary>
            WHEEL_R_THICKNESS = 999217,
            /// <summary>
            /// A
            /// Left Dress Thickness
            /// </summary>
            DRESS_L_THICKNESS = 999218,
            /// <summary>
            /// A
            /// Right Dress Thickness
            /// </summary>
            DRESS_R_THICKNESS = 999219,
            /// <summary>
            /// A
            /// Carrier Current Read_ID Value
            /// </summary>
            Carrier_Current_Read_ID = 999221,
            /// <summary>
            /// A
            /// Left_Max_Spindle_Load
            /// </summary>
            Left_Max_Spindle_Load = 999226,
            /// <summary>
            /// A
            /// Right_Max_Spindle_Load
            /// </summary>
            Right_Max_Spindle_Load = 999227,
            /// <summary>
            /// A
            /// Left_Dress_After_Strip_count
            /// </summary>
            Left_Dress_After_Strip_count = 999228,
            /// <summary>
            /// A
            /// Right_Dress_After_Strip_count
            /// </summary>
            Right_Dress_After_Strip_count = 999229,

            // 2021.07.23 lhs Start : 999240~999243 추가 (SCK+ 전용)
            /// <summary>
            /// A
            /// Top Mold Thickness Avg
            /// </summary>
            TOP_THK_AVG = 999240,
            /// <summary>
            /// A
            /// Top Mold Thickness Max
            /// </summary>
            TOP_THK_MAX = 999241,
            /// <summary>
            /// A
            /// Bottom Mold Thickness Avg
            /// </summary>
            BTM_THK_AVG = 999242,
            /// <summary>
            /// A
            /// Bottom Mold Thickness Max
            /// </summary>
            BTM_THK_MAX = 999243,
            // 2021.07.23 lhs End

            /// <summary>
            /// Right Value
            /// A
            /// 
            /// Reading MGZ ID
            /// </summary>
            Magazine_ID = 999401,
            /// <summary>
            /// U1
            /// 
            /// Carrier loading slot number in MGZ
            /// </summary>
            MGZ_Slot_Number = 999402,
            /// <summary>
            /// L<>
            /// <L[25]
            ///     <
            /// >
            /// </summary>
            MGZ_Carrier_List = 999403,
            /// <summary>
            /// U1
            /// 
            /// 1 : Good MGZ
            /// 2 : NG MGZ
            /// </summary>
            MGZ_Port_Number = 999404,
            /// <summary>
            /// A
            /// </summary>
            Current_Loaded_MATL1_ID = 999601,
            /// <summary>
            /// A
            /// </summary>
            Current_Loaded_MATL2_ID = 999602,
            /// <summary>
            /// A
            /// </summary>
            MATL1_ID = 999621,
            /// <summary>
            /// A
            /// </summary>
            MATL2_ID = 999622,
            /// <summary>
            /// Material Location Number
            /// </summary>
            MATL_Loc = 999631,
            /// <summary>
            /// A
            /// 
            /// 
            /// </summary>
            Current_Loaded_Tool_1_ID = 999701,
            /// <summary>
            /// A
            /// 
            /// 
            /// </summary>
            Current_Loaded_Tool_2_ID = 999702,
            /// <summary>
            /// A
            /// 
            /// Working Wheel ID
            /// </summary>
            Wheel_L_ID = 999711,
            Wheel_R_ID = 999712,
            /// <summary>
            /// A
            /// </summary>
            Tool_1_ID = 999721,
            /// <summary>
            /// A
            /// </summary>
            Tool_2_ID = 999722,
            /// <summary>
            /// A Wheel Serial Num
            /// </summary>
            Tool_L_Serial_Num = 999723,
            Tool_R_Serial_Num = 999724,
            /// <summary>
            /// U1
            /// 
            /// Tool Location Number
            /// </summary>
            Tool_Loc = 999731,
            /// <summary>
            /// U1
            /// 
            /// Working Table Location Number
            /// </summary>
            Table_Loc = 999741,

            // 2021.05.21 SungTae Start : [추가] SPIL용 SVID List Up
            Strip_Short_Side                    = 998000,
            Strip_Long_Side                     = 998001,
            Dual_Mode                           = 998002,
            Left_Total_Thickness                = 998003,
            Left_PCB_Thickness                  = 998004,
            Left_Mold_Thickness                 = 998005,
            Right_Total_Thickness               = 998006,
            Right_PCB_Thickness                 = 998007,
            Right_Mold_Thickness                = 998008,
            Left_Actual_Cycle_Depth             = 998009,
            Left_Actual_Spindle_Speed           = 998010,
            Left_Actual_Table_Speed             = 998011,
            Right_Actual_Cycle_Depth            = 998012,
            Right_Actual_Spindle_Speed          = 998013,
            Right_Actual_Table_Speed            = 998014,
            Left_Grinding_Mode                  = 998015,
            Left_Air_Cut_Depth                  = 998016,
            Left_R1_Target                      = 998017,
            Left_R1_Cycle_Depth                 = 998018,
            Left_R1_Table_Speed                 = 998019,
            Left_R1_Spindle_Speed               = 998020,
            Left_R1_Direction                   = 998021,
            Left_R2_Target                      = 998022,
            Left_R2_Cycle_Depth                 = 998023,
            Left_R2_Table_Speed                 = 998024,
            Left_R2_Spindle_Speed               = 998025,
            Left_R2_Direction                   = 998026,
            Left_R3_Target                      = 998027,
            Left_R3_Cycle_Depth                 = 998028,
            Left_R3_Table_Speed                 = 998029,
            Left_R3_Spindle_Speed               = 998030,
            Left_R3_Direction                   = 998031,
            Left_R4_Target                      = 998032,
            Left_R4_Cycle_Depth                 = 998033,
            Left_R4_Table_Speed                 = 998034,
            Left_R4_Spindle_Speed               = 998035,
            Left_R4_Direction                   = 998036,
            Left_R5_Target                      = 998037,
            Left_R5_Cycle_Depth                 = 998038,
            Left_R5_Table_Speed                 = 998039,
            Left_R5_Spindle_Speed               = 998040,
            Left_R5_Direction                   = 998041,
            Left_R6_Target                      = 998042,
            Left_R6_Cycle_Depth                 = 998043,
            Left_R6_Table_Speed                 = 998044,
            Left_R6_Spindle_Speed               = 998045,
            Left_R6_Direction                   = 998046,
            Left_R7_Target                      = 998047,
            Left_R7_Cycle_Depth                 = 998048,
            Left_R7_Table_Speed                 = 998049,
            Left_R7_Spindle_Speed               = 998050,
            Left_R7_Direction                   = 998051,
            Left_R8_Target                      = 998052,
            Left_R8_Cycle_Depth                 = 998053,
            Left_R8_Table_Speed                 = 998054,
            Left_R8_Spindle_Speed               = 998055,
            Left_R8_Direction                   = 998056,
            Left_R9_Target                      = 998057,
            Left_R9_Cycle_Depth                 = 998058,
            Left_R9_Table_Speed                 = 998059,
            Left_R9_Spindle_Speed               = 998060,
            Left_R9_Direction                   = 998061,
            Left_R10_Target                     = 998062,
            Left_R10_Cycle_Depth                = 998063,
            Left_R10_Table_Speed                = 998064,
            Left_R10_Spindle_Speed              = 998065,
            Left_R10_Direction                  = 998066,
            Left_R11_Target                     = 998067,
            Left_R11_Cycle_Depth                = 998068,
            Left_R11_Table_Speed                = 998069,
            Left_R11_Spindle_Speed              = 998070,
            Left_R11_Direction                  = 998071,
            Left_R12_Target                     = 998072,
            Left_R12_Cycle_Depth                = 998073,
            Left_R12_Table_Speed                = 998074,
            Left_R12_Spindle_Speed              = 998075,
            Left_R12_Direction                  = 998076,
            Left_Fine_Target                    = 998077,
            Left_Fine_Cycle_Depth               = 998078,
            Left_Fine_Table_Speed               = 998079,
            Left_Fine_Spindle_Speed             = 998080,
            Left_Fine_Direction                 = 998081,
            Right_Grinding_Mode                 = 998082,
            Right_Air_Cut_Depth                 = 998083,
            Right_R1_Target                     = 998084,
            Right_R1_Cycle_Depth                = 998085,
            Right_R1_Table_Speed                = 998086,
            Right_R1_Spindle_Speed              = 998087,
            Right_R1_Direction                  = 998088,
            Right_R2_Target                     = 998089,
            Right_R2_Cycle_Depth                = 998090,
            Right_R2_Table_Speed                = 998091,
            Right_R2_Spindle_Speed              = 998092,
            Right_R2_Direction                  = 998093,
            Right_R3_Target                     = 998094,
            Right_R3_Cycle_Depth                = 998095,
            Right_R3_Table_Speed                = 998096,
            Right_R3_Spindle_Speed              = 998097,
            Right_R3_Direction                  = 998098,
            Right_R4_Target                     = 998099,
            Right_R4_Cycle_Depth                = 998100,
            Right_R4_Table_Speed                = 998101,
            Right_R4_Spindle_Speed              = 998102,
            Right_R4_Direction                  = 998103,
            Right_R5_Target                     = 998104,
            Right_R5_Cycle_Depth                = 998105,
            Right_R5_Table_Speed                = 998106,
            Right_R5_Spindle_Speed              = 998107,
            Right_R5_Direction                  = 998108,
            Right_R6_Target                     = 998109,
            Right_R6_Cycle_Depth                = 998110,
            Right_R6_Table_Speed                = 998111,
            Right_R6_Spindle_Speed              = 998112,
            Right_R6_Direction                  = 998113,
            Right_R7_Target                     = 998114,
            Right_R7_Cycle_Depth                = 998115,
            Right_R7_Table_Speed                = 998116,
            Right_R7_Spindle_Speed              = 998117,
            Right_R7_Direction                  = 998118,
            Right_R8_Target                     = 998119,
            Right_R8_Cycle_Depth                = 998120,
            Right_R8_Table_Speed                = 998121,
            Right_R8_Spindle_Speed              = 998122,
            Right_R8_Direction                  = 998123,
            Right_R9_Target                     = 998124,
            Right_R9_Cycle_Depth                = 998125,
            Right_R9_Table_Speed                = 998126,
            Right_R9_Spindle_Speed              = 998127,
            Right_R9_Direction                  = 998128,
            Right_R10_Target                    = 998129,
            Right_R10_Cycle_Depth               = 998130,
            Right_R10_Table_Speed               = 998131,
            Right_R10_Spindle_Speed             = 998132,
            Right_R10_Direction                 = 998133,
            Right_R11_Target                    = 998134,
            Right_R11_Cycle_Depth               = 998135,
            Right_R11_Table_Speed               = 998136,
            Right_R11_Spindle_Speed             = 998137,
            Right_R11_Direction                 = 998138,
            Right_R12_Target                    = 998139,
            Right_R12_Cycle_Depth               = 998140,
            Right_R12_Table_Speed               = 998141,
            Right_R12_Spindle_Speed             = 998142,
            Right_R12_Direction                 = 998143,
            Right_Fine_Target                   = 998144,
            Right_Fine_Cycle_Depth              = 998145,
            Right_Fine_Table_Speed              = 998146,
            Right_Fine_Spindle_Speed            = 998147,
            Right_Fine_Direction                = 998148,
            Left_Contact_Point_Column_Count     = 998149,
            Left_Contact_Point_Row_Count        = 998150,
            Left_Contact_Unit_Width             = 998151,
            Left_Contact_Unit_Height            = 998152,
            Left_Contact_Unit_1_Center_Y        = 998153,
            Left_Contact_Unit_2_Center_Y        = 998154,
            Left_Contact_Unit_3_Center_Y        = 998155,
            Left_Contact_Unit_4_Center_Y        = 998156,
            Left_Contact_Before_Limit           = 998157,
            Left_Contact_After_Limit            = 998158,
            Left_Contact_TTV_Limit              = 998159,
            Left_Contact_One_Point_Limit        = 998160,
            Left_Contact_One_Point_Over_Check   = 998161,
            Right_Contact_Point_Column_Count    = 998162,
            Right_Contact_Point_Row_Count       = 998163,
            Right_Contact_Unit_Width            = 998164,
            Right_Contact_Unit_Height           = 998165,
            Right_Contact_Unit_1_Center_Y       = 998166,
            Right_Contact_Unit_2_Center_Y       = 998167,
            Right_Contact_Unit_3_Center_Y       = 998168,
            Right_Contact_Unit_4_Center_Y       = 998169,
            Right_Contact_Before_Limit          = 998170,
            Right_Contact_After_Limit           = 998171,
            Right_Contact_TTV_Limit             = 998172,
            Right_Contact_One_Point_Limit       = 998173,
            Right_Contact_One_Point_Over_Check  = 998174,
            BCR_Skip                            = 998175,
            BCR_Key_In_Skip                     = 998176,
            Orientation_Skip                    = 998177,
            Dry_Knife_Count                     = 998178,
            Dry_Pusher_Fast_Velocity            = 998179,
            Dry_Pusher_Slow_Velocity            = 998180,
            Dry_Bottom_Cleaning_Picker_Count    = 998181,
            Dry_Bottom_Cleaning_Strip_Count     = 998182,
            Left_Water_Knife_Clean_Velocity     = 998183,
            Left_Water_Knife_Clean_Count        = 998184,
            Left_Air_Knife_Clean_Velocity       = 998185,
            Left_Air_Knife_Clean_Count          = 998186,
            Right_Water_Knife_Clean_Velocity    = 998187,
            Right_Water_Knife_Clean_Count       = 998188,
            Right_Air_Knife_Clean_Velocity      = 998189,
            Right_Air_Knife_Clean_Count         = 998190,
            // 2021.05.21 SungTae End
        }
        /// <summary>
        /// StandardB    SVID
        /// </summary>
        public enum eBSVID
        {
            Equipment_State = 2003,
            Previous_Equipment_State = 2004,
            Control_State = 2005,
            Clock = 2006,
            OperatorId = 2007,
            Communication_State = 2008,



            Current_PPID = 2010,
            Changed_PPID = 2012,

            Carrier_Started_Time = 2016,
            Carrier_Ended_Time = 2017,
            Stage1_CarrierID = 2018,
            Stage2_CarrierID = 2019,
            Reading_Carrier_ID = 2020,
            Load_CarrierID_1 = 2021,
            Load_CarrierID_2 = 2022,
            Load_CarrierID_3 = 2023,
            Start_CarrierID = 2024,
            Start_CarrierID_1 = 2025,
            Start_CarrierID_2 = 2026,
            Start_CarrierID_3 = 2027,

            Stage1_CarrierID_1 = 2028,
            Stage1_CarrierID_2 = 2029,
            Stage1_CarrierID_3 = 2030,
            Stage2_CarrierID_1 = 2031,
            Stage2_CarrierID_2 = 2032,
            Stage2_CarrierID_3 = 2033,

            Complete_CarrierID = 2034,
            Complete_CarrierID_1 = 2035,
            Complete_CarrierID_2 = 2036,
            Complete_CarrierID_3 = 2037,

            Carrier_Complete_Result = 2038,               // 미사용
            Carrier_Complete_Failure_List = 2039,         // 미사용

            Complete_Lot_Barcode = 2040,                  // 미사용
            Carrier_Stored_Barcode = 2041,                // 미사용
            Carrier_Stored_Tray_Barcode = 2042,           // 미사용
            Carrier_Stored_Position = 2043,               // 미사용
            Carrier_Remove_Barcode = 2044,
            Carrier_Remove_Reason_Code_Type = 2045,
            Carrier_Remove_Reason_Code = 2046,
            Cancel_Tray_Barcode = 2047,
            Cancel_Carrier_Info_1 = 2048,
            Cancel_Carrier_Info_2 = 2049,
            Cancel_Carrier_Info_3 = 2050,

            /// <summary>
            /// L[A]
            /// </summary>
            Total_Carrier_List = 2061,
            /// <summary>
            /// L[A]
            /// </summary>
            Proceed_Carrier_List = 2062,
            /// <summary>
            /// U4
            /// </summary>
            Total_Unit_Count = 2063,
            /// <summary>
            /// U4
            /// </summary>
            Proceed_Unit_Count = 2064,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Read = 2065,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Validate = 2066,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Started = 2067,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Complete = 2068,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Reject = 2069,
            /// <summary>
            /// L<>
            /// <L
            ///     <A "Carrier_ID1>
            ///     ...
            ///     <A "Carrier_IDn>
            /// >
            /// </summary>
            Carrier_List_Unloaded = 2070,
            Carrier_Summary_Data = 2071,

            MGZ_Port_Number = 2101,
            Reading_Magazine_ID = 2102,
            Demand_Type = 2103,
            Slot_Map = 2104,

            LD_Magazine_ID = 2111,
            LD_Slot_ID = 2112,
            GOOD_Magazine_ID = 2113,
            GOOD_Slot_ID = 2114,
            NG_Magazine_ID = 2115,
            NG_Slot_ID = 2116,

            LOT_ID = 2121,
            Current_Lot_ID = 2122,
            /// <summary>
            /// L[A]
            /// </summary>
            Current_Lot_List = 2123,
            Lot_Started_Time = 2124,
            Lot_Ended_Time = 2125,

            PCB_TOPBTM_USE_MODE = 2141,
            Grind_Dual_Mode = 2142,
            Sg1_Grinding_Mode = 2143,
            Sg2_Grinding_Mode = 2144,
            Grinding_Reference = 2145,

            DF_DATA_PCB_AVR = 2151,
            DF_DATA_PCB_MAX = 2152,
            Left_Carrier_PCB_EMC_Work_Point = 2153,
            Left_Carrier_EMC_Raw_Point = 2154,
            Right_Carrier_PCB_EMC_Work_Point = 2155,
            Right_Carrier_EMC_Raw_Point = 2156,

            Table_Location = 2161,
            Carrier_Sg1_Work_Point = 2162,
            Carrier_Sg1_Raw_Point = 2163,
            Carrier_Sg2_Work_Point = 2164,
            Carrier_Sg2_Raw_Point = 2165,

            UnitPerHour = 2201,
            MeanTimeBetweenAssistance = 2202,
            MeanTimeBetweenFailure = 2203,

            MachineDowntime = 2211,
            MachineUptime = 2212,

            Loaded_Sg1_WheelID = 4001,
            Loaded_Sg2_WheelID = 4002,
            Wheel_Sg1_ID = 4003,
            Wheel_Sg2_ID = 4004,
            Wheel_FileName_1 = 4005,
            Wheel_FileName_2 = 4006,
            Wheel_Loc = 4007,

            Loaded_Sg1_DressID = 4011,
            Loaded_Sg2_DressID = 4012,
            Dress_Sg1_ID = 4013,
            Dress_Sg2_ID = 4014,
            Dress_Loc = 4015,

            Wheel_Sg1_Thickness = 4021,
            Wheel_Sg2_Thickness = 4022,
            Dress_Sg1_Thickness = 4023,
            Dress_Sg2_Thickness = 4024,

            Left_Actual_Table_Speed = 5001,
            Left_Actual_Spindle_Speed = 5002,
            Left_Current_Spindle_Load = 5003,
            Left_Max_Spindle_Load = 5004,

            Right_Actual_Table_Speed = 5011,
            Right_Actual_Spindle_Speed = 5012,
            Right_Current_Spindle_Load = 5013,
            Right_Max_Spindle_Load = 5014,

        }
    }
    #endregion JSCK
}