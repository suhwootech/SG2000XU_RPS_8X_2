#region IO
public enum eX
{
    X00 = 0,
    /// <summary>
    /// 
    /// </summary>
    SYS_MC = 0,
    X01 = 1,
    /// <summary>
    /// 메인 공압 센서
    /// </summary>
    SYS_Pneumatic = 1,
    X02 = 2,
    /// <summary>
    /// Emergency Controller
    /// </summary>
    SYS_EMGController = 2,
    X03 = 3,
    /// <summary>
    /// 장비 전면(왼쪽) Emergency Button
    /// </summary>
    SYS_EMGFront_L = 3,
    X04 = 4,
    /// <summary>
    /// 장비 전면(오른쪽) Emergency Button
    /// </summary>
    SYS_EMGFront_R = 4,
    X05 = 5,
    /// <summary>
    /// 장비 후면(왼쪽) Emergency Button
    /// </summary>
    SYS_EMGRear_L = 5,
    X06 = 6,
    /// <summary>
    /// 장비 후면(오른쪽) Emergency Button
    /// </summary>
    SYS_EMGRear_R = 6,
    X07 = 7,
    X08 = 8,
    /// <summary>
    /// Start Button (장비 전면)
    /// </summary>
    SYS_BtnStart = 8,
    X09 = 9,
    /// <summary>
    /// Stop Button (장비 전면)
    /// </summary>
    SYS_BtnStop = 9,
    X0A = 10,
    /// <summary>
    /// Reset Button (장비 전면)
    /// </summary>
    SYS_BtnReset = 10,
    X0B = 11,
    /// <summary>
    /// Gripper Clamp On-Rear (U 전용)
    /// </summary>
    INR_GripperClampOn_Rear = 11,
    X0C = 12,
    /// <summary>
    /// Gripper Clamp Off-Rear (U 전용)
    /// </summary>
    INR_GripperClampOff_Rear = 12,
    X0D = 13,
    X0E = 14,
    /// <summary>
    /// Left Tool Setter
    /// </summary>
    SYS_ToolsetterL = 14,
    X0F = 15,
    /// <summary>
    /// Right Tool Setter
    /// </summary>
    SYS_ToolsetterR = 15,
    X10 = 16,
    /// <summary>
    /// Inrail 자재 감지 Sensor (Onloader Pusher가 Inrail로 자재 투입 시 감지하는 센서)
    /// </summary>
    INR_StripInDetect = 16,
    X11 = 17,
    /// <summary>
    /// Inrail 자재 바닥면 감지 Sensor (Inrail에 자재가 안착됐는지 확인하기 위해 바닥면을 감지하는 센서)
    /// </summary>
    INR_StripBtmDetect = 17,
    X12 = 18,
    /// <summary>
    /// Inrail 자재 고정(Forward) Sensor (Inrail에 있는 자재를 고정할 때, 이를 감지하는 센서)
    /// </summary>
    INR_GuideForward = 18,
    X13 = 19,
    /// <summary>
    /// Inrail 자재 고정 해제(Backward) Sensor (Inrail에서 고정한 자재를 고정 해제할 때, 이를 감지하는 센서)
    /// </summary>
    INR_GuideBackward = 19,
    X14 = 20,
    /// <summary>
    /// Inrail Gripper 스트립 감지 Sensor
    /// </summary>
    INR_GripperDetect = 20,
    X15 = 21,
    /// <summary>
    /// Inrail Gripper Clamp on Sensor
    /// </summary>
    INR_GripperClampOn = 21,
    X16 = 22,
    /// <summary>
    /// Inrail Gripper clamp off Sensor
    /// </summary>
    INR_GripperClampOff = 22,
    X17 = 23,
    /// <summary>
    /// Unit #1 Detect  (U 전용)
    /// </summary>
    INR_Unit_1_Detect = 23,
    /// <summary>
    /// 4U Carrier Detect (4U 전용)
    /// //210910 syc : 2004U
    /// </summary>
    INR_Carrier_Detect = 23, //210831 syc : 2004U IV2
    X18 = 24,
    /// <summary>
    /// Inrail Lift Up Sensor
    /// </summary>
    INR_LiftUp = 24,
    X19 = 25,
    /// <summary>
    /// Inrail Lift Down Sensor
    /// </summary>
    INR_LiftDn = 25,
    X1A = 26,
    //20190611 ghk_onpclean
    /// <summary>
    /// Inrail OnLoaderPicker Cleaner Left
    /// </summary>
    INR_OnpCleaner_L = 26,
    X1B = 27,
    /// <summary>
    /// Inrail OnLoaderPicker Cleaner Right
    /// </summary>
    INR_OnpCleaner_R = 27,
    X1C = 28,
    /// <summary>
    /// Unit #2 Detect  (U 전용)
    /// </summary>
    INR_Unit_2_Detect = 28,
    X1D = 29,
    /// <summary>
    /// Unit 캐리어에 안착 시 틀어짐 확인 (Only U)
    /// </summary>
    INR_Unit_Tilt = 29,
    X1E = 30,
    /// <summary>
    /// Inrail 프로브 업
    /// </summary>
    INR_ProbeUp = 30,
    X1F = 31,
    /// <summary>
    /// Inrail 프로브 다운
    /// </summary>
    INR_ProbeDn = 31,
    X20 = 32,
    /// <summary>
    /// Onloader Picker 회전각도 0도 Sensor (Table 하고 같은 방향)
    /// </summary>
    ONP_Rotate0 = 32,
    X21 = 33,
    /// <summary>
    /// Onloader Picker 회전각도 90도 Sensor (Table 모양에서 90도 회전한 방향)
    /// </summary>
    ONP_Rotate90 = 33,
    X22 = 34,
    /// <summary>
    /// Onloader Picker 진공흡입 Sensor (onloader Picker가 자재를 흡착했을 때 감지되는 센서)
    /// </summary>
    ONP_Vacuum = 34,
    X23 = 35,
    /// <summary>
    /// Unit #3 Detect (U 전용)
    /// </summary>
    INR_Unit_3_Detect = 35,
    X24 = 36,
    /// <summary>
    /// Unit #4 Detect (U 전용)
    /// </summary>
    INR_Unit_4_Detect = 36,
    X25 = 37,
    X26 = 38,
    /// <summary>
    /// Onloader Picker Loadcell Sensor (Onloader Picker 하부에 일정 압력이 가해지면 감지되는 센서) 
    /// </summary>
    ONP_LoadCell = 38,
    X27 = 39,
    X28 = 40,
    /// <summary>
    /// Grind 전면 Door Sensor (장비 전면에 있는 문이 닫힐 때 감지되는 센서)
    /// </summary>
    GRD_DoorFront = 40,
    X29 = 41,
    /// <summary>
    /// Grind 후면 Door Sensor (장비 후면에 있는 문이 닫힐 때 감지되는 센서)
    /// </summary>
    GRD_DoorRear = 41,
    X2A = 42,
    /// <summary>
    /// Grind 전면 Cover (장비가 러닝중일 때 비산방지를 위해 전면에 있는 커버)
    /// </summary>
    GRD_CoverFront = 42,
    X2B = 43,
    /// <summary>
    /// Grind 후면 Cover (장비가 러닝중일 때 비산방지를 위해 후면에 있는 커버)
    /// </summary>
    GRD_CoverRear = 43,

    X2C = 44,
    X2D = 45,

    X2E = 46,
    /// <summary>
    /// Grind 밑면 누수 감지 센서
    /// </summary>
    GRL_Leak = 46, //ksg : 추가
    X2F = 47,
    /// <summary>
    /// Grind 밑면 누수 감지 센서
    /// </summary>
    GRR_Leak = 47, //ksg : 추가
    X30 = 48,
    X31 = 49,
    /// <summary>
    /// Grind Left Table 진공흡입 Sensor (Left Table이 자재를 흡착했을 때 감지되는 센서)
    /// </summary>
    GRDL_TbVaccum = 49,
    /// <summary>
    /// 4U Unit Vacuum (4U 전용)
    /// //2100901 syc : 2004U
    /// </summary>
    GRDL_Unit_Vacuum_4U = 49, // eX
    X32 = 50,
    /// <summary>
    /// Grind Left Table Flow Sensor (Left Table에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDL_TbFlow = 50,
    X33 = 51,
    /// <summary>
    /// Grind Left Top Cleaner Down Sensor (Left Top Cleaner가 Down일 때 감지되는 센서)
    /// </summary>
    GRDL_TopClnDn = 51,
    X34 = 52,
    X35 = 53,
    /// <summary>
    /// Grind Left Top Cleaner Flow Sensor (Left Top Cleaner에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDL_TopClnFlow = 53,
    X36 = 54,
    /// <summary>
    /// IV2 All OK 종합판정 (N.O.) (화상판별센서) //2021.04.02->2022.06.28 lhs
    /// </summary>
    OFFP_IV2_AllOK = 54,
    X37 = 55,
    /// <summary>
    /// IV2 Busy (N.O.) (화상판별센서) //2021.04.02->2022.06.28 lhs
    /// </summary>
    OFFP_IV2_Busy = 55,
    X38 = 56,
    /// <summary>
    /// IV2 Error (N.O.) (화상판별센서) //2021.04.02->2022.06.28 lhs
    /// </summary>
    OFFP_IV2_Error = 56,

    X39 = 57,
    X3A = 58,
    /// <summary>
    /// 4U Carrier Detect (4U 전용)
    /// //210910 syc : 2004U
    /// </summary>
    GRDL_Carrier_Vacuum_4U = 58,
    X3B = 59,
    /// <summary>
    /// Grind Left Probe Up Sensor (Grind Left 프로브 센서가 Up일 때 감지되는 센서)
    /// </summary>
    GRDL_ProbeAMP = 59,
    X3C = 60,
    /// <summary>
    /// Grind Left Wheel Zig Sensor (Left Spindle에 Wheel Jig가 삽입되있을 때 감지되는 센서)
    /// </summary>
    GRDL_WheelZig = 60,
    X3D = 61,
    /// <summary>
    /// Unit #1 Vacuum Left Table  (U 전용)
    /// </summary>
    GRDL_Unit_1_Vacuum = 61,
    X3E = 62,
    /// <summary>
    /// Grind Left Spindle PCW Sensor (Left Spindle에 PCW가 투입될 때 감지되는 센서)
    /// </summary>
    GRDL_SplPCW = 62,
    X3F = 63,
    /// <summary>
    /// Grind Left Spindle PCW 온도 초과 Sensor (Left Spindle에 투입되는 PCW 온도가 유저가 설정한 기준치를 넘었을 때 감지되는 센서)
    /// </summary>
    GRDL_SplPCWTemp = 63,
    X40 = 64,
    /// <summary>
    /// Grind Left Spindle Water Sensor (Left Spindle에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDL_SplWater = 64,
    X41 = 65,
    /// <summary>
    /// Grind Left Spindle Water 온도 초과 Sensor (Left Spindle에서 나오는 물 온도가 유저가 설정한 기준치를 넘었을 때 감지되는 센서)
    /// </summary>
    GRDL_SplWaterTemp = 65,
    X42 = 66,
    /// <summary>
    /// Grind Left Spindle Bottom Water Sensor (Grind Left Bottom에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDL_SplBtmWater = 66,
    X43 = 67,
    /// <summary>
    /// Unit #2 Vacuum LEft Table (U 전용)
    /// </summary>
    GRDL_Unit_2_Vacuum = 67,
    /// <summary>
    /// Grind Left Spindle Bottom Ait Sensor (Grind Left Bottom에서 Air가 나올 때 감지되는 센서)
    /// </summary>
    GRDL_SplBtmAir = 67,
    X44 = 68,
    X45 = 69,
    X46 = 70,
    /// <summary>
    /// Unit #3 Vacuum Left Table  (U 전용)
    /// </summary>
    GRDL_Unit_3_Vacuum = 70,
    X47 = 71,
    /// <summary>
    /// Unit #4 Vacuum Left Table (U 전용)
    /// </summary>
    GRDL_Unit_4_Vacuum = 71,
    X48 = 72,
    X49 = 73,
    /// <summary>
    /// Grind Right Table 진공흡입 Sensor (Right Table이 자재를 흡착했을 때 감지되는 센서)
    /// </summary>
    GRDR_TbVaccum = 73,
    /// <summary>
    /// Unit Vacuum (4U 전용)
    /// //2100901 syc : 2004U
    /// </summary>
    GRDR_Unit_Vacuum_4U = 73, // eX
    X4A = 74,
    /// <summary>
    /// Grind Right Table Flow Sensor (Right Table에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDR_TbFlow = 74,
    X4B = 75,
    /// <summary>
    /// Grind Right Top Cleaner Down Sensor (Right Top Cleaner가 다운일 때 감지되는 센서)
    /// </summary>
    GRDR_TopClnDn = 75,
    X4C = 76,
    X4D = 77,
    /// <summary>
    /// Grind Right Top Cleaner Flow Sensor (Right Top Cleaner에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDR_TopClnFlow = 77,

    // 2022.06.23 lhs Start : Dryer Cylinder Clamp Type 
    X4E = 78,
    /// <summary>
    /// [A] [DRY] Sensor - Front_Strip Clamp Cylinder Clamp
    /// </summary>
    DRY_Front_Clamp = 78,
    X4F = 79,
    /// <summary>
    /// [A] [DRY]Sensor - Front_Strip Clamp Cylinder Unclamp
    /// </summary>
    DRY_Front_Unclamp = 79,
    X50 = 80,
    /// <summary>
    /// [A] [DRY]Sensor - Rear_Strip Clamp Cylinder Clamp
    /// </summary>
    DRY_Rear_Clamp = 80,
    X51 = 81,
    /// <summary>
    /// [A] [DRY]Sensor - Rear_Strip Clamp Cylinder Unclamp 
    /// </summary>
    DRY_Rear_Unclamp = 81,
    // 2022.06.23 lhs End : Dryer Cylinder Clamp Type 

    X52 = 82,
    X53 = 83,
    /// <summary>
    /// Grind Right Probe Up Sensor (Grind Right 프로브 센서가 Up일 때 감지되는 센서)
    /// </summary>
    GRDR_ProbeAMP = 83,
    X54 = 84,
    /// <summary>
    /// Grind Right Wheel Jig 감지 Sensor (Right Spindle에 Wheel Jig가 삽입되있을 때 감지되는 센서)
    /// </summary>
    GRDR_WheelZig = 84,
    X55 = 85,
    X56 = 86,
    /// <summary>
    /// Grind Right Spindle PCW Sensor (Right Spindle에 PCW가 투입될 때 감지되는 센서)
    /// </summary>
    GRDR_SplPCW = 86,
    X57 = 87,
    /// <summary>
    /// 4U Carrier Detect (4U 전용)
    /// //210910 syc : 2004U
    /// </summary>
    GRDR_Carrier_Vacuum_4U = 87,
    X58 = 88,
    /// <summary>
    /// Grind Right Spindle Water Sensor (Right Spindle에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDR_SplWater = 88,
    X59 = 89,
    /// <summary>
    /// Unit #1 Vacuum Right Table  (U 전용)
    /// </summary>
    GRDR_Unit_1_Vacuum = 89,
    X5A = 90,
    /// <summary>
    /// Grind Right Spindle Bottom Water Sensor (Right Spindle Bottom에서 물이 나올 때 감지되는 센서)
    /// </summary>
    GRDR_SplBtmWater = 90,
    X5B = 91,
    /// <summary>
    /// Unit #2 Vacuum Right Table  (U 전용)
    /// </summary>
    GRDR_Unit_2_Vacuum = 91,
    /// <summary>
    /// Grind Right Spindle Bottom Air Sensor (Right Spindle Bottom에서 Air가 나올 때 감지되는 센서)
    /// </summary>
    GRDR_SplBtmAir = 91,
    X5C = 92,
    X5D = 93,
    X5E = 94,
    /// <summary>
    /// Unit #3 Vacuum Right Table  (U 전용)
    /// </summary>
    GRDR_Unit_3_Vacuum = 94,
    X5F = 95,
    /// <summary>
    /// Unit #4 Vacuum Right Table  (U 전용)
    /// </summary>
    GRDR_Unit_4_Vacuum = 95,
    X60 = 96,
    /// <summary>
    /// Offloader Picker 회전각도 0도 Sensor (Table과 같은 방향)
    /// </summary>
    OFFP_Rotate0 = 96,
    X61 = 97,
    /// <summary>
    /// Offloader Picker 회전각도 90도 Sensor (Table 모양에서 90도 회전한 방향)
    /// </summary>
    OFFP_Rotate90 = 97,
    X62 = 98,
    /// <summary>
    /// Offloader Picker 진공흡입 Sensor (offloader Picker가 자재를 흡착했을 때 감지되는 센서)
    /// </summary>
    OFFP_Vacuum = 98,

    X63 = 99,
    /// <summary>
    /// Dryer Unit #1 Detect
    /// </summary>
    DRY_Unit1_Detect = 99,
    /// <summary>
    /// 4U OFP Carrier Clamp Open 감지
    /// </summary>
    OFFP_Carrier_Clamp_Open = 99,

    X64 = 100,
    /// <summary>
    /// Dryer Unit #1 Detect
    /// </summary>
    DRY_Unit2_Detect = 100,
    /// <summary>
    /// 4U OFP Carrier Clamp Close 감지
    /// </summary>
    OFFP_Carrier_Clamp_Close = 100,

    X65 = 101,
    /// <summary>
    /// Dryer Unit #1 Detect
    /// </summary>
    DRY_Unit3_Detect = 101,
    /// <summary>
    /// 4U OFFP의 Carrier 감지
    /// </summary>
    OFFP_Picker_Carrier_Detect = 101,

    X66 = 102,
    /// <summary>
    /// Offloader Picker Loadcell Sensor (Offloader Picker 하부에 일정 압력이 가해지면 감지되는 센서)
    /// </summary>
    OFFP_LoadCell = 102,
    /// <summary>
    /// [DRY] Sensor - Bottom Cleaner Forward 
    /// Sponge -> Nozzle 개조, 노즐 이동용 실린더 위치 확인  // 2022.03.02 lhs
    /// </summary>
    DRY_ClnBtmNzlForward = 102,

    X67 = 103,
    /// <summary>
    /// Dryer Unit #1 Detect
    /// </summary>
    DRY_Unit4_Detect = 103,
    /// <summary>
    /// [DRY] Sensor - Bottom Cleaner Backward 
    /// Sponge -> Nozzle 개조, 노즐 이동용 실린더 위치 확인  // 2022.03.02 lhs
    /// </summary>
    DRY_ClnBtmNzlBackward = 103,

    X68 = 104,
    /// <summary>
    /// Dry Pusher 과부하 Sensor (Dry Pusher가 자재를 밀 때 일정 압력이 가해지면 감지되는 센서)
    /// </summary>
    DRY_PusherOverload = 104,
    X69 = 105,
    /// <summary>
    /// Dry 자재 감지 Sensor (Dry Pusher가 자재를 Offloader MGZ으로 밀 때 감지하는 센서)
    /// </summary>
    DRY_StripOutDetect = 105,

    X6A = 106,
    /// <summary>
    /// Dry Level Safety Sensor 1 (Dry Zone에 자재 안착 시 자재가 제대로 안착됐는지 확인하는 센서)
    /// </summary>
    DRY_LevelSafety1 = 106,
    /// <summary>
    /// 4U Dry시 Unit Cover 감지
    /// </summary>
    DRY_Carrier_Cover_Close = 106,

    X6B = 107,
    /// <summary>
    /// Dry Level Safety Sensor 2 (Dry Zone에 자재 안착 시 자재가 제대로 안착됐는지 확인하는 센서)
    /// </summary>
    DRY_LevelSafety2 = 107,
    X6C = 108,
    /// <summary>
    /// Dry Bottom Standby Position Sensor (Dry Bottom Cleaner 대기 포지션, 수정 필요)
    /// </summary>
    DRY_BtmStandbyPos = 108,
    /// <summary>
    /// Dry cover open sensor (only U)
    /// </summary>
    DRY_CoverOpen = 108,
    X6D = 109,
    /// <summary>
    /// Dry Bottom Target Position Sensor (Dry Bottom Cleaner 타겟 포지션, 수정 필요)
    /// </summary>
    DRY_BtmTargetPos = 109,
    /// <summary>
    /// Dry cover close sensor (only U)
    /// </summary>
    DRY_CoverClose = 109,
    X6E = 110,
    /// <summary>
    /// Dry Bottom 밑면 누수 감지 센서
    /// </summary>
    DRY_Leak = 110,
    X6F = 111,
    /// <summary>
    /// Dry Bottom Cleaner Flow Sensor (Bottom Cleaner에서 물이 나올 때 감지되는 센서)
    /// </summary>
    DRY_ClnBtmFlow = 111,
    X70 = 112,
    /// <summary>
    /// Left Pump Run Sensor (왼쪽 펌프 가동할 때 감지되는 센서)
    /// </summary>
    PUMPL_Run = 112,
    X71 = 113,
    /// <summary>
    /// Left Pump Flow Low Sensor (왼쪽 펌프 수압이 기준치보다 낮을 때 감지되는 센서)
    /// </summary>
    PUMPL_FlowLow = 113,
    X72 = 114,
    /// <summary>
    /// Left Pump 온도 초과 Sensor (왼쪽 펌프 수온이 기준치보다 높을 때 감지되는 센서)
    /// </summary>
    PUMPL_TempHigh = 114,
    X73 = 115,
    /// <summary>
    /// Left Pump 과부하 Sensor (왼쪽 펌프가 과부하 상태일 때 감지되는 센서)
    /// </summary>
    PUMPL_OverLoad = 115,
    X74 = 116,
    X75 = 117,
    X76 = 118,
    /// <summary>
    /// Right Pump Run Sensor (오른쪽 펌프 가동할 때 감지되는 센서)
    /// </summary>
    PUMPR_Run = 118,
    X77 = 119,
    /// <summary>
    /// Right Pump Flow Low Sensor (오른쪽 펌프 수압이 기준치보다 낮을 때 감지되는 센서)
    /// </summary>
    PUMPR_FlowLow = 119,
    X78 = 120,
    /// <summary>
    /// Right Pump 온도 초과 Sensor (오른쪽 펌프 수온이 기준치보다 높을 때 감지되는 센서)
    /// </summary>
    PUMPR_TempHigh = 120,
    X79 = 121,
    /// <summary>
    /// Right Pump 과부하 Sensor (오른쪽 펌프가 과부하 상태일 때 감지되는 센서)
    /// </summary>
    PUMPR_OverLoad = 121,
    X7A = 122,
    /// <summary>
    /// [On-Picker] 진공해제 신호 (= [A][ONP]Sensor - Ejector Picker)  
    /// </summary>
    ONP_VacuumFree = 122,
    X7B = 123,
    /// <summary>
    /// [Off-Picker] 진공해제 신호 (= [A][OFP]Sensor - Ejector Picker) 
    /// </summary>
    OFFP_VacuumFree = 123,
    X7C = 124, IOZL_Alarm = 124,
    X7D = 125, IOZR_Alarm = 125,
    X7E = 126,
    X7F = 127,
    X80 = 128, TNK_147C = 128,
    X81 = 129, TNK_2580 = 129,
    X82 = 130, TNK_3695 = 130,
    X83 = 131, TNK_123 = 131,
    X84 = 132, TNK_456 = 132,
    X85 = 133, TNK_789 = 133,
    X86 = 134, TNK_C0S = 134,
    
    X87 = 135,
    /// <summary>
    /// Left Pump 2 Run Sensor (왼쪽 펌프 가동할 때 감지되는 센서)
    /// </summary>
    ADD_PUMPL_Run = 135,        // 2020.10.22 JSKim
    X88 = 136,
    /// <summary>
    /// Left Pump 2 Flow Low Sensor (왼쪽 펌프 수압이 기준치보다 낮을 때 감지되는 센서)
    /// </summary>
    ADD_PUMPL_FlowLow = 136,    // 2020.10.22 JSKim
    X89 = 137,
    /// <summary>
    /// Left Pump 2 온도 초과 Sensor (왼쪽 펌프 수온이 기준치보다 높을 때 감지되는 센서)
    /// </summary>
    ADD_PUMPL_TempHigh = 137,   // 2020.10.22 JSKim
    X8A = 138,
    /// <summary>
    /// Left Pump 2 과부하 Sensor (왼쪽 펌프가 과부하 상태일 때 감지되는 센서)
    /// </summary>
    ADD_PUMPL_OverLoad = 138,   // 2020.10.22 JSKim
    X8B = 139,
    X8C = 140,
    /// <summary>
    /// Right Pump 2 Run Sensor (오른쪽 펌프 가동할 때 감지되는 센서)
    /// </summary>
    ADD_PUMPR_Run = 140,        // 2020.10.22 JSKim
    X8D = 141,
    /// <summary>
    /// Right Pump 2 Flow Low Sensor (오른쪽 펌프 수압이 기준치보다 낮을 때 감지되는 센서)
    /// </summary>
    ADD_PUMPR_FlowLow = 141,    // 2020.10.22 JSKim
    X8E = 142,
    /// <summary>
    /// Right Pump 2 온도 초과 Sensor (오른쪽 펌프 수온이 기준치보다 높을 때 감지되는 센서)
    /// </summary>
    ADD_PUMPR_TempHigh = 142,   // 2020.10.22 JSKim
    X8F = 143,
    /// <summary>
    /// Right Pump 과부하 Sensor (오른쪽 펌프가 과부하 상태일 때 감지되는 센서)
    /// </summary>
    ADD_PUMPR_OverLoad = 143,   // 2020.10.22 JSKim

    X90 = 144,
    /// <summary>
    /// Onloader Belt Run Button (Onloader 왼쪽면)
    /// </summary>
    ONL_BtnBeltRun = 144,
    X91 = 145,
    X92 = 146,
    /// <summary>
    /// Onloader 전면 Emergency Button
    /// </summary>
    ONL_EMGFront = 146,
    X93 = 147,
    /// <summary>
    /// Onloader 후면 Emergency Button
    /// </summary>
    ONL_EMGRear = 147,
    X94 = 148,
    /// <summary>
    /// Onloader 전면 Door Sensor (문이 닫힐 때 감지되는 센서)
    /// </summary>
    ONL_DoorFront = 148,
    X95 = 149,
    /// <summary>
    /// Onloader 왼쪽면 Door Sensor (문이 닫힐 때 감지되는 센서)
    /// </summary>
    ONL_DoorLeft = 149,
    X96 = 150,
    /// <summary>
    /// Onloader 후면 Door Sensor (문이 닫힐 때 감지되는 센서)
    /// </summary>
    ONL_DoorRear = 150,
    X97 = 151,
    /// <summary>
    /// Onloader Light Curtain (Onloader 왼쪽면에 있는 Area Sensor)
    /// </summary>
    ONL_LightCurtain = 151,
    X98 = 152,
    X99 = 153,
    X9A = 154,
    X9B = 155,
    X9C = 156,
    /// <summary>
    /// Onloader Top Conveyor MGZ 감지 Sensor 
    /// </summary>
    ONL_TopMGZDetect = 156,
    X9D = 157,
    /// <summary>
    /// Onloader Top Conveyor MGZ Full 감지 Sensor (이 Sensor가 감지되면 Top Conveyor에 MGZ이 다 찬 상태로 인식)
    /// </summary>
    ONL_TopMGZDetectFull = 157,
    X9E = 158,
    X9F = 159,
    XA0 = 160,
    /// <summary>
    /// Onloader Bottom Conveyor MGZ 감지 Sensor
    /// </summary>
    ONL_BtmMGZDetect = 160,
    XA1 = 161,
    XA2 = 162,
    XA3 = 163,
    XA4 = 164,
    /// <summary>
    /// Onloader Pusher 과부하 Sensor (Onloader Pusher가 자재를 밀 때 일정 압력이 가해지면 감지되는 센서)
    /// </summary>
    ONL_PusherOverLoad = 164,
    XA5 = 165,
    /// <summary>
    /// Onloader Pusher 전진 Sensor (Onloader Pusher가 전진했을 때 감지되는 센서)
    /// </summary>
    ONL_PusherForward = 165,
    XA6 = 166,
    /// <summary>
    /// Onloader Pusher 후진 Sensor (Onloader Pusher가 후진했을 때 감지되는 센서)
    /// </summary>
    ONL_PusherBackward = 166,
    XA7 = 167,
    XA8 = 168,
    /// <summary>
    /// Onloader Clamp MGZ Detect Sensor 1 
    /// </summary>
    ONL_ClampMGZDetect1 = 168,
    XA9 = 169,
    /// <summary>
    /// Onloader Clamp MGZ Detect Sensor 2
    /// </summary>
    ONL_ClampMGZDetect2 = 169,
    XAA = 170,
    /// <summary>
    /// Onloader Clamp On Sensor (Onloader Clamp가 Down일 때 감지되는 센서)
    /// </summary>
    ONL_ClampOn = 170,
    XAB = 171,
    /// <summary>
    /// Onloader Clamp Off Sensor (Onloader Clamp가 Up일 때 감지되는 센서)
    /// </summary>
    ONL_ClampOff = 171,
    XAC = 172,
    XAD = 173,
    XAE = 174,
    XAF = 175,
    XB0 = 176,
    /// <summary>
    /// Offloader Top Belt Run Button (Offloder 오른쪽면)
    /// </summary>
    OFFL_BtnBeltTopRun = 176,
    XB1 = 177,
    /// <summary>
    /// Offloader Bottom Belt Run Button (Offloder 오른쪽면)
    /// </summary>
    OFFL_BtnBeltBtmRun = 177,
    XB2 = 178,
    /// <summary>
    /// Offloader 전면 Emergency Button
    /// </summary>
    OFFL_EMGFront = 178,
    XB3 = 179,
    /// <summary>
    /// Offloader 후면 Emrgency Button
    /// </summary>
    OFFL_EMGRear = 179,
    XB4 = 180,
    /// <summary>
    /// Offloader 전면 Door Sensor (문이 닫힐 때 감지되는 센서)
    /// </summary>
    OFFL_DoorFront = 180,
    XB5 = 181,
    /// <summary>
    /// Offlaoder 오른쪽면 Door Sensor (문이 닫힐 때 감지되는 센서)
    /// </summary>
    OFFL_DoorRight = 181,
    XB6 = 182,
    /// <summary>
    /// Offloader 후면 Door Sensor (문이 닫힐 때 감지되는 센서)
    /// </summary>
    OFFL_DoorRear = 182,
    XB7 = 183,
    /// <summary>
    /// Offloader Light Curtain (Offloader 오른쪽면에 있는 Area Sensor)
    /// </summary>
    OFFL_LightCurtain = 183,
    XB8 = 184,
    /// <summary>
    /// Offloader Top Conveyor MGZ 감지 Sensor
    /// </summary>
    OFFL_TopMGZDetect = 184,
    XB9 = 185,
    /// <summary>
    /// Offloader Top Conveyor MGZ Full 감지 Sensor (이 Sensor가 감지되면 Top Conveyor에 MGZ이 다 찬 상태로 인식)
    /// </summary>
    OFFL_TopMGZDetectFull = 185,
    XBA = 186,
    XBB = 187,
    /// <summary>
    /// Offloader Middle Conveyor MGZ 감지 Sensor
    /// </summary>
    OFFL_MidMGZDetect = 187,
    // 201116 jym START : IO 추가
    /// <summary>
    /// QC 전면 도어 감지 센서
    /// </summary>
    OFL_QcFrontDoor = 188, XBC = 188,
    /// <summary>
    /// QC 후면 도어 감지 센서
    /// </summary>
    OFL_QcRearDoor = 189, XBD = 189,
    // 201116 jym END
    XBE = 190,
    /// <summary>
    /// Offloader Bottom Conveyor MGZ 감지 Sensor
    /// </summary>
    OFFL_BtmMGZDetect = 190,
    XBF = 191,
    /// <summary>
    /// Offloader Bottom Conveyor MGZ Full 감지 Sensor (이 Sensor가 감지되면 Bottom Conveyor에 MGZ이 다 찬 상태로 인식)
    /// </summary>
    OFFL_BtmMGZDetectFull = 191,
    XC0 = 192,
    /// <summary>
    /// Offloader Top Clamp MGZ 감지 Sensor 1
    /// </summary>
    OFFL_TopClampMGZDetect1 = 192,
    XC1 = 193,
    /// <summary>
    /// Offloader Top Clamp MGZ 감지 Sensor 2
    /// </summary>
    OFFL_TopClampMGZDetect2 = 193,
    XC2 = 194,
    /// <summary>
    /// Offloader Top MGZ Up Sensor (Offloader Top MGZ이 Up 상태일 때 감지되는 센서)
    /// </summary>
    OFFL_TopMGZUp = 194,
    XC3 = 195,
    /// <summary>
    /// Offloader TOp MGZ Down Sensor (Offloader Top MGZ이 Down 상태일 때 감지되는 센서)
    /// </summary>
    OFFL_TopMGZDn = 195,
    XC4 = 196,
    /// <summary>
    /// Offloader Top Clamp On Sensor (Offloader Top Clamp가 Down일 때 감지되는 Sensor)
    /// </summary>
    OFFL_TopClampOn = 196,
    XC5 = 197,
    /// <summary>
    /// Offloader Top Clamp Off Sensor (Offloader Top Clamp가 Up일 때 감지되는 Sensor)
    /// </summary>
    OFFL_TopClampOff = 197,
    XC6 = 198,
    XC7 = 199,
    XC8 = 200,
    /// <summary>
    /// Offloader Bottom Clamp MGZ 감지 Sensor 1
    /// </summary>
    OFFL_BtmClampMGZDetect1 = 200,
    XC9 = 201,
    /// <summary>
    /// Offloader Bottom Clamp MGZ 감지 Sensor 2
    /// </summary>
    OFFL_BtmClampMGZDetect2 = 201,
    XCA = 202,
    /// <summary>
    /// Offloader Bottom MGZ Up Sensor (Offloader Bottom MGZ이 Up일 때 감지되는 센서)
    /// </summary>
    OFFL_BtmMGZUp = 202,
    XCB = 203,
    /// <summary>
    /// Offloader Bottom MGz Down Sensor (Offloder Bottom MGZ이 Down일 때 감지되는 센서)
    /// </summary>
    OFFL_BtmMGZDn = 203,
    XCC = 204,
    /// <summary>
    /// Offloader Bottom Clamp On Sensor (Offloader Bottom Clamp가 Down일 때 감지되는 센서)
    /// </summary>
    OFFL_BtmClampOn = 204,
    XCD = 205,
    /// <summary>
    /// Offlaoder Bottom Clamp Off Sensor (Offloader Bottom Clamp가 Up일 때 감지되는 센서)
    /// </summary>
    OFFL_BtmClampOff = 205,
    XCE = 206,
    XCF = 207
}

public enum eY
{
    Y00 = 0, SYS_MC = 0,
    Y02 = 1,
    Y01 = 2,
    /// <summary>
    /// Tower Lamp Red (장비 상단)
    /// </summary>
    SYS_TwlRed = 2,
    Y03 = 3,
    /// <summary>
    /// Tower Lamp Yellow (장비 상단)
    /// </summary>
    SYS_TwlYellow = 3,
    Y04 = 4,
    /// <summary>
    /// Tower Lamp Green (장비 상단)
    /// </summary>
    SYS_TwlGreen = 4,
    Y05 = 5,
    /// <summary>
    /// Buzzer K1
    /// </summary>
    SYS_BuzzK1 = 5,
    Y06 = 6,
    /// <summary>
    /// Buzzer K2
    /// </summary>
    SYS_BuzzK2 = 6,
    Y07 = 7,
    /// <summary>
    /// Buzzer K3
    /// </summary>
    SYS_BuzzK3 = 7,
    Y08 = 8,
    /// <summary>
    /// Start Button Lamp(Green, 장비 전면)
    /// </summary>
    SYS_BtnStart = 8,
    Y09 = 9,
    /// <summary>
    /// Stop Button Lamp(Red, 장비 전면)
    /// </summary>
    SYS_BtnStop = 9,
    Y0A = 10,
    /// <summary>
    /// Reset Buttom Lamp(Blue, 장비 전면)
    /// </summary>
    SYS_BtnReset = 10,
    Y0B = 11,
    Y0C = 12,
    /// <summary>
    /// 장비 내부 등
    /// </summary>
    SYS_RoomLamp = 12,
    Y0D = 13,
    /// <summary>
    /// Grind Z축 MC 파워
    /// </summary>
    SYS_ZaxisMC = 13,
    Y0E = 14,
    Y0F = 15,
    Y10 = 16,
    Y11 = 17,
    Y12 = 18,
    /// <summary>
    /// Inrail Guide Align Cylinder (자재 고정 실린더, forward/backward)
    /// </summary>
    INR_GuideForward = 18,
    Y13 = 19,
    Y14 = 20,
    Y15 = 21,
    /// <summary>
    /// Inrail Gripper Cylinder (up/dowm)
    /// </summary>
    INR_GripperClampOn = 21,
    Y16 = 22,
    Y17 = 23,
    Y18 = 24,
    /// <summary>
    /// Inrail Lift Cylinder (up/down)
    /// </summary>
    INR_LiftUp = 24,
    //20190611 ghk_onpclean
    Y19 = 25,    
    INR_OnpCleaner_Air = 25,
    Y1A = 26,
    INR_OnpCleaner = 26,
    //
    Y1B = 27,
    /// <summary>
    /// Inrail OCR Air Blow Off / On
    /// </summary>
    INR_OcrAir = 27,
    Y1C = 28,
    Y1D = 29,
    Y1E = 30,
    /// <summary>
    /// Inrail 프로브 (up/down)
    /// </summary>
    INR_ProbeDn = 30,
    Y1F = 31,
    /// <summary>
    /// Inrail 프로브 에어(off/on)
    /// </summary>
    INR_ProbeAir = 31,
    Y20 = 32,
    /// <summary>
    /// Onloader Picker 회전각도 0도 Cylinder (Table과 같은 방향)
    /// </summary>
    ONP_Rotate0 = 32,
    Y21 = 33,
    /// <summary>
    /// Onloader Picker 회전각도 90도 Cylinder (Table 모양에서 90도 회전한 방향)
    /// </summary>
    ONP_Rotate90 = 33,
    Y22 = 34,
    /// <summary>
    /// Onloader Picker Vacuum 1 (자재 흡착)
    /// </summary>
    ONP_Vacuum1 = 34,
    Y23 = 35,
    /// <summary>
    /// Onloader Picker Vacuum 2 (자재 흡착)
    /// </summary>
    ONP_Vacuum2 = 35,
    Y24 = 36,
    /// <summary>
    /// Onloader Picker Ejector (자재 배출)
    /// </summary>
    ONP_Ejector = 36,
    Y25 = 37,
    /// <summary>
    /// Onloader Picker Drain (물 흡입)
    /// </summary>
    ONP_Drain = 37,
    Y26 = 38,
    Y27 = 39,
    Y28 = 40,
    /// <summary>
    /// Grind Door Lock (장비 전, 후면에 있는 문 잠금)
    /// </summary>
    GRD_DoorLock = 40,
    Y29 = 41,
    /// <summary>
    /// IV2 Trigger Out  (화상판별센서) //2021.04.02->2022.06.28 lhs
    /// </summary>
    OFFP_IV2_Trigger = 41,
    Y2A = 42,
    /// <summary>
    /// IV2 Program Bit0 : Program 변경을 위한 Flag로 사용됨, //2021.04.21->2022.06.28 lhs
    /// IV2에 설정된 Bit0, Bit1 조합하여 0 = P000, 1 = P001, 2=P003, 3=P004
    /// IV2 IN2에 주는 DO 신호
    /// </summary>
    OFFP_IV2_ProgBit0 = 42,    // OFFP_IV2_ProgBit0 으로 변경할 것

    Y2B = 43,
    /// <summary>
    /// IV2 Program Bit1 : Program 변경을 위한 Flag로 사용됨, //2022.06.28 lhs
    /// IV2에 설정된 Bit0, Bit1 조합하여 0 = P000, 1 = P001, 2=P003, 3=P004
    /// IV2 IN3에 주는 DO 신호
    /// </summary>
    OFFP_IV2_ProgBit1 = 43,   

    Y2C = 44,
    /// <summary>
    /// Grind Front Water (Grind Zone 전면에서 물 나오는 곳)
    /// </summary>
    GRD_FrontWater = 44,
    Y2D = 45,
    Y2E = 46,
    /// <summary>
    /// Grind Left Wheel Cleaner (Water on/off)
    /// </summary>
    GRDL_WhlCleaner = 46,
    Y2F = 47,
    /// <summary>
    /// Grind Right Wheel Cleaner (Water on/off)
    /// </summary>
    GRDR_WhlCleaner = 47,
    Y30 = 48,
    /// <summary>
    /// Grind Left Table Ejector (자재 배출)
    /// </summary>
    GRDL_TbEjector = 48,
    Y31 = 49,
    /// <summary>
    /// Grind Left Table Vacuum (자재 흡착)
    /// </summary>
    GRDL_TbVacuum = 49,
    Y32 = 50,
    /// <summary>
    /// Grind Left Table Flow (Left Table에서 Water on/off)
    /// </summary>
    GRDL_TbFlow = 50,
    Y33 = 51,
    /// <summary>
    /// Grind Left Top Cleaner Down Cylinder (up/down)
    /// </summary>
    GRDL_TopClnDn = 51,
    Y34 = 52,
    /// <summary>
    /// Grind Left Top Cleaner Air Cylinder (Top Cleaner에서 Air on/off)
    /// </summary>
    GRDL_TopClnAir = 52,
    Y35 = 53,
    /// <summary>
    /// Grind Left Top Cleaner Flow (Water on/off)
    /// </summary>
    GRDL_TopClnFlow = 53,
    Y36 = 54,
    /// <summary>
    /// Grind Left Top Cleaner Water Knife 
    /// </summary>
    GRDL_TopWaterKnife = 54,
    Y37 = 55,
    Y38 = 56,
    /// <summary>
    /// Grind Left Probe Sensor Down Cylinder (up/down)
    /// </summary>
    GRDL_ProbeDn = 56,
    Y39 = 57,
    /// <summary>
    /// Grind Left Probe Sensor Air Cylinder on/off (프로브 센서가 높이 측정 시 측정 위치 물기 제거)
    /// </summary>
    GRDL_ProbeAir = 57,
	Y3A = 58,
    /// <summary>
    /// Grind Left Probe Ejector on/off : P/G 시작 시 상시 On, 종료 시 Off(Only 2000X)
    /// </summary>
    GRDL_ProbeEjector = 58,  //Y3A = 58,     // 2020.09.16 SungTae 
    Y3B = 59,
    /// <summary>
    /// Carrier Vacuum Off/On Left Table
    /// </summary>
    GRDL_CarrierVacuum = 59,
    Y3C = 60,
    /// <summary>
    /// Grind Left Spindle Inverter MC on/off (Inverter 전원)
    /// </summary>
    GRDL_SplInverterMC = 60,
    Y3D = 61,
    /// <summary>
    /// Grind Left Spindle Cooling Air on/off
    /// </summary>
    GRDL_SplCDA = 61,
    Y3E = 62,
    /// <summary>
    /// Grind Left Spindle PCW on/off (스핀들 냉각수)
    /// </summary>
    GRDL_SplPCW = 62,

    Y3F = 63,
    /// <summary>
    /// Unit #1 Vacuum Off/On Left Table (U 전용)
    /// </summary>
    GRDL_Unit_1_Vacuum = 63,
    /// <summary>
    /// Unit Vacuum (4U 전용)
    /// //2100901 syc : 2004U
    /// </summary>
    GRDL_Unit_Vacuum_4U = 63, // eY


    Y40 = 64,
    /// <summary>
    /// Grind Left Spindle Water on/off 
    /// </summary>
    GRDL_SplWater = 64,

    Y41 = 65,
    /// <summary>
    /// Unit #2 Vacuum Off/On Left Table (U 전용)
    /// </summary>
    GRDL_Unit_2_Vacuum = 65,
    /// <summary>
    /// 4U Carrier Detect (4U 전용)
    /// //210910 syc : 2004U
    /// </summary>
    GRDL_Carrier_Vacuum_4U = 65,


    Y42 = 66,
    /// <summary>
    /// Grind Left Spindle Bottom Water on/off
    /// </summary>
    GRDL_SplBtmWater = 66,
    Y43 = 67,
    /// <summary>
    /// Unit #3 Vacuum Off/On Left Table (U 전용)
    /// </summary>
    GRDL_Unit_3_Vacuum = 67,
    Y44 = 68,
    /// <summary>
    /// Unit #4 Vacuum Off/On Left Table (U 전용)
    /// </summary>
    GRDL_Unit_4_Vacuum = 68,
    Y45 = 69,
    Y46 = 70,
    Y47 = 71,
    Y48 = 72,
    /// <summary>
    /// Grind Right Table Ejector (자재 배출)
    /// </summary>
    GRDR_TbEjector = 72,
    Y49 = 73,
    /// <summary>
    /// Grind Right Table Vacuum (자재 흡착)
    /// </summary>
    GRDR_TbVacuum = 73,
    Y4A = 74,
    /// <summary>
    /// Grind Right Table Flow (Water on/off)
    /// </summary>
    GRDR_TbFlow = 74,
    Y4B = 75,
    /// <summary>
    /// Grind Right Top Cleaner Dowm Cylinder (up/down)
    /// </summary>
    GRDR_TopClnDn = 75,
    Y4C = 76,
    /// <summary>
    /// Grind Right Top Cleaner Air on/off
    /// </summary>
    GRDR_TopClnAir = 76,
    Y4D = 77,
    /// <summary>
    /// Grind Right Top Cleaner Flow (Water on/off)
    /// </summary>
    GRDR_TopClnFlow = 77,
    Y4E = 78,
    /// <summary>
    /// Grind Right Top Cleaner Water Knife
    /// </summary>
    GRDR_TopWaterKnife = 78,
    Y4F = 79,
    Y50 = 80,
    /// <summary>
    /// Grind Right Probe Down Cylinder (up/down)
    /// </summary>
    GRDR_ProbeDn = 80,
    Y51 = 81,
    /// <summary>
    /// Grind Right Probe Sensor Air Cylinder on/off (프로브 센서가 높이 측정 시 측정 위치 물기 제거)
    /// </summary>
    GRDR_ProbeAir = 81,
	Y52 = 82,
    /// <summary>
    /// Grind Right Probe Ejector on/off : P/G 시작 시 상시 On, 종료 시 Off(Only 2000X)
    /// </summary>
    GRDR_ProbeEjector = 82,  //Y52 = 82,     // 2020.09.16 SungTae
    Y53 = 83,
    /// <summary>
    /// Carrier Vacuum Off/On Right Table
    /// </summary>
    GRDR_CarrierVacuum = 83,
    Y54 = 84,
    /// <summary>
    /// Grind Right Spindle Inverter MC on/off (Inverter 전원)
    /// </summary>
    GRDR_SplInverterMC = 84,
    Y55 = 85,
    /// <summary>
    /// Grind Right Spindle Cooling Air on/off
    /// </summary>
    GRDR_SplCDA = 85,
    Y56 = 86,
    /// <summary>
    /// Grind Right Spindle PCW on/off (스핀들 냉각수)
    /// </summary>
    GRDR_SplPCW = 86,

    Y57 = 87,
    /// <summary>
    /// Unit #1 Vacuum Right Table (U 전용)
    /// </summary>
    GRDR_Unit_1_Vacuum = 87,
    /// <summary>
    /// Unit Vacuum (4U 전용)
    /// //2100901 syc : 2004U
    /// </summary>
    GRDR_Unit_Vacuum_4U = 87, // eY

    Y58 = 88,
    /// <summary>
    /// Grind Right Spindle Water on/off 
    /// </summary>
    GRDR_SplWater = 88,

    Y59 = 89,
    /// <summary>
    /// Unit #2 Vacuum Right Table (U 전용)
    /// </summary>
    GRDR_Unit_2_Vacuum = 89,
    /// <summary>
    /// 4U Carrier Detect (4U 전용)
    /// //210910 syc : 2004U
    /// </summary>
    GRDR_Carrier_Vacuum_4U = 89,

    Y5A = 90,
    /// <summary>
    /// Grind Right Spindle Bottom Water on/off
    /// </summary>
    GRDR_SplBtmWater = 90,
    Y5B = 91,
    /// <summary>
    /// Unit #3 Vacuum Right Table (U 전용)
    /// </summary>
    GRDR_Unit_3_Vacuum = 91,
    Y5C = 92,
    /// <summary>
    /// Unit #4 Vacuum Right Table (U 전용)
    /// </summary>
    GRDR_Unit_4_Vacuum = 92,
    Y5D = 93,
    Y5E = 94,
    Y5F = 95,
    Y60 = 96,
    /// <summary>
    /// Offloader Picker 회전각도 0도 Cylinder (Table과 같은 방향)
    /// </summary>
    OFFP_Rotate0 = 96,
    Y61 = 97,
    /// <summary>
    /// Offloader Picker 회전각도 90도 Cylinder (Table 모양에서 90도 회전한 방향)
    /// </summary>
    OFFP_Rotate90 = 97,
    Y62 = 98,
    /// <summary>
    /// Offloader Picker Vacuum 1 (자재 흡착)
    /// </summary>
    OFFP_Vacuum1 = 98,
    Y63 = 99,
    /// <summary>
    /// Offloader Picker Vacuum 2 (자재 흡착)
    /// </summary>
    OFFP_Vacuum2 = 99,
    Y64 = 100,
    /// <summary>
    /// Offloader Ejector (자재 배출)
    /// </summary>
    OFFP_Ejector = 100,
    Y65 = 101,
    /// <summary>
    /// Offloader Picker Drain (물 흡입)
    /// </summary>
    OFFP_Drain = 101,

    Y66 = 102,
    /// <summary>
    /// 4U Carrer Clamp Open 
    /// </summary>
    OFFP_Carrier_Clamp_Open = 102,

    Y67 = 103,
    /// <summary>
    /// 4U Carrer Clamp Open 
    /// </summary>
    OFFP_Carrier_Clamp_Close = 103,

    Y68 = 104,
    /// <summary>
    /// 210930 syc 2004U
    /// 2004U Dry top air knife
    /// </summary>
    DRY_TopAirKnife = 104,

    Y69 = 105,
    /// <summary>
    /// 210930 syc 2004U
    /// 2004U Dry bottom air knife
    /// </summary>
    DRY_BtmAirKnife = 105,
    /// <summary>
    /// Dryer Level Safety 센서에 Air Blow 설치 (JCET VOC)
    /// </summary>
    DRY_LevelSafetyAir = 105,   // 2022.03.14 lhs

    Y6A = 106,
    /// <summary>
    /// Dry Top Air on/off 
    /// </summary>
    DRY_TopAir = 106,

    Y6B = 107,
    /// <summary>
    /// Dry Bottom Air on/off
    /// </summary>
    DRY_BtmAir = 107,
    Y6C = 108,
    /// <summary>
    /// Dry Bottom Standby Position Cylinder (Dry Bottom Cleaner를 Standby Position으로 이동하게 하는 실린더)
    /// </summary>
    DRY_BtmStandbyPos = 108,
    /// <summary>
    /// Dry cover open (only U)
    /// </summary>
    DRY_CoverOpen = 108,
    Y6D = 109,
    /// <summary>
    /// Dry Bottom Target Position Cylinder (Dry Bottom Cleaner를 Target Position으로 이동하게 하는 실린더)
    /// </summary>
    DRY_BtmTargetPos = 109,
    /// <summary>
    /// Dry cover close (only U)
    /// </summary>
    DRY_CoverClose = 109,
    Y6E = 110,
    /// <summary>
    /// Dry Out Rail Air Cylinder(자재가 Dry Zone에서 Unloader MGZ으로 이동할 때 자재 윗면에 Air를 배출하는 실린더)
    /// </summary>
    DRY_OutRailAir = 110,
    /// <summary>
    /// Dry Unit Sensor Check시 Air 출력 (Only U)
    /// </summary>
    DRY_CheckAir = 110,

    Y6F = 111,
    /// <summary>
    /// Dry Cleaner Bottom Flow (Dry Cleaner Bottom에서 Water on/off)
    /// </summary>
    DRY_ClnBtmFlow = 111,
    
    Y70 = 112,
    /// <summary>
    /// Left Pump Run (왼쪽 펌프 가동)
    /// </summary>
    PUMPL_Run = 112,
    
    Y71 = 113,
    /// <summary>
    /// Dry Cleaner Bottom Nozzle Move Sol (Dry Cleaner Bottom에서 Nozzle Move Sol Forward/Backward)
    /// </summary>
    DRY_ClnBtmNzlForward = 113,

    Y72 = 114,
    /// <summary>
    /// Dry Cleaner Bottom Air Blower Sol (Dry Cleaner Bottom에서 Air Blower Sol) 
    /// </summary>
    DRY_ClnBtmAirBlow = 114,

    Y73 = 115,
    Y74 = 116,
    Y75 = 117,

    Y76 = 118,
    /// <summary>
    /// Right Pump Run (오른쪽 펌프 가동)
    /// </summary>
    PUMPR_Run = 118,

    Y77 = 119,
    Y78 = 120,
    Y79 = 121,
    Y7A = 122,
    Y7B = 123,

    Y7C = 124, IOZL_Power = 124, //InRail
    Y7D = 125, IOZR_Power = 125, //Dry
    Y7E = 126, IOZT_Power = 126, //Table
    Y7F = 127, 
    Y80 = 128, TNK_1_1 = 128,
    Y81 = 129, TNK_1_2 = 129,
    Y82 = 130, TNK_1_4 = 130,
    Y83 = 131, TNK_1_8 = 131,
    Y84 = 132, TNK_10_1 = 132,
    Y85 = 133, TNK_10_2 = 133,
    Y86 = 134, TNK_10_4 = 134,
    Y87 = 135, TNK_10_8 = 135,
    Y88 = 136, TNK_100_1 = 136,
    Y89 = 137,
    /// <summary>
    /// [Dryer] SOL - Strip Clamp Cylinder 1 Off/On
    /// </summary>
    DRY_ClampOpenOnOff = 137, // 2022.06.28 lhs 
    Y8A = 138,

    /// <summary>
    /// [Delta Driver & RPS Spindle Wheel 3point Measure Control terminal
    /// GRL Control  // 2023.03.15 Max
    /// </summary>
    Y8B = 139,
    GRL_WheelMeasureControlTerminal = 139,

    
    
    Y8C = 140,
    Y8D = 141,
    Y8E = 142,

    /// <summary>
    /// [Delta Driver & RPS Spindle Wheel 3point Measure Control terminal
    /// GRR Control  // 2023.03.15 Max
    /// </summary>
    Y8F = 143,
    GRR_WheelMeasureControlTerminal = 143,

    Y90 = 144,
    /// <summary>
    /// Onloader Belt Run Button Lamp (Onloader 왼쪽면)
    /// </summary>
    ONL_BtnBeltRun = 144,
    Y91 = 145,
    Y92 = 146,
    Y93 = 147,
    Y94 = 148,
    /// <summary>
    /// Onloader Door Lock (Onloader Front/Left/Rear Door 잠금)
    /// </summary>
    ONL_DoorLock = 148,
    Y95 = 149,
    Y96 = 150,
    Y97 = 151,
    Y98 = 152,
    Y99 = 153,
    Y9A = 154,
    Y9B = 155,
    Y9C = 156,
    /// <summary>
    /// Onloader Top Conveyor Belt Run (Top Conveyor Belt 가동)
    /// </summary>
    ONL_TopBeltRun = 156,
    Y9D = 157,
    Y9E = 158,
    Y9F = 159,
    YA0 = 160,
    /// <summary>
    /// Onloader Bottom Conveyor Belt Run (Bottom Conveyor Belt 가동)
    /// </summary>
    ONL_BtmBeltRun = 160,
    YA1 = 161,
    /// <summary>
    /// Onloader Bottom Conveyor Belt CCW (Bottom Conveyor Belt 회전 방향 리버스)
    /// </summary>
    ONL_BtmBeltCCW = 161,
    YA2 = 162,
    YA3 = 163,
    YA4 = 164,
    YA5 = 165,
    /// <summary>
    /// Onloader Pusher Forward Cylinder (Pusher Forward/Backward)
    /// </summary>
    ONL_PusherForward = 165,
    YA6 = 166,
    YA7 = 167,
    YA8 = 168,
    YA9 = 169,
    YAA = 170,
    /// <summary>
    /// Onloader Clamp On Cylinder (Clamp Down)
    /// </summary>
    ONL_ClampOn = 170,
    YAB = 171,
    /// <summary>
    /// Onloader Clamp Off Cylinder (Clamp Up)
    /// </summary>
    ONL_ClampOff = 171,
    YAC = 172,
    YAD = 173,
    YAE = 174,
    YAF = 175,
    YB0 = 176,
    /// <summary>
    /// Offloader Top Belt Run Button Lamp (Offloader 오른쪽면)
    /// </summary>
    OFFL_BtnBeltTopRun = 176,
    YB1 = 177,
    /// <summary>
    /// Offlaoder Bottom Belt Run Button Lamp (Offloader 오른쪽면)
    /// </summary>
    OFFL_BtnBeltBtmRun = 177,
    YB2 = 178,
    YB3 = 179,
    YB4 = 180,
    /// <summary>
    /// Offloader Door Lock (Offlaoder Front/Right/Rear Door 잠금)
    /// </summary>
    OFFL_DoorLock = 180,
    YB5 = 181,
    YB6 = 182,
    YB7 = 183,
    YB8 = 184,
    /// <summary>
    /// Offloader Top Conveyor Belt Run (Top Conveyor Belt 가동)
    /// </summary>
    OFFL_TopBeltRun = 184,
    YB9 = 185,
    YBA = 186,
    YBB = 187,
    /// <summary>
    /// Offloader Middle Conveyor Belt Run (Middle Conveyor Belt 가동)
    /// </summary>
    OFFL_MidBeltRun = 187,
    YBC = 188,
    /// <summary>
    /// Offloader Middle Conveyor Belt CCW (Middle Conveyor Belt 회전방향 리버스)
    /// </summary>
    OFFL_MidBeltCCW = 188,
    YBD = 189,
    YBE = 190,
    /// <summary>
    /// Offloader Bottom Conveyor Belt Run (Bottom Conveyer Belt 가동)
    /// </summary>
    OFFL_BtmBeltRun = 190,
    YBF = 191,
    YC0 = 192,
    YC1 = 193,
    YC2 = 194,
    /// <summary>
    /// Offloader Top MGZ Up Cylinder 
    /// </summary>
    OFFL_TopMGZUp = 194,
    YC3 = 195,
    /// <summary>
    /// Offloader Top MGZ Down Cylinder
    /// </summary>
    OFFL_TopMGZDn = 195,
    YC4 = 196,
    /// <summary>
    /// Offloader Top Clamp On Cylinder (Top Clamp Down)
    /// </summary>
    OFFL_TopClampOn = 196,
    YC5 = 197,
    /// <summary>
    /// Offloader Top Clamp Off Cylinder (Top Clamp Up)
    /// </summary>
    OFFL_TopClampOff = 197,
    YC6 = 198,
    YC7 = 199,
    YC8 = 200,
    YC9 = 201,
    YCA = 202,
    /// <summary>
    /// Offloader Bottom MGZ Up Cylinder
    /// </summary>
    OFFL_BtmMGZUp = 202,
    YCB = 203,
    /// <summary>
    /// Offloader Bottom MGZ Down Cylinder
    /// </summary>
    OFFL_BtmMGZDn = 203,
    YCC = 204,
    /// <summary>
    /// Offloader Bottom Clamp On Cylinder (Bottom Clamp Down)
    /// </summary>
    OFFL_BtmClampOn = 204,
    YCD = 205,
    /// <summary>
    /// Offloader Bottom Clamp Off Cylinder (Bottom Clamp Up)
    /// </summary>
    OFFL_BtmClampOff = 205,
    YCE = 206,
    YCF = 207
}
#endregion

#region Motion
/// <summary>
/// Axis Index
/// 축 인덱스
/// </summary>
public enum EAx
{
    OnLoader_X        = 19,
    OnLoader_Y        = 20,
    OnLoader_Z        = 21,
                      
    OnLoaderPicker_X  = 6,
    OnLoaderPicker_Z  = 7,
    OnLoaderPicker_Y  = 3,
                      
    Inrail_X          = 4,
    Inrail_Y          = 5,
                      
    LeftGrindZone_X   = 8,
    LeftGrindZone_Y   = 9,
    LeftGrindZone_Z   = 10,
                      
    RightGrindZone_X  = 11,
    RightGrindZone_Y  = 12,
    RightGrindZone_Z  = 13,

    OffLoaderPicker_X = 14,
    OffLoaderPicker_Z = 15,

    DryZone_X         = 16,
    DryZone_Z         = 17,
    DryZone_Air       = 18,

    OffLoader_X       = 22,
    OffLoader_Y       = 23,
    OffLoader_Z       = 24,

    LeftSpindle       = 1,//koo Spindle
    RightSpindle      = 2 //koo Spindle
}
/// <summary>
/// Axis Command Mode
/// 축 명령 모드
/// </summary>
public enum EAxCM
{
    /// <summary>
    /// Position mode
    /// </summary>
    POS = 0,
    /// <summary>
    /// Velocity mode
    /// </summary>
    VEL = 1,
    /// <summary>
    /// Torque mode
    /// </summary>
    TRQ = 2
}
/// <summary>
/// Axis Operator Mode
/// 축 상태 모드
/// </summary>
public enum EAxOp
{
    Idle = 0,
    Pos = 1,
    Jog = 2,
    Home = 3,
    Sync = 4,
    GantryHome = 5,
    Stop = 6,
    Intpl = 7,
    DirectVelocityControl = 8,
    List = 9,
    ConstLinearVelocity = 10,
    Trq = 11,
    DirectControl = 12,
    PVT = 13,
    ECAM = 14,
    SyncCatchUp = 15,
    Offline = 16
}
#endregion

#region Device
/// <summary>
/// Grinding Start Direction
/// 그라인딩 시작 방향
/// </summary>
public enum eStartDir
{
    Forward,
    Backward
}
/// <summary>
/// Grinding Mode
/// 그라인딩 모드
/// </summary>
//2021-06-04 lhs Start : 변수명 변경
//public enum eGrdMode
//{
//	Normal = 0,
//	Top
//}
public enum eGrdMode
{
    Target = 0,  
    TopDown
}
//2021-06-04 lhs End
/// <summary>
/// Fine Grinding Mode
/// 파인 그라인딩 시 시작 모드
/// </summary>
public enum eFineMode
{
    WheelInsp,
    Compensation
}
/// <summary>
/// Step Mode
/// 각 스텝 시작 모드
/// </summary>
public enum eStepMode
{
    Both,    // Default
    OnEWay,
    Z_Mode
}
/// <summary>
/// Dry Direction
/// 드라이 방향 설정
/// </summary>
public enum eDryDir
{
    CCW,
    CW
}
/// <summary>
/// Use Dual Mode
/// 듀얼 모드 사용 여부
/// </summary>
public enum eDual
{
    Normal,
    Dual
}
/// <summary>
/// 자재의 Side 모드 선택 : 0 = Top (Single), 1 = Btm (Double), 2 = TopD (Double), 3 = BtmS (Single)
/// </summary>
//public enum eTopSide  // 2021.06.04 lhs 변수명 변경
public enum ESide
{
    Top,    // Top Single
    Btm,    // Btm Double
    TopD,   // Top Double  // 2021.07.09 lhs : TopD 기존에는 사용하지 않음, UseNewSckGrindPooc 적용시 사용
    BtmS    // Btm Single  // 2021.07.09 lhs : BtmS 추가, UseNewSckGrindPooc 적용시 사용
}

/// <summary>
/// Top Down Mode시 Start Pos
/// 듀얼 모드 사용 여부
/// </summary>
public enum eTDStartMode //190502 ksg :
{
    Max ,
    Avg ,
    Mean,
    Min ,
}

// 2021.07.27 lhs Start
/// <summary>
/// Mold 및 Total 기준 그라인딩 0=Mold, 1=Total
/// </summary>
public enum EBaseOnThick
{
    Mold,
    Total
}
// 2021.07.27 lhs End

#endregion

public enum ELang
{
    English = 0,
    China = 1,
    Korea = 2,
}

/// <summary>
/// Access Right Level
/// 접근 권한 레벨
/// </summary>
public enum ELv
{
    Operator  ,
    Technician,
    Engineer  ,
    Master
}

public enum EWay
{
    /// <summary>
    /// Left way
    /// </summary>
    L = 0,
    /// <summary>
    /// Right way
    /// </summary>
    R = 1,
    //20190220 ghk_dynamicfunction
    /// <summary>
    /// Inrail
    /// 다이나믹 펑션 용으로 추가
    /// </summary>
    INR = 2,
    //
}
public enum EPart
{
    /// <summary>
    /// On Loader Part
    /// </summary>
    ONL = 0,
    /// <summary>
    /// In Rail Part
    /// </summary>
    INR,
    /// <summary>
    /// On Loaer Picker Part
    /// </summary>
    ONP,
    /// <summary>
    /// Grind Left Part
    /// </summary>
    GRDL,
    /// <summary>
    /// Grind Right Part
    /// </summary>
    GRDR,
    /// <summary>
    /// Off Loader Picker Part
    /// </summary>
    OFP,
    /// <summary>
    /// Dry Part
    /// </summary>
    DRY,
    /// <summary>
    /// Off Loader Part - Good Strip
    /// </summary>
    OFL,
    /// <summary>
    /// Off Loader Part - Reject Strip 
    /// </summary>
    OFR,  //190325 ksg : QC
    // 2022.01.17 SungTae Start : [추가] (Qorvo향 VOC) 
    /// <summary>
    /// QC-Vision Part
    /// </summary>
    QCV
    // 2022.01.17 SungTae End
}

public enum EStatus
{
    Idle        ,
    Auto_Running,
    LOT_End     ,
    Manual      ,
    Warm_Up     ,
    All_Homing  ,
    Error       ,
    Reset       ,
    Stop
}

public enum ETowerStatus
{
    Idle   ,
    Init   ,
    Run    ,
    ToStop ,
    Stop   ,
    Warring,
    Error  ,
    LotEnd ,
    Manual ,
    SPCAlarm,
    LoadingStop,   // 2022.02.14 lhs : (SCK+)
    // 추가시는 여기에...


    MAX_TWR_STATUS
}

public enum ELamp
{
    Off     ,
    On      ,
    Flick   ,
    MAX_LAMP
}

public enum EBuzz
{
    Buzz1,
    Buzz2,
    Buzz3,
    Off  ,
    MAX_BUZZ
}
//ksg
public enum eAutoStatus
{
    idle  ,
    Ready ,
    Auto  ,
    GoStop,
    Stop 
}

public enum ESeq
{
    /// <summary>
    /// 유휴 상태
    /// </summary>
    Idle,
    /// <summary>
    /// 웜업 동작
    /// </summary>
    Warm_Up,
    /// <summary>
    /// 모든 축 서보 온
    /// </summary>
    All_Servo_On,
    /// <summary>
    /// 모든 축 서보 오프
    /// </summary>
    All_Servo_Off,
    /// <summary>
    /// 모든 축 홈 시퀀스
    /// </summary>
    All_Home,

    Reset,

    /// <summary>
    /// 1. ONL_Wait -> ONL_Pick
    /// 2. ONL_Pick -> ONL_PUsh
    /// 3. ONL_PUsherWork : 매거진 슬롯 유무 확인 하나라도 있을 경우 상태 유지
    /// 4. ONL_PUsherWork -> ONL_Place : 매거진 슬롯 전체 Empty 될 경우
    /// </summary>
    ONL_Wait,    //매거진 없는 상태 웨이트   Cyl_Wait
    ONL_Pick,   //매거진 집는거  Cyl_Pick
    ONL_Push,    //자재 투입    Cyl_PusherWork : Z축 높이 확인 함수 필요(피치 이동)
    ONL_Place,    //매거진 버리는거    Cyl_Place
    ONL_Home,    // 온로더 홈
    ONL_WorkEnd,    // 온로더 작업 종료

    //20191209 ghk_lotend_placemgz
    /// <summary>
    /// Lot End 버튼 클릭시 온로더, 오프로더 매거진 버리는 상태
    /// </summary>
    ONLOFL_Place,
    //
    //20200109 myk_ONL_Push_Position
    /// <summary>
    /// Onloader Push Position 이동
    /// </summary>
    ONL_Push_Pos,
    //

    // 2022.04.19 SungTae Start : [추가] ASE-KH용
    /// <summary>
    /// Onloader MGZ ID Read Position 이동 및 BCR Reading
    /// </summary>
    ONL_CheckBcr,
    // 2022.04.19 SungTae End

    /// <summary>
    /// INR_Wait -> INR_Recive
    /// INR_Recive -> INR_CheckBcr
    /// INR_CheckBcr -> INR_Align
    /// INR_Align -> INR_WaitRecive
    /// </summary>
    INR_Wait,    //웨이트    Cyl_Wait
    INR_Ready     ,    //자재 투입 대기
    INR_CheckBcr  ,    //바코드 리딩(OK,NG판별)    Cyl_CheckBcr
    INR_CheckOri  ,    //Orientation(OK,NG판별)    Cyl_CheckOri
    INR_Align     ,    //얼라인 위치    Cyl_Align
    INR_WaitPicker,    //얼라인 후 피커 대기 상태
    INR_Home      ,
    INR_WorkEnd   ,
    //20190218 ghk_dynamicfunction
    INR_CheckDynamic,
    INR_DynamicFail,
    //
    //20190604 ghk_onpbcr
    INR_BcrReady,
    INR_OriReady,
    INR_CheckBcrOri, //190821 ksg :
    //
    //210927 syc : 2004U
    INR_CheckIV2,
    // syc end

    /// <summary>
    /// 1. ONP_Wait      -> ONP_Pick
    /// 2. ONP_Pick      -> ONP_WaitPlace
    /// 듀얼 모드 아닐 경우
    /// 3. ONP_WaitPlace -> ONP_PlaceTbL or ONP_PlaceTbR : MGZ_No, Slot_No 순으로 오른쪽 부터 내려 놓음
    /// 4. ONP_PlaceTbL or ONP_PlaceTbR -> ONP_Wait
    /// 듀얼 모드 일 경우
    /// 3. ONP_WaitPlace -> ONP_PlaceTbL
    /// 4. ONP_PlaceTbL  -> ONP_Wait        : 왼쪽 테이블 그라인 완료 까지 대기
    /// 5. ONP_Wait      -> ONP_PickTbL
    /// 6. ONP_PickTbL   -> ONP_PlaceTbR
    /// 7. ONP_PlaceTbR  -> ONP_Wait
    /// </summary>
    ONP_Wait     ,   // 자재 없는 상태 웨이트    Cyl_Wait : 듀얼 모드일 경우 왼쪽 테이블 자재 있고, 오른쪽 테이블 자재 없을 경우 Inrail 자재 Pick하면 안됨.
    ONP_WaitPlace,   // 자재 있는 상태 웨이트    Cyl_Wait
    ONP_Pick     ,   // 인레일 자재 Pick        Cyl_Pick
    ONP_PickTbL  ,   // 왼쪽 테이블 자재 Pick    Cyl_PickTbL
    ONP_PlaceTbL ,   // 왼쪽 테이블에 자재 놓기   Cyl_PlaceTbL
    ONP_PlaceTbR ,   // 오른쪽 테이블에 자재 놓기  Cyl_PlaceTbR
    ONP_Conv     ,   // 양 Table 사이에 피커 위치 
    ONP_Home     ,
    ONP_WorkEnd  ,
    //20190604 ghk_onpbcr
    ONP_CheckBcr,
    ONP_CheckOri,
    //
    //20190611 ghk_onpclean
    ONP_Clean,
    ONP_CheckBcrOri, //190821 ksg :
    //
    //20190822 pjh OnpConvWait
    ONP_ConvWait,
    ONP_IV2, //210907 syc : 2004U
    //


    GRD_Wait,
    GRD_Home,
    GRD_DRESSER_REPLACE,
    GRD_BufferTest,

    //20200731 josh    dual dressing
    /// <summary>
    /// ASE kr VOC
    /// CSQ_Man.cs Cycle() case GRD_Dual_Dressing:
    /// frmMain.cs 작동함수, 버튼이벤트 등 처리
    /// </summary>
    GRD_Dual_Dressing,
    //
    ///<summary>
    ///220321 pjh : Dual Grinding
    ///ASE KH VOC
    /// </summary>
    GRD_Dual_Grinding,

    /// <summary>
    /// 드레싱 할 경우
    /// 1. GRDL_Wait -> GRDL_Dressing : 드레싱 사이클에 휠 측정, 드레셔 측정 포함 되있음
    /// 2. GRDL_Dressing -> GRDL_Wait
    /// 그라인딩 할 경우
    /// 1. GRDL_Wait -> GRDL_Grinding : 그라인딩 사이클에 자재 측정 포함 되있음
    /// 2. GRDL_Grinding -> GRDL_WaterKnife
    /// 3. GRDL_WaterKnife -> GRDL_WaitPick
    /// 4. GRDL_WaitPick -> GRDL_Wait
    /// </summary>
    GRL_Wait                        , //자재 없는 상태 웨이트    Cyl_Wait
    GRL_Ready                       ,
    GRL_WaitPick                    , //그라인딩 완료 후 오프로더 피커 집기전 웨이트    Cyl_Wait
    GRL_Dressing                    , //드레싱    Cyl_Dressing
    GRL_Grinding                    , //자재 그라인딩    Cyl_Dressing
    GRL_WaterKnife                  , //워터 나이프    Cyl_WaterKnife
    GRL_Home                        ,
    GRL_Table_Measure               ,
    GRL_Strip_Measure               ,
    GRL_Strip_Measureone            ,
    GRL_Water_Knife                 ,
    GRL_Dresser_Measure             ,
    GRL_Wheel_Measure               ,
    GRL_Table_Grinding              ,
    GRL_Table_Grinding_Wheel_Measure,
    GRL_Table_Grinding_Table_Measure,
    GRL_Table_Grinding_Work         ,
    GRL_WorkEnd                     ,
    GRL_ProbeTest                   ,
    GRL_Table_MeasureSix            , //190628 ksg :
    GRL_Table_Measure_8p            , //200928 lhs :
    //20190703 ghk_automeasure
    GRL_AfterMeaStrip,
    //
    //20190819 ghk_tableclean
    GRL_Table_Clean,
    //
    //20191010 ghk_manual_bcr
    GRL_Manual_GrdBcr,
    GRL_Manual_Bcr,
    //
    //200812 myk : Probe Calibration Check
    GRL_Probe_Calibration_Check,
    //
    //200818 myk : Probe Auto Calibration
    GRL_Probe_Auto_Calibration,
    //

    /// <summary>
    /// 드레싱 할 경우
    /// 1. GRDR_Wait -> GRDR_Dressing : 드레싱 사이클에 휠 측정, 드레셔 측정 포함 되있음
    /// 2. GRDR_Dressing -> GRDR_Wait
    /// 그라인딩 할 경우
    /// 1. GRDR_Wait -> GRDR_Grinding : 그라인딩 사이클에 자재 측정 포함 되있음
    /// 2. GRDR_Grinding -> GRDR_WaterKnife
    /// 3. GRDR_WaterKnife -> GRDR_WaitPick
    /// 4. GRDR_WaitPick -> GRDR_Wait
    /// </summary>
    GRR_Wait,    //자재 없는 상태 웨이트    Cyl_Wait
    GRR_Ready                       ,
    GRR_WaitPick                    ,    //그라인딩 완료 후 오프로더 피커 집기전 웨이트    Cyl_Wait
    GRR_Dressing                    ,    //드레싱    Cyl_Dressing
    GRR_Grinding                    ,    //자재 그라인딩    Cyl_Dressing
    GRR_WaterKnife                  ,    //워터 나이프    Cyl_WaterKnife
    GRR_Home                        ,
    GRR_Table_Measure               ,
    GRR_Strip_Measure               ,
    GRR_Water_Knife                 ,
    GRR_Strip_Measureone            ,
    GRR_Dresser_Measure             ,
    GRR_Wheel_Measure               ,
    GRR_Table_Grinding              ,
    GRR_Table_Grinding_Wheel_Measure,
    GRR_Table_Grinding_Table_Measure,
    GRR_Table_Grinding_Work         ,
    GRR_WorkEnd                     ,
    GRR_ProbeTest                   ,
    GRR_Table_MeasureSix            , //190628 ksg :
    GRR_Table_Measure_8p            , //200928 lhs :
    //20190703 ghk_automeasure
    GRR_AfterMeaStrip,
    //
    //20190819 ghk_tableclean
    GRR_Table_Clean,
    //
    //20191010 ghk_manual_bcr
    GRR_Manual_GrdBcr,
    GRR_Manual_Bcr,
    //
    //200812 myk : Probe Calibration Check
    GRR_Probe_Calibration_Check,
    //
    //200818 myk : Probe Auto Calibration
    GRR_Probe_Auto_Calibration,
    //

    /// <summary>
    /// 1. OFFP_BtmClean -> OFFP_Wait : 시작시 피커 바텀 부터 클리닝
    /// 듀얼 모드 아닐 경우
    /// 2. OFFP_Wait -> OFFP_PickTbL or OFFP_PickTbR : MGZ_No, Slot_No 순으로 집음
    /// 3. OFFP_PickTbL or OFFP_PickTbR -> OFFP_BtmCleanStrip
    /// 4. OFFP_BtmCleanStrip -> OFFP_WaitPlace
    /// 5. OFF_WaitPlace -> OFF_Place
    /// 6. OFF_Place -> OFF_BtmClean
    /// 7. OFF_BtmClean -> OFFP_Wait
    /// </summary>
    OFP_Wait,           //자재 없는 상태 웨이트(드라이 바텀 클리너 위에 위치, 피커 0도)    Cyl_Wait
    OFP_WaitPlace,      //자재 있는 상테 웨이트 : X축 드라이 상단, 피커 90도, 드라이 동작 중일경우 드라이 존 자재 배출 할때까지 대기
    OFP_PickTbL,        //왼쪽 테이블 자재 Pick    Cyl_PickTbL
    OFP_PickTbR,        //오른쪽 테이블 자재 Pick  Cyl_PickTbR
#if true //20200415 jhc : Picker Idle Time 개선
    OFF_WaitPickTbR,    //오른쪽 테이블 자재 Pick 대기   Cyl_WaitPickTbR
    OFF_WaitPickTbL,    //왼존 테이블 자재 Pick 대기     Cyl_WaitPickTbL 
    OFP_PlaceTbR,       //오른쪽 테이블에 자재 놓기       Cyl_PlaceTbR
#endif
    OFP_BtmClean,       //피커 바텀 클리닝      Cyl_BtmClean
    OFP_BtmCleanStrip,  //자재 바텀 클리닝      Cyl_BtmCleanStrip
    OFP_BtmCleanBrush,  //자재 브러쉬 클리닝    Cyl_BtmCleanBrush   // 2022.07.27 lhs
    OFP_Place,          //자재 드라이에 놓기    Cyl_Place
    // 2021.04.01 lhs Start
    OFP_IV_DryClamp,    // IV(화상판별센서)로 Dryer Clamp 정상여부, #1, #2 촬영 및 판정, Cyl_IV_DryClamp
    OFP_IV_DryTable,    // IV(화상판별센서)로 Dryer Table을 촬영하여 Strip 존재여부, #3 촬영 및 판정,     Cyl_IV_DryTable   // 2022.07.07 lhs
    OFP_CoverDryer,     // Dryer 커버역할 (Dryer Water Nozzle 분사시 물 튀는거 방지용) Cyl_CoverDryer
    OFP_CoverDryerKeep, // Dryer 커버역할 유지
    OFP_CoverDryerEnd,  // Dryer가 WaterNozzle이 끝나면 커버 역할을 끝내고 Wait/BtmClean/WorkEnd 중 하나로 상태 변경.  Cyl_ChCoverDryerEnd 
    // 2021.04.01 lhs End
    OFP_Conv,           //양 Table 사이에 피커 위치 
    OFP_Home,
    OFP_WorkEnd,
    OFP_ConvWait,       //Off Picker Conversion Wait Pos
    // 210927 syc : 2004U start
    OFP_IV2,            // Unit IV2 검사
    OFP_CoverIV2,       // Cover IV2 검사
    OFP_CoverPick,      // Cvoer Pick 
    OFP_WaitCoverPick,  // Cover Pick 대기 상태
    OFP_CoverPlace,     // Cover Place 상태

    /// <summary>
    /// 1. DRY_Wait -> DRY_CheckSensor : UP상태에서 대기, 오프로더 피커에서 자재 받은 후 센서 체크
    /// 2. DRY_CheckSensor -> DRY_Run
    /// 3. DRY_Run -> DRY_WaitOut : 런 완료 후 UP상태 대기
    /// 4. DRY_WaitOut -> DRY_Out : DRY_Out 동작 시 오프로더 매거진 유무, 슬롯 상태 확인
    /// 5. DRY_Out -> DRY_Wait
    /// 
    // 2021.04.09 lhs Start
    /// [ Water Nozzle 사용시 ]
    /// 1. DRY_Wait -> DRY_WaitPlace -> DRY_WaitIV2 : UP상태에서 대기, 오프로더 피커에서 자재 받은 후 IV2 촬영하여 Dry Tip 체크
    /// 2. DRY_WaitIV2 -> DRY_WaterNozzle -> Dry_Run : Tip 체크 후 Water로 자재 세척(저속회전) -> Dry_Run (이후 동일)
    // 2021.04.09 lhs End
    /// </summary>
    DRY_Wait,           //자재 없는 상태 웨이트
    DRY_WaitPlace,      //자재 받기전 대기 상태
    DRY_WaitIV2,        //자재 Place 이후 상태, IV2로 촬영시 위치 유지      // 2021.04.09 lhs
                        //2004U Unit 검사시에도 겸용으로 사용        //210927 syc : 2004U
    DRY_WaitCoverPick,  //2004U Cover Pick 이전까지 위치 유지     //210927 syc : 2004U
    DRY_WaitCoverPlace, //2004U Cover Place 이전까지 위치 유지    // 210927 syc : 2004U
    DRY_WaitOut,        //자재 있는 상태 웨이트(OUT 전상태) Cyl_WaitOut
    DRY_CheckSensor,    //자재 거취 불량 판별 센서 체크     Cyl_CheckSensor
    DRY_WaterNozzle,    //Water Nozzle로 Clean          Cyl_WaterNozzle // 2021.04.09 lhs
    DRY_Run,            //자재 드라이 러닝   Cyl_Run    
    DRY_Out,            //자재 배출         Cyl_Out
    DRY_Home,
    DRY_WorkEnd,

    /// <summary>
    /// 1. OFFL_Wait -> OFFL_TOPPick or OFFL_BtmPick : 사용자 지정한 클램프로 매거진 Pick, 현재 디폴트 Bottom
    /// 2. OFFL_TOPPick or OFFL_BtmPick -> OFFL_WaitTop or OFFL_WaitBtm : 매거진 받은 후 자재 받는 위치 대기
    /// 3. OFFL_WaitTop or OFFL_WaitBtm -> OFFL_TopRecive or OFFL_BtmRecive : 자재 받는 상태 받은 후 피치 이동
    /// 4. OFFL_TopRecive or OFFL_BtmRecive : 매거진 슬롯 다 채울때까지 상태 유지
    /// 5. OFFL_TopRecive or OFFL_BtmRecive -> OFFL_TopPlace or OFFL_BtmPlace : 매거진 슬롯 모두 채운 상태 매거진 배출
    /// 6. OFFL_TopPlace or OFFL_BtmPlace -> OFFL_Wait
    /// </summary>
    OFL_Wait,       //매거진 없는 상태 웨이트    Cyl_Wait
    OFL_BtmPick,    //바텀 매거진 집기    Cyl_BtmPick
    OFL_BtmPlace,   //바텀 매거진 버리기    Cyl_BtmPlace
    OFL_BtmRecive,  //바텀 자재 드라이에서 받기(받은 후 피치 이동, 슬롯 자재 유무 판별)    Cyl_BtmRecive
    OFL_TopPick,    //탑 매거진 집기    Cyl_TopPick
    OFL_TopPlace,   //탑 매거진 버리기    Cyl_TopPlace
    OFL_TopRecive,  //탑 자재 드라이에서 받기(반은 후 피치 이동, 슬롯 자재 유무 판별)    Cyl_TopRecive
    OFL_WorkEnd,
    OFL_Home,


    // 2022.04.19 SungTae Start : [추가] ASE-KH용
    /// <summary>
    /// Offloader MGZ ID Read Position 이동 및 BCR Reading
    /// </summary>
    OFL_CheckBcr,
    // 2022.04.19 SungTae End
}

public enum eSlotInfo
{
    Empty,
    Exist,
}

public enum eLog
{
    None,
    //    로그 분류
    //    대분류 -> 모든 로그는 전체 로그에 기록, 에러 발생 시에는 에러 로그 기록
    /// <summary>
    /// DEB.log    전체 로그 : Debug Log
    /// </summary>
    DBE,
    /// <summary>
    /// ERR.log    에러 발생 로그 : Error Log -> 에러발생값 같이 기록
    /// </summary>
    ERR,

    //생산기준별 -> 장비 가동 상태에 따라 분류하여 기록
    /// <summary>
    /// MR.log    매뉴얼 런 로그 : Manual Run Log -> 매뉴얼 상태에서 가동된 기록
    /// </summary>
    MR,
    /// <summary>
    /// AR.log    오토 런 로그 : Auto Run Log -> 오토 상태에서 가동된 기록
    /// </summary>
    AR,

    //상태별 - 이벤트성
    /// <summary>
    /// OPL.log    조작로그 : Operate Log -> 조작된 버튼
    /// </summary>
    OPL,
    /// <summary>
    /// DSL.log    데이터저장로그 : Data Save Log -> 2개의 파일로 기록, 통채로/변경된부분만
    /// </summary>
    DSL,
    /// <summary>
    /// RCL.log    권한변경로그 : Rights Log -> 권한 변경 시    이전/이후
    /// </summary>
    RCL,
    /// <summary>
    /// ESL.log    장비상태로그 : Equipment Status Log -> 유휴/런/에러
    /// </summary>
    ESL,
    /// <summary>
    /// LOT.log    LOT 로그 : Lot Log -> LOT 진행 중, 발생 데이터 기록. LOT폴더 안에 LOT명 폴더 안에 LOT_LOT명.log
    /// </summary>
    LOT,

    //구성 분야별 -> 구성 분야 별 로그 기록
    /// <summary>
    /// WMX.log    제어기 로그 : WMX Log -> 연결, 해제
    /// </summary>
    WMX,
    /// <summary>
    /// MOT.log    모션 로그 : Motion Log -> 모듈, 축명     지령, 결과
    /// </summary>
    MOT,
    //---    신호 로그 : Input / Output Log -> 별도 기록 안함, 문제가 되는 부분은 에러 로그에 기록된 값을 참조
    /// <summary>
    /// SPL_L.log, SPL_R.log    스핀들 로그 : Spindle Log -> LR    연결, rpm, run, stop
    /// </summary>
    SPL_L,
    /// <summary>
    /// SPL_L.log, SPL_R.log    스핀들 로그 : Spindle Log -> LR    연결, rpm, run, stop
    /// </summary>
    SPL_R,
    /// <summary>
    /// PRB_L.log, PRB_R.log    프로브 로그 : Probe Log -> LR    연결, 획득 값
    /// </summary>
    PRB_L,
    /// <summary>
    /// PRB_L.log, PRB_R.log    프로브 로그 : Probe Log -> LR    연결, 획득 값
    /// </summary>
    PRB_R,


    /// <summary>
    /// SG.log    SECS / GEM Log : SECS / GEM Log -> 추후 진행 시
    /// </summary>
    SG,
    /// <summary>
    /// OL    자재 기준 로그 : Object Log -> 자재의 투입 정보부터 배출 정보까지 자재 기준으로 작성.
    /// 오토런일 경우 : LOT 폴더 안에 LOT명 폴더 안에 OL_매거진no_슬롯no.log
    /// 매뉴얼 런일 경우 : LOT 폴더 안에 Manual 폴더 안에 OL_최초 시간.log
    /// </summary>
    OL,

    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    ONL,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    INR,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    ONP,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    GRL,
    /// <summary>
    /// 시퀀스 로그 : 좌측 그라인더에서 그라인딩 시 사용한 데이터 로그
    /// </summary>
    GRL_DATA,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    GRR,
    /// <summary>
    /// 시퀀스 로그 : 우측 그라인더에서 그라인딩 시 사용한 데이터 로그
    /// </summary>
    GRR_DATA,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    OFP,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    DRY,
    /// <summary>
    /// 시퀀스 로그 : 모듈별로 작성    사이클, 스텝. Sequence폴더 안에 바이너리 파일
    /// </summary>
    OFL,
    
    
}

public enum eWhlMeasureType
{
    /// <summary>
    /// 메뉴얼 휠 높이 체크
    /// </summary>
    MeasureWheel,
    /// <summary>
    /// 메뉴얼 드레싱 전
    /// </summary>
    BeforeManual,
    /// <summary>
    /// 메뉴얼 드레싱 후
    /// </summary>
    AfterManual,
    /// <summary>
    /// 오토 드레싱 전
    /// </summary>
    BeforeAuto,
    /// <summary>
    /// 오토 드레싱 후
    /// </summary>
    AfterAuto,
}

//220106 pjh : Dresser Measure Type Enum 추가
public enum eDrsMeasureType
{
    /// <summary>
    /// 매뉴얼 드레서 높이 체크
    /// </summary>
    MeasureDrs,
    /// <summary>
    /// 메뉴얼 드레싱 전
    /// </summary>
    BeforeManual,
    /// <summary>
    /// 메뉴얼 드레싱 후
    /// </summary>
    AfterManual,
    /// <summary>
    /// 오토 드레싱 전
    /// </summary>
    BeforeAuto,
    /// <summary>
    /// 오토 드레싱 후
    /// </summary>
    AfterAuto,
}

/// <summary>
/// Company use Option
/// </summary>
public enum ECompany
{
    Suhwoo,     //190521 ksg : 메뉴얼용

    ASE_KR,
    ASE_K12,    //190521 ksg : 메뉴얼용
    Qorvo,
    Qorvo_DZ,  // 200723 jym : 추가
    SkyWorks,
    SCK,        //200121 ksg : 추가
    JSCK,       //190417 ksg : -> SCK+   
    SST,        //191202 ksg : Qorvo와 동일하게 적용
    USI,        //191202 ksg :     
    SPIL,  //200228 LCY : 추가
    JCET,  //200622 lks : 추가
    Qorvo_RT,   // 21.03.16 SungTae : 추가
    Qorvo_NC,   // 21.03.16 SungTae : 추가
}

#region Bcr Status
/// <summary>
/// Nomal    : Inrail에 있음. Ase Kr, Sky Works(OCR)
/// Picker   : Picker에 있음. Qorvo , Ase K26       , 
/// Not Uses : JSCK
/// </summary>
public enum eEquType
{             
    Nomal , 
    Pikcer, 
}
/// Stop: BCR KeyIn 시 Error 띄우고 Stop 형식
/// Wait: BCR KeyIn 시 Error 안띄우고 Wait 형식
public enum eBcrKeyIn
{
    Stop,
    Wait,
}
#endregion
//20190628 ghk_onpclean
/// <summary>
/// ONP 클리너 사용 유무
/// Use : 클리너 사용
/// NotUse : 클리너 사용 안함
/// </summary>
public enum eOnpClean
{
    Use = 1,
    NotUse = 0,
}
//
//20190703 ghk_dfserver
/// <summary>
/// DfServer 사용 유무
/// Use : 서버 사용
/// NotUse : 서버 사용 안함
/// </summary>
public enum eDfserver
{
    Use = 1,
    NotUse = 0,
}
////20190910 ksg_dfProbe Type
/// <summary>
/// Df Probe Type
/// Keyence 
/// Hanse   
/// </summary>
public enum eDfProbe
{
    Keyence,
    Hanse,
}
//

//spindle
public enum ESw
{
    OFF = 0,
    ON = 1
}

/// <summary>
/// RFID 종류 SCK/+:RFID  SPIL:Keyence
/// </summary>
public enum ERfidType
{
    None,
    RFID,
    Keyence
}

/// <summary>
/// 20200120 ksg_OFL RFID 사용 여부
/// </summary>
public enum eOflRFID
{
    Use = 1,
    NotUse = 0,
}

/// <summary>
/// 20200120 ksg_ONL RFID 사용 여부
/// </summary>
public enum eOnlRFID
{
    Use = 1,
    NotUse = 0,
}

/// <summary>
/// 200121 ksg_ Pump Auto Off 사용 여부
/// </summary>
public enum ePumpAutoOff
{
    Use = 1,
    NotUse = 0,
}

/// <summary>
/// 자재(Package) 종류
/// </summary>
public enum ePkg
{
    Strip,
    Unit,
}

/// <summary>
/// Dryer Model
/// </summary>
public enum eDryer
{
    Rotate, // 2000X
    Knife,  // 2000U 
}

// 2022.07.01 lhs
/// <summary>
/// Dryer Clamp Type 
/// 0.None = 기존 Spring Type 
/// 1.Cylinder = 양쪽에서 Cylinder로 밀어 UnClamp 하는 Type (현재는 Output 1ea, Input 4ea)
/// </summary>
public enum EDryClamp
{
    None,
    Cylinder,
}

/// <summary>
/// 200131 ksg_ BCR Protocol Type
/// </summary>
public enum eBcrProtocol
{
    Old,
    New,
}

//spindle
public enum eSplError
{
    //koo spindle
        EQUIPMENT_SPINDLE_RPM_UNDER_ZERO =1  ,
        EQUIPMENT_SPINDLE_RPM_OVER_MAX       ,
        EQUIPMENT_SPINDLE_OVER_SET_CURRENT   ,
        EQUIPMENT_SPINDLE_CAN_NOT_GET_CURRENT,
        EQUIPMENT_SPINDLE_RPM_RUN_ERROR      ,
        EQUIPMENT_SPINDLE_QUICK_STOP_ERROR   ,
        EQUIPMENT_SPINDLE_TIMEOUT            ,
        EQUIPMENT_SPINDLE_ALARM              ,
        EQUIPMENT_SPINDLE_SERVO_OFF          ,
        EQUIPMENT_SPINDLE_STOP_ERROR         ,
        EQUIPMENT_SPINDLE_CAN_NOT_CLEAR_ALARM,
}

//20190617 ghk_dfserver
public enum ECMD
{
    scId = 0,
    scReady,
    scLotOpen,
    scLotExist,
    scLotEnd,
    scInrBcrOk,
    scInrBcrNg,
    //Grind Left Ack
    scGrdReadyL,
    scBfStartL,
    scPcbL,
    scRowL,
    scBfEndL,
    scAfStartL,
    scAfEndL,
    //Grind Right Ack
    scGrdReadyR,
    scBfStartR,
    scPcbR,
    scRowR,
    scBfEndR,
    scAfStartR,
    scAfEndR,

    MAX_ECMD,
}
//190723 ksg :
public enum eSecsGem
{
    NotUse = 0,
    Use    = 1
}
/// <summary>
/// Secsgem 버전
/// </summary>
public enum eSecsGemVer
{
    StandardA = 1, // 2000X 기본 사양
    StandardB = 2,     // 1000U와 혼용 사양
}
//20190926 ghk_ofppadclean
public enum eOffPadCleanType
{
    LRType = 0,
    UDType = 1
}
//

//20191010 ghk_manual_bcr
public enum eManualBcr
{
    NotUse = 0,
    Use = 1
}
//

//20191022 ghk_spindle_type
public enum eSpindleType
{
    Rs232 = 0,
    EtherCat = 1,
}
//

public enum eUseToolSetterIO
{
    NotUse = 0,
    Use = 1
}

//20191028 ghk_auto_tool_offset
public enum eAutoToolOffset
{
    NotUse = 0,
    Use = 1
}
//
//20191029 ghk_dfserver_notuse_df
public enum eDfServerType
{
    MeasureDf = 0,      //DF서버 사용시 DF 측정
    NotMeasureDf = 1    //DF서버 사용시 DF 측정 안함
}
//

//20191105 ghk_measuretype
public enum eMeasureType
{
    Step = 0,
    Jog = 1
}
//

//20191106 ghk_lightcontrol
public enum eLightControl
{
    NotUse = 0,
    Use = 1
}

public enum ELightCMD
{
    scOn1 = 0,
    scOn2,
    scOff1,
    scOff2,
    scBright1,
    scBright2,

    MAX_ECMD,
}
//

//20191111 ghk_regrindinglog
public enum eReGrdLog
{
    NotUse = 0,
    Use = 1
}
//

//191203 ksg : Table Grd Manual Set Pos
public enum eTblSetPos
{
    NotUse = 0,
    Use = 1
}
//
//191203 ksg : Auto Warm Up : 일정 시간 경과 후 자동 Warm Up
public enum eAutoWarmUp
{
    NotUse = 0,
    Use = 1
}
//
//191203 ksg : 모든 동작 시 Door Lock 되는 기능
public enum eManualDoorLock
{
    NotUse = 0,
    Use = 1
}
//
//191203 ksg : PCW 자동 Turn Off
public enum ePcwAutoOff
{
    NotUse = 0,
    Use = 1
}
//
//191203 ksg : Grd부 Leak sensor 사용 여부
public enum eChkGrdLeak
{
    NotUse = 0,
    Use = 1
}
//
//191203 ksg : Dry부 Leak Sensor 사용 여부
public enum eChkDryLeak
{
    NotUse = 0,
    Use = 1
}
//
//191203 ksg : Dry부 Leak Sensor 사용 여부
public enum eZUpOffset
{
    NotUse = 0,
    Use = 1
}
//
//20191206 ghk_lotend_placemgz
/// <summary>
/// 강제로 Lot End 했을 경우 온로더, 오프로더 매거진 Place 기능 사용 여부
/// </summary>
public enum eLotEndPlaceMgz
{
    NotUse = 0,
    Use = 1
}
//
//20191211 ghk_display_strip
/// <summary>
/// 시퀀스 자재 상태와 센서 상태 비교 하여 자재 점멸 표시 기능 사용 여부
/// </summary>
public enum eDisplayStrip
{
    NotUse = 0,
    Use = 1
}
//
//200102 ksg : 추가
public enum eTableClean
{
    NotUse = 0,
    Use = 1
}
//
//20200311 jhc : 2000U : UDP interface for BCR 2D Vision
#region UDP Interface
/// <summary>
/// BCR Interface Type
/// 2000X : INI file interface
/// 2000U : UDP 통신 interface
/// </summary>
public enum eBcrInterface
{
    Ini,
    Udp,
}
//
/// <summary>
/// BCR UDP Interface 용 Command 번호
/// </summary>
public enum eBcrCommand
{
    None = 0,
    ChangeRecipe = 1,       //Recipe Change
    ChangeAutoMode,         //Change 2D Vision SW to Auto Mode (Ready State >> Auto State)
    OriInr,                 //Orientation 검사 요청 (InRail)
    OriGL,                  //Orientation 검사 요청 (Left Table)
    OriGR,                  //Orientation 검사 요청 (Right Table)
    BcrInr,                 //2D 코드 검사 요청 (InRail)
    BcrGL,                  //2D 코드 검사 요청 (Left Table)
    BcrGR,                  //2D 코드 검사 요청 (Right Table)

    //201013 jhc : UDP(한솔루션) OCR 연동 추가
    OcrInr,                 //OCR 검사 요청 (InRail)
    OcrGL,                  //OCR 검사 요청 (Left Table)
    OcrGR,                  //OCR 검사 요청 (Right Table)
    BcrOcrInr,              //2D코드+OCR 검사 요청 (InRail)
    BcrOcrGL,               //2D코드+OCR 검사 요청 (Left Table)
    BcrOcrGR,               //2D코드+OCR 검사 요청 (Right Table)
    //

    ChangeUserLevel = 81,   //사용자 권한 변경 요청
    ChangeUI,               //2D Vision SW의 현재 화면 변경 요청 (Auto Mode 화면 <--> Teaching Mode 화면) 
    ChkAlive = 99,          //2D Vision SW Alive Check 용
}
/// <summary>
/// BCR UDP Interface 사용 시 상호 통신 상태 체크 용
/// </summary>
public enum eBcrUdpStatus
{
    _0_Ready = 0,   //UDP Server ON, Vision SW ON  :: Vision SW로 Command를 전송할 수 없는 상태 (Camera 연결 끊김 등 Vision SW에서 Comman 처리할 수 없는 상태, Command 전송 불가)
    _1_Train,       //UDP Server ON, Vision SW ON  :: Vision SW가 현재 Teaching Mode (Command 전송 불가)
    _2_Auto,        //UDP Server ON, Vision SW ON  :: Vision SW로 Command를 전송 가능 상태 (Command 전송 후 수신 완료 상태 포함) 
    _3_Running,     //UDP Server ON, Vision SW ON  :: Vision SW가 검사 진행 중인 상태 (Command 전송 불가)
    _4_Alarm,       //UDP Server ON, Vision SW ON  :: Vision SW Alarm 상태 (Command 전송 불가) //20200314 jhc :

    _98_SvrOn = 98, //UDP Server ON, Vision SW OFF :: 주기 Report 수신 못하고 있는 상태 (Vision SW 미 실행 상태일 가능성 있음)
    _99_None = 99,  //UDP Server OFF 상태
}
#endregion
//
//200406 jym : 추가
/// <summary>
/// Measure 시 현재 측정 단계 Before/After
/// </summary>
public enum EMeaStep
{
    Before,
    After
}
//
//20200415 jhc : Picker Idle Time 개선
/// <summary>
/// Picker Idle Time 개선 기능 사용/비사용 : Auto-Running && STEP 모드에서만 동작함
/// </summary>
public enum ePickerTimeImprove
{
    NotUse = 0,
    Use = 1
}
//20200513 jym :
/// <summary>
/// 매거진 배출 위치 
/// </summary>
public enum EMgzWay
{
    Btm = 0,
    Top = 1
}
//20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드, SPIL)
/// <summary>
/// KEYENCE BCR 위치
/// </summary>
public enum EKeyenceBcrPos
{
    OnLd  = 0,
    OffLd,
    MaxNum
}

//210827 syc : 2004U IV2
/// <summary>
/// IV2 카메라 위치
/// </summary>
public enum EIV2Pos
{
    ONP = 0,
    OFP
}


/// <summary>
/// KEYENCE BCR과 TCP 통신 단계
/// </summary>
public enum eKeyenceBcrTcpStatus
{
    READY = 0,      //Command 전송 전
    WAIT_CMD_RESP,  //Command 전송 완료 후 Response 대기 중
    WAIT_DATA_RESP, //Command Response 수신 완료 / Command Response 수신 완료 후 데이터 수신 대기
    DATA_COMPLETE,  //데이터 수신 완료
}
/// <summary>
/// KEYENCE BCR 전송 Command ("LON\r" / "LOFF\r" / "BCLR\r")
/// </summary>
public enum eKeyenceBcrCmd
{
    LON = 0, //Timing ON
    LOFF,    //Timing OFF
    BCLR     //Buffer Clear   
}

public enum EUnit
{
    Unit2 = 0,
    Unit4 = 1
}

public enum eMsg
{
    Notice,
    Warning,
    Error,
    Change,
    Query,      //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 Grinding/Strip Measure 시 18 Point 진행여부 질의를 위해 추가
    QueryAndNo  //2021.06.09 lhs : 일정시간 후 NO (Cancel) 리턴
}


// 2020.10.12 SungTae Start : Add
public enum EStepCnt
{
    DEFAULT_STEP = 12,
    STEP_3 = 3      // Only use Qorvo-BJ & DZ
}

public enum EDrsMea
{
    MeaPoint1 = 1,
    MeaPoint5 = 5
}
// 2020.10.12 SungTae End

// 2020.10.21 SungTae Start : Add
public enum EMeaPoint
{
    CENTER,
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}
// 2020.10.21 SungTae End

/// 2021.05.18 SungTae Start : Add
/// <summary>
/// X_Type    : Inrail에 있음. Ase Kr, Sky Works(OCR)
/// Picker   : Picker에 있음. Qorvo , Ase K26       , 
/// Not Uses : JSCK
/// </summary>
public enum eEqSeries
{
    U_2001,
    U_2003
}
/// 2021.05.18 SungTae End

// 2021.07.02 SungTae Start : [추가] 고객사(ASE-KR) 요청으로 Strip Cleaning Mode 추가
public enum eCleanMode
{
    BASIC,
    ROTATION
}
// 2021.07.02 SungTae End

// 2021-05-06, jhLee : Multi-LOT, LOT의 진행 상태
public enum ELotState
{
    eWait       = 0,            // 투입 대기중
    eRun        = 1,            // 투입 진행 중
    eClose      = 2,            // 투입 종료 (작업 진행중)
    eComplete   = 3,            // 작업 종료
    eAbort      = 4,            // User에 의한 투입 종료   // 2021.08.30 SungTae : [추가]
    eError      = 99,           // 문제가 발생한 이상 상태이다.
}


// 2021-07-05, jhLee : Multi-LOT, 투입시 LOT을 구분하는 모드
public enum ELotEndMode
{
    eEmptySlot      = 0,        // 연속 빈 Slot 으로 구분 (기본)
    eStripQty       = 1,        // 지정된 Strip 수량 투입시 LOT 종료
    eMgzQty         = 2,        // 지정된 Magazine 수량 투입시 LOT 종료
    eStripMgzQty    = 3,        // Strip 수량우선으로 체크, 이후 Magazine 수량을 보조로 체크
}


// 투입되던 LOT의 투입종료 여부를 사용자에게 묻고 회답을 받아 반영하도록 한다.
public enum ELotUserConfirm
{
    eNone       = 0,        // 아무런 상태도 아니다
    eQuestion   = 1,        // 질문을 하고있다.
    eWaitReply  = 2,        // 답변을 기다리고 있다.
    eReplyYes   = 3,        // Yes 응답을 받았다. LOT을 종료시키라는 응답
    eReplyNo    = 4,        // No 응답을 받았다.  LOT을 유지하고 투입을 계속하라는 응답
}

// 2021.10.04 lhs Start : OnLoader의 Mgz Pick을 할 것인지, WorkEnd를 할 것 인지 묻는 메시지
public enum EOnLUserConfirm
{
    eNone       = 0,        // 아무런 상태도 아니다
    eQuestion   = 1,        // 질문을 하고있다.
    eWaitReply  = 2,        // 답변을 기다리고 있다.
    eReplyYes   = 3,        // Yes 응답을 받았다. WorkEnd를 하라는 응답
    eReplyNo    = 4,        // No 응답을 받았다.  Mgz Pick을 계속하라는 응답
}
// 2021.10.04 lhs End

// 22.06.27 BaeJoo  Secsgem 측정결과 배열확인 용이하도록 추가 
public enum EPoint
{
    DF = 0,
    L_Before = 1,
    Onepoint = 2,
    SecondPoint = 3,
    L_After = 4,
    R_Before = 5,
    R_After = 6,

};
// 2021.08.10 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
public enum EDataShift
{
    EQ_READY        = 0,    // 설비 가동 전
    ONL_MGZ_PICK    = 1,    // On-Loader MGZ Pick Data
    ONL_STRIP_PUSH  = 2,    // In-Rail Load Strip Data (Strip Push -> Backward)
    INR_DF_MEAS     = 3,    // In-Rail DF Measure Data
    INR_BCR_READ    = 4,    // In-Rail BCR Data
    ONP_STRIP_PICK  = 5,    // On-Picker Strip Pick Data (Strip Pick -> Vacuum Sensor On 후)
    GRL_TBL_CHUCK   = 6,    // Left Table Data(ONP -> Left Table)
    GRL_BF_MEAS     = 7,    // Left Table Before Measure Data
    GRL_ONE_MEAS    = 8,    // Left Table One-Point Measure Data
    GRL_2ND_MEAS    = 9,    // Left Table 2'nd-Point Measure Data
    GRL_AF_MEAS     = 10,   // Left Table After Measure Data
    OFP_LT_PICK     = 11,   // Off-Picker Data (Left Table Strip -> OFP Pick)
    GRR_TBL_CHUCK   = 12,   // Right Table Data(ONP -> Right Table)
    GRR_BF_MEAS     = 13,   // Right Table Before Measure Data
    GRR_ONE_MEAS    = 14,   // Right Table One-Point Measure Data
    GRR_2ND_MEAS    = 15,   // Right Table 2'nd-Point Measure Data
    GRR_AF_MEAS     = 16,   // Right Table After Measure Data
    OFP_RT_PICK     = 17,   // Off-Picker Data (Right Table Strip -> OFP Pick)
    OFP_CLEAN       = 19,   // Off-Picker Strip Cleaning Data
    OFP_PLACE       = 20,   // Off-Picker Place Data (OFP -> DRY)
    DRY_WORK        = 21,   // Dry Working Data
    QC_JUDGE        = 22,   // QC-Vision Inspection Data (GOOD or NG)
    OFL_STRIP_OUT   = 23,   // Off-Loader Strip Out Data (Dry or QC-Vision -> OFL MGZ)
    OFL_MGZ_PLACE   = 24,   // Off-Loader MGZ Place Data
    LOT_END         = 25,   // Off-Loader Data
}
// 2021.08.10 SungTae End

// 2021.10.25 SungTae Start : [추가] 코드 확인 용이하도록 추가
public enum eTypeLDStop
{
    Inrail,
    Table
}
// 2021.10.25 SungTae End

// 2022.04.18 SungTae Start : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련 MGZ Loading Or Unloading 시 Track Out 상태 Flag
public enum eTrackOutFlag
{
    Error = -1,
    Valid,
}
// 2022.04.18 SungTae End

