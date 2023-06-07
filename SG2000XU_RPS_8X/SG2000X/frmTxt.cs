using System;
using System.IO;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmTxt : Form
    {
        public string Val { get; private set; }

        public frmTxt(string sTitle)
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

            lbl_Title.Text = sTitle;
            txt_Value.Select();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            
            Val = txt_Value.Text;
            if (Val == string.Empty || Val == null)
            {
                //tipName.SetToolTip(txt_Value, "New Group Name is Empty");
                tipName.Show("Value Empty", txt_Value, 5000);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btn_Can_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // 2021.06.07 lhs Start : 입력 폼에 Text 입력
        public void SetValText(string sVal)
        {
            txt_Value.Text = sVal;
		}
        // 2021.06.07 lhs End

    }
}
