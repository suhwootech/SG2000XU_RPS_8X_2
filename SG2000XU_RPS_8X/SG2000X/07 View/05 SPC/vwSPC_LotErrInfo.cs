using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SG2000X
{
    public partial class vwSPC_LotErrInfo : Form
    {
        private static CTim m_Kill = new CTim();
        private static string m_sErrPath = "";
        private static string m_sInfoPath = "";
        private static List<string> sRelease = new List<string>();

        private enum eHeader
        {
            CODE = 0,
            ERR_MESSAGE,
            COUNT,
            TIME,
        }

        public vwSPC_LotErrInfo(string sErrPath, string sInfoPath)
        {
            InitializeComponent();

            sRelease = new List<string>();

            m_sErrPath = sErrPath;
            m_sInfoPath = sInfoPath;

            dgv_LotErrorList.Font = new Font("Gulim", 9, FontStyle.Bold);

            dgv_LotErrorList.Columns[(int)eHeader.CODE].HeaderText = "Code";
            dgv_LotErrorList.Columns[(int)eHeader.ERR_MESSAGE].HeaderText = "Err Message";
            dgv_LotErrorList.Columns[(int)eHeader.COUNT].HeaderText = "Count";
            dgv_LotErrorList.Columns[(int)eHeader.TIME].HeaderText = "Time";

            dgv_LotErrorList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv_LotErrorList.Columns[(int)eHeader.CODE].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_LotErrorList.Columns[(int)eHeader.ERR_MESSAGE].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv_LotErrorList.Columns[(int)eHeader.COUNT].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_LotErrorList.Columns[(int)eHeader.TIME].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            UpdateLotErrorList(m_sErrPath);
        }

        private void UpdateLotErrorList(string sPath)
        {
            FileInfo fi;
            StreamReader sr;

            string sLine = "";
            string[] sData;

            fi = new FileInfo(sPath);

            if (fi.Exists)
            {
                try
                {
                    string filename = Path.GetFileName(sPath);
                    if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                    sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                    while (sr.Peek() > -1)
                    {
                        sLine = sr.ReadLine();
                        sRelease.Add(sLine);
                    }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                    CMsg.Show(eMsg.Error, "Error_Lot", ex.Message);
                    return;
                }

                //sRelease.RemoveAt(0);

                dgv_LotErrorList.Rows.Clear();

                for (int i = 0; i < sRelease.Count - 1; i++)
                {
                    sData = sRelease[i + 1].Split(',');

                    dgv_LotErrorList.Rows.Add();
                    dgv_LotErrorList.Rows[i].Resizable = DataGridViewTriState.False;

                    dgv_LotErrorList[(int)eHeader.CODE, i].Value = sData[0];
                    dgv_LotErrorList[(int)eHeader.ERR_MESSAGE, i].Value = sData[1];
                    dgv_LotErrorList[(int)eHeader.COUNT, i].Value = sData[2];
                    dgv_LotErrorList[(int)eHeader.TIME, i].Value = sData[3];
                }

                dgv_LotErrorList.ClearSelection();
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            FileInfo fi;
            StreamReader sr;
            StreamWriter sw;
            string sLine = "";

            sfd_Chart.ShowDialog();

            if (sfd_Chart.FileName != "")
            {
                int nCnt = 0;

                fi = new FileInfo(m_sInfoPath);

                if (fi.Exists)
                {
                    try
                    {
                        string filename = Path.GetFileName(m_sInfoPath);
                        if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                        sr = new StreamReader(m_sInfoPath, Encoding.GetEncoding("euc-kr"));

                        while (sr.Peek() > -1)
                        {
                            sLine = sr.ReadLine();
                            sRelease.Insert(nCnt, sLine);
                            nCnt++;
                        }
                        sr.Close();
                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error_Lot", ex.Message);
                        return;
                    }
                }
                sRelease.Insert(nCnt, "\r\n");

                FileStream FS = null;

                try
                {
                    FS = new FileStream(sfd_Chart.FileName, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(FS, Encoding.GetEncoding("euc-kr"));

                    for (int i = 0; i <= sRelease.Count - 1; i++)
                    {
                        //CLog.Check_File_Access(sfd_Chart.FileName, sRelease[i], true);
                        sw.WriteLine(sRelease[i]);
                    }

                    sw.Close();
                }
                finally
                {
                    if (FS != null)
                    {
                        FS.Dispose();
                    }
                }
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
