using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_08Tbl : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwOpt_08Tbl()
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

            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)      // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            {
                btnT_Ld_InspSix.Visible = true;
                btnT_Rd_InspSix.Visible = true;
                pnl_SixInsp.Visible = true;
            }
            else
            {
                btnT_Ld_InspSix.Visible = false;
                btnT_Rd_InspSix.Visible = false;
                pnl_SixInsp.Visible = false;
            }

            if (CDataOption.TableClean == eTableClean.NotUse)
            {
                CData.Opt.aTC_Cnt[(int)EWay.L] = 0;
                CData.Opt.aTC_Cnt[(int)EWay.R] = 0;
            }

            pnl_ManualSetPos.Visible = (CDataOption.TblSetPos == eTblSetPos.Use) ? true : false;
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            Set();

            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled) 
            { m_tmUpdt.Start(); }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled) 
            { m_tmUpdt.Stop(); }
        }

        public void Set()
        {
            // Step 1
            txtT_To1.Text   = CData.Opt.aTbGrd[0].dTotalDep.ToString();
            txtT_Cy1.Text   = CData.Opt.aTbGrd[0].dCycleDep.ToString();
            txtT_Tbl1.Text  = CData.Opt.aTbGrd[0].dTblSpd.ToString();
            txtT_Spl1.Text  = CData.Opt.aTbGrd[0].iSplSpd.ToString();

            // Step 2
            txtT_To2.Text   = CData.Opt.aTbGrd[1].dTotalDep.ToString();
            txtT_Cy2.Text   = CData.Opt.aTbGrd[1].dCycleDep.ToString();
            txtT_Tbl2.Text  = CData.Opt.aTbGrd[1].dTblSpd.ToString();
            txtT_Spl2.Text  = CData.Opt.aTbGrd[1].iSplSpd.ToString();

            // Air cut
            txtT_Air.Text = CData.Opt.dTbGrdAir.ToString();

            // Last date
            lblT_Ld_Last.Text = CData.Opt.dtLast_L.ToString();
            lblT_Rd_Last.Text = CData.Opt.dtLast_R.ToString();

            // 2021.03.23 SungTae Start : [추가]
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Opt.LeftTopPos    = CData.SPos.dGRD_Y_TblInsp[0];
                CData.Opt.LeftBtmPos    = CData.SPos.dGRD_Y_TblInsp[0] + Math.Abs((CData.SPos.dGRD_Y_TblInsp[0] - CData.MPos[0].dY_PRB_TBL_CENTER)) * 2;
                CData.Opt.RightTopPos   = CData.SPos.dGRD_Y_TblInsp[1];
                CData.Opt.RightBtmPos   = CData.SPos.dGRD_Y_TblInsp[1] + Math.Abs((CData.SPos.dGRD_Y_TblInsp[1] - CData.MPos[1].dY_PRB_TBL_CENTER)) * 2;

                //CData.Opt.LeftXPos      = CData.Axes[(int)EAx.LeftGrindZone_X].dSWMin;
                //CData.Opt.RightXPos     = CData.Axes[(int)EAx.RightGrindZone_X].dSWMax;
                //221004 pjh : 임시
                CData.Opt.LeftXPos      = 0;
                CData.Opt.RightXPos     = 0;
            }
            // 2021.03.23 SungTae End

            //191203 ksg : Table Grd Manual Pos
            txtT_LTopPos.Text = CData.Opt.LeftTopPos.ToString("F4");
            txtT_LBtmPos.Text = CData.Opt.LeftBtmPos.ToString("F4");
            txtT_RTopPos.Text = CData.Opt.RightTopPos.ToString("F4");
            txtT_RBtmPos.Text = CData.Opt.RightBtmPos.ToString("F4");
            txtT_LXPos.Text   = CData.Opt.LeftXPos.ToString("F4");
            txtT_RXPos.Text   = CData.Opt.RightXPos.ToString("F4");

            // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
            txtT_Ld_Limit_TB.Text = CData.Opt.LeftLimitTtoB.ToString("F4");
            txtT_Ld_Limit_LR.Text = CData.Opt.LeftLimitLtoR.ToString("F4");
            txtT_Rd_Limit_TB.Text = CData.Opt.RightLimitTtoB.ToString("F4");
            txtT_Rd_Limit_LR.Text = CData.Opt.RightLimitLtoR.ToString("F4");
            // 2021.10.08 SungTae End

            // 2021.03.23 SungTae Start : Table Grinding 허용 가능 잔여량 표시 위해 추가(ASE-KR)
            // 211022 syc : 2004U 경우에도 표시 추가
            if (CData.CurCompany == ECompany.ASE_KR || CDataOption.Use2004U) 
            {
                double dLMaxTblHeight = Math.Max(CData.Opt.aLTblMeasPos[0] , CData.Opt.aLTblMeasPos[1]);
                double dRMaxTblHeight = Math.Max(CData.Opt.aRTblMeasPos[0] , CData.Opt.aRTblMeasPos[1]);

                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    lblT_Ld_Remain.Text = (dLMaxTblHeight - (GV.EQP_TABLE_MIN_THICKNESS + 0.2)).ToString("F4");
                    lblT_Rd_Remain.Text = (dRMaxTblHeight - (GV.EQP_TABLE_MIN_THICKNESS + 0.2)).ToString("F4");
                }
                else if (CDataOption.Use2004U)
                {
                    lblT_Ld_Remain.Text = (dLMaxTblHeight - (GV.EQP_TABLE_MIN_THICKNESS + 0.6)).ToString("F4");
                    lblT_Rd_Remain.Text = (dRMaxTblHeight - (GV.EQP_TABLE_MIN_THICKNESS + 0.6)).ToString("F4");
                }
            }
            // 2021.03.23 SungTae End
        }

        public void Get()
        {
            // Step 1
            double.TryParse(txtT_To1.Text, out CData.Opt.aTbGrd[0].dTotalDep);
            double.TryParse(txtT_Cy1.Text, out CData.Opt.aTbGrd[0].dCycleDep);
            double.TryParse(txtT_Tbl1.Text, out CData.Opt.aTbGrd[0].dTblSpd);
            int.TryParse(txtT_Spl1.Text, out CData.Opt.aTbGrd[0].iSplSpd);

            // Step 2
            double.TryParse(txtT_To2.Text, out CData.Opt.aTbGrd[1].dTotalDep);
            double.TryParse(txtT_Cy2.Text, out CData.Opt.aTbGrd[1].dCycleDep);
            double.TryParse(txtT_Tbl2.Text, out CData.Opt.aTbGrd[1].dTblSpd);
            int.TryParse(txtT_Spl2.Text, out CData.Opt.aTbGrd[1].iSplSpd);

            // Air cut
            double.TryParse(txtT_Air.Text, out CData.Opt.dTbGrdAir);

            //191203 ksg : Table Grd Manual Pos
            double.TryParse(txtT_LTopPos.Text, out CData.Opt.LeftTopPos);
            double.TryParse(txtT_LBtmPos.Text, out CData.Opt.LeftBtmPos);
            double.TryParse(txtT_RTopPos.Text, out CData.Opt.RightTopPos);
            double.TryParse(txtT_RBtmPos.Text, out CData.Opt.RightBtmPos);
            double.TryParse(txtT_LXPos.Text, out CData.Opt.LeftXPos);
            double.TryParse(txtT_RXPos.Text, out CData.Opt.RightXPos);

            // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
            double.TryParse(txtT_Ld_Limit_TB.Text, out CData.Opt.LeftLimitTtoB);
            double.TryParse(txtT_Ld_Limit_LR.Text, out CData.Opt.LeftLimitLtoR);
            double.TryParse(txtT_Rd_Limit_TB.Text, out CData.Opt.RightLimitTtoB);
            double.TryParse(txtT_Rd_Limit_LR.Text, out CData.Opt.RightLimitLtoR);
            // 2021.10.08 SungTae End
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

        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            lblT_Ld_BfTop.Text = CData.Tbl_Bf[0, 0].ToString("F4");
            lblT_Ld_BfBtm.Text = CData.Tbl_Bf[0, 1].ToString("F4");
            lblT_Ld_AfTop.Text = CData.Tbl_Af[0, 0].ToString("F4");
            lblT_Ld_AfBtm.Text = CData.Tbl_Af[0, 1].ToString("F4");

            lblT_Rd_BfTop.Text = CData.Tbl_Bf[1, 0].ToString("F4");
            lblT_Rd_BfBtm.Text = CData.Tbl_Bf[1, 1].ToString("F4");
            lblT_Rd_AfTop.Text = CData.Tbl_Af[1, 0].ToString("F4");
            lblT_Rd_AfBtm.Text = CData.Tbl_Af[1, 1].ToString("F4");

            // 2021.09.23 SungTae Start : [추가]
            lblT_Ld_BfLeft.Text  = CData.Tbl_Bf[0, 2].ToString("F4");
            lblT_Ld_BfRight.Text = CData.Tbl_Bf[0, 3].ToString("F4");
            lblT_Ld_AfLeft.Text  = CData.Tbl_Af[0, 2].ToString("F4");
            lblT_Ld_AfRight.Text = CData.Tbl_Af[0, 3].ToString("F4");

            lblT_Rd_BfLeft.Text  = CData.Tbl_Bf[1, 2].ToString("F4");
            lblT_Rd_BfRight.Text = CData.Tbl_Bf[1, 3].ToString("F4");
            lblT_Rd_AfLeft.Text  = CData.Tbl_Af[1, 2].ToString("F4");
            lblT_Rd_AfRight.Text = CData.Tbl_Af[1, 3].ToString("F4");
            // 2021.09.23 SungTae End

            if (CData.TblMea == EMeaStep.After)
            {
                if (CData.Tbl_Bf[0, 0] != 999 && CData.Tbl_Af[0, 0] != 999)
                { lblT_Ld_LsTop.Text = (CData.Tbl_Bf[0, 0] - CData.Tbl_Af[0, 0]).ToString("F4"); }
                else
                { lblT_Ld_LsTop.Text = "0.0000"; }

                if (CData.Tbl_Bf[0, 1] != 999 && CData.Tbl_Af[0, 1] != 999)
                { lblT_Ld_LsBtm.Text = (CData.Tbl_Bf[0, 1] - CData.Tbl_Af[0, 1]).ToString("F4"); }
                else
                { lblT_Ld_LsBtm.Text = "0.0000"; }

                if (CData.Tbl_Bf[1, 0] != 999 && CData.Tbl_Af[1, 0] != 999)
                { lblT_Rd_LsTop.Text = (CData.Tbl_Bf[1, 0] - CData.Tbl_Af[1, 0]).ToString("F4"); }
                else
                { lblT_Rd_LsTop.Text = "0.0000"; }

                if (CData.Tbl_Bf[1, 1] != 999 && CData.Tbl_Af[1, 1] != 999)
                { lblT_Rd_LsBtm.Text = (CData.Tbl_Bf[1, 1] - CData.Tbl_Af[1, 1]).ToString("F4"); }
                else
                { lblT_Rd_LsBtm.Text = "0.0000"; }

                // 2021.09.23 SungTae Start : [추가]
                if (CData.Tbl_Bf[0, 2] != 999 && CData.Tbl_Af[0, 2] != 999)
                { lblT_Ld_LsLeft.Text = (CData.Tbl_Bf[0, 2] - CData.Tbl_Af[0, 2]).ToString("F4"); }
                else
                { lblT_Ld_LsLeft.Text = "0.0000"; }

                if (CData.Tbl_Bf[0, 3] != 999 && CData.Tbl_Af[0, 3] != 999)
                { lblT_Ld_LsRight.Text = (CData.Tbl_Bf[0, 3] - CData.Tbl_Af[0, 3]).ToString("F4"); }
                else
                { lblT_Ld_LsRight.Text = "0.0000"; }

                if (CData.Tbl_Bf[1, 2] != 999 && CData.Tbl_Af[1, 2] != 999)
                { lblT_Rd_LsLeft.Text = (CData.Tbl_Bf[1, 2] - CData.Tbl_Af[1, 2]).ToString("F4"); }
                else
                { lblT_Rd_LsLeft.Text = "0.0000"; }

                if (CData.Tbl_Bf[1, 3] != 999 && CData.Tbl_Af[1, 3] != 999)
                { lblT_Rd_LsRight.Text = (CData.Tbl_Bf[1, 3] - CData.Tbl_Af[1, 3]).ToString("F4"); }
                else
                { lblT_Rd_LsRight.Text = "0.0000"; }
                // 2021.09.23 SungTae End
            }

            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
            {
                lblT_Ld_St1Cnt.Text = CData.L_GRD.m_aGrd[0].ToString();
                lblT_Ld_St2Cnt.Text = CData.L_GRD.m_aGrd[1].ToString();
            }
            else
            {
                lblT_Ld_St1Cnt.Text = CData.L_GRD.m_aGrd_3[0].ToString();
                lblT_Ld_St2Cnt.Text = CData.L_GRD.m_aGrd_3[1].ToString();
            }

            lblT_Ld_CurCnt.Text = CData.L_GRD.m_iCnt.ToString();

            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
            {
                lblT_Rd_St1Cnt.Text = CData.R_GRD.m_aGrd[0].ToString();
                lblT_Rd_St2Cnt.Text = CData.R_GRD.m_aGrd[1].ToString();
            }
            else
            {
                lblT_Rd_St1Cnt.Text = CData.R_GRD.m_aGrd_3[0].ToString();
                lblT_Rd_St2Cnt.Text = CData.R_GRD.m_aGrd_3[1].ToString();
            }

            lblT_Rd_CurCnt.Text = CData.R_GRD.m_iCnt.ToString();

            lbl_LTSixLTop.Text = CData.Tbl_BfSix[0, 2].ToString("F4");
            lbl_LTSixLBtm.Text = CData.Tbl_BfSix[0, 3].ToString("F4");
            lbl_LTSixCTop.Text = CData.Tbl_BfSix[0, 0].ToString("F4");
            lbl_LTSixCBtm.Text = CData.Tbl_BfSix[0, 1].ToString("F4");
            lbl_LTSixRTop.Text = CData.Tbl_BfSix[0, 4].ToString("F4");
            lbl_LTSixRBtm.Text = CData.Tbl_BfSix[0, 5].ToString("F4");

            lbl_RTSixLTop.Text = CData.Tbl_BfSix[1, 2].ToString("F4");
            lbl_RTSixLBtm.Text = CData.Tbl_BfSix[1, 3].ToString("F4");
            lbl_RTSixCTop.Text = CData.Tbl_BfSix[1, 0].ToString("F4");
            lbl_RTSixCBtm.Text = CData.Tbl_BfSix[1, 1].ToString("F4");
            lbl_RTSixRTop.Text = CData.Tbl_BfSix[1, 4].ToString("F4");
            lbl_RTSixRBtm.Text = CData.Tbl_BfSix[1, 5].ToString("F4");

            // 2021.03.23 SungTae Start : Table Grinding 허용 가능 잔여량 표시 위해 추가(ASE-KR)

            lblT_Ld_BfDiff_TB.Text = CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.Before, 0].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.Before, 0] > CData.Opt.LeftLimitTtoB/*0.005*/)
            {
                lblT_Ld_BfDiff_TB.BackColor = System.Drawing.Color.Red;
                lblT_Ld_BfDiff_TB.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Ld_BfDiff_TB.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Ld_BfDiff_TB.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Ld_BfDiff_LR.Text = CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.Before, 1].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.Before, 1] > CData.Opt.LeftLimitLtoR/*0.005*/)
            {
                lblT_Ld_BfDiff_LR.BackColor = System.Drawing.Color.Red;
                lblT_Ld_BfDiff_LR.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Ld_BfDiff_LR.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Ld_BfDiff_LR.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Ld_AfDiff_TB.Text = CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.After, 0].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.After, 0] > CData.Opt.LeftLimitTtoB/*0.005*/)
            {
                lblT_Ld_AfDiff_TB.BackColor = System.Drawing.Color.Red;
                lblT_Ld_AfDiff_TB.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Ld_AfDiff_TB.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Ld_AfDiff_TB.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Ld_AfDiff_LR.Text = CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.After, 1].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.L, (int)EMeaStep.After, 1] > CData.Opt.LeftLimitLtoR/*0.005*/)
            {
                lblT_Ld_AfDiff_LR.BackColor = System.Drawing.Color.Red;
                lblT_Ld_AfDiff_LR.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Ld_AfDiff_LR.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Ld_AfDiff_LR.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Rd_BfDiff_TB.Text = CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.Before, 0].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.Before, 0] > CData.Opt.RightLimitTtoB/*0.005*/)
            {
                lblT_Rd_BfDiff_TB.BackColor = System.Drawing.Color.Red;
                lblT_Rd_BfDiff_TB.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Rd_BfDiff_TB.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Rd_BfDiff_TB.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Rd_BfDiff_LR.Text = CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.Before, 1].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.Before, 1] > CData.Opt.RightLimitLtoR/*0.005*/)
            {
                lblT_Rd_BfDiff_LR.BackColor = System.Drawing.Color.Red;
                lblT_Rd_BfDiff_LR.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Rd_BfDiff_LR.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Rd_BfDiff_LR.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Rd_AfDiff_TB.Text = CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.After, 0].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.After, 0] > CData.Opt.RightLimitTtoB/*0.005*/)
            {
                lblT_Rd_AfDiff_TB.BackColor = System.Drawing.Color.Red;
                lblT_Rd_AfDiff_TB.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Rd_AfDiff_TB.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Rd_AfDiff_TB.ForeColor = System.Drawing.Color.Blue;
            }

            lblT_Rd_AfDiff_LR.Text = CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.After, 1].ToString("F4");

            if (CData.dTblMeasDiff[(int)EWay.R, (int)EMeaStep.After, 1] > CData.Opt.RightLimitLtoR/*0.005*/)
            {
                lblT_Rd_AfDiff_LR.BackColor = System.Drawing.Color.Red;
                lblT_Rd_AfDiff_LR.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                lblT_Rd_AfDiff_LR.BackColor = System.Drawing.Color.WhiteSmoke;
                lblT_Rd_AfDiff_LR.ForeColor = System.Drawing.Color.Blue;
            }

            if (CData.CurCompany == ECompany.ASE_KR)
            {
                lblT_Ld_Remain_T.Visible    = true;
                lblT_Ld_Remain.Visible      = true;

                lblT_Rd_Remain_T.Visible    = true;
                lblT_Rd_Remain.Visible      = true;

                // 2021.09.23 SungTae Start : [추가]
                lblT_Ld_BfLeft_T.Visible    = true;
                lblT_Ld_BfRight_T.Visible   = true;
                lblT_Ld_AfLeft_T.Visible    = true;
                lblT_Ld_AfRight_T.Visible   = true;
                lblT_Ld_BfLeft.Visible      = true;
                lblT_Ld_BfRight.Visible     = true;
                lblT_Ld_AfLeft.Visible      = true;
                lblT_Ld_AfRight.Visible     = true;
                lblT_Ld_LsLeft_T.Visible    = true;
                lblT_Ld_LsRight_T.Visible   = true;
                lblT_Ld_LsLeft.Visible      = true;
                lblT_Ld_LsRight.Visible     = true;

                lblT_Rd_BfLeft_T.Visible    = true;
                lblT_Rd_BfRight_T.Visible   = true;
                lblT_Rd_AfLeft_T.Visible    = true;
                lblT_Rd_AfRight_T.Visible   = true;
                lblT_Rd_BfLeft.Visible      = true;
                lblT_Rd_BfRight.Visible     = true;
                lblT_Rd_AfLeft.Visible      = true;
                lblT_Rd_AfRight.Visible     = true;
                lblT_Rd_LsLeft_T.Visible    = true;
                lblT_Rd_LsRight_T.Visible   = true;
                lblT_Rd_LsLeft.Visible      = true;
                lblT_Rd_LsRight.Visible     = true;

                grb_LD_BfDiff.Visible       = true;
                grb_LD_AfDiff.Visible       = true;
                grb_RD_BfDiff.Visible       = true;
                grb_RD_AfDiff.Visible       = true;
                // 2021.09.23 SungTae End

                // 2021.10.08 SungTae Start : [추가]
                grb_GrdPosLeft.Visible      = true;
                grb_GrdPosRight.Visible     = true;
                grb_SetLimitLeft.Visible    = true;
                grb_SetLimitRight.Visible   = true;
                // 2021.10.08 SungTae End
            }
            //211022 syc : 2004U 테이블 잔여량 표시
            else if (CDataOption.Use2004U)
            {
                lblT_Ld_Remain_T.Visible    = true;
                lblT_Ld_Remain.Visible      = true;

                lblT_Rd_Remain_T.Visible    = true;
                lblT_Rd_Remain.Visible      = true;
            }
            else
            {
                lblT_Ld_Remain_T.Visible    = false;
                lblT_Ld_Remain.Visible      = false;

                lblT_Rd_Remain_T.Visible    = false;
                lblT_Rd_Remain.Visible      = false;

                // 2021.09.23 SungTae Start : [추가]
                lblT_Ld_BfLeft_T.Visible    = false;
                lblT_Ld_BfRight_T.Visible   = false;
                lblT_Ld_AfLeft_T.Visible    = false;
                lblT_Ld_AfRight_T.Visible   = false;
                lblT_Ld_BfLeft.Visible      = false;
                lblT_Ld_BfRight.Visible     = false;
                lblT_Ld_AfLeft.Visible      = false;
                lblT_Ld_AfRight.Visible     = false;
                lblT_Ld_LsLeft_T.Visible    = false;
                lblT_Ld_LsRight_T.Visible   = false;
                lblT_Ld_LsLeft.Visible      = false;
                lblT_Ld_LsRight.Visible     = false;

                lblT_Rd_BfLeft_T.Visible    = false;
                lblT_Rd_BfRight_T.Visible   = false;
                lblT_Rd_AfLeft_T.Visible    = false;
                lblT_Rd_AfRight_T.Visible   = false;
                lblT_Rd_BfLeft.Visible      = false;
                lblT_Rd_BfRight.Visible     = false;
                lblT_Rd_AfLeft.Visible      = false;
                lblT_Rd_AfRight.Visible     = false;
                lblT_Rd_LsLeft_T.Visible    = false;
                lblT_Rd_LsRight_T.Visible   = false;
                lblT_Rd_LsLeft.Visible      = false;
                lblT_Rd_LsRight.Visible     = false;

                grb_LD_BfDiff.Visible       = false;
                grb_LD_AfDiff.Visible       = false;
                grb_RD_BfDiff.Visible       = false;
                grb_RD_AfDiff.Visible       = false;
                // 2021.09.23 SungTae End

                // 2021.10.08 SungTae Start : [추가]
                grb_GrdPosLeft.Visible      = false;
                grb_GrdPosRight.Visible     = false;
                grb_SetLimitLeft.Visible    = false;
                grb_SetLimitRight.Visible   = false;
                // 2021.10.08 SungTae End
            }
            // 2021.03.23 SungTae End

            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 관련 설정값 확인 위해 추가
            lblT_Ld_TblGrdStPos.Text = CData.SPos.dGRD_Y_TblGrdSt[0].ToString("F4");
            lblT_Ld_TblGrdEdPos.Text = CData.SPos.dGRD_Y_TblGrdEd[0].ToString("F4");
            lblT_Rd_TblGrdStPos.Text = CData.SPos.dGRD_Y_TblGrdSt[1].ToString("F4");
            lblT_Rd_TblGrdEdPos.Text = CData.SPos.dGRD_Y_TblGrdEd[1].ToString("F4");
            // 2021.04.13 SungTae End
        }

        #endregion

        #region Private method
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

        private void btnT_Button_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);
            
            if (ESeq == ESeq.GRL_Table_Measure || ESeq == ESeq.GRL_Table_MeasureSix)
            {
                //if (CMsg.Show(eMsg.Warning, "Warning", "Do you want table inspection start?") == DialogResult.Cancel)
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want left table inspection start?") == DialogResult.Cancel)
                    return;
            }

            if (ESeq == ESeq.GRR_Table_Measure || ESeq == ESeq.GRR_Table_MeasureSix)
            {
                //if (CMsg.Show(eMsg.Warning, "Warning", "Do you want table inspection start?") == DialogResult.Cancel)
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want right table inspection start?") == DialogResult.Cancel)
                    return;
            }

            if (ESeq == ESeq.GRL_Table_Grinding)
            {
                //if (CMsg.Show(eMsg.Warning, "Warning", "Do you want table grinding start?") == DialogResult.Cancel)
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want left table grinding start?") == DialogResult.Cancel)
                    return;
            }

            if (ESeq == ESeq.GRR_Table_Grinding)
            {
                //if (CMsg.Show(eMsg.Warning, "Warning", "Do you want table grinding start?") == DialogResult.Cancel)
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want right table grinding start?") == DialogResult.Cancel)
                    return;
            }

            //200406 jym : 그라인딩 이전 Before 측정으로 플래그 변경
            CData.TblMea = EMeaStep.Before;

            CSQ_Man.It.Seq = ESeq;
        }
    }
}
