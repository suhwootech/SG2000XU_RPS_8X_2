using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SG2000X
{
    /// <summary>
    /// 파라미터,옵션 변경사항 저장
    /// 2020.07.11 lks
    /// </summary>
    static class CCheckChange
    {
        public static void CheckChanged(string title, string path, string oldData, string newData)
        {
            bool isNew = false;
            if (oldData == null || oldData.Equals("EPT")) isNew = true;

            try
            {
                //Console.WriteLine("==================================================================================");
                Dictionary<string, Hashtable> dicOld = new Dictionary<string, Hashtable>();
                Dictionary<string, Hashtable> dicNew = new Dictionary<string, Hashtable>();

                if (!isNew) dicOld = ConvertDicToStr(oldData);
                dicNew = ConvertDicToStr(newData);

                string changeLog = string.Empty;

                foreach (KeyValuePair<string, Hashtable> valDic in dicNew)
                {
                    foreach (DictionaryEntry valHt in valDic.Value)
                    {
                        if (isNew)
                        {
                            changeLog += valDic.Key + " " + valHt.Key + "=" + valHt.Value + "\r\n";
                        }
                        else
                        {
                            if (dicOld.ContainsKey(valDic.Key))
                            {
                                if (dicOld[valDic.Key].ContainsKey(valHt.Key))
                                {
                                    if (!dicOld[valDic.Key][valHt.Key].Equals(valHt.Value))
                                    {
                                        changeLog += valDic.Key + " " + valHt.Key + "=" + dicOld[valDic.Key][valHt.Key].ToString() + " -> " + valHt.Value + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(changeLog))
                {
                    Console.WriteLine(changeLog);
                    if(isNew)
                        CLog.Save_Log(eLog.None, eLog.DSL, title + " : New - log (" + path + ")\r\n" + changeLog);
                    else
                        CLog.Save_Log(eLog.None, eLog.DSL, title + " : Changed - log (" + path + ")\r\n" + changeLog);
                }
            }
            catch (Exception err)
            {
                CLog.Save_Log(eLog.None, eLog.DSL, title + " : Error - log (" + path + ") Msg : " + err.Message);
            }
        }

        public static void DeleteLog(string title, string path)
        {
            CLog.Save_Log(eLog.None, eLog.DSL, title + " : Delete - log (" + path + ")");
        }

        public static void SaveAs(string title, string srcPath, string tarPath)
        {
            CLog.Save_Log(eLog.None, eLog.DSL, title + " : SaveAs - log \r\n" + srcPath + " -> " + tarPath +")");
        }

        private static Dictionary<string, Hashtable> ConvertDicToStr(string str)
        {
            Dictionary<string, Hashtable> dicRet = new Dictionary<string, Hashtable>();
            Hashtable htTemp = new Hashtable();
            string tempKey = string.Empty;

            string[] newStrLines = str.Split('\n');
            for (int i = 0; i < newStrLines.Length; i++)
            {
                newStrLines[i] = newStrLines[i].Replace("\r", "").Trim();

                if (newStrLines[i].Contains("[") && !tempKey.Equals(newStrLines[i]))
                {                    
                    if (htTemp.Count > 0)
                    {
                        if (dicRet.ContainsKey(tempKey))
                        {
                            foreach (DictionaryEntry add in htTemp)
                            {
                                dicRet[tempKey].Add(add.Key,add.Value);
                            }
                        }
                        else
                            dicRet.Add(tempKey, htTemp);
                    }
                    tempKey = newStrLines[i];
                    htTemp = new Hashtable();
                }
                else if (!string.IsNullOrEmpty(newStrLines[i]))
                {
                    string[] convString = newStrLines[i].Split('=');
                    htTemp.Add(convString[0], convString[1]);
                }
            }

            if (htTemp.Count > 0)
            {
                if (dicRet.ContainsKey(tempKey))
                {
                    foreach (DictionaryEntry add in htTemp)
                    {
                        dicRet[tempKey].Add(add.Key, add.Value);
                    }
                }
                else
                    dicRet.Add(tempKey, htTemp);
            }

            return dicRet;
        }

        public static string ReadOldFile(string path)
        {
            string ret = "";
            string sPath = path;

            if (File.Exists(sPath))
            {
                try
                {
                    ret = File.ReadAllText(sPath);
                }
                catch
                {
                    ret = "ERR";
                }
            }
            else
            {
                ret = "EPT";
            }

            return ret;
        }
    }
}
