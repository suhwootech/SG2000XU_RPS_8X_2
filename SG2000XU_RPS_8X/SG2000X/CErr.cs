using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// 2020.12.09 JSKim St
using Microsoft.VisualBasic.FileIO;
// 2020.12.09 JSKim Ed

namespace SG2000X
{
public static class CErr
    {
        // private static frmErr mForm;
        private static CTim m_Kill = new CTim();
        // 2020.12.09 JSKim St
        // Error 발생 시간 동기화... 시간은 동기화는 할 수 있지만
        // 현재는 Error 발생 중 다른 번호 Error 발생 시 다른 Error로 넘어간다...
        // 따라 Error 별 Reset 시간에 대해서 구분할 수 없다..
        private static DateTime dtLastErrOccurTime = new DateTime();
        private static object lockObj_Err = new object();
        private static object lockObj_LotErr = new object();
        // 2020.12.09 JSKim Ed

        public static void Show(eErr eErr)
        {
            if (CData.Opt.bQcSimulation) return;		// 2021-01-? , YYY, for QC Simulation Test
			
            // 2020.12.07 JSKim St
            // 실제로 필요할지는 모르겠음.. Data File Load 후 모듈 연결을 확인해야 하는데
            // WMX 연결 확인이 먼저 되기 때문에 Error 처리에 문제가 있음
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.ErrList[(int)eErr].sName == null)
                {
                    Load();
                    LoadRadar();
                }
            }
            // 2020.12.07 JSKim Ed

            //mForm = new frmErr(eErr);
            string sTemp = "";
            sTemp = LoadLastErr();
            //190410 ksg :
            CSQ_Man.It.bBtnShow = true;
            if(sTemp != CData.ErrList[(int)eErr].sNo)
            {//현재 발생 한 에러가 마지막 에러와 다른 경우 신규 에러
                // 2020.12.09 JSKim St
                //SaveLastErr(eErr);
                //210115 jhc : error 반복 처리 중 발생 오류(hang-up) 에 대한 임시 디버깅
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)  { SaveErrLogLock(eErr); }
                else                                                                        { SaveErrLog(eErr);     }
                SaveLastErr(eErr);
                // 2020.12.09 JSKim Ed
                CLog.mLogSeq(string.Format("현재 발생 한 에러가 마지막 에러와 다른 경우 신규 에러 {0})", eErr));
            }
            else
            {//현재 발생 한 에러가 마지막 에러와 같은 경우 에러 조치 후 장비 돌리다가 같은에러가 다시 뜬경우
                if (!IsRelease())
                {//마지막 에러 조치 시간이 입력 되있는경우 신규 에러
                    // 2020.12.09 JSKim St
                    //210115 jhc : error 반복 처리 중 발생 오류(hang-up) 에 대한 임시 디버깅
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)  { SaveErrLogLock(eErr); }
                    else                                                                        { SaveErrLog(eErr);     }
                    // 마지막 Error 시간 갱신
                    SaveLastErr(eErr);
                    // 2020.12.09 JSKim Ed
                }
            }

            // 2020.12.10 JSKim St
            // 원래 Error 중에 타 Error이 들어와도 Log도 안찍고 Count도 안해야 되는 거 아닌가???
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.LotInfo.bLotOpen && CSQ_Main.It.m_iStat != EStatus.Error)
                {
                    CData.SpcInfo.iErrCnt++;
                    SaveErrLog_LotLock(eErr);
                }
            }
            // 2020.12.10 JSKim Ed

            CSQ_Man.It.Seq      = ESeq.Idle;
            CSQ_Main.It.m_iStat = EStatus.Error;

            CData.iErrNo        = (int)eErr;

            //jsck secsgem
            if (CData.GemForm != null)
            {
                CData.GemForm.Set_AlarmOccur((int)eErr + 1);
            }

            CData.ShowErrForm   = true;
            CData.IsErr         = eErr;
            //190801 ksg :
            //f(CTcpIp.It.bIsConnect)
            if(CGxSocket.It.IsConnected())
            {
                //CQcVisionCom.nQCeqpStatus 
                //0 : 초기화를 진행하지 않은 초기 상태
                //1 : 초기화 진행중
                //2 : STOP 상태
                //3 : AutoRun 상태
                //4 : ManualRun 상태
                //5 : Error 발생 상태

                if (CQcVisionCom.nQCeqpStatus == 3)
                {
                    //CTcpIp.It.SendError();
                    CGxSocket.It.SendMessage("AutoStop");
                }
            }
            //CSQ_Main.It.m_SendQCDelay.Set_Delay(500);

            // 2020.12.07 JSKim St
            // SPC에 Error Count와 맞추기 위해 그냥 둠
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.ErrList[CData.iErrNo].bRadarUse == true && CData.ErrList[CData.iErrNo].iRadarOptionCnt != 0)
                {
                    CData.ErrList[CData.iErrNo].iRadarErrorCnt++;
                }

                SaveRadar();
            }
            // 2020.12.07 JSKim Ed
        }

        public static string GetStr(int iErr)
        {
            return ((eErr)iErr).ToString();
        }

        public static void Load()
        {
            int iCnt = 0;
            string sRowVal = "";
            string sPath = GV.PATH_ERR;
            string[] sHearder;
            //string[] sData;
            string[] sData = new String[10];//191008 윈도우 에러
            string[] sTemp;//191008 윈도우 에러
            FileInfo fi;
            StreamReader sr;

            if ((int)ELang.Korea == CData.Opt.iSelLan)
            {
                sPath += "err_Kr.csv";
            }
            else if ((int)ELang.China == CData.Opt.iSelLan)
            {
                sPath += "err_Ch.csv";
            }
            else
            {
                sPath += "err.csv";
            }

            fi = new FileInfo(sPath);

            if(fi.Exists)
            {
                //koo 191101 : error window 
                try
                {
                    string filename = Path.GetFileName(sPath);
                    if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                    sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                    sRowVal = sr.ReadLine();
                    //koo 191106 error split
                    if (sRowVal==null)
                    {
                        string temp = sPath + " is Empty"; 
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return ;
                    }
                    sHearder = sRowVal.Split(',');

                    //2020.04.13 lks err.csv 에러 라인체크
                    string errLines = string.Empty;

                    while(sr.Peek() > -1)
                    {
                        sRowVal = sr.ReadLine();
                        if ((int)eErr.ERR_MAXCOUNT > iCnt)
                        {
                            try
                            {
                                //sData = sRowVal.Split(',');
                                //koo 191106 error split
                                if (sRowVal == null)
                                {
                                    string temp = sPath + " is Empty";
                                    CMsg.Show(eMsg.Error, "Error", temp);
                                    return;
                                }
                                sTemp = sRowVal.Split(',');//191008 윈도우 에러
                                                           //191008 윈도우 에러

                                //2020.04.13 lks
                                sData = new string[sTemp.Length];
                                for (int i = 0; i < sTemp.Length; i++)
                                {
                                    sData[i] = sTemp[i];
                                }

                                // 2020.12.07 JSKim St
                                // err.csv 파일 안에 Radar 정보가 들어 있으면 안된다. Update 할 수가 없다...(사용자가 조작하는 Data를 왜 여기 넣냐....)
                                // 전 고객사 통합으로 사용할 수 있게 수정
                                ///// <summary>
                                ///// Max 2020103 : SCK+ Rader & Error Text Modify
                                ///// </summary>
                                //// 2020.10.26 JSKim St
                                ////if (CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200625 lks
                                //// 2020.11.26 JSKim St
                                //// JCET는 기존 Error 파일을 이용했기 때문에 Rader System을 사용했을리 없다....
                                ////if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200625 lks
                                //if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                                //// 2020.11.26 JSKim Ed
                                //// 2020.10.26 JSKim Ed
                                //{
                                //    CData.ErrList[iCnt].sNo = sData[0];
                                //    if (sData[1].Length < 5) //2020.04.13 lks csv 오류검증
                                //    {
                                //        errLines += "[" + (iCnt + 1) + "] " + sData[1] + " - Errorload failed:Error Name" + "\r\n";
                                //    }
                                //    CData.ErrList[iCnt].sName = sData[1];
                                //    CData.ErrList[iCnt].sAction = sData[2];
                                //    int.TryParse(sData[3], out CData.ErrList[iCnt].iRaderUseCnt);
                                //    int.TryParse(sData[4], out CData.ErrList[iCnt].iRaderCounter);
                                //    int.TryParse(sData[5], out CData.ErrList[iCnt].iJamCount);
                                //    //CData.ErrList[iCnt].iRaderUseCnt = sData[3];
                                //    //CData.ErrList[iCnt].iRaderCounter = sData[4];
                                //    //CData.ErrList[iCnt].iJamCount = sData[5];
                                //    CData.nTotalJamCount = CData.nTotalJamCount + CData.ErrList[iCnt].iJamCount; // MTBA 계산에 사용.
                                //    CData.ErrList[iCnt].sSPCFlag = sData[6];
                                //    CData.ErrList[iCnt].sParts = sData[7];
                                //}
                                //else
                                //{
                                //    CData.ErrList[iCnt].sNo = sData[0];
                                //    if (sData[1].Length < 5) //2020.04.13 lks csv 오류검증
                                //    {
                                //        errLines += "[" + (iCnt + 1) + "] " + sData[1] + " - Errorload failed:Error Name" + "\r\n";
                                //    }
                                //    CData.ErrList[iCnt].sName = sData[1];
                                //    CData.ErrList[iCnt].sAction = sData[2];
                                //    CData.ErrList[iCnt].iRaderUseCnt = 0;
                                //    CData.ErrList[iCnt].iRaderCounter = 0;
                                //    CData.ErrList[iCnt].iJamCount = 0;
                                //    CData.nTotalJamCount = 0;
                                //    CData.ErrList[iCnt].sSPCFlag = "";
                                //    CData.ErrList[iCnt].sParts = "";
                                //}
                                CData.ErrList[iCnt].sNo = sData[0];

                                if (sData[1].Length < 5)
                                {
                                    errLines += "[" + (iCnt + 1) + "] " + sData[1] + " - Errorload failed:Error Name" + "\r\n";
                                }

                                CData.ErrList[iCnt].sName = sData[1];
                                CData.ErrList[iCnt].sAction = sData[2];
                                
								if(CData.CurCompany == ECompany.ASE_KR)
								{
									CData.ErrList[iCnt].sKorName = sData[3];    // 2021.09.06 SungTae : [추가] (ASE-KR VOC) Error명 한글 표시 요청
								}
								
                                if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)
                                {
                                    CData.ErrList[iCnt].bRadarUse = false;
                                    CData.ErrList[iCnt].iRadarOptionCnt = 0;
                                    CData.ErrList[iCnt].iRadarErrorCnt = 0;
                                }
                                // 2020.12.07 JSKim Ed
                            }
                            catch (Exception lineErr)
                            {
                                errLines += "[" + (iCnt+1) +"] "+lineErr.Message+"\r\n";
                            }                       
                        }
                        iCnt++;
                    }

                    if (!string.IsNullOrEmpty(errLines))
                    {
                        errLines = "ErrorList load failed\r\n" + errLines;
                        CMsg.Show(eMsg.Error, "Error", errLines);
                        sr.Close();
                        File.Copy(fi.FullName,fi.FullName + "_failed", false);
                    }
                    else
                    {
                        sr.Close();
                    }
                }
                catch (Exception ex)
                {
                    CMsg.Show(eMsg.Error, "Error", "ErrorList file load failed:" + ex.Message);
                    return ;
                }
            }
        }

        /// <summary>
        /// Max 2020103 : SCK+ Rader & Error Text Modify
        /// </summary>
        public static void SaveErr()
        {
            int iCnt = 0;
            string sPath = GV.PATH_ERR;
            string sTemp;
            StreamWriter sw;

            if ((int)ELang.Korea == CData.Opt.iSelLan)
            {
                sPath += "err_Kr.csv";
            }
            else if ((int)ELang.China == CData.Opt.iSelLan)
            {
                sPath += "err_Ch.csv";
            }
            else
            {
                sPath += "err.csv";
            }

            try
            {
                string filename = Path.GetFileName(sPath);
                
                if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                
                FileStream FS = new FileStream(sPath, FileMode.Truncate, FileAccess.Write);
                
                sw = new StreamWriter(FS, Encoding.GetEncoding("euc-kr"));

                // 2021.09.06 SungTae Start : [수정] (ASE-KR VOC) Error명 한글 표시 요청
                if(CData.CurCompany == ECompany.ASE_KR)
                    sw.WriteLine("No,Name,Action,Name(KOR)");
                else
                    sw.WriteLine("No,Name,Action");
                // 2021.09.06 SungTae End

                while (iCnt != (int)eErr.ERR_MAXCOUNT)
                {
                    // 2021.09.06 SungTae Start : [수정] (ASE-KR VOC) Error명 한글 표시 요청
                    if (CData.CurCompany == ECompany.ASE_KR)
                    {
                        sTemp = CData.ErrList[iCnt].sNo + "," +
                                CData.ErrList[iCnt].sName + "," +
                                CData.ErrList[iCnt].sAction + "," +
                                CData.ErrList[iCnt].sKorName;
                    }
                    else
                    {
                        sTemp = CData.ErrList[iCnt].sNo + "," +
                                CData.ErrList[iCnt].sName + "," +
                                CData.ErrList[iCnt].sAction;
                    }
                    // 2021.09.06 SungTae End

                    sTemp = sTemp.Replace("\r\n", "*");
                    sw.WriteLine(sTemp);
                    iCnt++;
                }
                // 2020.12.07 JSKim Ed

                sw.Close();
                FS.Close();
            }
            catch (Exception ex)
            {
                CMsg.Show(eMsg.Error, "Error", ex.Message);
                return;
            }

        }

        // 2020.12.07 JSKim St
        public static void LoadRadar()
        {
            int iCnt = 0;
            string sRowVal = "";
            string sPath = GV.PATH_ERR;
            string[] sHearder;
            string[] sData = new String[10];
            string[] sTemp;
            FileInfo fi;
            StreamReader sr;

            sPath += "radar.csv";

            fi = new FileInfo(sPath);

            if (fi.Exists)
            {
                try
                {
                    string filename = Path.GetFileName(sPath);
                    if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                    sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                    sRowVal = sr.ReadLine();

                    if (sRowVal == null)
                    {
                        string temp = sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Radar", temp);
                        //SaveRadar();
                        return;
                    }
                    sHearder = sRowVal.Split(',');

                    string errLines = string.Empty;

                    while (sr.Peek() > -1)
                    {
                        sRowVal = sr.ReadLine();
                        if ((int)eErr.ERR_MAXCOUNT > iCnt)
                        {
                            try
                            {
                                if (sRowVal == null)
                                {
                                    string temp = sPath + " is Empty";
                                    CMsg.Show(eMsg.Error, "Radar", temp);
                                    //SaveRadar();
                                    return;
                                }
                                sTemp = sRowVal.Split(',');

                                sData = new string[sTemp.Length];
                                for (int i = 0; i < sTemp.Length; i++)
                                {
                                    sData[i] = sTemp[i];
                                }
                                
                                CData.ErrList[iCnt].bRadarUse       = Convert.ToBoolean(sData[1]);
                                CData.ErrList[iCnt].iRadarOptionCnt = Convert.ToInt32(sData[2]);
                                CData.ErrList[iCnt].iRadarErrorCnt  = Convert.ToInt32(sData[3]);
                            }
                            catch (Exception lineErr)
                            {
                                errLines += "[" + (iCnt + 1) + "] " + lineErr.Message + "\r\n";
                            }
                        }
                        iCnt++;
                    }

                    if (!string.IsNullOrEmpty(errLines))
                    {
                        errLines = "RadarList load failed\r\n" + errLines;
                        CMsg.Show(eMsg.Error, "Error", errLines);
                        sr.Close();
                        File.Copy(fi.FullName, fi.FullName + "_failed", false);
                    }
                    else
                    {
                        sr.Close();
                    }
                }
                catch (Exception ex)
                {
                    CMsg.Show(eMsg.Error, "Error", "RadarList file load failed:" + ex.Message);
                    return;
                }
            }
        }

        public static void SaveRadar()
        {
            int iCnt = 0;
            string sPath = GV.PATH_ERR;
            string sTemp;
            StreamWriter sw;

            sPath += "radar.csv";

            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                FileStream FS = new FileStream(sPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(FS, Encoding.GetEncoding("euc-kr"));

                sw.WriteLine("Code,Radar Function,Set Data Count,Current Data Count");

                while (iCnt != (int)eErr.ERR_MAXCOUNT)
                {
                    sTemp = CData.ErrList[iCnt].sNo + "," +
                            CData.ErrList[iCnt].bRadarUse + "," +
                            CData.ErrList[iCnt].iRadarOptionCnt + "," +
                            CData.ErrList[iCnt].iRadarErrorCnt;

                    sTemp = sTemp.Replace("\r\n", "*");
                    sw.WriteLine(sTemp);
                    iCnt++;
                }
                sw.Close();
                FS.Close();
            }
            catch (Exception ex)
            {
                CMsg.Show(eMsg.Error, "Radar", ex.Message);
                return;
            }

        }
        // 2020.12.07 JSKim Ed

        // 2020.12.07 JSKim St
        ///// <summary>
        ///// Max 2020103 : SCK+ Rader & Error Text Modify
        ///// </summary>
        //public static void RaderSetModify(int CellNo, bool UseFlag, String RaderCnt)
        //{
        //    int iCnt = 0;
        //    string sPath = GV.PATH_ERR;
        //    string sTemp;
        //    StreamWriter sw;

        //    if ((int)ELang.Korea == CData.Opt.iSelLan)
        //    {
        //        sPath += "err_Kr.csv";
        //    }
        //    else if ((int)ELang.China == CData.Opt.iSelLan)
        //    {
        //        sPath += "err_Ch.csv";
        //    }
        //    else
        //    {
        //        // Max 2020103 : SCK+ Rader & Error Text Modify
        //        //if (CData.CurCompany == ECompany.JSCK)
        //        //{
        //        //    sPath += "err - Rader.csv";
        //        //}
        //        //else
        //        //{
        //            sPath += "err.csv";
        //        //}
        //    }

        //    try
        //    {
        //        CData.ErrList[CellNo].iRaderUseCnt = 0;
        //        if (UseFlag)
        //        { int.TryParse(RaderCnt, out CData.ErrList[CellNo].iRaderUseCnt); }
        //        CData.ErrList[CellNo].iRaderCounter = 0;


        //        string filename = Path.GetFileName(sPath);
        //        if (CLog.killps(filename) == true) m_Kill.Wait(2000);
        //        FileStream FS = new FileStream(sPath, FileMode.Truncate, FileAccess.Write);
        //        sw = new StreamWriter(FS, Encoding.GetEncoding("euc-kr"));
        //        sw.WriteLine("No,Rader Use,Radar Counter,JamCount,SPC Flag,Parts,Name,Action");
        //        while (iCnt != (int)eErr.ERR_MAXCOUNT)
        //        {
        //            sTemp = CData.ErrList[iCnt].sNo + "," +
        //                    CData.ErrList[iCnt].sName + "," +
        //                    CData.ErrList[iCnt].sAction + "," +
        //                    CData.ErrList[iCnt].iRaderUseCnt + "," +
        //                    CData.ErrList[iCnt].iRaderCounter + "," +
        //                    CData.ErrList[iCnt].iJamCount + "," +
        //                    CData.ErrList[iCnt].sSPCFlag + "," +
        //                    CData.ErrList[iCnt].sParts;

        //            sTemp = sTemp.Replace("\r\n", "*");
        //            sw.WriteLine(sTemp);
        //            iCnt++;
        //        }
        //        sw.Close();
        //        FS.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        CMsg.Show(eMsg.Error, "Error", ex.Message);
        //        return;
        //    }
        //}
        // 2020.12.07 JSKim Ed

        /// <summary>
        /// 마지막 에러 번호 and 시간 저장
        /// </summary>
        /// <param name="bWrite"></param>
        public static void SaveLastErr(eErr eErr)
        {
            string sPath = GV.PATH_ERRLOG;
            string sLine = "";         
        
            FileInfo fi;
            DirectoryInfo di;
            //koo 191002 : Speed Write StreamWriter sw;

            di = new DirectoryInfo(sPath);

            if (!di.Exists)
            { di.Create(); }

            sPath += "LastErr.csv";
            fi = new FileInfo(sPath);

            if (!fi.Exists)
            { 
                fi.Create().Close(); 
                GU.Delay(50);
            }

            // 2020.12.09 JSKim St - 마지막 Error 시간도 저장
            //sLine = CData.ErrList[(int)eErr].sNo;
            sLine = CData.ErrList[(int)eErr].sNo + ",";
            sLine += dtLastErrOccurTime.ToString("yyyy-MM-dd HH:mm:ss");
            // 2020.12.09 JSKim Ed

            CLog.Check_File_Access(sPath,sLine,false);
        }

        /// <summary>
        /// 마지막 에러 번호 or 시간 불러오기
        /// </summary>
        /// <returns></returns>
        // 2020.12.09 JSKim St
        //public static string LoadLastErr()
        public static string LoadLastErr(bool bGetTime = false)
        // 2020.12.09 JSKim Ed
        {
            string sPath = GV.PATH_ERRLOG;
            string sRet = "";
            // 2020.12.09 JSKim St
            string[] sTemp;
            // 2020.12.09 JSKim Ed

            FileInfo fi;
            DirectoryInfo di;
            StreamReader sr;

            di = new DirectoryInfo(sPath);
            if (!di.Exists)
            { return sRet; }
            sPath += "LastErr.csv";
            fi = new FileInfo(sPath);

            if (!fi.Exists)
            { return sRet; }

            //koo 191101 : error window 
            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                sRet = sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex)
            {
                CMsg.Show(eMsg.Error, "Error", ex.Message);
                return "";
            }

            // 2020.12.09 JSKim St
            sTemp = sRet.Split(',');
            if ( bGetTime == true )
            {
                if (sTemp.Length > 1)
                {
                    sRet = sTemp[1];
                }
                else
                {
                    sRet = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            else
            {
                sRet = sTemp[0];
            }
            // 2020.12.09 JSKim Ed

            return sRet;
        }

        // 2020.12.09 JSKim St - Error 발생과 Reset이 따로 동작하기 때문에 ErrLog.csv 에 쓰는데 문제가 있다;;
        // 최소한으로만 막자... 다 막기에는 시간 부족
        public static void SaveErrLogLock(eErr eErr)
        {
            lock (lockObj_Err)
            {
                if ((int)eErr == -1)
                {
                    SaveErrRelease();
                }
                else
                {
                    SaveErrLog(eErr);
                }
            }
        }
        // 2020.12.09 JSKim Ed

        /// <summary>
        /// 최초 에러 발생시 저장 에러
        /// 에러번호, 에러 발생시간, 조치시간 = "NONE"
        /// </summary>
        /// <param name="eErr"></param>
        public static void SaveErrLog(eErr eErr)
        {
            string sPath = GV.PATH_ERRLOG;
            string sLine = "";

            FileInfo fi;
            DirectoryInfo di;
            //koo 191002 : Speed Write StreamWriter sw;

            // 2020.12.09 JSKim St
            //sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            dtLastErrOccurTime = DateTime.Now;

            sPath += dtLastErrOccurTime.ToString("yyyyMMdd") + "\\";
            // 2020.12.09 JSKim Ed

            di = new DirectoryInfo(sPath);

            if (!di.Exists)
            { di.Create(); }

            sPath += "ErrLog.csv";
            fi = new FileInfo(sPath);

            if (!fi.Exists)
            {
                fi.Create().Close();
                
                sLine  = "ErrNo,"     ;
                sLine += "ErrName,"   ;
                sLine += "OccurTime," ;
                sLine += "ReleaseTime";
                CLog.Check_File_Access(sPath,sLine,true);
            }
            
            sLine = CData.ErrList[(int)eErr].sNo + ",";
            sLine += CData.ErrList[(int)eErr].sName + ",";
            // 2020.12.09 JSKim St
            //sLine += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            sLine += dtLastErrOccurTime.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            // 2020.12.09 JSKim Ed
            sLine += "NONE";
            CLog.Check_File_Access(sPath,sLine,true);
        }

        /// <summary>
        /// 에러 발생 시 해제 유무 확인
        /// 조치시간 = "NONE" 일경우  true, 아닐경우 false;
        /// </summary>
        /// <returns></returns>
        public static bool IsRelease()
        {
            bool bRet = false;
            string sPath = GV.PATH_ERRLOG;
            string sLine = "";
            string[] sData = new String[10];//191008 윈도우 에러
            string[] sTemp;//191008 윈도우 에러


            FileInfo fi;
            DirectoryInfo di;
            StreamReader sr;

            // 2020.12.09 JSKim St
            //sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
#if true //210115 jhc : error 반복 처리 중 발생 오류(exception) 에 대한 임시 디버깅
            DateTime dtTemp;
            if (false == DateTime.TryParse(LoadLastErr(true), out dtTemp))
            {
                return bRet;
            }
#else
            DateTime dtTemp = DateTime.Parse(LoadLastErr(true));
#endif //..      
            sPath += dtTemp.ToString("yyyyMMdd") + "\\";
            // 2020.12.09 JSKim Ed

            di = new DirectoryInfo(sPath);

            if (!di.Exists)
            { return bRet; }

            sPath += "ErrLog.csv";
            fi = new FileInfo(sPath);

            if (!fi.Exists)
            { return bRet; }

            //koo 191101 : error window 
            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                while (sr.Peek() > -1)
                {
                    sLine = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                CMsg.Show(eMsg.Error, "Error", ex.Message);
                return false;
            }

            sTemp = sLine.Split(',');//191008 윈도우 에러
            //191008 윈도우 에러
            for (int i=0; i<sTemp.Length;i++)
            {
                sData[i] = sTemp[i];
            }

            if (sData[3] == "NONE")
            { bRet = true; }
            return bRet;
        }

        public static void SaveErrRelease()
        {
            string sPath = GV.PATH_ERRLOG;
            string sLine = "";
            //string[] sData;
            string[] sData = new String[10];//191008 윈도우 에러
            string[] sTemp;//191008 윈도우 에러
            List<string> sRelease = new List<string>();

            FileInfo fi;
            DirectoryInfo di;
            StreamReader sr;
            //koo 191002 : Speed Write StreamWriter sw;
            StreamWriter sw;
            // 2020.12.09 JSKim St
            //sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
#if true //210115 jhc : error 반복 처리 중 발생 오류(exception) 에 대한 임시 디버깅
            DateTime dtTemp;
            if (false == DateTime.TryParse(LoadLastErr(true), out dtTemp))
            {
                return;
            }
#else
            DateTime dtTemp = DateTime.Parse(LoadLastErr(true));
#endif //..
            sPath += dtTemp.ToString("yyyyMMdd") + "\\";
            // 2020.12.09 JSKim Ed

            di = new DirectoryInfo(sPath);

            if (!di.Exists)
            { return ; }

            sPath += "ErrLog.csv";
            fi = new FileInfo(sPath);

            if (!fi.Exists)
            { return ; }

            //koo 191101 : error window 
            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                while(sr.Peek() > -1)
                {
                    sLine = sr.ReadLine();
                    sRelease.Add(sLine);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                CMsg.Show(eMsg.Error, "Error", ex.Message);
                return ;
            }

            //sData = sRelease[sRelease.Count - 1].Split(',');
            sTemp = sRelease[sRelease.Count - 1].Split(',');//191008 윈도우 에러
            //191008 윈도우 에러
            for (int i=0; i<sTemp.Length;i++)
            {
            	sData[i] = sTemp[i];
            }
            if(sData[3] == "NONE")
            {
                sRelease.RemoveAt(sRelease.Count - 1);
                sData[3] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sLine = sData[0] + "," + sData[1] + "," + sData[2] + "," + sData[3];
                sRelease.Add(sLine);
                //sw = new StreamWriter(sPath, false, Encoding.GetEncoding("euc-kr"));

                /*원본 koo 191002 : Speed Write 
                sw = new StreamWriter(sPath, false, Encoding.GetEncoding("euc-kr"));
                foreach(string line in sRelease)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
                */
                //koo 191002 : Speed Write 파일을 처음 부터 쓸려고 아래 두 구문 추가.
                sw = new StreamWriter(sPath, false, Encoding.GetEncoding("euc-kr"));
                sw.Close();
                foreach (string line in sRelease)
                {
                    CLog.Check_File_Access(sPath,line,true); 
                }
            }

            //20190624 josh
            //jsck secsgem
            //alarm reset
            if (sData[0] != string.Empty)
            {
                //if (CData.GemForm != null)
                if (CData.Opt.bSecsUse && CData.GemForm != null) // 2021.07.16 lhs : CData.Opt.bSecsUse 추가
                {
                    CData.GemForm.Set_AlarmReset(Convert.ToInt32(sData[0]));
                }
            }

            
        }

        // 2020.12.10 JSKim St
        // Lot End 시에만 호출 된다..
        public static void SaveErrLog_LotInfo()
        {
            FileInfo fi;
            DirectoryInfo di;

            StreamReader sr;

            string sFromPath = GV.PATH_ERR;
            string sToPath = GV.PATH_SPC;
            string sLotName = CData.SpcInfo.sLotName;

            string sLine = "";

            string[] sTemp;
            string[] sData;

            List<string> sRelease = new List<string>();

            di = new DirectoryInfo(sFromPath);

            if (!di.Exists)
            { di.Create(); }

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00");
            }

            sFromPath += sLotName + "_ErrLog.csv";

            fi = new FileInfo(sFromPath);

            if (fi.Exists)
            {
                try
                {
                    string sFromfilename = Path.GetFileName(sFromPath);
                    if (CLog.killps(sFromfilename) == true) m_Kill.Wait(2000);
                    sr = new StreamReader(sFromPath, Encoding.GetEncoding("euc-kr"));
                    while (sr.Peek() > -1)
                    {
                        sLine = sr.ReadLine();
                        sRelease.Add(sLine);
                    }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    CMsg.Show(eMsg.Error, "Error_Lot", ex.Message);
                    return;
                }

                sToPath += "LotLog\\";
                sToPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
                sToPath += sLotName + "\\";

                di = new DirectoryInfo(sToPath);

                if (!di.Exists)
                { di.Create(); }

                sToPath += "LotErrInfo.csv";

                fi = new FileInfo(sToPath);
 
                if (fi.Exists)
                {
                    string filename = Path.GetFileName(sToPath);
                    if (CLog.killps(filename) == true) m_Kill.Wait(2000);

                    FileSystem.DeleteFile(sToPath);
                }

                fi.Create().Close();

                sLine = "Code,";
                sLine += "Err Message,";
                sLine += "Count,";
                sLine += "Time";
                CLog.Check_File_Access(sToPath, sLine, true);

                int iErrCnt = 0;
                TimeSpan tSpan = new TimeSpan();

                sRelease.RemoveAt(0);

                for (int i = 0; i <= sRelease.Count - 1; i++)
                {
                    iErrCnt = 0;
                    sData = sRelease[i].Split(',');

                    if (sData[0] == "") break;

                    // 2021.04.06 lhs Start
                    //tSpan = TimeSpan.Parse(sData[4]); // "NONE"일 경우 에러 발생하여 수정
                    if (sData[4] != "NONE")
                    {
                        TimeSpan.TryParse(sData[4], out tSpan);
                    }
                    // 2021.04.06 lhs End

                    iErrCnt++;

                    for (int j = i + 1; j <= sRelease.Count - 1; j++)
                    {
                        sTemp = sRelease[j].Split(',');

                        if (sData[0] == sTemp[0])
                        {
                            sRelease.RemoveAt(j);
                            iErrCnt++;

                            if (sTemp[4] != "NONE")
                            {
                                // 2021.04.06 lhs Start
                                //tSpan += TimeSpan.Parse(sTemp[4]);
                                TimeSpan tsTmp;
                                if (TimeSpan.TryParse(sTemp[4], out tsTmp))
                                {
                                    tSpan += tsTmp;
                                }
                                else
                                {  /*냉무*/ }
                                // 2021.04.06 lhs End
                            }
                            j--;
                        }
                    }

                    sLine = sData[0] + ",";
                    sLine += sData[1] + ",";
                    sLine += iErrCnt.ToString() + ",";
                    sLine += tSpan.ToString(@"hh\:mm\:ss");

                    CLog.Check_File_Access(sToPath, sLine, true);
                }

                FileSystem.DeleteFile(sFromPath);
            }
        }

        public static void SaveErrLog_LotLock(eErr eErr)
        {
            lock (lockObj_LotErr)
            {
                if (eErr == eErr.NONE)
                {
                    SaveErrRelease_Lot();
                }
                else
                {
                    SaveErrLog_Lot(eErr);
                }
            }
        }

        /// <summary>
        /// Lot Open 간 발생한 Error 저장 Log
        /// 에러번호, 에러 발생시간, 조치시간 = "NONE"
        /// </summary>
        /// <param name="eErr"></param>
        public static void SaveErrLog_Lot(eErr eErr, bool bRelease = false )
        {
            FileInfo fi;
            DirectoryInfo di;

            string sPath = GV.PATH_ERR;
            string sLotName = CData.SpcInfo.sLotName;
            string sLine = "";

            di = new DirectoryInfo(sPath);

            if (!di.Exists)
            { di.Create(); }

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00");
            }

            sPath += sLotName + "_ErrLog.csv";

            fi = new FileInfo(sPath);

            if (!fi.Exists)
            {
                fi.Create().Close();

                sLine = "Code,";
                sLine += "Err Message,";
                sLine += "OccurTime,";
                sLine += "ReleaseTime";
                sLine += "Time";
                CLog.Check_File_Access(sPath, sLine, true);
            }
  
            sLine = CData.ErrList[(int)eErr].sNo + ",";
            sLine += CData.ErrList[(int)eErr].sName + ",";
            sLine += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            sLine += "NONE" + ",";
            sLine += "NONE";

            CLog.Check_File_Access(sPath, sLine, true);
        }

        public static void SaveErrRelease_Lot()
        {
            FileInfo fi;
            DirectoryInfo di;

            StreamReader sr;
            StreamWriter sw;

            TimeSpan tSpan = new TimeSpan();
            DateTime dtStTemp = DateTime.Now;
            DateTime dtEdTemp = DateTime.Now;

            string sPath = GV.PATH_ERR;
            string sLotName = CData.SpcInfo.sLotName;

            string sLine = "";

            string[] sTemp;
            string[] sData;

            List<string> sRelease = new List<string>();

            di = new DirectoryInfo(sPath);

            if (!di.Exists)
            { return; }

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00");
            }

            sPath += sLotName + "_ErrLog.csv";
            fi = new FileInfo(sPath);

            if (!fi.Exists)
            { return; }

            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                while (sr.Peek() > -1)
                {
                    sLine = sr.ReadLine();
                    sRelease.Add(sLine);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                CMsg.Show(eMsg.Error, "Error_Lot", ex.Message);
                return;
            }

            sTemp = sRelease[sRelease.Count - 1].Split(',');

            sData = new string[sTemp.Length];

            for (int i = 0; i < sTemp.Length; i++)
            {
                sData[i] = sTemp[i];
            }
            if (sData[3] == "NONE")
            {
                sRelease.RemoveAt(sRelease.Count - 1);
                dtStTemp = DateTime.Parse(sData[2]);

                sData[3] = dtEdTemp.ToString("yyyy-MM-dd HH:mm:ss");
                tSpan = dtEdTemp - dtStTemp;

                sData[4] = tSpan.ToString(@"hh\:mm\:ss");
                sLine = sData[0] + "," + sData[1] + "," + sData[2] + "," + sData[3] + "," + sData[4];
                sRelease.Add(sLine);

                sw = new StreamWriter(sPath, false, Encoding.GetEncoding("euc-kr"));
                sw.Close();
                foreach (string line in sRelease)
                {
                    CLog.Check_File_Access(sPath, line, true);
                }
            }
        }
        // 2020.12.10 JSKim Ed

        // 2020.12.09 JSKim St
        //public static void CalMtbaMtbf()
        //{
        //    int iMtbaCnt = 0;
        //    int iMtbfCnt = 0;

        //    string sNow = DateTime.Now.ToString("yyyy-MM-dd");
        //    string sBeofreDay;
        //    string sPath = GV.PATH_ERRLOG;
        //    string sErrFrom;
        //    string sErrTo;
        //    string sRreadLine;
        //    DirectoryInfo di;
        //    //StreamReader sr;

        //    TimeSpan tSpan;

        //    if (CData.Opt.iPeriod == 0)
        //    {//달
        //        sBeofreDay = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        //    }
        //    else if(CData.Opt.iPeriod == 1)
        //    {//일주일
        //        sBeofreDay = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        //    }
        //    else
        //    {//하루
        //        sBeofreDay = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        //    }
        //    //koo 191101 안씀
        //    //sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
        //}

        //public static void SaveHis()
        //{

        //}
        // 2020.12.09 JSKim Ed
    }
    // Error code
    // XXXX
    // 000000 ~ 009999 - System
    // 100000 ~ 109999 - 
    
    public enum eErr
    {
        // 2020.12.09 JSKim St
        NONE = -1,
        // 2020.12.09 JSKim Ed
        MANUAL_ALL_HOME_TIMEOUT,
        SYSTEM_PNEUMATIC_ERROR,
        SYSTEM_LEFT_WHEEL_FILE_NOT_LOAD,
        SYSTEM_RIGHT_WHEEL_FILE_NOT_LOAD,
        SYSTEM_WHEEL_FILE_NOT_SELECTE,
        SYSTEM_WHEEL_FILE_SAVE_ERROR,
        SYSTEM_WMX_CREATE_DEVICE,
        SYSTEM_WMX_START_COMMUNICATION,
        SYSTEM_WMX_CLOSE_ERROR,

        ONLOADER_ALL_SERVO_ON_TIMEOUT,
        ONLOADER_WAIT_TIMEOUT,
        ONLOADER_PICK_TIMEOUT,
        ONLOADER_PUSH_TIMEOUT,
        ONLOADER_PLACE_TIMEOUT,
        ONLOADER_HOME_ERROR,
        ONLOADER_X_NOT_READY,
        ONLOADER_Y_NOT_READY,
        ONLOADER_Z_NOT_READY,
        ONLOADER_CLAMP_DETECT_MGZ_READY,
        ONLOADER_NEED_MGZ,
        ONLOADER_PUSHER_OVERLOAD_ERROR,
        ONLOADER_PUSHERFAIL,
        ONLOADER_TOP_MGZ_DETECT_FULL,
        ONLOADER_DETECT_LIGHTCURTAIN,
        ONLOADER_PUSHER_FORWARD,
        ONLOADER_Y_NOT_WAITPOSITION,
        ONLOADER_CLAMP_DETECTED_MGZ,

        INRAIL_SERVO_ON_TIMEOUT,
        INRAIL_WAIT_TIMEOUT,
        INRAIL_BARCODE_TIMEOUT,
        INRAIL_ORIENTATION_TIMEOUT,
        INRAIL_DYNAMICFUNCTION_TIMEOUT,
        INRAIL_ALIGN_TIMEOUT,
        INRAIL_NOTHING_PCBBASE_VALUE,
        INRAIL_HOME_TIMEOUT,
        INRAIL_X_NOT_READY,
        INRAIL_Y_NOT_READY,
        INRAIL_FEEDING_ERROR,
        INRAIL_GRIPPER_NO_DETECT,
        INRAIL_BARCODE_NOT_READY,
        INRAIL_BARCODE_READING_FAIL,
        INRAIL_ORIENTATION_READING_FAIL,
        INRAIL_NEED_REMOVE_STRIP,
        INRAIL_DECTED_STRIPIN_SENSOR,
        INRAIL_NOT_DETECTED_STRIP,
        INRAIL_PROBE_RS232_PORT_OPEN_ERROR,
        INRAIL_PROBE_RS232_PORT_CLOSE_ERROR,
        INRAIL_DYNAMICFUNCTION_READING_FAIL,
        INRAIL_DYNAMICFUNCTION_PCBHEIGHT_RANGEOVER,

        ONLOADERPICKER_SERVO_ON_TIMEOUT,
        ONLOADERPICKER_HOME_TIMEOUT,
        ONLOADERPICKER_WAIT_TIMEOUT,
        ONLOADERPICKER_PICKRAIL_TIMEOUT,
        ONLOADERPICKER_PICKTABLELEFT_TIMEOUT,
        ONLOADERPICKER_PLACE_TIMEOUT,
        ONLOADERPICKER_ALREADY_DETECT_STRIP_ERROR,
        ONLOADERPICKER_DETECT_STRIP_ERROR,
        ONLOADERPICKER_VACUUM_ERROR,
        ONLOADERPICKER_NOT_DETECT_STRIP_ERROR,

        LEFT_GIRND_PROBE_RS232_PORT_OPEN_ERROR,
        LEFT_GIRND_PROBE_RS232_PORT_CLOSE_ERROR,
        LEFT_GRIND_INVALID_SET_VALUE,
        LEFT_GRIND_SPINDLE_SET_RPM_ERROR,
        LEFT_GRIND_SPINDLE_RUN_RPM_ERROR,
        LEFT_GRIND_SPINDLE_STOP_ERROR,
        LEFT_GRIND_SPINDLE_COOLANT_OFF,
        LEFT_GRIND_SPINDLE_CDA_OFF,
        LEFT_GRIND_Y_AXIS_NOT_WAITPOSITION,
        LEFT_GRIND_VACUUM_ERROR,
        LEFT_GRIND_ALL_SERVO_ON_TIMEOUT,
        LEFT_GRIND_HOMING_TIMEOUT,
        LEFT_GRIND_WAIT_POSITION_TIMEOUT,
        LEFT_GRIND_WHEEL_INSPECTION_TIMEOUT,        
        LEFT_GRIND_DRESSING_TIMEOUT,
        LEFT_GRIND_TABLE_INSPECTION_TIMEOUT,
        LEFT_GRIND_TABLE_GRINDING_TIMEOUT,
        LEFT_GRIND_STRIP_INSPECTION_BEFORE_TIMEOUT,
        LEFT_GRIND_STRIP_INSPECTION_AFTER_TIMEOUT,
        LEFT_GRIND_STRIP_INSPECTION_ONE_TIMEOUT,
        LEFT_GRIND_STRIP_GRINDING_TIMEOUT,
        LEFT_GRIND_WATERKNIFE_TIMEOUT,
        LEFT_GRIND_RANGEOVER_WHEN_BEFORE_MEASURE_STRIP,
        LEFT_GRIND_RANGEOVER_WHEN_AFTER_MEASURE_STRIP,
        LEFT_GRIND_TTV_RANGEOVER_WHEN_AFTER_MEASURE_STRIP,
        LEFT_GRIND_PROBE_SAFTY_ERROR,
        LEFT_GRIND_PROBE_NOT_READY,
        LEFT_GRIND_TABLE_SAFTY_ERROR,
        LEFT_GRIND_TABLE_NOT_READY,
        LEFT_GRIND_ZAXIS_TIMEOUT,
        LEFT_GRIND_ZAXIS_SAFTY_ERROR,
        LEFT_GRIND_ZAXIS_NOT_READY,
        LEFT_GRIND_OVER_WHEEL_THICKNESS,
        LEFT_GRIND_OVER_DRESSER_THICKNESS,
        LEFT_GRIND_PROBETEST_TIMEOUT,
        LEFT_GRIND_PROBETEST_FAIL,
        LEFT_INSPECTION_TABLE_FAIL,

        RIGHT_GIRND_PROBE_RS232_PORT_OPEN_ERROR,
        RIGHT_GIRND_PROBE_RS232_PORT_CLOSE_ERROR,
        RIGHT_GRIND_INVALID_SET_VALUE,
        RIGHT_GRIND_SPINDLE_SET_RPM_ERROR,
        RIGHT_GRIND_SPINDLE_RUN_RPM_ERROR,
        RIGHT_GRIND_SPINDLE_STOP_ERROR,
        RIGHT_GRIND_VACUUM_ERROR,
        RIGHT_GRIND_ALL_SERVO_ON_TIMEOUT,
        RIGHT_GRIND_HOMING_TIMEOUT,
        RIGHT_GRIND_WAIT_POSITION_TIMEOUT,
        RIGHT_GRIND_WHEEL_INSPECTION_TIMEOUT,      
        RIGHT_GRIND_DRESSING_TIMEOUT,
        RIGHT_GRIND_TABLE_INSPECTION_TIMEOUT,
        RIGHT_GRIND_TABLE_GRINDING_TIMEOUT,
        RIGHT_GRIND_STRIP_INSPECTION_BEFORE_TIMEOUT,
        RIGHT_GRIND_STRIP_INSPECTION_AFTER_TIMEOUT,
        RIGHT_GRIND_STRIP_INSPECTION_ONE_TIMEOUT,
        RIGHT_GRIND_STRIP_GRINDING_TIMEOUT,
        RIGHT_GRIND_WATERKNIFE_TIMEOUT,
        RIGHT_GRIND_RANGEOVER_WHEN_BEFORE_MEASURE_STRIP,
        RIGHT_GRIND_RANGEOVER_WHEN_AFTER_MEASURE_STRIP,
        RIGHT_GRIND_TTV_RANGEOVER_WHEN_AFTER_MEASURE_STRIP,
        RIGHT_GRIND_PROBE_SAFTY_ERROR,
        RIGHT_GRIND_PROBE_NOT_READY,
        RIGHT_GRIND_TABLE_SAFTY_ERROR,
        RIGHT_GRIND_TABLE_NOT_READY,
        RIGHT_GRIND_ZAXIS_SAFTY_ERROR,
        RIGHT_GRIND_ZAXIS_NOT_READY,
        RIGHT_GRIND_OVER_WHEEL_THICKNESS,
        RIGHT_GRIND_OVER_DRESSER_THICKNESS,
        RIGHT_GRIND_PROBETEST_TIMEOUT,
        RIGHT_GRIND_PROBETEST_FAIL,
        RIGHT_INSPECTION_TABLE_FAIL,

        OFFLOADERPICKER_WAIT_TIMEOUT,
        OFFLOADERPICKER_CLEAN_TIMEOUT,
        OFFLOADERPICKER_PICK_TIMEOUT,
        OFFLOADERPICKER_CLEAN_STRIP_TIMEOUT,
        OFFLOADERPICKER_PLACE_TIMEOUT,
        OFFLOADERPICKER_DETECT_STRIP_ERROR,
        OFFLOADERPICKER_VACUUM_CHECK_ERROR,

        DRY_WAIT_POSITION_TIMEOUT,
        DRY_CHECK_SAFETY_TIMEOUT,
        DRY_DRYWORK_TIMEOUT,
        DRY_STRIP_OUT_TIMEOUT,
        DRY_CHECK_SAFETY_SENSOR1_ERROR,
        DRY_CHECK_SAFETY_SENSOR2_ERROR,
        DRY_PUSHER_OVERLOAD_ERROR,
        DRY_X_NOT_READY,
        DRY_Z_NOT_READY,
        DRY_R_NOT_READY,
        DRY_BOTTOM_CLEAN_WATER_ERROR,
        DRY_Z_NOT_UP_POSITION,
        DRY_HOME_ERROR,
        DRY_NOT_DETECTED_STRIP,
        
        OFFLOADER_WAIT_TIMEOUT,
        OFFLOADER_PLACE_TIMEOUT,      
        OFFLOADER_HOME_TIMEOUT,
        OFFLOADER_BTM_MAGZIN_FULL,
        OFFLOADER_X_NOT_READY,
        OFFLOADER_Y_NOT_READY,
        OFFLOADER_Z_NOT_READY,
        OFFLOADER_BOTTOM_NOT_DETECT_MGZ_ERROR,
        OFFLOADER_MIDDLE_CONVEYOR_NOT_DETECT_MGZ_ERROR,
        OFFLOADER_NOT_DETECTED_BOTTOM_MGZ_ERROR,
        OFFLOADER_DETECT_LIGHTCURTAIN,

        PUMP_LEFT_RUN_TIMEOUT,
        PUMP_RIGHT_RUN_TIMEOUT,
        INRAIL_OCR_READING_FAIL,
        INRAIL_BCR_OCR_READING_FAIL,
        TABLE_LEAK_SENSOR_ERROR,
        ONLOADERPICKER_NO_STRIP_PICK_UP,
        OFFLOADERPICKER_NO_STRIP_PICK_UP_ON_LEFT_TABLE,
        OFFLOADERPICKER_NO_STRIP_PICK_UP_ON_Right_TABLE,
        GRIND_DRESSER_REPLACE_TIMEOUT,
        ONLOADERPICKER_CONVERSION_TIMEOUT,
        ONLOADERPICKER_CAN_NOT_MOVE_CONVERSION_POSITION,
        OFFLOADERPICKER_CONVERSION_TIMEOUT,
        OFFLOADERPICKER_CAN_NOT_MOVE_CONVERSION_POSITION,

        SYSTEM_MAIN_CONTROLLER_ERROR,
        RIGHT_GRIND_SPINDLE_COOLANT_OFF,
        GRIND_SPINDLE_COOLANT_TEMP,
        SYSTEM_MAIN_EMG_FRONT_LEFT,
        SYSTEM_MAIN_EMG_FRONT_RIGHT,
        SYSTEM_MAIN_EMG_REAR_LEFT,
        SYSTEM_MAIN_EMG_REAR_RIGHT,
        SYSTEM_ONLOADER_EMG_FRONT,
        SYSTEM_ONLOADER_EMG_REAR,
        SYSTEM_OFFLOADER_EMG_FRONT,
        SYSTEM_OFFLOADER_EMG_REAR,

        OFFLOADER_TOP_MAGZIN_FULL,
        OFFLOADER_NOT_DETECTED_TOP_MGZ_ERROR,
		LEFT_GRIND_WHEEL_MEASURE_TTV_OVER,
		RIGHT_GRIND_WHEEL_MEASURE_TTV_OVER,
		QC_CAN_NOT_OUT_STRIP_ERROR,
        DRY_LEAK_SENSOR_ERROR,
        OFFLOADER_CAN_NOT_READ_RFID,
        QC_CAN_NOT_OUT_STRIP_TO_OFFLOAD_ERROR, //190421 ksg :

        LEFT_GRIND_MANUALZIG_ON_THE_TABLE ,
        RIGHT_GRIND_MANUALZIG_ON_THE_TABLE,
        OFFLOADER_BOTTOM_CLAMP_NOT_CLOSE  ,//190514
        ONLOADER_CLAMP_NOT_CLOSE          , // 2021.06.16 SungTae : [수정] Error명 오타로 변경 (ONLOADER_CLAMP_NOT_CLOASE -> ONLOADER_CLAMP_NOT_CLOSE)
        //190522 koo 
        LEFT_PUMP_RUN_ERROR       ,
        LEFT_PUMP_FLOW_LOW_ERROR  ,
        LEFT_PUMP_TEMP_HIGH_ERROR ,
        LEFT_PUMP_OVERLOAD_ERROR  ,
        RIGHT_PUMP_RUN_ERROR      ,
        RIGHT_PUMP_FLOW_LOW_ERROR ,
        RIGHT_PUMP_TEMP_HIGH_ERROR,
        RIGHT_PUMP_OVERLOAD_ERROR,

        //20190604 ghk_onpbcr
        ONLOADERPICKER_BCR_TIMEOUT,
        ONLOADERPICKER_ORI_TIMEOUT,

		SYSTEM_WHEEL_JIG_CHECK    ,
        //20190618 ghk_dfserver
        DYNAMIC_FUNCTION_SERVER_NEED_RUN,
        DYNAMIC_FUNCTION_SERVER_NOTHING_GL1_DATA,
        //
        //20190611 ghk_onpclean
        ONLOADERPICKER_PICKERCLEAN_TIMEOUT,
        //
        OFFLOADER_ALREADY_BOTTOM_MGZ_ERROR,

        ONLOADER_X_AXIS_MOTOR_ALRAM, // X
        ONLOADER_Y_AXIS_MOTOR_ALRAM,
        ONLOADER_Z_AXIS_MOTOR_ALRAM,

        INRAIL_X_AXIS_MOTOR_ALRAM,
        INRAIL_Y_AXIS_MOTOR_ALRAM,

        ONLOADER_PICKER_X_AXIS_MOTOR_ALRAM,
        ONLOADER_PICKER_Y_AXIS_MOTOR_ALRAM, //X
        ONLOADER_PICKER_Z_AXIS_MOTOR_ALRAM,
        
        LEFT_GRIND_X_AXIS_MOTOR_ALRAM,
        LEFT_GRIND_Y_AXIS_MOTOR_ALRAM,
        LEFT_GRIND_Z_AXIS_MOTOR_ALRAM,

        RIGHT_GRIND_X_AXIS_MOTOR_ALRAM,
        RIGHT_GRIND_Y_AXIS_MOTOR_ALRAM,
        RIGHT_GRIND_Z_AXIS_MOTOR_ALRAM,

        OFFLOADER_PICKER_X_AXIS_MOTOR_ALRAM,
        OFFLOADER_PICKER_Z_AXIS_MOTOR_ALRAM,

        DRY_X_AXIS_MOTOR_ALRAM,
        DRY_Z_AXIS_MOTOR_ALRAM,
        DRY_AIR_AXIS_MOTOR_ALRAM,

        OFFLOADER_X_AXIS_MOTOR_ALRAM,
        OFFLOADER_Y_AXIS_MOTOR_ALRAM,
        OFFLOADER_Z_AXIS_MOTOR_ALRAM,

        INRAIL_X_AXIS_NOT_WIAT_POSITION         , //190717 ksg :
        OFFLOADERPICKER_X_AXIS_NOT_WIAT_POSITION, //190717 ksg :
        ONLOADERPICKER_X_AXIS_NOT_WIAT_POSITION , //190717 ksg :

        DRY_DETECTED_STRIP , //190724 ksg :

        RIGHT_GRIND_Y_AXIS_NOT_WAITPOSITION,
        //190813 ksg :
        LEFT_BEFORE_PROBE_DATA_LOST , 
        RIGHT_BEFORE_PROBE_DATA_LOST,
        LEFT_AFTER_PROBE_DATA_LOST  ,
        RIGHT_AFTER_PROBE_DATA_LOST ,

        LEFT_WRONG_CALCULATOR_GRIND_START_POSITION  ,
        RIGHT_WRONG_CALCULATOR_GRIND_START_POSITION ,
        LEFT_WRONG_CALCULATOR_GRIND_TARGET_POSITION ,
        RIGHT_WRONG_CALCULATOR_GRIND_TARGET_POSITION,

        //20190807 ghk_tableclean
        LEFT_GRIND_TABLECLEAN_TIMEOUT          ,
        RIGHT_GRIND_TABLECLEAN_TIMEOUT         ,
        //
        UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_LEFT ,
        UNKNOWN_STRIP_ON_THE_TABLE_TO_THE_RIGHT,

        //190904 ksg :
        TABLE_WATER_OFF_DURING_WARMUP,

        //20190705 ghk_meawhl
        LEFT_GRIND_NOT_DETECTED_TOOLSENSOR,
        RIGHT_GRIND_NOT_DETECTED_TOOLSENSOR,
        
        LEFT_TOOL_SETTER_IO_NOT_DETECT,
        RIGHT_TOOL_SETTER_IO_NOT_DETECT,

        LEFT_PROBE_UP_NOT_COMPLET,
        RIGHT_PROBE_UP_NOT_COMPLET,

        LEFT_TOOL_SETTER_SIGNAL_NOT_NOMAL,
        RIGHT_TOOL_SETTER_SIGNAL_NOT_NOMAL,

        LEFT_ONE_POINT_VALUE_WRONG,
        RIGHT_ONE_POINT_VALUE_WRONG,

        BCR_REPLY_TIMEOUT, //191031 ksg :

        BCR_NOT_READY, //191031 ksg :
		
		//20190724 ghk_caltarget        
		LEFT_GRIND_STRIP_HEIGHT_LESS_THEN_TARGET_WHEN_BEFORE_GRIND,
        RIGHT_GRIND_STRIP_HEIGHT_LESS_THEN_TARGET_WHEN_BEFORE_GRIND,
        //

        LIGHT_RS232_PORT_OPEN_ERROR ,
        LIGHT_RS232_PORT_CLOSE_ERROR,
		
		INRAIL_INPUT_DETECTED_STRIP, //191109

        //20191120 ghk_display_strip
        OFFLOADER_ALREADY_TOP_MGZ_ERROR,

        LEFT_GRIND_DETECTED_STRIP,
        RIGHT_GRIND_DETECTED_STRIP,

        OFFLOADER_TOP_NOT_DETECT_MGZ_ERROR,
        //

        //20191128 ghk_warmup_error
        LEFT_GRIND_TOPCLEAN_NOT_UP            ,
        RIGHT_GRIND_TOPCLEAN_NOT_UP           ,
        LEFT_GRIND_TOPCLEAN_WATER_ERROR       ,
        RIGHT_GRIND_TOPCLEAN_WATER_ERROR      ,
        LEFT_GRIND_SPINDLE_WATER_ERROR        ,
        RIGHT_GRIND_SPINDLE_WATER_ERROR       ,
        LEFT_GRIND_SPINDLE_BOTTOM_WATER_ERROR ,
        RIGHT_GRIND_SPINDLE_BOTTOM_WATER_ERROR,
        //

        //20200109 myk_ONL_Push_Pos
        ONLOADER_PUSH_POSITION_TIMEOUT,
        //
        MAIN_COVER_NOT_CLOSE,
        ONLOADER_CAN_NOT_READ_RFID,

        INRAIL_LIFT_UD_ERROR,

		HOST_COMMUCATION_OFFLINE_ERROR,
        HOST_COMMUCATION_DISCONNECT_ERROR,

        HOST_RECIPE_VERIFY_ERROR,

        HOST_LOT_ID_UNKONWN_ERROR,
        HOST_STRIP_ID_UNKNOWN_ERROR,
        HOST_LOT_STRIP_COUNT_ERROR,
        
        HOST_LEFT_TOOL_ID_UNKONWN_ERROR,
        HOST_RIGHT_TOOL_ID_UNKONWN_ERROR,

        HOST_LEFT_DRESS_ID_UNKONWN_ERROR,
        HOST_RIGHT_DRESS_ID_UNKONWN_ERROR,

        HOST_LEFT_WHEEL_ID_UNKONWN_ERROR,
        HOST_RIGHT_WHEEL_ID_UNKONWN_ERROR,

        HOST_UD_MGZ_ID_UNKONWN_ERROR,
        HOST_LOTEND_ID_UNKONWN_ERROR,

        HOST_NOT_REQUEST_ERROR,
        HOST_LOT_VERIFY_TIME_OVER_ERROR,
        HOST_STRIP_VERIFY_TIME_OVER_ERROR,
        HOST_STRIP_VERIFY_HOLD_TIME_OVER_ERROR,
        HOST_LD_MGZ_VERIFY_TIME_OVER_ERROR,
        HOST_ULD_MGZ_VERIFY_TIME_OVER_ERROR,

		QC_ERROR_STATUS,

        LEFT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR,
        LEFT_GRIND_UNIT1_VACUUM_ERROR,
        LEFT_GRIND_UNIT2_VACUUM_ERROR,
        LEFT_GRIND_UNIT3_VACUUM_ERROR,
        LEFT_GRIND_UNIT4_VACUUM_ERROR,

        RIGHT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR,
        RIGHT_GRIND_UNIT1_VACUUM_ERROR,
        RIGHT_GRIND_UNIT2_VACUUM_ERROR,
        RIGHT_GRIND_UNIT3_VACUUM_ERROR,
        RIGHT_GRIND_UNIT4_VACUUM_ERROR,

        DRY_CHECK_CARRIER_NOT_EXIST_ERROR,
        DRY_CHECK_UNIT1_NOT_EXIST_ERROR,
        DRY_CHECK_UNIT2_NOT_EXIST_ERROR,
        DRY_CHECK_UNIT3_NOT_EXIST_ERROR,
        DRY_CHECK_UNIT4_NOT_EXIST_ERROR,
		
		//20200314 jhc : 2D Vision Alarm
        BCR_2DVISION_CHKRUN_ERROR,
        BCR_2DVISION_ALARM_STATUS,
		LEFT_GRIND_SPINDLE_OVERLOAD , //200310 ksg : Spindle 부하
		RIGHT_GRIND_SPINDLE_OVERLOAD, //200310 ksg : Spindle 부하
        INRAIL_DYNAMICFUNCTION_BASEHEIGHT_RANGEOVER, //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-) 초과 Error
        //	
		DRY_X_WAIT_POSITION_ERROR,//200521 pjh : Dry X Position Check Error
        //
        LEFT_GRIND_SPINDLE_BOTTOM_AIR_ERROR,   //20200618 myk : Spindle Bottom Air Sensor Check Error
        RIGHT_GRIND_SPINDLE_BOTTOM_AIR_ERROR,  //20200618 myk : Spindle Bottom Air Sensor Check Error
        //
        LEFT_GRIND_TABLE_WATER_ERROR,
        RIGHT_GRIND_TABLE_WATER_ERROR,//200616 pjh : Table Water Flow Check Error
        //

        INRAIL_IONIZER_POWER_ERROR, //20200622 lks 이오나이저 전원
        DRY_IONIZER_POWER_ERROR, //20200622 lks 이오나이저 전원

        ONLOADERPICKER_X_NOT_READY, // 200721 jym : Ready error 추가
        ONLOADERPICKER_Y_NOT_READY, // 200721 jym : Ready error 추가
        ONLOADERPICKER_Z_NOT_READY, // 200721 jym : Ready error 추가

        OFFLOADERPICKER_X_NOT_READY, // 200721 jym : Ready error 추가
        OFFLOADERPICKER_Z_NOT_READY, // 200721 jym : Ready error 추가
		
		//200708 pjh : Error 자재 Check
		LEFT_GRIND_DETECT_REJECT_STRIP_ERROR,
        RIGHT_GRIND_DETECT_REJECT_STRIP_ERROR,
		//

        SECSGEM_USE_BCR_SKIP_OPTION_ERROR, //200807 jhc : SECS/GEM 사용 시 BCR Skip 옵션이면 Error

        // 2020.08.19 JSKim St
        LEFT_ONE_POINT_OVER_GRIND_CHECK_ERROR,    // Left One Point Checking 시 OverGrinding Range Error
        RIGHT_ONE_POINT_OVER_GRIND_CHECK_ERROR,   // Right One Point Checking 시 OverGrinding Range Error
        // 2020.08.19 JSKim Ed

        //200810 myk : Probe Calibration Check
        LEFT_GRIND_PROBE_CALIBRATION_CHECK_TIMEOUT,
        RIGHT_GRIND_PROBE_CALIBRATION_CHECK_TIMEOUT,

        //200818 myk : Probe Auto Calibration
        LEFT_GRIND_PROBE_AUTO_CALIBRATION_TIMEOUT,
        RIGHT_GRIND_PROBE_AUTO_CALIBRATION_TIMEOUT,

        //syc : Probe contact table
        LEFT_GRIND_PROBE_CALIBRATION_TIMEOUT,
        RIGHT_GRIND_PROBE_CALIBRATION_TIMEOUT,

        //200924 jym : Spindle error add
        LEFT_SPINDLE_SERVO_OFF,
        RIGHT_SPINDLE_SERVO_OFF,
        LEFT_SPINDLE_ALARM,
        RIGHT_SPINDLE_ALARM,

        // 200925 jym : Error add
        OFFLOADER_BOTTOM_PICK_TIMEOUT,
        OFFLOADER_TOP_PICK_TIMEOUT,

        // 2020.09.28 JSKim St
        QC_DETECTED_STRIP,
        // 2020.09.28 JSKim Ed

        //201013 jhc : ASE-Kr (SECS/GEM 비 사용)+(DF Bottom) 설정 상태에서 START 버튼 누르면 Alarm
        //(SECS/GEM 비 사용)+(DF Bottom) 설정 상태 => Mold 두께(DF Bottom, 낮은 값) 기준 Recipe로 Target 그라인딩하면 자재 파손 위험!!!
        BTM_MOLD_SECSGEM_OFF_ERROR,

        // 2020-10-19, jhLee : SPIL社 RCMD를 통한 설비 START 진행시 발생하는 예외 상황
        HOST_RCMD_START_TIMEOUT_ERROR,      // HOST에서 지정된 시간동안 RCMD로 START 명령이 내려오지 않았다.
        HOST_RCMD_RESPONSE_ERROR,           // HOST에서 지정된 RCMD로 START 명령대신 다른 명령이 내려왔다.

        // 201022 jym : 추가
        LEFT_AUTO_OFFSET_RANGEOVER,
        RIGHT_AUTO_OFFSET_RANGEOVER,

        // 2020.10.26 SungTae : Dresser Measure 관련 Alarm 추가
        RETRY_LEFT_DRESSER_CHANGE,
        RETRY_RIGHT_DRESSER_CHANGE,
		
		// 2020.10.22 JSKim St
        LEFT_ADD_PUMP_RUN_ERROR,
        LEFT_ADD_PUMP_FLOW_LOW_ERROR,
        LEFT_ADD_PUMP_TEMP_HIGH_ERROR,
        LEFT_ADD_PUMP_OVERLOAD_ERROR,
        RIGHT_ADD_PUMP_RUN_ERROR,
        RIGHT_ADD_PUMP_FLOW_LOW_ERROR,
        RIGHT_ADD_PUMP_TEMP_HIGH_ERROR,
        RIGHT_ADD_PUMP_OVERLOAD_ERROR,
        // 2020.10.22 JSKim Ed

        // 2020.11.07 lhs Start : 추가
        ADD_PUMP_LEFT_RUN_TIMEOUT_ERROR,
        ADD_PUMP_RIGHT_RUN_TIMEOUT_ERROR,
        // 2020.11.07 lhs End

        //201121 jhc : [추가] ASE-Kr (SECS/GEM Local Mode)+(DF Bottom) 설정 상태에서 START 버튼 누르면 Alarm
        BTM_MOLD_SECSGEM_LOCAL_ERROR,
        //

        //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
        LEFT_WHEEL_LOSS_CORRECT_LIMIT_OVER,
        RIGHT_WHEEL_LOSS_CORRECT_LIMIT_OVER,
        //

        //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
        LEFT_GRIND_SPINDLE_CURRENT_LOW,   //Spindle Current(전류) Lower Limit Alarm (Left)
		LEFT_GRIND_SPINDLE_CURRENT_HIGH,  //Spindle Current(전류) Upper Limit Alarm (Left)
        RIGHT_GRIND_SPINDLE_CURRENT_LOW,  //Spindle Current(전류) Lower Limit Alarm (Right)
		RIGHT_GRIND_SPINDLE_CURRENT_HIGH, //Spindle Current(전류) Upper Limit Alarm (Right)
        LEFT_GRIND_TABLE_VACUUM_LOW,      //Table Vacuum Lower Limit Alarm (Left)
        RIGHT_GRIND_TABLE_VACUUM_LOW,     //Table Vacuum Lower Limit Alarm (Right)
        //..

        // 210106 myk : Dressing 후 휠 높이 측정 시 드레싱 후 휠 두께가 드레싱 전 휠 두께보다 두꺼우면 에러 발생
        LEFT_GRIND_DRESSING_FAIL,   //Left Dressing 후 휠 높이가 Dressing 전 휠 높이보다 낮을 시 에러 발생
        RIGHT_GRIND_DRESSING_FAIL,  //Right Dressing 후 휠 높이가 Dressing 전 휠 높이보다 낮을 시 에러 발생

        // 2021.04.02 lhs Start
        OFFLOADERPICKER_CHECK_IV2_TIMEOUT,
        OFFLOADERPICKER_IV2_DI_ERROR,  // IV2에서 I/O로 받은 에러
        OFFLOADERPICKER_IV2_DI_NG,     // IV2에서 I/O로 받은 종합판정이 OK가 아님, NG
        OFFLOADERPICKER_COVER_DRYER_TIMEOUT,
        OFFLOADERPICKER_COVER_DRYER_END_TIMEOUT,

        DRY_WATER_NOZZLE_TIMEOUT,
        DRY_Z_NOT_TIP_CLAMPED_POSITION,
        DRY_Z_NOT_COVER_DRYER_POSITION,
        // 2021.04.02 lhs End

        INRAIL_CHECK_UNIT_NOT_EXIST_ERROR,      // 2021.05.06 SungTae : [추가] SPIL FJ 2003U 개조건 관련 Unit 유무 확인용 Alarm


        // 2021-06-23, jhLee, SPIL-FJ VOC, Offline이 되었을 때 알람을 울려준다.
        SECSGEM_OFFLINE_ERROR,                  //393 SECS/GEM 연결이 Offline이 되었다. 
		DF_SERVER_DATA_FILE_READING_FAIL, //210608 pjh : D/F Server Data Reading Fail Error

        // 2021.07.30 SungTae : [추가] ASE-KR VOC로 Grinding이 완료된 자재가 재투입 시 Over Grinding 되는 Issue 관련 Error 추가
        LEFT_WRONG_PCB_THICKNESS_DATA_ERROR,

		//2021.08.06 syc : Up상태일때 Probe 값 정상 범위 인지 확인
		//- Qorvo 프로브 앰프값 자동으로 바뀌는 이슈에 대한 방지책
		//- Probe 값이 18.9xxx ~ 19.0xxx 범위 이탈시 알람
		LEFT_PROBE_AMP_VALUE_WRONG,
		RIGHT_PROBE_AMP_VALUE_WRONG,

        // 2021.09.30 lhs Start : syc 업데이트 부분 추가		
        OFFLOADERPICKER_CLAMP_CHECK_ERROR,
		OFFLOADERPICKER_COVER_VACUUM_CHECK_ERROR,
		OFFLOADERPICKER_COVER_PICK_TIMEOUT_ERROR,
		OFFLOADERPICKER_COVER_CHECK_TIMEOUT_ERROR,
		OFFLOADERPICKER_UNIT_CHECK_TIMEOUT_ERROR,
		OFFLOADERPICKER_IV2_NOT_READY,
		OFFLOADERPICKER_IV2_PARAMETER_CHANGE_FAIL,
		OFFLOADERPICKER_IV2_RESULT_NG,
		ONLOADERPICKER_UNIT_CHECK_TIMEOUT_ERROR,
		ONLOADERPICKER_IV2_NOT_READY,
		ONLOADERPICKER_IV2_PARAMETER_CHANGE_FAIL,
		ONLOADERPICKER_IV2_RESULT_NG,
        // 2021.09.30 lhs End

        // 2021.09.30 lhs Start : PM Count 오버시 에러 
        PMCOUNT_LEFT_TABLE_SPONGE_CHECK_COUNT_OVER_ERROR,   // Left Table Sponge Count Over Error (Chcek)
        PMCOUNT_LEFT_TABLE_SPONGE_CHANGE_COUNT_OVER_ERROR,  // Left Table Sponge Count Over Error (Change)
        PMCOUNT_RIGHT_TABLE_SPONGE_CHECK_COUNT_OVER_ERROR,  // Right Table Sponge Count Over Error (Chcek)
        PMCOUNT_RIGHT_TABLE_SPONGE_CHANGE_COUNT_OVER_ERROR, // Right Table Sponge Count Over Error (Change)
        PMCOUNT_OFFPICKER_SPONGE_CHECK_COUNT_OVER_ERROR,    // OffPicker Sponge Count Over Error (Chcek)
        PMCOUNT_OFFPICKER_SPONGE_CHANGE_COUNT_OVER_ERROR,   // OffPicker Sponge Count Over Error (Change)
        // 2021.09.30 lhs End : PM Count 오버시 에러 

        // 2021.09.24 SungTae Start : [추가] ASE-KR VOC로 Table Tilt 관련 Alarm 추가
        LEFT_TBL_TOP_TO_BTM_TILT_OVER_ERROR,       // Left Table 상/하 측정값 간 차이가 설정된 Limit 보다 클 경우
        LEFT_TBL_LFT_TO_RGT_TILT_OVER_ERROR,       // Left Table 좌/우 측정값 간 차이가 설정된 Limit 보다 클 경우
        RIGHT_TBL_TOP_TO_BTM_TILT_OVER_ERROR,      // Right Table 상/하 측정값 간 차이가 설정된 Limit 보다 클 경우
        RIGHT_TBL_LFT_TO_RGT_TILT_OVER_ERROR,      // Right Table 좌/우 측정값 간 차이가 설정된 Limit 보다 클 경우
        // 2021.09.24 SungTae End

        // 2022.02.15 lhs Start : SecsGem에서 받은 데이터 Range Check
        SECSGEM_DOWNLOAD_PCBTHICKNESS_RNAGE_OVER_ERROR,
        SECSGEM_DOWNLOAD_TOTAL_THICKNESS_RNAGE_OVER_ERROR,
        // 2022.02.15 lhs End : SecsGem에서 받은 데이터 Range Check

        //DRY_BOTTOM_CLEAN_AIRBLOW_ERROR,    // 2022.03.09 lhs : Bottom Cleaner AirBlow Error, Spray Nozzle형 설치시 추가됨.

        // 2022.04.19 SungTae Start : [추가] ASE-KH ONL & OFL BCR 추가에 따른 Reading 실패 알람 추가
        ONLOADER_MGZ_ID_READ_TIMEOUT,
        ONLOADER_MGZ_ID_READ_FAIL_ERROR,

        OFFLOADER_MGZ_ID_READ_TIMEOUT,
        OFFLOADER_MGZ_ID_READ_FAIL_ERROR,
        // 2022.04.19 SungTae End

        // 2022.06.08 lhs Start : On/Off Picker Eject 후 진공해제 확인시 에러
        ONLOADERPICKER_VACUUM_FREE_ERROR,
        OFFLOADERPICKER_VACUUM_FREE_ERROR,
        // 2022.06.08 lhs End

        // 2022.08.05 SungTae Start : [추가] (ASE-KR 개발건) User가 설정한 Final Target 값과 Grinding 완료 후 측정값이 다를 경우 Alarm
        L_TOP_GRIND_DATA_FINAL_TARGET_COMPARE_ERROR,        // Left Table에서 Top Grinding 후 자재 측정 시 Final Target Thickness와 같지 않을 경우
        R_TOP_GRIND_DATA_FINAL_TARGET_COMPARE_ERROR,        // Right Table에서 Top Grinding 후 자재 측정 시 Final Target Thickness와 같지 않을 경우
        BTM_GRIND_DATA_FINAL_TARGET_COMPARE_ERROR,          // Bottom Grinding 후 자재 측정 시 Final Target Thickness와 같지 않을 경우
        // 2022.08.05 SungTae End

        OFFLOADERPICKER_CARRIER_DETECT_ERROR,   // 2004U : 2022.08.10 lhs

        ERR_MAXCOUNT,              
    }

    // 190711-maeng
    // ABBCCDDD
    // A  - 0:Software error
    //    - 1:Hardware error
    // BB - Part index
    // CC - Part sub index
    // DDD - Notice
    public enum eErr2
    {
        NONE = 00000,

        // System error - 00001 ~ 09999
        SYS_ERROR = 00001,
        SYS_ALL_SERVO_ON_TIMEOUT = 00002,
        SYS_ALL_SERVO_OFF_TIMEOUT = 00003,
        SYS_ALL_HOMING_TIMEOUT = 00004,

        // Onloader error - 10000 ~ 19999
        ONL_ERROR = 10000,
        ONL_SERVO_ON_TIMEOUT = 10001,
        ONL_HOMING_TIMEOUT = 10002,
        ONL_X_AXIS_ERROR = 10100,
        ONL_Y_AXIS_ERROR = 10200,

        // Inrail error - 20000 ~ 29999
        INR_ERROR = 20000,
        INR_SERVO_ON_TIMEOUT = 20001,
        INR_HOMING_TIMEOUT = 20002,

        // Onloader picker error - 30000 ~ 39999
        ONP_ERROR = 30000,
        ONP_SERVO_ON_TIMEOUT = 30001,
        ONP_HOMING_TIMEOUT = 30002,

        // Left grind error - 40000 ~ 49999
        LGD_ERROR = 40000,
        LGD_SERVO_ON_TIMEOUT = 40001,
        LGD_HOMING_TIMEOUT = 40002,

        // Right grind error - 50000 ~ 59999
        RGD_ERROR = 50000,
        RGD_SERVO_ON_TIMEOUT = 50001,
        RGD_HOMING_TIMEOUT = 50002,

        // Offloader picker error - 60000 ~ 69999
        OFP_ERROR = 60000,
        OFP_SERVO_ON_TIMEOUT = 60001,
        OFP_HOMING_TIMEOUT = 60002,

        // Dry error - 70000 ~ 79999
        DRY_ERROR = 70000,
        DRY_SERVO_ON_TIMEOUT = 70001,
        DRY_HOMING_TIMEOUT = 70002,

        // Offloader picker - 80000 ~ 89999
        OFL_ERROR = 80000,
        OFL_SERVO_ON_TIMEOUT = 80001,
        OFL_HOMING_TIMEOUT = 80002,


        // Part index : 02-Sequence
        // Sub index : 01-Onloader
        SW_SEQ_ONL_ERROR = 00201000,

        SW_SEQ_ONL_X_NOT_READY = 00201010,
        SW_SEQ_ONL_Y_NOT_READY = 00201020,
        SW_SEQ_ONL_Y_NOT_WAIT_POSITION = 00201021,
        SW_SEQ_ONL_Z_NOT_READY = 00201030,
        SW_SEQ_ONL_WAIT_TIMEOUT = 00201100,
        SW_SEQ_ONL_PICK_TIMEOUT = 00201110,
        SW_SEQ_ONL_PLACE_TIMEOUT = 00201120,
        SW_SEQ_ONL_PUSH_TIMEOUT = 00201130,
        SW_SEQ_ONL_PUSH_FAIL,
        SW_SEQ_ONL_CLAMP_DETECT_MGZ = 00201400,
        SW_SEQ_ONL_CLAMP_DETECT_MGZ_READY,
        SW_SEQ_ONL_NEED_MGZ,
        SW_SEQ_ONL_TOP_MGZ_DETECT_FULL,
        SW_SEQ_ONL_PUSHER_OVERLOAD,
        SW_SEQ_ONL_PUSHER_FORWARD,
        SW_SEQ_ONL_DETECT_LIGHT_CURTAIN,

        // Part index : 02-Sequence
        // Sub index : 02-Inrail
        SW_SEQ_INR_ERROR = 00202000,
        SW_SEQ_INR_ALL_SERVO_ON_TIMEOUT,

        // Part index : 02-Sequence
        // Sub index : 03-Onloader Picker
        SW_SEQ_ONP_ERROR = 00203000,
        SW_SEQ_ONP_ALL_SERVO_ON_TIMEOUT,

        // Part index : 02-Sequence
        // Sub index : 04-Grind Left
        SW_SEQ_GRL_ERROR = 00204000,
        SW_SEQ_GRL_ALL_SERVO_ON_TIMEOUT,

        // Part index : 02-Sequence
        // Sub index : 05-Grind Right
        SW_SEQ_GRR_ERROR = 00205000,
        SW_SEQ_GRR_ALL_SERVO_ON_TIMEOUT,

        // Part index : 02-Sequence
        // Sub index : 06-Offloader Picker
        SW_SEQ_OFP_ERROR = 00206000,

        // Part index : 02-Sequence
        // Sub index : 07-Dry
        SW_SEQ_DRY_ERROR = 00207000,

        // Part index : 02-Sequence
        // Sub index : 08-Offloader
        SW_SEQ_OFL_ERROR = 00208000,

        // Hardware
        // Parts
        //  00-Etc
        //  01-WMX
        //  02-Motion
        //  03-Spindle
        //  04-IO
        //  05-Probe
        HW_ERROR = 10000000,

        // Part index : 00-Etc
        // Sub index : 00-Common
        HW_ETC_PNEUMATIC_LOW = 10000001,

        // Part index : 01-WMX
        // Sub index : 00-Common
        HW_WMX_ERROR = 10100000,
        HW_WMX_CREATE_DEVICE_FAIL = 10100001,
        HW_WMX_CLOSE_DEVICE_FAIL = 10100002,
        HW_WMX_START_COMMUNICATION_FAIL = 10100003,
        HW_WMX_STOP_COMMUNICATION_FAIL = 10100004,
        HW_WMX_SET_EVENT_FAIL = 10100005,

        // Part index : 02-Motion
        // Sub index : Axis ID number
        HW_MOTION_ERROR = 10200000,

        // Part index : 03-Spindle
        // Sub index : 00-Common, 01-Left, 02-Right
        HW_SPINDLE_ERROR = 10300000,

        // Part index : 04-IO
        // Sub index : 00-Common, 01-Input, 02-Output
        HW_IO_ERROR = 10400000,

        // Part index : 05-Probe
        // Sub index : 00-Common, 01-Left, 02-Right, 03-D/F
        HW_PROBE_ERROR = 10500000
    }
}
