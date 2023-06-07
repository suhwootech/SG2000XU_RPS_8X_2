using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
namespace SG2000X
{
    public class CRfid : CStn<CRfid>
    {
        public SerialPort m_232C  ;
        public SerialPort m_232D  ; //20200120 ksg :
        public string ReciveMsg   ;
        public string ReciveMsgOnl; //200120 ksg :
        private CTim m_Delay = new CTim();
        private int OnlRecive = 0;
        private int offset = 0;
        private bool bStartSerial = false;
        private byte[] buff;
        
        private CRfid()
        {
              
        }

        public void Initial()
        {
            if (!CData.VIRTUAL && CData.WMX)
            {
                m_232C = new SerialPort();
                m_232C.PortName     = "COM6";
                m_232C.BaudRate     = 115200;
                m_232C.NewLine      = "\r";
                m_232C.DataBits     = (int)8;
                m_232C.Parity       = Parity.None;
                m_232C.StopBits     = StopBits.One;
                m_232C.ReadTimeout  = (int)500;
                m_232C.WriteTimeout = (int)500;
                m_232C.DataReceived += M_mSerial_DataReceived;
                m_232C.Open();

                if (m_232C.IsOpen)
                { Write_Init(); }
            }
            //200120 ksg : 
            if(CDataOption.OnlRfid == eOnlRFID.Use)
            {
                if (!CData.VIRTUAL && CData.WMX)
                {
                    m_232D = new SerialPort();
                    m_232D.PortName     = "COM1";
                    m_232D.BaudRate     = 115200;
                    m_232D.NewLine      = "\r";
                    m_232D.DataBits     = (int)8;
                    m_232D.Parity       = Parity.None;
                    m_232D.StopBits     = StopBits.One;
                    m_232D.ReadTimeout  = (int)500;
                    m_232D.WriteTimeout = (int)500;
                    m_232D.DataReceived += M_mSerial_DataReceived_Onl;
                    m_232D.Open();

                    if (m_232D.IsOpen)
                    { Write_Init(); }
                }
            }
        }

        public void Release()
        {
            if (m_232C != null)
            {
                if (m_232C.IsOpen)
                { m_232C.Close(); }
                m_232C.Dispose();
                m_232C = null;
            }
            //200120 ksg :
            if (m_232D != null)
            {
                if (m_232D.IsOpen)
                { m_232D.Close(); }
                m_232D.Dispose();
                m_232D = null;
            }

        }

        private void M_mSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            /*
            int intRecSize = m_232C.BytesToRead;
            string strRecData;
            string strTemp;
            char c;
            if (intRecSize != 0)
            {
                strRecData = "";
                byte[] buff = new byte[intRecSize];

                m_232C.Read(buff, 0, intRecSize);
                for (int iTemp = 0; iTemp < intRecSize; iTemp++)
                {
                    //strRecData += Convert.ToChar(buff[iTemp]);
                    c = Convert.ToChar(buff[iTemp]);
                    if(iTemp>9 && iTemp <25)
                    {
                        strTemp = c.ToString();
                        //if (strTemp != "\0" && strTemp != "\0A" && strTemp != "\u0001" && strTemp != "\u0013" && strTemp != "\u0010" && strTemp != "\u0015" && strTemp != "\u0012" && strTemp != "\u007f" && strTemp != "\u001e")
                        //if (strTemp != "\u01")
                        {
                                strRecData += strTemp;
                        }
                    }
                    
                }
                m_Delay.Wait(50);
                if(intRecSize > 8)
                ReciveMsg = strRecData;
            }
            */
            int intRecSize = m_232C.BytesToRead;
            string strRecData="";
            string strTemp;
            char c;
            if (offset == 0)
            {
                strRecData = "";
                buff = new byte[4096];
            }
            m_232C.Read(buff, offset, intRecSize);
            offset += intRecSize;
            for (int iTemp = 0; iTemp < offset; iTemp++)
            {
                c = Convert.ToChar(buff[iTemp]);
                if (c==0x7e)
                {
                    bStartSerial=true;
                }
                if (c==0x7f && bStartSerial)
                {
                    for (iTemp = 0; iTemp < offset; iTemp++)
                    {
                        //strRecData += Convert.ToChar(buff[iTemp]);
                        c = Convert.ToChar(buff[iTemp]);
                        strTemp = c.ToString();
                        if(iTemp>9 && iTemp <25)
                        {
                            strTemp = c.ToString();
                            {
                                strRecData += strTemp;
                            }
                        }
                    
                    }
                    ReciveMsg = strRecData;//test
                    offset=0;
                    bStartSerial=false;
                }
                    
            }

        }

        private void M_mSerial_DataReceived_Onl(object sender, SerialDataReceivedEventArgs e)
        {
            int intRecSize = m_232D.BytesToRead;
            string strRecData="";
            string strTemp;
            char c;
            if (offset == 0)
            {
                strRecData = "";
                buff = new byte[4096];
            }
            m_232D.Read(buff, offset, intRecSize);
            offset += intRecSize;
            for (int iTemp = 0; iTemp < offset; iTemp++)
            {
                c = Convert.ToChar(buff[iTemp]);
                if (c==0x7e)
                {
                    bStartSerial=true;
                }
                if (c==0x7f)
                {
                    for (iTemp = 0; iTemp < offset; iTemp++)
                    {
                        //strRecData += Convert.ToChar(buff[iTemp]);
                        c = Convert.ToChar(buff[iTemp]);
                        strTemp = c.ToString();
                        if(iTemp>9 && iTemp <25)
                        {
                            strTemp = c.ToString();
                            {
                                strRecData += strTemp;
                            }
                        }
                    
                    }
                    ReciveMsgOnl = strRecData;//test
                    offset=0;
                    bStartSerial=false;
                }
                    
            }
            /*
            m_Delay.Wait(50);
                //if(intRecSize > 8)
            ReciveMsgOnl = strRecData;
            if(intRecSize > 14)
            {
                if(OnlRecive == 0)ReciveMsgOnl = strRecData;
                else              ReciveMsgOnl += strRecData;
                OnlRecive++;
            }
            */
                
        }

        private int Write_Msg(string sMsg)
        {
            
            if (!m_232C.IsOpen)
            { return 1; }
            m_232C.Write(sMsg);
            
            return 0;
        }

        public int Write_Init()
        {
            int iEr = 0;
            return iEr;
        }
                     
        /// <summary>
        /// Set RPM
        /// </summary>
        /// <param name="iId"> Spindle ID </param>
        /// <param name="iRpm"> RPM </param>
        /// <returns></returns>
        public int ReadRfid(bool Btm)
        {
            int iEr = 0;

            byte[] byteSendData = new byte[200];
            int iSendCount = 0;
            string SendCmd;
            if(!Btm) SendCmd = "7E 01 01 00 05 41 10 00 00 10 41 7F"; //top
            else     SendCmd = "7E 01 01 00 05 41 10 01 00 10 40 7F"; //Btm
            try
            {
                foreach (string s in SendCmd.Split(' '))
                {
                    if (s != null && s != "")
                    {
                        byteSendData[iSendCount++] = Convert.ToByte(s, 16);
                        //cTemp = Convert.ToChar(byteSendData[iSendCount++]);
                        //Msg += cTemp;
                    }
                }
                //iEr = Write_Msg(Msg);
                m_232C.Write(byteSendData, 0, iSendCount);
            }
            catch (System.Exception ex)
            {
                
            }
            return iEr;
        }

        public int ReadRfidOnl(bool Btm)
        {
            int iEr = 0;

            byte[] byteSendData = new byte[200];
            int iSendCount = 0;
            string SendCmd;
            if(!Btm) SendCmd = "7E 01 01 00 05 41 10 00 00 10 41 7F"; //top
            else     SendCmd = "7E 01 01 00 05 41 10 01 00 10 40 7F"; //Btm
            try
            {
                foreach (string s in SendCmd.Split(' '))
                {
                    if (s != null && s != "")
                    {
                        byteSendData[iSendCount++] = Convert.ToByte(s, 16);
                        //cTemp = Convert.ToChar(byteSendData[iSendCount++]);
                        //Msg += cTemp;
                    }
                }
                //iEr = Write_Msg(Msg);
                OnlRecive = 0;
                m_232D.Write(byteSendData, 0, iSendCount);
                //m_Delay.Wait(1000);
                //Thread.Sleep(5000);
            }
            catch (System.Exception ex)
            {
                
            }
            return iEr;
        }
    }
}
