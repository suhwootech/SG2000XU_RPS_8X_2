using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace SG2000X
{
    /// <summary>
    /// Vision SW >> Main SW로 전송하는 주기적인 Report 또는 Command Response 데이터
    /// </summary>
    public struct tUdpRcvData
    {
        public string   sTime;          //Send Time             : 2D Vision SW에서 메시지 전송 시 할당
        public string   sReqId;         //Request ID            : 0=주기적인 Report, 그 외=Request ID (yyyyMMdd-HHmmss.fff 형식의 문자열)
        public int      iStatus;        //Status of Vision SW   : 0=Ready Mode, 1=Train Mode, 2=Auto Mode, 3=Running, 4=Alarm
        public string   sRecipe;        //Device Name           : 설비 Main SW에서 선택한 현재의 Device 명 (Recipe 동기화 용)
        public int      iOriResult;     //Orient Check Result   : 0=검사 전, 1=Pass, 2=Fail
        public int      i2DResult;      //2D Check Result       : 0=검사 전, 1=Pass, 2=Fail
        public string   s2DVal;         //2D Value              : 인식된 2D 코드 값
        public int      iOcrResult;     //OCR Check Result      : 0=검사 전, 1=Pass, 2=Fail
        public string   sOcrVal;        //OCR Value             : 인식된 ORC 값
        public int      iULevel;        //User Level            : 사용자 Level : 0=Operator, 1=Master
        public int      iUiMode;        //2D Vision SW UI Mode  : 0=Main Window, 1=Training Window
        public string   sAlarmMsg;      //Alarm Message         : iStatus 값이 4(Alarm)인 경우 해당 알람 메시지 //20200314 jhc : Vision SW Alarm Message
    }

    /// <summary>
    /// Main SW >> Vision SW로 전송하는 Command Request Data
    /// </summary>
    public struct tUdpSendData
    {
        public string   sReqID;         //Request ID            : 현재 시간 문자열 ("yyyyMMdd-HHmmss.fff")
        public int      iCmd;           //Command Number        : 1=Recipe 변경,  2=Auto모드 전환,  3=InRail Orient 검사,  4=L-Talbe Orient 검사,  5=R-Talble Orient 검사, 6=InRail BCR 검사, 7=L-Table BCR 검사, 8=R-Table BCR 검사, 99=Alive Check
        public string   sData;          //Data                  : CMD=1:Recipe Name, CMD=81:UserLevel(0/1/2/3), CMD=82:UI Mode(0/1)
    }

#if true //20200423 jhc : 수신데이터 콜백 처리
    public delegate int CallBackUpdateBcrData(tUdpRcvData rcvData);
#endif

    public class CUdpSocket : CStn<CUdpSocket>
    {
        #region UDP 통신 데이터 처리용
        public const byte   BYTE_LF         = 0x0A;         //전송 데이터의 끝의 byte 값 (LF = '\n' = 0x0A)
        public const char   CHAR_LF         = '\n';         //전송 데이터의 끝의 char 값 (LF = '\n' = 0x0A)
        public const string STR_LF          = "\n";         //전송 데이터의 끝의 string 값 (LF = '\n' = 0x0A)
        public const byte   BYTE_SEPARATOR  = 0x2C;         //송/수신 데이터 필드간 구분자의 BYTE 값
        public const char   CHAR_SEPARATOR  = ',';          //송/수신 데이터 필드간 구분자의 char 값
        public const string STR_SEPARATOR   = ",";          //송/수신 데이터 필드간 구분자의 string 값
        public const int    FIELD_COUNT     = 12;           //수신 데이터 필드 수 (헤드("SW:")/테일("\n") 제외) //20200314 jhc : 알람 메시지 추가 11 --> 12 

        public const string STR_BEGIN       = "SW:";        //전송 데이터의 시작
        public const string STR_END         = STR_LF;       //전송 테이터의 끝
        public const string SERVER_IP       = "127.0.0.1";  //UDP Socket Server IP = Local Host
        public const int    SERVER_PORT     = 2100;         //UDP Socket Server Port
        public const string CLIENT_IP       = SERVER_IP;    //UDP Socket Client IP = Local Host (설비 PC 내에 Vision SW 설치됨)
        public const int    CLIENT_PORT     = 3100;         //UDP Socket Client Port
        public const int    BUFF_SIZE       = (2 * 1024);   //수신 버퍼 사이즈 (Bytes)
        #endregion

        #region UDP 소켓 관리용
        public Socket _socket = null;   //UDP 서버 소켓
        public bool bStop = false;      //종료 플래그 : true > 추가 작업 중지, Socket Close해야 함
        public const int SIO_UDP_CONNRESET = -1744830452; //20200316 jhc : EndReceiveFrom() Exception 0x80004005

        private State state = new State();
#if false //현재 불필요
        private EndPoint epServer = null; //new IPEndPoint(IPAddress.Parse(SERVER_IP), SERVER_PORT);    //UDP Server Socket Bind 용
#endif
        private EndPoint epClient = new IPEndPoint(IPAddress.Parse(CLIENT_IP), CLIENT_PORT);    //UDP Server > Client Send 시 Client 지정 용
        private AsyncCallback recv = null;              //UDP Asynchronous Receive 용 Callback Function
        #endregion

#if true //20200423 jhc : 수신데이터 콜백 처리
        private CallBackUpdateBcrData CB_UpdateBcrData;
		
        public CUdpSocket(CallBackUpdateBcrData cb)
        {
            CB_UpdateBcrData = cb;
        }
#endif

        public class State
        {
            public byte[] buffer = new byte[BUFF_SIZE]; //수신 데이터 버퍼
        }

        /// <summary>
        /// UDP Server Socket 생성
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public int Server(string address=SERVER_IP, int port=SERVER_PORT)
        {
            if(string.IsNullOrWhiteSpace(address))
            {
                Log("Error : Invalid UDP Server Socket Address [" + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return(-1);
            }
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null); //20200316 jhc : EndReceiveFrom() Exception 0x80004005
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            int ret = BeginReceive(); //Begin Receive as a Server
            if( ret == 0 )
            {
                Log("UDP Server Opened [" + address + ":" + port.ToString() + "]");
                return (0);
            }
            else
            {
                Log("Error : UDP Server Open Fail");
                Close();
                return (-1);
            }
        }

        /// <summary>
        /// 정지 플래그 설정 후 소켓 close
        /// 반드시 프로그램 종료 시 1회만 실행하기
        /// </summary>
        public void Close()
        {
            bStop = true;
            Thread.Sleep(10);
            _socket.Close();
            Log("UDP Server Close");
        }

       /// <summary>
        /// 서버 > 클라이언트 ASCII 데이터 전송 (Blocking)
        /// </summary>
        /// <param name="text"></param>
        public int SendSync(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                Log("WARNING : Message Empty [" + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return (-1);
            }
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(text);
                int sndBytes = _socket.SendTo(data, epClient);
                if( sndBytes > 0 )
                {
                    //송신 데이터 로그
                    string strDisplay = text.Replace(STR_LF, "\\n"); //LF(0x0A) -> "\n", 표시 목적
                    Log(string.Format("SEND: {0}, {1}", sndBytes, strDisplay));
                    return (0);
                }
                else
                {
                    Log(string.Format("SEND: {0} : None", sndBytes));
                    return (-1);
                }
            }
            catch( Exception e )
            {
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return (-1);
            }
        }

        /// <summary>
        /// 클라이언트 > 서버 ASCII 데이터 전송 (Asynchronous, to Connected Socket)
        /// Connect 또는 Accept 선 호출 후에만 사용 가능
        /// Server 소켓은 그냥 Send를 사용하자!!!
        /// </summary>
        /// <param name="text"></param>
        public int SendAsync(string text)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(text);
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int sndBytes = _socket.EndSend(ar);
                    Log(string.Format("SEND: {0}, {1}", sndBytes, text));
                }, state);
                return (0);
            }
            catch( Exception e )
            {
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                return (-1);
            }
        }

        /// <summary>
        /// 데이터 수신 (Async)
        /// </summary>
        /// <param name="option"> 0:Server / 1:Client </param>
        /// <returns> 0:Success / !0:Fail </returns>
        private int BeginReceive()
        {  
            int result = 0;

            try
            {           
                _socket.BeginReceiveFrom(state.buffer, 0, BUFF_SIZE, SocketFlags.None, ref epClient, recv = (ar) =>
                {
                    if( !bStop )
                    {
                        State so = (State)ar.AsyncState;
                        int rcvBytes = 0;
                        try
                        {
                            rcvBytes = _socket.EndReceiveFrom(ar, ref epClient);
                        }
                        catch (Exception e)
                        {
                            Log(string.Format("EXCEPTION : EndReceiveFrom : {0}", e.ToString()));
                        }
                        if (rcvBytes > 0)
                        {
                            ProcessReceivedData(rcvBytes, Encoding.ASCII.GetString(so.buffer, 0, rcvBytes)); //수신 데이터 처리
                        }
                        else
                        {
                            Log(string.Format("RECV: {0}: {1} NONE", epClient.ToString(), rcvBytes));
                        }
                        try
                        {
                            EndPoint epRemote = new IPEndPoint(IPAddress.Parse(CLIENT_IP), CLIENT_PORT); //20200316 jhc : EndReceiveFrom() Exception 0x80004005
                            _socket.BeginReceiveFrom(so.buffer, 0, BUFF_SIZE, SocketFlags.None, ref epClient, recv, so);
                        }
                        catch (Exception e)
                        {
                            Log(String.Format("EXCEPTION : BeginReceiveFrom() : {0}", e.ToString()));
                        }
                    }
                    else
                    {
                        Log("RECV: Skip < STOP state");
                    }
                }, state);
            }
            catch (Exception e)
            {
                Log(string.Format("EXCEPTION : {0} [", e.ToString()) + MethodBase.GetCurrentMethod().Name.ToString() + "]");
                result = (-1);
            }
            return result;
        }

        /// <summary>
        /// UDP 수신 데이터 처리
        ///   1) 로그
        ///   2) 파싱
        ///   3) 내부 상태 변수 설정 (조건별 처리)
        /// </summary>
        /// <param name="len">수신 데이터 사이즈(Bytes)</param>
        /// <param name="data">수신 데이터 문자열</param>
        private void ProcessReceivedData(int len, string data)
        {
            //1) 수신 데이터 로그
            string strDisplay = data.Replace(STR_LF, "\\n"); //LF(0x0A) -> "\n", 표시 목적
            Log(string.Format("RECV: {0}: {1}, {2}", epClient.ToString(), len.ToString(), strDisplay));
            //2) 수신 데이터 파싱
            tUdpRcvData stRcvData = new tUdpRcvData();
            if(ParseReceivedData(data, ref stRcvData) == 0 )
            {
                //3) 내부 상태 변수 설정
#if true //20200423 jhc : 수신데이터 콜백 처리
                CB_UpdateBcrData(stRcvData); //tBcr CData.Brc 구조체 정보 업데이트
#else
                CBcr.It.UdpUpdateBcrData(stRcvData); //tBcr CData.Brc 구조체 정보 업데이트
#endif
            }

            //DEL: 확인용, 수신 데이터 Client에 전송 : 삭제 예정
            //SendSync(data);
            //
        }

        #region Parsing
        private bool GetStrVal(string inStr, int length, ref string outStr)
        {
            bool bResult = false;
            if(!(String.IsNullOrWhiteSpace(inStr)))
            {
                if((length > 0) && (inStr.Length != length))
                {
                    outStr = ""; //Data Size Missmatch
                    bResult = false;
                }
                else
                {
                    outStr = inStr; //정상 문자열
                    bResult = true;
                }
            }
            else
            {
                outStr = ""; //null or whitespace
                bResult = false;
            }
            return bResult;
        }
        private bool GetIntVal(string inStr, int min, int max, ref int outInt)
        {
            bool bResult = false;
            int output = 0;
            if(int.TryParse(inStr, out output))
            {
                outInt = output; //숫자로 변경 가능 문자열
                if((min <= output) && (output <= max))  bResult = true;  //허용 범위 내의 숫자 OK
                else                                    bResult = false; //허용 범위 밖의 숫자 NG
            }
            else
            {
                outInt = -1; //숫자로 변경 불가 문자열
                bResult = false;
            }
            return bResult;
        }
        private int ParseReceivedData(string data, ref tUdpRcvData stRcvData)
        {
            //빈 데이터는 버림
            if(string.IsNullOrEmpty(data))
            {
                Log("IGNORE: Empty Data");
                return (-1);
            }

            //최소 길이 이하의 데이터는 버림
            int dataLen = data.Length; //입력 인자 len 값과 dataLen 값이 동일해야 함
            int startLen = STR_BEGIN.Length;
            int endLen = STR_END.Length;
            int minLength = startLen + endLen;
            if(dataLen < minLength)
            {
                Log(string.Format("IGNORE: Data length too small [{0}) < {1}]", dataLen, minLength));
                return (-1);
            }

            //데이터 시작 체크
            string sTmp = data.Substring(0,startLen);
            if(!(sTmp.Equals(STR_BEGIN)))
            {
                Log(string.Format("IGNORE: Invalid Start Data [\"{0}\":\"{1}\"]", STR_BEGIN, sTmp));
                return (-1);
            }

            //데이터 끝 체크
            sTmp = data.Substring(dataLen-1,1);
            if(!(sTmp.Equals(STR_END)))
            {
                //끝 데이터 "\n" > Hexa String으로 표시하도록 구현  // strBuf.Replace( STR_LF,  "\\n" ); //LF(0x0A) -> "\n", 표시 목적
                Log(string.Format("IGNORE: Invalid End Data [\"{0}\":\"{1}\"]", STR_END, sTmp));
                return (-1);
            }

            //여기까지 왔으면 parsing 준비 완료..................................

            //1) Head("SW:"), Tail("\n") 떼어내기
            sTmp = data.Substring(startLen,(dataLen-minLength));
            
            //2) 각 필드 값 추출 (간단한 검증 포함)
            int nNGCount = 0;
            char[] delim = { CHAR_SEPARATOR };
            string[] fields = sTmp.Split(delim);
            int iFieldCount = fields.Length;

            if(iFieldCount == FIELD_COUNT)
            {
                if(!(this.GetStrVal(fields[0], 0, ref stRcvData.sTime)))            nNGCount++; //Time 필수
                if(!(this.GetStrVal(fields[1], 0, ref stRcvData.sReqId)))           nNGCount++; //Request ID 필수         : 0 또는 yyyyMMdd-HHmmss.fff
                if(!(this.GetIntVal(fields[2], 0, 4, ref stRcvData.iStatus)))       nNGCount++; //Statue 필수             : 0=Ready / 1=Train / 2=Auto / 3=Running / 4=Alarm //20200314 jhc : Alarm 추가됨
                if(!(this.GetStrVal(fields[3], 0, ref stRcvData.sRecipe)))          nNGCount++; //Recipe 필수
                if(!(this.GetIntVal(fields[4], 0, 2, ref stRcvData.iOriResult)))    nNGCount++; //Orientation Result 필수 : 0=검사전 / 1=Pass / 2=Fail
                if(!(this.GetIntVal(fields[5], 0, 2, ref stRcvData.i2DResult)))     nNGCount++; //2D Result 필수          : 0=검사전 / 1=Pass / 2=Fail
                stRcvData.s2DVal    = fields[6].Trim();                                         //2D Value 필수 아님 ("" 가능)
                if(!(this.GetIntVal(fields[7], 0, 2, ref stRcvData.iOcrResult)))    nNGCount++; //OCR Result 필수         : 0=검사전 / 1=Pass / 2=Fail
                stRcvData.sOcrVal   = fields[8].Trim();                                         //OCR Value 필수 아님 ("" 가능)
                if(!(this.GetIntVal(fields[9], 0, 3, ref stRcvData.iULevel)))       nNGCount++; //User Level 필수         : 0=Operator / 1=Technician / 2=Engineer / 3=Master
                if(!(this.GetIntVal(fields[10], 0, 1, ref stRcvData.iUiMode)))      nNGCount++; //2D Vision UI Mode       : 0=Main Window / 1=Training Window
                stRcvData.sAlarmMsg = fields[11].Trim();                                        //Alarm 번호 필수 아님 ("" 가능) //20200314 jhc : Alarm 추가됨
            }
            else
            {
                //수신 데이터 필드 수량 불량
                Log(string.Format("IGNORE: Invalid Field Count [{0}/{1}]", fields.Length, FIELD_COUNT));
                return (-1);
            }

            if(nNGCount > 0)
            {
                //수신 데이터 개별 필드 불량
                Log(string.Format("IGNORE: Invalid Field Data [{0}/{1}]", nNGCount, FIELD_COUNT));
                return (-1);
            }
            
            return 0;
        }
        #endregion

        public int MakeSendData(tUdpSendData stSndData, ref string data)
        {
            string strReqId = stSndData.sReqID.Trim();
            string strCmd = stSndData.iCmd.ToString();
            string strData = stSndData.sData.Trim();

            if((String.IsNullOrWhiteSpace(strData)) && ((stSndData.iCmd == (int)(eBcrCommand.ChangeRecipe)) || (stSndData.iCmd == (int)(eBcrCommand.ChangeUserLevel)) || (stSndData.iCmd == (int)(eBcrCommand.ChangeUI))))
            {
                //Invalid Command
                Log("ERROR : Invalid Command Format [CMD=" + stSndData.iCmd.ToString() + ", DATA=" + strData + "]");
                data = "";
                return (-1);
            }

            ///////////////////////////////////////////////////////////////////////////////////
            data = STR_BEGIN/*"SW:"*/ + strReqId + "," + strCmd + "," + strData + STR_END;
            ///////////////////////////////////////////////////////////////////////////////////

            return 0;
        }

        #region CUdpSocket 로그
        /// <summary>
        /// CUdpSocket 클래스 이벤트 로그 (송/수신 데이터 포함)
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
        /// CUdpSocket 클래스 이벤트 로그 (송/수신 데이터 포함)
        /// YYYY\MM\DD\UDP\YYYYMMDDHH.txt
        /// </summary>
        private void SaveLog(string msg)
        {
            DateTime dtNow = DateTime.Now;
            string sPath = GV.PATH_LOG + dtNow.Year.ToString("0000") + "\\" +  dtNow.Month.ToString("00") + "\\" + dtNow.Day.ToString("00") + "\\UDP\\";
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
}
