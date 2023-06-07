using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_03Inr : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_03Inr()
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

            //190417 ksg :
            if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK) //200121 ksg :
            { ckbInr_IOOn.Visible = false; }
            else
            { ckbInr_IOOn.Visible = true; }

            //20190626 ghk_OnpBcr
            if (CDataOption.CurEqu != eEquType.Nomal)
            {
                btnInr_Bcr.Visible = false;
                btnInr_Ori.Visible = false;
                pnlInr_Bcr.Visible = false;
            }

            if (CDataOption.Package == ePkg.Unit)
            {
                lbl_GripClpR.Visible = true;
                lbl_Unit1.Visible = true;
                lbl_Unit2.Visible = true;
                lbl_Unit3.Visible = true;
                lbl_Unit4.Visible = true;
                lbl_StripBtm.Visible = false;

                ckb_PrbDn.Visible = false;
                ckb_PrbAir.Visible = false;
            }

            //211006 syc : 2004U
            if(CDataOption.Use2004U)
            {
                lbl_GripClpR.Visible = true;

                ckb_PrbDn.Visible = false;
                ckb_PrbAir.Visible = false;

            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            int iAx1 = (int)EAx.Inrail_X;
            int iAx2 = (int)EAx.Inrail_Y;

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // X
                if (CMot.It.Chk_Alr(iAx1)) { lblInr_XStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx1) == EAxOp.Idle)
                { lblInr_XStat.BackColor = Color.LightGray; }
                else { lblInr_XStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lblInr_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lblInr_XPos.Text = CMot.It.Get_FP(iAx1).ToString("0.000");
                }
                else
                {
                    lblInr_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Y
                if (CMot.It.Chk_Alr(iAx2)) { lblInr_YStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx2) == EAxOp.Idle)
                { lblInr_YStat.BackColor = Color.LightGray; }
                else { lblInr_YStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lblInr_YPos.Text = CMot.It.Get_FP(iAx2).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lblInr_YPos.Text = CMot.It.Get_FP(iAx2).ToString("0.000");
                }
                else
                {
                    lblInr_YPos.Text = CMot.It.Get_FP(iAx2).ToString();
                }
                // 2020.11.27 JSKim Ed

                ckbInr_GripClpOn.Checked = CIO.It.Get_Y(eY.INR_GripperClampOn);
                ckbInr_AlgnF.Checked = CIO.It.Get_Y(eY.INR_GuideForward);
                ckbInr_LiftUp.Checked = CIO.It.Get_Y(eY.INR_LiftUp);
                ckb_PrbDn.Checked = CIO.It.Get_Y(eY.INR_ProbeDn);
                ckb_PrbAir.Checked = CIO.It.Get_Y(eY.INR_ProbeAir);
                ckbInr_IOOn.Checked = CIO.It.Get_Y(eY.IOZL_Power); //190417 ksg :
            }
            lblInr_StripIn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_StripInDetect]];
            lbl_StripBtm.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_StripBtmDetect]];
            // Gauide Align Backward/Forward
            string text;
            if (CIO.It.Get_X(eX.INR_GuideForward) && !CIO.It.Get_X(eX.INR_GuideBackward))
            {
                lblInr_Algn.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Align") + " " + CLang.It.GetLanguage("Guide") + "\r\n" + CLang.It.GetLanguage("Forward");
                SetWriteLanguage(lblInr_Algn.Name, text);
            }
            else if (!CIO.It.Get_X(eX.INR_GuideForward) && CIO.It.Get_X(eX.INR_GuideBackward))
            {
                lblInr_Algn.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Align") + " " + CLang.It.GetLanguage("Guide") + "\r\n" + CLang.It.GetLanguage("Backward");
                SetWriteLanguage(lblInr_Algn.Name, text);
            }
            else
            {
                lblInr_Algn.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Align") + " " + CLang.It.GetLanguage("Guide") + "\r\n";
                SetWriteLanguage(lblInr_Algn.Name, text);
            }

            lblInr_GripD.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GripperDetect]];

            // Gripper Unclamp/Clamp
            if (CIO.It.Get_X(eX.INR_GripperClampOn) && !CIO.It.Get_X(eX.INR_GripperClampOff))
            {
                lbl_GripClp.BackColor = Color.Lime;
                if (CDataOption.Package == ePkg.Strip)
                { text = CLang.It.GetLanguage("Gripper") + "\r\n" + CLang.It.GetLanguage("Clamp"); }
                else
                { text = CLang.It.GetLanguage("Gripper Front") + "\r\n" + CLang.It.GetLanguage("Clamp"); }
                SetWriteLanguage(lbl_GripClp.Name, text);
            }
            else if (!CIO.It.Get_X(eX.INR_GripperClampOn) && CIO.It.Get_X(eX.INR_GripperClampOff))
            {
                lbl_GripClp.BackColor = Color.LightGray;
                if (CDataOption.Package == ePkg.Strip)
                {
                    text = CLang.It.GetLanguage("Gripper") + "\r\n" + CLang.It.GetLanguage("Unclamp");
                }
                else
                { text = CLang.It.GetLanguage("Gripper Front") + "\r\n" + CLang.It.GetLanguage("Unclamp"); }
                SetWriteLanguage(lbl_GripClp.Name, text);
            }
            else
            {
                lbl_GripClp.BackColor = Color.LightGray;
                if (CDataOption.Package == ePkg.Strip)
                { text = CLang.It.GetLanguage("Gripper") + "\r\n"; }
                else
                { text = CLang.It.GetLanguage("Gripper Front") + "\r\n"; }
                SetWriteLanguage(lbl_GripClp.Name, text);
            }

            // Lift Down/Up
            if (CIO.It.Get_X(eX.INR_LiftUp) && !CIO.It.Get_X(eX.INR_LiftDn))
            {
                lblInr_LiftU.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Gripper") + " " + CLang.It.GetLanguage("Up");
                SetWriteLanguage(lblInr_LiftU.Name, text);
            }
            else if (!CIO.It.Get_X(eX.INR_LiftUp) && CIO.It.Get_X(eX.INR_LiftDn))
            {
                lblInr_LiftU.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Lift") + " " + CLang.It.GetLanguage("Down");
                SetWriteLanguage(lblInr_LiftU.Name, text);
            }
            else
            {
                lblInr_LiftU.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Lift");
                SetWriteLanguage(lblInr_LiftU.Name, text);
            }

            lblInr_Bcr.Text = CData.Bcr.sBcr;

            //20190220 ghk_dynamicfunction
            lblM_Pcb1.Text = CData.Dynamic.dPcb[0].ToString();
            lblM_Pcb2.Text = CData.Dynamic.dPcb[1].ToString();
            lblM_Pcb3.Text = CData.Dynamic.dPcb[2].ToString();
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            lblM_Pcb4.Text = CData.Dynamic.dPcb[3].ToString();
            lblM_Pcb5.Text = CData.Dynamic.dPcb[4].ToString();
            //

#if true //20200331 jhc : DF Max/Mean/TTV 표시 버그 수정
            double[] dTempPcb = new double[CData.Dev.iDynamicPosNum];
            for (int i = 0; i < CData.Dev.iDynamicPosNum; i++) dTempPcb[i] = CData.Dynamic.dPcb[i];
            double dTmpPcbMax  = dTempPcb.Max();
            double dTmpPcbMean = Math.Round(dTempPcb.Average(), 4);
            double dTmpPcbTtv  = Math.Round((dTempPcb.Max() - dTempPcb.Min()), 4);
            lblM_PcbMax.Text  = dTmpPcbMax.ToString();
            lblM_PcbMean.Text = dTmpPcbMean.ToString();
            lblM_PcbTtv.Text  = dTmpPcbTtv.ToString();
#else
            CData.Dynamic.dPcbMax = CData.Dynamic.dPcb.Max();
            lblM_PcbMax.Text = CData.Dynamic.dPcbMax.ToString();

            CData.Dynamic.dPcbMean = Math.Round(CData.Dynamic.dPcb.Average(), 4);
            lblM_PcbMean.Text = CData.Dynamic.dPcbMean.ToString();

            CData.Dynamic.dPcbTtv = Math.Round((CData.Dynamic.dPcb.Max() - CData.Dynamic.dPcb.Min()), 4);
            lblM_PcbTtv.Text = CData.Dynamic.dPcbTtv.ToString();
#endif

            //20191029 ghk_dfserver_notuse_df
            if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
            {//DF 사용시 DF 측정 할경우
                pl_Dynamic.Visible = !CData.Dev.bDynamicSkip;
                btnInr_Dynamic.Visible = !CData.Dev.bDynamicSkip;
            }


             if (CDataOption.Package == ePkg.Unit || CDataOption.Use2004U) //211006 syc : 2004U 일 경우에도 추가
            {
                lbl_GripClpR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GripperClampOn_Rear]];
                // Gripper Unclamp/Clamp
                if (CIO.It.Get_X(eX.INR_GripperClampOn_Rear) && !CIO.It.Get_X(eX.INR_GripperClampOff_Rear))
                {
                    lbl_GripClpR.BackColor = Color.Lime;
                    text = CLang.It.GetLanguage("Gripper Rear") + "\r\n" + CLang.It.GetLanguage("Clamp");
                    SetWriteLanguage(lbl_GripClpR.Name, text);
                }
                else if (!CIO.It.Get_X(eX.INR_GripperClampOn_Rear) && CIO.It.Get_X(eX.INR_GripperClampOff_Rear))
                {
                    lbl_GripClpR.BackColor = Color.LightGray;
                    text = CLang.It.GetLanguage("Gripper Rear") + "\r\n" + CLang.It.GetLanguage("Unclamp");
                    SetWriteLanguage(lbl_GripClpR.Name, text);

                }
                else
                {
                    lbl_GripClp.BackColor = Color.LightGray;
                    text = CLang.It.GetLanguage("Gripper Rear") + "\r\n";
                    SetWriteLanguage(lbl_GripClp.Name, text);
                }

                lbl_UTilt.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_Unit_Tilt]];
                lbl_Unit1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_Unit_1_Detect]];
                lbl_Unit2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_Unit_2_Detect]];
                lbl_Unit3.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_Unit_3_Detect]];
                lbl_Unit4.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_Unit_4_Detect]];
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

        public void _EnableBtn(bool bVal)
        {
            //inRail   
            btn2_Home        .Enabled = bVal;
            btn2_Wait        .Enabled = bVal;
            btnInr_Algn      .Enabled = bVal;
            btnInr_Dynamic   .Enabled = bVal;
            ckbInr_AlgnF     .Enabled = bVal;
            ckbInr_GripClpOn .Enabled = bVal;
            ckbInr_LiftUp    .Enabled = bVal;
            ckb_PrbDn   .Enabled = bVal;
            ckb_PrbAir.Enabled = bVal;
            btnInr_Bcr       .Enabled = bVal; //200211 ksg :
            btnInr_Ori       .Enabled = bVal; //200211 ksg :
        }

        /// <summary>
        /// Manual inrail view에 조작 로그 저장 함수
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

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        //SetConvertLanguage(lblS_GrdL_PumpA.Name,CData.Opt.iSelLan,"Overload");
        private void SetWriteLanguage(string controlName, string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }
#endregion

#region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            ckbInr_GripClpOn.Checked = CIO.It.Get_Y(eY.INR_GripperClampOn);
            ckbInr_AlgnF.Checked = CIO.It.Get_Y(eY.INR_GuideForward);
            ckbInr_LiftUp.Checked = CIO.It.Get_Y(eY.INR_LiftUp);
            ckb_PrbDn.Checked = CIO.It.Get_Y(eY.INR_ProbeDn);
            ckb_PrbAir.Checked = CIO.It.Get_Y(eY.INR_ProbeAir);
            ckbInr_IOOn.Checked = CIO.It.Get_Y(eY.IOZL_Power); //190417 ksg :

            //20191029 ghk_dfserver_notuse_df
            if (CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {//DF 사용시 DF 측정 할경우
                pl_Dynamic.Visible = false;
                btnInr_Dynamic.Visible = false;
            }

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

        private void btn_Cycle_Click(object sender, EventArgs e)
        {
            //190627 ksg : 
            _EnableBtn(false);
            CSQ_Man.It.bBtnShow = false;

            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);
            //190226 ksg :추가

            _SetLog(string.Format("N, {0} click", ESeq));
            //190414 ksg :
            if (!((ESeq == ESeq.All_Home) || (ESeq == ESeq.All_Servo_On) || (ESeq == ESeq.All_Servo_Off) || (ESeq == ESeq.ONL_Home) ||
                  (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home    ) || (ESeq == ESeq.GRL_Home     ) || (ESeq == ESeq.GRR_Home) ||
                  (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home    ) || (ESeq == ESeq.OFL_Home     )                              ))
            {
                //20210903 syc Test ----------------------------
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }

            //190211 ksg :
            if (CDataOption.CurEqu == eEquType.Nomal)
            {
                if (ESeq == ESeq.INR_CheckBcr || ESeq == ESeq.INR_CheckOri)
                {
                    CBcr.It.LoadRecipe();
                    if (CData.Bcr.sDX != CData.Bcr.sBCR)
                    {
                        CSQ_Man.It.bBtnShow = true; //190627 ksg :
                        CMsg.Show(eMsg.Error, "Error", "Different the device & BCR Recipe");
                        return;
                    }
                }
            }
            else
            {//20190604 ghk_onpbcr
                if (ESeq == ESeq.ONP_CheckBcr || ESeq == ESeq.ONP_CheckOri)
                {
                    CBcr.It.LoadRecipe();
                    if (CData.Bcr.sDX != CData.Bcr.sBCR)
                    {
                        CSQ_Man.It.bBtnShow = true; //190627 ksg :
                        CMsg.Show(eMsg.Error, "Error", "Different the device & BCR Recipe");
                        return;
                    }
                }
            }

            // 도어 체크  // 2022.06.11 lhs
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (!CSQ_Main.It.CheckDoor()) // SCK,SCK+
                {
                    CSQ_Man.It.bBtnShow = true;
                    CMsg.Show(eMsg.Warning, "Warning", "Door close Please");
                    return;
                }
            }

            //ksg 추가
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                return;
            }
            else
            {
                //CData.L_GRD.m_tStat.bDrsR = true;
                CSQ_Man.It.Seq = ESeq;
                //190410 ksg :
                _EnableBtn(false);
                CSQ_Man.It.bBtnShow = false;
            }

            _SetLog("F");
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            //ksg : 스톱 함수가 여긴가?
            CSQ_Man.It.bBtnShow = true;
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CSQ_Man.It.bStop = true;
            }
        }

        /// <summary>
        /// In Rail Part CheckBox Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            string sTxt;

            switch (iTag)
            {
                case 0:    // Align Guide Forward
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("ALIGN") + "\r\n" + CLang.It.GetLanguage("GUIDE") + "\r\n" + CLang.It.GetLanguage("FORWARD");
                    else
                        sTxt = CLang.It.GetLanguage("ALIGN") + "\r\n" + CLang.It.GetLanguage("GUIDE") + "\r\n" + CLang.It.GetLanguage("BACKWARD");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;
                case 1:    // Gripper Clamp On
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("GRIPPER") + "\r\n" + CLang.It.GetLanguage("CLAMP");
                    else
                        sTxt = CLang.It.GetLanguage("GRIPPER") + "\r\n" + CLang.It.GetLanguage("UNCLAMP");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;
                case 2:    // Lift Up
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("LIFT") + "\r\n" + CLang.It.GetLanguage("UP");
                    else
                        sTxt = CLang.It.GetLanguage("LIFT") + "\r\n" + CLang.It.GetLanguage("DOWN");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 3:
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("PROBE") + "\r\n" + CLang.It.GetLanguage("DOWN");
                    else
                        sTxt = CLang.It.GetLanguage("PROBE") + "\r\n" + CLang.It.GetLanguage("UP");

                    SetWriteLanguage(mCkb.Name, sTxt);

                    break;
                case 4:
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("PROBE") + "\r\n" + CLang.It.GetLanguage("AIR") + "\r\n" + CLang.It.GetLanguage("ON");
                    else
                        sTxt = CLang.It.GetLanguage("PROBE") + "\r\n" + CLang.It.GetLanguage("AIR") + "\r\n" + CLang.It.GetLanguage("OFF");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 5:
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("ON");
                    else
                        sTxt = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("OFF");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;
            }
        }

        private void ckb_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            _SetLog("Click, Tag:" + iTag);

            //190430 ksg : Auto & Manual 때 동작 안함
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            switch (iTag)
            {
                case 0:    // Align Guide Forward
                    CSq_Inr.It.Act_YAlignBF(mCkb.Checked);
                    break;
                case 1:    // Gripper Clamp On
                    CSq_Inr.It.Func_GripperClamp(mCkb.Checked);
                    break;
                case 2:    // Lift Up
                    CSq_Inr.It.Act_ZLiftDU(mCkb.Checked);
                    break;
                case 3:
                    CSq_Inr.It.Act_ProbeUD(mCkb.Checked);
                    break;
                case 4:
                    CIO.It.Set_Y(eY.INR_ProbeAir, mCkb.Checked);
                    break;
                case 5:
                    CIO.It.Set_Y(eY.IOZL_Power, mCkb.Checked);
                    break;
            }
        }

        /// <summary>
        /// Label text On/Off 변경 이벤트 [A접점]
        /// 190325-maeng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblIO_A_BackColorChanged(object sender, EventArgs e)
        {
            Label mLbl = sender as Label;
            string sVal = mLbl.Tag.ToString().Replace("$", "\r\n");
            string text;
            if (mLbl.BackColor == Color.Lime)
            {
                text = sVal + " " + CLang.It.GetLanguage("On");
                SetWriteLanguage(mLbl.Name, text);
            }
            else
            {
                text = sVal + " " + CLang.It.GetLanguage("Off");
                SetWriteLanguage(mLbl.Name, text);
            }
        }

        /// <summary>
        /// Label text On/Off 변경 이벤트 [B접점]
        /// 190409-maeng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblIO_B_BackColorChanged(object sender, EventArgs e)
        {
            Label mLbl = sender as Label;
            string sVal = mLbl.Tag.ToString().Replace("$", "\r\n");
            string text;
            if (mLbl.BackColor == Color.Lime)
            {
                text = sVal + " " + CLang.It.GetLanguage("Off");
                SetWriteLanguage(mLbl.Name, text);
            }
            else
            {
                text = sVal + " " + CLang.It.GetLanguage("On");
                SetWriteLanguage(mLbl.Name, text);
            }
        }
    }
}
