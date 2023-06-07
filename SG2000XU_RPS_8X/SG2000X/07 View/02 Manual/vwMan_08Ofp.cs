using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_08Ofp : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_08Ofp()
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

            // 2022.07.04 lhs Start
            btn_CoverDryer.Visible      = CDataOption.UseDryWtNozzle;
            btn_IV_DryTable.Visible     = CDataOption.UseOffP_IOIV;
            
            bool bOffP_IOIV = CDataOption.UseDryWtNozzle || CDataOption.UseOffP_IOIV;  
            btn_IV_DryClamp.Visible     = bOffP_IOIV;
            ckbOfP_IV_Trigger.Visible   = bOffP_IOIV;
            lblOfP_IV2_OK.Visible       = bOffP_IOIV;
            lblOfP_IV2_Busy.Visible     = bOffP_IOIV;
            lblOfP_IV2_Error.Visible    = bOffP_IOIV;

            if (bOffP_IOIV)
            {
                int nProgNum = CSq_OfP.It.Func_GetIVProgram();

                string sProgNum = nProgNum.ToString("000");
                lblOfP_IV2_OK.Text      = "IV2(P" + sProgNum + ") All OK\r\nOff";
                lblOfP_IV2_Busy.Text    = "IV2(P" + sProgNum + ") Busy\r\nOff";
                lblOfP_IV2_Error.Text   = "IV2(P" + sProgNum + ") Error\r\nOff";
            }
            // 2022.07.04 lhs End

            // 211006 syc : 2004U
            if (CDataOption.Use2004U)
            {
                lblOfP_LdCell.Visible           = false; // 2004U LoadCell 없음

                lblOfP_ClampOpen.Visible        = true;
                lblOfP_ClampClose.Visible       = true;
                lblOfP_CarrierDetect.Visible    = true;

                btnOfP_Unit_Check_IV2.Visible   = true;
                btnOfP_Cover_Check_IV2.Visible  = true;
                btnOfP_Cover_Place.Visible      = true;
                btnOfP_Cover_Pick.Visible       = true;

                ckbOfP_Clamp.Visible            = true;
            }
            // syc end

            // 2022.03.10 lhs Start : Spray Nozzle Btm Cleaner 사용시 채널이 중복되어 표시 안되도록 처리
            if (CDataOption.UseSprayBtmCleaner)
            {
                if (eX.OFFP_LoadCell == eX.DRY_ClnBtmNzlForward)
                {
                    lblOfP_LdCell.Visible = false;
                }
            }
            // 2022.03.10 lhs End
            // 2022.07.28 lhs Start
            bool bBrushCln = CDataOption.UseBrushBtmCleaner;
            btn_StripNzlCyl_N1.Visible = bBrushCln;
            btn_StripNzlCyl_N2.Visible = bBrushCln;
            btn_BtmClnBrush.Visible    = bBrushCln;
            // 2022.07.28 lhs End

            lblOfP_VacFree.Visible = CDataOption.UseOffPVacuumFree;  // 2022.06.16 lhs 

        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            int iAx1 = (int)EAx.OffLoaderPicker_X;
            int iAx2 = (int)EAx.OffLoaderPicker_Z;

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // X
                if (CMot.It.Chk_Alr(iAx1)) { lblOfp_XStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx1) == EAxOp.Idle)
                { lblOfp_XStat.BackColor = Color.LightGray; }
                else { lblOfp_XStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lblOfp_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lblOfp_XPos.Text = CMot.It.Get_FP(iAx1).ToString("0.000");
                }
                else
                {
                    lblOfp_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Z
                if (CMot.It.Chk_Alr(iAx2)) { lblOfp_ZStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx2) == EAxOp.Idle)
                { lblOfp_ZStat.BackColor = Color.LightGray; }
                else { lblOfp_ZStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lblOfp_ZPos.Text = CMot.It.Get_FP(iAx2).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lblOfp_ZPos.Text = CMot.It.Get_FP(iAx2).ToString("0.000");
                }
                else
                {
                    lblOfp_ZPos.Text = CMot.It.Get_FP(iAx2).ToString();
                }
                // 2020.11.27 JSKim Ed

                bool bVac = (CDataOption.IsRevPicker) ? !CIO.It.Get_Y(eY.OFFP_Vacuum1) && !CIO.It.Get_Y(eY.OFFP_Vacuum2) :
                                                        CIO.It.Get_Y(eY.OFFP_Vacuum1) && CIO.It.Get_Y(eY.OFFP_Vacuum2);
                ckbOfP_Vac.Checked  = bVac;
                ckbOfP_Ejt.Checked  = CIO.It.Get_Y(eY.OFFP_Ejector);
                ckbOfP_Drn.Checked  = CIO.It.Get_Y(eY.OFFP_Drain);
                ckbOfP_R0.Checked   = CIO.It.Get_Y(eY.OFFP_Rotate0);
            }

            lblOfP_R0.BackColor         = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Rotate0]];
            lblOfP_R90.BackColor        = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Rotate90]];
            lblOfP_Vac.BackColor        = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Vacuum]];
            lblOfP_VacFree.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_VacuumFree]];     // 2022.05.24 lhs
            lblOfP_LdCell.BackColor     = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_LoadCell]];
            
            //210929 syc : 2004U
            lblOfP_ClampClose.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Carrier_Clamp_Close]];
            lblOfP_ClampOpen.BackColor     = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Carrier_Clamp_Open]];
            lblOfP_CarrierDetect.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Picker_Carrier_Detect]];
            //

            // 2022.07.04 lhs Start
            int nProgNum = CSq_OfP.It.Func_GetIVProgram();

            string sProgNum = nProgNum.ToString("000");
            lblOfP_IV2_OK.Text = "IV2(P" + sProgNum + ") All OK\r\nOff";
            lblOfP_IV2_Busy.Text = "IV2(P" + sProgNum + ") Busy\r\nOff";
            lblOfP_IV2_Error.Text = "IV2(P" + sProgNum + ") Error\r\nOff";
            
            lblOfP_IV2_OK.BackColor     = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_AllOK]];
            lblOfP_IV2_Busy.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_Busy]];
            lblOfP_IV2_Error.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_Error]];

            if (CSq_OfP.It.m_nStripNzlCycle == 0)
            {
                btn_StripNzlCyl_N1.BackColor = Color.Lime;
                btn_StripNzlCyl_N2.BackColor = Color.White;
            }
            else
            {
                btn_StripNzlCyl_N1.BackColor = Color.White;
                btn_StripNzlCyl_N2.BackColor = Color.Lime;
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

        private void _EnableBtn(bool bVal)
        {
            btn_H.Enabled           = bVal;
            btn_Wait.Enabled        = bVal;
            btn_PickL.Enabled       = bVal;
            btn_PickR.Enabled       = bVal;
            btn_PlaceR.Enabled      = bVal; // 20200415 jhc : Picker Idle Time 개선
            btn_Place.Enabled       = bVal;
            btn_IV_DryClamp.Enabled = bVal; // 2021.04.16 lhs
            btn_IV_DryTable.Enabled = bVal; // 2022.07.07 lhs
            btn_CoverDryer.Enabled  = bVal; // 2021.04.16 lhs
            btn_PickCl.Enabled      = bVal;
            btn_StripCl.Enabled     = bVal;
            btn_BtmClnBrush.Enabled = bVal; // 2022.07.28 lhs
            ckbOfP_Vac.Enabled      = bVal;
            ckbOfP_Ejt.Enabled      = bVal;
            ckbOfP_Drn.Enabled      = bVal;
            ckbOfP_R0.Enabled       = bVal;

            // 2021.04.19 SungTae Start
            btn_PickerXWait.Enabled         = bVal;
            btn_PickerXDryPlace.Enabled     = bVal;
            btn_PickerXLeftPick.Enabled     = bVal;
            btn_PickerXRightPick.Enabled    = bVal;
            btn_PickerXPickerClnSt.Enabled  = bVal;
            btn_PickerXPickerClnEd.Enabled  = bVal;
            btn_PickerXStripClnSt.Enabled   = bVal;
            btn_PickerXStripClnEd.Enabled   = bVal;

            btn_PickerZWait.Enabled         = bVal;
            btn_PickerZDryPlace.Enabled     = bVal;
            btn_PickerZTblPickPlace.Enabled = bVal;
            btn_PickerZClnPicker.Enabled    = bVal;
            btn_PickerZClnStrip.Enabled     = bVal;
            // 2021.04.19 SungTae End

            // 211008 syc : 2004U
            btnOfP_Unit_Check_IV2   .Enabled = bVal;
            btnOfP_Cover_Check_IV2  .Enabled = bVal;
            btnOfP_Cover_Place      .Enabled = bVal;
            btnOfP_Cover_Pick       .Enabled = bVal;
            // syc end
        }

        /// <summary>
        /// Manual offloader picker view에 조작 로그 저장 함수
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
            //20200415 jhc : Picker Idle Time 개선
            if(CDataOption.PickerImprove != ePickerTimeImprove.Use)
            {
                btn_PlaceR.Visible = false;
            }

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            bool bVac = (CDataOption.IsRevPicker) ? !CIO.It.Get_Y(eY.OFFP_Vacuum1) && !CIO.It.Get_Y(eY.OFFP_Vacuum2) :
                                                        CIO.It.Get_Y(eY.OFFP_Vacuum1) && CIO.It.Get_Y(eY.OFFP_Vacuum2);
            ckbOfP_Vac.Checked = bVac;
            ckbOfP_Ejt.Checked = CIO.It.Get_Y(eY.OFFP_Ejector);
            ckbOfP_Drn.Checked = CIO.It.Get_Y(eY.OFFP_Drain);
            ckbOfP_R0.Checked = CIO.It.Get_Y(eY.OFFP_Rotate0);

            // 2021.04.15 SungTae Start : 
            if (CData.CurCompany == ECompany.ASE_KR)    { tab_CtrlOFP.Visible = true; }
            else                                        { tab_CtrlOFP.Visible = false; }

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
        }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            string sTxt;

            switch (iTag)
            {
                case 0:    // Vacuum
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("VACUUM") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("VACUUM") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                case 1:    // Ejector
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("EJECTOR") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("EJECTOR") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                case 2:    // Drain
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("DRAIN") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("DRAIN") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                case 3:    // Rotate 0
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("ROTATE") + "\r\n0º";
                        else                sTxt = CLang.It.GetLanguage("ROTATE") + "\r\n90º";
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                    //2022.07.19 lhs Start
                case 4:    // IV2 Trigger Out
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("IV") + "\r\n" + CLang.It.GetLanguage("TRIGGER") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("IV") + "\r\n" + CLang.It.GetLanguage("TRIGGER") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                    //2022.07.19 lhs End

                case 6:    // Carrier Calmp  210930 syc : 2004U 
                    {
                        if (mCkb.Checked) sTxt = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("CLAMP") + "\r\n" + CLang.It.GetLanguage("CLOSE");
                        else sTxt              = CLang.It.GetLanguage("PICKER") + "\r\n" + CLang.It.GetLanguage("CLAMP") + "\r\n" + CLang.It.GetLanguage("OPEN");
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
                        CIO.It.Set_Y(eY.OFFP_Ejector, false);
                        CIO.It.Set_Y(eY.OFFP_Drain, false);
                        CSq_OfP.It.Func_Vacuum(mCkb.Checked);
                        break;
                    }

                case 1:    // Ejector
                    {
                        CSq_OfP.It.Func_Vacuum(false);
                        CIO.It.Set_Y(eY.OFFP_Drain, false);
                        CIO.It.Set_Y(eY.OFFP_Ejector, mCkb.Checked);
                        break;
                    }

                case 2:    // Drain
                    {
                        if (mCkb.Checked)
                        {
                            CSq_OfP.It.Func_Vacuum(false);
                            CIO.It.Set_Y(eY.OFFP_Ejector, false);
                        }
                        CIO.It.Set_Y(eY.OFFP_Drain, mCkb.Checked);
                        break;
                    }

                case 3:    // Rotate 0
                    {
                        if (mCkb.Checked)
                        {//20200113 myk_OFP_Rotate_InterLock
                            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait)
                            {
                                CMsg.Show(eMsg.Error, "Error", "Check the Offloader Picker Z Axis Position");
                                return;
                            }
                            else if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z) != 1)
                            {
                                CMsg.Show(eMsg.Error, "Error", "First Offloader Picker Z Axis Home Please");
                                return;
                            }
                            else
                            { CSq_OfP.It.Func_Picker0(); }
                        }
                        else
                        {
                            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait)
                            {
                                CMsg.Show(eMsg.Error, "Error", "Check the Offloader Picker Z Axis Position");
                                return;
                            }
                            else if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z) != 1)
                            {
                                CMsg.Show(eMsg.Error, "Error", "First Offloader Picker Z Axis Home Please");
                                return;
                            }
                            else
                            { CSq_OfP.It.Func_Picker90(); }
                        }
                        break;
                    }

                    // 2022.07.19 lhs Start
                case 4:    // IV2 Trigger Out  : program 번호와 무관하게
                    {
                        System.Threading.Thread.Sleep(500);

                        if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
                        {
                            _SetLog("Error : IV is Error");
                        }
                        if (CIO.It.Get_X(eX.OFFP_IV2_Busy))  // Busy 확인
                        {
                            _SetLog("IV2 is busy");
                        }

                        // IV2의 외부 트리거
                        // 사양은 ON : Min 100us, OFF : Min 1.2ms
                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, true);
                        _SetLog("IV Trigger Out = ON");

                        System.Threading.Thread.Sleep(100);

                        CIO.It.Set_Y(eY.OFFP_IV2_Trigger, false);
                        _SetLog("IV Trigger Out = OFF");

                        mCkb.Checked = false;
                        break;
                    }
                    // 2022.07.19 lhs End

                case 6: // Clamp On/Off    //210929 syc : 2004U 
                    {
                        if (mCkb.Checked) //Calmp on
                        {
                            CSq_OfP.It.Func_CarrierClampClose();                           
                        }
                        else //Calmp Off
                        {
                            CSq_OfP.It.Func_CarrierClampOpen();
                        }
                        break;
                    }
                    //syc end                    
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
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker90())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 90 degrees. First, turn the picker 90 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_Wait + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.SPos.dOFP_X_Wait - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in wait position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Wait);
        }

        private void btn_PickerXDryPlace_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker90())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 90 degrees. First, turn the picker 90 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_Place + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.SPos.dOFP_X_Place - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in dry place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_Place);
        }

        private void btn_PickerXLeftPick_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickL + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.SPos.dOFP_X_PickL - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in left table pick position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_PickL);
        }

        private void btn_PickerXRightPick_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickR + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.SPos.dOFP_X_PickR - 0.01D )
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in right table pick & place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.SPos.dOFP_X_PickR);
        }

        private void btn_PickerXPickerClnSt_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.Dev.dOffP_X_ClnStart + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.Dev.dOffP_X_ClnStart - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in picker cleaning start position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.Dev.dOffP_X_ClnStart);
        }

        private void btn_PickerXPickerClnEd_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.Dev.dOffP_X_ClnEnd + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.Dev.dOffP_X_ClnEnd - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in picker cleaning end position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.Dev.dOffP_X_ClnEnd);
        }

        private void btn_PickerXStripClnSt_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.Dev.dOffP_X_ClnStart + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.Dev.dOffP_X_ClnStart - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in strip cleaning start position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.Dev.dOffP_X_ClnStart);
        }

        private void btn_PickerXStripClnEd_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is not wait position. First, move the offloader picker Z-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.Dev.dOffP_X_ClnEnd + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.Dev.dOffP_X_ClnEnd - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker X-axis is already in strip cleaning end position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_X, CData.Dev.dOffP_X_ClnEnd);
        }

        private void btn_PickerZWait_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) < CData.SPos.dOFP_Z_Wait + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker Z-axis is already in wait position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.SPos.dOFP_Z_Wait);
        }

        private void btn_PickerZTblPickPlace_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_X) >= CData.SPos.dONP_X_PlaceL - 150.0D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The onloader picker X-axis is not safety position. First, move the onloader picker X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.SPos.dOFP_X_PickL + 0.01D ||
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.SPos.dOFP_X_PickR - 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker X-axis is not table pick or place position. First, move the offloader picker X-axis to table pick or place position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.LeftGrindZone_Y)  > CData.SPos.dGRD_Y_Wait[(int)EWay.L] + 0.005D ||
                CMot.It.Get_FP((int)EAx.RightGrindZone_Y) > CData.SPos.dGRD_Y_Wait[(int)EWay.R] + 0.005D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The left or right grind Y-axis is not wait position. First, move the left or right grind Y-axis to wait position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) < CData.Dev.dOffP_Z_Pick + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.Dev.dOffP_Z_Pick - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker Z-axis is already in pick/place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.Dev.dOffP_Z_Pick);
        }

        private void btn_PickerZClnPicker_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.Dev.dOffP_X_ClnStart + 0.01D ||
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.Dev.dOffP_X_ClnEnd - 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker X-axis is not picker cleaning position. First, move the offloader picker X-axis to picker cleaning position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) < CData.Dev.dOffP_Z_ClnStart + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.Dev.dOffP_Z_ClnStart - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker Z-axis is already in picker cleaning position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.Dev.dOffP_Z_ClnStart);
        }

        private void btn_PickerZClnStrip_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.Dev.dOffP_X_ClnStart + 0.01D ||
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) < CData.Dev.dOffP_X_ClnEnd - 0.01D )
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker X-axis is not strip cleaning position. First, move the offloader picker X-axis to strip cleaning position.");
                return;
            }

            if (!CSq_OfP.It.Func_Picker0())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 0 degrees. First, turn the picker 0 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) < CData.Dev.dOffP_Z_ClnStart + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.Dev.dOffP_Z_ClnStart - 0.01D)
            {
                CMsg.Show(eMsg.Notice, "Notice", "The offloader picker Z-axis is already in strip cleaning position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.Dev.dOffP_Z_StripClnStart);
        }

        private void btn_PickerZDryPlace_Click(object sender, EventArgs e)
        {
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_X) > CData.SPos.dOFP_X_Place + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker X-axis is not place position. First, move the offloader picker X-axis to dry place position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.DryZone_X) > CData.SPos.dDRY_X_Wait + 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The dry X-axis is not wait position. First, move the dry X-axis to wait position.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.DryZone_Z) < CData.SPos.dDRY_Z_Up - 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The dry Z-axis is not up position. First, move the dry Z-axis to up position.");
                return;
            }

            if(!CSq_OfP.It.Func_Picker90())
            {
                CMsg.Show(eMsg.Warning, "Warning", "The picker is not turned 90 degrees. First, turn the picker 90 degrees.");
                return;
            }

            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) < CData.Dev.dOffP_Z_Place + 0.01D &&
                CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.Dev.dOffP_Z_Place - 0.01D)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The offloader picker Z-axis is already in dry place position.");
                return;
            }

            CMot.It.Mv_N((int)EAx.OffLoaderPicker_Z, CData.Dev.dOffP_Z_Place);
        }

		private void btn_StripNzlCyl_N1_Click(object sender, EventArgs e)
		{
            CSq_OfP.It.m_nStripNzlCycle = 0;    // #1 Count 적용 (Device)
        }

		private void btn_StripNzlCyl_N2_Click(object sender, EventArgs e)
		{
            CSq_OfP.It.m_nStripNzlCycle = 1;    // #2 Count 적용 (Device)
        }		
	}
}
