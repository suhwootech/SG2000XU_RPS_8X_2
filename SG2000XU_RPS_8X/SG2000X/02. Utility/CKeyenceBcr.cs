using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SG2000X
{
     #region CTcpSocket
    /// <summary>
    /// 수신 데이터 처리용 콜백
    /// </summary>
    /// <param name="rcvData"></param>
    /// <returns></returns>
    public delegate int CallBackKeyenceTcpRcvData(int idx, byte [] rcvByte, int rcvLen);

    public class CKeyenceTcpSocket : CStn<CKeyenceTcpSocket>
    {
        public const int    ERR_OK              = (0);          // 정상 (No Error)  
        public const int    ERR_GENERAL         = (-10);        // 일반적인 에러 (Invalid index, Invalid Format, ...)
        public const int    ERR_NOT_CONNECTED   = (-100);       // 소켓 연결 안 된 상태
        public const int    ERR_SOCKET          = (-200);       // SocketException
        public const int    ERR_EXCEPTION       = (-900);       // Exception

        public const int    FIONREAD            = 0x4004667F;   // FIONREAD is also available as the "Available" property

        public const char   CHAR_CR             = '\r';         // CR char 값 (CR = '\r' = 0x0D)
        public const string STR_CR              = "\r";         // CR string 값 (CR = '\r' = 0x0D)
        public const char   CHAR_LF             = '\n';         // LF char 값 (LF = '\n' = 0x0A)
        public const string STR_LF              = "\n";         // LF string 값 (LF = '\n' = 0x0A)

        public const int    BUFF_SIZE           = (2 * 1024);   // 수신 버퍼 사이즈 (Bytes)

        public int m_idx;

        public Socket m_sock;  //BCR TCP Client Socket
        public IPEndPoint m_ep;
        private int m_msConnTimeout = 3000;
        Thread ReceiveThread;

        private bool m_bConnected = false;  //소켓 연결 상태 체크용
        public bool m_bStop = false;        //종료 플래그 : true > 추가 작업 중지, Socket Close해야 함

        private CallBackKeyenceTcpRcvData CB_ProcessRcv;

        public CKeyenceTcpSocket(int idx, string sIpAddress, int bcrPort, int connectTimeout, CallBackKeyenceTcpRcvData cb)
        {
            m_idx = idx; //여러개의 BCR 사용 시 수신 데이터 처리 시 구분
            CB_ProcessRcv = cb;

            m_ep = new IPEndPoint(IPAddress.Parse(sIpAddress), bcrPort);
            m_sock = null;
            m_bConnected = false;
            m_bStop = false;
            ReceiveThread = null;

            m_msConnTimeout = connectTimeout;
        }

        public int Connect()
        {
            string msg;

            try
            {
                //소켓이 Open 상태라면 Close
                if (m_sock != null)
                {
                    _disconnect(m_sock);
                    if (m_sock != null)
                    { m_sock = null; }
                }

                //소켓 Create
                m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
                m_sock.SendTimeout = 1500;      //송신 timeout 1500 ms
                m_sock.ReceiveTimeout = 100;    //수신 timeout 100 ms

#if true //Method 0. Connect with timeout
                IAsyncResult asyncResult = m_sock.BeginConnect(m_ep, null, null);
                if (asyncResult.AsyncWaitHandle.WaitOne(m_msConnTimeout, false))
                {
                    m_sock.EndConnect(asyncResult);
                }
                else
                {
                    //throw new Exception(string.Format("{0}:{1} Connection Fail (Timeout).", m_ep.Address.ToString(), m_ep.Port.ToString()));
                    Log("ERROR : Connection Fail (Timeout) [" + m_ep.Address.ToString() + ":" + m_ep.Port.ToString() + "]");

                    _disconnect(m_sock);

                    if (m_sock != null)
                    { m_sock = null; }

                    return ERR_SOCKET;
                }

#else //Method 1. Connect without timeout (서버 연결 안 될 경우 약 20초 후 return 됨 -> No!!!)
                m_sock.Connect(m_ep);
#endif
            }
            catch (ArgumentOutOfRangeException e)
            {
                msg = "EXCEPTION : " + e.ToString() + " [" + MethodBase.GetCurrentMethod().Name.ToString() + "]  Failed to connect " + m_ep.ToString();
                Log(msg);
                
                m_sock = null;
                m_bConnected = false;
                return ERR_GENERAL;
            }
            catch (SocketException e)
            {
                msg = "EXCEPTION : " + e.ToString() + " [" + MethodBase.GetCurrentMethod().Name.ToString() + "]  Failed to connect " + m_ep.ToString();
                Log(msg);
                
                m_sock = null;
                m_bConnected = false;
                return ERR_SOCKET;
            }
            catch (Exception e)
            {
                msg = "EXCEPTION : " + e.ToString() + " [" + MethodBase.GetCurrentMethod().Name.ToString() + "]  Failed to connect " + m_ep.ToString();
                Log(msg);
                
                m_sock = null;
                m_bConnected = false;
                return ERR_EXCEPTION;
            }

            m_bConnected = true;
            m_bStop = false;

            Log("CONNECT " + m_ep.ToString());

            _beginReceive(); //연결되면 Receive 스레드 시작

            return ERR_OK;
        }

        private void _disconnect(Socket s)
        {
            _endReceive(); //수신 데이터 처리 쓰레드 Running 중이라면 종료

            if (s != null)
            {
                try
                {
                    s.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    s.Close();
                }
                s = null;
                Log("DISCONNECT " + m_ep.ToString());
            }

            m_bConnected = false;
        }

        public void Disconnect()
        {
            if (m_sock != null)
            {
                _disconnect(m_sock); //소켓 종료
                if (m_sock != null)
                { m_sock = null; }
            }
        }

        public bool IsConnected
        {
            get { return m_bConnected; } 
        }

        public int Send(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                Log("WARNING : Message Empty [" + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return ERR_GENERAL;
            }
            try
            {
                byte[] data = ASCIIEncoding.ASCII.GetBytes(text);

                if (!m_bStop && (m_sock != null))
                {
                    int sndBytes = m_sock.Send(data);

                    if( sndBytes > 0 )
                    {
                        //송신 로그
                        Log("SEND: " + m_ep.ToString() + " : [" + sndBytes.ToString() + "] \""
                            + (text.Replace(STR_CR, "\\r")).Replace(STR_LF, "\\n") + "\"", true);
                        return ERR_OK;
                    }
                    else
                    {
                        Log(string.Format("SEND: {0} : None", sndBytes));
                        return ERR_GENERAL;
                    }
                }
                else
                {
                    Log(string.Format("ERROR (SEND) : {0} is disconnected.", m_ep.ToString()));
                    m_bConnected = false;
                    return ERR_NOT_CONNECTED;
                }
            }
            catch (SocketException e)
            {
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return ERR_SOCKET;
            }
            catch ( Exception e )
            {
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return ERR_EXCEPTION;
            }
        }

        /// <summary>
        /// 소켓 수신 버퍼에 남아있는 데이터 크기 확인
        /// </summary>
        /// <param name="s">체크할 소켓</param>
        /// <returns></returns>
        private int _checkPendingByteCount(Socket s)
        {
            int bytesAvailable = 0;
            byte[] outValue = BitConverter.GetBytes(0);

            try
            {
                // Check how many bytes have been received.
                if (!m_bStop && (s != null))
                {

#if true //Method 0. Check Available property.
                    bytesAvailable = s.Available;
#endif

#if false //Method 1. Calling IOControl with DataToRead
                    s.IOControl(IOControlCode.DataToRead, null, outValue);
                    bytesAvailable = (int)(BitConverter.ToUInt32(outValue, 0));
#endif

#if false //Method 2. FIONREAD is also available as the "Available" property
                    s.IOControl(FIONREAD, null, outValue);
                    bytesAvailable = (int)(BitConverter.ToUInt32(outValue, 0));
#endif
                }
            }
            catch (SocketException e)
            {
                bytesAvailable = ERR_SOCKET;
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
            }
            catch (Exception e)
            {
                bytesAvailable = ERR_EXCEPTION;
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
            }

            return bytesAvailable;
        }

        /// <summary>
        /// 소켓 수신 버퍼 클리어 (Command Port, Data Port 모두)
        /// </summary>
        public void ClearReadBuffer()
        {
            int bytesAvailable = 0;
            Byte[] recvBytes = new Byte[BUFF_SIZE];
            int recvSize = 0;

            if (!m_bStop && (m_sock != null))
            {
                bytesAvailable = _checkPendingByteCount(m_sock);
                if (0 < bytesAvailable)
                {
                    try
                    {
                        recvSize = m_sock.Receive(recvBytes); //Read all the receive buffer data -> Clear receive buffer
                    }
                    catch (Exception e)
                    {
                        recvSize = 0;
                        Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                    }
                }
            }
        }

        private void _beginReceive()
        {
            _endReceive(); //스레드 Running 중이라면 종료

            m_bStop = false; //Receive Thread 종료 플래그 리셋

            ReceiveThread = new Thread(new ThreadStart(_receiveThreadProc));
            ReceiveThread.Start(); //Receive 스레드 시작
        }

        private void _endReceive()
        {
            m_bStop = true; //Receive Thread 종료 플래그 셋
            Thread.Sleep(500);

            if (ReceiveThread != null)
            { ReceiveThread.Join(); } //Thread 종료 대기
        }

        private void _receiveThreadProc()
        {
            Log("Receive Thread BEGIN [" + m_ep.ToString() + "]");

            Byte[] recvBytes = new Byte[BUFF_SIZE];
            int recvSize = 0;

            while (!m_bStop)
            {
                Thread.Sleep(1);
                if ((m_sock != null) && (0 < _checkPendingByteCount(m_sock)))
                {
                    try
                    {
                        recvSize = m_sock.Receive(recvBytes); //Read from Receive Buffer
                    }
                    catch (Exception e)
                    {
                        recvSize = 0;
                        Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                    }

                    if (0 < recvSize)
                    {
                        recvBytes[recvSize] = 0;
                        CB_ProcessRcv(m_idx, recvBytes, recvSize); //수신 데이터 처리

                        //수신 로그
                        Log("RECV: " + m_ep.ToString() + " : [" + recvSize.ToString() + "] \""
                            + ((Encoding.ASCII.GetString(recvBytes, 0, recvSize)).Replace(STR_CR, "\\r")).Replace(STR_LF, "\\n") + "\"", true);

                        //버퍼 클리어
                        recvBytes = new Byte[BUFF_SIZE];
                    }
                }
            }

            Log("Receive Thread END [" + m_ep.ToString() + "]");
        }

#region 로그
        /// <summary>
        /// CTcpSocket 클래스 이벤트 로그 (송/수신 데이터 포함)
        /// </summary>
        /// <param name="msg">로그 메시지</param>
        /// <param name="debug">true : 디버그 output 창에 출력</param>
        private void Log(string msg, bool debug=false)
        {
            SaveLog( msg );
            if(debug)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }

        /// <summary>
        /// CTcpSocket 클래스 이벤트 로그 (송/수신 데이터 포함)
        /// YYYY\MM\DD\KYNC_TCP\YYYYMMDDHH.txt
        /// </summary>
        private void SaveLog(string msg)
        {
            DateTime dtNow = DateTime.Now;
            string sPath = GV.PATH_LOG + dtNow.Year.ToString("0000") + "\\" +  dtNow.Month.ToString("00") + "\\" + dtNow.Day.ToString("00") + "\\KYNC_TCP_" + m_idx.ToString() + "\\";
            string sLine = "";

            if(!Directory.Exists(sPath.ToString()))
            {
                Directory.CreateDirectory(sPath.ToString());
            }

            sLine = dtNow.ToString("[yyyyMMdd-HHmmss fff]") + "\t" + msg;

            CLogManager Log = new CLogManager(sPath, null, null); //Daily
            Log.WriteLine(sLine);   
        }
#endregion
    }
#endregion //CTcpSocket




#region CKeyenceBcr ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class CKeyenceBcr : CStn<CKeyenceBcr>
    {
#region TCP 통신 데이터 처리용
        private const byte   BYTE_STX       = 0x02;         //헤더1
        private const byte   BYTE_ESC       = 0x1B;         //헤더2
        private const byte   BYTE_CR        = 0x0D;         //터미네이터0, 터미네이터1-1
        private const byte   BYTE_LF        = 0x0A;         //터미네이터1-2 (LF = '\n' = 0x0A)
        private const byte   BYTE_ETX       = 0x03;         //터미네이터2
                                                            
        private const char   CHAR_CR        = '\r';         //CR char 값 (CR = '\r' = 0x0D)
        private const char   CHAR_LF        = '\n';         //LF char 값 (LF = '\n' = 0x0A)
                                                            
        private const string STR_CR         = "\r";         //CR string 값 (CR = '\r' = 0x0D)
        private const string STR_LF         = "\n";         //LF string 값 (LF = '\n' = 0x0A)
                                                            
        private const byte   BYTE_SEPARATOR = 0x2C;         //송/수신 데이터 필드간 구분자의 BYTE 값
        private const char   CHAR_SEPARATOR = ',';          //송/수신 데이터 필드간 구분자의 char 값
        private const string STR_SEPARATOR  = ",";          //송/수신 데이터 필드간 구분자의 string 값

        private const string DEFAULT_IP     = "127.0.0.1";  //KEYENCE Barcode Reader IP (Server)             <= INI 파일에서 로딩 실패 시 사용하는 값
        private const int    DEFAULT_PORT   = 9004;         //KEYENCE Barcode Reader Port Number             <= INI 파일에서 로딩 실패 시 사용하는 값
        private const int    DEFAULT_TIMEOUT= 3000;         //KEYENCE Barcode Reader Connection Timeout (ms) <= INI 파일에서 로딩 실패 시 사용하는 값

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string[] sKeyenceBcrCmd = new string [] { "LON", "LOFF", "BCLR" };  // enum  eKeyenceBcrCmd 의 순서와 일치시켜야 함
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private const string STR_CMD_HEADER     = "";       //Command Header
        private const char   CHAR_CMD_TAIL      = CHAR_CR;  //Command Terminator
        private const string STR_CMD_TAIL       = STR_CR;   //Command Terminator

        private const string STR_CMD_RESP_OK    = "OK";     //Command Response OK
        private const string STR_CMD_RESP_ER    = "ER";     //Command Response ER

        private const string STR_DATA_RESP_ERR  = "ERROR";  //Data Response "ERROR", CR(0x0D) is terminator

        public const int KEYENCE_BCR_COUNT              = (int)EKeyenceBcrPos.MaxNum;               //KEYENCE BCR 수

        public string [] sReaderIP                      = new string[KEYENCE_BCR_COUNT];            //KEYENCE Barcode Reader's IP (Server)
        public int iBcrPort                             = DEFAULT_PORT;                             //KEYENCE Barcode Reader Command Port Number
        public int iBcrConnTimeout                      = DEFAULT_TIMEOUT;                          //KEYENCE Barcode Reader Connection Timeout (ms)

        //private CKeyenceTcpSocket[] keyenceTcpClient    = new CKeyenceTcpSocket[KEYENCE_BCR_COUNT]; //KEYENCE Barcode Reader 연결용 소켓 클래스
        private CKeyenceTcpSocket[] keyenceTcpClient    = {null, null};                             //KEYENCE Barcode Reader 연결용 소켓 클래스
#endregion

        private CKeyenceBcr()
        {

        }

        //20200611 jhc : Connection Error 구분 처리
        public int InitializeKeyenceBcr()
        {
            int ret = 0;

            _loadKeyenceBcrConfig(); //KEYENCE BCR Configuration Loading (D:\Suhwoo\SG2000X\Equip\Bcr\KEYENCE.ini)

            for (int i=0; i<KEYENCE_BCR_COUNT; i++)
            {
                if (((i == (int)EKeyenceBcrPos.OnLd) && (CDataOption.OnlRfid == eOnlRFID.Use))
                    || ((i == (int)EKeyenceBcrPos.OffLd) && (CDataOption.OflRfid == eOflRFID.Use)))
                {
                    keyenceTcpClient[i] = new CKeyenceTcpSocket(i, sReaderIP[i], iBcrPort, iBcrConnTimeout, ProcessReceivedData);

                    if (CKeyenceTcpSocket.ERR_OK != this.Connect((EKeyenceBcrPos)i))
                    { ret |= (0x01 << (i)); }
                }
            }
            return ret;
        }

        public void FinalizeKeyenceBcr()
        {
            for (int i=0; i<KEYENCE_BCR_COUNT; i++)
            {
                if (((i == (int)EKeyenceBcrPos.OnLd) && (CDataOption.OnlRfid != eOnlRFID.Use))
                    || ((i == (int)EKeyenceBcrPos.OffLd) && (CDataOption.OflRfid != eOflRFID.Use)))
                { continue; }

                this.Disconnect((EKeyenceBcrPos)i);
            }
        }

        public bool IsConnected(EKeyenceBcrPos ePos)
        {
            int idx = (int)ePos;

            if ((idx < 0) || (KEYENCE_BCR_COUNT <= idx))
            { return false; }

            if (keyenceTcpClient[idx] == null)
            { return false; }

            return keyenceTcpClient[idx].IsConnected;
        }

        /// <summary>
        /// KEYENCE 바코드 리더와 통신 상태/결과 데이터 초기화
        /// </summary>
        /// <param name="ePos"></param>
        public void ClearKeyBcrData(EKeyenceBcrPos ePos, eKeyenceBcrCmd eCmd = eKeyenceBcrCmd.LON)
        {
            int idx = (int)ePos;

            if ((idx < 0) || (KEYENCE_BCR_COUNT <= idx))
            { return; }

            CData.KeyBcr[idx].eState     = eKeyenceBcrTcpStatus.READY;
            CData.KeyBcr[idx].sCmd       = "";
            CData.KeyBcr[idx].sCmdResult = ""; 
            CData.KeyBcr[idx].sErrCode   = "";
            CData.KeyBcr[idx].bStErr     = false;

            //바코드 판독 Command인 경우 기존 바코드 정보 초기화
            if (eCmd == eKeyenceBcrCmd.LON)
            {
                CData.KeyBcr[idx].sBcr = ""; 
            }
        }

        /// <summary>
        /// KEYENCE BCR Configuration Loading (D:\Suhwoo\SG2000X\Equip\Bcr\KEYENCE.ini)
        /// </summary>
        private void _loadKeyenceBcrConfig()
        {
            CIni mIni = new CIni(GV.PATH_EQ_BCR + "KEYENCE.ini");
            
            string sSec = "KEYENCE";
            
            for (int i=0; i <KEYENCE_BCR_COUNT; i++)
            {
                sReaderIP[i] = mIni.Read(sSec,"Reader" + (i+1).ToString() + " IP Address");
                
                if(string.IsNullOrWhiteSpace(sReaderIP[i])) sReaderIP[i] = DEFAULT_IP;
            }
            
            int.TryParse(mIni.Read(sSec,"Port"), out this.iBcrPort);
            
            if(this.iBcrPort == 0) this.iBcrPort = DEFAULT_PORT;
            
            int.TryParse(mIni.Read(sSec,"Connect Timeout"), out this.iBcrConnTimeout);
            
            if(this.iBcrConnTimeout == 0) this.iBcrConnTimeout = DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Connect to the KEYENCE Barcode Reader
        /// </summary>
        /// <param name="i">0: OnLoader 쪽 BCR에 연결, 1: OffLoader 쪽 BCR에 연결</param>
        /// <returns></returns>
        public int Connect(EKeyenceBcrPos ePos)
        {
            int idx = (int)ePos;

            if ((idx < 0) || (KEYENCE_BCR_COUNT <= idx))
            { return (-1); } //Invalid Index

            if (keyenceTcpClient[idx] == null)
            { return (-1); } //No instance

            return (keyenceTcpClient[idx].Connect());
        }

        public void Disconnect(EKeyenceBcrPos ePos)
        {
            int idx = (int)ePos;

            if ((idx < 0) || (KEYENCE_BCR_COUNT <= idx))
            { return; } //Invalid Index

            if (keyenceTcpClient[idx] == null)
            { return; } //No instance

            keyenceTcpClient[idx].Disconnect();
        }

        private int _makeSendData(eKeyenceBcrCmd eCmd, ref string sCmd)
        {
            ///////////////////////////////////////////////////////////////////////////////////
            sCmd = STR_CMD_HEADER + this.sKeyenceBcrCmd[(int)eCmd] + STR_CMD_TAIL;
            ///////////////////////////////////////////////////////////////////////////////////

            return 0;
        }

        public int SendCommand(EKeyenceBcrPos ePos, eKeyenceBcrCmd eCmd)
        {
            int idx = (int)ePos;

            if ((idx < 0) || (KEYENCE_BCR_COUNT <= idx) || (eKeyenceBcrCmd.BCLR < eCmd))
            { return (-1); } //Invalid param

            if (keyenceTcpClient[idx] == null)
            { return (-1); } //No instance

            int ret = 0;

            if (!IsConnected(ePos))
            {
                if (0 != Connect(ePos))
                {
                    return CKeyenceTcpSocket.ERR_NOT_CONNECTED; //소켓 연결 안 된 상태
                }
            }

            // 1) 전송 command 구성
            string strCmd = "";
            _makeSendData(eCmd, ref strCmd);

            // 2) 로컬 수신버퍼 클리어
            keyenceTcpClient[idx].ClearReadBuffer();

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // 3) 통신 단계 설정
            CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.WAIT_CMD_RESP; //Command 송신 완료 후 Response 대기
            // 4) Command 문자열 저장 (Response 데이터 검증용)
            CData.KeyBcr[idx].sCmd = sKeyenceBcrCmd[(int)eCmd];
            CData.KeyBcr[idx].sCmdResult = "";
            CData.KeyBcr[idx].sErrCode = "";
            // 5) 바코드 판독 Command인 경우 기존 바코드 정보 초기화
            if (eCmd == eKeyenceBcrCmd.LON)
            {
                CData.KeyBcr[idx].sBcr = ""; 
            }
            // 6) 통신 Error 상태 초기화
            CData.KeyBcr[idx].bStErr = false;
            /////////////////////////////////////////////////////////////////////////////////////////////////

            // 7) Command 전송
            ret = keyenceTcpClient[idx].Send(strCmd);
            if (ret != CKeyenceTcpSocket.ERR_OK)
            {
                ClearKeyBcrData(ePos, eCmd); //전송 오류 => 관리 데이터 초기화
            }

            return ret;
        }

        /// <summary>
        /// 소켓 수신 데이터 처리
        /// </summary>
        /// <param name="idx">KEYENCE BCR 위치 인덱스 (OnLoader, OffLoader 구분)</param>
        /// <param name="recvBytes">수신 데이터 버퍼</param>
        /// <param name="recvSize">수신 데이터 길이</param>
        /// <returns></returns>
        public int ProcessReceivedData(int idx, byte [] recvBytes, int recvSize)
        {
            string sRcv = Encoding.ASCII.GetString(recvBytes, 0, recvSize);
            if(string.IsNullOrWhiteSpace(sRcv))
            {
                return (-2); //잘못된 데이터 형식 => 무시
            }

            return _parseAllData(idx, sRcv);
        }

        /// <summary>
        /// 소켓 수신 데이터 처리 (여러 개의 회신 데이터가 한번에 수신될 경우 개별 수신 데이터로 각각 분할하여 처리)
        /// </summary>
        /// <param name="idx">KEYENCE BCR 위치 인덱스 (OnLoader, OffLoader 구분)</param>
        /// <param name="data">수신 데이터(string)</param>
        /// <returns></returns>
        private int _parseAllData(int idx, string data)
        {
            if(string.IsNullOrWhiteSpace(data))
            {
                return (-2); //잘못된 데이터 형식 => 무시
            }

            int result = 0;

            string s = data; 
            
            //최소 데이터 길이 체크
            if(s.Length <= STR_CMD_TAIL.Length) //"\r" 1
            {
                return (-2); //잘못된 데이터 형식 => 무시
            }

            //데이터 끝("\r") 체크
            string s1 = s.Substring((s.Length - STR_CMD_TAIL.Length), STR_CMD_TAIL.Length);
            if(!(s1.Equals(STR_CMD_TAIL)))
            {
                return (-2); //잘못된 데이터 형식 => 무시
            }

            //여기까지 왔으면 parsing 준비 완료..................................

            //1) Tail("\r") 떼어내기
            s1 = s.Substring(0, (s.Length - STR_CMD_TAIL.Length));

            //2) 한꺼번에 들어온 여러 개의 회신 각각 분리하여 처리
            char[] delim = { CHAR_CMD_TAIL }; //'\r' //여러 개의 회신 데이터가 한번에 수신될 경우 분할하기 위한 구분자
            string[] fields = s1.Split(delim);
            int count = fields.Length;

            for (int i=0; i < count; i++)
            {
                //////////////////////////
                // 개별 회신 데이터 처리 //

                if (0 != this._parseEachData(idx, fields[i]))
                { result = (-2); }
            }
            return result;
        }

        /// <summary>
        /// 하나의 "\r"로 끝나는 개별 수신 데이터 처리
        /// </summary>
        /// <param name="idx">KEYENCE BCR 위치 인덱스 (OnLoader, OffLoader 구분)</param>
        /// <param name="data">수신 데이터(string)</param>
        /// <returns></returns>
        private int _parseEachData(int idx, string data)
        {
            if(string.IsNullOrWhiteSpace(data))
            {
                return (-2); //무효 데이터 무시
            }

            char[] delim = { CHAR_SEPARATOR }; //',' //하나의 회신 데이터 내에서의 구분자
            string s = data;
            string[] fields = s.Split(delim);
            int count = fields.Length;

            if (count == 1)
            {
                //////////////////////////////////
                // 에러, 바코드 둘 중 하나로 처리 //

                if (fields[0].Equals(STR_DATA_RESP_ERR))
                {
                    //////////////////
                    // "ERROR" 처리 //

                    if (CData.KeyBcr[idx].sCmd.Equals(sKeyenceBcrCmd[(int)eKeyenceBcrCmd.LOFF]))
                    {
                        if (CData.KeyBcr[idx].eState == eKeyenceBcrTcpStatus.WAIT_CMD_RESP)
                        {
                            /************************************************************************************
                             * CASE : "LOFF" command 전송 -> "ERROR" 수신 (Command Response보다 선행 수신)
                             * 
                             * BCR TCP Communication Step  : No Change
                             * Command                     : No Change
                             * Command Result              : No Change
                             * Error Code                  : No Change
                             * BCR Value                   : Clear ("")
                             * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sBcr = "";
                            return (0);
                        }
                        else if (CData.KeyBcr[idx].eState == eKeyenceBcrTcpStatus.WAIT_DATA_RESP)
                        {
                            /************************************************************************************
                             * CASE : "LOFF" command 전송 -> command response 수신 -> "ERROR" 수신
                             * 
                             * BCR TCP Communication Step  : DATA_COMPLETE
                             * Command                     : No Change
                             * Command Result              : No Change
                             * Error Code                  : No Change
                             * BCR Value                   : Clear ("")
                             * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sBcr = "";
                            CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.DATA_COMPLETE;
                            return (0);
                        }
                        else
                        {
                            return (-2); //그 외의 경우 수신 데이터 무시 ("ERROR" 수신할 시점이 아님)
                        }
                    }
                    else
                    {
                        return (-2); //그 외의 경우 수신 데이터 무시 ("ERROR" 수신할 시점이 아님)
                    }                                        
                }
                else
                {
                    //////////////////////////////////////
                    // "ERROR" 아님 => 바코드 값으로 처리 //

                    if (CData.KeyBcr[idx].sCmd.Equals(sKeyenceBcrCmd[(int)eKeyenceBcrCmd.LON]))
                    {
                        if (CData.KeyBcr[idx].eState == eKeyenceBcrTcpStatus.WAIT_CMD_RESP)
                        {
                            /************************************************************************************
                             * CASE : "LON" command 전송 -> 바코드 판독 데이터 수신 (Command Response보다 선행 수신)
                             * 
                             * BCR TCP Communication Step  : No Change
                             * Command                     : No Change
                             * Command Result              : No Change
                             * Error Code                  : No Change
                             * BCR Value                   : Received Data
                             * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sBcr = fields[0];
                            return (0);
                        }
                        else if (CData.KeyBcr[idx].eState == eKeyenceBcrTcpStatus.WAIT_DATA_RESP)
                        {
                            /************************************************************************************
                             * CASE : "LON" command 전송 -> command response 수신 -> 바코드 판독 데이터 수신
                             * 
                             * BCR TCP Communication Step  : DATA_COMPLETE
                             * Command                     : No Change
                             * Command Result              : No Change
                             * Error Code                  : No Change
                             * BCR Value                   : Received Data
                             * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sBcr = fields[0];
                            CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.DATA_COMPLETE;
                            return (0);
                        }
                        else
                        {
                            return (-2); //그 외의 경우 수신 데이터 무시 (바코드를 읽을 시점이 아님)
                        }
                    }
                    else
                    {
                        return (-2); //그 외의 경우 수신 데이터 무시 (바코드를 읽을 시점이 아님)
                    }
                }
            }
            else if (count == 2)
            {
                ////////////////////////////////////////////
                // Command 회신 OK : 아니라면 잘못된 데이터 //

                if (fields[0].Equals(STR_CMD_RESP_OK))
                {
                    if ((CData.KeyBcr[idx].eState == eKeyenceBcrTcpStatus.WAIT_CMD_RESP) && CData.KeyBcr[idx].sCmd.Equals(fields[1]))
                    {
                        if (CData.KeyBcr[idx].sCmd.Equals(sKeyenceBcrCmd[(int)eKeyenceBcrCmd.LON]))
                        {
                            if (CData.KeyBcr[idx].sBcr.Equals(""))
                            {
                                /************************************************************************************
                                 * CASE : "LON" command 전송 -> "OK,LON" 수신
                                 * 
                                 * BCR TCP Communication Step  : WAIT_DATA_RESP
                                 * Command                     : No Change
                                 * Command Result              : "OK"
                                 * Error Code                  : No Change
                                 * BCR Value                   : No Change
                                 * TCP Communication Err State : No Change
                                ************************************************************************************/
                                CData.KeyBcr[idx].sCmdResult = STR_CMD_RESP_OK;
                                CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.WAIT_DATA_RESP;
                                return (0);
                            }
                            else
                            {
                                /************************************************************************************
                                 * CASE : "LON" command 전송 -> 바코드 판독 데이터 수신 (선행 수신) -> "OK,LON" 수신
                                 * 
                                 * BCR TCP Communication Step  : DATA_COMPLETE
                                 * Command                     : No Change
                                 * Command Result              : "OK"
                                 * Error Code                  : No Change
                                 * BCR Value                   : No Change
                                 * TCP Communication Err State : No Change
                                ************************************************************************************/
                                CData.KeyBcr[idx].sCmdResult = STR_CMD_RESP_OK;
                                CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.DATA_COMPLETE;
                                return (0);
                            }
                        }
                        else if (CData.KeyBcr[idx].sCmd.Equals(sKeyenceBcrCmd[(int)eKeyenceBcrCmd.LOFF]))
                        {
                            /************************************************************************************
                                * CASE : "LOFF" command 전송 -> "OK,LOFF" 수신 : 이 때, "ERROR" 선행 수신 여부 무관
                                * 
                                * BCR TCP Communication Step  : DATA_COMPLETE
                                * Command                     : No Change
                                * Command Result              : "OK"
                                * Error Code                  : No Change
                                * BCR Value                   : No Change
                                * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sCmdResult = STR_CMD_RESP_OK;
                            CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.DATA_COMPLETE; //후행 수신 데이터 처리하지 않음
                            return (0);
                        }
                        else
                        {
                            return (-2); //나머지 Command ("BCLR")에 대한 Response는 처리하지 않아도 됨 => 무시
                        }
                    }
                    else
                    {
                        return (-2); //그 외의 경우 수신 데이터 무시 (Command Response를 수신할 시점이 아니거나, 전송한 Command와 다른 Command에 대한 회신 데이터)
                    }
                }
                else
                {
                    return (-2); //잘못된 데이터 형식 => 무시
                }
            }
            else if (count == 3)
            {
                ////////////////////////////////////////////
                // Command 회신 ER : 아니라면 잘못된 데이터 //

                if (fields[0].Equals(STR_CMD_RESP_ER))
                {
                    if ((CData.KeyBcr[idx].eState == eKeyenceBcrTcpStatus.WAIT_CMD_RESP) && CData.KeyBcr[idx].sCmd.Equals(fields[1]))
                    {
                        if (CData.KeyBcr[idx].sCmd.Equals(sKeyenceBcrCmd[(int)eKeyenceBcrCmd.LON]))
                        {
                            /************************************************************************************
                                * CASE : "LON" command 전송 -> "LON" command 전송 -> "ER,LON,ee" 수신
                                * 
                                * BCR TCP Communication Step  : No Change
                                * Command                     : No Change
                                * Command Result              : "ER"
                                * Error Code                  : "ee" <= 수신한 Error Code
                                * BCR Value                   : No Change
                                * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sCmdResult = STR_CMD_RESP_ER;
                            CData.KeyBcr[idx].sErrCode = fields[2];
                            return (0);
                        }
                        else if (CData.KeyBcr[idx].sCmd.Equals(sKeyenceBcrCmd[(int)eKeyenceBcrCmd.LOFF]))
                        {
                            /************************************************************************************
                                * CASE : "LOFF" command 전송 -> "ER,LOFF,ee" 수신
                                * 
                                * BCR TCP Communication Step  : DATA_COMPLETE
                                * Command                     : No Change
                                * Command Result              : "ER"
                                * Error Code                  : "ee" <= 수신한 Error Code
                                * BCR Value                   : No Change
                                * TCP Communication Err State : No Change
                            ************************************************************************************/
                            CData.KeyBcr[idx].sCmdResult = STR_CMD_RESP_ER;
                            CData.KeyBcr[idx].sErrCode = fields[2];
                            CData.KeyBcr[idx].eState = eKeyenceBcrTcpStatus.DATA_COMPLETE; //후행 수신 데이터 처리하지 않음
                            return (0);
                        }
                        else
                        {
                            return (-2); //나머지 Command ("BCLR")에 대한 에러 Response는 없음 => 처리하지 않아도 됨 => 무시
                        }
                    }
                    else
                    {
                        return (-2); //그 외의 경우 수신 데이터 무시 (Command Response를 수신할 시점이 아니거나, 전송한 Command와 다른 Command에 대한 회신 데이터)
                    }
                }
                else
                {
                    return (-2); //잘못된 데이터 형식 => 무시
                }
            }
            else
            {
                return (-2); //잘못된 데이터 형식 => 무시
            }
        }
    }
#endregion //CKeyenceBcr
}
