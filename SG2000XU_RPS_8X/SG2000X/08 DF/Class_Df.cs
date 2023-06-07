using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SG2000X
{ 
    public class CDf : CStn<CDf>
    {
        public CClient clientSocket;
        public string[] m_sProtocol;
        public int iCon = 0;
        public string sLog = "";
        private CDf()
        {
            clientSocket = new CClient();
            m_sProtocol = new string[(int)ECMD.MAX_ECMD];
        }

        public void Initial()
        {
            clientSocket = new CClient();
            Load();
            if (CData.dfInfo.sIp == null) { CData.dfInfo.sIp = "127.0.0.1"; }
            if (CData.dfInfo.iPort == 0) { CData.dfInfo.iPort = 9800; }
        }

        public void ConnectServer()
        {
            iCon = clientSocket.Connect(CData.dfInfo.sIp, CData.dfInfo.iPort);
        }

        public void DisConnectServer()
        {
            clientSocket.DisConnect();
        }

        public void SendID()
        {
            string sTemp = "";

            sTemp = "@ID," + CData.dfInfo.sId + ";";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendReady()
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + ",Ready;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendLotOpen()
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + ",LotOpen," + CData.dfInfo.sLotName + "," + CData.dfInfo.sGl + ";";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendLotExist()
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + ",LotExist;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendLotEnd()
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + ",LotEnd;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendInrBcr()
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + ",InrailBCR," + CData.dfInfo.sBcr + ";";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendResultBcr()
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + ",ResultBCR;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendGrdReady(string sWay)
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",GrdReady;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendBfStart(string sWay)
        {
            string sTemp = "";

            if(sWay == "LGD")
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",BfStart," + CData.GRLDfData.sBcr + ";";
            }
            else
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",BfStart," + CData.GRRDfData.sBcr + ";";
            }
            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendPcbData(string sWay)
        {
            string sTemp = "";

            if (sWay == "LGD")
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",PCB," + CData.GRLDfData.dPcb1 + "," + CData.GRLDfData.dPcb2 + "," + CData.GRLDfData.dPcb3 + "," + CData.GRLDfData.sPcbUse + ";";
            }
            else
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",PCB," + CData.GRRDfData.dPcb1 + "," + CData.GRRDfData.dPcb2 + "," + CData.GRRDfData.dPcb3 + "," + CData.GRRDfData.sPcbUse + ";";
            }

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendGrdRowData(string sWay, string sRowData)
        {
            string sTemp = "";
            
            if(sWay == "LGD")
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + "," + CData.GRLDfData.sRow + "," + sRowData + ";";
            }
            else
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + "," + CData.GRRDfData.sRow + "," + sRowData + ";";
            }

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendBfEnd(string sWay)
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",BfEnd;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendAfStart(string sWay)
        {
            string sTemp = "";

            if(sWay == "LGD")
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",AfStart," + CData.GRLDfData.sBcr + ";";
            }
            else
            {
                sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",AfStart," + CData.GRRDfData.sBcr + ";";
            }

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public void SendAfEnd(string sWay)
        {
            string sTemp = "";

            sTemp = "@" + CData.dfInfo.sId + "," + sWay + ",AfEnd;";

            clientSocket.sRet = "";
            clientSocket.SendData(sTemp);

            CData.dfInfo.bBusy = true;
        }

        public bool ReciveAck(int iCmd)
        {
            bool bRet = false;

            InitProtocol();

            if (clientSocket.sRet == m_sProtocol[iCmd])
            {
                bRet = true;
                CData.dfInfo.bBusy = false;
                sLog = clientSocket.sRet;
            }

            return bRet;            
        }

        public bool ReciveAckGRL(int iCmd)
        {
            bool bRet = false;
            InitProtocol();
            if(clientSocket.sRet == m_sProtocol[iCmd])
            {
                bRet = true;
                CData.dfInfo.bBusy = false;
                sLog = clientSocket.sRet;
            }
            
            return bRet;
        }

        public bool ReciveAckGRR(int iCmd)
        {
            bool bRet = false;
            InitProtocol();
            if (clientSocket.sRet == m_sProtocol[iCmd])
            {
                bRet = true;
                CData.dfInfo.bBusy = false;
                sLog = clientSocket.sRet;
            }

            return bRet;
        }

        public bool ReciveResultBcr()
        {
            bool bRet = false;
            string sTemp = "";
            string[] sData;

            

            sTemp = clientSocket.sRet;
            if(sTemp == "") {return false; }
            
            sData = sTemp.Split(',');
            if("ResultBCR" == sData[1])
            {
                if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
                {//DF 서버 사용 시 DF 측정 하는 사이트 사용
                    CData.dfInfo.dBcrMin = double.Parse(sData[2]);
                    CData.dfInfo.dBcrMax = double.Parse(sData[3]);
                    CData.dfInfo.dBcrAvg = double.Parse(sData[4]);
                    CData.dfInfo.sBcrUse = sData[5];
                }
                else
                {//DF 서버 사용 시 DF 측정 안 하는 사이트 사용
                    if(sData[2][sData[2].Length - 1] == ';' )
                    { 
                        sData[2] = sData[2].Remove(sData[2].Length - 1);
                    }
                    CData.dfInfo.dBcrMax = double.Parse(sData[2]);
                }

                bRet = true;
                CData.dfInfo.bBusy = false;
                sLog = clientSocket.sRet;
            }

            return bRet;
        }

        public int ReciveInrBcr()
        {
            int iRet = 0;

            InitProtocol();

            if(clientSocket.sRet == m_sProtocol[(int)ECMD.scInrBcrOk])
            {
                iRet = 0;
                CData.dfInfo.bBusy = false;
                sLog = clientSocket.sRet;
            }
            else if(clientSocket.sRet == m_sProtocol[(int)ECMD.scInrBcrNg])
            {
                iRet = 1;
                CData.dfInfo.bBusy = false;
                sLog = clientSocket.sRet;
            }
            else
            {
                iRet = -1;
            }

            return iRet;
        }

        public void InitProtocol()
        {
            //공용 Ack
            m_sProtocol[(int)ECMD.scId] = "@ID," + CData.dfInfo.sId + ",Ack;";
            m_sProtocol[(int)ECMD.scReady] = "@" + CData.dfInfo.sId + ",Ready,Ack;";
            m_sProtocol[(int)ECMD.scLotOpen] = "@" + CData.dfInfo.sId + ",LotOpen,Ack;";
            m_sProtocol[(int)ECMD.scLotExist] = "@" + CData.dfInfo.sId + ",LotExist,Ok;";
            m_sProtocol[(int)ECMD.scLotEnd] = "@" + CData.dfInfo.sId + ",LotEnd,Ack;";
            m_sProtocol[(int)ECMD.scInrBcrOk] = "@" + CData.dfInfo.sId + ",InrailBCR,Ok;";
            m_sProtocol[(int)ECMD.scInrBcrNg] = "@" + CData.dfInfo.sId + ",InrailBCR,Ng;";
            //Left Grind Ack
            m_sProtocol[(int)ECMD.scGrdReadyL] = "@" + CData.dfInfo.sId + ",LGD,GrdReady,Ack;";
            m_sProtocol[(int)ECMD.scBfStartL] = "@" + CData.dfInfo.sId + ",LGD,BfStart,Ack;";
            m_sProtocol[(int)ECMD.scPcbL] = "@" + CData.dfInfo.sId + ",LGD,PCB,Ack;";
            m_sProtocol[(int)ECMD.scRowL] = "@" + CData.dfInfo.sId + ",LGD," + CData.GRLDfData.sRow + ",Ack;";
            m_sProtocol[(int)ECMD.scBfEndL] = "@" + CData.dfInfo.sId + ",LGD,BfEnd,Ack;";
            m_sProtocol[(int)ECMD.scAfStartL] = "@" + CData.dfInfo.sId + ",LGD,AfStart,Ack;";
            m_sProtocol[(int)ECMD.scAfEndL] = "@" + CData.dfInfo.sId + ",LGD,AfEnd,Ack;";
            //Right Grind Ack
            m_sProtocol[(int)ECMD.scGrdReadyR] = "@" + CData.dfInfo.sId + ",RGD,GrdReady,Ack;";
            m_sProtocol[(int)ECMD.scBfStartR] = "@" + CData.dfInfo.sId + ",RGD,BfStart,Ack;";
            m_sProtocol[(int)ECMD.scPcbR] = "@" + CData.dfInfo.sId + ",RGD,PCB,Ack;";
            m_sProtocol[(int)ECMD.scRowR] = "@" + CData.dfInfo.sId + ",RGD," + CData.GRRDfData.sRow + ",Ack;";
            m_sProtocol[(int)ECMD.scBfEndR] = "@" + CData.dfInfo.sId + ",RGD,BfEnd,Ack;";
            m_sProtocol[(int)ECMD.scAfStartR] = "@" + CData.dfInfo.sId + ",RGD,AfStart,Ack;";
            m_sProtocol[(int)ECMD.scAfEndR] = "@" + CData.dfInfo.sId + ",RGD,AfEnd,Ack;";
        }

        public int Save()
        {
            string sPath = GV.PATH_EQ_DF + "DfServer.dfs";
            StringBuilder mSB = new StringBuilder();

            if (!Directory.Exists(GV.PATH_EQ_DF)) { Directory.CreateDirectory(GV.PATH_EQ_DF); }

            mSB.AppendLine("[DfServerInfo]");
            mSB.AppendLine("Ip="   + CData.dfInfo.sIp);
            mSB.AppendLine("Port=" + CData.dfInfo.iPort);
            mSB.AppendLine("Id="   + CData.dfInfo.sId);
            
            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            return 0;
        }

        public int Load()
        {
            string sSec = "";
            string sPath = GV.PATH_EQ_DF + "DfServer.dfs";

            if (!File.Exists(sPath))
            { return 1; }
            CIni mIni = new CIni(sPath);

            sSec = "DfServerInfo";
            CData.dfInfo.sIp = mIni.Read(sSec, "Ip");
            CData.dfInfo.iPort = mIni.ReadI(sSec, "Port");
            CData.dfInfo.sId = mIni.Read(sSec, "Id");

            return 0;
        }

        public bool CheckCon()
        {
            bool bRet = false;

            try
            {
                bRet = clientSocket.clientSocket.Connected;
            }
            catch
            { return false; }

            return bRet;
        }

        public void Release()
        {
            
        }
    }
}