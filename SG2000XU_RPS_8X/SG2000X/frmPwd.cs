using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmPwd : Form
    {
        private ELv CurLevel;
        public frmPwd(ELv eLv)
        {
            /*
            if ((int)eLanguage.English == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)eLanguage.Korea == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)eLanguage.China == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            */
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
            txtPw.Select();
            CurLevel = eLv;
            if (eLv == ELv.Technician)
            { BackColor = Color.Orange; }
            else
            { BackColor = Color.FromArgb(210, 56, 80); }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CIni m_Ini;
            string sPath = GV.PATH_CONFIG + "PW.cfg";
            string sSec    = "";
            string sKey    = "";
            string sEng    = "";
            string sManger = "";
            string sMaster = "";                // 2021-03-05, jhLee, Skyworks, 새로운 Master 암호체계 지원용
            
            m_Ini = new CIni(sPath);
            sSec = "PW";
            sKey = "TECHNICIAN";
            sEng = m_Ini.Read(sSec, sKey);
            sKey = "ENGINEER";
            sManger = m_Ini.Read(sSec, sKey);
            sMaster = "suhwoo1027" + DateTime.Now.ToString("hhmm");    // 2021-03-05, jhLee, Skyworks, 기존 Master 암호에 시/분 을 더한다.

            //if (CurLevel == eARL.Engineer && txtPw.Text == "test")
            if (CurLevel == ELv.Technician && txtPw.Text == sEng)
            { 
                
            }
            //else if(CurLevel == eARL.Manager && txtPw.Text == "test1")
            else if (CurLevel == ELv.Engineer && txtPw.Text == sManger)
            { 
                
            }
            // 2021-03-05, jhLee, Skyworks, 새로운 Master 암호체계 지원
            else if ( (CData.CurCompany == ECompany.SkyWorks) && ((CurLevel == ELv.Engineer) && (txtPw.Text == sMaster)))
            {
                CData.CurLevel = ELv.Master;
            }
            // Skyworks가 아닌경우 기존의 master 암호를 사용한다.
            //old else if (CurLevel == ELv.Engineer && txtPw.Text == "suhwoo1027")
            else if ((CData.CurCompany != ECompany.SkyWorks) && (CurLevel == ELv.Engineer && txtPw.Text == "suhwoo1027"))
            {
                CData.CurLevel = ELv.Master;
            }
            else 
            {
                CMsg.Show(eMsg.Warning, "Warning", "Wrong Passwaord");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCan_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            frmPwdChange mForm = new frmPwdChange(CurLevel);
            mForm.Location = new Point(this.Location.X + 230, Location.Y + 10 + btnChange.Location.Y);

            DialogResult = DialogResult.OK;
            if (mForm.ShowDialog() == DialogResult.OK)
            {
                Close();
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }
}
