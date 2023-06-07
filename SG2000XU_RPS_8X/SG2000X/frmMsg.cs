using System;
using System.IO;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmMsg : Form
    {

        // 2021.06.09 lhs Start
        private System.Windows.Forms.Timer m_tmUpdt;
        private DateTime m_StartT;
        // 2021.06.09 lhs End

        public frmMsg(eMsg iMsg, string sTitle, string sMsg)
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea == CData.Opt.iSelLan)     {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China == CData.Opt.iSelLan)     {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            InitializeComponent();

            lblRestTime.Visible = false;

            if (iMsg == eMsg.Notice)
            {
                btn_Ok.Location = btn_Can.Location;
                btn_Can.Visible = false;
            }
            else if (iMsg == eMsg.Warning)
            {
                BackColor = System.Drawing.Color.Orange;
            }
            else if (iMsg == eMsg.Error)
            {
                BackColor       = System.Drawing.Color.Firebrick;
                btn_Ok.Location = btn_Can.Location;
                btn_Can.Visible = false;
            }
            //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 Grinding/Strip Measure 시 18 Point 진행여부 질의를 위해 추가
            else if (iMsg == eMsg.Query)
            {
                btn_Ok.Text = "YES";
                btn_Can.Text = "NO";
            }
            // 2021.06.09 lhs Start : 일정시간 후 메시지창 닫기
            else if (iMsg == eMsg.QueryAndNo)
            {
                btn_Ok.Text = "YES";
                btn_Can.Text = "NO";

                m_tmUpdt = new System.Windows.Forms.Timer();
                m_tmUpdt.Interval = 200;
                m_tmUpdt.Tick += _M_tmUpdt_Tick;
                m_tmUpdt.Start();

                m_StartT = DateTime.Now;

                lblRestTime.Visible = true;
                lblRestTime.Text = "Close the window : 60 Sec";
            }
            // 2021.06.09 lhs End

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            {
                iMsg   = eMsg.Notice;
                sTitle = "Notice";
            }

            lbl_Title.Text = sTitle;
            lbl_Msg  .Text = sMsg  ;

        }


        // 2021.06.09 lhs Start : 일정시간(1분) 후 Cancel 리턴하고 창닫기 위한 Timer.
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            DateTime CurTime = DateTime.Now;
            TimeSpan DiffT = CurTime - m_StartT;
            lblRestTime.Text = string.Format("Close the window : {0} Sec", (60 - DiffT.Seconds));

            if (DiffT.Minutes >= 1)
            {
                lblRestTime.Visible = false;
                m_tmUpdt.Stop();
                m_tmUpdt.Dispose();

                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        // 2021.06.09 lhs End

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            // 2021.06.09 lhs Start
            if (m_tmUpdt != null)
            {
                m_tmUpdt.Stop();
                m_tmUpdt.Dispose();
            }
            // 2021.06.09 lhs End

            if(lbl_Title.Text == "Check PM Count")
            {
                CData.bBuzzPMMsgWnd      = false;
                CData.nShowStatePMMsgWnd = 2;
			}

            // 2022.08.29 lhs Start : Password 확인
            if (lbl_Title.Text == "Spindle Load Current Alarm")
            {
                if (CheckPwdForm() == false)  // 폼표시
                {
                    //_SetLog("CheckPwd() : Fail");
                    return;
                }
            }
            // 2022.08.29 lhs End


            DialogResult = DialogResult.OK;
			Close();
		}

        private void btn_Can_Click(object sender, EventArgs e)
        {
            // 2021.06.09 lhs Start
            if (m_tmUpdt != null)
            {
                m_tmUpdt.Stop();
                m_tmUpdt.Dispose();
            }
            // 2021.06.09 lhs End

            DialogResult = DialogResult.Cancel;
            Close();
        }


        // 2022.08.29 lhs : 알람창 닫을시 Password 확인
        private bool CheckPwdForm()
        {
            frmPwd mForm = new frmPwd(ELv.Engineer)
            {
                Location = Cursor.Position
            };
            mForm.TopMost = true;

            if (mForm.ShowDialog() == DialogResult.OK)
            {
                Close();
                return true;
            }
            return false;
        }

    }
}
