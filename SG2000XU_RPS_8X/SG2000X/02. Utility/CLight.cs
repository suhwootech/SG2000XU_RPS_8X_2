using System;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace SG2000X
{
    public class CLight : CStn<CLight>
    {
        /// <summary>
        /// OCR LightControl serial port
        /// </summary>
        private SerialPort m_232cLight;
        public string[] m_sCmd;
        public string[] m_sRec;
        
        private CLight()
        {
            m_sCmd = new string[(int)ELightCMD.MAX_ECMD];
            m_sRec = new string[(int)ELightCMD.MAX_ECMD];
        }

        public void Initial()
        {
            m_232cLight = new SerialPort();

            Load();
            Initiallize();
        }

        public void Initiallize()
        {
            m_sCmd[(int)ELightCMD.scOn1    ] = "N1000";
            m_sCmd[(int)ELightCMD.scOn2    ] = "N2000";
            m_sCmd[(int)ELightCMD.scOff1   ] = "F1000";
            m_sCmd[(int)ELightCMD.scOff2   ] = "F2000";
            m_sCmd[(int)ELightCMD.scBright1] = "B1"   ;
            m_sCmd[(int)ELightCMD.scBright2] = "B2"   ;

            m_sRec[(int)ELightCMD.scOn1    ] = "ON1" ;
            m_sRec[(int)ELightCMD.scOn2    ] = "ON2" ;
            m_sRec[(int)ELightCMD.scOff1   ] = "OFF1";
            m_sRec[(int)ELightCMD.scOff2   ] = "OFF2";
            m_sRec[(int)ELightCMD.scBright1] = "B1"  ;
            m_sRec[(int)ELightCMD.scBright2] = "B2"  ;
        }

        public void Release()
        {
            LightOff1();
            LightOff2();
            if (m_232cLight != null)
            {
                if (m_232cLight.IsOpen)
                { m_232cLight.Close(); }

                m_232cLight.Dispose();
                m_232cLight = null;
            }
        }

        /// <summary>
        /// Serial port open 상태 체크    false:Close  true:Open
        /// </summary>
        public bool Chk_Open()
        {
            FileInfo fi = new FileInfo(GV.PATH_EQ_LIGHT + "Light.lgt");
            if (fi.Exists)
            {
                return m_232cLight.IsOpen;
                //return true;
            }
            else
            { return false; }
        }

        public int Save()
        {
            string sPath = GV.PATH_EQ_LIGHT + "Light.lgt";
            StringBuilder mSB = new StringBuilder();
            if (!Directory.Exists(GV.PATH_EQ_LIGHT)) { Directory.CreateDirectory(GV.PATH_EQ_LIGHT); }
            

            mSB.AppendLine("[Light]");
            mSB.AppendLine("Port="      + CData.Light.sPort             );
            mSB.AppendLine("Baudrate="  + CData.Light.iBaud             );
            mSB.AppendLine("Databits="  + CData.Light.iDtBits           );
            mSB.AppendLine("Parity="    + CData.Light.eParity.ToString());
            mSB.AppendLine("Stopbits="  + CData.Light.eStBits.ToString());
            mSB.AppendLine("Rts="       + CData.Light.bRts   .ToString());
            mSB.AppendLine("LightVal1=" + CData.LightVal1    .ToString());
            mSB.AppendLine("LightVal2=" + CData.LightVal2    .ToString());
            //200213 ksg :
            CLog.Check_File_Access(sPath, mSB.ToString(), false);
            return 0;
        }

        public int Load()
        {
            string sSec = "";
            string sPath = GV.PATH_EQ_LIGHT + "Light.lgt";

            if (!File.Exists(sPath))
            { return 1; }
            CIni mIni = new CIni(sPath);

            sSec = "Light";
            CData.Light.sPort   = mIni.Read (sSec, "Port"    );
            CData.Light.iBaud   = mIni.ReadI(sSec, "Baudrate");
            CData.Light.iDtBits = mIni.ReadI(sSec, "Databits");
            Enum.TryParse(mIni.Read(sSec, "Parity"   ), out CData.Light.eParity);
            Enum.TryParse(mIni.Read(sSec, "Stopbits" ), out CData.Light.eStBits);
            bool.TryParse(mIni.Read(sSec, "Rts"      ), out CData.Light.bRts   );
            int .TryParse(mIni.Read(sSec, "LightVal1"), out CData.LightVal1    );
            int .TryParse(mIni.Read(sSec, "LightVal2"), out CData.LightVal2    );

            return 0;
        }

        public int Open_Port()
        {
            int iRet = 0;
            FileInfo fi = new FileInfo(GV.PATH_EQ_LIGHT + "Light.lgt");
            if (!fi.Exists)
            { return 0; }

            if (!m_232cLight.IsOpen)
            {
                iRet = Close_Port();

                m_232cLight.PortName  = CData.Light.sPort  ;
                m_232cLight.BaudRate  = CData.Light.iBaud  ;
                m_232cLight.Parity    = CData.Light.eParity;
                m_232cLight.DataBits  = CData.Light.iDtBits;
                m_232cLight.StopBits  = CData.Light.eStBits;
                m_232cLight.RtsEnable = CData.Light.bRts   ;
                m_232cLight.NewLine   = "\r\n";

                try
                {
                    m_232cLight.Open();
                }
                catch
                { }

                iRet = 1;
            }
            else
            {
                LightOn1();
                LightOn2();

                iRet = 0;
            }
            
            return iRet;
        }

        public int Close_Port()
        {
            int iRet = 0;
            FileInfo fi = new FileInfo(GV.PATH_EQ_LIGHT + "Light.lgt");
            if (!fi.Exists)
            { return 0; }
            if (m_232cLight == null)
            { return 0; }
            if (m_232cLight.IsOpen)
            {
                m_232cLight.Close();
            }

            if (!m_232cLight.IsOpen)
            { iRet = 0; }
            else
            { iRet = 1; }


            return iRet;
        }

        public string Write(string sMsg)
        {
            if (m_232cLight.IsOpen)
            {
                sMsg += "\r\n";
                m_232cLight.DiscardInBuffer();
                m_232cLight.Write(sMsg);

                GU.Delay(100);

                return m_232cLight.ReadExisting();
            }
            else
            {
                return "Not open=0\r\n";
            }
        }

        public string LightOn1()
        {
            string sRet = "";
            string sCmd = "";

            sCmd = m_sCmd[(int)ELightCMD.scOn1];
            sCmd += "\r\n";

            sRet = Write(sCmd);

            if(sRet != m_sRec[(int)ELightCMD.scOn1] + "\r\n")
            {
                sRet = "Faile On1";
            }
            
            return sRet;
        }

        public string LightOn2()
        {
            string sRet = "";
            string sCmd = "";

            sCmd = m_sCmd[(int)ELightCMD.scOn2];
            sCmd += "\r\n";

            sRet = Write(sCmd);

            if (sRet != m_sRec[(int)ELightCMD.scOn2] + "\r\n")
            {
                sRet = "Faile On2";
            }

            return sRet;
        }

        public string LightOff1()
        {
            string sRet = "";
            string sCmd = "";

            sCmd = m_sCmd[(int)ELightCMD.scOff1];
            sCmd += "\r\n";

            sRet = Write(sCmd);

            if (sRet != m_sRec[(int)ELightCMD.scOff1] + "\r\n")
            {
                sRet = "Faile Off1";
            }

            return sRet;
        }

        public string LightOff2()
        {
            string sRet = "";
            string sCmd = "";

            sCmd = m_sCmd[(int)ELightCMD.scOff2];
            sCmd += "\r\n";

            sRet = Write(sCmd);

            if (sRet != m_sRec[(int)ELightCMD.scOff2] + "\r\n")
            {
                sRet = "Faile Off2";
            }

            return sRet;
        }

        public string LightBright1(int iVal)
        {
            string sRet = "";
            string sCmd = "";

            sCmd = m_sCmd[(int)ELightCMD.scBright1];
            sCmd += iVal.ToString("000");
            sCmd += "\r\n";

            sRet = Write(sCmd);

            if (sRet != (m_sRec[(int)ELightCMD.scBright1] + iVal.ToString("000") + "\r\n"))
            {
                sRet = "Faile Bright1";
            }

            return sRet;
        }

        public string LightBright2(int iVal)
        {
            string sRet = "";
            string sCmd = "";

            sCmd = m_sCmd[(int)ELightCMD.scBright2];
            sCmd += iVal.ToString("000");
            sCmd += "\r\n";

            sRet = Write(sCmd);
                        

            if (sRet != (m_sRec[(int)ELightCMD.scBright2] + iVal.ToString("000") + "\r\n"))
            {
                sRet = "Faile Bright2";
            }

            return sRet;
        }
    }
}



