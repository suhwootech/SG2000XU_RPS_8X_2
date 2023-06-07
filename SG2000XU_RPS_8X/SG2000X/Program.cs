using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace SG2000X
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //CLog.mLogSeq("■ START ■");
            bool bNew;
            bool isDebug = false;
            //bool isChkLic = true;
            if (args.Length > 0)
            {
                foreach (string parm in args)
                {
                    if (parm.ToLower().Equals("debug"))
                        isDebug = true;
                    //if (parm.ToLower().Equals("nolic"))
                    //    isChkLic = true;
                }
            }

            //CDataOption.It.SetOption();
            if (!CDataOption.It.SetOptionNew(isDebug))
            {
                Application.Exit();
                return;
            }

            Application.AddMessageFilter(new AltF4Filter()); //200824 jhc : Alt+F4 키(종료) 막기

            CData.L_GRD = new CSq_Grd(EWay.L);
            CData.R_GRD = new CSq_Grd(EWay.R);
            Mutex mutex = new Mutex(true, "MutexName", out bNew);
            if (bNew)
            {
                // 소유권이 부여
                // 즉 해당 프로그램이 실행되고 있지 않은 경우
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                CData.FrmMain = new frmMain();
                Application.Run(CData.FrmMain);
                // 뮤텍스 릴리즈
                mutex.ReleaseMutex();
            }
            else
            {
                // 소유권이 부여되지 않음
                CMsg.Show(eMsg.Error, "Error", "이미실행중입니다.\r\n Already S/W Run");
                Application.Exit();
            }

        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            minidump.MiniDump.CreateMiniDump();
        }
    }

    //200824 jhc : Alt+F4 키(종료) 종료 막기
    public class AltF4Filter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            const int WM_SYSKEYDOWN = 0x0104;
            if (m.Msg == WM_SYSKEYDOWN)
            {
                bool alt = ((int)m.LParam & 0x20000000) != 0;
                if (alt && (m.WParam == new IntPtr((int)Keys.F4)))
                {
                    //Console.WriteLine("ALT+F4 Filtering.");
                    return true; //무시하기                
                }
            }
            return false;
        }
    }
}
