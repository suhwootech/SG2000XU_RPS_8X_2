using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
namespace SG2000X
{
    public static class CLog
    {
        public static object lockObject = new object(); 
        private static CTim m_Kill = new CTim();

        public static object lockObject2 = new object();
        private static CTim m_Kill2 = new CTim();

        #region Log "deleteDate " 이전 폴더는 삭제
        private static void mLog_Delete(string sPath)
        {
            try
            {
                int deleteDate = 1;
                DirectoryInfo di = new DirectoryInfo(sPath);
                if (di.Exists)
                {
                    DirectoryInfo[] dirinfo = di.GetDirectories();
                    string iDate = DateTime.Today.AddDays(-deleteDate).ToString("yyyyMMdd");
                    foreach (DirectoryInfo dir in dirinfo)
                    {
                        if (iDate.CompareTo(dir.LastWriteTime.ToString("yyyyMMdd")) > 0)
                        {
                            dir.Attributes = FileAttributes.Normal;
                            dir.Delete(true);
                        }
                    }
                }
            }
            catch (Exception) { }
        }
        #endregion
        #region 로그 저장
        /// <summary>
        /// Log 저장 함수
        /// </summary>
        /// 
        public static void mLogSeq(string sData)
        {
            string sLine;
            string sPath;

            DateTime Now = DateTime.Now;
            //string sLOG_PATH = @"D:\Suhwoo\SG2000X\Log\SEQ\";
            string sLOG_PATH = GV.PATH_LOG+ @"LOGS\SEQ\";

            sPath = sLOG_PATH + Now.Year.ToString("0000") + Now.Month.ToString("00") + Now.Day.ToString("00_SEQ") + "\\";
            if (!Directory.Exists(sPath.ToString()))
            {
                Directory.CreateDirectory(sPath.ToString());
            }
            mLog_Delete(sLOG_PATH);
            sLine = DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss.fff]");
            sLine += sData;
            CLogManager Log = new CLogManager(sPath, null, null);
            Log.WriteLine(sLine);
        }
        #endregion
        #region 로그 저장
        /// <summary>
        /// Log 저장 함수
        /// </summary>
        /// 
        public static void mLogEncoder(string sData)
        {
            if (GV.ENC_LOG)
            {
                string sLine = "";
                string sPath;

                DateTime Now = DateTime.Now;
                //string sLOG_PATH = @"D:\Suhwoo\SG2000X\Log\SEQ\";
                string sLOG_PATH = GV.PATH_LOG + @"LOGS\SEQ\";

                sPath = sLOG_PATH + Now.Year.ToString("0000") + Now.Month.ToString("00") + Now.Day.ToString("00_Encoder") + "\\";

                if (!Directory.Exists(sPath.ToString()))
                {
                    Directory.CreateDirectory(sPath.ToString());
                }
                mLog_Delete(sLOG_PATH);
                //sLine = DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss.fff]");
                sLine += sData;
                CLogManager Log = new CLogManager(sPath, null, null);
                Log.WriteLine(sLine);
            }
        }
        public static void mLogGnd(string sData)
        {
            string sLine;
            string sPath;

            DateTime Now = DateTime.Now;
            //string sLOG_PATH = @"D:\Suhwoo\SG2000X\Log\SEQ\";
            string sLOG_PATH = GV.PATH_LOG + @"LOGS\SEQ\";

            sPath = sLOG_PATH + Now.Year.ToString("0000") + Now.Month.ToString("00") + Now.Day.ToString("00_GND") + "\\";
            if (!Directory.Exists(sPath.ToString()))
            {
                Directory.CreateDirectory(sPath.ToString());
            }
            mLog_Delete(sLOG_PATH);
            sLine = DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss.fff]");
            sLine += sData;
            CLogManager Log = new CLogManager(sPath, null, null);
            Log.WriteLine(sLine);
        }


        #endregion

        /// <summary>
        /// 큐에 로그 데이터 저장
        /// </summary>
        /// <param name="LT"></param>
        /// <param name="Module"></param>
        /// <param name="sLog"></param>
        public static void Save_Log(eLog Seq, eLog LT, string sLog)
        {
            if (CData.QueLog != null)
            {
                tMainLog tTemp = new tMainLog();
                tTemp.ESeq = Seq;
                tTemp.eType = LT;
                tTemp.dtTime = DateTime.Now;
                tTemp.sMsg = sLog;
                CData.QueLog.Enqueue(tTemp);
            }
        }

        public static void Save(tMainLog tVal)
        {
            eLog Seq = tVal.ESeq;
            eLog LT = tVal.eType;
            DateTime dtNow = tVal.dtTime;
            string sLog = tVal.sMsg;
            string sDir = "";

            string sH     = "";    //Header
            string sSeq   = "";    //ONL, INR, ONP, GRL, GRR, OFP, DRY, OFL
            string sEvent = "";    //OPL, DSL, RCL, ESL
            string sPart  = "";    //MOT, SPL_L, SPL_R, PRB_L, PRB_R, WPP_L, WPP_R

            string sNow = dtNow.ToString("[yyyyMMdd-HHmmss fff]");
            string sName = dtNow.ToString("yyyyMMddHH");

            StringBuilder sSB = new StringBuilder();
            sSB.Append(GV.PATH_LOG);
            sSB.Append("\\");
            sSB.Append(dtNow.Year.ToString("0000"));
            sSB.Append("\\");
            sSB.Append(dtNow.Month.ToString("00"));
            sSB.Append("\\");
            sSB.Append(dtNow.Day.ToString("00"));
            sSB.Append("\\");

            sDir = sSB.ToString();

            if (!Directory.Exists(sDir))
            {
                Directory.CreateDirectory(sDir);
            }

            sSeq = ((Seq != eLog.None) && ((Seq >= eLog.ONL) && (Seq <= eLog.OFL))) ? "[" + Seq.ToString() + "] " : sSeq;
            sH += sSeq;
            sEvent = (LT == eLog.MR) ? "[Manual]" : sEvent;
            sEvent = (LT.ToString() == eLog.OPL.ToString()) ? "[" + eLog.OPL.ToString() + "] " : sEvent;
            sEvent = (LT.ToString() == eLog.DSL.ToString()) ? "[" + eLog.DSL.ToString() + "] " : sEvent;
            sEvent = (LT.ToString() == eLog.RCL.ToString()) ? "[" + eLog.RCL.ToString() + "] " : sEvent;
            sEvent = (LT.ToString() == eLog.ESL.ToString()) ? "[" + eLog.ESL.ToString() + "] " : sEvent;
            sH += sEvent;
            sPart = (LT.ToString() == eLog.WMX.ToString()) ? "[" + eLog.WMX.ToString() + "] " : sPart;
            sPart = (LT.ToString() == eLog.MOT.ToString()) ? "[" + eLog.MOT.ToString() + "] " : sPart;
            sPart = (LT.ToString() == eLog.SPL_L.ToString()) ? "[" + eLog.SPL_L.ToString() + "] " : sPart;
            sPart = (LT.ToString() == eLog.SPL_R.ToString()) ? "[" + eLog.SPL_R.ToString() + "] " : sPart;
            sPart = (LT.ToString() == eLog.PRB_L.ToString()) ? "[" + eLog.PRB_L.ToString() + "] " : sPart;
            sPart = (LT.ToString() == eLog.PRB_R.ToString()) ? "[" + eLog.PRB_R.ToString() + "] " : sPart;
            sH += sPart;


            //Seq
            if (sSeq != "")
            {
                sSB.Append("Sequence\\");
                sSB.Append(Seq.ToString());
                sSB.Append("\\");
                if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                sSB.Append(sName);
                sSB.Append(".log");
                Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
            }
            sSB = new StringBuilder(sDir);

            if (sEvent != "")
            {
                if (LT == eLog.MR)
                {
                    sSB.Append("Manual\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.OPL.ToString())
                {
                    sSB.Append("OPL\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.DSL.ToString())
                {
                    sSB.Append("DSL\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.RCL.ToString())
                {
                    sSB.Append("RCL\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.ESL.ToString())
                {
                    sSB.Append("ESL\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);
            }

            if (sPart != "")
            {
                if (LT.ToString() == eLog.WMX.ToString())
                {
                    sSB.Append("WMX\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".csv");
                    Check_File_Access2(sSB.ToString(), sNow + "," + sH + "," + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.MOT.ToString())
                {
                    sSB.Append("MOT\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.SPL_L.ToString())
                {
                    sSB.Append("SPL_L\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.SPL_R.ToString())
                {
                    sSB.Append("SPL_R\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.PRB_L.ToString())
                {
                    sSB.Append("PRB_L\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);

                if (LT.ToString() == eLog.PRB_R.ToString())
                {
                    sSB.Append("PRB_R\\");
                    if (!Directory.Exists(sSB.ToString())) { Directory.CreateDirectory(sSB.ToString()); }
                    sSB.Append(sName);
                    sSB.Append(".log");
                    Check_File_Access2(sSB.ToString(), sNow + "\t" + sH + sLog, true);
                }
                sSB = new StringBuilder(sDir);
            }
        }

        //koo 191101 : error window 
        public static void Check_File_Access(string _path,string log,bool append)
        {
            //CSV 파일이 Open되어 있으면 강제적으로 Kill후 write한다.
            string filename = Path.GetFileName(_path);
            if (killps(filename)== true) m_Kill.Wait(2000);
            FileInfo fi = new FileInfo(_path);
            lock (lockObject)
            {
                if(append)
                {
                    try
                    {
                        using (FileStream file = new FileStream(fi.FullName, FileMode.Append, FileAccess.Write, FileShare.Read))
                        using (StreamWriter writer = new StreamWriter(file))
                        {
                            // You can add code here if file is accessible
                            writer.WriteLine(log);
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error", ex.Message);
                        return ;
                    }
                }
                else
                {
                    try
                    {
                        using (FileStream file = new FileStream(fi.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                        using (StreamWriter writer = new StreamWriter(file))
                        {
                            // You can add code here if file is accessible
                            writer.WriteLine(log);
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error", ex.Message);
                        return ;
                    }
                }
            }
        }

        //koo 191101 : error window 
        public static bool killps(string filename)
        {
            bool kill = false;

            //200324 ksg :
            string[] sp = filename.Split('.');
            if(sp[1] != "csv" && sp[1] != "Csv" && sp[1] != "CSV") return kill;

            string sFullFileName = filename + " - Excel";
            var excelProcesses = Process.GetProcessesByName("excel");

            foreach (var process in excelProcesses)
            {
                if (process.MainWindowTitle == $"Microsoft Excel - {filename}") // String.Format for pre-C# 6.0 
                {
                    process.Kill();
                    kill = true;
                }
                else if (process.MainWindowTitle == sFullFileName) //190501 ksg : Excel 파일 Close 조건 추가
                {
                    process.Kill();
                    kill = true;
                    
                }
                else if (process.MainWindowTitle == filename) //190501 ksg : Excel 파일 Close 조건 추가
                {
                    process.Kill();
                    kill = true;                    
                }
                else 
                {
                }
            }
            return kill;
        }

        public static void Check_File_Access2(string _path, string log, bool append)
        {
            //CSV 파일이 Open되어 있으면 강제적으로 Kill후 write한다.
            string filename = Path.GetFileName(_path);
            if (killps2(filename) == true) m_Kill2.Wait(2000);
            FileInfo fi = new FileInfo(_path);
            lock (lockObject2)
            {
                if (append)
                {
                    try
                    {
                        using (FileStream file = new FileStream(fi.FullName, FileMode.Append, FileAccess.Write, FileShare.Read))
                        using (StreamWriter writer = new StreamWriter(file))
                        {
                            // You can add code here if file is accessible
                            writer.WriteLine(log);
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error", ex.Message);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        using (FileStream file = new FileStream(fi.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
                        using (StreamWriter writer = new StreamWriter(file))
                        {
                            // You can add code here if file is accessible
                            writer.WriteLine(log);
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error", ex.Message);
                        return;
                    }
                }
            }
        }

        //koo 191101 : error window 
        public static bool killps2(string filename)
        {
            bool kill = false;

            //200324 ksg :
            string[] sp = filename.Split('.');
            if (sp[1] != "csv" && sp[1] != "Csv" && sp[1] != "CSV") return kill;

            string sFullFileName = filename + " - Excel";
            var excelProcesses = Process.GetProcessesByName("excel");

            foreach (var process in excelProcesses)
            {
                if (process.MainWindowTitle == $"Microsoft Excel - {filename}") // String.Format for pre-C# 6.0 
                {
                    process.Kill();
                    kill = true;
                }
                else if (process.MainWindowTitle == sFullFileName) //190501 ksg : Excel 파일 Close 조건 추가
                {
                    process.Kill();
                    kill = true;

                }
                else if (process.MainWindowTitle == filename) //190501 ksg : Excel 파일 Close 조건 추가
                {
                    process.Kill();
                    kill = true;
                }
                else
                {
                }
            }
            return kill;
        }


    }
}
