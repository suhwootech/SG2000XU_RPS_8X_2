using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms; // Timer 때문에 추가함.


/*
 /// <summary>
        
//Delta C2000 Spindle Driver Parmeter 설정
  Parameter No.   주석                         설정값  Default값  참고
  00-20         마스터 주파수 명령 소스(자동)  1       0          읽기 전용 RS-485시리얼통신제어시  설정 되어야 함.
  00-21         운전명령 소스(자동)            2       0          읽기 전용 RS-485시리얼통신제어시  설정 되어야 함.
  02-12         디지털 입력 운전 방향          4       0          MI4 다단계 속도 명령 1로 설정 되어야 함. 그러지 않으면 운전 주파수 설정시 오류 발생됨. 2023.03.08 확인
  09-00         COM1 통신 주소                 1       1          (Grinder 1 = 1 / Grindler 2 = 2 ---> 485 Multi Drop 연결시)
  09-01         COM1 전송 속도                 115.2   9.6
  09-02         COM1 전송 오류 처리            0       3           0: 경고하고 계속 가동
                                                                   1: 경고하고 RAMP 정지
                                                                   2: 경고하고 COAST 정지
                                                                   3: 경고 없고 계속 가동
  09-03         COM1 시간초과 감지             0.0     0.0         0.0 : 사용 안함 / 0.0~ 100.0sec
  09-04         COM1 통신 프로토콜             1       6           Ascii의 7,N,2  ( TApdComPort 설정과 같이 한다)
    통신 프로토콜구성
            STX         ":" (0x3Ah)
            상위주소    8bit 2-Ascii
            하위주소    8bit 2-Ascii
            기능상위    8bit 2-Ascii
            기능하위    8bit 2-Ascii
            데이터(N-1)
            ''''        N * 8-bit 데이터 2n Ascii로 n<=16, n>=32 Ascii 코드
            데이터0
            LRC_CHK_Hi  LRC 확인 총합8bit 2 Ascii
            LRC_CHK_Lo
            종료상위    END1 = CR (0x0Dh)
            종료하위    END0 = LF (0x0Ah)


    ex-명령) 읽기기능 "03" (16진수아스키코드로 표현)
     STX   주소    기능     시작주소    데이터의 수(워드로 카운트됨)    LRC확인   종료
      :    01      03       2102        0002                            D7        CRLF

    ex-응답)               (16진수아스키코드로 표현)
      STX   주소    기능    데이터의 수(바이트로 카운트됨)    시작주소2102H의 내용   주소2103H의 내용   LRC확인   종료
       :    01      03      04                                1770                   0000               71        CRLF



    ex-명령) 쓰기기능 "06"  (16진수아스키코드로 표현)
      STX   주소    기능    데이터주소     데이터 내용  LRC확인 종료
       :    01      06      0100           1770         71      CRLF
    ex-응답)               (16진수아스키코드로 표현)
      STX   주소    기능    데이터주소     데이터 내용  LRC확인 종료
       :    01      06      0100           1770         71      CRLF

// 10H 여러 개의 레지스터를 기록 (레지스터에 여러 개의 데이터를 기록- 다단 속도제어)
    Pr.04-00 = 50.00 (1388h), Pr.04-01 = 40.00 (0FA0h), AC드라이브의 주소는 01h 입니다.
    ex-명령) 쓰기기능 "06"  (16진수아스키코드로 표현)
      STX   주소  기능   시작데이터주소  데이터의 수(워드로카운트)  데이터의 수(바이트로 카운트)  첫번째 데이터내용  두번째 데이터 내용  LRC확인 종료
       :    01    10     0500            0002                       04                            1388               0FA0                9A      CRLF
    ex-응답)               (16진수아스키코드로 표현)
      STX   주소  기능   시작데이터주소  데이터의 수(워드로 카운트)  LRC확인 종료
       :    01    10     0500            0002                        E8      CRLF


// Check Sum
     ASCII 모드
     LRC (세로 중복 검사)는 총합을 구해서 계산됩니다, 모듈 256, ADR1 으로 부터의 바이트
     값에서 마지막 데이터 특징 그리고 총합의 2 의 보수 십육진수를 나타낸 것을 계산.
     예를 들어,
     01H+03H+21H+02H+00H+02H=29H, 29H 의 두번째 보완부정은 D7H 입니다.

// 모터 동작 방법
    1. 2001h(주파수 명령)번지에 회전하고자 하는 속도(주파수)를 쓴다.
       Max 4000 RPM이고 60hz X 30 = 1800RPM (주파수에 30을 곱하면 RPM이다)
    2. 2000h(모터동작관련)번지에Bit 0~3번의값을 변경 (0: 기능없음, 1:정지, 2:운전동, 3: Jog+ 운전
                                Bit 4~5 설정에 따라 운전방향 변경
                                    00b  : 기능없음
                                    01b  : 정회전
                                    10b  : 역회전
                                    11b  : 방향전환


// Wheel Tip & Toolsetter 3point 측정 방법
    1. 사전제공 parameter 입력 (스핀들 파라메타로 미리 설정 되어 있어야 함)
    2. Relay (I/O Output 설정  On) 활성화
    3. 11-66 레지스터에 회전하고자 하는 펄스값 입력
       ex) 11-66 파라메터를 이용하여 원점 위치를 0->1000 으로 변경하는 방법
           01H(주소)+06H(명령)+0BH(11)+42H(66)+03E8H(1000)
    4. RUN 명령 전송
       휠 회전중 위치값 도달하면 정지함
    5. 휠 정지 확인
    6. 휠 측정
    7. 3번 ~ 6번 항목 반복 3point 측정 완료
    8. STOP 명령 전송
    9. Relay (I/O Output 설정 Off) 비 활성화
    10. 완료
/// </summary>
 */

namespace SG2000X
{
    public struct TSpin_485
    {
        public int Add2100h;  // 오류코드: Pr.06-17에서 Pr.06-22를 따름
        public int Add2102h;  // 주파수 명령(F)
        public int Add2103h;  // 출력 주파수(H)
        public int Add2116h;  // 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
        public int Add2119h;  // Bit  신호
        public int Add2200h;  // 출력 전류을 표시(A)
        public int Add2202h;  // 실제 출력 주파수를 표시(H)
        public int Add2207h; // 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
        public int Add2208h; // 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
        public int Add2209h; // PG 피드백을 표시(NOTE 1처럼)
        public int Add220Eh; // 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
        public int Add220Fh; // 캐패시턴스의 온도를 'C로 나타냄(i.)
        public int Add2215h; // 실제 모터의 회전 수(PG카드의 PG1)(P.) 실제 운전 방향이 바뀌거나 키패드가 멈춤에서 0을 나타내며 9에서 시작함. 최대는 65535(P.)
        public int Add2217h; // 펄스 입력 위치(PG카드의 PG2)(4.)
        public int Add221Fh; // Pr.00-05의 출력 값
    }
       

    public class CSpl_485 : CStn<CSpl_485>
    {
        /// <summary>
        /// Spindle 통신 Seq Step 초기화 및 통신용 타이머 선언
        /// </summary>
        public int Spl1_SeqStep = -1;
        public int Spl2_SeqStep = -1;
        private Timer tmrSpl1_Seq;
        private Timer tmrSpl2_Seq;

        /// <summary>
        /// Spindle 통신 포트 선언 및 통신 메세지 변수 선언
        /// </summary>
        private SerialPort m_RS485_Spl1;
        public string m_Spl1_ReciveData;
        private SerialPort m_RS485_Spl2;
        public string m_Spl2_ReciveData;

        /// <summary>
        /// TSpin_485 Class에서 생성된 Spindle Drv 정보를 저장하는 구조체 선언
        /// </summary>
        public TSpin_485 Spl1_Data;        
        public TSpin_485 Spl2_Data;

        /// <summary>
        // Sequece에서 Spindle Drv에게 명령을 보내기 위한 예약 변수
        /// </summary>
        private int nSpl1_SendReservsion, nSpl2_SendReservsion; // 0 : 보낼 Cmd 없다. 1: RUN, 2: STOP, 3: Fwd/Bwd, 4:Reset, 5: EF Stop, 6 : RPM, 7 : Wheel 3Point 측정;

        /// <summary>
        /// 송신후 수신값 입력 완료 Flag
        /// </summary>
        private bool bSpl1_ReciveEnd, bSpl2_ReciveEnd; // false : 초기값, true : 수신 완료

        /// <summary>
        /// Spindle Drv와 송수신을 위한 Sequence Timer 설정
        /// </summary>
        private CTim m_Timeout_Spindle1 = new CTim(); 
        private CTim m_Timeout_Spindle2 = new CTim(); 

        public string[] CommList;

        /// <summary>
        /// Spindle Drv 송수신 할 명령어 리스트
        /// </summary>
        private string[] SpindleCmdAddress =
        {
            "2100H", //오류코드: Pr.06-17에서 Pr.06-22를 따름
            //"2102H", //주파수 명령(F)
            //"2103H", //출력 주파수(H)
            "2116H", //다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
            //"2119H", //Bit  신호
            "2200H", //출력 전류을 표시(A)
            //"2202H", //실제 출력 주파수를 표시(H)
            "2207H", //드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
            "2208H", //드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
            //"2209H", //PG 피드백을 표시(NOTE 1처럼)
            //"220EH", //드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
            //"220FH", //캐패시턴스의 온도를 'C로 나타냄(i.)
            //"2215H", //실제 모터의 회전 수(PG카드의 PG1)(P.) 실제 운전 방향이 바뀌거나 키패드가 멈춤에서 0을 나타내며 9에서 시작함. 최대는 65535(P.)
            //"2217H", //펄스 입력 위치(PG카드의 PG2)(4.)
            //"221FH" //Pr.00-05의 출력 값
        };

        /// <summary>
        /// Spindle Drv 설정 값을 읽어오는 명령어를 순차적으로 송수신 하기 위한 변수
        /// </summary>
        private int Spindle1CmdAddresCnt, Spindle2CmdAddresCnt;
        private int SpindleCmdAddresMaxCnt;

        /// <summary>
        // Spindle Drv와 통신시 에러가 발생 되었는지 확인 하는 Flag 및 에러 메세지
        /// </summary>
        private int Spl1ErrorCode, Spl2ErrorCode;
        private string Spl1ErrorMsg, Spl2ErrorMsg;
        public bool GetErrorState_Spl1, GetErrorState_Spl2;


        /// <summary>
        // Sequece에서 Spindle Drv에게 명령을 보낼시 각 명령에 대한 송신 수신 Flag
        /// </summary>
        private bool m_CmdSendOK_Spl1, m_CmdSendOK_Spl2;
        private bool m_bRunCmdOk_Spl1, m_bRunCmdOk_Spl2;
        private bool m_bStopCmdOk_Spl1, m_bStopCmdOk_Spl2;
        private bool m_bFwdRevCmdOk_Spl1, m_bFwdRevCmdOk_Spl2;
        private bool m_bResetCmdOk_Spl1, m_bResetCmdOk_Spl2;
        private bool m_bEF_StopCmdOk_Spl1, m_bEF_StopCmdOk_Spl2;
        private bool m_bRPMCmdOk_Spl1, m_bRPMCmdOk_Spl2;
        private int m_nSetRPM_Spl1,m_nSetRPM_Spl2;
        private bool m_bWheelChkCmdOk_Spl1, m_bWheelChkCmdOk_Spl2;
        private int m_nSetWheelChkPnt_Spl1, m_nSetWheelChkPnt_Spl2;


        private CSpl_485()
        {
            //
        }

        // Delta C2000+ 드라이버 통신 및 변수 초기화
        public void _Initial()
        {
            /// <summary>
            /// PC COMM Port List 확보 : 사용 안함.
            /// </summary>
            CommList = SerialPort.GetPortNames();

            /// <summary>
            /// Spindle Drv와 송수신을 위한 Sequence Timer 설정
            /// </summary>
            m_RS485_Spl1 = new SerialPort();           
            m_RS485_Spl2 = new SerialPort();

            /// <summary>
            /// Spindle 1 Drv 통신시 상태값 Reading Timer 설정
            /// </summary>
            Spl1_SeqStep = -1;
            tmrSpl1_Seq = new System.Windows.Forms.Timer();
            tmrSpl1_Seq.Interval = 5;
            tmrSpl1_Seq.Tick += new EventHandler(tmrSpl1_Seq_Tick);


            /// <summary>
            /// Spindle 2 Drv 통신시 상태값 Reading Timer 설정
            /// </summary>
            Spl2_SeqStep = -1;
            tmrSpl2_Seq = new System.Windows.Forms.Timer();
            tmrSpl2_Seq.Interval = 5;
            tmrSpl2_Seq.Tick += new EventHandler(tmrSpl2_Seq_Tick);

            /// <summary>
            /// 송신후 수신값 입력 완료 Flag
            /// </summary>
            bSpl1_ReciveEnd = false;
            bSpl2_ReciveEnd = false;

            /// <summary>
            /// Spindle Drv 설정 값을 읽어오는 명령어를 순차적으로 송수신 하기 위한 변수
            /// </summary>
            Spindle1CmdAddresCnt = 0;
            Spindle2CmdAddresCnt = 0;
            SpindleCmdAddresMaxCnt = SpindleCmdAddress.Length;

            /// <summary>
            /// Sequece에서 Spindle Drv에게 명령을 보내기 위한 예약 변수
            /// </summary>
            nSpl1_SendReservsion = 0;
            nSpl2_SendReservsion = 0;

            /// <summary>
            /// Sequece에서 Spindle Drv에게 명령을 보내면 true, 수신 완료되면 False로 변경됨
            /// </summary>
            m_CmdSendOK_Spl1 = false;
            m_CmdSendOK_Spl2 = false;

            /// <summary>
            /// Sequece에서 Spindle Drv에게 명령을 보낼시 각 명령에 대한 송신 수신 Flag
            /// </summary>
            m_bRunCmdOk_Spl1 = false;
            m_bRunCmdOk_Spl2 = false;

            m_bStopCmdOk_Spl1 = false;
            m_bStopCmdOk_Spl2 = false;
            
            m_bFwdRevCmdOk_Spl1 = false;
            m_bFwdRevCmdOk_Spl2 = false;
            
            m_bResetCmdOk_Spl1 = false;
            m_bResetCmdOk_Spl2 = false;
            
            m_bEF_StopCmdOk_Spl1 = false;
            m_bEF_StopCmdOk_Spl2 = false;
            
            m_bRPMCmdOk_Spl1 = false;
            m_bRPMCmdOk_Spl2 = false;
            m_nSetRPM_Spl1 = 0;
            m_nSetRPM_Spl2 = 0;

            m_bWheelChkCmdOk_Spl1 = false;
            m_bWheelChkCmdOk_Spl2 = false;
            m_nSetWheelChkPnt_Spl1 = 0;
            m_nSetWheelChkPnt_Spl2 = 0;


            /// <summary>
            /// Spindle Drv와 통신시 에러가 발생 되었는지 확인 하는 Flag 및 에러 메세지
            /// </summary>
            Spl1ErrorCode = 0;
            Spl1ErrorMsg = "";
            GetErrorState_Spl1 = false;

            Spl2ErrorCode = 0;
            Spl2ErrorMsg = "";            
            GetErrorState_Spl2 = false;            
        }

        /// <summary>
        /// Spindle Drv와 송수신 Sequence Timer 
        /// </summary>
        private void StopTimer()
        {
            tmrSpl1_Seq.Enabled = false;
            tmrSpl2_Seq.Enabled = false;
        }


        /// <summary>
        /// Spindle Drv와 송수신 Sequence Timer 
        /// </summary>
        private void ResumeTimer()
        {
            tmrSpl1_Seq.Enabled = true;
            tmrSpl2_Seq.Enabled = true;
        }


        /// <summary>
        /// Spindle Drv와 송수신 위한 초기화 이후 통신 포트 오픈 상태 확인
        /// </summary>
        public bool isCommOpen(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay) bRet = m_RS485_Spl1.IsOpen;
            else if (EWay.R == eWay) bRet = m_RS485_Spl2.IsOpen;
            else bRet = false;

            return bRet;
        }


        /// <summary>
        /// Spindle Drv와  통신 포트 CLOSE 상태 확인
        /// </summary>
        public void CommClose(EWay eWay)
        {
            if (EWay.L == eWay) m_RS485_Spl1.Close();
            if (EWay.R == eWay) m_RS485_Spl2.Close();
        }


        /// <summary>
        /// Spindle Drv 통신 포트 인스턴스 제거
        /// </summary>
        public void Release(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                if (m_RS485_Spl1 != null)
                {
                    if (m_RS485_Spl1.IsOpen) CommClose(EWay.L);
                    m_RS485_Spl1.Dispose();
                    m_RS485_Spl1 = null;
                }

                if (tmrSpl1_Seq != null)
                {
                    tmrSpl1_Seq.Dispose();
                    tmrSpl1_Seq = null;
                }
            }

            if (EWay.R == eWay)
            {
                if (m_RS485_Spl2 != null)
                {
                    if (m_RS485_Spl2.IsOpen) CommClose(EWay.R);
                    m_RS485_Spl2.Dispose();
                    m_RS485_Spl2 = null;
                }

                if (tmrSpl2_Seq != null)
                {
                    tmrSpl2_Seq.Dispose();
                    tmrSpl2_Seq = null;
                }
            }            
        }


        /// <summary>
        /// Spindle 1 Drv 통신 포트 설정 및 변수 초기화
        /// </summary>
        //public bool InitialComm_Spl1(string CommNum)
        public int InitialComm_Spl1()
        {
            int iRet = 0;

            //m_RS485_Spl1.PortName = CommNum;
            m_RS485_Spl1.PortName = "COM7";
            m_RS485_Spl1.DiscardNull = false; // NULL 바이트 무시 설정
            m_RS485_Spl1.DataBits = 7;
            m_RS485_Spl1.StopBits = StopBits.Two;
            m_RS485_Spl1.Parity = Parity.None;
            m_RS485_Spl1.BaudRate = 115200;
            m_RS485_Spl1.NewLine = "\r\n";
            m_RS485_Spl1.DataReceived += M_mSerial_DataReceived_Spl1; // 이벤트 함수를 대입 하는 것임 += 은 이벤트를 더하는 것이며, 복수개도 가능, C++의 대입(=) 은 사용하는 것이 아님
            m_RS485_Spl1.Open();

            if (m_RS485_Spl1.IsOpen)
            {
                /// <summary>
                /// Spindle Drv 설정 값을 읽어오는 명령어를 순차적으로 송수신 하기 위한 변수
                /// </summary>
                Spindle1CmdAddresCnt = 0;
                SpindleCmdAddresMaxCnt = SpindleCmdAddress.Length;

                /// <summary>
                /// Sequece에서 Spindle Drv에게 명령을 보내기 위한 예약 변수
                /// </summary>
                nSpl1_SendReservsion = 0;

                /// <summary>
                /// Spindle Drv와 통신시 에러가 발생 되었는지 확인 하는 Flag 및 에러 메세지
                /// </summary>
                Spl1ErrorCode = 0;
                Spl1ErrorMsg = "";
                GetErrorState_Spl1 = false;

                /// <summary>
                /// Sequece에서 Spindle Drv에게 명령을 보낼시 각 명령에 대한 송신 수신 Flag
                /// </summary>
                m_bRunCmdOk_Spl1 = false;
                m_bStopCmdOk_Spl1 = false;
                m_bFwdRevCmdOk_Spl1 = false;
                m_bResetCmdOk_Spl1 = false;
                m_bEF_StopCmdOk_Spl1 = false;
                
                m_bRPMCmdOk_Spl1 = false;
                m_nSetRPM_Spl1 = 0;

                m_bWheelChkCmdOk_Spl1 = false;
                m_nSetWheelChkPnt_Spl1 = 0;

                m_CmdSendOK_Spl1 = false;

                /// <summary>
                /// Spindle 통신 Seq Step 초기화 및 수신 완료 변수 초기화
                /// </summary>
                Spl1_SeqStep = 0;
                bSpl1_ReciveEnd = false;

                /// <summary>
                /// Spindle Drv 통신을 위한 Timer 동작
                /// </summary>
                tmrSpl1_Seq.Enabled = true;
            }

            iRet = (m_RS485_Spl1.IsOpen) ? 0 : 1;

            return iRet;
        }

        //public bool InitialComm_Spl2(string CommNum)
        public int InitialComm_Spl2()
        {
            int iRet = 0;

            //m_RS485_Spl2.PortName = CommNum;
            m_RS485_Spl2.PortName = "COM8";
            m_RS485_Spl2.DiscardNull = false; // NULL 바이트 무시 설정
            m_RS485_Spl2.DataBits = 7;
            m_RS485_Spl2.StopBits = StopBits.Two;
            m_RS485_Spl2.Parity = Parity.None;
            m_RS485_Spl2.BaudRate = 115200;
            m_RS485_Spl2.NewLine = "\r\n";
            m_RS485_Spl2.DataReceived += M_mSerial_DataReceived_Spl2; // 이벤트 함수를 대입 하는 것임 += 은 이벤트를 더하는 것이며, 복수개도 가능, C++의 대입(=) 은 사용하는 것이 아님
            m_RS485_Spl2.Open();

            if (m_RS485_Spl2.IsOpen)
            {
                /// <summary>
                /// Spindle Drv 설정 값을 읽어오는 명령어를 순차적으로 송수신 하기 위한 변수
                /// </summary>
                Spindle2CmdAddresCnt = 0;
                SpindleCmdAddresMaxCnt = SpindleCmdAddress.Length;

                /// <summary>
                /// Sequece에서 Spindle Drv에게 명령을 보내기 위한 예약 변수
                /// </summary>
                nSpl2_SendReservsion = 0;

                /// <summary>
                /// Spindle Drv와 통신시 에러가 발생 되었는지 확인 하는 Flag 및 에러 메세지
                /// </summary>
                Spl2ErrorCode = 0;
                Spl2ErrorMsg = "";
                GetErrorState_Spl2 = false;

                /// <summary>
                /// Sequece에서 Spindle Drv에게 명령을 보낼시 각 명령에 대한 송신 수신 Flag
                /// </summary>
                m_bRunCmdOk_Spl2 = false;
                m_bStopCmdOk_Spl2 = false;
                m_bFwdRevCmdOk_Spl2 = false;
                m_bResetCmdOk_Spl2 = false;
                m_bEF_StopCmdOk_Spl2 = false;
                
                m_bRPMCmdOk_Spl2 = false;
                m_nSetRPM_Spl2 = 0;

                m_bWheelChkCmdOk_Spl2 = false;
                m_nSetWheelChkPnt_Spl2 = 0;

                m_CmdSendOK_Spl2 = false;

                /// <summary>
                /// Spindle 통신 Seq Step 초기화 및 수신 완료 변수 초기화
                /// </summary>
                Spl2_SeqStep = 0;
                bSpl2_ReciveEnd = false;

                /// <summary>
                /// Spindle Drv 통신을 위한 Timer 동작
                /// </summary>
                tmrSpl2_Seq.Enabled = true;
            }

            iRet = (m_RS485_Spl2.IsOpen) ? 0 : 1;

            return iRet;
        }


        /// <summary>
        /// Spindle 통신 Seq에서 주기적으로 정보확인을 위한 통신 메세지 전송 함수
        /// </summary>
        private void SendSpindle1Cmd()
        {
            string strAddress = SpindleCmdAddress[Spindle1CmdAddresCnt].Substring(0, 4) ;
            string strDataCnt = "0001";
            string strSUM = "0103" + strAddress + strDataCnt;

            string strLRC = Make_LRC(strSUM);
            string strSendData = ":" + strSUM + strLRC + "\r\n";

            bSpl1_ReciveEnd = false;
            m_Spl1_ReciveData = "";
            SendMsg_Spl1(strSendData);            
        }


        /// <summary>
        /// Spindle 통신 Seq에서 주기적으로 정보확인을 위한 통신 메세지 전송 함수
        /// </summary>
        private void SendSpindle2Cmd()
        {
            string strAddress = SpindleCmdAddress[Spindle2CmdAddresCnt].Substring(0, 4);
            string strDataCnt = "0001";
            string strSUM = "0103" + strAddress + strDataCnt;

            string strLRC = Make_LRC(strSUM);
            string strSendData = ":" + strSUM + strLRC + "\r\n";

            SendMsg_Spl2(strSendData);
        }

        /// <summary>
        /// Spindle 통신 Seq에서 주기적으로 정보확인시 통신으로 메세지 전송이후 수신되는 메세지 처리 이벤트
        /// </summary>
        private void M_mSerial_DataReceived_Spl1(object sender, SerialDataReceivedEventArgs e)
        {// RPS Spindle + Delta C2000+ Drv RS458 통신 수신 이벤트 함수 ( Spindle 1)                       
            string sData = m_RS485_Spl1.ReadLine(); // Start Code 포함 , CR, LF 제외
            if (m_CmdSendOK_Spl1 == true)
            {
                if (sData.Length != 15) return;
            }
            else
            {
                if (sData.Length != 13) return;
            }

            m_Spl1_ReciveData = sData;
            bSpl1_ReciveEnd = true;
        }


        /// <summary>
        /// Spindle 통신 Seq에서 주기적으로 정보 확인 및 Spindle RUN,STOP등 동작을 위한 Timer Seq.
        /// </summary>
        void tmrSpl1_Seq_Tick(object sender, EventArgs e)
        {
            switch (Spl1_SeqStep)
            {
                case 0: // 통신 포트 오픈 확인
                    if (m_RS485_Spl1.IsOpen)
                        Spl1_SeqStep++;
                    break;

                case 1: // Sequecne상에 Spindle 제어를 위한 예약 명령어가 있는지 확인                                        
                    if (nSpl1_SendReservsion != 0)
                    {
                        // nSpl1_SendReservsion ---> 0 : 보낼 Cmd 없다. 1: RUN, 2: STOP, 3: Fwd/Bwd, 4:Reset, 5: EF Stop, 6 : RPM;
                        // 예약없으면 스핀들 상태값 읽기 위한 스텝으로 이동, 아니면 예약 명령 전송 처리후 수신완료 확인 루틴으로 이동
                        Spl1_SeqStep = 3;
                        m_CmdSendOK_Spl1 = true;
                        if (nSpl1_SendReservsion == 1) RunSpindle(EWay.L);
                        if (nSpl1_SendReservsion == 2) StopSpindle(EWay.L);
                        if (nSpl1_SendReservsion == 3) FwdRevSpindle(EWay.L);
                        if (nSpl1_SendReservsion == 4) ResetSpindle(EWay.L);
                        if (nSpl1_SendReservsion == 5) EF_StopSpindle(EWay.L);
                        if (nSpl1_SendReservsion == 6) SetRPM_Spindle(EWay.L, m_nSetRPM_Spl1);
                        if (nSpl1_SendReservsion == 7) SetWheelChk_Spindle(EWay.L, m_nSetWheelChkPnt_Spl1);
                        m_Timeout_Spindle1.Set_Delay(300);
                    }
                    else
                    {
                        // nSpl1_SendReservsion ---> 0 : 보낼 Cmd 없다
                        Spl1_SeqStep++;
                    }
                    break;

                case 2: // 상태값 읽기위한 함수 실행                   
                    SendSpindle1Cmd();
                    m_Timeout_Spindle1.Set_Delay(300);
                    Spl1_SeqStep++;
                    break;

                case 3: // 상태값 읽기 또는 예약명령어에 대한 수신 완료 확인
                    if (bSpl1_ReciveEnd)
                    {
                        // 수신 완료
                        Spl1_SeqStep++;
                    }
                    else
                    {
                        // 상태값 읽기시 시간 지연 발생으로 초기화 하고 처음 부터 다시 시작
                        if (m_Timeout_Spindle1.Chk_Delay()) 
                        {
                            bSpl1_ReciveEnd = false;
                            m_Spl1_ReciveData = "";
                            Spl1_SeqStep = 0;
                        }
                    }
                    break;

                case 4: // 수신된 값에 대한 통신 LRC 코드 확인
                    if (bCheck_LRC(m_Spl1_ReciveData))
                    {
                        // 예약 명령인지, 수신완료 되었는지 확인 
                        if (nSpl1_SendReservsion != 0 && m_CmdSendOK_Spl1 == true)
                        {
                            // 예약 명령에 대한 처리 완료 Flag set
                            if (nSpl1_SendReservsion == 1) m_bRunCmdOk_Spl1 = true;
                            if (nSpl1_SendReservsion == 2) m_bStopCmdOk_Spl1 = true;
                            if (nSpl1_SendReservsion == 3) m_bFwdRevCmdOk_Spl1 = true;
                            if (nSpl1_SendReservsion == 4) m_bResetCmdOk_Spl1 = true;
                            if (nSpl1_SendReservsion == 5) m_bEF_StopCmdOk_Spl1 = true;
                            if (nSpl1_SendReservsion == 6) m_bRPMCmdOk_Spl1 = true;
                            if (nSpl1_SendReservsion == 7) m_bWheelChkCmdOk_Spl1 = true;

                            // 예약 명령 처리 완료 Flag Reset
                            m_CmdSendOK_Spl1 = false;
                            nSpl1_SendReservsion = 0;
                            Spl1_SeqStep = 0;
                        }
                        else 
                        {
                            // 상태값 읽기동작으로 어떤 상태값인지 확인 하러 이동
                            Spl1_SeqStep++;
                        }
                    }
                    else
                    {
                        // 수신된 값 통신 LRC 에러 발생 예약 코드 및 메세지 수신 완료 Reset후 초기 동작으로 이동
                        if (nSpl1_SendReservsion != 0) nSpl1_SendReservsion = 0;
                        if (m_CmdSendOK_Spl1) m_CmdSendOK_Spl1 = false;
                        Spl1_SeqStep = 0;
                    }
                    break;

                case 5: // 읽어온 상태 메세지 분석 및 처리
                    if (Spl1DataParshing(Spindle1CmdAddresCnt, m_Spl1_ReciveData))
                    {
                        // 수신 메세지 정상
                        Spl1_SeqStep++;
                    }
                    else
                    {
                        // 수신 메세지에 오류(에러)
                        GetErrorState_Spl1 = true;
                        Spl1ErrorMsg = "ERROR 알수 없음";
                        if (Spl1ErrorCode == 1) Spl1ErrorMsg = "ERROR 1 : 불법데이터 값 -> 명령 메세지에 받은 데이터 값은 드라이브에 사용불가 합니다.";
                        if (Spl1ErrorCode == 2) Spl1ErrorMsg = "ERROR 2 : 불볍 데이터 주소 --> 명령 메세지에 받은 데이터 주소는 드라이브에 사용 불가 합니다.";
                        if (Spl1ErrorCode == 3) Spl1ErrorMsg = "ERROR 3 : 파라미터가 잠김 --> 파라미터를 바꾸지 못합니다.";
                        if (Spl1ErrorCode == 4) Spl1ErrorMsg = "ERROR 4 : 파라미터는 운전중 바꾸지 못합니다.";
                        if (Spl1ErrorCode == 10) Spl1ErrorMsg = "ERROR 10 : 통신 시간 초과.";

                        Spl1_SeqStep = 0;
                    }
                    break;

                case 6: // 다음 상태값을 읽기위한 상태 메세지 변경
                    Spindle1CmdAddresCnt++;
                    if (Spindle1CmdAddresCnt == SpindleCmdAddresMaxCnt) Spindle1CmdAddresCnt = 0;
                    Spl1_SeqStep = 0;
                    break;

                default:
                    Spl1_SeqStep = -1;
                    break;
            }
        }


        /// <summary>
        /// Spindle 통신시 수신된 메세지 내용 처리 및 처리된 결과를 저장(값)
        /// </summary>
        private bool Spl1DataParshing(int Addr, string sParshing)
        {
            string ErrorFunction;
            string ErrorCode;

            if (sParshing == "") return false;

            ErrorFunction = sParshing.Substring(3, 2);
            if (ErrorFunction == "86")
            {
                ErrorCode = sParshing.Substring(5, 2);
                Spl1ErrorCode = Convert.ToInt32(ErrorCode);
                /*
                 Spl1ErrorCode = 1 or 2 or 3 or 4 or 10
                1 : 불법데이터 값 -> 명령 메세지에 받은 데이터 값은 드라이브에 사용불가 합니다.
                2 : 불볍 데이터 주소 --> 명령 메세지에 받은 데이터 주소는 드라이브에 사용 불가 합니다.
                3 : 파라미터가 잠김 --> 파라미터를 바꾸지 못합니다.
                4 : 파라미터는 운전중 바꾸지 못합니다.
                10 : 통신 시간 초과.
                */
                return false;
            }


            string DataCnt, DataValue;
            int nDataCnt, nTmpValue;

            //수신된 문자열중 데이터의 갯수가 몇개인지 추출
            DataCnt = sParshing.Substring(5, 2);
            nDataCnt = Convert.ToInt32(DataCnt);
            
            // 데이터의 갯수 만큼 데이터 문자열 추출 
            DataValue = sParshing.Substring(7, nDataCnt * 2);
            nTmpValue = ToDec(DataValue);

            switch (Addr)
            {
                /*
                case 0: Spl1_Data.Add2100h = nTmpValue; break; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
                case 1: Spl1_Data.Add2102h = nTmpValue; break; //"2102H", 주파수 명령(F)
                case 2: Spl1_Data.Add2103h = nTmpValue; break; //"2103H", 출력 주파수(H)
                case 3: Spl1_Data.Add2116h = nTmpValue; break; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
                case 4: Spl1_Data.Add2119h = nTmpValue; break; //"2119H", Bit  신호
                case 5: Spl1_Data.Add2200h = nTmpValue; CData.Spls[(int)EWay.L].nCurrent_Amp = nTmpValue; break; //"2200H", 출력 전류을 표시(A)
                case 6: Spl1_Data.Add2202h = nTmpValue; break; //"2202H", 실제 출력 주파수를 표시(H)
                case 7: Spl1_Data.Add2207h = nTmpValue; CData.Spls[(int)EWay.L].iRpm = nTmpValue; break; //"2207H", 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
                case 8: Spl1_Data.Add2208h = nTmpValue; CData.Spls[(int)EWay.L].dLoad = nTmpValue; break; //"2208H", 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
                case 9: Spl1_Data.Add2209h = nTmpValue; break; //"2209H", PG 피드백을 표시(NOTE 1처럼)
                case 10: Spl1_Data.Add220Eh = nTmpValue; CData.Spls[(int)EWay.R].dTemp_Val = nTmpValue / 10.0;  break; //"220EH", 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
                case 11: Spl1_Data.Add220Fh = nTmpValue; break; //"220FH", 캐패시턴스의 온도를 'C로 나타냄(i.)
                case 12: Spl1_Data.Add2215h = nTmpValue; break; //"2215H", 실제 모터의 회전 수(PG카드의 PG1)(P.) 실제 운전 방향이 바뀌거나 키패드가 멈춤에서 0을 나타내며 9에서 시작함. 최대는 65535(P.)
                case 13: Spl1_Data.Add2217h = nTmpValue; break; //"2217H", 펄스 입력 위치(PG카드의 PG2)(4.)
                case 14: Spl1_Data.Add221Fh = nTmpValue; break; //"221FH"  Pr.00-05의 출력 값
                */

                /*
                case 0: Spl1_Data.Add2100h = nTmpValue; break; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
                case 1: Spl1_Data.Add2102h = nTmpValue; break; //"2102H", 주파수 명령(F)
                case 2: Spl1_Data.Add2103h = nTmpValue; break; //"2103H", 출력 주파수(H)
                case 3: Spl1_Data.Add2116h = nTmpValue; break; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
                case 4: Spl1_Data.Add2200h = nTmpValue; CData.Spls[(int)EWay.L].nCurrent_Amp = nTmpValue; break; //"2200H", 출력 전류을 표시(A)
                case 5: Spl1_Data.Add2207h = nTmpValue; CData.Spls[(int)EWay.L].iRpm = nTmpValue; break; //"2207H", 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
                case 6: Spl1_Data.Add2208h = nTmpValue; CData.Spls[(int)EWay.L].dLoad = nTmpValue; break; //"2208H", 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
                case 7: Spl1_Data.Add220Eh = nTmpValue; CData.Spls[(int)EWay.L].dTemp_Val = nTmpValue / 10.0; break; //"220EH", 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
                */

                case 0: Spl1_Data.Add2100h = nTmpValue; break; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
                case 1: Spl1_Data.Add2116h = nTmpValue; break; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
                case 2: Spl1_Data.Add2200h = nTmpValue; CData.Spls[(int)EWay.L].nCurrent_Amp = nTmpValue; break; //"2200H", 출력 전류을 표시(A)
                case 3: Spl1_Data.Add2207h = nTmpValue; CData.Spls[(int)EWay.L].iRpm = nTmpValue; break; //"2207H", 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
                case 4: Spl1_Data.Add2208h = nTmpValue; CData.Spls[(int)EWay.L].dLoad = nTmpValue; break; //"2208H", 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
            }    

            return true;
        }


        /// <summary>
        /// Spindle 통신 Seq에서 주기적으로 정보확인시 통신으로 메세지 전송이후 수신되는 메세지 처리 이벤트
        /// </summary>
        private void M_mSerial_DataReceived_Spl2(object sender, SerialDataReceivedEventArgs e)
        {// RPS Spindle + Delta C2000+ Drv RS458 통신 수신 이벤트 함수 ( Spindle 2)            
            string sData = m_RS485_Spl2.ReadLine();
            if (m_CmdSendOK_Spl2 == true)
            {
                if (sData.Length != 15) return;
            }
            else
            {
                if (sData.Length != 13) return;
            }
            m_Spl2_ReciveData = sData;
            bSpl2_ReciveEnd = true;
        }



        /// <summary>
        /// Spindle 통신 Seq에서 주기적으로 정보 확인 및 Spindle RUN,STOP등 동작을 위한 Timer Seq.
        /// tmrSpl1_Seq_Tick 함수와 동일 함.
        /// </summary>
        void tmrSpl2_Seq_Tick(object sender, EventArgs e)
        {
            switch (Spl2_SeqStep)
            {
                case 0:
                    if (m_RS485_Spl2.IsOpen)
                        Spl2_SeqStep++;
                    break;

                case 1:
                    // nSpl2_SendReservsion ---> 0 : 보낼 Cmd 없다. 1: RUN, 2: STOP, 3: Fwd/Bwd, 4:Reset, 5: EF Stop, 6 : RPM;
                    if (nSpl2_SendReservsion != 0)
                    {
                        Spl2_SeqStep = 3;
                        m_CmdSendOK_Spl2 = true;
                        if (nSpl2_SendReservsion == 1) RunSpindle(EWay.R);
                        if (nSpl2_SendReservsion == 2) StopSpindle(EWay.R);
                        if (nSpl2_SendReservsion == 3) FwdRevSpindle(EWay.R);
                        if (nSpl2_SendReservsion == 4) ResetSpindle(EWay.R);
                        if (nSpl2_SendReservsion == 5) EF_StopSpindle(EWay.R);
                        if (nSpl2_SendReservsion == 6) SetRPM_Spindle(EWay.R, m_nSetRPM_Spl2);
                        if (nSpl2_SendReservsion == 7) SetWheelChk_Spindle(EWay.R, m_nSetWheelChkPnt_Spl2);
                        
                        m_Timeout_Spindle2.Set_Delay(300);
                    }
                    else
                    {
                        Spl2_SeqStep++;
                    }
                    break;

                case 2:
                    SendSpindle2Cmd();
                    m_Timeout_Spindle2.Set_Delay(300);
                    Spl2_SeqStep++;
                    break;

                case 3:
                    if (bSpl2_ReciveEnd)
                    {
                        // 수신 완료
                        Spl2_SeqStep++;
                    }
                    else
                    {
                        if (m_Timeout_Spindle2.Chk_Delay())
                        {
                            bSpl2_ReciveEnd = false;
                            m_Spl2_ReciveData = "";
                            Spl2_SeqStep = 0;
                        }
                    }
                    break;

                case 4:
                    if (bCheck_LRC(m_Spl2_ReciveData))
                    {
                        if (nSpl2_SendReservsion != 0 && m_CmdSendOK_Spl2 == true)
                        {
                            if (nSpl2_SendReservsion == 1) m_bRunCmdOk_Spl2 = true;
                            if (nSpl2_SendReservsion == 2) m_bStopCmdOk_Spl2 = true;
                            if (nSpl2_SendReservsion == 3) m_bFwdRevCmdOk_Spl2 = true;
                            if (nSpl2_SendReservsion == 4) m_bResetCmdOk_Spl2 = true;
                            if (nSpl2_SendReservsion == 5) m_bEF_StopCmdOk_Spl2 = true;
                            if (nSpl2_SendReservsion == 6) m_bRPMCmdOk_Spl2 = true;
                            if (nSpl2_SendReservsion == 7) m_bWheelChkCmdOk_Spl2 = true;                            

                            m_CmdSendOK_Spl2 = false;
                            nSpl2_SendReservsion = 0;
                            Spl2_SeqStep = 0;
                        }
                        else
                        {
                            Spl2_SeqStep++;
                        }
                    }
                    else
                    {
                        if (nSpl2_SendReservsion != 0) nSpl2_SendReservsion = 0;
                        if (m_CmdSendOK_Spl2) m_CmdSendOK_Spl2 = false;
                        Spl2_SeqStep = 0;
                    }
                    break;

                case 5:
                    if (Spl2DataParshing(Spindle2CmdAddresCnt, m_Spl2_ReciveData))
                    {
                        Spl2_SeqStep++;
                    }
                    else
                    {
                        GetErrorState_Spl2 = true;
                        Spl2ErrorMsg = "ERROR 알수 없음";
                        if (Spl2ErrorCode == 1) Spl2ErrorMsg = "ERROR 1 : 불법데이터 값 -> 명령 메세지에 받은 데이터 값은 드라이브에 사용불가 합니다.";
                        if (Spl2ErrorCode == 2) Spl2ErrorMsg = "ERROR 2 : 불볍 데이터 주소 --> 명령 메세지에 받은 데이터 주소는 드라이브에 사용 불가 합니다.";
                        if (Spl2ErrorCode == 3) Spl2ErrorMsg = "ERROR 3 : 파라미터가 잠김 --> 파라미터를 바꾸지 못합니다.";
                        if (Spl2ErrorCode == 4) Spl2ErrorMsg = "ERROR 4 : 파라미터는 운전중 바꾸지 못합니다.";
                        if (Spl2ErrorCode == 10) Spl2ErrorMsg = "ERROR 10 : 통신 시간 초과.";

                        Spl2_SeqStep = 0;
                    }
                    break;

                case 6:
                    Spindle2CmdAddresCnt++;
                    if (Spindle2CmdAddresCnt == SpindleCmdAddresMaxCnt) Spindle2CmdAddresCnt = 0;
                    Spl2_SeqStep = 0;
                    break;

                default:
                    Spl2_SeqStep = -1;
                    break;
            }
        }


        /// <summary>
        /// Spindle 통신시 수신된 메세지 내용 처리 및 처리된 결과를 저장(값)
        /// Spl1DataParshing 함수와 동일함.
        /// </summary>
        private bool Spl2DataParshing(int Addr, string sParshing)
        {
            string ErrorFunction;
            string ErrorCode;

            if (sParshing == "") return false;

            ErrorFunction = sParshing.Substring(3, 2);
            if (ErrorFunction == "86")
            {
                ErrorCode = sParshing.Substring(5, 2);
                Spl1ErrorCode = Convert.ToInt32(ErrorCode);
                /*
                1 : 불법데이터 값 -> 명령 메세지에 받은 데이터 값은 드라이브에 사용불가 합니다.
                2 : 불볍 데이터 주소 --> 명령 메세지에 받은 데이터 주소는 드라이브에 사용 불가 합니다.
                3 : 파라미터가 잠김 --> 파라미터를 바꾸지 못합니다.
                4 : 파라미터는 운전중 바꾸지 못합니다.
                10 : 통신 시간 초과.
                */
                return false;
            }


            string DataCnt, DataValue;
            int nDataCnt, nTmpValue;

            DataCnt = sParshing.Substring(5, 2);
            nDataCnt = Convert.ToInt32(DataCnt);

            DataValue = sParshing.Substring(7, nDataCnt * 2);
            nTmpValue = ToDec(DataValue);

            switch (Addr)
            {
                /*
                case 0: Spl2_Data.Add2100h = nTmpValue; break; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
                case 1: Spl2_Data.Add2102h = nTmpValue; break; //"2102H", 주파수 명령(F)
                case 2: Spl2_Data.Add2103h = nTmpValue; break; //"2103H", 출력 주파수(H)
                case 3: Spl2_Data.Add2116h = nTmpValue; break; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
                case 4: Spl2_Data.Add2119h = nTmpValue; break; //"2119H", Bit  신호
                case 5: Spl2_Data.Add2200h = nTmpValue; CData.Spls[(int)EWay.R].nCurrent_Amp = nTmpValue;  break; //"2200H", 출력 전류을 표시(A)
                case 6: Spl2_Data.Add2202h = nTmpValue; break; //"2202H", 실제 출력 주파수를 표시(H)
                case 7: Spl2_Data.Add2207h = nTmpValue; CData.Spls[(int)EWay.R].iRpm = nTmpValue; break; //"2207H", 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
                case 8: Spl2_Data.Add2208h = nTmpValue; CData.Spls[(int)EWay.R].dLoad = nTmpValue; break; //"2208H", 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
                case 9: Spl2_Data.Add2209h = nTmpValue; break; //"2209H", PG 피드백을 표시(NOTE 1처럼)
                case 10: Spl2_Data.Add220Eh = nTmpValue; CData.Spls[(int)EWay.R].dTemp_Val = nTmpValue / 10.0;  break; //"220EH", 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
                case 11: Spl2_Data.Add220Fh = nTmpValue; break; //"220FH", 캐패시턴스의 온도를 'C로 나타냄(i.)
                case 12: Spl2_Data.Add2215h = nTmpValue; break; //"2215H", 실제 모터의 회전 수(PG카드의 PG1)(P.) 실제 운전 방향이 바뀌거나 키패드가 멈춤에서 0을 나타내며 9에서 시작함. 최대는 65535(P.)
                case 13: Spl2_Data.Add2217h = nTmpValue; break; //"2217H", 펄스 입력 위치(PG카드의 PG2)(4.)
                case 14: Spl2_Data.Add221Fh = nTmpValue; break; //"221FH"  Pr.00-05의 출력 값
                */
                /*
                case 0: Spl2_Data.Add2100h = nTmpValue; break; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
                case 1: Spl2_Data.Add2102h = nTmpValue; break; //"2102H", 주파수 명령(F)
                case 2: Spl2_Data.Add2103h = nTmpValue; break; //"2103H", 출력 주파수(H)
                case 3: Spl2_Data.Add2116h = nTmpValue; break; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
                case 4: Spl2_Data.Add2200h = nTmpValue; CData.Spls[(int)EWay.R].nCurrent_Amp = nTmpValue; break; //"2200H", 출력 전류을 표시(A)
                case 5: Spl2_Data.Add2207h = nTmpValue; CData.Spls[(int)EWay.R].iRpm = nTmpValue; break; //"2207H", 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
                case 6: Spl2_Data.Add2208h = nTmpValue; CData.Spls[(int)EWay.R].dLoad = nTmpValue; break; //"2208H", 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
                case 7: Spl2_Data.Add220Eh = nTmpValue; CData.Spls[(int)EWay.R].dTemp_Val = nTmpValue / 10.0; break; //"220EH", 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
                */
                case 0: Spl2_Data.Add2100h = nTmpValue; break; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
                case 1: Spl2_Data.Add2116h = nTmpValue; break; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
                case 2: Spl2_Data.Add2200h = nTmpValue; CData.Spls[(int)EWay.R].nCurrent_Amp = nTmpValue; break; //"2200H", 출력 전류을 표시(A)
                case 3: Spl2_Data.Add2207h = nTmpValue; CData.Spls[(int)EWay.R].iRpm = nTmpValue; break; //"2207H", 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
                case 4: Spl2_Data.Add2208h = nTmpValue; CData.Spls[(int)EWay.R].dLoad = nTmpValue; break; //"2208H", 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
            };

            return true;
        }


        /// <summary>
        /// Spindle1 통신중 에러 발생시 에러 메세지 전송
        /// </summary>
        public string GetErrorMsg_Spl1()
        {
            string sMsg;

            sMsg = "Error[" + Convert.ToString(Spl1ErrorCode) + "] "+ Spl1ErrorMsg;

            GetErrorState_Spl1 = false;

            return sMsg;
        }


        /// <summary>
        /// Spindle1 통신중 에러 발생시 에러 메세지 전송
        /// </summary>
        public string GetErrorMsg_Spl2()
        {
            string sMsg;

            sMsg = "Error[" + Convert.ToString(Spl2ErrorCode) + "] " + Spl2ErrorMsg;

            GetErrorState_Spl2 = false;

            return sMsg;
        }

        /// <summary>
        /// Spindle Drv Command Message 전송
        /// </summary>
        private bool SendMsg_Spl1(string sMsg)
        {
            if (m_RS485_Spl1 == null) return false;  
            if (!m_RS485_Spl1.IsOpen)  return false; 
            
            m_RS485_Spl1.Write(sMsg);

            return true;
        }


        /// <summary>
        /// Spindle Drv Command Message 전송
        /// </summary>
        private bool SendMsg_Spl2(string sMsg)
        {
            if (m_RS485_Spl2 == null) return false;  
            if (!m_RS485_Spl2.IsOpen) return false;

            m_RS485_Spl2.Write(sMsg);

            return true;
        }


        /// <summary>
        /// Spindle Drv Command 전송 되도록 메세지 전송 예약 설정
        /// </summary>
        public void Spl1_Reservsion(int ReservsionCode, int nRPM)
        {
            if (m_RS485_Spl1.IsOpen == false) return;
            
            nSpl1_SendReservsion = ReservsionCode;

            if (nSpl1_SendReservsion == 6)
            {
                m_nSetRPM_Spl1 = nRPM;
            }

            if (nSpl1_SendReservsion == 7)
            {
                m_nSetWheelChkPnt_Spl1 = nRPM; // nRPM은 Wheel 회전 위치값
            }
        }


        /// <summary>
        /// Spindle Drv Command 전송 되도록 메세지 전송 예약 설정
        /// </summary>
        public void Spl2_Reservsion(int ReservsionCode, int nRPM)
        {
            if (m_RS485_Spl2.IsOpen == false) return;
            
            nSpl2_SendReservsion = ReservsionCode;

            if (nSpl2_SendReservsion == 6)
            {
                m_nSetRPM_Spl2 = nRPM;
            }

            if (nSpl2_SendReservsion == 7)
            {
                m_nSetWheelChkPnt_Spl2 = nRPM; // nRPM은 Wheel 회전 위치값
            }
        }

        /// <summary>
        /// Spindle Stop Command 전송 되도록 메세지 전송 예약 설정
        /// </summary>
        public void Write_Stop(EWay eWay)
        {
            if (eWay == EWay.L)
            {
                Spl1_Reservsion(2, 0);
            }
            else
            {
                Spl2_Reservsion(2, 0);
            }
        }


        /// <summary>
        /// Spindle RUN Command 전송 되도록 메세지 전송 예약 설정
        /// </summary>
        public void Write_Run(EWay eWay)
        {
            if (eWay == EWay.L)
            {
                Spl1_Reservsion(1, 0);
            }
            else
            {
                Spl2_Reservsion(1, 0);
            }
        }


        /// <summary>
        /// Spindle Drv 초기화 Command 전송 되도록 메세지 전송 예약 설정
        /// </summary>
        public void Write_Reset(EWay eWay)
        {
            if (eWay == EWay.L)
            {
                Spl1_Reservsion(4, 0);
            }
            else
            {
                Spl2_Reservsion(4, 0);
            }
        }


        /// <summary>
        /// Spindle Drv RPM 설정 Command 전송 되도록 메세지 전송 예약 설정
        /// </summary>
        public void Write_Rpm(EWay eWay, int iRpm)
        {
            
            if (eWay == EWay.L)
            {
                Spl1_Reservsion(6, iRpm);
            }
            else
            {
                Spl2_Reservsion(6, iRpm);
            }
        }

        public void Write_WheelChkPosition(EWay eWay, int nPnt)
        {

            if (eWay == EWay.L)
            {
                Spl1_Reservsion(7, nPnt);
            }
            else
            {
                Spl2_Reservsion(7, nPnt);
            }
        }        


        /// <summary>
        /// Spindle Drv 설정된 RPM 값 확인
        /// </summary>
        public int GetFrpm(EWay eWay)
        {
            return CData.Spls[(int)eWay].iRpm;
            //if (eWay == EWay.L) return Spl1_Data.Add220Eh;
            //else if (eWay == EWay.R) return Spl2_Data.Add220Eh;
            //else return -1;
        }


        /// <summary>
        /// Spindle Drv Encode(위치값 0 ~ 4096) 획득
        /// 단 Delta Drive 표시장치(디스플레이)에 G xxxx PLS로 설정 되어야 함.
        /// 아니면 값을 읽어 오지 못함.
        /// </summary>
        public int Get_Position_Value(EWay eWay, ref int iVal)
        {
            int iRet = 0;

            if (eWay == EWay.L)
            {
                iVal = Spl1_Data.Add2116h; //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
            }
            else
            {
                iVal = Spl2_Data.Add2116h;  //"2116H", 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
            }

            return iRet;
        }

        /// <summary>
        /// Spindle Drv 알람코드 읽기
        /// </summary>
        public bool Chk_Alarm(EWay eWay)
        {
            int nVal = 0;

            if (eWay == EWay.L)
            {
                nVal = Spl1_Data.Add2100h; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
            }
            else
            {
                nVal = Spl2_Data.Add2100h; // "2100H", 오류코드: Pr.06-17에서 Pr.06-22를 따름
            }

            return Convert.ToBoolean(nVal);
        }

        /// <summary>
        /// Spindle Drv 부하값 읽기
        /// </summary>
        public double Get_Load(EWay eWay)
        {
            return CData.Spls[(int)eWay].dLoad;            
        }

        /// <summary>
        /// Spindle Drv 온도값 읽기
        /// </summary>
        public double Get_Spindle_Temp(EWay eWay)
        {
            return CData.Spls[(int)eWay].dTemp_Val;
        }

        /// <summary>
        /// Spindle Drv Current값 읽기
        /// </summary>
        public int Get_Spindle_Current(EWay eWay)
        {
            return CData.Spls[(int)eWay].nCurrent_Amp;
        }


        /// <summary>
        /// Spindle Drv RUN Command 전송
        /// </summary>
        private bool RunSpindle(EWay eWay)
        {
            bool nRet = false;
            string msg = ":010620000002D7\r\n";


            if (eWay == EWay.L)
            {
                bSpl1_ReciveEnd = false;
                m_bRunCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }
            else
            {
                bSpl2_ReciveEnd = false;
                m_bRunCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }
            
            return nRet;
        }


        /// <summary>
        /// Spindle Drv RUN Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptRunSpindle(EWay eWay)
        {
            bool bRet = false ;

            if (EWay.L == eWay)
            {
                bRet = m_bRunCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bRunCmdOk_Spl2;
            }

            return bRet;
        }


        /// <summary>
        /// Spindle Drv RUN Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptRunSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bRunCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bRunCmdOk_Spl2 = false;
            }
        }


        /// <summary>
        /// Spindle Drv STOP Command 전송
        /// </summary>
        private bool StopSpindle(EWay eWay)
        {
            bool nRet = false;
            string msg = ":010620000001D8\r\n";

            if (EWay.L == eWay)
            {
                bSpl1_ReciveEnd = false;
                m_bStopCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }

            if (EWay.R == eWay)
            {
                bSpl2_ReciveEnd = false;
                m_bStopCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }

            return nRet;
        }


        /// <summary>
        /// Spindle Drv STOP Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptStopSpindle(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay)
            {
                bRet = m_bStopCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bStopCmdOk_Spl2;

            }

            return bRet;
        }


        /// <summary>
        /// Spindle Drv STOP Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptStopSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bStopCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bStopCmdOk_Spl2 = false;
            }
        }

        /// <summary>
        /// Spindle Drv Fwd/Back(회전방향 설정) Command 전송
        /// </summary>
        private bool FwdRevSpindle(EWay eWay)
        {
            bool nRet = false;
            string msg = ":010620000030A9\r\n";

            if (EWay.L == eWay)
            {
                bSpl1_ReciveEnd = false;
                m_bFwdRevCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }

            if (EWay.R == eWay)
            {
                bSpl2_ReciveEnd = false;
                m_bFwdRevCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }

            return nRet;
        }


        /// <summary>
        /// Spindle Drv Fwd/Back(회전방향 설정) Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptFwdRevSpindle(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay)
            {
                bRet = m_bFwdRevCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bFwdRevCmdOk_Spl2;
            }

            return bRet;
        }


        /// <summary>
        /// Spindle Drv Fwd/Back(회전방향 설정) Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptFwdRevSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bFwdRevCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bFwdRevCmdOk_Spl2 = false;
            }
        }


        /// <summary>
        /// Spindle Drv Reset(초기화) Command 전송
        /// </summary>
        private bool ResetSpindle(EWay eWay)
        {
            bool nRet = false;
            string msg = ":FF0620020002D7\r\n";

            if (EWay.L == eWay)
            {
                bSpl1_ReciveEnd = false;
                m_bResetCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }

            if (EWay.R == eWay)
            {
                bSpl2_ReciveEnd = false;
                m_bResetCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }

            return nRet;
        }

        /// <summary>
        /// Spindle Drv Reset(초기화) Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptResetSpindle(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay)
            {
                bRet = m_bResetCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bResetCmdOk_Spl2;
            }

            return bRet;
        }


        /// <summary>
        /// Spindle Drv Reset(초기화) Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptResetSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bResetCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bResetCmdOk_Spl2 = false;
            }
        }

        /// <summary>
        /// Spindle Drv EF-Stop (긴급정지) Command 전송
        /// </summary>
        private bool EF_StopSpindle(EWay eWay)
        {
            bool nRet = false;
            string msg = ":010620020001D6\r\n";

            if (EWay.L == eWay)
            {
                bSpl1_ReciveEnd = false;
                m_bEF_StopCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }

            if (EWay.R == eWay)
            {
                bSpl2_ReciveEnd = false;
                m_bEF_StopCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }

            return nRet;
        }

        /// <summary>
        /// Spindle Drv EF-Stop (긴급정지) Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptEF_StopSpindle(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay)
            {
                bRet = m_bEF_StopCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bEF_StopCmdOk_Spl2;
            }

            return bRet;
        }

        /// <summary>
        /// Spindle Drv EF-Stop (긴급정지) Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptEF_StopSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bEF_StopCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bEF_StopCmdOk_Spl2 = false;
            }
        }


        /// <summary>
        /// Spindle Drv RPM 설정시 입력된 RPM 값을 주파수 Range내 값으로 변환
        /// </summary>
        //private static int remap(int val, int in1, int in2, int out1, int out2)
        private double remap(double val, double in1, double in2, double out1, double out2)
        {
            // val : 변수
            // in1 : 변수의 최소값
            // in2 : 변수의 최대값
            // out1 : 변경할 최소값 
            // out2 : 변경할 최대값
            double dRet = 0;

            dRet = out1 + (val - in1) * (out2 - out1) / (in2 - in1);

            return dRet;
        }

        /// <summary>
        /// Spindle Drv RPM 설정  Command 전송시 설정 RPM 값을 기준으로 전송 Command 구성
        /// </summary>
        private bool SetRPM_Spindle(EWay eWay, int _RPM)
        {
            bool nRet = false;
            string msg = "";
            string strRPM = "";
            string strSUM = "";
            string strLRC = "";
            
            int nRPM = 0;

            // RPM = 주파수 * 30
            // 주파수 =  RPM / 30
            double dFreq = 0.0; // 최소 5Hz (150rpm) , 최대 133Hz (Max 4000rpm)
            int nFreq = 0;

            nRPM = _RPM;
            if (_RPM < 0) // if (_RPM < 150)
            {
                nRPM = 0;
            }
            if (_RPM > 4000)
            {
                nRPM = 4000;
            }

            // 입력된 RPM 값을 회전 주파수로 변환 
            //             Rpm 입력값, Rpm 최소값, Rpm 최대값, 주파수 최소값, 주파수 최대값
            dFreq = remap(_RPM, 0, 4000, 0.0, 133.4) * 100;
            nFreq = (int)dFreq;

            strRPM = ToHex16(nFreq);

            strSUM = "01062001" + strRPM;

            strLRC = Make_LRC(strSUM);
            msg = ":" + strSUM + strLRC + "\r\n";

            if (EWay.L == eWay)
            {
                bSpl1_ReciveEnd = false;
                m_bRPMCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }

            if (EWay.R == eWay)
            {
                bSpl2_ReciveEnd = false;
                m_bRPMCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }

            return nRet;
        }


        /// <summary>
        /// Spindle Drv RPM set Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptRPMSpindle(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay)
            {
                bRet = m_bRPMCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bRPMCmdOk_Spl2;
            }

            return bRet;
        }


        /// <summary>
        /// Spindle Drv RPM set Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptRPMSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bRPMCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bRPMCmdOk_Spl2 = false;
            }
        }


        /// <summary>
        /// Wheel Tip & Toolsetter 3point 측정을 위한 Spindle Drv 회전 Command 전송
        /// </summary>
        private bool SetWheelChk_Spindle(EWay eWay, int _nPosition)
        {
            bool nRet = false;
            string msg = "";
            string strPOSITION = "";
            string strSUM = "";
            string strLRC = "";

            int nPosition = 0;            

            nPosition = _nPosition;
            if (_nPosition < 0) // if (_RPM < 150)
            {
                nPosition = 0;
            }
            if (_nPosition > 4095)
            {
                nPosition = 4095;
            }

            strPOSITION = ToHex16(nPosition);

            strSUM = "01060B42" + strPOSITION;

            strLRC = Make_LRC(strSUM);
            msg = ":" + strSUM + strLRC + "\r\n";

            if (EWay.L == eWay)
            {
                bSpl1_ReciveEnd = false;
                m_bWheelChkCmdOk_Spl1 = false;
                m_Spl1_ReciveData = "";
                nRet = SendMsg_Spl1(msg);
            }

            if (EWay.R == eWay)
            {
                bSpl2_ReciveEnd = false;
                m_bWheelChkCmdOk_Spl2 = false;
                m_Spl2_ReciveData = "";
                nRet = SendMsg_Spl2(msg);
            }

            return nRet;
        }


        /// <summary>
        /// Wheel Tip & Toolsetter 3point 측정 Spindle Drv Command 예약 전송후 처리 완료 되었는지 확인
        /// </summary>
        public bool GetAcceptWheelChkSpindle(EWay eWay)
        {
            bool bRet = false;

            if (EWay.L == eWay)
            {
                bRet = m_bWheelChkCmdOk_Spl1;
            }

            if (EWay.R == eWay)
            {
                bRet = m_bWheelChkCmdOk_Spl2;
            }

            return bRet;
        }


        /// <summary>
        /// Wheel Tip & Toolsetter 3point 측정 Spindle Drv Command 예약 전송후 처리완료 설정값 리셋
        /// </summary>
        public void SetAcceptWheelChkSpindle(EWay eWay)
        {
            if (EWay.L == eWay)
            {
                m_bWheelChkCmdOk_Spl1 = false;
            }

            if (EWay.R == eWay)
            {
                m_bWheelChkCmdOk_Spl2 = false;
            }
        }


        /// <summary>
        /// Int를 16진수 문자 2자리로 반환
        /// </summary>
        private string ToHex8(int i)
        {
            string hex = i.ToString("x");
            if (hex.Length %2 != 0)
            {
                hex = "0" + hex;
            }
            return hex;
        }


        /// <summary>
        /// Int를 16진수 문자 4자리로 반환
        /// </summary>
        private string ToHex16(int i)
        {
            string hex = i.ToString("x");
            if (hex.Length == 1)
            {
                hex = "000" + hex;
            }
            else if (hex.Length == 2)
            {
                hex = "00" + hex;
            }
            else if (hex.Length == 3)
            {
                hex = "0" + hex;
            }

            return hex.ToUpper();
        }

        /// <summary>
        /// 16진수 문자를 int로 반환
        /// </summary>
        private int ToDec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }


        /// <summary>
        /// Spindle Drv로 전송 되는 문자(Command)의 통신오류 확인용 LRC값 생성
        /// </summary>
        private string Make_LRC(string msg)
        {
            string strRet;
            string strHex, strData, cHi, cLow;
            int cLRC_SUM, cLRC, cLRC_Data;

            strData = msg;

            cLRC_SUM = 0;            
            for (int i= 0; i < strData.Length; i+=2)
            {
                cHi = strData.Substring(i, 1);
                cLow = strData.Substring(i + 1, 1);

                strHex = cHi + cLow;
                cLRC_Data = ToDec(strHex);               
                cLRC_SUM += cLRC_Data;
            }

            cLRC = cLRC_SUM &  0xff;
            cLRC = cLRC ^ 0x00ff;
            cLRC = cLRC + 0x0001;
            cLRC = cLRC & 0x00ff; // 600,1900입력시 Integer로 되어 있어 LRC값이 3자리로 나타 나는 현상때문에 상위 바이트는 0으로 비트 And연산을 함.

            strRet = ToHex8(cLRC);
            return strRet.ToUpper();;
        }


        /// <summary>
        /// Spindle Drv로 수신 되는 문자(Command)를 통신오류가 있는지 확인 
        /// </summary>
        private bool bCheck_LRC(string strReciveData)
        {
            string strCmd,strData, strLRC, strLRC_CHK;
            int Lengths;

            strData = strReciveData.Trim();
            Lengths = strData.Length-1;
            strCmd = strData.Substring(1, Lengths);
            strData = strCmd;

            strLRC_CHK = strData.Substring(strCmd.Length - 2, 2);
            strData = strData.Substring(0, strCmd.Length - 2);

            strLRC = Make_LRC(strData);

            if (strLRC != strLRC_CHK) return false;
            return true;
        }
    }
}
