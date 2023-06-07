using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm_4ConU : UserControl
    {
        private EWay m_eWy;
        private int m_iWy;
        private DataGridViewCellStyle m_mStUse = new DataGridViewCellStyle() { BackColor = Color.Lime };
        private DataGridViewCellStyle m_mStW1 = new DataGridViewCellStyle() { BackColor = Color.White };
        private DataGridViewCellStyle m_mStW2 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
        private DataGridViewCellStyle m_mStW3 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(235, 235, 235) };
        private DataGridViewCellStyle m_mStW4 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(225, 225, 225) };
        private DataGridViewCellStyle m_mStW5 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(215, 215, 215) };

        public vwDev_02Prm_4ConU(EWay eWy)
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

            m_eWy = eWy;
            m_iWy = (int)eWy;

            if (m_eWy == EWay.R)
            {
                txtP_CtLChipC   .Enabled = false;
                txtP_CtLChipR   .Enabled = false;
                txtP_CtLChipW   .Enabled = false;
                txtP_CtLChipH   .Enabled = false;
                txt_Unit1.Enabled = false;
                txt_Unit2.Enabled = false;
                txt_Unit3.Enabled = false;
                txt_Unit4.Enabled = false;

            }
            else
            {
                txtP_CtLChipC   .Enabled = true;
                txtP_CtLChipR   .Enabled = true;
                txtP_CtLChipW   .Enabled = true;
                txtP_CtLChipH   .Enabled = true;
                txt_Unit1.Enabled = true;
                txt_Unit2.Enabled = true;
                txt_Unit3.Enabled = true;
                txt_Unit4.Enabled = true;

                txtP_CtLBfLit   .Enabled = true;
                txtP_CtLAfLit   .Enabled = true;
                txtP_CtLone     .Enabled = true;
            }
        }

        #region Private method
        /// <summary>
        /// Contact 좌표 및 측정 여부 값 획득
        /// </summary>
        private void _GetCont()
        {
            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;
            double dPtX = CData.Dev.dChipW / iCol;  //측정 포지션의 가로 길이
            double dPtY = CData.Dev.dChipH / iRow;  //측정 포지션의 세로 길이

            if (m_eWy == EWay.L || (m_eWy == EWay.R && CData.Dev.bDual == eDual.Dual))
            {
                CData.Dev.aData[m_iWy].aPosBf = new tCont[iRow, iCol];
                CData.Dev.aData[m_iWy].aPosAf = new tCont[iRow, iCol];

                if (m_eWy == EWay.L && CData.Dev.bDual == eDual.Normal)
                {
                    CData.Dev.aData[1].aPosBf = new tCont[iRow, iCol];
                    CData.Dev.aData[1].aPosAf = new tCont[iRow, iCol];
                }

                for (int i = 0; i < iCol; i++) // Column
                {
                    for (int j = 0; j < iRow; j++)  // Row
                    {
                        tCont tConL = new tCont();
                        tConL.aY = new double[GV.PKG_MAX_UNIT];
                        tCont tConR = new tCont();
                        tConR.aY = new double[GV.PKG_MAX_UNIT];

                        for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++) // Window
                        {
                            // 현재 유닛의 중앙 좌표
                            double dLCenX = CData.SPos.dGRD_X_Zero[(int)EWay.L];
                            double dLCenY = CData.MPos[(int)EWay.L].dY_PRB_TBL_CENTER + CData.Dev.aUnitCen[iU];
                            double dRCenX = CData.SPos.dGRD_X_Zero[(int)EWay.R];
                            double dRCenY = CData.MPos[(int)EWay.R].dY_PRB_TBL_CENTER + CData.Dev.aUnitCen[iU];

                            // 현재 유닛의 [0,0 ] 위치 값
                            double dLx = (iCol % 2 == 0) ? dLCenX - (((iCol / 2) - 1) * dPtX) - (dPtX / 2) : dLCenX - ((iCol / 2) * dPtX);
                            double dLy = (iRow % 2 == 0) ? dLCenY - (((iRow / 2) - 1) * dPtY) - (dPtY / 2) : dLCenY - ((iRow / 2) * dPtY);
                            double dRx = (iCol % 2 == 0) ? dRCenX + (((iCol / 2) - 1) * dPtX) - (dPtX / 2) : dRCenX + ((iCol / 2) * dPtX);
                            double dRy = (iRow % 2 == 0) ? dRCenY - (((iRow / 2) - 1) * dPtY) - (dPtY / 2) : dRCenY - ((iRow / 2) * dPtY);

                            tConL.dX = Math.Round(dLx + (i * dPtX) + (dPtX / 2), 6);
                            tConL.dY = Math.Round(dLy + (j * dPtY) + (dPtY / 2), 6);
                            tConL.aY[iU] = Math.Round(dLy + (j * dPtY) + (dPtY / 2), 6);
                            tConR.dX = Math.Round(dRx - (i * dPtX) + (dPtX / 2), 6);
                            tConR.dY = Math.Round(dRy + (j * dPtY) + (dPtY / 2), 6);
                            tConR.aY[iU] = Math.Round(dRy + (j * dPtY) + (dPtY / 2), 6);

                            // Before
                            if (m_eWy == EWay.L)
                            {
                                if (dgvP_CtLBf.Rows[j].Cells[i].Style.BackColor == Color.Lime)
                                { tConL.bUse = true; }
                                else
                                { tConL.bUse = false; }
                                CData.Dev.aData[m_iWy].aPosBf[j, i] = tConL;

                                if (CData.Dev.bDual == eDual.Normal)
                                {
                                    tConR.bUse = tConL.bUse;
                                    CData.Dev.aData[(int)EWay.R].aPosBf[j, i] = tConR;
                                }
                            }
                            else
                            {
                                if (dgvP_CtLBf.Rows[j].Cells[i].Style.BackColor == Color.Lime)
                                { tConR.bUse = true; }
                                else
                                { tConR.bUse = false; }
                                CData.Dev.aData[m_iWy].aPosBf[j, i] = tConR;
                            }


                            // After
                            if (m_eWy == EWay.L)
                            {
                                if (dgvP_CtLAf.Rows[j].Cells[i].Style.BackColor == Color.Lime)
                                { tConL.bUse = true; }
                                else
                                { tConL.bUse = false; }
                                CData.Dev.aData[m_iWy].aPosAf[j, i] = tConL;

                                if (CData.Dev.bDual == eDual.Normal)
                                {
                                    tConR.bUse = tConL.bUse;
                                    CData.Dev.aData[1].aPosAf[j, i] = tConR;
                                }
                            }
                            else
                            {
                                if (dgvP_CtLAf.Rows[j].Cells[i].Style.BackColor == Color.Lime)
                                { tConR.bUse = true; }
                                else
                                { tConR.bUse = false; }
                                CData.Dev.aData[m_iWy].aPosAf[j, i] = tConR;
                            }
                        }
                    } // Row for
                } // Column for
            }
        }

        private void _DrawCont()
        {
            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;
            int iCnt = 1;
            int iHei = 0;
            if (iRow != 0)
            {
                iHei = (dgvP_CtLBf.Height - 5) / iRow;
            }

            dgvP_CtLBf.Visible = false;
            dgvP_CtLAf.Visible = false;

            dgvP_CtLBf.Rows.Clear();
            dgvP_CtLBf.Columns.Clear();
            dgvP_CtLAf.Rows.Clear();
            dgvP_CtLAf.Columns.Clear();

            for (int i = 0; i < iCol; i++)
            {
                dgvP_CtLBf.Columns.Add(i.ToString(), i.ToString());
                dgvP_CtLBf.Columns[i].Resizable = DataGridViewTriState.False;
                dgvP_CtLAf.Columns.Add(i.ToString(), i.ToString());
                dgvP_CtLAf.Columns[i].Resizable = DataGridViewTriState.False;
            }

            dgvP_CtLBf.RowTemplate.Height = iHei;
            dgvP_CtLBf.RowTemplate.Resizable = DataGridViewTriState.False;
            dgvP_CtLAf.RowTemplate.Height = iHei;
            dgvP_CtLAf.RowTemplate.Resizable = DataGridViewTriState.False;

            for (int k = 0; k < iCnt; k++)
            {
                int iColor = 255 - (k * 10);
                Color tColor = Color.FromArgb(iColor, iColor, iColor);

                for (int j = 0; j < iRow; j++)
                {
                    dgvP_CtLBf.Rows.Add();
                    dgvP_CtLBf.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                    dgvP_CtLAf.Rows.Add();
                    dgvP_CtLAf.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                }
            }

            int iRowMin = Math.Min(iRow * iCnt, CData.Dev.aData[0].aPosBf.GetLength(0));
            int iColMin = Math.Min(iCol, CData.Dev.aData[0].aPosBf.GetLength(1));

            for (int i = 0; i < iColMin; i++)
            {
                for (int j = 0; j < iRowMin; j++)
                {
                    if (CData.Dev.aData[m_iWy].aPosBf[j, i].bUse)
                    { dgvP_CtLBf.Rows[j].Cells[i].Style.ApplyStyle(m_mStUse); }
                    if (CData.Dev.aData[m_iWy].aPosAf[j, i].bUse)
                    { dgvP_CtLAf.Rows[j].Cells[i].Style.ApplyStyle(m_mStUse); }
                }
            }

            dgvP_CtLBf.ClearSelection();
            dgvP_CtLAf.ClearSelection();

            dgvP_CtLBf.Visible = true;
            dgvP_CtLAf.Visible = true;
        }

        /// <summary>
        /// Manual view에 조작 로그 저장 함수
        /// </summary>
        /// <param name="sMsg"></param>
        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.OPL, string.Format("{0}.cs {1}() {2} Lv:{3}", sCls, sMth, sMsg, CData.Lev));
        }

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        private void SetWriteLanguage(string controlName, string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            // 2020.08.31 JSKim St
            if (CDataOption.Is1Point == false)
            {
                tabControl2.TabPages.Remove(tpP_CtLOnePos);
            }
            // 2020.08.31 JSKim Ed

            if (CData.Dev.iUnitCnt == 4)
            {
                lbl_Unit3_T.Visible = true;
                txt_Unit3.Visible = true;
                lbl_Unit3_U.Visible = true;
                lbl_Unit4_T.Visible = true;
                txt_Unit4.Visible = true;
                lbl_Unit4_U.Visible = true;
            }
            else
            {
                lbl_Unit3_T.Visible = false;
                txt_Unit3.Visible   = false;
                lbl_Unit3_U.Visible = false;
                lbl_Unit4_T.Visible = false;
                txt_Unit4.Visible   = false;
                lbl_Unit4_U.Visible = false;
            }

            Set();
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// 데이터를 화면에 출력
        /// </summary>
        public void Set()
        {
            txtP_CtLChipC   .Text = CData.Dev.iCol     .ToString();
            txtP_CtLChipR   .Text = CData.Dev.iRow     .ToString();
            txtP_CtLChipW   .Text = CData.Dev.dChipW   .ToString();
            txtP_CtLChipH   .Text = CData.Dev.dChipH   .ToString();
            for (int i = 0; i < CData.Dev.iUnitCnt; i++)
            {
                Controls.Find("txt_Unit" + (i + 1), true)[0].Text = CData.Dev.aUnitCen[i].ToString();
            }
            txtP_CtLBfLit   .Text = CData.Dev.aData[m_iWy].dBfLimit.ToString();
            txtP_CtLAfLit   .Text = CData.Dev.aData[m_iWy].dAfLimit.ToString();
            txtP_CtLone     .Text = CData.Dev.aData[m_iWy].dTTV.ToString();
            txtP_CtLOneLit  .Text = CData.Dev.aData[m_iWy].dOneLimit.ToString();//191018 ksg :
            txtP_CtLOneOver .Text = CData.Dev.aData[m_iWy].dOneOver.ToString();    // 2020.08.19 JSKim

            // 2020.08.31 JSKim St
            ckb_OnePointXPosFix.Checked = CData.Dev.aData[m_iWy].bOnePointXPosFix;
            txtP_CtLChipOneCol.Text = CData.Dev.aData[m_iWy].iOnePointCol.ToString();
            txtP_CtLChipOneRow.Text = CData.Dev.aData[m_iWy].iOnePointRow.ToString();
            // 2020.08.31 JSKim Ed

            _DrawCont();
            _DrawOnePoint();    // 2020.08.31 JSKim
        }

        public void Get()
        {
            // 2020.08.31 JSKim St
            if (_CheckParaChange())
            {
                txtP_CtLChipOneCol.Text = "0";
                txtP_CtLChipOneRow.Text = "0";
            }
            // 2020.08.31 JSKim Ed

            if (m_eWy == EWay.L)
            {
                int   .TryParse(txtP_CtLChipC   .Text, out CData.Dev.iCol     );
                int   .TryParse(txtP_CtLChipR   .Text, out CData.Dev.iRow     );
                double.TryParse(txtP_CtLChipW   .Text, out CData.Dev.dChipW   );
                double.TryParse(txtP_CtLChipH   .Text, out CData.Dev.dChipH   );
                CData.Dev.iWinCnt = 1;
                for (int i = 0; i < CData.Dev.iUnitCnt; i++)
                {
                     double.TryParse(Controls.Find("txt_Unit" + (i + 1), true)[0].Text, out CData.Dev.aUnitCen[i]);
                }
            }

            double.TryParse(txtP_CtLBfLit.Text, out CData.Dev.aData[m_iWy].dBfLimit);
            double.TryParse(txtP_CtLAfLit.Text, out CData.Dev.aData[m_iWy].dAfLimit);
            double.TryParse(txtP_CtLone.Text, out CData.Dev.aData[m_iWy].dTTV);
            double.TryParse(txtP_CtLOneLit.Text, out CData.Dev.aData[m_iWy].dOneLimit);
            double.TryParse(txtP_CtLOneOver.Text, out CData.Dev.aData[m_iWy].dOneOver); // 2020.08.19 JSKim
            // 2020.08.31 JSKim St
            CData.Dev.aData[m_iWy].bOnePointXPosFix = ckb_OnePointXPosFix.Checked;
            int.TryParse(txtP_CtLChipOneCol.Text, out CData.Dev.aData[m_iWy].iOnePointCol);
            int.TryParse(txtP_CtLChipOneRow.Text, out CData.Dev.aData[m_iWy].iOnePointRow);
            // 2020.08.31 JSKim End

            //200424 jym : 노말 모드에서는 좌측 데이터 우측에 복사
            if (CData.Dev.bDual == eDual.Normal)
            {
                CData.Dev.aData[(int)EWay.R].dBfLimit = CData.Dev.aData[(int)EWay.L].dBfLimit;
                CData.Dev.aData[(int)EWay.R].dAfLimit = CData.Dev.aData[(int)EWay.L].dAfLimit;
                CData.Dev.aData[(int)EWay.R].dTTV = CData.Dev.aData[(int)EWay.L].dTTV;
                CData.Dev.aData[(int)EWay.R].dOneLimit = CData.Dev.aData[(int)EWay.L].dOneLimit;
                CData.Dev.aData[(int)EWay.R].dOneOver = CData.Dev.aData[(int)EWay.L].dOneOver;  // 2020.08.19 JSKim
                // 2020.08.31 JSKim St
                CData.Dev.aData[(int)EWay.R].bOnePointXPosFix = CData.Dev.aData[(int)EWay.L].bOnePointXPosFix;
                CData.Dev.aData[(int)EWay.R].iOnePointCol = CData.Dev.aData[(int)EWay.L].iOnePointCol;
                CData.Dev.aData[(int)EWay.R].iOnePointRow = CData.Dev.aData[(int)EWay.L].iOnePointRow;
                // 2020.08.31 JSKim Ed
            }

            _DrawCont();
            _DrawOnePoint();    // 2020.08.31 JSKim
            _GetCont();
        }
        #endregion

        private void LeftTbProbeUseCopy(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;

            int Select = Convert.ToInt32(mBtn.Tag);

            if (Select == 0)
            {
                Array.Copy(CData.Dev.aData[m_iWy].aPosBf, CData.Dev.aData[m_iWy].aPosAf, CData.Dev.aData[m_iWy].aPosBf.Length);
            }

            if (Select == 1)
            {
                Array.Copy(CData.Dev.aData[m_iWy].aPosAf, CData.Dev.aData[m_iWy].aPosBf, CData.Dev.aData[m_iWy].aPosAf.Length);
            }

            _DrawCont();
        }

        private void cmsP_Cont_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            DataGridView mDgv = cmsP_Cont.SourceControl as DataGridView;
            int iBf = int.Parse(mDgv.Tag.ToString());
            bool bVal = true;

            foreach (DataGridViewCell mCell in mDgv.SelectedCells)
            {
                if (e.ClickedItem.Text == "Use")
                {
                    mCell.Style.ApplyStyle(m_mStUse);
                    bVal = true;
                }
                else
                {
                    if (mCell.RowIndex < CData.Dev.iRow)
                    { mCell.Style.ApplyStyle(m_mStW1); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 2)
                    { mCell.Style.ApplyStyle(m_mStW2); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 3)
                    { mCell.Style.ApplyStyle(m_mStW3); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 4)
                    { mCell.Style.ApplyStyle(m_mStW4); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 5)
                    { mCell.Style.ApplyStyle(m_mStW5); }
                    bVal = false;
                }

                if (iBf == 0)
                {
                    CData.Dev.aData[m_iWy].aPosBf[mCell.RowIndex, mCell.ColumnIndex].bUse = bVal;
                }
                else
                {
                    CData.Dev.aData[m_iWy].aPosAf[mCell.RowIndex, mCell.ColumnIndex].bUse = bVal;
                }
            }
        }

        // 2020.08.31 JSKim St
        private void cmsP_One_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;

            DataGridView mDgv = cmsP_One.SourceControl as DataGridView;
            //int iBf = int.Parse(mDgv.Tag.ToString());

            foreach (DataGridViewCell mCell in mDgv.SelectedCells)
            {
                if (e.ClickedItem.Text == "Select")
                {
                    txtP_CtLChipOneCol.Text = (mCell.ColumnIndex + 1).ToString();
                    txtP_CtLChipOneRow.Text = (mCell.RowIndex % iRow + 1).ToString();
                }
            }

            _DrawOnePoint();
        }

        /// <summary>
        /// Chip 배열 변경시 One Point Measure 측정 Chip 선택 해제
        /// </summary>
        /// <returns>-true=Chip 배열 변경, false=Chip 배열 미 변경</returns>
        private bool _CheckParaChange()
        {
            bool bRet = false;

            if (int.Parse(txtP_CtLChipC.Text) != CData.Dev.iCol
                || int.Parse(txtP_CtLChipR.Text) != CData.Dev.iRow)
            {
                bRet = true;
            }

            return bRet;
        }

        private void _DrawOnePoint()
        {
            if (CDataOption.Is1Point == false)
            { return; }

            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;
            int iCnt = 1;
            int iTotalRow = iRow * iCnt;

            int iHei = 0;
            if (iRow != 0)
            {
                iHei = (dgvP_CtLOne.Height - 5) / iRow;
            }

            dgvP_CtLOne.Visible = false;

            dgvP_CtLOne.Rows.Clear();
            dgvP_CtLOne.Columns.Clear();

            for (int i = 0; i < iCol; i++)
            {
                dgvP_CtLOne.Columns.Add(i.ToString(), i.ToString());
                dgvP_CtLOne.Columns[i].Resizable = DataGridViewTriState.False;
            }

            dgvP_CtLOne.RowTemplate.Height = iHei;
            dgvP_CtLOne.RowTemplate.Resizable = DataGridViewTriState.False;

            for (int k = 0; k < iCnt; k++)
            {
                int iColor = 255 - (k * 10);
                Color tColor = Color.FromArgb(iColor, iColor, iColor);

                for (int j = 0; j < iRow; j++)
                {
                    dgvP_CtLOne.Rows.Add();
                    dgvP_CtLOne.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                }
            }

            int iSelectCol = int.Parse(txtP_CtLChipOneCol.Text);
            int iSelectRow = int.Parse(txtP_CtLChipOneRow.Text);

            if (iCol == 0 || iRow == 0 || iSelectCol > iCol || iSelectRow > iTotalRow)
            {
                txtP_CtLChipOneCol.Text = "0";
                txtP_CtLChipOneRow.Text = "0";
            }

            if (iSelectCol == 0 || iSelectRow == 0 || txtP_CtLChipOneCol.Text == "0" || txtP_CtLChipOneRow.Text == "0")
            {
                txtP_CtLOneXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                txtP_CtLOneYPos.Text = "0.000";
            }
            else
            {
                if (ckb_OnePointXPosFix.Checked)
                {
                    txtP_CtLOneXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                }
                else
                {
                    if (m_eWy == EWay.L)
                    {
                        txtP_CtLOneXPos.Text = CData.Dev.aData[m_iWy].aPosBf[0, (iSelectCol - 1)].dX.ToString("0.000");
                    }
                    else
                    {
                        txtP_CtLOneXPos.Text = (-CData.Dev.aData[m_iWy].aPosBf[0, (iSelectCol - 1)].dX).ToString("0.000");
                    }
                }
                txtP_CtLOneYPos.Text = (-(CData.Dev.aData[m_iWy].aPosBf[(iSelectRow - 1), 0].aY[0] - CData.MPos[m_iWy].dY_PRB_TBL_CENTER - CData.Dev.aUnitCen[0])).ToString("0.000");
            }

            int iRowMin = Math.Min(iRow * iCnt, CData.Dev.aData[m_iWy].aPosBf.GetLength(0));
            int iColMin = Math.Min(iCol, CData.Dev.aData[m_iWy].aPosBf.GetLength(1));

            for (int i = 0; i < iColMin; i++)
            {
                for (int j = 0; j < iRowMin; j++)
                {
                    if (iSelectRow != 0 && iSelectCol != 0)
                    {
                        if (i == (iSelectCol - 1) && j == (iSelectRow - 1))
                        {
                            dgvP_CtLOne.Rows[j].Cells[i].Style.ApplyStyle(m_mStUse);
                        }
                    }
                }
            }

            dgvP_CtLOne.ClearSelection();
            dgvP_CtLOne.Visible = true;
        }

        private void OnePointDefault(object sender, EventArgs e)
        {
            txtP_CtLChipOneCol.Text = "0";
            txtP_CtLChipOneRow.Text = "0";

            _DrawOnePoint();
        }

        private void OnePointXPosFix(object sender, EventArgs e)
        {
            _DrawOnePoint();
        }
        // 2020.08.31 JSKim Ed
    }
}
