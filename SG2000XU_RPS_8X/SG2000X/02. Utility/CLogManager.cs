using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SG2000X
{
    public enum LogType { Daily,Monthly }
    class CLogManager : CStn<CLogManager>
    {
        private string _path;

        #region Constructors; 
        public CLogManager(string path,LogType logType,String prefix,String postfix)
        {
            _path = path;
            _SetLogPath(logType,prefix,postfix);
        }
        public CLogManager(String prefix,String postfix)
            :this(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Log"),LogType.Daily,prefix,postfix)
        {
        }
        public CLogManager(String FoldName,String prefix,String postfix)
            :this(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,FoldName),LogType.Daily,prefix,postfix)
        {
        }
        public CLogManager()
            :this(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Log"),LogType.Daily,null,null)
        {
        }
        #endregion
        #region Mothods
        public void _SetLogPath(LogType logType,String prefix,String postfix)
        {
            string path = String.Empty;
            string name = String.Empty;

            switch(logType)
            {
                case LogType.Daily:
                    path = String.Format(@"{0}\{1}", DateTime.Now.Year, DateTime.Now.ToString("MM"));
                    name = DateTime.Now.ToString("yyyyMMddHH");
                    break;
                case LogType.Monthly:
                    path = String.Format(@"{0}\", DateTime.Now.Year, DateTime.Now.ToString("MM"));
                    name = DateTime.Now.ToString("yyyyMMdd");
                    break;
            }
            //_path = System.IO.Path.Combine(_path, path);

            if (!System.IO.Directory.Exists(_path))
                System.IO.Directory.CreateDirectory(_path);
            if (!String.IsNullOrEmpty(prefix))
                name = prefix + name;
            if (!String.IsNullOrEmpty(postfix))
                name = name + postfix;
            name += ".txt";  

            _path = System.IO.Path.Combine(_path, name);
        }
        public void Write(string data)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(_path, true))
                {
                    writer.Write(data);
                }

            }
            catch(Exception ex)
            { }
        }
        public void WriteLine(string data)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(_path, true))
                {
                    //writer.WriteLine(System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss\t") + data);
                    //writer.WriteLine(System.DateTime.Now.ToString("yyyy.MM.dd_HH:mm.ss,") + data);
                    writer.WriteLine(data);
                }
            }
            catch (Exception ex)
            { }
        }
        #endregion
    }
}
