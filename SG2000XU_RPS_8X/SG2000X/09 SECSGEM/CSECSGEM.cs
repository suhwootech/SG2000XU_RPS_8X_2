using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace SECSGEM
{
    #region Param
    public class Param
    {
        public short nCountOfPara = 0;
        List<string> arrCode = new List<string>();
        List<string> arrPara = new List<string>();

        public void AddPara(string sCode, string sPara)
        {
            nCountOfPara++;
            arrCode.Add(sCode);
            arrPara.Add(sPara);
        }

        public void ClearPara()
        {
            for (int i = 0; i < nCountOfPara; i++)
            {
                arrCode.RemoveAt(0);
                arrPara.RemoveAt(0);
            }

            nCountOfPara = 0;
        }

        public short GetParaCount()
        {
            return nCountOfPara;
        }

        public string GetCode(int nNo)
        {
            string sRet = string.Empty;

            if (nNo < nCountOfPara)
            { sRet = arrCode[nNo]; }
            else
            { sRet = string.Empty; }

            return sRet;
        }

        public string GetPara(int nNo)
        {
            string sRet = string.Empty;

            if (nNo < nCountOfPara)
            { sRet = arrPara[nNo]; }
            else
            { sRet = string.Empty; }

            return sRet;
        }
    }
    #endregion Param

    #region ManagerLOT
    //=========================================================
    public static class TargetName
    {
        public const string WNDNAME = "Gem";

    }



    public class ManagerLot
    {
        public string m_strLotId = "";
        public string m_strMgzId = "";
        List<Carrier> m_pCarrierList = new List<Carrier>();

        public int Count
        {
            get { return m_pCarrierList.Count; }
        }


        public void SetLotID(string strLotId)
        {
            m_strLotId = strLotId;
        }

        public void SetMgzID(string strMgzId)
        {
            m_strMgzId = strMgzId;
        }

        public bool ClearCarrier()
        {
            if (m_pCarrierList.Count > 0)
            {
                m_pCarrierList.Clear();
                m_strLotId = "";
                m_strMgzId = "";
                return true;
            }
            else
                return false;
        }

        public string GetCarrierID(int i)
        {
            if (m_pCarrierList.Count > 0) // Carrier가 Add된 경우.
            {
                if (i < m_pCarrierList.Count && i > -1)
                {
                    try
                    {
                        return m_pCarrierList[i].strCarrierId;
                    }
                    catch (System.IndexOutOfRangeException e)  // CS0168
                    {
                        return e.Message;
                    }
                }
                else
                    return "";
            }
            else
            {
                return "";   // 리스트의 값이 0인 경우 배열에 접근하지 않는다
                             // 2020 05 13 skpark 예외처리( ""으로 Add한 경우 해당 리스트에 Carrier 가 Add되지 않음 )
            }
        }

        public int GetCarrierStatus(int i)
        {
            if (i <= m_pCarrierList.Count && i > -1)
            {
                return m_pCarrierList[i].nCarrierStatus;
            }
            else
                return -1;
        }

        public bool AddCarrier(string strCarrier)
        {
           // strCarrier = strCarrier.Replace(" " , "" ); //공백제거   20200131 LCY          
            if (IsExsitCarrier(strCarrier) == false && strCarrier != "" )
            {
                Carrier pCar = new Carrier();
                pCar.nCarrierStatus = (int)JSCK.eCarrierStatus.Idle;
                pCar.strCarrierId = strCarrier;
                m_pCarrierList.Add(pCar);

                return true;
            }
            return false;
        }

        public bool AddCarrier(List<string> pCarrierList)
        {
            for (int i = 0; i < pCarrierList.Count; i++)
            {
                if (IsExsitCarrier(pCarrierList[i]) == false)
                {
                    Carrier pCar = new Carrier();
                    pCar.nCarrierStatus = (int)JSCK.eCarrierStatus.Idle;
                    pCar.strCarrierId = pCarrierList[i];
                    m_pCarrierList.Add(pCar);

                    return true;
                }
            }
            return false;
        }

        public bool AddCarrier(ManagerLot pManager)
        {
            for (int i = 0; i < pManager.m_pCarrierList.Count; i++)
            {
                if (IsExsitCarrier(pManager.m_pCarrierList[i].strCarrierId) == false)
                {
                    Carrier pCar = new Carrier();
                    pCar.strCarrierId = pManager.m_pCarrierList[i].strCarrierId;
                    pCar.nCarrierStatus = pManager.m_pCarrierList[i].nCarrierStatus;
                    m_pCarrierList.Add(pCar);

                    return true;
                }
            }
            return false;
        }

        public Carrier GetCarrier(string strCarrier)
        {
            for (int i = 0; i < m_pCarrierList.Count; i++)
            {
                if (m_pCarrierList[i].strCarrierId == strCarrier)
                {
                    return m_pCarrierList[i];
                }
            }
            return null;
        }

        public bool IsExsitCarrier(string strCarrier)
        {
            for (int i = 0; i < m_pCarrierList.Count; i++)
            {
                if (m_pCarrierList[i].strCarrierId == strCarrier)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SetStartTime(string strCarrier)
        {

            string strTime = "";
            DateTime now = DateTime.Now;
            strTime = now.ToString("yyyyMMddHHmmss");

            for (int i = 0; i < m_pCarrierList.Count; i++)
            {
                if (m_pCarrierList[i].strCarrierId == strCarrier)
                {
                    m_pCarrierList[i].strCarrierStartTime = strTime;
                    return true;
                }
            }
            return false;
        }

        public string GetStartTime(string strCarrier)
        {
            for (int i = 0; i < m_pCarrierList.Count; i++)
            {
                if (m_pCarrierList[i].strCarrierId == strCarrier)
                {
                    return m_pCarrierList[i].strCarrierStartTime;
                }
            }
            return "";
        }

        public bool SetStripStatus(string strCarrier, int nStatus)
        {
            for (int i = 0; i < m_pCarrierList.Count; i++)
            {
                // 2022.04.14 SungTae Start : [추가] (ASE-KH VOC) Multi-LOT(Auto-In/Out) 관련 조건 추가
                if(strCarrier == string.Empty)
                {
                    m_pCarrierList[i].nCarrierStatus = nStatus;
                    return true;
                }
                // 2022.04.14 SungTae End

                if (m_pCarrierList[i].strCarrierId == strCarrier)
                {
                    m_pCarrierList[i].nCarrierStatus = nStatus;
                    return true;
                }
            }
            return false;
        }

        public string GetCarrierList()
        {
            string strReturn = "";
            List<string> pList = new List<string>();

            for (int i = 0; i < m_pCarrierList.Count; i++)
            {
                pList.Add(m_pCarrierList[i].strCarrierId);
            }
            strReturn = string.Join("|", pList);
            pList.Clear();

            return strReturn;

        }
    }

    public class Carrier /* Carrier */
    {
        public string strCarrierId = "";
        public string strCarrierStartTime = "";
        public int nCarrierStatus = (int)SECSGEM.JSCK.eCarrierStatus.Idle; // CarrierStatus.IDLE
    }

    public class Parm
    {
        public short nCountOfParam = 0;
        List<string> arrCcode = new List<string>();
        List<string> arrParm = new List<string>();

        public void AddParam(string strCode, string strParm)
        {
            nCountOfParam++;
            arrCcode.Add(strCode);
            arrParm.Add(strParm);
        }

        public void Clear()
        {
            for (int i = 0; i < nCountOfParam; i++)
            {
                arrCcode.RemoveAt(0);
                arrParm.RemoveAt(0);
            }
            nCountOfParam = 0;

        }
        public short GetParamCount()
        {
            return nCountOfParam;
        }

        public string GetCcode(int nNo)
        {
            if (nNo < nCountOfParam) return arrCcode[nNo];
            else return "";
        }
        public string GetParm(int nNo)
        {
            if (nNo < nCountOfParam) return arrParm[nNo];
            else return "";
        }



    }
    #endregion ManagerLOT

    #region GEM
    public static class GEM
    {
        /// <summary>
        /// 0 = red, 1 = cyan
        /// </summary>
        public static Color[] clrA_RC = new Color[2] { Color.Red, Color.Cyan };
        /// <summary>
        /// 0 = white, 1 = red, 2 = cyan
        /// </summary>
        public static Color[] clrA_WRC = new Color[3] { Color.White, Color.Red, Color.Cyan };
        /// <summary>
        /// 0 = white, 1 = red, 2 = yellow, 3 = cyan
        /// </summary>
        public static Color[] clrA_RYC = new Color[6] { Color.White, Color.Red, Color.Red, Color.Red, Color.Yellow, Color.Cyan };

        /// <summary>
        /// 0 = Yellow, 1 = Green, 2 = Cyan, 3 = Cyan, 4 = Red
        /// </summary>
        public static Color[] clrA_YGCR = new Color[5] { Color.Yellow, Color.Green, Color.Cyan, Color.Cyan, Color.Red };

        //
        public static String[] strA_Comm = new String[3] { "Disable", "NotCommunicating", "Communicating" };
        //
        public static String[] strA_Cont = new String[6] { "Unknown", "Offline", "HostOffline", "AttempToOnline", "Local", "Remote" };
        //
        public static String[] strA_Conn = new String[2] { "Disconnected", "Connected" };
        //
        public static String[] strA_PS = new String[5] { "Idle", "Run", "Ready", "Done", "Down" };



        /// <summary>
        /// bool Exist Form
        /// </summary>
        public static bool bExistForm = false;


        #region status
        /// <summary>
        /// Control State    1 = Offline, 2 = Local, 3 = Remote
        /// </summary>
        public static int iCS = 0;
        /// <summary>
        /// Previous Control State    1 = Offline, 2 = Local, 3 = Remote
        /// </summary>
        public static int iPCS = 0;

        /// <summary>
        /// Equipment State    0 = Unknown, 1 = Idle, 2 = Ready, 3 = Run, 4 = Down
        /// </summary>
        public static int iES = 0;
        /// <summary>
        /// Previous Equipment State    0 = Unknown, 1 = Idle, 2 = Ready, 3 = Run, 4 = Down
        /// </summary>
        public static int iPES = 0;

        /// <summary>
        /// Communication Sataus    1 = Disabled, 2 = Not Communicating, 3 = Communicating
        /// </summary>
        public static int iComm = 0;

        /// <summary>
        /// PPID
        /// </summary>
        public static string sPPID = string.Empty;
        /// <summary>
        /// Changed PPID
        /// </summary>
        public static string sCPPID = string.Empty;

        /// <summary>
        /// ALID
        /// </summary>
        public static int iALID = 0;

        /// <summary>
        /// Tool ID
        /// </summary>
        public static string sTID = string.Empty;

        /// <summary>
        /// LOT ID
        /// </summary>
        public static string sLOTID = string.Empty;

        /// <summary>
        /// Load/Unload Tool ID    false = Unload, true = Load
        /// </summary>
        public static bool bLTID = false;

        /// <summary>
        /// Reading/Manual Key in LOT ID    false = not, true = done
        /// </summary>
        public static bool bRMLOT = false;

        /// <summary>
        /// Reading LOT ID    false = not, true = done
        /// </summary>
        public static bool bRLOT = false;

        /// <summary>
        /// Manual Key in LOT ID    false = not, true = done
        /// </summary>
        public static bool bMLOT = false;

        /// <summary>
        /// Open/End LOT    false = End, true = Open
        /// </summary>
        public static bool bOLOT = false;

        public static int iMGZ_MAX = 25;
        #endregion status


        #region Path
        /// <summary>
        /// host
        /// </summary>
        public static readonly string sPath_Host = Application.StartupPath + "\\Device\\HOST";
        /// <summary>
        /// using
        /// </summary>
        public static readonly string sPath_Using = Application.StartupPath + "\\Device";
        //public static readonly string sPath_Using = Application.StartupPath + "\\Device\\GEM";
        /// <summary>
        /// test
        /// </summary>
        public static readonly string sTest_Recipe = "TestRecipe";
        #endregion Path

        #region String - define
        /// <summary>
        /// find string - ALID
        /// </summary>
        public static readonly string sALID = "ALID";
        /// <summary>
        /// find string - CEID
        /// </summary>
        public static readonly string sCEID = "CEID";
        /// <summary>
        /// find string - DATAID
        /// </summary>
        public static readonly string sDATAID = "DATAID";
        /// <summary>
        /// find string - RPTID
        /// </summary>
        public static readonly string sRPTID = "RPTID";
        /// <summary>
        /// find string - SVID
        /// </summary>
        public static readonly string sSVID = "SVID";
        /// <summary>
        /// find string - TRACEID
        /// </summary>
        public static readonly string sTRACEID = "TRACEID";
        /// <summary>
        /// find string - RCMD
        /// </summary>
        public static readonly string sRCMD = "RCMD";
        /// <summary>
        /// find string - RCMD_ACK
        /// </summary>
        public static readonly string sRCMD_ACK = "RCMD_ACK";
        /// <summary>
        /// find string - RCP
        /// </summary>
        public static readonly string sRCP = "RCP";
        /// <summary>
        /// find string - A
        /// </summary>
        public static readonly string sA = "A";
        /// <summary>
        /// find string - F4
        /// </summary>
        public static readonly string sF4 = "F4";
        /// <summary>
        /// find string - L
        /// </summary>
        public static readonly string sL = "L";
        /// <summary>
        /// find string - Object
        /// </summary>
        public static readonly string sObject = "OBJECT";
        /// <summary>
        /// find string - U1
        /// </summary>
        public static readonly string sU1 = "U1";
        /// <summary>
        /// find string - U2
        /// </summary>
        public static readonly string sU2 = "U2";
        /// <summary>
        /// find string - U4
        /// </summary>
        public static readonly string sU4 = "U4";
        /// <summary>
        /// find string - SECOND
        /// </summary>
        public static readonly string sSECOND = "SECOND";
        /// <summary>
        /// find string - 0
        /// </summary>
        public static readonly string s0 = "0";
        /// <summary>
        /// find string - 1
        /// </summary>
        public static readonly string s1 = "1";
        /// <summary>
        /// find string - 255
        /// </summary>
        public static readonly string s255 = "255";
        /// <summary>
        /// find string - 9999
        /// </summary>
        public static readonly string s9999 = "9999";

        /// <summary>
        /// define string - wnd name - target name
        /// </summary>
        public const string sWND = "SECSGEM";

        #endregion String - define

        #region HSMS
        /// <summary>
        /// HSMS - Internal port
        /// </summary>
        public const int iHSMS_InternalPort = 8080;
        /// <summary>
        /// HSMS - Max log count
        /// </summary>
        public const int iHSMS_MaxLog = 100;
        /// <summary>
        /// HSMS - HSMS
        /// </summary>
        public static readonly string sHSMS_HSMS = "HSMS";
        /// <summary>
        /// HSMS - GEM
        /// </summary>
        public static readonly string sHSMS_GEM = "GEM";
        /// <summary>
        /// HSMS - CONFIGURATION
        /// </summary>
        public static readonly string sHSMS_CONFIGURATION = "CONFIGURATION";
        /// <summary>
        /// HSMS - HSMS port
        /// </summary>
        public static readonly string sHSMS_Port = "Port";
        /// <summary>
        /// HSMS - Device ID
        /// </summary>
        public static readonly string sHSMS_DeviceID = "DeviceID";
        /// <summary>
        /// HSMS - T3
        /// </summary>
        public static readonly string sHSMS_T3 = "T3";
        /// <summary>
        /// HSMS - T5
        /// </summary>
        public static readonly string sHSMS_T5 = "T5";
        /// <summary>
        /// HSMS - T6
        /// </summary>
        public static readonly string sHSMS_T6 = "T6";
        /// <summary>
        /// HSMS - T7
        /// </summary>
        public static readonly string sHSMS_T7 = "T7";
        /// <summary>
        /// HSMS - T8
        /// </summary>
        public static readonly string sHSMS_T8 = "T8";
        /// <summary>
        /// HSMS - HSMS link test
        /// </summary>
        public static readonly string sHSMS_LinkTest = "LinkTest";
        /// <summary>
        /// HSMS - HSMS passive
        /// </summary>
        public static readonly string sHSMS_Passive = "Passive";
        /// <summary>
        /// HSMS - HSMS establish timeout
        /// </summary>
        public static readonly string sHSMS_EstTimeout = "Est timeout";
        /// <summary>
        /// HSMS - Default control
        /// </summary>
        public static readonly string sHSMS_DefaultControl = "DefaultControlState";
        /// <summary>
        /// HSMS - Communication request timeout
        /// </summary>
        public static readonly string sHSMS_RequestTimeout = "CommunicationRequestTimeout";
        /// <summary>
        /// HSMS - MDLN
        /// </summary>
        public static readonly string sHSMS_MDLN = "MDLN";
        /// <summary>
        /// HSMS - Softrev
        /// </summary>
        public static readonly string sHSMS_Softrev = "Softrev";
        /// <summary>
        /// HSMS - UseRecipeFolder
        /// </summary>
        public static readonly string sHSMS_UseRecipeFolder = "UseRecipeFolder";
        public static readonly string sHSMS_RMSPath = "RMSPath";
        #endregion HSMS

        #region cp name
        /// <summary>
        /// cp name - ppid
        /// </summary>
        public const string sCPN_PPID = "PPID";
        /// <summary>
        /// cp name - lot id
        /// </summary>
        public const string sCPN_LOTID = "LOT_ID";
        /// <summary>
        /// cp name - strip id
        /// </summary>
        public const string sCPN_SID = "STRIPID";
        /// <summary>
        /// cp name - mat id
        /// </summary>
        public const string sCPN_MATID = "MAT_ID";
        /// <summary>
        /// cp name - tool id
        /// </summary>
        public const string sCPN_TID = "TOOL_ID";
        /// <summary>
        /// cp name - table loc
        /// </summary>
        public const string sCPN_TLoc = "TABLE_LOC";
        /// <summary>
        /// cp name - result
        /// </summary>
        public const string sCPN_Result = "RESULT";
        /// <summary>
        /// cp name - Qty
        /// </summary>
        public const string sCPN_Qty = "QTY";
        /// <summary>
        /// cp name - CarrierList
        /// </summary>
        public const string sCPN_CL = "CARRIER_LIST";
        /// <summary>
        /// cp name - carrier id
        /// </summary>
        public const string sCPN_CID = "CARRIER_ID";
        /// <summary>
        /// cp name - mgz id
        /// </summary>
        public const string sCPN_MGZID = "MGZ_ID";
        /// <summary>
        /// cp name - port no
        /// </summary>
        public const string sCPN_PN = "PORT_NO";

        // 2021-06-03 lhs Start : 변수명 변경 
        ///// <summary>
        ///// cp name - PCB 두께 평균 값을 Host에서 내려 받은 값
        ///// </summary>
        //public const string sCPN_DF_PCB_AVG = "REF_THK_AVG";  
        ///// <summary>
        ///// cp name - PCB 두께 MaX 값을 Host에서 내려 받은 값
        ///// </summary>
        //public const string sCPN_DF_PCB_MAX = "REF_THK_MAX";

        /// <summary>
        /// cp name - 평균값 : Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB or AF)
        /// </summary>
        public const string sCPN_REF_THK_AVG = "REF_THK_AVG";
        /// <summary>
        /// cp name - 최대값 : Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB or AF)
        /// </summary>
        public const string sCPN_REF_THK_MAX = "REF_THK_MAX";
        // 2021-06-03 lhs End

        // 2021-07-23 lhs Start : CPNAME 추가 (SCK 전용)
        /// <summary>
        /// cp name - PCB Avg : Eq에서 Host로 Upload한 값을 다시 받는 변수
        /// </summary>
        public const string sCPN_REF_PCB_THK_AVG = "REF_PCB_THK_AVG";
        /// <summary>
        /// cp name - PCB Max : Eq에서 Host로 Upload한 값을 다시 받는 변수
        /// </summary>
        public const string sCPN_REF_PCB_THK_MAX = "REF_PCB_THK_MAX";
        /// <summary>
        /// cp name - Top Mold Avg : Eq에서 Host로 Upload한 값을 다시 받는 변수
        /// </summary>
        public const string sCPN_REF_TOP_THK_AVG = "REF_TOP_THK_AVG";
        /// <summary>
        /// cp name - Top Mold Max : Eq에서 Host로 Upload한 값을 다시 받는 변수
        /// </summary>
        public const string sCPN_REF_TOP_THK_MAX = "REF_TOP_THK_MAX";
        /// <summary>
        /// cp name - Bottom Mold Avg : Eq에서 Host로 Upload한 값을 다시 받는 변수
        /// </summary>
        public const string sCPN_REF_BTM_THK_AVG = "REF_BTM_THK_AVG";
        /// <summary>
        /// cp name - Bottom Mold Max : Eq에서 Host로 Upload한 값을 다시 받는 변수
        /// </summary>
        public const string sCPN_REF_BTM_THK_MAX = "REF_BTM_THK_MAX";
        // 2021-07-23 lhs End

        #endregion cp name

        #region RCMD value
        ///// <summary>
        ///// RCMD - start
        ///// </summary>
        public const string sRCMD_Start = "START";
        ///// <summary>
        ///// RCMD - stop
        ///// </summary>
        public const string sRCMD_Stop = "STOP";
        ///// <summary>
        ///// RCMD - pause
        ///// </summary>
        //public const string sRCMD_Pause = "PAUSE";
        ///// <summary>
        ///// RCMD - local
        ///// </summary>
        //public const string sRCMD_Local = "LOCAL";
        ///// <summary>
        ///// RCMD - remote
        ///// </summary>
        //public const string sRCMD_Remote = "REMOTE";
        ///// <summary>
        ///// RCMD - resume
        ///// </summary>
        //public const string sRCMD_Resume = "RESUME";
        ///// <summary>
        ///// RCMD - pp select
        ///// </summary>
        public const string sRCMD_PP_SELECT = "PP-SELECT";
        ///// <summary>
        ///// RCMD - MAT_VERIFICATION
        ///// </summary>
        public const string sRCMD_MAT_VERIFICATION = "MAT_VERIFICATION";
        ///// <summary>
        ///// RCMD - TOOL_VERIFICATION
        ///// </summary>
        public const string sRCMD_TOOL_VERIFICATION = "TOOL_VERIFICATION";
        ///// <summary>
        ///// RCMD - LOT_VERIFICATION
        ///// </summary>
        public const string sRCMD_LOT_VERIFICATION = "LOT_VERIFICATION";
        ///// <summary>
        ///// RCMD - CARRIER_VERIFICATION
        ///// </summary>
        public const string sRCMD_CARRIER_VERIFICATION = "CARRIER_VERIFICATION";
        ///// <summary>
        ///// RCMD - MAGAZINE_VERIFICATION
        ///// </summary>
        public const string sRCMD_MGZ_VERIFICATION = "MAGAZINE_VERIFICATION";
        ///// <summary>
        ///// RCMD - WIP_LOAD
        ///// </summary>
        public const string sRCMD_WIP_LOAD = "WIP-LOAD";
        ///// <summary>
        ///// RCMD - WIP_REJECT
        ///// </summary>
        public const string sRCMD_WIP_REJECT = "WIP-REJECT";

        #endregion RCMD value
    }
    #endregion GEM
}
