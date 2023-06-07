using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EZGemPlusCS;
using System.Runtime.InteropServices;
using System.IO;

using SECSGEM;
using Microsoft.VisualBasic.FileIO;

namespace SG2000X
{
    public partial class frmGEM : Form
    {
        #region Variable
        public CEZGemPlusLib mgem = new CEZGemPlusLib();

        private System.Windows.Forms.Timer m_tmUpdt;

        public frmTM sgfrmMsg;

        // 2021.10.13 SungTae : [추가] Multi-LOT 관련
        //public frmTM2 sgfrmMsg2;
        private frmView2 m_fmView;

        public const Int32 WM_COPYDATA = 0x004A;
        public const Int32 WM_COPYMSG = 0x004B;
        string sMsgdata;// 로그 기록용 변수

        private static int nError_set_Flag;

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;    //int32 -> int
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        struct ECIDValue
        {//MSMSII
            public uint m_nPort;
            public uint m_nDeviceID;
            public uint m_nT3;
            public uint m_nT5;
            public uint m_nT6;
            public uint m_nT7;
            public uint m_nT8;
            public uint m_nLinkTestInterval;
            public uint m_nCommReqeustTimeout;            
            public bool m_bPassive;
            public string m_szModelName;
            public string m_szSoftRev;            
        }

        ECIDValue ECV = new ECIDValue();

        public bool UseRecipeFolder = false;
        public string RMSPath = string.Empty;


        FileInfo fileinfo = new FileInfo(Application.ExecutablePath);


        /// <summary>
        /// execute file path
        /// </summary>
        public string m_sExePath;
        /// <summary>
        /// int control state
        /// </summary>
        public int m_iCtrlState/*m_iCS*/ = Convert.ToInt32(JSCK.eCont.Offline);     // 2021.08.31 SungTae : [수정] 변수명 변경 (m_iCS -> m_iCtrlState)
        /// <summary>
        /// int connection state
        /// </summary>
        public int m_iConnState = Convert.ToInt32(JSCK.eConn.Disconnected);         // 2021.08.31 SungTae : [수정] 변수명 변경 (m_iConn -> m_iConnState)
        /// <summary>
        /// int communication state
        /// </summary>
        public int m_iCommState = Convert.ToInt32(JSCK.eComm.NotCommunicating);     // 2021.08.31 SungTae : [수정] 변수명 변경 (m_iComm -> m_iCommState)


        //gem
        /// <summary>
        /// int Process State
        /// </summary>
        public int m_iPS = 0;

        /// <summary>
        /// int Pre Process State
        /// </summary>
        public int m_iPrePS = 0;

        /// <summary>
        /// int Alarm Occur
        /// </summary>
        public int m_iAO = 0;

        /// <summary>
        /// int Alarm Reset
        /// </summary>
        public int m_iAR = 0;


        /// <summary>
        /// string    SVID 2301    Tool ID
        /// </summary>
        public string m_sTID = string.Empty;

        /// <summary>
        /// int MGZ Count
        /// </summary>
        public int m_iMGZC = 0;

        /// <summary>
        /// string    SVID 2101    LOT ID
        /// </summary>
        public string m_sLOTI = string.Empty;

        /// <summary>
        /// int    SVID 2105    Slot Number
        /// </summary>
        public int m_iSN = 0;

        /// <summary>
        /// string    SVID 2103    Strip ID
        /// </summary>
        public string m_sSID = string.Empty;


        //////
        /// <summary>
        /// 작업하는 carrier list
        /// </summary>
        public ManagerLot m_pStartCarrierList;
        /// <summary>
        /// host에서 받은 carrier list
        /// </summary>
        //public ManagerLOT m_pHostLOT;
        /// <summary>
        /// 읽은 carrier list
        /// </summary>
        public ManagerLot m_pReadCarrierList;

        // 2021.10.21 SungTae Start : [추가]
        /// <summary>
        /// Loading Carrier List
        /// </summary>
        public ManagerLot m_pLoadCarrierList;
        // 2021.10.21 SungTae End

        /// <summary>
        /// 읽은 carrier list - 임시 저장
        /// </summary>
        public string m_sReadLOT_T = string.Empty;
        /// <summary>
        /// validate carrier list
        /// </summary>
        public ManagerLot m_pValidCarrierList;
        /// <summary>
        /// 작업한 carrier list
        /// </summary>
        public ManagerLot m_pEndedCarrierList;
        /// <summary>
        /// reject한 carrier list
        /// </summary>
        public ManagerLot m_pRejectCarrierList;

        /// <summary>
        /// process complete list - mgz. ok
        /// </summary>
        public ManagerLot m_pMgzGoodCarrierList;
        /// <summary>
        /// process complete list - mgz. ng, reject
        /// </summary>
        public ManagerLot m_pMgzNGCarrierList;

        /// <summary>
        /// string    Current Carrier ID
        /// </summary>
        public string m_sCCarID = string.Empty;
        /// <summary>
        /// string    Current LOT ID
        /// </summary>
        public string m_sCLOTID = string.Empty;
        /// <summary>
        /// string    Current Host LOT ID
        /// </summary>
        public string m_sCHLOTID = string.Empty;
        /// <summary>
        /// string    Current Host Carrier ID
        /// </summary>
        public string m_sCHCID = string.Empty;
        /// <summary>
        /// string    Loading PPID
        /// </summary>
        public string m_sPPID = string.Empty;
        /// <summary>
        /// string    Request Tool ID 1
        /// </summary>
        public string m_sRTID1 = string.Empty;
        /// <summary>
        /// string    Request Tool ID 2
        /// </summary>
        public string m_sRTID2 = string.Empty;
        /// <summary>
        /// string    Request Material ID 1
        /// </summary>
        public string m_sRMID1 = string.Empty;
        /// <summary>
        /// string    Request Material ID 2
        /// </summary>
        public string m_sRMID2 = string.Empty;
        /// <summary>
        /// string    Request MGZ ID 1
        /// </summary>
        public string m_sRMGZID1 = string.Empty;
        /// <summary>
        /// string    Request MGZ ID 2
        /// </summary>
        public string m_sRMGZID2 = string.Empty;
        /// <summary>
        /// string    Request MGZ ID 3 (Unloader)
        /// </summary>
        public string m_sRMGZID3 = string.Empty;
        //////

        #region Current ID 저장 변수
        /// <summary>
        /// string    Current Tool ID 1
        /// </summary>
        public string m_sCurrTID1 = string.Empty;
        /// <summary>
        /// string    Current Tool ID 2
        /// </summary>
        public string m_sCurrTID2 = string.Empty;
        /// <summary>
        /// string    Current Material ID 1
        /// </summary>
        public string m_sCurrMID1 = string.Empty;
        /// <summary>
        /// string    Current Material ID 2
        /// </summary>
        public string m_sCurrMID2 = string.Empty;
        /// <summary>
        /// string    Current MGZ ID 1
        /// </summary>
        public string m_sCurrMGZID1 = string.Empty;
        /// <summary>
        /// string    Current MGZ ID 2
        /// </summary>
        public string m_sCurrMGZID2 = string.Empty;
        #endregion



        // Left 스트립측정맵    0 = 측정안함, 1 = 측정함
        // "10001000..."
        public string m_sCarrierMap_L = string.Empty;
        // Right 스트립측정맵    0 = 측정안함, 1 = 측정함
        // "10001000..."
        public string m_sCarrierMap_R = string.Empty;



        /// <summary>
        /// device list
        /// </summary>
        List<string> mDevice_List = new List<string>();

        

        public string m_strLotTime;

        static int nLoad_flag = 0;

        // 2020-11-09, jhLee, 
        public int m_nLotOpenCount = 0;                    // Lot Open이 진행되어 처리중인가 ? START/RESTART 구분용, 진행중인 LOT 수량 0이면 아직 없음

        #endregion Variable

        #region form event
        public frmGEM()
        {
            InitializeComponent();

            // 2021.10.13 SungTae Start : [수정] Multi-LOT 관련
            sgfrmMsg = new frmTM(this);

            //if (CData.IsMultiLOT()) { sgfrmMsg2 = new frmTM2(this); }
            //else                    { sgfrmMsg  = new frmTM(this); }
            // 2021.10.13 SungTae End

            GEM.bExistForm = true;

            m_pStartCarrierList     = new ManagerLot();
            m_pEndedCarrierList     = new ManagerLot();
            m_pReadCarrierList      = new ManagerLot();
            m_pRejectCarrierList    = new ManagerLot();
            m_pValidCarrierList     = new ManagerLot();

            m_pMgzGoodCarrierList   = new ManagerLot();
            m_pMgzNGCarrierList     = new ManagerLot();

            m_pLoadCarrierList      = new ManagerLot();     // 2021.10.21 SungTae : [추가]

            GEM_Initial_Load(); //191121

            m_tmUpdt = new System.Windows.Forms.Timer();
            m_tmUpdt.Interval = 1000;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
            m_tmUpdt.Start();

            Log_wt("frmGEM()");

            if (CData.Opt.bSecsUse)
            {
//                tmrGEM.Start();
                Start_GEM();
                Log_wt("GEM_Initial_Load() CData.Opt.bSecsUse = true End");
            }
        }

        public void frmGEM_Display()
        {
            Log_wt("frmGEM_Display()");

            // 2021.10.13 SungTae Start : [수정] Multi-LOT 관련
            //if (CData.IsMultiLOT())
            //{
            //    if (sgfrmMsg2.Visible == true)
            //    {
            //        Console.WriteLine("---- c0");
            //        sgfrmMsg2.Display_Hide();
            //    }
            //    else
            //    {
            //        Console.WriteLine("---- c1");
            //        sgfrmMsg2.Display();
            //        //sgfrmMsg.ShowGrid(m_pHostLOT);
            //    }
            //}
            //else
            {
                if (sgfrmMsg.Visible == true)
                {
                    Console.WriteLine("---- c0");
                    sgfrmMsg.Display_Hide();
                }
                else
                {
                    Console.WriteLine("---- c1");
                    sgfrmMsg.Display();
                    //sgfrmMsg.ShowGrid(m_pHostLOT);
                }
            }
            // 2021.10.13 SungTae End
        }

        private void GEM_Initial_Load()
        {
            Log_wt("GEM_Initial_Load()");

            //내부 이벤트를 받아옴
            mgem.OnEZGemEvent += new ON_EZGEM_EVENT(OnEventReceived);

            //Host로 부터 받은 메세지를 전달
            mgem.OnEZGemMsg += new ON_EZGEM_MSG(OnMsgReceived);

            //m_sExePath = fileinfo.Directory.FullName;
            //m_sExePath = GEM.sPath_Using;

            Set_ECID();
            Get_Config();
            Set_GEM();
            Set_Show();
            
            frmGEM_Initial_Data();
            tmrGEM.Interval = 30 * 1000;

            if (UseRecipeFolder)
            {
                m_sExePath = GV.PATH_DEVICE + RMSPath;
            }
            else
            {
                m_sExePath = GEM.sPath_Using;
            }
            Get_DeviceFileList();

            Log_wt("GEM_Initial_Load() CData.Opt.bSecsUse = true End  !!");
        }

        private void frmGEM_Load(object sender, EventArgs e)
        {
            //내부 이벤트를 받아옴
            //mgem.OnEZGemEvent += new ON_EZGEM_EVENT(OnEventReceived);

            //Host로 부터 받은 메세지를 전달
            //mgem.OnEZGemMsg += new ON_EZGEM_MSG(OnMsgReceived);

            //m_sExePath = fileinfo.Directory.FullName;
            /*
            m_sExePath = GEM.sPath_Using;

            Set_ECID();
            Get_Config();
            Set_GEM();
            Set_Show();
            Get_DeviceFileList();
            */
        }
        private void frmGEM_Initial_Data()
        { 
            tmrSet.Enabled = true;
            Log_wt("frmGEM_Initial_Data()");

            btnConn_Start.Enabled       = (m_iConnState == Convert.ToInt32(JSCK.eConn.Connected)) ? false : true;
            btnConn_Stop.Enabled        = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;

            lblComm_Status.Text         = JSCK.eComm.NotCommunicating.ToString();
            lblComm_Status.BackColor    = GEM.clrA_WRC[Convert.ToInt32(JSCK.eComm.NotCommunicating)];

            lblCont_Status.Text         = JSCK.eCont.Offline.ToString();
            lblCont_Status.BackColor    = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Offline)];

            lblConn_Status.Text         = JSCK.eConn.Disconnected.ToString();
            lblConn_Status.BackColor    = GEM.clrA_RC[Convert.ToInt32(JSCK.eConn.Disconnected)];

            btnCont_Offline.Enabled     = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;
            btnCont_Local.Enabled       = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;
            btnCont_Remote.Enabled      = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;

            m_sCLOTID   = CData.LotInfo.sLotName;       Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), m_sCLOTID);
            m_sRMGZID1  = CData.LotInfo.sGMgzId;        Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), m_sRMGZID1);
            m_sRTID1    = CData.LotInfo.sLToolId;       Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), m_sRTID1);
            m_sRTID2    = CData.LotInfo.sRToolId;       Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), m_sRTID2);
            m_sRMID1    = CData.LotInfo.sLDrsId;        Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), m_sRMID1);
            m_sRMID2    = CData.LotInfo.sRDrsId;        Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), m_sRMID2);

            ToolSerial_Number_Set(CData.LotInfo.sLTool_Serial_Num, (int)JSCK.eTableLoc.N1);
            ToolSerial_Number_Set(CData.LotInfo.sRTool_Serial_Num, (int)JSCK.eTableLoc.N2);
        }

        private void frmGEM_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        ~frmGEM()
        {
            // if (m_pStrip != null)
            // { m_pStrip = null; }

            tmrSet.Stop();
            Stop_GEM();

            tmrGEM.Dispose();
            tmrSet.Dispose();

            sgfrmMsg.Close();
            //sgfrmMsg2.Close();      // 2021.10.13 SungTae : [추가] Multi-LOT 관련

            m_pStartCarrierList = null;
            mgem.Stop();
        }
        #endregion form event

        #region Timer
        private void tmrGEM_Tick(object sender, EventArgs e)
        {         
            tmrGEM.Stop();
        }

        private void tmrSet_Tick(object sender, EventArgs e)
        {
            string stime = string.Empty;
            DateTime dtnow = DateTime.Now;

            stime = dtnow.ToString("yyyyMMddHHmmss00");    //SECSGEM Spec
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Clock), stime); }
            else { mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Clock), stime); }
        }

        private void SetSVValue()
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardB)
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Left_Actual_Table_Speed), CMot.It.Get_Vel((int)EAx.LeftGrindZone_Y).ToString("0.0"));
                // 2023.03.15 Max
                //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Left_Actual_Spindle_Speed), CSpl.It.Get_Frpm(EWay.L).ToString());
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Left_Actual_Spindle_Speed), CSpl_485.It.GetFrpm(EWay.L).ToString());

                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Left_Current_Spindle_Load), CData.Spls[0].dLoad.ToString());
                

                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Right_Actual_Table_Speed), CMot.It.Get_Vel((int)EAx.RightGrindZone_Y).ToString("0.0"));
                // 2023.03.15 Max
                //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Right_Actual_Spindle_Speed), CSpl.It.Get_Frpm(EWay.R).ToString());
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Right_Actual_Spindle_Speed), CSpl_485.It.GetFrpm(EWay.R).ToString());

                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Right_Current_Spindle_Load), CData.Spls[1].dLoad.ToString());
            }
        }
        #endregion Timer

        #region ALID
        public void Add_ALID()
        {
            string sP = string.Format("{0}\\ALARM.txt", fileinfo.Directory.FullName);

            Log_wt("Add_ALID()");
            cboALID.Items.Clear();

            using (FileStream fs = new FileStream(sP, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.Peek() > -1)
                    {
                        string input = sr.ReadLine();
                        char[] Separators = new char[] { '\t' };
                        string[] SplitNum = input.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        mgem.AddALID(int.Parse(SplitNum[0]), SplitNum[2], SplitNum[1]);    // ALID, ptr, strALCD
                        cboALID.Items.Add(int.Parse(SplitNum[0]));
                    }

                    if (cboALID.Items.Count > -1)
                    { cboALID.SelectedIndex = 0; }

                    sr.Close();
                }
                fs.Close();
            }
        }
        #endregion ALID

        #region CEID
        public void Add_CEID()
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { Add_StdA_CEID(); }
            else { Add_StdB_CEID(); }
        }
        public void Add_StdA_CEID()
        {
            Log_wt("Add_CEID()");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Control_State_Change),      string.Format("{0} {1}", JSCK.eCEID.Control_State_Change.ToString(),    Convert.ToInt32(JSCK.eCEID.Control_State_Change)),      "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Process_State_Chagne),      string.Format("{0} {1}", JSCK.eCEID.Process_State_Chagne.ToString(),    Convert.ToInt32(JSCK.eCEID.Process_State_Chagne)),      "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Remote_Paused)      ,       string.Format("{0} {1}", JSCK.eCEID.Remote_Paused.ToString(),           Convert.ToInt32(JSCK.eCEID.Remote_Paused)),             "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Remote_Resume)      ,       string.Format("{0} {1}", JSCK.eCEID.Remote_Resume.ToString(),           Convert.ToInt32(JSCK.eCEID.Remote_Resume)),             "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Machine_Start)      ,       string.Format("{0} {1}", JSCK.eCEID.Machine_Start.ToString(),           Convert.ToInt32(JSCK.eCEID.Machine_Start)),             "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Machine_ReStart)    ,       string.Format("{0} {1}", JSCK.eCEID.Machine_ReStart.ToString(),         Convert.ToInt32(JSCK.eCEID.Machine_ReStart)),           ""); // 2021-06-23, jhLee, for SPIL

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Recipe_Created)     ,       string.Format("{0} {1}", JSCK.eCEID.Recipe_Created.ToString(),          Convert.ToInt32(JSCK.eCEID.Recipe_Created)),            "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Recipe_Loaded)      ,       string.Format("{0} {1}", JSCK.eCEID.Recipe_Loaded.ToString(),           Convert.ToInt32(JSCK.eCEID.Recipe_Loaded)),             "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Recipe_Parameter_Changed),  string.Format("{0} {1}", JSCK.eCEID.Recipe_Parameter_Changed.ToString(),Convert.ToInt32(JSCK.eCEID.Recipe_Parameter_Changed)),  "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Recipe_Deleted)     ,       string.Format("{0} {1}", JSCK.eCEID.Recipe_Deleted.ToString(),          Convert.ToInt32(JSCK.eCEID.Recipe_Deleted)),            "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.LOT_Verify_Request) ,       string.Format("{0} {1}", JSCK.eCEID.LOT_Verify_Request.ToString(),      Convert.ToInt32(JSCK.eCEID.LOT_Verify_Request)),        "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.LOT_Verified)       ,       string.Format("{0} {1}", JSCK.eCEID.LOT_Verified.ToString(),            Convert.ToInt32(JSCK.eCEID.LOT_Verified)),              "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.LOT_Started)        ,       string.Format("{0} {1}", JSCK.eCEID.LOT_Started.ToString(),             Convert.ToInt32(JSCK.eCEID.LOT_Started)),               "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.LOT_Ended)          ,       string.Format("{0} {1}", JSCK.eCEID.LOT_Ended.ToString(),               Convert.ToInt32(JSCK.eCEID.LOT_Ended)),                 "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.LOT_Changed)        ,       string.Format("{0} {1}", JSCK.eCEID.LOT_Changed.ToString(),             Convert.ToInt32(JSCK.eCEID.LOT_Changed)),               "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request),    string.Format("{0} {1}", JSCK.eCEID.Carrier_Verify_Request.ToString(),  Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request)),    "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verified)   ,       string.Format("{0} {1}", JSCK.eCEID.Carrier_Verified.ToString(),        Convert.ToInt32(JSCK.eCEID.Carrier_Verified)),          "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Started)    ,       string.Format("{0} {1}", JSCK.eCEID.Carrier_Started.ToString(),         Convert.ToInt32(JSCK.eCEID.Carrier_Started)),           "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Ended)      ,       string.Format("{0} {1}", JSCK.eCEID.Carrier_Ended.ToString(),           Convert.ToInt32(JSCK.eCEID.Carrier_Ended)),             "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Unloaded)   ,       string.Format("{0} {1}", JSCK.eCEID.Carrier_Unloaded.ToString(),        Convert.ToInt32(JSCK.eCEID.Carrier_Unloaded)),          "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Summary_Uploaded),  string.Format("{0} {1}", JSCK.eCEID.Carrier_Summary_Uploaded.ToString(),Convert.ToInt32(JSCK.eCEID.Carrier_Summary_Uploaded)),  "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Carrier_Grind_Finished),    string.Format("{0} {1}", JSCK.eCEID.Carrier_Grind_Finished.ToString(),  Convert.ToInt32(JSCK.eCEID.Carrier_Grind_Finished)),    "");            

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verify_Request),   string.Format("{0} {1}", JSCK.eCEID.Magazine_Verify_Request.ToString(), Convert.ToInt32(JSCK.eCEID.Magazine_Verify_Request)),   "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verified)      ,   string.Format("{0} {1}", JSCK.eCEID.Magazine_Verified.ToString(),       Convert.ToInt32(JSCK.eCEID.Magazine_Verified)),         "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Magazine_Started)       ,   string.Format("{0} {1}", JSCK.eCEID.Magazine_Started.ToString(),        Convert.ToInt32(JSCK.eCEID.Magazine_Started)),          "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Magazine_Ended)         ,   string.Format("{0} {1}", JSCK.eCEID.Magazine_Ended.ToString(),          Convert.ToInt32(JSCK.eCEID.Magazine_Ended)),            "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Material_Verify_Request),   string.Format("{0} {1}", JSCK.eCEID.Material_Verify_Request.ToString(), Convert.ToInt32(JSCK.eCEID.Material_Verify_Request)),   "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Material_Verified)      ,   string.Format("{0} {1}", JSCK.eCEID.Material_Verified.ToString(),       Convert.ToInt32(JSCK.eCEID.Material_Verified)),         "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Material_Changed)       ,   string.Format("{0} {1}", JSCK.eCEID.Material_Changed.ToString(),        Convert.ToInt32(JSCK.eCEID.Material_Changed)),          "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Tool_Verify_Request)    ,   string.Format("{0} {1}", JSCK.eCEID.Tool_Verify_Request.ToString(),     Convert.ToInt32(JSCK.eCEID.Tool_Verify_Request)),       "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Tool_Verified)          ,   string.Format("{0} {1}", JSCK.eCEID.Tool_Verified.ToString(),           Convert.ToInt32(JSCK.eCEID.Tool_Verified)),             "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eCEID.Tool_Changed)           ,   string.Format("{0} {1}", JSCK.eCEID.Tool_Changed.ToString(),            Convert.ToInt32(JSCK.eCEID.Tool_Changed)),              "");
        }
        
        public void Add_StdB_CEID()
        {
            Log_wt("Add_StandB_CEID()");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Control_State_Change), string.Format("{0} {1}", JSCK.eBCEID.Control_State_Change.ToString(), Convert.ToInt32(JSCK.eBCEID.Control_State_Change)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Equipment_State_Change), string.Format("{0} {1}", JSCK.eBCEID.Equipment_State_Change.ToString(), Convert.ToInt32(JSCK.eBCEID.Equipment_State_Change)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Communication_State_Change), string.Format("{0} {1}", JSCK.eBCEID.Communication_State_Change.ToString(), Convert.ToInt32(JSCK.eBCEID.Communication_State_Change)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Start_Allowed_Reqeust), string.Format("{0} {1}", JSCK.eBCEID.Start_Allowed_Reqeust.ToString(), Convert.ToInt32(JSCK.eBCEID.Start_Allowed_Reqeust)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Machine_Start), string.Format("{0} {1}", JSCK.eBCEID.Machine_Start.ToString(), Convert.ToInt32(JSCK.eBCEID.Machine_Start)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.LOT_Started), string.Format("{0} {1}", JSCK.eBCEID.LOT_Started.ToString(), Convert.ToInt32(JSCK.eBCEID.LOT_Started)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Lot_Ended), string.Format("{0} {1}", JSCK.eBCEID.Lot_Ended.ToString(), Convert.ToInt32(JSCK.eBCEID.Lot_Ended)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Verify_Request), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Verify_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Verify_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Verified), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Verified.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Verified)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Loaded), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Loaded.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Loaded)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Started), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Started.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Started)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Summary_Uploaded), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Summary_Uploaded.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Summary_Uploaded)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Grind_Finished), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Grind_Finished.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Grind_Finished)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Ended), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Ended.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Ended)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Unloaded), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Unloaded.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Unloaded)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Scrap), string.Format("{0} {1}", JSCK.eBCEID.Carrier_Scrap.ToString(), Convert.ToInt32(JSCK.eBCEID.Carrier_Scrap)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Verify_Request), string.Format("{0} {1}", JSCK.eBCEID.Magazine_Verify_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Magazine_Verify_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Verified), string.Format("{0} {1}", JSCK.eBCEID.Magazine_Verified.ToString(), Convert.ToInt32(JSCK.eBCEID.Magazine_Verified)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Started), string.Format("{0} {1}", JSCK.eBCEID.Magazine_Started.ToString(), Convert.ToInt32(JSCK.eBCEID.Magazine_Started)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Ended), string.Format("{0} {1}", JSCK.eBCEID.Magazine_Ended.ToString(), Convert.ToInt32(JSCK.eBCEID.Magazine_Ended)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Wheel_Verify_Request), string.Format("{0} {1}", JSCK.eBCEID.Wheel_Verify_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Wheel_Verify_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Wheel_Verified), string.Format("{0} {1}", JSCK.eBCEID.Wheel_Verified.ToString(), Convert.ToInt32(JSCK.eBCEID.Wheel_Verified)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Wheel_Changed), string.Format("{0} {1}", JSCK.eBCEID.Wheel_Changed.ToString(), Convert.ToInt32(JSCK.eBCEID.Wheel_Changed)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Dresser_Verify_Request), string.Format("{0} {1}", JSCK.eBCEID.Dresser_Verify_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Dresser_Verify_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Dresser_Verified), string.Format("{0} {1}", JSCK.eBCEID.Dresser_Verified.ToString(), Convert.ToInt32(JSCK.eBCEID.Dresser_Verified)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Dresser_Changed), string.Format("{0} {1}", JSCK.eBCEID.Dresser_Changed.ToString(), Convert.ToInt32(JSCK.eBCEID.Dresser_Changed)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Loader_Normal_MGZ_Load_Request), string.Format("{0} {1}", JSCK.eBCEID.Loader_Normal_MGZ_Load_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Loader_Normal_MGZ_Load_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Loader_Normal_MGZ_Load_Complete), string.Format("{0} {1}", JSCK.eBCEID.Loader_Normal_MGZ_Load_Complete.ToString(), Convert.ToInt32(JSCK.eBCEID.Loader_Normal_MGZ_Load_Complete)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Loader_Empty_MGZ_Place), string.Format("{0} {1}", JSCK.eBCEID.Loader_Empty_MGZ_Place.ToString(), Convert.ToInt32(JSCK.eBCEID.Loader_Empty_MGZ_Place)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Loader_Empty_MGZ_Unload_Request), string.Format("{0} {1}", JSCK.eBCEID.Loader_Empty_MGZ_Unload_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Loader_Empty_MGZ_Unload_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Loader_Empty_MGZ_Unload_Complete), string.Format("{0} {1}", JSCK.eBCEID.Loader_Empty_MGZ_Unload_Complete.ToString(), Convert.ToInt32(JSCK.eBCEID.Loader_Empty_MGZ_Unload_Complete)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Unloader_Empty_MGZ_Load_Request), string.Format("{0} {1}", JSCK.eBCEID.Unloader_Empty_MGZ_Load_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Unloader_Empty_MGZ_Load_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Unloader_Empty_MGZ_Load_Complete), string.Format("{0} {1}", JSCK.eBCEID.Unloader_Empty_MGZ_Load_Complete.ToString(), Convert.ToInt32(JSCK.eBCEID.Unloader_Empty_MGZ_Load_Complete)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Unloader_Normal_MGZ_Place), string.Format("{0} {1}", JSCK.eBCEID.Unloader_Normal_MGZ_Place.ToString(), Convert.ToInt32(JSCK.eBCEID.Unloader_Normal_MGZ_Place)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Unloader_Normal_MGZ_Unload_Request), string.Format("{0} {1}", JSCK.eBCEID.Unloader_Normal_MGZ_Unload_Request.ToString(), Convert.ToInt32(JSCK.eBCEID.Unloader_Normal_MGZ_Unload_Request)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Unloader_Normal_MGZ_Unload_Complete), string.Format("{0} {1}", JSCK.eBCEID.Unloader_Normal_MGZ_Unload_Complete.ToString(), Convert.ToInt32(JSCK.eBCEID.Unloader_Normal_MGZ_Unload_Complete)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Operator_LogIn), string.Format("{0} {1}", JSCK.eBCEID.Operator_LogIn.ToString(), Convert.ToInt32(JSCK.eBCEID.Operator_LogIn)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Operator_LogOut), string.Format("{0} {1}", JSCK.eBCEID.Operator_LogOut.ToString(), Convert.ToInt32(JSCK.eBCEID.Operator_LogOut)), "");


            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Equipment_Constants_Change), string.Format("{0} {1}", JSCK.eBCEID.Equipment_Constants_Change.ToString(), Convert.ToInt32(JSCK.eBCEID.Equipment_Constants_Change)), "");

            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Loaded), string.Format("{0} {1}", JSCK.eBCEID.Recipe_Loaded.ToString(), Convert.ToInt32(JSCK.eBCEID.Recipe_Loaded)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Created), string.Format("{0} {1}", JSCK.eBCEID.Recipe_Created.ToString(), Convert.ToInt32(JSCK.eBCEID.Recipe_Created)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Deleted), string.Format("{0} {1}", JSCK.eBCEID.Recipe_Deleted.ToString(), Convert.ToInt32(JSCK.eBCEID.Recipe_Deleted)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Parameters_modified), string.Format("{0} {1}", JSCK.eBCEID.Recipe_Parameters_modified.ToString(), Convert.ToInt32(JSCK.eBCEID.Recipe_Parameters_modified)), "");
            mgem.AddCEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Selected_Failed), string.Format("{0} {1}", JSCK.eBCEID.Recipe_Selected_Failed.ToString(), Convert.ToInt32(JSCK.eBCEID.Recipe_Selected_Failed)), "");

        }
        #endregion CEID

        #region ECID
        public void Add_ECID()
        {
            Log_wt("Add_ECID()");
            mgem.AddECID(Convert.ToInt32(JSCK.eECID.T3_Timeout), JSCK.eECID.T3_Timeout.ToString(), GEM.sSECOND, GEM.sU1);
            mgem.SetECRange(Convert.ToInt32(JSCK.eECID.T3_Timeout), GEM.s1, GEM.s255);
            mgem.SetECValue(Convert.ToInt32(JSCK.eECID.T3_Timeout), ECV.m_nT3.ToString());

            mgem.AddECID(Convert.ToInt32(JSCK.eECID.T5_Timeout), JSCK.eECID.T5_Timeout.ToString(), GEM.sSECOND, GEM.sU1);
            mgem.SetECRange(Convert.ToInt32(JSCK.eECID.T5_Timeout), GEM.s1, GEM.s255);
            mgem.SetECValue(Convert.ToInt32(JSCK.eECID.T5_Timeout), ECV.m_nT5.ToString());

            mgem.AddECID(Convert.ToInt32(JSCK.eECID.T6_Timeout), JSCK.eECID.T6_Timeout.ToString(), GEM.sSECOND, GEM.sU1);
            mgem.SetECRange(Convert.ToInt32(JSCK.eECID.T6_Timeout), GEM.s1, GEM.s255);
            mgem.SetECValue(Convert.ToInt32(JSCK.eECID.T6_Timeout), ECV.m_nT6.ToString());

            mgem.AddECID(Convert.ToInt32(JSCK.eECID.T7_Timeout), JSCK.eECID.T7_Timeout.ToString(), GEM.sSECOND, GEM.sU1);
            mgem.SetECRange(Convert.ToInt32(JSCK.eECID.T7_Timeout), GEM.s1, GEM.s255);
            mgem.SetECValue(Convert.ToInt32(JSCK.eECID.T7_Timeout), ECV.m_nT7.ToString());

            mgem.AddECID(Convert.ToInt32(JSCK.eECID.T8_Timeout), JSCK.eECID.T8_Timeout.ToString(), GEM.sSECOND, GEM.sU1);
            mgem.SetECRange(Convert.ToInt32(JSCK.eECID.T8_Timeout), GEM.s1, GEM.s255);
            mgem.SetECValue(Convert.ToInt32(JSCK.eECID.T8_Timeout), ECV.m_nT8.ToString());

            mgem.AddECID(Convert.ToInt32(JSCK.eECID.Communication_Establish_Timeout), JSCK.eECID.Communication_Establish_Timeout.ToString(), GEM.sSECOND, GEM.sU2);
            mgem.SetECRange(Convert.ToInt32(JSCK.eECID.Communication_Establish_Timeout), GEM.s0, GEM.s9999);
            mgem.SetECValue(Convert.ToInt32(JSCK.eECID.Communication_Establish_Timeout), ECV.m_nCommReqeustTimeout.ToString());

        }
        #endregion ECID

        #region SVID
        public void Add_SVID()
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { Add_StdA_SVID(); }
            else { Add_StdB_SVID(); }
        }
        public void Add_StdA_SVID()
        {
            // SVID를 gem dll에 등록한다.
            // ID , ID's Name , ID's Format( A , U1 , U2 , U4 , I1 , I2 , I4 , F4 , B , BOOL ..) , UNIT

            Log_wt("Add_SVID()");
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Clock),                     JSCK.eSVID.Clock.ToString(),                    GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.EQ_ID),                     JSCK.eSVID.EQ_ID.ToString(),                    GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Previous_Control_State),    JSCK.eSVID.Previous_Control_State.ToString(),   GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_Control_State),     JSCK.eSVID.Current_Control_State.ToString(),    GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Previous_Process_Status),   JSCK.eSVID.Previous_Process_Status.ToString(),  GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_Process_Status),    JSCK.eSVID.Current_Process_Status.ToString(),   GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_PPID),              JSCK.eSVID.Current_PPID.ToString(),             GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.PPID),                      JSCK.eSVID.PPID.ToString(),                     GEM.sA.ToString(),      string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID),            JSCK.eSVID.Current_LOT_ID.ToString(),           GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.LOT_ID),                    JSCK.eSVID.LOT_ID.ToString(),                   GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.LOT_Started_Time),          JSCK.eSVID.LOT_Started_Time.ToString(),         GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.LOT_Ended_Time),            JSCK.eSVID.LOT_Ended_Time.ToString(),           GEM.sA.ToString(),      string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Total_Carrier_List),        JSCK.eSVID.Total_Carrier_List.ToString(),       GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Proceed_Carrier_List),      JSCK.eSVID.Proceed_Carrier_List.ToString(),     GEM.sObject.ToString(), string.Empty);

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Total_Carrier_List));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Total_Carrier_List));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Total_Carrier_List));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Proceed_Carrier_List));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Proceed_Carrier_List));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Proceed_Carrier_List));

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Total_Unit_Count),          JSCK.eSVID.Total_Unit_Count.ToString(),         GEM.sU4.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Proceed_Unit_Count),        JSCK.eSVID.Proceed_Unit_Count.ToString(),       GEM.sU4.ToString(),     string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read),         JSCK.eSVID.Carrier_List_Read.ToString(),        GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate),     JSCK.eSVID.Carrier_List_Validate.ToString(),    GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Started),      JSCK.eSVID.Carrier_List_Started.ToString(),     GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete),     JSCK.eSVID.Carrier_List_Complete.ToString(),    GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject),       JSCK.eSVID.Carrier_List_Reject.ToString(),      GEM.sObject.ToString(), string.Empty);

            mgem.CloseMsg(      Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
            mgem.OpenListItem(  Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
            mgem.CloseListItem( Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

            mgem.CloseMsg(      Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));
            mgem.OpenListItem(  Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));
            mgem.CloseListItem( Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));

            mgem.CloseMsg(      Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));
            mgem.OpenListItem(  Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));
            mgem.CloseListItem( Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));

            mgem.CloseMsg(      Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));
            mgem.OpenListItem(  Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));
            mgem.CloseListItem( Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));

            mgem.CloseMsg(      Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));
            mgem.OpenListItem(  Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));
            mgem.CloseListItem( Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));
                        
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Grind__Mode),               JSCK.eSVID.Grind__Mode.ToString(),              GEM.sU4.ToString(),     string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID),                JSCK.eSVID.Carrier_ID.ToString(),               GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Started_Time),      JSCK.eSVID.Carrier_Started_Time.ToString(),     GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Ended_Time),        JSCK.eSVID.Carrier_Ended_Time.ToString(),       GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Map),               JSCK.eSVID.Carrier_Map.ToString(),              GEM.sA.ToString(),      string.Empty);

            #region Measure Point
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point),   JSCK.eSVID.Carrier_Left_Work_Point.ToString(),  GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point),    JSCK.eSVID.Carrier_Left_Raw_Point.ToString(),   GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point),  JSCK.eSVID.Carrier_Right_Work_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point),   JSCK.eSVID.Carrier_Right_Raw_Point.ToString(),  GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Carrier_Current_Read_ID),   JSCK.eSVID.Carrier_Current_Read_ID.ToString(),  GEM.sA.ToString(),      string.Empty);
            #endregion

            #region Spindle & Dress Count Data
            //20210106 yyy SECSGEM message type change from "Object" to Ascii
            //mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Max_Spindle_Load)       , JSCK.eSVID.Left_Max_Spindle_Load.ToString()        , GEM.sObject.ToString(), string.Empty);
            //mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Max_Spindle_Load)      , JSCK.eSVID.Right_Max_Spindle_Load.ToString()       , GEM.sObject.ToString(), string.Empty);
            //mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Dress_After_Strip_count), JSCK.eSVID.Left_Dress_After_Strip_count.ToString() , GEM.sObject.ToString(), string.Empty);
            //mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Dress_After_Strip_count), JSCK.eSVID.Right_Dress_After_Strip_count.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Max_Spindle_Load),         JSCK.eSVID.Left_Max_Spindle_Load.ToString(),            GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Max_Spindle_Load)      ,  JSCK.eSVID.Right_Max_Spindle_Load.ToString()       ,    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Dress_After_Strip_count),  JSCK.eSVID.Left_Dress_After_Strip_count.ToString() ,    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Dress_After_Strip_count), JSCK.eSVID.Right_Dress_After_Strip_count.ToString(),    GEM.sA.ToString(), string.Empty);

            #endregion


            #region Wheel & Dress 두께 20200731
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS), JSCK.eSVID.WHEEL_L_THICKNESS.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS), JSCK.eSVID.WHEEL_R_THICKNESS.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS), JSCK.eSVID.DRESS_L_THICKNESS.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS), JSCK.eSVID.DRESS_R_THICKNESS.ToString(), GEM.sObject.ToString(), string.Empty);
            #endregion

            #region Measure Point
            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));

            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));

            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));

            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            #endregion

            #region Wheel & Dress 두께 20200731
            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));

            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));

            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));

            mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            mgem.CloseListItem  (Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            #endregion

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID),               JSCK.eSVID.Magazine_ID.ToString(),              GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.MGZ_Slot_Number),           JSCK.eSVID.MGZ_Slot_Number.ToString(),          GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List),          JSCK.eSVID.MGZ_Carrier_List.ToString(),         GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number),           JSCK.eSVID.MGZ_Port_Number.ToString(),          GEM.sU1.ToString(),     string.Empty);
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List));

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL1_ID),   JSCK.eSVID.Current_Loaded_MATL1_ID.ToString() , GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL2_ID),   JSCK.eSVID.Current_Loaded_MATL2_ID.ToString() , GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID)               ,   JSCK.eSVID.MATL1_ID.ToString()                , GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID)               ,   JSCK.eSVID.MATL2_ID.ToString()                , GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.MATL_Loc)               ,   JSCK.eSVID.MATL_Loc.ToString()                , GEM.sU1.ToString(),     string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_1_ID),  JSCK.eSVID.Current_Loaded_Tool_1_ID.ToString(), GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_2_ID),  JSCK.eSVID.Current_Loaded_Tool_2_ID.ToString(), GEM.sA.ToString(),      string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID)         ,       JSCK.eSVID.Wheel_L_ID.ToString()          ,     GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID)         ,       JSCK.eSVID.Wheel_R_ID.ToString()          ,     GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID)          ,       JSCK.eSVID.Tool_1_ID.ToString()           ,     GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID)          ,       JSCK.eSVID.Tool_2_ID.ToString()           ,     GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Tool_L_Serial_Num)  ,       JSCK.eSVID.Tool_L_Serial_Num.ToString()   ,     GEM.sA.ToString(),      string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Tool_R_Serial_Num)  ,       JSCK.eSVID.Tool_R_Serial_Num.ToString()   ,     GEM.sA.ToString(),      string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Tool_Loc),                  JSCK.eSVID.Tool_Loc.ToString(),                 GEM.sU1.ToString(),     string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Table_Loc),                 JSCK.eSVID.Table_Loc.ToString(),                GEM.sU1.ToString(),     string.Empty);

            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Process_Status), JSCK.eProcStatus.Idle.ToString());

            //2019 10 24 skpark
            // 프로그램 시작시 화면의 정의된 Tool Id , Material Id , PPID(deviceId) 를 svid에 세팅해야 함.
            // 아래 내용을 저장해야 함. string.Empty 대신에 값을 넣어 줘야 함.
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL1_ID)   , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0]);            
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL2_ID)   , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]);                          
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_1_ID)  , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_2_ID)  , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID)                , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID)                , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Tool_L_Serial_Num)         , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Tool_R_Serial_Num)         , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.PPID)                      , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_PPID)              , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
            // 2020 02 05 LCY
            
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR),           JSCK.eSVID.DF_DATA_PCB_AVR.ToString(),              GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX),           JSCK.eSVID.DF_DATA_PCB_MAX.ToString(),              GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE),       JSCK.eSVID.PCB_TOPBTM_USE_MODE.ToString(),          GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE),          JSCK.eSVID.PCB_ORIENT_VALUE.ToString(),             GEM.sU1.ToString(), string.Empty);

            // 2021.07.23 lhs Start : SCK+용 SVID 추가
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_AVG),               JSCK.eSVID.TOP_THK_AVG.ToString(),                  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_MAX),               JSCK.eSVID.TOP_THK_MAX.ToString(),                  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_AVG),               JSCK.eSVID.BTM_THK_AVG.ToString(),                  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_MAX),               JSCK.eSVID.BTM_THK_MAX.ToString(),                  GEM.sA.ToString(), string.Empty);
            // 2021.07.23 lhs End : SCK+용 SVID 추가

            // 2021.05.21 SungTae Start : [추가] SPIL용 SVID List Up
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Strip_Short_Side),          JSCK.eSVID.Strip_Short_Side.ToString(),             GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Strip_Long_Side),           JSCK.eSVID.Strip_Long_Side.ToString(),              GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Dual_Mode),                 JSCK.eSVID.Dual_Mode.ToString(),                    GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Total_Thickness),      JSCK.eSVID.Left_Total_Thickness.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_PCB_Thickness),        JSCK.eSVID.Left_PCB_Thickness.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Mold_Thickness),       JSCK.eSVID.Left_Mold_Thickness.ToString(),          GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Total_Thickness),     JSCK.eSVID.Right_Total_Thickness.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_PCB_Thickness),       JSCK.eSVID.Right_PCB_Thickness.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Mold_Thickness),      JSCK.eSVID.Right_Mold_Thickness.ToString(),         GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Cycle_Depth),   JSCK.eSVID.Left_Actual_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Spindle_Speed), JSCK.eSVID.Left_Actual_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Table_Speed),   JSCK.eSVID.Left_Actual_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Cycle_Depth),  JSCK.eSVID.Right_Actual_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Spindle_Speed),JSCK.eSVID.Right_Actual_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Table_Speed),  JSCK.eSVID.Right_Actual_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Grinding_Mode),        JSCK.eSVID.Left_Grinding_Mode.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Air_Cut_Depth),        JSCK.eSVID.Left_Air_Cut_Depth.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Target),            JSCK.eSVID.Left_R1_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Cycle_Depth),       JSCK.eSVID.Left_R1_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Table_Speed),       JSCK.eSVID.Left_R1_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Spindle_Speed),     JSCK.eSVID.Left_R1_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Direction),         JSCK.eSVID.Left_R1_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Target),            JSCK.eSVID.Left_R2_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Cycle_Depth),       JSCK.eSVID.Left_R2_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Table_Speed),       JSCK.eSVID.Left_R2_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Spindle_Speed),     JSCK.eSVID.Left_R2_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Direction),         JSCK.eSVID.Left_R2_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Target),            JSCK.eSVID.Left_R3_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Cycle_Depth),       JSCK.eSVID.Left_R3_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Table_Speed),       JSCK.eSVID.Left_R3_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Spindle_Speed),     JSCK.eSVID.Left_R3_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Direction),         JSCK.eSVID.Left_R3_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Target),            JSCK.eSVID.Left_R4_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Cycle_Depth),       JSCK.eSVID.Left_R4_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Table_Speed),       JSCK.eSVID.Left_R4_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Spindle_Speed),     JSCK.eSVID.Left_R4_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Direction),         JSCK.eSVID.Left_R4_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Target),            JSCK.eSVID.Left_R5_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Cycle_Depth),       JSCK.eSVID.Left_R5_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Table_Speed),       JSCK.eSVID.Left_R5_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Spindle_Speed),     JSCK.eSVID.Left_R5_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Direction),         JSCK.eSVID.Left_R5_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Target),            JSCK.eSVID.Left_R6_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Cycle_Depth),       JSCK.eSVID.Left_R6_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Table_Speed),       JSCK.eSVID.Left_R6_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Spindle_Speed),     JSCK.eSVID.Left_R6_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Direction),         JSCK.eSVID.Left_R6_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Target),            JSCK.eSVID.Left_R7_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Cycle_Depth),       JSCK.eSVID.Left_R7_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Table_Speed),       JSCK.eSVID.Left_R7_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Spindle_Speed),     JSCK.eSVID.Left_R7_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Direction),         JSCK.eSVID.Left_R7_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Target),            JSCK.eSVID.Left_R8_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Cycle_Depth),       JSCK.eSVID.Left_R8_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Table_Speed),       JSCK.eSVID.Left_R8_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Spindle_Speed),     JSCK.eSVID.Left_R8_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Direction),         JSCK.eSVID.Left_R8_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Target),            JSCK.eSVID.Left_R9_Target.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Cycle_Depth),       JSCK.eSVID.Left_R9_Cycle_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Table_Speed),       JSCK.eSVID.Left_R9_Table_Speed.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Spindle_Speed),     JSCK.eSVID.Left_R9_Spindle_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Direction),         JSCK.eSVID.Left_R9_Direction.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Target),           JSCK.eSVID.Left_R10_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Cycle_Depth),      JSCK.eSVID.Left_R10_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Table_Speed),      JSCK.eSVID.Left_R10_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Spindle_Speed),    JSCK.eSVID.Left_R10_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Direction),        JSCK.eSVID.Left_R10_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Target),           JSCK.eSVID.Left_R12_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Cycle_Depth),      JSCK.eSVID.Left_R12_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Table_Speed),      JSCK.eSVID.Left_R12_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Spindle_Speed),    JSCK.eSVID.Left_R12_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Direction),        JSCK.eSVID.Left_R12_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
			mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Target),           JSCK.eSVID.Left_R12_Target.ToString(),          GEM.sA.ToString(), string.Empty);
			mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Cycle_Depth),      JSCK.eSVID.Left_R12_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Table_Speed),      JSCK.eSVID.Left_R12_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Spindle_Speed),    JSCK.eSVID.Left_R12_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Direction),        JSCK.eSVID.Left_R12_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Target),          JSCK.eSVID.Left_Fine_Target.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Cycle_Depth),     JSCK.eSVID.Left_Fine_Cycle_Depth.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Table_Speed),     JSCK.eSVID.Left_Fine_Table_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Spindle_Speed),   JSCK.eSVID.Left_Fine_Spindle_Speed.ToString(),  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Direction),       JSCK.eSVID.Left_Fine_Direction.ToString(),      GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Grinding_Mode),       JSCK.eSVID.Right_Grinding_Mode.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Cut_Depth),       JSCK.eSVID.Right_Air_Cut_Depth.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Target),           JSCK.eSVID.Right_R1_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Cycle_Depth),      JSCK.eSVID.Right_R1_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Table_Speed),      JSCK.eSVID.Right_R1_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Spindle_Speed),    JSCK.eSVID.Right_R1_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Direction),        JSCK.eSVID.Right_R1_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Target),           JSCK.eSVID.Right_R2_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Cycle_Depth),      JSCK.eSVID.Right_R2_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Table_Speed),      JSCK.eSVID.Right_R2_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Spindle_Speed),    JSCK.eSVID.Right_R2_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Direction),        JSCK.eSVID.Right_R2_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Target),           JSCK.eSVID.Right_R3_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Cycle_Depth),      JSCK.eSVID.Right_R3_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Table_Speed),      JSCK.eSVID.Right_R3_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Spindle_Speed),    JSCK.eSVID.Right_R3_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Direction),        JSCK.eSVID.Right_R3_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Target),           JSCK.eSVID.Right_R4_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Cycle_Depth),      JSCK.eSVID.Right_R4_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Table_Speed),      JSCK.eSVID.Right_R4_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Spindle_Speed),    JSCK.eSVID.Right_R4_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Direction),        JSCK.eSVID.Right_R4_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Target),           JSCK.eSVID.Right_R5_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Cycle_Depth),      JSCK.eSVID.Right_R5_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Table_Speed),      JSCK.eSVID.Right_R5_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Spindle_Speed),    JSCK.eSVID.Right_R5_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Direction),        JSCK.eSVID.Right_R5_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Target),           JSCK.eSVID.Right_R6_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Cycle_Depth),      JSCK.eSVID.Right_R6_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Table_Speed),      JSCK.eSVID.Right_R6_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Spindle_Speed),    JSCK.eSVID.Right_R6_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Direction),        JSCK.eSVID.Right_R6_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Target),           JSCK.eSVID.Right_R7_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Cycle_Depth),      JSCK.eSVID.Right_R7_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Table_Speed),      JSCK.eSVID.Right_R7_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Spindle_Speed),    JSCK.eSVID.Right_R7_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Direction),        JSCK.eSVID.Right_R7_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Target),           JSCK.eSVID.Right_R8_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Cycle_Depth),      JSCK.eSVID.Right_R8_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Table_Speed),      JSCK.eSVID.Right_R8_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Spindle_Speed),    JSCK.eSVID.Right_R8_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Direction),        JSCK.eSVID.Right_R8_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Target),           JSCK.eSVID.Right_R9_Target.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Cycle_Depth),      JSCK.eSVID.Right_R9_Cycle_Depth.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Table_Speed),      JSCK.eSVID.Right_R9_Table_Speed.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Spindle_Speed),    JSCK.eSVID.Right_R9_Spindle_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Direction),        JSCK.eSVID.Right_R9_Direction.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Target),          JSCK.eSVID.Right_R10_Target.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Cycle_Depth),     JSCK.eSVID.Right_R10_Cycle_Depth.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Table_Speed),     JSCK.eSVID.Right_R10_Table_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Spindle_Speed),   JSCK.eSVID.Right_R10_Spindle_Speed.ToString(),  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Direction),       JSCK.eSVID.Right_R10_Direction.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Target),          JSCK.eSVID.Right_R12_Target.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Cycle_Depth),     JSCK.eSVID.Right_R12_Cycle_Depth.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Table_Speed),     JSCK.eSVID.Right_R12_Table_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Spindle_Speed),   JSCK.eSVID.Right_R12_Spindle_Speed.ToString(),  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Direction),       JSCK.eSVID.Right_R12_Direction.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Target),          JSCK.eSVID.Right_R12_Target.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Cycle_Depth),     JSCK.eSVID.Right_R12_Cycle_Depth.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Table_Speed),     JSCK.eSVID.Right_R12_Table_Speed.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Spindle_Speed),   JSCK.eSVID.Right_R12_Spindle_Speed.ToString(),  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Direction),       JSCK.eSVID.Right_R12_Direction.ToString(),      GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Target),         JSCK.eSVID.Right_Fine_Target.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Cycle_Depth),    JSCK.eSVID.Right_Fine_Cycle_Depth.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Table_Speed),    JSCK.eSVID.Right_Fine_Table_Speed.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Spindle_Speed),  JSCK.eSVID.Right_Fine_Spindle_Speed.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Direction),      JSCK.eSVID.Right_Fine_Direction.ToString(),     GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Point_Column_Count),   JSCK.eSVID.Left_Contact_Point_Column_Count.ToString(),  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Point_Row_Count),      JSCK.eSVID.Left_Contact_Point_Row_Count.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_Width),           JSCK.eSVID.Left_Contact_Unit_Width.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_Height),          JSCK.eSVID.Left_Contact_Unit_Height.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_1_Center_Y),      JSCK.eSVID.Left_Contact_Unit_1_Center_Y.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_2_Center_Y),      JSCK.eSVID.Left_Contact_Unit_2_Center_Y.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_3_Center_Y),      JSCK.eSVID.Left_Contact_Unit_3_Center_Y.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_4_Center_Y),      JSCK.eSVID.Left_Contact_Unit_4_Center_Y.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Before_Limit),         JSCK.eSVID.Left_Contact_Before_Limit.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_After_Limit),          JSCK.eSVID.Left_Contact_After_Limit.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_TTV_Limit),            JSCK.eSVID.Left_Contact_TTV_Limit.ToString(),           GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_One_Point_Limit),      JSCK.eSVID.Left_Contact_One_Point_Limit.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_One_Point_Over_Check), JSCK.eSVID.Left_Contact_One_Point_Over_Check.ToString(),GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Point_Column_Count),  JSCK.eSVID.Right_Contact_Point_Column_Count.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Point_Row_Count),     JSCK.eSVID.Right_Contact_Point_Row_Count.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_Width),          JSCK.eSVID.Right_Contact_Unit_Width.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_Height),         JSCK.eSVID.Right_Contact_Unit_Height.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_1_Center_Y),     JSCK.eSVID.Right_Contact_Unit_1_Center_Y.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_2_Center_Y),     JSCK.eSVID.Right_Contact_Unit_2_Center_Y.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_3_Center_Y),     JSCK.eSVID.Right_Contact_Unit_3_Center_Y.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_4_Center_Y),     JSCK.eSVID.Right_Contact_Unit_4_Center_Y.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Before_Limit),        JSCK.eSVID.Right_Contact_Before_Limit.ToString(),       GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_After_Limit),         JSCK.eSVID.Right_Contact_After_Limit.ToString(),        GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_TTV_Limit),           JSCK.eSVID.Right_Contact_TTV_Limit.ToString(),          GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_One_Point_Limit),     JSCK.eSVID.Right_Contact_One_Point_Limit.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_One_Point_Over_Check),JSCK.eSVID.Right_Contact_One_Point_Over_Check.ToString(), GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.BCR_Skip),                          JSCK.eSVID.BCR_Skip.ToString(),                         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.BCR_Key_In_Skip),                   JSCK.eSVID.BCR_Key_In_Skip.ToString(),                  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Orientation_Skip),                  JSCK.eSVID.Orientation_Skip.ToString(),                 GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Dry_Knife_Count),                   JSCK.eSVID.Dry_Knife_Count.ToString(),                  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Dry_Pusher_Fast_Velocity),          JSCK.eSVID.Dry_Pusher_Fast_Velocity.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Dry_Pusher_Slow_Velocity),          JSCK.eSVID.Dry_Pusher_Slow_Velocity.ToString(),         GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Dry_Bottom_Cleaning_Picker_Count),  JSCK.eSVID.Dry_Bottom_Cleaning_Picker_Count.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Dry_Bottom_Cleaning_Strip_Count),   JSCK.eSVID.Dry_Bottom_Cleaning_Strip_Count.ToString(),  GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Water_Knife_Clean_Velocity),   JSCK.eSVID.Left_Water_Knife_Clean_Velocity.ToString(),  GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Water_Knife_Clean_Count),      JSCK.eSVID.Left_Water_Knife_Clean_Count.ToString(),     GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Air_Knife_Clean_Velocity),     JSCK.eSVID.Left_Air_Knife_Clean_Velocity.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Left_Air_Knife_Clean_Count),        JSCK.eSVID.Left_Air_Knife_Clean_Count.ToString(),       GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Water_Knife_Clean_Velocity),  JSCK.eSVID.Right_Water_Knife_Clean_Velocity.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Water_Knife_Clean_Count),     JSCK.eSVID.Right_Water_Knife_Clean_Count.ToString(),    GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Knife_Clean_Velocity),    JSCK.eSVID.Right_Air_Knife_Clean_Velocity.ToString(),   GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Knife_Clean_Count),       JSCK.eSVID.Right_Air_Knife_Clean_Count.ToString(),      GEM.sA.ToString(), string.Empty);
            // 2021.05.21 SungTae End
        }
        public void Add_StdB_SVID()
        {
            Log_wt("Add_StandB_SVID()");
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Equipment_State), JSCK.eBSVID.Equipment_State.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Previous_Equipment_State), JSCK.eBSVID.Previous_Equipment_State.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Control_State), JSCK.eBSVID.Control_State.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Clock), JSCK.eBSVID.Clock.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.OperatorId), JSCK.eBSVID.OperatorId.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Communication_State), JSCK.eBSVID.Communication_State.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Current_PPID), JSCK.eBSVID.Current_PPID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Changed_PPID), JSCK.eBSVID.Changed_PPID.ToString(), GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Started_Time), JSCK.eBSVID.Carrier_Started_Time.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Ended_Time), JSCK.eBSVID.Carrier_Ended_Time.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage1_CarrierID), JSCK.eBSVID.Stage1_CarrierID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage2_CarrierID), JSCK.eBSVID.Stage2_CarrierID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Reading_Carrier_ID), JSCK.eBSVID.Reading_Carrier_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Load_CarrierID_1), JSCK.eBSVID.Load_CarrierID_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Load_CarrierID_2), JSCK.eBSVID.Load_CarrierID_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Load_CarrierID_3), JSCK.eBSVID.Load_CarrierID_3.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Start_CarrierID), JSCK.eBSVID.Start_CarrierID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Start_CarrierID_1), JSCK.eBSVID.Start_CarrierID_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Start_CarrierID_2), JSCK.eBSVID.Start_CarrierID_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Start_CarrierID_3), JSCK.eBSVID.Start_CarrierID_3.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage1_CarrierID_1), JSCK.eBSVID.Stage1_CarrierID_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage1_CarrierID_2), JSCK.eBSVID.Stage1_CarrierID_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage1_CarrierID_3), JSCK.eBSVID.Stage1_CarrierID_3.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage2_CarrierID_1), JSCK.eBSVID.Stage2_CarrierID_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage2_CarrierID_2), JSCK.eBSVID.Stage2_CarrierID_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Stage2_CarrierID_3), JSCK.eBSVID.Stage2_CarrierID_3.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Complete_CarrierID), JSCK.eBSVID.Complete_CarrierID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Complete_CarrierID_1), JSCK.eBSVID.Complete_CarrierID_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Complete_CarrierID_2), JSCK.eBSVID.Complete_CarrierID_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Complete_CarrierID_3), JSCK.eBSVID.Complete_CarrierID_3.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Complete_Result), JSCK.eBSVID.Carrier_Complete_Result.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Complete_Failure_List), JSCK.eBSVID.Carrier_Complete_Failure_List.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Complete_Lot_Barcode), JSCK.eBSVID.Complete_Lot_Barcode.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Stored_Barcode), JSCK.eBSVID.Carrier_Stored_Barcode.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Stored_Tray_Barcode), JSCK.eBSVID.Carrier_Stored_Tray_Barcode.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Stored_Position), JSCK.eBSVID.Carrier_Stored_Position.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Remove_Barcode), JSCK.eBSVID.Carrier_Remove_Barcode.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Remove_Reason_Code_Type), JSCK.eBSVID.Carrier_Remove_Reason_Code_Type.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Remove_Reason_Code), JSCK.eBSVID.Carrier_Remove_Reason_Code.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Cancel_Tray_Barcode), JSCK.eBSVID.Cancel_Tray_Barcode.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Cancel_Carrier_Info_1), JSCK.eBSVID.Cancel_Carrier_Info_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Cancel_Carrier_Info_2), JSCK.eBSVID.Cancel_Carrier_Info_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Cancel_Carrier_Info_3), JSCK.eBSVID.Cancel_Carrier_Info_3.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Summary_Data), JSCK.eBSVID.Carrier_Summary_Data.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Total_Carrier_List), JSCK.eBSVID.Total_Carrier_List.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Proceed_Carrier_List), JSCK.eBSVID.Proceed_Carrier_List.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Unloaded), JSCK.eBSVID.Carrier_List_Unloaded.ToString(), GEM.sObject.ToString(), string.Empty);

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Summary_Data));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Summary_Data));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Summary_Data));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Total_Carrier_List));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Total_Carrier_List));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Total_Carrier_List));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Proceed_Carrier_List));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Proceed_Carrier_List));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Proceed_Carrier_List));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Unloaded));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Unloaded));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Unloaded));

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Total_Unit_Count), JSCK.eBSVID.Total_Unit_Count.ToString(), GEM.sU4.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Proceed_Unit_Count), JSCK.eBSVID.Proceed_Unit_Count.ToString(), GEM.sU4.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read), JSCK.eBSVID.Carrier_List_Read.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Validate), JSCK.eBSVID.Carrier_List_Validate.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Started), JSCK.eBSVID.Carrier_List_Started.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Complete), JSCK.eBSVID.Carrier_List_Complete.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Reject), JSCK.eBSVID.Carrier_List_Reject.ToString(), GEM.sObject.ToString(), string.Empty);

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Current_Lot_List));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Current_Lot_List));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Current_Lot_List));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Validate));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Validate));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Validate));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Started));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Started));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Started));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Complete));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Complete));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Complete));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Reject));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Reject));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Reject));

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Port_Number), JSCK.eBSVID.MGZ_Port_Number.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Reading_Magazine_ID), JSCK.eBSVID.Reading_Magazine_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Demand_Type), JSCK.eBSVID.Demand_Type.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Slot_Map), JSCK.eBSVID.Slot_Map.ToString(), GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.LD_Magazine_ID), JSCK.eBSVID.LD_Magazine_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.LD_Slot_ID), JSCK.eBSVID.LD_Slot_ID.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.GOOD_Magazine_ID), JSCK.eBSVID.GOOD_Magazine_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.GOOD_Slot_ID), JSCK.eBSVID.GOOD_Slot_ID.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.NG_Magazine_ID), JSCK.eBSVID.NG_Magazine_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.NG_Slot_ID), JSCK.eBSVID.NG_Slot_ID.ToString(), GEM.sU1.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.LOT_ID), JSCK.eBSVID.LOT_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Current_Lot_ID), JSCK.eBSVID.Current_Lot_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Current_Lot_List), JSCK.eBSVID.Current_Lot_List.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Lot_Started_Time), JSCK.eBSVID.Lot_Started_Time.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Lot_Ended_Time), JSCK.eBSVID.Lot_Ended_Time.ToString(), GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.PCB_TOPBTM_USE_MODE), JSCK.eBSVID.PCB_TOPBTM_USE_MODE.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Grind_Dual_Mode), JSCK.eBSVID.Grind_Dual_Mode.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Sg1_Grinding_Mode), JSCK.eBSVID.Sg1_Grinding_Mode.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Sg2_Grinding_Mode), JSCK.eBSVID.Sg2_Grinding_Mode.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Grinding_Reference), JSCK.eBSVID.Grinding_Reference.ToString(), GEM.sU1.ToString(), string.Empty);


            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_AVR), JSCK.eBSVID.DF_DATA_PCB_AVR.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_MAX), JSCK.eBSVID.DF_DATA_PCB_MAX.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_PCB_EMC_Work_Point), JSCK.eBSVID.Left_Carrier_PCB_EMC_Work_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_EMC_Raw_Point), JSCK.eBSVID.Left_Carrier_EMC_Raw_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_PCB_EMC_Work_Point), JSCK.eBSVID.Right_Carrier_PCB_EMC_Work_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_EMC_Raw_Point), JSCK.eBSVID.Right_Carrier_EMC_Raw_Point.ToString(), GEM.sObject.ToString(), string.Empty);

            #region Measure Point
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Table_Location), JSCK.eBSVID.Table_Location.ToString(), GEM.sU1.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), JSCK.eBSVID.Carrier_Sg1_Work_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), JSCK.eBSVID.Carrier_Sg1_Raw_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), JSCK.eBSVID.Carrier_Sg2_Work_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), JSCK.eBSVID.Carrier_Sg2_Raw_Point.ToString(), GEM.sObject.ToString(), string.Empty);
            #endregion

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.UnitPerHour), JSCK.eBSVID.UnitPerHour.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.MeanTimeBetweenAssistance), JSCK.eBSVID.MeanTimeBetweenAssistance.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.MeanTimeBetweenFailure), JSCK.eBSVID.MeanTimeBetweenFailure.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.MachineDowntime), JSCK.eBSVID.MachineDowntime.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.MachineUptime), JSCK.eBSVID.MachineUptime.ToString(), GEM.sA.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg1_WheelID), JSCK.eBSVID.Loaded_Sg1_WheelID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg2_WheelID), JSCK.eBSVID.Loaded_Sg2_WheelID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_ID), JSCK.eBSVID.Wheel_Sg1_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_ID), JSCK.eBSVID.Wheel_Sg2_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_1), JSCK.eBSVID.Wheel_FileName_1.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_2), JSCK.eBSVID.Wheel_FileName_2.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Loc), JSCK.eBSVID.Wheel_Loc.ToString(), GEM.sU1.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg1_DressID), JSCK.eBSVID.Loaded_Sg1_DressID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg2_DressID), JSCK.eBSVID.Loaded_Sg2_DressID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_ID), JSCK.eBSVID.Dress_Sg1_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_ID), JSCK.eBSVID.Dress_Sg2_ID.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Dress_Loc), JSCK.eBSVID.Dress_Loc.ToString(), GEM.sU1.ToString(), string.Empty);


            #region Wheel & Dress 두께 20200731
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness), JSCK.eBSVID.Wheel_Sg1_Thickness.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness), JSCK.eBSVID.Wheel_Sg2_Thickness.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness), JSCK.eBSVID.Dress_Sg1_Thickness.ToString(), GEM.sObject.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness), JSCK.eBSVID.Dress_Sg2_Thickness.ToString(), GEM.sObject.ToString(), string.Empty);

            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Left_Actual_Table_Speed), JSCK.eBSVID.Left_Actual_Table_Speed.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Left_Actual_Spindle_Speed), JSCK.eBSVID.Left_Actual_Spindle_Speed.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Left_Current_Spindle_Load), JSCK.eBSVID.Left_Current_Spindle_Load.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Left_Max_Spindle_Load), JSCK.eBSVID.Left_Max_Spindle_Load.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Right_Actual_Table_Speed), JSCK.eBSVID.Right_Actual_Table_Speed.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Right_Actual_Spindle_Speed), JSCK.eBSVID.Right_Actual_Spindle_Speed.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Right_Current_Spindle_Load), JSCK.eBSVID.Right_Current_Spindle_Load.ToString(), GEM.sA.ToString(), string.Empty);
            mgem.AddSVID(Convert.ToInt32(JSCK.eBSVID.Right_Max_Spindle_Load), JSCK.eBSVID.Right_Max_Spindle_Load.ToString(), GEM.sA.ToString(), string.Empty);
            #endregion


            #region Measure Point
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_PCB_EMC_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_PCB_EMC_Work_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_PCB_EMC_Work_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_EMC_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_EMC_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Left_Carrier_EMC_Raw_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_PCB_EMC_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_PCB_EMC_Work_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_PCB_EMC_Work_Point));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_EMC_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_EMC_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Right_Carrier_EMC_Raw_Point));

            #endregion

            #region Wheel & Dress 두께 20200731
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            #endregion

            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Process_Status), JSCK.eProcStatus.Idle.ToString());
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Current_Loaded_MATL1_ID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Dress_Name[0]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Current_Loaded_MATL2_ID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Dress_Name[1]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Current_Loaded_Tool_1_ID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Tool_Name[0]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Current_Loaded_Tool_2_ID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Tool_Name[1]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_ID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Tool_Name[0]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_ID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Tool_Name[1]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Tool_L_Serial_Num), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Tool_Serial_Num[0]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Tool_R_Serial_Num), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Tool_Serial_Num[1]);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.PPID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Recipe_Name);
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Current_PPID), CData.Gem_Strip_Data[(int)EDataShift.EQ_READY/*0*/, 0].sCurr_Recipe_Name);

        }
        #endregion SVID

        #region HSMS - function
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string sClassName, string sWindowName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath);

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);

        public String GetIniValue(String Section, String Key, String iniPath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);

            return temp.ToString();
        }

        public void SetIniValue(String Section, String Key, String Value, String iniPath)
        {
            WritePrivateProfileString(Section, Key, Value, iniPath);
        }

        public uint TransShort(string sVal)
        {
            int m = 0;
            uint nRet = 0;

            try
            { m = Int32.Parse(sVal); }
            catch
            { m = 0; }

            nRet = (uint)m;

            return nRet;
        }

        public void Get_Config()
        {
            string sVal = string.Empty;
            string sSec = string.Empty;
            string sKey = string.Empty;
            string sPath = string.Empty;

            Log_wt("Get_Config()");

            sSec = GEM.sHSMS_HSMS;
            sKey = GEM.sHSMS_Port;
            sPath = fileinfo.Directory.FullName + @"\GEM.INI";
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nPort = uint.Parse(sVal); }

            sKey = GEM.sHSMS_DeviceID;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nDeviceID = uint.Parse(sVal); }

            sKey = GEM.sHSMS_T3;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nT3 = uint.Parse(sVal); }

            sKey = GEM.sHSMS_T5;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nT5 = uint.Parse(sVal); }

            sKey = GEM.sHSMS_T6;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nT6 = uint.Parse(sVal); }

            sKey = GEM.sHSMS_T7;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nT7 = uint.Parse(sVal); }

            sKey = GEM.sHSMS_T8;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nT8 = uint.Parse(sVal); }

            sKey = GEM.sHSMS_Passive;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal == "1")
            { ECV.m_bPassive = true; }
            else
            { ECV.m_bPassive = false; }

            sSec = GEM.sHSMS_GEM;
            sKey = GEM.sHSMS_LinkTest;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nLinkTestInterval = uint.Parse(sVal); }

            sKey = GEM.sHSMS_RequestTimeout;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_nCommReqeustTimeout = uint.Parse(sVal); }

            sSec = GEM.sHSMS_GEM;
            sKey = GEM.sHSMS_MDLN;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_szModelName = sVal; }

            sKey = GEM.sHSMS_Softrev;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { ECV.m_szSoftRev = sVal; }


            sSec = GEM.sHSMS_CONFIGURATION;
            sKey = GEM.sHSMS_UseRecipeFolder;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { UseRecipeFolder = bool.Parse(sVal); }

            sKey = GEM.sHSMS_RMSPath;
            sVal = string.Empty;
            sVal = GetIniValue(sSec, sKey, sPath);
            if (sVal != string.Empty)
            { RMSPath = sVal; }
        }

        //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
        private void _check_RecipeName(ref string sRecipeName)
        {
            if (string.IsNullOrWhiteSpace(sRecipeName)) { return; }

            string [] sSeparators = new string[] { "\\" };
            string [] sSplitPath = sRecipeName.Split(sSeparators, StringSplitOptions.RemoveEmptyEntries);
            int iLen = sSplitPath.Length;
            if (iLen >= 2)
            {
                sRecipeName = sSplitPath[iLen-2] + "\\" + sSplitPath[iLen-1].Replace(".dev", "");
            }
            else
            {
                string sName = CData.DevCur.Replace(GV.PATH_DEVICE, "");
                sRecipeName = sName.Replace(".dev", "");
            }
        }

        //191118 ksg :
        public void LastLoad()
        {
            Log_wt("LastLoad()");

            int    j = 0;
            int    k = 0;
            string sSec = "";
            string sPath = GV.PATH_CONFIG + "LastSecGem.cfg";

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    LastLoad();

                });
            }
            else
            {
                if (!File.Exists(sPath))    { return; }

                //CIni mIni = new CIni(sPath);
                IniFile mIni = new IniFile();
                mIni.Load(sPath);
                sSec = "SecGem";
                int i = 0;
                for( i = 0; i < CData.JSCK_Max_Cnt; i++)
                {
                    //if(!((i == 0) || (i == 4) || (i == 7) || (i == 10) || (i == 11) || (i == 14) || (i == 20))) continue;
                    if ((i == 1)  || (i == 3)  || (i == 4)  || (i == 5)  || (i == 6) || (i == 7) || (i == 10) ||
                        (i == 11) || (i == 12) || (i == 13) || (i == 16) || (i == 17) || (i == 23)) continue;// 20200430 LCY : 사용하는 배열만 저장
                    CData.JSCK_Gem_Data[i].sSaBUN            = mIni[sSec]["SaBun_"            + i.ToString()].GetString();
                    CData.JSCK_Gem_Data[i].sCurr_Recipe_Name = mIni[sSec]["Curr_Recipe_Name_" + i.ToString()].GetString();
                    _check_RecipeName(ref CData.JSCK_Gem_Data[i].sCurr_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
                    CData.JSCK_Gem_Data[i].sNew_Recipe_Name  = mIni[sSec]["New_Recipe_Name_"  + i.ToString()].GetString();
                    _check_RecipeName(ref CData.JSCK_Gem_Data[i].sNew_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비

                    for(j = 0; j < 5; j++)
                    {
                        CData.JSCK_Gem_Data[i].sCurr_Wheel_Name[j]      = mIni[sSec]["Curr_Wheel_Name_"        + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sNew_Wheel_Name [j]      = mIni[sSec]["New_Wheel_Name_"         + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sCurr_Dress_Name[j]      = mIni[sSec]["sCurr_Dress_Name_"       + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sNEW_Dress_Name [j]      = mIni[sSec]["sNEW_Dress_Name_"        + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sCurr_Tool_Name [j]      = mIni[sSec]["sCurr_Tool_Name_"        + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sNEW_Tool_Name  [j]      = mIni[sSec]["sNEW_Tool_Name_"         + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sCurr_Tool_Serial_Num[j] = mIni[sSec]["sCurr_Tool_Serial_Num_"  + i.ToString() + "_" + j.ToString()].GetString();
                        CData.JSCK_Gem_Data[i].sNEW_Tool_Serial_Num[j]  = mIni[sSec]["sNEW_Tool_Serial_Num_"   + i.ToString() + "_" + j.ToString()].GetString();
                    }

                    CData.JSCK_Gem_Data[i].sLot_Name                  = mIni[sSec]["sLot_Name_"                 + i.ToString()].GetString();  
                    CData.JSCK_Gem_Data[i].sLD_MGZ_ID                 = mIni[sSec]["sLD_MGZ_ID_"                + i.ToString()].GetString();  
                    CData.JSCK_Gem_Data[i].sLD_MGZ_ID_New             = mIni[sSec]["sLD_MGZ_ID_New_"            + i.ToString()].GetString();
                    CData.JSCK_Gem_Data[i].sInRail_Strip_ID           = mIni[sSec]["sInRail_Strip_ID_"          + i.ToString()].GetString();  
                    CData.JSCK_Gem_Data[i].sInRail_Verified_Strip_ID  = mIni[sSec]["sInRail_Verified_Strip_ID_" + i.ToString()].GetString();  
                    CData.JSCK_Gem_Data[i].sULD_MGZ_ID                = mIni[sSec]["sULD_MGZ_ID_"               + i.ToString()].GetString();  
                    CData.JSCK_Gem_Data[i].nLD_MGZ_Cnt                = mIni[sSec]["nLD_MGZ_Cnt_"               + i.ToString()].ToInt();  
                    CData.JSCK_Gem_Data[i].nLD_MGZ_Strip_Cnt          = mIni[sSec]["nLD_MGZ_Strip_Cnt_"         + i.ToString()].ToInt();  
                    CData.JSCK_Gem_Data[i].nLot_Strip_Cnt             = mIni[sSec]["nLot_Strip_Cnt_"            + i.ToString()].ToInt();  
                    CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_Start_Cnt  = mIni[sSec]["nUnit_LEFT_Work_Start_Cnt_" + i.ToString()].ToInt();  
                    CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_End_Cnt    = mIni[sSec]["nUnit_LEFT_Work_End_Cnt_"   + i.ToString()].ToInt();  
                    CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_Start_Cnt = mIni[sSec]["nUnit_RIGHT_Work_Start_Cnt_"+ i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_End_Cnt   = mIni[sSec]["nUnit_RIGHT_Work_End_Cnt_"  + i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_Start_Cnt = mIni[sSec]["nUnit_TOTAL_Work_Start_Cnt_"+ i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_End_Cnt   = mIni[sSec]["nUnit_TOTAL_Work_End_Cnt_"  + i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nStrip_Insp_Result         = mIni[sSec]["nStrip_Insp_Result_"        + i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nULD_MGZ_Cnt               = mIni[sSec]["nULD_MGZ_Cnt_"              + i.ToString()].ToInt();
                    CData.JSCK_Gem_Data[i].nULD_MGZ_Port              = mIni[sSec]["nULD_MGZ_Port_"             + i.ToString()].ToInt();
                    CData.JSCK_Gem_Data[i].nULD_MGZ_Strip_Cnt         = mIni[sSec]["nULD_MGZ_Strip_Cnt_"        + i.ToString()].ToInt();
                    CData.JSCK_Gem_Data[i].nWork_Continue_Verified    = mIni[sSec]["nWork_Continue_Verified_"   + i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nDF_User_Mode              = mIni[sSec]["nDF_User_Mode_"             + i.ToString()].ToInt(); 
                    CData.JSCK_Gem_Data[i].nDF_User_New_Mode          = mIni[sSec]["nDF_User_New_Mode_"         + i.ToString()].ToInt(); 

                    for(j = 0; j < 7; j++)
                    {
                        for(k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        {
                            CData.JSCK_Gem_Data[i].fMeasure_Data[j,k] = mIni[sSec]["fMeasure_Data_"   + i.ToString() + "_" + j.ToString() + "_" + k.ToString()].ToDouble();                        
                        }
                        CData.JSCK_Gem_Data[i].fMeasure_Min_Data[j] = mIni[sSec]["fMeasure_Min_Data_" + i.ToString() + "_" + j.ToString()].ToDouble();
                        CData.JSCK_Gem_Data[i].fMeasure_Max_Data[j] = mIni[sSec]["fMeasure_Max_Data_" + i.ToString() + "_" + j.ToString()].ToDouble();
                        CData.JSCK_Gem_Data[i].fMeasure_Avr_Data[j] = mIni[sSec]["fMeasure_Avr_Data_" + i.ToString() + "_" + j.ToString()].ToDouble();
                    }
                    #region //200731 Wheel,Dress 두께 측정값 요청 (SPIL-CH VOC)
                    for (j = 0; j < 2; j++)
                    {
                        for (k = 0; k < 4; k++) 
                        {
                            CData.JSCK_Gem_Data[i].fMeasure_Wheel_Left_Thick[j, k]  = mIni[sSec]["fMeasure_Wheel_Left_Thick_"  + i.ToString() + "_" + j.ToString() + "_" + k.ToString()].ToDouble();
                            CData.JSCK_Gem_Data[i].fMeasure_Wheel_Right_Thick[j, k] = mIni[sSec]["fMeasure_Wheel_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString()].ToDouble();
                            CData.JSCK_Gem_Data[i].fMeasure_Dress_Left_Thick[j, k]  = mIni[sSec]["fMeasure_Dress_Left_Thick_"  + i.ToString() + "_" + j.ToString() + "_" + k.ToString()].ToDouble();
                            CData.JSCK_Gem_Data[i].fMeasure_Dress_Right_Thick[j, k] = mIni[sSec]["fMeasure_Dress_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString()].ToDouble();;
                        }
                    }
                    #endregion

                    for (j = 0; j < 7; j++)
                    {
                        for(k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        {
                            CData.JSCK_Gem_Data[i].fMeasure_New_Data[j,k] = mIni[sSec]["fMeasure_New_Data_"   + i.ToString() + "_" + j.ToString() + "_" + k.ToString()].ToDouble();                        
                        }
                        CData.JSCK_Gem_Data[i].fMeasure_New_Min_Data[j] = mIni[sSec]["fMeasure_New_Min_Data_" + i.ToString() + "_" + j.ToString()].ToDouble();
                        CData.JSCK_Gem_Data[i].fMeasure_New_Max_Data[j] = mIni[sSec]["fMeasure_New_Max_Data_" + i.ToString() + "_" + j.ToString()].ToDouble();
                        CData.JSCK_Gem_Data[i].fMeasure_New_Avr_Data[j] = mIni[sSec]["fMeasure_New_Avr_Data_" + i.ToString() + "_" + j.ToString()].ToDouble();
                    }
                }

                // 2020-11-09, jhLee, SPIL VOC 대응용, 현재 처리중인 LOT open 수량
                m_nLotOpenCount = mIni[sSec]["nLotOpenCount"].ToInt();
                if (m_nLotOpenCount < 0) m_nLotOpenCount = 0;

            }


        }

        //public void LastLoad_()
        //{
        //    Log_wt("LastLoad()");
        //    int    j = 0;
        //    int    k = 0;
        //    string sSec = "";
        //    string sVal = "";
        //    string sPath = GV.PATH_CONFIG + "LastSecGem.cfg";

        //    if (listLog.InvokeRequired)
        //    {
        //        Invoke((MethodInvoker)delegate ()
        //        {
        //            Console.WriteLine("---- a0");
        //            LastLoad();

        //        });

        //    }
        //    else
        //    {

        //        if (!File.Exists(sPath))
        //        { return; }

        //        CIni mIni = new CIni(sPath);
        //        sSec = "SecGem";
        //        int i = 0;
        //        for (i = 0; i < CData.JSCK_Max_Cnt; i++)
        //        {
        //            //if(!((i == 0) || (i == 4) || (i == 7) || (i == 10) || (i == 11) || (i == 14) || (i == 20))) continue;
        //            if ((i == 1) || (i == 3) || (i == 4) || (i == 5) || (i == 6) || (i == 7) || (i == 10) ||
        //                (i == 11) || (i == 12) || (i == 13) || (i == 16) || (i == 17) || (i == 23)) continue;// 20200430 LCY : 사용하는 배열만 저장
        //            CData.JSCK_Gem_Data[i].sSaBUN               = mIni.Read(sSec, "SaBun_" + i.ToString());
        //            CData.JSCK_Gem_Data[i].sCurr_Recipe_Name    = mIni.Read(sSec, "Curr_Recipe_Name_" + i.ToString());
        //            _check_RecipeName(ref CData.JSCK_Gem_Data[i].sCurr_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
        //            CData.JSCK_Gem_Data[i].sNew_Recipe_Name     = mIni.Read(sSec, "New_Recipe_Name_" + i.ToString());
        //            _check_RecipeName(ref CData.JSCK_Gem_Data[i].sNew_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비

        //            for (j = 0; j < 5; j++)
        //            {
        //                CData.JSCK_Gem_Data[i].sCurr_Wheel_Name[j]      = mIni.Read(sSec, "Curr_Wheel_Name_"        + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sNew_Wheel_Name[j]       = mIni.Read(sSec, "New_Wheel_Name_"         + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sCurr_Dress_Name[j]      = mIni.Read(sSec, "sCurr_Dress_Name_"       + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sNEW_Dress_Name[j]       = mIni.Read(sSec, "sNEW_Dress_Name_"        + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sCurr_Tool_Name[j]       = mIni.Read(sSec, "sCurr_Tool_Name_"        + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sNEW_Tool_Name[j]        = mIni.Read(sSec, "sNEW_Tool_Name_"         + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sCurr_Tool_Serial_Num[j] = mIni.Read(sSec, "sCurr_Tool_Serial_Num_"  + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].sNEW_Tool_Serial_Num[j]  = mIni.Read(sSec, "sNEW_Tool_Serial_Num_"   + i.ToString() + "_" + j.ToString());
        //            }

        //            CData.JSCK_Gem_Data[i].sLot_Name                    = mIni.Read(sSec, "sLot_Name_"                      + i.ToString());
        //            CData.JSCK_Gem_Data[i].sLD_MGZ_ID                   = mIni.Read(sSec, "sLD_MGZ_ID_" + i.ToString());
        //            CData.JSCK_Gem_Data[i].sLD_MGZ_ID_New               = mIni.Read(sSec, "sLD_MGZ_ID_New_"                 + i.ToString());
        //            CData.JSCK_Gem_Data[i].sInRail_Strip_ID             = mIni.Read(sSec, "sInRail_Strip_ID_"               + i.ToString());
        //            CData.JSCK_Gem_Data[i].sInRail_Verified_Strip_ID    = mIni.Read(sSec, "sInRail_Verified_Strip_ID_"      + i.ToString());
        //            CData.JSCK_Gem_Data[i].sULD_MGZ_ID                  = mIni.Read(sSec, "sULD_MGZ_ID_"                    + i.ToString());
        //            CData.JSCK_Gem_Data[i].nLD_MGZ_Cnt                  = mIni.ReadI(sSec, "nLD_MGZ_Cnt_"                   + i.ToString());
        //            CData.JSCK_Gem_Data[i].nLD_MGZ_Strip_Cnt            = mIni.ReadI(sSec, "nLD_MGZ_Strip_Cnt_"             + i.ToString());
        //            CData.JSCK_Gem_Data[i].nLot_Strip_Cnt               = mIni.ReadI(sSec, "nLot_Strip_Cnt_"                + i.ToString());
        //            CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_Start_Cnt    = mIni.ReadI(sSec, "nUnit_LEFT_Work_Start_Cnt_"     + i.ToString());
        //            CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_End_Cnt      = mIni.ReadI(sSec, "nUnit_LEFT_Work_End_Cnt_"       + i.ToString());
        //            CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_Start_Cnt   = mIni.ReadI(sSec, "nUnit_RIGHT_Work_Start_Cnt_"    + i.ToString());
        //            CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_End_Cnt     = mIni.ReadI(sSec, "nUnit_RIGHT_Work_End_Cnt_"      + i.ToString());
        //            CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_Start_Cnt   = mIni.ReadI(sSec, "nUnit_TOTAL_Work_Start_Cnt_"    + i.ToString());
        //            CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_End_Cnt     = mIni.ReadI(sSec, "nUnit_TOTAL_Work_End_Cnt_"      + i.ToString());
        //            CData.JSCK_Gem_Data[i].nStrip_Insp_Result           = mIni.ReadI(sSec, "nStrip_Insp_Result_"            + i.ToString());
        //            CData.JSCK_Gem_Data[i].nULD_MGZ_Cnt                 = mIni.ReadI(sSec, "nULD_MGZ_Cnt_"                  + i.ToString());
        //            CData.JSCK_Gem_Data[i].nULD_MGZ_Port                = mIni.ReadI(sSec, "nULD_MGZ_Port_"                 + i.ToString());
        //            CData.JSCK_Gem_Data[i].nULD_MGZ_Strip_Cnt           = mIni.ReadI(sSec, "nULD_MGZ_Strip_Cnt_"            + i.ToString());
        //            CData.JSCK_Gem_Data[i].nWork_Continue_Verified      = mIni.ReadI(sSec, "nWork_Continue_Verified_"       + i.ToString());
        //            CData.JSCK_Gem_Data[i].nDF_User_Mode                = mIni.ReadI(sSec, "nDF_User_Mode_"                 + i.ToString());
        //            CData.JSCK_Gem_Data[i].nDF_User_New_Mode            = mIni.ReadI(sSec, "nDF_User_New_Mode_"             + i.ToString());

        //            for (j = 0; j < 7; j++)
        //            {
        //                for (k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //                {
        //                    CData.JSCK_Gem_Data[i].fMeasure_Data[j, k] = mIni.ReadD(sSec, "fMeasure_Data_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString());
        //                }
        //                CData.JSCK_Gem_Data[i].fMeasure_Min_Data[j] = mIni.ReadD(sSec, "fMeasure_Min_Data_" + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].fMeasure_Max_Data[j] = mIni.ReadD(sSec, "fMeasure_Max_Data_" + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].fMeasure_Avr_Data[j] = mIni.ReadD(sSec, "fMeasure_Avr_Data_" + i.ToString() + "_" + j.ToString());
        //            }
        //            #region //200731 Wheel,Dress 두께 측정값 요청 (SPIL-CH VOC)
        //            for (j = 0; j < 2; j++)
        //            {
        //                for (k = 0; k < 4; k++)
        //                {
        //                    CData.JSCK_Gem_Data[i].fMeasure_Wheel_Left_Thick[j, k] = mIni.ReadD(sSec, "fMeasure_Wheel_Left_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString());
        //                    CData.JSCK_Gem_Data[i].fMeasure_Wheel_Right_Thick[j, k] = mIni.ReadD(sSec, "fMeasure_Wheel_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString());
        //                    CData.JSCK_Gem_Data[i].fMeasure_Dress_Left_Thick[j, k] = mIni.ReadD(sSec, "fMeasure_Dress_Left_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString());
        //                    CData.JSCK_Gem_Data[i].fMeasure_Dress_Right_Thick[j, k] = mIni.ReadD(sSec, "fMeasure_Dress_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString());
        //                }
        //            }
        //            #endregion

        //            for (j = 0; j < 7; j++)
        //            {
        //                for (k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //                {
        //                    CData.JSCK_Gem_Data[i].fMeasure_New_Data[j, k] = mIni.ReadD(sSec, "fMeasure_New_Data_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString());
        //                }
        //                CData.JSCK_Gem_Data[i].fMeasure_New_Min_Data[j] = mIni.ReadD(sSec, "fMeasure_New_Min_Data_" + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].fMeasure_New_Max_Data[j] = mIni.ReadD(sSec, "fMeasure_New_Max_Data_" + i.ToString() + "_" + j.ToString());
        //                CData.JSCK_Gem_Data[i].fMeasure_New_Avr_Data[j] = mIni.ReadD(sSec, "fMeasure_New_Avr_Data_" + i.ToString() + "_" + j.ToString());
        //            }
        //        }
        //    }
        //}

        //191118 ksg :
        public void LastSave()
        {
            Log_wt("LastSave()");

            int    j = 0;
            int    k = 0;
            string sSec = "";
            string sPath = GV.PATH_CONFIG + "LastSecGem.cfg";

            //CIni mIni = new CIni(sPath);
            IniFile mIni = new IniFile();
            sSec = "SecGem";

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    LastSave();

                });

            }else{

                for(int i = 0; i < CData.JSCK_Max_Cnt; i++)
                {
                    //if(!((i == 0) || (i == 4) || (i == 7) || (i == 10) || (i == 11) || (i == 14) || (i == 20))) continue;
                    if ((i == 1)  || (i == 3)  || (i == 4)  || (i == 5)  || (i == 6)  || (i == 7) || (i == 10) ||
                        (i == 11) || (i == 12) || (i == 13) || (i == 16) || (i == 17) || (i == 23))     continue;   // 20200430 LCY : 사용하는 배열만 저장
                    mIni[sSec]["SaBun_"                     + i.ToString()] =  CData.JSCK_Gem_Data[i].sSaBUN;
                    _check_RecipeName(ref CData.JSCK_Gem_Data[i].sCurr_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
                    mIni[sSec]["Curr_Recipe_Name_"          + i.ToString()] = CData.JSCK_Gem_Data[i].sCurr_Recipe_Name;
                    _check_RecipeName(ref CData.JSCK_Gem_Data[i].sNew_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
                    mIni[sSec]["New_Recipe_Name_"           + i.ToString()] = CData.JSCK_Gem_Data[i].sNew_Recipe_Name; 
                    for(j = 0; j < 5; j++)
                    {
                        mIni[sSec]["Curr_Wheel_Name_"  + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].sCurr_Wheel_Name[j];
                        mIni[sSec]["New_Wheel_Name_"   + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].sNew_Wheel_Name [j];
                        mIni[sSec]["sCurr_Dress_Name_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].sCurr_Dress_Name[j];
                        mIni[sSec]["sNEW_Dress_Name_"  + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].sNEW_Dress_Name [j];
                        mIni[sSec]["sCurr_Tool_Name_"  + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].sCurr_Tool_Name [j];
                        mIni[sSec]["sNEW_Tool_Name_"   + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].sNEW_Tool_Name  [j];
                    }
                
                    mIni[sSec]["sLot_Name_"                     + i.ToString()] = CData.JSCK_Gem_Data[i].sLot_Name; 
                    mIni[sSec]["sLD_MGZ_ID_"                    + i.ToString()] = CData.JSCK_Gem_Data[i].sLD_MGZ_ID; 
                    mIni[sSec]["sLD_MGZ_ID_New_"                + i.ToString()] = CData.JSCK_Gem_Data[i].sLD_MGZ_ID_New; 
                    mIni[sSec]["sInRail_Strip_ID_"              + i.ToString()] = CData.JSCK_Gem_Data[i].sInRail_Strip_ID; 
                    mIni[sSec]["sInRail_Verified_Strip_ID_"     + i.ToString()] = CData.JSCK_Gem_Data[i].sInRail_Verified_Strip_ID ; 
                    mIni[sSec]["sULD_MGZ_ID_"                   + i.ToString()] = CData.JSCK_Gem_Data[i].sULD_MGZ_ID; 
                    mIni[sSec]["nLD_MGZ_Cnt_"                   + i.ToString()] = CData.JSCK_Gem_Data[i].nLD_MGZ_Cnt; 
                    mIni[sSec]["nLD_MGZ_Strip_Cnt_"             + i.ToString()] = CData.JSCK_Gem_Data[i].nLD_MGZ_Strip_Cnt; 
                    mIni[sSec]["nLot_Strip_Cnt_"                + i.ToString()] = CData.JSCK_Gem_Data[i].nLot_Strip_Cnt; 
                    mIni[sSec]["nUnit_LEFT_Work_Start_Cnt_"     + i.ToString()] = CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_Start_Cnt ; 
                    mIni[sSec]["nUnit_LEFT_Work_End_Cnt_"       + i.ToString()] = CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_End_Cnt; 
                    mIni[sSec]["nUnit_RIGHT_Work_Start_Cnt_"    + i.ToString()] = CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_Start_Cnt;
                    mIni[sSec]["nUnit_RIGHT_Work_End_Cnt_"      + i.ToString()] = CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_End_Cnt;
                    mIni[sSec]["nUnit_TOTAL_Work_Start_Cnt_"    + i.ToString()] = CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_Start_Cnt;
                    mIni[sSec]["nUnit_TOTAL_Work_End_Cnt_"      + i.ToString()] = CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_End_Cnt;
                    mIni[sSec]["nStrip_Insp_Result_"            + i.ToString()] = CData.JSCK_Gem_Data[i].nStrip_Insp_Result;
                    mIni[sSec]["nULD_MGZ_Cnt_"                  + i.ToString()] = CData.JSCK_Gem_Data[i].nULD_MGZ_Cnt;
                    mIni[sSec]["nULD_MGZ_Port_"                 + i.ToString()] = CData.JSCK_Gem_Data[i].nULD_MGZ_Port;
                    mIni[sSec]["nULD_MGZ_Strip_Cnt_"            + i.ToString()] = CData.JSCK_Gem_Data[i].nULD_MGZ_Strip_Cnt;
                    mIni[sSec]["nDF_User_Mode_"                 + i.ToString()] = CData.JSCK_Gem_Data[i].nDF_User_Mode;
                    mIni[sSec]["nDF_User_New_Mode_"             + i.ToString()] = CData.JSCK_Gem_Data[i].nDF_User_New_Mode;

                    for(j = 0; j < 5; j++)
                    {
                        for(k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        {
                            mIni[sSec]["fMeasure_Data_"     + i.ToString() + "_" + j.ToString() + "_" + k.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Data[j,k];
                        }
                        mIni[sSec]["fMeasure_Min_Data_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Min_Data[j];
                        mIni[sSec]["fMeasure_Max_Data_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Max_Data[j];
                        mIni[sSec]["fMeasure_Avr_Data_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Avr_Data[j];
                    }

                    #region //200731 Wheel,Dress 두께 측정값 요청 (SPIL-CH VOC)
                    for (j = 0; j < 2; j++)
                    {
                        for (k = 0; k < 4; k++)
                        {
                            mIni[sSec]["fMeasure_Wheel_Left_Thick_"  + i.ToString() + "_" + j.ToString() + "_" + k.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Wheel_Left_Thick[j, k];
                            mIni[sSec]["fMeasure_Wheel_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Wheel_Right_Thick[j, k];
                            mIni[sSec]["fMeasure_Dress_Left_Thick_"  + i.ToString() + "_" + j.ToString() + "_" + k.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Dress_Left_Thick[j, k];
                            mIni[sSec]["fMeasure_Dress_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_Dress_Right_Thick[j, k];
                        }
                    }
                    #endregion
                    for (j = 0; j < 5; j++)
                    {
                        for(k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        {
                            mIni[sSec]["fMeasure_New_Data_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_New_Data[j,k];                        
                        }
                        mIni[sSec]["fMeasure_New_Min_Data_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_New_Min_Data[j];
                        mIni[sSec]["fMeasure_New_Max_Data_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_New_Max_Data[j];
                        mIni[sSec]["fMeasure_New_Avr_Data_" + i.ToString() + "_" + j.ToString()] = CData.JSCK_Gem_Data[i].fMeasure_New_Avr_Data[j];
                    }
                }

                // 2020-11-09, jhLee, SPIL VOC 대응용, 현재 처리중인 LOT open 수량
                mIni[sSec]["nLotOpenCount"] = m_nLotOpenCount;
            }

            mIni.Save(sPath);
        }

        //public void LastSave_()
        //{
        //    Log_wt("LastSave()");
        //    int    j = 0;
        //    int    k = 0;
        //    string sSec = "";
        //    string sVal = "";
        //    string sPath = GV.PATH_CONFIG + "LastSecGem.cfg";
        //    CIni mIni = new CIni(sPath);

        //    sSec = "SecGem";


        //    if (listLog.InvokeRequired)
        //    {
        //        Invoke((MethodInvoker)delegate ()
        //        {
        //            Console.WriteLine("---- a0");
        //            LastSave();

        //        });

        //    }
        //    else
        //    {

        //        for (int i = 0; i < CData.JSCK_Max_Cnt; i++)
        //        {
        //            //if(!((i == 0) || (i == 4) || (i == 7) || (i == 10) || (i == 11) || (i == 14) || (i == 20))) continue;
        //            if ((i == 1)  || (i == 3)  || (i == 4)  || (i == 5)  || (i == 6)  || (i == 7) || (i == 10) ||
        //                (i == 11) || (i == 12) || (i == 13) || (i == 16) || (i == 17) || (i == 23))     continue;// 20200430 LCY : 사용하는 배열만 저장
        //            mIni.Write(sSec, "SaBun_" + i.ToString(),               CData.JSCK_Gem_Data[i].sSaBUN);
        //            _check_RecipeName(ref CData.JSCK_Gem_Data[i].sCurr_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
        //            mIni.Write(sSec, "Curr_Recipe_Name_" + i.ToString(),    CData.JSCK_Gem_Data[i].sCurr_Recipe_Name);
        //            _check_RecipeName(ref CData.JSCK_Gem_Data[i].sNew_Recipe_Name); //200824 jhc : 잘못 저장된 Recipe Name에 대한 대비
        //            mIni.Write(sSec, "New_Recipe_Name_" + i.ToString(),     CData.JSCK_Gem_Data[i].sNew_Recipe_Name);
        //            for (j = 0; j < 5; j++)
        //            {
        //                mIni.Write(sSec, "Curr_Wheel_Name_"     + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].sCurr_Wheel_Name[j]);
        //                mIni.Write(sSec, "New_Wheel_Name_"      + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].sNew_Wheel_Name[j]);
        //                mIni.Write(sSec, "sCurr_Dress_Name_"    + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].sCurr_Dress_Name[j]);
        //                mIni.Write(sSec, "sNEW_Dress_Name_"     + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].sNEW_Dress_Name[j]);
        //                mIni.Write(sSec, "sCurr_Tool_Name_"     + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].sCurr_Tool_Name[j]);
        //                mIni.Write(sSec, "sNEW_Tool_Name_"      + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].sNEW_Tool_Name[j]);
        //            }

        //            mIni.Write(sSec, "sLot_Name_"                   + i.ToString(), CData.JSCK_Gem_Data[i].sLot_Name);
        //            mIni.Write(sSec, "sLD_MGZ_ID_"                  + i.ToString(), CData.JSCK_Gem_Data[i].sLD_MGZ_ID);
        //            mIni.Write(sSec, "sLD_MGZ_ID_New_"              + i.ToString(), CData.JSCK_Gem_Data[i].sLD_MGZ_ID_New);
        //            mIni.Write(sSec, "sInRail_Strip_ID_"            + i.ToString(), CData.JSCK_Gem_Data[i].sInRail_Strip_ID);
        //            mIni.Write(sSec, "sInRail_Verified_Strip_ID_"   + i.ToString(), CData.JSCK_Gem_Data[i].sInRail_Verified_Strip_ID);
        //            mIni.Write(sSec, "sULD_MGZ_ID_"                 + i.ToString(), CData.JSCK_Gem_Data[i].sULD_MGZ_ID);
        //            mIni.Write(sSec, "nLD_MGZ_Cnt_"                 + i.ToString(), CData.JSCK_Gem_Data[i].nLD_MGZ_Cnt);
        //            mIni.Write(sSec, "nLD_MGZ_Strip_Cnt_"           + i.ToString(), CData.JSCK_Gem_Data[i].nLD_MGZ_Strip_Cnt);
        //            mIni.Write(sSec, "nLot_Strip_Cnt_"              + i.ToString(), CData.JSCK_Gem_Data[i].nLot_Strip_Cnt);
        //            mIni.Write(sSec, "nUnit_LEFT_Work_Start_Cnt_"   + i.ToString(), CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_Start_Cnt);
        //            mIni.Write(sSec, "nUnit_LEFT_Work_End_Cnt_"     + i.ToString(), CData.JSCK_Gem_Data[i].nUnit_LEFT_Work_End_Cnt);
        //            mIni.Write(sSec, "nUnit_RIGHT_Work_Start_Cnt_"  + i.ToString(), CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_Start_Cnt);
        //            mIni.Write(sSec, "nUnit_RIGHT_Work_End_Cnt_"    + i.ToString(), CData.JSCK_Gem_Data[i].nUnit_RIGHT_Work_End_Cnt);
        //            mIni.Write(sSec, "nUnit_TOTAL_Work_Start_Cnt_"  + i.ToString(), CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_Start_Cnt);
        //            mIni.Write(sSec, "nUnit_TOTAL_Work_End_Cnt_"    + i.ToString(), CData.JSCK_Gem_Data[i].nUnit_TOTAL_Work_End_Cnt);
        //            mIni.Write(sSec, "nStrip_Insp_Result_"          + i.ToString(), CData.JSCK_Gem_Data[i].nStrip_Insp_Result);
        //            mIni.Write(sSec, "nULD_MGZ_Cnt_"                + i.ToString(), CData.JSCK_Gem_Data[i].nULD_MGZ_Cnt);
        //            mIni.Write(sSec, "nULD_MGZ_Port_"               + i.ToString(), CData.JSCK_Gem_Data[i].nULD_MGZ_Port);
        //            mIni.Write(sSec, "nULD_MGZ_Strip_Cnt_"          + i.ToString(), CData.JSCK_Gem_Data[i].nULD_MGZ_Strip_Cnt);
        //            mIni.Write(sSec, "nDF_User_Mode_"               + i.ToString(), CData.JSCK_Gem_Data[i].nDF_User_Mode);
        //            mIni.Write(sSec, "nDF_User_New_Mode_"           + i.ToString(), CData.JSCK_Gem_Data[i].nDF_User_New_Mode);

        //            for (j = 0; j < 5; j++)
        //            {
        //                for (k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //                {
        //                    mIni.Write(sSec, "fMeasure_Data_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Data[j, k]);
        //                }
        //                mIni.Write(sSec, "fMeasure_Min_Data_" + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Min_Data[j]);
        //                mIni.Write(sSec, "fMeasure_Max_Data_" + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Max_Data[j]);
        //                mIni.Write(sSec, "fMeasure_Avr_Data_" + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Avr_Data[j]);
        //            }

        //            #region //200731 Wheel,Dress 두께 측정값 요청 (SPIL-CH VOC)
        //            for (j = 0; j < 2; j++)
        //            {
        //                for (k = 0; k < 4; k++)
        //                {
        //                    mIni.Write(sSec, "fMeasure_Wheel_Left_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Wheel_Left_Thick[j, k]);
        //                    mIni.Write(sSec, "fMeasure_Wheel_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Wheel_Right_Thick[j, k]);
        //                    mIni.Write(sSec, "fMeasure_Dress_Left_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Dress_Left_Thick[j, k]);
        //                    mIni.Write(sSec, "fMeasure_Dress_Right_Thick_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), CData.JSCK_Gem_Data[i].fMeasure_Dress_Right_Thick[j, k]);
        //                }
        //            }
        //            #endregion
        //            for (j = 0; j < 5; j++)
        //            {
        //                for (k = 0; k < GV.STRIP_MEASURE_POINT_MAX/*20*/; k++) //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //                {
        //                    mIni.Write(sSec, "fMeasure_New_Data_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), CData.JSCK_Gem_Data[i].fMeasure_New_Data[j, k]);
        //                }
        //                mIni.Write(sSec, "fMeasure_New_Min_Data_" + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].fMeasure_New_Min_Data[j]);
        //                mIni.Write(sSec, "fMeasure_New_Max_Data_" + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].fMeasure_New_Max_Data[j]);
        //                mIni.Write(sSec, "fMeasure_New_Avr_Data_" + i.ToString() + "_" + j.ToString(), CData.JSCK_Gem_Data[i].fMeasure_New_Avr_Data[j]);
        //            }

        //            // 2021.05.21 SungTae Start : [추가] SPIL용 SVID List Up
        //            //mIni.Write(sSec, "Strip_Short_Side" + i.ToString(), CData.JSCK_Gem_Data[i].Strip_Short_Side);
        //            // 2021.05.21 SungTae End
        //        }
        //    }
        //}

        //
        // 2020-11-09, jhLee, SPIL向 START/RESTART구분을 위해 현재 LOT이 open되어 진행중인지 여부를 조회한다.
        // return   true : LOT이 Open되어있다.
        //          false : LOT이 아직 Open되지 않았다.
        public bool IsLotOpen()
        {
            return (m_nLotOpenCount > 0);  // Lot Open이 진행되어 처리중인가 ? START/RESTART 구분용, 진행중인 LOT 수량 0이면 아직 없음
        }


        public string Get_Now()
        {
            string sRet = string.Empty;
            DateTime now = DateTime.Now;

            sRet = now.ToString("yyyy-MM-dd HH:mm:ss fff");
            return sRet;
        }

        public void Get_Show()
        {
            sMsgdata = string.Format("Get_Show()");
            Log_wt(sMsgdata);

            ECV.m_nPort                 = TransShort(txtHSMS_Port1.Text);
            ECV.m_nDeviceID             = TransShort(txtHSMS_DID1.Text);
            ECV.m_nT3                   = TransShort(txtHSMS_T31.Text);
            ECV.m_nT5                   = TransShort(txtHSMS_T51.Text);
            ECV.m_nT6                   = TransShort(txtHSMS_T61.Text);
            ECV.m_nT7                   = TransShort(txtHSMS_T71.Text);
            ECV.m_nT8                   = TransShort(txtHSMS_T81.Text);
            ECV.m_nLinkTestInterval     = TransShort(txtHSMS_LT1.Text);
            ECV.m_nCommReqeustTimeout   = TransShort(txtHSMS_CRT1.Text);
            ECV.m_bPassive              = true;
        }

        public void Set_ECID()
        {
            Log_wt(string.Format("Set_ECID()"));

            ECV.m_nCommReqeustTimeout   = 5;
            ECV.m_nPort                 = 5000;
            ECV.m_nDeviceID             = 0;
            ECV.m_nT3                   = 15;
            ECV.m_nT5                   = 5;
            ECV.m_nT6                   = 6;
            ECV.m_nT7                   = 7;
            ECV.m_nT8                   = 8;
            ECV.m_nLinkTestInterval     = 60;
            ECV.m_bPassive              = true;
            ECV.m_szModelName           = "SG2004X";
            ECV.m_szSoftRev             = "20190803";             
        }

        public void Set_GEM()
        {
            Log_wt(string.Format("Set_GEM()"));

            mgem.Port               = (short)ECV.m_nPort;
            mgem.DeviceID           = (short)ECV.m_nDeviceID;
            mgem.PassiveMode        = (ECV.m_bPassive == true) ? (short)1 : (short)0;
            mgem.LinkTestInterval   = (short)ECV.m_nLinkTestInterval;
            mgem.T3                 = (short)ECV.m_nT3;
            mgem.T5                 = (short)ECV.m_nT5;
            mgem.T6                 = (short)ECV.m_nT6;
            mgem.T7                 = (short)ECV.m_nT7;
            mgem.T8                 = (short)ECV.m_nT8;
            mgem.CommRequest        = (short)ECV.m_nCommReqeustTimeout;

        }

        public void Set_Show()
        {
            Log_wt(string.Format("Set_Show()"));

            txtHSMS_Port1.Text  = ECV.m_nPort.ToString();
            txtHSMS_DID1.Text   = ECV.m_nDeviceID.ToString();
            txtHSMS_LT1.Text    = ECV.m_nLinkTestInterval.ToString();
            txtHSMS_CRT1.Text   = ECV.m_nCommReqeustTimeout.ToString();
            txtHSMS_T31.Text    = ECV.m_nT3.ToString();
            txtHSMS_T51.Text    = ECV.m_nT5.ToString();
            txtHSMS_T61.Text    = ECV.m_nT6.ToString();
            txtHSMS_T71.Text    = ECV.m_nT7.ToString();
            txtHSMS_T81.Text    = ECV.m_nT8.ToString();
        }

        public void Start_GEM()
        {
            if(nLoad_flag == 0 ) nLoad_flag = 1;
            else { return;  }

            Log_wt(string.Format("Start_GEM()"));

            mgem.DeviceID           = 0;    //default = 0
            mgem.PassiveMode        = 1;    //default = 1    active mode
            mgem.T3                 = 30;   //wait interval time for response
            mgem.T5                 = 30;   //wait interval time for reconnected
            mgem.T6                 = 30;   //wait interval time for control message
            mgem.T7                 = 30;   //wait interval time for between connected, selected
            mgem.T8                 = 30;   //multi message accepted time
            mgem.Port               = 5000; //default = 5000
            mgem.RetryCount         = 0;    //default = 0
            mgem.LinkTestInterval   = 30;   //default = 30    sec
            mgem.CommRequest        = 2;    //interval time between selected    S1F13w
            mgem.SetIP(         string.Format("127.0.0.1"));        //default = 127.0.0.1
            mgem.SetLogFile(    string.Format("Log\\GEM.log"));     //default 
            mgem.SetFormatFile( string.Format("FORMAT.SML"));       //
            mgem.SetFormatCheck(true);
            mgem.WriteUserLog(  string.Format("{0} C# 전용 EZGEM DLL 버전 구동 시작", Get_Now()));
            
            Get_Show();
            Set_GEM();
            Set_Show();

            Add_ALID();
            Add_CEID();
            Add_ECID();
            Add_SVID();

            mgem.SetFormatCode(GEM.sALID,       GEM.sU4);
            mgem.SetFormatCode(GEM.sCEID,       GEM.sU4);
            mgem.SetFormatCode(GEM.sDATAID,     GEM.sU4);
            mgem.SetFormatCode(GEM.sRPTID,      GEM.sU4);
            mgem.SetFormatCode(GEM.sSVID,       GEM.sU4);
            mgem.SetFormatCode(GEM.sTRACEID,    GEM.sU4);

            mgem.SetModelName(ECV.m_szModelName);
            mgem.SetSoftRev(ECV.m_szSoftRev);

            mgem.DisableAutoReply(2, 41);   // Host Commucation Send
            mgem.DisableAutoReply(2, 49);   // Host Commucation Send

            mgem.DisableAutoReply(7, 1);    // PPI (Process Program Load Inquire)
            mgem.DisableAutoReply(7, 3);    // PPS (Process Program Send)
            mgem.DisableAutoReply(7, 5);    // PPR (Process Program Request)
            mgem.DisableAutoReply(7, 17);   // DPS (Delete Process Program Send)
            mgem.DisableAutoReply(7, 19);   // RER (Current EPPD Request)

            if (mgem.Start() == 0)
            {
                mgem.GoOnlineRemote();

                m_iConnState = Convert.ToInt32(JSCK.eConn.Connected);
                m_iCtrlState = Convert.ToInt32(JSCK.eCont.Remote);

                if (mgem.PassiveMode == 1)  { Log_wt("Passive mode"); }
                else                        { Log_wt("Active mode"); }

                Log_wt(string.Format("Port = {0}", ECV.m_nPort));
                Log_wt(string.Format("Device ID = {0}", ECV.m_nDeviceID));
                Log_wt("EZGEM Started");

                btnConn_Start.Enabled   = (m_iConnState == Convert.ToInt32(JSCK.eConn.Connected))    ? false : true;
                btnConn_Stop.Enabled    = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;
            }
        }

        public void Save_Config()
        {
            string sSec = string.Empty;
            string sKey = string.Empty;
            string sVal = string.Empty;
            string sPath = string.Empty;

            Log_wt(string.Format("Save_Config()"));

            sPath = fileinfo.Directory.FullName + @"\GEM.INI";  
            sSec = GEM.sHSMS_HSMS;  
            sKey = GEM.sHSMS_Port;              sVal = string.Empty;    sVal = ECV.m_nPort.ToString();                  SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_DeviceID;          sVal = string.Empty;    sVal = ECV.m_nDeviceID.ToString();              SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_T3;                sVal = string.Empty;    sVal = ECV.m_nT3.ToString();                    SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_T5;                sVal = string.Empty;    sVal = ECV.m_nT5.ToString();                    SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_T6;                sVal = string.Empty;    sVal = ECV.m_nT6.ToString();                    SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_T7;                sVal = string.Empty;    sVal = ECV.m_nT7.ToString();                    SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_T8;                sVal = string.Empty;    sVal = ECV.m_nT8.ToString();                    SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_Passive;           sVal = string.Empty;    sVal = (ECV.m_bPassive == true) ? "1" : "0";    SetIniValue(sSec, sKey, sVal, sPath);
            
            sSec = GEM.sHSMS_GEM;
            sKey = GEM.sHSMS_LinkTest;          sVal = string.Empty;    sVal = ECV.m_nLinkTestInterval.ToString();      SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_RequestTimeout;    sVal = string.Empty;    sVal = ECV.m_nCommReqeustTimeout.ToString();    SetIniValue(sSec, sKey, sVal, sPath);

            sSec = GEM.sHSMS_CONFIGURATION;
            sKey = GEM.sHSMS_UseRecipeFolder; sVal = string.Empty; sVal = UseRecipeFolder.ToString(); SetIniValue(sSec, sKey, sVal, sPath);
            sKey = GEM.sHSMS_RMSPath; sVal = string.Empty; sVal = RMSPath.ToString(); SetIniValue(sSec, sKey, sVal, sPath);

        }
        #endregion HSMS - function

        #region GEM - function
        public bool Try_StringToInt(string sStr, ref int iRet)
        {
            bool bRet = false;
            int j = 0;

            if (int.TryParse(sStr, out j))
            {
                iRet = j;
                bRet = true;
            }
            else
            {
                bRet = false;
            }

            return bRet;
        }

        public uint TranShort(string sVal)
        {
            int m = 0;
            uint s = 0;

            try
            {
                m = Int32.Parse(sVal);
            }
            catch
            {
                m = 0;
            }

            s = (uint)m;

            return s;
        }

        public bool isExistRecipe(string strPPID)
        {
            Log_wt(string.Format("isExistRecipe(%s)", strPPID));

            string spath = string.Empty;

            // 2021.08.16 SungTae Start : [삭제] 미사용으로 추후 완전 삭제 예정
            //DirectoryInfo d;
            //FileInfo[] fs;
            // 2021.08.16 SungTae End

            spath = string.Format("{0}\\{1}.dev", m_sExePath, strPPID);

            if (File.Exists(spath)) return true;
            else                    return false;
        }

        public void AddStripFromHost(List<string> pStrip, string LotId)
        {
            //m_pHostLot.DeleteAll(); //S2F41을 통해서 Host에서 전송한 Strip ID List를 잠시 가지고 있는 전역 변수.
            Log_wt(string.Format("AddStripFromHost()"));

            // 2021.08.09 SungTae Start : [추가] (ASE-KR VOC) Remote 상태에서 Carrier List 처리를 위해 추가
            //작업을 시작할 리스트를 초기화 후 저장
            m_pStartCarrierList.ClearCarrier();

            if (m_pStartCarrierList.Count < 1)
            {
                for (int i = 0; i < pStrip.Count; i++)
                {
                    // 2021.08.31 SungTae Start : [수정] StartCarrierList 초기화 후 왜 HostLOT에 Data를 넣을까?
                    //CData.m_pHostLOT.AddCarrier(pStrip[i]);
                    //CData.m_pHostLOT.SetStripStatus(pStrip[i], Convert.ToInt32(JSCK.eCarrierStatus.Read));
                    m_pStartCarrierList.AddCarrier(pStrip[i]);
                    m_pStartCarrierList.SetStripStatus(pStrip[i], Convert.ToInt32(JSCK.eCarrierStatus.Read));
                    // 2021.08.31 SungTae End
                }
            }

            //// 전체 리스트를 저장.
            //CData.m_pHostLOT.ClearCarrier();

            //if (CData.m_pHostLOT.Count < 1)
            //{
            //    for (int i = 0; i < pStrip.Count; i++)
            //    {
            //        CData.m_pHostLOT.AddCarrier(pStrip[i]);
            //    }
            //}

            // 전체 리스트를 저장.
            if (CData.IsMultiLOT())
            {
                int nIdx = CData.LotMgr.GetListIndexNum(LotId);

                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.ClearCarrier();

                if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count < 1)
                {
                    for (int i = 0; i < pStrip.Count; i++)
                    {
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.AddCarrier(pStrip[i]);
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(pStrip[i], Convert.ToInt32(JSCK.eCarrierStatus.Read));      // 2021.08.31 SungTae : [추가] 누락되어 있어서 추가

                        Log_wt(string.Format("STRIP_ID {0:00} : {1}, STATUS : {2}", i + 1, pStrip[i], JSCK.eCarrierStatus.Read.ToString()));
                    }
                }
            }
            else
            {
                CData.m_pHostLOT.ClearCarrier();

                if (CData.m_pHostLOT.Count < 1)
                {
                    for (int i = 0; i < pStrip.Count; i++)
                    {
                        CData.m_pHostLOT.AddCarrier(pStrip[i]);
                        CData.m_pHostLOT.SetStripStatus(pStrip[i], Convert.ToInt32(JSCK.eCarrierStatus.Read));      // 2021.08.31 SungTae : [추가] 누락되어 있어서 추가

                        Log_wt(string.Format("STRIP_ID {0:00} : {1}, STATUS : {2}", i + 1, pStrip[i], JSCK.eCarrierStatus.Read.ToString()));
                    }
                }
            }
            // 2021.08.09 SungTae End

            // 호스트가 전달해준 작업해야 할 전체 list 
            Make_SVID( Convert.ToInt32(JSCK.eSVID.Total_Carrier_List) );
        }

        public void Get_DeviceFileList()
        {
            Log_wt(string.Format("Get_DeviceFileList()"));

            string spath = string.Empty;
            DirectoryInfo d;
            FileInfo[] fs;

            mDevice_List.Clear();
            cboDevice.Items.Clear();
            /*if (CData.Opt.bSecsUse == true)
            {
                spath = string.Format("{0}\\Device\\GEM", fileinfo.Directory.FullName);
            }else*/
            //{
            //    spath = string.Format("{0}\\Device", fileinfo.Directory.FullName);
            //}
            d = new DirectoryInfo(m_sExePath);
            if (d.Exists == false)
            {
                d.Create();
            }
            fs = d.GetFiles("*.dev");

            foreach (FileInfo f in fs)
            {
                string fn = string.Empty;

                fn = f.Name.Replace(".dev", "");
                cboDevice.Items.Add(fn);
                mDevice_List.Add(fn);
            }
        }

        public void Set_SVID(int iSVID, string sVal)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    Set_SVID(iSVID, sVal);

                });
            }
            else {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                //Log_wt(string.Format("Set_SVID({0}={1})", iSVID, sVal));
                Log_wt(string.Format("Set_SVID({0}) = {1}", iSVID, sVal));

                string sMsg = string.Empty;
                sMsg = string.Format("{0}={1}", iSVID, sVal);

                //mgem.SetSVIDValue( iSVID, sVal );
                Send_SVID(sMsg);
            }
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA://장비 프로그램에서 호출하여 윈도우 메세지를 통해서 form에서 수신하여 gem 변수값에 저장 및 이벤트 보고 루틴.                        
                    {
                        COPYDATASTRUCT lParam = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));

                        switch ((int)lParam.dwData)
                        {
                            case (int)JSCK.eMID.SVID:
                                Log_wt(string.Format("SVID={0}", lParam.lpData));
                                On_SVID(lParam.lpData);
                                break;

                            case (int)JSCK.eMID.CEID:
                                Log_wt(string.Format("CEID={0}", lParam.lpData));
                                On_CEID(lParam.lpData);
                                break;

                            case (int)JSCK.eMID.ECID:
                                Log_wt(string.Format("ECID={0}", lParam.lpData));
                                On_ECID(lParam.lpData);
                                break;

                            case (int)JSCK.eMID.AlarmOccur:
                                Log_wt(string.Format("ALARMOCCUR={0}", lParam.lpData));
                                On_AlarmOccur(lParam.lpData);
                                break;

                            case (int)JSCK.eMID.AlarmReset:
                                Log_wt(string.Format("ALARMRESET={0}", lParam.lpData));
                                On_AlarmReset(lParam.lpData);
                                break;

                            case (int)JSCK.eMID.LOG:
                                //Log(string.Format("LOG={0}", lParam.lpData));
                                On_Log(lParam.lpData);
                                break;

                            default: break;
                        }
                        break;
                    }
                default: break;
            }

            base.WndProc(ref m);
        }
        public void On_Log(string strLog)
        {
            string stime = string.Empty;
            DateTime dtNow = DateTime.Now;

            stime = dtNow.ToString("[yyyy-MM-dd HH:mm:ss fff]");
            listLog.Items.Add(string.Format("{0}    {1}", stime, strLog));

            while (listLog.Items.Count > 100)
            { listLog.Items.RemoveAt(0); }

            listLog.SetSelected(listLog.Items.Count - 1, true);
        }

        public void On_AlarmOccur(string sMsg)
        {
            int     iALID = 0;
            short   nALCD = 0;

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    On_AlarmOccur(sMsg);
                });
            }
            else
            {
                Log_wt(string.Format("On_AlarmOccur({0})", sMsg));

                if (Try_StringToInt(sMsg, ref iALID))
                {
                    nALCD = mgem.GetAlarmCode(iALID);
                    nALCD += 128;
                    mgem.SendAlarmReport(iALID, nALCD);
                }
                else
                {
                    Log_wt(string.Format("Unknown AL_O={0}", sMsg));
                }
            }
        }

        public void On_AlarmReset(string sMsg)
        {
            int     iALID = 0;
            short   nALCD = 0;

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    On_AlarmReset(sMsg);
                });
            }
            else
            {
                Log_wt(string.Format("On_AlarmReset({0})", sMsg));

                if (Try_StringToInt(sMsg, ref iALID))
                {
                    mgem.SendAlarmReport(iALID, nALCD);
                }
                else
                {
                    Log_wt(string.Format("Unknown AL_R={0}", sMsg));
                }
            }
        }

        public void On_CEID(string sMsg)
        {
            int iCEID = 0;
            Log_wt(string.Format("On_CEID({0})", sMsg));

            if (Try_StringToInt(sMsg, ref iCEID))
            {
                mgem.SendEventReport(iCEID);
            }
            else
            {
                Log_wt(string.Format("Unknown CEID={0}", sMsg));
            }
        }

        public void On_ECID(string sMsg)
        {
            int iECID = 0;
            string[] sArrM = sMsg.Split('=');

            Log_wt(string.Format("On_ECID({0})", sMsg));

            if (sArrM.Length > 1)
            {
                if (Try_StringToInt(sArrM[0], ref iECID))
                {//일반적인 ECID 값을 세팅
                    string sVal = sArrM[1];
                    mgem.SetECValue(iECID, sVal);
                }
                else
                {
                    Log_wt(string.Format("Unknown ECID={0}", sMsg));
                }
            }
        }

        public void On_SVID(string sMsg)
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { On_StdA_SVID(sMsg); }
            else { On_StdB_SVID(sMsg); }

        }
        public void On_StdA_SVID(string sMsg)
        {
            int iSVID = 0;
            string[] sArrM = sMsg.Split('=');
            string sVal = string.Empty;

            Log_wt(string.Format("On_SVID({0})", sMsg));

            if (sArrM.Length > 1)
            {
                if (Try_StringToInt(sArrM[0], ref iSVID))
                {
                    sVal = sArrM[1];

                    // 2020 07 16 skpark SVID에서 OBJECT리스트를 String으로 받아서 Split로 OBJECT에 접근 --------------------------------------------
                    if (iSVID == (int)JSCK.eSVID.Carrier_List_Read      || iSVID == (int)JSCK.eSVID.Carrier_List_Validate   || iSVID == (int)JSCK.eSVID.Carrier_List_Started ||
                        iSVID == (int)JSCK.eSVID.Carrier_List_Complete  || iSVID == (int)JSCK.eSVID.Carrier_List_Reject     || iSVID == (int)JSCK.eSVID.Total_Carrier_List   ||
                        iSVID == (int)JSCK.eSVID.Proceed_Carrier_List )
                    {
                        if( sVal.Length < 1 )
                        {
                            mgem.CloseMsg(iSVID);
                            mgem.OpenListItem(iSVID);
                            mgem.CloseListItem(iSVID);
                        }
                        else
                        {
                            mgem.CloseMsg(iSVID);
                            mgem.OpenListItem(iSVID);

                            string[] strCarrierList = sVal.Split(',');

                            for( int j = 0 ; j < strCarrierList.Length ; j++ )
                            {
                                string strTemp = strCarrierList[j];
                                mgem.AddAsciiItem(iSVID, strTemp, strTemp.Length);
                            }

                            mgem.CloseListItem(iSVID);
                        }
                    }
                    else
                    {
                        mgem.SetSVIDValue(iSVID, sVal);
                    }

                    Log_wt(string.Format("SVID({0})={1}", iSVID, sVal));
                    //----------------------------------------------------------------------------------------------------------------------
                }
                else
                {
                    Log_wt(string.Format("Unknown SVID={0}", sMsg));
                }
            }
        }
        public void On_StdB_SVID(string sMsg)
        {
            int iSVID = 0;
            string[] sArrM = sMsg.Split('=');
            string sVal = string.Empty;

            Log_wt(string.Format("On_SVID({0})", sMsg));

            if (sArrM.Length > 1)
            {
                if (Try_StringToInt(sArrM[0], ref iSVID))
                {
                    sVal = sArrM[1];

                    // 2020 07 16 skpark SVID에서 OBJECT리스트를 String으로 받아서 Split로 OBJECT에 접근 --------------------------------------------
                    if (iSVID == (int)JSCK.eSVID.Carrier_List_Read      || iSVID == (int)JSCK.eSVID.Carrier_List_Validate   || iSVID == (int)JSCK.eSVID.Carrier_List_Started ||
                        iSVID == (int)JSCK.eSVID.Carrier_List_Complete  || iSVID == (int)JSCK.eSVID.Carrier_List_Reject     || iSVID == (int)JSCK.eSVID.Total_Carrier_List   ||
                        iSVID == (int)JSCK.eSVID.Proceed_Carrier_List)
                    {
                        if (sVal.Length < 1)
                        {
                            mgem.CloseMsg(iSVID);
                            mgem.OpenListItem(iSVID);
                            mgem.CloseListItem(iSVID);
                        }
                        else
                        {
                            mgem.CloseMsg(iSVID);
                            mgem.OpenListItem(iSVID);

                            string[] strCarrierList = sVal.Split(',');

                            for (int j = 0; j < strCarrierList.Length; j++)
                            {
                                string strTemp = strCarrierList[j];
                                mgem.AddAsciiItem(iSVID, strTemp, strTemp.Length);
                            }

                            mgem.CloseListItem(iSVID);
                        }
                    }
                    else
                    {
                        mgem.SetSVIDValue(iSVID, sVal);
                    }

                    Log_wt(string.Format("SVID({0})={1}", iSVID, sVal));
                    //----------------------------------------------------------------------------------------------------------------------
                }
                else
                {
                    Log_wt(string.Format("Unknown SVID={0}", sMsg));
                }
            }
        }

        public void Clear_CarrierList()
        {
            Log_wt(string.Format("Clear_CarrierList()"));

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련 수정
            //CData.m_pHostLOT.ClearCarrier();
            
            if (CData.IsMultiLOT())
            {
                int nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);
                
                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.ClearCarrier();
            }
            else
            {
                CData.m_pHostLOT.ClearCarrier();
            }
            // 2021.08.25 SungTae End

            m_sReadLOT_T = string.Empty;

            m_pStartCarrierList.ClearCarrier();
            m_pMgzGoodCarrierList.ClearCarrier();
            m_pMgzNGCarrierList.ClearCarrier();
            m_pEndedCarrierList.ClearCarrier();
            m_pReadCarrierList.ClearCarrier();
            m_pRejectCarrierList.ClearCarrier();
            m_pLoadCarrierList.ClearCarrier();      // 2021.10.21 SungTae : [추가]
        }

        public void LOT_Verified()
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
                //------------------------------------------------------------------------------------------------------
                Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Verified).ToString());

                // 2021.09.01 SungTae Start : [수정]
                //Log_wt(string.Format("LOT_Verified()"));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).", Convert.ToInt32(JSCK.eCEID.LOT_Verified), JSCK.eCEID.LOT_Verified.ToString()));
                // 2021.09.01 SungTae End
            }
            else
            {

            }
        }
        #endregion GEM - function

        #region CEID Event - function
        /// <summary>
        /// CEID 999002    Prcess State Change
        /// </summary>
        /// <param name="iStat"></param>
        public void Set_ProcStateChange(int iStat)
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { Set_StdA_ProcStateChange(iStat); }
            else { Set_StdB_ProcStateChange(iStat); }
        }
        public void Set_StdA_ProcStateChange(int iStat)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :
            if (m_iPS == iStat){ return; }

            // 2021.09.01 SungTae Start : [수정]
            // Log_wt(string.Format("Set_ProcStateChange({0})", iStat));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), Process State : {2} -> {3}",
                                                                        Convert.ToInt32(JSCK.eCEID.Process_State_Chagne),
                                                                        JSCK.eCEID.Process_State_Chagne.ToString(),
                                                                        m_iPS, iStat));
            // 2021.09.01 SungTae End

            // 2022.04.05 SungTae : [수정] (ASE_KH VOC) Multi-LOT(Auto In/Out) 관련 Company 조건 추가
            // 2020-10-19, jhLee : SPIL社가 아니거나 RCMD START에 의해 동작하도록 설정되어있지 않다면 설비 AUTO로의 전환을 이곳에서 보고한다.
            //  HOST로부터 RCMD START를 받아 처리하도록 되어있다면 이곳에서 보내지 않고 CSQ_Main에서 직접 전송한다.
            //if ((CData.CurCompany != ECompany.SPIL) || (CData.Opt.iRCMDStartTimeout <= 0))
            if ((CData.CurCompany != ECompany.SPIL && CData.CurCompany != ECompany.ASE_K12) || (CData.Opt.iRCMDStartTimeout <= 0))
            {
                if (iStat == (int)JSCK.eProcStatus.Run)
                {
                    // 2022.04.05 SungTae : [수정] (ASE_KH VOC) Multi-LOT(Auto In/Out) 관련 Company 조건 추가
                    // 2021-06-22, jhLee, SPIL VOC, 진행중인 LOT이 있다면 Restart를 전송한다.
                    //if ((CData.CurCompany == ECompany.SPIL) && CData.GemForm.IsLotOpen())
                    if ((CData.CurCompany == ECompany.SPIL || CData.CurCompany == ECompany.ASE_K12) && CData.GemForm.IsLotOpen())
                    {
                        // RESTART를 상태 변화 없이 HOST로 송신한다.
                        Set_MachineReStart(iStat, true);
                    }
                    else
                    {
                        // START를 상태 변화 없이 HOST로 송신한다.
                        Set_MachineStart(iStat);
                    }

                }
            }

            m_iPrePS = m_iPS;
            m_iPS = iStat;

            Set_SVID (Convert.ToInt32(JSCK.eSVID.Previous_Process_Status), m_iPrePS.ToString());
            Set_SVID (Convert.ToInt32(JSCK.eSVID.Current_Process_Status ), m_iPS.ToString());

            Send_CEID(Convert.ToInt32(JSCK.eCEID.Process_State_Chagne   ).ToString());
        }
        public void Set_StdB_ProcStateChange(int iStat)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :
            if (m_iPS == iStat) { return; }

            // 2021.09.01 SungTae Start : [수정]
            // Log_wt(string.Format("Set_ProcStateChange({0})", iStat));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), Process State : {2} -> {3}",
                                                                        Convert.ToInt32(JSCK.eBCEID.Equipment_State_Change),
                                                                        JSCK.eBCEID.Equipment_State_Change.ToString(),
                                                                        m_iPS, iStat));

            m_iPrePS = m_iPS;
            m_iPS = iStat;

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Previous_Equipment_State), m_iPrePS.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Equipment_State), m_iPS.ToString());

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Equipment_State_Change).ToString());
        }
       // HOST에게 설비가 START 되었음을 전송한다.
        public void Set_MachineStart(int nStats, bool bForce=false)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            // 2020-10-19, jhLee, SPIL社 전용, HOST에게 RCMD를 요청하기 위해 설비 시작전에 999011 (START)를 미리 전송한다.
            if (!bForce)        // RCMD START 지원하지 않는다면 이전 루틴 수행
            {
                if (m_iPS == nStats) { return; }
                m_iPS = nStats;
            }

            // 2021.03.09 SungTae Start : Local Mode 관련 조건 추가 및 수정
            //Log_wt(string.Format("Set_MachineStart({0}), LotOpenCount:{1}, On-line Mode:{2}", nStats, m_nLotOpenCount));

            if (m_nLotOpenCount > 0 && CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Remote))
            {
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), State : {2}, LotOpenCount : {3}, On-line Mode : {4}",
                                                                        Convert.ToInt32(JSCK.eCEID.Machine_Start),
                                                                        JSCK.eCEID.Machine_Start.ToString(),
                                                                        nStats, m_nLotOpenCount, JSCK.eCont.Remote.ToString()));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID)     , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            }
            else
            {
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), State : {2}, LotOpenCount : {3}, On-line Mode : {4}",
                                                                        Convert.ToInt32(JSCK.eCEID.Machine_Start),
                                                                        JSCK.eCEID.Machine_Start.ToString(),
                                                                        nStats, m_nLotOpenCount, JSCK.eCont.Local.ToString()));

                if(CData.IsMultiLOT())
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.LotMgr.LoadingLotName);
                else
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.LotInfo.sLotName);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID), string.Empty);
            }
            // 2021.03.09 SungTae End

            Send_CEID(Convert.ToInt32(JSCK.eCEID.Machine_Start).ToString());    // 999011
        }

        // 2020-11-09, jhLee, HOST에게 설비가 LOT 진행중에 RESTART가 되었음을 전송한다. for SPIL
        public void Set_MachineReStart(int nStats, bool bForce = false)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            // SPIL社 전용, HOST에게 RCMD를 요청하기 위해 설비 시작전에 999012 (RESTART)를 미리 전송한다.
            if (!bForce)        // RCMD START 지원하지 않는다면 이전 루틴 수행
            {
                if (m_iPS == nStats) { return; }
                m_iPS = nStats;
            }

            //Log_wt(string.Format("Set_MachineReStart({0}), LotOpenCount:{1}", nStats, m_nLotOpenCount));

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Remote))
            {
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), State : {2}, LotOpenCount : {3}, On-line Mode : {4}",
                                                                        Convert.ToInt32(JSCK.eCEID.Machine_ReStart),
                                                                        JSCK.eCEID.Machine_ReStart.ToString(),
                                                                        nStats, m_nLotOpenCount, JSCK.eCont.Remote.ToString()));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID)     , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            }
            else
            {
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), State : {2}, LotOpenCount : {3}, On-line Mode : {4}",
                                                                        Convert.ToInt32(JSCK.eCEID.Machine_ReStart),
                                                                        JSCK.eCEID.Machine_ReStart.ToString(),
                                                                        nStats, m_nLotOpenCount, JSCK.eCont.Local.ToString()));

                if (CData.IsMultiLOT())
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.LotMgr.LoadingLotName);
                else
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.LotInfo.sLotName);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , string.Empty);
            }

            Send_CEID(Convert.ToInt32(JSCK.eCEID.Machine_ReStart).ToString());      // 999012
        }

        /// <summary>
        /// Alarm Occur
        /// </summary>
        /// <param name="iALID"></param>
        /// 

        public void Set_AlarmOccur(int iALID)
        {
		    if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    Set_AlarmOccur(iALID);

                });

            }
            else
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                // 2021.08.27 SungTae Start : [수정]
                //CLog.mLogSeq(string.Format("Set_AlarmOccur({0})", iALID));
                //Log_wt(string.Format("Set_AlarmOccur({0})", iALID));

                string sMsg = string.Format("[SEND](H<-E) S5F1 Set_AlarmOccur({0})", iALID);
                CLog.mLogSeq(sMsg);
                Log_wt(sMsg);
                // 2021.08.27 SungTae End

                if (nError_set_Flag == 1) return;
                nError_set_Flag = 1;

                m_iAO = iALID;
                Send_AlarmOccur(m_iAO.ToString());
            }
        }

        /// <summary>
        /// Alarm Reset
        /// </summary>
        /// <param name="iALID"></param>
        public void Set_AlarmReset(int iALID)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    Set_AlarmReset(iALID);
                });
            }
            else 
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                if (nError_set_Flag == 0) return;
                nError_set_Flag = 0;

                // 2021.08.27 SungTae Start : [수정]
                //Log_wt(string.Format("Set_AlarmReset({0})", iALID));
                Log_wt(string.Format("[SEND](H<-E) S5F1 Set_AlarmReset({0})", iALID));
                // 2021.08.27 SungTae End

                m_iAR = iALID;
                Send_AlarmReset(m_iAR.ToString());

                if (m_iAO == m_iAR)
                {
                    m_iAO = 0;
                    m_iAR = 0;
                }
            }
        }

        #region Recipe Event CEID
        /// SVID - 999021(Current Recipe Folder List),999022(신규 Recipe)
        /// CEID : S6F11 - 999021(Create),999022(Load),999023(Change), 999024(Delete)

        /// <summary>
        /// CEID 999022    Recipe Loaded
        /// </summary>
        /// <param name="sPPID"></param>
        public void Set_PPIDC(string sPPID)
        {
            if(!CData.Opt.bSecsUse) return; //191031 ksg :
            if (m_sPPID == sPPID){ return; }// 기존 Recipe 와 동일 하면 Return

            // 2021.09.01 SungTae Start : [수정]
            //Log_wt(string.Format("Set_PPIDC({0})", sPPID));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}). Load Device Name : {2}", Convert.ToInt32(JSCK.eCEID.Recipe_Loaded), JSCK.eCEID.Recipe_Loaded.ToString(), sPPID));
            // 2021.09.01 SungTae End

            CData.FrmMain.m_vwRcp.LoadDeviceSecs(sPPID);
            m_sPPID = SetRecipeName(sPPID);

            

            // 2021.05.21 SungTae Start : [추가] Device Parameter 변경 시 새로 추가한 SVID 설정
            if (CData.CurCompany == ECompany.SPIL)
            {
                CData.GemForm.OnSetAddSVID_Device();
            }
            // 2021.05.21 SungTae End

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_PPID), m_sPPID);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID), m_sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Loaded).ToString());
            }
            else
            { // 2021.05.21 SungTae End
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Current_PPID), m_sPPID);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Changed_PPID), m_sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Loaded).ToString());
            }
            

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
                Local_OnS2F41_Set(GEM.sRCMD_PP_SELECT, m_sPPID);
            }
        }
        /// 
        /// <summary>
        /// CEID 999021    Recipe Created
        /// </summary>
        /// <param name="sPPID"></param>
        public void Set_PPIDA(string sPPID)
        {
            if(!CData.Opt.bSecsUse) return; //191125 ksg :

            // 2021.09.01 SungTae Start : [수정]
            //Log_wt(string.Format("Set_PPIDA({0})", sPPID));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Create Device Name : {2}", Convert.ToInt32(JSCK.eCEID.Recipe_Created), JSCK.eCEID.Recipe_Created.ToString(), sPPID));
            // 2021.09.01 SungTae End

            Get_DeviceFileList();
            sPPID = SetRecipeName(sPPID);
            // 2021.05.21 SungTae Start : [추가] Device Parameter 변경 시 새로 추가한 SVID 설정
            if (CData.CurCompany == ECompany.SPIL)
            {
                CData.GemForm.OnSetAddSVID_Device();
            }
            // 2021.05.21 SungTae End
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID), sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Created).ToString());
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Changed_PPID), sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Created).ToString());
            }
            

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
                Local_OnS2F41_Set(GEM.sRCMD_PP_SELECT, sPPID);
            }
        }

        /// <summary>
        /// CEID 999024    Recipe Deleted
        /// </summary>
        /// <param name="sPPID"></param>
        public void Set_PPIDD(string sPPID)
        {
            //if(CData.SecsUse == eSecsGem.NotUse) return; //191031 ksg :
            if(!CData.Opt.bSecsUse) return; //191031 ksg :

            // 2021.09.01 SungTae Start : [수정]
            //Log_wt(string.Format("Set_PPIDD({0})", sPPID));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Delete Device Name : {2}", Convert.ToInt32(JSCK.eCEID.Recipe_Deleted), JSCK.eCEID.Recipe_Deleted.ToString(), sPPID));
            // 2021.09.01 SungTae End

            Get_DeviceFileList();
            sPPID = SetRecipeName(sPPID);

            // 2021.05.21 SungTae Start : [추가] Device Parameter 변경 시 새로 추가한 SVID 설정
            if (CData.CurCompany == ECompany.SPIL)
            {
                CData.GemForm.OnSetAddSVID_Device();
            }
            // 2021.05.21 SungTae End

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID), sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Deleted).ToString());
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Changed_PPID), sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Deleted).ToString());
            }
            

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
                Local_OnS2F41_Set(GEM.sRCMD_PP_SELECT, sPPID);
            }
        }

        /// <summary>
        /// CEID 999023    Recipe Parameter Changed
        /// </summary>
        /// <param name="sPPID"></param>
        public void Set_PPIDE(string sPPID)
        {
            if(!CData.Opt.bSecsUse) return; //191031 ksg :

            // 2021.09.01 SungTae Start : [수정]
            //Log_wt(string.Format("Set_PPIDE({0})", sPPID));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Parameter Change Device Name : {2}", Convert.ToInt32(JSCK.eCEID.Recipe_Parameter_Changed), JSCK.eCEID.Recipe_Parameter_Changed.ToString(), sPPID));
            // 2021.09.01 SungTae End

            Get_DeviceFileList();
            sPPID = SetRecipeName(sPPID);

            // 2021.05.21 SungTae Start : [추가] Device Parameter 변경 시 새로 추가한 SVID 설정
            if (CData.CurCompany == ECompany.SPIL)
            {
                CData.GemForm.OnSetAddSVID_Device();
            }
            // 2021.05.21 SungTae End

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID), sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Parameter_Changed).ToString());
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Changed_PPID), sPPID);
                Send_CEID(Convert.ToInt32(JSCK.eBCEID.Recipe_Parameters_modified).ToString());
            }
            

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
                Local_OnS2F41_Set(GEM.sRCMD_PP_SELECT, sPPID);
            }
        }
        /// <summary>
        /// 파일경로 & 확장자명 제거
        /// </summary>
        public string SetRecipeName(string sPath)
        {
            string sPPID = string.Empty;

            string t = @"\";
            string filename = sPath.Replace(".dev", "");
            string[] path = filename.Split(new string[] { t }, StringSplitOptions.None);

            if (UseRecipeFolder)
            {
                sPPID = path[path.Length - 1];
            }
            else
            {
                sPPID = Path.Combine(path[path.Length - 2], path[path.Length - 1]);
            }

            return sPPID;
        }
        // 2021.05.21 SungTae Start : [추가] SPIL용 Device 관련 Event 발생 시 추가한 SVID List Up
        public void OnSetAddSVID_ActualCycleDepth(EWay eWay, bool bUseStep = false, int nStepNum = 0)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            Log_wt(string.Format("OnSetAddSVID_ActualCycleDepth() - Left & Right Actual Cycle Depth."));

            if (bUseStep)
            {
                if (eWay == EWay.L) Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Cycle_Depth ), CData.Dev.aData[(int)EWay.L].aSteps[nStepNum].dCycleDep.ToString());
                else                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Cycle_Depth), CData.Dev.aData[(int)EWay.R].aSteps[nStepNum].dCycleDep.ToString());
            }
            else
            {
                if (eWay == EWay.L) Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Cycle_Depth ), "0.0");
                else                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Cycle_Depth), "0.0");
            }
        }
        public void OnSetAddSVID_ActualSpeed()
        {
            if (!CData.Opt.bSecsUse) return;

            Log_wt(string.Format("OnSetAddSVID_ActualSpeed() - Left & Right Actual Spindle & Table Speed."));

            //Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Spindle_Speed)  , CSpl.It.Get_Frpm(EWay.L).ToString());
            // 2023.03.15 Max
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Spindle_Speed), CSpl_485.It.GetFrpm(EWay.L).ToString());

            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Actual_Table_Speed)    , CMot.It.Get_Vel((int)EAx.LeftGrindZone_Y).ToString());

            //Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Spindle_Speed) , CSpl.It.Get_Frpm(EWay.R).ToString());
            // 2023.03.15 Max
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Spindle_Speed), CSpl_485.It.GetFrpm(EWay.R).ToString());

            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Actual_Table_Speed)   , CMot.It.Get_Vel((int)EAx.RightGrindZone_Y).ToString());
        }


        public void OnSetAddSVID_Device()
        {
            if (!CData.Opt.bSecsUse) return;

            Log_wt(string.Format("OnSetAddSVID_Device()"));

            Set_SVID(Convert.ToInt32(JSCK.eSVID.Strip_Short_Side)                       , CData.Dev.dShSide.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Strip_Long_Side)                        , CData.Dev.dLnSide.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Dual_Mode)                              , CData.Dev.bDual.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Total_Thickness)                   , CData.Dev.aData[(int)EWay.L].dTotalTh.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_PCB_Thickness)                     , CData.Dev.aData[(int)EWay.L].dPcbTh.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Mold_Thickness)                    , CData.Dev.aData[(int)EWay.L].dMoldTh.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Total_Thickness)                  , CData.Dev.aData[(int)EWay.R].dTotalTh.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_PCB_Thickness)                    , CData.Dev.aData[(int)EWay.R].dPcbTh.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Mold_Thickness)                   , CData.Dev.aData[(int)EWay.R].dMoldTh.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Grinding_Mode)                     , CData.Dev.aData[(int)EWay.L].eGrdMod.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Air_Cut_Depth)                     , CData.Dev.aData[(int)EWay.L].dAir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[0].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[0].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[0].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[0].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R1_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[0].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[1].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[1].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[1].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[1].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R2_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[1].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[2].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[2].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[2].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[2].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R3_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[2].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[3].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[3].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[3].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[3].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R4_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[3].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[4].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[4].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[4].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[4].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R5_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[4].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[5].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[5].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[5].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[5].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R6_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[5].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[6].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[6].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[6].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[6].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R7_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[6].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[7].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[7].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[7].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[7].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R8_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[7].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Target)                         , CData.Dev.aData[(int)EWay.L].aSteps[8].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Cycle_Depth)                    , CData.Dev.aData[(int)EWay.L].aSteps[8].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Table_Speed)                    , CData.Dev.aData[(int)EWay.L].aSteps[8].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Spindle_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[8].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R9_Direction)                      , CData.Dev.aData[(int)EWay.L].aSteps[8].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Target)                        , CData.Dev.aData[(int)EWay.L].aSteps[9].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Cycle_Depth)                   , CData.Dev.aData[(int)EWay.L].aSteps[9].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Table_Speed)                   , CData.Dev.aData[(int)EWay.L].aSteps[9].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Spindle_Speed)                 , CData.Dev.aData[(int)EWay.L].aSteps[9].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R10_Direction)                     , CData.Dev.aData[(int)EWay.L].aSteps[9].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Target)                        , CData.Dev.aData[(int)EWay.L].aSteps[10].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Cycle_Depth)                   , CData.Dev.aData[(int)EWay.L].aSteps[10].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Table_Speed)                   , CData.Dev.aData[(int)EWay.L].aSteps[10].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Spindle_Speed)                 , CData.Dev.aData[(int)EWay.L].aSteps[10].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R11_Direction)                     , CData.Dev.aData[(int)EWay.L].aSteps[10].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Target)                        , CData.Dev.aData[(int)EWay.L].aSteps[11].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Cycle_Depth)                   , CData.Dev.aData[(int)EWay.L].aSteps[11].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Table_Speed)                   , CData.Dev.aData[(int)EWay.L].aSteps[11].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Spindle_Speed)                 , CData.Dev.aData[(int)EWay.L].aSteps[11].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_R12_Direction)                     , CData.Dev.aData[(int)EWay.L].aSteps[11].eDir.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Target)                       , CData.Dev.aData[(int)EWay.L].aSteps[12].dTotalDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Cycle_Depth)                  , CData.Dev.aData[(int)EWay.L].aSteps[12].dCycleDep.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Table_Speed)                  , CData.Dev.aData[(int)EWay.L].aSteps[12].dTblSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Spindle_Speed)                , CData.Dev.aData[(int)EWay.L].aSteps[12].iSplSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Fine_Direction)                    , CData.Dev.aData[(int)EWay.L].aSteps[12].eDir.ToString());

            if (CData.Dev.bDual == eDual.Normal)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Grinding_Mode)                , CData.Dev.aData[(int)EWay.L].eGrdMod.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Cut_Depth)                , CData.Dev.aData[(int)EWay.L].dAir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[0].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[0].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[0].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[0].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[0].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[1].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[1].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[1].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[1].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[1].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[2].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[2].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[2].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[2].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[2].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[3].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[3].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[3].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[3].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[3].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[4].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[4].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[4].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[4].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[4].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[5].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[5].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[5].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[5].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[5].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[6].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[6].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[6].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[6].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[6].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[7].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[7].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[7].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[7].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[7].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Target)                    , CData.Dev.aData[(int)EWay.L].aSteps[8].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Cycle_Depth)               , CData.Dev.aData[(int)EWay.L].aSteps[8].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Table_Speed)               , CData.Dev.aData[(int)EWay.L].aSteps[8].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Spindle_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[8].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Direction)                 , CData.Dev.aData[(int)EWay.L].aSteps[8].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Target)                   , CData.Dev.aData[(int)EWay.L].aSteps[9].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Cycle_Depth)              , CData.Dev.aData[(int)EWay.L].aSteps[9].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Table_Speed)              , CData.Dev.aData[(int)EWay.L].aSteps[9].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Spindle_Speed)            , CData.Dev.aData[(int)EWay.L].aSteps[9].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Direction)                , CData.Dev.aData[(int)EWay.L].aSteps[9].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Target)                   , CData.Dev.aData[(int)EWay.L].aSteps[10].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Cycle_Depth)              , CData.Dev.aData[(int)EWay.L].aSteps[10].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Table_Speed)              , CData.Dev.aData[(int)EWay.L].aSteps[10].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Spindle_Speed)            , CData.Dev.aData[(int)EWay.L].aSteps[10].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Direction)                , CData.Dev.aData[(int)EWay.L].aSteps[10].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Target)                   , CData.Dev.aData[(int)EWay.L].aSteps[11].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Cycle_Depth)              , CData.Dev.aData[(int)EWay.L].aSteps[11].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Table_Speed)              , CData.Dev.aData[(int)EWay.L].aSteps[11].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Spindle_Speed)            , CData.Dev.aData[(int)EWay.L].aSteps[11].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Direction)                , CData.Dev.aData[(int)EWay.L].aSteps[11].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Target)                  , CData.Dev.aData[(int)EWay.L].aSteps[12].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Cycle_Depth)             , CData.Dev.aData[(int)EWay.L].aSteps[12].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Table_Speed)             , CData.Dev.aData[(int)EWay.L].aSteps[12].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Spindle_Speed)           , CData.Dev.aData[(int)EWay.L].aSteps[12].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Direction)               , CData.Dev.aData[(int)EWay.L].aSteps[12].eDir.ToString());
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Grinding_Mode)                , CData.Dev.aData[(int)EWay.R].eGrdMod.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Cut_Depth)                , CData.Dev.aData[(int)EWay.R].dAir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[0].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[0].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[0].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[0].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R1_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[0].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[1].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[1].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[1].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[1].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R2_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[1].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[2].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[2].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[2].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[2].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R3_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[2].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[3].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[3].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[3].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[3].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R4_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[3].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[4].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[4].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[4].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[4].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R5_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[4].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[5].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[5].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[5].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[5].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R6_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[5].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[6].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[6].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[6].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[6].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R7_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[6].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[7].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[7].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[7].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[7].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R8_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[7].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Target)                    , CData.Dev.aData[(int)EWay.R].aSteps[8].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Cycle_Depth)               , CData.Dev.aData[(int)EWay.R].aSteps[8].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Table_Speed)               , CData.Dev.aData[(int)EWay.R].aSteps[8].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Spindle_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[8].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R9_Direction)                 , CData.Dev.aData[(int)EWay.R].aSteps[8].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Target)                   , CData.Dev.aData[(int)EWay.R].aSteps[9].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Cycle_Depth)              , CData.Dev.aData[(int)EWay.R].aSteps[9].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Table_Speed)              , CData.Dev.aData[(int)EWay.R].aSteps[9].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Spindle_Speed)            , CData.Dev.aData[(int)EWay.R].aSteps[9].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R10_Direction)                , CData.Dev.aData[(int)EWay.R].aSteps[9].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Target)                   , CData.Dev.aData[(int)EWay.R].aSteps[10].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Cycle_Depth)              , CData.Dev.aData[(int)EWay.R].aSteps[10].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Table_Speed)              , CData.Dev.aData[(int)EWay.R].aSteps[10].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Spindle_Speed)            , CData.Dev.aData[(int)EWay.R].aSteps[10].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R11_Direction)                , CData.Dev.aData[(int)EWay.R].aSteps[10].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Target)                   , CData.Dev.aData[(int)EWay.R].aSteps[11].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Cycle_Depth)              , CData.Dev.aData[(int)EWay.R].aSteps[11].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Table_Speed)              , CData.Dev.aData[(int)EWay.R].aSteps[11].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Spindle_Speed)            , CData.Dev.aData[(int)EWay.R].aSteps[11].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_R12_Direction)                , CData.Dev.aData[(int)EWay.R].aSteps[11].eDir.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Target)                  , CData.Dev.aData[(int)EWay.R].aSteps[12].dTotalDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Cycle_Depth)             , CData.Dev.aData[(int)EWay.R].aSteps[12].dCycleDep.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Table_Speed)             , CData.Dev.aData[(int)EWay.R].aSteps[12].dTblSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Spindle_Speed)           , CData.Dev.aData[(int)EWay.R].aSteps[12].iSplSpd.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Fine_Direction)               , CData.Dev.aData[(int)EWay.R].aSteps[12].eDir.ToString());
            }

            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Point_Column_Count)        , CData.Dev.iCol.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Point_Row_Count)           , CData.Dev.iRow.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_Width)                , CData.Dev.dChipW.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_Height)               , CData.Dev.dChipH.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_1_Center_Y)           , CData.Dev.aWinSt[0].Y.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_2_Center_Y)           , CData.Dev.aWinSt[1].Y.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_3_Center_Y)           , CData.Dev.aWinSt[2].Y.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Unit_4_Center_Y)           , CData.Dev.aWinSt[3].Y.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_Before_Limit)              , CData.Dev.aData[(int)EWay.L].dBfLimit.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_After_Limit)               , CData.Dev.aData[(int)EWay.L].dAfLimit.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_TTV_Limit)                 , CData.Dev.aData[(int)EWay.L].dTTV.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_One_Point_Limit)           , CData.Dev.aData[(int)EWay.L].dOneLimit.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Contact_One_Point_Over_Check)      , CData.Dev.aData[(int)EWay.L].dOneOver.ToString());

            if (CData.Dev.bDual == eDual.Normal)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Point_Column_Count)   , CData.Dev.iCol.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Point_Row_Count)      , CData.Dev.iRow.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_Width)           , CData.Dev.dChipW.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_Height)          , CData.Dev.dChipH.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_1_Center_Y)      , CData.Dev.aWinSt[0].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_2_Center_Y)      , CData.Dev.aWinSt[1].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_3_Center_Y)      , CData.Dev.aWinSt[2].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_4_Center_Y)      , CData.Dev.aWinSt[3].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Before_Limit)         , CData.Dev.aData[(int)EWay.L].dBfLimit.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_After_Limit)          , CData.Dev.aData[(int)EWay.L].dAfLimit.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_TTV_Limit)            , CData.Dev.aData[(int)EWay.L].dTTV.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_One_Point_Limit)      , CData.Dev.aData[(int)EWay.L].dOneLimit.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_One_Point_Over_Check) , CData.Dev.aData[(int)EWay.L].dOneOver.ToString());
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Point_Column_Count)   , CData.Dev.iCol.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Point_Row_Count)      , CData.Dev.iRow.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_Width)           , CData.Dev.dChipW.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_Height)          , CData.Dev.dChipH.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_1_Center_Y)      , CData.Dev.aWinSt[0].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_2_Center_Y)      , CData.Dev.aWinSt[1].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_3_Center_Y)      , CData.Dev.aWinSt[2].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Unit_4_Center_Y)      , CData.Dev.aWinSt[3].Y.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_Before_Limit)         , CData.Dev.aData[(int)EWay.R].dBfLimit.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_After_Limit)          , CData.Dev.aData[(int)EWay.R].dAfLimit.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_TTV_Limit)            , CData.Dev.aData[(int)EWay.R].dTTV.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_One_Point_Limit)      , CData.Dev.aData[(int)EWay.R].dOneLimit.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Contact_One_Point_Over_Check) , CData.Dev.aData[(int)EWay.R].dOneOver.ToString());
            }

            Set_SVID(Convert.ToInt32(JSCK.eSVID.BCR_Skip)                               , CData.Dev.bBcrSkip.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.BCR_Key_In_Skip)                        , CData.Dev.bBcrKeyInSkip.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Orientation_Skip)                       , CData.Dev.bOriSkip.ToString());

            Set_SVID(Convert.ToInt32(JSCK.eSVID.Dry_Knife_Count)                        , CData.Dev.iDryCnt.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Dry_Pusher_Fast_Velocity)               , CData.Dev.dPushF.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Dry_Pusher_Slow_Velocity)               , CData.Dev.dPushS.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Dry_Bottom_Cleaning_Picker_Count)       , CData.Dev.iOffpClean.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Dry_Bottom_Cleaning_Strip_Count)        , CData.Dev.iOffpCleanStrip.ToString());
            
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Water_Knife_Clean_Velocity)        , CData.Dev.aData[(int)EWay.L].dTpBubSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Water_Knife_Clean_Count)           , CData.Dev.aData[(int)EWay.L].iTpBubCnt.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Air_Knife_Clean_Velocity)          , CData.Dev.aData[(int)EWay.L].dTpAirSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Air_Knife_Clean_Count)             , CData.Dev.aData[(int)EWay.L].iTpAirCnt.ToString());

            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Water_Knife_Clean_Velocity)       , CData.Dev.aData[(int)EWay.R].dTpBubSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Water_Knife_Clean_Count)          , CData.Dev.aData[(int)EWay.R].iTpBubCnt.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Knife_Clean_Velocity)         , CData.Dev.aData[(int)EWay.R].dTpAirSpd.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Air_Knife_Clean_Count)            , CData.Dev.aData[(int)EWay.R].iTpAirCnt.ToString());
        }
        // 2021.05.21 SungTae End
        #endregion

        #region Carrier Verify Event 1
        /// <summary>
        /// CEID 999200    Carrier Verify Request
        /// </summary>
        /// <param name="sCarrierId"></param>
        public void OnCarrierVerifyRequset(string sCarrierId)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnCarrierVerifyRequset(sCarrierId);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnCarrierVerifyRequset(sCarrierId); }
                else { StdB_OnCarrierVerifyRequset(sCarrierId); }
            }
        }
        public void StdA_OnCarrierVerifyRequset(string sCarrierId)
        {
            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnCarrierVerifyRequset(sCarrierId);
            //    });
            //}
            //else
            {
                // 2021.08.05 SungTae Start : [수정] SVID=999212(PCB_TOPBTM_USE_MODE) 바뀌는 것과 관련하여 임희성 책임님이 수정한 것을 함수로 변경.
                string sSideIdx = GetMoldSideInfo(CData.Dev.eMoldSide);     // "0";     // "0" = Top, "1" = Btm, "2" = TopD, "3" = BtmS

                string sDF_P_Min = "";
                string sDF_P_Max = "";
                string sDF_P_Avg = "";

                if (CData.Dev.bDual == eDual.Normal)    { Set_SVID(Convert.ToInt32(JSCK.eSVID.Grind__Mode), "1"); } // 0, 1이 반대로 되어 있군 !!!
                else                                    { Set_SVID(Convert.ToInt32(JSCK.eSVID.Grind__Mode), "0"); }

                // 2021.07.31 lhs Start : 새로운 Grind Process 추가 (SCK 전용)
                if (CDataOption.UseNewSckGrindProc)
                {
                    if (CData.Dev.eMoldSide == ESide.Top || CData.Dev.eMoldSide == ESide.BtmS) // Top or BtmS Side 일때 DF 측정값 입력 (Pcb 두께)
                    {
						sDF_P_Min = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0].ToString();
						sDF_P_Max = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0].ToString();
						sDF_P_Avg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0].ToString();
					}
                }
                else  // 기존 방식
                // 2021.07.31 lhs End
                {
                    if (CData.Dev.eMoldSide == ESide.Top) // Top Side 일때 DF 측정값 입력
                    {
                        sDF_P_Min = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0].ToString();
                        sDF_P_Max = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0].ToString();
                        sDF_P_Avg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0].ToString();
                    }
				}

				Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX)    , sDF_P_Max);
				Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR)    , sDF_P_Avg);
				Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
				Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE)   , sSideIdx);

                // 2021.08.05 SungTae : [수정] SECS/GEM 사용 시에도 Multi-LOT 기능 사용 추가
                //Log_wt(string.Format("OnCarrierVerifyRequset({0})", sCarrierId));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Carrier ID : {2}", Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request), JSCK.eCEID.Carrier_Verify_Request, sCarrierId));
                // 2021.08.05 SungTae End

                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID = sCarrierId;
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/;            // 시퀸스 초기화

                Log_wt(string.Format("CData.SecsGem_Data.nStrip_Check = 1;// 시퀸스 초기화"));

                if (!CData.Opt.bSecsUse)
                {
                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;      // 시퀸스 계속 진행

                    //Log_wt(string.Format("CData.SecsGem_Data.nStrip_Check = 3;// 시퀸스 계속 진행"));
                    Log_wt($"SECS/GEM not use mode. Strip Check State : {CData.SecsGem_Data.nStrip_Check}");
                    return;
                }

                string sLotId   = string.Empty;
                m_sCCarID       = sCarrierId;

                int nIdx = -1;

                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                    if (nIdx < 0)
                    {
                        //첫 carrier 들어온 경우
                        m_sReadLOT_T = sCarrierId;
                        m_pReadCarrierList.ClearCarrier();
                        m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                        // 2022.04.14 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 조건 추가
                        //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                        if (CData.CurCompany != ECompany.ASE_K12)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                        }
                        // 2022.04.14 Sung End

                        // 읽은 리스트에 다시 저장.
                        Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                        sMsgdata = string.Format("[Multi-LOT]  첫 Carrier 들어온 경우");
                    }
                    else
                    {
                        if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                        {
                            if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.IsExsitCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID) == true)
                            {
                                // 읽은 Carrier가 Host에서 보내 준 리스트에 존재하기 때문에 상태값 변경 
                                // 및 Read Carrier값을 보고 후 Carrier Verification 대기.
                                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Read));
                                m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                                sMsgdata = string.Format("[Multi-LOT]  LOT List에 등록된 Carrier ID : {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                            }
                            else
                            {   //다른 LOT인 경우, Host Carrier List에 존재하지 않음.
                                //장비에서 error 처리
                                //manual lot end 보고 후, 새로 시작해야 함.

                                // 2022.04.14 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 조건 추가
                                //if (CData.CurCompany == ECompany.ASE_KR)
                                if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.ASE_K12)
                                {
                                    m_sReadLOT_T = sCarrierId;
                                    m_pReadCarrierList.ClearCarrier();
                                    m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                                    //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                                    if (CData.CurCompany != ECompany.ASE_K12)
                                    {
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                                    }

                                    // 읽은 리스트에 다시 저장.
                                    Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
                                }

                                //if (CData.CurCompany == ECompany.ASE_KR)
                                if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.ASE_K12)
                                    sMsgdata = string.Format("[Multi-LOT]  다른 LOT의 첫 Carrier 들어온 경우 기존 List 초기화 후 Carrier 정보 추가");
                                else
                                    sMsgdata = string.Format("[Multi-LOT]  다른 LOT인 경우, Host Carrier List에 존재하지 않음.");
                                // 2022.04.14 Sung End
                            }
                        }
                        else
                        {
                            //첫 carrier 들어온 경우
                            m_sReadLOT_T = sCarrierId;
                            m_pReadCarrierList.ClearCarrier();
                            m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                            // 읽은 리스트에 다시 저장.
                            Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                            sMsgdata = string.Format("[Multi-LOT MODE] 첫 Carrier 들어온 경우");
                        }
                    }

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID)             , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Current_Read_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request).ToString());

                    Log_wt(sMsgdata);

                    // 2021.03.04 SungTae : Lot 수량이 0으로 Grid를 그릴 때 Hang-up 발생하여 조건 추가(LOCAL MODE일 경우)
                    if (nIdx >= 0)
                    {
                        //for(int i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                        //{
                        //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                        //}
                        if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                        {
                            try
                            {
                                sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                else        // Multi-LOT 미사용 시
                {
                    if (CData.m_pHostLOT.Count > 0)
                    {
                        if (CData.m_pHostLOT.IsExsitCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID) == true)
                        {
                            // 읽은 Carrier가 Host에서 보내 준 리스트에 존재하기 때문에 상태값 변경 
                            // 및 Read Carrier값을 보고 후 Carrier Verification 대기.
                            CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Read));
                            m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                            Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                            sMsgdata = string.Format("LOT List에 등록된 carrier id {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                        }
                        else
                        {
                            //다른 LOT인 경우, Host Carrier List에 존재하지 않음.
                            //장비에서 error 처리
                            //manual lot end 보고 후, 새로 시작해야 함.

                            // 2021.09.02 SungTae Start : [추가] Multi-LOT 처리 위해 추가
                            if (CData.CurCompany == ECompany.ASE_KR)
                            {
                                m_sReadLOT_T = sCarrierId;
                                m_pReadCarrierList.ClearCarrier();
                                m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";

                                // 읽은 리스트에 다시 저장.
                                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
                                
                                sMsgdata = string.Format("[Multi-LOT]  다른 LOT의 첫 Carrier 들어온 경우 기존 List 초기화 후 Carrier 정보 추가");
                            }
                            else
                                sMsgdata = string.Format("[Multi-LOT]  다른 LOT인 경우, Host Carrier List에 존재하지 않음.");
                            // 2021.09.02 SungTae End
                        }
                    }
                    else
                    {//첫 carrier 들어온 경우

                        // 2021-07-01, SPIL-QLE VOC : LOT Verify request 전송 취소요청 
                        //
                        //// 2021-06-22, jhLee, SPIL VOC, LOT의 첫번째 Strip의 경우 LOT Verify request를 먼저 전송한다.
                        //if (CData.CurCompany == ECompany.SPIL)
                        //{
                        //    // In-Rail에 배정된 LOT ID를 대입시키고 LOT Verify Request를 전송한다.
                        //    // PPID는 On-Line연결시에 지정해준다.
                        //    Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID), CData.JSCK_Gem_Data[4].sLot_Name);

                        //    // .LOT_ID 는 이전에 처리된 LOT ID이다. LOT End 보고시 Current LOT ID가 Reset되면서 값을 이동시킨다.
                        //    // Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.JSCK_Gem_Data[4].sLot_Name);
                        //    Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.sLastLot);

                        //    Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Verify_Request).ToString());
                        //    Log_wt($"(SPIL) LOT Verify Request Current_LOT_ID:{CData.JSCK_Gem_Data[4].sLot_Name}, Before LOT_ID:{CData.sLastLot}");
                        //}

                        m_sReadLOT_T = sCarrierId;
                        m_pReadCarrierList.ClearCarrier();
                        m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                       
                        // 읽은 리스트에 다시 저장.
                        Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
                        
                        sMsgdata = string.Format("첫 carrier 들어온 경우");
                    }

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID)             , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Current_Read_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request).ToString());
                    
                    Log_wt(sMsgdata);

                    // 2021.03.04 SungTae : Lot 수량이 0으로 Grid를 그릴 때 Hang-up 발생하여 조건 추가(LOCAL MODE일 경우)
                    if (CData.m_pHostLOT.Count > 0)
                    {
                        try
                        {
                            sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행

                //Local_OnS2F41_Set(GEM.sRCMD_CARRIER_VERIFICATION, CData.m_pHostLOT.Count.ToString());
                Local_OnS2F41_Set(GEM.sRCMD_CARRIER_VERIFICATION, CData.LotInfo.iTotalStrip.ToString());
            }
        }
        public void StdB_OnCarrierVerifyRequset(string sCarrierId) 
        {
            {
                // 2021.08.05 SungTae Start : [수정] SVID=999212(PCB_TOPBTM_USE_MODE) 바뀌는 것과 관련하여 임희성 책임님이 수정한 것을 함수로 변경.
                string sSideIdx = GetMoldSideInfo(CData.Dev.eMoldSide);     // "0";     // "0" = Top, "1" = Btm, "2" = TopD, "3" = BtmS

                string sDF_P_Min = "";
                string sDF_P_Max = "";
                string sDF_P_Avg = "";

                if (CData.Dev.bDual == eDual.Normal) { Set_SVID(Convert.ToInt32(JSCK.eBSVID.Grind_Dual_Mode), "1"); } // 0, 1이 반대로 되어 있군 !!!
                else { Set_SVID(Convert.ToInt32(JSCK.eBSVID.Grind_Dual_Mode), "0"); }

                // 2021.07.31 lhs Start : 새로운 Grind Process 추가 (SCK 전용)
                if (CDataOption.UseNewSckGrindProc)
                {
                    if (CData.Dev.eMoldSide == ESide.Top || CData.Dev.eMoldSide == ESide.BtmS) // Top or BtmS Side 일때 DF 측정값 입력 (Pcb 두께)
                    {
                        sDF_P_Min = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[(int)EPoint.DF].ToString();
                        sDF_P_Max = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[(int)EPoint.DF].ToString();
                        sDF_P_Avg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[(int)EPoint.DF].ToString();
                    }
                }
                else  // 기존 방식
                // 2021.07.31 lhs End
                {
                    if (CData.Dev.eMoldSide == ESide.Top) // Top Side 일때 DF 측정값 입력
                    {
                        sDF_P_Min = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[(int)EPoint.DF].ToString();
                        sDF_P_Max = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[(int)EPoint.DF].ToString();
                        sDF_P_Avg = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[(int)EPoint.DF].ToString();
                    }
                }

                Set_SVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_MAX), sDF_P_Max);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_AVR), sDF_P_Avg);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
               // Set_SVID(Convert.ToInt32(JSCK.eBSVID.PCB_ORIENT_VALUE), sSideIdx);

                // 2021.08.05 SungTae : [수정] SECS/GEM 사용 시에도 Multi-LOT 기능 사용 추가
                //Log_wt(string.Format("OnCarrierVerifyRequset({0})", sCarrierId));
                Log_wt($"[SEND](H<-E) S6F11 CEID = {Convert.ToInt32(JSCK.eBCEID.Carrier_Verify_Request)}" +
                    $"({JSCK.eBCEID.Carrier_Verify_Request}).  Carrier ID : {sCarrierId}");
                // 2021.08.05 SungTae End

                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID = sCarrierId;
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/;            // 시퀸스 초기화

                Log_wt(string.Format("CData.SecsGem_Data.nStrip_Check = 1;// 시퀸스 초기화"));

                if (!CData.Opt.bSecsUse)
                {
                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;      // 시퀸스 계속 진행

                    //Log_wt(string.Format("CData.SecsGem_Data.nStrip_Check = 3;// 시퀸스 계속 진행"));
                    Log_wt($"SECS/GEM not use mode. Strip Check State : {CData.SecsGem_Data.nStrip_Check}");
                    return;
                }

                string sLotId = string.Empty;
                m_sCCarID = sCarrierId;

                int nIdx = -1;

                if (CData.IsMultiLOT())
                {
                    #region MultiLot이용
                    nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                    if (nIdx < 0)
                    {
                        //첫 carrier 들어온 경우
                        m_sReadLOT_T = sCarrierId;
                        m_pReadCarrierList.ClearCarrier();
                        m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                        // 2022.04.14 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 조건 추가
                        //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                        if (CData.CurCompany != ECompany.ASE_K12)
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                        }
                        // 2022.04.14 Sung End

                        // 읽은 리스트에 다시 저장.
                        Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));

                        sMsgdata = string.Format("[Multi-LOT]  첫 Carrier 들어온 경우");
                    }
                    else
                    {
                        if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                        {
                            if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.IsExsitCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID) == true)
                            {
                                // 읽은 Carrier가 Host에서 보내 준 리스트에 존재하기 때문에 상태값 변경 
                                // 및 Read Carrier값을 보고 후 Carrier Verification 대기.
                                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Read));
                                m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                                Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));

                                sMsgdata = string.Format("[Multi-LOT]  LOT List에 등록된 Carrier ID : {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                            }
                            else
                            {
                                if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.ASE_K12)
                                {
                                    m_sReadLOT_T = sCarrierId;
                                    m_pReadCarrierList.ClearCarrier();
                                    m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                                    //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                                    if (CData.CurCompany != ECompany.ASE_K12)
                                    {
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = "";
                                    }

                                    // 읽은 리스트에 다시 저장.
                                    Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));
                                }

                                //if (CData.CurCompany == ECompany.ASE_KR)
                                if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.ASE_K12)
                                    sMsgdata = string.Format("[Multi-LOT]  다른 LOT의 첫 Carrier 들어온 경우 기존 List 초기화 후 Carrier 정보 추가");
                                else
                                    sMsgdata = string.Format("[Multi-LOT]  다른 LOT인 경우, Host Carrier List에 존재하지 않음.");
                                // 2022.04.14 Sung End
                            }
                        }
                        else
                        {
                            //첫 carrier 들어온 경우
                            m_sReadLOT_T = sCarrierId;
                            m_pReadCarrierList.ClearCarrier();
                            m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ].sInRail_Strip_ID);

                            // 읽은 리스트에 다시 저장.
                            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));

                            sMsgdata = string.Format("[Multi-LOT MODE] 첫 Carrier 들어온 경우");
                        }
                    }

                    Set_SVID(Convert.ToInt32(JSCK.eBSVID.Load_CarrierID_1), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                    Set_SVID(Convert.ToInt32(JSCK.eBSVID.Reading_Carrier_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                    Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Verify_Request).ToString());

                    Log_wt(sMsgdata);

                    // 2021.03.04 SungTae : Lot 수량이 0으로 Grid를 그릴 때 Hang-up 발생하여 조건 추가(LOCAL MODE일 경우)
                    if (nIdx >= 0)
                    {
                        //for(int i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                        //{
                        //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                        //}
                        if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                        {
                            try
                            {
                                sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                            }
                            catch
                            {

                            }
                        }
                    }
                    #endregion MultiLot 이용
                }
                else        // Multi-LOT 미사용 시
                {
                    if (CData.bSecLotOpen == false)
                    {//첫 carrier 들어온 경우

                        m_sReadLOT_T = sCarrierId;
                        m_pReadCarrierList.ClearCarrier();

                        sMsgdata = string.Format("첫 carrier 들어온 경우");
                    }

                    if(m_pReadCarrierList.IsExsitCarrier(sCarrierId) == false)
                    {
                        m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                        Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Read));
                    }

                    Set_SVID(Convert.ToInt32(JSCK.eBSVID.Load_CarrierID_1), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                    Set_SVID(Convert.ToInt32(JSCK.eBSVID.Reading_Carrier_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                    Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Verify_Request).ToString());

                    Log_wt(sMsgdata);

                }
            }

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행

                //Local_OnS2F41_Set(GEM.sRCMD_CARRIER_VERIFICATION, CData.m_pHostLOT.Count.ToString());
                Local_OnS2F41_Set(GEM.sRCMD_CARRIER_VERIFICATION, CData.LotInfo.iTotalStrip.ToString());
            }
        }
        #endregion

        #region Carrier Verify Event 2
        // 2020 01 30 skpark JSCK DF정보를 보내기 위하여 Parameter 추가.
        /// <summary>
        /// CEID 999200    Carrier Verify Request
        /// </summary>
        /// <param name="sCarrierId"></param>
        public void OnCarrierVerifyRequset(string sCarrierId, 
                            string sDF_P_Min, string sDF_P_Max, string sDF_P_Avg, string sSideIdx, 
                            string sDF_T_Min, string sDF_T_Max, string sDF_T_Avg, string sDF_T_Using_Value, 
                            string sDF_B_Min, string sDF_B_Max, string sDF_B_Avg, string sDF_B_Using_Value)
        { 
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnCarrierVerifyRequset(sCarrierId, sDF_P_Min, sDF_P_Max, sDF_P_Avg, sSideIdx, sDF_T_Min, sDF_T_Max, sDF_T_Avg, sDF_T_Using_Value, sDF_B_Min, sDF_B_Max, sDF_B_Avg, sDF_B_Using_Value);

                });

            }
            else
            {
                // 2020 01 30 skpark DF값을 CarrierID 보고시 같이 보고 --------------------------------------
                Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX)    , sDF_P_Max);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR)    , sDF_P_Avg);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE)   , sSideIdx);
                //-------------------------------------------------------------------------------------

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                int nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);          // 2021.08.31 SungTae : [추가] Multi-LOT 관련

                if(CData.IsMultiLOT())
                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}),  Carrier ID : {2}, Count = {3}",  Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request),
                                                                                                                JSCK.eCEID.Carrier_Verify_Request.ToString(),
                                                                                                                sCarrierId,
                                                                                                                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count));
                else
                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}),  Carrier ID : {2}, Count = {3}", Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request),
                                                                                                                JSCK.eCEID.Carrier_Verify_Request.ToString(),
                                                                                                                sCarrierId,
                                                                                                                CData.LotInfo.m_pHostLOT.Count));
                // 2021.08.25 SungTae End

                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID = sCarrierId;
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/;// 시퀸스 초기화        
                
                Log_wt(string.Format("CData.SecsGem_Data.nStrip_Check = 1;// 시퀸스 초기화"));

                if (!CData.Opt.bSecsUse)
                {
                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행
                    
                    Log_wt(string.Format("CData.SecsGem_Data.nStrip_Check = 3;// 시퀸스 계속 진행"));
                    
                    return;
                }

                string sLotId   = string.Empty;
                m_sCCarID       = sCarrierId;
         
                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                    {
                        if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.IsExsitCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID) == true)
                        {
                            // 읽은 Carrier가 Host에서 보내 준 리스트에 존재하기 때문에 상태값 변경 
                            // 및 Read Carrier값을 보고 후 Carrier Verification 대기.
                            CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Read));
                            m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                            
                            Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                            sMsgdata = string.Format("LOT List에 등록된 Carrier ID : {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                        }
                        else
                        {
                            //다른 LOT인 경우, Host Carrier List에 존재하지 않음.
                            //장비에서 error 처리
                            //manual lot end 보고 후, 새로 시작해야 함.

                            sMsgdata = string.Format("다른 LOT인 경우, Host Carrier List에 존재하지 않음.");
                        }
                    }
                    else
                    {
                        //첫 carrier 들어온 경우
                        m_sReadLOT_T = sCarrierId;
                        m_pReadCarrierList.ClearCarrier();
                        m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                        // 읽은 리스트에 다시 저장.
                        Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                        sMsgdata = string.Format("첫 Carrier 들어온 경우");
                    }

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Current_Read_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request).ToString());
                    
                    Log_wt(sMsgdata);
                    
                    // 2021.10.19 SungTae Start : [수정] Multi-LOT 관련
                    sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    //for (int i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                    //{
                    //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                    //}
                    // 2021.10.19 SungTae End
                }
                else
                {   // Multi-LOT 미사용 시
                    if (CData.m_pHostLOT.Count > 0)
                    {
                        if (CData.m_pHostLOT.IsExsitCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID) == true)
                        {
                            // 읽은 Carrier가 Host에서 보내 준 리스트에 존재하기 때문에 상태값 변경 
                            // 및 Read Carrier값을 보고 후 Carrier Verification 대기.
                            CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Read));
                            m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                            
                            Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

                            sMsgdata = string.Format("LOT List에 등록된 Carrier ID : {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                        }
                        else
                        {
                            //다른 LOT인 경우, Host Carrier List에 존재하지 않음.
                            //장비에서 error 처리
                            //manual lot end 보고 후, 새로 시작해야 함.

                            sMsgdata = string.Format("다른 LOT인 경우, Host Carrier List에 존재하지 않음.");
                        }
                    }
                    else
                    {
                        //첫 carrier 들어온 경우
                        m_sReadLOT_T = sCarrierId;
                        m_pReadCarrierList.ClearCarrier();
                        m_pReadCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                        // 읽은 리스트에 다시 저장.
                        Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
                        
                        sMsgdata = string.Format("첫 Carrier 들어온 경우");
                    }

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID)             , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Current_Read_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verify_Request).ToString());

                    Log_wt(sMsgdata);

                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                }
                // 2021.08.25 SungTae End
            }

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Local))
            {// Local Mode 진행에 따른 처리 20200718
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행
                Log_wt(string.Format("CData.SecsGem_Data.nRemote_Flag = JSCK.eCont.Local"));
            }
        }
        #endregion


        /// <summary>
        /// CEID 999202    Carrier Started
        /// </summary>
        /// <param name="sCarrierId"></param>
        //public void OnCarrierStarted(string sCarrierId)
        public void OnCarrierStarted(string sLotId, string sCarrierId)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    //OnCarrierStarted(sCarrierId); 
                    OnCarrierStarted(sLotId, sCarrierId);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnCarrierStarted(sLotId, sCarrierId); }
                else { StdB_OnCarrierStarted(sLotId, sCarrierId); }
            }
          
        }
        public void StdA_OnCarrierStarted(string sLotId, string sCarrierId)
        {

            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        //OnCarrierStarted(sCarrierId); 
            //        OnCarrierStarted(sLotId, sCarrierId);
            //    });

            //}
            //else
            {
                if(!CData.Opt.bSecsUse) return; //191031 ksg :

                // 2021.08.27 SungTae : [수정]
                //sMsgdata = string.Format("OnCarrierStarted({0} CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID {1}", sCarrierId, CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                sMsgdata = string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  LOT : {2}, Carrier ID : {3}",
                                                                    Convert.ToInt32(JSCK.eCEID.Carrier_Started),
                                                                    JSCK.eCEID.Carrier_Started.ToString(),
                                                                    sLotId, CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                // 2021.08.27 SungTae End

                Log_wt(sMsgdata);

                //string sLotId = m_sCLOTID;
                //JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = sCarrierId;
                //JSCK_Gem_Data[4].sLot_Name = sLotId;

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                int nIdx = CData.LotMgr.GetListIndexNum(sLotId);

                m_pStartCarrierList.AddCarrier( CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID );

                if (CData.IsMultiLOT())
                {
                    CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                            Convert.ToInt32(JSCK.eCarrierStatus.Start));
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Start));
                }
                // 2021.08.25 SungTae End

                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));

                mgem.WriteUserLog(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);

                string strTime = string.Empty;
                DateTime now = DateTime.Now;
                strTime = now.ToString("yyyyMMddHHmmss");

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotMgr.LoadingLotName;
                    CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                }
                else
                {
                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotInfo.sLotName;// 강제로 설정 20200513
                    CData.m_pHostLOT.SetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                }
                // 2021.08.25 SungTae End

                if (m_pStartCarrierList.Count <= 1) // lot started send report
                {
                    m_strLotTime = strTime;

                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID), CData.JSCK_Gem_Data[4].sLot_Name);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Started).ToString());

                    m_nLotOpenCount++;      // Lot Open이 진행되어 처리중인가 ? START/RESTART 구분용, 진행중인 LOT 수량 0이면 아직 없음

                    if (m_nLotOpenCount <= 0)
                    {
                        m_nLotOpenCount = 1;        // LOT Open이 보고되었다면 최소한 1개 이상이 현재 진행중이다.
                    }

                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), LOT Start:{2}, nLotOpenCount:{3}",
                                                                    Convert.ToInt32(JSCK.eCEID.LOT_Started),
                                                                    JSCK.eCEID.LOT_Started.ToString(),
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name, m_nLotOpenCount));
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID)     , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);                
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Started).ToString());

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    // 2021.10.19 SungTae Start : [수정] Multi-LOT 관련
                    sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    //for (int i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                    //{
                    //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                    //}
                    // 2021.10.19 SungTae End
                }
                else
                {
                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                }
                // 2021.08.25 SungTae End
            }
        }
        public void StdB_OnCarrierStarted(string sLotId, string sCarrierId) 
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            // 2021.08.27 SungTae : [수정]
            //sMsgdata = string.Format("OnCarrierStarted({0} CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID {1}", sCarrierId, CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            sMsgdata = string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  LOT : {2}, Carrier ID : {3}",
                                                                Convert.ToInt32(JSCK.eBCEID.Carrier_Started),
                                                                JSCK.eBCEID.Carrier_Started.ToString(),
                                                                sLotId, CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            // 2021.08.27 SungTae End

            Log_wt(sMsgdata);

            //string sLotId = m_sCLOTID;
            //JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = sCarrierId;
            //JSCK_Gem_Data[4].sLot_Name = sLotId;

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            int nIdx = CData.LotMgr.GetListIndexNum(sLotId);

            m_pStartCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);

            if (CData.IsMultiLOT())
            {
                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                        Convert.ToInt32(JSCK.eCarrierStatus.Start));
            }
            else
            {
                CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Start));
            }
            // 2021.08.25 SungTae End

            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Started));

            mgem.WriteUserLog(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);

            string strTime = string.Empty;
            DateTime now = DateTime.Now;
            strTime = now.ToString("yyyyMMddHHmmss");

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            if (CData.IsMultiLOT())
            {
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotMgr.LoadingLotName;
                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            }
            else
            {
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotInfo.sLotName;// 강제로 설정 20200513
                CData.m_pHostLOT.SetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            }
            // 2021.08.25 SungTae End

            if (m_pStartCarrierList.Count <= 1) // lot started send report
            {
                m_strLotTime = strTime;

                //Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID), CData.JSCK_Gem_Data[4].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.LOT_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Current_Lot_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Send_CEID(Convert.ToInt32(JSCK.eBCEID.LOT_Started).ToString());

                m_nLotOpenCount++;      // Lot Open이 진행되어 처리중인가 ? START/RESTART 구분용, 진행중인 LOT 수량 0이면 아직 없음

                if (m_nLotOpenCount <= 0)
                {
                    m_nLotOpenCount = 1;        // LOT Open이 보고되었다면 최소한 1개 이상이 현재 진행중이다.
                }

                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}), LOT Start:{2}, nLotOpenCount:{3}",
                                                                Convert.ToInt32(JSCK.eBCEID.LOT_Started),
                                                                JSCK.eBCEID.LOT_Started.ToString(),
                                                                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name, m_nLotOpenCount));
            }

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.LOT_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Start_CarrierID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Start_CarrierID_1), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Started).ToString());

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            if (CData.IsMultiLOT())
            {
                // 2021.10.19 SungTae Start : [수정] Multi-LOT 관련
                sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                //for (int i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                //{
                //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                //}
                // 2021.10.19 SungTae End
            }
            else
            {
                sgfrmMsg.ShowGrid(CData.m_pHostLOT);
            }
        }
        #region Wheel 두께 Data SecsGEM 에 값 Save
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nWay"></param>
        /// <param name="nSel"></param> 0 - Wheel, 1 - Dress
        /// <param name="nWhen"></param> 0- Before, 1 - Aftere
        /// <param name="fData"></param>
        public void Measure_Wheel_Dress_Data_Set (int nWay, int nSel,int nWhen)
        {
            if (nWay == (int)EWay.L)// Left
            {
                if (nSel == 0)
                {// Wheel Before (0), After (1)
                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[nWhen, 0] = CData.WhlMea[0, 0] + 0.0005;// Left Wheel Data                    
                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[nWhen, 1] = CData.WhlMea[0, 1] + 0.0005;// Left Wheel Data
                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[nWhen, 2] = CData.WhlMea[0, 2] + 0.0005;// Left Wheel Data
                    
                    if (nWhen == 0)
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[nWhen, 3]    = CData.Whls[0].dWhlBf + 0.0005; // Left Wheel Max Data
                    else
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[nWhen, 3] = CData.Whls[0].dWhlAf + 0.0005;// Left Wheel Max Data

                    //Wheel_Left_Thickness_Data_Set();

                    if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
                    {
                        StdA_Wheel_Left_Thickness_Data_Set();
                    }
                    else
                    {
                        StdB_Wheel_Left_Thickness_Data_Set();
                    }
                }
                else 
                {// Dress
                    // 0 ~ 2 는 Empty , Dress 는 한번 측정 하니까
                    if (nWhen == 0)
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Left_Thick[nWhen, 3]  = CData.Whls[0].dDrsBf + 0.0005;// Left Dress Max Data
                    else
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Left_Thick[nWhen, 3] = CData.Whls[0].dDrsAf + 0.0005;// Left Dress Max Data

                    //Dress_Left_Thickness_Data_Set();

                    if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
                    {
                        StdA_Dress_Left_Thickness_Data_Set();
                    }
                    else
                    {
                        StdB_Dress_Left_Thickness_Data_Set();
                    }
                }
            }
            else //Right
            {
                if (nSel == 0)
                {// Wheel
                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[nWhen, 0] = CData.WhlMea[1, 0] + 0.0005;// Right Wheel Data
                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[nWhen, 1] = CData.WhlMea[1, 1] + 0.0005;// Right Wheel Data
                    CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[nWhen, 2] = CData.WhlMea[1, 2] + 0.0005;// Right Wheel Data
                    
                    if (nWhen == 0)
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[nWhen, 3] = CData.Whls[1].dWhlBf + 0.0005;// Right Wheel Max Data
                    else
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[nWhen, 3] = CData.Whls[1].dWhlAf + 0.0005;// Right Wheel Max Data

                    //Wheel_Right_Thickness_Data_Set();
                    
                    if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
                    {
                        StdA_Wheel_Right_Thickness_Data_Set();
                    }
                    else
                    {
                        StdB_Wheel_Right_Thickness_Data_Set();
                    }

                }
                else
                {// Dress
                    // 0 ~ 2 는 Empty , Dress 는 한번 측정 하니까
                    if (nWhen == 0)
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Right_Thick[nWhen, 3]  = CData.Whls[1].dDrsBf + 0.0005;// Right Dress Max Data
                    else
                        CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Right_Thick[nWhen, 3] = CData.Whls[1].dDrsAf + 0.0005;// Right Dress Max Data

                    if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
                    {
                        StdA_Dress_Right_Thickness_Data_Set();
                    }
                    else
                    {
                        StdB_Dress_Right_Thickness_Data_Set();
                    }
                }
            }
        }
        #endregion

        #region Strip Data Shift 함수 
        /// <summary>
        ///  Strip Data Shift 함수 
        // 5 : Picker Data
        // 6 : Left Table Data
        // 7 : Left Table Before Data
        // 10: Left Table After Data
        // 11: Right Table Before Data
        // 14: Right Table After Data
        // 20: Dry Data
        
        public void Strip_Data_Shift(int nSrc_num, int nDest_Num)
        {
            /// 2020.04.24 Work Table Count 설정
            /// Left Table 에서 작업 (1),Right Table 에서 작업 (2), L/ R 작업  (3)
            //if      (nSrc_num == 4)     { CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location = 0; } // Picker 가 Rail 에서 Strip Pick'up 시 초기화
            //else if (nSrc_num == 10)    { CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location = 1; } // Left Table Vacuum On 후 "1" Set                                
            //else if (nSrc_num == 16)    // Right Table Vacuum On 후 "+2"
            if      (nSrc_num == (int)EDataShift.INR_BCR_READ/*4*/)   { CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location = 0; } // Picker 가 Rail 에서 Strip Pick'up 시 초기화
            else if (nSrc_num == (int)EDataShift.GRL_AF_MEAS/*10*/)   { CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location = 1; } // Left Table Vacuum On 후 "1" Set                                
            else if (nSrc_num == (int)EDataShift.GRR_AF_MEAS/*16*/)   // Right Table Vacuum On 후 "+2" 
            {
                if (CData.Dev.bDual == eDual.Dual)
                {//step mode
                    CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location += 2;

                    if (CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location > 2)
                    {
                        CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location = 3;
                    }
                }// normal                 
                else
                {
                    CData.JSCK_Gem_Data[nSrc_num].nTable_Work_Location = 2;
                }
            }

            CData.JSCK_Gem_Data[nDest_Num] = CData.JSCK_Gem_Data[nSrc_num].clone();
            
            //sMsgdata = string.Format("Strip_Data_Shift nSrc_num -> {0},nDest_Num -> {1} CData.JSCK_Gem_Data[{2}].nTable_Work_Location : {3},Src-{4}, Dest-{5}, MgzID:{6}",
            sMsgdata = string.Format("[Data Shift] Src({0}) -> Dest({1}), Table_Work_Location : {2}, Src : {3}, Dest : {4}",
                                            nSrc_num, nDest_Num, CData.JSCK_Gem_Data[nDest_Num].nTable_Work_Location,
                                            CData.JSCK_Gem_Data[nSrc_num].sInRail_Verified_Strip_ID,
                                            CData.JSCK_Gem_Data[nDest_Num].sInRail_Verified_Strip_ID);
            //CData.JSCK_Gem_Data[nSrc_num] = new JSCK_SECSGEM_STRIP_DATA();
            Log_wt(sMsgdata);
        }
        #endregion

        #region
        /// <summary>
        /// CEID 999307    OnCarrier_Grind_Finish
        /// nTableLoc = 1(Left), 2(Right)
        /// 2020년 10월 31일
        public void OnCarrierGrindFinished(int nTableLoc)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnCarrierGrindFinished(nTableLoc);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnCarrierGrindFinished(nTableLoc); }
                else { StdB_OnCarrierGrindFinished(nTableLoc); }
            }
           
        }
        public void StdA_OnCarrierGrindFinished(int nTableLoc)
        {
            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnCarrierGrindFinished(nTableLoc);
            //    });
            //}
            //else
            {
                // 2022.04.04 SungTae Start : [수정] Log 추가
                //if (!CData.Opt.bSecsUse) return;
                if (!CData.Opt.bSecsUse)
                {
                    Log_wt($"[SEND](H<-E) S6F11 CEID = {(int)JSCK.eCEID.Carrier_Grind_Finished}({JSCK.eCEID.Carrier_Grind_Finished}). Secs/Gem : {CData.Opt.bSecsUse}, Table Loc : {nTableLoc}");
                    return;
                }
                // 2022.04.04 SungTae End

                string sET = string.Empty;
                float ftemp = 0;

                /* Left Table 일경우 Event 번호 10, Right Table 일경우 Event 번호 16 */ 
                int nEvent_nm = (int)EDataShift.GRL_AF_MEAS/*10*/;

                if (nTableLoc == 1) nEvent_nm = (int)EDataShift.GRL_AF_MEAS/*10*/; // Left 에서 Grind 완료 시
                else                nEvent_nm = (int)EDataShift.GRR_AF_MEAS/*16*/; // Normal , Dual 일 경우 nTableLoc =  2 또는 3

                string sDF_P_Min = "";
                string sDF_P_Max = "";
                string sDF_P_Avg = "";
                
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Carrier ID = {2}, Table Loc : {3}",
                                                                Convert.ToInt32(JSCK.eCEID.Carrier_Grind_Finished),
                                                                JSCK.eCEID.Carrier_Grind_Finished.ToString(),
                                                                CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID,
                                                                nTableLoc.ToString()));
                
                string sSideIdx = GetMoldSideInfo(CData.Dev.eMoldSide);     // "0";     // "0" = Top, "1" = Btm, "2" = TopD

                int i = 0;
                int nCk_Point = 0;

                CData.JSCK_Gem_Data[(int)EDataShift.GRL_BF_MEAS/*7*/].sSaBUN = "";

                // 2022.05.31 SungTae Start : [수정] 코드 정리
                #region Table Grind Finish Data Set : Left(nEvent_nm = 10) / Right(nEvent_nm = 16)
                
                sDF_P_Min = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Min_Data[0].ToString();
                sDF_P_Max = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Max_Data[0].ToString();
                sDF_P_Avg = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Avr_Data[0].ToString();

                Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX)    , sDF_P_Max);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR)    , sDF_P_Avg);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE)   , sSideIdx);
  
                if (CDataOption.UseNewSckGrindProc) // SCK 전용 : TopMold, BtmMold 올리기
                {
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Avg.ToString());
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Max.ToString());
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Avg.ToString());
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Max.ToString());
                }
                
                mgem.WriteUserLog($">> OnCarrierGrindFinished() - nEventNum : {nEvent_nm}, InRail_Verified_Strip_ID = {CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID}");
                
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc)  , nTableLoc.ToString());

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                #endregion

                if (nTableLoc == 1)
                {
                    ////#region Left Table Grind Finish Data Set :  nEvent_nm = 10;

                    //// 2021.10.07 lhs Start : 코드 정리
                    //sDF_P_Min = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Min_Data[0].ToString();
                    //sDF_P_Max = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Max_Data[0].ToString();
                    //sDF_P_Avg = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Avr_Data[0].ToString();

                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX)    , sDF_P_Max);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR)    , sDF_P_Avg);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE)   , sSideIdx);
                    //// 2021.10.07 lhs End

                    //// 2021.07.31 lhs Start : 새로운 Grind Process 추가 (SCK 전용)
                    //if (CDataOption.UseNewSckGrindProc)
                    //{
                    //    // TopMold, BtmMold 올리기
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Avg.ToString());
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Max.ToString());
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Avg.ToString());
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Max.ToString());
                    //}
                    //// 2021.07.31 lhs End

                    //mgem.WriteUserLog(string.Format("CData.JSCK_Gem_Data[{0}].sInRail_Verified_Strip_ID={1}", nEvent_nm, CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID));
                    ////---------------------
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc)  , nTableLoc.ToString());

                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                    
                    #region Left Table Measure Left Min,Max Avr Data
                    mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                    mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
					//-------- 
					mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
					mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[1].ToString())); // before 
					mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[1].ToString())); // before
					mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[1].ToString())); // before

					Log_wt(string.Format("OnCarrierGrindFinished 0 ------- {0}, {1}, {2}",  CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[1],
																							CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[1],
																							CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[1]));
					mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
					//--------
					mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
					mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[4].ToString())); // after
					mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[4].ToString())); // after
					mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[4].ToString())); // after

					Log_wt(string.Format("OnCarrierGrindFinished 1 ------- {0}, {1}, {2}",  CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[4],
																							CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[4],
																							CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[4]));
					mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
					//--------
					mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                    #endregion

                    #region Measure Left Raw Data
                    mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));// Before Data

                    nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[1];//Left Befor Measure 측정 갯수 확인 필요

                    if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    {
                        ftemp = 0;
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), ftemp);// Befor Data
                    }
                    else 
                    {
                        for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                        {
                            mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point),  float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[1, i].ToString()));// Befor Data
                            sMsgdata = string.Format("left befor ={0},{1}", i,                  float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[1, i].ToString()));
                            Log_wt(sMsgdata);
                        }
                    }

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                    mgem.OpenListItem (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point)); // Aftre Data

                    nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[4];// Left After Measure 측정 갯수 확인 필요
                    
                    if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    {
                        ftemp = 0;
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), ftemp);
                    }
                    else 
                    {
                        for (i = 0; i < nCk_Point; i++)
                        {
                            mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point),  float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[4, i].ToString())); // Left After Data
                            sMsgdata = string.Format("left after ={0},{1}", i,                  float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[4, i].ToString()));
                            Log_wt(sMsgdata);
                        }
                    }

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                    #endregion
                    //#endregion
                } // end : if (nTableLoc == 1)
                else 
                {
                    //#region Right Table Grind Finish Data Set : nEvent_nm = 16;

                    //// 2021.10.07 lhs Start : 코드 정리
                    //sDF_P_Min = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Min_Data[0].ToString();
                    //sDF_P_Max = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Max_Data[0].ToString();
                    //sDF_P_Avg = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Avr_Data[0].ToString();

                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX)    , sDF_P_Max);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR)    , sDF_P_Avg);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE)   , sSideIdx);
                    //// 2021.10.07 lhs End

                    //// 2021.07.31 lhs Start : 새로운 Grind Process 추가 (SCK 전용)
                    //if (CDataOption.UseNewSckGrindProc)
                    //{
                    //    // TopMold, BtmMold 올리기
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Avg.ToString());
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Max.ToString());
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Avg.ToString());
                    //    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Max.ToString());
                    //}
                    //// 2021.07.31 lhs End

                    //mgem.WriteUserLog(string.Format("CData.JSCK_Gem_Data[{0}].sInRail_Verified_Strip_ID={1}", nEvent_nm, CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID));
                    ////---------------------
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID) , CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc)  , nTableLoc.ToString());

                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                    //Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);

                    #region 프로그램 재실행 후 첫번째 Top Grinding 자재에 대한 Event 보고일 경우

                    if (CData.CurCompany == ECompany.ASE_KR && CData.Dev.eMoldSide == ESide.Top && CData.bChkFirstTopGrdFlag)
                    {
                        #region Left Table Measure Left Min,Max Avr Data
                        mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                        mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                            //-------- 
                            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse("0.0000")); // before 
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse("0.0000")); // before
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse("0.0000")); // before

                                Log_wt($"OnCarrierGrindFinished 0 ------- {(float)0:F4}, {(float)0:F4}, {(float)0:F4}");

                            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                            //--------
                            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse("0.0000")); // after
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse("0.0000")); // after
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse("0.0000")); // after

                                Log_wt($"OnCarrierGrindFinished 1 ------- {(float)0:F4}, {(float)0:F4}, {(float)0:F4}");

                        mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                            //--------
                        mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                        #endregion

                        #region Measure Left Raw Data
                        mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                        mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));// Before Data

                            nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[1];//Left Befor Measure 측정 갯수 확인 필요

                            if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                            {
                                ftemp = 0;
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), ftemp);// Befor Data
                            }
                            else
                            {
                                for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                                {
                                    CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[1, i] = 0.0;

                                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[1, i].ToString()));// Befor Data
                                    sMsgdata = string.Format("left befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[1, i].ToString()));
                                    Log_wt(sMsgdata);
                                }
                            }

                            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point)); // Aftre Data

                            nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[4];// Left After Measure 측정 갯수 확인 필요

                            if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                            {
                                ftemp = 0;
                                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), ftemp);
                            }
                            else
                            {
                                for (i = 0; i < nCk_Point; i++)
                                {
                                    CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[4, i] = 0.0;

                                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[4, i].ToString())); // Left After Data
                                    sMsgdata = string.Format("left after ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[4, i].ToString()));
                                    Log_wt(sMsgdata);
                                }
                            }

                            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                        mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                        #endregion

                        CData.bChkFirstTopGrdFlag = false;
                    }
                    #endregion

                    #region Right Table Measure Right Min,Max, Avr Data
                    mgem.CloseMsg       (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                    mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                    mgem.OpenListItem   (Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));

                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[5].ToString())); // before                
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[5].ToString())); // before
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[5].ToString())); // before
                    
                    Log_wt(string.Format("OnCarrierGrindFinished 0 ++++++++++ {0}, {1}, {2}",        CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[5],
                                                                                                     CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[5],
                                                                                                     CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[5]));

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                    
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[6].ToString())); // after
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[6].ToString())); // after
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[6].ToString())); // after
                    
                    Log_wt(string.Format("OnCarrierGrindFinished 1 ++++++++++ {0}, {1}, {2}",        CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[6],
                                                                                                     CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[6],
                                                                                                     CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[6]));

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                    #endregion

                    #region Measure Right Raw Data
                    mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

                    nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[5];// Right Befor Measure 측정 갯수 확인 필요                            

                    if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    {
                        ftemp = 0;
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), ftemp);// Befor Data
                    }
                    else
                    {
                        for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                        {
                            mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[5, i].ToString()));// Befor Data
                            sMsgdata = string.Format("Right befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[5, i].ToString()));
                            Log_wt(sMsgdata);
                        }
                    }

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

                    nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[6];// Right After Measure 측정 갯수 확인 필요
                                                                                 // 
                    if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    {
                        ftemp = 0;
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), ftemp);// Befor Data
                    }
                    else
                    {
                        for (i = 0; i < nCk_Point; i++)
                        {
                            mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[6, i].ToString()));// After Data
                            sMsgdata = string.Format("Right After ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[6, i].ToString()));
                            Log_wt(sMsgdata);
                        }
                    }

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                    #endregion
                    //#endregion
                }
                // 2022.05.31 SungTae End

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Grind_Finished).ToString()); // Carrier_Grind_Finished  = 999307, // add V1.0.16b , 20201031 ASE-KR
            }
        }
        public void StdB_OnCarrierGrindFinished(int nTableLoc) 
        {
            if (!CData.Opt.bSecsUse)
            {
                Log_wt($"[SEND](H<-E) S6F11 CEID = {(int)JSCK.eBCEID.Carrier_Grind_Finished}({JSCK.eBCEID.Carrier_Grind_Finished}). " +
                    $"Secs/Gem : {CData.Opt.bSecsUse}, Table Loc : {nTableLoc}");
                return;
            }
            // 2022.04.04 SungTae End

            string sET = string.Empty;
            float ftemp = 0;

            /* Left Table 일경우 Event 번호 10, Right Table 일경우 Event 번호 16 */
            int nEvent_nm = (int)EDataShift.GRL_AF_MEAS/*10*/;

            if (nTableLoc == 1) nEvent_nm = (int)EDataShift.GRL_AF_MEAS/*10*/; // Left 에서 Grind 완료 시
            else nEvent_nm = (int)EDataShift.GRR_AF_MEAS/*16*/; // Normal , Dual 일 경우 nTableLoc =  2 또는 3

            string sDF_P_Min = "";
            string sDF_P_Max = "";
            string sDF_P_Avg = "";

            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Carrier ID = {2}, Table Loc : {3}",
                                                            Convert.ToInt32(JSCK.eBCEID.Carrier_Grind_Finished),
                                                            JSCK.eBCEID.Carrier_Grind_Finished.ToString(),
                                                            CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID,
                                                            nTableLoc.ToString()));

            string sSideIdx = GetMoldSideInfo(CData.Dev.eMoldSide);     // "0";     // "0" = Top, "1" = Btm, "2" = TopD

            int i = 0;
            int nCk_Point = 0;

            CData.JSCK_Gem_Data[(int)EDataShift.GRL_BF_MEAS/*7*/].sSaBUN = "";

            // 2022.05.31 SungTae Start : [수정] 코드 정리
            #region Table Grind Finish Data Set : Left(nEvent_nm = 10) / Right(nEvent_nm = 16)

            sDF_P_Min = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Min_Data[0].ToString();
            sDF_P_Max = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Max_Data[0].ToString();
            sDF_P_Avg = CData.JSCK_Gem_Data[nEvent_nm].fMeasure_New_Avr_Data[0].ToString();

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_MAX), sDF_P_Max);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_AVR), sDF_P_Avg);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
            //Set_SVID(Convert.ToInt32(JSCK.eBSVID.PCB_ORIENT_VALUE), sSideIdx);

            if (CDataOption.UseNewSckGrindProc) // SCK 전용 : TopMold, BtmMold 올리기
            {
                //Set_SVID(Convert.ToInt32(JSCK.eBSVID.TOP_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Avg.ToString());
                //Set_SVID(Convert.ToInt32(JSCK.eBSVID.TOP_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_TopMold_Max.ToString());
                //Set_SVID(Convert.ToInt32(JSCK.eBSVID.BTM_THK_AVG), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Avg.ToString());
                //Set_SVID(Convert.ToInt32(JSCK.eBSVID.BTM_THK_MAX), CData.JSCK_Gem_Data[nEvent_nm].dMeasure_BtmMold_Max.ToString());
            }

            mgem.WriteUserLog($">> OnCarrierGrindFinished() - nEventNum : {nEvent_nm}, InRail_Verified_Strip_ID = {CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID}");
            if (nTableLoc.Equals(1))
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Stage1_CarrierID), CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Stage2_CarrierID), "");
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Stage1_CarrierID), "");
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Stage2_CarrierID), CData.JSCK_Gem_Data[nEvent_nm].sInRail_Verified_Strip_ID);
            }
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Table_Location), nTableLoc.ToString());

            #endregion

            if (nTableLoc == 1)
            {
                #region Left Table Measure Left Min,Max Avr Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                //-------- 
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.L_Before].ToString())); // before 
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.L_Before].ToString())); // before
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.L_Before].ToString())); // before

                Log_wt(string.Format("OnCarrierGrindFinished 0 ------- {0}, {1}, {2}", CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.L_Before],
                                                                                        CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.L_Before],
                                                                                        CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.L_Before]));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                //--------
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.L_After].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.L_After].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.L_After].ToString())); // after

                Log_wt(string.Format("OnCarrierGrindFinished 1 ------- {0}, {1}, {2}", CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.L_After],
                                                                                        CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.L_After],
                                                                                        CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.L_After]));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                //--------
                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                #endregion

                #region Measure Left Raw Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));// Before Data

                nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[(int)EPoint.L_Before];//Left Befor Measure 측정 갯수 확인 필요

                if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), ftemp);// Befor Data
                }
                else
                {
                    for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                    {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_Before, i].ToString()));// Befor Data
                        sMsgdata = string.Format("left befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_Before, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }

                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point)); // Aftre Data

                nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[(int)EPoint.L_After];// Left After Measure 측정 갯수 확인 필요

                if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), ftemp);
                }
                else
                {
                    for (i = 0; i < nCk_Point; i++)
                    {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_After, i].ToString())); // Left After Data
                        sMsgdata = string.Format("left after ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_After, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }

                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                #endregion
                //#endregion
            } // end : if (nTableLoc == 1)
            else
            {
                
                #region 프로그램 재실행 후 첫번째 Top Grinding 자재에 대한 Event 보고일 경우

                if (CData.CurCompany == ECompany.ASE_KR && CData.Dev.eMoldSide == ESide.Top && CData.bChkFirstTopGrdFlag)
                {
                    #region Left Table Measure Left Min,Max Avr Data
                    mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    //-------- 
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse("0.0000")); // before 
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse("0.0000")); // before
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse("0.0000")); // before

                    Log_wt($"OnCarrierGrindFinished 0 ------- {(float)0:F4}, {(float)0:F4}, {(float)0:F4}");

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    //--------
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse("0.0000")); // after
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse("0.0000")); // after
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse("0.0000")); // after

                    Log_wt($"OnCarrierGrindFinished 1 ------- {(float)0:F4}, {(float)0:F4}, {(float)0:F4}");

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    //--------
                    mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
                    #endregion

                    #region Measure Left Raw Data
                    mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));// Before Data

                    nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[(int)EPoint.L_Before];//Left Befor Measure 측정 갯수 확인 필요

                    if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    {
                        ftemp = 0;
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), ftemp);// Befor Data
                    }
                    else
                    {
                        for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                        {
                            CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_Before, i] = 0.0;

                            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_Before, i].ToString()));// Befor Data
                            sMsgdata = string.Format("left befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_Before, i].ToString()));
                            Log_wt(sMsgdata);
                        }
                    }

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point)); // Aftre Data

                    nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[(int)EPoint.L_After];// Left After Measure 측정 갯수 확인 필요

                    if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    {
                        ftemp = 0;
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), ftemp);
                    }
                    else
                    {
                        for (i = 0; i < nCk_Point; i++)
                        {
                            CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_After, i] = 0.0;

                            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_After, i].ToString())); // Left After Data
                            sMsgdata = string.Format("left after ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.L_After, i].ToString()));
                            Log_wt(sMsgdata);
                        }
                    }

                    mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                    mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
                    #endregion

                    CData.bChkFirstTopGrdFlag = false;
                }
                #endregion

                #region Right Table Measure Right Min,Max, Avr Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));

                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.R_Before].ToString())); // before                
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.R_Before].ToString())); // before
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.R_Before].ToString())); // before

                Log_wt(string.Format("OnCarrierGrindFinished 0 ++++++++++ {0}, {1}, {2}", CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.R_Before],
                                                                                                 CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.R_Before],
                                                                                                 CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.R_Before]));

                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));

                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.R_After].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.R_After].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.R_After].ToString())); // after

                Log_wt(string.Format("OnCarrierGrindFinished 1 ++++++++++ {0}, {1}, {2}", CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Min_Data[(int)EPoint.R_After],
                                                                                                 CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Max_Data[(int)EPoint.R_After],
                                                                                                 CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Avr_Data[(int)EPoint.R_After]));

                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
                #endregion

                #region Measure Right Raw Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));

                nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[(int)EPoint.R_Before];// Right Befor Measure 측정 갯수 확인 필요                            

                if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), ftemp);// Befor Data
                }
                else
                {
                    for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                    {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.R_Before, i].ToString()));// Befor Data
                        sMsgdata = string.Format("Right befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.R_Before, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }

                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));

                nCk_Point = CData.JSCK_Gem_Data[nEvent_nm].nMeasure_Count[(int)EPoint.R_After];// Right After Measure 측정 갯수 확인 필요
                                                                             // 
                if (nCk_Point == 0)// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), ftemp);// Befor Data
                }
                else
                {
                    for (i = 0; i < nCk_Point; i++)
                    {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.R_After, i].ToString()));// After Data
                        sMsgdata = string.Format("Right After ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[nEvent_nm].fMeasure_Data[(int)EPoint.R_After, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }

                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
                #endregion
                //#endregion
            }
            // 2022.05.31 SungTae End

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Grind_Finished).ToString()); // Carrier_Grind_Finished  = 999307, // add V1.0.16b , 20201031 ASE-KR
        }
        #endregion

        /// <summary>
        /// // 2020.11.23 SECS/GEM 1.0.18 SVID 추가
        /// </summary> SVID 999226 ~ 999229
        /// <param name="nTableLoc"></param>
        /// <param name="sLoad"></param>
        /// <param name="scount"></param>
        public void OnCarrierGrindEnd_Dataset(int nTableLoc, string sLoad, string scount)
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnCarrierGrindEnd_Dataset(nTableLoc, sLoad, scount); }
            else { StdB_OnCarrierGrindEnd_Dataset(nTableLoc, sLoad, scount); }
        }
        public void StdA_OnCarrierGrindEnd_Dataset(int nTableLoc, string sLoad, string scount)
        {
            if (nTableLoc == 3) //201216 jhc : Debugging by cy01.lim 
            {
                #region Left Data Set
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Max_Spindle_Load)       , sLoad);// SVID 999226
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Left_Dress_After_Strip_count), scount);// SVID 999228
                Log_wt(string.Format("OnCarrierGrindEnd_Dataset Left {0}, {1}, {2}", nTableLoc, sLoad, scount));

                #endregion
            }
            else
            {
                #region Right Data Set
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Max_Spindle_Load)         , sLoad); // SVID 999227
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Right_Dress_After_Strip_count)  , scount);// SVID 999229 
                
                // 2022.04.04 SungTae : [수정] 오타 수정
                //Log_wt(string.Format("OnCarrierGrindEnd_Dataset Righjt {0}, {1}, {2}", nTableLoc, sLoad, scount));
                Log_wt(string.Format("OnCarrierGrindEnd_Dataset Right {0}, {1}, {2}", nTableLoc, sLoad, scount));
                #endregion
            }
        }
        public void StdB_OnCarrierGrindEnd_Dataset(int nTableLoc, string sLoad, string scount)
        {
            if (nTableLoc == 3) //201216 jhc : Debugging by cy01.lim 
            {
                #region Left Data Set
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Left_Max_Spindle_Load), sLoad);
                //Set_SVID(Convert.ToInt32(JSCK.eBSVID.Left_Dress_After_Strip_count), scount);
                Log_wt($"OnCarrierGrindEnd_Dataset Left TableLoc:{nTableLoc}, Load:{sLoad}, count:{scount}");

                #endregion
            }
            else
            {
                #region Right Data Set
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Right_Max_Spindle_Load), sLoad); 
                //Set_SVID(Convert.ToInt32(JSCK.eBSVID.Right_Dress_After_Strip_count), scount);

                Log_wt($"OnCarrierGrindEnd_Dataset Right TableLoc:{nTableLoc}, Load:{sLoad}, count:{scount}");
                #endregion
            }
        }
        /// <summary>
        /// CEID 999203    Carrier Ended
        /// </summary>
        /// <param name="sLotId"></param>
        /// <param name="sCarrierId"></param>
        /// <param name="nSlotNo"></param>
        /// <param name="nTableLoc"></param>
        /// <param name="nGoodNg"></param>
        public void OnCarrierEnded(string sLotId, string sCarrierId, uint nSlotNo, int nTableLoc, uint nGoodNg)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnCarrierEnded(sLotId, sCarrierId, nSlotNo, nTableLoc, nGoodNg);
                });

            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnCarrierEnded(sLotId, sCarrierId, nSlotNo, nTableLoc, nGoodNg); }
                else { StdB_OnCarrierEnded(sLotId, sCarrierId, nSlotNo, nTableLoc, nGoodNg); }
            }
        }
        public void StdA_OnCarrierEnded(string sLotId, string sCarrierId, uint nSlotNo, int nTableLoc, uint nGoodNg)
        {
            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnCarrierEnded(sLotId, sCarrierId, nSlotNo, nTableLoc, nGoodNg);
            //    });

            //}
            //else
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                string sCarrierMap = "1111111111111";
                string sCarrierStartTime = string.Empty;
                string sET = string.Empty;
                float ftemp = 0;

                int nIdx = -1;      // 2021.08.31 SungTae : [추가] Multi-LOT 관련

                nTableLoc = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nTable_Work_Location;

                // 2021.08.27 SungTae Start : [추가]
                //sMsgdata = string.Format("OnCarrierEnded({0},{1},{2},{3},{4}", sLotId, sCarrierId, nSlotNo, nTableLoc, nGoodNg);
                sMsgdata = string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}). LOT = {2}, Carrier ID = {3}, Slot No : {4}, Table Loc : {5}, Judgement : {6}",
                                                                    Convert.ToInt32(JSCK.eCEID.Carrier_Ended),
                                                                    JSCK.eCEID.Carrier_Ended.ToString(),
                                                                    sLotId, sCarrierId, nSlotNo, nTableLoc, (nGoodNg == 1) ? "GOOD" : "NG");
                // 2021.08.27 SungTae End

                Log_wt(sMsgdata);
                
                mgem.WriteUserLog(string.Format("sCarrierID={0}" , sCarrierId));

                string sDF_P_Min;
                string sDF_P_Max;
                string sDF_P_Avg;
				
				// 2021.08.05 SungTae Start : [수정] SVID=999212(PCB_TOPBTM_USE_MODE) 바뀌는 것과 관련하여 임희성 책임님이 수정한 것을 함수로 변경.
				string sSideIdx = GetMoldSideInfo(CData.Dev.eMoldSide);     // "0";

                // 2021.10.07 lhs Start : 코드 정리
                sDF_P_Min = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_New_Min_Data[0].ToString();
                sDF_P_Max = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_New_Max_Data[0].ToString();
                sDF_P_Avg = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_New_Avr_Data[0].ToString();
                
                Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_MAX),       sDF_P_Max);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.DF_DATA_PCB_AVR),       sDF_P_Avg);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_TOPBTM_USE_MODE),   sSideIdx);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE),      sSideIdx);
                // 2021.10.07 lhs End

                // 2021.07.31 lhs Start : 새로운 Grind Process 추가 (SCK 전용)
                if (CDataOption.UseNewSckGrindProc)
                {
					// TopMold, BtmMold 올리기
					Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_AVG), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_TopMold_Avg.ToString());
					Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_MAX), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_TopMold_Max.ToString());
					Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_AVG), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_BtmMold_Avg.ToString());
					Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_MAX), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_BtmMold_Max.ToString());
				}

                mgem.WriteUserLog(string.Format("CData.JSCK_Gem_Data[23].sInRail_Verified_Strip_ID={0}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID));

                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Slot_Num = (int)nSlotNo;             
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name = sLotId;

                if (nGoodNg == (int)JSCK.eMGZPort.GOOD)
                {
                    Set_SVID((int)JSCK.eSVID.Magazine_ID,     CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);                
                    Set_SVID((int)JSCK.eSVID.MGZ_Slot_Number, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Slot_Num.ToString());
                    Set_SVID((int)JSCK.eSVID.MGZ_Port_Number, nGoodNg.ToString());

                    // 2021.09.06 SungTae Start : [추가] Multi-LOT 관련
                    if (CData.LotMgr.CheckExistLOT(sLotId) == false)
                    {
                        m_pMgzGoodCarrierList.ClearCarrier();
                        m_pEndedCarrierList.ClearCarrier();
                    }
                    // 2021.09.06 SungTae End

                    m_pMgzGoodCarrierList.AddCarrier( CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID );
                    m_pEndedCarrierList.AddCarrier  ( CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID );

                    // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                    if (CData.IsMultiLOT())
                    {
                        nIdx = CData.LotMgr.GetListIndexNum(sLotId);

                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.End));
                    }
                    else
                    {
                        CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.End));
                    }
                    // 2021.08.25 SungTae End

                    sMsgdata = string.Format("OnCarrierEnded() Judgment = {0}", JSCK.eMGZPort.GOOD.ToString());
                }
                else if (nGoodNg == (int)JSCK.eMGZPort.NG)
                {
                    Set_SVID((int)JSCK.eSVID.Magazine_ID,       CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                    Set_SVID((int)JSCK.eSVID.MGZ_Slot_Number,   CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Slot_Num.ToString());
                    Set_SVID((int)JSCK.eSVID.MGZ_Port_Number, nGoodNg.ToString());

                    // 2021.09.06 SungTae Start : [추가] Multi-LOT 관련
                    if (CData.LotMgr.CheckExistLOT(sLotId) == false)
                    {
                        m_pMgzNGCarrierList.ClearCarrier();
                        m_pRejectCarrierList.ClearCarrier();
                    }
                    // 2021.09.06 SungTae End

                    m_pMgzNGCarrierList.AddCarrier  ( CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID );
                    m_pRejectCarrierList.AddCarrier ( CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID );

                    // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                    if (CData.IsMultiLOT())
                    {
                        nIdx = CData.LotMgr.GetListIndexNum(sLotId);
                        
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Reject));
                    }
                    else
                    {
                        CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Reject));
                    }
                    // 2021.08.25 SungTae End

                    sMsgdata = string.Format("OnCarrierEnded() Judgment = {0}", JSCK.eMGZPort.NG.ToString());
                }

                Log_wt(sMsgdata);
                
                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));
                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));

                //---------------------
                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID),        CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID),    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Map),   sCarrierMap);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc),     nTableLoc.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID),    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID),    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT()) 
                    sCarrierStartTime = CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.GetStartTime( CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                else
                    sCarrierStartTime = CData.m_pHostLOT.GetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                // 2021.08.25 SungTae End

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Started_Time), sCarrierStartTime);

                sMsgdata = string.Format("Strip start time's strip id={0}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                mgem.WriteUserLog(sMsgdata);
                Log_wt(sMsgdata);
                sMsgdata = string.Format("Strip start time ={0}", sCarrierStartTime);
                Log_wt(sMsgdata);

                DateTime now = DateTime.Now;
                sET = now.ToString("yyyyMMddHHmmss");
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_Ended_Time), sET);


                #region Measure Left Min,Max Avr Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                //-----------
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));             

                if(nTableLoc == 2)	// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
				{
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[1] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[1] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[1] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[4] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[4] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[4] = 0;
                }

                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[1].ToString())); // before                            
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[1].ToString())); // before
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[1].ToString())); // before
                Log_wt(string.Format("0 ------- {0}, {1}, {2}",     CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[1],
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[1]  ,
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[1]));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[4].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[4].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[4].ToString())); // after

                Log_wt(string.Format("1 ------- {0}, {1}, {2}",     CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[4],
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[4]  ,
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[4]));

                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                //-----------
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Work_Point));
                #endregion
                
                #region Measure Left Raw Data
                mgem.CloseMsg    (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));// Before Data
                int i = 0;
                int nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[1];//Left Befor Measure 측정 갯수 확인 필요            
                if((nCk_Point == 0) || (nTableLoc == 2))// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), ftemp);// Befor Data
                }else {
                    for (i = 0; i< nCk_Point; i++)// Measure 측정 갯수 확인 필요
                    {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[1,i].ToString()) );// Befor Data
                        sMsgdata = string.Format("left befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[1, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                mgem.OpenListItem (Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point)); // Aftre Data

                nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[4];// Left After Measure 측정 갯수 확인 필요            
                
                if((nCk_Point == 0) || (nTableLoc == 2))// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), ftemp);
                }
				else
				{
                    for (i = 0; i< nCk_Point; i++)
                     {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[4, i].ToString()));// After Data
                        sMsgdata = string.Format("left after ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[4, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Left_Raw_Point));
                #endregion

                #region Measure Right Min,Max, Avr Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));

                CData.JSCK_Gem_Data[(int)EDataShift.GRL_BF_MEAS/*7*/].sSaBUN = "";

                if (nTableLoc == 1)
                {// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[5] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[5] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[5] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[6] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[6] =
                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[6] = 0;
                }
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[5].ToString())); // before                
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[5].ToString())); // before
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[5].ToString())); // before
                Log_wt(string.Format("0 ++++++++++ {0}, {1}, {2}",  CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[5],
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[5],
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[5]));

                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[6].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[6].ToString())); // after
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[6].ToString())); // after
                Log_wt(string.Format("1 ++++++++++ {0}, {1}, {2}",  CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[6],
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[6],
                                                                    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[6]));

                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Work_Point));
                #endregion

                #region Measure Right Raw Data
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

                nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[5];// Right Befor Measure 측정 갯수 확인 필요                            

                if ((nCk_Point == 0) || (nTableLoc == 1))// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), ftemp);// Befor Data

                }
                else {
                    for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                    {
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[5, i].ToString()));// Befor Data
                        sMsgdata = string.Format("Right befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[5, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

                nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[6];// Right After Measure 측정 갯수 확인 필요            
                
                if ((nCk_Point == 0) || (nTableLoc == 1))   // nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                {
                    ftemp = 0;
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), ftemp);// Befor Data
                }
                else {
                    for (i = 0; i < nCk_Point; i++)
                    {
                        //Log_wt(string.Format("1 ++++++++++ {0}, {1}, {2}",loat.Parse(CData.JSCK_Gem_Data[14].fMeasure_Data[4, i].ToString() );
                        mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[6, i].ToString()));// After Data
                        sMsgdata = string.Format("Right After ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[6, i].ToString()));
                        Log_wt(sMsgdata);
                    }
                }

                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
                #endregion

                #region Dress 진행 후 Update 하니 여기서는 설정 하지 말자
                /*
                #region //-- Wheel Left 두께  20200731
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));// Before Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Left_Thick[0, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Wheel_Left_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Left_Thick[0, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS)); // Aftre Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Left_Thick[1, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Wheel_Left_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Left_Thick[1, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
                #endregion

                #region//-- Wheel Right 두께  20200731
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));// Before Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Right_Thick[0, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Wheel_Right_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Right_Thick[0, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS)); // Aftre Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Right_Thick[1, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Wheel_Right_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Wheel_Right_Thick[1, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
                #endregion//--

                #region//-- Dress Left 두께  20200731
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));// Before Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Left_Thick[0, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Dress_Left_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Left_Thick[0, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS)); // Aftre Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Left_Thick[1, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Dress_Left_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Left_Thick[1, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
                #endregion//--

                #region//-- Dress Right 두께  20200731
                mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));// Before Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Right_Thick[0, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Dress_Right_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Right_Thick[0, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
                mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS)); // Aftre Data
                for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Right_Thick[1, i].ToString()));
                    sMsgdata = string.Format("fMeasure_Dress_Right_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[1].fMeasure_Dress_Right_Thick[1, i].ToString()));
                    Log_wt(sMsgdata);
                }
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
                mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
                #endregion//--
                */
                #endregion 20200911 LCY

                #region Magazine Data

                if (nGoodNg == (int)JSCK.eMGZPort.GOOD)
                { 
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New); 
                }
                else if (nGoodNg == (int)JSCK.eMGZPort.NG)
                { 
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New); 
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), nGoodNg.ToString());
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Slot_Number), nSlotNo.ToString());

                #endregion
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Summary_Uploaded).ToString());
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Ended).ToString());
                // 만약, lot의 마지막 carrier인 경우 lot end를 보고 후 Current Lotid를 공백으로 처리.

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    // 2021.10.19 SungTae Start : [수정] Multi-LOT 관련
                    sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    //for (i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                    //{
                    //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                    //}
                    // 2021.10.19 SungTae End
                }
                else
                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                // 2021.08.25 SungTae End
            }
        }
        public void StdB_OnCarrierEnded(string sLotId, string sCarrierId, uint nSlotNo, int nTableLoc, uint nGoodNg) 
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            string sCarrierStartTime = string.Empty;
            string sET = string.Empty;
            float ftemp = 0;

            int nIdx = -1;      // 2021.08.31 SungTae : [추가] Multi-LOT 관련

            nTableLoc = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nTable_Work_Location;

            // 2021.08.27 SungTae Start : [추가]
            //sMsgdata = string.Format("OnCarrierEnded({0},{1},{2},{3},{4}", sLotId, sCarrierId, nSlotNo, nTableLoc, nGoodNg);
            sMsgdata = string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}). LOT = {2}, Carrier ID = {3}, Slot No : {4}, Table Loc : {5}, Judgement : {6}",
                                                                Convert.ToInt32(JSCK.eBCEID.Carrier_Ended),
                                                                JSCK.eBCEID.Carrier_Ended.ToString(),
                                                                sLotId, sCarrierId, nSlotNo, nTableLoc, (nGoodNg == 1) ? "GOOD" : "NG");
            // 2021.08.27 SungTae End

            Log_wt(sMsgdata);

            mgem.WriteUserLog(string.Format("sCarrierID={0}", sCarrierId));

            string sDF_P_Min;
            string sDF_P_Max;
            string sDF_P_Avg;

            // 2021.08.05 SungTae Start : [수정] SVID=999212(PCB_TOPBTM_USE_MODE) 바뀌는 것과 관련하여 임희성 책임님이 수정한 것을 함수로 변경.
            string sSideIdx = GetMoldSideInfo(CData.Dev.eMoldSide);     // "0";

            // 2021.10.07 lhs Start : 코드 정리
            sDF_P_Min = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_New_Min_Data[(int)EPoint.DF].ToString();
            sDF_P_Max = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_New_Max_Data[(int)EPoint.DF].ToString();
            sDF_P_Avg = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_New_Avr_Data[(int)EPoint.DF].ToString();

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_MAX), sDF_P_Max);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.DF_DATA_PCB_AVR), sDF_P_Avg);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.PCB_TOPBTM_USE_MODE), sSideIdx);
            //Set_SVID(Convert.ToInt32(JSCK.eSVID.PCB_ORIENT_VALUE), sSideIdx);
            // 2021.10.07 lhs End

            // 2021.07.31 lhs Start : 새로운 Grind Process 추가 (SCK 전용)
            //if (CDataOption.UseNewSckGrindProc)
            //{
            //    // TopMold, BtmMold 올리기
            //    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_AVG), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_TopMold_Avg.ToString());
            //    Set_SVID(Convert.ToInt32(JSCK.eSVID.TOP_THK_MAX), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_TopMold_Max.ToString());
            //    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_AVG), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_BtmMold_Avg.ToString());
            //    Set_SVID(Convert.ToInt32(JSCK.eSVID.BTM_THK_MAX), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].dMeasure_BtmMold_Max.ToString());
            //}

            mgem.WriteUserLog(string.Format("CData.JSCK_Gem_Data[23].sInRail_Verified_Strip_ID={0}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID));

            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Slot_Num = (int)nSlotNo;
            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name = sLotId;

            if (nGoodNg == (int)JSCK.eMGZPort.GOOD)
            {
                Set_SVID((int)JSCK.eBSVID.GOOD_Magazine_ID, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                Set_SVID((int)JSCK.eBSVID.GOOD_Slot_ID, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Slot_Num.ToString());
                Set_SVID((int)JSCK.eBSVID.MGZ_Port_Number, nGoodNg.ToString());

                // 2021.09.06 SungTae Start : [추가] Multi-LOT 관련
                if (CData.LotMgr.CheckExistLOT(sLotId) == false)
                {
                    m_pMgzGoodCarrierList.ClearCarrier();
                    m_pEndedCarrierList.ClearCarrier();
                }
                // 2021.09.06 SungTae End

                m_pMgzGoodCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                m_pEndedCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(sLotId);

                    CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.End));
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.End));
                }
                // 2021.08.25 SungTae End

                sMsgdata = string.Format("OnCarrierEnded() Judgment = {0}", JSCK.eMGZPort.GOOD.ToString());
            }
            else if (nGoodNg == (int)JSCK.eMGZPort.NG)
            {
                //Set_SVID((int)JSCK.eBSVID.Magazine_ID, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
               // Set_SVID((int)JSCK.eBSVID.MGZ_Slot_Number, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nULD_MGZ_Strip_Slot_Num.ToString());
                Set_SVID((int)JSCK.eBSVID.MGZ_Port_Number, nGoodNg.ToString());

                // 2021.09.06 SungTae Start : [추가] Multi-LOT 관련
                if (CData.LotMgr.CheckExistLOT(sLotId) == false)
                {
                    m_pMgzNGCarrierList.ClearCarrier();
                    m_pRejectCarrierList.ClearCarrier();
                }
                // 2021.09.06 SungTae End

                m_pMgzNGCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                m_pRejectCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(sLotId);

                    CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Reject));
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Reject));
                }
                // 2021.08.25 SungTae End

                sMsgdata = $"OnCarrierEnded() Judgment = {JSCK.eMGZPort.NG.ToString()}";
            }

            Log_wt(sMsgdata);

            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Complete));
            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Reject));

            //---------------------
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.LOT_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Complete_CarrierID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
           // Set_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Map), sCarrierMap);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Table_Location), nTableLoc.ToString());
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            if (CData.IsMultiLOT())
                sCarrierStartTime = CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.GetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
            else
                sCarrierStartTime = CData.m_pHostLOT.GetStartTime(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
            // 2021.08.25 SungTae End

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Started_Time), sCarrierStartTime);

            sMsgdata = string.Format("Strip start time's strip id={0}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
            mgem.WriteUserLog(sMsgdata);
            Log_wt(sMsgdata);
            sMsgdata = string.Format("Strip start time ={0}", sCarrierStartTime);
            Log_wt(sMsgdata);

            DateTime now = DateTime.Now;
            sET = now.ToString("yyyyMMddHHmmss");
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_Ended_Time), sET);


            #region Measure Left Min,Max Avr Data
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            //-----------
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));

            if (nTableLoc == 2) // nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
            {
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.L_Before] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.L_Before] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.L_Before] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.L_After] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.L_After] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.L_After] = 0;
            }

            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.L_Before].ToString())); // before                            
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.L_Before].ToString())); // before
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.L_Before].ToString())); // before
            Log_wt(string.Format("0 ------- {0}, {1}, {2}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.L_Before],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.L_Before],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.L_Before]));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.L_After].ToString())); // after
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.L_After].ToString())); // after
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.L_After].ToString())); // after

            Log_wt(string.Format("1 ------- {0}, {1}, {2}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.L_After],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.L_After],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.L_After]));

            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            //-----------
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Work_Point));
            #endregion

            #region Measure Left Raw Data
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));// Before Data
            int i = 0;
            int nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[(int)EPoint.L_Before];//Left Befor Measure 측정 갯수 확인 필요            
            if ((nCk_Point == 0) || (nTableLoc == 2))// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
            {
                ftemp = 0;
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), ftemp);// Befor Data
            }
            else
            {
                for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.L_Before, i].ToString()));// Befor Data
                    sMsgdata = string.Format("left befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.L_Before, i].ToString()));
                    Log_wt(sMsgdata);
                }
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point)); // Aftre Data

            nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[(int)EPoint.L_After];// Left After Measure 측정 갯수 확인 필요            

            if ((nCk_Point == 0) || (nTableLoc == 2))// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
            {
                ftemp = 0;
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), ftemp);
            }
            else
            {
                for (i = 0; i < nCk_Point; i++)
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.L_After, i].ToString()));// After Data
                    sMsgdata = string.Format("left after ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.L_After, i].ToString()));
                    Log_wt(sMsgdata);
                }
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg1_Raw_Point));
            #endregion

            #region Measure Right Min,Max, Avr Data
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));

            CData.JSCK_Gem_Data[(int)EDataShift.GRL_BF_MEAS/*7*/].sSaBUN = "";

            if (nTableLoc == 1)
            {// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.R_Before] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.R_Before] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.R_Before] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.R_After] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.R_After] =
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.R_After] = 0;
            }
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.R_Before].ToString())); // before                
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.R_Before].ToString())); // before
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.R_Before].ToString())); // before
            Log_wt(string.Format("0 ++++++++++ {0}, {1}, {2}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.R_Before],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.R_Before],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.R_Before]));

            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.R_After].ToString())); // after
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.R_After].ToString())); // after
            mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.R_After].ToString())); // after
            Log_wt(string.Format("1 ++++++++++ {0}, {1}, {2}", CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Min_Data[(int)EPoint.R_After],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Max_Data[(int)EPoint.R_After],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Avr_Data[(int)EPoint.R_After]));

            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Work_Point));
            #endregion

            #region Measure Right Raw Data
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));

            nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[(int)EPoint.R_Before];// Right Befor Measure 측정 갯수 확인 필요                            

            if ((nCk_Point == 0) || (nTableLoc == 1))// nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
            {
                ftemp = 0;
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), ftemp);// Befor Data

            }
            else
            {
                for (i = 0; i < nCk_Point; i++)// Measure 측정 갯수 확인 필요
                {
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.R_Before, i].ToString()));// Befor Data
                    sMsgdata = string.Format("Right befor ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.R_Before, i].ToString()));
                    Log_wt(sMsgdata);
                }
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));

            nCk_Point = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].nMeasure_Count[(int)EPoint.R_After];// Right After Measure 측정 갯수 확인 필요            

            if ((nCk_Point == 0) || (nTableLoc == 1))   // nTableLoc = (1:Left, 2, Right , 3: L/R) Right 에서 작업을 했는지?
            {
                ftemp = 0;
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), ftemp);// Befor Data
            }
            else
            {
                for (i = 0; i < nCk_Point; i++)
                {
                    //Log_wt(string.Format("1 ++++++++++ {0}, {1}, {2}",loat.Parse(CData.JSCK_Gem_Data[14].fMeasure_Data[4, i].ToString() );
                    mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.R_After, i].ToString()));// After Data
                    sMsgdata = string.Format("Right After ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].fMeasure_Data[(int)EPoint.R_After, i].ToString()));
                    Log_wt(sMsgdata);
                }
            }

            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Carrier_Sg2_Raw_Point));
            #endregion

            #region Magazine Data

            if (nGoodNg == (int)JSCK.eMGZPort.GOOD)
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.GOOD_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.GOOD_Slot_ID), nSlotNo.ToString());
            }
            else if (nGoodNg == (int)JSCK.eMGZPort.NG)
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.NG_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.NG_Slot_ID), nSlotNo.ToString());
            }

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Port_Number), nGoodNg.ToString());
           

            #endregion

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Summary_Uploaded).ToString());
            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Ended).ToString());
            // 만약, lot의 마지막 carrier인 경우 lot end를 보고 후 Current Lotid를 공백으로 처리.

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            if (CData.IsMultiLOT())
            {
                // 2021.10.19 SungTae Start : [수정] Multi-LOT 관련
                sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                //for (i = 0; i < CData.LotMgr.ListLOTInfo.Count; i++)
                //{
                //    sgfrmMsg2.ShowGrid(CData.LotMgr.ListLOTInfo.ElementAt(i).m_pHostLOT, sLotId, i + 1);
                //}
                // 2021.10.19 SungTae End
            }
            else
                sgfrmMsg.ShowGrid(CData.m_pHostLOT);
            // 2021.08.25 SungTae End
        }
        #region //-- Wheel Left 두께 함수 20200731
        public void StdA_Wheel_Left_Thickness_Data_Set()
        {
            int i = 0;

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Left_Data_Set()  fMeasure_Wheel_Left_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[0, i].ToString("0.000")));
                //Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Left_Data_Set() fMeasure_Wheel_Left_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Left_Thick[1, i].ToString("0.000")));
               // Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_L_THICKNESS));
        }
        private void StdB_Wheel_Left_Thickness_Data_Set()
        {
            int i = 0;

            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Left_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Left_Data_Set()  fMeasure_Wheel_Left_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Left_Thick[0, i].ToString("0.000")));
                // CLog.Register(eLog.SECSGEM,sMsgdata);
                // thread 로그양 폭발
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Left_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Left_Data_Set() fMeasure_Wheel_Left_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Left_Thick[1, i].ToString("0.000")));
                // CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg1_Thickness));
        }
        #endregion
        
        #region//-- Wheel Right 두께 함수 20200731
        public void StdA_Wheel_Right_Thickness_Data_Set()
        {
            int i = 0;
            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Right_Data_Set() fMeasure_Wheel_Right_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[0, i].ToString("0.000")));
               // Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Right_Data_Set() fMeasure_Wheel_Right_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Wheel_Right_Thick[1, i].ToString("0.000")));
                //Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.WHEEL_R_THICKNESS));
        }
        public void StdB_Wheel_Right_Thickness_Data_Set()
        {
            int i = 0;
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Right_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Right_Data_Set() fMeasure_Wheel_Right_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Right_Thick[0, i].ToString("0.000")));
                // CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Right_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Wheel_Right_Data_Set() fMeasure_Wheel_Right_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Wheel_Right_Thick[1, i].ToString("0.000")));
                //CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Wheel_Sg2_Thickness));
        }
        #endregion//--
        #region//-- Dress Left 두께  함수 20200731
        public void StdA_Dress_Left_Thickness_Data_Set()
        {
            int i = 0;
            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Left_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Left_Data_Set() fMeasure_Dress_Left_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Left_Thick[0, i].ToString("0.000")));
                //Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Left_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Left_Data_Set() fMeasure_Dress_Left_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Left_Thick[1, i].ToString("0.000")));
                //Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_L_THICKNESS));
        }
        private void StdB_Dress_Left_Thickness_Data_Set()
        {
            int i = 0;
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Left_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Left_Data_Set() fMeasure_Dress_Left_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Left_Thick[0, i].ToString("0.000")));
                //CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Left_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Left_Data_Set() fMeasure_Dress_Left_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Left_Thick[1, i].ToString("0.000")));
                //CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg1_Thickness));
        }
        #endregion//--

        #region//-- Dress Right 두께  함수 20200731
        public void StdA_Dress_Right_Thickness_Data_Set()
        {
            int i = 0;
            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Right_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Right_Data_Set() fMeasure_Dress_Right_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Right_Thick[0, i].ToString()));
                //Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Right_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Right_Data_Set() fMeasure_Dress_Right_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].fMeasure_Dress_Right_Thick[1, i].ToString()));
                //Log_wt(sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.DRESS_R_THICKNESS));
        }
        private void StdB_Dress_Right_Thickness_Data_Set()
        {
            int i = 0;
            mgem.CloseMsg(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));// Before Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Right_Thick[0, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Right_Data_Set() fMeasure_Dress_Right_Thick Before ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Right_Thick[0, i].ToString()));
                //CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness)); // Aftre Data
            for (i = 0; i < 4; i++)// Measure 측정 갯수 확인 필요
            {
                mgem.AddF4Item(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness), float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Right_Thick[1, i].ToString("0.000")));
                sMsgdata = string.Format("Dress_Right_Data_Set() fMeasure_Dress_Right_Thick Aftere ={0},{1}", i, float.Parse(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK].fMeasure_Dress_Right_Thick[1, i].ToString()));
                //CLog.Register(eLog.SECSGEM,sMsgdata);
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eBSVID.Dress_Sg2_Thickness));
        }
        #endregion//--

        /// <summary>
        /// 999204    
        /// </summary>
        /// <param name="JSCK_Gem_Data[20].sLot_Name"></param>
        /// <param name="sCarrierId"></param>
        /// <param name="nSlotNo"></param>
        /// <param name="nTableLoc"></param>
        /// <param name="nGoodNg"></param>
        public void OnCarrierUnloaded(string sLotId, string sCarrierId)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    OnCarrierUnloaded(sLotId, sCarrierId);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnCarrierUnloaded(sLotId, sCarrierId); }
                else { StdB_OnCarrierUnloaded(sLotId, sCarrierId); }
            }
        }
        public void StdA_OnCarrierUnloaded(string sLotId, string sCarrierId)
        {
            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- s5");
            //        OnCarrierUnloaded(sLotId, sCarrierId);
            //    });
            //}
            //else
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                // 2021.08.27 SungTae Start : [수정]
                //Log_wt(string.Format("OnCarrierUnloaded({0},{1})", sLotId, sCarrierId));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1})  LOT = {2}, Carrier ID = {3}",
                                                                Convert.ToInt32(JSCK.eCEID.Carrier_Unloaded),
                                                                JSCK.eCEID.Carrier_Unloaded.ToString(),
                                                                sLotId, sCarrierId));
                // 2021.08.27 SungTae End

                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name = sLotId;

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID),        CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID),    CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Unloaded).ToString());
            }
        }
        public void StdB_OnCarrierUnloaded(string sLotId, string sCarrierId)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            // 2021.08.27 SungTae Start : [수정]
            //Log_wt(string.Format("OnCarrierUnloaded({0},{1})", sLotId, sCarrierId));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1})  LOT = {2}, Carrier ID = {3}",
                                                            Convert.ToInt32(JSCK.eBCEID.Carrier_Unloaded),
                                                            JSCK.eBCEID.Carrier_Unloaded.ToString(),
                                                            sLotId, sCarrierId));
            // 2021.08.27 SungTae End

            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name = sLotId;

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.LOT_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Complete_CarrierID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sInRail_Verified_Strip_ID);

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Carrier_Unloaded).ToString());

        }

        /// <summary>
        /// CEID 999403    MGZ Ended
        /// </summary>
        /// <param name="sMgzId"></param>
        /// <param name="nMGZPort"></param>
        public void OnMagazineEnd(uint nMGZPort)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnMagazineEnd(nMGZPort);

                });

            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnMagazineEnd(nMGZPort); }
                else { StdB_OnMagazineEnd(nMGZPort); }
            }
            
        }
        public void StdA_OnMagazineEnd(uint nMGZPort)
        {

            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnMagazineEnd(nMGZPort);

            //    });

            //}
            //else
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :
                                                 // bool bTrue = true; // Mgz carrier list에 CarrierId, LotId , slot no를 넣을지 체크.

                string sMGZPort = nMGZPort.ToString();
                string sMGZId = string.Empty;

                // 2021.08.30 SungTae Start : [수정] Multi-LOT 관련
                //Log_wt(string.Format("OnMagazineEnd({0}) MgzID:{1}/{2}", nMGZPort, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New));
                Log_wt(string.Format("[SEND](H < -E) S6F11 CEID = {0}({1}).  Mgz Port : {2}, MgzID : {3}/{4}",
                                                                Convert.ToInt32(JSCK.eCEID.Magazine_Ended),
                                                                JSCK.eCEID.Magazine_Ended.ToString(),
                                                                (nMGZPort == 1) ? JSCK.eMGZPort.GOOD.ToString() : JSCK.eMGZPort.NG.ToString(),
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID,
                                                                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New));


                if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.GOOD))
                {
                    sMGZId = m_pMgzGoodCarrierList.m_strMgzId;
                }
                else if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.NG))
                {
                    sMGZId = m_pMgzNGCarrierList.m_strMgzId;
                }
                // 2021.08.30 SungTae End

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), sMGZPort);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Slot_Number), (0).ToString());

                //----------------- MAGAZINE CARRIER LIST ---------------------------------------------------------
                /*
                ASE-Kr  Exception 발생 문제 추정 지점 분석 =>  쓰레드 호출로 예상됩니다.
                Mgz End이벤트시 Strip List를 만들기만 하고 사용하지 않는 상태입니다. (2020 10/9)
                */

                // 2022.04.13 SungTae Start : [수정] ASE-KH의 경우 Magazine End 시 해당 MGZ ID와 Carrier List를 보고해야 해서 SITE 옵션으로 구분
                if (CData.CurCompany == ECompany.ASE_K12)
                {
                    mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List));
                    mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List));

                    string strTmpList = "";

                    if (nMGZPort == (int)JSCK.eMGZPort.GOOD)
                    {
                        //sMsgdata = string.Format("OnMagazineEnd() nMGZPort == (int)JSCK.eMGZPort.GOOD)");
                        sMsgdata = string.Format($"OnMagazineEnd()  nMGZPort : {JSCK.eMGZPort.GOOD}, Total Count : {m_pMgzGoodCarrierList.Count})");

                        for (int i = 0; i < m_pMgzGoodCarrierList.Count; i++)
                        {
                            string strTmp = m_pMgzGoodCarrierList.GetCarrierID(i);
                            
                            strTmpList += $"\r\nNo : {i:00}, Strip ID : {strTmp}";

                            mgem.AddAsciiItem(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List), strTmp, strTmp.Length);
                        }
                    }
                    else if (nMGZPort == (int)JSCK.eMGZPort.NG)
                    {
                        //sMsgdata = string.Format("OnMagazineEnd() nMGZPort == (int)JSCK.eMGZPort.NG)");
                        sMsgdata = string.Format($"OnMagazineEnd()  nMGZPort : {JSCK.eMGZPort.NG}, Total Count : {m_pMgzNGCarrierList.Count})");

                        for (int i = 0; i < m_pMgzNGCarrierList.Count; i++)
                        {
                            string strTmp = m_pMgzNGCarrierList.GetCarrierID(i);

                            strTmpList += $"\r\nNo : {i:00}, Strip ID : {strTmp}";

                            mgem.AddAsciiItem(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List), strTmp, strTmp.Length);
                        }
                    }

                    sMsgdata += strTmpList;

                    Log_wt(sMsgdata);
                    mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.MGZ_Carrier_List));
                }
                // 2022.04.13 SungTae End
                //-------------------------------------------------------------------------------------------------


                // 2020-11-13, jhLee, LOT End시 Magazine ID가 누락되는 문제, Magazine End시 LotEnd로 데이터를 이동시켜준다.
                // Magazine Port를 새로이 만들어서 대입해준다.

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Ended).ToString());

                CData.GemForm.Strip_Data_Shift((int)EDataShift.OFL_STRIP_OUT/*23*/, (int)EDataShift.LOT_END/*25*/);// PickUp Data  -> Dry Data 로 이동
                CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].nULD_MGZ_Port = (int)nMGZPort;    // 어느 Port로 지정되었는지 지정

                if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.GOOD))
                {
                    m_pMgzGoodCarrierList.ClearCarrier();

                    //sMsgdata = string.Format("OnMagazineEnd() nMGZPort == Convert.ToInt32(JSCK.eMGZPort.GOOD), MgzID=" + CData.JSCK_Gem_Data[25].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[25].sULD_MGZ_ID_New);
                    sMsgdata = string.Format("OnMagazineEnd() nMGZPort = GOOD, MgzID=" + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New);
                }
                else if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.NG))
                {
                    m_pMgzNGCarrierList.ClearCarrier();

                    //sMsgdata = string.Format("OnMagazineEnd() nMGZPort == Convert.ToInt32(JSCK.eMGZPort.NG), MgzID=" + CData.JSCK_Gem_Data[25].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[25].sULD_MGZ_ID_New);
                    sMsgdata = string.Format("OnMagazineEnd() nMGZPort = NG, MgzID=" + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New);
                }

                Log_wt(sMsgdata);
            }
        }
        public void StdB_OnMagazineEnd(uint nMGZPort)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :
                                             // bool bTrue = true; // Mgz carrier list에 CarrierId, LotId , slot no를 넣을지 체크.

            string sMGZPort = nMGZPort.ToString();
            string sMGZId = string.Empty;

            // 2021.08.30 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("OnMagazineEnd({0}) MgzID:{1}/{2}", nMGZPort, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID, CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New));
            Log_wt(string.Format("[SEND](H < -E) S6F11 CEID = {0}({1}).  Mgz Port : {2}, MgzID : {3}/{4}",
                                                            Convert.ToInt32(JSCK.eBCEID.Magazine_Ended),
                                                            JSCK.eBCEID.Magazine_Ended.ToString(),
                                                            (nMGZPort == 1) ? JSCK.eMGZPort.GOOD.ToString() : JSCK.eMGZPort.NG.ToString(),
                                                            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID,
                                                            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New));


            if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.GOOD))
            {
                sMGZId = m_pMgzGoodCarrierList.m_strMgzId;
            }
            else if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.NG))
            {
                sMGZId = m_pMgzNGCarrierList.m_strMgzId;
            }
            // 2021.08.30 SungTae End



            // 2020-11-13, jhLee, LOT End시 Magazine ID가 누락되는 문제, Magazine End시 LotEnd로 데이터를 이동시켜준다.
            // Magazine Port를 새로이 만들어서 대입해준다.

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Port_Number), sMGZPort);

            CData.GemForm.Strip_Data_Shift((int)EDataShift.OFL_STRIP_OUT/*23*/, (int)EDataShift.LOT_END/*25*/);// PickUp Data  -> Dry Data 로 이동
            CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].nULD_MGZ_Port = (int)nMGZPort;    // 어느 Port로 지정되었는지 지정
            if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.LOADER))
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.LD_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLD_MGZ_ID_New);
                
               // Set_SVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Slot_Number), (0).ToString());
            }
            else if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.GOOD))
            {
                m_pMgzGoodCarrierList.ClearCarrier();
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.GOOD_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                //sMsgdata = string.Format("OnMagazineEnd() nMGZPort == Convert.ToInt32(JSCK.eMGZPort.GOOD), MgzID=" + CData.JSCK_Gem_Data[25].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[25].sULD_MGZ_ID_New);
                sMsgdata = string.Format("OnMagazineEnd() nMGZPort = GOOD, MgzID=" + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New);
            }
            else if (nMGZPort == Convert.ToInt32(JSCK.eMGZPort.NG))
            {
                m_pMgzNGCarrierList.ClearCarrier();
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.NG_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                //sMsgdata = string.Format("OnMagazineEnd() nMGZPort == Convert.ToInt32(JSCK.eMGZPort.NG), MgzID=" + CData.JSCK_Gem_Data[25].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[25].sULD_MGZ_ID_New);
                sMsgdata = string.Format("OnMagazineEnd() nMGZPort = NG, MgzID=" + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID + " / " + CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New);
            }

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Ended).ToString());
            Log_wt(sMsgdata);

        }


        /// <summary>
        /// CEID 999400   Loader MGZ Verify Request
        /// </summary>
        /// <param name="sMgzId"></param>
        /// <param name="nMgzPort"></param>
        public void LoaderMgzVerifyRequest(string sMgzId, uint nMgzPort)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    LoaderMgzVerifyRequest(sMgzId, nMgzPort);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_LoaderMgzVerifyRequest(sMgzId, nMgzPort); }
                else { StdB_LoaderMgzVerifyRequest(sMgzId, nMgzPort); }
            }

        }
        public void StdA_LoaderMgzVerifyRequest(string sMgzId, uint nMgzPort)
        {
            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        LoaderMgzVerifyRequest(sMgzId, nMgzPort);
            //    });
            //}
            //else
            {
                CData.SecsGem_Data.nLdMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Run)/*1*/;// Loader MGZ 시퀸스 초기화        
                
                if (!CData.Opt.bSecsUse) {
                    CData.SecsGem_Data.nLdMGZ_Check = 3;  // Loader MGZ 시퀸스 계속 진행
                    return; 
                }

                // 2021.08.30 SungTae Start : [수정]
                //Log_wt(string.Format("LoaderMgzVerifyRequest({0},{1})", sMgzId, nMgzPort));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  MgzID : {2}, Mgz Port : {3} (LOADER)",
                                                                Convert.ToInt32(JSCK.eCEID.Magazine_Verify_Request),
                                                                JSCK.eCEID.Magazine_Verify_Request.ToString(),
                                                                sMgzId, nMgzPort.ToString()));
                // 2021.08.30 SungTae End

                //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New = sMgzId;
				CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID_New = sMgzId;
                
                //Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID),       CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID)	,   CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID_New);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number),   nMgzPort.ToString());
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verify_Request).ToString());

                m_sRMGZID3 = sMgzId; 
            }

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
            {// 20200718 Local Mode 사용
                CData.SecsGem_Data.nMGZ_Check = 3;  // Loader MGZ 시퀸스 계속 진행
                Local_OnS2F41_Set(GEM.sRCMD_MGZ_VERIFICATION, nMgzPort.ToString());
            }
        }
        public void StdB_LoaderMgzVerifyRequest(string sMgzId, uint nMgzPort)
        {
            CData.SecsGem_Data.nLdMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Run)/*1*/;// Loader MGZ 시퀸스 초기화        

            if (!CData.Opt.bSecsUse)
            {
                CData.SecsGem_Data.nLdMGZ_Check = 3;  // Loader MGZ 시퀸스 계속 진행
                return;
            }

            // 2021.08.30 SungTae Start : [수정]
            //Log_wt(string.Format("LoaderMgzVerifyRequest({0},{1})", sMgzId, nMgzPort));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  MgzID : {2}, Mgz Port : {3} (LOADER)",
                                                            Convert.ToInt32(JSCK.eBCEID.Magazine_Verify_Request),
                                                            JSCK.eBCEID.Magazine_Verify_Request.ToString(),
                                                            sMgzId, nMgzPort.ToString()));
            // 2021.08.30 SungTae End

            //CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New = sMgzId;
            CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID_New = sMgzId;

            //Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID),       CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Reading_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID_New);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Port_Number), nMgzPort.ToString());

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Verify_Request).ToString());

            m_sRMGZID3 = sMgzId;


            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
            {// 20200718 Local Mode 사용
                CData.SecsGem_Data.nMGZ_Check = 3;  // Loader MGZ 시퀸스 계속 진행
                Local_OnS2F41_Set(GEM.sRCMD_MGZ_VERIFICATION, nMgzPort.ToString());
            }
        }

        /// <summary>
        /// CEID 999400  Unloader MGZ Verify Request
        /// </summary>
        /// <param name="sMgzId"></param>
        /// <param name="nMgzPort"></param>
        
        public void UnloaderMgzVerifyRequest(string sMgzId, uint nMgzPort) 
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    UnloaderMgzVerifyRequest(sMgzId, nMgzPort);

                });

            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_UnloaderMgzVerifyRequest(sMgzId, nMgzPort); }
                else { StdB_UnloaderMgzVerifyRequest(sMgzId, nMgzPort); }
            }
        }
        public void StdA_UnloaderMgzVerifyRequest(string sMgzId, uint nMgzPort)
        {

            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnMgzVerifyRequest(sMgzId, nMgzPort);

            //    });

            //}
            //else
            {
                CData.SecsGem_Data.nMGZ_Check = 1;// Unloader MGZ 시퀸스 초기화        

                if (!CData.Opt.bSecsUse) //191031 ksg :
                {
                    CData.SecsGem_Data.nMGZ_Check = 3;  // UnLoader MGZ 시퀸스 계속 진행
                    return; //191031 ksg :
                }

                // 2021.08.30 SungTae Start : [수정]
                //Log_wt(string.Format("OnMgzVerifyRequest({0},{1})", sMgzId, nMgzPort));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  MgzID : {2}, Mgz Port : {3}",
                                                                Convert.ToInt32(JSCK.eCEID.Magazine_Verify_Request),
                                                                JSCK.eCEID.Magazine_Verify_Request.ToString(),
                                                                sMgzId, nMgzPort.ToString()));
                // 2021.08.30 SungTae End
                CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New = sMgzId;

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID)    , CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), nMgzPort.ToString());
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verify_Request).ToString());

                if      (nMgzPort.ToString() == "1")    { m_sRMGZID1 = sMgzId; }
                else if (nMgzPort.ToString() == "2")    { m_sRMGZID2 = sMgzId; }
                else if (nMgzPort.ToString() == "3")    { m_sRMGZID3 = sMgzId; }

                if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
                {// 20200718 Local Mode 사용
                    CData.SecsGem_Data.nMGZ_Check = 3;  // UnLoader MGZ 시퀸스 계속 진행
                    Local_OnS2F41_Set(GEM.sRCMD_MGZ_VERIFICATION, nMgzPort.ToString());
                }                
            }
        }
        public void StdB_UnloaderMgzVerifyRequest(string sMgzId, uint nMgzPort)
        {

            CData.SecsGem_Data.nMGZ_Check = 1;// Unloader MGZ 시퀸스 초기화        

            if (!CData.Opt.bSecsUse) //191031 ksg :
            {
                CData.SecsGem_Data.nMGZ_Check = 3;  // UnLoader MGZ 시퀸스 계속 진행
                return; //191031 ksg :
            }

            // 2021.08.30 SungTae Start : [수정]
            //Log_wt(string.Format("OnMgzVerifyRequest({0},{1})", sMgzId, nMgzPort));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  MgzID : {2}, Mgz Port : {3}",
                                                            Convert.ToInt32(JSCK.eBCEID.Magazine_Verify_Request),
                                                            JSCK.eBCEID.Magazine_Verify_Request.ToString(),
                                                            sMgzId, nMgzPort.ToString()));
            // 2021.08.30 SungTae End

            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New = sMgzId;

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Reading_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Port_Number), nMgzPort.ToString());

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Magazine_Verify_Request).ToString());

            if (nMgzPort.ToString() == "1") { m_sRMGZID1 = sMgzId; }
            else if (nMgzPort.ToString() == "2") { m_sRMGZID2 = sMgzId; }
            else if (nMgzPort.ToString() == "3") { m_sRMGZID3 = sMgzId; }

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
            {// 20200718 Local Mode 사용
                CData.SecsGem_Data.nMGZ_Check = 3;  // UnLoader MGZ 시퀸스 계속 진행
                Local_OnS2F41_Set(GEM.sRCMD_MGZ_VERIFICATION, nMgzPort.ToString());
            }

        }

        /// <summary>
        /// CEID 999103    LOT Ended
        /// </summary>
        /// <param name="sLotId"></param>
        /// <param name="sTUCnt"></param>
        /// <param name="sPUCnt"></param>
        public void OnLotEnded(string sLotId, string sTUCnt, string sPUCnt)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnLotEnded(sLotId, sTUCnt, sPUCnt);

                });

            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnLotEnded(sLotId, sTUCnt, sPUCnt); }
                else { StdB_OnLotEnded(sLotId, sTUCnt, sPUCnt); }
            }
        }
        public void StdA_OnLotEnded(string sLotId, string sTUCnt, string sPUCnt)
        {

            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnLotEnded(sLotId, sTUCnt, sPUCnt);

            //    });

            //}
            //else
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                int nIdx = -1;                          // 2021.08.31 SungTae : [추가] Multi-LOT 관련
                //int iCnt = CData.m_pHostLOT.Count;    // 2021.08.10 SungTae : [삭제] 미사용 코드로 추후 완전 삭제 예정
                string sStripId = string.Empty;

                // 2021.08.27 SungTae Start : [수정] 
                //Log_wt(string.Format("OnLotEnded({0},{1},{2}) MgzID={3} / {4}, Port={5}", sLotId, sTUCnt, sPUCnt, 
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1})  LOT = {2} (In : {3}, Out : {4}) MgzID={5} / {6}, Port={7}",
                                                                Convert.ToInt32(JSCK.eCEID.LOT_Ended),
                                                                JSCK.eCEID.LOT_Ended.ToString(),
                                                                sLotId, sTUCnt, sPUCnt,
                                                                CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID,
                                                                CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New,
                                                                CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].nULD_MGZ_Port));
                // 2021.08.27 SungTae Start End

                int nTotalCount = 0;

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    // 2022.05.18 SungTae Start : [수정] ASE-KR Multi-LOT 관련
                    // 정상적으로 LOT END 후 작업자가 습관적으로 "MULTI-LOT END" 버튼 클릭 시 강제 종료되는 현상이 확인되어 조건을 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0)
                    {
                        nIdx = CData.LotMgr.GetListIndexNum(sLotId);

                        nTotalCount = CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count;
                    }
                    else
                    {
                        Log_wt($"[EXCEPTION] OnLotEnded() >>> LOT List Count = {CData.LotMgr.ListLOTInfo.Count}");
                        return;
                    }
                    // 2022.05.18 SungTae End
                }
                else
                {
                    nTotalCount = CData.m_pHostLOT.Count;
                }
                // 2021.08.25 SungTae End

                Log_wt(string.Format("OnLotEnded(Total_Carrier_List = {0})", nTotalCount));

                int nProcCount = m_pEndedCarrierList.Count;

                Log_wt(string.Format("OnLotEnded(Proceed_Carrier_List = {0})", nProcCount));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Total_Unit_Count), nTotalCount.ToString() );    //HOST에서 보내 온 랏의 전체 스트립갯수
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Proceed_Unit_Count), nProcCount.ToString() );   //작업 완료(GOOD)한 랏의 스트립갯수
                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_Started_Time), m_strLotTime );

                m_strLotTime = string.Empty;
       
                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));
                Make_SVID(Convert.ToInt32(JSCK.eSVID.Proceed_Carrier_List));
                Make_SVID(Convert.ToInt32(JSCK.eSVID.Total_Carrier_List));

                string sTime = string.Empty;
                DateTime now = DateTime.Now;
                sTime = now.ToString("yyyyMMddHHmmss");

                // 2020-11-13, jhLee, SPIL에서 CEID 999103 (LOT End) 전송시 SVID 999401인 Magazine ID의 값이 공백(널)로 보고되는 문제 수정

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID)    , CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New);              // SVID 999401
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].nULD_MGZ_Port.ToString());  // SVID 999404
                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_Ended_Time) , sTime);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Ended).ToString());

                // 2021-06-23, jhLee, SPIC VOC, LOT end 보고 후 현재 LOT ID는 이전 LOT ID로 치환시키고 공백으로 Reset 한다.
                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID)         , CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sLot_Name);        
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID) , "");       

                m_nLotOpenCount--;                    // Lot Open이 진행되어 처리중인가 ? START/RESTART 구분용, 진행중인 LOT 수량 0이면 아직 없음
                CData.OpenLotCnt--;     // 2021.08.19 SungTae : [추가]

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                     CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.ClearCarrier();
                }
                else
                {
                    CData.m_pHostLOT.ClearCarrier();
                }
                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련

                m_pStartCarrierList.ClearCarrier(); 
                m_pEndedCarrierList.ClearCarrier();
                m_pReadCarrierList.ClearCarrier();
                m_pRejectCarrierList.ClearCarrier();
                m_pValidCarrierList.ClearCarrier();

                MakeZeroList();

                // 만약, lot의 마지막 carrier인 경우 lot end를 보고 후 Current Lotid를 공백으로 처리.
                Log_wt(string.Format("Lot End 시에 지우지 않고 신규 Lot 입력시에 초기화 함, nLotOpenCount:{0}", m_nLotOpenCount));
                //sgfrmMsg.ShowGrid(CData.m_pHostLOT); //Lot End 시에 지우지 않고 신규 Lot 입력시에 초기화 함
            }
        }
        public void StdB_OnLotEnded(string sLotId, string sTUCnt, string sPUCnt)
        {
            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            int nIdx = -1;                          // 2021.08.31 SungTae : [추가] Multi-LOT 관련
                                                    //int iCnt = CData.m_pHostLOT.Count;    // 2021.08.10 SungTae : [삭제] 미사용 코드로 추후 완전 삭제 예정
            string sStripId = string.Empty;

            // 2021.08.27 SungTae Start : [수정] 
            //Log_wt(string.Format("OnLotEnded({0},{1},{2}) MgzID={3} / {4}, Port={5}", sLotId, sTUCnt, sPUCnt, 
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1})  LOT = {2} (In : {3}, Out : {4}) MgzID={5} / {6}, Port={7}",
                                                            Convert.ToInt32(JSCK.eBCEID.Lot_Ended),
                                                            JSCK.eBCEID.Lot_Ended.ToString(),
                                                            sLotId, sTUCnt, sPUCnt,
                                                            CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID,
                                                            CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New,
                                                            CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].nULD_MGZ_Port));
            // 2021.08.27 SungTae Start End

            int nTotalCount = 0;

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            if (CData.IsMultiLOT())
            {
                // 2022.05.18 SungTae Start : [수정] ASE-KR Multi-LOT 관련
                // 정상적으로 LOT END 후 작업자가 습관적으로 "MULTI-LOT END" 버튼 클릭 시 강제 종료되는 현상이 확인되어 조건을 추가
                if (CData.LotMgr.ListLOTInfo.Count > 0)
                {
                    nIdx = CData.LotMgr.GetListIndexNum(sLotId);

                    nTotalCount = CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count;
                }
                else
                {
                    Log_wt($"[EXCEPTION] OnLotEnded() >>> LOT List Count = {CData.LotMgr.ListLOTInfo.Count}");
                    return;
                }
                // 2022.05.18 SungTae End
            }
            else
            {
                nTotalCount = CData.m_pHostLOT.Count;
            }
            // 2021.08.25 SungTae End

            Log_wt(string.Format("OnLotEnded(Total_Carrier_List = {0})", nTotalCount));

            int nProcCount = m_pEndedCarrierList.Count;

            Log_wt(string.Format("OnLotEnded(Proceed_Carrier_List = {0})", nProcCount));

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Total_Unit_Count), nTotalCount.ToString());    //HOST에서 보내 온 랏의 전체 스트립갯수
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Proceed_Unit_Count), nProcCount.ToString());   //작업 완료(GOOD)한 랏의 스트립갯수
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Lot_Started_Time), m_strLotTime);

            m_strLotTime = string.Empty;

            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Carrier_List_Complete));
            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Proceed_Carrier_List));
            Make_SVID(Convert.ToInt32(JSCK.eBSVID.Total_Carrier_List));

            string sTime = string.Empty;
            DateTime now = DateTime.Now;
            sTime = now.ToString("yyyyMMddHHmmss");

            // 2020-11-13, jhLee, SPIL에서 CEID 999103 (LOT End) 전송시 SVID 999401인 Magazine ID의 값이 공백(널)로 보고되는 문제 수정

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.GOOD_Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sULD_MGZ_ID_New);              // SVID 999401
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.MGZ_Port_Number), CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].nULD_MGZ_Port.ToString());  // SVID 999404
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Lot_Ended_Time), sTime);

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Lot_Ended).ToString());

            // 2021-06-23, jhLee, SPIC VOC, LOT end 보고 후 현재 LOT ID는 이전 LOT ID로 치환시키고 공백으로 Reset 한다.
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.LOT_ID), CData.JSCK_Gem_Data[(int)EDataShift.LOT_END/*25*/].sLot_Name);
            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Current_Lot_ID), "");

            m_nLotOpenCount--;                    // Lot Open이 진행되어 처리중인가 ? START/RESTART 구분용, 진행중인 LOT 수량 0이면 아직 없음
            CData.OpenLotCnt--;     // 2021.08.19 SungTae : [추가]

            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
            if (CData.IsMultiLOT())
            {
                CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.ClearCarrier();
            }
            else
            {
                CData.m_pHostLOT.ClearCarrier();
            }
            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련

            m_pStartCarrierList.ClearCarrier();
            m_pEndedCarrierList.ClearCarrier();
            m_pReadCarrierList.ClearCarrier();
            m_pRejectCarrierList.ClearCarrier();
            m_pValidCarrierList.ClearCarrier();

            MakeZeroList();

            // 만약, lot의 마지막 carrier인 경우 lot end를 보고 후 Current Lotid를 공백으로 처리.
            Log_wt(string.Format("Lot End 시에 지우지 않고 신규 Lot 입력시에 초기화 함, nLotOpenCount:{0}", m_nLotOpenCount));
            //sgfrmMsg.ShowGrid(CData.m_pHostLOT); //Lot End 시에 지우지 않고 신규 Lot 입력시에 초기화 함
        }
        #region 
        /// <summary>
        /// CEID 999601    Material Verify Request
        /// </summary>
        /// <param name="sMatId"></param> 신규 Dress ID
        /// <param name="nLoc"></param> 신규 등록 하고자 하는 Dress 번호(왼 - 1, 오 - 2)
        /// 
        private string sToolLoc;
        public void OnMatVerifyRequest( string sMatId, uint nLoc)
        {
            sToolLoc = "0";
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- a0");
                    OnMatVerifyRequest(sMatId, nLoc);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnMatVerifyRequest(sMatId, nLoc); }
                else { StdB_OnMatVerifyRequest(sMatId, nLoc); }
            }
        }
        public void StdA_OnMatVerifyRequest(string sMatId, uint nLoc)
        {
            sToolLoc = "0";
            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- a0");
            //        OnMatVerifyRequest(sMatId, nLoc);
            //    });
            //}
            //else
            {
                if (!CData.Opt.bSecsUse) return; //191031 ksg :   

                sToolLoc = nLoc.ToString();

                // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
                //Log_wt(string.Format("OnMatVerifyRequest({0},{1})", sMatId, nLoc));                
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Mat ID = {2},  Table Loc. : {3}",
                                                                Convert.ToInt32(JSCK.eCEID.Material_Verify_Request),
                                                                JSCK.eCEID.Material_Verify_Request.ToString(),
                                                                sMatId, nLoc));
                // 2021.09.01 SungTae End

                if (nLoc == (int)JSCK.eTableLoc.N1)
                {
                    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[0] = sMatId;

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[0]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), "");                    
                }
                else if (nLoc == (int)JSCK.eTableLoc.N2)
                {
                    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[1] = sMatId;

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), "");
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[1]);
                }

                Set_SVID (Convert.ToInt32(JSCK.eSVID.MATL_Loc), sToolLoc);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Verify_Request).ToString());
            }
            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
            {// 20200718 Local Mode 사용
                Local_OnS2F41_Set(GEM.sRCMD_MAT_VERIFICATION, sToolLoc);
            }
        }
        public void StdB_OnMatVerifyRequest(string sMatId, uint nLoc)
        {
            sToolLoc = "0";

            if (!CData.Opt.bSecsUse) return; //191031 ksg :   

            sToolLoc = nLoc.ToString();

            // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("OnMatVerifyRequest({0},{1})", sMatId, nLoc));                
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Mat ID = {2},  Table Loc. : {3}",
                                                            Convert.ToInt32(JSCK.eBCEID.Dresser_Verify_Request),
                                                            JSCK.eBCEID.Dresser_Verify_Request.ToString(),
                                                            sMatId, nLoc));
            // 2021.09.01 SungTae End

            if (nLoc == (int)JSCK.eTableLoc.N1)
            {
                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[0] = sMatId;

                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg1_DressID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg2_DressID), "");
            }
            else if (nLoc == (int)JSCK.eTableLoc.N2)
            {
                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[1] = sMatId;

                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg1_DressID), "");
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg2_DressID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[1]);
            }

            Set_SVID(Convert.ToInt32(JSCK.eBSVID.Dress_Loc), sToolLoc);

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Dresser_Verify_Request).ToString());

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
            {// 20200718 Local Mode 사용
                Local_OnS2F41_Set(GEM.sRCMD_MAT_VERIFICATION, sToolLoc);
            }
        }
        #endregion

        public void MakeZeroList()
        {
            Log_wt(string.Format("Lot End 시에 각 LIST SVID의 Carrier List를 초기화 함."));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Read));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Started));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Complete));

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_List_Reject));
        }
        
        public void ToolSerial_Number_Set(string sSerial_Num, uint nLoc)
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_ToolSerial_Number_Set(sSerial_Num, nLoc); }
            else { StdB_ToolSerial_Number_Set(sSerial_Num, nLoc); }
        }
        public void StdA_ToolSerial_Number_Set(string sSerial_Num, uint nLoc)
        {
            Log_wt(string.Format("00_OnToolSerial_Number_Set({0},{1})", nLoc, sSerial_Num));

            if (nLoc == (int)JSCK.eTableLoc.N1)
            {
                CData.LotInfo.sLTool_Serial_Num = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0] = sSerial_Num;

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_L_Serial_Num), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
            }
            else
            {
                CData.LotInfo.sRTool_Serial_Num = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1] = sSerial_Num;

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_R_Serial_Num), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
            }
        }
        public void StdB_ToolSerial_Number_Set(string sSerial_Num, uint nLoc)
        {
            Log_wt(string.Format("00_OnToolSerial_Number_Set({0},{1})", nLoc, sSerial_Num));

            if (nLoc == (int)JSCK.eTableLoc.N1)
            {
                CData.LotInfo.sLTool_Serial_Num = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0] = sSerial_Num;

                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_1), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
            }
            else
            {
                CData.LotInfo.sRTool_Serial_Num = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1] = sSerial_Num;

                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_2), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
            }
        }

        public void ToolSerial_Number_Set(uint nLoc)
        {
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                StdA_ToolSerial_Number_Set(nLoc);
            }
            else
            {
                StdB_ToolSerial_Number_Set(nLoc);
            }
        }
        public void StdA_ToolSerial_Number_Set(uint nLoc)
        {
            Log_wt(string.Format("01_OnToolSerial_Number_Set({0}, {1},{2})", nLoc, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0], CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]));
            
            if (nLoc == (int)JSCK.eTableLoc.N1)
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_L_Serial_Num), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_R_Serial_Num), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
            }
        }
        public void StdB_ToolSerial_Number_Set(uint nLoc)
        {
            Log_wt(string.Format("01_OnToolSerial_Number_Set({0}, {1},{2})", nLoc, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0], CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]));

            if (nLoc == (int)JSCK.eTableLoc.N1)
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_1), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
            }
            else
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_2), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
            }
        }

        /// <summary>
        /// CEID 999701    Tool Verify Request
        /// </summary>
        /// <param name="sToolId"></param>
        /// <param name="nLoc"></param>
        public void OnToolVerifyRequest(string sToolId, uint nLoc)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    OnToolVerifyRequest(sToolId, nLoc);
                });
            }
            else
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { StdA_OnToolVerifyRequest(sToolId, nLoc); }
                else { StdB_OnToolVerifyRequest(sToolId, nLoc); }
            }
        }
        public void StdA_OnToolVerifyRequest(string sToolId, uint nLoc)
        {

            //if (listLog.InvokeRequired)
            //{
            //    Invoke((MethodInvoker)delegate ()
            //    {
            //        Console.WriteLine("---- s5");
            //        OnToolVerifyRequest(sToolId, nLoc);
            //    });
            //}
            //else
            {
                string sToolLoc = "";

                if (!CData.Opt.bSecsUse) return; //191031 ksg :

                // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
                //Log_wt(string.Format("OnToolVerifyRequest({0},{1},{2},{3})", sToolId, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0], CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1], nLoc));             
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Tool ID = {2},  Table Loc. : {3}, Current Tool-1 ID : {4}, Current Tool-2 ID : {5}",
                                                                Convert.ToInt32(JSCK.eCEID.Tool_Verify_Request),
                                                                JSCK.eCEID.Tool_Verify_Request.ToString(),
                                                                sToolId, nLoc,
                                                                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0],
                                                                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]));
                // 2021.09.01 SungTae End

                sToolLoc = nLoc.ToString();
                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0] = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1] =  sToolId; // [0]=Left, [1]=Right

                if (nLoc == (int)JSCK.eTableLoc.N1)
                {
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID)          , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID)          , "");
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_L_Serial_Num)  , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_Loc)           , sToolLoc);
                }
                else if (nLoc == (int)JSCK.eTableLoc.N2)
                {
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID)          , "");
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID)          , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_R_Serial_Num)  , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_Loc)           , sToolLoc);
                    //CData.LotInfo.sRTool_Serial_Num = CData.JSCK_Gem_Data[0].sCurr_Tool_Serial_Num[1];
                }

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Verify_Request).ToString());
                
                if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
                {// 20200718 Local Mode 사용
                    Local_OnS2F41_Set(GEM.sRCMD_TOOL_VERIFICATION, sToolLoc);
                }
            }
        }
        public void StdB_OnToolVerifyRequest(string sToolId, uint nLoc)
        {
            string sToolLoc = "";

            if (!CData.Opt.bSecsUse) return; //191031 ksg :

            // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("OnToolVerifyRequest({0},{1},{2},{3})", sToolId, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0], CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1], nLoc));             
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Tool ID = {2},  Table Loc. : {3}, Current Tool-1 ID : {4}, Current Tool-2 ID : {5}",
                                                            Convert.ToInt32(JSCK.eBCEID.Wheel_Verify_Request),
                                                            JSCK.eBCEID.Wheel_Verify_Request.ToString(),
                                                            sToolId, nLoc,
                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0],
                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]));
            // 2021.09.01 SungTae End

            sToolLoc = nLoc.ToString();
            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0] = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1] = sToolId; // [0]=Left, [1]=Right

            if (nLoc == (int)JSCK.eTableLoc.N1)
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg1_WheelID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg2_WheelID), "");
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_1), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Loc), sToolLoc);
            }
            else if (nLoc == (int)JSCK.eTableLoc.N2)
            {
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg1_WheelID), "");
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Loaded_Sg2_WheelID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1]);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_FileName_1), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1]);
                Set_SVID(Convert.ToInt32(JSCK.eBSVID.Wheel_Loc), sToolLoc);
                //CData.LotInfo.sRTool_Serial_Num = CData.JSCK_Gem_Data[0].sCurr_Tool_Serial_Num[1];
            }

            Send_CEID(Convert.ToInt32(JSCK.eBCEID.Wheel_Verify_Request).ToString());

            if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Local))
            {// 20200718 Local Mode 사용
                Local_OnS2F41_Set(GEM.sRCMD_TOOL_VERIFICATION, sToolLoc);
            }
        }
    #endregion  CEID Event - function

    #region Recipe 변경 부분
    public void Device_Add()
        {
            string sPPID = string.Empty;
            string sPath = string.Empty;
            tDev tRcp;
            Log_wt(string.Format("Device_Add()"));

            using (frmTxt mForm = new frmTxt("생성할 디바이스의 이름을 지정해 주세요."))
            {
                if (mForm.ShowDialog() == DialogResult.OK){
                    if (mForm.Val == string.Empty){
                        CMsg.Show(eMsg.Error, "에러", "생성할 디바이스 이름이 지정되지 않았습니다. 생성할 디바이스 이름을 지정해 주세요.");
                        return;
                    }
                    sPPID = mForm.Val;
                    sPath = string.Format("{0}\\Device\\GEM\\{1}.dev", fileinfo.Directory.FullName, sPPID);

                    if (File.Exists(sPath)){
                        CMsg.Show(eMsg.Error, "에러", "같은 이름의 디바이스가 있습니다.");
                        return;
                    }

                    CDev.It.InitDev(out tRcp);
                    tRcp.sName = sPPID;
                    CDev.It.Save(sPath, tRcp);

                    if (sPPID != string.Empty)
                    { Set_PPIDA(sPPID); }
                }
            }
        }

        public void Device_Del()
        {
            string sPPID = string.Empty;
            string sPath = string.Empty;
            Log_wt(string.Format("Device_Del()"));

            using (frmTxt mForm = new frmTxt("삭제할 디바이스의 이름을 지정해 주세요."))
            {
                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val == string.Empty)
                    {
                        CMsg.Show(eMsg.Error, "에러", "삭제할 디바이스 이름이 지정되지 않았습니다. 삭제할 디바이스 이름을 지정해 주세요.");
                        return;
                    }

                    sPPID = mForm.Val;
                    sPath = string.Format("{0}\\Device\\GEM\\{1}.dev", fileinfo.Directory.FullName, sPPID);

                    if (!File.Exists(sPath))
                    {
                        CMsg.Show(eMsg.Error, "에러", "같은 이름의 디바이스가 없습니다.");
                        return;
                    }

                    if (CMsg.Show(eMsg.Warning, "주의", sPPID + " 디바이스 파일을 삭제하시겠습니까?") == DialogResult.Cancel) { return; }

                    FileSystem.DeleteFile(sPath);

                    if (sPPID != string.Empty)  { Set_PPIDD(sPPID); }
                }
            }
        }

        public void Device_Edit()
        {
        }

        public void Device_Send_Parameter()
        {
        }

        public void Device_Change()
        {
        }

        #endregion  CEID Event - function


        #region Send Message
       
         
        public void Send_Msg(int nMID, string sMsg)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s1");
                    Send_Msg(nMID,sMsg);

                });

            }
            else
            {
                string sTN = string.Empty;
                byte[] tBuff;
                IntPtr hwnd;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();

                sTN         = GEM.sWND;
                hwnd        = FindWindow(null, sTN);
                hwnd        = this.Handle;
                tBuff       = System.Text.Encoding.Default.GetBytes(sMsg);
                cds.dwData  = new IntPtr(nMID);
                cds.cbData  = tBuff.Length + 1;
                cds.lpData  = sMsg;

                SendMessage(hwnd, WM_COPYDATA, hwnd, ref cds);   
            }        
        }

        public void Send_SVID(string sMsg)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s2");
                    Send_SVID(sMsg);

                });

            }
            else
            {
                string sTN = string.Empty;
                byte[] tBuff;
                IntPtr hwnd;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();

                sTN = GEM.sWND;
                hwnd = FindWindow(null, sTN);

                //hwnd = this.Handle;
                tBuff       = System.Text.Encoding.Default.GetBytes(sMsg);
                cds.dwData  = new IntPtr(Convert.ToInt32(JSCK.eMID.SVID));
                cds.cbData  = tBuff.Length + 1;
                cds.lpData  = sMsg;
                SendMessage(hwnd, WM_COPYDATA, hwnd, ref cds);
            }
        }
        public void Send_CEID(string sMsg)
        {
            //mgem.SendEventReport( Int32.Parse(sMsg));
            //return;
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s3");
                    Send_CEID(sMsg);

                });

            }
            else
            {
                string sTN = string.Empty;
                byte[] tBuff;
                IntPtr hwnd;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();

                sTN         = GEM.sWND;
                hwnd        = FindWindow(null, sTN);
                //hwnd = this.Handle;
                tBuff       = System.Text.Encoding.Default.GetBytes(sMsg);
                cds.dwData  = new IntPtr(Convert.ToInt32(JSCK.eMID.CEID));
                cds.cbData  = tBuff.Length + 1;
                cds.lpData  = sMsg;
                SendMessage(hwnd, WM_COPYDATA, hwnd, ref cds);
            }
        }

        public void Send_AlarmOccur(string sMsg)
        {

            //mgem.SendEventReport( Int32.Parse(sMsg));
            //return;
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s4");
                    Send_AlarmOccur(sMsg);

                });

            }
            else
            {
                string sTN = string.Empty;
                byte[] tBuff;
                IntPtr hwnd;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();

                sTN         = GEM.sWND;
                hwnd        = FindWindow(null, sTN);
                hwnd        = this.Handle;
                tBuff       = System.Text.Encoding.Default.GetBytes(sMsg);
                cds.dwData  = new IntPtr(Convert.ToInt32(JSCK.eMID.AlarmOccur));
                cds.cbData  = tBuff.Length + 1;
                cds.lpData  = sMsg;

                SendMessage(hwnd, WM_COPYDATA, hwnd, ref cds);
            }
        }

        public void Send_AlarmReset(string sMsg)
        {

            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Console.WriteLine("---- s5");
                    Send_AlarmReset(sMsg);

                });

            }else{
                string sTN = string.Empty;
                byte[] tBuff;
                IntPtr hwnd;
                COPYDATASTRUCT cds = new COPYDATASTRUCT();

                sTN         = GEM.sWND;
                hwnd        = FindWindow(null, sTN);
                hwnd        = this.Handle;
                tBuff       = System.Text.Encoding.Default.GetBytes(sMsg);
                cds.dwData  = new IntPtr(Convert.ToInt32(JSCK.eMID.AlarmReset));
                cds.cbData  = tBuff.Length + 1;
                cds.lpData  = sMsg;
                SendMessage(hwnd, WM_COPYDATA, hwnd, ref cds);
            }
        }
        #endregion Send Message

        #region function Event - OnEventReceived
        private void OnEventReceived(IntPtr lpParam, short nEventId, int lParam)
        {
            switch (nEventId)
            {
                case 1:
                    {//tcp connect
                        OnConnected();

                        break;
                    }
                case 2:
                    {//tcp disconnect
                        OnDisconnected();

                        break;
                    }
                case 401:
                    {//message in
                        OnMsgIn(lParam);

                        break;
                    }
                case 402:
                    {//message out
                        OnMsgOut(lParam);

                        break;
                    }
                case 1001:
                    {//S1F15
                        OnOffline();

                        break;
                    }
                case 1002:
                    {//S1F17
                        OnOnlineLocal();

                        break;
                    }
                case 1003:
                    {//S1F17
                        OnOnlineRemote();

                        break;
                    }
                case 1010:
                    {//
                        OnCommunicating();

                        break;
                    }
                case 1015:
                    {//S2F15w
                        OnNewHost_ECID(lParam);

                        break;
                    }
                case 1030:
                    {//S2F41w
                        OnRemoteCommand(lParam);

                        break;
                    }
                case 1050:
                    {//S10F3, S10F5
                        OnTerminalMsg(lParam);

                        break;
                    }
                default:
                    {//
                        break;
                    }
            }
        }

        public void OnMsgIn(int lParam)
        {
            int nS = 0;    //Stream
            int nF = 0;    //Function

            nS = (int)(lParam / 1000);
            nF = lParam % 1000;

            Log_wt(string.Format("[RECV](H->E) S{0}, F{1}", nS, nF));
        }

        public void OnMsgOut(int lParam)
        {
            int nS = 0;    //Stream
            int nF = 0;    //Function

            nS = (int)(lParam / 1000);
            nF = lParam % 1000;

            Log_wt(string.Format("[SEND](H<-E) S{0}, F{1}", nS, nF));
        }

        public void OnConnected()
        {
            Log_wt(string.Format("OnConnected()"));

            m_iConnState = Convert.ToInt32(JSCK.eConn.Connected);
            CData.SecsGem_Data.nConnect_Flag = m_iConnState;

            lblConn_Status.Text = JSCK.eConn.Connected.ToString();
            lblConn_Status.BackColor = GEM.clrA_RC[Convert.ToInt32(JSCK.eConn.Connected)];

            // 2021.05.23 SungTae Start: [추가] 추가한 SVID(Actual Parameter(Spindle & Grd Y-Axis Speed)) Update 설정 Delay
            if (CData.CurCompany == ECompany.SPIL)
            {
                OnSetAddSVID_ActualCycleDepth(EWay.L);
                OnSetAddSVID_ActualCycleDepth(EWay.R);
                OnSetAddSVID_ActualSpeed();
                OnSetAddSVID_Device();
            }
            // 2021.05.23 SungTae End
        }

        public void OnDisconnected()
        {
            Log_wt(string.Format("OnDisconnected()"));

            m_iConnState = Convert.ToInt32(JSCK.eConn.Disconnected);
            CData.SecsGem_Data.nConnect_Flag = m_iConnState;

            lblConn_Status.Text = JSCK.eConn.Disconnected.ToString();
            lblConn_Status.BackColor = GEM.clrA_RC[Convert.ToInt32(JSCK.eConn.Disconnected)];

            // 2021-06-23, jhLee : SPIL VOC : OFF-Line으로 변경시 Alarm 발생
            if (CData.CurCompany == ECompany.SPIL)
            {
                // Alarm 발생 처리
                Log_wt($"Error : Communication state change to Disconnected ");
                CErr.Show(eErr.SECSGEM_OFFLINE_ERROR);                 //393 SECS/GEM 연결이 Offline이 되었다. 
            }
        }

        // Off-Line으로 변경
        public void OnOffline()
        {
            // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("OnOffline()"));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Control State : {2}", Convert.ToInt32(JSCK.eCEID.Control_State_Change), JSCK.eCEID.Control_State_Change.ToString(), JSCK.eCont.Offline.ToString()));
            // 2021.09.01 SungTae End

            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Offline);
            CData.SecsGem_Data.nRemote_Flag = m_iCtrlState;

            // 2021.06.02 lhs Start : 오류 수정
            //old mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Process_Status), m_iCtrlState.ToString());
            //mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Process_Status), m_iPS.ToString());    // 2021-05-12, jhLee : SPIL-FJ VOC, On-line 연결시 현재 상태가 1로 전송됨 (실제로는 0)

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Control_State), m_iCtrlState.ToString());
                // 2021.06.02 lhs End          
                mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Control_State_Change));
            }
            else
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Equipment_State), m_iCtrlState.ToString());        
                mgem.SendEventReport(Convert.ToInt32(JSCK.eBCEID.Control_State_Change));
            }
           
            lblCont_Status.Text         = JSCK.eCont.Offline.ToString();
            lblCont_Status.BackColor    = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Offline)];

            btnCont_Offline.Enabled     = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Offline) ) ? false : true;
            btnCont_Local.Enabled       = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Local)   ) ? false : true;
            btnCont_Remote.Enabled      = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Remote)  ) ? false : true;

            // 2021-06-23, jhLee : SPIL VOC : OFF-Line으로 변경시 Alarm 발생
            if (CData.CurCompany == ECompany.SPIL)
            {
                // Alarm 발생 처리
                Log_wt($"Error : Communication state change to OFF-Line ");
                CErr.Show(eErr.SECSGEM_OFFLINE_ERROR);                 //393 SECS/GEM 연결이 Offline이 되었다. 
            }
        }

        public void OnOnlineLocal()
        {
            // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("OnOnlineLocal()"));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Control State : {2}", Convert.ToInt32(JSCK.eCEID.Control_State_Change), JSCK.eCEID.Control_State_Change.ToString(), JSCK.eCont.Local.ToString()));
            // 2021.09.01 SungTae End

            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Local);
            CData.SecsGem_Data.nRemote_Flag = m_iCtrlState;
            
            mgem.GoOnlineLocal();

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Control_State), m_iCtrlState.ToString());
                mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Control_State_Change));
            }
            else
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Equipment_State), m_iCtrlState.ToString());
                mgem.SendEventReport(Convert.ToInt32(JSCK.eBCEID.Control_State_Change));
            }
            
            lblCont_Status.Text         = JSCK.eCont.Local.ToString();
            lblCont_Status.BackColor    = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Local)];

            btnCont_Offline.Enabled     = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Offline) ) ? false : true;
            btnCont_Local.Enabled       = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Local)   ) ? false : true;
            btnCont_Remote.Enabled      = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Remote)  ) ? false : true;
        }

        public void OnOnlineRemote()
        {
            // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("OnOnlineRemote()"));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Control State : {2}", Convert.ToInt32(JSCK.eCEID.Control_State_Change), JSCK.eCEID.Control_State_Change.ToString(), JSCK.eCont.Remote.ToString()));
            // 2021.09.01 SungTae End

            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Remote);
            CData.SecsGem_Data.nRemote_Flag = m_iCtrlState;
            mgem.GoOnlineRemote();

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Control_State), m_iCtrlState.ToString());
                mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Control_State_Change));
            }
            else
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Equipment_State), m_iCtrlState.ToString());
                mgem.SendEventReport(Convert.ToInt32(JSCK.eBCEID.Control_State_Change));
            }
            
            lblCont_Status.Text         = JSCK.eCont.Remote.ToString();
            lblCont_Status.BackColor    = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Remote)];

            btnCont_Offline.Enabled     = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Offline)) ? false : true;
            btnCont_Local.Enabled       = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Local)) ? false : true;
            btnCont_Remote.Enabled      = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Remote)) ? false : true;

            Initial_PPID_SVID_SET();
        }

        private void Initial_PPID_SVID_SET()
        {// 20200701 SECSGEM 초기화 이후 Recipe Load Event 송부

            //200824 jhc : GV.PATH_DEVICE 잘못 설정될 경우에 대한 대비
            string [] sSeparators = new string[] { "\\" };
            string [] sSplitPath = CData.DevCur.Split(sSeparators, StringSplitOptions.RemoveEmptyEntries);
            int iLen = sSplitPath.Length;
            if (iLen >= 2)
            {
                if (UseRecipeFolder)
                {
                    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY].sCurr_Recipe_Name = sSplitPath[iLen - 1].Replace(".dev", "");
                }
                else
                {
                    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sSplitPath[iLen - 2] + "\\" + sSplitPath[iLen - 1].Replace(".dev", "");
                }
               // CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sSplitPath[iLen - 2] + "\\" + sSplitPath[iLen - 1].Replace(".dev", "");
            }
            else
            {
                string sName = CData.DevCur.Replace(GV.PATH_DEVICE, "");
                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sName.Replace(".dev", "");
            }

            // 2021.09.01 SungTae Start : [수정] Multi-LOT 관련
            //Log_wt(string.Format("Initial_PPID_SVID_SET() Start {0} {1}", CData.DevCur, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name));
            Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  PPID : {2}", Convert.ToInt32(JSCK.eCEID.Recipe_Loaded), JSCK.eCEID.Recipe_Loaded.ToString(), JSCK.eCont.Remote.ToString()));
            // 2021.09.01 SungTae End
            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_PPID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.PPID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
                mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Recipe_Loaded));
            }
            else
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Current_PPID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Changed_PPID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
                mgem.SendEventReport(Convert.ToInt32(JSCK.eBCEID.Recipe_Loaded));
            }
            
            //Log_wt(string.Format("Initial_PPID_SVID_SET() End {0} {1}", CData.DevCur, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name));
        }

        public void OnCommunicating()
        {
            Log_wt(string.Format("OnCommunicating()"));

            m_iCommState = Convert.ToInt32(JSCK.eComm.Communicating);
            CData.SecsGem_Data.nComm_Flag = m_iCommState;
            
            lblComm_Status.Text      = JSCK.eComm.Communicating.ToString();
            lblComm_Status.BackColor = GEM.clrA_WRC[Convert.ToInt32(JSCK.eComm.Communicating)];

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardA) { }
            else
            {
                mgem.SetSVIDValue(Convert.ToInt32(JSCK.eBSVID.Communication_State), m_iCommState.ToString());
                mgem.SendEventReport(Convert.ToInt32(JSCK.eBCEID.Communication_State_Change));
            }

        }

        public void OnRemoteCommand(int lMID)
        {//S2F41 사용
            Log_wt(string.Format("OnRemoteCommand()"));
        }

        public void OnNewHost_ECID(int lMID)
        {
            short nECIDCount = 0;
            int iECID = 0;
            string sVal = string.Empty;

            nECIDCount = 0;

            while (nECIDCount > -1)
            {
                //host에서 전송한 ECID, ECID Value를 가져옴
                nECIDCount = mgem.GetHostSetECID(lMID, ref iECID, ref sVal);

                if (nECIDCount < 0)
                { break; }

                Log_wt(string.Format("ECID = {0}, Value = {1}", iECID, sVal));
                mgem.SetECValue(iECID, sVal);

                switch (iECID)
                {
                    case (int)JSCK.eECID.T3_Timeout:
                        {
							this.Invoke(new MethodInvoker(delegate ()
							{// cross thread
								txtHSMS_T31.Text = sVal;
								ECV.m_nT3 = TransShort(sVal);

							}));
                            break;
                        }
                    case (int)JSCK.eECID.T5_Timeout:
                        {
                            this.Invoke(new MethodInvoker(delegate ()
							{
								txtHSMS_T51.Text = sVal;
								ECV.m_nT5 = TransShort(sVal);
							}));
                            break;
                        }
                    case (int)JSCK.eECID.T6_Timeout:
                        {
                            this.Invoke(new MethodInvoker(delegate ()
							{
								txtHSMS_T61.Text = sVal;
								ECV.m_nT6 = TransShort(sVal);
							}));
                            break;
                        }
                    case (int)JSCK.eECID.T7_Timeout:
                        {
                            this.Invoke(new MethodInvoker(delegate ()
							{
								txtHSMS_T71.Text = sVal;
								ECV.m_nT7 = TransShort(sVal);
							}));
                            break;
                        }
                    case (int)JSCK.eECID.T8_Timeout:
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                txtHSMS_T81.Text = sVal;
                                ECV.m_nT8 = TransShort(sVal);
                            }));

                            break;
                        }
                    case (int)JSCK.eECID.Link_Test_Interval:
                        {
                            this.Invoke(new MethodInvoker(delegate ()
							{
								txtHSMS_LT1.Text = sVal;
								ECV.m_nLinkTestInterval = TransShort(sVal);
							}));
                            break;
                        }
                    case (int)JSCK.eECID.Communication_Establish_Timeout:
                        {
                            
							this.Invoke(new MethodInvoker(delegate ()
							{
								ECV.m_nCommReqeustTimeout = TransShort(sVal);
							}));
                            break;
                        }
                }


                // 2021-05-11, jhLee, 엔비아소프트 박성규 차장 소스 반영
                //              SPIL-FJ에서 S2F15로 명령 전송시 프로그램 죽는 문제 대응

                if (nECIDCount < 1) // 0에서 1로 변경, 0인 경우 마지막 ecid
                {
                    break;
                }
            }

            mgem.ReplyHostSetECID(lMID, 0);
        }


        public void OnTerminalMsg(int lMID)
        {//S10F3, S10F5를 받은 경우 장비화면이나 팝업화면에 표시
            short   nTID    = 0;
            int     iCount  = 0;
            int     iCnt    = 0;
            string  sMsg    = string.Empty;
            string  sTMsg   = string.Empty;

            Log_wt(string.Format("OnTerminalMsg()"));

            while (iCount > -1)
            {
                iCount = mgem.GetTerminalMsg(lMID, ref nTID, ref sMsg);

                if (iCount < 0) { break; }
                if (iCnt == 0)  { sTMsg = sMsg; }
                else            { sTMsg = string.Format("{0}\r\n{1}", sTMsg, sMsg); }

                iCnt++;
            }

            if (CData.SecsGem_Data.qTerminal_Msg != null)
            {
                CData.SecsGem_Data.qTerminal_Msg.Enqueue(sTMsg);

                // 2021.10.13 SungTae Start : [수정] Multi-LOT 관련
                //if (CData.IsMultiLOT())
                //{
                //    if (sgfrmMsg2.Visible == false)
                //    {
                //        sgfrmMsg2.Display();
                //    }
                //    else
                //    {
                //        sgfrmMsg2.Visible = true;
                //        sgfrmMsg2.ShowIcon = true;
                //        notifyIcon1.Visible = false;
                //    }
                //}
                //else
                {
                    if (sgfrmMsg.Visible == false)
                    {
                        sgfrmMsg.Display();
                    }
                    else
                    {
                        sgfrmMsg.Visible = true;
                        sgfrmMsg.ShowIcon = true;
                        notifyIcon1.Visible = false;
                    }
                }
                // 2021.10.13 SungTae End
            }

            sTMsg = string.Empty;
            sMsg = string.Empty;

        }
        #endregion function Event - OnEventReceived

        #region function Event - OnMsgReceived
        private void OnMsgReceived(IntPtr lpParam, int lMID)
        {
            short nS = 0;    //Stream
            short nF = 0;    //Function
            short nWbit = 0;
            int nLen = 0;
            Log_wt(string.Format("OnMsgReceived()"));

            mgem.GetMsgInfo(lMID, ref nS, ref nF, ref nWbit, ref nLen);

            if (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Offline))
            {
                Log_wt(string.Format("JSCK.eCont.Offline"));
                mgem.AbortMsg(lMID);
                return;
            }
            else if (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Local))
            {//20200718 Local Mode 일때 Return
                Log_wt(string.Format("JSCK.eCont.Local"));
                mgem.AbortMsg(lMID);
                return;
            }
            else
            {
                if      (nS == 7 && nF == 1)    { OnS7F1(lMID);     }
                else if (nS == 7 && nF == 3)    { OnS7F3(lMID);     }
                else if (nS == 7 && nF == 5)    { OnS7F5(lMID);     }
                else if (nS == 7 && nF == 17)   { OnS7F17(lMID);    }
                else if (nS == 7 && nF == 19)   { OnS7F19(lMID);    }
                else if (nS == 2 && nF == 41)   { OnS2F41(lMID);    }
                else if (nS == 2 && nF == 49)   { OnS2F49(lMID);    } // Sklyworks Site 
                else
                { }
            }
        }

        public void OnS7F1(int lMID)
        {
            int rMID = mgem.CreateReplyMsg(lMID);
            //List<string> listRecipe = new List<string>();

            Log_wt(string.Format("OnS7F1()"));
            //20190625 josh
            //실제 장비의 recipe list를 넣어주어야 함
            Get_DeviceFileList();
            byte nPPGNT = 0;
            mgem.AddBinaryItem(rMID, nPPGNT);
            mgem.SendMsg(rMID);
        }

        public void OnS7F3(int lMID)
        {
            string sPPID = string.Empty;
            string sFile = string.Empty;

            Log_wt(string.Format("OnS7F3()"));
            mgem.GetListItemOpen(lMID);
            mgem.GetAsciiItem(lMID, ref sPPID);
            sPPID = sPPID.Trim();

            sFile = string.Format("{0}\\{1}.dev", m_sExePath, sPPID);
            mgem.GetFileBinaryItem(lMID, sFile);
            mgem.GetListItemClose(lMID);

            byte nAck7 = 0x00;
            int rMID = mgem.CreateReplyMsg(lMID);

            mgem.AddBinaryItem(rMID, nAck7);
            mgem.SendMsg(rMID);
        }

        public void OnS7F5(int lMID)
        {
            string sPPID = string.Empty;
            string sFile = string.Empty;
            int rMID = 0;
            
            //Log_wt(string.Format("OnS7F5()"));

            mgem.GetAsciiItem(lMID, ref sPPID);
            sFile = string.Format("{0}\\{1}.dev", m_sExePath, sPPID);
            rMID = mgem.CreateReplyMsg(lMID);

            //Log_wt(string.Format("[RECV](H->E) S7F5(Process Program Request), PPID : ", sPPID));
			Log_wt($"[RECV](H->E) S7F5(Process Program Request), PPID : {sPPID}");

            if (File.Exists(sFile))
            {
                mgem.OpenListItem(rMID);
                mgem.AddAsciiItem(rMID, sPPID, sPPID.Length);
                mgem.AddFileBinaryItem(rMID, sFile);
                mgem.CloseListItem(rMID);
                mgem.SendMsg(rMID);
            }
            else
            {
                mgem.OpenListItem(rMID);
                mgem.CloseListItem(rMID);
                mgem.SendMsg(rMID);
            }
        }

        public void OnS7F17(int lMID)
        {//20190724 josh    needs
            List<string> listDeleteRecipe = new List<string>();
            byte nAck7 = 0x00;
            string sFile = m_sExePath;
            DirectoryInfo d;

            int nCnt = mgem.GetListItemOpen(lMID);

            Log_wt(string.Format("OnS7F17()"));

            for (int i = 0; i < nCnt; i++)
            {
                string sR = string.Empty;

                mgem.GetAsciiItem(lMID, ref sR);
                listDeleteRecipe.Add(sR);
            }

            mgem.GetListItemClose(lMID);

            if (nCnt == 0)
            {//전체 삭제
             //만약 작업 중이거나 레시피를 삭제할 수 없는 경우 0x01
             //삭제할 수 있는 경우 0x01
                try
                {
                    if (UseRecipeFolder)
                    {
                        d = new DirectoryInfo(m_sExePath);


                        FileInfo[] file = d.GetFiles();
                        for (int i = 0; i < file.Length; i++)
                        {
                            string sf = string.Format("{0}\\{1}", sFile, file[i]);
                            File.Delete(sf);
                        }
                        nAck7 = 0x00;
                    }
                    else
                    { // Device폴더 전체 삭제 예방
                        nAck7 = 0x01;
                    }
                }
                catch (Exception ex)
                {
                    nAck7 = 0x01;
                    Log_wt(string.Format("S7F17 Delete Fail : {0}", ex.ToString()));
                }
            }
            else
            {
                //삭제할 수 없는 조건인 경우 삭제하지 않고 0x01
                //nCnt 개수만큼 파일이 존재하는 지 확인 후 존재하면 0x00 보고 후 삭제

                bool bExistDR = true;




                for (int i = 0; i < nCnt; i++)
                {
                    string sf = string.Format("{0}\\{1}.dev", sFile, listDeleteRecipe[i]);

                    if (!File.Exists(sf))
                    {
                        bExistDR = false;
                        nAck7 = 0x01;

                        break;
                    }
                }

                if (bExistDR)
                {
                    for (int i = 0; i < nCnt; i++)
                    {
                        string sf = string.Format("{0}\\{1}.dev", sFile, listDeleteRecipe[i]);

                        File.Delete(sf);
                    }
                }
            }

            int rMID = mgem.CreateReplyMsg(lMID);

            mgem.AddBinaryItem(rMID, nAck7);
            mgem.SendMsg(rMID);
        }

        public void OnS7F19(int lMID)
        {

            int rMsgId = mgem.CreateReplyMsg(lMID); // S7F19의 응답인 S7F20을 생성.
            mgem.OpenListItem(rMsgId);
            //DirectoryInfo d = new DirectoryInfo( m_strExePath );//Assuming Test is your Folder
            string strRecipeFolder = m_sExePath;
            DirectoryInfo f = new DirectoryInfo(strRecipeFolder);
            DirectoryInfo[] dir = f.GetDirectories();

            if (UseRecipeFolder)
            {
                DirectoryInfo d = new DirectoryInfo(strRecipeFolder);
                FileInfo[] Files = d.GetFiles("*.dev"); //Getting Text files
                foreach (FileInfo file in Files)
                {

                    string filename = file.Name.Replace(".dev", "");
                    // filename = string.Format("{0}\\{1}", di.Name, filename);
                    mgem.AddAsciiItem(rMsgId, filename, filename.Length);
                }
            }
            else
            {
                foreach (DirectoryInfo di in dir)
                {
                    //string strFullPath = strRecipePolder + "\\" + di.Name + "\\";
                    string strFullPath = string.Format( "{0}\\{1}" , strRecipeFolder, di.Name);

                    DirectoryInfo d = new DirectoryInfo(strFullPath);
                    FileInfo[] Files = d.GetFiles("*.dev"); //Getting Text files
                    foreach (FileInfo file in Files)
                    {

                        string filename = file.Name.Replace(".dev", "");
                        filename = string.Format("{0}\\{1}", di.Name, filename);
                        mgem.AddAsciiItem(rMsgId, filename, filename.Length);
                    }
                }
            }
            mgem.CloseListItem(rMsgId);
            mgem.SendMsg(rMsgId);
        }
        /// <summary>
        /// Local Mode 에서는 자체에서 값을 설정
        /// 
        /// </summary>        
        public void Local_OnS2F41_Set(string strval, string sSub_data)
        {
            List<string> aSubSt = new List<string>();

            //string strQTY       = string.Empty;
            string strQTY       = sSub_data;
            string strPPID      = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sCurr_Recipe_Name;
            string strCarrierId = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID;                                  
            string strToolId    = string.Empty;
            string strMgzId     = string.Empty;
            string strPortNo    = string.Empty;
            string strLotId     = string.Empty;

            Log_wt(string.Format("Local_OnS2F41_Set : {0} , {1}) ", strval, sSub_data));

            if (strval == GEM.sRCMD_LOT_VERIFICATION)
            {
                /*
                CData.JSCK_Gem_Data[4].sLot_Name = CData.LotInfo.sLotName ;
                Log_wt(string.Format(CData.JSCK_Gem_Data[4].sLot_Name + " Local  Carrid ID"));
                CData.SecsGem_Data.nStrip_Check = 2;// Lot Varification 완료 Strip Varifaction 시퀸스 계속 진행
                SecLotOpen(CData.JSCK_Gem_Data[4].sLot_Name, strQTY, strPPID);

                AddStripFromHost(aSubSt, CData.JSCK_Gem_Data[4].sLot_Name);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID), CData.JSCK_Gem_Data[4].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID), CData.JSCK_Gem_Data[4].sLot_Name);
                CData.LotInfo.sLotName = CData.JSCK_Gem_Data[4].sLot_Name;

                Log_wt(string.Format("2. CData.JSCK_Gem_Data[4].sLot_Name {0}", CData.JSCK_Gem_Data[4].sLot_Name));
                Log_wt(string.Format("3. m_sReadLOT_T {0}", m_sReadLOT_T));

                m_sReadLOT_T = ""; // 임시변수 초기화

                m_pReadCarrierList.SetLotID(strLotId);
                m_pStartCarrierList.SetLotID(strLotId);
                m_pValidCarrierList.SetLotID(strLotId);
                m_pEndedCarrierList.SetLotID(strLotId);
                m_pRejectCarrierList.SetLotID(strLotId);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Verified).ToString());

                CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[4].sInRail_Strip_ID, (int)SECSGEM.JSCK.eCarrierStatus.Read);
                Log_wt(string.Format("2. Lot ID - Varification 완료 CData.JSCK_Gem_Data[4].sLot_Name {0}", CData.JSCK_Gem_Data[4].sLot_Name));
                sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                */
            }
            else if (strval == GEM.sRCMD_CARRIER_VERIFICATION)
            {
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotInfo.sLotName;
                
                Log_wt(string.Format(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name + " Local Carrier ID 설정 함"));
                
                SecLotOpen(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name, strQTY, strPPID);// Strip 매수와 Lot ID 는 어찌 설정
 
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;// 읽은 2D Carrid ID
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Min_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Min_Data[0];
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Max_Data[0];
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].fMeasure_Avr_Data[0];
                CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode        = CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].nDF_User_Mode;


                /* 이거는 어찌 하는 거지 ??*/
                //m_pValidCarrierList.AddCarrier(CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID);
                //CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Validate));
                //Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));

                //Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID), CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID);
                //Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verified).ToString());
                // 3- 2) JSCK SECSGEM 사용시 Strip Varification 진행 후 Pick 승인 완료 하여 작업 시작
                //CData.SecsGem_Data.nStrip_Check = 3;// 시퀸스 계속 진행
                //CSq_OnP.It.m_bPickOK = true; 20200910 삭제 LCY
                //sgfrmMsg.ShowGrid(CData.m_pHostLOT);

                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행                
                Log_wt(string.Format("{0} Local Carrier ID  설정 함.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));
            }
            else if (strval == GEM.sRCMD_MAT_VERIFICATION)
            {
                string strTableLoc = sSub_data;
                /*
                if (strTableLoc == "1")
                {
                    CData.LotInfo.sCurr_LDrsId  = CData.JSCK_Gem_Data[0].sCurr_Dress_Name[0];  // 현재 설정 된  Dress ID
                    CData.LotInfo.sLDrsId       = CData.JSCK_Gem_Data[0].sCurr_Dress_Name[0];  // 현재 설정 된  Dress ID
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), CData.JSCK_Gem_Data[0].sCurr_Dress_Name[0]);
                }
                else if (strTableLoc == "2")
                {
                    CData.LotInfo.sCurr_RDrsId  = CData.JSCK_Gem_Data[0].sCurr_Dress_Name[1];
                    CData.LotInfo.sRDrsId       = CData.JSCK_Gem_Data[0].sCurr_Dress_Name[1];
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), CData.JSCK_Gem_Data[0].sCurr_Dress_Name[1]);
                }*/

                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
                {
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL_Loc), strTableLoc);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc), strTableLoc);
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Changed).ToString());
                }
                else
                {
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL_Loc), strTableLoc);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc), strTableLoc);
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Changed).ToString());
                }
            }
            else if (strval == GEM.sRCMD_PP_SELECT)
            {
                /*
                if (isExistRecipe(strPPID) == true){
                    if (!CData.LotInfo.bLotOpen){
                        CData.JSCK_Gem_Data[4].sNew_Recipe_Name = strPPID;// 현재 내려 받은 Recipe ID
                        Log_wt(string.Format(" Local {0} Recipe Open.", CData.JSCK_Gem_Data[4].sNew_Recipe_Name));
                        CData.FrmMain.m_vwRcp.LoadDeviceSecs(CData.JSCK_Gem_Data[4].sNew_Recipe_Name);

                        Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_PPID)   , CData.JSCK_Gem_Data[4].sNew_Recipe_Name);
                        Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID)           , CData.JSCK_Gem_Data[4].sNew_Recipe_Name);
                        Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Loaded).ToString());
                    }else
                    {   
                        Log_wt(string.Format(" Local {0} Lot Open 중으로 Recipe Open 불가.", CData.JSCK_Gem_Data[4].sNew_Recipe_Name));
                    }
                }else{
                    Log_wt(string.Format(" Local {0} Recipe 없어 Open 불가.", CData.JSCK_Gem_Data[4].sNew_Recipe_Name));
                } 
                */
            }
            else if (strval == GEM.sRCMD_TOOL_VERIFICATION)
            {
                if (CDataOption.SecsGemVer == eSecsGemVer.StandardA)
                {
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Changed).ToString());
                }
                else
                {
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Changed).ToString());
                }
            }
            else if (strval != GEM.sRCMD_MGZ_VERIFICATION)
            {
                strPortNo = sSub_data;
                if (strPortNo == "3")
                {
                    CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New;
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New);
                }
                else
                {// Unloader Mgz Verifyed
                    if      (strPortNo == "1")  {   CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New; }
                    else if (strPortNo == "2")  {   CData.LotInfo.sNMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New; }
                    
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);

                    m_pMgzNGCarrierList.ClearCarrier();
                    m_pMgzNGCarrierList.SetMgzID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                    m_pMgzNGCarrierList.SetLotID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), strPortNo);
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verified).ToString());
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Started).ToString());
            }
        }     
        /// <summary>
        /// Verify 실패시에 RCM NAC 발생 후 GUI 및 내부 변수 초기화 진행
        /// </summary>
        /// <param name="strval"></param>
        private void RCMD_Fail_Data_Clear(string sLoc, string strval)
        {
            Log_wt(string.Format("RCMD_Fail_Data_Clear(sLoc {0} , strval {1}) ", sLoc,strval));
            if      (strval == GEM.sRCMD_LOT_VERIFICATION)      {   } 
            else if (strval == GEM.sRCMD_CARRIER_VERIFICATION)  {   } 
            else if (strval == GEM.sRCMD_MAT_VERIFICATION) 
            {
                if (sLoc == "1")   CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] = ""; 
                else               CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] = ""; 
            } 
            else if (strval == GEM.sRCMD_PP_SELECT)             {   } 
            else if (strval == GEM.sRCMD_TOOL_VERIFICATION) 
            {
                if (sLoc == "1") CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0] = "";
                else             CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] = "";
            } 
            else if (strval != GEM.sRCMD_MGZ_VERIFICATION)      {    }
        }

        #region S2F41 메세지
        public void OnS2F41(int lMsgId)
        {// OnRemoteCommand
            List<string>    aSubSt  = new List<string>();
            List<byte>      aCPAck  = new List<byte>();
            List<string>    aCPName = new List<string>();
            
            Log_wt(string.Format("OnS2F41()"));

            bool bResult            = false;
            byte nLocation          = 0x00;
            byte nHcack             = 0x00;
            string strRcmd          = string.Empty;
            string strCPName        = string.Empty;
            string strCPValue       = string.Empty;
            string strMatId         = string.Empty;
            string strLotId         = string.Empty;
            string strPPID          = string.Empty;
            string strQTY           = string.Empty;
            string strErrorMessage  = string.Empty;
            string strTableLoc      = string.Empty;
            string strCarrierId     = string.Empty;
            string strToolId        = string.Empty;
            string strMgzId         = string.Empty;
            string strPortNo        = string.Empty;
			short nCntOfCP			= 0;
            short i                 = 0;
			uint nQty               = 0;

            int nIdx = -1;      // 2021.08.31 SungTae : [추가] Multi-LOT 관련
            
            // 2021-06-03 lhs Start
            string strREF_THK_MAX       = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB or AF)
            string strREF_THK_AVG       = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB or AF)
            // 2021-06-03 lhs End

            // 2021-07-23 lhs Start : CPNAME 추가 (SCK 전용)
            string strREF_PCB_THK_MAX   = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB Max)
            string strREF_PCB_THK_AVG   = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB Avg)
            string strREF_TOP_THK_MAX   = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (Top Mold Max)
            string strREF_TOP_THK_AVG   = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (Top Mold Avg)
            string strREF_BTM_THK_MAX   = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (Bottom Mold Max)
            string strREF_BTM_THK_AVG   = "";     // Eq에서 Host로 Upload한 값을 다시 받는 변수 (Bottom Mold Avg)
            // 2021-07-23 lhs End

            mgem.GetListItemOpen(lMsgId);
            mgem.GetAsciiItem(lMsgId, ref strRcmd);
            nCntOfCP = mgem.GetListItemOpen(lMsgId);

            for (i = 0; i < nCntOfCP; i++)
            {
                mgem.GetListItemOpen(lMsgId);
                mgem.GetAsciiItem(lMsgId, ref strCPName);

                if      (strCPName == GEM.sCPN_MATID)   {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);    strMatId        = strCPValue;           }   // A
                else if (strCPName == GEM.sCPN_TID)     {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);    strToolId       = strCPValue;           }   // A
                else if (strCPName == GEM.sCPN_LOTID)   {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);    strLotId        = strCPValue;           }   // A
                else if (strCPName == GEM.sCPN_TLoc)    {   mgem.GetU1Item(     lMsgId, ref nLocation);     strTableLoc     = nLocation.ToString(); }   // U1
                else if (strCPName == GEM.sCPN_CID)     {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);    strCarrierId    = strCPValue;           }
                else if (strCPName == GEM.sCPN_Qty)     {   mgem.GetU4Item(     lMsgId, ref nQty);          strQTY          = nQty.ToString();      }
                else if (strCPName == GEM.sCPN_PPID)    {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);    strPPID         = strCPValue;           }
                else if (strCPName == GEM.sCPN_MGZID)   {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);    strMgzId        = strCPValue;           }
                else if (strCPName == GEM.sCPN_PN)      {   mgem.GetU1Item(     lMsgId, ref nLocation);     strPortNo       = nLocation.ToString(); }
                
                // 2021-06-03 lhs Start : 변수명 변경
                else if (strCPName == GEM.sCPN_REF_THK_MAX)     { mgem.GetAsciiItem(lMsgId, ref strREF_THK_MAX); }
                else if (strCPName == GEM.sCPN_REF_THK_AVG)     { mgem.GetAsciiItem(lMsgId, ref strREF_THK_AVG); }
                // 2021-06-03 lhs End

                // 2021-07-23 lhs Start : CPNAME 추가 (SCK 전용)
                else if (strCPName == GEM.sCPN_REF_PCB_THK_MAX) { mgem.GetAsciiItem(lMsgId, ref strREF_PCB_THK_MAX); }
                else if (strCPName == GEM.sCPN_REF_PCB_THK_AVG) { mgem.GetAsciiItem(lMsgId, ref strREF_PCB_THK_AVG); }
                else if (strCPName == GEM.sCPN_REF_TOP_THK_MAX) { mgem.GetAsciiItem(lMsgId, ref strREF_TOP_THK_MAX); }
                else if (strCPName == GEM.sCPN_REF_TOP_THK_AVG) { mgem.GetAsciiItem(lMsgId, ref strREF_TOP_THK_AVG); }
                else if (strCPName == GEM.sCPN_REF_BTM_THK_MAX) { mgem.GetAsciiItem(lMsgId, ref strREF_BTM_THK_MAX); }
                else if (strCPName == GEM.sCPN_REF_BTM_THK_AVG) { mgem.GetAsciiItem(lMsgId, ref strREF_BTM_THK_AVG); }
                // 2021-07-23 lhs End

                else if (strCPName == GEM.sCPN_Result)
                {
                    mgem.GetListItemOpen(lMsgId);                  // <L
                    mgem.GetBoolItem(lMsgId,    ref bResult);        //      <BOOL
                    mgem.GetAsciiItem(lMsgId,   ref strCPValue);     //      <A
                    strErrorMessage = strCPValue;
                    mgem.GetListItemClose(lMsgId);                 // >

                    // 2022.04.26 SungTae Start : [수정] Log 확인 시 Result 값이 null 표시되어 수정 
                    //Log_wt(string.Format("RESULT : {0}", strErrorMessage));   // 2022.02.21 lhs
                    Log_wt($"RESULT : {bResult}, Description : {strErrorMessage}");
                    // 2022.04.26 SungTae End
                }
                else if (strCPName == GEM.sCPN_CL)  // Carrier List
                {
                    short nCntOfSubSt = mgem.GetListItemOpen(lMsgId);   // <L
                    
                    Log_wt(string.Format("LOT[{0}] Carrier List Info >> ", strLotId));

                    for (short j = 0; j < nCntOfSubSt; j++)
                    {
                        string strSubst = "";
                        mgem.GetAsciiItem(lMsgId, ref strSubst);        //      <A
                        
                        aSubSt.Add(strSubst);

                        Log_wt("Slot No : "+ (j+1) + ", Carrier ID = " + strSubst);
                    }
                    
                    mgem.GetListItemClose(lMsgId);                      // >
                }
                else //------------- 존재하지 않는 CPNAME ----------
                {
                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                    
                    Log_wt(string.Format("------ 존재하지 않는 CPNAME -----{0}", strCPName));
                }

                mgem.GetListItemClose(lMsgId);
            }

            mgem.GetListItemClose(lMsgId);
            mgem.GetListItemClose(lMsgId);

            if ((strRcmd != GEM.sRCMD_LOT_VERIFICATION)  && (strRcmd != GEM.sRCMD_CARRIER_VERIFICATION) &&
                (strRcmd != GEM.sRCMD_MAT_VERIFICATION)  && (strRcmd != GEM.sRCMD_PP_SELECT)            &&
                (strRcmd != GEM.sRCMD_TOOL_VERIFICATION) && (strRcmd != GEM.sRCMD_MGZ_VERIFICATION)     &&
                (strRcmd != GEM.sRCMD_Start)             && (strRcmd != GEM.sRCMD_Stop))
            {
                nHcack = (int)JSCK.eHCACK.DoesNotExist/*1*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
            }

            #region [SEND](H<-E) S2F42 NACK : 존재하지 않는 RCMD에 대한 NACK 처리 후 함수 종료
            if (nHcack != (int)JSCK.eHCACK.AckOK/*0*/)      // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
            {
                //존재하지 않는 RCMD인 경우, NACK를 보내고 함수를 종료한다. // 2019 05 10
                //S2F41의 NAC 응답을 먼저 보냄.  
                //-------------------------------------------------------------------------------
                //-------------------------------------------------------------------------------
                int sMsgId = mgem.CreateReplyMsg(lMsgId);

                mgem.OpenListItem(sMsgId);
                mgem.AddBinaryItem(sMsgId, nHcack);
                mgem.OpenListItem(sMsgId);
                mgem.CloseListItem(sMsgId);
                mgem.CloseListItem(sMsgId);
                mgem.SendMsg(sMsgId);
                

                // Nack 일경우 MAT 와 Tool 은 Clear 을 진행 한다.
                RCMD_Fail_Data_Clear(strTableLoc, strRcmd);
                return;
            }
            #endregion

            Log_wt($"[RECV](H->E) S2F41 RCMD = {strRcmd}.  Result : {bResult}");

            // nHcack 
            // 0 - OK, 1 - cmd Does Not Exist, 2- Can't Permomence now, 3- Para is Invalid, 4- Permon late, 5- already run reject;
            if (strRcmd == GEM.sRCMD_Stop)
            {
                if (CSQ_Main.It.m_iStat != EStatus.Auto_Running)
                {
                    Log_wt(string.Format("Auto Run 상태가 아닌데 Stop 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    
                    nHcack = (int)JSCK.eHCACK.Rejected/*5*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경         
                }
                else
                {
                    Log_wt(string.Format("Auto Run 상태 Stop 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    
                    CSQ_Main.It.m_bBtStop = true;

                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }

                CSQ_Main.It.m_nRCMDRecv = -1;   // HOST로부터 START외의 값을 수신받았다.
            }
            else if (strRcmd == GEM.sRCMD_Start)
            {
                if (CSQ_Main.It.m_iStat == EStatus.Error)
                {
                    Log_wt(string.Format("Error 일때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
                else if (CSQ_Main.It.m_iStat == EStatus.Manual)
                {
                    Log_wt(string.Format("Manual 구동 일때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
                else if ((CSQ_Main.It.m_iStat == EStatus.Idle) || (CSQ_Main.It.m_iStat == EStatus.Stop))
                {// 여기가 Start 성공 하는 곳
                    Log_wt(string.Format("Idle or Stop 일때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    
                    CSQ_Main.It.m_bBtStart = true;

                    // 2020-10-19, jhLee : SPIL CS, 설비 AUTO 전환시 RCMD 이용
                    CSQ_Main.It.m_nRCMDRecv = 1;   // HOST로부터 START 리모트명령을 수신받았다.
                    
                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
                else
                {
                    Log_wt(string.Format("Idle or Stop 아닐때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }                
            }
            else if (strRcmd == GEM.sRCMD_LOT_VERIFICATION)
            {
                //fGemMsg.Trace_Display("GEM.sRCMD_LV 수신됨 ");
                if (CData.SecsGem_Data.nStrip_Check != Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/)
                {
                    CData.SecsGem_Data.nStrip_Check = -1;// 시퀸스 에러 발생

                    // 2022.04.05 SungTae : [수정] 오타 수정
                    //Log_wt(string.Format("Carrier ID Verifaction 하지 않아는데 sRCMD_LV 내려옴."));
                    Log_wt(string.Format("Carrier ID Verifaction 하지 않았는데, sRCMD_LV 내려옴."));

                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
                else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/)
                {
                    if (bResult != true)
                    {
                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                        Log_wt(string.Format("Strip Verificatrion 요청하지 않았으나, Data 내려옴 {0}", CData.SecsGem_Data.nStrip_Check));

                        nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                    }
                    else
                    {
                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotSuccess)/*2*/;// Lot Varification

                        if (aSubSt.Count < 1)// Strip Count 0 이라서 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                            Log_wt(string.Format("Strip Count {0} 이라서 에러 ", aSubSt.Count));

                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                        }
                        else if (nQty < 1)// Strip Count 0 이라서 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                            Log_wt(string.Format("Strip Count nQty {0} 이라서 에러 ", nQty));

                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                        }
                        else if (strLotId == string.Empty)// Strip Lot ID 가 없어어 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                            Log_wt(string.Format("Strip Lot ID 가  {0} 이라서 에러 ", strLotId));

                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                        }
                        // 2022.04.28 SungTae Start : [추가] ASE-KH의 경우 LOT ID 비교 조건 추가
                        else if (CData.CurCompany == ECompany.ASE_K12 &&
                                 strLotId != CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLot_Name)  // Lot ID가 다르면 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                            Log_wt($"MAGAZINE_VERIFICATION Event로 받은 LOT ID[{CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLot_Name}]와 Strip Lot ID가 {strLotId} 서로 달라서 에러");

                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;
                        }
                        // 2022.04.28 SungTae End
                        else
                        {
                            // 2021.08.09 SungTae Start : [수정] Remote 상태에서의 Multi-LOT 동작 관련 수정
                            CData.OpenLotCnt++;     // LOT_VERIFICATION Event가 발생할 때마다 Count 증가 (Default = 0)

                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotMgr.LoadingLotName = CData.LotInfo.sLotName = strLotId;

                            Log_wt(string.Format("[RECV](H->E) Open LOT Count : " + CData.OpenLotCnt));
                            Log_wt(string.Format("[RECV](H->E) LOT ID : " + strLotId));
                            Log_wt(string.Format("[RECV](H->E) Carrier 수량 : " + strQTY));
                            Log_wt(string.Format("[RECV](H->E) 진행할 PPID : " + strPPID));

                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotSuccess)/*2*/;// Lot Varification 완료 Strip Varifaction 시퀸스 계속 진행
                                                                                                                    // 2021.08.09 SungTae End

                            SecLotOpen(strLotId, strQTY, strPPID);
                            
                            Log_wt(string.Format("(H->E) SECS/GEM LOT[ {0} ] OPEN!!! (by On-line REMOTE Status)", strLotId));
                        }
                    }
                }
            }
            else if (strRcmd == GEM.sRCMD_CARRIER_VERIFICATION)// Strip Varification
            {
                if ((CData.SecsGem_Data.nStrip_Check != Convert.ToInt32(JSCK.eChkStripVerify.LotSuccess)/*2*/) && (!CData.LotInfo.bLotOpen)) 
                {
                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                    
                    Log_wt(string.Format("Lot ID Verifaction 하지 않았는데 sRCMD_CV 내려옴."));
                    
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
                else 
                {
                    if (bResult != true)
                    {
                        // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                        //if (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID != strCarrierId)
                        //{
                        //    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                        //    Log_wt(string.Format("읽지 않은 Carrier ID 의 Carrier Validation이 내려옴."));
                        //}
                        //else
                        //{
                        //    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                        //    Log_wt(string.Format("else 읽지 않은 Carrier ID 의 Carrier Validation이 내려옴."));
                        //}

                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                        Log_wt(string.Format("읽지 않은 Carrier ID의 CARRIER_VERIFICATION이 내려옴."));

                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;
                        // 2022.04.01 SungTae End
                    }
                    else 
                    {
                        // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                        if (CData.IsMultiLOT())
                        {
                            nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                            // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                            if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                            {
                                if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                                {
                                    if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.IsExsitCarrier(strCarrierId) != true)
                                    {
                                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행

                                        Log_wt(string.Format("LOT List에 존재 하지 않는 Carrier ID 의 Carrier Validation이 내려옴."));

                                        nHcack = (int)JSCK.eHCACK.Rejected/*5*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                                    }
                                    else if (m_pReadCarrierList.IsExsitCarrier(strCarrierId) == true)
                                    {
                                        if (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID != strCarrierId)
                                        {
                                            CData.SecsGem_Data.qTerminal_Msg.Enqueue(string.Format("Strip ID Not Same : Up -> {0}, Down ->  {1}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, strCarrierId));

                                            sgfrmMsg.Display();

                                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                                        }
                                        else
                                        {
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = double.Parse(strREF_THK_MAX);  // lhs 확인 필요
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = double.Parse(strREF_THK_AVG);

                                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 진행

                                            CLog.mLogGnd(string.Format("FrmGEM  CData.SecsGem_Data.nStrip_Check = 3, CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = {0}, Measure Min = {1}, Max = {2}, Avg = {3}, Max/Avr = {4}",
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));

                                            Log_wt(string.Format("Host에서 내려온 Carrier ID : {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));

                                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                                        }
                                    }
                                    else
                                    {// only 1st Strip 
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;
                                        Log_wt(string.Format(" 00 CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));
                                    }
                                }
                                else
                                {
                                    if (m_sCCarID != strCarrierId)
                                    {
                                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                                        Log_wt("m_sCCarID != 읽지 않은 Carrier ID 의 Carrier Validation이 내려옴.");
                                    }
                                }
                            }
                            // 2022.06.14 SungTae End
                        }
                        else
                        {   // Multi-LOT 미사용 시
                            if (CData.m_pHostLOT.Count > 0)
                            {
                                if (CData.m_pHostLOT.IsExsitCarrier(strCarrierId) != true)
                                {
                                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                                    
                                    Log_wt(string.Format("LOT List에 존재 하지 않는 Carrier ID 의 Carrier Validation이 내려옴."));
                                    
                                    nHcack = (int)JSCK.eHCACK.Rejected/*5*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                                }
                                else if (m_pReadCarrierList.IsExsitCarrier(strCarrierId) == true)
                                {
                                    // 2020 01 30 skpark 
                                    // to do --> strDF_PCB_MIN, strDF_TOP_MIN , strDF_BOTTOM_MIN등 받은 값을 검증하여
                                    // 가능한 경우 작업 진행. 가능하지 않은 경우 Alarm 처리.
                                    if (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID != strCarrierId)
                                    {
                                        CData.SecsGem_Data.qTerminal_Msg.Enqueue(string.Format("Strip ID Not Same Up -> {0}, Down ->  {1}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, strCarrierId));
                                        sgfrmMsg.Display();
                                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                                    }
                                    else
                                    {
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;// 현재 내려 받은 Carrid ID
                                                                                                                                        
                                        // 2021.07.30 lhs : 변수 Comment
                                        // strREF_THK_MAX, strREF_THK_AVG 는 이전에 올린 값을 그대로 내려 받음 (하나의 변수로...ㅠㅠ)
                                        // Pcb값을 올렸으면 Pcb값을, Af값(Total/Mold)을 올렸으면 Af값을 내려 받음 

                                        // 2021.07.30 lhs Start : Host로부터 내려받은 Pcb, TopMold, Btm Mold // 2021.11.08 lhs 누락되어 다시 복원
                                        if (CDataOption.UseNewSckGrindProc)
                                        {
                                            //--------------------------------------------
                                            double dRefThkMax,       dRefThkAvg;

                                            double dRef_Pcb_Max,     dRef_Pcb_Avg;
                                            double dRef_TopMold_Max, dRef_TopMold_Avg;
                                            double dRef_BtmMold_Max, dRef_BtmMold_Avg;

                                            //--------------------------------------------
                                            double.TryParse(strREF_THK_MAX, out dRefThkMax);
                                            double.TryParse(strREF_THK_AVG, out dRefThkAvg);

                                            // TopS or BtmS 인 경우는 Pcb 값 (Pcb을 측정하여 올린 값을 다시 내려받음)
                                            // TopD or BtmD 인 경우는 이전모드의 Total Thickness
                                            //  - 이전모드가 TopS 이었으면 strREF_THK_AVG는 Total (TopMold + Pcb)
                                            //  - 이전모드가 BtmS 이었으면 strREF_THK_AVG는 Total (BtmMold + Pcb)
                                            //  - 이전모드가 TopD 이었으면 strREF_THK_AVG는 Total (TopMold + Pcb + BtmMold)
                                            //  - 이전모드가 BtmD 이었으면 strREF_THK_AVG는 Total (TopMold + Pcb + BtmMold)
                                            if (CData.Dev.eMoldSide == ESide.Top)  // TopS
                                            {
                                                // Pcb
                                            }
                                            else if (CData.Dev.eMoldSide == ESide.BtmS) // BtmS
                                            {
                                                // Pcb
                                            }
                                            else if (CData.Dev.eMoldSide == ESide.TopD) // TopD
                                            {
                                                // 이전모드의 Total Thickness
                                            }
                                            else if (CData.Dev.eMoldSide == ESide.Btm)  // BtmD
                                            {
                                                // 이전모드의 Total Thickness
                                            }
                                            //--------------------------------------------
                                            // Pcb, TopMold, BtmMold 다운로드
                                            double.TryParse(strREF_PCB_THK_MAX, out dRef_Pcb_Max);
                                            double.TryParse(strREF_PCB_THK_AVG, out dRef_Pcb_Avg);

                                            double.TryParse(strREF_TOP_THK_MAX, out dRef_TopMold_Max);
                                            double.TryParse(strREF_TOP_THK_AVG, out dRef_TopMold_Avg);
                                            double.TryParse(strREF_BTM_THK_MAX, out dRef_BtmMold_Max);
                                            double.TryParse(strREF_BTM_THK_AVG, out dRef_BtmMold_Avg);

                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0]     = dRef_Pcb_Max;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0]     = dRef_Pcb_Avg;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = dRef_Pcb_Max;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = dRef_Pcb_Avg;

                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max     = dRef_TopMold_Max;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg     = dRef_TopMold_Avg;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max     = dRef_BtmMold_Max;
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg     = dRef_BtmMold_Avg;
                                            //--------------------------------------------
                                        }
                                        else    // 기존 로직
                                        {
                                            // 2020.09.11 JSKim St
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = double.Parse(strREF_THK_MAX);
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = double.Parse(strREF_THK_AVG);
                                            // 2020.09.11 JSKim Ed
                                        }

                                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 진행

                                        if (CDataOption.UseNewSckGrindProc)
                                        {
                                            CLog.mLogGnd(string.Format("FrmGEM  CData.SecsGem_Data.nStrip_Check = 3, CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = {0}, Measure Min = {1}, Max = {2}, Avg = {3}, Max/Avr = {4}, TopMold Max = {5}, TopMold Avg = {6}, BtmMold Max = {7}, BtmMold Avg = {8}",
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode,
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Max,
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_TopMold_Avg,
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Max,
                                                                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].dMeasure_BtmMold_Avg));
                                        }
                                        else
                                        {
                                            CLog.mLogGnd(string.Format("FrmGEM  CData.SecsGem_Data.nStrip_Check = 3, CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = {0}, Measure Min = {1}, Max = {2}, Avg = {3}, Max/Avr = {4}",
                                                                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));
                                        }

                                        Log_wt(string.Format("Host에서 내려온 Carrier ID : {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));

                                        nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                                        //20191023
                                        ////Pick 승인
                                        //CSq_OnP.It.m_bPickOK = true;
                                    }
                                }
                                else
                                {// only 1st Strip 
                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;
                                    Log_wt(string.Format(" 00 CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));
                                }
                            }
                            else
                            {
                                if (m_sCCarID != strCarrierId)
                                {
                                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                                    Log_wt("m_sCCarID != 읽지 않은 Carrier ID 의 Carrier Validation이 내려옴.");
                                }
                            }
                        }
                    }
                }
            }
            else if (strRcmd == GEM.sRCMD_PP_SELECT)
            {
                if (isExistRecipe(strPPID) == true)
                {
                    if (!CData.LotInfo.bLotOpen)
                    {
                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name = strPPID;// 현재 내려 받은 Recipe ID
                        
                        Log_wt(string.Format(" {0} Recipe 내려옴.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name));
                        
                        nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                        // m_sPPID = strPPID;                     
                    }
                    else
                    {
                        nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                        //m_sPPID = strPPID;
                    }
                }
                else
                {// 레시피가 존재하지 않음
                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
            }
            else if (strRcmd == GEM.sRCMD_TOOL_VERIFICATION)
            {
                if (bResult == true)
                {                    
                    if (strTableLoc == "1")
                    {
                        if (strToolId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            Log_wt(string.Format("nHcack = 3; 다운로드 Tool ID {0} != sNew_Tool_Name[0]  {1}", strToolId, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0]));
                            
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0] = "";
                        }
                        else
                        {
                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0] = strToolId;// 현재 내려 받은 Tool ID
                            Log_wt(string.Format("1 > {0} Lot ID  내려옴.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));
                        }
                    }
                    else if (strTableLoc == "2")
                    {
                        if (strToolId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            Log_wt(string.Format("nHcack = 3; 다운로드 Tool ID {0} != sNew_Tool_Name[1]  {1}", strToolId, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1]));
                            
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] = "";
                        }
                        else
                        {
                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] = strToolId;// 현재 내려 받은 Tool ID
                            
                            Log_wt(string.Format("2 >  {0} Lot ID  내려옴.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));
                        }
                    }
                    else
                    {
                        Log_wt(string.Format("nHcack = 3; 다운로드 strTableLoc {0} ", strTableLoc));

                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                    }
                }
                else 
                {
                    CData.SecsGem_Data.qTerminal_Msg.Enqueue(strErrorMessage);
                    
                    sgfrmMsg.Display();

                    //                  sgfrmMsg.AddString(strErrorMessage);
                    //                  sgfrmMsg.Show();
                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
            }
            else if (strRcmd == GEM.sRCMD_MAT_VERIFICATION)
            {//999601 Dress Verify 응답에 대한 예외처리
                if (bResult == true)
                {
                    if (strTableLoc == "1")
                    {
                        if (strMatId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[0])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] = "";
                        }
                        else
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] = strMatId;// 현재 내려 받은 Dress ID
                        }
                    }
                    else if (strTableLoc == "2")
                    {
                        if (strMatId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[1])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] = "";
                        }
                        else
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] = strMatId;// 현재 내려 받은 Dress ID
                        }
                    }
                }
                else 
                {
                    CData.SecsGem_Data.qTerminal_Msg.Enqueue(strErrorMessage);
                    
                    sgfrmMsg.Display();

                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
            }
            else if (strRcmd == GEM.sRCMD_MGZ_VERIFICATION)
            {
                if (bResult == true) 
                {
                    if      (strPortNo == "3")   // Loader Port
                    {
                        if (strMgzId != CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID)
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;
                        else
                            CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID_New = strMgzId;

                        // 2022.04.26 SungTae Start : [추가] ASE-KH Multi-LOT(Auto-In/Out) 관련
                        if (CData.CurCompany == ECompany.ASE_K12)   { CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLot_Name = strLotId; }
                        // 2022.04.26 SungTae End
                    }
                    else if (strPortNo == "1")     // Unloader Good Port
                    {
                        if (strMgzId != CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID)
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;
                        else
                            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New = strMgzId;
                    }
                    else if (strPortNo == "2")       // Unloader NG Port
                    {
                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;
                    }
                    else 
                    {
                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;
                    }
                }
                else 
                {
                    if (strPortNo == "3") 
                    {
                        CData.SecsGem_Data.nLdMGZ_Check = (int)JSCK.eChkVerify.Error/*-1*/;// Loader 시퀸스 에러
                    }
                    else 
                    {
                        CData.SecsGem_Data.nMGZ_Check = (int)JSCK.eChkVerify.Error/*-1*/;// Unloader 시퀸스 에러
                    }

                    CData.SecsGem_Data.qTerminal_Msg.Enqueue(strErrorMessage);
                    
                    sgfrmMsg.Display();

                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
                }
            }

            //-------------------------------------------------------------------------------
            //S2F41의 ACK 응답을 먼저 보냄. ---------------------------------------------------
            //-------------------------------------------------------------------------------
            #region [SEND](H<-E) S2F42 Host Command Acknowledge (ACK or NACK)
            int rMsgId = mgem.CreateReplyMsg(lMsgId);

            mgem.OpenListItem(rMsgId);
            mgem.AddBinaryItem(rMsgId, nHcack);
            mgem.OpenListItem(rMsgId);
            mgem.CloseListItem(rMsgId);
            mgem.CloseListItem(rMsgId);
            mgem.SendMsg(rMsgId);

            Log_wt($"[SEND](H<-E) S2F42 HACK = {nHcack}, Result : {bResult}");

            #endregion

            //-------------------------------------------------------------------------------
            //-Data 변경에 대한 회신 보내는 곳 ------------------------------------------------
            //-------------------------------------------------------------------------------

            if ((strRcmd == GEM.sRCMD_LOT_VERIFICATION) && (bResult == true) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/))  // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
            {
                // 2021.07.19 SungTae Start : [수정] Data Shift 시 확인이 용이하도록 수정
                // 장비에서 Start -> Strip 보고 -> Lot 정보 내려 주는 곳
                // 2. Lot ID - Varification 완료
                //CData.m_pHostLOT.DeleteAll();

                AddStripFromHost(aSubSt, CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);//

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID)         , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID) , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    CData.LotMgr.LoadingLotName = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name;
                }
                else
                {
                    CData.LotInfo.sLotName = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name;
                }

                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  LOT : {2}, Carrier ID : {3}" , Convert.ToInt32(JSCK.eCEID.LOT_Verified)
                                                                                                        , JSCK.eCEID.LOT_Verified.ToString()
                                                                                                        , strLotId, m_sReadLOT_T));

                m_sReadLOT_T = ""; // 임시변수 초기화

                m_pReadCarrierList.SetLotID(strLotId);
                m_pStartCarrierList.SetLotID(strLotId);
                m_pValidCarrierList.SetLotID(strLotId);
                m_pEndedCarrierList.SetLotID(strLotId);
                m_pRejectCarrierList.SetLotID(strLotId);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Verified).ToString());

                Log_wt(string.Format("LOT Verification 완료 보고"));    // 2022.04.14 SungTae : [추가] Log 추가

                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(strLotId);

                    // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                    {
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sInRail_Strip_ID, (int)JSCK.eCarrierStatus.Idle);

                        sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    }
                    // 2022.06.14 SungTae End
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, (int)JSCK.eCarrierStatus.Read);

                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                }
            }
            else if ((strRcmd == GEM.sRCMD_CARRIER_VERIFICATION) && (bResult == true) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/))   // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
            {
                // 1. Strip ID 보고 완료
                // 2. Lot ID - Varification 완료
                // 3. Strip Varication 완료 후  투입 시작
                
                // 3- 1) Strip Varification 완료 보고
                Log_wt(string.Format("3- 1) Strip Verification 완료 보고 "));

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.LotMgr.CheckExistLOT(strLotId) == false)
                {
                    m_pValidCarrierList.ClearCarrier();
                }

                m_pValidCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);

                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(strLotId);

                    // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                    {
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Validate));
                    }
                    // 2022.06.14 SungTae End
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Validate));
                }
                // 2021.08.25 SungTae End

                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verified).ToString());

                // 2021.08.27 SungTae Start : [추가]
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Carrier ID : {2}", Convert.ToInt32(JSCK.eCEID.Carrier_Verified)
                                                                                            , JSCK.eCEID.Carrier_Verified.ToString()
                                                                                            , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));
                // 2021.08.27 SungTae End

                // 3- 2) JSCK SECSGEM 사용시 Strip Varification 진행 후 Pick 승인 완료 하여 작업 시작
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행
                //CSq_OnP.It.m_bPickOK = true; 20200910 삭제 LCY
                
                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if(CData.IsMultiLOT())
                {
                    // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                    {
                        sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    }
                    // 2022.06.14 SungTae End
                }
                else
                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                // 2021.08.25 SungTae End
            }
            else if ((strRcmd == GEM.sRCMD_PP_SELECT) && (nHcack == 0))
            {
                // 실제 레시피가 변경되는 부분. 
                // 20190625 josh
                // 레시피 변경 지령이 들어온 상황
                // m_sPPID로 디바이스 변경해야 함.
                //                
                Log_wt(string.Format("레시피 변경 지령이 들어온 상황"));

                CData.FrmMain.m_vwRcp.LoadDeviceSecs(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_PPID),  CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID),          CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Loaded).ToString());

                // 2021.08.27 SungTae Start : [추가]
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  New Device Name : {2}", Convert.ToInt32(JSCK.eCEID.Recipe_Loaded)
                                                                                            , JSCK.eCEID.Recipe_Loaded.ToString()
                                                                                            , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name));
                // 2021.08.27 SungTae End
            }
            else if ((strRcmd == GEM.sRCMD_TOOL_VERIFICATION) && (nHcack == 0) && (bResult == true))
            {
                // Tool, Wheel Verifaction 진행 
                // 장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신
                Log_wt(string.Format("장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신"));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID), "");
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID), "");

                if (strTableLoc == "1")
                {
                    CData.LotInfo.sCurr_LToolId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0];
                    CData.LotInfo.sLToolId      = CData.LotInfo.sCurr_LToolId;

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID),    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Wheel_Name[0]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID),     CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                }
                else if (strTableLoc == "2")
                {
                    CData.LotInfo.sCurr_RToolId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1];
                    CData.LotInfo.sRToolId      = CData.LotInfo.sCurr_RToolId;

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID),    CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Wheel_Name[1]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID),     CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID)              , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID)              , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_Loc)                , strTableLoc);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc)               , strTableLoc);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Verified).ToString());
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Changed).ToString());

                // 2021.08.27 SungTae Start : [추가]
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Tool_1_ID : {2}, Tool_2_ID : {3}", Convert.ToInt32(JSCK.eCEID.Tool_Verified),
                                                                                                            JSCK.eCEID.Tool_Verified.ToString(),
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0],
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  Tool_1_ID : {2}, Tool_2_ID : {3}", Convert.ToInt32(JSCK.eCEID.Tool_Changed),
                                                                                                            JSCK.eCEID.Tool_Changed.ToString(),
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0],
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]));
                // 2021.08.27 SungTae End
            }
            else if ((strRcmd == GEM.sRCMD_MAT_VERIFICATION) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/) && (bResult == true))   // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
            {
                // Dress Verifaction 진행 
                // 장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신
                Log_wt(string.Format("장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신"));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), "");
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), "");

                if (strTableLoc == "1")
                {
                    CData.LotInfo.sCurr_LDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0];
                    //CData.LotInfo.sCurr_LDrsId = strMatId;
                    CData.LotInfo.sLDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0];

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0]);
                }
                else if (strTableLoc == "2")
                {
                    CData.LotInfo.sCurr_RDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1];
                    //CData.LotInfo.sCurr_RDrsId = strMatId;
                    CData.LotInfo.sRDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1];

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]);
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL_Loc), strTableLoc);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc), strTableLoc);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Verified).ToString());
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Changed).ToString());

                // 2021.08.27 SungTae Start : [추가]
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  MATL1_ID : {2}, MATL2_ID : {3}", Convert.ToInt32(JSCK.eCEID.Material_Verified),
                                                                                                            JSCK.eCEID.Material_Verified.ToString(),
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0],
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]));
                Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  MATL1_ID : {2}, MATL2_ID : {3}", Convert.ToInt32(JSCK.eCEID.Material_Changed),
                                                                                                            JSCK.eCEID.Material_Changed.ToString(),
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0],
                                                                                                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]));
                // 2021.08.27 SungTae End
            }
            else if ((strRcmd == GEM.sRCMD_MGZ_VERIFICATION) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/) && (bResult == true))
            {
                // MGZ Verifaction 진행 
                // 장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신                
                if (strPortNo == "3") 
                {
                    #region Loader MGZ Verified
                    // Loader Mgz Verifyed 
                    Log_wt(string.Format("장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신 , strPortNo -> 3"));
                    
                    CData.SecsGem_Data.nLdMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Complete)/*2*/;// 시퀸스 계속 진행

                    // 2022.04.26 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련
                    //CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New;
                    if (CData.CurCompany == ECompany.ASE_K12)
                        CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLot_Name;
                    else
                        CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New;

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), strPortNo);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Started).ToString());

                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  LD Magazine_ID : {2}, MGZ_Port_Number : {3}", Convert.ToInt32(JSCK.eCEID.Magazine_Verified),
                                                                                                                           JSCK.eCEID.Magazine_Verified.ToString(),
                                                                                                                           CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New,
                                                                                                                           strPortNo));
                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).", Convert.ToInt32(JSCK.eCEID.Magazine_Started), JSCK.eCEID.Magazine_Started.ToString()));
                    #endregion
                }
                else 
                {
                    #region UnLoader MGZ Verified
                    // Unloader Mgz Verifyed
                    if (strPortNo == "1") 
                    {
                        Log_wt(string.Format("장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신 , strPortNo -> 1"));
                        
                        CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New;
                        
                        Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);

                        m_pMgzGoodCarrierList.ClearCarrier();
                        m_pMgzGoodCarrierList.SetMgzID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                        m_pMgzGoodCarrierList.SetLotID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                    }
                    else if (strPortNo == "2") 
                    {
                        Log_wt(string.Format("장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신, strPortNo = 2"));

                        CData.LotInfo.sNMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New;
                        
                        Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);

                        m_pMgzNGCarrierList.ClearCarrier();                                 // 2019 11 27 skpark
                        m_pMgzNGCarrierList.SetMgzID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);  // unload mgz id 
                        m_pMgzNGCarrierList.SetLotID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                    }

                    CData.SecsGem_Data.nMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Complete)/*2*/;// 시퀸스 계속 진행

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), strPortNo);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Started).ToString());

                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).  ULD Magazine_ID : {2}, MGZ_Port_Number : {3}", Convert.ToInt32(JSCK.eCEID.Magazine_Verified),
                                                                                                                           JSCK.eCEID.Magazine_Verified.ToString(),
                                                                                                                           CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLD_MGZ_ID_New,
                                                                                                                           (strPortNo == "1") ? "GOOD" : "NG"));
                    Log_wt(string.Format("[SEND](H<-E) S6F11 CEID = {0}({1}).", Convert.ToInt32(JSCK.eCEID.Magazine_Started), JSCK.eCEID.Magazine_Started.ToString()));
                    #endregion
                }

            }
        }
        #endregion

        #region S2F49 메세지
        public void OnS2F49(int lMsgId)
        {// OnRemoteCommand
            List<string> aSubSt  = new List<string>();
            List<byte>   aCPAck  = new List<byte>();
            List<string> aCPName = new List<string>();
            
            Log_wt(string.Format("OnS2F49()"));

            bool bResult = false;
            byte nLocation = 0x00;
            byte nHcack = 0x00;

            string strRcmd          = string.Empty;
            string strCPName        = string.Empty;
            string strCPValue       = string.Empty;
            string strMatId         = string.Empty;
            string strLotId         = string.Empty;
            string strPPID          = string.Empty;
            string strQTY           = string.Empty;
            string strErrorMessage  = string.Empty;
            string strTableLoc      = string.Empty;
            string strCarrierId     = string.Empty;
            string strToolId        = string.Empty;
            string strMgzId         = string.Empty;
            string strPortNo        = string.Empty;

            short nCntOfCP = 0;
            short i = 0;
            uint nQty = 0;

            int nIdx = -1;      // 2021.08.30 SungTae : [추가] Multi-LOT 관련

            // 2021-06-03 lhs Start : 변수명 변경
            string strREF_THK_MAX = "";   // Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB or AF)
            string strREF_THK_AVG = "";   // Eq에서 Host로 Upload한 값을 다시 받는 변수 (PCB or AF)
            // 2021-06-03 lhs End

            mgem.GetListItemOpen(lMsgId);

            uint nDataId = 0;               //사용하지 않으므로 변수에 넣지만 쓰지 않음.
            string strSpec = "";            // 사용하지 않으므로 변수에 넣지만 쓰지 않음.

            mgem.GetU4Item(lMsgId, ref nDataId);
            mgem.GetAsciiItem(lMsgId, ref strSpec);

            mgem.GetAsciiItem(lMsgId, ref strRcmd);
            nCntOfCP = mgem.GetListItemOpen(lMsgId);

            for (i = 0; i < nCntOfCP; i++)
            {
                mgem.GetListItemOpen(lMsgId);
                mgem.GetAsciiItem(lMsgId, ref strCPName);

                if      (strCPName == GEM.sCPN_MATID)       {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);     strMatId       = strCPValue;           }
                else if (strCPName == GEM.sCPN_TID)         {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);     strToolId      = strCPValue;           }
                else if (strCPName == GEM.sCPN_LOTID)       {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);     strLotId       = strCPValue;           }
                else if (strCPName == GEM.sCPN_TLoc)        {   mgem.GetU1Item(     lMsgId, ref nLocation);      strTableLoc    = nLocation.ToString(); }
                else if (strCPName == GEM.sCPN_CID)         {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);     strCarrierId   = strCPValue;           }
                else if (strCPName == GEM.sCPN_Qty)         {   mgem.GetU4Item(     lMsgId, ref nQty);           strQTY         = nQty.ToString();      }
                else if (strCPName == GEM.sCPN_PPID)        {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);     strPPID        = strCPValue;           }
                else if (strCPName == GEM.sCPN_MGZID)       {   mgem.GetAsciiItem(  lMsgId, ref strCPValue);     strMgzId       = strCPValue;           }
                else if (strCPName == GEM.sCPN_PN)          {   mgem.GetU1Item(     lMsgId, ref nLocation);      strPortNo      = nLocation.ToString(); }
                // 2021-06-03 lhs Start : 변수명 변경
                else if (strCPName == GEM.sCPN_REF_THK_MAX) { mgem.GetAsciiItem(lMsgId, ref strREF_THK_MAX); }
                else if (strCPName == GEM.sCPN_REF_THK_AVG) { mgem.GetAsciiItem(lMsgId, ref strREF_THK_AVG); }
                // 2021-06-03 lhs End
                else if (strCPName == GEM.sCPN_Result)
                {
                    mgem.GetListItemOpen(lMsgId);                  // <L
                    mgem.GetBoolItem(lMsgId, ref bResult);         //      <BOOL
                    mgem.GetAsciiItem(lMsgId, ref strCPValue);     //      <A
                    strErrorMessage = strCPValue;
                    mgem.GetListItemClose(lMsgId);                 // >
                }
                else if (strCPName == GEM.sCPN_CL)
                {
                    short nCntOfSubSt = 0;
                    nCntOfSubSt = mgem.GetListItemOpen(lMsgId);    // <L

                    for (short j = 0; j < nCntOfSubSt; j++)
                    {
                        string strSubst = "";
                        mgem.GetAsciiItem(lMsgId, ref strSubst);   //      <A
                        aSubSt.Add(strSubst);
                        Log_wt(strSubst);
                    }

                    mgem.GetListItemClose(lMsgId);                 // >
                }
                else 
                {//------------- 존재하지 않는 CPNAME ----------
                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경

                    Log_wt(string.Format("------ 존재하지 않는 CPNAME -----{0}", strCPName));
                }

                mgem.GetListItemClose(lMsgId);
            }

            mgem.GetListItemClose(lMsgId);
            mgem.GetListItemClose(lMsgId);

            if ((strRcmd != GEM.sRCMD_LOT_VERIFICATION) && (strRcmd != GEM.sRCMD_CARRIER_VERIFICATION) &&
                (strRcmd != GEM.sRCMD_MAT_VERIFICATION) && (strRcmd != GEM.sRCMD_PP_SELECT) &&
                (strRcmd != GEM.sRCMD_TOOL_VERIFICATION) && (strRcmd != GEM.sRCMD_MGZ_VERIFICATION) &&
                (strRcmd != GEM.sRCMD_Start) && (strRcmd != GEM.sRCMD_Stop))
            {
                nHcack = 1;
            }

            if (nHcack != 0)
            {
                //존재하지 않는 RCMD인 경우, NACK를 보내고 함수를 종료한다. // 2019 05 10
                //S2F41의 NAC 응답을 먼저 보냄.  
                //-------------------------------------------------------------------------------
                //-------------------------------------------------------------------------------
                int sMsgId = mgem.CreateReplyMsg(lMsgId);

                mgem.OpenListItem(sMsgId);
                mgem.AddBinaryItem(sMsgId, nHcack);
                mgem.OpenListItem(sMsgId);
                mgem.CloseListItem(sMsgId);
                mgem.CloseListItem(sMsgId);
                mgem.SendMsg(sMsgId);

                // Nack 일경우 MAT 와 Tool 은 Clear 을 진행 한다.
                RCMD_Fail_Data_Clear(strTableLoc, strRcmd);
                return;
            }

            Log_wt(string.Format("[RECV][H->E] S2F49 RCMD = " + strRcmd));      // 2021.08.23 SungTae : [추가]

            // nHcack 
            // 0 - OK, 1 - cmd Does Not Exist, 2- Can't Permomence now, 3- Para is Invalid, 4- Permon late, 5- already run reject;
            if (strRcmd == GEM.sRCMD_Stop)
            {
                if (CSQ_Main.It.m_iStat != EStatus.Auto_Running)
                {
                    Log_wt(string.Format("Auto Run 상태가 아닌데 Stop 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    nHcack = (int)JSCK.eHCACK.Rejected/*5*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
                else
                {
                    Log_wt(string.Format("Auto Run 상태 Stop 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    CSQ_Main.It.m_bBtStop = true;
                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;       // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
            }
            else if (strRcmd == GEM.sRCMD_Start)
            {
                if (CSQ_Main.It.m_iStat == EStatus.Error)
                {
                    Log_wt(string.Format("Error 일때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
                else if (CSQ_Main.It.m_iStat == EStatus.Manual)
                {
                    Log_wt(string.Format("Manual 구동 일때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
                else if (CSQ_Main.It.m_iStat == EStatus.Idle)
                {// 여기가 Start 성공 하는 곳
                    Log_wt(string.Format("Idle 일때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    CSQ_Main.It.m_bBtStart = true;
                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;           // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
                else
                {
                    Log_wt(string.Format("Idle 아닐때 Start 명령 내려옴. CSQ_Main.It.m_iStat {0}", CSQ_Main.It.m_iStat));
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;   // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
            }
            else if (strRcmd == GEM.sRCMD_LOT_VERIFICATION)
            {
                //fGemMsg.Trace_Display("GEM.sRCMD_LV 수신됨 ");
                if (CData.SecsGem_Data.nStrip_Check != Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/)
                {
                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.ReqError)/*-1*/;// 시퀸스 에러 발생
                    Log_wt(string.Format("Carrier ID Verifaction 하지 않아는데 sRCMD_LV 내려옴."));
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
                else if (CData.SecsGem_Data.nStrip_Check == Convert.ToInt32(JSCK.eChkStripVerify.InitVerify)/*1*/)
                {
                    if (bResult != true)
                    {
                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                        Log_wt(string.Format("Strip Verificatrion 요청 하지 않았으나 Data 내려옴 {0}", CData.SecsGem_Data.nStrip_Check));
                        nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                    }
                    else
                    {
                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotSuccess)/*2*/;// Lot Varification
                        
                        if (aSubSt.Count < 1)// Strip Count 0 이라서 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생
                            Log_wt(string.Format("Strip Count {0} 이라서 에러 ", aSubSt.Count));
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정

                        }
                        else if (nQty < 1)// Strip Count 0 이라서 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                            Log_wt(string.Format("Strip Count nQty {0} 이라서 에러 ", nQty));
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        }
                        else if (strLotId == string.Empty)// Strip Lot ID 가 없어어 에러
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotError)/*-2*/;// 시퀸스 에러 발생

                            Log_wt(string.Format("Strip Lot ID 가  {0} 이라서 에러 ", strLotId));
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        }
                        else //if (m_pHostLOT.Count > 0) 이건 뭐지??
                        {
                            // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name = CData.LotMgr.LoadingLotName = CData.LotInfo.sLotName = strLotId;//20200513 현재 내려 받은 Lot ID
                            // 2021.08.25 SungTae End

                            Log_wt(string.Format("현재 내려 받은 Carrier ID : " + CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));
                            
                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정

                            //20190925
                            //정상적인 LOT OPEN
                            //LOT 정보 획득
                            //LOT name
                            //LOT Qty
                            //Device name
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.LotSuccess)/*2*/;// Lot Varification 완료 Strip Varifaction 시퀸스 계속 진행
                            
                            SecLotOpen(strLotId, strQTY, strPPID);
                        }
                    }
                }
            }
            else if (strRcmd == GEM.sRCMD_CARRIER_VERIFICATION)// Strip Varification
            {
                if ((CData.SecsGem_Data.nStrip_Check != Convert.ToInt32(JSCK.eChkStripVerify.LotSuccess)/*2*/) && (!CData.LotInfo.bLotOpen))
                {
                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                    Log_wt(string.Format("Lot ID Verifaction 하지 않아는데 sRCMD_CV 내려옴."));
                    nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
                else
                {
                    if (bResult != true)
                    {
                        if (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID != strCarrierId)
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                            Log_wt(string.Format("읽지 않은 Carrier ID 의 Carrier Validation이 내려옴."));
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        }
                        else
                        {
                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                            Log_wt(string.Format("else 읽지 않은 Carrier ID 의 Carrier Validation이 내려옴."));
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        }
                    }
                    else
                    {
                        // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                        if (CData.IsMultiLOT())
                        {
                            nIdx = CData.LotMgr.GetListIndexNum(CData.LotMgr.LoadingLotName);

                            // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                            if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                            {
                                if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.Count > 0)
                                {
                                    if (CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.IsExsitCarrier(strCarrierId) != true)
                                    {
                                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                                        Log_wt(string.Format("LOT List에 존재 하지 않는 Carrier ID 의 Carrier Validation이 내려옴."));
                                        nHcack = (int)JSCK.eHCACK.Rejected/*5*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                    }
                                    else if (m_pReadCarrierList.IsExsitCarrier(strCarrierId) == true)
                                    {
                                        if (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID != strCarrierId)
                                        {
                                            CData.SecsGem_Data.qTerminal_Msg.Enqueue(string.Format("Strip ID Not Same Up -> {0}, Down ->  {1}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, strCarrierId));
                                            sgfrmMsg.Display();
                                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                        }
                                        else
                                        {
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;// 현재 내려 받은 Carrid ID
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = double.Parse(strREF_THK_MAX);
                                            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = double.Parse(strREF_THK_AVG);

                                            CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 진행

                                            CLog.mLogGnd(string.Format("FrmGEM  CData.SecsGem_Data.nStrip_Check = 3, CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = {0}, Measure Min = {1}, Max = {2}, Avg = {3}, Max/Avr = {4}",
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                                     CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));
                                            Log_wt(string.Format("{0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID + "Carrier ID  내려옴."));
                                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                        }
                                    }
                                    else
                                    {// only 1st Strip 
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;
                                        Log_wt(string.Format(" 00 CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));
                                    }
                                }
                            }
                            // 2022.06.14 SungTae End
                            else
                            {
                                if (m_sCCarID != strCarrierId)
                                {
                                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                    Log_wt("m_sCCarID != 읽지 않은 Carrier ID 의 Carrier Validation이 내려옴.");
                                }
                            }
                        }
                        else
                        {   // Multi-LOT 미사용 시
                            if (CData.m_pHostLOT.Count > 0)
                            {
                                if (CData.m_pHostLOT.IsExsitCarrier(strCarrierId) != true)
                                {
                                    CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripError)/*-3*/;// 시퀸스 에러 진행
                                    Log_wt(string.Format("LOT List에 존재 하지 않는 Carrier ID 의 Carrier Validation이 내려옴."));
                                    nHcack = (int)JSCK.eHCACK.Rejected/*5*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                }
                                else if (m_pReadCarrierList.IsExsitCarrier(strCarrierId) == true)
                                {
                                    // 2020 01 30 skpark 
                                    // to do --> strDF_PCB_MIN, strDF_TOP_MIN , strDF_BOTTOM_MIN등 받은 값을 검증하여
                                    // 가능한 경우 작업 진행. 가능하지 않은 경우 Alarm 처리.
                                    if (CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID != strCarrierId)
                                    {
                                        CData.SecsGem_Data.qTerminal_Msg.Enqueue(string.Format("Strip ID Not Same Up -> {0}, Down ->  {1}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, strCarrierId));
                                        sgfrmMsg.Display();
                                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                    }
                                    else
                                    {
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;// 현재 내려 받은 Carrid ID
                                                                                                                                        // 2020.09.11 JSKim St
                                                                                                                                        //CData.JSCK_Gem_Data[4].fMeasure_New_Max_Data[0]  = double.Parse(strDF_PCB_MAX);
                                                                                                                                        //CData.JSCK_Gem_Data[4].fMeasure_New_Avr_Data[0]  = double.Parse(strDF_PCB_AVG);
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Max_Data[0] = double.Parse(strREF_THK_MAX);
                                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0] = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_New_Avr_Data[0] = double.Parse(strREF_THK_AVG);
                                        // 2020.09.11 JSKim Ed
                                        CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 진행
                                        CLog.mLogGnd(string.Format("FrmGEM  CData.SecsGem_Data.nStrip_Check = 3, CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID = {0}, Measure Min = {1}, Max = {2}, Avg = {3}, Max/Avr = {4}",
                                                                                 CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID,
                                                                                 CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Min_Data[0],
                                                                                 CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Max_Data[0],
                                                                                 CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].fMeasure_Avr_Data[0],
                                                                                 CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].nDF_User_New_Mode));
                                        Log_wt(string.Format("{0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID + "Carrier ID  내려옴."));
                                        nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                        //20191023
                                        ////Pick 승인
                                        //CSq_OnP.It.m_bPickOK = true;
                                    }
                                }
                                else
                                {// only 1st Strip 
                                    CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID = strCarrierId;
                                    Log_wt(string.Format(" 00 CData.JSCK_Gem_Data[4].sInRail_Verified_Strip_ID {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID));
                                }
                            }
                            else
                            {
                                if (m_sCCarID != strCarrierId)
                                {
                                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                                    Log_wt("m_sCCarID != 읽지 않은 Carrier ID 의 Carrier Validation이 내려옴.");
                                }
                            }
                        }
                        // 2021.08.25 SungTae End
                    }
                }
            }
            else if (strRcmd == GEM.sRCMD_PP_SELECT)
            {
                if (isExistRecipe(strPPID) == true)
                {
                    if (!CData.LotInfo.bLotOpen)
                    {
                        CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name = strPPID;// 현재 내려 받은 Recipe ID
                        Log_wt(string.Format(" {0} Recipe 내려옴.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name));
                        nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        // m_sPPID = strPPID;                     
                    }
                    else
                    {
                        nHcack = (int)JSCK.eHCACK.CannotPerform/*2*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        //m_sPPID = strPPID;
                    }
                }
                else
                {// 레시피가 존재하지 않음
                    nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
            }
            else if (strRcmd == GEM.sRCMD_TOOL_VERIFICATION)
            {
                if (bResult == true)
                {
                    if (strTableLoc == "1")
                    {
                        if (strToolId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            Log_wt(string.Format("nHcack = 3; 다운로드 Tool ID {0} != sNew_Tool_Name[0]  {1}", strToolId, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0]));
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0] = "";
                        }
                        else
                        {
                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0] = strToolId;// 현재 내려 받은 Tool ID
                            Log_wt(string.Format("1 > {0} Lot ID  내려옴.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));
                        }
                    }
                    else if (strTableLoc == "2")
                    {
                        if (strToolId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            Log_wt(string.Format("nHcack = 3; 다운로드 Tool ID {0} != sNew_Tool_Name[1]  {1}", strToolId, CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1]));
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] = "";
                        }
                        else
                        {
                            nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] = strToolId;// 현재 내려 받은 Tool ID
                            Log_wt(string.Format("2 >  {0} Lot ID  내려옴.", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));
                        }
                    }
                    else
                    {
                        Log_wt(string.Format("nHcack = 3; 다운로드 strTableLoc {0} ", strTableLoc));
                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                    }
                }
                else
                {
                    CData.SecsGem_Data.qTerminal_Msg.Enqueue(strErrorMessage);
                    sgfrmMsg.Display();

                    //                    sgfrmMsg.AddString(strErrorMessage);
                    //                  sgfrmMsg.Show();
                    nHcack = 0;
                }
            }
            else if (strRcmd == GEM.sRCMD_MAT_VERIFICATION)
            {//999601 Dress Verify 응답에 대한 예외처리
                if (bResult == true)
                {
                    if (strTableLoc == "1")
                    {
                        if (strMatId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[0])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] = "";
                        }
                        else
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] = strMatId;// 현재 내려 받은 Dress ID
                        }
                    }
                    else if (strTableLoc == "2")
                    {
                        if (strMatId != CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Dress_Name[1])
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                            // Verify Nack 발생 시 초기화 진행 20200430 LCY
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] = "";
                        }
                        else
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] = strMatId;// 현재 내려 받은 Dress ID
                        }
                    }

                }
                else
                {
                    CData.SecsGem_Data.qTerminal_Msg.Enqueue(strErrorMessage);
                    sgfrmMsg.Display();
                    nHcack = (int)JSCK.eHCACK.AckOK/*0*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                }
            }
            else if (strRcmd == GEM.sRCMD_MGZ_VERIFICATION)
            {
                if (bResult == true)
                {
                    if (strPortNo == "3")
                    { // Loader Port
                        if (strMgzId != CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID)
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        }
                        else
                        {
                            CData.JSCK_Gem_Data[(int)EDataShift.ONL_MGZ_PICK/*1*/].sLD_MGZ_ID_New = strMgzId;
                        }
                    }
                    else if (strPortNo == "1")
                    {// Unloader Good Port
                        if (strMgzId != CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID)
                        {
                            nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                        }
                        else
						{
                            CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New = strMgzId;
                        }
                    }
                    else if (strPortNo == "2")
                    {// Unloader NG Port
                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                    }
                    else
                    {
                        nHcack = (int)JSCK.eHCACK.Invalid/*3*/;    // 2022.06.14 SungTae : [수정] 코드 확인 용이하도록 수정
                    }
                }
                else
                {
                    if (strPortNo == "3")
                    {
                        CData.SecsGem_Data.nLdMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Error)/*-1*/;// Loader 시퀸스 에러
                    }
                    else
					{
                        CData.SecsGem_Data.nMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Error)/*-1*/;// Unloader 시퀸스 에러
                    }

                    CData.SecsGem_Data.qTerminal_Msg.Enqueue(strErrorMessage);
                    sgfrmMsg.Display();

                    nHcack = 0;
                }
            }

            //-------------------------------------------------------------------------------
            //S2F41의 ACK 응답을 먼저 보냄. ---------------------------------------------------
            //-------------------------------------------------------------------------------
            int rMsgId = mgem.CreateReplyMsg(lMsgId);
            mgem.OpenListItem(rMsgId);
            mgem.AddBinaryItem(rMsgId, nHcack);
            mgem.OpenListItem(rMsgId);
            mgem.CloseListItem(rMsgId);
            mgem.CloseListItem(rMsgId);
            mgem.SendMsg(rMsgId);

            //-------------------------------------------------------------------------------
            //-Data 변경에 대한 회신 보내는 곳 ------------------------------------------------
            //-------------------------------------------------------------------------------

            if ((strRcmd == GEM.sRCMD_LOT_VERIFICATION) && (bResult == true) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/))  // 2022.04.01 SungTae Start : [수정] 코드 확인 시 용이하도록 변경
            {
                // 장비에서 Start -> Strip 보고 -> Lot 정보 내려 주는 곳
                // 2. Lot ID - Varification 완료
                //CData.m_pHostLOT.DeleteAll();
                AddStripFromHost(aSubSt, CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);//

                Set_SVID(Convert.ToInt32(JSCK.eSVID.LOT_ID)         , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_LOT_ID) , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name);

                // m_sCLOTID = JSCK_Gem_Data[4].sLot_Name;
                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    CData.LotMgr.LoadingLotName = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name;
                }
                else
                {
                    CData.LotInfo.sLotName = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name;
                }
                // 2021.08.25 SungTae End

                //Log_wt(string.Format("2. CData.JSCK_Gem_Data[4].sLot_Name {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));
                //Log_wt(string.Format("3. m_sReadLOT_T {0}", m_sReadLOT_T));

                m_sReadLOT_T = ""; // 임시변수 초기화

                m_pReadCarrierList.SetLotID(strLotId);
                m_pStartCarrierList.SetLotID(strLotId);
                m_pValidCarrierList.SetLotID(strLotId);
                m_pEndedCarrierList.SetLotID(strLotId);
                m_pRejectCarrierList.SetLotID(strLotId);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.LOT_Verified).ToString());

                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                Log_wt(string.Format("2. Lot ID - Varification 완료 CData.JSCK_Gem_Data[4].sLot_Name {0}", CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLot_Name));

                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(strLotId);

                    // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                    {
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, (int)SECSGEM.JSCK.eCarrierStatus.Read);

                        sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    }
                    // 2022.06.14 SungTae End
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Strip_ID, (int)SECSGEM.JSCK.eCarrierStatus.Read);
                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                }
                // 2021.08.25 SungTae End
            }
            else if ((strRcmd == GEM.sRCMD_CARRIER_VERIFICATION) && (bResult == true) && (nHcack == 0))
            {
                // 1. Strip ID 보고 완료
                // 2. Lot ID - Verification 완료
                // 3. Strip Verication 완료 후  투입 시작
                //
                // 3- 1) Strip Varification 완료 보고
                Log_wt(string.Format("3- 1) Strip Verification 완료 보고 "));

                m_pValidCarrierList.AddCarrier(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                    
                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    nIdx = CData.LotMgr.GetListIndexNum(strLotId);

                    // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                    {
                        CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Validate));
                    }
                    // 2022.06.14 SungTae End
                }
                else
                {
                    CData.m_pHostLOT.SetStripStatus(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID, Convert.ToInt32(JSCK.eCarrierStatus.Validate));
                }
                // 2021.08.25 SungTae End

                Make_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_List_Validate));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Carrier_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sInRail_Verified_Strip_ID);
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Carrier_Verified).ToString());
                // 3- 2) JSCK SECSGEM 사용시 Strip Varification 진행 후 Pick 승인 완료 하여 작업 시작
                CData.SecsGem_Data.nStrip_Check = Convert.ToInt32(JSCK.eChkStripVerify.StripSuccess)/*3*/;// 시퀸스 계속 진행
                                                                                                          //CSq_OnP.It.m_bPickOK = true; 20200910 삭제 LCY
                // 2021.08.25 SungTae Start : [수정] Multi-LOT 관련
                if (CData.IsMultiLOT())
                {
                    // 2022.06.14 SungTae Start : [수정] (ASE-KR VOC) Remote 상태에서 Multi-LOT 사용 중 프로그램 꺼짐 현상 관련 조건 추가
                    if (CData.LotMgr.ListLOTInfo.Count > 0 && nIdx != -1)
                    {
                        sgfrmMsg.ShowGrid(CData.LotMgr.ListLOTInfo[nIdx].m_pHostLOT);
                    }
                    // 2022.06.14 SungTae End
                }
                else
                    sgfrmMsg.ShowGrid(CData.m_pHostLOT);
                // 2021.08.25 SungTae End
            }
            else if ((strRcmd == GEM.sRCMD_PP_SELECT) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/))
            {
                // 실제 레시피가 변경되는 부분. 
                // 20190625 josh
                // 레시피 변경 지령이 들어온 상황
                // m_sPPID로 디바이스 변경해야 함.
                //                
                Log_wt(string.Format("레시피 변경 지령이 들어온 상황"));
                
                CData.FrmMain.m_vwRcp.LoadDeviceSecs(CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_PPID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.PPID)        , CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNew_Recipe_Name);
                
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Recipe_Loaded).ToString());
            }
            else if ((strRcmd == GEM.sRCMD_TOOL_VERIFICATION) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/) && (bResult == true))
            {
                // Tool, Wheel Verifaction 진행 
                // 장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신
                Log_wt(string.Format("장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신"));
                
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID), "");
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID), "");

                if (strTableLoc == "1")
                {
                    CData.LotInfo.sCurr_LToolId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0];
                    CData.LotInfo.sLToolId = CData.LotInfo.sCurr_LToolId;
                    
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Wheel_Name[0]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_1_ID)  , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                }
                else if (strTableLoc == "2")
                {
                    CData.LotInfo.sCurr_RToolId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1];
                    CData.LotInfo.sRToolId = CData.LotInfo.sCurr_RToolId;
                    
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID) , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Wheel_Name[1]);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_2_ID)  , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_Tool_2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_L_ID)              , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Wheel_R_ID)              , CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Tool_Loc), strTableLoc);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc), strTableLoc);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Verified).ToString());
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Tool_Changed).ToString());
            }
            else if ((strRcmd == GEM.sRCMD_MAT_VERIFICATION) && (nHcack == 0) && (bResult == true))
            {
                // Dress Verifaction 진행 
                // 장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신
                Log_wt(string.Format("장비 GUI 에서 변경 요청 -> Host 에서 응답 -> 장비에서 회신"));

                Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), "");
                Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), "");

                if (strTableLoc == "1")
                {
                    CData.LotInfo.sCurr_LDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0];
                    //CData.LotInfo.sCurr_LDrsId = strMatId;
                    CData.LotInfo.sLDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0];

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0]);
                }
                else if (strTableLoc == "2")
                {
                    CData.LotInfo.sCurr_RDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1];
                    //CData.LotInfo.sCurr_RDrsId = strMatId;
                    CData.LotInfo.sRDrsId = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1];

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]);
                }

                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL1_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0]);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Current_Loaded_MATL2_ID), CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1]);

                Set_SVID(Convert.ToInt32(JSCK.eSVID.MATL_Loc), strTableLoc);
                Set_SVID(Convert.ToInt32(JSCK.eSVID.Table_Loc), strTableLoc);

                Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Verified).ToString());
                Send_CEID(Convert.ToInt32(JSCK.eCEID.Material_Changed).ToString());
            }
            else if ((strRcmd == GEM.sRCMD_MGZ_VERIFICATION) && (nHcack == (int)JSCK.eHCACK.AckOK/*0*/) && (bResult == true))
            {
                // MGZ Verifaction 진행 
                // 장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신                
                if (strPortNo == "3")
                {// Loader Mgz Verifyed 
                    Log_wt(string.Format("장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신 , strPortNo -> 3"));
                    
                    CData.SecsGem_Data.nLdMGZ_Check = Convert.ToInt32(JSCK.eChkVerify.Complete)/*2*/;// 시퀸스 계속 진행

                    // 2021.08.30 SungTae Start : [수정] Multi-LOT 관련
                    if (CData.IsMultiLOT())
                        CData.LotMgr.ListLOTInfo[nIdx].sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New;
                    else
                        CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New;
                    // 2021.08.30 SungTae End

                    Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sLD_MGZ_ID_New);
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), strPortNo);

                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Started).ToString());
                }
                else
                {// Unloader Mgz Verifyed

                    // 2021.08.30 SungTae Start : [수정] Multi-LOT 관련
                    if (CData.IsMultiLOT())
                        CData.LotMgr.ListLOTInfo[nIdx].sNMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New;
                    else
                        CData.LotInfo.sNMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New;
                    // 2021.08.30 SungTae End

                    if (strPortNo == "1")
                    {
                        Log_wt(string.Format("장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신 , strPortNo -> 1"));
                        
                        //CData.LotInfo.sGMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New;
                        
                        Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);

                        m_pMgzGoodCarrierList.ClearCarrier();
                        m_pMgzGoodCarrierList.SetMgzID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);
                        m_pMgzGoodCarrierList.SetLotID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                    }
                    else if (strPortNo == "2")
                    {
                        Log_wt(string.Format("장비 변경 요청 -> Host 에서 응답 -> 장비에서 회신, strPortNo = 2"));

                        //CData.LotInfo.sNMgzId = CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New;
                        
                        Set_SVID(Convert.ToInt32(JSCK.eSVID.Magazine_ID), CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);

                        m_pMgzNGCarrierList.ClearCarrier();                                 // 2019 11 27 skpark
                        m_pMgzNGCarrierList.SetMgzID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sULD_MGZ_ID_New);  // unload mgz id 
                        m_pMgzNGCarrierList.SetLotID(CData.JSCK_Gem_Data[(int)EDataShift.OFL_STRIP_OUT/*23*/].sLot_Name);
                    }

                    CData.SecsGem_Data.nMGZ_Check = 2;// 시퀸스 계속 진행
                    
                    Set_SVID(Convert.ToInt32(JSCK.eSVID.MGZ_Port_Number), strPortNo);
                    
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Verified).ToString());
                    Send_CEID(Convert.ToInt32(JSCK.eCEID.Magazine_Started).ToString());
                }
            }
        }
        #endregion



        public void SendMakeSVID( int iSVID , string strMessage )
		{
			// 2020 07 16 skpark object SVID를 string으로 wndmessage로 전달
			string sMsg = string.Empty;
			sMsg = string.Format("{0}={1}", iSVID, strMessage);
				
			string sTN = string.Empty;
			byte[] tBuff;
			IntPtr hwnd;
			COPYDATASTRUCT cds = new COPYDATASTRUCT();

			sTN  = GEM.sWND;
			hwnd = FindWindow(null, sTN);

			//hwnd = this.Handle;
			tBuff = System.Text.Encoding.Default.GetBytes(sMsg);

			cds.dwData = new IntPtr(Convert.ToInt32(JSCK.eMID.SVID));
			cds.cbData = tBuff.Length + 1; 
			cds.lpData = sMsg;

			SendMessage(hwnd, WM_COPYDATA, hwnd, ref cds);
		}

        public void Make_SVID(int iSVID)
        {
			// 2020 07 16 object를 직접 만들지 않고 string을 wndproc으로 전달하여 object를 생성.
            int iCnt = 0;
            string stmp = string.Empty;
			string strListCarrier = string.Empty;

            try
            {
                //Log_wt(string.Format(iSVID.ToString() + " LOT = " + m_sReadLOT_T));
                Log_wt(string.Format("Make_SVID(" + iSVID.ToString() + "), Carrier = " + m_sReadLOT_T));

                if (iSVID == (int)JSCK.eSVID.Carrier_List_Read)
                {
                    iCnt = m_pReadCarrierList.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = m_pReadCarrierList.GetCarrierID(i);

                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }

					SendMakeSVID( iSVID , strListCarrier );
                }
                else if (iSVID == (int)JSCK.eSVID.Carrier_List_Validate)
                {
                    iCnt = m_pValidCarrierList.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = m_pValidCarrierList.GetCarrierID(i);

                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }

                    SendMakeSVID(iSVID, strListCarrier);
                }
                else if (iSVID == (int)JSCK.eSVID.Carrier_List_Started)
                {
                    iCnt = m_pStartCarrierList.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = m_pStartCarrierList.GetCarrierID(i);
                            
                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }

                    SendMakeSVID( iSVID , strListCarrier );
                }
                else if (iSVID == (int)JSCK.eSVID.Carrier_List_Complete)
                {
                    iCnt = m_pEndedCarrierList.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = m_pEndedCarrierList.GetCarrierID(i);

                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }

                    SendMakeSVID( iSVID , strListCarrier );
                }
                else if (iSVID == (int)JSCK.eSVID.Carrier_List_Reject)
                {
                    iCnt = m_pRejectCarrierList.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = m_pRejectCarrierList.GetCarrierID(i);

                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }
 
                    SendMakeSVID( iSVID , strListCarrier );
                }
                else if (iSVID == (int)JSCK.eSVID.Total_Carrier_List)
                {
                    iCnt = CData.m_pHostLOT.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = CData.m_pHostLOT.GetCarrierID(i);

                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }

                    SendMakeSVID( iSVID , strListCarrier );
                }
                else if (iSVID == (int)JSCK.eSVID.Proceed_Carrier_List)
                {
                    iCnt = m_pEndedCarrierList.Count;

                    for (int i = 0; i < iCnt; i++)
                    {
                        stmp = m_pEndedCarrierList.GetCarrierID(i);

                        if (i == 0) strListCarrier = stmp;
                        else        strListCarrier = strListCarrier + "," + stmp;
                    }

                    SendMakeSVID( iSVID , strListCarrier );
				}

				//-------------------------------------------------
				Log_wt(string.Format("E SendMakeSVID(" + iSVID.ToString() + " , " + strListCarrier));
            }
            catch (System.IndexOutOfRangeException e)  // CS0168
            {
                Log_wt(string.Format("Make_SVID Error : "+ e.Message));
                return;// e.Message;
            }
        }
        #endregion function Event - OnMsgReceived


        public void SecLotOpen(string LotName, string TotalNum, string devName)
        {
            // 2021-06-17, jhLee : 최초 1회 LOT Verification 처리를 Skyworks 전용으로 변경
            //                      Skyworks 외에 ASE-KR 및 SPIL에서 2번째 처리되는 LOT이 처리않되는 문제가 발생하여 Skyworks 전용으로 전환하였다.
            // 2021.06.02. SungTae Start : [수정] 연속으로 LOT OPEN이 안되는 문제로 인해 ASE-KR에서는 아래 조건 SKIP.
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                if (!CData.IsMultiLOT())
                {
                    CData.nLotVerifyCount++;
                    // 2021-01-18, jhLee : Skyworks에서 SECS/GEM 연결시 In/out Strip count가 올바르지 않고 In:2, Out:1 로 수량 고정되는 문제 발생
                    //                      매번 LOT Validation 정보가 내려올때 마다 호출하던것은 단 1회만 호출할 수 있도록 한다.
                    if (CData.nLotVerifyCount > 1)  // 이미 SECS에 의한 Lot가 Open되었다면 더이상 처리하지 않는다.
                    {
                        Log_wt($"Already SECS-Lot Opened, LOT Verification Info count : {CData.nLotVerifyCount} ");
                        return;
                    }
                }
            }
            // 2021.06.02. SungTae End

            // 2021.08.10 SungTae Start : [수정] Remote 상태에서의 연속 LOT 처리 위해 수정
            CData.LotInfo.sLotName = LotName;
            
            if (CData.IsMultiLOT())
            {
                CData.LotMgr.LoadingLotName = LotName;

                CData.LotMgr.AddLot(LotName, 0, 0);     // Host로부터 받은 LOT ID와 Strip 수량을 추가

                CData.LotMgr.ListUpdateFlag = true;

                int nIdx = CData.LotMgr.GetListIndexNum(LotName);

                CData.LotMgr.ListLOTInfo[nIdx].bLotOpen = true;
                CData.LotMgr.ListLOTInfo[nIdx].bLotEnd  = false;

                // 2022.05.15 SungTae Start : [삭제] 
                //CData.LotMgr.ListLOTInfo[nIdx].iTInCnt  = 1;
                //CData.LotMgr.ListLOTInfo[nIdx].iTOutCnt = 0;
                // 2022.05.15 SungTae End

                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sEndTime   = "";
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sIdleTime  = "";
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sJamTime   = "";
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sRunTime   = "";
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sTotalTime = "";

                // 2022.05.12 SungTae Start : [삭제]
                //if (CData.Opt.bSecsUse)
                //{
                //    CData.LotMgr.ListLOTInfo[nIdx].nType    = CData.LotMgr.GetNextType();
                //    CData.LotMgr.ListLOTInfo[nIdx].LotColor = CData.LotMgr.GetLotTypeColor(CData.LotMgr.ListLOTInfo[nIdx].nType);
                //}
                // 2022.05.12 SungTae End

                //if (CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(JSCK.eCont.Local))
                if (CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(JSCK.eCont.Remote))
                {
                    CData.LotMgr.ListLOTInfo[nIdx].iTotalStrip = Convert.ToInt32(TotalNum);
                }

                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sLotName      = CData.LotMgr.LoadingLotName;
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sDevice       = devName;
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
                CData.LotMgr.ListLOTInfo[nIdx].rSpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;
            }
            // 2021.08.10 SungTae End

            CData.LotInfo.bLotOpen  = true;
            CData.LotInfo.bLotEnd   = false;
            CData.LotInfo.iTInCnt   = 1;
            CData.LotInfo.iTOutCnt  = 0;

            CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

            // 2021.03.09 SungTae : Local Mode 관련 조건 추가
            if (CData.SecsGem_Data.nRemote_Flag != Convert.ToInt32(JSCK.eCont.Local))
            {
                CData.LotInfo.iTotalStrip = Convert.ToInt32(TotalNum); //koo : Qorvo Lot Rework
            }

            CData.bLotEndShow = false;
            CData.bLotEndMsgShow = false;  //190521 ksg :
			
            // 201113 jym : SECS/GEM 사용시 여기서 시작시간을 지워버려서 문제가 발생
            //              섹잼 사용시에는 시작시간은 이미 저장 되었기에 여기서 초기화를 다시하면 안됨
            //CData.SpcInfo.sStartTime = "";
            CData.SpcInfo.sEndTime   = "";
            CData.SpcInfo.sIdleTime  = "";
            CData.SpcInfo.sJamTime   = "";
            CData.SpcInfo.sRunTime   = "";
            CData.SpcInfo.sTotalTime = "";

            //lotinfo 데이터 입력
            CData.SpcInfo.sLotName      = CData.LotInfo.sLotName;
            CData.SpcInfo.sDevice       = devName;
            CData.SpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
            CData.SpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;

            CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

            CData.bLotEndShow = false;
            CData.bLotEndMsgShow = false;  //190521 ksg :

            //CData.swLotTotal.Reset();
            CData.bLotTotalReset = true;
            //CData.swLotIdle.Reset();
            CData.bLotIdleReset = true;
            //CData.swLotJam.Reset();
            CData.bLotJamReset = true;

            CSq_OfL.It.m_GetQcWorkEnd = false; //190408 ksg : Qc
            //CTcpIp.It.bResultGood = false; //190408 ksg : Qc
            //CTcpIp.It.bResultFail = false; //190408 ksg : Qc
            CQcVisionCom.rcvQCReadyQueryGOOD = CQcVisionCom.rcvQCReadyQueryNG = false;

            // 2021.09.30 lhs Start : Loading Stop 자동 해제 안되도록 수정 (SCK VOC)
            //if (!CSQ_Main.It.bSetUpStripStatus)//210714 pjh : Auto Load Stop 인터락 추가. Auto load stop 및 Set up strip 진행 시에는 초기화 시키지 않음.
            //{ CSQ_Main.It.m_bPause = false; }//190506 ksg :
            
            if (!CSQ_Main.It.bSetUpStripStatus)//210714 pjh : Auto Load Stop 인터락 추가. Auto load stop 및 Set up strip 진행 시에는 초기화 시키지 않음.
            {
                if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)    // 2021.09.30 lhs : SCK JSCK 제외 (SCK VOC)
                {
                    CSQ_Main.It.m_bPause = false; //190506 ksg  
                }
            }
            // 2021.09.30 lhs End

            CData.bSecLotOpen = true;
        }


        #region HSMS - button event
        private void btnMini_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;    //josh z order
            //is.Hide();
            this.Visible = false;
            this.ShowIcon = false;
            notifyIcon1.Visible = true;
        }

        private void btnConn_Start_Click(object sender, EventArgs e)
        {
            tmrGEM.Stop();
            Start_GEM();
        }
        public void GEM_Start()
        {
            tmrGEM.Stop();
            Start_GEM();
        }

        private void btnConn_Stop_Click(object sender, EventArgs e)
        {
            Stop_GEM();
        }
        public void Stop_GEM()
        {
            if (nLoad_flag == 1) nLoad_flag = 0;
            else
            {
                Log_wt("Already GEM Stopped");
                return;
            }

            if (mgem.Stop() == 0)   { Log_wt("EZGEM Stopped"); }

            // 2021-06-23, jhLee, SPIL-FJ VOC, GEM 종료시 Off-Line 보고 요청
            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Offline);
            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Control_State), m_iCtrlState.ToString());
            mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Control_State_Change));
            // end of Offile event

            m_iConnState = Convert.ToInt32(JSCK.eConn.Disconnected);
            CData.SecsGem_Data.nConnect_Flag = m_iConnState;

            mgem.GoOffline();
            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Offline);

            Get_Show();
            Save_Config();
            LastSave();

            lblCont_Status.Text      = JSCK.eCont.Offline.ToString();
            lblCont_Status.BackColor = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Offline)];

            btnCont_Offline.Enabled  = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;
            btnCont_Local.Enabled    = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;
            btnCont_Remote.Enabled   = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;

            lblConn_Status.Text      = JSCK.eConn.Disconnected.ToString();
            lblConn_Status.BackColor = GEM.clrA_RC[Convert.ToInt32(JSCK.eConn.Disconnected)];

            lblComm_Status.Text      = JSCK.eComm.NotCommunicating.ToString();
            lblComm_Status.BackColor = GEM.clrA_WRC[Convert.ToInt32(JSCK.eComm.NotCommunicating)];

            btnConn_Start.Enabled    = (m_iConnState == Convert.ToInt32(JSCK.eConn.Connected))    ? false : true;
            btnConn_Stop.Enabled     = (m_iConnState == Convert.ToInt32(JSCK.eConn.Disconnected)) ? false : true;
        }

        private void btnCont_Offline_Click(object sender, EventArgs e)
        {
            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Offline);

            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Control_State), m_iCtrlState.ToString());
            mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Control_State_Change));
            mgem.GoOffline();

            lblCont_Status.Text      = JSCK.eCont.Offline.ToString();
            lblCont_Status.BackColor = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Offline)];

            btnCont_Offline.Enabled  = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Offline)) ? false : true;
            btnCont_Local.Enabled    = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Local))   ? false : true;
            btnCont_Remote.Enabled   = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Remote))  ? false : true;
        }

        private void btnCont_Local_Click(object sender, EventArgs e)
        {
            mgem.GoOnlineLocal();
        }

        private void btnCont_Remote_Click(object sender, EventArgs e)
        {
            mgem.GoOnlineRemote();
        }

        private void btnHSMS_Save_Click(object sender, EventArgs e)
        {
            Get_Show();
            Save_Config();
        }
        #endregion HSMS - button event

        #region GEM - button event
        private void radES_CheckedChanged(object sender, EventArgs e)
        {//sample - process state change
            RadioButton radtmp = (RadioButton)sender;

            if (m_iPS != Convert.ToInt32(radtmp.Tag.ToString()))
            {
                Set_ProcStateChange(Convert.ToInt32(radtmp.Tag.ToString()));
            }
        }

        private void btnAlarm_Set_Click(object sender, EventArgs e)
        {//sample - alarm occur
            int iALID = 0;

            iALID = Convert.ToInt32(cboALID.SelectedItem.ToString());

            Set_AlarmOccur(iALID);
        }

        private void btnAlarm_Reset_Click(object sender, EventArgs e)
        {//sample - alarm reset
            int iALID = 0;

            iALID = Convert.ToInt32(cboALID.SelectedItem.ToString());

            Set_AlarmReset(iALID);
        }

        private void btnDevice_Search_Click(object sender, EventArgs e)
        {//sample - device file search
            Get_DeviceFileList();
        }

        private void btnDevice_Change_Click(object sender, EventArgs e)
        {//sample - recipe loaded
            string sPPID = string.Empty;

            if (cboDevice.SelectedIndex == -1)
            { return; }

            sPPID = cboDevice.SelectedItem.ToString();

            if (sPPID != string.Empty)
            { Set_PPIDC(sPPID); }
        }

        private void btnDevice_Add_Click(object sender, EventArgs e)
        {
            Device_Add();
        }

        private void btnDevice_Delete_Click(object sender, EventArgs e)
        {
            Device_Del();
        }

        private void btnDevice_Edit_Click(object sender, EventArgs e)
        {
            string sPPID = string.Empty;

            if (cboDevice.SelectedIndex == -1)
            { return; }

            sPPID = cboDevice.SelectedItem.ToString();

            if (sPPID != string.Empty)
            { Set_PPIDE(sPPID); }
        }

        private void btnStrip_CarrierVerifyRequest_Click(object sender, EventArgs e)
        {//sample - Carrier Verify Request
            string sCarrierID = string.Empty;

            if (txtStrip_CarrierID.Text == string.Empty)
            { return; }

            sCarrierID = txtStrip_CarrierID.Text;
            OnCarrierVerifyRequset(sCarrierID);
        }

        private void btnStrip_ClearCarrierList_Click(object sender, EventArgs e)
        {
            Clear_CarrierList();
        }

        private void btnStrip_LOTVerified_Click(object sender, EventArgs e)
        {
            LOT_Verified();
        }

        private void btnStrip_CarrierCreate_Click(object sender, EventArgs e)
        {//sample - Strip Create
            string  sLotId      = string.Empty;
            string  sCarrierID  = string.Empty;
            string  sSlotNo     = string.Empty;

            uint    nVal = 0;

            if (txtStrip_LOTID.Text     == string.Empty)    { return; }
            if (txtStrip_CarrierID.Text == string.Empty)    { return; }
            if (txtStrip_SlotNo.Text    == string.Empty)    { return; }

            sLotId      = txtStrip_LOTID.Text;
            sCarrierID  = txtStrip_StripID.Text;
            sSlotNo     = txtStrip_SlotNo.Text;

            uint.TryParse(sSlotNo, out nVal);

            if (nVal != 0)
            {
                m_pReadCarrierList.AddCarrier(sCarrierID);
            }
        }

        private void btnStrip_CarrierStarted_Click(object sender, EventArgs e)
        {//sample - Carrier Started
            string  sCarrierId  = string.Empty;
            string  sLotId      = string.Empty;

            if (txtStrip_CarrierID.Text == string.Empty)    { return; }
            if (txtStrip_LOTID.Text     == string.Empty)    { return; }

            sLotId      = txtStrip_LOTID.Text;
            sCarrierId  = txtStrip_CarrierID.Text;
            
            OnCarrierStarted(sLotId, sCarrierId);
        }

        private void btnStrip_GetStripID_Click(object sender, EventArgs e)
        {//sample - Get Strip ID
            string  sLotId      = string.Empty;
            string  sCarrierId  = string.Empty;

            if (txtStrip_LOTID.Text     == string.Empty)    { return; }
            if (txtStrip_CarrierID.Text == string.Empty)    { return; }

            sLotId      = txtStrip_LOTID.Text;
            sCarrierId  = txtStrip_CarrierID.Text;
        }

        private void btnStrip_BeforeM_Click(object sender, EventArgs e)
        {
            //값 사용안함.
            //OnCarrierEnd에서 처리
        }

        private void btnStrip_MiddleM_Click(object sender, EventArgs e)
        {
            //값 사용안함.
            //OnCarrierEnd에서 처리
        }

        private void btnStrip_EndM_Click(object sender, EventArgs e)
        {
            //값 사용안함.
            //OnCarrierEnd에서 처리
        }

        private void btnStrip_CarrierEnd1_Click(object sender, EventArgs e)
        {//sample - Carrier End 1
            string  sCarrierId  = string.Empty;
            string  sLotId      = string.Empty;
            string  sSlotNo     = string.Empty;
            string  sGoodNg     = string.Empty;

            uint    nSlotNo     = 0;
            uint    nGoodNg     = 0;

            int     iTableLoc   = 1;

            if (txtStrip_CarrierID.Text == string.Empty)    { return; }
            if (txtStrip_LOTID.Text     == string.Empty)    { return; }
            if (txtStrip_SlotNo.Text    == string.Empty)    { return; }
            if (txtStrip_GoodNg.Text    == string.Empty)    { return; }

            sCarrierId  = txtStrip_CarrierID.Text;
            sLotId      = txtStrip_LOTID.Text;
            sSlotNo     = txtStrip_SlotNo.Text;
            sGoodNg     = txtStrip_GoodNg.Text;

            uint.TryParse(sSlotNo, out nSlotNo);
            uint.TryParse(sGoodNg, out nGoodNg);

            if (nSlotNo != 0)
            {
                OnCarrierEnded(sLotId, sCarrierId, nSlotNo, iTableLoc, nGoodNg);
            }
        }

        private void btnStrip_CarrierEnd2_Click(object sender, EventArgs e)
        {//sample - Carrier End 2
            string  sCarrierId  = string.Empty;
            string  sLotId      = string.Empty;
            string  sSlotNo     = string.Empty;
            string  sGoodNg     = string.Empty;

            uint    nSlotNo     = 0;
            uint    nGoodNg     = 0;

            int     iTableLoc   = 2;

            if (txtStrip_CarrierID.Text == string.Empty)    { return; }
            if (txtStrip_LOTID.Text     == string.Empty)    { return; }
            if (txtStrip_SlotNo.Text    == string.Empty)    { return; }
            if (txtStrip_GoodNg.Text    == string.Empty)    { return; }

            sCarrierId  = txtStrip_CarrierID.Text;
            sLotId      = txtStrip_LOTID.Text;
            sSlotNo     = txtStrip_SlotNo.Text;
            sGoodNg     = txtStrip_GoodNg.Text;

            uint.TryParse(sSlotNo, out nSlotNo);
            uint.TryParse(sGoodNg, out nGoodNg);

            if (nSlotNo != 0)
            {
                OnCarrierEnded(sLotId, sCarrierId, nSlotNo, iTableLoc, nGoodNg);
            }
        }

        private void btnStrip_LOTEnded_Click(object sender, EventArgs e)
        {//sample - LOT Ended
            string  sLotId      = string.Empty;
            string  sTotalUnit  = string.Empty;
            string  sProceed    = string.Empty;

            uint    nTotalUnit  = 0;
            uint    nProceed    = 0;

            if (txtStrip_LOTID.Text  == string.Empty)   { return; }
            if (txtStrip_SlotNo.Text == string.Empty)   { return; }
            if (txtStrip_GoodNg.Text == string.Empty)   { return; }

            sLotId      = txtStrip_LOTID.Text;
            sTotalUnit  = txtStrip_SlotNo.Text;
            sProceed    = txtStrip_GoodNg.Text;

            uint.TryParse(sTotalUnit, out nTotalUnit);
            uint.TryParse(sProceed, out nProceed);

            OnLotEnded(sLotId, sTotalUnit, sProceed);
        }

        private void btnV_ToolVerifyRequest_Click(object sender, EventArgs e)
        {//sample - Tool Verify Request
            string  sToolId  = string.Empty;
            string  sToolLoc = string.Empty;

            uint    nToolLoc = 0;

            if (txtV_ToolID.Text    == string.Empty)    { return; }
            if (txtV_ToolLoc.Text   == string.Empty)    { return; }

            sToolId  = txtV_ToolID.Text;
            sToolLoc = txtV_ToolLoc.Text;

            if (sToolLoc == "1")    { CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[0] = txtV_ToolID.Text; }
            else                    { CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sNEW_Tool_Name[1] = txtV_ToolID.Text; }

            uint.TryParse(sToolLoc, out nToolLoc);
            string sSerial_Num = nToolLoc + "Serial_Num";

            ToolSerial_Number_Set(sSerial_Num, nToolLoc);
            OnToolVerifyRequest(sToolId, nToolLoc);
        }

        private void btnV_MatVerifyRequest_Click(object sender, EventArgs e)
        {//sample - Mat Verify Request
            string  sMatId  = string.Empty;
            string  sMatLoc = string.Empty;

            uint    nMatLoc = 0;

            if (txtV_MatID.Text  == string.Empty)   { return; }
            if (txtV_MatLoc.Text == string.Empty)   { return; }

            sMatId  = txtV_MatID.Text;
            sMatLoc = txtV_MatLoc.Text;

            if (sMatLoc == "1") { CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNEW_Dress_Name[0] = txtV_MatID.Text; }
            else                { CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sNEW_Dress_Name[1] = txtV_MatID.Text; }

            uint.TryParse(sMatLoc, out nMatLoc);
            
            OnMatVerifyRequest(sMatId, nMatLoc);
        }

        private void btnV_MGZVerifyRequest_Click(object sender, EventArgs e)
        {//sample - MGZ Verify Request
            string  sMGZId   = string.Empty;
            string  sMGZPort = string.Empty;

            uint    nMGZPort = 0;

            if (txtV_MGZID.Text   == string.Empty)  { return; }
            if (txtV_MGZPort.Text == string.Empty)  { return; }

            sMGZId   = txtV_MGZID.Text;
            sMGZPort = txtV_MGZPort.Text;

            CData.JSCK_Gem_Data[(int)EDataShift.INR_BCR_READ/*4*/].sULD_MGZ_ID_New = txtV_MGZID.Text;
            
            uint.TryParse(sMGZPort, out nMGZPort);

            UnloaderMgzVerifyRequest(sMGZId, nMGZPort);
        }

        private void btnV_MGZEnded_Click(object sender, EventArgs e)
        {//sample - MGZ Ended
            string  sMGZId   = string.Empty;
            string  sMGZPort = string.Empty;

            uint    nMGZPort = 0;

            if (txtV_MGZID.Text   == string.Empty)  { return; }
            if (txtV_MGZPort.Text == string.Empty)  { return; }

            sMGZId   = txtV_MGZID.Text;
            sMGZPort = txtV_MGZPort.Text;
            
            uint.TryParse(sMGZPort, out nMGZPort);

            //OnMagazineEnd(sMGZId, nMGZPort);
            OnMagazineEnd(nMGZPort);
        }
        #endregion GEM - button event

        #region Tray Icon 부분 
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;    //josh z order
            //this.Hide();

            //this.Visible = false;
            //this.ShowIcon = false;
            ///notifyIcon1.Visible = true;
            ///

            // 2021.08.07 lhs Start : 전체폼 보기려면 주석처리해야 함.
            this.Height = 495;
            this.Width  = 560;
            // 2021.08.07 lhs End

            this.Show();
            m_tmUpdt.Stop();

            #region
            /*
            /// Host 연결 상태 연동 위해 Test 진행 중
            CData.SecsGem_Data.nComm_Flag       = m_iCommState;
            CData.SecsGem_Data.nConnect_Flag    = m_iConnState;
            CData.SecsGem_Data.nRemote_Flag     = m_iCtrlState;
            */
            #endregion
        }

        private void frmGEM_Resize(int nSlot,int nWork_Status)
        {
           // sgfrmMsg.ShowGrid(m_pHostLOT);
        }

        private void frmGEM_Resize(object sender, EventArgs e)
        {
            return;

            if(this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.ShowIcon = false;
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.ShowIcon = true;
            notifyIcon1.Visible = false;
        }

        public void frmGEM_ShowWindow()
        {
            this.Visible = true;
            this.ShowIcon = true;
            notifyIcon1.Visible = false;

            this.Activate();
            this.BringToFront();

        }
        private void btnMini_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowIcon = false;
            notifyIcon1.Visible = true;
        }
        #endregion


        // cross thread시 ui의 부분을 해당 UI쓰렉드가 처리하도록 message pump시킴.
        /*
        * InvokeRequired 속성 (Control.InvokeRequired, MSDN)
        *   짧게 말해서, 이 컨트롤이 만들어진 스레드와 현재의 스레드가 달라서
        *   컨트롤에서 스레드를 만들어야 하는지를 나타내는 속성입니다.  
        * 
        * InvokeRequired 속성의 값이 참이면, 컨트롤에서 스레드를 만들어 텍스트를 변경하고,
        * 그렇지 않은 경우에는 그냥 변경해도 아무 오류가 없기 때문에 텍스트를 변경합니다.
         * https://exynoa.tistory.com/326
        */
        public void AddLog(string strLog)
        {
            if (listLog.InvokeRequired)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    Log_wt(strLog);
                });

            }
            else
            {
                string strTime = "";
                DateTime now = DateTime.Now;
                strTime = now.ToString("MM-dd HH:mm:ss");

                listLog.Items.Add(string.Format("{0} {1}", strTime, strLog));
                while (listLog.Items.Count > 100)
                {
                    listLog.Items.RemoveAt(0);
                }
                listLog.SetSelected(listLog.Items.Count - 1, true);
            }

        }

        #region Log 저장
        /// <summary>
        /// Log 저장 함수
        /// </summary>
         private int nDebug_Flag = 0;
        public void Log_wt(string sData)
        {
            AddLog(sData);

            string sLine = "";
            string sDir, sPath, sDay;

            DateTime Now = DateTime.Now;
            string sLOG_PATH = @"D:\Suhwoo\SG2000X\Log\";
            
            sDir = sLOG_PATH + Now.Year.ToString("0000") + Now.Month.ToString("00") +  Now.Day.ToString("00") + "\\";
            if (!Directory.Exists(sDir.ToString()))
            {
                Directory.CreateDirectory(sDir.ToString());
            }

            sDay = DateTime.Now.ToString("[yyyyMMdd-HHmmss fff]:");
            sLine = sDay + "\t";
            sLine += sData;
            /*
            if (CData.SecsGem_Data.qTerminal_Log != null)
            {
                CData.SecsGem_Data.qTerminal_Log.Enqueue(sLine);
                if(nDebug_Flag ==1) sgfrmMsg.Display();
                //sgfrmMsg.Show();
            }*/
            sPath = sDir;
            CLogManager Log = new CLogManager(sPath, null, null);
            Log.WriteLine(sLine);
        }
        #endregion

        private void btnCont_Offline_Click_1(object sender, EventArgs e)
        {
            Log_wt(string.Format("OnOffline()"));

            m_iCtrlState = Convert.ToInt32(JSCK.eCont.Offline);
            CData.SecsGem_Data.nRemote_Flag = m_iCtrlState;

            mgem.SetSVIDValue(Convert.ToInt32(JSCK.eSVID.Current_Process_Status), m_iCtrlState.ToString());    // 2021-05-28, jhLee : SPIL-FJ VOC, Off-line 보고시 control state를 off-line으로 보고시킴
            mgem.SendEventReport(Convert.ToInt32(JSCK.eCEID.Control_State_Change));
            mgem.GoOffline();

            lblCont_Status.Text      = JSCK.eCont.Offline.ToString();
            lblCont_Status.BackColor = GEM.clrA_RYC[Convert.ToInt32(JSCK.eCont.Offline)];

            btnCont_Offline.Enabled  = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Offline)) ? false : true;
            btnCont_Local.Enabled    = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Local))   ? false : true;
            btnCont_Remote.Enabled   = (m_iCtrlState == Convert.ToInt32(JSCK.eCont.Remote))  ? false : true;
        }

        private void btnCont_Local_Click_1(object sender, EventArgs e)
        {
            mgem.GoOnlineLocal();
        }

        private void btnCont_Remote_Click_1(object sender, EventArgs e)
        {
            
            mgem.GoOnlineRemote();
        }

        private void btnConn_Start_Click_1(object sender, EventArgs e)
        {
        }
        private void test()
        {

            mgem.CloseMsg(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

            int nCk_Point = 0;// Right Befor Measure 측정 갯수 확인 필요                            
            
            if(nCk_Point == 0)
            {
                float ftemp = 0;
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), ftemp);// Befor Data
            }
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));

            mgem.OpenListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            nCk_Point = 3;// Right After Measure 측정 갯수 확인 필요            
            for (int i = 0; i < nCk_Point; i++)
            {
                //Log_wt(string.Format("1 ++++++++++ {0}, {1}, {2}",loat.Parse(CData.JSCK_Gem_Data[14].fMeasure_Data[4, i].ToString() );
                float nF = 0;
                mgem.AddF4Item(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point), nF );// Befor Data
            
            }

            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            mgem.CloseListItem(Convert.ToInt32(JSCK.eSVID.Carrier_Right_Raw_Point));
            //---------------------------------------------------------------------------------------
        }

        // 2021.08.05 SungTae Start : [추가] SVID=999212(PCB_TOPBTM_USE_MODE) 바뀌는 것과 관련하여 임희성 책임님이 수정한 것을 함수로 변경.
        private string GetMoldSideInfo(ESide eMold)
        {
            string sInfo = "";

            if      (eMold == ESide.Top)    { sInfo = "0"; }
            else if (eMold == ESide.Btm)    { sInfo = "1"; }
            else if (eMold == ESide.TopD)   { sInfo = "2"; }
			else if (eMold == ESide.BtmS)   { sInfo = "3"; }

            return sInfo;
        }
        // 2021.08.05 SungTae End
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

}