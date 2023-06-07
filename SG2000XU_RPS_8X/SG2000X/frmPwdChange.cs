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
    public partial class frmPwdChange : Form
    {
        private ELv CurLevel;
        public frmPwdChange(ELv eLv)
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
            
            CurLevel = eLv;
            if (eLv == ELv.Technician)
            { BackColor = Color.Orange; }
            else
            { BackColor = Color.FromArgb(210, 56, 80); }
        }

        private void btnCan_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CIni m_Ini;
            string sPath = GV.PATH_CONFIG + "PW.cfg";
            string sSec = "";
            string sKey = "";
            string sEng = "";
            string sManger = "";

            m_Ini = new CIni(sPath);
            sSec = "PW";
            sKey = "TECHNICIAN";
            sEng = m_Ini.Read(sSec, sKey);
            sKey = "ENGINEER";
            sManger = m_Ini.Read(sSec, sKey);

            if (CurLevel == ELv.Technician)
            {
                if(txtPw.Text == sEng)
                {
                    if(txtNew.Text == txtConfirm.Text)
                    {
                        sKey = "TECHNICIAN";
                        m_Ini.Write(sSec, sKey, txtNew.Text);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Wrong Confirm Passwaord");
                        return;
                    }
                }
                else
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Wrong Passwaord");
                    return;
                }
            }
            else if (CurLevel == ELv.Engineer)
            {
                if (txtPw.Text == sManger)
                {
                    if (txtNew.Text == txtConfirm.Text)
                    {
                        sKey = "ENGINEER";
                        m_Ini.Write(sSec, sKey, txtNew.Text);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Wrong Confirm Passwaord");
                        return;
                    }
                }
                else
                {
                    CMsg.Show(eMsg.Warning, "Warning", "Wrong Passwaord");
                    return;
                }
            }
            DialogResult = DialogResult.OK;
        }
    }
}
