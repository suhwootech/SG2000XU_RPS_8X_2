using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCPIPDLL_Client;
using System.IO;


namespace SG2000X
{
    public class CTcpIp : CStn<CTcpIp>
    {
        public bool bIsConnect;
        public bool bIsNewMsg;
        public string sReciveMsg;
        public bool bResultGood = false;
        public bool bResultFail = false;
        public bool bResultDu = false;
        public bool bEmit_Ok = false;
        public bool bEmit_Fail = false;
        public bool bReady_Ok = false;
        public bool bByPass_Ok = false;
        public bool bTest_Ok = false;
        public bool bDuringEmit = false;
        public bool bSuccess_Ok = false;
        public bool bSuccess_Fail = false;
        public bool bSuccess_Du = false;
        public bool bQCError = false;
        public bool bError_Ok = false;
        public bool bPause_Ok = false;
        public bool bResume_Ok = false;
        public bool bStatus_Ok = false;
        public bool bStatus_Fail = false;
        public bool bStatus_RecvFlag = false;               // Status 조회 요청에 대한 응답을 받았다.
        public bool bJob_Ok = false;  //200312 ksg :
        public bool bTResult_Ok = false;  //200316 ksg :
        public bool bQcRunStaus = false;
        // 2020.09.28 JSKim St
        public bool bRecivePermission = false;
        public bool bPermission = false;
        // 2020.10.20 JSKim St
        //private object lockObject = new object();
        private object SendlockObject = new object();
        private object RecivelockObject = new object();
        // 2020.10.20 JSKim Ed
        // 2020.09.28 JSKim Ed
        public string[] sStringInfo = new string[5]; //190731 ksg :
        //public string[] sStringInfo = new string[6]; //200606 lks

        //20200512 lks
        private bool bConnectAct = false;

        static Queue<string> qRsvMsgs;// = new Queue<string>();

        private CTcpIp()
        {
            qRsvMsgs = new Queue<string>();
            SaveLogOnly("TCPIP_Client Version:" + Tcpip_Client.GetVersion());
        }

        public void Initial()
        {            
            Tcpip_Client.IsConnect();
            //qRsvMsgs = new Queue<string>();
        }

        public void Release()
        {

        }

        public bool TcpIpConnect()
        {
            try
            {
                if (bConnectAct) return false;
                if (Tcpip_Client.Connected && bConnectAct)
                {
                    return true;
                }
                bConnectAct = true;

                //Tcpip_Client.Connect("10.0.0.1", 5500, true);
                //Tcpip_Client.Connect("192.168.0.31", 5500, true);
                //Tcpip_Client.Connect("127.0.0.1", 5500);
                int port = 0;
                if (!int.TryParse(CData.Opt.sQcServerPort, out port))
                    port = 5500;
                Tcpip_Client.Connect(CData.Opt.sQcServerIp, port, true);

                //if (Tcpip_Client.IsConnect() == true)
                if (Tcpip_Client.IsConnect() == true)
                {
                    bIsConnect = true;
                    SendMsg("@QC Connected;");
                }
                else bIsConnect = false;

                bConnectAct = false;
                return bIsConnect;
            }
            catch (Exception e)
            {
                bConnectAct = false;
                SaveLog(true, "TcpIpConnect():" + e.Message);
                return false;
            }
        }

        public bool IsConnect()
        {
            try
            {
                bool IsConnect;
                IsConnect = Tcpip_Client.IsConnect();
                IsConnect = (Tcpip_Client.IsConnect() && Tcpip_Client.Connected );
                return bIsConnect;
            }
            catch (Exception e)
            {
                SaveLog(true, "IsConnect():" + e.Message);
                return false;
            }
        }

        public void TcpIpClose()
        {
            try
            {
                Tcpip_Client.Disconnect();
                bIsConnect = false;//koo : modify
            }
            catch (Exception e)
            {
                SaveLog(true, "TcpIpClose():" + e.Message);
            }
        }

        public void SendMsg(string Msg)
        {
            try
            {
                // 2020.09.28 JSKim St
                // 2020.10.20 JSKim St
                //lock (lockObject)
                lock (SendlockObject)
                // 2020.10.20 JSKim Ed
                {
                // 2020.09.28 JSKim Ed
                    if (!CData.Opt.bQcUse) return; //190802 ksg : 추가
                    if (!IsConnect()) return;
                    sReciveMsg = "";
                    Tcpip_Client.SendMsg(Msg);
                    SaveLog(true, Msg);
                // 2020.09.28 JSKim St
                }
                // 2020.09.28 JSKim Ed
            }
            catch (Exception e)
            {
                SaveLog(true, "ERR.SendMsg(" + Msg + "):" + e.Message);
            }
        }

        public void ReciveNewMsg()
        {
            try
            {
                // 2020.09.28 JSKim St
                // 2020.10.20 JSKim St
                //lock (lockObject)
                lock (RecivelockObject)
                // 2020.10.20 JSKim Ed
                {
                // 2020.09.28 JSKim Ed
                    bIsNewMsg = Tcpip_Client.GetIsRcvDataFromServer();

                    if (bIsNewMsg)
                    {
                        sReciveMsg = Tcpip_Client.GetRcvDataFromServer();
                        if (qRsvMsgs != null)
                        {
                            qRsvMsgs.Enqueue(sReciveMsg);
                            //SaveLog(false, "Enqueue(" + qRsvMsgs.Count + "):" + sReciveMsg);

                        }
                        else
                        {
                            //SaveLog(false, "Else (" + qRsvMsgs.Count + "):" + sReciveMsg);
                            ReciveMsg(sReciveMsg);
                        }
                    }
                // 2020.09.28 JSKim St
                }
                // 2020.09.28 JSKim Ed
            }
            catch (Exception e)
            {
                SaveLog(false, "ReciveNewMsg()-Err:" + e.Message);
            }
        }

        public void ProcessQueueMsg()
        {
            try
            {
                if (qRsvMsgs.Count > 0)
                {
                    int queCount = qRsvMsgs.Count;
                    for (int i = 0; i < queCount; i++)
                    {
                        string deqMsg = qRsvMsgs.Dequeue();
                        //SaveLog(false, "Dequeue(" + qRsvMsgs.Count + "):" + deqMsg);
                        if(!string.IsNullOrEmpty(deqMsg))
                            ReciveMsg(deqMsg);
                        //ReciveMsg(sReciveMsg);
                    }
                }
            }
            catch (Exception e)
            {
                SaveLog(false, "ProcessQueueMsg()-Err:"+e.Message);
            }
        }

        public void GetStatusDll()
        {
            System.Windows.Forms.MessageBox.Show(Tcpip_Client.GetStatusQueue());
        }

        public string ReciveMsg(string Msg)
        {
            string sReciveMsg = Msg;
            try
            {
                //sReciveMsg = Tcpip_Client.GetRcvDataFromServer();
                SaveLog(false, sReciveMsg);
                sReciveMsg = GetCmd(sReciveMsg);

                if (sReciveMsg == "Error" && !bQCError) { sReciveMsg = ""; bQCError = true; SaveLog(false, sReciveMsg); }//CSq_OfL.It.m_GetQcWorkEnd = true; }
                else if (sReciveMsg == "WorkEnd_Ok")    { sReciveMsg = ""; CSq_OfL.It.m_GetQcWorkEnd = true; }
                else if (sReciveMsg == "CloseServer")
                {
                    // 2020.10.15 JSKim St
                    TcpIpClose();
                    // 2020.10.15 JSKim Ed
                    sReciveMsg = "";
                    bIsConnect = false;
                }
                else if ((sReciveMsg == "Result_Good" || sReciveMsg == "Ergebnis_Good") && !bResultGood && !bResultFail)
                {
                    //SendResult();
                    sReciveMsg = "";
                    bResultGood = true;
                    bResultFail = false;
                    bResultDu = false;
                }
                else if ((sReciveMsg == "Result_Fail" || sReciveMsg == "Ergebnis_Fail") && !bResultGood && !bResultFail)
                {
                    //SendResult();
                    sReciveMsg = "";
                    bResultGood = false;
                    bResultFail = true;
                    bResultDu = false;
                }
                else if (sReciveMsg == "Result_Du" && !bResultGood && !bResultFail)
                {
                    sReciveMsg = "";
                    bResultGood = false;
                    bResultFail = false;
                    bResultDu = true;
                }
                else if (sReciveMsg == "Emit_Ok" && !bEmit_Ok)
                {
                    sReciveMsg = "";
                    bEmit_Ok = true;
                    bEmit_Fail = false;
                }
                else if (sReciveMsg == "Emit_Fail" && !bEmit_Fail)
                {
                    if (bDuringEmit)
                    {
                        sReciveMsg = "";
                        bEmit_Ok = false;
                        bEmit_Fail = true;
                    }
                    else
                    {
                        sReciveMsg = "";
                    }
                }
                // 2020.09.28 JSKim St
                else if (sReciveMsg == "Permission_Ok")
                {
                    sReciveMsg = "";
                    bPermission = true;
                    bRecivePermission = true;
                }
                else if (sReciveMsg == "Permission_Ng")
                {
                    sReciveMsg = "";
                    bPermission = false;
                    bRecivePermission = true;
                }
                // 2020.09.28 JSKim Ed
                else if (sReciveMsg == "Ready_Ok" && !bReady_Ok) { sReciveMsg = ""; bReady_Ok = true; }
                else if (sReciveMsg == "ByPass_Ok" && !bByPass_Ok) { sReciveMsg = ""; bByPass_Ok = true; }
                else if (sReciveMsg == "Test_Ok" && !bTest_Ok) { sReciveMsg = ""; bTest_Ok = true; }
                else if (sReciveMsg == "EmitSuc_Ok" && !bSuccess_Ok) { sReciveMsg = ""; bSuccess_Ok = true; bSuccess_Fail = false; bSuccess_Du = false; }
                else if (sReciveMsg == "EmitSuc_Fail" && !bSuccess_Fail) { sReciveMsg = ""; bSuccess_Ok = false; bSuccess_Fail = true; bSuccess_Du = false; }
                else if (sReciveMsg == "EmitSuc_Du" && !bSuccess_Du) { sReciveMsg = ""; bSuccess_Ok = false; bSuccess_Fail = false; bSuccess_Du = true; }
                else if (sReciveMsg == "Error_Ok" && !bError_Ok) { sReciveMsg = ""; bError_Ok = true; }
                else if (sReciveMsg == "Pause_Ok" && !bPause_Ok) { sReciveMsg = ""; bPause_Ok = true; bResume_Ok = false; }
                else if (sReciveMsg == "Resume_Ok" && !bResume_Ok) { sReciveMsg = ""; bPause_Ok = false; bResume_Ok = true; }
                //else if (sReciveMsg == "Status_Ok" && !bStatus_Ok) { sReciveMsg = ""; bStatus_Ok = true; bStatus_Fail = false; bStatus_RecvFlag = true; }
                //else if (sReciveMsg == "Status_Fail" && !bStatus_Fail) { sReciveMsg = ""; bStatus_Ok = false; bStatus_Fail = true; bStatus_RecvFlag = true; }
                else if (sReciveMsg == "Status_Ok" )  { sReciveMsg = ""; bStatus_Ok = true; bStatus_Fail = false; bStatus_RecvFlag = true; }
                else if (sReciveMsg == "Status_Fail") { sReciveMsg = ""; bStatus_Ok = false; bStatus_Fail = true; bStatus_RecvFlag = true; }
                else if (sReciveMsg == "JobChange_Ok" && !bJob_Ok) { sReciveMsg = ""; bJob_Ok = true; } //200312 ksg :
                else if (sReciveMsg == "TestResult" && !bTResult_Ok) { sReciveMsg = ""; bTResult_Ok = true; } //200316 ksg :
                else if (sReciveMsg == "AutoRun_Ok" && !bQcRunStaus) { sReciveMsg = ""; bQcRunStaus = true; } //200606 lks :
                else if (sReciveMsg == "AutoRun_Fail" && bQcRunStaus) { sReciveMsg = ""; bQcRunStaus = false; } //200606 lks :
                // 2020.10.20 JSKim St
                else if (sReciveMsg == "Reset_Ok") { sReciveMsg = ""; }
                else if (sReciveMsg == "DoorLock_On_Ok") { sReciveMsg = ""; }
                else if (sReciveMsg == "DoorLock_On_fail") { sReciveMsg = ""; bQcRunStaus = false; }
                else if (sReciveMsg == "DoorLock_Off_Ok") { sReciveMsg = ""; }
                else { sReciveMsg = ""; }
                // 2020.10.20 JSKim Ed

                SaveLog(false, "Return:"+sReciveMsg);
                return sReciveMsg;
            }
            catch (Exception e)
            {
                SaveLog(false, "Return:" + e.Message + "::"+sReciveMsg);
                return "Error:" + e.Message;
            }
        }

        //Cmd - Request Change Device(Not Use) 
        /*
        public void SendRequestChangeDevice()
        {
            string Msg = "";

            Msg = "@ReadyJobChange;";
            SendMsg(Msg);
            //SaveLog(true, Msg);
        }
        */
        //1. Cmd : EQ -> Qc Change Device 
        public void SendChageDevice(string Group, string Device)
        {
            string Msg = "";
            string Opt = "";
            if (CData.Opt.bQcByPass) Opt = "Voff";
            else Opt = "Von";
            Msg = "@JB:" + Group + "," + Device + "," + Opt.ToString() + ";";
            SendMsg(Msg);
        }

        //2. Cmd : EQ -> Qc AutoRun
        public void SendAutoRun()
        {
            string Msg = "";

            Msg = "@AutoRun;";

            bQcRunStaus = true;

            SendMsg(Msg);
        }
        //3~4.2까지 Dry <-> 부분
        //3. cmd : Dry -> Qc
        public void SendStripOut()
        {
            string Msg = "";

            Msg = "@Ready;";
            SendMsg(Msg);
        }

        //4-1. cmd : Dry -> Qc ByPass
        public void SendByPass()
        {
            string Msg = "";
            //190731 ksg :추가
            //-------------------------------------------
            //int iSlotNum = CSq_OfL.It.GetBtmEmptySlot();
            string LotName = CData.Parts[(int)EPart.DRY].sLotName;
            string MgzNum = CData.Parts[(int)EPart.DRY].iMGZ_No.ToString();
            //string SlotNum = iSlotNum.ToString();
            string SlotNum = CData.Parts[(int)EPart.DRY].iSlot_No.ToString(); //200606 lks
            string GroupName = CDev.It.m_sGrp;
            string DeviceName = CData.Dev.sName;
            string OcrInfo = CData.Parts[(int)EPart.DRY].sBcr;
            if (string.IsNullOrEmpty(OcrInfo)) OcrInfo = "-";
            //-------------------------------------------
            //Msg = "@ByPass;";
            Msg = "@ByPass,LotName:" + LotName + ",NoMgz:" + MgzNum + ",NoSlot:" + SlotNum + ",GN:" + GroupName + ",DN:" + DeviceName + ",OcrInfo:" + OcrInfo + ";";
            SendMsg(Msg);
        }

        //4-2. cmd : Dry -> Qc Test
        public void SendTest()
        {
            string Msg = "";
            //190731 ksg :추가
            //-------------------------------------------
            //int iSlotNum = CSq_OfL.It.GetBtmEmptySlot();
            string LotName = CData.Parts[(int)EPart.DRY].sLotName;
            string MgzNum = CData.Parts[(int)EPart.DRY].iMGZ_No.ToString();
            //string SlotNum = iSlotNum.ToString();
            string SlotNum = CData.Parts[(int)EPart.DRY].iSlot_No.ToString(); //200606 lks
            string GroupName = CDev.It.m_sGrp;
            string DeviceName = CData.Dev.sName;
            string OcrInfo = CData.Parts[(int)EPart.DRY].sBcr;
            if (string.IsNullOrEmpty(OcrInfo)) OcrInfo = "-";
            //-------------------------------------------

            //Msg = "@Test;";
            Msg = "@Test,LotName:" + LotName + ",NoMgz:" + MgzNum + ",NoSlot:" + SlotNum + ",GN:" + GroupName + ",DN:" + DeviceName + ",OcrInfo:" + OcrInfo + ";";
            SendMsg(Msg);
        }

        //5. cmd : OffLoader -> QC
        public void SendResult()
        {
            string Msg = "";

            Msg = "@Result;";
            SendMsg(Msg);
            bDuringEmit = true;
        }

        //6. cmd : Off Loader -> Qc 
        public void SendEmit()
        {
            string Msg = "";

            Msg = "@Emit;";
            SendMsg(Msg);
            bDuringEmit = true;
        }

        //7. cmd : Off Loader -> Qc 
        public void SendSucces()
        {
            string Msg = "";

            Msg = "@Success;";
            SendMsg(Msg);
            bDuringEmit = true;
        }

        //8.cmd : Qc -> Off Loader
        public void SendWorkEnd()
        {
            string Msg = "";

            Msg = "@WorkEnd;";
            SendMsg(Msg);
        }
        //--------------------------------- ksg : 새로 추가
        //9.cmd : Main -> QC
        public void SendPause()
        {
            string Msg = "";

            Msg = "@Pause;";
            SendMsg(Msg);
        }

        //10.cmd : Main -> QC
        public void SendResume()
        {
            string Msg = "";

            Msg = "@Resume;";
            SendMsg(Msg);
        }

        //11.cmd : Main -> QC
        public void SendStatus()
        {
            string Msg = "";
            bStatus_RecvFlag = false;       // Status 조회에 대한 응답 수신 여부 clear

            Msg = "@Status;";
            SendMsg(Msg);
        }

        //12.cmd : Main -> QC
        public void SendError()
        {
            string Msg = "";

            Msg = "@Error;";
            SendMsg(Msg);
        }

        //13.cmd : Main -> QC
        public void SendReset()
        {
            string Msg = "";

            Msg = "@Reset;";
            SendMsg(Msg);
        }

        //200317 ksg :
        //14.cmd : OFL -> QC
        public void SendTestResult()
        {
            string Msg = "";

            Msg = "@TestResult;";
            SendMsg(Msg);
        }

        //15.cmd : Main -> QC
        public void SendLotEnd()
        {
            string Msg = "";

            Msg = "@LotEnd;";
            SendMsg(Msg);
        }

        // 2020.10.20 JSKim St
        public void SendDoorLock(bool Lock)
        {
            string Msg = "";

            if (Lock == false)
            {
                Msg = "@DoorLock_Off;";
            }
            else
            {
                Msg = "@DoorLock_On;";
            }
            SendMsg(Msg);
        }
        // 2020.10.20 JSKim Ed

        // 2020.09.28 JSKim St
        public void SendPermission()
        {
            string Msg = "";

            if (CData.Opt.bQcUse)
            {
                bPermission = false;
                bRecivePermission = false;
            }
            else
            {
                bPermission = true;
                bRecivePermission = true;
            }

            Msg = "@Permission;";
            SendMsg(Msg);
        }
        // 2020.09.28 JSKim Ed

        private string GetCmd(string text)
        {
            try
            {
                //ksg : first char seach add
                string Temp;
                if (text == "") return Temp = "";
                string sFirst = text.Substring(0, 1);
                if (text == "" || text == null || sFirst != "@")
                {
                    return "";
                }
                //190731 ksg : 추가
                //char  [] sep    = { '@', ';' };
                char[] sep = { '@', ',', ':', ';' };
                string[] result = text.Split(sep);
                //--------------------------------------
                if (result[1] == "EmitSuc_Ok")// && sStringInfo[5].Equals("SAV"))
                {
                    sStringInfo[0] = result[3]; //Lot name
                    sStringInfo[1] = result[5]; //Mgz Num
                    sStringInfo[2] = result[7]; //Slot Num
                    sStringInfo[3] = result[9]; //Group 
                    sStringInfo[4] = result[11]; //Device
                    //sStringInfo[5] = "RSV";
                }
                //--------------------------------------
                //200317 ksg :
                if (result[1] == "TestResult")
                {
                    SaveQcResult(result);
                }

                return result[1];
            }
            catch (Exception e)
            {
                SaveLogOnly("ERROR-"+e.Message+"::" + text);
                return "Error(GetCmd())";
            }
        }

        //200317 ksg :
        private void SaveQcResult(string[] Msg)
        {
            try
            {

                DirectoryInfo di;
                string AddMsg = "";
                string sPath = GV.PATH_SPC + "QC\\";
                int Row = Convert.ToInt32(Msg[3]);
                int Col = Convert.ToInt32(Msg[5]);

                //20200504 lks
                sPath += DateTime.Now.ToString("yyyyMMdd");// + "\\";
                string fileName = sStringInfo[0] + ".csv";

                di = new DirectoryInfo(sPath);

                if (di.Exists == false)
                {
                    di.Create();
                }

                sPath += "\\" + fileName;

                AddMsg += "Lot Name," + sStringInfo[0] + ",\n";
                AddMsg += "Mgz Num," + sStringInfo[1] + ",\n";
                AddMsg += "Slot Num," + sStringInfo[2] + ",\n";
                //AddMsg += sStringInfo[5] + ",\n";
                for (int i = 7; i < (Row * Col) + 7; i++)
                {

                    if (Msg[i] != "") AddMsg += Msg[i];
                    else AddMsg += ".";
                    AddMsg += ",";

                    if (((i - 6) > 0) && ((i - 6) % Col == 0))
                    {
                        AddMsg += "\n";
                    }

                }
                CLog.Check_File_Access(sPath, AddMsg, true);
            }
            catch (Exception err)
            {
                SaveLogOnly("ERROR-" + err.Message);
            }
        }

        private void SaveLog(bool Send, string Msg)
        {
            try
            {
                string sLine = "";
                string sDir, sPath, sDay, sHour;

                DateTime Now = DateTime.Now;

                sDir = GV.PATH_LOG + Now.Year.ToString("0000") + "\\" + Now.Month.ToString("00") + "\\" + Now.Day.ToString("00") + "\\QC\\";

                if (!Directory.Exists(sDir.ToString()))
                {
                    Directory.CreateDirectory(sDir.ToString());
                }

                sDay = DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss fff]");
                sHour = DateTime.Now.ToString("yyyyMMddHH");

                sLine = sDay + "\t";
                if (Send)
                {
                    sLine += "[Equip -> QC] : " + Msg;// + "\n";
                }
                else
                {
                    sLine += "[Qc -> Equip] : " + Msg;// + "\n";
                }

                sPath = sDir + sHour + ".log";
                /*
                FileInfo fI = new FileInfo(sPath);
                if(!fI.Exists)
                {
                    fI.Create().Close();
                    GU.Delay(50);
                }

                StreamWriter Log = new StreamWriter(sPath, true);
                Log.WriteLine(sLine);
                Log.Close();
                */
                //LogManager Log = new LogManager(sPath, null, null);
                CLogManager Log = new CLogManager(sDir, null, null);
                Log.WriteLine(sLine);
            }
            catch(Exception e)
            {

            }
        }

        public void SaveLogOnly(string Msg)
        {
            try
            {
                string sLine = "";
                string sDir, sPath, sDay, sHour;

                DateTime Now = DateTime.Now;

                sDir = GV.PATH_LOG + Now.Year.ToString("0000") + "\\" + Now.Month.ToString("00") + "\\" + Now.Day.ToString("00") + "\\QC\\";

                if (!Directory.Exists(sDir.ToString()))
                {
                    Directory.CreateDirectory(sDir.ToString());
                }

                sDay = DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss fff]");
                sHour = DateTime.Now.ToString("yyyyMMddHH");

                sLine = sDay + "\t";
                sLine += "[Equip Info] : " + Msg;// + "\n";

                sPath = sDir + sHour + ".log";
                
                CLogManager Log = new CLogManager(sDir, null, null);
                Log.WriteLine(sLine);
            }
            catch (Exception e)
            {

            }
        }
    }
}
