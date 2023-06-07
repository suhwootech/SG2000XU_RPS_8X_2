using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_07Grr : UserControl
    {
        private int m_iWy = (int)EWay.R;
        private int m_iCol = 0;
        private int m_iRow = 0;
        private int m_iCnt = 0;
        private double m_dStripTick; //190226 ksg
        private int m_iStepMaxCnt = 0;      // 2020.09.08 SungTae : 3 Step Mode 추가
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_07Grr()
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

            if(CDataOption.TableClean == eTableClean.NotUse) //200102 ksg :
            {
                btn_TbClean.Visible = false;
            }
            else btn_TbClean.Visible = true;

             if (CDataOption.Package == ePkg.Unit)
            {
                lbl_Unit1.Visible = true;
                lbl_Unit2.Visible = true;
                lbl_Unit3.Visible = true;
                lbl_Unit4.Visible = true;
            }

            //syc : new cleaner
            if (CDataOption.UseNewClenaer)
            {
                lblGrR_TcDn.Visible = false;
                ckbGrR_TcDn.Visible = false;
            }

            // 2020.09.08 SungTae : 3 Step Mode
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
            else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

            if (CDataOption.UseNewClenaer) // syc : new cleaner
            { ckbGrR_TcWKn.Text = "TOP" + "\n" + "CLEANER" + "\n" + "POINT WATER" + "\n" + "OFF"; }

        }

        private void ViewUpdate(int iStepCnt)
        {
            //if (iStepCnt == 12)
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)      // 2020.10.09 SungTae : Modify
            {
                pnlGrR_R4.Visible = true;
                pnlGrR_R5.Visible = true;
                pnlGrR_R6.Visible = true;
                pnlGrR_R7.Visible = true;
                pnlGrR_R8.Visible = true;
                pnlGrR_R9.Visible = true;
                pnlGrR_R10.Visible = true;
                pnlGrR_R11.Visible = true;
                pnlGrR_R12.Visible = true;
            }
            else
            {
                pnlGrR_R4.Visible = false;
                pnlGrR_R5.Visible = false;
                pnlGrR_R6.Visible = false;
                pnlGrR_R7.Visible = false;
                pnlGrR_R8.Visible = false;
                pnlGrR_R9.Visible = false;
                pnlGrR_R10.Visible = false;
                pnlGrR_R11.Visible = false;
                pnlGrR_R12.Visible = false;
            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                // Grind Right X
                if (CMot.It.Chk_Alr((int)EAx.RightGrindZone_X)) { lbl_XStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP((int)EAx.RightGrindZone_X) == EAxOp.Idle)
                {
                    lbl_XStat.BackColor = Color.LightGray;
                }
                else { lbl_XStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl_XPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_X).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl_XPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_X).ToString("0.000");
                }
                else
                {
                    lbl_XPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_X).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Grind Right Y
                if (CMot.It.Chk_Alr((int)EAx.RightGrindZone_Y)) { lbl_YStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP((int)EAx.RightGrindZone_Y) == EAxOp.Idle)
                {
                    lbl_YStat.BackColor = Color.LightGray;
                }
                else { lbl_YStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl_YPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_Y).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl_YPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_Y).ToString("0.000");
                }
                else
                {
                    lbl_YPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_Y).ToString();
                }
                // 2020.11.27 JSKim Ed
                // Grind Right Z
                if (CMot.It.Chk_Alr((int)EAx.RightGrindZone_Z)) { lbl_ZStat.BackColor = Color.Red; }
                if (CMot.It.Chk_OP((int)EAx.RightGrindZone_Z) == EAxOp.Idle)
                {
                    lbl_ZStat.BackColor = Color.LightGray;
                }
                else { lbl_ZStat.BackColor = Color.Lime; }
                // 2020.11.27 JSKim St
                //lbl_ZPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_Z).ToString();
                if (CData.CurCompany == ECompany.JCET)
                {
                    lbl_ZPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_Z).ToString("0.000");
                }
                else
                {
                    lbl_ZPos.Text = CMot.It.Get_FP((int)EAx.RightGrindZone_Z).ToString();
                }
                // 2020.11.27 JSKim Ed


                //210929 syc : 2004U
                if (CDataOption.Use2004U)
                {
                    int iCheckTbVac = 0;

                    if (CIO.It.aInput[(int)eX.GRDR_Carrier_Vacuum_4U] == 1 &&
                        CIO.It.aInput[(int)eX.GRDR_Unit_Vacuum_4U] == 1)
                    {
                        iCheckTbVac = 1;
                    }
                    lbl_TblVac.BackColor = GV.CR_X[iCheckTbVac];
                }
                else { lbl_TblVac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TbVaccum]]; }
                //syc end
                
                lblGrR_TblFlw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TbFlow]];
                lblGrR_TcDn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TopClnDn]];
                lblGrR_TcFlw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TopClnFlow]];
                lblGrR_PrbAMP.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_ProbeAMP]];
                lblGrR_SplZig.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_WheelZig]];
                lblGrR_SplPcw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_SplPCW]];
                lblGrR_GrdFlw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_SplWater]];
                lblGrR_BtmFlw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_SplBtmWater]];
                // 190711-maeng Tool Setter 감지
                lblGrR_TSetter.BackColor = GV.CR_X[CIO.It.Get_TS(EWay.R)];

                //210903 syc : 2004U
                if (CDataOption.Use2004U)
                {
                    ckb_Vac.Checked = CIO.It.Get_Y(eY.GRDR_Unit_Vacuum_4U) && CIO.It.Get_Y(eY.GRDR_Carrier_Vacuum_4U);
                }
                else
                {
                    ckb_Vac.Checked = CIO.It.Get_Y(eY.GRDR_TbVacuum);
                }                
                ckbGrR_Wtr.Checked = CIO.It.Get_Y(eY.GRDR_TbFlow);
                ckb_Ejt.Checked = CIO.It.Get_Y(eY.GRDR_TbEjector);
                ckbGrR_TcDn.Checked = CIO.It.Get_Y(eY.GRDR_TopClnDn);
                ckbGrR_TcWKn.Checked = CIO.It.Get_Y(eY.GRDR_TopWaterKnife);
                ckbGrR_TcAKn.Checked = CIO.It.Get_Y(eY.GRDR_TopClnAir);
                ckbGrR_TcW.Checked = CIO.It.Get_Y(eY.GRDR_TopClnFlow);
            }

            //190319 ksg -> 190325 maeng 수정
            lblGrR_BMax.Text = CData.PbResultVal[1].dBMax.ToString("F3");
            lblGrR_BMin.Text = CData.PbResultVal[1].dBMin.ToString("F3");
            lblGrR_BAvg.Text = CData.PbResultVal[1].dBAvg.ToString("F3");
            lblGrR_AMax.Text = CData.PbResultVal[1].dAMax.ToString("F3");
            lblGrR_AMin.Text = CData.PbResultVal[1].dAMin.ToString("F3");
            lblGrR_AAvg.Text = CData.PbResultVal[1].dAAvg.ToString("F3");

            //20191010 ghk_manual_bcr
            lblGrR_Bcr.Text = CData.Parts[(int)EWay.R].sBcr;

            if (CData.Dev.iCol > 0 && CData.Dev.iRow > 0)
            {
                int iCol = CData.Dev.iCol;
                int iRow = CData.Dev.iRow * CData.Dev.iWinCnt;

                if (CDataOption.Package == ePkg.Strip)
                {
                    if (CData.GrData[m_iWy].aMeaBf.GetLength(0) == iRow)
                    {
                        for (int i = 0; i < iCol; i++)
                        {
                            for (int j = 0; j < iRow; j++)
                            {
                                if (CData.GrData[m_iWy].aMeaBf != null)
                                {
                                    //190116 ksg : 에러남 // 200410 pjh : Range Over 시 Color 변경
                                    dgvGrR_Bf.Rows[j].Cells[i].Style.ForeColor = (CData.R_GRD.m_abMeaBfErr[j, i]) ? Color.Red : Color.Black;
                                    dgvGrR_Bf.Rows[j].Cells[i].Value = CData.GrData[m_iWy].aMeaBf[j, i];
                                }

                                if (CData.GrData[m_iWy].aMeaAf != null)
                                {
                                    dgvGrR_Af.Rows[j].Cells[i].Style.ForeColor = (CData.R_GRD.m_abMeaBfErr[j, i]) ? Color.Red : Color.Black;
                                    dgvGrR_Af.Rows[j].Cells[i].Value = CData.GrData[m_iWy].aMeaAf[j, i];
                                }
                            }
                        }
                    }
                }
                else // Unit mode
                {
                    iRow = CData.Dev.iRow;

                    if (CData.GrData[m_iWy].aUnit[0].aMeaBf.GetLength(0) == iRow && CData.GrData[m_iWy].aUnit[0].aErrBf.GetLength(0) == iRow &&
                        CData.GrData[m_iWy].aUnit[0].aMeaBf.GetLength(1) == iCol && CData.GrData[m_iWy].aUnit[0].aErrBf.GetLength(1) == iCol)
                    {
                        for (int i = 0; i < iCol; i++)
                        {
                            for (int iU = CData.Dev.iUnitCnt - 1; iU >= 0; iU--)
                            {
                                for (int j = 0; j < iRow; j++)
                                {
                                    // 현재 Row 인덱스 계산
                                    int iR = j + ((CData.Dev.iUnitCnt - (iU + 1)) * iRow);

                                    if (CData.GrData[m_iWy].aUnit[iU].aMeaBf != null)
                                    {
                                        dgvGrR_Bf.Rows[iR].Cells[i].Style.ForeColor = (CData.GrData[m_iWy].aUnit[iU].aErrBf[j, i]) ? Color.Red : Color.Black;
                                        dgvGrR_Bf.Rows[iR].Cells[i].Value = CData.GrData[m_iWy].aUnit[iU].aMeaBf[j, i];
                                    }

                                    if (CData.GrData[m_iWy].aUnit[iU].aMeaAf != null)
                                    {
                                        dgvGrR_Af.Rows[iR].Cells[i].Style.ForeColor = (CData.GrData[m_iWy].aUnit[iU].aErrAf[j, i]) ? Color.Red : Color.Black;
                                        dgvGrR_Af.Rows[iR].Cells[i].Value = CData.GrData[m_iWy].aUnit[iU].aMeaAf[j, i];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (CSQ_Man.It.Seq == ESeq.GRR_Grinding)
            {
                // 2021.12.11 lhs Start : CData.R_GRD.m_iIndex 일때만 데이터를 표시되는 문제 수정
                if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt; }
                else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3; }

                for (int i = 0; i < m_iStepMaxCnt - 1; i++)
                {
                    bool bStepUse = false; 
                    if (CData.Dev.bDual == eDual.Dual)  { if (CData.Dev.aData[(int)EWay.R].aSteps[i].bUse)  bStepUse = true; }  
                    else                                { if (CData.Dev.aData[(int)EWay.L].aSteps[i].bUse)  bStepUse = true; }

                    if (bStepUse)
                    {
                        if (i == CData.R_GRD.m_iIndex)  { ((Label)(Controls.Find("lblGrR_R" + (i + 1), true)[0])).BackColor = Color.Lime;       }
                        else                            { ((Label)(Controls.Find("lblGrR_R" + (i + 1), true)[0])).BackColor = Color.Gainsboro;  }

                        ((Label)(Controls.Find("lblGrR_R" + (i + 1) + "Tar",    true)[0])).Text = CData.GrData[m_iWy].aTar[i].ToString();   //m_iWy = (int)EWay.R;
                        ((Label)(Controls.Find("lblGrR_R" + (i + 1) + "Cnt",    true)[0])).Text = string.Format("{0} / {1}", CData.GrData[m_iWy].aInx[i].ToString("00"),    CData.GrData[m_iWy].aCnt[i].ToString("00"));
                        ((Label)(Controls.Find("lblGrR_R" + (i + 1) + "One",    true)[0])).Text = CData.GrData[m_iWy].a1Pt[i].ToString("F3");
                        ((Label)(Controls.Find("lblGrR_R" + (i + 1) + "ReCnt",  true)[0])).Text = string.Format("{0} / {1}", CData.GrData[m_iWy].aReInx[i].ToString("00"),  CData.GrData[m_iWy].aReCnt[i].ToString("00")); 
                        ((Label)(Controls.Find("lblR_R"   + (i + 1) + "Num",    true)[0])).Text = CData.GrData[m_iWy].aReNum[i].ToString();
                    }
                }

                bool bFiStepUse = false;
                if (CData.Dev.bDual == eDual.Dual)  { if (CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].bUse)  bFiStepUse = true; }
                else                                { if (CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].bUse)  bFiStepUse = true; }
                if (bFiStepUse)
                {
                    if (CData.R_GRD.m_iIndex == m_iStepMaxCnt - 1)  { lblGrR_Fi.BackColor = Color.Lime;      }
                    else                                            { lblGrR_Fi.BackColor = Color.Gainsboro; }

                    lblGrR_FiTar.Text   = CData.GrData[m_iWy].aTar[m_iStepMaxCnt - 1].ToString();
                    lblGrR_FiCnt.Text   = string.Format("{0} / {1}", CData.GrData[m_iWy].aInx[m_iStepMaxCnt - 1].ToString("00"),    CData.GrData[m_iWy].aCnt[m_iStepMaxCnt - 1].ToString("00"));
                    lblGrR_FiReCnt.Text = string.Format("{0} / {1}", CData.GrData[m_iWy].aReInx[m_iStepMaxCnt - 1].ToString("00"),  CData.GrData[m_iWy].aReCnt[m_iStepMaxCnt - 1].ToString("00"));
                    lblR_FiNum.Text     = CData.GrData[m_iWy].aReNum[m_iStepMaxCnt - 1].ToString();
                }
                    
                ////////////////////////   확인시 삭제요
                //int Index = CData.R_GRD.m_iIndex;

                //if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
                //else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

                //if (CData.R_GRD.m_iIndex < m_iStepMaxCnt - 1)
                //{
                //    ((Label)(Controls.Find("lblGrR_R" +(Index+1)          , true)[0])).BackColor = Color.Lime;
                //    ((Label)(Controls.Find("lblGrR_R" +(Index+1) + "Cnt"  , true)[0])).Text      = string.Format("{0} / {1}", CData.GrData[m_iWy].aInx[Index].ToString("00"), CData.GrData[m_iWy].aCnt[Index].ToString("00")); //200824 jhc : //CData.R_GRD.m_aGrd[Index]);
                //    ((Label)(Controls.Find("lblGrR_R" +(Index+1) + "One"  , true)[0])).Text      = CData.GrData[(int)EWay.R].a1Pt[Index].ToString("F3");
                //    ((Label)(Controls.Find("lblGrR_R" +(Index+1) + "ReCnt", true)[0])).Text      = string.Format("{0} / {1}", CData.GrData[m_iWy].aReInx[Index].ToString("00"), CData.GrData[m_iWy].aReCnt[Index].ToString("00")); //200824 jhc : //CData.R_GRD.m_aReGrd[Index].ToString("00"));
                //    ((Label)(Controls.Find("lblR_R"   +(Index+1) + "Num"  , true)[0])).Text      = CData.GrData[(int)EWay.R].aReNum[Index].ToString();
                //}
                //else if (CData.R_GRD.m_iIndex == m_iStepMaxCnt - 1)
                //{
                //    lblGrR_Fi     .BackColor = Color.Lime;
                //    lblGrR_FiCnt  .Text      = string.Format("{0} / {1}", CData.GrData[m_iWy].aInx[Index].ToString("00"), CData.GrData[m_iWy].aCnt[Index].ToString("00")); //200824 jhc : //CData.R_GRD.m_aGrd[Index]);
                //    lblGrR_FiReCnt.Text      = string.Format("{0} / {1}", CData.GrData[m_iWy].aReInx[Index].ToString("00"), CData.GrData[m_iWy].aReCnt[Index].ToString("00")); //200824 jhc : //CData.R_GRD.m_aReGrd[Index].ToString("00"));
                //    lblR_FiNum    .Text      = CData.GrData[(int)EWay.R].aReNum[Index].ToString();
                //}
                //for(int i = 0; i < m_iStepMaxCnt - 1; i++)  
                //{
                //    ((Label)(Controls.Find("lblGrR_R" +(i+1) + "Tar", true)[0])).Text = CData.GrData[1].aTar[i].ToString();
                //}
                //if(Index > 0)
                //{ 
                //    for(int i = 0; i < Index; i++)
                //    {
                //        if (Index > m_iStepMaxCnt - 1) continue; 
                //        ((Label)(Controls.Find("lblGrR_R" +(i+1), true)[0])).BackColor = Color.Gainsboro;
                //    }
                //}

                //if (Index < m_iStepMaxCnt - 1) lblGrR_Fi.BackColor = Color.Gainsboro;
                //lblGrR_FiTar.Text = CData.GrData[1].aTar[m_iStepMaxCnt - 1].ToString();

                ////////////////////////
                ///


                // 2021.12.11 lhs End : CData.R_GRD.m_iIndex 일때만 데이터를 표시되는 문제 수정
            }

            //--------- aSteps[i].bUse 만 표시
            for (int i = 0; i < m_iStepMaxCnt - 1; i++)
            {
                if (CData.Dev.bDual == eDual.Dual)
                {
                    if (!CData.Dev.aData[(int)EWay.R].aSteps[i].bUse) ((Panel)(Controls.Find("pnlGrR_R" +(i+1), true)[0])).Visible = false;
                    else                                              ((Panel)(Controls.Find("pnlGrR_R" +(i+1), true)[0])).Visible = true ;
                }
                else
                {
                    if (!CData.Dev.aData[(int)EWay.L].aSteps[i].bUse) ((Panel)(Controls.Find("pnlGrR_R" +(i+1), true)[0])).Visible = false;
                    else                                              ((Panel)(Controls.Find("pnlGrR_R" +(i+1), true)[0])).Visible = true ;
                }
            }
            
            if (CData.Dev.bDual == eDual.Dual)
            {
                if (CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].bUse) pnlGrR_Fi.Visible = true;
                else                                                             pnlGrR_Fi.Visible = false;
            }
            else
            {
                if (CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].bUse) pnlGrR_Fi.Visible = true;
                else                                                             pnlGrR_Fi.Visible = false;
            }
            //--------- 

            lbl_GrdTime.Text = CData.GrdElp[(int)EWay.R].tsEls.ToString(@"hh\:mm\:ss");
            lbl_DrsTime.Text = CData.DrsElp[(int)EWay.R].tsEls.ToString(@"hh\:mm\:ss");

            // Dressing Count 갱신
            lbl6_UDrs1Cnt.Text = CData.DrData[1].aInx[0] + " / " + CData.DrData[1].aCnt[0];
            lbl6_UDrs2Cnt.Text = CData.DrData[1].aInx[1] + " / " + CData.DrData[1].aCnt[1];

            if (CDataOption.Package == ePkg.Unit)
            {
                lbl_Unit1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_Unit_1_Vacuum]];
                lbl_Unit2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_Unit_2_Vacuum]];
                lbl_Unit3.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_Unit_3_Vacuum]];
                lbl_Unit4.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_Unit_4_Vacuum]];
            }

            // 2020.11.23 JSKim St
            // Dressing Left Parameter
            if (tabGrR_StripInsp.SelectedIndex == 3)
            {
                //double dDrsAirCut = CData.Whls[(int)EWay.R].dDair;
                //211013 pjh : Device Wheel 사용 시 표시되는 air cut 변경
                double dDrsAirCut = 0;
                if (CDataOption.UseDeviceWheel)
                {
                    dDrsAirCut = CDev.It.a_tWhl[(int)EWay.R].dDair;

                    if (CDataOption.IsDrsAirCutReplace && CData.DrData[1].bDrsR)
                    {
                        dDrsAirCut = CDev.It.a_tWhl[(int)EWay.R].dDairRep;
                    }
                }
                else
                {
                    dDrsAirCut = CData.Whls[(int)EWay.R].dDair;
                    if (CDataOption.IsDrsAirCutReplace && CData.DrData[1].bDrsR)
                    {
                        dDrsAirCut = CData.Whls[(int)EWay.R].dDairRep;
                    }
                }

                lbl6_DrsAir.Text = dDrsAirCut.ToString();

                //if (CData.Whls[1].aNewP != null && CData.Whls[1].aUsedP != null) //190430 ksg : 조건 바꿈
                if ((CData.Whls[1].aNewP != null && CData.Whls[1].aUsedP != null) || (CDataOption.UseDeviceWheel && CDev.It.a_tWhl[1].aNewP != null && CDev.It.a_tWhl[1].aUsedP != null))
                {
                    if (CDataOption.UseDeviceWheel)
                    {
                        if (CData.DrData[1].bDrsR) { CData.DrData[1].aParm = CDev.It.a_tWhl[1].aNewP; }
                        else { CData.DrData[1].aParm = CDev.It.a_tWhl[1].aUsedP; }
                    }
                    else
                    {
                        if (CData.DrData[1].bDrsR) { CData.DrData[1].aParm = CData.Whls[1].aNewP; }
                        else { CData.DrData[1].aParm = CData.Whls[1].aUsedP; }
                    }

                    lbl6_UDrs1Tot.Text = CData.DrData[1].aParm[0].dTotalDep.ToString();
                    lbl6_UDrs1Cyl.Text = CData.DrData[1].aParm[0].dCycleDep.ToString();
                    lbl6_UDrs1Tbl.Text = CData.DrData[1].aParm[0].dTblSpd.ToString();
                    lbl6_UDrs1Spl.Text = CData.DrData[1].aParm[0].iSplSpd.ToString();
                    lbl6_UDrs2Tot.Text = CData.DrData[1].aParm[1].dTotalDep.ToString();
                    lbl6_UDrs2Cyl.Text = CData.DrData[1].aParm[1].dCycleDep.ToString();
                    lbl6_UDrs2Tbl.Text = CData.DrData[1].aParm[1].dTblSpd.ToString();
                    lbl6_UDrs2Spl.Text = CData.DrData[1].aParm[1].iSplSpd.ToString();
                }
            }
            // 2020.11.23 JSKim Ed
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

        public void _EnableBtn(bool bVal)
        {
            btn_H.Enabled = bVal;
            btn_Wait.Enabled = bVal;
            btn_Grd.Enabled = bVal;
            btn_Drs.Enabled = bVal;
            btn_WaterKnife.Enabled = bVal;
            btn_TbClean.Enabled = bVal;
            ckb_Vac.Enabled = bVal;
            ckb_Ejt.Enabled = bVal;
            ckbGrR_Wtr.Enabled = bVal;
            ckbGrR_TcDn.Enabled = bVal;
            ckbGrR_TcWKn.Enabled = bVal;
            ckbGrR_TcAKn.Enabled = bVal;
            ckbGrR_TcW.Enabled = bVal;
            btnGrR_StInsp.Enabled = bVal;
            btnGrR_WhInsp.Enabled = bVal;
            btnGrR_DrInsp.Enabled = bVal;
        }

        /// <summary>
        /// Manual right grind view에 조작 로그 저장 함수
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
        //SetConvertLanguage(lblS_GrdL_PumpA.Name,CData.Opt.iSelLan,"Overload");
        private void SetWriteLanguage(string controlName, string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }

        private void _Draw()
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

            // 측정 배열이 바뀌지 않았다면, datagridview를 다시 그리지 않는다.
            // 191106-maeng 같은 배열에 측정위치만 바뀌면 다시 그리지를 않는다
            //if (m_iCol == iCol && m_iRow == iRow && m_iCnt == iCnt)
            //{
            //    return;
            //}

            // 200317 mjy : 조건 추가
            if (CDataOption.Package == ePkg.Unit)
            {
                iCnt = CData.Dev.iUnitCnt;
            }

            m_iCol = iCol;
            m_iRow = iRow;
            m_iCnt = iCnt;

            dgvGrR_Bf.Rows.Clear();
            dgvGrR_Bf.Columns.Clear();
            dgvGrR_Af.Rows.Clear();
            dgvGrR_Af.Columns.Clear();

            for (int i = 0; i < iCol; i++)
            {
                dgvGrR_Bf.Columns.Add(i.ToString(), i.ToString());
                dgvGrR_Af.Columns.Add(i.ToString(), i.ToString());
            }

            for (int k = 0; k < iCnt; k++)
            {
                int iColor = 255 - (k * 10);

                // 200317 mjy : Unit에서는 홀수, 짝수 유닛의 배경색 구분
                if (CDataOption.Package == ePkg.Unit)
                {
                    // 홀수
                    if (k % 2 != 0)
                    { iColor = 200; }
                    else // 짝수
                    { iColor = 255; }
                }

                Color tColor = Color.FromArgb(iColor, iColor, iColor);

                for (int j = 0; j < iRow; j++)
                {
                    dgvGrR_Bf.Rows.Add();
                    dgvGrR_Bf.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                    dgvGrR_Af.Rows.Add();
                    dgvGrR_Af.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                }
            }

            for (int i = 0; i < iCol; i++)
            {
                if (CDataOption.Package == ePkg.Strip)
                {
                    for (int j = 0; j < iRow * iCnt; j++)
                    {
                        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        if ((CDataOption.Use18PointMeasure == true) && (0 < CData.Dev.i18PStripCount)) //Leader Strip Count 0일 경우 Leader Strip 용으로 설정된 Contact Point 표시 안 함
                        {
                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[1].aPosBf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[1].aPosBf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvGrR_Bf.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvGrR_Af.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                        }
                        else
                        {
                            if (CData.Dev.aData[1].aPosBf[j, i].bUse)
                            { dgvGrR_Bf.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse)
                            { dgvGrR_Af.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
                        }
                        //
                    }
                }
                else
                {
                    for (int j = 0; j < iRow; j++)
                    {
                        for (int iU = 1; iU <= CData.Dev.iUnitCnt; iU++)
                        {
                            // 현재 Row 인덱스 계산
                            int iR = j + ((iU - 1) * iRow);

                            if (CData.Dev.aData[m_iWy].aPosBf[j, i].bUse)
                            { dgvGrR_Bf.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[m_iWy].aPosAf[j, i].bUse)
                            { dgvGrR_Af.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
                        }
                    }
                }
            }
        }
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            double iSum = 0;

            //200211 ksg :
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            _Draw();

            // 2020.09.08 SungTae : 3 Step Mode
            ViewUpdate(CDataOption.StepCnt);

            iSum = 0;
            //190221 ksg :
            if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Target)
            {
                SetWriteLanguage(lblGrR_Mod.Name, CLang.It.GetLanguage("Target"));
                // 2021.11.26 lhs Start
                if (CDataOption.UseNewSckGrindProc)
                {
                    if (CData.Dev.aData[(int)EWay.R].eBaseOnThick == EBaseOnThick.Mold)     { lblGrR_Mod.Text += " / Mold";     }
                    if (CData.Dev.aData[(int)EWay.R].eBaseOnThick == EBaseOnThick.Total)    { lblGrR_Mod.Text += " / Total";    }
                }
                // 2021.11.26 lhs End
            }
            else
            {
                SetWriteLanguage(lblGrR_Mod.Name, CLang.It.GetLanguage("TopDown"));
            }

            //200414 ksg : 12 Step  기능 추가
            /*
            // Rough 1
            pnlGrR_R1.Enabled = CData.Dev.aData[(int)EWay.R].aSteps[0].bUse;
            //lblGrR_R1ToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[0].dTotalDep.ToString(); //190226 ksg :
            if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Normal)
            {
                if (CData.Dev.bDual == eDual.Normal)
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh;
                }
                else
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.R].dTotalTh;
                }
                iSum = m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[0].dTotalDep;
                lblGrR_R1ToTh.Text = m_dStripTick.ToString();
            }
            else
            {
                lblGrR_R1ToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[0].dTotalDep.ToString();
            }
            lblGrR_R1CyTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[0].dCycleDep.ToString();
            lblGrR_R1Tbl.Text = CData.Dev.aData[(int)EWay.R].aSteps[0].dTblSpd.ToString();
            lblGrR_R1Spl.Text = CData.Dev.aData[(int)EWay.R].aSteps[0].iSplSpd.ToString();

            // Rough 2
            pnlGrR_R2.Enabled = CData.Dev.aData[(int)EWay.R].aSteps[1].bUse;
            //lblGrR_R2ToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[1].dTotalDep.ToString(); //190226 ksg :
            if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Normal && CData.Dev.aData[(int)EWay.R].aSteps[1].bUse)
            {
                if (CData.Dev.bDual == eDual.Normal)
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                }
                else
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.R].dTotalTh - iSum;
                }
                iSum += m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[1].dTotalDep;
                lblGrR_R2ToTh.Text = m_dStripTick.ToString();
            }
            else
            {
                lblGrR_R2ToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[1].dTotalDep.ToString();
            }
            lblGrR_R2CyTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[1].dCycleDep.ToString();
            lblGrR_R2Tbl.Text = CData.Dev.aData[(int)EWay.R].aSteps[1].dTblSpd.ToString();
            lblGrR_R2Spl.Text = CData.Dev.aData[(int)EWay.R].aSteps[1].iSplSpd.ToString();

            // Rough 3
            pnlGrR_R3.Enabled = CData.Dev.aData[(int)EWay.R].aSteps[2].bUse;
            //lblGrR_R3ToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[2].dTotalDep.ToString(); //190226 ksg :
            if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Normal && CData.Dev.aData[(int)EWay.R].aSteps[2].bUse)
            {
                if (CData.Dev.bDual == eDual.Normal)
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                }
                else
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.R].dTotalTh - iSum;
                }
                iSum += m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[2].dTotalDep;
                lblGrR_R3ToTh.Text = m_dStripTick.ToString();
            }
            else
            {
                lblGrR_R3ToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[2].dTotalDep.ToString();
            }
            lblGrR_R3CyTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[2].dCycleDep.ToString();
            lblGrR_R3Tbl.Text = CData.Dev.aData[(int)EWay.R].aSteps[2].dTblSpd.ToString();
            lblGrR_R3Spl.Text = CData.Dev.aData[(int)EWay.R].aSteps[2].iSplSpd.ToString();

            // Fine
            pnlGrR_Fi.Enabled = CData.Dev.aData[(int)EWay.R].aSteps[3].bUse;
            //lblGrR_FiToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[3].dTotalDep.ToString(); //190226 ksg :
            if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Normal && CData.Dev.aData[(int)EWay.R].aSteps[3].bUse)
            {
                if (CData.Dev.bDual == eDual.Normal)
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                }
                else
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.R].dTotalTh - iSum;
                }
                iSum += m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[3].dTotalDep;
                lblGrR_FiToTh.Text = m_dStripTick.ToString();
            }
            else
            {
                lblGrR_FiToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[3].dTotalDep.ToString();
            }
            lblGrR_FiCyTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[3].dCycleDep.ToString();
            lblGrR_FiTbl.Text = CData.Dev.aData[(int)EWay.R].aSteps[3].dTblSpd.ToString();
            lblGrR_FiSpl.Text = CData.Dev.aData[(int)EWay.R].aSteps[3].iSplSpd.ToString();
            */

            // 2020.09.08 SungTae : 3 Step Mode
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
            else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

            //200414 ksg : 12 Step  기능 추가
            //for(int i = 0; i < GV.StepMaxCnt - 1; i++)
            for (int i = 0; i < m_iStepMaxCnt - 1; i++)     // 2020.09.08 SungTae : Modify
            {
                ((Panel)(Controls.Find("pnlGrR_R" +(i+1), true)[0])).Visible = CData.Dev.aData[(int)EWay.R].aSteps[i].bUse;
                if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Target && CData.Dev.aData[(int)EWay.R].aSteps[i].bUse)
                {
                    if (CData.Dev.bDual == eDual.Normal) m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                    else                                 m_dStripTick = CData.Dev.aData[(int)EWay.R].dTotalTh - iSum;
                    if(i == 0) iSum  = m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[i].dTotalDep;
                    else       iSum += m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[i].dTotalDep;
                    ((Label)(Controls.Find("lblGrR_R" +(i+1) + "ToTh", true)[0])).Text = m_dStripTick.ToString();
                }
                else
                {
                    ((Label)(Controls.Find("lblGrR_R" +(i+1) + "ToTh", true)[0])).Text = CData.Dev.aData[(int)EWay.R].aSteps[i].dTotalDep.ToString();
                }
                ((Label)(Controls.Find("lblGrR_R" +(i+1) + "CyTh", true)[0])).Text = CData.Dev.aData[(int)EWay.R].aSteps[i].dCycleDep.ToString();
                ((Label)(Controls.Find("lblGrR_R" +(i+1) + "Tbl" , true)[0])).Text = CData.Dev.aData[(int)EWay.R].aSteps[i].dTblSpd  .ToString();
                ((Label)(Controls.Find("lblGrR_R" +(i+1) + "Spl" , true)[0])).Text = CData.Dev.aData[(int)EWay.R].aSteps[i].iSplSpd  .ToString();
            }
            // Fine
            //pnlGrR_Fi.Enabled = CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt - 1].bUse;
            pnlGrR_Fi.Enabled = CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].bUse;        // 2020.09.08 SungTae : Modify

            //lblGrR_FiToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[3].dTotalDep.ToString(); //190226 ksg :
            if (CData.Dev.aData[(int)EWay.R].eGrdMod == eGrdMode.Target && CData.Dev.aData[(int)EWay.R].aSteps[3].bUse)
            {
                if (CData.Dev.bDual == eDual.Normal) m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                else                                 m_dStripTick = CData.Dev.aData[(int)EWay.R].dTotalTh - iSum;
                
                //iSum += m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt - 1].dTotalDep;
                iSum += m_dStripTick - CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].dTotalDep;        // 2020.09.08 SungTae : Modify

                lblGrR_FiToTh.Text = m_dStripTick.ToString();
            }
            else
            {
                //lblGrR_FiToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt - 1].dTotalDep.ToString();
                lblGrR_FiToTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].dTotalDep.ToString();        // 2020.09.08 SungTae : Modify
            }

            // 2020.09.08 SungTae : Modify
            //lblGrR_FiCyTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt - 1].dCycleDep.ToString();
            //lblGrR_FiTbl .Text = CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt - 1].dTblSpd.ToString();
            //lblGrR_FiSpl .Text = CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt - 1].iSplSpd.ToString();
            lblGrR_FiCyTh.Text = CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].dCycleDep.ToString();
            lblGrR_FiTbl.Text  = CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].dTblSpd.ToString();
            lblGrR_FiSpl.Text  = CData.Dev.aData[(int)EWay.R].aSteps[m_iStepMaxCnt - 1].iSplSpd.ToString();

            // Dressing Right Parameter
            // 2020.11.23 JSKim St - 값이 바로 갱신이 안된다는 말이 있어 Timer에서 처리
            //lbl6_DrsAir.Text = CData.Whls[(int)EWay.R].dDair.ToString();
            
            //if (CDataOption.IsDrsAirCutReplace && CData.DrData[1].bDrsR)
            //{
            //    lbl6_DrsAir.Text = CData.Whls[(int)EWay.R].dDairRep.ToString();
            //}

            //// 프로그램 시작시 현재 사용중인 파라메터 파일 비었을시 에러 방지 
            ////if (CData.DrData[1].aParm != null)
            //if (CData.Whls[1].aNewP != null && CData.Whls[1].aUsedP != null) //190430 ksg : 조건 바꿈
            //{
            //    if (CData.DrData[1].bDrsR) { CData.DrData[1].aParm = CData.Whls[1].aNewP; }
            //    else { CData.DrData[1].aParm = CData.Whls[1].aUsedP; }

            //    lbl6_UDrs1Tot.Text = CData.DrData[1].aParm[0].dTotalDep.ToString();
            //    lbl6_UDrs1Cyl.Text = CData.DrData[1].aParm[0].dCycleDep.ToString();
            //    lbl6_UDrs1Tbl.Text = CData.DrData[1].aParm[0].dTblSpd.ToString();
            //    lbl6_UDrs1Spl.Text = CData.DrData[1].aParm[0].iSplSpd.ToString();
            //    lbl6_UDrs2Tot.Text = CData.DrData[1].aParm[1].dTotalDep.ToString();
            //    lbl6_UDrs2Cyl.Text = CData.DrData[1].aParm[1].dCycleDep.ToString();
            //    lbl6_UDrs2Tbl.Text = CData.DrData[1].aParm[1].dTblSpd.ToString();
            //    lbl6_UDrs2Spl.Text = CData.DrData[1].aParm[1].iSplSpd.ToString();
            //}
            // 2020.11.23 JSKim Ed

            ckb_Vac.Checked = CIO.It.Get_Y(eY.GRDR_TbVacuum     );
            ckbGrR_Wtr  .Checked = CIO.It.Get_Y(eY.GRDR_TbFlow       );
            ckb_Ejt     .Checked = CIO.It.Get_Y(eY.GRDR_TbEjector    );
            ckbGrR_TcDn .Checked = CIO.It.Get_Y(eY.GRDR_TopClnDn     );
            ckbGrR_TcWKn.Checked = CIO.It.Get_Y(eY.GRDR_TopWaterKnife);
            ckbGrR_TcAKn.Checked = CIO.It.Get_Y(eY.GRDR_TopClnAir    );
            ckbGrR_TcW  .Checked = CIO.It.Get_Y(eY.GRDR_TopClnFlow   );

            //20191010 ghk_manual_bcr
            if (CDataOption.ManualBcr == eManualBcr.NotUse)
            {
                lblGrR_Bcr.Visible = false;
                btn_Bcr.Visible = false;
            }

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
        #endregion

        private void btnMan_Cycle_Click(object sender, EventArgs e)
        {
            //190627 ksg : 
            _EnableBtn(false);
            CSQ_Man.It.bBtnShow = false;

            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);
            //190226 ksg :추가

            // 2021.10.29 SungTae Start : [추가] Multi-LOT 관련
            if (CData.Opt.bSecsUse) { CData.bChkGrdM = true; }
            else                    { CData.bChkGrdM = false; }
            // 2021.10.29 SungTae End

            _SetLog(string.Format("N, {0} click", ESeq));
            //190414 ksg :
            if (!((ESeq == ESeq.All_Home) || (ESeq == ESeq.All_Servo_On) || (ESeq == ESeq.All_Servo_Off) || (ESeq == ESeq.ONL_Home) ||
                 (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home) || (ESeq == ESeq.GRL_Home) || (ESeq == ESeq.GRR_Home) ||
                 (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home) || (ESeq == ESeq.OFL_Home)))
            {
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }

            if (ESeq == ESeq.GRL_Grinding       || ESeq == ESeq.GRR_Grinding        ||
                ESeq == ESeq.GRL_Strip_Measure  || ESeq == ESeq.GRR_Strip_Measure   ||
                ESeq == ESeq.GRL_Wheel_Measure  || ESeq == ESeq.GRR_Wheel_Measure)
            {
                if (!CSQ_Main.It.CheckWarmUp())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Error, "Error", "Need Warm Up Please");
                    return;
                }
            }

            // 도어 체크  // 2022.06.11 lhs
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (!CSQ_Main.It.CheckDoor()) // SCK,SCK+
                {
                    CSQ_Man.It.bBtnShow = true;
                    CMsg.Show(eMsg.Warning, "Warning", "Door close Please");
                    return;
                }
            }

            // 2020.12.18 JSKim St
            if (CData.CurCompany == ECompany.ASE_K12)
            {
                if (ESeq == ESeq.GRL_Grinding || ESeq == ESeq.GRL_Manual_GrdBcr || 
                    ESeq == ESeq.GRR_Grinding || ESeq == ESeq.GRR_Manual_GrdBcr)
                {
                    if (CSQ_Main.It.CheckDoor() == false)  // ASE_K12
                    {
                        CMsg.Show(eMsg.Error, "Error", "Door Close Please");
                        CSQ_Man.It.bBtnShow = true; //
                        return;
                    }
                }
            }
            // 2020.12.18 JSKim Ed

            //190131 ksg : Table 동작 중 Strip Check 기능 추가
            if (ESeq == ESeq.GRR_Dressing       || ESeq == ESeq.GRR_Dresser_Measure ||
                ESeq == ESeq.GRR_Wheel_Measure  || ESeq == ESeq.GRR_Table_Grinding)
            {
                if (CDataOption.Use2004U) //210902 syc : 2004U
                {
                    if (CIO.It.Get_Y(eY.GRDR_Unit_Vacuum_4U) || CIO.It.Get_Y(eY.GRDR_Carrier_Vacuum_4U) || CData.Parts[(int)EPart.GRDR].bExistStrip == true)  // 2020.12.28 lhs
                    {
                        CSQ_Man.It.bBtnShow = true; //190627 ksg :
                        CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                        return;
                    }
                }
                else
                {
                    if (CIO.It.Get_Y(eY.GRDR_TbVacuum) || CData.Parts[(int)EPart.GRDR].bExistStrip == true)  // 2020.12.28 lhs
                    {
                        CSQ_Man.It.bBtnShow = true; //190627 ksg :
                        CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                        return;
                    }
                }
            }

            // 200730 myk : Manual Grinding 동작 중 Barcode 입력 기능 추가
            if ((ESeq == ESeq.GRR_Grinding || ESeq == ESeq.GRR_Manual_GrdBcr) && CDataOption.IsGrdBcr)
            {
                using (frmTxt mForm = new frmTxt("Input Strip ID"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        CData.Parts[(int)EPart.GRDR].sBcr = mForm.Val;
                    }
                }
            }

            if (ESeq == ESeq.GRR_Grinding && CData.Opt.aTblSkip[(int)EWay.R])
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Error, "Error", "Right Table Skip Now!");
                return;
            }

            if (ESeq == ESeq.GRR_Dressing         || ESeq == ESeq.GRR_Grinding        || ESeq == ESeq.GRR_WaterKnife    ||
                ESeq == ESeq.GRR_Table_Measure    || ESeq == ESeq.GRR_Strip_Measure   || ESeq == ESeq.GRR_Water_Knife   ||
                ESeq == ESeq.GRR_Strip_Measureone || ESeq == ESeq.GRR_Dresser_Measure || ESeq == ESeq.GRR_Wheel_Measure ||
                ESeq == ESeq.GRR_Table_Grinding   || ESeq == ESeq.GRR_Manual_GrdBcr   || ESeq == ESeq.GRR_Manual_Bcr    ||
                ESeq == ESeq.GRL_Dressing         || ESeq == ESeq.GRL_Grinding        || ESeq == ESeq.GRL_WaterKnife    ||
                ESeq == ESeq.GRL_Table_Measure    || ESeq == ESeq.GRL_Strip_Measure   || ESeq == ESeq.GRL_Water_Knife   ||
                ESeq == ESeq.GRL_Strip_Measureone || ESeq == ESeq.GRL_Dresser_Measure || ESeq == ESeq.GRL_Wheel_Measure ||
                ESeq == ESeq.GRL_Table_Grinding   || ESeq == ESeq.GRL_Manual_GrdBcr   || ESeq == ESeq.GRL_Manual_Bcr      ) //200130 ksg : Menual Grd bcr 버튼 추가
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "One More Check Strip Jig. Please") == DialogResult.Cancel)
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    return;
                }

                if (!CData.Opt.bCoverSkip && !CSQ_Main.It.CheckCover())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "Grinding Cover Check Please");
                    return;
                }

                // 2020.12.31 lhs Start
                // Rear Cover가 열여 있으면 다시 한번 사용자 확인
                // 1. 휠노즐 장작된 설비 
                // 2. 뒷커버 열림 확인 
                bool bClosed = CIO.It.Get_X(eX.GRD_CoverRear);
                if (CDataOption.IsWhlCleaner && bClosed == false)
                {
                    string sMsg = "";
                    sMsg = "Grinding Rear-Cover Opened !!! \n\r";
                    sMsg += "\n\r";
                    sMsg += "Please check !!! \n\r";
                    sMsg += "\n\r";
                    sMsg += "Do you want continue?";

                    if (CMsg.Show(eMsg.Query, "Warning", sMsg) == DialogResult.Cancel)
                    {
                        CSQ_Man.It.bBtnShow = true;
                        return;
                    }
                }
                // 2020.12.31 lhs End

                //200201 ksg :
                if ((ESeq == ESeq.GRR_Manual_GrdBcr || ESeq == ESeq.GRL_Manual_GrdBcr || ESeq == ESeq.GRL_Manual_Bcr || ESeq == ESeq.GRR_Manual_Bcr) && 
                    CDataOption.ManualBcr == eManualBcr.Use && !CIO.It.Get_X(eX.GRD_CoverFront))
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "Grinding Cover Check Please");
                    return;
                }
            }

            if (ESeq == ESeq.GRR_Dressing)
            { tabGrR_StripInsp.SelectTab(3); }

            //ksg 추가
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                return;
            }
            else
            {
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                //1. Manual Grinding, Manual Measure Strip 시 18 Point 진행 여부 Query
                if (CDataOption.Use18PointMeasure && (0 < CData.Dev.i18PStripCount))
                {
                    if (ESeq == ESeq.GRR_Manual_GrdBcr || ESeq == ESeq.GRR_Strip_Measure)
                    {
                        if (CMsg.Show(eMsg.Query, "Strip Measure Type", "Is This Test Strip(18 Points) Measure?") == DialogResult.OK) //Query 팝업
                        { CData.R_GRD.m_bManual18PMeasure = true; }
                        else
                        { CData.R_GRD.m_bManual18PMeasure = false; }
                    }
                    else
                    {
                        CData.R_GRD.m_bManual18PMeasure = false;
                    }
                }
                else
                {
                    CData.R_GRD.m_bManual18PMeasure = false;
                }
                //2. 설정된 측정 포인트 수 체크
                if (ESeq == ESeq.GRR_Manual_GrdBcr || ESeq == ESeq.GRR_Strip_Measure)
                {
                    string strResult = CData.R_GRD.CheckMeasurePointCount(true); //스트립 측정 포인트 수 체크 : 매뉴얼 동작에서는 반드시 CData.R_GRD.m_bManual18PMeasure 값 설정 후 호출해야 정확한 측정 포인트 수가 파악됨
                    if (0 < strResult.Length)
                    {
                        CSQ_Man.It.bBtnShow = true;
                        CMsg.Show(eMsg.Error, "Error", strResult); //측정 포인트 수가 SECS/GEM용 배열 사이즈 범위 초과(위험)
                        return;
                    }
                }
                //

                //CData.L_GRD.m_tStat.bDrsR = true;
                CSQ_Man.It.Seq = ESeq;
                //190410 ksg :
                _EnableBtn(false);
                CSQ_Man.It.bBtnShow = false;

                //200708 pjh : Manual Grinding 시 Grinding 중 Error Check 변수 초기화
                if (ESeq == ESeq.GRL_Grinding)
                {
                    CData.Parts[(int)EPart.GRDL].bChkGrd = false;
                }
                else if (ESeq == ESeq.GRR_Grinding)
                {
                    CData.Parts[(int)EPart.GRDR].bChkGrd = false;
                }
                //
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            //ksg : 스톱 함수가 여긴가?
            CSQ_Man.It.bBtnShow = true;
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CSQ_Man.It.bStop = true;
            }
        }

        /// <summary>
        /// 그라인드 좌, 우 테이블 조작 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbGrd_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            string sTxt;
            
            switch (iTag)
            {
                case 0:    // Vacuum
                    if (mCkb.Checked) 
                    sTxt = CLang.It.GetLanguage("TABLE") + "\r\n" + CLang.It.GetLanguage("VACUUM") + "\r\n" + CLang.It.GetLanguage("ON");
                    else 
                    sTxt = CLang.It.GetLanguage("TABLE") + "\r\n" + CLang.It.GetLanguage("VACUUM") + "\r\n" + CLang.It.GetLanguage("OFF");
                    
                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 1:    // Water
                    if (mCkb.Checked) 
                    sTxt = CLang.It.GetLanguage("TABLE") + "\r\n" + CLang.It.GetLanguage("WATER") + "\r\n" + CLang.It.GetLanguage("ON");
                    else 
                    sTxt = CLang.It.GetLanguage("TABLE") + "\r\n" + CLang.It.GetLanguage("WATER") + "\r\n" + CLang.It.GetLanguage("OFF");
                    
                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 2:    // Eject
                    if (mCkb.Checked) 
                    sTxt = CLang.It.GetLanguage("TABLE") + "\r\n" + CLang.It.GetLanguage("EJECTOR") + "\r\n" + CLang.It.GetLanguage("ON");
                    else 
                    sTxt = CLang.It.GetLanguage("TABLE") + "\r\n" + CLang.It.GetLanguage("EJECTOR") + "\r\n" + CLang.It.GetLanguage("OFF");
                   
                     SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 3:    // Top Cleaner Down
                    if (mCkb.Checked) 
                    sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("DOWN");
                    else 
                    sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("UP");
                    
                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 4:    // Top Cleaner Water Knife
                    if (CDataOption.UseNewClenaer) //syc : new cleaner
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("POINT WATER") + "\r\n" + CLang.It.GetLanguage("ON");
                        else
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("POINT WATER") + "\r\n" + CLang.It.GetLanguage("OFF");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }
                    else
                    {
                        if (mCkb.Checked)
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("WATER KNIFE") + "\r\n" + CLang.It.GetLanguage("ON");
                        else
                            sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("WATER KNIFE") + "\r\n" + CLang.It.GetLanguage("OFF");

                        SetWriteLanguage(mCkb.Name, sTxt);
                        break;
                    }

                case 5:    // Top Cleaner Air Knife
                    if (mCkb.Checked) 
                    sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("AIR KNIFE") + "\r\n" + CLang.It.GetLanguage("ON");
                    else 
                    sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("AIR KNIFE") + "\r\n" + CLang.It.GetLanguage("OFF");
                   
                     SetWriteLanguage(mCkb.Name, sTxt);
                    break;

                case 6:    // Top Cleaner Water
                    if (mCkb.Checked) 
                    sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("WATER") + CLang.It.GetLanguage("ON");
                    else 
                    sTxt = CLang.It.GetLanguage("TOP") + "\r\n" + CLang.It.GetLanguage("CLEANER") + "\r\n" + CLang.It.GetLanguage("WATER") + CLang.It.GetLanguage("OFF");
                    
                    SetWriteLanguage(mCkb.Name, sTxt);
                    break;
            }
        }

        private void ckb_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            //190430 ksg : Auto & Manual 때 동작 안함
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            CheckBox mCkb = sender as CheckBox;

            int iType = int.Parse(mCkb.Tag.ToString());
            _SetLog("Click, Tag:" + iType);

            // 2020.10.22 JSKim St
            bool bPumpRunResult = false;
            // 2020.10.22 JSKim Ed

            eX eIPRun    = eX.PUMPR_Run;
            // 2020.10.22 JSKim St
            eX eIAddPRun = eX.ADD_PUMPR_Run; 
            // 2020.10.22 JSKim Ed
            eY eOPRun    = eY.PUMPR_Run;

            switch (iType)
            {
                case 0:    // Vacuum
                    if (mCkb.Checked)
                    {
                        //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                        CData.R_GRD.ActWater(false); //Table Water OFF
                        CData.R_GRD.ActEject(false); //Table Eject OFF
                        //

                        // 2020.10.22 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                        {
                            bPumpRunResult = CIO.It.Get_X(eIPRun) == false || CIO.It.Get_X(eIAddPRun) == false;
                        }
                        else
                        {
                            bPumpRunResult = CIO.It.Get_X(eIPRun) == false;
                        }
                        // 2020.10.22 JSKim Ed

                        // 2020.10.22 JSKim St
                        //if (!CIO.It.Get_X(eIPRun))
                        if (bPumpRunResult)
                        // 2020.10.22 JSKim Ed
                        {
                            CIO.It.Set_Y(eOPRun, true);

                            CTim mTOut = new CTim();
                            mTOut.Set_Delay(10000);
                            while (true)
                            {
                                if (CIO.It.Get_X(eIPRun))
                                {
                                    break;
                                }
                                if (mTOut.Chk_Delay())
                                {
                                    CErr.Show(eErr.PUMP_RIGHT_RUN_TIMEOUT); return;
                                }
                                Application.DoEvents();
                            }

                            // 2020.11.07 lhs Start
                            if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                            {
                                while (true)
                                {
                                    if (CIO.It.Get_X(eIAddPRun))
                                    {
                                        break;
                                    }
                                    if (mTOut.Chk_Delay())
                                    {
                                        CErr.Show(eErr.ADD_PUMP_RIGHT_RUN_TIMEOUT_ERROR); return;
                                    }
                                    Application.DoEvents();
                                }
                            }
                            // 2020.11.07 lhs End
                        }
                        //210902 syc : 2004U
                        if (CDataOption.Use2004U)
                        {
                            CIO.It.Set_Y(eY.GRDR_Unit_Vacuum_4U, true);
                            CIO.It.Set_Y(eY.GRDR_Carrier_Vacuum_4U, true);
                        }
                        else
                        {
                            CIO.It.Set_Y(eY.GRDR_TbVacuum, true);
                        }

                        if (CDataOption.Package == ePkg.Unit)
                        {
                            GU.Delay(3000);
                            GU.Delay(1000); // 오른쪽이 관이 길어 시간이 더 필요

                            string sUnit = "";
                            CIO.It.Set_Y(eY.GRDR_CarrierVacuum, true);
                            GU.Delay(GV.UNIT_VAC_DELAY);
                            if (!CIO.It.Get_X(eX.GRDR_TbVaccum))
                            {
                                CErr.Show(eErr.RIGHT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR);
                                return;
                            }

                            if (CData.Dev.iUnitCnt == 4)
                            {
                                // Unit #1
                                CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDR_Unit_1_Vacuum))
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[0] = true;
                                    sUnit += "Unit #1  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[0] = false;
                                    CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, false);
                                }

                                // Unit #2
                                CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDR_Unit_2_Vacuum))
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[1] = true;
                                    sUnit += "Unit #2  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[1] = false;
                                    CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, false);
                                }

                                // Unit #3
                                CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDR_Unit_3_Vacuum))
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[2] = true;
                                    sUnit += "Unit #3  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[2] = false;
                                    CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, false);
                                }

                                // Unit #4
                                CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDR_Unit_4_Vacuum))
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[3] = true;
                                    sUnit += "Unit #4  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[3] = false;
                                    CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, false);
                                }
                            }
                            else
                            {
                                // Unit #1
                                CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, true);
                                CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDR_Unit_1_Vacuum) && CIO.It.Get_X(eX.GRDR_Unit_2_Vacuum))
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[0] = true;
                                    sUnit += "Unit #1  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[0] = false;
                                    CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, false);
                                    CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, false);
                                }

                                // Unit #2
                                CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, true);
                                CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDR_Unit_3_Vacuum) && CIO.It.Get_X(eX.GRDR_Unit_4_Vacuum))
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[1] = true;
                                    sUnit += "Unit #2  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.R].aUnitEx[1] = false;
                                    CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, false);
                                    CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, false);
                                }
                            }

                            CMsg.Show(eMsg.Notice, "Notice", "Vacuum success\r\n" + sUnit);
                        }
                    }
                    else
                    {
                        if (CDataOption.Use2004U)
                        {
                            CIO.It.Set_Y(eY.GRDR_Unit_Vacuum_4U, false);
                            CIO.It.Set_Y(eY.GRDR_Carrier_Vacuum_4U, false);
                        }
                        CIO.It.Set_Y(eY.GRDR_TbVacuum, false);
                        CIO.It.Set_Y(eY.GRDR_CarrierVacuum, false);
                        CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, false);
                        CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, false);
                        CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, false);
                        CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, false);
                    }

                    break;

                case 1:    // Water
                    if (mCkb.Checked)
                    {
#if true //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                        CData.R_GRD.ActEject(false);  //Table Eject OFF
                        CData.R_GRD.ActVacuum(false); //Table Vacuum OFF
#else
                        if (CIO.It.Get_Y(eY.PUMPR_Run))
                        { CIO.It.Set_Y(eY.PUMPR_Run, false); }
#endif
                    }
                    CIO.It.Set_Y(eY.GRDR_TbFlow, mCkb.Checked);
                    if (CDataOption.Package == ePkg.Unit)
                    {
                        CIO.It.Set_Y(eY.GRDR_CarrierVacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, mCkb.Checked);
                    }

                    break;

                case 2:    // Eject
                    if (mCkb.Checked)
                    {
#if true //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                        CData.R_GRD.ActWater(false);  //Table Water OFF
                        CData.R_GRD.ActVacuum(false); //Table Vacuum OFF
#else
                        if (CIO.It.Get_Y(eY.PUMPR_Run))
                        { CIO.It.Set_Y(eY.PUMPR_Run, false); }
#endif
                    }
                    CIO.It.Set_Y(eY.GRDR_TbEjector, mCkb.Checked);
                    if (CDataOption.Package == ePkg.Unit)
                    {
                        CIO.It.Set_Y(eY.GRDR_CarrierVacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_1_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_2_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_3_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDR_Unit_4_Vacuum, mCkb.Checked);
                    }

                    break;

                case 3:    // Top Cleaner Down
                    CIO.It.Set_Y(eY.GRDR_TopClnDn, mCkb.Checked);
                    break;

                case 4:    // Top Cleaner Water Knife
                    CIO.It.Set_Y(eY.GRDR_TopWaterKnife, mCkb.Checked);
                    break;

                case 5:    // Top Cleaner Air Knife
                    CIO.It.Set_Y(eY.GRDR_TopClnAir, mCkb.Checked);
                    break;

                case 6:    // Top Cleaner Water
                    CIO.It.Set_Y(eY.GRDR_TopClnFlow, mCkb.Checked);
                    break;
            }
        }

        /// <summary>
        /// Label text On/Off 변경 이벤트 [A접점]
        /// 190325-maeng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblIO_A_BackColorChanged(object sender, EventArgs e)
        {
            Label mLbl = sender as Label;
            string sVal = mLbl.Tag.ToString().Replace("$", "\r\n");
            string text;
            if (mLbl.BackColor == Color.Lime)
            {
                //mLbl.Text = sVal + " On";
                text = sVal + " " + CLang.It.GetLanguage("On");
                SetWriteLanguage(mLbl.Name, text);
            }
            else
            {
                //mLbl.Text = sVal + " Off";
                text = sVal + " " + CLang.It.GetLanguage("Off");
                SetWriteLanguage(mLbl.Name, text);
            }
        }

        /// <summary>
        /// Label text On/Off 변경 이벤트 [B접점]
        /// 190409-maeng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblIO_B_BackColorChanged(object sender, EventArgs e)
        {
            Label mLbl = sender as Label;
            string sVal = mLbl.Tag.ToString().Replace("$", "\r\n");
            string text;
            if (mLbl.BackColor == Color.Lime)
            {
                //mLbl.Text = sVal + " Off";
                text = sVal + " " + CLang.It.GetLanguage("Off");
                SetWriteLanguage(mLbl.Name, text);
            }
            else
            {
                //mLbl.Text = sVal + " On";
                text = sVal + " " + CLang.It.GetLanguage("On");
                SetWriteLanguage(mLbl.Name, text);
            }
        }
    }
}
