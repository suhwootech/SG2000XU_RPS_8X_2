using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_09Dry : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_09Dry()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;


            //200121 ksg :
            if (CData.CurCompany == ECompany.SCK    ||  
                CData.CurCompany == ECompany.JSCK   ||
                CData.CurCompany == ECompany.JCET)  { ckbDry_IOOn.Visible = true;   }
            else                                    { ckbDry_IOOn.Visible = false;  }


            btn_WaterNozzle.Visible = false;  // 2022.03.03 lhs : 초기화

            // 2022.07.01 lhs Start : DryClamp Cylinder 사용시 표시
            bool bCylinder = CDataOption.eDryClamp == EDryClamp.Cylinder;
            ckb_ClampOpen.Visible       = bCylinder;
            btn_Z_Up_Pos.Visible        = bCylinder;    // 2022.07.18 lhs
            lbl_FrontClamp.Visible      = bCylinder;
            lbl_FrontUnClamp.Visible    = bCylinder;
            lbl_RearClamp.Visible       = bCylinder;
            lbl_RearUnClamp.Visible     = bCylinder;
            // 2022.07.01 lhs End

            // 2022.03.03 lhs Start : SprayBtmCleaner 사용시 표시
            bool bSprayCln = CDataOption.UseSprayBtmCleaner;
            ckbDry_BtmClnNzlMove.Visible        = bSprayCln;
            ckbDry_BtmClnAirBlow.Visible        = bSprayCln;
            lblDry_BtmClnNzlForward.Visible     = bSprayCln;
            lblDry_BtmClnNzlBackward.Visible    = bSprayCln;
            // 2022.03.03 lhs End

            // 2022.03.14 lhs Start : Dryer Level Safety Air Blow 사용시 표시
            bool bLevelAir = CDataOption.UseDryerLevelAirBlow;
            ckbDry_LevelAirBlow.Visible = bLevelAir;
            // 2022.03.14 lhs End

            // 200316 mjy : Dry UI 초기화
            if (CDataOption.Dryer == eDryer.Knife)
            {
                lbl_RStat.Text = "Y Axis";
                lbl_RUnit.Text = "Position [mm]";

                lbl_Lv1.Visible = false;
                lbl_Lv2.Visible = false;

                lbl_BtmStandby.Tag = "Cover Open$";
                lbl_BtmStandby.Text = "Cover Open\nOff";
                lbl_BtmTarget.Tag = "Cover Close$";
                lbl_BtmTarget.Text = "Cover Close\nOff";

                // 211001 syc : 2004U
                if (CDataOption.Use2004U)
                {                    
                    btn_CkSafty.Text = "[71]CHECK\nCOVER\nSENSOR";

                    lbl_Lv1.Visible = true;
                    lbl_Lv1.Text    = "Carrier Cover Detect";
                    lbl_Lv1.Tag     = "Carrier Cover Detect$";                    
                }
                else
                {
                    btn_CkSafty.Text = "[71]CHECK\nUNIT\nSENSOR";

                    lbl_Unit1.Visible = true;
                    lbl_Unit2.Visible = true;
                    lbl_Unit3.Visible = true;
                    lbl_Unit4.Visible = true;
                }
                // syc end
            }
            // 2021.04.16 lhs Start
            else  // Rotate
            {
                if (CDataOption.UseDryWtNozzle)
                {
                    btn_CkSafty.Visible     = false;
                    lbl_Lv1.Visible         = false;
                    lbl_Lv2.Visible         = false;

                    btn_WaterNozzle.Visible = true;
                    btn_WaterNozzle.Left    = btn_CkSafty.Left;
                    btn_WaterNozzle.Top     = btn_CkSafty.Top;

                    lbl_BtmStandby.Tag      = "Water Nozzle\nStandby Position";
                    lbl_BtmStandby.Text     = "Water Nozzle\nStandby Position Off";
                    lbl_BtmTarget.Tag       = "Water Nozzle\nTarget Position";
                    lbl_BtmTarget.Text      = "Water Nozzle\nTarget Position Off";

                    ckbDry_Btm.Text         = "WATER\nNOZZLE\nOFF";
                } 
                // 2021.04.16 lhs End
                else
                {
                    if(CDataOption.eDryClamp == EDryClamp.None) // 기존 Spring Type
                    {
                        //
                    }
                    else if(CDataOption.eDryClamp == EDryClamp.Cylinder)
                    {
                        btn_CkSafty.Visible = false;
                        lbl_Lv1.Visible     = false;
                        lbl_Lv2.Visible     = false;
                    }
                }
            }
            

        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            int iAx1 = (int)EAx.DryZone_X;
            int iAx2 = (int)EAx.DryZone_Z;
            int iAx3 = (int)EAx.DryZone_Air;

            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // X
                if (CMot.It.Chk_Alr(iAx1))              { lbl_XStat.BackColor = Color.Red;          }
                if (CMot.It.Chk_OP(iAx1) == EAxOp.Idle) { lbl_XStat.BackColor = Color.LightGray;    }
                else                                    { lbl_XStat.BackColor = Color.Lime;         }
                // R
                if (CMot.It.Chk_Alr(iAx2))              { lbl_ZStat.BackColor = Color.Red;          }
                if (CMot.It.Chk_OP(iAx2) == EAxOp.Idle) { lbl_ZStat.BackColor = Color.LightGray;    }
                else                                    { lbl_ZStat.BackColor = Color.Lime;         }
                // Z
                if (CMot.It.Chk_Alr(iAx3))              { lbl_RStat.BackColor = Color.Red;          }
                if (CMot.It.Chk_OP(iAx3) == EAxOp.Idle) { lbl_RStat.BackColor = Color.LightGray;    }
                else                                    { lbl_RStat.BackColor = Color.Lime;         }

                if (CData.CurCompany == ECompany.JCET)
                {
                    lblDry_XPos.Text = CMot.It.Get_FP(iAx1).ToString("0.000");
                    lblDry_ZPos.Text = CMot.It.Get_FP(iAx2).ToString("0.000");
                    lblDry_RPos.Text = CMot.It.Get_FP(iAx3).ToString("0.000");
                }
                else
                {
                    lblDry_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                    lblDry_ZPos.Text = CMot.It.Get_FP(iAx2).ToString();
                    lblDry_RPos.Text = CMot.It.Get_FP(iAx3).ToString();
                }

                ckbDry_Top.Checked   = CIO.It.Get_Y(eY.DRY_TopAir);
                ckbDry_Btm.Checked   = CIO.It.Get_Y(eY.DRY_BtmAir);
                ckbDry_Stick.Checked = CIO.It.Get_Y(eY.DRY_BtmStandbyPos);
                ckbDry_Flow.Checked  = CIO.It.Get_Y(eY.DRY_ClnBtmFlow);
                ckbDry_IOOn.Checked  = CIO.It.Get_Y(eY.IOZR_Power);

                // 2022.03.03 lhs Start : Spray Nozzle Btm Cleaner
                if (CDataOption.UseSprayBtmCleaner)
                {
                    ckbDry_BtmClnNzlMove.Checked = CIO.It.Get_Y(eY.DRY_ClnBtmNzlForward);
                    ckbDry_BtmClnAirBlow.Checked = CIO.It.Get_Y(eY.DRY_ClnBtmAirBlow);
                }
                // 2022.03.03 lhs End

                // 2022.03.14 lhs Start : Level Safety Air Blow
                if (CDataOption.UseDryerLevelAirBlow)
                {
                    ckbDry_LevelAirBlow.Checked = CIO.It.Get_Y(eY.DRY_LevelSafetyAir);
                }
                // 2022.03.14 lhs End
                
                ckb_ClampOpen.Checked       = CIO.It.Get_Y(eY.DRY_ClampOpenOnOff);      // 2022.07.01 lhs 
            }            

            lblDry_PushO.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_PusherOverload]];
            lblDry_StpOutD.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.DRY_StripOutDetect]];
            lbl_Lv1.BackColor           = GV.CR_X[CIO.It.aInput[(int)eX.DRY_LevelSafety1]];
            lbl_Lv2.BackColor           = GV.CR_X[CIO.It.aInput[(int)eX.DRY_LevelSafety2]];

            // 2022.07.01 lhs Start
            lbl_FrontClamp.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Front_Clamp]];
            lbl_FrontUnClamp.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Front_Unclamp]];
            lbl_RearClamp.BackColor     = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Rear_Clamp]];
            lbl_RearUnClamp.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Rear_Unclamp]];
            // 2022.07.01 lhs End

            if (CDataOption.Dryer == eDryer.Rotate)
            {
                lbl_BtmStandby.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.DRY_BtmStandbyPos]];
                lbl_BtmTarget.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.DRY_BtmTargetPos]];
            }
            else
            {
                lbl_BtmStandby.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.DRY_CoverOpen]];
                lbl_BtmTarget.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.DRY_CoverClose]];
                lbl_Unit1.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Unit1_Detect]];
                lbl_Unit2.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Unit2_Detect]];
                lbl_Unit3.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Unit3_Detect]];
                lbl_Unit4.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Unit4_Detect]];
            }
            lblDry_BtmFlow.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmFlow]];

            // 2022.03.03 lhs Start : SprayBtmCleaner 사용시 표시
            if (CDataOption.UseSprayBtmCleaner)
            {
                lblDry_BtmClnNzlForward.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmNzlForward]];
                lblDry_BtmClnNzlBackward.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmNzlBackward]];
            }
            // 2022.03.03 lhs End
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
            btn_Home.Enabled                = bVal;
            btn_Wait.Enabled                = bVal;
            btn_CkSafty.Enabled             = bVal;
            btn_DryWork.Enabled             = bVal;
            btn_StripOut.Enabled            = bVal;
            btn_WaterNozzle.Enabled         = bVal; // 2021.04.16 lhs
            ckbDry_Top.Enabled              = bVal;
            ckbDry_Btm.Enabled              = bVal;
            ckbDry_Stick.Enabled            = bVal;
            ckbDry_Flow.Enabled             = bVal;
            ckbDry_IOOn.Enabled             = bVal; // 2022.03.14 lhs
            ckbDry_BtmClnNzlMove.Enabled    = bVal; // 2022.03.03 lhs
            ckbDry_BtmClnAirBlow.Enabled    = bVal; // 2022.03.03 lhs
            ckbDry_LevelAirBlow.Enabled     = bVal; // 2022.03.14 lhs
            ckb_ClampOpen.Enabled           = bVal; // 2022.07.01 lhs
            btn_Z_Up_Pos.Enabled            = bVal; // 2022.07.22 lhs

        }

        /// <summary>
        /// Manual dry view에 조작 로그 저장 함수
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

            ckbDry_Top.Checked = CIO.It.Get_Y(eY.DRY_TopAir);
            ckbDry_Btm.Checked = CIO.It.Get_Y(eY.DRY_BtmAir);
            ckbDry_Stick.Checked = CIO.It.Get_Y(eY.DRY_BtmStandbyPos);
            ckbDry_Flow.Checked = CIO.It.Get_Y(eY.DRY_ClnBtmFlow);
            ckbDry_IOOn.Checked = CIO.It.Get_Y(eY.IOZR_Power);

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

            if (CData.Opt.bQcUse && ESeq == ESeq.DRY_Out)
            {
                //CTcpIp.It.SendStripOut();
                //#2 ReadyQuery
                CGxSocket.It.SendMessage("EQReadyQuery");
            }

            //190615 pjh:
            //Dry Stick Check
            if (ESeq == ESeq.All_Home || ESeq == ESeq.DRY_Run || ESeq == ESeq.DRY_Out || ESeq == ESeq.DRY_CheckSensor || 
                ESeq == ESeq.DRY_WaterNozzle) // 2021.04.11 lhs : Dry_WaterNozzle 추가, UI 수정 및 확인할 것
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
                case 0:    // Top Air Blow
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 1:    // Bottom Air Blow
                    {
                        if(CDataOption.UseDryWtNozzle)
                        {
                            if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("WATER") + "\r\n" + CLang.It.GetLanguage("NOZZLE") + "\r\n" + CLang.It.GetLanguage("ON");
                            else                sTxt = CLang.It.GetLanguage("WATER") + "\r\n" + CLang.It.GetLanguage("NOZZLE") + "\r\n" + CLang.It.GetLanguage("OFF");
                        }
                        else
                        {
                            if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("ON");
                            else                sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("OFF");
                        }
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 2:    // Bottom Air Blow Standby Position
                    {
                        if (mCkb.Checked)
                        {
                            if (CDataOption.Dryer == eDryer.Rotate) { sTxt = CLang.It.GetLanguage("STANDBY") + "\r\n" + CLang.It.GetLanguage("POSITION");   }
                            else                                    { sTxt = CLang.It.GetLanguage("COVER")   + "\r\n" + CLang.It.GetLanguage("OPEN");       }
                        }
                        else
                        {
                            if (CDataOption.Dryer == eDryer.Rotate) { sTxt = CLang.It.GetLanguage("TARGET")  + "\r\n" + CLang.It.GetLanguage("POSITION");   }
                            else                                    { sTxt = CLang.It.GetLanguage("COVER")   + "\r\n" + CLang.It.GetLanguage("CLOSE");      }
                        }
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 3:    // Bottom Clean Flow
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("WATER") + " " + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("WATER") + " " + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 4:    // Ionizer
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
             
               case 5:    // Bottom Clean Nozzle Move (Forward/Backward)
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLEAN") + "\r\n" + CLang.It.GetLanguage("NOZZLE") + "\r\n" + CLang.It.GetLanguage("FORWARD");
                        else                sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLEAN") + "\r\n" + CLang.It.GetLanguage("NOZZLE") + "\r\n" + CLang.It.GetLanguage("BACKWARD");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 6:    // Bottom Clean Air Blower
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLEAN") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLEAN") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                case 7:    // Level Safety Air Blow
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("LEVEL SAFETY") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("ON");
                        else                sTxt = CLang.It.GetLanguage("LEVEL SAFETY") + "\r\n" + CLang.It.GetLanguage("AIR BLOW") + "\r\n" + CLang.It.GetLanguage("OFF");
                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                case 8:    // Strip Clamp Cylinder On/Off
                    {
                        if (mCkb.Checked)   sTxt = CLang.It.GetLanguage("STRIP") + "\r\n" + CLang.It.GetLanguage("CALAMP") + "\r\n" + CLang.It.GetLanguage("CLOSE");
                        else                sTxt = CLang.It.GetLanguage("STRIP") + "\r\n" + CLang.It.GetLanguage("CALAMP") + "\r\n" + CLang.It.GetLanguage("OPEN");
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
                case 0:    // Top Air Blow
                    {
                        CIO.It.Set_Y(eY.DRY_TopAir, mCkb.Checked);
                        break;
                    }
                case 1:    // Bottom Air Blow
                    {
                        CIO.It.Set_Y(eY.DRY_BtmAir, mCkb.Checked);
                        break;
                    }

                case 2:    // Bottom Air Blow Standby Position
                    {
                        if (mCkb.Checked)
                        {
                            if (CDataOption.Dryer == eDryer.Rotate)
                            {
                                CSq_Dry.It.m_bStickRun = false;
                                CSq_Dry.It.Act_Set_Stick_StandyPos();
                            }
                            else
                            {
                                CSq_Dry.It.m_bStickRun = false;
                                CSq_Dry.It.Func_CoverClose(false);
                            }
                        }
                        else
                        {// 200103_pjh_Dry_Stick_InterLock
                            if (CDataOption.Dryer == eDryer.Rotate)
                            {
                                if (!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos))
                                {
                                    if (CDataOption.UseDryWtNozzle) CMsg.Show(eMsg.Error, "Error", "Check Water Nozzle Stick Sensor");
                                    else                            CMsg.Show(eMsg.Error, "Error", "Check Bottom Air Blow Stick Sensor");
                                    return;
                                }

								if (CDataOption.UseDryWtNozzle || CDataOption.eDryClamp == EDryClamp.Cylinder)  // 2022.07.01 lhs : Cylinder Clamp 추가
                                {
                                    if (CMot.It.Get_FP((int)EAx.DryZone_Z) > CData.SPos.dDRY_Z_Check) // 반드시 Z축이 내려져 있어야 충돌이 안됨.
                                    {
                                        CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                                        return;
                                    }
                                    else
                                    { CSq_Dry.It.Act_Set_Stick_TargetPos(); }
                                }
                                else
                                {
                                    if (!CIO.It.Get_X(eX.DRY_LevelSafety1) || !CIO.It.Get_X(eX.DRY_LevelSafety2))
                                    {
                                        CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                                        return;
                                    }
                                    else
                                    { CSq_Dry.It.Act_Set_Stick_TargetPos(); }
                                }
                                // 2021.04.16 lhs End
                            }
                            else
                            { CSq_Dry.It.Func_CoverClose(true); }
                        }

                        break;
                    }

                case 3:    // Bottom Clean Flow
                    {
                        CIO.It.Set_Y(eY.DRY_ClnBtmFlow, mCkb.Checked);
                        break;
                    }

                case 4:    // Ionizer
                    {
                        CIO.It.Set_Y(eY.IOZR_Power, mCkb.Checked);
                        break;
                    }
                case 5:
                    {
                        CIO.It.Set_Y(eY.DRY_ClnBtmNzlForward, mCkb.Checked);
                        break;
                    }
                case 6:
                    {
                        CIO.It.Set_Y(eY.DRY_ClnBtmAirBlow, mCkb.Checked);
                        break;
                    }
                case 7:
                    {
                        CIO.It.Set_Y(eY.DRY_LevelSafetyAir, mCkb.Checked);
                        break;
                    }
                case 8:   // 2022.07.01 lhs : Clamp Cylinder On/Off
                    {
                        CIO.It.Set_Y(eY.DRY_ClampOpenOnOff, mCkb.Checked);
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

        private void btn_Z_Up_Pos_Click(object sender, EventArgs e)
        {
            if (CDataOption.Dryer == eDryer.Rotate)
            {
                if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                    return;
                }

                // Rotate Check : dDRY_R_Wait
                int iAxis1 = (int)EAx.DryZone_Air;
                double dPos1 = CData.SPos.dDRY_R_Wait;
                double dCurPos1 = CMot.It.Get_FP(iAxis1);

                if (!CMot.It.Get_Mv(iAxis1, dPos1)) 
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Check Dry R Axis");
                    return;
                }
            }

            if (CDataOption.eDryClamp == EDryClamp.Cylinder) // 2022.07.12 lhs
            {
                if (!CSq_Dry.It.Func_DryClampOpen(false))
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Check Dry Unclalmp");
                    return;
                }
            }
            
            CMot.It.Mv_N((int)(int)EAx.DryZone_Z, CData.SPos.dDRY_Z_Up);

        }
    }
}
