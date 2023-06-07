//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// C# Gaus Library
// 
// CGxLog : 화면(ListBox)와 파일로 지정된 문자열을 기록한다.
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.IO;                // for Directory and File handling
using System.Text;              // for string data encoding
using System.Windows.Forms;     // for ListBox
using System.Diagnostics;       // for Stopwatch

// Log를 남겨주는 Namespace
namespace SG2000X.Gaus.Log
{
    public class TLogPrefixString
    {
        public string sPrefix;                          // 줄마다 앞에 기술해줄 문자열
        public string sSeqName;                         // 시퀀스 함수 이름
        public int nSubStep;                            // 시퀀스 실행 하위 스텝 순번

        // 선두 문자열를 만들어준다. 만들어진 문자열을 사용하거나 원하는 형식을 새로이 만들 수 있다.
        public void SetPrefix(string sName, int nStep)
        {
            sSeqName = sName;
            nSubStep = nStep;
            sPrefix = $"{sName}, {nStep:0#}, ";
        }

        // 지정한 문자열로 선두 문자열을 곧바로 지정한다.
        public void SetPrefix(string sMsg)
        {
            sPrefix = sMsg;
        }

        // 현재 지정된 선두 문자열을 조회한다.
        public string GetPrefix()
        {
            return sPrefix;
        }

    }

    public class CGxLog
    {
        public bool m_bUnicode = true;                  // Unicode로 저장할것인가 ? 
        public bool m_bHourCut = false;                 // 시간단위로 파일을 생성할 것인가 ?
        public bool m_bTimePrint = true;                // 시각을 출력할 것인가 ?
        public bool m_bFilenameDate = true;             // File 이름에 자동으로 날짜를 붙일 것인가 ?
        public bool m_bFileWrite = false;               // 파일로 기록을 할것인가 ?

        public ListBox  m_myList = null;                // log를 기록할 List Box control
        private readonly object m_thisLock = new object();// thread-safety를 위한 lock

        public int m_nMaxLine = 100;                      // ListBox에 표시하는 문자열 줄수 지정
        public Encoding m_Encoding = Encoding.UTF8;

        public string m_sFileExt = "log";              // 확장자
        public string m_sItemName = "Log";                 // 어떠한 내용을 기록할 것인지 파일이름에 나타낸다.
        public string m_sGroupName = "";                // Log의 저장 Group

        public string m_sFileName = "";                 // 기록할 파일 이름
        public string m_sFilePath = "";                 // 파일을 기록할 전체 경로
        public string m_sFileBase = "";                 // 파일을 기록할 기본 경로
        public string m_sDate = "";                     // 호출된 날짜
        public string m_sTime = "";                     // 호출된 시각
        public string m_sHeader = "";                   // Header 기록

        public System.DateTime m_dtNow = System.DateTime.Now;       // 현재의 시각을 기억

        public TLogPrefixString m_rPrefix = null;          // 줄마다 앞에 기술해줄 문자열

        protected string m_sBody = "";                  // 기록을 할 문자열 문장
        protected bool m_bHeaderWrite = false;          // Header를 기록해야하는가 ?

        public CGxLog()
        {

        }

        // Prefix 문자열을 만들 수 있는 class 객체를 지정해준다.
        public void SetPrefixObject(TLogPrefixString pObj)
        {
            m_rPrefix = pObj;
        }

        public void SetListBox(ListBox pList)           // 화면에 표시하기위한 ListBox 지정
        {
            m_myList = pList;
        }

        public void SetFileName(string sPath, string sName) // 파일이 기록될 경로를 지정한다.
        {
            m_sFileBase = sPath;
            m_sGroupName = "";
            m_sItemName = sName;

            m_bFileWrite = true;
        }

        // Group Name을 추가로 지정해준다.
        public void SetFileName(string sPath, string sGroup, string sName) // 파일이 기록될 경로를 지정한다.
        {
            m_sFileBase = sPath;
            m_sGroupName = sGroup;
            m_sItemName = sName;

            m_bFileWrite = true;
        }

        // 파일로 기록을 할것인가 ?
        public void SetEnabled(bool bFlag)
        {
            m_bFileWrite = bFlag;
        }

        // 현재 log 기록이 활성화 되어있는가 ?
        public bool GetEnabled()
        {
            return m_bFileWrite;
        }

        // Log 내용에 현재 시각을 함께 기록할 것인가 ?
        public void SetTimePrint(bool bFlag)
        {
            m_bTimePrint = bFlag;
        }

        //public void SetUnicode(bool bFlag)              // Unicode(UTF8)로 저장할 것인가 ? 
        //{
        //    m_Encoding = bFlag ? System.Text.Encoding.UTF8 : System.Text.Encoding.Default;
        //}


        public void Write(string sMsg)
        {
            if (string.IsNullOrEmpty(sMsg)) return;                 // 출력할 내용이 없다면 곧바로 종료시킨다.
            if (!m_bFileWrite && (m_myList == null)) return;        // 파일과 List로 기록하지 않게 설정되어있다면

            // 호출된 시각을 날짜와 시간으로 분리한다.
            m_dtNow = System.DateTime.Now;       // 현재의 시각을 기억

            // 호출된 시각을 날짜와 시간으로 분리한다.
            // 시간단위로 생성한다면 시간도 날짜에 포함하여 파일 생성시 사용한다.
            m_sDate = m_bHourCut ? System.DateTime.Now.ToString("yyyy-MM-dd_HH") : System.DateTime.Now.ToString("yyyy-MM-dd");
            m_sTime = m_dtNow.ToString("HH:mm:ss.fff ");


            // 시간 데이터를 출력하는가 ?
            if (m_bTimePrint)
            {
                // 선두 문자를 출력할 것인가 ?
                if (m_rPrefix != null)
                {
                    m_sBody = m_sTime + m_rPrefix.sPrefix + sMsg;
                }
                else
                    m_sBody = m_sTime + sMsg;
            }
            else
            {
                if (m_rPrefix != null)
                {
                    m_sBody = m_rPrefix.sPrefix + sMsg;
                }
                else
                    m_sBody = sMsg;
            }


            // ListBox에 내용을 추가한다.
            if (m_myList != null)
            {
                // 생성된 Main UI Thread가 아닌 다른 Thread에서 호출되었을때 수행
                if (m_myList.InvokeRequired)
                {
                    m_myList.Invoke((MethodInvoker)(() =>
                    {
                        m_myList.Items.Add(m_sBody);
                        if (m_myList.Items.Count > m_nMaxLine) m_myList.Items.RemoveAt(0);        // 최초 1줄을 삭제한다.

                        m_myList.SelectedIndex = m_myList.Items.Count - 1;                      // 최종 추가된 줄을 표시한다.
                    }));
                }
                else
                {
                    try
                    {
                        m_myList.Items.Add(m_sBody);
                        if (m_myList.Items.Count > m_nMaxLine) m_myList.Items.RemoveAt(0);        // 최초 1줄을 삭제한다.
                        m_myList.SelectedIndex = m_myList.Items.Count - 1;
                    }
                    catch // (Exception ex)
                    {
                        Debug.WriteLine($"* Exception ! GxLog {m_sFileName} : {sMsg}");
                    }
                }
            }


            if (!m_bFileWrite) return;                          // 파일로 기록하지 않는다면 곧바로 나간다. 

            // File에 내용을 기록한다.
            if (m_sGroupName == "")
            {
                m_sFilePath = $"{m_sFileBase}\\{m_dtNow.Year}\\{m_dtNow.Month:0#}\\{m_dtNow.Day:0#}";  // 지정 폴더 아래 날짜로된 폴더 생성
            }
            else
                m_sFilePath = $"{m_sFileBase}\\{m_dtNow.Year}\\{m_dtNow.Month:0#}\\{m_dtNow.Day:0#}\\{m_sGroupName}";  // Group Name까지 하위 폴더를 추가한다.

            if (m_bFilenameDate)             // File 이름에 자동으로 날짜를 붙일 것인가 ?
            {
                m_sFileName = m_sFilePath + @"\" + m_sDate + "_" + m_sItemName + "." + m_sFileExt;     // 기록할 파일의 전체 경로 생성
            }
            else
            {
                m_sFileName = m_sFilePath + @"\" + m_sItemName + "." + m_sFileExt;     // 파일이름에 날짜를 넣지 않는다.
            }

            // 지정 폴더가 존재하지 않는다면 폴더를 생성한다.
            if (Directory.Exists(m_sFilePath) != true)
            {
                Directory.CreateDirectory(m_sFilePath);           // 폴더 생성
            }

            if (m_sHeader != "")
            {
                m_bHeaderWrite = !File.Exists(m_sFileName);     // 파일이 존재하지 않느다면 Header를 출력해준다.
            }
            else
            {
                m_bHeaderWrite = false;
            }

            lock (m_thisLock)
            {
                // ASCII로 기록이 되지 않는다.... (MBCS) 나중에 log 용량을 줄이기위해 연구하여 구현해야한다.
                //
                //if (m_Encoding == Encoding.Default)
                //using (StreamWriter sw = new StreamWriter(m_sFileName, true, m_Encoding, 4096))
                //{
                //    byte[] byteArray = Encoding.UTF8.GetBytes(m_sBody);
                //    byte[] asciiArray = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, byteArray);

                //    sw.WriteLine(Encoding.ASCII.GetString(asciiArray));
                //    sw.Close();
                //}
                //else



                //// FileStream 사용 : 마찬가지로 느리다. 60ms 소요
                //using (FileStream file = new FileStream(m_sFileName, FileMode.Open, FileAccess.Write))
                //{
                //    using (StreamWriter sw = new StreamWriter(file))
                //    {
                //        sw.WriteLine(m_sBody);
                //        sw.Close();
                //    }

                //    file.Close();
                //}



                //if (!File.Exists(m_sFileName))
                //{
                //    // 가장 빠른 기록, 20ms 이내로 기록된다.
                //    using (StreamWriter sw = new StreamWriter(File.Create(m_sFileName)))
                //    {
                //        sw.WriteLine(m_sBody);
                //        sw.Close();
                //    }
                //}
                //else
                //{
                //    File.AppendAllText(m_sFileName, System.Environment.NewLine);
                //    File.AppendAllText(m_sFileName, m_sBody);
                //}




                //김명균 차장님 방법, 점점 느려짐
                //using (StreamWriter swFile = File.AppendText(m_sFileName))
                //{
                //    swFile.WriteLine(m_sBody);
                //    swFile.Flush();
                //    swFile.Close();
                //}



                // 약 70ms 정도 소요
                using (StreamWriter sw = new StreamWriter(m_sFileName, true, m_Encoding, 4096))
                {
                    if (m_bHeaderWrite)
                    {
                        sw.WriteLine(m_sHeader);        // Header를 먼저 출력한다.
                    }

                    sw.WriteLine(m_sBody);
                    sw.Close();
                }

            }
        }//of void Write()
    }



}
