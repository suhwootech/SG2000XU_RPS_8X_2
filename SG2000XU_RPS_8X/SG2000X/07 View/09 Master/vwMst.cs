using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMst : UserControl
    {
        private int m_iAx1 = 0;

        private int m_iPage = 0;

        private bool m_bJog = false;

        private string m_sStep = "1";

        private Timer m_tmUp;

        public vwMst()
        {
            /*
            if ((int)eLanguage.English == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)eLanguage.Korea == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)eLanguage.China == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            */
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

            m_tmUp = new Timer();
            m_tmUp.Interval = 50;
            m_tmUp.Tick += M_tmUp_Tick;

            rdbS_1.Checked = true;

            //syc : Qorvo 수정 - 옵셋값 안보이게 수정, Master 들어가서 고객사가 볼 가능성 있음. 안보이게 수정.
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ)
            {
                panel4.Visible = false; //Left offset
                panel5.Visible = false; //Right offset
            }

        }

        #region Basic method
        public void Open()
        {
            if (m_tmUp != null && !CData.VIRTUAL)
            {
                if (!m_tmUp.Enabled)
                { m_tmUp.Start(); }
            }

            m_iPage = 0;
            tabGrd.SelectTab(0);
            m_iAx1 = (int)EAx.LeftGrindZone_X;

            _Set();
        }

        public void Close()
        {
            if (m_tmUp.Enabled)
            { m_tmUp.Stop(); }
        }

        public void Release()
        {
            if (m_tmUp != null)
            {
                if (m_tmUp.Enabled)
                { m_tmUp.Stop(); }

                m_tmUp.Dispose();
                m_tmUp = null;
            }
        }

        public void _Set()
        {
            if (!CData.VIRTUAL && CData.WMX)
            {
                chkLs_ProbDn.Checked = CIO.It.Get_Y(eY.GRDL_ProbeDn);
                chkRs_ProbDn.Checked = CIO.It.Get_Y(eY.GRDR_ProbeDn);
            }

            // Manual
            txtLs_MTblBs     .Text = CData.MPos[0].dZ_TBL_BASE       .ToString();
            txtLs_MWhlBs     .Text = CData.MPos[0].dZ_WHL_BASE       .ToString();
            txtLs_MDrsBs     .Text = CData.MPos[0].dZ_DRS_BASE       .ToString();
            txtLs_MTSetter   .Text = CData.MPos[0].dZ_PRB_TOOL_SETTER.ToString();
            txtLs_MTblCen    .Text = CData.MPos[0].dY_PRB_TBL_CENTER .ToString();
            txtLs_MWhlTbl    .Text = CData.MPos[0].dY_WHL_TBL_CENTER .ToString();
            txtLs_CTSetGap   .Text = CData.MPos[0].dTOOL_SETTER_GAP  .ToString();
            txtLs_ProbeOffset.Text = CData.MPos[0].fake       .ToString(); //190311 ksg :
            txtLs_PCBThick.Text = CData.MPos[0].dPCBThickness     .ToString();//191031 ksg :    

            // Calculate
            txtLs_CTblStPos     .Text = CData.MPos[0].dZ_TBL_MEA_POS    .ToString();
            txtLs_CDrsStPos     .Text = CData.MPos[0].dZ_DRS_MEA_POS    .ToString();
            txtLs_CWhlCenTSetter.Text = CData.MPos[0].dY_WHL_TOOL_SETTER.ToString();
            txtLs_CWhlCenDrsCen .Text = CData.MPos[0].dY_WHL_DRS        .ToString();
            txtLs_CPrbDrsCen    .Text = CData.MPos[0].dY_PRB_DRS        .ToString();
            txtLs_CProbWhlBs    .Text = CData.MPos[0].dPRB_TO_WHL_BASE  .ToString();

            // Manual
            txtRs_MTblBs     .Text = CData.MPos[1].dZ_TBL_BASE       .ToString();
            txtRs_MWhlBs     .Text = CData.MPos[1].dZ_WHL_BASE       .ToString();
            txtRs_MDrsBs     .Text = CData.MPos[1].dZ_DRS_BASE       .ToString();
            txtRs_MTSetter   .Text = CData.MPos[1].dZ_PRB_TOOL_SETTER.ToString();
            txtRs_MTblCen    .Text = CData.MPos[1].dY_PRB_TBL_CENTER .ToString();
            txtRs_MWhlTbl    .Text = CData.MPos[1].dY_WHL_TBL_CENTER .ToString();
            txtRs_CTSetGap   .Text = CData.MPos[1].dTOOL_SETTER_GAP  .ToString();
            txtRs_ProbeOffset.Text = CData.MPos[1].fake       .ToString(); //190311 ksg :

            // Calculate
            txtRs_CTblStPos     .Text = CData.MPos[1].dZ_TBL_MEA_POS    .ToString();
            txtRs_CDrsStPos     .Text = CData.MPos[1].dZ_DRS_MEA_POS    .ToString();
            txtRs_CWhlCenTSetter.Text = CData.MPos[1].dY_WHL_TOOL_SETTER.ToString();
            txtRs_CWhlCenDrsCen .Text = CData.MPos[1].dY_WHL_DRS        .ToString();
            txtRs_CPrbDrsCen    .Text = CData.MPos[1].dY_PRB_DRS        .ToString();
            txtRs_CProbWhlBs    .Text = CData.MPos[1].dPRB_TO_WHL_BASE  .ToString();

            // 2020.10.19 SungTae : Modify(Skip Count 0 추가)
            //cmb_ReSkip.SelectedIndex = CData.ReSkipCnt - 1;
            cmb_ReSkip.SelectedIndex = CData.ReSkipCnt;

            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            ckb_CalcByWheelZAxis.Checked = CData.bUseWheelZAxisAfterMeasureWheel;
            //
        }

        public void _Get()
        {
            // Manual
            double.TryParse(txtLs_MTblBs     .Text, out CData.MPos[0].dZ_TBL_BASE       );
            double.TryParse(txtLs_MWhlBs     .Text, out CData.MPos[0].dZ_WHL_BASE       );
            double.TryParse(txtLs_MDrsBs     .Text, out CData.MPos[0].dZ_DRS_BASE       );
            double.TryParse(txtLs_MTSetter   .Text, out CData.MPos[0].dZ_PRB_TOOL_SETTER);
            double.TryParse(txtLs_MTblCen    .Text, out CData.MPos[0].dY_PRB_TBL_CENTER );
            double.TryParse(txtLs_MWhlTbl    .Text, out CData.MPos[0].dY_WHL_TBL_CENTER );
            double.TryParse(txtLs_CTSetGap   .Text, out CData.MPos[0].dTOOL_SETTER_GAP  );

            double.TryParse(txtLs_ProbeOffset.Text, out CData.MPos[0].fake       ); //190311 ksg :
            double.TryParse(txtLs_PCBThick   .Text, out CData.MPos[0].dPCBThickness     ); //191031 ksg :

            // Calculate
            double.TryParse(txtLs_CTblStPos     .Text, out CData.MPos[0].dZ_TBL_MEA_POS    );
            double.TryParse(txtLs_CDrsStPos     .Text, out CData.MPos[0].dZ_DRS_MEA_POS    );
            double.TryParse(txtLs_CWhlCenTSetter.Text, out CData.MPos[0].dY_WHL_TOOL_SETTER);
            double.TryParse(txtLs_CWhlCenDrsCen .Text, out CData.MPos[0].dY_WHL_DRS        );
            double.TryParse(txtLs_CPrbDrsCen    .Text, out CData.MPos[0].dY_PRB_DRS        );
            double.TryParse(txtLs_CProbWhlBs    .Text, out CData.MPos[0].dPRB_TO_WHL_BASE  );

            // Manual
            double.TryParse(txtRs_MTblBs     .Text, out CData.MPos[1].dZ_TBL_BASE       );
            double.TryParse(txtRs_MWhlBs     .Text, out CData.MPos[1].dZ_WHL_BASE       );
            double.TryParse(txtRs_MDrsBs     .Text, out CData.MPos[1].dZ_DRS_BASE       );
            double.TryParse(txtRs_MTSetter   .Text, out CData.MPos[1].dZ_PRB_TOOL_SETTER);
            double.TryParse(txtRs_MTblCen    .Text, out CData.MPos[1].dY_PRB_TBL_CENTER );
            double.TryParse(txtRs_MWhlTbl    .Text, out CData.MPos[1].dY_WHL_TBL_CENTER );
            double.TryParse(txtRs_CTSetGap   .Text, out CData.MPos[1].dTOOL_SETTER_GAP  );
            double.TryParse(txtRs_ProbeOffset.Text, out CData.MPos[1].fake       ); //190311 ksg :

            // Calculate
            double.TryParse(txtRs_CTblStPos     .Text, out CData.MPos[1].dZ_TBL_MEA_POS    );
            double.TryParse(txtRs_CDrsStPos     .Text, out CData.MPos[1].dZ_DRS_MEA_POS    );
            double.TryParse(txtRs_CWhlCenTSetter.Text, out CData.MPos[1].dY_WHL_TOOL_SETTER);
            double.TryParse(txtRs_CWhlCenDrsCen .Text, out CData.MPos[1].dY_WHL_DRS        );
            double.TryParse(txtRs_CPrbDrsCen    .Text, out CData.MPos[1].dY_PRB_DRS        );
            double.TryParse(txtRs_CProbWhlBs    .Text, out CData.MPos[1].dPRB_TO_WHL_BASE  );

            // 2020.10.19 SungTae : Modify(Skip Count 0 추가)
            //CData.ReSkipCnt = cmb_ReSkip.SelectedIndex + 1;
            CData.ReSkipCnt = cmb_ReSkip.SelectedIndex;

            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            CData.bUseWheelZAxisAfterMeasureWheel = ckb_CalcByWheelZAxis.Checked;
            //
        }
        #endregion

        private void M_tmUp_Tick(object sender, EventArgs e)
        {
            if (!CData.VIRTUAL && CData.WMX)
            {
                lblLs_ProbDn .BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_Y(eY.GRDL_ProbeDn))];
                lblRs_ProbDn .BackColor = GV.CR_X[Convert.ToInt32(CIO.It.Get_Y(eY.GRDR_ProbeDn))];
                lblLs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(10, 7)];

                if (CData.DrData[0].bDrs)
                { lbl_DrsL.BackColor = Color.Lime; }
                else
                { lbl_DrsL.BackColor = Color.LightGray; }

                if (CData.DrData[1].bDrs)
                { lbl_DrsR.BackColor = Color.Lime; }
                else
                { lbl_DrsR.BackColor = Color.LightGray; }

                //20191022 ghk_spindle_type
                //if (CSpl.It.bUse232)
                //if (CDataOption.SplType == eSpindleType.Rs232)
                //{
                //    if (CDataOption.CurEqu == eEquType.Nomal)
                //    { lblRs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(85, 7)]; }
                //    else
                //    { lblRs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(89, 7)]; }
                //}
                //else
                //{
                //    if (CDataOption.CurEqu == eEquType.Nomal) lblRs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(97, 7)];
                //    else lblRs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(101, 7)];
                //}

                // 2023.03.15 Max
                if(CDataOption.CurEqu == eEquType.Nomal) lblRs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(97, 7)];
                else                                     lblRs_TSetter.BackColor = GV.CR_X[CIO.It.Get_X(101, 7)];

                for (int i = 0; i < 3; i++)
                {
                    int iAx = m_iAx1 + i;
                    string sId = (i + 1).ToString();
                    Panel mPnl = (Panel)this.Controls["pnlS_Ax" + sId];

                    mPnl.Controls["lblS_Ax" + sId].Text = ((EAx)iAx).ToString();
                    if (CMot.It.Chk_OP(iAx) == EAxOp.Idle)
                    { mPnl.Controls["lblS_Ax" + sId].BackColor = GV.CR_X[0]; }
                    else
                    { mPnl.Controls["lblS_Ax" + sId].BackColor = GV.CR_X[1]; }

                    mPnl.Controls["lblS_Srv" + sId].BackColor = GV.CR_X[CMot.It.Chk_SrvI(iAx)];
                    mPnl.Controls["lblS_Al" + sId].BackColor = GV.CR_Y[CMot.It.Chk_AlrI(iAx)];
                    mPnl.Controls["lblS_HD" + sId].BackColor = GV.CR_X[CMot.It.Chk_HDI(iAx)];
                    mPnl.Controls["lblS_N" + sId].BackColor = GV.CR_X[CMot.It.Chk_NOTI(iAx)];
                    mPnl.Controls["lblS_H" + sId].BackColor = GV.CR_X[CMot.It.Chk_HomeI(iAx)];
                    mPnl.Controls["lblS_P" + sId].BackColor = GV.CR_X[CMot.It.Chk_POTI(iAx)];

                    mPnl.Controls["lblS_Cmd" + sId].Text = CMot.It.Get_CP(iAx).ToString();
                    mPnl.Controls["lblS_Enc" + sId].Text = CMot.It.Get_FP(iAx).ToString();
                }
            }
        }

        private void tabGrd_SelectedIndexChanged(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            if (m_iPage != tabGrd.SelectedIndex)
            {
                m_iPage = tabGrd.SelectedIndex;

                rdbS_1.Checked = true;

                if (m_iPage == 0)
                {
                    m_iAx1 = (int)EAx.LeftGrindZone_X;
                }
                else if (m_iPage == 1)
                {
                    m_iAx1 = (int)EAx.RightGrindZone_X;
                }

                _Set();
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F " + m_iPage.ToString());
        }

        /// <summary>
        /// 스텝 라디오 버튼 체인지 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbS_CheckedChanged(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            RadioButton mRdb = sender as RadioButton;
            m_sStep = mRdb.Tag.ToString();

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F " + m_sStep.ToString());
        }

        private void chk_ProbDn_CheckedChanged(object sender, EventArgs e)
        {
            // 2020.10.28 JSKim St 
            if (!(CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))//syc : (Idle 이거나 Stop)이 아니면 확인
            {
                string sTemp = "";

                if (m_iPage == 0 && CIO.It.Get_Y(eY.GRDL_ProbeDn) == false && chkLs_ProbDn.Checked == true)
                {
                    sTemp = "Do you want to perform left probe down action?";
                }
                else if (m_iPage == 1 && CIO.It.Get_Y(eY.GRDR_ProbeDn) == false && chkRs_ProbDn.Checked == true)
                {
                    sTemp = "Do you want to perform right probe down action?";
                }

                if (sTemp != "")
                {
                    if (CMsg.Show(eMsg.Warning, "Warning", sTemp) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }            
            // 2020.10.28 JSKim Ed

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            string stmp = "";

            if (m_iPage == 0)
            {
                CIO.It.Set_Y(eY.GRDL_ProbeDn, chkLs_ProbDn.Checked);
                stmp = "GRDL " + chkLs_ProbDn.Checked.ToString();
            }
            else if (m_iPage == 1)
            {
                CIO.It.Set_Y(eY.GRDR_ProbeDn, chkRs_ProbDn.Checked);
                stmp = "GRDR " + chkRs_ProbDn.Checked.ToString();
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F " + stmp);
        }

        private void btnLs_PosGet_Click(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            Button mBtn = sender as Button;
            int iAx = 0;
            double dPos = 0;
            string[] aVal = mBtn.Tag.ToString().Split(',');

            if (m_iPage == 0)
            {
                if (aVal[1] == "Z")
                { iAx = (int)EAx.LeftGrindZone_Z; }
                else if (aVal[1] == "Y")
                { iAx = (int)EAx.LeftGrindZone_Y; }
                else
                {
                    MessageBox.Show("Error");
                    return;
                }

                dPos = CMot.It.Get_FP(iAx);
                pnlLs_Pos.Controls["pnlLs_" + aVal[0]].Controls["txtLs_" + aVal[0]].Text = dPos.ToString();
            }
            else if (m_iPage == 1)
            {
                if (aVal[1] == "Z")
                { iAx = (int)EAx.RightGrindZone_Z; }
                else if (aVal[1] == "Y")
                { iAx = (int)EAx.RightGrindZone_Y; }
                else
                {
                    MessageBox.Show("Error");
                    return;
                }

                dPos = CMot.It.Get_FP(iAx);
                pnlRs_Pos.Controls["pnlRs_" + aVal[0]].Controls["txtRs_" + aVal[0]].Text = dPos.ToString();
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        private void btnLs_PosMove_Click(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            Button mBtn = sender as Button;
            int iRet = 0;

            int iAx = 0;
            double dPos = 0;
            string[] aVal = mBtn.Tag.ToString().Split(',');

            if (m_iPage == 0)
            {
                if (aVal[1] == "Z")
                { iAx = (int)EAx.LeftGrindZone_Z; }
                else if (aVal[1] == "Y")
                { iAx = (int)EAx.LeftGrindZone_Y; }
                else
                {
                    MessageBox.Show("Error");
                    return;
                }

                if (double.TryParse(pnlLs_Pos.Controls["pnlLs_" + aVal[0]].Controls["txtLs_" + aVal[0]].Text, out dPos) == false)
                {
                    //에러처리
                    //_SetLog(eLog.ERR, "Invalid position value");
                    CMsg.Show(eMsg.Error, "Error", "Invalid position value");
                    return;
                }
            }
            else if (m_iPage == 1)
            {
                if (aVal[1] == "Z")
                { iAx = (int)EAx.RightGrindZone_Z; }
                else if (aVal[1] == "Y")
                { iAx = (int)EAx.RightGrindZone_Y; }
                else
                {
                    MessageBox.Show("Error");
                    return;
                }

                if (double.TryParse(pnlRs_Pos.Controls["pnlRs_" + aVal[0]].Controls["txtRs_" + aVal[0]].Text, out dPos) == false)
                {
                    //에러처리
                    CMsg.Show(eMsg.Error, "Error", "Invalid position value");
                    return;
                }
            }

            string sMsg = string.Format("{0} Axis move position -> {1} mm", ((EAx)iAx).ToString(), dPos);
            if (CMsg.Show(eMsg.Warning, "Warning", sMsg) == DialogResult.OK)
            {
                iRet = CMot.It.Mv_N(iAx, dPos);
                if (iRet != 0)
                {
                    //에러처리
                    //_SetLog(eLog.ERR, "Position move error");
                    CMot.It.EStop(iAx);
                    CMsg.Show(eMsg.Error, "Error", "Position move error");
                }
                else
                {
                    GU.Delay(200);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(iAx, dPos))
                        {
                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }
                }
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        private void btn_Calc_Click(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            _Calc();

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");
            CLog.Save_Log(eLog.None, eLog.DSL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            int iRet = 0;

            _Calc();

            if (CMsg.Show(eMsg.Warning, "Warninig", "Do you want to save change to Equipment position") == DialogResult.OK)
            {
                _Get();

                 iRet = CSetup.It.Save();  
                if (iRet != 0)
                {
                    //에러처리
                    CMsg.Show(eMsg.Error, "Error", "Position file save error");
                }

                CMsg.Show(eMsg.Notice, "Notice", "Save complete");
            }

            CLog.Save_Log(eLog.None, eLog.DSL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        private void _Calc()
        {
            //시작로그

            double dVal1 = 0;
            double dVal2 = 0;
            double dGap = 0;
            double dCalc = 0;
            // Table 높이 측정 시작 위치
            double.TryParse(txtLs_MTblBs.Text, out dVal1);
            dCalc = Math.Round(dVal1 - GV.EQP_TABLE_MIN_THICKNESS, 6);
            txtLs_CTblStPos.Text = dCalc.ToString();

            // Dresser 측정 시작 위치
            double.TryParse(txtLs_MDrsBs.Text, out dVal1);
            dCalc = Math.Round(dVal1 - GV.EQP_DRESSER_BLOCK_HEIGHT, 6);
            txtLs_CDrsStPos.Text = dCalc.ToString();

            // Wheel center가  Tool setter에 위치하는 포지션
            double.TryParse(txtLs_MWhlTbl.Text, out dVal1);            
            if (CDataOption.Package == ePkg.Unit)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_TOOLSETTER_U_MODEL, 6);
            }
            //210902 syc : 2004U
            else if (CDataOption.Use2004U)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_TOOLSETTER_4U_MODEL, 6);
            }
            else dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_TOOLSETTER, 6);
            txtLs_CWhlCenTSetter.Text = dCalc.ToString();

            // Wheel center가  Dresser center에 위치하는 포지션
            if (CDataOption.Package == ePkg.Unit)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_U_MODEL, 6);
            }
            //210902 syc : 2004U
            else if (CDataOption.Use2004U)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_4U_MODEL, 6);
            }
            else dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER, 6);
            txtLs_CWhlCenDrsCen.Text = dCalc.ToString();

            // 프로브가 드레셔 센터에 위치하는 포지션
            double.TryParse(txtLs_MTblCen.Text, out dVal1);
            if (CDataOption.Package == ePkg.Unit)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_U_MODEL, 6);
            }
                        //210902 syc : 2004U
            else if (CDataOption.Use2004U)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_4U_MODEL, 6);
            }
            else dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER, 6);
            txtLs_CPrbDrsCen.Text = dCalc.ToString();

            // Probe의 끝과 Wheel 베이스의 차이 값
            double.TryParse(txtLs_MWhlBs.Text, out dVal1);
            double.TryParse(txtLs_MTSetter.Text, out dVal2);
            double.TryParse(txtLs_CTSetGap.Text, out dGap);
            dCalc = Math.Abs(Math.Round((dVal1 - dVal2) - dGap, 5));
            txtLs_CProbWhlBs.Text = dCalc.ToString();

            // Table 높이 측정 시작 위치
            double.TryParse(txtRs_MTblBs.Text, out dVal1);
            dCalc = Math.Round(dVal1 - GV.EQP_TABLE_MIN_THICKNESS, 6);
            txtRs_CTblStPos.Text = dCalc.ToString();

            // Dresser 측정 시작 위치
            double.TryParse(txtRs_MDrsBs.Text, out dVal1);
            dCalc = Math.Round(dVal1 - GV.EQP_DRESSER_BLOCK_HEIGHT, 6);
            txtRs_CDrsStPos.Text = dCalc.ToString();

            // Wheel center가  Tool setter에 위치하는 포지션
            double.TryParse(txtRs_MWhlTbl.Text, out dVal1);
            if (CDataOption.Package == ePkg.Unit)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_TOOLSETTER_U_MODEL, 6);
            }
            //210902 syc : 2004U
            else if (CDataOption.Use2004U)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_TOOLSETTER_4U_MODEL, 6);
            }
            else dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_TOOLSETTER, 6);
            txtRs_CWhlCenTSetter.Text = dCalc.ToString();

            // Wheel center가  Dresser center에 위치하는 포지션
            if (CDataOption.Package == ePkg.Unit)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_U_MODEL, 6);
            }
            //210902 syc : 2004U
            else if (CDataOption.Use2004U)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_4U_MODEL, 6);
            }
            else dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER, 6);
            txtRs_CWhlCenDrsCen.Text = dCalc.ToString();

            // 프로브가 드레셔 센터에 위치하는 포지션
            double.TryParse(txtRs_MTblCen.Text, out dVal1);
            if (CDataOption.Package == ePkg.Unit)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_U_MODEL, 6);
            }
            //210902 syc : 2004U
            else if (CDataOption.Use2004U)
            {
                dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER_4U_MODEL, 6);
            }
            else dCalc = Math.Round(dVal1 - GV.EQP_TABLE_CENTER_TO_DRESSER_CENTER, 6);
            txtRs_CPrbDrsCen.Text = dCalc.ToString();

            // Probe의 끝과 Wheel 베이스의 차이 값
            double.TryParse(txtRs_MWhlBs.Text, out dVal1);
            double.TryParse(txtRs_MTSetter.Text, out dVal2);
            double.TryParse(txtRs_CTSetGap.Text, out dGap);
            dCalc = Math.Abs(Math.Round((dVal1 - dVal2) - dGap, 6));
            txtRs_CProbWhlBs.Text = dCalc.ToString();

            //종료로그
        }

        /// <summary>
        /// 축 이동버튼 클릭 이벤트 (조그 제외한 스탭 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Mv_Click(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            Button mBtn = sender as Button;
            int iRet = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;

            if (m_sStep == "Jog")
            {
                return;
            }
            else
            {
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                    return;
                }
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                    return;
                }

                double dStp = 0.0;
                if (m_sStep == "Custom")
                { double.TryParse(txtS_Cus.Text, out dStp); }
                else
                { double.TryParse(m_sStep, out dStp); }

                if (mBtn.Tag.ToString() == "N")
                { dStp *= -1; }

                iRet = CMot.It.Mv_I(iAx, dStp);
                if (iRet != 0)
                {
                    CMot.It.EStop(iAx);
                    CMsg.Show(eMsg.Error, "Error", "Step move error");
                }
                else
                {
                    GU.Delay(100);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(iAx))
                        {
                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }

                    CMsg.Show(eMsg.Notice, "Notice", "Step move ok");
                }
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        /// <summary>
        /// 스탑 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Stop_Click(object sender, EventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            Button mBtn = sender as Button;
            int iRet = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;

            iRet = CMot.It.Stop(iAx);
            if (iRet != 0)
            {
                //CErr.Show()
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        /// <summary>
        /// 축 이동버튼 마우스 다운 이벤트 (조그 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Mv_MouseDown(object sender, MouseEventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            if (m_sStep == "Jog")
            {
                Button mBtn = sender as Button;
                bool bPot = true;
                int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
                string sTxt = mBtn.Tag.ToString();

                if (sTxt == "N")
                { bPot = false; }

                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                    return;
                }
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                    return;
                }

                CMot.It.Mv_J(iAx, bPot);
            }
            else
            { return; }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        /// <summary>
        /// 축 이동버튼 마우스 업 이벤트 (조그 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Mv_MouseUp(object sender, MouseEventArgs e)
        {
            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            Button mBtn = sender as Button;

            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            //string sTxt = mBtn.Tag.ToString();

            CMot.It.EStop(iAx);

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " F");
        }

        private void btn_SkipDrs_Click(object sender, EventArgs e)
        {
            CData.DrData[0].bDrs = false;
            CData.DrData[1].bDrs = false;
        }

        // 2020.10.28 JSKim St
        private void btn_AllWaterOn_Click(object sender, EventArgs e)
        {
            if (CMsg.Show(eMsg.Warning, "Warning", "Do you want to perform all Water on action?") == DialogResult.Cancel)
            {
                return;
            }

            CLog.Save_Log(eLog.None, eLog.OPL, MethodBase.GetCurrentMethod().Name.ToString() + " S");

            CSQ_Man.It.WaterOnOff(true); //syc: All Water On / Of
            CSQ_Man.It.StartWater();
        }
        // 2020.10.28 JSKim Ed
    }
}
