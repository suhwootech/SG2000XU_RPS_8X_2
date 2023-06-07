using System;
using System.IO;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMain_Whl : UserControl
    {
        public vwMain_Whl()
        {
            InitializeComponent();
        }

        public void Set()
        {
            string sPath = GV.PATH_WHEEL;

            sPath += "Left\\";
            cmb_WhlL.Items.Clear();
            if (Directory.Exists(sPath))
            {
                DirectoryInfo mFile = new DirectoryInfo(sPath);
                foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                {
                    cmb_WhlL.Items.Add(mInfo.Name);
                }
            }
            else
            { Directory.CreateDirectory(sPath); }
            cmb_WhlL.SelectedItem = CData.Dev.aData[(int)EWay.L].sWhl;

            sPath = GV.PATH_WHEEL + "Right\\";
            cmb_WhlR.Items.Clear();
            if (Directory.Exists(sPath))
            {
                DirectoryInfo mFile = new DirectoryInfo(sPath);
                foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                {
                    cmb_WhlR.Items.Add(mInfo.Name);
                }
            }
            else
            { Directory.CreateDirectory(sPath); }
            cmb_WhlR.SelectedItem = CData.Dev.aData[(int)EWay.R].sWhl;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            CData.Dev.aData[(int)EWay.L].sWhl = cmb_WhlL.SelectedItem.ToString();
            CData.Dev.aData[(int)EWay.R].sWhl = cmb_WhlR.SelectedItem.ToString();

            string sPath = GV.PATH_DEVICE + string.Format("{0}\\{1}.dev", CData.DevGr, CData.Dev.sName);
            CDev.It.Save(sPath);

            CData.VwWhl.bSelc = true;
            CData.VwWhl.bOk = true;
            CData.VwWhl.bCan = false;
            CData.VwWhl.bView = false;
        }

        private void btn_Can_Click(object sender, EventArgs e)
        {
            CData.VwWhl.bSelc = true;
            CData.VwWhl.bOk = false;
            CData.VwWhl.bCan = true;
            CData.VwWhl.bView = false;
        }
    }
}
