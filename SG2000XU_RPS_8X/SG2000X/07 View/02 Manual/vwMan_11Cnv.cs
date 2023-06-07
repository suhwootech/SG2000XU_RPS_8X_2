using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_11Cnv : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_11Cnv()
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

            //190521 ksg :
            /*
            if (CData.CurCompany == eCompany.Qorvo)
            {
                panel31.Visible = true;
            }
            else
            {
                panel31.Visible = false;
            }
            */
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            //200211 ksg :
            /*
            if (CSQ_Man.It.bBtnShow || (CSQ_Main.It.m_iStat == eStatus.Idle || CSQ_Main.It.m_iStat == eStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = false;
                _EnableBtn(true);
            }
            */
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
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
            btn_ConOnLPick.Enabled = bVal;
            button15.Enabled = bVal;
            button17.Enabled = bVal;
            btn_ConOfLPick.Enabled = bVal;
            button14.Enabled = bVal;
            button18.Enabled = bVal;
            button16.Enabled = bVal;
        }

        /// <summary>
        /// Manual cnvertion view에 조작 로그 저장 함수
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
            /*
            if (CSQ_Man.It.bBtnShow || (CSQ_Main.It.m_iStat == eStatus.Idle || CSQ_Main.It.m_iStat == eStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }
            */
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

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


            if (ESeq == ESeq.Warm_Up)
            {
                if (CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDR_TbVaccum))
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Notice, "Notice", "Strip exist on the table. Please check");
                    return;
                }

            }

            //1900116 ksg : 조건 추가
            if (ESeq == ESeq.GRL_Grinding || ESeq == ESeq.GRR_Grinding ||
               ESeq == ESeq.GRL_Strip_Measure || ESeq == ESeq.GRR_Strip_Measure)
            {
                if (!CSQ_Main.It.CheckWarmUp())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Error, "Error", "Need Warm Up Please");
                    return;
                }
            }
            //190131 ksg : Table 동작 중 Strip Check 기능 추가
            if (ESeq == ESeq.Warm_Up)
            {
                if (CIO.It.Get_Y(eY.GRDL_TbVacuum) || CIO.It.Get_Y(eY.GRDR_TbVacuum))
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                    return;
                }
            }
            //190131 ksg : Table 동작 중 Strip Check 기능 추가
            if (ESeq == ESeq.GRR_Dressing || ESeq == ESeq.GRR_Dresser_Measure ||
               ESeq == ESeq.GRR_Wheel_Measure || ESeq == ESeq.GRR_Table_Grinding)
            {
                if (CIO.It.Get_Y(eY.GRDR_TbVacuum))
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                    return;
                }
            }
            //190131 ksg : Table 동작 중 Strip Check 기능 추가
            if (ESeq == ESeq.GRL_Dressing || ESeq == ESeq.GRL_Dresser_Measure ||
               ESeq == ESeq.GRL_Wheel_Measure || ESeq == ESeq.GRL_Table_Grinding)
            {
                if (CIO.It.Get_Y(eY.GRDL_TbVacuum))
                {
                    CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                    return;
                }
            }

            if (ESeq == ESeq.GRL_Grinding && CData.Opt.aTblSkip[(int)EWay.L])
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Error, "Error", "Left Table Skip Now!");
                return;
            }
            if (ESeq == ESeq.GRR_Grinding && CData.Opt.aTblSkip[(int)EWay.R])
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Error, "Error", "Right Table Skip Now!");
                return;
            }

            if (ESeq == ESeq.GRR_Dressing || ESeq == ESeq.GRR_Grinding || ESeq == ESeq.GRR_WaterKnife ||
               ESeq == ESeq.GRR_Table_Measure || ESeq == ESeq.GRR_Strip_Measure || ESeq == ESeq.GRR_Water_Knife ||
               ESeq == ESeq.GRR_Strip_Measureone || ESeq == ESeq.GRR_Dresser_Measure || ESeq == ESeq.GRR_Wheel_Measure ||
               ESeq == ESeq.GRR_Table_Grinding ||
               ESeq == ESeq.GRL_Dressing || ESeq == ESeq.GRL_Grinding || ESeq == ESeq.GRL_WaterKnife ||
               ESeq == ESeq.GRL_Table_Measure || ESeq == ESeq.GRL_Strip_Measure || ESeq == ESeq.GRL_Water_Knife ||
               ESeq == ESeq.GRL_Strip_Measureone || ESeq == ESeq.GRL_Dresser_Measure || ESeq == ESeq.GRL_Wheel_Measure ||
               ESeq == ESeq.GRL_Table_Grinding)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "One More Check Strip Jig. Please") == DialogResult.Cancel)
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    return;
                }

                if (!CData.Opt.bCoverSkip && !CSQ_Main.It.CheckCover())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "Grinding Cover Check Please");
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

            if (CData.Opt.bQcUse && ESeq == ESeq.DRY_Out)
            {
                //CTcpIp.It.SendStripOut();
                //#2 ReadyQuery
                CGxSocket.It.SendMessage("EQReadyQuery");
            }

            //190615 pjh:
            //Dry Stick Check

            if (ESeq == ESeq.All_Home || ESeq == ESeq.DRY_Run || ESeq == ESeq.DRY_Out || ESeq == ESeq.DRY_CheckSensor || 
                ESeq == ESeq.DRY_WaterNozzle) // 2021.04.11 lhs : Dry_WaterNozzle 추가
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
                CSQ_Man.It.bBtnShow = true; //190627 ksg 
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
    }
}
