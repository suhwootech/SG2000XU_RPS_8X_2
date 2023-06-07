using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SG2000X
{   
    public class CClient
    {
        public Socket clientSocket;
        public string sRet = "";
        public string sErr = "";

        public CClient() {}

        public void Initial()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public int Connect(string sIp, int iPort)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                clientSocket.Connect(sIp, iPort);
                Class_AsyncObject obj = new Class_AsyncObject(4096);
                obj.WorkingSocket = clientSocket;
                clientSocket.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

                SaveLog("Connected", true);
                return 1;
            }
            catch (SocketException ex)
            {
                SaveLog("Connected Error", false);
                return -1;
            }
        }

        public void DisConnect()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
                SaveLog("DisConnected", true);
            }
        }

        public void DataReceived(IAsyncResult ar)
        {
            Class_AsyncObject obj = (Class_AsyncObject)ar.AsyncState;

            if (!obj.WorkingSocket.Connected)
            { return; }
            try
            {
                int iReceived = obj.WorkingSocket.EndReceive(ar);

                if (iReceived <= 0)
                {
                    obj.WorkingSocket.Close();
                    return;
                }

                string sTemp = Encoding.UTF8.GetString(obj.Buffer);

                string[] tokens = sTemp.Split(new char[] { '\0' });
                string sMsg = tokens[0];

                sRet = sMsg;
                CDf.It.sLog = sMsg;

                obj.ClearBuffer();

                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
                SaveLog(sRet, false);
            }
            catch(SocketException ex)
            {
                return;
            }
        }

        public void SendData(string sMsg)
        {
            if(clientSocket == null)
            {
                SaveLog("Not Connected", true);
                return;
            }
            if (!clientSocket.Connected)
            {
                SaveLog("Not Connected", true);
                return;
            }

            IPEndPoint ip = (IPEndPoint)clientSocket.LocalEndPoint;
            string sAdd = ip.Address.ToString();

            byte[] bDts = Encoding.UTF8.GetBytes(sMsg);
            clientSocket.Send(bDts);

            SaveLog(sMsg, true);
        }

        public void SaveLog(string sTemp, bool bSend)
        {
            string sLog = "";
            string sPath = "d:\\DfServerLog\\";
            DateTime dt = DateTime.Now;

            sPath += dt.ToString("yyyyMM") + "\\";
            sPath += dt.ToString("dd") + "\\";
            sPath += dt.ToString("HH") + "\\";
            /*
            df = new DirectoryInfo(sPath);
            if (!df.Exists)
            { df.Create(); }
            */
            if (!Directory.Exists(sPath.ToString()))
            {
                Directory.CreateDirectory(sPath.ToString());
            }

            //sPath += "DfServerLog.Log";
            /*
            fi = new FileInfo(sPath);
            if (!fi.Exists)
            { fi.Create().Close(); }

            sw = new StreamWriter(sPath, true, Encoding.GetEncoding("euc-kr"));
            */
            sLog = DateTime.Now.ToString("HH:mm:ss") + "\t";
            
            if (bSend)
            { sLog += ">>\t"; }
            else
            { sLog += "<<\t"; }

            sLog += sTemp;
            //sw.WriteLine(sLog);

            //sw.Close();
            CLogManager Log = new CLogManager(sPath, null, null);
            Log.WriteLine(sLog);   
        }
    }
}
