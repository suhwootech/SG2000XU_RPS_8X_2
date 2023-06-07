using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm_4Con : UserControl
    {
        private EWay m_eWy;
        private int m_iWy;
        private DataGridViewCellStyle m_mStUse = new DataGridViewCellStyle() { BackColor = Color.Lime };
        private DataGridViewCellStyle m_mStW1 = new DataGridViewCellStyle() { BackColor = Color.White };
        private DataGridViewCellStyle m_mStW2 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
        private DataGridViewCellStyle m_mStW3 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(235, 235, 235) };
        private DataGridViewCellStyle m_mStW4 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(225, 225, 225) };
        private DataGridViewCellStyle m_mStW5 = new DataGridViewCellStyle() { BackColor = Color.FromArgb(215, 215, 215) };

        public vwDev_02Prm_4Con(EWay eWy)
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
                txtP_CtLChipGapW.Enabled = false;
                txtP_CtLChipGapH.Enabled = false;
                txtP_CtLWinCnt  .Enabled = false;
                txtP_CtLWin1X   .Enabled = false;
                txtP_CtLWin1Y   .Enabled = false;
                txtP_CtLWin2X   .Enabled = false;
                txtP_CtLWin2Y   .Enabled = false;
                txtP_CtLWin3X   .Enabled = false;
                txtP_CtLWin3Y   .Enabled = false;
                txtP_CtLWin4X   .Enabled = false;
                txtP_CtLWin4Y   .Enabled = false;
                txtP_CtLWin5X   .Enabled = false;
                txtP_CtLWin5Y   .Enabled = false;
            }
            else
            {
                txtP_CtLChipC   .Enabled = true;
                txtP_CtLChipR   .Enabled = true;
                txtP_CtLChipW   .Enabled = true;
                txtP_CtLChipH   .Enabled = true;
                txtP_CtLChipGapW.Enabled = true;
                txtP_CtLChipGapH.Enabled = true;
                txtP_CtLWinCnt  .Enabled = true;
                txtP_CtLWin1X   .Enabled = true;
                txtP_CtLWin1Y   .Enabled = true;
                txtP_CtLWin2X   .Enabled = true;
                txtP_CtLWin2Y   .Enabled = true;
                txtP_CtLWin3X   .Enabled = true;
                txtP_CtLWin3Y   .Enabled = true;
                txtP_CtLWin4X   .Enabled = true;
                txtP_CtLWin4Y   .Enabled = true;
                txtP_CtLWin5X   .Enabled = true;
                txtP_CtLWin5Y   .Enabled = true;

                txtP_CtLBfLit   .Enabled = true;
                txtP_CtLBfLitLower.Enabled = true;
                txtP_CtLAfLit   .Enabled = true;
                txtP_CtLone     .Enabled = true;
            }

            _init18PStripUIProperty(); //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: Company Option으로 기능 적용 결정되므로 Form 생성 시 처리

            // 2022.07.25 SungTae Start : [수정] (ASE-KR 개조건)
            // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
            // 2021-03-10, jhLee : Skyworks VOC, Before measure 값의 상/하한값을 별도로 운영
            //if (CData.CurCompany == ECompany.SkyWorks)
            if (CData.CurCompany == ECompany.SkyWorks || CData.CurCompany == ECompany.ASE_KR)
            {
                //// 입력에 필요한 컨트롤들의 속성을 지정한다.
                //lblBeforeUpper.Text = "Before Upper Limit";
                //lblBeforeLower.Text = "Before Lower Limit";

                if (CData.CurCompany == ECompany.SkyWorks)
                {
                    lblBeforeUpper.Text = "Before Upper Limit";
                    lblBeforeLower.Text = "Before Lower Limit";
                }
                else if (CData.CurCompany == ECompany.ASE_KR)
                {
                    lblBeforeUpper.Text = "Before Limit";
                    lblBeforeLower.Text = "Final Target Thickness";
                }

                lblBeforeLower.Visible      = true;
                txtP_CtLBfLitLower.Visible  = true;
                lblBeforeUnit2.Visible      = true;
            }
            // 2022.07.25 SungTae End

            // 2022.08.08 SungTae Start : [추가] (ASE-KR 개조건) 
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                txtP_CtLBfLitLower.ForeColor = Color.Red;    
            }
            // 2022.08.08 SungTae End

            // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
            if (CData.CurCompany == ECompany.ASE_KR)    { grb_MeasureOffset.Visible = true; }
            else                                        { grb_MeasureOffset.Visible = false; }
            // 2021.08.02 SungTae End
        }

        #region Private method
        /// <summary>
        /// Contact 좌표 및 측정 여부 값 획득
        /// </summary>
        private void _GetCont()
        {
            Color bkColor; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;
            int iCnt = CData.Dev.iWinCnt;
            double dGapX = CData.Dev.dChipWGap;
            double dGapY = CData.Dev.dChipHGap;

            if (m_eWy == EWay.L || (m_eWy == EWay.R && CData.Dev.bDual == eDual.Dual))
            {
                CData.Dev.aData[m_iWy].aPosBf = new tCont[iRow * iCnt, iCol];
                CData.Dev.aData[m_iWy].aPosAf = new tCont[iRow * iCnt, iCol];

                if (CDataOption.Use2004U)
                {
                    CData.Dev.aData[m_iWy].bDummy = new bool[iRow * iCnt, iCol];    // 2022.01.24 lhs : 2004U, Carrier내 Dummy 설정
                    if(m_eWy == EWay.L) // 한번만
                    {
                        CData.Dev.bCopyDummy = new bool[iRow * iCnt, iCol];         // 2022.03.24 lhs
					}
                }

                if (m_eWy == EWay.L && CData.Dev.bDual == eDual.Normal)
                {
                    CData.Dev.aData[1].aPosBf = new tCont[iRow * iCnt, iCol];
                    CData.Dev.aData[1].aPosAf = new tCont[iRow * iCnt, iCol];

                    if (CDataOption.Use2004U)
                    {
                        CData.Dev.aData[1].bDummy = new bool[iRow * iCnt, iCol];    // 2022.01.24 lhs : 2004U, Carrier내 Dummy 설정
                    }
                }

                for (int k = 0; k < iCnt; k++) // Window
                {
                     // 각 윈도우의 좌상점 실제 좌표
                    double dLx = CData.SPos.dGRD_X_Zero[0]       + CData.Dev.aWinSt[k].X;
                    double dLy = CData.MPos[0].dY_PRB_TBL_CENTER + CData.Dev.aWinSt[k].Y;
                    double dRx = CData.SPos.dGRD_X_Zero[1]       - CData.Dev.aWinSt[k].X;
                    double dRy = CData.MPos[1].dY_PRB_TBL_CENTER + CData.Dev.aWinSt[k].Y;
                    
                    for (int i = 0; i < iCol; i++) // Column
                    {
                        for (int j = 0; j < iRow; j++)  // Row
                        {
                            tCont tCon = new tCont();

                            // Before
                            //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                            bkColor = dgvP_CtLBf.Rows[j + (k * iRow)].Cells[i].Style.BackColor;

                            if      (bkColor == GV.CELL_BK_COLOR[1])    { tCon.bUse = true;   tCon.bUse18P = false; }
                            else if (bkColor == GV.CELL_BK_COLOR[2])    { tCon.bUse = false;  tCon.bUse18P = true;  }
                            else if (bkColor == GV.CELL_BK_COLOR[3])    { tCon.bUse = true;   tCon.bUse18P = true;  }
                            else                                        { tCon.bUse = false;  tCon.bUse18P = false; }
                            //

                            //200410 pjh : Measure Position
                            if(m_iWy == 0)
                            {
                                tCon.dX = Math.Round(dLx + (i * dGapX), 6);
                                tCon.dY = Math.Round(dLy + (j * dGapY), 6);
                            }
                            else
                            {
                                tCon.dX = Math.Round(dRx - (i * dGapX), 6);
                                tCon.dY = Math.Round(dRy + (j * dGapY), 6);
                            }
                            //
                            CData.Dev.aData[m_iWy].aPosBf[j + (k * iRow), i] = tCon;
                            // 2020.12.18 JSKim St
                            // 여기서 해야지 밑에서 tCon 값을 바꾸는데

                            // After
                            //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                            bkColor = dgvP_CtLAf.Rows[j + (k * iRow)].Cells[i].Style.BackColor;

                            if      (bkColor == GV.CELL_BK_COLOR[1])    { tCon.bUse = true;     tCon.bUse18P = false; }
                            else if (bkColor == GV.CELL_BK_COLOR[2])    { tCon.bUse = false;    tCon.bUse18P = true; }
                            else if (bkColor == GV.CELL_BK_COLOR[3])    { tCon.bUse = true;     tCon.bUse18P = true; }
                            else                                        { tCon.bUse = false;    tCon.bUse18P = false; }

                            CData.Dev.aData[m_iWy].aPosAf[j + (k * iRow), i] = tCon;

                            // Before
                            //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                            bkColor = dgvP_CtLBf.Rows[j + (k * iRow)].Cells[i].Style.BackColor;

                            if      (bkColor == GV.CELL_BK_COLOR[1])    { tCon.bUse = true;     tCon.bUse18P = false; }
                            else if (bkColor == GV.CELL_BK_COLOR[2])    { tCon.bUse = false;    tCon.bUse18P = true; }
                            else if (bkColor == GV.CELL_BK_COLOR[3])    { tCon.bUse = true;     tCon.bUse18P = true; }
                            else                                        { tCon.bUse = false;    tCon.bUse18P = false; }
 
                            if (m_eWy == EWay.L && CData.Dev.bDual == eDual.Normal)
                            {
                                // 200616-jym : 노말모드 일때 우측 에는 X축 포지션 반대
                                tCon.dX = Math.Round(dRx - (i * dGapX), 6);
                                tCon.dY = Math.Round(dRy + (j * dGapY), 6);
                                CData.Dev.aData[1].aPosBf[j + (k * iRow), i] = tCon; 
                            }

                            // 2020.12.25 JSKim St
                            //// After
                            ////200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                            //bkColor = dgvP_CtLAf.Rows[j + (k * iRow)].Cells[i].Style.BackColor;
                            //if (bkColor == GV.CELL_BK_COLOR[1])
                            //{ tCon.bUse = true;   tCon.bUse18P = false; }
                            //else if (bkColor == GV.CELL_BK_COLOR[2])
                            //{ tCon.bUse = false;  tCon.bUse18P = true; }
                            //else if (bkColor == GV.CELL_BK_COLOR[3])
                            //{ tCon.bUse = true;   tCon.bUse18P = true; }
                            //else
                            //{ tCon.bUse = false;  tCon.bUse18P = false; }
                            ////
                            // 2020.12.25 JSKim Ed

                            // 2020.12.18 JSKim St - 여기서 이러지 맙시다...
                            //CData.Dev.aData[m_iWy].aPosAf[j + (k * iRow), i] = tCon;
                            // 2020.12.18 JSKim Ed

                            // After
                            //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                            bkColor = dgvP_CtLAf.Rows[j + (k * iRow)].Cells[i].Style.BackColor;

                            if      (bkColor == GV.CELL_BK_COLOR[1])    { tCon.bUse = true;     tCon.bUse18P = false; }
                            else if (bkColor == GV.CELL_BK_COLOR[2])    { tCon.bUse = false;    tCon.bUse18P = true; }
                            else if (bkColor == GV.CELL_BK_COLOR[3])    { tCon.bUse = true;     tCon.bUse18P = true; }
                            else                                        { tCon.bUse = false;    tCon.bUse18P = false; }

                            if (m_eWy == EWay.L && CData.Dev.bDual == eDual.Normal)
                            {
                                // 200616-jym : 노말모드 일때 우측 에는 X축 포지션 반대
                                tCon.dX = Math.Round(dRx - (i * dGapX), 6);
                                tCon.dY = Math.Round(dRy + (j * dGapY), 6);

                                CData.Dev.aData[1].aPosAf[j + (k * iRow), i] = tCon; 
                            }
                        } // Row for
                    } // Column for
                } // Window for
            }
        }

        private void _DrawCont()
        {
            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            int iBkColor = 0; //Leader Strip Contact Point Use 설정 시 표시할 색상의 배열 내 인덱스 : GV.CELL_BK_COLOR 참조
                              // 0: 비측정 포인트(Window단위로 색상 변경됨)
                              // 1: Main Strip 전용 측정 포인트(Color Lime)
                              // 2: Leader Strip 전용 측정 포인트(Color Yellow)
                              // 3: Leader Strip, Main Strip 공용 측정 포인트(Color Pink)

            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;
            int iCnt = CData.Dev.iWinCnt;
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


            //int iRowMin = Math.Min(iRow * iCnt, CData.Dev.aData[0].aPosBf.GetLength(0));
            //int iColMin = Math.Min(iCol, CData.Dev.aData[0].aPosBf.GetLength(1));
            //기존 코드
            int iRowMin = Math.Min(iRow * iCnt, CData.Dev.aData[m_iWy].aPosBf.GetLength(0));
            int iColMin = Math.Min(iCol, CData.Dev.aData[m_iWy].aPosBf.GetLength(1));
            //수정된 코드
            //200409 : Chip Count Error
            for (int i = 0; i < iColMin; i++)
            {
                for (int j = 0; j < iRowMin; j++)
                {
                    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                    if ((CDataOption.Use18PointMeasure == true) /*&& (0 < CData.Dev.i18PStripCount)*/) //Leading Strip Count 0일 경우 이전 설정 포인트 지워진다고 issue 제기 시 (0 < CData.Dev.i18PStripCount) 옵션은 삭제하면 됨
                    {
                        iBkColor = 0; //초기화
                        if (CData.Dev.aData[m_iWy].aPosBf[j, i].bUse)    { iBkColor |= 0x01; }
                        if (CData.Dev.aData[m_iWy].aPosBf[j, i].bUse18P) { iBkColor |= 0x02; }
                        if (0 < iBkColor && iBkColor <= 3)
                        {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                            dgvP_CtLBf.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                        }
                        iBkColor = 0; //초기화
                        if (CData.Dev.aData[m_iWy].aPosAf[j, i].bUse)    { iBkColor |= 0x01; }
                        if (CData.Dev.aData[m_iWy].aPosAf[j, i].bUse18P) { iBkColor |= 0x02; }
                        if (0 < iBkColor && iBkColor <= 3)
                        {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                            dgvP_CtLAf.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                        }
                    }
                    else
                    {
                        if (CData.Dev.aData[m_iWy].aPosBf[j, i].bUse)
                        { dgvP_CtLBf.Rows[j].Cells[i].Style.ApplyStyle(m_mStUse); }
                        if (CData.Dev.aData[m_iWy].aPosAf[j, i].bUse)
                        { dgvP_CtLAf.Rows[j].Cells[i].Style.ApplyStyle(m_mStUse); }
                    }
                    //
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
            if (CDataOption.Is1Point)
            {
                if (CData.Lev == ELv.Master)
                {
                    groupBox3.Visible = true;
                    if (CData.Dev.bDual == eDual.Normal)
                    {
                        groupBox3.Text = "Select Axis Position (Left)";
                        groupBox4.Visible = true;
                    }
                    else
                    {
                        groupBox3.Text = "Select Axis Position";
                        groupBox4.Visible = false;
                    }
                }
                else
                {
                    groupBox3.Visible = false;
                    groupBox4.Visible = false;
                }
            }
            else
            {
                tabControl2.TabPages.Remove(tpP_CtLOnePos);
            }
            // 2020.08.31 JSKim Ed

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
            txtP_CtLChipGapW.Text = CData.Dev.dChipWGap.ToString();
            txtP_CtLChipGapH.Text = CData.Dev.dChipHGap.ToString();
            txtP_CtLWinCnt  .Text = CData.Dev.iWinCnt  .ToString();
            txtP_CtLWin1X   .Text = CData.Dev.aWinSt[0].X.ToString();
            txtP_CtLWin1Y   .Text = CData.Dev.aWinSt[0].Y.ToString();
            txtP_CtLWin2X   .Text = CData.Dev.aWinSt[1].X.ToString();
            txtP_CtLWin2Y   .Text = CData.Dev.aWinSt[1].Y.ToString();
            txtP_CtLWin3X   .Text = CData.Dev.aWinSt[2].X.ToString();
            txtP_CtLWin3Y   .Text = CData.Dev.aWinSt[2].Y.ToString();
            txtP_CtLWin4X   .Text = CData.Dev.aWinSt[3].X.ToString();
            txtP_CtLWin4Y   .Text = CData.Dev.aWinSt[3].Y.ToString();
            txtP_CtLWin5X   .Text = CData.Dev.aWinSt[4].X.ToString();
            txtP_CtLWin5Y   .Text = CData.Dev.aWinSt[4].Y.ToString();
            txtP_CtLBfLit.Text = CData.Dev.aData[m_iWy].dBfLimit.ToString();

            // 2022.07.25 SungTae Start : [수정] (ASE-KR 개조건)
            // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
            //txtP_CtLBfLitLower.Text = CData.Dev.aData[m_iWy].dBfLimitLower.ToString();   // 2021-03-08, jhLee, for Skyworks VOC
            if (CData.CurCompany == ECompany.ASE_KR)
                txtP_CtLBfLitLower.Text = CData.Dev.aData[m_iWy].dFinalTarget.ToString();    // 있는 UI 재활용
            else
                txtP_CtLBfLitLower.Text = CData.Dev.aData[m_iWy].dBfLimitLower.ToString();   // 2021-03-08, jhLee, for Skyworks VOC
            // 2022.07.25 SungTae End

            txtP_CtLAfLit.Text = CData.Dev.aData[m_iWy].dAfLimit.ToString();
            txtP_CtLone     .Text = CData.Dev.aData[m_iWy].dTTV.ToString();
            txtP_CtLOneLit  .Text = CData.Dev.aData[m_iWy].dOneLimit.ToString();//191018 ksg :
            txtP_CtLOneOver .Text = CData.Dev.aData[m_iWy].dOneOver.ToString();    // 2020.08.19 JSKim

            ckb_OnePointXPosFix.Checked = CData.Dev.aData[m_iWy].bOnePointXPosFix;
            txtP_CtLChipOneWin.Text = CData.Dev.aData[m_iWy].iOnePointWin.ToString();
            txtP_CtLChipOneCol.Text = CData.Dev.aData[m_iWy].iOnePointCol.ToString();
            txtP_CtLChipOneRow.Text = CData.Dev.aData[m_iWy].iOnePointRow.ToString();


            _set18PStripCount(); //200712 jhc : 18 포인트 측정 (ASE-KR VOC)

            // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                txtP_CtLMeasOffsetX.Text = CData.Dev.dMeasOffsetX.ToString();
                txtP_CtLMeasOffsetY.Text = CData.Dev.dMeasOffsetY.ToString();
            }
            else
            {
                txtP_CtLMeasOffsetX.Text = "0.0";
                txtP_CtLMeasOffsetY.Text = "0.0";
            }
            // 2021.08.02 SungTae End

            _DrawCont();
            _DrawOnePoint();    // 2020.08.31 JSKim
        }

        public void Get()
        {
            // 2020.08.31 JSKim St
            if (_CheckParaChange())
            {
                txtP_CtLChipOneWin.Text = "0";
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
                double.TryParse(txtP_CtLChipGapW.Text, out CData.Dev.dChipWGap);
                double.TryParse(txtP_CtLChipGapH.Text, out CData.Dev.dChipHGap);
                int   .TryParse(txtP_CtLWinCnt  .Text, out CData.Dev.iWinCnt  );
                CData.Dev.aWinSt[0].X = float.Parse(txtP_CtLWin1X.Text);
                CData.Dev.aWinSt[0].Y = float.Parse(txtP_CtLWin1Y.Text);
                CData.Dev.aWinSt[1].X = float.Parse(txtP_CtLWin2X.Text);
                CData.Dev.aWinSt[1].Y = float.Parse(txtP_CtLWin2Y.Text);
                CData.Dev.aWinSt[2].X = float.Parse(txtP_CtLWin3X.Text);
                CData.Dev.aWinSt[2].Y = float.Parse(txtP_CtLWin3Y.Text);
                CData.Dev.aWinSt[3].X = float.Parse(txtP_CtLWin4X.Text);
                CData.Dev.aWinSt[3].Y = float.Parse(txtP_CtLWin4Y.Text);
                CData.Dev.aWinSt[4].X = float.Parse(txtP_CtLWin5X.Text);
                CData.Dev.aWinSt[4].Y = float.Parse(txtP_CtLWin5Y.Text);

                // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
                double.TryParse(txtP_CtLMeasOffsetX.Text, out CData.Dev.dMeasOffsetX);
                double.TryParse(txtP_CtLMeasOffsetY.Text, out CData.Dev.dMeasOffsetY);
                // 2021.08.02 SungTae End

                _get18PStripCount(); //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            }

            double.TryParse(txtP_CtLBfLit.Text, out CData.Dev.aData[m_iWy].dBfLimit);

            // 2022.07.25 SungTae Start : [수정] (ASE-KR 개조건)
            // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
            //double.TryParse(txtP_CtLBfLitLower.Text, out CData.Dev.aData[m_iWy].dBfLimitLower);     // 2021-03-10, jhLee, for Skyworks VOC
            //CData.Dev.aData[m_iWy].dBfLimitLower = Math.Abs(CData.Dev.aData[m_iWy].dBfLimitLower);  // 음수가 나오지 않게 한다.
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                double.TryParse(txtP_CtLBfLitLower.Text, out CData.Dev.aData[m_iWy].dFinalTarget);
                CData.Dev.aData[m_iWy].dFinalTarget = Math.Abs(CData.Dev.aData[m_iWy].dFinalTarget);
            }
            else
            {
                double.TryParse(txtP_CtLBfLitLower.Text, out CData.Dev.aData[m_iWy].dBfLimitLower);     // 2021-03-10, jhLee, for Skyworks VOC
                CData.Dev.aData[m_iWy].dBfLimitLower = Math.Abs(CData.Dev.aData[m_iWy].dBfLimitLower);  // 음수가 나오지 않게 한다.
            }
            // 2022.07.25 SungTae End

            double.TryParse(txtP_CtLAfLit.Text, out CData.Dev.aData[m_iWy].dAfLimit);
            double.TryParse(txtP_CtLone.Text, out CData.Dev.aData[m_iWy].dTTV);
            double.TryParse(txtP_CtLOneLit.Text, out CData.Dev.aData[m_iWy].dOneLimit);
            double.TryParse(txtP_CtLOneOver.Text, out CData.Dev.aData[m_iWy].dOneOver); //  2020.08.19 JSKim
            // 2020.08.31 JSKim St
            CData.Dev.aData[m_iWy].bOnePointXPosFix = ckb_OnePointXPosFix.Checked;
            int.TryParse(txtP_CtLChipOneWin.Text, out CData.Dev.aData[m_iWy].iOnePointWin);
            int.TryParse(txtP_CtLChipOneCol.Text, out CData.Dev.aData[m_iWy].iOnePointCol);
            int.TryParse(txtP_CtLChipOneRow.Text, out CData.Dev.aData[m_iWy].iOnePointRow);
            // 2020.08.31 JSKim End

            //200424 jym : 노말 모드에서는 좌측 데이터 우측에 복사
            if (CData.Dev.bDual == eDual.Normal)
            {
                CData.Dev.aData[(int)EWay.R].dBfLimit = CData.Dev.aData[(int)EWay.L].dBfLimit;

                // 2022.07.25 SungTae Start : [수정] (ASE-KR 개조건)
                // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
                //CData.Dev.aData[(int)EWay.R].dBfLimitLower = CData.Dev.aData[(int)EWay.L].dBfLimitLower;  // 2021-03-08, jhLee, for Skyworks VOC
                if(CData.CurCompany == ECompany.ASE_KR)
                    CData.Dev.aData[(int)EWay.R].dFinalTarget   = CData.Dev.aData[(int)EWay.L].dFinalTarget;  // 있는 UI 재활용
                else
                    CData.Dev.aData[(int)EWay.R].dBfLimitLower  = CData.Dev.aData[(int)EWay.L].dBfLimitLower;  // 2021-03-08, jhLee, for Skyworks VOC
                // 2022.07.25 SungTae End

                CData.Dev.aData[(int)EWay.R].dAfLimit           = CData.Dev.aData[(int)EWay.L].dAfLimit;
                CData.Dev.aData[(int)EWay.R].dTTV               = CData.Dev.aData[(int)EWay.L].dTTV;
                CData.Dev.aData[(int)EWay.R].dOneLimit          = CData.Dev.aData[(int)EWay.L].dOneLimit;
                CData.Dev.aData[(int)EWay.R].dOneOver           = CData.Dev.aData[(int)EWay.L].dOneOver;  // 2020.08.19 JSKim
                CData.Dev.aData[(int)EWay.R].bOnePointXPosFix   = CData.Dev.aData[(int)EWay.L].bOnePointXPosFix;
                CData.Dev.aData[(int)EWay.R].iOnePointWin       = CData.Dev.aData[(int)EWay.L].iOnePointWin;
                CData.Dev.aData[(int)EWay.R].iOnePointCol       = CData.Dev.aData[(int)EWay.L].iOnePointCol;
                CData.Dev.aData[(int)EWay.R].iOnePointRow       = CData.Dev.aData[(int)EWay.L].iOnePointRow;
            }

            _DrawCont();
            _GetCont();         // 2021.09.07 lhs  버퍼 사이즈 문제로 에러가 발생하여 _DrawOnePoint()와 순서변경
            _DrawOnePoint();    // 2020.08.31 JSKim
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
            int iBkColor = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                              // 0: 비측정 포인트 / 1: Main Strip 전용 측정 포인트 / 2: 18 Point(Leading Strip) 전용 측정 포인트 / 3: Leading Strip, Main Strip 공용 측정 포인트

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
                    if      (mCell.RowIndex < CData.Dev.iRow)       { mCell.Style.ApplyStyle(m_mStW1); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 2)   { mCell.Style.ApplyStyle(m_mStW2); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 3)   { mCell.Style.ApplyStyle(m_mStW3); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 4)   { mCell.Style.ApplyStyle(m_mStW4); }
                    else if (mCell.RowIndex < CData.Dev.iRow * 5)   { mCell.Style.ApplyStyle(m_mStW5); }

                    bVal = false;
                }

                if (iBf == 0) // DataGridView Tag 0 : Before
                {
                    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                    if (rdb_Strip18P.Checked)
                    { CData.Dev.aData[m_iWy].aPosBf[mCell.RowIndex, mCell.ColumnIndex].bUse18P = bVal; } //Leader Strip Contact Point Use/NotUse 설정
                    else
                    { CData.Dev.aData[m_iWy].aPosBf[mCell.RowIndex, mCell.ColumnIndex].bUse = bVal; }    //Main Strip Contact Point Use/NotUse 설정
                }
                else // DataGridView Tag 1 : After
                {
                    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                    if (rdb_Strip18P.Checked)
                    { CData.Dev.aData[m_iWy].aPosAf[mCell.RowIndex, mCell.ColumnIndex].bUse18P = bVal; } //Leader Strip Contact Point Use/NotUse 설정
                    else
                    { CData.Dev.aData[m_iWy].aPosAf[mCell.RowIndex, mCell.ColumnIndex].bUse = bVal; }    //Main Strip Contact Point Use/NotUse 설정
                }

                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                if (CDataOption.Use18PointMeasure == true)
                {
                    iBkColor = 0; //초기화
                    
                    if (iBf == 0)
                    {
                        if (CData.Dev.aData[m_iWy].aPosBf[mCell.RowIndex, mCell.ColumnIndex].bUse)    { iBkColor |= 0x01; }
                        if (CData.Dev.aData[m_iWy].aPosBf[mCell.RowIndex, mCell.ColumnIndex].bUse18P) { iBkColor |= 0x02; }
                    }
                    else
                    {
                        if (CData.Dev.aData[m_iWy].aPosAf[mCell.RowIndex, mCell.ColumnIndex].bUse)    { iBkColor |= 0x01; }
                        if (CData.Dev.aData[m_iWy].aPosAf[mCell.RowIndex, mCell.ColumnIndex].bUse18P) { iBkColor |= 0x02; }
                    }

                    if (0 < iBkColor && iBkColor <= 3)
                    {//Lead Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                        mCell.Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                    }
                }
                //
            }
        }

        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)

        /// <summary>
        /// Company Option에 따라 Leader Strip Count 속성 표시/비표시 설정
        /// Leader Strip Count 콤보 박스에 아이템(Leader Strip 수량) 추가
        /// </summary>
        private void _init18PStripUIProperty()
        {
            if (CDataOption.Use18PointMeasure == true)
            {
                cmbO_18PStripCount.Items.Clear();
                for (int i=0; i <= GV.LEADER_STRIP_CNT_MAX; i++)
                {
                    cmbO_18PStripCount.Items.Add(i.ToString());
                }
                
                if (m_eWy == EWay.L)
                { cmbO_18PStripCount.Enabled = true; }
                else
                { cmbO_18PStripCount.Enabled = false; }

                pnl18P.Visible = true;
                /*
                lbl_18PStripCountTitle.Visible = true;
                cmbO_18PStripCount.Visible     = true;
                lbl_18PStripCountUnit.Visible  = true;

                grb_StripClass.Visible            = true;
                grb_ContactPointColor.Visible     = true;
                */
            }
            else
            {
                CData.Dev.i18PStripCount = 0;
                cmbO_18PStripCount.SelectedIndex = 0;
                rdb_StripMain.Checked = true;

                pnl18P.Visible = false;
                /*
                lbl_18PStripCountTitle.Visible = false;
                cmbO_18PStripCount.Visible     = false;
                lbl_18PStripCountUnit.Visible  = false;
                
                grb_StripClass.Visible            = false;
                grb_ContactPointColor.Visible     = false;
                */
            }
        }

        /// <summary>
        /// Leader Strip Count 값에 따른 Strip Class 그룹 박스 상태/값 설정
        /// </summary>
        private void _set18PStripUIProperty()
        {
            if (CDataOption.Use18PointMeasure == true)
            {
                if (cmbO_18PStripCount.SelectedIndex == 0)
                {
                    rdb_StripMain.Checked  = true;
                    
                    rdb_StripMain.ForeColor = Color.Red;
                    rdb_StripMain.Font = new Font(rdb_StripMain.Font, FontStyle.Bold);
                    rdb_Strip18P.ForeColor = Color.Black;
                    rdb_Strip18P.Font = new Font(rdb_Strip18P.Font, FontStyle.Regular);
                    //200713 jhc : grb_StripClass.Enabled = false;
                }
                else
                {
                    if (rdb_StripMain.Checked == true)
                    {
                        rdb_StripMain.ForeColor = Color.Red;
                        rdb_StripMain.Font = new Font(rdb_StripMain.Font, FontStyle.Bold);
                        rdb_Strip18P.ForeColor = Color.Black;
                        rdb_Strip18P.Font = new Font(rdb_Strip18P.Font, FontStyle.Regular);
                    }
                    else
                    {
                        rdb_StripMain.ForeColor = Color.Black;
                        rdb_StripMain.Font = new Font(rdb_StripMain.Font, FontStyle.Regular);
                        rdb_Strip18P.ForeColor = Color.Red;
                        rdb_Strip18P.Font = new Font(rdb_Strip18P.Font, FontStyle.Bold);
                    }
                    //200713 jhc : grb_StripClass.Enabled = true;
                }
                //pnl18P.Visible = true;
            }
            else
            {
                //rdb_StripMain.Checked  = true;
                //pnl18P.Visible = false;
            }          
        }

        private void _set18PStripCount()
        {
            cmbO_18PStripCount.SelectedIndex = CData.Dev.i18PStripCount;
        }
        private void _get18PStripCount()
        {
            CData.Dev.i18PStripCount = cmbO_18PStripCount.SelectedIndex;
        }

        private void cmbO_18PStripCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            _set18PStripUIProperty();
        }

        private void rdb_StripClass_CheckedChanged(object sender, EventArgs e)
        {
            _set18PStripUIProperty();
        }

        /// <summary>
        /// 설정한 측정 포인트 수가 제한 범위 수량 한도 내에 있는지 여부 체크하기 위해 설정된 측정 포인트 수량 확인
        /// </summary>
        /// <returns>-1=Error, 0=성공</returns>
        public int CheckMeasurePointCount(ref int [] iUseCnt, ref int [] iUse18PCnt)
        {
            int result = 0;

            //CData.Dev.aData[m_iWy].aPosBf
            //CData.Dev.aData[m_iWy].aPosAf

            Color bkColor;
            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow * CData.Dev.iWinCnt;

            if ((iUseCnt.Length < 2) || (iUse18PCnt.Length < 2))
            { return (-1); }

            iUseCnt[0] = iUseCnt[1] = 0;        //index 0=before, 1=after
            iUse18PCnt[0] = iUse18PCnt[1] = 0;  //index 0=before, 1=after

            for (int i = 0; i < iCol; i++) // Column
            {
                for (int j = 0; j < iRow; j++)  // Row
                {
                    // Before
                    //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                    bkColor = dgvP_CtLBf.Rows[j].Cells[i].Style.BackColor;
                    if (bkColor == GV.CELL_BK_COLOR[1])
                    { iUseCnt[0]++; }
                    else if (bkColor == GV.CELL_BK_COLOR[2])
                    { iUse18PCnt[0]++; }
                    else if (bkColor == GV.CELL_BK_COLOR[3])
                    { iUseCnt[0]++;   iUse18PCnt[0]++; }
                    // After
                    //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: 셀 색상으로 Contact Point 설정 상태 파악
                    bkColor = dgvP_CtLAf.Rows[j].Cells[i].Style.BackColor;
                    if (bkColor == GV.CELL_BK_COLOR[1])
                    { iUseCnt[1]++; }
                    else if (bkColor == GV.CELL_BK_COLOR[2])
                    { iUse18PCnt[1]++; }
                    else if (bkColor == GV.CELL_BK_COLOR[3])
                    { iUseCnt[1]++;   iUse18PCnt[1]++; }
                } // Row for
            } // Column for

            //200713 jhc : 측정포인트 없는 경우 체크
            if (iUseCnt[0] <= 0)
            { return (10); } //Main Strip Before Measure Point Count = 0
            else if (iUseCnt[1] <= 0)
            { return (11); } //Main Strip After Measure Point Count = 0
            else if ((CDataOption.Use18PointMeasure == true) && (0 < CData.Dev.i18PStripCount)) //210104 jhc : DEBUG : SAVE 버튼 누르기 전 선택한 18 Point 수로 비교해야 함 //(0 < cmbO_18PStripCount.SelectedIndex))
            {
                if (iUse18PCnt[0] <= 0)
                { return (20); } //Test Strip Before Measure Point Count = 0
                else if (iUse18PCnt[1] <= 0)
                { return (21); } //Test Strip After Measure Point Count = 0
            }
            //

            return result;
        }
        //

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
                    txtP_CtLChipOneWin.Text = (mCell.RowIndex / iRow + 1).ToString();
                }
            }

            _DrawOnePoint();
        }

        /// <summary>
        /// Chip 배열 변경시 One Point Measure 측정 Chip 선택 해제
        /// 배열이 변경되면 없는 배열에 접근할 수가 있어 추가
        /// </summary>
        /// <returns>-true=Chip 배열 변경, false=Chip 배열 미 변경</returns>
        private bool _CheckParaChange()
        {
            bool bRet = false;

            if (int.Parse(txtP_CtLChipC.Text) != CData.Dev.iCol
                || int.Parse(txtP_CtLChipR.Text) != CData.Dev.iRow
                || int.Parse(txtP_CtLWinCnt.Text) != CData.Dev.iWinCnt)
            {
                bRet = true;
            }

            return bRet;
        }

        /// <summary>
        /// One Point 측정 위치 변경관련 DataGrid에 그려주는 함수
        /// </summary>
        private void _DrawOnePoint()
        {
            if (CDataOption.Is1Point == false)
            { return; }

            int iCol = CData.Dev.iCol;
            int iRow = CData.Dev.iRow;
            int iWinCnt = CData.Dev.iWinCnt;
            int iTotalRow = iRow * iWinCnt;

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

            for (int k = 0; k < iWinCnt; k++)
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
            int iSelectRow = int.Parse(txtP_CtLChipOneRow.Text) + iRow * (int.Parse(txtP_CtLChipOneWin.Text) - 1);

            if (iWinCnt == 0 || iCol == 0 || iRow == 0 || iSelectCol > iCol || iSelectRow > iTotalRow || int.Parse(txtP_CtLChipOneWin.Text) == 0)
            {
                txtP_CtLChipOneWin.Text = "0";
                txtP_CtLChipOneCol.Text = "0";
                txtP_CtLChipOneRow.Text = "0";
            }

            if (iSelectCol == 0 || iSelectRow == 0 || txtP_CtLChipOneWin.Text == "0" || txtP_CtLChipOneCol.Text == "0" || txtP_CtLChipOneRow.Text == "0")
            {
                txtP_CtLChipOneWin.Text = "0";
                txtP_CtLOneXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                if (iCol == 0 || iRow == 0 || iWinCnt == 0)
                {
                    txtP_CtLOneYPos.Text = "0.000";
                    txtP_CtLOneAxisLYPos.Text = "0.000";
                    txtP_CtLOneAxisRYPos.Text = "0.000";
                    if (CData.Dev.bDual == eDual.Normal)
                    {
                        txtP_CtLOneAxisLXPos.Text = CData.SPos.dGRD_X_Zero[(int)EWay.L].ToString("0.000");
                        txtP_CtLOneAxisRXPos.Text = CData.SPos.dGRD_X_Zero[(int)EWay.R].ToString("0.000");
                    }
                    else
                    {
                        txtP_CtLOneAxisLXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                    }
                }
                else
                {
                    txtP_CtLOneYPos.Text = (CData.Dev.aData[m_iWy].aPosBf[(iRow / 2), 0].dY - CData.MPos[m_iWy].dY_PRB_TBL_CENTER).ToString("0.000");
                    if (CData.Dev.bDual == eDual.Normal)
                    {
                        txtP_CtLOneAxisLXPos.Text = CData.SPos.dGRD_X_Zero[(int)EWay.L].ToString("0.000");
                        txtP_CtLOneAxisLYPos.Text = CData.Dev.aData[(int)EWay.L].aPosBf[(iRow / 2), 0].dY.ToString("0.000");
                        txtP_CtLOneAxisRXPos.Text = CData.SPos.dGRD_X_Zero[(int)EWay.R].ToString("0.000");
                        txtP_CtLOneAxisRYPos.Text = CData.Dev.aData[(int)EWay.R].aPosBf[(iRow / 2), 0].dY.ToString("0.000");
                    }
                    else
                    {
                        txtP_CtLOneAxisLXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                        txtP_CtLOneAxisLYPos.Text = CData.Dev.aData[m_iWy].aPosBf[(iRow / 2), 0].dY.ToString("0.000");
                    }
                }
            }
            else
            {
                txtP_CtLOneYPos.Text = (-(CData.Dev.aData[m_iWy].aPosBf[(iSelectRow - 1), 0].dY - CData.MPos[m_iWy].dY_PRB_TBL_CENTER)).ToString("0.000");
                if(ckb_OnePointXPosFix.Checked)
                {
                    txtP_CtLOneXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                    txtP_CtLOneAxisLXPos.Text = CData.SPos.dGRD_X_Zero[m_iWy].ToString("0.000");
                }
                else
                {
                    if (CData.Dev.bDual == eDual.Normal)
                    {
                        txtP_CtLOneXPos.Text = CData.Dev.aData[m_iWy].aPosBf[0, (iSelectCol - 1)].dX.ToString("0.000");
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
                }
                if (CData.Dev.bDual == eDual.Normal)
                {
                    txtP_CtLOneAxisLXPos.Text = CData.Dev.aData[(int)EWay.L].aPosBf[0, (iSelectCol - 1)].dX.ToString("0.000");
                    txtP_CtLOneAxisRXPos.Text = CData.Dev.aData[(int)EWay.R].aPosBf[0, (iSelectCol - 1)].dX.ToString("0.000");
                    txtP_CtLOneAxisLYPos.Text = CData.Dev.aData[(int)EWay.L].aPosBf[(iSelectRow - 1), 0].dY.ToString("0.000");
                    txtP_CtLOneAxisRYPos.Text = CData.Dev.aData[(int)EWay.R].aPosBf[(iSelectRow - 1), 0].dY.ToString("0.000");
                }
                else
                {
                    txtP_CtLOneAxisLXPos.Text = CData.Dev.aData[m_iWy].aPosBf[0, (iSelectCol - 1)].dX.ToString("0.000");
                    txtP_CtLOneAxisLYPos.Text = CData.Dev.aData[m_iWy].aPosBf[(iSelectRow - 1), 0].dY.ToString("0.000");
                }
            }

            int iRowMin = Math.Min(iRow * iWinCnt, CData.Dev.aData[m_iWy].aPosBf.GetLength(0));
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

        /// <summary>
        /// One Point 측정 시 기존 위치 사용
        /// </summary>
        private void OnePointDefault(object sender, EventArgs e)
        {
            txtP_CtLChipOneWin.Text = "0";
            txtP_CtLChipOneCol.Text = "0";
            txtP_CtLChipOneRow.Text = "0";

            _DrawOnePoint();
        }

        /// <summary>
        /// ckb_OnePointXPosFix 체크 시 X축 값은 고정 Y축 값만 변경됨
        /// ckb_OnePointXPosFix 미 체크 시 X, Y축 값 모두 변경 됨
        /// </summary>
        private void OnePointXPosFix(object sender, EventArgs e)
        {
            _DrawOnePoint();
        }
        // 2020.08.31 JSKim Ed
    }
}
