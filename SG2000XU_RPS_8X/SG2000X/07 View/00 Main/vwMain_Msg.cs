using System;
using System.IO;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMain_Msg : UserControl
    {
        public vwMain_Msg()
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
        }

        public void Set()
        {
            lbl_Title.Text = CData.VwMsg.sTitle;
            lbl_Note.Text = CData.VwMsg.sNote;

            if (CData.VwMsg.eType == eMsg.Notice)
            {
                btn_Ok.Location = btn_Can.Location;
                btn_Can.Visible = false;
            }
            else if (CData.VwMsg.eType == eMsg.Warning)
            {
                BackColor = System.Drawing.Color.Orange;
            }
            else if (CData.VwMsg.eType == eMsg.Error)
            {
                BackColor = System.Drawing.Color.Firebrick;
                btn_Ok.Location = btn_Can.Location;
                btn_Can.Visible = false;
            }
            //200712 jhc : 18 포인트 측정 (ASE-KR VOC) - 매뉴얼 Grinding/Strip Measure 시 18 Point 진행여부 질의를 위해 추가
            else if (CData.VwMsg.eType == eMsg.Query)
            {
                btn_Ok.Text = "YES";
                btn_Can.Text = "NO";
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            CData.VwMsg.bSelc = true;
            CData.VwMsg.bOk = true;
            CData.VwMsg.bCan = false;
            CData.VwMsg.bView = false;
        }

        private void btn_Can_Click(object sender, EventArgs e)
        {
            CData.VwMsg.bSelc = true;
            CData.VwMsg.bOk = false;
            CData.VwMsg.bCan = true;
            CData.VwMsg.bView = false;
        }
    }
}
