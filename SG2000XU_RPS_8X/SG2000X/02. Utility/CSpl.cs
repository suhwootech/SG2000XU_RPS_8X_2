using System;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
     public struct TSpin
    {
        public double dMinS;
        public double dMaxS;
    }
    public class CSpl : CStn<CSpl>
    {
        private readonly string LID = "203";
        private readonly string RID = "205";

        public SerialPort m_232C;
        public TSpin LeftCrrSpl;
        public TSpin RightCrrSpl;

        private CSpl()
        {              
        }

        public void _SetLog(EWay eWy, string sMsg, bool bData = false)
        {
            StackTrace mTrace = new StackTrace();
            StackFrame mFrame = mTrace.GetFrame(1);

            string sMth = mFrame.GetMethod().Name.PadRight(20);

            //200824 jhc : 
            string sStat = CSQ_Main.It.m_iStat.ToString();
            if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                sStat = string.Format("{0}[{1}]", sStat, (CData.iErrNo + 1).ToString("000"));
            }
            sStat.PadRight(20);

            string sLog = string.Format("{0}\t[{1}()]\t{2}", sStat, sMth, sMsg);
            CLog.Save_Log((eWy == EWay.L) ? eLog.GRL : eLog.GRR, eLog.None, sLog);
            if (bData)
            { CLog.Save_Log((eWy == EWay.L) ? eLog.GRL_DATA : eLog.GRR_DATA, eLog.None, sLog); }
        }

        public int Initial()
        {
            int iRet = 0;
            int iAx;
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                CData.Spls[0] = new tSplData();
                CData.Spls[0].aStat = new BitArray(16, false);
                CData.Spls[1] = new tSplData();
                CData.Spls[1].aStat = new BitArray(16, false);

                if (!CData.VIRTUAL && CData.WMX)
                {
                    m_232C = new SerialPort();
                    m_232C.PortName = "COM1";
                    m_232C.BaudRate = 115200;
                    m_232C.NewLine = "\r";
                    m_232C.DataReceived += M_mSerial_DataReceived;
                    m_232C.Open();

                    if (m_232C.IsOpen)
                    { Write_Init(); }
                }

                return iRet;
            }
            else
            {
                iAx = (int)EAx.LeftSpindle;
                LeftCrrSpl = new TSpin();
                iRet = Load(true);
                if (iRet == (iAx * 1000) + 40)
                {
                    Save(true);
                }
                iAx = (int)EAx.RightSpindle;
                RightCrrSpl = new TSpin();
                iRet = Load(false);
                if (iRet == (iAx * 1000) + 40)
                {
                    Save(false);
                }

                iRet = Write_Reset(EWay.L);
                GU.Delay(500);
                iRet = Write_Reset(EWay.R);
                GU.Delay(500);

                return iRet;
            }
        }

        public void Release()
        {//EtherCat 구문 없음, Can 만 사용
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                if (m_232C != null)
                {
                    if (m_232C.IsOpen)
                    { m_232C.Close(); }
                    m_232C.Dispose();
                    m_232C = null;
                }
            }
        }

        private void M_mSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {//Can만 사용
            byte[] aStat = new byte[2];
            string sVal = "";
            double dVal = 0;

            // t 185 8 40 72 00 00 00 00 FB FF
            string sData = m_232C.ReadLine();
            if (sData.Length != 21)
            { return; }
            sData = sData.Replace("t", "");
            string sID = sData.Substring(0, 3);
            string sLen = sData.Substring(3, 1);
            string sDt1 = sData.Substring(4, 2);
            string sDt2 = sData.Substring(6, 2);
            string sDt3 = sData.Substring(8, 2);
            string sDt4 = sData.Substring(10, 2);
            string sDt5 = sData.Substring(12, 2);
            string sDt6 = sData.Substring(14, 2);
            string sDt7 = sData.Substring(16, 2);
            string sDt8 = sData.Substring(18, 2);

            if (sID == "183")
            {
                // Status word
                aStat = new byte[] { (byte)Convert.ToInt32(sDt1, 16), (byte)Convert.ToInt32(sDt2, 16) };
                CData.Spls[0].aStat = new BitArray(aStat);
                
                // Actual speed value (RPM)
                aStat = new byte[] { (byte)Convert.ToInt32(sDt4, 16), (byte)Convert.ToInt32(sDt3, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");
                CData.Spls[0].iRpm = Calc_RpmToDec(sVal);
                
                // Drive errors
                aStat = new byte[] { (byte)Convert.ToInt32(sDt6, 16), (byte)Convert.ToInt32(sDt5, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");
                CData.Spls[0].sErr = sVal;

                // Actual current : 2022.09.01 lhs 추가
                aStat = new byte[] { (byte)Convert.ToInt32(sDt8, 16), (byte)Convert.ToInt32(sDt7, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");
                dVal = Math.Round(((double)GU.ToDec(sVal) * GV.SPINDLE_PEAK_CURRENT / 0x3fff) * 1000.0, 0);       // mA
                CData.Spls[0].nCurrent_Amp = (int)dVal;

            }
            else if (sID == "283")
            {
                aStat = new byte[] { (byte)Convert.ToInt32(sDt8, 16), (byte)Convert.ToInt32(sDt7, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");

                dVal = Math.Round(((double)GU.ToDec(sVal) / 0x3fff) * 100.0, 2);
                if (dVal < 380) 
                {
                    // 2021.12.03 lhs Start : Spindle Load가 EtherCat 타입보다 10배 크게 계산되어 1/10로 수정
                    //CData.Spls[0].dLoad = Math.Round((dVal * dVal * 100) / (32 * 32), 2); 
                    CData.Spls[0].dLoad = Math.Round((dVal * dVal * 10) / (32 * 32), 2);
                    // 2021.12.03 lhs End
                }
            }
            else if (sID == "185")
            {
                // Status word
                aStat = new byte[] { (byte)Convert.ToInt32(sDt1, 16), (byte)Convert.ToInt32(sDt2, 16) };
                CData.Spls[1].aStat = new BitArray(aStat);

                // Actual speed value (RPM)
                aStat = new byte[] { (byte)Convert.ToInt32(sDt4, 16), (byte)Convert.ToInt32(sDt3, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");
                CData.Spls[1].iRpm = Calc_RpmToDec(sVal);

                // Drive errors
                aStat = new byte[] { (byte)Convert.ToInt32(sDt6, 16), (byte)Convert.ToInt32(sDt5, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");
                CData.Spls[1].sErr = sVal;
                
                // Actual current : 2022.09.01 lhs 추가
                aStat = new byte[] { (byte)Convert.ToInt32(sDt8, 16), (byte)Convert.ToInt32(sDt7, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");
                dVal = Math.Round(((double)GU.ToDec(sVal) * GV.SPINDLE_PEAK_CURRENT / 0x3fff) * 1000.0, 0);       // mA
                CData.Spls[1].nCurrent_Amp = (int)dVal;

            }
            else if (sID == "285")
            {
                aStat = new byte[] { (byte)Convert.ToInt32(sDt8, 16), (byte)Convert.ToInt32(sDt7, 16) };
                sVal = BitConverter.ToString(aStat).Replace("-", "");

                dVal = Math.Round(((double)GU.ToDec(sVal) / 0x3fff) * 100.0, 2);
                if (dVal < 380) 
                {
                    // 2021.12.03 lhs Start : Spindle Load가 EtherCat 타입보다 10배 크게 계산되어 1/10로 수정
                    //CData.Spls[1].dLoad = Math.Round((dVal * dVal * 100) / (32 * 32), 2);
                    CData.Spls[1].dLoad = Math.Round((dVal * dVal * 10) / (32 * 32), 2);
                    // 2021.12.03 lhs End
                }
            }
        }

        private int Write_Msg(string sMsg)
        {//Can만 사용
            
            if(m_232C == null) 
                return 1;  // 2021.07.15 lhs : null이라 exception 에러 발생하여 추가

            if (!m_232C.IsOpen)
            { return 1; }

            m_232C.Write(sMsg);

            return 0;
        }

        public int Write_Init()
        {
            string sMsg = "";
            int iEr = 0;

            if (CDataOption.SplType == eSpindleType.Rs232)
            {//can 구문 추가
                sMsg += "t000";
                sMsg += "2";
                sMsg += "01";
                sMsg += "00";
                sMsg += "\r";

                iEr = Write_Msg(sMsg);
                GU.Delay(100);
            }

            iEr = Write_Stop(EWay.L);
            GU.Delay(100);
            iEr = Write_Stop(EWay.R);
            GU.Delay(100);
            iEr = Write_Reset(EWay.L);
            GU.Delay(100);
            iEr = Write_Reset(EWay.R);
            GU.Delay(100);

            return iEr;
        }

        /// <summary>
        /// Set RPM
        /// </summary>
        /// <param name="iId"> Spindle ID </param>
        /// <param name="iRpm"> RPM </param>
        /// <returns></returns>
        public int Write_Rpm(EWay eWy, int iRpm)
        {
            int iEr = 0;

            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                string sMsg = "t";
                string[] aRpm = new string[2];

                if (eWy == EWay.L) { sMsg += LID; }
                else { sMsg += RID; }

                sMsg += "8";
                aRpm = Calc_RpmToByte(iRpm);

                sMsg += "06";
                sMsg += "00";
                sMsg += aRpm[0];
                sMsg += aRpm[1];
                sMsg += "FF";
                sMsg += "3F";
                sMsg += "FF";
                sMsg += "3F";
                sMsg += "\r";

                iEr = Write_Msg(sMsg);

            }

            return iEr;
        }

        /// <summary>
        /// Run Spindle
        /// 중간에 속도 변경 할때 RunRpm만 호출할것
        /// SetRpm 호출 할시 Spindle 정지 됨
        /// </summary>
        /// <param name="iId"> Spindle Id </param>
        /// <param name="iRpm">RPM </param>
        /// <returns></returns>
        public int Write_Run(EWay eWy, int iRpm)
        {
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                string sMsg = "t";
                string[] aRpm = new string[2];
                
                int iEr = 0;

                if (eWy == EWay.L) { sMsg += LID; }
                else { sMsg += RID; }

                sMsg += "8";
                aRpm = Calc_RpmToByte(iRpm);

                sMsg += "0F";
                sMsg += "00";
                sMsg += aRpm[0];
                sMsg += aRpm[1];
                sMsg += "FF";
                sMsg += "3F";
                sMsg += "FF";
                sMsg += "3F";
                sMsg += "\r";

                iEr = Write_Msg(sMsg);

                return iEr;
            }
            else
            {
                int iRet = 0;
                int iVal = 0;

                if (eWy == EWay.L)
                {
                    iRet = CkRdy(eWy);
                    
                    if (iRet != 0)
                    {
                        return iRet;
                    }

                    if (iRpm < LeftCrrSpl.dMinS) { return (int)eSplError.EQUIPMENT_SPINDLE_RPM_UNDER_ZERO; }
                    if (iRpm > LeftCrrSpl.dMaxS) { return (int)eSplError.EQUIPMENT_SPINDLE_RPM_OVER_MAX; }

                    if (CkPurge(eWy) == false)
                    {
                        Purge(eWy, true);
                    }
                    
                    if (CkCool(eWy) == false)
                    {
                        Coolant(eWy, true);
                    }
                    
                    iRet = Spl_MaxCrr_Set(true, iRpm);
                    
                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_OVER_SET_CURRENT;
                    }

                    iRet = Spl_MaxCrr_Get(true, ref iVal);
                    
                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_CAN_NOT_GET_CURRENT;
                    }

                    if (iVal != iRpm)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_CAN_NOT_GET_CURRENT;
                    }

                    iRet = CWmx.It.WLib.basicVelocity.StartVel((short)EAx.LeftSpindle, (double)iRpm, (double)10000);
                    
                    if (iRet != 0)
                    {
                        string s = "";
                        CWmx.It.WLib.GetLastErrorString(ref s, 1000);

                        return (int)eSplError.EQUIPMENT_SPINDLE_RPM_RUN_ERROR;
                    }
                    return iRet;
                }
                else
                {
                    iRet = CkRdy(eWy);
                    
                    if (iRet != 0)
                    {
                        return iRet;
                    }

                    if (iRpm < RightCrrSpl.dMinS) { return (int)eSplError.EQUIPMENT_SPINDLE_RPM_UNDER_ZERO; }
                    if (iRpm > RightCrrSpl.dMaxS) { return (int)eSplError.EQUIPMENT_SPINDLE_RPM_OVER_MAX; }

                    if (CkPurge(eWy) == false)
                    {
                        Purge(eWy, true);
                    }
                    
                    if (CkCool(eWy) == false)
                    {
                        Coolant(eWy, true);
                    }
                    
                    iRet = Spl_MaxCrr_Set(false, iRpm);
                    
                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_OVER_SET_CURRENT;
                    }

                    iRet = Spl_MaxCrr_Get(false, ref iVal);
                    
                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_CAN_NOT_GET_CURRENT;
                    }

                    if (iVal != iRpm)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_CAN_NOT_GET_CURRENT;
                    }

                    iRet = CWmx.It.WLib.basicVelocity.StartVel((short)EAx.RightSpindle, (double)iRpm, (double)10000);
                    
                    if (iRet != 0)
                    {
                        string s = "";
                        CWmx.It.WLib.GetLastErrorString(ref s, 1000);

                        return (int)eSplError.EQUIPMENT_SPINDLE_RPM_RUN_ERROR;
                    }

                    return iRet;
                }
            }
        }

        /// <summary>
        /// Spindle Stop
        /// </summary>
        /// <param name="iId"> Spindle Id </param>
        /// <returns></returns>
        public int Write_Stop(EWay eWy)
        {
            int iRet = 0;

            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                string sMsg = "t";

                if (eWy == EWay.L) { sMsg += LID; }
                else { sMsg += RID; }

                sMsg += "2";
                sMsg += "06";
                sMsg += "00";
                sMsg += "\r";

                iRet = Write_Msg(sMsg);

                return iRet;
            }
            else
            {
                if (eWy == EWay.L)
                {
                    iRet = Spl_MaxCrr_Set(true, 0);

                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_OVER_SET_CURRENT;
                    }
                    iRet = CWmx.It.WLib.basicVelocity.Stop((int)EAx.LeftSpindle);

                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_STOP_ERROR;
                    }

                    return iRet;
                }
                else
                {
                    iRet = Spl_MaxCrr_Set(false, 0);

                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_OVER_SET_CURRENT;
                    }

                    iRet = CWmx.It.WLib.basicVelocity.Stop((int)EAx.RightSpindle);

                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_STOP_ERROR;
                    }

                    return iRet;
                }
            }
        }

        /// <summary>
        /// Fault reset
        /// 1. Command Fault reset (0x0080)
        /// 2. Command Shutdown (0x0060)
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        public int Write_Reset(EWay eWy)
        {
            int iEr = 0;

            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                string sMsg = "t";

                if (eWy == EWay.L) { sMsg += LID; }
                else { sMsg += RID; }

                sMsg += "2";
                sMsg += "80";
                sMsg += "00";
                sMsg += "\r";

                iEr = Write_Msg(sMsg);

                GU.Delay(500);

                sMsg = "t";

                if (eWy == EWay.L) { sMsg += LID; }
                else { sMsg += RID; }

                sMsg += "2";
                sMsg += "06";
                sMsg += "00";
                sMsg += "\r";

                iEr = Write_Msg(sMsg);

                return iEr;
            }
            else
            {
                int iRet = 0;

                if (eWy == EWay.L)
                {
                    iRet = CWmx.It.WLib.axisControl.ClearAmpAlarm((int)EAx.LeftSpindle);

                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_CAN_NOT_CLEAR_ALARM;
                    }
                }
                else
                {
                    iRet = CWmx.It.WLib.axisControl.ClearAmpAlarm((int)EAx.RightSpindle);

                    if (iRet != 0)
                    {
                        return (int)eSplError.EQUIPMENT_SPINDLE_CAN_NOT_CLEAR_ALARM;
                    }
                }

                return iEr;
            }
        }

        #region Calculate
        /// <summary>
        /// calculation RPM
        /// 계산 후 Byte 값으로 반환
        /// </summary>
        /// <param name="iRpm"> RPM </param>
        /// <returns></returns>
        public string[] Calc_RpmToByte(int iRpm)
        {
            string[] aRpm = new string[2];
            int iTp = 0;
            string sTp = "";

            iTp = 0x3fff * iRpm / 6000;
            sTp = iTp.ToString("X4");
            aRpm[0] = sTp.Substring(2, 2);
            aRpm[1] = sTp.Substring(0, 2);

            return aRpm;
        }

        /// <summary>
        /// Hex -> Decimal 
        /// </summary>
        /// <param name="sHex"></param>
        /// <returns></returns>
        public int Calc_RpmToDec(string sHex)
        {
            int iVal = GU.ToDec(sHex);
            iVal = (iVal * 6000) / 0x3fff;
            if(iVal > 20000)
            {
                iVal = 0;
            }
            return iVal;
        }
        #endregion

        /// <summary>
        /// Spindle ready 신호 확인
        /// </summary>
        /// <param name="eWy"></param>
        /// <returns></returns>
        public bool Chk_Servo(EWay eWy)
        {//Can만 사용, (호출하는 곳 없음)
            if (CDataOption.SplType == eSpindleType.Rs232)
            { return Convert.ToBoolean(CData.Spls[(int)eWy].aStat[0]); }
            else
            { return Convert.ToBoolean(CWmx.It.Stat.ServoOn[(eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle]); }
        }

        public bool Chk_Alarm(EWay eWy)
        {//Can만 사용, (호출하는 곳 없음)
            if (CDataOption.SplType == eSpindleType.Rs232)
            { return false; }// Convert.ToBoolean(CData.Spls[(int)eWy].aStat[0]); }
            else
            { return Convert.ToBoolean(CWmx.It.Stat.AmpAlarm[(eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle]); }
        }
        //public double GetLoad(bool isLeft)
        //{//can, ethercat 구문 같음
        //    if (CDataOption.SplType == eSpindleType.Rs232)
        //    {
        //        return CData.Spls[(isLeft) ? 0 : 1].dLoad;
        //    }
        //    else
        //    {
        //        int iInd = 8257;    //index 0x2041
        //        int iSub = 0;    //sub index
        //        int iVal = 0;
        //        int iRet = 0;

        //        if (isLeft == true)
        //        {
        //            iRet = CWmx.It.Read_SDO((int)EAx.LeftSpindle, iInd, iSub, ref iVal);
        //        }
        //        else
        //        {
        //            iRet = CWmx.It.Read_SDO((int)EAx.RightSpindle, iInd, iSub, ref iVal);
        //        }

        //        return iVal / 10.0;
        //    }
        //}

        /// <summary>
        /// 스핀들 부하량 [%]
        /// </summary>
        /// <param name="eWy">스핀들 방향</param>
        /// <returns></returns>
        public double Get_Load(EWay eWy)
        {
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                return CData.Spls[(int)eWy].dLoad;
            }
            else
            {
                int iAX = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
                int iInd = 8257;    //index 0x2041
                int iSub = 0;    //sub index
                int iVal = 0;
                int iRet = 0;
                iRet = CWmx.It.Read_SDO(iAX, iInd, iSub, ref iVal);

                return iVal / 10.0;
            }
        }
        
        /// <summary>
        /// 스핀들 온도 [%]
        /// </summary>
        /// <param name="eWy">스핀들 방향</param>
        /// - Object name: “Motor temp monitor actual”               
        /// - Object Number: 0x203F hex
        /// - Subindex: 0                  - Size: integer 16Bit
        /// - Scaling: the value 0.1°C
        /// <returns></returns>
        public double Get_Spindle_Temp(EWay eWy)
        {
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                return CData.Spls[(int)eWy].dLoad;
            }
            else
            {
                int iAX = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
                int iInd = 8255;    //index 0x203F
                int iSub = 0;       //sub index
                int iVal = 0;
                int iRet = 0;
                iRet = CWmx.It.Read_SDO(iAX, iInd, iSub, ref iVal);

                return iVal/10.0;
            }
        }

        /// <summary>
        /// 스핀들 소모 전류 [A]
        /// </summary>
        /// <param name="eWy">스핀들 방향</param>
        /// - Object name: “Motor 소모 전류 monitor actual”               
        /// - Object Number: 0x6078 hex
        /// - Subindex: 0                  - Size: integer 16Bit
        /// - Scaling: the value 1 => 인버터 정격 전류 설정/1000 
        ///   ex ) 1 -> 사용 전류 0.001[A] : 정격 32 A -> 부하율 0.003125 %
        /// <returns></returns>
        public int Get_Spindle_Current(EWay eWy) 
        {
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                ////return CData.Spls[(int)eWy].dLoad;
                //turn 0;
                return CData.Spls[(int)eWy].nCurrent_Amp;   // 2022.09.01 lhs 
            }
            else
            {
                int iAX = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
                int iInd = 0x6078;    //index 0x7078F
                int iSub = 0;       //sub index
                int iVal = 0;
                int iRet = 0;
                iRet = CWmx.It.Read_SDO(iAX, iInd, iSub, ref iVal);

                return (int)(GV.SPINDLE_CURRENT_COEFFICIENT * iVal * 1000); //[mA] 단위
            }
        }

        public int GetFrpm(bool isLeft)
        {//EtherCat, Can 같은 구문
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                return CData.Spls[(isLeft) ? 0 : 1].iRpm;
            }
            else
            {
                int iAx = (isLeft) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
                double dVal = 0.0;

                CWmx.It.WLib.axisControl.GetVelFeedback((short)iAx, ref dVal);

                return (int)GU.Truncate(dVal, 0);
            }
        }

        /// <summary>
        /// 현재 속도 [rpm]
        /// </summary>
        /// <param name="eWy">스핀들 방향</param>
        /// <returns></returns>
        public int Get_Frpm(EWay eWy)
        {   //EtherCat, Can 같은 구문
            if (CDataOption.SplType == eSpindleType.Rs232)
            {
                return CData.Spls[(int)eWy].iRpm;
            }
            else
            {
                short iAx = (eWy == EWay.L) ? (short)EAx.LeftSpindle : (short)EAx.RightSpindle;
                double dVal = 0.0;

                CWmx.It.WLib.axisControl.GetVelFeedback(iAx, ref dVal);

                return (int)GU.Truncate(dVal, 0);
            }
        }

        public int Save(bool isLeft)
        {//EtherCat 만 사용
            int iRet = 0;
            string sPath = string.Format("{0}Spindle_Parm.mot", GV.PATH_EQ_MOTION);
;

            if (Directory.Exists(GV.PATH_EQ_MOTION) == false)
            {
                Directory.CreateDirectory(GV.PATH_EQ_MOTION);
            }
            if(isLeft==true)
            {
                CIni mIni = new CIni(sPath);
                mIni.Write("LEFT", "MIN LIMIT", LeftCrrSpl.dMinS.ToString());
                mIni.Write("LEFT", "MAX LIMIT", LeftCrrSpl.dMaxS.ToString());
                return iRet;
            }
            else
            {
                CIni mIni = new CIni(sPath);
                mIni.Write("RIGHT", "MIN LIMIT", RightCrrSpl.dMinS.ToString());
                mIni.Write("RIGHT", "MAX LIMIT", RightCrrSpl.dMaxS.ToString());
                return iRet;
            }
        }

        public int Load(bool isLeft)
        {//EtherCat 만 사용
            //int iRet = (int)Error.NONE;
            int iRet = 0;
            int iAx = 0;
            string sPath = string.Format("{0}Spindle_Parm.mot", GV.PATH_EQ_MOTION);
            if (isLeft)
            {
                iAx = (int)EAx.LeftSpindle;
                if (File.Exists(sPath) == false)
                {
                    return (iAx * 1000) + 40;
                }
                CIni mIni = new CIni(sPath);
                double.TryParse(mIni.Read("LEFT", "MIN LIMIT"), out LeftCrrSpl.dMinS);
                double.TryParse(mIni.Read("LEFT", "MAX LIMIT"), out LeftCrrSpl.dMaxS);
                return iRet;
            }
            else
            {
                iAx = (int)EAx.RightSpindle;
                if (File.Exists(sPath) == false)
                {
                    return (iAx * 1000) + 40;
                }
                CIni mIni = new CIni(sPath);
                double.TryParse(mIni.Read("RIGHT", "MIN LIMIT"), out RightCrrSpl.dMinS);
                double.TryParse(mIni.Read("RIGHT", "MAX LIMIT"), out RightCrrSpl.dMaxS);
                return iRet;
            }
        }

        public int Spl_MaxCrr_Get(bool isLeft,ref int iVal)
        {//EtherCat 만 사용
            int iRet = 0;
            //_SetLog(ELog.DBG, "START>>>");
            //x6073 Read, Spindle Max Current
            if(isLeft== true)
            {
                int iInd = 24691;    //index
                int iSub = 0;    //sub index
                byte[] tBuff = new byte[2];    //pdoBuff

                iRet = CWmx.It.Read_PDO((int)EAx.LeftSpindle, iInd, iSub, ref iVal);
                if (iRet != 0)
                {
                    //_SetLog(ELog.SPL, string.Format("ERROR : Read PDO    WMX CODE : {0}",((ErrorCode)iRet).ToString()));
                    return iRet;
                }
                //_SetLog(ELog.SPL, string.Format("Max current get    VALUE : {0}", iVal));
                //_SetLog(ELog.DBG, "<<<FINISH");
                return iRet;
            }
            else
            {
                int iInd = 24691;    //index
                int iSub = 0;    //sub index
                byte[] tBuff = new byte[2];    //pdoBuff

                iRet = CWmx.It.Read_PDO((int)EAx.RightSpindle, iInd, iSub, ref iVal);
                if (iRet != 0)
                {
                    //_SetLog(ELog.SPL, string.Format("ERROR : Read PDO    WMX CODE : {0}",((ErrorCode)iRet).ToString()));
                    return iRet;
                }
                //_SetLog(ELog.SPL, string.Format("Max current get    VALUE : {0}", iVal));
                //_SetLog(ELog.DBG, "<<<FINISH");
                return iRet;

            }
        }

        public int Get_MaxCur(EWay eWy, ref int iVal)
        {   //EtherCat 만 사용
            int iAx = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
            int iRet = 0;
            int iInd = 24691;    //index
            int iSub = 0;    //sub index
            byte[] tBuff = new byte[2];    //pdoBuff
            // x6073 Read, Spindle Max Current
            iRet = CWmx.It.Read_PDO(iAx, iInd, iSub, ref iVal);
            if (iRet != 0)
            {
                return iRet;
            }

            return iRet;
        }

        public int Spl_MaxCrr_Set(bool isLeft,int iRPM)
        {//EtherCat 만 사용
            int iRet = 0;
            //_SetLog(ELog.DBG, "START>>>");
            //x6073 Write, Spindle Max Current
            int iInd = 24691;    //index
            int iSub = 0;    //sub index
            if(isLeft==true)
            {
                iRet = CWmx.It.Write_PDO((int)EAx.LeftSpindle, iInd, iSub, iRPM);
                if (iRet != 0)
                {
                    //_SetLog(ELog.SPL, string.Format("ERROR : Write PDO    WMX CODE : {0}",((ErrorCode)iRet).ToString()));
                    return iRet;
                }
                //_SetLog(ELog.SPL, string.Format("Max current set    VALUE : {0}", iRPM));
                //_SetLog(ELog.DBG, "<<<FINISH");
                return iRet;
            }
            else
            {
                iRet = CWmx.It.Write_PDO((int)EAx.RightSpindle, iInd, iSub, iRPM);
                if (iRet != 0)
                {
                    //_SetLog(ELog.SPL, string.Format("ERROR : Write PDO    WMX CODE : {0}",((ErrorCode)iRet).ToString()));
                    return iRet;
                }
                //_SetLog(ELog.SPL, string.Format("Max current set    VALUE : {0}", iRPM));
                //_SetLog(ELog.DBG, "<<<FINISH");
                return iRet;
            }
        }

        public int Set_MaxCur(EWay eWy, int iRPM)
        {   //EtherCat 만 사용
            int iAx = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
            int iRet = 0;            
            int iInd = 24691;    //index
            int iSub = 0;    //sub index
            // x6073 Write, Spindle Max Current
            iRet = CWmx.It.Write_PDO(iAx, iInd, iSub, iRPM);
            if (iRet != 0)
            {
                return iRet;
            }

            return iRet;
        }

        public int QStop(bool isLeft)
        {//EtherCat 만 사용
            int iRet = 0;
            if(isLeft==true)
            {
                //_SetLog(ELog.DBG, "START>>>");
                iRet = CWmx.It.WLib.basicVelocity.ExecQuickStop((int)EAx.LeftSpindle);
                if (iRet != 0)
                {
                    //_SetLog(ELog.SPL, string.Format("ERROR : Quick stop    WMX CODE : " + ((ErrorCode)iRet).ToString()));
                    return (int)eSplError.EQUIPMENT_SPINDLE_QUICK_STOP_ERROR;
                }
                //_SetLog(ELog.SPL, "Quick stop");
                //_SetLog(ELog.DBG, "<<<FINISH");
                return iRet;
            }
            else
            {
                //_SetLog(ELog.DBG, "START>>>");
                iRet = CWmx.It.WLib.basicVelocity.ExecQuickStop((int)EAx.RightSpindle);
                if (iRet != 0)
                {
                    //_SetLog(ELog.SPL, string.Format("ERROR : Quick stop    WMX CODE : " + ((ErrorCode)iRet).ToString()));
                    return (int)eSplError.EQUIPMENT_SPINDLE_QUICK_STOP_ERROR;
                }
                //_SetLog(ELog.SPL, "Quick stop");
                //_SetLog(ELog.DBG, "<<<FINISH");
                return iRet;

            }
        }

        public int CkRdy(EWay eWy)
        {//EtherCat 만 사용
            if (CkAlarm(eWy) == true) return (int)eSplError.EQUIPMENT_SPINDLE_ALARM;
            if (CkSrv(eWy) == false) return (int)eSplError.EQUIPMENT_SPINDLE_SERVO_OFF;
            return 0;
        }

        public bool CkSrv(EWay eWy)
        {//EtherCat 만 사용
            int iAx = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
            short nVal = CWmx.It.Stat.ServoOn[iAx];
            return Convert.ToBoolean(nVal);
        }

        public bool CkAlarm(EWay eWy)
        {//EtherCat 만 사용
            int iAx = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;
            short nVal = CWmx.It.Stat.AmpAlarm[iAx];
            return Convert.ToBoolean(nVal);
        }

        public bool CkPurge(EWay eWy)
        {//EtherCat 만 사용
            return CIO.It.Get_Y((eWy == EWay.L) ? eY.GRDL_SplCDA : eY.GRDR_SplCDA);
        }

        public int Purge(EWay eWy, bool bOn)
        {//EtherCat 만 사용
            int iRet  = Convert.ToInt32(CIO.It.Set_Y((eWy == EWay.L) ? eY.GRDL_SplCDA : eY.GRDR_SplCDA, bOn));
            return iRet;
        }

        public bool CkCool(EWay eWy)
        {//EtherCat 만 사용
            return CIO.It.Get_X((eWy == EWay.L) ? eX.GRDL_SplPCW : eX.GRDR_SplPCW);
        }

        public int Coolant(EWay eWy, bool bOn)
        {//EtherCat 만 사용
            int iRet = Convert.ToInt32(CIO.It.Set_Y((eWy == EWay.L) ? eY.GRDL_SplPCW : eY.GRDR_SplPCW, bOn));
            return iRet;
        }
        #region Position Mode 사용 함수
        /// <summary>
        /// SIEB&MEYER Ethercat (2)	포지션 모드에서 사용
        /// Position Actual Value ( object number: 0x6063 )
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="iVal"></param>
        /// <returns></returns>
        public int Get_Position_Value(int iAx, ref int iVal)
        {//EtherCat 만 사용
            int iRet = 0;
            int iInd = 0x6063;    //Position Actual Value ( object number: 0x6063 )
            int iSub = 0;        //sub index

            if ((iAx != (int)EAx.LeftSpindle) && (iAx != (int)EAx.RightSpindle))
            {
                iRet = -1;
                Console.WriteLine("Cmd_SetPara axis set error; {0}", iAx);
                return iRet;
            }
            else {
                iRet = CWmx.It.Read_SDO(iAx, iInd, iSub, ref iVal);
                if (iRet != 0)
                {
                    Console.WriteLine("Get_Position_Value iRet; {0}", iRet);
                    return iRet;
                }
                Console.WriteLine("Get_Position_Value = {0}", iVal);
            }
            return iRet;
        }
        public int Get_Position_Value(EWay eWy, ref int iVal)
        {//EtherCat 만 사용
            int iRet = 0;
            int iInd = 0x6063;    //Position Actual Value ( object number: 0x6063 )
            int iSub = 0;         //sub index
            int iAx;

            if ((eWy != EWay.L) && (eWy != EWay.R))
            {
                iRet = -1;
                Console.WriteLine("Cmd_SetPara axis set error >; {0}", eWy);
                return iRet;
            }
            else {
                iAx = (eWy == EWay.L) ? (int)EAx.LeftSpindle : (int)EAx.RightSpindle;

                iRet = CWmx.It.Read_SDO(iAx, iInd, iSub, ref iVal);
                if (iRet != 0)
                {
                    Console.WriteLine("Get_Position_Value iRet >; {0}", iRet);
                    return iRet;
                }
                Console.WriteLine("Get_Position_Value> = {0}", iVal);
            }
            return iRet;
        }

        #endregion
    }
}
