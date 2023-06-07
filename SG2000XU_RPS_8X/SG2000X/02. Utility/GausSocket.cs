//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// C# Gaus Library
// 
// Gaus.Comm : TCP/IP Socket 통신 및 RS232/485 Serial 통신 class
//
// 2020-02-25, jhLee
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
// Gaus Library
using SG2000X.Gaus.Log;                     // for CGxLog

namespace SG2000X
{
    // Gaus TCP/IP Socket class
    // 비동기방식의 socket class로 Server와 Client를 동시에 지원한다.
    // 문자열 및 바이트 배열을 전송하고, 문자열은 Unicode와 ASCII를 지원한다.
    // OnConnect, OnDisconnect, OnReceive Callback 함수를 지원한다.
    // CGxLog를 통해 화면과 file로 전송데이터 및 이벤트/예외 내용을 저장 가능하다.

    public partial class CGxSocket : CStn<CGxSocket>
    // Server socket class
    { 
        public List<byte[]> listRcvData = new List<byte[]>();        // Byte 배열을 가지는 list queue

        public Socket socketServer = null;
        public Socket socketClient = null;                  // client Socket class

        public const int BufferSize = 4096;                    // Size of receive buffer.  
        public byte[] byBuffer = new byte[BufferSize];      // Receive buffer. 
        // public byte[] byPopData = null;                     // 수신 list에서 사용을 위해 빼어낸 데이터의 임시 보관용
        // public StringBuilder sb = new StringBuilder();      // Received data string. 

        // 송수신 데이터 길이
        public int nSendLength = 0;                         // 송신 완료 길이
        public int nRecvLength = 0;                         // 수신 완료 길이

        private CGxLog log = new CGxLog();                 // Gaus Log Class


        public string sMyName;
        public string sIPAddress = "";                      // 접속 할 IP 주소
        public int nPortNo;                          // 접속 할 Port 주소

        //d  IPAddress ipAddress;
        //d IPEndPoint localEPoint;
        //d public int nConnectDelay = 0;


        public IAsyncResult arConnect;                      // BeginConnect의 결과를 담아둔다.
        public int nAbnormalCount = 0;                      // 비정상 상태 반복횟수
        private bool bAutoConnectFlag = false;              // 자동 접속 동작을 활성화할 것인가 ?

        System.Timers.Timer tmrTryConnect = new System.Timers.Timer();  // 접속용 Timer

        // 비동기 처리시 각종 수행 완료 event
        public ManualResetEvent evtConnectDone;             // Connect 완료 event
        public ManualResetEvent evtSendDone;                // Send 완료 event
        public ManualResetEvent evtReceiveDone;             // Receive 완료 event
        public ManualResetEvent evtReceiveFlag;             // Receive 완료 event, 수신 데이터 처리를 위한 Event


        // 각종 Event callback 함수
        private AsyncCallback fnAcceptHandle = null;        // Accept event
        private AsyncCallback fnSendHandle = null;          // Send event
        private AsyncCallback fnReceiveHandle = null;       // Receive event
        private AsyncCallback fnConnectHandle = null;       // Connect event  


        // 최종 사용단에서 각 이벤트 발생시 처리 할 내용 지정
        private AsyncCallback fnCallbackConnect = null;     // 연결이 이루어질 경우 발생되는 callback 함수
        private AsyncCallback fnCallbackDisconnect = null;  // 연결이 끊어졌을 경우 발생되는 callback 함수
        private AsyncCallback fnCallbackReceive = null;     // 데이터가 수신되었을때 수행되는 callback 함수

        // Property

        public CGxLog Log { get { return log; } }           // Log 객체를 조회한다.
        public string sName { get; set; } = "NONE";         // 객체 이름 지정
        public int nID { get; set; } = 0;                   // 구분 ID 지정

        public bool IsUnicode { get; set; } = false;        // 전송시 Unicode 문자열을 이용하는가 ? true:Unicode, false:ASCII

        // Log 기록관련
        public bool IsStringData { get; set; } = true;      // 전송데이터가 문자열인가 바이너리인가 ?
        public bool IsStringUnicode { get; set; } = false;  // 문자열을 Unicode로 송/수신 할것인가 ?
        public bool IsLogData { get; set; } = true;         // Send / Receive 동작시 문자열로 데이터를 log에 남길것인가 ?
        public bool IsLogLength { get; set; } = true;       // 송/수신 데이터 길이를 log 에 남길것인가 ?

        public bool IsLogException { get; set; } = true;    // 예외 발생을 log에 남길것인가 ?
        public bool IsLogEvent { get; set; } = true;        // Event 발생을 log에 남길것인가 ?
        public bool IsLogEventReceive { get; set; } = false;    // Receive event발생시 log에 남길것인가 ? (자주 발생되는 이벤트 로그기록)

        // 재접속 관련 설정
        public bool IsAutoConnectTry { get; set; } = false;     // 자동으로 재접속을 시도한다.
        public int ConnectTimeout { get; set; } = 1500;     // Client connect timeout
        public int ConnectDelay { get; set; } = 3000;       // 얼마의 시간 뒤에 다시 재접속을 시도할 것인가 ?


        // 추가 변수
        public bool m_bIsUseETX = true;                            // 종료문자를 사용할 것인가 ?
        public char m_charETX = '\n';                              // 전문 종료 문자 = CR
        public int m_nSaveLimit = 2048;                            // 종료 문자를 기다리는 최대 문자열 길이, 이 길이를 넘어서면 데이터 수신으로 간주
        public string m_sSavedMsg = string.Empty;                   // EQ 이전에 수신된 문자가 있는가 ?


        private CGxSocket() // 생성시 초기화
        {
            CQcVisionCom.initQCcomVar();
            //fnAcceptHandle = new AsyncCallback(OnAcceptCallback);
            fnSendHandle = new AsyncCallback(OnSendCallback);
            fnReceiveHandle = new AsyncCallback(OnReceiveCallback);
            fnConnectHandle = new AsyncCallback(OnConnectCallback);

            // 비동기 처리시 각종 수행 완료 event
            evtConnectDone = new ManualResetEvent(false);             // Connect 완료 event
            evtSendDone = new ManualResetEvent(false);                // Send 완료 event
            evtReceiveDone = new ManualResetEvent(false);             // Receive 완료 event
            evtReceiveFlag = new ManualResetEvent(false);             // Receive 완료 event, 수신 데이터 처리를 위한 Event

            // 접속 시도 Timer 기본 설정
            tmrTryConnect.Interval = 2000;                      // 이벤트 발생 주기
            tmrTryConnect.AutoReset = false;                    // 이벤트를 1번만 발생시킨다.
            tmrTryConnect.Elapsed += OnTryConnectEvent;         // 재접속 시도 이벤트 지정
            log.SetFileName(@"D:\Log_SG2X\QC", "QCComm");
            initiCGxSocket();
        }

        public bool initiCGxSocket()
        {
            IsStringData = true;                    // 문자열로 데이터를 처리한다.
            IsStringUnicode = false;                 // ASCII 
            IsAutoConnectTry = true;                 // 연결이 끊어질 경우 자동으로 Retry를 수행한다.

            nID = 1;                                 // 통신 ID는 1
            sName = "QCTCP";                         // Vision 과연 연결 TCP/IP 통신
            nPortNo = 5500;
            SetCallbackReceive(OnClientReceiveFunction);
            SetCallbackConnect(OnClientConnectFunction);
            SetCallbackDisconnect(OnClientDisconnectFunction);
            return true;
        }

        // 클라이언트 소켓에서 데이터를 수신 받은경우 처리
        private void OnClientReceiveFunction(IAsyncResult ar)
        {
            byte[] byData;
            string sMsg;

            while (GetByteData(out byData))
            {
                sMsg = Encoding.ASCII.GetString(byData, 0, byData.Length);

                //OLD ProcessMessage(sMsg);           // 수신된 데이터 처리

                // 2021-01-20, jhLee : 종료문자별로 데이터를 분리하여 처리할 수 있도록 기능 추가
                //
                if (sMsg.Length <= 0) continue;                         // 수신된 문자가 없다면 수행하지 않고 무시한다.

                // 종료 문자를 사용한다고 지정되었다면
                if (m_bIsUseETX)
                {
                    // 이전에 저장된 메세지가 있다면 
                    if (string.IsNullOrEmpty(m_sSavedMsg) == false)
                    {
                        sMsg = m_sSavedMsg + sMsg;                      // 보간되고있던 문자열을 덧붙인다.
                        m_sSavedMsg = string.Empty;
                    }

                    string[] sArrMsg = sMsg.Split(m_charETX);           // 지정된 종료문자로 분리를 한다.

                    if (sMsg[sMsg.Length - 1] == m_charETX)           // 마지막 문자가 종료 문자인가 ?
                    {
                        for (int i = 0; i < sArrMsg.Length; i++)        // 모든 수신 데이터에 대하여
                        {
                            if (string.IsNullOrEmpty(sArrMsg[i]) == false)  // 분리된 내용이 널이 아니라면 처리 가능
                            {
                                ProcessMessage(sArrMsg[i]);                 // 수신된 데이터 처리
                            }
                        }
                    }
                    else  // 마지막이 종료 문자가 아니라면 마지막에 수신된 데이터 블럭은 다음에 수신된 데이터에 덧붙여서 처리해야한다.
                    {
                        for (int i = 0; i < (sArrMsg.Length - 1); i++)  // 마지막에 수신된 데이터를 제외하고
                        {
                            ProcessMessage(sArrMsg[i]);                 // 수신된 데이터 처리
                        }

                        m_sSavedMsg = sArrMsg[sArrMsg.Length - 1];      // 마지막에 수신된 데이터 보관, 나중에 덧붙인다.

                        // 보관되고 있는 데이터가 일정 길이를 넘어서면 강제로 처리를 시키도록 한다.
                        if (m_sSavedMsg.Length >= m_nSaveLimit)
                        {
                            log.Write($"* ERROR limit over saved buffer size {m_sSavedMsg.Length}, Force process data : {m_sSavedMsg}");

                            ProcessMessage(m_sSavedMsg);
                            m_sSavedMsg = string.Empty;
                        }
                    }
                }//of if use ETX ?
                else // 종료문자를 사용하지않으면 받아들인 데이터 그대로 처리를 하도록 한다.
                {
                    ProcessMessage(sMsg.Trim());              // 종료문자 CR을 제거하고 처리한다.
                }

            }

        }

        // Clinet가 연결에 성공했을 때
        private void OnClientConnectFunction(IAsyncResult ar)
        {
            CGxSocket socket = (ar.AsyncState as CGxSocket);

            log.Write($"---- Client connect success ID:{socket.nID}, Name:{socket.sName}");
        }

        // Clinet가 연결이 끊어졌을 때
        private void OnClientDisconnectFunction(IAsyncResult ar)
        {
            CGxSocket socket = (ar.AsyncState as CGxSocket);

            log.Write($"**** Client Disconnect ID:{socket.nID}, Name:{socket.sName}");
        }

        protected void ProcessMessage(string sMsg)
        {
            int nMsgCnt = 0;
            if (sMsg.Length <= 0) return;               // 수신된 데이터가 없으면 처리 불가

            //log.Write($"   [{sMsg}]");                // 수신된 데이터


            string[] arrRecv = sMsg.Split(',', ':', ';', '_');         // 분리자로 각 항목을 분리한다.
            nMsgCnt = arrRecv.Length;
            if (nMsgCnt < 1)
            { return; }

            string sCmd = arrRecv[0].Trim();            // 명령문을 토대로 데이터를 처리한다.
            string sMode = "";                          // 명령문에 따라서 다양한 선택 데이터가 존재한다.
            string sValue = "";                          
            if (nMsgCnt >= 2)
            {
                sMode = arrRecv[1];
            }
            

            //
            // SG2000X DryZone ==> QC-Vision으로 strip을 반송할 때 사용
            //
            // EQ -> QC : 회신, SG2000X이 내가 보낸 EQSendRequest 요청에 대한 회신을 보냈다.
            
            // #1 SendRequst Msg EQ [EQSendRequest.ACK] <- QC [EQSendRequest]
            if (String.Equals(sCmd, "EQSendRequest", StringComparison.OrdinalIgnoreCase))
            {
                log.Write($"<- [EQSendRequest,{sMode}] Reply");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ<<<<< [EQSendRequest.ACK]");

                CQcVisionCom.rcvEQSendRequst = true;
                SendMessage("EQSendRequest,ACK");//Rcv OK response. 

            }
            // 제품을 받을 준비가 되었는지에 대한 응답.
            //#2 EQReadyQery Msg  EQ [EQReadyQuery] -> QC [EQReadyQuery,NG][EQReadyQuery,OK]
            else if (String.Equals(sCmd, "EQReadyQuery", StringComparison.OrdinalIgnoreCase))
            {
                log.Write($"<- [EQReadyQuery][{sMsg}]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ<<<<<:"+ sMsg);
                if (sMode.StartsWith("NG"))
                {
                    //QC not ready 
                    CQcVisionCom.rcvEQReadyQueryOK = false;
                }
                else if (sMode.StartsWith("OK"))
                {
                    //QC ready 
                    CQcVisionCom.rcvEQReadyQueryOK = true;
                }
                else
                {
                    //Wrong Message
                }
            }
            else if (String.Equals(sCmd, "EQSendOut", StringComparison.OrdinalIgnoreCase))
            {
                if (sMode.StartsWith("ACK"))
                {
                    log.Write("<- [EQSendOut,Bypass]");
                    vmMan_13QcVision.m_qcVision.addMsgList("EQ<<<<< [EQSendOut,Bypass]");
                    CQcVisionCom.rcvEQSendOutACK = true;
                }
                else{
                    vmMan_13QcVision.m_qcVision.addMsgList(sMsg);
                }
            }
            else if (String.Equals(sCmd, "EQSendEnd", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [EQSendEnd]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ<<<<< [EQSendEnd]");
            }
            else if (String.Equals(sCmd, "EQAbortTransfer", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [EQAbortTransfer.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ<<<<< [EQAbortTransfer.ACK]");
                CQcVisionCom.rcvEQAbortTransferACK = true;
            }
            else if (String.Equals(sCmd, "QCSendRequest", StringComparison.OrdinalIgnoreCase))
            {
                //Off loader QCSendRequest -> <- QCSendRequest.ACK
                log.Write("<- [QCSendRequest.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ  <<<<<  [QCSendRequest.ACK]");
                //CSq_OfL.It.m_GetQcWorkEnd = true; //QC vision has Strip
                CQcVisionCom.rcvQCSendRequestACK = true;
            }
            else if (String.Equals(sCmd, "QCReadyQuery", StringComparison.OrdinalIgnoreCase))
            {
                if (sMode.StartsWith("GOOD"))
                {
                    // jhLee:양품처리 요청 및 준비 회신
                    CQcVisionCom.m_nReadyReqMgz = CQcVisionCom.eMGZ_GOOD;       // 양품 Magazine을 요청하였다. 

                    if (CQcVisionCom.m_nMgzReady == CQcVisionCom.eMGZ_GOOD)     // 양품 Magazine이 준비되었다면 
                    {
                        SendMessage("QCReadyQuery,OK");
                    }
                    else
                        SendMessage("QCReadyQuery,NG");                         // 아직 준비가 되지 않았다.


                    //// 양품 처리 준비가 되었다면 OK를 회신하고 아직 준비가 되지 않았다면 NG로 회신한다.
                    //if ((CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_BtmRecive) && (CSq_OfL.It.m_nMgzReady == 1) && (CSq_OfL.It.m_bDuringGOut == true))
                    //{
                    //    SendMessage("QCReadyQuery,OK");
                    //}
                    //else
                    //    SendMessage("QCReadyQuery,NG");

                    
                    CQcVisionCom.rcvQCReadyQueryGOOD = true;
                    CQcVisionCom.rcvQCReadyQueryNG = false;
                }
                else if (sMode.StartsWith("NG"))
                {
                    // jhLee:불량품 처리 준비가 되었다면 OK를 회신하고 아직 준비가 되지 않았다면 NG로 회신한다.
                    CQcVisionCom.m_nReadyReqMgz = CQcVisionCom.eMGZ_NG;       // 불량품 Magazine을 요청하였다. 

                    if (CQcVisionCom.m_nMgzReady == CQcVisionCom.eMGZ_NG)     // 불량품 Magazine이 준비되었다면 
                    {
                        SendMessage("QCReadyQuery,OK");
                    }
                    else
                        SendMessage("QCReadyQuery,NG");                         // 아직 준비가 되지 않았다.


                    //if ((CData.Parts[(int)EPart.OFL].iStat == ESeq.OFL_TopRecive) && (CSq_OfL.It.m_nMgzReady == 2) && (CSq_OfL.It.m_bDuringFOut == true))
                    //{
                    //    SendMessage("QCReadyQuery,OK");
                    //}
                    //else
                    //    SendMessage("QCReadyQuery,NG");

                    CQcVisionCom.rcvQCReadyQueryGOOD = false;
                    CQcVisionCom.rcvQCReadyQueryNG = true;
                }


                log.Write(sMsg.TrimEnd());
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<< " + sMsg);
            }
            else if (String.Equals(sCmd, "QCSendOut", StringComparison.OrdinalIgnoreCase))
            {
                CQcVisionCom.m_nQCSend = CQcVisionCom.eQCSend_Start;             //jhLee: QC에서 전송 시작


                if (sMode.StartsWith("GOOD"))
                {
                    CQcVisionCom.rcvQCSendOutGOOD = true;
                    CQcVisionCom.rcvQCSendOutNG = false;
                }
                else if (sMode.StartsWith("NG"))
                {
                    CQcVisionCom.rcvQCSendOutGOOD = false;
                    CQcVisionCom.rcvQCSendOutNG = true;
                }

                if (nMsgCnt >= 9)
                {
                    CQcVisionCom.sStringInfo[0] = arrRecv[2];//LotName
                    CQcVisionCom.sStringInfo[1] = arrRecv[3];//MgzNo
                    CQcVisionCom.sStringInfo[2] = arrRecv[4];//SlotNo
                    CQcVisionCom.sStringInfo[3] = arrRecv[5];//SlotName
                    CQcVisionCom.sStringInfo[4] = arrRecv[6];//GN
                    CQcVisionCom.sStringInfo[5] = arrRecv[7];//DN
                    CQcVisionCom.sStringInfo[6] = arrRecv[8];//StripName

                }
                log.Write(sMsg.TrimEnd());
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<< " +sMsg);

                SendMessage("QCSendOut,ACK");
            }

            else if (String.Equals(sCmd, "QCSendEnd", StringComparison.OrdinalIgnoreCase))
            {
                CQcVisionCom.m_nQCSend = CQcVisionCom.eQCSend_End;             //jhLee: QC에서 전송 완료

                CQcVisionCom.rcvQCSendEnd = true;   //QC 반송 완료
                SendMessage("QCSendEnd,ACK");
                log.Write(sMsg.TrimEnd());
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<< " + sMsg);
            }

            else if (String.Equals(sCmd, "QCAbortTransfer", StringComparison.OrdinalIgnoreCase))
            {
                CQcVisionCom.m_nQCSend = CQcVisionCom.eQCSend_Abort;             //jhLee: QC에서 전송 취소

                log.Write("<- [QCAbortTransfer]");
                vmMan_13QcVision.m_qcVision.addMsgList("<- [EQAbortTransfer.ACK]");
                SendMessage("QCAbortTransfer,ACK");

                CQcVisionCom.rcvQCAbortTransfer = true;

                //미사용 CSq_OfL.It.QcAbortTransferring(); //!
            }

            else if (String.Equals(sCmd, "AutoRun", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [AutoRun.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<<  [AutoRun.ACK]");
                if (sMode.StartsWith("ACK"))
                {
                    CQcVisionCom.rcvAutoRunACK = true;
                }
            }
            else if (String.Equals(sCmd, "AutoStop", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [AutoStop.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<<  [AutoStop.ACK]");
            }
            else if (String.Equals(sCmd, "ErrorReset", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("EQ <<<<< [ErrorReset.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("<- [ErrorReset.ACK]");
            }
            else if (String.Equals(sCmd, "LotEnd", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("EQ <<<<< [LotEnd.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("<- [LotEnd.ACK]");
            }
            else if (String.Equals(sCmd, "DoorLock", StringComparison.OrdinalIgnoreCase))
            {
                if (nMsgCnt >= 3)
                {
                    sValue = arrRecv[2];
                }
                else
                {
                    vmMan_13QcVision.m_qcVision.addMsgList("<- [DoorLock.ACK]");
                    return;
                }
                if (sMode.StartsWith("On"))
                {
                    if (sValue.StartsWith("OK"))
                    {
                        CQcVisionCom.rcvQCDoorLockOnOK = true;
                        CQcVisionCom.rcvQCDoorLockOnNG = false;
                    }
                    else if (sValue.StartsWith("NG"))
                    {
                        CQcVisionCom.rcvQCDoorLockOnOK = false;
                        CQcVisionCom.rcvQCDoorLockOnNG = true;
                    }
                    
                }
                else if (sMode.StartsWith("Off"))
                {
                    if (sValue.StartsWith("OK"))
                    {
                        CQcVisionCom.rcvQCDoorLockOffOK = true;
                        CQcVisionCom.rcvQCDoorLockOffNG = false;
                    }
                    else if (sValue.StartsWith("NG"))
                    {
                        CQcVisionCom.rcvQCDoorLockOffOK = false;
                        CQcVisionCom.rcvQCDoorLockOffNG = true;
                    }
                }
                else
                {
                    vmMan_13QcVision.m_qcVision.addMsgList(sMsg.Trim());
                    return;
                }
                log.Write("EQ <<<<< [DoorLock.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("<- [DoorLock.ACK]");
            }
            else if (String.Equals(sCmd, "JobChange", StringComparison.OrdinalIgnoreCase))
            {
                if (sMode.StartsWith("OK"))
                {
                    CQcVisionCom.rcvQCJobChangeOK = true;
                    CQcVisionCom.rcvQCJobChangeNG = false;
                }
                else if (sMode.StartsWith("NG"))
                {
                    CQcVisionCom.rcvQCJobChangeOK = false;
                    CQcVisionCom.rcvQCJobChangeNG = true;
                }
                log.Write(sMsg);
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<<" + sMsg.TrimEnd());
 
            }
            else if (String.Equals(sCmd, "EmgStop", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [EmgStop]");
                vmMan_13QcVision.m_qcVision.addMsgList("<<<< [EmgStop]");
 
            }
            else if (String.Equals(sCmd, "StatusQuery", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [StatusQuery.ACK]");
                vmMan_13QcVision.m_qcVision.addMsgList("<<<< [StatusQuery.ACK]");
    
            }
            else if (String.Equals(sCmd, "Status", StringComparison.OrdinalIgnoreCase))
            {
                log.Write(sMsg.Trim());
                
                vmMan_13QcVision.m_qcVision.addMsgList(sMsg.Trim());

                if (nMsgCnt >= 4)
                {
                    CQcVisionCom.nQCeqpStatus = Int32.Parse(arrRecv[1]);
                    //0: init , 1:initializing , 2:stop, 3: AutoRun , 4:manual run, 5: error
                    CQcVisionCom.nQCexistStrip = Int32.Parse(arrRecv[2]);
                    //0:not exist 1: exist
                    CQcVisionCom.nMgzMoving = Int32.Parse(arrRecv[3]);
                    //m_sMsg = string.Format("rcv Data State[{0}], exit strip[{1}], Mgz Moving[{2}]", CQcVisionCom.nQCeqpStatus, CQcVisionCom.nQCexistStrip, CQcVisionCom.nMgzMoving);
                    //mMan_13QcVision.m_qcVision.addMsgList("EQ <<<<<" + m_sMsg.TrimEnd());
                    //0:Mgz 쪽으로 Strip stop , 1:Mgz 쪽으로 Strip moving
                }
                log.Write(sMsg);
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<<<" + sMsg.TrimEnd());

            }
            else if (String.Equals(sCmd, "ResultQuery", StringComparison.OrdinalIgnoreCase))
            {
                CQcVisionCom.rcvResultQuery = true;
                log.Write("<<<< [ResultQuery]");
                vmMan_13QcVision.m_qcVision.addMsgList("EQ <<<< [ResultQuery]");
                int nRow, nCol;

                if(nMsgCnt > 3)
                {
                    nRow = Int32.Parse(arrRecv[3]);
                    nCol = Int32.Parse(arrRecv[5]);
                }                
            }
            /*
            else if (String.Equals(sCmd, "ResultQueryOK", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [ResultQueryOK]");
                vmMan_13QcVision.m_qcVision.addMsgList("<- [ResultQueryOK]");
                CQcVisionCom.rcvQCresultOK = true;
                CQcVisionCom.rcvQCresultNG = false;
                CQcVisionCom.rcvQCresultDU = false;

            }
            else if (String.Equals(sCmd, "ResultQueryNG", StringComparison.OrdinalIgnoreCase))
            {
                log.Write("<- [ResultQueryNG]");
                vmMan_13QcVision.m_qcVision.addMsgList("<- [ResultQueryNG]");
                CQcVisionCom.rcvQCresultOK = false;
                CQcVisionCom.rcvQCresultNG = true;
                CQcVisionCom.rcvQCresultDU = false;

            }
            */

        }

        public void SendMessage(string sMsg)
        {
            if (IsConnected())
            {
                vmMan_13QcVision.m_qcVision.addMsgList("EQ>>>>>>"+sMsg);
                bool bRet = SendString(sMsg + "\n");
                log.Write($"-> {sMsg.Trim()} : {bRet}");
            }
            else
                log.Write($"*> Send fail : {sMsg.Trim()}");

        }

        public void SendChageDevice(string Group, string Device)
        {
            string Msg = "";
            string Opt = "";
            if (CData.Opt.bQcByPass) Opt = "Voff";
            else Opt = "Von";
            Msg = "JobChange," + Group + "," + Device + "," + Opt.ToString();
            SendMessage(Msg);
        }

        // 선두의 데이터를 바이트 배열로 받아온다. (원본 삭제)
        // return 데이터를 정상적으로 받아왔는지 여부, true:정상적으로 데이터를 취득하였다. false:데이터가 존재하지 않는다.
        public bool GetByteData(out byte[] OutData)
        {
            if (listRcvData.Count > 0)              // 수신 데이터가 존재하는가 ?
            {
                try
                {
                    OutData = new byte[listRcvData[0].Length];
                    Array.Copy(listRcvData[0], 0, OutData, 0, listRcvData[0].Length);       // 배열을 복가한다.
                    listRcvData.RemoveAt(0);                                                // 선두 데이터를 삭제한다.

                    return true;
                }
                catch (Exception exp)
                {
                    if (IsLogException) log.Write($"** GetByteData() Exception : {exp.Message}");
                }
            }

            OutData = null;
            return false;                           // 데이터가 존재하지 않는다.
        }


        // 선두의 데이터를 문자열로 받아온다. (원본 삭제)
        // return 데이터를 정상적으로 받아왔는지 여부, true:정상적으로 데이터를 취득하였다. false:데이터가 존재하지 않는다.
        public bool GetStringData(out string OutMsg)
        {
            if (listRcvData.Count > 0)              // 수신 데이터가 존재하는가 ?
            {
                try
                {
                    if (IsStringUnicode)
                    {
                        OutMsg = Encoding.Unicode.GetString(listRcvData[0], 0, listRcvData[0].Length);  // Unicode
                    }
                    else
                        OutMsg = Encoding.ASCII.GetString(listRcvData[0], 0, listRcvData[0].Length);    // ASCII

                    listRcvData.RemoveAt(0);                                                // 선두 데이터를 삭제한다.

                    return true;
                }
                catch (Exception exp)
                {
                    if (IsLogException) log.Write($"** GetStringData() Exception : {exp.Message}");
                }
            }

            OutMsg = "";
            return false;                           // 데이터가 존재하지 않는다.
        }


        // Event callback 함수 대입
        public void SetCallbackFunction(AsyncCallback fnConnect, AsyncCallback fnDisconnect, AsyncCallback fnReceive)
        {
            fnCallbackConnect = fnConnect;          // 연결이 이루어질 경우 발생되는 callback 함수
            fnCallbackDisconnect = fnDisconnect;    // 연결이 끊어졌을 경우 발생되는 callback 함수
            fnCallbackReceive = fnReceive;          // 데이터가 수신되었을때 수행되는 callback 함수
        }

        // 개별적인 callback 함수 대입
        // 연결이 된 경우 호출
        public void SetCallbackConnect(AsyncCallback fn)
        {
            fnCallbackConnect = fn;         // 연결이 이루어질 경우 발생되는 callback 함수
        }

        // 연결이 끊어진 경우 호출
        public void SetCallbackDisconnect(AsyncCallback fn)
        {
            fnCallbackDisconnect = fn;     // 연결이 끊어졌을 경우 발생되는 callback 함수
        }

        // 데이터가 수신된 경우 호출
        public void SetCallbackReceive(AsyncCallback fn)
        {
            fnCallbackReceive = fn;      // 데이터가 수신되었을때 수행되는 callback 함수
        }


        // Client socket이 재접속을 하도록 만들어주는 Timer event 생성
        private void OnTryConnectEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            // Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);

            // Client socket이 초기화 되었다면 새로이 생성하고 연결을 시도한다.
            if (socketClient == null)
            {
                // 자동 연결에 대한 동작이 중지되었다면 아무런 수행도 하지 않는다.
                // (사용자가 명시적으로 ClientClose를 수행한 경우)
                if (bAutoConnectFlag == false) return;

                // 연결을 시도하고 연결 지령을 성공적으로 수행하지 못했다면
                if (Connect() == false)
                {
                    try
                    {
                        socketClient.Close();           // socket  객체를 닫고 초기화 해준다.
                    }
                    catch (Exception exp)
                    {
                        if (IsLogException) log.Write($"** OnTryConnectEvent exception 1 : {exp.Message}");

                    }

                    socketClient = null;

                    tmrTryConnect.Interval = ConnectDelay;      // 다시 연결을 지령하기위한 지연 시간
                }
                else // 연결 요청 성공
                    tmrTryConnect.Interval = ConnectTimeout;    // 연결대기 시간초과

                tmrTryConnect.Start();          // 다시 타이머를 동작시킨다.
            }
            else      // socket 객체가 생성되어있다면, 이전 단계에서 connect를 시도하였다.
            {
                // 아직까지 연결이 이루어지지 않았다면 Timeout으로 간주한다.
                if (IsConnected() == false)
                {
                    try
                    {
                        socketClient.Close();                       // 연결을 중지시킨다.
                    }
                    catch (Exception exp)
                    {
                        if (IsLogException) log.Write($"** OnTryConnectEvent exception 2 : {exp.Message}");

                    }
                }

                // Close 이후로는 Receive event가 발생하면서 Read size가 0이 되어 disconnect 처리하게 되어있다.
                // 그리고 이 Timer는 1회성 timer이므로 명시적으로 start 시켜주지 않으면 반복되지 않게된다.
            }
        }


        // 수신대기를 시작한다.
        public bool SetListen()                              // Server 수신대기를 시작한다.
        {
            return SetListen(nPortNo);                      // 내정된 Port로 listen을 수행한디ㅏ.
        }

        // 지정 Port로 수신대기를 시작한다.
        public bool SetListen(int nPort)                    // Server 수신대기를 시작한다.
        {
            nPortNo = nPort;
            IPEndPoint localEPoint = new IPEndPoint(IPAddress.Any, nPort);         // 포트 지정

            try
            {
                socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);    // server Socket class
                socketServer.Bind(localEPoint);                           // 지정 포트로 바인딩 한다.
                socketServer.Listen(10);
                socketServer.BeginAccept(fnAcceptHandle, this); //  socketServer);   // Accept call back 함수를 지정한다.

                if (IsLogEvent) log.Write($"!! SetListen({nPort}) ok");
                return true;
            }
            catch (SocketException exp)
            {
                // Socket에 액세스하려고 시도하는 동안 오류가 발생했습니다.
                if (IsLogException) log.Write($"** SetListen() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** SetListen exception : {exp.Message}");

            }

            return false;
        }

        public void rcvMsgFromSvr()
        {
            //vmMan_13QcVision qcVision = new vmMan_13QcVision();
            //qcVision.addMsgList("say hi");
            
        }

        // Server의 연결 대기를 종료한다.
        public void StopListen()
        {
            try
            {
                if (socketClient != null)
                {
                    socketClient.Disconnect(false);  //  .BeginDisconnect(true, OnDisConnectCallback, socketClient);
                    //socketClient.Close();
                }

                socketServer.Close();
                //socketClient = null;
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** StopListen exception : {exp.Message}");

            }

        }

        // Client 연결을 닫는다.
        public void CloseClient()
        {
            try
            {
                bAutoConnectFlag = false;                       // 명시적으로 연결을 끊는경우에는 자동 재접속을 수행하지 않도록 한다.
                socketClient.Disconnect(false);                 // 전송중인 데이터를 모두 종료하고 연결을 끊는다.

                // 이후 Receive event가 발생하며 Reading 길이가 0인 경우가 발생하므로 이를 최종 Close 단계로 인식하면 된다.

                //d socketClient.Shutdown(SocketShutdown.Both);  // 즉시 연결을 끊는다. 
                //d socketClient.Close();
                //d socketClient = null;
                if (IsLogEvent) log.Write($"!! Close server connected Client ...");

                vmMan_13QcVision.m_qcVision.addMsgList("Close connection !!!");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** CloseClient exception : {exp.Message}");
            }
        }

        // 연결 요청이 발생하면 호출될 콜백 함수
        public void OnAcceptCallback(IAsyncResult ar)
        {
            // 접속을 허용시킨다.
            try
            {
                // Get the socket that handles the client request.  
                socketClient = socketServer.EndAccept(ar);                // Accept를 끝낸다.
                socketClient.BeginReceive(byBuffer, 0, byBuffer.Length, 0, fnReceiveHandle, this);   // 수신 Callback 함수를 대입한다.

                evtConnectDone.Set();               // 연결이 설정되었다.

                if (fnCallbackConnect != null)
                {
                    fnCallbackConnect(ar);          // 연결이 이루어질 경우 발생되는 callback 함수 호출
                }

                // 연결이 끊어 진 뒤 다시 재연결이 가능하도록 Accept를 재가동 시킨다.
                socketServer.BeginAccept(fnAcceptHandle, this); //  socketServer);   // Accept call back 함수를 지정한다.
                if (IsLogEvent) log.Write($"!! OnAcceptCallback() EndAccept : {socketClient.Connected}");           // 완료 표시
                return;
            }
            catch (ObjectDisposedException exp)
            {
                // Socket 개체가 닫힌 경우
                if (IsLogException) log.Write($"** OnAcceptCallback() ObjectDisposedException : {exp.Message}");
            }
            catch (ArgumentNullException exp)
            {
                // asyncResult가 비어 있는 경우
                if (IsLogException) log.Write($"** OnAcceptCallback() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentException exp)
            {
                // BeginAccept(AsyncCallback, Object)를 호출했지만 asyncResult가 만들어지지 않은 경우
                if (IsLogException) log.Write($"** OnAcceptCallback() ArgumentException : {exp.Message}");
            }
            catch (InvalidOperationException exp)
            {
                // EndAccept(IAsyncResult) 메서드가 이미 호출된 경우
                if (IsLogException) log.Write($"** OnAcceptCallback() InvalidOperationException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // Socket에 액세스하려고 시도하는 동안 오류가 발생했습니다.
                if (IsLogException) log.Write($"** OnAcceptCallback() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** OnAcceptCallback() Exception : {exp.Message}");
            }

            evtConnectDone.Reset();         // 연결이 해제되었다.

            //log.Write($"---- OnAcceptCallback() EndAccept ----");           // 완료 표시
        }


        // Client가 접속 할 주소를 지정한다.
        public void SetAddress(string sAddr, int nPort)
        {
            sIPAddress = sAddr;                      // 접속 할 IP 주소
            nPortNo = nPort;                         // 접속 할 Port 주소
        }


        // 기존에 설정된 주소로 접속을 시도한다.
        public bool Connect()
        {
            return Connect(sIPAddress, nPortNo);
        }


        // Client socket이 begin connect를 수행한다.
        public bool Connect(string sAddr, int nPort)
        {
            bAutoConnectFlag = true;

            if (socketClient == null)
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);    // server Socket class
                if (IsLogEvent) log.Write($"Create new stocket instance {sAddr}:{nPort}");
            }

            try
            {
                if (IsLogEvent) log.Write($"!! BeginConnect to {sAddr}:{nPort}");

                // socketClient.Connect(sIPAddress, nPortNo);
                arConnect = socketClient.BeginConnect(sAddr, nPort, fnConnectHandle, this); //  socketClient);


                //bool bRet = arConnect.AsyncWaitHandle.WaitOne(2000, true);
                //    log.Write("* Connect Timeout error");
                //    // throw new SocketException(10060);   // Connection time out  발생시킨다.
                //}
                vmMan_13QcVision.m_qcVision.addMsgList("Try Connection with QC vision !!!");
                return true;
            }
            catch (ArgumentNullException exp)
            {
                // host이(가) null인 경우
                if (IsLogException) log.Write($"** Connect() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentOutOfRangeException exp)
            {
                // 포트 번호가 잘못된 경우
                if (IsLogException) log.Write($"** Connect() ArgumentOutOfRangeException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.
                if (IsLogException) log.Write($"** Connect() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** Connect() ObjectDisposedException : {exp.Message}");
            }
            catch (NotSupportedException exp)
            {
                // 이 메서드는 InterNetwork 또는 InterNetworkV6 제품군의 소켓에 유효합니다.
                if (IsLogException) log.Write($"** Connect() NotSupportedException : {exp.Message}");
            }
            catch (InvalidOperationException exp)
            {
                // Socket이 Listen(Int32)을 호출하여 수신 상태에 배치되었습니다.
                if (IsLogException) log.Write($"** Connect() InvalidOperationException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** Connect() Exception : {exp.Message}");
            }

            return false;       // 예외 발생시 수행 실패
        }


        public bool IsConnected()
        {
            if (socketClient == null) return false;

            try
            {
                return !((socketClient.Poll(1000, SelectMode.SelectRead) && (socketClient.Available == 0)) || !socketClient.Connected);
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.
                if (IsLogException) log.Write($"** IsConnected() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** IsConnected() ObjectDisposedException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** IsConnected() Exception : {exp.Message}");
            }

            return false;

            // The long, but simpler-to-understand version:
            //        bool part1 = s.Poll(1000, SelectMode.SelectRead);
            //        bool part2 = (s.Available == 0);
            //        if ((part1 && part2 ) || !s.Connected)
            //            return false;
            //        else
            //            return true;
            //
        }



        // 접속 시도에 대한 Callback event 발생
        public void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                socketClient.EndConnect(ar);            // 연결시도 종료
                socketClient.BeginReceive(byBuffer, 0, byBuffer.Length, 0, fnReceiveHandle, this); // socketClient);   // 수신 Callback 함수를 대입한다.

                tmrTryConnect.Stop();                   // Timeout 처리 및 재접속 timer 중지
                nAbnormalCount = 0;                     // 비정상 상태 반복횟수
                evtConnectDone.Set();                   // 연결이 설정되었다.

                if (fnCallbackConnect != null)
                {
                    fnCallbackConnect(ar);              // 연결이 이루어질 경우 발생되는 callback 함수
                }

                if (IsLogEvent) log.Write($"!! OnConnectCallback(), connected : {socketClient.Connected}");
                return;
            }
            catch (ArgumentNullException exp)
            {
                // host이(가) null인 경우
                if (IsLogException) log.Write($"** OnConnectCallback() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentOutOfRangeException exp)
            {
                // 포트 번호가 잘못된 경우
                if (IsLogException) log.Write($"** OnConnectCallback() ArgumentOutOfRangeException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.
                if (IsLogException) log.Write($"** OnConnectCallback() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** OnConnectCallback() ObjectDisposedException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** OnConnectCallback() Exception : {exp.Message}");
            }

            // 정상적이지 않다.
            // log.Write($"* OnConnectCallback(), connected fail");
            socketClient.Close();
            socketClient = null;
            evtConnectDone.Reset();         // 연결이 해제되었다.

            // 자동으로 재접속을 시도하기로 지정되었다면
            if (IsAutoConnectTry)
            {
                tmrTryConnect.Interval = ConnectDelay;      // 다시 연결을 지령하기위한 지연 시간
                tmrTryConnect.Start();          // 다시 타이머를 동작시킨다.
            }

        }


        // 데이터 수신이 이루어졌을때 발생하는 Callback 함수
        public void OnReceiveCallback(IAsyncResult ar)
        {
            nRecvLength = 0;

            try
            {
                // 자료를 수신하고, 수신받은 바이트를 가져옵니다.
                nRecvLength = socketClient.EndReceive(ar);
            }
            catch (ArgumentNullException exp)
            {
                // host이(가) null인 경우
                if (IsLogException) log.Write($"** OnReceiveCallback() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentOutOfRangeException exp)
            {
                // 포트 번호가 잘못된 경우
                if (IsLogException) log.Write($"** OnReceiveCallback() ArgumentOutOfRangeException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.
                if (IsLogException) log.Write($"** OnReceiveCallback() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** OnReceiveCallback() ObjectDisposedException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** OnReceiveCallback() Exception : {exp.Message}");
            }


            // 수신받은 자료의 크기가 1 이상일 때에만 자료 처리
            if (nRecvLength > 0)
            {
                byte[] data = new byte[nRecvLength];            // 수신된 데이터 수 만큼 배열을 잡는다.

                Array.Copy(byBuffer, 0, data, 0, nRecvLength);  // 수신된 길이만큼 내용을 복사한다

                listRcvData.Add(data);                          // 복사된 수신 데이터를 list에 보관한다.


                if (IsLogEventReceive) log.Write($"!! OnReceiveCallback : {nRecvLength}, RcvList:{listRcvData.Count} ");    //, [{sMsg}]");
                if (IsLogData) // 송수신 메세지를 로그로 기록하라고 되어있다면
                {

                    if (IsStringData)      // 전송데이터가 문자열인가 바이너리인가 ?
                    {
                        string sMsg;

                        if (IsStringUnicode) // 문자열을 Unicode로 송/수신 할것인가 ?
                        {
                            sMsg = Encoding.Unicode.GetString(byBuffer, 0, nRecvLength);
                        }
                        else
                            sMsg = Encoding.ASCII.GetString(byBuffer, 0, nRecvLength);

                        // 수신 내용을 log에 남긴다.
                        if (IsLogLength)            // 수신받은 데이터 길이를 표시할 것인가 ?
                        {
                            log.Write($"<- ({nRecvLength}) {sMsg}");    // 수신받은 길이 표시
                        }
                        else
                            log.Write($"<- {sMsg}");
                    }
                    else
                    {       // Binary data
                        StringBuilder sbMsg = new StringBuilder();

                        for (int i = 0; i < data.Length; i++)
                        {
                            sbMsg.Append($"{data[i]:X2} ");     // 2자리 16진수로 표시
                        }

                        if (IsLogLength)            // 수신받은 데이터 길이를 표시할 것인가 ?
                        {
                            log.Write($"<- ({nRecvLength}) {sbMsg}");
                        }
                        else
                            log.Write($"<- {sbMsg}");
                    }

                }

                // 수신데이터 처리를 위한 event set 최종 사용자가 사용한다.
                evtReceiveFlag.Set();

                // 수신완료 함수 수행
                if (fnCallbackReceive != null)
                {
                    fnCallbackReceive(ar);              // 데이터 수신이 이루어질 경우 발생되는 callback 함수
                }

                // byBuffer.Initialize(); -> 정상 동작하지 않음
                Array.Clear(byBuffer, 0x0, byBuffer.Length);            // Buffer Clear

                // 다음 수신을 대기한다.
                socketClient.BeginReceive(byBuffer, 0, byBuffer.Length, 0, fnReceiveHandle, this); // socketClient);
            }
            else
            {
                // 수신된 Size가 0일 경우 연결이 끊어진 것이다.
                if (IsLogEvent) log.Write($"!! OnReceiveCallback : receive {nRecvLength} Bytes -> Client close");

                evtConnectDone.Reset();         // 연결이 해제되었다.

                if (fnCallbackDisconnect != null)
                {
                    fnCallbackDisconnect(ar);              // 연결이 끊어질 경우 발생되는 callback 함수
                }
                socketClient.Close();
                socketClient = null;

                // 자동으로 재접속을 시도하기로 지정되었다면
                if (IsAutoConnectTry)
                {
                    tmrTryConnect.Interval = ConnectDelay;      // 다시 연결을 지령하기위한 지연 시간
                    tmrTryConnect.Start();          // 다시 타이머를 동작시킨다.
                }

            }
        }

        // 데이터 송신이 이루어졌을때 발생하는 Callback 함수
        public void OnSendCallback(IAsyncResult ar)
        {
            try
            {
                nSendLength = socketClient.EndSend(ar);                     // 송신 처리
                evtSendDone.Set();

                if (IsLogEventReceive) log.Write($"!! OnSendCallback : {nSendLength}");
            }
            catch (Exception exp)
            {
                nSendLength = 0;                                            // 송신 실패
                if (IsLogException) log.Write($"** OnSendCallback() SocketException : {exp.Message}");

                evtConnectDone.Reset();         // 연결이 해제되었다.

            }
        }


        // 문자열을 전송한다. (비동기 방식)
        public bool SendStringASync(string sMsg)
        {
            if (sMsg.Length <= 0) return false;                     // 전송 할 내용이 없다면 전송 실패


            byte[] data = IsUnicode ? Encoding.Unicode.GetBytes(sMsg) : Encoding.ASCII.GetBytes(sMsg);

            try
            {
                // 지정 데이터를 전송 시도한다.
                nSendLength = 0;
                evtSendDone.Reset();

                socketClient.BeginSend(data, 0, data.Length, 0, fnSendHandle, this);    // socketClient);

                nAbnormalCount = 0;                      // 비정상 상태 반복횟수 clear

                return true;
            }
            catch (ArgumentNullException exp)
            {
                // buffers은 null입니다.
                if (IsLogException) log.Write($"** SendString() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentException exp)
            {
                // buffers가 비어 있는 경우
                if (IsLogException) log.Write($"** SendString() ArgumentException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.아래의 설명 부분을 참조하십시오.
                if (IsLogException) log.Write($"** SendString() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** SendString() ObjectDisposedException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** SendString() Exception : {exp.Message}");
            }

            ++nAbnormalCount;                      // 비정상 상태 반복횟수
            return false;
        }


        // 문자열을 전송한다. (동기 방식)
        public bool SendString(string sMsg)
        {
            if (sMsg.Length <= 0) return false;                     // 전송 할 내용이 없다면 전송 실패

            byte[] data;

            if (IsStringUnicode) // 문자열을 Unicode로 송/수신 할것인가 ?
            {
                // sMsg = Encoding.Unicode.GetString(byBuffer, 0, nRecvLength);
                data = Encoding.Unicode.GetBytes(sMsg);
            }
            else
                data = Encoding.ASCII.GetBytes(sMsg);

            // byte[] data = Encoding.Unicode.GetBytes(sMsg);
            // byte[] data = Encoding.ASCII.GetBytes(sMsg);

            try
            {
                // 지정 데이터를 전송 시도한다.
                evtSendDone.Reset();
                nSendLength = socketClient.Send(data, data.Length, 0);  // , MSG_NOSIGNAL);

                nAbnormalCount = 0;                                   // 비정상 상태 반복횟수 clear
                evtSendDone.Set();

                if (IsLogData) // 송수신 메세지를 로그로 기록하라고 되어있다면
                {

                    if (IsStringData)      // 전송데이터가 문자열인가 바이너리인가 ?
                    {
                        // 수신 내용을 log에 남긴다.
                        if (IsLogLength)            // 수신받은 데이터 길이를 표시할 것인가 ?
                        {
                            log.Write($"-> ({nSendLength}) {sMsg}");
                        }
                        else
                            log.Write($"-> {sMsg}");
                    }
                    else
                    {       // Binary data
                        StringBuilder sbMsg = new StringBuilder();

                        for (int i = 0; i < data.Length; i++)
                        {
                            sbMsg.Append($"{data[i]:X2} ");
                        }

                        if (IsLogLength)            // 수신받은 데이터 길이를 표시할 것인가 ?
                        {
                            log.Write($"-> ({nSendLength}) {sbMsg}");
                        }
                        else
                            log.Write($"-> {sbMsg}");
                    }
                }

                return (nSendLength > 0);
            }
            catch (ArgumentNullException exp)
            {
                // buffers은 null입니다.
                if (IsLogException) log.Write($"** SendString() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentException exp)
            {
                // buffers가 비어 있는 경우
                if (IsLogException) log.Write($"** SendString() ArgumentException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.아래의 설명 부분을 참조하십시오.
                if (IsLogException) log.Write($"** SendString() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** SendString() ObjectDisposedException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** SendString() Exception : {exp.Message}");
            }

            ++nAbnormalCount;                      // 비정상 상태 반복횟수
            return false;
        }

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
            //Lot1,Mgz1,Slot1,SlotOK,Group1,Deive1,Strip1
            //Msg = "EQSendOut,Bypass,[" + LotName + ",NoMgz:" + MgzNum + ",NoSlot:" + SlotNum + ",GN:" + GroupName + ",DN:" + DeviceName + ",OcrInfo:" + OcrInfo + ";";
            Msg = "EQSendOut,Bypass," + LotName + "," + MgzNum + "," + SlotNum + ",SlotOK," + GroupName + "," + DeviceName + "," + OcrInfo;
            SendMessage(Msg);
        }

        public void SendStartTest()
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
            Msg = "EQSendOut,Test," + LotName + "," + MgzNum + "," + SlotNum + ",SlotOK," + GroupName + "," + DeviceName + "," + OcrInfo ;
            SendMessage(Msg);
        }

        public void writeLog(string sMsg)
        {
            log.Write($" {sMsg}");
        }

        // 지정 Byte 배열의 데이터를 전송한다. (동기 방식)
        public bool Send(byte[] SendData)
        {
            if (SendData.Length <= 0) return false;                     // 전송 할 내용이 없다면 전송 실패

            try
            {
                // 지정 데이터를 전송 시도한다.
                evtSendDone.Reset();
                nSendLength = socketClient.Send(SendData, SendData.Length, 0);

                nAbnormalCount = 0;                                   // 비정상 상태 반복횟수 clear
                evtSendDone.Set();

                if (IsLogData) // 송수신 메세지를 로그로 기록하라고 되어있다면
                {
                    // Binary data
                    StringBuilder sbMsg = new StringBuilder();

                    for (int i = 0; i < SendData.Length; i++)
                    {
                        sbMsg.Append($"{SendData[i]:X2} ");
                    }

                    if (IsLogLength)            // 수신받은 데이터 길이를 표시할 것인가 ?
                    {
                        log.Write($"-> ({nSendLength}) {sbMsg}");
                    }
                    else
                        log.Write($"-> {sbMsg}");
                }

                return (nSendLength > 0);
            }
            catch (ArgumentNullException exp)
            {
                // buffers은 null입니다.
                if (IsLogException) log.Write($"** SendString() ArgumentNullException : {exp.Message}");
            }
            catch (ArgumentException exp)
            {
                // buffers가 비어 있는 경우
                if (IsLogException) log.Write($"** SendString() ArgumentException : {exp.Message}");
            }
            catch (SocketException exp)
            {
                // 소켓에 액세스하는 동안 오류가 발생했습니다.아래의 설명 부분을 참조하십시오.
                if (IsLogException) log.Write($"** SendString() SocketException : {exp.Message}, ErrorCode:{exp.ErrorCode}");
            }
            catch (ObjectDisposedException exp)
            {
                // Socket이 닫혔습니다.
                if (IsLogException) log.Write($"** SendString() ObjectDisposedException : {exp.Message}");
            }
            catch (Exception exp)
            {
                if (IsLogException) log.Write($"** SendString() Exception : {exp.Message}");
            }

            ++nAbnormalCount;                      // 비정상 상태 반복횟수
            return false;
        }
    } //of public class CGxSocket

}
