using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG2000X
{
    class CQcVisionCom 
    {
        
        public static bool sndEQAbortTransfer = false; //EQ -> QC 
        public static bool sndEQAutoStop = false; //EQ -> QC 

        public static bool rcvEQAbortTransferACK = false; //EQ <- QC 
        public static bool rcvEQSendRequst = false; //EQ <- QC 
        public static bool rcvEQReadyQueryOK = false; //EQ <- QC 
        public static bool rcvEQSendOutACK = false; //EQ <- QC 
        public static bool rcvEQSendEndACK = false; //EQ <- QC 

        public static bool rcvQCSendRequest = false; //EQ <- QC 
        public static bool rcvQCSendRequestACK = false; //EQ <- QC 
        public static bool rcvQCReadyQueryNG = false; //EQ <- QC         
        public static bool rcvQCReadyQueryGOOD = false; //EQ <- QC         
        public static bool rcvQCSendOutGOOD = false; //EQ <- QC
        public static bool rcvQCSendOutNG = false; //EQ <- QC
        public static bool rcvQCSendEnd = false; //EQ <- QC
        public static bool rcvQCAbortTransfer = false; //EQ <- QC

        public static bool rcvAutoRunACK = false; //EQ <- QC
        public static bool rcvAutoStopACK = false; //EQ <- QC
        public static bool rcvErrorResetACK = false; //EQ <- QC

        public static bool rcvLotEndACK = false; //EQ <- QC
        public static bool rcvDoorLock_On_OK = false; //EQ <- QC
        public static bool rcvDoorLock_On_NG = false; //EQ <- QC

        public static bool rcvDoorLock_Off_OK = false; //EQ <- QC
        public static bool rcvDoorLock_Off_NG = false; //EQ <- QC
        public static bool rcvJobChangeOK = false; //EQ <- QC
        public static bool rcvJobChangeNG = false; //EQ <- QC

        public static bool rcvEmgStopACK = false; //EQ <- QC
        public static bool rcvBuzzerOn = false; //EQ <- QC EQ쪽 버저를 울리고 5초뒤 자동으로 Off 한다.
        public static bool rcvBuzzerOff = false; //EQ <- QC EQ쪽 버저를 즉시 Off 한다.

        public static bool rcvStatusXYZ = false; //EQ <- QC
        public static bool rcvResultQuery = false; //EQ <- QC
        public static bool rcvQCreqBypass = false; //EQ <- QC

        public static bool rcvQCresultOK = false; //EQ <- QC
        public static bool rcvQCresultNG = false; //EQ <- QC
        public static bool rcvQCresultDU = false; //EQ <- QC
        public static bool rcvPermission = false;
        public static bool rcvQCDoorLockOnOK = false;
        public static bool rcvQCDoorLockOnNG = false;
        public static bool rcvQCDoorLockOffOK = false;
        public static bool rcvQCDoorLockOffNG = false;

        public static bool rcvQCJobChangeOK = false;
        public static bool rcvQCJobChangeNG = false;

        public static string[] sStringInfo = new string[10];

        public static int nQCeqpStatus = 0; //EQ <- QC 
        public static int nQCCheckDelay = 0; //
        

        //0 : 초기화를 진행하지 않은 초기 상태
        //1 : 초기화 진행중
        //2 : STOP 상태
        //3 : AutoRun 상태
        //4 : ManualRun 상태
        //5 : Error 발생 상태

        public static int nQCexistStrip = 0; //EQ <- QC
        //0 : 존재하지 않는다. (Work End)
        //1 : 존재한다

        public static int nMgzMoving = 0; //EQ <- QC 
        //0 : 간섭이 없는 안전한 상태이다.
        //1 : 배출 동작중으로 간섭이 발생할 수 있는 상태이다

        public static int nGet_QcVisionPermissionCnt = 0;
        public static int nQcVisionPermissionTO = 2 * 30;//30 초 


        // 2021-02-02, jhLee : for QC control 각종 상수 선언
        public const int eMGZ_NONE = 0;                // 없다.
        public const int eMGZ_GOOD = 1;                // Good Magazine, 
        public const int eMGZ_NG = 2;                  // NG Magazine
        public const int eQCSend_None = 0;              // QC에서 전송이 아직 없음
        public const int eQCSend_Start = 1;             // QC에서 전송 시작
        public const int eQCSend_End = 2;               // QC에서 전송 완료
        public const int eQCSend_Abort = 3;             // QC에서 전송 취소

        // QC에서 전송해오는 interface용 값
        public static int m_nReadyReqMgz = 0;          // QC가 요청한 Magazine 종료 
        public static int m_nMgzReady = 0;             // Magazine에 Strip을 담을 준비가 되어있는가 ? 0:안됨, 1:양품 준비완료, 2:불량품 준비완료
        public static int m_nQCSend = 0;               // QC가 전송하는 과정 및 결과
        // end of jhLee


        public CQcVisionCom() // 생성시 초기화
        {
        
        }

        public static void initQCcomVar()
        {

            sndEQAutoStop = false;
            sndEQAbortTransfer = false;//EQ -> QC 

            rcvEQSendRequst = false; //EQ <- QC 
            rcvEQReadyQueryOK = false; //EQ <- QC 
            rcvEQSendOutACK = false; //EQ <- QC 
            rcvEQSendEndACK = false; //EQ <- QC 
            rcvQCSendRequest = false; //EQ <- QC 
            rcvQCSendRequestACK = false; //
            rcvQCReadyQueryNG = false; //EQ <- QC 
            rcvQCReadyQueryGOOD = false;
            rcvQCSendOutGOOD = false; //EQ <- QC
            rcvQCSendOutNG = false; //EQ <- QC
            rcvQCSendEnd = false; //EQ <- QC
            rcvQCAbortTransfer = false; //EQ <- QC
            rcvAutoRunACK = false; //EQ <- QC
            rcvAutoStopACK = false; //EQ <- QC
            rcvErrorResetACK = false; //EQ <- QC

            rcvLotEndACK = false; //EQ <- QC
            rcvDoorLock_On_OK = false; //EQ <- QC
            rcvDoorLock_On_NG = false; //EQ <- QC

            rcvDoorLock_Off_OK = false; //EQ <- QC
            rcvDoorLock_Off_NG = false; //EQ <- QC
            rcvJobChangeOK = false; //EQ <- QC
            rcvJobChangeNG = false; //EQ <- QC

            rcvEmgStopACK = false; //EQ <- QC
            rcvBuzzerOn = false; //EQ <- QC EQ쪽 버저를 울리고 5초뒤 자동으로 Off 한다.
            rcvBuzzerOff = false; //EQ <- QC EQ쪽 버저를 즉시 Off 한다.

            rcvStatusXYZ = false; //EQ <- QC
            rcvResultQuery = false; //EQ <- QC
            rcvQCreqBypass = false;

            rcvQCDoorLockOnOK = false; //EQ <- QC
            rcvQCDoorLockOnNG = false;//EQ <- QC
            rcvQCDoorLockOffOK = false; //EQ <- QC
            rcvQCDoorLockOffNG = false;//EQ <- QC

            rcvQCJobChangeOK = false;
            rcvQCJobChangeNG = false;
        }
    }
}
