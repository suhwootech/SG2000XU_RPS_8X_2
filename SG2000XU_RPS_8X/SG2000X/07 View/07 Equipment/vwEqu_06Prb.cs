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

namespace SG2000X
{
    public partial class vwEqu_06Prb : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        private CTim m_CheckTime_0 = new CTim();
        private CTim m_CheckTime_1 = new CTim();
        private CTim m_CheckTime_2 = new CTim();
        private CTim m_CheckTime_3 = new CTim();

        // 포지션 Get 버튼 클릭 이벤트 //syc : Probe Auto Calibration Position
        private int m_iAx1 = 0;
        private int m_iAx2 = 0;
        private int m_iAx3 = 0;

        public vwEqu_06Prb()
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

            //if ((CData.CurCompany == eCompany.Qorvo) || (CData.CurCompany == eCompany.SkyWorks) || (CData.CurCompany == eCompany.AseK26) || (CData.CurCompany == eCompany.AseKr)) //190616 ksg :
            if (CData.CurCompany == ECompany.Qorvo   || CData.CurCompany == ECompany.Qorvo_DZ || CData.CurCompany == ECompany.SkyWorks || 
                CData.CurCompany == ECompany.ASE_K12 || CData.CurCompany == ECompany.ASE_KR   ||
                CData.CurCompany == ECompany.SST     || CData.CurCompany == ECompany.USI      ||
                //(CData.CurCompany == ECompany.SCK   )) //191202 ksg :
                CData.CurCompany == ECompany.SCK     || CData.CurCompany == ECompany.JSCK  || // 2020.10.21 JSKim 
                CData.CurCompany == ECompany.JCET)                                            // 2021.01.14 lhs : JCET 추가
            {
                pnlPr_InrPort.Visible = true; //190225 ksg :
            }
            else
            {
                pnlPr_InrPort.Visible = false; //190225 ksg :
            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            bool bConn = CPrb.It.Chk_Open(EWay.L);
            bool bConn2 = CPrb.It.Chk_Open(EWay.R);
            bool bConn3 = CPrb.It.Chk_Open(EWay.INR);

            cmb_LPort.Enabled = !bConn;
            cmb_LBaud.Enabled = !bConn;
            btn_LOpen.Enabled = !bConn;
            btn_LClose.Enabled = bConn;
            btn_LRead.Enabled = bConn;

            cmb_RPort.Enabled = !bConn2;
            cmb_RBaud.Enabled = !bConn2;
            btn_ROpen.Enabled = !bConn2;
            btn_RClose.Enabled = bConn2;
            btn_RRead.Enabled = bConn2;

            cmb_InPort.Enabled = !bConn3;
            cmb_InBaud.Enabled = !bConn3;
            btn_InOpen.Enabled = !bConn3;
            btn_InClose.Enabled = bConn3;
            btn_InRead.Enabled = bConn3;

            ckb_LDn.Checked = CIO.It.Get_Y(eY.GRDL_ProbeDn);
            ckb_LAir.Checked = CIO.It.Get_Y(eY.GRDL_ProbeAir);
            lbl_LAmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_ProbeAMP]];
            ckb_RDn.Checked = CIO.It.Get_Y(eY.GRDR_ProbeDn);
            ckb_RAir.Checked = CIO.It.Get_Y(eY.GRDR_ProbeAir);
            lbl_RAmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_ProbeAMP]];
            ckbPr_InDn.Checked = CIO.It.Get_Y(eY.INR_ProbeDn);
            ckb_InAir.Checked = CIO.It.Get_Y(eY.INR_ProbeAir);

            //200811 myk : Probe Calibration Check
            if (CData.L_GRD.bCalCheck)
            {
                txtU_LCalCheckCount.Text = CData.L_GRD.m_nCalCheckCount.ToString();
                txtPr_LLog.Text = CData.L_GRD.sProbe;
                txtPr_LLog.ScrollToCaret();
            }

            if (CData.R_GRD.bCalCheck)
            {
                txtU_RCalCheckCount.Text = CData.R_GRD.m_nCalCheckCount.ToString();
                txtPr_RLog.Text = CData.R_GRD.sProbe;
                txtPr_RLog.ScrollToCaret();
            }

            // lhs
            lbl_X.Text = "LX=" + CMot.It.Get_FP((int)EAx.LeftGrindZone_X).ToString() + ",  RX=" + CMot.It.Get_FP((int)EAx.RightGrindZone_X).ToString();   
            lbl_Y.Text = "LY=" + CMot.It.Get_FP((int)EAx.LeftGrindZone_Y).ToString() + ",  RY=" + CMot.It.Get_FP((int)EAx.RightGrindZone_Y).ToString();
            lbl_Z.Text = "LZ=" + CMot.It.Get_FP((int)EAx.LeftGrindZone_Z).ToString() + ",  RZ=" + CMot.It.Get_FP((int)EAx.RightGrindZone_Z).ToString();
            
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
            CPrb.It.Load();

            cmb_LPort.SelectedItem = CData.Prbs[(int)EWay.L].sPort;
            cmb_LBaud.SelectedItem = CData.Prbs[(int)EWay.L].iBaud.ToString();
            //syc : Probe Auto Calibration
            txt_LX.Text = CData.Prbs[(int)EWay.L].dX.ToString();
            txt_LY.Text = CData.Prbs[(int)EWay.L].dY.ToString();

            cmb_RPort.SelectedItem = CData.Prbs[(int)EWay.R].sPort;
            cmb_RBaud.SelectedItem = CData.Prbs[(int)EWay.R].iBaud.ToString();
            //syc : Probe Auto Calibration
            txt_RX.Text = CData.Prbs[(int)EWay.R].dX.ToString();
            txt_RY.Text = CData.Prbs[(int)EWay.R].dY.ToString();

            //20190220 ghk_dynamicfunction
            cmb_InPort.SelectedItem = CData.Prbs[(int)EWay.INR].sPort;
            cmb_InBaud.SelectedItem = CData.Prbs[(int)EWay.INR].iBaud.ToString();

            //syc : Probe Auto Calibration
            if (CData.Lev == ELv.Master)
            {
                btn_LAuto.Visible = true;
                btn_LCheck.Visible = true;
                lb_LX.Visible = true;
                txt_LX.Visible = true;
                lb_LY.Visible = true;
                txt_LY.Visible = true;

                btn_RAuto.Visible = true;
                btn_RCheck.Visible = true;
                lb_RX.Visible = true;
                txt_RX.Visible = true;
                lb_RY.Visible = true;
                txt_RY.Visible = true;

                //syc : Probe Auto Calibration : 201116
                //btn_Save.Visible = true;
            }
            else
            {
                btn_LAuto.Visible = false;
                btn_LCheck.Visible = false;
                lb_LX.Visible = false;
                txt_LX.Visible = false;
                lb_LY.Visible = false;
                txt_LY.Visible = false;

                btn_RAuto.Visible = false;
                btn_RCheck.Visible = false;
                lb_RX.Visible = false;
                txt_RX.Visible = false;
                lb_RY.Visible = false;
                txt_RY.Visible = false;

                //syc : Probe Auto Calibration : 201116
                //btn_Save.Visible = false;
            }
        }

        /// <summary>
        /// 화면에 값을 데이터로 저장
        /// </summary>
        private void _Get()
        {
            // Left probe rs232 data
            CData.Prbs[(int)EWay.L].sPort = cmb_LPort.SelectedItem.ToString();
            int.TryParse(cmb_LBaud.Text, out CData.Prbs[(int)EWay.L].iBaud);
            double.TryParse(txt_LX.Text, out CData.Prbs[(int)EWay.L].dX); //syc : Probe Auto Calibration
            double.TryParse(txt_LY.Text, out CData.Prbs[(int)EWay.L].dY); 

            // Right probe rs232 data
            CData.Prbs[(int)EWay.R].sPort = cmb_RPort.SelectedItem.ToString();
            int.TryParse(cmb_RBaud.Text, out CData.Prbs[(int)EWay.R].iBaud);
            double.TryParse(txt_RX.Text, out CData.Prbs[(int)EWay.R].dX); //syc : Probe Auto Calibration
            double.TryParse(txt_RY.Text, out CData.Prbs[(int)EWay.R].dY);

            //20190220 ghk_dynamicfunction
            // Inrail probe rs232 data
            CData.Prbs[(int)EWay.INR].sPort = cmb_InPort.SelectedItem.ToString();
            int.TryParse(cmb_InBaud.Text, out CData.Prbs[(int)EWay.INR].iBaud);
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
        public void Open()
        {
            cmb_LPort.DataSource = SerialPort.GetPortNames();
            cmb_RPort.DataSource = SerialPort.GetPortNames();
            //20190220 ghk_dynamicfunction
            cmb_InPort.DataSource = SerialPort.GetPortNames();

            _Set();

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

        private void btnPr_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;

            _Get();

            iRet = CPrb.It.Save();
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "Probe config file save fail");
            }
            else
            {
                CMsg.Show(eMsg.Notice, "Notice", "Probe config file save success");
            }

            _Set();
        }

        private void btnPr_Open_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;
            Button mBtn = sender as Button;
            EWay eWy = (EWay)int.Parse(mBtn.Tag.ToString());
            _Set();

            iRet = CPrb.It.Open_Port(eWy);
            if (iRet != 0)
            {
                if (eWy == EWay.L)
                { CErr.Show(eErr.LEFT_GIRND_PROBE_RS232_PORT_OPEN_ERROR); }
                else if (eWy == EWay.R)
                { CErr.Show(eErr.RIGHT_GIRND_PROBE_RS232_PORT_OPEN_ERROR); }
                //20190221 ghk_dynamicfunction
                else
                { CErr.Show(eErr.INRAIL_PROBE_RS232_PORT_OPEN_ERROR); }
            }
            else
            { CMsg.Show(eMsg.Notice, "Notice", eWy.ToString() + " probe open success"); }
        }

        private void btnPr_Close_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;
            Button mBtn = sender as Button;
            EWay eWy = (EWay)int.Parse(mBtn.Tag.ToString());

            iRet = CPrb.It.Close_Port(eWy);
            if (iRet != 0)
            {
                if (eWy == EWay.L)
                { CErr.Show(eErr.LEFT_GIRND_PROBE_RS232_PORT_CLOSE_ERROR); }
                else if (eWy == EWay.R)
                { CErr.Show(eErr.RIGHT_GIRND_PROBE_RS232_PORT_CLOSE_ERROR); }
                //20190221 ghk_dynamicfunction
                else
                { CErr.Show(eErr.INRAIL_PROBE_RS232_PORT_CLOSE_ERROR); }
            }
            else
            { CMsg.Show(eMsg.Notice, "Notice", eWy.ToString() + " probe close success"); }
        }

        private void btnPr_LRead_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sRet = CPrb.It.Write(EWay.L, ">R01");

            //200812 myk : Probe Calibration Check
            CData.L_GRD.bCalCheck = false;

            txtPr_LLog.AppendText(sRet);

            txtPr_LLog.SelectionStart = txtPr_LLog.Text.Length;
            txtPr_LLog.ScrollToCaret();
            
        }

        private void btnPr_RRead_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sRet = CPrb.It.Write(EWay.R, ">R01");

            txtPr_RLog.AppendText(sRet);

            //200812 myk : Probe Calibration Check
            CData.R_GRD.bCalCheck = false;

            txtPr_RLog.SelectionStart = txtPr_RLog.Text.Length;
            txtPr_RLog.ScrollToCaret();
            
        }

        private void btnPr_InrRead_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string[] sTemp;
            string   sRet;
            string   sResult;

            if(CDataOption.eDfType == eDfProbe.Keyence)
            {
                sRet = CPrb.It.Write(EWay.INR, "SR,00,000\r\n");
            
                sTemp = sRet.Split(',');
                if (sTemp.Length > 3)
                {
                    txtPr_InrLog.AppendText(sTemp[3]);
                }
                txtPr_InrLog.SelectionStart = txtPr_InrLog.Text.Length;
                txtPr_InrLog.ScrollToCaret();
            }
            else
            {
                sRet = CPrb.It.Write(EWay.INR, ">R01");
                sResult = sRet.Remove(0, 5);
                if (sResult != "")
                {
                    txtPr_InrLog.AppendText(sResult);
                }
                txtPr_InrLog.SelectionStart = txtPr_InrLog.Text.Length;
                txtPr_InrLog.ScrollToCaret();
            }
        }

        private void ckb_Click(object sender, EventArgs e)
        {            
            CData.bLastClick = true; //190509 ksg :
            CheckBox mCkb = sender as CheckBox;
            eY eOut = (eY)int.Parse(mCkb.Tag.ToString());

            _SetLog(eOut.ToString() + " Click");

            CIO.It.Set_Y(eOut, mCkb.Checked);
        }

        private void btn_LRepeat_Click(object sender, EventArgs e)
        {
            Stopwatch mSW = new Stopwatch();
            Stopwatch mTOut = new Stopwatch();
            int iTOut = 30000;
            bool bTOut = false;
            long[] aTime = new long[10];

            // 프로브 업
            CIO.It.Set_Y(eY.GRDL_ProbeDn, false);

            // 10번 반복 
            for (int i = 1; i <= 10; i++)
            {
                // 프로브 다운
                CIO.It.Set_Y(eY.GRDL_ProbeDn, true);

                // 스탑워치 시작
                mSW.Start();
                mTOut.Start();
                while (true)
                {
                    double dVal = CPrb.It.Read_Val(EWay.L);
                    // 프로브가 최종 다운 됫을때 값이 0이나 1임 (캘리브레이션 진행 안했을땐 0), 프로브 엠프 꺼짐
                    if ((dVal == 0 || dVal == 1) && !CIO.It.Get_X(eX.GRDL_ProbeAMP))
                    {
                        mSW.Stop();
                        break;
                    }

                    // 프로브 다운 타임아웃 체크
                    if (mTOut.ElapsedMilliseconds > iTOut)
                    {
                        mTOut.Stop();
                        bTOut = true;
                        break;
                    }
                }

                // 타임아웃 발생 시 로그 표시 이후 반복 탈출
                if (bTOut)
                {
                    txtPr_LLog.AppendText(string.Format("Repeat:{0},  Probe down time out error\r\n", i));
                    break;
                }
                else
                {
                    aTime[i] = mSW.ElapsedMilliseconds;
                    txtPr_LLog.AppendText(string.Format("Repeat:{0},  Time:{1}ms\r\n", i, aTime[i]));
                }

                // 스탑워치 리셋
                mTOut.Reset();
                mTOut.Start();
                mSW.Reset();

                // 프로브 업
                CIO.It.Set_Y(eY.GRDL_ProbeDn, false);
                while (true)
                {
                    // 프로브 업 센서 확인
                    if (CIO.It.Get_X(eX.GRDL_ProbeAMP))
                    { break; }

                    // 프로브 업 타임아웃 체크
                    if (mTOut.ElapsedMilliseconds > iTOut)
                    {
                        mTOut.Stop();
                        bTOut = true;
                        break;
                    }
                }

                mTOut.Reset();

                if (bTOut)
                {
                    txtPr_LLog.AppendText(string.Format("Repeat:{0},  Probe up time out error\r\n", i));
                    break;
                }
            }

            // 최종 데이터 출력
            if (bTOut)
            {
                txtPr_LLog.AppendText("Time out error !!!\r\n");
            }
            else
            {
                txtPr_LLog.AppendText(string.Format("Max:{0\r\n", aTime.Max()));
                txtPr_LLog.AppendText(string.Format("Min:{0\r\n", aTime.Min()));
                txtPr_LLog.AppendText(string.Format("Avg:{0\r\n", aTime.Average()));
            }
        }

        private void btn_RRepeat_Click(object sender, EventArgs e)
        {
            Stopwatch mSW = new Stopwatch();
            Stopwatch mTOut = new Stopwatch();
            int iTOut = 30000;
            bool bTOut = false;
            long[] aTime = new long[10];

            // 프로브 업
            CIO.It.Set_Y(eY.GRDR_ProbeDn, false);

            // 10번 반복 
            for (int i = 1; i <= 10; i++)
            {
                // 프로브 다운
                CIO.It.Set_Y(eY.GRDR_ProbeDn, true);

                // 스탑워치 시작
                mSW.Start();
                mTOut.Start();
                while (true)
                {
                    double dVal = CPrb.It.Read_Val(EWay.R);
                    // 프로브가 최종 다운 됫을때 값이 0이나 1임 (캘리브레이션 진행 안했을땐 0), 프로브 엠프 꺼짐
                    if ((dVal == 0 || dVal == 1) && !CIO.It.Get_X(eX.GRDR_ProbeAMP))
                    {
                        mSW.Stop();
                        break;
                    }

                    // 프로브 다운 타임아웃 체크
                    if (mTOut.ElapsedMilliseconds > iTOut)
                    {
                        mTOut.Stop();
                        bTOut = true;
                        break;
                    }
                }

                // 타임아웃 발생 시 로그 표시 이후 반복 탈출
                if (bTOut)
                {
                    txtPr_RLog.AppendText(string.Format("Repeat:{0},  Probe down time out error\r\n", i));
                    break;
                }
                else
                {
                    aTime[i] = mSW.ElapsedMilliseconds;
                    txtPr_RLog.AppendText(string.Format("Repeat:{0},  Time:{1}ms\r\n", i, aTime[i]));
                }

                // 스탑워치 리셋
                mTOut.Reset();
                mTOut.Start();
                mSW.Reset();

                // 프로브 업
                CIO.It.Set_Y(eY.GRDR_ProbeDn, false);
                while (true)
                {
                    // 프로브 업 센서 확인
                    if (CIO.It.Get_X(eX.GRDR_ProbeAMP))
                    { break; }

                    // 프로브 업 타임아웃 체크
                    if (mTOut.ElapsedMilliseconds > iTOut)
                    {
                        mTOut.Stop();
                        bTOut = true;
                        break;
                    }
                }

                mTOut.Reset();

                if (bTOut)
                {
                    txtPr_RLog.AppendText(string.Format("Repeat:{0},  Probe up time out error\r\n", i));
                    break;
                }
            }

            // 최종 데이터 출력
            if (bTOut)
            {
                txtPr_RLog.AppendText("Time out error !!!\r\n");
            }
            else
            {
                txtPr_RLog.AppendText(string.Format("Max:{0\r\n", aTime.Max()));
                txtPr_RLog.AppendText(string.Format("Min:{0\r\n", aTime.Min()));
                txtPr_RLog.AppendText(string.Format("Avg:{0\r\n", aTime.Average()));
            }   
        }

        //200812 myk : Probe Calibration Check
        private void btnPr_LCheck_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true;
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);

            if (ESeq == ESeq.GRL_Probe_Calibration_Check)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Have you removed the wheel? ") == DialogResult.Cancel)
                    return;
            }

            CSQ_Man.It.Seq = ESeq;
        }

        private void btnPr_RCheck_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true;
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);

            if (ESeq == ESeq.GRR_Probe_Calibration_Check)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Have you removed the wheel? ") == DialogResult.Cancel)
                    return;
            }

            CSQ_Man.It.Seq = ESeq;
        }

        // 200820 myk : Probe Auto Calibration
        private void btnPr_LAuto_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true;
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);

            if (ESeq == ESeq.GRL_Probe_Auto_Calibration)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Have you removed the wheel? ") == DialogResult.Cancel)
                    return;
            }

            CSQ_Man.It.Seq = ESeq;
        }

        private void btnPr_RAuto_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true;
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);

            if (ESeq == ESeq.GRR_Probe_Auto_Calibration)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Have you removed the wheel? ") == DialogResult.Cancel)
                    return;
            }

            CSQ_Man.It.Seq = ESeq;
        }

        private int m_tVacOnTime = 0;
        private int m_tAfterOffDelay = 0;
        private int m_tVacOffDelay = 0;
        private int m_tUpErrRetryTimeout = 0;
        private int m_nPrbTestCount = 10;
        private bool m_bPrbTestStop = false;
        private bool m_bPrbTestFinish = true;
        private int m_bPrbTestFunc = 0;

        private double m_fZaxis_mv_add_val = 0.0;

        System.Threading.Thread PrbTestThread = null;

        private delegate void SafeCallDelegate(string text);

        private void _SetLogProbe(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.PRB_L, string.Format("{0}.cs {1}() {2} Lv:{3}", sCls, sMth, sMsg, CData.Lev));
        }

        private void _addPrbTestMessage(string msg)
        {
            if (txtPr_LLog.InvokeRequired)
            {
                var d = new SafeCallDelegate(_addPrbTestMessage);
                Invoke(d, new object[] { msg });
            }
            else
            {
                if (txtPr_LLog.TextLength > (txtPr_LLog.MaxLength - 1000))
                {
                    txtPr_LLog.Clear();
                }

                txtPr_LLog.AppendText(msg + "\r\n");
                txtPr_LLog.ScrollToCaret(); //커서 맨 끝으로
                //txtPr_LLog.Invalidate();
                //txtPr_LLog.Update();
            }

            _SetLogProbe(msg); //File Log : OPL
        }

        private int _set_out(eY eOut, bool bAct, int tms)
        {
#if false //200731 jhc : Local PC에서 테스트 용으로만
            System.Threading.Thread.Sleep(1);
            return 0;
#endif

            CTim tmOut  = new CTim();

            tmOut.Set_Delay(tms);
            while (!CIO.It.Set_Y(eOut, bAct))
            {
                if (tmOut.Chk_Delay())
                {
                    return (-1); //Timeout
                }
                System.Threading.Thread.Sleep(1);
            }

            if (eOut == eY.GRDL_ProbeDn)
            {
                if (bAct == true) //프로브 DOWN => 프로브 앰프 신호 체크
                {
                    while (CIO.It.Get_X(eX.GRDL_ProbeAMP))
                    {
                        if (tmOut.Chk_Delay())
                        {
                            return (-1); //Timeout
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }
                else //프로브 UP => 프로브 앰프 신호 && 높이 체크
                {
                    while (!CIO.It.Get_X(eX.GRDL_ProbeAMP) || CPrb.It.Read_Val(EWay.L) < 17.0)
                    {
                        if (tmOut.Chk_Delay())
                        {
                            return (-1); //Timeout
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }

            return (0);
        }

        private void btn_LPrbT1_Click(object sender, EventArgs e)
        {
            if (this.m_bPrbTestFinish)
            {
                this.m_bPrbTestFinish = false;  //Thread 종료 확인 플래그 리셋
            }
            else
            {
                return; //테스트 진행 중이면 버튼 클릭 무효
            }

            //테스트 종류 파악
            Button mBtn = sender as Button;
            int.TryParse(mBtn.Tag.ToString(), out this.m_bPrbTestFunc);

            if (this.m_bPrbTestFunc != 1 && this.m_bPrbTestFunc != 2)
            {
                _addPrbTestMessage("Invalid Test Func : Check Button Tag!");
                this.m_bPrbTestFinish = true; //Thread 종료 확인 플래그 셋
                return;
            }


            //쓰레드 시작
            PrbTestThread = new System.Threading.Thread(new System.Threading.ThreadStart(_LPrbTProc));
            PrbTestThread.Start();
        }

        private void btn_LPrbTestStop_Click(object sender, EventArgs e)
        {
            this.m_bPrbTestStop = true; //Thread 종료 플래그 셋
            System.Threading.Thread.Sleep(500);

            CData.L_GRD.Func_PrbAir(false);
            CData.L_GRD.Func_PrbDown(false);
            CMot.It.Mv_H((int)EAx.LeftGrindZone_Z); //home

            while (!this.m_bPrbTestFinish)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                //201118 jhc : DEBUG //this.m_bPrbTestFinish = true;
            }
            
            if (PrbTestThread != null)
            { PrbTestThread.Join(); } //Thread 종료 대기

            //201118 jhc : 더블 체크
            this.m_bPrbTestFinish = true; //Thread 종료 확인 플래그 셋
        }

        private void _LPrbTProc()
        {
            bool bTmOut = false;
            int  nFlag  = 0;
            DateTime tStart;
            TimeSpan tTackTime;

            int.TryParse(txtS_VacOnTime.Text, out this.m_tVacOnTime);
            int.TryParse(txtS_AfterOffDelay.Text, out this.m_tAfterOffDelay);
            int.TryParse(txtS_VacOffDelay.Text, out this.m_tVacOffDelay);
            int.TryParse(txtS_UpErrRetryTmout.Text, out this.m_tUpErrRetryTimeout);
            int.TryParse(txtS_PrbTestCount.Text, out this.m_nPrbTestCount);

            this.m_bPrbTestStop = false;

            if (this.m_bPrbTestFunc == 1) { _addPrbTestMessage("Probe UP Test Start ######## (No Vacuum)"); }
            else { _addPrbTestMessage("Probe UP Test Start ######## (with Vacuum)"); }

            // 프로브 업, 체크
            if (0 != _set_out(eY.GRDL_ProbeDn, true, 5000))
            {
                _addPrbTestMessage("ERROR : Probe UP Timeout");
                this.m_bPrbTestFinish = true; //Thread 종료 확인 플래그 셋
                return;
            }

            Stopwatch mSW = new Stopwatch();

            // 반복 Loop
            int nCnt = 0;
            while ((nCnt++ < m_nPrbTestCount) && (!this.m_bPrbTestStop))
            {
                nFlag = 0; //장애 플래그 초기화

                // 프로브 Air ON, 체크
                if (0 != _set_out(eY.GRDL_ProbeAir, true, 1000)) { bTmOut = true; nFlag = 1; break; }

                // 프로브 DOWN, 체크
                if (0 != _set_out(eY.GRDL_ProbeDn, true, 3000)) { bTmOut = true; nFlag = 2; break; }

                if (this.m_bPrbTestFunc == 2)
                {
                    // 프로브 Vac ON, 체크
                    if (0 != _set_out(eY.Y3A, true, 1000)) { bTmOut = true; nFlag = 3; break; }
                }

                // 프로브 Air OFF, 체크
                if (0 != _set_out(eY.GRDL_ProbeAir, false, 1000)) { bTmOut = true; nFlag = 4; break; }

                if (this.m_bPrbTestFunc == 2)
                {
                    // 프로브 Vac ON 후 대기
                    System.Threading.Thread.Sleep(this.m_tVacOnTime); //T2 Vacuum ON Time
                }
                else
                {
                    // 프로브 Air OFF After Delay
                    System.Threading.Thread.Sleep(this.m_tAfterOffDelay); //T1 UP Delay
                }

                // 프로브 높이 기록
                _addPrbTestMessage("Probe Down Height : " + (Math.Round(CPrb.It.Read_Val(EWay.L), 4)).ToString());

                /////////////////////////////////////////////////////////

                // 시작 시간 기록
                tStart = DateTime.Now;

                // 프로브 UP, 체크
                if (0 != _set_out(eY.GRDL_ProbeDn, false, 5000))
                {
                    if (this.m_bPrbTestFunc == 1)
                    {
                        ///////////////////////////////////////////////////////
                        // 기존 방식으로 UP 장애 발생 시 Vacuum ON 하여 업 시도 //

                        _addPrbTestMessage("Probe UP Time(" + nCnt.ToString() + ") : ERROR Timeout => Retry with Vacuum ON");

                        // 프로브 Vac ON, 체크
                        if (0 != _set_out(eY.Y3A, true, 1000)) { bTmOut = true; nFlag = 3; break; }
                        // 프로브 UP, 체크
                        if (0 != _set_out(eY.GRDL_ProbeDn, false, this.m_tUpErrRetryTimeout)) { bTmOut = true; nFlag = 5; break; }

                        // 종료 시간 기록
                        tTackTime = DateTime.Now - tStart;
                        _addPrbTestMessage("Probe UP Time(" + nCnt.ToString() + ") : " + tTackTime.ToString());

                        // 프로브 Air OFF After Delay
                        System.Threading.Thread.Sleep(this.m_tVacOffDelay);

                        // 프로브 Vac OFF, 체크
                        if (0 != _set_out(eY.Y3A, false, 1000)) { bTmOut = true; nFlag = 6; break; }

                        // 프로브 높이 기록
                        _addPrbTestMessage("Probe Height : " + (Math.Round(CPrb.It.Read_Val(EWay.L), 4)).ToString());

                        continue; //잘 되면 계속 테스트
                    }
                    else
                    {
                        bTmOut = true;
                        nFlag = 5;
                        break;
                    }
                }

                while (!CIO.It.Get_X(eX.GRDL_ProbeAMP))
                {
                    Application.DoEvents();
                }

                // 종료 시간 기록
                tTackTime = DateTime.Now - tStart;
                _addPrbTestMessage("Probe UP Time(" + nCnt.ToString() + ") : " + tTackTime.ToString());

                // 프로브 Air OFF After Delay
                System.Threading.Thread.Sleep(this.m_tVacOffDelay);

                if (this.m_bPrbTestFunc == 2)
                {
                    // 프로브 Vac OFF, 체크
                    if (0 != _set_out(eY.Y3A, false, 1000)) { bTmOut = true; nFlag = 6; break; }
                }

                // 프로브 높이 기록
                _addPrbTestMessage("Probe Height : " + (Math.Round(CPrb.It.Read_Val(EWay.L), 4)).ToString());
            }

            if (bTmOut)
            {
                switch (nFlag)
                {
                    case 1: { _addPrbTestMessage("ERROR: Probe Air ON Timeout!!!"); break; }
                    case 2: { _addPrbTestMessage("ERROR: Probe DOWN Timeout!!!"); break; }
                    case 3: { _addPrbTestMessage("ERROR: Probe Vac ON Timeout!!!"); break; }
                    case 4: { _addPrbTestMessage("ERROR: Probe Air OFF Timeout!!!"); break; }

                    case 5:
                        {
                            _addPrbTestMessage("ERROR: Probe UP Timeout!!!");
                            // 프로브 Vac OFF, 체크
                            _set_out(eY.Y3A, false, 1000);
                            break;
                        }

                    case 6: { _addPrbTestMessage("ERROR: Probe Vac OFF Timeout!!!"); break; }
                    default: { _addPrbTestMessage("No Message"); break; }
                }
            }

            // 프로브 Vac OFF
            _set_out(eY.Y3A, false, 1000);
            // 프로브 Air OFF
            _set_out(eY.GRDL_ProbeAir, false, 1000);

            _addPrbTestMessage("Probe UP Test Finish...........................");

            this.m_bPrbTestFinish = true; //Thread 종료 확인 플래그 셋
        }
        
        private int m_tVacOnTimeR = 0;
        private int m_tAfterOffDelayR = 0;
        private int m_tVacOffDelayR = 0;
        private int m_tUpErrRetryTimeoutR = 0;
        private int m_nPrbTestCountR = 10;
        private bool m_bPrbTestStopR = false;
        private bool m_bPrbTestFinishR = true;
        private int m_bPrbTestFuncR = 0;

        System.Threading.Thread PrbTestThreadR = null;

        //private delegate void SafeCallDelegate(string text);

        private void _SetLogProbeR(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.PRB_R, string.Format("{0}.cs {1}() {2} Lv:{3}", sCls, sMth, sMsg, CData.Lev));
        }

        private void _addPrbTestMessageR(string msg)
        {
            if (txtPr_RLog.InvokeRequired)
            {
                var d = new SafeCallDelegate(_addPrbTestMessageR);
                Invoke(d, new object[] { msg });
            }
            else
            {
                if (txtPr_RLog.TextLength > (txtPr_RLog.MaxLength - 1000))
                {
                    txtPr_RLog.Clear();
                }

                txtPr_RLog.AppendText(msg + "\r\n");
                txtPr_RLog.ScrollToCaret(); //커서 맨 끝으로
                //txtPr_LLog.Invalidate();
                //txtPr_LLog.Update();
            }

            _SetLogProbeR(msg); //File Log : OPL
        }

        private int _set_outR(eY eOut, bool bAct, int tms)
        {
#if false //200731 jhc : Local PC에서 테스트 용으로만
            System.Threading.Thread.Sleep(1);
            return 0;
#endif

            CTim tmOut  = new CTim();

            tmOut.Set_Delay(tms);
            while (!CIO.It.Set_Y(eOut, bAct))
            {
                if (tmOut.Chk_Delay())
                {
                    return (-1); //Timeout
                }
                System.Threading.Thread.Sleep(1);
            }

            if (eOut == eY.GRDR_ProbeDn)
            {
                if (bAct == true) //프로브 DOWN => 프로브 앰프 신호 체크
                {
                    while (CIO.It.Get_X(eX.GRDR_ProbeAMP))
                    {
                        if (tmOut.Chk_Delay())
                        {
                            return (-1); //Timeout
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }
                else //프로브 UP => 프로브 앰프 신호 && 높이 체크
                {
                    while (!CIO.It.Get_X(eX.GRDR_ProbeAMP) || CPrb.It.Read_Val(EWay.R) < 17.0)
                    {
                        if (tmOut.Chk_Delay())
                        {
                            return (-1); //Timeout
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }

            return (0);
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (this.m_bPrbTestFinishR)
            {
                this.m_bPrbTestFinishR = false;  //Thread 종료 확인 플래그 리셋
            }
            else
            {
                return; //테스트 진행 중이면 버튼 클릭 무효
            }

            //테스트 종류 파악
            Button mBtn = sender as Button;
            int.TryParse(mBtn.Tag.ToString(), out this.m_bPrbTestFuncR);

            if (this.m_bPrbTestFuncR != 1 && this.m_bPrbTestFuncR != 2)
            {
                _addPrbTestMessageR("Invalid Test Func : Check Button Tag!");
                this.m_bPrbTestFinishR = true; //Thread 종료 확인 플래그 셋
                return;
            }


            //쓰레드 시작
            PrbTestThreadR = new System.Threading.Thread(new System.Threading.ThreadStart(_RPrbTProc));
            PrbTestThreadR.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.m_bPrbTestStopR = true; //201118 jhc : DEBUG //this.m_bPrbTestStop = true; //Thread 종료 플래그 셋
            System.Threading.Thread.Sleep(500);

            CData.R_GRD.Func_PrbAir(false);
            CData.R_GRD.Func_PrbDown(false);
            CMot.It.Mv_H((int)EAx.RightGrindZone_Z); //home

            while (!this.m_bPrbTestFinishR)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }

            if (PrbTestThreadR != null)
            { PrbTestThreadR.Join(); } //Thread 종료 대기

            //201118 jhc : 더블 체크
            this.m_bPrbTestFinishR = true; //Thread 종료 확인 플래그 셋
        }

        private void _RPrbTProc()
        {
            bool bTmOut = false;
            int  nFlag  = 0;
            DateTime tStart;
            TimeSpan tTackTime;

            int.TryParse(txtS_VacOnTimeR.Text, out this.m_tVacOnTimeR);
            int.TryParse(txtS_AfterOffDelayR.Text, out this.m_tAfterOffDelayR);
            int.TryParse(txtS_VacOffDelayR.Text, out this.m_tVacOffDelayR);
            int.TryParse(txtS_UpErrRetryTmoutR.Text, out this.m_tUpErrRetryTimeoutR);
            int.TryParse(txtS_PrbTestCountR.Text, out this.m_nPrbTestCountR);

            this.m_bPrbTestStopR = false;

            if (this.m_bPrbTestFuncR == 1) { _addPrbTestMessageR("Probe UP Test Start ######## (No Vacuum)"); }
            else { _addPrbTestMessageR("Probe UP Test Start ######## (with Vacuum)"); }

            // 프로브 업, 체크
            if (0 != _set_out(eY.GRDR_ProbeDn, true, 5000))
            {
                _addPrbTestMessageR("ERROR : Probe UP Timeout");
                this.m_bPrbTestFinishR = true; //Thread 종료 확인 플래그 셋
                return;
            }

            Stopwatch mSW = new Stopwatch();

            // 반복 Loop
            int nCnt = 0;
            while ((nCnt++ < m_nPrbTestCountR) && (!this.m_bPrbTestStopR))
            {
                nFlag = 0; //장애 플래그 초기화

                // 프로브 Air ON, 체크
                if (0 != _set_outR(eY.GRDR_ProbeAir, true, 1000)) { bTmOut = true; nFlag = 1; break; }

                // 프로브 DOWN, 체크
                if (0 != _set_outR(eY.GRDR_ProbeDn, true, 3000)) { bTmOut = true; nFlag = 2; break; }

                if (this.m_bPrbTestFuncR == 2)
                {
                    // 프로브 Vac ON, 체크
                    if (0 != _set_outR(eY.Y3A, true, 1000)) { bTmOut = true; nFlag = 3; break; }
                }

                // 프로브 Air OFF, 체크
                if (0 != _set_outR(eY.GRDR_ProbeAir, false, 1000)) { bTmOut = true; nFlag = 4; break; }

                if (this.m_bPrbTestFuncR == 2)
                {
                    // 프로브 Vac ON 후 대기
                    System.Threading.Thread.Sleep(this.m_tVacOnTimeR); //T2 Vacuum ON Time
                }
                else
                {
                    // 프로브 Air OFF After Delay
                    System.Threading.Thread.Sleep(this.m_tAfterOffDelayR); //T1 UP Delay
                }

                // 프로브 높이 기록
                _addPrbTestMessageR("Probe Down Height : " + (Math.Round(CPrb.It.Read_Val(EWay.R), 4)).ToString());

                /////////////////////////////////////////////////////////

                // 시작 시간 기록
                tStart = DateTime.Now;

                // 프로브 UP, 체크
                if (0 != _set_out(eY.GRDR_ProbeDn, false, 5000))
                {
                    if (this.m_bPrbTestFuncR == 1)
                    {
                        ///////////////////////////////////////////////////////
                        // 기존 방식으로 UP 장애 발생 시 Vacuum ON 하여 업 시도 //

                        _addPrbTestMessageR("Probe UP Time(" + nCnt.ToString() + ") : ERROR Timeout => Retry with Vacuum ON");

                        // 프로브 Vac ON, 체크
                        if (0 != _set_outR(eY.Y52, true, 1000)) { bTmOut = true; nFlag = 3; break; }
                        // 프로브 UP, 체크
                        if (0 != _set_outR(eY.GRDR_ProbeDn, false, this.m_tUpErrRetryTimeoutR)) { bTmOut = true; nFlag = 5; break; }

                        // 종료 시간 기록
                        tTackTime = DateTime.Now - tStart;
                        _addPrbTestMessageR("Probe UP Time(" + nCnt.ToString() + ") : " + tTackTime.ToString());

                        // 프로브 Air OFF After Delay
                        System.Threading.Thread.Sleep(this.m_tVacOffDelayR);

                        // 프로브 Vac OFF, 체크
                        if (0 != _set_outR(eY.Y52, false, 1000)) { bTmOut = true; nFlag = 6; break; }

                        // 프로브 높이 기록
                        _addPrbTestMessageR("Probe Height : " + (Math.Round(CPrb.It.Read_Val(EWay.R), 4)).ToString());

                        continue; //잘 되면 계속 테스트
                    }
                    else
                    {
                        bTmOut = true;
                        nFlag = 5;
                        break;
                    }
                }

                while (!CIO.It.Get_X(eX.GRDR_ProbeAMP))
                {
                    Application.DoEvents();
                }

                // 종료 시간 기록
                tTackTime = DateTime.Now - tStart;
                _addPrbTestMessageR("Probe UP Time(" + nCnt.ToString() + ") : " + tTackTime.ToString());

                // 프로브 Air OFF After Delay
                System.Threading.Thread.Sleep(this.m_tVacOffDelayR);

                if (this.m_bPrbTestFuncR == 2)
                {
                    // 프로브 Vac OFF, 체크
                    if (0 != _set_outR(eY.Y52, false, 1000)) { bTmOut = true; nFlag = 6; break; }
                }

                // 프로브 높이 기록
                _addPrbTestMessageR("Probe Height : " + (Math.Round( CPrb.It.Read_Val(EWay.R), 4)).ToString());
            }

            if (bTmOut)
            {
                switch (nFlag)
                {
                    case 1: { _addPrbTestMessageR("ERROR: Probe Air ON Timeout!!!"); break; }
                    case 2: { _addPrbTestMessageR("ERROR: Probe DOWN Timeout!!!"); break; }
                    case 3: { _addPrbTestMessageR("ERROR: Probe Vac ON Timeout!!!"); break; }
                    case 4: { _addPrbTestMessageR("ERROR: Probe Air OFF Timeout!!!"); break; }

                    case 5:
                        {
                            _addPrbTestMessageR("ERROR: Probe UP Timeout!!!");
                            // 프로브 Vac OFF, 체크
                            _set_out(eY.Y52, false, 1000);
                            break;
                        }

                    case 6: { _addPrbTestMessageR("ERROR: Probe Vac OFF Timeout!!!"); break; }
                    default: { _addPrbTestMessageR("No Message"); break; }
                }
            }

            // 프로브 Vac OFF
            _set_outR(eY.Y52, false, 1000);
            // 프로브 Air OFF
            _set_outR(eY.GRDR_ProbeAir, false, 1000);

            _addPrbTestMessageR("Probe UP Test Finish...........................");

            this.m_bPrbTestFinishR = true; //Thread 종료 확인 플래그 셋
        }

        private static int nfinish_flag = 0;
        private static int eWay;
        private void btn_LPrbmT1_Click(object sender, EventArgs e)
        {

            if (this.m_bPrbTestFinish)
            {
                nfinish_flag = 0;
                this.m_bPrbTestFinish = false;  //Thread 종료 확인 플래그 리셋
                _addPrbTestMessage("this.m_bPrbTestFinish = false");
            }
            else
            {
                return; //테스트 진행 중이면 버튼 클릭 무효
            }
            eWay = (int)EWay.L;
            //테스트 종류 파악
            Button mBtn = sender as Button;
            int.TryParse(mBtn.Tag.ToString(), out this.m_bPrbTestFunc);

            if (this.m_bPrbTestFunc != 1 && this.m_bPrbTestFunc != 2)
            {
                PrbTestMessage("Invalid Test Func : Check Button Tag!");
                this.m_bPrbTestFinish = true; //Thread 종료 확인 플래그 셋
                return;
            }


            //쓰레드 시작
            PrbTestThread = new System.Threading.Thread(new System.Threading.ThreadStart(_mLPrbTProc));
            PrbTestThread.Start();
        }



        private void btn_RPrbmT1_Click(object sender, EventArgs e)
        {
            if (this.m_bPrbTestStopR)
            {
                nfinish_flag = 0;
                this.m_bPrbTestStopR = false;  //Thread 종료 확인 플래그 리셋
                _addPrbTestMessageR("this.m_bPrbTestStopR = false");
            }
            else
            {
                return; //테스트 진행 중이면 버튼 클릭 무효
            }

            eWay = (int)EWay.R;
            //테스트 종류 파악
            Button mBtn = sender as Button;
            int.TryParse(mBtn.Tag.ToString(), out this.m_bPrbTestFunc);

            if (this.m_bPrbTestFunc != 1 && this.m_bPrbTestFunc != 2)
            {
                PrbTestMessage("Invalid Test Func : Check Button Tag!");
                this.m_bPrbTestStopR = true; //Thread 종료 확인 플래그 셋
                return;
            }


            //쓰레드 시작
            PrbTestThread = new System.Threading.Thread(new System.Threading.ThreadStart(_mLPrbTProc));
            PrbTestThread.Start();
        }

        static int Old_nSeq_Cnt;

        private void PrbTestMessage(string sData)
        {
            if (eWay == 0)
            {
                _addPrbTestMessage(sData);

            
                }
            else
            {
                _addPrbTestMessageR(sData);
            }
        }

        private void _mLPrbTProc()
        {
            this.m_bPrbTestStop = false;

            if (this.m_bPrbTestFunc == 1) { PrbTestMessage("Probe UP Test Start ######## (No Vacuum)"); }
            else { PrbTestMessage("Probe UP Test Start ######## (with Vacuum)"); }

            // 프로브 업, 체크

            int m_iX;
            int m_iY;
            int m_iZ;

            eX m_eIn1;
            eY m_eOt1;
            eY m_eOt2;

            double ftemp_Pos_Y= 0.0;
            double ftemp_Pos_X = 0.0;
            double ftemp_Pos_Z = 0.0;
            double fcurr_Pos_Z = 0.0;
            double ftemp_Pos_Z_UP = 0.0;

            double fReadVal = 0.0;
            
            string sData = "Left";

            if (eWay == 0)
            {
                //CData.L_GRD.Func_PrbAir(false);
                CData.L_GRD.Func_PrbDown(false);

                if (0 != _set_out(eY.GRDL_ProbeDn, true, 5000))
                {
                    PrbTestMessage("ERROR : Left Probe UP Timeout");
                    this.m_bPrbTestFinish = true; //Thread 종료 확인 플래그 셋
                    return;
                }
                m_iX = (int)EAx.LeftGrindZone_X;
                m_iY = (int)EAx.LeftGrindZone_Y;
                m_iZ = (int)EAx.LeftGrindZone_Z;

                m_eIn1 = eX.GRDL_ProbeAMP;
                m_eOt1 = eY.GRDL_ProbeDn;
                m_eOt2 = eY.GRDL_ProbeAir;

                ftemp_Pos_Y = CData.SPos.dGRD_Y_TblInsp[(int)eWay];
                ftemp_Pos_X = CData.SPos.dGRD_X_Zero[(int)eWay];
                ftemp_Pos_Z = CData.MPos[(int)eWay].dZ_TBL_BASE - GV.EQP_TABLE_MIN_THICKNESS;
                //ftemp_Pos_Z_UP = ftemp_Pos_Z - 10.0;
                ftemp_Pos_Z_UP = ftemp_Pos_Z - 25.0;    //lhs

                int.TryParse(txtS_VacOnTime.Text, out this.m_tVacOnTime);
                int.TryParse(txtS_AfterOffDelay.Text, out this.m_tAfterOffDelay);
                int.TryParse(txtS_VacOffDelay.Text, out this.m_tVacOffDelay);
                int.TryParse(txtS_UpErrRetryTmout.Text, out this.m_tUpErrRetryTimeout);
                int.TryParse(txtS_PrbTestCount.Text, out this.m_nPrbTestCount);
                double.TryParse(L_Z_Pos.Text, out this.m_fZaxis_mv_add_val);
            }
            else
            {
               // CData.R_GRD.Func_PrbAir(false);
                CData.R_GRD.Func_PrbDown(false);

                if (0 != _set_out(eY.GRDR_ProbeDn, true, 5000))
                {
                    PrbTestMessage("ERROR : Right Probe UP Timeout");
                    this.m_bPrbTestStopR = true; //Thread 종료 확인 플래그 셋
                    return;
                }
                m_iX = (int)EAx.RightGrindZone_X;
                m_iY = (int)EAx.RightGrindZone_Y;
                m_iZ = (int)EAx.RightGrindZone_Z;

                m_eIn1 = eX.GRDR_ProbeAMP;
                m_eOt1 = eY.GRDR_ProbeDn;
                m_eOt2 = eY.GRDR_ProbeAir;

                ftemp_Pos_Y = CData.SPos.dGRD_Y_TblInsp[(int)eWay];
                ftemp_Pos_X = CData.SPos.dGRD_X_Zero[(int)eWay];
                ftemp_Pos_Z = CData.MPos[(int)eWay].dZ_TBL_BASE - GV.EQP_TABLE_MIN_THICKNESS;
                //ftemp_Pos_Z_UP = ftemp_Pos_Z - 10.0;
                ftemp_Pos_Z_UP = ftemp_Pos_Z - 25.0;    //lhs

                int.TryParse(txtS_VacOnTimeR.Text, out this.m_tVacOnTime);
                int.TryParse(txtS_AfterOffDelayR.Text, out this.m_tAfterOffDelay);
                int.TryParse(txtS_VacOffDelayR.Text, out this.m_tVacOffDelay);
                int.TryParse(txtS_UpErrRetryTmoutR.Text, out this.m_tUpErrRetryTimeout);
                int.TryParse(txtS_PrbTestCountR.Text, out this.m_nPrbTestCount);
                double.TryParse(R_Z_Pos.Text, out this.m_fZaxis_mv_add_val);

                sData = "Right";
            }

            Stopwatch mSW = new Stopwatch();
            // 반복 Loop
            int nCnt = 0;
            int nSeq_Cnt = 0;
            int Old_nSeq_Cnt = -1; //lhs
            int iRet_0 = 0;
            int iRet_1 = 0;

            //while ((nCnt++ < m_nPrbTestCount) && (!this.m_bPrbTestStop))
            while (nfinish_flag != 2)
            {
                if (Old_nSeq_Cnt != nSeq_Cnt) Trace.WriteLine("nSeq_Cnt = " +  nSeq_Cnt);
                Old_nSeq_Cnt = nSeq_Cnt;
                switch (nSeq_Cnt)
                {
                    case 0: //Motor Z Home

                        iRet_0 = CMot.It.Mv_H(m_iZ);
                        if (iRet_0 != 0)
                        {

                            CMsg.Show(eMsg.Error, "Error", sData + " Z-axis Home error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        m_CheckTime_0.Set_Delay(60 * 1000);
                        nSeq_Cnt++;
                        break;
                    case 1: //Motor Z Home check
                            // XY Home
                        if (m_CheckTime_0.Chk_Delay())
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + " Z-axis Home Time Out error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        else if (!CMot.It.Get_HD(m_iZ))
                            break;  //return;

                        iRet_0 = CMot.It.Mv_H(m_iX);
                        iRet_1 = CMot.It.Mv_H(m_iY);
                        if (iRet_0 != 0)
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + " X-axis Home error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        else if (iRet_1 != 0)
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + " Y-axis Home error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        m_CheckTime_0.Set_Delay(60 * 1000);

                        nSeq_Cnt++;
                        break;
                    case 2: //Motor XY Home check                        
                        if (m_CheckTime_0.Chk_Delay())
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + " X,Y-axis Home Time Out error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        else if (!CMot.It.Get_HD(m_iX) || !CMot.It.Get_HD(m_iY))
                        {
                            break;// return;
                        }
                        m_CheckTime_0.Set_Delay(1 * 1000);
                        nSeq_Cnt++;
                        break;
                    case 3: //Motor XY Move
                        if (m_CheckTime_0.Chk_Delay())
                            return;

                        ftemp_Pos_Y = CData.SPos.dGRD_Y_TblInsp[(int)eWay];
                        ftemp_Pos_X = CData.SPos.dGRD_X_Zero[(int)eWay];
                        ftemp_Pos_Z = CData.MPos[(int)eWay].dZ_TBL_BASE - GV.EQP_TABLE_MIN_THICKNESS + m_fZaxis_mv_add_val;

                        CMot.It.Mv_N(m_iY, ftemp_Pos_Y);
                        //CMot.It.Mv_N(m_iY, ftemp_Pos_1);  //lhs
                        CMot.It.Mv_N(m_iX, ftemp_Pos_X);  //lhs
                        m_CheckTime_0.Set_Delay(30 * 1000);
                        PrbTestMessage(sData + "Motor XY Move : Y = " + (ftemp_Pos_Y.ToString()) + ",X = " + (ftemp_Pos_X.ToString()));
                        nSeq_Cnt++;
                        break;
                    case 4: //Motor XY Move check
                        if (m_CheckTime_0.Chk_Delay())
                        {
                            if (!CMot.It.Get_Mv(m_iX, ftemp_Pos_X))
                            {
                                CMsg.Show(eMsg.Error, "Error", sData + " X-axis Move Time Out error");
                                this.m_bPrbTestFinish = true;
                                this.m_bPrbTestStopR = true;
                                nfinish_flag = 2;
                            }
                            else
                            {
                                CMsg.Show(eMsg.Error, "Error", sData + " Y-axis Move Time Out error");
                                this.m_bPrbTestFinish = true;
                                this.m_bPrbTestStopR = true;
                                nfinish_flag = 2;
                            }
                            return;
                        }
                        else if (CMot.It.Get_Mv(m_iY, ftemp_Pos_Y) && CMot.It.Get_Mv(m_iX, ftemp_Pos_X))
                        {
                            PrbTestMessage(sData + "Motor XY Move End: Y = " + (ftemp_Pos_Y.ToString()) + ",X = " + (ftemp_Pos_X.ToString()));
                            m_CheckTime_0.Set_Delay(1 * 1000);
                            nSeq_Cnt++; //lhs
                        }
                        // nSeq_Cnt++; //lhs
                        break;
                    case 5: // 프로브 다운, 에어온
                        if (eWay == 0)
                        {
                            CData.L_GRD.Func_PrbAir(true);
                            CData.L_GRD.Func_PrbDown(true);
                        }
                        else
                        {
                            CData.R_GRD.Func_PrbAir(true);
                            CData.R_GRD.Func_PrbDown(true);
                        }

                        m_CheckTime_0.Set_Delay(10 * 1000);
                        nSeq_Cnt++;
                        break;
                    case 6: //프로브 다운, 프로브 에어 온 확인
                        if (m_CheckTime_0.Chk_Delay())
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + " ProbeDown & ProbeAirOn Check Time Out error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        if (CIO.It.Get_Y(m_eOt1) && CIO.It.Get_Y(m_eOt2))
                        {
                            m_CheckTime_0.Set_Delay(2 * 1000);
                            m_CheckTime_1.Set_Delay(10 * 1000);
                            if (eWay == 0) { 
                                CData.L_GRD.Func_PrbAir(false); // Air off
                            }else
                            {
                                CData.R_GRD.Func_PrbAir(false); // Air off

                            }
                            nSeq_Cnt++;  //lhs
                        }
                        break;
                    case 7://프로브 에어 오프 확인
                        if (m_CheckTime_1.Chk_Delay())
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + "Probe Air Off Time Out error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        if (!CIO.It.Get_Y(m_eOt2))
                        {
                            nSeq_Cnt++;
                        }
                        break;
                    case 8: //Motor down
                        CMot.It.Mv_N(m_iZ, ftemp_Pos_Z);
                        m_CheckTime_0.Set_Delay(30 * 1000);

                        nSeq_Cnt++;
                        break;
                    case 9: //Motor down check
                        if (m_CheckTime_0.Chk_Delay())
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + " Z-axis Move Time Out error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        else if (!CMot.It.Get_Mv(m_iZ, ftemp_Pos_Z))
                        {
                            break;  //return;
                        }
                        m_CheckTime_0.Set_Delay(this.m_tAfterOffDelay);
                        nSeq_Cnt++;
                        break;
                    case 10: //Probe 값 읽음
                        if (!m_CheckTime_0.Chk_Delay())
                        {
                            //CMsg.Show(eMsg.Error, "Error", sData + " Probe Read Time Out error");
                            //this.m_bPrbTestFinish = true;
                            //nfinish_flag = 2;
                            //return;
                            break;
                        }
                        if (eWay == 0) { 
                            fReadVal = CPrb.It.Read_Val(EWay.L);
                        }else fReadVal = CPrb.It.Read_Val(EWay.R);
                        fcurr_Pos_Z = CMot.It.Get_FP(m_iZ);
                        PrbTestMessage(sData + ",Count : "+ nCnt + ",Probe Height : " + (Math.Round(fReadVal, 4)).ToString() + ",Z-axis Pos : " + fcurr_Pos_Z.ToString());

                        if (nCnt++ < (m_nPrbTestCount-1))
                        {
                            nSeq_Cnt++;

                            nfinish_flag = 0;
                            if (this.m_bPrbTestStop)
                            {
                                nfinish_flag = 1;
                            }
                        }
                        else
                        {// count 작업 종료... 
                            if (eWay == 0)
                            {
                                CData.L_GRD.Func_PrbAir(false);
                                CData.L_GRD.Func_PrbDown(false);
                            }
                            else
                            {
                                CData.R_GRD.Func_PrbAir(false);
                                CData.R_GRD.Func_PrbDown(false);
                            }

                            ftemp_Pos_Z_UP = 0.0;
                            nfinish_flag = 1;
                            nSeq_Cnt++;
                        }
                        break;
                    case 11: //Motor Up
                        //CMot.It.Mv_N(m_iZ, ftemp_Pos_Z);
                        CMot.It.Mv_N(m_iZ, ftemp_Pos_Z_UP);
                        m_CheckTime_0.Set_Delay(10 * 1000);
                        nSeq_Cnt++;
                        break;
                    case 12: //Motor Up check
                        if (m_CheckTime_0.Chk_Delay())
                        {
                            CMsg.Show(eMsg.Error, "Error", sData + "Z-axis Move Time Out error");
                            this.m_bPrbTestFinish = true;
                            this.m_bPrbTestStopR = true;
                            nfinish_flag = 2;
                            return;
                        }
                        //else if (!CMot.It.Get_Mv(m_iZ, ftemp_Pos_Z))
                        else if (!CMot.It.Get_Mv(m_iZ, ftemp_Pos_Z_UP))        // lhs
                        {
                            break;  //return;
                        }
                        else if (nfinish_flag == 1)
                        {                            
                            nSeq_Cnt = 21;                            
                        }
                        else
                        {
                            m_CheckTime_0.Set_Delay(2 * 1000);
                            m_CheckTime_1.Set_Delay(10 * 1000);
                            fcurr_Pos_Z = CMot.It.Get_FP(m_iZ);
                            //PrbTestMessage(sData + "Probe Z-axis UP." + ",Z-axis Pos : " + fcurr_Pos_Z.ToString());

                            nSeq_Cnt = 8;
                        }                        
                        break;

                    case 21: //Motor Up check
                        PrbTestMessage(sData + "Probe Z-axis Test Finish...........................");
                        this.m_bPrbTestStop = true;
                        nfinish_flag = 2;
                        
                        break;

                    default: break;
                }
                if (this.m_bPrbTestStop == true) nfinish_flag = 2;
                Application.DoEvents();
            }
           
        }

    }
}
