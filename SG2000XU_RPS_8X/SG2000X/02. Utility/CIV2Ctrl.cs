using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SG2000X.Gaus.Log;
using System.Windows.Forms;  // for ListBox

namespace SG2000X
{
    public class IV2Result
    {
        public const int NONE = -1;  // 아직 결과가 없다.
        public const int OK = 1;     // OK 결과
        public const int NG = 0;     // NG 결과
    }

    class CIV2Ctrl
    {
        public CGxTCPSocket m_sockClient = new CGxTCPSocket();     // 통신 Socket
        CGxLog m_log;

        int m_nIdx = 0;                             // 통신 인스턴스 Index
        public char m_charCR = '\r';                // 전문 종료 문자 = CR
        public bool m_bIsUseETX = true;             // 종료문자를 사용할 것인가 ?

        public int m_nSaveLimit = 1024;             // 종료 문자를 기다리는 최대 문자열 길이, 이 길이를 넘어서면 데이터 수신으로 간주
        public string m_sSavedMsg = string.Empty;   // EQ 이전에 수신된 문자가 있는가 ?

        /// <summary>
        ///보낸 명령에 대한 회신을 받았는가 ?
        /// </summary>
        public bool IsRcvReply { set; get; } = false;
        /// <summary>
        /// 회신된 프로그램 번호 조회 결과
        /// </summary> 
        public int RcvProgramNo { set; get; } = IV2Result.NONE;
        /// <summary>
        /// 회신된 프로그램 번호 설정 결과
        /// </summary>
        public int RcvSetProgramNo { set; get; } = IV2Result.NONE;
        /// <summary>
        /// 회신된 트리거 결과
        /// </summary>
        public int RcvResult { set; get; } = IV2Result.NONE;
        /// <summary>
        /// 회신된 운전상태 결과
        /// </summary>
        public int RcvRunCheck { set; get; } = IV2Result.NONE;
        /// <summary>
        /// 회신된 센서 상태 결과
        /// 211004 현재 Busy 상태만 확인 중
        /// </summary>
        public int RcvStateCheck { set; get; } = IV2Result.NONE;
        /// <summary>
        /// 카메라 에러상태 조회
        /// OK : 정상
        /// NG : 에러 상태
        /// </summary>
        public int RcvErrCheck { set; get; } = IV2Result.NONE;
        /// <summary>
        /// IV2 연결 여부
        /// </summary>
        public bool IsConnect { get { return (m_sockClient?.IsConnected() ?? false); } }

        #region 통신 연결
        public CIV2Ctrl(int nIdx = 0)
        {
            m_nIdx = nIdx;
            string sPath = ""; //210907 syc : 2004U IV2

            // EQ 통신에 대한 설정
            m_sockClient.IsStringData = true;                    // 문자열로 데이터를 처리한다.
            m_sockClient.IsStringUnicode = false;                 // ASCII 
            m_sockClient.IsAutoConnectTry = true;                 // 연결이 끊어질 경우 자동으로 Retry를 수행한다.

            m_sockClient.nID = 0;                                 // 통신 ID는 0
            m_sockClient.sName = $"VI2_{m_nIdx}";                 // IV2 연결 TCP/IP 통신
            m_log = m_sockClient.Log;                               // Log instance

            //210907 syc : 2004U IV2
            sPath = m_nIdx == 0 ? "ONP" : "OFP";
            m_log.SetFileName(@"D:\Log_SG2X\IV2", sPath);

            m_sockClient.SetCallbackReceive(OnClientReceiveFunction);
            m_sockClient.SetCallbackConnect(OnClientConnectFunction);
            m_sockClient.SetCallbackDisconnect(OnClientDisconnectFunction);
        }
        //210907 syc : 2004U
        public void LogList(ListBox pLi)
        {
            m_log.SetListBox(pLi);
        }

        //접속할 IV2 주소지정
        public void SetAddress(string sAddr, int nPort)
        {
            m_sockClient.SetAddress(sAddr, nPort);
        }

        // IV2와 접속을 시도한다. 접속 실패시 자동으로 Retry를 수행한다.
        public void Connect()
        {
            m_sockClient.Connect();
        }

        // IV2와의 접속을 끊는다.
        public void Close()
        {
            m_sockClient.CloseClient();
        }

        // Clinet가 연결에 성공했을 때
        private void OnClientConnectFunction(IAsyncResult ar)
        {
            CGxTCPSocket socket = (ar.AsyncState as CGxTCPSocket);

            m_log.Write($"---- Client connect success ID:{socket.nID}, Name:{socket.sName}");
        }

        // Clinet가 연결이 끊어졌을 때
        private void OnClientDisconnectFunction(IAsyncResult ar)
        {
            CGxTCPSocket socket = (ar.AsyncState as CGxTCPSocket);

            m_log.Write($"**** Client Disconnect ID:{socket.nID}, Name:{socket.sName}");
        }
        #endregion

        // EQ 클라이언트 소켓에서 데이터를 수신 받은경우 처리
        private void OnClientReceiveFunction(IAsyncResult ar)
        {
            byte[] byData;
            string sMsg;

            while (m_sockClient.GetByteData(out byData))
            {
                sMsg = Encoding.ASCII.GetString(byData, 0, byData.Length);

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

                    string[] sArrMsg = sMsg.Split(m_charCR);           // 지정된 종료문자로 분리를 한다.

                    if (sMsg[sMsg.Length - 1] == m_charCR)           // 마지막 문자가 종료 문자인가 ?
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
                            m_log.Write($"* ERROR limit over saved buffer size {m_sSavedMsg.Length}, Force process data : {m_sSavedMsg}");

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

        #region  명령어 전송
        // 명령어 전송 --------------------------------------------------------------------------------------------------------------------------------------- 
        public bool SendMessage(string sMsg)
        {
            bool bRet = false;

            if (m_sockClient.IsConnected())
            {
                bRet = m_sockClient.SendString(sMsg + "\r");
                m_log.Write($"-> {sMsg.Trim()} : {bRet}");
            }
            else
                m_log.Write($"*> Send fail : {sMsg.Trim()}");

            return bRet;            // 전송 성공 여부
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // 현재 설정된 프로그램 번호를 조회한다. -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 설정된 프로그램 번호 조회
        /// </summary>
        /// <returns></returns>
        public bool SendProgramRead()
        {
            IsRcvReply = false;                        // 회신 여부 플랙그 초기화
            RcvProgramNo = IV2Result.NONE;             // 회신되어 온 Program 번호 초기화

            return SendMessage("PR");                  // 명령 전송
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // 지정한 프로그램 번호를 설정한다. ------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 지정한 프로그램 번호 설정
        /// </summary>
        /// <param name="nProgram"></param>
        /// <returns></returns>
        public bool SendProgramWrite(int nProgram)
        {
            IsRcvReply = false;                        // 회신 여부 플랙그 초기화
            RcvSetProgramNo = IV2Result.NONE;          // 회신된 프로그램 변경 결과 초기화

            return SendMessage($"PW,{nProgram:00#}");  // 명령 전송

        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // 트리거 신호 전송----------------------------------------------------------------------------------------------------------------------------------- 
        /// <summary>
        /// 트리거 신호 전송
        /// </summary>
        /// <returns></returns>
        public bool SendTrigger()
        {
            IsRcvReply = false;                        // 회신 여부 플랙그 초기화
            RcvResult = IV2Result.NONE;                // 회신되어 온 Trigger 결과 초기화

            return SendMessage("T2");                  // 명령 전송
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // 운전상태 확인 ------------------------------------------------------------------------------------------------------------------------------------- 
        /// <summary>
        /// 운전 상태 확인
        /// </summary>
        /// <param name="sMsg"></param>
        /// <returns></returns>
        public bool SendRM()
        {
            IsRcvReply = false;                        // 회신 여부 플랙그 초기화
            RcvRunCheck = IV2Result.NONE;              // 회신되어 온 운전상태 결과 초기화

            return SendMessage("RM");
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // IV2 센서 상태 확인--------------------------------------------------------------------------------------------------------------------------------- 
        /// <summary>       
        /// IV2 센서 상태 확인
        /// IV2 센서 사용 가능 상태 확인  //Busy 상태 확인 목적
        /// Busy 면 NG
        /// Busy 가 아니면 OK
        /// </summary>
        /// <returns></returns>
        public bool SendSR()
        {
            IsRcvReply = false;                        // 회신 여부 플랙그 초기화  
            RcvStateCheck = IV2Result.NONE;            // 회신되어 온 센서 상태 결과 초기화

            return SendMessage("SR");
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // 에러 상태 확인------------------------------------------------------------------------------------------------------------------------------------- 
        //210907 syc : 2004U
        // 메세지 전송
        public bool SendERRCheck()
        {
            IsRcvReply = false;                        // 회신 여부 플랙그 초기화
            RcvErrCheck = IV2Result.NONE;              // 에러 결과 초기화

            return SendMessage("RER");                  // 명령 전송
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        // 입력 텍스트 전송----------------------------------------------------------------------------------------------------------------------------------- 
        //210907 syc : 2004U
        // 메세지 전송
        public bool SendString(string sMsg)
        {
            IsRcvReply = false;                      // 회신 여부 플랙그 초기화
            RcvResult = IV2Result.NONE;              // 회신되어  결과 초기화

            return SendMessage(sMsg);                   // 명령 전송
        }
        // end ----------------------------------------------------------------------------------------------------------------------------------------------- 
        #endregion

        #region 명령어 회신 처리 (결과값 반환)
        // 수신된 전문을 처리한다.
        protected void ProcessMessage(string sMsg)
        {
            if (sMsg.Length <= 0) return;               // 수신된 데이터가 없으면 처리 불가

            m_log.Write($"   [{sMsg}]");                // 수신된 데이터


            string[] arrRecv = sMsg.Split(',', ':', ';', '_');         // 분리자로 각 항목을 분리한다.
            string sCmd = arrRecv[0].Trim();            // 명령문을 토대로 데이터를 처리한다.
            string sMode = "";                          // 명령문에 따라서 다양한 선택 데이터가 존재한다.

            if (arrRecv.Length >= 2)                    // 뒤이어서 데이터가 붙어오는가 ?
            {
                sMode = arrRecv[1];
            }

            // 현재 프로그램 조회 --------------------------------------------------------------------------------------------------------------------------------
            if (String.Equals(sCmd, "PR", StringComparison.OrdinalIgnoreCase))
            {
                m_log.Write($"<- [PR] Reply : {sMode}");

                int nValue = IV2Result.NONE;

                if (int.TryParse(sMode, out nValue))
                {
                    RcvProgramNo = nValue;
                }
                else
                    RcvProgramNo = IV2Result.NONE;

                IsRcvReply = true;
            }
            // end -----------------------------------------------------------------------------------------------------------------------------------------------            
            // PW 프로그램 변경에 대한 회신 ----------------------------------------------------------------------------------------------------------------------
            // 프로그램 변경 성공
            else if (String.Equals(sCmd, "PW", StringComparison.OrdinalIgnoreCase))
            {
                //PW가 수신된 경우 PW(프로그램 변경) 성공
                m_log.Write("$<- [PW] Reply");

                RcvSetProgramNo = IV2Result.OK; //프로그램 변경 성공
                IsRcvReply = true;           // 회신 왔다.
            }
            // 프로그램 변경 실패
            else if (String.Equals(sCmd, "ER", StringComparison.OrdinalIgnoreCase) && String.Equals(arrRecv[1], "PW", StringComparison.OrdinalIgnoreCase))
            {
                //"ER, PW, 프록램 번호" -> 해당번호 프로그램 변경 실패
                m_log.Write($"<- [RT] Reply, : {arrRecv[1]}, {arrRecv[2]}");
                RcvSetProgramNo = IV2Result.NG;
            }
            // end -----------------------------------------------------------------------------------------------------------------------------------------------            
            // T2 트리거 결과에 대한 회신 ------------------------------------------------------------------------------------------------------------------------
            else if (String.Equals(sCmd, "RT", StringComparison.OrdinalIgnoreCase))
            {
                //210908 syc : 2004U
                // "명령어, 트리거 번호, 판정결과" 순서로 들어옴
                m_log.Write($"<- [RT] Reply, : {arrRecv[1]}, {arrRecv[2]}");
                if (arrRecv[2].ToString() == "OK") { RcvResult = IV2Result.OK; }
                else if (arrRecv[2].ToString() == "NG") { RcvResult = IV2Result.NG; }

                IsRcvReply = true;                        // 회신 왔다.
            }
            // end ------------------------------------------------------------------------------------------------------------------------------------------------
            // 프로그램 운전상태 확인 결과에 대한 회신-------------------------------------------------------------------------------------------------------------
            else if (String.Equals(sCmd, "RM", StringComparison.OrdinalIgnoreCase))
            {
                //210915 syc : 2004U
                // "명령어, 판정결과" 순서로 들어옴
                // 0 : 설정 중
                // 1 : 운전 중
                m_log.Write($"<- [RT] Reply, : {arrRecv[1]}");
                if (arrRecv[1].ToString() == "1") { RcvRunCheck = IV2Result.OK; }
                else if (arrRecv[1].ToString() == "0") { RcvRunCheck = IV2Result.NG; }

                IsRcvReply = true;                        // 회신 왔다.
            }
            // end ------------------------------------------------------------------------------------------------------------------------------------------------
            // 프로그램 센서상태 확인 결과에 대한 회신-------------------------------------------------------------------------------------------------------------
            else if (String.Equals(sCmd, "SR", StringComparison.OrdinalIgnoreCase))
            {
                //210915 syc : 2004U
                // 0 : Busy 아님
                // 1 : Busy 상태
                m_log.Write($"<- [RT] Reply, : {arrRecv[1]}");
                if (arrRecv[1].ToString() == "1") { RcvStateCheck = IV2Result.NG; } // 센서 상태 Busy
                else if (arrRecv[1].ToString() == "0") { RcvStateCheck = IV2Result.OK; } // 센서 상태 Busy 아님

                IsRcvReply = true;                        // 회신 왔다.
            }
            // end ------------------------------------------------------------------------------------------------------------------------------------------------
            // 에러상태 확인에 대한 회신---------------------------------------------------------------------------------------------------------------------------
            else if (String.Equals(sCmd, "RER", StringComparison.OrdinalIgnoreCase))
            {
                //210915 syc : 2004U
                // 000 : 에러 없음

                m_log.Write($"<- [RT] Reply, : {arrRecv[1]}");
                if (arrRecv[1].ToString() == "000") { RcvErrCheck = IV2Result.OK; } // 에러상태 아님
                else { RcvErrCheck = IV2Result.NG; } // 에러 상태

                IsRcvReply = true;                        // 회신 왔다.
            }
            // end ------------------------------------------------------------------------------------------------------------------------------------------------
            #endregion
        }
    } //class end
} // namespace ned
