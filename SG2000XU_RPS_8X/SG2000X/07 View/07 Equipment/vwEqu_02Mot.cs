using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_02Mot : UserControl
    {
        /// <summary>
        /// 현재 사용중인 축 번호
        /// </summary>
        private int m_iAx = -1;
        /// <summary>
        /// 스텝 이동 시 거리 값
        /// </summary>
        private double m_dStep = 0;
        /// <summary>
        /// Repeat stop flag    true:stop  false:run
        /// </summary>
        private bool m_bRep = false;
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwEqu_02Mot()
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

            cboM_Axis.Items.Clear();
            for (int i = CMot.It.AxS; i <= CMot.It.AxL; i++)
            {
                EAx Ax = (EAx)i;
                cboM_Axis.Items.Add(Ax.ToString());
            }

            rdbM_1000.Checked = true;
            m_dStep = 1;
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (m_iAx >= 0 && CData.WMX)
            {
                lblM_Stat_SOn.BackColor = GV.CR_X[CMot.It.Chk_SrvI(m_iAx)];
                lblM_Stat_Alarm.BackColor = GV.CR_Y[CMot.It.Chk_AlrI(m_iAx)];
                lblM_Stat_Home.BackColor = GV.CR_X[CMot.It.Chk_HomeI(m_iAx)];
                lblM_Stat_POT.BackColor = GV.CR_X[CMot.It.Chk_POTI(m_iAx)];
                lblM_Stat_NOT.BackColor = GV.CR_X[CMot.It.Chk_NOTI(m_iAx)];
                lblM_Stat_HD.BackColor = GV.CR_X[CMot.It.Chk_HDI(m_iAx)];
                lblM_Stat_Inpos.BackColor = GV.CR_X[CMot.It.Chk_InPI(m_iAx)];

                lblM_OP.Text = CMot.It.Chk_OP(m_iAx).ToString();
                lblM_EC.Text = CWmx.It.Stat.AmpAlarmCode[m_iAx].ToString();

                lblM_CP.Text = CMot.It.Get_CP(m_iAx).ToString();
                lblM_EP.Text = CMot.It.Get_FP(m_iAx).ToString();
                lblM_AV.Text = CMot.It.Get_Vel(m_iAx).ToString();
            }
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
            if (m_iAx < 0)
            { return; }

            double dPP1 = CData.Axes[m_iAx].dPP1;

            lblM_PI.Text = m_iAx.ToString("00");
            chkM_Use.Checked = CData.Axes[m_iAx].bUse;
            lbl_CM.Text = CWmx.It.Stat.AxisCommandMode[m_iAx].ToString();
            lbl_GRN.Text = CWmx.It.AParm.GearRatioNumerator[m_iAx].ToString();
            lbl_GRD.Text = CWmx.It.AParm.GearRatioDenominator[m_iAx].ToString();
            lbl_M.Text = CWmx.It.AParm.AxisMultiplier[m_iAx].ToString();
            lbl_P.Text = CWmx.It.AParm.AxisPolarity[m_iAx].ToString();
            txtM_IW.Text = (CWmx.It.SParm.FeedbackParam[m_iAx].InPosWidth / dPP1).ToString();
            lbl_BSP.Text = CData.Axes[m_iAx].dBSP.ToString();
            lbl_1P.Text = CData.Axes[m_iAx].dPP1.ToString();
            lbl_IPLS.Text = CWmx.It.SParm.LimitParam[m_iAx].InvertPositiveLSLogic.ToString();
            lbl_INLS.Text = CWmx.It.SParm.LimitParam[m_iAx].InvertNegativeLSLogic.ToString();
            txtM_QSD.Text = (CWmx.It.SParm.MotionParam[m_iAx].QuickStopDec / dPP1).ToString();

            // Run slow parameter
            txtM_RVS.Text = CData.Axes[m_iAx].tRS.iVel.ToString();
            txtM_RVSA.Text = CData.Axes[m_iAx].tRS.iAcc.ToString();
            txtM_RVSD.Text = CData.Axes[m_iAx].tRS.iDec.ToString();
            // Run fast parameter
            txtM_RVF.Text = CData.Axes[m_iAx].tRN.iVel.ToString();
            txtM_RVFA.Text = CData.Axes[m_iAx].tRN.iAcc.ToString();
            txtM_RVFD.Text = CData.Axes[m_iAx].tRN.iDec.ToString();
            // Jog parameter
            txtM_JV.Text = CData.Axes[m_iAx].tJN.iVel.ToString();
            txtM_JVA.Text = CData.Axes[m_iAx].tJN.iAcc.ToString();
            txtM_JVD.Text = CData.Axes[m_iAx].tJN.iDec.ToString();

            // Software limit
            txtM_LNP.Text = CData.Axes[m_iAx].dSWMin.ToString();
            txtM_LPP.Text = CData.Axes[m_iAx].dSWMax.ToString();

            txtM_AccR.Text = CData.Axes[m_iAx].iAccR.ToString();
            txtM_DecR.Text = CData.Axes[m_iAx].iDecR.ToString();

            // Check limit sensor
            chkM_NOT.Checked = CData.Axes[m_iAx].bNOT;
            chkM_POT.Checked = CData.Axes[m_iAx].bPOT;

            // Init home parameter
            cboM_HT.SelectedIndex = (int)CWmx.It.SParm.HomeParam[m_iAx].HomeType;
            cboM_HD.SelectedIndex = (int)CWmx.It.SParm.HomeParam[m_iAx].HomeDirection;
            cboM_HS.SelectedIndex = CWmx.It.SParm.HomeParam[m_iAx].HSLogic;
            txtM_HVS.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlow / dPP1).ToString();
            txtM_HVSA.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlowAcc / dPP1).ToString();
            txtM_HVSD.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlowDec / dPP1).ToString();
            txtM_HVF.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFast / dPP1).ToString();
            txtM_HVFA.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFastAcc / dPP1).ToString();
            txtM_HVFD.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFastDec / dPP1).ToString();
            txtM_HSDi.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomeShiftDistance / dPP1).ToString();
            txtM_HSV.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomeShiftVelocity / dPP1).ToString();
            txtM_HSA.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomeShiftAcc / dPP1).ToString();
            txtM_HSD.Text = (CWmx.It.SParm.HomeParam[m_iAx].HomeShiftDec / dPP1).ToString();
        }

        private void _GetWMX()
        {
            int dPP1 = (int)CData.Axes[m_iAx].dPP1;
            double dInP = 0;

            //Enum.TryParse(cboM_CM.SelectedText, out CWmx.It.Stat.AxisCommandMode[m_iAx]);
            CData.Axes[m_iAx].bUse = chkM_Use.Checked;
            //uint.TryParse(txtM_GRN.Text, out CWMX.Inst.AP.GearRatioNumerator[m_iAx]);
            //uint.TryParse(txtM_GRD.Text, out CWMX.Inst.AP.GearRatioDenominator[m_iAx]);
            //uint.TryParse(txtM_M.Text, out CWMX.Inst.AP.AxisMultiplier[m_iAx]);
            //CWMX.Inst.AP.AxisPolarity[m_iAx] = (sbyte)cboM_P.SelectedIndex;
            double.TryParse(txtM_IW.Text, out dInP);
            CWmx.It.SParm.FeedbackParam[m_iAx].InPosWidth = (int)(dInP * dPP1);

            //CWmx.It.SParm.LimitParam[m_iAx].InvertPositiveLSLogic = (byte)cboM_IPLS.SelectedIndex;
            //CWmx.It.SParm.LimitParam[m_iAx].InvertNegativeLSLogic = (byte)cboM_INLS.SelectedIndex;
            double.TryParse(txtM_QSD.Text, out CWmx.It.SParm.MotionParam[m_iAx].QuickStopDec);
            CWmx.It.SParm.MotionParam[m_iAx].QuickStopDec *= dPP1;

            // Init home parameter
            Enum.TryParse(cboM_HT.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomeType);
            CWmx.It.SParm.HomeParam[m_iAx].HomeDirection = (wmxCLRLibrary.common.HomeDirection)cboM_HD.SelectedIndex;
            CWmx.It.SParm.HomeParam[m_iAx].HSLogic = (byte)cboM_HS.SelectedIndex;
            double.TryParse(txtM_HVS.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlow);
            CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlow *= dPP1;
            double.TryParse(txtM_HVSA.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlowAcc);
            CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlowAcc *= dPP1;
            double.TryParse(txtM_HVSD.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlowDec);
            CWmx.It.SParm.HomeParam[m_iAx].HomingVelocitySlowDec *= dPP1;
            double.TryParse(txtM_HVF.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFast);
            CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFast *= dPP1;
            double.TryParse(txtM_HVFA.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFastAcc);
            CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFastAcc *= dPP1;
            double.TryParse(txtM_HVFD.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFastDec);
            CWmx.It.SParm.HomeParam[m_iAx].HomingVelocityFastDec *= dPP1;
            double.TryParse(txtM_HSDi.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomeShiftDistance);
            CWmx.It.SParm.HomeParam[m_iAx].HomeShiftDistance *= dPP1;
            double.TryParse(txtM_HSV.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomeShiftVelocity);
            CWmx.It.SParm.HomeParam[m_iAx].HomeShiftVelocity *= dPP1;
            double.TryParse(txtM_HSA.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomeShiftAcc);
            CWmx.It.SParm.HomeParam[m_iAx].HomeShiftAcc *= dPP1;
            double.TryParse(txtM_HSD.Text, out CWmx.It.SParm.HomeParam[m_iAx].HomeShiftDec);
            CWmx.It.SParm.HomeParam[m_iAx].HomeShiftDec *= dPP1;
        }

        private void _GetINI()
        {
            // General parameter
            CData.Axes[m_iAx].bUse = chkM_Use.Checked;
            double.TryParse(lbl_BSP.Text, out CData.Axes[m_iAx].dBSP);
            double.TryParse(lbl_1P.Text, out CData.Axes[m_iAx].dPP1);

            // Run slow parameter
            int.TryParse(txtM_RVS.Text, out CData.Axes[m_iAx].tRS.iVel);
            int.TryParse(txtM_RVSA.Text, out CData.Axes[m_iAx].tRS.iAcc);

            int.TryParse(txtM_RVSD.Text, out CData.Axes[m_iAx].tRS.iDec);
            // Run fast parameter
            int.TryParse(txtM_RVF.Text, out CData.Axes[m_iAx].tRN.iVel);
            int.TryParse(txtM_RVFA.Text, out CData.Axes[m_iAx].tRN.iAcc);
            int.TryParse(txtM_RVFD.Text, out CData.Axes[m_iAx].tRN.iDec);
            // Jog parameter
            int.TryParse(txtM_JV.Text, out CData.Axes[m_iAx].tJN.iVel);
            int.TryParse(txtM_JVA.Text, out CData.Axes[m_iAx].tJN.iAcc);
            int.TryParse(txtM_JVD.Text, out CData.Axes[m_iAx].tJN.iDec);

            // Software Limit
            double.TryParse(txtM_LNP.Text, out CData.Axes[m_iAx].dSWMin);
            double.TryParse(txtM_LPP.Text, out CData.Axes[m_iAx].dSWMax);

            // Acc, Dec Ratio
            int.TryParse(txtM_AccR.Text, out CData.Axes[m_iAx].iAccR);
            int.TryParse(txtM_DecR.Text, out CData.Axes[m_iAx].iDecR);

            // Check limit sensor
            CData.Axes[m_iAx].bNOT = chkM_NOT.Checked;
            CData.Axes[m_iAx].bPOT = chkM_POT.Checked;
        }

        /// <summary>
        /// koo : pause 개별 홈동작 area sensor 감지시 해당 축 일시 정지 
        /// </summary>
        /// <param name="OnLOn"></param>
        /// <param name="OffLOn"></param>
        /// <returns></returns>
        private bool _SetStop(bool OnLOn, bool OffLOn)
        {
            if (OnLOn)
            {
                //OnLoader elevator Parts
                CMot.It.EStop((int)EAx.OnLoader_X);
                CMot.It.EStop((int)EAx.OnLoader_Y);
                CMot.It.EStop((int)EAx.OnLoader_Z);
                //OnLoader Picker Parts
                CMot.It.EStop((int)EAx.OnLoaderPicker_X);
                CMot.It.EStop((int)EAx.OnLoaderPicker_Z);
                CMot.It.EStop((int)EAx.OnLoaderPicker_Y);
                //Inrail Parts
                CMot.It.EStop((int)EAx.Inrail_X);
                CMot.It.EStop((int)EAx.Inrail_Y);
                return true;
            }
            if (OffLOn)
            {
                //OffLoader elevator Parts
                CMot.It.EStop((int)EAx.OffLoader_X);
                CMot.It.EStop((int)EAx.OffLoader_Y);
                CMot.It.EStop((int)EAx.OffLoader_Z);

                //OffLoader Picker Parts
                CMot.It.EStop((int)EAx.OffLoaderPicker_X);
                CMot.It.EStop((int)EAx.OffLoaderPicker_Z);

                //Dryzone Parts
                CMot.It.EStop((int)EAx.DryZone_X);
                CMot.It.EStop((int)EAx.DryZone_Z);
                CMot.It.EStop((int)EAx.DryZone_Air);
                return true;
            }
            return false;
        }

        private void _EnableBtn(bool bVal)
        {
            this.Enabled = bVal;
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
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            cboM_Axis.SelectedIndex = 0;

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

        private void cboM_Axis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboM_Axis.SelectedIndex >= 0)
            {
                m_iAx = cboM_Axis.SelectedIndex + CMot.It.AxS;
                _SetLog("Click, Select axis:" + m_iAx);
                _Set();
            }
        }

        private void btnM_On_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :
            if (!CData.VIRTUAL)
            {
                CMot.It.Set_SOn(m_iAx, true);
            }            
        }

        private void btnM_Off_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :
            if (!CData.VIRTUAL)
            {
                CMot.It.Set_SOn(m_iAx, false);
            }            
        }

        private void btnM_CA_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :
            if (!CData.VIRTUAL)
            {
                CMot.It.Set_Clr(m_iAx);
            }            
        }

        private void btnM_H_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;
            bool Result=true;

            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                return;
            }

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Mv_H(m_iAx);
                if (iRet != 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Home error");
                    return;
                }
                else
                {

                    GU.Delay(GV.AX_DELAY);
                    while (true)
                    {
                        if (CMot.It.Get_HD(m_iAx))
                        {
                            break;
                        }
                        //koo : pause
                        if (CData.CurCompany == ECompany.SkyWorks)
                        {
                            if (_SetStop(CIO.It.Get_X(eX.ONL_LightCurtain), CIO.It.Get_X(eX.OFFL_LightCurtain)))
                            {
                                Result = false;
                                break;
                            }
                        }
                        // Time out error

                        Application.DoEvents();
                    }
                    //koo : pause
                    if (Result == true) CMsg.Show(eMsg.Notice, "Notice", "Home sequence ok");
                    else CMsg.Show(eMsg.Error, "Error", "Home error");  
                }
            }            
        }

        private void btnM_S_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Stop(m_iAx);
                if (iRet != 0)
                {
                    CMot.It.Stop(m_iAx);
                    CMsg.Show(eMsg.Error, "Error", "Axis stop error");
                }
                else
                {
                    while (true)
                    {
                        if (CMot.It.Chk_OP(m_iAx) == EAxOp.Idle)
                        { break; }
                        Application.DoEvents();

                        // Time out error
                    }

                    CMsg.Show(eMsg.Notice, "Noticce", "Axis stop ok");
                }
            }            
        }

        private void btnM_ES_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            if (!CData.VIRTUAL)
            {
                int iRet = 0;
                iRet = CMot.It.EStop(m_iAx);
            }            
        }

        private void btnM_M_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;
            double dPos = 0;

            // position value check
            if (!double.TryParse(txtM_Pos.Text, out dPos))
            {
                CMsg.Show(eMsg.Error, "Error", "Invalid position value");
                return;
            }

            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                return;
            }

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Mv_N(m_iAx, dPos);
                if (iRet != 0)
                {
                    CMot.It.EStop(m_iAx);
                    CMsg.Show(eMsg.Error, "Error", "Position move error");
                }
                else
                {
                    GU.Delay(GV.AX_DELAY);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(m_iAx, dPos))
                        {
                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }

                    CMsg.Show(eMsg.Notice, "Notice", "Position move ok");
                }
            }            
        }

        #region Jog move control
        // Jog positive
        private void btnM_PJ_MouseDown(object sender, MouseEventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;

            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            //// 200824 jym : 프로브 체크
            //if (m_iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            //{
            //    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
            //    return;
            //}

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Mv_J(m_iAx, true);
                if (iRet != 0)
                {
                    CMot.It.EStop(m_iAx);
                    CMsg.Show(eMsg.Error, "Error", "Jog positive error");
                }
                else
                { }
            }
        }

        // Jog negative
        private void btnM_NJ_MouseDown(object sender, MouseEventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;

            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            //// 200824 jym : 프로브 체크
            //if (m_iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            //{
            //    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
            //    return;
            //}

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Mv_J(m_iAx, false);
                if (iRet != 0)
                {
                    CMot.It.EStop(m_iAx);
                    CMsg.Show(eMsg.Error, "Error", "Jog negative error");
                }
                else
                { }
            }
        }

        // Jog stop
        private void btnM_MouseUp(object sender, MouseEventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.EStop(m_iAx);
            }
        }
        #endregion

        #region Step move control
        private void rdbM_CheckedChanged(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);

            RadioButton mRdb = sender as RadioButton;
            if (mRdb.Tag.ToString() == "Custom")
            {
            }
            else
            {
                double.TryParse(mRdb.Tag.ToString(), out m_dStep);
            }

            
        }

        private void btnM_StepN_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;
            double dStp = 0;

            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                return;
            }

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            if (rdbM_Cust.Checked)
            {
                // position value check
                if (!double.TryParse(txtM_Step.Text, out dStp))
                {
                    //에러처리
                    CMsg.Show(eMsg.Error, "Error", "Invalid step value");
                    return;
                }
            }
            else
            {
                dStp = m_dStep;
            }

            dStp *= -1.0;

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Mv_I(m_iAx, dStp);
                if (iRet != 0)
                {
                    //에러처리
                    CMot.It.EStop(m_iAx);
                    CMsg.Show(eMsg.Error, "Error", "Step move error");
                }
                else
                {
                    GU.Delay(100);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(m_iAx))
                        {
                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }

                    _SetLog("N, Step move success");
                    CMsg.Show(eMsg.Notice, "Notice", "Step move ok");
                }
            } 
        }

        private void btmM_StepP_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;
            double dStp = 0;

            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            // 200824 jym : 프로브 체크
            if (m_iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                return;
            }

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            if (rdbM_Cust.Checked)
            {
                // position value check
                if (!double.TryParse(txtM_Step.Text, out dStp))
                {
                    //에러처리
                    CMsg.Show(eMsg.Error, "Error", "Invalid step value");
                    return;
                }
            }
            else
            {
                dStp = m_dStep;
            }

            if (!CData.VIRTUAL)
            {
                iRet = CMot.It.Mv_I(m_iAx, dStp);
                if (iRet != 0)
                {
                    //에러처리
                    CMot.It.EStop(m_iAx);
                    CMsg.Show(eMsg.Error, "Error", "Step move error");
                }
                else
                {
                    GU.Delay(100);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(m_iAx))
                        {
                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }

                    CMsg.Show(eMsg.Notice, "Notice", "Step move ok");
                }
            }
        }
        #endregion

        private void btnM_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;

            _GetINI();    // 현재 화면에서 정보 취득

            _GetWMX();

            // 2-1 save XML file
            iRet = CWmx.It.Save();
            GU.Delay(1000);

            iRet = CWmx.It.Set_Home(m_iAx);
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "Set home parameter fail");
                return;
            }

            iRet = CMot.It.Save(m_iAx);     // INI 파일로 저장
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "Motion file save fail");
            }
            else
            {
                CMsg.Show(eMsg.Notice, "Notice", "Motion file save success");
                _Set();
            } 
        }

        #region Repeat control
        private void btnM_RepSt_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);

            TimeSpan tSpan;
            DateTime now = DateTime.Now;

            CData.bLastClick = true; //190509 ksg :

            int iRet = 0;
            double dSt;
            double dEd;
            double dOld = 0;
            double dTar = 0;
            double fCurrPos = 0.0;
            double fCurrPos_01 = 0.0;
            int iTm;
            int iAx = m_iAx;
            int nRepeatCount = 0;
            //tSpan = DateTime.Now - now;
            string stime;
            double dSpd;
            double dAcc;
            double dDec;


            // 200824 jym : 프로브 체크
            if (iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
            {
                CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                return;
            }
            // 200824 jym : 프로브 체크
            //if (iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
            //{
            //    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
            //    return;
            //}

            //190615 pjh:
            //Dry Stick Check
            if (m_iAx == 17)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(m_iAx))
            { return; }
            //

            // Check start position
            if (double.TryParse(txtM_RepSP.Text, out dSt) == false)
            {
                //에러처리
                CMsg.Show(eMsg.Error, "Error", "Invalid start position value");
                return;
            }

            // Check end position
            if (double.TryParse(txtM_RepEP.Text, out dEd) == false)
            {
                //에러처리
                CMsg.Show(eMsg.Error, "Error", "Invalid end position value");
                return;
            }

            // Check delay time
            if (int.TryParse(txtM_RepD.Text, out iTm) == false)
            {
                //에러처리
                CMsg.Show(eMsg.Error, "Error", "Invalid delay time");
                return;
            }
            else
            { iTm *= 1000; }

            if (!CData.VIRTUAL)
            {
                m_bRep = false;

                // Move start position
                iRet = CMot.It.Mv_N(iAx, dSt);
                now = DateTime.Now;
                if (iRet != 0)
                {
                    //에러처리
                    CMot.It.EStop(iAx);
                    CMsg.Show(eMsg.Error, "Error", "Start position move error");
                }
                else
                {
                   // GU.Delay(100);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(iAx, dSt))
                        {// 여기가 구동 완료
                            tSpan = DateTime.Now - now;
                            fCurrPos = CMot.It.Get_FP(iAx);
                            nRepeatCount = 0;
                            AddString(string.Format("Motor[{0}], {1}[mm] Move End", iAx, fCurrPos));

                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }

                    dOld = dSt;
                }

                // Delay
                GU.Delay(iTm);

                // Repeat move
                while (true)
                {
                    dSpd = CData.Axes[iAx].tRN.iVel;// * CData.Axes[iAx].dPP1;
                    dAcc = CData.Axes[iAx].tRN.iAcc;// * CData.Axes[iAx].dPP1;
                    dDec = CData.Axes[iAx].tRN.iDec;// * CData.Axes[iAx].dPP1;

                    if (dOld == dSt)
                    { dTar = dEd; }
                    else
                    { dTar = dSt; }

                    dOld = dTar;
                    // Move start position
                    fCurrPos = CMot.It.Get_FP(iAx);                    
                    iRet = CMot.It.Mv_N(iAx, dTar);
                    now = DateTime.Now;
                    if (fCurrPos < (dSt + 1.0)) nRepeatCount++;// 이동 회수 증가
                    if (iRet != 0)
                    {
                        CMot.It.EStop(iAx);
                        CMsg.Show(eMsg.Error, "Error", "Position move error");
                    }
                    else
                    {
                        //GU.Delay(100);
                        while (true)
                        {
                            if (CMot.It.Get_Mv(iAx, dTar))
                            {// 여기가 구동 완료
                                tSpan = DateTime.Now - now;
                                stime = tSpan.TotalSeconds.ToString("00.000");// ("[yyyy-MM-dd HH:mm:ss fff]");
                                fCurrPos_01 = CMot.It.Get_FP(iAx);
                                AddString(string.Format("Motor[{0},Spd:{1},Acc:{2}],Cnt:{3},{4}->{5}[mm],{6}[s]", iAx, dAcc, dSpd,nRepeatCount.ToString("00"), fCurrPos, fCurrPos_01, tSpan.TotalSeconds.ToString("00.000")));
                                break;
                            }

                            // Time out error

                            Application.DoEvents();
                        }

                    }

                    Application.DoEvents();

                    if (m_bRep)
                    {
                        CMsg.Show(eMsg.Notice, "Notice", "Repeat finish");
                        break;
                    }

                    // Delay
                    GU.Delay(iTm);
                }
            }
        }

        private void btnM_RepSp_Click(object sender, EventArgs e)
        {
            _SetLog("Click, Axis:" + (EAx)m_iAx);
            CData.bLastClick = true; //190509 ksg :

            m_bRep = true;
        }
        #endregion

        //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
        private bool _CheckStrip(int iAx)
        {
            bool result = false;

            if (iAx == (int)EAx.OnLoader_Y || iAx == (int)EAx.OnLoader_Z)
            {
                if (!CIO.It.Get_X(eX.INR_StripInDetect))
                {
                    //CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                    CMsg.Show(eMsg.Error, "Error", "INRAIL Strip In Detect");
                    result = true;
                }
            }
            else if (iAx == (int)EAx.OffLoader_Y || iAx == (int)EAx.OffLoader_Z)
            {
                if(!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    //CErr.Show(eErr.DRY_DETECTED_STRIP);
                    CMsg.Show(eMsg.Error, "Error", "Dry Out Detect Strip");
                    result = true;
                }
            }

            return result;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }        //



        #region function
        public void AddString(string sMsg)
        {

            int nLineCnt = 0;
            string stime = string.Empty;
            DateTime dtNow = DateTime.Now;

            //stime = dtNow.ToString("[yyyy-MM-dd HH:mm:ss fff]");
            stime = dtNow.ToString("[mm:ss.fff]");
            //lstLog.Items.Add(string.Format("{0}    {1}", stime, slog));
            sMsg = string.Format("{0} {1} \n", stime, sMsg);

            _SetLog("CBM Motor Test : " + sMsg);

            richmeasure_List.AppendText(sMsg);
            richmeasure_List.ScrollToCaret();

            nLineCnt = richmeasure_List.Lines.Length;
            if (nLineCnt > 100)
            {
                int cnt = nLineCnt - 101;
                for (int i = 0; i < cnt; i++)
                {
                    DeleteLine(0);
                }
            }

        }

        private void DeleteLine(int a_line)
        {
            int start_index = richmeasure_List.GetFirstCharIndexFromLine(a_line);
            int count = richmeasure_List.Lines[a_line].Length;

            // Eat new line chars
            if (a_line < richmeasure_List.Lines.Length - 1)
            {
                count += richmeasure_List.GetFirstCharIndexFromLine(a_line + 1)
                        - ((start_index + count - 1) + 1);
            }

            richmeasure_List.Text = richmeasure_List.Text.Remove(start_index, count);
        }
        #endregion function

        private void btnClear_Click(object sender, EventArgs e)
        {
            richmeasure_List.Clear();
        }
    }
}
