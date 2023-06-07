using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SECSGEM;
namespace SG2000X
{
    public partial class frmCL : Form
    {
        frmGEM sgfrmGEM;
        private System.Windows.Forms.Timer m_tmUpdt;

        public frmCL(frmGEM _frmGEM)
        {
            InitializeComponent();

            sgfrmGEM = _frmGEM;

            m_tmUpdt = new System.Windows.Forms.Timer();
            m_tmUpdt.Interval = 1000;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
            m_tmUpdt.Start();
        }

        #region function
        public void ShowGrid(SECSGEM.ManagerLot pMLOT)
        {
            string[] stitle = new string[3];
            int iCnt = pMLOT.Count;

            grdDataView.ColumnCount = 3;
            grdDataView.RowCount = 1;
            stitle[0] = "No";
            stitle[1] = "Carrier ID";
            stitle[2] = "Status";
            grdDataView[0, 0].Value = stitle[0];
            grdDataView[1, 0].Value = stitle[1];
            grdDataView[2, 0].Value = stitle[2];
            grdDataView.RowCount = iCnt + 1;

            for (int i = 0; i < iCnt; i++)
            {
                int iNumber = i + 1;
                int iStatus = 0;
                string[] sdata = new string[3];

                sdata[0] = iNumber.ToString();

                sdata[1] = pMLOT.GetCarrierID(i);

                iStatus = (int)pMLOT.GetCarrierStatus(i);

                if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Idle)
                {
                    sdata[2] = "Not read";
                    grdDataView[2, iNumber].Style.BackColor = Color.Gray;
                }
                else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Read)
                {
                    sdata[2] = "Read";
                    grdDataView[2, iNumber].Style.BackColor = Color.Yellow;
                }
                else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Validate)
                {
                    sdata[2] = "Validation";
                    grdDataView[2, iNumber].Style.BackColor = Color.Blue;
                }
                else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Start)
                {
                    sdata[2] = "Start";
                    grdDataView[2, iNumber].Style.BackColor = Color.Green;
                }
                else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.End)
                {
                    sdata[2] = "End";
                    grdDataView[2, iNumber].Style.BackColor = Color.Aqua;
                }
                else if (iStatus == (int)SECSGEM.JSCK.eCarrierStatus.Reject)
                {
                    sdata[2] = "Reject";
                    grdDataView[2, iNumber].Style.BackColor = Color.Red;
                }

                grdDataView[0, iNumber].Value = sdata[0];
                grdDataView[1, iNumber].Value = sdata[1];
                grdDataView[2, iNumber].Value = sdata[2];
            }

        }
        #endregion function
             
        private void btnHide_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnHide_Click_1(object sender, EventArgs e)
        {
            Close();
        }
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            m_tmUpdt.Stop();
            Hide();
        }
    }
}