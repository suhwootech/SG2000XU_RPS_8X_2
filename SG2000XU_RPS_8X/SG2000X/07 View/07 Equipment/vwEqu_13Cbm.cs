using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;



namespace SG2000X
{
    public partial class vwEqu_13Cbm : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>

        private enum eManStep
        {
            Idle,
            Ready,
            Sol_On,
            Sol_Off,
            Stop
        }

        private enum eAutoStep
        {
            Idle,
            Ready,
            Sol_On,
            Sol_Off,
            Delay,
            Stop
        }

        private Timer m_tmUpdt;
        private int m_nPreSel = -1;

        private bool m_bManualMode = false;
        private bool m_bAutoMode = false;

        private int m_nManStep = (int)eManStep.Idle;
        private int m_nAutoStep = (int)eAutoStep.Idle;
        private int m_nPreAutoStep = (int)eAutoStep.Idle;
        private bool m_bInStep = false;

        private int m_nWait_ms = 1000;
        private int m_nRepeatNum = 10;
        private int m_nRepeatCnt = 0;


        CCylinder m_SelCyld = new CCylinder();

        private CTim m_ManDelay = new CTim();
        private CTim m_AutoDelay = new CTim();
        private Stopwatch m_Stopwatch = new Stopwatch();
        List<long> listResultTime = new List<long>();


       



        public vwEqu_13Cbm()
        {
            if ((int)ELang.English == CData.Opt.iSelLan)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)ELang.Korea == CData.Opt.iSelLan)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)ELang.China == CData.Opt.iSelLan)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            InitializeComponent();
            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;            
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            Display();

            if (m_bManualMode)
            {
                CyldManRun();
            }
            else if (m_bAutoMode)
            {
                CyldAutoRun();
            }
            else
            {

            }
        }


        /// <summary>
        /// Cylinder 수동 동작
        /// </summary>
        private void CyldManRun()
        {
            if (m_nManStep == (int)eManStep.Idle)
            {
                // Idle
            }
            else if (m_nManStep == (int)eManStep.Ready)
            {
                //
            }
            else if (m_nManStep == (int)eManStep.Sol_On)
            {
                if (m_bInStep) // 이 Step에 처음 들어 왔으면
                {
                    m_bInStep = false;
                    m_Stopwatch.Restart();
                    m_ManDelay.Set_Delay(5000);
                }

                bool bRet = Func_Cylinder_OnOff(true);
                if (bRet)
                {
                    long lElaped = m_Stopwatch.ElapsedMilliseconds;
                    m_Stopwatch.Stop();

                    string sMsg1 = string.Format("CBM Cylinder Test : {0} = ", m_SelCyld.sName);
                    string sMsg2 = string.Format("#{0}  [Man Off -> On]  Time: {1} ms\r\n", m_nRepeatCnt + 1, lElaped);
                    sMsg1 += sMsg2;
                    _SetLog(sMsg1);

                    richCyldMeas.AppendText(sMsg2);
                    richCyldMeas.ScrollToCaret();

                    m_nManStep = (int)eManStep.Idle;
                    m_bInStep = true;
                    m_nRepeatCnt++;
                }

                if (m_ManDelay.Chk_Delay())
                {
                    m_Stopwatch.Stop();
                    m_nManStep = (int)eManStep.Idle;
                    m_bInStep = true;
                }
            }
            else if (m_nManStep == (int)eManStep.Sol_Off)
            {
                if (m_bInStep) // 이 Step에 처음 들어 왔으면
                {
                    m_bInStep = false;
                    m_Stopwatch.Restart();
                    m_ManDelay.Set_Delay(5000);
                }

                bool bRet = Func_Cylinder_OnOff(false);
                if (bRet)
                {
                    long lElaped = m_Stopwatch.ElapsedMilliseconds;
                    m_Stopwatch.Stop();

                    string sMsg1 = string.Format("CBM Cylinder Test : {0} = ", m_SelCyld.sName);
                    string sMsg2 = string.Format("#{0}  [Man On -> Off]  Time: {1} ms\r\n", m_nRepeatCnt + 1, lElaped);
                    sMsg1 += sMsg2;
                    _SetLog(sMsg1);

                    richCyldMeas.AppendText(sMsg2);
                    richCyldMeas.ScrollToCaret();

                    m_nManStep = (int)eManStep.Idle;
                    m_bInStep = true;
                    m_nRepeatCnt++;
                }

                if (m_ManDelay.Chk_Delay())
                {
                    m_Stopwatch.Stop();
                    m_nManStep = (int)eManStep.Idle;
                    m_bInStep = true;

                }
            }
        }

        /// <summary>
        /// Cylinder 반복 수행, 동작시간 측정
        /// </summary>
        private void CyldAutoRun()
        {
            //-------------------------------------------
            if (m_nAutoStep == (int)eAutoStep.Idle)
            {
                // Idle
            }
            //-------------------------------------------
            else if (m_nAutoStep == (int)eAutoStep.Ready) // Backward에서 시작 준비
            {
                if (m_bInStep) // 이 Step에 처음 들어 왔으면
                {
                    m_bInStep = false;
                    m_AutoDelay.Set_Delay(5000);
                }
                
                bool bRet = Func_Cylinder_OnOff(false);
                if (bRet)
                {
                    m_nPreAutoStep = m_nAutoStep;
                    m_nAutoStep = (int)eAutoStep.Delay;
                    m_bInStep = true;
                    m_nRepeatCnt = 0;

                    richCyldMeas.Clear();
                    listResultTime.Clear();
                }

                if (m_AutoDelay.Chk_Delay())
                {
                    m_nAutoStep = (int)eAutoStep.Idle;
                    m_bInStep = true;
                }
            }
            //-------------------------------------------
            else if (m_nAutoStep == (int)eAutoStep.Sol_On)
            {
                if (m_bInStep) // 이 Step에 처음 들어 왔으면
                {
                    m_bInStep = false;
                    m_Stopwatch.Restart();
                    m_AutoDelay.Set_Delay(5000);
                }

                bool bRet = Func_Cylinder_OnOff(true);
                if (bRet)
                {
                    long lElaped = m_Stopwatch.ElapsedMilliseconds;
                    m_Stopwatch.Stop();
                    listResultTime.Add(lElaped);

                    string sMsg1 = string.Format("CBM Cylinder Test : {0} = ", m_SelCyld.sName);
                    string sMsg2 = string.Format("#{0}  [Auto Off -> On]  Time: {1} ms\r\n", m_nRepeatCnt + 1, lElaped);
                    sMsg1 += sMsg2;
                    _SetLog(sMsg1);

                    richCyldMeas.AppendText(sMsg2);
                    richCyldMeas.ScrollToCaret();

                    if (m_nRepeatCnt + 1 >= m_nRepeatNum)
                    {
                        ResultTime();

                        m_nAutoStep = (int)eAutoStep.Idle;
                        m_bInStep = true;
                        return;
                    }

                    m_nPreAutoStep = m_nAutoStep;
                    m_nAutoStep = (int)eAutoStep.Delay;
                    m_bInStep = true;
                    m_nRepeatCnt++;
                }

                if (m_AutoDelay.Chk_Delay())
                {
                    m_Stopwatch.Stop();
                    m_nAutoStep = (int)eAutoStep.Idle;
                    m_bInStep = true;
                }
            }
            //-------------------------------------------
            else if (m_nAutoStep == (int)eAutoStep.Sol_Off)
            {
                if (m_bInStep) // 이 Step에 처음 들어 왔으면
                {
                    m_bInStep = false;
                    m_Stopwatch.Restart();
                    m_AutoDelay.Set_Delay(5000);
                }

                bool bRet = Func_Cylinder_OnOff(false);
                if (bRet)
                {
                    long lElaped = m_Stopwatch.ElapsedMilliseconds;
                    m_Stopwatch.Stop();
                    listResultTime.Add(lElaped);

                    string sMsg1 = string.Format("CBM Cylinder Test : {0} = ", m_SelCyld.sName);
                    string sMsg2 = string.Format("#{0}  [Auto On -> Off]  Time: {1} ms\r\n", m_nRepeatCnt + 1, lElaped);
                    sMsg1 += sMsg2;
                    _SetLog(sMsg1);

                    richCyldMeas.AppendText(sMsg2);
                    richCyldMeas.ScrollToCaret();

                    if (m_nRepeatCnt + 1 >= m_nRepeatNum)
                    {
                        ResultTime();

                        m_nAutoStep = (int)eAutoStep.Idle;
                        m_bInStep = true;
                        return;
                    }

                    m_nPreAutoStep = m_nAutoStep;
                    m_nAutoStep = (int)eAutoStep.Delay;
                    m_bInStep = true;
                    m_nRepeatCnt++;
                }

                if (m_AutoDelay.Chk_Delay())
                {
                    m_Stopwatch.Stop();
                    m_nAutoStep = (int)eAutoStep.Idle;
                    m_bInStep = true;
                }
            }
            //-------------------------------------------
            else if (m_nAutoStep == (int)eAutoStep.Delay)
            {
                if (m_bInStep)
                {
                    m_bInStep = false;
                    m_AutoDelay.Set_Delay(m_nWait_ms);
                }

                if (m_AutoDelay.Chk_Delay())
                {
                    if      (m_nPreAutoStep == (int)eAutoStep.Ready) { m_nAutoStep = (int)eAutoStep.Sol_On; }
                    else if (m_nPreAutoStep == (int)eAutoStep.Sol_On) { m_nAutoStep = (int)eAutoStep.Sol_Off; }
                    else if (m_nPreAutoStep == (int)eAutoStep.Sol_Off) { m_nAutoStep = (int)eAutoStep.Sol_On; }
                    else { }
                    m_bInStep = true;
                }
            }
            //-------------------------------------------
        }

        private bool Func_Cylinder_OnOff(bool bSolOn)
        {
            bool bRet = false;
            if (m_SelCyld.sName == "InRail_Probe")
            {
                bRet = Func_InRail_ProbeDown(bSolOn);
            }
            else if (m_SelCyld.sName == "Left_Probe")
            {
                bRet = Func_GrdL_ProbeDown(bSolOn);
            }
            else if (m_SelCyld.sName == "Right_Probe")
            {
                bRet = Func_GrdR_ProbeDown(bSolOn);
            }
            else
            {
                // Out 
                CIO.It.Set_Y((eY)m_SelCyld.nOut_01_Addr, bSolOn);
                if (m_SelCyld.bDualOut)
                {
                    CIO.It.Set_Y((eY)m_SelCyld.nOut_02_Addr, !bSolOn);
                }

                // In Check
                if (bSolOn)
                {
                    bRet = CIO.It.Get_X((eX)m_SelCyld.nIn_01_Addr);
                    if (m_SelCyld.bDualIn)
                    {
                        bRet = (CIO.It.Get_X((eX)m_SelCyld.nIn_01_Addr) && !CIO.It.Get_X((eX)m_SelCyld.nIn_02_Addr));
                    }
                }
                else
                {
                    bRet = !CIO.It.Get_X((eX)m_SelCyld.nIn_01_Addr);
                    if (m_SelCyld.bDualIn)
                    {
                        bRet = (!CIO.It.Get_X((eX)m_SelCyld.nIn_01_Addr) && CIO.It.Get_X((eX)m_SelCyld.nIn_02_Addr));
                    }
                }
            }
            return bRet;
        }


        private bool Func_InRail_ProbeDown(bool bDn)
        {
            CIO.It.Set_Y(eY.INR_ProbeDn, bDn);
            return (CIO.It.Get_X(eX.INR_ProbeDn) == bDn)  && (!CIO.It.Get_X(eX.INR_ProbeUp) == bDn);
        }

        private bool Func_GrdL_ProbeDown(bool bDn)
        {
            CIO.It.Set_Y(eY.GRDL_ProbeDn, bDn);
            return (CIO.It.Get_Y(eY.GRDL_ProbeDn) == bDn) && (!CIO.It.Get_X(eX.GRDL_ProbeAMP) == bDn);
        }

        private bool Func_GrdR_ProbeDown(bool bDn)
        {
            CIO.It.Set_Y(eY.GRDR_ProbeDn, bDn);
            return (CIO.It.Get_Y(eY.GRDR_ProbeDn) == bDn) && (!CIO.It.Get_X(eX.GRDR_ProbeAMP) == bDn);
        }



        private void ResultTime()
        {
            long lMin = listResultTime.Min();
            long lMax = listResultTime.Max();
            double dAvg = listResultTime.Average();

            string sMsg = string.Format("Min : {0} ms,   Max : {1} ms,   Average : {2} ms\r\n", lMin, lMax, (long)dAvg);
            richCyldMeas.AppendText(sMsg);

            _SetLog(sMsg);

            btn_Save.Enabled = true;
        }


        private void Display()
        {
            if (m_nManStep == (int)eManStep.Idle && m_nAutoStep == (int)eAutoStep.Idle)
            {
                listboxCylinder.Enabled = true;
            }

            string sText = "";
            Color cr = Color.WhiteSmoke;

            lbIn_01.BackColor = cr;
            lbIn_02.BackColor = cr;
            lbOut_01.BackColor = cr;
            lbOut_02.BackColor = cr;
            lbIn_01.Text = "";
            lbIn_02.Text = "";
            lbOut_01.Text = "";
            lbOut_02.Text = "";

            if (listboxCylinder.SelectedIndex < 0)
                return;

            sText = m_SelCyld.sIn_01_Addr + " : " + m_SelCyld.sIn_01_Name; lbIn_01.Text = sText;
            sText = m_SelCyld.sIn_02_Addr + " : " + m_SelCyld.sIn_02_Name; lbIn_02.Text = sText;
            sText = m_SelCyld.sOut_01_Addr + " : " + m_SelCyld.sOut_01_Name; lbOut_01.Text = sText;
            sText = m_SelCyld.sOut_02_Addr + " : " + m_SelCyld.sOut_02_Name; lbOut_02.Text = sText;

            lbIn_01.BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_X((eX)m_SelCyld.nIn_01_Addr))];
            if (m_SelCyld.bDualIn)
                lbIn_02.BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_X((eX)m_SelCyld.nIn_02_Addr))];
            lbOut_01.BackColor = GV.CR_Y[Convert.ToInt32(CIO.It.Get_Y(m_SelCyld.nOut_01_Addr))];
            if (m_SelCyld.bDualOut)
                lbOut_02.BackColor = GV.CR_Y[Convert.ToInt32(CIO.It.Get_Y(m_SelCyld.nOut_02_Addr))];
        }


        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
            // 타이머에 대한 모든 리소스 해제
            if (m_tmUpdt != null)
            {
                Close();
                m_tmUpdt.Dispose();
                m_tmUpdt = null;
            }
        }

        /// <summary>
        /// 데이터를 화면에 출력
        /// </summary>
        private void _Set()
        { 
            CCbm.It.Load();

            listboxCylinder.Items.Clear();
            tab_AutoView.SelectedTab = tabPage_Cyl;

            for (int i = 0; i < CCbm.It.m_alCylinder.Count; i++)
            {
                CCylinder cyld = CCbm.It.m_alCylinder[i] as CCylinder;

                // 리스트박스에 실린더 추가
                listboxCylinder.Items.Add(cyld.sName);
            }

        }

        /// <summary>
        /// 화면에 값을 데이터로 저장
        /// </summary>
        private void _Get()
        {

        }

        /// <summary>
        /// Manual view에 조작 로그 저장 함수
        /// </summary>
        /// <param name="sMsg"></param>
        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.OPL, string.Format("{0}.cs {1}() {2} Lv:{3}", sCls, sMth, sMsg, CData.Lev));
        }
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        /// 
        public void Open()
        {

            //CCbm.It.sCBM_PATH;
            _Set();
            /*
            if (fi.Exists)
            {
                
                CCbm.It.Open_Port();
                CCbm.It.LightOn1();
                CCbm.It.LightOn2();
              
            }
          */
            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled) { m_tmUpdt.Start(); }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {         
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled) { m_tmUpdt.Stop(); }
        }
        #endregion

        private void btn_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");

            int iRet = 0;

            //_Get();
            iRet = CCbm.It.Save(m_SelCyld.sName, richCyldMeas.Text);
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "CBM_Cylinder Result file save fail");
            }
            else
            {
                CMsg.Show(eMsg.Notice, "Notice", "CBM_Cylinder Result file save success");
            }
        }


        private void lbxCylList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSel = listboxCylinder.SelectedIndex;
            if (nSel < 0)
                return;

            if (CCbm.It.m_alCylinder.Count < 1)
                return;

            if (m_nManStep != (int)eManStep.Idle || m_nAutoStep != (int)eAutoStep.Idle)
            {
                listboxCylinder.SelectedIndex = m_nPreSel;
                return;
            }

            m_nPreSel = nSel;

            m_SelCyld = CCbm.It.m_alCylinder[nSel] as CCylinder;

            Display();


        }


        private void btnManual_SolOn_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Cylinder:" + m_SelCyld.sName);

            m_bManualMode = true;
            m_bAutoMode = false;

            m_nManStep = (int)eManStep.Sol_On;
            m_bInStep = true;
           
            listboxCylinder.Enabled = false;

        }

        private void btnManual_SolOff_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Cylinder:" + m_SelCyld.sName);

            m_bManualMode = true;
            m_bAutoMode = false;

            m_nManStep = (int)eManStep.Sol_Off;
            m_bInStep = true;

            listboxCylinder.Enabled = false;
        }


        private void btnAutoRunStart_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Cylinder:" + m_SelCyld.sName);

            m_bAutoMode = true;
            m_bManualMode = false;

            m_nAutoStep = (int)eAutoStep.Ready;
            m_bInStep = true;
            m_nRepeatCnt = 0;
            m_nRepeatNum = int.Parse(textRepeatNum.Text);
            m_nWait_ms = int.Parse(textDelay_ms.Text);

            listboxCylinder.Enabled = false;
            btn_Save.Enabled = false;
        }

        private void btnAutoRunStop_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Cylinder:" + m_SelCyld.sName);

            m_bAutoMode = false;
            m_nAutoStep = (int)eAutoStep.Idle;
            listboxCylinder.Enabled = true;
        }

        private void btnLogClear_Click(object sender, EventArgs e)
        {
            richCyldMeas.Clear();
        }

    }
}
