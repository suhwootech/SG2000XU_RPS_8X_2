using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_10Ofl : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        private Timer m_tmQCMonitor;                // 2021-07-21, jhLee : QC의 간섭 여부를 모니터링 한다.

        public vwMan_10Ofl()
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

            m_tmQCMonitor = new Timer();
            m_tmQCMonitor.Interval = 1000;                  // 1초마다 수행
            m_tmQCMonitor.Tick += _M_tmQCMonitor_Tick;

            //20200513 jhc : 매거진 배출 위치 변경 추가
            UpdateTopBtmBtnVisible();
        }

        //20200513 jhc : 매거진 배출 위치 변경 추가 => 옵션에 따른 버튼 보기/감춤 재설정
        private void UpdateTopBtmBtnVisible()
        {
            if (CData.Opt.bQcUse)
            {
                btn_PickTop.Visible = true;
                btn_PlaceTop.Visible = true;
                btn_RcvTop.Visible = true;
                btn_PickBtm.Visible = true;
                btn_PlaceBtm.Visible = true;
                btn_RcvBtm.Visible = true;
            }
            else
            {
                bool bTop = (CData.Opt.eEmitMgz == EMgzWay.Top) ? true : false;
                btn_PickTop.Visible = bTop;
                btn_PlaceTop.Visible = bTop;
                btn_RcvTop.Visible = bTop;

                // 2022.04.20 SungTae Start : [추가] ASE-KH 관련
                if (CData.CurCompany == ECompany.ASE_K12)
                    btn_TopCheckBcr.Visible = bTop;
                else
                    btn_TopCheckBcr.Visible = false;
                // 2022.04.20 SungTae End

                btn_PickBtm.Visible = !bTop;
                btn_PlaceBtm.Visible = !bTop;
                btn_RcvBtm.Visible = !bTop;
            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            int iAx1 = (int)EAx.OffLoader_X;
            int iAx2 = (int)EAx.OffLoader_Y;
            int iAx3 = (int)EAx.OffLoader_Z;

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // X
                if (CMot.It.Chk_Alr(iAx1)) { lbl_XStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx1) == EAxOp.Idle)
                { lbl_XStat.BackColor = Color.LightGray; }
                else { lbl_XStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl_XPos.Text = CMot.It.Get_FP(iAx1).ToString("0.000");
                }
                else
                {
                    lbl_XPos.Text = CMot.It.Get_FP(iAx1).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Y
                if (CMot.It.Chk_Alr(iAx2)) { lbl_YStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx2) == EAxOp.Idle)
                { lbl_YStat.BackColor = Color.LightGray; }
                else { lbl_YStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl_YPos.Text = CMot.It.Get_FP(iAx2).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl_YPos.Text = CMot.It.Get_FP(iAx2).ToString("0.000");
                }
                else
                {
                    lbl_YPos.Text = CMot.It.Get_FP(iAx2).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Z
                if (CMot.It.Chk_Alr(iAx3)) { lbl_ZStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP(iAx3) == EAxOp.Idle)
                { lbl_ZStat.BackColor = Color.LightGray; }
                else { lbl_ZStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl_ZPos.Text = CMot.It.Get_FP(iAx3).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl_ZPos.Text = CMot.It.Get_FP(iAx3).ToString("0.000");
                }
                else
                {
                    lbl_ZPos.Text = CMot.It.Get_FP(iAx3).ToString();
                }
                // 2020.11.27 JSKim Ed

                ckbOfL_TcR.Checked = CIO.It.Get_Y(eY.OFFL_TopBeltRun);
                ckbOfL_TmUp.Checked = CIO.It.Get_Y(eY.OFFL_TopMGZUp);
                ckbOfL_TmOn.Checked = CIO.It.Get_Y(eY.OFFL_TopClampOn);
                ckbOfL_McCCW.Checked = CIO.It.Get_Y(eY.OFFL_MidBeltCCW);
                ckbOfL_McR.Checked = CIO.It.Get_Y(eY.OFFL_MidBeltRun);
                ckbOfL_BcR.Checked = CIO.It.Get_Y(eY.OFFL_BtmBeltRun);
                ckbOfL_BmUp.Checked = CIO.It.Get_Y(eY.OFFL_BtmMGZUp);
                ckbOfL_BmOn.Checked = CIO.It.Get_Y(eY.OFFL_BtmClampOn);
            }

            lblOfL_EmgF.BackColor = GV.CR_Y[CIO.It.aInput[(int)eX.OFFL_EMGFront]];
            lblOfL_EmgR.BackColor = GV.CR_Y[CIO.It.aInput[(int)eX.OFFL_EMGRear]];
            lblOfL_DorF.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_DoorFront]];
            lblOfL_DorRg.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_DoorRight]];
            lblOfL_DorR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_DoorRear]];
            lblOfL_LiCur.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_LightCurtain]];
            lblOfL_TcMgzD.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZDetect]];
            lblOfL_TcMgzF.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZDetectFull]];
            lblOfL_McMgzD.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_MidMGZDetect]];
            lblOfL_BcMgzD.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZDetect]];
            lblOfL_BcMgzF.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZDetectFull]];
            lblOfL_TmD1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopClampMGZDetect1]];
            lblOfL_TmD2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopClampMGZDetect2]];
            string text;
            // Top Magazine Down/Up
            if (CIO.It.Get_X(eX.OFFL_TopMGZUp) && !CIO.It.Get_X(eX.OFFL_TopMGZDn))
            {
                lblOfL_TmUp.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Top") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Cylinder") + " " + CLang.It.GetLanguage("Up");
                SetWriteLanguage(lblOfL_TmUp.Name, text);

            }
            else if (!CIO.It.Get_X(eX.OFFL_TopMGZUp) && CIO.It.Get_X(eX.OFFL_TopMGZDn))
            {
                lblOfL_TmUp.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Top") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Cylinder") + " " + CLang.It.GetLanguage("Down");
                SetWriteLanguage(lblOfL_TmUp.Name, text);
            }
            else
            {
                lblOfL_TmUp.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Top") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Cylinder");
                SetWriteLanguage(lblOfL_TmUp.Name, text);
            }
            // Top Magazine Unclamp/Clamp
            if (CIO.It.Get_X(eX.OFFL_TopClampOn) && !CIO.It.Get_X(eX.OFFL_TopClampOff))
            {
                lblOfL_TmOn.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Top") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Clamp");
                SetWriteLanguage(lblOfL_TmOn.Name, text);
            }
            else if (!CIO.It.Get_X(eX.OFFL_TopClampOn) && CIO.It.Get_X(eX.OFFL_TopClampOff))
            {
                lblOfL_TmOn.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Top") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Unclamp");
                SetWriteLanguage(lblOfL_TmOn.Name, text);
            }
            else
            {
                lblOfL_TmOn.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Top") + " " + CLang.It.GetLanguage("MGZ") + "\r\n";
                SetWriteLanguage(lblOfL_TmOn.Name, text);
            }

            lblOfL_BmD1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmClampMGZDetect1]];
            lblOfL_BmD2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmClampMGZDetect2]];
            // Bottom Magazine Down/Up
            if (CIO.It.Get_X(eX.OFFL_BtmMGZUp) && !CIO.It.Get_X(eX.OFFL_BtmMGZDn))
            {
                lblOfL_BmUp.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Bottom") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Cylinder") + " " + CLang.It.GetLanguage("Up");
                SetWriteLanguage(lblOfL_BmUp.Name, text);
            }
            else if (!CIO.It.Get_X(eX.OFFL_BtmMGZUp) && CIO.It.Get_X(eX.OFFL_BtmMGZDn))
            {
                lblOfL_BmUp.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Bottom") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Cylinder") + " " + CLang.It.GetLanguage("Down");
                SetWriteLanguage(lblOfL_BmUp.Name, text);
            }
            else
            {
                lblOfL_BmUp.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Bottom") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Cylinder");
                SetWriteLanguage(lblOfL_BmUp.Name, text);

            }
            // Bottom Magazine Unclamp/Clamp
            if (CIO.It.Get_X(eX.OFFL_BtmClampOn) && !CIO.It.Get_X(eX.OFFL_BtmClampOff))
            {
                lblOfL_BmOn.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Bottom") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Clamp");
                SetWriteLanguage(lblOfL_BmOn.Name, text);
            }
            else if (!CIO.It.Get_X(eX.OFFL_BtmClampOn) && CIO.It.Get_X(eX.OFFL_BtmClampOff))
            {
                lblOfL_BmOn.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Bottom") + " " + CLang.It.GetLanguage("MGZ") + "\r\n" + CLang.It.GetLanguage("Unclamp");
                SetWriteLanguage(lblOfL_BmOn.Name, text);
            }
            else
            {
                lblOfL_BmOn.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Bottom") + " " + CLang.It.GetLanguage("MGZ") + "\r\n";
                SetWriteLanguage(lblOfL_BmOn.Name, text);
            }

            // 2022.04.19 SungTae Start : [추가] ASE-KH 관련
            if (CData.CurCompany == ECompany.ASE_K12)
            {
                btn_TopCheckBcr.Visible = true;
                pnlOfl_Bcr.Visible = true;

                lblOfl_Bcr.Text = CData.KeyBcr[(int)EKeyenceBcrPos.OffLd].sBcr;
            }
            else
            {
                btn_TopCheckBcr.Visible = false;
                pnlOfl_Bcr.Visible = false;
            }
            // 2022.04.19 SungTae End
        }


        // 2021-07-21, jhLee : 주기적으로 QC의 X-Feeder 물리적 간섭 여부를 체크한다.
        private void _M_tmQCMonitor_Tick(object sender, EventArgs e)
        {
            // QC와 연결이 되어있다면 상태 조회를 요청한다.
            if ( CGxSocket.It.IsConnected() )
            {
                CGxSocket.It.SendMessage("StatusQuery");            // 상태를 조회한다.
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
            btn_Home.Enabled = bVal;
            btn_Wait.Enabled = bVal;


            // 2022.04.20 SungTae Start : [추가] ASE-KH 관련
            if (CData.CurCompany == ECompany.ASE_K12)
                btn_TopCheckBcr.Enabled = bVal;
            else
                btn_TopCheckBcr.Enabled = false;
            // 2022.04.20 SungTae End


            //20200513 jhc : 매거진 배출 위치 변경 추가
            if (CData.Opt.bQcUse) //if (CData.CurCompany == ECompany.SkyWorks)
            {
                btn_PickTop.Enabled = bVal;
                btn_PlaceTop.Enabled = bVal;
                btn_RcvTop.Enabled = bVal;
            }
            else
            {
                if (CData.Opt.eEmitMgz == EMgzWay.Top)
                {
                    btn_PickTop.Enabled = bVal;
                    btn_PlaceTop.Enabled = bVal;
                    btn_RcvTop.Enabled = bVal;
                    btn_TopCheckBcr.Enabled = bVal;
                }
            }
            //

            btn_PickBtm.Enabled = bVal;
            btn_PlaceBtm.Enabled = bVal;
            btn_RcvBtm.Enabled = bVal;
            ckbOfL_TcR.Enabled = bVal;
            ckbOfL_TmUp.Enabled = bVal;
            ckbOfL_TmOn.Enabled = bVal;
            ckbOfL_McCCW.Enabled = bVal;
            ckbOfL_McR.Enabled = bVal;
            ckbOfL_BcR.Enabled = bVal;
            ckbOfL_BmUp.Enabled = bVal;
            ckbOfL_BmOn.Enabled = bVal;
        }

        /// <summary>
        /// Manual offloader view에 조작 로그 저장 함수
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

            ckbOfL_TcR.Checked = CIO.It.Get_Y(eY.OFFL_TopBeltRun);
            ckbOfL_TmUp.Checked = CIO.It.Get_Y(eY.OFFL_TopMGZUp);
            ckbOfL_TmOn.Checked = CIO.It.Get_Y(eY.OFFL_TopClampOn);
            ckbOfL_McCCW.Checked = CIO.It.Get_Y(eY.OFFL_MidBeltCCW);
            ckbOfL_McR.Checked = CIO.It.Get_Y(eY.OFFL_MidBeltRun);
            ckbOfL_BcR.Checked = CIO.It.Get_Y(eY.OFFL_BtmBeltRun);
            ckbOfL_BmUp.Checked = CIO.It.Get_Y(eY.OFFL_BtmMGZUp);
            ckbOfL_BmOn.Checked = CIO.It.Get_Y(eY.OFFL_BtmClampOn);

            //20200513 jhc : 매거진 배출 위치 변경 추가
            UpdateTopBtmBtnVisible();
            //

            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled) { m_tmUpdt.Start(); }

            if (CDataOption.UseQC && CData.Opt.bQcUse) // 2021-07-21, jhLee, QC로 상태 조회 요청
            {
                if (!m_tmQCMonitor.Enabled)
                {
                    m_tmQCMonitor.Start();
                }
            }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled) { m_tmUpdt.Stop(); }

            if (m_tmQCMonitor.Enabled)          //  2021-07-21, jhLee, QC의 상태 조회요청 중지
            {
                m_tmQCMonitor.Stop();
            }

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
                  (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home    ) || (ESeq == ESeq.GRL_Home     ) || (ESeq == ESeq.GRR_Home) ||
                  (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home    ) || (ESeq == ESeq.OFL_Home)))
            {
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }

            // 2021-07-20, jhLee, Skyworks VOC, QC가 안전한 상태가 아니라면 수동 동작을 수행해서는 않된다.
            // 동작 불가 QC 상태 조건
            // 1. 자동 시퀀스상 QC -> EQ로 배출 동작 시퀀스를 처리 중인 경우
            // 2. Pusher가 Magazine과 간섭위치 가까이에 존재하면 동작 불가
            //
            if ( CSq_OfL.It.QcVisionPermission() == 1)                      // 간섭 발생
            {
                CSQ_Man.It.bBtnShow = true;
                CMsg.Show(eMsg.Error, "Error", "Risk of collision with QC-Vision.\n Check QC-Vision State");
                return;
            }

            //210730 pjh : Manual 동작 시 Strip Out detect Check
            if ((CDataOption.UseQC && CData.Opt.bQcUse) == false)
            {
                if(!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    CErr.Show(eErr.DRY_DETECTED_STRIP);
                    _SetLog("Error : Strip-out detect.");
                    return;
                }
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
        }


        /// <summary>
        /// Off Loader Part CheckBox Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbOfL_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            string sTxt;

            switch (iTag)
            {
                case 0:    // Top conveyor run
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("RUN");
                        else
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("STOP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 1:    // Top cylinder up
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CYLINDER") + "\r\n" + CLang.It.GetLanguage("UP");
                        else
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CYLINDER") + "\r\n" + CLang.It.GetLanguage("DOWN");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 2:    // Top clamp on
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLAMP");
                        else
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("UNCLAMP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 3:    // Middle conveyor CCW
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("MIDDLE") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("CCW");
                        else
                            sTxt = CLang.It.GetLanguage("MIDDLE") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("CW");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 4:    // Middle conveyor run
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("MIDDLE") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("RUN");
                        else
                            sTxt = CLang.It.GetLanguage("MIDDLE") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("STOP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 5:    // Bottom conveyor run
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("RUN");
                        else
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CONVEYOR") + "\r\n" + CLang.It.GetLanguage("STOP");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 6:    // Bottom cylinder up
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CYLINDER") + "\r\n" + CLang.It.GetLanguage("UP");
                        else
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CYLINDER") + "\r\n" + CLang.It.GetLanguage("DOWN");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 7:    // Bottom clamp on
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("CLAMP");
                        else
                            sTxt = CLang.It.GetLanguage("BOTTOM") + "\r\n" + CLang.It.GetLanguage("UNCLAMP");

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
                case 0:    // Top conveyor run
                    {
                        CSq_OfL.It.Belt_Top(mCkb.Checked);
                        break;
                    }

                case 1:    // Top cylinder up
                    {
                        CSq_OfL.It.Act_TopClampModuleDU(mCkb.Checked);
                        break;
                    }

                case 2:    // Top clamp on
                    {
                        CSq_OfL.It.Act_TopClampOC(mCkb.Checked);
                        break;
                    }

                case 3:    // Middle conveyor CCW
                    {
                        CIO.It.Set_Y(eY.OFFL_MidBeltCCW, mCkb.Checked);
                        break;
                    }
                case 4:    // Middle conveyor run
                    {
                        CSq_OfL.It.Belt_Mid(mCkb.Checked, ckbOfL_McCCW.Checked);
                        break;
                    }

                case 5:    // Bottom conveyor run
                    {
                        CSq_OfL.It.Belt_Btm(mCkb.Checked);
                        break;
                    }

                case 6:    // Bottom cylinder up
                    {
                        CSq_OfL.It.Act_BtmClampModuleDU(mCkb.Checked);
                        break;
                    }

                case 7:    // Bottom clamp on
                    {
                        CSq_OfL.It.Act_BtmClampOC(mCkb.Checked);
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
    }
}
