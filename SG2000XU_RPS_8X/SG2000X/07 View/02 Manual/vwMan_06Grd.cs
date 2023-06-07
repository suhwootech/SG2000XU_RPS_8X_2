using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_06Grd : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_06Grd()
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
            //if (CData.CurCompany != eCompany.JSCK)
            // 2020.09.18 JSKim St
            //if (CData.CurCompany != ECompany.JSCK && CData.CurCompany != ECompany.SCK) //200121 ksg :
            if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK && CData.CurCompany != ECompany.JCET)
            // 2020.09.18 JSKim Ed
            {
                ckbGRD_IOOn.Visible = false;
            }
            else
            {
                ckbGRD_IOOn.Visible = true;
            }

            //20200731 josh    dual dressing
            // 20210112 myk : Skyworks도 Dual Dressing 사용 가능하게 수정
            btnGRD_DualDressing.Visible = (GV.MANUAL_DUAL_DRESSING && (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.SkyWorks || CData.CurCompany == ECompany.ASE_K12) ) ? true : false;
            //

            //220321 pjh : ASE KH Dual Grinding
            btnGRD_DualGrinding.Visible = (CData.CurCompany == ECompany.ASE_K12 && CDataOption.UseDualGrinding) ? true : false;
            //
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            ckbGRD_IOOn.Checked = CIO.It.Get_Y(eY.IOZT_Power);
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
            btnGrd_H.Enabled = bVal;
            btnGrd_Wait.Enabled = bVal;
            btnGrd_DrsReplace.Enabled = bVal;

            btnGRD_DualDressing.Enabled = bVal; //20200731 josh    dual dressing
            btnGRD_DualGrinding.Enabled = bVal; //220321 pjh : Dual Grinding
        }

        /// <summary>
        /// Manual dual grind view에 조작 로그 저장 함수
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

            ckbGRD_IOOn.Checked = CIO.It.Get_Y(eY.IOZT_Power);

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

            //220426 pjh : Dual Grinding/Dressing Warm Up Check
            if (ESeq == ESeq.GRD_Dual_Grinding || ESeq == ESeq.GRD_Dual_Dressing)
            {
                if (!CSQ_Main.It.CheckWarmUp())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Error, "Error", "Need Warm Up Please");
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

            //220426 pjh : Dual Grinding/Dressing Door Lock Check
            if (CData.CurCompany == ECompany.ASE_K12)
            {
                if (ESeq == ESeq.GRD_Dual_Grinding || ESeq == ESeq.GRD_Dual_Dressing)
                {
                    if (CSQ_Main.It.CheckDoor() == false) // ASE_K12
                    {
                        CMsg.Show(eMsg.Error, "Error", "Door Close Please");
                        CSQ_Man.It.bBtnShow = true; //
                        return;
                    }
                }
            }
            
            //220321 pjh : Dual Grinding 시 양쪽 Vacuum On 돼있지 않으면 return
            if(ESeq == ESeq.GRD_Dual_Grinding)
            {
                //Test
                bool b1 = CIO.It.Get_X(eX.GRDL_TbVaccum);
                bool b2 = CIO.It.Get_X(eX.GRDR_TbVaccum);

                if (!(CIO.It.Get_X(eX.GRDL_TbVaccum) && CIO.It.Get_X(eX.GRDR_TbVaccum)))
                {
                    CSQ_Man.It.bBtnShow = true;
                    CMsg.Show(eMsg.Error, "Error", "Dual Grinding need 2 strips. Please Check Table Vacuum.");
                    return;
                }

                if (CMsg.Show(eMsg.Warning, "Warning", "One More Check Strip Jig. Please") == DialogResult.Cancel)
                {
                    CSQ_Man.It.bBtnShow = true;
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

                // 2021-06-24, jhLee, Skyworks VOC, 어느한 쪽이 먼저 끝났을 경우 Timeout 발생시키지않고 다른 한쪽이 완료될때까지 대기하도록 수정
                if (ESeq == ESeq.GRD_Dual_Dressing)
                {
                    CSQ_Man.It.bDDressFinish_L = false;        // 왼쪽 Table dressing 완료 여부
                    CSQ_Man.It.bDDressFinish_R = false;        // 오른쪽 Table dressing 완료 여부
                }
                //220321 pjh : ASE KH Dual Grinding 
                if (ESeq == ESeq.GRD_Dual_Grinding)
                {
                    CSQ_Man.It.bDGrindFinish_L = false;        // 왼쪽 Table Grinding 완료 여부
                    CSQ_Man.It.bDGrindFinish_R = false;        // 오른쪽 Table Grinding 완료 여부
                }

                //CData.L_GRD.m_tStat.bDrsR = true;
                CSQ_Man.It.Seq = ESeq;
                //190410 ksg :
                _EnableBtn(false);
                CSQ_Man.It.bBtnShow = false;
            }
        }

        /// <summary>
        /// 그라인드 좌, 우 테이블 조작 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            //190430 ksg : Auto & Manual 때 동작 안함
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            CheckBox mCkb = sender as CheckBox;
            _SetLog("Click");

            string text;
            // IONAZER
            CIO.It.Set_Y(eY.IOZT_Power, mCkb.Checked);
            
            CData.Opt.bIOZT_ManualClick = true; // 2021.09.15 lhs : 사용자 수동 클릭시에는 IOZT Off를 안할 목적 (SCK VOC)

            if (mCkb.Checked) text = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("ON");
            else text = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("OFF");
            SetWriteLanguage(mCkb.Name, text);
        }
    }
}
