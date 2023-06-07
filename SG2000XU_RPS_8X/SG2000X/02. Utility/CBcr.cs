using System;
using System.Diagnostics;

namespace SG2000X
{
    public class CBcr : CStn<CBcr>
    {
        //20200311 jhc : Add 2D Vision(UDP) Interface for BCR
        #region UDP Interface
        //UDP Interface Error
        public const string     ERROR_UDP_REQ       = "ERROR : BCR UdpRequest() : ";    //UDP Main >> Vision 으로의 Command 전송 실패 시 CData.m_sBcrLog 에 저장하는 string 헤더
        //Socket
        public CUdpSocket       udpServer           = null;                             //UDP 소켓
        public bool             bUdpOpen            = false;                            //UDP 서버 소켓 생성 여부

        //Vision
        public const string     VISION_PATH_NAME    = "D:\\SG2_VISION\\bin\\IntelligentFactory.exe"; //Default Value : Full Path Name of 2D Vision SW
        public const string     VISION_PROCESS_NAME = "IntelligentFactory";             //Default Value : Process Name of 2D Vision SW //IMPL : 2D Vision SW Process 명 확인하여 적용하기
        public const int        TIMEOUT_MILLISEC    = 30000;                            //Default Value : 2D Vision Response Timeout (milliseconds)
        public const string     VISION_TIME_FORMAT  = "yyyyMMdd-HH:mm:ss.fff";          //Time field format of 2D Vision Report/Response Data //20200314 jhc : 

        public string           s2DVisFullPathName  = "";                               //Full Path Name of 2D Vision SW
        public string           s2DVisProcessName   = "";                               //Process Name of 2D Vision SW
        public int              iTimeout            = 0;                                //2D Vision Response Timeout (milliseconds)

        public eBcrUdpStatus    eBcrCommStatus      = eBcrUdpStatus._99_None;           //UDP 통신 상황                 : Vision SW로부터의 수신 데이터에 의해 현재 값이 결정됨
        //20200314 jhc : Vision SW Alarm
        public string           s2DVisAlarmMsg      = "";                               //2D Vision SW 알람 메시지 => 각 시퀀스에서 Status가 Alarm(4)인 경우 참고하여 에러 팝업 필요
        public bool             b2DVisAlarmNotify   = false;                            //2D Vision SW 알람 발생 시 Error 팝업 발생시켜야 하는지 여부
        //
        public bool             bTryCheckAlive      = true;                             //주기적 Report가 없거나 Response Timeout 발생 시 Alive Check 신호를 보낼 것인지 여부

        //Main ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string           sUdpRecipe          = "";                               //UDP 서버 정보::현재 Recipe      : Main과 Vision 사이의 Recipe 동기화 확인 용
        public const string     UDP_REQID_FORMAT    = "yyyyMMdd-HHmmss.fff";            //UDP Request ID format //20200314 jhc : 
        public string           sUdpReqId           = "";                               //UDP 송신 정보::Last Request ID  : 전송 Command와 수신 데이터 매칭 용
        public eBcrCommand      eUdpCmd             = eBcrCommand.None;                 //UDP 송신 정보::Last Command     : 수신 데이터 처리 분기 용
        public DateTime         dtUdpRcvLast        = DateTime.Now;                     //UDP 마지막 수신 시간             : Vision SW로부터의 수신 데이터 Timeout 체크 용 
        public bool             bUdpResult          = false;                            //UDP 수신 정보::Last Result      : 마지막 Command 회신 결과, true=정상수신, false=비정상수신
        //.........................................................................................................................................................................
        #endregion
        //

        private CBcr()
        {
            //20200311 jhc : 2D Vision(UDP) interface
            if(CDataOption.BcrInterface == eBcrInterface.Udp) this.UdpLoad2DVisionConfig(); //2D Vision 환경 설정 변수 로딩
            //
        }

        public bool Chk_Run()
        {
            Load();
            //return CData.Bcr.bRun;
            return CData.Bcr.bRun && CData.Bcr.bCon; //190211 ksg : 옵션 추가
        }

        public string Chk_Bcr(bool bBcr = false)
        {
            CData.Bcr.bTrigger = true;
            CData.Bcr.bBcr = (!CData.Dev.bBcrSkip) ? true : false;
            CData.Bcr.bOcr = (!CData.Dev.bOcrSkip) ? true : false;
            CData.Bcr.bOri = false; 
            Save();

#if true //201013 jhc : UDP(한솔루션) OCR 연동 추가
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if ((!CData.Dev.bBcrSkip) && (CData.Dev.bOcrSkip))
                {//2D코드 판독
                    if(UdpRequest(eBcrCommand.BcrInr, "") != 0)
                    {
                        //ERROR : Request Command Fail [2D Read : In-Rail]
                    }
                }
                else if ((CData.Dev.bBcrSkip) && (!CData.Dev.bOcrSkip))
                {//OCR 판독
                    if(UdpRequest(eBcrCommand.OcrInr, "") != 0)
                    {
                        //ERROR : Request Command Fail [2D Read : In-Rail]
                    }
                }
                else if ((!CData.Dev.bBcrSkip) && (!CData.Dev.bOcrSkip))
                {//2D코드, OCR 동시 판독
                    //if(UdpRequest(eBcrCommand.BcrOcrInr, "") != 0)
                    //{
                    //    //ERROR : Request Command Fail [2D Read : In-Rail]
                    //}

                    // 201104 jym : BCR 측정 후 실패 시 OCR 측정 
                    if (!bBcr)
                    {
                        if (UdpRequest(eBcrCommand.BcrInr, "") != 0)
                        {
                            //ERROR : Request Command Fail [2D Read : In-Rail]
                        }
                    }
                    else
                    {
                        if (UdpRequest(eBcrCommand.OcrInr, "") != 0)
                        {
                            //ERROR : Request Command Fail [2D Read : In-Rail]
                        }
                    }
                }   
            }
#else
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.BcrInr, "") != 0)
                {
                    //ERROR : Request Command Fail [2D Read : In-Rail]
                }               
            }
            //
#endif

            return CData.Bcr.sBcr;
        }
        //200129 ksg :
        public void Chk_Bcr_InR(bool bBcr = false)
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bBcr_Ir = true;
            else CData.Bcr.bBcr_Ir = false;
            Save_New();

#if true //201013 jhc : UDP(한솔루션) OCR 연동 추가
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if ((!CData.Dev.bBcrSkip) && (CData.Dev.bOcrSkip))
                {//2D코드 판독
                    if(UdpRequest(eBcrCommand.BcrInr, "") != 0)
                    {
                        //ERROR : Request Command Fail [2D Read : In-Rail]
                    }
                }
                else if ((CData.Dev.bBcrSkip) && (!CData.Dev.bOcrSkip))
                {//OCR 판독
                    if(UdpRequest(eBcrCommand.OcrInr, "") != 0)
                    {
                        //ERROR : Request Command Fail [2D Read : In-Rail]
                    }
                }
                else if ((!CData.Dev.bBcrSkip) && (!CData.Dev.bOcrSkip))
                {//2D코드, OCR 동시 판독
                    //if(UdpRequest(eBcrCommand.BcrOcrInr, "") != 0)
                    //{
                    //    //ERROR : Request Command Fail [2D Read : In-Rail]
                    //}

                    // 201104 jym : BCR 측정 후 실패 시 OCR 측정 
                    if (!bBcr)
                    {
                        if (UdpRequest(eBcrCommand.BcrInr, "") != 0)
                        {
                            //ERROR : Request Command Fail [2D Read : In-Rail]
                        }
                    }
                    else
                    {
                        if (UdpRequest(eBcrCommand.OcrInr, "") != 0)
                        {
                            //ERROR : Request Command Fail [2D Read : In-Rail]
                        }
                    }
                }   
            }
#else
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.BcrInr, "") != 0)
                {
                    //ERROR : Request Command Fail [2D Read : In-Rail]
                }           
            }
            //
#endif
        }
        //200129 ksg :
        public void Chk_Bcr_LTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bBcr_Gl = true;
            else CData.Bcr.bBcr_Gl = false;
            Save_New();

            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.BcrGL, "") != 0)
                {
                    //ERROR : Request Command Fail [2D Read : Left-Table]
                }               
            }
            //
        }
        //200129 ksg :
        public void Chk_Bcr_RTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bBcr_Gr = true;
            else CData.Bcr.bBcr_Gr = false;
            Save_New();

            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.BcrGR, "") != 0)
                {
                    //ERROR : Request Command Fail [2D Read : Right-Table]
                }               
            }
            //
        }
        //200129 ksg :
        public void Chk_Ocr_InR()
        {
            SetTriValFalse();
            if (!CData.Dev.bOcrSkip) CData.Bcr.bOcr_Ir = true;
            else CData.Bcr.bOcr_Ir = false;
            Save_New();

            //201013 jhc : UDP(한솔루션) OCR 연동 추가
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OcrInr, "") != 0)
                {
                    //ERROR : Request Command Fail [OCR Read : In-Rail]
                }           
            }
            //
        }

        //200129 ksg :
        public void Chk_Ocr_LTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bOcrSkip) CData.Bcr.bOcr_Gl = true;
            else CData.Bcr.bOcr_Gl = false;
            Save_New();

            //201013 jhc : UDP(한솔루션) OCR 연동 추가
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OcrGL, "") != 0)
                {
                    //ERROR : Request Command Fail [OCR Read : Left-Table]
                }           
            }
            //
        }

        //200129 ksg :
        public void Chk_Ocr_RTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bOcrSkip) CData.Bcr.bOcr_Gr = true;
            else CData.Bcr.bOcr_Gr = false;
            Save_New();

            //201013 jhc : UDP(한솔루션) OCR 연동 추가
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OcrGR, "") != 0)
                {
                    //ERROR : Request Command Fail [OCR Read : Right-Table]
                }           
            }
            //
        }

        public string Chk_Ori()
        {
            CData.Bcr.bTrigger = true;
            CData.Bcr.bBcr = false; //190211 ksg :
            CData.Bcr.bOcr = false; //190309 ksg :
            CData.Bcr.bOri = true; //190211 ksg :
            CData.Bcr.bMark = true; //190712 ksg :
            Save();
            //GU.Delay(1000);
            //Load();

            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OriInr, "") != 0)
                {
                    //ERROR : Request Command Fail [Orientation : In-Rail]
                }
                return CData.Bcr.bRetOri.ToString(); //20200314 jhc:
            }
            //

            return CData.Bcr.sBcr;
        }

        //200129 ksg :
        public void Chk_Ori_InR()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bOri_Ir = true;
            else CData.Bcr.bOri_Ir = false;
            Save_New();

            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OriInr, "") != 0)
                {
                    //ERROR : Request Command Fail [Orientation : In-Rail]
                }               
            }
            //
        }
        //200129 ksg :
        public void Chk_Ori_LTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bOri_Gl = true;
            else CData.Bcr.bOri_Gl = false;
            Save_New();

            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OriGL, "") != 0)
                {
                    //ERROR : Request Command Fail [Orientation : Left-Table]
                }               
            }
            //
        }
        //200129 ksg :
        public void Chk_Ori_RTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bOri_Gr = true;
            else CData.Bcr.bOri_Gr = false;
            Save_New();

            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.OriGR, "") != 0)
                {
                    //ERROR : Request Command Fail [Orientation : Right-Table]
                }               
            }
            //
        }

        //200129 ksg :
        public void Chk_Mark_InR()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bMark_Ir = true;
            else CData.Bcr.bMark_Ir = false;
            Save_New();
        }
        //200129 ksg :
        public void Chk_Mark_LTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bMark_Gl = true;
            else CData.Bcr.bMark_Gl = false;
            Save_New();
        }
        //200129 ksg :
        public void Chk_Mark_RTable()
        {
            SetTriValFalse();
            if (!CData.Dev.bBcrSkip) CData.Bcr.bMark_Gr = true;
            else CData.Bcr.bMark_Gr = false;
            Save_New();
        }
        public void Load()
        {
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Ini)
            {
                string sSec = "";
                string sRun, sCon, sRet, sRetOri, sRetTOri, sRetOcr, sRetCom, sStatus;
                string sRetMark; //190712 ksg :

	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Run";
	            //CData.Bcr.bRun = Convert.ToBoolean(mIni.ReadInt(sSec, "bRun"));
	            //CData.Bcr.bCon = Convert.ToBoolean(mIni.ReadInt(sSec, "bCon"));

	            sRun = mIni.Read(sSec, "bRun");
	            sCon = mIni.Read(sSec, "bCon");

	            if(sRun == "1" || sRun == "True" || sRun == "true") CData.Bcr.bRun = true ;
	            else                                                CData.Bcr.bRun = false;

	            if(sCon == "1" || sCon == "True" || sCon == "true") CData.Bcr.bCon = true ;
	            else                                                CData.Bcr.bCon = false;


	            sSec = "Ret";
	            //CData.Bcr.bRet    = Convert.ToBoolean(mIni.ReadInt(sSec, "bRet"));
	            //CData.Bcr.bRetOri = Convert.ToBoolean(mIni.ReadInt(sSec, "bOri"));
            
	            sRet     = mIni.Read(sSec, "bRet"    );
	            sRetOri  = mIni.Read(sSec, "bOri"    );
	            sRetTOri = mIni.Read(sSec, "bCTOri"  );
	            sRetOcr  = mIni.Read(sSec, "bOCR"    ); //190309 ksg :
	            sRetCom  = mIni.Read(sSec, "bCompare"); //190309 ksg :
	            sRetMark = mIni.Read(sSec, "bMarked" ); //190712 ksg :

	            if(sRet == "1" || sRet == "True" || sRet == "true") { CData.Bcr.bRet = true ; CData.m_sBcrLog = "1"; }
	            else                                                { CData.Bcr.bRet = false; CData.m_sBcrLog = "0"; }
                              
	            if(sRetOri == "1"  || sRetOri == "True"  || sRetOri  == "true") { CData.Bcr.bRetOri = true ; CData.m_sBcrLog += ",1"; }
	            else                                                            { CData.Bcr.bRetOri = false; CData.m_sBcrLog += ",0"; }
                
                // 아래로 이동, Skyworks 처리 내용 참조
                //if (sRetOcr == "1" || sRetOcr == "True" || sRetOcr == "true") { CData.Bcr.bRetOcr = true; CData.m_sBcrLog += ",1"; }
                //else { CData.Bcr.bRetOcr = false; CData.m_sBcrLog += ",0"; }

                //200118 ksg :
                if (sRetTOri == "1" || sRetTOri == "True" || sRetTOri == "true") { CData.Bcr.bRetTOri = true ; CData.m_sBcrLog += ",1"; }
	            else                                                            { CData.Bcr.bRetTOri = false; CData.m_sBcrLog += ",0"; }

	            //190309 ksg :
	            if(sRetCom == "1"  || sRetCom == "True"  || sRetCom  == "true") { CData.Bcr.bRetCom = true ; CData.m_sBcrLog += ",1"; }
	            else                                                            { CData.Bcr.bRetCom = false; CData.m_sBcrLog += ",0"; }

	            //190309 ksg :
	            if(sRetMark == "1" || sRetMark == "True" || sRetMark == "true") { CData.Bcr.bRetMark = true ; CData.m_sBcrLog += ",1"; }
	            else                                                            { CData.Bcr.bRetMark = false; CData.m_sBcrLog += ",0"; }
            
	            CData.Bcr.sBcr    = mIni.Read(sSec, "sTrigger");
	            CData.Bcr.sOcr    = mIni.Read(sSec, "sOCR"    );


                // 2021-01-25, jhLee : Skyworks VOC, OCR 판독 글자수가 지정 수치 이외라면 읽기 오류로 판정
                if (CData.CurCompany == ECompany.SkyWorks)
                {
                    CData.Bcr.sOcr.Trim();                  // 앞뒤 공백을 자른다.
                    CData.Bcr.sOcr.Replace(" ", "");        // 중간의 공백을 제거한다. 기예르모氏 제안

                    // 2022-05-26, jhLee : OCR 인식 글자수가 10자리 혹은 14자리로 체계가 변경된다.
                    if ((CData.Bcr.sOcr.Length != 10) && (CData.Bcr.sOcr.Length != 14))      // Skyworks에서는 10글자 or 14글자가 아니면 잘못된 Strip ID로 간주 
                    {
                        // 읽기에 실패했다고 판단한다.
                        CData.Bcr.bRetOcr = false;
                        CData.m_sBcrLog += ",0";
                    }
                    else // 글자수가 일치하면 판독 결과 신호를 인용한다.
                    { 
                        if (sRetOcr == "1" || sRetOcr == "True" || sRetOcr == "true") { CData.Bcr.bRetOcr = true; CData.m_sBcrLog += ",1"; }
                        else { CData.Bcr.bRetOcr = false; CData.m_sBcrLog += ",0"; }
                    }
                }
                else // Skyworks가 아닐경우 기존 루틴을 사용한다.
                {
                    // 글자수가 일치하면 판독 결과 신호를 인용한다.
                    if (sRetOcr == "1" || sRetOcr == "True" || sRetOcr == "true") { CData.Bcr.bRetOcr = true; CData.m_sBcrLog += ",1"; }
                    else { CData.Bcr.bRetOcr = false; CData.m_sBcrLog += ",0"; }
                }



                //190827 ksg : 추가
                sSec    = "Status";
	            sStatus = mIni.Read(sSec, "iBCR");

	            if(sStatus == "1" || sStatus == "True" || sStatus == "true") { CData.Bcr.bStatusBcr = true ; CData.m_sBcrLog += ",1"; }
	            else                                                         { CData.Bcr.bStatusBcr = false; CData.m_sBcrLog += ",0"; }

	            //sSec = "Trigger";
	            //mIni.Write(sSec, "bTrigger", "False");            
            }        
        }

        //190617 ksg :
        public void Save_RunStatus()
        {
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Ini)
            {
	            string sSec = "";
	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Run";
	            mIni.Write(sSec, "bRun", "False");
            }
        }

        public void Save()
        {
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Ini)
            {
	            string sSec = "";
	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Ret";
	            mIni.Write(sSec, "sTrigger", "error");
	            mIni.Write(sSec, "sOCR"    , "error"); //190309 ksg : ocr 추가
	            //mIni.Write(sSec, "bRet", 0);
	            //mIni.Write(sSec, "bOri", 0);
	            mIni.Write(sSec, "bRet"    , "False");
	            mIni.Write(sSec, "bOri"    , "False");
	            mIni.Write(sSec, "bOCR"    , "False"); //190309 ksg : ocr 추가
	            mIni.Write(sSec, "bCompare", "False"); //190309 ksg : ocr 추가
	            mIni.Write(sSec, "bMarked" , "False"); //190712 ksg : ocr 추가
            
	            sSec = "Trigger";
	            mIni.Write(sSec, "bBCR"    , CData.Bcr.bBcr    .ToString());
	            mIni.Write(sSec, "bOri"    , CData.Bcr.bOri    .ToString());
	            mIni.Write(sSec, "bCTOri"  , CData.Bcr.bTOri   .ToString()); //200118 ksg :
	            mIni.Write(sSec, "bOCR"    , CData.Bcr.bOcr    .ToString());
	            mIni.Write(sSec, "bMarked" , CData.Bcr.bMark   .ToString());
	            mIni.Write(sSec, "bTrigger", CData.Bcr.bTrigger.ToString()); //190712 ksg : Trigger는 항상 마지막에 저장 해야 됨

	            CData.m_sBcrLog = "BCR Save()" + CData.Bcr.bBcr    .ToString() + ", " +  
	                              CData.Bcr.bOri    .ToString() + ", " +
	                              CData.Bcr.bOcr    .ToString() + ", " +
	                              CData.Bcr.bMark   .ToString() + ", " +
	                              CData.Bcr.bTrigger.ToString();
            }
        }

        //200129 ksg :
        public void Save_New()
        {
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Ini)
            {
	            string sSec = "";
	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Ret";
	            mIni.Write(sSec, "sTrigger", "error");
	            mIni.Write(sSec, "sOCR"    , "error"); //190309 ksg : ocr 추가
	            mIni.Write(sSec, "bRet"    , "False");
	            mIni.Write(sSec, "bOri"    , "False");
	            mIni.Write(sSec, "bOCR"    , "False"); //190309 ksg : ocr 추가
	            mIni.Write(sSec, "bCompare", "False"); //190309 ksg : ocr 추가
	            mIni.Write(sSec, "bMarked" , "False"); //190712 ksg : ocr 추가
            
	            sSec = "Trigger";
	            //mIni.Write(sSec, "bBCR"    , CData.Bcr.bBcr    .ToString());
	            //mIni.Write(sSec, "bOri"    , CData.Bcr.bOri    .ToString());
	            //mIni.Write(sSec, "bCTOri"  , CData.Bcr.bTOri   .ToString()); //200118 ksg :
	            //mIni.Write(sSec, "bOCR"    , CData.Bcr.bOcr    .ToString());
	            //mIni.Write(sSec, "bMarked" , CData.Bcr.bMark   .ToString());
	            //mIni.Write(sSec, "bTrigger", CData.Bcr.bTrigger.ToString()); //190712 ksg : Trigger는 항상 마지막에 저장 해야 됨
	            mIni.Write(sSec, "bBCR_IR"   , CData.Bcr.bBcr_Ir .ToString());
	            mIni.Write(sSec, "bOri_IR"   , CData.Bcr.bOri_Ir .ToString());
	            mIni.Write(sSec, "bOCR_IR"   , CData.Bcr.bOcr_Ir .ToString()); //200118 ksg :
	            mIni.Write(sSec, "bMarked_IR", CData.Bcr.bMark_Ir.ToString());
	            mIni.Write(sSec, "bBCR_GL"   , CData.Bcr.bBcr_Gl .ToString());
	            mIni.Write(sSec, "bOri_GL"   , CData.Bcr.bOri_Gl .ToString()); //190712 ksg : Trigger는 항상 마지막에 저장 해야 됨
	            mIni.Write(sSec, "bOCR_GL"   , CData.Bcr.bOcr_Gl .ToString());
	            mIni.Write(sSec, "bMarked_GL", CData.Bcr.bMark_Gl.ToString());
	            mIni.Write(sSec, "bBCR_GR"   , CData.Bcr.bBcr_Gr .ToString()); //200118 ksg :
	            mIni.Write(sSec, "bOri_GR"   , CData.Bcr.bOri_Gr .ToString());
	            mIni.Write(sSec, "bOCR_GR"   , CData.Bcr.bOcr_Gr .ToString());
	            mIni.Write(sSec, "bMarked_GR", CData.Bcr.bMark_Gr.ToString()); //190712 ksg : Trigger는 항상 마지막에 저장 해야 됨
            }
        }

        public void SaveRecipe(string Path, int nDigitType=0)
        {
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.ChangeRecipe, Path) != 0)
                {
                    //ERROR : Request Command Fail [Recipe Change]
                }
            }
            else
            {
	            string sSec = "";
	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Recipe";
	            mIni.Write(sSec, "sDX", Path);
                mIni.Write(sSec, "nDigitType", $"{nDigitType}");    // OCR 자릿수
            }
        }

        public void LoadRecipe()
        {
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Ini)
            {
	            string sSec = "";
	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Recipe";
	            CData.Bcr.sDX  = mIni.Read(sSec, "sDX" );
	            CData.Bcr.sBCR = mIni.Read(sSec, "sBCR");
                CData.Bcr.iDigitType = mIni.ReadI(sSec, "nDigitType");      // OCR 자릿수
            }
        }

        public void SaveStatus(bool bExit)
        {
            string sSec = "";
            string sLv = "";
            if (!bExit)
            {
                if (CData.Lev == ELv.Operator)
                { sLv = "0"; }
                else
                { sLv = "1"; }
            }
            else
            {
                sLv = "0";
            }
            //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                if(UdpRequest(eBcrCommand.ChangeUserLevel, sLv) != 0)
                {
                    //ERROR : Request Command Fail [Change User Level]
                }
            }
            else
            {
	            CIni mIni = new CIni(GV.PATH_EQ_BCR + "Trigger.ini");
	            sSec = "Status";
	            mIni.Write(sSec, "iDXLv", sLv);
            }
        }
        //
        public void SetTriValFalse()
        {
            CData.Bcr.bTrigger = false;
            CData.Bcr.bBcr     = false;
            CData.Bcr.bOcr     = false;
            CData.Bcr.bOri     = false;
            CData.Bcr.bTOri    = false;
            CData.Bcr.bMark    = false;
            //---------------------------
            CData.Bcr.bBcr_Ir  = false;
            CData.Bcr.bOri_Ir  = false;
            CData.Bcr.bOcr_Ir  = false;
            CData.Bcr.bMark_Ir = false;
            CData.Bcr.bBcr_Gl  = false;
            CData.Bcr.bOri_Gl  = false;
            CData.Bcr.bOcr_Gl  = false;
            CData.Bcr.bMark_Gl = false;
            CData.Bcr.bBcr_Gr  = false;
            CData.Bcr.bOri_Gr  = false;
            CData.Bcr.bOcr_Gr  = false;
            CData.Bcr.bMark_Gr = false;
        }


        //20200311 jhc : UDP interface
        #region UDP Interface for [ Main <<-->> Vision ]
        /// <summary>
        /// 2D Vision Configuration Loading (D:\Suhwoo\SG2000X\Equip\Bcr\2DVision.ini)
        /// </summary>
        public void UdpLoad2DVisionConfig()
        {
            CIni mIni = new CIni(GV.PATH_EQ_BCR + "2DVision.ini");
            string sSec = "Process";
            //Full Path Name of 2D Vision SW
            s2DVisFullPathName = mIni.Read(sSec,"FullPathName");
            if(string.IsNullOrWhiteSpace(s2DVisFullPathName)) s2DVisFullPathName = CBcr.VISION_PATH_NAME;
            //Process Name of 2D Vision SW
            s2DVisProcessName = mIni.Read(sSec,"ProcessName");
            if(string.IsNullOrWhiteSpace(s2DVisProcessName)) s2DVisProcessName = CBcr.VISION_PROCESS_NAME;

            sSec = "Communication";
            //Response Timeout
            int.TryParse(mIni.Read(sSec, "Timeout"), out this.iTimeout);
            if(this.iTimeout == 0) this.iTimeout = CBcr.TIMEOUT_MILLISEC;
        }

        public int UdpOpen()
        {
            int result = 0;

            //내부 관리 정보 초기화 : Recipe
            this.UdpInitRecipe();
            //UDP Server Socket 
#if true //20200423 jhc : 수신데이터 콜백 처리
            this.udpServer = new CUdpSocket(UdpUpdateBcrData); //UDP 소켓 생성 : 수신 데이터 처리용 콜백함수 전달
#else
            this.udpServer = new CUdpSocket(); //UDP 소켓 생성
#endif
            result = this.udpServer.Server();  //UDP Server 설정 및 수신 시작
            //UDP 통신 상태 초기화
            if(result == 0) this.UdpClearStatus(true);
            else            this.UdpClearStatus(false);

            return result;
        }

        public void UdpClose()
        {
            //UDP 서버 소켓 닫기
            if((this.udpServer != null) && this.bUdpOpen) this.udpServer.Close();
            //UDP 통신 상태 초기화
            this.UdpClearStatus(false);
        }

        private void UdpClearStatus(bool bSvrOn)
        {
            this.bUdpOpen = bSvrOn;         //UDP 서버 소켓 상태
            this.UdpClearVisionStatus();    //통신 상태 및 2D Vision SW 상태 초기화
            this.UdpClearRequestInfo();     //Main 측 Request 정보 초기화
        }

        /// <summary>
        /// 통신 상태 및 2D Vision SW 상태 변수 초기화
        /// 2D Vision SW가 실행 중이지 않거나, 응답 시간 타임아웃 발생 시 >> 2D Vision 상태 변수 초기화
        /// </summary>
        private void UdpClearVisionStatus()
        {
            ///////////////////////////////////////////////////////////////////
            //1) 통신 상태 초기화
            if(this.bUdpOpen)   this.eBcrCommStatus = eBcrUdpStatus._98_SvrOn;
            else                this.eBcrCommStatus = eBcrUdpStatus._99_None;
            //2) Vision SW Ready(0) 상태 : Main >> Vision Command 전송 불가 상태
            CData.Bcr.bCon = false;
            ///////////////////////////////////////////////////////////////////
        }

        private void UdpInitRecipe()
        {
            this.sUdpRecipe = CDev.It.m_sGrp + "\\" + CData.Dev.sName;
            CData.Bcr.sDX = this.sUdpRecipe;
        }

        /// <summary>
        /// 현재 실행중인 프로세스를 검토해서 이미 실행 중이지 않은 경우 지정한 full path name의 프로그램을 실행함
        /// </summary>
        public void UdpRun2DVisionProcess()
        {
            if(!UdpIsRun2DVision()) Process.Start(this.s2DVisFullPathName); //2D Vision SW 실행
        }

        /// <summary>
        /// 지정된 프로세스 명으로 실행중 여부 확인
        /// </summary>
        /// <returns></returns>
        private bool UdpIsRun2DVision()
        {
            Process[] ProcessList = Process.GetProcessesByName(this.s2DVisProcessName);
            if(ProcessList.Length >= 1) return true;
            else                        return false;               
        }

        /// <summary>
        /// 2D Vision SW가 실행 중인지?, 주기 Reporting을 잘 하고 있는지 체크
        /// 만약 주기 Reporting을 하고 있지 않다면, Alive Check Command 전송
        /// 본 함수는 frmMain._Cycle() 내에서 반복 실행되어, 실시간 모니터링의 효과를 얻는다.
        /// </summary>
        /// <param name="iMS">2D Vision SW로부터 응답 timeout</param>
        public void UdpMonitor2DVision(int iMS=60000)
        {
            bool bRun = UdpIsRun2DVision();
            /////////////////////////////////////////////////////
            CData.Bcr.bRun = bRun; //내부 관리 변수 실시간 업데이트
            /////////////////////////////////////////////////////
            if(bRun)
            {
                //타임아웃 체크
                DateTime dtTimeout = this.dtUdpRcvLast.Add(new TimeSpan(0, 0, 0, 0, iTimeout)); //마지막 응답 시간 + 타임아웃 기간
                if(dtTimeout < DateTime.Now)
                {
                    /////////////////////////////////
                    // 2D Vision SW의 응답 타임아웃 //

                    //1) 2D Vision 상태 초기화
                    UdpClearVisionStatus();
                    //2) Alive Check Command 전송
                    if(this.bTryCheckAlive)
                    {
                        if(this.UdpRequest(eBcrCommand.ChkAlive, "") != 0)
                        {
                            //ERROR : Request Command Fail [Check Alive]
                        }
                        //3) Check Alive 신호 전송 플래그 리셋 (Check Alive Command는 1회만 전송)
                        this.bTryCheckAlive = false; //여기서 이 플래그 리셋하지 않으면 매 Timeout 지속될 경우 매 주기마다 Check Alive Command 전송함
                    }
                    //4) 마지막 응답 시간 초기화 : 타임아웃 발생 시 Alive Check Command를 1회만 보내기 위해 마지막 응답시간을 초기화함
                    this.dtUdpRcvLast = DateTime.Now;

                    return;
                }
                else
                {
                    //////////////////////////////////////
                    // 2D Vision SW의 응답 타임아웃 아님 //

                    //주기적 응답에 의한 내부 관리 변수 체크 : 2D Vision SW의 현재 상태 체크
                    if(CData.Bcr.bCon)
                    {
                        //정상 상태 : Not Ready(0) >> Train(1) or Auto(2) or Running(3) or Alarm(4)
                    }
                    else
                    {
                        //Ready(0) 상태 : Main >> Vision Command 전송 불가 상태
                    }
                }
            }
            else
            {
                ////////////////////////////////
                // 2D Vision SW 실행 상태 아님 //

                //1) 2D Vision 상태 초기화
                UdpClearVisionStatus(); //this.eBcrCommStatus, CData.Bcr.bCon
            }
        }

        /// <summary>
        /// Main 측 Request 정보 초기화
        /// </summary>
        private void UdpClearRequestInfo()
        {
            //Main 측 정보 초기화 ////////////////////////////////
            this.sUdpReqId = "";              //요청 Command 없음
            this.eUdpCmd = eBcrCommand.None;  //요청 Command 없음
            this.bUdpResult = false;          //요청 Command 없음
            /////////////////////////////////////////////////////
        }

        /// <summary>
        /// Main >> Vision 명령 전송
        /// </summary>
        /// <param name="cmd">전송할 Command 번호: 1     = Recipe Change
        ///                                       2     = Auto Mode 전환
        ///                                       3     = InRail Orient 검사
        ///                                       4     = Left Table Orient 검사
        ///                                       5     = Right Table Orient 검사
        ///                                       6     = InRail 2D 검사
        ///                                       7     = Left Table 2D 검사
        ///                                       8     = Right Table 2D 검사
        ///                                       81    = User Level 변경 (0/1)
        ///                                       82    = UI Mode 변경 (0/1)
        ///                                       99    = Alive 체크</param>
        /// <param name="strData">Recipe Name</param>
        /// <returns></returns>
        public int UdpRequest(eBcrCommand cmd, string strData="")
        {
            int result = 0;
            int ret = 0;

            CData.m_sBcrLog = ""; //Clear Bcr Log

            //0) Main 측 Request 정보 초기화 ////////////////
            this.UdpClearRequestInfo();
            //..............................................

            //1) Command별 검사 결과 정보 초기화 ////////////
            switch(cmd)
            {
                case eBcrCommand.OriInr:
                    CData.Bcr.bRetOri = false;
                    break;
                case eBcrCommand.OriGL:
                case eBcrCommand.OriGR:
                    CData.Bcr.bRetOri = false;
                    CData.Bcr.bRetTOri = false;
                    break;
#if true //201013 jhc : UDP(한솔루션) OCR 연동 추가
                case eBcrCommand.BcrInr:
                case eBcrCommand.BcrGL:
                case eBcrCommand.BcrGR:
                case eBcrCommand.OcrInr:
                case eBcrCommand.OcrGL:
                case eBcrCommand.OcrGR:
                case eBcrCommand.BcrOcrInr:
                case eBcrCommand.BcrOcrGL:
                case eBcrCommand.BcrOcrGR:
                    CData.Bcr.bRet = false;
                    CData.Bcr.sBcr = "";
                    CData.Bcr.sOcr = "";
#else
                case eBcrCommand.BcrInr:
                case eBcrCommand.BcrGL:
                case eBcrCommand.BcrGR:
                    CData.Bcr.bRet = false;
                    CData.Bcr.sBcr = "";
#endif
                    break;
            }
            ///////////////////////////////////////////////

            //2) 전송할 데이터 체크
            if((cmd == eBcrCommand.ChangeRecipe) || (cmd == eBcrCommand.ChangeUserLevel) || (cmd == eBcrCommand.ChangeUI))
            {
                if(String.IsNullOrWhiteSpace(strData))
                {
                    CData.m_sBcrLog = ERROR_UDP_REQ + "Invalid Command Format [CMD=" + cmd.ToString() + ", DATA=" + strData + "]";
                    return (-1);
                }
                else if(cmd == eBcrCommand.ChangeRecipe)
                {
                    //Recipe Change인 경우 내부 변수 갱신 ///////////////////////////////////////////////////////////
                    this.sUdpRecipe = strData;  //UDP I/F에서 사용하는 Main 측 정보 설정
                    CData.Bcr.sDX = strData;    //기존 INI I/F에서 사용하는 Main 측 Recipe 정보도 함께 업데이트해야 함
                    ///////////////////////////////////////////////////////////////////////////////////////////////
                }
            }

            //3) 서버 소켓 체크
            if((this.udpServer == null) || (!this.bUdpOpen))
            {
                CData.m_sBcrLog = ERROR_UDP_REQ + "UDP Server not Opened [CMD=" + cmd.ToString() + ", DATA=" + strData + "]";
                return (-1);
            }

            //4) Command String 구성
            tUdpSendData sndData = new tUdpSendData();
            sndData.sReqID = DateTime.Now.ToString(UDP_REQID_FORMAT);  //4-1) Request ID (=현재시간) //20200314 jhc : "yyyyMMdd-HHmmss.fff"
            sndData.iCmd = (int)cmd;                                   //4-2) Command 번호
            sndData.sData = strData;                                   //4-3) Recipe Name (Recipe Change Command(1)인 경우에 유효)
            string strCmd = "";
            ret = this.udpServer.MakeSendData(sndData, ref strCmd);
            if(ret != 0)
            {
                CData.m_sBcrLog = ERROR_UDP_REQ + "CUdpSocket.MakeSendData() Fail [CMD=" + cmd.ToString() + ", DATA=" + strData + "]";
                return (-1);
            }

            //5) Command 전송
            result = udpServer.SendSync(strCmd);
            if(result == 0)
            {
                //5-OK) Main 측 정보 설정 //////////////////////////////////////////////////////////////////////////////////////
                this.sUdpReqId = sndData.sReqID;  //Last Request ID   : 전송 Command와 수신 데이터 매칭 용
                this.eUdpCmd = cmd;               //Last Command      : 수신 데이터 처리 분기 용
                this.bUdpResult = false;          //Last Result       : 마지막 Command 회신 결과, true=정상수신, false=비정상수신
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //20200314 jhc : Request Command 송신 후 Vision SW 상태(내부 관리 변수) Running으로 변경 ////////////////////////
                CData.Bcr.bStatusBcr = true; //Not eBcrUdpStatus._2_Auto
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            else
            {   //result == (-1)
                CData.m_sBcrLog = ERROR_UDP_REQ + "CUdpSocket.SendSync() Fail [CMD=" + cmd.ToString() + ", DATA=" + strData + "]";
            }

            return result;
        }

        //20200314 jhc :
        /// <summary>
        /// Request Command 시점과 주기 응답 시점의 시간차가 미묘할 경우,
        /// Main에서는 Command를 보냈으나, 미처 Vision SW의 Running Status로 변경되기 전 Auto 상태로 주기 Reporting을 수신하여
        /// Main SW에서 Vision SW의 검사완료 상태로 오인할 경우가 발생할 수 있음
        /// Request Command 후 Response 수신 전, 특정 시간 이내의 주기 송신 정보 여부를 체크하여 무시할 수 있도록 확인하는 기능
        /// </summary>
        /// <param name="rcvData">수신 데이터 문자열</param>
        /// <param name="tmMs">Command 송신 이 후 주기 리포팅을 무시할 시간(milliseconds)</param>
        /// <returns></returns>
        private bool UdpIsDiscardReport(tUdpRcvData rcvData, int tmMs)
        {
            bool bResult = false;

            if ((rcvData.sReqId == "0")/*주기 리포팅 메시지*/ && (!string.IsNullOrWhiteSpace(this.sUdpReqId))/*Command Request 상태*/)
            {
                DateTime dtReq = DateTime.Now;
                DateTime dtRcv = DateTime.Now;
                if(DateTime.TryParseExact(this.sUdpReqId, UDP_REQID_FORMAT, null, 0, out dtReq))
                {
                    if(DateTime.TryParseExact(rcvData.sTime, VISION_TIME_FORMAT, null, 0, out dtRcv))
                    {
                        if(dtRcv < dtReq.AddMilliseconds(tmMs)) bResult = true; //Command Request 송신 후 아직 적당 시간이 지나지 않아 무시할 주기 리포팅입니다.
                    }
                }
            }
            return bResult;
        }
        //

        /// <summary>
        /// UDP 수신 데이터를 근거로 Bcr 클래스 내부 관리 정보 구조체 변수(CData.Bcr) 업데이트
        /// 검사 결과 정보는 요청 시 Request ID와 수신 시 Request ID 값이 일치할 경우에만 업데이트 함
        /// </summary>
        /// <param name="rcvData">Vision SW로부터 수신 데이터를 파싱한 정보 구조체</param>
        /// <returns>0: Command Response (정상) / 1: 주기적 Report / -1: Command Respose (오류)</returns>
        public int UdpUpdateBcrData(tUdpRcvData rcvData)
        {
            int result = 0;

            //마지막 수신 시간 업데이트 ////////
            this.dtUdpRcvLast = DateTime.Now;
            //////////////////////////////////

            //Vision SW 상태 1)
#if false
            CData.Bcr.bRun      = true; //데이터 수신 되었으므로 Vision SW 실행 중인 상태 //UdpMonitor2DVision()에서 처리하므로 여기서 처리하지 않음
#endif
            CData.Bcr.bCon      = true;  //데이터 수신이 되었으므로 Vision SW 연결은 정상 상태 (주기 Report 등이 없어서 Timeout 발생 시 false로 전환됨) 
            //

            //20200314 jhc : Request Command 송신 후 Response 수신 전, 지정 시간 이내의 주기 리포트인 경우 무시 //////////
            if(UdpIsDiscardReport(rcvData, 1000)) return (1); //1000 ms = 1 s 
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            //UDP 통신 상황 업데이트 //////////////////////////////
            //20200314 jhc : 2D Vision SW 알람 상태 체크
            if((this.eBcrCommStatus != eBcrUdpStatus._4_Alarm) && (rcvData.iStatus == (int)eBcrUdpStatus._4_Alarm))
            {
                /////////////////////////////////////////
                // Vision SW : Normal 상태 > Alarm 상태 //
                this.s2DVisAlarmMsg = rcvData.sAlarmMsg;
                this.b2DVisAlarmNotify = true;
            }
            else if(rcvData.iStatus != (int)eBcrUdpStatus._4_Alarm)
            {
                ////////////////////////////////
                // Vision SW : Not Alarm 상태 //
                this.b2DVisAlarmNotify = false; //이전 상태와 무관하게 Alarm 해제 : Vision 동작 전 Alarm 발생 > Alarm 해제 전환된다면 설비 SW에서는 모르고 지나감!!!
                this.s2DVisAlarmMsg = "";
            }
            //
            this.eBcrCommStatus = (eBcrUdpStatus)rcvData.iStatus;
            //////////////////////////////////////////////////////

            //Vision SW 상태 2)

#if true //20200401 jhc : 2D-Vision SW에서 RUN 가능 상태 별도 체크
            CData.Bcr.bStatusBcr= ( rcvData.iStatus == (int)eBcrUdpStatus._0_Ready ||               //Ready 상태  : 검사 Command 전송 시 Auto Mode 전환 후 검사
                                    rcvData.iStatus == (int)eBcrUdpStatus._1_Train ||               //Train 상태  : 검사 Command 전송 시 Auto Mode 전환 후 검사
                                    rcvData.iStatus == (int)eBcrUdpStatus._2_Auto) ? false : true;  //Auto 상태   : 검사 Command 전송 시 검사 후 회신
#else
            CData.Bcr.bStatusBcr= (rcvData.iStatus == (int)eBcrUdpStatus._2_Auto) ? false : true;   //Auto 상태에서만 검사 요청 Command 전송 가능
#endif
            CData.Bcr.iUserLevel= rcvData.iULevel;  //User Level
            CData.Bcr.iUIMode   = rcvData.iUiMode;  //현재 화면

            //Recipe
#if false
            CData.Bcr.sDX       = ""; //Main SW 측 Recipe는 Device 변경 적용 시 Main SW 측에 SaveRecipe() Call 통해 직접 설정
#endif
            CData.Bcr.sBCR = rcvData.sRecipe; //Vision SW 측 Recipe

            if(CData.Bcr.bCon && !this.bTryCheckAlive) this.bTryCheckAlive = true; //Check Alive 신호 전송 플래그 리셋 : 2D Vision SW 타임아웃 발생 시 Check Alive Command 1회 전송할 수 있도록 준비함

#if false //UDP I/F에서 미사용
            //Trigger 정보
            CData.Bcr.bTrigger  = false;
            CData.Bcr.bOri      = false;
            CData.Bcr.bBcr      = false;
            CData.Bcr.bOcr      = false;
            CData.Bcr.bTOri     = false;
            CData.Bcr.bMark     = false;
            //
            CData.Bcr.bBcr_Ir   = false;
            CData.Bcr.bOri_Ir   = false;
            CData.Bcr.bOcr_Ir   = false;
            CData.Bcr.bMark_Ir  = false;
            CData.Bcr.bBcr_Gl   = false;
            CData.Bcr.bOri_Gl   = false;
            CData.Bcr.bOcr_Gl   = false;
            CData.Bcr.bMark_Gl  = false;
            CData.Bcr.bBcr_Gr   = false;
            CData.Bcr.bOri_Gr   = false;
            CData.Bcr.bOcr_Gr   = false;
            CData.Bcr.bMark_Gr  = false;
#endif

            //CASE 1: 주기적 Report ("0")
            if (rcvData.sReqId == "0")
            {
                //추가 설정 항목 없음
                result = (1); //주기적 report
            }
            //CASE 2: Command 전송의 결과로 받은 회신 ("yyyyMMdd-HHmmss.fff")
            else
            {
                if (this.sUdpReqId == rcvData.sReqId)
                {
                    //검사 결과 정보 : Command 전송 시 Request ID와 Response 수신 시 Request ID가 동일한 경우에만 업데이트 해야 함 /////////////////////////////////////////
                    //20200314 jhc : Command 전송과 관련된 검사 결과 값만 참조하여 업데이트함
                    if((this.eUdpCmd == eBcrCommand.OriInr) || (this.eUdpCmd == eBcrCommand.OriGL) || (this.eUdpCmd == eBcrCommand.OriGR))
                    {
                        CData.Bcr.bRetOri   = (rcvData.iOriResult == 1) ? true : false;
                        if((this.eUdpCmd == eBcrCommand.OriGL) || (this.eUdpCmd == eBcrCommand.OriGR)) CData.Bcr.bRetTOri = CData.Bcr.bRetOri;
                    }
                    else if((this.eUdpCmd == eBcrCommand.BcrInr) || (this.eUdpCmd == eBcrCommand.BcrGL) || (this.eUdpCmd == eBcrCommand.BcrGR))
                    {
                        CData.Bcr.bRet      = (rcvData.i2DResult == 1) ? true : false;
                        CData.Bcr.sBcr      = rcvData.s2DVal;
                    }
#if true //201013 jhc : UDP(한솔루션) OCR 연동 추가
                    else if((this.eUdpCmd == eBcrCommand.OcrInr) || (this.eUdpCmd == eBcrCommand.OcrGL) || (this.eUdpCmd == eBcrCommand.OcrGR))
                    {
                        CData.Bcr.bRetOcr   = (rcvData.iOcrResult == 1) ? true : false;
                        CData.Bcr.sOcr      = rcvData.sOcrVal;
                    }
                    else if((this.eUdpCmd == eBcrCommand.BcrOcrInr) || (this.eUdpCmd == eBcrCommand.BcrOcrGL) || (this.eUdpCmd == eBcrCommand.BcrOcrGR))
                    {
                        CData.Bcr.bRet      = (rcvData.i2DResult == 1) ? true : false;
                        CData.Bcr.sBcr      = rcvData.s2DVal;
                        CData.Bcr.bRetOcr   = (rcvData.iOcrResult == 1) ? true : false;
                        CData.Bcr.sOcr      = rcvData.sOcrVal;
                    }
#else
                    CData.Bcr.bRetOcr   = (rcvData.iOcrResult == 1) ? true : false;
                    CData.Bcr.sOcr      = rcvData.sOcrVal;
#endif

#if false //UDP I/F에서 미사용
                    CData.Bcr.bRetCom   = false;
                    CData.Bcr.bRetMark  = false;
#endif
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Main 측 정보 설정 : 이 값의 true 설정은 반드시 상기 검사 결과 정보 설정보다 나중에 해야 함 ///////////////
                    this.bUdpResult = true; //요청한 Command에 대해 정상 수신 받음 (통신 관점 OK, 인식 결과는 별도 확인해야 함)
                    //////////////////////////////////////////////////////////////////////////////////////////////////////

                    result = (0); //Command Response (정상)
                }
                else
                {
                    //Garbage
                    //요청한 Request ID와 수신한 Request ID가 상이함 -> 오류
                    //IMPL 1) : 오류 로그? (UDP Log 참고하면 됨)
                    //IMPL 2) : 잘못된 데이터가 수신되면 Command를 재 송신해야 하나? (Timeout 걸림)

                    //Main 측 정보 설정 ////////////////////////////////////////////////////////////
                    this.bUdpResult = false; //요청한 Command에 대한 수신 데이터 비정상 (통신 관점 NG)
                    ///////////////////////////////////////////////////////////////////////////////

                    result = (-1); //Command Respose (오류)
                }
                //Main 측 정보 초기화 ////////////////////////////////////////////
                this.sUdpReqId = "";              //수신 완료 >> 요청 Command 없음
                this.eUdpCmd = eBcrCommand.None;  //수신 완료 >> 요청 Command 없음
                /////////////////////////////////////////////////////////////////
            }

            return result;
        }

        /// <summary>
        /// Command Response 정상 완료 여부 체크
        /// </summary>
        /// <returns>0: Response 완료(정상), 1: Response 미완료, -1: Response 완료(비정상) 또는 Command Request 없었음/returns>
        public int UdpCheckRvcComplete()
        {
            int result = 0;

            if(this.bUdpResult)             result = (0);   //Response 완료(정상)
            else if(this.sUdpReqId == "")   result = (-1);  //Response 완료(비정상) 또는 Command Request 없었음
            else                            result = (1);   //Response 미완료

            return result;
        }
#endregion
        //

        //20200314 jhc : Auto Run -> Request 2D Vision SW Auto Mode
        /// <summary>
        /// 2D Vision SW가 Auto Mode가 아닌 경우 Auto Mode 전환 요청
        /// AutoRun() 시작 시점에 1회 call하도록 함
        /// </summary>
        public void UdpSet2DVisionAutoMode()
        {
            if(this.eBcrCommStatus != eBcrUdpStatus._2_Auto )
            {
                if(UdpRequest(eBcrCommand.ChangeAutoMode, "") != 0)
                {
                    //ERROR : Request Command Fail [Change Auto Mode]
                } 
            }
        }
    }
}
