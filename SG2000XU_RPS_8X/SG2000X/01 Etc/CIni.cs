using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SG2000X
{
    public class CIni
    {
        private string m_sPath = "";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
                            string section,
                            string key,
                            string val,
                            string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
                            string section,
                            string key,
                            string def,
                            StringBuilder retVal,
                            int size,
                            string filePath);

        public CIni(string sPath)
        {
            //if (strPath.Contains(".ini") == false)
            //    strPath += ".ini";

            m_sPath = sPath;
        }

        public bool ExistINI()
        {
            return File.Exists(m_sPath);
        }

        public void Write(string sSection, string sKey, int iVal)
        {
            Write(sSection, sKey, iVal.ToString());
        }

        public void Write(string sSection, string sKey, double dVal)
        {
            Write(sSection, sKey, dVal.ToString());
        }

        public void Write(string sSection, string sKey, string sVal)
        {
            WritePrivateProfileString(sSection, sKey, sVal, m_sPath);
        }

        public void DeleteSection(string sSection)
        {
            WritePrivateProfileString(sSection, null, null, m_sPath);
        }

        public string Read(string sSection, string sKey)
        {
            StringBuilder sValue = new StringBuilder(255);
            int i = GetPrivateProfileString(sSection, sKey, "", sValue, 255, m_sPath);
            return sValue.ToString();
        }

        /// <summary>
        /// Read 후 Int형으로 반환
        /// </summary>
        /// <param name="sSection"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public int ReadI(string sSection, string sKey)
        {
            string sVal = Read(sSection, sKey);
            if (sVal == "")
                sVal = "0";
            return Convert.ToInt32(sVal);
        }

        /// <summary>
        /// Read 후 Double형으로 반환
        /// </summary>
        /// <param name="sSection"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public double ReadD(string sSection, string sKey)
        {
            string sVal = Read(sSection, sKey);
            if (sVal == "")
                sVal = "0";
            return Convert.ToDouble(sVal);
        }
    }
}

