using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_03Spl : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        private CTim m_Timout_485 = new CTim(); //  2023.03.15 Max

        public vwEqu_03Spl()
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

            //20191022 ghk_spindle_type
            //if (CSpl.It.bUse232)
            //if (CDataOption.SplType == eSpindleType.Rs232)
            //{
            //    btnSpl_LSvoOn.Visible = false;
            //    btnSpl_LSvoOff.Visible = false;
            //    btnSpl_RSvoOn.Visible = false;
            //    btnSpl_RSvoOff.Visible = false;
            //}
            //else
            //{
            //    btnSpl_LSvoOn.Visible = true;
            //    btnSpl_LSvoOff.Visible = true;
            //    btnSpl_RSvoOn.Visible = true;
            //    btnSpl_RSvoOff.Visible = true;
            //}
            // 2023.03.15 Max
            btnSpl_LSvoOn.Visible = false;
            btnSpl_LSvoOff.Visible = false;
            btnSpl_RSvoOn.Visible = false;
            btnSpl_RSvoOff.Visible = false;
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                if (CData.Spls[0].aStat != null)
                { pnlSpl_Left.Controls["lblSpl_Lbit" + i].BackColor = GV.CR_X[Convert.ToInt32(CData.Spls[0].aStat[i])]; }
                else
                { pnlSpl_Left.Controls["lblSpl_Lbit" + i].BackColor = GV.CR_X[0]; }

                if (CData.Spls[1].aStat != null)
                { pnlSpl_Right.Controls["lblSpl_Rbit" + i].BackColor = GV.CR_X[Convert.ToInt32(CData.Spls[1].aStat[i])]; }
                else
                { pnlSpl_Right.Controls["lblSpl_Rbit" + i].BackColor = GV.CR_X[0]; }
            }

            //20191022 ghk_spindle_type
            //if (CSpl.It.bUse232)
            //if (CDataOption.SplType == eSpindleType.Rs232)
            //{
            //    lblSpl_LRpm.Text = CData.Spls[0].iRpm.ToString() + " rpm";
            //    lblSpl_LLoad.Text = CData.Spls[0].dLoad.ToString() + " %";
            //    lblSpl_RRpm.Text = CData.Spls[1].iRpm.ToString() + " rpm";
            //    lblSpl_RLoad.Text = CData.Spls[1].dLoad.ToString() + " %";

            //    btnSpl_LSvoOn.Visible = false;
            //    btnSpl_LSvoOff.Visible = false;
            //    btnSpl_RSvoOn.Visible = false;
            //    btnSpl_RSvoOff.Visible = false;
            //}
            //else
            //{
            //    lblSpl_LRpm.Text = CSpl.It.GetFrpm(true).ToString() + " rpm";
            //    lblSpl_LLoad.Text = CData.Spls[0].dLoad.ToString() + " %";
            //    lblSpl_RRpm.Text = CSpl.It.GetFrpm(false).ToString() + " rpm";
            //    lblSpl_RLoad.Text = CData.Spls[1].dLoad.ToString() + " %"; 
            //}
            // 2023.03.15 Max
            lblSpl_LRpm.Text = CData.Spls[0].iRpm.ToString() + " rpm";
            lblSpl_LLoad.Text = CData.Spls[0].dLoad.ToString() + " %";
            lblSpl_RRpm.Text = CData.Spls[1].iRpm.ToString() + " rpm";
            lblSpl_RLoad.Text = CData.Spls[1].dLoad.ToString() + " %";

            btnSpl_LSvoOn.Visible = false;
            btnSpl_LSvoOff.Visible = false;
            btnSpl_RSvoOn.Visible = false;
            btnSpl_RSvoOff.Visible = false;

            lblSpl_LFlow.BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_X(eX.GRDL_SplPCW))];
            lblSpl_RFlow.BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_X(eX.GRDR_SplPCW))];
            lblSpl_Temp.BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_X(eX.GRDL_SplPCWTemp))];
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
            ckbSpl_LMc.Checked = CIO.It.Get_Y(eY.GRDL_SplInverterMC);
            ckbSpl_LCool.Checked = CIO.It.Get_Y(eY.GRDL_SplPCW);
            ckbSpl_LCda.Checked = CIO.It.Get_Y(eY.GRDL_SplCDA);

            ckbSpl_RMc.Checked = CIO.It.Get_Y(eY.GRDR_SplInverterMC);
            ckbSpl_RCool.Checked = CIO.It.Get_Y(eY.GRDR_SplPCW);
            ckbSpl_RCda.Checked = CIO.It.Get_Y(eY.GRDR_SplCDA);

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

        private void btnSpl_Init_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            //CSpl.It.Write_Init();

            // 2023.03.15 Max
            bool bExit = false;
            bool bTimeOutFlag = false;            
            CSpl_485.It.Write_Stop(EWay.L);
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptStopSpindle(EWay.L);
            } while (bExit != true);
            CSpl_485.It.SetAcceptStopSpindle(EWay.L);
            //if (bTimeOutFlag)
            //{
            //    CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
            //    return;
            //}


            
            bExit = false;
            bTimeOutFlag = false;
            CSpl_485.It.Write_Reset(EWay.L);
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptResetSpindle(EWay.L);
            } while (bExit != true);
            CSpl_485.It.SetAcceptResetSpindle(EWay.L);
            //if (bTimeOutFlag)
            //{
            //    CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
            //    return;
            //}


            bExit = false;
            bTimeOutFlag = false;
            CSpl_485.It.Write_Stop(EWay.R);
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptStopSpindle(EWay.R);
            } while (bExit != true);
            CSpl_485.It.SetAcceptStopSpindle(EWay.R);
            //if (bTimeOutFlag)
            //{
            //    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
            //    return;            
            //}


            bExit = false;
            bTimeOutFlag = false;
            CSpl_485.It.Write_Reset(EWay.R);
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptResetSpindle(EWay.R);
            } while (bExit != true);
            CSpl_485.It.SetAcceptResetSpindle(EWay.R);
            //if (bTimeOutFlag)
            //{
            //    CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
            //    return;
            //}
        }

        private void btnSpl_Run_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            //int iRet = 0;
            int iRpm = 0;
            string sVal = "";
            Button mBtn = sender as Button;
            EWay eWy = (EWay)int.Parse(mBtn.Tag.ToString());

            if (eWy == EWay.L)
            {
                sVal = txtSpl_LRun.Text;
            }
            else
            {
                sVal = txtSpl_RRun.Text;
            }

            if (!int.TryParse(sVal, out iRpm))
            {

                if (eWy == EWay.L)
                {
                    CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
                    _SetLog("Error : Need Left spindle value check.");
                    return;
                }
                else
                {
                    CErr.Show(eErr.RIGHT_GRIND_INVALID_SET_VALUE);
                    _SetLog("Error : Need Right spindle value check.");
                    return;
                }
            }
            else
            {
/*
                if (eWy == EWay.L)
                {
                    // MC check
                    if (!CIO.It.Get_Y(eY.GRDL_SplInverterMC))
                    {
                        CErr.Show(eErr.LEFT_GRIND_INVALID_SET_VALUE);
                        _SetLog("Error : Need Left spindle MC check.");
                        return;
                    }
                    // PCW check
                    if (!CIO.It.Get_X(eX.GRDL_SplPCW))
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_COOLANT_OFF); 
                        return;
                    }
                    // CDA check
                    if (!CIO.It.Get_Y(eY.GRDL_SplCDA))
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_CDA_OFF); 
                        return;
                    }
                }
                else
                {
                    // MC check
                    if (!CIO.It.Get_Y(eY.GRDR_SplInverterMC))
                    {
                        CErr.Show(eErr.RIGHT_GRIND_INVALID_SET_VALUE);
                        _SetLog("Error : Need Right spindle MC check.");
                        return;
                    }
                    // PCW check
                    if (!CIO.It.Get_X(eX.GRDR_SplPCW))
                    {
                        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_COOLANT_OFF);
                        return;
                    }
                    // CDA check
                    if (!CIO.It.Get_Y(eY.GRDR_SplCDA))
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_CDA_OFF);
                        return;
                    }
                }
*/
                // Set RPM                
                //iRet = CSpl.It.Write_Rpm(eWy, iRpm);
                //if (iRet != 0)
                //{
                //    if (eWy == EWay.L)
                //    {
                //        CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
                //        return;
                //    }
                //    else
                //    {
                //        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
                //        return;
                //    }
                //}
                // 2023.03.15 Max
                CSpl_485.It.Write_Rpm(eWy, iRpm);
                bool bExit = false;
                bool bTimeOutFlag = false;
                m_Timout_485.Set_Delay(300);
                do
                {
                    Application.DoEvents();
                    if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                    bExit = CSpl_485.It.GetAcceptRPMSpindle(eWy);
                } while (bExit != true);
                CSpl_485.It.SetAcceptRPMSpindle(eWy);

                if (bTimeOutFlag)
                {
                    if (eWy == EWay.L)
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
                        return;
                    }
                    else
                    {
                        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
                        return;
                    }
                }




                // Run
                //iRet = CSpl.It.Write_Run(eWy, iRpm);
                //if (iRet != 0)
                //{
                //    if (eWy == EWay.L)
                //    {
                //        CErr.Show(eErr.LEFT_GRIND_SPINDLE_RUN_RPM_ERROR);
                //        return;
                //    }
                //    else
                //    {
                //        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_RUN_RPM_ERROR);
                //        return;
                //    }
                //}
                // Spindle RUN
                bExit = false;
                bTimeOutFlag = false;
                CSpl_485.It.Write_Run(eWy);
                m_Timout_485.Set_Delay(300);
                do
                {
                    Application.DoEvents();
                    if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                    bExit = CSpl_485.It.GetAcceptRunSpindle(eWy);
                } while (bExit != true);
                CSpl_485.It.SetAcceptRunSpindle(eWy);

                if (bTimeOutFlag)
                {
                    if (eWy == EWay.L)
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_RUN_RPM_ERROR);
                        return;
                    }
                    else
                    {
                        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_RUN_RPM_ERROR);
                        return;
                    }
                }
            }          
        }

        private void btnSpl_Stop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            //int iRet = 0;
            Button mBtn = sender as Button;
            EWay eWy = (EWay)int.Parse(mBtn.Tag.ToString());     

        // Run
        //iRet = CSpl.It.Write_Stop(eWy);
        //if (iRet != 0)
        //{
        //    if (eWy == EWay.L)
        //    {
        //        CErr.Show(eErr.LEFT_GRIND_SPINDLE_STOP_ERROR);
        //        return;
        //    }
        //    else
        //    {
        //        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_STOP_ERROR);
        //        return;
        //    }
        //}

            // 2023.03.15 Max
            CSpl_485.It.Write_Stop(eWy);
            bool bExit = false;
            bool bTimeOutFlag = false;
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay())  { bTimeOutFlag = true;  bExit = true; }
                bExit = CSpl_485.It.GetAcceptStopSpindle(eWy);                
            } while (bExit != true);
            CSpl_485.It.SetAcceptStopSpindle(eWy);

            if (bTimeOutFlag)
            {
                if (eWy == EWay.L)
                {
                    CErr.Show(eErr.LEFT_GRIND_SPINDLE_STOP_ERROR);
                    return;
                }
                else
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_STOP_ERROR);
                    return;
                }
            }
        }

        private void btnSpl_Reset_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            //int iRet = 0;
            Button mBtn = sender as Button;
            EWay eWy = (EWay)int.Parse(mBtn.Tag.ToString());

            // Run
            //iRet = CSpl.It.Write_Reset(eWy);
            //if (iRet != 0)
            //{
            //    if (eWy == EWay.L)
            //    {
            //        CErr.Show(eErr.LEFT_GRIND_SPINDLE_STOP_ERROR);
            //        return;
            //    }
            //    else
            //    {
            //        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_STOP_ERROR);
            //        return;
            //    }
            //}

            // 2023.03.15 Max
            CSpl_485.It.Write_Reset(eWy);
            bool bExit = false;
            bool bTimeOutFlag = false;
            m_Timout_485.Set_Delay(300);
            do
            {
                Application.DoEvents();
                if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                bExit = CSpl_485.It.GetAcceptResetSpindle(eWy);
            } while (bExit != true);
            CSpl_485.It.SetAcceptResetSpindle(eWy);

            if (bTimeOutFlag)
            {
                if (eWy == EWay.L)
                {
                    CErr.Show(eErr.LEFT_GRIND_SPINDLE_STOP_ERROR);
                    return;
                }
                else
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_STOP_ERROR);
                    return;
                }
            }

        }

        private void ckbSpl_Mc_CheckedChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            CheckBox mCkb = sender as CheckBox;
            EWay eWy = (EWay)int.Parse(mCkb.Tag.ToString());

            if (eWy == EWay.L)
            { CIO.It.Set_Y(eY.GRDL_SplInverterMC, mCkb.Checked); }
            else
            { CIO.It.Set_Y(eY.GRDR_SplInverterMC, mCkb.Checked); }
        }

        private void ckbSpl_Cool_CheckedChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            CheckBox mCkb = sender as CheckBox;
            EWay eWy = (EWay)int.Parse(mCkb.Tag.ToString());

            if (eWy == EWay.L)
            { CIO.It.Set_Y(eY.GRDL_SplPCW, mCkb.Checked); }
            else
            { CIO.It.Set_Y(eY.GRDR_SplPCW, mCkb.Checked); }
        }

        private void ckbSpl_Cda_CheckedChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            CheckBox mCkb = sender as CheckBox;
            EWay eWy = (EWay)int.Parse(mCkb.Tag.ToString());

            if (eWy == EWay.L)
            { CIO.It.Set_Y(eY.GRDL_SplCDA, mCkb.Checked); }
            else
            { CIO.It.Set_Y(eY.GRDR_SplCDA, mCkb.Checked); }
        }

        private void btnSpl_LSvoOn_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CMot.It.Set_SOn((int)EAx.LeftSpindle, true);            
        }

        private void btnSpl_LSvoOff_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CMot.It.Set_SOn((int)EAx.LeftSpindle, false);            
        }

        private void btnSpl_RSvoOn_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CMot.It.Set_SOn((int)EAx.RightSpindle, true);            
        }

        private void btnSpl_RSvoOff_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CMot.It.Set_SOn((int)EAx.RightSpindle, false);            
        }
    }
}
