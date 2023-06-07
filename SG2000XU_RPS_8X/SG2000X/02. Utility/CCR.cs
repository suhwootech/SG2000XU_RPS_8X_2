using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace SG2000X
{
    /// <summary>
    /// 220302 pjh : ASE KH Card reader 함수
    /// </summary>
    public class CCR : CStn<CCR>
    {
        //220325 pjh : Card Reader Port
        public SerialPort m_232cCard;
        private int offset = 0;
        private byte[] buff;
        private bool bStartSerial = false;
        public string ReciveMsg = "";
        /// <summary>
        /// 220325 pjh : Card Reader Check 변수
        /// </summary>
        public bool bCRRead = false;//Card Data가 들어왔는지 확인하는 변수
        public int iUserCnt = 0;//User 숫자 등록 변수
        public bool bUserForm = false;//User 등록 Form이 표시 되고 있는지 확인 하는 변수
        public Dictionary<string, string> dicUser = new Dictionary<string, string>(); // Data 저장

        private CCR() { }

        //220325 pjh : Card Reader Initial 함수. 포트 7고정
        public void Initial()
        {
            // 2022.04.20 SungTae Start : [수정] ASE-KH Card Reader 관련
            // Card Reader 미장착 상태에서 License Option이 없으면 Port Open 인해 Motion 설정이 바뀌는 문제가 있어서 Option 추가
            //if (!CData.VIRTUAL && CData.WMX)
            if (!CData.VIRTUAL && CData.WMX && CDataOption.UseCardReader)
            // 2022.04.20 SungTae End
            {
                m_232cCard = new SerialPort();
                m_232cCard.PortName = "COM7";
                m_232cCard.BaudRate = 9600;
                m_232cCard.NewLine = "\r";
                m_232cCard.DataBits = 8;
                m_232cCard.Parity = Parity.None;
                m_232cCard.StopBits = StopBits.One;
                m_232cCard.ReadTimeout = (int)500;
                m_232cCard.WriteTimeout = (int)500;
                m_232cCard.DataReceived += M_mSerial_DataReceive_CardReader;
                m_232cCard.Open();
            }
        }
        //

        //220325 pjh : Card Reader Release 함수
        public void Release()
        {
            if (m_232cCard != null)
            {
                if (m_232cCard.IsOpen)
                {
                    m_232cCard.Close();
                    m_232cCard.Dispose();
                    m_232cCard = null;
                }
            }
        }
        //

        //220325 pjh : Card Reader Data Read 함수
        public void M_mSerial_DataReceive_CardReader(object sendor, SerialDataReceivedEventArgs e)
        {
            int iRecSize = m_232cCard.BytesToRead;
            string strRecData = "";
            string strTemp;
            char c;
            bCRRead = false;

            if (offset == 0)
            {
                strRecData = "";
                buff = new byte[4096];
            }
            m_232cCard.Read(buff, offset, iRecSize);
            offset += iRecSize;
            for (int iTemp = 0; iTemp < offset; iTemp++)
            {
                c = Convert.ToChar(buff[iTemp]);
                if (c == 0x02)
                {
                    bStartSerial = true;
                }
                if (c == 0x03 && bStartSerial)
                {
                    for (iTemp = 0; iTemp < offset; iTemp++)
                    {
                        c = Convert.ToChar(buff[iTemp]);
                        strTemp = c.ToString();
                        if (iTemp > 0 && iTemp < 11)
                        {
                            strTemp = c.ToString();
                            {
                                strRecData += strTemp;
                            }
                        }
                    }
                    ReciveMsg = strRecData;//test
                    bCRRead = true;
                    offset = 0;
                    bStartSerial = false;
                }
            }
        }
        //
        //220325 pjh : User Data 받아서 저장하는 함수
        public int Save(string sPath)
        {
            int iRet = 0;
            int iUserNo = 1;

            StringBuilder mSB = new StringBuilder();

            if (!Directory.Exists(GV.PATH_USER))
            { Directory.CreateDirectory(GV.PATH_USER); }

            mSB.AppendLine("[User Count]");
            mSB.AppendLine("Count=" + dicUser.Count.ToString());
            mSB.AppendLine("");
            
            mSB.AppendLine("[User Information]");

            //Dictionary 내부 Item들 저장
            foreach(var pair in dicUser)
            {
                mSB.AppendLine("[User" + iUserNo.ToString() + "]");
                mSB.AppendLine("User ID=" + pair.Key);
                mSB.AppendLine("User Level=" + pair.Value);
                mSB.AppendLine("");

                iUserNo++;
            }
            //

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            if (File.Exists(sPath) == false)
            { iRet = 1; }

            return iRet;
        }

        //220325 pjh : 저장 된 Data 불러오는 함수
        public int Load(string sPath, int iCnt)
        {
            int iRet = 0;
            string sSec = "";
            CIni mIni = new CIni(sPath);
            dicUser.Clear();


            for (int i = 0; i < iCnt; i++)
            {
                sSec = "User" + (i+1).ToString();
                dicUser.Add(mIni.Read(sSec, "User ID"), mIni.Read(sSec, "User Level"));
            }
            return iRet;
        }
        //220325 pjh : User Count 불러오는 함수
        public void GetCount(string sPath)
        {
            string sSec = "";
            iUserCnt = 0;

            CIni mIni = new CIni(sPath);
            sSec = "User Count";
            iUserCnt = int.Parse(mIni.Read(sSec, "Count"));
        }
    } 
}
