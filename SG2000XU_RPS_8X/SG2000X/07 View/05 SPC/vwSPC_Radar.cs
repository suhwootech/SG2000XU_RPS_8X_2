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
    public partial class vwSPC_Radar : Form
    {
        private int iLastSelectRow = 0;

        private enum eHeader
        {
            CODE = 0,
            RADAR_FUNCTION,
            CONTENT,
            SET_DATA_COUNT,
            CURRENT_DATA_COUNT,
        }

        public vwSPC_Radar()
        {
            InitializeComponent();

            dgv_RadarList.Font = new Font("Gulim", 9, FontStyle.Bold);
            
            dgv_RadarList.Columns[(int)eHeader.CODE].HeaderText = "Code";
            dgv_RadarList.Columns[(int)eHeader.RADAR_FUNCTION].HeaderText = "Radar\nFunction";
            dgv_RadarList.Columns[(int)eHeader.CONTENT].HeaderText = "Content";
            dgv_RadarList.Columns[(int)eHeader.SET_DATA_COUNT].HeaderText = "Set Data\nCount";
            dgv_RadarList.Columns[(int)eHeader.CURRENT_DATA_COUNT].HeaderText = "Current Data\nCount";

            dgv_RadarList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv_RadarList.Columns[(int)eHeader.CODE].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_RadarList.Columns[(int)eHeader.RADAR_FUNCTION].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_RadarList.Columns[(int)eHeader.CONTENT].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_RadarList.Columns[(int)eHeader.SET_DATA_COUNT].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_RadarList.Columns[(int)eHeader.CURRENT_DATA_COUNT].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            CErr.LoadRadar();

            UpdateRadarList(true);
        }

        public void UpdateRadarList(bool bInit = false, bool bResetCount = false)
        {
            if (bInit == true)
            {
                dgv_RadarList.Rows.Clear();
            }

            for (int iRow = 0; iRow < CData.ErrList.Length; iRow++)
            {
                if (bInit == true)
                {
                    dgv_RadarList.Rows.Add();
                    dgv_RadarList.Rows[iRow].Resizable = DataGridViewTriState.False;
                }

                if (bResetCount == true)
                {
                    CData.ErrList[iRow].iRadarErrorCnt = 0;
                }

                dgv_RadarList[(int)eHeader.CODE, iRow].Value = CData.ErrList[iRow].sNo;
                dgv_RadarList[(int)eHeader.RADAR_FUNCTION, iRow].Value = CData.ErrList[iRow].bRadarUse;
                dgv_RadarList[(int)eHeader.CONTENT, iRow].Value = CData.ErrList[iRow].sName;
                dgv_RadarList[(int)eHeader.SET_DATA_COUNT, iRow].Value = CData.ErrList[iRow].iRadarOptionCnt; // Radar Option Count
                dgv_RadarList[(int)eHeader.CURRENT_DATA_COUNT, iRow].Value = CData.ErrList[iRow].iRadarErrorCnt;// Radar Error Count

                if (Equals(dgv_RadarList[(int)eHeader.RADAR_FUNCTION, iRow].Value, true))
                    //&& Convert.ToInt32(dgv_RadarList[(int)eHeader.SET_DATA_COUNT, iRow].Value) > 0)
                {
                    dgv_RadarList[(int)eHeader.RADAR_FUNCTION, iRow].Style.BackColor = Color.ForestGreen;
                    dgv_RadarList[(int)eHeader.CONTENT, iRow].Style.BackColor = Color.Yellow;
                    if (Convert.ToInt32(dgv_RadarList[(int)eHeader.SET_DATA_COUNT, iRow].Value) > 0)
                    {
                        dgv_RadarList[(int)eHeader.SET_DATA_COUNT, iRow].Style.BackColor = Color.Yellow;
                    }
                }
                else
                {
                    dgv_RadarList[(int)eHeader.RADAR_FUNCTION, iRow].Style.BackColor = Color.White;
                    dgv_RadarList[(int)eHeader.CONTENT, iRow].Style.BackColor = Color.White;
                    dgv_RadarList[(int)eHeader.SET_DATA_COUNT, iRow].Style.BackColor = Color.White;
                }
            }
        }

        public void SetRadarList()
        {
            for (int iRow = 0; iRow < CData.ErrList.Length; iRow++)
            {
                if (Equals(dgv_RadarList[(int)eHeader.RADAR_FUNCTION, iRow].Value, true))
                {
                    CData.ErrList[iRow].bRadarUse = true;
                }
                else
                {
                    CData.ErrList[iRow].bRadarUse = false;
                }

                CData.ErrList[iRow].iRadarOptionCnt = Convert.ToInt32(dgv_RadarList[(int)eHeader.SET_DATA_COUNT, iRow].Value); // Radar Option Count

                //if (CData.ErrList[iRow].bRadarUse == true && CData.ErrList[iRow].iRadarOptionCnt == 0)
                //{
                //    CData.ErrList[iRow].iRadarOptionCnt = 1;
                //}

                if (CData.ErrList[iRow].iRadarErrorCnt >= CData.ErrList[iRow].iRadarOptionCnt)
                {
                    CData.ErrList[iRow].iRadarErrorCnt = 0;  // Radar Error Count
                }
                else
                {
                    CData.ErrList[iRow].iRadarErrorCnt = Convert.ToInt32(dgv_RadarList[(int)eHeader.CURRENT_DATA_COUNT, iRow].Value);  // Radar Error Count
                }
            }
        }

        private void dgv_RadarList_CurrentCeelDirtyStateChanged(object sender, EventArgs e)
        {
            SetRadarList();
            UpdateRadarList();
        }

        private void dgv_RadarList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(dgv_RadarList_KeyPress);
        }

        private void dgv_RadarList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void vwSPC_Radar_FormClosing(object sender, FormClosingEventArgs e)
        {
            CErr.LoadRadar();
        }

        private void btn_Page_Click(object sender, EventArgs e)
        {
            Button mbtn = sender as Button;

            int iTag = Convert.ToInt32(mbtn.Tag);
            int iDisplayColumn = (dgv_RadarList.Size.Height - dgv_RadarList.ColumnHeadersHeight) / dgv_RadarList.RowTemplate.Height - 1;

            if (iTag < iLastSelectRow)
            {
                iDisplayColumn = 0;
            }

            dgv_RadarList.Rows[iTag + iDisplayColumn].Cells[0].Selected = true;
            dgv_RadarList.Rows[iTag].Cells[0].Selected = true;
        }

        private void dgv_RadarList_SelectionChanged(object sender, EventArgs e)
        {
            iLastSelectRow = dgv_RadarList.SelectedRows[0].Index;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SetRadarList();
            CErr.SaveRadar();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            CErr.LoadRadar();
            UpdateRadarList();
        }

        private void btn_ResetCount_Click(object sender, EventArgs e)
        {
            UpdateRadarList(false, true);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
