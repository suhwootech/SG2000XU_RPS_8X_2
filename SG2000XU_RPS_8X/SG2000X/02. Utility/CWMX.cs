using System;
using System.Text;
using wmxCLRLibrary;
using EcApiCLRLib;
using System.Diagnostics;
using System.IO;
using wmxCLRLibrary.common;

namespace SG2000X
{
    public class CWmx : CStn<CWmx>
    {
        public int SLAVE_COUNT = 26;
		//public int SLAVE_COUNT = 28;
        //public int SLAVE_COUNT = 27; //190524 ksg : OnloadPicker Y축으로 인해 27
        private Status m_Stat;
        private EcMasterInfo m_EcInfo;

        /// <summary>
        /// WMX Main Library
        /// </summary>
        public WMXLib WLib { get; private set; }
        /// <summary>
        /// WMX EcApi Library
        /// </summary>
        public EcApiLib ELib { get; private set; }
        /// <summary>
        /// WMX Status Struct
        /// </summary>
        public Status Stat { get { return m_Stat; } }
        /// <summary>
        /// EcMaster Information 연결 상태 및 축의 상태 
        /// </summary>
        public EcMasterInfo EcInfo { get { return m_EcInfo; } }
        /// <summary>
        /// WMX Axis Parameter Struct
        /// </summary>
        public AxisParam AParm = new AxisParam();
        /// <summary>
        /// WMX System Parameter Struct
        /// </summary>
        public SystemParam SParm = new SystemParam();

        private CWmx() { }

        public int Create()
        {
            /*
            if(CDataOption.SplType == eSpindleType.Rs232)
            {
                if(CDataOption.CurEqu == eEquType.Nomal) SLAVE_COUNT = 26;
                else                                     SLAVE_COUNT = 27;
            }
            else 
            {
                if(CDataOption.CurEqu == eEquType.Nomal) SLAVE_COUNT = 28;
                else                                     SLAVE_COUNT = 29;
            }
            */
            // 2023.03.15 Max
            if (CDataOption.CurEqu == eEquType.Nomal) SLAVE_COUNT = 26;
            else                                      SLAVE_COUNT = 27;

            _SetLog("S");

            int iRet = -1;
            string sPth = @"C:\Program Files\Softservo\WMX2\";
            int iEr = 0;

            m_Stat = new Status();
            m_EcInfo = new EcMasterInfo();
            WLib   = new WMXLib();
            ELib   = new EcApiLib();
            PlatformType ePlatform = PlatformType.EtherCATEngine;

            CData.WMX = false;

            iRet = WLib.CreateDevice(sPth, ePlatform);
            
            if (iRet != (int)ErrorCode.None)
            {
                CData.WMX = false;
                _SetLog(string.Format("E, Create device fail -> {0}", Get_Err()));
                //20190227 ghk_err
                //return (int)eErr.WMX_CREATE_ERROR;
                return 1;
            }
            _SetLog(string.Format("N, Create device success, Path:{0}, Platform:{1}", sPth, ePlatform));
            GU.Delay(3000);

            ELib.CreateDevice();
            _SetLog("N, EcApiLib Create device success");

            // Import XML file
            iRet = WLib.config.ImportAndSetAll(GV.PATM_WMX_XML);
            if (iRet != 0)
            {
                _SetLog(string.Format("E, Import XML file error -> {0}", Get_Err()));
                //20190227 ghk_err
                //return (int)eErr.WMX_LOAD_AND_SET_XML_ERROR; ;
                return 1;
            }
            _SetLog("N, XML file import success");

            // 설정되어 있는 이벤트 제거
            WLib.eventControl.ClearAllEvent();

            _SetLog("F");
            return iEr;
        }

        /// <summary>
        /// WMX Communication Start
        /// 190326 - maeng : Slave 상태 확인 하여 Retry 기능 추가
        /// </summary>
        /// <returns>0:None</returns>
        public bool Open()
        {
            _SetLog("S");
            bool bCkStat = false;
            int iRet = 0;
            int iRetry = 5;    // Retry Count

            //190709 ksg :
            ELib.StartHotconnect();
            GU.Delay(4000);

            iRet = WLib.StartCommunication();
            if (iRet != (int)ErrorCode.None)
            {
                _SetLog(string.Format("E, Start communication fail -> {0}", Get_Err()));
                return false;
            }
            // 3초 딜레이
            GU.Delay(3000);

            for (int i = 0; i< iRetry; i++)
            {
                // 500ms 딜레이
                GU.Delay(500);
                bCkStat = true;

                ELib.ScanNetwork();
                iRet = ELib.GetStatus(m_EcInfo);
                if (iRet != (int)ErrorCode.None)
                {
                    // ecLib.GetStatus함수 호출이 실패할 경우는 ecLib.CreateDevice()호출이 실패할 경우 주로 발생. 
                    //그러므로 프로그램 초기 로딩으로 넘어가서 ecLib.CreateDevice()를 다시 호출한다.
                    _SetLog(string.Format("E, Get EcMaster information fail -> {0}", Get_Err()));
                    return false;
                }

                if (m_EcInfo.NumOfSlaves != SLAVE_COUNT)
                {
                    // 예를 들어 26개의 슬레이브가 네트워크상에 연결되어 있지만, 
                    //실제 통신 후 NumOfSlaves가 26개 이하로 잡히는 경우가 간혹있다.
                    // (슬레이브의 전원을 인가하자마자 곧바로 통신을 시작하면 간혹 발생함)
                    // 이와같은 경우 통신을 종료 한 후 다시 시작하면 정상적으로 연결됩니다.
                    return false;
                }

                // 2023.03.15 Max
                if (m_EcInfo.NumOfSlaves == 26)
                {   // Spindle = Rs232, BCR = Inrail
                    CDataOption.SplType = eSpindleType.Rs232;
                    CDataOption.CurEqu = eEquType.Nomal;
                }
                else if (m_EcInfo.NumOfSlaves == 27)
                {
                    CDataOption.SplType = eSpindleType.Rs232;
                    CDataOption.CurEqu = eEquType.Pikcer;
                }
                else if (m_EcInfo.NumOfSlaves == 28)
                {
                    CDataOption.SplType = eSpindleType.EtherCat;
                    CDataOption.CurEqu = eEquType.Nomal;
                }
                else
                {
                    CDataOption.SplType = eSpindleType.EtherCat;
                    CDataOption.CurEqu = eEquType.Pikcer;
                }

                for (int j = 0; j < m_EcInfo.NumOfSlaves; j++)
                {
                    if (m_EcInfo.Slaves[j].State != EcStateMachine.Op)
                    {
                        // 슬레이브의 알람 상태 및 비정상적인 상태일 경우 통신을 시작해도 OP상태까지 못 올라가는 경우가 있으며,
                        // 혹은 OP상태 까지 올라가는 시간이 오래 걸릴 수 있으므로, 
                        // 몇차례 상태확인을 한 후 그래도 모든 노드가 OP로 올라가지 않으면 통신 종료 후 다시 통신 시작
                        _SetLog(string.Format("E, Node AL Status error -> {0}", m_EcInfo.Slaves[j].State));
                        bCkStat = false;
                    }
                }

                // bCkStat 변수가 true면 정상 통신상태 이므로 함수 종료
                if (bCkStat)
                { break; }
            }

            if (bCkStat)
            {
                _SetLog("N, Start comunication success");
                CData.WMX = true;

                Get_Parm();
                _SetLog("N, Get parameters");
            }

            _SetLog("F");
            return bCkStat;
        }

        public bool Open_()
        {
            bool bCkStat = false;
            int iRet = 0;
            int iRetry = 5;    // Retry Count
            int iSlvCnt = 0;

            for (int i = 0; i < iRetry; i++)
            {
                bool bPick = false;
                bool bSplL = false;
                bool bSplR = false;    

                // 500ms 딜레이
                GU.Delay(500);
                bCkStat = true;

                WLib.StopCommunication();

                ELib.ScanNetwork();

                //190709 ksg :
                ELib.StartHotconnect();
                GU.Delay(4000);

                iRet = WLib.StartCommunication();
                if (iRet != (int)ErrorCode.None)
                {
                    _SetLog(string.Format("Error : Start communication fail -> {0}", Get_Err()));
                    return false;
                }
                // 3초 딜레이
                GU.Delay(3000);

                iRet = ELib.GetStatus(m_EcInfo);
                if (iRet != (int)ErrorCode.None)
                {
                    // ecLib.GetStatus함수 호출이 실패할 경우는 ecLib.CreateDevice()호출이 실패할 경우 주로 발생. 
                    //그러므로 프로그램 초기 로딩으로 넘어가서 ecLib.CreateDevice()를 다시 호출한다.
                    _SetLog(string.Format("Error : Get EcMaster information fail -> {0}", Get_Err()));
                    return false;
                }

                iSlvCnt = (int)m_EcInfo.NumOfSlaves;

                if (iSlvCnt < 26)
                {
                    // 예를 들어 26개의 슬레이브가 네트워크상에 연결되어 있지만, 
                    //실제 통신 후 NumOfSlaves가 26개 이하로 잡히는 경우가 간혹있다.
                    // (슬레이브의 전원을 인가하자마자 곧바로 통신을 시작하면 간혹 발생함)
                    // 이와같은 경우 통신을 종료 한 후 다시 시작하면 정상적으로 연결됩니다.
                    continue;
                }

                /*
                if (iSlvCnt == 26)
                {   // Spindle = Rs232, BCR = Inrail
                    CDataOption.SplType = eSpindleType.Rs232;
                    CDataOption.CurEqu = eEquType.Nomal;
                }
                else if (iSlvCnt == 27)
                {
                    CDataOption.SplType = eSpindleType.Rs232;
                    CDataOption.CurEqu = eEquType.Pikcer;
                }
                else if (iSlvCnt == 28)
                {
                    CDataOption.SplType = eSpindleType.EtherCat;
                    CDataOption.CurEqu = eEquType.Nomal;
                }
                else
                {
                    CDataOption.SplType = eSpindleType.EtherCat;
                    CDataOption.CurEqu = eEquType.Pikcer;
                }
                */
                // 2023.03.15 Max
                if (iSlvCnt == 26)
                {   // Spindle = Rs232, BCR = Inrail
                    CDataOption.SplType = eSpindleType.Rs232;
                    CDataOption.CurEqu = eEquType.Nomal;
                }
                else if (iSlvCnt == 27)
                {
                    CDataOption.SplType = eSpindleType.Rs232;
                    CDataOption.CurEqu = eEquType.Pikcer;
                }


                for (int j = 0; j < iSlvCnt; j++)
                {
                    EcSlaveInfo mSlave = m_EcInfo.Slaves[j];
                    if (mSlave.State != EcStateMachine.Op)
                    {
                        // 슬레이브의 알람 상태 및 비정상적인 상태일 경우 통신을 시작해도 OP상태까지 못 올라가는 경우가 있으며,
                        // 혹은 OP상태 까지 올라가는 시간이 오래 걸릴 수 있으므로, 
                        // 몇차례 상태확인을 한 후 그래도 모든 노드가 OP로 올라가지 않으면 통신 종료 후 다시 통신 시작
                        _SetLog(string.Format("Error : Node AL Status error -> {0}", mSlave.State));
                        bCkStat = false;

                        break;
                    }
                    else
                    {
                        if (mSlave.NumOfAxes == 1)
                        {   //Axis
                            EAx eAx = (EAx)mSlave.Alias;

                            if (CDataOption.CurEqu == eEquType.Pikcer && eAx == EAx.OnLoaderPicker_Y)
                            { bPick = true; }

                            if (CDataOption.SplType == eSpindleType.EtherCat && eAx == EAx.LeftSpindle)
                            { bSplL = true; }

                            if (CDataOption.SplType == eSpindleType.EtherCat && eAx == EAx.RightSpindle)
                            { bSplR = true; }
                        }
                    }
                }

                // bCkStat 변수가 true면 정상 통신상태 이므로 함수 종료
                if (bCkStat)
                {
                    if (CDataOption.CurEqu == eEquType.Nomal && bPick == true)
                    { continue; }
                    if (CDataOption.CurEqu == eEquType.Pikcer && bPick == false)
                    { continue; }
                    if (CDataOption.SplType == eSpindleType.Rs232 && (bSplL == true || bSplR == true))
                    { continue; }
                    if (CDataOption.SplType == eSpindleType.EtherCat && (bSplL == false || bSplR == false))
                    { continue; }

                    break;
                }
            }

            if (bCkStat)
            {
                _SetLog("Start comunication success.");
                CData.WMX = true;

                Get_Parm();
                _SetLog("Get parameters.");
            }

            _SetLog("WMX Open complete.");
            return bCkStat;
        }

        /// <summary>
        /// WMX Device Close
        /// </summary>
        /// <returns>0:None</returns>
        public int Close()
        {
            int iRe = -1;
            //20190227 ghk_err
            //int iEr = (int)eErr.NONE; 
            int iEr = 0;

            CData.WMX = false;

            // Stop Communication
            iRe = WLib.StopCommunication();
            if (iRe == (int)ErrorCode.None)
            {
                _SetLog("Stop comunication success");
                iRe = ELib.CloseDevice();
                _SetLog("EcApiLib close device");

                iRe = WLib.CloseDevice();
                if (iRe != (int)ErrorCode.None)
                {
                    iEr = 1;
                    _SetLog(string.Format("Error : Close device fail -> {0}", Get_Err()));
                }
                _SetLog("Close device success");
            }
            else
            {
                iEr = 1;
                _SetLog(string.Format("Error : Stop communication fail -> {0}", Get_Err()));
            }

            _SetLog("WMX Device close.");
            return iEr;
        }

        /// <summary>
        /// WMX의 마지막 발생 에러를 스트링으로 반환
        /// </summary>
        public void Get_Stat()
        {
            WLib.GetStatusSync(ref m_Stat);
            ELib.GetStatus(m_EcInfo);
        }

        public string Get_Err()
        {
            string sEr = "";
            WLib.GetLastErrorString(ref sEr, 255);
            return sEr;
        }

        /// <summary>
        /// Alias ID를 파라메터로 주면 물리적인 Slave ID를 찾아서 반환
        /// </summary>
        /// <param name="iAlias"></param>
        /// <returns></returns>
        public int Get_SlvID(int iAlias)
        {
            int iRet = 0;
            // EcStatus 정보 받아오기
            ELib.GetStatus(m_EcInfo);
            for (int i = 0; i < 32; i++)
            {
                // Alias ID 같은 정보 찾기
                if (m_EcInfo.Slaves[i].Alias == iAlias && m_EcInfo.Slaves[i].NumOfAxes == 1)
                {
                    iRet = i;
                    break;
                }
            }

            return iRet;
        }

        /// <summary>
        /// Chuck Vacuum 값 읽어 오기 위한 함수
        /// </summary>
        /// <returns></returns>
        public int Get_Vac_Value(int iAx, ref double iVal)
        {
            short nOffset = 74;
            short nSize = 2;
            int iRet = 0;
            byte[] tBuff = new byte[2];    //Get Data

            if (CDataOption.CurEqu == eEquType.Nomal)
            {
                if (iAx == 9)
                {
                    nOffset = 70;
                    iRet = It.WLib.io.GetInBytes(nOffset, nSize, ref tBuff);
                }
                else if (iAx == 12)
                {
                    nOffset = 72;
                    iRet = It.WLib.io.GetInBytes(nOffset, nSize, ref tBuff);
                }
            }

            else
            {
                if (iAx == 9)
                {
                    nOffset = 74;
                    iRet = It.WLib.io.GetInBytes(nOffset, nSize, ref tBuff);
                }
                else if (iAx == 12)
                {
                    nOffset = 76;
                    iRet = It.WLib.io.GetInBytes(nOffset, nSize, ref tBuff);
                }
            }

            if (iRet != (int)ErrorCode.None)
            {
                return iRet;
            }

            iVal = BitConverter.ToInt16(tBuff, 0);
            iVal = Math.Round(-(GV.TblVacZeroValue - iVal) / GV.TblVacOneValue, 1); //1 [Kpa] = 110 , Ex) 0 (16520) , -20 ( 14320)

            return iRet;
        }
        /// <summary>
        /// WMX에 XML파일을 읽어와 AxisParm과 SystemParm에 데이터 갱신
        /// </summary>
        /// <returns></returns>
        public int Load()
        {
            int iRe = 0;
            int iEr = 0;
            iRe = WLib.config.Import(GV.PATM_WMX_XML, ref SParm, ref AParm);
            if (iRe != (int)ErrorCode.None)
            {
                iEr = 1;
                _SetLog(string.Format("Error : XML file load fail -> {0}", Get_Err()));
            }

            _SetLog("Load complete.");
            return iEr;
        }

        /// <summary>
        /// WMX에 현재 AxisParm과 SystemParm을 XML 파일로 저장
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            int iRe = 0;
            int iEr = 0;
            iRe = WLib.config.Export(GV.PATM_WMX_XML, SParm, AParm);
            if (iRe != (int)ErrorCode.None)
            {
                iEr = 1;
                _SetLog(string.Format("Error : XML file save fail -> {0}", Get_Err()));
            }

            _SetLog("Save complete.");
            return iEr;
        }

        /// <summary>
        /// WMX에 AxisParm과 SystemParm을 적용
        /// </summary>
        /// <returns></returns>
        public int Set_Parm()
        {
            int iRe = 0;
            int iEr = 0;

            iRe = WLib.config.SetAxisParam(AParm);
            if (iRe != (int)ErrorCode.None)
            {
                iEr = 1;
                _SetLog(string.Format("Error : Set axes parameter fail -> {0}", Get_Err()));
            }
            else
            {
                iRe = WLib.config.SetParam(SParm);
                if (iRe != (int)ErrorCode.None)
                {
                    iEr = 1;
                    _SetLog(string.Format("Error : Set system parameter error -> {0}", Get_Err()));
                }
            }

            _SetLog("Set parameter complete.");
            return iEr;
        }

        public int Set_Home(int iAx)
        {
            int iRe = 0;
            string sEr = "";
            int iEr = 0;

            iRe = WLib.config.SetHomeParam((short)iAx, SParm.HomeParam[iAx]);
            if (iRe != (int)ErrorCode.None)
            {
                iEr = 1;
                WLib.GetLastErrorString(ref sEr, 255);
                _SetLog(string.Format("Error : Set home parameter error -> {0}", Get_Err()));
            }

            _SetLog("Set home parameter complete.  Axis : " + iAx);
            return iEr;
        }

        /// <summary>
        /// WMX에서 AxisParm과 SystemParm의 데이터를 추출
        /// </summary>
        /// <returns></returns>
        public int Get_Parm()
        {
            int iRe = 0;
            int iEr = 0;

            iRe = WLib.config.GetAxisParam(ref AParm);
            if (iRe != (int)ErrorCode.None)
            {
                iEr = 1;
                _SetLog(string.Format("Error : Get axis parameter error -> {0}", Get_Err()));
            }
            iRe = WLib.config.GetParam(ref SParm);
            if (iRe != (int)ErrorCode.None)
            {
                iEr = 1;
                _SetLog(string.Format("Error : Get system parameter error -> {0}", Get_Err()));
            }

            _SetLog("Get parameter complete.");
            return iEr;
        }

        //spindle
        public int Read_PDO(int iAx, int iIdx, int iSub, ref int iVal)
        {
            int iRet = 0;
            byte[] tBuff = new byte[2];    //pdoBuff
            uint iSz = 0;

            iRet = ELib.ReadRxTxPDO(Get_SlvID(iAx), iIdx, iSub, tBuff, ref iSz);
            if (iRet != (int)ErrorCode.None)
            {
                iVal = 0;
                return iRet;
            }

            iVal = BitConverter.ToInt16(tBuff, 0);
            return iRet;
        }

        public int Write_PDO(int iAx, int iIdx, int iSub, int iVal)
        {
            int iRet = 0;
            byte[] tBuff = new byte[2];    //pdoBuff
            tBuff = BitConverter.GetBytes(iVal);

            iRet = ELib.WriteTxPDO(Get_SlvID(iAx), iIdx, iSub, tBuff);
            if (iRet != (int)ErrorCode.None)
            {
                string e = ELib.ErrorToString(ELib.GetLastError());
                return iRet;
            }

            return iRet;
        }

        public int Read_SDO(int iAx, int iIdx, int iSub, ref int iVal)
        {
            int iRet = 0;
            byte[] tBuff = new byte[4];    //pdoBuff
            uint nSize = 0;    //actualSize
            uint nErr = 0;

            iRet = ELib.UploadSDO(Get_SlvID(iAx), iIdx, iSub, tBuff, ref nSize, ref nErr);
            if (iRet != (int)ErrorCode.None)
            {

                return iRet;
            }

            iVal = BitConverter.ToInt32(tBuff, 0);

            return iRet;
        }

        public int Write_SDO()
        {
            int iRet = 0;

            return iRet;
        }

            /// <summary>
            /// 전체 Slave 상태 체크 하여 정상이면 true반환
            /// </summary>
            /// <returns></returns>
        public bool IsAllAlive()
        {
            if (ELib == null)
            {
                return false;
            }

            ELib.GetStatus(m_EcInfo);

            for (uint i = 0; i < m_EcInfo.NumOfSlaves; i++)
            {
                if (m_EcInfo.Slaves[i].State != EcStateMachine.Op)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 연결이 끊어진 Slave가 있는지 확인, 있으면 true 반환
        /// </summary>
        /// <returns></returns>
        public bool IsLostSlave()
        {
            if (ELib == null)
            {
                return false;
            }

            ELib.GetStatus(m_EcInfo);

            for (uint i = 0; i < m_EcInfo.NumOfSlaves; i++)
            {
                if (m_EcInfo.Slaves[i].State == EcStateMachine.None
                    || m_EcInfo.Slaves[i].State == EcStateMachine.Init)
                {
                    return true;
                }
            }
            return false;
        }

        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sStat = CSQ_Main.It.m_iStat.ToString();
            string sLog = string.Format("{0},{1}(),{2}", sStat, sf.GetMethod().Name, sMsg);

            CLog.Save_Log(eLog.None, eLog.WMX, sLog);
        }
    }
}


