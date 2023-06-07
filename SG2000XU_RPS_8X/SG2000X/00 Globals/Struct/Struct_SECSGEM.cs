using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
///  20191101 LCY SECS Interface
/// </summary>

public struct TSecsgem_Data
{
    public int nComm_Flag;      // 1: Connection, 2: Not Connection
    public int nConnect_Flag;   // 1: Connection, 2: Not Connection
    public int nRemote_Flag;    // 4: Local,      5: Remote;

    public Queue<int>       qError_Count;
    public Queue<string>    qTerminal_Msg;
    public Queue<string>    qTerminal_Log;
    public string   sTerminal_Msg;
    public int      nLdMGZ_Check;   // Loader MGZ Verify 미진행 = 1, 완료시 = 2, Error = -1
    public int      nMGZ_Check;     // Verify 미진행 = 1, 완료시 = 2, Error = -1
    public int      nStrip_Check;   //  0 = 초기 값
                                    // -1 = 요청 하지 않았는데 Lot 값이 내려 옮
                                    // -2 = Lot Verify Error
                                    // -3 = Strip Verify Error
                                    //  1 = Strip Verify 시작시 시퀸스 초기화 하며 1로 설정
                                    //  2 = Lot Verify 성공
                                    //  3 = Strip Verify 성공
    public int      nLot_Check;     // Verify 미진행 = 1, 완료시 = 2, Error = -1

    // nStrip_Check = 0, nLot_Check = 0 ==> 
    // nStrip_Check = 0, nLot_Check = 1 ==> 
    // nStrip_Check = 1, nLot_Check = 1,        
}

public class JSCK_SECSGEM_STRIP_DATA
{
    public string       sSaBUN;                     // 작업자 사번
    public string       sCurr_Recipe_Name;          // Strip 투입시의 Recipe Name
    public string       sNew_Recipe_Name;           // Strip 투입시의 Recipe Name
    public string[]     sCurr_Wheel_Name            = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sNew_Wheel_Name             = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sCurr_Dress_Name            = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sNEW_Dress_Name             = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sCurr_Tool_Name             = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sNEW_Tool_Name              = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sCurr_Tool_Serial_Num       = new string[5];    // 0- Right, 1- Right ~~
    public string[]     sNEW_Tool_Serial_Num        = new string[5];    // 0- Right, 1- Right ~~
    public string       sLot_Name;                  // 작업 하는 Lot Name
    public string       sLD_MGZ_ID;                 // 투입되는 MZ RFID 읽은 후 보고한 값
    public string       sLD_MGZ_ID_New;             // 투입되는 MZ RFID Host에서 내려받은 값
    public int          nLD_MGZ_Cnt;                // 해당 Lot 의 몇 번째 MZ
    public int          nLD_MGZ_Strip_Cnt;          // 투입된 MZ 의 몇번째 Strip
    public int          nLot_Strip_Cnt;             // 해당 Lot 중 몇번째 Strip 
    public string       sInRail_Strip_ID;           // 읽은 Strip ID
    public string       sInRail_Verified_Strip_ID;  // 서버에서 전송 받은 Strip ID
    public int          nUnit_LEFT_Work_Start_Cnt;  // 해당 공정에 투입된 Strip Count
    public int          nUnit_LEFT_Work_End_Cnt;    // 해당 공정에 배출된 Strip count
    public int          nUnit_RIGHT_Work_Start_Cnt; // 해당 공정에 투입된 Strip Count
    public int          nUnit_RIGHT_Work_End_Cnt;   // 해당 공정에 배출된 Strip count
    public int          nUnit_TOTAL_Work_Start_Cnt; // 해당 공정에 투입된 Strip Count
    public int          nUnit_TOTAL_Work_End_Cnt;   // 해당 공정에 배출된 Strip count
    public int          nTable_Work_Location;       // 1- Right Table , 2- Right Table , 3- Right after Right 
    public double[,]    fMeasure_Data               = new double[7, GV.STRIP_MEASURE_POINT_MAX/*20*/];   // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    public int[]        nMeasure_Count              = new int[7];       // 측정 갯수 확인 필요
    public double[]     fMeasure_Min_Data           = new double[7];    // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After
    public double[]     fMeasure_Max_Data           = new double[7];    // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After
    public double[]     fMeasure_Avr_Data           = new double[7];    // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After

    // 2021.07.30 lhs Start : TopMold, BtmMold (SCK 전용)
    public double       dMeasure_TopMold_Max;       // 측정한 Top Mold Max
    public double       dMeasure_TopMold_Avg;       // 측정한 Top Mold Avg
    public double       dMeasure_BtmMold_Max;       // 측정한 Btm Mold Max
    public double       dMeasure_BtmMold_Avg;       // 측정한 Btm Mold Avg
    // 2021.07.30 lhs End

    public double[,]    fMeasure_Wheel_Left_Thick   = new double[2, 4]; // 0-Before(1'st, 2'nd, 3'rd, Total), 1-After(1'st, 2'nd, 3'rd, Total)
    public double[,]    fMeasure_Wheel_Right_Thick  = new double[2, 4]; // 0-Before(1'st, 2'nd, 3'rd, Total), 1-After(1'st, 2'nd, 3'rd, Total)
    public double[,]    fMeasure_Dress_Left_Thick   = new double[2, 4]; // 0-Before(1'st, 2'nd, 3'rd, Total), 1-After(1'st, 2'nd, 3'rd, Total)
    public double[,]    fMeasure_Dress_Right_Thick  = new double[2, 4]; // 0-Before(1'st, 2'nd, 3'rd, Total), 1-After(1'st, 2'nd, 3'rd, Total)

    // Host 에서 다운 받은 값
    public int          nDF_Use;                    // DF 사용 : 0=Use, 1=NotUse
    public int          nDF_New_Use;                // DF 사용 : 0=Use, 1=NotUse
    public int          nDF_User_Mode;              // DF 사용 Mode               : 0=Max, 1=Avr
    public int          nDF_User_New_Mode;          // Host 에서 받은 DF 사용 Mode : 0=Max, 1=Avr
    public double[,]    fMeasure_New_Data           = new double[7, GV.STRIP_MEASURE_POINT_MAX/*20*/];  // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
    public double[]     fMeasure_New_Min_Data       = new double[7];  // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After
    public double[]     fMeasure_New_Max_Data       = new double[7];  // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After
    public double[]     fMeasure_New_Avr_Data       = new double[7];  // 0=DF, 1=Before, 2=OnePoint, 3=SecondPoint, 4=after, 5=R_Before, 6=R_After

    public int          nStrip_Insp_Result;         // 비전 검사 Data 1- Good, ~~NG
    public string       sULD_MGZ_ID;                // Unload MZ RFID 읽은 후 보고한 값
    public string       sULD_MGZ_ID_New;            // Unload MZ RFID Host에서 내려받은 값
    public int          nULD_MGZ_Cnt;               // Lot 시작후 몇 번째 Unload MZ Count
    public int          nULD_MGZ_Strip_Cnt;         // Lot 시작후 몇 번째 Unload MZ Count
    public int          nULD_MGZ_Strip_Slot_Num;    // Unload 하고자 하는 MZ Slot Num  Add
    public int          nWork_Continue_Verified;    // -1=Error  1=작업 허가 요청, 2=진행중, 3=작업 완료
    public int          nULD_MGZ_Port;              // Unload된 Magazine의 Port 번호 

    public JSCK_SECSGEM_STRIP_DATA clone()
    {
        JSCK_SECSGEM_STRIP_DATA cl      = new JSCK_SECSGEM_STRIP_DATA();
      
        cl.sSaBUN                       = this.sSaBUN;              // 작업자 사번
        cl.sCurr_Recipe_Name            = this.sCurr_Recipe_Name;   // Strip 투입시의 Recipe Name
        cl.sNew_Recipe_Name             = this.sNew_Recipe_Name;    // Strip 투입시의 Recipe Name

        Array.Copy(this.sCurr_Wheel_Name,       cl.sCurr_Wheel_Name,        this.sCurr_Wheel_Name.Length);
        Array.Copy(this.sNew_Wheel_Name,        cl.sNew_Wheel_Name,         this.sNew_Wheel_Name.Length);
        Array.Copy(this.sCurr_Dress_Name,       cl.sCurr_Dress_Name,        this.sCurr_Dress_Name.Length);
        Array.Copy(this.sNEW_Dress_Name,        cl.sNEW_Dress_Name,         this.sNEW_Dress_Name.Length);
        Array.Copy(this.sCurr_Tool_Name,        cl.sCurr_Tool_Name,         this.sCurr_Tool_Name.Length);
        Array.Copy(this.sNEW_Tool_Name,         cl.sNEW_Tool_Name,          this.sNEW_Tool_Name.Length);
        Array.Copy(this.sCurr_Tool_Serial_Num,  cl.sCurr_Tool_Serial_Num,   this.sCurr_Tool_Serial_Num.Length);
        Array.Copy(this.sNEW_Tool_Serial_Num,   cl.sNEW_Tool_Serial_Num,    this.sNEW_Tool_Serial_Num.Length);

        cl.sLot_Name                    = this.sLot_Name;
        cl.sLD_MGZ_ID                   = this.sLD_MGZ_ID;                  // 투입되는 MZ RFID 읽은 후 보고한 값
        cl.sLD_MGZ_ID_New               = this.sLD_MGZ_ID_New;              // 투입되는 MZ RFID Host에서 내려받은 값
        cl.nLD_MGZ_Cnt                  = this.nLD_MGZ_Cnt;                 // 해당 Lot 의 몇 번째 MZ
        cl.nLD_MGZ_Strip_Cnt            = this.nLD_MGZ_Strip_Cnt;           // 투입된 MZ 의 몇번째 Strip
        cl.nLot_Strip_Cnt               = this.nLot_Strip_Cnt;              // 해당 Lot 중 몇번째 Strip 
        cl.sInRail_Strip_ID             = this.sInRail_Strip_ID;            // 읽은 Strip ID
        cl.sInRail_Verified_Strip_ID    = this.sInRail_Verified_Strip_ID;   // 서버에서 전송 받은 Strip ID
        cl.nUnit_LEFT_Work_Start_Cnt    = this.nUnit_LEFT_Work_Start_Cnt;   // 해당 공정에 투입된 Strip Count
        cl.nUnit_LEFT_Work_End_Cnt      = this.nUnit_LEFT_Work_End_Cnt;     // 해당 공정에 배출된 Strip count
        cl.nUnit_RIGHT_Work_Start_Cnt   = this.nUnit_RIGHT_Work_Start_Cnt;  // 해당 공정에 투입된 Strip Count
        cl.nUnit_RIGHT_Work_End_Cnt     = this.nUnit_RIGHT_Work_End_Cnt;    // 해당 공정에 배출된 Strip count
        cl.nUnit_TOTAL_Work_Start_Cnt   = this.nUnit_TOTAL_Work_Start_Cnt;  // 해당 공정에 투입된 Strip Count
        cl.nUnit_TOTAL_Work_End_Cnt     = this.nUnit_TOTAL_Work_End_Cnt;    // 해당 공정에 배출된 Strip count
        cl.nTable_Work_Location         = this.nTable_Work_Location;        // 1 = Right Table, 2 = Right Table , 3 = Right after Right 

        Array.Copy(this.fMeasure_Data,              cl.fMeasure_Data,               this.fMeasure_Data.Length);
        Array.Copy(this.nMeasure_Count,             cl.nMeasure_Count,              this.nMeasure_Count.Length);
        Array.Copy(this.fMeasure_Min_Data,          cl.fMeasure_Min_Data,           this.fMeasure_Min_Data.Length);
        Array.Copy(this.fMeasure_Max_Data,          cl.fMeasure_Max_Data,           this.fMeasure_Max_Data.Length);
        Array.Copy(this.fMeasure_Avr_Data,          cl.fMeasure_Avr_Data,           this.fMeasure_Avr_Data.Length);

        // 2021.07.30 lhs Start : 측정용 TopMold, BtmMold (SCK 전용)
        cl.dMeasure_TopMold_Max     = this.dMeasure_TopMold_Max;
        cl.dMeasure_TopMold_Avg     = this.dMeasure_TopMold_Avg;
        cl.dMeasure_BtmMold_Max     = this.dMeasure_BtmMold_Max;
        cl.dMeasure_BtmMold_Avg     = this.dMeasure_BtmMold_Avg;
        // 2021.07.30 lhs End

        Array.Copy(this.fMeasure_Wheel_Left_Thick,  cl.fMeasure_Wheel_Left_Thick,   this.fMeasure_Wheel_Left_Thick.Length);
        Array.Copy(this.fMeasure_Wheel_Right_Thick, cl.fMeasure_Wheel_Right_Thick,  this.fMeasure_Wheel_Right_Thick.Length);
        Array.Copy(this.fMeasure_Dress_Left_Thick,  cl.fMeasure_Dress_Left_Thick,   this.fMeasure_Dress_Left_Thick.Length);
        Array.Copy(this.fMeasure_Dress_Right_Thick, cl.fMeasure_Dress_Right_Thick,  this.fMeasure_Dress_Right_Thick.Length);

    // Host 에서 다운 받은 값
        cl.nDF_Use                  = this.nDF_Use;                 // DF 사용 : 0=Use, 1=NotUse
        cl.nDF_New_Use              = this.nDF_New_Use;             // DF 사용 : 0=Use, 1=NotUse
        cl.nDF_User_Mode            = this.nDF_User_Mode;           // DF 사용 Mode : 0=Max, 1=Avr
        cl.nDF_User_New_Mode        = this.nDF_User_New_Mode;       // Host 에서 받은 DF 사용 Mode : 0=Max, 1=Avr

        Array.Copy(this.fMeasure_New_Data,          cl.fMeasure_New_Data,           this.fMeasure_New_Data.Length);
        Array.Copy(this.fMeasure_New_Min_Data,      cl.fMeasure_New_Min_Data,       this.fMeasure_New_Min_Data.Length);
        Array.Copy(this.fMeasure_New_Max_Data,      cl.fMeasure_New_Max_Data,       this.fMeasure_New_Max_Data.Length);
        Array.Copy(this.fMeasure_New_Avr_Data,      cl.fMeasure_New_Avr_Data,       this.fMeasure_New_Avr_Data.Length);

        cl.nStrip_Insp_Result       = this.nStrip_Insp_Result;      // 비전 검사 Data 1- Good, ~~NG
        cl.sULD_MGZ_ID              = this.sULD_MGZ_ID;             // Unload MZ RFID 읽은 후 보고한 값
        cl.sULD_MGZ_ID_New          = this.sULD_MGZ_ID_New;         // Unload MZ RFID Host에서 내려받은 값
        cl.nULD_MGZ_Cnt             = this.nULD_MGZ_Cnt;            // Lot 시작후 몇 번째 Unload MZ Count
        cl.nULD_MGZ_Port            = this.nULD_MGZ_Port;           // Unload된 Magazine의 Port 번호 
        cl.nULD_MGZ_Strip_Cnt       = this.nULD_MGZ_Strip_Cnt;      // Lot 시작후 몇 번째 Unload MZ Count
        cl.nULD_MGZ_Strip_Slot_Num  = this.nULD_MGZ_Strip_Slot_Num; // Unload 하고자 하는 MZ Slot Num  Add
        cl.nWork_Continue_Verified  = this.nWork_Continue_Verified; // -1=Error  1=작업 허가 요청, 2=진행중, 3=작업 완료

        return cl;
    }
}
//    const int JSCK_Max_Cnt = 30;
//    public JSCK_SECSGEM_STRIP_DATA[] JSCK_Gem_Data = new JSCK_SECSGEM_STRIP_DATA[JSCK_Max_Cnt];

// 0  - 설비 가동 전 변경
//      ~~~~ CEID ~~조건 
// 1  - LD Elevator가 MZ 에서 Strip 투입 전
// 2  - Inrail 에 Strip Push  -> Back 후
// 3  - DF Measure 후
// 4  - Strip id 읽음
//    ~~~~ CEID ~~조건 
// 5  - Onp Picker 가 Strip Pickup 하여 진공 센서 On 후

// 6  - Left Chuck 이 Strip 진공 센서 On 후
// 7  - Left Strip Befor Measure  후 
//      Grinding CEID
// 8  - Left Strip OnePoint Measure  후
//      redoing 
// 9  - Left Strip SecondPoint Measure  후
// 10 - Left Strip After Measure  후
//      Work End CEID
// 11 - Left Table Strip 를 Pick'Up 후

// 12 - Right Chuck 이 Strip 진공 센서 On 후
// 13 - Right Strip Befor Measure  후
//      Grinding CEID 
// 14 - Right Strip OnePoint Measure  후
// 15 - Right Strip SecondPoint Measure  후
// 16 - Right Strip After Measure  후
//      Work End CEID
// 17 - Right Table Strip 를 Pick'Up 후

// 18 - Ud Picker Right Chuck 에서 진공 센서 On 후
// 19 - Ud Picker Cleaning 시작 시
// 20 - Unloader Rail 에 Strip Place 후
// 21 - Unloader Dry Zone 에 작업 시작 후
// 22- Unloader 비전 검사 후
//  ~~ 조건 CEID 
// 23 - Unloader MZ 에 Strip 투입시
// 24 - Unloader MZ 에 배출시
// 25 - Lot End
//     CEID 


/// <summary>
/// SECSGEM    Equipment Status
/// </summary>
public struct tSG_ES
{
    /// <summary>
    /// int    Previous Equipment Status   0 : Idle, 1 : Run, 2 : Ready(Done), 4 : Down
    /// </summary>
    public static int iP;
    /// <summary>
    /// int    Current Equipment Status    0 : Idle, 1 : Run, 2 : Ready(Done), 4 : Down
    /// </summary>
    public static int iC;
}

public struct tSG
{
    public static tSG_ES SG_ES;
}
