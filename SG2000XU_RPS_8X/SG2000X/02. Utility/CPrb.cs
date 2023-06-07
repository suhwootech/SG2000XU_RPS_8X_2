using System;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace SG2000X
{
    public class CPrb : CStn<CPrb>
    {
        /// <summary>
        /// Left probe serial port
        /// </summary>
        private SerialPort m_232cL;
        /// <summary>
        /// Right probe serial port
        /// </summary>
        private SerialPort m_232cR;

        //20190220 ghk_dynamicfunction
        /// <summary>
        /// Inrail probe serial port
        /// </summary>
        private SerialPort m_232cInr;
        //

        private static double[] bufValProbe = new double[3]; //koo WriteLog
        public double GetBufValProbe(int axis)
        {
            return bufValProbe[axis];
        }

        private CPrb() { }

        public void Initial()
        {
            m_232cL = new SerialPort();
            m_232cR = new SerialPort();
            //20190220 ghk_dynamicfunction
            m_232cInr = new SerialPort();
            //
            Load();
        }

        public void Release()
        {
            if (m_232cL != null)
            {
                if (m_232cL.IsOpen)
                { m_232cL.Close(); }
                m_232cL.Dispose();
                m_232cL = null;
            }

            if (m_232cR != null)
            {
                if (m_232cR.IsOpen)
                { m_232cR.Close(); }
                m_232cR.Dispose();
                m_232cR = null;
            }

            //20190220 ghk_dynamicfunction
            if (m_232cInr != null)
            {
                if (m_232cInr.IsOpen)
                { m_232cInr.Close(); }
                m_232cInr.Dispose();
                m_232cInr = null;
            }
            //
        }

        /// <summary>
        /// Serial port open 상태 체크    false:Close  true:Open
        /// </summary>
        /// <param name="eWy"></param>
        /// <returns></returns>
        public bool Chk_Open(EWay eWy)
        {
            if (eWy == EWay.L)
            { return m_232cL.IsOpen; }
            else if (eWy == EWay.R)
            { return m_232cR.IsOpen; }

            //20190220 ghk_dynamicfunction
            else
            { return m_232cInr.IsOpen; }
            //
        }

        public int Save()
        {
            string sPath = GV.PATH_EQ_PROBE + "Probe.prb";
            StringBuilder mSB = new StringBuilder();
            if (!Directory.Exists(GV.PATH_EQ_PROBE)) 
            { Directory.CreateDirectory(GV.PATH_EQ_PROBE); }

            mSB.AppendLine("[Probe Left]");
            mSB.AppendLine("Port="     + CData.Prbs[0].sPort             );
            mSB.AppendLine("Baudrate=" + CData.Prbs[0].iBaud             );
            mSB.AppendLine("Databits=" + CData.Prbs[0].iDtBits           );
            mSB.AppendLine("Parity="   + CData.Prbs[0].eParity.ToString());
            mSB.AppendLine("Stopbits=" + CData.Prbs[0].eStBits.ToString());
            mSB.AppendLine("Rts="      + CData.Prbs[0].bRts   .ToString());
            //syc: Probe Auto Calibration
            mSB.AppendLine("LX="       + CData.Prbs[0].dX     .ToString());
            mSB.AppendLine("LY="       + CData.Prbs[0].dY     .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[Probe Right]");
            mSB.AppendLine("Port="     + CData.Prbs[1].sPort             );
            mSB.AppendLine("Baudrate=" + CData.Prbs[1].iBaud             );
            mSB.AppendLine("Databits=" + CData.Prbs[1].iDtBits           );
            mSB.AppendLine("Parity="   + CData.Prbs[1].eParity.ToString());
            mSB.AppendLine("Stopbits=" + CData.Prbs[1].eStBits.ToString());
            mSB.AppendLine("Rts="      + CData.Prbs[1].bRts   .ToString());
            //syc: Probe Auto Calibration
            mSB.AppendLine("RX="       + CData.Prbs[1].dX     .ToString());
            mSB.AppendLine("RY="       + CData.Prbs[1].dY     .ToString());
            mSB.AppendLine();
            //20190220 ghk_dynamicfunction
            mSB.AppendLine("[Probe Inrail]");
            mSB.AppendLine("Port="     + CData.Prbs[2].sPort             );
            mSB.AppendLine("Baudrate=" + CData.Prbs[2].iBaud             );
            mSB.AppendLine("Databits=" + CData.Prbs[2].iDtBits           );
            mSB.AppendLine("Parity="   + CData.Prbs[2].eParity.ToString());
            mSB.AppendLine("Stopbits=" + CData.Prbs[2].eStBits.ToString());
            mSB.AppendLine("Rts="      + CData.Prbs[2].bRts   .ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            return 0;
        }

        public int Load()
        {
            string sSec = "";
            string sPath = GV.PATH_EQ_PROBE + "Probe.prb";

            if (!File.Exists(sPath))
            { return 1; }
            CIni mIni = new CIni(sPath);

            sSec = "Probe Left";
            CData.Prbs[0].sPort = mIni.Read(sSec, "Port");
            CData.Prbs[0].iBaud = mIni.ReadI(sSec, "Baudrate");
            CData.Prbs[0].iDtBits = mIni.ReadI(sSec, "Databits");
            Enum.TryParse(mIni.Read(sSec, "Parity"), out CData.Prbs[0].eParity);
            Enum.TryParse(mIni.Read(sSec, "Stopbits"), out CData.Prbs[0].eStBits);
            bool.TryParse(mIni.Read(sSec, "Rts"), out CData.Prbs[0].bRts);
            //syc : Probe Auto Calibration
            CData.Prbs[0].dX = mIni.ReadD(sSec, "LX");
            CData.Prbs[0].dY = mIni.ReadD(sSec, "LY");

            sSec = "Probe Right";
            CData.Prbs[1].sPort = mIni.Read(sSec, "Port");
            CData.Prbs[1].iBaud = mIni.ReadI(sSec, "Baudrate");
            CData.Prbs[1].iDtBits = mIni.ReadI(sSec, "Databits");
            Enum.TryParse(mIni.Read(sSec, "Parity"), out CData.Prbs[1].eParity);
            Enum.TryParse(mIni.Read(sSec, "Stopbits"), out CData.Prbs[1].eStBits);
            bool.TryParse(mIni.Read(sSec, "Rts"), out CData.Prbs[1].bRts);
            //syc : Probe Auto Calibration
            CData.Prbs[1].dX = mIni.ReadD(sSec, "RX");
            CData.Prbs[1].dY = mIni.ReadD(sSec, "RY");

            //20190220 ghk_dynamicfunction
            sSec = "Probe Inrail";
            CData.Prbs[2].sPort = mIni.Read(sSec, "Port");
            CData.Prbs[2].iBaud = mIni.ReadI(sSec, "Baudrate");
            CData.Prbs[2].iDtBits = mIni.ReadI(sSec, "Databits");
            Enum.TryParse(mIni.Read(sSec, "Parity"), out CData.Prbs[2].eParity);
            Enum.TryParse(mIni.Read(sSec, "Stopbits"), out CData.Prbs[2].eStBits);
            bool.TryParse(mIni.Read(sSec, "Rts"), out CData.Prbs[2].bRts);

            return 0;
        }

        public int Open_Port(EWay eWy)
        {
            int iRet = 0;

            if (eWy == EWay.L)
            {
                iRet = Close_Port(EWay.L);

                m_232cL.PortName = CData.Prbs[0].sPort;
                m_232cL.BaudRate = CData.Prbs[0].iBaud;
                m_232cL.Parity = Parity.None;
                m_232cL.DataBits = 8;
                m_232cL.StopBits = StopBits.One;
                m_232cL.RtsEnable = false;
                m_232cL.NewLine = "\r\n";

                m_232cL.Open();
                iRet = (m_232cL.IsOpen) ? 0 : 1;
            }
            else if (eWy == EWay.R)
            {
                iRet = Close_Port(EWay.R);

                m_232cR.PortName = CData.Prbs[1].sPort;
                m_232cR.BaudRate = CData.Prbs[1].iBaud;
                m_232cR.Parity = Parity.None;
                m_232cR.DataBits = 8;
                m_232cR.StopBits = StopBits.One;
                m_232cR.RtsEnable = false;
                m_232cR.NewLine = "\r\n";

                m_232cR.Open();
                iRet = (m_232cR.IsOpen) ? 0 : 1;
            }
            //20190220 ghk_dynamicfunction
            else
            {
                iRet = Close_Port(EWay.INR);

                m_232cInr.PortName = CData.Prbs[2].sPort;
                m_232cInr.BaudRate = CData.Prbs[2].iBaud;
                m_232cInr.Parity = Parity.None;
                m_232cInr.DataBits = 8;
                m_232cInr.StopBits = StopBits.One;
                m_232cInr.RtsEnable = false;
                m_232cInr.NewLine = "\r\n";

                m_232cInr.Open();
                iRet = (m_232cInr.IsOpen) ? 0 : 1;
            }

            return iRet;
        }

        public int Close_Port(EWay eWy)
        {
            int iRet = 0;

            if (eWy == EWay.L)
            {
                if (m_232cL.IsOpen)
                {
                    m_232cL.Close();
                }

                if (!m_232cL.IsOpen)
                { iRet = 0; }
                //20190227 ghk_err
                /*
                else
                { iRet = (int)eErr.PROBE_LEFT_PORT_NOT_CLOSE; }
                */
                else
                { iRet = 1; }
            }
            else if (eWy == EWay.R)
            {
                if (m_232cR.IsOpen)
                {
                    m_232cR.Close();
                }

                if (!m_232cR.IsOpen)
                { iRet = 0; }
                //20190227 ghk_err
                /*
                else
                { iRet = (int)eErr.PROBE_RIGHT_PORT_NOT_CLOSE; }
                */
                else
                { iRet = 1; }
            }
            //20190220 ghk_dynamicfunction
            else
            {
                if (m_232cInr.IsOpen)
                {
                    m_232cInr.Close();
                }

                if (!m_232cInr.IsOpen)
                { iRet = 0; }
                //20190227 ghk_err
                /*
                else
                { iRet = (int)eErr.PROBE_INRAIL_PORT_NOT_CLOSE; }
                */
                else
                { iRet = 1; }
            }
            //
            return iRet;
        }

        public string Write(EWay eWy, string sMsg)
        {
            if (eWy == EWay.L)
            {
                if (m_232cL.IsOpen)
                {
                    sMsg += "\r\n";
                    m_232cL.DiscardInBuffer();
                    m_232cL.Write(sMsg);
                    GU.Delay(100);
                    return m_232cL.ReadExisting();
                }
                else
                {
                    return "Not open=0\r\n";
                }
            }
            else if (eWy == EWay.R)
            {
                if (m_232cR.IsOpen)
                {
                    sMsg += "\r\n";
                    m_232cR.DiscardInBuffer();
                    m_232cR.Write(sMsg);
                    GU.Delay(100);
                    return m_232cR.ReadExisting();
                }
                else
                {
                    return "Not open=0\r\n";
                }
            }
            //20190220 ghk_dynamicfunction
            else
            {
                if (m_232cInr.IsOpen)
                {
                    sMsg += "\r\n";
                    m_232cInr.DiscardInBuffer();
                    m_232cInr.Write(sMsg);
                    GU.Delay(100);
                    return m_232cInr.ReadExisting();
                }
                else
                {
                    return "Not open=0\r\n";
                }
            }
        }

        public double Read_Val(EWay eWy)
        {
            /* //ASE Korea 프로브 펌웨어 업데이트로 인하여 분기
            string sVal = Write(eWy, ">R01");
            string[] asVal = sVal.Split('=');
            double dVal = 0;
            if (asVal.Length > 1)
            { double.TryParse(asVal[1], out dVal); }
            return dVal;
            */
            //Qorvo
            double dVal = 0.0;
            string sVal = "";
            string sTemp = "";
            string[] sRet;

            if (eWy == EWay.INR)
            {//인레일 프로브 제픔 다름
                //190910 ksg : Probe Type 추가
                if(CDataOption.eDfType == eDfProbe.Keyence)
                {
                    sVal = Write(eWy, "sr,00,000\r\n");
                    sRet = sVal.Split(',');
                    if (sRet.Length > 3)
                    { sTemp = sRet[3]; }
                    else
                    { sTemp = "999.999"; }
                }
                else
                {
                    sVal = Write(eWy, ">R01");
                    sTemp = sVal.Remove(0, 5);
                }
                
            }
            else
            {
                sVal = Write(eWy, ">R01");
                sTemp = sVal.Remove(0, 5);
            }
            double.TryParse(sTemp, out dVal);
            bufValProbe[(int)eWy] = dVal;
            return dVal;
        }
        public string WriteCal(EWay eWy, string sMsg)
        {
            if (eWy == EWay.L)
            {
                if (m_232cL.IsOpen)
                {
                    sMsg += "\r\n";
                    m_232cL.DiscardInBuffer();
                    m_232cL.Write(sMsg);
                    GU.Delay(100);
                    return m_232cL.ReadExisting();
                }

                else
                {
                    return "Not open=0";
                }
            }

            else
            {
                if (m_232cR.IsOpen)
                {
                    sMsg += "\r\n";
                    m_232cR.DiscardInBuffer();
                    m_232cR.Write(sMsg);
                    GU.Delay(100);
                    return m_232cR.ReadExisting();
                }

                else
                {
                    return "Not open=0";
                }
            }
        }
    }
}



