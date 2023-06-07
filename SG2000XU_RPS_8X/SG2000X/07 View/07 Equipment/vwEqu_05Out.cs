using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwEqu_05Out : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwEqu_05Out()
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

            pnlIO_Out_Main.Controls.Clear();
            pnlIO_Out_OnLoader.Controls.Clear();
            pnlIO_Out_OffLoader.Controls.Clear();

            // Output - Main setting
            for (int i = 0; i < CIO.It.MAIN_OUT_SIZE; i++)
            {
                Label mLbl = new Label();
                mLbl.Name = "lbl_" + CIO.It.aMain_OutTxt[i].Substring(0, 5);
                mLbl.Size = new Size(478, 20);
                mLbl.Location = new Point(10, 10 + (i * (mLbl.Height + 5)));
                mLbl.Text = CIO.It.aMain_OutTxt[i];
                mLbl.TextAlign = ContentAlignment.MiddleLeft;
                mLbl.Tag = i;
                mLbl.Click += _MLbl_Click;
                pnlIO_Out_Main.Controls.Add(mLbl);
            }

            // Output - On Loader setting
            for (int i = 0; i < CIO.It.LDR_OUT_SIZE; i++)
            {
                Label mLbl = new Label();
                mLbl.Name = "lbl_" + CIO.It.aLdr_OutTxt[i].Substring(0, 5);
                mLbl.Size = new Size(478, 20);
                mLbl.Location = new Point(10, 10 + (i * (mLbl.Height + 5)));
                mLbl.Text = CIO.It.aLdr_OutTxt[i];
                mLbl.TextAlign = ContentAlignment.MiddleLeft;
                mLbl.Tag = (CIO.It.MAIN_OUT_SIZE + i);
                mLbl.Click += _MLbl_Click;
                pnlIO_Out_OnLoader.Controls.Add(mLbl);
            }

            // Output - On Loader setting
            for (int i = 0; i < CIO.It.ODR_OUT_SIZE; i++)
            {
                Label mLbl = new Label();
                mLbl.Name = "lbl_" + CIO.It.aOdr_OutTxt[i].Substring(0, 5);
                mLbl.Size = new Size(478, 20);
                mLbl.Location = new Point(10, 10 + (i * (mLbl.Height + 5)));
                mLbl.Text = CIO.It.aOdr_OutTxt[i];
                mLbl.TextAlign = ContentAlignment.MiddleLeft;
                mLbl.Tag = (CIO.It.MAIN_OUT_SIZE + CIO.It.LDR_OUT_SIZE + i);
                mLbl.Click += _MLbl_Click;
                pnlIO_Out_OffLoader.Controls.Add(mLbl);
            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            // Output - Main
            for (int i = 0; i < CIO.It.MAIN_OUT_SIZE; i++)
            {
                if (CIO.It.aMain_OutVal[i] == 0)
                {
                    pnlIO_Out_Main.Controls[i].BackColor = Color.LightGray;  //아무것도 없을 때 색깔
                }
                else
                {
                    pnlIO_Out_Main.Controls[i].BackColor = Color.Red;   //아웃풋 색깔
                }
            }

            // Output - On Loader
            for (int i = 0; i < CIO.It.LDR_OUT_SIZE; i++)
            {
                if (CIO.It.aLdr_OutVal[i] == 0)
                {
                    pnlIO_Out_OnLoader.Controls[i].BackColor = Color.LightGray;  //아무것도 없을 때 색깔
                }
                else
                {
                    pnlIO_Out_OnLoader.Controls[i].BackColor = Color.Red;   //아웃풋 색깔
                }
            }

            // Output - On Loader
            for (int i = 0; i < CIO.It.ODR_OUT_SIZE; i++)
            {
                if (CIO.It.aOdr_OutVal[i] == 0)
                {
                    pnlIO_Out_OffLoader.Controls[i].BackColor = Color.LightGray;  //아무것도 없을 때 색깔
                }
                else
                {
                    pnlIO_Out_OffLoader.Controls[i].BackColor = Color.Red;   //아웃풋 색깔
                }
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

        private void _MLbl_Click(object sender, EventArgs e)
        {
            Label mLbl = sender as Label;
            int iId = (int)mLbl.Tag;
            bool bVal = false;

            if (CData.WMX)
            {
                // 200316 mjy : Rotate 조건 추가
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    // 2021.04.16 lhs Start
                    if (CDataOption.UseDryWtNozzle || CDataOption.eDryClamp == EDryClamp.Cylinder)  // 2022.07.01 lhs : Cylinder Clamp 추가
                    {
                        if ( ( CMot.It.Get_FP((int)EAx.DryZone_Z) > CData.SPos.dDRY_Z_Check) && iId == 109 )
                        {
                            CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                            return;
                        }
                        else if ((!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos)) && iId == 109)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Check Bottom Air Blow Stick Sensor");
                            return;
                        }
                    } // 2021.04.16 lhs End
                    else
                    {   // 200103_pjh_Dry_Stick_InterLock
                        if ((!CIO.It.Get_X(eX.DRY_LevelSafety1) || !CIO.It.Get_X(eX.DRY_LevelSafety2)) && iId == 109)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                            return;
                        }
                        else if ((!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos)) && iId == 109)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Check Bottom Air Blow Stick Sensor");
                            return;
                        }
                    }
                }

                if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait && (iId == 32 || iId == 33))
                {//20200113 myk_ONP_Rotate_InterLock
                    CMsg.Show(eMsg.Error, "Error", "Check the Onloader Picker Z Axis Position");
                    return;
                }
                else if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z) != 1 && (iId == 32 || iId == 33))
                {
                    CMsg.Show(eMsg.Error, "Error", "First Onloader Picker Z Axis Home Please");
                    return;
                }

                if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait && (iId == 96 || iId == 97))
                {//20200113 myk_OFP_Rotate_InterLock
                    CMsg.Show(eMsg.Error, "Error", "Check the Offloader Picker Z Axis Position");
                    return;
                }
                else if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z) != 1 && (iId == 96 || iId == 97))
                {
                    CMsg.Show(eMsg.Error, "Error", "First Offloader Picker Z Axis Home Please");
                    return;
                }
                else
                {
                    bVal = CIO.It.Get_Y(iId);
                    _SetLog(string.Format("Click, {0}  {1} -> {2}", mLbl.Text, bVal, !bVal));
                    CIO.It.Set_Y(iId);
                }
            }
        }
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
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
    }
}
