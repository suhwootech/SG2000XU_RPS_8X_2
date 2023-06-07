using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_04Onp : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_04Onp()
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

            //20190611 ghk_onpclean
            if (CDataOption.eOnpClean == eOnpClean.Use)
            {
                btnOnP_Clean.Visible = true;
                ckbOnP_Clean.Visible = true;
                lblOnP_CleanL.Visible = true;
                lblOnP_CleanR.Visible = true;
            }
            else
            {
                btnOnP_Clean.Visible = false;
                ckbOnP_Clean.Visible = false;
                lblOnP_CleanL.Visible = false;
                lblOnP_CleanR.Visible = false;
            }
            //
            //20190626 ghk_OnpBcr
            if (CDataOption.CurEqu == eEquType.Nomal)
            {
                btnOnp_Bcr.Visible = false;
                btnOnp_Ori.Visible = false;
                pnlOnp_Bcr.Visible = false;
            }
            //211006 syc : 2004U 
            if (CDataOption.Use2004U)
            {
                btnOnP_Unit_Check_IV2.Visible = true;
                lblOnP_LdCell.Visible = false; //2004U LoadCell 없음
            }

            lblOnP_VacFree.Visible = CDataOption.UseOnPVacuumFree;    // 2022.06.16 lhs : Vacuum Free를 사용하면

        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            int iAx1 = (int)EAx.OnLoaderPicker_X;
            int iAx2 = (int)EAx.OnLoaderPicker_Z;
            int iAx3 = (int)EAx.OnLoaderPicker_Y;

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // X
                if (CMot.It.Chk_Alr(iAx1)) { lblOnp_XStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx1) == EAxOp.Idle)
                { lblOnp_XStat.BackColor = Color.LightGray; }
                else { lblOnp_XStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lblOnp_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lblOnp_XPos.Text = CMot.It.Get_FP(iAx1).ToString("0.000");
                }
                else
                {
                    lblOnp_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Z
                if (CMot.It.Chk_Alr(iAx2)) { lblOnp_ZStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx2) == EAxOp.Idle)
                { lblOnp_ZStat.BackColor = Color.LightGray; }
                else { lblOnp_ZStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lblOnp_ZPos.Text = CMot.It.Get_FP(iAx2).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lblOnp_ZPos.Text = CMot.It.Get_FP(iAx2).ToString("0.000");
                }
                else
                {
                    lblOnp_ZPos.Text = CMot.It.Get_FP(iAx2).ToString();
                }
                // 2020.11.27 JSKim Ed

                if (CDataOption.CurEqu == eEquType.Pikcer)
                {
                    // 20190604 ghk_onpbcr
                    // Y
                    if (CMot.It.Chk_Alr(iAx3)) { lblOnp_YStat.BackColor = Color.Red; }
                    if (CMot.It.Chk_OP(iAx3) == EAxOp.Idle)
                    { lblOnp_YStat.BackColor = Color.LightGray; }
                    else { lblOnp_YStat.BackColor = Color.Lime; }
                    // 2020.11.27 JSKim St
                    //lblOnp_YPos.Text = CMot.It.Get_FP(iAx3).ToString();
                    if (CData.CurCompany == ECompany.JCET)
                    {
                        lblOnp_YPos.Text = CMot.It.Get_FP(iAx3).ToString("0.000");
                    }
                    else
                    {
                        lblOnp_YPos.Text = CMot.It.Get_FP(iAx3).ToString();
                    }
                    // 2020.11.27 JSKim Ed
                }

                ckbOnP_R0.Checked = CIO.It.Get_Y(eY.ONP_Rotate0);
                bool bVac = (CDataOption.IsRevPicker) ? !CIO.It.Get_Y(eY.ONP_Vacuum1) && !CIO.It.Get_Y(eY.ONP_Vacuum2) :
                                                        CIO.It.Get_Y(eY.ONP_Vacuum1) && CIO.It.Get_Y(eY.ONP_Vacuum2);
                ckbOnP_Vac.Checked = bVac;
                ckbOnP_Ejt.Checked = CIO.It.Get_Y(eY.ONP_Ejector);
                ckbOnP_Drn.Checked = CIO.It.Get_Y(eY.ONP_Drain);
                ckbOnP_Clean.Checked = CIO.It.Get_Y(eY.INR_OnpCleaner);
            }

            lblOnP_R0.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Rotate0]];
            lblOnP_R90.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Rotate90]];
            lblOnP_Vac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Vacuum]];
            lblOnP_VacFree.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_VacuumFree]];     // 2022.05.24 lhs
            lblOnP_LdCell.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_LoadCell]];
            //20190628 ghk_onpclean
            lblOnP_CleanL.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_OnpCleaner_L]];
            lblOnP_CleanL.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_OnpCleaner_R]];

            lblOnp_Bcr.Text = CData.Bcr.sBcr; //190709 ksg : 이동

            // 2021.04.15 SungTae Start
            if (CData.CurCompany == ECompany.ASE_KR)    { tab_CtrlONP.Visible = true; }
            else                                        { tab_CtrlONP.Visible = false; }
            // 2021.04.15 SungTae End
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
            btnOnp_Bcr.Enabled = bVal;
            btnOnp_Ori.Enabled = bVal;
            btnOnP_H.Enabled = bVal;
            btnOnP_Wait.Enabled = bVal;
            btnOnP_PickRail.Enabled = bVal;
            btnOnP_PickLTbl.Enabled = bVal;
            btnOnP_PlaceL.Enabled = bVal;
            btnOnP_PlaceR.Enabled = bVal;
            ckbOnP_Vac.Enabled = bVal;
            ckbOnP_Ejt.Enabled = bVal;
            ckbOnP_Drn.Enabled = bVal;
            ckbOnP_R0.Enabled = bVal;
            //20190611 ghk_onpclean
            btnOnP_Clean.Enabled = bVal;
            ckbOnP_Clean.Enabled = bVal;
            btnOnP_BcrOri.Enabled = bVal; //191110 ksg :

            // 2021.04.19 SungTae Start
            btn_PickerXWait.Enabled         = bVal;
            btn_PickerXRailPick.Enabled     = bVal;
            btn_PickerXLeftPlace.Enabled    = bVal;
            btn_PickerXRightPlace.Enabled   = bVal;

            btn_PickerZWait.Enabled         = bVal;
            btn_PickerZRailPick.Enabled     = bVal;
            btn_PickerZTblPickPlace.Enabled = bVal;
            // 2021.04.19 SungTae End

            // 211008 syc : 2004U
            btnOnP_Unit_Check_IV2.Enabled = bVal;
            // syc end
        }

        /// <summary>
        /// Manual onloader picker view에 조작 로그 저장 함수
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

            ckbOnP_R0.Checked = CIO.It.Get_Y(eY.ONP_Rotate0);
            bool bVac = (CDataOption.IsRevPicker) ? !CIO.It.Get_Y(eY.ONP_Vacuum1) && !CIO.It.Get_Y(eY.ONP_Vacuum2) :
                                                        CIO.It.Get_Y(eY.ONP_Vacuum1) && CIO.It.Get_Y(eY.ONP_Vacuum2);
            ckbOnP_Vac.Checked = bVac;
            ckbOnP_Ejt.Checked = CIO.It.Get_Y(eY.ONP_Ejector);
            ckbOnP_Drn.Checked = CIO.It.Get_Y(eY.ONP_Drain);
            //20190628 ghk_onpclean
            ckbOnP_Clean.Checked = CIO.It.Get_Y(eY.INR_OnpCleaner);
            
            // 20190604 ghk_onpbcr
            if (CDataOption.CurEqu == eEquType.Pikcer)
            { pnlOnp_Y.Visible = true; }

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

            _SetLog(string.Format("N, {0} click", ESeq));
            //190414 ksg :
            if (!((ESeq == ESeq.All_Home) || (ESeq == ESeq.All_Servo_On) || (ESeq == ESeq.All_Servo_Off) || (ESeq == ESeq.ONL_Home) ||
                 (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home) || (ESeq == ESeq.GRL_Home) || (ESeq == ESeq.GRR_Home) ||
                 (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home) || (ESeq == ESeq.OFL_Home)))
            {
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }


            //190211 ksg :
            if (CDataOption.CurEqu != eEquType.Nomal)
            {//20190604 ghk_onpbcr
                //if (ESeq == ESeq.ONP_CheckBcr || ESeq == ESeq.ONP_CheckOri) 
                if (ESeq == ESeq.ONP_CheckBcr || ESeq == ESeq.ONP_CheckOri || ESeq == ESeq.ONP_CheckBcrOri) 
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

            if (CDataOption.CurEqu != eEquType.Nomal && ESeq == ESeq.ONP_CheckBcrOri)
            {
                if(CData.Dev.bBcrSkip || CData.Dev.bOriSkip)
                {
                     CSQ_Man.It.bBtnShow = true; //190627 ksg :
                     CMsg.Show(eMsg.Error, "Error", "First Bcr & Orientation Option check Please");
                     return;
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

                //---- 2022.08.19 lhs Start
                if (CDataOption.Use2004U)
                {
                    if (ESeq != ESeq.ONP_Wait) // Wait 버튼이 아닌 다른 버튼 클릭할 경우는 flag 리셋. 
                    {
                        CSq_OnP.It.m_bRetryPickTbL = false;
                    }
                }
                //---- 2022.08.19 lhs End
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
        }

        /// <summary>
        /// 
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
                case 0:    // Vacuum
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("VACUUM") + "\r\n" + CLang.It.GetLanguage("ON");
                    else
                        sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("VACUUM") + "\r\n" + CLang.It.GetLanguage("OFF");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 1:    // Ejector          
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("EJECTOR") + "\r\n" + CLang.It.GetLanguage("ON");
                    else
                        sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("EJECTOR") + "\r\n" + CLang.It.GetLanguage("OFF");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 2:    // Drain
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("DRAIN") + "\r\n" + CLang.It.GetLanguage("ON");
                    else
                        sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("DRAIN") + "\r\n" + CLang.It.GetLanguage("OFF");

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 3:    // Rotate 0
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("ROTATE") + "\r\n0º";
                    else
                        sTxt = CLang.It.GetLanguage("ROTATE") + "\r\n90º";

                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 4:
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("CLEANER") + "\r\nLEFT";
                        else
                            sTxt = CLang.It.GetLanguage("CLEANER") + "\r\nRIGHT";

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
                case 0:    // Vacuum
                    {
                        CSq_OnP.It.Func_Eject(false);
                        CSq_OnP.It.Func_Vacuum(mCkb.Checked);
                        break;
                    }

                case 1:    // Ejector  
                    {
                        CSq_OnP.It.Func_Vacuum(false);
                        CSq_OnP.It.Func_Eject(mCkb.Checked);
                        break;
                    }

                case 2:    // Drain
                    {
                        if (mCkb.Checked)
                        {
                            CSq_OnP.It.Func_Vacuum(false);
                            CSq_OnP.It.Func_Eject(false);
                        }
                        CSq_OnP.It.Func_Drain(mCkb.Checked);
                        break;
                    }

                case 3:    // Rotate 0
                    {
                        if (mCkb.Checked)
                        {//20200113 myk_ONP_Rotate_interLock
                            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait)
                            {
                                CMsg.Show(eMsg.Error, "Error", "Check the Onloader Picker Z Axis Position");
                                return;
                            }
                            else if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z) != 1)
                            {
                                CMsg.Show(eMsg.Error, "Error", "First Onloader Picker Z Axis Home Please");
                                return;
                            }
                            else
                            {
                                CSq_OnP.It.Func_Picker0();
                            }
                        }
                        else
                        {
                            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait)
                            {
                                CMsg.Show(eMsg.Error, "Error", "Check the Onloader Picker Z Axis Position");
                                return;
                            }
                            else if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z) != 1)
                            {
                                CMsg.Show(eMsg.Error, "Error", "First Onloader Picker Z Axis Home Please");
                                return;
                            }
                            else
                            {
                                CSq_OnP.It.Func_Picker90();
                            }
                        }
                        break;
                    }

                //20190628 ghk_onpclean
                case 4:
                    {
                        if (mCkb.Checked)
                        { CSq_OnP.It.Func_CleanerLeft(); }
                        else
                        { CSq_OnP.It.Func_CleanerRight(); }
                        break;
                    }     
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

        private void btn_PickerXWait_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker Z-axis is not wait position. First, move the onloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OnP.It.Func_Picker90())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 90 degrees. First, turn the picker 90 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) < CData.Dev.dOnP_X_Wait + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) > CData.Dev.dOnP_X_Wait - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker X-axis is already in wait position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_X, CData.Dev.dOnP_X_Wait);
        }

        private void btn_PickerXRailPick_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker Z-axis is not wait position. First, move the onloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OnP.It.Func_Picker90())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 90 degrees. First, turn the picker 90 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) < CData.Dev.dOnP_X_Wait + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) > CData.Dev.dOnP_X_Wait - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker X-axis is already in rail pick position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_X, CData.Dev.dOnP_X_Wait);
        }

        private void btn_PickerXLeftPlace_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) >= CData.Dev.dOffP_X_ClnStart)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker X-axis is not safety position. First, move the offloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker Z-axis is not wait position. First, move the onloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OnP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) < CData.SPos.dONP_X_PlaceL + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) > CData.SPos.dONP_X_PlaceL - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker X-axis is already in left table pick/place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_X, CData.SPos.dONP_X_PlaceL);
        }

        private void btn_PickerXRightPlace_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) >= CData.Dev.dOffP_X_ClnStart + 0.005D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker X-axis is not safety position. First, move the offloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker Z-axis is not wait position. First, move the onloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OnP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) < CData.SPos.dONP_X_PlaceR + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) > CData.SPos.dONP_X_PlaceR - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker X-axis is already in right table place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_X, CData.SPos.dONP_X_PlaceR);
        }

        private void btn_PickerZWait_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) < CData.SPos.dONP_Z_Wait + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker Z-axis is already in wait position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_Z, CData.SPos.dONP_Z_Wait);
        }

        private void btn_PickerZRailPick_Click(object sender, EventArgs e)
        {
            if (!CSq_Inr.It.Act_ZLiftDU(false))
            {
                CMsg.Show(eMsg.Warning, "Warning", "The inrail lift is not down. First, take the lift down.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.Inrail_X) > CData.SPos.dINR_X_Wait)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The in-rail X-axis is not wait position. First, move the in-rail X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.Inrail_Y) > CData.SPos.dINR_Y_Align)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The in-rail Y-axis is not align position. First, move the in-rail Y-axis to align position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) > CData.Dev.dOnP_X_Wait + 0.005D ||
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) < CData.Dev.dOnP_X_Wait - 0.005D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not rail pick position. First, move the onloader picker X-axis to rail pick position.");
                return;
            }

            if (!CSq_OnP.It.Func_Picker90())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 90 degrees. First, turn the picker 90 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) < CData.Dev.dOnP_Z_Pick + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.Dev.dOnP_Z_Pick - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker Z-axis is already in rail pick position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_Z, CData.Dev.dOnP_Z_Pick);
        }

        private void btn_PickerZTblPickPlace_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) < CData.SPos.dONP_X_PlaceL - 0.005D ||
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) > CData.SPos.dONP_X_PlaceR + 0.005D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not table place position. First, move the onloader picker X-axis to table place position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.LeftGrindZone_Y)  > CData.SPos.dGRD_Y_Wait[(int)EWay.L] + 0.005D ||
                CMot.It.Get_FP((int)EAx.RightGrindZone_Y) > CData.SPos.dGRD_Y_Wait[(int)EWay.R] + 0.005D) 
            {
                CMsg.Show(eMsg.Warning, "Warning", "The left or right grind Y-axis is not wait position. First, move the left or right grind Y-axis to wait position.");
                return;
            }

            if (!CSq_OnP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) < CData.Dev.dOnP_Z_Place + 0.01D &&
                CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.Dev.dOnP_Z_Place - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The onloader picker Z-axis is already in left/right table place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OnLoaderPicker_Z, CData.Dev.dOnP_Z_Place);
        }

		
	}
}
