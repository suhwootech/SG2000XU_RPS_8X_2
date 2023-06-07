using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace TCPIPDLL_Client
{
    public class Tcpip_Client
    {

        public static NetworkStream stream;
        private static bool bRcvDataFromServer;
        private static string strRcvDataFromServer;

        private static string strStatus;



        public static TcpClient Client;

        public static StreamReader Reader;

        public static StreamWriter Writer;



        public static Thread ReceiveThread;
        public static Thread SendThread;


        public static bool Connected;
        //private delegate void AddTextDelegate(string strText);

        static Queue<string> quResvMsgs;
        static Queue<string> quSendMsgs;
        static bool isNew = false;

        public static string GetVersion()
        {
            return "2.22";
        }
        public static bool GetIsRcvDataFromServer()
        {
            if (!isNew)
                return bRcvDataFromServer;
            else
                return GetIsRcvDataFromServerNew();
        }
        public static bool GetIsRcvDataFromServerNew()
        {
            if (quResvMsgs.Count > 0)
                return true;
            else
                return false;
        }

        public static void ClearRcvDataFromServer()
        {
            strRcvDataFromServer = "";
        }
        public static string GetRcvDataFromServer()
        {
            if (!isNew)
            {
                bRcvDataFromServer = false;
                return strRcvDataFromServer;
            }
            else
            {
                return GetRcvDataFromServerNew();
            }

        }

        public static string GetRcvDataFromServerNew()
        {
            if (quResvMsgs.Count > 0)
                return quResvMsgs.Dequeue();
            else
                return "Error string";

        }

        public static string GetStatusServer()
        {

            return strStatus;
        }

        public static bool IsConnect()
        {
            try
            {
                if (Client != null && Client.Client != null && Client.Client.Connected)
                {
                    // Detect if client disconnected 
                    if (Client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (Client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected 
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool Connect(string IP, int Port)
        {
            if (IsConnect() == true) return false;
            Connected = false;
            try
            {
                Client = new TcpClient();
                Client.Connect(IP, Port);
            }
            catch (Exception e)
            {
                //throw e;
                return false;
            }
            stream = Client.GetStream();

            //stream.ReadTimeout = 900000;
            //tc.ReceiveTimeout = 0;

            Connected = true;
            //textBox1.AppendText("Connected to Server!" + "\r\n");
            strStatus = "Connected to Server!";
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);

            ReceiveThread = new Thread(new ThreadStart(Receive));
            ReceiveThread.Start();

            return true;
        }

        public static bool Connect(string IP, int Port, bool setNew)
        {
            if (IsConnect() == true) return false;
            Connected = false;
            try
            {
                Client = new TcpClient();
                Client.Connect(IP, Port);
            }
            catch (Exception e)
            {
                //throw e;
                return false;
            }
            stream = Client.GetStream();

            //stream.ReadTimeout = 900000;
            //tc.ReceiveTimeout = 0;

            Connected = true;
            //textBox1.AppendText("Connected to Server!" + "\r\n");
            strStatus = "Connected to Server!";
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);
            if (setNew)
            {
                quResvMsgs = new Queue<string>();
                quSendMsgs = new Queue<string>();
                isNew = true;
            }
            else
                isNew = false;

            ReceiveThread = new Thread(new ThreadStart(Receive));
            ReceiveThread.Start();

            SendThread = new Thread(new ThreadStart(SendMessage));
            SendThread.Start();

            return true;
        }

        public static void Receive() // 서버로 부터 값 받아오기
        {
            //AddTextDelegate AddText = new AddTextDelegate(textBox1.AppendText);
            while (true)
            {
                Thread.Sleep(1);
                if (Connected)
                {
                    if (stream.CanRead)
                    {
                        try
                        {
                            string tempStr = Reader.ReadLine();
                            if (tempStr != null && tempStr.Length > 0)
                            {
                                if (!isNew)
                                {
                                    strRcvDataFromServer = tempStr;
                                    bRcvDataFromServer = true;
                                }
                                else
                                {
                                    quResvMsgs.Enqueue(tempStr);
                                }
                            }
                        }
                        catch
                        {
                            Connected = Client.Connected;
                        }
                    }
                    Connected = Client.Connected;
                }
            }
        }

        private static void SendMessage() // 클라이언트에게 받기
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1);
                    if (Connected)
                    {

                        if (quSendMsgs.Count > 0)
                        {
                            SendMsgQueue();
                        }
                    }
                }
            }
            catch
            {
                Connected = Client.Connected;
            }
        }

        public static void Disconnect()
        {
            Connected = false;
            if (Reader != null) Reader.Close();
            if (Writer != null) Writer.Close();
            if (Client != null) Client.Close();
            if (ReceiveThread != null) ReceiveThread.Abort();
            // 2020.10.20 JSKim St
            if (SendThread != null) SendThread.Abort();
            // 2020.10.20 JSKim Ed
        }
        public static void SendMsg(string msg)
        {
            if (!isNew)
            {
                if (Client.Connected == false) return;
                //textBox1.AppendText("Me : " + textBox2.Text + "\r\n");
                Writer.WriteLine(msg); // 보내버리기
                Writer.Flush();
            }
            else
            {
                SendMsgNew(msg);
            }
        }

        public static void SendMsgNew(string msg)
        {
            if (quSendMsgs != null)
            {
                quSendMsgs.Enqueue(msg);
            }
        }

        public static void SendMsgQueue()
        {
            if (Client.Connected == false) return;
            //textBox1.AppendText("Me : " + textBox2.Text + "\r\n");
            Writer.WriteLine(quSendMsgs.Dequeue()); // 보내버리기
            Writer.Flush();
        }

        public static string GetStatusQueue()
        {
            return "quSendMsgs:" + quSendMsgs.Count + "/quResvMsgs:" + quResvMsgs.Count;
        }
        /*
        public void Method()
        {
            throw new System.NotImplementedException();
        }

        public int Property
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }
        */
    }
}
