using System;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMain_Scn : UserControl
    {
        public vwMain_Scn()
        {
            InitializeComponent();
        }

        public void SetType(bool bDrs)
        {
            if (bDrs)
            {
                lbl_Msg.Text = "After Changing the Dresser, Press 'OK'.";
                btn_Ok.Visible = true;
            }
            else
            {
                lbl_Msg.Text = "Detecting Wheel Jig Now.";
                btn_Ok.Visible = false;
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            CData.bDrsChange = false;
            CData.DrsChgSeq.iStep = 2;
        }
    }
}
