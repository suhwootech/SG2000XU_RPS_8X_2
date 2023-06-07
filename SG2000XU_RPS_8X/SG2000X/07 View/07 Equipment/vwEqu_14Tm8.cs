using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_14Tm8 : UserControl
    {
        public enum EEditMode
        {
            None,
            Edit
        }

        private DataGridViewCellStyle m_mStUse = new DataGridViewCellStyle() { BackColor = Color.Lime };
        private DataGridViewCellStyle m_mStW1 = new DataGridViewCellStyle() { BackColor = Color.White };

        CTableInfo SelTi = new CTableInfo();
        EEditMode m_eEditMode = EEditMode.None;

        private Timer m_tmUpdt;
        

        public vwEqu_14Tm8()
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

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
        }


        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            lbl_L_XPos.Text = "LT_X = " + CMot.It.Get_FP((int)EAx.LeftGrindZone_X).ToString();
            lbl_L_YPos.Text = "LT_Y = " + CMot.It.Get_FP((int)EAx.LeftGrindZone_Y).ToString();
            lbl_L_ZPos.Text = "LT_Z = " + CMot.It.Get_FP((int)EAx.LeftGrindZone_Z).ToString();

            lbl_R_XPos.Text = "RT_X = " + CMot.It.Get_FP((int)EAx.RightGrindZone_X).ToString();
            lbl_R_YPos.Text = "RT_Y = " + CMot.It.Get_FP((int)EAx.RightGrindZone_Y).ToString();
            lbl_R_ZPos.Text = "RT_Z = " + CMot.It.Get_FP((int)EAx.RightGrindZone_Z).ToString();

            DisplayResult(0);   // left
            DisplayResult(1);   // right
        }

        public void _EnableBtn(bool bVal)
        {
            panelTableInfo.Enabled = bVal;

            btnLeftStart.Enabled = bVal;
            btnRightStart.Enabled = bVal;


        }


        public void Open()
        {
            // Table 정보 로딩 및 디스플레이 
            _Set();

            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled) { m_tmUpdt.Start(); }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled) { m_tmUpdt.Stop(); }
        }

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
            // 타이머에 대한 모든 리소스 해제
            if (m_tmUpdt != null)
            {
                Close();
                m_tmUpdt.Dispose();
                m_tmUpdt = null;
            }
        }

        /// <summary>
        /// 데이터를 화면에 출력
        /// </summary>
        private void _Set()
        {
            // Table 정보 로딩
            int nRet = CTm8.It.Load();
            if (nRet != 0)
                return;

            // Select Table combobox 리스트 추가
            comboTableItemAdd();
            comboTable.SelectedIndex = CTm8.It.m_nSelectedTable;
            SelTi = CTm8.It.m_alTableInfo[CTm8.It.m_nSelectedTable] as CTableInfo;

            // 선택된 Table 정보 표시
            DisplayTableInfo();

            m_eEditMode = EEditMode.None;
            EnableComponent();
            
        }

        /// <summary>
        /// 화면에 값을 데이터로 저장
        /// </summary>
        private void _Get()
        {
            //--------------------------
            // 편집 모드
            //--------------------------
            if (m_eEditMode == EEditMode.Edit)
            {
                //--------------------------
                // 화면 -> Table정보
                //--------------------------
                // TextBox 부분
                SelTi.sName = txtName.Text.Trim();
                SelTi.sComment = txtComment.Text.Trim();
                double.TryParse(txtTableShort.Text, out SelTi.dShortLength);
                double.TryParse(txtTableLong.Text, out SelTi.dLongLength);
                int.TryParse(txtCol.Text, out SelTi.nDivCol);
                int.TryParse(txtRow.Text, out SelTi.nDivRow);


                int.TryParse(txtLTMeasCnt.Text, out SelTi.aTbl_MeasCnt[0]);
                int.TryParse(txtRTMeasCnt.Text, out SelTi.aTbl_MeasCnt[1]);
                int.TryParse(txtLTRepeatCnt.Text, out SelTi.aTbl_RepeatCnt[0]);
                int.TryParse(txtRTRepeatCnt.Text, out SelTi.aTbl_RepeatCnt[1]);

                // 2020.11.30 lhs 최소값 확인
                if (SelTi.aTbl_MeasCnt[0] <= 0) { CMsg.Show(eMsg.Warning, "Warning", "No point has been specified to measure the table."); return; } 
                if (SelTi.aTbl_MeasCnt[1] <= 0) { CMsg.Show(eMsg.Warning, "Warning", "No point has been specified to measure the table."); return; }
                if (SelTi.aTbl_RepeatCnt[0] <= 0) { CMsg.Show(eMsg.Warning, "Warning", "The number of repetitions is not specified."); return; }
                if (SelTi.aTbl_RepeatCnt[1] <= 0) { CMsg.Show(eMsg.Warning, "Warning", "The number of repetitions is not specified."); return; }

                // 2020.11.27 lhs 최대값 제한
                if (SelTi.aTbl_MeasCnt[0]   > CTm8.m_nMaxPt) SelTi.aTbl_MeasCnt[0]    = CTm8.m_nMaxPt;
                if (SelTi.aTbl_MeasCnt[1]   > CTm8.m_nMaxPt) SelTi.aTbl_MeasCnt[1]    = CTm8.m_nMaxPt;
                if (SelTi.aTbl_RepeatCnt[0] > CTm8.m_nMaxRept) SelTi.aTbl_RepeatCnt[0]  = CTm8.m_nMaxRept;
                if (SelTi.aTbl_RepeatCnt[1] > CTm8.m_nMaxRept) SelTi.aTbl_RepeatCnt[1]  = CTm8.m_nMaxRept;

                if (SelTi.nDivCol % 2 == 0) SelTi.nDivCol++;  // 홀수만
                if (SelTi.nDivRow % 2 == 0) SelTi.nDivRow++;
                if (SelTi.nDivCol < 7) SelTi.nDivCol = 7;     // 최소 7개 이상으로
                if (SelTi.nDivRow < 7) SelTi.nDivRow = 7;

                // GridView 부분
                // 최대를 넘지 않게
                for (int i = 0; i < SelTi.aTbl_MeasCnt[0]; i++)
                {
                    if ((int)SelTi.alLTbl_Colidx[i] >= SelTi.nDivCol) SelTi.alLTbl_Colidx[i] = SelTi.nDivCol - 1;
                    if ((int)SelTi.alLTbl_Rowidx[i] >= SelTi.nDivRow) SelTi.alLTbl_Rowidx[i] = SelTi.nDivRow - 1;
                }
                for (int i = 0; i < SelTi.aTbl_MeasCnt[1]; i++)
                {
                    if ((int)SelTi.alRTbl_Colidx[i] >= SelTi.nDivCol) SelTi.alRTbl_Colidx[i] = SelTi.nDivCol - 1;
                    if ((int)SelTi.alRTbl_Rowidx[i] >= SelTi.nDivRow) SelTi.alRTbl_Rowidx[i] = SelTi.nDivRow - 1;
                }

                CTm8.It.CalcMeasPoints(ref SelTi);

                // 현재의 지우고 insert
                int nIdx = comboTable.SelectedIndex;
                CTm8.It.m_alTableInfo.RemoveAt(nIdx);
                CTm8.It.m_alTableInfo.Insert(nIdx, SelTi);
            }

            CTm8.It.mTable = SelTi;

        }


        /// <summary>
        /// Select Table combobox에 리스트 추가
        /// </summary>
        private void comboTableItemAdd()
        {
            string sNum = "";
            string sSec = "";
            comboTable.Items.Clear();

            for (int i = 0; i < CTm8.It.m_alTableInfo.Count; i++)
            {
                // 콤보박스에 Table 리스트 추가
                sNum = string.Format("{0:D2}", i + 1);
                sSec = "Table_" + sNum;
                comboTable.Items.Add(sSec);
            }
            comboTable.Enabled = true;
        }

        /// <summary>
        /// 선택된 Table 정보 표시
        /// </summary>
        private void DisplayTableInfo()
        {
            _DispTextbox();
            _DrawMeasCell();
            _DispMeasPoint();
        }

        /// <summary>
        /// 선택된 Table 정보 표시 : textbox
        /// </summary>
        private void _DispTextbox()
        {
            if (CTm8.It.m_nSelectedTable < 0)
                return;

            txtName.Text = SelTi.sName;
            txtComment.Text = SelTi.sComment;
            txtTableShort.Text = SelTi.dShortLength.ToString();
            txtTableLong.Text = SelTi.dLongLength.ToString();
            txtCol.Text = SelTi.nDivCol.ToString();
            txtRow.Text = SelTi.nDivRow.ToString();

            txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[0].ToString();
            txtLTRepeatCnt.Text = SelTi.aTbl_RepeatCnt[0].ToString();
            txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[1].ToString();
            txtRTRepeatCnt.Text = SelTi.aTbl_RepeatCnt[1].ToString();
        }

        /// <summary>
        /// 선택된 Table 정보 표시 : DataGridView
        /// </summary>
        private void _DrawMeasCell()
        {
            if (CTm8.It.m_nSelectedTable < 0)
                return;

            int iCol = SelTi.nDivCol;
            int iRow = SelTi.nDivRow;

            int iHei = 0;
            if (iRow != 0)
            {
                iHei = (dgvLeftTable.Height - 5) / iRow;
            }

            dgvLeftTable.Visible = false;
            dgvRightTable.Visible = false;

            dgvLeftTable.Rows.Clear();
            dgvRightTable.Rows.Clear();
            dgvLeftTable.Columns.Clear();
            dgvRightTable.Columns.Clear();

            for (int i = 0; i < iCol; i++)
            {
                dgvLeftTable.Columns.Add(i.ToString(), i.ToString());
                dgvRightTable.Columns.Add(i.ToString(), i.ToString());
                dgvLeftTable.Columns[i].Resizable = DataGridViewTriState.False;
                dgvRightTable.Columns[i].Resizable = DataGridViewTriState.False;
            }

            dgvLeftTable.RowTemplate.Height = iHei;
            dgvRightTable.RowTemplate.Height = iHei;
            dgvLeftTable.RowTemplate.Resizable = DataGridViewTriState.False;
            dgvRightTable.RowTemplate.Resizable = DataGridViewTriState.False;

            int iColor = 255;
            Color tColor = Color.FromArgb(iColor, iColor, iColor);

            for (int j = 0; j < iRow; j++)
            {
                dgvLeftTable.Rows.Add();
                dgvRightTable.Rows.Add();
                dgvLeftTable.Rows[j].DefaultCellStyle.BackColor = tColor;
                dgvRightTable.Rows[j].DefaultCellStyle.BackColor = tColor;
            }

            for (int i = 0; i < SelTi.aTbl_MeasCnt[0]; i++)
            {
                int col = (int)SelTi.alLTbl_Colidx[i];
                int row = (int)SelTi.alLTbl_Rowidx[i];
                dgvLeftTable.Rows[row].Cells[col].Style.ApplyStyle(m_mStUse);
            }
            for (int i = 0; i < SelTi.aTbl_MeasCnt[1]; i++)
            {
                int col = (int)SelTi.alRTbl_Colidx[i];
                int row = (int)SelTi.alRTbl_Rowidx[i];
                dgvRightTable.Rows[row].Cells[col].Style.ApplyStyle(m_mStUse);
            }

            dgvLeftTable.Rows[iRow / 2].Cells[iCol / 2].Value = "C";
            dgvRightTable.Rows[iRow / 2].Cells[iCol / 2].Value = "C";
            dgvLeftTable.ClearSelection();
            dgvRightTable.ClearSelection();
            dgvLeftTable.Visible = true;
            dgvRightTable.Visible = true;
        }


        /// <summary>
        /// 선택된 Table 정보 표시 : 측정포인트 Textbox에 좌표 표시 ([Col, Row], X=x.xxx, Y=x.xxx)
        /// </summary>
        private void _DispMeasPoint()
        {
            if (CTm8.It.m_nSelectedTable < 0)
                return;

            string strPoint = "";
            txtLTMeasPoint.Text = "";
            txtRTMeasPoint.Text = "";
            for (int i = 0; i < SelTi.aTbl_MeasCnt[0]; i++)
            {
                int col = (int)SelTi.alLTbl_Colidx[i];
                int row = (int)SelTi.alLTbl_Rowidx[i];
                double dX = (double)SelTi.alLTbl_X[i];
                double dY = (double)SelTi.alLTbl_Y[i];
                strPoint = string.Format("#{0} [{1,2}, {2,2}]  X={3:0.0}, Y={4:0.0}", i + 1, col, row, dX, dY);
                txtLTMeasPoint.Text += strPoint + Environment.NewLine;
            }
            for (int i = 0; i < SelTi.aTbl_MeasCnt[1]; i++)
            {
                int col = (int)SelTi.alRTbl_Colidx[i];
                int row = (int)SelTi.alRTbl_Rowidx[i];
                double dX = (double)SelTi.alRTbl_X[i];
                double dY = (double)SelTi.alRTbl_Y[i];
                strPoint = string.Format("#{0} [{1,2}, {2,2}]  X={3:0.0}, Y={4:0.0}", i + 1, col, row, dX, dY);
                txtRTMeasPoint.Text += strPoint + Environment.NewLine;
            }
        }

        /// <summary>
        /// 데이터 취득시 측정 결과 표시 
        /// </summary>
        private void DisplayResult(int nRT)
        {
            int nAcqReptCnt = CTm8.It.m_nAcqReptCnt[nRT];   // 갯수 : 초기 0, 취득시 1~
            int nAcqMeasCnt = CTm8.It.m_nAcqPtCnt[nRT];   // 갯수 : 초기 0, 취득시 1~
            bool bAcqed = CTm8.It.m_bAcqed[nRT];            // 데이터 취득 여부

            if ((nAcqReptCnt <= 0) || (nAcqMeasCnt <= 0)) // 취득 전이면...
                return;

            if (!bAcqed)    return;
            CTm8.It.m_bAcqed[nRT] = false;

            string sMsg = "";

            if (nRT == 0)
            {
                sMsg = string.Format("Repeat #{0}, Point #{1} : Table H = {2:0.0000} [mm]\r\n", nAcqReptCnt, nAcqMeasCnt, CTm8.It.m_aLTbl_RltZ[nAcqReptCnt - 1, nAcqMeasCnt - 1]);
                richLeftResult.AppendText(sMsg);
                richLeftResult.ScrollToCaret();
                _SetLog(sMsg);
            }
            else if (nRT == 1)
            {
                sMsg = string.Format("Repeat #{0}, Point #{1} : Table H = {2:0.0000} [mm]\r\n", nAcqReptCnt, nAcqMeasCnt, CTm8.It.m_aRTbl_RltZ[nAcqReptCnt - 1, nAcqMeasCnt - 1]);
                richRightResult.AppendText(sMsg);
                richRightResult.ScrollToCaret();
                _SetLog(sMsg);
            }
            else { }

            //---------------
            // 모든 측정 완료시 저장
            if ( (nAcqReptCnt >= CTm8.It.mTable.aTbl_RepeatCnt[nRT]) &&
                (nAcqMeasCnt >= CTm8.It.mTable.aTbl_MeasCnt[nRT]) )
            {
                CalcTTV(nRT, nAcqReptCnt, nAcqMeasCnt);

                SaveResult(nRT);
            }
            //---------------

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


        /// <summary>
        /// Table 정보 Textbox : Enable/Disable
        /// </summary>
        private void EnableTextBox(bool bEnable)
        {
            txtName.Enabled = bEnable;
            txtComment.Enabled = bEnable;
            txtTableShort.Enabled = bEnable;
            txtTableLong.Enabled = bEnable;
            txtCol.Enabled = bEnable;
            txtRow.Enabled = bEnable;

            txtLTRepeatCnt.Enabled = bEnable;
            txtRTRepeatCnt.Enabled = bEnable;
        }

        /// <summary>
        /// Table 측정 Start 버튼 클릭
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {

            CData.bLastClick = true;
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);
            _SetLog(string.Format("N, {0} click", ESeq.ToString()));

            _Set(); // Edit Mode도 포함됨

            if (ESeq == ESeq.GRL_Table_Measure_8p)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want Left Table Measure Start?") == DialogResult.Cancel)
                    return;
                richLeftResult.Clear();  // richbox
            }
            if (ESeq == ESeq.GRR_Table_Measure_8p)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want Right Table Measure Start?") == DialogResult.Cancel)
                    return;
                richRightResult.Clear(); // richbox
            }

            CTm8.It.mTable = SelTi;

            CSQ_Man.It.Seq = ESeq;
            CSQ_Man.It.bStop = false;
            _EnableBtn(false);
            CSQ_Man.It.bBtnShow = false;

        }


        /// <summary>
        /// Table 측정 Stop 버튼 클릭
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true;

            /// Z축을 안전하게...
            DoSafeZAxis();

            CSQ_Man.It.bStop = true;
            _EnableBtn(true);
            CSQ_Man.It.bBtnShow = true;
        }

        private void DoSafeZAxis()
        {
            int iWy = 0;
            int iAZ = (int)EAx.LeftGrindZone_Z;
            eY eOt1 = eY.GRDL_ProbeDn;
            eY eOt2 = eY.GRDL_ProbeAir;

            if      (CSQ_Man.It.Seq == ESeq.GRL_Table_Measure_8p)
            {           
            }
            else if (CSQ_Man.It.Seq == ESeq.GRR_Table_Measure_8p)
            {
                iWy = 1;
                iAZ = (int)EAx.RightGrindZone_Z;
                eOt1 = eY.GRDR_ProbeDn;
                eOt2 = eY.GRDR_ProbeAir;
            }

            // Z축 ABLE 위치 이동
            CMot.It.Mv_N(iAZ, CData.SPos.dGRD_Z_Able[iWy]);
            // 프로브 Air OFF 
            CIO.It.Set_Y(eOt2, false);
            // 프로브 UP 
            CIO.It.Set_Y(eOt1, false);
        }

        /// <summary>
        /// Table 추가 버튼 클릭
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            _SetLog("Click");

            CTableInfo NewTi = CTm8.It.MakeDefaultTable();

            int nCurCnt = CTm8.It.m_alTableInfo.Count;
            NewTi.sName = "Table Name" + (nCurCnt + 1).ToString();
            NewTi.sComment = "Table Comment" + (nCurCnt + 1).ToString();

            CTm8.It.m_alTableInfo.Add(NewTi); //추가
            CTm8.It.m_nSelectedTable = CTm8.It.m_alTableInfo.Count - 1;

            CTm8.It.Save();

            _Set();
        }

        /// <summary>
        /// Table 삭제 버튼 클릭
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            _SetLog("Click");

            // 삭제 여부 확인
            if (CMsg.Show(eMsg.Query, "Delete Table", "Delete table information?") == DialogResult.OK)
            {
                int nSel = comboTable.SelectedIndex;

                CTm8.It.m_alTableInfo.RemoveAt(nSel);
                comboTable.SelectedIndex = CTm8.It.m_alTableInfo.Count - 1;
                CTm8.It.m_nSelectedTable = comboTable.SelectedIndex;
                CTm8.It.Save();

                _Set();
            }
            else
            {
                comboTable.Enabled = true;

                m_eEditMode = EEditMode.None;
                EnableComponent();
            }
        }

        /// <summary>
        /// Table 수정 버튼 클릭
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            _SetLog("Click");

            if (m_eEditMode == EEditMode.Edit)
            {
                comboTable.Enabled = true;
                m_eEditMode = EEditMode.None;
                EnableComponent();
            }
            else
            {
                comboTable.Enabled = false;
                m_eEditMode = EEditMode.Edit;
                EnableComponent();
            }
        }


        /// <summary>
        /// Edit Mode에 따른 Component Enable/Disable
        /// </summary>
        private void EnableComponent()
        {
            if (m_eEditMode == EEditMode.None)
            {
                EnableTextBox(false);     // TextBox 활성화
                btnSave.Enabled = false;
                labelLeftTable.Text = "Left Table Measure Point";
                labelRightTable.Text = "Right Table Measure Point";
                labelLeftTable.ForeColor = Color.White;
                labelRightTable.ForeColor = Color.White;
            }
            else if (m_eEditMode == EEditMode.Edit)
            {
                EnableTextBox(true);     // TextBox 활성화
                btnSave.Enabled = true;
                labelLeftTable.Text = "Left Table Measure Point - EditMode";
                labelRightTable.Text = "Right Table Measure Point - EditMode";
                labelLeftTable.ForeColor = Color.Black;
                labelRightTable.ForeColor = Color.Black;
            }
            else { }

        }

        /// <summary>
        /// comboTable 변경시
        /// </summary>
        private void comboTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboTable.SelectedIndex < 0)
                return;

            CTm8.It.m_nSelectedTable = comboTable.SelectedIndex;
            SelTi = CTm8.It.m_alTableInfo[CTm8.It.m_nSelectedTable] as CTableInfo;

            DisplayTableInfo();
        }


        /// <summary>
        /// 저장버튼 클릭
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;

            _Get();

            iRet = CTm8.It.Save();
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "Table Information file save fail");
            }
            else
            {
                CMsg.Show(eMsg.Notice, "Notice", "Table Information file save success");
            }

            _Set();
        }

        /// <summary>
        /// 컨텍스트 메뉴 변경시
        /// </summary>
        private void cmsTable_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (m_eEditMode != EEditMode.Edit)
                return;

            DataGridView mDgv = cmsTable.SourceControl as DataGridView;
            int nRT = int.Parse(mDgv.Tag.ToString());

            foreach (DataGridViewCell mCell in mDgv.SelectedCells)
            {
                int nColidx = mCell.ColumnIndex;
                int nRowidx = mCell.RowIndex;

                if (e.ClickedItem.Text == "Use")
                {
                    mCell.Style.ApplyStyle(m_mStUse);
                    AddMeasPoint(nRT, nColidx, nRowidx);
                }
                else
                {
                    mCell.Style.ApplyStyle(m_mStW1);
                    RemoveMeasPoints(nRT, nColidx, nRowidx);
                }
            }

            CTm8.It.CalcMeasPoints(ref SelTi);

        }//


        /// <summary>
        /// 측정포인트 추가 로직
        /// </summary>
        private void AddMeasPoint(int nRTable, int nColidx, int nRowidx)
        {
            if (nRTable == 0) // Left Table
            {
                // 측정셀이 전혀 없을 때  // 2020.11.10 lhs
                if (SelTi.aTbl_MeasCnt[nRTable] == 0)
                {
                    SelTi.alLTbl_Colidx.Add(nColidx);
                    SelTi.alLTbl_Rowidx.Add(nRowidx);
                    SelTi.alLTbl_X.Add(0.0);
                    SelTi.alLTbl_Y.Add(0.0);
                    SelTi.aTbl_MeasCnt[nRTable]++;
                    txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                    return;
                }

                for (int n = 0; n < SelTi.aTbl_MeasCnt[nRTable]; n++)
                {
                    // col 이 작으면 바로 insert
                    if (nColidx < (int)SelTi.alLTbl_Colidx[n])
                    {
                        SelTi.alLTbl_Colidx.Insert(n, nColidx);
                        SelTi.alLTbl_Rowidx.Insert(n, nRowidx);
                        SelTi.alLTbl_X.Insert(n, 0.0);
                        SelTi.alLTbl_Y.Insert(n, 0.0);
                        SelTi.aTbl_MeasCnt[nRTable]++;
                        txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                        return;
                    }
                    // col 이 같으면 Row 체크
                    else if (nColidx == (int)SelTi.alLTbl_Colidx[n])
                    {
                        // 같은 Colidx의 갯수를 모두 찾는다
                        // 갯수만큼 for문을 돌려
                        // 기존거보다 Rowidx가 작으면 바로 insert
                        // 기존거와 Rowidx 같으면 무시
                        // 기존거보다 Rowidx 크면 continue하고 마지막이면 뒤에 insert
                        int nSameColCnt = 0;
                        for (int cc = n; cc < (int)SelTi.aTbl_MeasCnt[nRTable]; cc++)
                        {
                            if (nColidx == (int)SelTi.alLTbl_Colidx[cc])
                                nSameColCnt++;
                        }

                        for (int cc = n; cc < (n + nSameColCnt); cc++)
                        {
                            //  row가 작으면 바로 insert
                            if (nRowidx < (int)SelTi.alLTbl_Rowidx[cc])
                            {
                                SelTi.alLTbl_Colidx.Insert(cc, nColidx);
                                SelTi.alLTbl_Rowidx.Insert(cc, nRowidx);
                                SelTi.alLTbl_X.Insert(cc, 0.0);
                                SelTi.alLTbl_Y.Insert(cc, 0.0);
                                SelTi.aTbl_MeasCnt[nRTable]++;
                                txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                                return;
                            }
                            //  row가 같으면 무시
                            else if (nRowidx == (int)SelTi.alLTbl_Rowidx[cc])
                            {
                                return;
                            }
                            // 기존거보다 Rowidx 크면 continue하고 마지막이면 뒤에 insert
                            else if (nRowidx > (int)SelTi.alLTbl_Rowidx[cc])
                            {
                                if (cc == n + nSameColCnt - 1) // 마지막
                                {
                                    SelTi.alLTbl_Colidx.Insert(cc + 1, nColidx);
                                    SelTi.alLTbl_Rowidx.Insert(cc + 1, nRowidx);
                                    SelTi.alLTbl_X.Insert(cc + 1, 0.0);
                                    SelTi.alLTbl_Y.Insert(cc + 1, 0.0);
                                    SelTi.aTbl_MeasCnt[nRTable]++;
                                    txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                                    return;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                    // col이 크면 바로 Add
                    else if (nColidx > (int)SelTi.alLTbl_Colidx[n])
                    {
                        if (n == SelTi.aTbl_MeasCnt[nRTable] - 1)
                        {
                            SelTi.alLTbl_Colidx.Add(nColidx);
                            SelTi.alLTbl_Rowidx.Add(nRowidx);
                            SelTi.alLTbl_X.Add(0.0);
                            SelTi.alLTbl_Y.Add(0.0);
                            SelTi.aTbl_MeasCnt[nRTable]++;
                            txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                            return;
                        }
                        else
                            continue;
                    }
                } // for
            } // if(nRTable == 0)
            else
            {
                // 측정셀이 전혀 없을 때  // 2020.11.10 lhs
                if (SelTi.aTbl_MeasCnt[nRTable] == 0)
                {
                    SelTi.alRTbl_Colidx.Add(nColidx);
                    SelTi.alRTbl_Rowidx.Add(nRowidx);
                    SelTi.alRTbl_X.Add(0.0);
                    SelTi.alRTbl_Y.Add(0.0);
                    SelTi.aTbl_MeasCnt[nRTable]++;
                    txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                    return;
                }

                for (int n = 0; n < SelTi.aTbl_MeasCnt[nRTable]; n++)
                {
                    // col 이 작으면 바로 insert
                    if (nColidx < (int)SelTi.alRTbl_Colidx[n])
                    {
                        SelTi.alRTbl_Colidx.Insert(n, nColidx);
                        SelTi.alRTbl_Rowidx.Insert(n, nRowidx);
                        SelTi.alRTbl_X.Insert(n, 0.0);
                        SelTi.alRTbl_Y.Insert(n, 0.0);
                        SelTi.aTbl_MeasCnt[nRTable]++;
                        txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                        return;
                    }
                    // col 이 같으면 Row 체크
                    else if (nColidx == (int)SelTi.alRTbl_Colidx[n])
                    {
                        // 같은 Colidx의 갯수를 모두 찾는다
                        // 갯수만큼 for문을 돌려
                        // 기존거보다 Rowidx가 작으면 바로 insert
                        // 기존거와 Rowidx 같으면 무시
                        // 기존거보다 Rowidx 크면 continue하고 마지막이면 뒤에 insert
                        int nSameColCnt = 0;
                        for (int cc = n; cc < (int)SelTi.aTbl_MeasCnt[nRTable]; cc++)
                        {
                            if (nColidx == (int)SelTi.alRTbl_Colidx[cc])
                                nSameColCnt++;
                        }

                        for (int cc = n; cc < (n + nSameColCnt); cc++)
                        {
                            //  row가 작으면 바로 insert
                            if (nRowidx < (int)SelTi.alRTbl_Rowidx[cc])
                            {
                                SelTi.alRTbl_Colidx.Insert(cc, nColidx);
                                SelTi.alRTbl_Rowidx.Insert(cc, nRowidx);
                                SelTi.alRTbl_X.Insert(cc, 0.0);
                                SelTi.alRTbl_Y.Insert(cc, 0.0);
                                SelTi.aTbl_MeasCnt[nRTable]++;
                                txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                                return;
                            }
                            //  row가 같으면 무시
                            else if (nRowidx == (int)SelTi.alRTbl_Rowidx[cc])
                            {
                                return;
                            }
                            // 기존거보다 Rowidx 크면 continue하고 마지막이면 뒤에 insert
                            else if (nRowidx > (int)SelTi.alRTbl_Rowidx[cc])
                            {
                                if (cc == (n + nSameColCnt - 1)) // 마지막
                                {
                                    SelTi.alRTbl_Colidx.Insert(cc + 1, nColidx);
                                    SelTi.alRTbl_Rowidx.Insert(cc + 1, nRowidx);
                                    SelTi.alRTbl_X.Insert(cc + 1, 0.0);
                                    SelTi.alRTbl_Y.Insert(cc + 1, 0.0);
                                    SelTi.aTbl_MeasCnt[nRTable]++;
                                    txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                                    return;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                    // col이 크면 바로 Add
                    else if (nColidx > (int)SelTi.alRTbl_Colidx[n])
                    {
                        if (n == SelTi.aTbl_MeasCnt[nRTable] - 1)
                        {
                            SelTi.alRTbl_Colidx.Add(nColidx);
                            SelTi.alRTbl_Rowidx.Add(nRowidx);
                            SelTi.alRTbl_X.Add(0.0);
                            SelTi.alRTbl_Y.Add(0.0);
                            SelTi.aTbl_MeasCnt[nRTable]++;
                            txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                            return;
                        }
                        else
                            continue;
                    }
                } // for
            } // else

        }// AddMeasPoints

        /// <summary>
        /// 측정포인트 삭제 로직
        /// </summary>
        private void RemoveMeasPoints(int nRTable, int nColidx, int nRowidx)
        {
            if (nRTable == 0) // Left Table
            {
                for (int n = 0; n < SelTi.aTbl_MeasCnt[nRTable]; n++)
                {
                    // 측정위치 제거시는 col row 일치시 제거
                    if ((nColidx == (int)SelTi.alLTbl_Colidx[n]) && (nRowidx == (int)SelTi.alLTbl_Rowidx[n]))
                    {
                        SelTi.alLTbl_Colidx.RemoveAt(n);
                        SelTi.alLTbl_Rowidx.RemoveAt(n);
                        SelTi.alLTbl_X.RemoveAt(n);
                        SelTi.alLTbl_Y.RemoveAt(n);
                        SelTi.aTbl_MeasCnt[nRTable]--;
                        txtLTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                        return;
                    }

                } // for
            } // if(nRTable == 0)
            else
            {
                for (int n = 0; n < SelTi.aTbl_MeasCnt[nRTable]; n++)
                {
                    // 측정위치 제거시는 col row 일치시 제거
                    if ((nColidx == (int)SelTi.alRTbl_Colidx[n]) && (nRowidx == (int)SelTi.alRTbl_Rowidx[n]))
                    {
                        SelTi.alRTbl_Colidx.RemoveAt(n);
                        SelTi.alRTbl_Rowidx.RemoveAt(n);
                        SelTi.alRTbl_X.RemoveAt(n);
                        SelTi.alRTbl_Y.RemoveAt(n);
                        SelTi.aTbl_MeasCnt[nRTable]--;
                        txtRTMeasCnt.Text = SelTi.aTbl_MeasCnt[nRTable].ToString();
                        return;
                    }
                } // for
            } // else
        }// RemoveMeasPoints

        /// <summary>
        /// Table 측정 결과값에서 Max, Min, TTV 계산
        /// </summary>
        private void CalcTTV(int nRT, int nReptCnt, int nMeasCnt)
        {
            List<List<double>> listRepeat = new List<List<double>>();
            List<List<double>> listPoint = new List<List<double>>();

            // 반복 횟수별 Point 데이터 
            for (int nRe = 0; nRe < nReptCnt; nRe++)
            {
                listRepeat.Add(new List<double>());
                for (int nM = 0; nM < nMeasCnt; nM++)
                {
                    if (nRT == 0) listRepeat[nRe].Add(CTm8.It.m_aLTbl_RltZ[nRe, nM]);
                    if (nRT == 1) listRepeat[nRe].Add(CTm8.It.m_aRTbl_RltZ[nRe, nM]);
                }
            }
            // Point별 반복 데이터 
            for (int nM = 0; nM < nMeasCnt; nM++)
            {
                listPoint.Add(new List<double>());
                for (int nRe = 0; nRe < nReptCnt; nRe++)
                {
                    if (nRT == 0) listPoint[nM].Add(CTm8.It.m_aLTbl_RltZ[nRe, nM]);
                    if (nRT == 1) listPoint[nM].Add(CTm8.It.m_aRTbl_RltZ[nRe, nM]);
                }
            }

            // 표시
            string sMsg = "";
            for (int nRe = 0; nRe < nReptCnt; nRe++)
            {
                double dMax = listRepeat[nRe].Max();
                double dMin = listRepeat[nRe].Min();
                double dTTV = dMax - dMin;
                sMsg = string.Format("Repeat #{0} : Max = {1:0.0000}, Min = {2:0.0000}, TTV = {3:0.0000}\r\n", nRe + 1, dMax, dMin, dTTV);
                if (nRT == 0) richLeftResult.AppendText(sMsg);
                if (nRT == 1) richRightResult.AppendText(sMsg);
                _SetLog(sMsg);
            }

            // 표시
            sMsg = "";
            for (int nM = 0; nM < nMeasCnt; nM++)
            {
                double dMax = listPoint[nM].Max();
                double dMin = listPoint[nM].Min();
                double dTTV = dMax - dMin;
                sMsg = string.Format("Point #{0} : Max = {1:0.0000}, Min = {2:0.0000}, TTV = {3:0.0000}\r\n", nM + 1, dMax, dMin, dTTV);
                if (nRT == 0) richLeftResult.AppendText(sMsg);
                if (nRT == 1) richRightResult.AppendText(sMsg);
                _SetLog(sMsg);
            }
        }

        /// <summary>
        /// Left Table 측정결과값 저장 버튼
        /// </summary>
        private void btnSaveLTRlt_Click(object sender, EventArgs e)
        {
            int nRT = 0;
            SaveResult(nRT); //left
        }

        /// <summary>
        /// Right Table 측정결과값 저장 버튼
        /// </summary>
        private void btnSaveRTRlt_Click(object sender, EventArgs e)
        {
            int nRT = 1;
            SaveResult(nRT); //right
        }

        /// <summary>
        /// Table 측정 결과값 저장 함수
        /// </summary>
        private void SaveResult(int nRT)
        {
            _SetLog("Click");

            int iRet = 0;
            string sName = "";

            if (nRT == 0)
            {
                sName = SelTi.sName + "_LT";
                iRet = CTm8.It.SaveResult(sName, richLeftResult.Text);

                if (iRet != 0)  {   CMsg.Show(eMsg.Error, "Error", "Left Table Measure Result file save fail");        }
                else            {   CMsg.Show(eMsg.Notice, "Notice", "Left Table Measure Result file save success");   }
            }
            if (nRT == 1)
            {
                sName = SelTi.sName + "_RT";
                iRet = CTm8.It.SaveResult(sName, richRightResult.Text);

                if (iRet != 0)  { CMsg.Show(eMsg.Error, "Error", "Right Table Measure Result file save fail"); }
                else            { CMsg.Show(eMsg.Notice, "Notice", "Right Table Measure Result file save success"); }
            }

        }
    }


}
