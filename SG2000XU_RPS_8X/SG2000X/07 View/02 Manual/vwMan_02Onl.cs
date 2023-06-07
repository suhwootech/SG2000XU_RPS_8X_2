using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_02Onl : UserControl
    {   
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_02Onl()
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
            int iAx1 = (int)EAx.OnLoader_X;
            int iAx2 = (int)EAx.OnLoader_Y;
            int iAx3 = (int)EAx.OnLoader_Z;
            string sTxt;

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // X
                if (CMot.It.Chk_Alr(iAx1)) { lbl1_XStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx1) == EAxOp.Idle)
                { lbl1_XStat.BackColor = Color.LightGray; }
                else { lbl1_XStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl1_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl1_XPos.Text = CMot.It.Get_FP(iAx1).ToString("0.000");
                }
                else
                {
                    lbl1_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Y
                if (CMot.It.Chk_Alr(iAx2)) { lbl1_YStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx2) == EAxOp.Idle)
                { lbl1_YStat.BackColor = Color.LightGray; }
                else { lbl1_YStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl1_YPos.Text = CMot.It.Get_FP(iAx2).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl1_YPos.Text = CMot.It.Get_FP(iAx2).ToString("0.000");
                }
                else
                {
                    lbl1_YPos.Text = CMot.It.Get_FP(iAx2).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Z
                if (CMot.It.Chk_Alr(iAx3)) { lbl1_ZStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx3) == EAxOp.Idle)
                { lbl1_ZStat.BackColor = Color.LightGray; }
                else { lbl1_ZStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl1_ZPos.Text = CMot.It.Get_FP(iAx3).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl1_ZPos.Text = CMot.It.Get_FP(iAx3).ToString("0.000");
                }
                else
                {
                    lbl1_ZPos.Text = CMot.It.Get_FP(iAx3).ToString();
                }
                // 2020.11.27 JSKim Ed

                ckb1_Clamp.Checked = CIO.It.Get_Y(eY.ONL_ClampOn);
                ckb1_Pusher.Checked = CIO.It.Get_Y(eY.ONL_PusherForward);
                ckb1_TopRun.Checked = CIO.It.Get_Y(eY.ONL_TopBeltRun);
                ckb1_CCW.Checked = CIO.It.Get_Y(eY.ONL_BtmBeltCCW);
                ckb1_BtmRun.Checked = CIO.It.Get_Y(eY.ONL_BtmBeltRun);
            }

            lbl1_EmgF.BackColor = GV.CR_Y[CIO.It.aInput[(int)eX.ONL_EMGFront]];
            lbl1_EmgR.BackColor = GV.CR_Y[CIO.It.aInput[(int)eX.ONL_EMGRear]];
            lbl1_DorF.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_DoorFront]];
            lbl1_DorL.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_DoorLeft]];
            lbl1_DorR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_DoorRear]];
            lbl1_LiCur.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_LightCurtain]];
            lbl1_TcMgzD.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_TopMGZDetect]];
            lbl1_TcMgzF.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_TopMGZDetectFull]];
            lbl1_BtMgzD.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_BtmMGZDetect]];
            lbl1_PushO.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_PusherOverLoad]];
            
            if (CIO.It.Get_X(eX.ONL_PusherForward) && !CIO.It.Get_X(eX.ONL_PusherBackward))
            {
                lbl1_Push.BackColor = Color.Lime;
                sTxt = CLang.It.GetLanguage("Pusher") + "\r\n" + CLang.It.GetLanguage("Forward");
                SetWriteLanguage(lbl1_Push.Name, sTxt);

            }
            else if (!CIO.It.Get_X(eX.ONL_PusherForward) && CIO.It.Get_X(eX.ONL_PusherBackward))
            {
                lbl1_Push.BackColor = Color.LightGray;
                sTxt = CLang.It.GetLanguage("Pusher") + "\r\n" + CLang.It.GetLanguage("Backward");
                SetWriteLanguage(lbl1_Push.Name, sTxt);
            }
            else
            {
                lbl1_Push.BackColor = Color.LightGray;
                sTxt = CLang.It.GetLanguage("Pusher") + "\r\n";
                SetWriteLanguage(lbl1_Push.Name, sTxt);
            }

            lbl1_MgzD1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_ClampMGZDetect1]];
            lbl1_MgzD2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_ClampMGZDetect2]];
            if (CIO.It.Get_X(eX.ONL_ClampOn) && !CIO.It.Get_X(eX.ONL_ClampOff))
            {
                lbl1_Clamp.BackColor = Color.Lime;
                sTxt = CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Clamp");
                SetWriteLanguage(lbl1_Clamp.Name, sTxt);
            }
            else if (!CIO.It.Get_X(eX.ONL_ClampOn) && CIO.It.Get_X(eX.ONL_ClampOff))
            {
                lbl1_Clamp.BackColor = Color.LightGray;
                sTxt = CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Unclamp");
                SetWriteLanguage(lbl1_Clamp.Name, sTxt);
            }
            else
            {
                lbl1_Clamp.BackColor = Color.LightGray;
                sTxt = CLang.It.GetLanguage("MGZ") + "\r\n";
                SetWriteLanguage(lbl1_Clamp.Name, sTxt);
            }

            // 2022.04.19 SungTae Start : [추가] ASE-KH 관련
            if (CData.CurCompany == ECompany.ASE_K12)
            {
                btn1_CheckBcr.Visible   = true;
                pnlOnl_Bcr.Visible      = true;
                
                lblOnl_Bcr.Text = CData.KeyBcr[(int)EKeyenceBcrPos.OnLd].sBcr;
            }
            else
            {
                btn1_CheckBcr.Visible   = false;
                pnlOnl_Bcr.Visible      = false;
            }
            // 2022.04.19 SungTae End
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

        private void _EnableBtn(bool bVal)
        {
            //OnLoader Btn
            btn1_Hm      .Enabled = bVal;
            btn1_Wait    .Enabled = bVal;
            btn1_Pick    .Enabled = bVal;
            btn1_Place   .Enabled = bVal;
            btn1_Push    .Enabled = bVal;
            btn1_CheckBcr.Enabled = bVal;       // 2022.04.20 SungTae : [추가] ASE-KH 관련
            ckb1_Clamp   .Enabled = bVal;
            ckb1_Pusher  .Enabled = bVal;
            ckb1_TopRun  .Enabled = bVal;
            ckb1_BtmRun  .Enabled = bVal;
            ckb1_CCW     .Enabled = bVal;
            btn1_Push_Pos.Enabled = bVal;
        }

        /// <summary>
        /// Manual Onloader view에 조작 로그 저장 함수
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

            ckb1_Clamp.Checked = CIO.It.Get_Y(eY.ONL_ClampOn);
            ckb1_Pusher.Checked = CIO.It.Get_Y(eY.ONL_PusherForward);
            ckb1_TopRun.Checked = CIO.It.Get_Y(eY.ONL_TopBeltRun);
            ckb1_CCW.Checked = CIO.It.Get_Y(eY.ONL_BtmBeltCCW);
            ckb1_BtmRun.Checked = CIO.It.Get_Y(eY.ONL_BtmBeltRun);

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

        private void btnMan_Cycle_Click(object sender, EventArgs e)
        {
            //190627 ksg : 
            _EnableBtn(false);
            CSQ_Man.It.bBtnShow = false;

            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);
            //190226 ksg :추가

            // 2022.04.25 SungTae Start : [추가] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag
            if (CData.Opt.bSecsUse) { CData.bChkManual = true; }
            else                    { CData.bChkManual = false; }
            // 2021.04.25 SungTae End

            _SetLog(string.Format("N, {0} click", ESeq.ToString()));
            //190414 ksg :
            if (!((ESeq == ESeq.All_Home) || (ESeq == ESeq.All_Servo_On) || (ESeq == ESeq.All_Servo_Off) || (ESeq == ESeq.ONL_Home) ||
                  (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home    ) || (ESeq == ESeq.GRL_Home     ) || (ESeq == ESeq.GRR_Home) ||
                  (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home    ) || (ESeq == ESeq.OFL_Home     )                              ))
            {
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }

            //210730 pjh : Manual 동작 시 Strip In detect Check
            if(!CIO.It.Get_X(eX.INR_StripInDetect))
            {
                CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                _SetLog("Error : Strip-out detect.");
                return;
            }
            //


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

            CData.bChkManual = false;   // 2022.04.25 SungTae : [추가] Manual 동작 시 SECS/GEM Data를 Host에 보고하지 않기 위해 추가한 Flag 초기화
        }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {            
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            string sTxt;

            switch (iTag)
            {
                case 0:    // Clamp
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("MAGAZINE") + "\r\n" + CLang.It.GetLanguage("CLAMP");
                        else
                            sTxt = CLang.It.GetLanguage("MAGAZINE") + "\r\n" + CLang.It.GetLanguage("UNCLAMP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 1:    // Pusher
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("PUSHER") + "\r\n" + CLang.It.GetLanguage("FORWARD");
                        else
                            sTxt = CLang.It.GetLanguage("PUSHER") + "\r\n" + CLang.It.GetLanguage("BACKWARD");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 2:    // Top belt run
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("RUN");
                        else
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("STOP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 3:    // Bottom belt run
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("RUN");
                        else
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("STOP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 4:    // Bottom belt CCW
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("CCW");
                        else
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("CW");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
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
                case 0:    // Clamp
                    CSq_OnL.It.Func_MgzClamp(mCkb.Checked);
                    break;

                case 1:    // Pusher
                    CSq_OnL.It.Func_PusherForward(mCkb.Checked);
                    break;

                case 2:    // Top belt run
                    CSq_OnL.It.Func_TopBeltRun(mCkb.Checked);
                    break;

                case 3:    // Bottom belt run
                    CSq_OnL.It.Func_BtmBeltRun(mCkb.Checked, ckb1_CCW.Checked);
                    break;

                case 4:    // Bottom belt CCW
                    CIO.It.Set_Y(eY.ONL_BtmBeltCCW, mCkb.Checked);
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
                //mLbl.Text = sVal + " On";
                text = sVal + " " + CLang.It.GetLanguage("On");
                SetWriteLanguage(mLbl.Name, text);
            }
            else
            {
                //mLbl.Text = sVal + " Off";
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
                //mLbl.Text = sVal + " Off";
                text = sVal + " " + CLang.It.GetLanguage("Off");
                SetWriteLanguage(mLbl.Name, text);
            }
            else
            {
                //mLbl.Text = sVal + " On";
                text = sVal + " " + CLang.It.GetLanguage("On");
                SetWriteLanguage(mLbl.Name, text);
            }
        }


    }
}
