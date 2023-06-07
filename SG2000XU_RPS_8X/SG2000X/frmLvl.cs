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
    public partial class frmLvl : Form
    {
        public ELv RetLv;

        public frmLvl()
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

            if(CData.CurCompany == ECompany.ASE_K12 && CDataOption.UseCardReader)
            {
                if (CData.Lev < ELv.Engineer)
                {
                    btnAddUser.Visible = false;
                }
            }
            else
            {
                btnAddUser.Visible = false;
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            int iTag = int.Parse(mBtn.Tag.ToString());
            frmPwd mForm = new frmPwd((ELv)iTag);
            mForm.Location = new Point(this.Location.X + 230, Location.Y + 10 + mBtn.Location.Y);

            DialogResult = DialogResult.OK;
            switch (iTag)
            {
                case 0:
                    RetLv     = ELv.Operator;
                    CData.Lev = ELv.Operator;
                    //frmMain.bInitPage = true;
                    CData.bInitPage = true;
                    Close();
                    return;

                case 1:
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        RetLv     = ELv.Technician;
                        CData.Lev = ELv.Technician;
                        Close();
                    }
                    break;

                case 2:
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        if(CData.CurLevel == ELv.Master)
                        {
                            RetLv     = ELv.Master;
                            CData.Lev = ELv.Master;
                            CData.CurLevel = ELv.Operator;
                            Close();
                        }
                        else
                        {
                            RetLv     = ELv.Engineer;
                            CData.Lev = ELv.Engineer;
                            Close();
                        }
                    }
                    break;
            }
            CData.bInitPage = true;
            CBcr.It.SaveStatus(false);
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (CCR.It.bUserForm) return;//이미 Form이 Open 되어 있는 경우 return;
            CData.bLastClick = true;
            frmUser mForm = new frmUser();

            mForm.OpenForm();
            mForm.Show();
        }
    }
}
