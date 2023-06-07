using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SG2000X
{
    public class CTim
    {
        private int m_iPre;
        private DateTime m_dtTar;
        //ksg
        private long SetTime;
        private long PreTime;
        private long CurTime;
        private Stopwatch stopwatch = new Stopwatch();

        public CTim(bool _bScan = false)
        {
        }

        public void Set_Delay(int iMs)
        {
            m_iPre = iMs;
            m_dtTar = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, iMs));
        }

        public bool Chk_Delay()
        {
            if (m_dtTar >= DateTime.Now)
            { return false; }
            else
            { return true; }
        }

        //ksg
        public long GetTickTime_ms()
        {
            return stopwatch.ElapsedTicks * 1000L / Stopwatch.Frequency;
        } 

        public bool OnDelay(bool SeqInput, long _SetTime)
        {
            SetTime = _SetTime;
            return OnDelay(SeqInput);
        }
        private bool OnDelay(bool SeqInput)
        {
            bool Result = false;
            if (SeqInput)
            {
                CurTime = GetTickTime_ms();
                if(CurTime==0)
                {
                    stopwatch = Stopwatch.StartNew();
                    PreTime = GetTickTime_ms();
                }
                if (CurTime-PreTime >= SetTime)
                {
                    //stopwatch = Stopwatch.StartNew();
                    PreTime = CurTime;
                    Result = true;
                }
                else
                {
                    Result=false;
                }
            }
            else
            {
                stopwatch = Stopwatch.StartNew();
                PreTime = GetTickTime_ms();

            }
            return Result;
        }

        public DateTime Wait(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }
    }
}
